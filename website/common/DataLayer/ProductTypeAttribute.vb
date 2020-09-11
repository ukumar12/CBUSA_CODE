Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

    Public Enum InputType
        Text
        Number
        Dropdown
        YesNo
    End Enum

	Public Class ProductTypeAttributeRow
		Inherits ProductTypeAttributeRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, ProductTypeAttributeID as Integer)
			MyBase.New(DB, ProductTypeAttributeID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal ProductTypeAttributeID As Integer) As ProductTypeAttributeRow
			Dim row as ProductTypeAttributeRow 
			
			row = New ProductTypeAttributeRow(DB, ProductTypeAttributeID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal ProductTypeAttributeID As Integer)
			Dim row as ProductTypeAttributeRow 
			
			row = New ProductTypeAttributeRow(DB, ProductTypeAttributeID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from ProductTypeAttribute"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods
        Public Shared Function GetListByType(ByVal DB As Database, ByVal ProductTypeId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim sql As String = "select * from ProductTypeAttribute where ProductTypeId = " & DB.Number(ProductTypeId)
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetOptions(ByVal DB As Database, ByVal ProductTypeAttributeId As Integer) As DataTable
            Dim sql As String = "select * from ProductTypeAttributeValueOption where ProductTypeAttributeId=" & DB.Number(ProductTypeAttributeId)
            Return DB.GetDataTable(sql)
        End Function

        Public Function ClearOptions() As Boolean
            Dim sql As String = "delete from ProductTypeAttributeValueOption where ProductTypeAttributeId=" & DB.Number(ProductTypeAttributeID)
            Return DB.ExecuteSQL(sql)
        End Function
	End Class
	
	Public MustInherit Class ProductTypeAttributeRowBase
		Private m_DB as Database
		Private m_ProductTypeAttributeID As Integer = nothing
		Private m_ProductTypeID As Integer = nothing
		Private m_Attribute As String = nothing
        Private m_InputType As InputType = Nothing
		Private m_IsRequired As Boolean = nothing
		Private m_DefaultValue As String = nothing
		Private m_IsActive As Boolean = nothing
	
	
		Public Property ProductTypeAttributeID As Integer
			Get
				Return m_ProductTypeAttributeID
			End Get
			Set(ByVal Value As Integer)
				m_ProductTypeAttributeID = value
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
	
		Public Property Attribute As String
			Get
				Return m_Attribute
			End Get
			Set(ByVal Value As String)
				m_Attribute = value
			End Set
		End Property
	
        Public Property InputType() As InputType
            Get
                Return m_InputType
            End Get
            Set(ByVal Value As InputType)
                m_InputType = Value
            End Set
        End Property
	
		Public Property IsRequired As Boolean
			Get
				Return m_IsRequired
			End Get
			Set(ByVal Value As Boolean)
				m_IsRequired = value
			End Set
		End Property
	
		Public Property DefaultValue As String
			Get
				Return m_DefaultValue
			End Get
			Set(ByVal Value As String)
				m_DefaultValue = value
			End Set
		End Property
	
		Public Property IsActive As Boolean
			Get
				Return m_IsActive
			End Get
			Set(ByVal Value As Boolean)
				m_IsActive = value
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
	
		Public Sub New(ByVal DB As Database, ProductTypeAttributeID as Integer)
			m_DB = DB
			m_ProductTypeAttributeID = ProductTypeAttributeID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM ProductTypeAttribute WHERE ProductTypeAttributeID = " & DB.Number(ProductTypeAttributeID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_ProductTypeAttributeID = Convert.ToInt32(r.Item("ProductTypeAttributeID"))
			m_ProductTypeID = Convert.ToInt32(r.Item("ProductTypeID"))
			m_Attribute = Convert.ToString(r.Item("Attribute"))
            m_InputType = Convert.ToInt32(r.Item("InputTypeId"))
			m_IsRequired = Convert.ToBoolean(r.Item("IsRequired"))
			if IsDBNull(r.Item("DefaultValue")) then
				m_DefaultValue = nothing
			else
				m_DefaultValue = Convert.ToString(r.Item("DefaultValue"))
			end if	
			m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String

            SQL = " INSERT INTO ProductTypeAttribute (" _
             & " ProductTypeID" _
             & ",Attribute" _
             & ",InputTypeId" _
             & ",IsRequired" _
             & ",DefaultValue" _
             & ",IsActive" _
             & ") VALUES (" _
             & m_DB.NullNumber(ProductTypeID) _
             & "," & m_DB.Quote(Attribute) _
             & "," & m_DB.Number(InputType) _
             & "," & CInt(IsRequired) _
             & "," & m_DB.Quote(DefaultValue) _
             & "," & CInt(IsActive) _
             & ")"

			ProductTypeAttributeID = m_DB.InsertSQL(SQL)
			
			Return ProductTypeAttributeID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
            SQL = " UPDATE ProductTypeAttribute SET " _
             & " ProductTypeID = " & m_DB.NullNumber(ProductTypeID) _
             & ",Attribute = " & m_DB.Quote(Attribute) _
             & ",InputTypeId = " & m_DB.Number(InputType) _
             & ",IsRequired = " & CInt(IsRequired) _
             & ",DefaultValue = " & m_DB.Quote(DefaultValue) _
             & ",IsActive = " & CInt(IsActive) _
             & " WHERE ProductTypeAttributeID = " & m_DB.Quote(ProductTypeAttributeID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM ProductTypeAttribute WHERE ProductTypeAttributeID = " & m_DB.Number(ProductTypeAttributeID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class ProductTypeAttributeCollection
		Inherits GenericCollection(Of ProductTypeAttributeRow)
	End Class

End Namespace

