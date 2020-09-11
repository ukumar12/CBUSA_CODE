Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class delete
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BROADCAST")

        Dim ListId As Integer = Convert.ToInt32(Request("ListId"))
        Try
            DB.BeginTransaction()
            MailingListRow.RemoveRow(DB, ListId)
            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
