Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
	Inherits AdminPage

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		CheckAccess("AUTOMATIC_MESSAGES")

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
            If Not LoggedInIsInternal Then
                AddNew.Visible = False
            End If
			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "Title"

			BindList()
		End If
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL as String
		Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
		SQL = " FROM AutomaticMessages  "

        If F_Subject.Text <> Nothing Then
            SQL &= Conn & "Subject like " & DB.FilterQuote(F_Subject.Text)
            Conn = " and "
        End If

        If F_Title.Text <> Nothing Then
            SQL &= Conn & "Title like " & DB.FilterQuote(F_Title.Text)
            Conn = " and "
        End If

		gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)
				
		Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
		gvList.DataSource = res.DefaultView
		gvList.DataBind()
	End Sub

	Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
		Response.Redirect("edit.aspx?" & GetPageParams(FilterFieldType.All))
	End Sub

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub

        Dim ltl As Literal = e.Row.FindControl("ltlMessage")
        Dim msg As String = Convert.ToString(e.Row.DataItem("Message"))
        Dim len As Integer = SysParam.GetValue(DB, "PreviewTextLength")
        If msg.Length > len Then
            msg = msg.Substring(0, Math.Min(msg.Length, msg.LastIndexOf(" ", len))) & "..."
        End If
        ltl.Text = msg
    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        BindList()
    End Sub
End Class
