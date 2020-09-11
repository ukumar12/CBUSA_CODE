Imports Components
Imports DataLayer
Imports PopupForm

Partial Class rebates_discrepancy_response
    Inherits SitePage
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Protected LastClickedReasonId As Integer
    Protected BuilderInitialReportedTotalAmount As Double = 0

    Private ReadOnly Property LastQuarter() As Integer
        Get
            Dim ret As Integer = Math.Ceiling(Now.Month / 3) - 1
            Return IIf(ret = 0, 4, ret)
        End Get
    End Property

    Private m_dvInvoices As DataView
    Private ReadOnly Property dvInvoices() As DataView
        Get
            If m_dvInvoices Is Nothing Then
                m_dvInvoices = SalesReportBuilderInvoiceRow.GetAllVendorInvoices(DB, Session("VendorID"), LastQuarter, LastYear).DefaultView
            End If
            Return m_dvInvoices
        End Get
    End Property

    Private ReadOnly Property LastYear() As Integer
        Get
            Return IIf(LastQuarter = 4, Now.Year - 1, Now.Year) '2015
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

    Private m_dtResponses As DataTable
    Private ReadOnly Property dtResponses() As DataTable
        Get
            If m_dtResponses Is Nothing Then
                m_dtResponses = DisputeResponseRow.GetList(DB)
            End If
            Return m_dtResponses
        End Get
    End Property

    Private m_dtReasons As DataTable
    Private ReadOnly Property dtReasons() As DataTable
        Get
            If m_dtReasons Is Nothing Then
                m_dtReasons = DisputeResponseReasonRow.GetList(DB)
            End If
            Return m_dtReasons
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureVendorAccess()

        frmDeadline.EnsureChildrenCreated()

        If Deadline.Date.AddDays(-1) < Now.Date Then
            ltlDeadline.Text = "<b>The dispute response deadline for the current reporting period has passed.</b>"
            ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenDeadline", "Sys.Application.add_load(OpenDeadline);", True)
        ElseIf ReportDeadline.Date.AddDays(-1) > Now.Date Then
            ltlDeadline.Text = "<b>The reporting deadline for last quarter has not closed yet.</b>"
            ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenDeadline", "Sys.Application.add_load(OpenDeadline);", True)


        PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorId")
        UserName = Session("Username")

        ElseIf Not IsPostBack And Not ScriptManager.GetCurrent(Page).IsInAsyncPostBack Then

         Core.DataLog("Discrepancy Report", PageURL, CurrentUserId, "Vendor Left Menu Click", "", "", "", "", UserName)

            BindReport()
            LastClickedReasonId = 0
        End If

    End Sub

    Private Sub BindReport()
        rptReport.DataSource = SalesReportDisputeRow.GetListByVendor(DB, Session("VendorID"), LastQuarter, LastYear, True)
        rptReport.DataBind()
    End Sub

    Protected Sub frmResponse_TemplateLoaded(ByVal sender As Object, ByVal args As System.EventArgs)
        Dim form As PopupForm.PopupForm = CType(sender, PopupForm.PopupForm)
        Dim rptItem As RepeaterItem = CType(sender, Control).NamingContainer
        If rptItem.DataItem IsNot Nothing Then
            CType(form.FindControl("hdnSalesReportDisputeId"), HiddenField).Value = rptItem.DataItem("SalesReportDisputeId")
            CType(form.FindControl("ltlBuilder"), Literal).Text = rptItem.DataItem("BuilderCompany")
            If Not IsDBNull(rptItem.DataItem("VendorTotalAmount")) Then
                CType(form.FindControl("ltlVendorAmount"), Literal).Text = FormatCurrency(rptItem.DataItem("VendorTotalAmount"))
            End If
            If Not IsDBNull(rptItem.DataItem("BuilderTotalAmount")) Then
                CType(form.FindControl("ltlBuilderAmount"), Literal).Text = FormatCurrency(rptItem.DataItem("BuilderTotalAmount"))
            End If

            CType(form.FindControl("txtComments"), TextBox).Text = IIf(IsDBNull(rptItem.DataItem("VendorComments")), String.Empty, rptItem.DataItem("VendorComments"))
            CType(form.FindControl("ltlBuilderComments"), Literal).Text = IIf(IsDBNull(rptItem.DataItem("BuilderComments")), String.Empty, rptItem.DataItem("BuilderComments"))

            CType(form.FindControl("ltlVendorComments"), Literal).Text = IIf(IsDBNull(rptItem.DataItem("VendorComments")), String.Empty, rptItem.DataItem("VendorComments"))

            Dim trReason As HtmlTableRow = form.FindControl("trReason")
            Dim trNewAmount As HtmlTableRow = form.FindControl("trNewAmount")

            Dim rptOptionButtons As Repeater = form.FindControl("rptOptionButtons")
            rptOptionButtons.DataSource = DisputeResponseRow.GetList(DB, "DisputeResponse")
            rptOptionButtons.DataBind()

            trReason.Style.Add("display", "none")
            trNewAmount.Style.Add("display", "none")
            trComments.Style.Add("display", "none")
            trAccept.Style.Add("display", "none")

            Dim rblReason As RadioButtonList = form.FindControl("rblReason")
            rblReason.DataSource = DisputeResponseReasonRow.GetList(DB)
            rblReason.DataTextField = "DisputeResponseReason"
            rblReason.DataValueField = "DisputeResponseReasonID"
            rblReason.DataBind()

            Dim drpResponse As DropDownList = form.FindControl("drpResponse")
            drpResponse.DataSource = DisputeResponseRow.GetList(DB)
            drpResponse.DataTextField = "DisputeResponse"
            drpResponse.DataValueField = "DisputeResponseID"
            drpResponse.DataBind()

            Dim dbResponse As New DisputeResponseRow
            If Not IsDBNull(rptItem.DataItem("DisputeResponseID")) Then
                dbResponse = DisputeResponseRow.GetRow(DB, rptItem.DataItem("DisputeResponseID"))
                pSubmit.Style.Remove("display")

                drpResponse.SelectedValue = rptItem.DataItem("DisputeResponseID")

                If Not IsDBNull(rptItem.DataItem("ResolutionAmount")) And dbResponse.DisputeResponse.Contains("New") Then
                    CType(form.FindControl("txtNewAmount"), TextBox).Text = FormatCurrency(rptItem.DataItem("ResolutionAmount"))
                End If

                If dbResponse.DisputeResponse.ToLower.Contains("refuse") Then
                    rblReason.SelectedValue = rptItem.DataItem("DisputeResponseReasonID")
                    trReason.Style.Remove("display")
                    trComments.Style.Remove("display")
                    trNewAmount.Style.Add("display", "none")
                    trAccept.Style.Add("display", "none")
                ElseIf dbResponse.DisputeResponse.ToLower.Contains("new") Then
                    trAccept.Style.Add("display", "none")
                    trReason.Style.Add("display", "none")
                    trNewAmount.Style.Remove("display")
                    trComments.Style.Remove("display")
                ElseIf dbResponse.DisputeResponse.ToLower.Contains("accept") Then
                    trReason.Style.Add("display", "none")
                    trNewAmount.Style.Add("display", "none")
                    trComments.Style.Add("display", "none")
                    trAccept.Style.Remove("display")
                End If
            End If
        End If
    End Sub

    Protected Sub frmResponse_Postback(ByVal sender As Object, ByVal args As System.EventArgs)
        Dim form As PopupForm.PopupForm = CType(sender, Control).NamingContainer
        form.Validate()
        If Not form.IsValid Then Exit Sub
        Dim DisputeFinalResolutionAmount As Double = 0 ' added newly by angshuman mstn ticket #T-1111

        Dim ltlErrors As Literal = form.FindControl("ltlErrors")
        Dim SalesReportDisputeId As Integer = CType(form.FindControl("hdnSalesReportDisputeId"), HiddenField).Value
        Dim dbDispute As SalesReportDisputeRow = SalesReportDisputeRow.GetRow(DB, SalesReportDisputeId)
        dbDispute.DisputeResponseID = CType(form.FindControl("drpResponse"), DropDownList).SelectedValue

        Dim dbResponse As DisputeResponseRow = DisputeResponseRow.GetRow(DB, dbDispute.DisputeResponseID)

        divError.Visible = False
        ltlErrors.Text = ""
        upErrors.Update()

        If dbResponse.DisputeResponse.ToLower.Contains("refuse") Then
            Integer.TryParse(CType(form.FindControl("rblReason"), RadioButtonList).SelectedValue, dbDispute.DisputeResponseReasonID)

            If dbDispute.DisputeResponseReasonID = Nothing Then
                ltlErrors.Text = "Must select a reason."
                divError.Visible = True
                upErrors.Update()
                Exit Sub
            End If
        Else
            dbDispute.DisputeResponseReasonID = Nothing
        End If

        If dbResponse.DisputeResponse.ToLower.Contains("new") Then

            If CType(form.FindControl("txtNewAmount"), TextBox).Text = String.Empty Then
                ltlErrors.Text = "Must enter a valid adjusted sales report."
                divError.Visible = True
                upErrors.Update()
                Exit Sub
            End If

            dbDispute.ResolutionAmount = CType(form.FindControl("txtNewAmount"), TextBox).Text

            If dbDispute.ResolutionAmount = Nothing Then
                ltlErrors.Text = "Must enter a valid adjusted sales report."
                divError.Visible = True
                upErrors.Update()
                Exit Sub
            End If
            If dbDispute.ResolutionAmount < 0 Then
                ltlErrors.Text = "Adjusted sales report Amount should be greater than 0."
                divError.Visible = True
                upErrors.Update()
                Exit Sub
            End If



        Else
            dbDispute.ResolutionAmount = Nothing
        End If
        If dbResponse.DisputeResponse.ToString = "Accept" Then dbDispute.ResolutionAmount = dbDispute.BuilderTotalAmount
        If dbResponse.DisputeResponse.ToString = "Refuse" Then
            If CType(form.FindControl("txtAdjustAmount"), TextBox).Text <> String.Empty Then
                dbDispute.ResolutionAmount = CType(form.FindControl("txtAdjustAmount"), TextBox).Text
            Else
                dbDispute.ResolutionAmount = dbDispute.VendorTotalAmount
            End If
            DisputeFinalResolutionAmount = dbDispute.ResolutionAmount
        End If

        dbDispute.VendorComments = CType(form.FindControl("txtComments"), TextBox).Text
        dbDispute.ModifyDate = Date.Now
        dbDispute.Update()
        BindReport()

        'Send DisputeResponse email to Builder

        Dim BuilderId As Integer = dbDispute.BuilderID
        Dim dbAutoMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "DisputeResponse")
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, BuilderId)
        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, Session("VendorId"))

        Dim MsgBody As String = String.Empty
        MsgBody = dbVendor.CompanyName & " provided the following response to your dispute:" & vbCrLf & vbCrLf








        If dbResponse.DisputeResponse.ToString = "Refuse" Then
            MsgBody &= "Vendor Reported Total (original): " & CType(form.FindControl("ltlVendorAmount"), Literal).Text & vbCrLf ' added newly by angshuman mstn ticket #T-1111
            MsgBody &= "Builder Reported Total (original): $" & CType(form.FindControl("ltlBuilderAmount"), Literal).Text & vbCrLf ' added newly by angshuman mstn ticket #T-1111
        End If
        MsgBody &= "Response: " & DisputeResponseRow.GetRow(DB, dbDispute.DisputeResponseID).DisputeResponse & vbCrLf
        If dbResponse.DisputeResponse.ToString = "Refuse" Then
            MsgBody &= "Reason: " & DisputeResponseReasonRow.GetRow(DB, dbDispute.DisputeResponseReasonID).DisputeResponseReason & vbCrLf
            MsgBody &= "Final Resolution Amount: $" & DisputeFinalResolutionAmount & vbCrLf ' added newly by angshuman mstn ticket #T-1111
            If CType(form.FindControl("txtAdjustAmount"), TextBox).Text <> String.Empty Then
                MsgBody &= "Adjusted Amount: " & CType(form.FindControl("txtAdjustAmount"), TextBox).Text & vbCrLf & vbCrLf
            End If

            If CType(form.FindControl("txtComments"), TextBox).Text <> String.Empty Then
                MsgBody &= "Comments: " & CType(form.FindControl("txtComments"), TextBox).Text & vbCrLf & vbCrLf
            End If
        End If
        If dbResponse.DisputeResponse.ToString <> "Refuse" Then
            If CType(form.FindControl("txtNewAmount"), TextBox).Text <> String.Empty Then
                MsgBody &= "New Amount: " & CType(form.FindControl("txtNewAmount"), TextBox).Text & vbCrLf & vbCrLf
            End If
        End If

        MsgBody &= "Please review your discrepancy report to review the details:" & vbCrLf & vbCrLf & System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/rebates/discrepancy-report.aspx"
        dbAutoMsg.Send(dbBuilder, MsgBody)

        phResult.Visible = True
        upReport.Update()
    End Sub

    Protected Sub rptReport_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptReport.ItemCreated
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        Dim frmResponse As PopupForm.PopupForm = e.Item.FindControl("frmResponse")
        AddHandler frmResponse.TemplateLoaded, AddressOf frmResponse_TemplateLoaded
        AddHandler frmResponse.Postback, AddressOf frmResponse_Postback
        Dim rptInvoices As Repeater = e.Item.FindControl("rptInvoices")
        If e.Item.DataItem IsNot Nothing Then
            If Not IsDBNull(e.Item.DataItem("BuilderID")) Then
                dvInvoices.RowFilter = "BuilderID=" & e.Item.DataItem("BuilderID")
                If dvInvoices.Count > 0 Then
                    rptInvoices.DataSource = dvInvoices
                    rptInvoices.DataBind()
                Else
                    e.Item.FindControl("phNoInvoices").Visible = True
                End If
            End If

        End If

        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(frmResponse)
    End Sub

    Protected Sub rptReport_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptReport.ItemDataBound
        If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim ltlReason As Literal = e.Item.FindControl("ltlReason")
        Dim ltlResponse As Literal = e.Item.FindControl("ltlResponse")
        Dim btnResponse As Button = e.Item.FindControl("btnResponse")

	Dim strControlIndex As String = btnResponse.ClientID.Replace("_btnResponse", "").Replace("rptReport_", "")
        Dim strFormId As String = "rptReport_" & strControlIndex & "_frmResponse_window"
        btnResponse.OnClientClick = "javascript:return ShowResponseForm('" & strFormId & "');"
        'btnResponse.OnClientClick = "javascript:return ShowResponseForm('rptReport_frmResponse_" & e.Item.ItemIndex & "_window_" & e.Item.ItemIndex & "');"

        Dim form As PopupForm.PopupForm = e.Item.FindControl("frmResponse")
        Dim btnSpanClose As Button = form.FindControl("btnSpanClose")
        btnSpanClose.OnClientClick = "javascript:return HideResponseForm('" & strFormId & "');"

        Dim btnCancel As Button = form.FindControl("btnCancel")
        btnCancel.OnClientClick = "javascript:return HideResponseForm('" & strFormId & "');"

        Dim ltlFinalAmount As Literal = e.Item.FindControl("ltlFinalAmount")

        Dim ltlVendorTotal As Literal = e.Item.FindControl("ltlVendorTotal")
        Dim ltlBuilderTotal As Literal = e.Item.FindControl("ltlBuilderTotal")
        Dim Popup As PopupForm.PopupForm = e.Item.FindControl("frmResponse")
        Dim txtPrevAmount As TextBox = Popup.FindControl("txtPrevAmount")

        Dim pr As PurchasesReportRow = PurchasesReportRow.GetPurchasesReportByPeriod(DB, e.Item.DataItem("BuilderID"), LastYear, LastQuarter)
        Dim sr As SalesReportRow = SalesReportRow.GetSalesReportByPeriod(DB, Session("VendorID"), LastYear, LastQuarter)

        'Returns DNR if one side did not report
        ltlBuilderTotal.Text = pr.GetReportedPurchases(Session("VendorID"))
        txtPrevAmount.Text = IIf(ltlBuilderTotal.Text = "DNR", "$0.00", ltlBuilderTotal.Text)
        txtPrevAmount.BackColor = Color.LightGray
        txtPrevAmount.ForeColor = Color.DarkBlue

        Dim SalesReportID As Object = DB.ExecuteScalar("SELECT TOP 1 SalesReportID FROM SalesReport WHERE Submitted is not null AND PeriodYear=" & LastYear & " AND PeriodQuarter=" & LastQuarter & " AND VendorID=" & Session("VendorID"))
        If Not IsDBNull(SalesReportID) And SalesReportID IsNot Nothing Then
            ltlVendorTotal.Text = sr.GetReportedSales(e.Item.DataItem("BuilderID"))
        Else
            ltlVendorTotal.Text = "DNR"
        End If

        'ltlVendorTotal.Text = sr.GetReportedSales(e.Item.DataItem("BuilderID"))

        If Not IsDBNull(e.Item.DataItem("DisputeResponseID")) Then
            Dim rows As DataRow() = dtResponses.Select("DisputeResponseID=" & e.Item.DataItem("DisputeResponseID"))
            If rows.Length > 0 Then
                ltlResponse.Text = rows(0)("DisputeResponse")
            End If
            btnResponse.Text = "Edit Response"
            If Not IsDBNull(e.Item.DataItem("ResolutionAmount")) Then
                ltlFinalAmount.Text = FormatCurrency(e.Item.DataItem("ResolutionAmount"))
            Else
                ltlFinalAmount.Text = FormatCurrency(e.Item.DataItem("VendorTotalAmount"))
            End If

        End If

        If Not IsDBNull(e.Item.DataItem("DisputeResponseReasonID")) Then
            Dim rows As DataRow() = dtReasons.Select("DisputeResponseReasonID=" & e.Item.DataItem("DisputeResponseReasonID"))
            If rows.Length > 0 Then
                ltlReason.Text = If(rows(0)("DisputeResponseReason").ToString.StartsWith("Other"), Core.GetString(e.Item.DataItem("VendorComments")), rows(0)("DisputeResponseReason"))
            End If
        Else
            ltlReason.Text = Core.GetString(e.Item.DataItem("VendorComments"))
        End If
        Dim rptInvoices As Repeater = e.Item.FindControl("rptInvoices")

        If rptInvoices.Items.Count = 0 Then
            e.Item.FindControl("btnDetails").Visible = False
        End If
        Dim divDetails As HtmlGenericControl = e.Item.FindControl("divDetails")
        divDetails.Style("display") = "none"
    End Sub

    Protected Sub ReasonClick(ByVal sender As Object, ByVal e As System.EventArgs)
        'Hackish. RepeaterItem.NamingContainer -> Repeater.NamingContainer -> PopupForm
        Dim form As PopupForm.PopupForm = CType(sender, Control).NamingContainer.NamingContainer.NamingContainer
        CType(form.FindControl("drpResponse"), DropDownList).SelectedValue = CType(sender, Button).CommandArgument
        CType(form.FindControl("upErrors"), UpdatePanel).Update()
    End Sub

    Protected Sub rptOptionButtons_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptOptionButtons.ItemDataBound
        If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then Exit Sub

        Dim btnReason As Button = e.Item.FindControl("btnReason")
        btnReason.CommandArgument = e.Item.DataItem("DisputeResponseId")
        btnReason.Text = e.Item.DataItem("DisputeResponse")
    End Sub

End Class
