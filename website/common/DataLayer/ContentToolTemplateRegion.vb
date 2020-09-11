Option Explicit On 

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class ContentToolTemplateRegionRow
        Inherits ContentToolTemplateRegionRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            MyBase.New(database)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal TemplateRegionId As Integer)
            MyBase.New(database, TemplateRegionId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal _Database As Database, ByVal TemplateRegionId As Integer) As ContentToolTemplateRegionRow
            Dim row As ContentToolTemplateRegionRow

            row = New ContentToolTemplateRegionRow(_Database, TemplateRegionId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal _Database As Database, ByVal TemplateRegionId As Integer)
            Dim row As ContentToolTemplateRegionRow

            row = New ContentToolTemplateRegionRow(_Database, TemplateRegionId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetCollectionByTemplateId(ByVal DB As Database, ByVal TemplateId As Integer) As ContentToolTemplateRegionCollection
            Dim col As New ContentToolTemplateRegionCollection
            Dim SQL As String = "select * from ContentToolTemplateRegion where TemplateId = " & TemplateId

            Dim dr As SqlDataReader = DB.GetReader(SQL)
            While dr.Read
                Dim region As New ContentToolTemplateRegionRow
                region.Load(dr)

                col.Add(region)
            End While
            dr.Close()

            Return col
        End Function

    End Class

    Public MustInherit Class ContentToolTemplateRegionRowBase
        Private m_DB As Database
        Private m_TemplateRegionId As Integer = Nothing
        Private m_TemplateId As Integer = Nothing
        Private m_ContentRegion As String = Nothing
        Private m_RegionName As String = Nothing

        Public Property TemplateRegionId() As Integer
            Get
                Return m_TemplateRegionId
            End Get
            Set(ByVal Value As Integer)
                m_TemplateRegionId = Value
            End Set
        End Property

        Public Property TemplateId() As Integer
            Get
                Return m_TemplateId
            End Get
            Set(ByVal Value As Integer)
                m_TemplateId = Value
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

        Public Property RegionName() As String
            Get
                Return m_RegionName
            End Get
            Set(ByVal Value As String)
                m_RegionName = Value
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

        Public Sub New(ByVal database As Database, ByVal TemplateRegionId As Integer)
            m_DB = database
            m_TemplateRegionId = TemplateRegionId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM ContentToolTemplateRegion WITH (NOLOCK) WHERE TemplateRegionId = " & DB.Quote(TemplateRegionId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Public Overridable Sub Load(ByVal r As SqlDataReader)
            m_TemplateRegionId = r.Item("TemplateRegionId")
            If r.Item("TemplateId") Is Convert.DBNull Then
                m_TemplateId = Nothing
            Else
                m_TemplateId = Convert.ToInt32(r.Item("TemplateId"))
            End If
            If r.Item("ContentRegion") Is Convert.DBNull Then
                m_ContentRegion = Nothing
            Else
                m_ContentRegion = Convert.ToString(r.Item("ContentRegion"))
            End If
            If r.Item("RegionName") Is Convert.DBNull Then
                m_RegionName = Nothing
            Else
                m_RegionName = Convert.ToString(r.Item("RegionName"))
            End If
        End Sub 'Load

        Private ReadOnly Property InsertStatement() As String
            Get
                Dim SQL As String

                SQL = " INSERT INTO ContentToolTemplateRegion (" _
                 & " TemplateId" _
                 & ",ContentRegion" _
                 & ",RegionName" _
                 & ") VALUES (" _
                 & m_DB.Quote(TemplateId) _
                 & "," & m_DB.Quote(ContentRegion) _
                 & "," & m_DB.Quote(RegionName) _
                 & ")"

                Return SQL
            End Get
        End Property

        Public Overridable Sub Insert()
            m_DB.ExecuteSQL(InsertStatement)
        End Sub 'Insert

        Function AutoInsert() As Integer
            TemplateRegionId = m_DB.InsertSQL(InsertStatement)
            Return TemplateRegionId
        End Function


        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE ContentToolTemplateRegion SET " _
             & " TemplateId = " & m_DB.Quote(TemplateId) _
             & ",ContentRegion = " & m_DB.Quote(ContentRegion) _
             & ",RegionName = " & m_DB.Quote(RegionName) _
             & " WHERE TemplateRegionId = " & m_DB.Quote(TemplateRegionId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM ContentToolTemplateRegion WHERE TemplateRegionId = " & m_DB.Quote(TemplateRegionId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class ContentToolTemplateRegionCollection
        Inherits GenericCollection(Of ContentToolTemplateRegionRow)
    End Class

End Namespace


