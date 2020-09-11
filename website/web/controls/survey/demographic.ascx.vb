Imports Components
Imports Controls

Namespace Survey
    Namespace Controls

        Public Class Demographic
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

            Public Function Validate() As Boolean
                Dim item As RepeaterItem, Valid As Boolean = True
				'tdRequire.Attributes.Add("class", "fieldreq")
				For Each item In Me.rptDemographics.Items
					If item.ItemType = ListItemType.AlternatingItem Or item.ItemType = ListItemType.Item Then
						Dim lbl As Label = item.FindControl("lblIsRequired")
                        If lbl.Text = "True" AndAlso Not ValidateDemographics(item) Then
                            Valid = False
                        End If
					End If
				Next
                If Not Valid Then
                    Return False
                End If
                Return True
            End Function

            Public Function SaveAnswers() As Boolean

                DB.BeginTransaction()

                Try
                    DataLayer.SurveyResponseDemographicRow.DeleteDemographicFromResponseId(DB, ResponseId)

                    Dim item As RepeaterItem, iTypeId As Integer
                    Dim dbSurveyResponseDemographic As DataLayer.SurveyResponseDemographicRow

                    For Each item In Me.rptDemographics.Items
                        iTypeId = CType(item.FindControl("TypeId"), HtmlInputText).Value

                        dbSurveyResponseDemographic = New DataLayer.SurveyResponseDemographicRow(DB)
                        dbSurveyResponseDemographic.ResponseId = ResponseId

                        Select Case iTypeId
                            Case 1, 2, 7, 8, 10, 11
                                Dim txt As TextBox = item.FindControl("Demographic")
                                dbSurveyResponseDemographic.DemographicId = txt.Attributes("DemographicId")
                                dbSurveyResponseDemographic.SurveyQuestionDemographicId = txt.Attributes("SurveyQuestionDemographicId")
                                If txt.Text <> "" Then
                                    dbSurveyResponseDemographic.Value = txt.Text
                                    dbSurveyResponseDemographic.Insert()
                                End If

                            Case 5, 6, 9
                                Dim drp As DropDownList = item.FindControl("Demographic")
                                dbSurveyResponseDemographic.DemographicId = drp.Attributes("DemographicId")
                                dbSurveyResponseDemographic.SurveyQuestionDemographicId = drp.Attributes("SurveyQuestionDemographicId")
                                If drp.SelectedValue <> "" Then
                                    dbSurveyResponseDemographic.Value = drp.SelectedValue
                                    dbSurveyResponseDemographic.Insert()
                                End If
                            Case 3
                                Dim dp As DatePicker = item.FindControl("Demographic")
                                dbSurveyResponseDemographic.DemographicId = dp.Attributes("DemographicId")
                                dbSurveyResponseDemographic.SurveyQuestionDemographicId = dp.Attributes("SurveyQuestionDemographicId")
                                If dp.Text <> "" Then
                                    dbSurveyResponseDemographic.Value = dp.Value
                                    dbSurveyResponseDemographic.Insert()
                                End If

                            Case 4
                                Dim phone As Phone = item.FindControl("Demographic")
                                dbSurveyResponseDemographic.DemographicId = phone.Attributes("DemographicId")
                                dbSurveyResponseDemographic.SurveyQuestionDemographicId = phone.Attributes("SurveyQuestionDemographicId")
                                If phone.Value <> "" Then
                                    dbSurveyResponseDemographic.Value = phone.Value
                                    dbSurveyResponseDemographic.Insert()
                                End If
                        End Select

                    Next

                Catch ex As SqlClient.SqlException
                    DB.RollbackTransaction()
                    Return True
                End Try

                DB.CommitTransaction()
                Return True

            End Function

            Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

				If Not Question.IsRequired Then tdRequire.Attributes.Remove("class")

                LoadFromDB()

                If Not IsPostBack Then
                    LoadAnswers()
                End If
            End Sub

            Private Sub LoadAnswers()
                Dim item As RepeaterItem, iTypeId As Integer
                Dim dtDemographics As DataView = DataLayer.SurveyResponseDemographicRow.GetDemographicsFromResponseId(DB, ResponseId).DefaultView

                For Each item In Me.rptDemographics.Items

                    iTypeId = CType(item.FindControl("TypeId"), HtmlInputText).Value

                    Select Case iTypeId
                        Case 1, 2, 7, 8, 10, 11
                            Dim txt As TextBox = item.FindControl("Demographic")
                            dtDemographics.RowFilter = "SurveyQuestionDemographicId=" & txt.Attributes("SurveyQuestionDemographicId")
                            If dtDemographics.Count > 0 Then
                                txt.Text = dtDemographics(0)("Value")
                            End If

                        Case 5, 6, 9
                            Dim drp As DropDownList = item.FindControl("Demographic")
                            dtDemographics.RowFilter = "SurveyQuestionDemographicId=" & drp.Attributes("SurveyQuestionDemographicId")
                            If dtDemographics.Count > 0 Then
                                drp.SelectedValue = dtDemographics(0)("Value")
                            End If

                        Case 3
                            Dim dp As DatePicker = item.FindControl("Demographic")
                            dtDemographics.RowFilter = "SurveyQuestionDemographicId=" & dp.Attributes("SurveyQuestionDemographicId")
                            If dtDemographics.Count > 0 Then
                                dp.Text = dtDemographics(0)("Value")
                            End If

                        Case 4
                            Dim phone As Phone = item.FindControl("Demographic")
                            dtDemographics.RowFilter = "SurveyQuestionDemographicId=" & phone.Attributes("SurveyQuestionDemographicId")
                            If dtDemographics.Count > 0 Then
                                phone.Value = dtDemographics(0)("Value")
                            End If

                    End Select
                Next
            End Sub

            Private Sub LoadFromDB()
                Me.ltlQuestionText.Text = Question.Text



                Dim dt As DataTable = DataLayer.SurveyQuestionDemographicRow.GetQuestionDemographics(DB, QuestionId)
                Me.rptDemographics.DataSource = dt
                Me.rptDemographics.DataBind()


            End Sub

            Protected Sub rptDemographics_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptDemographics.ItemDataBound
                If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then
                    Dim td As HtmlControls.HtmlTableCell

                    td = e.Item.FindControl("tdText")
					td.InnerHtml = e.Item.DataItem("DisplayText")

					If Not e.Item.DataItem("IsRequired") Then CType(e.Item.FindControl("tdRequireDemographic"), HtmlTableCell).Attributes.Remove("class")
					Dim lbl As New Label
					lbl.Visible = False
					lbl.ID = "lblIsRequired"
					lbl.Text = e.Item.DataItem("IsRequired")
					td.Controls.Add(lbl)

                    Dim txt As New HtmlControls.HtmlInputText
                    txt.ID = "TypeId"
                    txt.Value = e.Item.DataItem("TypeId")
                    txt.Style.Add("display", "none")
                    td.Controls.Add(txt)

                    td = e.Item.FindControl("tdField")
                    AddControls(td, e)

                End If
            End Sub

            Private Sub AddControls(ByRef td As HtmlControls.HtmlTableCell, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
                Select Case e.Item.DataItem("TypeId")
                    Case 1
                        Dim txt As New TextBox
                        txt.ID = "Demographic"
                        txt.Attributes.Add("class", "ibox")
                        txt.Attributes.Add("TypeId", e.Item.DataItem("TypeId"))
                        txt.Attributes.Add("DemographicId", e.Item.DataItem("DemographicId"))
                        txt.Attributes.Add("SurveyQuestionDemographicId", e.Item.DataItem("SurveyQuestionDemographicId"))
                        txt.CssClass = "SurveyInputField"
                        txt.ValidationGroup = "Survey"
                        txt.Attributes.Add("IsRequired", e.Item.DataItem("IsRequired"))
                        td.Controls.Add(txt)
                        
                    Case 2
                        Dim txt As New TextBox
                        txt.ID = "Demographic"
                        txt.Attributes.Add("class", "ibox")
                        txt.Attributes.Add("DemographicId", e.Item.DataItem("DemographicId"))
                        txt.Attributes.Add("SurveyQuestionDemographicId", e.Item.DataItem("SurveyQuestionDemographicId"))
                        txt.Attributes.Add("TypeId", e.Item.DataItem("TypeId"))
                        txt.CssClass = "SurveyInputField"
                        txt.TextMode = TextBoxMode.MultiLine
                        txt.Attributes.Add("IsRequired", e.Item.DataItem("IsRequired"))
                        td.Controls.Add(txt)
                    Case 3
                        Dim dp As New DatePicker
                        dp.ID = "Demographic"
                        dp.Attributes.Add("class", "ibox")
                        dp.Attributes.Add("DemographicId", e.Item.DataItem("DemographicId"))
                        dp.Attributes.Add("SurveyQuestionDemographicId", e.Item.DataItem("SurveyQuestionDemographicId"))
                        dp.Attributes.Add("TypeId", e.Item.DataItem("TypeId"))
                        dp.Attributes.Add("IsRequired", e.Item.DataItem("IsRequired"))
                        td.Controls.Add(dp)
                    Case 4
                        Dim phone As New Phone
                        phone.ID = "Demographic"
                        phone.Attributes.Add("DemographicId", e.Item.DataItem("DemographicId"))
                        phone.Attributes.Add("SurveyQuestionDemographicId", e.Item.DataItem("SurveyQuestionDemographicId"))
                        phone.Attributes.Add("class", "ibox")
                        phone.Attributes.Add("TypeId", e.Item.DataItem("TypeId"))
                        phone.Attributes.Add("IsRequired", e.Item.DataItem("IsRequired"))
                        td.Controls.Add(phone)
                    Case 5
                        Dim drp As New DropDownList
						Dim dt As DataTable = DataLayer.StateRow.GetStateList(DB)
                        drp.ID = "Demographic"
                        drp.Attributes.Add("DemographicId", e.Item.DataItem("DemographicId"))
                        drp.Attributes.Add("SurveyQuestionDemographicId", e.Item.DataItem("SurveyQuestionDemographicId"))
                        drp.Attributes.Add("TypeId", e.Item.DataItem("TypeId"))
                        drp.Attributes.Add("class", "ibox")
                        drp.DataSource = dt
                        drp.DataTextField = "StateName"
                        drp.DataValueField = "StateCode"
                        drp.DataBind()
                        drp.Items.Insert(0, New ListItem("", ""))
                        drp.Attributes.Add("IsRequired", e.Item.DataItem("IsRequired"))
                        td.Controls.Add(drp)
                    Case 6
                        Dim drp As New DropDownList
						Dim dt As DataTable = DataLayer.CountryRow.GetCountryList(DB)
                        drp.ID = "Demographic"
                        drp.Attributes.Add("DemographicId", e.Item.DataItem("DemographicId"))
                        drp.Attributes.Add("SurveyQuestionDemographicId", e.Item.DataItem("SurveyQuestionDemographicId"))
                        drp.Attributes.Add("TypeId", e.Item.DataItem("TypeId"))
                        drp.DataSource = dt
                        drp.Attributes.Add("class", "ibox")
                        drp.DataTextField = "CountryName"
                        drp.DataValueField = "CountryCode"
                        drp.DataBind()
                        drp.Items.Insert(0, New ListItem("", ""))
                        drp.Attributes.Add("IsRequired", e.Item.DataItem("IsRequired"))
                        td.Controls.Add(drp)
                    Case 7
                        Dim txt As New TextBox
                        txt.ID = "Demographic"
                        txt.Attributes.Add("DemographicId", e.Item.DataItem("DemographicId"))
                        txt.Attributes.Add("SurveyQuestionDemographicId", e.Item.DataItem("SurveyQuestionDemographicId"))
                        txt.Attributes.Add("TypeId", e.Item.DataItem("TypeId"))
                        txt.Attributes.Add("IsRequired", e.Item.DataItem("IsRequired"))
                        td.Controls.Add(txt)
                    Case 8
                        Dim txt As New TextBox
                        txt.ID = "Demographic"
                        txt.Attributes.Add("DemographicId", e.Item.DataItem("DemographicId"))
                        txt.Attributes.Add("SurveyQuestionDemographicId", e.Item.DataItem("SurveyQuestionDemographicId"))
                        txt.Attributes.Add("class", "ibox")
                        txt.Attributes.Add("TypeId", e.Item.DataItem("TypeId"))
                        txt.Attributes.Add("IsRequired", e.Item.DataItem("IsRequired"))
                        td.Controls.Add(txt)
                    Case 9
                        Dim drp As New DropDownList
                        drp.Items.Add(New ListItem("", ""))
                        drp.Items.Add(New ListItem("Male", "Male"))
                        drp.Items.Add(New ListItem("Female", "Female"))
                        drp.Attributes.Add("class", "ibox")
                        drp.ID = "Demographic"
                        drp.Attributes.Add("DemographicId", e.Item.DataItem("DemographicId"))
                        drp.Attributes.Add("SurveyQuestionDemographicId", e.Item.DataItem("SurveyQuestionDemographicId"))
                        drp.Attributes.Add("TypeId", e.Item.DataItem("TypeId"))
                        drp.Attributes.Add("IsRequired", e.Item.DataItem("IsRequired"))
                        td.Controls.Add(drp)
                    Case 10
                        Dim txt As New TextBox
                        txt.ID = "Demographic"
                        txt.Attributes.Add("DemographicId", e.Item.DataItem("DemographicId"))
                        txt.Attributes.Add("class", "ibox")
                        txt.Attributes.Add("SurveyQuestionDemographicId", e.Item.DataItem("SurveyQuestionDemographicId"))
                        txt.Attributes.Add("TypeId", e.Item.DataItem("TypeId"))
                        txt.Attributes.Add("IsRequired", e.Item.DataItem("IsRequired"))
                        td.Controls.Add(txt)
                    Case 11
                        Dim txt As New TextBox
                        txt.ID = "Demographic"
                        txt.Attributes.Add("class", "ibox")
                        txt.Attributes.Add("DemographicId", e.Item.DataItem("DemographicId"))
                        txt.Attributes.Add("SurveyQuestionDemographicId", e.Item.DataItem("SurveyQuestionDemographicId"))
                        txt.Attributes.Add("TypeId", e.Item.DataItem("TypeId"))
                        txt.Attributes.Add("IsRequired", e.Item.DataItem("IsRequired"))
                        td.Controls.Add(txt)

                End Select
            End Sub
            Private Function ValidateDemographics(ByVal item As RepeaterItem) As Boolean

                CType(item.FindControl("tdRequireDemographic"), HtmlTableCell).Attributes.Remove("class")

                Select Case CLng(CType(item.FindControl("TypeId"), HtmlInputText).Value)
                    Case 1, 2, 7, 8, 10, 11
                        Dim txt As TextBox = item.FindControl("Demographic")
                        If txt.Text = "" Then
                            CType(item.FindControl("tdRequireDemographic"), HtmlTableCell).Attributes.Add("class", "fieldred")
                            tdRequire.Attributes.Add("class", "fieldreq")
                            Return False
                        End If
                    Case 3
                        Dim dp As DatePicker = item.FindControl("Demographic")
                        If dp.Text = "" Then
                            CType(item.FindControl("tdRequireDemographic"), HtmlTableCell).Attributes.Add("class", "fieldred")
                            tdRequire.Attributes.Add("class", "fieldreq")
                            Return False
                        End If
                    Case 4
                        Dim phone As Phone = item.FindControl("Demographic")
                        If phone.Value = "" Then
                            CType(item.FindControl("tdRequireDemographic"), HtmlTableCell).Attributes.Add("class", "fieldred")
                            tdRequire.Attributes.Add("class", "fieldreq")
                            Return False
                        End If

                    Case 5, 6, 9
                        Dim drp As DropDownList = item.FindControl("Demographic")
                        If drp.SelectedValue = "" Then
                            CType(item.FindControl("tdRequireDemographic"), HtmlTableCell).Attributes.Add("class", "fieldred")
                            tdRequire.Attributes.Add("class", "fieldreq")
                            Return False
                        End If

                End Select
                Return True
            End Function
        End Class

    End Namespace
End Namespace
