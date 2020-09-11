Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class RecommendedProductRow
		Inherits RecommendedProductRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, RecommendedProductID as Integer)
			MyBase.New(DB, RecommendedProductID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal RecommendedProductID As Integer) As RecommendedProductRow
			Dim row as RecommendedProductRow 
			
			row = New RecommendedProductRow(DB, RecommendedProductID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal RecommendedProductID As Integer)
			Dim row as RecommendedProductRow 
			
			row = New RecommendedProductRow(DB, RecommendedProductID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from RecommendedProduct"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods

	End Class
	
	Public MustInherit Class RecommendedProductRowBase
		Private m_DB as Database
		Private m_RecommendedProductID As Integer = nothing
		Private m_VendorID As Integer = nothing
		Private m_RecommendedProduct As String = nothing
		Private m_Description As String = nothing
		Private m_ManufacturerID As Integer = nothing
		Private m_Size As String = nothing
		Private m_UnitOfMeasureID As Integer = nothing
		Private m_Grade As String = nothing
		Private m_ProductTypeID As Integer = nothing
		Private m_RecommendedProductStatusID As Integer = nothing
		Private m_Submitted As DateTime = nothing
	
	
		Public Property RecommendedProductID As Integer
			Get
				Return m_RecommendedProductID
			End Get
			Set(ByVal Value As Integer)
				m_RecommendedProductID = value
			End Set
		End Property
	
		Public Property VendorID As Integer
			Get
				Return m_VendorID
			End Get
			Set(ByVal Value As Integer)
				m_VendorID = value
			End Set
		End Property
	
		Public Property RecommendedProduct As String
			Get
				Return m_RecommendedProduct
			End Get
			Set(ByVal Value As String)
				m_RecommendedProduct = value
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
	
		Public Property ManufacturerID As Integer
			Get
				Return m_ManufacturerID
			End Get
			Set(ByVal Value As Integer)
				m_ManufacturerID = value
			End Set
		End Property
	
		Public Property Size As String
			Get
				Return m_Size
			End Get
			Set(ByVal Value As String)
				m_Size = value
			End Set
		End Property
	
		Public Property UnitOfMeasureID As Integer
			Get
				Return m_UnitOfMeasureID
			End Get
			Set(ByVal Value As Integer)
				m_UnitOfMeasureID = value
			End Set
		End Property
	
		Public Property Grade As String
			Get
				Return m_Grade
			End Get
			Set(ByVal Value As String)
				m_Grade = value
			End Set
		End Property
	
		Public Property ProductTypeID As Integer
			Get
				Return m_ProductTypeID
			End Get
			Set(ByVal Value As Integer)
				m_ProductTypeID = value
			End Set
		End Property
	
		Public Property RecommendedProductStatusID As Integer
			Get
				Return m_RecommendedProductStatusID
			End Get
			Set(ByVal Value As Integer)
				m_RecommendedProductStatusID = value
			End Set
		End Property
	
        Public ReadOnly Property Submitted() As DateTime
            Get
                Return m_Submitted
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
	
		Public Sub New(ByVal DB As Database, RecommendedProductID as Integer)
			m_DB = DB
			m_RecommendedProductID = RecommendedProductID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM RecommendedProduct WHERE RecommendedProductID = " & DB.Number(RecommendedProductID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_RecommendedProductID = Convert.ToInt32(r.Item("RecommendedProductID"))
			m_VendorID = Convert.ToInt32(r.Item("VendorID"))
			m_RecommendedProduct = Convert.ToString(r.Item("RecommendedProduct"))
			if IsDBNull(r.Item("Description")) then
				m_Description = nothing
			else
				m_Description = Convert.ToString(r.Item("Description"))
			end if	
			if IsDBNull(r.Item("ManufacturerID")) then
				m_ManufacturerID = nothing
			else
				m_ManufacturerID = Convert.ToInt32(r.Item("ManufacturerID"))
			end if	
			if IsDBNull(r.Item("Size")) then
				m_Size = nothing
			else
				m_Size = Convert.ToString(r.Item("Size"))
			end if	
			if IsDBNull(r.Item("UnitOfMeasureID")) then
				m_UnitOfMeasureID = nothing
			else
				m_UnitOfMeasureID = Convert.ToInt32(r.Item("UnitOfMeasureID"))
			end if	
			if IsDBNull(r.Item("Grade")) then
				m_Grade = nothing
			else
				m_Grade = Convert.ToString(r.Item("Grade"))
			end if	
			m_ProductTypeID = Convert.ToInt32(r.Item("ProductTypeID"))
			m_RecommendedProductStatusID = Convert.ToInt32(r.Item("RecommendedProductStatusID"))
			m_Submitted = Convert.ToDateTime(r.Item("Submitted"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
            SQL = " INSERT INTO RecommendedProduct (" _
             & " VendorID" _
             & ",RecommendedProduct" _
             & ",Description" _
             & ",ManufacturerID" _
             & ",Size" _
             & ",UnitOfMeasureID" _
             & ",Grade" _
             & ",ProductTypeID" _
             & ",RecommendedProductStatusID" _
             & ",Submitted" _
             & ") VALUES (" _
             & m_DB.NullNumber(VendorID) _
             & "," & m_DB.Quote(RecommendedProduct) _
             & "," & m_DB.Quote(Description) _
             & "," & m_DB.NullNumber(ManufacturerID) _
             & "," & m_DB.Quote(Size) _
             & "," & m_DB.NullNumber(UnitOfMeasureID) _
             & "," & m_DB.Quote(Grade) _
             & "," & m_DB.NullNumber(ProductTypeID) _
             & "," & m_DB.NullNumber(RecommendedProductStatusID) _
             & "," & m_DB.NullQuote(Now) _
             & ")"

			RecommendedProductID = m_DB.InsertSQL(SQL)
			
			Return RecommendedProductID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
            SQL = " UPDATE RecommendedProduct SET " _
             & " VendorID = " & m_DB.NullNumber(VendorID) _
             & ",RecommendedProduct = " & m_DB.Quote(RecommendedProduct) _
             & ",Description = " & m_DB.Quote(Description) _
             & ",ManufacturerID = " & m_DB.NullNumber(ManufacturerID) _
             & ",Size = " & m_DB.Quote(Size) _
             & ",UnitOfMeasureID = " & m_DB.NullNumber(UnitOfMeasureID) _
             & ",Grade = " & m_DB.Quote(Grade) _
             & ",ProductTypeID = " & m_DB.NullNumber(ProductTypeID) _
             & ",RecommendedProductStatusID = " & m_DB.NullNumber(RecommendedProductStatusID) _
             & " WHERE RecommendedProductID = " & m_DB.Quote(RecommendedProductID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM RecommendedProduct WHERE RecommendedProductID = " & m_DB.Number(RecommendedProductID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class RecommendedProductCollection
		Inherits GenericCollection(Of RecommendedProductRow)
	End Class

End Namespace

