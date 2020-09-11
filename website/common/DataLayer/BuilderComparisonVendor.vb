Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class BuilderComparisonVendorRow
        Inherits BuilderComparisonVendorRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BuilderID As Integer, ByVal SortIndex As Integer)
            MyBase.New(DB, BuilderID, SortIndex)
        End Sub 'New

        ''Shared function to get one row
        'Public Shared Function GetRow(ByVal DB As Database, ByVal BuilderID As Integer) As BuilderComparisonVendorRow
        '    Dim row As BuilderComparisonVendorRow

        '    row = New BuilderComparisonVendorRow(DB, BuilderID)
        '    row.Load()

        '    Return row
        'End Function

        'Shared function to get one row by builder and sort order
        Public Shared Function GetRowByBuilder(ByVal DB As Database, ByVal BuilderID As Integer, ByVal SortIndex As Integer) As BuilderComparisonVendorRow
            Dim row As BuilderComparisonVendorRow

            row = New BuilderComparisonVendorRow(DB, BuilderID, SortIndex)
            row.LoadByBuilderIndex()

            Return row
        End Function

        'Public Shared Sub RemoveRow(ByVal DB As Database, ByVal BuilderID As Integer)
        '    Dim row As BuilderComparisonVendorRow

        '    row = New BuilderComparisonVendorRow(DB, BuilderID)
        '    row.Remove()
        'End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from BuilderComparisonVendor"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Sub DeleteByBuilder(ByVal DB As Database, ByVal BuilderID As Integer)
            Dim SQL As String = "DELETE FROM BuilderComparisonVendor WHERE BuilderID = " & BuilderID
            DB.ExecuteSQL(SQL)
        End Sub

    End Class

    Public MustInherit Class BuilderComparisonVendorRowBase
        Private m_DB As Database
        Private m_BuilderID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_SortOrder As Integer = Nothing


        Public Property BuilderID() As Integer
            Get
                Return m_BuilderID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderID = value
            End Set
        End Property

        Public Property VendorID() As Integer
            Get
                Return m_VendorID
            End Get
            Set(ByVal Value As Integer)
                m_VendorID = value
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

        Public Sub New(ByVal DB As Database, ByVal BuilderID As Integer, ByVal SortIndex As Integer)
            m_DB = DB
            m_BuilderID = BuilderID
            m_SortOrder = SortIndex
        End Sub 'New

        'Protected Overridable Sub Load()
        '    Dim r As SqlDataReader
        '    Dim SQL As String

        '    SQL = "SELECT * FROM BuilderComparisonVendor WHERE BuilderID = " & DB.Number(BuilderID)
        '    r = m_DB.GetReader(SQL)
        '    If r.Read Then
        '        Me.Load(r)
        '    End If
        '    r.Close()
        'End Sub

        Protected Sub LoadByBuilderIndex()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT TOP 1 * FROM BuilderComparisonVendor WHERE BuilderID = " & DB.Number(BuilderID) & " AND SortOrder = " & DB.Number(SortOrder)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_BuilderID = Convert.ToInt32(r.Item("BuilderID"))
            m_VendorID = Convert.ToInt32(r.Item("VendorID"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from BuilderComparisonVendor WHERE BuilderId = " & BuilderID & " order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO BuilderComparisonVendor (" _
             & " BuilderID" _
             & ",VendorID" _
             & ",SortOrder" _
             & ") VALUES (" _
             & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.NullNumber(VendorID) _
             & "," & MaxSortOrder _
             & ")"

            BuilderID = m_DB.ExecuteSQL(SQL)

            Return BuilderID
        End Function

        'Public Sub Remove()
        '    Dim SQL As String

        '    SQL = "DELETE FROM BuilderComparisonVendor WHERE BuilderID = " & m_DB.Number(BuilderID)
        '    m_DB.ExecuteSQL(SQL)
        'End Sub 'Remove
    End Class

    Public Class BuilderComparisonVendorCollection
        Inherits GenericCollection(Of BuilderComparisonVendorRow)
    End Class

End Namespace

