Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class delete
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckInternalAccess("CONTENT_TOOL")

        Dim ModuleId As Integer = Convert.ToInt32(Request("ModuleId"))
        Try
            DB.BeginTransaction()
            ContentToolModuleRow.RemoveRow(DB, ModuleId)
            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
