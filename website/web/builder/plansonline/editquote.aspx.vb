Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components
Imports Controls
Imports System.Configuration.ConfigurationManager
Imports System.Linq
Imports System.Collections.Generic

Public Class EditQuote
    Inherits SitePage

    Protected QuoteId As Integer
    Private dtVendors As DataTable = Nothing
    Private dbQuote As POQuoteRow
    Private dbProject As ProjectRow
    Private dbBuilder As BuilderRow
    Protected LLC As Integer = 0
    Protected Redir As String = String.Empty
    Private AccDB As New Database
    Protected AllowExcludingVendors As Boolean = False

    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureBuilderAccess()

        If Not CType(Me.Page, SitePage).IsLoggedInBuilder Then
            Response.Redirect("/default.aspx")
        End If


        dbBuilder = BuilderRow.GetRow(DB, Session("BuilderId"))

        If Not dbBuilder.IsPlansOnline Then
            Response.Redirect("noaccess.aspx")
        End If

        LLC = dbBuilder.LLCID

        AllowExcludingVendors = LLCRow.GetRow(DB, dbBuilder.LLCID).AllowExcludingVendors
        QuoteId = Convert.ToInt32(Request("QuoteId"))

        If QuoteId > 0 Then
            dbQuote = POQuoteRow.GetRow(DB, QuoteId)

            dbProject = ProjectRow.GetRow(DB, dbQuote.ProjectId)
            If dbProject.BuilderID <> Session("BuilderId") Then
                Response.Redirect("/default.aspx")
            End If
        End If

        mudUpload.QuoteId = QuoteId
        mudUpload.BuilderId = Session("BuilderId")

        If Request("QuoteRequestId") <> String.Empty Then
            Redir = "quoterequestmessages.aspx?QuoteRequestId=" & Request("QuoteRequestId")
            btnBack.Visible = True
        End If

        If Not IsPostBack Then
            Session("VendorIds") = Nothing

            LoadFromDB()

            If QuoteId = 0 Then
                Try
                    If Request("F_ProjectId") <> String.Empty Then
                        drpProjectId.SelectedValue = Request("F_ProjectId")
                    End If
                Catch ex As Exception
                End Try
                phStep2.Visible = False
                btnSave.Text = "Save & Continue"
                btnStartBid.Visible = False
            End If

        End If
        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")
    End Sub

    Private Sub LoadFromDB()
        drpProjectId.DataSource = ProjectRow.GetBuilderProjects(DB, Session("BuilderId"), "ProjectName")
        drpProjectId.DataValueField = "ProjectId"
        drpProjectId.DataTextField = "ProjectName"
        drpProjectId.DataBind()
        drpProjectId.Items.Insert(0, New ListItem("", ""))

        lsSupplyPhases.DataSource = VendorCategoryRow.GetPOList(DB)
        lsSupplyPhases.DataTextField = "Category"
        lsSupplyPhases.DataValueField = "VendorCategoryID"
        lsSupplyPhases.DataBind()

        If QuoteId = 0 Then
            btnUpdate.Visible = False
            btnStopBid.Visible = False
            Exit Sub
        End If

        txtTitle.Text = dbQuote.Title
        txtInstructions.Text = dbQuote.Instructions
        ltlAwardedTotal.Text = dbQuote.AwardedTotal
        dtDeadline.Value = dbQuote.Deadline
        ltlStatusDate.Text = dbQuote.StatusDate
        ltlAwardedDate.Text = dbQuote.AwardedDate
        drpProjectId.SelectedValue = dbQuote.ProjectId
        ltlStatus.Text = dbQuote.Status
        If dbQuote.Status = "New" Then
            ltlStatus.Text = "Pending - Bid Request Not Sent"
        End If
        ltlAwardedToVendor.Text = VendorRow.GetRow(DB, dbQuote.AwardedToVendorId).CompanyName
        lsSupplyPhases.SelectedValues = dbQuote.GetSelectedVendorCategories()
        If dbQuote.Status = "Awarded" Then
            phawarded.Visible = True
        End If
        btnUpdate.Visible = dbQuote.Status = "Bidding In Progress"
        btnStartBid.Visible = (dbQuote.Status = "New" Or dbQuote.Status = "Cancelled")
        btnStopBid.Visible = dbQuote.Status <> "Cancelled"

        'If dbQuote.Status = "Cancelled" Then btnStartBid.Text = "Resume Bidding Process"

        LoadVendors()
        BindDocuments()
    End Sub

    Protected Sub LoadVendors()
        If LLC > 0 Then
            Dim SQL As String
            SQL = "select VendorId,HistoricID, CompanyName from Vendor where IsActive = 1 And IsPlansOnline = 1 "
            If Not lsSupplyPhases.SelectedValues = String.Empty Then

                SQL &= " and VendorId in (select VendorID from VendorCategoryVendor where VendorCategoryId in (" & lsSupplyPhases.SelectedValues & "))"
                SQL &= " and VendorId in (select VendorId from LLCVendor where LLCID =" & DB.Number(LLC) & " )"
                SQL &= " Order By CompanyName"
                dtVendors = DB.GetDataTable(SQL)

                ltlVendors.Text = ""

                If dtVendors.Rows.Count > 0 Then
                    rptVendors.DataSource = dtVendors
                    'Dim dt As DataTable = LoadAccount()

                    ''Dim vendorFlag As String = String.Empty
                    ''If (Not Core.GetBoolean(dv("IsOverwrite"))) Then
                    ''    If (Core.GetDate(dr("InvoiceDate")) < DateAdd(DateInterval.Day, 30, Date.Today) AndAlso Core.GetDouble(dr("TotalDue")) > 0) Then
                    ''        vendorFlag = "Flagged for Past Due Rebates"
                    ''    ElseIf Core.GetBoolean(dv("IsFlaggedForRebates")) Then
                    ''        vendorFlag = "Flagged for Past Due Rebates"
                    ''    End If

                    ''    If Core.GetBoolean(dv("IsFlaggedForRatings")) Then
                    ''        vendorFlag &= "Flagged for Low Vendor Rating"
                    ''    End If
                    ''End If

                    ''Dim result() As DataRow = (From dr As DataRow In dt.AsEnumerable Join dv As DataRow In dtVendors.AsEnumerable _
                    ''         On Core.GetInt(dr("VendorHistoricID")) Equals Core.GetInt(dv("HistoricID")) Order By dv("CompanyName") Select dr, dv)

                    ''For Each dr As DataRow In result
                    ''    Dim vendorFlag As String = String.Empty
                    ''    If (Not Core.GetBoolean(dr("IsOverwrite"))) Then
                    ''        If (Core.GetDate(dr("InvoiceDate")) < DateAdd(DateInterval.Day, 30, Date.Today) AndAlso Core.GetDouble(dr("TotalDue")) > 0) Then
                    ''            vendorFlag = "Flagged for Past Due Rebates"
                    ''        ElseIf Core.GetBoolean(dr("IsFlaggedForRebates")) Then
                    ''            vendorFlag = "Flagged for Past Due Rebates"
                    ''        End If

                    ''        If Core.GetBoolean(dr("IsFlaggedForRatings")) Then
                    ''            vendorFlag &= "Flagged for Low Vendor Rating"
                    ''        End If
                    ''    End If

                    ''    dr("VendorFlag") = vendorFlag
                    ''Next

                    'rptVendors.DataSource = (From dr As DataRow In dt.AsEnumerable Join dv As DataRow In dtVendors.AsEnumerable _
                    '                   On Core.GetInt(dr("VendorHistoricID")) Equals Core.GetInt(dv("HistoricID")) Order By dv("CompanyName") _
                    '                   Select New With { _
                    '                                 .VendorRebateFlag = IIf(Not Core.GetBoolean(dv("IsOverwrite")), IIf((Core.GetDate(dr("InvoiceDate")) < DateAdd(DateInterval.Day, 30, Date.Today) AndAlso Core.GetDouble(dr("TotalDue")) > 0) OrElse Core.GetBoolean(dv("IsFlaggedForRebates")), "Flagged for Past Due Rebates", ""), ""), _
                    '                                 .VendorRatingFlag = IIf(Not Core.GetBoolean(dv("IsOverwrite")), IIf(Core.GetBoolean(dv("IsFlaggedForRatings")), "Flagged for Low Vendor Rating", ""), ""), _
                    '                                 .VendorId = dv("VendorId"), _
                    '                                 .IsFlaggedForRebates = dv("IsFlaggedForRebates"), _
                    '                                 .IsFlaggedForRatings = dv("IsFlaggedForRatings"), _
                    '                                 .CompanyName = dv("CompanyName"), _
                    '                                 .InvoiceDate = dr("InvoiceDate") _
                    '                                 } _
                    '                            )

                Else
                    ltlVendors.Text = "There are no vendors in the selected supply phase(s)."
                End If
            Else
                ltlVendors.Text = "Please select supply phases to add vendors to this bid request."
            End If
        Else
            ltlVendors.Text = "Your account is not associated to a market. You will not be able to start the bidding process. Please contact the system administrator."
        End If
        rptVendors.DataBind()
    End Sub

    Private Function LoadAccount() As DataTable
        AccDB.Open(DBConnectionString.GetConnectionString(AppSettings("AccountingConnectionString"), AppSettings("AccountingConnectionStringUsername"), AppSettings("AccountingConnectionStringPassword")))
        SQL = _
                " select " _
                & "     VNDRID as VendorHistoricID," _
                & "     MAX(RBATEINVDATE) as InvoiceDate," _
                & "     SUM(COALESCE(REBATEAMT,0) + COALESCE(CUSTRBATAMT,0) + COALESCE(RBATADJAMT,0)) - SUM(COALESCE(RBATPAID,0)) as TotalDue " _
                & " from " _
                & "     cbusa_BuilderRebates" _
                & " where " _
                & "     LLCID=(SELECT TOP 1 STRING1 FROM VBCloud_Param WHERE CODE=1 AND LONGINT1=" & DB.Number(dbBuilder.LLCID) & ")" _
                & " group by " _
                & "     VNDRID"

        Return AccDB.GetDataTable(SQL)

    End Function

    Protected Sub lsSupplyPhases_SelectedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lsSupplyPhases.SelectedChanged
        'log  Vendor Supply phases Change
        Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Select Vendor from Supply phases", "", "", "", "", UserName)
        'end log
        LoadVendors()
    End Sub

    Protected Sub BindDocuments()

        Dim SQL As String = String.Empty
        Dim dt As DataTable

        SQL = "Select * From POBuilderDocument Where BuilderDocumentId In (Select BuilderDocumentId From POQuoteBuilderDocument Where QuoteId = " & DB.Number(QuoteId) & ")"

        If SQL <> String.Empty Then
            dt = DB.GetDataTable(SQL)

            Me.rptDocuments.DataSource = dt
            Me.rptDocuments.DataBind()

            If dt.Rows.Count > 0 Then
                divNoCurrentDocuments.Visible = False
            End If
        End If
    End Sub

    Protected Sub rptDocuments_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptDocuments.ItemDataBound

        If Not e.Item.ItemType = ListItemType.Item And Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim lnkMessageTitle As HtmlAnchor = e.Item.FindControl("lnkMessageTitle")

        Dim lnkDelete As ConfirmImageButton = e.Item.FindControl("lnkDelete")
        lnkDelete.CommandArgument = e.Item.DataItem("BuilderDocumentId")

        lnkMessageTitle.HRef = "/assets/plansonline/builderdocument/" & e.Item.DataItem("FileName").ToString

    End Sub

    Protected Sub rptDocuments_ItemCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptDocuments.ItemCommand
        Select Case e.CommandName
            Case Is = "Remove"
                Try
                    DB.BeginTransaction()
                    Dim FileInfo As System.IO.FileInfo
                    Try
                        FileInfo = New System.IO.FileInfo(Server.MapPath("/assets/plansonline/builderdocument/" & POBuilderDocumentRow.GetRow(DB, e.CommandArgument).FileName))
                        FileInfo.Delete()
                    Catch ex As Exception

                    End Try
                    POBuilderDocumentRow.RemoveRow(DB, e.CommandArgument)
                    DB.CommitTransaction()
                    BindDocuments()
                Catch ex As SqlException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    AddError(ErrHandler.ErrorText(ex))
                End Try
        End Select
    End Sub

    Protected Function ValidatePage() As Boolean
        If Not IsValid Then Return False
        Return True
    End Function
    Protected Function ValidateVendors() As Boolean
        If dtVendors Is Nothing OrElse dtVendors.Rows.Count = 0 Then
            AddError("Please select supply phases to add vendors to this bid request.")
            Return False
        End If
        Return True
    End Function
    Protected Function SaveChanges() As Boolean
        Try
            Dim bIsNew As Boolean = False

            DB.BeginTransaction()

            If QuoteId <> 0 Then
                dbQuote = POQuoteRow.GetRow(DB, QuoteId)
            Else
                dbQuote = New POQuoteRow(DB)
                dbQuote.StatusDate = Now()
                dbQuote.Status = "New"
                bIsNew = True
            End If
            dbQuote.Title = txtTitle.Text
            dbQuote.Instructions = txtInstructions.Text
            dbQuote.Deadline = dtDeadline.Value
            dbQuote.ProjectId = drpProjectId.SelectedValue

            If QuoteId <> 0 Then
                dbQuote.Update()
                'log  Update BidRequest
                Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Update BidRequest", QuoteId, "", "", "", UserName)
                'end log
            Else
                QuoteId = dbQuote.Insert
                'log  Add BidRequest
                Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Add New BidRequest", QuoteId, "", "", "", UserName)
                'end log
            End If
            dbQuote.DeleteFromAllVendorCategories()
            dbQuote.InsertToVendorCategories(lsSupplyPhases.SelectedValues)

            If Session("VendorIds") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Session("VendorIds").ToString().Trim()) Then
                'dbQuote.DeleteFromAllPOQuoteVendors()
                dbQuote.InsertToPOQuoteVendors(Session("VendorIds"))
            End If

            DB.CommitTransaction()

            'Dim dtQuoteVendors As DataTable = DB.GetDataTable("SELECT * FROM POQuoteVendor WHERE (IsEmailSent=0 OR IsEmailSent IS NULL) And QuoteId=" & QuoteId)

            'If (dtQuoteVendors.Rows.Count > 0) Then
            '    For Each row As DataRow In dtQuoteVendors.Rows
            '        Try
            '            Dim dbVendor As VendorRow = VendorRow.GetRow(DB, CType(row("VendorId"), Integer))

            '            Dim fromName As String = SysParam.GetValue(DB, "ContactUsName")
            '            Dim fromEmail As String = SysParam.GetValue(DB, "ContactUsEmail")
            '            Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "VendorFlagEmail")
            '            Dim sBody As String = FormatMessage(dbMsg.Message)
            '            dbMsg.Send(dbVendor, sBody, True)

            '            If dbMsg.CCList <> String.Empty Then
            '                Dim aEmails() As String = dbMsg.CCList.Split(",")
            '                For Each email As String In aEmails
            '                    Core.SendSimpleMail(fromEmail, fromName, email, email, dbMsg.Subject, sBody)
            '                Next
            '            End If

            '            DB.BeginTransaction()
            '            DB.ExecuteSQL("UPDATE POQuoteVendor SET IsEmailSent=1 WHERE QuoteId=" & QuoteId & " AND VendorId=" & dbVendor.VendorID)
            '            DB.CommitTransaction()
            '        Catch ex As Exception
            '            DB.RollbackTransaction()
            '        End Try
            '    Next
            'End If

            If bIsNew Then Response.Redirect("/builder/plansonline/editquote.aspx?QuoteId=" & QuoteId & "&" & GetPageParams(FilterFieldType.All))


            Return True

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
            Return False
        End Try
    End Function

    Private Function GetVendorIds(ByVal dtVendors As DataTable) As String
        Dim sVendorIds As String = String.Empty
        Dim sConn As String = ""
        For Each dr As DataRow In dtVendors.Rows
            sVendorIds &= sConn & dr("VendorId")
            sConn = ", "
        Next
        Return sVendorIds
    End Function

    Private Sub SendCancellation()
        Dim Sql As String = "Select QuoteRequestId, VendorId From POQuoteRequest Where QuoteId = " & DB.Number(QuoteId) & " And RequestStatus In ('New', 'Request Information')"
        Dim dt As DataTable = DB.GetDataTable(Sql)
        If dt.Rows.Count > 0 Then
            Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "QuoteRequestCancelled")
            Dim sBody As String = FormatMessage(dbMsg.Message)

            For Each dr As DataRow In dt.Rows
                Try
                    DB.BeginTransaction()
                    Sql = "Update POQuoteRequest Set RequestStatus = 'Cancelled' Where QuoteRequestId = " & DB.Number(dr("QuoteRequestId"))
                    DB.ExecuteSQL(Sql)
                    DB.CommitTransaction()
                    'send cencellation message to vendor
                    Dim sFinalBody = sBody.Replace("%%QuoteRequestUrl%%", GlobalSecureName & "/vendor/plansonline/quoterequestmessages.aspx?QuoteRequestId=" & dr("QuoteRequestId"))
                    dbMsg.Send(VendorRow.GetRow(DB, dr("VendorId")), sFinalBody, True)
                Catch ex As SqlException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                End Try
            Next
        End If
    End Sub

    Private Function GetQuoteDetailsBody() As String
        Dim sBody As String = "Quote #: " & dbQuote.QuoteId & vbCrLf

        sBody &= "Title: " & dbQuote.Title & vbCrLf
        sBody &= "Deadline: " & dbQuote.Deadline & vbCrLf
        sBody &= "Status: " & dbQuote.Status & " (" & dbQuote.StatusDate & ")" & vbCrLf
        sBody &= "Project: " & dbProject.ProjectName & vbCrLf
        sBody &= "Project Status: " & ProjectStatusRow.GetRow(DB, dbProject.ProjectStatusID).ProjectStatus & vbCrLf
        sBody &= "Subdivision: " & dbProject.Subdivision & vbCrLf
        sBody &= "Lot #: " & dbProject.LotNumber & vbCrLf
        sBody &= "City: " & dbProject.City & vbCrLf
        sBody &= "State: " & dbProject.State & vbCrLf

        Dim dt As DataTable = DB.GetDataTable("SELECT qvc.QuoteId, vc.Category FROM POQuoteVendorCategory qvc INNER JOIN VendorCategory vc ON vc.VendorCategoryId = qvc.VendorCategoryId where qvc.QuoteId = " & DB.Number(dbQuote.QuoteId))
        If dt.Rows.Count > 0 Then
            Dim Conn As String = String.Empty
            Dim Result As String = String.Empty
            For Each item As DataRow In dt.Rows
                Result &= Conn & item("Category")
                Conn = ","
            Next
            sBody &= "Phases: " & Result
        End If
        Return sBody

        Return sBody

    End Function

    Private Function FormatMessage(ByVal Msg As String) As String

        Msg = Msg.Replace("%%Builder%%", dbBuilder.CompanyName)
        Msg = Msg.Replace("%%QuoteDetails%%", GetQuoteDetailsBody())

        Return Msg
    End Function

    Private Sub UpdateQuoteRequests(ByVal dtVendors As DataTable, ByVal bSendUpdate As Boolean)
        Dim Sql As String = String.Empty
        Dim dt As DataTable = Nothing
        Dim dtDeletedQuoteVendor As DataTable = Nothing
        Dim dbUpdateRequestMsg As AutomaticMessagesRow = Nothing
        Dim sUpdateBody As String = String.Empty

        Dim dbNewRequestMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "NewQuoteRequest")
        Dim sNewBody As String = FormatMessage(dbNewRequestMsg.Message)

        If bSendUpdate Then
            dbUpdateRequestMsg = AutomaticMessagesRow.GetRowByTitle(DB, "QuoteRequestUpdate")
            sUpdateBody = FormatMessage(dbUpdateRequestMsg.Message)
        End If
        Sql = "Select QuoteId, VendorId From POQuoteVendor Where QuoteId = " & DB.Number(QuoteId)
        dtDeletedQuoteVendor = DB.GetDataTable(Sql)

        For Each dr As DataRow In dtVendors.Rows
            If (dtDeletedQuoteVendor.AsEnumerable.Where(Function(row) row("VendorId") = dr("VendorId")).Count > 0) Then
                Sql = "Delete From POQuoteRequest WHERE QuoteId = " & DB.Number(QuoteId) & " And VendorId = " & DB.Number(dr("VendorId"))
                DB.ExecuteSQL(Sql)
                Continue For
            End If

            Sql = "Select QuoteRequestId, RequestStatus From POQuoteRequest Where QuoteId = " & DB.Number(QuoteId) & " And VendorId = " & DB.Number(dr("VendorId"))
            dt = DB.GetDataTable(Sql)
            If dt.Rows.Count = 0 Then
                Try
                    DB.BeginTransaction()
                    'Add quote requests for new vendor
                    Sql = "Insert Into POQuoteRequest (QuoteId, VendorId, RequestStatus, CreateDate, ModifyDate) Values (" & DB.Number(QuoteId) & ", " & DB.Number(dr("VendorId")) & ", 'New', " & DB.Quote(Now) & ", " & DB.Quote(Now) & ")"
                    Dim QuoteRequestId As Integer = DB.InsertSQL(Sql)
                    DB.CommitTransaction()
                    'Send New Quote Request message to vendor
                    Dim sFinalBody = sNewBody.Replace("%%QuoteRequestUrl%%", GlobalSecureName & "/vendor/plansonline/quoterequestmessages.aspx?QuoteRequestId=" & QuoteRequestId)
                    dbNewRequestMsg.Send(VendorRow.GetRow(DB, dr("VendorId")), sFinalBody, True)
                Catch ex As SqlException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                End Try
            ElseIf bSendUpdate Then
                'Send Quote Update message to vendor not Decline by Vendor
                If dt.Rows(0)("RequestStatus") = "New" Or dt.Rows(0)("RequestStatus") = "Request Information" Then
                    Dim sFinalBody = sUpdateBody.Replace("%%QuoteRequestUrl%%", GlobalSecureName & "/vendor/plansonline/quoterequestmessages.aspx?QuoteRequestId=" & dt.Rows(0)("QuoteRequestId"))
                    dbUpdateRequestMsg.Send(VendorRow.GetRow(DB, dr("VendorId")), sFinalBody, True)
                End If
            End If
        Next

        Dim sVendorIds As String = GetVendorIds(dtVendors)

        'Vendors exisitng market


        Sql = "Select QuoteRequestId, VendorId, VendorContactEmail From POQuoteRequest Where QuoteId = " & DB.Number(QuoteId) & " And RequestStatus <> 'Exited Market'"
        If dtVendors.Rows.Count > 0 Then
            Sql &= " And VendorId Not In " & DB.QuoteMultiple(sVendorIds)
        End If

        dt = DB.GetDataTable(Sql)
        If dt.Rows.Count > 0 Then
            Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "QuoteRequestExitedMarket")
            Dim sBody As String = FormatMessage(dbMsg.Message)
            For Each dr As DataRow In dt.Rows
                Try
                    DB.BeginTransaction()
                    Sql = "Update POQuoteRequest Set RequestStatus = 'Exited Market' Where QuoteRequestId = " & DB.Number(dr("QuoteRequestId"))
                    DB.ExecuteSQL(Sql)
                    DB.CommitTransaction()
                    'send Exited Market message to vendor
                    Dim sFinalBody = sBody.Replace("%%QuoteRequestUrl%%", GlobalSecureName & "/vendor/plansonline/quoterequestmessages.aspx?QuoteRequestId=" & dr("QuoteRequestId"))
                    dbMsg.Send(VendorRow.GetRow(DB, dr("VendorId")), sFinalBody, True)
                Catch ex As SqlException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                End Try
            Next
        End If

        If dtVendors.Rows.Count > 0 Then
            'Vendors re-entering market
            Sql = "Select QuoteRequestId, VendorId From POQuoteRequest Where QuoteId = " & DB.Number(QuoteId) & " And RequestStatus = 'Exited Market' And VendorId In " & DB.QuoteMultiple(sVendorIds)
            dt = DB.GetDataTable(Sql)
            If dt.Rows.Count > 0 Then
                Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "QuoteRequestReenteredMarket")
                Dim sBody As String = FormatMessage(dbMsg.Message)
                For Each dr As DataRow In dt.Rows
                    Try
                        DB.BeginTransaction()
                        Sql = "Update POQuoteRequest Set RequestStatus = 'New' Where QuoteRequestId = " & DB.Number(dr("QuoteRequestId"))
                        DB.ExecuteSQL(Sql)
                        DB.CommitTransaction()
                        'send Re-enter Market message to vendor
                        Dim sFinalBody = sBody.Replace("%%QuoteRequestUrl%%", GlobalSecureName & "/vendor/plansonline/quoterequestmessages.aspx?QuoteRequestId=" & dr("QuoteRequestId"))
                        dbMsg.Send(VendorRow.GetRow(DB, dr("VendorId")), sFinalBody, True)
                    Catch ex As SqlException
                        If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    End Try
                Next
            End If
        End If

        'Resume bid
        If dbQuote.Status = "Bidding In Progress" Then
            Sql = "Select QuoteRequestId, VendorId From POQuoteRequest Where QuoteId = " & DB.Number(QuoteId) & " And RequestStatus In ('Cancelled', 'Awarded')"
            dt = DB.GetDataTable(Sql)
            For Each dr As DataRow In dt.Rows
                Try
                    DB.BeginTransaction()
                    Sql = "Update POQuoteRequest Set RequestStatus = 'New' Where QuoteRequestId = " & DB.Number(dr("QuoteRequestId"))
                    DB.ExecuteSQL(Sql)
                    DB.CommitTransaction()
                    'send Quote Update Market message to vendor
                    Dim sFinalBody = sNewBody.Replace("%%QuoteRequestUrl%%", GlobalSecureName & "/vendor/plansonline/quoterequestmessages.aspx?QuoteRequestId=" & dr("QuoteRequestId"))
                    dbNewRequestMsg.Send(VendorRow.GetRow(DB, dr("VendorId")), sFinalBody, True)
                Catch ex As SqlException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                End Try
            Next
        End If

    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not ValidatePage() Then Exit Sub
        If Not SaveChanges() Then Exit Sub
        dbQuote = POQuoteRow.GetRow(DB, QuoteId)
        If dbQuote.Status = "Bidding In Progress" Then
            LoadVendors()
            UpdateQuoteRequests(dtVendors, False)
            Session("BidMsg") = "Bid Request details saved with success. Email notifications were sent to only those vendors that have exited or re-entered the market or selected supply phases."
        Else
            Session("BidMsg") = "Bid Request details saved with success. No email notifications were sent."
        End If
        Response.Redirect("quotes.aspx?" & GetPageParams(FilterFieldType.All))
        divMsg.Visible = True
        LoadFromDB()
    End Sub

    Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If Not ValidatePage() Then Exit Sub
        If Not SaveChanges() Then Exit Sub
        LoadVendors()
        UpdateQuoteRequests(dtVendors, True)
        Session("BidMsg") = "Bid Request details saved with success. Bid Request Update email notifications were sent to all vendors."
        Response.Redirect("quotes.aspx?" & GetPageParams(FilterFieldType.All))
        divMsg.Visible = True
        LoadFromDB()
    End Sub

    Protected Sub btnStartBid_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStartBid.Click
        If Not ValidatePage() Then Exit Sub
        LoadVendors()
        If Not ValidateVendors() Then Exit Sub
        If Not SaveChanges() Then Exit Sub
        Try
            DB.BeginTransaction()

            dbQuote = POQuoteRow.GetRow(DB, QuoteId)

            Dim PreviousStatus As String = dbQuote.Status

            dbQuote.Status = "Bidding In Progress"
            dbQuote.StatusDate = Now()
            dbQuote.Update()
            'log  Send BidRequest
            Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Send BidRequest", QuoteId, "", "", "", UserName)
            'end log

            DB.CommitTransaction()

            UpdateQuoteRequests(dtVendors, True)

            Session("BidMsg") = "Bid Request details saved with success. Bid Request email notifications were sent to all vendors."
            Response.Redirect("quotes.aspx?" & GetPageParams(FilterFieldType.All))
            divMsg.Visible = True
            LoadFromDB()
        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnStopBid_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStopBid.Click
        If Not ValidatePage() Then Exit Sub
        If Not SaveChanges() Then Exit Sub
        Try
            DB.BeginTransaction()

            dbQuote = POQuoteRow.GetRow(DB, QuoteId)

            dbQuote.Status = "Cancelled"
            dbQuote.StatusDate = Now()
            dbQuote.Update()
            'log  Cancel BidRequest
            Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Cancel BidRequest", QuoteId, "", "", "", UserName)
            'end log

            DB.CommitTransaction()

            SendCancellation()

            Session("BidMsg") = "Bid Request details saved with success. Bid Request Cancellation email notifications were sent to all vendors."
            Response.Redirect("quotes.aspx?" & GetPageParams(FilterFieldType.All))
            divMsg.Visible = True
            LoadFromDB()
        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try

    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        If Redir <> String.Empty Then
            Response.Redirect(Redir)
        Else
            'log  Cancel Bid Request
            Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Btn Cancel Bid request", QuoteId, "", "", "", UserName)
            'end log
            Response.Redirect("quotes.aspx?" & GetPageParams(FilterFieldType.All))
        End If
    End Sub


    Protected Sub btnBack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.Click
        Response.Redirect(Redir)
    End Sub

    Protected Sub btnTransferDocs_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTransferDocs.Click
        'log  Transfer uploaded document
        Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Transfer uploaded document", QuoteId, "", "", "", UserName)
        'end log
        BindDocuments()
    End Sub



    Protected Sub rptVendors_ItemCommand(source As Object, e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptVendors.ItemCommand

        If e.CommandName = "VendorDelete" Then
            If Session("VendorIds") Is Nothing Then
                Session("VendorIds") = e.CommandArgument.ToString()
            Else
                If Not Session("VendorIds").ToString.Contains(e.CommandArgument.ToString()) Then
                    Session("VendorIds") = Session("VendorIds").ToString() & "," & e.CommandArgument.ToString()
                End If
            End If

            e.Item.FindControl("lnkVendorReinstate").Visible = True
            CType(e.Item.FindControl("lnkVendorReinstateText"), Literal).Text = "<div style=""padding-top:8px; width:200px;"">Removed From Bid Process</div>"
            e.Item.FindControl("lnkVendorDelete").Visible = False

        ElseIf e.CommandName = "VendorReinstate" Then
            If Session("VendorIds") Is Nothing Then
                ' Do nothing
            Else
                If Session("VendorIds").ToString.Contains(e.CommandArgument.ToString()) Then
                    Dim Ids() As String = Session("VendorIds").ToString.Split(",")
                    Dim i = (From id In Ids Where id <> e.CommandArgument.ToString)
                    For count As Integer = 0 To count - 1 Step 1
                        Session("VendorIds") = i(count) & IIf(count = i.Count - 1, "", ",")
                    Next
                End If
            End If
            If QuoteId > 0 Then
                Dim ReinstatedVendors As Integer = DB.ExecuteScalar("DELETE From POQuoteVendor WHERE VendorId = " & e.CommandArgument & " AND QuoteId=" & QuoteId)

                SQL = "Select QuoteRequestId, RequestStatus From POQuoteRequest Where QuoteId = " & DB.Number(QuoteId) & " And VendorId = " & DB.Number(e.CommandArgument)
                Dim dtReinstatedVendor As DataTable = DB.GetDataTable(SQL)
                Dim dbNewRequestMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "NewQuoteRequest")
                Dim sReinstateBody As String = FormatMessage(dbNewRequestMsg.Message)

                If dbQuote.Status = "Bidding In Progress" Then
                    If dtReinstatedVendor.Rows.Count = 0 Then
                        Try
                            DB.BeginTransaction()
                            'Add quote requests for new vendor
                            SQL = "Insert Into POQuoteRequest (QuoteId, VendorId, RequestStatus, CreateDate, ModifyDate) Values (" & DB.Number(QuoteId) & ", " & DB.Number(e.CommandArgument) & ", 'New', " & DB.Quote(Now) & ", " & DB.Quote(Now) & ")"
                            Dim QuoteRequestId As Integer = DB.InsertSQL(SQL)
                            DB.CommitTransaction()
                            'Send New Quote Request message to vendor
                            Dim sFinalBody = sReinstateBody.Replace("%%QuoteRequestUrl%%", GlobalSecureName & "/vendor/plansonline/quoterequestmessages.aspx?QuoteRequestId=" & QuoteRequestId)
                            dbNewRequestMsg.Send(VendorRow.GetRow(DB, e.CommandArgument), sFinalBody, True)
                            Response.Redirect("quotes.aspx?" & GetPageParams(FilterFieldType.All), True)
                        Catch ex As SqlException
                            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                        End Try
                    End If
                End If

            End If

            e.Item.FindControl("lnkVendorReinstate").Visible = False
            e.Item.FindControl("lnkVendorDelete").Visible = True
            CType(e.Item.FindControl("lnkVendorReinstateText"), Literal).Text = ""

            If QuoteId > 0 Then
                If dbQuote.Status = "Bidding In Progress" Then
                    If IncludedVendorsInBid(DB, QuoteId).ContainsKey(e.CommandArgument) Then
                        e.Item.FindControl("lnkVendorDelete").Visible = False
                    End If
                End If
            End If

        End If
    End Sub

    Public Function IncludedVendorsInBid(ByVal db As Database, ByVal QuoteID As Integer) As Dictionary(Of String, Integer)
        Dim dict As New Dictionary(Of String, Integer)
        Dim SQL As String = "SELECT QuoteID, VendorID FROM POQuoteRequest WHERE QUOTEID = " & QuoteID
        Using dr As SqlDataReader = DB.GetReader(SQL)
            While dr.Read
                dict.Add(dr("VendorID"), dr("QuoteID"))
            End While
        End Using

        Return dict
    End Function


    Protected Sub rptVendors_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptVendors.ItemDataBound

        If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then
            Dim lnkVendorReinstate As LinkButton = CType(e.Item.FindControl("lnkVendorReinstate"), LinkButton)
            Dim lnkVendorDelete As LinkButton = CType(e.Item.FindControl("lnkVendorDelete"), LinkButton)
            Dim lnkVendorReinstateText As Literal = CType(e.Item.FindControl("lnkVendorReinstateText"), Literal)

            Dim dt As DataTable = DB.GetDataTable("SELECT VendorId From POQuoteVendor WHERE VendorId = " & lnkVendorReinstate.CommandArgument & " AND QuoteId=" & QuoteId)

            If dt.Rows.Count > 0 Then
                lnkVendorDelete.Visible = False
                lnkVendorReinstate.Visible = True
                lnkVendorReinstateText.Text = "Removed From Bid Process"
            End If

            If Not AllowExcludingVendors Then
                lnkVendorDelete.Visible = False
                lnkVendorReinstate.Visible = False
            End If

            If QuoteId > 0 Then
                If dbQuote.Status = "Bidding In Progress" Then
                    If IncludedVendorsInBid(DB, QuoteId).ContainsKey(e.Item.DataItem("VendorID")) Then
                        lnkVendorDelete.Visible = False
                    End If
                End If
            End If


        End If

    End Sub
End Class

