Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected AutomaticMessageID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("AUTOMATIC_MESSAGES")

		AutomaticMessageID = Convert.ToInt32(Request("AutomaticMessageID"))
		If Not IsPostBack Then

            cblVendorRoles.DataSource = VendorRoleRow.GetList(DB)
            cblVendorRoles.DataTextField = "VendorRole"
            cblVendorRoles.DataValueField = "VendorRoleId"
            cblVendorRoles.DataBind()

            LoadFromDB()
            'If Not LoggedInIsInternal Then
            '    HideFields()
            'End If
		End If
	End Sub

	Private Sub LoadFromDB()
		If AutomaticMessageID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbAutomaticMessages As AutomaticMessagesRow = AutomaticMessagesRow.GetRow(DB, AutomaticMessageID)
		txtSubject.Text = dbAutomaticMessages.Subject
		txtTitle.Text = dbAutomaticMessages.Title
		txtCondition.Text = dbAutomaticMessages.Condition
        txtMessage.Value = dbAutomaticMessages.Message
		txtCCList.Text = dbAutomaticMessages.CCList
		rblIsEmail.SelectedValue = dbAutomaticMessages.IsEmail
        rblIsMessage.SelectedValue = dbAutomaticMessages.IsMessage

        cblVendorRoles.SelectedValues = dbAutomaticMessages.GetSelectedVendorRoles
    End Sub

    Private Sub HideFields()
        trTitle.Visible = False
        trCondition.Visible = False
        trCCList.Visible = False
        trIsEmail.Visible = False
        trIsMessage.Visible = False
        btnDelete.Visible = False
    End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbAutomaticMessages As AutomaticMessagesRow

			If AutomaticMessageID <> 0 Then
				dbAutomaticMessages = AutomaticMessagesRow.GetRow(DB, AutomaticMessageID)
			Else
				dbAutomaticMessages = New AutomaticMessagesRow(DB)
			End If
			dbAutomaticMessages.Subject = txtSubject.Text
			dbAutomaticMessages.Title = txtTitle.Text
			dbAutomaticMessages.Condition = txtCondition.Text
            dbAutomaticMessages.Message = txtMessage.Value
			dbAutomaticMessages.CCList = txtCCList.Text
			dbAutomaticMessages.IsEmail = rblIsEmail.SelectedValue
			dbAutomaticMessages.IsMessage = rblIsMessage.SelectedValue
	
			If AutomaticMessageID <> 0 Then
				dbAutomaticMessages.Update()
            Else
                dbAutomaticMessages.StartDate = Now()
                AutomaticMessageID = dbAutomaticMessages.Insert
			End If

            dbAutomaticMessages.DeleteFromAllVendorRoles()
            dbAutomaticMessages.InsertToVendorRoles(cblVendorRoles.SelectedValues)

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
		Response.Redirect("delete.aspx?AutomaticMessageID=" & AutomaticMessageID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
