Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public MustInherit Class PORequestInfoMessageRowBase
        Private m_DB As Database
        Private m_MessageId As Integer = Nothing
        Private m_ThreadId As Integer = Nothing
        Private m_Message As String = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_BuilderId As Integer = Nothing
        Private m_VendorId As Integer = Nothing
        Private m_CreatedByUser As String = Nothing

        Public Property MessageId() As Integer
            Get
                Return m_MessageId
            End Get
            Set(ByVal Value As Integer)
                m_MessageId = value
            End Set
        End Property

        Public Property ThreadId() As Integer
            Get
                Return m_ThreadId
            End Get
            Set(ByVal Value As Integer)
                m_ThreadId = value
            End Set
        End Property

        Public Property Message() As String
            Get
                Return m_Message
            End Get
            Set(ByVal Value As String)
                m_Message = value
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

        Public Sub New(ByVal DB As Database, ByVal MessageId As Integer)
            m_DB = DB
            m_MessageId = MessageId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM PORequestInfoMessage WHERE MessageId = " & DB.Number(MessageId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_MessageId = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_MessageId = Core.GetInt(r.Item("MessageId"))
            m_ThreadId = Core.GetInt(r.Item("ThreadId"))
            m_Message = Core.GetString(r.Item("Message"))
            m_CreateDate = Core.GetDate(r.Item("CreateDate"))
            m_BuilderId = Core.GetInt(r.Item("BuilderId"))
            m_VendorId = Core.GetInt(r.Item("VendorId"))
            m_CreatedByUser = Core.GetString(r.Item("CreatedByUser"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO PORequestInfoMessage (" _
             & " ThreadId" _
             & ",Message" _
             & ",CreateDate" _
             & ",BuilderId" _
             & ",VendorId" _
             & ",CreatedByUser" _
             & ") VALUES (" _
             & m_DB.NullNumber(ThreadId) _
             & "," & m_DB.Quote(Message) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(BuilderId) _
             & "," & m_DB.NullNumber(VendorId) _
             & "," & m_DB.Quote(CreatedByUser) _
             & ")"

            MessageId = m_DB.InsertSQL(SQL)

            Return MessageId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE PORequestInfoMessage WITH (ROWLOCK) SET " _
             & " ThreadId = " & m_DB.NullNumber(ThreadId) _
             & ",Message = " & m_DB.Quote(Message) _
             & ",BuilderId = " & m_DB.NullNumber(BuilderId) _
             & ",VendorId = " & m_DB.NullNumber(VendorId) _
             & ",CreatedByUser = " & m_DB.Quote(CreatedByUser) _
             & " WHERE MessageId = " & m_DB.quote(MessageId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class PORequestInfoMessageCollection
        Inherits GenericCollection(Of PORequestInfoMessageRow)
    End Class

End Namespace


