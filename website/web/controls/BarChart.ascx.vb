Imports Components
Imports System.Linq
Imports InfoSoftGlobal

Partial Class controls_BarChart
    Inherits BaseControl

    Private m_ChartTitle As String
    Public Property ChartTitle() As String
        Get
            Return m_ChartTitle
        End Get
        Set(ByVal value As String)
            m_ChartTitle = value
        End Set
    End Property

    Private m_YSeries As Generic.List(Of String)
    Public ReadOnly Property YSeries() As Generic.List(Of String)
        Get
            If m_YSeries Is Nothing Then
                m_YSeries = New Generic.List(Of String)
            End If
            Return m_YSeries
        End Get
    End Property

    Private m_XSeries As String
    Public Property XSeries() As String
        Get
            Return m_XSeries
        End Get
        Set(ByVal value As String)
            m_XSeries = value
        End Set
    End Property

    Private m_Series As Generic.Dictionary(Of String, Generic.Dictionary(Of String, String))
    Public ReadOnly Property Series() As Generic.Dictionary(Of String, Generic.Dictionary(Of String, String))
        Get
            If m_Series Is Nothing Then
                m_Series = New Generic.Dictionary(Of String, Generic.Dictionary(Of String, String))()
            End If
            Return m_Series
        End Get
    End Property

    Public Sub AddPoint(ByVal SeriesName As String, ByVal xValue As Double, ByVal yValue As Double)
        If Not Series.ContainsKey(SeriesName) Then
            Series.Add(SeriesName, New Generic.Dictionary(Of String, String))
        End If
        Series(SeriesName).Add(xValue, yValue)
    End Sub

    Protected Function GetChartXml() As String
        Dim out As New StringBuilder
        out.AppendLine("<graph xaxisname='" & XSeries & "' rotateNames='0' animation='1' yAxisMaxValue='0' numdivlines='9' divLineColor='CCCCCC' divLineAlpha='80' decimalPrecision='0' showAlternateHGridColor='1' AlternateHGridAlpha='30' AlternateHGridColor='CCCCCC' caption='" & ChartTitle & "'>")
        out.AppendLine("<categories>")
        Dim values As Generic.Dictionary(Of String, String)
        values = (From s In Series Select s.Value).FirstOrDefault
        For Each v In values.Keys
            out.AppendLine("<category name='" & v & "' />")
        Next
        out.AppendLine("</categories>")

        For Each l As String In YSeries
            out.AppendLine("<dataset seriesname='" & l & "'>")
            For Each v As String In Series(l).Values
                out.AppendLine("<value='" & v & "' />")
            Next
            out.AppendLine("</dataset>")
        Next
        out.AppendLine("</graph>")
        Return out.ToString
    End Function

    Protected Sub Page_PreRender1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        ltlChart.Text = FusionCharts.RenderChartHTML("../../FusionCharts/FCF_Line.swf", "", GetChartXml(), "Multi-series Line 2D", "500", "500", False)
    End Sub
End Class
