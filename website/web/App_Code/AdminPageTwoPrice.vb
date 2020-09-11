Imports Microsoft.VisualBasic
Imports Components
Imports DataLayer
Imports TwoPrice.DataLayer

Namespace Components
    Public Class AdminPageTwoPrice
        Inherits AdminPage

        Protected GlobalSecureName As String
        Protected GlobalRefererName As String

        Protected Overrides Sub OnInit(e As System.EventArgs)
            MyBase.OnInit(e)

            GlobalRefererName = ConfigurationManager.AppSettings("GlobalRefererName")
            GlobalSecureName = ConfigurationManager.AppSettings("GlobalSecureName")
        End Sub
        'Copied from SitePage
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
                        Response.Redirect("/builder/update.aspx")
                        'If regDate.Year = Now.Year And dbBuilderAccount.PasswordEncrypted Then
                        '    Response.Redirect(GlobalSecureName & "/forms/builder-registration/payment.aspx")
                        'Else
                        '    Response.Redirect(GlobalSecureName & "/forms/builder-registration/default.aspx")
                        '    'Response.Redirect("/changepw.aspx")
                        'End If
                    End If
                Else
                    Session("LastLogin") = Now()
                    Return True
                End If
                Return False
            End If
        End Function

    End Class
End Namespace

