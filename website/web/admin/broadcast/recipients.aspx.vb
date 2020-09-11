Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient

Partial Class Recipients
    Inherits AdminPage

    Protected MessageId As Integer

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BROADCAST")

        MessageId = Request("MessageId")
        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        Dim dbMailingMessage As MailingMessageRow = MailingMessageRow.GetRow(DB, MessageId)
        ltlName.Text = dbMailingMessage.Name

        F_MimeType.Text = Request("F_MimeType")

        gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
        gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
        If gvList.SortBy = String.Empty Then gvList.SortBy = "FullName"

        BindList()
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "

        SQL = String.Empty
        SQL &= " from ("
        SQL &= " select mm.name, mlm.email, full_name as FullName, num_bounces as NumBounces, 'HTML' as MimeType"
        SQL &= " from "
        SQL &= " MailingMessage mm, mailing_lyris_member mlm"
        SQL &= " where "
        SQL &= " 	mm.ListHTMLId = mlm.list_id"
        SQL &= " and mm.MessageId = " & DB.Number(MessageId)
        SQL &= " union"
        SQL &= " select mm.name, mlm.email, full_name as FullName, num_bounces as NumBounces, 'TEXT' as MimeType"
        SQL &= " from "
        SQL &= " MailingMessage mm, mailing_lyris_member mlm"
        SQL &= " where "
        SQL &= " 	mm.ListTextId = mlm.list_id"
        SQL &= " and mm.MessageId = " & DB.Number(MessageId)
        SQL &= " ) as tmp"

        If Not F_MimeType.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "MimeType  = " & DB.Quote(F_MimeType.SelectedValue)
            Conn = " AND "
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not Page.IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub

End Class