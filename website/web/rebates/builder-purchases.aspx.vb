Imports Components
Imports DataLayer
Imports System.Data
Imports System.Linq
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager
Imports System.IO
Imports System.Net
Partial Class rebates_builder_purchases
    Inherits SitePage

    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private PurchasesReportVendorPOId As String = ""
    Private VendorId As String = ""
    Protected PerformanceSurveyID As Integer

    Protected TotalAmount As Double = 0


    Protected ReadOnly Property PrintUrl() As String
        Get
            Dim Output As String = Request.ServerVariables("URL").ToString.Trim & "?"
            If Not Request.ServerVariables("QUERY_STRING") Is Nothing AndAlso Request.ServerVariables("QUERY_STRING").ToString.Trim <> String.Empty Then
                Dim TempArray As String() = Split(Request.ServerVariables("QUERY_STRING").ToString.Trim, "&")
                For Each Param As String In TempArray
                    Dim TempArray2 As String() = Split(Param.ToString.Trim, "=")
                    If UBound(TempArray2) = 1 Then
                        If TempArray2(0).ToString.Trim <> String.Empty AndAlso TempArray2(0).ToString.ToLower.Trim <> "print" Then
                            If Right(Output, 1) <> "?" Then Output &= "&"
                            Output &= TempArray2(0).ToString.Trim & "=" & TempArray2(1).ToString.Trim
                        End If
                    End If
                Next
                If Right(Output, 1) <> "?" Then Output &= "&"
            End If
            Output &= "print=y&" & GetPageParams(Components.FilterFieldType.All)
            Return Output
        End Get
    End Property

#Region "Properties"
    Protected ReadOnly Property CurrentQuarter() As Integer
        Get
            Return Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
        End Get
    End Property

    Protected ReadOnly Property CurrentYear() As Integer
        Get
            Return DatePart(DateInterval.Year, Now)
        End Get
    End Property

    Protected ReadOnly Property LastQuarter() As Integer
        Get
            'Return 1
            Return IIf(CurrentQuarter - 1 = 0, 4, CurrentQuarter - 1)
        End Get
    End Property

    Protected ReadOnly Property LastYear() As Integer
        Get
            Return IIf(LastQuarter = 4, CurrentYear - 1, CurrentYear)
        End Get
    End Property

    Private m_ResponseDeadline As DateTime = Nothing
    Protected ReadOnly Property ResponseDeadline() As DateTime
        Get
            If m_ResponseDeadline = Nothing Then
                'Dim LastQuarterEnd As DateTime = (LastQuarter * 3) & "/" & Date.DaysInMonth(LastYear, LastQuarter * 3) & "/" & LastYear
                Dim LastQuarterEnd As DateTime = New Date(LastYear, (LastQuarter * 3), Date.DaysInMonth(LastYear, LastQuarter * 3)).Date  ' (LastQuarter * 3) & "/" & Date.DaysInMonth(LastYear, LastQuarter * 3) & "/" & LastYear
                m_ResponseDeadline = GetDeadline(LastQuarterEnd)
            End If
            Return m_ResponseDeadline
        End Get
    End Property

    Protected Property ReportQuarter() As Integer
        Get
            Return ViewState("ReportQuarter")
        End Get
        Set(ByVal value As Integer)
            ViewState("ReportQuarter") = value
            If m_dbPurchasesReport IsNot Nothing AndAlso ViewState("ReportQuarter") <> m_dbPurchasesReport.PeriodQuarter Then
                dbPurchasesReport = Nothing
            End If
        End Set
    End Property

    Protected Property ReportYear() As Integer
        Get
            Return ViewState("ReportYear")
        End Get
        Set(ByVal value As Integer)
            ViewState("ReportYear") = value
            If m_dbPurchasesReport IsNot Nothing AndAlso ViewState("ReportYear") <> m_dbPurchasesReport.PeriodYear Then
                dbPurchasesReport = Nothing
            End If
        End Set
    End Property

    Private m_dbPurchasesReport As PurchasesReportRow
    Private Property dbPurchasesReport() As PurchasesReportRow
        Get
            If m_dbPurchasesReport Is Nothing OrElse (m_dbPurchasesReport.PeriodQuarter <> ReportQuarter Or m_dbPurchasesReport.PeriodYear <> ReportYear) Then
                m_dbPurchasesReport = PurchasesReportRow.GetPurchasesReportByPeriod(DB, Session("BuilderId"), ReportYear, ReportQuarter)
                If m_dbPurchasesReport.Created = Nothing Then
                    m_dbPurchasesReport.BuilderID = Session("BuilderId")
                    m_dbPurchasesReport.PeriodQuarter = ReportQuarter
                    m_dbPurchasesReport.PeriodYear = ReportYear
                    m_dbPurchasesReport.CreatorBuilderAccountID = Session("BuilderAccountID")
                    m_dbPurchasesReport.Insert()
                End If
            End If
            Return m_dbPurchasesReport
        End Get
        Set(ByVal value As PurchasesReportRow)
            m_dbPurchasesReport = value
            m_dtTotals = Nothing
            m_dtPurchases = Nothing
        End Set
    End Property

    Private m_dtTotals As DataTable
    Private Property dtTotals() As DataTable
        Get
            If m_dtTotals Is Nothing Then
                m_dtTotals = PurchasesReportRow.GetVendors(DB, dbPurchasesReport.PurchasesReportID)
            End If
            Return m_dtTotals
        End Get
        Set(ByVal value As DataTable)
            m_dtTotals = value
        End Set
    End Property

    Private m_dtVendors As DataTable
    Private ReadOnly Property dtVendors() As DataTable
        Get
            If m_dtVendors Is Nothing Then
                Dim LastQuarterEnd As DateTime = New Date(LastYear, (LastQuarter * 3), Date.DaysInMonth(LastYear, LastQuarter * 3)).Date  '(LastQuarter * 3) & "/" & Date.DaysInMonth(LastYear, LastQuarter * 3) & "/" & LastYear
                m_dtVendors = BuilderRow.GetAllVendors(DB, Session("BuilderId"), DateAdd(DateInterval.Day, 1, LastQuarterEnd))
            End If
            Return m_dtVendors
        End Get
    End Property

    Private m_dtPurchases As DataTable
    Private m_dvPurchases As DataView
    Private ReadOnly Property dvPurchases() As DataView
        Get
            If m_dvPurchases Is Nothing Or m_dtPurchases Is Nothing Then
                If m_dtPurchases Is Nothing Then
                    m_dtPurchases = PurchasesReportRow.GetPurchases(DB, dbPurchasesReport.PurchasesReportID)
                End If
                m_dvPurchases = m_dtPurchases.DefaultView
                m_dvPurchases.Sort = "VendorID"
            End If
            Return m_dvPurchases
        End Get
    End Property

    Private m_VendorIds As Generic.List(Of Integer)
    Private ReadOnly Property VendorIds() As Generic.List(Of Integer)
        Get
            If m_VendorIds Is Nothing Then
                m_VendorIds = New Generic.List(Of Integer)
                m_VendorIds.AddRange((From row As DataRow In dtVendors.AsEnumerable Select CInt(row("VendorId"))).DefaultIfEmpty)
            End If
            Return m_VendorIds
        End Get
    End Property

    Private Property EditMode() As Boolean
        Get
            Return ViewState("EditMode")
        End Get
        Set(ByVal value As Boolean)
            ViewState("EditMode") = value
        End Set
    End Property

    Private Property EditIndex() As Integer
        Get
            Return ViewState("EditIndex")
        End Get
        Set(ByVal value As Integer)
            ViewState("EditIndex") = value
        End Set
    End Property

    Private Property PurchaseEditID() As Integer
        Get
            Return ViewState("PurchaseEditID")
        End Get
        Set(ByVal value As Integer)
            ViewState("PurchaseEditID") = value
        End Set
    End Property

    Private m_Keys As Generic.List(Of String)
    Public ReadOnly Property Keys() As Generic.List(Of String)
        Get
            If m_Keys Is Nothing Then
                m_Keys = New Generic.List(Of String)
            End If
            Return m_Keys
        End Get
    End Property
#End Region

    Protected Overrides Function SaveViewState() As Object
        Dim base As Object = MyBase.SaveViewState
        Dim mystate As String = String.Empty
        If Keys.Count > 0 Then
            Dim s As New StringBuilder
            For Each k As String In Keys
                s.Append(IIf(s.Length = 0, k, "," & k))
            Next
            mystate = s.ToString
            'mystate = Keys.Aggregate(Function(sum, append) sum & IIf(sum = String.Empty, append, "," & append))
        End If
        Return IIf(base Is Nothing, mystate, New Pair(base, mystate))
    End Function

    Protected Overrides Sub LoadViewState(ByVal savedState As Object)
        Dim mystate As String
        If TypeOf savedState Is Pair Then
            Dim state As Pair = DirectCast(savedState, Pair)
            MyBase.LoadViewState(state.First)
            mystate = DirectCast(state.Second, String)
        Else
            mystate = DirectCast(savedState, String)
        End If
        Keys.Clear()
        For Each key As String In mystate.Split(",")
            Keys.Add(key)
        Next
    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureBuilderAccess()

        If Not Request("print") Is Nothing AndAlso Request("print").ToString.Trim <> String.Empty Then
            pnlPrint.Visible = False
            trLeftHeader.Visible = False
            trLeftColumn.Visible = False
        End If

        Dim sm As ScriptManager = ScriptManager.GetCurrent(Page)

        sm.RegisterAsyncPostBackControl(hdnPostback)

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")
        hdnCurrentQuarter.Value = LastQuarter

        If LastQuarter = 1 Or LastQuarter = 2 Or LastQuarter = 3 Then
            Dim dtSurveyData = DB.ExecuteScalar("SELECT SurveyData FROM PerformanceSurvey WHERE BuilderId = " & CurrentUserId & " AND [Quarter] = " & LastQuarter & " AND [Year] = " & CurrentYear)
            Dim dtSurveyDataProj = DB.ExecuteScalar("SELECT SurveyData FROM PerformanceSurvey WHERE BuilderId = " & CurrentUserId & " AND [Quarter] = " & LastQuarter + 1 & " AND [Year] = " & CurrentYear)

            'If dtSurveyData Is Nothing And dtSurveyDataProj Is Nothing Then
            If dtSurveyDataProj Is Nothing Then
                hdnSurveyData.Value = "False"
            Else
                hdnSurveyData.Value = "True"
            End If
        Else
            Dim dtSurveyData = DB.ExecuteScalar("SELECT SurveyData FROM PerformanceSurvey WHERE BuilderId = " & CurrentUserId & " AND [Quarter] = " & LastQuarter & " AND [Year] = " & LastYear)
            Dim dtSurveyDataProj = DB.ExecuteScalar("SELECT SurveyData FROM PerformanceSurvey WHERE BuilderId = " & CurrentUserId & " AND [Quarter] = 1 AND [Year] = " & CurrentYear)
            ' If dtSurveyData Is Nothing And dtSurveyDataProj Is Nothing Then
            If dtSurveyDataProj Is Nothing Then
                hdnSurveyData.Value = "False"
            Else
                hdnSurveyData.Value = "True"
            End If
        End If

        Dim SelectiveBuilder = DB.ExecuteScalar("SELECT Builderid FROM Builder WHERE BuilderId = " & Session("BuilderId"))

        If Not IsPostBack And Not sm.IsInAsyncPostBack Then

            Core.DataLog("Reporting", PageURL, CurrentUserId, "Builder Top Menu Click", "", "", "", "", UserName)
            EditMode = True
            If ResponseDeadline < Now Then
                rblQuarter.Visible = False

                ltlCurrentQuarter.Visible = True
                ltlCurrentQuarter.Text = "Quarter " & CurrentQuarter & ", " & CurrentYear
                pSubmit.Visible = False

                ReportQuarter = CurrentQuarter
                ReportYear = CurrentYear
                hdnSurvey.Value = "False"
            Else
                Dim dbLastReport As PurchasesReportRow = PurchasesReportRow.GetPurchasesReportByPeriod(DB, Session("BuilderId"), LastYear, LastQuarter)

                If hdnSurveyData.Value = "False" Then
                    If SelectiveBuilder IsNot Nothing Then
                        hdnPostSurvey.Value = "True"
                    Else
                        hdnPostSurvey.Value = "False"
                    End If
                End If
                rblQuarter.Visible = True
                rblQuarter.Items.Add(New ListItem("Purchases Invoiced During Quarter " & LastQuarter & ", " & LastYear, LastQuarter & "/" & LastYear))
                rblQuarter.Items.Add(New ListItem("Purchases Invoiced During Quarter " & CurrentQuarter & ", " & CurrentYear, CurrentQuarter & "/" & CurrentYear))

                'If dbLastReport.Submitted <> Nothing Then
                '    ReportQuarter = CurrentQuarter
                '    ReportYear = CurrentYear
                '    rblQuarter.SelectedIndex = 1
                '    pSubmit.Visible = False
                '    ltlDeadline.Text = "(Deadline: " & FormatDateTime(ResponseDeadline, DateFormat.ShortDate) & ")"
                '    btnSubmit.Text = "Edit Quarter " & LastQuarter & " " & IIf(LastQuarter < 4, ReportYear, ReportYear - 1) & " Purchases Report"
                'Else
                '    'default to last qtr report if not submitted yet
                '    dbPurchasesReport = dbLastReport
                '    ReportQuarter = LastQuarter
                '    ReportYear = LastYear
                '    rblQuarter.SelectedIndex = 0
                '    pSubmit.Visible = True
                '    ltlDeadline.Text = "(Deadline: " & FormatDateTime(ResponseDeadline, DateFormat.ShortDate) & ")"
                '    btnSubmit.Text = "Submit Final Quarter " & LastQuarter & " " & IIf(LastQuarter < 4, ReportYear, ReportYear - 1) & " Purchases Report"
                'End If

                ReportQuarter = LastQuarter
                ReportYear = LastYear
                rblQuarter.SelectedIndex = 0
                pSubmit.Visible = True
                If dbLastReport.Submitted <> Nothing Then
                    ltlDeadline.Text = "(Deadline: " & FormatDateTime(ResponseDeadline.AddDays(-1), DateFormat.ShortDate) & ")"
                    btnSubmit.Text = "Edit Quarter " & LastQuarter & " " & LastYear & " Purchases Report"
                    EditMode = False
                    hdnSurvey.Value = "False"
                    btnSubmit.Enabled = True
                Else
                    ltlDeadline.Text = "(Deadline: " & FormatDateTime(ResponseDeadline.AddDays(-1), DateFormat.ShortDate) & ")"
                    btnSubmit.Text = "Submit Final Quarter " & LastQuarter & " " & LastYear & " Purchases Report"
                    EditMode = True
                    If hdnSurveyData.Value = "False" Then
                        If SelectiveBuilder IsNot Nothing Then
                            hdnSurvey.Value = "True"
                        Else
                            hdnSurvey.Value = "False"
                        End If
                    End If
                End If

                ltlCurrentQuarter.Visible = False
            End If

            EditIndex = -1

            BindUnreported()
            BindReported()
            LoadPerformanceSurvey()
        Else

            hdnSurvey.Value = "False"

            If hdnSurveyData.Value = "False" Then
                If SelectiveBuilder IsNot Nothing Then
                    hdnPostSurvey.Value = "True"
                Else
                    hdnPostSurvey.Value = "False"
                End If
            End If
        End If
    End Sub

    Private Sub CleanReportZeros()
        DB.ExecuteSQL("Delete From PurchasesReportVendorTotalAmount Where TotalAmount = 0 And PurchasesReportID = " & DB.Number(dbPurchasesReport.PurchasesReportID))
        BindUnreported()
        upUnreported.Update()
    End Sub

    Private Sub CleanPurchaseZeros()
        DB.ExecuteSQL("Delete From PurchasesReportVendorPO Where POAmount = 0 And PurchasesReportID = " & DB.Number(dbPurchasesReport.PurchasesReportID))
    End Sub

    Private Function GetDeadline(ByVal QuarterEnd As DateTime) As DateTime
        'Dim deadline As DateTime = DateAdd(DateInterval.Day, 30, QuarterEnd)
        'deadline = AddBusinessDays(deadline, SysParam.GetValue(DB, "ReportDeadlineDays"))
        'deadline = AddBusinessDays(deadline, SysParam.GetValue(DB, "DiscrepancyDeadlineDays"))
        'deadline = AddBusinessDays(deadline, SysParam.GetValue(DB, "DiscrepancyResponseDeadlineDays"))

        Dim deadline As DateTime = QuarterEnd.AddMonths(1)
        deadline = deadline.AddDays(SysParam.GetValue(DB, "ReportDeadlineDays") + 1)
        Return deadline
    End Function

    Private Function AddBusinessDays(ByVal start As DateTime, ByVal days As Integer) As DateTime
        Dim EndDay As Integer = DatePart(DateInterval.Weekday, DateAdd(DateInterval.Day, days, start))

        Dim quot As Integer = Math.Floor(days / 7)
        Dim remain As Integer = days Mod 7
        days += 2 * quot
        If remain > 0 Then
            Select Case start.DayOfWeek
                Case 1
                    days += IIf(remain = 6, 2, 1)
                Case 7
                    days += IIf(remain = 1, 1, 2)
                Case Else
                    If start.DayOfWeek + remain = 7 Then
                        days += 1
                    ElseIf (start.DayOfWeek + remain) Mod 7 < start.DayOfWeek Then
                        days += 2
                    End If
            End Select
        End If
        Return DateAdd(DateInterval.Day, days, start)
    End Function

    Private Sub UpdateTotals()
        dtTotals = PurchasesReportRow.GetVendors(DB, dbPurchasesReport.PurchasesReportID)
    End Sub

    Private Function UpdatePurchases(Optional ByVal VendorID As Integer = Nothing) As Double
        m_dtPurchases = PurchasesReportRow.GetPurchases(DB, dbPurchasesReport.PurchasesReportID)
        m_dvPurchases = m_dtPurchases.DefaultView
        m_dvPurchases.Sort = "VendorID"

        Dim sum As Double = 0
        For Each row As DataRowView In m_dvPurchases
            If row("VendorID") = VendorID Then
                sum += Core.GetDouble(row("POAmount"))
            End If
        Next
        Return sum
        'Return m_dtPurchases.AsEnumerable.Where(Function(row) Core.GetInt(row("VendorID")) = VendorID).Select(Of Double)(Function(row) Core.GetDouble(row("POAmount"))).DefaultIfEmpty.Aggregate(Function(sum, add) sum + add)
    End Function

    Protected Sub BindReported()
        Keys.Clear()
        Dim q = (From row As DataRow In dtTotals.AsEnumerable Join vendor As DataRow In dtVendors.AsEnumerable On row("VendorID") Equals vendor("VendorID") Join id As Integer In VendorIds On row("VendorID") Equals id Select New With {.VendorID = id, .TotalAmount = row("TotalAmount"), .CompanyName = vendor("CompanyName")}).OrderBy(Function(key) key.CompanyName)
        If q.Count > 0 Then
            rptReported.DataSource = q
        End If
        rptReported.DataBind()
    End Sub

    Protected Sub BindUnreported()
        Dim q = (From vendor As DataRow In dtVendors.AsEnumerable Join id In VendorIds On vendor("VendorID") Equals id Where Not (From total As DataRow In dtTotals.AsEnumerable Select total("VendorID")).Contains(vendor("VendorID")) Select vendor).OrderBy(Function(key) key("CompanyName"))
        If q.Count > 0 Then
            rptUnreported.DataSource = q
        End If
        rptUnreported.DataBind()
    End Sub

    Protected Sub hdnPostback_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles hdnPostback.ValueChanged
        Dim filter As String = txtVendorFilter.Text
        VendorIds.Clear()
        VendorIds.AddRange((From vendor As DataRow In dtVendors.AsEnumerable Where Left(vendor("CompanyName"), Math.Min(filter.Length, CStr(vendor("CompanyName")).Length)).ToLower = filter.ToLower Select vendor("VendorID")).Cast(Of Integer).DefaultIfEmpty)
        BindReported()
        BindUnreported()
        upReported.Update()
        upUnreported.Update()
    End Sub

    Protected Sub rptUnreported_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptUnreported.ItemCommand
        Dim dbTotal As PurchasesReportVendorTotalAmountRow = PurchasesReportVendorTotalAmountRow.GetRow(DB, dbPurchasesReport.PurchasesReportID, e.CommandArgument)
        Dim txtTotal As TextBox = e.Item.FindControl("txtTotal")
        If txtTotal.Text <> Nothing Then
            Dim cleaned As String = Regex.Replace(txtTotal.Text, "[^\d.]", "")
            If cleaned <> Nothing Then
                If cleaned > 0 Then
                    dbTotal.TotalAmount = cleaned
                    dbTotal.BuilderReportedInitialTotal = cleaned                   'Added by Apala (Medullus) on 29.11.2017 for VSO#9393
                    If dbTotal.Created = Nothing Then
                        dbTotal.CreatorBuilderAccountID = Session("BuilderAccountId")
                        dbTotal.Insert()
                    End If
                    ltlUnreportedMsg.Text = ""
                    'log Select unreported vendor to submit report
                    Core.DataLog("Reporting", PageURL, CurrentUserId, "Select From Unreported Vendor", dbTotal.VendorID, "", "", "", UserName)
                    'end log
                Else
                    ltlUnreportedMsg.Text = "<span class=""bold red"">There is no need to report values of $0.00.  If you did not purchase from this vendor, please leave this field blank.</span>"
                End If
            End If
        End If
        UpdateTotals()
        UpdatePurchases()
        BindReported()
        BindUnreported()
        upUnreported.Update()
        upReported.Update()
    End Sub

    Protected Sub rptUnreported_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptUnreported.ItemDataBound
        CType(e.Item.FindControl("ltlVendor"), Literal).Text = e.Item.DataItem("CompanyName")
        Dim btn As Button = e.Item.FindControl("btnSaveVendor")
        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btn)
    End Sub

    Protected Sub rblQuarter_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblQuarter.SelectedIndexChanged
        Dim period As String() = rblQuarter.SelectedValue.Split("/")
        ReportQuarter = period(0)
        ReportYear = period(1)

        'AR: This was causing duplicate purchases report record. Was this even necesary? Looks pretty redundant.
        'dbPurchasesReport = PurchasesReportRow.GetPurchasesReportByPeriod(DB, Session("BuilderId"), ReportYear, ReportQuarter)
        'If dbPurchasesReport.Created = Nothing Then
        '    dbPurchasesReport.PeriodYear = ReportYear
        '    dbPurchasesReport.PeriodQuarter = ReportQuarter
        '    dbPurchasesReport.BuilderID = Session("BuilderId")
        '    dbPurchasesReport.CreatorBuilderAccountID = Session("BuilderAccountID")
        '    dbPurchasesReport.SubmitterBuilderAccountID = Session("BuilderAccountID")
        '    dbPurchasesReport.Insert()
        'End If
        UpdateTotals()
        UpdatePurchases()

        If ResponseDeadline >= Now And ReportQuarter = LastQuarter Then
            pSubmit.Visible = True
            If dbPurchasesReport.Submitted = Nothing Then
                EditMode = True
            Else
                EditMode = False
            End If
        Else
            If ReportQuarter = CurrentQuarter Then
                EditMode = True
            Else
                EditMode = False
            End If
            pSubmit.Visible = False
        End If

        BindReported()
        BindUnreported()
        upReported.Update()
        upUnreported.Update()
    End Sub

    Protected Sub rptReported_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptReported.ItemCommand
        Select Case e.CommandName
            Case "Edit"
                EditIndex = e.Item.ItemIndex
                'log Edit Total Purchases from reported vendor
                Dim dbtotal As PurchasesReportVendorTotalAmountRow = PurchasesReportVendorTotalAmountRow.GetRow(DB, dbPurchasesReport.PurchasesReportID, e.CommandArgument)
                VendorId = dbtotal.VendorID
                Dim Msgbdy As String = "Edit Reported Vendors Id='" & VendorId & "'"
                Core.DataLog("Reporting", PageURL, CurrentUserId, Msgbdy, dbtotal.PurchasesReportID, "", "", "", UserName)
                'end log  
            Case "Delete"
                Dim dbTotal As PurchasesReportVendorTotalAmountRow = PurchasesReportVendorTotalAmountRow.GetRow(DB, dbPurchasesReport.PurchasesReportID, e.CommandArgument)
                dbTotal.Remove()

                dvPurchases.RowFilter = "VendorID=" & e.CommandArgument
                For Each row As DataRowView In dvPurchases
                    PurchasesReportVendorPORow.RemoveRow(DB, row("PurchasesReportVendorPOID"))
                Next
                UpdateTotals()
                UpdatePurchases()

                'log delete vendor from reported vendor
                VendorId = dbTotal.VendorID
                Dim Msgbdy As String = "Delete Reported Vendor Id='" & VendorId & "'"
                Core.DataLog("Reporting", PageURL, CurrentUserId, Msgbdy, dbTotal.PurchasesReportID, "", "", "", UserName)
                'end log                 

            Case "Save"
                Page.Validate(e.Item.UniqueID)
                If Not Page.IsValid Then
                    RenderErrors()
                    Exit Sub
                End If

                Dim dbTotal As PurchasesReportVendorTotalAmountRow = PurchasesReportVendorTotalAmountRow.GetRow(DB, dbPurchasesReport.PurchasesReportID, e.CommandArgument)
                Dim Cleaned As Double = 0
                Cleaned = Regex.Replace(DirectCast(e.Item.FindControl("txtTotal"), TextBox).Text, "[^\d.]", "")
                If Cleaned > 0 Then
                    dbTotal.TotalAmount = Cleaned
                    dbTotal.BuilderReportedInitialTotal = Cleaned       'Added by Apala (Medullus) on 29.11.2017 for VSO#9393
                    If dbTotal.Created <> Nothing Then
                        dbTotal.Update()
                    Else
                        dbTotal.VendorID = e.CommandArgument
                        dbTotal.CreatorBuilderAccountID = Session("BuilderAccountID")
                        dbTotal.PurchasesReportID = dbPurchasesReport.PurchasesReportID
                        dbTotal.Insert()
                    End If
                    UpdateTotals()

                    'log Save
                    VendorId = dbTotal.VendorID
                    Dim Msgbdy As String = "Save Reported Vendors Id='" & VendorId & "'"
                    Core.DataLog("Reporting", PageURL, CurrentUserId, Msgbdy, dbTotal.PurchasesReportID, "", "", "", UserName)
                    'end log 

                    EditIndex = -1
                    ltlReportedMsg.Text = ""
                Else
                    ltlReportedMsg.Text = "<span class=""bold red"">There is no need to report values of $0.00.  If you did not purchase from this vendor, please enter a value or delete the record.</span>"
                End If


            Case "Cancel"
                EditIndex = -1
                ltlReportedMsg.Text = ""
            Case "AddPurchase"
                Dim dbPurchase As New PurchasesReportVendorPORow(DB)
                dbPurchase.PurchasesReportID = dbPurchasesReport.PurchasesReportID
                dbPurchase.VendorID = e.CommandArgument
                dbPurchase.POAmount = 0
                dbPurchase.CreatorBuilderAccountID = Session("BuilderAccountId")
                dbPurchase.Insert()
                UpdatePurchases()
                PurchaseEditID = dbPurchase.PurchasesReportVendorPOID
                'log add purchase
                VendorId = dbPurchase.VendorID
                Dim Msgbdy As String = "Add Purchase For Vendor Id='" & VendorId & "'"
                Core.DataLog("Reporting", PageURL, CurrentUserId, Msgbdy, PurchaseEditID, "", "", "", UserName)
                'end log
        End Select
        CleanReportZeros()
        BindUnreported()
        upUnreported.Update()
        BindReported()
    End Sub

    Protected Sub rptReported_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptReported.ItemCreated
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If
        Keys.Add(DataBinder.Eval(e.Item.DataItem, "VendorID"))
        Dim rptPurchases As Repeater = e.Item.FindControl("rptPurchases")
        AddHandler rptPurchases.ItemDataBound, AddressOf rptPurchases_ItemDataBound
        AddHandler rptPurchases.ItemCommand, AddressOf rptPurchases_ItemCommand
    End Sub

    Protected Sub rptReported_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptReported.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        If Not IsDBNull(DataBinder.Eval(e.Item.DataItem, "TotalAmount")) Then
            TotalAmount += DataBinder.Eval(e.Item.DataItem, "TotalAmount")
        End If


        Dim spanTotal As HtmlGenericControl = e.Item.FindControl("spanTotal")
        Dim txtTotal As TextBox = e.Item.FindControl("txtTotal")

        Dim btnEdit As Button = e.Item.FindControl("btnEdit")
        Dim btnDelete As Button = e.Item.FindControl("btnDelete")
        Dim btnSave As Button = e.Item.FindControl("btnSave")
        Dim btnCancel As Button = e.Item.FindControl("btnCancel")
        Dim btnPurchases As HtmlInputButton = e.Item.FindControl("btnPurchases")
        Dim phNoPurchases As PlaceHolder = e.Item.FindControl("phNoPurchases")

        Dim rptPurchases As Repeater = e.Item.FindControl("rptPurchases")

        Dim divPurchases As HtmlGenericControl = e.Item.FindControl("divPurchases")
        Dim hdnPurchasesState As HiddenField = e.Item.FindControl("hdnPurchasesState")
        If Request(hdnPurchasesState.UniqueID) IsNot Nothing AndAlso Request(hdnPurchasesState.UniqueID) = "visible" Then
            divPurchases.Style.Remove("display")
            hdnPurchasesState.Value = "visible"
            btnPurchases.Value = "Hide Purchases"
        Else
            divPurchases.Style.Add("display", "none")
            hdnPurchasesState.Value = "hidden"
        End If

        Dim btnAddPurchase As Button = divPurchases.FindControl("btnAddPurchase")
        Dim btnImport As Button = divPurchases.FindControl("btnImport")
        btnAddPurchase.Visible = EditMode
        btnImport.Visible = EditMode

        dvPurchases.RowFilter = "VendorID=" & DataBinder.Eval(e.Item.DataItem, "VendorID")
        rptPurchases.DataSource = dvPurchases
        rptPurchases.DataBind()

        Dim POTotal As Double = 0
        If dvPurchases.Count > 0 Then
            'POTotal = (From purchase As DataRowView In dvPurchases.OfType(Of DataRowView)() Select purchase("POAmount")).Aggregate(Function(a, b) a + b)
            For Each row As DataRowView In dvPurchases
                POTotal += Core.GetDouble(row("POAmount"))
            Next
        End If
        If Math.Round(POTotal, 2) > CDbl(txtTotal.Text) Then
            Dim div As HtmlGenericControl = e.Item.FindControl("divWarning")
            Dim msg As String = "Purchase Orders for vendor '" & DataBinder.Eval(e.Item.DataItem, "CompanyName") & "' exceed entered total."
            div.InnerHtml = msg
            div.Visible = True
            ScriptManager.RegisterArrayDeclaration(Page, "Warnings", """" & msg & """")
        End If
        phNoPurchases.Visible = (dvPurchases.Count = 0)

        If e.Item.ItemIndex = EditIndex And EditMode Then
            btnEdit.Visible = False
            btnDelete.Visible = False
            btnSave.Visible = True
            btnCancel.Visible = True

            spanTotal.Visible = False
            txtTotal.Visible = True
        Else
            If EditMode Then
                btnEdit.Visible = True

                'must delete invoices before the total
                If dvPurchases.Count = 0 Then
                    btnDelete.Visible = True
                Else
                    btnDelete.Visible = False
                End If
                btnSave.Visible = False
                btnCancel.Visible = False
            Else
                btnEdit.Visible = False
                btnDelete.Visible = False
                btnPurchases.Visible = dvPurchases.Count > 0
                btnSave.Visible = False
                btnCancel.Visible = False
            End If
            txtTotal.Visible = False
            spanTotal.Visible = True
        End If
    End Sub

    Protected Sub rptPurchases_ItemCommand(ByVal sender As Object, ByVal e As RepeaterCommandEventArgs)
        Dim VendorIdx As Integer = CType(CType(sender, Repeater).NamingContainer, RepeaterItem).ItemIndex

        Select Case e.CommandName
            Case "Edit"
                PurchaseEditID = e.CommandArgument
                'log Edit Purchases from reported vendor section
                Dim dbPurchase As PurchasesReportVendorPORow = PurchasesReportVendorPORow.GetRow(DB, e.CommandArgument)
                VendorId = dbPurchase.VendorID
                Dim Msgbdy As String = "Edit Added Purchase For Vendor Id='" & VendorId & "'"
                Core.DataLog("Reporting", PageURL, CurrentUserId, Msgbdy, PurchaseEditID, "", "", "", UserName)
                'end log  
            Case "Delete"
                Dim dbPurchase As PurchasesReportVendorPORow = PurchasesReportVendorPORow.GetRow(DB, e.CommandArgument)
                dbPurchase.Remove()
                Dim dbTotal As PurchasesReportVendorTotalAmountRow = PurchasesReportVendorTotalAmountRow.GetRow(DB, dbPurchasesReport.PurchasesReportID, Keys(VendorIdx))
                dbTotal.TotalAmount = UpdatePurchases(dbPurchase.VendorID())
                dbTotal.Update()
                'log Delete Added Purchases from reported vendor section
                VendorId = dbPurchase.VendorID
                Dim Msgbdy As String = "Delete Purchase For Vendor Id='" & VendorId & "'"
                Core.DataLog("Reporting", PageURL, CurrentUserId, Msgbdy, dbPurchase.PurchasesReportVendorPOID, "", "", "", UserName)
                'end log 
            Case "Save"
                Page.Validate(e.Item.UniqueID)
                If Not Page.IsValid Then
                    RenderErrors()
                    Exit Sub
                End If

                Dim dbPurchase As PurchasesReportVendorPORow = PurchasesReportVendorPORow.GetRow(DB, e.CommandArgument)
                dbPurchase.VendorID = Keys(VendorIdx)
                Dim Cleaned As Double = 0
                Cleaned = Regex.Replace(CType(e.Item.FindControl("txtAmount"), TextBox).Text, "[^\d.]", "")
                If Cleaned > 0 Then
                    dbPurchase.POAmount = Cleaned
                    dbPurchase.PONumber = CType(e.Item.FindControl("txtNumber"), TextBox).Text
                    dbPurchase.PODate = CType(e.Item.FindControl("dpDate"), Controls.DatePicker).Value
                    dbPurchase.PurchasesReportID = dbPurchasesReport.PurchasesReportID
                    If dbPurchase.PurchasesReportVendorPOID = Nothing Then
                        dbPurchase.CreatorBuilderAccountID = Session("BuilderAccountId")
                        dbPurchase.Insert()
                    Else
                        dbPurchase.Update()
                    End If
                    Dim dbTotal As PurchasesReportVendorTotalAmountRow = PurchasesReportVendorTotalAmountRow.GetRow(DB, dbPurchasesReport.PurchasesReportID, Keys(VendorIdx))
                    dbTotal.TotalAmount = UpdatePurchases(dbPurchase.VendorID())
                    dbTotal.BuilderReportedInitialTotal = UpdatePurchases(dbPurchase.VendorID())    'Added by Apala (Medullus) on 29.11.2017 for VSO#9393
                    dbTotal.Update()

                    'log Save Purchases
                    VendorId = dbPurchase.VendorID
                    Dim Msgbdy As String = "Save Purchase For Vendor Id='" & VendorId & "'"
                    Core.DataLog("Reporting", PageURL, CurrentUserId, Msgbdy, dbPurchase.PurchasesReportVendorPOID, "", "", "", UserName)
                    'end log

                    PurchaseEditID = -1
                    ltlReportedMsg.Text = ""
                Else
                    ltlReportedMsg.Text = "<span class=""bold red"">There is no need to report values of $0.00.  If you did not purchase from this vendor, please enter a value or delete the record.</span>"
                End If

            Case "Cancel"
                PurchaseEditID = -1
        End Select
        CleanPurchaseZeros()
        CleanReportZeros()
        BindReported()
    End Sub

    Protected Sub rptPurchases_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs)
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        Dim ltlAmount As Literal = e.Item.FindControl("ltlAmount")
        Dim txtAmount As TextBox = e.Item.FindControl("txtAmount")
        Dim rfvAmount As RequiredFieldValidator = e.Item.FindControl("rfvAmount")
        Dim fvAmount As Controls.CurrencyValidator = e.Item.FindControl("fvAmount")
        Dim ltlNumber As Literal = e.Item.FindControl("ltlNumber")
        Dim txtNumber As TextBox = e.Item.FindControl("txtNumber")
        Dim ltlDate As Literal = e.Item.FindControl("ltlDate")
        Dim dpDate As Controls.DatePicker = e.Item.FindControl("dpDate")
        Dim dvDate As Controls.DateValidator = e.Item.FindControl("dvDate")

        Dim btnEdit As Button = e.Item.FindControl("btnEdit")
        Dim btnSave As Button = e.Item.FindControl("btnSave")
        Dim btnCancel As Button = e.Item.FindControl("btnCancel")
        Dim btnDelete As Button = e.Item.FindControl("btnDelete")
        'Dim btnImport As Button = e.Item.FindControl("btnImport")

        Dim container As RepeaterItem = CType(sender, Control).NamingContainer
        btnEdit.Visible = EditMode
        btnSave.Visible = EditMode
        btnCancel.Visible = EditMode
        btnDelete.Visible = EditMode

        If Not IsDBNull(e.Item.DataItem("PODate")) Then
            dpDate.Value = e.Item.DataItem("PODate")
            ltlDate.Text = FormatDateTime(e.Item.DataItem("PODate"), DateFormat.ShortDate) & "<br/>"
        End If

        If e.Item.DataItem("PurchasesReportVendorPOID") = PurchaseEditID Then
            ltlAmount.Visible = False
            ltlNumber.Visible = False
            ltlDate.Visible = False

            btnEdit.Visible = False
            btnDelete.Visible = False
        Else
            txtAmount.Visible = False
            rfvAmount.Enabled = False
            fvAmount.Enabled = False
            txtNumber.Visible = False
            dpDate.Visible = False
            dvDate.Enabled = False

            btnSave.Visible = False
            btnCancel.Visible = False
        End If
    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        If Not IsValid Then Exit Sub

        ReportQuarter = LastQuarter
        ReportYear = LastYear
        dbPurchasesReport.Submitted = Nothing
        dbPurchasesReport.SubmitterBuilderAccountID = Nothing
        dbPurchasesReport.Update()

        btnSubmit.Text = "Submit Final Quarter " & LastQuarter & " " & LastYear & " Sales Report"
        EditMode = True
        'log edit submitted report
        Core.DataLog("Reporting", PageURL, CurrentUserId, "Edit Submitted Purchase Report", dbPurchasesReport.PurchasesReportID, "", "", "", UserName)
        'end log
        UpdateTotals()
        UpdatePurchases()
        BindReported()
        BindUnreported()

        SendEditNotices()
    End Sub

    Private Sub SendEditNotices()
        'send emails here

        'Send PurchasesReportRetracted.

        Dim dbAutoMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "PurchasesReportRetracted")
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
        Dim MsgBody As String = String.Empty
        MsgBody = dbBuilder.CompanyName & " retracted the " & dbPurchasesReport.PeriodYear & " Q" & dbPurchasesReport.PeriodQuarter & " report. TotalAmount: " & FormatCurrency(TotalAmount)
        'dbAutoMsg.Send(dbBuilder, MsgBody)
        dbAutoMsg.SendAdmin(MsgBody)
    End Sub

    Protected Sub frmConfirm_Callback(ByVal sender As Object, ByVal args As PopupForm.PopupFormEventArgs) Handles frmConfirm.Callback
        If frmConfirm.IsValid Then
            Dim ret As String = String.Empty
            Dim Err As String = String.Empty
            Dim dt As DataTable = DB.GetDataTable("Select p.TotalAmount, v.CompanyName From PurchasesReportVendorTotalAmount p, Vendor v Where p.VendorId = v.VendorId And p.PurchasesReportID = " & DB.Number(dbPurchasesReport.PurchasesReportID))
            For Each dr As DataRow In dt.Rows
                If dr("TotalAmount") = 0 Then
                    Err &= "<li>" & dr("CompanyName")
                End If
            Next
            If Err <> String.Empty Then
                Err = "<br><br><p>You have reported $0 for the following vendor(s):</p>" & Err & "<br><br><p>Please correct this by either entering values for the Total Amount or deleting the record from the Reported Vendors section.</p>"
                ret = "<div class=""red"" style=""padding:25px;font-weight:bold;"">There were errors and your purchases report could not be submitted! "
                ret &= Err
                ret &= "<br /><br /><center><input type=""button"" value=""Go Back"" onclick=""location.href='builder-purchases.aspx';"" class=""btnred"" /></center>"
                frmConfirm.CallbackResult = ret
                Exit Sub
            End If

            dbPurchasesReport.Submitted = Now
            dbPurchasesReport.SubmitterBuilderAccountID = Session("BuilderAccountID")
            dbPurchasesReport.Update()

            'log submit purchase report
            Core.DataLog("Reporting", PageURL, CurrentUserId, "Submit Final Purchase Report", dbPurchasesReport.PurchasesReportID, "", "", "", UserName)
            'end log


            'Sync with insightly
            SyncSalesReportSubmittedStatus(Session("BuilderId"))
            'Send PurchasesReportSubmittedToAdmins.

            BindReported()

            Dim dbAutoMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "PurchasesReportSubmittedToAdmins")
            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
            Dim MsgBody As String = String.Empty
            MsgBody = dbBuilder.CompanyName & " submitted the " & dbPurchasesReport.PeriodYear & " Q" & dbPurchasesReport.PeriodQuarter & " report. TotalAmount: " & FormatCurrency(TotalAmount) & vbCrLf & vbCrLf & System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/admin/"
            dbAutoMsg.SendAdmin(MsgBody)

            ret = "<div style=""padding:25px;font-weight:bold;text-align:center;"">Purchases report submitted for Quarter " & ReportQuarter & ", " & ReportYear
            ret &= "<br /><br /><input type=""button"" value=""Continue"" onclick=""location.href='builder-purchases.aspx';"" class=""btnred"" />"
            frmConfirm.CallbackResult = ret
        End If
    End Sub

    Protected Sub UpdateConfirmMessage()

        frmConfirm.EnsureChildrenCreated()

        Dim q = From vendor As DataRow In dtVendors.AsEnumerable Group Join total As DataRow In dtTotals.AsEnumerable On vendor("VendorID") Equals total("VendorID") Into totals = Group
                From total In totals.DefaultIfEmpty Select New With {.vendor = vendor("CompanyName"), .total = total}

        Dim reported As String = String.Empty
        Dim unreported As String = String.Empty
        Dim reportedCnt As Integer = 0
        Dim unreportedCnt As Integer = 0
        For Each o As Object In q
            If o.total IsNot Nothing Then
                If reportedCnt Mod 2 = 1 Then
                    reported &= "<tr>"
                Else
                    reported &= "<tr class=""alt"">"
                End If
                reported &= "<td>" & o.vendor & "</td><td>" & FormatCurrency(o.total("TotalAmount")) & "</td></tr>"
                reportedCnt += 1
            Else
                If unreportedCnt Mod 2 = 1 Then
                    unreported &= "<tr>"
                Else
                    unreported &= "<tr class=""alt"">"
                End If
                unreported &= "<td>" & o.vendor & "</td>"
                unreportedCnt += 1
            End If
        Next
        If reported <> String.Empty Then
            reported = "<table cellpadding=""2"" cellspacing=""0"" border=""0"">" & reported & "</table>"
        End If
        If unreported <> String.Empty Then
            unreported = "<table cellpadding=""2"" cellspacing=""0"" border=""0"">" & unreported & "</table>"
        End If
        If ScriptManager.GetCurrent(Page).IsInAsyncPostBack Then
            'Dim json As New System.Web.Script.Serialization.JavaScriptSerializer
            ScriptManager.GetCurrent(Page).RegisterDataItem(frmConfirm, "{'unreported':'" & unreported.Replace("\", "\\").Replace("'", "\'") & "','reported':'" & reported.Replace("\", "\\").Replace("'", "\'") & "'}", True)
        Else
            ltlConfirmReported.InnerHtml = reported
            ltlConfirmUnreported.InnerHtml = unreported
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        UpdateConfirmMessage()

        'submit btn full postback to go to edit mode
        If Not EditMode Then
            btnSubmit.OnClientClick = Nothing
        Else
            btnSubmit.OnClientClick = "OpenConfirm();return false;"
        End If
    End Sub

    Private Sub RenderErrors()
        Dim sm As ScriptManager = ScriptManager.GetCurrent(Page)
        If Not sm.IsInAsyncPostBack Then Exit Sub

        Dim out As New StringBuilder
        Dim sw As New IO.StringWriter(out)
        Dim hw As New HtmlTextWriter(sw)

        Dim eph As MasterPages.ErrorMessage = CType(Page, SitePage).ErrorPlaceHolder
        eph.RenderControl(hw)
        sm.RegisterDataItem(eph, out.ToString)
    End Sub

    Protected Sub btnSaveAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveAll.Click
        ltlUnreportedMsg.Text = ""
        For Each item As RepeaterItem In rptUnreported.Items
            Dim btn As Button = item.FindControl("btnSaveVendor")
            Dim amt As TextBox = item.FindControl("txtTotal")
            If amt.Text <> Nothing Then
                Dim cleaned As String = Regex.Replace(amt.Text, "[^\d.]", "")
                If cleaned <> Nothing Then
                    If cleaned > 0 Then
                        Dim dbTotal As PurchasesReportVendorTotalAmountRow = PurchasesReportVendorTotalAmountRow.GetRow(DB, dbPurchasesReport.PurchasesReportID, btn.CommandArgument)
                        dbTotal.TotalAmount = cleaned
                        dbTotal.BuilderReportedInitialTotal = cleaned           'Added by Apala (Medullus) on 29.11.2017 for VSO#9393
                        If dbTotal.Created = Nothing Then
                            dbTotal.CreatorBuilderAccountID = Session("BuilderAccountId")
                            dbTotal.Insert()
                        End If
                    Else
                        ltlUnreportedMsg.Text = "<span class=""bold red"">There is no need to report values of $0.00.  If you did not purchase from this vendor, please leave this field blank.</span>"
                    End If

                End If
            End If
        Next
        UpdateTotals()
        UpdatePurchases()

        'log btn save all clicked
        Core.DataLog("Reporting", PageURL, CurrentUserId, "Btn Save All", "", "", "", "", UserName)
        'end log

        BindReported()
        BindUnreported()
        upUnreported.Update()
        upReported.Update()
    End Sub

    Protected Sub btnHistory_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnHistory.Click
        'log Purchase Report History
        Core.DataLog("Reporting", PageURL, CurrentUserId, "Btn Purchase Report History", "", "", "", "", UserName)
        'end log
        Response.Redirect("purchases-history.aspx")
    End Sub

    '--------*** PerformanceSurvey Code ***-----------
    Protected Sub btnSavePerfromance_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSavePerfromance.Click

        If Not IsValid Then Exit Sub

        '--------------- INSERT PROJECTED VALUE FOR CURRENT QUARTER ----------------
        Dim sqlProjectedValue As String = "IF NOT EXISTS (SELECT * FROM PerformanceSurvey WHERE BuilderId = " & Core.ProtectParam(Session("BuilderID")) & " AND [Quarter] = " & hdnProjectedQuarter.Value & " AND [Year] = " & hdnProjectedYear.Value & " AND Projected = 1) " _
                                          & "INSERT INTO PerformanceSurvey (BuilderId, [Quarter], SurveyData, Projected, [Year], CreatedOn, UpdatedOn) VALUES (" _
                                          & Core.ProtectParam(Session("BuilderID")) & ", " & hdnProjectedQuarter.Value & ", " & Core.ProtectParam(txtvalue5.Text) & ", " & 1 & ", " & hdnProjectedYear.Value & ", GETDATE(), GETDATE()) "
        '----------------------------------------------------------------------------

        If hdnPerformanceSurveyId4.Value <> "" Then
            '------ UPDATE ACTUAL VALUE THAT WAS PROJECTED IN PREVIOUS QUARTER 
            Dim sqlInsertActualValueForLastQuarter As String = "IF NOT EXISTS (SELECT * FROM PerformanceSurvey WHERE BuilderId = " & Core.ProtectParam(Session("BuilderID")) & " AND [Quarter] = (SELECT [Quarter] FROM performancesurvey WHERE PerformanceSurveyid = " & hdnPerformanceSurveyId4.Value & ") AND [Year] = (SELECT [Year] FROM performancesurvey WHERE PerformanceSurveyid = " & hdnPerformanceSurveyId4.Value & ") AND Projected = 0) " _
                                                               & "INSERT INTO PerformanceSurvey (BuilderId, [Quarter], [Year], SurveyData, Projected, CreatedOn, UpdatedOn) VALUES (" _
                                                               & Core.ProtectParam(Session("BuilderID")) & ", " _
                                                               & "(SELECT [Quarter] FROM performancesurvey WHERE PerformanceSurveyid = " & hdnPerformanceSurveyId4.Value & "), " _
                                                               & "(SELECT [Year] FROM performancesurvey WHERE PerformanceSurveyid = " & hdnPerformanceSurveyId4.Value & "), " _
                                                               & Core.ProtectParam(txtvalue4.Text) & ", 0, GETDATE(), GETDATE())"
            DB.ExecuteSQL(sqlInsertActualValueForLastQuarter)

            'Dim updateProjectedSql = "UPDATE PerformanceSurvey SET SurveyData = " & Core.ProtectParam(txtvalue4.Text) & ", Projected = 0, UpdatedOn = GETDATE() WHERE PerformanceSurveyid = " & hdnPerformanceSurveyId4.Value
            'DB.ExecuteSQL(updateProjectedSql)
            '---------------------------------------------------------------------
        Else
            '--------------- INSERT ACTUAL VALUE FOR LAST QUARTER ----------------
            Dim sqlActualValue As String = "IF NOT EXISTS (SELECT * FROM PerformanceSurvey WHERE BuilderId = " & Core.ProtectParam(Session("BuilderID")) & " AND [Quarter] = " & LastQuarter & " AND [Year] = " & LastYear & " AND Projected = 0) " _
                                           & "INSERT INTO PerformanceSurvey (BuilderId, [Quarter], SurveyData, Projected, [Year], CreatedOn, UpdatedOn) VALUES (" _
                                           & Core.ProtectParam(Session("BuilderID")) & ", " & LastQuarter & ", " & Core.ProtectParam(txtvalue4.Text) & ", " & 0 & ", " & LastYear & ", GETDATE(), GETDATE())"

            DB.ExecuteSQL(sqlActualValue)
            '----------------------------------------------------------------------
        End If

        DB.ExecuteSQL(sqlProjectedValue)

        '----------------- INSERT ANNUAL ACTUALS AND PROJECTIONS ------------------
        Dim sqlAnnualFigures As String = "IF NOT EXISTS (SELECT * FROM Quarterly_YearlyPerformanceSurvey WHERE BuilderId = " & Core.ProtectParam(Session("BuilderID")) & " AND ActualYear = " & CInt(lblActualYear.InnerText.Trim()) & ") " _
                                         & "INSERT INTO Quarterly_YearlyPerformanceSurvey (BuilderId, GrossRevenueActual, GrossRevenueProjection, TotalStartsActual, TotalStartsProjection, ActualYear, CreatedOn, UpdatedOn) VALUES (" _
                                         & Core.ProtectParam(Session("BuilderID")) & "," & Core.ProtectParam(txtRevenueActual.Value) & "," & Core.ProtectParam(txtRevenueProj.Value) & "," & Core.ProtectParam(txtStartActual.Value) & "," & Core.ProtectParam(txtStartProj.Value) & "," & CInt(lblActualYear.InnerText.Trim()) & ", GETDATE(), GETDATE())"

        If LastQuarter = 4 Then
            DB.ExecuteSQL(sqlAnnualFigures)
        End If
        hdnPostSurvey.Value = "False"
        '----------------- -------------------------------------- ------------------

        '---------------- UPDATE PREVIOUS QUARTER VALUES --------------
        Dim sqlUpdatePreviousQuater1Value As String = "UPDATE PerformanceSurvey SET SurveyData = " & Core.ProtectParam(hdnNewSurveyValue1.Value) & ", Projected = 0, UpdatedOn = GETDATE() WHERE PerformanceSurveyid = " & hdnPerformanceSurveyId1.Value
        If hdnNewSurveyValue1.Value <> "" Then
            DB.ExecuteSQL(sqlUpdatePreviousQuater1Value)
        End If

        Dim sqlUpdatePreviousQuater2Value As String = "UPDATE PerformanceSurvey SET SurveyData = " & Core.ProtectParam(hdnNewSurveyValue2.Value) & ", Projected = 0, UpdatedOn = GETDATE() WHERE PerformanceSurveyid = " & hdnPerformanceSurveyId2.Value
        If hdnNewSurveyValue2.Value <> "" Then
            DB.ExecuteSQL(sqlUpdatePreviousQuater2Value)
        End If

        Dim sqlUpdatePreviousQuater3Value As String = "UPDATE PerformanceSurvey SET SurveyData = " & Core.ProtectParam(hdnNewSurveyValue3.Value) & ", Projected = 0, UpdatedOn = GETDATE() WHERE PerformanceSurveyid = " & hdnPerformanceSurveyId3.Value
        If hdnNewSurveyValue3.Value <> "" Then
            DB.ExecuteSQL(sqlUpdatePreviousQuater3Value)
        End If
        '---------------- ------------------------------- --------------

        BindReported()
        BindUnreported()

    End Sub

    Private Sub LoadPerformanceSurvey()

        Dim BuilderId = Session("BuilderId")

        Dim PrevQuarter1 As String = "0"
        Dim PrevQuarter2 As String = "0"
        Dim PrevQuarter3 As String = "0"

        Dim objDB As New Database
        Dim sr As DataTable = Nothing

        Dim iQtrCounter As Int16
        Dim PreviousQtr As Int16 = 0

        For iQtrCounter = 1 To 4
            If PreviousQtr > 0 Then
                PreviousQtr = PreviousQtr - 1
            End If

            If PreviousQtr = 0 Then
                PreviousQtr = IIf((LastQuarter - iQtrCounter = 0), 4, Math.Abs(LastQuarter - iQtrCounter))
            End If

            If PrevQuarter1 = "0" Then
                PrevQuarter1 = PreviousQtr
            ElseIf PrevQuarter2 = "0" Then
                PrevQuarter2 = PreviousQtr
            ElseIf PrevQuarter3 = "0" Then
                PrevQuarter3 = PreviousQtr
            End If
        Next

        SpnCurrentQuarter.InnerHtml = LastYear & " Q" & LastQuarter & " "

        objDB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))

        If LastQuarter = 1 Or LastQuarter = 2 Or LastQuarter = 3 Then
            Dim PreviousYear As Integer = CurrentYear - 1
            sr = objDB.GetDataTable("SELECT * FROM PerformanceSurvey WHERE BuilderId = " & BuilderId & " AND [Year] IN (" & CurrentYear & "," & PreviousYear & ") AND [Quarter] IN (" & LastQuarter & "," & PrevQuarter1 & "," & PrevQuarter2 & "," & PrevQuarter3 & ") ORDER BY [Year] ASC, [Quarter] ASC ")
        ElseIf LastQuarter = 4 Then
            Dim PreviousYear As Integer = CurrentYear - 1
            Dim sqlPerformanceSurvey As String = "SELECT * FROM PerformanceSurvey WHERE BuilderId = " & BuilderId & " AND [Year] IN (" & PreviousYear & ") AND [Quarter] IN (" & PrevQuarter1 & "," & PrevQuarter2 & "," & PrevQuarter3 & ") AND Projected = 0 " &
                                                 "UNION " &
                                                 "SELECT * FROM PerformanceSurvey WHERE BuilderId = " & BuilderId & " AND [Year] IN (" & LastYear & ") AND [Quarter] IN (" & LastQuarter & ") AND Projected = 1 " &
                                                 "ORDER BY [Year] ASC, [Quarter] ASC "
            sr = objDB.GetDataTable(sqlPerformanceSurvey)
        End If

        If sr.Rows.Count > 0 Then
            Dim RowsCount As Integer = sr.Rows.Count
            Dim iCounter As Integer = 0
            Dim iIndex As Integer = sr.Rows.Count - 1

            For iCounter = 4 To 1 Step -1

                If iIndex < 0 Then Exit For

                If (iCounter = 4) OrElse (iCounter < 4 AndAlso sr.Rows(iIndex)("Projected") = False) Then

                    Dim iControlCounter As Int16 = iCounter

                    If iIndex >= RowsCount Then
                        Me.FindControl("txtvalue" & iControlCounter).Visible = False
                        Me.FindControl("divModifiedDate" & iControlCounter).Visible = False
                        Me.FindControl("Year_Quarter" & iControlCounter).Visible = False
                    Else
                        Dim txtProjValue As New TextBox
                        txtProjValue = Me.FindControl("txtvalue" & iControlCounter)
                        txtProjValue.Text = sr.Rows(iIndex)("SurveyData").ToString()
                        txtProjValue.Style.Add("display", "block")

                        Dim hdnSurveyValue As New HiddenField
                        hdnSurveyValue = Me.FindControl("SurveyValue" & iControlCounter)
                        If Not hdnSurveyValue Is Nothing Then
                            hdnSurveyValue.Value = sr.Rows(iIndex)("SurveyData").ToString()
                        End If

                        Dim hdnProjectedValue As New HiddenField
                        hdnProjectedValue = Me.FindControl("ProjectedValue" & iControlCounter)
                        hdnProjectedValue.Value = sr.Rows(iIndex)("Projected").ToString()

                        Dim divModifiedDate As HtmlGenericControl
                        divModifiedDate = Me.FindControl("divModifiedDate" & iControlCounter)
                        Dim LastQtr As Int32 = IIf(CInt(sr.Rows(iIndex)("Quarter")) = 1, 4, CInt(sr.Rows(iIndex)("Quarter")) - 1)
                        divModifiedDate.InnerText = If(sr.Rows(iIndex)("Projected").ToString() = "True", "Estimated with Q" & LastQtr.ToString() & " Report", "Entered with Q" & sr.Rows(iIndex)("Quarter").ToString() & " Report")

                        Dim divYearQuarter As HtmlGenericControl
                        divYearQuarter = Me.FindControl("Year_Quarter" & iControlCounter)
                        divYearQuarter.InnerText = sr.Rows(iIndex)("Year").ToString() & " Q" & sr.Rows(iIndex)("Quarter").ToString()

                        Dim hdnPerformanceSurveyId As New HiddenField
                        hdnPerformanceSurveyId = Me.FindControl("hdnPerformanceSurveyId" & iControlCounter)
                        hdnPerformanceSurveyId.Value = sr.Rows(iIndex)("PerformanceSurveyId").ToString()
                    End If
                End If

                iIndex = iIndex - 1
            Next
        Else
            txtvalue1.Visible = "false"
            txtvalue2.Visible = "false"
            txtvalue3.Visible = "false"
            txtvalue4.Style.Item("display") = "block"

            divModifiedDate1.Visible = "false"
            divModifiedDate2.Visible = "false"
            divModifiedDate3.Visible = "false"
            divModifiedDate4.InnerText = "Enter Actual Starts"

            Year_Quarter1.Visible = "false"
            Year_Quarter2.Visible = "false"
            Year_Quarter3.Visible = "false"
            Year_Quarter4.InnerText = IIf(LastQuarter = 4, (CurrentYear - 1) & " Q" & LastQuarter, CurrentYear & " Q" & LastQuarter)
        End If

        PopulateProjectedYearlyFields()

    End Sub
    Private Sub PopulateProjectedYearlyFields()
        Dim objDB As New Database
        objDB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))

        Dim strYear As String = IIf(LastQuarter = 1 Or LastQuarter = 2 Or LastQuarter = 3, CurrentYear, CurrentYear)
        Dim strQuarter As String = IIf(LastQuarter = 4, 1, LastQuarter + 1)

        divProjectedYear.InnerText = strYear & " Q" & strQuarter
        divProjected.InnerText = "Estimated Q" & strQuarter & " Starts"

        hdnProjectedYear.Value = IIf(LastQuarter = 1 Or LastQuarter = 2 Or LastQuarter = 3, CurrentYear, CurrentYear)
        hdnProjectedQuarter.Value = IIf(LastQuarter = 4, 1, LastQuarter + 1)

        If LastQuarter = 4 Then
            divPerformance_Q4.Visible = "true"
            lblPercent.InnerHtml = "Percent of " & CurrentYear & " Starts on Land You Own:"
            lblURSquare.InnerHtml = "Total Under-Roof Square Footage of " & CurrentYear & ":"
            lblAvg.InnerHtml = "Avg " & CurrentYear & " Per Square Foot Sales Price:"

            lblActualYear.InnerText = (CurrentYear - 1).ToString()
            lblProjYear.InnerText = CurrentYear.ToString()

            'Dim Qyp As DataTable = Nothing
            'Qyp = objDB.GetDataTable("SELECT GrossRevenueProjection, TotalStartsProjection FROM Quarterly_YearlyPerformanceSurvey WHERE BuilderId = " & Session("BuilderId") & " AND ActualYear = " & CurrentYear)

            'If Qyp.Rows.Count <> 0 Then                '--*changed in dec as requested by brian
            '    txtRevenueActual.Value = Qyp.Rows(0)("GrossRevenueProjection").ToString()
            'End If
            ' txtStartActual.Value = DB.ExecuteScalar("select sum(Surveydata) from performancesurvey where Quarter in (1,2,3,4) and year=" & CurrentYear & " and builderid=" & Session("BuilderId"))   '--* for populating statactual after 2020 (changed in dec as requested by brian)
        End If

    End Sub
    '--------*** End PerformanceSurvey Code ***-----------

    '*Start*******Code Added BY Medullus(Debashis) on 19/09/2018 #UserStory - 16526
    Protected Sub btnImportProduct_Click(sender As Object, e As EventArgs)
        ltrErrorMsg.Text = ""
        Dim FileInfo As System.IO.FileInfo
        Dim OriginalExtension As String
        Dim NewFileName As String

        If fulDocument.NewFileName <> String.Empty Then

            OriginalExtension = System.IO.Path.GetExtension(fulDocument.MyFile.FileName)

            If OriginalExtension <> ".csv" And OriginalExtension <> ".xls" Then
                ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
                ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""><li>Please enter a valid .csv or .xls file. Your file extension was: " & OriginalExtension & "</span></td></tr></table>"
                ltrErrorMsg.Visible = True
                Exit Sub
            End If

            fulDocument.Folder = "/assets/builder/reporting"
            fulDocument.SaveNewFile()

            FileInfo = New System.IO.FileInfo(Server.MapPath(fulDocument.Folder & fulDocument.NewFileName))

            NewFileName = Core.GenerateFileID
            FileInfo.CopyTo(Server.MapPath(fulDocument.Folder & NewFileName & OriginalExtension))

            FileInfo.Delete()
        Else
            ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
            ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""><li>Please enter the file you want to upload</span></td></tr></table>"
            ltrErrorMsg.Visible = True

            LoadPerformanceSurvey()
            Exit Sub
        End If

        Session("POImportFile") = NewFileName & OriginalExtension

        Try
            ImportPO(Session("POImportFile"))
            BindReported()
            BindUnreported()
        Catch ex As Exception
            ltrErrorMsg.Text = Err.Description
            ltrErrorMsg.Visible = True
        End Try
    End Sub
    Protected Sub ImportPO(ByVal FileName As String)
        Dim aLine As String()
        Dim Count As Integer = 0
        Dim BadCount As Integer = 0
        Dim tblErr As String = String.Empty

        Dim POAmount As String = String.Empty
        Dim PONumber As String = String.Empty
        Dim DateOfPurchase As String = String.Empty

        Dim ErrorStatus As String = String.Empty


        If Not File.Exists(Server.MapPath(fulDocument.Folder & FileName)) Then
            ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
            ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""></li>Cannot find the file to process</span></td></tr></table>"

            LoadPerformanceSurvey()
            Exit Sub
        End If


        Dim dtPODetails As New DataTable
        dtPODetails.Columns.Add("POAmount", GetType(String))
        dtPODetails.Columns.Add("PONumber", GetType(String))
        dtPODetails.Columns.Add("DateOfPurchase", GetType(String))


        Dim f As StreamReader = New StreamReader(Server.MapPath(fulDocument.Folder & FileName))
        While Not f.EndOfStream
            Count = Count + 1

            Dim sLine As String = f.ReadLine()

            Dim bInside As Boolean = False
            For iLoop As Integer = 1 To Len(sLine)
                If Mid(sLine, iLoop, 1) = """" Then
                    If bInside = False Then
                        bInside = True
                    Else
                        bInside = False
                    End If
                End If
                If Mid(sLine, iLoop, 1) = "," Then
                    If Not bInside Then
                        sLine = Left(sLine, iLoop - 1) & "|" & Mid(sLine, iLoop + 1, Len(sLine) - iLoop)
                    End If
                End If
            Next

            aLine = sLine.Split("|")

            If aLine.Length >= 3 AndAlso aLine(0) <> String.Empty Then

                POAmount = Trim(Core.StripDblQuote(aLine(0)))
                PONumber = Trim(Core.StripDblQuote(aLine(1)))
                DateOfPurchase = Trim(Core.StripDblQuote(aLine(2)))

                If Count = 1 And POAmount <> "POAmount" Then
                    ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
                    ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""><li>The file you uploaded does not appear to be in the correct format.</td></tr></table>"

                    LoadPerformanceSurvey()
                    Exit Sub
                End If
                'POAmount must be a numeric value and greater than 0
                If Count <> 1 And POAmount <> String.Empty Then
                    If Not IsNumeric(POAmount) Then
                        BadCount = BadCount + 1
                        ErrorStatus &= "Invalid Amount for " & PONumber
                        Continue While
                    ElseIf POAmount <= 0 Then
                        POAmount = 0
                        'BadCount = BadCount + 1
                        'ErrorStatus &= "Non-Positive Amount" & PONumber
                        'Continue While
                    End If
                End If
                'DateOfPurchase must be a valid Date
                If Count <> 1 Then
                    If Not IsDate(DateOfPurchase) Then
                        BadCount = BadCount + 1
                        ErrorStatus &= "Invalid DateOfPurchase for " & PONumber
                        Continue While
                    Else
                        'checking date according to valid  Querter Date 
                        Dim MinDate As DateTime = (((ReportQuarter - 1) * 3 + 1) & "/1/" & ReportYear)
                        If MinDate <> Nothing Then
                            Dim dt As DateTime = DateOfPurchase
                            If dt < MinDate Then
                                BadCount = BadCount + 1
                                ErrorStatus &= "Invalid DateOfPurchase for " & PONumber
                                Continue While
                            End If
                        End If
                    End If
                End If

                'save to datatable
                If Count <> 1 Then
                    dtPODetails.Rows.Add(POAmount, PONumber, DateOfPurchase)
                End If

            Else
                BadCount = BadCount + 1
                tblErr &= "<tr class=""red""><td>Row: " & Count & "</td><td>BAD ROW</td><tr>"
                Continue While
            End If
        End While

        'Save Data To Database
        If ErrorStatus <> String.Empty Then
            ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
            ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""><li>The uploaded file contains invalid Date Of Purchase and/or PO Amount . </td></tr></table>"

            LoadPerformanceSurvey()
            Exit Sub


        Else
            'save data
            If dtPODetails.Rows.Count > 0 Then
                For Each row As Object In dtPODetails.Rows
                    Dim dbPurchase As PurchasesReportVendorPORow = PurchasesReportVendorPORow.GetRow(DB, hdnVendorID.Value)
                    dbPurchase.VendorID = hdnVendorID.Value
                    Dim Cleaned As Double = 0
                    Cleaned = Regex.Replace(row.Item("POAmount"), "[^\d.]", "")
                    If Cleaned > 0 Then
                        dbPurchase.POAmount = Cleaned
                        dbPurchase.PONumber = row.Item("PONumber")
                        dbPurchase.PODate = row.Item("DateOfPurchase")
                        dbPurchase.PurchasesReportID = dbPurchasesReport.PurchasesReportID
                        dbPurchase.CreatorBuilderAccountID = Session("BuilderAccountId")
                        dbPurchase.Insert()

                        'Update for total Ammount of vendor
                        Dim dbTotal As PurchasesReportVendorTotalAmountRow = PurchasesReportVendorTotalAmountRow.GetRow(DB, dbPurchasesReport.PurchasesReportID, hdnVendorID.Value)
                        dbTotal.TotalAmount = UpdatePurchases(dbPurchase.VendorID())
                        dbTotal.BuilderReportedInitialTotal = UpdatePurchases(dbPurchase.VendorID())
                        dbTotal.Update()

                    End If
                Next
            End If

            LoadPerformanceSurvey()
        End If

    End Sub
    '*End*******Code Added BY Medullus(Debashis) on 19/09/2018 #UserStory - 16526
    Private Sub SyncSalesReportSubmittedStatus(ByVal BuilderID As String)

        Dim SQL As String = "SELECT CRMID FROM BUILDER WHERE BUILDERID=" & BuilderID & ""
        Dim ORGANISATION_ID As String = DB.ExecuteScalar(SQL)
        Dim strBody As String = "{""ORGANISATION_ID"":" & ORGANISATION_ID & ",""CUSTOMFIELDS"":[{""FIELD_NAME"":""Quarterly_Report__c"",""FIELD_VALUE"":""Report Received""}]}"
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12 Or SecurityProtocolType.Ssl3
        Dim req1 As WebRequest = WebRequest.Create("https://api.insightly.com/v3.1/Organisations")
        req1.Method = "PUT"
        req1.Headers("Authorization") = "Basic ZDgyNDdjNzAtYWIyZC00NDlkLTllMGMtNzViODAxODBkZTkyOg=="
        req1.ContentLength = strBody.Length

        If (Not strBody Is Nothing) Then
            Dim postBytes = Encoding.ASCII.GetBytes(strBody)
            req1.ContentLength = postBytes.Length
            Dim requestStream As Stream = req1.GetRequestStream()
            requestStream.Write(postBytes, 0, postBytes.Length)
        End If
        Using resp As HttpWebResponse = TryCast(req1.GetResponse(), HttpWebResponse)
            If (resp.StatusCode = HttpStatusCode.OK) Then
            End If
        End Using
    End Sub

End Class

