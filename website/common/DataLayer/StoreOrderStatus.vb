Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class StoreOrderStatusRow
        Inherits StoreOrderStatusRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal StatusId As Integer)
            MyBase.New(DB, StatusId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal StatusId As Integer) As StoreOrderStatusRow
            Dim row As StoreOrderStatusRow

            row = New StoreOrderStatusRow(DB, StatusId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal StatusId As Integer)
            Dim row As StoreOrderStatusRow

            row = New StoreOrderStatusRow(DB, StatusId)
            row.Remove()
        End Sub

        'Custom Methods

        Public Shared Function GetAllStoreOrderStatuss(ByVal DB As Database) As DataTable
            Dim dt As DataTable = DB.GetDataTable("select * from StoreOrderStatus order by ProcessSortOrder")
            Return dt
        End Function

        Public Shared Function GetOrderStatusesWithSummary(ByVal DB As Database) As DataTable
            Dim SQL As String = String.Empty

            SQL &= " SELECT ss.Code, RTRIM(ss.Name) + ' (' + RTRIM(STR(COUNT(sor.RecipientId))) + ')' AS name" & _
                   " FROM StoreOrderStatus AS ss LEFT OUTER JOIN StoreOrderRecipient AS sor ON ss.Code = sor.Status " & _
                   " WHERE (sor.OrderId IN (SELECT OrderId FROM StoreOrder AS so WHERE (OrderId = sor.OrderId) AND (ProcessDate IS NOT NULL))) " & _
                   " GROUP BY ss.Code, ss.Name ORDER BY ss.Code, ss.Name"
            Dim dt As DataTable = DB.GetDataTable(SQL)
            Return dt
        End Function

        Public Shared Function GetRowByCode(ByVal DB As Database, ByVal Code As String) As StoreOrderStatusRow
            Dim SQL As String = "SELECT * FROM StoreOrderStatus WHERE Code = " & DB.Quote(Code)
            Dim r As SqlDataReader
            Dim row As StoreOrderStatusRow = New StoreOrderStatusRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

    End Class

    Public MustInherit Class StoreOrderStatusRowBase
        Private m_DB As Database
        Private m_StatusId As Integer = Nothing
        Private m_Code As String = Nothing
        Private m_Name As String = Nothing
        Private m_ProcessSortOrder As Integer = Nothing
        Private m_IsFinalAction As Boolean = Nothing

        Public Property IsFinalAction() As Boolean
            Get
                Return m_IsFinalAction
            End Get
            Set(ByVal value As Boolean)
                m_IsFinalAction = value
            End Set
        End Property
        Public Property ProcessSortOrder() As Integer
            Get
                Return m_ProcessSortOrder
            End Get
            Set(ByVal value As Integer)
                m_ProcessSortOrder = value
            End Set
        End Property
        Public Property StatusId() As Integer
            Get
                Return m_StatusId
            End Get
            Set(ByVal Value As Integer)
                m_StatusId = value
            End Set
        End Property

        Public Property Code() As String
            Get
                Return m_Code
            End Get
            Set(ByVal Value As String)
                m_Code = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal Value As String)
                m_Name = value
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

        Public Sub New(ByVal DB As Database, ByVal StatusId As Integer)
            m_DB = DB
            m_StatusId = StatusId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM StoreOrderStatus WHERE StatusId = " & DB.Number(StatusId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_StatusId = Convert.ToInt32(r.Item("StatusId"))
            If IsDBNull(r.Item("Code")) Then
                m_Code = Nothing
            Else
                m_Code = Convert.ToString(r.Item("Code"))
            End If
            If IsDBNull(r.Item("Name")) Then
                m_Name = Nothing
            Else
                m_Name = Convert.ToString(r.Item("Name"))
            End If

            m_ProcessSortOrder = Convert.ToInt32(r.Item("ProcessSortOrder"))
            m_IsFinalAction = Convert.ToBoolean(r.Item("IsFinalAction"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO StoreOrderStatus (" _
             & " Code" _
             & ",Name" _
             & ",ProcessSortOrder" _
             & ",IsFinalAction" _
             & ") VALUES (" _
             & m_DB.Quote(Code) _
             & "," & m_DB.Quote(Name) _
             & "," & m_DB.Number(ProcessSortOrder) _
             & "," & CInt(IsFinalAction) _
             & ")"

            StatusId = m_DB.InsertSQL(SQL)

            Return StatusId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE StoreOrderStatus SET " _
             & " Code = " & m_DB.Quote(Code) _
             & ",Name = " & m_DB.Quote(Name) _
             & ",ProcessSortOrder = " & m_DB.Number(ProcessSortOrder) _
             & ",IsFinalAction = " & CInt(IsFinalAction) _
             & " WHERE StatusId = " & m_DB.Quote(StatusId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM StoreOrderStatus WHERE StatusId = " & m_DB.Quote(StatusId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class StoreOrderStatusCollection
        Inherits GenericCollection(Of StoreOrderStatusRow)
    End Class

End Namespace


