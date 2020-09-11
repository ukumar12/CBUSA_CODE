Imports System.Web.Services
Imports System.Data.SqlClient
Imports Components
Imports System.Configuration.ConfigurationManager
Imports System.Collections.Generic
Imports Utility

Imports DataLayer
Imports IDevSearch
Imports System.Web

Imports System.Web.Services.Protocols
Imports System.Diagnostics

Partial Class builder_StatementService
    Inherits SitePage

    <WebMethod()> _
    Public Shared Function GetRebateLineDetails(ByVal BuilderId As String, ByVal VendorId As String, ByVal VendorName As String, ByVal DisplayReportingYearQtr As String, ByVal DateFrom As Date, ByVal DateTo As Date, ByVal OrigDocNumber As String) As String
        'return dosage;            
        Dim sb As New StringBuilder()
        Dim dtRebateLineDetails As DataTable = Nothing
        Dim CacheKeySuffix As String = "_"
        Dim CacheKey As String = "GetRebateLineDetails_" & BuilderId & CacheKeySuffix & OrigDocNumber
        If Not System.Web.HttpContext.Current Is Nothing Then
            If Not TypeOf (System.Web.HttpContext.Current.Cache(CacheKey)) Is DataTable Then
                System.Web.HttpContext.Current.Cache.Remove(CacheKey)
            End If
            dtRebateLineDetails = System.Web.HttpContext.Current.Cache(CacheKey)
        End If

        If dtRebateLineDetails Is Nothing Then
            dtRebateLineDetails = GetBuilderStmtRebateDetail("RG_BuilderStmtRebateDetail", BuilderId, OrigDocNumber)
        End If


        
        Dim FilteredRebateLineDetails As DataTable = dtRebateLineDetails.DefaultView.ToTable()
        Dim watch As Stopwatch = Stopwatch.StartNew()
        watch.Start()
        Dim dtapv As DataTable = GlobalDB.DB.GetDataTable("Select InitialVendorAmount,InitialBuilderAmount,BuilderAmountInDispute,VendorAmountInDispute,FinalAmount,ResolutionAmount,DisputeResponse,DisputeResponseReason from [vAPVWithDisputes] where PeriodQuarter = " & GlobalDB.DB.Number(FilteredRebateLineDetails.Rows(0).Item("ReportingQtr")) & " AND PeriodYear = " & GlobalDB.DB.Number(FilteredRebateLineDetails.Rows(0).Item("ReportingYear")) & " AND HistoricVendorID = " & GlobalDB.DB.NQuote(FilteredRebateLineDetails.Rows(0).Item("VendorID").ToString.Trim) & " AND HistoricBuilderid = " & GlobalDB.DB.Number(BuilderId))
        Dim RebateRate As String
        Dim OrigRebateAmount As String
        Dim OrigInvoiceDate As String
        Dim Adjustments As String
        Dim rebateTotal As String
        Dim rebateTotalD As Decimal
        Dim TotalPaymentsRecd As String
        Dim BuilderreportedAmount As String = " "
        Dim BuilderreportedAmountD As Decimal = 0
        Dim VendorreportedAmount As String = String.Empty
        Dim VendorreportedAmountD As Decimal = 0
        Dim DiscrepancyAmount As String = ""
        Dim DiscrepancyCode As String = String.Empty
        Dim DiscrepancyResponse As String = String.Empty
        Dim qpv As String = ""

        If Not String.IsNullOrEmpty(FilteredRebateLineDetails.Rows(0).Item("RebateRate")) Then
            RebateRate = FormatPercent(FilteredRebateLineDetails.Rows(0).Item("RebateRate"), 2)
        Else
            RebateRate = "-"
        End If
        If Not String.IsNullOrEmpty(FilteredRebateLineDetails.Rows(0).Item("OrigRebateAmount")) Then
            OrigRebateAmount = FormatCurrency(FilteredRebateLineDetails.Rows(0).Item("OrigRebateAmount"), 2, , TriState.True)
        Else
            OrigRebateAmount = "-"
        End If
        If Not String.IsNullOrEmpty(FilteredRebateLineDetails.Rows(0).Item("OrigDocDate")) Then
            OrigInvoiceDate = FormatDateTime(FilteredRebateLineDetails.Rows(0).Item("OrigDocDate"), DateFormat.ShortDate)
        Else
            OrigInvoiceDate = "-"
        End If
        If Not String.IsNullOrEmpty(FilteredRebateLineDetails.Rows(0).Item("RebateAdjAmount")) Then
            Adjustments = FormatCurrency(FilteredRebateLineDetails.Rows(0).Item("RebateAdjAmount"), 2, , TriState.True)
        Else
            Adjustments = "-"
        End If

        rebateTotalD = FilteredRebateLineDetails.Rows(0).Item("OrigRebateAmount") + FilteredRebateLineDetails.Rows(0).Item("RebateAdjAmount")
        rebateTotal = FormatCurrency(rebateTotalD, 2, , TriState.True)

        If Not String.IsNullOrEmpty(FilteredRebateLineDetails.Rows(0).Item("RebatePmt")) Then
            TotalPaymentsRecd = FormatCurrency(FilteredRebateLineDetails.Rows(0).Item("RebatePmt"), 2, , TriState.True)
        Else
            TotalPaymentsRecd = "-"
        End If

        If dtapv.Rows.Count > 0 Then
            If Not IsDBNull(dtapv.Rows(0).Item("InitialBuilderAmount")) Then
                BuilderreportedAmount = FormatCurrency(dtapv.Rows(0).Item("InitialBuilderAmount"), 2, , TriState.True)
                BuilderreportedAmountD = dtapv.Rows(0).Item("InitialBuilderAmount")
            Else
                BuilderreportedAmount = "-"
                BuilderreportedAmountD = 0.0
            End If

            If Not IsDBNull(dtapv.Rows(0).Item("InitialVendorAmount")) Then
                VendorreportedAmount = FormatCurrency(dtapv.Rows(0).Item("InitialVendorAmount"), 2, , TriState.True)
                VendorreportedAmountD = dtapv.Rows(0).Item("InitialVendorAmount")
            Else
                VendorreportedAmount = "-"
                VendorreportedAmountD = 0.0
            End If

            DiscrepancyAmount = FormatCurrency((BuilderreportedAmountD - VendorreportedAmountD).ToString, 2, TriState.True, TriState.True)


            If Not IsDBNull(dtapv.Rows(0).Item("FinalAmount")) Then
                qpv = FormatCurrency(dtapv.Rows(0).Item("FinalAmount"), 2, TriState.True, TriState.True)
            Else
                qpv = "-"
            End If

            If Not IsDBNull(dtapv.Rows(0).Item("DisputeResponseReason")) Then
                DiscrepancyResponse = dtapv.Rows(0).Item("DisputeResponseReason")
            Else
                DiscrepancyResponse = "-"
            End If

            If Not IsDBNull(dtapv.Rows(0).Item("DisputeResponse")) Then
                DiscrepancyCode = dtapv.Rows(0).Item("DisputeResponse")
            Else
                DiscrepancyCode = "-"
            End If
        End If

        watch.Stop()


        sb.AppendLine("")
        sb.AppendLine("				<div class=""rebateStatementClose"">close</div>")
        sb.AppendLine("				<div><strong>Line Item Detail Report:</strong> " & VendorName & " (" & DisplayReportingYearQtr & ")" & "</div>")
        sb.AppendLine("				<div class=""clear""></div>")
        sb.AppendLine("				<div class=""rebateDetCol"" style=""float: left;"">			")
        sb.AppendLine("					<table cellpadding=""0"" cellspacing=""0"">")
        sb.AppendLine("						<tr>")
        sb.AppendLine("							<td>Builder reported amount</td>")
        sb.AppendLine("							<td>" & BuilderreportedAmount & "</td>")
        sb.AppendLine("						</tr>")
        sb.AppendLine("						<tr>")
        sb.AppendLine("							<td>Vendor reported amount</td>")
        sb.AppendLine("							<td>" & VendorreportedAmount & "</td>")
        sb.AppendLine("						</tr>")
        sb.AppendLine("						<tr>")
        sb.AppendLine("							<td>Discrepancy amount</td>")
        sb.AppendLine("							<td>" & DiscrepancyAmount & "</td>")
        sb.AppendLine("						</tr>")
        sb.AppendLine("						<tr>")
        sb.AppendLine("							<td>&nbsp;</td>")
        sb.AppendLine("							<td>&nbsp;</td>")
        sb.AppendLine("						</tr>							")
        sb.AppendLine("						<tr>")
        sb.AppendLine("							<td>Discrepancy code </td>")
        sb.AppendLine("							<td>" & DiscrepancyCode & "</td>")
        sb.AppendLine("						</tr>")
        sb.AppendLine("						<tr>")
        sb.AppendLine("							<td>Dispute response</td>")
        sb.AppendLine("							<td>" & DiscrepancyResponse & "</td>")
        sb.AppendLine("						</tr>	")
        sb.AppendLine("						<tr>")
        sb.AppendLine("							<td>&nbsp;</td>")
        sb.AppendLine("							<td>&nbsp;</td>")
        sb.AppendLine("						</tr>							")
        sb.AppendLine("						<tr>")
        sb.AppendLine("							<td>Qualified Purchase Volume</td>")
        sb.AppendLine("							<td>" & qpv & "</td>")
        sb.AppendLine("						</tr>")
        sb.AppendLine("					</table>")
        sb.AppendLine("				</div>")
        sb.AppendLine("				<div class=""rebateDetCol""  style=""float: right;"">			")
        sb.AppendLine("					<table cellpadding=""0"" cellspacing=""0"">")
        sb.AppendLine("						<tr>")
        sb.AppendLine("							<td>Rebate rate</td>")
        sb.AppendLine("							<td>" & RebateRate & "</td>")
        sb.AppendLine("						</tr>")
        sb.AppendLine("						<tr>")
        sb.AppendLine("							<td>Original rebate amount</td>")
        sb.AppendLine("							<td>" & OrigRebateAmount & "</td>")
        sb.AppendLine("						</tr>")
        sb.AppendLine("						<tr>")
        sb.AppendLine("							<td>Original invoice date</td>")
        sb.AppendLine("							<td>" & OrigInvoiceDate & "</td>")
        sb.AppendLine("						</tr>					")
        sb.AppendLine("						<tr>")
        sb.AppendLine("							<td>&nbsp;</td>")
        sb.AppendLine("							<td>&nbsp;</td>")
        sb.AppendLine("						</tr>")
        sb.AppendLine("						<tr>")
        sb.AppendLine("							<td>Adjustments</td>")
        sb.AppendLine("							<td>" & Adjustments & "</td>")
        sb.AppendLine("						</tr>")
        sb.AppendLine("						<tr>")
        sb.AppendLine("							<td>Rebate total</td>")
        sb.AppendLine("							<td>" & rebateTotal & "</td>")
        sb.AppendLine("						</tr>					")
        sb.AppendLine("						<tr>")
        sb.AppendLine("							<td>&nbsp;</td>")
        sb.AppendLine("							<td>&nbsp;</td> ")
        sb.AppendLine("						</tr>")
        sb.AppendLine("						<tr>")
        sb.AppendLine("							<td>Total payments received</td>")
        sb.AppendLine("							<td>" & TotalPaymentsRecd & "</td>")
        sb.AppendLine("						</tr>						")
        sb.AppendLine("					</table>")
        sb.AppendLine("					<div class=""clear""></div>")
        sb.AppendLine("				</div>	")
        sb.AppendLine("				<div class=""clear""></div>				")
        sb.AppendLine("		 ")




        Return sb.ToString
    End Function

    Private Shared Function GetBuilderStmtRebateDetail(ByVal StoredProcedureName As String, ByVal BuilderID As Integer, ByVal OrigDocNumber As String) As DataTable
        Dim ResDb As New Database
        Dim dt As New DataTable
        Dim prams(1) As SqlParameter
        prams(0) = New SqlParameter("@ORIGDOCNUMBER", OrigDocNumber.Trim)
        prams(1) = New SqlParameter("@BUILDERID", BuilderID)

        Try
            ResDb.Open(DBConnectionString.GetConnectionString(AppSettings("ResgroupConnectionString"), AppSettings("ResgroupConnectionStringUsername"), AppSettings("ResgroupConnectionStringPassword")))
            ResDb.RunProc(StoredProcedureName, prams, dt)
            Dim CacheKeySuffix As String = "_"
            Dim CacheKey As String = "GetRebateLineDetails_" & BuilderID & CacheKeySuffix & OrigDocNumber
            System.Web.HttpContext.Current.Cache.Insert(CacheKey, dt, Nothing, Date.UtcNow.AddSeconds(600), TimeSpan.Zero)
            Return dt
        Catch ex As Exception
        Finally
            ResDb.Close()
        End Try
        Return Nothing
    End Function

End Class
