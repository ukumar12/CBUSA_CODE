Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class PIQDocumentRow
        Inherits PIQDocumentRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal PIQDocumentID As Integer)
            MyBase.New(DB, PIQDocumentID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal PIQDocumentID As Integer) As PIQDocumentRow
            Dim row As PIQDocumentRow

            row = New PIQDocumentRow(DB, PIQDocumentID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal PIQDocumentID As Integer)
            Dim row As PIQDocumentRow

            row = New PIQDocumentRow(DB, PIQDocumentID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from PIQDocument"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class PIQDocumentRowBase
        Private m_DB As Database
        Private m_PIQDocumentID As Integer = Nothing
        Private m_PIQID As Integer = Nothing
        Private m_Title As String = Nothing
        Private m_FileName As String = Nothing
        Private m_GUID As String = Nothing
        Private m_Uploaded As DateTime = Nothing
        Private m_IsApproved As Boolean = Nothing


        Public Property PIQDocumentID() As Integer
            Get
                Return m_PIQDocumentID
            End Get
            Set(ByVal Value As Integer)
                m_PIQDocumentID = value
            End Set
        End Property

        Public Property PIQID() As Integer
            Get
                Return m_PIQID
            End Get
            Set(ByVal Value As Integer)
                m_PIQID = value
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

        Public Property IsApproved() As Boolean
            Get
                Return m_IsApproved
            End Get
            Set(ByVal Value As Boolean)
                m_IsApproved = value
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

        Public Sub New(ByVal DB As Database, ByVal PIQDocumentID As Integer)
            m_DB = DB
            m_PIQDocumentID = PIQDocumentID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM PIQDocument WHERE PIQDocumentID = " & DB.Number(PIQDocumentID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_PIQDocumentID = Convert.ToInt32(r.Item("PIQDocumentID"))
            m_PIQID = Convert.ToInt32(r.Item("PIQID"))
            m_Title = Convert.ToString(r.Item("Title"))
            m_FileName = Convert.ToString(r.Item("FileName"))
            m_GUID = Convert.ToString(r.Item("GUID"))
            m_Uploaded = Convert.ToDateTime(r.Item("Uploaded"))
            m_IsApproved = Convert.ToBoolean(r.Item("IsApproved"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO PIQDocument (" _
             & " PIQID" _
             & ",Title" _
             & ",FileName" _
             & ",GUID" _
             & ",Uploaded" _
             & ",IsApproved" _
             & ") VALUES (" _
             & m_DB.NullNumber(PIQID) _
             & "," & m_DB.Quote(Title) _
             & "," & m_DB.Quote(FileName) _
             & "," & m_DB.Quote(GUID) _
             & "," & m_DB.NullQuote(Uploaded) _
             & "," & CInt(IsApproved) _
             & ")"

            PIQDocumentID = m_DB.InsertSQL(SQL)

            Return PIQDocumentID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE PIQDocument SET " _
             & " PIQID = " & m_DB.NullNumber(PIQID) _
             & ",Title = " & m_DB.Quote(Title) _
             & ",FileName = " & m_DB.Quote(FileName) _
             & ",GUID = " & m_DB.Quote(GUID) _
             & ",Uploaded = " & m_DB.NullQuote(Uploaded) _
             & ",IsApproved = " & CInt(IsApproved) _
             & " WHERE PIQDocumentID = " & m_DB.quote(PIQDocumentID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM PIQDocument WHERE PIQDocumentID = " & m_DB.Number(PIQDocumentID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class PIQDocumentCollection
        Inherits GenericCollection(Of PIQDocumentRow)
    End Class

End Namespace

