Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.IO

Partial Class Index
    Inherits AdminPage

    Private ReadOnly Property CurrentQuarter() As Integer
        Get
            Return Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
        End Get
    End Property

    Private ReadOnly Property CurrentYear() As Integer
        Get
            Return DatePart(DateInterval.Year, Now)
        End Get
    End Property

    Private ReadOnly Property LastQuarter() As Integer
        Get
            Return IIf(CurrentQuarter - 1 = 0, 4, CurrentQuarter - 1)
        End Get
    End Property

    Private ReadOnly Property LastYear() As Integer
        Get
            Return IIf(LastQuarter = 4, CurrentYear - 1, CurrentYear)
        End Get
    End Property

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BUILDERS")

        gvList.BindList = AddressOf BindList

        EnableDisableUpdate0Button()

        If Not IsPostBack Then
            F_HistoricId.Text = Request("F_HistoricId")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "CompanyName"

            PopulateBuilderList()
            PopulateYearQuarterDropdowns()
            PopulateLLLCList()
            BindList()
        Else
            '*********** UPDATE SURVEY VALUE IF REQUESTED ************
            If hdnPerfSurveyID.Value <> "0" AndAlso hdnNewSurveyData.Value <> "" Then
                Dim PerfSurveyId As Int32 = CInt(hdnPerfSurveyID.Value)
                Dim EditedValue As Int32 = CInt(hdnNewSurveyData.Value)
                UpdateSurveyData(PerfSurveyId, EditedValue)
                BindList()
            End If
            '*********************************************************
        End If
    End Sub

    Private Sub EnableDisableUpdate0Button()

        Dim ReportDeadlineDays As Integer = DB.ExecuteScalar("Select Value From SysParam Where Name = 'ReportDeadlineDays'")

        Dim currentQtr As Integer = Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
        Dim lastQtr As Integer = IIf(currentQtr = 1, 4, currentQtr - 1)
        Dim lastYear As Integer = IIf(lastQtr = 4, Now.Year - 1, Now.Year)
        Dim lastQtrEnd As DateTime = (lastQtr * 3) & "/" & Date.DaysInMonth(lastYear, lastQtr * 3) & "/" & lastYear
        Dim deadline As DateTime = lastQtrEnd.AddMonths(1).AddDays(ReportDeadlineDays)

        hdnReportingQuarter.Value = lastQtr
        hdnReportingYear.Value = lastYear

        hdnProjectionQuarter.Value = IIf(lastQtr = 4, 1, lastQtr + 1)
        hdnProjectionYear.Value = IIf(lastQtr = 4, lastYear + 1, lastYear)

        If Now.Date > deadline Then
            btnUpdateHomeStartsTo0.Style.Add("display", "block")
        Else
            btnUpdateHomeStartsTo0.Style.Add("display", "none")
        End If

    End Sub

    Private Sub PopulateBuilderList()

        ddlBuilder.DataSource = BuilderRow.GetList(DB, "CompanyName")
        ddlBuilder.DataTextField = "CompanyName"
        ddlBuilder.DataValueField = "BuilderID"
        ddlBuilder.DataBind()
        ddlBuilder.Items.Insert(0, New ListItem("-- ALL --", ""))

    End Sub

    Private Sub PopulateYearQuarterDropdowns()

        ddlQuarter.Items.Clear()
        ddlQuarter.Items.Insert(0, New ListItem("-- ALL --", ""))
        For i As Integer = 1 To 4
            Dim liQuarter As New ListItem("Q" & i.ToString(), i.ToString())
            ddlQuarter.Items.Add(liQuarter)
        Next

        ddlYear.Items.Clear()
        ddlYear.Items.Insert(0, New ListItem("-- ALL --", ""))
        For i As Integer = 2 To 0 Step -1
            Dim iYear As Integer = DateAdd(DateInterval.Year, (i * -1), Now.Date).Year
            Dim liYear As New ListItem(iYear.ToString(), iYear.ToString())
            ddlYear.Items.Add(liYear)
        Next

    End Sub

    Private Sub PopulateLLLCList()

        ddlLLC.DataSource = LLCRow.GetList(DB, "LLC")
        ddlLLC.DataTextField = "LLC"
        ddlLLC.DataValueField = "LLCID"
        ddlLLC.DataBind()
        ddlLLC.Items.Insert(0, New ListItem("-- ALL --", ""))

    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " Where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "Select TOP " & (gvList.PageIndex + 1) * gvList.PageSize &
                    " PS.PerformanceSurveyId, PS.BuilderId, b.HistoricID, b.CompanyName, 'Q' + CAST(PS.[Quarter] AS VARCHAR) AS [Quarter], PS.[Year], " &
                    "COALESCE((SELECT TOP 1 PerformanceSurveyId FROM PerformanceSurvey ProjectedPS WHERE BuilderId = B.BuilderId AND Projected = 1 AND [Quarter] = PS.[Quarter] AND [Year] = PS.[Year]), 0) AS ProjectionSurveyID, " &
                    "(SELECT TOP 1 SurveyData FROM PerformanceSurvey ProjectedPS WHERE BuilderId = B.BuilderId AND Projected = 1 AND [Quarter] = PS.[Quarter] AND [Year] = PS.[Year]) AS ProjectedValue, " &
                    "PS.Projected, PS.SurveyData, PS.UpdatedOn "
        SQL = " FROM PerformanceSurvey PS LEFT JOIN Builder B On B.BuilderId = PS.BuilderId WHERE PS.Projected = 0 "

        If Not F_HistoricId.Text = String.Empty Then
            Conn = " And "
            SQL = SQL & Conn & "B.HistoricId = " & DB.Quote(F_HistoricId.Text)
        End If

        If Not ddlBuilder.SelectedIndex = 0 Then
            Conn = " And "
            SQL = SQL & Conn & "B.[BuilderID] = " & DB.Quote(ddlBuilder.SelectedValue)
        End If

        If Not ddlQuarter.SelectedIndex = 0 Then
            Conn = " And "
            SQL = SQL & Conn & "PS.[Quarter] = " & DB.Quote(ddlQuarter.SelectedValue)
        End If

        If Not ddlYear.SelectedIndex = 0 Then
            Conn = " And "
            SQL = SQL & Conn & "PS.[Year] = " & DB.Quote(ddlYear.SelectedValue)
        End If

        If Not ddlLLC.SelectedIndex = 0 Then
            Conn = " And "
            SQL = SQL & Conn & "B.[BuilderID] IN (SELECT BuilderID FROM Builder WHERE LLCID = " & DB.Quote(ddlLLC.SelectedValue) & ") "
        End If

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub UpdateSurveyData(ByVal pPerformanceSurveyId As Int32, ByVal pEditedSurveyData As Int32)

        Dim sqlUpdateSurveyData As String = "UPDATE PerformanceSurvey SET SurveyData = " & pEditedSurveyData & ", UpdatedOn = GETDATE() WHERE PerformanceSurveyId = " & pPerformanceSurveyId
        DB.ExecuteSQL(sqlUpdateSurveyData)

    End Sub

    Private Sub UpdateNonReportedHomeStartsTo0()
        Dim sqlUpdateHomeStartsActuals As String = "INSERT INTO PerformanceSurvey (BuilderId, [Quarter], SurveyData, Projected, [Year], CreatedOn, UpdatedOn) " _
                                                    & "SELECT BuilderId, " & Core.ProtectParam(hdnReportingQuarter.Value) & ", " _
                                                    & "COALESCE((SELECT SurveyData FROM PerformanceSurvey WHERE BuilderID = NonReportedBuilders.BuilderID AND Projected = 1 AND [Quarter] = " & Core.ProtectParam(hdnReportingQuarter.Value) & " AND [Year] = " & Core.ProtectParam(hdnReportingYear.Value) & "), 0), " _
                                                    & "0, " & Core.ProtectParam(hdnReportingYear.Value) & ", GETDATE(), GETDATE() FROM " _
                                                    & "(SELECT DISTINCT BuilderId FROM Builder WHERE IsActive = 1 And BuilderId Not IN (SELECT DISTINCT BuilderId FROM PerformanceSurvey WHERE [Year] = " & Core.ProtectParam(hdnReportingYear.Value) & " AND [Quarter] = " & Core.ProtectParam(hdnReportingQuarter.Value) & " AND Projected = 0)) NonReportedBuilders"
        DB.ExecuteSQL(sqlUpdateHomeStartsActuals)

        Dim sqlUpdateHomeStartsProjection As String = "INSERT INTO PerformanceSurvey (BuilderId, [Quarter], SurveyData, Projected, [Year], CreatedOn, UpdatedOn) " _
                                                     & "SELECT BuilderId, " & Core.ProtectParam(hdnProjectionQuarter.Value) & ", 0, 1, " & Core.ProtectParam(hdnProjectionYear.Value) & ", GETDATE(), GETDATE() FROM " _
                                                     & "(SELECT DISTINCT BuilderId FROM Builder WHERE IsActive = 1 And BuilderId Not IN (SELECT DISTINCT BuilderId FROM PerformanceSurvey WHERE [Year] = " & Core.ProtectParam(hdnReportingYear.Value) & " AND [Quarter] = " & Core.ProtectParam(hdnReportingQuarter.Value) & ")) NonReportedBuilders"
        DB.ExecuteSQL(sqlUpdateHomeStartsProjection)

    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub

    Private Sub gvList_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvList.RowDataBound

        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim rowTxtEditSurveyData As TextBox = e.Row.FindControl("txtEditSurveyData")
            Dim rowTxtEditProjectionData As TextBox = e.Row.FindControl("txtEditProjectionData")

            Dim rowLnkUpdate As HyperLink = e.Row.FindControl("lnkUpdate")
            Dim rowLnkUpdateProjection As HyperLink = e.Row.FindControl("lnkUpdateProjection")

            rowLnkUpdate.Attributes.Add("onclick", "return EditPerformanceSurveyData('" & rowTxtEditSurveyData.ClientID & "','" & e.Row.DataItem("PerformanceSurveyId") & "')")
            rowLnkUpdateProjection.Attributes.Add("onclick", "return EditPerformanceSurveyData('" & rowTxtEditProjectionData.ClientID & "','" & e.Row.DataItem("ProjectionSurveyID") & "')")

            If e.Row.DataItem("ProjectionSurveyID") = 0 Then
                rowTxtEditProjectionData.Visible = False
                rowLnkUpdateProjection.Visible = False
            End If
        End If

    End Sub

    Private Sub btnDummySave_Click(sender As Object, e As EventArgs) Handles btnDummySave.Click

    End Sub

    Private Sub btnUpdateHomeStartsTo0_Click(sender As Object, e As EventArgs) Handles btnUpdateHomeStartsTo0.Click
        UpdateNonReportedHomeStartsTo0()
    End Sub

End Class
