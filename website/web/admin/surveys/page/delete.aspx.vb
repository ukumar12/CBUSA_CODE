Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class admin_Survey_Page_Delete
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("SURVEY")

        Dim PageId As Integer = Convert.ToInt32(Request("PageId"))
        Dim SurveyId As Integer = Convert.ToInt32(Request("SurveyId"))
        Try
            DB.BeginTransaction()
            SurveyPageRow.RemoveRow(DB, PageId)
            DB.CommitTransaction()

            Response.Redirect("default.aspx?SurveyId=" & SurveyID & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
