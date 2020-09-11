Imports Components
Imports DataLayer
Imports System.Data
Imports System.IO
Imports System.Security.Cryptography.X509Certificates

Partial Class _default
    Inherits SitePage
    Private PageUrl As String = ""
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private HashString As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Temporary code for showing splash page before go-live 
        '-------------------------------------------------------
        'If Not Request.QueryString("checkkey") Is Nothing Then
        '    Dim strKey As String = Trim(Request.QueryString("checkkey"))

        '    If strKey <> "5F748EFD-99F3-4AAF-B3F6-1761C9CAA659" Then
        '        Response.Redirect("app_offline.html")
        '    End If
        'Else
        '    Response.Redirect("app_offline.html")
        'End If
        '-------------------------------------------------------

        EnsureSSL()

        If IsLoggedInBuilder() Then
            ' Response.Redirect(GlobalRefererName & "/builder/")
            If Not String.IsNullOrEmpty(Request.QueryString("mod")) AndAlso Request.QueryString("mod") = "tpc" Then
                If Not String.IsNullOrEmpty(Request.QueryString("Tcam")) AndAlso Not String.IsNullOrEmpty(Request.QueryString("Opt")) Then
                    Response.Redirect(GlobalRefererName & "/builder/CpEvent/DataEntry.aspx?Tcam=" & Request.QueryString("Tcam") & "&Opt=" & Request.QueryString("Opt"))
                Else
                    Response.Redirect(GlobalRefererName & "/builder/")
                End If

            Else
                Response.Redirect(GlobalRefererName & "/builder/")
            End If

        ElseIf IsLoggedInVendor() Then
            If Not String.IsNullOrEmpty(Request.QueryString("mod")) AndAlso Request.QueryString("mod") = "inv" Then
                Response.Redirect(GlobalRefererName & "/rebates/RebateStatements-Vendor.aspx")
            Else
                Response.Redirect(GlobalRefererName & "/vendor/")
            End If
        ElseIf IsLoggedInPIQ() Then
            Response.Redirect(GlobalRefererName & "/piq/")
        End If
    End Sub

    'For testing only -- allows invalid certificates
    Private Shared Function SecurityCallback(ByVal sender As Object, ByVal cert As X509Certificate, ByVal chain As X509Chain, ByVal e As System.Net.Security.SslPolicyErrors) As Boolean
        Return True
    End Function

    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogin.Click
        If Not IsValid Then
            Exit Sub
        End If

        'TESTING - disables SSL validation
        If SysParam.GetValue(DB, "TestMode") Then
            System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf SecurityCallback
        End If

        Dim sha As System.Security.Cryptography.SHA1 = System.Security.Cryptography.SHA1.Create
        Dim pwBytes As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes("change-me--" & txtPassword.Text & "--")
        Dim hashBytes As Byte() = sha.ComputeHash(pwBytes)
        'HashString = System.Text.ASCIIEncoding.ASCII.GetString(hashBytes)
        HashString = String.Empty
        For Each b As Byte In hashBytes
            Dim token As String = b.ToString("x")
            If token.Length = 1 Then
                token = "0" & token
            End If
            HashString &= token
        Next

        If LogInBuilder() Then
            If chkPersist.Checked Then
                Utility.CookieUtil.SetTripleDESEncryptedCookie("cBuilderId", Session("BuilderId"), Now.AddDays(360))
                Utility.CookieUtil.SetTripleDESEncryptedCookie("cBuilderAccountId", Session("BuilderAccountId"), Now.AddDays(360))
            Else
                Utility.CookieUtil.SetTripleDESEncryptedCookie("cBuilderId", "", Now.AddDays(-1))
                Utility.CookieUtil.SetTripleDESEncryptedCookie("cBuilderAccountId", "", Now.AddDays(-1))
            End If
            'If Not Session("Redirect") Is Nothing Then
            '    Response.Redirect(GlobalRefererName & Session("Redirect"))
            'Else
            '    Response.Redirect(GlobalRefererName & "/builder/")
            'End If
            If Not String.IsNullOrEmpty(Request.QueryString("mod")) AndAlso Request.QueryString("mod") = "tpc" Then
                If Not String.IsNullOrEmpty(Request.QueryString("Tcam")) AndAlso Not String.IsNullOrEmpty(Request.QueryString("Opt")) Then
                    Response.Redirect(GlobalRefererName & "/builder/CpEvent/DataEntry.aspx?Tcam=" & Request.QueryString("Tcam") & "&Opt=" & Request.QueryString("Opt"))
                Else
                    Response.Redirect(GlobalRefererName & "/builder/")
                End If
            ElseIf Not Session("Redirect") Is Nothing Then
                Response.Redirect(GlobalRefererName & Session("Redirect"))
            Else
                Response.Redirect(GlobalRefererName & "/builder/")
            End If

        ElseIf LogInVendor() Then
            If chkPersist.Checked Then
                Utility.CookieUtil.SetTripleDESEncryptedCookie("cVendorId", Session("VendorId"), Now.AddDays(360))
                Utility.CookieUtil.SetTripleDESEncryptedCookie("cVendorAccountId", Session("VendorAccountId"), Now.AddDays(360))
            Else
                Utility.CookieUtil.SetTripleDESEncryptedCookie("cVendorId", "", Now.AddDays(-1))
                Utility.CookieUtil.SetTripleDESEncryptedCookie("cVendorAccountId", "", Now.AddDays(-1))
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("mod")) AndAlso Request.QueryString("mod") = "inv" Then
                Response.Redirect(GlobalRefererName & "/rebates/RebateStatements-Vendor.aspx")
            Else
                Response.Redirect(GlobalRefererName & "/vendor/")
            End If


        ElseIf LogInPIQ() Then
            If chkPersist.Checked Then
                Utility.CookieUtil.SetTripleDESEncryptedCookie("cPIQId", Session("PIQId"), Now.AddDays(360))
                Utility.CookieUtil.SetTripleDESEncryptedCookie("cPIQAccountId", Session("PIQAccountId"), Now.AddDays(360))
            Else
                Utility.CookieUtil.SetTripleDESEncryptedCookie("cPIQId", "", Now.AddDays(-1))
                Utility.CookieUtil.SetTripleDESEncryptedCookie("cPIQAccountId", "", Now.AddDays(-1))
            End If
            If Not Session("Redirect") Is Nothing Then
                Response.Redirect(GlobalRefererName & Session("Redirect"))
            Else
                Response.Redirect(GlobalRefererName & "/piq/")
            End If
        End If
    End Sub

    Private Function LogInVendor() As Boolean
        Dim dbVendorAccount As VendorAccountRow = VendorAccountRow.GetVendorByUsername(DB, txtUsername.Text)
        Dim bVendorIsActive As Boolean = False
        If dbVendorAccount.VendorAccountID <> Nothing Then
            bVendorIsActive = DB.ExecuteScalar("select IsActive from Vendor where VendorId = " & DB.Number(dbVendorAccount.VendorID))
        End If

        If dbVendorAccount.VendorAccountID = Nothing OrElse (dbVendorAccount.HistoricPasswordSha1 <> HashString And dbVendorAccount.Password <> txtPassword.Text) Then
            AddError("Invalid Username or Password.  Please check and try again.")
            Return False
        ElseIf dbVendorAccount.IsActive = False Then
            AddError("Your account is inactive.  Please contact an administrator for further details.")
            Return False
        ElseIf bVendorIsActive = False Then
            AddError("Your account is linked to an inactive Vendor.  Please contact an adminstrator for further details.")
            Return False
        Else
            Session("VendorAccountId") = dbVendorAccount.VendorAccountID
            Session("VendorId") = dbVendorAccount.VendorID

            Dim dbVendor As VendorRow = VendorRow.GetRow(DB, dbVendorAccount.VendorID)
            'Log Vendor Login Activity
            Session("Username") = dbVendorAccount.Username
            PageUrl = Request.Url.ToString()
            CurrentUserId = Session("VendorAccountID")
            UserName = Session("Username")
            Core.DataLog("Login", PageUrl, CurrentUserId, "Vendor Login", "", "", "", "", UserName)
            ' End Log 

            Dim dbRegistration As VendorRegistrationRow = VendorRegistrationRow.GetRowByVendor(DB, dbVendor.VendorID)
            Dim dbStatus As RegistrationStatusRow = RegistrationStatusRow.GetRow(DB, dbRegistration.RegistrationStatusID)
            If dbRegistration.VendorRegistrationID = Nothing Or dbStatus.RegistrationStatusID = Nothing OrElse Not dbStatus.IsComplete Then
                Response.Redirect(GlobalSecureName & "/forms/vendor-registration/default.aspx")
            ElseIf dbVendorAccount.Password = Nothing OrElse Not dbVendorAccount.PasswordEncrypted Or dbVendorAccount.RequirePasswordUpdate Then
                Response.Redirect("/changepw.aspx")
            Else
                Return True
            End If

        End If
    End Function


    Private Function LogInBuilder() As Boolean

        Dim dbBuilderAccount As BuilderAccountRow = Nothing
        dbBuilderAccount = BuilderAccountRow.GetAccountByUsername(DB, txtUsername.Text)
        Dim bBuilderIsActive As Boolean = False

        If dbBuilderAccount.BuilderAccountID <> Nothing Then
            bBuilderIsActive = DB.ExecuteScalar("select IsActive from Builder where BuilderID = " & DB.Number(dbBuilderAccount.BuilderID))
        End If


        If dbBuilderAccount.BuilderAccountID = Nothing OrElse (dbBuilderAccount.HistoricPasswordSha1 <> HashString And dbBuilderAccount.Password <> txtPassword.Text) Then
            AddError("Invalid Username or Password.  Please check and try again.")
            Return False
        ElseIf dbBuilderAccount.IsActive = False Then
            AddError("Your account is inactive.  Please contact an administrator for further details.")
            Return False
        ElseIf bBuilderIsActive = False Then
            AddError("Your account is linked to an inactive Builder.  Please contact an adminstrator for further details.")
            Return False
        Else
            Session("BuilderAccountId") = dbBuilderAccount.BuilderAccountID
            Session("BuilderId") = dbBuilderAccount.BuilderID

            Dim dbbuilder As BuilderRow = BuilderRow.GetRow(DB, dbBuilderAccount.BuilderID)
            'log builder login
            PageUrl = Request.Url.ToString()
            CurrentUserId = Session("BuilderAccountID")
            Session("Username") = dbBuilderAccount.Username
            UserName = Session("Username")
            Core.DataLog("Login", PageUrl, CurrentUserId, "Builder Login", "", "", "", "", UserName)
            'end log

            Dim p As New VindiciaPaymentProcessor(DB)
            p.IsTestMode = SysParam.GetValue(DB, "TestMode")

            Dim dbPrimary As BuilderAccountRow = BuilderAccountRow.GetPrimaryAccount(DB, dbbuilder.BuilderID)
            If dbPrimary.BuilderAccountID = Nothing Then
                dbBuilderAccount.IsPrimary = True
                dbBuilderAccount.Update()
            End If

            Dim dbRegistration As BuilderRegistrationRow = BuilderRegistrationRow.GetRowByBuilder(DB, dbbuilder.BuilderID)

            Dim regDate As DateTime
            If p.CheckSubscriptionStatus(dbbuilder, regDate) Then
                'Per Brian: Remove New Year Check 010410
                'If dbRegistration.CompleteDate.Year <> Now.Year Then
                '    If dbbuilder.Guid = Nothing Then
                '        'dbbuilder.Guid = Core.GenerateFileID()
                '        'dbbuilder.Update()
                '        dbbuilder.UpdateGuid(Core.GenerateFileID)
                '    End If
                '    Response.Redirect(GlobalSecureName & "/forms/builder-registration/default.aspx?id=" & dbbuilder.Guid)
                'End If
                If dbBuilderAccount.RequirePasswordUpdate Or dbBuilderAccount.Password = Nothing OrElse Not dbBuilderAccount.PasswordEncrypted Then
                    Response.Redirect(GlobalSecureName & "/changepw.aspx")
                Else
                    Return True
                End If
            Else
                If dbbuilder.Guid = Nothing Then
                    'dbbuilder.Guid = Core.GenerateFileID()
                    'dbbuilder.Update()
                    dbbuilder.UpdateGuid(Core.GenerateFileID)
                End If

                If SysParam.GetValue(DB, "UseEntitlements") Then
                    If dbbuilder.SkipEntitlementCheck Then
                        If dbRegistration.BuilderRegistrationID = Nothing Then
                            Response.Redirect("/forms/builder-registration/default.aspx")
                        Else
                            Response.Redirect("/builder/")
                        End If
                    End If
                End If
                'Per Brian: Remove New Year Check 010410
                'If dbRegistration.CompleteDate.Year <> Now.Year Then
                'Response.Redirect(GlobalSecureName & "/forms/builder-registration/default.aspx?id=" & dbbuilder.Guid)
                'Else
                'Response.Redirect(GlobalSecureName & "/builder/update.aspx")
                'End If

                '---------------------------------------------------------------------------------------------------------
                '----------------------- ALLOW ACCESS TO SYSTEM IF BILLING START DATE IS IN FUTURE -----------------------
                Dim dbBBP As BuilderBillingPlanRow = BuilderBillingPlanRow.GetRow(DB, dbbuilder.BuilderID)

                If dbBBP.SubscriptionStartDate > Now.Date Then
                    If dbbuilder.IsNew = True Or dbRegistration.BuilderRegistrationID = Nothing Then
                        Response.Redirect("/forms/builder-registration/default.aspx")
                    Else
                        Session("SkipRegistrationCheck") = True
                        Response.Redirect("/builder/")
                    End If
                Else
                    Response.Redirect(GlobalSecureName & "/builder/update.aspx")
                End If
                '---------------------------------------------------------------------------------------------------------
            End If
        End If
    End Function

    Private Function LogInPIQ() As Boolean
        Dim dbPIQAccount As PIQAccountRow = PIQAccountRow.GetRowByUsername(DB, txtUsername.Text)
        Dim dbPIQ As PIQRow = PIQRow.GetRow(DB, dbPIQAccount.PIQID)

        If (dbPIQAccount.PIQAccountID = Nothing Or dbPIQ.PIQID = Nothing) OrElse dbPIQAccount.Password <> txtPassword.Text Then
            AddError("Invalid Username or Password.  Please check and try again.")
        Else
            Session("PIQId") = dbPIQ.PIQID
            Session("PIQAccountId") = dbPIQAccount.PIQAccountID
            'log PIQ login
            PageUrl = Request.Url.ToString()
            CurrentUserId = Session("PIQAccountID")
            Session("Username") = dbPIQAccount.Username
            UserName = Session("Username")
            Core.DataLog("Login", PageUrl, CurrentUserId, "PIQ Login", "", "", "", "", UserName)
            'end log

            If dbPIQAccount.RequirePasswordUpdate Then
                Response.Redirect(GlobalSecureName & "/changepw.aspx")
            Else
                Response.Redirect(GlobalRefererName & "/piq/default.aspx")
            End If
        End If
    End Function

End Class
