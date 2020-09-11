Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager
Imports System.Net

Public Class PlansOnlineVendorEntryNotification

    Public Shared Sub Run(ByVal DB As Database)
        Console.WriteLine("Running PlansOnlineVendorEntryNotification ... ")
        Dim dbTaskLog As TaskLogRow = Nothing

        Try
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "PlansOnlineVendorEntryNotification"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()

            Dim sql As String = "Select * From POQuote Where Status = 'Bidding In Progress'"
            Dim dt As DataTable = DB.GetDataTable(sql)
            Dim Count As Integer = 0
            Dim rCount As Integer = 0
            Dim sVendors As String = "0"
            For Each row As DataRow In dt.Rows
                Count += 1
                Console.WriteLine("Processing bid " & Count & " of " & dt.Rows.Count)
                Dim vCount As Integer = 0
                sql = "Select Distinct v.VendorId From Vendor v Inner Join VendorCategoryVendor vc On v.VendorId = vc.VendorId Where v.IsActive = 1 And v.IsPlansOnline = 1 And vc.VendorCategoryId In (Select VendorCategoryId From POQuoteVendorCategory Where QuoteId = " & DB.Number(row("QuoteId")) & ") And (Select Count(QuoteRequestId) From POQuoteRequest Where VendorId = v.VendorId And QuoteId = " & DB.Number(row("QuoteId")) & ") = 0 And Exists (Select q.QuoteId From POQuote q Inner Join Project p On q.ProjectId = p.ProjectId Inner Join Builder b On p.BuilderId = b.BuilderId Where q.QuoteId = " & DB.Number(row("QuoteId")) & " And b.LLCID In (Select LLCID From LLCVendor Where VendorId = v.VendorId))"
                Dim dtVendor As DataTable = DB.GetDataTable(sql)
                For Each r As DataRow In dtVendor.Rows
                    Try
                        DB.BeginTransaction()
                        'Add quote requests for new vendor
                        sql = "Insert Into POQuoteRequest (QuoteId, VendorId, RequestStatus, CreateDate, ModifyDate) Values (" & DB.Number(row("QuoteId")) & ", " & DB.Number(r("VendorId")) & ", 'New', " & DB.Quote(Now) & ", " & DB.Quote(Now) & ")"
                        Dim QuoteRequestId As Integer = DB.InsertSQL(sql)
                        DB.CommitTransaction()
                        'Collect VendorId to send emails later.
                        If InStr(sVendors, "," & r("VendorId")) = 0 Then
                            sVendors &= "," & r("VendorId")
                        End If
                        rCount += 1
                    Catch ex As Exception
                        If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                        Console.WriteLine("Error Processing row " & Count & " of " & dt.Rows.Count & " Error: " & ex.Message)
                    End Try
                Next
            Next

            'send email to vendors
            Dim eCount As Integer = 0
            If sVendors <> "0" Then
                sVendors = sVendors.Replace("0,", "")
                Dim fromEmail As String = SysParam.GetValue(DB, "ContactUsEmail")
                Dim fromName As String = SysParam.GetValue(DB, "ContactUsName")
                Dim sBody As String = "You have new Plans Online Bid Requests. Please log on to https://app.custombuilders-usa.com/vendor/plansonline/ to review them."

                sql = "Select VendorId, CompanyName, Email From Vendor Where VendorId In " & DB.NumberMultiple(sVendors)
                dt = DB.GetDataTable(sql)
                For Each row As DataRow In dt.Rows
                    Try
                        Core.SendSimpleMail(fromEmail, fromName, row("Email"), row("CompanyName"), "Plans Online - New Bid Request", sBody)
                        eCount += 1
                    Catch ex As Exception
                        Console.WriteLine("Error sending email" & row("CompanyName") & " " & ex.Message)
                    End Try
                Next
            End If
            

            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "PlansOnlineVendorEntryNotification"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = "Requests: " & rCount & "<br>Emails: " & eCount
            dbTaskLog.Insert()
        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "PlansOnlineVendorEntryNotification"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        End Try
    End Sub

End Class
