Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected DisputeResponseReasonID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("DISPUTE_RESPONSE_REASONS")

		DisputeResponseReasonID = Convert.ToInt32(Request("DisputeResponseReasonID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If DisputeResponseReasonID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbDisputeResponseReason As DisputeResponseReasonRow = DisputeResponseReasonRow.GetRow(DB, DisputeResponseReasonID)
		txtDisputeResponseReason.Text = dbDisputeResponseReason.DisputeResponseReason
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbDisputeResponseReason As DisputeResponseReasonRow

			If DisputeResponseReasonID <> 0 Then
				dbDisputeResponseReason = DisputeResponseReasonRow.GetRow(DB, DisputeResponseReasonID)
			Else
				dbDisputeResponseReason = New DisputeResponseReasonRow(DB)
			End If
			dbDisputeResponseReason.DisputeResponseReason = txtDisputeResponseReason.Text
	
			If DisputeResponseReasonID <> 0 Then
				dbDisputeResponseReason.Update()
			Else
				DisputeResponseReasonID = dbDisputeResponseReason.Insert
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
		Response.Redirect("delete.aspx?DisputeResponseReasonID=" & DisputeResponseReasonID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
