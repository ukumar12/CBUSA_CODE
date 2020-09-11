Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class EditPhoto
    Inherits SitePage

    Protected PhotoId As Integer
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureVendorAccess()

        If Not CType(Me.Page, SitePage).IsLoggedInVendor Then
            Response.Redirect("/default.aspx")
        End If

        PhotoId = Convert.ToInt32(Request("PhotoId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
        PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorId")
        UserName = Session("Username")
    End Sub

    Private Sub LoadFromDB()
        
        If PhotoId = 0 Then
            btnDelete.Visible = False
            'If DB.GetDataTable("Select PhotoId From VendorPhoto Where VendorId = " & DB.Number(Session("VendorId"))).Rows.Count > 9 Then
            '    Response.Redirect("default.aspx")
            'End If
            Exit Sub
        End If

        Dim dbVendorPhoto As VendorPhotoRow = VendorPhotoRow.GetRow(DB, PhotoId)
        txtCaption.Text = dbVendorPhoto.Caption
        txtAltText.Text = dbVendorPhoto.AltText
        fuPhoto.CurrentFileName = dbVendorPhoto.Photo
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbVendorPhoto As VendorPhotoRow

            If PhotoId <> 0 Then
                dbVendorPhoto = VendorPhotoRow.GetRow(DB, PhotoId)
            Else
                dbVendorPhoto = New VendorPhotoRow(DB)
            End If
            dbVendorPhoto.Caption = txtCaption.Text
            dbVendorPhoto.AltText = txtAltText.Text
            dbVendorPhoto.VendorId = Session("VendorId")
            If fuPhoto.NewFileName <> String.Empty Then
                fuPhoto.SaveNewFile()
                dbVendorPhoto.Photo = fuPhoto.NewFileName
                Dim VendorPhotoWidth As Integer = CType(SysParam.GetValue(DB, "VendorPhotoWidth"), Integer)
                Dim VendorPhotoHeight As Integer = CType(SysParam.GetValue(DB, "VendorPhotoHeight"), Integer)
                Core.ResizeImageWithQuality(Server.MapPath("/assets/vendorphoto/" & dbVendorPhoto.Photo), Server.MapPath("/assets/vendorphoto/" & dbVendorPhoto.Photo), VendorPhotoWidth, VendorPhotoHeight, 100)
                Dim VendorPhotoThumbWidth As Integer = CType(SysParam.GetValue(DB, "VendorPhotoThumbWidth"), Integer)
                Dim VendorPhotoThumbHeight As Integer = CType(SysParam.GetValue(DB, "VendorPhotoThumbHeight"), Integer)
                Core.ResizeImageWithQuality(Server.MapPath("/assets/vendorphoto/" & dbVendorPhoto.Photo), Server.MapPath("/assets/vendorphoto/thumb/" & dbVendorPhoto.Photo), VendorPhotoThumbWidth, VendorPhotoThumbHeight, 100)
            ElseIf fuPhoto.MarkedToDelete Then
                dbVendorPhoto.Photo = Nothing
            End If

            If PhotoId <> 0 Then
                dbVendorPhoto.Update()
                'log Btn Save
                Core.DataLog("Photo Gallery", PageURL, CurrentUserId, "Update Photo", "", "", "", "", UserName)
                'end log
            Else
                PhotoId = dbVendorPhoto.Insert
                'log Btn Save
                Core.DataLog("Photo Gallery", PageURL, CurrentUserId, "Add Photo", "", "", "", "", UserName)
                'end log
            End If

            DB.CommitTransaction()

            If fuPhoto.NewFileName <> String.Empty OrElse fuPhoto.MarkedToDelete Then fuPhoto.RemoveOldFile()

            Response.Redirect("default.aspx")

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx")
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?PhotoId=" & PhotoId)
    End Sub
End Class

