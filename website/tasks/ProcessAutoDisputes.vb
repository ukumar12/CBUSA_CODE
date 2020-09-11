Imports Components
Imports DataLayer

Public Class ProcessAutoDisputes

    'This script creates automatic disputes for vondors who haven't reported for builder that have reported for them.

    'Schedule: runs on the reporting deadline date

    Public Shared Sub Run(ByVal DB As Database)
        Logger.Info("Running ProcessAutoDisputes...")
        Dim currentQtr As Integer = Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
        Dim lastQtr As Integer = IIf(currentQtr = 1, 4, currentQtr - 1)
        Dim lastYear As Integer = IIf(lastQtr = 4, Now.Year - 1, Now.Year)
        Dim lastQtrEnd As DateTime = (lastQtr * 3) & "/" & Date.DaysInMonth(lastYear, lastQtr * 3) & "/" & lastYear
        Dim deadline As DateTime = lastQtrEnd.AddMonths(1).AddDays(SysParam.GetValue(DB, "ReportDeadlineDays") + 1)

        Dim dbTaskLog As TaskLogRow = Nothing
        Try
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "ProcessAutoDisputes"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1)
            dbTaskLog.Insert()

            If deadline.Date <> Now.Date Then
                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "ProcessAutoDisputes"
                dbTaskLog.Status = "Skipped"
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1)
                dbTaskLog.Insert()
                Exit Sub
            End If

            'All Vendors that haven not reported
            Dim sql As String = _
                  "select bb.CompanyName, a.*, p.* from PurchasesReportVendorTotalAmount a " _
                    & " inner join PurchasesReport p on a.PurchasesReportID=p.PurchasesReportID" _
                    & " inner join builder bb on p.builderid = bb.builderid" _
                    & " where p.PeriodQuarter= " & DB.Number(lastQtr) & " and p.PeriodYear = " & DB.Number(lastYear) & " and p.BuilderId Not In (" _
                    & " Select b.BuilderId From SalesReportBuilderTotalAmount b " _
                    & " inner join SalesReport s on b.SalesReportId = s.SalesReportId " _
                    & " Where s.PeriodQuarter= " & DB.Number(lastQtr) & " and s.PeriodYear = " & DB.Number(lastYear) & " and s.VendorID= a.VendorId)" _
                    & " and p.BuilderId Not In (Select d.BuilderId From SalesReportDispute d inner join SalesReport sr On d.SalesReportId = sr.SalesReportId " _
                    & " Where sr.PeriodQuarter= " & DB.Number(lastQtr) & " and sr.PeriodYear = " & DB.Number(lastYear) & " and sr.VendorID= a.VendorId) order by VendorId"

            Dim dtVendors As DataTable = DB.GetDataTable(sql)

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
                    End If

                    'Create dispute for vendors that haven't reported
                    sql = "Select * From SalesReportDispute d INNER JOIN SalesReport r ON d.SalesReportID = r.SalesReportID WHERE r.PeriodQuarter=" & DB.Number(lastQtr) & " and r.PeriodYear=" & DB.Number(lastYear) & " And VendorId = " & DB.Number(row("VendorID")) & " And BuilderId = " & DB.Number(row("BuilderID"))
                    Dim dtDispute As DataTable = DB.GetDataTable(sql)
                    If dtDispute.Rows.Count = 0 Then
                        Dim dbDispute As New SalesReportDisputeRow(DB)
                        dbDispute.BuilderID = row("BuilderID")
                        dbDispute.BuilderTotalAmount = row("TotalAmount")
                        dbDispute.SalesReportID = dbSalesReport.SalesReportID
                        dbDispute.VendorTotalAmount = 0
                        dbDispute.Insert()
                        cnt += 1
                    End If
                    
                    DB.CommitTransaction()
                Catch ex As Exception
                    If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                    Logger.Error(Logger.GetErrorMessage(ex))
                End Try
            Next
            Logger.Info(cnt & " disputes created")
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "ProcessAutoDisputes"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1) & " (" & cnt & " auto disputes added)"
            dbTaskLog.Insert()

        Catch ex As Exception
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "ProcessAutoDisputes"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        End Try
    End Sub
End Class
