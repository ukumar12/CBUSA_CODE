Option Strict Off

Imports Components
Imports DataLayer
Imports InfoSoftGlobal
Partial Class _default
    Inherits ModuleControl

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

    End Sub


    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack Then


        End If

    End Sub

    Protected Sub LoadVendors()

    End Sub

    Protected Sub LoadSupplyPhases()

    End Sub

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


        dt = Me.DB.GetDataTable(LineChartSQL)

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

    Protected Function LineChartSQL() As String

        Dim SQL As String = String.Empty

        SQL &= "SELECT" & vbCrLf
        SQL &= " TimePeriodDate, " & vbCrLf
        SQL &= "  StartedUnits," & vbCrLf
        SQL &= "  SoldUnits," & vbCrLf
        SQL &= "  ClosingUnits," & vbCrLf
        SQL &= "  UnsoldUnits" & vbCrLf
        SQL &= "FROM" & vbCrLf
        SQL &= "  BuilderMonthlyStats" & vbCrLf
        SQL &= "WHERE" & vbCrLf
        SQL &= "  BuilderID=" & DB.Number(Session("BuilderID")) & vbCrLf
        SQL &= "ORDER BY" & vbCrLf
        SQL &= "  TimePeriodDate" & vbCrLf

        Return SQL

    End Function

End Class
