Option Explicit On 

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class ContentToolSectionRow
        Inherits ContentToolSectionRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            MyBase.New(database)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal SectionId As Integer)
            MyBase.New(database, SectionId)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal Folder As String)
            MyBase.New(database, Folder)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal _Database As Database, ByVal SectionId As Integer) As ContentToolSectionRow
            Dim row As ContentToolSectionRow

            row = New ContentToolSectionRow(_Database, SectionId)
            row.Load()

            Return row
        End Function

        Public Shared Function GetRowByDirectory(ByVal _Database As Database, ByVal Directory As String) As ContentToolSectionRow
            Dim row As New ContentToolSectionRow(_Database)
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM ContentToolSection WHERE Folder = " & _Database.Quote(Directory)
            r = _Database.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal _Database As Database, ByVal SectionId As Integer)
            Dim row As ContentToolSectionRow

            row = New ContentToolSectionRow(_Database, SectionId)
            row.Remove()
        End Sub

        'Custom Methods
    End Class

    Public MustInherit Class ContentToolSectionRowBase
        Private m_DB As Database
        Private m_SectionId As Integer = Nothing
        Private m_SectionName As String = Nothing
        Private m_Folder As String = Nothing

        Public Property SectionId() As Integer
            Get
                Return m_SectionId
            End Get
            Set(ByVal Value As Integer)
                m_SectionId = Value
            End Set
        End Property

        Public Property SectionName() As String
            Get
                Return m_SectionName
            End Get
            Set(ByVal Value As String)
                m_SectionName = Value
            End Set
        End Property

        Public Property Folder() As String
            Get
                Return m_Folder
            End Get
            Set(ByVal Value As String)
                m_Folder = Value
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

        Public Sub New(ByVal database As Database, ByVal SectionId As Integer)
            m_DB = database
            m_SectionId = SectionId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM ContentToolSection WHERE SectionId = " & DB.Quote(SectionId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_SectionId = r.Item("SectionId")
            If IsDBNull(r.Item("SectionName")) Then
                m_SectionName = Nothing
            Else
                m_SectionName = Convert.ToString(r.Item("SectionName"))
            End If
            If IsDBNull(r.Item("Folder")) Then
                m_Folder = Nothing
            Else
                m_Folder = Convert.ToString(r.Item("Folder"))
            End If
        End Sub 'Load

        Private ReadOnly Property InsertStatement() As String
            Get
                Dim SQL As String

                SQL = " INSERT INTO ContentToolSection (" _
                 & " SectionName" _
                 & ",Folder" _
                 & ") VALUES (" _
                 & m_DB.Quote(SectionName) _
                 & "," & m_DB.Quote(Folder) _
                 & ")"

                Return SQL
            End Get
        End Property

        Public Overridable Sub Insert()
            m_DB.ExecuteSQL(InsertStatement)
        End Sub 'Insert

        Function AutoInsert() As Integer
            SectionId = m_DB.InsertSQL(InsertStatement)
            Return SectionId
        End Function


        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE ContentToolSection SET " _
             & " SectionName = " & m_DB.Quote(SectionName) _
             & ",Folder = " & m_DB.Quote(Folder) _
             & " WHERE SectionId = " & m_DB.Quote(SectionId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM ContentToolSection WHERE SectionId = " & m_DB.Quote(SectionId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class ContentToolSectionCollection
        Inherits GenericCollection(Of ContentToolSectionRow)
    End Class

End Namespace


