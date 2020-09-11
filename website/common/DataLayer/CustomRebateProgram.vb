Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class CustomRebateProgramRow
		Inherits CustomRebateProgramRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, CustomRebateProgramID as Integer)
			MyBase.New(DB, CustomRebateProgramID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal CustomRebateProgramID As Integer) As CustomRebateProgramRow
			Dim row as CustomRebateProgramRow 
			
			row = New CustomRebateProgramRow(DB, CustomRebateProgramID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal CustomRebateProgramID As Integer)
			Dim row as CustomRebateProgramRow 
			
			row = New CustomRebateProgramRow(DB, CustomRebateProgramID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from CustomRebateProgram"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods
        Public Shared Function GetVendorPrograms(ByVal DB As Database, ByVal VendorID As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC", Optional ByVal PageNumber As Integer = 0, Optional ByVal PageSize As Integer = 10) As DataTable
            Dim base As String = " * from CustomRebateProgram where VendorId=" & DB.Number(VendorID)
            Dim sql As String = "select " & base
            If SortBy <> String.Empty Then
                Dim orderby As String = Core.ProtectParam(SortBy & " " & SortOrder)
                If PageNumber <> Nothing Then
                    Sql = "select top " & PageSize & " temp.* from (select Row_Number() Over (order by " & orderby & ") as RowNumber," & base & ") as temp where temp.RowNumber >= " & DB.Number((PageNumber - 1) * PageSize) & " order by RowNumber"
                Else
                    Sql = "select " & base & " order by " & orderby
                End If
            End If
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetVendorProgramCount(ByVal DB As Database, ByVal VendorId As Integer) As Integer
            Return DB.ExecuteScalar("select count(*) from CustomRebateProgram where VendorId=" & DB.Number(VendorId))
        End Function
	End Class
	
	Public MustInherit Class CustomRebateProgramRowBase
		Private m_DB as Database
		Private m_CustomRebateProgramID As Integer = nothing
		Private m_VendorID As Integer = nothing
		Private m_ProgramName As String = nothing
		Private m_RebateYear As Integer = nothing
		Private m_RebateQuarter As Integer = nothing
		Private m_MinimumPurchase As Double = nothing
		Private m_RebatePercentage As Integer = nothing
		Private m_Details As String = nothing
	
	
		Public Property CustomRebateProgramID As Integer
			Get
				Return m_CustomRebateProgramID
			End Get
			Set(ByVal Value As Integer)
				m_CustomRebateProgramID = value
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
	
		Public Property ProgramName As String
			Get
				Return m_ProgramName
			End Get
			Set(ByVal Value As String)
				m_ProgramName = value
			End Set
		End Property
	
		Public Property RebateYear As Integer
			Get
				Return m_RebateYear
			End Get
			Set(ByVal Value As Integer)
				m_RebateYear = value
			End Set
		End Property
	
		Public Property RebateQuarter As Integer
			Get
				Return m_RebateQuarter
			End Get
			Set(ByVal Value As Integer)
				m_RebateQuarter = value
			End Set
		End Property
	
		Public Property MinimumPurchase As Double
			Get
				Return m_MinimumPurchase
			End Get
			Set(ByVal Value As Double)
				m_MinimumPurchase = value
			End Set
		End Property
	
		Public Property RebatePercentage As Integer
			Get
				Return m_RebatePercentage
			End Get
			Set(ByVal Value As Integer)
				m_RebatePercentage = value
			End Set
		End Property
	
		Public Property Details As String
			Get
				Return m_Details
			End Get
			Set(ByVal Value As String)
				m_Details = value
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
	
		Public Sub New(ByVal DB As Database, CustomRebateProgramID as Integer)
			m_DB = DB
			m_CustomRebateProgramID = CustomRebateProgramID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM CustomRebateProgram WHERE CustomRebateProgramID = " & DB.Number(CustomRebateProgramID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_CustomRebateProgramID = Core.GetInt(r.Item("CustomRebateProgramID"))
			m_VendorID = Core.GetInt(r.Item("VendorID"))
			m_ProgramName = Core.GetString(r.Item("ProgramName"))
			m_RebateYear = Core.GetInt(r.Item("RebateYear"))
			m_RebateQuarter = Core.GetInt(r.Item("RebateQuarter"))
			m_MinimumPurchase = Core.GetDouble(r.Item("MinimumPurchase"))
			m_RebatePercentage = Core.GetInt(r.Item("RebatePercentage"))
			m_Details = Core.GetString(r.Item("Details"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
			SQL = " INSERT INTO CustomRebateProgram (" _
				& " VendorID" _
				& ",ProgramName" _
				& ",RebateYear" _
				& ",RebateQuarter" _
				& ",MinimumPurchase" _
				& ",RebatePercentage" _
				& ",Details" _
				& ") VALUES (" _
				& m_DB.NullNumber(VendorID) _
				& "," & m_DB.Quote(ProgramName) _
				& "," & m_DB.Number(RebateYear) _
				& "," & m_DB.Number(RebateQuarter) _
				& "," & m_DB.Number(MinimumPurchase) _
				& "," & m_DB.Number(RebatePercentage) _
				& "," & m_DB.Quote(Details) _
				& ")"

			CustomRebateProgramID = m_DB.InsertSQL(SQL)
			
			Return CustomRebateProgramID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
			SQL = " UPDATE CustomRebateProgram SET " _
				& " VendorID = " & m_DB.NullNumber(VendorID) _
				& ",ProgramName = " & m_DB.Quote(ProgramName) _
				& ",RebateYear = " & m_DB.Number(RebateYear) _
				& ",RebateQuarter = " & m_DB.Number(RebateQuarter) _
				& ",MinimumPurchase = " & m_DB.Number(MinimumPurchase) _
				& ",RebatePercentage = " & m_DB.Number(RebatePercentage) _
				& ",Details = " & m_DB.Quote(Details) _
				& " WHERE CustomRebateProgramID = " & m_DB.quote(CustomRebateProgramID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM CustomRebateProgram WHERE CustomRebateProgramID = " & m_DB.Number(CustomRebateProgramID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class CustomRebateProgramCollection
		Inherits GenericCollection(Of CustomRebateProgramRow)
	End Class

End Namespace

