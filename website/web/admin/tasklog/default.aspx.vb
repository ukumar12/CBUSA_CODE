Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager
Imports System.IO

Partial Class Index
    Inherits AdminPage
    Private IndexDirectoryRoot As String = AppSettings("SearchIndexDirectory")
    Private IndexDirectory As String = AppSettings("SearchIndexDirectory") & "\live\cbusa_app\"
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("TASK_LOGS")

        Dim dtPrimary As DateTime = Nothing
        Try
            If File.Exists(IndexDirectory & "reindexed.txt") Then
                dtPrimary = FormatDateTime(File.ReadAllText(IndexDirectory & "reindexed.txt"), DateFormat.GeneralDate)
            End If
        Catch ex As Exception
        End Try

        ltlIdevIndex.Text = "iDev Search Index Updated On: <b>" & IIf(dtPrimary <> Nothing, dtPrimary.ToString, "n/a") & "</b><br>" & IndexDirectory

        Dim IdevSearchTriggerDateTime As DateTime = SysParam.GetValue(DB, "IdevSearchTriggerDateTime")
        ltlIdevSearchTrigger.Text = "iDev Search Index Update Triggered On : <b>" & IdevSearchTriggerDateTime

        If dtPrimary < IdevSearchTriggerDateTime Then

            btnUpdateIdevSearch.Enabled = False
            btnUpdateIdevSearch.Text = "Processing ..."

        End If

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then

            F_TaskName.SelectedValue = Request("F_TaskName")
            F_Status.SelectedValue = Request("F_Status")
            F_LogDateLbound.Text = Request("F_LogDateLBound")
            F_LogDateUbound.Text = Request("F_LogDateUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            gvList.PageIndex = IIf(Request("F_pg") = String.Empty, 0, Core.ProtectParam(Request("F_pg")))

            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "LogDate"
                gvList.SortOrder = "DESC"
            End If



            LoadReportingParameters()
            BindList()
        End If
    End Sub

    Private Sub LoadReportingParameters()
        Dim ReportDeadlineDays As Integer = DB.ExecuteScalar("Select Value From SysParam Where Name = 'ReportDeadlineDays'")
        Dim DiscrepancyDeadlineDays As Integer = DB.ExecuteScalar("Select Value From SysParam Where Name = 'DiscrepancyDeadlineDays'")
        Dim DiscrepancyResponseDeadlineDays As Integer = DB.ExecuteScalar("Select Value From SysParam Where Name = 'DiscrepancyResponseDeadlineDays'")
        Dim ActivateAutoDisputesTask As Integer = DB.ExecuteScalar("Select Value From SysParam Where Name = 'ActivateAutoDisputesTask'")
        Dim currentQtr As Integer = Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
        Dim lastQtr As Integer = IIf(currentQtr = 1, 4, currentQtr - 1)
        Dim lastYear As Integer = IIf(lastQtr = 4, Now.Year - 1, Now.Year)
        Dim lastQtrEnd As DateTime = (lastQtr * 3) & "/" & Date.DaysInMonth(lastYear, lastQtr * 3) & "/" & lastYear
        Dim deadline As DateTime = lastQtrEnd.AddMonths(1).AddDays(ReportDeadlineDays)

        ltlQtrEndDate.Text = "Q" & lastQtr & " " & lastYear & " (" & lastQtrEnd & ")"
        txtReportingDeadlineDays.Text = ReportDeadlineDays
        ltlReportingDeadline.Text = deadline
        txtDiscrepancyDeadlineDays.Text = DiscrepancyDeadlineDays
        ltlDiscrepancyDeadline.Text = deadline.AddDays(DiscrepancyDeadlineDays)
        txtDisputeDeadlineDays.Text = DiscrepancyResponseDeadlineDays
        ltlDisputeDeadline.Text = deadline.AddDays(DiscrepancyDeadlineDays + DiscrepancyResponseDeadlineDays)
        drpActivateAutoDisputes.SelectedValue = ActivateAutoDisputesTask


    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder
        ViewState("F_pg") = gvList.PageIndex.ToString

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM TaskLog  "

        If Not F_TaskName.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "TaskName = " & DB.Quote(F_TaskName.SelectedValue)
            Conn = " AND "
        End If
        If Not F_Status.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "Status = " & DB.Quote(F_Status.SelectedValue)
            Conn = " AND "
        End If
        If Not F_LogDateLBound.Text = String.Empty Then
            SQL = SQL & Conn & "LogDate >= " & DB.Quote(F_LogDateLBound.Text)
            Conn = " AND "
        End If
        If Not F_LogDateUBound.Text = String.Empty Then
            SQL = SQL & Conn & "LogDate < " & DB.Quote(DateAdd("d", 1, F_LogDateUBound.Text))
            Conn = " AND "
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()

    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        gvList.PageIndex = 0
        BindList()
    End Sub


    Protected Sub btnUpdateReporting_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateReporting.Click
        If Not Page.IsValid Then Exit Sub

        Try

            Dim Sql As String = "Update SysParam Set Value = " & DB.Number(txtReportingDeadlineDays.Text) & " Where Name = 'ReportDeadlineDays'"
            DB.ExecuteSQL(SQL)

            Sql = "Update SysParam Set Value = " & DB.Number(txtDiscrepancyDeadlineDays.Text) & " Where Name = 'DiscrepancyDeadlineDays'"
            DB.ExecuteSQL(Sql)

            Sql = "Update SysParam Set Value = " & DB.Number(txtDisputeDeadlineDays.Text) & " Where Name = 'DiscrepancyResponseDeadlineDays'"
            DB.ExecuteSQL(Sql)

            Sql = "Update SysParam Set Value = " & DB.Number(drpActivateAutoDisputes.SelectedValue) & " Where Name = 'ActivateAutoDisputesTask'"
            DB.ExecuteSQL(Sql)

            Response.Redirect("default.aspx")

        Catch ex As Exception
            AddError("There was an error. Please try again.")
        End Try

    End Sub


    Protected Sub btnUpdateIdevSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateIdevSearch.Click
        If Not Page.IsValid Then Exit Sub

        Try

            Dim Sql As String = "Update SysParam Set Value = '1' Where Name = 'IdevSearchTrigger'"
            DB.ExecuteSQL(Sql)

            Sql = "Update SysParam Set Value = " & DB.NQuote(Now()) & " Where Name = 'IdevSearchTriggerDateTime'"
            DB.ExecuteSQL(Sql)

            Response.Redirect("default.aspx")
        Catch ex As Exception
            AddError("There was an error. Please try again.")
        End Try


    End Sub


End Class