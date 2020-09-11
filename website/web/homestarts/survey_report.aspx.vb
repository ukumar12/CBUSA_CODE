
Imports System.Globalization
Imports Newtonsoft.Json
Imports Components
Imports System.Data.SqlClient

Partial Class homestarts_survey_report
    Inherits BasePage

    Public Property C_Month As Integer
        Get
            Return If(ViewState("C_Month") IsNot Nothing, CInt(ViewState("C_Month")), 0)
        End Get
        Set(ByVal value As Integer)
            ViewState("C_YC_Monthear") = value
        End Set
    End Property

    Public Property C_Year As Integer
        Get
            Return If(ViewState("C_Year") IsNot Nothing, CInt(ViewState("C_Year")), 0)
        End Get
        Set(ByVal value As Integer)
            ViewState("C_Year") = value
        End Set
    End Property
    Public Property BuilderID As Integer
        Get
            Return If(ViewState("BuilderID") IsNot Nothing, CInt(ViewState("BuilderID")), 0)
        End Get
        Set(ByVal value As Integer)
            ViewState("BuilderID") = value
        End Set
    End Property

    Public Property BuilderAccountID As Integer
        Get
            Return If(ViewState("BuilderAccountID") IsNot Nothing, CInt(ViewState("BuilderAccountID")), 0)
        End Get
        Set(ByVal value As Integer)
            ViewState("BuilderAccountID") = value
        End Set
    End Property

    Public Property NetworkDataJson As String

    Public Property MarketDataJson As String
    Public Property MarketName As String
    Public Property MarketPer As String

    Public Property NetworkPer As String


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Not IsPostBack Then

                Dim OrganisationID As Integer = Request.QueryString("oid")
                Dim ContactId As Integer = Request.QueryString("cid")
                Dim CompanyName As String = String.Empty
                Dim LLCID As Integer = 0
                ViewState("C_Month") = Val(DateTime.Now.ToString("MM"))
                ViewState("C_Year") = Val(DateTime.Now.ToString("yyyy"))
                Dim CompanyInfo As DataTable = DB.GetDataTable("select CompanyName, BuilderID, LLCID from  Builder   where CRMID=" & OrganisationID & " ")
                If CompanyInfo.Rows.Count > 0 Then
                    CompanyName = CompanyInfo.Rows(0)("CompanyName")
                    ViewState("BuilderID") = CompanyInfo.Rows(0)("BuilderID")
                    lblCompanyName.Text = CompanyName

                    Dim LLCData As DataTable = DB.GetDataTable("SELECT LLCID,LLC FROM LLC  WHERE LLCID = " + CompanyInfo.Rows(0)("LLCID").ToString() + "")
                    If LLCData.Rows.Count > 0 Then
                        MarketName = LLCData.Rows(0)("LLC")
                        LLCID = LLCData.Rows(0)("LLCID")
                        'MarketPer = DB.ExecuteScalar("select (select count(ID) from HomeStartSurvey where surveymonth=" + C_Month.ToString() + " and builderid in  " &
                        '"  (select builderid from builder where LLCID=" + LLCID.ToString() + "))*100/(Select count(builderid) from Builder where isactive=1 and LLCID=" + LLCID.ToString() + ") As MarketPer")

                        'NetworkPer = DB.ExecuteScalar("select (select count(*) from HomeStartSurvey where surveymonth=4)*100/(select count(*) from Builder where isactive=1) as Network")




                    End If
                Else
                    ltrlException.Text = "An Error occurred. Please Try again. If the problem persists, please contact <a href='mailto:customerservice@cbusa.us'>customerservice@cbusa.us</a>."
                    pnlException.Visible = True
                    pnlChart.Visible = False
                    Return
                End If
                Dim BuilderAccount As DataTable = DB.GetDataTable("select FirstName+' '+ LastName as UserName,BuilderAccountID from BuilderAccount where CRMID=" & ContactId & " and BuilderID=" & BuilderID & "")

                If BuilderAccount.Rows.Count > 0 Then
                    ViewState("BuilderAccountID") = BuilderAccount.Rows(0)("BuilderAccountID")
                    lblUserName.Text = BuilderAccount.Rows(0)("UserName")
                Else
                    ltrlException.Text = "An error occurred. Please try again. If the problem persists, please contact <a href='mailto:customerservice@cbusa.us'>customerservice@cbusa.us</a>."
                    pnlException.Visible = True
                    pnlChart.Visible = False
                    Return
                End If


                Dim colors() As String = {"#ee6863", "#90E6E6", "#325897", "#a61d21", "#1f842d", "#bcbc2b", "#a92aba", "#e2d809", "#86d6ae", "#896c6d", "#0e269e", "#6ba4d3"}

                'Network  Data Login
                'Dim NetworkSurveyData As DataTable = DB.GetDataTable("SELECT surveymonth-1 AS surveymonth, sum(surveyData) Actual ,sum(SurveyProjected) Projected  from homestartsurvey  where surveyyear=year(getdate()) group by surveymonth")
                'New Logic with Stored Procedure to Get Network Stats
                Dim params() As SqlParameter = New SqlParameter() _
                    {
                      New SqlParameter("@reportingMonth", SqlDbType.Int) With {.Value = 3},
                      New SqlParameter("@reportingYear", SqlDbType.Int) With {.Value = C_Year}
                    }

                Dim NetworkSurveyData As New DataTable()
                DB.RunProc("rpt_CalculateMonthlyHomeStarts", params, NetworkSurveyData)


                Dim RowCount As Integer = NetworkSurveyData.Rows.Count
                Dim color_code As String = "#0e2d50"
                Dim sReportingMonthAbbr As String = String.Empty

                Dim c_row As Integer = 0
                If NetworkSurveyData.Rows.Count > 0 Then

                    Dim NetworkData = New List(Of ChartProp)
                    For Each row As DataRow In NetworkSurveyData.Rows
                        sReportingMonthAbbr = row("ReportingMonthAbbr")
                        c_row = c_row + 1
                        If c_row = RowCount - 1 Then
                            color_code = "#a61d21"
                        End If
                        If c_row = RowCount Then
                            color_code = "#d7dadb"
                            sReportingMonthAbbr = row("ReportingMonthAbbr") & " (Projected)"
                        End If
                        NetworkData.Add(New ChartProp With {
                        .backgroundColor = color_code,
                        .label = sReportingMonthAbbr,
                        .borderColor = color_code,
                        .borderWidth = "1",
                        .maxBarThickness = 60,
                        .data = New List(Of Integer)({Val(row("TotalStartsScaled"))})
                    })
                        '''row("ReportingMonthAbbr"),
                        '' colors(Val(row("ReportingMonth"))),
                        ''DateTimeFormatInfo.CurrentInfo.MonthNames(Val(row("ReportingMonth")) - 1).Substring(0, 3)
                        NetworkPer = Math.Round(Convert.ToDouble(row("ReportingRate")) * 100, 2)
                    Next row


                    'Dim lastRow As DataRow = NetworkSurveyData.Rows(RowCount - 1)
                    'NetworkData.Add(New ChartProp With {
                    '    .backgroundColor = "#d7dadb",
                    '    .label = "(Projected)",
                    '    .borderColor = "#d7dadb",
                    '    .borderWidth = "1",
                    '    .maxBarThickness = 60,
                    '.data = New List(Of Integer)({Val(lastRow("ReportedOrProjectedTotal"))})
                    '})
                    NetworkDataJson = JsonConvert.SerializeObject(NetworkData)

                    'DateTimeFormatInfo.CurrentInfo.MonthNames(Val(lastRow("ReportingMonth"))).Substring(0, 3) + " (Projected)",
                End If

                'Market  Data Login
                'Dim sql As String = "SELECT surveymonth-1 AS surveymonth, sum(surveyData) Actual ,sum(SurveyProjected) Projected  from homestartsurvey  where surveyyear=year(getdate()) and BuilderID in " _
                '& " (select builderid from Builder where LLCID= " + LLCID.ToString() + " )  group by surveymonth"

                'Dim MarketSurveyData As DataTable = DB.GetDataTable(sql)

                'New Logic with Stored Procedure to Get Network Stats
                Dim params_market() As SqlParameter = New SqlParameter() _
                    {
                      New SqlParameter("@market", SqlDbType.Int) With {.Value = LLCID},
                      New SqlParameter("@reportingMonth", SqlDbType.Int) With {.Value = 3},
                      New SqlParameter("@reportingYear", SqlDbType.Int) With {.Value = C_Year}
                    }

                Dim MarketSurveyData As New DataTable()
                DB.RunProc("rpt_CalculateMonthlyHomeStarts", params_market, MarketSurveyData)
                If MarketSurveyData.Rows.Count > 0 Then

                    RowCount = MarketSurveyData.Rows.Count
                    c_row = 0
                    color_code = "#0e2d50"
                    sReportingMonthAbbr = String.Empty

                    Dim _marketData = New List(Of ChartProp)
                    For Each row As DataRow In MarketSurveyData.Rows

                        sReportingMonthAbbr = row("ReportingMonthAbbr")
                        c_row = c_row + 1
                        If c_row = RowCount - 1 Then
                            color_code = "#a61d21"
                        End If
                        If c_row = RowCount Then
                            color_code = "#d7dadb"
                            sReportingMonthAbbr = row("ReportingMonthAbbr") & " (Projected)"
                        End If
                        _marketData.Add(New ChartProp With {
                        .backgroundColor = color_code,
                        .label = sReportingMonthAbbr,
                        .borderColor = color_code,
                        .borderWidth = "1",
                        .maxBarThickness = 60,
                        .data = New List(Of Integer)({Val(row("TotalStartsScaled"))})
                    })
                        MarketPer = Math.Round(Convert.ToDouble(row("ReportingRate")) * 100, 2)
                    Next row
                    'DateTimeFormatInfo.CurrentInfo.MonthNames(Val(row("ReportingMonth")) - 1).Substring(0, 3),


                    'Dim lastRow As DataRow = MarketSurveyData.Rows(RowCount - 1)
                    '_marketData.Add(New ChartProp With {
                    '    .backgroundColor = "#d7dadb",
                    '    .label = " (Projected)",
                    '    .borderColor = "#d7dadb",
                    '    .borderWidth = "1",
                    '    .maxBarThickness = 60,
                    '.data = New List(Of Integer)({Val(lastRow("ReportedOrProjectedTotal"))})
                    '})
                    MarketDataJson = JsonConvert.SerializeObject(_marketData)

                    'DateTimeFormatInfo.CurrentInfo.MonthNames(Val(lastRow("ReportingMonth"))).Substring(0, 3) + " (Projected)"
                Else
                    MarketDataJson = JsonConvert.SerializeObject(New List(Of ChartProp))
                End If
                pnlChart.Visible = True


            End If
        Catch ex As Exception
            ltrlException.Text = "An Error occurred. Please Try again. If the problem persists, please contact <a href='mailto:customerservice@cbusa.us'>customerservice@cbusa.us</a>."
            pnlException.Visible = True
            pnlChart.Visible = False

        End Try



    End Sub





    Public Sub AlertMessage(ByVal Message As String)
        Dim Msg As String = Message
        Dim sb As New System.Text.StringBuilder()
        sb.Append("<script type = 'text/javascript'>")
        'sb.Append("window.onload=function(){")
        'sb.Append("alert('")
        'sb.Append(Msg)
        'sb.Append("')};")
        sb.Append("alert('" + Msg + "');")
        sb.Append("</script>")
        ClientScript.RegisterClientScriptBlock(Me.GetType(), "alert", sb.ToString())
    End Sub

End Class


Public Class ChartProp
    Public Property label As String
    Public Property backgroundColor As String
    Public Property borderColor As String
    Public Property borderWidth As String
    Public Property maxBarThickness As Integer
    Public Property data As List(Of Int32)
End Class
