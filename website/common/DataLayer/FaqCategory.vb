Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

	Public Class FaqCategoryRow
		Inherits FaqCategoryRowBase

		Public Sub New()
			MyBase.New()
		End Sub	'New

		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub	'New

		Public Sub New(ByVal DB As Database, ByVal FaqCategoryId As Integer)
			MyBase.New(DB, FaqCategoryId)
		End Sub	'New

		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB As Database, ByVal FaqCategoryId As Integer) As FaqCategoryRow
			Dim row As FaqCategoryRow

			row = New FaqCategoryRow(DB, FaqCategoryId)
			row.Load()

			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB As Database, ByVal FaqCategoryId As Integer)
			Dim row As FaqCategoryRow

			row = New FaqCategoryRow(DB, FaqCategoryId)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
			Dim SQL As String = "select * from FaqCategory"
			If Not SortBy = String.Empty Then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End If
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods
		Public Shared Function GetAllFaqCategorys(ByVal DB As Database) As DataSet
			Dim ds As DataSet = DB.GetDataSet("select * from FaqCategory order by CategoryName")
			Return ds
		End Function

	End Class

	Public MustInherit Class FaqCategoryRowBase
		Private m_DB As Database
		Private m_FaqCategoryId As Integer = Nothing
		Private m_SortOrder As Integer = Nothing
		Private m_IsActive As Boolean = Nothing
		Private m_CategoryName As String = Nothing
		Private m_AdminId As Integer = Nothing


		Public Property FaqCategoryId() As Integer
			Get
				Return m_FaqCategoryId
			End Get
			Set(ByVal Value As Integer)
				m_FaqCategoryId = value
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

		Public Property IsActive() As Boolean
			Get
				Return m_IsActive
			End Get
			Set(ByVal Value As Boolean)
				m_IsActive = value
			End Set
		End Property

		Public Property CategoryName() As String
			Get
				Return m_CategoryName
			End Get
			Set(ByVal Value As String)
				m_CategoryName = value
			End Set
		End Property

		Public Property AdminId() As Integer
			Get
				Return m_AdminId
			End Get
			Set(ByVal Value As Integer)
				m_AdminId = value
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

		Public Sub New(ByVal DB As Database, ByVal FaqCategoryId As Integer)
			m_DB = DB
			m_FaqCategoryId = FaqCategoryId
		End Sub	'New

		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String

			SQL = "SELECT * FROM FaqCategory WHERE FaqCategoryId = " & DB.Number(FaqCategoryId)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub


		Protected Overridable Sub Load(ByVal r As sqlDataReader)
			m_FaqCategoryId = Convert.ToInt32(r.Item("FaqCategoryId"))
			m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
			m_CategoryName = Convert.ToString(r.Item("CategoryName"))
			If IsDBNull(r.Item("AdminId")) Then
				m_AdminId = Nothing
			Else
				m_AdminId = Convert.ToInt32(r.Item("AdminId"))
			End If
		End Sub	'Load

		Public Overridable Function Insert() As Integer
			Dim SQL As String

			Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from FaqCategory order by SortOrder desc")
			MaxSortOrder += 1

			SQL = " INSERT INTO FaqCategory (" _
			 & " SortOrder" _
			 & ",IsActive" _
			 & ",CategoryName" _
			 & ",AdminId" _
			 & ") VALUES (" _
			 & CInt(SortOrder) _
			 & "," & CInt(IsActive) _
			 & "," & m_DB.Quote(CategoryName) _
			 & "," & m_DB.NullNumber(AdminId) _
			 & ")"

			FaqCategoryId = m_DB.InsertSQL(SQL)

			Return FaqCategoryId
		End Function

		Public Overridable Sub Update()
			Dim SQL As String

			SQL = " UPDATE FaqCategory SET " _
			 & " IsActive = " & CInt(IsActive) _
			 & ",CategoryName = " & m_DB.Quote(CategoryName) _
			 & ",AdminId = " & m_DB.NullNumber(AdminId) _
			 & " WHERE FaqCategoryId = " & m_DB.Quote(FaqCategoryId)

			m_DB.ExecuteSQL(SQL)

		End Sub	'Update

		Public Sub Remove()
			Dim SQL As String

			SQL = "DELETE FROM FaqCategory WHERE FaqCategoryId = " & m_DB.Number(FaqCategoryId)
			m_DB.ExecuteSQL(SQL)
		End Sub	'Remove
	End Class

	Public Class FaqCategoryCollection
		Inherits GenericCollection(Of FaqCategoryRow)
	End Class

End Namespace
