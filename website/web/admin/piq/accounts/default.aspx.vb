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
            F_PIQID.Datasource = PIQRow.GetList(DB, "CompanyName")
            F_PIQID.DataValueField = "PIQID"
            F_PIQID.DataTextField = "CompanyName"
            F_PIQID.Databind()
            F_PIQID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_LastName.Text = Request("F_LastName")
            F_Username.Text = Request("F_Username")
            F_IsPrimary.Text = Request("F_IsPrimary")
            F_PIQID.SelectedValue = Request("F_PIQID")
            F_CreatedLBound.Text = Request("F_CreatedLBound")
            F_CreatedUBound.Text = Request("F_CreatedUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "PIQAccountID"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " *, (Select CompanyName From PIQ Where PIQID = PIQAccount.PIQID) As PIQName "
        SQL = " FROM PIQAccount  "

        If Not F_PIQID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "PIQID = " & DB.Quote(F_PIQID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_LastName.Text = String.Empty Then
            SQL = SQL & Conn & "LastName LIKE " & DB.FilterQuote(F_LastName.Text)
            Conn = " AND "
        End If
        If Not F_Username.Text = String.Empty Then
            SQL = SQL & Conn & "Username LIKE " & DB.FilterQuote(F_Username.Text)
            Conn = " AND "
        End If
        If Not F_IsPrimary.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "IsPrimary  = " & DB.Number(F_IsPrimary.SelectedValue)
            Conn = " AND "
        End If
        If Not F_CreatedLBound.Text = String.Empty Then
            SQL = SQL & Conn & "Created >= " & DB.Quote(F_CreatedLBound.Text)
            Conn = " AND "
        End If
        If Not F_CreatedUBound.Text = String.Empty Then
            SQL = SQL & Conn & "Created < " & DB.Quote(DateAdd("d", 1, F_CreatedUBound.Text))
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

