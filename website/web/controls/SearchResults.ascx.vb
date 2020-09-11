Imports DataLayer
Imports Components
Partial Class SearchItem
    Inherits ModuleControl

    Private m_Item As StoreItemRow
    Private m_Member As MemberRow
    Private m_BrandId As Integer = Nothing
    Private m_DepartmentId As Integer = Nothing

    Public Property Item() As StoreItemRow
        Get
            Return m_Item
        End Get
        Set(ByVal value As StoreItemRow)
            m_Item = value
        End Set
    End Property

    Public Property Member() As MemberRow
        Get
            Return m_Member
        End Get
        Set(ByVal value As MemberRow)
            m_Member = value
        End Set
    End Property

    Protected ReadOnly Property URL() As String
        Get
            Dim qs As URLParameters = New URLParameters(Request.QueryString, "sm;SectionId;grade;ItemId")
            qs.Add("ItemId", Item.ItemId)
            Return "/store/item.aspx" & qs.ToString()
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Item Is Nothing Then
            ltlPrice.Text = "<span class=""bold price "
            If Item.IsOnSale = False Then ltlPrice.Text &= "blue"">" Else ltlPrice.Text &= "red"">"
            If Item.IsOnSale = False Then
                ltlPrice.Text &= FormatCurrency(Item.Price)
            Else
                ltlPrice.Text &= FormatCurrency(Item.SalePrice)
            End If

            ltlPrice.Text &= "</span>"

            If Item.InventoryAction = "In Stock" Then
                pnlInStock.Visible = True
                pnlNotInStock.Visible = False
            Else
                pnlInStock.Visible = False
                pnlNotInStock.Visible = True
                ltlStockMessage.Text = "<a href=""" & URL & """>" & Item.InventoryAction & "</a>"
            End If
        End If
    End Sub

    Protected Sub btnAdd2Cart_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnAdd2Cart.Click
        Session("DepartmentId") = Nothing
        Session("LastDepartmentId") = Nothing
        Session("LastBrandId") = Nothing
        Session("LastItemId") = Nothing

        If m_BrandId > 0 Then Session("LastBrandId") = m_BrandId
        If m_DepartmentId > 0 Then Session("LastDepartmentId") = m_DepartmentId


        Dim DepartmentId As Integer = Convert.ToInt32(Request.QueryString("DepartmentId"))
        Dim BrandId As Integer = Convert.ToInt32(Request.QueryString("BrandId"))
        ShoppingCart.Add2Cart(DB, Session("OrderId"), Member.MemberId, Item.ItemId, 1, DepartmentId, BrandId, "Myself")
        Item.DB.Close()
        Response.Redirect("/store/bag.aspx")
    End Sub


End Class
