Imports Components
Imports DataLayer

Partial Class StoreRecentlyViewed
    Inherits ModuleControl

    Private ItemId As Integer
    Private MemberId As Integer
    Private NofItems As Integer = 3

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		ItemId = IIf(IsNumeric(Request("ItemId")), Request("ItemId"), 0)
        MemberId = IIf(Session("MemberId") Is Nothing, 0, Session("MemberId"))
        BindData()
    End Sub

    Private Sub BindData()
        Dim dt As DataTable = StoreItemRow.GetRecentlyViewedItems(DB, NofItems, ItemId, MemberId, Session.SessionID)
        If dt.Rows.Count = 0 Then Visible = False
        rptRecentlyViewed.DataSource = dt
        rptRecentlyViewed.DataBind()
    End Sub

    Protected Sub rptRecentlyViewed_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptRecentlyViewed.ItemDataBound
        If Not e.Item.ItemType = ListItemType.Item AndAlso Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If
        Dim lnkItem As HtmlAnchor = e.Item.FindControl("lnkItem")
        If IsDBNull(e.Item.DataItem("CustomURL")) Then
            lnkItem.HRef = "/store/item.aspx?ItemId=" & e.Item.DataItem("ItemId")
        Else
            lnkItem.HRef = e.Item.DataItem("CustomURL")
        End If
        Dim ltlPrice As HtmlGenericControl = e.Item.FindControl("ltlPrice")
        If e.Item.DataItem("IsOnSale") Then
            ltlPrice.InnerHtml = "<strike>" & FormatCurrency(e.Item.DataItem("Price")) & "</strike><span class=""red"">" & FormatCurrency(e.Item.DataItem("SalePrice")) & "</span>"
        Else
            ltlPrice.InnerHtml = FormatCurrency(e.Item.DataItem("Price"))
        End If
    End Sub


End Class
