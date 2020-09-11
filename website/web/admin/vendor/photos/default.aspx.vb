Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("VENDORS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_VendorId.Datasource = VendorRow.GetList(DB, "CompanyName")
            F_VendorId.DataValueField = "VendorID"
            F_VendorId.DataTextField = "CompanyName"
            F_VendorId.Databind()
            F_VendorId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_Caption.Text = Request("F_Caption")
            F_AltText.Text = Request("F_AltText")
            F_VendorId.SelectedValue = Request("F_VendorId")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            gvList.PageIndex = IIf(Request("F_pg") = String.Empty, 0, Core.ProtectParam(Request("F_pg")))

            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "PhotoId"
                gvList.SortOrder = "ASC"
            End If

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder
        ViewState("F_pg") = gvList.PageIndex.ToString

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " *, (Select CompanyName From Vendor Where VendorId = VendorPhoto.VendorId) As Vendor "
        SQL = " FROM VendorPhoto  "

        If Not F_VendorId.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "VendorId = " & DB.Quote(F_VendorId.SelectedValue)
            Conn = " AND "
        End If
        If Not F_Caption.Text = String.Empty Then
            SQL = SQL & Conn & "Caption LIKE " & DB.FilterQuote(F_Caption.Text)
            Conn = " AND "
        End If
        If Not F_AltText.Text = String.Empty Then
            SQL = SQL & Conn & "AltText LIKE " & DB.FilterQuote(F_AltText.Text)
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

