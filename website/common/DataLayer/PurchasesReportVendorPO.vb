Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class PurchasesReportVendorPORow
		Inherits PurchasesReportVendorPORowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, PurchasesReportVendorPOID as Integer)
			MyBase.New(DB, PurchasesReportVendorPOID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal PurchasesReportVendorPOID As Integer) As PurchasesReportVendorPORow
			Dim row as PurchasesReportVendorPORow 
			
			row = New PurchasesReportVendorPORow(DB, PurchasesReportVendorPOID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal PurchasesReportVendorPOID As Integer)
			Dim row as PurchasesReportVendorPORow 
			
			row = New PurchasesReportVendorPORow(DB, PurchasesReportVendorPOID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from PurchasesReportVendorPO"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods
        Public Shared Function GetAllVendorPOs(ByVal DB As Database, ByVal BuilderID As Integer, ByVal PeriodQuarter As Integer, ByVal PeriodYear As Integer) As DataTable
            Dim sql As String = _
                  " select o.* " _
                & " from PurchasesReportVendorPO o inner join PurchasesReport p on o.PurchasesReportId=p.PurchasesReportId " _
                & " where p.BuilderId=" & DB.Number(BuilderID) & " and PeriodQuarter=" & DB.Number(PeriodQuarter) & " and PeriodYear=" & DB.Number(PeriodYear) _
                & " order by o.VendorId, o.PONumber, o.PODate"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetVendorPOTotal(ByVal DB As Database, ByVal BuilderID As Integer, ByVal VendorID As Integer, ByVal PeriodQuarter As Integer, ByVal PeriodYear As Integer) As Double
            Dim sql As String = _
                  " select sum(coalesce(o.POAmount,0)) from PurchasesReportVendorPO o inner join PurchasesReport p on o.PurchasesReportId=p.PurchasesReportId" _
                & " where p.BuilderId=" & DB.Number(BuilderID) & " and PeriodQuarter=" & DB.Number(PeriodQuarter) & " and PeriodYear=" & DB.Number(PeriodYear)
            Return DB.ExecuteScalar(sql)
        End Function
	End Class
	
	Public MustInherit Class PurchasesReportVendorPORowBase
		Private m_DB as Database
		Private m_PurchasesReportVendorPOID As Integer = nothing
		Private m_PurchasesReportID As Integer = nothing
		Private m_VendorID As Integer = nothing
		Private m_POAmount As Double = nothing
		Private m_PONumber As String = nothing
		Private m_PODate As DateTime = nothing
		Private m_Created As DateTime = nothing
		Private m_CreatorBuilderAccountID As Integer = nothing

	
		Public Property PurchasesReportVendorPOID As Integer
			Get
				Return m_PurchasesReportVendorPOID
			End Get
			Set(ByVal Value As Integer)
				m_PurchasesReportVendorPOID = value
			End Set
		End Property
	
		Public Property PurchasesReportID As Integer
			Get
				Return m_PurchasesReportID
			End Get
			Set(ByVal Value As Integer)
				m_PurchasesReportID = value
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
	
		Public Property POAmount As Double
			Get
				Return m_POAmount
			End Get
			Set(ByVal Value As Double)
				m_POAmount = value
			End Set
		End Property
	
		Public Property PONumber As String
			Get
				Return m_PONumber
			End Get
			Set(ByVal Value As String)
				m_PONumber = value
			End Set
		End Property
	
		Public Property PODate As DateTime
			Get
				Return m_PODate
			End Get
			Set(ByVal Value As DateTime)
				m_PODate = value
			End Set
		End Property
	
        Public ReadOnly Property Created() As DateTime
            Get
                Return m_Created
            End Get
        End Property
	
		Public Property CreatorBuilderAccountID As Integer
			Get
				Return m_CreatorBuilderAccountID
			End Get
			Set(ByVal Value As Integer)
				m_CreatorBuilderAccountID = value
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
	
		Public Sub New(ByVal DB As Database, PurchasesReportVendorPOID as Integer)
			m_DB = DB
			m_PurchasesReportVendorPOID = PurchasesReportVendorPOID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM PurchasesReportVendorPO WHERE PurchasesReportVendorPOID = " & DB.Number(PurchasesReportVendorPOID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_PurchasesReportVendorPOID = Convert.ToInt32(r.Item("PurchasesReportVendorPOID"))
			m_PurchasesReportID = Convert.ToInt32(r.Item("PurchasesReportID"))
			m_VendorID = Convert.ToInt32(r.Item("VendorID"))
			m_POAmount = Convert.ToDouble(r.Item("POAmount"))
			if IsDBNull(r.Item("PONumber")) then
				m_PONumber = nothing
			else
				m_PONumber = Convert.ToString(r.Item("PONumber"))
			end if	
			if IsDBNull(r.Item("PODate")) then
				m_PODate = nothing
			else
				m_PODate = Convert.ToDateTime(r.Item("PODate"))
			end if	
			m_Created = Convert.ToDateTime(r.Item("Created"))
			m_CreatorBuilderAccountID = Convert.ToInt32(r.Item("CreatorBuilderAccountID"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
            SQL = " INSERT INTO PurchasesReportVendorPO (" _
             & " PurchasesReportID" _
             & ",VendorID" _
             & ",POAmount" _
             & ",PONumber" _
             & ",PODate" _
             & ",Created" _
             & ",CreatorBuilderAccountID" _
             & ") VALUES (" _
             & m_DB.NullNumber(PurchasesReportID) _
             & "," & m_DB.NullNumber(VendorID) _
             & "," & m_DB.Number(POAmount) _
             & "," & m_DB.Quote(PONumber) _
             & "," & m_DB.NullQuote(PODate) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(CreatorBuilderAccountID) _
             & ")"

			PurchasesReportVendorPOID = m_DB.InsertSQL(SQL)
			
			Return PurchasesReportVendorPOID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
            SQL = " UPDATE PurchasesReportVendorPO SET " _
             & " PurchasesReportID = " & m_DB.NullNumber(PurchasesReportID) _
             & ",VendorID = " & m_DB.NullNumber(VendorID) _
             & ",POAmount = " & m_DB.Number(POAmount) _
             & ",PONumber = " & m_DB.Quote(PONumber) _
             & ",PODate = " & m_DB.NullQuote(PODate) _
             & " WHERE PurchasesReportVendorPOID = " & m_DB.Quote(PurchasesReportVendorPOID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM PurchasesReportVendorPO WHERE PurchasesReportVendorPOID = " & m_DB.Number(PurchasesReportVendorPOID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class PurchasesReportVendorPOCollection
		Inherits GenericCollection(Of PurchasesReportVendorPORow)
	End Class

End Namespace

