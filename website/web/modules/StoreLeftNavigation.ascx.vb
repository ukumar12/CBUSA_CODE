Option Strict Off

Imports Components
Imports DataLayer

Partial Class StoreLeftNavigation
    Inherits ModuleControl

    Private dtChildrenDepartments As DataTable
    Private DepartmentId As Integer
    Private BrandId As Integer

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
		DepartmentId = IIf(IsNumeric(Request("DepartmentId")), Request("DepartmentId"), 0)
		BrandId = IIf(IsNumeric(Request("BrandId")), Request("BrandId"), 0)

        Dim dbDepartment As StoreDepartmentRow = StoreDepartmentRow.GetRow(DB, DepartmentId)
        Dim dbDefault As StoreDepartmentRow = StoreDepartmentRow.GetDefaultDepartment(DB)
        Dim dbBrand As StoreBrandRow = StoreBrandRow.GetRow(DB, BrandId)

        rptBrands.DataSource = StoreBrandRow.GetActiveBrands(DB)
        rptBrands.DataBind()

        If dbDepartment.DepartmentId = 0 Then dbDepartment = dbDefault
        dtChildrenDepartments = dbDepartment.GetChildrenDepartments

        Dim dbParent As StoreDepartmentRow = StoreDepartmentRow.GetRow(DB, dbDepartment.ParentId)
        ltlHeader.Text = dbDepartment.Name
        If dbParent.CustomURL = String.Empty Then
            If dbParent.DepartmentId = 0 Then
                lnkBack.HRef = "/store/"
            Else
                If dbParent.ParentId = dbDefault.DepartmentId Then
                    lnkBack.HRef = "/store/main.aspx?DepartmentId=" & dbParent.DepartmentId
                Else
                    lnkBack.HRef = "/store/default.aspx?DepartmentId=" & dbParent.DepartmentId
                End If
            End If
        Else
            lnkBack.HRef = dbParent.CustomURL
        End If
        If Not dbParent.DepartmentId = 0 Then lnkBack.InnerHtml = "&laquo; back to " & dbParent.Name
        If Not dbBrand.BrandId = 0 Then lnkBack.InnerHtml = "&laquo; back to Store"

        If dtChildrenDepartments.Rows.Count = 0 Then
            dtChildrenDepartments = dbParent.GetChildrenDepartments
        End If
        rptDepartments.DataSource = dtChildrenDepartments
        rptDepartments.DataBind()
    End Sub

    Protected Sub rptDepartments_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptDepartments.ItemDataBound
        If Not e.Item.ItemType = ListItemType.Item AndAlso Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If
        Dim liSel As HtmlGenericControl = e.Item.FindControl("liSel")
        Dim liLnk As HtmlGenericControl = e.Item.FindControl("liLnk")
        If DepartmentId = e.Item.DataItem("DepartmentId") Then
            liSel.Visible = True
            liLnk.Visible = False
        Else
            liSel.Visible = False
            liLnk.Visible = True
        End If
    End Sub

    Protected Sub rptBrands_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptBrands.ItemDataBound
        If Not e.Item.ItemType = ListItemType.Item AndAlso Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If
        Dim liSel As HtmlGenericControl = e.Item.FindControl("liSel")
        Dim liLnk As HtmlGenericControl = e.Item.FindControl("liLnk")
        If BrandId = e.Item.DataItem("BrandId") Then
            liSel.Visible = True
            liLnk.Visible = False
        Else
            liSel.Visible = False
            liLnk.Visible = True
        End If
    End Sub

End Class
