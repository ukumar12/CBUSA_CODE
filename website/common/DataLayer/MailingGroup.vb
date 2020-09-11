Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class MailingGroupRow
        Inherits MailingGroupRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            MyBase.New(database)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal GroupId As Integer)
            MyBase.New(database, GroupId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal _Database As Database, ByVal GroupId As Integer) As MailingGroupRow
            Dim row As MailingGroupRow

            row = New MailingGroupRow(_Database, GroupId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal _Database As Database, ByVal GroupId As Integer)
            Dim row As MailingGroupRow

            Dim SQL As String = "delete from MailingGroupList where GroupId = " & GroupId
            _Database.ExecuteSQL(SQL)

            row = New MailingGroupRow(_Database, GroupId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared ReadOnly Property GetSelectedMailingLists(ByVal db As Database, ByVal GroupId As Integer) As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select ListId from MailingGroupList where GroupId = " & GroupId)
                Dim Conn As String = String.Empty
                Dim Result As String = String.Empty

                While dr.Read()
                    Result &= Conn & dr("ListId")
                    Conn = ","
                End While
                dr.Close()
                Return Result
            End Get
        End Property

        Public ReadOnly Property GetSelectedMailingLists() As String
            Get
                Return GetSelectedMailingLists(DB, GroupId)
            End Get
        End Property

        Public ReadOnly Property GetSelectedMailingListNames() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select ml.Name from MailingGroupList mgl, MailingList ml where mgl.ListId = ml.ListId and mgl.GroupId = " & GroupId)
                Dim Conn As String = String.Empty
                Dim Result As String = String.Empty

                While dr.Read()
                    Result &= Conn & dr("Name")
                    Conn = ", "
                End While
                dr.Close()
                Return Result
            End Get
        End Property

        Public Sub DeleteFromAllMailingLists()
            DB.ExecuteSQL("delete from MailingGroupList where GroupId = " & GroupId)
        End Sub

        Public Sub InsertToMailingLists(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                InsertToMailingList(Element)
            Next
        End Sub

        Public Sub InsertToMailingList(ByVal ListId As Integer)
            Dim SQL As String = "insert into MailingGroupList (GroupId, ListId) values (" & GroupId & "," & ListId & ")"
            DB.ExecuteSQL(SQL)
        End Sub

        Public Shared Function GetPermanentGroupList(ByVal DB As Database) As DataTable
            Return DB.GetDataTable("select * from MailingGroup where IsPermanent = 1 order by Name")
        End Function

    End Class

    Public MustInherit Class MailingGroupRowBase
        Private m_DB As Database
        Private m_GroupId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_Description As String = Nothing
        Private m_StartDate As DateTime = Nothing
        Private m_EndDate As DateTime = Nothing
        Private m_IsPermanent As Boolean = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_CreateAdminId As Integer = Nothing
        Private m_ModifyDate As DateTime = Nothing
        Private m_ModifyAdminId As Integer = Nothing


        Public Property GroupId() As Integer
            Get
                Return m_GroupId
            End Get
            Set(ByVal Value As Integer)
                m_GroupId = value
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

        Public Property Description() As String
            Get
                Return m_Description
            End Get
            Set(ByVal Value As String)
                m_Description = value
            End Set
        End Property

        Public Property StartDate() As DateTime
            Get
                Return m_StartDate
            End Get
            Set(ByVal Value As DateTime)
                m_StartDate = value
            End Set
        End Property

        Public Property EndDate() As DateTime
            Get
                Return m_EndDate
            End Get
            Set(ByVal Value As DateTime)
                m_EndDate = value
            End Set
        End Property

        Public Property IsPermanent() As Boolean
            Get
                Return m_IsPermanent
            End Get
            Set(ByVal Value As Boolean)
                m_IsPermanent = Value
            End Set
        End Property

        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
        End Property

        Public Property CreateAdminId() As Integer
            Get
                Return m_CreateAdminId
            End Get
            Set(ByVal Value As Integer)
                m_CreateAdminId = value
            End Set
        End Property

        Public ReadOnly Property ModifyDate() As DateTime
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

        Public Sub New(ByVal database As Database, ByVal GroupId As Integer)
            m_DB = database
            m_GroupId = GroupId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM MailingGroup WHERE GroupId = " & DB.Quote(GroupId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_GroupId = Convert.ToInt32(r.Item("GroupId"))
            m_Name = Convert.ToString(r.Item("Name"))
            If IsDBNull(r.Item("Description")) Then
                m_Description = Nothing
            Else
                m_Description = Convert.ToString(r.Item("Description"))
            End If
            If IsDBNull(r.Item("StartDate")) Then
                m_StartDate = Nothing
            Else
                m_StartDate = Convert.ToDateTime(r.Item("StartDate"))
            End If
            If IsDBNull(r.Item("EndDate")) Then
                m_EndDate = Nothing
            Else
                m_EndDate = Convert.ToDateTime(r.Item("EndDate"))
            End If
            m_IsPermanent = Convert.ToBoolean(r.Item("IsPermanent"))
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            m_CreateAdminId = Convert.ToInt32(r.Item("CreateAdminId"))
            m_ModifyDate = Convert.ToDateTime(r.Item("ModifyDate"))
            If IsDBNull(r.Item("ModifyAdminId")) Then
                m_ModifyAdminId = Nothing
            Else
                m_ModifyAdminId = Convert.ToInt32(r.Item("ModifyAdminId"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            SQL = " INSERT INTO MailingGroup (" _
             & " Name" _
             & ",Description" _
             & ",StartDate" _
             & ",EndDate" _
             & ",IsPermanent" _
             & ",CreateDate" _
             & ",CreateAdminId" _
             & ",ModifyDate" _
             & ",ModifyAdminId" _
             & ") VALUES (" _
             & m_DB.Quote(Name) _
             & "," & m_DB.Quote(Description) _
             & "," & m_DB.Quote(StartDate) _
             & "," & m_DB.Quote(EndDate) _
             & "," & CInt(IsPermanent) _
             & "," & m_DB.Quote(Now) _
             & "," & m_DB.NullQuote(CreateAdminId) _
             & "," & m_DB.Quote(Now) _
             & "," & m_DB.NullQuote(ModifyAdminId) _
             & ")"

            GroupId = m_DB.InsertSQL(SQL)

            Return GroupId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE MailingGroup SET " _
             & " Name = " & m_DB.Quote(Name) _
             & ",Description = " & m_DB.Quote(Description) _
             & ",StartDate = " & m_DB.Quote(StartDate) _
             & ",EndDate = " & m_DB.Quote(EndDate) _
             & ",IsPermanent = " & CInt(IsPermanent) _
             & ",ModifyDate = " & m_DB.Quote(Now) _
             & ",ModifyAdminId = " & m_DB.NullQuote(ModifyAdminId) _
             & " WHERE GroupId = " & m_DB.Quote(GroupId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM MailingGroup WHERE GroupId = " & m_DB.Quote(GroupId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class MailingGroupCollection
        Inherits GenericCollection(Of MailingGroupRow)
    End Class

End Namespace


