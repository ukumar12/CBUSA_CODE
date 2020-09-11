Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient

Partial Class Review
    Inherits AdminPage

    Protected MessageId As Integer

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

        Dim oLyris As New Lyris
        Dim dbMailingMessage As MailingMessageRow = MailingMessageRow.GetRow(DB, MessageId)
        Dim dbTemplate As MailingTemplateRow = MailingTemplateRow.GetRow(DB, dbMailingMessage.TemplateId)

        oLyris.NewsletterDate = dbMailingMessage.NewsletterDate
        oLyris.SavedText = dbMailingMessage.SavedText
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

            'INSERT PREVIEW MESSAGE - THIS IS EXACT COPY OF CURRENT MESSAGE
            SQL = " INSERT INTO MailingMessage (" _
              & " ParentId, ListPrefix, NewsletterDate, Step1, Step2, Step3, GroupId, Name, TemplateId, MimeType, FromEmail, FromName, ReplyEmail, SentDate, Subject, ScheduledDate, Status, CreateDate, CreateAdminId, ModifyDate, ModifyAdminId, " _
              & " HTMLQuery, HTMLLyrisQuery, TextQuery, TextLyrisQuery, TargetType, MessageHTML, MessageText, SavedText" _
              & " ) SELECT " _
              & " MessageId, ListPrefix, NewsletterDate, Step1, Step2, Step3, GroupId, Name, TemplateId, MimeType, FromEmail, FromName, ReplyEmail, SentDate, Subject, ScheduledDate, Status, CreateDate, CreateAdminId, " & DB.Quote(Now()) & ", " & LoggedInAdminId & ", " _
              & " HTMLQuery, HTMLLyrisQuery, TextQuery, TextLyrisQuery, TargetType, MessageHTML, MessageText, SavedText" _
              & " FROM MailingMessage WHERE MessageId = " & MessageId

            Dim PreviewId As Integer = DB.InsertSQL(SQL)

            'SAVE LINKS
            For i As Integer = 0 To oLyris.Links.Count - 1
                Dim link As LyrisLink = oLyris.Links(i)
                If Not link.Href = String.Empty Then
                    Dim href As String = Replace(link.Href, "&amp;", "&")
                    SQL = "INSERT INTO MailingLink (LinkId, MessageId, Link, MimeType) VALUES (" & i + 1 & "," & PreviewId & "," & DB.Quote(href) & "," & DB.Quote(link.Type.ToString) & ")"
                    DB.ExecuteSQL(SQL)
                End If
            Next

            'Send HTML preview
            Dim ListHTMLId As Integer = 0
            If dbMailingMessage.MimeType = "BOTH" Or dbMailingMessage.MimeType = "HTML" Then
                Dim ListHTMLName As String = System.Configuration.ConfigurationManager.AppSettings("LyrisListName") & "_" & MessageId & "_TEST_HTML_" & Lyris.GetDateString(Now())
                ListHTMLId = Lyris.AddList(ListHTMLName, ListHTMLName)
                Lyris.AddMember(ListHTMLName, 0, txtPreviewName.Text, txtPreviewEmail.Text, True, txtPreviewEmail.Text, 0)
                Lyris.SendHTMLMessage(ListHTMLName, dbMailingMessage.FromName, dbMailingMessage.FromEmail, dbMailingMessage.ReplyEmail, oLyris.HTMLMessage, dbMailingMessage.Subject, DateAdd("h", -1, Now()))
            End If

            'Send Text preview
            Dim ListTextId As Integer = 0
            If dbMailingMessage.MimeType = "BOTH" Or dbMailingMessage.MimeType = "TEXT" Then
                Dim ListTextName As String = System.Configuration.ConfigurationManager.AppSettings("LyrisListName") & "_" & MessageId & "_TEST_TEXT_" & Lyris.GetDateString(Now())
                ListTextId = Lyris.AddList(ListTextName, ListTextName)
                Lyris.AddMember(ListTextName, 0, txtPreviewName.Text, txtPreviewEmail.Text, True, txtPreviewEmail.Text, 0)
                Lyris.SendTextMessage(ListTextName, dbMailingMessage.FromName, dbMailingMessage.FromEmail, dbMailingMessage.ReplyEmail, oLyris.TextMessage, dbMailingMessage.Subject, DateAdd("h", -1, Now()))
            End If

            dbMailingMessage.MessageHTML = oLyris.HTMLMessage
            dbMailingMessage.MessageText = oLyris.TextMessage
            dbMailingMessage.ListPrefix = System.Configuration.ConfigurationManager.AppSettings("LyrisListName")
            dbMailingMessage.Step3 = True
            dbMailingMessage.ModifyAdminId = LoggedInAdminId
            dbMailingMessage.Update()

            SQL = " UPDATE MailingMessage SET " _
                & " ListPrefix = " & DB.Quote(dbMailingMessage.ListPrefix) & "," _
                & " ListHTMLId = " & DB.Number(ListHTMLId) & "," _
                & " ListTextId = " & DB.Number(ListTextId) & "," _
                & " MessageHTML = " & DB.Quote(dbMailingMessage.MessageHTML) & "," _
                & " MessageText = " & DB.Quote(dbMailingMessage.MessageText) & "," _
                & " Step3 = 1," _
                & " ModifyAdminId = " & LoggedInAdminId & "," _
                & " ModifyDate = " & DB.Quote(Now()) _
                & " WHERE MessageId = " & PreviewId

            DB.ExecuteSQL(SQL)

            DB.CommitTransaction()

            Response.Redirect("send.aspx?MessageId=" & MessageId & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

End Class