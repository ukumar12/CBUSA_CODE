Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class delete
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("USERS")

        Dim GroupId As Integer = Convert.ToInt32(Request("GroupId"))
        Dim dbGroup As AdminGroupRow = AdminGroupRow.GetRow(DB, GroupId)
        Try
            DB.BeginTransaction()
            AdminAccessRow.RemoveByGroup(DB, GroupId)
            AdminGroupRow.RemoveRow(DB, GroupId)
            DB.CommitTransaction()
            Core.LogEvent("Administrator Group """ & dbGroup.Description & """ was deleted by user """ & Session("Username") & """", Diagnostics.EventLogEntryType.Information)
            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
