Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BUILDER_REGISTRATIONS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_SubmittedLBound.Text = Request("F_SubmittedLBound")
            F_SubmittedUBound.Text = Request("F_SubmittedUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "BuilderRegistrationPaymentID"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM BuilderRegistrationPayment  "

        If Not F_SubmittedLBound.Text = String.Empty Then
            SQL = SQL & Conn & "Submitted >= " & DB.Quote(F_SubmittedLBound.Text)
            Conn = " AND "
        End If
        If Not F_SubmittedUBound.Text = String.Empty Then
            SQL = SQL & Conn & "Submitted < " & DB.Quote(DateAdd("d", 1, F_SubmittedUBound.Text))
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

