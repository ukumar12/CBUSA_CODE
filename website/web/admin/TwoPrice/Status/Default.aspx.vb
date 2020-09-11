Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient
Imports TwoPrice.DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("TWO_PRICE_CAMPAIGNS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_Name.Text = Request("F_Name")
            F_Value.Text = Request("F_Value")
            F_IsActive.Text = Request("F_IsActive")
            F_CreateDateLBound.Text = Request("F_CreateDateLBound")
            F_CreateDateUBound.Text = Request("F_CreateDateUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            gvList.PageIndex = IIf(Request("F_pg") = String.Empty, 0, Core.ProtectParam(Request("F_pg")))

            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "SortOrder"
                gvList.SortOrder = ""
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

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM TwoPriceStatus  "

        If Not F_Name.Text = String.Empty Then
            SQL = SQL & Conn & "Name LIKE " & DB.FilterQuote(F_Name.Text)
            Conn = " AND "
        End If
        If Not F_Value.Text = String.Empty Then
            SQL = SQL & Conn & "Value LIKE " & DB.FilterQuote(F_Value.Text)
            Conn = " AND "
        End If
        If Not F_IsActive.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "IsActive  = " & DB.Number(F_IsActive.SelectedValue)
            Conn = " AND "
        End If
        If Not F_CreateDateLBound.Text = String.Empty Then
            SQL = SQL & Conn & "CreateDate >= " & DB.Quote(F_CreateDateLBound.Text)
            Conn = " AND "
        End If
        If Not F_CreateDateUBound.Text = String.Empty Then
            SQL = SQL & Conn & "CreateDate < " & DB.Quote(DateAdd("d", 1, F_CreateDateUBound.Text))
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