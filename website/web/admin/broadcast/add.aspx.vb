Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class add
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BROADCAST")

        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Protected Sub btnContinue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnContinue.Click
        If Not IsValid Then Exit Sub

        'If the user copied the message from a previous one
        If Not drpPastNewsletters.SelectedValue = String.Empty Then
            Dim MessageId As Integer = DuplicateMailingMessage()
            If Not MessageId = 0 Then Response.Redirect("layout.aspx?MessageId=" & MessageId)
        Else
            Dim TargetType As String = String.Empty
            If rbTargetTypeDynamic.Checked Then TargetType = "DYNAMIC" Else TargetType = "MEMBER"
            Response.Redirect("layout.aspx?TemplateId=" & TemplateButtonValue & "&TargetType=" & TargetType)
        End If

    End Sub

    Private Function DuplicateMailingMessage() As Integer
        Try
            DB.BeginTransaction()
            Dim MessageId As Integer = MailingMessageRow.DuplicateMessage(DB, drpPastNewsletters.SelectedValue, LoggedInAdminId)
            DB.CommitTransaction()

            Return MessageId

        Catch ex As Exception
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Function

    Private Sub LoadFromDB()
        dlTemplates.DataSource = MailingTemplateRow.GetAllTemplates(DB)
        dlTemplates.DataBind()

        drpPastNewsletters.DataSource = MailingMessageRow.GetPastNewsletters(DB)
        drpPastNewsletters.DataValueField = "MessageId"
        drpPastNewsletters.DataTextField = "Name"
        drpPastNewsletters.DataBind()
        drpPastNewsletters.Items.Insert(0, New ListItem("-- please select -- ", ""))
    End Sub

    Private ReadOnly Property TemplateButtonValue() As Integer
        Get
            Dim TemplateId As Integer
            For Each item As DataListItem In dlTemplates.Items
                Dim rbTemplate As RadioButtonEx = CType(item.FindControl("rbTemplate"), RadioButtonEx)
                If rbTemplate.Checked Then
                    TemplateId = rbTemplate.Value
                    Exit For
                End If
            Next
            Return TemplateId
        End Get
    End Property

    Private Function IsTemplateButtonChecked() As Boolean
        For Each item As DataListItem In dlTemplates.Items
            Dim rbTemplate As RadioButtonEx = CType(item.FindControl("rbTemplate"), RadioButtonEx)
            If rbTemplate.Checked Then
                Return True
            End If
        Next
        Return False
    End Function

    Protected Sub valCustom_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles valCustom.ServerValidate
        args.IsValid = True
        If drpPastNewsletters.SelectedValue = String.Empty And Not IsTemplateButtonChecked() Then
            args.IsValid = False
        End If
    End Sub

    Protected Sub valTarget_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles valTarget.ServerValidate
        args.IsValid = False
        If Not drpPastNewsletters.SelectedValue = String.Empty Then
            args.IsValid = True
        End If
        If Not IsTemplateButtonChecked() Then Exit Sub
        If Not rbTargetTypeMemeber.Checked And Not rbTargetTypeDynamic.Checked Then Exit Sub
        args.IsValid = True
    End Sub
End Class