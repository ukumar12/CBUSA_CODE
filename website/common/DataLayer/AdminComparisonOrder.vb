Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class AdminComparisonOrderRow
        Inherits AdminComparisonOrderRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal SortIndex As Integer)
            MyBase.New(DB, SortIndex)
        End Sub 'New

        ''Shared function to get one row
        'Public Shared Function GetRow(ByVal DB As Database, ByVal AdminID As Integer) As AdminComparisonOrderRow
        '    Dim row As AdminComparisonOrderRow

        '    row = New AdminComparisonOrderRow(DB, AdminID)
        '    row.Load()

        '    Return row
        'End Function

        'Shared function to get one row by Admin and sort order
        Public Shared Function GetRowByIndex(ByVal DB As Database, ByVal SortIndex As Integer) As AdminComparisonOrderRow
            Dim row As AdminComparisonOrderRow

            row = New AdminComparisonOrderRow(DB, SortIndex)
            row.LoadByAdminIndex()

            Return row
        End Function

        'Public Shared Sub RemoveRow(ByVal DB As Database, ByVal AdminID As Integer)
        '    Dim row As AdminComparisonOrderRow

        '    row = New AdminComparisonOrderRow(DB, AdminID)
        '    row.Remove()
        'End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from AdminComparisonOrder"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Sub DeleteAll(ByVal DB As Database)
            Dim SQL As String = "DELETE FROM AdminComparisonOrder"
            DB.ExecuteSQL(SQL)
        End Sub

    End Class

    Public MustInherit Class AdminComparisonOrderRowBase
        Private m_DB As Database
        Private m_AdminID As Integer = Nothing
        Private m_OrderID As Integer = Nothing
        Private m_SortOrder As Integer = Nothing


        Public Property AdminID() As Integer
            Get
                Return m_AdminID
            End Get
            Set(ByVal Value As Integer)
                m_AdminID = Value
            End Set
        End Property

        Public Property OrderID() As Integer
            Get
                Return m_OrderID
            End Get
            Set(ByVal Value As Integer)
                m_OrderID = Value
            End Set
        End Property

        Public Property SortOrder() As Integer
            Get
                Return m_SortOrder
            End Get
            Set(ByVal Value As Integer)
                m_SortOrder = Value
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

        Public Sub New(ByVal DB As Database, ByVal SortIndex As Integer)
            m_DB = DB
            m_SortOrder = SortIndex
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM AdminComparisonOrder WHERE AdminID = " & DB.Number(AdminID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Sub LoadByAdminIndex()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT TOP 1 * FROM AdminComparisonOrder WHERE SortOrder = " & DB.Number(SortOrder)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_AdminID = Convert.ToInt32(r.Item("AdminID"))
            m_OrderID = Convert.ToInt32(r.Item("OrderID"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from AdminComparisonOrder order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO AdminComparisonOrder (" _
             & " AdminID" _
             & ",OrderID" _
             & ",SortOrder" _
             & ") VALUES (" _
             & m_DB.NullNumber(AdminID) _
             & "," & m_DB.NullNumber(OrderID) _
             & "," & MaxSortOrder _
             & ")"

            AdminID = m_DB.ExecuteSQL(SQL)

            Return AdminID
        End Function

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM AdminComparisonOrder WHERE AdminID = " & m_DB.Number(AdminID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class AdminComparisonOrderCollection
        Inherits GenericCollection(Of AdminComparisonOrderRow)
    End Class

End Namespace

