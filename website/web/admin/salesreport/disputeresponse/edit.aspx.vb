Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected DisputeResponseID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("DISPUTE_RESPONSES")

		DisputeResponseID = Convert.ToInt32(Request("DisputeResponseID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If DisputeResponseID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbDisputeResponse As DisputeResponseRow = DisputeResponseRow.GetRow(DB, DisputeResponseID)
        txtDisputeResponse.Text = dbDisputeResponse.DisputeResponse
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbDisputeResponse As DisputeResponseRow

            If DisputeResponseID <> 0 Then
                dbDisputeResponse = DisputeResponseRow.GetRow(DB, DisputeResponseID)
            Else
                dbDisputeResponse = New DisputeResponseRow(DB)
            End If
            dbDisputeResponse.DisputeResponse = txtDisputeResponse.Text

            If DisputeResponseID <> 0 Then
                dbDisputeResponse.Update()
            Else
                DisputeResponseID = dbDisputeResponse.Insert
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
		Response.Redirect("delete.aspx?DisputeResponseID=" & DisputeResponseID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
