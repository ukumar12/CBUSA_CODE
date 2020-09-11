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

        AdminMessageID = Convert.ToInt32(Request("AdminMessageID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If AdminMessageID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbAdminMessage As AdminMessageRow = AdminMessageRow.GetRow(DB, AdminMessageID)
        txtSubject.Text = dbAdminMessage.Subject
        txtTitle.Text = dbAdminMessage.Title
        txtMessage.Text = dbAdminMessage.Message
        dtStartDate.Value = dbAdminMessage.StartDate
        dtEndDate.Value = dbAdminMessage.EndDate
        rblSendEmailCopy.SelectedValue = dbAdminMessage.SendEmailCopy
        rblIsActive.SelectedValue = dbAdminMessage.IsActive
        rblIsAlert.SelectedValue = dbAdminMessage.IsAlert
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbAdminMessage As AdminMessageRow

            If AdminMessageID <> 0 Then
                dbAdminMessage = AdminMessageRow.GetRow(DB, AdminMessageID)
            Else
                dbAdminMessage = New AdminMessageRow(DB)
            End If
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


            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?AdminMessageID=" & AdminMessageID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
