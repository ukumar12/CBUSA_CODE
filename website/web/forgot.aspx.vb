Imports Components
Imports DataLayer

Partial Class forgot
    Inherits SitePage
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        If txtUsername.Text = String.Empty Then
            AddError("Please enter your username in the box below.")
            Exit sub
        End If

        Dim dbBuilderAccount As BuilderAccountRow = BuilderAccountRow.GetAccountByUsername(DB, txtUsername.Text)
        Dim dbVendorAccount As VendorAccountRow = VendorAccountRow.GetVendorByUsername(DB, txtUsername.Text)
        Dim dbPIQAccount As PIQAccountRow = PIQAccountRow.GetRowByUsername(DB, txtUsername.Text)
        Dim password As String = Core.GenerateFileID().Substring(0, 8)

        If dbBuilderAccount.Created <> Nothing Then
            dbBuilderAccount.Password = password
            dbBuilderAccount.RequirePasswordUpdate = True
            dbBuilderAccount.Update()
            SendPassword(dbBuilderAccount.Email, Core.BuildFullName(dbBuilderAccount.FirstName, String.Empty, dbBuilderAccount.LastName), dbBuilderAccount.Username, password)
            'SendPassword("steve.schlereth@americaneagle.com", "Steve Schlereth", dbBuilderAccount.Username, password)
            'Log Forgot Password Mail
            PageURL = Request.Url.ToString()
            CurrentUserId = dbBuilderAccount.BuilderID
            UserName = dbBuilderAccount.Username
            Core.DataLog("User Login", PageURL, CurrentUserId, "Forgot Password Mail", "", "", "", "", UserName)
            'End logging activity
        ElseIf dbVendorAccount.Created <> Nothing Then
            dbVendorAccount.Password = password
            dbVendorAccount.RequirePasswordUpdate = True
            dbVendorAccount.Update()

            SendPassword(dbVendorAccount.Email, Core.BuildFullName(dbVendorAccount.FirstName, String.Empty, dbVendorAccount.LastName), dbVendorAccount.Username, password)
            'SendPassword("steve.schlereth@americaneagle.com", "Steve Schlereth", dbVendorAccount.Username, password)
            'Log Forgot Password Mail
            PageURL = Request.Url.ToString()
            CurrentUserId = dbVendorAccount.VendorID
            UserName = dbVendorAccount.Username
            Core.DataLog("User Login", PageURL, CurrentUserId, "Forgot Password Mail", "", "", "", "", UserName)
            'End logging activity
        ElseIf dbPIQAccount.Created <> Nothing Then
            dbPIQAccount.Password = password
            dbPIQAccount.RequirePasswordUpdate = True
            dbPIQAccount.Update()

            SendPassword(SysParam.GetValue(DB, "AdminEmail"), Core.BuildFullName(dbPIQAccount.FirstName, String.Empty, dbPIQAccount.LastName), dbPIQAccount.Username, password)
            'Log Forgot Password Mail
            PageURL = Request.Url.ToString()
            CurrentUserId = dbPIQAccount.PIQID
            UserName = dbPIQAccount.Username
            Core.DataLog("User Login", PageURL, CurrentUserId, "Forgot Password Mail", "", "", "", "", UserName)
            'End logging activity
        Else
            AddError("Username could not be found.  Check spelling and try again.")
        End If

    End Sub

    Private Sub SendPassword(ByVal Email As String, ByVal Name As String, ByVal Username As String, ByVal Password As String)
        Dim msg As String = Name & "," & vbCrLf & vbCrLf
        msg &= "The password for your CBUSA account has been reset." & vbCrLf & vbCrLf
        msg &= "Log in to " & GlobalSecureName & "/changepw.aspx using the temporary password provided below to access the CBUSA website." & vbCrLf & vbCrLf
        msg &= "Username: " & Username & vbCrLf
        msg &= "Password: " & Password & vbCrLf & vbCrLf

        Dim FromAddress As String = SysParam.GetValue(DB, "ContactUsEmail")
        Dim FromName As String = SysParam.GetValue(DB, "ContactUsName")
        Core.SendSimpleMail(FromAddress, FromName, Email, Name, "CBUSA Forgot Password", msg)

        divSent.Visible = True
        tblForm.Visible = False

        Page.ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "window.setTimeout(""location.href='/default.aspx';"",30000);", True)
    End Sub
End Class
