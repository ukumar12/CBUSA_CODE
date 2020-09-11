Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class MailingMemberRow
        Inherits MailingMemberRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            MyBase.New(database)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal MemberId As Integer)
            MyBase.New(database, MemberId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal _Database As Database, ByVal MemberId As Integer) As MailingMemberRow
            Dim row As MailingMemberRow

            row = New MailingMemberRow(_Database, MemberId)
            row.Load()

            Return row
        End Function

        'Custom Methods
        Public Shared Function GetRowByEmail(ByVal _Database As Database, ByVal Email As String) As MailingMemberRow
            Dim SQL As String = "SELECT * FROM MailingMember WHERE Email = " & _Database.Quote(Email)
            Dim r As SqlDataReader

            Dim row As MailingMemberRow = New MailingMemberRow(_Database)
            r = _Database.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

        Public Sub InsertToList(ByVal ListId As Integer)
            Dim SQL As String = "if not exists (select memberid from mailinglistmember where memberid=" & MemberId & " and ListId=" & ListId & ") begin insert into MailingListMember (MemberId, ListId) values (" & MemberId & "," & ListId & ") end"
            DB.ExecuteSQL(SQL)
        End Sub

        Public Sub DeleteFromList(ByVal ListId As Integer)
            Dim SQL As String = "delete from MailingListMember where MemberId = " & MemberId & " and ListId= " & ListId
            DB.ExecuteSQL(SQL)
        End Sub

        Public Sub DeleteFromAllLists()
            DB.ExecuteSQL("delete from mailinglistmember where memberid=" & MemberId)
        End Sub

        Public Sub DeleteFromAllPermanentLists()
            DB.ExecuteSQL("DELETE FROM MailingListMember WHERE MemberId = " & MemberId & " AND ListId IN (SELECT ListId FROM MailingList WHERE IsPermanent = 1)")
        End Sub

        Public Sub DeleteFromAllNotPermanentLists()
            DB.ExecuteSQL("DELETE FROM MailingListMember WHERE MemberId = " & MemberId & " AND ListId IN (SELECT ListId FROM MailingList WHERE IsPermanent = 0)")
        End Sub

        Public Sub InsertToLists(ByVal Lists As String)
            If Lists = String.Empty Then Exit Sub

            Dim aLists As String()
            aLists = Lists.Split(",")
            For Each s As String In aLists
                InsertToList(CInt(s))
            Next
        End Sub

        Public ReadOnly Property SubscribedLists() As String
            Get
                Dim sdr As SqlDataReader = Me.DB.GetReader("select listid from mailinglistmember where memberid=" & Me.MemberId)
                Dim d As String = String.Empty
                Dim ret As String = String.Empty

                While sdr.Read()
                    ret &= d & sdr("listid")
                    d = ","
                End While
                sdr.Close()
                Return ret
            End Get
        End Property

    End Class

    Public MustInherit Class MailingMemberRowBase
        Private m_DB As Database
        Private m_MemberId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_Email As String = Nothing
        Private m_MimeType As String = Nothing
        Private m_Status As String = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_ModifyDate As DateTime = Nothing
        Private m_Unsubscribe As DateTime = Nothing


        Public Property MemberId() As Integer
            Get
                Return m_MemberId
            End Get
            Set(ByVal Value As Integer)
                m_MemberId = value
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

        Public Property Email() As String
            Get
                Return m_Email
            End Get
            Set(ByVal Value As String)
                m_Email = value
            End Set
        End Property

        Public Property MimeType() As String
            Get
                Return m_MimeType
            End Get
            Set(ByVal Value As String)
                m_MimeType = value
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

        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
        End Property

        Public ReadOnly Property ModifyDate() As DateTime
            Get
                Return m_ModifyDate
            End Get
        End Property

        Public Property Unsubscribe() As DateTime
            Get
                Return m_Unsubscribe
            End Get
            Set(ByVal Value As DateTime)
                m_Unsubscribe = value
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

        Public Sub New(ByVal database As Database, ByVal MemberId As Integer)
            m_DB = database
            m_MemberId = MemberId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM MailingMember WHERE MemberId = " & DB.Quote(MemberId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_MemberId = Convert.ToInt32(r.Item("MemberId"))
            If IsDBNull(r.Item("Name")) Then
                m_Name = Nothing
            Else
                m_Name = Convert.ToString(r.Item("Name"))
            End If
            m_Email = Convert.ToString(r.Item("Email"))
            m_MimeType = Convert.ToString(r.Item("MimeType"))
            m_Status = Convert.ToString(r.Item("Status"))
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            m_ModifyDate = Convert.ToDateTime(r.Item("ModifyDate"))
            If IsDBNull(r.Item("Unsubscribe")) Then
                m_Unsubscribe = Nothing
            Else
                m_Unsubscribe = Convert.ToDateTime(r.Item("Unsubscribe"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            SQL = " INSERT INTO MailingMember (" _
             & " Name" _
             & ",Email" _
             & ",MimeType" _
             & ",Status" _
             & ",CreateDate" _
             & ",ModifyDate" _
             & ",Unsubscribe" _
             & ") VALUES (" _
             & m_DB.Quote(Name) _
             & "," & m_DB.Quote(Email) _
             & "," & m_DB.Quote(MimeType) _
             & "," & m_DB.Quote(Status) _
             & "," & m_DB.Quote(Now) _
             & "," & m_DB.Quote(Now) _
             & "," & m_DB.Quote(Unsubscribe) _
             & ")"

            MemberId = m_DB.InsertSQL(SQL)

            Return MemberId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE MailingMember SET " _
             & " Name = " & m_DB.Quote(Name) _
             & ",Email = " & m_DB.Quote(Email) _
             & ",MimeType = " & m_DB.Quote(MimeType) _
             & ",Status = " & m_DB.Quote(Status) _
             & ",ModifyDate = " & m_DB.Quote(Now) _
             & ",Unsubscribe = " & m_DB.Quote(Unsubscribe) _
             & " WHERE MemberId = " & m_DB.Quote(MemberId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM MailingMember WHERE MemberId = " & m_DB.Quote(MemberId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class MailingMemberCollection
        Inherits GenericCollection(Of MailingMemberRow)
    End Class

End Namespace


