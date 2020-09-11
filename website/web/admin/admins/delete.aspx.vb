Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class delete
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("USERS")

        Dim AdminId As Integer = Convert.ToInt32(Request("AdminId"))
        Dim dbAdmin As AdminRow = AdminRow.GetRow(DB, AdminId)
        Try
            DB.BeginTransaction()
            AdminRow.RemoveRow(DB, AdminId)
            DB.CommitTransaction()
            Core.LogEvent("Admin with username """ & dbAdmin.Username & """ was deleted by """ & Session("Username") & """", Diagnostics.EventLogEntryType.Information)
            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
