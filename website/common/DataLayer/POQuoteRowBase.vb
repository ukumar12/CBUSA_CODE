Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public MustInherit Class POQuoteRowBase
        Private m_DB As Database
        Private m_QuoteId As Integer = Nothing
        Private m_ProjectId As Integer = Nothing
        Private m_Title As String = Nothing
        Private m_Deadline As DateTime = Nothing
        Private m_Instructions As String = Nothing
        Private m_Status As String = Nothing
        Private m_StatusDate As DateTime = Nothing
        Private m_AwardedToVendorId As Integer = Nothing
        Private m_AwardedDate As DateTime = Nothing
        Private m_AwardedTotal As Double = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_ModifyDate As DateTime = Nothing

        Public Property QuoteId() As Integer
            Get
                Return m_QuoteId
            End Get
            Set(ByVal Value As Integer)
                m_QuoteId = value
            End Set
        End Property

        Public Property ProjectId() As Integer
            Get
                Return m_ProjectId
            End Get
            Set(ByVal Value As Integer)
                m_ProjectId = value
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

        Public Property Deadline() As DateTime
            Get
                Return m_Deadline
            End Get
            Set(ByVal Value As DateTime)
                m_Deadline = value
            End Set
        End Property

        Public Property Instructions() As String
            Get
                Return m_Instructions
            End Get
            Set(ByVal Value As String)
                m_Instructions = value
            End Set
        End Property

        Public Property Status() As String
            Get
                Return m_Status
            End Get
            Set(ByVal Value As String)
                m_Status = value
            End Set
        End Property

        Public Property StatusDate() As DateTime
            Get
                Return m_StatusDate
            End Get
            Set(ByVal Value As DateTime)
                m_StatusDate = value
            End Set
        End Property

        Public Property AwardedToVendorId() As Integer
            Get
                Return m_AwardedToVendorId
            End Get
            Set(ByVal Value As Integer)
                m_AwardedToVendorId = value
            End Set
        End Property

        Public Property AwardedDate() As DateTime
            Get
                Return m_AwardedDate
            End Get
            Set(ByVal Value As DateTime)
                m_AwardedDate = value
            End Set
        End Property

        Public Property AwardedTotal() As Double
            Get
                Return m_AwardedTotal
            End Get
            Set(ByVal Value As Double)
                m_AwardedTotal = value
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

        Public Sub New(ByVal DB As Database, ByVal QuoteId As Integer)
            m_DB = DB
            m_QuoteId = QuoteId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM POQuote WHERE QuoteId = " & DB.Number(QuoteId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_QuoteId = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_QuoteId = Core.GetInt(r.Item("QuoteId"))
            m_ProjectId = Core.GetInt(r.Item("ProjectId"))
            m_Title = Core.GetString(r.Item("Title"))
            m_Deadline = Core.GetDate(r.Item("Deadline"))
            m_Instructions = Core.GetString(r.Item("Instructions"))
            m_Status = Core.GetString(r.Item("Status"))
            m_StatusDate = Core.GetDate(r.Item("StatusDate"))
            m_AwardedToVendorId = Core.GetInt(r.Item("AwardedToVendorId"))
            m_AwardedDate = Core.GetDate(r.Item("AwardedDate"))
            m_AwardedTotal = Core.GetDouble(r.Item("AwardedTotal"))
            m_CreateDate = Core.GetDate(r.Item("CreateDate"))
            m_ModifyDate = Core.GetDate(r.Item("ModifyDate"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO POQuote (" _
             & " ProjectId" _
             & ",Title" _
             & ",Deadline" _
             & ",Instructions" _
             & ",Status" _
             & ",StatusDate" _
             & ",AwardedToVendorId" _
             & ",AwardedDate" _
             & ",AwardedTotal" _
             & ",CreateDate" _
             & ",ModifyDate" _
             & ") VALUES (" _
             & m_DB.NullNumber(ProjectId) _
             & "," & m_DB.Quote(Title) _
             & "," & m_DB.NullQuote(Deadline) _
             & "," & m_DB.Quote(Instructions) _
             & "," & m_DB.Quote(Status) _
             & "," & m_DB.NullQuote(StatusDate) _
             & "," & m_DB.NullNumber(AwardedToVendorId) _
             & "," & m_DB.NullQuote(AwardedDate) _
             & "," & m_DB.Number(AwardedTotal) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullQuote(Now) _
             & ")"

            QuoteId = m_DB.InsertSQL(SQL)

            Return QuoteId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE POQuote WITH (ROWLOCK) SET " _
             & " ProjectId = " & m_DB.NullNumber(ProjectId) _
             & ",Title = " & m_DB.Quote(Title) _
             & ",Deadline = " & m_DB.NullQuote(Deadline) _
             & ",Instructions = " & m_DB.Quote(Instructions) _
             & ",Status = " & m_DB.Quote(Status) _
             & ",StatusDate = " & m_DB.NullQuote(StatusDate) _
             & ",AwardedToVendorId = " & m_DB.NullNumber(AwardedToVendorId) _
             & ",AwardedDate = " & m_DB.NullQuote(AwardedDate) _
             & ",AwardedTotal = " & m_DB.Number(AwardedTotal) _
             & ",ModifyDate = " & m_DB.NullQuote(Now) _
             & " WHERE QuoteId = " & m_DB.quote(QuoteId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class POQuoteCollection
        Inherits GenericCollection(Of POQuoteRow)
    End Class

End Namespace


