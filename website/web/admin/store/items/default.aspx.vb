Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private dsDepartments As DataTable

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("STORE")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_TemplateId.DataSource = StoreItemTemplateRow.GetTemplates(DB)
            F_TemplateId.DataValueField = "TemplateId"
            F_TemplateId.DataTextField = "TemplateName"
            F_TemplateId.DataBind()
            F_TemplateId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_BrandId.DataSource = StoreBrandRow.GetBrands(DB)
            F_BrandId.DataValueField = "BrandId"
            F_BrandId.DataTextField = "Name"
            F_BrandId.DataBind()
            F_BrandId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_ItemName.Text = Request("F_ItemName")
            F_SKU.Text = Request("F_SKU")
            F_IsOnSale.Text = Request("F_IsOnSale")
            F_IsActive.Text = Request("F_IsActive")
            F_IsFeatured.Text = Request("F_IsFeatured")
            F_TemplateId.SelectedValue = Request("F_TemplateId")
            F_BrandId.SelectedValue = Request("F_BrandId")
            F_DepartmentId.SelectedValue = Request("F_DepartmentId")

            BindDepartmentsDropDown(DB, F_DepartmentId, 5, "-- ALL --")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "ItemName"

            BindList()
        End If
    End Sub

    Private Sub BindDepartmentsDropDown(ByVal DB As Database, ByVal DepartmentId As DropDownList, ByVal Level As Integer, ByVal FirstField As String)
        Dim SQL As String = String.Empty
        If Level = Nothing Then
            SQL = "SELECT * FROM StoreDepartment ORDER BY NAME ASC"
        Else
            SQL = " SELECT P1.DepartmentId, REPLICATE('   ', COUNT(P2.DepartmentId)-2) + p1.NAME AS NAME" _
                & " FROM StoreDepartment P1, StoreDepartment P2" _
                & " WHERE P1.lft BETWEEN P2.lft AND P2.rgt" _
                & " GROUP BY P1.DepartmentId, P1.lft, p1.rgt, p1.NAME" _
                & " HAVING COUNT(P2.DepartmentId) > 1 AND COUNT(P2.DepartmentId) <= " & DB.Quote((Level + 1).ToString) _
                & " ORDER BY P1.lft"
        End If
        Dim ds As DataSet = DB.GetDataSet(SQL)
        DepartmentId.DataSource = ds
        DepartmentId.DataTextField = "Name"
        DepartmentId.DataValueField = "DepartmentId"
        DepartmentId.DataBind()
        DepartmentId.Items.Insert(0, New ListItem(FirstField, ""))
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        'Load Departments
        SQL = "SELECT sdi.ItemId, sd.NAME FROM StoreDepartment sd, StoreDepartmentItem sdi WHERE sd.DepartmentId = sdi.DepartmentId"
        dsDepartments = DB.GetDataTable(SQL)

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " *"
        SQLFields &= ", (SELECT TemplateName FROM StoreItemTemplate st WHERE st.TemplateId = si.TemplateId) As TemplateName "
        SQLFields &= ", (SELECT Name FROM StoreBrand sb WHERE sb.BrandId = si.BrandId) As BrandName "
        SQL = " FROM StoreItem si "

        If Not F_TemplateId.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "si.TemplateId = " & DB.Quote(F_TemplateId.SelectedValue)
            Conn = " AND "
        End If
        If Not F_BrandId.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "si.BrandId = " & DB.Quote(F_BrandId.SelectedValue)
            Conn = " AND "
        End If
        If Not DB.IsEmpty(F_DepartmentId.SelectedValue) Then
            SQL &= Conn & " ItemId in ("
            SQL &= " select ItemId from StoreDepartmentItem sdi, StoreDepartment sd, StoreDepartment p "
            SQL &= " where p.DepartmentId = " & DB.Number(F_DepartmentId.SelectedValue)
            SQL &= " and sdi.DepartmentId = sd.DepartmentId and p.lft <= sd.Lft and p.rgt >= sd.rgt"
            SQL &= " )"
            Conn = " AND "
        End If
        If Not F_ItemName.Text = String.Empty Then
            SQL = SQL & Conn & "ItemName LIKE " & DB.FilterQuote(F_ItemName.Text)
            Conn = " AND "
        End If
        If Not F_SKU.Text = String.Empty Then
			SQL = SQL & Conn & "(si.SKU LIKE " & DB.FilterQuote(F_SKU.Text) & " OR si.ItemId IN (SELECT ItemId FROM StoreItemAttribute WHERE FinalSKU LIKE " & DB.FilterQuote(F_SKU.Text) & "))"
            Conn = " AND "
        End If
        If Not F_IsOnSale.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "IsOnSale  = " & DB.Number(F_IsOnSale.SelectedValue)
            Conn = " AND "
        End If
        If Not F_IsActive.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "IsActive  = " & DB.Number(F_IsActive.SelectedValue)
            Conn = " AND "
        End If
        If Not F_IsFeatured.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "IsFeatured  = " & DB.Number(F_IsFeatured.SelectedValue)
            Conn = " AND "
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim SortByAndOrder As String = gvList.SortByAndOrder
        If gvList.SortBy = "Price" Then
            SortByAndOrder = "case when IsOnSale = 1 then SalePrice else Price End " & gvList.SortOrder
        End If

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not (e.Row.RowType = ListItemType.Item Or e.Row.RowType = ListItemType.AlternatingItem) Then
            Exit Sub
        End If

        Dim img As Literal= CType(e.Row.FindControl("img"), Literal)
        Dim imglink As Label = CType(e.Row.FindControl("imglink"), Label)
        Dim noimg As Label = CType(e.Row.FindControl("noimg"), Label)
        If Convert.ToString(e.Row.DataItem("Image")) <> String.Empty Then
            img.Text = "<img src=""/assets/item/thumbnail/" & e.Row.DataItem("Image") & """>"
            imglink.Visible = True
            noimg.Visible = False
        Else
            imglink.Visible = False
            noimg.Visible = True
        End If

        Dim ltlPrice As Literal = CType(e.Row.FindControl("ltlPrice"), Literal)
        If e.Row.DataItem("IsOnSale") Then
            ltlPrice.Text = "<span class=""red"">" & FormatCurrency(e.Row.DataItem("SalePrice"), 2) & "</span>"
        Else
            ltlPrice.Text = FormatCurrency(e.Row.DataItem("Price"), 2)
        End If

        dsDepartments.DefaultView.RowFilter = "ItemId = " & e.Row.DataItem("ItemId")

        Dim Departments As Repeater = CType(e.Row.FindControl("Departments"), Repeater)
        Departments.DataSource = dsDepartments.DefaultView
        Departments.DataBind()
    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub
End Class
