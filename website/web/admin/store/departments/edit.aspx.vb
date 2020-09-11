Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports Controls
Imports Components
Imports DataLayer
Imports System.Drawing

Public Class Edit
    Inherits AdminPage

    Private DepartmentId As Integer

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("STORE")

        DepartmentId = Convert.ToInt32(Request("DepartmentId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If DepartmentId = 0 Then Exit Sub

        Dim dbStoreDepartment As StoreDepartmentRow = StoreDepartmentRow.GetRow(DB, DepartmentId)
        Name.Text = dbStoreDepartment.Name
        txtPageTitle.Text = dbStoreDepartment.PageTitle
        txtMetaDescription.Text = dbStoreDepartment.MetaDescription
        txtMetaKeywords.Text = dbStoreDepartment.MetaKeywords
        txtCustomURL.Text = dbStoreDepartment.CustomURL
        fuViewImage.CurrentFileName = dbStoreDepartment.ViewImage
        txtDescription.Value = dbStoreDepartment.Description
		txtViewImageAlt.Text = dbStoreDepartment.ViewImageAlt
    End Sub

    Private Sub Save_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Save.Click
        If Not IsValid Then Exit Sub

        Try
            DB.BeginTransaction()

            Dim dbStoreDepartment As StoreDepartmentRow = StoreDepartmentRow.GetRow(DB, DepartmentId)

            If Not txtCustomURL.Text = String.Empty AndAlso dbStoreDepartment.CustomURL <> txtCustomURL.Text Then
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

            dbStoreDepartment.PageTitle = txtPageTitle.Text
            dbStoreDepartment.MetaKeywords = txtMetaKeywords.Text
            dbStoreDepartment.MetaDescription = txtMetaDescription.Text
            dbStoreDepartment.CustomURL = txtCustomURL.Text
            dbStoreDepartment.Description = txtDescription.Value
			dbStoreDepartment.ViewImageAlt = txtViewImageAlt.Text

            If fuViewImage.NewFileName <> String.Empty Then
                fuViewImage.SaveNewFile()

                dbStoreDepartment.ViewImage = fuViewImage.NewFileName

                Core.ResizeImage(Server.MapPath("/assets/department/original/" & dbStoreDepartment.ViewImage), Server.MapPath("/assets/department/thumbnail/" & dbStoreDepartment.ViewImage), SysParam.GetValue(DB, "StoreDepartmentThumbnailWidth"), SysParam.GetValue(DB, "StoreDepartmentThumbnailHeight"))
                Core.ResizeImage(Server.MapPath("/assets/department/original/" & dbStoreDepartment.ViewImage), Server.MapPath("/assets/department/large/" & dbStoreDepartment.ViewImage), SysParam.GetValue(DB, "StoreDepartmentLargeWidth"), SysParam.GetValue(DB, "StoreDepartmentLargeHeight"))
                Core.ResizeImage(Server.MapPath("/assets/department/original/" & dbStoreDepartment.ViewImage), Server.MapPath("/assets/department/regular/" & dbStoreDepartment.ViewImage), SysParam.GetValue(DB, "StoreDepartmentRegularWidth"), SysParam.GetValue(DB, "StoreDepartmentRegularHeight"))

                'save ThumbnailWidth and ThumbnailHeight
                Core.GetImageSize(Server.MapPath("/assets/department/thumbnail/" & dbStoreDepartment.ViewImage), dbStoreDepartment.ThumbnailWidth, dbStoreDepartment.ThumbnailHeight)

            ElseIf fuViewImage.MarkedToDelete Then
                dbStoreDepartment.ViewImage = Nothing
            End If

            dbStoreDepartment.Update()
            DB.CommitTransaction()

            'Remove old files
            If fuViewImage.NewFileName <> String.Empty Or fuViewImage.MarkedToDelete Then
                fuViewImage.RemoveOldFile()
                fuViewImage.RemoveOldFile("/assets/department/regular/")
                fuViewImage.RemoveOldFile("/assets/department/large/")
            End If

            ' Redirect on complete
            Response.Redirect("default.aspx?DepartmentId=" & DepartmentId)

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))

        Catch ex As ApplicationException
            AddError(ex.Message)
        End Try
    End Sub

    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Response.Redirect("default.aspx?DepartmentId=" & DepartmentId)
    End Sub
End Class