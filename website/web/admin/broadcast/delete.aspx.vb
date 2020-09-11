Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports FredCK.FCKeditorV2
Imports System.Data.SqlClient

Partial Class Delete
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BROADCAST")

        Dim MessageId As Integer = Convert.ToInt32(Request("MessageId"))
        Try
            DB.BeginTransaction()
            MailingMessageRow.RemoveRow(DB, MessageId)
            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        End Try

    End Sub
End Class