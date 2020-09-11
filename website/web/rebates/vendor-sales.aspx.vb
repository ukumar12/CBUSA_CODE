Imports Components
Imports DataLayer
Imports System.Data
Imports System.Linq
Imports System.Net
Imports System.IO

Partial Class rebates_vendor_sales
    Inherits SitePage
    Protected TotalAmount As Double = 0

    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private PurchasesReportVendorPOId As String = ""
    Private BuilderId As String = ""
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

    'Private ReadOnly Property Now() As DateTime
    '    Get
    '        Dim param As String = SysParam.GetValue(DB, "DemoReportingDate")
    '        If param = Nothing OrElse param = "0" Then
    '            Return DateTime.Now
    '        Else
    '            Return param
    '        End If
    '    End Get
    'End Property

#Region "Properties"
    Private ReadOnly Property CurrentQuarter() As Integer
        Get
            Return Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
        End Get
    End Property

    Private ReadOnly Property CurrentYear() As Integer
        Get
            Return Now.Year
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

    Private m_ResponseDeadline As DateTime
    Private ReadOnly Property ResponseDeadline() As DateTime
        Get
            If m_ResponseDeadline = Nothing Then
                Dim LastQuarterEnd As DateTime = (LastQuarter * 3) & "/" & Date.DaysInMonth(LastYear, LastQuarter * 3) & "/" & LastYear
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
            If m_dbSalesReport IsNot Nothing AndAlso ViewState("ReportQuarter") <> m_dbSalesReport.PeriodQuarter Then
                dbSalesReport = Nothing
            End If
        End Set
    End Property

    Protected Property ReportYear() As Integer
        Get
            Return ViewState("ReportYear")
        End Get
        Set(ByVal value As Integer)
            ViewState("ReportYear") = value
            If m_dbSalesReport IsNot Nothing AndAlso ViewState("ReportYear") <> m_dbSalesReport.PeriodYear Then
                dbSalesReport = Nothing
            End If
        End Set
    End Property

    Private m_dbSalesReport As SalesReportRow
    Private Property dbSalesReport() As SalesReportRow
        Get
            If m_dbSalesReport Is Nothing OrElse (m_dbSalesReport.PeriodQuarter <> ReportQuarter Or m_dbSalesReport.PeriodYear <> ReportYear) Then
                m_dbSalesReport = SalesReportRow.GetSalesReportByPeriod(DB, Session("VendorId"), ReportYear, ReportQuarter)
                If m_dbSalesReport.Created = Nothing Then
                    m_dbSalesReport.VendorID = Session("VendorID")
                    m_dbSalesReport.PeriodQuarter = ReportQuarter
                    m_dbSalesReport.PeriodYear = ReportYear
                    m_dbSalesReport.CreatorVendorAccountID = Session("VendorAccountID")
                    m_dbSalesReport.Insert()
                End If
            End If
            Return m_dbSalesReport
        End Get
        Set(ByVal value As SalesReportRow)
            m_dbSalesReport = value
            m_dtTotals = Nothing
            m_dtInvoices = Nothing
        End Set
    End Property

    Private m_dtTotals As DataTable
    Private Property dtTotals() As DataTable
        Get
            If m_dtTotals Is Nothing Then
                m_dtTotals = SalesReportRow.GetBuilders(DB, dbSalesReport.SalesReportID)
            End If
            Return m_dtTotals
        End Get
        Set(ByVal value As DataTable)
            m_dtTotals = value
        End Set
    End Property

    Private m_dtBuilders As DataTable
    Private ReadOnly Property dtBuilders() As DataTable
        Get
            If m_dtBuilders Is Nothing Then
                '***** Following line commented and next line added by Apala (Medullus) on 08.02.2018 for mGuard#T-1092
                'm_dtBuilders = VendorRow.GetAllBuilders(DB, Session("VendorId"))
                m_dtBuilders = VendorRow.GetAllBuildersForSalesReport(DB, Session("VendorId"))
            End If
            Return m_dtBuilders
        End Get
    End Property

    Private m_dtInvoices As DataTable
    Private m_dvInvoices As DataView
    Private ReadOnly Property dvInvoices() As DataView
        Get
            If m_dvInvoices Is Nothing Or m_dtInvoices Is Nothing Then
                If m_dtInvoices Is Nothing Then
                    m_dtInvoices = SalesReportRow.GetInvoices(DB, dbSalesReport.SalesReportID)
                End If
                m_dvInvoices = m_dtInvoices.DefaultView
                m_dvInvoices.Sort = "BuilderID"
            End If
            Return m_dvInvoices
        End Get
    End Property

    Private m_BuilderIds As Generic.List(Of Integer)
    Private ReadOnly Property BuilderIds() As Generic.List(Of Integer)
        Get
            If m_BuilderIds Is Nothing Then
                m_BuilderIds = New Generic.List(Of Integer)
                m_BuilderIds.AddRange(From row As DataRow In dtBuilders.AsEnumerable Select CInt(row("BuilderId")))
            End If
            Return m_BuilderIds
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

    Private Property InvoiceEditId() As Integer
        Get
            Return ViewState("InvoiceEditId")
        End Get
        Set(ByVal value As Integer)
            ViewState("InvoiceEditId") = value
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
            'mystate = Keys.Aggregate(Function(sum, append) sum & IIf(sum = String.Empty, append, "," & append))
            Dim s As New StringBuilder
            For Each k As String In Keys
                s.Append(IIf(s.Length = 0, k, "," & k))
            Next
            mystate = s.ToString
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
        EnsureVendorAccess()

        If Not Request("print") Is Nothing AndAlso Request("print").ToString.Trim <> String.Empty Then
            pnlPrint.Visible = False
            trLeftHeader.Visible = False
            trLeftColumn.Visible = False
        End If

        Dim sm As ScriptManager = ScriptManager.GetCurrent(Page)

        'acBuilders.WhereClause = "LLCID in (select LLCID from LLCVendor where VendorId=" & DB.Number(Session("VendorId")) & ")"

        sm.RegisterAsyncPostBackControl(hdnPostback)

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorId")
        UserName = Session("Username")

        If Not IsPostBack And Not sm.IsInAsyncPostBack Then

            Core.DataLog("Reporting", PageURL, CurrentUserId, "Vendor Menu Click", "", "", "", "", UserName)

            EditMode = True
            If ResponseDeadline < Now Then
                rblQuarter.Visible = False

                ltlCurrentQuarter.Visible = True
                ltlCurrentQuarter.Text = "Quarter " & CurrentQuarter & ", " & CurrentYear
                pSubmit.Visible = False

                ReportQuarter = CurrentQuarter
                ReportYear = CurrentYear
            Else
                Dim dbLastReport As SalesReportRow = SalesReportRow.GetSalesReportByPeriod(DB, Session("VendorID"), LastYear, LastQuarter)

                rblQuarter.Visible = True
                rblQuarter.Items.Add(New ListItem("Sales Invoiced During Quarter " & LastQuarter & ", " & LastYear, LastQuarter & "/" & LastYear))
                rblQuarter.Items.Add(New ListItem("Sales Invoiced During Quarter " & CurrentQuarter & ", " & CurrentYear, CurrentQuarter & "/" & CurrentYear))

                'If dbLastReport.Submitted <> Nothing Then
                '    ReportQuarter = CurrentQuarter
                '    ReportYear = CurrentYear
                '    rblQuarter.SelectedIndex = 1
                '    pSubmit.Visible = False
                '    ltlDeadline.Text = "(Deadline: " & FormatDateTime(ResponseDeadline, DateFormat.ShortDate) & ")"
                '    btnSubmit.Text = "Edit Quarter " & LastQuarter & " " & LastYear & " Sales Report"
                'Else
                '    'default to last qtr report if not submitted yet
                '    dbSalesReport = dbLastReport
                '    ReportQuarter = LastQuarter
                '    ReportYear = LastYear
                '    rblQuarter.SelectedIndex = 0
                '    pSubmit.Visible = True
                '    ltlDeadline.Text = "(Deadline: " & FormatDateTime(ResponseDeadline, DateFormat.ShortDate) & ")"
                '    btnSubmit.Text = "Submit Final Quarter " & LastQuarter & " " & LastYear & " Sales Report"
                'End If

                ReportQuarter = LastQuarter
                ReportYear = LastYear
                rblQuarter.SelectedIndex = 0
                pSubmit.Visible = True
                If dbLastReport.Submitted <> Nothing Then
                    ltlDeadline.Text = "(Deadline: " & FormatDateTime(ResponseDeadline.AddDays(-1), DateFormat.ShortDate) & ")"
                    btnSubmit.Text = "Edit Quarter " & LastQuarter & " " & LastYear & " Sales Report"
                    EditMode = False
                Else
                    ltlDeadline.Text = "(Deadline: " & FormatDateTime(ResponseDeadline.AddDays(-1), DateFormat.ShortDate) & ")"
                    btnSubmit.Text = "Submit Final Quarter " & LastQuarter & " " & LastYear & " Sales Report"
                    EditMode = True
                End If

                ltlCurrentQuarter.Visible = False
            End If

            EditIndex = -1

            BindUnreported()
            BindReported()
        End If

    End Sub

    Private Sub CleanReportZeros()
        DB.ExecuteSQL("Delete From SalesReportBuilderTotalAmount Where TotalAmount = 0 And SalesReportID = " & DB.Number(dbSalesReport.SalesReportID))
        BindUnreported()
        upUnreported.Update()
    End Sub

    Private Sub CleanInvoiceZeros()
        DB.ExecuteSQL("Delete From SalesReportBuilderInvoice Where InvoiceAmount = 0 And SalesReportID = " & DB.Number(dbSalesReport.SalesReportID))
    End Sub

    Private Function GetDeadline(ByVal QuarterEnd As DateTime) As DateTime
        'Dim deadline As DateTime = DateAdd(DateInterval.Day, 30, QuarterEnd)
        'Dim deadline As DateTime = SysParam.GetValue(DB, "CurrentReportingDeadline")
        'deadline = AddBusinessDays(deadline, SysParam.GetValue(DB, "ReportDeadlineDays"))
        'deadline = AddBusinessDays(deadline, SysParam.GetValue(DB, "DiscrepancyDeadlineDays"))
        'deadline = AddBusinessDays(deadline, SysParam.GetValue(DB, "DiscrepancyResponseDeadlineDays"))
        'Return deadline

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
        dtTotals = SalesReportRow.GetBuilders(DB, dbSalesReport.SalesReportID)
    End Sub

    Private Function UpdateInvoices(Optional ByVal BuilderID As Integer = Nothing) As Double
        m_dtInvoices = SalesReportRow.GetInvoices(DB, dbSalesReport.SalesReportID)
        m_dvInvoices = m_dtInvoices.DefaultView
        m_dvInvoices.Sort = "BuilderID"
        If m_dtInvoices.AsEnumerable.Count > 0 Then
            Dim sum As Double = 0
            For Each row As DataRow In m_dtInvoices.Rows
                If row("BuilderID") = BuilderID Then
                    sum += row("InvoiceAmount")
                End If
            Next
            Return sum
            'Dim q = m_dtInvoices.AsEnumerable.Where(Function(row) Core.GetInt(row("BuilderID")) = BuilderID).DefaultIfEmpty
            'If q IsNot Nothing AndAlso q.Count > 0 Then
            '    Return q.Select(Of Double)(Function(row) Core.GetDouble(row("InvoiceAmount"))).Aggregate(Function(sum, add) sum + add)
            'Else
            '    Return 0
            'End If
        Else
            Return 0
        End If

    End Function

    Protected Sub BindReported()
        Keys.Clear()
        Dim q = (From row As DataRow In dtTotals.AsEnumerable Join builder As DataRow In dtBuilders.AsEnumerable On row("BuilderId") Equals builder("BuilderId") Join id As Integer In BuilderIds On row("BuilderID") Equals id Select New With {.BuilderID = id, .TotalAmount = row("TotalAmount"), .CompanyName = builder("CompanyName")}).OrderBy(Function(key) key.CompanyName)
        If q.Count > 0 Then
            rptReported.DataSource = q
        End If
        rptReported.DataBind()
    End Sub

    Protected Sub BindUnreported()
        Dim q = (From builder As DataRow In dtBuilders.AsEnumerable Join id In BuilderIds On builder("BuilderID") Equals id Where Not (From total As DataRow In dtTotals.AsEnumerable Select total("BuilderId")).Contains(builder("BuilderId")) Select builder).OrderBy(Function(key) key("CompanyName"))

        If q.Count > 0 Then
            rptUnreported.DataSource = q
        End If
        rptUnreported.DataBind()
    End Sub

    Protected Sub hdnPostback_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles hdnPostback.ValueChanged
        'Dim json As New System.Web.Script.Serialization.JavaScriptSerializer()
        'Dim list As Object() = json.Deserialize(Of Object())(hdnPostback.Value)
        'BuilderIds.Clear()
        'For Each item As Generic.Dictionary(Of String, Object) In list
        '    BuilderIds.Add(item.Item("value"))
        'Next
        BuilderIds.Clear()
        Dim filter As String = txtBuilderFilter.Text
        BuilderIds.AddRange((From builder As DataRow In dtBuilders.AsEnumerable Where Left(builder("CompanyName"), Math.Min(CStr(builder("CompanyName")).Length, filter.Length)).ToLower = filter.ToLower Select builder("BuilderID")).Cast(Of Integer))
        'Dim dr As SqlClient.SqlDataReader = VendorRow.GetFilteredBuildersReader(DB, Session("VendorID"), txtBuilderFilter.Text)
        'While dr.Read
        '    BuilderIds.Add(dr("BuilderID"))
        'End While
        'dr.Close()

        BindReported()
        BindUnreported()
        upReported.Update()
        upUnreported.Update()
    End Sub

    Protected Sub rptUnreported_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptUnreported.ItemCommand
        Dim dbTotal As SalesReportBuilderTotalAmountRow = SalesReportBuilderTotalAmountRow.GetRow(DB, dbSalesReport.SalesReportID, e.CommandArgument)
        Dim txtTotal As TextBox = e.Item.FindControl("txtTotal")

        If BuilderIds.Contains(e.CommandArgument) Then
            If txtTotal.Text <> Nothing Then
                Dim cleaned As String = Regex.Replace(txtTotal.Text, "[^\d.]", "")
                If cleaned <> Nothing Then
                    If cleaned > 0 Then
                        dbTotal.TotalAmount = cleaned
                        If dbTotal.Created = Nothing Then
                            dbTotal.CreatorVendorAccountID = Session("VendorAccountId")
                            dbTotal.Insert()
                        End If
                        ltlUnreportedMsg.Text = ""
                        'log Select unreported Builder to submit report
                        Core.DataLog("Reporting", PageURL, CurrentUserId, "Select From Unreported Builder", dbTotal.BuilderID, "", "", "", UserName)
                        'end log
                    Else
                        ltlUnreportedMsg.Text = "<span class=""bold red"">There is no need to report values of $0.00.  If you did not sell anything this builder, please leave this field blank.</span>"
                    End If
                End If

            End If
        End If
        UpdateTotals()
        UpdateInvoices()
        BindReported()
        BindUnreported()
        upUnreported.Update()
        upReported.Update()
    End Sub

    Protected Sub rptUnreported_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptUnreported.ItemDataBound
        CType(e.Item.FindControl("ltlBuilder"), Literal).Text = e.Item.DataItem("CompanyName")
        Dim btn As Button = e.Item.FindControl("btnSaveBuilder")
        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btn)
    End Sub

    Protected Sub rblQuarter_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblQuarter.SelectedIndexChanged
        Dim period As String() = rblQuarter.SelectedValue.Split("/")
        ReportQuarter = period(0)
        ReportYear = period(1)

        'AR: This was causing duplicate sales report record. Was this even necesary? Looks pretty redundant.
        'dbSalesReport = SalesReportRow.GetSalesReportByPeriod(DB, Session("VendorId"), ReportYear, ReportQuarter)
        'If dbSalesReport.Created = Nothing Then
        '    dbSalesReport.PeriodYear = ReportYear
        '    dbSalesReport.PeriodQuarter = ReportQuarter
        '    dbSalesReport.VendorID = Session("VendorId")
        '    dbSalesReport.CreatorVendorAccountID = Session("VendorAccountId")
        '    dbSalesReport.Insert()
        'End If
        UpdateTotals()
        UpdateInvoices()

        If ResponseDeadline >= Now And ReportQuarter = LastQuarter Then
            pSubmit.Visible = True
            If dbSalesReport.Submitted = Nothing Then
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
                'log Edit Total Purchases from reported builder
                Dim dbtotal As SalesReportBuilderTotalAmountRow = SalesReportBuilderTotalAmountRow.GetRow(DB, dbSalesReport.SalesReportID, e.CommandArgument)
                BuilderId = dbtotal.BuilderID
                Dim Msgbdy As String = "Edit Reported Builder Id='" & BuilderId & "'"
                Core.DataLog("Reporting", PageURL, CurrentUserId, Msgbdy, dbSalesReport.SalesReportID, "", "", "", UserName)
                'end log 

            Case "Delete"
                Dim dbTotal As SalesReportBuilderTotalAmountRow = SalesReportBuilderTotalAmountRow.GetRow(DB, dbSalesReport.SalesReportID, e.CommandArgument)
                dbTotal.Remove()

                dvInvoices.RowFilter = "BuilderID=" & e.CommandArgument
                For Each row As DataRowView In dvInvoices
                    SalesReportBuilderInvoiceRow.RemoveRow(DB, row("SalesReportBuilderInvoiceId"))
                Next
                UpdateTotals()
                UpdateInvoices()
                BindUnreported()
                upUnreported.Update()

                'log delete vendor from reported vendor
                BuilderId = dbTotal.BuilderID
                Dim Msgbdy As String = "Delete Reported Builder Id='" & BuilderId & "'"
                Core.DataLog("Reporting", PageURL, CurrentUserId, Msgbdy, dbSalesReport.SalesReportID, "", "", "", UserName)
                'end log 

            Case "Save"
                Dim dbTotal As SalesReportBuilderTotalAmountRow = SalesReportBuilderTotalAmountRow.GetRow(DB, dbSalesReport.SalesReportID, e.CommandArgument)
                Dim Cleaned As Double = 0
                Cleaned = Regex.Replace(DirectCast(e.Item.FindControl("txtTotal"), TextBox).Text, "[^\d.]", "")
                If Cleaned > 0 Then
                    dbTotal.TotalAmount = Cleaned
                    If dbTotal.Created <> Nothing Then
                        dbTotal.Update()
                    Else
                        dbTotal.BuilderID = e.CommandArgument
                        dbTotal.CreatorVendorAccountID = Session("VendorAccountID")
                        dbTotal.SalesReportID = dbSalesReport.SalesReportID
                        dbTotal.Insert()
                    End If
                    UpdateTotals()
                    EditIndex = -1
                    ltlReportedMsg.Text = ""
                    'log Save Reported Builder 
                    BuilderId = dbTotal.BuilderID
                    Dim Msgbdy As String = "Save Reported Builder Id='" & BuilderId & "'"
                    Core.DataLog("Reporting", PageURL, CurrentUserId, Msgbdy, dbSalesReport.SalesReportID, "", "", "", UserName)
                    'end log 
                Else
                    ltlReportedMsg.Text = "<span class=""bold red"">There is no need to report values of $0.00.  If you did not sell anything to this builder, please enter a value or delete the record.</span>"
                End If

            Case "Cancel"
                EditIndex = -1

            Case "AddInvoice"
                Dim dbInvoice As New SalesReportBuilderInvoiceRow(DB)
                dbInvoice.SalesReportID = dbSalesReport.SalesReportID
                dbInvoice.BuilderID = e.CommandArgument
                dbInvoice.InvoiceAmount = 0
                dbInvoice.CreatorVendorAccountID = Session("VendorAccountId")
                dbInvoice.Insert()
                UpdateInvoices()
                InvoiceEditId = dbInvoice.SalesReportBuilderInvoiceID
                'log Save Reported Builder 
                BuilderId = dbInvoice.BuilderID
                Dim Msgbdy As String = "Save Reported Builder Id='" & BuilderId & "'"
                Core.DataLog("Reporting", PageURL, CurrentUserId, Msgbdy, dbSalesReport.SalesReportID, "", "", "", UserName)
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
        Keys.Add(DataBinder.Eval(e.Item.DataItem, "BuilderID"))
        Dim rptInvoices As Repeater = e.Item.FindControl("rptInvoices")
        AddHandler rptInvoices.ItemDataBound, AddressOf rptInvoices_ItemDataBound
        AddHandler rptInvoices.ItemCommand, AddressOf rptInvoices_ItemCommand
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
        Dim btnInvoices As HtmlInputButton = e.Item.FindControl("btnInvoices")
        Dim phNoInvoices As PlaceHolder = e.Item.FindControl("phNoInvoices")

        Dim rptInvoices As Repeater = e.Item.FindControl("rptInvoices")

        Dim divInvoices As HtmlGenericControl = e.Item.FindControl("divInvoices")
        Dim hdnInvoiceState As HiddenField = e.Item.FindControl("hdnInvoiceState")
        If Request(hdnInvoiceState.UniqueID) IsNot Nothing AndAlso Request(hdnInvoiceState.UniqueID) = "visible" Then
            divInvoices.Style.Remove("display")
            hdnInvoiceState.Value = "visible"
            btnInvoices.Value = "Hide Invoices"
        Else
            divInvoices.Style.Add("display", "none")
            hdnInvoiceState.Value = "hidden"
        End If

        dvInvoices.RowFilter = "BuilderID=" & DataBinder.Eval(e.Item.DataItem, "BuilderID")
        rptInvoices.DataSource = dvInvoices
        rptInvoices.DataBind()

        Dim InvoicesTotal As Double = 0
        If dvInvoices.Count > 0 Then
            For Each row As DataRowView In dvInvoices
                InvoicesTotal += Core.GetDouble(row("InvoiceAmount"))
            Next
            'InvoicesTotal = (From invoice As DataRowView In dvInvoices.OfType(Of DataRowView)() Select invoice("InvoiceAmount")).Aggregate(Function(a, b) a + b)
        End If
        Dim cInvoiceTotal As Double = Math.Round(InvoicesTotal, 2)
        Dim cTotal As Double = Math.Round(CDbl(txtTotal.Text), 2)
        If cInvoiceTotal > cTotal Then
            Dim div As HtmlGenericControl = e.Item.FindControl("divWarning")
            Dim msg As String = "Invoices for builder '" & DataBinder.Eval(e.Item.DataItem, "CompanyName") & "' exceed entered total."
            div.InnerHtml = msg
            div.Visible = True
            ScriptManager.RegisterArrayDeclaration(Page, "Warnings", """" & msg & """")
        End If
        phNoInvoices.Visible = (dvInvoices.Count = 0)

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
                If dvInvoices.Count = 0 Then
                    btnDelete.Visible = True
                    'btnInvoices.Value = "Add Invoices"
                Else
                    'btnInvoices.Value = "Show Invoices"
                    btnDelete.Visible = False
                End If
                btnSave.Visible = False
                btnCancel.Visible = False
            Else
                btnEdit.Visible = False
                btnDelete.Visible = False
                btnInvoices.Visible = dvInvoices.Count > 0
                btnSave.Visible = False
                btnCancel.Visible = False
            End If
            txtTotal.Visible = False
            spanTotal.Visible = True
        End If
    End Sub

    Protected Sub rptInvoices_ItemCommand(ByVal sender As Object, ByVal e As RepeaterCommandEventArgs)
        Dim BuilderIdx As Integer = CType(CType(sender, Repeater).NamingContainer, RepeaterItem).ItemIndex

        Select Case e.CommandName
            Case "Edit"
                InvoiceEditId = e.CommandArgument
                'log Edit Invoice from reported builder
                Dim dbInvoice As SalesReportBuilderInvoiceRow = SalesReportBuilderInvoiceRow.GetRow(DB, e.CommandArgument)
                BuilderId = dbInvoice.BuilderID
                Dim Msgbdy As String = "Edit Invoice For Builder Id='" & BuilderId & "'"
                Core.DataLog("Reporting", PageURL, CurrentUserId, Msgbdy, dbInvoice.SalesReportBuilderInvoiceID, "", "", "", UserName)
                'end log 

            Case "Delete"
                Dim dbInvoice As SalesReportBuilderInvoiceRow = SalesReportBuilderInvoiceRow.GetRow(DB, e.CommandArgument)
                dbInvoice.Remove()
                Dim dbTotal As SalesReportBuilderTotalAmountRow = SalesReportBuilderTotalAmountRow.GetRow(DB, dbInvoice.SalesReportID, Keys(BuilderIdx))
                dbTotal.TotalAmount = UpdateInvoices(dbInvoice.BuilderID)
                dbTotal.Update()
                'log Delete Invoice from reported builder
                BuilderId = dbInvoice.BuilderID
                Dim Msgbdy As String = "Delete Invoice For Builder Id='" & BuilderId & "'"
                Core.DataLog("Reporting", PageURL, CurrentUserId, Msgbdy, dbInvoice.SalesReportBuilderInvoiceID, "", "", "", UserName)
                'end log 

            Case "Save"
                Page.Validate(e.Item.UniqueID)
                If Not Page.IsValid Then
                    RenderErrors()
                    Exit Sub
                End If

                Dim Cleaned As Double = 0
                Cleaned = Regex.Replace(CType(e.Item.FindControl("txtAmount"), TextBox).Text, "[^\d.]", "")
                If Cleaned > 0 Then
                    Dim dbInvoice As SalesReportBuilderInvoiceRow = SalesReportBuilderInvoiceRow.GetRow(DB, e.CommandArgument)
                    dbInvoice.BuilderID = Keys(BuilderIdx)
                    dbInvoice.InvoiceAmount = Cleaned
                    dbInvoice.InvoiceNumber = CType(e.Item.FindControl("txtNumber"), TextBox).Text
                    dbInvoice.InvoiceDate = CType(e.Item.FindControl("dpDate"), Controls.DatePicker).Value
                    dbInvoice.SalesReportID = dbSalesReport.SalesReportID
                    If dbInvoice.SalesReportBuilderInvoiceID = Nothing Then
                        dbInvoice.CreatorVendorAccountID = Session("VendorAccountId")
                        dbInvoice.Insert()
                    Else
                        dbInvoice.Update()
                    End If

                    Dim dbTotal As SalesReportBuilderTotalAmountRow = SalesReportBuilderTotalAmountRow.GetRow(DB, dbInvoice.SalesReportID, Keys(BuilderIdx))
                    dbTotal.TotalAmount = UpdateInvoices(dbInvoice.BuilderID)
                    dbTotal.Update()

                    'log Save Invoice from builder in reported section
                    BuilderId = dbInvoice.BuilderID
                    Dim Msgbdy As String = "Save Invoice For Builder Id='" & BuilderId & "'"
                    Core.DataLog("Reporting", PageURL, CurrentUserId, Msgbdy, dbInvoice.SalesReportBuilderInvoiceID, "", "", "", UserName)
                    'end log 

                    InvoiceEditId = -1
                    ltlReportedMsg.Text = ""
                Else
                    ltlReportedMsg.Text = "<span class=""bold red"">There is no need to report values of $0.00.  If you did not sell anything to this builder, please enter a value or delete the record.</span>"
                End If


            Case "Cancel"
                InvoiceEditId = -1
        End Select
        CleanInvoiceZeros()
        CleanReportZeros()
        BindReported()
    End Sub

    Protected Sub rptInvoices_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs)
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

        Dim container As RepeaterItem = CType(sender, Control).NamingContainer
        btnEdit.Visible = EditMode
        btnSave.Visible = EditMode
        btnCancel.Visible = EditMode
        btnDelete.Visible = EditMode

        If Not IsDBNull(e.Item.DataItem("InvoiceDate")) Then
            dpDate.Value = e.Item.DataItem("InvoiceDate")
            ltlDate.Text = FormatDateTime(e.Item.DataItem("InvoiceDate"), DateFormat.ShortDate) & "<br/>"
        End If

        If e.Item.DataItem("SalesReportBuilderInvoiceID") = InvoiceEditId Then
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

        If Not EditMode Then
            SendEditNotices()
        End If

        ReportQuarter = LastQuarter
        ReportYear = LastYear
        dbSalesReport.Submitted = Nothing
        dbSalesReport.SubmitterVendorAccountID = Nothing
        dbSalesReport.Update()

        btnSubmit.Text = "Submit Final Quarter " & LastQuarter & " " & LastYear & " Sales Report"
        EditMode = True

        'log edit Submitted Sales Report
        Core.DataLog("Reporting", PageURL, CurrentUserId, "Edit Submitted Sales Report", dbSalesReport.SalesReportID, "", "", "", UserName)
        'end log

        UpdateTotals()
        UpdateInvoices()
        BindReported()
        BindUnreported()
    End Sub

    Private Sub SendEditNotices()
        'send emails here

        'Send SalesReportRetracted.

        Dim dbAutoMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "SalesReportRetracted")
        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, Session("VendorId"))
        Dim MsgBody As String = String.Empty
        MsgBody = dbVendor.CompanyName & " retracted the " & dbSalesReport.PeriodYear & " Q" & dbSalesReport.PeriodQuarter & " report. TotalAmount: " & FormatCurrency(TotalAmount)
        'dbAutoMsg.Send(dbVendor, MsgBody)
        dbAutoMsg.SendAdmin(MsgBody)
    End Sub

    Protected Sub frmConfirm_Callback(ByVal sender As Object, ByVal args As PopupForm.PopupFormEventArgs) Handles frmConfirm.Callback
        If frmConfirm.IsValid Then
            Dim ret As String = String.Empty
            Dim Err As String = String.Empty
            Dim dt As DataTable = DB.GetDataTable("Select s.TotalAmount, b.CompanyName From SalesReportBuilderTotalAmount s, Builder b Where s.BuilderId = b.BuilderId And s.SalesReportID = " & DB.Number(dbSalesReport.SalesReportID))
            For Each dr As DataRow In dt.Rows
                If dr("TotalAmount") = 0 Then
                    Err &= "<li>" & dr("CompanyName")
                End If
            Next
            If Err <> String.Empty Then
                Err = "<br><br><p>You have reported $0 for the following builder(s):</p>" & Err & "<br><br><p>Please correct this by either entering values for the Total Amount or deleting the record from the Reported Builders section.</p>"
                ret = "<div class=""red"" style=""padding:25px;font-weight:bold;"">There were errors and your sales report could not be submitted! "
                ret &= Err
                ret &= "<br /><br /><center><input type=""button"" value=""Go Back"" onclick=""location.href='vendor-sales.aspx';"" class=""btnred"" /></center>"
                frmConfirm.CallbackResult = ret
                Exit Sub
            End If
            dbSalesReport.Submitted = Now
            dbSalesReport.SubmitterVendorAccountID = Session("VendorAccountID")
            dbSalesReport.Update()

            'Sync with insightly
            SyncSalesReportSubmittedStatus(Session("VendorId"))

            'log Submit Sales Report
            Core.DataLog("Reporting", PageURL, CurrentUserId, "Submit Sales Report", dbSalesReport.SalesReportID, "", "", "", UserName)
            'end log

            BindReported()

            Dim dbAutoMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "SalesReportSubmittedToAdmins")
            Dim dbVendor As VendorRow = VendorRow.GetRow(DB, Session("VendorId"))
            Dim MsgBody As String = String.Empty
            MsgBody = dbVendor.CompanyName & " submitted the " & dbSalesReport.PeriodYear & " Q" & dbSalesReport.PeriodQuarter & " report. TotalAmount: " & FormatCurrency(TotalAmount) & vbCrLf & vbCrLf & System.Configuration.ConfigurationManager.AppSettings("GlobalSecureName") & "/admin/"
            dbAutoMsg.SendAdmin(MsgBody)

            ret = "<div style=""padding:25px;font-weight:bold;text-align:center;"">Sales report submitted for Quarter " & ReportQuarter & ", " & ReportYear
            ret &= "<br /><br /><input type=""button"" value=""Continue"" onclick=""location.href='vendor-sales.aspx';"" class=""btnred"" />"
            frmConfirm.CallbackResult = ret
        End If
    End Sub

    Protected Sub frmConfirm_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmConfirm.TemplateLoaded
        'UpdateConfirmMessage()
    End Sub

    Private Sub UpdateConfirmMessage()
        Dim q = From builder As DataRow In dtBuilders.AsEnumerable Group Join total As DataRow In dtTotals.AsEnumerable On builder("BuilderID") Equals total("BuilderID") Into totals = Group _
                From total In totals.DefaultIfEmpty Select New With {.builder = builder("CompanyName"), .total = total}

        frmConfirm.EnsureChildrenCreated()

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
                reported &= "<td>" & o.builder & "</td><td>" & FormatCurrency(o.total("TotalAmount")) & "</td></tr>"
                reportedCnt += 1
            Else
                If unreportedCnt Mod 2 = 1 Then
                    unreported &= "<tr>"
                Else
                    unreported &= "<tr class=""alt"">"
                End If
                unreported &= "<td>" & o.builder & "</td>"
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
            btnSubmit.OnClientClick = "return OpenConfirm();"
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
        For Each item As RepeaterItem In rptUnreported.Items
            Dim btnSave As Button = item.FindControl("btnSaveBuilder")
            Dim txtTotal As TextBox = item.FindControl("txtTotal")
            If BuilderIds.Contains(btnSave.CommandArgument) Then
                If txtTotal.Text <> String.Empty Then
                    Dim cleaned As String = Regex.Replace(txtTotal.Text, "[^\d.]", "")
                    If cleaned <> Nothing Then
                        If cleaned > 0 Then
                            Dim dbRow As SalesReportBuilderTotalAmountRow = SalesReportBuilderTotalAmountRow.GetRow(DB, dbSalesReport.SalesReportID, btnSave.CommandArgument)
                            dbRow.TotalAmount = cleaned
                            dbRow.BuilderID = btnSave.CommandArgument
                            dbRow.SalesReportID = dbSalesReport.SalesReportID
                            If dbRow.Created = Nothing Then
                                dbRow.CreatorVendorAccountID = Session("VendorAccountId")
                                dbRow.Insert()
                            Else
                                dbRow.Update()
                            End If
                        Else
                            ltlUnreportedMsg.Text = "<span class=""bold red"">There is no need to report values of $0.00.  If you did not sell anything to this builder, please leave this field blank.</span>"
                        End If
                    End If
                End If
            End If
        Next
        'log Btn save all clicked
        Core.DataLog("Reporting", PageURL, CurrentUserId, "Btn Save All", "", "", "", "", UserName)
        'end log
        BindUnreported()
        BindReported()
        upReported.Update()
    End Sub

    Protected Sub btnHistory_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnHistory.Click
        'log Btn Sales Report History
        Core.DataLog("Reporting", PageURL, CurrentUserId, "Btn Sales Report History", "", "", "", "", UserName)
        'end log
        Response.Redirect("sales-history.aspx")
    End Sub

    'Protected Sub btnDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnDashBoard.Click
    'Response.Redirect("/builder/default.aspx")
    'End Sub

    Private Sub SyncSalesReportSubmittedStatus(ByVal VendorID As String)

        Dim SQL As String = "SELECT CRMID FROM VENDOR WHERE VENDORID=" & VendorID & ""
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
