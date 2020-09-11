Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
	Inherits AdminPage

    Protected VendorRegistrationId As Integer = 0

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("VENDOR_REGISTRATIONS")

        If Request("VendorRegistrationId") <> String.Empty Then
            VendorRegistrationId = CType(Request("VendorRegistrationId"), Integer)
        Else
            Response.Redirect("/admin/default.aspx")
        End If

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
            F_State.DataSource = StateRow.GetStateList(DB)
            F_State.DataValueField = "StateCode"
			F_State.DataTextField = "StateCode"
			F_State.Databind
			F_State.Items.Insert(0, New ListItem("-- ALL --",""))
	
			F_FirstName.Text = Request("F_FirstName")
			F_LastName.Text = Request("F_LastName")
			F_City.Text = Request("F_City")
			F_Zip.Text = Request("F_Zip")
			F_Phone.Text = Request("F_Phone")
			F_State.SelectedValue = Request("F_State")
	
			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then gvList.SortBy = "VendorRegistrationCustomerReferenceID"

			BindList()
		End If
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL as String
		Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
		SQL = " FROM VendorRegistrationCustomerReference  "

		if not F_Phone.Text = String.Empty then
			SQL = SQL & Conn & "Phone = " & DB.Quote(F_Phone.Text)
			Conn = " AND "
		end if
		if not F_State.SelectedValue = String.Empty then
			SQL = SQL & Conn & "State = " & DB.Quote(F_State.SelectedValue)
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
		if not F_City.Text = String.Empty then
			SQL = SQL & Conn & "City LIKE " & DB.FilterQuote(F_City.Text)
			Conn = " AND "
		end if
		if not F_Zip.Text = String.Empty then
			SQL = SQL & Conn & "Zip LIKE " & DB.FilterQuote(F_Zip.Text)
			Conn = " AND "
        End If
        SQL = SQL & Conn & " VendorRegistrationId = " & DB.Number(VendorRegistrationId)
        Conn = " AND "

		gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

		Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
		gvList.DataSource = res.DefaultView
		gvList.DataBind()
	End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?VendorRegistrationId=" & VendorRegistrationId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

	Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
		If Not IsValid Then Exit Sub
		
		gvList.PageIndex = 0
		BindList()
    End Sub

    Protected Sub btnBackToRegistration_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBackToRegistration.Click
        Response.Redirect("../edit.aspx?VendorRegistrationID=" & VendorRegistrationId)
    End Sub
End Class
