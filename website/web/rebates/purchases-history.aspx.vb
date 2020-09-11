Imports Components
Imports DataLayer

Partial Class rebates_purchases_history
    Inherits SitePage
    Protected DisplayNewColumn As Boolean = False

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureBuilderAccess()

        If Not Request("display") Is Nothing Then
            DisplayNewColumn = True
        End If

        gvPurchases.BindList = AddressOf BindData
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
        'Ticket #306298 want to change it again to be displayed after deadine ends
	'Changed by Apala (Medullus) on 14.12.2017 for VSO#9155
        Dim CurrentDisputeReponseDeadline As Date = deadline.AddDays(DiscrepancyDeadlineDays + DiscrepancyResponseDeadlineDays)
        'Dim CurrentDisputeReponseDeadline As Date = deadline

        Dim HasCurrentReportingPeriodNotEnded As Boolean = False
        If CurrentDisputeReponseDeadline > Date.Now Then
            HasCurrentReportingPeriodNotEnded = True
        End If

        Dim dt As DataTable = PurchasesReportRow.GetBuilderReports(DB, Session("BuilderId"), IncludeCurrentReportingPeriod:=HasCurrentReportingPeriodNotEnded)
        If dt.Rows.Count = 0 Then
            divNoPurchases.Visible = True
        Else
            gvPurchases.DataSource = dt
            gvPurchases.DataBind()
            divNoPurchases.Visible = False
        End If
    End Sub




    Protected Sub gvPurchases_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvPurchases.RowDataBound
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If

        
        Dim lnkReport As HyperLink = e.Row.FindControl("lnkReport")
        lnkReport.NavigateUrl = "view-purchases.aspx?PurchasesReportID=" & e.Row.DataItem("PurchasesReportID")

        Dim ltlFinalResolutionAmount As Literal = e.Row.FindControl("ltlFinalResolutionAmount")
       


      
        Dim FinalResolutionAmount As Double = 0.0


        Dim dtFinalResolutionAmount As DataTable = PurchasesReportRow.GetVendorsAndDisputes(DB, e.Row.DataItem("PurchasesReportID"))
        If Not IsDBNull(e.Row.DataItem("Submitted")) Then
            If dtFinalResolutionAmount.Rows.Count > 0 Then
                For Each drFinalResolutionAmount In dtFinalResolutionAmount.Rows
                    If Not IsDBNull(drFinalResolutionAmount("SalesReportDisputeID")) Then
                        If IsDBNull(drFinalResolutionAmount("ResolutionAmount")) Then
                            If Not IsDBNull(drFinalResolutionAmount("DisputeResponseID")) Then
                                FinalResolutionAmount += 0.0
                            End If
                        Else
                            FinalResolutionAmount += Core.GetDouble(drFinalResolutionAmount("ResolutionAmount"))
                        End If

                    Else
                        Dim SalesReportID As Object = DB.ExecuteScalar("SELECT TOP 1 SalesReportID FROM SalesReport WHERE Submitted is not null AND PeriodYear=" & drFinalResolutionAmount("PeriodYear") & " AND PeriodQuarter=" & drFinalResolutionAmount("PeriodQuarter") & " AND VendorID=" & drFinalResolutionAmount("VendorID"))
                        Dim sr As SalesReportRow = SalesReportRow.GetRow(DB, SalesReportID)
                        FinalResolutionAmount += sr.GetReportedSales(Session("BuilderId"), False)
                        'FinalResolutionAmount += 100
                    End If
                Next
            End If

            ltlFinalResolutionAmount.Text = FinalResolutionAmount
        Else
            ltlFinalResolutionAmount.Text = "DNR"
            lnkReport.NavigateUrl = ""
            ltlFinalResolutionAmount.Text = "DNR"
            lnkReport.CssClass = "btngold"
            lnkReport.Visible = False
        End If


 

    End Sub
End Class
