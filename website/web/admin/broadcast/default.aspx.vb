Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BROADCAST")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_TemplateId.Datasource = MailingTemplateRow.GetAllTemplates(DB)
            F_TemplateId.DataValueField = "TemplateId"
            F_TemplateId.DataTextField = "Name"
            F_TemplateId.Databind()
            F_TemplateId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_Status.SelectedValue = Request("F_Status")
            F_TargetType.Text = Request("F_TargetType")
            F_TemplateId.SelectedValue = Request("F_TemplateId")
            F_SentDateLBound.Text = Request("F_SentDateLBound")
            F_SentDateUBound.Text = Request("F_SentDateUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "mm.ModifyDate"
                gvList.SortOrder = "DESC"
            End If

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " and "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " mt.Name as TemplateName, mm.* "
        SQL = " FROM MailingMessage mm, MailingTemplate mt where mm.ParentId is null and mm.TemplateId = mt.TemplateId and mm.Status <> 'DELETED'"

        If Not F_TemplateId.Text = String.Empty Then
            SQL = SQL & Conn & "mm.TemplateId = " & DB.Number(F_TemplateId.SelectedValue)
            Conn = " AND "
        End If
        If Not F_Status.Selectedvalue = String.Empty Then
            SQL = SQL & Conn & "mm.Status = " & DB.Quote(F_Status.Text)
            Conn = " AND "
        End If
        If Not F_TargetType.Text = String.Empty Then
            SQL = SQL & Conn & "mm.TargetType LIKE " & DB.FilterQuote(F_TargetType.Text)
            Conn = " AND "
        End If
        If Not F_SentDateLBound.Text = String.Empty Then
            SQL = SQL & Conn & "mm.SentDate >= " & DB.Quote(F_SentDateLbound.Text)
            Conn = " AND "
        End If
        If Not F_SentDateUBound.Text = String.Empty Then
            SQL = SQL & Conn & "mm.SentDate < " & DB.Quote(DateAdd("d", 1, F_SentDateUbound.Text))
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

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not e.Row.RowType = DataControlRowType.DataRow Then Exit Sub

        Dim lblMimeType As Label = e.Row.FindControl("lblMimeType")
        Dim MimeType As String = IIf(IsDBNull(e.Row.DataItem("MimeType")), String.Empty, e.Row.DataItem("MimeType"))
        Select Case MimeType
            Case "BOTH"
                lblMimeType.Text = "HTML and Text"
            Case "HTML"
                lblMimeType.Text = "HTML"
            Case "TEXT"
                lblMimeType.Text = "Plain Text"
        End Select

        Dim lblTargetType As Label = e.Row.FindControl("lblTargetType")
        Dim TargetType As String = IIf(IsDBNull(e.Row.DataItem("TargetType")), String.Empty, e.Row.DataItem("TargetType").ToString)
        Select Case TargetType
            Case "DYNAMIC"
                lblTargetType.Text = "Uploaded List"
            Case "MEMBER"
                lblTargetType.Text = "Subscribers"
        End Select

        Dim lblDateTime As Label = e.Row.FindControl("lblDateTime")
        If e.Row.DataItem("Status").ToString = "SCHEDULED" Then
            lblDateTime.Text = Format(e.Row.DataItem("ScheduledDate"), "MM/dd/yyyy <br />hh:mm:ss tt")
        ElseIf e.Row.DataItem("Status").ToString = "SENT" Then
            lblDateTime.Text = Format(e.Row.DataItem("SentDate"), "MM/dd/yyyy <br />hh:mm:ss tt")
        End If

        Dim lnkEditView As HyperLink = e.Row.FindControl("lnkEditView")
        Dim Status As String = e.Row.DataItem("Status")
        Select Case Status
            Case "SCHEDULED", "SENT"
                lnkEditView.ImageUrl = "/images/admin/preview.gif"
                lnkEditView.NavigateUrl = "view.aspx?MessageId=" & e.Row.DataItem("MessageId") & "&" & GetPageParams(Components.FilterFieldType.All)
        End Select

    End Sub
End Class

