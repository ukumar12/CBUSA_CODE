Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class MailingListRow
        Inherits MailingListRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            MyBase.New(database)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal ListId As Integer)
            MyBase.New(database, ListId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal _Database As Database, ByVal ListId As Integer) As MailingListRow
            Dim row As MailingListRow

            row = New MailingListRow(_Database, ListId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal _Database As Database, ByVal ListId As Integer)
            Dim row As MailingListRow

            row = New MailingListRow(_Database, ListId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetLists(ByVal db As Database) As DataTable
            Dim dt As DataTable = db.GetDataTable("select * from mailinglist where Status = 'Active'")
            Return dt
        End Function

        Public Shared Function GetPermanentLists(ByVal db As Database) As DataTable
            Dim dt As DataTable = db.GetDataTable("select * from mailinglist where IsPermanent = 1 and Status = 'Active'")
            Return dt
        End Function

        Public Shared Function GetDynamicLists(ByVal db As Database) As DataTable
            Dim dt As DataTable = db.GetDataTable("select * from mailinglist where IsPermanent = 0 and Status = 'Active'")
            Return dt
        End Function

    End Class

    Public MustInherit Class MailingListRowBase
        Private m_DB As Database
        Private m_ListId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_Status As String = Nothing
        Private m_Filename As String = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_CreateAdminId As Integer = Nothing
        Private m_ModifyDate As DateTime = Nothing
        Private m_ModifyAdminId As Integer = Nothing
        Private m_IsPermanent As Boolean = Nothing


        Public Property ListId() As Integer
            Get
                Return m_ListId
            End Get
            Set(ByVal Value As Integer)
                m_ListId = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal Value As String)
                m_Name = value
            End Set
        End Property

        Public Property Status() As String
            Get
                Return m_Status
            End Get
            Set(ByVal Value As String)
                m_Status = value
            End Set
        End Property

        Public Property Filename() As String
            Get
                Return m_Filename
            End Get
            Set(ByVal Value As String)
                m_Filename = value
            End Set
        End Property

        Public Readonly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
        End Property

        Public Property CreateAdminId() As Integer
            Get
                Return m_CreateAdminId
            End Get
            Set(ByVal Value As Integer)
                m_CreateAdminId = Value
            End Set
        End Property

        Public Readonly Property ModifyDate() As DateTime
            Get
                Return m_ModifyDate
            End Get
        End Property

        Public Property ModifyAdminId() As Integer
            Get
                Return m_ModifyAdminId
            End Get
            Set(ByVal Value As Integer)
                m_ModifyAdminId = value
            End Set
        End Property

        Public Property IsPermanent() As Boolean
            Get
                Return m_IsPermanent
            End Get
            Set(ByVal Value As Boolean)
                m_IsPermanent = value
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

        Public Sub New(ByVal database As Database, ByVal ListId As Integer)
            m_DB = database
            m_ListId = ListId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM MailingList WHERE ListId = " & DB.Quote(ListId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_ListId = Convert.ToInt32(r.Item("ListId"))
            m_Name = Convert.ToString(r.Item("Name"))
            m_Status = Convert.ToString(r.Item("Status"))
            If IsDBNull(r.Item("Filename")) Then
                m_Filename = Nothing
            Else
                m_Filename = Convert.ToString(r.Item("Filename"))
            End If
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            m_CreateAdminId = Convert.ToInt32(r.Item("CreateAdminId"))
            m_ModifyDate = Convert.ToDateTime(r.Item("ModifyDate"))
            m_ModifyAdminId = Convert.ToInt32(r.Item("ModifyAdminId"))
            m_IsPermanent = Convert.ToBoolean(r.Item("IsPermanent"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            SQL = " INSERT INTO MailingList (" _
             & " Name" _
             & ",Status" _
             & ",Filename" _
             & ",CreateDate" _
             & ",CreateAdminId" _
             & ",ModifyDate" _
             & ",ModifyAdminId" _
             & ",IsPermanent" _
             & ") VALUES (" _
             & m_DB.Quote(Name) _
             & "," & m_DB.Quote(Status) _
             & "," & m_DB.Quote(Filename) _
             & "," & m_DB.Quote(Now) _
             & "," & m_DB.NullQuote(CreateAdminId) _
             & "," & m_DB.Quote(Now) _
             & "," & m_DB.NullQuote(ModifyAdminId) _
             & "," & CInt(IsPermanent) _
             & ")"

            ListId = m_DB.InsertSQL(SQL)

            Return ListId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE MailingList SET " _
             & " Name = " & m_DB.Quote(Name) _
             & ",Status = " & m_DB.Quote(Status) _
             & ",Filename = " & m_DB.Quote(Filename) _
             & ",ModifyDate = " & m_DB.Quote(Now) _
             & ",ModifyAdminId = " & m_DB.NullQuote(ModifyAdminId) _
             & ",IsPermanent = " & CInt(IsPermanent) _
             & " WHERE ListId = " & m_DB.Quote(ListId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM MailingListMember WHERE ListId = " & m_DB.Quote(ListId)
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM MailingList WHERE ListId = " & m_DB.Quote(ListId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class MailingListCollection
        Inherits GenericCollection(Of MailingListRow)
    End Class

End Namespace

