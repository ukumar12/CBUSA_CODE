Imports Components

Namespace Survey
    Namespace Controls

        Public Class Quantity
            Inherits BaseSurveyControl

            Dim iNumberOfChoices As Integer
            Dim m_Question As DataLayer.SurveyQuestionRow = Nothing

            Public ReadOnly Property Question() As DataLayer.SurveyQuestionRow
                Get
                    If m_Question Is Nothing Then
                        m_Question = DataLayer.SurveyQuestionRow.GetRow(DB, QuestionId)
                    End If
                    Return m_Question
                End Get
            End Property


            Public Function Validate() As Boolean
                If Question.IsRequired Then
                    Dim item As RepeaterItem, iTotal As Integer = 0

                    For Each item In Me.rptChoices.Items
                        Dim txt As TextBox = item.FindControl("txtChoice")
                        Try
                            If txt.Text <> "" Then 'numeric check
                                iTotal += txt.Text
                            End If
                        Catch ex As Exception
                            tdRequire.Attributes.Add("class", "fieldred")
                            AddError("Verify the sum of all percentages is 100%")
                            Return False
                        End Try
                    Next

                    If iTotal = 0 Then
                        tdRequire.Attributes.Add("class", "fieldred")
                        Return False
                    End If

                End If
                Return True
            End Function

            Public Function SaveAnswers() As Boolean
                Dim item As RepeaterItem, txt As TextBox, iPercentage As Integer
                DB.BeginTransaction()
                Try
                    DataLayer.SurveyAnswerRow.RemoveQuestionAnswers(DB, QuestionId, ResponseId)

                    For Each item In Me.rptChoices.Items
                        txt = item.FindControl("txtChoice")
                        If IsNumeric(Trim(txt.Text)) Then
                            iPercentage = Convert.ToInt32(Trim(txt.Text))
                            If iPercentage <> 0 Then
                                Dim dbSurveyAnswer As New DataLayer.SurveyAnswerRow(DB)
                                dbSurveyAnswer.CategoryId = Nothing
                                dbSurveyAnswer.ChoiceId = txt.Attributes("ChoiceId")
                                dbSurveyAnswer.QuestionId = QuestionId
                                dbSurveyAnswer.ResponseId = ResponseId
                                dbSurveyAnswer.Selected = True
                                dbSurveyAnswer.SurveyId = SurveyId
                                dbSurveyAnswer.Value = iPercentage.ToString
                                If LCase(txt.Attributes("ShowResponse")) = "true" Then
                                    txt = item.FindControl("txtChoiceResponse")
                                    dbSurveyAnswer.Response = txt.Text
                                Else
                                    dbSurveyAnswer.Response = Nothing
                                End If
                                dbSurveyAnswer.Insert()
                            End If
                        End If
                    Next
                Catch ex As SqlClient.SqlException
                    DB.RollbackTransaction()
                    Return False
                End Try

                DB.CommitTransaction()
                Return True
                
            End Function




            Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

                If Not Question.IsRequired Then tdRequire.Attributes.Remove("class")
                If Not IsPostBack Then
                    LoadFromDB()
                    LoadAnswers()
                End If

            End Sub
            Private Sub LoadFromDB()
                Me.ltlQuestionText.Text = Question.Text

                Dim dtChoices As DataTable = DataLayer.SurveyQuestionChoiceRow.getChoicesByQuestionId(DB, QuestionId)
                iNumberOfChoices = dtChoices.Rows.Count
                Me.rptChoices.DataSource = dtChoices
                rptChoices.DataBind()
            End Sub

            Protected Sub rptChoices_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptChoices.ItemDataBound
                If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then

                    Dim tr As HtmlControls.HtmlTableRow = e.Item.FindControl("trChoiceResponse")
                    If e.Item.DataItem("ShowResponseField") Then
                        tr.Visible = True
                    Else
                        tr.Visible = False
                    End If

                    Dim ltl As Literal = e.Item.FindControl("ltlChoiceText")
                    ltl.Text = e.Item.DataItem("DisplayText")

                    Dim chk As TextBox = e.Item.FindControl("txtChoice")
                    chk.Attributes.Add("ChoiceId", e.Item.DataItem("ChoiceId"))
                    chk.Attributes.Add("QuestionId", e.Item.DataItem("QuestionId"))
                    chk.Attributes.Add("ShowResponse", e.Item.DataItem("ShowResponseField"))

                End If
            End Sub

            Private Sub LoadAnswers()
                Dim dvSurveyAnswer As DataView = DataLayer.SurveyAnswerRow.GetAnswers(DB, QuestionId, ResponseId).DefaultView
                Dim item As RepeaterItem, txt As TextBox, txtResponse As TextBox
                If dvSurveyAnswer.Count > 0 Then
                    For Each item In Me.rptChoices.Items
                        txt = item.FindControl("txtChoice")
                        txtResponse = item.FindControl("txtChoiceResponse")
                        dvSurveyAnswer.RowFilter = "ChoiceId = " & txt.Attributes("ChoiceId")
                        If dvSurveyAnswer.Count > 0 Then
                            txt.Text = dvSurveyAnswer(0)("Value").ToString
                            If Not dvSurveyAnswer(0)("Response") Is DBNull.Value Then
                                txtResponse.Text = dvSurveyAnswer(0)("Response").ToString
                            End If
                        End If
                    Next
                End If
            End Sub

        End Class
    End Namespace
End Namespace
