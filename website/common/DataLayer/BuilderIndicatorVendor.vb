Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class BuilderIndicatorVendorRow
        Inherits BuilderIndicatorVendorRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BuilderIndicatorVendorID As Integer)
            MyBase.New(DB, BuilderIndicatorVendorID)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BuilderID As Integer, ByVal SortIndex As Integer)
            MyBase.New(DB, BuilderID, SortIndex)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal BuilderIndicatorVendorID As Integer) As BuilderIndicatorVendorRow
            Dim row As BuilderIndicatorVendorRow

            row = New BuilderIndicatorVendorRow(DB, BuilderIndicatorVendorID)
            row.Load()

            Return row
        End Function

        'Shared function to get one row by builder and sort order
        Public Shared Function GetRowByBuilder(ByVal DB As Database, ByVal BuilderID As Integer, ByVal SortIndex As Integer) As BuilderIndicatorVendorRow
            Dim row As BuilderIndicatorVendorRow

            row = New BuilderIndicatorVendorRow(DB, BuilderID, SortIndex)
            row.LoadByBuilderIndex()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal BuilderIndicatorVendorID As Integer)
            Dim row As BuilderIndicatorVendorRow

            row = New BuilderIndicatorVendorRow(DB, BuilderIndicatorVendorID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from BuilderIndicatorVendor"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Sub DeleteByBuilder(ByVal DB As Database, ByVal BuilderID As Integer)
            Dim SQL As String = "DELETE FROM BuilderIndicatorVendor WHERE BuilderID = " & BuilderID
            DB.ExecuteSQL(SQL)
        End Sub

    End Class

    Public MustInherit Class BuilderIndicatorVendorRowBase
        Private m_DB As Database
        Private m_BuilderIndicatorVendorID As Integer = Nothing
        Private m_BuilderID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_SortOrder As Integer = Nothing
        Private m_BuilderIndicatorProductID As Integer = Nothing

        Public Property BuilderIndicatorVendorID() As Integer
            Get
                Return m_BuilderIndicatorVendorID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderIndicatorVendorID = Value
            End Set
        End Property

        Public Property BuilderID() As Integer
            Get
                Return m_BuilderID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderID = Value
            End Set
        End Property

        Public Property VendorID() As Integer
            Get
                Return m_VendorID
            End Get
            Set(ByVal Value As Integer)
                m_VendorID = Value
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

        Public Property BuilderIndicatorProductID() As Integer
            Get
                Return m_BuilderIndicatorProductID
            End Get
            Set(ByVal value As Integer)
                m_BuilderIndicatorProductID = value
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

        Public Sub New(ByVal DB As Database, ByVal BuilderIndicatorVendorID As Integer)
            m_DB = DB
            m_BuilderIndicatorVendorID = BuilderIndicatorVendorID
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BuilderID As Integer, ByVal SortIndex As Integer)
            m_DB = DB
            m_BuilderID = BuilderID
            m_SortOrder = SortIndex
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM BuilderIndicatorVendor WHERE BuilderIndicatorVendorID = " & DB.Number(BuilderIndicatorVendorID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Sub LoadByBuilderIndex()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT TOP 1 * FROM BuilderIndicatorVendor WHERE BuilderID = " & DB.Number(BuilderID) & " AND SortOrder = " & DB.Number(SortOrder)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_BuilderIndicatorVendorID = Convert.ToInt32(r.Item("BuilderIndicatorVendorID"))
            m_BuilderID = Convert.ToInt32(r.Item("BuilderID"))
            m_VendorID = Convert.ToInt32(r.Item("VendorID"))
            m_BuilderIndicatorProductID = Core.GetInt(r.Item("BuilderIndicatorProductID"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from BuilderIndicatorVendor WHERE BuilderID = " & BuilderID & " order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO BuilderIndicatorVendor (" _
             & " BuilderID" _
             & ",VendorID" _
             & ",SortOrder" _
             & ",BuilderIndicatorProductID" _
             & ") VALUES (" _
             & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.NullNumber(VendorID) _
             & "," & MaxSortOrder _
             & "," & m_DB.NullNumber(BuilderIndicatorProductID) _
             & ")"

            BuilderIndicatorVendorID = m_DB.InsertSQL(SQL)

            Return BuilderIndicatorVendorID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE BuilderIndicatorVendor SET " _
             & " BuilderID = " & m_DB.NullNumber(BuilderID) _
             & ",VendorID = " & m_DB.NullNumber(VendorID) _
             & ",BuilderIndicatorProductID = " & m_DB.NullNumber(BuilderIndicatorProductID) _
             & " WHERE BuilderIndicatorVendorID = " & m_DB.Quote(BuilderIndicatorVendorID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM BuilderIndicatorVendor WHERE BuilderIndicatorVendorID = " & m_DB.Number(BuilderIndicatorVendorID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class BuilderIndicatorVendorCollection
        Inherits GenericCollection(Of BuilderIndicatorVendorRow)
    End Class

End Namespace