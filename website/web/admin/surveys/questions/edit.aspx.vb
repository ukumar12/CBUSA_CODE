Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Admin_Survey_Question_Edit
    Inherits AdminPage

    Protected SurveyId As Integer
    Protected PageId As Integer
    Protected QuestionId As Integer


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("SURVEY")

        SurveyId = Convert.ToInt32(Request("SurveyId"))
        PageId = Convert.ToInt32(Request("PageId"))
        QuestionId = Convert.ToInt32(Request("QuestionId"))
        If Not IsPostBack Then

            drpQuestionTypeId.Attributes.Add("onchange", "QuestionTypeChange(this.value);")

            LoadFromDB()
        End If
        Dim s As String = "<script> if (window.addEventListener) {window.addEventListener('load', AJAXQuestion, false);} else if (window.attachEvent) {	window.attachEvent('onload', AJAXQuestion);}</script>"
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "onload", s)

    End Sub

    Private Sub LoadFromDB()
        drpQuestionTypeId.DataSource = SurveyQuestionRow.GetAllSurveyQuestionTypes(DB)
        drpQuestionTypeId.DataValueField = "QuestionTypeId"
        drpQuestionTypeId.DataTextField = "Name"
        drpQuestionTypeId.Databind()
        drpQuestionTypeId.Items.Insert(0, New ListItem("", ""))

        If QuestionId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbSurveyQuestion As SurveyQuestionRow = SurveyQuestionRow.GetRow(DB, QuestionId)
        txtName.Text = dbSurveyQuestion.Name
        txtText.Value = dbSurveyQuestion.Text
        drpQuestionTypeId.SelectedValue = dbSurveyQuestion.QuestionTypeId
        chkIsRequired.Checked = dbSurveyQuestion.IsRequired
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If SaveForm() Then Response.Redirect("default.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Function SaveForm()

        If Not IsValid Then Return False
        Try
            DB.BeginTransaction()

            Dim dbSurveyQuestion As SurveyQuestionRow

            If QuestionId <> 0 Then
                dbSurveyQuestion = SurveyQuestionRow.GetRow(DB, QuestionId)
            Else
                dbSurveyQuestion = New SurveyQuestionRow(DB)
            End If
            dbSurveyQuestion.PageId = PageId
            dbSurveyQuestion.Name = txtName.Text
            dbSurveyQuestion.Text = txtText.Value
            dbSurveyQuestion.QuestionTypeId = drpQuestionTypeId.SelectedValue
            dbSurveyQuestion.IsRequired = chkIsRequired.Checked

            If QuestionId <> 0 Then
                dbSurveyQuestion.Update()
            Else
                QuestionId = dbSurveyQuestion.Insert
            End If

            DB.CommitTransaction()
            Return True

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()

            AddError(ErrHandler.ErrorText(ex))
        End Try
        Return False
    End Function


    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&QuestionId=" & QuestionId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnChoices_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnChoices.Click
        If SaveForm() Then
            Response.Redirect("/admin/surveys/choice/default.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&QuestionId=" & QuestionId & "&" & GetPageParams(FilterFieldType.All))
        End If
    End Sub

    Protected Sub btnCategories_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCategories.Click
        If SaveForm() Then
            Response.Redirect("/admin/surveys/category/default.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&QuestionId=" & QuestionId & "&" & GetPageParams(FilterFieldType.All))
        End If
    End Sub

    Protected Sub btnDemographic_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDemographic.Click
        If SaveForm() Then
            Response.Redirect("/admin/surveys/demographic/default.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&QuestionId=" & QuestionId & "&" & GetPageParams(FilterFieldType.All))
        End If
    End Sub
End Class
