Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Diagnostics
Imports System.Text
Imports System.Data.SqlClient
Imports MasterPages
Imports Utility
Imports System.Configuration.ConfigurationManager
Imports DataLayer
Imports System.Linq

Namespace Components

	Public Class SitePage
		Inherits BasePage

		Protected GlobalRefererName As String
        Protected GlobalSecureName As String

		Public Property MetaRobots() As String
			Get
				Dim mp As MasterPages.MasterPage = Me.Page.FindControl("CTMain")
				If Not mp Is Nothing Then Return mp.MetaRobots Else Return String.Empty
			End Get

			Set(ByVal Value As String)
				Dim mp As MasterPages.MasterPage = Me.Page.FindControl("CTMain")
				If Not mp Is Nothing Then mp.MetaRobots = Value
			End Set
		End Property

        Protected Overrides Sub OnInit(ByVal e As EventArgs)
            MyBase.OnInit(e)

            If Session("ReferralURL") Is Nothing Then
                Session("ReferralURL") = Request.ServerVariables("HTTP_REFERER")
            End If

            If Session("ref") Is Nothing AndAlso Request("ref") <> String.Empty Then
                Session("ref") = Request("ref")
                DataLayer.ReferralRow.AddClick(DB, Request("ref"), Request.ServerVariables("REMOTE_ADDR"))
            End If

            If AppSettings("SiteInMaintenanceMode") Then
                Response.Redirect("/maintenance.aspx")
            End If

            GlobalRefererName = ConfigurationManager.AppSettings("GlobalRefererName")
            GlobalSecureName = ConfigurationManager.AppSettings("GlobalSecureName")
        End Sub

		Protected Sub SitePageLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim Principal As SitePrincipal = Nothing

            CheckBrowserCompatibility()

            'Load MemberId from cookies
            If Session("MemberId") Is Nothing Then
                If Not CookieUtil.GetTripleDESEncryptedCookieValue("MemberId") = Nothing Then
                    Session("MemberId") = CookieUtil.GetTripleDESEncryptedCookieValue("MemberId")
                End If
                If Not IsNumeric(Session("MemberId")) Then
                    Session("MemberId") = Nothing
                    CookieUtil.SetTripleDESEncryptedCookie("MemberId", Nothing)
                End If
            End If

            'Load OrderId from cookies
            If Session("OrderId") Is Nothing Then
                If Not CookieUtil.GetTripleDESEncryptedCookieValue("OrderId") = Nothing Then
                    Session("OrderId") = CookieUtil.GetTripleDESEncryptedCookieValue("OrderId")
                End If
                If Not IsNumeric(Session("OrderId")) Then
                    Session("OrderId") = Nothing
                    CookieUtil.SetTripleDESEncryptedCookie("OrderId", Nothing)
                End If
            End If
            If Not Session("OrderId") Is Nothing Then
                Dim id As Integer = DB.ExecuteScalar("SELECT TOP 1 OrderId FROM StoreOrder WHERE OrderId = " & Session("OrderId") & " AND ProcessDate IS NULL")
                If id = 0 Then
                    Session("OrderId") = Nothing
                    CookieUtil.SetTripleDESEncryptedCookie("OrderId", Nothing)
                End If
            End If

            If Not Session("MemberId") Is Nothing Then
                Try
                    Principal = New SitePrincipal(DB, CInt(Session("MemberId")))
                Catch ex As NullReferenceException
                    Response.Redirect("/members/logout.aspx")
                End Try
                If Principal Is Nothing Then Exit Sub
            End If
        End Sub

        Protected Function IsLoggedIn() As Boolean
            If Session("MemberId") Is Nothing Then
                Return False
            Else
                Return True
            End If
        End Function

        Protected Function CheckAccess(ByVal Redir As String) As Boolean
            If Not IsLoggedIn() Then
                Session("Redirect") = Redir
                Response.Redirect(AppSettings("GlobalSecureName") & "/members/login.aspx")
            End If
        End Function

        Protected Sub EnsureMemberAccess()
            CheckAccess("/members/login.aspx")
        End Sub

        Protected Sub EnsureMemberOrderHistoryAccess()
            CheckAccess("/members/view-order.aspx")
        End Sub

        Public Function IsLoggedInBuilder() As Boolean

            If Session("BuilderId") Is Nothing Then
                Try
                    Session("BuilderId") = IIf(Not Utility.CookieUtil.GetTripleDESEncryptedCookieValue("cBuilderId") = "", Utility.CookieUtil.GetTripleDESEncryptedCookieValue("cBuilderId"), Nothing)
                    Session("BuilderAccountId") = IIf(Not Utility.CookieUtil.GetTripleDESEncryptedCookieValue("cBuilderAccountId") = "", Utility.CookieUtil.GetTripleDESEncryptedCookieValue("cBuilderAccountId"), Nothing)
                Catch ex As Exception
                End Try
            End If

            If Session("BuilderId") Is Nothing Or Session("BuilderAccountId") Is Nothing Then
                Return False
            Else
                If DateDiff(DateInterval.Hour, Session("LastLogin"), Now()) > 2 Then
                    Dim b As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
                    Dim dbBuilderAccount As BuilderAccountRow = BuilderAccountRow.GetRow(DB, Session("BuilderAccountId"))
                    If dbBuilderAccount.BuilderAccountID = Nothing Then
                        Return False
                    End If

                    Dim p As New VindiciaPaymentProcessor(DB)
                    p.IsTestMode = DataLayer.SysParam.GetValue(DB, "TestMode")

                    Dim regDate As DateTime
                    Dim UseEntitlements As Boolean = SysParam.GetValue(DB, "UseEntitlements")
                    Dim SubStatusConfirmed As Boolean = HttpContext.Current.Session("SubStatusConfirmed")

                    If (Session("SkipRegistrationCheck")) Or (SubStatusConfirmed OrElse p.CheckSubscriptionStatus(b, regDate)) Then
                        'If Session("SkipRegistrationCheck") Or SubStatusConfirmed OrElse (UseEntitlements OrElse regDate.Year = Now.Year) Then
                        'Per Brian: remove new year registration check.
                        If Session("SkipRegistrationCheck") Or SubStatusConfirmed OrElse UseEntitlements Then
                            HttpContext.Current.Session("SubStatusConfirmed") = True
                            If dbBuilderAccount.RequirePasswordUpdate Or Not dbBuilderAccount.PasswordEncrypted Then
                                'Response.Redirect(GlobalSecureName & "/forms/builder-registration/account.aspx")
                                Response.Redirect(GlobalSecureName & "/changepw.aspx")
                            Else
                                Session("LastLogin") = Now()
                                Return True
                            End If
                        Else
                            Response.Redirect(GlobalSecureName & "/forms/builder-registration/default.aspx")
                        End If
                    Else
                        If UseEntitlements Then
                            If b.SkipEntitlementCheck Then
                                Dim dbRegistration As BuilderRegistrationRow = BuilderRegistrationRow.GetRowByBuilder(DB, b.BuilderID)
                                If dbRegistration.BuilderRegistrationID = Nothing Then
                                    Response.Redirect("/forms/builder-registration/default.aspx")
                                Else
                                    Session("LastLogin") = Now()
                                    Return True
                                End If
                            End If
                        End If
                        'Response.Redirect("/builder/update.aspx")
                        If regDate.Year = Now.Year And dbBuilderAccount.PasswordEncrypted Then
                            Response.Redirect(GlobalSecureName & "/forms/builder-registration/payment.aspx")
                        Else
			     '---------------------------------------------------------------------------------------------------------
                            '----------------------- ALLOW ACCESS TO SYSTEM IF BILLING START DATE IS IN FUTURE -----------------------
                            Dim dbBBP As BuilderBillingPlanRow = BuilderBillingPlanRow.GetRow(DB, CInt(Session("BuilderId")))

                            If dbBBP.SubscriptionStartDate > Now.Date Then
                                Session("SkipRegistrationCheck") = True
                                Response.Redirect("/builder/")
                            Else
                                Response.Redirect("/forms/builder-registration/default.aspx")
                            End If
                            '---------------------------------------------------------------------------------------------------------
                        End If
                    End If
                Else
                    Session("LastLogin") = Now()
                    Return True
                End If
                Return False
            End If
        End Function

        Protected Function CheckBuilderAccess(ByVal Redir As String) As Boolean
            If Not IsLoggedInBuilder() Then
                Session("Redirect") = Redir
                Response.Redirect(AppSettings("GlobalSecureName") & "/default.aspx")
            End If
        End Function

        Protected Sub EnsureBuilderAccess()
            CheckBuilderAccess("/default.aspx")
        End Sub

        Private ReadOnly Property RegistrationStatuses() As RegistrationStatusCollection
            Get
                Dim d As RegistrationStatusCollection = HttpContext.Current.Cache("RegistrationStatuses")
                If d Is Nothing Then
                    d = RegistrationStatusRow.GetStatuses(DB)
                    HttpContext.Current.Cache.Add("RegistrationStatuses", d, Nothing, DateAdd(DateInterval.Minute, 15, Now), Nothing, CacheItemPriority.Default, Nothing)
                End If
                Return d
            End Get
        End Property

        Public Function IsLoggedInVendor() As Boolean

            If Session("VendorId") Is Nothing Then
                Try
                    Session("VendorId") = IIf(Not Utility.CookieUtil.GetTripleDESEncryptedCookieValue("cVendorId") = "", Utility.CookieUtil.GetTripleDESEncryptedCookieValue("cVendorId"), Nothing)
                    Session("VendorAccountId") = IIf(Not Utility.CookieUtil.GetTripleDESEncryptedCookieValue("cVendorAccountId") = "", Utility.CookieUtil.GetTripleDESEncryptedCookieValue("cVendorAccountId"), Nothing)
                Catch ex As Exception
                End Try
            End If

            If Session("VendorId") Is Nothing Or Session("VendorAccountId") Is Nothing Then
                Return False
            Else
                'Dim dbRegistration As VendorRegistrationRow = VendorRegistrationRow.GetRowByVendor(DB, Session("VendorId"), Now.Year)
                'Per Brian: remove new year registration check. 01/04/2010
                Dim dbRegistration As VendorRegistrationRow = VendorRegistrationRow.GetRowByVendor(DB, Session("VendorId"))
                'Dim status As RegistrationStatusRow = (From s As RegistrationStatusRow In RegistrationStatuses Where s.RegistrationStatusID = dbRegistration.RegistrationStatusID Select s).FirstOrDefault
                Dim dbAccount As VendorAccountRow = VendorAccountRow.GetRow(DB, Session("VendorAccountId"))
                'If Session("SkipRegistrationCheck") Or dbRegistration IsNot Nothing AndAlso dbRegistration.CompleteDate.Year = Now.Year Then
                'Per Brian: remove new year registration check. 01/04/2010
                If Session("SkipRegistrationCheck") Or dbRegistration IsNot Nothing Then
                    If dbAccount.RequirePasswordUpdate Then
                        Response.Redirect(GlobalSecureName & "/changepw.aspx")
                    Else
                        Return True
                    End If
                Else
                    Response.Redirect(GlobalSecureName & "/forms/vendor-registration/default.aspx")
                End If
            End If
        End Function

        Protected Function CheckVendorAccess(ByVal Redir As String) As Boolean
            If Not IsLoggedInVendor() Then
                Session("Redirect") = Redir
                Response.Redirect(AppSettings("GlobalSecureName") & "/default.aspx")
            End If
        End Function

        Protected Sub EnsureVendorAccess()
            CheckVendorAccess("/default.aspx")
        End Sub

        Public Function IsLoggedInPIQ() As Boolean
            If Session("PIQId") Is Nothing Then
                Try
                    Session("PIQId") = IIf(Not Utility.CookieUtil.GetTripleDESEncryptedCookieValue("cPIQId") = "", Utility.CookieUtil.GetTripleDESEncryptedCookieValue("cPIQId"), Nothing)
                    Session("PIQAccountId") = IIf(Not Utility.CookieUtil.GetTripleDESEncryptedCookieValue("cPIQAccountId") = "", Utility.CookieUtil.GetTripleDESEncryptedCookieValue("cPIQAccountId"), Nothing)
                Catch ex As Exception
                End Try
            End If
            If Session("PIQId") Is Nothing Or Session("PIQAccountId") Is Nothing Then
                Return False
            Else
                Dim dbAccount As PIQAccountRow = PIQAccountRow.GetRow(DB, Session("PIQAccountId"))
                If dbAccount.RequirePasswordUpdate Then
                    Response.Redirect(GlobalSecureName & "/changepw.aspx")
                Else
                    Return True
                End If
            End If
        End Function

        Protected Function CheckPIQAccess(ByVal Redir As String) As Boolean
            If Not IsLoggedInPIQ() Then
                Session("Redirect") = Redir
                Response.Redirect(AppSettings("GlobalSecureName") & "/default.aspx")
            End If
        End Function

        Protected Sub EnsurePIQAccess()
            CheckPIQAccess("/default.aspx")
        End Sub

        Public Property PageTitle() As String
            Get
                Dim mp As MasterPages.MasterPage = Me.Page.FindControl("CTMain")
                If Not mp Is Nothing Then Return mp.PageTitle Else Return String.Empty
            End Get

            Set(ByVal Value As String)
                Dim mp As MasterPages.MasterPage = Me.Page.FindControl("CTMain")
                If Not mp Is Nothing Then mp.PageTitle = Value
            End Set
        End Property

        Public Property MetaKeywords() As String
            Get
                Dim mp As MasterPages.MasterPage = Me.Page.FindControl("CTMain")
                If Not mp Is Nothing Then Return mp.MetaKeywords Else Return String.Empty
            End Get

            Set(ByVal Value As String)
                Dim mp As MasterPages.MasterPage = Me.Page.FindControl("CTMain")
                If Not mp Is Nothing Then mp.MetaKeywords = Value
            End Set
        End Property

        Public Property MetaDescription() As String
            Get
                Dim mp As MasterPages.MasterPage = Me.Page.FindControl("CTMain")
                If Not mp Is Nothing Then Return mp.MetaDescription Else Return String.Empty
            End Get

            Set(ByVal Value As String)
                Dim mp As MasterPages.MasterPage = Me.Page.FindControl("CTMain")
                If Not mp Is Nothing Then mp.MetaDescription = Value
            End Set
        End Property

        Public WriteOnly Property SmartBugUrl() As String
            Set(ByVal value As String)
                Dim mp As MasterPages.MasterPage = Me.Page.FindControl("CTMain")
                If Not mp Is Nothing Then
                    Dim bug As ISmartBug = mp.FindControl("bug")
                    If Not bug Is Nothing Then
                        bug.URL = value
                    End If
                End If
            End Set
        End Property

        Private Sub CheckBrowserCompatibility()
            If Not Request("Print") = String.Empty Then
                Exit Sub
            End If
            If Session("BrowserCompatible") Is Nothing Then
                Session("BrowserCompatible") = True
                'Dim browser As HttpBrowserCapabilities = Request.Browser
                'If browser.Browser = "IE" AndAlso browser.Version < 5.5 OrElse browser.Browser = "Netscape" AndAlso browser.Version < 7 OrElse browser.Browser = "Safari" AndAlso browser.Version < 1.3 OrElse browser.Browser = "Opera" AndAlso browser.Version < 8 Then
                '    Session("BrowserCompatible") = False
                'Else
                '    Session("BrowserCompatible") = True
                'End If
            End If
            If Not Session("BrowserCompatible") And Not LCase(Request.ServerVariables("URL")) = "/browser.aspx" Then
                Response.Redirect("/browser.aspx")
            End If
        End Sub
    End Class

End Namespace
