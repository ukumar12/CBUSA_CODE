Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
	Inherits AdminPage

    Protected VendorID As Integer

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		CheckAccess("VENDOR_PRODUCT_PRICES")

        VendorID = Request("VendorID")
        If VendorID = Nothing Then Response.Redirect("/admin/vendor/default.aspx")

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
	
			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then gvList.SortBy = "VendorID"

			BindList()
		End If
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL as String
		Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " vpp.*, p.Product as ProductName "
        SQL = " FROM VendorProductPrice vpp inner join Product p on vpp.ProductID=p.ProductID "

        SQL &= " where VendorID=" & DB.Number(VendorID)
        Conn = " and "

		gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)
				
		Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
		gvList.DataSource = res.DefaultView
		gvList.DataBind()
	End Sub

    Protected Sub btnRollPricing_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRollPricing.Click
        Dim sql As String = _
              " update VendorProductPrice set " _
            & "     VendorPrice = Coalesce(NextPrice,VendorPrice)," _
            & "     NextPrice = null," _
            & "     NextPriceApplies = null," _
            & "     IsDiscontinued = 0" _
            & " where " _
            & "     VendorID=" & DB.Number(VendorID)

        DB.ExecuteSQL(sql)
        BindList()
        ScriptManager.RegisterStartupScript(Page, Me.GetType, "Alert", "Sys.Application.add_load(function() {Sys.Application.remove_load(arguments.callee);alert('Pricing has been updated');});", True)
    End Sub
End Class
