Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager

Public Class SendMemberReminders

    Public Shared Sub Run(ByVal DB As Database)
        Console.WriteLine("Running SendMemberReminders ... ")

        Try
            Dim sSQL As String, dvReminders As DataView, sMsg As String
            Dim ReminderId As Integer, Email As String, Name As String, EventDate As DateTime, Body As String, IsRecurrent As Boolean

            sSQL = "select * from (" _
                      & " select " _
                      & " ReminderId, IsRecurrent, Memberid, Name, Email, EventDate, " _
                      & " CASE WHEN IsRecurrent = 1 THEN " _
                      & " 	CASE WHEN MONTH(DATEADD(d,-DaysBefore1,EventDate)) = Month(getdate()) AND DAY(DATEADD(d,-DaysBefore1,EventDate)) = Day(getdate()) THEN 1 ELSE 0 END" _
                      & " ELSE" _
                      & " 	CASE WHEN DATEDIFF(d, getdate(), DATEADD(d,-DaysBefore1,EventDate)) = 0 THEN 1 ELSE 0 END" _
                      & " END AS SAME1," _
                      & " CASE WHEN IsRecurrent = 1 THEN " _
                      & " 	CASE WHEN MONTH(DATEADD(d,-DaysBefore2,EventDate)) = Month(getdate()) AND DAY(DATEADD(d,-DaysBefore2,EventDate)) = Day(getdate()) THEN 1 ELSE 0 END" _
                      & " ELSE" _
                      & " 	CASE WHEN DATEDIFF(d, getdate(), DATEADD(d,-DaysBefore2,EventDate)) = 0 THEN 1 ELSE 0 END" _
                      & " END AS SAME2," _
                      & " Coalesce(BODY,'') As Body from MemberReminder" _
                      & " ) as tmp WHERE SAME1 = 1 OR SAME2 = 1"

            dvReminders = DB.GetDataTable(sSQL).DefaultView
            For iLoop As Integer = 0 To dvReminders.Count - 1
                ReminderId = Trim(dvReminders(iLoop).Item("ReminderId"))
                Email = Trim(dvReminders(iLoop).Item("Email"))
                Name = Trim(dvReminders(iLoop).Item("Name"))
                EventDate = Trim(dvReminders(iLoop).Item("EventDate"))
                Body = Trim(dvReminders(iLoop).Item("Body"))
                IsRecurrent = Trim(dvReminders(iLoop).Item("IsRecurrent"))

                sMsg = "You requested to be reminded for the following event: " & Name & vbCrLf
                sMsg &= "Event Date: " & FormatDateTime(EventDate, DateFormat.LongDate).ToString
                If IsRecurrent = True Then sMsg &= " - Recurring Annual Event"

                If Not Body = String.Empty Then sMsg = sMsg & vbCrLf & vbCrLf & "You also specified the following comments for this event: " & vbCrLf & Body

                sMsg = sMsg & vbCrLf & vbCrLf
                sMsg = sMsg & "Sincerely," & vbCrLf
                sMsg = sMsg & AppSettings("ReminderEmailSignedName")

                Core.SendSimpleMail(AppSettings("ReminderEmailFrom"), AppSettings("ReminderEmailFromName"), Email, Name, AppSettings("ReminderEmailSubject"), sMsg)

                DB.ExecuteSQL("INSERT INTO MemberReminderLog (ReminderId) VALUES (" & DB.Number(ReminderId) & ")")
            Next

        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
        End Try
    End Sub
End Class
