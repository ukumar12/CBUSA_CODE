Imports Components
Imports DataLayer

Partial Class modules_BuilderMonthlyData1
    Inherits ModuleControl

    Protected Property EditIndex() As Integer
        Get
            Return ViewState("EditIndex")
        End Get
        Set(ByVal value As Integer)
            ViewState("EditIndex") = value
        End Set
    End Property

    Protected Property CurrentInterval() As Integer
        Get
            Return ViewState("CurrentInterval")
        End Get
        Set(ByVal value As Integer)
            ViewState("CurrentInterval") = value
        End Set
    End Property

    Private ShowAddRow As Boolean

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(Me)

        If Not IsPostBack Then
            EditIndex = -1
            BindData()

            F_Interval.Items.Add(New ListItem("6 Months", "6"))
            F_Interval.Items.Add(New ListItem("12 Months", "12"))
            F_Interval.Items.Add(New ListItem("18 Months", "18"))
            F_Interval.Items.Add(New ListItem("24 Months", "24"))
        End If
    End Sub

    Private Sub BindData()
        Dim dt As DataTable = BuilderMonthlyStatsRow.GetBuilderStats(DB, Session("BuilderId"), "Year,Month", "Asc", Now.AddMonths(-CurrentInterval))
        If dt.Rows.Count = 0 And Not ShowAddRow Then
            ltlNoItems.Text = "<b>No matching records for the selected date range</b><br/>"
        Else
            ltlNoItems.Text = String.Empty
            rptData.DataSource = dt
        End If
        If ShowAddRow Then
            For Each col As DataColumn In dt.Columns
                col.AllowDBNull = True
            Next
            dt.Rows.Add(dt.NewRow)
            EditIndex = dt.Rows.Count - 1
            ShowAddRow = False
        End If
        rptData.DataBind()
    End Sub

    Protected Sub rptData_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptData.ItemCommand
        Select Case e.CommandName
            Case "Edit"
                EditIndex = e.Item.ItemIndex
            Case "Delete"
                Dim year As Integer = CType(e.Item.FindControl("drpYear"), DropDownList).SelectedValue
                Dim month As Integer = CType(e.Item.FindControl("drpMonth"), DropDownList).SelectedValue
                BuilderMonthlyStatsRow.RemoveRow(DB, Session("BuilderID"), year, month)
            Case "Save"
                Dim year As Integer = CType(e.Item.FindControl("drpYear"), DropDownList).SelectedValue
                Dim month As Integer = CType(e.Item.FindControl("drpMonth"), DropDownList).SelectedValue
                Dim row As BuilderMonthlyStatsRow = BuilderMonthlyStatsRow.GetRow(DB, Session("BuilderId"), year, month)
                row.ClosingUnits = CType(e.Item.FindControl("txtClosing"), TextBox).Text
                row.SoldUnits = CType(e.Item.FindControl("txtSold"), TextBox).Text
                row.StartedUnits = CType(e.Item.FindControl("txtStarted"), TextBox).Text
                row.TimePeriodDate = Convert.ToDateTime("1/" & month & "/" & year)
                row.UnsoldUnits = CType(e.Item.FindControl("txtUnsold"), TextBox).Text
                If row.Updated = Nothing Then
                    row.Insert()
                Else
                    row.Update()
                End If
            Case "Cancel"
                EditIndex = -1
        End Select

        BindData()
    End Sub

    Protected Sub rptData_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptData.ItemCreated
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        Dim drpMonth As DropDownList = e.Item.FindControl("drpMonth")
        drpMonth.Items.Add(New ListItem("", ""))
        For m As Integer = 1 To 12
            Dim temp As Date = Date.Parse(m & "/1/1900")
            drpMonth.Items.Add(New ListItem(temp.ToString("MMMM"), m))
        Next

        Dim drpYear As DropDownList = e.Item.FindControl("drpYear")
        drpYear.Items.Add(New ListItem("", ""))
        For y As Integer = Now.Year To (Now.Year - 5) Step -1
            drpYear.Items.Add(New ListItem(y, y))
        Next
    End Sub

    Protected Sub rptData_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptData.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        If Not IsDBNull(e.Item.DataItem("Month")) Then
            Dim ltlMonth As Literal = e.Item.FindControl("ltlMonth")
            ltlMonth.Text = Convert.ToDateTime(Core.GetInt(e.Item.DataItem("Month")) & "/1/1900").ToString("MMMM") & ", " & Core.GetInt(e.Item.DataItem("Year"))
        End If

        If e.Item.ItemIndex = EditIndex Then
            For Each ctl As Control In e.Item.Controls
                If TypeOf ctl Is Literal Then
                    ctl.Visible = False
                ElseIf TypeOf ctl Is TextBox Or TypeOf ctl Is DropDownList Then
                    ctl.Visible = True
                ElseIf TypeOf ctl Is BaseValidator Then
                    CType(ctl, BaseValidator).Enabled = True
                End If
            Next
            CType(e.Item.FindControl("btnEdit"), Button).Visible = False
            CType(e.Item.FindControl("btnSave"), Button).Visible = True
            CType(e.Item.FindControl("btnDelete"), Button).Visible = False
            CType(e.Item.FindControl("btnCancel"), Button).Visible = True
        Else
            For Each ctl As Control In e.Item.Controls
                If TypeOf ctl Is Literal Then
                    ctl.Visible = True
                ElseIf TypeOf ctl Is TextBox Or TypeOf ctl Is DropDownList Then
                    ctl.Visible = False
                ElseIf TypeOf ctl Is BaseValidator Then
                    CType(ctl, BaseValidator).Enabled = False
                End If
            Next
            CType(e.Item.FindControl("btnEdit"), Button).Visible = True
            CType(e.Item.FindControl("btnSave"), Button).Visible = False
            CType(e.Item.FindControl("btnDelete"), Button).Visible = True
            CType(e.Item.FindControl("btnCancel"), Button).Visible = False
        End If
    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        ShowAddRow = True
        BindData()
    End Sub

    Protected Sub F_Interval_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles F_Interval.SelectedIndexChanged
        CurrentInterval = F_Interval.SelectedValue
        BindData()
    End Sub
End Class
