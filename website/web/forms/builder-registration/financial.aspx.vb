Imports Components
Imports DataLayer
Imports InfoSoftGlobal
Imports System.Linq
Imports System.Data

Partial Class forms_builder_registration_financial
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
            Response.Redirect("default.aspx")
        Else
            Session("BuilderId") = dbBuilder.BuilderID
        End If

        dbRegistration = BuilderRegistrationRow.GetRowByBuilder(DB, dbBuilder.BuilderID)
        BuilderRegistrationId = dbRegistration.BuilderRegistrationID

        If dbRegistration.CompleteDate = Nothing OrElse dbRegistration.CompleteDate.Year <> Now.Year Then
            btnContinue.Visible = True
            btnDashboard.Visible = False
            ctrlSteps.Visible = True
            btnBack.Text = "Go Back"
        Else
            btnBack.Text = "Cancel"
            btnContinue.Visible = False
            btnDashboard.Visible = True
            ctrlSteps.Visible = False
        End If

        If Not IsPostBack Then
            LoadFromDb()
        End If
    End Sub

    Private Sub LoadFromDb()

        With dbRegistration
            If .HomeStartsLastYear > 0 Then txtNumStarts.Text = .HomeStartsLastYear
            If .HomeStartsNextYear > 0 Then txtNumProjected.Text = .HomeStartsNextYear
            If .ClosingsLastYear > 0 Then txtNumClosings.text = .ClosingsLastYear
            If .DirectCostsLastYear > 0 Then txtDirectcosts.text = .DirectCostsLastYear
            If .UnsoldLastYear > 0 Then txtunsold.text = .UnsoldLastYear
            If .UnderConstructionLastYear > 0 Then txtunderconstruction.text = .UnderConstructionLastYear
        End With
    End Sub

    Private Function Process() As Boolean
        Page.Validate("BuilderFinance")
        If Not Page.IsValid Then
            Return False
        End If

        Try
            DB.BeginTransaction()

            With dbRegistration
                .HomeStartsLastYear = txtNumStarts.Text
                .HomeStartsNextYear = txtNumProjected.Text
                .ClosingsLastYear = txtNumClosings.Text
                .DirectCostsLastYear = Regex.Replace(txtDirectCosts.Text, "[^\d.]", "")
                .UnsoldLastYear = txtUnsold.Text
                .UnderConstructionLastYear = txtUnderConstruction.Text
            End With

            dbRegistration.Update()

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
            Response.Redirect("payment.aspx")
        End If
    End Sub

    Protected Sub btnDashboard_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDashboard.Click
        If Process() Then
            Response.Redirect("/default.aspx")
        End If
    End Sub


#Region "Chart"
    Protected Function CreateMoneyTrend() As String
        'create the chart
        Return FusionCharts.RenderChartHTML("../../FusionCharts/FCF_Line.swf", "", LineChartXML(), "Multi-series Line 2D", "500", "500", False)
    End Function

    Protected Function LineChartXML() As String
        Dim XML As String = String.Empty
        Dim dt As DataTable
        Dim i As Integer = 12
        Dim previousHUD As Integer = 5
        Dim currentHUD As Integer = 0
        Dim previousUnsold As Integer = 3
        Dim previousHUC As Decimal = 0D
        Dim currentHUC As Decimal = 0D
        Dim monthDC As Decimal = 15757.58D

        Dim row As DataRow

        dt = Me.DB.GetDataTable(LineChartSQL(SysParam.GetValue(DB, "SampleChartBuilderId")))

        XML &= "<graph caption='Estimated DC' subcaption='Past 12 Months' xAxisName='Month' yAxisMinValue='15000' yAxisName='' numberPrefix='$' showNames='1' showValues='0' rotateNames='1' showColumnShadow='1' animation='1' showAlternateHGridColor='1' AlternateHGridColor='ff5904' divLineColor='ff5904' divLineAlpha='20' alternateHGridAlpha='5' canvasBorderColor='666666' baseFontColor='666666'>"

        For Each row In dt.Rows

            currentHUD = previousHUD + CType(row.Item(1), Integer) - CType(row.Item(3), Integer) - CType(row.Item(4), Integer) + previousUnsold
            currentHUC = ((currentHUD + previousHUD) / 2) * monthDC

            XML &= "<set name='" & Now.Date.AddMonths(-1 * i).ToString("MMMM") & " " & Now.Date.AddMonths(-1 * i).ToString("yyyy") & "' value='" & currentHUC & "' />"

            previousHUD = currentHUD
            previousHUC = currentHUC
            previousUnsold = CType(row.Item(4), Integer)

            i = i - 1

        Next

        XML &= "</graph>"

        Return XML
    End Function

    Protected Function LineChartSQL(ByVal BuilderID As Integer) As String
        Dim SQL As String = String.Empty

        SQL &= "SELECT" & vbCrLf
        SQL &= " TimePeriodDate, " & vbCrLf
        SQL &= "  StartedUnits," & vbCrLf
        SQL &= "  SoldUnits," & vbCrLf
        SQL &= "  ClosingUnits," & vbCrLf
        SQL &= "  UnsoldUnits" & vbCrLf
        SQL &= "FROM" & vbCrLf
        SQL &= "  SampleMonthlyStat" & vbCrLf
        SQL &= "ORDER BY" & vbCrLf
        SQL &= "  TimePeriodDate" & vbCrLf

        Return SQL
    End Function
#End Region

End Class
