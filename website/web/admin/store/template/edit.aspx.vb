Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected TemplateId As Integer
	Private IsInUse As Boolean

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("STORE")

        TemplateId = Convert.ToInt32(Request("TemplateId"))

		If TemplateId > 0 Then
		    IsInUse = Not DB.ExecuteScalar("select top 1 itemid from storeitem where templateid = " & TemplateId) = Nothing
		    chkIsAttributes.Enabled = Not IsInUse
        End If

        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If TemplateId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbStoreItemTemplate As StoreItemTemplateRow = StoreItemTemplateRow.GetRow(DB, TemplateId)
        txtTemplateName.Text = dbStoreItemTemplate.TemplateName
        chkIsAttributes.Checked = dbStoreItemTemplate.IsAttributes
        chkIsToAndFrom.Checked = dbStoreItemTemplate.IsToAndFrom
        chkIsGiftMessage.Checked = dbStoreItemTemplate.IsGiftMessage
		drpDisplayMode.SelectedValue = dbStoreItemTemplate.DisplayMode
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbStoreItemTemplate As StoreItemTemplateRow

            If TemplateId <> 0 Then
                dbStoreItemTemplate = StoreItemTemplateRow.GetRow(DB, TemplateId)
            Else
                dbStoreItemTemplate = New StoreItemTemplateRow(DB)
            End If
            dbStoreItemTemplate.TemplateName = txtTemplateName.Text
            dbStoreItemTemplate.IsAttributes = chkIsAttributes.Checked
            dbStoreItemTemplate.IsToAndFrom = chkIsToAndFrom.Checked
            dbStoreItemTemplate.IsGiftMessage = chkIsGiftMessage.Checked
			dbStoreItemTemplate.DisplayMode = drpDisplayMode.SelectedValue

			If dbStoreItemTemplate.DisplayMode = "TableLayout" AndAlso DB.ExecuteScalar("select count(*) from storeitemtemplateattribute where parentid is null and templateid = " & TemplateId) > 1 Then
				Throw New ApplicationException("You cannot specify Table Layout on templates with multiple attribute roots.")
			End If

            If TemplateId <> 0 Then
                dbStoreItemTemplate.Update()
            Else
                TemplateId = dbStoreItemTemplate.Insert
            End If

            DB.CommitTransaction()


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
        Response.Redirect("delete.aspx?TemplateId=" & TemplateId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
