Imports Components

Namespace Survey
    Namespace Controls

        Public Class ShortResponse
            Inherits BaseSurveyControl

            Dim m_Question As DataLayer.SurveyQuestionRow = Nothing

            Public ReadOnly Property Question() As DataLayer.SurveyQuestionRow
                Get
                    If m_Question Is Nothing Then
                        m_Question = DataLayer.SurveyQuestionRow.GetRow(DB, QuestionId)
                    End If
                    Return m_Question
                End Get
            End Property

            Public Function SaveAnswers() As Boolean

                DB.BeginTransaction()
                Try
                    DataLayer.SurveyAnswerRow.RemoveQuestionAnswers(DB, QuestionId, ResponseId)

                    Dim dbSurveyAnswer As New DataLayer.SurveyAnswerRow(DB)
                    dbSurveyAnswer.CategoryId = Nothing
                    dbSurveyAnswer.ChoiceId = Nothing
                    dbSurveyAnswer.Selected = False
                    dbSurveyAnswer.QuestionId = QuestionId
                    dbSurveyAnswer.Response = Me.txtResponse.Text
                    dbSurveyAnswer.ResponseId = ResponseId
                    dbSurveyAnswer.SurveyId = SurveyId
                    dbSurveyAnswer.Insert()

                Catch ex As SqlClient.SqlException
                    DB.RollbackTransaction()
                    Return False
                End Try
                DB.CommitTransaction()
                Return True

            End Function



            Public Function Validate() As Boolean

                If Question.IsRequired Then

                    If txtResponse.Text = "" Then
                        tdRequire.Attributes.Add("class", "fieldred")
                        Return False
                    End If

                End If
                Return True
            End Function

            Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
                If Not Question.IsRequired Then tdRequire.Attributes.Remove("class")

                Me.ltlQuestionText.Text = Question.Text

                If Not IsPostBack Then
                    LoadAnswers()
                End If


            End Sub

            Private Sub LoadAnswers()
                Dim dtSurveyAnswer As DataTable = DataLayer.SurveyAnswerRow.GetAnswers(DB, QuestionId, ResponseId)
                If dtSurveyAnswer.Rows.Count > 0 Then
                    Me.txtResponse.Text = Convert.ToString(dtSurveyAnswer.Rows(0)("Response"))
                End If
            End Sub
        End Class

    End Namespace
End Namespace
