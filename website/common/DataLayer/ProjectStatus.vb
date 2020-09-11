Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class ProjectStatusRow
		Inherits ProjectStatusRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, ProjectStatusID as Integer)
			MyBase.New(DB, ProjectStatusID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal ProjectStatusID As Integer) As ProjectStatusRow
			Dim row as ProjectStatusRow 
			
			row = New ProjectStatusRow(DB, ProjectStatusID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal ProjectStatusID As Integer)
			Dim row as ProjectStatusRow 
			
			row = New ProjectStatusRow(DB, ProjectStatusID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from ProjectStatus"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods

	End Class
	
	Public MustInherit Class ProjectStatusRowBase
		Private m_DB as Database
		Private m_ProjectStatusID As Integer = nothing
		Private m_ProjectStatus As String = nothing
		Private m_SortOrder As Integer = nothing
	
	
		Public Property ProjectStatusID As Integer
			Get
				Return m_ProjectStatusID
			End Get
			Set(ByVal Value As Integer)
				m_ProjectStatusID = value
			End Set
		End Property
	
		Public Property ProjectStatus As String
			Get
				Return m_ProjectStatus
			End Get
			Set(ByVal Value As String)
				m_ProjectStatus = value
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
	
		Public Sub New(ByVal DB As Database, ProjectStatusID as Integer)
			m_DB = DB
			m_ProjectStatusID = ProjectStatusID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM ProjectStatus WHERE ProjectStatusID = " & DB.Number(ProjectStatusID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_ProjectStatusID = Convert.ToInt32(r.Item("ProjectStatusID"))
			m_ProjectStatus = Convert.ToString(r.Item("ProjectStatus"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
			Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from ProjectStatus order by SortOrder desc")
			MaxSortOrder += 1
	
			SQL = " INSERT INTO ProjectStatus (" _
				& " ProjectStatus" _
				& ",SortOrder" _
				& ") VALUES (" _
				& m_DB.Quote(ProjectStatus) _
				& "," & MaxSortOrder _
				& ")"

			ProjectStatusID = m_DB.InsertSQL(SQL)
			
			Return ProjectStatusID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
			SQL = " UPDATE ProjectStatus SET " _
				& " ProjectStatus = " & m_DB.Quote(ProjectStatus) _
				& " WHERE ProjectStatusID = " & m_DB.quote(ProjectStatusID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM ProjectStatus WHERE ProjectStatusID = " & m_DB.Number(ProjectStatusID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class ProjectStatusCollection
		Inherits GenericCollection(Of ProjectStatusRow)
	End Class

End Namespace

