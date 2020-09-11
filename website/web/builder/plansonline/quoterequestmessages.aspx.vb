Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient

Partial Class POQuoteRequestMessages
    Inherits SitePage

    Protected QuoteRequestId As Integer
    Protected dbQuoteRequest As POQuoteRequestRow
    Protected dbQuote As POQuoteRow
    Protected dbProject As ProjectRow
    Protected dbBuilder As BuilderRow
    Protected dbBuilderAccount As BuilderAccountRow

    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private NewPostThreadId As String = ""
    Private NewTopicTreadId As String = ""
    Private RemoveThreadId As String = ""
    Private MessageId As String = ""
    Private ThreadId As String = ""
    Private GetThreadId As String = ""

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckBuilderAccess(Request.Url.PathAndQuery)

        If Not CType(Me.Page, SitePage).IsLoggedInBuilder Then
            Response.Redirect("/default.aspx")
        End If

        dbBuilder = BuilderRow.GetRow(DB, Session("BuilderId"))

        If Not dbBuilder.IsPlansOnline Then
            Response.Redirect("noaccess.aspx")
        End If

        QuoteRequestId = Convert.ToInt32(Request("QuoteRequestId"))

        If QuoteRequestId = 0 Then
            Response.Redirect("/default.aspx")
        End If

        dbQuoteRequest = POQuoteRequestRow.GetRow(DB, QuoteRequestId)
        dbQuote = POQuoteRow.GetRow(DB, dbQuoteRequest.QuoteId)
        dbProject = ProjectRow.GetRow(DB, dbQuote.ProjectId)

        dbBuilderAccount = BuilderAccountRow.GetRow(DB, Session("BuilderAccountId"))

        If dbProject.BuilderId <> Session("BuilderId") Then
            Response.Redirect("/default.aspx")
        End If
        If Request("F_ProjectId") <> Nothing Then
            hplBidRequestFooter.HRef = "quotes.aspx?F_ProjectId=" & Request("F_ProjectId")
            hplBidRequestHeader.HRef = "quotes.aspx?F_ProjectId=" & Request("F_ProjectId")
        End If
        If Request("F_ProjectId") <> Nothing AndAlso Request("F_QuoteId") <> Nothing Then
            hplBidStatusFooter.HRef = "quoterequests.aspx?F_ProjectId=" & Request("F_ProjectId") & "&F_QuoteId=" & Request("F_QuoteId")
            hplBidStatusHeader.HRef = "quoterequests.aspx?F_ProjectId=" & Request("F_ProjectId") & "&F_QuoteId=" & Request("F_QuoteId")
        End If

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            LoadDetails()
            BindThreads()
        End If

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")
    End Sub

    Protected Sub LoadDetails()

        dbQuoteRequest = POQuoteRequestRow.GetRow(DB, QuoteRequestId)
        dbQuote = POQuoteRow.GetRow(DB, dbQuoteRequest.QuoteId)

        btnAward.Visible = True
        btnDecline.Visible = True
        'btnRequestInfo.Visible = True

        If dbQuoteRequest.RequestStatus = "Exited Market" Or dbQuoteRequest.RequestStatus = "Declined By Vendor" Then
            btnAward.Visible = False
            btnDecline.Visible = False
            'btnRequestInfo.Visible = False
        End If
        If dbQuote.Status = "Awarded" Then
            btnAward.Visible = False
            btnAwardDiv.Visible = False
        End If
        If dbQuoteRequest.RequestStatus = "Declined By Builder" Then
            btnAward.Visible = False
            btnDecline.Visible = False
        End If
        If dbQuoteRequest.RequestStatus = "Request Information" Then
            'btnRequestInfo.Visible = False
        End If

        ltlProject.Text = "<a href=""edit.aspx?QuoteRequestId=" & QuoteRequestId & "&ProjectId=" & dbProject.ProjectID & """>" & dbProject.ProjectName & " (" & dbProject.Subdivision
        If dbProject.City <> "" Then
            ltlProject.Text &= ", " & dbProject.City
        End If
        If dbProject.State <> "" Then
            ltlProject.Text &= ", " & dbProject.State
        End If
        ltlProject.Text &= ")</a>"

        ltlQuoteId.Text = dbQuote.QuoteId
        ltlQuote.Text = "<a href=""editquote.aspx?QuoteRequestId=" & QuoteRequestId & "&QuoteId=" & dbQuote.QuoteId & """>" & dbQuote.Title & "</a>"
        ltlQuoteDeadline.Text = dbQuote.Deadline
        ltlStatus.Text = dbQuote.Status
        ltlStatusDate.Text = dbQuote.StatusDate
        ltlInstructions.Text = Core.Text2HTML(dbQuote.Instructions)
        ltlVendor.Text = VendorRow.GetRow(DB, dbQuoteRequest.VendorId).CompanyName
        ltlRequestStatus.Text = dbQuoteRequest.RequestStatus
        ltlRequestStatusDate.Text = dbQuoteRequest.ModifyDate
        ltlExpiration.Text = IIf(Not dbQuoteRequest.QuoteExpirationDate = Nothing, dbQuoteRequest.QuoteExpirationDate, "")
        ltlTotal.Text = IIf(dbQuoteRequest.QuoteTotal > 0, FormatCurrency(dbQuoteRequest.QuoteTotal, 2), "")

        ltlBuilderContact.Text = dbProject.ContactName
        If dbProject.ContactEmail <> "" Then
            ltlBuilderContact.Text &= "<br>" & dbProject.ContactEmail
        End If
        If dbProject.ContactPhone <> "" Then
            ltlBuilderContact.Text &= "<br>" & dbProject.ContactPhone
        End If

        ltlVendorContact.Text = dbQuoteRequest.VendorContactName
        If dbQuoteRequest.VendorContactEmail <> "" Then
            ltlVendorContact.Text &= "<br>" & dbQuoteRequest.VendorContactEmail
        End If
        If dbQuoteRequest.VendorContactPhone <> "" Then
            ltlVendorContact.Text &= "<br>" & dbQuoteRequest.VendorContactPhone
        End If

        ltlVendorCategory.Text = ""

        Dim dt As DataTable = DB.GetDataTable("Select Category From VendorCategory Where VendorCategoryID In " & DB.NumberMultiple(dbQuote.GetSelectedVendorCategories()))
        Dim sConn As String = ""
        For Each dr As DataRow In dt.Rows
            ltlVendorCategory.Text &= sConn & dr("Category")
            sConn = ", "
        Next

        If dbQuote.Status = "Awarded" Then
            phawarded.Visible = True
            ltlAwardedDate.Text = dbQuote.AwardedDate
            ltlAwardedTotal.Text = FormatCurrency(dbQuote.AwardedTotal, 2)
            ltlAwardedToVendor.Text = VendorRow.GetRow(DB, dbQuote.AwardedToVendorId).CompanyName
        End If

        ltlStartDate.Text = dbQuoteRequest.StartDate
        ltlCompletionTime.Text = dbQuoteRequest.CompletionTime
        ltlPaymentTerms.Text = dbQuoteRequest.PaymentTerms

        If gvList.SortBy = String.Empty Then
            gvList.SortBy = "CreateDate"
            gvList.SortOrder = "DESC"
        End If

        BindDocuments(dbQuote.QuoteId)
        BindVendorDocuments(dbQuote.QuoteId)
        BindList()
        upDetails.Update()
        upMessages.Update()

        If Request("award") = "y" Then
            divAwardBid.Visible = True
        End If

    End Sub

    Protected Sub BindDocuments(ByVal QuoteId As Integer)

        Dim SQL As String = String.Empty
        Dim dt As DataTable

        SQL = "Select * From POBuilderDocument Where BuilderDocumentId In (Select BuilderDocumentId From POQuoteBuilderDocument Where QuoteId = " & DB.Number(QuoteId) & ")"

        If SQL <> String.Empty Then
            dt = DB.GetDataTable(SQL)

            If dt.Rows.Count > 0 Then
                Me.rptDocuments.DataSource = dt
                Me.rptDocuments.DataBind()
                divNoCurrentDocuments.Visible = False
            End If
        End If
    End Sub

    Protected Sub rptDocuments_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptDocuments.ItemDataBound

        If Not e.Item.ItemType = ListItemType.Item And Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim lnkMessageTitle As HtmlAnchor = e.Item.FindControl("lnkMessageTitle")

        lnkMessageTitle.HRef = "/assets/plansonline/builderdocument/" & e.Item.DataItem("FileName").ToString

    End Sub

    Protected Sub BindVendorDocuments(ByVal QuoteId As Integer)

        Dim SQL As String = String.Empty
        Dim dt As DataTable

        SQL = "Select * From POVendorDocument Where VendorId = " & DB.Number(dbQuoteRequest.VendorId) & " And VendorDocumentId In (Select VendorDocumentId From POQuoteVendorDocument Where QuoteId = " & DB.Number(QuoteId) & ")"

        If SQL <> String.Empty Then
            dt = DB.GetDataTable(SQL)
            If dt.Rows.Count > 0 Then

                Me.rptVendorDocuments.DataSource = dt
                Me.rptVendorDocuments.DataBind()

                divNoVendorDocuments.Visible = False
            End If
        End If
    End Sub

    Protected Sub rptVendorDocuments_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptVendorDocuments.ItemDataBound

        If Not e.Item.ItemType = ListItemType.Item And Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim lnkMessageTitle As HtmlAnchor = e.Item.FindControl("lnkMessageTitle")

        lnkMessageTitle.HRef = "/assets/plansonline/vendordocument/" & e.Item.DataItem("FileName").ToString

    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " And "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder
        ViewState("F_pg") = gvList.PageIndex.ToString

        SQLFields = "SELECT *"
        SQLFields &= ", (Case When FromVendorId Is Null Then 'Builder' Else 'Vendor' End) As FromUser "
        SQL = " FROM POQuoteRequestMessage Where QuoteRequestId = " & DB.Number(QuoteRequestId)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Protected Sub gvList_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvList.RowCommand
        Select Case e.CommandName
            Case Is = "Read"
                Try
                    DB.BeginTransaction()
                    Dim dbMessage As POQuoteRequestMessageRow = POQuoteRequestMessageRow.GetRow(DB, e.CommandArgument)
                    dbMessage.IsRead = Not dbMessage.IsRead
                    dbMessage.Update()
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

        Dim btnRead As LinkButton = e.Row.FindControl("btnRead")

        btnRead.CommandArgument = e.Row.DataItem("MessageId")

        btnRead.Visible = IsDBNull(e.Row.DataItem("FromBuilderId"))

        If btnRead.Visible Then
            If e.Row.DataItem("IsRead") Then
                btnRead.Text = "Mark as Unread"
            Else
                e.Row.Style.Add("font-weight", "bold")
            End If
        End If

        Dim ltlMessage As Literal = e.Row.FindControl("ltlMessage")

        ltlMessage.Text = Core.Text2HTML(e.Row.DataItem("Message"))

    End Sub

    Protected Function SaveChanges(Optional ByVal Status As String = "") As Boolean

        Try
            DB.BeginTransaction()

            Dim BuilderAccount As DataLayer.BuilderAccountRow
            BuilderAccount = DataLayer.BuilderAccountRow.GetRow(Me.DB, Session("BuilderAccountId"))

            Dim sMessage As String = String.Empty
            If Status <> "" Then
                dbQuoteRequest.RequestStatus = Status
                If Status = "Awarded" Then
                    dbQuote.Status = "Awarded"
                    dbQuote.StatusDate = Now()
                    dbQuote.AwardedToVendorId = dbQuoteRequest.VendorId
                    dbQuote.AwardedTotal = dbQuoteRequest.QuoteTotal
                    dbQuote.AwardedDate = Now()
                    dbQuote.Update()
                    'log  Award Bid
                    Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Award Bid", QuoteRequestId, "", "", "", UserName)
                    'end log

                    If txtAwardBidMessage.Text <> Nothing Then
                        sMessage = txtAwardBidMessage.Text
                    Else
                        sMessage = "Awarded"
                    End If
                ElseIf Status = "Declined By Builder" Then
                    If txtDeclineMessage.Text <> Nothing Then
                        sMessage = txtDeclineMessage.Text
                    Else
                        sMessage = "Declined By Builder"
                    End If
                    'log  Decline Bid
                    Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Decline Bid", QuoteRequestId, "", "", "", UserName)
                    'end log
                End If
            Else
                sMessage = txtMessage.Text
            End If
            dbQuoteRequest.Update()
            If sMessage <> "" Then
                Dim dbMessage As POQuoteRequestMessageRow = New POQuoteRequestMessageRow(DB)
                dbMessage.QuoteRequestId = QuoteRequestId
                dbMessage.Message = sMessage
                dbMessage.FromBuilderId = Session("BuilderId")
                dbMessage.FromName = BuilderAccount.FirstName & " " & BuilderAccount.LastName
                dbMessage.MessageQuoteStatus = dbQuoteRequest.RequestStatus
                dbMessage.MessageQuoteTotal = dbQuoteRequest.QuoteTotal
                dbMessage.MessageQuoteExpirationDate = dbQuoteRequest.QuoteExpirationDate
                dbMessage.Insert()

                DB.CommitTransaction()

                LoadDetails()

                Return True
            Else
                Return False
            End If
        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
            Return False
        End Try

    End Function
    Private Function GetQuoteDetailsBodyForAwardedBids(ByVal sMessage As String) As String

        Dim sBody As String = "<strong>Market :</strong> " & LLCRow.GetRow(DB, dbBuilder.LLCID).LLC & vbCrLf

        sBody &= IIf(sMessage <> "", "Message: " & sMessage & vbCrLf & vbCrLf, "")
        'sBody &= "<strong>Quote #:</strong> " & dbQuote.QuoteId & vbCrLf
        sBody &= "<strong>Title:</strong> " & dbQuote.Title & vbCrLf
        ' sBody &= "<strong>Deadline:</strong> " & dbQuote.Deadline & vbCrLf
        sBody &= "<strong>Status:</strong> " & dbQuote.Status & " (" & dbQuote.StatusDate & ")" & vbCrLf
        sBody &= "<strong>Project: </strong>" & dbProject.ProjectName & vbCrLf
        'sBody &= "<strong>Project Status:</strong> " & ProjectStatusRow.GetRow(DB, dbProject.ProjectStatusID).ProjectStatus & vbCrLf
        sBody &= "<strong>Subdivision:</strong> " & dbProject.Subdivision & vbCrLf
        sBody &= "<strong>Lot #:</strong> " & dbProject.LotNumber & vbCrLf
        sBody &= "<strong>City:</strong> " & dbProject.City & vbCrLf
        sBody &= "<strong>State:</strong> " & dbProject.State & vbCrLf
        Dim dt As DataTable = DB.GetDataTable("SELECT qvc.QuoteId, vc.Category FROM POQuoteVendorCategory qvc INNER JOIN VendorCategory vc ON vc.VendorCategoryId = qvc.VendorCategoryId where qvc.QuoteId = " & DB.Number(dbQuote.QuoteId))
        If dt.Rows.Count > 0 Then
            Dim Conn As String = String.Empty
            Dim Result As String = String.Empty
            For Each item As DataRow In dt.Rows
                Result &= Conn & item("Category")
                Conn = ","
            Next
            sBody &= "<strong>Phases:</strong> " & Result
        End If
        Return sBody

    End Function
    Private Function GetQuoteDetailsBody(ByVal sMessage As String) As String
        Dim sBody As String = "<strong>Market :</strong> " & LLCRow.GetRow(DB, dbBuilder.LLCID).LLC & vbCrLf
        sBody &= IIf(sMessage <> "", "Message: " & sMessage & vbCrLf & vbCrLf, "")
        'sBody &= "<strong>Quote #:</strong> " & dbQuote.QuoteId & vbCrLf
        sBody &= "<strong>Title:</strong> " & dbQuote.Title & vbCrLf
        sBody &= "<strong>Deadline:</strong> " & dbQuote.Deadline & vbCrLf
        'sBody &= "<strong>Status:</strong> " & dbQuote.Status & " (" & dbQuote.StatusDate & ")" & vbCrLf
        sBody &= "<strong>Project:</strong> " & dbProject.ProjectName & vbCrLf
        'sBody &= "<strong>Project Status:</strong> " & ProjectStatusRow.GetRow(DB, dbProject.ProjectStatusID).ProjectStatus & vbCrLf
        sBody &= "<strong>Subdivision:</strong> " & dbProject.Subdivision & vbCrLf
        sBody &= "<strong>Lot #:</strong> " & dbProject.LotNumber & vbCrLf
        sBody &= "<strong>City:</strong> " & dbProject.City & vbCrLf
        sBody &= "<strong>State:</strong> " & dbProject.State & vbCrLf
        Dim dt As DataTable = DB.GetDataTable("SELECT qvc.QuoteId, vc.Category FROM POQuoteVendorCategory qvc INNER JOIN VendorCategory vc ON vc.VendorCategoryId = qvc.VendorCategoryId where qvc.QuoteId = " & DB.Number(dbQuote.QuoteId))
        If dt.Rows.Count > 0 Then
            Dim Conn As String = String.Empty
            Dim Result As String = String.Empty
            For Each item As DataRow In dt.Rows
                Result &= Conn & item("Category")
                Conn = ","
            Next
            sBody &= "<strong>Phases:</strong> " & Result
        End If
        Return sBody

    End Function

    Private Function GetRequestDetailsBody() As String
        Dim sBody As String = ""
        If txtMessage.Text <> "" Then
            '  sBody = "<strong>Request Status:</strong> " & dbQuoteRequest.RequestStatus & " (" & dbQuoteRequest.ModifyDate & ")" & vbCrLf
            '  sBody &= "<strong>Message:</strong> " & txtMessage.Text & vbCrLf
        End If

        Return sBody

    End Function
    Private Function FormatMessageWithStatus(ByVal Msg As String, ByVal sMessage As String) As String
        Msg = Msg.Replace("%%Builder%%", dbBuilder.CompanyName)
        Msg = Msg.Replace("%%QuoteDetails%%", GetQuoteDetailsBodyForAwardedBids(sMessage))
        Msg = Msg.Replace("%%QuoteRequestDetails%%", GetRequestDetailsBody())
        Msg = Msg.Replace("%%QuoteRequestUrl%%", GlobalSecureName & "/vendor/plansonline/quoterequestmessages.aspx?QuoteRequestId=" & QuoteRequestId)

        Return Msg
    End Function
    Private Function FormatMessage(ByVal Msg As String, ByVal sMessage As String) As String
        Msg = Msg.Replace("%%Builder%%", dbBuilder.CompanyName)
        Msg = Msg.Replace("%%QuoteDetails%%", GetQuoteDetailsBody(sMessage))
        Msg = Msg.Replace("%%QuoteRequestDetails%%", GetRequestDetailsBody())
        Msg = Msg.Replace("%%QuoteRequestUrl%%", GlobalSecureName & "/vendor/plansonline/quoterequestmessages.aspx?QuoteRequestId=" & QuoteRequestId)

        Return Msg
    End Function
    Private Sub SendAwardedMessage(ByVal MessageCode As String, ByVal sMessage As String)
        Try
            Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, MessageCode)
            Dim sBody As String = FormatMessageWithStatus(dbMsg.Message, sMessage)
            dbMsg.Send(VendorRow.GetRow(DB, dbQuoteRequest.VendorId), sBody, True, IIf(dbQuoteRequest.VendorContactEmail <> "", dbQuoteRequest.VendorContactEmail, ""))
            txtMessage.Text = ""
        Catch ex As Exception
            AddError("Changes saved but email failed to be sent.")
        End Try
    End Sub
    Private Sub SendMessage(ByVal MessageCode As String, ByVal sMessage As String)
        Try
            Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, MessageCode)
            Dim sBody As String = FormatMessage(dbMsg.Message, sMessage)
            dbMsg.Send(VendorRow.GetRow(DB, dbQuoteRequest.VendorId), sBody, True, IIf(dbQuoteRequest.VendorContactEmail <> "", dbQuoteRequest.VendorContactEmail, ""))

            'log  Mail Sent
            Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Mail Sent For Added Mesage", "", "", "", "", UserName)
            'end log

            txtMessage.Text = ""
        Catch ex As Exception
            AddError("Changes saved but email failed to be sent.")
        End Try
    End Sub

    Protected Sub btnAddMessage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddMessage.Click

        If Not SaveChanges() Then Exit Sub
        'log  Add Message
        Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Btn Add Message", "", "", "", "", UserName)
        'end log

        'Send email to vendor
        SendMessage("NewMessageToVendor", txtMessage.Text)
        ltlMsg.Text = "Message added and sent to the vendor successfully."
        divMsg.Visible = True
        upMessages.Update()
    End Sub

    Protected Sub btnRequestInfo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRequestInfo.Click
        If Not SaveChanges("Request Information") Then Exit Sub

        'Send email to vendor
        SendMessage("NewMessageToVendor", txtMessage.Text)

        ltlMsg.Text = "Message added and sent to the vendor successfully. Request Status updated to Request Information."
        divMsg.Visible = True
    End Sub

    Private Sub SendCancellation()
        Dim Sql As String = "Select QuoteRequestId, VendorId From POQuoteRequest Where QuoteId = " & DB.Number(dbQuote.QuoteId) & " And RequestStatus In ('New', 'Request Information') and VendorId <> " & DB.Number(dbQuoteRequest.VendorId)
        Dim dt As DataTable = DB.GetDataTable(Sql)
        If dt.Rows.Count > 0 Then
            Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "QuoteRequestDeclinedByBuilder")
            Dim sBody As String = FormatMessageWithStatus(dbMsg.Message, "")
            For Each dr As DataRow In dt.Rows
                Try
                    DB.BeginTransaction()
                    Sql = "Update POQuoteRequest Set RequestStatus = 'Declined By Builder' Where QuoteRequestId = " & DB.Number(dr("QuoteRequestId"))
                    DB.ExecuteSQL(Sql)
                    DB.CommitTransaction()
                    'send cencellation message to vendor
                    dbMsg.Send(VendorRow.GetRow(DB, dr("VendorId")), sBody, True)
                Catch ex As SqlException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                End Try
            Next
        End If
    End Sub

    Protected Sub btnAward_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAward.Click
        If Not SaveChanges("Awarded") Then Exit Sub

        'Send email to vendor
        SendAwardedMessage("QuoteRequestAwarded", txtAwardBidMessage.Text)

        'Decline all other requests
        SendCancellation()

        ltlMsg.Text = "Quote Awarded message added and sent to the vendor successfully. Request Status and Quote Status updated to Awarded."
        divMsg.Visible = True
        divAwardBid.Visible = False
        upMessages.Update()
    End Sub
    Protected Sub btnConfirmDecline_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirmDecline.Click
        divDeclineBid.Visible = True
    End Sub
    Protected Sub btnAwardDiv_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAwardDiv.Click
        divAwardBid.Visible = True
    End Sub
    Protected Sub btnCancelAwardBid_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelAward.Click
        divAwardBid.Visible = False
    End Sub
    Protected Sub btnCancelDeclineBid_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelDeclineBid.Click
        divDeclineBid.Visible = False
    End Sub

    Protected Sub btnDecline_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDecline.Click
        If Not SaveChanges("Declined By Builder") Then Exit Sub

        'Send email to vendor
        SendMessage("QuoteRequestDeclinedByBuilder", txtDeclineMessage.Text)

        ltlMsg.Text = "Quote Declined message added and sent to the vendor successfully. Request Status updated to Declined By Builder."
        divMsg.Visible = True
        divDeclineBid.Visible = False
    End Sub

    Protected Sub lnkAddMessage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkAddMessage.Click
        divAddMessage.Visible = True
    End Sub


    Protected Sub btnCancelMessage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelMessage.Click
        divAddMessage.Visible = False
    End Sub


    Private Sub BindThreads()
        Dim SQL As String

        SQL = "Select *, CreatedByUser + ' (' + (Case When BuilderId Is Null Then (Select CompanyName From Vendor Where VendorId = PORequestInfoThread.VendorId) Else (Select CompanyName From Builder Where BuilderId = PORequestInfoThread.BuilderId) End) + ')' As CreatedBy FROM PORequestInfoThread Where QuoteId = " & DB.Number(dbQuoteRequest.QuoteId) & " Order By CreateDate Desc"

        Dim res As DataTable = DB.GetDataTable(SQL)
        gvThreads.DataSource = res.DefaultView
        gvThreads.DataBind()
    End Sub

    Private Sub BindPosts(ByVal ThreadId As Integer)
        Dim SQL As String

        SQL = "Select *, CreatedByUser + ' (' + (Case When BuilderId Is Null Then (Select CompanyName From Vendor Where VendorId = PORequestInfoMessage.VendorId) Else (Select CompanyName From Builder Where BuilderId = PORequestInfoMessage.BuilderId) End) + ')' As CreatedBy FROM PORequestInfoMessage Where ThreadId = " & DB.Number(ThreadId) & " Order By CreateDate Desc"

        Dim res As DataTable = DB.GetDataTable(SQL)
        gvPosts.DataSource = res.DefaultView
        gvPosts.DataBind()
    End Sub

    Protected Sub btnAddThread_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddThread.Click
        Try
            If txtThread.Text <> "" Then
                DB.BeginTransaction()

                Dim dbPORequestInfoThread As PORequestInfoThreadRow

                dbPORequestInfoThread = New PORequestInfoThreadRow(DB)
                dbPORequestInfoThread.QuoteId = dbQuoteRequest.QuoteId
                dbPORequestInfoThread.Thread = txtThread.Text
                dbPORequestInfoThread.BuilderId = Session("BuilderId")
                dbPORequestInfoThread.CreatedByUser = dbBuilderAccount.FirstName & " " & dbBuilderAccount.LastName

                dbPORequestInfoThread.Insert()

                DB.CommitTransaction()
                'log Add Topic
                NewTopicTreadId = dbPORequestInfoThread.ThreadId
                Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Add Topic", NewTopicTreadId, "", "", "", UserName)
                'end log

                'Send email to all vendors
                'Dim Sql As String = "Select QuoteRequestId, VendorId From POQuoteRequest Where QuoteId = " & DB.Number(dbQuote.QuoteId) & " And RequestStatus In ('New', 'Request Information')"
                'Dim dt As DataTable = DB.GetDataTable(Sql)
                'If dt.Rows.Count > 0 Then
                '    Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "NewRFIThread")
                '    Dim sBody As String = FormatMessage(dbMsg.Message)
                '    sBody = sBody.Replace("%%Thread%%", "Added by Builder: " & dbBuilder.CompanyName & vbCrLf & "Thread: " & dbPORequestInfoThread.Thread)
                '    For Each dr As DataRow In dt.Rows
                '        Try
                '            dbMsg.Send(VendorRow.GetRow(DB, dr("VendorId")), sBody, True)
                '        Catch ex As SqlException
                '        End Try
                '    Next
                'End If

                divAddThread.Visible = False

                txtThread.Text = ""

                BindThreads()

                hdnThreadId.Value = dbPORequestInfoThread.ThreadId
                ltlThread.Text = dbPORequestInfoThread.Thread
                BindPosts(dbPORequestInfoThread.ThreadId)
                lnkAddPost.Visible = True
                ltlNoThread.Visible = False
                gvPosts.Visible = True
                pThread.Visible = True
                trThread.Visible = False
                trPost.Visible = True
                divAddPost.Visible = True
            End If


        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try

    End Sub

    Protected Sub btnAddPost_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddPost.Click
        Try
            If txtPost.Text <> "" Then

                DB.BeginTransaction()

                Dim dbPORequestInfoMessage As PORequestInfoMessageRow

                dbPORequestInfoMessage = New PORequestInfoMessageRow(DB)
                dbPORequestInfoMessage.ThreadId = hdnThreadId.Value
                dbPORequestInfoMessage.Message = txtPost.Text
                dbPORequestInfoMessage.BuilderId = Session("BuilderId")
                dbPORequestInfoMessage.CreatedByUser = dbBuilderAccount.FirstName & " " & dbBuilderAccount.LastName

                dbPORequestInfoMessage.Insert()

                DB.CommitTransaction()

                'log Add Post
                NewPostThreadId = dbPORequestInfoMessage.ThreadId
                Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Add Post", NewPostThreadId, "", "", "", UserName)
                'end log

                'Send email to all vendors
                Dim Sql As String = "Select QuoteRequestId, VendorId From POQuoteRequest Where QuoteId = " & DB.Number(dbQuote.QuoteId) & " And RequestStatus In ('New', 'Request Information')"
                Dim dt As DataTable = DB.GetDataTable(Sql)
                If dt.Rows.Count > 0 Then
                    Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "NewRFIPost")
                    Dim sBody As String = FormatMessage(dbMsg.Message, "")
                    sBody = sBody.Replace("%%Thread%%", "Posted by Builder: " & dbBuilder.CompanyName & vbCrLf & "Thread: " & PORequestInfoThreadRow.GetRow(DB, dbPORequestInfoMessage.ThreadId).Thread) & vbCrLf & vbCrLf
                    sBody = sBody.Replace("%%Post%%", "Post: " & dbPORequestInfoMessage.Message)
                    For Each dr As DataRow In dt.Rows
                        Try
                            dbMsg.Send(VendorRow.GetRow(DB, dr("VendorId")), sBody, True)
                        Catch ex As SqlException
                        End Try
                    Next
                End If

                divAddPost.Visible = False

                txtPost.Text = ""

                BindPosts(hdnThreadId.Value)

            End If

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try

    End Sub

    Protected Sub btnCancelThread_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelThread.Click
        divAddThread.Visible = False
    End Sub

    Protected Sub lnkAddThread_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkAddThread.Click
        divAddThread.Visible = True
    End Sub

    Protected Sub btnCancelPost_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelPost.Click
        divAddPost.Visible = False
    End Sub

    Protected Sub lnkAddPost_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkAddPost.Click
        divAddPost.Visible = True
    End Sub

    Protected Sub gvThreads_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvThreads.RowCommand
        Select Case e.CommandName
            Case Is = "Remove"
                Try
                    DB.BeginTransaction()
                    PORequestInfoThreadRow.RemoveRow(DB, e.CommandArgument)
                    DB.CommitTransaction()
                    BindThreads()
                    hdnThreadId.Value = Nothing
                    lnkAddPost.Visible = False
                    BindPosts(0)
                    ltlNoThread.Visible = True
                    gvPosts.Visible = False
                    trPost.Visible = False
                    pThread.Visible = False
                    'log Delete Topic
                    RemoveThreadId = Convert.ToString(e.CommandArgument)
                    Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Delete Topic", RemoveThreadId, "", "", "", UserName)
                    'end log
                Catch ex As SqlException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    AddError(ErrHandler.ErrorText(ex))
                End Try
            Case Is = "GetMessages"
                hdnThreadId.Value = e.CommandArgument
                Dim dbThread As PORequestInfoThreadRow = PORequestInfoThreadRow.GetRow(DB, e.CommandArgument)
                ltlThread.Text = dbThread.Thread
                BindPosts(e.CommandArgument)
                lnkAddPost.Visible = True
                ltlNoThread.Visible = False
                gvPosts.Visible = True
                pThread.Visible = True
                trThread.Visible = False
                trPost.Visible = True
                'log Get Post
                GetThreadId = dbThread.ThreadId
                Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Show Posts", GetThreadId, "", "", "", UserName)
                'end log
        End Select
    End Sub

    Protected Sub gvPosts_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvPosts.RowCommand
        Select Case e.CommandName
            Case Is = "Remove"
                Try
                    DB.BeginTransaction()
                    PORequestInfoMessageRow.RemoveRow(DB, e.CommandArgument)
                    DB.CommitTransaction()
                    BindPosts(hdnThreadId.Value)
                    'log Delete Post
                    MessageId = Convert.ToString(e.CommandArgument)
                    Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Delete Post", MessageId, "", "", "", UserName)
                    'end log
                Catch ex As SqlException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    AddError(ErrHandler.ErrorText(ex))
                End Try
        End Select
    End Sub

    Protected Sub gvThreads_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvThreads.RowDataBound
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If
        Dim lnkDelete As ConfirmImageButton = e.Row.FindControl("lnkDelete")
        lnkDelete.CommandArgument = e.Row.DataItem("ThreadId")

        Dim lnkMessages As Button = e.Row.FindControl("lnkMessages")
        lnkMessages.CommandArgument = e.Row.DataItem("ThreadId")

    End Sub

    Protected Sub gvPosts_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvPosts.RowDataBound
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If
        Dim lnkDelete As ConfirmImageButton = e.Row.FindControl("lnkDelete")
        lnkDelete.CommandArgument = e.Row.DataItem("MessageId")

    End Sub

    Protected Sub lnkBackThread_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkBackThread.Click
        trThread.Visible = True
        trPost.Visible = False
    End Sub

End Class

