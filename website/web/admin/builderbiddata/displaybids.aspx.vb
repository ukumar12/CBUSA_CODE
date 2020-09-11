Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage
    Protected BuilderID As Integer
    Private dvPhases As DataView



    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BUILDERS")

        BuilderID = Request("BuilderID")

        If BuilderID = Nothing Then Response.Redirect("/admin/default.aspx")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then

            F_ProjectID.DataSource = ProjectRow.GetBuilderProjects(DB, BuilderID, "ProjectName")
            F_ProjectID.DataValueField = "ProjectId"
            F_ProjectID.DataTextField = "ProjectName"
            F_ProjectID.DataBind()
            F_ProjectID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_VendorCategoryId.DataSource = VendorCategoryRow.GetPOList(DB)
            F_VendorCategoryId.DataTextField = "Category"
            F_VendorCategoryId.DataValueField = "VendorCategoryID"
            F_VendorCategoryId.DataBind()
            F_VendorCategoryId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_AwardedToVendorId.DataSource = DB.GetDataTable("Select VendorId, CompanyName From Vendor Where VendorId In (Select q.AwardedToVendorId From POQuote q Inner Join Project p On q.ProjectId = p.ProjectId Where p.BuilderId = " & BuilderID & ")")
            F_AwardedToVendorId.DataValueField = "VendorID"
            F_AwardedToVendorId.DataTextField = "CompanyName"
            F_AwardedToVendorId.Databind()
            F_AwardedToVendorId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_Title.Text = Request("F_Title")
            F_ProjectID.SelectedValue = Request("F_ProjectId")
            F_VendorCategoryId.SelectedValue = Request("F_VendorCategoryId")
            F_AwardedToVendorId.SelectedValue = Request("F_AwardedToVendorId")
            F_DeadlineLbound.Text = Request("F_DeadlineLBound")
            F_DeadlineUbound.Text = Request("F_DeadlineUBound")
            F_StatusDateLbound.Text = Request("F_StatusDateLBound")
            F_StatusDateUbound.Text = Request("F_StatusDateUBound")
            F_AwardedDateLbound.Text = Request("F_AwardedDateLBound")
            F_AwardedDateUbound.Text = Request("F_AwardedDateUBound")
            F_AwardedTotalLBound.Text = Request("F_AwardedTotalLBound")
            F_AwardedTotalUbound.Text = Request("F_AwardedTotalUBound")
            F_CreateDateLbound.Text = Request("F_CreateDateLbound")
            F_CreateDateUbound.Text = Request("F_CreateDateUbound")
            F_BuilderDocument.Text = Request("F_BuilderDocument")

            If Request("F_Status") <> String.Empty Then
                F_Status.SelectedValue = Request("F_Status")
            Else
                F_Status.SelectedValue = "All"
            End If

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            gvList.PageIndex = IIf(Request("F_pg") = String.Empty, 0, Core.ProtectParam(Request("F_pg")))

            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "CreateDate"
                gvList.SortOrder = "DESC"
            End If
            lnkReturn.NavigateUrl = "/admin/builderbiddata/default.aspx"
            BindList()
        End If

    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL, PhaseSQLFields, PhaseSQL As String
        Dim Conn As String = " And "
        Dim PhaseConn As String = " And "

        If Request("BuilderID") <> String.Empty Then
            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, BuilderID)
            ltlCompanyName.Text = "<h3> Builder Name:  " & dbBuilder.CompanyName & "</h3>"
        End If

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder
        ViewState("F_pg") = gvList.PageIndex.ToString

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQLFields &= ", (Select ProjectName From Project Where ProjectId = POQuote.ProjectId) As Project"
        SQLFields &= ", (Select Count(QuoteRequestId) From POQuoteRequest Where (Select Count(*) From vPOQuoteRequestMessages Where QuoteRequestId = POQuoteRequest.QuoteRequestId And QuoteId = POQuote.QuoteId And FromVendorId Is Not Null And IsRead = 0 And RequestStatus In ('New', 'Request Information')) > 0) As ActiveQuoteRequests "
        SQLFields &= ", (Select Count(QuoteRequestId) From POQuoteRequest Where QuoteId = POQuote.QuoteId And RequestStatus In ('New', 'Request Information', 'Awarded')) As TotalQuoteRequests"
        SQLFields &= ", (Select Count(QuoteRequestId) From POQuoteRequest Where QuoteId = POQuote.QuoteId And RequestStatus In ('New', 'Request Information', 'Awarded') And QuoteTotal > 0) As BidsReceived"
        SQLFields &= ", (Select Count(QuoteRequestId) From POQuoteRequest Where QuoteId = POQuote.QuoteId) As Total"
        SQL = " FROM POQuote Where ProjectId In (Select ProjectId From Project Where BuilderId = " & BuilderID & ") "

        If Not F_ProjectID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "ProjectId = " & DB.Quote(F_ProjectID.SelectedValue)
            Conn = " AND "
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

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
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


    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub

   

End Class
