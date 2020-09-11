Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public MustInherit Class TwoPriceCampaignRowBase
        Private m_DB As Database
        Private m_TwoPriceCampaignId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_Status As String = Nothing
        Private m_StartDate As DateTime = Nothing
        Private m_EndDate As DateTime = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_UpdatePrice As Boolean = Nothing
        Private m_AwardedVendorId As Integer = Nothing
        Private m_VendorBidDeadline As DateTime = Nothing

        Public Property TwoPriceCampaignId As Integer
            Get
                Return m_TwoPriceCampaignId
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceCampaignId = value
            End Set
        End Property

        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(ByVal Value As String)
                m_Name = value
            End Set
        End Property

        Public Property Status As String
            Get
                Return m_Status
            End Get
            Set(ByVal Value As String)
                m_Status = value
            End Set
        End Property

        Public Property StartDate As DateTime
            Get
                Return m_StartDate
            End Get
            Set(ByVal Value As DateTime)
                m_StartDate = value
            End Set
        End Property

        Public Property EndDate As DateTime
            Get
                Return m_EndDate
            End Get
            Set(ByVal Value As DateTime)
                m_EndDate = value
            End Set
        End Property

        Public Property VendorBidDeadline As DateTime
            Get
                Return m_VendorBidDeadline
            End Get
            Set(ByVal Value As DateTime)
                m_VendorBidDeadline = Value
            End Set
        End Property

        Public Property IsActive As Boolean
            Get
                Return m_IsActive
            End Get
            Set(ByVal Value As Boolean)
                m_IsActive = value
            End Set
        End Property
        Public Property IsUpdatePrice As Boolean
            Get
                Return m_UpdatePrice

            End Get
            Set(ByVal Value As Boolean)
                m_UpdatePrice = Value
            End Set
        End Property

        Public Property AwardedVendorId As Integer
            Get
                Return m_AwardedVendorId
            End Get
            Set(ByVal Value As Integer)
                m_AwardedVendorId = value
            End Set
        End Property

        Public ReadOnly Property CreateDate As DateTime
            Get
                Return m_CreateDate
            End Get
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

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, TwoPriceCampaignId As Integer)
            m_DB = DB
            m_TwoPriceCampaignId = TwoPriceCampaignId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM TwoPriceCampaign WHERE TwoPriceCampaignId = " & DB.Number(TwoPriceCampaignId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_TwoPriceCampaignId = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_TwoPriceCampaignId = Core.GetInt(r.Item("TwoPriceCampaignId"))
            m_Name = Core.GetString(r.Item("Name"))
            m_Status = Core.GetString(r.Item("Status"))
            m_StartDate = Core.GetDate(r.Item("StartDate"))
            m_EndDate = Core.GetDate(r.Item("EndDate"))
            m_CreateDate = Core.GetDate(r.Item("CreateDate"))
            m_IsActive = Core.GetBoolean(r.Item("IsActive"))
            m_UpdatePrice = Core.GetBoolean(r.Item("AllowPriceUpdate"))
            m_AwardedVendorId = Core.GetInt(r.Item("AwardedVendorId"))
            m_VendorBidDeadline = Core.GetDate(r.Item("VendorBidDeadline"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO TwoPriceCampaign (" _
                & " Name" _
                & ",Status" _
                & ",StartDate" _
                & ",EndDate" _
                & ",CreateDate" _
                & ",IsActive" _
                & ",AllowPriceUpdate" _
                & ",AwardedVendorId" _
                & ") VALUES (" _
                & m_DB.Quote(Name) _
                & "," & m_DB.Quote(Status) _
                & "," & m_DB.NullQuote(StartDate) _
                & "," & m_DB.NullQuote(EndDate) _
                & "," & m_DB.NullQuote(Now) _
                & "," & CInt(IsActive) _
                & "," & CInt(IsUpdatePrice) _
                & "," & m_DB.NullNumber(AwardedVendorId) _
                & ")"

            TwoPriceCampaignId = m_DB.InsertSQL(SQL)

            Return TwoPriceCampaignId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE TwoPriceCampaign WITH (ROWLOCK) SET " _
                & " Name = " & m_DB.Quote(Name) _
                & ",Status = " & m_DB.Quote(Status) _
                & ",StartDate = " & m_DB.NullQuote(StartDate) _
                & ",EndDate = " & m_DB.NullQuote(EndDate) _
                & ",IsActive = " & CInt(IsActive) _
                & ",AllowPriceUpdate = " & CInt(IsUpdatePrice) _
                & ",AwardedVendorId = " & m_DB.NullNumber(AwardedVendorId) _
                & " WHERE TwoPriceCampaignId = " & m_DB.Quote(TwoPriceCampaignId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class TwoPriceCampaignCollection
        Inherits GenericCollection(Of TwoPriceCampaignRow)
    End Class

End Namespace
