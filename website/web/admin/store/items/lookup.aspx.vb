Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components
Imports Controls

Public Class Lookup
    Inherits AdminPage

    Protected hdnImageName As String
    Protected divImageName As String
    Protected divUpload As String
	Protected frmUpload As String
	Protected ImageFolder As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("STORE")

        hdnImageName = Request("hdnImageName")
        divImageName = Request("divImageName")
        divUpload = Request("divUpload")
        frmUpload = Request("frmUpload")

		If Request("type") = "product" Then
			ImageFolder = "/assets/item/cart/"
		Else
			ImageFolder = "/assets/item/swatch/"
		End If
	End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub

		Dim InnerHTML As String = String.Empty

		If fuImageName.NewFileName <> String.Empty Then
			fuImageName.SaveNewFile()
			Dim ImageName As String = fuImageName.NewFileName
			If Request("type") = "product" Then
				Core.ResizeImage(Server.MapPath("/assets/item/original/" & ImageName), Server.MapPath("/assets/item/cart/" & ImageName), SysParam.GetValue(DB, "StoreItemImageCartWidth"), SysParam.GetValue(DB, "StoreItemImageCartHeight"))
				Core.ResizeImage(Server.MapPath("/assets/item/original/" & ImageName), Server.MapPath("/assets/item/related/" & ImageName), SysParam.GetValue(DB, "StoreItemImageRelatedWidth"), SysParam.GetValue(DB, "StoreItemImageRelatedHeight"))
				Core.ResizeImage(Server.MapPath("/assets/item/original/" & ImageName), Server.MapPath("/assets/item/featured/" & ImageName), SysParam.GetValue(DB, "StoreItemImageFeaturedWidth"), SysParam.GetValue(DB, "StoreItemImageFeaturedHeight"))
				Core.ResizeImage(Server.MapPath("/assets/item/original/" & ImageName), Server.MapPath("/assets/item/regular/" & ImageName), SysParam.GetValue(DB, "StoreItemImageRegularWidth"), SysParam.GetValue(DB, "StoreItemImageRegularHeight"))
				Core.ResizeImage(Server.MapPath("/assets/item/original/" & ImageName), Server.MapPath("/assets/item/large/" & ImageName), SysParam.GetValue(DB, "StoreItemImageLargeWidth"), SysParam.GetValue(DB, "StoreItemImageLargeHeight"))
				Core.ResizeImage(Server.MapPath("/assets/item/original/" & ImageName), Server.MapPath("/assets/item/thumbnail/" & ImageName), SysParam.GetValue(DB, "StoreItemImageThumbnailWidth"), SysParam.GetValue(DB, "StoreItemImageThumbnailHeight"))
			Else
				Core.ResizeImage(Server.MapPath("/assets/item/original/" & ImageName), Server.MapPath("/assets/item/swatch/" & ImageName), SysParam.GetValue(DB, "StoreItemSwatchWidth"), SysParam.GetValue(DB, "StoreItemSwatchHeight"))
				Core.ResizeImage(Server.MapPath("/assets/item/original/" & ImageName), Server.MapPath("/assets/item/swatch/small/" & ImageName), SysParam.GetValue(DB, "StoreItemSwatchSmallWidth"), SysParam.GetValue(DB, "StoreItemSwatchSmallHeight"))
			End If

			ViewState("ImageName") = ImageName
		End If

        pnlUpload.Visible = False
        pnlPreview.Visible = True

		InnerHTML = "<img src=""" & ImageFolder & ViewState("ImageName") & """ />"

        btnSelect.Attributes("OnClick") = "parent.SetImageValues('" & hdnImageName & "'," & Core.Escape(ViewState("ImageName")) & ",'" & divImageName & "'," & Core.Escape(InnerHTML) & "); parent.HideUploadFrame(" & Core.Escape(divUpload) & "," & Core.Escape(frmUpload) & ");"
    End Sub

    Protected Sub btnMakeChanges_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnMakeChanges.Click
        pnlUpload.Visible = True
        pnlPreview.Visible = False
    End Sub
End Class
