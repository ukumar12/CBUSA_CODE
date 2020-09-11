Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient

Partial Class Layout
    Inherits AdminPage

    Protected Property MessageId() As Integer
        Get
            If ViewState("MessageId") Is Nothing Then ViewState("MessageId") = 0
            Return ViewState("MessageId")
        End Get
        Set(ByVal value As Integer)
            ViewState("MessageId") = value
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

    Public Property TargetType() As String
        Get
            If ViewState("TargetType") Is Nothing Then ViewState("TargetType") = String.Empty
            Return ViewState("TargetType")
        End Get
        Set(ByVal value As String)
            ViewState("TargetType") = value
        End Set
    End Property

    Protected Property TemplateId() As Integer
        Get
            If ViewState("TemplateId") Is Nothing Then ViewState("TemplateId") = 0
            Return ViewState("TemplateId")
        End Get
        Set(ByVal value As Integer)
            ViewState("TemplateId") = value
        End Set
    End Property

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BROADCAST")

        PreviewCtrl.SaveText = AddressOf SaveText
        If Not IsPostBack Then
            LoadFromDB()
        End If
        RefreshControls()
    End Sub

    Public Function Save() As Boolean
        Try
            DB.BeginTransaction()

            Dim dbMessage As MailingMessageRow
            If MessageId <> 0 Then
                dbMessage = MailingMessageRow.GetRow(DB, MessageId)
            Else
                dbMessage = New MailingMessageRow(DB)
                dbMessage.CreateAdminId = LoggedInAdminId
                dbMessage.TargetType = TargetType
                dbMessage.TemplateId = TemplateId
            End If

            dbMessage.ModifyAdminId = LoggedInAdminId
            dbMessage.Name = txtName.Text
            dbMessage.FromEmail = txtFromEmail.Text
            dbMessage.FromName = txtFromName.Text
            dbMessage.MimeType = rblMimeType.SelectedValue
            dbMessage.NewsletterDate = dtNewsletterDate.Value
            dbMessage.ReplyEmail = txtReplyEmail.Text
            dbMessage.Subject = txtSubject.Text
            dbMessage.Status = "SAVED"
            dbMessage.Step1 = True
            dbMessage.SavedText = SavedText

            If MessageId <> 0 Then
                dbMessage.Update()
            Else
                MessageId = dbMessage.Insert()
            End If

            DB.CommitTransaction()
            DB.BeginTransaction()

            'Save Slots
            MailingMessageSlotRow.RemoveByMessage(DB, MessageId)

            Dim Counter As Integer = 0
            For Each row As RepeaterItem In rptSlots.Items
                Dim txtHeadline As System.Web.UI.WebControls.TextBox = CType(row.FindControl("txtHeadline"), System.Web.UI.WebControls.TextBox)
                Dim Text As String = String.Empty

                Counter += 1
                If rblMimeType.SelectedValue = "HTML" Or rblMimeType.SelectedValue = "BOTH" Then
                    Dim htmSlot As CKEditor = row.FindControl("htmSlot")
                    Text = htmSlot.Value
                    Text = Replace(Text, System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/UserFiles/", "/UserFiles/")
                    Text = Replace(Text, "/UserFiles/", System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/UserFiles/")
                Else
                    Dim txtSlot As System.Web.UI.WebControls.TextBox = CType(row.FindControl("txtSlot"), System.Web.UI.WebControls.TextBox)
                    Text = txtHeadline.Text
                End If
                Dim dbSlot As New MailingMessageSlotRow(DB)
                dbSlot.Headline = txtHeadline.Text
                dbSlot.MessageId = MessageId
                dbSlot.Slot = Text
                dbSlot.SortOrder = Counter
                dbSlot.Insert()
            Next

            DB.CommitTransaction()

            Return True

        Catch ex As Exception
            AddError(ErrHandler.ErrorText(ex))
            Return False
        End Try

    End Function

    Protected Sub btnContinue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnContinue.Click
        If Not IsValid Then Exit Sub
        If Save() Then
            Response.Redirect("target.aspx?MessageId=" & MessageId & "&" & GetPageParams(FilterFieldType.All))
        End If
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub

        If Save() Then
            Response.Redirect("default.aspx?MessageId=" & MessageId & "&" & GetPageParams(FilterFieldType.All))
        End If
    End Sub

    Private Sub LoadFromDB()
        MessageId = Request("MessageId")

        Dim dbMessage As MailingMessageRow = MailingMessageRow.GetRow(DB, MessageId)
        txtName.Text = dbMessage.Name
        txtFromEmail.Text = dbMessage.FromEmail
        txtFromName.Text = dbMessage.FromName
        rblMimeType.SelectedValue = dbMessage.MimeType
        dtNewsletterDate.Value = dbMessage.NewsletterDate
        txtReplyEmail.Text = dbMessage.ReplyEmail
        txtSubject.Text = dbMessage.Subject
        Steps.Step1 = dbMessage.Step1
        Steps.Step2 = dbMessage.Step2
        Steps.Step3 = dbMessage.Step3
        Steps.MessageId = MessageId
        SavedText = dbMessage.SavedText

        If MessageId = 0 Then
            rblMimeType.SelectedValue = "BOTH"
            dtNewsletterDate.Text = Date.Today
            TargetType = Request("TargetType")
            TemplateId = Request("TemplateId")
        Else
            TargetType = dbMessage.TargetType
            TemplateId = dbMessage.TemplateId
        End If

        rptSlots.DataSource = MailingMessageSlotRow.GetSlotsByMessage(DB, TemplateId, MessageId)
        rptSlots.DataBind()
    End Sub

    Protected Sub RefreshControls()
        For Each row As RepeaterItem In rptSlots.Items
            Dim htmSlot As CKEditor = row.FindControl("htmSlot")
            Dim txtSlot As System.Web.UI.WebControls.TextBox = CType(row.FindControl("txtSlot"), System.Web.UI.WebControls.TextBox)
            If rblMimeType.SelectedValue = "HTML" Or rblMimeType.SelectedValue = "BOTH" Then
                htmSlot.Visible = True
                txtSlot.Visible = False
            Else
                htmSlot.Visible = False
                txtSlot.Visible = True
            End If
        Next
    End Sub

    Protected Sub rptSlots_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptSlots.ItemDataBound
        If Not e.Item.ItemType = ListItemType.Item And Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim Headline As String = IIf(IsDBNull(e.Item.DataItem("Headline")), String.Empty, e.Item.DataItem("Headline"))
        Dim Slot As String = IIf(IsDBNull(e.Item.DataItem("Slot")), String.Empty, e.Item.DataItem("Slot"))

        Dim txtHeadline As System.Web.UI.WebControls.TextBox = CType(e.Item.FindControl("txtHeadline"), System.Web.UI.WebControls.TextBox)
        txtHeadline.Text = Headline
        If rblMimeType.SelectedValue = "HTML" Or rblMimeType.SelectedValue = "BOTH" Then
            Dim htmSlot As CKEditor = e.Item.FindControl("htmSlot")
            htmSlot.Value = Slot
        Else
            Dim txtSlot As System.Web.UI.WebControls.TextBox = CType(e.Item.FindControl("txtSlot"), System.Web.UI.WebControls.TextBox)
            txtSlot.Text = Slot
        End If
    End Sub

    Protected Sub btnPreview_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        divLayout.Visible = False
        divPreview.Visible = True

        ltlName.Text = txtName.Text

        Dim Counter As Integer = 0
        Dim Slots As New GenericSerializableCollection(Of MailingMessageSlot)
        For Each row As RepeaterItem In rptSlots.Items
            Dim txtHeadline As System.Web.UI.WebControls.TextBox = CType(row.FindControl("txtHeadline"), System.Web.UI.WebControls.TextBox)
            Dim Text As String = String.Empty

            Counter += 1
            If rblMimeType.SelectedValue = "HTML" Or rblMimeType.SelectedValue = "BOTH" Then
                Dim htmSlot As CKEditor = row.FindControl("htmSlot")
                Text = htmSlot.Value
                Text = Replace(Text, System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/UserFiles/", "/UserFiles/")
                Text = Replace(Text, "/UserFiles/", System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/UserFiles/")
            Else
                Dim txtSlot As System.Web.UI.WebControls.TextBox = CType(row.FindControl("txtSlot"), System.Web.UI.WebControls.TextBox)
                Text = txtHeadline.Text
            End If
            Dim dbSlot As New MailingMessageSlot
            dbSlot.Headline = txtHeadline.Text
            dbSlot.MessageId = MessageId
            dbSlot.Slot = Text
            dbSlot.SortOrder = Counter
            Slots.Add(dbSlot)
        Next
        PreviewCtrl.Slots = Slots
        PreviewCtrl.TargetType = TargetType
        PreviewCtrl.TemplateId = TemplateId
        PreviewCtrl.MessageId = MessageId
        PreviewCtrl.MimeType = rblMimeType.SelectedValue
        PreviewCtrl.NewsletterDate = dtNewsletterDate.Text
        PreviewCtrl.SavedText = SavedText
    End Sub

    Protected Sub lnkBack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkBack.Click
        divLayout.Visible = True
        divPreview.Visible = False
    End Sub

    Private Sub SaveText(ByVal value As String)
        SavedText = value
    End Sub
End Class