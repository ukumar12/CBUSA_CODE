Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient

Partial Class POQuotes
    Inherits SitePage
    Private dvPhases As DataView

    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private CurrentQuotetId As String = ""

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        EnsureBuilderAccess()

        If Not CType(Me.Page, SitePage).IsLoggedInBuilder Then
            Response.Redirect("/default.aspx")
        End If

        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))

        If Not dbBuilder.IsPlansOnline Then
            Response.Redirect("noaccess.aspx")
        End If

        gvList.BindList = AddressOf BindList
        If Session("BidMsg") <> Nothing Then
            divMsg.Visible = True
            ltlMsg.Text = Session("BidMsg")
            Session("BidMsg") = Nothing
        End If
        If Not IsPostBack Then
            F_ProjectId.DataSource = ProjectRow.GetBuilderProjects(DB, Session("BuilderId"), "ProjectName")
            F_ProjectId.DataValueField = "ProjectId"
            F_ProjectId.DataTextField = "ProjectName"
            F_ProjectId.Databind()
            F_ProjectId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_VendorCategoryId.DataSource = VendorCategoryRow.GetPOList(DB)
            F_VendorCategoryId.DataTextField = "Category"
            F_VendorCategoryId.DataValueField = "VendorCategoryID"
            F_VendorCategoryId.DataBind()
            F_VendorCategoryId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_AwardedToVendorId.DataSource = DB.GetDataTable("Select VendorId, CompanyName From Vendor Where VendorId In (Select q.AwardedToVendorId From POQuote q Inner Join Project p On q.ProjectId = p.ProjectId Where p.BuilderId = " & DB.NullNumber(Session("BuilderId")) & ")")
            F_AwardedToVendorId.DataValueField = "VendorID"
            F_AwardedToVendorId.DataTextField = "CompanyName"
            F_AwardedToVendorId.Databind()
            F_AwardedToVendorId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_Title.Text = Request("F_Title")
            F_ProjectId.SelectedValue = Request("F_ProjectId")
            F_VendorCategoryId.SelectedValue = Request("F_VendorCategoryId")
            F_AwardedToVendorId.SelectedValue = Request("F_AwardedToVendorId")
            F_DeadlineLBound.Text = Request("F_DeadlineLBound")
            F_DeadlineUBound.Text = Request("F_DeadlineUBound")
            F_StatusDateLBound.Text = Request("F_StatusDateLBound")
            F_StatusDateUBound.Text = Request("F_StatusDateUBound")
            F_AwardedDateLBound.Text = Request("F_AwardedDateLBound")
            F_AwardedDateUBound.Text = Request("F_AwardedDateUBound")
            F_AwardedTotalLBound.Text = Request("F_AwardedTotalLBound")
            F_AwardedTotalUBound.Text = Request("F_AwardedTotalUBound")
            F_CreateDateLBound.Text = Request("F_CreateDateLBound")
            F_CreateDateUbound.Text = Request("F_CreateDateUBound")
            F_BuilderDocument.Text = Request("F_BuilderDocument")

            If Request("F_Status") <> String.Empty Then
                F_Status.SelectedValue = Request("F_Status")
            Else
                F_Status.SelectedValue = "Active"
            End If

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            gvList.PageIndex = IIf(Request("F_pg") = String.Empty, 0, Core.ProtectParam(Request("F_pg")))

            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "CreateDate"
                gvList.SortOrder = "DESC"
            End If

            BindList()
        End If
        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL, PhaseSQLFields, PhaseSQL As String
        Dim Conn As String = " And "
        Dim PhaseConn As String = " And "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder
        ViewState("F_pg") = gvList.PageIndex.ToString

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQLFields &= ", (Select ProjectName From Project Where ProjectId = POQuote.ProjectId) As Project"
        SQLFields &= ", (Select Count(QuoteRequestId) From POQuoteRequest Where (Select Count(*) From vPOQuoteRequestMessages Where QuoteRequestId = POQuoteRequest.QuoteRequestId And QuoteId = POQuote.QuoteId And FromVendorId Is Not Null And IsRead = 0 And RequestStatus In ('New', 'Request Information')) > 0) As ActiveQuoteRequests "
        SQLFields &= ", (Select Count(QuoteRequestId) From POQuoteRequest Where QuoteId = POQuote.QuoteId And RequestStatus In ('New', 'Request Information', 'Awarded')) As TotalQuoteRequests"
        SQLFields &= ", (Select Count(QuoteRequestId) From POQuoteRequest Where QuoteId = POQuote.QuoteId And RequestStatus In ('New', 'Request Information', 'Awarded') And QuoteTotal > 0) As BidsReceived"
        SQLFields &= ", (Select Count(QuoteRequestId) From POQuoteRequest Where QuoteId = POQuote.QuoteId) As Total"
        SQL = " FROM POQuote Where ProjectId In (Select ProjectId From Project Where BuilderId = " & DB.Number(Session("BuilderId")) & ") "

        If Not F_ProjectId.SelectedValue = String.Empty Then
            Dim dbProject As ProjectRow = ProjectRow.GetRow(DB, F_ProjectId.SelectedValue)
            ltlProjectHeader.Text = "<h3><b>Project:</b> " & dbProject.ProjectName & " ("
            ltlProjectHeader.Text &= dbProject.Subdivision
            If Not IsDBNull(dbProject.City) Then
                ltlProjectHeader.Text &= ", " & dbProject.City
            End If
            If Not IsDBNull(dbProject.State) Then
                ltlProjectHeader.Text &= ", " & dbProject.State
            End If
            ltlProjectHeader.Text &= ")</h3>"
            'gvList.Columns.RemoveAt(2)
            gvList.Columns.Item(2).Visible = False
            SQL = SQL & Conn & "ProjectId = " & DB.Quote(F_ProjectId.SelectedValue)
            Conn = " AND "
        Else
            ltlProjectHeader.Text = Nothing
            gvList.Columns.Item(2).Visible = True
        End If
        If Not F_VendorCategoryId.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "QuoteId In (Select QuoteId From POQuoteVendorCategory Where VendorCategoryId = " & DB.Quote(F_VendorCategoryId.SelectedValue) & ")"
            Conn = " AND "
        End If
        If F_Status.SelectedValue <> "All" Then
            If F_Status.SelectedValue = "Active" Then
                SQL = SQL & Conn & "Status In ('New', 'Bidding In Progress', 'Awarded')"
            Else
                SQL = SQL & Conn & "Status = " & DB.Quote(F_Status.SelectedValue)
            End If
            Conn = " AND "
        End If
        If Not F_BuilderDocument.Text = String.Empty Then
            SQL = SQL & Conn & "QuoteId In (Select QuoteId From POBuilderDocument d Inner Join POQuoteBuilderDocument qd On d.BuilderDocumentId = qd.BuilderDocumentId Where qd.QuoteId = POQuote.QuoteId And d.Title Like " & DB.FilterQuote(F_BuilderDocument.Text) & ")"
            Conn = " AND "
        End If
        If Not F_AwardedToVendorId.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "AwardedToVendorId = " & DB.Quote(F_AwardedToVendorId.SelectedValue)
            Conn = " AND "
        End If
        If Not F_Title.Text = String.Empty Then
            SQL = SQL & Conn & "Title LIKE " & DB.FilterQuote(F_Title.Text)
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
        If Not F_StatusDateLbound.Text = String.Empty Then
            SQL = SQL & Conn & "StatusDate >= " & DB.Quote(F_StatusDateLbound.Text)
            Conn = " AND "
        End If
        If Not F_StatusDateUbound.Text = String.Empty Then
            SQL = SQL & Conn & "StatusDate < " & DB.Quote(DateAdd("d", 1, F_StatusDateUbound.Text))
            Conn = " AND "
        End If
        If Not F_AwardedDateLbound.Text = String.Empty Then
            SQL = SQL & Conn & "AwardedDate >= " & DB.Quote(F_AwardedDateLbound.Text)
            Conn = " AND "
        End If
        If Not F_AwardedDateUbound.Text = String.Empty Then
            SQL = SQL & Conn & "AwardedDate < " & DB.Quote(DateAdd("d", 1, F_AwardedDateUbound.Text))
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
        If Not F_AwardedTotalLBound.Text = String.Empty Then
            SQL = SQL & Conn & "AwardedTotal >= " & DB.Number(F_AwardedTotalLBound.Text)
            Conn = " AND "
        End If
        If Not F_AwardedTotalUbound.Text = String.Empty Then
            SQL = SQL & Conn & "AwardedTotal <= " & DB.Number(F_AwardedTotalUbound.Text)
            Conn = " AND "
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)
        PhaseSQLFields = "Select qvc.QuoteId,vc.Category  "
        PhaseSQL = " from POQuoteVendorCategory qvc inner join VendorCategory vc on vc.VendorCategoryId = qvc.VendorCategoryId "
        If Not F_VendorCategoryId.SelectedValue = String.Empty Then
            PhaseSQL = PhaseSQL & PhaseConn & "qvc.QuoteId In (Select QuoteId From POQuoteVendorCategory Where VendorCategoryId = " & DB.Quote(F_VendorCategoryId.SelectedValue) & ")"
            PhaseConn = " AND "
        End If

        dvPhases = DB.GetDataTable(PhaseSQLFields & PhaseSQL).DefaultView

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
                    'log Delete BidRequest
                    CurrentQuotetId = Convert.ToString(e.CommandArgument)
                    Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Delete BidRequest", CurrentQuotetId, "", "", "", UserName)
                    'end log
                    DB.CommitTransaction()
                    BindList()
                Catch ex As SqlException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    AddError(ErrHandler.ErrorText(ex))
                End Try
            Case Is = "Copy"
                Try
                    DB.BeginTransaction()
                    Dim dbQuote As POQuoteRow = POQuoteRow.GetRow(DB, e.CommandArgument)
                    Dim dbNewQuote As POQuoteRow = New POQuoteRow(DB)
                    dbNewQuote.ProjectId = dbQuote.ProjectId
                    dbNewQuote.Title = "Copy of " & dbQuote.Title
                    dbNewQuote.Instructions = dbQuote.Instructions
                    dbNewQuote.Deadline = dbQuote.Deadline
                    dbNewQuote.Status = "New"
                    dbNewQuote.StatusDate = Now()
                    Dim QuoteId As Integer = dbNewQuote.Insert()

                    'add vendor categories
                    Dim Sql As String = "Insert Into POQuoteVendorCategory (QuoteId, VendorCategoryId) Select " & DB.Number(QuoteId) & ", VendorCategoryId From POQuoteVendorCategory Where QuoteId = " & DB.Number(dbQuote.QuoteId)
                    DB.ExecuteSQL(Sql)

                    'add vendor categories
                    Sql = "Insert Into POQuoteBuilderDocument (QuoteId, BuilderDocumentId) Select " & DB.Number(QuoteId) & ", BuilderDocumentId From POQuoteBuilderDocument Where QuoteId = " & DB.Number(dbQuote.QuoteId)
                    DB.ExecuteSQL(Sql)

                    'log copy BidRequest
                    Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Copy BidRequest", QuoteId, "", "", "", UserName)
                    'end log

                    DB.CommitTransaction()
                    Response.Redirect("editquote.aspx?QuoteId=" & QuoteId & "&" & GetPageParams(FilterFieldType.All))
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
        Dim lnkDelete As ConfirmImageButton = e.Row.FindControl("lnkDelete")

        lnkDelete.CommandArgument = e.Row.DataItem("QuoteId")
        
        Dim lnkCopy As ConfirmLinkButton = e.Row.FindControl("lnkCopy")
        lnkCopy.CommandArgument = e.Row.DataItem("QuoteId")

        If e.Row.DataItem("Total") > 0 Then
            lnkDelete.Visible = False
        End If

        Dim ltlPhases As Literal = e.Row.FindControl("ltlPhases")
        dvPhases.RowFilter = "QuoteId=" & e.Row.DataItem("QuoteId")
        If dvPhases.Count > 0 Then
            Dim Conn As String = String.Empty
            Dim Result As String = String.Empty
            For Each item As DataRowView In dvPhases
                Result &= Conn & item("Category")
                Conn = ","
            Next
            ltlPhases.Text = Result
        End If

        Dim ltlStatus As Literal = e.Row.FindControl("ltlStatus")
        If e.Row.DataItem("Status") = "New" Then
            ltlStatus.Text = "Pending - Bid Request Not Sent"
        Else
            ltlStatus.Text = e.Row.DataItem("Status")
        End If

    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        'log  Add BidRequest
        Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Btn Add BidRequest", "", "", "", "", UserName)
        'end log
        Response.Redirect("editquote.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub



End Class

