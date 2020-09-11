Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient

Partial Class POVendorQuoteRequests
    Inherits SitePage
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckVendorAccess(Request.Url.PathAndQuery)

        If Not CType(Me.Page, SitePage).IsLoggedInVendor Then
            Response.Redirect("/default.aspx")
        End If

        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, Session("VendorId"))

        If Not dbVendor.IsPlansOnline Then
            Response.Redirect("noaccess.aspx")
        End If

        gvList.BindList = AddressOf BindList

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorId")
        UserName = Session("Username")

        If Not IsPostBack Then

         Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Vendor Top Menu Click", "", "", "", "", UserName)

            If Session("BidMsg") <> "" Then
                divMsg.Visible = True
                ltlMsg.Text = Session("BidMsg")
                Session("BidMsg") = ""
            End If

            F_VendorCategoryId.DataSource = VendorCategoryRow.GetPOList(DB)
            F_VendorCategoryId.DataTextField = "Category"
            F_VendorCategoryId.DataValueField = "VendorCategoryID"
            F_VendorCategoryId.DataBind()
            F_VendorCategoryId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_BuilderId.DataSource = BuilderRow.GetList(DB, "CompanyName")
            F_BuilderId.DataValueField = "BuilderID"
            F_BuilderId.DataTextField = "CompanyName"
            F_BuilderId.DataBind()
            F_BuilderId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_QuoteNumber.Text = Request("F_QuoteNumber")
            F_BuilderContactName.Text = Request("F_BuilderContactName")
            F_BuilderContactEmail.Text = Request("F_BuilderContactEmail")
            F_Quote.Text = Request("F_Quote")
            F_BuilderId.SelectedValue = Request("F_BuilderId")
            F_RequestStatus.SelectedValue = Request("F_RequestStatus")
            F_QuoteTotalLBound.Text = Request("F_QuoteTotalLBound")
            F_QuoteTotalUbound.Text = Request("F_QuoteTotalUBound")
            F_QuoteExpirationDateLbound.Text = Request("F_QuoteExpirationDateLBound")
            F_QuoteExpirationDateUbound.Text = Request("F_QuoteExpirationDateUBound")
            F_CreateDateLbound.Text = Request("F_CreateDateLBound")
            F_CreateDateUbound.Text = Request("F_CreateDateUBound")
            F_DeadlineLbound.Text = Request("F_DeadlineLBound")
            F_DeadlineUbound.Text = Request("F_DeadlineUBound")
            F_Project.Text = Request("F_Project")
            F_VendorCategoryId.SelectedValue = Request("F_VendorCategoryId")
            F_BuilderDocument.Text = Request("F_BuilderDocument")
            F_VendorDocument.Text = Request("F_VendorDocument")

            If Request("F_RequestStatus") <> String.Empty Then
                F_RequestStatus.SelectedValue = Request("F_RequestStatus")
            Else
                F_RequestStatus.SelectedValue = "Active"
            End If

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            gvList.PageIndex = IIf(Request("F_pg") = String.Empty, 0, Core.ProtectParam(Request("F_pg")))

            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "LastMessageDate"
                gvList.SortOrder = "DESC"
            End If

            BindList()
        End If

    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " And "

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
        SQL = " FROM vPOQuoteRequests Where VendorId = " & DB.Number(Session("VendorId"))

        If Not F_Project.Text = String.Empty Then
            SQL = SQL & Conn & "Project Like " & DB.FilterQuote(F_Project.Text)
            Conn = " AND "
        End If
        If Not F_VendorCategoryId.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "QuoteId In (Select QuoteId From POQuoteVendorCategory Where VendorCategoryId = " & DB.Quote(F_VendorCategoryId.SelectedValue) & ")"
            Conn = " AND "
        End If
        If F_RequestStatus.SelectedValue <> "All" Then
            If F_RequestStatus.SelectedValue = "Active" Then
                SQL = SQL & Conn & "RequestStatus In ('New', 'Request Information', 'Declined By Builder', 'Awarded')"
            Else
                SQL = SQL & Conn & "RequestStatus = " & DB.Quote(F_RequestStatus.SelectedValue)
            End If
            Conn = " AND "
        End If
        If Not F_BuilderDocument.Text = String.Empty Then
            SQL = SQL & Conn & "QuoteId In (Select QuoteId From POBuilderDocument d Inner Join POQuoteBuilderDocument qd On d.BuilderDocumentId = qd.BuilderDocumentId Where qd.QuoteId = vPOQuoteRequests.QuoteId And d.Title Like " & DB.FilterQuote(F_BuilderDocument.Text) & ")"
            Conn = " AND "
        End If
        If Not F_VendorDocument.Text = String.Empty Then
            SQL = SQL & Conn & "QuoteId In (Select QuoteId From POVendorDocument d Inner Join POQuoteVendorDocument qd On d.VendorDocumentId = qd.VendorDocumentId Where qd.QuoteId = vPOQuoteRequests.QuoteId And d.VendorId = " & DB.Number(Session("VendorId")) & " And d.Title Like " & DB.FilterQuote(F_VendorDocument.Text) & ")"
            Conn = " AND "
        End If
        If Not F_Quote.Text = String.Empty Then
            SQL = SQL & Conn & "Quote Like " & DB.FilterQuote(F_Quote.Text)
            Conn = " AND "
        End If
        If Not F_BuilderId.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "VendorId = " & DB.Quote(F_BuilderId.SelectedValue)
            Conn = " AND "
        End If
        If Not F_QuoteNumber.Text = String.Empty Then
            SQL = SQL & Conn & "QuoteId = " & DB.Number(F_QuoteNumber.Text)
            Conn = " AND "
        End If
        If Not F_BuilderContactName.Text = String.Empty Then
            SQL = SQL & Conn & "BuilderContactName LIKE " & DB.FilterQuote(F_BuilderContactName.Text)
            Conn = " AND "
        End If
        If Not F_BuilderContactEmail.Text = String.Empty Then
            SQL = SQL & Conn & "BuilderContactEmail LIKE " & DB.FilterQuote(F_BuilderContactEmail.Text)
            Conn = " AND "
        End If
        If Not F_QuoteExpirationDateLbound.Text = String.Empty Then
            SQL = SQL & Conn & "QuoteExpirationDate >= " & DB.Quote(F_QuoteExpirationDateLbound.Text)
            Conn = " AND "
        End If
        If Not F_QuoteExpirationDateUbound.Text = String.Empty Then
            SQL = SQL & Conn & "QuoteExpirationDate < " & DB.Quote(DateAdd("d", 1, F_QuoteExpirationDateUbound.Text))
            Conn = " AND "
        End If
        If Not F_CreateDateLbound.Text = String.Empty Then
            SQL = SQL & Conn & "CreateDate >= " & DB.Quote(F_CreateDateLbound.Text)
            Conn = " AND "
        End If
        If Not F_CreateDateUbound.Text = String.Empty Then
            SQL = SQL & Conn & "CreateDate < " & DB.Quote(DateAdd("d", 1, F_CreateDateUbound.Text))
            Conn = " AND "
        End If
        If Not F_DeadlineLbound.Text = String.Empty Then
            SQL = SQL & Conn & "Deadline >= " & DB.Quote(F_DeadlineLbound.Text)
            Conn = " AND "
        End If
        If Not F_DeadlineUbound.Text = String.Empty Then
            SQL = SQL & Conn & "Deadline < " & DB.Quote(DateAdd("d", 1, F_DeadlineUbound.Text))
            Conn = " AND "
        End If
        If Not F_QuoteTotalLBound.Text = String.Empty Then
            SQL = SQL & Conn & "QuoteTotal >= " & DB.Number(F_QuoteTotalLBound.Text)
            Conn = " AND "
        End If
        If Not F_QuoteTotalUbound.Text = String.Empty Then
            SQL = SQL & Conn & "QuoteTotal <= " & DB.Number(F_QuoteTotalUbound.Text)
            Conn = " AND "
        End If

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Protected Sub gvList_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvList.RowCommand
        Select Case e.CommandName
            Case Is = "Remove"
                Try
                    DB.BeginTransaction()
                    POQuoteRow.RemoveRow(DB, e.CommandArgument)
                    DB.CommitTransaction()
                    BindList()
                Catch ex As SqlException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    AddError(ErrHandler.ErrorText(ex))
                End Try
        End Select
    End Sub

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
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

        Dim ltlMessage As Literal = e.Row.FindControl("ltlMessage")

        If Not IsDBNull(e.Row.DataItem("LastMessage")) Then
            ltlMessage.Text = "<b>Date: </b>" & e.Row.DataItem("LastMessageDate") & Core.Text2HTML(e.Row.DataItem("LastMessage"))
        Else
            ltlMessage.Text = "No Messages (New Quote Request)"
        End If

    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub

   ' Protected Sub btnDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnDashBoard.Click
       ' Response.Redirect("/builder/default.aspx")
    'End Sub

End Class

