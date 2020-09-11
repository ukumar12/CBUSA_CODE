Imports Components
Imports DataLayer

Public Class ReportingReminders


    'This script sends the following messages:
    'PurchasesReportReminder: to Builder
    'PurchasesReportReminder2: to Builder
    'PurchasesReportDue: to Builder
    'SalesReportReminder: to Vendor
    'SalesReportReminder2: to Vendor
    'SalesReportDue: to Vendor

    'Schedule: runs every 15 and 30 days after the end of quarter and 1 day before reporting deadline.

    Public Shared Sub Run(ByVal DB As Database)
        Logger.Info("Running ReportingReminders task...")
        Dim SendAdminEmails As Boolean = True
        Dim qtrCurrent As Integer = Math.Ceiling(Now.Month / 3)
        Dim qtrLast As Integer = IIf(qtrCurrent = 1, 4, qtrCurrent - 1)
        Dim yrLast As Integer = IIf(qtrLast = 4, Now.Year - 1, Now.Year)

        Dim qtrEnd As DateTime = New Date(yrLast, qtrLast * 3, Date.DaysInMonth(yrLast, qtrLast * 3))
        Dim deadline As DateTime = qtrEnd.AddMonths(1).AddDays(SysParam.GetValue(DB, "ReportDeadlineDays") + 1)
        Dim reminder1 As DateTime = DateAdd(DateInterval.Day, 15, qtrEnd)
        Dim reminder2 As DateTime = DateAdd(DateInterval.Day, 30, qtrEnd)
        Dim reminder3 As DateTime = DateAdd(DateInterval.Day, -1, deadline)

        Logger.Info("Reminder 2 date = " & reminder2)

        Dim dbTaskLog As TaskLogRow = Nothing
        Try
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "ReportingReminders"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()

            If Now.Date <> reminder1.Date And Now.Date <> reminder2.Date And Now.Date <> reminder3.Date Then
                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "ReportingReminders"
                dbTaskLog.Status = "Skipped"
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = ""
                dbTaskLog.Insert()
                Exit Sub
            End If

            Logger.Info("Sending Reminders")

            If Now.Date = reminder1.Date Then

                'AR 080409: Only ACTIVE users that HAVE NOT reported should get reminders
                Dim dtBuilders As DataTable = DB.GetDataTable("select BuilderId from Builder where IsActive = 1 AND Submitted < " & DB.Quote(qtrEnd) & " And BuilderId not in (select BuilderId from PurchasesReport where PeriodQuarter=" & qtrLast & " and PeriodYear=" & yrLast & ")")
                Dim dtVendors As DataTable = DB.GetDataTable("select VendorId from Vendor where IsActive = 1 AND QuarterlyReportingOn = 1 AND Submitted < " & DB.Quote(qtrEnd) & " AND VendorId not in (select VendorId from SalesReport where PeriodQuarter=" & qtrLast & " and PeriodYear=" & yrLast & ")")

                Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "PurchasesReportReminder")
                Dim sMsg As String = "You may submit your Q" & qtrLast & " " & yrLast & " report. The due date is " & FormatDateTime(deadline.AddDays(-1), DateFormat.ShortDate) & ". http://app.custombuilders-usa.com/rebates/builder-purchases.aspx"
                Try

                    dbMsg.SendLLCNotificationListAReminder(LLCRow.GetLLCNotificationList(DB), "PurchasesReportReminder Sent" & vbCrLf & sMsg)
                Catch ex As Exception

                End Try
                For Each row As DataRow In dtBuilders.Rows
                    Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, row("BuilderId"))
                    Try
                        dbMsg.Send(dbBuilder, sMsg, CCLLCNotification:=False)
                    Catch ex As Exception
                        Continue For
                    End Try

                Next

                dbMsg = AutomaticMessagesRow.GetRowByTitle(DB, "SalesReportReminder")
                sMsg = "You may submit your Q" & qtrLast & " " & yrLast & " report. The due date is " & FormatDateTime(deadline.AddDays(-1), DateFormat.ShortDate) & ". http://app.custombuilders-usa.com/rebates/vendor-sales.aspx"

                Try
                    dbMsg.SendLLCNotificationListAReminder(LLCRow.GetLLCNotificationList(DB), "SalesReportReminder Sent" & vbCrLf & sMsg)
                Catch ex As Exception
                End Try

                For Each row As DataRow In dtVendors.Rows
                    Dim dbVendor As VendorRow = VendorRow.GetRow(DB, row("VendorId"))
                    Try
                        dbMsg.Send(dbVendor, sMsg, CCLLCNotification:=False)
                    Catch ex As Exception
                        Continue For
                    End Try

                Next
            ElseIf Now.Date = reminder2.Date Then
                'AR 080409: Only ACTIVE users that HAVE NOT reported should get reminders
                Dim dtBuilders As DataTable = DB.GetDataTable("select BuilderId from Builder where IsActive = 1 AND Submitted < " & DB.Quote(qtrEnd) & " And BuilderId not in (select BuilderId from PurchasesReport where PeriodQuarter=" & qtrLast & " and PeriodYear=" & yrLast & ")")
                Dim dtVendors As DataTable = DB.GetDataTable("select VendorId from Vendor where IsActive = 1 AND QuarterlyReportingOn = 1 AND Submitted < " & DB.Quote(qtrEnd) & " And VendorId not in (select VendorId from SalesReport where PeriodQuarter=" & qtrLast & " and PeriodYear=" & yrLast & ")")

                Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "PurchasesReportReminder2")
                Dim sMsg As String = "Q" & qtrLast & " " & yrLast & " transactions have ended. Please complete your purchases report. The due date is " & FormatDateTime(deadline.AddDays(-1), DateFormat.ShortDate) & ". http://app.custombuilders-usa.com/rebates/builder-purchases.aspx"

                Try
                    dbMsg.SendLLCNotificationListAReminder(LLCRow.GetLLCNotificationList(DB), "PurchasesReportReminder2 Sent" & vbCrLf & sMsg)
                Catch ex As Exception
                End Try

                For Each row As DataRow In dtBuilders.Rows
                    Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, row("BuilderId"))
                    Try
                        dbMsg.Send(dbBuilder, sMsg, CCLLCNotification:=False)
                    Catch ex As Exception
                        Continue For
                    End Try
                Next

                dbMsg = AutomaticMessagesRow.GetRowByTitle(DB, "SalesReportReminder2")
                sMsg = "Q" & qtrLast & " " & yrLast & " transactions have ended. Please complete your sales report. The due date is " & FormatDateTime(deadline.AddDays(-1), DateFormat.ShortDate) & ". http://app.custombuilders-usa.com/rebates/vendor-sales.aspx"
                Try
                    dbMsg.SendLLCNotificationListAReminder(LLCRow.GetLLCNotificationList(DB), "SalesReportReminder2 Sent" & vbCrLf & sMsg)
                Catch ex As Exception
                End Try
                For Each row As DataRow In dtVendors.Rows
                    Dim dbVendor As VendorRow = VendorRow.GetRow(DB, row("VendorId"))
                    Try
                        dbMsg.Send(dbVendor, sMsg, CCLLCNotification:=False)

                    Catch ex As Exception
                        Continue For
                    End Try

                Next
            ElseIf Now.Date = reminder3.Date Then
                'AR 080409: Only ACTIVE users that HAVE NOT reported should get reminders
                Dim dtBuilders As DataTable = DB.GetDataTable("select BuilderId from Builder where IsActive = 1 AND Submitted < " & DB.Quote(qtrEnd) & " And BuilderId not in (select BuilderId from PurchasesReport where PeriodQuarter=" & qtrLast & " and PeriodYear=" & yrLast & ")")
                Dim dtVendors As DataTable = DB.GetDataTable("select VendorId from Vendor where IsActive = 1 AND QuarterlyReportingOn = 1 AND Submitted < " & DB.Quote(qtrEnd) & " And VendorId not in (select VendorId from SalesReport where PeriodQuarter=" & qtrLast & " and PeriodYear=" & yrLast & ")")

                Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "PurchasesReportDue")
                Dim sMsg As String = "Q" & qtrLast & " " & yrLast & " reports are due today. Please submit your report. http://app.custombuilders-usa.com/rebates/builder-purchases.aspx "


                Try
                    dbMsg.SendLLCNotificationListAReminder(LLCRow.GetLLCNotificationList(DB), "PurchasesReportDue Sent" & vbCrLf & sMsg)
                Catch ex As Exception
                End Try
                For Each row As DataRow In dtBuilders.Rows
                    Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, row("BuilderId"))
                    Console.WriteLine("Sending Builder reminder")
                    Try
                        dbMsg.Send(dbBuilder, sMsg, CCLLCNotification:=False)

                    Catch ex As Exception
                        Continue For
                    End Try

                Next

                dbMsg = AutomaticMessagesRow.GetRowByTitle(DB, "SalesReportDue")
                sMsg = dbMsg.Message
                sMsg = sMsg.Replace("%%Quarter%%", "Q" & qtrLast & " " & yrLast)

                ' sMsg = "Q" & qtrLast & " " & yrLast & " reports are due today. Please submit your report. http://app.custombuilders-usa.com/rebates/vendor-sales.aspx"
                Try
                    dbMsg.SendLLCNotificationListAReminder(LLCRow.GetLLCNotificationList(DB), "SalesReportDue Sent" & vbCrLf & sMsg, True)
                Catch ex As Exception
                End Try
                For Each row As DataRow In dtVendors.Rows

                    Dim dbVendor As VendorRow = VendorRow.GetRow(DB, row("VendorId"))
                    Try

                        Console.WriteLine("Sending Vendor reminder")
                        dbMsg.Send(dbVendor, sMsg, True, CCLLCNotification:=False)

                    Catch ex As Exception
                        Continue For
                    End Try

                Next
            End If
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "ReportingReminders"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()

        Catch ex As Exception
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "ReportingReminders"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        End Try
    End Sub
End Class
