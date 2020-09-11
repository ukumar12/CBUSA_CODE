Imports Components
Imports DataLayer
Imports System.Configuration
Imports System.Configuration.ConfigurationManager
Public Class CpEventReminder
    Public Shared Sub Run(ByVal DB As Database)
        Dim objDB As New Database
        Dim dbTaskLog As TaskLogRow = Nothing
        Try
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "CpReminderTask"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()


            objDB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))
            Dim dtReminder1 As DataTable = objDB.GetDataTable("Select tpbi.twopricecampaignid from twopricebuilderinvitation tpbi join TwopriceCampaign tpc on tpbi.TwopriceCampaignId = tpc.TwopriceCampaignid where  datepart(d, tpbi.Reminder1scheduledutc) = datepart(d, getdate()) and datepart(MM, tpbi.Reminder1scheduledutc) = datepart(MM, getdate()) and datepart(YY, tpbi.Reminder1scheduledutc) = datepart(YY, getdate()) and datepart(hh, tpbi.Reminder1scheduledutc) = datepart(hh, getdate()) And DatePart(Minute, tpbi.Reminder1scheduledutc) = DatePart(Minute, getdate()) and tpc.TwopriceCampaignId not in (Select TwoPriceCampaignId From TwoPriceCampaign Where Status='Awarded')") 'And DatePart(Minute, tpbi.Reminder1scheduledutc) = DatePart(Minute, getdate())

            Dim dtReminder2 As DataTable = objDB.GetDataTable("Select tpbi.twopricecampaignid from twopricebuilderinvitation tpbi join TwopriceCampaign tpc on tpbi.TwopriceCampaignId = tpc.TwopriceCampaignid where  datepart(d, tpbi.Reminder2scheduledutc) = datepart(d, getdate()) and datepart(MM, tpbi.Reminder2scheduledutc) = datepart(MM, getdate()) and datepart(YY, tpbi.Reminder2scheduledutc) = datepart(YY, getdate()) and datepart(hh, tpbi.Reminder2scheduledutc) = datepart(hh, getdate()) And DatePart(Minute, tpbi.Reminder2scheduledutc) = DatePart(Minute, getdate()) and tpc.TwopriceCampaignId not in (Select TwoPriceCampaignId From TwoPriceCampaign Where Status='Awarded')")

            If dtReminder1.Rows.Count = 0 Then
                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "CpReminder1Task"
                dbTaskLog.Status = "Skipped"
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "Reminder 1 Script turned off. "
                dbTaskLog.Insert()
            Else

                Dim i As Integer = 0

                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "CpReminder1Task"
                dbTaskLog.Status = "started Reminder1 "
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "Reminder1Utc Matches"
                dbTaskLog.Insert()
                    For Each row As DataRow In dtReminder1.Rows

                    Dim TwoPricecampaignid As String = dtReminder1.Rows(i).Item(0).ToString

                    Dim ReminderEnroll As String = DB.ExecuteScalar("Select count(bldr.builderid)  from builder bldr join TwoPriceCampaignLLC_Rel  llcRel On bldr.llcid = llcrel.llcid " _
                    & " join twopricebuilderinvitation BuilInvi On builinvi.twopricecampaignid = llcrel.twopricecampaignid " _
                    & " where bldr.builderid Not In " _
                    & " (Select tpbp.builderid from twopricebuilderparticipation tpbp where tpbp.twopricecampaignid =" & TwoPricecampaignid & ")And" _
                    & "  bldr.isactive=1 And BuilInvi.TwopriceCampaignId = " & TwoPricecampaignid)

                    Dim ReminderProject As String = DB.ExecuteScalar(" Select count(tpbp.builderid) from TwoPriceBuilderParticipation tpbp  " _
                    & " where tpbp.participationtype = 1  And tpbp.TwopriceCampaignId=" & TwoPricecampaignid & " And tpbp.builderid  In " _
                    & " (Select builderid from TwoPriceBuilderParticipation where ParticipationType != 3 and ParticipationType != 2 and TwoPriceCampaignId=" & TwoPricecampaignid & ") " _
                    & "and tpbp.builderid not in (select builderid from twopricebuilderproject where TwoPriceCampaignid=" & TwoPricecampaignid & ") ")

                    If ReminderEnroll <> 0 Then
                        GetTwoPriceBuilderReminder1Enroll(DB, TwoPricecampaignid)
                    End If
                    If ReminderProject <> 0 Then
                        GetTwoPriceBuilderProjectReminder1(DB, TwoPricecampaignid)
                    End If

                    i = i + 1
                    Next

            End If
            If dtReminder2.Rows.Count = 0 Then
                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "CpReminder2Task"
                dbTaskLog.Status = "Skipped"
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "Reminder2 Script turned off. "
                dbTaskLog.Insert()
                'Exit Sub
            Else
                Dim i As Integer

                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "CpReminder2Task"
                dbTaskLog.Status = "started Reminder2 "
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "Reminder2Utc Matches"
                dbTaskLog.Insert()

                For Each row As DataRow In dtReminder2.Rows
                    Dim RemTwoPricecampaignid_2 As String = dtReminder2.Rows(i).Item(0).ToString

                    Dim ReminderEnroll As String = DB.ExecuteScalar("Select count(bldr.builderid)  from builder bldr join TwoPriceCampaignLLC_Rel  llcRel On bldr.llcid = llcrel.llcid " _
                    & " join twopricebuilderinvitation BuilInvi On builinvi.twopricecampaignid = llcrel.twopricecampaignid " _
                    & " where bldr.builderid Not In " _
                    & " (Select tpbp.builderid from twopricebuilderparticipation tpbp where tpbp.twopricecampaignid =" & RemTwoPricecampaignid_2 & ")And" _
                    & "  bldr.isactive=1 And BuilInvi.TwopriceCampaignId = " & RemTwoPricecampaignid_2)

                    Dim ReminderProject As String = DB.ExecuteScalar("  Select count(tpbp.builderid) from TwoPriceBuilderParticipation tpbp  " _
                    & " where tpbp.participationtype = 1  And tpbp.TwopriceCampaignId=" & RemTwoPricecampaignid_2 & " And tpbp.builderid  In " _
                    & " (Select builderid from TwoPriceBuilderParticipation where ParticipationType != 3 and ParticipationType != 2 and TwoPriceCampaignId=" & RemTwoPricecampaignid_2 & ") " _
                    & "and tpbp.builderid not in (select builderid from twopricebuilderproject where TwoPriceCampaignid=" & RemTwoPricecampaignid_2 & ") ")

                    If ReminderEnroll <> 0 Then
                        GetTwoPriceBuilderReminder2Enroll(DB, RemTwoPricecampaignid_2)
                    End If
                    If ReminderProject <> 0 Then
                        GetTwoPriceBuilderProjectReminder2(DB, RemTwoPricecampaignid_2)
                    End If
                    i = i + 1
                Next
            End If

        Catch ex As Exception
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "CpReminderTask"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        Finally
            objDB.Close()
            DB.Close()
        End Try
    End Sub
    Public Shared Sub GetTwoPriceBuilderReminder1Enroll(ByVal DB As Database, TwoPricecampaignid As String)       'Reminder1 for enroll
        Dim objDB As New Database
        Dim dtBuilder As DataTable
        Dim dtBuilderEnroll As DataTable
        Dim dbTaskLog As TaskLogRow = Nothing
        Dim cnt As Integer = 0

        objDB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))

        dtBuilderEnroll = objDB.GetDataTable("Select distinct  tpc.Name as EventTitle, " _
        & " Convert(VARCHAR(10), CAST(tpc.StartDate As Date), 101)  As EventStart, " _
        & " Convert(VARCHAR(10), CAST(tpc.EndDate As Date), 101)  As EventEnd, " _
        & " BldrInvi.EventDescription As EventDescription, " _
        & " BldrInvi.ResponseDeadline  As ResponseDeadline, " _
        & " Format(BldrInvi.ResponseDeadline, 'MMMM dd,yyyy') AS ResponseDeadlineFull, " _
        & " Format(BldrInvi.ResponseDeadline, 'dddd,MM/dd/yyyy') as ResponseDeadlineDay, " _
        & " Format(BldrInvi.ResponseDeadline,'D','en-US') AS ResponseDeadlineFullDay, " _
        & " BldrInvi.InvitationMessage As MailBody, " _
        & " BldrInvi.Reminder1EnrollSubject as ReminderMailsubject, " _
        & " BldrInvi.Reminder1EnrollMessage As ReminderMailBody, " _
        & " BldrInvi.InvitationOptInText as OptIn, " _
        & " BldrInvi.InvitationOptOutText AS OptOut, " _
        & " tpc.twopricecampaignid, " _
        & " amd.FirstName+' '+ amd.LastName AS ContactName, " _
        & " amd.Email AS ContactEmail, " _
        & " amd.Contact as ContactPhone, " _
        & " amd.Email as AdminEmail " _
        & " from TwoPriceBuilderInvitation BldrInvi join twopricecampaign tpc on tpc.twopricecampaignid = bldrinvi.twopricecampaignid " _
        & " join admin amd on BldrInvi.PrimaryContact= amd.AdminId " _
        & " where BldrInvi.twopricecampaignid = " & TwoPricecampaignid)

        Dim sEventTitle, sEventStart, sEventEnd, sEventDescription, sResponseDeadline, sResponseDeadlineFull, sResponseDeadlineDay, sResponseDeadlineFullDay, sMailBody, sOptin, sOptout, sContactName, sContactEmail, sContactPhone, sAdminEmail, Eventid, sReminderMailsubject, sReminderMailBody As String
        sEventTitle = dtBuilderEnroll.Rows(0)("EventTitle").ToString()
        sEventStart = dtBuilderEnroll.Rows(0)("EventStart").ToString()
        sEventEnd = dtBuilderEnroll.Rows(0)("EventEnd").ToString()
        sEventDescription = dtBuilderEnroll.Rows(0)("EventDescription").ToString()
        sResponseDeadline = dtBuilderEnroll.Rows(0)("ResponseDeadline").ToString()
        sResponseDeadlineFull = dtBuilderEnroll.Rows(0)("ResponseDeadlineFull").ToString()
        sResponseDeadlineDay = dtBuilderEnroll.Rows(0)("ResponseDeadlineDay").ToString()
        sResponseDeadlineFullDay = dtBuilderEnroll.Rows(0)("ResponseDeadlineFullDay").ToString()
        sMailBody = dtBuilderEnroll.Rows(0)("MailBody").ToString()
        sOptin = dtBuilderEnroll.Rows(0)("OptIn").ToString()
        sOptout = dtBuilderEnroll.Rows(0)("OptOut").ToString()
        sContactName = dtBuilderEnroll.Rows(0)("ContactName").ToString()
        sContactEmail = dtBuilderEnroll.Rows(0)("ContactEmail").ToString()
        sContactPhone = dtBuilderEnroll.Rows(0)("ContactPhone").ToString()
        sAdminEmail = dtBuilderEnroll.Rows(0)("AdminEmail").ToString()
        Eventid = dtBuilderEnroll.Rows(0)("twopricecampaignid").ToString()
        sReminderMailsubject = dtBuilderEnroll.Rows(0)("ReminderMailsubject").ToString()
        sReminderMailBody = dtBuilderEnroll.Rows(0)("ReminderMailBody").ToString()

        Dim OptInLink As String = AppSettings("GlobalRefererName") & "/default.aspx?mod=tpc&Tcam=" & Eventid & "&Opt=1"
        Dim OptIn As String = "<a style=""padding:  2px 4px; border: 1px solid #282927; font-size:12px; line-height:12px;font - weight:400; color:#FFF; background:#38761d;text-decoration:none;"" href=""" & OptInLink & """>" & sOptin & "</a>"

        'replace mailbody to
        Dim MailBody As String = sMailBody
        MailBody = Replace(MailBody, "{{%EventTitle%}}", sEventTitle)
        MailBody = Replace(MailBody, "{{%EventStart%}}", sEventStart)
        MailBody = Replace(MailBody, "{{%EventEnd%}}", sEventEnd)
        MailBody = Replace(MailBody, "{{%EventDescription%}}", sEventDescription)
        MailBody = Replace(MailBody, "{{%ResponseDeadline%}}", sResponseDeadline)
        MailBody = Replace(MailBody, "{{%ResponseDeadlineFull%}}", sResponseDeadlineFull)
        MailBody = Replace(MailBody, "{{%ResponseDeadlineDay%}}", sResponseDeadlineDay)
        MailBody = Replace(MailBody, "{{%ResponseDeadlineFullDay%}}", sResponseDeadlineFullDay)
        MailBody = Replace(MailBody, "{{%OptIn%}}", OptIn)
        MailBody = Replace(MailBody, "{{%ContactName%}}", sContactName)
        MailBody = Replace(MailBody, "{{%ContactEmail%}}", sContactEmail)
        MailBody = Replace(MailBody, "{{%ContactPhone%}}", sContactPhone)

        Dim ReminderMailsubject As String = sReminderMailsubject
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%EventTitle%}}", sEventTitle)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%EventStart%}}", sEventStart)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%EventEnd%}}", sEventEnd)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%EventDescription%}}", sEventDescription)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ResponseDeadline%}}", sResponseDeadline)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ResponseDeadlineFull%}}", sResponseDeadlineFull)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ResponseDeadlineDay%}}", sResponseDeadlineDay)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ResponseDeadlineFullDay%}}", sResponseDeadlineFullDay)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ContactName%}}", sContactName)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ContactEmail%}}", sContactEmail)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ContactPhone%}}", sContactPhone)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ReminderBlock%}}", "")
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%OptIn%}}", OptIn)

        'replace ReminderMailbody to
        Dim ReminderMailBody As String = sReminderMailBody
        ReminderMailBody = Replace(ReminderMailBody, "{{%EventTitle%}}", sEventTitle)
        ReminderMailBody = Replace(ReminderMailBody, "{{%EventStart%}}", sEventStart)
        ReminderMailBody = Replace(ReminderMailBody, "{{%EventEnd%}}", sEventEnd)
        ReminderMailBody = Replace(ReminderMailBody, "{{%EventDescription%}}", sEventDescription)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ResponseDeadline%}}", sResponseDeadline)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ResponseDeadlineFull%}}", sResponseDeadlineFull)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ResponseDeadlineDay%}}", sResponseDeadlineDay)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ResponseDeadlineFullDay%}}", sResponseDeadlineFullDay)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ContactName%}}", sContactName)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ContactEmail%}}", sContactEmail)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ContactPhone%}}", sContactPhone)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ReminderBlock%}}", "")
        ReminderMailBody = Replace(ReminderMailBody, "{{%OptIn%}}", OptIn)

        dtBuilder = objDB.GetDataTable("Select distinct bldr.builderid, " _
            & " ba.BuilderAccountID, " _
            & " bldr.CompanyName As BuilderName," _
            & " ba.FirstName As RecipientFirstName, " _
            & " ba.LastName As RecipientLastName, " _
            & " ba.FirstName +' '+ba.Lastname AS RecipientFullName," _
            & " ba.Email as RecipientEmail " _
            & " From builder bldr Join LLc llc On bldr.llcid = llc.llcid Join " _
            & " TwoPriceCampaignLLC_Rel  llcRel On llcRel.LLCId = LLC.LLCID " _
            & " Join TwoPriceCampaign tpc On tpc.TwoPriceCampaignId = llcRel.TwoPriceCampaignId " _
            & " join builderaccount ba on ba.builderid= bldr.builderid " _
            & " Where bldr.builderid Not In (Select Prttyp.builderid from TwoPriceBuilderParticipation Prttyp where Prttyp.TwoPriceCampaignId=" & TwoPricecampaignid & " ) " _
            & " And tpc.TwoPriceCampaignId= " & TwoPricecampaignid & " And " _
            & " ba.IsActive=1 and ba.email is not null And bldr.IsActive=1")

        Dim BuilderIdList As String = ""
        If dtBuilder.Rows.Count > 0 Then
            Dim i As Integer = 0
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "CpReminderTask"
            dbTaskLog.Status = "GetTwoPriceBuilderReminder1Enroll"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = dtBuilder.Rows.Count.ToString()
            dbTaskLog.Insert()
            For Each row As DataRow In dtBuilder.Rows
                Try
                    cnt += 1
                    Dim AccountMailBody, RemAccountMailSubject, RemAccountMailBody As String
                    Dim sBuilderName, sRecipientFirstName, sRecipientLastName, sRecipientFullName, sRecipientEmail, sBuilderId, sBuilderAccountId As String
                    sBuilderId = dtBuilder.Rows(i)("BuilderId").ToString()
                    sBuilderAccountId = dtBuilder.Rows(i)("BuilderAccountId").ToString()
                    sBuilderName = dtBuilder.Rows(i)("BuilderName").ToString()
                    sRecipientFirstName = dtBuilder.Rows(i)("RecipientFirstName").ToString()
                    sRecipientLastName = dtBuilder.Rows(i)("RecipientLastName").ToString()
                    sRecipientFullName = dtBuilder.Rows(i)("RecipientFullName").ToString()
                    sRecipientEmail = dtBuilder.Rows(i)("RecipientEmail").ToString()

                    Dim OptOutLink As String = AppSettings("GlobalRefererName") & "/cpmodule/OptOut.aspx?Mod=tpc&Tcam=" & Eventid & "&Opt=3&BuilderId=" & sBuilderId & "&BuilderAccountID=" & sBuilderAccountId

                    Dim OptOut As String = "<a style=""padding: 2px 4px; border:1px solid #282927; font-size:12px; line-height:12px;font-weight:400; color:#FFF; background:#cc0000;text-decoration:none;"" href=""" & OptOutLink & """>" & sOptout & "</a>"

                    RemAccountMailSubject = ReminderMailsubject
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%BuilderName%}}", sBuilderName)
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%RecipientFirstName%}}", sRecipientFirstName)
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%RecipientLastName%}}", sRecipientLastName)
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%RecipientFullName%}}", sRecipientFullName)
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%OptOut%}}", OptOut)
                    RemAccountMailSubject = RemAccountMailSubject.Replace("&lt;", "<").Replace("&gt;", ">").Replace("nbsp;", " ").Replace("&amp;", "").Replace("rsquo;", "'")

                    RemAccountMailBody = ReminderMailBody
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%BuilderName%}}", sBuilderName)
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%RecipientFirstName%}}", sRecipientFirstName)
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%RecipientLastName%}}", sRecipientLastName)
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%RecipientFullName%}}", sRecipientFullName)
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%OptOut%}}", OptOut)
                    RemAccountMailBody = RemAccountMailBody.Replace("&lt;", "<").Replace("&gt;", ">").Replace("nbsp;", " ").Replace("&amp;", "").Replace("rsquo;", "'")
                    Dim RemMailBodyStyle As String = "<div style=""border:2px solid #d01e1e;padding:10px;margin-bottom: 25px;"">" & RemAccountMailBody & "</div>"

                    AccountMailBody = MailBody
                    AccountMailBody = Replace(AccountMailBody, "{{%BuilderName%}}", sBuilderName)
                    AccountMailBody = Replace(AccountMailBody, "{{%RecipientFirstName%}}", sRecipientFirstName)
                    AccountMailBody = Replace(AccountMailBody, "{{%RecipientLastName%}}", sRecipientLastName)
                    AccountMailBody = Replace(AccountMailBody, "{{%RecipientFullName%}}", sRecipientFullName)
                    AccountMailBody = Replace(AccountMailBody, "{{%OptOut%}}", OptOut)
                    AccountMailBody = Replace(AccountMailBody, "{{%ReminderBlock%}}", RemMailBodyStyle)
                    Dim ReqMailBody As String = AccountMailBody.Replace("&lt;", "<").Replace("&gt;", ">").Replace("nbsp;", " ").Replace("&amp;", "").Replace("rsquo;", "'")

                    Core.SendSimpleMail(sAdminEmail, "CBUSA Customer Service", sRecipientEmail, sBuilderName, RemAccountMailSubject, ReqMailBody, "", "")

                    Dim strBuilderId As String = dtBuilder.Rows(i)("BuilderId").ToString()
                    If BuilderIdList.Contains(strBuilderId) = False Then
                        BuilderIdList = String.Concat(BuilderIdList, ",", dtBuilder.Rows(i)("BuilderId").ToString())
                    End If
                    i = i + 1
                    If i = 1 Then
                        Core.SendSimpleMail(sAdminEmail, "CBUSA Customer Service", sAdminEmail, sContactName, RemAccountMailSubject, ReqMailBody, "", "")
                    End If
                Catch ex As Exception
                    Console.WriteLine(ex)
                    Logger.Error(Logger.GetErrorMessage(ex))
                    dbTaskLog = New TaskLogRow(DB)
                    dbTaskLog.TaskName = "CpReminderEnroll1Task"
                    dbTaskLog.Status = "Failed"
                    dbTaskLog.LogDate = Now()
                    dbTaskLog.Msg = ex.Message
                    dbTaskLog.Insert()
                End Try
            Next
        End If

        Console.WriteLine(cnt & " CpEnrollReminder Sent")
        dbTaskLog = New TaskLogRow(DB)
        dbTaskLog.TaskName = "CpReminderTask"
        dbTaskLog.Status = "Completed"
        dbTaskLog.LogDate = Now()
        dbTaskLog.Msg = cnt & " CpEnrollReminder1 Sent, TPC ID - " & TwoPricecampaignid & "(" & BuilderIdList & ")"
        dbTaskLog.Insert()

    End Sub

    Public Shared Sub GetTwoPriceBuilderReminder2Enroll(ByVal DB As Database, RemTwoPricecampaignid_2 As String)       'Reminder1 for enroll
        Dim objRDB As New Database
        Dim dtBuilderReminder As DataTable
        Dim dtBuilderRemEnroll As DataTable
        Dim dbTaskLog As TaskLogRow = Nothing
        Dim cnt As Integer = 0
        objRDB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))

        dtBuilderRemEnroll = objRDB.GetDataTable("Select distinct  tpc.Name  as EventTitle, " _
                & " Convert(VARCHAR(10), CAST(tpc.StartDate As Date), 101)  As EventStart, " _
                & " Convert(VARCHAR(10), CAST(tpc.EndDate As Date), 101)  As EventEnd, " _
                & " BldrInvi.EventDescription As EventDescription, " _
                & " BldrInvi.ResponseDeadline  As ResponseDeadline, " _
                & " Format(BldrInvi.ResponseDeadline, 'MMMM dd,yyyy') AS ResponseDeadlineFull, " _
                & " Format(BldrInvi.ResponseDeadline, 'dddd,MM/dd/yyyy') as ResponseDeadlineDay, " _
                & " Format(BldrInvi.ResponseDeadline,'D','en-US') AS ResponseDeadlineFullDay, " _
                & " BldrInvi.InvitationMessage As MailBody, " _
                & " BldrInvi.Reminder2EnrollSubject as ReminderMailsubject, " _
                & " BldrInvi.Reminder2EnrollMessage As ReminderMailBody, " _
                & " BldrInvi.InvitationOptInText as OptIn, " _
                & " BldrInvi.InvitationOptOutText AS OptOut, " _
                & " amd.FirstName+' '+ amd.LastName AS ContactName, " _
                & " amd.Email AS ContactEmail, " _
                & " amd.Contact as ContactPhone, " _
                & " amd.Email as AdminEmail, " _
                & " tpc.twopricecampaignid " _
                & " from TwoPriceBuilderInvitation BldrInvi join twopricecampaign tpc on tpc.twopricecampaignid = bldrinvi.twopricecampaignid " _
                & " join admin amd on BldrInvi.PrimaryContact = amd.AdminId " _
                & " where BldrInvi.twopricecampaignid = " & RemTwoPricecampaignid_2)

        Dim sEventTitle, sEventStart, sEventEnd, sEventDescription, sResponseDeadline, sResponseDeadlineFull, sResponseDeadlineDay, sResponseDeadlineFullDay, sMailBody, sOptin, sOptout, sContactName, sContactEmail, sContactPhone, sAdminEmail, Eventid, sReminderMailsubject, sReminderMailBody As String
        sEventTitle = dtBuilderRemEnroll.Rows(0)("EventTitle").ToString()
        sEventStart = dtBuilderRemEnroll.Rows(0)("EventStart").ToString()
        sEventEnd = dtBuilderRemEnroll.Rows(0)("EventEnd").ToString()
        sEventDescription = dtBuilderRemEnroll.Rows(0)("EventDescription").ToString()
        sResponseDeadline = dtBuilderRemEnroll.Rows(0)("ResponseDeadline").ToString()
        sResponseDeadlineFull = dtBuilderRemEnroll.Rows(0)("ResponseDeadlineFull").ToString()
        sResponseDeadlineDay = dtBuilderRemEnroll.Rows(0)("ResponseDeadlineDay").ToString()
        sResponseDeadlineFullDay = dtBuilderRemEnroll.Rows(0)("ResponseDeadlineFullDay").ToString()
        sMailBody = dtBuilderRemEnroll.Rows(0)("MailBody").ToString()
        sOptin = dtBuilderRemEnroll.Rows(0)("OptIn").ToString()
        sOptout = dtBuilderRemEnroll.Rows(0)("OptOut").ToString()
        sContactName = dtBuilderRemEnroll.Rows(0)("ContactName").ToString()
        sContactEmail = dtBuilderRemEnroll.Rows(0)("ContactEmail").ToString()
        sContactPhone = dtBuilderRemEnroll.Rows(0)("ContactPhone").ToString()
        sAdminEmail = dtBuilderRemEnroll.Rows(0)("AdminEmail").ToString()
        Eventid = dtBuilderRemEnroll.Rows(0)("twopricecampaignid").ToString()
        sReminderMailsubject = dtBuilderRemEnroll.Rows(0)("ReminderMailsubject").ToString()
        sReminderMailBody = dtBuilderRemEnroll.Rows(0)("ReminderMailBody").ToString()

        Dim OptInLink As String = AppSettings("GlobalRefererName") & "/default.aspx?mod=tpc&Tcam=" & Eventid & "&Opt=1"
        Dim OptIn As String = "<a style=""padding:  2px 4px; border: 1px solid #282927; font-size:12px; line-height:12px;font - weight:400; color:#FFF; background:#38761d;text-decoration:none;"" href=""" & OptInLink & """>" & sOptin & "</a>"

        'replace mailbody to
        Dim MailBody As String = sMailBody
        MailBody = Replace(MailBody, "{{%EventTitle%}}", sEventTitle)
        MailBody = Replace(MailBody, "{{%EventStart%}}", sEventStart)
        MailBody = Replace(MailBody, "{{%EventEnd%}}", sEventEnd)
        MailBody = Replace(MailBody, "{{%EventDescription%}}", sEventDescription)
        MailBody = Replace(MailBody, "{{%ResponseDeadline%}}", sResponseDeadline)
        MailBody = Replace(MailBody, "{{%ResponseDeadlineFull%}}", sResponseDeadlineFull)
        MailBody = Replace(MailBody, "{{%ResponseDeadlineDay%}}", sResponseDeadlineDay)
        MailBody = Replace(MailBody, "{{%ResponseDeadlineFullDay%}}", sResponseDeadlineFullDay)
        MailBody = Replace(MailBody, "{{%OptIn%}}", OptIn)
        MailBody = Replace(MailBody, "{{%ContactName%}}", sContactName)
        MailBody = Replace(MailBody, "{{%ContactEmail%}}", sContactEmail)
        MailBody = Replace(MailBody, "{{%ContactPhone%}}", sContactPhone)


        Dim ReminderMailsubject As String = sReminderMailsubject
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%EventTitle%}}", sEventTitle)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%EventStart%}}", sEventStart)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%EventEnd%}}", sEventEnd)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%EventDescription%}}", sEventDescription)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ResponseDeadline%}}", sResponseDeadline)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ResponseDeadlineFull%}}", sResponseDeadlineFull)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ResponseDeadlineDay%}}", sResponseDeadlineDay)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ResponseDeadlineFullDay%}}", sResponseDeadlineFullDay)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ContactName%}}", sContactName)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ContactEmail%}}", sContactEmail)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ContactPhone%}}", sContactPhone)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ReminderBlock%}}", "")
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%OptIn%}}", OptIn)

        'replace ReminderMailbody to
        Dim ReminderMailBody As String = sReminderMailBody
        ReminderMailBody = Replace(ReminderMailBody, "{{%EventTitle%}}", sEventTitle)
        ReminderMailBody = Replace(ReminderMailBody, "{{%EventStart%}}", sEventStart)
        ReminderMailBody = Replace(ReminderMailBody, "{{%EventEnd%}}", sEventEnd)
        ReminderMailBody = Replace(ReminderMailBody, "{{%EventDescription%}}", sEventDescription)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ResponseDeadline%}}", sResponseDeadline)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ResponseDeadlineFull%}}", sResponseDeadlineFull)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ResponseDeadlineDay%}}", sResponseDeadlineDay)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ResponseDeadlineFullDay%}}", sResponseDeadlineFullDay)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ContactName%}}", sContactName)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ContactEmail%}}", sContactEmail)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ContactPhone%}}", sContactPhone)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ReminderBlock%}}", "")
        ReminderMailBody = Replace(ReminderMailBody, "{{%OptIn%}}", OptIn)

        dtBuilderReminder = objRDB.GetDataTable("select distinct bldr.builderid, " _
            & " ba.BuilderAccountID, " _
            & " bldr.CompanyName as BuilderName," _
            & " ba.FirstName AS RecipientFirstName, " _
            & " ba.LastName as RecipientLastName, " _
            & " ba.FirstName +' '+ba.Lastname AS RecipientFullName, " _
            & " ba.Email as RecipientEmail " _
        & " from builder bldr join " _
        & " TwoPriceCampaignLLC_Rel  llcRel ON llcRel.LLCId = bldr.LLCID " _
        & " join Twopricebuilderinvitation BldrInvi on BldrInvi.TwoPriceCampaignId = llcRel.TwoPriceCampaignId " _
        & " join builderaccount ba on ba.builderid = bldr.builderid " _
        & " where bldr.builderid not in (select Prttyp.builderid from TwoPriceBuilderParticipation Prttyp where Prttyp.TwoPriceCampaignId=" & RemTwoPricecampaignid_2 & ") " _
        & " and BldrInvi.twopricecampaignid = " & RemTwoPricecampaignid_2 & " and " _
        & " ba.IsActive=1 and ba.email is not null and bldr.IsActive=1")

        Dim BuilderIdList As String = ""
        If dtBuilderReminder.Rows.Count > 0 Then
            Dim i As Integer = 0
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "CpReminderTask"
            dbTaskLog.Status = "GetTwoPriceBuilderReminder2Enroll"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = dtBuilderReminder.Rows.Count.ToString()
            dbTaskLog.Insert()
            For Each row As DataRow In dtBuilderReminder.Rows
                Try
                    cnt += 1
                    Dim AccountMailBody, RemAccountMailSubject, RemAccountMailBody As String
                    Dim sBuilderName, sRecipientFirstName, sRecipientLastName, sRecipientFullName, sRecipientEmail, sBuilderId, sBuilderAccountId As String

                    sBuilderId = dtBuilderReminder.Rows(i)("builderid").ToString()
                    sBuilderAccountId = dtBuilderReminder.Rows(i)("BuilderAccountID").ToString()
                    sBuilderName = dtBuilderReminder.Rows(i)("BuilderName").ToString()
                    sRecipientFirstName = dtBuilderReminder.Rows(i)("RecipientFirstName").ToString()
                    sRecipientLastName = dtBuilderReminder.Rows(i)("RecipientLastName").ToString()
                    sRecipientFullName = dtBuilderReminder.Rows(i)("RecipientFullName").ToString()
                    sRecipientEmail = dtBuilderReminder.Rows(i)("RecipientEmail").ToString()

                    Dim OptOutLink As String = AppSettings("GlobalRefererName") & "/cpmodule/OptOut.aspx?mod=tpc&Tcam=" & Eventid & "&Opt=3&BuilderId=" & sBuilderId & "&BuilderAccountID=" & sBuilderAccountId

                    Dim OptOut As String = "<a style=""padding: 2px 4px; border:1px solid #282927; font-size:12px; line-height:12px;font-weight:400; color:#FFF; background:#cc0000;text-decoration:none;"" href=""" & OptOutLink & """>" & sOptout & "</a>"

                    RemAccountMailSubject = ReminderMailsubject
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%BuilderName%}}", sBuilderName)
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%RecipientFirstName%}}", sRecipientFirstName)
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%RecipientLastName%}}", sRecipientLastName)
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%RecipientFullName%}}", sRecipientFullName)
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%OptOut%}}", OptOut)
                    RemAccountMailSubject = RemAccountMailSubject.Replace("&lt;", "<").Replace("&gt;", ">").Replace("nbsp;", " ").Replace("&amp;", "").Replace("rsquo;", "'")

                    RemAccountMailBody = ReminderMailBody
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%BuilderName%}}", sBuilderName)
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%RecipientFirstName%}}", sRecipientFirstName)
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%RecipientLastName%}}", sRecipientLastName)
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%RecipientFullName%}}", sRecipientFullName)
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%OptOut%}}", OptOut)
                    RemAccountMailBody = RemAccountMailBody.Replace("&lt;", "<").Replace("&gt;", ">").Replace("nbsp;", " ").Replace("&amp;", "").Replace("rsquo;", "'")
                    Dim RemMailBodyStyle As String = "<div style=""border:2px solid #d01e1e;padding:10px;margin-bottom:25px;"">" & RemAccountMailBody & "</div>"

                    AccountMailBody = MailBody
                    AccountMailBody = Replace(AccountMailBody, "{{%BuilderName%}}", sBuilderName)
                    AccountMailBody = Replace(AccountMailBody, "{{%RecipientFirstName%}}", sRecipientFirstName)
                    AccountMailBody = Replace(AccountMailBody, "{{%RecipientLastName%}}", sRecipientLastName)
                    AccountMailBody = Replace(AccountMailBody, "{{%RecipientFullName%}}", sRecipientFullName)
                    AccountMailBody = Replace(AccountMailBody, "{{%OptOut%}}", OptOut)
                    AccountMailBody = Replace(AccountMailBody, "{{%ReminderBlock%}}", RemMailBodyStyle)
                    Dim ReqMailBody As String = AccountMailBody.Replace("&lt;", "<").Replace("&gt;", ">").Replace("nbsp;", " ").Replace("&amp;", "").Replace("rsquo;", "'")

                    Core.SendSimpleMail(sAdminEmail, "CBUSA Customer Service", sRecipientEmail, sBuilderName, RemAccountMailSubject, ReqMailBody, "", "")

                    Dim strBuilderId As String = dtBuilderReminder.Rows(i)("BuilderId").ToString()
                    If BuilderIdList.Contains(strBuilderId) = False Then
                        BuilderIdList = String.Concat(BuilderIdList, ",", dtBuilderReminder.Rows(i)("BuilderId").ToString())
                    End If

                    i = i + 1
                    If i = 1 Then
                        Core.SendSimpleMail(sAdminEmail, "CBUSA Customer Service", sAdminEmail, sContactName, RemAccountMailSubject, ReqMailBody, "", "")
                    End If
                Catch ex As Exception
                    Console.WriteLine(ex)
                    Logger.Error(Logger.GetErrorMessage(ex))
                End Try
            Next
        End If
        Console.WriteLine(cnt & " CpEnrollReminder2 Sent")
        dbTaskLog = New TaskLogRow(DB)
        dbTaskLog.TaskName = "CpReminderTask"
        dbTaskLog.Status = "Completed"
        dbTaskLog.LogDate = Now()
        dbTaskLog.Msg = cnt & " CpEnrollReminder2 Sent, TPC ID - " & RemTwoPricecampaignid_2 & "(" & BuilderIdList & ")"
        dbTaskLog.Insert()
    End Sub
    Public Shared Sub GetTwoPriceBuilderProjectReminder1(ByVal DB As Database, TwoPricecampaignid As String)       'Reminder1 for Project
        Dim objPDB As New Database
        Dim dtBuilderProjReminder As DataTable
        Dim dtBuilderRemProj As DataTable
        Dim dbTaskLog As TaskLogRow = Nothing
        Dim cnt As Integer = 0
        objPDB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))

        dtBuilderRemProj = objPDB.GetDataTable("Select distinct  tpc.Name  as EventTitle, " _
    & " Convert(VARCHAR(10), CAST(tpc.StartDate As Date), 101)  As EventStart, " _
    & " Convert(VARCHAR(10), CAST(tpc.EndDate As Date), 101)  As EventEnd, " _
    & " BldrInvi.EventDescription As EventDescription, " _
    & " BldrInvi.ResponseDeadline  As ResponseDeadline, " _
    & " Format(BldrInvi.ResponseDeadline, 'MMMM dd,yyyy') AS ResponseDeadlineFull, " _
    & " Format(BldrInvi.ResponseDeadline, 'dddd,MM/dd/yyyy') as ResponseDeadlineDay, " _
    & " Format(BldrInvi.ResponseDeadline,'D','en-US') AS ResponseDeadlineFullDay, " _
    & " BldrInvi.InvitationMessage As MailBody, " _
    & " BldrInvi.Reminder1ProjectsSubject as ReminderMailsubject, " _
    & " BldrInvi.Reminder1ProjectsMessage As ReminderMailBody, " _
    & " BldrInvi.InvitationOptInText as OptIn, " _
    & " BldrInvi.InvitationOptOutText AS OptOut, " _
    & " amd.FirstName+' '+ amd.LastName AS ContactName, " _
    & " amd.Email AS ContactEmail, " _
    & " amd.Contact as ContactPhone, " _
    & " amd.Email as AdminEmail, " _
    & " tpc.twopricecampaignid " _
    & " from TwoPriceBuilderInvitation BldrInvi join twopricecampaign tpc on tpc.twopricecampaignid = bldrinvi.twopricecampaignid " _
    & " join admin amd on BldrInvi.PrimaryContact= amd.AdminId " _
    & " where BldrInvi.twopricecampaignid = " & TwoPricecampaignid)

        Dim sEventTitle, sEventStart, sEventEnd, sEventDescription, sResponseDeadline, sResponseDeadlineFull, sResponseDeadlineDay, sResponseDeadlineFullDay, sMailBody, sOptin, sOptout, sContactName, sContactEmail, sContactPhone, sAdminEmail, Eventid, sReminderMailsubject, sReminderMailBody As String
        sEventTitle = dtBuilderRemProj.Rows(0)("EventTitle").ToString()
        sEventStart = dtBuilderRemProj.Rows(0)("EventStart").ToString()
        sEventEnd = dtBuilderRemProj.Rows(0)("EventEnd").ToString()
        sEventDescription = dtBuilderRemProj.Rows(0)("EventDescription").ToString()
        sResponseDeadline = dtBuilderRemProj.Rows(0)("ResponseDeadline").ToString()
        sResponseDeadlineFull = dtBuilderRemProj.Rows(0)("ResponseDeadlineFull").ToString()
        sResponseDeadlineDay = dtBuilderRemProj.Rows(0)("ResponseDeadlineDay").ToString()
        sResponseDeadlineFullDay = dtBuilderRemProj.Rows(0)("ResponseDeadlineFullDay").ToString()
        sMailBody = dtBuilderRemProj.Rows(0)("MailBody").ToString()
        sOptin = dtBuilderRemProj.Rows(0)("OptIn").ToString()
        sOptout = dtBuilderRemProj.Rows(0)("OptOut").ToString()
        sContactName = dtBuilderRemProj.Rows(0)("ContactName").ToString()
        sContactEmail = dtBuilderRemProj.Rows(0)("ContactEmail").ToString()
        sContactPhone = dtBuilderRemProj.Rows(0)("ContactPhone").ToString()
        sAdminEmail = dtBuilderRemProj.Rows(0)("AdminEmail").ToString()
        Eventid = dtBuilderRemProj.Rows(0)("twopricecampaignid").ToString()
        sReminderMailsubject = dtBuilderRemProj.Rows(0)("ReminderMailsubject").ToString()
        sReminderMailBody = dtBuilderRemProj.Rows(0)("ReminderMailBody").ToString()


        Dim OptInLink As String = AppSettings("GlobalRefererName") & "/default.aspx?mod=tpc&Tcam=" & Eventid & "&Opt=1"
        Dim OptIn As String = "<a style=""padding:  2px 4px; border: 1px solid #282927; font-size:12px; line-height:12px;font - weight:400; color:#FFF; background:#38761d;text-decoration:none;"" href=""" & OptInLink & """>" & sOptin & "</a>"

        'replace mailbody to
        Dim MailBody As String = sMailBody
        MailBody = Replace(MailBody, "{{%EventTitle%}}", sEventTitle)
        MailBody = Replace(MailBody, "{{%EventStart%}}", sEventStart)
        MailBody = Replace(MailBody, "{{%EventEnd%}}", sEventEnd)
        MailBody = Replace(MailBody, "{{%EventDescription%}}", sEventDescription)
        MailBody = Replace(MailBody, "{{%ResponseDeadline%}}", sResponseDeadline)
        MailBody = Replace(MailBody, "{{%ResponseDeadlineFull%}}", sResponseDeadlineFull)
        MailBody = Replace(MailBody, "{{%ResponseDeadlineDay%}}", sResponseDeadlineDay)
        MailBody = Replace(MailBody, "{{%ResponseDeadlineFullDay%}}", sResponseDeadlineFullDay)
        MailBody = Replace(MailBody, "{{%OptIn%}}", OptIn)
        MailBody = Replace(MailBody, "{{%ContactName%}}", sContactName)
        MailBody = Replace(MailBody, "{{%ContactEmail%}}", sContactEmail)
        MailBody = Replace(MailBody, "{{%ContactPhone%}}", sContactPhone)

        Dim ReminderMailsubject As String = sReminderMailsubject
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%EventTitle%}}", sEventTitle)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%EventStart%}}", sEventStart)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%EventEnd%}}", sEventEnd)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%EventDescription%}}", sEventDescription)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ResponseDeadline%}}", sResponseDeadline)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ResponseDeadlineFull%}}", sResponseDeadlineFull)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ResponseDeadlineDay%}}", sResponseDeadlineDay)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ResponseDeadlineFullDay%}}", sResponseDeadlineFullDay)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ContactName%}}", sContactName)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ContactEmail%}}", sContactEmail)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ContactPhone%}}", sContactPhone)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ReminderBlock%}}", "")
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%OptIn%}}", OptIn)

        'replace ReminderMailbody to
        Dim ReminderMailBody As String = sReminderMailBody
        ReminderMailBody = Replace(ReminderMailBody, "{{%EventTitle%}}", sEventTitle)
        ReminderMailBody = Replace(ReminderMailBody, "{{%EventStart%}}", sEventStart)
        ReminderMailBody = Replace(ReminderMailBody, "{{%EventEnd%}}", sEventEnd)
        ReminderMailBody = Replace(ReminderMailBody, "{{%EventDescription%}}", sEventDescription)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ResponseDeadline%}}", sResponseDeadline)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ResponseDeadlineFull%}}", sResponseDeadlineFull)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ResponseDeadlineDay%}}", sResponseDeadlineDay)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ResponseDeadlineFullDay%}}", sResponseDeadlineFullDay)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ContactName%}}", sContactName)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ContactEmail%}}", sContactEmail)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ContactPhone%}}", sContactPhone)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ReminderBlock%}}", "")
        ReminderMailBody = Replace(ReminderMailBody, "{{%OptIn%}}", OptIn)

        dtBuilderProjReminder = objPDB.GetDataTable(" Select distinct bldr.builderid, " _
             & " ba.BuilderAccountID,  " _
             & " bldr.CompanyName as BuilderName, " _
             & " ba.FirstName AS RecipientFirstName, " _
             & " ba.LastName as RecipientLastName,  " _
             & " ba.FirstName +' '+ba.Lastname AS RecipientFullName, " _
             & " ba.Email as RecipientEmail  " _
             & " from builder bldr join " _
             & " TwoPriceCampaignLLC_Rel  llcRel On llcRel.LLCId = bldr.LLCID " _
             & " join Twopricebuilderinvitation BldrInvi On BldrInvi.TwoPriceCampaignId = llcRel.TwoPriceCampaignId  " _
             & " join BuilderAccount ba on ba.builderid = bldr.builderid Where bldr.builderid In  " _
             & " (Select builderid from TwoPriceBuilderParticipation where ParticipationType != 3 and ParticipationType != 2 and TwoPriceCampaignId=" & TwoPricecampaignid & ") and " _
             & " bldr.builderid not in (select builderid from twopricebuilderproject where TwoPriceCampaignid=" & TwoPricecampaignid & ") and ba.IsActive=1 and ba.email is not null and bldr.IsActive=1 and BldrInvi.TwoPriceCampaignId=" & TwoPricecampaignid)

        Dim BuilderIdList As String = ""
        If dtBuilderProjReminder.Rows.Count > 0 Then
            Dim i As Integer = 0
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "CpReminderTask"
            dbTaskLog.Status = "GetTwoPriceBuilderProjectReminder1"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = dtBuilderProjReminder.Rows.Count.ToString()
            dbTaskLog.Insert()
            For Each row As DataRow In dtBuilderProjReminder.Rows
                Try
                    cnt += 1
                    Dim AccountMailBody, RemAccountMailSubject, RemAccountMailBody As String
                    Dim sBuilderName, sRecipientFirstName, sRecipientLastName, sRecipientFullName, sRecipientEmail, sBuilderId, sBuilderAccountId As String
                    sBuilderId = dtBuilderProjReminder.Rows(i)("BuilderId").ToString()
                    sBuilderAccountId = dtBuilderProjReminder.Rows(i)("BuilderAccountId").ToString()
                    sBuilderName = dtBuilderProjReminder.Rows(i)("BuilderName").ToString()
                    sRecipientFirstName = dtBuilderProjReminder.Rows(i)("RecipientFirstName").ToString()
                    sRecipientLastName = dtBuilderProjReminder.Rows(i)("RecipientLastName").ToString()
                    sRecipientFullName = dtBuilderProjReminder.Rows(i)("RecipientFullName").ToString()
                    sRecipientEmail = dtBuilderProjReminder.Rows(i)("RecipientEmail").ToString()

                    Dim OptOutLink As String = AppSettings("GlobalRefererName") & "/cpmodule/OptOut.aspx?mod=tpc&Tcam=" & Eventid & "&Opt=3&BuilderId=" & sBuilderId & "&BuilderAccountID=" & sBuilderAccountId

                    Dim OptOut As String = "<a style=""padding: 2px 4px; border:1px solid #282927; font-size:12px; line-height:12px;font - weight:400; color:#FFF; background:#cc0000;text-decoration:none;"" href=""" & OptOutLink & """ > " & sOptout & "</a>"

                    RemAccountMailSubject = ReminderMailsubject
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%BuilderName%}}", sBuilderName)
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%RecipientFirstName%}}", sRecipientFirstName)
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%RecipientLastName%}}", sRecipientLastName)
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%RecipientFullName%}}", sRecipientFullName)
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%OptOut%}}", OptOut)
                    RemAccountMailSubject = RemAccountMailSubject.Replace("&lt;", "<").Replace("&gt;", ">").Replace("nbsp;", " ").Replace("&amp;", "").Replace("rsquo;", "'")

                    RemAccountMailBody = ReminderMailBody
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%BuilderName%}}", sBuilderName)
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%RecipientFirstName%}}", sRecipientFirstName)
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%RecipientLastName%}}", sRecipientLastName)
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%RecipientFullName%}}", sRecipientFullName)
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%OptOut%}}", OptOut)
                    RemAccountMailBody = RemAccountMailBody.Replace("&lt;", "<").Replace("&gt;", ">").Replace("nbsp;", " ").Replace("&amp;", "").Replace("rsquo;", "'")
                    Dim RemMailBodyStyle As String = "<div style=""border:2px solid #d01e1e;padding:10px;margin-bottom: 25px;"">" & RemAccountMailBody & "</div>"

                    AccountMailBody = MailBody
                    AccountMailBody = Replace(AccountMailBody, "{{%BuilderName%}}", sBuilderName)
                    AccountMailBody = Replace(AccountMailBody, "{{%RecipientFirstName%}}", sRecipientFirstName)
                    AccountMailBody = Replace(AccountMailBody, "{{%RecipientLastName%}}", sRecipientLastName)
                    AccountMailBody = Replace(AccountMailBody, "{{%RecipientFullName%}}", sRecipientFullName)
                    AccountMailBody = Replace(AccountMailBody, "{{%OptOut%}}", OptOut)
                    AccountMailBody = Replace(AccountMailBody, "{{%ReminderBlock%}}", RemMailBodyStyle)
                    Dim ReqMailBody As String = AccountMailBody.Replace("&lt;", "<").Replace("&gt;", ">").Replace("nbsp;", " ").Replace("&amp;", "").Replace("rsquo;", "'")

                    Core.SendSimpleMail(sAdminEmail, "CBUSA Customer Service", sRecipientEmail, sBuilderName, RemAccountMailSubject, ReqMailBody, "", "")

                    Dim strBuilderId As String = dtBuilderProjReminder.Rows(i)("BuilderId").ToString()
                    If BuilderIdList.Contains(strBuilderId) = False Then
                        BuilderIdList = String.Concat(BuilderIdList, ",", dtBuilderProjReminder.Rows(i)("BuilderId").ToString())
                    End If

                    i = i + 1
                    If i = 1 Then
                        Core.SendSimpleMail(sAdminEmail, "CBUSA Customer Service", sAdminEmail, sContactName, RemAccountMailSubject, ReqMailBody, "", "")
                    End If
                Catch ex As Exception
                    Console.WriteLine(ex)
                    Logger.Error(Logger.GetErrorMessage(ex))
                End Try
            Next
        End If
        Console.WriteLine(cnt & " CpProjectReminder1 Sent")
        dbTaskLog = New TaskLogRow(DB)
        dbTaskLog.TaskName = "CpReminderTask"
        dbTaskLog.Status = "Completed"
        dbTaskLog.LogDate = Now()
        dbTaskLog.Msg = cnt & " CpProjectReminder1 Sent, TPC ID - " & TwoPricecampaignid & "(" & BuilderIdList & ")"
        dbTaskLog.Insert()
    End Sub
    Public Shared Sub GetTwoPriceBuilderProjectReminder2(ByVal DB As Database, RemTwoPricecampaignid_2 As String)       'Reminder1 for Project
        Dim objPRDB As New Database
        Dim dtBuilderProjReminder2 As DataTable
        Dim dtBuilderRemProj2 As DataTable
        Dim dbTaskLog As TaskLogRow = Nothing
        Dim cnt As Integer = 0
        objPRDB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))

        dtBuilderRemProj2 = objPRDB.GetDataTable("Select distinct  tpc.Name  as EventTitle, " _
& " Convert(VARCHAR(10), CAST(tpc.StartDate As Date), 101)  As EventStart, " _
& " Convert(VARCHAR(10), CAST(tpc.EndDate As Date), 101)  As EventEnd, " _
& " BldrInvi.EventDescription As EventDescription, " _
& " BldrInvi.ResponseDeadline  As ResponseDeadline, " _
& " Format(BldrInvi.ResponseDeadline, 'MMMM dd,yyyy') AS ResponseDeadlineFull, " _
& " Format(BldrInvi.ResponseDeadline, 'dddd,MM/dd/yyyy') as ResponseDeadlineDay, " _
& " Format(BldrInvi.ResponseDeadline,'D','en-US') AS ResponseDeadlineFullDay, " _
& " BldrInvi.InvitationMessage As MailBody, " _
& " BldrInvi.Reminder2ProjectsSubject as ReminderMailsubject, " _
& " BldrInvi.Reminder2ProjectsMessage As ReminderMailBody, " _
& " BldrInvi.InvitationOptInText as OptIn, " _
& " BldrInvi.InvitationOptOutText AS OptOut, " _
& " amd.FirstName+' '+ amd.LastName AS ContactName, " _
& " amd.Email AS ContactEmail, " _
& " amd.Contact as ContactPhone, " _
& " amd.Email as AdminEmail, " _
& " tpc.twopricecampaignid " _
& " from TwoPriceBuilderInvitation BldrInvi join twopricecampaign tpc on tpc.twopricecampaignid = bldrinvi.twopricecampaignid " _
& " join admin amd on BldrInvi.PrimaryContact= amd.AdminId " _
& " where BldrInvi.twopricecampaignid = " & RemTwoPricecampaignid_2)

        Dim sEventTitle, sEventStart, sEventEnd, sEventDescription, sResponseDeadline, sResponseDeadlineFull, sResponseDeadlineDay, sResponseDeadlineFullDay, sMailBody, sOptin, sOptout, sContactName, sContactEmail, sContactPhone, sAdminEmail, Eventid, sReminderMailsubject, sReminderMailBody As String
        sEventTitle = dtBuilderRemProj2.Rows(0)("EventTitle").ToString()
        sEventStart = dtBuilderRemProj2.Rows(0)("EventStart").ToString()
        sEventEnd = dtBuilderRemProj2.Rows(0)("EventEnd").ToString()
        sEventDescription = dtBuilderRemProj2.Rows(0)("EventDescription").ToString()
        sResponseDeadline = dtBuilderRemProj2.Rows(0)("ResponseDeadline").ToString()
        sResponseDeadlineFull = dtBuilderRemProj2.Rows(0)("ResponseDeadlineFull").ToString()
        sResponseDeadlineDay = dtBuilderRemProj2.Rows(0)("ResponseDeadlineDay").ToString()
        sResponseDeadlineFullDay = dtBuilderRemProj2.Rows(0)("ResponseDeadlineFullDay").ToString()
        sMailBody = dtBuilderRemProj2.Rows(0)("MailBody").ToString()
        sOptin = dtBuilderRemProj2.Rows(0)("OptIn").ToString()
        sOptout = dtBuilderRemProj2.Rows(0)("OptOut").ToString()
        sContactName = dtBuilderRemProj2.Rows(0)("ContactName").ToString()
        sContactEmail = dtBuilderRemProj2.Rows(0)("ContactEmail").ToString()
        sContactPhone = dtBuilderRemProj2.Rows(0)("ContactPhone").ToString()
        sAdminEmail = dtBuilderRemProj2.Rows(0)("AdminEmail").ToString()
        Eventid = dtBuilderRemProj2.Rows(0)("twopricecampaignid").ToString()
        sReminderMailsubject = dtBuilderRemProj2.Rows(0)("ReminderMailsubject").ToString()
        sReminderMailBody = dtBuilderRemProj2.Rows(0)("ReminderMailBody").ToString()

        Dim OptInLink As String = AppSettings("GlobalRefererName") & "/default.aspx?mod=tpc&Tcam=" & Eventid & "&Opt=1"
        Dim OptIn As String = "<a style=""padding:  2px 4px; border: 1px solid #282927; font-size:12px; line-height:12px;font - weight:400; color:#FFF; background:#38761d;text-decoration:none;"" href=""" & OptInLink & """>" & sOptin & "</a>"

        'replace mailbody to
        Dim MailBody As String = sMailBody
        MailBody = Replace(MailBody, "{{%EventTitle%}}", sEventTitle)
        MailBody = Replace(MailBody, "{{%EventStart%}}", sEventStart)
        MailBody = Replace(MailBody, "{{%EventEnd%}}", sEventEnd)
        MailBody = Replace(MailBody, "{{%EventDescription%}}", sEventDescription)
        MailBody = Replace(MailBody, "{{%ResponseDeadline%}}", sResponseDeadline)
        MailBody = Replace(MailBody, "{{%ResponseDeadlineFull%}}", sResponseDeadlineFull)
        MailBody = Replace(MailBody, "{{%ResponseDeadlineDay%}}", sResponseDeadlineDay)
        MailBody = Replace(MailBody, "{{%ResponseDeadlineFullDay%}}", sResponseDeadlineFullDay)
        MailBody = Replace(MailBody, "{{%OptIn%}}", OptIn)
        MailBody = Replace(MailBody, "{{%ContactName%}}", sContactName)
        MailBody = Replace(MailBody, "{{%ContactEmail%}}", sContactEmail)
        MailBody = Replace(MailBody, "{{%ContactPhone%}}", sContactPhone)

        Dim ReminderMailsubject As String = sReminderMailsubject
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%EventTitle%}}", sEventTitle)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%EventStart%}}", sEventStart)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%EventEnd%}}", sEventEnd)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%EventDescription%}}", sEventDescription)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ResponseDeadline%}}", sResponseDeadline)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ResponseDeadlineFull%}}", sResponseDeadlineFull)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ResponseDeadlineDay%}}", sResponseDeadlineDay)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ResponseDeadlineFullDay%}}", sResponseDeadlineFullDay)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ContactName%}}", sContactName)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ContactEmail%}}", sContactEmail)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ContactPhone%}}", sContactPhone)
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%ReminderBlock%}}", "")
        ReminderMailsubject = Replace(ReminderMailsubject, "{{%OptIn%}}", OptIn)

        'replace ReminderMailbody to
        Dim ReminderMailBody As String = sReminderMailBody
        ReminderMailBody = Replace(ReminderMailBody, "{{%EventTitle%}}", sEventTitle)
        ReminderMailBody = Replace(ReminderMailBody, "{{%EventStart%}}", sEventStart)
        ReminderMailBody = Replace(ReminderMailBody, "{{%EventEnd%}}", sEventEnd)
        ReminderMailBody = Replace(ReminderMailBody, "{{%EventDescription%}}", sEventDescription)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ResponseDeadline%}}", sResponseDeadline)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ResponseDeadlineFull%}}", sResponseDeadlineFull)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ResponseDeadlineDay%}}", sResponseDeadlineDay)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ResponseDeadlineFullDay%}}", sResponseDeadlineFullDay)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ContactName%}}", sContactName)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ContactEmail%}}", sContactEmail)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ContactPhone%}}", sContactPhone)
        ReminderMailBody = Replace(ReminderMailBody, "{{%ReminderBlock%}}", "")
        ReminderMailBody = Replace(ReminderMailBody, "{{%OptIn%}}", OptIn)

        dtBuilderProjReminder2 = objPRDB.GetDataTable(" Select distinct bldr.builderid, " _
             & " ba.BuilderAccountID,  " _
             & " bldr.CompanyName as BuilderName, " _
             & " ba.FirstName AS RecipientFirstName, " _
             & " ba.LastName as RecipientLastName,  " _
             & " ba.FirstName +' '+ba.Lastname AS RecipientFullName, " _
             & " ba.Email as RecipientEmail  " _
             & " from builder bldr join " _
             & " TwoPriceCampaignLLC_Rel  llcRel On llcRel.LLCId = bldr.LLCID " _
             & " join Twopricebuilderinvitation BldrInvi On BldrInvi.TwoPriceCampaignId = llcRel.TwoPriceCampaignId  " _
             & " join BuilderAccount ba on ba.builderid = bldr.builderid Where bldr.builderid In  " _
             & " (Select builderid from TwoPriceBuilderParticipation where ParticipationType != 3 and ParticipationType != 2 and TwoPriceCampaignId=" & RemTwoPricecampaignid_2 & ") and " _
             & " bldr.builderid not in (select builderid from twopricebuilderproject where TwoPriceCampaignid=" & RemTwoPricecampaignid_2 & ") and ba.IsActive=1 and ba.email is not null and bldr.IsActive=1 and BldrInvi.TwoPriceCampaignId=" & RemTwoPricecampaignid_2)

        Dim BuilderIdList As String = ""
        If dtBuilderProjReminder2.Rows.Count > 0 Then
            Dim i As Integer = 0
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "CpReminderTask"
            dbTaskLog.Status = "GetTwoPriceBuilderProjectReminder2"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = dtBuilderProjReminder2.Rows.Count.ToString()
            dbTaskLog.Insert()
            For Each row As DataRow In dtBuilderProjReminder2.Rows
                Try

                    cnt += 1
                    Dim AccountMailBody, RemAccountMailSubject, RemAccountMailBody As String
                    Dim sBuilderName, sRecipientFirstName, sRecipientLastName, sRecipientFullName, sRecipientEmail, sBuilderId, sBuilderAccountId As String

                    sBuilderId = dtBuilderProjReminder2.Rows(i)("BuilderId").ToString()
                    sBuilderAccountId = dtBuilderProjReminder2.Rows(i)("BuilderAccountId").ToString()
                    sBuilderName = dtBuilderProjReminder2.Rows(i)("BuilderName").ToString()
                    sRecipientFirstName = dtBuilderProjReminder2.Rows(i)("RecipientFirstName").ToString()
                    sRecipientLastName = dtBuilderProjReminder2.Rows(i)("RecipientLastName").ToString()
                    sRecipientFullName = dtBuilderProjReminder2.Rows(i)("RecipientFullName").ToString()
                    sRecipientEmail = dtBuilderProjReminder2.Rows(i)("RecipientEmail").ToString()

                    Dim OptOutLink As String = AppSettings("GlobalRefererName") & "/cpmodule/OptOut.aspx?mod=tpc&Tcam=" & Eventid & "&Opt=3&BuilderId=" & sBuilderId & "&BuilderAccountID=" & sBuilderAccountId

                    Dim OptOut As String = "<a style=""padding: 2px 4px; border:1px solid #282927; font-size:12px; line-height:12px;font-weight:400; color:#FFF; background:#cc0000;text-decoration:none;"" href=""" & OptOutLink & """>" & sOptout & "</a>"

                    RemAccountMailSubject = ReminderMailsubject
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%BuilderName%}}", sBuilderName)
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%RecipientFirstName%}}", sRecipientFirstName)
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%RecipientLastName%}}", sRecipientLastName)
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%RecipientFullName%}}", sRecipientFullName)
                    RemAccountMailSubject = Replace(RemAccountMailSubject, "{{%OptOut%}}", OptOut)
                    RemAccountMailSubject = RemAccountMailSubject.Replace("&lt;", "<").Replace("&gt;", ">").Replace("nbsp;", " ").Replace("&amp;", "").Replace("rsquo;", "'")

                    RemAccountMailBody = ReminderMailBody
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%BuilderName%}}", sBuilderName)
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%RecipientFirstName%}}", sRecipientFirstName)
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%RecipientLastName%}}", sRecipientLastName)
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%RecipientFullName%}}", sRecipientFullName)
                    RemAccountMailBody = Replace(RemAccountMailBody, "{{%OptOut%}}", OptOut)
                    RemAccountMailBody = RemAccountMailBody.Replace("&lt;", "<").Replace("&gt;", ">").Replace("nbsp;", " ").Replace("&amp;", "").Replace("rsquo;", "'")
                    Dim RemMailBodyStyle As String = "<div style=""border:2px solid #d01e1e;padding:10px;margin-bottom: 25px;"">" & RemAccountMailBody & "</div>"

                    AccountMailBody = MailBody
                    AccountMailBody = Replace(AccountMailBody, "{{%BuilderName%}}", sBuilderName)
                    AccountMailBody = Replace(AccountMailBody, "{{%RecipientFirstName%}}", sRecipientFirstName)
                    AccountMailBody = Replace(AccountMailBody, "{{%RecipientLastName%}}", sRecipientLastName)
                    AccountMailBody = Replace(AccountMailBody, "{{%RecipientFullName%}}", sRecipientFullName)
                    AccountMailBody = Replace(AccountMailBody, "{{%OptOut%}}", OptOut)
                    AccountMailBody = Replace(AccountMailBody, "{{%ReminderBlock%}}", RemMailBodyStyle)
                    Dim ReqMailBody As String = AccountMailBody.Replace("&lt;", "<").Replace("&gt;", ">").Replace("nbsp;", " ").Replace("&amp;", "").Replace("rsquo;", "'")

                    Core.SendSimpleMail(sAdminEmail, "CBUSA Customer Service", sRecipientEmail, sBuilderName, RemAccountMailSubject, ReqMailBody, "", "")

                    Dim strBuilderId As String = dtBuilderProjReminder2.Rows(i)("BuilderId").ToString()
                    If BuilderIdList.Contains(strBuilderId) = False Then
                        BuilderIdList = String.Concat(BuilderIdList, ",", dtBuilderProjReminder2.Rows(i)("BuilderId").ToString())
                    End If

                    i = i + 1
                    If i = 1 Then
                        Core.SendSimpleMail(sAdminEmail, "CBUSA Customer Service", sAdminEmail, sContactName, RemAccountMailSubject, ReqMailBody, "", "")
                    End If
                Catch ex As Exception
                    Console.WriteLine(ex)
                    Logger.Error(Logger.GetErrorMessage(ex))
                End Try
            Next
        End If
        Console.WriteLine(cnt & " CpProjectReminder Sent")
        dbTaskLog = New TaskLogRow(DB)
        dbTaskLog.TaskName = "CpReminderTask"
        dbTaskLog.Status = "Completed"
        dbTaskLog.LogDate = Now()
        dbTaskLog.Msg = cnt & " CpProjectReminder Sent, TPC ID - " & RemTwoPricecampaignid_2 & "(" & BuilderIdList & ")"
        dbTaskLog.Insert()
    End Sub
End Class



