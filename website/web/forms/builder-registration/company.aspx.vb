Imports Components
Imports DataLayer
Imports System.Linq
Imports System.Data

Partial Class forms_builder_registration_company
    Inherits SitePage

    Private dbBuilder As BuilderRow
    Private dbRegistration As BuilderRegistrationRow
    Private BuilderRegistrationId As Integer
    Private dtSupplyPhase As DataTable

    Protected ReadOnly Property Guid() As String
        Get
            If Request("id") IsNot Nothing Then
                Return Request("id")
            End If
            Return String.Empty
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        dbBuilder = BuilderRow.GetRow(DB, Session("BuilderId"))
        If dbBuilder.BuilderID = Nothing Then
            dbBuilder = BuilderRow.GetBuilderByGuid(DB, Guid)
        End If
        If dbBuilder Is Nothing OrElse dbBuilder.BuilderID = 0 Then
            Response.Redirect("/default.aspx")
        Else
            Session("BuilderId") = dbBuilder.BuilderID
        End If

        dbRegistration = BuilderRegistrationRow.GetRowByBuilder(DB, dbBuilder.BuilderID)
        BuilderRegistrationId = dbRegistration.BuilderRegistrationID

        If Not IsPostBack Then
            If dbRegistration.CompleteDate <> Nothing AndAlso dbRegistration.CompleteDate.Year = Now.Year Then
                btnDashboard.Visible = True
                btnContinue.Visible = False
                btnBack.Text = "Cancel"
                ctrlSteps.Visible = False

                trEULA.Visible = False
                rfvcbAsp.Enabled = False
            Else
                btnDashboard.Visible = False
                btnContinue.Visible = True
                btnBack.Text = "Go Back"
                ctrlSteps.Visible = True
            End If
            LoadFromDb()
        End If
    End Sub

    Private Sub LoadFromDb()

        Dim dt As DataTable


        If BuilderRegistrationId = 0 Then
            Try
                DB.BeginTransaction()

                With dbRegistration
                    .BuilderID = dbBuilder.BuilderID
                    .YearStarted = 0
                    .Employees = 0
                    .HomesBuiltAndDelivered = 0
                    .HomeStartsLastYear = 0
                    .HomeStartsNextYear = 0
                    .SizeRangeMin = 0
                    .SizeRangeMax = 0
                    .PriceRangeMin = 0
                    .PriceRangeMax = 0
                    .AvgCostPerSqFt = 0
                    .RevenueLastYear = 0
                    .RevenueNextYear = 0
                    .TotalCOGS = 0
                    .Affiliations = ""
                    .WhereYouBuild = ""
                    .AcceptsTerms = False
                    .WhereYouBuild = " "
                    .Submitted = Now()
                End With

                dbRegistration.Insert()

                DB.CommitTransaction()

                BuilderRegistrationId = dbRegistration.BuilderRegistrationID


            Catch ex As Exception
                If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                AddError(ErrHandler.ErrorText(ex))
            End Try
        Else
            With dbRegistration
                txtNumYears.Text = .YearStarted
                txtNumEmployees.Text = .Employees
                txtNumDelivered.Text = .HomesBuiltAndDelivered
                txtPriceRangeMin.Text = .PriceRangeMin
                txtPriceRangeMax.Text = .PriceRangeMax
                txtAvgPerFoot.Text = .AvgCostPerSqFt
                txtRevenue.Text = .RevenueLastYear
                txtRevenueProjected.Text = .RevenueNextYear
                txtMemberships.Text = .Affiliations
                txtawards.text = .Awards
                txtAreas.Text = .WhereYouBuild
            End With
        End If

        'load sub-forms here

        'dt = BuilderRegistrationReferenceRow.GetListByRegistration(Me.DB, BuilderRegistrationId)
        'Me.rptReferences.DataSource = dt
        'Me.rptReferences.DataBind()

        'dt = BuilderRegistrationPhaseExpenditureRow.GetListByRegistration(Me.DB, BuilderRegistrationId)
        'Me.rptExpenditures.DataSource = dt
        'Me.rptExpenditures.DataBind()

        'acSupplyPhase.WhereClause = "SupplyPahseId Not In (Select SupplyPhaseId From SupplyPhaseLLCExclusion)"

    End Sub

    Private Function Process() As Boolean
        Dim bError As Boolean = False

        Page.Validate("BuilderFinance")
        If Not Page.IsValid Or bError Then
            Return False
        End If

        Try
            DB.BeginTransaction()

            With dbRegistration
                .YearStarted = txtNumYears.Text
                .Employees = txtNumEmployees.Text
                .HomesBuiltAndDelivered = txtNumDelivered.Text
                .PriceRangeMin = Regex.Replace(txtPriceRangeMin.Text, "[^\d.]", "")
                .PriceRangeMax = Regex.Replace(txtPriceRangeMax.Text, "[^\d.]", "")
                .AvgCostPerSqFt = Regex.Replace(txtAvgPerFoot.Text, "[^\d.]", "")
                .RevenueLastYear = Regex.Replace(txtRevenue.Text, "[^\d.]", "")
                .RevenueNextYear = Regex.Replace(txtRevenueProjected.Text, "[^\d.]", "")
                .Affiliations = txtMemberships.Text
                .WhereYouBuild = txtAreas.Text
                .AcceptsTerms = cbAsp.Checked
            End With

            If dbRegistration.BuilderRegistrationID = 0 Then
                dbRegistration.Insert()
            Else
                dbRegistration.Update()
            End If

            If dbRegistration.CompleteDate = Nothing OrElse dbRegistration.CompleteDate.Year <> Now.Year Then
                Dim statuses As RegistrationStatusCollection = RegistrationStatusRow.GetStatuses(DB)
                Dim currentStatus As RegistrationStatusRow = (From status As RegistrationStatusRow In statuses Where status.RegistrationStatusID = dbRegistration.RegistrationStatusID Select status).FirstOrDefault
                If currentStatus IsNot Nothing Then
                    Dim nextStatus As RegistrationStatusRow = (From status As RegistrationStatusRow In statuses Where status.RegistrationStep = currentStatus.RegistrationStep + 1 Select status).FirstOrDefault
                    If nextStatus IsNot Nothing Then
                        dbRegistration.RegistrationStatusID = nextStatus.RegistrationStatusID
                        dbRegistration.Update()
                    End If
                End If
            End If
            DB.CommitTransaction()

            Return True
        Catch ex As Exception
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try

        Return False
    End Function

    Protected Sub btnContinue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnContinue.Click
        If Process() Then
            Response.Redirect("financial.aspx")
        End If
    End Sub

    Protected Sub btnDashboard_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDashboard.Click
        If Process() Then
            Response.Redirect("/builder/default.aspx")
        End If
    End Sub
End Class
