Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient
Imports System.IO

Partial Class Index
    Inherits AdminPage

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
            F_LLC.DataSource = LLCRow.GetList(DB, "LLC")
            F_LLC.DataTextField = "LLC"
            F_LLC.DataValueField = "LLCID"
            F_LLC.DataBind()
            F_VendorID.DataSource = VendorRow.GetList(DB, "CompanyName")
            F_VendorID.DataValueField = "VendorID"
            F_VendorID.DataTextField = "CompanyName"
            F_VendorID.DataBind()
            F_VendorID.Items.Insert(0, New ListItem("-- ALL --", ""))
            F_VendorID.SelectedValue = Request("F_VendorID")
            If gvList.SortBy = String.Empty Then
                gvList.SortBy = " LLC "
                gvList.SortOrder = "ASC"
            End If

        End If
    End Sub

    Private Function BuildQuery() As DataTable

        Dim Conn As String = " where "


        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder
        ViewState("F_pg") = gvList.PageIndex.ToString
        Dim StartYear As Integer = 0
        Dim EndYear As Integer = 0
        Dim StartQuarter As Integer = 0
        Dim EndQuarter As Integer = 0
        Integer.TryParse(F_StartPeriodYear.SelectedValue, StartYear)
        Integer.TryParse(F_StartPeriodQuarter.SelectedValue, StartQuarter)
        Integer.TryParse(F_ComparePeriodYear.SelectedValue, EndYear)
        Integer.TryParse(F_ComparePeriodQuarter.SelectedValue, EndQuarter)
        Dim SqlConn As New System.Text.StringBuilder

        SqlConn.Append("With CTE_main as ( " & vbCrLf)
        SqlConn.Append(" Select tmp.VendorID ,   tmp.PeriodYear, tmp.PeriodQuarter,tmp.salesreportid, " & vbCrLf)
        SqlConn.Append("  (select top 1 LLC from LLC inner join LLCVendor on LLC.LLCID = LLCVendor .LLCID where tmp .VendorID = LLCVendor.VendorID ) as LLC , " & vbCrLf)
        SqlConn.Append("   " & vbCrLf)
        SqlConn.Append("  sum(coalesce(tmp.InitialVendorAmount,0)) as 'totalamount' " & vbCrLf)
        SqlConn.Append(" from ( " & vbCrLf)
        SqlConn.Append("select  sr.VendorID , sr.PeriodQuarter ,  sr.PeriodYear, sr.salesreportid,  (select sum(coalesce(TotalAmount,0)) from SalesReportBuilderTotalAmount where " & vbCrLf)
        SqlConn.Append("SalesReportID=sr.SalesReportID)  as InitialVendorAmount " & vbCrLf)
        SqlConn.Append("  from SalesReport sr ) tmp   group by tmp.VendorID , tmp.PeriodQuarter , tmp.PeriodYear ,tmp.SalesReportID " & vbCrLf)
        SqlConn.Append("), CTE_Year1 as ( " & vbCrLf)
        SqlConn.Append("Select * " & vbCrLf)
        SqlConn.Append("from CTE_main " & vbCrLf)
        SqlConn.Append("where CTE_main.PeriodYear = " & StartYear & " and  CTE_main.PeriodQuarter = " & StartQuarter & vbCrLf)
        SqlConn.Append("),CTE_Year2 as ( " & vbCrLf)
        SqlConn.Append("Select * " & vbCrLf)
        SqlConn.Append("from CTE_main " & vbCrLf)
        SqlConn.Append("where CTE_main.PeriodYear = " & EndYear & " and  CTE_main.PeriodQuarter =  " & EndQuarter & vbCrLf)
        SqlConn.Append(") " & vbCrLf)
        SqlConn.Append("Select (select top 1 LLC from LLC inner join LLCVendor on LLC.LLCID = LLCVendor .LLCID where v.VendorID = LLCVendor.VendorID ) as LLC ,cte_year1.SalesReportID as Year1SalesReportID,cte_year2.SalesReportID as Year2SalesReportID ,  v.VendorID , v.CompanyName ,v.Phone as CompanyPhone , v.Email as CompanyEmail     ,CTE_Year1.PeriodYear,Coalesce(CTE_Year1.totalamount,0) as 'Year1InitialVendorAmount',CTE_Year2.PeriodYear,Coalesce(CTE_Year2.totalamount,0) as 'Year2InitialVendorAmount'   , " & vbCrLf)
        SqlConn.Append("(Coalesce(CTE_Year2.totalamount,0) - Coalesce(CTE_Year1.totalamount,0)) as 'Variance' " & vbCrLf)
        SqlConn.Append("  " & vbCrLf)
        SqlConn.Append("from dbo.Vendor  V " & vbCrLf)
        SqlConn.Append("Left Join CTE_Year1 " & vbCrLf)
        SqlConn.Append("on v.VendorID  = CTE_Year1.VendorID " & vbCrLf)
        SqlConn.Append("Left Join CTE_Year2 " & vbCrLf)
        SqlConn.Append("on v.VendorID = CTE_Year2.VendorID")
        SqlConn.Append(Conn & "v.Isactive  = 1 ")
        Conn = " AND "
        If Not F_LLC.SelectedValues = String.Empty Then
            SqlConn.Append(Conn & " V.VendorID in (select VendorID from LLCVendor where LLCID in " & DB.NumberMultiple(F_LLC.SelectedValues) & ")")
            Conn = " AND "
        End If
        If Not F_VendorID.SelectedValue = String.Empty Then
            SqlConn.Append(Conn & "v.VendorID = " & DB.Quote(F_VendorID.SelectedValue))
            Conn = " AND "
        End If



        Return DB.GetDataTable(SqlConn.ToString & " ORDER BY " & gvList.SortByAndOrder)

    End Function

    Private Sub BindList()
        Dim res As DataTable = BuildQuery()
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub
        If F_StartPeriodYear.SelectedValue = "" Or F_StartPeriodQuarter.SelectedValue = "" Or F_ComparePeriodYear.SelectedValue = "" Or F_ComparePeriodQuarter.SelectedValue = "" Then
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
        sw.WriteLine("LLC ,Vendor ,Initial Vendor Amount (Previous Quarter), Initial Vendor Amount (Current Quarter), Variance, AP-ContactName,AP-Phone, AP-Email, VendorInfo ")

        If res.Rows.Count > 0 Then

            For Each row As DataRow In res.Rows


                Dim LLC As String = String.Empty
                If Not IsDBNull(row("LLC")) Then
                    LLC = row("LLC")
                End If

                Dim Vendor As String = String.Empty
                If Not IsDBNull(row("CompanyName")) Then
                    Vendor = row("CompanyName")
                End If

                Dim Year1InitialVendorAmount As String = String.Empty
                If Not IsDBNull(row("Year1SalesReportID")) Then
                    Year1InitialVendorAmount = GetFinalSalesReportAmount(DB, row("Year1SalesReportID"), row("Year1InitialVendorAmount"))
                Else
                    Year1InitialVendorAmount = "DNR"
                End If

                Dim Year2InitialVendorAmount As String = String.Empty

                If Not IsDBNull(row("Year2SalesReportID")) Then
                    Year2InitialVendorAmount = GetFinalSalesReportAmount(DB, row("Year2SalesReportID"), row("Year2InitialVendorAmount"))
                Else
                    Year2InitialVendorAmount = "DNR"
                End If

                Dim Variance As String = String.Empty
                If Not IsDBNull(row("Variance")) Then
                    Variance = FormatCurrency(row("Variance"))
                End If

                Dim APcontactName As String = String.Empty
                Dim APcontactPhone As String = String.Empty
                Dim APcontactEmail As String = String.Empty
                Dim VendorInformation As String = String.Empty

                Dim dtVendorAccount As DataTable = GetAllVendorUserRoles(DB, row("VendorID"))

                For Each dr As DataRow In dtVendorAccount.Rows
                    If Not IsDBNull(dr("Phone")) Then
                        APcontactPhone = dr("Phone")
                    End If
                    If Not IsDBNull(dr("Email")) Then
                        APcontactEmail = dr("Email")
                    End If
                    If Not IsDBNull(dr("FirstName")) Then
                        APcontactName = Core.BuildFullName(IIf(IsDBNull(dr("FirstName")), String.Empty, dr("FirstName")), " ", IIf(IsDBNull(dr("LastName")), String.Empty, dr("LastName")))
                    End If
                Next


                VendorInformation = Core.BuildFullName(IIf(IsDBNull(row("CompanyPhone")), String.Empty, row("CompanyPhone")), " ", IIf(IsDBNull(row("CompanyEmail")), String.Empty, row("CompanyEmail")))
                sw.WriteLine(Core.QuoteCSV(LLC) & "," & Core.QuoteCSV(Vendor) & "," & Core.QuoteCSV(Year1InitialVendorAmount) & "," & Core.QuoteCSV(Year2InitialVendorAmount) & "," & Core.QuoteCSV(Variance) & "," & Core.QuoteCSV(APcontactName) & "," & Core.QuoteCSV(APcontactPhone) & "," & Core.QuoteCSV(APcontactEmail) & "," & Core.QuoteCSV(VendorInformation))
            Next
            sw.Flush()
            sw.Close()
            sw.Dispose()
            Response.Redirect(Folder & FileName)
        End If
    End Sub

    Protected Sub gvList_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If
        Dim ltlYear1InitialVendorAmount As Literal = e.Row.FindControl("ltlYear1InitialVendorAmount")
        Dim ltlYear2InitialVendorAmount As Literal = e.Row.FindControl("ltlYear2InitialVendorAmount")
        If Not IsDBNull(e.Row.DataItem("Year1SalesReportID")) Then
            ltlYear1InitialVendorAmount.Text = GetFinalSalesReportAmount(DB, e.Row.DataItem("Year1SalesReportID"), e.Row.DataItem("Year1InitialVendorAmount"))
        Else
            ltlYear1InitialVendorAmount.Text = "DNR"
        End If


        If Not IsDBNull(e.Row.DataItem("Year2SalesReportID")) Then
            ltlYear2InitialVendorAmount.Text = GetFinalSalesReportAmount(DB, e.Row.DataItem("Year2SalesReportID"), e.Row.DataItem("Year2InitialVendorAmount"))
        Else
            ltlYear2InitialVendorAmount.Text = "DNR"
        End If

        Dim ltlContactName As Literal = e.Row.FindControl("ltlContactName")
        Dim ltlPhone As Literal = e.Row.FindControl("ltlPhone")
        Dim ltlEmail As Literal = e.Row.FindControl("ltlEmail")
        Dim ltlCompanyInfo As Literal = e.Row.FindControl("ltlCompanyInfo")
        Dim ltlContactPhone As Literal = e.Row.FindControl("ltlContactPhone")

        Dim dtVendorAccount As DataTable = GetAllVendorUserRoles(DB, e.Row.DataItem("VendorID"))

        For Each row As DataRow In dtVendorAccount.Rows
            If Not IsDBNull(row("Phone")) Then
                ltlContactPhone.Text = row("Phone")
            End If
            If Not IsDBNull(row("Email")) Then
                ltlEmail.Text = row("Email")
            End If
            If Not IsDBNull(row("FirstName")) Then
                ltlContactName.Text = Core.BuildFullName(IIf(IsDBNull(row("FirstName")), String.Empty, row("FirstName")), " ", IIf(IsDBNull(row("LastName")), String.Empty, row("LastName")))
            End If
        Next

        ltlCompanyInfo.Text = Core.BuildFullName(IIf(IsDBNull(e.Row.DataItem("CompanyPhone")), String.Empty, e.Row.DataItem("CompanyPhone")), " ", IIf(IsDBNull(e.Row.DataItem("CompanyEmail")), String.Empty, e.Row.DataItem("CompanyEmail")))

    End Sub
    Private Function GetAllVendorUserRoles(ByVal DB As Database, ByVal VendorID As Integer) As DataTable
        Dim sql As String = _
              " select a.FirstName, a.LastName , a.Email , a.Phone , v.VendorRole, v.VendorRoleID " _
            & " from VendorAccount a inner join VendorAccountVendorRole r on a.VendorAccountID=r.VendorAccountID " _
            & "     inner join VendorRole v on r.VendorRoleID=v.VendorRoleID " _
            & " where a.VendorID = " & DB.Number(VendorID) _
            & "  and  r.VendorRoleID = 2 order by r.VendorRoleID desc "

        Return DB.GetDataTable(sql)
    End Function
    Private Function GetFinalSalesReportAmount(ByVal Db As Database, ByVal SalesreportID As Integer, ByVal InitialVendorAmount As Double) As Object
        Dim FinalSalesReportAmount As String = String.Empty
        Dim FromSQL As String = " FROM SalesReportBuilderTotalAmount WHERE SalesReportID = " & SalesreportID
        Dim Val As Integer = Core.GetInt(Db.ExecuteScalar("SELECT Count(TotalAmount) " & FromSQL))
        If Val <= 0 Then
            Dim Sql As String = "Select Submitted From SalesReport where SalesreportID = " & SalesreportID
            If IsDBNull(Db.ExecuteScalar(Sql)) Then
                FinalSalesReportAmount = "DNR"
            Else
                FinalSalesReportAmount = "$0.00"
            End If
        Else
            FinalSalesReportAmount = FormatCurrency(InitialVendorAmount, 2)
        End If
        Return FinalSalesReportAmount
    End Function

End Class

