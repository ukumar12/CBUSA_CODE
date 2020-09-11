Imports Components
Imports DataLayer

Public Class DiscrepancyReport

    'This script sends the following messages:
    'DiscrepancyReport: to Builder 

    'Schedule: Runs the next day after reporting deadline has passed.

    Public Shared Sub Run(ByVal DB As Database)
        Logger.Info("Running DiscrepancyReport...")

        Dim currentQtr As Integer = Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
        Dim lastQtr As Integer = IIf(currentQtr = 1, 4, currentQtr - 1)
        Dim lastYear As Integer = IIf(lastQtr = 4, Now.Year - 1, Now.Year)
        Dim lastQtrEnd As DateTime = (lastQtr * 3) & "/" & Date.DaysInMonth(lastYear, lastQtr * 3) & "/" & lastYear
        Dim deadline As DateTime = lastQtrEnd.AddMonths(1).AddDays(SysParam.GetValue(DB, "ReportDeadlineDays") + 1)
        Dim DeadlinePassed As Boolean = DateDiff("d", deadline.Date, Now.Date) > 0

        Dim dbTaskLog As TaskLogRow = Nothing
        Try
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "DiscrepancyReport"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1)
            dbTaskLog.Insert()

            If deadline.Date <> Now.Date Then
                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "DiscrepancyReport"
                dbTaskLog.Status = "Skipped"
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1)
                dbTaskLog.Insert()
                Exit Sub
            End If

            Dim sql As String = _
                  "Select distinct BuilderID from PurchasesReport " _
                & " Where PeriodQuarter=" & DB.Number(lastQtr) _
                & " And PeriodYear = " & DB.Number(lastYear)

            Dim dtBuilders As DataTable = DB.GetDataTable(sql)

            Dim cnt As Integer = 0
            For Each row As DataRow In dtBuilders.Rows
                Dim dtDiscrepancy As DataTable = PurchasesReportRow.GetDiscrepancyReport(DB, row("BuilderId"), lastQtr, lastYear)

                If dtDiscrepancy.Rows.Count > 0 Then
                    Try

                        'Send DiscrepancyReport: to Builder

                        Dim dbAutoMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "DiscrepencyReport")
                        Dim MsgBody As String = String.Empty

                        MsgBody = "Discrepency Report: " & dtDiscrepancy.Rows.Count & " discrepencies were found between your Purchases report and the Vendors' Sales reports. The deadline to dispute these discrepencies is " & deadline.AddDays(-1) & ". Click here to view the full report: " & vbCrLf & vbCrLf & System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/rebates/discrepancy-report.aspx"

                        dbAutoMsg.Send(BuilderRow.GetRow(DB, row("BuilderId")), MsgBody)

                        cnt += 1

                    Catch ex As Exception
                        Logger.Info("Error sending DiscrepancyReport email to builder: " & row("BuilderID"))
                    End Try
                End If
            Next
            Logger.Info(cnt & " DiscrepancyReport sent")
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "DiscrepancyReport"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1) & " (" & cnt & " DiscrepancyReport emails sent)"
            dbTaskLog.Insert()
        Catch ex As Exception
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "DiscrepancyReport"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        End Try

    End Sub
End Class
