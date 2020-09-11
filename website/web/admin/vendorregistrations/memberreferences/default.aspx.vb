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
			F_FirstName.Text = Request("F_FirstName")
			F_LastName.Text = Request("F_LastName")
			F_CompanyName.Text = Request("F_CompanyName")
	
			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then gvList.SortBy = "VendorRegistrationMemberReferenceID"

			BindList()
		End If
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL as String
		Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
		SQL = " FROM VendorRegistrationMemberReference  "

		if not F_FirstName.Text = String.Empty then
			SQL = SQL & Conn & "FirstName LIKE " & DB.FilterQuote(F_FirstName.Text)
			Conn = " AND "
		end if
		if not F_LastName.Text = String.Empty then
			SQL = SQL & Conn & "LastName LIKE " & DB.FilterQuote(F_LastName.Text)
			Conn = " AND "
		end if
		if not F_CompanyName.Text = String.Empty then
			SQL = SQL & Conn & "CompanyName LIKE " & DB.FilterQuote(F_CompanyName.Text)
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