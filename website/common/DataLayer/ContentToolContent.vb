Option Explicit On 

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class ContentToolContentRow
        Inherits ContentToolContentRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            MyBase.New(database)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal ContentId As Integer)
            MyBase.New(database, ContentId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal _Database As Database, ByVal ContentId As Integer) As ContentToolContentRow
            Dim row As ContentToolContentRow

            row = New ContentToolContentRow(_Database, ContentId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal _Database As Database, ByVal ContentId As Integer)
            Dim row As ContentToolContentRow

            row = New ContentToolContentRow(_Database, ContentId)
            row.Remove()
        End Sub

        'Custom Methods

    End Class

    Public MustInherit Class ContentToolContentRowBase
        Private m_DB As Database
        Private m_ContentId As Integer = Nothing
        Private m_Content As String = Nothing


        Public Property ContentId() As Integer
            Get
                Return m_ContentId
            End Get
            Set(ByVal Value As Integer)
                m_ContentId = Value
            End Set
        End Property

        Public Property Content() As String
            Get
                Return m_Content
            End Get
            Set(ByVal Value As String)
                m_Content = Value
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

        Public Sub New(ByVal database As Database, ByVal ContentId As Integer)
            m_DB = database
            m_ContentId = ContentId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader = Nothing
            Dim SQL As String

            SQL = "SELECT * FROM ContentToolContent WITH (NOLOCK) WHERE ContentId = " & DB.Quote(ContentId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            If r.Item("Content") Is Convert.DBNull Then
                m_Content = Nothing
            Else
                m_Content = Convert.ToString(r.Item("Content"))
            End If
        End Sub 'Load

        Private ReadOnly Property InsertStatement() As String
            Get
                Dim SQL As String

                SQL = " INSERT INTO ContentToolContent (" _
                 & " Content" _
                 & ") VALUES (" _
                 & m_DB.Quote(Content) _
                 & ")"

                Return SQL
            End Get
        End Property

        Public Overridable Sub Insert()
            m_DB.ExecuteSQL(InsertStatement)
        End Sub 'Insert

        Function AutoInsert() As Integer
            ContentId = m_DB.InsertSQL(InsertStatement)
            Return ContentId
        End Function


        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE ContentToolContent SET " _
             & " Content = " & m_DB.Quote(Content) _
             & " WHERE ContentId = " & m_DB.Quote(ContentId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM ContentToolContent WHERE ContentId = " & m_DB.Quote(ContentId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class ContentToolContentCollection
        Inherits GenericCollection(Of ContentToolContentRow)
    End Class

End Namespace


