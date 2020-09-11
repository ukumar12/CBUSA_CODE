Imports Components
Imports DataLayer
Imports Controls
Imports TwoPrice.DataLayer

Partial Class modules_TwoPrice_CurrentTwoPriceTakeOffs
    Inherits ModuleControl

    Private dbTakeoff As TakeOffRow
    Private TwoPriceCampaignId As Integer
    Private dbTwoPriceCampaign As TwoPriceCampaignRow
    Private m_dtComparisons As DataTable
    Private ReadOnly Property dtComparisons() As DataTable
        Get
            If m_dtComparisons Is Nothing Then
                m_dtComparisons = PriceComparisonRow.GetSavedComparisons(DB, drpBuilders.SelectedValue)
            End If
            Return m_dtComparisons
        End Get
    End Property
    Protected ReadOnly Property TwoPriceTakeOff() As TwoPriceTakeOffRow
        Get
            'Check if Campaign has a TakeOff associated to it.
            If TwoPriceCampaignId <> Nothing Then
                dbTwoPriceTakeOff = TwoPriceTakeOffRow.GetRowByTwoPriceCampaignId(DB, TwoPriceCampaignId)
                If dbTwoPriceTakeOff.TwoPriceTakeOffID <> Nothing Then
                    Session("CurrentTwoPriceTakeOffId") = dbTwoPriceTakeOff.TwoPriceTakeOffID
                End If
            End If
            'If there is no takeoff create one
            If dbTwoPriceTakeOff Is Nothing Then
                dbTwoPriceTakeOff = New TwoPriceTakeOffRow(DB)
                dbTwoPriceTakeOff.TwoPriceCampaignId = TwoPriceCampaignId
                Session("CurrentTwoPriceTakeOffId") = dbTwoPriceTakeOff.Insert()
            End If
            Return dbTwoPriceTakeOff
        End Get
    End Property
    Protected dbTwoPriceTakeOff As TwoPriceTakeOffRow

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        dbTakeoff = TakeOffRow.GetRow(DB, TwoPriceTakeOffId)
        TwoPriceCampaignId = Convert.ToInt32(Request.QueryString("TwoPriceCampaignId"))
        dbTwoPriceCampaign = TwoPriceCampaignRow.GetRow(DB, TwoPriceCampaignId)
    End Sub

    Private Sub LoadControls()
        drpBuilders.DataSource = BuilderRow.GetListByLLCs(DB, dbTwoPriceCampaign.GetSelectedLLCs)
        drpBuilders.DataValueField = "BuilderId"
        drpBuilders.DataTextField = "CompanyName"
        drpBuilders.DataBind()
        drpBuilders.Items.Insert(0, New ListItem("--Select--", ""))
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        gvList.BindList = AddressOf BindData
        If Not IsPostBack Then
            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "Saved"
                gvList.SortOrder = "Desc"
            End If
            LoadControls()
            BindData()
        End If
    End Sub

    Private Sub BindData()
        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        gvList.DataKeyNames = New String() {"TakeoffId"}
        If drpBuilders.SelectedValue <> Nothing Then
            gvList.Pager.NofRecords = TakeOffRow.GetBuilderTakeoffCount(DB, drpBuilders.SelectedValue)

            Dim res As DataTable = TakeOffRow.GetBuilderTakeoffs(DB, drpBuilders.SelectedValue, gvList.SortBy, gvList.SortOrder, gvList.PageIndex + 1, gvList.PageSize)
            gvList.DataSource = res.DefaultView
        End If
        gvList.DataBind()
    End Sub

    'Protected Sub gvList_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvList.RowCommand
    '    If e.CommandName = "DeleteTakeoff" Then
    '        TakeOffRow.RemoveRow(DB, e.CommandArgument)
    '        BindData()
    '    ElseIf e.CommandName = "ChangeTakeoff" Then
    '        ChangeCurrentTakeoff()
    '        dbTakeoff = TakeOffRow.GetRow(DB, e.CommandArgument)
    '        CurrentTwoPriceTakeOffId = dbTakeoff.TakeOffID
    '        Response.Redirect("edit.aspx")
    '    End If
    'End Sub
    Protected ReadOnly Property TwoPriceTakeOffId() As Integer
        Get
            If Not IsNumeric(Session("CurrentTwoPriceTakeOffId")) Then Session("CurrentTwoPriceTakeOffId") = Nothing
            Return Session("CurrentTwoPriceTakeOffId")
        End Get
    End Property

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        If TwoPriceTakeOffId <> Nothing Then
            Dim cnt As Integer = 0
            For Each row As GridViewRow In gvList.Rows
                Dim cb As CheckBox = row.FindControl("cbInclude")
                If cb.Checked Then
                    cnt += 1
                    Dim TakeoffId As Integer = gvList.DataKeys(row.RowIndex)(0)
                    If TakeoffId <> Nothing Then
                        TwoPriceTakeOffRow.CopyToTwoPriceTakeOff(DB, TakeoffId, TwoPriceTakeOffId)
                    End If
                End If
            Next
        If cnt = 0 Then
            AddError("You must select at least one takeoff to add from")
        Else
                Response.Redirect("edit.aspx?TwoPriceCampaignId=" & TwoPriceCampaignId)
            End If
        End If
    End Sub

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then
            Exit Sub
        End If
        Dim ltlComparisons As Literal = e.Row.FindControl("ltlComparisons")

        'Dim dt As DataTable = PriceComparisonRow.GetLatestSavedComparison(DB, Session("BuilderId"), e.Row.DataItem("TakeoffID"))
        Dim dt As DataTable = PriceComparisonRow.GetLastComparison(DB, e.Row.DataItem("TakeoffID"))

        For Each row As DataRow In dt.Rows
            ltlComparisons.Text &= "<a class=""smallest"" href=""/comparison/default.aspx?PriceComparisonId=" & row("PriceComparisonID") & """>" & row("Created") & "</a><br/>"
        Next
    End Sub

    Protected Sub drpBuilders_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles drpBuilders.SelectedIndexChanged
        BindData()
    End Sub

End Class
