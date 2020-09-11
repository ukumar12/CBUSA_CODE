Imports Components
Imports DataLayer
Imports System.Data.SqlClient

Partial Class PasswordChange
    Inherits AdminPage
    Private PasswordEx As Boolean

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("AdminExpiredPassword") Is Nothing Then Session("AdminExpiredPassword") = False
        If Session("AdminIsNew") Is Nothing Then Session("AdminIsNew") = False

        PasswordEx = SysParam.GetValue(DB, "PasswordEx")

        If PasswordEx = False Then
            pnlPasswordEx.Visible = False
        End If

        pnlAdminExpiredPassword.Visible = Session("AdminExpiredPassword")
        pnlAdminIsNew.Visible = Session("AdminIsNew")
        If Session("AdminExpiredPassword") Or Session("AdminIsNew") Then
            tblWrapper.Attributes("align") = "center"
            ErrorPlaceHolder.Width = "600"
        End If
    End Sub

    Private Sub submitButton_onClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles submitButton.Click
        If Not IsValid Then Exit Sub

        Dim IsError As Boolean = False
        Dim dbAdmin As AdminRow = AdminRow.GetRow(DB, LoggedInAdminId)
        Dim NumRecords As Integer = 4

        If Not dbAdmin.Password = PASSWORD_OLD.Text Then
            AddError("The Old Password does not match your current password.  Please try again.")
            IsError = True
        End If
        If dbAdmin.Password = PASSWORD_NEW.Text Then
            AddError("The Old Password and the new one are the same. Please try again.")
            IsError = True
        End If
        If AdminPasswordRow.PasswordUsedBefore(DB, LoggedInAdminId, PASSWORD_NEW.Text, NumRecords) Then
            AddError("You may not use any of your last " & NumRecords & " passwords for New Password")
            IsError = True
        End If



        If PasswordEx = True Then
            If Not dbAdmin.PasswordEx = PASSWORDEX_OLD.Text Then
                AddError("The Old Secondary Password does not match your current password.  Please try again.")
                IsError = True
            End If
            If dbAdmin.Password = PASSWORDEX_NEW.Text Then
                AddError("The Old Secondary Password and the new one are the same. Please try again.")
                IsError = True
            End If
            If AdminPasswordRow.PasswordUsedBefore(DB, LoggedInAdminId, PASSWORDEX_NEW.Text, NumRecords) Then
                AddError("You may not use any of your last " & NumRecords & " passwords for New Secondary Password")
                IsError = True
            End If
        End If

        If IsError Then
            Exit Sub
        End If

        Try
            DB.BeginTransaction()
            dbAdmin.Password = PASSWORD_NEW.Text
            dbAdmin.PasswordEx = PASSWORDEX_NEW.Text
            dbAdmin.PasswordDate = Now()
            dbAdmin.Update()

            'Save password in the password log
            Dim dbAdminPassword As AdminPasswordRow

            dbAdminPassword = New AdminPasswordRow(DB)
            dbAdminPassword.AdminId = dbAdmin.AdminId
            dbAdminPassword.Password = dbAdmin.EncryptedPassword
            dbAdminPassword.PasswordDate = dbAdmin.PasswordDate
            dbAdminPassword.Insert()

            'Save password in the password log
            If PasswordEx = True Then
                dbAdminPassword = New AdminPasswordRow(DB)
                dbAdminPassword.AdminId = dbAdmin.AdminId
                dbAdminPassword.Password = dbAdmin.EncryptedPasswordEx
                dbAdminPassword.PasswordDate = dbAdmin.PasswordDate
                dbAdminPassword.Insert()
            End If

            DB.CommitTransaction()

            If Session("AdminExpiredPassword") Or Session("AdminIsNew") Then
                Session("AdminExpiredPassword") = False
                Session("AdminIsNew") = False
                Response.Redirect("/admin/")
            Else
                Session("Confirmation") = "Your password has been successfully changed."
                Core.LogEvent("Password was changed for username """ & Session("Username") & """", Diagnostics.EventLogEntryType.Information)
                Response.Redirect("/admin/confirm.aspx")
            End If
        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        If Session("AdminExpiredPassword") Or Session("AdminIsNew") Then
            Response.Redirect("/admin/logout.aspx")
        Else
            Response.Redirect("/admin/main.aspx")
        End If
    End Sub

End Class
