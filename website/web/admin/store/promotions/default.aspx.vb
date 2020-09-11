Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("MARKETING_TOOLS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_PromotionName.Text = Request("F_PromotionName")
            F_PromotionCode.Text = Request("F_PromotionCode")
            F_drpPromotionType.SelectedValue = Request("F_drpPromotionType")
            F_IsActive.Text = Request("F_IsActive")
            F_StartDateLbound.Text = Request("F_StartDateLBound")
            F_StartDateUbound.Text = Request("F_StartDateUBound")
            F_EndDateLbound.Text = Request("F_EndDateLBound")
            F_EndDateUbound.Text = Request("F_EndDateUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "PromotionName"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM StorePromotion  "

        If Not F_PromotionName.Text = String.Empty Then
            SQL = SQL & Conn & "PromotionName LIKE " & DB.FilterQuote(F_PromotionName.Text)
            Conn = " AND "
        End If
        If Not F_PromotionCode.Text = String.Empty Then
            SQL = SQL & Conn & "PromotionCode LIKE " & DB.FilterQuote(F_PromotionCode.Text)
            Conn = " AND "
        End If
        If Not F_drpPromotionType.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "PromotionType = " & DB.Quote(F_drpPromotionType.SelectedValue)
            Conn = " AND "
        End If
        If Not F_IsActive.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "IsActive  = " & DB.Number(F_IsActive.SelectedValue)
            Conn = " AND "
        End If
        If Not F_StartDateLbound.Text = String.Empty Then
            SQL = SQL & Conn & "StartDate >= " & DB.Quote(F_StartDateLbound.Text)
            Conn = " AND "
        End If
        If Not F_StartDateUbound.Text = String.Empty Then
            SQL = SQL & Conn & "StartDate < " & DB.Quote(DateAdd("d", 1, F_StartDateUbound.Text))
            Conn = " AND "
        End If
        If Not F_EndDateLbound.Text = String.Empty Then
            SQL = SQL & Conn & "EndDate >= " & DB.Quote(F_EndDateLbound.Text)
            Conn = " AND "
        End If
        If Not F_EndDateUbound.Text = String.Empty Then
            SQL = SQL & Conn & "EndDate < " & DB.Quote(DateAdd("d", 1, F_EndDateUbound.Text))
            Conn = " AND "
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not Page.IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub

    Protected Sub gvList_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvList.RowCommand
        If e.CommandName = "RelatedItems" Then
            DB.Close()
            Response.Redirect("/admin/store/promotions/specific-items.aspx?PromotionId=" & e.CommandArgument & "&" & GetPageParams())
        End If
    End Sub

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim btnRelatedItems As Button = CType(e.Row.FindControl("btnRelatedItems"), Button)
            btnRelatedItems.CommandName = "RelatedItems"
            btnRelatedItems.CommandArgument = e.Row.DataItem("PromotionId")

            Dim ltlDiscount As Literal = CType(e.Row.FindControl("ltlDiscount"), Literal)
            If e.Row.DataItem("IsItemSpecific") = True Then btnRelatedItems.Visible = True Else btnRelatedItems.Visible = False
            If e.Row.DataItem("PromotionType") = "Percentage" Then ltlDiscount.Text = FormatPercent(e.Row.DataItem("Discount") / 100) Else ltlDiscount.Text = FormatCurrency(e.Row.DataItem("Discount"))
        End If
    End Sub
End Class
