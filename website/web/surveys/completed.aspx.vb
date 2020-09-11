Imports Components
Imports DataLayer

Partial Class surveys_completed
    Inherits SitePage

    Protected SurveyId As Integer
    Protected ResponseId As Integer
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ResponseId = Convert.ToInt32(Utility.Crypt.DecryptTripleDES(Request.QueryString("ResponseId")))
            SurveyId = DataLayer.SurveyResponseRow.GetSurveyId(DB, ResponseId)
            If Not SurveyRow.IsLive(DB, SurveyId) Then Response.Redirect("notfound.aspx")
        Catch ex As Exception
            Response.Redirect("notfound.aspx")
        End Try
        DB.BeginTransaction()
        Try

            Dim dbSurveyResponse As DataLayer.SurveyResponseRow = DataLayer.SurveyResponseRow.GetRow(DB, ResponseId)
            dbSurveyResponse.CompleteDate = Now
            dbSurveyResponse.Status = 2
            dbSurveyResponse.Update()

        Catch ex As SqlClient.SqlException
            DB.RollbackTransaction()
        End Try
        DB.CommitTransaction()
        
    End Sub
End Class
