Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Survey_Question_Demographic_Edit
    Inherits AdminPage

    Protected SurveyQuestionDemographicId As Integer
    Protected QuestionId As Integer
    Protected SurveyId As Integer
    Protected PageId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("SURVEY")

        QuestionId = Convert.ToInt32(Request("QuestionId"))
        SurveyId = Convert.ToInt32(Request("SurveyId"))
        PageId = Convert.ToInt32(Request("PageId"))

        SurveyQuestionDemographicId = Convert.ToInt32(Request("SurveyQuestionDemographicId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        drpDemographicId.DataSource = SurveyDemographicFieldRow.GetAllSurveyDemographicFields(DB)
        drpDemographicId.DataValueField = "DemographicId"
        drpDemographicId.DataTextField = "Name"
        drpDemographicId.Databind()
        drpDemographicId.Items.Insert(0, New ListItem("", ""))

        If SurveyQuestionDemographicId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbSurveyQuestionDemographic As SurveyQuestionDemographicRow = SurveyQuestionDemographicRow.GetRow(DB, SurveyQuestionDemographicId)
        txtDisplayText.Text = dbSurveyQuestionDemographic.DisplayText
        drpDemographicId.SelectedValue = dbSurveyQuestionDemographic.DemographicId
        chkIsRequired.Checked = dbSurveyQuestionDemographic.IsRequired
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbSurveyQuestionDemographic As SurveyQuestionDemographicRow

            If SurveyQuestionDemographicId <> 0 Then
                dbSurveyQuestionDemographic = SurveyQuestionDemographicRow.GetRow(DB, SurveyQuestionDemographicId)
            Else
                dbSurveyQuestionDemographic = New SurveyQuestionDemographicRow(DB)
            End If
            dbSurveyQuestionDemographic.QuestionId = QuestionId
            dbSurveyQuestionDemographic.DisplayText = txtDisplayText.Text
            dbSurveyQuestionDemographic.DemographicId = drpDemographicId.SelectedValue
            dbSurveyQuestionDemographic.IsRequired = chkIsRequired.Checked

            If SurveyQuestionDemographicId <> 0 Then
                dbSurveyQuestionDemographic.Update()
            Else
                SurveyQuestionDemographicId = dbSurveyQuestionDemographic.Insert
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
        Response.Redirect("delete.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&QuestionId=" & QuestionId & "&SurveyQuestionDemographicId=" & SurveyQuestionDemographicId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
