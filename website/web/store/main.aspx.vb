Imports Components
Imports DataLayer

Partial Class store_default
    Inherits SitePage

    Private Filter As New DepartmentFilterField
    Private SubDepartmentCounter As Integer = 0
    Private ItemsCollectionCount As Integer = 0
    Private ItemsCollection As StoreItemCollection
    Dim dbDepartment As StoreDepartmentRow

    Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
		If Not Request("pg") = String.Empty Then MetaRobots = "<meta name=""robots"" content=""noindex,follow"">"

        BindDepartments()
        BindItems()

        'Overwrite default PageTitle and meta tags
        PageTitle = dbDepartment.PageTitle
        If Not dbDepartment.MetaKeywords = String.Empty Then MetaKeywords = dbDepartment.MetaKeywords
        If Not dbDepartment.MetaDescription = String.Empty Then MetaDescription = dbDepartment.MetaDescription

        'Overwrite smartbug URL
        SmartBugUrl = "/admin/default.aspx?FrameURL=" & Server.UrlEncode("/admin/store/departments/edit.aspx?DepartmentId=" & dbDepartment.DepartmentId)

        ltlBreadcrumb.Text = StoreDepartmentRow.DisplayBreadCrumb(DB, dbDepartment.DepartmentId, False)
    End Sub

    Private Sub BindDepartments()
		Dim DepartmentId As Integer = IIf(IsNumeric(Request("DepartmentId")), Request("DepartmentId"), 0)
		dbDepartment = StoreDepartmentRow.GetRow(DB, DepartmentId)
        If dbDepartment.DepartmentId = 0 Then
            dbDepartment = StoreDepartmentRow.GetDefaultDepartment(DB)
        End If
        Session("LastDepartmentId") = dbDepartment.DepartmentId

        'If item is not longer active, then redirect to home page
        If Not dbDepartment.IsActive Then
            Response.Redirect("/")
        End If

        If dbDepartment.ParentId <> StoreDepartmentRow.GetDefaultDepartmentId(DB) Then
            Response.Redirect(Request.Url.PathAndQuery.ToLower.Replace("main.aspx", "default.aspx"))
        End If

        Dim dbStoreDepartment As StoreDepartmentRow = StoreDepartmentRow.GetRow(DB, dbDepartment.DepartmentId)
        Dim dtDepartments As DataTable = dbStoreDepartment.GetChildrenDepartments()

        hTitle.InnerText = dbStoreDepartment.Name
        SubDepartmentCounter = dtDepartments.Rows.Count
        pnlDepartments.Visible = (SubDepartmentCounter > 0)
        rptDepartments.DataSource = dtDepartments
        rptDepartments.DataBind()
    End Sub

    Private Sub rptDepartments_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptDepartments.ItemDataBound
        If Not e.Item.ItemType = ListItemType.Item AndAlso Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If
        Dim lnkDepartment As HtmlAnchor = e.Item.FindControl("lnkDepartment")
        If IsDBNull(e.Item.DataItem("CustomURL")) Then
            Dim PageName As String = "default.aspx"
            If e.Item.DataItem("ParentId") = StoreDepartmentRow.GetDefaultDepartmentId(DB) Then
                PageName = "main.aspx"
            End If
            lnkDepartment.HRef = "/store/" & PageName & "?DepartmentId=" & e.Item.DataItem("DepartmentId")
        Else
            lnkDepartment.HRef = e.Item.DataItem("CustomURL")
        End If
        Dim imgDepartment As HtmlImage = e.Item.FindControl("imgDepartment")
        If IsDBNull(e.Item.DataItem("ViewImage")) Then
            imgDepartment.Src = "/images/spacer.gif"
            imgDepartment.Width = SysParam.GetValue(DB, "StoreDepartmentThumbnailWidth")
            imgDepartment.Height = SysParam.GetValue(DB, "StoreDepartmentThumbnailHeight")
        Else
            imgDepartment.Src = "/assets/department/thumbnail/" & e.Item.DataItem("ViewImage")
            imgDepartment.Width = e.Item.DataItem("ThumbnailWidth")
            imgDepartment.Height = e.Item.DataItem("ThumbnailHeight")
            imgDepartment.Style.Item("margin-top") = CInt((SysParam.GetValue(DB, "StoreDepartmentThumbnailHeight") - e.Item.DataItem("ThumbnailHeight")) / 2) & "px"
            imgDepartment.Style.Item("margin-bottom") = imgDepartment.Style.Item("margin-top")
        End If
		If IsDBNull(e.Item.DataItem("ViewImageAlt")) Then
			imgDepartment.Alt = e.Item.DataItem("Name")
		Else
			imgDepartment.Alt = e.Item.DataItem("ViewImageAlt")
		End If

        Dim divSpacer As HtmlGenericControl = CType(e.Item.FindControl("divSpacer"), HtmlGenericControl)
        divSpacer.Visible = (e.Item.ItemIndex Mod 4 = 3)
        If e.Item.ItemIndex = SubDepartmentCounter - 1 Then divSpacer.Visible = True
    End Sub

    Public Sub BindItems()
        Filter.SortBy = Request("sort")
        Filter.SortOrder = Request("dir")
		Filter.MaxPerPage = IIf(IsNumeric(Request("perpage")), Request("perpage"), IIf(Not Request("F_All") = String.Empty, -1, 12))
		Filter.pg = IIf(IsNumeric(Request("pg")), Request("pg"), 1)
        Filter.IncludeItemsFromSubdepartments = False
        Filter.DepartmentId = dbDepartment.DepartmentId

        ItemsCollection = StoreItemRow.GetActiveItems(DB, Filter)
        ItemsCollectionCount = StoreItemRow.GetActiveItemsCount(DB, Filter)

        NavigatorTop.NofRecords = ItemsCollectionCount
        NavigatorBottom.NofRecords = ItemsCollectionCount
        NavigatorTop.MaxPerPage = Filter.MaxPerPage
        NavigatorBottom.MaxPerPage = Filter.MaxPerPage
		NavigatorTop.Pg = Filter.pg
		NavigatorBottom.Pg = Filter.pg
        NavigatorTop.Sort = Request("sort")
        NavigatorBottom.Sort = Request("sort")
        NavigatorTop.URL = Core.GetURLOnly(Request.RawUrl)
        NavigatorBottom.URL = Core.GetURLOnly(Request.RawUrl)
        If Not dbDepartment.CustomURL = String.Empty Then
            NavigatorTop.Exclude = "departmentid"
            NavigatorBottom.Exclude = "departmentid"
        End If

        pnlItems.Visible = (ItemsCollectionCount > 0)
        rptItems.DataSource = ItemsCollection
        rptItems.DataBind()
    End Sub

    Private Sub rptItems_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptItems.ItemDataBound
        If Not e.Item.ItemType = ListItemType.Item AndAlso Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If
        Dim dbItem As StoreItemRow = CType(e.Item.DataItem, StoreItemRow)
        Dim lnkItem As HtmlAnchor = e.Item.FindControl("lnkItem")
        If dbItem.CustomURL = String.Empty Then
            lnkItem.HRef = "/store/item.aspx?DepartmentId=" & dbDepartment.DepartmentId & "&ItemId=" & dbItem.ItemId & "&" & GetPageParams()
        Else
            lnkItem.HRef = dbItem.CustomURL & "?DepartmentId=" & dbDepartment.DepartmentId
        End If

        Dim imgItem As HtmlImage = e.Item.FindControl("imgItem")
        If dbItem.Image = String.Empty Then
            imgItem.Src = "/images/spacer.gif"
            imgItem.Width = SysParam.GetValue(DB, "StoreItemImageThumbnailWidth")
            imgItem.Height = SysParam.GetValue(DB, "StoreItemImageThumbnailHeight")
        Else
            imgItem.Src = "/assets/item/thumbnail/" & dbItem.Image
            imgItem.Width = dbItem.ThumbnailWidth
            imgItem.Height = dbItem.ThumbnailHeight
            imgItem.Style.Item("margin-top") = CInt((SysParam.GetValue(DB, "StoreItemImageThumbnailHeight") - dbItem.ThumbnailHeight) / 2) & "px"
            imgItem.Style.Item("margin-bottom") = imgItem.Style.Item("margin-top")
        End If

        Dim ltlPrice As HtmlGenericControl = e.Item.FindControl("ltlPrice")
        If dbItem.IsOnSale Then
            ltlPrice.InnerHtml = "<strike>" & FormatCurrency(dbItem.Price) & "</strike> <span class=""red"">" & FormatCurrency(dbItem.SalePrice) & "</span>"
        Else
            ltlPrice.InnerHtml = FormatCurrency(dbItem.Price)
        End If

        Dim divSpacer As HtmlGenericControl = CType(e.Item.FindControl("divSpacer"), HtmlGenericControl)
        divSpacer.Visible = (e.Item.ItemIndex Mod 4 = 3)
        If e.Item.ItemIndex = ItemsCollectionCount - 1 Then divSpacer.Visible = True
    End Sub

End Class
