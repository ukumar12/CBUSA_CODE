Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class TwoPriceOrderProductRow
        Inherits TwoPriceOrderProductRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, TwoPriceOrderProductID As Integer)
            MyBase.New(DB, TwoPriceOrderProductID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal TwoPriceOrderProductID As Integer) As TwoPriceOrderProductRow
            Dim row As TwoPriceOrderProductRow

            row = New TwoPriceOrderProductRow(DB, TwoPriceOrderProductID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal TwoPriceOrderProductID As Integer)
            Dim SQL As String

            SQL = "DELETE FROM TwoPriceOrderProduct WHERE TwoPriceOrderProductID = " & DB.Number(TwoPriceOrderProductID)
            DB.ExecuteSQL(SQL)
        End Sub

        Public Sub Remove()
            RemoveRow(DB, TwoPriceOrderProductID)
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from TwoPriceOrderProduct"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function


        Public Shared Sub AddProduct(ByVal DB As Database, TwoPriceOrderID As Integer, ProductId As Integer, Price As Double, Quantity As Integer, Optional ByVal AddToQuantity As Boolean = True, Optional VendorSku As String = Nothing, Optional PriceState As Integer = 0, Optional TakeOffID As Integer = 0)
            Dim TwoPriceOrderProduct As TwoPriceOrderProductRow
            'Per ticket 205529 - customer doesnot want merge products - insert the same way as in takeoff

            'Product is not part of this order, add it.
            TwoPriceOrderProduct = New TwoPriceOrderProductRow(DB)
            TwoPriceOrderProduct.VendorPrice = Price
            TwoPriceOrderProduct.Quantity = Quantity
            TwoPriceOrderProduct.ProductID = ProductId
            TwoPriceOrderProduct.TwoPriceOrderID = TwoPriceOrderID
            TwoPriceOrderProduct.PriceRequestState = PriceState
            TwoPriceOrderProduct.VendorSku = VendorSku
            TwoPriceOrderProduct.TakeOffID = TakeOffID
            TwoPriceOrderProduct.TwoPriceOrderProductID = TwoPriceOrderProduct.Insert()

        End Sub
        'Custom Methods

        '*************** New sub added by Apala (Medullus) for mGuard#T-1086
        Public Shared Sub AddSpecialOrderProduct(ByVal DB As Database, TwoPriceOrderID As Integer, SpecialOrderProductId As Integer, Price As Double, Quantity As Integer, Optional ByVal AddToQuantity As Boolean = True, Optional VendorSku As String = Nothing, Optional PriceState As Integer = 0, Optional TakeOffID As Integer = 0)
            Dim TwoPriceOrderProduct As TwoPriceOrderProductRow

            'Product is not part of this order, add it.
            TwoPriceOrderProduct = New TwoPriceOrderProductRow(DB)
            TwoPriceOrderProduct.VendorPrice = Price
            TwoPriceOrderProduct.Quantity = Quantity
            TwoPriceOrderProduct.SpecialOrderProductID = SpecialOrderProductId
            TwoPriceOrderProduct.TwoPriceOrderID = TwoPriceOrderID
            TwoPriceOrderProduct.PriceRequestState = PriceState
            TwoPriceOrderProduct.VendorSku = VendorSku
            TwoPriceOrderProduct.TakeOffID = TakeOffID
            TwoPriceOrderProduct.TwoPriceOrderProductID = TwoPriceOrderProduct.Insert()

        End Sub

    End Class

    Public MustInherit Class TwoPriceOrderProductRowBase
        Private m_DB As Database
        Private m_TwoPriceOrderProductID As Integer = Nothing
        Private m_TwoPriceOrderID As Integer = Nothing
        Private m_ProductID As Integer = Nothing
        Private m_SpecialOrderProductID As Integer = Nothing
        Private m_SupplyPhaseID As Integer = Nothing
        Private m_DropID As Integer = Nothing
        Private m_Quantity As Integer = Nothing
        Private m_SortOrder As Integer = Nothing
        Private m_VendorSku As String = Nothing
        Private m_VendorPrice As Double = Nothing
        Private m_PriceRequestState As Integer = Nothing
        Private m_TakeOffID As Integer = Nothing
        Public Property TwoPriceOrderProductID As Integer
            Get
                Return m_TwoPriceOrderProductID
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceOrderProductID = value
            End Set
        End Property

        Public Property TwoPriceOrderID As Integer
            Get
                Return m_TwoPriceOrderID
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceOrderID = value
            End Set
        End Property

        Public Property ProductID As Integer
            Get
                Return m_ProductID
            End Get
            Set(ByVal Value As Integer)
                m_ProductID = value
            End Set
        End Property

        Public Property SpecialOrderProductID As Integer
            Get
                Return m_SpecialOrderProductID
            End Get
            Set(ByVal Value As Integer)
                m_SpecialOrderProductID = value
            End Set
        End Property

        Public Property SupplyPhaseID As Integer
            Get
                Return m_SupplyPhaseID
            End Get
            Set(ByVal Value As Integer)
                m_SupplyPhaseID = value
            End Set
        End Property
        Public Property PriceRequestState As Integer
            Get
                Return m_PriceRequestState
            End Get
            Set(ByVal Value As Integer)
                m_PriceRequestState = Value
            End Set
        End Property
        Public Property DropID As Integer
            Get
                Return m_DropID
            End Get
            Set(ByVal Value As Integer)
                m_DropID = value
            End Set
        End Property

        Public Property Quantity As Integer
            Get
                Return m_Quantity
            End Get
            Set(ByVal Value As Integer)
                m_Quantity = value
            End Set
        End Property

        Public Property SortOrder As Integer
            Get
                Return m_SortOrder
            End Get
            Set(ByVal Value As Integer)
                m_SortOrder = value
            End Set
        End Property
        Public Property TakeOffID As Integer
            Get
                Return m_TakeOffID
            End Get
            Set(ByVal Value As Integer)
                m_TakeOffID = Value
            End Set
        End Property

        Public Property VendorSku As String
            Get
                Return m_VendorSku
            End Get
            Set(ByVal Value As String)
                m_VendorSku = value
            End Set
        End Property

        Public Property VendorPrice As Double
            Get
                Return m_VendorPrice
            End Get
            Set(ByVal Value As Double)
                m_VendorPrice = value
            End Set
        End Property


        Public Property DB() As Database
            Get
                DB = m_DB
            End Get
            Set(ByVal Value As Database)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, TwoPriceOrderProductID As Integer)
            m_DB = DB
            m_TwoPriceOrderProductID = TwoPriceOrderProductID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM TwoPriceOrderProduct WHERE TwoPriceOrderProductID = " & DB.Number(TwoPriceOrderProductID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_TwoPriceOrderProductID = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_TwoPriceOrderProductID = Core.GetInt(r.Item("TwoPriceOrderProductID"))
            m_TwoPriceOrderID = Core.GetInt(r.Item("TwoPriceOrderID"))
            m_ProductID = Core.GetInt(r.Item("ProductID"))
            m_SpecialOrderProductID = Core.GetInt(r.Item("SpecialOrderProductID"))
            m_SupplyPhaseID = Core.GetInt(r.Item("SupplyPhaseID"))
            m_DropID = Core.GetInt(r.Item("DropID"))
            m_Quantity = Core.GetInt(r.Item("Quantity"))
            m_VendorSku = Core.GetString(r.Item("VendorSku"))
            m_VendorPrice = Core.GetDouble(r.Item("VendorPrice"))
            m_PriceRequestState = Core.GetInt(r.Item("PriceRequestState"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from TwoPriceOrderProduct order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO TwoPriceOrderProduct (" _
                & " TwoPriceOrderID" _
                & ",ProductID" _
                & ",SpecialOrderProductID" _
                & ",SupplyPhaseID" _
                & ",DropID" _
                & ",Quantity" _
                & ",SortOrder" _
                & ",VendorSku" _
                & ",VendorPrice" _
                & ",PriceRequestState" _
                & ",TakeOffID" _
                & ") VALUES (" _
                & m_DB.NullNumber(TwoPriceOrderID) _
                & "," & m_DB.NullNumber(ProductID) _
                & "," & m_DB.NullNumber(SpecialOrderProductID) _
                & "," & m_DB.NullNumber(SupplyPhaseID) _
                & "," & m_DB.NullNumber(DropID) _
                & "," & m_DB.Number(Quantity) _
                & "," & MaxSortOrder _
                & "," & m_DB.Quote(VendorSku) _
                & "," & m_DB.Number(VendorPrice) _
                & "," & m_DB.Number(PriceRequestState) _
                & "," & m_DB.Number(TakeOffID) _
                & ")"

            TwoPriceOrderProductID = m_DB.InsertSQL(SQL)

            Return TwoPriceOrderProductID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE TwoPriceOrderProduct WITH (ROWLOCK) SET " _
                & " TwoPriceOrderID = " & m_DB.NullNumber(TwoPriceOrderID) _
                & ",ProductID = " & m_DB.NullNumber(ProductID) _
                & ",SpecialOrderProductID = " & m_DB.NullNumber(SpecialOrderProductID) _
                & ",SupplyPhaseID = " & m_DB.NullNumber(SupplyPhaseID) _
                & ",DropID = " & m_DB.NullNumber(DropID) _
                & ",Quantity = " & m_DB.Number(Quantity) _
                & ",VendorSku = " & m_DB.Quote(VendorSku) _
                & ",VendorPrice = " & m_DB.Number(VendorPrice) _
                & ",PriceRequestState = " & m_DB.NullNumber(PriceRequestState) _
                & " WHERE TwoPriceOrderProductID = " & m_DB.Quote(TwoPriceOrderProductID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class TwoPriceOrderProductCollection
        Inherits GenericCollection(Of TwoPriceOrderProductRow)
    End Class

End Namespace
