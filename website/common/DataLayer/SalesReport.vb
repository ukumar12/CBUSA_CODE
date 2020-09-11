Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class SalesReportRow
		Inherits SalesReportRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, SalesReportID as Integer)
			MyBase.New(DB, SalesReportID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal SalesReportID As Integer) As SalesReportRow
			Dim row as SalesReportRow 
			
			row = New SalesReportRow(DB, SalesReportID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal SalesReportID As Integer)
			Dim row as SalesReportRow 
			
			row = New SalesReportRow(DB, SalesReportID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from SalesReport"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods
        Public Shared Function GetBuilders(ByVal DB As Database, ByVal SalesReportID As Integer) As DataTable
            Dim sql As String = _
                " select *" _
                & " from SalesReport r" _
                & "     inner join SalesReportBuilderTotalAmount t on r.SalesReportID=t.SalesReportID" _
                & " where " _
                & "     r.SalesReportID=" & DB.Number(SalesReportID)
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetInvoices(ByVal DB As Database, ByVal SalesReportID As Integer) As DataTable
            Dim sql As String = "select * from SalesReportBuilderInvoice where SalesReportID=" & DB.Number(SalesReportID) & " order by BuilderID, InvoiceNumber, InvoiceDate"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetSalesReportByDate(ByVal DB As Database, ByVal VendorId As Integer, Optional ByVal ReportDate As DateTime = Nothing) As SalesReportRow
            If ReportDate = Nothing Then ReportDate = Now
            Dim year As Integer = DatePart(DateInterval.Year, ReportDate)
            Dim month As Integer = DatePart(DateInterval.Month, ReportDate)
            Dim quarter = month / 4I + 1
            Return GetSalesReportByPeriod(DB, VendorId, year, quarter)
        End Function

        Public Shared Function GetSalesReportByPeriod(ByVal DB As Database, ByVal VendorId As Integer, ByVal Year As Integer, ByVal Quarter As Integer) As SalesReportRow
            Dim out As New SalesReportRow(DB)
            Dim sql As String = "select * from SalesReport where VendorId=" & DB.Number(VendorId) & " and PeriodYear=" & DB.Number(Year) & " and PeriodQuarter=" & DB.Number(Quarter)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read() Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetVendorReports(ByVal DB As Database, ByVal VendorID As Integer, Optional ByVal IncludeCurrent As Boolean = False, Optional ByVal IncludeCurrentReportingPeriod As Boolean = True) As DataTable

            Dim currentQtr As Integer = Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
            Dim lastQtr As Integer = IIf(currentQtr = 1, 4, currentQtr - 1)
            Dim lastYear As Integer = IIf(lastQtr = 4, Now.Year - 1, Now.Year)
            Dim lastQtrEnd As DateTime = (lastQtr * 3) & "/" & Date.DaysInMonth(lastYear, lastQtr * 3) & "/" & lastYear
            Dim sql As String = "select sr.*, (select sum(coalesce(TotalAmount,0)) from SalesReportBuilderTotalAmount where SalesReportID=sr.SalesReportID) as TotalAmount from SalesReport sr where sr.VendorID=" & DB.Number(VendorID)

            If Not IncludeCurrent Then
                sql &= " and (PeriodYear < " & IIf(IncludeCurrentReportingPeriod, lastYear, Now.Year) & " or (PeriodYear = " & IIf(IncludeCurrentReportingPeriod, lastYear, Now.Year) & " and PeriodQuarter < " & IIf(IncludeCurrentReportingPeriod, lastQtr, Math.Ceiling(Now.Month / 3)) & "))"
            End If
            sql &= " Order by PeriodYear Desc, PeriodQuarter Desc"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetBuildersAndDisputes(ByVal DB As Database, ByVal SalesReportID As Integer) As DataTable
            Dim sql As String = _
                " select *, (select CompanyName from Builder where BuilderID=t.BuilderID) as CompanyName" _
                & " from SalesReport r" _
                & "     inner join SalesReportBuilderTotalAmount t on r.SalesReportID=t.SalesReportID" _
                & "     left outer join SalesReportDispute srd on srd.SalesReportID=r.SalesReportID and srd.BuilderID=t.BuilderID" _
                & " where " _
                & "     r.SalesReportID=" & DB.Number(SalesReportID)

            Return DB.GetDataTable(sql)
        End Function
        Public Shared Function GetUnReportedBuildersAndDisputes(ByVal DB As Database, ByVal Vendorid As Integer, ByVal PeriodQuarter As Integer, ByVal PeriodYear As Integer) As DataTable
            ' When vendor does not submit a sales report or does not submit data for a builder
            'Get that data from purchase report and display - so both vendor and builder view are same.
            Dim Sql As New System.Text.StringBuilder
            Sql.Append("select *, (select CompanyName from Builder where BuilderID =r.BuilderID) as CompanyName from PurchasesReport r ")
            Sql.Append("   inner join PurchasesReportVendorTotalAmount t on r.PurchasesReportID=t.PurchasesReportID ")
            Sql.Append("       left outer join (select srt.TotalAmount as VendorReportedTotal, sr.PeriodYear, sr.PeriodQuarter, ")
            Sql.Append("       sr.VendorID, srd.* from SalesReportDispute srd inner join SalesReport sr on srd.SalesReportID=sr.SalesReportID ")
            Sql.Append("       left outer join SalesReportBuilderTotalAmount srt on srd.SalesReportID=srt.SalesReportID and srd.BuilderID=srt.BuilderID) ")
            Sql.Append("       as d on d.PeriodQuarter=r.PeriodQuarter and d.PeriodYear=r.PeriodYear ")
            Sql.Append("  and d.VendorID=t.VendorID and d.BuilderID=r.BuilderID where ")
            Sql.Append("    r.PeriodQuarter = " & PeriodQuarter & " and  r.PeriodYear = " & PeriodYear & " and   t.VendorID = " & Vendorid)


            Return DB.GetDataTable(Sql.ToString)
        End Function
        Public Function GetReportedSales(BuilderId As Integer, Optional ByVal DisplayDNR As Boolean = True) As Object
            Dim FromSQL As String = " FROM SalesReportBuilderTotalAmount WHERE SalesReportID = " & SalesReportID & " AND BuilderID = " & BuilderId
            Dim Val As Object = DB.ExecuteScalar("SELECT TOP 1 TotalAmount " & FromSQL)

            If DB.ExecuteScalar("SELECT TOP 1 CreatorVendorAccountID " & FromSQL) <= 0 OrElse IsDBNull(Val) Then
                'If the builder doesn't have an entry in SalesReportBuilderTotalAmount, or the entry has -1 for a VendorID, the vendor never reported this sale - DNR

                If DisplayDNR Then
                    If Not IsDBNull(DB.ExecuteScalar("Select Submitted from SalesReport Where SalesReportID = " & SalesReportID)) Then
                        Return FormatCurrency(0.0)
                    End If
                    Return "DNR"
                Else
                    Return FormatCurrency(0.0)
                End If
            Else
                Return FormatCurrency(Val)
            End If
        End Function
    End Class
	
	Public MustInherit Class SalesReportRowBase
		Private m_DB as Database
		Private m_SalesReportID As Integer = nothing
		Private m_VendorID As Integer = nothing
		Private m_PeriodYear As Integer = nothing
		Private m_PeriodQuarter As Integer = nothing
		Private m_Created As DateTime = nothing
		Private m_CreatorVendorAccountID As Integer = nothing
		Private m_Submitted As DateTime = nothing
		Private m_SubmitterVendorAccountID As Integer = nothing
	
	
		Public Property SalesReportID As Integer
			Get
				Return m_SalesReportID
			End Get
			Set(ByVal Value As Integer)
				m_SalesReportID = value
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
	
		Public Property PeriodYear As Integer
			Get
				Return m_PeriodYear
			End Get
			Set(ByVal Value As Integer)
				m_PeriodYear = value
			End Set
		End Property
	
		Public Property PeriodQuarter As Integer
			Get
				Return m_PeriodQuarter
			End Get
			Set(ByVal Value As Integer)
				m_PeriodQuarter = value
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
	
        Public Property Submitted() As DateTime
            Get
                Return m_Submitted
            End Get
            Set(ByVal value As DateTime)
                m_Submitted = value
            End Set
        End Property
	
		Public Property SubmitterVendorAccountID As Integer
			Get
				Return m_SubmitterVendorAccountID
			End Get
			Set(ByVal Value As Integer)
				m_SubmitterVendorAccountID = value
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
	
		Public Sub New(ByVal DB As Database, SalesReportID as Integer)
			m_DB = DB
			m_SalesReportID = SalesReportID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM SalesReport WHERE SalesReportID = " & DB.Number(SalesReportID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_SalesReportID = Convert.ToInt32(r.Item("SalesReportID"))
			m_VendorID = Convert.ToInt32(r.Item("VendorID"))
			m_PeriodYear = Convert.ToInt32(r.Item("PeriodYear"))
			m_PeriodQuarter = Convert.ToInt32(r.Item("PeriodQuarter"))
			m_Created = Convert.ToDateTime(r.Item("Created"))
			m_CreatorVendorAccountID = Convert.ToInt32(r.Item("CreatorVendorAccountID"))
			if IsDBNull(r.Item("Submitted")) then
				m_Submitted = nothing
			else
				m_Submitted = Convert.ToDateTime(r.Item("Submitted"))
			end if	
			if IsDBNull(r.Item("SubmitterVendorAccountID")) then
				m_SubmitterVendorAccountID = nothing
			else
				m_SubmitterVendorAccountID = Convert.ToInt32(r.Item("SubmitterVendorAccountID"))
			end if	
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
            SQL = " INSERT INTO SalesReport (" _
             & " VendorID" _
             & ",PeriodYear" _
             & ",PeriodQuarter" _
             & ",Created" _
             & ",CreatorVendorAccountID" _
             & ",Submitted" _
             & ",SubmitterVendorAccountID" _
             & ") VALUES (" _
             & m_DB.NullNumber(VendorID) _
             & "," & m_DB.Number(PeriodYear) _
             & "," & m_DB.Number(PeriodQuarter) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(CreatorVendorAccountID) _
             & "," & m_DB.NullQuote(Submitted) _
             & "," & m_DB.NullNumber(SubmitterVendorAccountID) _
             & ")"

			SalesReportID = m_DB.InsertSQL(SQL)
			
			Return SalesReportID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
            SQL = " UPDATE SalesReport SET " _
             & " VendorID = " & m_DB.NullNumber(VendorID) _
             & ",PeriodYear = " & m_DB.Number(PeriodYear) _
             & ",PeriodQuarter = " & m_DB.Number(PeriodQuarter) _
             & ",Submitted = " & m_DB.NullQuote(Submitted) _
             & ",SubmitterVendorAccountID = " & m_DB.NullNumber(SubmitterVendorAccountID) _
             & " WHERE SalesReportID = " & m_DB.Quote(SalesReportID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM SalesReport WHERE SalesReportID = " & m_DB.Number(SalesReportID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class SalesReportCollection
		Inherits GenericCollection(Of SalesReportRow)
	End Class

End Namespace

