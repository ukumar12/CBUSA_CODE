Imports Components
Imports DataLayer
Imports System.Data
Imports System.Data.SqlClient

Partial Class substitutions_delete
	Inherits SitePage

	Protected Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load
		Dim ProductId As Integer = Convert.ToInt32(Request("ProductId"))
		Try
			DB.BeginTransaction()
			DB.ExecuteSQL("DELETE FROM BuilderProductSubstitution WHERE BuilderId = " & DB.Number(Session("BuilderId")) & " AND ProductId = " & DB.Number(ProductId))
			'ProjectRow.RemoveRow(DB, ProjectID)
			DB.CommitTransaction()

			Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

		Catch ex As Exception
			If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
			AddError(ErrHandler.ErrorText(ex))
		End Try

	End Sub
End Class
