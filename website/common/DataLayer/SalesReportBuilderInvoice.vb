Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class SalesReportBuilderInvoiceRow
		Inherits SalesReportBuilderInvoiceRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, SalesReportBuilderInvoiceID as Integer)
			MyBase.New(DB, SalesReportBuilderInvoiceID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal SalesReportBuilderInvoiceID As Integer) As SalesReportBuilderInvoiceRow
			Dim row as SalesReportBuilderInvoiceRow 
			
			row = New SalesReportBuilderInvoiceRow(DB, SalesReportBuilderInvoiceID)
			row.Load()
			
			Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal SalesReportBuilderInvoiceID As Integer)
            Dim row As SalesReportBuilderInvoiceRow

            row = New SalesReportBuilderInvoiceRow(DB, SalesReportBuilderInvoiceID)
            row.Remove()
        End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from SalesReportBuilderInvoice"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods
        Public Shared Function GetBuilderInvoices(ByVal DB As Database, ByVal SalesReportID As Integer, ByVal BuilderID As Integer) As DataTable
            Dim sql As String = "select * from SalesReportBuilderInvoice where SalesReportID=" & DB.Number(SalesReportID) & " and BuilderID=" & DB.Number(BuilderID) & " order by InvoiceNumber, InvoiceDate"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetAllBuilderInvoices(ByVal DB As Database, ByVal BuilderId As Integer, ByVal PeriodQuarter As Integer, ByVal PeriodYear As Integer) As DataTable
            Dim sql As String = " select s.VendorID, i.* " _
                & " from SalesReportBuilderInvoice i inner join SalesReport s on i.SalesReportId=s.SalesReportId " _
                & " where i.BuilderId=" & DB.Number(BuilderId) & " and s.PeriodQuarter=" & DB.Number(PeriodQuarter) & " and s.PeriodYear=" & DB.Number(PeriodYear) _
                & " order by s.VendorId, i.InvoiceNumber, i.InvoiceDate"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetAllVendorInvoices(ByVal DB As Database, ByVal VendorID As Integer, ByVal PeriodQuarter As Integer, ByVal PeriodYear As Integer) As DataTable
            Dim sql As String = " select s.VendorID, i.* " _
                & " from SalesReportBuilderInvoice i inner join SalesReport s on i.SalesReportId=s.SalesReportId " _
                & " where s.VendorID = " & DB.Number(VendorID) & " and s.PeriodQuarter=" & DB.Number(PeriodQuarter) & " and s.PeriodYear=" & DB.Number(PeriodYear) _
                & " order by i.BuilderId, i.InvoiceNumber, i.InvoiceDate"
            Return DB.GetDataTable(sql)
        End Function

    End Class
	
	Public MustInherit Class SalesReportBuilderInvoiceRowBase
		Private m_DB as Database
		Private m_SalesReportBuilderInvoiceID As Integer = nothing
		Private m_SalesReportID As Integer = nothing
		Private m_BuilderID As Integer = nothing
		Private m_InvoiceAmount As Double = nothing
		Private m_InvoiceNumber As String = nothing
		Private m_InvoiceDate As DateTime = nothing
		Private m_Created As DateTime = nothing
		Private m_CreatorVendorAccountID As Integer = nothing
	
	
		Public Property SalesReportBuilderInvoiceID As Integer
			Get
				Return m_SalesReportBuilderInvoiceID
			End Get
			Set(ByVal Value As Integer)
				m_SalesReportBuilderInvoiceID = value
			End Set
		End Property
	
		Public Property SalesReportID As Integer
			Get
				Return m_SalesReportID
			End Get
			Set(ByVal Value As Integer)
				m_SalesReportID = value
			End Set
		End Property
	
		Public Property BuilderID As Integer
			Get
				Return m_BuilderID
			End Get
			Set(ByVal Value As Integer)
				m_BuilderID = value
			End Set
		End Property
	
		Public Property InvoiceAmount As Double
			Get
				Return m_InvoiceAmount
			End Get
			Set(ByVal Value As Double)
				m_InvoiceAmount = value
			End Set
		End Property
	
		Public Property InvoiceNumber As String
			Get
				Return m_InvoiceNumber
			End Get
			Set(ByVal Value As String)
				m_InvoiceNumber = value
			End Set
		End Property
	
		Public Property InvoiceDate As DateTime
			Get
				Return m_InvoiceDate
			End Get
			Set(ByVal Value As DateTime)
				m_InvoiceDate = value
			End Set
		End Property
	
        Public ReadOnly Property Created() As DateTime
            Get
                Return m_Created
            End Get
        End Property
	
		Public Property CreatorVendorAccountID As Integer
			Get
				Return m_CreatorVendorAccountID
			End Get
			Set(ByVal Value As Integer)
				m_CreatorVendorAccountID = value
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
	
		Public Sub New(ByVal DB As Database, SalesReportBuilderInvoiceID as Integer)
			m_DB = DB
			m_SalesReportBuilderInvoiceID = SalesReportBuilderInvoiceID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM SalesReportBuilderInvoice WHERE SalesReportBuilderInvoiceID = " & DB.Number(SalesReportBuilderInvoiceID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_SalesReportBuilderInvoiceID = Convert.ToInt32(r.Item("SalesReportBuilderInvoiceID"))
			m_SalesReportID = Convert.ToInt32(r.Item("SalesReportID"))
			m_BuilderID = Convert.ToInt32(r.Item("BuilderID"))
			m_InvoiceAmount = Convert.ToDouble(r.Item("InvoiceAmount"))
			if IsDBNull(r.Item("InvoiceNumber")) then
				m_InvoiceNumber = nothing
			else
				m_InvoiceNumber = Convert.ToString(r.Item("InvoiceNumber"))
			end if	
			if IsDBNull(r.Item("InvoiceDate")) then
				m_InvoiceDate = nothing
			else
				m_InvoiceDate = Convert.ToDateTime(r.Item("InvoiceDate"))
			end if	
			m_Created = Convert.ToDateTime(r.Item("Created"))
			m_CreatorVendorAccountID = Convert.ToInt32(r.Item("CreatorVendorAccountID"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
            SQL = " INSERT INTO SalesReportBuilderInvoice (" _
             & " SalesReportID" _
             & ",BuilderID" _
             & ",InvoiceAmount" _
             & ",InvoiceNumber" _
             & ",InvoiceDate" _
             & ",Created" _
             & ",CreatorVendorAccountID" _
             & ") VALUES (" _
             & m_DB.NullNumber(SalesReportID) _
             & "," & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.Number(InvoiceAmount) _
             & "," & m_DB.Quote(InvoiceNumber) _
             & "," & m_DB.NullQuote(InvoiceDate) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(CreatorVendorAccountID) _
             & ")"

			SalesReportBuilderInvoiceID = m_DB.InsertSQL(SQL)
			
			Return SalesReportBuilderInvoiceID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
            SQL = " UPDATE SalesReportBuilderInvoice SET " _
             & " SalesReportID = " & m_DB.NullNumber(SalesReportID) _
             & ",BuilderID = " & m_DB.NullNumber(BuilderID) _
             & ",InvoiceAmount = " & m_DB.Number(InvoiceAmount) _
             & ",InvoiceNumber = " & m_DB.Quote(InvoiceNumber) _
             & ",InvoiceDate = " & m_DB.NullQuote(InvoiceDate) _
             & " WHERE SalesReportBuilderInvoiceID = " & m_DB.Quote(SalesReportBuilderInvoiceID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM SalesReportBuilderInvoice WHERE SalesReportBuilderInvoiceID = " & m_DB.Number(SalesReportBuilderInvoiceID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class SalesReportBuilderInvoiceCollection
		Inherits GenericCollection(Of SalesReportBuilderInvoiceRow)
	End Class

End Namespace

