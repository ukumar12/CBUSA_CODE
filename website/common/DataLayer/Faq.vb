Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

	Public Class FaqRow
		Inherits FaqRowBase

		Public Sub New()
			MyBase.New()
		End Sub	'New

		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub	'New

		Public Sub New(ByVal DB As Database, ByVal FaqId As Integer)
			MyBase.New(DB, FaqId)
		End Sub	'New

		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB As Database, ByVal FaqId As Integer) As FaqRow
			Dim row As FaqRow

			row = New FaqRow(DB, FaqId)
			row.Load()

			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB As Database, ByVal FaqId As Integer)
			Dim row As FaqRow

			row = New FaqRow(DB, FaqId)
			row.Remove()
		End Sub

		'Custom Methods

	End Class

	Public MustInherit Class FaqRowBase
		Private m_DB As Database
		Private m_FaqId As Integer = Nothing
		Private m_FaqCategoryId As Integer = Nothing
		Private m_IsActive As Boolean = Nothing
		Private m_SortOrder As Integer = Nothing
		Private m_Question As String = Nothing
		Private m_Answer As String = Nothing
		Private m_Email As String = Nothing


		Public Property FaqId() As Integer
			Get
				Return m_FaqId
			End Get
			Set(ByVal Value As Integer)
				m_FaqId = value
			End Set
		End Property

		Public Property FaqCategoryId() As Integer
			Get
				Return m_FaqCategoryId
			End Get
			Set(ByVal Value As Integer)
				m_FaqCategoryId = value
			End Set
		End Property

		Public Property IsActive() As Boolean
			Get
				Return m_IsActive
			End Get
			Set(ByVal Value As Boolean)
				m_IsActive = value
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

		Public Property Question() As String
			Get
				Return m_Question
			End Get
			Set(ByVal Value As String)
				m_Question = value
			End Set
		End Property

		Public Property Answer() As String
			Get
				Return m_Answer
			End Get
			Set(ByVal Value As String)
				m_Answer = value
			End Set
		End Property

		Public Property Email() As String
			Get
				Return m_Email
			End Get
			Set(ByVal value As String)
				m_Email = value
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

		Public Sub New(ByVal DB As Database, ByVal FaqId As Integer)
			m_DB = DB
			m_FaqId = FaqId
		End Sub	'New

		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String

			SQL = "SELECT * FROM Faq WHERE FaqId = " & DB.Number(FaqId)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub


		Protected Overridable Sub Load(ByVal r As sqlDataReader)
			m_FaqId = Convert.ToInt32(r.Item("FaqId"))
			m_FaqCategoryId = Convert.ToInt32(r.Item("FaqCategoryId"))
			m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
			m_Question = Convert.ToString(r.Item("Question"))
			m_Answer = Convert.ToString(r.Item("Answer"))
			m_Email = Convert.ToString(r.Item("Email"))
		End Sub	'Load

		Public Overridable Function Insert() As Integer
			Dim SQL As String

			Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from Faq order by SortOrder desc")
			MaxSortOrder += 1

			SQL = " INSERT INTO Faq (" _
			 & " FaqCategoryId" _
			 & ",IsActive" _
			 & ",SortOrder" _
			 & ",Question" _
			 & ",Answer" _
			 & ",Email" _
			 & ") VALUES (" _
			 & m_DB.NullNumber(FaqCategoryId) _
			 & "," & CInt(IIf(Answer = Nothing, False, IsActive)) _
			 & "," & MaxSortOrder _
			 & "," & m_DB.Quote(Question) _
			 & "," & m_DB.Quote(Answer) _
			 & "," & m_DB.Quote(Email) _
			 & ")"

			FaqId = m_DB.InsertSQL(SQL)

			Return FaqId
		End Function

		Public Overridable Sub Update()
			Dim SQL As String

			SQL = " UPDATE Faq SET " _
			 & " FaqCategoryId = " & m_DB.NullNumber(FaqCategoryId) _
			 & ",IsActive = " & CInt(IIf(Answer = Nothing, False, IsActive)) _
			 & ",Question = " & m_DB.Quote(Question) _
			 & ",Answer = " & m_DB.Quote(Answer) _
			 & ",Email = " & m_DB.Quote(Email) _
			 & " WHERE FaqId = " & m_DB.Quote(FaqId)

			m_DB.ExecuteSQL(SQL)

		End Sub	'Update

		Public Sub Remove()
			Dim SQL As String

			SQL = "DELETE FROM Faq WHERE FaqId = " & m_DB.Quote(FaqId)
			m_DB.ExecuteSQL(SQL)
		End Sub	'Remove
	End Class

	Public Class FaqCollection
		Inherits GenericCollection(Of FaqRow)
	End Class

End Namespace

