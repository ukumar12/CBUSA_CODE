Imports Components
Imports Utility
Imports System.Data
Imports DataLayer
Imports System.Configuration.ConfigurationManager

Partial Class MailingSummary
    Inherits BaseControl

    Dim HTMLTotal As Integer
    Dim TextTotal As Integer
    Dim HTMLCopied As Boolean
    Dim TextCopied As Boolean
    Protected dbMailingMessage As MailingMessageRow

    Private ReadOnly Property BasePage() As BasePage
        Get
            Return CType(Page, BasePage)
        End Get
    End Property

    Public Property MessageId() As Integer
        Get
            Return ViewState("MessageId")
        End Get
        Set(ByVal value As Integer)
            ViewState("MessageId") = value
        End Set
    End Property

    Public Property Mode() As String
        Get
            If ViewState("Mode") Is Nothing Then ViewState("Mode") = "Edit"
            Return ViewState("Mode")
        End Get
        Set(ByVal value As String)
            ViewState("Mode") = value
        End Set
    End Property

    Protected Property IsHTML() As Boolean
        Get
            If ViewState("IsHTML") Is Nothing Then ViewState("IsHTML") = False
            Return ViewState("IsHTML")
        End Get
        Set(ByVal value As Boolean)
            ViewState("IsHTML") = value
        End Set
    End Property

    Protected Property IsText() As Boolean
        Get
            If ViewState("IsText") Is Nothing Then ViewState("IsText") = False
            Return ViewState("IsText")
        End Get
        Set(ByVal value As Boolean)
            ViewState("IsText") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadFromDb()
        End If
    End Sub

    Public Sub LoadFromDb()
        Dim SQL As String
        Dim tmpTotal As Integer

        dbMailingMessage = MailingMessageRow.GetRow(DB, MessageId)

        ltlName.Text = dbMailingMessage.Name
        Select Case dbMailingMessage.MimeType
            Case "BOTH"
                ltlFormat.Text = "HTML and Text"
                IsHTML = True
                IsText = True
            Case "HTML"
                ltlFormat.Text = "HTML Only"
                IsHTML = True
            Case "TEXT"
                ltlFormat.Text = "Plain Text Only"
                IsText = True
        End Select
        ltlFromEmail.Text = dbMailingMessage.FromEmail
        ltlFromName.Text = dbMailingMessage.FromName
        ltlReplyTo.Text = dbMailingMessage.ReplyEmail
        ltlSubject.Text = dbMailingMessage.Subject
        Select Case dbMailingMessage.TargetType
            Case "DYNAMIC"
                ltlTargetType.Text = "Uploaded E-mail List"
            Case "MEMBER"
                ltlTargetType.Text = "Subscribers"
        End Select

        Dim dbMailingGroup As MailingGroupRow = MailingGroupRow.GetRow(DB, dbMailingMessage.GroupId)
        Dim col As New NameValueCollection
        col("Lists") = dbMailingGroup.GetSelectedMailingLists()
        col("StartDate") = IIf(dbMailingGroup.StartDate = Nothing, "", dbMailingGroup.StartDate)
        col("EndDate") = IIf(dbMailingGroup.EndDate = Nothing, "", dbMailingGroup.EndDate)

        ltlLists.Text = dbMailingGroup.GetSelectedMailingListNames()
        If Not dbMailingGroup.StartDate = Nothing And Not dbMailingGroup.EndDate = Nothing Then
            ltlSubscriptionDate.Text = "Between " & dbMailingGroup.StartDate & " and " & dbMailingGroup.EndDate
        ElseIf Not dbMailingGroup.StartDate = Nothing Or Not dbMailingGroup.EndDate = Nothing Then
            If Not dbMailingGroup.StartDate = Nothing Then
                ltlSubscriptionDate.Text = "After " & dbMailingGroup.StartDate
            Else
                ltlSubscriptionDate.Text = "Before " & dbMailingGroup.EndDate
            End If
        Else
            ltlSubscriptionDate.Text = "N/A"
        End If

        Dim dbTemplate As MailingTemplateRow = MailingTemplateRow.GetRow(DB, dbMailingMessage.TemplateId)
        img.Src = "/assets/broadcast/templates/" & dbTemplate.ImageName

        If Mode = "Edit" Then
            ltlHTMLRecipientsEdit.Text = DB.ExecuteScalar(MailingHelper.GetHTMLQueryCount(DB, col, "BOTH"))
            ltlTextRecipientsEdit.Text = DB.ExecuteScalar(MailingHelper.GetTextQueryCount(DB, col, "BOTH"))
        Else
            ltlHTMLRecipientsEdit.Text = DB.ExecuteScalar("select count(*) from MailingRecipient where Messageid = " & MessageId & " and MimeType = 'HTML'")
            ltlTextRecipientsEdit.Text = DB.ExecuteScalar("select count(*) from MailingRecipient where Messageid = " & MessageId & " and MimeType = 'TEXT'")
        End If
        ltlHTMLRecipientsView.Text = ltlHTMLRecipientsEdit.Text
        ltlTextRecipientsView.Text = ltlTextRecipientsEdit.Text

        dlSlots.DataSource = MailingMessageSlotRow.GetSlotsByMessage(DB, dbMailingMessage.TemplateId, dbMailingMessage.MessageId)
        dlSlots.DataBind()

        If Mode = "View" Then

            'Html GridView
            If IsHTML Then
                HTMLTotal = DB.ExecuteScalar("select count(*) from (" & dbMailingMessage.HTMLLyrisQuery & ") as tmp")

                ltlHTMLOpen.Text = DB.ExecuteScalar("select count(*) from MailingMessageOpen where ListId = " & dbMailingMessage.ListHTMLId)

                SQL = "SELECT count(*) as HOWMANY FROM MAILING_LYRIS_OUTMAIL WHERE LISTID_ = " & dbMailingMessage.ListHTMLId
                tmpTotal = DB.ExecuteScalar(SQL)
                If tmpTotal = 0 Then HTMLCopied = False Else HTMLCopied = True

                If Not dbMailingMessage.ListHTMLId = Nothing Then
                    ltlHTML.Text = "<a href=""log.aspx?MessageId=" & MessageId & "&ListId=" & dbMailingMessage.ListHTMLId & """>View Lyris Log</a>"
                    If HTMLCopied Then
                        ltlHTML.Text &= " | <a href=""recipients.aspx?MessageId=" & MessageId & "&ListId=" & dbMailingMessage.ListHTMLId & "&F_MimeType=HTML"">Recipients</a>"
                    End If
                End If

                If HTMLCopied Then
                    SQL = " SELECT E_SuccessCount_, E_ToSendCount_, E_TransCount_, E_UnreachCount_, Finished_, ID_SuccessCount_, ID_ToSendCount_, ID_TransCount_, ID_UnreachCount_, RetryTime_, SendDate_, SendTry_, Status_, Transact_ " _
                      & " FROM Mailing_Lyris_Outmail WHERE LISTID_ = " & dbMailingMessage.ListHTMLId

                    gvHTML.DataSource = DB.GetDataTable(SQL)
                    gvHTML.DataBind()
                Else
                    SQL = " SELECT E_SuccessCount_, E_ToSendCount_, E_TransCount_, E_UnreachCount_, Finished_, ID_SuccessCount_, ID_ToSendCount_, ID_TransCount_, ID_UnreachCount_, RetryTime_, SendDate_, SendTry_, Status_, Transact_ " _
                      & " FROM outmail_ WHERE type_ = 'list' AND list_ = " & DB.Quote(dbMailingMessage.ListPrefix & "_" & MessageId & "_HTML")

                    Dim DBLyris As New Database
                    DBLyris.Open(AppSettings("LyrisConnectionString"))
                    gvHTML.DataSource = DBLyris.GetDataTable(SQL)
                    gvHTML.DataBind()
                    DBLyris.Close()
                End If
            End If

            'Text GridView
            If IsText Then
                TextTotal = DB.ExecuteScalar("select count(*) from (" & dbMailingMessage.TextLyrisQuery & ") as tmp")

                SQL = "SELECT count(*) as HOWMANY FROM MAILING_LYRIS_OUTMAIL WHERE LISTID_ = " & dbMailingMessage.ListTextId
                tmpTotal = DB.ExecuteScalar(SQL)
                If tmpTotal = 0 Then TextCopied = False Else TextCopied = True

                If Not dbMailingMessage.ListTextId = Nothing Then
                    ltlText.Text = "<a href=""log.aspx?MessageId=" & MessageId & "&ListId=" & dbMailingMessage.ListTextId & """>View Lyris Log</a>"
                    If TextCopied Then
                        ltlText.Text &= " | <a href=""recipients.aspx?MessageId=" & MessageId & "&ListId=" & dbMailingMessage.ListTextId & "&F_MimeType=TEXT"">Recipients</a>"
                    End If
                End If

                If TextCopied Then
                    SQL = " SELECT E_SuccessCount_, E_ToSendCount_, E_TransCount_, E_UnreachCount_, Finished_, ID_SuccessCount_, ID_ToSendCount_, ID_TransCount_, ID_UnreachCount_, RetryTime_, SendDate_, SendTry_, Status_, Transact_ " _
                      & " FROM MAILING_LYRIS_OUTMAIL WHERE LISTID_ = " & dbMailingMessage.ListTextId

                    gvText.DataSource = DB.GetDataTable(SQL)
                Else
                    SQL = " SELECT E_SuccessCount_, E_ToSendCount_, E_TransCount_, E_UnreachCount_, Finished_, ID_SuccessCount_, ID_ToSendCount_, ID_TransCount_, ID_UnreachCount_, RetryTime_, SendDate_, SendTry_, Status_, Transact_ " _
                      & " FROM outmail_ WHERE type_ = 'list' AND list_ = " & DB.Quote(dbMailingMessage.ListPrefix & "_" & MessageId & "_TEXT")

                    Dim DBLyris As New Database
                    DBLyris.Open(AppSettings("LyrisConnectionString"))
                    gvText.DataSource = DBLyris.GetDataTable(SQL)
                    DBLyris.Close()
                End If
                gvText.DataBind()
            End If

            'Link tracking dataview
            SQL = " select ml.MimeType, ml.MessageId, count(*) as Clicked from MailingLink ml, MailingLinkHit mlh" _
              & " where ml.MessageId = " & MessageId _
              & " and ml.LinkId = mlh.LinkId" _
              & " and ml.MessageId = mlh.MessageId" _
              & " group by ml.MessageId, ml.MimeType"

            gvLinks.DataSource = DB.GetDataTable(SQL)
            gvLinks.DataBind()
        End If

    End Sub

    Protected Sub dlSlots_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles dlSlots.ItemDataBound
        If Not e.Item.ItemType = ListItemType.Item And Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If
        Dim divWithText As HtmlGenericControl = e.Item.FindControl("divWithText")
        Dim divNoText As HtmlGenericControl = e.Item.FindControl("divNoText")
        If IsDBNull(e.Item.DataItem("Slot")) Then
            divWithText.Visible = False
            divNoText.Visible = True
        Else
            divWithText.Visible = True
            divNoText.Visible = False
        End If
    End Sub

    Protected Sub btnModify_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModify.Click
        Response.Redirect("target.aspx?MessageId=" & MessageId & "&" & BasePage.GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnLayout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLayout.Click
        Response.Redirect("layout.aspx?MessageId=" & MessageId & "&" & BasePage.GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnPreview_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPreview.Click
        Response.Redirect("preview.aspx?MessageId=" & MessageId & "&BackUrl=" & BasePage.Request.Url.LocalPath & "&" & BasePage.GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub gvHTML_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvHTML.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim Total As Integer = e.Row.DataItem("ID_SuccessCount_") + e.Row.DataItem("ID_UnreachCount_") + e.Row.DataItem("ID_TransCount_")
            Dim ltlOKPerc As Literal = e.Row.FindControl("ltlOKPerc")
            ltlOKPerc.Text = Math.Round(e.Row.DataItem("ID_SuccessCount_") / Total * 100)

            Dim ltlSoftPerc As Literal = e.Row.FindControl("ltlSoftPerc")
            ltlSoftPerc.Text = Math.Round(e.Row.DataItem("ID_TransCount_") / Total * 100)

            Dim ltlHardPerc As Literal = e.Row.FindControl("ltlHardPerc")
            ltlHardPerc.Text = Math.Round(e.Row.DataItem("ID_UnreachCount_") / Total * 100)

            Dim ltlRecipients As Literal = e.Row.FindControl("ltlRecipients")
            If HTMLCopied Then
                ltlRecipients.Text = "<a href=""recipients.aspx?MessageId=" & MessageId & "&ListId=" & dbMailingMessage.ListHTMLId & "&F_MimeType=HTML"">" & Total & "</a>"
            Else
                ltlRecipients.Text = Total
            End If
        End If
    End Sub

    Protected Sub gvText_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvText.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim Total As Integer = e.Row.DataItem("ID_SuccessCount_") + e.Row.DataItem("ID_UnreachCount_") + e.Row.DataItem("ID_TransCount_")
            Dim ltlOKPerc As Literal = e.Row.FindControl("ltlOKPerc")
            ltlOKPerc.Text = Math.Round(e.Row.DataItem("ID_SuccessCount_") / Total * 100)

            Dim ltlSoftPerc As Literal = e.Row.FindControl("ltlSoftPerc")
            ltlSoftPerc.Text = Math.Round(e.Row.DataItem("ID_TransCount_") / Total * 100)

            Dim ltlHardPerc As Literal = e.Row.FindControl("ltlHardPerc")
            ltlHardPerc.Text = Math.Round(e.Row.DataItem("ID_UnreachCount_") / Total * 100)

            Dim ltlRecipients As Literal = e.Row.FindControl("ltlRecipients")
            If TextCopied Then
                ltlRecipients.Text = "<a href=""recipients.aspx?MessageId=" & MessageId & "&ListId=" & dbMailingMessage.ListTextId & "&F_MimeType=TEXT"">" & Total & "</a>"
            Else
                ltlRecipients.Text = Total
            End If
        End If
    End Sub
End Class
