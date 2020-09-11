Imports Components
Imports System.IO
Imports DataLayer
Imports MasterPages
Imports System.Data.SqlClient

Public Class Unsubscribe
    Inherits SitePage

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        txtEmail.Text = Request("e")

        Dim dbMailingMember As MailingMemberRow = MailingMemberRow.GetRowByEmail(DB, txtEmail.Text)
        If dbMailingMember.MemberId = 0 Then Response.Redirect("/")

        txtName.Text = dbMailingMember.Name
        rblMimeType.SelectedValue = dbMailingMember.MimeType
        cblLists.DataSource = MailingListRow.GetPermanentLists(DB)
        cblLists.DataTextField = "name"
        cblLists.DataValueField = "listid"
        cblLists.DataBind()
        cblLists.SelectedValues = dbMailingMember.SubscribedLists
    End Sub

    Protected Sub btnUpdate_Click1(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbMailingMember As MailingMemberRow = MailingMemberRow.GetRowByEmail(DB, txtEmail.Text)
            dbMailingMember.Email = txtEmail.Text
            dbMailingMember.Name = txtName.Text
            dbMailingMember.MimeType = rblMimeType.SelectedValue
            dbMailingMember.Update()
            dbMailingMember.DeleteFromAllPermanentLists()
            dbMailingMember.InsertToLists(cblLists.SelectedValues)

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
