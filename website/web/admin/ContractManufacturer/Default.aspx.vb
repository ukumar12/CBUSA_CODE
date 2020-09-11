Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BUILDERS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then

            F_MailingState.DataSource = StateRow.GetStateList(DB)
            F_MailingState.DataTextField = "StateName"
            F_MailingState.DataValueField = "StateCode"
            F_MailingState.DataBind()
            F_MailingState.Items.Insert(0, New ListItem("", ""))

            F_CompanyName.Text = Request("F_CompanyName")
            F_MailingCity.Text = Request("F_MailingCity")
            F_MailingState.SelectedValue = Request("F_MailingState")
            F_MailingZip.Text = Request("F_MailingZip")
            F_APContactName.Text = Request("F_APContactName")
            F_APContactEmail.Text = Request("F_APContactEmail")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            gvList.PageIndex = IIf(Request("F_pg") = String.Empty, 0, Core.ProtectParam(Request("F_pg")))

            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "NCPManufacturerID"
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

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM NCPManufacturer  "

        If Not F_CompanyName.Text = String.Empty Then
            SQL = SQL & Conn & "CompanyName LIKE " & DB.FilterQuote(F_CompanyName.Text)
            Conn = " AND "
        End If
        If Not F_MailingCity.Text = String.Empty Then
            SQL = SQL & Conn & "MailingCity LIKE " & DB.FilterQuote(F_MailingCity.Text)
            Conn = " AND "
        End If
        If Not F_MailingState.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "MailingState LIKE " & DB.FilterQuote(F_MailingState.SelectedValue)
            Conn = " AND "
        End If
        If Not F_MailingZip.Text = String.Empty Then
            SQL = SQL & Conn & "MailingZip LIKE " & DB.FilterQuote(F_MailingZip.Text)
            Conn = " AND "
        End If
        If Not F_APContactName.Text = String.Empty Then
            SQL = SQL & Conn & "APContactName LIKE " & DB.FilterQuote(F_APContactName.Text)
            Conn = " AND "
        End If
        If Not F_APContactEmail.Text = String.Empty Then
            SQL = SQL & Conn & "APContactEmail LIKE " & DB.FilterQuote(F_APContactEmail.Text)
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
