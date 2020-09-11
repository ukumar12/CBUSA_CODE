Imports Components
Imports DataLayer

Partial Class rebates_program
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack And Not ScriptManager.GetCurrent(Page).IsInAsyncPostBack Then
            gvPrograms.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvPrograms.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvPrograms.SortBy = String.Empty Then gvPrograms.SortBy = "ProgramName"

            BindData()
        End If
    End Sub

    Private Sub BindData()
        ViewState("F_SortBy") = gvPrograms.SortBy
        ViewState("F_SortOrder") = gvPrograms.SortOrder

        Dim res As DataTable = ProjectRow.GetBuilderProjects(DB, Session("BuilderId"))
        gvPrograms.DataSource = res.DefaultView
        gvPrograms.DataBind()
    End Sub

    Protected Sub frmDetails_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim form As PopupForm.PopupForm = sender
        Dim row As GridViewRow = CType(form.NamingContainer, GridViewRow)

        CType(form.FindControl("txtProgramName"), TextBox).Text = row.DataItem("ProgramName")
        CType(form.FindControl("txtMinimumAmount"), TextBox).Text = IIf(IsDBNull(row.DataItem("MinimumAmount")), Nothing, row.DataItem("MinimumAmount"))
        CType(form.FindControl("txtRebatePercentage"), TextBox).Text = IIf(IsDBNull(row.DataItem("RebatePercentage")), Nothing, row.DataItem("RebatePercentage"))
        CType(form.FindControl("txtDetails"), TextBox).Text = IIf(IsDBNull(row.DataItem("Details")), Nothing, row.DataItem("Details"))
        CType(form.FindControl("hdnCustomRebateProgramId"), HiddenField).Value = row.DataItem("CustomRebateProgramID")

        AddHandler form.Postback, AddressOf frmDetails_Postback
    End Sub

    Protected Sub frmDetails_Postback(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim form As PopupForm.PopupForm = CType(sender, Control).NamingContainer
        form.Validate()
        If Not form.IsValid Then Exit Sub

        Dim ProgramId As Integer = CType(form.FindControl("hdnCustomRebateProgramId"), HiddenField).Value
        Dim dbProgram As CustomRebateProgramRow = CustomRebateProgramRow.GetRow(DB, ProgramId)
        dbProgram.Details = CType(form.FindControl("txtDetails"), TextBox).Text

        Dim MinimumAmount As String = CType(form.FindControl("txtMinimumAmount"), TextBox).Text
        If MinimumAmount <> String.Empty Then
            dbProgram.MinimumPurchase = Double.Parse(MinimumAmount)
        End If

        Dim RebatePercentage As String = CType(form.FindControl("txtRebatePercentage"), TextBox).Text
        If RebatePercentage <> String.Empty Then
            dbProgram.RebatePercentage = Double.Parse(RebatePercentage)
        End If

        dbProgram.RebateQuarter = IIf(Math.Floor(Now.Month / 3) + 1 = 5, 1, Math.Floor(Now.Month / 3) + 1)
        dbProgram.RebateYear = IIf(dbProgram.RebateQuarter = 1, Now.Year + 1, Now.Year)
        dbProgram.ProgramName = CType(form.FindControl("txtProgramName"), TextBox).Text
        dbProgram.VendorID = Session("VendorId")

        If dbProgram.CustomRebateProgramID = Nothing Then
            dbProgram.Insert()
        Else
            dbProgram.Update()
        End If
    End Sub
End Class

