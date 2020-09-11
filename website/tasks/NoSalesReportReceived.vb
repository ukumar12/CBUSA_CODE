Imports Components
Imports DataLayer

Public Class NoSalesReportReceived

    'This script sends the following messages:
    'NoSalesReportReceived: to Vendor and QuarterlyReporter 
    'NoSalesReportReceivedToAdmin

    'Schedule: run one day after reporting deadline.

    Public Shared Sub Run(ByVal DB As Database)
        Logger.Info("Running NoSalesReportReceived...")
        Dim currentQtr As Integer = Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
        Dim lastQtr As Integer = IIf(currentQtr = 1, 4, currentQtr - 1)

        Dim lastYear As Integer = IIf(lastQtr = 4, Now.Year - 1, Now.Year)
        Dim lastQtrEnd As DateTime = (lastQtr * 3) & "/" & Date.DaysInMonth(lastYear, lastQtr * 3) & "/" & lastYear
        Dim deadline As DateTime = lastQtrEnd.AddMonths(1).AddDays(SysParam.GetValue(DB, "ReportDeadlineDays") + 1)


        Dim dbTaskLog As TaskLogRow = Nothing
        Try
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "NoSalesReportReceived"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1)
            dbTaskLog.Insert()

            If deadline.Date <> Now.Date Then
                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "NoSalesReportReceived"
                dbTaskLog.Status = "Skipped"
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1)
                dbTaskLog.Insert()
                Exit Sub
            End If

            Dim sql As String = _
                  "select distinct VendorID from PurchasesReportVendorTotalAmount a inner join PurchasesReport p on a.PurchasesReportID=p.PurchasesReportID " _
                & " where p.PeriodQuarter=" & DB.Number(lastQtr) _
                & " and p.PeriodYear = " & DB.Number(lastYear) _
                & " and a.VendorID not in (select sr.VendorID from SalesReport sr where CreatorVendorAccountID <> -1 and PeriodQuarter=" & DB.Number(lastQtr) & " and PeriodYear=" & DB.Number(lastYear) & " and exists (select * from SalesReportBuilderTotalAmount where SalesReportID=sr.SalesReportID))"

            Dim dtVendors As DataTable = DB.GetDataTable(sql)

            sql = "select a.*, p.BuilderID from PurchasesReportVendorTotalAmount a inner join PurchasesReport p on a.PurchasesReportID=p.PurchasesReportID " _
                & " where p.PeriodQuarter=" & DB.Number(lastQtr) _
                & " and p.PeriodYear = " & DB.Number(lastYear) _
                & " and a.VendorID not in (select sr.VendorID from SalesReport sr where CreatorVendorAccountID <> -1 and PeriodQuarter=" & DB.Number(lastQtr) & " and PeriodYear=" & DB.Number(lastYear) & " and exists (select * from SalesReportBuilderTotalAmount where SalesReportID=sr.SalesReportID))"

            Dim dtReports As DataTable = DB.GetDataTable(sql)
            Dim cnt As Integer = 0
            For Each row As DataRow In dtVendors.Rows
                Dim dbSalesReport As SalesReportRow = SalesReportRow.GetSalesReportByPeriod(DB, row("VendorID"), lastYear, lastQtr)
                If dbSalesReport.Created = Nothing Then
                    dbSalesReport.CreatorVendorAccountID = -1
                    dbSalesReport.PeriodQuarter = lastQtr
                    dbSalesReport.PeriodYear = lastYear
                    dbSalesReport.VendorID = row("VendorID")
                    dbSalesReport.Insert()
                End If

                Try

                    'Send NoSalesReportReceived: to Vendor and QuarterlyReporter

                    Dim dbAutoMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "NoSalesReportReceived")
                    Dim dbVendor As VendorRow = VendorRow.GetRow(DB, row("VendorID"))
                    Dim QuarterlyReporter As String = String.Empty
                    Dim dt As DataTable = DB.GetDataTable("Select va.Email From VendorAccount va, VendorAccountVendorRole vr Where va.VendorAccountId = vr.VendorAccountId And vr.VendorRoleId = 2 And va.VendorId = " & DB.Number(row("VendorID")))
                    If dt.Rows.Count > 0 Then
                        For Each dr As DataRow In dt.Rows
                            If Not IsDBNull(dr("Email")) Then
                                QuarterlyReporter = dr("Email")
                            End If
                        Next
                    End If

                    If QuarterlyReporter <> String.Empty AndAlso QuarterlyReporter <> dbVendor.Email Then
                        If dbAutoMsg.CCList <> String.Empty Then
                            dbAutoMsg.CCList &= "," & QuarterlyReporter
                        Else
                            dbAutoMsg.CCList &= QuarterlyReporter
                        End If
                    End If

                    Dim MsgBody As String = String.Empty

                    MsgBody = "We did not receive your " & lastYear & " Q" & lastQtr & " report. Please review the discrepancy report on your dashboard to ensure your customers receive credit for their purchases: " & vbCrLf & vbCrLf & System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/rebates/discrepancy-response.aspx"

                    dbAutoMsg.Send(dbVendor, MsgBody)

                    'Send DisputeSubmittedToAdmin.

                    dbAutoMsg = AutomaticMessagesRow.GetRowByTitle(DB, "NoSalesReportReceivedToAdmins")

                    MsgBody = dbVendor.CompanyName & " did not submit a " & lastYear & " Q" & lastQtr & " report." & vbCrLf & vbCrLf & System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/admin/"

                    dbAutoMsg.SendAdmin(MsgBody)

                    cnt += 1

                Catch ex As Exception
                    Logger.Info("Error sending NoSalesReportReceived email to vendor: " & row("VendorID"))
                End Try

                'This was causing duplicate dispute entries. Uncomment when reviwed and corrected.

                'Dim reports As DataRow() = dtReports.Select("VendorID=" & DB.Number(row("VendorID")))
                'For Each reportRow As DataRow In reports
                '    Dim dbDispute As New SalesReportDisputeRow(DB)
                '    dbDispute.BuilderID = reportRow("BuilderID")
                '    dbDispute.BuilderTotalAmount = reportRow("TotalAmount")
                '    dbDispute.SalesReportID = dbSalesReport.SalesReportID
                '    dbDispute.VendorTotalAmount = 0
                '    dbDispute.Insert()
                'Next
            Next
            Logger.Info(cnt & " NoSalesReportReceived sent")

            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "NoSalesReportReceived"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1) & " (" & cnt & " NoSalesReportReceived emails sent)"
            dbTaskLog.Insert()

        Catch ex As Exception
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "NoSalesReportReceived"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        End Try
    End Sub
End Class
