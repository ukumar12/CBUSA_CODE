Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected BuilderRegistrationID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BUILDER_REGISTRATIONS")

        BuilderRegistrationID = Convert.ToInt32(Request("BuilderRegistrationID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        drpBuilderID.Datasource = BuilderRow.GetList(DB, "CompanyName")
        drpBuilderID.DataValueField = "BuilderID"
        drpBuilderID.DataTextField = "CompanyName"
        drpBuilderID.Databind()
        drpBuilderID.Items.Insert(0, New ListItem("", ""))

        drpRegistrationStatusID.Datasource = RegistrationStatusRow.GetList(DB, "RegistrationStatus")
        drpRegistrationStatusID.DataValueField = "RegistrationStatusID"
        drpRegistrationStatusID.DataTextField = "RegistrationStatus"
        drpRegistrationStatusID.Databind()
        drpRegistrationStatusID.Items.Insert(0, New ListItem("", ""))

        If BuilderRegistrationID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbBuilderRegistration As BuilderRegistrationRow = BuilderRegistrationRow.GetRow(DB, BuilderRegistrationID)
        txtYearStarted.Text = dbBuilderRegistration.YearStarted
        txtEmployees.Text = dbBuilderRegistration.Employees
        txtHomesBuiltAndDelivered.Text = dbBuilderRegistration.HomesBuiltAndDelivered
        txtHomeStartsLastYear.Text = dbBuilderRegistration.HomeStartsLastYear
        txtHomeStartsNextYear.Text = dbBuilderRegistration.HomeStartsNextYear
        txtPriceRangeMin.Text = dbBuilderRegistration.PriceRangeMin
        txtPriceRangeMax.Text = dbBuilderRegistration.PriceRangeMax
        txtAvgCostPerSqFt.Text = dbBuilderRegistration.AvgCostPerSqFt
        txtAffiliations.Text = dbBuilderRegistration.Affiliations
        txtWhereYouBuild.Text = dbBuilderRegistration.WhereYouBuild
        txtAwards.Text = dbBuilderRegistration.Awards
        txtDirectCostsLastYear.Text = dbBuilderRegistration.DirectCostsLastYear
        txtUnsoldLastYear.Text = dbBuilderRegistration.UnsoldLastYear
        txtUnderConstructionLastYear.Text = dbBuilderRegistration.UnderConstructionLastYear
        dtCompleteDate.Value = dbBuilderRegistration.CompleteDate
        drpBuilderID.SelectedValue = dbBuilderRegistration.BuilderID
        drpRegistrationStatusID.SelectedValue = dbBuilderRegistration.RegistrationStatusID
        rblAcceptsTerms.SelectedValue = dbBuilderRegistration.AcceptsTerms
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbBuilderRegistration As BuilderRegistrationRow

            If BuilderRegistrationID <> 0 Then
                dbBuilderRegistration = BuilderRegistrationRow.GetRow(DB, BuilderRegistrationID)
            Else
                dbBuilderRegistration = New BuilderRegistrationRow(DB)
            End If
            dbBuilderRegistration.YearStarted = txtYearStarted.Text
            dbBuilderRegistration.Employees = txtEmployees.Text
            dbBuilderRegistration.HomesBuiltAndDelivered = txtHomesBuiltAndDelivered.Text
            dbBuilderRegistration.HomeStartsLastYear = txtHomeStartsLastYear.Text
            dbBuilderRegistration.HomeStartsNextYear = IIf(txtHomeStartsNextYear.Text = "", 0, txtHomeStartsNextYear.Text)
            dbBuilderRegistration.PriceRangeMin = txtPriceRangeMin.Text
            dbBuilderRegistration.PriceRangeMax = txtPriceRangeMax.Text
            dbBuilderRegistration.AvgCostPerSqFt = txtAvgCostPerSqFt.Text
            dbBuilderRegistration.Affiliations = txtAffiliations.Text
            dbBuilderRegistration.WhereYouBuild = txtWhereYouBuild.Text
            dbBuilderRegistration.Awards = txtAwards.Text
            dbBuilderRegistration.DirectCostsLastYear = txtDirectCostsLastYear.Text
            dbBuilderRegistration.UnsoldLastYear = txtUnsoldLastYear.Text
            dbBuilderRegistration.UnderConstructionLastYear = txtUnderConstructionLastYear.Text
            dbBuilderRegistration.CompleteDate = dtCompleteDate.Value
            dbBuilderRegistration.BuilderID = drpBuilderID.SelectedValue
            If dbBuilderRegistration.RegistrationStatusID <> drpRegistrationStatusID.SelectedValue Then
                Dim dbStatus As RegistrationStatusRow = RegistrationStatusRow.GetRow(DB, drpRegistrationStatusID.SelectedValue)
                Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, dbBuilderRegistration.BuilderID)
                Dim dbMsg As AutomaticMessagesRow = Nothing
                If dbStatus.RegistrationStep = 2 Then
                    dbMsg = AutomaticMessagesRow.GetRowByTitle(DB, "FinancialsUpdateRequired")
                ElseIf Not dbStatus.IsComplete Then
                    dbMsg = AutomaticMessagesRow.GetRowByTitle(DB, "UpdateRequired")
                End If
                If dbMsg IsNot Nothing Then
                    dbMsg.Send(dbBuilder, vbCrLf & System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName"))
                End If
            End If
            dbBuilderRegistration.RegistrationStatusID = drpRegistrationStatusID.SelectedValue
            dbBuilderRegistration.AcceptsTerms = rblAcceptsTerms.SelectedValue

            If BuilderRegistrationID <> 0 Then
                dbBuilderRegistration.Update()
            Else
                BuilderRegistrationID = dbBuilderRegistration.Insert
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
        Response.Redirect("delete.aspx?BuilderRegistrationID=" & BuilderRegistrationID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

