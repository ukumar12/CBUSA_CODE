Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Admin_Survey_Page_Edit
    Inherits AdminPage

    Protected PageId As Integer
    Protected SurveyId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("SURVEY")

        PageId = Convert.ToInt32(Request("PageId"))
        SurveyId = Convert.ToInt32(Request("SurveyId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If PageId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbSurveyPage As SurveyPageRow = SurveyPageRow.GetRow(DB, PageId)
        txtName.Text = dbSurveyPage.Name
        txtDisplayName.Text = dbSurveyPage.DisplayName
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbSurveyPage As SurveyPageRow

            If PageId <> 0 Then
                dbSurveyPage = SurveyPageRow.GetRow(DB, PageId)
            Else
                dbSurveyPage = New SurveyPageRow(DB)
            End If
            dbSurveyPage.SurveyId = SurveyId
            dbSurveyPage.Name = txtName.Text
            dbSurveyPage.DisplayName = txtDisplayName.Text

            If PageId <> 0 Then
                dbSurveyPage.Update()
            Else
                PageId = dbSurveyPage.Insert
            End If

            DB.CommitTransaction()


            Response.Redirect("default.aspx?SurveyId=" & SurveyId & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?SurveyId=" & SurveyId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
