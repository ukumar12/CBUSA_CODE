Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("PRICE_COMPARISONS")

        Dim dbAdminComparisonVendor As DataLayer.AdminComparisonVendorRow
        Dim dbVendor As DataLayer.VendorRow

        Dim dbAdminComparisonOrder As DataLayer.AdminComparisonOrderRow
        Dim dbOrder As DataLayer.OrderRow

        If Not IsPostBack Then

            dbAdminComparisonVendor = DataLayer.AdminComparisonVendorRow.GetRowByIndex(Me.DB, 1)
            If dbAdminComparisonVendor.VendorID <> 0 Then
                dbVendor = DataLayer.VendorRow.GetRow(Me.DB, dbAdminComparisonVendor.VendorID)
                Me.acdVendorID1.LoadValue = dbAdminComparisonVendor.VendorID.ToString
                Me.acdVendorID1.Text = dbVendor.CompanyName
            End If

            dbAdminComparisonVendor = DataLayer.AdminComparisonVendorRow.GetRowByIndex(Me.DB, 2)
            If dbAdminComparisonVendor.VendorID <> 0 Then
                dbVendor = DataLayer.VendorRow.GetRow(Me.DB, dbAdminComparisonVendor.VendorID)
                Me.acdVendorID2.LoadValue = dbAdminComparisonVendor.VendorID.ToString
                Me.acdVendorID2.Text = dbVendor.CompanyName
            End If

            dbAdminComparisonVendor = DataLayer.AdminComparisonVendorRow.GetRowByIndex(Me.DB, 3)
            If dbAdminComparisonVendor.VendorID <> 0 Then
                dbVendor = DataLayer.VendorRow.GetRow(Me.DB, dbAdminComparisonVendor.VendorID)
                Me.acdVendorID3.LoadValue = dbAdminComparisonVendor.VendorID.ToString
                Me.acdVendorID3.Text = dbVendor.CompanyName
            End If

            dbAdminComparisonOrder = DataLayer.AdminComparisonOrderRow.GetRowByIndex(Me.DB, 1)
            If dbAdminComparisonOrder.OrderID <> 0 Then
                dbOrder = DataLayer.OrderRow.GetRow(Me.DB, dbAdminComparisonOrder.OrderID)
                Me.acdOrderID1.LoadValue = dbAdminComparisonOrder.OrderID.ToString
                Me.acdOrderID1.Text = dbOrder.Title
            End If

            dbAdminComparisonOrder = DataLayer.AdminComparisonOrderRow.GetRowByIndex(Me.DB, 2)
            If dbAdminComparisonOrder.OrderID <> 0 Then
                dbOrder = DataLayer.OrderRow.GetRow(Me.DB, dbAdminComparisonOrder.OrderID)
                Me.acdOrderID2.LoadValue = dbAdminComparisonOrder.OrderID.ToString
                Me.acdOrderID2.Text = dbOrder.Title
            End If

            dbAdminComparisonOrder = DataLayer.AdminComparisonOrderRow.GetRowByIndex(Me.DB, 3)
            If dbAdminComparisonOrder.OrderID <> 0 Then
                dbOrder = DataLayer.OrderRow.GetRow(Me.DB, dbAdminComparisonOrder.OrderID)
                Me.acdOrderID3.LoadValue = dbAdminComparisonOrder.OrderID.ToString
                Me.acdOrderID3.Text = dbOrder.Title
            End If

            dbAdminComparisonOrder = DataLayer.AdminComparisonOrderRow.GetRowByIndex(Me.DB, 4)
            If dbAdminComparisonOrder.OrderID <> 0 Then
                dbOrder = DataLayer.OrderRow.GetRow(Me.DB, dbAdminComparisonOrder.OrderID)
                Me.acdOrderID4.LoadValue = dbAdminComparisonOrder.OrderID.ToString
                Me.acdOrderID4.Text = dbOrder.Title
            End If

            dbAdminComparisonOrder = DataLayer.AdminComparisonOrderRow.GetRowByIndex(Me.DB, 5)
            If dbAdminComparisonOrder.OrderID <> 0 Then
                dbOrder = DataLayer.OrderRow.GetRow(Me.DB, dbAdminComparisonOrder.OrderID)
                Me.acdOrderID5.LoadValue = dbAdminComparisonOrder.OrderID.ToString
                Me.acdOrderID5.Text = dbOrder.Title
            End If

        End If

    End Sub


    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbAdminComparisonVendor As DataLayer.AdminComparisonVendorRow
            Dim dbAdminComparisonOrder As DataLayer.AdminComparisonOrderRow

            DataLayer.AdminComparisonOrderRow.DeleteAll(Me.DB)
            DataLayer.AdminComparisonVendorRow.DeleteAll(Me.DB)

            If IsNumeric(Me.acdVendorID1.Value) And Me.acdVendorID1.Text <> String.Empty Then
                dbAdminComparisonVendor = New DataLayer.AdminComparisonVendorRow(Me.DB)
                dbAdminComparisonVendor.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
                dbAdminComparisonVendor.VendorID = Me.acdVendorID1.Value
                dbAdminComparisonVendor.Insert()
            End If

            If IsNumeric(Me.acdVendorID2.Value) And Me.acdVendorID2.Text <> String.Empty Then
                dbAdminComparisonVendor = New DataLayer.AdminComparisonVendorRow(Me.DB)
                dbAdminComparisonVendor.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
                dbAdminComparisonVendor.VendorID = Me.acdVendorID2.Value
                dbAdminComparisonVendor.Insert()
            End If

            If IsNumeric(Me.acdVendorID3.Value) And Me.acdVendorID3.Text <> String.Empty Then
                dbAdminComparisonVendor = New DataLayer.AdminComparisonVendorRow(Me.DB)
                dbAdminComparisonVendor.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
                dbAdminComparisonVendor.VendorID = Me.acdVendorID3.Value
                dbAdminComparisonVendor.Insert()
            End If

            If IsNumeric(Me.acdOrderID1.Value) And Me.acdOrderID1.Text <> String.Empty Then
                dbAdminComparisonOrder = New DataLayer.AdminComparisonOrderRow(Me.DB)
                dbAdminComparisonOrder.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
                dbAdminComparisonOrder.OrderID = Me.acdOrderID1.Value
                dbAdminComparisonOrder.Insert()
            End If

            If IsNumeric(Me.acdOrderID2.Value) And Me.acdOrderID2.Text <> String.Empty Then
                dbAdminComparisonOrder = New DataLayer.AdminComparisonOrderRow(Me.DB)
                dbAdminComparisonOrder.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
                dbAdminComparisonOrder.OrderID = Me.acdOrderID2.Value
                dbAdminComparisonOrder.Insert()
            End If

            If IsNumeric(Me.acdOrderID3.Value) And Me.acdOrderID3.Text <> String.Empty Then
                dbAdminComparisonOrder = New DataLayer.AdminComparisonOrderRow(Me.DB)
                dbAdminComparisonOrder.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
                dbAdminComparisonOrder.OrderID = Me.acdOrderID3.Value
                dbAdminComparisonOrder.Insert()
            End If

            If IsNumeric(Me.acdOrderID4.Value) And Me.acdOrderID4.Text <> String.Empty Then
                dbAdminComparisonOrder = New DataLayer.AdminComparisonOrderRow(Me.DB)
                dbAdminComparisonOrder.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
                dbAdminComparisonOrder.OrderID = Me.acdOrderID4.Value
                dbAdminComparisonOrder.Insert()
            End If

            If IsNumeric(Me.acdOrderID5.Value) And Me.acdOrderID5.Text <> String.Empty Then
                dbAdminComparisonOrder = New DataLayer.AdminComparisonOrderRow(Me.DB)
                dbAdminComparisonOrder.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
                dbAdminComparisonOrder.OrderID = Me.acdOrderID5.Value
                dbAdminComparisonOrder.Insert()
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
