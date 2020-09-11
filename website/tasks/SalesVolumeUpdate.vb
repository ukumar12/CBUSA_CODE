Imports Components
Imports DataLayer

Public Class SalesVolumeUpdate

    Public Shared Sub Run(ByVal DB As Database)
        Dim currentQtr As Integer = Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
        Dim lastQtr As Integer = IIf(currentQtr = 1, 4, currentQtr - 1)

        Dim lastYear As Integer = IIf(lastQtr = 4, Now.Year - 1, Now.Year)
        Dim lastQtrEnd As DateTime = (lastQtr * 3) & "/" & Date.DaysInMonth(lastYear, lastQtr * 3) & "/" & lastYear
        Dim deadline As DateTime = lastQtrEnd.AddMonths(1).AddDays(SysParam.GetValue(DB, "ReportDeadlineDays")).AddDays(SysParam.GetValue(DB, "DiscrepancyDeadlineDays")).AddDays(SysParam.GetValue(DB, "DiscrepancyResponseDeadlineDays")).AddDays(7)

        Dim dbTaskLog As TaskLogRow = Nothing

        Try
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "SalesVolumeUpdate"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1)
            dbTaskLog.Insert()

            If deadline.Date <> Now.Date Then
                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "SalesVolumeUpdate"
                dbTaskLog.Status = "Skipped"
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1)
                dbTaskLog.Insert()
                Exit Sub
            End If

            Dim sqlVendorListSalesVolumeUpdate As String = "SELECT APVR.VendorId, APVR.VendorName, COALESCE(SUM(APVR.InitialVendorAmount), 0.00) AS VendorReportedTotal, COALESCE(SUM(APVR.FinalAmount), 0.00) AS VendorTotalFinalAmount, " _
                                                            & "APVR.HasReportedVendor, LLC.LLC, LLCTPV.LLCTotalPurchaseVolume, LLC.OperationsManager, A.FirstName, A.LastName, A.Contact " _
                                                            & "FROM vAPVforReport APVR  " _
                                                            & "JOIN LLCVendor ON APVR.VendorID = LLCVendor.VendorID  " _
                                                            & "JOIN LLC On LLCVendor.LLCID = LLC.LLCID  " _
                                                            & "JOIN LLCTotalPurchaseVolume LLCTPV ON LLCTPV.LLC = LLC.LLC " _
                                                            & "JOIN [Admin] A ON LLC.OperationsManager = A.Email  " _
                                                            & "WHERE APVR.PeriodYear = " & Core.ProtectParam(lastYear) & " AND APVR.PeriodQuarter = " & Core.ProtectParam(lastQtr) & " " _
                                                            & "AND LLCTPV.PeriodQuarter = " & Core.ProtectParam(lastQtr) & " AND LLCTPV.PeriodYear = " & Core.ProtectParam(lastYear) & " " _
                                                            & "GROUP BY APVR.VendorId, APVR.VendorName, APVR.HasReportedVendor, LLC.LLC, LLCTPV.LLCTotalPurchaseVolume, LLC.OperationsManager, A.FirstName, A.LastName, A.Contact"

            'Dim sqlVendorListSalesVolumeUpdate As String = "SELECT V.VendorId, V.CompanyName AS VendorName, 0.00 AS VendorReportedTotal, 0.00 AS VendorTotalFinalAmount, " _
            '                                                & "'No' AS HasReportedVendor, LLC.LLC, LLCTPV.LLCTotalPurchaseVolume, LLC.OperationsManager, A.FirstName, A.LastName, A.Contact " _
            '                                                & "FROM Vendor V " _
            '                                                & "JOIN LLCVendor ON V.VendorID = LLCVendor.VendorID  " _
            '                                                & "JOIN LLC On LLCVendor.LLCID = LLC.LLCID  " _
            '                                                & "JOIN LLCTotalPurchaseVolume LLCTPV ON LLCTPV.LLC = LLC.LLC " _
            '                                                & "JOIN [Admin] A ON LLC.OperationsManager = A.Email  " _
            '                                                & "WHERE V.VendorID Not IN (SELECT DISTINCT VendorID FROM vAPVforReport WHERE PeriodYear = 2019 And PeriodQuarter = 2) " _
            '                                                & "AND V.IsActive = 1 " _
            '                                                & "And LLCTPV.PeriodQuarter = 2 And LLCTPV.PeriodYear = 2019 " _
            '                                                & "GROUP BY V.VendorId, V.CompanyName, LLC.LLC, LLCTPV.LLCTotalPurchaseVolume, LLC.OperationsManager, A.FirstName, A.LastName, A.Contact " _
            '                                                & "ORDER BY V.CompanyName"

            Dim dtVendors As DataTable = DB.GetDataTable(sqlVendorListSalesVolumeUpdate)
            Dim cnt As Integer = 0

            For Each row As DataRow In dtVendors.Rows
                Try
                    Dim strVendorID As String = row("VendorID")
                    Dim strVendorName As String = row("VendorName")
                    Dim VendorReportedTotal As Double = CDbl(row("VendorReportedTotal"))
                    Dim VendorTotalFinalAmount As Double = CDbl(row("VendorTotalFinalAmount"))
                    Dim strLLC As String = row("LLC")
                    Dim HasReportedVendor As String = row("HasReportedVendor")
                    Dim LLCTotalPurchaseVolume As Double = CDbl(row("LLCTotalPurchaseVolume"))
                    Dim strOpsManagerEmail As String = row("OperationsManager")
                    Dim strOpsManagerName As String = String.Concat(row("FirstName"), " ", row("LastName"))
                    Dim strOpsManagerContactNumber As String = row("Contact")

                    Console.WriteLine("Sending out mail to " & strVendorName)

                    Dim dbAutoMsg As AutomaticMessagesRow

                    If HasReportedVendor = "Yes" And VendorReportedTotal > 0.0 And VendorTotalFinalAmount > 0.00 Then
                        dbAutoMsg = AutomaticMessagesRow.GetRowByTitle(DB, "SalesVolumeUpdate_VendorReported")
                    ElseIf HasReportedVendor = "No" And VendorTotalFinalAmount > 0.00 Then
                        dbAutoMsg = AutomaticMessagesRow.GetRowByTitle(DB, "SalesVolumeUpdate_VendorDNR_FinalAmountNonZero")
                    ElseIf HasReportedVendor = "No" And VendorTotalFinalAmount = 0.00 Then
                        dbAutoMsg = AutomaticMessagesRow.GetRowByTitle(DB, "SalesVolumeUpdate_VendorDNR_FinalAmountZero")
                    ElseIf HasReportedVendor = "Yes" And VendorReportedTotal = 0.0 And VendorTotalFinalAmount > 0.00 Then
                        dbAutoMsg = AutomaticMessagesRow.GetRowByTitle(DB, "SalesVolumeUpdate_Vendor0Report_FinalAmountNonZero")
                    ElseIf HasReportedVendor = "Yes" And VendorReportedTotal = 0.0 And VendorTotalFinalAmount = 0.00 Then
                        dbAutoMsg = AutomaticMessagesRow.GetRowByTitle(DB, "SalesVolumeUpdate_Vendor0Report_FinalAmount0")
                    Else
                        Console.WriteLine("Please help!!")
                    End If

                    Dim VendorAccountEmail As String = String.Empty

                    Dim strVendorTotalFinalAmount As String = FormatCurrency(VendorTotalFinalAmount)
                    Dim strLLCTotalSalesVolume As String = FormatCurrency(LLCTotalPurchaseVolume)

                    Dim dtVendorContacts As DataTable = DB.GetDataTable("SELECT DISTINCT VA.FirstName, VA.Email FROM VendorAccount VA WHERE VA.VendorId = " & DB.Number(row("VendorID")) & " AND VA.IsActive = 1")
                    If dtVendorContacts.Rows.Count > 0 Then

                        Dim strMessageTemplate As String = dbAutoMsg.Message

                        For Each dr As DataRow In dtVendorContacts.Rows
                            If Not IsDBNull(dr("Email")) Then
                                VendorAccountEmail = dr("Email")
                            End If

                            Dim MsgBody As String = String.Empty

                            MsgBody = strMessageTemplate

                            MsgBody = MsgBody.Replace("%%ContactFirstName%%", dr("FirstName"))
                            MsgBody = MsgBody.Replace("%%VendorName%%", strVendorName)
                            MsgBody = MsgBody.Replace("%%Quarter%%", lastQtr)
                            MsgBody = MsgBody.Replace("%%Year%%", lastYear)
                            MsgBody = MsgBody.Replace("%%LLC%%", strLLC)
                            MsgBody = MsgBody.Replace("%%VendorTotalFinalAmount%%", strVendorTotalFinalAmount)
                            MsgBody = MsgBody.Replace("%%LLCTotalFinalAmount%%", strLLCTotalSalesVolume)
                            MsgBody = MsgBody.Replace("%%OpsManagerFullName%%", strOpsManagerName)
                            MsgBody = MsgBody.Replace("%%OpsManagerTitle%%", "Operations Manager - CBUSA")
                            MsgBody = MsgBody.Replace("%%OpsManagerEmail%%", strOpsManagerEmail)
                            MsgBody = MsgBody.Replace("%%OpsManagerContactNumber%%", strOpsManagerContactNumber)

                            dbAutoMsg.Message = MsgBody
                            dbAutoMsg.Subject = dbAutoMsg.Subject.Replace("%%LLC%%", row("LLC")).Replace("%%Quarter%%", lastQtr)

                            'VendorAccountEmail = "abasu@medullus.com"
                            Core.SendSimpleMail(strOpsManagerEmail, strOpsManagerName, VendorAccountEmail, "", dbAutoMsg.Subject, dbAutoMsg.Message, "", "")

                            'If HasReportedVendor = "No" And VendorTotalFinalAmount > 0.00 Then
                            '    Core.SendSimpleMail(strOpsManagerEmail, strOpsManagerName, VendorAccountEmail, "", dbAutoMsg.Subject, dbAutoMsg.Message, "", "")
                            'ElseIf HasReportedVendor = "Yes" And VendorTotalFinalAmount > 0.00 Then
                            '    Core.SendSimpleMail(strOpsManagerEmail, strOpsManagerName, VendorAccountEmail, "", dbAutoMsg.Subject, dbAutoMsg.Message, "", "")
                            'ElseIf HasReportedVendor = "Yes" And VendorReportedTotal = 0.0 And VendorTotalFinalAmount > 0.00 Then
                            '    Core.SendSimpleMail(strOpsManagerEmail, strOpsManagerName, VendorAccountEmail, "", dbAutoMsg.Subject, dbAutoMsg.Message, "", "")
                            'End If
                        Next
                    End If

                    cnt += 1

                Catch ex As Exception
                    'Logger.Info("Error sending SalesVolumeUpdate email to vendor: " & row("VendorID"))
                End Try
            Next

            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "SalesVolumeUpdate"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = "Deadline: " & deadline.AddDays(-1) & " (" & cnt & " SalesVolumeUpdate emails sent)"
            dbTaskLog.Insert()

        Catch ex As Exception
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "SalesVolumeUpdate"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        End Try

    End Sub

End Class
