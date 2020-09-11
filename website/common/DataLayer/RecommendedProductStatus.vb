Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class RecommendedProductStatusRow
		Inherits RecommendedProductStatusRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, RecommendedProductStatusID as Integer)
			MyBase.New(DB, RecommendedProductStatusID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal RecommendedProductStatusID As Integer) As RecommendedProductStatusRow
			Dim row as RecommendedProductStatusRow 
			
			row = New RecommendedProductStatusRow(DB, RecommendedProductStatusID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal RecommendedProductStatusID As Integer)
			Dim row as RecommendedProductStatusRow 
			
			row = New RecommendedProductStatusRow(DB, RecommendedProductStatusID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from RecommendedProductStatus"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods

	End Class
	
	Public MustInherit Class RecommendedProductStatusRowBase
		Private m_DB as Database
		Private m_RecommendedProductStatusID As Integer = nothing
		Private m_RecommendedProductStatus As String = nothing
	
	
		Public Property RecommendedProductStatusID As Integer
			Get
				Return m_RecommendedProductStatusID
			End Get
			Set(ByVal Value As Integer)
				m_RecommendedProductStatusID = value
			End Set
		End Property
	
		Public Property RecommendedProductStatus As String
			Get
				Return m_RecommendedProductStatus
			End Get
			Set(ByVal Value As String)
				m_RecommendedProductStatus = value
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
	
		Public Sub New(ByVal DB As Database, RecommendedProductStatusID as Integer)
			m_DB = DB
			m_RecommendedProductStatusID = RecommendedProductStatusID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM RecommendedProductStatus WHERE RecommendedProductStatusID = " & DB.Number(RecommendedProductStatusID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_RecommendedProductStatusID = Convert.ToInt32(r.Item("RecommendedProductStatusID"))
			m_RecommendedProductStatus = Convert.ToString(r.Item("RecommendedProductStatus"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
			SQL = " INSERT INTO RecommendedProductStatus (" _
				& " RecommendedProductStatus" _
				& ") VALUES (" _
				& m_DB.Quote(RecommendedProductStatus) _
				& ")"

			RecommendedProductStatusID = m_DB.InsertSQL(SQL)
			
			Return RecommendedProductStatusID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
			SQL = " UPDATE RecommendedProductStatus SET " _
				& " RecommendedProductStatus = " & m_DB.Quote(RecommendedProductStatus) _
				& " WHERE RecommendedProductStatusID = " & m_DB.quote(RecommendedProductStatusID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM RecommendedProductStatus WHERE RecommendedProductStatusID = " & m_DB.Number(RecommendedProductStatusID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class RecommendedProductStatusCollection
		Inherits GenericCollection(Of RecommendedProductStatusRow)
	End Class

End Namespace

