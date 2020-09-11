Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public MustInherit Class POVendorDocumentRowBase
        Private m_DB As Database
        Private m_VendorDocumentId As Integer = Nothing
        Private m_VendorId As Integer = Nothing
        Private m_Title As String = Nothing
        Private m_FileName As String = Nothing
        Private m_GUID As String = Nothing
        Private m_Uploaded As DateTime = Nothing

        Public Property VendorDocumentId() As Integer
            Get
                Return m_VendorDocumentId
            End Get
            Set(ByVal Value As Integer)
                m_VendorDocumentId = value
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

        Public Sub New(ByVal DB As Database, ByVal VendorDocumentId As Integer)
            m_DB = DB
            m_VendorDocumentId = VendorDocumentId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM POVendorDocument WHERE VendorDocumentId = " & DB.Number(VendorDocumentId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_VendorDocumentId = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_VendorDocumentId = Core.GetInt(r.Item("VendorDocumentId"))
            m_VendorId = Core.GetInt(r.Item("VendorId"))
            m_Title = Core.GetString(r.Item("Title"))
            m_FileName = Core.GetString(r.Item("FileName"))
            m_GUID = Core.GetString(r.Item("GUID"))
            m_Uploaded = Core.GetDate(r.Item("Uploaded"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO POVendorDocument (" _
             & " VendorId" _
             & ",Title" _
             & ",FileName" _
             & ",GUID" _
             & ",Uploaded" _
             & ") VALUES (" _
             & m_DB.NullNumber(VendorId) _
             & "," & m_DB.Quote(Title) _
             & "," & m_DB.Quote(FileName) _
             & "," & m_DB.Quote(GUID) _
             & "," & m_DB.NullQuote(Uploaded) _
             & ")"

            VendorDocumentId = m_DB.InsertSQL(SQL)

            Return VendorDocumentId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE POVendorDocument WITH (ROWLOCK) SET " _
             & " VendorId = " & m_DB.NullNumber(VendorId) _
             & ",Title = " & m_DB.Quote(Title) _
             & ",FileName = " & m_DB.Quote(FileName) _
             & ",GUID = " & m_DB.Quote(GUID) _
             & ",Uploaded = " & m_DB.NullQuote(Uploaded) _
             & " WHERE VendorDocumentId = " & m_DB.quote(VendorDocumentId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class POVendorDocumentCollection
        Inherits GenericCollection(Of POVendorDocumentRow)
    End Class

End Namespace


