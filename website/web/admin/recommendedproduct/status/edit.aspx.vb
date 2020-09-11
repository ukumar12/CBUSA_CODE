Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected RecommendedProductStatusID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("RECOMMENDED_PRODUCTS")

		RecommendedProductStatusID = Convert.ToInt32(Request("RecommendedProductStatusID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If RecommendedProductStatusID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbRecommendedProductStatus As RecommendedProductStatusRow = RecommendedProductStatusRow.GetRow(DB, RecommendedProductStatusID)
		txtRecommendedProductStatus.Text = dbRecommendedProductStatus.RecommendedProductStatus
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbRecommendedProductStatus As RecommendedProductStatusRow

			If RecommendedProductStatusID <> 0 Then
				dbRecommendedProductStatus = RecommendedProductStatusRow.GetRow(DB, RecommendedProductStatusID)
			Else
				dbRecommendedProductStatus = New RecommendedProductStatusRow(DB)
			End If
			dbRecommendedProductStatus.RecommendedProductStatus = txtRecommendedProductStatus.Text
	
			If RecommendedProductStatusID <> 0 Then
				dbRecommendedProductStatus.Update()
			Else
				RecommendedProductStatusID = dbRecommendedProductStatus.Insert
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
		Response.Redirect("delete.aspx?RecommendedProductStatusID=" & RecommendedProductStatusID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
