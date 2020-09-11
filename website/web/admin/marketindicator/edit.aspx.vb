Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("MARKET_INDICATORS")

        Dim dbAdminIndicatorVendor As DataLayer.AdminIndicatorVendorRow
        Dim dbVendor As DataLayer.VendorRow

        Dim dbAdminIndicatorProduct As DataLayer.AdminIndicatorProductRow
        Dim dbProduct As DataLayer.ProductRow

        If Not IsPostBack Then

            dbAdminIndicatorVendor = DataLayer.AdminIndicatorVendorRow.GetRowByIndex(Me.DB, 1)
            If dbAdminIndicatorVendor.VendorID <> 0 Then
                dbVendor = DataLayer.VendorRow.GetRow(Me.DB, dbAdminIndicatorVendor.VendorID)
                Me.acdVendorID1.Value = dbAdminIndicatorVendor.VendorID.ToString
                Me.acdVendorID1.Text = dbVendor.CompanyName
            End If

            dbAdminIndicatorVendor = DataLayer.AdminIndicatorVendorRow.GetRowByIndex(Me.DB, 2)
            If dbAdminIndicatorVendor.VendorID <> 0 Then
                dbVendor = DataLayer.VendorRow.GetRow(Me.DB, dbAdminIndicatorVendor.VendorID)
                Me.acdVendorID2.Value = dbAdminIndicatorVendor.VendorID.ToString
                Me.acdVendorID2.Text = dbVendor.CompanyName
            End If

            dbAdminIndicatorVendor = DataLayer.AdminIndicatorVendorRow.GetRowByIndex(Me.DB, 3)
            If dbAdminIndicatorVendor.VendorID <> 0 Then
                dbVendor = DataLayer.VendorRow.GetRow(Me.DB, dbAdminIndicatorVendor.VendorID)
                Me.acdVendorID3.Value = dbAdminIndicatorVendor.VendorID.ToString
                Me.acdVendorID3.Text = dbVendor.CompanyName
            End If

            dbAdminIndicatorProduct = DataLayer.AdminIndicatorProductRow.GetRowByIndex(Me.DB, 1)
            If dbAdminIndicatorProduct.ProductID <> 0 Then
                dbProduct = DataLayer.ProductRow.GetRow(Me.DB, dbAdminIndicatorProduct.ProductID)
                Me.acdProductID1.Value = dbAdminIndicatorProduct.ProductID.ToString
                Me.acdProductID1.Text = dbProduct.Product
            End If

            dbAdminIndicatorProduct = DataLayer.AdminIndicatorProductRow.GetRowByIndex(Me.DB, 2)
            If dbAdminIndicatorProduct.ProductID <> 0 Then
                dbProduct = DataLayer.ProductRow.GetRow(Me.DB, dbAdminIndicatorProduct.ProductID)
                Me.acdProductID2.Value = dbAdminIndicatorProduct.ProductID.ToString
                Me.acdProductID2.Text = dbProduct.Product
            End If

            dbAdminIndicatorProduct = DataLayer.AdminIndicatorProductRow.GetRowByIndex(Me.DB, 3)
            If dbAdminIndicatorProduct.ProductID <> 0 Then
                dbProduct = DataLayer.ProductRow.GetRow(Me.DB, dbAdminIndicatorProduct.ProductID)
                Me.acdProductID3.Value = dbAdminIndicatorProduct.ProductID.ToString
                Me.acdProductID3.Text = dbProduct.Product
            End If

            dbAdminIndicatorProduct = DataLayer.AdminIndicatorProductRow.GetRowByIndex(Me.DB, 4)
            If dbAdminIndicatorProduct.ProductID <> 0 Then
                dbProduct = DataLayer.ProductRow.GetRow(Me.DB, dbAdminIndicatorProduct.ProductID)
                Me.acdProductID4.Value = dbAdminIndicatorProduct.ProductID.ToString
                Me.acdProductID4.Text = dbProduct.Product
            End If

            dbAdminIndicatorProduct = DataLayer.AdminIndicatorProductRow.GetRowByIndex(Me.DB, 5)
            If dbAdminIndicatorProduct.ProductID <> 0 Then
                dbProduct = DataLayer.ProductRow.GetRow(Me.DB, dbAdminIndicatorProduct.ProductID)
                Me.acdProductID5.Value = dbAdminIndicatorProduct.ProductID.ToString
                Me.acdProductID5.Text = dbProduct.Product
            End If

        End If

    End Sub


    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbAdminIndicatorVendor As DataLayer.AdminIndicatorVendorRow
            Dim dbAdminIndicatorProduct As DataLayer.AdminIndicatorProductRow

            DataLayer.AdminIndicatorProductRow.DeleteAll(Me.DB)
            DataLayer.AdminIndicatorVendorRow.DeleteAll(Me.DB)

            If IsNumeric(Me.acdVendorID1.Value) And Me.acdVendorID1.Text <> String.Empty Then
                dbAdminIndicatorVendor = New DataLayer.AdminIndicatorVendorRow(Me.DB)
                dbAdminIndicatorVendor.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
                dbAdminIndicatorVendor.VendorID = Me.acdVendorID1.Value
                dbAdminIndicatorVendor.Insert()
            End If

            If IsNumeric(Me.acdVendorID2.Value) And Me.acdVendorID2.Text <> String.Empty Then
                dbAdminIndicatorVendor = New DataLayer.AdminIndicatorVendorRow(Me.DB)
                dbAdminIndicatorVendor.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
                dbAdminIndicatorVendor.VendorID = Me.acdVendorID2.Value
                dbAdminIndicatorVendor.Insert()
            End If

            If IsNumeric(Me.acdVendorID3.Value) And Me.acdVendorID3.Text <> String.Empty Then
                dbAdminIndicatorVendor = New DataLayer.AdminIndicatorVendorRow(Me.DB)
                dbAdminIndicatorVendor.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
                dbAdminIndicatorVendor.VendorID = Me.acdVendorID3.Value
                dbAdminIndicatorVendor.Insert()
            End If

            If IsNumeric(Me.acdProductID1.Value) And Me.acdProductID1.Text <> String.Empty Then
                dbAdminIndicatorProduct = New DataLayer.AdminIndicatorProductRow(Me.DB)
                dbAdminIndicatorProduct.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
                dbAdminIndicatorProduct.ProductID = Me.acdProductID1.Value
                dbAdminIndicatorProduct.Insert()
            End If

            If IsNumeric(Me.acdProductID2.Value) And Me.acdProductID2.Text <> String.Empty Then
                dbAdminIndicatorProduct = New DataLayer.AdminIndicatorProductRow(Me.DB)
                dbAdminIndicatorProduct.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
                dbAdminIndicatorProduct.ProductID = Me.acdProductID2.Value
                dbAdminIndicatorProduct.Insert()
            End If

            If IsNumeric(Me.acdProductID3.Value) And Me.acdProductID3.Text <> String.Empty Then
                dbAdminIndicatorProduct = New DataLayer.AdminIndicatorProductRow(Me.DB)
                dbAdminIndicatorProduct.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
                dbAdminIndicatorProduct.ProductID = Me.acdProductID3.Value
                dbAdminIndicatorProduct.Insert()
            End If

            If IsNumeric(Me.acdProductID4.Value) And Me.acdProductID4.Text <> String.Empty Then
                dbAdminIndicatorProduct = New DataLayer.AdminIndicatorProductRow(Me.DB)
                dbAdminIndicatorProduct.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
                dbAdminIndicatorProduct.ProductID = Me.acdProductID4.Value
                dbAdminIndicatorProduct.Insert()
            End If

            If IsNumeric(Me.acdProductID5.Value) And Me.acdProductID5.Text <> String.Empty Then
                dbAdminIndicatorProduct = New DataLayer.AdminIndicatorProductRow(Me.DB)
                dbAdminIndicatorProduct.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
                dbAdminIndicatorProduct.ProductID = Me.acdProductID5.Value
                dbAdminIndicatorProduct.Insert()
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

End Class
