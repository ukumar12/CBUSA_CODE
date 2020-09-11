Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
	Inherits AdminPage

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		CheckAccess("ORDER_DROPS")

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
            F_State.DataSource = StateRow.GetStateList(DB)
			F_State.DataValueField = "StateId"
			F_State.DataTextField = "StateName"
			F_State.Databind
			F_State.Items.Insert(0, New ListItem("-- ALL --",""))
	
			F_DropName.Text = Request("F_DropName")
			F_City.Text = Request("F_City")
			F_State.SelectedValue = Request("F_State")
			F_RequestedDeliveryLBound.Text = Request("F_RequestedDeliveryLBound")
			F_RequestedDeliveryUBound.Text = Request("F_RequestedDeliveryUBound")
			F_CreatedLBound.Text = Request("F_CreatedLBound")
			F_CreatedUBound.Text = Request("F_CreatedUBound")
			F_CreatorBuilderIDLBound.Text = Request("F_CreatorBuilderIDLBound")
			F_CreatorBuilderIDUBound.Text = Request("F_CreatorBuilderIDUBound")
	
			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then gvList.SortBy = "OrderDropID"

			BindList()
		End If
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL as String
		Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
		SQL = " FROM OrderDrop  "

		if not F_State.SelectedValue = String.Empty then
			SQL = SQL & Conn & "State = " & DB.Quote(F_State.SelectedValue)
			Conn = " AND "
		end if
		if not F_DropName.Text = String.Empty then
			SQL = SQL & Conn & "DropName LIKE " & DB.FilterQuote(F_DropName.Text)
			Conn = " AND "
		end if
		if not F_City.Text = String.Empty then
			SQL = SQL & Conn & "City LIKE " & DB.FilterQuote(F_City.Text)
			Conn = " AND "
		end if
		if not F_RequestedDeliveryLBound.Text = String.Empty then
			SQL = SQL & Conn & "RequestedDelivery >= " & DB.Quote(F_RequestedDeliveryLBound.Text)
			Conn = " AND "
		end if
		if not F_RequestedDeliveryUBound.Text = String.Empty then
			SQL = SQL & Conn & "RequestedDelivery < " & DB.Quote(DateAdd("d", 1, F_RequestedDeliveryUBound.Text))
			Conn = " AND "
		end if
		if not F_CreatedLBound.Text = String.Empty then
			SQL = SQL & Conn & "Created >= " & DB.Quote(F_CreatedLBound.Text)
			Conn = " AND "
		end if
		if not F_CreatedUBound.Text = String.Empty then
			SQL = SQL & Conn & "Created < " & DB.Quote(DateAdd("d", 1, F_CreatedUBound.Text))
			Conn = " AND "
		end if
		if not F_CreatorBuilderIDLBound.Text = String.Empty then
			SQL = SQL & Conn & "CreatorBuilderID >= " & DB.Number(F_CreatorBuilderIDLBound.Text)
			Conn = " AND "
		end if
		if not F_CreatorBuilderIDUBound.Text = String.Empty then
			SQL = SQL & Conn & "CreatorBuilderID <= " & DB.Number(F_CreatorBuilderIDUBound.Text)
			Conn = " AND "
		end if
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
