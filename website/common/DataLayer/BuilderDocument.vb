Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class BuilderDocumentRow
        Inherits BuilderDocumentRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BuilderDocumentID As Integer)
            MyBase.New(DB, BuilderDocumentID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal BuilderDocumentID As Integer) As BuilderDocumentRow
            Dim row As BuilderDocumentRow

            row = New BuilderDocumentRow(DB, BuilderDocumentID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal BuilderDocumentID As Integer)
            Dim row As BuilderDocumentRow

            row = New BuilderDocumentRow(DB, BuilderDocumentID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from BuilderDocument"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class BuilderDocumentRowBase
        Private m_DB As Database
        Private m_BuilderDocumentID As Integer = Nothing
        Private m_BuilderID As Integer = Nothing
        Private m_Title As String = Nothing
        Private m_FileName As String = Nothing
        Private m_GUID As String = Nothing
        Private m_Uploaded As DateTime = Nothing
        Private m_IsApproved As Boolean = Nothing


        Public Property BuilderDocumentID() As Integer
            Get
                Return m_BuilderDocumentID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderDocumentID = value
            End Set
        End Property

        Public Property BuilderID() As Integer
            Get
                Return m_BuilderID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderID = value
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

        Public Sub New(ByVal DB As Database, ByVal BuilderDocumentID As Integer)
            m_DB = DB
            m_BuilderDocumentID = BuilderDocumentID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM BuilderDocument WHERE BuilderDocumentID = " & DB.Number(BuilderDocumentID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_BuilderDocumentID = Convert.ToInt32(r.Item("BuilderDocumentID"))
            m_BuilderID = Convert.ToInt32(r.Item("BuilderID"))
            m_Title = Convert.ToString(r.Item("Title"))
            m_FileName = Convert.ToString(r.Item("FileName"))
            m_GUID = Convert.ToString(r.Item("GUID"))
            m_Uploaded = Convert.ToDateTime(r.Item("Uploaded"))
            m_IsApproved = Convert.ToBoolean(r.Item("IsApproved"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO BuilderDocument (" _
             & " BuilderID" _
             & ",Title" _
             & ",FileName" _
             & ",GUID" _
             & ",Uploaded" _
             & ",IsApproved" _
             & ") VALUES (" _
             & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.Quote(Title) _
             & "," & m_DB.Quote(FileName) _
             & "," & m_DB.Quote(GUID) _
             & "," & m_DB.NullQuote(Uploaded) _
             & "," & CInt(IsApproved) _
             & ")"

            BuilderDocumentID = m_DB.InsertSQL(SQL)

            Return BuilderDocumentID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE BuilderDocument SET " _
             & " BuilderID = " & m_DB.NullNumber(BuilderID) _
             & ",Title = " & m_DB.Quote(Title) _
             & ",FileName = " & m_DB.Quote(FileName) _
             & ",GUID = " & m_DB.Quote(GUID) _
             & ",Uploaded = " & m_DB.NullQuote(Uploaded) _
             & ",IsApproved = " & CInt(IsApproved) _
             & " WHERE BuilderDocumentID = " & m_DB.quote(BuilderDocumentID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM BuilderDocument WHERE BuilderDocumentID = " & m_DB.Number(BuilderDocumentID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class BuilderDocumentCollection
        Inherits GenericCollection(Of BuilderDocumentRow)
    End Class

End Namespace

