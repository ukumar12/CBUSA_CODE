Imports Components
Imports DataLayer
Imports System.Configuration
Imports System.Configuration.ConfigurationManager
Imports System.Text

Public Class VendorBidSubmissionReminder

    Public Shared Sub Run(ByVal DB As Database)
        Dim objDB As New Database
        Dim dbTaskLog As TaskLogRow = Nothing
        Dim sql As String = String.Empty
        Try
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "CPVendorBidSubmissionReminder"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()

            '================= WRITE CODE HERE ====================
            'sql = "Select distinct  tpc.TwoPriceCampaignId, tpcv.VendorId,  tpc.Name  as EventTitle,  v.CompanyName, BldrInvi.PrimaryContact,"
            'sql &= " Convert(VARCHAR(10), CAST(tpc.StartDate As Date), 101)  As EventStart,  "
            'sql &= " Convert(VARCHAR(10), CAST(tpc.EndDate As Date), 101)  As EventEnd,  "
            'sql &= " BldrInvi.EventDescription As EventDescription,  "
            'sql &= " BldrInvi.ResponseDeadline  As ResponseDeadline,"
            'sql &= " cast(getdate()+2 As Date) As Today,  "
            'sql &= " Format(BldrInvi.ResponseDeadline,'D','en-US') AS ResponseDeadlineFullDay, "
            'sql &= " Format(tpc.EndDate,'D','en-US') AS EventEndDateFullDay, "
            'sql &= " amd.FirstName+' '+ amd.LastName AS ContactName,  "
            'sql &= " amd.Email AS ContactEmail,  "
            'sql &= " amd.Contact As ContactPhone,  "
            'sql &= " amd.Email as AdminEmail,  "
            'sql &= " tpc.twopricecampaignid  "
            'sql &= " from twopricecampaign tpc "
            'sql &= " inner join TwoPriceBuilderInvitation BldrInvi  on tpc.twopricecampaignid   = bldrinvi.twopricecampaignid  "
            'sql &= " inner join TwoPriceCampaignVendor_Rel	tpcv	on tpcv.TwoPriceCampaignId  = tpc.TwoPriceCampaignId  "
            'sql &= " inner join Vendor v							on tpcv.VendorId           = v.VendorId"
            'sql &= " inner join admin amd                           on BldrInvi.PrimaryContact  = amd.AdminId "
            'sql &= " where cast(tpc.enddate As Date)  >=cast(getdate() As Date)"
            'sql &= " And tpc.Status != 'Awarded' and tpc.isactive =1 "
            'sql &= "  AND  tpcv.VendorId not in (SELECT DISTINCT VendorId FROM TwoPriceVendorProductPrice WHERE VendorId=tpcv.VendorId  AND TwoPriceCampaignId =    tpc.TwoPriceCampaignId AND Submitted = 1) "
            'sql &= " AND CAST(GETDATE() As Date) = 
            '        CASE 
            '          WHEN DATEPART(DW, tpc.VendorBidDeadline) = 1 THEN  dateadd(DAY,-2,CAST(tpc.VendorBidDeadline As Date))
            '          WHEN DATEPART(DW, tpc.VendorBidDeadline) = 2 THEN  dateadd(DAY,-3,CAST(tpc.VendorBidDeadline As Date))
            '          WHEN DATEPART(DW, tpc.VendorBidDeadline) = 7 THEN  dateadd(DAY,-1,CAST(tpc.VendorBidDeadline As Date)) 
            '          ELSE   dateadd(DAY,-1,CAST(tpc.VendorBidDeadline As Date) ) END" '1- Sunday , 2-Monday,  7 - Saturday

            sql = "Select distinct  tpc.TwoPriceCampaignId, tpcv.VendorId,  tpc.Name  as EventTitle,  v.CompanyName,"
            sql &= " Convert(VARCHAR(10), CAST(tpc.StartDate As Date), 101)  As EventStart,  "
            sql &= " Convert(VARCHAR(10), CAST(tpc.EndDate As Date), 101)  As EventEnd,  "
            sql &= " tpc.NAME AS EventDescription, "
            sql &= " cast(getdate()+2 As Date) As Today,  "
            sql &= " Format(tpc.VendorBidDeadline,'D','en-US') AS VendorBidDeadlineFullDay,  "
            sql &= " Format(tpc.EndDate,'D','en-US') AS EventEndDateFullDay, "
            sql &= " tpc.twopricecampaignid  "
            sql &= " from twopricecampaign tpc "
            sql &= " inner join TwoPriceCampaignVendor_Rel	tpcv	on tpcv.TwoPriceCampaignId  = tpc.TwoPriceCampaignId  "
            sql &= " inner join Vendor v							on tpcv.VendorId           = v.VendorId"
            sql &= " where cast(tpc.enddate As Date)  >=cast(getdate() As Date)"
            sql &= " And tpc.Status = 'BiddingInProgress' and tpc.isactive =1 "
            sql &= " AND tpcv.VendorId not in (SELECT DISTINCT VendorId FROM TwoPriceVendorProductPrice WHERE VendorId=tpcv.VendorId  AND TwoPriceCampaignId =    tpc.TwoPriceCampaignId AND Submitted = 1) "
            sql &= " AND CAST(GETDATE() As Date) = 
            Case 
                            WHEN DATEPART(DW, tpc.VendorBidDeadline) = 1 THEN  dateadd(DAY,-2,CAST(tpc.VendorBidDeadline As Date))
                            WHEN DATEPART(DW, tpc.VendorBidDeadline) = 2 THEN  dateadd(DAY,-3,CAST(tpc.VendorBidDeadline As Date))
                            WHEN DATEPART(DW, tpc.VendorBidDeadline) = 7 THEN  dateadd(DAY,-1,CAST(tpc.VendorBidDeadline As Date)) 
                  Else   dateadd(DAY,-1,CAST(tpc.VendorBidDeadline As Date) ) End" '1- Sunday , 2-Monday,  7 - Saturday

            Dim dtTwoPriceEvent As DataTable = DB.GetDataTable(sql)
            Dim bHasErrors As Boolean = False
            Dim errorMsg As New StringBuilder
            Dim ecount As Integer = 0
            If dtTwoPriceEvent.Rows.Count > 0 Then

                errorMsg.AppendLine("CPVendorBidSubmissionReminder Task: " & DateTime.Now)
                Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "CPVendorBidSubmissionReminder")
                For Each row As DataRow In dtTwoPriceEvent.Rows
                    Try

                        Dim sSubject As String = FormatSubject(DB, row, dbMsg.Subject)

                        Dim sBody As String = FormatMessage(DB, row, dbMsg.Message)
                        sBody = sBody.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&nbsp;", " ").Replace("&amp;", "").Replace("rsquo;", "'")

                        'Automatic Message Recipients
                        If dbMsg.IsEmail And dbMsg.IsActive Then

                            Dim sqlRecipients As String =
                                " select Distinct va.FirstName , va.LastName,va.Email  " _
                                & " from VendorAccount va inner join VendorAccountVendorRole vavr on va.VendorAccountId=vavr.VendorAccountId" _
                                & "     inner join AutomaticMessageVendorRole amvr on vavr.VendorRoleId=amvr.VendorRoleId" _
                                & " where " _
                                & "     va.VendorId=" & row("VendorID") _
                                & " and " _
                                & "     amvr.AutomaticMessageId=" & dbMsg.AutomaticMessageID

                            Dim dtRecipients As DataTable = DB.GetDataTable(sqlRecipients)
                            If dtRecipients.Rows.Count > 0 Then
                                For Each rowRecipients As DataRow In dtRecipients.Rows
                                    Dim addr As String = rowRecipients("Email")
                                    If SysParam.GetValue(DB, "AutoMessageTestMode") Then
                                        addr = SysParam.GetValue(DB, "AdminEmail")
                                    End If
                                    If Core.IsEmail(addr) Then
                                        Dim FullName As String = Core.BuildFullName(Core.GetString(rowRecipients("FirstName")), "", Core.GetString(rowRecipients("LastName")))
                                        sBody = sBody.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&nbsp;", " ").Replace("&amp;", "").Replace("rsquo;", "'")
                                        Dim fromEmail As String = "customerservice@cbusa.us"
                                        Dim fromName As String = row("CompanyName")
                                        Core.SendSimpleMail(fromEmail, fromName, addr, FullName, sSubject, sBody)
                                    End If
                                Next
                            End If
                        End If

                        'SendEmail(row, sSubject, sBody)

                    Catch ex As Exception
                        bHasErrors = True
                        errorMsg.AppendLine("----------------------------------")
                        errorMsg.AppendLine("Error sending to :")
                        errorMsg.AppendLine("Vendor: " & Core.GetString(row("CompanyName")))
                        errorMsg.AppendLine("Event Title: " & Core.GetString(row("EventTitle")))
                        errorMsg.AppendLine("Vendor Bid Dead line Day: " & Core.GetString(row("VendorBidDeadlineFullDay")))
                        errorMsg.AppendLine(ex.Message)
                        errorMsg.AppendLine("----------------------------------")
                    End Try
                Next

                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "CPVendorBidSubmissionReminder"
                dbTaskLog.Status = "Completed"
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "CP Vendor Bid Submission Reminder"
                dbTaskLog.Insert()
            End If
            If bHasErrors Then
                Logger.Error(errorMsg.ToString)
                Console.WriteLine("Errors occured in this task.  Please review log4net to see detailed information regarding these errors")
            End If

        Catch ex As Exception
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "CPVendorBidSubmissionReminder"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        Finally
            objDB.Close()
            DB.Close()
        End Try
    End Sub



    Private Shared Sub SendEmail(ByVal drEvent As DataRow, ByVal sSubject As String, ByVal sBody As String)

        Dim fromEmail As String = drEvent("AdminEmail")
        Dim fromName As String = drEvent("CompanyName")

        Dim ToEmail As String = drEvent("ContactEmail")
        Dim ToName As String = drEvent("CompanyName")

        If Core.IsEmail(ToEmail) Then
            Core.SendSimpleMail(fromEmail, fromName, ToEmail, ToName, sSubject, sBody)
            'Core.SendSimpleMail(FromAddress, FromName, Email, Name, "CBUSA Forgot Password", msg)
            'Core.SendSimpleMail("customerservice@cbusa.us", "CBUSA", "arrowdev@medullus.com", lblUserName.Text, "Home Starts Survey", sMailBody)
            Console.WriteLine("Email Sent to: " & ToEmail & " | Company: " & drEvent("CompanyName") & " | Event: " & drEvent("EventTitle") & " | Sub: " & sSubject)
            'log this to a text file
        Else
            Throw New Exception("Vendor Email was invalid")
        End If
    End Sub

    Private Shared Function FormatSubject(ByVal DB As Database, ByVal drEvent As DataRow, ByVal Subject As String) As String
        Subject = Subject.Replace("%%EventDescription%%", drEvent("EventDescription"))
        Return Subject
    End Function

    Private Shared Function FormatMessage(ByVal DB As Database, ByVal drEvent As DataRow, ByVal Msg As String) As String

        Msg = Msg.Replace("%%EventDescription%%", drEvent("EventDescription"))
        Msg = Msg.Replace("%%VENDOR_BID_DEAD_LINE%%", drEvent("VendorBidDeadlineFullDay"))
        Msg = Msg.Replace("%%EVENT_END_DATE%%", drEvent("EventEndDateFullDay"))
        'Msg = Msg.Replace("%%NAME_CONTACT_INFO%%", GetEventCreatorInfo(DB, drEvent))
        Msg = Msg.Replace("%%NAME_CONTACT_INFO%%", "<br/>Thanks,<br/> CBUSA")
        Return Msg
    End Function


    Private Shared Function GetEventCreatorInfo(ByVal DB As Database, ByVal drEvent As DataRow) As String

        Dim sContactInfo As String = String.Empty
        Dim sb As StringBuilder = New StringBuilder()
        Dim sQuery As String = String.Empty
        sQuery &= "Select  * From Admin WHERE AdminId = " & drEvent("PrimaryContact") & ""

        Dim dtAdmin As DataTable = DB.GetDataTable(sQuery)
        If dtAdmin.Rows.Count > 0 Then
            Dim dr As DataRow = dtAdmin.Rows(0)
            Dim sBody As String = "<b> " & dr("FirstName") & " " & dr("LastName") & "</b>" & vbCrLf
            sBody &= "" & dr("Contact") & vbCrLf
            sBody &= "" & dr("Email") & vbCrLf
            sBody &= "<a href='https://cbusa.us'>www.cbusa.us</a>" & vbCrLf
            Return sContactInfo & sBody
        End If

        Return sContactInfo


    End Function

End Class
