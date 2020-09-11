Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected SurveyCategoryId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("SURVEYS")

        SurveyCategoryId = Convert.ToInt32(Request("SurveyCategoryId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If SurveyCategoryId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbSurveyCategory As SurveyCategoryRow = SurveyCategoryRow.GetRow(DB, SurveyCategoryId)
        txtDescription.Text = dbSurveyCategory.Description
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbSurveyCategory As SurveyCategoryRow

            If SurveyCategoryId <> 0 Then
                dbSurveyCategory = SurveyCategoryRow.GetRow(DB, SurveyCategoryId)
            Else
                dbSurveyCategory = New SurveyCategoryRow(DB)
            End If
            dbSurveyCategory.Description = txtDescription.Text

            If SurveyCategoryId <> 0 Then
                dbSurveyCategory.Update()
            Else
                SurveyCategoryId = dbSurveyCategory.Insert
            End If

            DB.CommitTransaction()


            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?SurveyCategoryId=" & SurveyCategoryId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
