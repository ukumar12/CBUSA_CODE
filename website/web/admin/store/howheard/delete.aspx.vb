Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class Admin_HowHeard_Delete
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("STORE")

        Dim HowHeardId As Integer = Convert.ToInt32(Request("HowHeardId"))
        Try
            DB.BeginTransaction()
            HowHeardRow.RemoveRow(DB, HowHeardId)
            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class

