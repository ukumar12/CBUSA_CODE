Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient
Imports System.IO

Partial Class Index
    Inherits AdminPage
    Private Year1InitialVendorAmountTotal As Double = 0
    Private Year2InitialVendorAmountTotal As Double = 0
    Private VarianceTotal As Double = 0
   
     
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BUILDERS")

        gvList.BindList = AddressOf BindList
        Dim i As Integer = 1
        If Not IsPostBack Then

            For i = 1 To 4
                Me.F_StartPeriodQuarter.Items.Insert(i - 1, (New ListItem(i, i)))
                Me.F_ComparePeriodQuarter.Items.Insert(i - 1, (New ListItem(i, i)))
            Next

            For i = 1 To 15
                Me.F_StartPeriodYear.Items.Insert(i - 1, (New ListItem(Now.Year - 15 + i, Now.Year - 15 + i)))
                Me.F_ComparePeriodYear.Items.Insert(i - 1, (New ListItem(Now.Year - 15 + i, Now.Year - 15 + i)))
            Next

            Me.F_StartPeriodQuarter.SelectedValue = ((Now.AddMonths(-6).Month - 1) \ 3) + 1
            Me.F_ComparePeriodQuarter.SelectedValue = ((Now.AddMonths(-3).Month - 1) \ 3) + 1

            Me.F_StartPeriodYear.SelectedValue = Now.AddMonths(-6).Year
            Me.F_ComparePeriodYear.SelectedValue = Now.AddMonths(-3).Year

            BindList()

        End If




    End Sub

    Private Function BuildQuery() As DataTable



        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder
        ViewState("F_pg") = gvList.PageIndex.ToString
        Dim StartYear As Integer = 0
        Dim EndYear As Integer = 0
        Dim StartQuarter As Integer = 0
        Dim EndQuarter As Integer = 0
        Integer.TryParse(F_StartPeriodYear.Text, StartYear)
        Integer.TryParse(F_StartPeriodQuarter.Text, StartQuarter)
        Integer.TryParse(F_ComparePeriodYear.Text, EndYear)
        Integer.TryParse(F_ComparePeriodQuarter.Text, EndQuarter)
        Dim SqlConn As New System.Text.StringBuilder
        SqlConn.Append("With CTE_main as ( " & vbCrLf)
        SqlConn.Append("Select tmp.llc , tmp.PeriodYear, tmp.PeriodQuarter , sum(coalesce(tmp.InitialVendorAmount,0)) as 'totalamount'   from ( " & vbCrLf)
        SqlConn.Append("select  sr.PeriodQuarter , sr.PeriodYear  , (select sum(coalesce(TotalAmount,0)) from SalesReportBuilderTotalAmount where " & vbCrLf)
        SqlConn.Append("SalesReportID=sr.SalesReportID)  as InitialVendorAmount, " & vbCrLf)
        SqlConn.Append("(select top 1 LLC from LLC inner join LLCVendor on LLC.LLCID = LLCVendor .LLCID where sr.VendorID = LLCVendor.VendorID ) as LLC " & vbCrLf)
        SqlConn.Append(" " & vbCrLf)
        SqlConn.Append(" from SalesReport sr        ) tmp   group by tmp.LLC , tmp.PeriodQuarter , tmp.PeriodYear " & vbCrLf)
        SqlConn.Append("), CTE_Year1 as ( " & vbCrLf)
        SqlConn.Append("Select * " & vbCrLf)
        SqlConn.Append("from CTE_main " & vbCrLf)
        SqlConn.Append("where CTE_main.PeriodYear = " & StartYear & " and  CTE_main.PeriodQuarter = " & StartQuarter & vbCrLf)
        SqlConn.Append("),CTE_Year2 as ( " & vbCrLf)
        SqlConn.Append("Select * " & vbCrLf)
        SqlConn.Append("from CTE_main " & vbCrLf)
        SqlConn.Append("where CTE_main.PeriodYear = " & EndYear & " and  CTE_main.PeriodQuarter =  " & EndQuarter & vbCrLf)
        SqlConn.Append(") " & vbCrLf)
        SqlConn.Append("Select l.LLC,CTE_Year1.PeriodYear,Coalesce(CTE_Year1.totalamount,0) as 'Year1InitialVendorAmount',CTE_Year2.PeriodYear,Coalesce(CTE_Year2.totalamount,0) as 'Year2InitialVendorAmount'   , " & vbCrLf)
        SqlConn.Append("(Coalesce(CTE_Year2.totalamount,0) - Coalesce(CTE_Year1.totalamount,0)) as 'Variance' " & vbCrLf)
        SqlConn.Append(" " & vbCrLf)
        SqlConn.Append("from dbo.LLC l " & vbCrLf)
        SqlConn.Append("Left Join CTE_Year1 " & vbCrLf)
        SqlConn.Append("on l.LLC = CTE_Year1.LLC " & vbCrLf)
        SqlConn.Append("Left Join CTE_Year2 " & vbCrLf)
        SqlConn.Append("on l.LLC = CTE_Year2.LLC")


        Return DB.GetDataTable(SqlConn.ToString & " Order By LLC")

    End Function

    Private Sub BindList()
        Dim res As DataTable = BuildQuery()
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub
        If F_StartPeriodYear.Text = "" Or F_StartPeriodQuarter.Text = "" Or F_ComparePeriodYear.Text = "" Or F_ComparePeriodQuarter.Text = "" Then
            AddError("Please enter both Start and End Period Year and Period Quarter")
            Exit Sub
        End If
        gvList.PageIndex = 0
        BindList()
    End Sub
    Private Sub btnExport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExport.Click
        If Not IsValid Then Exit Sub
        If F_StartPeriodYear.Text = "" Or F_StartPeriodQuarter.Text = "" Or F_ComparePeriodYear.Text = "" Or F_ComparePeriodQuarter.Text = "" Then
            AddError("Please enter both Start and End Period Year and Period Quarter")
            Exit Sub
        End If
        ExportReport()

    End Sub
    Public Sub ExportReport()
        gvList.PageSize = 5000
        Dim res As DataTable = BuildQuery()
        Dim Folder As String = "/assets/catalogs/"
        Dim FileName As String = Core.GenerateFileID & ".csv"
        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        sw.WriteLine("LLC ,Initial Vendor Amount (Previous Quarter), Initial Vendor Amount (Current Quarter), Variance ")

        If res.Rows.Count > 0 Then

            For Each row As DataRow In res.Rows


                Dim LLC As String = String.Empty
                If Not IsDBNull(row("LLC")) Then
                    LLC = row("LLC")
                End If
                Dim Year1InitialVendorAmount As String = String.Empty
                If Not IsDBNull(row("Year1InitialVendorAmount")) Then
                    Year1InitialVendorAmountTotal += row("Year1InitialVendorAmount")
                    Year1InitialVendorAmount = FormatCurrency(row("Year1InitialVendorAmount"))
                End If

                Dim Year2InitialVendorAmount As String = String.Empty
                If Not IsDBNull(row("Year2InitialVendorAmount")) Then
                    Year2InitialVendorAmountTotal += row("Year2InitialVendorAmount")
                    Year2InitialVendorAmount = FormatCurrency(row("Year2InitialVendorAmount"))
                End If

                Dim Variance As String = String.Empty
                If Not IsDBNull(row("Variance")) Then
                    VarianceTotal += row("Variance")
                    Variance = FormatCurrency(row("Variance"))
                End If
                sw.WriteLine(Core.QuoteCSV(LLC) & "," & Core.QuoteCSV(Year1InitialVendorAmount) & "," & Core.QuoteCSV(Year2InitialVendorAmount) & "," & Core.QuoteCSV(Variance))
            Next
            sw.WriteLine(Core.QuoteCSV("TOTAL") & "," & Core.QuoteCSV(FormatCurrency(Year1InitialVendorAmountTotal)) & "," & Core.QuoteCSV(FormatCurrency(Year2InitialVendorAmountTotal)) & "," & Core.QuoteCSV(FormatCurrency(VarianceTotal)))
            sw.Flush()
            sw.Close()
            sw.Dispose()
            Response.Redirect(Folder & FileName)
        End If
    End Sub

    Protected Sub gvList_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Year1InitialVendorAmountTotal += e.Row.DataItem("Year1InitialVendorAmount")
            Year2InitialVendorAmountTotal += e.Row.DataItem("Year2InitialVendorAmount")
            VarianceTotal += e.Row.DataItem("Variance")
             
        End If


        If e.Row.RowType = DataControlRowType.Footer Then
            If e.Row.RowType = DataControlRowType.Footer Then
                e.Row.Cells(0).Text = "Total:"
                e.Row.Cells(1).Text = FormatCurrency(Year1InitialVendorAmountTotal)
                e.Row.Cells(2).Text = FormatCurrency(Year2InitialVendorAmountTotal)
                e.Row.Cells(3).Text = FormatCurrency(VarianceTotal)

            End If
        End If
    End Sub
End Class

