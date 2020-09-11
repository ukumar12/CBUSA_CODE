Imports Components
Imports DataLayer

Partial Class rebates_sales_history
    Inherits SitePage
    Protected DisplayNewColumn As Boolean = False

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureVendorAccess()
        If Not Request("display") Is Nothing Then
            DisplayNewColumn = True
        End If

        gvSales.BindList = AddressOf BindData
        If Not IsPostBack Then
            BindData()
        End If
    End Sub

    Private Sub BindData()
        Dim ReportDeadlineDays As Integer = DB.ExecuteScalar("Select Value From SysParam Where Name = 'ReportDeadlineDays'")
        Dim DiscrepancyDeadlineDays As Integer = DB.ExecuteScalar("Select Value From SysParam Where Name = 'DiscrepancyDeadlineDays'")
        Dim DiscrepancyResponseDeadlineDays As Integer = DB.ExecuteScalar("Select Value From SysParam Where Name = 'DiscrepancyResponseDeadlineDays'")
        Dim currentQtr As Integer = Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
        Dim lastQtr As Integer = IIf(currentQtr = 1, 4, currentQtr - 1)
        Dim lastYear As Integer = IIf(lastQtr = 4, Now.Year - 1, Now.Year)
        Dim lastQtrEnd As DateTime = (lastQtr * 3) & "/" & Date.DaysInMonth(lastYear, lastQtr * 3) & "/" & lastYear
        Dim deadline As DateTime = lastQtrEnd.AddMonths(1).AddDays(ReportDeadlineDays)
        
        'Changed by Apala (Medullus) on 14.12.2017 for VSO#9155
        Dim CurrentDisputeReponseDeadline As Date = deadline.AddDays(DiscrepancyDeadlineDays + DiscrepancyResponseDeadlineDays)
        'Dim CurrentDisputeReponseDeadline As Date = deadline

        Dim HasCurrentReportingPeriodNotEnded As Boolean = False
        If CurrentDisputeReponseDeadline > Date.Now Then
            HasCurrentReportingPeriodNotEnded = True
        End If


        Dim dt As DataTable = SalesReportRow.GetVendorReports(DB, Session("VendorId"), IncludeCurrentReportingPeriod:=HasCurrentReportingPeriodNotEnded)
        If dt.Rows.Count = 0 Then
            divNoSales.Visible = True
        Else
            gvSales.DataSource = dt
            gvSales.DataBind()
            divNoSales.Visible = False
        End If
    End Sub



    Protected Sub gvSales_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvSales.RowDataBound

        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If
        Dim lnkReport As HyperLink = e.Row.FindControl("lnkReport")
        lnkReport.NavigateUrl = "view-sales.aspx?SalesReportID=" & e.Row.DataItem("SalesReportID")

        Dim ltlFinalResolutionAmount As Literal = e.Row.FindControl("ltlFinalResolutionAmount")
        
        'waiting  for new report approval
        Dim FinalResolutionAmount As Double = 0.0


        Dim dtFinalResolutionAmount As DataTable = SalesReportRow.GetBuildersAndDisputes(DB, e.Row.DataItem("SalesReportID"))
        Dim conn As String = String.Empty
        Dim buildersfromgvReport As String = String.Empty
        If dtFinalResolutionAmount.Rows.Count > 0 Then
            For Each drFinalResolutionAmount In dtFinalResolutionAmount.Rows
                buildersfromgvReport &= conn & Convert.ToString(drFinalResolutionAmount("BuilderID"))
                conn = ","

                If Not IsDBNull(drFinalResolutionAmount("SalesReportDisputeID")) Then
                    If IsDBNull(drFinalResolutionAmount("ResolutionAmount")) Then
                        If IsDBNull(drFinalResolutionAmount("DisputeResponseID")) Then
                            FinalResolutionAmount += 0.0
                        End If
                    Else
                        FinalResolutionAmount += Core.GetDouble(drFinalResolutionAmount("ResolutionAmount"))
                    End If
                Else
                    FinalResolutionAmount += Core.GetDouble(drFinalResolutionAmount("TotalAmount"))
                End If
            Next
        End If
        ' Dim dvUnReportedBuildersAndDisputes As DataView = GetUnReportedBuildersandDisputesFromBuilderView(DB, Session("VendorId"), PeriodQuarter, PeriodYear).DefaultView

        Dim dvUnReportedBuildersAndDisputes As DataView = SalesReportRow.GetUnReportedBuildersAndDisputes(DB, e.Row.DataItem("VendorID"), e.Row.DataItem("PeriodQuarter"), e.Row.DataItem("PeriodYear")).DefaultView

        If buildersfromgvReport <> String.Empty Then
            dvUnReportedBuildersAndDisputes.RowFilter = "BuilderID NOT IN ( " & buildersfromgvReport & ")"
        End If

        dtFinalResolutionAmount = dvUnReportedBuildersAndDisputes.ToTable
        If dtFinalResolutionAmount.Rows.Count > 0 Then
            For Each drFinalResolutionAmount In dtFinalResolutionAmount.Rows
                If Not IsDBNull(drFinalResolutionAmount("SalesReportDisputeID")) Then
                    If IsDBNull(drFinalResolutionAmount("ResolutionAmount")) Then
                        If IsDBNull(drFinalResolutionAmount("DisputeResponseID")) Then
                            FinalResolutionAmount += 0.0
                        End If
                    Else
                        FinalResolutionAmount += Core.GetDouble(drFinalResolutionAmount("ResolutionAmount"))
                    End If
                Else
                    FinalResolutionAmount += 0.0
                End If

            Next
        End If

        ltlFinalResolutionAmount.Text = FormatCurrency(FinalResolutionAmount, 2)



    End Sub
End Class
