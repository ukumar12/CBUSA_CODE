Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class delete
	Inherits AdminPage

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("CONTACT_US")

		Dim ContactUsId As Integer = Convert.ToInt32(Request("ContactUsId"))
		Try
			DB.BeginTransaction()
			ContactUsRow.RemoveRow(DB, ContactUsId)
			DB.CommitTransaction()

			Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

		Catch ex As SqlException
			If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
			AddError(ErrHandler.ErrorText(ex))
		End Try
	End Sub
End Class

