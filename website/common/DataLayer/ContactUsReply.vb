Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

	Public Class ContactUsReplyRow
		Inherits ContactUsReplyRowBase

		Public Sub New()
			MyBase.New()
		End Sub	'New

		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub	'New

		Public Sub New(ByVal DB As Database, ByVal ReplyId As Integer)
			MyBase.New(DB, ReplyId)
		End Sub	'New

		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB As Database, ByVal ReplyId As Integer) As ContactUsReplyRow
			Dim row As ContactUsReplyRow

			row = New ContactUsReplyRow(DB, ReplyId)
			row.Load()

			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ReplyId As Integer)
			Dim row As ContactUsReplyRow

			row = New ContactUsReplyRow(DB, ReplyId)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
			Dim SQL As String = "select * from ContactUsReply"
			If Not SortBy = String.Empty Then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End If
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods
		Public Shared Function GetContactUsReplies(ByVal DB As Database, ByVal ContactUsId As Integer) As DataTable
			Dim SQL As String = "select *, coalesce((select top 1 firstname + ' ' + lastname from admin where adminid = contactusreply.adminid),'') as AdminName from contactusreply where contactusid = " & ContactUsId & " order by createdate desc"
			Return DB.GetDataTable(SQL)
		End Function

	End Class

	Public MustInherit Class ContactUsReplyRowBase
		Private m_DB As Database
		Private m_ReplyId As Integer = Nothing
		Private m_ContactUsId As Integer = Nothing
		Private m_FullName As String = Nothing
		Private m_Email As String = Nothing
		Private m_Subject As String = Nothing
		Private m_Message As String = Nothing
		Private m_CreateDate As DateTime = Nothing
		Private m_AdminId As Integer = Nothing

		Public Property ReplyId() As Integer
			Get
				Return m_ReplyId
			End Get
			Set(ByVal Value As Integer)
				m_ReplyId = value
			End Set
		End Property

		Public Property ContactUsId() As Integer
			Get
				Return m_ContactUsId
			End Get
			Set(ByVal Value As Integer)
				m_ContactUsId = value
			End Set
		End Property

		Public Property FullName() As String
			Get
				Return m_FullName
			End Get
			Set(ByVal Value As String)
				m_FullName = value
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

		Public Property Subject() As String
			Get
				Return m_Subject
			End Get
			Set(ByVal Value As String)
				m_Subject = value
			End Set
		End Property

		Public Property Message() As String
			Get
				Return m_Message
			End Get
			Set(ByVal Value As String)
				m_Message = value
			End Set
		End Property

		Public Property AdminId() As Integer
			Get
				Return m_AdminId
			End Get
			Set(ByVal value As Integer)
				m_AdminId = value
			End Set
		End Property

		Public ReadOnly Property CreateDate() As DateTime
			Get
				Return m_CreateDate
			End Get
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
		End Sub	'New

		Public Sub New(ByVal DB As Database)
			m_DB = DB
		End Sub	'New

		Public Sub New(ByVal DB As Database, ByVal ReplyId As Integer)
			m_DB = DB
			m_ReplyId = ReplyId
		End Sub	'New

		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String

			SQL = "SELECT * FROM ContactUsReply WHERE ReplyId = " & DB.Number(ReplyId)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub


		Protected Overridable Sub Load(ByVal r As sqlDataReader)
			m_ReplyId = Convert.ToInt32(r.Item("ReplyId"))
			m_ContactUsId = Convert.ToInt32(r.Item("ContactUsId"))
			m_FullName = Convert.ToString(r.Item("FullName"))
			m_Email = Convert.ToString(r.Item("Email"))
			m_Subject = Convert.ToString(r.Item("Subject"))
			m_Message = Convert.ToString(r.Item("Message"))
			m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
			m_AdminId = Convert.ToInt32(r.Item("AdminId"))
		End Sub	'Load

		Public Overridable Function Insert() As Integer
			Dim SQL As String


			SQL = " INSERT INTO ContactUsReply (" _
			 & " ContactUsId" _
			 & ",FullName" _
			 & ",Email" _
			 & ",Subject" _
			 & ",Message" _
			 & ",CreateDate" _
			 & ",AdminId" _
			 & ") VALUES (" _
			 & m_DB.NullNumber(ContactUsId) _
			 & "," & m_DB.Quote(FullName) _
			 & "," & m_DB.Quote(Email) _
			 & "," & m_DB.Quote(Subject) _
			 & "," & m_DB.Quote(Message) _
			 & "," & m_DB.NullQuote(Now) _
			 & "," & m_DB.NullNumber(AdminId) _
			 & ")"

			ReplyId = m_DB.InsertSQL(SQL)

			Return ReplyId
		End Function

		Public Overridable Sub Update()
			Dim SQL As String

			SQL = " UPDATE ContactUsReply SET " _
			 & " ContactUsId = " & m_DB.NullNumber(ContactUsId) _
			 & ",FullName = " & m_DB.Quote(FullName) _
			 & ",Email = " & m_DB.Quote(Email) _
			 & ",Subject = " & m_DB.Quote(Subject) _
			 & ",Message = " & m_DB.Quote(Message) _
			 & ",AdminId = " & m_DB.NullNumber(AdminId) _
			 & " WHERE ReplyId = " & m_DB.Quote(ReplyId)

			m_DB.ExecuteSQL(SQL)

		End Sub	'Update

		Public Sub Remove()
			Dim SQL As String

			SQL = "DELETE FROM ContactUsReply WHERE ReplyId = " & m_DB.Number(ReplyId)
			m_DB.ExecuteSQL(SQL)
		End Sub	'Remove
	End Class

	Public Class ContactUsReplyCollection
		Inherits GenericCollection(Of ContactUsReplyRow)
	End Class

End Namespace


