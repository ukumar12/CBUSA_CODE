Imports Components

Namespace Survey
    Namespace Controls

        Public Class SelectAllThatApply
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
                    Dim Valid As Boolean = False
                    Dim item As RepeaterItem
                    For Each item In Me.rptChoices.Items
                        Dim chk As WebControls.CheckBox = item.FindControl("chkChoice")
                        If chk.Checked Then
                            Valid = True

                            'Verify the response text has a response if checked and if the response text field is displayed.
                            If item.FindControl("trChoiceResponse").Visible = True Then
                                If CType(item.FindControl("txtChoiceResponse"), TextBox).Text = "" Then
                                    Valid = False
                                    tdRequire.Attributes.Add("class", "fieldred")
                                    Return False
                                End If
                            End If
                        End If
                    Next
                    If Not Valid Then
                        tdRequire.Attributes.Add("class", "fieldred")
                        Return False
                    End If
                End If

                Return True
            End Function




            Public Function SaveAnswers() As Boolean
                Dim item As RepeaterItem, chk As CheckBox, txt As TextBox

                DB.BeginTransaction()
                Try

                    DataLayer.SurveyAnswerRow.RemoveQuestionAnswers(DB, QuestionId, ResponseId)

                    For Each item In Me.rptChoices.Items
                        chk = item.FindControl("chkChoice")

                        If chk.Checked Then
                            Dim dbSurveyAnswer As New DataLayer.SurveyAnswerRow(DB)
                            dbSurveyAnswer.CategoryId = Nothing
                            dbSurveyAnswer.ChoiceId = chk.Attributes("ChoiceId")
                            dbSurveyAnswer.QuestionId = QuestionId
                            dbSurveyAnswer.ResponseId = ResponseId
                            dbSurveyAnswer.Selected = True
                            dbSurveyAnswer.SurveyId = SurveyId
                            If LCase(chk.Attributes("ShowResponse")) = "true" Then
                                txt = item.FindControl("txtChoiceResponse")
                                dbSurveyAnswer.Response = txt.Text
                            Else
                                dbSurveyAnswer.Response = Nothing
                            End If
                            dbSurveyAnswer.Insert()
                        End If
                        
                    Next
                Catch ex As Exception
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

                    Dim chk As CheckBox = e.Item.FindControl("chkChoice")
                    chk.Attributes.Add("ChoiceId", e.Item.DataItem("ChoiceId"))
                    chk.Attributes.Add("QuestionId", e.Item.DataItem("QuestionId"))
                    chk.Attributes.Add("ShowResponse", e.Item.DataItem("ShowResponseField"))
                    chk.Attributes.Add("onclick", "document.getElementById('" & e.Item.FindControl("txtChoiceResponse").ClientID & "').focus();")

                End If
            End Sub

            Private Sub LoadAnswers()
                Dim dvSurveyAnswer As DataView = DataLayer.SurveyAnswerRow.GetAnswers(DB, QuestionId, ResponseId).DefaultView
                Dim item As RepeaterItem, chk As CheckBox, txt As TextBox
                If dvSurveyAnswer.Count > 0 Then
                    For Each item In Me.rptChoices.Items
                        chk = item.FindControl("chkChoice")
                        txt = item.FindControl("txtChoiceResponse")
                        dvSurveyAnswer.RowFilter = "ChoiceId = " & chk.Attributes("ChoiceId")
                        If dvSurveyAnswer.Count > 0 Then
                            chk.Checked = Convert.ToBoolean(dvSurveyAnswer(0)("Selected"))
                            If Not dvSurveyAnswer(0)("Response") Is DBNull.Value Then
                                txt.Text = dvSurveyAnswer(0)("Response")
                            End If
                        End If
                    Next
                End If
            End Sub

        End Class
    End Namespace
End Namespace
