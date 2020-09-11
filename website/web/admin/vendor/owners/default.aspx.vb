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
			F_FirstName.Text = Request("F_FirstName")
			F_LastName.Text = Request("F_LastName")
			F_Phone.Text = Request("F_Phone")
	
			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then gvList.SortBy = "VendorOwnerID"

			BindList()
		End If
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL as String
		Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
		SQL = " FROM VendorOwners  "

		if not F_Phone.Text = String.Empty then
			SQL = SQL & Conn & "Phone = " & DB.Quote(F_Phone.Text)
			Conn = " AND "
		end if
		if not F_FirstName.Text = String.Empty then
			SQL = SQL & Conn & "FirstName LIKE " & DB.FilterQuote(F_FirstName.Text)
			Conn = " AND "
		end if
		if not F_LastName.Text = String.Empty then
			SQL = SQL & Conn & "LastName LIKE " & DB.FilterQuote(F_LastName.Text)
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
