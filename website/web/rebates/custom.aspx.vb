Imports Components
Imports DataLayer
Imports System.Linq

Partial Class rebates_custom
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureVendorAccess()

        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(gvTerms)
        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(gvPrograms)
        ScriptManager.GetCurrent(Page).RegisterPostBackControl(frmRebate)
        ScriptManager.GetCurrent(Page).RegisterPostBackControl(frmProgram)
        If Not IsPostBack Then
            BindData()
            BindPrograms()
        End If
    End Sub

    Private Sub BindData()
        Dim sql As String = _
              " select r.*, b.CompanyName, p.ProgramName" _
            & " from CustomRebate r inner join Builder b on r.BuilderID=b.BuilderID" _
            & "     left outer join CustomRebateProgram p on r.CustomRebateProgramID=p.CustomRebateProgramID" _
            & " where r.VendorID=" & DB.Number(Session("VendorId"))

        gvTerms.DataSource = DB.GetDataTable(sql)
        gvTerms.DataBind()
    End Sub

    Private Sub BindPrograms()
        gvPrograms.DataSource = CustomRebateProgramRow.GetVendorPrograms(DB, Session("VendorId"), "ProgramName")
        gvPrograms.DataBind()
    End Sub

    Protected Sub gvTerms_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvTerms.RowCommand
        Dim CustomRebateID As Integer = e.CommandArgument
        Dim dbCustomRebate As CustomRebateRow = CustomRebateRow.GetRow(DB, CustomRebateID)

        hdnCustomRebateID.Value = dbCustomRebate.CustomRebateID
        txtApplicablePurchaseAmount.Text = dbCustomRebate.ApplicablePurchaseAmount
        drpBuilder.SelectedValue = dbCustomRebate.BuilderID
        drpProgram.SelectedValue = dbCustomRebate.CustomRebateProgramID
        txtDetails.Text = dbCustomRebate.Details
        txtMinimumPurchase.Text = dbCustomRebate.MinimumPurchase
        txtRebateAmount.Text = dbCustomRebate.RebateAmount
        txtRebatePercentage.Text = dbCustomRebate.RebatePercentage

        upRebateForm.Update()
        ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenEditForm", "Sys.Application.add_load(OpenEditForm);", True)
    End Sub

    Protected Sub gvPrograms_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvPrograms.RowCommand
        Dim CustomRebateProgramID As Integer = e.CommandArgument
        Dim dbProgram As CustomRebateProgramRow = CustomRebateProgramRow.GetRow(DB, CustomRebateProgramID)

        hdnCustomRebateProgramId.Value = dbProgram.CustomRebateProgramID
        txtProgramMinimumAmount.Text = dbProgram.MinimumPurchase
        txtProgramName.Text = dbProgram.ProgramName
        txtProgramDetails.Text = dbProgram.Details
        txtProgramRebatePercentage.Text = dbProgram.RebatePercentage

        upProgramForm.Update()
        ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenProgramForm", "Sys.Application.add_load(OpenProgramForm);", True)
    End Sub

    Protected Sub frmRebate_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmRebate.Postback
        Dim CustomRebateID As Integer = IIf(hdnCustomRebateID.Value = String.Empty, 0, hdnCustomRebateID.Value)
        Dim dbCustomRebate As CustomRebateRow = CustomRebateRow.GetRow(DB, CustomRebateID)

        dbCustomRebate.ApplicablePurchaseAmount = txtApplicablePurchaseAmount.Text
        dbCustomRebate.BuilderID = drpBuilder.SelectedValue
        If drpProgram.SelectedValue <> String.Empty Then
            dbCustomRebate.CustomRebateProgramID = drpProgram.SelectedValue
        Else
            dbCustomRebate.CustomRebateProgramID = Nothing
        End If
        dbCustomRebate.Details = txtDetails.Text
        If txtMinimumPurchase.Text <> String.Empty Then
            dbCustomRebate.MinimumPurchase = txtMinimumPurchase.Text
        Else
            dbCustomRebate.MinimumPurchase = Nothing
        End If
        If txtRebatePercentage.Text <> String.Empty Then
            dbCustomRebate.RebatePercentage = txtRebatePercentage.Text
        Else
            dbCustomRebate.RebatePercentage = Nothing
        End If
        If txtRebateAmount.Text = String.Empty Then
            dbCustomRebate.RebateAmount = dbCustomRebate.RebatePercentage / 100 * dbCustomRebate.ApplicablePurchaseAmount
        Else
            dbCustomRebate.RebateAmount = txtRebateAmount.Text
        End If

        dbCustomRebate.RebateQuarter = Math.Ceiling(Now.Month / 3)
        dbCustomRebate.RebateYear = Now.Year

        dbCustomRebate.VendorID = Session("VendorId")
        If dbCustomRebate.CustomRebateID = Nothing Then
            dbCustomRebate.Insert()
        Else
            dbCustomRebate.Update()
        End If

        BindData()
        upRebateForm.Update()
    End Sub

    Protected Sub frmRebate_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmRebate.TemplateLoaded
        Dim LLCID As String = VendorRow.GetLLCList(DB, Session("VendorID")).Split(",").FirstOrDefault
        Dim dtBuilders As DataTable = BuilderRow.GetSortedListByLLC(DB, LLCID, "CompanyName")
        drpBuilder.DataSource = dtBuilders
        drpBuilder.DataTextField = "CompanyName"
        drpBuilder.DataValueField = "BuilderID"
        drpBuilder.DataBind()
        drpBuilder.Items.Insert(0, New ListItem("", "0"))

        drpProgram.DataSource = CustomRebateProgramRow.GetVendorPrograms(DB, Session("VendorID"))
        drpProgram.DataTextField = "ProgramName"
        drpProgram.DataValueField = "CustomRebateProgramID"
        drpProgram.DataBind()
        drpProgram.Items.Insert(0, New ListItem("", "0"))
    End Sub

    Protected Sub drpProgram_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles drpProgram.SelectedIndexChanged
        If drpProgram.SelectedValue <> Nothing Then
            Dim dbProgram As CustomRebateProgramRow = CustomRebateProgramRow.GetRow(DB, drpProgram.SelectedValue)
            txtMinimumPurchase.Text = dbProgram.MinimumPurchase
            txtRebatePercentage.Text = dbProgram.RebatePercentage
            If txtApplicablePurchaseAmount.Text <> String.Empty Then
                txtRebateAmount.Text = FormatNumber(CDbl(txtApplicablePurchaseAmount.Text) * dbProgram.RebatePercentage / 100, 2)
            End If
            txtDetails.Text = dbProgram.Details
        End If
        upRebateForm.Update()
        ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenEditForm", "Sys.Application.add_load(OpenEditForm)", True)
    End Sub

    Protected Sub btnPrograms_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrograms.Click
        Response.Redirect("program.aspx")
    End Sub

    Protected Sub frmProgram_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmProgram.Postback
        Dim ProgramID As Integer = IIf(hdnCustomRebateProgramId.Value = String.Empty, 0, hdnCustomRebateProgramId.Value)
        Dim dbProgram As CustomRebateProgramRow = CustomRebateProgramRow.GetRow(DB, ProgramID)

        dbProgram.Details = txtProgramDetails.Text
        If txtMinimumPurchase.Text <> String.Empty Then
            dbProgram.MinimumPurchase = txtMinimumPurchase.Text
        End If

        dbProgram.ProgramName = txtProgramName.Text
        If txtProgramRebatePercentage.Text <> String.Empty Then
            dbProgram.RebatePercentage = txtProgramRebatePercentage.Text
        End If
        dbProgram.RebateQuarter = Math.Ceiling(Now.Month / 3)
        dbProgram.RebateYear = Now.Year
        dbProgram.VendorID = Session("VendorId")

        If dbProgram.CustomRebateProgramID = Nothing Then
            dbProgram.Insert()
        Else
            dbProgram.Update()
        End If

        BindPrograms()
    End Sub
End Class
