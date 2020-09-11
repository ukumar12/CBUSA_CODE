Imports Components
Imports DataLayer
Imports System.Configuration
Imports System.Configuration.ConfigurationManager
Imports System.Text

Public Class VendorEventNotification

    Public Shared Property Keywords As Dictionary(Of String, String)



    Public Shared Sub Run(ByVal DB As Database)
        Dim objDB As New Database
        Dim dbTaskLog As TaskLogRow = Nothing
        Dim sql As String = String.Empty

        Keywords = New Dictionary(Of String, String) From {
                    {"lumber", "Lumber"},
                    {"lumb", "Lumber"},
                    {"ewp", "EWP"},
                    {"engineered", "EWP"},
                    {"fc", "FC Siding"},
                    {"fiber cement", "FC Siding"},
                    {"spray", "Spray Foam Insulation"},
                    {"sf", "Spray Foam Insulation"},
                    {"fibergl", "Fiberglass Insulation"},
                    {"Framing", "Lumnber"},
                    {"Frame", "Lumnber"},
                    {"drywall", "Drywall"},
                    {"concrete", "Concrete"},
                    {"roof", "Roofing"},
                    {"window", "Windows"},
                    {"block", "Block"},
                    {"shingle", "Shingles"}
         }

        Try
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "CPVendorEventNotification"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()

            '================= WRITE CODE HERE ====================
            sql = "  Select distinct  tpc.TwoPriceCampaignId, v.VendorId,  tpc.Name  as EventTitle,  v.CompanyName, BldrInvi.PrimaryContact, "
            sql &= " Convert(VARCHAR(10), CAST(tpc.StartDate As Date), 101)  As EventStart,  "
            sql &= " Convert(VARCHAR(10), CAST(tpc.EndDate As Date), 101)  As EventEnd,  "
            sql &= " BldrInvi.EventDescription As EventDescription,  "
            sql &= " BldrInvi.ResponseDeadline  As ResponseDeadline,"
            sql &= " Format(isnull(tpc.VendorBidDeadLine,tpc.StartDate+10),'D','en-US')  AS VendorBidDeadLineFullDay, "
            sql &= " Format(BldrInvi.ResponseDeadline,'D','en-US')  AS ResponseDeadlineFullDay, "
            sql &= " Format(tpc.StartDate,'D','en-US')  AS StartDateFullDay, "
            sql &= " Format(tpc.EndDate,'D','en-US')    AS EndDateFullDay, "
            sql &= " amd.FirstName+' '+ amd.LastName AS ContactName,  "
            sql &= " amd.Email AS ContactEmail,  "
            sql &= " amd.Contact As ContactPhone,  "
            sql &= " amd.Email as AdminEmail,  "
            sql &= " tpc.twopricecampaignid,  "
            sql &= " STUFF((SELECT ', ' + CAST(LLC AS VARCHAR(10)) [text()]
                     FROM LLC where LLCID IN 	 (SELECT LLCId FROM TwoPriceCampaignLLC_Rel llc_rel 
                                                   WHERE llc_rel.TwoPriceCampaignId = tpc.TwoPriceCampaignId)
                      FOR XML PATH(''), TYPE) .value('.','NVARCHAR(MAX)'),1,2,' ') Markets "
            sql &= "  FROM       twopricecampaign tpc "
            sql &= "  INNER JOIN TwoPriceBuilderInvitation BldrInvi     on tpc.twopricecampaignid       = BldrInvi.twopricecampaignid   "
            sql &= "  INNER JOIN TwoPrice_NotificationToVendor	tpcnv	on tpcnv.twopricecampaignid    = tpc.twopricecampaignid "
            sql &= "  INNER JOIN Vendor v							    on tpcnv.VendorID              = v.VendorId"
            sql &= "  INNER JOIN admin amd                              on BldrInvi.PrimaryContact      = amd.AdminId "
            sql &= "  where cast(tpc.StartDate As Date)                 >=      cast(getdate() As Date)"
            sql &= "  And   IsNull(tpc.VendorInvitationStatus,0)        =       0"
            sql &= "  And   cast(tpc.enddate As Date)                   >=      cast(getdate() As Date)"
            sql &= "  And   BldrInvi.InvitationStatus                   =       1 "                          'legacy system
            sql &= "  And tpc.Status                                    =      'New'  "
            sql &= "  And tpc.IsActive                                  =       1 "
            sql &= "  And tpc.SendNotificationToVendor                  =       1 "

            Dim dtTwoPriceEvent As DataTable = DB.GetDataTable(sql)
            Dim bHasErrors As Boolean = False
            Dim errorMsg As New StringBuilder
            Dim ecount As Integer = 0
            If dtTwoPriceEvent.Rows.Count > 0 Then

                errorMsg.AppendLine("CPVendorEventNotification Task: " & DateTime.Now)
                Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "CPVendorEventNotification")
                For Each row As DataRow In dtTwoPriceEvent.Rows
                    Try
                        Dim sSubject As String = FormatSubject(DB, row, dbMsg.Subject)
                        Dim sBody As String = FormatMessage(DB, row, dbMsg.Message)

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
                                        Dim FirstName As String = Core.GetString(rowRecipients("FirstName"))
                                        sBody = sBody.Replace("%%RecipientFirstName%%", FirstName)
                                        sBody = sBody.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&nbsp;", " ").Replace("&amp;", "").Replace("rsquo;", "'")
                                        Dim fromEmail As String = "customerservice@cbusa.us"
                                        Dim fromName As String = row("CompanyName")
                                        Core.SendSimpleMail(fromEmail, fromName, addr, FirstName, sSubject, sBody)
                                    End If
                                Next
                            End If
                        End If

                    Catch ex As Exception
                        bHasErrors = True
                        errorMsg.AppendLine("----------------------------------")
                        errorMsg.AppendLine("Error sending to :")
                        errorMsg.AppendLine("Vendor: " & Core.GetString(row("CompanyName")))
                        errorMsg.AppendLine("Event Title: " & Core.GetString(row("EventTitle")))
                        errorMsg.AppendLine("Response Dead line Day: " & Core.GetString(row("ResponseDeadlineFullDay")))
                        errorMsg.AppendLine(ex.Message)
                        errorMsg.AppendLine("----------------------------------")
                    End Try
                Next

                'Update Campaign VendorInvitationStatus after sending notifications
                Dim _tpcIds As String = String.Empty
                Dim view As DataView = New DataView(dtTwoPriceEvent)
                Dim dtCampaignId As DataTable = view.ToTable(True, "TwoPriceCampaignId")
                For Each row As DataRow In dtCampaignId.Rows
                    _tpcIds += row("TwoPriceCampaignId").ToString() + ","
                Next
                If Not String.IsNullOrEmpty(_tpcIds) Then

                    DB.ExecuteSQL("Update TwoPriceCampaign set VendorInvitationStatus=1, VendorInvitationSent=getdate() where  TwoPriceCampaignId in (" & _tpcIds.TrimEnd(",") & ")")
                End If


                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "CPVendorEventNotification"
                dbTaskLog.Status = "Completed"
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "CP Vendor Event Notification Sent "
                dbTaskLog.Insert()
            End If
            If bHasErrors Then
                Logger.Error(errorMsg.ToString)
                Console.WriteLine("Errors occured in this task.  Please review log4net to see detailed information regarding these errors")
            End If

        Catch ex As Exception
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "CPVendorEventNotification"
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
            Console.WriteLine("Email Sent to: " & ToEmail & " | Company: " & drEvent("CompanyName") & " | Event: " & drEvent("EventTitle"))
        Else
            Throw New Exception("Vendor Email was invalid")
        End If
    End Sub

    Private Shared Function FormatSubject(ByVal DB As Database, ByVal drEvent As DataRow, ByVal Subject As String) As String
        Subject = Subject.Replace("%%EventDescription%%", drEvent("EventDescription"))

        Return Subject
    End Function

    Private Shared Function FormatMessage(ByVal DB As Database, ByVal drEvent As DataRow, ByVal Msg As String) As String



        Dim EventName As String = drEvent("EventTitle").ToString().ToLower()
        'EventName = Keywords.Aggregate(EventName, Function(result, s) result.Replace(s.Key, s.Value))
        Dim EventNameArray = EventName.Split(" ")
        For Each Str As String In EventNameArray
            If Keywords.ContainsKey(Str) Then
                EventName = Keywords(Str)
                Exit For
            End If
        Next

        ' Msg = Msg.Replace("%%RecipientFirstName%%", drEvent("CompanyName"))
        Msg = Msg.Replace("%%LLC%%", drEvent("Markets"))
        Msg = Msg.Replace("%%EVENT_NAME%%", EventName)
        Msg = Msg.Replace("%%EVENT_START_DATE%%", drEvent("StartDateFullDay"))
        Msg = Msg.Replace("%%EVENT_END_DATE%%", drEvent("EndDateFullDay"))
        Msg = Msg.Replace("%%RESPONSE_DEAD_LINE%%", drEvent("ResponseDeadlineFullDay"))
        Msg = Msg.Replace("%%BID_DEAD_LINE%%", drEvent("VendorBidDeadLineFullDay"))
        Msg = Msg.Replace("%%CONTACT_NAME%%", drEvent("ContactName"))
        Msg = Msg.Replace("%%CONTACT_INFO%%", GetEventCreatorInfo(DB, drEvent))

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
            Dim sBody As String = " <b>" & dr("FirstName") & " " & dr("LastName") & "</b>" & vbCrLf
            sBody &= "" & dr("Contact") & vbCrLf
            sBody &= "" & dr("Email") & vbCrLf
            sBody &= "<a href='https://cbusa.us'>www.cbusa.us</a>" & vbCrLf
            Return sContactInfo & sBody
        End If

        Return sContactInfo


    End Function

End Class
