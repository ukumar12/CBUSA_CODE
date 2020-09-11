Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected SalesReportID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("SALES_REPORTS")

		SalesReportID = Convert.ToInt32(Request("SalesReportID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		drpVendorID.Datasource = VendorRow.GetList(DB,"CompanyName")
		drpVendorID.DataValueField = "VendorID"
		drpVendorID.DataTextField = "CompanyName"
		drpVendorID.Databind
		drpVendorID.Items.Insert(0, New ListItem("",""))
	
		If SalesReportID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbSalesReport As SalesReportRow = SalesReportRow.GetRow(DB, SalesReportID)
		txtPeriodYear.Text = dbSalesReport.PeriodYear
		txtPeriodQuarter.Text = dbSalesReport.PeriodQuarter
		txtCreatorVendorAccountID.Text = dbSalesReport.CreatorVendorAccountID
		txtSubmitterVendorAccountID.Text = dbSalesReport.SubmitterVendorAccountID
		drpVendorID.SelectedValue = dbSalesReport.VendorID
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbSalesReport As SalesReportRow

			If SalesReportID <> 0 Then
				dbSalesReport = SalesReportRow.GetRow(DB, SalesReportID)
			Else
				dbSalesReport = New SalesReportRow(DB)
			End If
			dbSalesReport.PeriodYear = txtPeriodYear.Text
			dbSalesReport.PeriodQuarter = txtPeriodQuarter.Text
			dbSalesReport.CreatorVendorAccountID = txtCreatorVendorAccountID.Text
			dbSalesReport.SubmitterVendorAccountID = txtSubmitterVendorAccountID.Text
			dbSalesReport.VendorID = drpVendorID.SelectedValue
	
			If SalesReportID <> 0 Then
				dbSalesReport.Update()
			Else
				SalesReportID = dbSalesReport.Insert
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
		Response.Redirect("delete.aspx?SalesReportID=" & SalesReportID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
