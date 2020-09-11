Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class ProductTypeAttributeValueOptionRow
		Inherits ProductTypeAttributeValueOptionRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, ProductTypeAttributeValueOptionID as Integer)
			MyBase.New(DB, ProductTypeAttributeValueOptionID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal ProductTypeAttributeValueOptionID As Integer) As ProductTypeAttributeValueOptionRow
			Dim row as ProductTypeAttributeValueOptionRow 
			
			row = New ProductTypeAttributeValueOptionRow(DB, ProductTypeAttributeValueOptionID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal ProductTypeAttributeValueOptionID As Integer)
			Dim row as ProductTypeAttributeValueOptionRow 
			
			row = New ProductTypeAttributeValueOptionRow(DB, ProductTypeAttributeValueOptionID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from ProductTypeAttributeValueOption"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods

	End Class
	
	Public MustInherit Class ProductTypeAttributeValueOptionRowBase
		Private m_DB as Database
		Private m_ProductTypeAttributeValueOptionID As Integer = nothing
		Private m_ProductTypeAttributeID As Integer = nothing
		Private m_ValueOption As String = nothing

		Public Property ProductTypeAttributeValueOptionID As Integer
			Get
				Return m_ProductTypeAttributeValueOptionID
			End Get
			Set(ByVal Value As Integer)
				m_ProductTypeAttributeValueOptionID = value
			End Set
		End Property
	
		Public Property ProductTypeAttributeID As Integer
			Get
				Return m_ProductTypeAttributeID
			End Get
			Set(ByVal Value As Integer)
				m_ProductTypeAttributeID = value
			End Set
		End Property
	
		Public Property ValueOption As String
			Get
				Return m_ValueOption
			End Get
			Set(ByVal Value As String)
				m_ValueOption = value
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
	
		Public Sub New(ByVal DB As Database, ProductTypeAttributeValueOptionID as Integer)
			m_DB = DB
			m_ProductTypeAttributeValueOptionID = ProductTypeAttributeValueOptionID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM ProductTypeAttributeValueOption WHERE ProductTypeAttributeValueOptionID = " & DB.Number(ProductTypeAttributeValueOptionID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_ProductTypeAttributeValueOptionID = Convert.ToInt32(r.Item("ProductTypeAttributeValueOptionID"))
            m_ProductTypeAttributeID = Convert.ToInt32(r.Item("ProductTypeAttributeID"))
            m_ValueOption = Convert.ToString(r.Item("ValueOption"))
        End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
			SQL = " INSERT INTO ProductTypeAttributeValueOption (" _
				& " ProductTypeAttributeID" _
				& ",ValueOption" _
				& ") VALUES (" _
				& m_DB.NullNumber(ProductTypeAttributeID) _
				& "," & m_DB.Quote(ValueOption) _
				& ")"

			ProductTypeAttributeValueOptionID = m_DB.InsertSQL(SQL)
			
			Return ProductTypeAttributeValueOptionID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
			SQL = " UPDATE ProductTypeAttributeValueOption SET " _
				& " ProductTypeAttributeID = " & m_DB.NullNumber(ProductTypeAttributeID) _
				& ",ValueOption = " & m_DB.Quote(ValueOption) _
				& " WHERE ProductTypeAttributeValueOptionID = " & m_DB.quote(ProductTypeAttributeValueOptionID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM ProductTypeAttributeValueOption WHERE ProductTypeAttributeValueOptionID = " & m_DB.Number(ProductTypeAttributeValueOptionID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class ProductTypeAttributeValueOptionCollection
		Inherits GenericCollection(Of ProductTypeAttributeValueOptionRow)
	End Class

End Namespace

