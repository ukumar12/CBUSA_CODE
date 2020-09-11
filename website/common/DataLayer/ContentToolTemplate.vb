Option Explicit On 

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class ContentToolTemplateRow
        Inherits ContentToolTemplateRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            MyBase.New(database)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal TemplateId As Integer)
            MyBase.New(database, TemplateId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal _Database As Database, ByVal TemplateId As Integer) As ContentToolTemplateRow
            Dim row As ContentToolTemplateRow

            row = New ContentToolTemplateRow(_Database, TemplateId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal _Database As Database, ByVal TemplateId As Integer)
            Dim row As ContentToolTemplateRow

            row = New ContentToolTemplateRow(_Database, TemplateId)
            row.Remove()
        End Sub

        'Custom Methods

    End Class

    Public MustInherit Class ContentToolTemplateRowBase
        Private m_DB As Database
        Private m_TemplateId As Integer = Nothing
        Private m_TemplateName As String = Nothing
        Private m_DefaultContentId As Integer = Nothing
        Private m_IsDefault As Boolean = Nothing
        Private m_TemplateHTML As String = Nothing
        Private m_PrintHTML As String = Nothing

        Public Property TemplateId() As Integer
            Get
                Return m_TemplateId
            End Get
            Set(ByVal Value As Integer)
                m_TemplateId = Value
            End Set
        End Property

        Public Property TemplateName() As String
            Get
                Return m_TemplateName
            End Get
            Set(ByVal Value As String)
                m_TemplateName = Value
            End Set
        End Property

        Public Property DefaultContentId() As Integer
            Get
                Return m_DefaultContentId
            End Get
            Set(ByVal Value As Integer)
                m_DefaultContentId = Value
            End Set
        End Property

        Public Property IsDefault() As Boolean
            Get
                Return m_IsDefault
            End Get
            Set(ByVal Value As Boolean)
                m_IsDefault = Value
            End Set
        End Property

        Public Property TemplateHTML() As String
            Get
                Return m_TemplateHTML
            End Get
            Set(ByVal Value As String)
                m_TemplateHTML = Value
            End Set
        End Property

        Public Property PrintHTML() As String
            Get
                Return m_PrintHTML
            End Get
            Set(ByVal Value As String)
                m_PrintHTML = Value
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

        Public Sub New(ByVal database As Database, ByVal TemplateId As Integer)
            m_DB = database
            m_TemplateId = TemplateId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM ContentToolTemplate WITH (NOLOCK) WHERE TemplateId = " & DB.Quote(TemplateId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            TemplateId = r.Item("TemplateId")
            If r.Item("TemplateName") Is Convert.DBNull Then
                m_TemplateName = Nothing
            Else
                m_TemplateName = Convert.ToString(r.Item("TemplateName"))
            End If
            If r.Item("DefaultContentId") Is Convert.DBNull Then
                m_DefaultContentId = Nothing
            Else
                m_DefaultContentId = Convert.ToInt32(r.Item("DefaultContentId"))
            End If
            m_IsDefault = Convert.ToBoolean(r.Item("IsDefault"))
            If r.Item("TemplateHTML") Is Convert.DBNull Then
                m_TemplateHTML = Nothing
            Else
                m_TemplateHTML = Convert.ToString(r.Item("TemplateHTML"))
            End If
            If r.Item("PrintHTML") Is Convert.DBNull Then
                m_PrintHTML = Nothing
            Else
                m_PrintHTML = Convert.ToString(r.Item("PrintHTML"))
            End If
        End Sub 'Load

        Private ReadOnly Property InsertStatement() As String
            Get
                Dim SQL As String

                SQL = " INSERT INTO ContentToolTemplate (" _
                 & " TemplateName" _
                 & ",DefaultContentId" _
                 & ",IsDefault" _
                 & ",TemplateHTML" _
                 & ",PrintHTML" _
                 & ") VALUES (" _
                 & m_DB.Quote(TemplateName) _
                 & "," & m_DB.Quote(DefaultContentId) _
                 & "," & CInt(IsDefault) _
                 & "," & m_DB.Quote(TemplateHTML) _
                 & "," & m_DB.Quote(PrintHTML) _
                 & ")"

                Return SQL
            End Get
        End Property

        Public Overridable Sub Insert()
            m_DB.ExecuteSQL(InsertStatement)
        End Sub 'Insert

        Function AutoInsert() As Integer
            TemplateId = m_DB.InsertSQL(InsertStatement)
            Return TemplateId
        End Function


        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE ContentToolTemplate SET " _
             & " TemplateName = " & m_DB.Quote(TemplateName) _
             & ",DefaultContentId = " & m_DB.Quote(DefaultContentId) _
             & ",IsDefault = " & CInt(IsDefault) _
             & ",TemplateHTML = " & m_DB.Quote(TemplateHTML) _
             & ",PrintHTML = " & m_DB.Quote(PrintHTML) _
             & " WHERE TemplateId = " & m_DB.Quote(TemplateId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM ContentToolTemplate WHERE TemplateId = " & m_DB.Quote(TemplateId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class ContentToolTemplateCollection
        Inherits GenericCollection(Of ContentToolTemplateRow)
    End Class

End Namespace


