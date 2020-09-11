Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected PurchasesReportVendorPOID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("PURCHASES_REPORT_VENDOR_POS")

		PurchasesReportVendorPOID = Convert.ToInt32(Request("PurchasesReportVendorPOID"))
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
	
		If PurchasesReportVendorPOID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbPurchasesReportVendorPO As PurchasesReportVendorPORow = PurchasesReportVendorPORow.GetRow(DB, PurchasesReportVendorPOID)
		txtPurchasesReportID.Text = dbPurchasesReportVendorPO.PurchasesReportID
		txtPOAmount.Text = dbPurchasesReportVendorPO.POAmount
		txtPONumber.Text = dbPurchasesReportVendorPO.PONumber
		txtCreatorBuilderAccountID.Text = dbPurchasesReportVendorPO.CreatorBuilderAccountID
		dtPODate.Value = dbPurchasesReportVendorPO.PODate
		drpVendorID.SelectedValue = dbPurchasesReportVendorPO.VendorID
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbPurchasesReportVendorPO As PurchasesReportVendorPORow

			If PurchasesReportVendorPOID <> 0 Then
				dbPurchasesReportVendorPO = PurchasesReportVendorPORow.GetRow(DB, PurchasesReportVendorPOID)
			Else
				dbPurchasesReportVendorPO = New PurchasesReportVendorPORow(DB)
			End If
			dbPurchasesReportVendorPO.PurchasesReportID = txtPurchasesReportID.Text
			dbPurchasesReportVendorPO.POAmount = txtPOAmount.Text
			dbPurchasesReportVendorPO.PONumber = txtPONumber.Text
			dbPurchasesReportVendorPO.CreatorBuilderAccountID = txtCreatorBuilderAccountID.Text
			dbPurchasesReportVendorPO.PODate = dtPODate.Value
			dbPurchasesReportVendorPO.VendorID = drpVendorID.SelectedValue
	
			If PurchasesReportVendorPOID <> 0 Then
				dbPurchasesReportVendorPO.Update()
			Else
				PurchasesReportVendorPOID = dbPurchasesReportVendorPO.Insert
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
		Response.Redirect("delete.aspx?PurchasesReportVendorPOID=" & PurchasesReportVendorPOID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
