Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected AdminMessageID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("MESSAGES")

        If Not IsPostBack Then

        End If
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbAdminMessage As AdminMessageRow
            dbAdminMessage = New AdminMessageRow(DB)
            dbAdminMessage.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
            dbAdminMessage.Subject = txtSubject.Text
            dbAdminMessage.Title = txtTitle.Text
            dbAdminMessage.Message = txtMessage.Text
            dbAdminMessage.StartDate = dtStartDate.Value
            dbAdminMessage.EndDate = dtEndDate.Value
            dbAdminMessage.SendEmailCopy = rblSendEmailCopy.SelectedValue
            dbAdminMessage.IsActive = rblIsActive.SelectedValue
            dbAdminMessage.IsAlert = rblIsAlert.SelectedValue

            If AdminMessageID <> 0 Then
                dbAdminMessage.Update()
            Else
                AdminMessageID = dbAdminMessage.Insert
            End If

            DB.CommitTransaction()

            Response.Redirect("recipient.aspx?AdminMessageID=" & dbAdminMessage.AdminMessageID)

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx")
    End Sub

End Class
