Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

    Protected VendorID As Integer
    Protected TakeOffProductID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("VENDOR_TAKE_OFF_PRODUCT_SUBSTITUTES")

        VendorID = Convert.ToInt32(Request("VendorID"))
        TakeOffProductID = Convert.ToInt32(Request("TakeOffProductID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		drpTakeOffProductID.Datasource = TakeOffProductRow.GetList(DB,"")
		drpTakeOffProductID.DataValueField = "TakeOffProductID"
		drpTakeOffProductID.DataTextField = ""
		drpTakeOffProductID.Databind
		drpTakeOffProductID.Items.Insert(0, New ListItem("",""))
	
		drpSubstituteProductID.Datasource = ProductRow.GetList(DB,"Product")
		drpSubstituteProductID.DataValueField = "ProductID"
		drpSubstituteProductID.DataTextField = "Product"
		drpSubstituteProductID.Databind
		drpSubstituteProductID.Items.Insert(0, New ListItem("",""))
	
		drpCreatorVendorAccountID.Datasource = VendorAccountRow.GetList(DB,"Username")
		drpCreatorVendorAccountID.DataValueField = "VendorAccountID"
		drpCreatorVendorAccountID.DataTextField = "Username"
		drpCreatorVendorAccountID.Databind
		drpCreatorVendorAccountID.Items.Insert(0, New ListItem("",""))
	
		If VendorID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

        Dim dbVendorTakeOffProductSubstitute As VendorTakeOffProductSubstituteRow = VendorTakeOffProductSubstituteRow.GetRow(DB, VendorID, TakeoffProductID)
		txtRecommendedQuantity.Text = dbVendorTakeOffProductSubstitute.RecommendedQuantity
		drpTakeOffProductID.SelectedValue = dbVendorTakeOffProductSubstitute.TakeOffProductID
		drpSubstituteProductID.SelectedValue = dbVendorTakeOffProductSubstitute.SubstituteProductID
		drpCreatorVendorAccountID.SelectedValue = dbVendorTakeOffProductSubstitute.CreatorVendorAccountID
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbVendorTakeOffProductSubstitute As VendorTakeOffProductSubstituteRow

			If VendorID <> 0 Then
                dbVendorTakeOffProductSubstitute = VendorTakeOffProductSubstituteRow.GetRow(DB, VendorID, TakeoffProductID)
			Else
				dbVendorTakeOffProductSubstitute = New VendorTakeOffProductSubstituteRow(DB)
			End If
			dbVendorTakeOffProductSubstitute.RecommendedQuantity = txtRecommendedQuantity.Text
			dbVendorTakeOffProductSubstitute.TakeOffProductID = drpTakeOffProductID.SelectedValue
			dbVendorTakeOffProductSubstitute.SubstituteProductID = drpSubstituteProductID.SelectedValue
			dbVendorTakeOffProductSubstitute.CreatorVendorAccountID = drpCreatorVendorAccountID.SelectedValue
	
			If VendorID <> 0 Then
				dbVendorTakeOffProductSubstitute.Update()
			Else
				VendorID = dbVendorTakeOffProductSubstitute.Insert
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
		Response.Redirect("delete.aspx?VendorID=" & VendorID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
