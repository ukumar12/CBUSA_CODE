Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("MARKET_INDICATORS")

        Dim dbAdminIndicatorVendor As DataLayer.AdminIndicatorVendorRow
        Dim dbVendor As DataLayer.VendorRow

        Dim dbAdminIndicatorProduct As DataLayer.AdminIndicatorProductRow
        Dim dbProduct As DataLayer.ProductRow

        If Not IsPostBack Then

            dbAdminIndicatorVendor = DataLayer.AdminIndicatorVendorRow.GetRowByIndex(Me.DB, 1)
            If dbAdminIndicatorVendor.VendorID <> 0 Then
                dbVendor = DataLayer.VendorRow.GetRow(Me.DB, dbAdminIndicatorVendor.VendorID)
                Me.ltrVendor1.Text = dbVendor.CompanyName
            End If

            dbAdminIndicatorVendor = DataLayer.AdminIndicatorVendorRow.GetRowByIndex(Me.DB, 2)
            If dbAdminIndicatorVendor.VendorID <> 0 Then
                dbVendor = DataLayer.VendorRow.GetRow(Me.DB, dbAdminIndicatorVendor.VendorID)
                Me.ltrVendor2.Text = dbVendor.CompanyName
            End If

            dbAdminIndicatorVendor = DataLayer.AdminIndicatorVendorRow.GetRowByIndex(Me.DB, 3)
            If dbAdminIndicatorVendor.VendorID <> 0 Then
                dbVendor = DataLayer.VendorRow.GetRow(Me.DB, dbAdminIndicatorVendor.VendorID)
                Me.ltrVendor3.Text = dbVendor.CompanyName
            End If

            dbAdminIndicatorProduct = DataLayer.AdminIndicatorProductRow.GetRowByIndex(Me.DB, 1)
            If dbAdminIndicatorProduct.ProductID <> 0 Then
                dbProduct = DataLayer.ProductRow.GetRow(Me.DB, dbAdminIndicatorProduct.ProductID)
                Me.ltrProduct1.Text = dbProduct.Product
            End If

            dbAdminIndicatorProduct = DataLayer.AdminIndicatorProductRow.GetRowByIndex(Me.DB, 2)
            If dbAdminIndicatorProduct.ProductID <> 0 Then
                dbProduct = DataLayer.ProductRow.GetRow(Me.DB, dbAdminIndicatorProduct.ProductID)
                Me.ltrProduct2.Text = dbProduct.Product
            End If

            dbAdminIndicatorProduct = DataLayer.AdminIndicatorProductRow.GetRowByIndex(Me.DB, 3)
            If dbAdminIndicatorProduct.ProductID <> 0 Then
                dbProduct = DataLayer.ProductRow.GetRow(Me.DB, dbAdminIndicatorProduct.ProductID)
                Me.ltrProduct3.Text = dbProduct.Product
            End If

            dbAdminIndicatorProduct = DataLayer.AdminIndicatorProductRow.GetRowByIndex(Me.DB, 4)
            If dbAdminIndicatorProduct.ProductID <> 0 Then
                dbProduct = DataLayer.ProductRow.GetRow(Me.DB, dbAdminIndicatorProduct.ProductID)
                Me.ltrProduct4.Text = dbProduct.Product
            End If

            dbAdminIndicatorProduct = DataLayer.AdminIndicatorProductRow.GetRowByIndex(Me.DB, 5)
            If dbAdminIndicatorProduct.ProductID <> 0 Then
                dbProduct = DataLayer.ProductRow.GetRow(Me.DB, dbAdminIndicatorProduct.ProductID)
                Me.ltrProduct5.Text = dbProduct.Product
            End If

        End If


    End Sub

    Protected Sub btnEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEdit.Click
        Response.Redirect("edit.aspx")
    End Sub

End Class
