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
    Protected dbVendor As VendorRow
    Protected VendorAccount As VendorAccountRow

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
        CheckVendorAccess(Request.Url.PathAndQuery)

        If Not CType(Me.Page, SitePage).IsLoggedInVendor Then
            Response.Redirect("/default.aspx")
        End If

        dbVendor = VendorRow.GetRow(DB, Session("VendorId"))

        If Not dbVendor.IsPlansOnline Then
            Response.Redirect("noaccess.aspx")
        End If

        QuoteRequestId = Convert.ToInt32(Request("QuoteRequestId"))

        If QuoteRequestId = 0 Then
            Response.Redirect("/default.aspx")
        End If

        VendorAccount = VendorAccountRow.GetRow(Me.DB, Session("VendorAccountId"))

        dbQuoteRequest = POQuoteRequestRow.GetRow(DB, QuoteRequestId)
        dbQuote = POQuoteRow.GetRow(DB, dbQuoteRequest.QuoteId)
        dbProject = ProjectRow.GetRow(DB, dbQuote.ProjectId)

        If dbQuoteRequest.VendorId <> Session("VendorId") Then
            Response.Redirect("/default.aspx")
        End If

        mudUpload.QuoteId = dbQuoteRequest.QuoteId
        mudUpload.VendorId = Session("VendorId")

        gvList.BindList = AddressOf BindList
        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            LoadDetails()
            BindThreads()
        End If
        PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorId")
        UserName = Session("Username")
    End Sub

    Protected Sub LoadDetails()

        dbQuoteRequest = POQuoteRequestRow.GetRow(DB, QuoteRequestId)
        dbQuote = POQuoteRow.GetRow(DB, dbQuoteRequest.QuoteId)

        'btnDecline.Visible = True
        'btnRequestInfo.Visible = True

        If dbQuoteRequest.RequestStatus = "Awarded" Or dbQuoteRequest.RequestStatus = "Declined By Vendor" Or dbQuoteRequest.RequestStatus = "Exited Market" Or dbQuoteRequest.RequestStatus = "Declined By Builder" Then
            'btnDecline.Visible = False
            'btnRequestInfo.Visible = False
            btnAddMessage.Visible = False
        End If
        If dbQuote.Status = "Awarded" Or dbQuote.Status = "Cancelled" Then
            btnAddMessage.Visible = False
        End If
        'If dbQuoteRequest.RequestStatus = "Declined By Vendor" Then
        '    btnDecline.Visible = False
        'End If
        If dbQuoteRequest.RequestStatus = "Request Information" Then
            'btnRequestInfo.Visible = False
        End If

        'btnDeclineDiv.Visible = btnDecline.Visible

        ltlProject.Text = dbProject.ProjectName & " (" & dbProject.Subdivision
        If dbProject.City <> "" Then
            ltlProject.Text &= ", " & dbProject.City
        End If
        If dbProject.State <> "" Then
            ltlProject.Text &= ", " & dbProject.State
        End If
        ltlProject.Text &= ")</a>"

        ltlQuoteId.Text = dbQuote.QuoteId
        ltlQuote.Text = dbQuote.Title
        ltlQuoteDeadline.Text = "<b><span class=""red"">" & dbQuote.Deadline & "</span></b>"
        ltlStatus.Text = dbQuote.Status
        'ltlStatusDate.Text = dbQuote.StatusDate
        ltlInstructions.Text = Core.Text2HTML(dbQuote.Instructions)
        ltlBuilder.Text = BuilderRow.GetRow(DB, dbProject.BuilderID).CompanyName
        ltlBuilderContact.Text = dbProject.ContactName
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

        If dbQuoteRequest.VendorContactName <> "" Then
            txtContactName.Text = dbQuoteRequest.VendorContactName
        Else
            txtContactName.Text = VendorAccount.FirstName & " " & VendorAccount.LastName
        End If

        If dbQuoteRequest.VendorContactPhone <> "" Then
            txtContactPhone.Text = dbQuoteRequest.VendorContactPhone
        Else
            txtContactPhone.Text = VendorAccount.Phone
        End If

        If dbQuoteRequest.VendorContactEmail <> "" Then
            txtContactEmail.Text = dbQuoteRequest.VendorContactEmail
        Else
            txtContactEmail.Text = VendorAccount.Email
        End If

        txtQuoteTotal.Text = dbQuoteRequest.QuoteTotal
        dtExpiration.Value = dbQuoteRequest.QuoteExpirationDate

        txtStartDate.Text = dbQuoteRequest.StartDate
        txtCompletionTime.Text = dbQuoteRequest.CompletionTime
        ltlPaymentTerms2.Text = dbVendor.PaymentTerms

        ltlStartDate.Text = dbQuoteRequest.StartDate
        ltlCompletionTime.Text = dbQuoteRequest.CompletionTime
        ltlPaymentTerms.Text = dbQuoteRequest.PaymentTerms

        If dbQuote.Status = "Awarded" Then
            trQuoteTotal.Visible = False
            trQuoteExpiration.Visible = False
        End If

        ltlVendorCategory.Text = ""

        Dim dt As DataTable = DB.GetDataTable("Select Category From VendorCategory Where VendorCategoryID In " & DB.NumberMultiple(dbQuote.GetSelectedVendorCategories()))
        Dim sConn As String = ""
        For Each dr As DataRow In dt.Rows
            ltlVendorCategory.Text &= sConn & dr("Category")
            sConn = ", "
        Next

        If gvList.SortBy = String.Empty Then
            gvList.SortBy = "CreateDate"
            gvList.SortOrder = "DESC"
        End If

        BindDocuments(dbQuote.QuoteId)
        BindVendorDocuments(dbQuote.QuoteId)
        BindList()
        upDetails.Update()
        upMessages.Update()
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
        lnkMessageTitle.HRef = "/assets/plansonline/builderdocument/" & e.Item.DataItem("FileName").ToString()

    End Sub

    Protected Sub BindVendorDocuments(ByVal QuoteId As Integer)

        Dim SQL As String = String.Empty
        Dim dt As DataTable

        SQL = "Select * From POVendorDocument Where VendorId = " & DB.Number(dbQuoteRequest.VendorId) & " And VendorDocumentId In (Select VendorDocumentId From POQuoteVendorDocument Where QuoteId = " & DB.Number(QuoteId) & ")"

        dt = DB.GetDataTable(SQL)
        If dt.Rows.Count > 0 Then

            Me.rptVendorDocuments.DataSource = dt

            divNoVendorDocuments.Visible = False
        End If

        Me.rptVendorDocuments.DataBind()
    End Sub

    Protected Sub rptVendorDocuments_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptVendorDocuments.ItemDataBound

        If Not e.Item.ItemType = ListItemType.Item And Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim lnkMessageTitle As HtmlAnchor = e.Item.FindControl("lnkMessageTitle")
        lnkMessageTitle.HRef = "/assets/plansonline/vendordocument/" & e.Item.DataItem("FileName").ToString

        Dim lnkDelete As ConfirmImageButton = e.Item.FindControl("lnkDelete")
        lnkDelete.CommandArgument = e.Item.DataItem("VendorDocumentId")

    End Sub

    Protected Sub rptVendorDocuments_ItemCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptVendorDocuments.ItemCommand
        Select Case e.CommandName
            Case Is = "Remove"
                Try
                    DB.BeginTransaction()
                    Dim FileInfo As System.IO.FileInfo
                    Try
                        FileInfo = New System.IO.FileInfo(Server.MapPath("/assets/plansonline/vendordocument/" & POVendorDocumentRow.GetRow(DB, e.CommandArgument).FileName))
                        FileInfo.Delete()
                    Catch ex As Exception

                    End Try
                    POVendorDocumentRow.RemoveRow(DB, e.CommandArgument)
                    DB.CommitTransaction()
                    BindVendorDocuments(dbQuote.QuoteId)
                Catch ex As SqlException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    AddError(ErrHandler.ErrorText(ex))
                End Try
        End Select
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

        btnRead.Visible = IsDBNull(e.Row.DataItem("FromVendorId"))

        If btnRead.Visible Then
            If e.Row.DataItem("IsRead") Then
                btnRead.Text = "Mark as Unread"
            Else
                e.Row.Style.Add("font-weight", "bold")
            End If
        Else
            lnkAddMessage.Text = "Submit an Updated Bid"
            ltlSubmitBidHeader.Text = "Bid Form"
            btnAddMessage.Text = "Submit an Updated Bid"
        End If

        Dim ltlMessage As Literal = e.Row.FindControl("ltlMessage")

        ltlMessage.Text = Core.Text2HTML(e.Row.DataItem("Message"))

    End Sub

    Protected Function SaveChanges(Optional ByVal Status As String = "", Optional ByVal JustMessage As Boolean = False) As Boolean

        Try
            DB.BeginTransaction()
            If JustMessage = False Then
                If Status <> "" Then
                    dbQuoteRequest.RequestStatus = Status
                End If
                dbQuoteRequest.VendorContactName = txtContactName.Text
                dbQuoteRequest.VendorContactEmail = txtContactEmail.Text
                dbQuoteRequest.VendorContactPhone = txtContactPhone.Text
                dbQuoteRequest.QuoteTotal = txtQuoteTotal.Text
                dbQuoteRequest.QuoteExpirationDate = dtExpiration.Value
                dbQuoteRequest.StartDate = txtStartDate.Text
                dbQuoteRequest.CompletionTime = txtCompletionTime.Text
                dbQuoteRequest.PaymentTerms = dbVendor.PaymentTerms
                dbQuoteRequest.Update()

                Dim sMessage As String
                If Status = "Declined By Vendor" Then
                    If ltbDeclineMessage.Text = Nothing Then
                        ltbDeclineMessage.Text = "Declined By Vendor"
                    End If
                    sMessage = ltbDeclineMessage.Text
                Else
                    If txtMessage.Text = Nothing Then
                        txtMessage.Text = "No Message"
                    End If
                    sMessage = txtMessage.Text
                End If
                If sMessage <> "" Then
                    Dim dbMessage As POQuoteRequestMessageRow = New POQuoteRequestMessageRow(DB)
                    dbMessage.QuoteRequestId = QuoteRequestId
                    dbMessage.Message = sMessage
                    dbMessage.FromVendorId = Session("VendorId")
                    dbMessage.FromName = VendorAccount.FirstName & " " & VendorAccount.LastName
                    dbMessage.MessageQuoteStatus = dbQuoteRequest.RequestStatus
                    dbMessage.MessageQuoteTotal = dbQuoteRequest.QuoteTotal
                    dbMessage.MessageQuoteExpirationDate = dbQuoteRequest.QuoteExpirationDate

                    dbMessage.Insert()
                End If
            Else
                If txtPrivateMessage.Text <> "" Then
                    Dim dbMessage As POQuoteRequestMessageRow = New POQuoteRequestMessageRow(DB)
                    dbMessage.QuoteRequestId = QuoteRequestId
                    dbMessage.Message = txtPrivateMessage.Text
                    dbMessage.FromVendorId = Session("VendorId")
                    dbMessage.FromName = VendorAccount.FirstName & " " & VendorAccount.LastName
                    dbMessage.MessageQuoteStatus = dbQuoteRequest.RequestStatus
                    dbMessage.MessageQuoteTotal = dbQuoteRequest.QuoteTotal
                    dbMessage.MessageQuoteExpirationDate = dbQuoteRequest.QuoteExpirationDate

                    dbMessage.Insert()
                End If
            End If



            DB.CommitTransaction()

            LoadDetails()

            divAddMessage.Visible = False

            Return True
        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
            Return False
        End Try

    End Function

    Private Function GetQuoteDetailsBody(ByVal sMessage As String) As String
        Dim sBody As String = IIf(sMessage <> "", "<strong>Message</strong>: " & sMessage & vbCrLf & vbCrLf, "")
        'sBody &= "Bid Request #: " & dbQuote.QuoteId & vbCrLf
        sBody &= "<strong>Title</strong>: " & dbQuote.Title & vbCrLf
        sBody &= "<strong>Deadline</strong>: " & dbQuote.Deadline & vbCrLf
        'sBody &= "<strong>Status</strong>: " & dbQuote.Status & " (" & dbQuote.StatusDate & ")" & vbCrLf
        sBody &= "<strong>Project</strong>: " & dbProject.ProjectName & vbCrLf
        sBody &= "<strong>Subdivision</strong>: " & dbProject.Subdivision & vbCrLf
        sBody &= "<strong>Lot #</strong>: " & dbProject.LotNumber & vbCrLf
        sBody &= "<strong>City</strong>: " & dbProject.City & vbCrLf
        sBody &= "<strong>State</strong>: " & dbProject.State & vbCrLf
        Dim dt As DataTable = DB.GetDataTable("SELECT qvc.QuoteId, vc.Category FROM POQuoteVendorCategory qvc INNER JOIN VendorCategory vc ON vc.VendorCategoryId = qvc.VendorCategoryId where qvc.QuoteId = " & DB.Number(dbQuote.QuoteId))
        If dt.Rows.Count > 0 Then
            Dim Conn As String = String.Empty
            Dim Result As String = String.Empty
            For Each item As DataRow In dt.Rows
                Result &= Conn & item("Category")
                Conn = ","
            Next
            sBody &= "<strong>Phases</strong>: " & Result
        End If
        Return sBody

    End Function


    Private Function GetRequestDetailsBody(Optional ByVal ShowBidDetails As Boolean = False) As String
        Dim sBody As String = String.Empty
        If ShowBidDetails Then
            sBody &= vbCrLf
            sBody &= "<strong>Bid Total</strong>: " & IIf(dbQuoteRequest.QuoteTotal > 0, FormatCurrency(dbQuoteRequest.QuoteTotal, 2), "") & vbCrLf
            sBody &= "<strong>Expiration Date</strong>: " & IIf(Not dbQuoteRequest.QuoteExpirationDate = Nothing, dbQuoteRequest.QuoteExpirationDate, "") & vbCrLf
        End If
        Return sBody

    End Function

    Private Function FormatMessage(ByVal Msg As String, ByVal sMessage As String, Optional ByVal AddBidDetails As Boolean = False) As String

        Msg = Msg.Replace("%%Vendor%%", dbVendor.CompanyName)
        Msg = Msg.Replace("%%QuoteDetails%%", GetQuoteDetailsBody(sMessage))
        Msg = Msg.Replace("%%QuoteRequestDetails%%", GetRequestDetailsBody(AddBidDetails))
        Msg = Msg.Replace("%%QuoteRequestUrl%%", GlobalSecureName & "/builder/plansonline/quoterequestmessages.aspx?QuoteRequestId=" & QuoteRequestId)

        Return Msg
    End Function

    Private Sub SendMessage(ByVal MessageCode As String, ByVal sMessage As String, Optional ByVal IncludeBiddetails As Boolean = False)
        Try
            Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, MessageCode)
            Dim sBody As String = FormatMessage(dbMsg.Message, sMessage, IncludeBiddetails)

            dbMsg.Send(BuilderRow.GetRow(DB, dbProject.BuilderID), sBody, True, IIf(dbProject.ContactEmail <> "" AndAlso dbProject.ContactEmail.ToLower <> BuilderRow.GetRow(DB, dbProject.BuilderID).Email.ToLower, dbProject.ContactEmail, ""))
            txtMessage.Text = ""
            'log Sent Mail  
            Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Mail Sent", QuoteRequestId, "", "", "", UserName)
            'end log
        Catch ex As Exception
            AddError("Changes saved but email failed to be sent.")
        End Try
    End Sub

    Protected Sub btnAddMessage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddMessage.Click

        If Not IsValid Then Exit Sub
        If Not SaveChanges() Then Exit Sub
        'log  Add Message
        Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Submit Bid", QuoteRequestId, "", "", "", UserName)
        'end log
        'Send email to builder
        SendMessage("NewBidToBuilder", txtMessage.Text, True)

        ltlMsg.Text = "Bid sent to the builder successfully."
        Session("BidMsg") = "Bid sent to the builder successfully."
        Response.Redirect("default.aspx")
        divMsg.Visible = True
    End Sub
    Protected Sub btnAddMessage2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddMessage2.Click
        If Not IsValid Then Exit Sub
        If Not SaveChanges("", True) Then Exit Sub
        'log  Add Message
        Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Add Message", "", "", "", "", UserName)
        'end log 
        SendMessage("NewMessageToBuilder", txtPrivateMessage.Text)
        ltlMsg2.Text = "Message added and sent to the builder successfully."
        divAddMsg.Visible = True
        divSendMessage.Visible = False
    End Sub

    Protected Sub btnRequestInfo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRequestInfo.Click
        If Not IsValid Then Exit Sub
        If Not SaveChanges("Request Information") Then Exit Sub

        'Send email to builder
        SendMessage("NewMessageToBuilder", txtMessage.Text)

        ltlMsg.Text = "Message added and sent to the builder successfully. Request Status updated to Request Information."
        divMsg.Visible = True
    End Sub
    'Protected Sub btnDeclineDiv_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeclineDiv.Click
    '    'divDeclineBid.Visible = True
    '    divAddMessage.Visible = False
    '    'upCancelBid.Update()
    'End Sub

    Protected Sub btnDecline_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDecline.Click
        If Not SaveChanges("Declined By Vendor") Then Exit Sub

        'log  Decline Bid
        Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Decline Bid", QuoteRequestId, "", "", "", UserName)
        'end log

        'Send email to builder
        SendMessage("QuoteRequestDeclinedByVendor", ltbDeclineMessage.Text)

        ltlMsg.Text = "Quote Declined message added and sent to the builder successfully. Request Status updated to Declined By Vendor."
        divMsg.Visible = True
        'divDeclineBid.Visible = False

    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        If Not SaveChanges() Then Exit Sub
        'log Save Changes
        Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Save Changes", QuoteRequestId, "", "", "", UserName)
        'end log
        ltlMsg.Text = "Quote Details updated with success.  No email was sent to the builder."
        divMsg.Visible = True
    End Sub

    Protected Sub lnkAddMessage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkAddMessage.Click
        divAddMessage.Visible = True
        'divDeclineBid.Visible = False
    End Sub
    Protected Sub lnkSendMessage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkSendMessage.Click
        divSendMessage.Visible = True
    End Sub

    Protected Sub btnCancelMessage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelMessage.Click
        divAddMessage.Visible = False
    End Sub
    Protected Sub btnCancelMessage2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelMessage2.Click
        divSendMessage.Visible = False
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
                dbPORequestInfoThread.VendorId = Session("VendorId")
                dbPORequestInfoThread.CreatedByUser = VendorAccount.FirstName & " " & VendorAccount.LastName

                dbPORequestInfoThread.Insert()

                DB.CommitTransaction()

                'log Add Topic
                NewTopicTreadId = dbPORequestInfoThread.ThreadId
                Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Add Topic", NewTopicTreadId, "", "", "", UserName)
                'end log

                'Send email to builder
                'Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "NewRFIThread")
                'Dim sBody As String = FormatMessage(dbMsg.Message)
                'sBody = sBody.Replace("%%Thread%%", "Added by Vendor: " & dbVendor.CompanyName & vbCrLf & "Thread: " & dbPORequestInfoThread.Thread)

                'dbMsg.Send(BuilderRow.GetRow(DB, dbProject.BuilderID), sBody, True)

                ''Send email to all vendors
                'Dim Sql As String = "Select QuoteRequestId, VendorId From POQuoteRequest Where VendorId <> " & DB.Number(Session("VendorId")) & " And QuoteId = " & DB.Number(dbQuote.QuoteId) & " And RequestStatus In ('New', 'Request Information')"
                'Dim dt As DataTable = DB.GetDataTable(Sql)
                'If dt.Rows.Count > 0 Then
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
                dbPORequestInfoMessage.VendorId = Session("VendorId")
                dbPORequestInfoMessage.CreatedByUser = VendorAccount.FirstName & " " & VendorAccount.LastName

                dbPORequestInfoMessage.Insert()

                DB.CommitTransaction()

                'log Add Post
                NewPostThreadId = dbPORequestInfoMessage.ThreadId
                Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Add Post", NewPostThreadId, "", "", "", UserName)
                'end log

                'Send email to builder
                Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "NewRFIPost")
                Dim sBody As String = FormatMessage(dbMsg.Message, "")
                sBody = sBody.Replace("%%Thread%%", "Posted by Vendor: " & dbVendor.CompanyName & vbCrLf & "Builder : " & BuilderRow.GetRow(DB, dbProject.BuilderID).CompanyName & vbCrLf & "Thread: " & PORequestInfoThreadRow.GetRow(DB, dbPORequestInfoMessage.ThreadId).Thread)
                sBody = sBody.Replace("%%Post%%", "Post: " & dbPORequestInfoMessage.Message)

                dbMsg.Send(BuilderRow.GetRow(DB, dbProject.BuilderID), sBody, True)

                'Send email to all vendors
                Dim Sql As String = "Select QuoteRequestId, VendorId From POQuoteRequest Where VendorId <> " & DB.Number(Session("VendorId")) & " And QuoteId = " & DB.Number(dbQuote.QuoteId) & " And RequestStatus In ('New', 'Request Information')"
                Dim dt As DataTable = DB.GetDataTable(Sql)
                If dt.Rows.Count > 0 Then
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

                    'log Delete Post
                    ThreadId = Convert.ToString(e.CommandArgument)
                    Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Delete Post", ThreadId, "", "", "", UserName)
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
                    'log remove message
                    MessageId = Convert.ToString(e.CommandArgument)
                    Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Remove Message", MessageId, "", "", "", UserName)
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
        lnkDelete.Visible = False

        Dim lnkMessages As Button = e.Row.FindControl("lnkMessages")
        lnkMessages.CommandArgument = e.Row.DataItem("ThreadId")

    End Sub

    Protected Sub gvPosts_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvPosts.RowDataBound
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If
        Dim lnkDelete As ConfirmImageButton = e.Row.FindControl("lnkDelete")
        lnkDelete.CommandArgument = e.Row.DataItem("MessageId")
        lnkDelete.Visible = False
    End Sub

    Protected Sub btnTransferDocs_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTransferDocs.Click
        BindVendorDocuments(dbQuoteRequest.QuoteId)
    End Sub

    Protected Sub lnkBackThread_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkBackThread.Click
        trThread.Visible = True
        trPost.Visible = False
    End Sub

End Class

