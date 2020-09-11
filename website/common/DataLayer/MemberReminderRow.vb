Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class MemberReminderRow
        Inherits MemberReminderRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ReminderId As Integer)
            MyBase.New(DB, ReminderId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal ReminderId As Integer) As MemberReminderRow
            Dim row As MemberReminderRow

            row = New MemberReminderRow(DB, ReminderId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ReminderId As Integer)
            Dim row As MemberReminderRow

            row = New MemberReminderRow(DB, ReminderId)
            row.Remove()
        End Sub

        'Custom Methods
    End Class

    Public MustInherit Class MemberReminderRowBase
        Private m_DB As Database
        Private m_ReminderId As Integer = Nothing
        Private m_MemberId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_IsRecurrent As Boolean = Nothing
        Private m_EventDate As DateTime = Nothing
        Private m_DaysBefore1 As String = Nothing
        Private m_DaysBefore2 As String = Nothing
        Private m_Email As String = Nothing
        Private m_Body As String = Nothing


        Public Property ReminderId() As Integer
            Get
                Return m_ReminderId
            End Get
            Set(ByVal Value As Integer)
                m_ReminderId = value
            End Set
        End Property

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

        Public Property IsRecurrent() As Boolean
            Get
                Return m_IsRecurrent
            End Get
            Set(ByVal Value As Boolean)
                m_IsRecurrent = value
            End Set
        End Property

        Public Property EventDate() As DateTime
            Get
                Return m_EventDate
            End Get
            Set(ByVal Value As DateTime)
                m_EventDate = value
            End Set
        End Property

        Public Property DaysBefore1() As String
            Get
                Return m_DaysBefore1
            End Get
            Set(ByVal Value As String)
                m_DaysBefore1 = Value
            End Set
        End Property

        Public Property DaysBefore2() As String
            Get
                Return m_DaysBefore2
            End Get
            Set(ByVal Value As String)
                m_DaysBefore2 = Value
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

        Public Property Body() As String
            Get
                Return m_Body
            End Get
            Set(ByVal Value As String)
                m_Body = value
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

        Public Sub New(ByVal DB As Database, ByVal ReminderId As Integer)
            m_DB = DB
            m_ReminderId = ReminderId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM MemberReminder WHERE ReminderId = " & DB.Number(ReminderId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_ReminderId = Convert.ToInt32(r.Item("ReminderId"))
            m_MemberId = Convert.ToInt32(r.Item("MemberId"))
            m_Name = Convert.ToString(r.Item("Name"))
            m_IsRecurrent = Convert.ToBoolean(r.Item("IsRecurrent"))
            m_EventDate = Convert.ToDateTime(r.Item("EventDate"))
            If IsDBNull(r.Item("DaysBefore1")) Then
                m_DaysBefore1 = Nothing
            Else
                m_DaysBefore1 = Convert.ToString(r.Item("DaysBefore1"))
            End If
            If IsDBNull(r.Item("DaysBefore2")) Then
                m_DaysBefore2 = Nothing
            Else
                m_DaysBefore2 = Convert.ToString(r.Item("DaysBefore2"))
            End If
            m_Email = Convert.ToString(r.Item("Email"))
            If IsDBNull(r.Item("Body")) Then
                m_Body = Nothing
            Else
                m_Body = Convert.ToString(r.Item("Body"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO MemberReminder (" _
             & " MemberId" _
             & ",Name" _
             & ",IsRecurrent" _
             & ",EventDate" _
             & ",DaysBefore1" _
             & ",DaysBefore2" _
             & ",Email" _
             & ",Body" _
             & ") VALUES (" _
             & m_DB.Number(MemberId) _
             & "," & m_DB.Quote(Name) _
             & "," & CInt(IsRecurrent) _
             & "," & m_DB.NullQuote(EventDate) _
             & "," & m_DB.Quote(DaysBefore1) _
             & "," & m_DB.Quote(DaysBefore2) _
             & "," & m_DB.Quote(Email) _
             & "," & m_DB.Quote(Body) _
             & ")"

            ReminderId = m_DB.InsertSQL(SQL)

            Return ReminderId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE MemberReminder SET " _
             & " MemberId = " & m_DB.Number(MemberId) _
             & ",Name = " & m_DB.Quote(Name) _
             & ",IsRecurrent = " & CInt(IsRecurrent) _
             & ",EventDate = " & m_DB.NullQuote(EventDate) _
             & ",DaysBefore1 = " & m_DB.Quote(DaysBefore1) _
             & ",DaysBefore2 = " & m_DB.Quote(DaysBefore2) _
             & ",Email = " & m_DB.Quote(Email) _
             & ",Body = " & m_DB.Quote(Body) _
             & " WHERE ReminderId = " & m_DB.Quote(ReminderId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM MemberReminder WHERE ReminderId = " & m_DB.Quote(ReminderId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class MemberReminderCollection
        Inherits GenericCollection(Of MemberReminderRow)
    End Class

End Namespace

