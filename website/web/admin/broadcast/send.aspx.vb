Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager

Partial Class SendSchedule
    Inherits AdminPage

    Protected MessageId As Integer
    Protected Status As String = "NEW"

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BROADCAST")

        MessageId = Request("MessageId")
        MailingSummaryCtrl.MessageId = MessageId
        If Not IsPostBack Then
            Dim dbMailingMessage As MailingMessageRow = MailingMessageRow.GetRow(DB, MessageId)
            Steps.Step1 = dbMailingMessage.Step1
            Steps.Step2 = dbMailingMessage.Step2
            Steps.Step3 = dbMailingMessage.Step3
            Steps.MessageId = dbMailingMessage.MessageId
        End If
    End Sub

    Protected Sub btnContinue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnContinue.Click
        If Not IsValid Then Exit Sub

        Dim dbMailingMessage As MailingMessageRow = MailingMessageRow.GetRow(DB, MessageId)

        Dim oLyris As New Lyris
        oLyris.NewsletterDate = dbMailingMessage.NewsletterDate
        oLyris.SavedText = dbMailingMessage.SavedText

        Dim dbTemplate As MailingTemplateRow = MailingTemplateRow.GetRow(DB, dbMailingMessage.TemplateId)
        If dbMailingMessage.TargetType = "DYNAMIC" Then
            oLyris.HTMLTemplate = dbTemplate.HTMLDynamic
            oLyris.TextTemplate = dbTemplate.TextDynamic
        Else
            oLyris.HTMLTemplate = dbTemplate.HTMLMember
            oLyris.TextTemplate = dbTemplate.TextMember
        End If
        oLyris.MessageSlots = dbMailingMessage.GetSlots()
        oLyris.TemplateSlots = dbTemplate.GetSlots()
        oLyris.MIMEType = dbMailingMessage.MimeType
        oLyris.GenerateMessages(True)

        Try
            DB.BeginTransaction()

            'SAVE TRACKED LINKS IN THE MAILING_LINK TABLE
            SQL = "DELETE FROM MailingLinkHit WHERE MessageId = " & MessageId
            DB.ExecuteSQL(SQL)

            'SAVE TRACKED LINKS IN THE MAILING_LINK TABLE
            SQL = "DELETE FROM MailingLink WHERE MessageId = " & MessageId
            DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM MailingRecipient WHERE MessageId = " & MessageId
            DB.ExecuteSQL(SQL)

            'SAVE LINKS
            For i As Integer = 0 To oLyris.Links.Count - 1
                Dim link As LyrisLink = oLyris.Links(i)
                If Not link.Href = String.Empty Then
                    Dim href As String = Replace(link.Href, "&amp;", "&")
                    SQL = "INSERT INTO MailingLink (LinkId, MessageId, Link, MimeType) VALUES (" & i + 1 & "," & MessageId & "," & DB.Quote(href) & "," & DB.Quote(link.Type.ToString) & ")"
                    DB.ExecuteSQL(SQL)
                End If
            Next

            'SAVE RECIPIENTS
            Dim dbMailingGroup As MailingGroupRow = MailingGroupRow.GetRow(DB, dbMailingMessage.GroupId)
            Dim col As New NameValueCollection
            col("Lists") = dbMailingGroup.GetSelectedMailingLists
            col("StartDate") = IIf(dbMailingGroup.StartDate = Nothing, "", dbMailingGroup.StartDate)
            col("EndDate") = IIf(dbMailingGroup.EndDate = Nothing, "", dbMailingGroup.EndDate)

            'HTML Message
            Dim HTMLQuery As String = String.Empty
            Dim HTMLQueryLyris As String = String.Empty
            If dbMailingMessage.MimeType = "BOTH" Or dbMailingMessage.MimeType = "HTML" Then
                HTMLQuery = MailingHelper.GetHTMLQuery(DB, col, MessageId, dbMailingMessage.MimeType)
                HTMLQuery = "INSERT INTO MailingRecipient (MessageId, MemberId, Email, FullName, MimeType) " & HTMLQuery
                DB.ExecuteSQL(HTMLQuery, 60)

                'REMOVE ALL DUPLICATE EMAILS FOR THIS MESSAGE
                SQL = "exec sp_MailingRemoveDuplicates " & MessageId & "," & DB.Quote("HTML")
                Call DB.ExecuteSQL(SQL, 60)

                HTMLQueryLyris = "SELECT MemberId, Email, FullName, NULL AS ADDITIONAL, NULL AS COMMENT FROM " & System.Configuration.ConfigurationManager.AppSettings("LyrisDatabaseLocation") & "." & System.Configuration.ConfigurationManager.AppSettings("LyrisTableOwner") & ".MailingRecipient WHERE MessageId = " & MessageId & " AND MimeType = 'HTML'"
            End If

            'Text Message
            Dim TextQuery As String = String.Empty
            Dim TextQueryLyris As String = String.Empty
            If dbMailingMessage.MimeType = "BOTH" Or dbMailingMessage.MimeType = "TEXT" Then
                TextQuery = MailingHelper.GetTextQuery(DB, col, MessageId, dbMailingMessage.MimeType)
                TextQuery = "INSERT INTO MailingRecipient (MessageId, MemberId, Email, FullName, MimeType) " & TextQuery
                DB.ExecuteSQL(TextQuery, 60)

                'REMOVE ALL DUPLICATE EMAILS FOR THIS MESSAGE
                SQL = "exec sp_MailingRemoveDuplicates " & MessageId & "," & DB.Quote("TEXT")
                Call DB.ExecuteSQL(SQL, 60)

                TextQueryLyris = "SELECT MemberId, Email, FullName, NULL AS ADDITIONAL, NULL AS COMMENT FROM " & System.Configuration.ConfigurationManager.AppSettings("LyrisDatabaseLocation") & "." & System.Configuration.ConfigurationManager.AppSettings("LyrisTableOwner") & ".MailingRecipient WHERE MessageId = " & MessageId & " AND MimeType = 'TEXT'"
            End If

            Dim ScheduledDateTime As String = String.Empty
            If Not ScheduledDate.Value = Nothing Then
                dbMailingMessage.ScheduledDate = ScheduledDate.Value & " " & ScheduledTime.Text
                dbMailingMessage.Status = "SCHEDULED"
                ScheduledDateTime = ScheduledDate.Value & " " & ScheduledTime.Text
            Else
                dbMailingMessage.SentDate = Now()
                dbMailingMessage.Status = "SENT"
            End If

            dbMailingMessage.ListHTMLId = Nothing
            dbMailingMessage.ListTextId = Nothing
            dbMailingMessage.MessageHTML = oLyris.HTMLMessage
            dbMailingMessage.MessageText = oLyris.TextMessage
            dbMailingMessage.HTMLQuery = HTMLQuery
            dbMailingMessage.HTMLLyrisQuery = HTMLQueryLyris
            dbMailingMessage.TextQuery = TextQuery
            dbMailingMessage.TextLyrisQuery = TextQueryLyris
            dbMailingMessage.ListPrefix = System.Configuration.ConfigurationManager.AppSettings("LyrisListName")
            dbMailingMessage.ModifyAdminId = LoggedInAdminId
            dbMailingMessage.Update()

            'Insert records to lyris queue

            Dim DBLyris As New Database
            DBLyris.Open(AppSettings("LyrisQueueConnectionString"))

            If dbMailingMessage.MimeType = "BOTH" Or dbMailingMessage.MimeType = "HTML" Then
                SQL = " INSERT INTO LYRIS_QUEUE (MESSAGE_ID, LIST_PREFIX, FROM_EMAIL, FROM_NAME, REPLY_EMAIL, SUBJECT, BODY, [DATABASE], STATUS, CREATE_DATE, SCHEDULE_DATE, MIME_TYPE, [SQL], VERSION) VALUES (" _
                    & MessageId & "," _
                    & DB.Quote(System.Configuration.ConfigurationManager.AppSettings("LyrisListName")) & "," _
                    & DB.Quote(dbMailingMessage.FromEmail) & "," _
                    & DB.Quote(dbMailingMessage.FromName) & "," _
                    & DB.Quote(dbMailingMessage.ReplyEmail) & "," _
                    & DB.Quote(dbMailingMessage.Subject) & "," _
                    & DB.Quote(dbMailingMessage.MessageHTML) & "," _
                    & DB.Quote(System.Configuration.ConfigurationManager.AppSettings("LyrisDatabaseLocation")) & "," _
                    & DB.Quote(Status) & "," _
                    & DB.Quote(Now()) & "," _
                    & DB.Quote(ScheduledDateTime) & "," _
                    & DB.Quote("HTML") & "," _
                    & DB.Quote(HTMLQueryLyris) & "," _
                    & DB.Quote("NET 6.1") _
                    & ")"

                DBLyris.ExecuteSQL(SQL)
            End If

            If dbMailingMessage.MimeType = "BOTH" Or dbMailingMessage.MimeType = "TEXT" Then
                SQL = " INSERT INTO LYRIS_QUEUE (MESSAGE_ID, LIST_PREFIX, FROM_EMAIL, FROM_NAME, REPLY_EMAIL, SUBJECT, BODY, [DATABASE], STATUS, CREATE_DATE, SCHEDULE_DATE, MIME_TYPE, [SQL], VERSION) VALUES (" _
                    & MessageId & "," _
                    & DB.Quote(System.Configuration.ConfigurationManager.AppSettings("LyrisListName")) & "," _
                    & DB.Quote(dbMailingMessage.FromEmail) & "," _
                    & DB.Quote(dbMailingMessage.FromName) & "," _
                    & DB.Quote(dbMailingMessage.ReplyEmail) & "," _
                    & DB.Quote(dbMailingMessage.Subject) & "," _
                    & DB.Quote(dbMailingMessage.MessageText) & "," _
                    & DB.Quote(System.Configuration.ConfigurationManager.AppSettings("LyrisDatabaseLocation")) & "," _
                    & DB.Quote(Status) & "," _
                    & DB.Quote(Now()) & "," _
                    & DB.Quote(ScheduledDateTime) & "," _
                    & DB.Quote("TEXT") & "," _
                    & DB.Quote(TextQueryLyris) & "," _
                    & DB.Quote("NET 6.1") _
                    & ")"

                DBLyris.ExecuteSQL(SQL)
            End If

            DbLyris.Close()

            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        End Try

    End Sub

    Protected Sub valCustom_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles valCustom.ServerValidate
        If ScheduledDate.Text = String.Empty AndAlso ScheduledTime.Text = String.Empty Then
            args.IsValid = True
            Exit Sub
        End If

        If Not ScheduledDate.Text = String.Empty AndAlso Not ScheduledTime.Text = String.Empty Then
            args.IsValid = True
            Exit Sub
        End If

        args.IsValid = False
    End Sub
End Class