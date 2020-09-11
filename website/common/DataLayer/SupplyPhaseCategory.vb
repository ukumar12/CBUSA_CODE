Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class SupplyPhaseCategoryRow
		Inherits SupplyPhaseCategoryRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, SupplyPhaseCategoryId as Integer)
			MyBase.New(DB, SupplyPhaseCategoryId)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal SupplyPhaseCategoryId As Integer) As SupplyPhaseCategoryRow
			Dim row as SupplyPhaseCategoryRow 
			
			row = New SupplyPhaseCategoryRow(DB, SupplyPhaseCategoryId)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal SupplyPhaseCategoryId As Integer)
			Dim row as SupplyPhaseCategoryRow 
			
			row = New SupplyPhaseCategoryRow(DB, SupplyPhaseCategoryId)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from SupplyPhaseCategory"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods

	End Class
	
	Public MustInherit Class SupplyPhaseCategoryRowBase
		Private m_DB as Database
		Private m_SupplyPhaseCategoryId As Integer = nothing
		Private m_SupplyPhaseCategory As String = nothing
		Private m_Description As String = nothing
		Private m_SortOrder As Integer = nothing
		Private m_CreateDate As DateTime = nothing
		Private m_ModifyDate As DateTime = nothing
	
	
		Public Property SupplyPhaseCategoryId As Integer
			Get
				Return m_SupplyPhaseCategoryId
			End Get
			Set(ByVal Value As Integer)
				m_SupplyPhaseCategoryId = value
			End Set
		End Property
	
		Public Property SupplyPhaseCategory As String
			Get
				Return m_SupplyPhaseCategory
			End Get
			Set(ByVal Value As String)
				m_SupplyPhaseCategory = value
			End Set
		End Property
	
		Public Property Description As String
			Get
				Return m_Description
			End Get
			Set(ByVal Value As String)
				m_Description = value
			End Set
		End Property
	
		Public Property SortOrder As Integer
			Get
				Return m_SortOrder
			End Get
			Set(ByVal Value As Integer)
				m_SortOrder = value
			End Set
		End Property
	
		Public Readonly Property CreateDate As DateTime
			Get
				Return m_CreateDate
			End Get
		End Property
	
		Public Readonly Property ModifyDate As DateTime
			Get
				Return m_ModifyDate
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
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			m_DB = DB
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, SupplyPhaseCategoryId as Integer)
			m_DB = DB
			m_SupplyPhaseCategoryId = SupplyPhaseCategoryId
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM SupplyPhaseCategory WHERE SupplyPhaseCategoryId = " & DB.Number(SupplyPhaseCategoryId)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_SupplyPhaseCategoryId = Core.GetInt(r.Item("SupplyPhaseCategoryId"))
			m_SupplyPhaseCategory = Core.GetString(r.Item("SupplyPhaseCategory"))
			m_Description = Core.GetString(r.Item("Description"))
			m_CreateDate = Core.GetDate(r.Item("CreateDate"))
			m_ModifyDate = Core.GetDate(r.Item("ModifyDate"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
			Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from SupplyPhaseCategory order by SortOrder desc")
			MaxSortOrder += 1
	
			SQL = " INSERT INTO SupplyPhaseCategory (" _
				& " SupplyPhaseCategory" _
				& ",Description" _
				& ",SortOrder" _
				& ",CreateDate" _
				& ",ModifyDate" _
				& ") VALUES (" _
				& m_DB.Quote(SupplyPhaseCategory) _
				& "," & m_DB.Quote(Description) _
				& "," & MaxSortOrder _
				& "," & m_DB.NullQuote(Now) _
				& "," & m_DB.NullQuote(Now) _
				& ")"

			SupplyPhaseCategoryId = m_DB.InsertSQL(SQL)
			
			Return SupplyPhaseCategoryId
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
			SQL = " UPDATE SupplyPhaseCategory SET " _
				& " SupplyPhaseCategory = " & m_DB.Quote(SupplyPhaseCategory) _
				& ",Description = " & m_DB.Quote(Description) _
				& ",ModifyDate = " & m_DB.NullQuote(Now) _
				& " WHERE SupplyPhaseCategoryId = " & m_DB.quote(SupplyPhaseCategoryId)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM SupplyPhaseCategory WHERE SupplyPhaseCategoryId = " & m_DB.Number(SupplyPhaseCategoryId)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class SupplyPhaseCategoryCollection
		Inherits GenericCollection(Of SupplyPhaseCategoryRow)
	End Class

End Namespace

