Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public MustInherit Class PORequestInfoThreadRowBase
        Private m_DB As Database
        Private m_ThreadId As Integer = Nothing
        Private m_QuoteId As Integer = Nothing
        Private m_Thread As String = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_BuilderId As Integer = Nothing
        Private m_VendorId As Integer = Nothing
        Private m_CreatedByUser As String = Nothing

        Public Property ThreadId() As Integer
            Get
                Return m_ThreadId
            End Get
            Set(ByVal Value As Integer)
                m_ThreadId = value
            End Set
        End Property

        Public Property QuoteId() As Integer
            Get
                Return m_QuoteId
            End Get
            Set(ByVal Value As Integer)
                m_QuoteId = value
            End Set
        End Property

        Public Property Thread() As String
            Get
                Return m_Thread
            End Get
            Set(ByVal Value As String)
                m_Thread = value
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

        Public Property VendorId() As Integer
            Get
                Return m_VendorId
            End Get
            Set(ByVal Value As Integer)
                m_VendorId = value
            End Set
        End Property

        Public Property CreatedByUser() As String
            Get
                Return m_CreatedByUser
            End Get
            Set(ByVal Value As String)
                m_CreatedByUser = value
            End Set
        End Property

        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
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

        Public Sub New(ByVal DB As Database, ByVal ThreadId As Integer)
            m_DB = DB
            m_ThreadId = ThreadId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM PORequestInfoThread WHERE ThreadId = " & DB.Number(ThreadId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_ThreadId = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_ThreadId = Core.GetInt(r.Item("ThreadId"))
            m_QuoteId = Core.GetInt(r.Item("QuoteId"))
            m_Thread = Core.GetString(r.Item("Thread"))
            m_CreateDate = Core.GetDate(r.Item("CreateDate"))
            m_BuilderId = Core.GetInt(r.Item("BuilderId"))
            m_VendorId = Core.GetInt(r.Item("VendorId"))
            m_CreatedByUser = Core.GetString(r.Item("CreatedByUser"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO PORequestInfoThread (" _
             & " QuoteId" _
             & ",Thread" _
             & ",CreateDate" _
             & ",BuilderId" _
             & ",VendorId" _
             & ",CreatedByUser" _
             & ") VALUES (" _
             & m_DB.NullNumber(QuoteId) _
             & "," & m_DB.Quote(Thread) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(BuilderId) _
             & "," & m_DB.NullNumber(VendorId) _
             & "," & m_DB.Quote(CreatedByUser) _
             & ")"

            ThreadId = m_DB.InsertSQL(SQL)

            Return ThreadId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE PORequestInfoThread WITH (ROWLOCK) SET " _
             & " QuoteId = " & m_DB.NullNumber(QuoteId) _
             & ",Thread = " & m_DB.Quote(Thread) _
             & ",BuilderId = " & m_DB.NullNumber(BuilderId) _
             & ",VendorId = " & m_DB.NullNumber(VendorId) _
             & ",CreatedByUser = " & m_DB.Quote(CreatedByUser) _
             & " WHERE ThreadId = " & m_DB.quote(ThreadId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class PORequestInfoThreadCollection
        Inherits GenericCollection(Of PORequestInfoThreadRow)
    End Class

End Namespace


