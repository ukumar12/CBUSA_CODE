Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("MESSAGES")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_Subject.Text = Request("F_Subject")
            F_Title.Text = Request("F_Title")
            F_SendEmailCopy.Text = Request("F_SendEmailCopy")
            F_IsActive.Text = Request("F_IsActive")
            F_IsAlert.Text = Request("F_IsAlert")
            F_StartDateLBound.Text = Request("F_StartDateLBound")
            F_StartDateUBound.Text = Request("F_StartDateUBound")
            F_EndDateLBound.Text = Request("F_EndDateLBound")
            F_EndDateUBound.Text = Request("F_EndDateUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "AdminMessageID"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM AdminMessage  "

        If Not F_Subject.Text = String.Empty Then
            SQL = SQL & Conn & "Subject LIKE " & DB.FilterQuote(F_Subject.Text)
            Conn = " AND "
        End If
        If Not F_Title.Text = String.Empty Then
            SQL = SQL & Conn & "Title LIKE " & DB.FilterQuote(F_Title.Text)
            Conn = " AND "
        End If
        If Not F_SendEmailCopy.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "SendEmailCopy  = " & DB.Number(F_SendEmailCopy.SelectedValue)
            Conn = " AND "
        End If
        If Not F_IsActive.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "IsActive  = " & DB.Number(F_IsActive.SelectedValue)
            Conn = " AND "
        End If
        If Not F_IsAlert.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "IsAlert  = " & DB.Number(F_IsAlert.SelectedValue)
            Conn = " AND "
        End If
        If Not F_StartDateLBound.Text = String.Empty Then
            SQL = SQL & Conn & "StartDate >= " & DB.Quote(F_StartDateLBound.Text)
            Conn = " AND "
        End If
        If Not F_StartDateUBound.Text = String.Empty Then
            SQL = SQL & Conn & "StartDate < " & DB.Quote(DateAdd("d", 1, F_StartDateUBound.Text))
            Conn = " AND "
        End If
        If Not F_EndDateLBound.Text = String.Empty Then
            SQL = SQL & Conn & "EndDate >= " & DB.Quote(F_EndDateLBound.Text)
            Conn = " AND "
        End If
        If Not F_EndDateUBound.Text = String.Empty Then
            SQL = SQL & Conn & "EndDate < " & DB.Quote(DateAdd("d", 1, F_EndDateUBound.Text))
            Conn = " AND "
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("add.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub
End Class
