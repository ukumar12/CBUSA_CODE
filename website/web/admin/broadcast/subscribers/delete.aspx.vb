Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class delete
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BROADCAST")

        Dim MemberId As Integer = Convert.ToInt32(Request("MemberId"))
        Try
            DB.BeginTransaction()
            Dim dbMember As MailingMemberRow = MailingMemberRow.GetRow(DB, MemberId)
            dbMember.Status = "Deleted"
            dbMember.Update()
            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class