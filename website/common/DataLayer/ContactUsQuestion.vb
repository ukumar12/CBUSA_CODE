Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

	Public Class ContactUsQuestionRow
		Inherits ContactUsQuestionRowBase

		Public Sub New()
			MyBase.New()
		End Sub	'New

		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub	'New

		Public Sub New(ByVal DB As Database, ByVal QuestionId As Integer)
			MyBase.New(DB, QuestionId)
		End Sub	'New

		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB As Database, ByVal QuestionId As Integer) As ContactUsQuestionRow
			Dim row As ContactUsQuestionRow

			row = New ContactUsQuestionRow(DB, QuestionId)
			row.Load()

			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB As Database, ByVal QuestionId As Integer)
			Dim row As ContactUsQuestionRow

			row = New ContactUsQuestionRow(DB, QuestionId)
			row.Remove()
		End Sub

		'Custom Methods
		Public Shared Function GetAllContactUsQuestions(ByVal DB As Database) As DataTable
			Dim SQL As String = "select * from contactusquestion order by sortorder"
			Return DB.GetDataTable(SQL)
		End Function
	End Class

	Public MustInherit Class ContactUsQuestionRowBase
		Private m_DB As Database
		Private m_QuestionId As Integer = Nothing
		Private m_Question As String = Nothing
		Private m_EmailAddress As String = Nothing
		Private m_SortOrder As Integer = Nothing


		Public Property QuestionId() As Integer
			Get
				Return m_QuestionId
			End Get
			Set(ByVal Value As Integer)
				m_QuestionId = value
			End Set
		End Property

		Public Property Question() As String
			Get
				Return m_Question
			End Get
			Set(ByVal Value As String)
				m_Question = value
			End Set
		End Property

		Public Property EmailAddress() As String
			Get
				Return m_EmailAddress
			End Get
			Set(ByVal Value As String)
				m_EmailAddress = value
			End Set
		End Property

		Public Property SortOrder() As Integer
			Get
				Return m_SortOrder
			End Get
			Set(ByVal Value As Integer)
				m_SortOrder = value
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
		End Sub	'New

		Public Sub New(ByVal DB As Database)
			m_DB = DB
		End Sub	'New

		Public Sub New(ByVal DB As Database, ByVal QuestionId As Integer)
			m_DB = DB
			m_QuestionId = QuestionId
		End Sub	'New

		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String

			SQL = "SELECT * FROM ContactUsQuestion WHERE QuestionId = " & DB.Number(QuestionId)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub


		Protected Overridable Sub Load(ByVal r As sqlDataReader)
			m_QuestionId = Convert.ToInt32(r.Item("QuestionId"))
			m_Question = Convert.ToString(r.Item("Question"))
			If IsDBNull(r.Item("EmailAddress")) Then
				m_EmailAddress = Nothing
			Else
				m_EmailAddress = Convert.ToString(r.Item("EmailAddress"))
			End If
		End Sub	'Load

		Public Overridable Function Insert() As Integer
			Dim SQL As String

			Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from ContactUsQuestion order by SortOrder desc")
			MaxSortOrder += 1

			SQL = " INSERT INTO ContactUsQuestion (" _
			 & " Question" _
			 & ",EmailAddress" _
			 & ",SortOrder" _
			 & ") VALUES (" _
			 & m_DB.Quote(Question) _
			 & "," & m_DB.Quote(EmailAddress) _
			 & "," & MaxSortOrder _
			 & ")"

			QuestionId = m_DB.InsertSQL(SQL)

			Return QuestionId
		End Function

		Public Overridable Sub Update()
			Dim SQL As String

			SQL = " UPDATE ContactUsQuestion SET " _
			 & " Question = " & m_DB.Quote(Question) _
			 & ",EmailAddress = " & m_DB.Quote(EmailAddress) _
			 & " WHERE QuestionId = " & m_DB.quote(QuestionId)

			m_DB.ExecuteSQL(SQL)

		End Sub	'Update

		Public Sub Remove()
			Dim SQL As String

			SQL = "DELETE FROM ContactUsQuestion WHERE QuestionId = " & m_DB.Number(QuestionId)
			m_DB.ExecuteSQL(SQL)
		End Sub	'Remove
	End Class

	Public Class ContactUsQuestionCollection
		Inherits GenericCollection(Of ContactUsQuestionRow)
	End Class

End Namespace


