Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class OrderProductRow
        Inherits OrderProductRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal OrderProductID As Integer)
            MyBase.New(DB, OrderProductID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal OrderProductID As Integer) As OrderProductRow
            Dim row As OrderProductRow

            row = New OrderProductRow(DB, OrderProductID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal OrderProductID As Integer)
            Dim row As OrderProductRow

            row = New OrderProductRow(DB, OrderProductID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from OrderProduct"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        'Public Shared Function GetBuilderOrderProducts(ByVal DB As Database, ByVal BuilderID As Integer) As DataTable
        '    Dim sql As String = _
        '          " select op.*, sp.SupplyPhase,p.SKU,p.Product as ItemName, u.UnitOfMeasure as UnitOfMeasure, vp.VendorPrice, (vp.VendorPrice * op.Quantity) as ExtendedPrice " _
        '        & " from OrderProduct op left outer join SupplyPhase sp on op.SupplyPhaseId=sp.SupplyPhaseId " _
        '        & "     inner join Product p on op.ProductId=p.ProductId" _
        '        & "     left outer join UnitOfMeasure u on p.SizeUnitOfMeasureID=u.UnitOfMeasureID" _
        '        & "     inner join VendorProductPrice vp on p.ProductID=vp.ProductId" _
        '        & "     inner join [Order] o on op.OrderId=o.OrderId" _
        '        & " where o.BuilderId=" & DB.Number(BuilderID) _
        '        & "     and vp.VendorId=o.VendorId" _
        '        & " union all " _
        '        & " select ops.*, sps.SupplyPhase, null as SKU, sop.SpecialOrderProduct as ItemName, us.UnitOfMeasure as UnitOfMeasure, vsop.VendorPrice, (vsop.VendorPrice * ops.Quantity) as ExtendedPrice " _
        '        & " from OrderProduct ops left outer join SupplyPhase sps on ops.SupplyPhaseId=sps.SupplyPhaseId" _
        '        & "     inner join SpecialOrderProduct sop on ops.SpecialOrderProductId=sop.SpecialOrderProductId" _
        '        & "     left outer join UnitOfMeasure us on sop.UnitOfMeasureId=us.UnitOfMeasureID" _
        '        & "     inner join VendorSpecialOrderProductPrice vsop on sop.SpecialOrderProductId=vsop.SpecialOrderProductId" _
        '        & "     inner join [Order] os on ops.OrderId=ops.OrderId" _
        '        & " where os.BuilderId=" & DB.Number(BuilderID) _
        '        & "     and vsop.VendorId=os.VendorId" _
        '        & " order by OrderID"
        '    Return DB.GetDataTable(sql)
        'End Function

        Public Shared Function GetBuilderOrderProducts(ByVal DB As Database, ByVal BuilderID As Integer) As DataTable
            Dim sql As String = _
                  " select op.*, sp.SupplyPhase,op.VendorSKU as SKU,p.Product as ItemName, u.UnitOfMeasure as UnitOfMeasure, (op.VendorPrice * op.Quantity) as ExtendedPrice " _
                & " from OrderProduct op left outer join SupplyPhase sp on op.SupplyPhaseId=sp.SupplyPhaseId " _
                & "     inner join Product p on op.ProductId=p.ProductId" _
                & "     left outer join UnitOfMeasure u on p.SizeUnitOfMeasureID=u.UnitOfMeasureID" _
                & "     inner join [Order] o on op.OrderId=o.OrderId" _
                & " where o.BuilderId=" & DB.Number(BuilderID) _
                & " union all " _
                & " select ops.*, sps.SupplyPhase, ops.VendorSku as SKU, sop.SpecialOrderProduct as ItemName, us.UnitOfMeasure as UnitOfMeasure, (ops.VendorPrice * ops.Quantity) as ExtendedPrice " _
                & " from OrderProduct ops left outer join SupplyPhase sps on ops.SupplyPhaseId=sps.SupplyPhaseId" _
                & "     inner join SpecialOrderProduct sop on ops.SpecialOrderProductId=sop.SpecialOrderProductId" _
                & "     left outer join UnitOfMeasure us on sop.UnitOfMeasureId=us.UnitOfMeasureID" _
                & "     inner join [Order] os on ops.OrderId=os.OrderId" _
                & " where os.BuilderId=" & DB.Number(BuilderID) _
                & " order by OrderID"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetOrderProducts(ByVal DB As Database, ByVal OrderID As Integer) As DataTable
            Dim sql As String = _
                  " select op.*, p.Product as Product, vpp.*, 'Product' as PriceType " _
                  & " from OrderProduct op left outer join Product p on op.ProductID=p.ProductID" _
                  & "   inner join VendorProductPrice vpp on op.ProductID=vpp.ProductID" _
                  & " union " _
                  & " select op.*, p.SpecialOrderProduct as Product, vsop.*, 'Special' as PriceType " _
                  & " from OrderProduct op inner join SpecialOrderProduct p on op.SpecialOrderProductID=p.SpecialOrderProductID" _
                  & "   inner join VendorSpecialOrderProductPrice vsop on op.SpecialOrderProductID=vsop.SpecialOrderProductID"

            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function UpdateProductDrop(ByVal DB As Database, ByVal OrderProductID As Integer, ByVal OrderDropID As Integer, ByVal SortOrder As Integer) As Boolean
            Dim sql As String = "update OrderProduct set DropId=" & DB.Number(OrderDropID) & ", SortOrder=" & DB.Number(SortOrder) & " where OrderProductID=" & DB.Number(OrderProductID)
            Return DB.ExecuteSQL(sql)
        End Function
    End Class

    Public MustInherit Class OrderProductRowBase
        Private m_DB As Database
        Private m_OrderProductID As Integer = Nothing
        Private m_OrderID As Integer = Nothing
        Private m_ProductID As Integer = Nothing
        Private m_SpecialOrderProductID As Integer = Nothing
        Private m_SupplyPhaseID As Integer = Nothing
        Private m_DropID As Integer = Nothing
        Private m_Quantity As Integer = Nothing
        Private m_SortOrder As Integer = Nothing
        Private m_VendorSku As String = Nothing
        Private m_VendorPrice As Double = Nothing

        Public Property OrderProductID() As Integer
            Get
                Return m_OrderProductID
            End Get
            Set(ByVal Value As Integer)
                m_OrderProductID = value
            End Set
        End Property

        Public Property OrderID() As Integer
            Get
                Return m_OrderID
            End Get
            Set(ByVal Value As Integer)
                m_OrderID = value
            End Set
        End Property

        Public Property ProductID() As Integer
            Get
                Return m_ProductID
            End Get
            Set(ByVal Value As Integer)
                m_ProductID = value
            End Set
        End Property

        Public Property SpecialOrderProductID() As Integer
            Get
                Return m_SpecialOrderProductID
            End Get
            Set(ByVal Value As Integer)
                m_SpecialOrderProductID = value
            End Set
        End Property

        Public Property SupplyPhaseID() As Integer
            Get
                Return m_SupplyPhaseID
            End Get
            Set(ByVal Value As Integer)
                m_SupplyPhaseID = value
            End Set
        End Property

        Public Property DropID() As Integer
            Get
                Return m_DropID
            End Get
            Set(ByVal Value As Integer)
                m_DropID = value
            End Set
        End Property

        Public Property Quantity() As Integer
            Get
                Return m_Quantity
            End Get
            Set(ByVal Value As Integer)
                m_Quantity = value
            End Set
        End Property

        Public Property SortOrder() As Integer
            Get
                Return m_SortOrder
            End Get
            Set(ByVal Value As Integer)
                m_SortOrder = value
            End Set
        End Property

        Public Property VendorSku() As String
            Get
                Return m_VendorSku
            End Get
            Set(ByVal value As String)
                m_VendorSku = value
            End Set
        End Property

        Public Property VendorPrice() As Double
            Get
                Return m_VendorPrice
            End Get
            Set(ByVal value As Double)
                m_VendorPrice = value
            End Set
        End Property


        Public Property DB() As Database
            Get
                DB = m_DB
            End Get
            Set(ByVal Value As DataBase)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal OrderProductID As Integer)
            m_DB = DB
            m_OrderProductID = OrderProductID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM OrderProduct WHERE OrderProductID = " & DB.Number(OrderProductID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_OrderProductID = Convert.ToInt32(r.Item("OrderProductID"))
            m_OrderID = Convert.ToInt32(r.Item("OrderID"))
            If IsDBNull(r.Item("ProductID")) Then
                m_ProductID = Nothing
            Else
                m_ProductID = Convert.ToInt32(r.Item("ProductID"))
            End If
            If IsDBNull(r.Item("SpecialOrderProductID")) Then
                m_SpecialOrderProductID = Nothing
            Else
                m_SpecialOrderProductID = Convert.ToInt32(r.Item("SpecialOrderProductID"))
            End If
            If IsDBNull(r.Item("SupplyPhaseID")) Then
                m_SupplyPhaseID = Nothing
            Else
                m_SupplyPhaseID = Convert.ToInt32(r.Item("SupplyPhaseID"))
            End If
            If IsDBNull(r.Item("DropID")) Then
                m_DropID = Nothing
            Else
                m_DropID = Convert.ToInt32(r.Item("DropID"))
            End If
            m_Quantity = Convert.ToInt32(r.Item("Quantity"))
            m_VendorSku = Core.GetString(r.Item("VendorSku"))
            m_VendorPrice = Core.GetDouble(r.Item("VendorPrice"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from OrderProduct order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO OrderProduct (" _
             & " OrderID" _
             & ",ProductID" _
             & ",SpecialOrderProductID" _
             & ",SupplyPhaseID" _
             & ",DropID" _
             & ",Quantity" _
             & ",SortOrder" _
             & ",VendorSku" _
             & ",VendorPrice" _
             & ") VALUES (" _
             & m_DB.NullNumber(OrderID) _
             & "," & m_DB.NullNumber(ProductID) _
             & "," & m_DB.NullNumber(SpecialOrderProductID) _
             & "," & m_DB.NullNumber(SupplyPhaseID) _
             & "," & m_DB.NullNumber(DropID) _
             & "," & m_DB.Number(Quantity) _
             & "," & MaxSortOrder _
             & "," & m_DB.Quote(VendorSku) _
             & "," & m_DB.NullNumber(VendorPrice) _
             & ")"

            OrderProductID = m_DB.InsertSQL(SQL)

            Return OrderProductID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE OrderProduct SET " _
             & " OrderID = " & m_DB.NullNumber(OrderID) _
             & ",ProductID = " & m_DB.NullNumber(ProductID) _
             & ",SpecialOrderProductID = " & m_DB.NullNumber(SpecialOrderProductID) _
             & ",SupplyPhaseID = " & m_DB.NullNumber(SupplyPhaseID) _
             & ",DropID = " & m_DB.NullNumber(DropID) _
             & ",Quantity = " & m_DB.Number(Quantity) _
             & ",VendorSku = " & m_DB.Quote(VendorSku) _
             & ",VendorPrice = " & m_DB.NullNumber(VendorPrice) _
             & " WHERE OrderProductID = " & m_DB.Quote(OrderProductID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM OrderProduct WHERE OrderProductID = " & m_DB.Number(OrderProductID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class OrderProductCollection
        Inherits GenericCollection(Of OrderProductRow)
    End Class

End Namespace


