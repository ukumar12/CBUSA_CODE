Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected VendorProductPriceRequestID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("VENDOR_PRODUCT_PRICE_REQUESTS")

        VendorProductPriceRequestID = Convert.ToInt32(Request("VendorProductPriceRequestID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        drpVendorID.Datasource = VendorRow.GetList(DB, "CompanyName")
        drpVendorID.DataValueField = "VendorID"
        drpVendorID.DataTextField = "CompanyName"
        drpVendorID.Databind()
        drpVendorID.Items.Insert(0, New ListItem("", ""))

        drpBuilderID.Datasource = BuilderRow.GetList(DB, "CompanyName")
        drpBuilderID.DataValueField = "BuilderID"
        drpBuilderID.DataTextField = "CompanyName"
        drpBuilderID.Databind()
        drpBuilderID.Items.Insert(0, New ListItem("", ""))

        drpProductID.Datasource = ProductRow.GetList(DB, "Product")
        drpProductID.DataValueField = "ProductID"
        drpProductID.DataTextField = "Product"
        drpProductID.Databind()
        drpProductID.Items.Insert(0, New ListItem("", ""))

        drpSpecialOrderProductID.Datasource = SpecialOrderProductRow.GetList(DB, "SpecialOrderProduct")
        drpSpecialOrderProductID.DataValueField = "SpecialOrderProductID"
        drpSpecialOrderProductID.DataTextField = "SpecialOrderProduct"
        drpSpecialOrderProductID.Databind()
        drpSpecialOrderProductID.Items.Insert(0, New ListItem("", ""))

        If VendorProductPriceRequestID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbVendorProductPriceRequest As VendorProductPriceRequestRow = VendorProductPriceRequestRow.GetRow(DB, VendorProductPriceRequestID)
        txtCreatorBuilderAccountID.Text = dbVendorProductPriceRequest.CreatorBuilderAccountID
        drpVendorID.SelectedValue = dbVendorProductPriceRequest.VendorID
        drpBuilderID.SelectedValue = dbVendorProductPriceRequest.BuilderID
        drpProductID.SelectedValue = dbVendorProductPriceRequest.ProductID
        drpSpecialOrderProductID.SelectedValue = dbVendorProductPriceRequest.SpecialOrderProductID
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbVendorProductPriceRequest As VendorProductPriceRequestRow

            If VendorProductPriceRequestID <> 0 Then
                dbVendorProductPriceRequest = VendorProductPriceRequestRow.GetRow(DB, VendorProductPriceRequestID)
            Else
                dbVendorProductPriceRequest = New VendorProductPriceRequestRow(DB)
            End If
            dbVendorProductPriceRequest.CreatorBuilderAccountID = txtCreatorBuilderAccountID.Text
            dbVendorProductPriceRequest.VendorID = drpVendorID.SelectedValue
            dbVendorProductPriceRequest.BuilderID = drpBuilderID.SelectedValue
            dbVendorProductPriceRequest.ProductID = drpProductID.SelectedValue
            dbVendorProductPriceRequest.SpecialOrderProductID = drpSpecialOrderProductID.SelectedValue

            If VendorProductPriceRequestID <> 0 Then
                dbVendorProductPriceRequest.Update()
            Else
                VendorProductPriceRequestID = dbVendorProductPriceRequest.Insert
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
        Response.Redirect("delete.aspx?VendorProductPriceRequestID=" & VendorProductPriceRequestID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

