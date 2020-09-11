Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class TwoPriceOrderDropRow
        Inherits TwoPriceOrderDropRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, OrderDropID As Integer)
            MyBase.New(DB, OrderDropID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal OrderDropID As Integer) As TwoPriceOrderDropRow
            Dim row As TwoPriceOrderDropRow

            row = New TwoPriceOrderDropRow(DB, OrderDropID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal OrderDropID As Integer)
            Dim row As TwoPriceOrderDropRow

            row = New TwoPriceOrderDropRow(DB, OrderDropID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from TwoPriceOrderDrop"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Sub AddProductsToDrop(ByVal DB As Database, ByVal OrderDropID As Integer, ByVal sItems As String)
            Dim sql As String = "update OrderProduct set DropID=" & DB.Number(OrderDropID) & " where OrderProductID in " & DB.NumberMultiple(sItems)
            DB.ExecuteSQL(sql)
        End Sub
    End Class

    Public MustInherit Class TwoPriceOrderDropRowBase
        Private m_DB As Database
        Private m_OrderDropID As Integer = Nothing
        Private m_OrderID As Integer = Nothing
        Private m_DropName As String = Nothing
        Private m_RequestedDelivery As String = Nothing
        Private m_ShipToAddress As String = Nothing
        Private m_ShipToAddress2 As String = Nothing
        Private m_City As String = Nothing
        Private m_State As String = Nothing
        Private m_Zip As String = Nothing
        Private m_DeliveryInstructions As String = Nothing
        Private m_Notes As String = Nothing
        Private m_Created As DateTime = Nothing
        Private m_CreatorBuilderID As Integer = Nothing


        Public Property OrderDropID As Integer
            Get
                Return m_OrderDropID
            End Get
            Set(ByVal Value As Integer)
                m_OrderDropID = Value
            End Set
        End Property

        Public Property OrderID As Integer
            Get
                Return m_OrderID
            End Get
            Set(ByVal Value As Integer)
                m_OrderID = Value
            End Set
        End Property

        Public Property DropName As String
            Get
                Return m_DropName
            End Get
            Set(ByVal Value As String)
                m_DropName = Value
            End Set
        End Property

        Public Property RequestedDelivery As String
            Get
                Return m_RequestedDelivery
            End Get
            Set(ByVal Value As String)
                m_RequestedDelivery = Value
            End Set
        End Property

        Public Property ShipToAddress As String
            Get
                Return m_ShipToAddress
            End Get
            Set(ByVal Value As String)
                m_ShipToAddress = Value
            End Set
        End Property

        Public Property ShipToAddress2 As String
            Get
                Return m_ShipToAddress2
            End Get
            Set(ByVal Value As String)
                m_ShipToAddress2 = Value
            End Set
        End Property

        Public Property City As String
            Get
                Return m_City
            End Get
            Set(ByVal Value As String)
                m_City = Value
            End Set
        End Property

        Public Property State As String
            Get
                Return m_State
            End Get
            Set(ByVal Value As String)
                m_State = Value
            End Set
        End Property

        Public Property Zip As String
            Get
                Return m_Zip
            End Get
            Set(ByVal Value As String)
                m_Zip = Value
            End Set
        End Property

        Public Property DeliveryInstructions As String
            Get
                Return m_DeliveryInstructions
            End Get
            Set(ByVal Value As String)
                m_DeliveryInstructions = Value
            End Set
        End Property

        Public Property Notes As String
            Get
                Return m_Notes
            End Get
            Set(ByVal Value As String)
                m_Notes = Value
            End Set
        End Property

        Public ReadOnly Property Created() As DateTime
            Get
                Return m_Created
            End Get
        End Property

        Public Property CreatorBuilderID As Integer
            Get
                Return m_CreatorBuilderID
            End Get
            Set(ByVal Value As Integer)
                m_CreatorBuilderID = Value
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

        Public Sub New(ByVal DB As Database, OrderDropID As Integer)
            m_DB = DB
            m_OrderDropID = OrderDropID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM TwoPriceOrderDrop WHERE OrderDropID = " & DB.Number(OrderDropID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_OrderDropID = Convert.ToInt32(r.Item("OrderDropID"))
            m_OrderID = Convert.ToInt32(r.Item("OrderID"))
            m_DropName = Convert.ToString(r.Item("DropName"))
            If IsDBNull(r.Item("RequestedDelivery")) Then
                m_RequestedDelivery = Nothing
            Else
                m_RequestedDelivery = Convert.ToString(r.Item("RequestedDelivery"))
            End If
            m_ShipToAddress = Convert.ToString(r.Item("ShipToAddress"))
            If IsDBNull(r.Item("ShipToAddress2")) Then
                m_ShipToAddress2 = Nothing
            Else
                m_ShipToAddress2 = Convert.ToString(r.Item("ShipToAddress2"))
            End If
            m_City = Convert.ToString(r.Item("City"))
            m_State = Convert.ToString(r.Item("State"))
            m_Zip = Convert.ToString(r.Item("Zip"))
            If IsDBNull(r.Item("DeliveryInstructions")) Then
                m_DeliveryInstructions = Nothing
            Else
                m_DeliveryInstructions = Convert.ToString(r.Item("DeliveryInstructions"))
            End If
            If IsDBNull(r.Item("Notes")) Then
                m_Notes = Nothing
            Else
                m_Notes = Convert.ToString(r.Item("Notes"))
            End If
            m_Created = Convert.ToDateTime(r.Item("Created"))
            m_CreatorBuilderID = Convert.ToInt32(r.Item("CreatorBuilderID"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO TwoPriceOrderDrop (" _
             & " OrderID" _
             & ",DropName" _
             & ",RequestedDelivery" _
             & ",ShipToAddress" _
             & ",ShipToAddress2" _
             & ",City" _
             & ",State" _
             & ",Zip" _
             & ",DeliveryInstructions" _
             & ",Notes" _
             & ",Created" _
             & ",CreatorBuilderID" _
             & ") VALUES (" _
             & m_DB.NullNumber(OrderID) _
             & "," & m_DB.Quote(DropName) _
             & "," & m_DB.Quote(RequestedDelivery) _
             & "," & m_DB.Quote(ShipToAddress) _
             & "," & m_DB.Quote(ShipToAddress2) _
             & "," & m_DB.Quote(City) _
             & "," & m_DB.Quote(State) _
             & "," & m_DB.Quote(Zip) _
             & "," & m_DB.Quote(DeliveryInstructions) _
             & "," & m_DB.Quote(Notes) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(CreatorBuilderID) _
             & ")"

            OrderDropID = m_DB.InsertSQL(SQL)

            Return OrderDropID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE TwoPriceOrderDrop SET " _
             & " OrderID = " & m_DB.NullNumber(OrderID) _
             & ",DropName = " & m_DB.Quote(DropName) _
             & ",RequestedDelivery = " & m_DB.Quote(RequestedDelivery) _
             & ",ShipToAddress = " & m_DB.Quote(ShipToAddress) _
             & ",ShipToAddress2 = " & m_DB.Quote(ShipToAddress2) _
             & ",City = " & m_DB.Quote(City) _
             & ",State = " & m_DB.Quote(State) _
             & ",Zip = " & m_DB.Quote(Zip) _
             & ",DeliveryInstructions = " & m_DB.Quote(DeliveryInstructions) _
             & ",Notes = " & m_DB.Quote(Notes) _
             & " WHERE OrderDropID = " & m_DB.Quote(OrderDropID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM TwoPriceOrderDrop WHERE OrderDropID = " & m_DB.Number(OrderDropID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class TwoPriceOrderDropCollection
        Inherits GenericCollection(Of TwoPriceOrderDropRow)
    End Class

End Namespace

