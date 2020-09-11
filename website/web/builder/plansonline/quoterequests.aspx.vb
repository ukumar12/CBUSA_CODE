Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient

Partial Class POQuoteRequests
    Inherits SitePage
    Private dbBuilder As BuilderRow
    Private QuoteId As Integer = 0

    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckBuilderAccess(Request.Url.PathAndQuery)

        If Not CType(Me.Page, SitePage).IsLoggedInBuilder Then
            Response.Redirect("/default.aspx")
        End If

        dbBuilder = BuilderRow.GetRow(DB, Session("BuilderId"))

        If Not dbBuilder.IsPlansOnline Then
            Response.Redirect("noaccess.aspx")
        End If
        If Request("F_QuoteId") <> Nothing Then
            QuoteId = Convert.ToInt32(Request("F_QuoteId"))
        End If

        If QuoteId = 0 OrElse Request("F_RequestStatus") = "Awarded" OrElse Request("F_RequestStatus") = "Cancelled" Then
            pnlMessages.Visible = False
            lnkBidReminders.Visible = False


        Else
            pnlMessages.Visible = True
            lnkBidReminders.Visible = True

        End If
        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then

            F_ProjectId.DataSource = ProjectRow.GetBuilderProjects(DB, Session("BuilderId"), "ProjectName")
            F_ProjectId.DataValueField = "ProjectId"
            F_ProjectId.DataTextField = "ProjectName"
            F_ProjectId.DataBind()
            F_ProjectId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_VendorCategoryId.DataSource = VendorCategoryRow.GetPOList(DB)
            F_VendorCategoryId.DataTextField = "Category"
            F_VendorCategoryId.DataValueField = "VendorCategoryID"
            F_VendorCategoryId.DataBind()
            F_VendorCategoryId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_QuoteId.DataSource = POQuoteRow.GetListByBuilder(DB, Session("BuilderId"), "Title")
            F_QuoteId.DataValueField = "QuoteId"
            F_QuoteId.DataTextField = "Title"
            F_QuoteId.DataBind()
            F_QuoteId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_VendorId.DataSource = DB.GetDataTable("Select VendorId, CompanyName From Vendor Where VendorId In (Select r.VendorId From POQuoteRequest r Inner Join POQuote q On r.QuoteId = q.QuoteId Inner Join Project p On q.ProjectId = p.ProjectId Where p.BuilderId = " & DB.NullNumber(Session("BuilderId")) & ")")
            F_VendorId.DataValueField = "VendorID"
            F_VendorId.DataTextField = "CompanyName"
            F_VendorId.DataBind()
            F_VendorId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_QuoteNumber.Text = Request("F_QuoteNumber")
            F_VendorContactName.Text = Request("F_VendorContactName")
            F_VendorContactEmail.Text = Request("F_VendorContactEmail")
            F_QuoteId.SelectedValue = Request("F_QuoteId")
            If Request("F_QuoteId") <> Nothing AndAlso Request("F_ProjectId") = Nothing Then
                F_ProjectId.SelectedValue = DB.ExecuteScalar("SELECT ProjectId FROM vPOQuoteRequests where QuoteId = " & DB.Number(Request("F_QuoteId")))
            Else
                F_ProjectId.SelectedValue = Request("F_ProjectId")
            End If
            If F_ProjectId.SelectedValue <> Nothing Then
                lnkBidRequest.HRef = "quotes.aspx?F_ProjectId=" & Server.UrlEncode(F_ProjectId.SelectedValue)
            End If
            F_VendorId.SelectedValue = Request("F_VendorId")
            F_RequestStatus.SelectedValue = Request("F_RequestStatus")
            F_QuoteTotalLBound.Text = Request("F_QuoteTotalLBound")
            F_QuoteTotalUbound.Text = Request("F_QuoteTotalUBound")
            F_QuoteExpirationDateLbound.Text = Request("F_QuoteExpirationDateLBound")
            F_QuoteExpirationDateUbound.Text = Request("F_QuoteExpirationDateUBound")
            F_CreateDateLbound.Text = Request("F_CreateDateLBound")
            F_CreateDateUbound.Text = Request("F_CreateDateUBound")
            F_DeadlineLbound.Text = Request("F_DeadlineLBound")
            F_DeadlineUbound.Text = Request("F_DeadlineUBound")

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
        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " And "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder
        ViewState("F_pg") = gvList.PageIndex.ToString

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQLFields &= ", (Select Count(MessageId) From POQuoteRequestMessage Where QuoteRequestId = vPOQuoteRequests.QuoteRequestId And FromVendorId Is Not Null And IsRead = 0) As UnreadMessages "
        SQLFields &= ", (Select Count(MessageId) From POQuoteRequestMessage Where QuoteRequestId = vPOQuoteRequests.QuoteRequestId And FromVendorId Is Not Null) As TotalMessages"
        SQLFields &= ", (Select Count(p.MessageId) From PORequestInfoMessage p Inner Join PORequestInfoThread t On p.ThreadId = t.ThreadId Where t.QuoteId = vPOQuoteRequests.QuoteId And p.VendorId = vPOQuoteRequests.VendorId And DateDiff(dd, p.CreateDate, GetDate()) = 0) As NewPosts"
        SQLFields &= ", (Select Count(p.MessageId) From PORequestInfoMessage p Inner Join PORequestInfoThread t On p.ThreadId = t.ThreadId Where t.QuoteId = vPOQuoteRequests.QuoteId) As TotalPosts"
        SQLFields &= ", (Case When (Select Top 1 CreateDate From POQuoteRequestMessage Where QuoteRequestId = vPOQuoteRequests.QuoteRequestId Order By CreateDate Desc) Is Not NULL Then (Select Top 1 CreateDate From POQuoteRequestMessage Where QuoteRequestId = vPOQuoteRequests.QuoteRequestId Order By CreateDate Desc) Else CreateDate End) As LastMessageDate"
        SQLFields &= ", (Select Top 1 (Case When IsRead = 0 Then ' <b><i>New!</i></b>' Else '' End) + '<br><b>From ' + (Case When FromBuilderId Is Not Null Then 'Builder' Else 'Vendor' End) + ':</b> ' + FromName + '<br><br>' + Message From POQuoteRequestMessage Where QuoteRequestId = vPOQuoteRequests.QuoteRequestId Order By CreateDate Desc) As LastMessage"
        SQLFields &= ", vi.PaymentTerms"
        SQL = " FROM vPOQuoteRequests INNER JOIN Vendor vi on vi.VendorId = vPOQuoteRequests.VendorId Where BuilderId = " & DB.Number(Session("BuilderId"))
        
        If F_QuoteId.SelectedValue <> Nothing Then
            Dim dbQuote As POQuoteRow = POQuoteRow.GetRow(DB, F_QuoteId.SelectedValue)
            F_ProjectId.SelectedValue = dbQuote.ProjectId
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
            ltlProjectHeader.Text &= "<h3><b>Bid:</b> " & dbQuote.Title & " (" & dbQuote.Deadline & ")</h3>"
            ltlProjectHeader.Text &= "<h3>Status: " & dbQuote.Status & "</h3>"

            If dbQuote.Status = "Awarded" OrElse dbQuote.Status = "Cancelled" Then
                pnlMessages.Visible = False
                lnkBidReminders.Visible = False
            End If

            gvList.Columns.Item(0).Visible = False
            gvList.Columns.Item(1).Visible = False
            SQL = SQL & Conn & "QuoteId = " & DB.Quote(F_QuoteId.SelectedValue)
            Conn = " AND "
        ElseIf Not F_ProjectId.SelectedValue = String.Empty Then
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

            gvList.Columns.Item(0).Visible = False
            SQL = SQL & Conn & "ProjectId = " & DB.Quote(F_ProjectId.SelectedValue)
            Conn = " AND "
        End If
        If Not F_VendorCategoryId.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "QuoteId In (Select QuoteId From POQuoteVendorCategory Where VendorCategoryId = " & DB.Quote(F_VendorCategoryId.SelectedValue) & ")"
            Conn = " AND "
        End If
        If F_RequestStatus.SelectedValue <> "All" Then
            If F_RequestStatus.SelectedValue = "Active" Then
                SQL = SQL & Conn & "RequestStatus In ('New', 'Request Information', 'Declined By Vendor', 'Awarded')"
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
            SQL = SQL & Conn & "QuoteId In (Select QuoteId From POVendorDocument d Inner Join POQuoteVendorDocument qd On d.VendorDocumentId = qd.VendorDocumentId Where qd.QuoteId = vPOQuoteRequests.QuoteId And d.VendorId = vPOQuoteRequests.VendorId And d.Title Like " & DB.FilterQuote(F_VendorDocument.Text) & ")"
            Conn = " AND "
        End If
        If Not F_QuoteId.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "QuoteId = " & DB.Quote(F_QuoteId.SelectedValue)
            Conn = " AND "
        End If
        If Not F_VendorId.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "VendorId = " & DB.Quote(F_VendorId.SelectedValue)
            Conn = " AND "
        End If
        If Not F_QuoteNumber.Text = String.Empty Then
            SQL = SQL & Conn & "QuoteId = " & DB.Number(F_QuoteNumber.Text)
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
    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub

#Region "gvList"
    Protected Sub gvList_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvList.RowCommand
        Select Case e.CommandName
            Case Is = "Remove"
                Try
                    DB.BeginTransaction()
                    POQuoteRow.RemoveRow(DB, e.CommandArgument)
                    DB.CommitTransaction()
                    'log Delete 
                    'Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Delete", QuoteId, "", "", "", UserName)
                    'end log
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

        Dim lnkAward As HyperLink = e.Row.FindControl("lnkAward")
        lnkAward.Visible = e.Row.DataItem("Status") <> "Awarded"

        Dim ltlProject As Literal = e.Row.FindControl("ltlProject")
        If ltlProject IsNot Nothing Then
            ltlProject.Text = "<b><a href=""edit.aspx?ProjectId=" & e.Row.DataItem("ProjectId") & """>" & e.Row.DataItem("Project") & "</a></b>"
            ltlProject.Text &= "<br>" & e.Row.DataItem("Subdivision")
            If Not IsDBNull(e.Row.DataItem("City")) Then
                ltlProject.Text &= ", " & e.Row.DataItem("City")
            End If
            If Not IsDBNull(e.Row.DataItem("State")) Then
                ltlProject.Text &= ", " & e.Row.DataItem("State")
            End If
        End If

        Dim ltlQuote As Literal = e.Row.FindControl("ltlQuote")
        ltlQuote.Text = "<b><a href=""editquote.aspx?QuoteId=" & e.Row.DataItem("QuoteId") & """>" & e.Row.DataItem("Quote") & "</b>"
        ltlQuote.Text &= " (By: " & e.Row.DataItem("Deadline") & ")</a>"
        ltlQuote.Text &= "<br>#" & e.Row.DataItem("QuoteId")
        ltlQuote.Text &= "<br>" & e.Row.DataItem("Status")
        ltlQuote.Text &= " (" & e.Row.DataItem("StatusDate") & ")"


        Dim ltlContact As Literal = e.Row.FindControl("ltlContact")
        ltlContact.Text = "<b>" & e.Row.DataItem("Vendor") & "</b>"
        If Not IsDBNull(e.Row.DataItem("VendorContactName")) Then
            ltlContact.Text &= "<br>" & e.Row.DataItem("VendorContactName")
        End If
        If Not IsDBNull(e.Row.DataItem("VendorContactPhone")) Then
            ltlContact.Text &= "<br>" & e.Row.DataItem("VendorContactPhone")
        End If
        If Not IsDBNull(e.Row.DataItem("VendorContactEmail")) Then
            ltlContact.Text &= "<br>" & e.Row.DataItem("VendorContactEmail")
        End If
        ltlContact.Text &= "<br>" '<a style=""text-decoration:none; border:0;"" target=""_blank"" href=""/directory/vendor.aspx?VendorId=" & e.Row.DataItem("VendorId") & """><div>"
        'Rating
        Dim OverallAvg As Double = Core.GetDouble(DB.ExecuteScalar("select Avg(cast(coalesce(Rating,0) as float)) from VendorRatingCategoryRating where VendorID=" & DB.Number(e.Row.DataItem("VendorId")) & " and Coalesce(Rating,0) > 0"))
        Dim i As Integer
        For i = 1 To Math.Ceiling(OverallAvg)
            ltlContact.Text &= "<img style=""cursor:pointer;"" onclick=""window.open('/directory/vendor.aspx?VendorId=" & e.Row.DataItem("VendorId") & "');"" alt=""" & OverallAvg & """ src=""/images/rating/star-red-sm.gif"" />"
        Next

        For i = i To 10
            ltlContact.Text &= "<img style=""cursor:pointer;"" onclick=""window.open('/directory/vendor.aspx?VendorId=" & e.Row.DataItem("VendorId") & "');"" alt=""" & OverallAvg & """ src=""/images/rating/star-gr-sm.gif"" />"
        Next
        ltlContact.Text &= "<br/><b>Rating:</b> " & Math.Round(OverallAvg, 1)
        'ltlContact.Text &= "</div></a>"

        Dim ltlRebate As Literal = e.Row.FindControl("ltlRebate")

        Dim dtRebateTerms As DataTable
        dtRebateTerms = RebateTermRow.GetCurrentTerms(DB, e.Row.DataItem("VendorId"))
        Dim sRebates As String = String.Empty
        Dim Conn As String = String.Empty
        If Not dtRebateTerms.Rows.Count = 0 Then
            For Each row As DataRow In dtRebateTerms.Rows
                sRebates &= Conn & row("RebatePercentage") & "%"
                Conn = vbCrLf
            Next
        End If

        ltlRebate.Text = sRebates

        Dim ltlMessage As Literal = e.Row.FindControl("ltlMessage")

        ltlMessage.Text = "<b>Request Status: </b>" & e.Row.DataItem("RequestStatus") & "</br>"

        If Not IsDBNull(e.Row.DataItem("LastMessage")) Then
            ltlMessage.Text &= "<b>Date: </b>" & e.Row.DataItem("LastMessageDate") & Core.Text2HTML(e.Row.DataItem("LastMessage"))
        Else
            ltlMessage.Text &= "No Messages (New Quote Request)"
        End If

    End Sub
#End Region
    

#Region "Messaging"
    Protected Sub btnAddMessage_Click(sender As Object, e As System.EventArgs) Handles btnAddMessage.Click
        If txtMessage.Text = Nothing Then
            AddError("Field 'Message' is blank.")
            Exit Sub
        End If
        Dim sql As String
        sql = " select qr.QuoteRequestId, q.QuoteId, qr.VendorId, b.companyName as BuilderName, l.LLC ,l.LLCID , "
        sql &= " qr.QuoteTotal, qr.QuoteExpirationDate, qr.CreateDate, qr.RequestStatus, qr.ModifyDate, "
        sql &= " v.CompanyName, v.Email as VendorEmail, "
        sql &= " q.Title, q.Deadline, q.StatusDate, q.Status,  "
        sql &= " p.ProjectName, p.Subdivision, p.LotNumber, p.City, p.State "
        sql &= " from POQuoteRequest qr  "
        sql &= " inner join Vendor v on v.VendorID = qr.VendorId  "
        sql &= " inner join POQuote q on qr.QuoteId = q.QuoteId  "
        sql &= " inner join Project p on q.ProjectId = p.ProjectID "
        sql &= " inner join Builder b on p.BuilderID =b.BuilderID "
        sql &= " inner join LLC l on l.LLCID = b.LLCID "
        sql &= " where(qr.QuoteTotal = 0 Or qr.QuoteTotal Is null) "
        sql &= " and qr.QuoteId = " & DB.Number(QuoteId)
        sql &= " and qr.RequestStatus not in ('Declined By Vendor','Declined By Builder','Awarded','Cancelled')"

        Dim dt As DataTable = DB.GetDataTable(sql)
        If dt.Rows.Count > 0 Then
            Dim dbMessage As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "BidRequestReminder")
            For Each row As DataRow In dt.Rows
                Dim sBody As String = FormatMessage(DB, row, txtMessage.Text)
                ' SendEmail(DB, row, sBody)
                dbMessage.Send(VendorRow.GetRow(DB, row("vendorid")), sBody, True)
            Next
        End If

        divAddMessage.Visible = False
        txtMessage.Text = SysParam.GetValue(DB, "BidReminderDefaultEmailMessage")
        ltlReminderMsg.Text = "Messages successfully sent"
        'log  send bid reminders
        Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Send Bid Reminders", "", "", "", "", UserName)
        'end log
    End Sub
    Private Function FormatMessage(ByVal DB As Database, ByVal drVendor As DataRow, ByVal Msg As String) As String
        'Msg = Msg.Replace("%%Vendor%%", drVendor("CompanyName"))
        Dim tempMsg As String
        Dim dbMessage As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "BidRequestReminder")
        tempMsg = dbMessage.Message
        tempMsg = tempMsg.Replace("%%Vendor%%", drVendor("CompanyName"))
        tempMsg = tempMsg.Replace("%%Builder%%", dbBuilder.CompanyName)
        tempMsg = tempMsg.Replace("%%BuilderCustomMessage%%", Msg)
        tempMsg = tempMsg.Replace("%%QuoteDetails%%", GetQuoteDetailsBody(drVendor))
        tempMsg = tempMsg.Replace("%%QuoteRequestDetails%%", GetRequestDetailsBody(drVendor))
        tempMsg = tempMsg.Replace("%%QuoteRequestUrl%%", IIf(ConfigurationManager.AppSettings("GlobalSecureName") <> Nothing, ConfigurationManager.AppSettings("GlobalSecureName"), ConfigurationManager.AppSettings("GlobalRefererName")) & "/vendor/plansonline/quoterequestmessages.aspx?QuoteRequestId=" & drVendor("QuoteRequestId"))

        Return tempMsg
    End Function
    Private Shared Function GetQuoteDetailsBody(ByVal drQuoteInfo As DataRow) As String
        Dim sBody As String = vbCrLf & vbCrLf
        sBody &= "<strong>Builder Name:</strong> " & Core.GetString(drQuoteInfo("BuilderName")) & vbCrLf
        sBody &= "<strong>Market :</strong> " & Core.GetString(drQuoteInfo("LLC")) & vbCrLf
		sBody &= "<p>&nbsp;</p>" & vbCrLf
        'sBody &= "Bid Request #: " & Core.GetString(drQuoteInfo("QuoteId")) & vbCrLf
        sBody &= "<strong>Title:</strong> " & Core.GetString(drQuoteInfo("Title")) & vbCrLf
        sBody &= "<strong>Deadline:</strong> " & Core.GetString(drQuoteInfo("Deadline")) & vbCrLf
        sBody &= "<strong>Status:</strong> " & Core.GetString(drQuoteInfo("Status")) & " (" & Core.GetString(drQuoteInfo("StatusDate")) & ")" & vbCrLf
        sBody &= "<strong>Project:</strong> " & Core.GetString(drQuoteInfo("ProjectName")) & vbCrLf
        sBody &= "<strong>Subdivision:</strong> " & Core.GetString(drQuoteInfo("Subdivision")) & vbCrLf
        sBody &= "<strong>Lot #:</strong> " & Core.GetString(drQuoteInfo("LotNumber")) & vbCrLf
        sBody &= "<strong>City:</strong> " & Core.GetString(drQuoteInfo("City")) & vbCrLf
        sBody &= "<strong>State:</strong> " & Core.GetString(drQuoteInfo("State")) & vbCrLf

        Return sBody

    End Function
    Private Function GetRequestDetailsBody(ByVal drQuoteInfo As DataRow) As String
        Dim QuoteTotal As Double = Core.GetDouble(drQuoteInfo("QuoteTotal"))
        Dim QuoteExpirationDate As DateTime = Core.GetDate(drQuoteInfo("QuoteExpirationDate"))
        'Dim sBody As String = "Request Status: " & drQuoteInfo("RequestStatus") & " (" & drQuoteInfo("ModifyDate") & ")" & vbCrLf
        'sBody &= "Bid Total: " & IIf(QuoteTotal > 0, FormatCurrency(QuoteTotal, 2), "") & vbCrLf
        'sBody &= "Expiration Date: " & IIf(QuoteTotal <> Nothing, QuoteTotal.ToString(), "") & vbCrLf
        'Add new content future
        Dim sBody As String = String.Empty

        Return sBody

    End Function
    Private Sub SendEmail(ByVal DB As Database, ByVal Account As DataRow, ByVal AdditionalMessage As String)
        Dim sBody As String

        sBody = AdditionalMessage

        'TestMode'
        Dim addr As String
        If SysParam.GetValue(DB, "TestMode") = True Then
            addr = SysParam.GetValue(DB, "AdminEmail")
        Else
            addr = Account("VendorEmail")
        End If

        If Core.IsEmail(addr) Then
            Core.SendSimpleMail(dbBuilder.Email, dbBuilder.CompanyName, addr, Account("CompanyName"), "Bid Request Reminder", sBody)
        End If
    End Sub

    Protected Sub lnkAddMessage_Click(sender As Object, e As System.EventArgs) Handles lnkAddMessage.Click
        txtMessage.Text = SysParam.GetValue(DB, "BidReminderDefaultEmailMessage")
        divAddMessage.Visible = True
        ltlReminderMsg.Text = Nothing
    End Sub

    Protected Sub btnCancelMessage_Click(sender As Object, e As System.EventArgs) Handles btnCancelMessage.Click
        divAddMessage.Visible = False
    End Sub
#End Region

End Class

