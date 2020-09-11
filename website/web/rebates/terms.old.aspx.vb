Imports Components
Imports DataLayer
Imports System.Linq
Imports System.Data

Partial Class rebates_terms_old
    Inherits SitePage

    Private ReadOnly Property Now() As DateTime
        Get
            Dim param As String = SysParam.GetValue(DB, "DemoReportingDate")
            If param = Nothing OrElse param = "0" Then
                Return DateTime.Now
            Else
                Return param
            End If
        End Get
    End Property

    Protected ReadOnly Property NextQuarter() As Integer
        Get
            Dim cur As Integer = Math.Floor(Now.Month / 3) + 1
            Return IIf(cur = 4, 1, cur + 1)
        End Get
    End Property

    Protected ReadOnly Property NextYear() As Integer
        Get
            Return IIf(NextQuarter = 1, Now.Year + 1, Now.Year)
        End Get
    End Property

    Private Property EditIndex() As Integer
        Get
            Return ViewState("EditIndex")
        End Get
        Set(ByVal value As Integer)
            ViewState("EditIndex") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'EnsureVendorAccess()
        If Session("VendorId") Is Nothing Or Session("VendorAccountId") Is Nothing Then
            Response.Redirect("/default.aspx")
        End If

        Dim dbRegistration As VendorRegistrationRow = VendorRegistrationRow.GetRowByVendor(DB, Session("VendorId"))
        If dbRegistration.CompleteDate <> Nothing Then
            ctlSteps.Visible = False
        End If
        ctlErrors.visible = False
        If Not IsPostBack And Not ScriptManager.GetCurrent(Page).IsInAsyncPostBack Then
            If Request("guid") IsNot Nothing Then
                btnContinue.Text = "Continue"
            Else
                btnContinue.Text = "Return to Dashboard"
            End If

            EditIndex = -1
            Dim dtTerms As DataTable = LoadTerms()
            'rblProgramType.Items(0).Attributes.Add("onclick", "OpenTermsConfirm(event);")
            rblProgramType.Attributes.Add("onclick", "OpenTermsConfirm(event);")



            If dtTerms.Rows.Count = 1 Then
                rblProgramType.SelectedValue = "flat"
                btnAddTerms.Visible = False
            Else
                rblProgramType.SelectedValue = "tiered"
                btnAddTerms.Visible = True
            End If
            BindTerms(dtTerms)
        End If
    End Sub

    Private Function LoadTerms() As DataTable
        Dim dt As DataTable = RebateTermRow.GetCurrentTerms(DB, Session("VendorID"))
        If dt.Rows.Count = 0 Then
            Dim dbTerms As New RebateTermRow(DB)
            dbTerms.VendorID = Session("VendorId")
            dbTerms.CreatorVendorAccountID = Session("VendorAccountId")
            dbTerms.IsAnnualPurchaseRange = True
            dbTerms.PurchaseRangeFloor = 0
            dbTerms.RebatePercentage = SysParam.GetValue(DB, "RebatePercentageFloor")
            dbTerms.StartQuarter = Math.Ceiling(Now.Month / 3)
            dbTerms.StartYear = Now.Year
            dbTerms.Insert()

            dt = RebateTermRow.GetCurrentTerms(DB, Session("VendorID"))
        End If

        Return dt
    End Function

    Private Sub BindTerms(ByVal dt As DataTable)
        rptTerms.DataSource = dt
        rptTerms.DataBind()
    End Sub

    Protected Sub rptTerms_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptTerms.ItemCommand
        Select Case e.CommandName
            Case "Edit"
                EditIndex = e.Item.ItemIndex
            Case "Save"
                Dim dbTerms As RebateTermRow = RebateTermRow.GetRow(DB, e.CommandArgument)

                If rblProgramType.SelectedValue = "tiered" Then
                    If dbTerms.Created = Nothing Then
                        If rptTerms.Items.Count = 1 Then
                            dbTerms.PurchaseRangeFloor = 0
                        Else
                            'dbTerms.PurchaseRangeFloor = rptTerms.Items.AsQueryable.OfType(of repeateritem).Aggregate(Function(a,b) IIf(Regex.Replace(CType(a.FindControl("tdRangeFloor"),HtmlTableCell).InnerHtml,"[\D]","") < Regex.Replace(CType(b.FindControl("tdRangeFloor"),HtmlTableCell).InnerHtml,"[\D]",""),b,a)
                            'dbTerms.PurchaseRangeFloor = (From item As RepeaterItem In rptTerms.Items Select Regex.Replace(CType(item.FindControl("tdRangeFloor"), HtmlTableCell).InnerHtml, "[\D]", "")).Aggregate(Function(a, b) IIf(a < b, b, a))
                            Dim temp As String = Regex.Replace(CType(e.Item.FindControl("tdRangeFloor"), HtmlTableCell).InnerHtml, "[\D]", "")
                            If temp <> String.Empty Then
                                dbTerms.PurchaseRangeFloor = Double.Parse(temp)
                            Else
                                dbTerms.PurchaseRangeFloor = (From item As RepeaterItem In rptTerms.Items Select Regex.Replace(CType(item.FindControl("tdRangeFloor"), HtmlTableCell).InnerHtml, "[\D]", "")).Aggregate(Function(a, b) IIf(a < b, b, a))
                            End If
                        End If
                        dbTerms.PurchaseRangeFloor = RebateTermRow.GetNextFloor(DB, Session("VendorID"), NextQuarter, NextYear)
                    End If
                    dbTerms.PurchaseRangeCeiling = CType(e.Item.FindControl("txtRangeCeiling"), TextBox).Text
                    If dbTerms.PurchaseRangeFloor >= dbTerms.PurchaseRangeCeiling Then
                        ctlErrors.AddError("Range Ceiling must be grather than the Range Floor")
                        ctlErrors.visible = True
                        Exit Sub
                    End If
                    Dim rblRangeApplies As RadioButtonList = e.Item.FindControl("rblRangeApplies")
                    If rblRangeApplies.SelectedValue = "annual" Then
                        dbTerms.IsAnnualPurchaseRange = True
                    Else
                        dbTerms.IsAnnualPurchaseRange = False
                    End If
                End If
                Dim pct As Double = Regex.Replace(CType(e.Item.FindControl("txtRebatePercentage"), TextBox).Text, "[^\d.]", "")
                If pct < 1 Then
                    ctlErrors.AddError("Rebate Percentage cannot be less than 1%")
                    ctlErrors.Visible = True
                    upTerms.Update()
                    Exit Sub
                End If
                Dim bSendNotification As Boolean = False
                If dbTerms.RebatePercentage < pct Then
                    bSendNotification = True
                End If
                dbTerms.RebatePercentage = pct

                dbTerms.VendorID = Session("VendorID")
                dbTerms.StartQuarter = NextQuarter
                dbTerms.StartYear = NextYear

                If dbTerms.Created = Nothing Then
                    dbTerms.CreatorVendorAccountID = Session("VendorAccountID")
                    dbTerms.Insert()
                Else
                    dbTerms.Update()
                End If
                EditIndex = -1

                Dim dbVendor As VendorRow = VendorRow.GetRow(DB, Session("VendorId"))
                Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "RebateTermsChanged")
                Dim msg As String = dbVendor.CompanyName & " has changed or deleted rebate terms.  Link to Admin: " & GlobalSecureName & "/admin/login.aspx"
                dbMsg.SendAdmin(msg)

                If bSendNotification Then
                    Dim LLCID As Integer = dbVendor.GetSelectedLLCs.Split(",")(0)
                    Dim dtBuilders As DataTable = BuilderRow.GetListByLLC(DB, LLCID)
                    dbMsg = AutomaticMessagesRow.GetRowByTitle(DB, "IncreasedRebate")
                    For Each row As DataRow In dtBuilders.Rows
                        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, row("BuilderId"))
                        dbMsg.Send(dbBuilder, "Vendor: " & vbTab & dbVendor.CompanyName & vbCrLf & "New Percentage: " & vbTab & dbTerms.RebatePercentage & "%")
                    Next
                End If

            Case "Cancel"
                EditIndex = -1
            Case "Delete"
                RebateTermRow.RemoveRow(DB, e.CommandArgument, True)

                EditIndex = -1
        End Select
        BindTerms(LoadTerms)

    End Sub

    Protected Sub rptTerms_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptTerms.ItemCreated
        If e.Item.ItemType = ListItemType.Header Then
            If rblProgramType.SelectedValue = "flat" Then
                Dim tdRangeFloor As HtmlTableCell = e.Item.FindControl("tdRangeFloor")
                Dim tdRangeCeiling As HtmlTableCell = e.Item.FindControl("tdRangeCeiling")
                Dim tdRangeApplies As HtmlTableCell = e.Item.FindControl("tdRangeApplies")
                tdRangeFloor.Visible = False
                tdRangeCeiling.Visible = False
                tdRangeApplies.Visible = False
            End If
        End If
    End Sub

    Protected Sub rptTerms_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptTerms.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        Dim ltlPeriod As Literal = e.Item.FindControl("ltlPeriod")
        ltlPeriod.Text = "Quarter " & e.Item.DataItem("StartQuarter") & ", " & e.Item.DataItem("StartYear")

        Dim tdRangeFloor As HtmlTableCell = e.Item.FindControl("tdRangeFloor")
        Dim tdRangeCeiling As HtmlTableCell = e.Item.FindControl("tdRangeCeiling")
        Dim ltlRangeCeiling As Literal = e.Item.FindControl("ltlRangeCeiling")
        Dim txtRangeCeiling As TextBox = e.Item.FindControl("txtRangeCeiling")
        Dim tdRangeApplies As HtmlTableCell = e.Item.FindControl("tdRangeApplies")
        Dim rblRangeApplies As RadioButtonList = e.Item.FindControl("rblRangeApplies")
        Dim tdRebatePercentage As HtmlTableCell = e.Item.FindControl("tdRebatePercentage")
        Dim ltlRebatePercentage As Literal = e.Item.FindControl("ltlRebatePercentage")
        Dim txtRebatePercentage As TextBox = e.Item.FindControl("txtRebatePercentage")

        Dim btnEdit As Button = e.Item.FindControl("btnEdit")
        Dim btnSave As Button = e.Item.FindControl("btnSave")
        Dim btnCancel As Button = e.Item.FindControl("btnCancel")
        Dim btnDelete As Button = e.Item.FindControl("btnDelete")

        If rblProgramType.SelectedValue = "flat" Then
            tdRangeFloor.Visible = False
            tdRangeCeiling.Visible = False
            tdRangeApplies.Visible = False
        Else
            If Not IsDBNull(e.Item.DataItem("PurchaseRangeFloor")) Then
                tdRangeFloor.InnerHtml = FormatCurrency(e.Item.DataItem("PurchaseRangeFloor"))
            End If
        End If
        If e.Item.ItemIndex = EditIndex Then
            btnEdit.Visible = False
            btnSave.Visible = True
            btnCancel.Visible = True
            btnDelete.Visible = True

            txtRangeCeiling.Text = IIf(IsDBNull(e.Item.DataItem("PurchaseRangeCeiling")), String.Empty, e.Item.DataItem("PurchaseRangeCeiling"))
            txtRebatePercentage.Text = IIf(IsDBNull(e.Item.DataItem("RebatePercentage")), String.Empty, e.Item.DataItem("RebatePercentage") & "%")
        Else
            btnEdit.Visible = True
            btnSave.Visible = False
            btnCancel.Visible = False
            btnDelete.Visible = True

            If Not IsDBNull(e.Item.DataItem("PurchaseRangeCeiling")) Then
                ltlRangeCeiling.Text = FormatCurrency(e.Item.DataItem("PurchaseRangeCeiling"))
            End If
            txtRangeCeiling.Visible = False
            ltlRebatePercentage.Text = IIf(IsDBNull(e.Item.DataItem("RebatePercentage")), String.Empty, e.Item.DataItem("RebatePercentage") & "%")
            txtRebatePercentage.Visible = False
        End If

        If e.Item.ItemIndex = 0 Then
            If Not IsDBNull(e.Item.DataItem("IsAnnualPurchaseRange")) Then
                If Convert.ToBoolean(e.Item.DataItem("IsAnnualPurchaseRange")) Then
                    rblRangeApplies.SelectedValue = "annual"
                Else
                    rblRangeApplies.SelectedValue = "quarter"
                End If
            End If
            If EditIndex <> 0 Then
                rblRangeApplies.Enabled = False
            End If
        Else
            Dim rblTemp As RadioButtonList = rptTerms.Items(0).FindControl("rblRangeApplies")
            rblRangeApplies.SelectedValue = rblTemp.SelectedValue
            rblRangeApplies.Enabled = False
        End If

    End Sub

    Protected Sub btnAddTerms_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddTerms.Click
        Dim dt As DataTable = LoadTerms()
        For Each c As DataColumn In dt.Columns
            c.AllowDBNull = True
        Next
        Dim row As DataRow = dt.NewRow
        If rptTerms.Items.Count = 0 Then
            row("PurchaseRangeFloor") = 0
        Else
            'dbTerms.PurchaseRangeFloor = rptTerms.Items.AsQueryable.OfType(of repeateritem).Aggregate(Function(a,b) IIf(Regex.Replace(CType(a.FindControl("tdRangeFloor"),HtmlTableCell).InnerHtml,"[\D]","") < Regex.Replace(CType(b.FindControl("tdRangeFloor"),HtmlTableCell).InnerHtml,"[\D]",""),b,a)
            'row("PurchaseRangeFloor") = (From item As RepeaterItem In rptTerms.Items Select Regex.Replace(CType(item.FindControl("tdRangeFloor"), HtmlTableCell).InnerHtml, "[\D]", "")).Aggregate(Function(a, b) IIf(a < b, b, a))
            row("PurchaseRangeFloor") = RebateTermRow.GetNextFloor(DB, Session("VendorID"), NextQuarter, NextYear)
        End If
        row("StartQuarter") = NextQuarter
        row("StartYear") = NextYear
        row("VendorID") = Session("VendorID")
        dt.Rows.Add(row)
        EditIndex = dt.Rows.Count - 1
        BindTerms(dt)
    End Sub

    Protected Sub rblProgramType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblProgramType.SelectedIndexChanged
        If rblProgramType.SelectedValue = "flat" Then
            btnAddTerms.Visible = False
            RebateTermRow.MakeFlat(DB, Session("VendorId"))
        Else
            btnAddTerms.Visible = True
        End If
        BindTerms(LoadTerms)
    End Sub

    Protected Sub btnContinue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnContinue.Click
        Dim dbRegistration As VendorRegistrationRow = VendorRegistrationRow.GetRowByVendor(DB, Session("VendorId"))
        If dbRegistration.CompleteDate = Nothing Then
            Dim dbStat As RegistrationStatusRow = RegistrationStatusRow.GetStatusByStep(DB, 4)
            dbRegistration.RegistrationStatusID = dbStat.RegistrationStatusID
            dbRegistration.Update()
            Response.Redirect("/forms/vendor-registration/supplyphase.aspx?guid=" & Request("guid"))
        Else
            Response.Redirect("/vendor/default.aspx")
        End If
    End Sub
End Class
