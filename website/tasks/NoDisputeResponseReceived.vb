Imports Components
Imports DataLayer

Public Class NoDisputeResponseReceived

    'This script sends the following messages:
    'NoDisputeResponseReceived: to Builder 
    'NoDisputeResponseReceivedToAdmins

    'Schedule: run on the Discrepancy Response Deadline which is Discrepancy Report Deadline + set number of day in system parameters 

    Public Shared Sub Run(ByVal DB As Database)
        Logger.Info("Running NoDisputeResponseReceived...")
        Dim currentQtr As Integer = Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
        Dim lastQtr As Integer = IIf(currentQtr = 1, 4, currentQtr - 1)

        Dim lastYear As Integer = IIf(lastQtr = 4, Now.Year - 1, Now.Year)
        Dim lastQtrEnd As DateTime = (lastQtr * 3) & "/" & Date.DaysInMonth(lastYear, lastQtr * 3) & "/" & lastYear
        Dim deadline As DateTime = lastQtrEnd.AddMonths(1).AddDays(SysParam.GetValue(DB, "ReportDeadlineDays") + 1)
        'Discrepancy Report Deadline:
        deadline = deadline.AddDays(SysParam.GetValue(DB, "DiscrepancyDeadlineDays"))
        'Discrepancy Response Deadline:
        deadline = deadline.AddDays(SysParam.GetValue(DB, "DiscrepancyResponseDeadlineDays"))

        Dim dbTaskLog As TaskLogRow = Nothing
        Try
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "NoDisputeResponseReceived"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1)
            dbTaskLog.Insert()

            If deadline.Date <> Now.Date Then
                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "NoDisputeResponseReceived"
                dbTaskLog.Status = "Skipped"
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1)
                dbTaskLog.Insert()
                Exit Sub
            End If

            Dim sql As String = _
                  "SELECT sr.SalesReportID, sr.VendorID, srd.BuilderID, srd.BuilderTotalAmount, srd.VendorTotalAmount " _
                & "FROM SalesReport sr INNER JOIN SalesReportDispute srd ON sr.SalesReportID = srd.SalesReportID " _
                & " where sr.PeriodQuarter=" & DB.Number(lastQtr) _
                & " and sr.PeriodYear = " & DB.Number(lastYear) _
                & " and srd.DisputeResponseID IS NULL"

            Dim dtResponse As DataTable = DB.GetDataTable(sql)

            Dim cnt As Integer = 0
            For Each row As DataRow In dtResponse.Rows
                Try

                    'Send NoDisputeResponseReceived: to Builder

                    Dim dbAutoMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "NoDisputeResponseReceived")
                    Dim dbVendor As VendorRow = VendorRow.GetRow(DB, row("VendorID"))
                    Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, row("BuilderID"))

                    Dim MsgBody As String = String.Empty

                    MsgBody = dbVendor.CompanyName & " did not respond to your dispute." & vbCrLf & vbCrLf
                    MsgBody &= "Vendor Amount: " & FormatCurrency(row("VendorTotalAmount")) & vbCrLf
                    MsgBody &= "Builder Amount: " & FormatCurrency(row("BuilderTotalAmount")) & vbCrLf
                    MsgBody &= "Difference: " & FormatCurrency(row("VendorTotalAmount") - row("BuilderTotalAmount")) & vbCrLf & vbCrLf
                    MsgBody &= System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/rebates/discrepancy-report.aspx"

                    dbAutoMsg.Send(dbBuilder, MsgBody)

                    'Send DisputeSubmittedToAdmins.

                    dbAutoMsg = AutomaticMessagesRow.GetRowByTitle(DB, "NoDisputeResponseReceivedToAdmins")

                    MsgBody = dbVendor.CompanyName & " did not respond to " & dbBuilder.CompanyName & "'s dispute." & vbCrLf & vbCrLf
                    MsgBody &= "Vendor Amount: " & FormatCurrency(row("VendorTotalAmount")) & vbCrLf
                    MsgBody &= "Builder Amount: " & FormatCurrency(row("BuilderTotalAmount")) & vbCrLf
                    MsgBody &= "Difference: " & FormatCurrency(row("VendorTotalAmount") - row("BuilderTotalAmount")) & vbCrLf & vbCrLf
                    MsgBody &= System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/admin/"

                    dbAutoMsg.SendAdmin(MsgBody)

                    cnt += 1

                Catch ex As Exception
                    Logger.Info("Error sending NoDisputeResponseReceived email to Builder: " & row("BuilderID"))
                End Try

            Next
            Logger.Info(cnt & " NoDisputeResponseReceived sent")

            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "NoDisputeResponseReceived"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1) & " (" & cnt & " NoDisputeResponseReceived emails sent)"
            dbTaskLog.Insert()

        Catch ex As Exception
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "NoDisputeResponseReceived"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        End Try
    End Sub
End Class
