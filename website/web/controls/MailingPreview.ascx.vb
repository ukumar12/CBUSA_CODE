Imports Components
Imports Utility
Imports System.Data
Imports DataLayer

Partial Class MailingPreview
    Inherits BaseControl

    Public Delegate Sub SaveTextHandler(ByVal value As String)
    Public SaveText As SaveTextHandler

    Public Property ViewOnly() As Boolean
        Get
            If ViewState("ViewOnly") Is Nothing Then ViewState("ViewOnly") = False
            Return ViewState("ViewOnly")
        End Get
        Set(ByVal value As Boolean)
            ViewState("ViewOnly") = value
        End Set
    End Property

    Public Property SavedText() As String
        Get
            If ViewState("SavedText") Is Nothing Then ViewState("SavedText") = String.Empty
            Return ViewState("SavedText")
        End Get
        Set(ByVal value As String)
            ViewState("SavedText") = value
        End Set
    End Property

    Public Property NewsletterDate() As String
        Get
            Return ViewState("NewsletterDate")
        End Get
        Set(ByVal value As String)
            ViewState("NewsletterDate") = value
        End Set
    End Property

    Public Property Slots() As GenericSerializableCollection(Of MailingMessageSlot)
        Get
            Return ViewState("Slots")
        End Get
        Set(ByVal value As GenericSerializableCollection(Of MailingMessageSlot))
            ViewState("Slots") = value
        End Set
    End Property

    Public Property MessageId() As Integer
        Get
            If ViewState("MessageId") Is Nothing Then ViewState("MessageId") = 0
            Return ViewState("MessageId")
        End Get
        Set(ByVal value As Integer)
            ViewState("MessageId") = value
        End Set
    End Property

    Public Property TargetType() As String
        Get
            If ViewState("TargetType") Is Nothing Then ViewState("TargetType") = String.Empty
            Return ViewState("TargetType")
        End Get
        Set(ByVal value As String)
            ViewState("TargetType") = value
        End Set
    End Property

    Public Property TemplateId() As Integer
        Get
            If ViewState("TemplateId") Is Nothing Then ViewState("TemplateId") = 0
            Return ViewState("TemplateId")
        End Get
        Set(ByVal value As Integer)
            ViewState("TemplateId") = value
        End Set
    End Property

    Protected Property FileName() As String
        Get
            If ViewState("FileName") Is Nothing Then ViewState("FileName") = String.Empty
            Return ViewState("FileName")
        End Get
        Set(ByVal value As String)
            ViewState("FileName") = value
        End Set
    End Property

    Public Property MimeType() As String
        Get
            If ViewState("MimeType") Is Nothing Then ViewState("MimeType") = String.Empty
            Return ViewState("MimeType")
        End Get
        Set(ByVal value As String)
            ViewState("MimeType") = value
        End Set
    End Property

    Protected Property PreviewFormat() As String
        Get
            If ViewState("PreviewFormat") Is Nothing Then ViewState("PreviewFormat") = String.Empty
            Return ViewState("PreviewFormat")
        End Get
        Set(ByVal value As String)
            ViewState("PreviewFormat") = value
        End Set
    End Property

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not Visible Then Exit Sub

        If PreviewFormat = String.Empty Then
            If MimeType = "HTML" Or MimeType = "BOTH" Then PreviewFormat = "HTML" Else PreviewFormat = "TEXT"
        End If

        divHTML.Visible = False
        divText.Visible = False
        lnkHTML.CssClass = "L1"
        lnkText.CssClass = "L1"

        If PreviewFormat = "HTML" Then
            GenerateHTMLFile()
            divHTML.Visible = True
            lnkHTML.CssClass = "L3"
        End If
        If PreviewFormat = "TEXT" Then
            If SavedText = String.Empty Then
                txtText.Value = GenerateText()
            Else
                txtText.Value = SavedText
            End If
            divText.Visible = True
            lnkText.CssClass = "L3"
        End If

        If SavedText = String.Empty Then
            divSavedText.Visible = False
            divGeneratedText.Visible = True
        Else
            divSavedText.Visible = True
            divGeneratedText.Visible = False
        End If

        If ViewOnly = True Then
            divSavedText.Visible = False
            divGeneratedText.Visible = False
            btnSave.Visible = False
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Private Sub GenerateHTMLFile()
        Dim oLyris As New Lyris
        Dim dbTemplate As MailingTemplateRow = MailingTemplateRow.GetRow(DB, TemplateId)
        If TargetType = "DYNAMIC" Then
            oLyris.HTMLTemplate = dbTemplate.HTMLDynamic
            oLyris.TextTemplate = dbTemplate.TextDynamic
        Else
            oLyris.HTMLTemplate = dbTemplate.HTMLMember
            oLyris.TextTemplate = dbTemplate.TextMember
        End If
        oLyris.NewsletterDate = NewsletterDate
        oLyris.MessageSlots = Slots()
        oLyris.TemplateSlots = dbTemplate.GetSlots()
        oLyris.MIMEType = "HTML"
        oLyris.GenerateMessages(False)

        FileName = MailingHelper.CreateTempFile(Server.MapPath("/assets/temp/"), oLyris.HTMLMessage, "htm")
    End Sub

    Private Function GenerateText() As String
        Dim oLyris As New Lyris
        Dim dbTemplate As MailingTemplateRow = MailingTemplateRow.GetRow(DB, TemplateId)

        oLyris.SavedText = SavedText
        If TargetType = "DYNAMIC" Then
            oLyris.HTMLTemplate = dbTemplate.HTMLDynamic
            oLyris.TextTemplate = dbTemplate.TextDynamic
        Else
            oLyris.HTMLTemplate = dbTemplate.HTMLMember
            oLyris.TextTemplate = dbTemplate.TextMember
        End If
        oLyris.NewsletterDate = NewsletterDate
        oLyris.MessageSlots = Slots()
        oLyris.TemplateSlots = dbTemplate.GetSlots()
        oLyris.MIMEType = "TEXT"
        oLyris.GenerateMessages(False)

        Return oLyris.TextMessage
    End Function

    Protected Sub lnkHTML_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkHTML.Click
        PreviewFormat = "HTML"
    End Sub

    Protected Sub lnkText_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkText.Click
        PreviewFormat = "TEXT"
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        SavedText = txtText.Value
        If Not SaveText Is Nothing Then
            SaveText(SavedText)
        End If
    End Sub

    Protected Sub btnReload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReload.Click
        SavedText = String.Empty
        txtText.Value = GenerateText()
        If Not SaveText Is Nothing Then
            SaveText(SavedText)
        End If
    End Sub
End Class
