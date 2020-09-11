Imports Components
Imports DataLayer

Public Class Reporting

    'This script creates automatic disputes for vondors who haven't reported at all.

    'Schedule: runs on the reporting deadline date

    Public Shared Sub Run(ByVal DB As Database)
        Logger.Info("Running Reporting...")
        Dim currentQtr As Integer = Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
        Dim lastQtr As Integer = IIf(currentQtr = 1, 4, currentQtr - 1)
        Dim lastYear As Integer = IIf(lastQtr = 4, Now.Year - 1, Now.Year)
        Dim lastQtrEnd As DateTime = (lastQtr * 3) & "/" & Date.DaysInMonth(lastYear, lastQtr * 3) & "/" & lastYear
        Dim deadline As DateTime = lastQtrEnd.AddMonths(1).AddDays(SysParam.GetValue(DB, "ReportDeadlineDays") + 1)
        Dim IsActiveAutoDispute As Boolean = SysParam.GetValue(DB, "ActivateAutoDisputesTask")
        Dim AdminEmail As String = SysParam.GetValue(DB, "AdminEmail")
        Dim dbTaskLog As TaskLogRow = Nothing
        Try
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "Reporting"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1)
            dbTaskLog.Insert()

            If Not IsActiveAutoDispute Then
                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "Reporting"
                dbTaskLog.Status = "Skipped"
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "Script turned off. Deadline: " & deadline.AddDays(-1)
                dbTaskLog.Insert()
                Exit Sub
            ElseIf deadline.Date > Now.Date Then
                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "Reporting"
                dbTaskLog.Status = "Skipped"
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "Script activated before deadline. Deadline: " & deadline.AddDays(-1)
                dbTaskLog.Insert()
                Core.SendSimpleMail(AdminEmail, AdminEmail, AdminEmail, AdminEmail, "CBUSA - Auto Dispute Task Error", dbTaskLog.Msg)
                Exit Sub
            ElseIf DB.GetDataTable("Select * From TaskLog Where DateDiff(d,GetDate(),LogDate) >= 0 And TaskName = 'Reporting' And Status = 'Completed'").Rows.Count > 0 Then
                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "Reporting"
                dbTaskLog.Status = "Skipped"
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "Script attempted to run more than once during current reporting period. Deadline: " & deadline.AddDays(-1)
                dbTaskLog.Insert()
                Core.SendSimpleMail(AdminEmail, AdminEmail, AdminEmail, AdminEmail, "CBUSA - Auto Dispute Task Error", dbTaskLog.Msg)
                Exit Sub
            End If

            'All Vendors that haven not reported
            Dim sql As String = _
                  "select distinct VendorID from PurchasesReportVendorTotalAmount a inner join PurchasesReport p on a.PurchasesReportID=p.PurchasesReportID " _
                & " where p.PeriodQuarter=" & DB.Number(lastQtr) _
                & " and p.PeriodYear = " & DB.Number(lastYear) _
                & " and a.VendorID not in (select sr.VendorID from SalesReport sr where CreatorVendorAccountID <> -1 and PeriodQuarter=" & DB.Number(lastQtr) & " and PeriodYear=" & DB.Number(lastYear) & " and exists (select * from SalesReportBuilderTotalAmount where SalesReportID=sr.SalesReportID))"

            Dim dtVendors As DataTable = DB.GetDataTable(sql)

            'All purchase report rows where builder reported for vendor that hasn't reported. 
            sql = "select a.*, p.BuilderID from PurchasesReportVendorTotalAmount a inner join PurchasesReport p on a.PurchasesReportID=p.PurchasesReportID " _
                & " where p.PeriodQuarter=" & DB.Number(lastQtr) _
                & " and p.PeriodYear = " & DB.Number(lastYear) _
                & " and a.VendorID not in (select sr.VendorID from SalesReport sr where CreatorVendorAccountID <> -1 and PeriodQuarter=" & DB.Number(lastQtr) & " and PeriodYear=" & DB.Number(lastYear) & " and exists (select * from SalesReportBuilderTotalAmount where SalesReportID=sr.SalesReportID))"

            Dim dtReports As DataTable = DB.GetDataTable(sql)
            Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "NoSalesReportReceived")
            Dim cnt As Integer = 0

            'Exit Sub

            For Each row As DataRow In dtVendors.Rows
                Try
                    DB.BeginTransaction()
                    'Create sales report for vendors that don't have one.
                    Dim dbSalesReport As SalesReportRow = SalesReportRow.GetSalesReportByPeriod(DB, row("VendorID"), lastYear, lastQtr)
                    If dbSalesReport.Created = Nothing Then
                        dbSalesReport.CreatorVendorAccountID = -1
                        dbSalesReport.PeriodQuarter = lastQtr
                        dbSalesReport.PeriodYear = lastYear
                        dbSalesReport.VendorID = row("VendorID")
                        dbSalesReport.Insert()
                        Try
                            dbMsg.Send(VendorRow.GetRow(DB, row("VendorID")))
                        Catch ex As Exception
                            Logger.Info("Error sending email to vendor:" & row("VendorID"))
                        End Try
                    End If

                    'Create dispute for vendors that haven't reported
                    Dim reports As DataRow() = dtReports.Select("VendorID=" & DB.Number(row("VendorID")))
                    For Each reportRow As DataRow In reports
                        sql = "Select * From SalesReportDispute d INNER JOIN SalesReport r ON d.SalesReportID = r.SalesReportID WHERE r.PeriodQuarter=" & DB.Number(lastQtr) & " and r.PeriodYear=" & DB.Number(lastYear) & " And VendorId = " & DB.Number(row("VendorID")) & " And BuilderId = " & DB.Number(reportRow("BuilderID"))
                        Dim dtDispute As DataTable = DB.GetDataTable(sql)
                        If dtDispute.Rows.Count = 0 Then
                            Dim dbDispute As New SalesReportDisputeRow(DB)
                            dbDispute.BuilderID = reportRow("BuilderID")
                            dbDispute.BuilderTotalAmount = reportRow("TotalAmount")
                            dbDispute.SalesReportID = dbSalesReport.SalesReportID
                            dbDispute.VendorTotalAmount = 0
                            dbDispute.Insert()
                            cnt += 1
                        End If
                    Next

                    DB.CommitTransaction()
                Catch ex As Exception
                    If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                    Logger.Error(Logger.GetErrorMessage(ex))
                End Try
            Next
            Logger.Info(cnt & " disputes created")
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "Reporting"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1) & " (" & cnt & " auto disputes added)"
            dbTaskLog.Insert()

            DB.ExecuteSQL("Update SysParam Set Value = 0 Where Name = 'ActivateAutoDisputesTask'")

        Catch ex As Exception
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "Reporting"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        End Try
    End Sub
End Class
