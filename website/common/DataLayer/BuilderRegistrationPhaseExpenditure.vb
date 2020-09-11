Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class BuilderRegistrationPhaseExpenditureRow
		Inherits BuilderRegistrationPhaseExpenditureRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, BuilderRegistrationPhaseExpenditureID as Integer)
			MyBase.New(DB, BuilderRegistrationPhaseExpenditureID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal BuilderRegistrationPhaseExpenditureID As Integer) As BuilderRegistrationPhaseExpenditureRow
			Dim row as BuilderRegistrationPhaseExpenditureRow 
			
			row = New BuilderRegistrationPhaseExpenditureRow(DB, BuilderRegistrationPhaseExpenditureID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal BuilderRegistrationPhaseExpenditureID As Integer)
			Dim row as BuilderRegistrationPhaseExpenditureRow 
			
			row = New BuilderRegistrationPhaseExpenditureRow(DB, BuilderRegistrationPhaseExpenditureID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from BuilderRegistrationPhaseExpenditure"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

        'Custom Methods

        Public Shared Function GetListByRegistration(ByVal DB As Database, ByVal BuilderRegistrationID As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select be.*, Case be.PreferredVendorId When 1 then be.OtherPreferredVendor Else (Select CompanyName From Vendor Where VendorId = be.PreferredVendorId) End As Vendor, (Select SupplyPhase From SupplyPhase Where SupplyPhaseId = be.SupplyPhaseId) As Phase from BuilderRegistrationPhaseExpenditure be Where be.BuilderRegistrationID = " & DB.Number(BuilderRegistrationID)
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

	End Class
	
	Public MustInherit Class BuilderRegistrationPhaseExpenditureRowBase
		Private m_DB as Database
		Private m_BuilderRegistrationPhaseExpenditureID As Integer = nothing
		Private m_BuilderRegistrationID As Integer = nothing
		Private m_SupplyPhaseID As Integer = nothing
		Private m_AmountSpentLastYear As Double = nothing
		Private m_PreferredVendorID As Integer = nothing
		Private m_OtherPreferredVendor As String = nothing
	
	
		Public Property BuilderRegistrationPhaseExpenditureID As Integer
			Get
				Return m_BuilderRegistrationPhaseExpenditureID
			End Get
			Set(ByVal Value As Integer)
				m_BuilderRegistrationPhaseExpenditureID = value
			End Set
		End Property
	
		Public Property BuilderRegistrationID As Integer
			Get
				Return m_BuilderRegistrationID
			End Get
			Set(ByVal Value As Integer)
				m_BuilderRegistrationID = value
			End Set
		End Property
	
		Public Property SupplyPhaseID As Integer
			Get
				Return m_SupplyPhaseID
			End Get
			Set(ByVal Value As Integer)
				m_SupplyPhaseID = value
			End Set
		End Property
	
		Public Property AmountSpentLastYear As Double
			Get
				Return m_AmountSpentLastYear
			End Get
			Set(ByVal Value As Double)
				m_AmountSpentLastYear = value
			End Set
		End Property
	
		Public Property PreferredVendorID As Integer
			Get
				Return m_PreferredVendorID
			End Get
			Set(ByVal Value As Integer)
				m_PreferredVendorID = value
			End Set
		End Property
	
		Public Property OtherPreferredVendor As String
			Get
				Return m_OtherPreferredVendor
			End Get
			Set(ByVal Value As String)
				m_OtherPreferredVendor = value
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
	
		Public Sub New(ByVal DB As Database, BuilderRegistrationPhaseExpenditureID as Integer)
			m_DB = DB
			m_BuilderRegistrationPhaseExpenditureID = BuilderRegistrationPhaseExpenditureID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM BuilderRegistrationPhaseExpenditure WHERE BuilderRegistrationPhaseExpenditureID = " & DB.Number(BuilderRegistrationPhaseExpenditureID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_BuilderRegistrationPhaseExpenditureID = Convert.ToInt32(r.Item("BuilderRegistrationPhaseExpenditureID"))
			m_BuilderRegistrationID = Convert.ToInt32(r.Item("BuilderRegistrationID"))
			m_SupplyPhaseID = Convert.ToInt32(r.Item("SupplyPhaseID"))
			m_AmountSpentLastYear = Convert.ToDouble(r.Item("AmountSpentLastYear"))
			m_PreferredVendorID = Convert.ToInt32(r.Item("PreferredVendorID"))
			if IsDBNull(r.Item("OtherPreferredVendor")) then
				m_OtherPreferredVendor = nothing
			else
				m_OtherPreferredVendor = Convert.ToString(r.Item("OtherPreferredVendor"))
			end if	
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
			SQL = " INSERT INTO BuilderRegistrationPhaseExpenditure (" _
				& " BuilderRegistrationID" _
				& ",SupplyPhaseID" _
				& ",AmountSpentLastYear" _
				& ",PreferredVendorID" _
				& ",OtherPreferredVendor" _
				& ") VALUES (" _
				& m_DB.NullNumber(BuilderRegistrationID) _
				& "," & m_DB.NullNumber(SupplyPhaseID) _
				& "," & m_DB.Number(AmountSpentLastYear) _
				& "," & m_DB.NullNumber(PreferredVendorID) _
				& "," & m_DB.Quote(OtherPreferredVendor) _
				& ")"

			BuilderRegistrationPhaseExpenditureID = m_DB.InsertSQL(SQL)
			
			Return BuilderRegistrationPhaseExpenditureID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
			SQL = " UPDATE BuilderRegistrationPhaseExpenditure SET " _
				& " BuilderRegistrationID = " & m_DB.NullNumber(BuilderRegistrationID) _
				& ",SupplyPhaseID = " & m_DB.NullNumber(SupplyPhaseID) _
				& ",AmountSpentLastYear = " & m_DB.Number(AmountSpentLastYear) _
				& ",PreferredVendorID = " & m_DB.NullNumber(PreferredVendorID) _
				& ",OtherPreferredVendor = " & m_DB.Quote(OtherPreferredVendor) _
				& " WHERE BuilderRegistrationPhaseExpenditureID = " & m_DB.quote(BuilderRegistrationPhaseExpenditureID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM BuilderRegistrationPhaseExpenditure WHERE BuilderRegistrationPhaseExpenditureID = " & m_DB.Number(BuilderRegistrationPhaseExpenditureID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class BuilderRegistrationPhaseExpenditureCollection
		Inherits GenericCollection(Of BuilderRegistrationPhaseExpenditureRow)
	End Class

End Namespace

