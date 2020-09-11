Imports Components
Imports DataLayer
Imports System.Linq

Partial Class rebates_view_sales
    Inherits SitePage

    Protected SalesReportID As Integer

    Protected dvInvoices As DataView
    Protected dvInvoicesUnreported As DataView

    Private dtResponses As DataTable
    Private dtReasons As DataTable
    Private dtBuildersAndDisputes As DataTable
    Private PeriodQuarter As Integer
    Private PeriodYear As Integer
    Protected dvUnReportedBuildersAndDisputes As DataView
    Private buildersfromgvReport As String = String.Empty
    Protected dtUnReportedBuildersAndDisputes As DataTable
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureVendorAccess()

        SalesReportID = Request("SalesReportID")
        Dim dbReport As SalesReportRow = SalesReportRow.GetRow(DB, SalesReportID)
        If dbReport.SalesReportID = Nothing OrElse dbReport.VendorID <> Session("VendorId") Then
            Response.Redirect("sales-history.aspx")
        Else
            PeriodQuarter = dbReport.PeriodQuarter
            PeriodYear = dbReport.PeriodYear

            ltlTitle.Text = "Historical Sales Report: Quarter " & PeriodQuarter & ", " & PeriodYear
        End If

        If Not IsPostBack Then
            BindData()
        End If
    End Sub

    Private Sub BindData()
        dtResponses = DisputeResponseRow.GetList(DB)
        dtReasons = DisputeResponseReasonRow.GetList(DB)

        dvInvoices = SalesReportRow.GetInvoices(DB, SalesReportID).DefaultView
        'dtBuildersAndDisputes = SalesReportRow.GetBuildersAndDisputes(DB, SalesReportID)
        dtBuildersAndDisputes = GetReportedBuildersandDisputes(DB, SalesReportID)
        'dvInvoicesUnreported = SalesReportRow.GetInvoices(DB, SalesReportID).DefaultView
        dvUnReportedBuildersAndDisputes = GetUnReportedBuildersandDisputesFromBuilderView(DB, Session("VendorId"), PeriodQuarter, PeriodYear).DefaultView
        Dim conn As String = String.Empty
        For Each dr As DataRow In dtBuildersAndDisputes.Rows
            buildersfromgvReport &= conn & Convert.ToString(dr("BuilderID"))
            conn = ","
        Next
        If buildersfromgvReport <> String.Empty Then
            dvUnReportedBuildersAndDisputes.RowFilter = "BuilderID NOT IN ( " & buildersfromgvReport & ")"
        End If

        dtUnReportedBuildersAndDisputes = dvUnReportedBuildersAndDisputes.ToTable

        Dim AllReportedAndUnreportedBuildersandDisputes As DataTable

        Dim cols() As String = {"ReportedByVendor", "SalesReportID", "PurchasesReportID", "PeriodQuarter", "PeriodYear", "SalesReportDisputeID", "DisputeResponseID", "disputeResponseReasonID", "BuilderComments", "VendorComments", "ResolutionAmount", "TotalAmount", "VendorID", "builderid", "VendorReportedTotal", "CompanyName"}

       
        AllReportedAndUnreportedBuildersandDisputes = MergeData(dtBuildersAndDisputes, dtUnReportedBuildersAndDisputes, cols)
        
        gvReport.DataSource = AllReportedAndUnreportedBuildersandDisputes
        gvReport.DataBind()

        If AllReportedAndUnreportedBuildersandDisputes.Rows.Count = 0 Then
            gvReport.EmptyDataText = "No Sales  reported for this Quarter"
        End If

    End Sub


    Protected Sub gvReport_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvReport.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then
            Exit Sub
        End If

        Dim gvInvoices As GridView = e.Row.FindControl("gvInvoices")
        Dim ltlFinalAmount As Literal = e.Row.FindControl("ltlFinalAmount")
        Dim ltlDispute As Literal = e.Row.FindControl("ltlDispute")
        Dim ltlVendorTotal As Literal = e.Row.FindControl("ltlVendorTotal")
        Dim ltlBuilderTotal As Literal = e.Row.FindControl("ltlBuilderTotal")

        'gvPurchases.DataSource = dtPurchases.Select("VendorID=" & e.Row.DataItem("VendorID"))
        dvInvoices.RowFilter = "BuilderID=" & e.Row.DataItem("BuilderID")
        gvInvoices.DataSource = dvInvoices
        gvInvoices.DataBind()

        If e.Row.DataItem("ReportedByVendor") Then
            If Not IsDBNull(e.Row.DataItem("SalesReportID")) Then
                Dim sr As SalesReportRow = SalesReportRow.GetRow(DB, e.Row.DataItem("SalesReportID"))
                ltlVendorTotal.Text = sr.GetReportedSales(e.Row.DataItem("BuilderId"))
            Else
                ltlVendorTotal.Text = "DNR"
            End If

            Dim PurchaseReportID As Object = DB.ExecuteScalar("SELECT TOP 1 PurchasesReportID FROM PurchasesReport WHERE PeriodYear=" & e.Row.DataItem("PeriodYear") & " AND PeriodQuarter=" & e.Row.DataItem("PeriodQuarter") & " AND BuilderID=" & e.Row.DataItem("BuilderId"))
            If Not IsDBNull(PurchaseReportID) Then
                Dim pr As PurchasesReportRow = PurchasesReportRow.GetRow(DB, PurchaseReportID)
                ltlBuilderTotal.Text = pr.GetReportedPurchases(Session("VendorID"))
            Else
                ltlBuilderTotal.Text = "DNR"
            End If

            If Not IsDBNull(e.Row.DataItem("SalesReportDisputeID")) Then
                Dim resp As String = (From dr As DataRow In dtResponses.AsEnumerable Where Core.GetInt(dr("DisputeResponseID")) = Core.GetInt(e.Row.DataItem("DisputeResponseID")) Select Core.GetString(dr("DisputeResponse"))).FirstOrDefault
                Dim reason As String = (From dr As DataRow In dtReasons.AsEnumerable Where Core.GetInt(dr("DisputeResponseReasonID")) = Core.GetInt(e.Row.DataItem("DisputeResponseReasonID")) Select Core.GetString(dr("DisputeResponseReason"))).FirstOrDefault

                If resp = String.Empty Then
                    resp = "<em>No Vendor Response</em>"
                    reason = Nothing
                ElseIf reason = String.Empty Then
                    reason = "<em>No Reason Specified</em>"
                End If
                If IsDBNull(e.Row.DataItem("ResolutionAmount")) Then
                    If IsDBNull(e.Row.DataItem("DisputeResponseID")) Then
                        'ltlFinalAmount.Text = FormatCurrency(0.0)
                        ltlFinalAmount.Text = IIf(ltlVendorTotal.Text = "DNR", "$0.00", ltlVendorTotal.Text)            '---------------- Changed by Apala for mGuard#T-10048
                    End If
                Else
                    ltlFinalAmount.Text = FormatCurrency(Core.GetDouble(e.Row.DataItem("ResolutionAmount")))
                End If



                ltlDispute.Text = "<b>Disputed</b><br/><table style=""margin:5px;"" cellpadding=""3"" cellspacing=""0"" border=""0"">" _
                    & "<tr><td style=""white-space:nowrap;""><b>Vendor Response:</b></td><td>" & resp & "</td></tr>"

                If reason <> Nothing Then
                    ltlDispute.Text &= "<tr><td><b>Reason:</b></td><td>" & reason & "</td></tr>"
                End If

                If Not IsDBNull(e.Row.DataItem("ResolutionAmount")) Then
                    ltlDispute.Text &= "<tr><td style=""white-space:nowrap;""><b>Resolution Amount:</b></td><td>" & FormatCurrency(e.Row.DataItem("ResolutionAmount")) & "</td></tr>"
                End If

                ltlDispute.Text &= "<tr><td style=""white-space:nowrap;""><b>Builder Comments:</b></td><td>" & Core.GetString(e.Row.DataItem("BuilderComments")) & "</td></tr>"
                ltlDispute.Text &= "<tr><td style=""white-space:nowrap;""><b>Vendor Comments:</b></td><td>" & Core.GetString(e.Row.DataItem("VendorComments")) & "</td></tr>"
                ltlDispute.Text &= "</table>"
            Else
                ltlDispute.Text = "<em>Not Disputed</em>"
                ltlFinalAmount.Text = FormatCurrency(Core.GetDouble(e.Row.DataItem("TotalAmount")))
            End If
        Else


            Dim SalesReportID As Object = DB.ExecuteScalar("SELECT TOP 1 SalesReportID FROM SalesReport WHERE     PeriodYear=" & e.Row.DataItem("PeriodYear") & " AND PeriodQuarter=" & e.Row.DataItem("PeriodQuarter") & " AND VendorID=" & e.Row.DataItem("VendorID"))
            If Not IsDBNull(SalesReportID) And SalesReportID IsNot Nothing Then
                Dim sr As SalesReportRow = SalesReportRow.GetRow(DB, SalesReportID)
                ltlVendorTotal.Text = sr.GetReportedSales(Session("BuilderId"))
            Else
                ltlVendorTotal.Text = "DNR"
            End If



            Dim pr As PurchasesReportRow = PurchasesReportRow.GetRow(DB, e.Row.DataItem("PurchasesReportID"))
            ltlBuilderTotal.Text = pr.GetReportedPurchases(e.Row.DataItem("VendorID"))

            If Not IsDBNull(e.Row.DataItem("SalesReportDisputeID")) Then
                Dim resp As String = (From dr As DataRow In dtResponses.AsEnumerable Where Core.GetInt(dr("DisputeResponseID")) = Core.GetInt(e.Row.DataItem("DisputeResponseID")) Select Core.GetString(dr("DisputeResponse"))).FirstOrDefault
                Dim reason As String = (From dr As DataRow In dtReasons.AsEnumerable Where Core.GetInt(dr("DisputeResponseReasonID")) = Core.GetInt(e.Row.DataItem("DisputeResponseReasonID")) Select Core.GetString(dr("DisputeResponseReason"))).FirstOrDefault

                If resp = String.Empty Then
                    resp = "<em>No Vendor Response</em>"
                    reason = Nothing
                ElseIf reason = String.Empty Then
                    reason = "<em>No Reason Specified</em>"
                End If

                ltlDispute.Text = "<b>Disputed</b><br/><table style=""margin:5px;"" cellpadding=""3"" cellspacing=""0"" border=""0"">" _
                    & "<tr><td style=""white-space:nowrap;""><b>Vendor Response:</b></td><td>" & resp & "</td></tr>"

                If reason <> Nothing Then
                    ltlDispute.Text &= "<tr><td><b>Reason:</b></td><td>" & reason & "</td></tr>"
                End If


                If IsDBNull(e.Row.DataItem("ResolutionAmount")) Then
                    If IsDBNull(e.Row.DataItem("DisputeResponseID")) Then
                        'ltlFinalAmount.Text = FormatCurrency(0.0)
                        ltlFinalAmount.Text = IIf(ltlVendorTotal.Text = "DNR", "$0.00", ltlVendorTotal.Text)                '---------------- Changed by Apala for mGuard#T-10048
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
                ltlFinalAmount.Text = sr.GetReportedSales(e.Row.DataItem("BuilderID"))
            End If


        End If


    End Sub


    Protected Sub gvReportNotReported_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvReportNotReported.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then
            Exit Sub
        End If

        
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
       
    End Sub


    Protected Function GetReportedBuildersandDisputes(ByVal db As Database, ByVal SalesReportID As Integer) As DataTable
        Dim sql As String = String.Empty

        sql = "SELECT  "
        sql = sql & " 1 as ReportedByVendor ,"
        sql = sql & "    r.SalesReportID AS SalesReportID , "
        sql = sql & "   0  AS  PurchasesReportID , "
        sql = sql & "       r.PeriodQuarter AS PeriodQuarter , "
        sql = sql & "       r.PeriodYear AS PeriodYear , "
        sql = sql & "       srd.SalesReportDisputeID AS SalesReportDisputeID , "
        sql = sql & "       srd.DisputeResponseID AS DisputeResponseID , "
        sql = sql & "       srd.DisputeResponseReasonID AS DisputeResponseReasonID , "
        sql = sql & "       srd.BuilderComments AS BuilderComments , "
        sql = sql & "       srd.VendorComments AS VendorComments , "
        sql = sql & "       srd.ResolutionAmount AS ResolutionAmount, "
        sql = sql & "       t.TotalAmount AS TotalAmount ,  "
        sql = sql & "       r.VendorID AS VendorID , "
        sql = sql & "       t.builderid   AS builderid, "
        sql = sql & "       t.TotalAmount AS VendorReportedTotal ,  "
        sql = sql & "       (SELECT CompanyName "
        sql = sql & "        FROM   Builder "
        sql = sql & "        WHERE  BuilderID = t.BuilderID) AS CompanyName "
        sql = sql & "FROM   SalesReport r "
        sql = sql & "       INNER JOIN SalesReportBuilderTotalAmount t ON r.SalesReportID = t.SalesReportID "
        sql = sql & "       LEFT OUTER JOIN SalesReportDispute srd ON srd.SalesReportID = r.SalesReportID AND "
        sql = sql & "                                                 srd.BuilderID = t.BuilderID "
        'sql = sql & "WHERE r.submitted is not null and  r.SalesReportID = " & SalesReportID
        sql = sql & "WHERE  r.SalesReportID = " & SalesReportID
        Return db.GetDataTable(sql)
    End Function
    Protected Function GetUnReportedBuildersandDisputesFromBuilderView(ByVal db As Database, ByVal VendorID As Integer, ByVal PeriodQuarter As Integer, ByVal periodYear As Integer) As DataTable
        Dim sql As String = String.Empty
        sql = "SELECT "
        sql = sql & " 0 as ReportedByVendor ,"
        sql = sql & " 0 as SalesReportID ,"
        sql = sql & " r.PurchasesReportID AS PurchasesReportID , "
        sql = sql & "       r.PeriodQuarter AS PeriodQuarter , "
        sql = sql & "       r.PeriodYear AS PeriodYear , "
        sql = sql & "       d.SalesReportDisputeID AS SalesReportDisputeID , "
        sql = sql & "       d.DisputeResponseID AS DisputeResponseID , "
        sql = sql & "       d.DisputeResponseReasonID AS DisputeResponseReasonID , "
        sql = sql & "       d.BuilderComments AS BuilderComments , "
        sql = sql & "       d.VendorComments AS VendorComments , "
        sql = sql & "       d.ResolutionAmount AS ResolutionAmount, "
        sql = sql & "       t.TotalAmount AS TotalAmount ,  "
        sql = sql & "       t.VendorID AS VendorID, "
        sql = sql & "       r.BuilderID AS builderid, "
        sql = sql & "      d.VendorReportedTotal AS VendorReportedTotal, "
        sql = sql & "       (SELECT CompanyName "
        sql = sql & "        FROM   Builder "
        sql = sql & "        WHERE  BuilderID = r.BuilderID) AS CompanyName "
        sql = sql & "FROM   PurchasesReport r "
        sql = sql & "       INNER JOIN PurchasesReportVendorTotalAmount t ON r.PurchasesReportID = t.PurchasesReportID "
        sql = sql & "       LEFT OUTER JOIN (SELECT srt.TotalAmount AS VendorReportedTotal, "
        sql = sql & "                               sr.PeriodYear, "
        sql = sql & "                               sr.PeriodQuarter, "
        sql = sql & "                               sr.VendorID, "
        sql = sql & "                               srd.* "
        sql = sql & "                        FROM   SalesReportDispute srd "
        sql = sql & "                               INNER JOIN SalesReport sr ON srd.SalesReportID = sr.SalesReportID "
        sql = sql & "                               LEFT OUTER JOIN SalesReportBuilderTotalAmount srt ON srd.SalesReportID = srt.SalesReportID AND "
        sql = sql & "                               srd.BuilderID = srt.BuilderID) AS d ON d.PeriodQuarter = r.PeriodQuarter AND "
        sql = sql & "                            d.PeriodYear = r.PeriodYear AND "
        sql = sql & "                            d.VendorID = t.VendorID AND "
        sql = sql & "                            d.BuilderID = r.BuilderID "
        sql = sql & "WHERE   r.PeriodQuarter = " & PeriodQuarter & " AND "
        sql = sql & "       r.PeriodYear = " & periodYear & " AND "
        sql = sql & "       t.VendorID = " & VendorID



        Return db.GetDataTable(sql)

    End Function

    Private Function MergeData(ByVal tblA As DataTable, ByVal tblB As DataTable, ByVal colsA() As String) As DataTable
        Dim mergedtbl As New DataTable
        Dim col As DataColumn
        Dim sColumnName As String
        For Each sColumnName In colsA
            col = tblA.Columns(sColumnName)
            mergedtbl.Columns.Add(New DataColumn(col.ColumnName, col.DataType))
        Next
        For Each row As DataRow In tblA.Rows
            mergedtbl.ImportRow(row)
        Next row

        For Each row As DataRow In tblB.Rows
            mergedtbl.ImportRow(row)
        Next row
        Return mergedtbl
    End Function

End Class
