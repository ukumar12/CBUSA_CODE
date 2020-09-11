Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class MailingLyrisLookupCompletionStatusRow
        Inherits MailingLyrisLookupCompletionStatusRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            MyBase.New(database)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal CompletionStatusID As Integer)
            MyBase.New(database, CompletionStatusID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal _Database As Database, ByVal CompletionStatusID As Integer) As MailingLyrisLookupCompletionStatusRow
            Dim row As MailingLyrisLookupCompletionStatusRow

            row = New MailingLyrisLookupCompletionStatusRow(_Database, CompletionStatusID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal _Database As Database, ByVal CompletionStatusID As Integer)
            Dim row As MailingLyrisLookupCompletionStatusRow

            row = New MailingLyrisLookupCompletionStatusRow(_Database, CompletionStatusID)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetList(ByVal Db As Database) As DataTable
            Dim SQL As String = "select * from MailingLyrisLookupCompletionStatus order by Description"
            Dim dt As DataTable = Db.GetDataTable(SQL)

            Return dt
        End Function
    End Class

    Public MustInherit Class MailingLyrisLookupCompletionStatusRowBase
        Private m_DB As Database
        Private m_CompletionStatusID As Integer = Nothing
        Private m_Description As String = Nothing


        Public Property CompletionStatusID() As Integer
            Get
                Return m_CompletionStatusID
            End Get
            Set(ByVal Value As Integer)
                m_CompletionStatusID = value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return m_Description
            End Get
            Set(ByVal Value As String)
                m_Description = value
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

        Public Sub New(ByVal database As Database)
            m_DB = database
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal CompletionStatusID As Integer)
            m_DB = database
            m_CompletionStatusID = CompletionStatusID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM MailingLyrisLookupCompletionStatus WHERE CompletionStatusID = " & DB.Quote(CompletionStatusID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_CompletionStatusID = Convert.ToInt32(r.Item("CompletionStatusID"))
            m_Description = Convert.ToString(r.Item("Description"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            SQL = " INSERT INTO MailingLyrisLookupCompletionStatus (" _
             & " Description" _
             & ") VALUES (" _
             & m_DB.Quote(Description) _
             & ")"

            CompletionStatusID = m_DB.InsertSQL(SQL)

            Return CompletionStatusID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE MailingLyrisLookupCompletionStatus SET " _
             & " Description = " & m_DB.Quote(Description) _
             & " WHERE CompletionStatusID = " & m_DB.quote(CompletionStatusID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM MailingLyrisLookupCompletionStatus WHERE CompletionStatusID = " & m_DB.Quote(CompletionStatusID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class MailingLyrisLookupCompletionStatusCollection
        Inherits GenericCollection(Of MailingLyrisLookupCompletionStatusRow)
    End Class

End Namespace


