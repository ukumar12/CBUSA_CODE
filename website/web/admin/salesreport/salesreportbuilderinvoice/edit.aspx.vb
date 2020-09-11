Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected SalesReportBuilderInvoiceID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("SALES_REPORTS")

		SalesReportBuilderInvoiceID = Convert.ToInt32(Request("SalesReportBuilderInvoiceID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		drpBuilderID.Datasource = BuilderRow.GetList(DB,"CompanyName")
		drpBuilderID.DataValueField = "BuilderID"
		drpBuilderID.DataTextField = "CompanyName"
		drpBuilderID.Databind
		drpBuilderID.Items.Insert(0, New ListItem("",""))
	
		If SalesReportBuilderInvoiceID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbSalesReportBuilderInvoice As SalesReportBuilderInvoiceRow = SalesReportBuilderInvoiceRow.GetRow(DB, SalesReportBuilderInvoiceID)
		txtSalesReportID.Text = dbSalesReportBuilderInvoice.SalesReportID
		txtInvoiceAmount.Text = dbSalesReportBuilderInvoice.InvoiceAmount
		txtInvoiceNumber.Text = dbSalesReportBuilderInvoice.InvoiceNumber
		txtCreatorVendorAccountID.Text = dbSalesReportBuilderInvoice.CreatorVendorAccountID
		dtInvoiceDate.Value = dbSalesReportBuilderInvoice.InvoiceDate
		drpBuilderID.SelectedValue = dbSalesReportBuilderInvoice.BuilderID
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbSalesReportBuilderInvoice As SalesReportBuilderInvoiceRow

			If SalesReportBuilderInvoiceID <> 0 Then
				dbSalesReportBuilderInvoice = SalesReportBuilderInvoiceRow.GetRow(DB, SalesReportBuilderInvoiceID)
			Else
				dbSalesReportBuilderInvoice = New SalesReportBuilderInvoiceRow(DB)
			End If
			dbSalesReportBuilderInvoice.SalesReportID = txtSalesReportID.Text
			dbSalesReportBuilderInvoice.InvoiceAmount = txtInvoiceAmount.Text
			dbSalesReportBuilderInvoice.InvoiceNumber = txtInvoiceNumber.Text
			dbSalesReportBuilderInvoice.CreatorVendorAccountID = txtCreatorVendorAccountID.Text
			dbSalesReportBuilderInvoice.InvoiceDate = dtInvoiceDate.Value
			dbSalesReportBuilderInvoice.BuilderID = drpBuilderID.SelectedValue
	
			If SalesReportBuilderInvoiceID <> 0 Then
				dbSalesReportBuilderInvoice.Update()
			Else
				SalesReportBuilderInvoiceID = dbSalesReportBuilderInvoice.Insert
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
		Response.Redirect("delete.aspx?SalesReportBuilderInvoiceID=" & SalesReportBuilderInvoiceID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
