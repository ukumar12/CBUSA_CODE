Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("ORDER_STATUS_HISTORYS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_OrderStatusID.Datasource = OrderStatusRow.GetList(DB, "OrderStatus")
            F_OrderStatusID.DataValueField = "OrderStatusID"
            F_OrderStatusID.DataTextField = "OrderStatus"
            F_OrderStatusID.Databind()
            F_OrderStatusID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_OrderID.DataSource = OrderRow.GetList(DB, "OrderNumber")
            F_OrderID.DataValueField = "OrderID"
            F_OrderID.DataTextField = "OrderNumber"
            F_OrderID.DataBind()
            F_OrderID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_OrderStatusID.SelectedValue = Request("F_OrderStatusID")
            F_OrderID.SelectedValue = Request("F_OrderID")
            F_CreatedLbound.Text = Request("F_CreatedLBound")
            F_CreatedUBound.Text = Request("F_CreatedUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "OrderStatusHistoryID"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " h.*, o.OrderNumber, s.OrderStatus "
        SQL = " FROM OrderStatusHistory h INNER JOIN Order o on h.OrderID=o.OrderID " _
            & " INNER JOIN OrderStatus s ON h.OrderStatusID=s.OrderStatusID"

        If Not F_OrderStatusID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "OrderStatusID = " & DB.Quote(F_OrderStatusID.SelectedValue)
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
        If Not F_OrderID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "OrderID = " & DB.Number(F_OrderID.SelectedValue)
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

