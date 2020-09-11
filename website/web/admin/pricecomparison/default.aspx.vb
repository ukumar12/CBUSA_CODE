Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("PRICE_COMPARISONS")

        Dim dbAdminComparisonVendor As DataLayer.AdminComparisonVendorRow
        Dim dbVendor As DataLayer.VendorRow

        Dim dbAdminComparisonOrder As DataLayer.AdminComparisonOrderRow
        Dim dbOrder As DataLayer.OrderRow

        If Not IsPostBack Then

            dbAdminComparisonVendor = DataLayer.AdminComparisonVendorRow.GetRowByIndex(Me.DB, 1)
            If dbAdminComparisonVendor.VendorID <> 0 Then
                dbVendor = DataLayer.VendorRow.GetRow(Me.DB, dbAdminComparisonVendor.VendorID)
                Me.ltrVendor1.Text = dbVendor.CompanyName
            End If

            dbAdminComparisonVendor = DataLayer.AdminComparisonVendorRow.GetRowByIndex(Me.DB, 2)
            If dbAdminComparisonVendor.VendorID <> 0 Then
                dbVendor = DataLayer.VendorRow.GetRow(Me.DB, dbAdminComparisonVendor.VendorID)
                Me.ltrVendor2.Text = dbVendor.CompanyName
            End If

            dbAdminComparisonVendor = DataLayer.AdminComparisonVendorRow.GetRowByIndex(Me.DB, 3)
            If dbAdminComparisonVendor.VendorID <> 0 Then
                dbVendor = DataLayer.VendorRow.GetRow(Me.DB, dbAdminComparisonVendor.VendorID)
                Me.ltrVendor3.Text = dbVendor.CompanyName
            End If

            dbAdminComparisonOrder = DataLayer.AdminComparisonOrderRow.GetRowByIndex(Me.DB, 1)
            If dbAdminComparisonOrder.OrderID <> 0 Then
                dbOrder = DataLayer.OrderRow.GetRow(Me.DB, dbAdminComparisonOrder.OrderID)
                Me.ltrOrder1.Text = dbOrder.Title
            End If

            dbAdminComparisonOrder = DataLayer.AdminComparisonOrderRow.GetRowByIndex(Me.DB, 2)
            If dbAdminComparisonOrder.OrderID <> 0 Then
                dbOrder = DataLayer.OrderRow.GetRow(Me.DB, dbAdminComparisonOrder.OrderID)
                Me.ltrOrder2.Text = dbOrder.Title
            End If

            dbAdminComparisonOrder = DataLayer.AdminComparisonOrderRow.GetRowByIndex(Me.DB, 3)
            If dbAdminComparisonOrder.OrderID <> 0 Then
                dbOrder = DataLayer.OrderRow.GetRow(Me.DB, dbAdminComparisonOrder.OrderID)
                Me.ltrOrder3.Text = dbOrder.Title
            End If

            dbAdminComparisonOrder = DataLayer.AdminComparisonOrderRow.GetRowByIndex(Me.DB, 4)
            If dbAdminComparisonOrder.OrderID <> 0 Then
                dbOrder = DataLayer.OrderRow.GetRow(Me.DB, dbAdminComparisonOrder.OrderID)
                Me.ltrOrder4.Text = dbOrder.Title
            End If

            dbAdminComparisonOrder = DataLayer.AdminComparisonOrderRow.GetRowByIndex(Me.DB, 5)
            If dbAdminComparisonOrder.OrderID <> 0 Then
                dbOrder = DataLayer.OrderRow.GetRow(Me.DB, dbAdminComparisonOrder.OrderID)
                Me.ltrOrder5.Text = dbOrder.Title
            End If

        End If


    End Sub

    Protected Sub btnEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEdit.Click
        Response.Redirect("edit.aspx")
    End Sub

End Class
