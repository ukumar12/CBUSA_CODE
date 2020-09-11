Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class ContentToolPageRegionRow
        Inherits ContentToolPageRegionRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            MyBase.New(database)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal PageRegionId As Integer)
            MyBase.New(database, PageRegionId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal _Database As Database, ByVal PageRegionId As Integer) As ContentToolPageRegionRow
            Dim row As ContentToolPageRegionRow

            row = New ContentToolPageRegionRow(_Database, PageRegionId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal _Database As Database, ByVal PageRegionId As Integer)
            Dim row As ContentToolPageRegionRow

            row = New ContentToolPageRegionRow(_Database, PageRegionId)
            row.Remove()
        End Sub

        'Custom Methods

    End Class

    Public MustInherit Class ContentToolPageRegionRowBase
        Private m_DB As Database
        Private m_PageRegionId As Integer = Nothing
        Private m_PageId As Integer = Nothing
        Private m_RegionType As String = Nothing
        Private m_ContentRegion As String = Nothing

        Public Property PageRegionId() As Integer
            Get
                Return m_PageRegionId
            End Get
            Set(ByVal Value As Integer)
                m_PageRegionId = Value
            End Set
        End Property

        Public Property PageId() As Integer
            Get
                Return m_PageId
            End Get
            Set(ByVal Value As Integer)
                m_PageId = Value
            End Set
        End Property

        Public Property RegionType() As String
            Get
                Return m_RegionType
            End Get
            Set(ByVal Value As String)
                m_RegionType = Value
            End Set
        End Property

        Public Property ContentRegion() As String
            Get
                Return m_ContentRegion
            End Get
            Set(ByVal Value As String)
                m_ContentRegion = Value
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

        Public Sub New(ByVal database As Database)
            m_DB = database
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal PageRegionId As Integer)
            m_DB = database
            m_PageRegionId = PageRegionId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader = Nothing
            Dim SQL As String

            SQL = "SELECT * FROM ContentToolPageRegion WHERE PageRegionId = " & DB.Quote(PageRegionId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_PageRegionId = Convert.ToInt32(r.Item("PageRegionId"))
            If r.Item("PageId") Is Convert.DBNull Then
                m_PageId = Nothing
            Else
                m_PageId = Convert.ToInt32(r.Item("PageId"))
            End If
            If r.Item("RegionType") Is Convert.DBNull Then
                m_RegionType = Nothing
            Else
                m_RegionType = Convert.ToString(r.Item("RegionType"))
            End If
            If r.Item("ContentRegion") Is Convert.DBNull Then
                m_ContentRegion = Nothing
            Else
                m_ContentRegion = Convert.ToString(r.Item("ContentRegion"))
            End If
        End Sub 'Load

        Private ReadOnly Property InsertStatement() As String
            Get
                Dim SQL As String

                SQL = " INSERT INTO ContentToolPageRegion (" _
                 & " PageId" _
                 & ",RegionType" _
                 & ",ContentRegion" _
                 & ") VALUES (" _
                 & m_DB.Quote(PageId) _
                 & "," & m_DB.Quote(RegionType) _
                 & "," & m_DB.Quote(ContentRegion) _
                 & ")"

                Return SQL
            End Get
        End Property

        Public Overridable Sub Insert()
            m_DB.ExecuteSQL(InsertStatement)
        End Sub 'Insert

        Function AutoInsert() As Integer
            PageRegionId = m_DB.InsertSQL(InsertStatement)
            Return PageRegionId
        End Function


        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE ContentToolPageRegion SET " _
             & " PageId = " & m_DB.Quote(PageId) _
             & ",RegionType = " & m_DB.Quote(RegionType) _
             & ",ContentRegion= " & m_DB.Quote(ContentRegion) _
             & " WHERE PageRegionId = " & m_DB.Quote(PageRegionId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM ContentToolPageRegion WHERE PageRegionId = " & m_DB.Quote(PageRegionId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

End Namespace


