Imports Components
Imports DataLayer
Imports PopupForm
Imports System.Linq
Imports System.Data
Imports System.Web.Services

Partial Class rebates_discrepancy_report
    Inherits SitePage

    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    'Sysparams are a bit crowded, but didn't want to use straight strings for this everywhere.
    Public Const NoDisputeDefault As String = "Not disputed."
    Public Const NoResponseDefault As String = "Dispute filed."

    Private ReadOnly Property LastQuarter() As Integer
        Get
            Dim ret As Integer = Math.Ceiling(Now.Month / 3) - 1
            Return IIf(ret = 0, 4, ret)
        End Get
    End Property

    Private ReadOnly Property LastYear() As Integer
        Get
            Return IIf(LastQuarter = 4, Now.Year - 1, Now.Year)
        End Get
    End Property

    Private m_Deadline As DateTime
    Private ReadOnly Property Deadline() As DateTime
        Get
            If m_Deadline = Nothing Then
                Dim LastQuarterEnd As DateTime = (LastQuarter * 3) & "/" & Date.DaysInMonth(LastYear, LastQuarter * 3) & "/" & LastYear
                m_Deadline = LastQuarterEnd.AddMonths(1).AddDays(SysParam.GetValue(DB, "ReportDeadlineDays") + 1).AddDays(SysParam.GetValue(DB, "DiscrepancyDeadlineDays")).AddDays(SysParam.GetValue(DB, "DiscrepancyResponseDeadlineDays"))
            End If
            Return m_Deadline
        End Get
    End Property

    Private m_DisputeDeadline As DateTime
    Private ReadOnly Property DisputeDeadline() As DateTime
        Get
            If m_DisputeDeadline = Nothing Then
                Dim LastQuarterEnd As DateTime = (LastQuarter * 3) & "/" & Date.DaysInMonth(LastYear, LastQuarter * 3) & "/" & LastYear
                m_DisputeDeadline = LastQuarterEnd.AddMonths(1).AddDays(SysParam.GetValue(DB, "ReportDeadlineDays") + 1).AddDays(SysParam.GetValue(DB, "DiscrepancyDeadlineDays"))
            End If
            Return m_DisputeDeadline
        End Get
    End Property

    Private m_ReportDeadline As DateTime
    Private ReadOnly Property ReportDeadline() As DateTime
        Get
            If m_ReportDeadline = Nothing Then
                Dim LastQuarterEnd As DateTime = (LastQuarter * 3) & "/" & Date.DaysInMonth(LastYear, LastQuarter * 3) & "/" & LastYear
                m_ReportDeadline = LastQuarterEnd.AddMonths(1).AddDays(SysParam.GetValue(DB, "ReportDeadlineDays") + 1)
            End If
            Return m_ReportDeadline
        End Get
    End Property

    Private m_dvInvoices As DataView
    Private ReadOnly Property dvInvoices() As DataView
        Get
            If m_dvInvoices Is Nothing Then
                m_dvInvoices = SalesReportBuilderInvoiceRow.GetAllBuilderInvoices(DB, Session("BuilderId"), LastQuarter, LastYear).DefaultView
            End If
            Return m_dvInvoices
        End Get
    End Property

    Private m_dvPurchases As DataView
    Private ReadOnly Property dvPurchases() As DataView
        Get
            If m_dvPurchases Is Nothing Then
                m_dvPurchases = PurchasesReportVendorPORow.GetAllVendorPOs(DB, Session("BuilderId"), LastQuarter, LastYear).DefaultView
            End If
            Return m_dvPurchases
        End Get
    End Property

    Private m_dtDisputeResponse As DataTable
    Private ReadOnly Property dtDisputeResponse() As DataTable
        Get
            If m_dtDisputeResponse Is Nothing Then
                m_dtDisputeResponse = DisputeResponseRow.GetList(DB)
            End If
            Return m_dtDisputeResponse
        End Get
    End Property

    Private m_dtDisputeResponseReason As DataTable
    Private ReadOnly Property dtDisputeResponseReason() As DataTable
        Get
            If m_dtDisputeResponseReason Is Nothing Then
                m_dtDisputeResponseReason = DisputeResponseReasonRow.GetList(DB)
            End If
            Return m_dtDisputeResponseReason
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureBuilderAccess()
        If ReportDeadline > Now.Date Then
            frmDeadline.EnsureChildrenCreated()
            ltlDeadline.Text = "<b>The reporting period has not closed yet.  Disputes can be added starting " & FormatDateTime(ReportDeadline, DateFormat.ShortDate)
            ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenDeadline", "Sys.Application.add_load(OpenDeadline);", True)
        ElseIf Deadline < Now.Date Then
            frmDeadline.EnsureChildrenCreated()
            ltlDeadline.Text = "<b>The dispute deadline for the current reporting period has passed.</b>"
            ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenDeadline", "Sys.Application.add_load(OpenDeadline);", True)

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")

        ElseIf Not IsPostBack And Not ScriptManager.GetCurrent(Page).IsInAsyncPostBack Then
        Core.DataLog("Discrepancy Report", PageURL, CurrentUserId, "Left Menu Click", "", "", "", "", UserName)
            BindReports()
        End If

    End Sub

    Private Sub BindReports()
        rptReport.DataSource = PurchasesReportRow.GetDiscrepancyReport(DB, Session("BuilderId"), LastQuarter, LastYear)
        rptReport.DataBind()
    End Sub

    Protected Sub frmDispute_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim form As PopupForm.PopupForm = CType(sender, PopupForm.PopupForm)
        Dim rptItem As RepeaterItem = CType(sender, Control).NamingContainer
        If rptItem.DataItem IsNot Nothing Then
            'hdnSalesReportId.Value = rptItem.DataItem("SalesReportID")
            CType(form.FindControl("hdnVendorId"), HiddenField).Value = rptItem.DataItem("VendorID")
            CType(form.FindControl("ltlVendor"), Literal).Text = rptItem.DataItem("VendorCompany")
            If Not IsDBNull(rptItem.DataItem("VendorTotal")) Then
                CType(form.FindControl("ltlVendorAmount"), Literal).Text = FormatCurrency(rptItem.DataItem("VendorTotal"))
            End If
            CType(form.FindControl("txtBuilderAmount"), TextBox).Text = FormatCurrency(rptItem.DataItem("BuilderTotal"))
        End If
        CType(form.FindControl("btnClose"), Button).OnClientClick = "CloseDisputeForm('" & form.ClientID & "');return false;"
    End Sub

    Protected Sub frmDispute_Callback(ByVal sender As Object, ByVal e As PopupFormEventArgs)
        Dim form As PopupForm.PopupForm = CType(sender, PopupForm.PopupForm)
        If Not form.IsValid Then Exit Sub

        Dim VendorID As Integer = e.Data("hdnVendorID")

        Dim BuilderAmount As Double = e.Data("txtBuilderAmount")
        Dim dbSalesReport As SalesReportRow = SalesReportRow.GetSalesReportByPeriod(DB, VendorID, LastYear, LastQuarter)
        If dbSalesReport.SalesReportID = Nothing Then
            dbSalesReport.CreatorVendorAccountID = -1
            dbSalesReport.PeriodQuarter = LastQuarter
            dbSalesReport.PeriodYear = LastYear
            dbSalesReport.VendorID = VendorID
            dbSalesReport.Insert()
        End If

        Dim dbVendorReport As SalesReportBuilderTotalAmountRow = SalesReportBuilderTotalAmountRow.GetRow(DB, dbSalesReport.SalesReportID, Session("BuilderId"))

        Dim dbDispute As SalesReportDisputeRow = SalesReportDisputeRow.GetRowBySalesReportIdAndBuilderId(DB, dbSalesReport.SalesReportID, Session("BuilderId"))
        dbDispute.BuilderID = Session("BuilderId")
        dbDispute.BuilderComments = e.Data("txtComments")
        dbDispute.BuilderTotalAmount = BuilderAmount
        dbDispute.SalesReportID = IIf(dbSalesReport.SalesReportID = Nothing, -1, dbSalesReport.SalesReportID)
        dbDispute.VendorTotalAmount = dbVendorReport.TotalAmount
        If dbDispute.Created = Nothing Then
            dbDispute.Insert()
            'log Update Dispute
            Core.DataLog("Discrepancy Report", PageURL, CurrentUserId, "Submit Dispute", dbDispute.SalesReportDisputeID, "", "", "", UserName)
            'end log
        Else
            dbDispute.Update()
            'log Update Dispute
            Core.DataLog("Discrepancy Report", PageURL, CurrentUserId, "Update Dispute", dbDispute.SalesReportDisputeID, "", "", "", UserName)
            'end log
        End If

        'Send DisputeSubmitted to Vendor and Quarterly Reporter.

        Dim dbAutoMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "DisputeSubmitted")
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, VendorID)
        Dim QuarterlyReporter As String = String.Empty
        Dim dt As DataTable = DB.GetDataTable("Select va.Email From VendorAccount va, VendorAccountVendorRole vr Where va.VendorAccountId = vr.VendorAccountId And vr.VendorRoleId = 2 And va.VendorId = " & DB.Number(VendorID))
        If dt.Rows.Count > 0 Then
            For Each dr As DataRow In dt.Rows
                If Not IsDBNull(dr("Email")) Then
                    QuarterlyReporter = dr("Email")
                End If
            Next
        End If

        If QuarterlyReporter <> String.Empty AndAlso QuarterlyReporter <> dbVendor.Email Then
            If dbAutoMsg.CCList <> String.Empty Then
                dbAutoMsg.CCList &= "," & QuarterlyReporter
            Else
                dbAutoMsg.CCList &= QuarterlyReporter
            End If
        End If

        Dim MsgBody As String = String.Empty
        MsgBody = dbBuilder.CompanyName & " has disputed the reported sales amount on your " & LastYear & " Q" & LastQuarter & " report. Please review your discrepancy report and respond to the dispute: " & vbCrLf & vbCrLf & System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/rebates/discrepancy-response.aspx"
        dbAutoMsg.Send(dbVendor, MsgBody)

        'Send DisputeSubmittedToAdmin.

        dbAutoMsg = AutomaticMessagesRow.GetRowByTitle(DB, "DisputeSubmittedToAdmin")
        MsgBody = dbBuilder.CompanyName & " has disputed the reported sales amount on " & dbVendor.CompanyName & "'s " & LastYear & " Q" & LastQuarter & " report." & vbCrLf & vbCrLf & System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/admin/"

        dbAutoMsg.SendAdmin(MsgBody)

        BindReports()
    End Sub

    Protected Sub rptReport_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptReport.ItemCreated
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        Dim form As PopupForm.PopupForm = e.Item.FindControl("frmDispute")
        AddHandler form.TemplateLoaded, AddressOf frmDispute_TemplateLoaded
        AddHandler form.Callback, AddressOf frmDispute_Callback

        Dim rptInvoices As Repeater = e.Item.FindControl("rptInvoices")
        If e.Item.DataItem IsNot Nothing Then
            If Not IsDBNull(e.Item.DataItem("VendorId")) Then
                dvInvoices.RowFilter = "VendorId=" & e.Item.DataItem("VendorId")
                If dvInvoices.Count > 0 Then
                    rptInvoices.DataSource = dvInvoices
                    rptInvoices.DataBind()
                Else
                    e.Item.FindControl("phNoInvoices").Visible = True
                End If
            End If

            Dim rptPurchases As Repeater = e.Item.FindControl("rptPurchases")
            dvPurchases.RowFilter = "VendorId=" & e.Item.DataItem("VendorId")
            If dvPurchases.Count > 0 Then
                rptPurchases.DataSource = dvPurchases
                rptPurchases.DataBind()
            Else
                e.Item.FindControl("phNoPurchases").Visible = False
            End If
        End If
    End Sub

    Dim comma As String = String.Empty

    Protected Sub rptReport_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptReport.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        ViewState("VendorId") &= comma & e.Item.DataItem("VendorId")
        comma = ","

        Dim ltlVendorTotal As Literal = e.Item.FindControl("ltlVendorTotal")
        Dim ltlBuilderTotal As Literal = e.Item.FindControl("ltlBuilderTotal")
        Dim ltlDifference As Literal = e.Item.FindControl("ltlDifference")
        Dim ltlVendorAcceptedTotal As Literal = e.Item.FindControl("ltlVendorAcceptedTotal")
        Dim ltlBuilderProposedTotal As Literal = e.Item.FindControl("ltlBuilderProposedTotal")
        Dim ltlComments As Literal = e.Item.FindControl("ltlComments")

        ltlBuilderProposedTotal.Text = FormatCurrency(IIf(Not IsDBNull(e.Item.DataItem("BuilderTotalAmount")), e.Item.DataItem("BuilderTotalAmount"), 0))

        'Returns DNR if one side did not report
        If Not IsDBNull(e.Item.DataItem("PurchasesReportID")) Then
            Dim pr As PurchasesReportRow = PurchasesReportRow.GetRow(DB, e.Item.DataItem("PurchasesReportID"))
            ltlBuilderTotal.Text = pr.GetReportedPurchases(e.Item.DataItem("VendorID"))
        Else
            ltlBuilderTotal.Text = "DNR"
        End If

        If Not IsDBNull(e.Item.DataItem("SalesReportID")) Then
            Dim sr As SalesReportRow = SalesReportRow.GetRow(DB, e.Item.DataItem("SalesReportID"))
            ltlVendorTotal.Text = sr.GetReportedSales(Session("BuilderId"))
        Else
            ltlVendorTotal.Text = "DNR"
        End If


        If IsDBNull(e.Item.DataItem("VendorTotal")) Or IsDBNull(e.Item.DataItem("BuilderTotal")) Then
            ltlDifference.Text = "DNR"
        Else
            Dim diff As Double = e.Item.DataItem("VendorTotal") - e.Item.DataItem("BuilderTotal")
            ltlDifference.Text = FormatCurrency(diff)
        End If


        If Not IsDBNull(e.Item.DataItem("CreatorVendorAccountId")) Then
            If e.Item.DataItem("CreatorVendorAccountId") = -1 And Not IsDBNull(e.Item.DataItem("SalesReportDisputeID")) Then
                ltlComments.Text = "Vendor did not report, dispute automatically filed"
            End If
        End If

        Dim rptInvoices As Repeater = e.Item.FindControl("rptInvoices")
        Dim rptPurchases As Repeater = e.Item.FindControl("rptPurchases")
        If rptInvoices.Items.Count = 0 And rptPurchases.Items.Count = 0 Then
            e.Item.FindControl("btnDetails").Visible = False
        End If

        Dim divDetails As HtmlGenericControl = e.Item.FindControl("divDetails")
        divDetails.Style("display") = "none"

        Dim btnDispute As Button = e.Item.FindControl("btnDispute")

        Dim strControlIndex as string = btnDispute.ClientID.Replace("_btnDispute", "").Replace("rptReport_", "")
	Dim strFormId as string = "rptReport_" & strControlIndex & "_frmDispute_window"
        btnDispute.OnClientClick = "javascript:return ShowDisputeForm('" & strFormId & "');"

        Dim form As PopupForm.PopupForm = e.Item.FindControl("frmDispute")
        Dim btnCancel As Button = form.FindControl("btnCancel")
        btnCancel.OnClientClick = "javascript:return HideDisputeForm('rptReport_frmDispute_" & e.Item.ItemIndex & "_window_" & e.Item.ItemIndex & "');"

        Dim btnAccept As Button = e.Item.FindControl("btnAccept")
        Dim btnCancelDispute As Button = e.Item.FindControl("btnCancelDispute")

        Dim ltlDisputeResponse As Literal = e.Item.FindControl("ltlDisputeResponse")
        If IsDBNull(e.Item.DataItem("VendorTotal")) Then
            btnDispute.Style("display") = "none"
            btnCancelDispute.Style("display") = "none"
            ltlDisputeResponse.Visible = False
        Else
            If DisputeDeadline < Now Then
                btnDispute.Style("display") = "none"
                btnCancelDispute.Style("display") = "none"
                ltlDisputeResponse.Text = NoDisputeDefault
            End If

            If IsDBNull(e.Item.DataItem("SalesReportDisputeID")) Then
                If Not IsDBNull(e.Item.DataItem("Modified")) Then btnAccept.Style("display") = "none"
                btnCancelDispute.Style("display") = "none"
                ltlDisputeResponse.Text = NoDisputeDefault
            Else
                btnAccept.Style("display") = "none"
                Page.ClientScript.RegisterArrayDeclaration("SalesReportDisputeIds", "{'btnId':'" & btnCancelDispute.ClientID & "','disputeId':'" & e.Item.DataItem("SalesReportDisputeID") & "'}")
                If IsDBNull(e.Item.DataItem("DisputeResponseID")) Then
                    btnDispute.Style("display") = "none"
                    ltlDisputeResponse.Text = NoResponseDefault
                Else
                    Dim response As String = (From row As DataRow In dtDisputeResponse.AsEnumerable Where Core.GetInt(row("DisputeResponseID")) = Core.GetInt(e.Item.DataItem("DisputeResponseID")) Select row("DisputeResponse")).FirstOrDefault
                    Dim reason As String = (From row As DataRow In dtDisputeResponseReason.AsEnumerable Where Core.GetInt(row("DisputeResponseReasonID")) = Core.GetInt(e.Item.DataItem("DisputeResponseReasonID")) Select row("DisputeResponseReason")).FirstOrDefault


                    ltlDisputeResponse.Text = "<p style=""text-align:center;"">"
                    Select Case response
                        Case "Refuse"
                            ltlDisputeResponse.Text &= " Vendor refused your dispute for the following reason: "
                            ltlVendorAcceptedTotal.Text = FormatCurrency(e.Item.DataItem("VendorTotalAmount"))
                        Case "Accept"
                            ltlDisputeResponse.Text &= " Vendor accepted your original reported amount: "
                        Case "Accept New Amount"
                            ltlDisputeResponse.Text &= " Vendor accepted the following new amount: "
                    End Select
                    'ltlDisputeResponse.Text &= "<b>" & response & "</b>"
                    If Not IsDBNull(e.Item.DataItem("ResolutionAmount")) Then
                        ltlDisputeResponse.Text &= "<br/>&nbsp;" & FormatCurrency(e.Item.DataItem("ResolutionAmount"))
                        ltlVendorAcceptedTotal.Text = FormatCurrency(e.Item.DataItem("ResolutionAmount"))
                    End If
                    ltlDisputeResponse.Text &= "<br/><span class=""smaller"">" & reason & "</span></p>"
                    If Not IsDBNull(e.Item.DataItem("BuilderComments")) Then
                        ltlDisputeResponse.Text &= "<p class=""smaller""><b>Builder Comments:</b><br/>" & e.Item.DataItem("BuilderComments") & "</p>"
                    End If
                    If Not IsDBNull(e.Item.DataItem("VendorComments")) Then
                        ltlDisputeResponse.Text &= "<p class=""smaller""><b>Vendor Comments:</b><br/>" & e.Item.DataItem("VendorComments") & "</p>"
                    End If
                    ltlDisputeResponse.Visible = True
                    btnDispute.Visible = False
                    btnCancelDispute.Visible = False
                End If
            End If
        End If
        'Hide accept button
        ' btnAccept.Style("display") = "none"
    End Sub

    <WebMethod(EnableSession:=True)> _
    Public Shared Function CancelDispute(ByVal SalesReportDisputeID As Integer) As Boolean
        Dim dbDispute As SalesReportDisputeRow = SalesReportDisputeRow.GetRow(Utility.GlobalDB.DB, SalesReportDisputeID)
        dbDispute.Remove()

        Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(Utility.GlobalDB.DB, "DisputeCanceled")
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(Utility.GlobalDB.DB, HttpContext.Current.Session("BuilderId"))
        Dim dbReport As SalesReportRow = SalesReportRow.GetRow(Utility.GlobalDB.DB, dbDispute.SalesReportID)
        Dim dbVendor As VendorRow = VendorRow.GetRow(Utility.GlobalDB.DB, dbReport.VendorID)
        Dim msg As String = dbBuilder.CompanyName & " has cancelled the dispute on your " & dbReport.PeriodYear & " Q" & dbReport.PeriodQuarter & " report."
        dbMsg.Send(dbVendor, msg)

        dbMsg = AutomaticMessagesRow.GetRowByTitle(Utility.GlobalDB.DB, "DisputeCanceledToAdmins")
        msg = dbBuilder.CompanyName & " has cancelled the dispute on " & dbVendor.CompanyName & " Q" & dbReport.PeriodQuarter & " report."
        dbMsg.SendAdmin(msg)
        Return True
    End Function

    Protected Sub frmAcceptConfirm_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmAcceptConfirm.Postback
        Dim SalesReportID As Integer = Nothing
        Try
            SalesReportID = hdnAcceptReportID.Value
        Catch ex As Exception
        End Try

        Dim DisputeID As String = hdnAcceptDisputeID.Value

        Dim dbPurchasesReport As PurchasesReportRow = PurchasesReportRow.GetPurchasesReportByPeriod(DB, Session("BuilderId"), LastYear, LastQuarter)
        Dim dbSalesReport As SalesReportRow = SalesReportRow.GetRow(DB, SalesReportID)
        Dim dbVendorTotalPurchases As PurchasesReportVendorTotalAmountRow = PurchasesReportVendorTotalAmountRow.GetRow(DB, dbPurchasesReport.PurchasesReportID, dbSalesReport.VendorID)
        Dim dbVendorTotalSales As SalesReportBuilderTotalAmountRow = SalesReportBuilderTotalAmountRow.GetRow(DB, dbSalesReport.SalesReportID, Session("BuilderId"))

        Dim previoustotal As Double = dbVendorTotalPurchases.TotalAmount
        dbVendorTotalPurchases.TotalAmount = dbVendorTotalSales.TotalAmount


        If dbVendorTotalPurchases.Created = Nothing Then
            If dbPurchasesReport.PurchasesReportID = Nothing Then
                dbPurchasesReport.BuilderID = Session("BuilderId")
                dbPurchasesReport.CreatorBuilderAccountID = Session("BuilderAccountId")
                dbPurchasesReport.PeriodQuarter = LastQuarter
                dbPurchasesReport.PeriodYear = LastYear
                dbPurchasesReport.SubmitterBuilderAccountID = Session("BuilderAccountId")
                dbPurchasesReport.Insert()
            End If

            Dim BuilderInitialTotal As Object

            If Not IsDBNull(dbPurchasesReport.PurchasesReportID) Then
                Dim pr As PurchasesReportRow = PurchasesReportRow.GetRow(DB, dbPurchasesReport.PurchasesReportID)
                BuilderInitialTotal = pr.GetReportedPurchases(dbSalesReport.VendorID, True)
            End If

            dbVendorTotalPurchases.CreatorBuilderAccountID = Session("BuilderAccountId")
            dbVendorTotalPurchases.PurchasesReportID = dbPurchasesReport.PurchasesReportID
            dbVendorTotalPurchases.VendorID = dbSalesReport.VendorID

            If BuilderInitialTotal.ToString() = "DNR" Then
                BuilderInitialTotal = 0.00
            Else
                BuilderInitialTotal = Convert.ToDouble(BuilderInitialTotal.ToString().Replace("$", ""))
            End If

            dbVendorTotalPurchases.BuilderReportedInitialTotal = BuilderInitialTotal 'IIf(BuilderInitialTotal.ToString() = "DNR", 0.00, )
            dbVendorTotalPurchases.Insert()
            'log Accpet Dispute
            Core.DataLog("Discrepancy Report", PageURL, CurrentUserId, "Accpet Dispute", dbVendorTotalPurchases.PurchasesReportID, "", "", "", UserName)
            'end log
        Else
            dbVendorTotalPurchases.BuilderReportedInitialTotal = previoustotal
            dbVendorTotalPurchases.Modified = Date.Now
            dbVendorTotalPurchases.ModifyBuilderAccountID = Session("BuilderAccountId")
            dbVendorTotalPurchases.Update()
            'log Update Dispute
            Core.DataLog("Discrepancy Report", PageURL, CurrentUserId, "Update Dispute", dbVendorTotalPurchases.PurchasesReportID, "", "", "", UserName)
            'end log
        End If

        If DisputeID <> String.Empty Then
            Dim dbDispute As SalesReportDisputeRow = SalesReportDisputeRow.GetRow(DB, DisputeID)
            dbDispute.ResolutionAmount = dbVendorTotalSales.TotalAmount
            dbDispute.Update()
        End If


        BindReports()
    End Sub

    Protected Sub frmCancelConfirm_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmCancelConfirm.Postback
        If hdnCancelDisputeID.Value = String.Empty Then Exit Sub
        Dim dbDispute As SalesReportDisputeRow = SalesReportDisputeRow.GetRow(DB, hdnCancelDisputeID.Value)

        Dim VendorId As Integer = SalesReportRow.GetRow(DB, dbDispute.SalesReportID).VendorID

        dbDispute.Remove()

        'log Cancel Dispute
        Core.DataLog("Discrepancy Report", PageURL, CurrentUserId, "Cancel Dispute", dbDispute.SalesReportDisputeID, "", "", "", UserName)
        'end log

        'Send DisputeCanceled to Vendor and Quarterly Reporter.

        Dim dbAutoMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "DisputeCanceled")
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, VendorId)
        Dim QuarterlyReporter As String = String.Empty
        Dim dt As DataTable = DB.GetDataTable("Select va.Email From VendorAccount va, VendorAccountVendorRole vr Where va.VendorAccountId = vr.VendorAccountId And vr.VendorRoleId = 2 And va.VendorId = " & DB.Number(VendorId))
        If dt.Rows.Count > 0 Then
            For Each dr As DataRow In dt.Rows
                If Not IsDBNull(dr("Email")) Then
                    QuarterlyReporter = dr("Email")
                End If
            Next
        End If

        If QuarterlyReporter <> String.Empty AndAlso QuarterlyReporter <> dbVendor.Email Then
            If dbAutoMsg.CCList <> String.Empty Then
                dbAutoMsg.CCList &= "," & QuarterlyReporter
            Else
                dbAutoMsg.CCList &= QuarterlyReporter
            End If
        End If

        Dim MsgBody As String = String.Empty
        MsgBody = dbBuilder.CompanyName & " has canceled the dispute on your " & LastYear & " Q" & LastQuarter & " report." & vbCrLf & vbCrLf & System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/rebates/discrepancy-response.aspx"
        dbAutoMsg.Send(dbVendor, MsgBody)

        'Send DisputeCanceledToAdmin.

        dbAutoMsg = AutomaticMessagesRow.GetRowByTitle(DB, "DisputeCanceledToAdmins")
        MsgBody = dbBuilder.CompanyName & " has canceled the dispute on " & dbVendor.CompanyName & "'s " & LastYear & " Q" & LastQuarter & " report." & vbCrLf & vbCrLf & System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/admin/"

        dbAutoMsg.Send(dbVendor, MsgBody)

        BindReports()
    End Sub
End Class
