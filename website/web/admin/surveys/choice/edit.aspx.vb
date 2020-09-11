Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class admin_survey_choice_Edit
    Inherits AdminPage

    Protected ChoiceId As Integer
    Protected QuestionId As Integer
    Protected SurveyId As Integer
    Protected PageId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("SURVEY")

        ChoiceId = Convert.ToInt32(Request("ChoiceId"))
        SurveyId = Convert.ToInt32(Request("SurveyId"))
        PageId = Convert.ToInt32(Request("PageId"))
        QuestionId = Convert.ToInt32(Request("QuestionId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()


        drpChildQuestionId.DataSource = SurveyQuestionRow.GetSurveyPageQuestions(DB, PageId)
        drpChildQuestionId.DataValueField = "QuestionId"
        drpChildQuestionId.DataTextField = "Name"
        drpChildQuestionId.DataBind()
        drpChildQuestionId.Items.Insert(0, New ListItem("", ""))

        drpSkipToPageId.DataSource = SurveyPageRow.GetAllSurveyPages(DB, SurveyId)
        drpSkipToPageId.DataValueField = "PageId"
        drpSkipToPageId.DataTextField = "Name"
        drpSkipToPageId.DataBind()
        drpSkipToPageId.Items.Insert(0, New ListItem("", ""))

        If ChoiceId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbSurveyQuestionChoice As SurveyQuestionChoiceRow = SurveyQuestionChoiceRow.GetRow(DB, ChoiceId)
        txtName.Text = dbSurveyQuestionChoice.Name
        txtDisplayText.Value = dbSurveyQuestionChoice.DisplayText
        'drpSkipToPageId.SelectedValue = dbSurveyQuestionChoice.SkipToPageId
        'drpChildQuestionId.SelectedValue = dbSurveyQuestionChoice.ChildQuestionId
        chkShowResponseField.Checked = Convert.ToBoolean(dbSurveyQuestionChoice.ShowResponseField)

    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbSurveyQuestionChoice As SurveyQuestionChoiceRow

            If ChoiceId <> 0 Then
                dbSurveyQuestionChoice = SurveyQuestionChoiceRow.GetRow(DB, ChoiceId)
            Else
                dbSurveyQuestionChoice = New SurveyQuestionChoiceRow(DB)
            End If
            dbSurveyQuestionChoice.QuestionId = QuestionId
            dbSurveyQuestionChoice.Name = txtName.Text
            dbSurveyQuestionChoice.DisplayText = txtDisplayText.Value
            If drpSkipToPageId.SelectedValue = "" Then
                dbSurveyQuestionChoice.SkipToPageId = Nothing
            Else
                dbSurveyQuestionChoice.SkipToPageId = drpSkipToPageId.SelectedValue
            End If
            If drpChildQuestionId.SelectedValue = "" Then
                dbSurveyQuestionChoice.ChildQuestionId = Nothing
            Else
                dbSurveyQuestionChoice.ChildQuestionId = drpChildQuestionId.SelectedValue
            End If

            dbSurveyQuestionChoice.ShowResponseField = chkShowResponseField.checked

            If ChoiceId <> 0 Then
                dbSurveyQuestionChoice.Update()
            Else
                ChoiceId = dbSurveyQuestionChoice.Insert
            End If

            DB.CommitTransaction()


            Response.Redirect("default.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&QuestionId=" & QuestionId & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&QuestionId=" & QuestionId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&QuestionId=" & QuestionId & "&ChoiceId=" & ChoiceId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
