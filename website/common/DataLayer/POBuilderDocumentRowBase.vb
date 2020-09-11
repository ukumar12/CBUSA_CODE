Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public MustInherit Class POBuilderDocumentRowBase
        Private m_DB As Database
        Private m_BuilderDocumentId As Integer = Nothing
        Private m_BuilderId As Integer = Nothing
        Private m_Title As String = Nothing
        Private m_FileName As String = Nothing
        Private m_GUID As String = Nothing
        Private m_Uploaded As DateTime = Nothing

        Public Property BuilderDocumentId() As Integer
            Get
                Return m_BuilderDocumentId
            End Get
            Set(ByVal Value As Integer)
                m_BuilderDocumentId = value
            End Set
        End Property

        Public Property BuilderId() As Integer
            Get
                Return m_BuilderId
            End Get
            Set(ByVal Value As Integer)
                m_BuilderId = value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(ByVal Value As String)
                m_Title = value
            End Set
        End Property

        Public Property FileName() As String
            Get
                Return m_FileName
            End Get
            Set(ByVal Value As String)
                m_FileName = value
            End Set
        End Property

        Public Property GUID() As String
            Get
                Return m_GUID
            End Get
            Set(ByVal Value As String)
                m_GUID = value
            End Set
        End Property

        Public Property Uploaded() As DateTime
            Get
                Return m_Uploaded
            End Get
            Set(ByVal Value As DateTime)
                m_Uploaded = value
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

        Public Sub New(ByVal DB As Database, ByVal BuilderDocumentId As Integer)
            m_DB = DB
            m_BuilderDocumentId = BuilderDocumentId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM POBuilderDocument WHERE BuilderDocumentId = " & DB.Number(BuilderDocumentId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_BuilderDocumentId = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_BuilderDocumentId = Core.GetInt(r.Item("BuilderDocumentId"))
            m_BuilderId = Core.GetInt(r.Item("BuilderId"))
            m_Title = Core.GetString(r.Item("Title"))
            m_FileName = Core.GetString(r.Item("FileName"))
            m_GUID = Core.GetString(r.Item("GUID"))
            m_Uploaded = Core.GetDate(r.Item("Uploaded"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO POBuilderDocument (" _
             & " BuilderId" _
             & ",Title" _
             & ",FileName" _
             & ",GUID" _
             & ",Uploaded" _
             & ") VALUES (" _
             & m_DB.NullNumber(BuilderId) _
             & "," & m_DB.Quote(Title) _
             & "," & m_DB.Quote(FileName) _
             & "," & m_DB.Quote(GUID) _
             & "," & m_DB.NullQuote(Uploaded) _
             & ")"

            BuilderDocumentId = m_DB.InsertSQL(SQL)

            Return BuilderDocumentId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE POBuilderDocument WITH (ROWLOCK) SET " _
             & " BuilderId = " & m_DB.NullNumber(BuilderId) _
             & ",Title = " & m_DB.Quote(Title) _
             & ",FileName = " & m_DB.Quote(FileName) _
             & ",GUID = " & m_DB.Quote(GUID) _
             & ",Uploaded = " & m_DB.NullQuote(Uploaded) _
             & " WHERE BuilderDocumentId = " & m_DB.quote(BuilderDocumentId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class POBuilderDocumentCollection
        Inherits GenericCollection(Of POBuilderDocumentRow)
    End Class

End Namespace


