Imports Components
Imports DataLayer
Public Class NoPurchasesReportReceived

    'This script sends the following messages:
    'NoPurchasesReportReceived: to Builder
    'NoPurchasesReportReceivedToAdmins

    'Schedule: run one day after reporting deadline.

    Public Shared Sub Run(ByVal DB As Database)
        Logger.Info("Running NoPurchasesReportReceived...")
        Dim currentQtr As Integer = Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
        Dim lastQtr As Integer = IIf(currentQtr = 1, 4, currentQtr - 1)
        Dim lastYear As Integer = IIf(lastQtr = 4, Now.Year - 1, Now.Year)
        Dim lastQtrEnd As DateTime = (lastQtr * 3) & "/" & Date.DaysInMonth(lastYear, lastQtr * 3) & "/" & lastYear
        Dim deadline As DateTime = lastQtrEnd.AddMonths(1).AddDays(SysParam.GetValue(DB, "ReportDeadlineDays") + 1)
        Dim DeadlinePassed As Boolean = DateDiff("d", deadline.Date, Now.Date) > 0

        Dim dbTaskLog As TaskLogRow = Nothing
        Try
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "NoPurchasesReportReceived"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = "Deadline: " & deadline
            dbTaskLog.Insert()

            If deadline.Date <> Now.Date Then
                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "NoPurchasesReportReceived"
                dbTaskLog.Status = "Skipped"
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1)
                dbTaskLog.Insert()
                Exit Sub
            End If

            Dim sql As String = _
                  "Select Distinct BuilderId From Builder Where IsActive = 1 And BuilderId Not In (Select BuilderID from PurchasesReport " _
                & " Where PeriodQuarter=" & DB.Number(lastQtr) _
                & " And PeriodYear = " & DB.Number(lastYear) & ")"

            Dim dtBuilders As DataTable = DB.GetDataTable(sql)

            Dim cnt As Integer = 0
            For Each row As DataRow In dtBuilders.Rows
                Try

                    'Send NoPurchasesReportReceived: to Builder

                    Dim dbAutoMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "NoPurchasesReportReceived")
                    Dim MsgBody As String = String.Empty
                    Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, row("BuilderId"))

                    MsgBody = "We did not receive your " & lastYear & " Q" & lastQtr & " report. Please review the discrepancy report on your dashboard to ensure the accuracy of the vendor's reports:" & vbCrLf & vbCrLf & System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/rebates/discrepancy-report.aspx"

                    dbAutoMsg.Send(dbBuilder, MsgBody)

                    'Send NoPurchasesReportReceivedToAdmin

                    dbAutoMsg = AutomaticMessagesRow.GetRowByTitle(DB, "NoPurchasesReportReceivedToAdmins")

                    MsgBody = dbBuilder.CompanyName & " did not submit a " & lastYear & " Q" & lastQtr & " report." & vbCrLf & vbCrLf & System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/admin/"

                    dbAutoMsg.SendAdmin(MsgBody)

                    cnt += 1

                Catch ex As Exception
                    Logger.Info("Error sending NoPurchasesReportReceived email to builder: " & row("BuilderID"))
                End Try
            Next

            Logger.Info(cnt & " NoPurchasesReportReceived sent")

            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "NoPurchasesReportReceived"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1) & " (" & cnt & " NoPurchasesReportReceived emails sent)"
            dbTaskLog.Insert()

        Catch ex As Exception
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "NoPurchasesReportReceived"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        End Try
    End Sub
End Class
