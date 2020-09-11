Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

	Protected FaqId As Integer
	Protected dbFaq As FaqRow

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("FAQ")

        FaqId = Convert.ToInt32(Request("FaqId"))
		If FaqId <> 0 Then
			dbFaq = FaqRow.GetRow(DB, FaqId)
		Else
			dbFaq = New FaqRow(DB)
		End If

		up1.Visible = Not dbFaq.Email = Nothing

		If Not IsPostBack Then
			LoadFromDB()
		End If
    End Sub

    Private Sub LoadFromDB()
        drpFaqCategoryId.Datasource = FaqCategoryRow.GetAllFaqCategorys(DB)
        drpFaqCategoryId.DataValueField = "FaqCategoryId"
        drpFaqCategoryId.DataTextField = "CategoryName"
        drpFaqCategoryId.Databind()
        drpFaqCategoryId.Items.Insert(0, New ListItem("", ""))

        If FaqId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

		txtSubject.Text = SysParam.GetValue(DB, "FaqReplySubject")
		txtFromName.Text = SysParam.GetValue(DB, "FaqReplyFromName")
		txtFromEmail.Text = SysParam.GetValue(DB, "FaqReplyFromEmail")

		btnSend.Visible = False
		btnCancelReply.Visible = False
		pnlReply.Visible = False

		txtQuestion.Text = dbFaq.Question
        txtAnswer.Value = dbFaq.Answer
        drpFaqCategoryId.SelectedValue = dbFaq.FaqCategoryId
        chkIsActive.Checked = dbFaq.IsActive

		BindReplies()
	End Sub

	Private Sub BindReplies()
		gvReply.DataSource = FaqReplyRow.GetFaqReplies(DB, dbFaq.FaqId)
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

		Dim dbReply As New FaqReplyRow(DB)
		dbReply.FaqId = dbFaq.FaqId
		dbReply.Subject = txtSubject.Text
		dbReply.FullName = txtFromName.Text
		dbReply.Email = txtFromEmail.Text
		dbReply.Message = txtMessage.Text
		dbReply.AdminId = LoggedInAdminId
		dbReply.Insert()

		Core.SendSimpleMail(dbReply.Email, dbReply.FullName, dbFaq.Email, dbFaq.Email, dbReply.Subject, dbReply.Message)

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

			dbFaq.Question = txtQuestion.Text
			dbFaq.Answer = txtAnswer.Value
			dbFaq.FaqCategoryId = drpFaqCategoryId.SelectedValue
			dbFaq.IsActive = chkIsActive.Checked

			If FaqId <> 0 Then
				dbFaq.Update()
			Else
				FaqId = dbFaq.Insert
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
        Response.Redirect("delete.aspx?FaqId=" & FaqId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
