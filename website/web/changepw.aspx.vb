Imports Components
Imports DataLayer

Partial Class changepw
    Inherits SitePage

    Private dbVendorAccount As VendorAccountRow
    Private dbBuilderAccount As BuilderAccountRow
    Private dbPIQAccount As PIQAccountRow
    Private UserName As String = ""
    Private PageURL As String = ""
    Private CurrentBuilderId As String = ""
    Private CurrentVendorId As String = ""
    Private CurrentPIQId As String = ""

    Protected ReadOnly Property BuilderAccountId() As Integer
        Get
            If dbBuilderAccount Is Nothing Then
                Return Nothing
            Else
                Return dbBuilderAccount.BuilderAccountID
            End If
        End Get
    End Property

    Protected ReadOnly Property VendorAccountId() As Integer
        Get
            If dbVendorAccount Is Nothing Then
                Return Nothing
            Else
                Return dbVendorAccount.VendorAccountID
            End If
        End Get
    End Property

    Protected ReadOnly Property PIQAccountId() As Integer
        Get
            If dbPIQAccount Is Nothing Then
                Return Nothing
            Else
                Return dbPIQAccount.PIQAccountID
            End If
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("VendorAccountId") IsNot Nothing Then
            dbVendorAccount = VendorAccountRow.GetRow(DB, Session("VendorAccountId"))
        ElseIf Session("BuilderAccountId") IsNot Nothing Then
            dbBuilderAccount = BuilderAccountRow.GetRow(DB, Session("BuilderAccountId"))
        ElseIf Session("PIQAccountId") IsNot Nothing Then
            dbPIQAccount = PIQAccountRow.GetRow(DB, Session("PIQAccountId"))

            trTitle.Visible = False
            rfvtxtTItle.Enabled = False
        Else
            Response.Redirect("/default.aspx")
        End If
        If Not IsPostBack Then
            If dbVendorAccount IsNot Nothing Then
                LoadFromDb(dbVendorAccount)
            ElseIf dbBuilderAccount IsNot Nothing Then
                LoadFromDb(dbBuilderAccount)
            ElseIf dbPIQAccount IsNot Nothing Then
                LoadFromDb(dbPIQAccount)
            End If
        End If
        PageURL = Request.Url.ToString()
        CurrentVendorId = Session("VendorId")
        CurrentBuilderId = Session("BuilderId")
        UserName = Session("Username")
        CurrentPIQId = Session("PIQID")
    End Sub

    Private Sub LoadFromDb(ByVal member As Object)
        txtFirstName.Text = member.FirstName
        txtLastName.Text = member.LastName
        'If dbPIQAccount Is Nothing Then
        txtEmail.Text = member.Email
        'End If
        txtPhone.Text = member.Phone
        txtUsername.Text = member.Username
    End Sub

    Protected Sub btnContinue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnContinue.Click
        If Not Page.IsValid Then Exit Sub

        If Not CheckAvailability() Then
            AddError("The entered Username is unavailable.")
        ElseIf Session("RequirePasswordChange") IsNot Nothing AndAlso txtPassword.Text = String.Empty Then
            AddError("You must change your password before you log in.")
        ElseIf txtPassword.Text <> txtConfirmpassword.Text Then
            AddError("Password and Confirm Password do not match.")
        Else
            If dbVendorAccount IsNot Nothing Then
                dbVendorAccount.FirstName = txtFirstName.Text
                dbVendorAccount.LastName = txtLastName.Text
                dbVendorAccount.Email = txtEmail.Text
                dbVendorAccount.Username = txtUsername.Text
                dbVendorAccount.Title = txtTitle.Text
                If txtPassword.Text <> Nothing Then
                    dbVendorAccount.Password = txtPassword.Text
                    dbVendorAccount.RequirePasswordUpdate = False
                ElseIf dbVendorAccount.RequirePasswordUpdate Or Not dbVendorAccount.PasswordEncrypted Or dbVendorAccount.Password = Nothing Then
                    AddError("You must change your password before you log in.")
                    Exit Sub
                End If
                dbVendorAccount.Update()
                'log Change Password of Vendor
                Core.DataLog("Change Password", PageURL, CurrentVendorId, "Change Password of Vendor", "", "", "", "", UserName)
                'end log
                Response.Redirect("/vendor/")
            ElseIf dbBuilderAccount IsNot Nothing Then
                dbBuilderAccount.FirstName = txtFirstName.Text
                dbBuilderAccount.LastName = txtLastName.Text
                dbBuilderAccount.Email = txtEmail.Text
                dbBuilderAccount.Username = txtUsername.Text
                dbBuilderAccount.Title = txtTitle.Text
                If txtPassword.Text <> Nothing Then
                    dbBuilderAccount.Password = txtPassword.Text
                    dbBuilderAccount.RequirePasswordUpdate = False
                ElseIf dbBuilderAccount.RequirePasswordUpdate Or Not dbBuilderAccount.PasswordEncrypted Or dbBuilderAccount.Password = Nothing Then
                    AddError("You must change your password before you log in.")
                    Exit Sub
                End If
                dbBuilderAccount.Update()
                'log Change Password of Builder
                Core.DataLog("Change Password", PageURL, CurrentBuilderId, "Change Password of Builder", "", "", "", "", UserName)
                'end log
                Response.Redirect("/builder/")
            ElseIf dbPIQAccount IsNot Nothing Then
                dbPIQAccount.FirstName = txtFirstName.Text
                dbPIQAccount.LastName = txtLastName.Text
                dbPIQAccount.Username = txtUsername.Text
                dbPIQAccount.Email = txtEmail.Text
                dbPIQAccount.Phone = txtPhone.Text
                If txtPassword.Text <> Nothing Then
                    dbPIQAccount.Password = txtPassword.Text
                    dbPIQAccount.RequirePasswordUpdate = False
                ElseIf dbPIQAccount.RequirePasswordUpdate Or dbPIQAccount.Password = Nothing Then
                    AddError("You must change your password before you log in.")
                    Exit Sub
                End If
                dbPIQAccount.Update()
                'log Change Password of PIQ
                Core.DataLog("Change Password", PageURL, CurrentPIQId, "Change Password of PIQ", "", "", "", "", UserName)
                'end log
                Response.Redirect("/piq/")
                End If
            End If
    End Sub

    Private Function CheckAvailability() As Boolean
        Dim sql As String = "select top 1 * from BuilderAccount where Username=" & DB.Quote(txtUsername.Text)

        If Session("BuilderAccountId") IsNot Nothing Then
            sql &= " and BuilderAccountId <> " & DB.Number(Session("BuilderAccountId"))
        End If

        If DB.ExecuteSQL(sql) > 0 Then
            Return False
        End If

        sql = "select top 1 * from VendorAccount where Username=" & DB.Quote(txtUsername.Text)
        If Session("LoginVendorAccountId") IsNot Nothing Then
            sql &= " and VendorAccountId <> " & DB.Number(Session("LoginVendorAccountId"))
        End If

        If DB.ExecuteSQL(sql) > 0 Then
            Return False
        End If

        sql = "select top 1 * from PIQAccount where Username=" & DB.Quote(txtUsername.Text)
        If Session("LoginPIQAccountId") IsNot Nothing Then
            sql &= " and PIQAccountId <> " & DB.Number(Session("LoginPIQAccountId"))
        End If

        If DB.ExecuteSQL(sql) > 0 Then
            Return False
        End If

        Return True
    End Function
End Class
