Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class move
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("PROJECT_STATUS")

        Dim ProjectStatusID As Integer = Convert.ToInt32(Request("ProjectStatusID"))
        Dim Action as String = Request("ACTION")
        Try
            DB.BeginTransaction()
            If Core.ChangeSortOrder(DB, "ProjectStatusID", "ProjectStatus", "SortOrder", "", ProjectStatusID, Action) Then 
            	DB.CommitTransaction() 
            Else 
            	DB.RollbackTransaction()
			End if
            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
