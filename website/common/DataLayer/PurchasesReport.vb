Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class PurchasesReportRow
		Inherits PurchasesReportRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
        Public Sub New(ByVal DB As Database, ByVal PurchasesReportID As Integer)
            MyBase.New(DB, PurchasesReportID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal PurchasesReportID As Integer) As PurchasesReportRow
            Dim row As PurchasesReportRow

            row = New PurchasesReportRow(DB, PurchasesReportID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal PurchasesReportID As Integer)
            Dim row As PurchasesReportRow

            row = New PurchasesReportRow(DB, PurchasesReportID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from PurchasesReport"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetPurchasesReportByPeriod(ByVal DB As Database, ByVal BuilderID As Integer, ByVal Year As Integer, ByVal Quarter As Integer) As PurchasesReportRow
            Dim out As New PurchasesReportRow(DB)
            Dim sql As String = "select * from PurchasesReport where BuilderID=" & DB.Number(BuilderID) & " and PeriodYear=" & DB.Number(Year) & " and PeriodQuarter=" & DB.Number(Quarter)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read() Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetVendors(ByVal DB As Database, ByVal PurchasesReportID As Integer) As DataTable
            Dim sql As String = _
                " select *" _
                & " from PurchasesReport r" _
                & "     inner join PurchasesReportVendorTotalAmount t on r.PurchasesReportID=t.PurchasesReportID" _
                & " where " _
                & "     r.PurchasesReportID=" & DB.Number(PurchasesReportID)
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetVendorsAndDisputes(ByVal DB As Database, ByVal PurchasesReportID As Integer) As DataTable
            Dim sql As String = _
                " select *, (select CompanyName from Vendor where VendorID=t.VendorID) as CompanyName" _
                & " from PurchasesReport r" _
                & "     inner join PurchasesReportVendorTotalAmount t on r.PurchasesReportID=t.PurchasesReportID" _
                & "     left outer join (select srt.TotalAmount as VendorReportedTotal, sr.PeriodYear, sr.PeriodQuarter, sr.VendorID, srd.* from SalesReportDispute srd inner join SalesReport sr on srd.SalesReportID=sr.SalesReportID left outer join SalesReportBuilderTotalAmount srt on srd.SalesReportID=srt.SalesReportID and srd.BuilderID=srt.BuilderID) as d on d.PeriodQuarter=r.PeriodQuarter and d.PeriodYear=r.PeriodYear and d.VendorID=t.VendorID and d.BuilderID=r.BuilderID" _
                & " where " _
                & "     r.PurchasesReportID=" & DB.Number(PurchasesReportID)
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetPurchases(ByVal DB As Database, ByVal PurchasesReportID As Integer) As DataTable
            Dim sql As String = "select * from PurchasesReportVendorPO where PurchasesReportID=" & DB.Number(PurchasesReportID) & " order by VendorID,PONumber,PODate"
            Return DB.GetDataTable(sql)
        End Function

        'Public Shared Function GetDiscrepancyReport(ByVal DB As Database, ByVal BuilderID As Integer, ByVal PeriodQuarter As Integer, ByVal PeriodYear As Integer) As DataTable
        '    Dim sql As String = _
        '        "select pr.PurchasesReportID, v.VendorID, v.CompanyName as VendorCompany, pvt.TotalAmount as BuilderTotal, srt.TotalAmount as VendorTotal, srt.SalesReportID, srd.SalesReportDisputeID, srd.DisputeResponseID, srd.DisputeResponseReasonID, srd.ResolutionAmount, srd.BuilderComments, srd.VendorComments" _
        '      & " from PurchasesReport pr inner join PurchasesReportVendorTotalAmount pvt on pr.PurchasesReportID=pvt.PurchasesReportID" _
        '      & "   inner join Vendor v on pvt.VendorId=v.VendorID" _
        '      & "   left outer join (select * from SalesReport where PeriodYear=" & DB.Number(PeriodYear) & " and PeriodQuarter=" & DB.Number(PeriodQuarter) & ") as sr on sr.VendorID=v.VendorID" _
        '      & "   left outer join (select * from SalesReportBuilderTotalAmount where BuilderID=" & DB.Number(BuilderID) & ") as srt on srt.SalesReportID=sr.SalesReportID" _
        '      & "   left outer join (select * from SalesReportDispute where BuilderID=" & DB.Number(BuilderID) & ") as srd on sr.SalesReportID=srd.SalesReportID" _
        '      & " where " _
        '      & "   pr.BuilderID=" & DB.Number(BuilderID) _
        '      & " and " _
        '      & "   pvt.TotalAmount <> srt.TotalAmount" _
        '      & " and " _
        '      & "   pr.PeriodQuarter = " & DB.Number(PeriodQuarter) _
        '      & " and " _
        '      & "   pr.PeriodYear = " & DB.Number(PeriodYear)

        '    Return DB.GetDataTable(sql)
        'End Function

        Public Shared Function GetDiscrepancyReport(ByVal DB As Database, ByVal BuilderID As Integer, ByVal PeriodQuarter As Integer, ByVal PeriodYear As Integer)
            Dim sql As String = _
                  " select ptmp.PurchasesReportID,ptmp.Modified,coalesce(ptmp.TotalAmount,0) as BuilderTotal, v.VendorID, v.CompanyName as VendorCompany, coalesce(stmp.VendorTotal,0) as VendorTotal, stmp.SalesReportID, stmp.SalesReportDisputeID, stmp.DisputeResponseID, stmp.DisputeResponseReasonID, stmp.ResolutionAmount,stmp.VendorTotalAmount,stmp.BuilderTotalAmount ,stmp.BuilderComments, stmp.VendorComments,stmp.CreatorVendorAccountId" _
                & " from " _
                & " Vendor v left outer join" _
                & " (select pr.PurchasesReportID, pvt.TotalAmount,pvt.VendorID,pvt.Modified from PurchasesReport pr inner join PurchasesReportVendorTotalAmount pvt on pr.PurchasesReportID=pvt.PurchasesReportID where pr.BuilderID=" & DB.Number(BuilderID) & " and pr.PeriodQuarter=" & DB.Number(PeriodQuarter) & " and pr.PeriodYear=" & DB.Number(PeriodYear) & ") as ptmp" _
                & "     on v.VendorID = ptmp.VendorID" _
                & " full outer join " _
                & " (select sr.VendorID, srt.TotalAmount as VendorTotal, sr.SalesReportID, srd.SalesReportDisputeID, srd.DisputeResponseID, srd.DisputeResponseReasonID, srd.ResolutionAmount, srd.BuilderComments, srd.VendorComments,srd.VendorTotalAmount,srd.BuilderTotalAmount,srt.CreatorVendorAccountId from " _
                & "         (select * from SalesReport where PeriodYear=" & DB.Number(PeriodYear) & " and PeriodQuarter=" & DB.Number(PeriodQuarter) & ") as sr" _
                & "     left outer join " _
                & "         (select * from SalesReportBuilderTotalAmount where BuilderID=" & DB.Number(BuilderID) & ") as srt on srt.SalesReportID=sr.SalesReportID" _
                & "     left outer join " _
                & "         (select * from SalesReportDispute where BuilderID=" & DB.Number(BuilderID) & ") as srd on sr.SalesReportID=srd.SalesReportID" _
                & " ) as stmp" _
                & " on v.VendorID = stmp.VendorID" _
                & " where " _
                & "  (ptmp.Modified is not null or coalesce(ptmp.TotalAmount,0) <> coalesce(stmp.VendorTotal,0) or stmp.SalesReportDisputeID is not null)" _
                & " and (ptmp.VendorID is not null or stmp.VendorID is not null)  and coalesce(ptmp.TotalAmount,0) <> coalesce(stmp.VendorTotal,0)"

            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetBuilderReports(ByVal DB As Database, ByVal BuilderID As Integer, Optional ByVal IncludeCurrent As Boolean = False, Optional ByVal IncludeCurrentReportingPeriod As Boolean = True) As DataTable
            Dim currentQtr As Integer = Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
            Dim lastQtr As Integer = IIf(currentQtr = 1, 4, currentQtr - 1)
            Dim lastYear As Integer = IIf(lastQtr = 4, Now.Year - 1, Now.Year)
            Dim lastQtrEnd As DateTime = (lastQtr * 3) & "/" & Date.DaysInMonth(lastYear, lastQtr * 3) & "/" & lastYear
            Dim sql As String = "select pr.*, (select sum(coalesce(TotalAmount,0)) from PurchasesReportVendorTotalAmount where PurchasesReportID=pr.PurchasesReportID) as TotalAmount from PurchasesReport pr where pr.BuilderID=" & DB.Number(BuilderID)
            If Not IncludeCurrent Then
                sql &= " and (PeriodYear < " & IIf(IncludeCurrentReportingPeriod, lastYear, Now.Year) & " or (PeriodYear = " & IIf(IncludeCurrentReportingPeriod, lastYear, Now.Year) & " and PeriodQuarter < " & IIf(IncludeCurrentReportingPeriod, lastQtr, Math.Ceiling(Now.Month / 3)) & "))"
            End If
            sql &= " Order by PeriodYear Desc, PeriodQuarter Desc"
            Return DB.GetDataTable(sql)
        End Function

        Public Function GetReportedPurchases(VendorId As Integer, Optional ByVal DisplayDNR As Boolean = True) As Object
            Dim FromSQL As String = "  FROM PurchasesReportVendorTotalAmount WHERE PurchasesReportID = " & PurchasesReportID & " AND VendorID = " & VendorId
            Dim Val As Object = DB.ExecuteScalar("SELECT TOP 1 TotalAmount " & FromSQL)

            If DB.ExecuteScalar("SELECT COUNT(*) " & FromSQL) = 0 OrElse IsDBNull(Val) Then
                'If the vendor doesn't have an entry in PurchasesReportVendorTotalAmount, the builder never reported this purchase - DNR
                If DisplayDNR Then
                    Return "DNR"
                Else
                    Return FormatCurrency(0.0)
                End If

            Else
                Return FormatCurrency(Val)
            End If
        End Function
    End Class

    Public MustInherit Class PurchasesReportRowBase
        Private m_DB As Database
        Private m_PurchasesReportID As Integer = Nothing
        Private m_BuilderID As Integer = Nothing
        Private m_PeriodYear As Integer = Nothing
        Private m_PeriodQuarter As Integer = Nothing
        Private m_Created As DateTime = Nothing
        Private m_CreatorBuilderAccountID As Integer = Nothing
        Private m_Submitted As DateTime = Nothing
        Private m_SubmitterBuilderAccountID As Integer = Nothing

        Public Property PurchasesReportID() As Integer
            Get
                Return m_PurchasesReportID
            End Get
            Set(ByVal Value As Integer)
                m_PurchasesReportID = Value
            End Set
        End Property

        Public Property BuilderID() As Integer
            Get
                Return m_BuilderID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderID = Value
            End Set
        End Property

        Public Property PeriodYear() As Integer
            Get
                Return m_PeriodYear
            End Get
            Set(ByVal Value As Integer)
                m_PeriodYear = Value
            End Set
        End Property

        Public Property PeriodQuarter() As Integer
            Get
                Return m_PeriodQuarter
            End Get
            Set(ByVal Value As Integer)
                m_PeriodQuarter = Value
            End Set
        End Property

        Public ReadOnly Property Created() As DateTime
            Get
                Return m_Created
            End Get
        End Property

        Public Property CreatorBuilderAccountID() As Integer
            Get
                Return m_CreatorBuilderAccountID
            End Get
            Set(ByVal Value As Integer)
                m_CreatorBuilderAccountID = Value
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

        Public Property SubmitterBuilderAccountID() As Integer
            Get
                Return m_SubmitterBuilderAccountID
            End Get
            Set(ByVal value As Integer)
                m_SubmitterBuilderAccountID = value
            End Set
        End Property

        Public Property DB() As Database
            Get
                DB = m_DB
            End Get
            Set(ByVal Value As Database)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal PurchasesReportID As Integer)
            m_DB = DB
            m_PurchasesReportID = PurchasesReportID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM PurchasesReport WHERE PurchasesReportID = " & DB.Number(PurchasesReportID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_PurchasesReportID = Convert.ToInt32(r.Item("PurchasesReportID"))
            m_BuilderID = Convert.ToInt32(r.Item("BuilderID"))
            m_PeriodYear = Convert.ToInt32(r.Item("PeriodYear"))
            m_PeriodQuarter = Convert.ToInt32(r.Item("PeriodQuarter"))
            m_Created = Convert.ToDateTime(r.Item("Created"))
            m_CreatorBuilderAccountID = Convert.ToInt32(r.Item("CreatorBuilderAccountID"))
            If IsDBNull(r.Item("Submitted")) Then
                m_Submitted = Nothing
            Else
                m_Submitted = Convert.ToDateTime(r.Item("Submitted"))
            End If
            If IsDBNull(r.Item("SubmitterBuilderAccountID")) Then
                m_SubmitterBuilderAccountID = Nothing
            Else
                m_SubmitterBuilderAccountID = Convert.ToInt32(r.Item("SubmitterBuilderAccountID"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO PurchasesReport (" _
             & " BuilderID" _
             & ",PeriodYear" _
             & ",PeriodQuarter" _
             & ",Created" _
             & ",CreatorBuilderAccountID" _
             & ",Submitted" _
             & ",SubmitterBuilderAccountID" _
             & ") VALUES (" _
             & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.Number(PeriodYear) _
             & "," & m_DB.Number(PeriodQuarter) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(CreatorBuilderAccountID) _
             & "," & m_DB.Quote(Submitted) _
             & "," & m_DB.Number(SubmitterBuilderAccountID) _
             & ")"

            PurchasesReportID = m_DB.InsertSQL(SQL)

            Return PurchasesReportID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE PurchasesReport SET " _
             & " BuilderID = " & m_DB.NullNumber(BuilderID) _
             & ",PeriodYear = " & m_DB.Number(PeriodYear) _
             & ",PeriodQuarter = " & m_DB.Number(PeriodQuarter) _
             & ",Submitted = " & m_DB.Quote(Submitted) _
             & ",SubmitterBuilderAccountID = " & m_DB.Number(SubmitterBuilderAccountID) _
             & " WHERE PurchasesReportID = " & m_DB.Quote(PurchasesReportID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM PurchasesReport WHERE PurchasesReportID = " & m_DB.Number(PurchasesReportID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class
	
	Public Class PurchasesReportCollection
		Inherits GenericCollection(Of PurchasesReportRow)
	End Class

End Namespace

