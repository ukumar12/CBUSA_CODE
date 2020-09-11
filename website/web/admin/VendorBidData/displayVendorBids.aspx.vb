Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage
    Private VendorID As Integer




    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("VENDORS")

        VendorID = Request("VendorID")
        If VendorID = Nothing Then Response.Redirect("/admin/default.aspx")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then

            'F_ProjectID.DataSource = ProjectRow.GetBuilderProjects(DB, VendorID, "ProjectName")
            F_ProjectID.DataSource = DB.GetDataTable("Select distinct( Project) from vPOQuoteRequests where VendorID = " & VendorID)
            F_ProjectID.DataValueField = "Project"
            F_ProjectID.DataTextField = "Project"
            F_ProjectID.DataBind()
            F_ProjectID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_BuilderId.DataSource = DB.GetDataTable("Select distinct( Builder) from vPOQuoteRequests where VendorID = " & VendorID)
            F_BuilderId.DataValueField = "Builder"
            F_BuilderId.DataTextField = "Builder"
            F_BuilderId.DataBind()
            F_BuilderId.Items.Insert(0, New ListItem("-- ALL --", ""))

            'F_Title.Text = Request("F_Title")
            F_ProjectID.SelectedValue = Request("F_ProjectId")
            F_BuilderId.SelectedValue = Request("F_BuilderID")
            F_Quote.Text = Request("F_Quote")
            F_RequestStatus.SelectedValue = Request("F_RequestStatus")
            F_QuoteTotalLBound.Text = Request("F_QuoteTotalLBound")
            F_QuoteTotalUBound.Text = Request("F_QuoteTotalUBound")
            F_QuoteExpirationDateLbound.Text = Request("F_QuoteExpirationDateLBound")
            F_QuoteExpirationDateUbound.Text = Request("F_QuoteExpirationDateUBound")
            F_CreateDateLbound.Text = Request("F_CreateDateLBound")
            F_CreateDateUbound.Text = Request("F_CreateDateUBound")
            F_DeadlineLbound.Text = Request("F_DeadlineLBound")
            F_DeadlineUbound.Text = Request("F_DeadlineUBound")
            F_BuilderDocument.Text = Request("F_BuilderDocument")
            F_VendorDocument.Text = Request("F_VendorDocument")
           

            'If Request("F_Status") <> String.Empty Then
            '    F_Status.SelectedValue = Request("F_Status")
            'Else
            '    F_Status.SelectedValue = "All"
            'End If



            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            gvList.PageIndex = IIf(Request("F_pg") = String.Empty, 0, Core.ProtectParam(Request("F_pg")))

            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "Builder"
                gvList.SortOrder = "ASC"
            End If
            lnkReturn.NavigateUrl = "/admin/Vendorbiddata/default.aspx"
            BindList()
        End If

    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " And "
        Dim PhaseConn As String = " And "

        If Request("VendorID") <> String.Empty Then
            Dim dbvendor As VendorRow = VendorRow.GetRow(DB, VendorID)
            ltlCompanyName.Text = "<h3> Vendor Name:  " & dbvendor.CompanyName & "</h3>"
        End If

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder
        ViewState("F_pg") = gvList.PageIndex.ToString

        'SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        'SQLFields &= ", (Select ProjectName From Project Where ProjectId = POQuote.ProjectId) As Project"
        'SQLFields &= ", (Select Count(QuoteRequestId) From POQuoteRequest Where (Select Count(*) From vPOQuoteRequestMessages Where QuoteRequestId = POQuoteRequest.QuoteRequestId And QuoteId = POQuote.QuoteId And FromVendorId Is Not Null And IsRead = 0 And RequestStatus In ('New', 'Request Information')) > 0) As ActiveQuoteRequests "
        'SQLFields &= ", (Select Count(QuoteRequestId) From POQuoteRequest Where QuoteId = POQuote.QuoteId And RequestStatus In ('New', 'Request Information', 'Awarded')) As TotalQuoteRequests"
        'SQLFields &= ", (Select Count(QuoteRequestId) From POQuoteRequest Where QuoteId = POQuote.QuoteId And RequestStatus In ('New', 'Request Information', 'Awarded') And QuoteTotal > 0) As BidsReceived"
        'SQLFields &= ", (Select Count(QuoteRequestId) From POQuoteRequest Where QuoteId = POQuote.QuoteId) As Total"
        'SQL = " FROM POQuote Where ProjectId In (Select ProjectId From Project Where BuilderId = " & BuilderID & ") "

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM vPOQuoteRequests where VendorId = " & VendorID

        If Not F_ProjectID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "Project = " & DB.Quote(F_ProjectID.SelectedValue)
            Conn = " AND "
        End If

        If Not F_BuilderId.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "Builder = " & DB.Quote(F_BuilderId.SelectedValue)
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
        If Not F_Quote.Text = String.Empty Then
            SQL = SQL & Conn & "Quote Like " & DB.FilterQuote(F_Quote.Text)
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
        If Not F_QuoteTotalUBound.Text = String.Empty Then
            SQL = SQL & Conn & "QuoteTotal <= " & DB.Number(F_QuoteTotalUBound.Text)
            Conn = " AND "
        End If
       
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & "ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

   

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub



End Class
