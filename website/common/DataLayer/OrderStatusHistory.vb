Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class OrderStatusHistoryRow
        Inherits OrderStatusHistoryRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal OrderStatusHistoryID As Integer)
            MyBase.New(DB, OrderStatusHistoryID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal OrderStatusHistoryID As Integer) As OrderStatusHistoryRow
            Dim row As OrderStatusHistoryRow

            row = New OrderStatusHistoryRow(DB, OrderStatusHistoryID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal OrderStatusHistoryID As Integer)
            Dim row As OrderStatusHistoryRow

            row = New OrderStatusHistoryRow(DB, OrderStatusHistoryID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from OrderStatusHistory"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class OrderStatusHistoryRowBase
        Private m_DB As Database
        Private m_OrderStatusHistoryID As Integer = Nothing
        Private m_OrderID As Integer = Nothing
        Private m_OrderStatusID As Integer = Nothing
        Private m_Created As DateTime = Nothing
        Private m_CreatorVendorAccountID As Integer = Nothing


        Public Property OrderStatusHistoryID() As Integer
            Get
                Return m_OrderStatusHistoryID
            End Get
            Set(ByVal Value As Integer)
                m_OrderStatusHistoryID = value
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

        Public Property OrderStatusID() As Integer
            Get
                Return m_OrderStatusID
            End Get
            Set(ByVal Value As Integer)
                m_OrderStatusID = value
            End Set
        End Property

        Public ReadOnly Property Created() As DateTime
            Get
                Return m_Created
            End Get
        End Property

        Public Property CreatorVendorAccountID() As Integer
            Get
                Return m_CreatorVendorAccountID
            End Get
            Set(ByVal Value As Integer)
                m_CreatorVendorAccountID = Value
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

        Public Sub New(ByVal DB As Database, ByVal OrderStatusHistoryID As Integer)
            m_DB = DB
            m_OrderStatusHistoryID = OrderStatusHistoryID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM OrderStatusHistory WHERE OrderStatusHistoryID = " & DB.Number(OrderStatusHistoryID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_OrderStatusHistoryID = Core.GetInt(r.Item("OrderStatusHistoryID"))
            m_OrderID = Core.GetInt(r.Item("OrderID"))
            m_OrderStatusID = Core.GetInt(r.Item("OrderStatusID"))
            m_Created = Core.GetDate(r.Item("Created"))
            m_CreatorVendorAccountID = Core.GetInt(r.Item("CreatorVendorAccountID"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO OrderStatusHistory (" _
             & " OrderID" _
             & ",OrderStatusID" _
             & ",Created" _
             & ",CreatorVendorAccountID" _
             & ") VALUES (" _
             & m_DB.NullNumber(OrderID) _
             & "," & m_DB.NullNumber(OrderStatusID) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(CreatorVendorAccountID) _
             & ")"

            OrderStatusHistoryID = m_DB.InsertSQL(SQL)

            Return OrderStatusHistoryID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE OrderStatusHistory SET " _
             & " OrderID = " & m_DB.NullNumber(OrderID) _
             & ",OrderStatusID = " & m_DB.NullNumber(OrderStatusID) _
             & " WHERE OrderStatusHistoryID = " & m_DB.Quote(OrderStatusHistoryID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM OrderStatusHistory WHERE OrderStatusHistoryID = " & m_DB.Number(OrderStatusHistoryID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class OrderStatusHistoryCollection
        Inherits GenericCollection(Of OrderStatusHistoryRow)
    End Class

End Namespace


