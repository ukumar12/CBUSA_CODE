Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private objTemplate As StoreItemTemplateRow
	Protected IsInUse As Boolean

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("STORE")

        objTemplate = StoreItemTemplateRow.GetRow(DB, Request("TemplateId"))
        If objTemplate.TemplateId = Nothing Then
            DB.Close()
            Response.Redirect("/admin/store/template/")
        End If

		IsInUse = Not DB.ExecuteScalar("select top 1 itemid from storeitem where templateid = " & DB.Number(Request("TemplateId"))) = Nothing
		gvList.Columns(1).Visible = Not IsInUse
		AddNew.Visible = Not IsInUse

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            ltlTemplateName.Text = objTemplate.TemplateName
            AddNew.Text = "Add New " & objTemplate.TemplateName & " Attribute"

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "SortOrder"

            BindList()
        End If
    End Sub

    Private Sub BindList()
		Dim SQL As String = "exec sp_GetTemplateAttributeTreeByTemplate " & objTemplate.TemplateId
		Dim res As DataTable = DB.GetDataTable(SQL)

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

		gvList.Pager.NofRecords = res.Rows.Count

        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?TemplateId=" & objTemplate.TemplateId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

End Class
