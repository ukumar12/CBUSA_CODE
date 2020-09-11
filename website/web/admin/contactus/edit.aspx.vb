Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected ContactUsId As Integer
	Protected dbContactUs As ContactUsRow

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("CONTACT_US")

		ContactUsId = Convert.ToInt32(Request("ContactUsId"))

		If ContactUsId <> 0 Then
			dbContactUs = ContactUsRow.GetRow(DB, ContactUsId)
		Else
			dbContactUs = New ContactUsRow(DB)
		End If

		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		drpQuestionId.DataSource = ContactUsQuestionRow.GetAllContactUsQuestions(DB)
		drpQuestionId.DataValueField = "QuestionId"
		drpQuestionId.DataTextField = "Question"
		drpQuestionId.DataBind()
		drpQuestionId.Items.Insert(0, New ListItem("", ""))

		If ContactUsId = 0 Then
			phReplies.visible = False
			btnDelete.Visible = False
			Exit Sub
		End If

		txtSubject.Text = SysParam.GetValue(DB, "ContactUsReplySubject")
		txtFromName.Text = SysParam.GetValue(DB, "ContactUsReplyFromName")
		txtFromEmail.Text = SysParam.GetValue(DB, "ContactUsReplyFromEmail")

		btnSend.Visible = False
		btnCancelReply.Visible = False
		pnlReply.Visible = False

		txtFullName.Text = dbContactUs.FullName
		txtEmail.Text = dbContactUs.Email
		txtOrderNumber.Text = dbContactUs.OrderNumber
		txtPhone.Text = dbContactUs.Phone
		txtYourMessage.Text = dbContactUs.YourMessage
		drpQuestionId.SelectedValue = dbContactUs.QuestionId

		BindReplies()
	End Sub

	Private Sub BindReplies()
		gvReply.DataSource = ContactUsReplyRow.GetContactUsReplies(DB, dbContactUs.ContactUsId)
		gvReply.DataBind()
	End Sub

	Protected Sub btnReply_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnReply.Click
		btnReply.Visible = False
		btnSend.Visible = True
		btnCancelReply.Visible = True
		pnlReply.Visible = True

		btnSave.Visible = False
		btnDelete.Visible = False
		btnCancel.Visible = False
	End Sub

	Protected Sub btnCancelReply_click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancelReply.Click
		HideReply()
	End Sub

	Private Sub btnSend_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSend.Click
		If Not IsValid Then Exit Sub

		Dim dbReply As New ContactUsReplyRow(DB)
		dbReply.ContactUsId = dbContactUs.ContactUsId
		dbReply.Subject = txtSubject.Text
		dbReply.FullName = txtFromName.Text
		dbReply.Email = txtFromEmail.Text
		dbReply.Message = txtMessage.Text
		dbReply.AdminId = LoggedInAdminId
		dbReply.Insert()

		Core.SendSimpleMail(dbReply.Email, dbReply.FullName, dbContactUs.Email, dbContactUs.FullName, dbReply.Subject, dbReply.Message)

		HideReply()
		BindReplies()
	End Sub

	Private Sub HideReply()
		btnReply.Visible = True
		btnSend.Visible = False
		btnCancelReply.Visible = False
		pnlReply.Visible = False

		btnSave.Visible = True
		btnDelete.Visible = True
		btnCancel.Visible = True
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			dbContactUs.FullName = txtFullName.Text
			dbContactUs.Email = txtEmail.Text
			dbContactUs.OrderNumber = txtOrderNumber.Text
			dbContactUs.Phone = txtPhone.Text
			dbContactUs.YourMessage = txtYourMessage.Text
			dbContactUs.QuestionId = drpQuestionId.SelectedValue

			If ContactUsId <> 0 Then
				dbContactUs.Update()
			Else
				ContactUsId = dbContactUs.Insert
			End If

			DB.CommitTransaction()


			Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

		Catch ex As SqlException
			If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
			AddError(ErrHandler.ErrorText(ex))
		End Try
	End Sub

	Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
		Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
	End Sub

	Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
		Response.Redirect("delete.aspx?ContactUsId=" & ContactUsId & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class

