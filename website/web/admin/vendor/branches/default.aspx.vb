Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
	Inherits AdminPage
    
    Protected VendorId As Integer = 0

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		CheckAccess("VENDORS")
        
        If Request("VendorId") <> String.Empty Then
            VendorId = CType(Request("VendorId"), Integer)
        Else
            Response.Redirect("/admin/default.aspx")
        End If

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
			F_State.Datasource = StateRow.GetStateList(DB)
			F_State.DataValueField = "StateCode"
			F_State.DataTextField = "StateCode"
			F_State.Databind
			F_State.Items.Insert(0, New ListItem("-- ALL --",""))
	
			F_Address.Text = Request("F_Address")
			F_City.Text = Request("F_City")
			F_Zip.Text = Request("F_Zip")
			F_State.SelectedValue = Request("F_State")
	
			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then gvList.SortBy = "VendorBranchOfficeID"

			BindList()
		End If
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL as String
		Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
		SQL = " FROM VendorBranchOffice  "

		if not F_State.SelectedValue = String.Empty then
			SQL = SQL & Conn & "State = " & DB.Quote(F_State.SelectedValue)
			Conn = " AND "
		end if
		if not F_Address.Text = String.Empty then
			SQL = SQL & Conn & "Address LIKE " & DB.FilterQuote(F_Address.Text)
			Conn = " AND "
		end if
		if not F_City.Text = String.Empty then
			SQL = SQL & Conn & "City LIKE " & DB.FilterQuote(F_City.Text)
			Conn = " AND "
		end if
		if not F_Zip.Text = String.Empty then
			SQL = SQL & Conn & "Zip LIKE " & DB.FilterQuote(F_Zip.Text)
			Conn = " AND "
		end if


        SQL = SQL & Conn & " VendorId = " & DB.Number(VendorId)
        Conn = " AND "
		gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)
				
		Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
		gvList.DataSource = res.DefaultView
		gvList.DataBind()
	End Sub

	Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
		Response.Redirect("edit.aspx?VendorId=" & VendorId & "&" & GetPageParams(FilterFieldType.All))
	End Sub

	Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
		If Not IsValid Then Exit Sub
		
		gvList.PageIndex = 0
		BindList()
	End Sub

    Protected Sub btnBackToVendor_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBackToVendor.Click
        Response.Redirect("../edit.aspx?VendorID=" & VendorId)
    End Sub
End Class
