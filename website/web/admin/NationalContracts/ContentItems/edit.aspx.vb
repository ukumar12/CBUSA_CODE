Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected ContentItemsID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        ContentItemsID = Convert.ToInt32(Request("ContentItemsID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If ContentItemsID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbContentItems As ContentItemsRow = ContentItemsRow.GetRow(DB, ContentItemsID)
        txtTitle.Text = dbContentItems.Title
        fuFileName.CurrentFileName = dbContentItems.FileName
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbContentItems As ContentItemsRow

            If ContentItemsID <> 0 Then
                dbContentItems = ContentItemsRow.GetRow(DB, ContentItemsID)
            Else
                dbContentItems = New ContentItemsRow(DB)
            End If
            dbContentItems.AdminID = LoggedInAdminId
            dbContentItems.Title = txtTitle.Text
            dbContentItems.Uploaded = Date.Now

            If fuFileName.NewFileName <> String.Empty Then
                fuFileName.SaveNewFile()
                dbContentItems.FileName = fuFileName.NewFileName
            ElseIf fuFileName.MarkedToDelete Then
                dbContentItems.FileName = Nothing
            End If

            If ContentItemsID <> 0 Then
                dbContentItems.Update()
            Else
                ContentItemsID = dbContentItems.Insert
            End If

            DB.CommitTransaction()

            If fuFileName.NewFileName <> String.Empty OrElse fuFileName.MarkedToDelete Then fuFileName.RemoveOldFile()

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
        Response.Redirect("delete.aspx?ContentItemsID=" & ContentItemsID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
