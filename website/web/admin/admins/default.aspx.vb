Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("USERS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_Username.Text = Request("F_Username")
            F_FirstName.Text = Request("F_FirstName")
            F_LastName.Text = Request("F_LastName")
            F_IsLocked.Text = Request("F_IsLocked")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "AdminId"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM Admin  "

        If Not F_Username.Text = String.Empty Then
            SQL = SQL & Conn & "Username = " & DB.Quote(F_Username.Text)
            Conn = " AND "
        End If
        If Not F_FirstName.Text = String.Empty Then
            SQL = SQL & Conn & "FirstName LIKE " & DB.FilterQuote(F_FirstName.Text)
            Conn = " AND "
        End If
        If Not F_LastName.Text = String.Empty Then
            SQL = SQL & Conn & "LastName LIKE " & DB.FilterQuote(F_LastName.Text)
            Conn = " AND "
        End If
        If Not F_IsLocked.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "IsLocked  = " & DB.Number(F_IsLocked.SelectedValue)
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
