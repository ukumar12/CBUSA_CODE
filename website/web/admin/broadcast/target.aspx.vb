Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports FredCK.FCKeditorV2

Partial Class Target
    Inherits AdminPage

    Protected MessageId As Integer

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BROADCAST")

        MessageId = Request("MessageId")
        If Not IsPostBack Then
            LoadFromDb()
            RefreshControls()
        End If
    End Sub

    Public Sub LoadFromDb()
        Dim dbMailingMessage As MailingMessageRow = MailingMessageRow.GetRow(DB, MessageId)
        Steps.Step1 = dbMailingMessage.Step1
        Steps.Step2 = dbMailingMessage.Step2
        Steps.Step3 = dbMailingMessage.Step3
        Steps.MessageId = dbMailingMessage.MessageId

        If dbMailingMessage.TargetType = "MEMBER" Then
            cblLists.DataSource = MailingListRow.GetPermanentLists(DB)
        Else
            cblLists.DataSource = MailingListRow.GetDynamicLists(DB)
        End If
        cblLists.DataTextField = "Name"
        cblLists.DataValueField = "ListId"
        cblLists.DataBind()

        F_GroupId.DataSource = MailingGroupRow.GetPermanentGroupList(DB)
        F_GroupId.DataTextField = "Name"
        F_GroupId.DataValueField = "GroupId"
        F_GroupId.DataBind()
        F_GroupId.Items.Insert(0, New ListItem("-- select --", "0"))

        LoadGroupFilter(dbMailingMessage.GroupId)
    End Sub

    Protected Sub btnReload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReload.Click
        If Not IsValid Then Exit Sub
        RefreshControls()
    End Sub

    Private Sub LoadGroupFilter(ByVal GroupId As Integer)
        Dim dbMailingGroup As MailingGroupRow = MailingGroupRow.GetRow(DB, GroupId)

        dtStartDate.Text = dbMailingGroup.StartDate
        dtEndDate.Text = dbMailingGroup.EndDate
        cblLists.SelectedValues = MailingGroupRow.GetSelectedMailingLists(DB, GroupId)
    End Sub

    Private Sub RefreshControls()
        Dim col As New NameValueCollection

        col("Lists") = cblLists.SelectedValues
        col("StartDate") = dtStartDate.Text
        col("EndDate") = dtEndDate.Text

        ltlHTMLRecipients.Text = DB.ExecuteScalar(MailingHelper.GetHTMLQueryCount(DB, col, "BOTH"))
        ltlTextRecipients.Text = DB.ExecuteScalar(MailingHelper.GetTextQueryCount(DB, col, "BOTH"))
    End Sub

    Private Function Save() As Boolean
        Try
            DB.BeginTransaction()

            Dim dbMailingMessage As MailingMessageRow = MailingMessageRow.GetRow(DB, MessageId)
            Dim GroupId As Integer = dbMailingMessage.GroupId

            Dim dbMailingGroup As MailingGroupRow
            If GroupId <> 0 Then
                dbMailingGroup = MailingGroupRow.GetRow(DB, GroupId)
            Else
                dbMailingGroup = New MailingGroupRow(DB)
            End If
            dbMailingGroup.Name = dbMailingMessage.Name
            dbMailingGroup.Description = String.Empty
            dbMailingGroup.StartDate = dtStartDate.Value
            dbMailingGroup.EndDate = dtEndDate.Value
            dbMailingGroup.IsPermanent = False
            If GroupId <> 0 Then
                dbMailingGroup.ModifyAdminId = LoggedInAdminId
                dbMailingGroup.Update()
            Else
                dbMailingGroup.CreateAdminId = LoggedInAdminId
                dbMailingGroup.ModifyAdminId = LoggedInAdminId
                GroupId = dbMailingGroup.Insert
            End If
            dbMailingGroup.DeleteFromAllMailingLists()
            dbMailingGroup.InsertToMailingLists(cblLists.SelectedValues)

            dbMailingMessage.Step2 = True
            dbMailingMessage.ModifyAdminId = LoggedInAdminId
            dbMailingMessage.GroupId = GroupId
            dbMailingMessage.Status = "SAVED"
            dbMailingMessage.Update()

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
            Response.Redirect("review.aspx?MessageId=" & MessageId & "&" & GetPageParams(FilterFieldType.All))
        End If
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        If Save() Then
            Response.Redirect("default.aspx?MessageId=" & MessageId & "&" & GetPageParams(FilterFieldType.All))
        End If
    End Sub

    Protected Sub F_GroupId_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles F_GroupId.SelectedIndexChanged
        LoadGroupFilter(F_GroupId.SelectedValue)
        RefreshControls()
    End Sub
End Class