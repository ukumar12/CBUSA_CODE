Imports Components
Imports DataLayer
Imports Utilities

Partial Class forms_builder_registration_account
    Inherits SitePage

    Private dbVendorAccount As VendorAccountRow
    Private dbBuilderAccount As BuilderAccountRow
    Private dbPIQAccount As PIQAccountRow

    Protected ReadOnly Property BuilderAccountID() As Integer
        Get
            If dbBuilderAccount Is Nothing Then
                Return Nothing
            Else
                Return dbBuilderAccount.BuilderAccountID
            End If
        End Get
    End Property

    Protected ReadOnly Property VendorAccountID() As Integer
        Get
            If dbVendorAccount Is Nothing Then
                Return Nothing
            Else
                Return dbVendorAccount.VendorAccountID
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
        End If
    End Sub

    Private Sub LoadFromDb(ByVal member As Object)
        txtFirstName.Text = member.FirstName
        txtLastName.Text = member.LastName
        If dbPIQAccount Is Nothing Then
            txtEmail.Text = member.Email
        End If
        txtUsername.Text = member.Username
    End Sub

    Protected Sub btnContinue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnContinue.Click
        If Not Page.IsValid Then Exit Sub

        If Not cbTerms.Checked Then
            AddError("You must accept the End User License Agreement to continue")
            Exit Sub
        End If

        If Not CheckAvailability() Then
            AddError("The entered Username is unavailable.")
            Exit Sub
        ElseIf Session("RequirePasswordChange") IsNot Nothing AndAlso txtPassword.Text = String.Empty Then
            AddError("You must change your password before you log in.")
            Exit Sub
        ElseIf txtPassword.Text <> txtConfirmpassword.Text Then
            AddError("Password and Confirm Password do not match.")
            Exit Sub
        Else

            If dbVendorAccount IsNot Nothing Then
                If dbVendorAccount.RequirePasswordUpdate And txtPassword.Text = String.Empty Then
                    AddError("You must enter a new password in the Password and Confirm Password fields")
                    Exit Sub
                End If
                dbVendorAccount.FirstName = txtFirstName.Text
                dbVendorAccount.LastName = txtLastName.Text
                dbVendorAccount.Email = txtEmail.Text
                dbVendorAccount.Username = txtUsername.Text
                dbVendorAccount.Password = txtPassword.Text
                dbVendorAccount.Title = txtTitle.Text
                dbVendorAccount.Update()

                '' Salesforce Integration
                'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
                'If sfHelper.Login() Then

                '    If sfHelper.UpsertVendorAccount(dbVendorAccount) = False Then
                '        'throw error
                '    End If
                'End If

                Session("RequirePasswordChange") = False
                Response.Redirect("/vendor/")
            ElseIf dbBuilderAccount IsNot Nothing Then
                If dbBuilderAccount.RequirePasswordUpdate And txtPassword.Text = String.Empty Then
                    AddError("You must enter a new password in the Password and Confirm Password fields")
                    Exit Sub
                End If

                dbBuilderAccount.FirstName = txtFirstName.Text
                dbBuilderAccount.LastName = txtLastName.Text
                dbBuilderAccount.Email = txtEmail.Text
                dbBuilderAccount.Username = txtUsername.Text
                dbBuilderAccount.Password = txtPassword.Text
                dbBuilderAccount.Title = txtTitle.Text
                dbBuilderAccount.Update()

                '' Salesforce Integration
                'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
                'If sfHelper.Login() Then
                '    If sfHelper.UpsertBuilderAccount(dbBuilderAccount) = False Then
                '        'throw error
                '    End If
                'End If
                Session("RequirePasswordChange") = False
                Response.Redirect("/builder/")
            ElseIf dbPIQAccount IsNot Nothing Then
                If dbPIQAccount.RequirePasswordUpdate And txtPassword.Text = String.Empty Then
                    AddError("You must enter a new password in the Password and Confirm Password fields")
                    Exit Sub
                End If

                dbPIQAccount.FirstName = txtFirstName.Text
                dbPIQAccount.LastName = txtLastName.Text
                dbPIQAccount.Username = txtUsername.Text
                dbPIQAccount.Password = txtPassword.Text
                dbPIQAccount.Update()

                Session("RequirePasswordChange") = False
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
