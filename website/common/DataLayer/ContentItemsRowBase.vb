Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public MustInherit Class ContentItemsRowBase
        Private m_DB As Database
        Private m_ContentItemsID As Integer = Nothing
        Private m_AdminID As Integer = Nothing
        Private m_Title As String = Nothing
        Private m_FileName As String = Nothing
        Private m_Uploaded As DateTime = Nothing

        Public Property ContentItemsID As Integer
            Get
                Return m_ContentItemsID
            End Get
            Set(ByVal Value As Integer)
                m_ContentItemsID = value
            End Set
        End Property

        Public Property AdminID As Integer
            Get
                Return m_AdminID
            End Get
            Set(ByVal Value As Integer)
                m_AdminID = value
            End Set
        End Property

        Public Property Title As String
            Get
                Return m_Title
            End Get
            Set(ByVal Value As String)
                m_Title = value
            End Set
        End Property

        Public Property FileName As String
            Get
                Return m_FileName
            End Get
            Set(ByVal Value As String)
                m_FileName = value
            End Set
        End Property

        Public Property Uploaded As DateTime
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

        Public Sub New(ByVal DB As Database, ContentItemsID As Integer)
            m_DB = DB
            m_ContentItemsID = ContentItemsID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM ContentItems WHERE ContentItemsID = " & DB.Number(ContentItemsID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_ContentItemsID = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_ContentItemsID = Core.GetInt(r.Item("ContentItemsID"))
            m_AdminID = Core.GetInt(r.Item("AdminID"))
            m_Title = Core.GetString(r.Item("Title"))
            m_FileName = Core.GetString(r.Item("FileName"))
            m_Uploaded = Core.GetDate(r.Item("Uploaded"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO ContentItems (" _
             & " AdminID" _
             & ",Title" _
             & ",FileName" _
             & ",Uploaded" _
             & ") VALUES (" _
             & m_DB.NullNumber(AdminID) _
             & "," & m_DB.Quote(Title) _
             & "," & m_DB.Quote(FileName) _
             & "," & m_DB.NullQuote(Uploaded) _
             & ")"

            ContentItemsID = m_DB.InsertSQL(SQL)

            Return ContentItemsID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE ContentItems WITH (ROWLOCK) SET " _
             & " AdminID = " & m_DB.NullNumber(AdminID) _
             & ",Title = " & m_DB.Quote(Title) _
             & ",FileName = " & m_DB.Quote(FileName) _
             & ",Uploaded = " & m_DB.NullQuote(Uploaded) _
             & " WHERE ContentItemsID = " & m_DB.quote(ContentItemsID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class ContentItemsCollection
        Inherits GenericCollection(Of ContentItemsRow)
    End Class

End Namespace

