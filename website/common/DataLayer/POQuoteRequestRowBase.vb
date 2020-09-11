Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public MustInherit Class POQuoteRequestRowBase
        Private m_DB As Database
        Private m_QuoteRequestId As Integer = Nothing
        Private m_QuoteId As Integer = Nothing
        Private m_VendorId As Integer = Nothing
        Private m_RequestStatus As String = Nothing
        Private m_VendorContactName As String = Nothing
        Private m_VendorContactPhone As String = Nothing
        Private m_VendorContactEmail As String = Nothing
        Private m_QuoteTotal As Double = Nothing
        Private m_QuoteExpirationDate As DateTime = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_ModifyDate As DateTime = Nothing
        Private m_StartDate As String = Nothing
        Private m_CompletionTime As String = Nothing
        Private m_PaymentTerms As String = Nothing


        Public Property QuoteRequestId() As Integer
            Get
                Return m_QuoteRequestId
            End Get
            Set(ByVal Value As Integer)
                m_QuoteRequestId = value
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

        Public Property VendorId() As Integer
            Get
                Return m_VendorId
            End Get
            Set(ByVal Value As Integer)
                m_VendorId = value
            End Set
        End Property

        Public Property RequestStatus() As String
            Get
                Return m_RequestStatus
            End Get
            Set(ByVal Value As String)
                m_RequestStatus = value
            End Set
        End Property

        Public Property VendorContactName() As String
            Get
                Return m_VendorContactName
            End Get
            Set(ByVal Value As String)
                m_VendorContactName = value
            End Set
        End Property

        Public Property VendorContactPhone() As String
            Get
                Return m_VendorContactPhone
            End Get
            Set(ByVal Value As String)
                m_VendorContactPhone = value
            End Set
        End Property

        Public Property VendorContactEmail() As String
            Get
                Return m_VendorContactEmail
            End Get
            Set(ByVal Value As String)
                m_VendorContactEmail = value
            End Set
        End Property

        Public Property QuoteTotal() As Double
            Get
                Return m_QuoteTotal
            End Get
            Set(ByVal Value As Double)
                m_QuoteTotal = value
            End Set
        End Property

        Public Property QuoteExpirationDate() As DateTime
            Get
                Return m_QuoteExpirationDate
            End Get
            Set(ByVal Value As DateTime)
                m_QuoteExpirationDate = value
            End Set
        End Property

        Public Property StartDate() As String
            Get
                Return m_StartDate
            End Get
            Set(ByVal Value As String)
                m_StartDate = Value
            End Set
        End Property

        Public Property CompletionTime() As String
            Get
                Return m_CompletionTime
            End Get
            Set(ByVal Value As String)
                m_CompletionTime = value
            End Set
        End Property

        Public Property PaymentTerms() As String
            Get
                Return m_PaymentTerms
            End Get
            Set(ByVal Value As String)
                m_PaymentTerms = value
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

        Public Sub New(ByVal DB As Database, ByVal QuoteRequestId As Integer)
            m_DB = DB
            m_QuoteRequestId = QuoteRequestId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM POQuoteRequest WHERE QuoteRequestId = " & DB.Number(QuoteRequestId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_QuoteRequestId = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_QuoteRequestId = Core.GetInt(r.Item("QuoteRequestId"))
            m_QuoteId = Core.GetInt(r.Item("QuoteId"))
            m_VendorId = Core.GetInt(r.Item("VendorId"))
            m_RequestStatus = Core.GetString(r.Item("RequestStatus"))
            m_VendorContactName = Core.GetString(r.Item("VendorContactName"))
            m_VendorContactPhone = Core.GetString(r.Item("VendorContactPhone"))
            m_VendorContactEmail = Core.GetString(r.Item("VendorContactEmail"))
            m_QuoteTotal = Core.GetDouble(r.Item("QuoteTotal"))
            m_QuoteExpirationDate = Core.GetDate(r.Item("QuoteExpirationDate"))
            m_CreateDate = Core.GetDate(r.Item("CreateDate"))
            m_ModifyDate = Core.GetDate(r.Item("ModifyDate"))
            m_StartDate = Core.GetString(r.Item("StartDate"))
            m_CompletionTime = Core.GetString(r.Item("CompletionTime"))
            m_PaymentTerms = Core.GetString(r.Item("PaymentTerms"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO POQuoteRequest (" _
             & " QuoteId" _
             & ",VendorId" _
             & ",RequestStatus" _
             & ",VendorContactName" _
             & ",VendorContactPhone" _
             & ",VendorContactEmail" _
             & ",QuoteTotal" _
             & ",QuoteExpirationDate" _
             & ",CreateDate" _
             & ",ModifyDate" _
             & ",StartDate" _
             & ",CompletionTime" _
             & ",PaymentTerms" _
             & ") VALUES (" _
             & m_DB.NullNumber(QuoteId) _
             & "," & m_DB.NullNumber(VendorId) _
             & "," & m_DB.Quote(RequestStatus) _
             & "," & m_DB.Quote(VendorContactName) _
             & "," & m_DB.Quote(VendorContactPhone) _
             & "," & m_DB.Quote(VendorContactEmail) _
             & "," & m_DB.Number(QuoteTotal) _
             & "," & m_DB.NullQuote(QuoteExpirationDate) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.Quote(StartDate) _
             & "," & m_DB.Quote(CompletionTime) _
             & "," & m_DB.Quote(PaymentTerms) _
             & ")"

            QuoteRequestId = m_DB.InsertSQL(SQL)

            Return QuoteRequestId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE POQuoteRequest WITH (ROWLOCK) SET " _
             & " QuoteId = " & m_DB.NullNumber(QuoteId) _
             & ",VendorId = " & m_DB.NullNumber(VendorId) _
             & ",RequestStatus = " & m_DB.Quote(RequestStatus) _
             & ",VendorContactName = " & m_DB.Quote(VendorContactName) _
             & ",VendorContactPhone = " & m_DB.Quote(VendorContactPhone) _
             & ",VendorContactEmail = " & m_DB.Quote(VendorContactEmail) _
             & ",QuoteTotal = " & m_DB.Number(QuoteTotal) _
             & ",QuoteExpirationDate = " & m_DB.NullQuote(QuoteExpirationDate) _
             & ",ModifyDate = " & m_DB.NullQuote(Now) _
             & ",StartDate = " & m_DB.Quote(StartDate) _
             & ",CompletionTime = " & m_DB.Quote(CompletionTime) _
             & ",PaymentTerms = " & m_DB.Quote(PaymentTerms) _
             & " WHERE QuoteRequestId = " & m_DB.Quote(QuoteRequestId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class POQuoteRequestCollection
        Inherits GenericCollection(Of POQuoteRequestRow)
    End Class

End Namespace


