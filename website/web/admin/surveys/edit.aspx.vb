Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Admin_Survey_Edit
    Inherits AdminPage

    Protected SurveyId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("SURVEY")

        SurveyId = Convert.ToInt32(Request("SurveyId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If SurveyId = 0 Then
            btnDelete.Visible = False
            chkIsBuilder.Checked = True
            chkIsVendor.Checked = True
            chkIsPIQ.Checked = True
            Exit Sub
        End If

        Dim dbSurvey As SurveyRow = SurveyRow.GetRow(DB, SurveyId)
        txtName.Text = dbSurvey.Name
        txtDisplayTitle.Text = dbSurvey.DisplayTitle
        txtDescription.Text = dbSurvey.Description
        dtStartDate.Value = dbSurvey.StartDate
        dtEndDate.Value = dbSurvey.EndDate
        chkIsActive.Checked = dbSurvey.IsActive
        chkIsFollowUp.Checked = dbSurvey.IsFollowUp
        chkIsBuilder.Checked = dbSurvey.IsBuilder
        chkIsVendor.Checked = dbSurvey.IsVendor
        chkIsPIQ.Checked = dbSurvey.IsPIQ
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbSurvey As SurveyRow

            If SurveyId <> 0 Then
                dbSurvey = SurveyRow.GetRow(DB, SurveyId)
            Else
                dbSurvey = New SurveyRow(DB)
            End If
            dbSurvey.Name = txtName.Text
            dbSurvey.DisplayTitle = txtDisplayTitle.Text
            dbSurvey.Description = txtDescription.Text
            dbSurvey.StartDate = dtStartDate.Value
            dbSurvey.EndDate = dtEndDate.Value
            dbSurvey.IsActive = chkIsActive.Checked
            dbSurvey.IsBuilder = chkIsBuilder.Checked
            dbSurvey.IsVendor = chkIsVendor.Checked
            dbSurvey.IsPIQ = chkIsPIQ.Checked

			If dbSurvey.IsActive Then dbSurvey.IsFollowUp = chkIsFollowUp.Checked Else dbSurvey.IsFollowUp = False

			If dbSurvey.IsFollowUp = True Then DB.ExecuteSQL("update survey set isfollowup = 0")

            If SurveyId <> 0 Then
                dbSurvey.Update()
            Else
                SurveyId = dbSurvey.Insert
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
        Try
            DB.BeginTransaction()

            Dim dbSurvey As SurveyRow
            dbSurvey = SurveyRow.GetRow(DB, SurveyId)
            dbSurvey.IsDeleted = True
            dbSurvey.Update()

            DB.CommitTransaction()
        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
        
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
