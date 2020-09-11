Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

	Public Class ContactUsRow
		Inherits ContactUsRowBase

		Public Sub New()
			MyBase.New()
		End Sub	'New

		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub	'New

		Public Sub New(ByVal DB As Database, ByVal ContactUsId As Integer)
			MyBase.New(DB, ContactUsId)
		End Sub	'New

		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB As Database, ByVal ContactUsId As Integer) As ContactUsRow
			Dim row As ContactUsRow

			row = New ContactUsRow(DB, ContactUsId)
			row.Load()

			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ContactUsId As Integer)
			Dim row As ContactUsRow

			row = New ContactUsRow(DB, ContactUsId)
			row.Remove()
		End Sub

		'Custom Methods

	End Class

	Public MustInherit Class ContactUsRowBase
		Private m_DB As Database
		Private m_ContactUsId As Integer = Nothing
		Private m_FullName As String = Nothing
		Private m_Email As String = Nothing
		Private m_OrderNumber As String = Nothing
		Private m_Phone As String = Nothing
		Private m_QuestionId As Integer = Nothing
		Private m_YourMessage As String = Nothing
		Private m_CreateDate As DateTime = Nothing
		Private m_HowHeardId As Integer = Nothing
		Private m_HowHeardName As String = Nothing

		Public Property HowHeardId() As Integer
			Get
				Return m_HowHeardId
			End Get
			Set(ByVal value As Integer)
				m_HowHeardId = value
			End Set
		End Property

		Public Property HowHeardName() As String
			Get
				Return m_HowHeardName
			End Get
			Set(ByVal value As String)
				m_HowHeardName = value
			End Set
		End Property

		Public Property ContactUsId() As Integer
			Get
				Return m_ContactUsId
			End Get
			Set(ByVal Value As Integer)
				m_ContactUsId = Value
			End Set
		End Property

		Public Property FullName() As String
			Get
				Return m_FullName
			End Get
			Set(ByVal Value As String)
				m_FullName = Value
			End Set
		End Property

		Public Property Email() As String
			Get
				Return m_Email
			End Get
			Set(ByVal Value As String)
				m_Email = Value
			End Set
		End Property

		Public Property OrderNumber() As String
			Get
				Return m_OrderNumber
			End Get
			Set(ByVal Value As String)
				m_OrderNumber = Value
			End Set
		End Property

		Public Property Phone() As String
			Get
				Return m_Phone
			End Get
			Set(ByVal Value As String)
				m_Phone = Value
			End Set
		End Property

		Public Property QuestionId() As Integer
			Get
				Return m_QuestionId
			End Get
			Set(ByVal Value As Integer)
				m_QuestionId = Value
			End Set
		End Property

		Public Property YourMessage() As String
			Get
				Return m_YourMessage
			End Get
			Set(ByVal Value As String)
				m_YourMessage = Value
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
			Set(ByVal Value As Database)
				m_DB = Value
			End Set
		End Property

		Public Sub New()
		End Sub	'New

		Public Sub New(ByVal DB As Database)
			m_DB = DB
		End Sub	'New

		Public Sub New(ByVal DB As Database, ByVal ContactUsId As Integer)
			m_DB = DB
			m_ContactUsId = ContactUsId
		End Sub	'New

		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String

			SQL = "SELECT * FROM ContactUs WHERE ContactUsId = " & DB.Number(ContactUsId)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub


		Protected Overridable Sub Load(ByVal r As SqlDataReader)
			m_ContactUsId = Convert.ToInt32(r.Item("ContactUsId"))
			m_FullName = Convert.ToString(r.Item("FullName"))
			m_Email = Convert.ToString(r.Item("Email"))
			If IsDBNull(r.Item("OrderNumber")) Then
				m_OrderNumber = Nothing
			Else
				m_OrderNumber = Convert.ToString(r.Item("OrderNumber"))
			End If
			If IsDBNull(r.Item("Phone")) Then
				m_Phone = Nothing
			Else
				m_Phone = Convert.ToString(r.Item("Phone"))
			End If
			m_QuestionId = Convert.ToInt32(r.Item("QuestionId"))
			m_YourMessage = Convert.ToString(r.Item("YourMessage"))
			m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
			If Not IsDBNull(r.Item("HowHeardId")) Then
				m_HowHeardId = Convert.ToInt32(r.Item("HowHeardId"))
			Else
				m_HowHeardId = Nothing
			End If
			m_HowHeardName = Convert.ToString(r.Item("HowHeardName"))
		End Sub	'Load

		Public Overridable Function Insert() As Integer
			Dim SQL As String


			SQL = " INSERT INTO ContactUs (" _
			 & " FullName" _
			 & ",Email" _
			 & ",OrderNumber" _
			 & ",Phone" _
			 & ",QuestionId" _
			 & ",YourMessage" _
			 & ",CreateDate" _
			 & ",HowHeardId" _
			 & ",HowHeardName" _
			 & ") VALUES (" _
			 & m_DB.Quote(FullName) _
			 & "," & m_DB.Quote(Email) _
			 & "," & m_DB.Quote(OrderNumber) _
			 & "," & m_DB.Quote(Phone) _
			 & "," & m_DB.NullNumber(QuestionId) _
			 & "," & m_DB.Quote(YourMessage) _
			 & "," & m_DB.NullQuote(Now) _
			 & "," & m_DB.NullNumber(HowHeardId) _
			 & "," & m_DB.Quote(HowHeardName) _
			 & ")"

			ContactUsId = m_DB.InsertSQL(SQL)

			Return ContactUsId
		End Function

		Public Overridable Sub Update()
			Dim SQL As String

			SQL = " UPDATE ContactUs SET " _
			 & " FullName = " & m_DB.Quote(FullName) _
			 & ",Email = " & m_DB.Quote(Email) _
			 & ",OrderNumber = " & m_DB.Quote(OrderNumber) _
			 & ",Phone = " & m_DB.Quote(Phone) _
			 & ",QuestionId = " & m_DB.NullNumber(QuestionId) _
			 & ",YourMessage = " & m_DB.Quote(YourMessage) _
			 & ",HowHeardId = " & m_DB.NullNumber(HowHeardId) _
			 & ",HowHeardName = " & m_DB.Quote(HowHeardName) _
			 & " WHERE ContactUsId = " & m_DB.Quote(ContactUsId)

			m_DB.ExecuteSQL(SQL)

		End Sub	'Update

		Public Sub Remove()
			Dim SQL As String

			SQL = "DELETE FROM ContactUs WHERE ContactUsId = " & m_DB.Number(ContactUsId)
			m_DB.ExecuteSQL(SQL)
		End Sub	'Remove
	End Class

	Public Class ContactUsCollection
		Inherits GenericCollection(Of ContactUsRow)
	End Class

End Namespace


