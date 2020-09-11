Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected F_ItemId As Integer
    Protected ImageId As Integer
    Protected dbStoreItem As StoreItemRow

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("STORE")

        ImageId = Convert.ToInt32(Request("ImageId"))
        F_ItemId = Convert.ToInt32(Request("F_ItemId"))
        dbStoreItem = StoreItemRow.GetRow(DB, F_ItemId)
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If ImageId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbStoreItemImage As StoreItemImageRow = StoreItemImageRow.GetRow(DB, ImageId)
        txtImageAltTag.Text = dbStoreItemImage.ImageAltTag
        fuImage.CurrentFileName = dbStoreItemImage.Image
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbStoreItemImage As StoreItemImageRow

            If ImageId <> 0 Then
                dbStoreItemImage = StoreItemImageRow.GetRow(DB, ImageId)
            Else
                dbStoreItemImage = New StoreItemImageRow(DB)
            End If
            dbStoreItemImage.ItemId = F_ItemId
            dbStoreItemImage.ImageAltTag = txtImageAltTag.Text
            If fuImage.NewFileName <> String.Empty Then
                fuImage.SaveNewFile("/assets/item/alternate/original/")
                dbStoreItemImage.Image = fuImage.NewFileName

                Core.ResizeImage(Server.MapPath("/assets/item/alternate/original/" & dbStoreItemImage.Image), Server.MapPath("/assets/item/alternate/" & dbStoreItemImage.Image), SysParam.GetValue(DB, "StoreItemImageAlternateSmallWidth"), SysParam.GetValue(DB, "StoreItemImageAlternateSmallHeight"))
                Core.ResizeImage(Server.MapPath("/assets/item/alternate/original/" & dbStoreItemImage.Image), Server.MapPath("/assets/item/alternate/large/" & dbStoreItemImage.Image), SysParam.GetValue(DB, "StoreItemImageAlternateLargeWidth"), SysParam.GetValue(DB, "StoreItemImageAlternateLargeHeight"))

            ElseIf fuImage.MarkedToDelete Then
                dbStoreItemImage.Image = Nothing
            End If

            If ImageId <> 0 Then
                dbStoreItemImage.Update()
            Else
                ImageId = dbStoreItemImage.Insert
            End If

            DB.CommitTransaction()

            If fuImage.NewFileName <> String.Empty Or fuImage.MarkedToDelete Then fuImage.RemoveOldFile()

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
        Response.Redirect("delete.aspx?ImageId=" & ImageId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

