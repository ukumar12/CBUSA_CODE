Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class AdminDocumentPIQRecipientRow
        Inherits AdminDocumentPIQRecipientRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal AdminDocumentID As Integer)
            MyBase.New(DB, AdminDocumentID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal AdminDocumentID As Integer) As AdminDocumentPIQRecipientRow
            Dim row As AdminDocumentPIQRecipientRow

            row = New AdminDocumentPIQRecipientRow(DB, AdminDocumentID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal AdminDocumentID As Integer)
            Dim row As AdminDocumentPIQRecipientRow

            row = New AdminDocumentPIQRecipientRow(DB, AdminDocumentID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from AdminDocumentPIQRecipient"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class AdminDocumentPIQRecipientRowBase
        Private m_DB As Database
        Private m_AdminDocumentID As Integer = Nothing
        Private m_PIQID As Integer = Nothing


        Public Property AdminDocumentID() As Integer
            Get
                Return m_AdminDocumentID
            End Get
            Set(ByVal Value As Integer)
                m_AdminDocumentID = value
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

        Public Sub New(ByVal DB As Database, ByVal AdminDocumentID As Integer)
            m_DB = DB
            m_AdminDocumentID = AdminDocumentID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM AdminDocumentPIQRecipient WHERE AdminDocumentID = " & DB.Number(AdminDocumentID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_AdminDocumentID = Convert.ToInt32(r.Item("AdminDocumentID"))
            m_PIQID = Convert.ToInt32(r.Item("PIQID"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO AdminDocumentPIQRecipient (" _
             & " AdminDocumentID," _
             & " PIQID" _
             & ") VALUES (" _
             & m_DB.NullNumber(AdminDocumentID) & ", " _
             & m_DB.NullNumber(PIQID) _
             & ")"

            AdminDocumentID = m_DB.InsertSQL(SQL)

            Return AdminDocumentID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE AdminDocumentPIQRecipient SET " _
             & " AdminDocumentID = " & m_DB.NullNumber(AdminDocumentID) _
             & ", PIQID = " & m_DB.NullNumber(PIQID) _
             & " WHERE AdminDocumentID = " & m_DB.Quote(AdminDocumentID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM AdminDocumentPIQRecipient WHERE AdminDocumentID = " & m_DB.Number(AdminDocumentID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class AdminDocumentPIQRecipientCollection
        Inherits GenericCollection(Of AdminDocumentPIQRecipientRow)
    End Class

End Namespace

