Imports Components
Imports DataLayer

Partial Class StoreRelatedItems
    Inherits BaseControl

    Public Property ItemId() As Integer
        Get
            Return ViewState("ItemId")
        End Get
        Set(ByVal value As Integer)
            ViewState("ItemId") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindData()
        End If
    End Sub

    Private Sub BindData()
        Dim dt As DataTable = StoreItemRow.GetRelatedItems(DB, ItemId, 4)
        If dt.Rows.Count = 0 Then Visible = False
        rptRelated.DataSource = dt
        rptRelated.DataBind()
    End Sub

    Protected Sub rptRelated_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptRelated.ItemDataBound
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
