
Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class TwoPriceBuilderTakeOffProductPendingRow
        Inherits TwoPriceBuilderTakeOffProductPendingRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, TwoPriceBuilderTakeOffProductPendingID As Integer)
            MyBase.New(DB, TwoPriceBuilderTakeOffProductPendingID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal TwoPriceBuilderTakeOffProductPendingID As Integer) As TwoPriceBuilderTakeOffProductPendingRow
            Dim row As TwoPriceBuilderTakeOffProductPendingRow

            row = New TwoPriceBuilderTakeOffProductPendingRow(DB, TwoPriceBuilderTakeOffProductPendingID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal TwoPriceBuilderTakeOffProductPendingID As Integer)
            Dim SQL As String

            SQL = "DELETE FROM TwoPriceBuilderTakeOffProductPending WHERE TwoPriceBuilderTakeOffProductPendingID = " & DB.Number(TwoPriceBuilderTakeOffProductPendingID)
            DB.ExecuteSQL(SQL)
        End Sub

        Public Sub Remove()
            RemoveRow(DB, TwoPriceBuilderTakeOffProductPendingID)
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from TwoPriceBuilderTakeOffProductPending"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        Public Shared Function GetAllProductsWithPendingPricing(ByVal DB As Database, ByVal OrderId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            'Dim sql As String = _
            '      "Select * from TwoPriceBuilderTakeOffProductPending tpb inner join Product p on tpb.ProductId=p.ProductId" _
            '  & " where tpb.TwoPriceOrderId=" & DB.Number(OrderId)

            '*******  Line changed by Apala (Medullus) on 05.02.2018 for mGuard#T-1086  *******
            Dim sql As String = "Select tpb.TwoPriceBuilderTakeOffProductPendingID, 'Regular' as ProductType, tpb.ProductId, p.Product, tpb.Quantity, tpb.VendorID, tpb.VendorSKU, tpb.VendorPrice, tpb.PricerequestState " _
                                & " from TwoPriceBuilderTakeOffProductPending tpb inner join Product p on tpb.ProductId = p.ProductId " _
                                & " where tpb.TwoPriceOrderId = " & DB.Number(OrderId) _
                                & " UNION " _
                                & "Select tpb.TwoPriceBuilderTakeOffProductPendingID, 'Special' as ProductType, tpb.SpecialOrderProductID as ProductId, p.SpecialOrderProduct as Product, tpb.Quantity, tpb.VendorID, tpb.VendorSKU, tpb.VendorPrice, tpb.PricerequestState " _
                                & " from TwoPriceBuilderTakeOffProductPending tpb inner join SpecialOrderProduct p on tpb.SpecialOrderProductId=p.SpecialOrderProductID " _
                                & " where tpb.TwoPriceOrderId = " & DB.Number(OrderId)

            'If SortBy <> String.Empty Then
            '    sql &= " order by " & Core.ProtectParam(SortBy & " " & SortOrder)
            'End If
            Return DB.GetDataTable(sql)
        End Function
        '******* Method Add by Debashis (Medullus) on 05.09.2018 for USER STORY- 16464  *******
        Public Shared Function GetAllProductsWithPendingPricingWithCbusaSKU(ByVal DB As Database, ByVal OrderId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim sql As String = "Select tpb.TwoPriceBuilderTakeOffProductPendingID, 'Regular' as ProductType, tpb.ProductId, p.Product, tpb.Quantity, tpb.VendorID, tpb.VendorSKU, tpb.VendorPrice,p.SKU as ProductSku, tpb.PricerequestState " _
                                & " from TwoPriceBuilderTakeOffProductPending tpb inner join Product p on tpb.ProductId = p.ProductId " _
                                & " where tpb.TwoPriceOrderId = " & DB.Number(OrderId) _
                                & " UNION " _
                                & "Select tpb.TwoPriceBuilderTakeOffProductPendingID, 'Special' as ProductType, tpb.SpecialOrderProductID as ProductId, p.SpecialOrderProduct as Product, tpb.Quantity, tpb.VendorID, tpb.VendorSKU, tpb.VendorPrice, '' as ProductSku,tpb.PricerequestState " _
                                & " from TwoPriceBuilderTakeOffProductPending tpb inner join SpecialOrderProduct p on tpb.SpecialOrderProductId=p.SpecialOrderProductID " _
                                & " where tpb.TwoPriceOrderId = " & DB.Number(OrderId)

            Return DB.GetDataTable(sql)
        End Function

        'Custom Methods
        Public Shared Function GetAllPendingPricingProductsWithInitOrPendingStatus(ByVal DB As Database, ByVal OrderId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim sql As String =
                  "Select * from TwoPriceBuilderTakeOffProductPending tpb " _
              & " where tpb.PriceRequestState in (1,2) AND  tpb.TwoPriceOrderId=" & DB.Number(OrderId)

            If SortBy <> String.Empty Then
                sql &= " order by " & Core.ProtectParam(SortBy & " " & SortOrder)
            End If
            Return DB.GetDataTable(sql)
        End Function
    End Class

End Namespace

