Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
	Inherits AdminPage

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		CheckAccess("SALES_REPORT_DISPUTES")

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
            F_DisputeResponseID.Datasource = DisputeResponseRow.GetList(DB, "DisputeResponse")
            F_DisputeResponseID.DataValueField = "DisputeResponseID"
            F_DisputeResponseID.DataTextField = "DisputeResponse"
            F_DisputeResponseID.Databind()
            F_DisputeResponseID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_DisputeResponseReasonID.Datasource = DisputeResponseReasonRow.GetList(DB, "DisputeResponseReason")
            F_DisputeResponseReasonID.DataValueField = "DisputeResponseReasonID"
            F_DisputeResponseReasonID.DataTextField = "DisputeResponseReason"
            F_DisputeResponseReasonID.Databind()
            F_DisputeResponseReasonID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_DisputeResponseID.SelectedValue = Request("F_DisputeResponseID")
            F_DisputeResponseReasonID.SelectedValue = Request("F_DisputeResponseReasonID")
            F_SalesReportIDLBound.Text = Request("F_SalesReportIDLBound")
            F_SalesReportIDUBound.Text = Request("F_SalesReportIDUBound")
            F_CreatedLBound.Text = Request("F_CreatedLBound")
            F_CreatedUBound.Text = Request("F_CreatedUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "SalesReportDisputeID"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM SalesReportDispute  "

        If Not F_DisputeResponseID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "DisputeResponseID = " & DB.Quote(F_DisputeResponseID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_DisputeResponseReasonID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "DisputeResponseReasonID = " & DB.Quote(F_DisputeResponseReasonID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_CreatedLBound.Text = String.Empty Then
            SQL = SQL & Conn & "Created >= " & DB.Quote(F_CreatedLBound.Text)
            Conn = " AND "
        End If
        If Not F_CreatedUBound.Text = String.Empty Then
            SQL = SQL & Conn & "Created < " & DB.Quote(DateAdd("d", 1, F_CreatedUBound.Text))
            Conn = " AND "
        End If
        If Not F_SalesReportIDLBound.Text = String.Empty Then
            SQL = SQL & Conn & "SalesReportID >= " & DB.Number(F_SalesReportIDLBound.Text)
            Conn = " AND "
        End If
        If Not F_SalesReportIDUBound.Text = String.Empty Then
            SQL = SQL & Conn & "SalesReportID <= " & DB.Number(F_SalesReportIDUBound.Text)
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
		If Not IsValid Then Exit Sub
		
		gvList.PageIndex = 0
		BindList()
	End Sub
End Class
