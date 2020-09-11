Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient

Partial Class Index
    Inherits AdminPage

    Private PageTotal As Integer
    Private PageHTMLTotal As Integer
    Private PageTextTotal As Integer
    Private Total As Integer
    Private HTMLTotal As Integer
    Private TextTotal As Integer

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BROADCAST")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_TemplateId.Datasource = MailingTemplateRow.GetAllTemplates(DB)
            F_TemplateId.DataValueField = "TemplateId"
            F_TemplateId.DataTextField = "Name"
            F_TemplateId.Databind()
            F_TemplateId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_TargetType.Text = Request("F_TargetType")
            F_TemplateId.SelectedValue = Request("F_TemplateId")
            F_SentDateLBound.Text = Request("F_SentDateLBound")
            F_SentDateUBound.Text = Request("F_SentDateUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "SentDate"
                gvList.SortOrder = "ASC"
            End If

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " and "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " MessageId, Name, Subject, SentDate, HTMLCount, TextCount, TotalCount "

        SQL = " from ("
        SQL &= " SELECT mm.MessageId, mm.Name, mm.Subject, mm.SentDate,"
        SQL &= " sum(CASE WHEN mr.MimeType = 'HTML' THEN 1 ELSE 0 END) AS HTMLCount, "
        SQL &= " sum(CASE WHEN mr.MimeType = 'TEXT' THEN 1 ELSE 0 END) AS TextCount, "
        SQL &= " count(mr.RecipientId) AS TotalCount FROM MailingMessage mm, MailingRecipient mr WHERE mm.SentDate IS NOT NULL AND mm.ParentId IS NULL AND mr.MessageId = mm.MessageId"

        If Not F_TemplateId.Text = String.Empty Then
            SQL = SQL & Conn & "mm.TemplateId = " & DB.Number(F_TemplateId.SelectedValue)
            Conn = " AND "
        End If
        If Not F_TargetType.Text = String.Empty Then
            SQL = SQL & Conn & "mm.TargetType LIKE " & DB.FilterQuote(F_TargetType.Text)
            Conn = " AND "
        End If
        If Not F_SentDateLbound.Text = String.Empty Then
            SQL = SQL & Conn & "mm.SentDate >= " & DB.Quote(F_SentDateLbound.Text)
            Conn = " AND "
        End If
        If Not F_SentDateUbound.Text = String.Empty Then
            SQL = SQL & Conn & "mm.SentDate < " & DB.Quote(DateAdd("d", 1, F_SentDateUbound.Text))
            Conn = " AND "
        End If
        SQL &= " GROUP BY mm.MessageId, mm.Name, mm.Subject, mm.SentDate ) as tmp"

        'Get total counts
        Dim SQLCount As String = "select sum(HTMLCount) as HTMLCount, sum(TextCount) as TextCount, sum(TotalCount) as TotalCount " & SQL
        Dim dr As SqlDataReader = DB.GetReader(SQLCount)
        If dr.Read Then
            Total = IIf(IsDBNull(dr("TotalCount")), 0, dr("TotalCount"))
            HTMLTotal = IIf(IsDBNull(dr("HTMLCount")), 0, dr("HTMLCount"))
            TextTotal = IIf(IsDBNull(dr("TextCount")), 0, dr("TextCount"))
        End If
        dr.Close()

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()

        lblTotal.Text = Total
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            PageTotal += e.Row.DataItem("TotalCount")
            PageHTMLTotal += e.Row.DataItem("HTMLCount")
            PageTextTotal += e.Row.DataItem("TextCount")
        End If
        If e.Row.RowType = DataControlRowType.Footer Then
            e.Row.Cells(3).Text = "Page Total:<br/>Total:"
            e.Row.Cells(4).Text = PageHTMLTotal & "<br />" & HTMLTotal
            e.Row.Cells(5).Text = PageTextTotal & "<br />" & TextTotal
            e.Row.Cells(6).Text = PageTotal & "<br />" & Total
        End If
    End Sub
End Class

