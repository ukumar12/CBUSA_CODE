Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("VENDORS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_QuoteId.Datasource = POQuoteRow.GetList(DB, "Title")
            F_QuoteId.DataValueField = "QuoteId"
            F_QuoteId.DataTextField = "Title"
            F_QuoteId.Databind()
            F_QuoteId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_VendorId.Datasource = VendorRow.GetList(DB, "CompanyName")
            F_VendorId.DataValueField = "VendorID"
            F_VendorId.DataTextField = "CompanyName"
            F_VendorId.Databind()
            F_VendorId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_RequestStatus.Text = Request("F_RequestStatus")
            F_VendorContactName.Text = Request("F_VendorContactName")
            F_VendorContactEmail.Text = Request("F_VendorContactEmail")
            F_QuoteId.SelectedValue = Request("F_QuoteId")
            F_VendorId.SelectedValue = Request("F_VendorId")
            F_CreateDateLBound.Text = Request("F_CreateDateLBound")
            F_CreateDateUBound.Text = Request("F_CreateDateUBound")
            F_ModifyDateLBound.Text = Request("F_ModifyDateLBound")
            F_ModifyDateUBound.Text = Request("F_ModifyDateUBound")
            F_StartDateLBound.Text = Request("F_StartDateLBound")
            F_StartDateUBound.Text = Request("F_StartDateUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            gvList.PageIndex = IIf(Request("F_pg") = String.Empty, 0, Core.ProtectParam(Request("F_pg")))

            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "QuoteRequestId"
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
        SQLFields &= ", (Select Count(MessageId) From POQuoteRequestMessage Where QuoteRequestId = vPOQuoteRequests.QuoteRequestId And FromBuilderId Is Not Null And IsRead = 0) As UnreadMessages "
        SQLFields &= ", (Select Count(MessageId) From POQuoteRequestMessage Where QuoteRequestId = vPOQuoteRequests.QuoteRequestId And FromBuilderId Is Not Null) As TotalMessages"
        SQLFields &= ", (Select Count(p.MessageId) From PORequestInfoMessage p Inner Join PORequestInfoThread t On p.ThreadId = t.ThreadId Where t.QuoteId = vPOQuoteRequests.QuoteId And p.BuilderId = vPOQuoteRequests.BuilderId And DateDiff(dd, p.CreateDate, GetDate()) = 0) As NewPosts"
        SQLFields &= ", (Select Count(p.MessageId) From PORequestInfoMessage p Inner Join PORequestInfoThread t On p.ThreadId = t.ThreadId Where t.QuoteId = vPOQuoteRequests.QuoteId) As TotalPosts"
        SQLFields &= ", (Case When (Select Top 1 CreateDate From POQuoteRequestMessage Where QuoteRequestId = vPOQuoteRequests.QuoteRequestId Order By CreateDate Desc) Is Not NULL Then (Select Top 1 CreateDate From POQuoteRequestMessage Where QuoteRequestId = vPOQuoteRequests.QuoteRequestId Order By CreateDate Desc) Else CreateDate End) As LastMessageDate"
        SQLFields &= ", (Select Top 1 (Case When IsRead = 0 Then ' <b><i>New!</i></b>' Else '' End) + '<br><b>From ' + (Case When FromBuilderId Is Not Null Then 'Builder' Else 'Vendor' End) + ':</b> ' + FromName + '<br><br>' + Message From POQuoteRequestMessage Where QuoteRequestId = vPOQuoteRequests.QuoteRequestId Order By CreateDate Desc) As LastMessage"
        SQL = " FROM vPOQuoteRequests  "

        If Not F_QuoteId.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "QuoteId = " & DB.Number(F_QuoteId.SelectedValue)
            Conn = " AND "
        End If
        If Not F_VendorId.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "VendorId = " & DB.Number(F_VendorId.SelectedValue)
            Conn = " AND "
        End If
        If Not F_RequestStatus.Text = String.Empty Then
            SQL = SQL & Conn & "RequestStatus LIKE " & DB.FilterQuote(F_RequestStatus.Text)
            Conn = " AND "
        End If
        If Not F_VendorContactName.Text = String.Empty Then
            SQL = SQL & Conn & "VendorContactName LIKE " & DB.FilterQuote(F_VendorContactName.Text)
            Conn = " AND "
        End If
        If Not F_VendorContactEmail.Text = String.Empty Then
            SQL = SQL & Conn & "VendorContactEmail LIKE " & DB.FilterQuote(F_VendorContactEmail.Text)
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
        If Not F_ModifyDateLBound.Text = String.Empty Then
            SQL = SQL & Conn & "ModifyDate >= " & DB.Quote(F_ModifyDateLBound.Text)
            Conn = " AND "
        End If
        If Not F_ModifyDateUBound.Text = String.Empty Then
            SQL = SQL & Conn & "ModifyDate < " & DB.Quote(DateAdd("d", 1, F_ModifyDateUBound.Text))
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
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()

        Me.trSort.Visible = res.Rows.Count > 0
        Me.ltlSortBy.Text = F_Sort.SelectedItem.Text & " " & IIf(gvList.SortOrder = "ASC", "Ascending", "Descending")


    End Sub

    Protected Sub gvList_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvList.RowCommand
        Select Case e.CommandName
            Case Is = "Remove"
                Try
                    DB.BeginTransaction()
                    POQuoteRequestRow.RemoveRow(DB, e.CommandArgument)
                    DB.CommitTransaction()
                    BindList()
                Catch ex As SQLException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    AddError(ErrHandler.ErrorText(ex))
                End Try
        End Select
    End Sub

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If
        Dim lnkDelete As ConfirmImageButton = e.Row.FindControl("lnkDelete")
        lnkDelete.CommandArgument = e.Row.DataItem("QuoteRequestId")

        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If

        Dim ltlProject As Literal = e.Row.FindControl("ltlProject")
        ltlProject.Text = "<b>" & e.Row.DataItem("Project") & "</b>"
        'ltlProject.Text &= "<br>" & e.Row.DataItem("Subdivision")
        'If Not IsDBNull(e.Row.DataItem("City")) Then
        '    ltlProject.Text &= ", " & e.Row.DataItem("City")
        'End If
        'If Not IsDBNull(e.Row.DataItem("State")) Then
        '    ltlProject.Text &= ", " & e.Row.DataItem("State")
        'End If

        Dim ltlQuote As Literal = e.Row.FindControl("ltlQuote")
        ltlQuote.Text = "<b>" & e.Row.DataItem("Quote") & "</b>"
        ltlQuote.Text &= "<br/><i>(Created: " & CDate(e.Row.DataItem("CreateDate")).ToString("MM/dd/yyyy") & ")</i>"
        'ltlQuote.Text &= "<br>#" & e.Row.DataItem("QuoteId")
        'ltlQuote.Text &= "<br>" & e.Row.DataItem("Status")
        'ltlQuote.Text &= " (" & e.Row.DataItem("StatusDate") & ")"


        Dim ltlContact As Literal = e.Row.FindControl("ltlContact")
        ltlContact.Text = "<b>" & e.Row.DataItem("Builder") & "</b>"
        If Not IsDBNull(e.Row.DataItem("BuilderContactName")) Then
            ltlContact.Text &= "<br>" & e.Row.DataItem("BuilderContactName")
        End If
        If Not IsDBNull(e.Row.DataItem("BuilderContactPhone")) Then
            ltlContact.Text &= "<br>" & e.Row.DataItem("BuilderContactPhone")
        End If
        If Not IsDBNull(e.Row.DataItem("BuilderContactEmail")) Then
            ltlContact.Text &= "<br>" & e.Row.DataItem("BuilderContactEmail")
        End If


    End Sub

    Protected Sub drpSort_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles F_Sort.SelectedIndexChanged
        gvList.SortBy = sender.SelectedValue
        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder
        BindList()
    End Sub

    Protected Sub lnkSortOrder_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkSortOrder.Click
        If InStr(sender.Text, "Descend") > 0 Then
            gvList.SortOrder = "DESC"
            Me.lnkSortOrder.Text = "<img src=""/images/admin/sort_asc.gif"" border=""0"" align=""absmiddle"" /> Ascend"
        Else
            gvList.SortOrder = "ASC"
            Me.lnkSortOrder.Text = "<img src=""/images/admin/sort_desc.gif"" border=""0""  align=""absmiddle"" /> Descend"
        End If
        gvList.SortBy = F_Sort.SelectedValue
        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder
        BindList()
    End Sub
    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub
End Class