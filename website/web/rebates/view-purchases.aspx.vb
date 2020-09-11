Imports Components
Imports DataLayer
Imports System.Linq
Imports System.Data


Partial Class rebates_view_purchases
    Inherits SitePage

    Protected PurchasesReportID As Integer

    Protected dvPurchases As DataView

    Private dtResponses As DataTable
    Private dtReasons As DataTable

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureBuilderAccess()

        PurchasesReportID = Request("PurchasesReportID")
        Dim dbReport As PurchasesReportRow = PurchasesReportRow.GetRow(DB, PurchasesReportID)
        If dbReport.PurchasesReportID = Nothing OrElse dbReport.BuilderID <> Session("BuilderId") Then
            Response.Redirect("purchases-history.aspx")
        Else
            ltlTitle.Text = "Historical Purchases Report: Quarter " & dbReport.PeriodQuarter & ", " & dbReport.PeriodYear
        End If

        If Not IsPostBack Then
            BindData()
        End If
    End Sub

    Private Sub BindData()
        dtResponses = DisputeResponseRow.GetList(DB)
        dtReasons = DisputeResponseReasonRow.GetList(DB)

        dvPurchases = PurchasesReportRow.GetPurchases(DB, PurchasesReportID).DefaultView

        gvReport.DataSource = PurchasesReportRow.GetVendorsAndDisputes(DB, PurchasesReportID)
        gvReport.DataBind()
    End Sub


    Protected Sub gvReport_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvReport.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then
            Exit Sub
        End If

        Dim gvPurchases As GridView = e.Row.FindControl("gvPurchases")
        'gvPurchases.DataSource = dtPurchases.Select("VendorID=" & e.Row.DataItem("VendorID"))
        dvPurchases.RowFilter = "VendorID=" & e.Row.DataItem("VendorID")
        gvPurchases.DataSource = dvPurchases
        gvPurchases.DataBind()
        Dim ltlFinalAmount As Literal = e.Row.FindControl("ltlFinalAmount")
        Dim ltlDispute As Literal = e.Row.FindControl("ltlDispute")

        'If IsDBNull(e.Row.DataItem("ResolutionAmount")) Then
        '    If IsDBNull(e.Row.DataItem("VendorReportedTotal")) Then
        '        ltlFinalAmount.Text = FormatCurrency(Core.GetDouble(e.Row.DataItem("TotalAmount")))
        '    Else
        '        ltlFinalAmount.Text = FormatCurrency(Core.GetDouble(e.Row.DataItem("VendorReportedTotal")))
        '    End If
        'Else
        '    ltlFinalAmount.Text = FormatCurrency(Core.GetDouble(e.Row.DataItem("ResolutionAmount")))
        'End If

        Dim ltlVendorTotal As Literal = e.Row.FindControl("ltlVendorTotal")
        Dim ltlBuilderTotal As Literal = e.Row.FindControl("ltlBuilderTotal")
        Dim SalesReportID As Object = DB.ExecuteScalar("SELECT TOP 1 SalesReportID FROM SalesReport WHERE  PeriodYear=" & e.Row.DataItem("PeriodYear") & " AND PeriodQuarter=" & e.Row.DataItem("PeriodQuarter") & " AND VendorID=" & e.Row.DataItem("VendorID"))
        If Not IsDBNull(SalesReportID) And SalesReportID IsNot Nothing Then
            Dim sr As SalesReportRow = SalesReportRow.GetRow(DB, SalesReportID)
            ltlVendorTotal.Text = sr.GetReportedSales(Session("BuilderId"))
        Else
            ltlVendorTotal.Text = "DNR"
        End If

        'If IsDBNull(e.Row.DataItem("ResolutionAmount")) Then
        '    If IsDBNull(e.Row.DataItem("VendorReportedTotal")) Then

        '        ltlFinalAmount.Text = FormatCurrency(Core.GetDouble(e.Row.DataItem("TotalAmount")))
        '    Else
        '        ltlFinalAmount.Text = FormatCurrency(Core.GetDouble(e.Row.DataItem("VendorReportedTotal")))
        '    End If
        'Else
        '    ltlFinalAmount.Text = FormatCurrency(Core.GetDouble(e.Row.DataItem("ResolutionAmount")))
        'End If




        Dim pr As PurchasesReportRow = PurchasesReportRow.GetRow(DB, PurchasesReportID)
        ltlBuilderTotal.Text = pr.GetReportedPurchases(e.Row.DataItem("VendorID"))

        If Not IsDBNull(e.Row.DataItem("SalesReportDisputeID")) Then
            Dim resp As String = (From dr As DataRow In dtResponses.AsEnumerable Where Core.GetInt(dr("DisputeResponseID")) = Core.GetInt(e.Row.DataItem("DisputeResponseID")) Select Core.GetString(dr("DisputeResponse"))).FirstOrDefault
            Dim reason As String = (From dr As DataRow In dtReasons.AsEnumerable Where Core.GetInt(dr("DisputeResponseReasonID")) = Core.GetInt(e.Row.DataItem("DisputeResponseReasonID")) Select Core.GetString(dr("DisputeResponseReason"))).FirstOrDefault

            If resp = String.Empty Then
                resp = "<em>No Vendor Response</em>"
                reason = Nothing
                'ltlFinalAmount.Text = "$0.00"
                ltlFinalAmount.Text = IIf(ltlVendorTotal.Text = "DNR", "$0.00", ltlVendorTotal.Text)       '------ Changed by Apala on 31.10.2017 for VSO#8456
            ElseIf reason = String.Empty Then
                reason = "<em>No Reason Specified</em>"
            End If

            ltlDispute.Text = "<b>Disputed</b><br/><table style=""margin:5px;"" cellpadding=""3"" cellspacing=""0"" border=""0"">" _
                & "<tr><td style=""white-space:nowrap;""><b>Vendor Response:</b></td><td>" & resp & "</td></tr>"

            If reason <> Nothing Then
                ltlDispute.Text &= "<tr><td><b>Reason:</b></td><td>" & reason & "</td></tr>"
            End If
            If IsDBNull(e.Row.DataItem("ResolutionAmount")) Then
                If Not IsDBNull(e.Row.DataItem("DisputeResponseID")) Then
                    ltlFinalAmount.Text = FormatCurrency(0.0)
                End If
            Else
                ltlFinalAmount.Text = FormatCurrency(Core.GetDouble(e.Row.DataItem("ResolutionAmount")))
            End If


            If Not IsDBNull(e.Row.DataItem("ResolutionAmount")) Then
                ltlDispute.Text &= "<tr><td style=""white-space:nowrap;""><b>Resolution Amount:</b></td><td>" & FormatCurrency(e.Row.DataItem("ResolutionAmount")) & "</td></tr>"
            End If

            ltlDispute.Text &= "<tr><td style=""white-space:nowrap;""><b>Builder Comments:</b></td><td>" & Core.GetString(e.Row.DataItem("BuilderComments")) & "</td></tr>"
            ltlDispute.Text &= "<tr><td style=""white-space:nowrap;""><b>Vendor Comments:</b></td><td>" & Core.GetString(e.Row.DataItem("VendorComments")) & "</td></tr>"
            ltlDispute.Text &= "</table>"
        Else
            ltlDispute.Text = "<em>Not Disputed</em>"
            Dim sr As SalesReportRow = SalesReportRow.GetRow(DB, SalesReportID)
            ltlFinalAmount.Text = sr.GetReportedSales(Session("BuilderId"))
        End If
    End Sub
End Class
