Imports Components
Imports System.IO
Imports DataLayer
Imports MasterPages
Imports System.Data.SqlClient

Public Class Optout
    Inherits SitePage

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            txtEmail.Text = Request("e")
        End If
    End Sub

    Protected Sub btnUnsubscribe_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUnsubscribe.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbMailingMember As MailingMemberRow
            Dim MemberId As Integer = 0

            dbMailingMember = MailingMemberRow.GetRowByEmail(DB, txtEmail.Text)
            dbMailingMember.Email = txtEmail.Text
            dbMailingMember.DeleteFromAllNotPermanentLists()
            dbMailingMember.Unsubscribe = Now()
            dbMailingMember.Update()

            DB.CommitTransaction()

            divConfirm.Visible = True
            divMain.Visible = False
            ltlEmail.Text = txtEmail.Text

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
