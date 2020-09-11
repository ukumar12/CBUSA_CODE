Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("SHIPPING_TAX")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_CountryCode.Text = Request("F_CountryCode")
            F_CountryName.Text = Request("F_CountryName")
            F_ShippingLBound.Text = Request("F_ShippingLBound")
            F_ShippingUBound.Text = Request("F_ShippingUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "CountryName"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM Country  "

        If Not F_CountryCode.Text = String.Empty Then
            SQL = SQL & Conn & "CountryCode LIKE " & DB.FilterQuote(F_CountryCode.Text)
            Conn = " AND "
        End If
        If Not F_CountryName.Text = String.Empty Then
            SQL = SQL & Conn & "CountryName LIKE " & DB.FilterQuote(F_CountryName.Text)
            Conn = " AND "
        End If
        If Not F_ShippingLBound.Text = String.Empty Then
            SQL = SQL & Conn & "Shipping >= " & DB.Number(F_ShippingLBound.Text)
            Conn = " AND "
        End If
        If Not F_ShippingUBound.Text = String.Empty Then
            SQL = SQL & Conn & "Shipping <= " & DB.Number(F_ShippingUBound.Text)
            Conn = " AND "
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub
End Class

