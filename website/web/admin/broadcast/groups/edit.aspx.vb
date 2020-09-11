Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected GroupId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BROADCAST")

        GroupId = Convert.ToInt32(Request("GroupId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        cblLists.DataSource = MailingListRow.GetPermanentLists(DB)
        cblLists.DataTextField = "Name"
        cblLists.DataValueField = "ListId"
        cblLists.DataBind()

        If GroupId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbMailingGroup As MailingGroupRow = MailingGroupRow.GetRow(DB, GroupId)
        txtName.Text = dbMailingGroup.Name
        txtDescription.Text = dbMailingGroup.Description
        dtStartDate.Value = dbMailingGroup.StartDate
        dtEndDate.Value = dbMailingGroup.EndDate
        cblLists.SelectedValues = dbMailingGroup.GetSelectedMailingLists
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbMailingGroup As MailingGroupRow

            If GroupId <> 0 Then
                dbMailingGroup = MailingGroupRow.GetRow(DB, GroupId)
            Else
                dbMailingGroup = New MailingGroupRow(DB)
            End If
            dbMailingGroup.Name = txtName.Text
            dbMailingGroup.Description = txtDescription.Text
            dbMailingGroup.StartDate = dtStartDate.Value
            dbMailingGroup.EndDate = dtEndDate.Value
            dbMailingGroup.IsPermanent = True
            dbMailingGroup.ModifyAdminId = LoggedInAdminId

            If GroupId <> 0 Then
                dbMailingGroup.Update()
            Else
                dbMailingGroup.CreateAdminId = LoggedInAdminId
                GroupId = dbMailingGroup.Insert
            End If
            dbMailingGroup.DeleteFromAllMailingLists()
            dbMailingGroup.InsertToMailingLists(cblLists.SelectedValues)

            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?GroupId=" & GroupId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

