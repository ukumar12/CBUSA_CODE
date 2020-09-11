Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected FaqCategoryId As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("FAQ")

		FaqCategoryId = Convert.ToInt32(Request("FaqCategoryId"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		drpAdminId.Datasource = AdminRow.GetAllAdmins(DB)
		drpAdminId.DataValueField = "AdminId"
		drpAdminId.DataTextField = "Username"
		drpAdminId.Databind()
		drpAdminId.Items.Insert(0, New ListItem("", "0"))

		If FaqCategoryId = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End If

		Dim dbFaqCategory As FaqCategoryRow = FaqCategoryRow.GetRow(DB, FaqCategoryId)
		txtCategoryName.Text = dbFaqCategory.CategoryName
		drpAdminId.SelectedValue = dbFaqCategory.AdminId
		rblIsActive.SelectedValue = dbFaqCategory.IsActive
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbFaqCategory As FaqCategoryRow

			If FaqCategoryId <> 0 Then
				dbFaqCategory = FaqCategoryRow.GetRow(DB, FaqCategoryId)
			Else
				dbFaqCategory = New FaqCategoryRow(DB)
			End If
			dbFaqCategory.CategoryName = txtCategoryName.Text
			dbFaqCategory.AdminId = drpAdminId.SelectedValue
			dbFaqCategory.IsActive = rblIsActive.SelectedValue

			If FaqCategoryId <> 0 Then
				dbFaqCategory.Update()
			Else
				FaqCategoryId = dbFaqCategory.Insert
			End If

			DB.CommitTransaction()


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
		Response.Redirect("delete.aspx?FaqCategoryId=" & FaqCategoryId & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class

