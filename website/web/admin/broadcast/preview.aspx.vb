Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports FredCK.FCKeditorV2
Imports System.Data.SqlClient

Partial Class Preview
    Inherits AdminPage

    Protected MessageId As Integer
    Protected BackURL As String

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BROADCAST")

        MessageId = Request("MessageId")
        BackURL = Request("BackUrl")
        If InStr(BackURL, "/view.aspx") >= 0 Then
            PreviewCtrl.ViewOnly = True
        End If

        PreviewCtrl.SaveText = AddressOf SaveText
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        Dim dbMailingMessage As MailingMessageRow = MailingMessageRow.GetRow(DB, MessageId)

        ltlName.Text = dbMailingMessage.Name

        PreviewCtrl.Slots = dbMailingMessage.GetSlots
        PreviewCtrl.TargetType = dbMailingMessage.TargetType
        PreviewCtrl.TemplateId = dbMailingMessage.TemplateId
        PreviewCtrl.MessageId = dbMailingMessage.MessageId
        PreviewCtrl.MimeType = dbMailingMessage.MimeType
        PreviewCtrl.NewsletterDate = dbMailingMessage.NewsletterDate
        PreviewCtrl.SavedText = dbMailingMessage.SavedText
    End Sub

    Private Sub SaveText(ByVal value As String)
        Dim dbMailingMessage As MailingMessageRow = MailingMessageRow.GetRow(DB, MessageId)
        dbMailingMessage.SavedText = value
        dbMailingMessage.Update()
    End Sub

    Protected Sub lnkBack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkBack.Click
        Response.Redirect(BackURL & "?MessageId=" & MessageId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class