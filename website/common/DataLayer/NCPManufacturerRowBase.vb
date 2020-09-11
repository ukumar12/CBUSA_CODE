Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public MustInherit Class NCPManufacturerRowBase
        Private m_DB As Database
        Private m_NCPManufacturerID As Integer = Nothing
        Private m_HistoricID As Integer = Nothing
        Private m_ClassID As String = Nothing
        Private m_CompanyName As String = Nothing
        Private m_MailingAddress As String = Nothing
        Private m_MailingCity As String = Nothing
        Private m_MailingState As String = Nothing
        Private m_MailingZip As String = Nothing
        Private m_Website As String = Nothing
        Private m_PrimaryContactName As String = Nothing
        Private m_PrimaryContactEmail As String = Nothing
        Private m_PrimaryContactPhone As String = Nothing
        Private m_APContactName As String = Nothing
        Private m_APContactEmail As String = Nothing
        Private m_APContactPhone As String = Nothing
        Private m_PaymentTerms As String = Nothing

        Public Property NCPManufacturerID As Integer
            Get
                Return m_NCPManufacturerID
            End Get
            Set(ByVal Value As Integer)
                m_NCPManufacturerID = value
            End Set
        End Property

        Public Property HistoricID As Integer
            Get
                Return m_HistoricID
            End Get
            Set(ByVal Value As Integer)
                m_HistoricID = value
            End Set
        End Property

        Public Property ClassID As String
            Get
                Return m_ClassID
            End Get
            Set(ByVal Value As String)
                m_ClassID = value
            End Set
        End Property

        Public Property CompanyName As String
            Get
                Return m_CompanyName
            End Get
            Set(ByVal Value As String)
                m_CompanyName = value
            End Set
        End Property

        Public Property MailingAddress As String
            Get
                Return m_MailingAddress
            End Get
            Set(ByVal Value As String)
                m_MailingAddress = value
            End Set
        End Property

        Public Property MailingCity As String
            Get
                Return m_MailingCity
            End Get
            Set(ByVal Value As String)
                m_MailingCity = value
            End Set
        End Property

        Public Property MailingState As String
            Get
                Return m_MailingState
            End Get
            Set(ByVal Value As String)
                m_MailingState = value
            End Set
        End Property

        Public Property MailingZip As String
            Get
                Return m_MailingZip
            End Get
            Set(ByVal Value As String)
                m_MailingZip = value
            End Set
        End Property

        Public Property Website As String
            Get
                Return m_Website
            End Get
            Set(ByVal Value As String)
                m_Website = value
            End Set
        End Property

        Public Property PrimaryContactName As String
            Get
                Return m_PrimaryContactName
            End Get
            Set(ByVal Value As String)
                m_PrimaryContactName = value
            End Set
        End Property

        Public Property PrimaryContactEmail As String
            Get
                Return m_PrimaryContactEmail
            End Get
            Set(ByVal Value As String)
                m_PrimaryContactEmail = value
            End Set
        End Property

        Public Property PrimaryContactPhone As String
            Get
                Return m_PrimaryContactPhone
            End Get
            Set(ByVal Value As String)
                m_PrimaryContactPhone = value
            End Set
        End Property

        Public Property APContactName As String
            Get
                Return m_APContactName
            End Get
            Set(ByVal Value As String)
                m_APContactName = value
            End Set
        End Property

        Public Property APContactEmail As String
            Get
                Return m_APContactEmail
            End Get
            Set(ByVal Value As String)
                m_APContactEmail = value
            End Set
        End Property

        Public Property APContactPhone As String
            Get
                Return m_APContactPhone
            End Get
            Set(ByVal Value As String)
                m_APContactPhone = value
            End Set
        End Property

        Public Property PaymentTerms As String
            Get
                Return m_PaymentTerms
            End Get
            Set(ByVal Value As String)
                m_PaymentTerms = value
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

        Public Sub New(ByVal DB As Database, NCPManufacturerID As Integer)
            m_DB = DB
            m_NCPManufacturerID = NCPManufacturerID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM NCPManufacturer WHERE NCPManufacturerID = " & DB.Number(NCPManufacturerID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_NCPManufacturerID = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_NCPManufacturerID = Core.GetInt(r.Item("NCPManufacturerID"))
            m_HistoricID = Core.GetInt(r.Item("HistoricID"))
            m_ClassID = Core.GetString(r.Item("ClassID"))
            m_CompanyName = Core.GetString(r.Item("CompanyName"))
            m_MailingAddress = Core.GetString(r.Item("MailingAddress"))
            m_MailingCity = Core.GetString(r.Item("MailingCity"))
            m_MailingState = Core.GetString(r.Item("MailingState"))
            m_MailingZip = Core.GetString(r.Item("MailingZip"))
            m_Website = Core.GetString(r.Item("Website"))
            m_PrimaryContactName = Core.GetString(r.Item("PrimaryContactName"))
            m_PrimaryContactEmail = Core.GetString(r.Item("PrimaryContactEmail"))
            m_PrimaryContactPhone = Core.GetString(r.Item("PrimaryContactPhone"))
            m_APContactName = Core.GetString(r.Item("APContactName"))
            m_APContactEmail = Core.GetString(r.Item("APContactEmail"))
            m_APContactPhone = Core.GetString(r.Item("APContactPhone"))
            m_PaymentTerms = Core.GetString(r.Item("PaymentTerms"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String
            Dim MaximumHistoricId As Integer = Me.DB.ExecuteScalar("SELECT MAX(HistoricId) FROM (SELECT MAX(HistoricId) HistoricId FROM Builder UNION SELECT MAX(HistoricId) HistoricId FROM Vendor UNION SELECT MAX(HistoricId) HistoricId FROM NCPManufacturer) a")
            MaximumHistoricId = MaximumHistoricId + 1

            SQL = " INSERT INTO NCPManufacturer (" _
             & " HistoricID" _
             & ",ClassID" _
             & ",CompanyName" _
             & ",MailingAddress" _
             & ",MailingCity" _
             & ",MailingState" _
             & ",MailingZip" _
             & ",Website" _
             & ",PrimaryContactName" _
             & ",PrimaryContactEmail" _
             & ",PrimaryContactPhone" _
             & ",APContactName" _
             & ",APContactEmail" _
             & ",APContactPhone" _
             & ",PaymentTerms" _
             & ") VALUES (" _
             & m_DB.NullNumber(MaximumHistoricId) _
             & "," & m_DB.Quote(ClassID) _
             & "," & m_DB.Quote(CompanyName) _
             & "," & m_DB.Quote(MailingAddress) _
             & "," & m_DB.Quote(MailingCity) _
             & "," & m_DB.Quote(MailingState) _
             & "," & m_DB.Quote(MailingZip) _
             & "," & m_DB.Quote(Website) _
             & "," & m_DB.Quote(PrimaryContactName) _
             & "," & m_DB.Quote(PrimaryContactEmail) _
             & "," & m_DB.Quote(PrimaryContactPhone) _
             & "," & m_DB.Quote(APContactName) _
             & "," & m_DB.Quote(APContactEmail) _
             & "," & m_DB.Quote(APContactPhone) _
             & "," & m_DB.Quote(PaymentTerms) _
             & ")"

            NCPManufacturerID = m_DB.InsertSQL(SQL)

            Return NCPManufacturerID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE NCPManufacturer WITH (ROWLOCK) SET " _
             & " HistoricID = " & m_DB.NullNumber(HistoricID) _
             & ",ClassID = " & m_DB.Quote(ClassID) _
             & ",CompanyName = " & m_DB.Quote(CompanyName) _
             & ",MailingAddress = " & m_DB.Quote(MailingAddress) _
             & ",MailingCity = " & m_DB.Quote(MailingCity) _
             & ",MailingState = " & m_DB.Quote(MailingState) _
             & ",MailingZip = " & m_DB.Quote(MailingZip) _
             & ",Website = " & m_DB.Quote(Website) _
             & ",PrimaryContactName = " & m_DB.Quote(PrimaryContactName) _
             & ",PrimaryContactEmail = " & m_DB.Quote(PrimaryContactEmail) _
             & ",PrimaryContactPhone = " & m_DB.Quote(PrimaryContactPhone) _
             & ",APContactName = " & m_DB.Quote(APContactName) _
             & ",APContactEmail = " & m_DB.Quote(APContactEmail) _
             & ",APContactPhone = " & m_DB.Quote(APContactPhone) _
             & ",PaymentTerms = " & m_DB.Quote(PaymentTerms) _
             & " WHERE NCPManufacturerID = " & m_DB.quote(NCPManufacturerID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class NCPManufacturerCollection
        Inherits GenericCollection(Of NCPManufacturerRow)
    End Class

End Namespace

