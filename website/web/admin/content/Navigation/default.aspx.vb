Imports Components
Imports Controls
Imports System.Data

Partial Class Index
    Inherits AdminPage

    Dim dsSubSections As DataTable

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("CONTENT_TOOL")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim res As DataTable = DB.GetDataTable("SELECT * FROM ContentToolNavigation where ParentId is null  ORDER BY SortOrder")
        dsSubSections = DB.GetDataTable("select * from ContentToolNavigation where ParentId is not null order by ParentId, SortOrder")
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not e.Row.RowType = DataControlRowType.DataRow Then Exit Sub

        Dim gvSubList As GridView = e.Row.FindControl("gvSubList")
        dsSubSections.DefaultView.RowFilter = "ParentId = " & e.Row.DataItem("NavigationId")
        gvSubList.DataSource = dsSubSections.DefaultView
        gvSubList.DataBind()
    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

End Class

