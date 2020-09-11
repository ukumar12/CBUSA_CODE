Imports Components
Imports System.Data
Imports System.Data.SqlClient

Partial Class admin_ajax_surveys
	Inherits AdminPage

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Response.Cache.SetCacheability(HttpCacheability.NoCache)

		CheckAccess("SURVEY")

		Select Case Request("F")
			Case "QuestionTypeProperties"
				QuestionTypeProperties()
		End Select
	End Sub

	Private Sub QuestionTypeProperties()
		Dim QuestionTypeId, SQL
		Dim sText = ""

		If Request("QuestionTypeId") = "" Then
			Response.Write("0|0")
			Exit Sub
		End If
		QuestionTypeId = Request("QuestionTypeId")

		SQL = " SELECT CanHaveChoices, CanHaveCategories, IsDemographic FROM SurveyQuestionType WHERE QuestionTypeId = " & QuestionTypeId
		Dim dr As SqlDataReader = DB.GetReader(SQL)
		While dr.Read
			If dr("CanHaveChoices") Then
				sText = "1|"
			Else
				sText = "0|"
			End If
			If dr("CanHaveCategories") Then
				sText &= "1|"
			Else
				sText &= "0|"
			End If
			If dr("IsDemographic") Then
				sText &= "1"
			Else
				sText &= "0"
			End If

		End While
		dr.Close()

		Response.Write(sText)
	End Sub

End Class
