Option Strict Off

Imports Components
Imports DataLayer

Partial Class Contact
	Inherits ModuleControl

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		If Not IsPostBack Then
			drpQuestionId.DataSource = ContactUsQuestionRow.GetAllContactUsQuestions(DB)
			drpQuestionId.DataTextField = "question"
			drpQuestionId.DataValueField = "questionid"
			drpQuestionId.DataBind()

			cblLists.DataSource = MailingListRow.GetPermanentLists(DB)
			cblLists.DataTextField = "name"
			cblLists.DataValueField = "listid"
			cblLists.DataBind()

			pnlThanks.Visible = False
		End If
	End Sub

	Private Sub lnkSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkSubmit.Click
		If Not Me.Page.IsValid Then Exit Sub

		Try
			DB.BeginTransaction()

			Dim r As New ContactUsRow(DB)
			r.FullName = txtFullName.Text
			r.OrderNumber = txtOrderNumber.Text
			r.Phone = txtPhone.Text
			r.QuestionId = drpQuestionId.SelectedValue
			r.Email = txtEmail.Text
			r.YourMessage = txtYourMessage.Text
			r.HowHeardId = drpDiscovery.SelectedID
			r.HowHeardName = IIf(drpDiscovery.SelectedValue = String.Empty, "", drpDiscovery.SelectedValue)
			r.Insert()

			Dim sBody As String = "Full Name: " & txtFullName.Text & vbCrLf & _
			"Email: " & txtEmail.Text & vbCrLf & _
			"Order #: " & txtOrderNumber.Text & vbCrLf & _
			"Phone: " & txtPhone.Text & vbCrLf & _
			"Question: " & drpQuestionId.SelectedItem.Text & vbCrLf & _
			"Message: " & vbCrLf & txtYourMessage.Text

			If drpNewsletter.SelectedValue = "Yes" Then
				Dim dbMailingMember As MailingMemberRow
				Dim MemberId As Integer = 0

				dbMailingMember = MailingMemberRow.GetRowByEmail(DB, txtEmail.Text)
				dbMailingMember.Email = r.Email
				dbMailingMember.Name = r.FullName
				dbMailingMember.MimeType = rblMimeType.SelectedValue
				dbMailingMember.Status = "ACTIVE"
				If dbMailingMember.MemberId <> 0 Then
					dbMailingMember.Update()
				Else
					MemberId = dbMailingMember.Insert
				End If
				dbMailingMember.DeleteFromAllPermanentLists()
				dbMailingMember.InsertToLists(cblLists.SelectedValues)
			End If

			DB.CommitTransaction()

			Dim Email As String = SysParam.GetValue(DB, "ContactUsEmail")
			Dim dbQuestion As ContactUsQuestionRow = ContactUsQuestionRow.GetRow(DB, drpQuestionId.SelectedValue)
			If Not dbQuestion.EmailAddress = Nothing Then Email = dbQuestion.EmailAddress

			Core.SendSimpleMail(txtEmail.Text, txtFullName.Text, Email, SysParam.GetValue(DB, "ContactUsName"), SysParam.GetValue(DB, "ContactUsEmailSubject"), sBody)
		Catch ex As Exception
			DB.RollbackTransaction()
			CType(Me.Page, BasePage).AddError(CType(Me.Page, BasePage).ErrHandler.ErrorText(ex))
			Exit Sub
		End Try

		Response.Redirect("/service/ThankYou.aspx")
	End Sub

End Class
