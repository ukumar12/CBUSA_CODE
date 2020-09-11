Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public MustInherit Class POQuoteRequestMessageRowBase
        Private m_DB As Database
        Private m_MessageId As Integer = Nothing
        Private m_QuoteRequestId As Integer = Nothing
        Private m_FromBuilderId As Integer = Nothing
        Private m_FromVendorId As Integer = Nothing
        Private m_FromName As String = Nothing
        Private m_MessageQuoteStatus As String = Nothing
        Private m_MessageQuoteTotal As Double = Nothing
        Private m_MessageQuoteExpirationDate As DateTime = Nothing
        Private m_Message As String = Nothing
        Private m_IsRead As Boolean = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_ModifyDate As DateTime = Nothing

        Public Property MessageId() As Integer
            Get
                Return m_MessageId
            End Get
            Set(ByVal Value As Integer)
                m_MessageId = value
            End Set
        End Property

        Public Property QuoteRequestId() As Integer
            Get
                Return m_QuoteRequestId
            End Get
            Set(ByVal Value As Integer)
                m_QuoteRequestId = value
            End Set
        End Property

        Public Property FromBuilderId() As Integer
            Get
                Return m_FromBuilderId
            End Get
            Set(ByVal Value As Integer)
                m_FromBuilderId = value
            End Set
        End Property

        Public Property FromVendorId() As Integer
            Get
                Return m_FromVendorId
            End Get
            Set(ByVal Value As Integer)
                m_FromVendorId = value
            End Set
        End Property

        Public Property FromName() As String
            Get
                Return m_FromName
            End Get
            Set(ByVal Value As String)
                m_FromName = value
            End Set
        End Property

        Public Property MessageQuoteStatus() As String
            Get
                Return m_MessageQuoteStatus
            End Get
            Set(ByVal Value As String)
                m_MessageQuoteStatus = value
            End Set
        End Property

        Public Property MessageQuoteTotal() As Double
            Get
                Return m_MessageQuoteTotal
            End Get
            Set(ByVal Value As Double)
                m_MessageQuoteTotal = value
            End Set
        End Property

        Public Property MessageQuoteExpirationDate() As DateTime
            Get
                Return m_MessageQuoteExpirationDate
            End Get
            Set(ByVal Value As DateTime)
                m_MessageQuoteExpirationDate = value
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

        Public Property IsRead() As Boolean
            Get
                Return m_IsRead
            End Get
            Set(ByVal Value As Boolean)
                m_IsRead = value
            End Set
        End Property

        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
        End Property

        Public ReadOnly Property ModifyDate() As DateTime
            Get
                Return m_ModifyDate
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

            SQL = "SELECT * FROM POQuoteRequestMessage WHERE MessageId = " & DB.Number(MessageId)
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
            m_QuoteRequestId = Core.GetInt(r.Item("QuoteRequestId"))
            m_FromBuilderId = Core.GetInt(r.Item("FromBuilderId"))
            m_FromVendorId = Core.GetInt(r.Item("FromVendorId"))
            m_FromName = Core.GetString(r.Item("FromName"))
            m_MessageQuoteStatus = Core.GetString(r.Item("MessageQuoteStatus"))
            m_MessageQuoteTotal = Core.GetDouble(r.Item("MessageQuoteTotal"))
            m_MessageQuoteExpirationDate = Core.GetDate(r.Item("MessageQuoteExpirationDate"))
            m_Message = Core.GetString(r.Item("Message"))
            m_IsRead = Core.GetBoolean(r.Item("IsRead"))
            m_CreateDate = Core.GetDate(r.Item("CreateDate"))
            m_ModifyDate = Core.GetDate(r.Item("ModifyDate"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO POQuoteRequestMessage (" _
             & " QuoteRequestId" _
             & ",FromBuilderId" _
             & ",FromVendorId" _
             & ",FromName" _
             & ",MessageQuoteStatus" _
             & ",MessageQuoteTotal" _
             & ",MessageQuoteExpirationDate" _
             & ",Message" _
             & ",IsRead" _
             & ",CreateDate" _
             & ",ModifyDate" _
             & ") VALUES (" _
             & m_DB.NullNumber(QuoteRequestId) _
             & "," & m_DB.NullNumber(FromBuilderId) _
             & "," & m_DB.NullNumber(FromVendorId) _
             & "," & m_DB.Quote(FromName) _
             & "," & m_DB.Quote(MessageQuoteStatus) _
             & "," & m_DB.Number(MessageQuoteTotal) _
             & "," & m_DB.NullQuote(MessageQuoteExpirationDate) _
             & "," & m_DB.Quote(Message) _
             & "," & CInt(IsRead) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullQuote(Now) _
             & ")"

            MessageId = m_DB.InsertSQL(SQL)

            Return MessageId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE POQuoteRequestMessage WITH (ROWLOCK) SET " _
             & " QuoteRequestId = " & m_DB.NullNumber(QuoteRequestId) _
             & ",FromBuilderId = " & m_DB.NullNumber(FromBuilderId) _
             & ",FromVendorId = " & m_DB.NullNumber(FromVendorId) _
             & ",FromName = " & m_DB.Quote(FromName) _
             & ",MessageQuoteStatus = " & m_DB.Quote(MessageQuoteStatus) _
             & ",MessageQuoteTotal = " & m_DB.Number(MessageQuoteTotal) _
             & ",MessageQuoteExpirationDate = " & m_DB.NullQuote(MessageQuoteExpirationDate) _
             & ",Message = " & m_DB.Quote(Message) _
             & ",IsRead = " & CInt(IsRead) _
             & ",ModifyDate = " & m_DB.NullQuote(Now) _
             & " WHERE MessageId = " & m_DB.quote(MessageId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class POQuoteRequestMessageCollection
        Inherits GenericCollection(Of POQuoteRequestMessageRow)
    End Class

End Namespace


