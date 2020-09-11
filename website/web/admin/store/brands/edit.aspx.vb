Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected BrandId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("STORE")

        BrandId = Convert.ToInt32(Request("BrandId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If BrandId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbStoreBrand As StoreBrandRow = StoreBrandRow.GetRow(DB, BrandId)
        txtName.Text = dbStoreBrand.Name
        txtPageTitle.Text = dbStoreBrand.PageTitle
        txtMetaDescription.Text = dbStoreBrand.MetaDescription
        txtMetaKeywords.Text = dbStoreBrand.MetaKeywords
        txtCustomURL.Text = dbStoreBrand.CustomURL
        txtDescription.Value = dbStoreBrand.Description
        fuImage.CurrentFileName = dbStoreBrand.Image
        fuLogo.CurrentFileName = dbStoreBrand.Logo
        chkIsActive.Checked = dbStoreBrand.IsActive
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbStoreBrand As StoreBrandRow

            If BrandId <> 0 Then
                dbStoreBrand = StoreBrandRow.GetRow(DB, BrandId)
            Else
                dbStoreBrand = New StoreBrandRow(DB)
            End If

            If Not txtCustomURL.Text = String.Empty AndAlso dbStoreBrand.CustomURL <> txtCustomURL.Text Then
                Dim dbCustomURLHistory As CustomURLHistoryRow = CustomURLHistoryRow.GetFromCustomURL(DB, Me.txtCustomURL.Text)
                If dbCustomURLHistory.CustomURLHistoryId > 0 Then
                    Throw New ApplicationException("Custom URL has been used in the past. For SEO purposes, please use a different custom url. ")
                    Exit Sub
                End If
                If Not URLMappingManager.IsValidFolder(txtCustomURL.Text) Then
                    Throw New ApplicationException("Cannot use a URL rewrite which points to a system folder. Please try another folder")
                End If
                If Not URLMappingManager.IsValidURL(DB, txtCustomURL.Text) Then
                    Throw New ApplicationException("The requested Custom URL is already used. Please provide different URL")
                End If
            End If

            dbStoreBrand.Name = txtName.Text
            dbStoreBrand.PageTitle = txtPageTitle.Text
            dbStoreBrand.MetaDescription = txtMetaDescription.Text
            dbStoreBrand.MetaKeywords = txtMetaKeywords.Text
            dbStoreBrand.CustomURL = txtCustomURL.Text
            dbStoreBrand.Description = txtDescription.Value

            If fuImage.NewFileName <> String.Empty Then
                fuImage.SaveNewFile()
                dbStoreBrand.Image = fuImage.NewFileName
            ElseIf fuImage.MarkedToDelete Then
                dbStoreBrand.Image = Nothing
            End If
            If fuLogo.NewFileName <> String.Empty Then
                fuLogo.SaveNewFile()
                dbStoreBrand.Logo = fuLogo.NewFileName

                Core.ResizeImage(Server.MapPath("/assets/brand/original/" & dbStoreBrand.Logo), Server.MapPath("/assets/brand/thumbnail/" & dbStoreBrand.Logo), SysParam.GetValue(DB, "StoreBrandLogoThumbnailWidth"), SysParam.GetValue(DB, "StoreBrandLogoThumbnailHeight"))

                'save ThumbnailWidth and ThumbnailHeight
                Core.GetImageSize(Server.MapPath("/assets/brand/thumbnail/" & dbStoreBrand.Logo), dbStoreBrand.ThumbnailWidth, dbStoreBrand.ThumbnailHeight)

            ElseIf fuLogo.MarkedToDelete Then
                dbStoreBrand.Logo = Nothing
            End If
            dbStoreBrand.IsActive = chkIsActive.Checked

            If BrandId <> 0 Then
                dbStoreBrand.Update()
            Else
                BrandId = dbStoreBrand.Insert
            End If

            DB.CommitTransaction()

            If fuImage.NewFileName <> String.Empty OrElse fuImage.MarkedToDelete Then fuImage.RemoveOldFile()
            If fuLogo.NewFileName <> String.Empty OrElse fuLogo.MarkedToDelete Then fuLogo.RemoveOldFile()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As ApplicationException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ex.Message)
        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?BrandId=" & BrandId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

