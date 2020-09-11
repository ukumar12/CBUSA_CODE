Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class VendorCategoryRow
		Inherits VendorCategoryRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, VendorCategoryID as Integer)
			MyBase.New(DB, VendorCategoryID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal VendorCategoryID As Integer) As VendorCategoryRow
			Dim row as VendorCategoryRow 
			
			row = New VendorCategoryRow(DB, VendorCategoryID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal VendorCategoryID As Integer)
			Dim row as VendorCategoryRow 
			
			row = New VendorCategoryRow(DB, VendorCategoryID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorCategory Order By Category"
            'if not SortBy = String.Empty then
            '	SortBy = Core.ProtectParam(SortBy)
            '	SortOrder = Core.ProtectParam(SortOrder)

            '	SQL &= " order by " & SortBy & " " & SortOrder
            'End if
			Return DB.GetDataTable(SQL)
		End Function

        Public Shared Function GetNames(ByVal DB As Database, ByVal ids As String, Optional ByVal Delimiter As String = "|") As String
            Dim sql As String = "select Category from VendorCategory where VendorCategoryId in " & DB.NumberMultiple(ids)
            Dim out As New StringBuilder()
            Dim conn As String = String.Empty
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            While sdr.Read
                out.Append(conn & sdr("Category"))
                conn = Delimiter
            End While
            sdr.Close()
            Return out.ToString
        End Function

        'Custom Methods

        Public Shared Function GetPOList(ByVal DB As Database) As DataTable
            Dim SQL As String = "select * from VendorCategory Where IsPlansOnline = 1 Order By Category"
            Return DB.GetDataTable(SQL)
        End Function

	End Class
	
	Public MustInherit Class VendorCategoryRowBase
		Private m_DB as Database
		Private m_VendorCategoryID As Integer = nothing
		Private m_Category As String = nothing
        Private m_SortOrder As Integer = Nothing
        Private m_IsPlansOnline As Boolean = Nothing
	
	
		Public Property VendorCategoryID As Integer
			Get
				Return m_VendorCategoryID
			End Get
			Set(ByVal Value As Integer)
				m_VendorCategoryID = value
			End Set
		End Property
	
		Public Property Category As String
			Get
				Return m_Category
			End Get
			Set(ByVal Value As String)
				m_Category = value
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

        Public Property IsPlansOnline() As Boolean
            Get
                Return m_IsPlansOnline
            End Get
            Set(ByVal Value As Boolean)
                m_IsPlansOnline = value
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
	
		Public Sub New(ByVal DB As Database, VendorCategoryID as Integer)
			m_DB = DB
			m_VendorCategoryID = VendorCategoryID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM VendorCategory WHERE VendorCategoryID = " & DB.Number(VendorCategoryID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_VendorCategoryID = Core.GetInt(r.Item("VendorCategoryID"))
            m_Category = Core.GetString(r.Item("Category"))
            m_IsPlansOnline = Core.GetBoolean(r.Item("IsPlansOnline"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
			Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from VendorCategory order by SortOrder desc")
			MaxSortOrder += 1
	
            SQL = " INSERT INTO VendorCategory (" _
             & " Category" _
             & ",SortOrder" _
             & ",IsPlansOnline" _
             & ") VALUES (" _
             & m_DB.Quote(Category) _
             & "," & MaxSortOrder _
             & "," & CInt(IsPlansOnline) _
             & ")"

			VendorCategoryID = m_DB.InsertSQL(SQL)
			
			Return VendorCategoryID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
            SQL = " UPDATE VendorCategory SET " _
             & " Category = " & m_DB.Quote(Category) _
             & ",IsPlansOnline = " & CInt(IsPlansOnline) _
             & " WHERE VendorCategoryID = " & m_DB.Quote(VendorCategoryID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM VendorCategory WHERE VendorCategoryID = " & m_DB.Number(VendorCategoryID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class VendorCategoryCollection
		Inherits GenericCollection(Of VendorCategoryRow)
	End Class

End Namespace

