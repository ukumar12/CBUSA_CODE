Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class delete
    Inherits AdminPage

    Public PIQID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("PIQ")

        PIQID = Convert.ToInt32(Request("PIQID"))

        If IsDBNull(PIQID) OrElse PIQID = 0 Then Response.Redirect("/admin/piq/default.aspx?" & GetPageParams(FilterFieldType.All))

        Dim PIQAdID As Integer = Convert.ToInt32(Request("PIQAdID"))
        Try
            DB.BeginTransaction()
            PIQAdRow.RemoveRow(DB, PIQAdID)
            DB.CommitTransaction()

            Response.Redirect("default.aspx?PIQID=" & PIQID & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class

