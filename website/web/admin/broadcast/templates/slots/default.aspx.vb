Imports Components
Imports Controls
Imports DataLayer
Imports System.Data
Imports System.Data.SqlClient

Partial Class Index
    Inherits AdminPage

    Protected dbTemplate As MailingTemplateRow

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BROADCAST")

        dbTemplate = MailingTemplateRow.GetRow(Me.DB, Convert.ToInt32(Request("F_TemplateId")))

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            gvList.SortBy = "SortOrder"
            BindList()
        End If
    End Sub


    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " mts.*, mt.name "
        SQL = " FROM MailingTemplateSlot as mts join mailingtemplate as mt on mt.templateid = mts.templateid  where mts.templateid = " & dbTemplate.TemplateId

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

End Class
