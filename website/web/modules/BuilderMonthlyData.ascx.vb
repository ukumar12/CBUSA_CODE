Imports Components
Imports DataLayer

Partial Class modules_BuilderMonthlyData
    Inherits ModuleControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then
            rblView.SelectedValue = "Current"

            BindData()
        End If
    End Sub

    Private Sub BindData()
        Select Case rblView.SelectedValue
            Case "Current"
                Dim current As DateTime = IIf(Now.Month = 1, 12, Now.Month - 1) & "/1/" & IIf(Now.Month = 1, Now.Year - 1, Now.Year)
                Dim dt As DataTable = BuilderMonthlyStatsRow.GetBuilderStats(DB, Session("BuilderId"), "Year,Month", "Asc", current, current)
                If dt.Rows.Count = 0 Then
                    For Each dc As DataColumn In dt.Columns
                        dc.AllowDBNull = True
                    Next
                    Dim dr As DataRow = dt.NewRow
                    dr("Month") = IIf(Now.Month = 1, 12, Now.Month - 1)
                    dt.Rows.Add(dr)
                End If
                rptData.DataSource = dt
            Case "Missing", "All"
                Dim dt As DataTable = BuilderMonthlyStatsRow.GetBuilderStats(DB, Session("BuilderId"), "Year,Month", "Asc", "1/1/" & Now.Year, Now)
                For Each dc As DataColumn In dt.Columns
                    dc.AllowDBNull = True
                Next

                For i As Integer = 1 To Now.Month - 1
                    If dt.Select("Month=" & i).Length = 0 Then
                        Dim dr As DataRow = dt.NewRow
                        dr("Year") = Now.Year
                        dr("Month") = i
                        dt.Rows.Add(dr)
                    End If
                Next

                If rblView.SelectedValue = "Missing" Then
                    rptData.DataSource = dt.Select("StartedUnits is null", "Month Asc")
                Else
                    rptData.DataSource = dt.Select("", "Month Asc")
                End If
        End Select

        rptData.DataBind()
    End Sub

    Protected Sub rptData_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptData.ItemDataBound
        If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim ltlMonth As Literal = e.Item.FindControl("ltlMonth")
        Dim hdnMonth As HiddenField = e.Item.FindControl("hdnMonth")
        Dim txtStartedUnits As TextBox = e.Item.FindControl("txtStartedUnits")
        Dim txtSoldUnits As TextBox = e.Item.FindControl("txtSoldUnits")
        Dim txtClosedUnits As TextBox = e.Item.FindControl("txtClosedUnits")
        Dim txtUnsoldSpecs As TextBox = e.Item.FindControl("txtUnsoldSpecs")

        If rblView.SelectedValue = "Current" Then
            ltlMonth.Text = MonthName(e.Item.DataItem("Month")) & ", " & IIf(Now.Month = 1, Now.Year - 1, Now.Year)
        Else
            ltlMonth.Text = MonthName(e.Item.DataItem("Month")) & ", " & Now.Year
        End If

        hdnMonth.Value = e.Item.DataItem("Month")
        If Not IsDBNull(e.Item.DataItem("StartedUnits")) Then
            txtStartedUnits.Text = e.Item.DataItem("StartedUnits")
        End If
        If Not IsDBNull(e.Item.DataItem("SoldUnits")) Then
            txtSoldUnits.Text = e.Item.DataItem("SoldUnits")
        End If
        If Not IsDBNull(e.Item.DataItem("ClosingUnits")) Then
            txtClosedUnits.Text = e.Item.DataItem("ClosingUnits")
        End If
        If Not IsDBNull(e.Item.DataItem("UnsoldUnits")) Then
            txtUnsoldSpecs.Text = e.Item.DataItem("UnsoldUnits")
        End If
    End Sub

    Protected Sub rblView_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblView.SelectedIndexChanged
        BindData()
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not Page.IsValid Then
            Exit Sub
        End If

        'validate
        Dim bError As Boolean = False
        For Each item As RepeaterItem In rptData.Items
            Dim trMonth As HtmlTableRow = item.FindControl("trMonth")
            Dim txtStartedUnits As TextBox = item.FindControl("txtStartedUnits")
            Dim txtSoldUnits As TextBox = item.FindControl("txtSoldUnits")
            Dim txtClosedUnits As TextBox = item.FindControl("txtClosedUnits")
            Dim txtUnsoldSpecs As TextBox = item.FindControl("txtUnsoldSpecs")

            If txtStartedUnits.Text <> Nothing Or txtSoldUnits.Text <> Nothing Or txtClosedUnits.Text <> Nothing Or txtUnsoldSpecs.Text <> Nothing Then
                If txtStartedUnits.Text = Nothing Or txtSoldUnits.Text = Nothing Or txtClosedUnits.Text = Nothing Or txtUnsoldSpecs.Text = Nothing Then
                    bError = True
                    trMonth.Style("background-color") = "#fcc"
                    ctlErrors.AddError("Row #" & item.ItemIndex + 1 & " is incomplete")
                Else
                    trMonth.Style("background-color") = "#fff"
                End If
            End If
        Next

        If bError Then
            ctlErrors.Visible = True
            trError.Visible = True
            Exit Sub
        Else
            ctlErrors.Visible = False
            trError.Visible = False
        End If

        'save data
        For Each item As RepeaterItem In rptData.Items
            Dim hdnMonth As HiddenField = item.FindControl("hdnMonth")
            Dim txtStartedUnits As TextBox = item.FindControl("txtStartedUnits")
            Dim txtSoldUnits As TextBox = item.FindControl("txtSoldUnits")
            Dim txtClosedUnits As TextBox = item.FindControl("txtClosedUnits")
            Dim txtUnsoldSpecs As TextBox = item.FindControl("txtUnsoldSpecs")

            Dim dbData As BuilderMonthlyStatsRow = BuilderMonthlyStatsRow.GetRow(DB, Session("BuilderId"), Now.Year, hdnMonth.Value)

            If txtStartedUnits.Text = String.Empty Then
                dbData.Remove()
                Continue For
            End If

            dbData.BuilderID = Session("BuilderId")
            dbData.Month = hdnMonth.Value
            If rblView.SelectedValue = "Current" Then
                dbData.Year = IIf(Now.Month = 1, Now.Year - 1, Now.Year)
            Else
                dbData.Year = Now.Year
            End If

            dbData.ClosingUnits = txtClosedUnits.Text
            dbData.SoldUnits = txtSoldUnits.Text
            dbData.StartedUnits = txtStartedUnits.Text
            dbData.TimePeriodDate = hdnMonth.Value & "/1/" & Now.Year
            dbData.UnsoldUnits = txtUnsoldSpecs.Text

            If dbData.Updated = Nothing Then
                dbData.Insert()
            Else
                dbData.Update()
            End If
        Next

        BindData()

        ScriptManager.RegisterStartupScript(Page, Me.GetType, "MonthlyDataPopup", "Sys.Application.add_load(OpenSavedPopup);", True)
    End Sub
End Class
