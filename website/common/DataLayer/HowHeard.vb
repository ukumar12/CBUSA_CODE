Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

	Public Class HowHeardRow
		Inherits HowHeardRowBase

		Public Sub New()
			MyBase.New()
		End Sub	'New

		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub	'New

		Public Sub New(ByVal DB As Database, ByVal HowHeardId As Integer)
			MyBase.New(DB, HowHeardId)
		End Sub	'New

		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB As Database, ByVal HowHeardId As Integer) As HowHeardRow
			Dim row As HowHeardRow

			row = New HowHeardRow(DB, HowHeardId)
			row.Load()

			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB As Database, ByVal HowHeardId As Integer)
			Dim row As HowHeardRow

			row = New HowHeardRow(DB, HowHeardId)
			row.Remove()
		End Sub

		'Custom Methods
		Public Shared Function GetAllHowHeard(ByVal DB As Database) As DataTable
			Return DB.GetDataTable("select * from HowHeard order by SortOrder")
		End Function

	End Class

	Public MustInherit Class HowHeardRowBase
		Private m_DB As Database
		Private m_HowHeardId As Integer = Nothing
		Private m_HowHeard As String = Nothing
		Private m_IsUserInput As Boolean = False
		Private m_UserInputLabel As String = Nothing
		Private m_SortOrder As Integer = Nothing

		Public Property HowHeardId() As Integer
			Get
				Return m_HowHeardId
			End Get
			Set(ByVal Value As Integer)
				m_HowHeardId = Value
			End Set
		End Property

		Public Property HowHeard() As String
			Get
				Return m_HowHeard
			End Get
			Set(ByVal Value As String)
				m_HowHeard = value
			End Set
		End Property

		Public Property IsUserInput() As Boolean
			Get
				Return m_IsUserInput
			End Get
			Set(ByVal Value As Boolean)
				m_IsUserInput = Value
			End Set
		End Property

		Public Property UserInputLabel() As String
			Get
				Return m_UserInputLabel
			End Get
			Set(ByVal Value As String)
				m_UserInputLabel = Value
			End Set
		End Property

		Public Property SortOrder() As Integer
			Get
				Return m_SortOrder
			End Get
			Set(ByVal Value As Integer)
				m_SortOrder = Value
			End Set
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

		Public Sub New(ByVal DB As Database, ByVal HowHeardId As Integer)
			m_DB = DB
			m_HowHeardId = HowHeardId
		End Sub	'New

		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String

			SQL = "SELECT * FROM HowHeard WHERE HowHeardId = " & DB.Number(HowHeardId)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub

		Protected Overridable Sub Load(ByVal r As SqlDataReader)
			m_HowHeardId = Convert.ToInt32(r.Item("HowHeardId"))
			m_HowHeard = Convert.ToString(r.Item("HowHeard"))
			m_IsUserInput = Convert.ToString(r.Item("IsUserInput"))
			m_UserInputLabel = Convert.ToString(r.Item("UserInputLabel"))
		End Sub	'Load

		Public Overridable Function Insert() As Integer
			Dim SQL As String

			Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from HowHeard order by SortOrder desc")
			MaxSortOrder += 1

			SQL = " INSERT INTO HowHeard (" _
			 & " HowHeard" _
			 & ",IsUserInput" _
			 & ",UserInputLabel" _
			 & ",SortOrder" _
			 & ") VALUES (" _
			 & m_DB.Quote(HowHeard) _
			 & "," & CInt(IsUserInput) _
			 & "," & m_DB.Quote(UserInputLabel) _
			 & "," & MaxSortOrder _
			 & ")"

			HowHeardId = m_DB.InsertSQL(SQL)

			Return HowHeardId
		End Function

		Public Overridable Sub Update()
			Dim SQL As String

			SQL = " UPDATE HowHeard SET " _
			 & " HowHeard = " & m_DB.Quote(HowHeard) _
			 & " ,IsUserInput = " & CInt(IsUserInput) _
			 & " ,UserInputLabel = " & m_DB.Quote(UserInputLabel) _
			 & " WHERE HowHeardId = " & m_DB.Quote(HowHeardId)

			m_DB.ExecuteSQL(SQL)

		End Sub	'Update

		Public Sub Remove()
			Dim SQL As String

			SQL = "DELETE FROM HowHeard WHERE HowHeardId = " & m_DB.Quote(HowHeardId)
			m_DB.ExecuteSQL(SQL)
		End Sub	'Remove
	End Class

	Public Class HowHeardCollection
		Inherits GenericCollection(Of HowHeardRow)
	End Class

End Namespace