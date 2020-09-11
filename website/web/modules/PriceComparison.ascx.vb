Option Strict Off

Imports Components
Imports DataLayer
Imports System.Net

Partial Class PriceComparison
    Inherits ModuleControl

    Private Property Updated() As Boolean
        Get
            Return Session("ComparisonsUpdated")
        End Get
        Set(ByVal value As Boolean)
            Session("ComparisonsUpdated") = value
        End Set
    End Property

    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            'If Not Updated Then
            'UpdateComparisons()
            '    Updated = True
            'End If

            BindJobs()

            BindComparisons()
            BindAdmin()

            ShowHideDashboard()
        End If

    End Sub

    Private Sub ShowHideDashboard()

        '-------------- CALCULATE REPORTING END DATE ---------------
        Dim ReportDeadlineDays As Integer = DB.ExecuteScalar("Select Value From SysParam Where Name = 'ReportDeadlineDays'")
        Dim DiscrepancyDeadlineDays As Integer = DB.ExecuteScalar("Select Value From SysParam Where Name = 'DiscrepancyDeadlineDays'")
        Dim DiscrepancyResponseDeadlineDays As Integer = DB.ExecuteScalar("Select Value From SysParam Where Name = 'DiscrepancyResponseDeadlineDays'")

        Dim currentQtr As Integer = Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
        Dim currentYear As Integer = DatePart(DateInterval.Year, Now)
        Dim currQtrEnd As DateTime = (currentQtr * 3) & "/" & Date.DaysInMonth(currentYear, currentQtr * 3) & "/" & currentYear

        Dim lastQtr As Integer = IIf(currentQtr = 1, 4, currentQtr - 1)
        Dim lastYear As Integer = IIf(currentQtr = 1, currentYear - 1, currentYear)
        Dim lastQtrEnd As DateTime = (lastQtr * 3) & "/" & Date.DaysInMonth(lastYear, lastQtr * 3) & "/" & lastYear

        Dim LastQtrReportingStartDate As DateTime = DateAdd(DateInterval.Day, (ReportDeadlineDays + DiscrepancyDeadlineDays + DiscrepancyResponseDeadlineDays), lastQtrEnd)

        If LastQtrReportingStartDate > DateTime.Now.Date Then
            lastYear = IIf(lastQtr = 1, currentYear - 1, currentYear)
            lastQtr = IIf(lastQtr = 1, 4, lastQtr - 1)
            lastQtrEnd = (lastQtr * 3) & "/" & Date.DaysInMonth(lastYear, lastQtr * 3) & "/" & lastYear
        End If

        'Dim ReportingDeadline As DateTime = lastQtrEnd.AddMonths(1).AddDays(ReportDeadlineDays)
        'Dim FinalInvoicingDate As DateTime = ReportingDeadline.AddDays(DiscrepancyDeadlineDays + DiscrepancyResponseDeadlineDays + 1)

        Dim ReportingQuarter As Integer
        Dim ReportingYear As Integer

        ReportingQuarter = lastQtr
        ReportingYear = lastYear

        spnReportingQtrYear.InnerText = String.Concat(ReportingYear.ToString(), " Q", ReportingQuarter.ToString())

        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))

        If dbBuilder.Submitted > lastQtrEnd Then           'Hide dashboard for new Builders
            divBuilderDashboardOuter.Visible = False
        End If

    End Sub

    Private Sub UpdateComparisons()
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
        Dim dtAdmin As DataTable = PriceComparisonRow.GetAdminComparionIds(DB, dbBuilder.LLCID)
        For Each row As DataRow In dtAdmin.Rows
            Dim dbComparison As PriceComparisonRow = PriceComparisonRow.GetRow(DB, row("PriceComparisonId"))
            dbComparison.UpdateAll()
        Next

        Dim dtSaved As DataTable = PriceComparisonRow.GetSavedComparisons(DB, dbBuilder.BuilderID)
        For Each row As DataRow In dtSaved.Rows
            Dim dbComparison As PriceComparisonRow = PriceComparisonRow.GetRow(DB, row("PriceComparisonId"))
            dbComparison.UpdateAll()
        Next
    End Sub

    Private Sub BindComparisons()
        Dim c As PriceComparisonCollection = PriceComparisonRow.GetDashboardComparisons(DB, Session("BuilderId"))
        rptComparisons.DataSource = c
        rptComparisons.DataBind()
    End Sub

    Private Sub BindJobs()
        Dim dt As DataTable = PriceComparisonRow.GetRecentJobs(DB, Session("BuilderID"), 5)
        rptJobs.DataSource = dt
        rptJobs.DataBind()
    End Sub

    Private Sub BindAdmin()
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
        Dim dt As PriceComparisonCollection = PriceComparisonRow.GetAdminComparisons(DB, dbBuilder.LLCID)
        rptAdmin.DataSource = dt
        rptAdmin.DataBind()
    End Sub

    Protected Sub rptComparisons_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptComparisons.ItemCommand
        Select Case e.CommandName
            Case "Remove"
                Dim dbComparison As PriceComparisonRow = PriceComparisonRow.GetRow(DB, e.CommandArgument)
                dbComparison.IsDashboard = False
                If Not dbComparison.IsSaved Then
                    dbComparison.Remove()
                Else
                    dbComparison.Update()
                End If
                BindComparisons()
        End Select
    End Sub

    Protected Sub rptAdmin_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptAdmin.ItemCommand
        Select Case e.CommandName
            Case "Remove"
                Dim dbComparison As PriceComparisonRow = PriceComparisonRow.GetRow(DB, e.CommandArgument)
                dbComparison.IsDashboard = False
                If Not dbComparison.IsSaved Then
                    dbComparison.Remove()
                Else
                    dbComparison.Update()
                End If
                BindAdmin()
        End Select
    End Sub

    Protected Sub rptComparisons_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptComparisons.ItemDataBound
        If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim pc As PriceComparisonRow = e.Item.DataItem
        'pc.UpdateAll()
        Dim dbTakeoff As TakeOffRow = TakeOffRow.GetRow(DB, pc.TakeoffID)
        Dim dtVendors As DataTable = pc.GetTopVendors(3)
        If dtVendors.Rows.Count = 0 Then
            e.Item.Visible = False
            Exit Sub
        End If
        Dim ltlVendorHeaders As Literal = e.Item.FindControl("ltlVendorHeaders")
        Dim ltlTitle As Literal = e.Item.FindControl("ltlTitle")
        Dim ltlVendorPrices As Literal = e.Item.FindControl("ltlVendorPrices")

        ltlTitle.Text = "<a href=""/comparison/default.aspx?PriceComparisonID=" & e.Item.DataItem.PriceComparisonID & """>" & dbTakeoff.Title & "</a>"
        Dim cnt As Integer = 1
        For Each row As DataRow In dtVendors.Rows
            ltlVendorHeaders.Text &= "<th>" & row("CompanyName") & "</th>"
            ltlVendorPrices.Text &= "<td>" & FormatCurrency(row("SubTotal")) & "</td>"
            cnt += 1
        Next
        For i As Integer = cnt To 3 Step 1
            ltlVendorHeaders.Text &= "<th>&nbsp;</th>"
            ltlVendorPrices.Text &= "<td>&nbsp;</td>"
        Next
    End Sub

    Protected Sub rptJobs_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptJobs.ItemDataBound
        If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim ltlProject As Literal = e.Item.FindControl("ltlProject")
        Dim ltlTakeoff As Literal = e.Item.FindControl("ltlTakeoff")
        Dim ltlCreated As Literal = e.Item.FindControl("ltlCreated")
        Dim ltlUpdated As Literal = e.Item.FindControl("ltlUpdated")
        Dim ltlLink As Literal = e.Item.FindControl("ltlLink")


        ltlProject.Text = Core.GetString(e.Item.DataItem("ProjectName"))
        ltlTakeoff.Text = "<a href=""/takeoffs/edit.aspx?TakeoffID=" & Core.GetInt(e.Item.DataItem("TakeoffID")) & """>" & Core.GetString(e.Item.DataItem("Title")) & "</a>"
        ltlCreated.Text = FormatDateTime(e.Item.DataItem("Created"), DateFormat.ShortDate)
        ltlUpdated.Text = FormatDateTime(e.Item.DataItem("Saved"), DateFormat.GeneralDate)


        'Dim dt As DataTable = PriceComparisonRow.GetLatestSavedComparison(DB, Session("BuilderId"), e.Item.DataItem("TakeoffID"))
        Dim dt As DataTable = PriceComparisonRow.GetLastComparison(DB, e.Item.DataItem("TakeoffID"))

        For Each row As DataRow In dt.Rows
            ltlLink.Text &= "<a class=""btnblue"" href=""/comparison/default.aspx?PriceComparisonId=" & row("PriceComparisonID") & """>View Comparison</a>"

        Next
    End Sub

    Protected Sub rptAdmin_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptAdmin.ItemDataBound
        If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim pc As PriceComparisonRow = e.Item.DataItem
        'pc.UpdateAll()
        Dim dbTakeoff As TakeOffRow = TakeOffRow.GetRow(DB, pc.TakeoffID)
        Dim dtVendors As DataTable = pc.GetTopVendors(3)

        Dim ltlVendorHeaders As Literal = e.Item.FindControl("ltlVendorHeaders")
        Dim ltlTitle As Literal = e.Item.FindControl("ltlTitle")
        Dim ltlVendorPrices As Literal = e.Item.FindControl("ltlVendorPrices")

        ltlTitle.Text = "<a href=""/comparison/default.aspx?PriceComparisonID=" & e.Item.DataItem.PriceComparisonID & """>" & dbTakeoff.Title & "</a>"
        Dim cnt As Integer = 1
        For Each row As DataRow In dtVendors.Rows
            ltlVendorHeaders.Text &= "<th>" & row("CompanyName") & "</th>"
            ltlVendorPrices.Text &= "<td>" & FormatCurrency(row("Total")) & "</td>"
            cnt += 1
        Next
        For i As Integer = cnt To 3 Step 1
            ltlVendorHeaders.Text &= "<th>&nbsp;</th>"
            ltlVendorPrices.Text &= "<td>&nbsp;</td>"
        Next
    End Sub

    Protected Sub btnUpdateComp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateComp.Click
        Dim dt As PriceComparisonCollection = PriceComparisonRow.GetDashboardComparisons(DB, Session("BuilderId"))
        For Each dr As PriceComparisonRow In dt
            If DateDiff(DateInterval.Minute, dr.ModifyDate, Now()) > 240 Then
                Try
                    Dim request As WebRequest = WebRequest.Create(ConfigurationManager.AppSettings("GlobalRefererName") & "/comparison/default.aspx?IsUpdate=Y&BuilderId=" & Session("BuilderId") & "&PriceComparisonID=" & dr.PriceComparisonID)
                    request.Method = "GET"
                    request.Timeout = 20 * 1000
                    Dim response As WebResponse = request.GetResponse()
                    response.Close()
                Catch ex As Exception
                    AddError(ex.Message)
                End Try
            End If
        Next
        BindComparisons()
        divProgress.Visible = False
        upComparisons.Update()

        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
        dt = PriceComparisonRow.GetAdminComparisons(DB, dbBuilder.LLCID)
        For Each dr As PriceComparisonRow In dt
            If DateDiff(DateInterval.Minute, dr.ModifyDate, Now()) > 480 Then
                Try
                    Dim request As WebRequest = WebRequest.Create(ConfigurationManager.AppSettings("GlobalRefererName") & "/comparison/default.aspx?IsUpdate=Y&BuilderId=" & Session("BuilderId") & "&PriceComparisonID=" & dr.PriceComparisonID)
                    request.Method = "GET"
                    request.Timeout = 20 * 1000
                    Dim response As WebResponse = request.GetResponse()
                    response.Close()
                Catch ex As Exception
                    AddError(ex.Message)
                End Try
            End If
        Next
        BindAdmin()
        divProgressEO.Visible = False
        upAdmin.Update()

        btnUpdateComp.Visible = False

    End Sub
End Class
