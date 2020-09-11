Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class VendorDocumentRow
        Inherits VendorDocumentRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorDocumentID As Integer)
            MyBase.New(DB, VendorDocumentID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorDocumentID As Integer) As VendorDocumentRow
            Dim row As VendorDocumentRow

            row = New VendorDocumentRow(DB, VendorDocumentID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorDocumentID As Integer)
            Dim row As VendorDocumentRow

            row = New VendorDocumentRow(DB, VendorDocumentID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorDocument"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class VendorDocumentRowBase
        Private m_DB As Database
        Private m_VendorDocumentID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_Title As String = Nothing
        Private m_FileName As String = Nothing
        Private m_GUID As String = Nothing
        Private m_Uploaded As DateTime = Nothing
        Private m_IsApproved As Boolean = Nothing


        Public Property VendorDocumentID() As Integer
            Get
                Return m_VendorDocumentID
            End Get
            Set(ByVal Value As Integer)
                m_VendorDocumentID = value
            End Set
        End Property

        Public Property VendorID() As Integer
            Get
                Return m_VendorID
            End Get
            Set(ByVal Value As Integer)
                m_VendorID = value
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

        Public Sub New(ByVal DB As Database, ByVal VendorDocumentID As Integer)
            m_DB = DB
            m_VendorDocumentID = VendorDocumentID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM VendorDocument WHERE VendorDocumentID = " & DB.Number(VendorDocumentID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_VendorDocumentID = Convert.ToInt32(r.Item("VendorDocumentID"))
            m_VendorID = Convert.ToInt32(r.Item("VendorID"))
            m_Title = Convert.ToString(r.Item("Title"))
            m_FileName = Convert.ToString(r.Item("FileName"))
            m_GUID = Convert.ToString(r.Item("GUID"))
            m_Uploaded = Convert.ToDateTime(r.Item("Uploaded"))
            m_IsApproved = Convert.ToBoolean(r.Item("IsApproved"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO VendorDocument (" _
             & " VendorID" _
             & ",Title" _
             & ",FileName" _
             & ",GUID" _
             & ",Uploaded" _
             & ",IsApproved" _
             & ") VALUES (" _
             & m_DB.NullNumber(VendorID) _
             & "," & m_DB.Quote(Title) _
             & "," & m_DB.Quote(FileName) _
             & "," & m_DB.Quote(GUID) _
             & "," & m_DB.NullQuote(Uploaded) _
             & "," & CInt(IsApproved) _
             & ")"

            VendorDocumentID = m_DB.InsertSQL(SQL)

            Return VendorDocumentID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE VendorDocument SET " _
             & " VendorID = " & m_DB.NullNumber(VendorID) _
             & ",Title = " & m_DB.Quote(Title) _
             & ",FileName = " & m_DB.Quote(FileName) _
             & ",GUID = " & m_DB.Quote(GUID) _
             & ",Uploaded = " & m_DB.NullQuote(Uploaded) _
             & ",IsApproved = " & CInt(IsApproved) _
             & " WHERE VendorDocumentID = " & m_DB.quote(VendorDocumentID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM VendorDocument WHERE VendorDocumentID = " & m_DB.Number(VendorDocumentID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class VendorDocumentCollection
        Inherits GenericCollection(Of VendorDocumentRow)
    End Class

End Namespace

