Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected BuilderRegistrationReferenceID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BUILDER_REGISTRATIONS")

		BuilderRegistrationReferenceID = Convert.ToInt32(Request("BuilderRegistrationReferenceID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If BuilderRegistrationReferenceID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbBuilderRegistrationReference As BuilderRegistrationReferenceRow = BuilderRegistrationReferenceRow.GetRow(DB, BuilderRegistrationReferenceID)
		txtBuilderRegistrationID.Text = dbBuilderRegistrationReference.BuilderRegistrationID
		txtContactFirstName.Text = dbBuilderRegistrationReference.ContactFirstName
		txtContactLastName.Text = dbBuilderRegistrationReference.ContactLastName
		txtCompany.Text = dbBuilderRegistrationReference.Company
		txtPhone.Text = dbBuilderRegistrationReference.Phone
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbBuilderRegistrationReference As BuilderRegistrationReferenceRow

			If BuilderRegistrationReferenceID <> 0 Then
				dbBuilderRegistrationReference = BuilderRegistrationReferenceRow.GetRow(DB, BuilderRegistrationReferenceID)
			Else
				dbBuilderRegistrationReference = New BuilderRegistrationReferenceRow(DB)
			End If
			dbBuilderRegistrationReference.BuilderRegistrationID = txtBuilderRegistrationID.Text
			dbBuilderRegistrationReference.ContactFirstName = txtContactFirstName.Text
			dbBuilderRegistrationReference.ContactLastName = txtContactLastName.Text
			dbBuilderRegistrationReference.Company = txtCompany.Text
			dbBuilderRegistrationReference.Phone = txtPhone.Text
	
			If BuilderRegistrationReferenceID <> 0 Then
				dbBuilderRegistrationReference.Update()
			Else
				BuilderRegistrationReferenceID = dbBuilderRegistrationReference.Insert
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
		Response.Redirect("delete.aspx?BuilderRegistrationReferenceID=" & BuilderRegistrationReferenceID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
