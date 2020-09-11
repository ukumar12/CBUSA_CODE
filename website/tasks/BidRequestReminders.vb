Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components
Imports Controls
Imports System.Configuration.ConfigurationManager

Public Class BidRequestReminders

    Private Shared GlobalRefererName As String = AppSettings("GlobalRefererName")
    Private Shared GlobalSecureName As String = AppSettings("GlobalSecureName")
    Protected Shared fromEmail As String
    Private Shared fromName As String
    Private dbQuote As POQuoteRow

    Public Shared Sub Run(ByVal DB As Database)

        GlobalRefererName = AppSettings("GlobalRefererName")
        GlobalSecureName = AppSettings("GlobalSecureName")
        fromName = SysParam.GetValue(DB, "ContactUsName")
        fromEmail = SysParam.GetValue(DB, "ContactUsEmail")
        Dim sql As String = String.Empty
        Dim dbTaskLog As TaskLogRow = Nothing


        Console.WriteLine("Running BidRequestReminders ... ")

        Try

            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "BidRequestReminder"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()

            sql = "select  qr.QuoteRequestId, q.QuoteId, qr.VendorId,b.CompanyName As BuilderName, l.LLC , l.LLCID  , "
            sql &= " qr.QuoteTotal, qr.QuoteExpirationDate, qr.CreateDate, qr.RequestStatus, qr.ModifyDate,"
            sql &= " v.CompanyName, v.Email as VendorEmail, "
            sql &= " q.Title, q.Deadline, q.StatusDate, q.Status, "
            sql &= " p.ProjectName, p.Subdivision, p.LotNumber, p.City, p.State"
            sql &= " from POQuoteRequest qr "
            sql &= " inner join Vendor v on v.VendorID = qr.VendorId "
            sql &= " inner join POQuote q on qr.QuoteId = q.QuoteId "
            sql &= " inner join Project p on q.ProjectId = p.ProjectID  "
            sql &= " inner join Builder b on p.BuilderID = b.BuilderID  "
            sql &= " inner join LLC l on l.LLCID = b.LLCID   "
            sql &= " where(DateDiff(Day, getdate(), q.Deadline) = 1) "
            'sql &= "where year(q.Deadline) = 2017 and month(q.Deadline) = 12 "
            sql &= " AND (qr.QuoteTotal = 0 or qr.QuoteTotal is null) and (qr.RequestStatus <>'Cancelled')   "
            sql &= " and (qr.RequestStatus <>'Exited Market') and v.IsActive = 1  "     '*** Line added by Apala (Medullus) on 02.02.2018 to fix mGuard Ticket T-1087

            Dim dtQuoteRequests As DataTable = DB.GetDataTable(sql)
            Dim bHasErrors As Boolean = False
            Dim errorMsg As New StringBuilder
            Dim ecount As Integer = 0
            If dtQuoteRequests.Rows.Count > 0 Then

                errorMsg.AppendLine("BidRequestReminder Task: " & DateTime.Now)
                Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "BidRequestDeadline")
                For Each row As DataRow In dtQuoteRequests.Rows
                    Try

                        Dim sBody As String = FormatMessage(DB, row, dbMsg.Message)
                        'SendEmail(DB, row, sBody, dbMsg)

                        dbMsg.Send(VendorRow.GetRow(DB, row("vendorid")), sBody, True)

                        If dbMsg.CCList <> String.Empty Then
                            Dim aEmails() As String = dbMsg.CCList.Split(",")
                            For Each email As String In aEmails
                                Core.SendSimpleMail(fromEmail, fromName, email, email, dbMsg.Subject, sBody)
                            Next
                        End If

                    Catch ex As Exception
                        bHasErrors = True
                        errorMsg.AppendLine("----------------------------------")
                        errorMsg.AppendLine("Error sending to :")
                        errorMsg.AppendLine("Vendor: " & Core.GetString(row("CompanyName")))
                        errorMsg.AppendLine("QuoteId: " & Core.GetInt(row("QuoteId")))
                        errorMsg.AppendLine("QuoteRequestId: " & Core.GetInt(row("QuoteRequestId")))
                        errorMsg.AppendLine("QuoteExpirationDate: " & Core.GetString(row("QuoteExpirationDate")))
                        errorMsg.AppendLine(ex.Message)
                        errorMsg.AppendLine("----------------------------------")
                    End Try
                Next

                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "BidRequestReminder"
                dbTaskLog.Status = "Completed"
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "BidRequest Reminders Sent "
                dbTaskLog.Insert()
            End If
            If bHasErrors Then
                Logger.Error(errorMsg.ToString)
                Console.WriteLine("Errors occured in this task.  Please review log4net to see detailed information regarding these errors")
            End If

        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "BidRequestReminder"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        End Try
    End Sub
    Private Shared Sub SendEmail(ByVal DB As Database, ByVal Account As DataRow, ByVal AdditionalMessage As String, ByVal dbAutoMessage As AutomaticMessagesRow)
        Dim sBody As String

        sBody = AdditionalMessage


        Dim addr As String = Account("VendorEmail")

        If Core.IsEmail(addr) Then
            Core.SendSimpleMail(fromEmail, fromName, addr, Account("CompanyName"), dbAutoMessage.Subject, sBody)

            Console.WriteLine("Email Sent to: " & addr & " | Company: " & Account("CompanyName") & " | QuoteRequestId: " & Account("QuoteRequestId"))
        Else
            Throw New Exception("Vendor Email was invalid")
        End If
    End Sub
    Private Shared Function FormatMessage(ByVal DB As Database, ByVal drVendor As DataRow, ByVal Msg As String) As String
        Msg = Msg.Replace("%%Vendor%%", drVendor("CompanyName"))
        Msg = Msg.Replace("%%QuoteDetails%%", GetQuoteDetailsBody(DB, drVendor, Msg))
        Msg = Msg.Replace("%%QuoteRequestDetails%%", GetRequestDetailsBody(drVendor))
        Msg = Msg.Replace("%%QuoteRequestUrl%%", IIf(GlobalSecureName <> Nothing, GlobalSecureName, GlobalRefererName) & "/vendor/plansonline/quoterequestmessages.aspx?QuoteRequestId=" & drVendor("QuoteRequestId"))

        Return Msg
    End Function
    Private Shared Function GetQuoteDetailsBody(ByVal DB As Database, ByVal drQuoteInfo As DataRow, ByVal sMessage As String) As String
        Dim sBody As String = vbCrLf & vbCrLf

        sBody &= "Builder Name: " & Core.GetString(drQuoteInfo("BuilderName")) & vbCrLf
        sBody &= "Market : " & Core.GetString(drQuoteInfo("LLC")) & vbCrLf
        sBody &= "Bid Request #: " & Core.GetString(drQuoteInfo("QuoteId")) & vbCrLf
        sBody &= "Title: " & Core.GetString(drQuoteInfo("Title")) & vbCrLf
        sBody &= "Deadline: " & DateTime.Parse(Core.GetDate(drQuoteInfo("Deadline")).ToShortDateString()).ToString("dd-MMM-yyyy") & vbCrLf          '*** Line added by Apala (Medullus) on 02.02.2018 to fix mGuard Ticket T-1090
        sBody &= "Status: " & Core.GetString(drQuoteInfo("Status")) & " (" & Core.GetString(drQuoteInfo("StatusDate")) & ")" & vbCrLf
        sBody &= "Project: " & Core.GetString(drQuoteInfo("ProjectName")) & vbCrLf
        sBody &= "Subdivision: " & Core.GetString(drQuoteInfo("Subdivision")) & vbCrLf
        sBody &= "Lot #: " & Core.GetString(drQuoteInfo("LotNumber")) & vbCrLf
        sBody &= "City: " & Core.GetString(drQuoteInfo("City")) & vbCrLf
        sBody &= "State: " & Core.GetString(drQuoteInfo("State")) & vbCrLf
        Dim dt As DataTable = DB.GetDataTable("SELECT qvc.QuoteId, vc.Category FROM POQuoteVendorCategory qvc INNER JOIN VendorCategory vc ON vc.VendorCategoryId = qvc.VendorCategoryId where qvc.QuoteId = " & Core.GetString(drQuoteInfo("QuoteID")))
        If dt.Rows.Count > 0 Then
            Dim Conn As String = String.Empty
            Dim Result As String = String.Empty
            For Each item As DataRow In dt.Rows
                Result &= Conn & item("Category")
                Conn = ","
            Next
            sBody &= "Phases: " & Result
        End If
        Return sBody

    End Function
    Private Shared Function GetRequestDetailsBody(ByVal drQuoteInfo As DataRow) As String
        Dim QuoteTotal As Double = Core.GetDouble(drQuoteInfo("QuoteTotal"))
        Dim QuoteExpirationDate As DateTime = Core.GetDate(drQuoteInfo("QuoteExpirationDate"))
        Dim sBody As String = "Request Status: " & drQuoteInfo("RequestStatus") & " (" & drQuoteInfo("ModifyDate") & ")" & vbCrLf
        sBody &= "Bid Total: " & IIf(QuoteTotal > 0, FormatCurrency(QuoteTotal, 2), "") & vbCrLf
        sBody &= "Expiration Date: " & IIf(QuoteTotal <> Nothing, QuoteTotal.ToString(), "") & vbCrLf

        Return sBody

    End Function
End Class
