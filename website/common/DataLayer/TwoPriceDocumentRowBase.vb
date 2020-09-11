Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public MustInherit Class TwoPriceDocumentRowBase
        Private m_DB As Database
        Private m_DocumentId As Integer = Nothing
        Private m_Title As String = Nothing
        Private m_FileName As String = Nothing
        Private m_GUID As String = Nothing
        Private m_Uploaded As DateTime = Nothing
        Private m_TwoPriceCampaignId As Integer = Nothing





        Public Property TwoPriceCampaignId() As Integer
            Get
                Return m_TwoPriceCampaignId
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceCampaignId = Value
            End Set
        End Property
        Public Property DocumentId() As Integer
            Get
                Return m_DocumentId
            End Get
            Set(ByVal Value As Integer)
                m_DocumentId = Value
            End Set
        End Property



        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(ByVal Value As String)
                m_Title = Value
            End Set
        End Property

        Public Property FileName() As String
            Get
                Return m_FileName
            End Get
            Set(ByVal Value As String)
                m_FileName = Value
            End Set
        End Property

        Public Property GUID() As String
            Get
                Return m_GUID
            End Get
            Set(ByVal Value As String)
                m_GUID = Value
            End Set
        End Property

        Public Property Uploaded() As DateTime
            Get
                Return m_Uploaded
            End Get
            Set(ByVal Value As DateTime)
                m_Uploaded = Value
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

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal TwoPriceDocumentId As Integer)
            m_DB = DB
            m_DocumentId = TwoPriceDocumentId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM  TwoPriceDocument WHERE DocumentId = " & DB.Number(DocumentId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                DocumentId = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_DocumentId = Core.GetInt(r.Item("DocumentId"))
            m_Title = Core.GetString(r.Item("Title"))
            m_FileName = Core.GetString(r.Item("FileName"))
            m_GUID = Core.GetString(r.Item("GUID"))
            m_Uploaded = Core.GetDate(r.Item("Uploaded"))
            m_TwoPriceCampaignId = Core.GetInt(r.Item("TwoPriceCampaignId"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO TwoPriceDocument (" _
             & "Title" _
             & ",FileName" _
             & ",GUID" _
             & ",Uploaded" _
             & ",TwoPriceCampaignId" _
             & ") VALUES (" _
             & m_DB.Quote(Title) _
             & "," & m_DB.Quote(FileName) _
             & "," & m_DB.Quote(GUID) _
             & "," & m_DB.NullQuote(Uploaded) _
             & "," & m_DB.Number(TwoPriceCampaignId) _
             & ")"

            DocumentId = m_DB.InsertSQL(SQL)

            Return DocumentId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE TwoPriceDocument WITH (ROWLOCK) SET " _
             & "Title = " & m_DB.Quote(Title) _
             & ",FileName = " & m_DB.Quote(FileName) _
             & ",GUID = " & m_DB.Quote(GUID) _
             & ",Uploaded = " & m_DB.NullQuote(Uploaded) _
                      & ",TwoPriceCampaignId = " & m_DB.Number(TwoPriceCampaignId) _
             & " WHERE  DocumentId = " & m_DB.Quote(DocumentId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class TwoPriceDocumentCollection
        Inherits GenericCollection(Of TwoPriceDocumentRow)
    End Class

End Namespace


