Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class ProductTypeRow
		Inherits ProductTypeRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, ProductTypeID as Integer)
			MyBase.New(DB, ProductTypeID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal ProductTypeID As Integer) As ProductTypeRow
			Dim row as ProductTypeRow 
			
			row = New ProductTypeRow(DB, ProductTypeID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal ProductTypeID As Integer)
			Dim row as ProductTypeRow 
			
			row = New ProductTypeRow(DB, ProductTypeID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from ProductType"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods

	End Class
	
	Public MustInherit Class ProductTypeRowBase
		Private m_DB as Database
		Private m_ProductTypeID As Integer = nothing
		Private m_ProductType As String = nothing
	
	
		Public Property ProductTypeID As Integer
			Get
				Return m_ProductTypeID
			End Get
			Set(ByVal Value As Integer)
				m_ProductTypeID = value
			End Set
		End Property
	
		Public Property ProductType As String
			Get
				Return m_ProductType
			End Get
			Set(ByVal Value As String)
				m_ProductType = value
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
	
		Public Sub New(ByVal DB As Database, ProductTypeID as Integer)
			m_DB = DB
			m_ProductTypeID = ProductTypeID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM ProductType WHERE ProductTypeID = " & DB.Number(ProductTypeID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_ProductTypeID = Convert.ToInt32(r.Item("ProductTypeID"))
			m_ProductType = Convert.ToString(r.Item("ProductType"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
			SQL = " INSERT INTO ProductType (" _
				& " ProductType" _
				& ") VALUES (" _
				& m_DB.Quote(ProductType) _
				& ")"

			ProductTypeID = m_DB.InsertSQL(SQL)
			
			Return ProductTypeID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
			SQL = " UPDATE ProductType SET " _
				& " ProductType = " & m_DB.Quote(ProductType) _
				& " WHERE ProductTypeID = " & m_DB.quote(ProductTypeID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM ProductType WHERE ProductTypeID = " & m_DB.Number(ProductTypeID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class ProductTypeCollection
		Inherits GenericCollection(Of ProductTypeRow)
	End Class

End Namespace

