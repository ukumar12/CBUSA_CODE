'Public Class TakeOffServiceDocument

'End Class

Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public MustInherit Class TakeOffServiceDocumentRowBase
        Private m_DB As Database
        Private m_DocumentId As Integer = Nothing
        Private m_Title As String = Nothing
        Private m_FileName As String = Nothing
        Private m_GUID As String = Nothing
        Private m_Uploaded As DateTime = Nothing
        Private m_TakeOffServiceID As Integer = Nothing





        Public Property TakeOffServiceID() As Integer
            Get
                Return m_TakeOffServiceID
            End Get
            Set(ByVal Value As Integer)
                m_TakeOffServiceID = Value
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

        Public Sub New(ByVal DB As Database, ByVal TakeOffServiceDocumentId As Integer)
            m_DB = DB
            m_DocumentId = TakeOffServiceDocumentId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM  TakeOffServiceDocument WHERE DocumentId = " & DB.Number(DocumentId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                DocumentId = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_DocumentId = Core.GetInt(r.Item("DocumentId"))
            m_Title = Core.GetString(r.Item("Title"))
            m_FileName = Core.GetString(r.Item("FileName"))
            m_GUID = Core.GetString(r.Item("GUID"))
            m_Uploaded = Core.GetDate(r.Item("Uploaded"))
            m_TakeOffServiceID = Core.GetInt(r.Item("TakeOffServiceID"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO TakeOffServiceDocument (" _
             & "Title" _
             & ",FileName" _
             & ",GUID" _
             & ",Uploaded" _
             & ",TakeOffServiceID" _
             & ") VALUES (" _
             & m_DB.Quote(Title) _
             & "," & m_DB.Quote(FileName) _
             & "," & m_DB.Quote(GUID) _
             & "," & m_DB.NullQuote(Uploaded) _
             & "," & m_DB.Number(TakeOffServiceID) _
             & ")"

            DocumentId = m_DB.InsertSQL(SQL)

            Return DocumentId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE TakeOffServiceDocument WITH (ROWLOCK) SET " _
             & "Title = " & m_DB.Quote(Title) _
             & ",FileName = " & m_DB.Quote(FileName) _
             & ",GUID = " & m_DB.Quote(GUID) _
             & ",Uploaded = " & m_DB.NullQuote(Uploaded) _
                      & ",TakeOffServiceID = " & m_DB.Number(TakeOffServiceID) _
             & " WHERE  DocumentId = " & m_DB.Quote(DocumentId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class TakeOffServiceDocumentCollection
        Inherits GenericCollection(Of TakeOffServiceDocumentRow)
    End Class



    Public Class TakeOffServiceDocumentRow
        Inherits TakeOffServiceDocumentRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal TakeOffServiceDocumentId As Integer)
            MyBase.New(DB, TakeOffServiceDocumentId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal DocumentId As Integer) As TakeOffServiceDocumentRow
            Dim row As TakeOffServiceDocumentRow

            row = New TakeOffServiceDocumentRow(DB, DocumentId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal DocumentId As Integer)
            Dim SQL As String

            SQL = "DELETE FROM   TakeOffServiceDocument WHERE  DocumentId = " & DB.Number(DocumentId)
            DB.ExecuteSQL(SQL)
        End Sub

        Public Sub Remove()
            RemoveRow(DB, DocumentId)
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from  TakeOffServiceDocument"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class



End Namespace


