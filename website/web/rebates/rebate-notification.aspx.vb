Imports Components
Imports DataLayer
Imports PopupForm
Imports System.Linq
Imports System.Data
Imports System.Web.Services
Imports System.Configuration.ConfigurationManager

Partial Class rebate_notification
    Inherits SitePage
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Protected sHistoricId As String = "0"
    Protected sBuilderAccountEmail, sBuilderName, excludedVendors As String
    Private ResDb As New Database

    Protected dbbuilder As BuilderRow
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        EnsureBuilderAccess()

        sBuilderAccountEmail = DB.ExecuteScalar("SELECT Email FROM BuilderAccount WHERE BuilderAccountId=" & Session("BuilderAccountID"))
        sHistoricId = DB.ExecuteScalar("SELECT historicId FROM Builder WHERE builderId=" & Session("BuilderId"))
        sBuilderName = DB.ExecuteScalar("SELECT CompanyName FROM Builder WHERE builderId=" & Session("BuilderId"))
        excludedVendors = DB.ExecuteScalar("SELECT coalesce( stuff((    SELECT ', ' + cast(HistoricID as varchar(max))    FROM Vendor where excludedVendor = 1    FOR XML PATH('')    ), 1, 1, ''),'')")



        If Not Request("c") = String.Empty Then
            ltlmsgSend.Visible = True
        End If
        If Not IsPostBack Then
            BindVendors()
            BindPreferences()
        End If

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")
        Core.DataLog("Past Due Rebate Report", PageURL, CurrentUserId, "Left Menu Click", "", "", "", "", UserName)

    End Sub

    Private Sub BindPreferences()
        dbbuilder = BuilderRow.GetRow(DB, Session("BuilderId"))
        RblEmailPreference.SelectedValue = dbbuilder.RebatesEmailPreferences
    End Sub

    Private Sub BindVendors()
        Dim sVendors As New System.Text.StringBuilder
        sVendors.Append("SELECT DISTINCT([vndrid]) AS vndrid,vndrname FROM RG_ARReport " & vbCrLf)
        sVendors.Append("WHERE DaysPastDue >= 30 AND  bldrid =  " & sHistoricId & vbCrLf)
        If Not excludedVendors = String.Empty Then
            sVendors.Append("and vndrid not in ( " & excludedVendors & " ) " & vbCrLf)
        End If
        sVendors.Append(" ORDER BY vndrid")

        Dim dt As DataTable

        dt = GetDataTableFromAccounting(sVendors.ToString())
        If (dt.Rows.Count > 0) Then
            NoRecords.Visible = False
            rptVendors.DataSource = dt
            rptVendors.DataBind()
        Else
            NoRecords.Visible = True
            divbtn.Visible = False
            btnSubmit.Visible = False
        End If


    End Sub

    Private Function GetDataTableFromAccounting(ByVal sQuery As String) As DataTable

        Try
            ResDb.Open(DBConnectionString.GetConnectionString(AppSettings("ResgroupConnectionString"), AppSettings("ResgroupConnectionStringUsername"), AppSettings("ResgroupConnectionStringPassword")))
            Return ResDb.GetDataTable(sQuery)
        Catch ex As Exception
            AddError(ErrHandler.ErrorText(ex))
            Return New DataTable
        Finally
            ResDb.Close()

        End Try
        Return New DataTable
    End Function

    Protected Sub rptVendors_ItemCommand(source As Object, e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptVendors.ItemCommand
        If e.CommandName = "SendNotice" Then
            Dim sVendorId = e.CommandArgument.ToString.Trim
            Dim Url As String = IIf(AppSettings("GlobalSecureName") = Nothing, AppSettings("GlobalRefererName"), AppSettings("GlobalSecureName")) & "/Email/VendorRebateNotice.aspx?HistoricBuilderId=" & sHistoricId & "&VendorId=" & sVendorId

            Dim sMsg As String = Core.GetRenderedHtml(Url)
            ' sMsg = Replace(sMsg, "%%BUILDER_COMPANY%%", sBuilderCompany)

            Dim sSubject As String = "Past Due CBUSA Rebates"

            Dim sVendorIds = DB.ExecuteScalar("SELECT vendorid FROM Vendor WHERE historicId=" & sVendorId)
            'Dim sVendorEmails As String = DB.ExecuteScalar("select CAST(stuff((SELECT ',' + t1.Email FROM VendorAccount t1 WHERE t1.VendorID = t2.VendorID AND t1.IsActive = 1 FOR XML PATH('')),1,1,'') as varchar(max)) + ', ' + t2.Email from Vendor t2 where t2.VendorID = " & sVendorIds)
            'Dim sVendorEmails As String = DB.ExecuteScalar("select CAST(stuff((SELECT ',' + t1.Email FROM VendorAccount t1 join VendorAccountVendorRole t3 on t1.VendorAccountid= t3.vendoraccountid WHERE t1.VendorID = t2.VendorID AND t1.IsActive = 1 and t3.VendorRoleid in (1,5) FOR XML PATH('')),1,1,'') as varchar(max)) + ', ' + COALESCE(t2.Email, 'customerservice@cbusa.us') from Vendor t2 where t2.VendorID =  " & sVendorIds)
            Dim sVendorEmails As String = DB.ExecuteScalar("SELECT COALESCE(STUFF((SELECT ',' + VendorEmail.Email FROM(SELECT DISTINCT VA.Email FROM VendorAccount VA JOIN VendorAccountVendorRole VAVR ON VA.VendorAccountID = VAVR.VendorAccountID WHERE VA.VendorID = " & sVendorIds & " AND VA.IsActive = 1 AND VAVR.VendorRoleID IN (1, 5)) AS VendorEmail FOR XML PATH('')),1,1,''), 'NA') VendorEmails")

            Dim llcBuilderRow As LLCRow = LLCRow.GetBuilderLLC(DB, Session("BuilderId"))
            Dim addr As String = SysParam.GetValue(DB, "RebateNotificationList")
            Dim addrCC As String = SysParam.GetValue(DB, "RebateNotificationListCC")

            If sVendorEmails <> "NA" Then
                Dim arr As String() = Split(sVendorEmails, ",")
                Dim iCounter As Integer = 0

                For Each sEmail As String In arr
                    'Response.Write(sEmail)
                    If iCounter = 0 Then
                        Core.SendHTMLMail(sBuilderAccountEmail, sBuilderName, sEmail, sEmail, sSubject, sMsg, llcBuilderRow.NotificationEmailList & IIf(addrCC <> String.Empty, "," & addrCC, ""), addr)
                    Else
                        Core.SendHTMLMail(sBuilderAccountEmail, sBuilderName, sEmail, sEmail, sSubject, sMsg)
                    End If

                    iCounter = iCounter + 1
                Next
            End If

            'Response.End()
            'Core.SendHTMLMailToMultipleRecipient(sBuilderAccountEmail, sBuilderName, sVendorEmails, sVendorEmails, sSubject, sMsg, llcBuilderRow.NotificationEmailList & IIf(addrCC <> String.Empty, "," & addrCC, ""), addr)

            Response.Redirect("Rebate-Notification.aspx?c=y")
        End If

    End Sub


    Protected Sub rptVendors_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptVendors.ItemDataBound

        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then

            Dim sVendorId As String = CType(e.Item.FindControl("litVendorId"), Literal).Text.Trim()
            Dim rptReport As Repeater = CType(e.Item.FindControl("rptReport"), Repeater)
            Dim sVendors As New System.Text.StringBuilder
            sVendors.Append("SELECT vndrid,[vndrname], " & vbCrLf)
            sVendors.Append("       [invoice], " & vbCrLf)
            sVendors.Append("       [period] + ' - ' + [year] AS [Qtr/Yr], " & vbCrLf)
            sVendors.Append("       [date], " & vbCrLf)
            sVendors.Append("       [period], " & vbCrLf)
            sVendors.Append("       [year], " & vbCrLf)
            sVendors.Append("       [dayspastdue], " & vbCrLf)
            sVendors.Append("       [purchvol], " & vbCrLf)
            sVendors.Append("       [rebaterate], " & vbCrLf)
            sVendors.Append("       [amountdue] " & vbCrLf)
            sVendors.Append("FROM   [RG_ARReport] " & vbCrLf)
            sVendors.Append("WHERE DaysPastDue >= 30 AND bldrid =  " & sHistoricId & " AND vndrid = " & sVendorId & " ORDER BY vndrid")

            Dim dtVendors As DataTable = GetDataTableFromAccounting(sVendors.ToString())
            'Dim dtRebateNoticeVendors As DataTable = DB.GetDataTable(" SELECT * FROM RebateEmailSent WHERE  bldrid =  " & sHistoricId & " AND vndrid = " & sVendorId & " ORDER BY vndrid")
            'dtRebateNoticeVendors.Merge(
            'Dim l = From dr As DataRow In dtVendors Join 
            AddHandler rptReport.ItemDataBound, AddressOf rptReport_ItemDataBound
            rptReport.DataSource = dtVendors
            rptReport.DataBind()

            Dim Url As String = IIf(AppSettings("GlobalSecureName") = Nothing, AppSettings("GlobalRefererName"), AppSettings("GlobalSecureName")) & "/Email/VendorRebateNotice.aspx?HistoricBuilderId=" & sHistoricId & "&VendorId=" & sVendorId & "&isPreview=y"

            Dim lnkNoticeType As HyperLink = CType(e.Item.FindControl("lnkNoticeType"), HyperLink)
            lnkNoticeType.Target = "_blank"
            lnkNoticeType.NavigateUrl = Url
        End If

    End Sub

    Protected Sub rptReport_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim litSubmittedDate As Literal = CType(e.Item.FindControl("litSubmittedDate"), Literal)
            Dim litRebateAmount As Literal = CType(e.Item.FindControl("litRebateAmount"), Literal)
            Dim sVendorId As String = "0"
            Dim sInvoiceId As String = "0"
            If e.Item.DataItem IsNot Nothing Then
                sVendorId = CType(e.Item.DataItem, DataRowView)("vndrid")
                sInvoiceId = CType(e.Item.DataItem, DataRowView)("invoice")
            End If
            Try
                ResDb.Open(DBConnectionString.GetConnectionString(AppSettings("ResgroupConnectionString"), AppSettings("ResgroupConnectionStringUsername"), AppSettings("ResgroupConnectionStringPassword")))
                Dim RebateAmount As String = ResDb.ExecuteScalar("select SUM(AmountDue) AS RebateAmount from RG_ARReport  where Invoice = " & DB.Quote(sInvoiceId) & " GROUP BY VNDRID,vndrname ,INVOICE,PERIOD,YEAR,DATE,DaysPastDue")
                litRebateAmount.Text = FormatCurrency(RebateAmount, 2)
            Catch ex As Exception
                AddError(ErrHandler.ErrorText(ex))
            Finally
                ResDb.Close()
            End Try

            If litSubmittedDate IsNot Nothing Then


                Dim submittedDate As Object = DB.ExecuteScalar("select Max(SubmittedDate) AS SubmittedDate from RebateEmailSent where bldrid =  " & sHistoricId & " AND IsAutoEmailTask <> 1 AND VNDRID = " & sVendorId & " AND Invoice = " & DB.Quote(sInvoiceId))

                If Not IsDBNull(submittedDate) And submittedDate IsNot Nothing Then
                    litSubmittedDate.Text = submittedDate.ToShortDateString
                End If
            End If
        End If
    End Sub


    Protected Sub btnSubmit_Click(sender As Object, e As System.EventArgs) Handles btnSubmit.Click, btnSubmitTop.Click


        For Each item As RepeaterItem In rptVendors.Items
            Dim chkSelect As CheckBox = CType(item.FindControl("chkSelect"), CheckBox)
            If chkSelect.Checked Then

                Dim sVendorId As String = CType(item.FindControl("litVendorId"), Literal).Text.Trim
                Dim Url As String = IIf(AppSettings("GlobalSecureName") = Nothing, AppSettings("GlobalRefererName"), AppSettings("GlobalSecureName")) & "/Email/VendorRebateNotice.aspx?HistoricBuilderId=" & sHistoricId & "&VendorId=" & sVendorId
                Dim sMsg As String = Core.GetRenderedHtml(Url)
                'sMsg = Replace(sMsg, "%%BUILDER_COMPANY%%", sBuilderCompany)

                Dim sSubject As String = "Past Due CBUSA Rebates"


                Dim sVendorIds = DB.ExecuteScalar("SELECT vendorid FROM Vendor WHERE historicId=" & sVendorId)
                'Dim sVendorEmails As String = DB.ExecuteScalar("select CAST(stuff((SELECT ',' + t1.Email FROM VendorAccount t1 WHERE t1.VendorID = t2.VendorID AND t1.IsActive = 1 FOR XML PATH('')),1,1,'') as varchar(max)) + ', ' + t2.Email from Vendor t2 where t2.VendorID = " & sVendorIds)
                'Dim sVendorEmails As String = DB.ExecuteScalar("select CAST(stuff((SELECT ',' + t1.Email FROM VendorAccount t1 join VendorAccountVendorRole t3 on t1.VendorAccountid= t3.vendoraccountid WHERE t1.VendorID = t2.VendorID AND t1.IsActive = 1 and t3.VendorRoleid in (1,5) FOR XML PATH('')),1,1,'') as varchar(max)) + ', ' + COALESCE(t2.Email, 'customerservice@cbusa.us') from Vendor t2 where t2.VendorID =  " & sVendorIds)
                Dim sVendorEmails As String = DB.ExecuteScalar("SELECT COALESCE(STUFF((SELECT ',' + VendorEmail.Email FROM(SELECT DISTINCT VA.Email FROM VendorAccount VA JOIN VendorAccountVendorRole VAVR ON VA.VendorAccountID = VAVR.VendorAccountID WHERE VA.VendorID = " & sVendorIds & " AND VA.IsActive = 1 AND VAVR.VendorRoleID IN (1, 5)) AS VendorEmail FOR XML PATH('')),1,1,''), 'NA') VendorEmails")

                Dim llcBuilderRow As LLCRow = LLCRow.GetBuilderLLC(DB, Session("BuilderId"))
                Dim addr As String = SysParam.GetValue(DB, "RebateNotificationList")
                Dim addrCC As String = SysParam.GetValue(DB, "RebateNotificationListCC")
                'Core.SendHTMLMailToMultipleRecipient(sBuilderAccountEmail, sBuilderName, sVendorEmails, sVendorEmails, sSubject, sMsg, llcBuilderRow.NotificationEmailList & IIf(addrCC <> String.Empty, "," & addrCC, ""), addr)

                If sVendorEmails <> "NA" Then
                    Dim arr As String() = Split(sVendorEmails, ",")
                    Dim iCounter As Integer = 0

                    For Each sEmail As String In arr
                        If iCounter = 0 Then
                            Core.SendHTMLMail(sBuilderAccountEmail, sBuilderName, sEmail, sEmail, sSubject, sMsg, llcBuilderRow.NotificationEmailList & IIf(addrCC <> String.Empty, "," & addrCC, ""), addr)
                        Else
                            Core.SendHTMLMail(sBuilderAccountEmail, sBuilderName, sEmail, sEmail, sSubject, sMsg)
                        End If

                        iCounter = iCounter + 1
                    Next
                End If

            End If
        Next
        Response.Redirect("Rebate-Notification.aspx?c=y")
    End Sub

    Protected Sub btnHistory_Click(sender As Object, e As System.EventArgs) Handles btnHistory.Click
        Response.Redirect("RebateNotificationHistory.aspx")
    End Sub

    Protected Sub RblEmailPreference_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles RblEmailPreference.SelectedIndexChanged
        If Session("BuilderId") > 0 Then
            dbbuilder = BuilderRow.GetRow(DB, Session("BuilderId"))
            dbbuilder.RebatesEmailPreferences = RblEmailPreference.SelectedValue
            dbbuilder.Update()
            BindPreferences()
        End If
    End Sub
End Class
