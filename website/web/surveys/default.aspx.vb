Imports Components
Imports DataLayer
Imports Utility

Partial Class Surveys_default
    Inherits SitePage

    Protected PageId As Integer
    Protected SurveyId As Integer
    Protected ResponseId As Integer
    Private dtQuestions As DataTable


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Try
            ResponseId = Convert.ToInt32(Utility.Crypt.DecryptTripleDES(Request.QueryString("ResponseId")))
            SurveyId = DataLayer.SurveyResponseRow.GetSurveyId(DB, ResponseId)
            If Not SurveyRow.IsLive(DB, SurveyId) Then Response.Redirect("/404.aspx")
        Catch ex As Exception
            Response.Redirect("/404.aspx")
        End Try

        If Request("PageId") = "" Then
            PageId = SurveyPageRow.getFirstPageId(DB, SurveyId)
        Else
            PageId = Convert.ToInt32(Request("PageId"))
        End If

        Dim dbSurvey As SurveyRow = SurveyRow.GetRow(DB, SurveyId)
        ltlSurveyName.Text = dbSurvey.DisplayTitle
        ltlSurveyDesc.Text = dbSurvey.Description


        Dim dbSurveyPage As SurveyPageRow = SurveyPageRow.GetRow(DB, PageId)
        lblPageName.Text = dbSurveyPage.DisplayName

        If dbSurveyPage.IsFirstPage Then
            Me.lnkBtnPreviousBottom.Visible = False
            Me.lnkBtnPrevious.Visible = False
            sep.Visible = False
            sep2.Visible = False
		End If
		If PageId = SurveyPageRow.getLastPageId(DB, SurveyId) Then
            lnkBtnNext.Text = "Submit"
			lnkBtnNextBottom.Text = lnkBtnNext.Text
		End If


		dtQuestions = SurveyQuestionRow.GetSurveyPageQuestions(DB, PageId).Tables(0)
		PopagateSurveyPage()

	End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Page.ClientScript.RegisterClientScriptInclude("SurveyFunctions", "../includes/survey.js")

    End Sub


    Private Sub PopagateSurveyPage()
        Dim dr As DataRow, iCounter As Integer = 1

        For Each dr In dtQuestions.Rows

            Select Case dr("QuestionTypeId")
                Case 1
                    Dim question As Survey.Controls.ShortResponse = CType(LoadControl("/controls/survey/shortresponse.ascx"), UserControl)
                    question.ID = "Question" & dr("QuestionId").ToString
                    question.SurveyId = SurveyId
                    question.ResponseId = ResponseId
                    question.QuestionId = dr("QuestionId")
                    plcQuestions.Controls.Add(question)
                Case 2
                    Dim question As Survey.Controls.LongResponse = LoadControl("/controls/survey/longresponse.ascx")
                    question.ID = "Question" & dr("QuestionId").ToString
                    question.SurveyId = SurveyId
                    question.ResponseId = ResponseId
                    question.QuestionId = dr("QuestionId")
                    plcQuestions.Controls.Add(question)
                Case 3
                    Dim question As Survey.Controls.SelectOne = LoadControl("/controls/survey/selectone.ascx")
                    question.ID = "Question" & dr("QuestionId").ToString
                    question.SurveyId = SurveyId
                    question.ResponseId = ResponseId
                    question.QuestionId = dr("QuestionId")
                    plcQuestions.Controls.Add(question)
                Case 4
                    Dim question As Survey.Controls.SelectAllThatApply = LoadControl("/controls/survey/selectallthatapply.ascx")
                    question.ID = "Question" & dr("QuestionId").ToString
                    question.SurveyId = SurveyId
                    question.ResponseId = ResponseId
                    question.QuestionId = dr("QuestionId")
                    plcQuestions.Controls.Add(question)
                Case 5
                    Dim question As Survey.Controls.StandardRank = LoadControl("/controls/survey/standardrank.ascx")
                    question.ID = "Question" & dr("QuestionId").ToString
                    question.SurveyId = SurveyId
                    question.ResponseId = ResponseId
                    question.QuestionId = dr("QuestionId")
                    plcQuestions.Controls.Add(question)
                Case 6
                    Dim question As Survey.Controls.PercentageRank = LoadControl("/controls/survey/percentagerank.ascx")
                    question.ID = "Question" & dr("QuestionId").ToString
                    question.SurveyId = SurveyId
                    question.ResponseId = ResponseId
                    question.QuestionId = dr("QuestionId")
                    plcQuestions.Controls.Add(question)
                Case 7
                    Dim question As Survey.Controls.Date = LoadControl("/controls/survey/datecontrol.ascx")
                    question.ID = "Question" & dr("QuestionId").ToString
                    question.SurveyId = SurveyId
                    question.ResponseId = ResponseId
                    question.QuestionId = dr("QuestionId")
                    plcQuestions.Controls.Add(question)
                Case 8
                    Dim question As Survey.Controls.Quantity = LoadControl("/controls/survey/quantity.ascx")
                    question.ID = "Question" & dr("QuestionId").ToString
                    question.SurveyId = SurveyId
                    question.ResponseId = ResponseId
                    question.QuestionId = dr("QuestionId")
                    plcQuestions.Controls.Add(question)
                Case 9
                    Dim question As Survey.Controls.Rate = LoadControl("/controls/survey/rate.ascx")
                    question.ID = "Question" & dr("QuestionId").ToString
                    question.SurveyId = SurveyId
                    question.ResponseId = ResponseId
                    question.QuestionId = dr("QuestionId")
                    plcQuestions.Controls.Add(question)
                Case 10
                    Dim question As Survey.Controls.Demographic = LoadControl("/controls/survey/demographic.ascx")
                    question.ID = "Question" & dr("QuestionId").ToString
                    question.SurveyId = SurveyId
                    question.ResponseId = ResponseId
                    question.QuestionId = dr("QuestionId")
                    plcQuestions.Controls.Add(question)
            End Select


            If iCounter <> dtQuestions.Rows.Count Then
                plcQuestions.Controls.Add(New LiteralControl("<br />"))
            End If
            
            iCounter += 1
        Next


    End Sub

    Private Function ValidateQuestions() As Boolean
        Dim dr As DataRow, Valid As Boolean = True
        For Each dr In dtQuestions.Rows
            Select Case dr("QuestionTypeId")
                Case 1
                    Dim question As Survey.Controls.ShortResponse = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    If Not question.Validate() Then Valid = False
                Case 2
                    Dim question As Survey.Controls.LongResponse = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    If Not question.Validate() Then Valid = False
                Case 3
                    Dim question As Survey.Controls.SelectOne = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    If Not question.Validate() Then Valid = False
                Case 4
                    Dim question As Survey.Controls.SelectAllThatApply = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    If Not question.Validate() Then Valid = False
                Case 5
                    Dim question As Survey.Controls.StandardRank = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    If Not question.Validate() Then Valid = False
                Case 6
                    Dim question As Survey.Controls.PercentageRank = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    If Not question.Validate() Then Valid = False
                Case 7
                    Dim question As Survey.Controls.Date = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    If Not question.Validate() Then Valid = False
                Case 8
                    Dim question As Survey.Controls.Quantity = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    If Not question.Validate() Then Valid = False
                Case 9
                    Dim question As Survey.Controls.Rate = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    If Not question.Validate() Then Valid = False
                Case 10
                    Dim question As Survey.Controls.Demographic = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    If Not question.Validate() Then Valid = False
            End Select
        Next
        Return Valid
    End Function

    Private Function SaveAnswers() As Boolean
        Dim dr As DataRow, Valid As Boolean = True
        For Each dr In dtQuestions.Rows
            Select Case dr("QuestionTypeId")
                Case 1
                    Dim question As Survey.Controls.ShortResponse = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    question.SaveAnswers()
                Case 2
                    Dim question As Survey.Controls.LongResponse = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    question.SaveAnswers()
                Case 3
                    Dim question As Survey.Controls.SelectOne = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    question.SaveAnswers()
                Case 4
                    Dim question As Survey.Controls.SelectAllThatApply = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    question.SaveAnswers()
                Case 5
                    Dim question As Survey.Controls.StandardRank = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    question.SaveAnswers()
                Case 6
                    Dim question As Survey.Controls.PercentageRank = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    question.SaveAnswers()
                Case 7
                    Dim question As Survey.Controls.Date = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    question.SaveAnswers()
                Case 8
                    Dim question As Survey.Controls.Quantity = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    question.SaveAnswers()
                Case 9
                    Dim question As Survey.Controls.Rate = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    question.SaveAnswers()
                Case 10
                    Dim question As Survey.Controls.Demographic = Me.plcQuestions.FindControl("Question" & dr("QuestionId").ToString)
                    question.SaveAnswers()
            End Select
        Next
        Return Valid
    End Function

    Protected Sub lnkBtnNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkBtnNext.Click, lnkBtnNextBottom.Click
        If Not ValidateQuestions() Then
            AddError("Please answer all questions as marked.")
            SaveAnswers()
            Exit Sub
        End If
        SaveAnswers()
        PageId = DataLayer.SurveyPageRow.GetNextPage(DB, PageId, SurveyId)
        If PageId = 0 Then
			Response.Redirect("completed.aspx?SurveyId=" & SurveyId & "&ResponseId=" & Server.UrlEncode(Request("ResponseId")))
        End If
		Response.Redirect("default.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&ResponseId=" & Server.UrlEncode(Request("ResponseId")))

    End Sub

    Protected Sub lnkBtnPrevious_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkBtnPrevious.Click, lnkBtnPreviousBottom.Click
         
        SaveAnswers()
        PageId = DataLayer.SurveyPageRow.GetPreviousPage(DB, PageId, SurveyId)
        If PageId = 0 Then Exit Sub
		Response.Redirect("default.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&ResponseId=" & Server.UrlEncode(Request("ResponseId")))

    End Sub
End Class