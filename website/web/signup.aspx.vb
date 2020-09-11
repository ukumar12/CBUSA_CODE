Imports Components
Imports System.IO
Imports DataLayer
Imports MasterPages
Imports System.Data.SqlClient

Public Class Signup
    Inherits SitePage

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            txtEmail.Text = Request("e")
            If txtEmail.Text = "your@email.com" Then txtEmail.Text = String.Empty
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        cblLists.DataSource = MailingListRow.GetPermanentLists(DB)
        cblLists.DataTextField = "name"
        cblLists.DataValueField = "listid"
        cblLists.DataBind()

        For Each objListItem As ListItem In cblLists.Items
            objListItem.Selected = True
        Next
    End Sub

    Protected Sub btnSignup_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSignup.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbMailingMember As MailingMemberRow
            Dim MemberId As Integer = 0

            dbMailingMember = MailingMemberRow.GetRowByEmail(DB, txtEmail.Text)
            dbMailingMember.Email = txtEmail.Text
            dbMailingMember.Name = txtName.Text
            dbMailingMember.MimeType = rblMimeType.SelectedValue
            dbMailingMember.Status = "ACTIVE"
            If dbMailingMember.MemberId <> 0 Then
                dbMailingMember.Update()
            Else
                MemberId = dbMailingMember.Insert
            End If
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
