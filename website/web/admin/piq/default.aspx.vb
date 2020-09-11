Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
	Inherits AdminPage

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		CheckAccess("PIQ")

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
            F_State.DataSource = StateRow.GetStateList(DB)
            F_State.DataValueField = "StateCode"
			F_State.DataTextField = "StateName"
			F_State.Databind
			F_State.Items.Insert(0, New ListItem("-- ALL --",""))
	
			F_CompanyName.Text = Request("F_CompanyName")
			F_City.Text = Request("F_City")
			F_LastName.Text = Request("F_LastName")
			F_Email.Text = Request("F_Email")
			F_IsActive.Text = Request("F_IsActive")
			F_State.SelectedValue = Request("F_State")
			F_StartDateLBound.Text = Request("F_StartDateLBound")
			F_StartDateUBound.Text = Request("F_StartDateUBound")
			F_EndDateLBound.Text = Request("F_EndDateLBound")
            F_EndDateUbound.Text = Request("F_EndDateUBound")
            F_HasCatalogAccess.Text = Request("F_HasCatalogAccess")
	
			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then gvList.SortBy = "StartDate"

			BindList()
		End If
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL as String
		Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
		SQL = " FROM PIQ  "

		if not F_State.SelectedValue = String.Empty then
			SQL = SQL & Conn & "State = " & DB.Quote(F_State.SelectedValue)
			Conn = " AND "
		end if
		if not F_CompanyName.Text = String.Empty then
			SQL = SQL & Conn & "CompanyName LIKE " & DB.FilterQuote(F_CompanyName.Text)
			Conn = " AND "
		end if
		if not F_City.Text = String.Empty then
			SQL = SQL & Conn & "City LIKE " & DB.FilterQuote(F_City.Text)
			Conn = " AND "
		end if
		if not F_LastName.Text = String.Empty then
			SQL = SQL & Conn & "LastName LIKE " & DB.FilterQuote(F_LastName.Text)
			Conn = " AND "
		end if
		if not F_Email.Text = String.Empty then
			SQL = SQL & Conn & "Email LIKE " & DB.FilterQuote(F_Email.Text)
			Conn = " AND "
		end if
		If Not F_IsActive.SelectedValue = String.Empty Then
			SQL = SQL & Conn & "IsActive  = " & DB.Number(F_IsActive.SelectedValue)
			Conn = " AND "
		end if
		if not F_StartDateLBound.Text = String.Empty then
			SQL = SQL & Conn & "StartDate >= " & DB.Quote(F_StartDateLBound.Text)
			Conn = " AND "
		end if
		if not F_StartDateUBound.Text = String.Empty then
			SQL = SQL & Conn & "StartDate < " & DB.Quote(DateAdd("d", 1, F_StartDateUBound.Text))
			Conn = " AND "
		end if
		if not F_EndDateLBound.Text = String.Empty then
			SQL = SQL & Conn & "EndDate >= " & DB.Quote(F_EndDateLBound.Text)
			Conn = " AND "
		end if
		if not F_EndDateUBound.Text = String.Empty then
			SQL = SQL & Conn & "EndDate < " & DB.Quote(DateAdd("d", 1, F_EndDateUBound.Text))
			Conn = " AND "
        End If
        If Not F_HasCatalogAccess.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "HasCatalogAccess  = " & DB.Number(F_HasCatalogAccess.SelectedValue)
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
