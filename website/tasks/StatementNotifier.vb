
Imports System.Configuration.ConfigurationManager
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports Components
Imports System.Net.Mail
Imports DataLayer
Imports System.Web
Imports System.Net
Imports System.Data.SqlClient
Imports System.Collections.Specialized
Imports System.Linq

Public Class StatementNotifier

    Private Shared path As String = ""

    Public Shared Sub Run(ByVal DB As Database)
        Console.WriteLine("Running StatementNotifier ... ")

        Dim Sql As String = String.Empty
        Dim sql2 As String = String.Empty
        Dim dbTaskLog As TaskLogRow = Nothing

        path = SysParam.GetValue(DB, "StatementsFilePath")

        dbTaskLog = New TaskLogRow(DB)
        dbTaskLog.TaskName = "StatementNotifier"
        dbTaskLog.Status = "Started"
        dbTaskLog.LogDate = Now()
        dbTaskLog.Msg = ""
        dbTaskLog.Insert()

        Dim FilesNames As String = ""
        Dim con As String = ""
        Dim fileEntries() As String
        Dim fileEntries1 As List(Of String) = New List(Of String)
        Dim NewFiles() As String
        Dim LoggingString As String = ""

        Try
            'If Directory.Exists(path) Then
            Console.WriteLine("Starting the Try... ")
            fileEntries = Directory.GetFiles(path)

            Dim i As Integer = 0
            While i < fileEntries.Count
                fileEntries(i) = (fileEntries(i).Replace(path, ""))

                If fileEntries(i).Contains(".pdf") Then
                    fileEntries1.Add(fileEntries(i))
                    FilesNames = FilesNames & con & fileEntries(i).Replace(".pdf", "")
                    Console.WriteLine("Processed file '{0}'.", fileEntries(i))
                    con = ","
                End If
                i += 1
            End While

            fileEntries = fileEntries1.ToArray()

            i = 0
            Console.WriteLine("loop into files ... ")
            While i < fileEntries.Count
                fileEntries(i) = (fileEntries(i).Replace(path, ""))

                If fileEntries(i).Contains(".pdf") Then
                    FilesNames = FilesNames & con & fileEntries(i).Replace(".pdf", "")
                    Console.WriteLine("Processed file '{0}'.", fileEntries(i))
                    con = ","
                End If
                i += 1
            End While

            LoggingString &= " Total of files found : " & fileEntries.Count & "<br />"
            Console.WriteLine("Total of files found : " & fileEntries.Count)

            Sql = " select  FileName "
            Sql &= " from Statement  "

            Console.WriteLine("Getting file names from the database")
            Dim result As DataTable = DB.GetDataTable(Sql)
            Dim rows() As String = (From row In result.AsEnumerable()
                                    Select row.Field(Of String)("FileName")).ToArray()

            Console.WriteLine("Found " & rows.Count & "rows")
            If rows.Count > 0 Then
                NewFiles = fileEntries.Except(rows).ToArray
            Else
                NewFiles = fileEntries
            End If

            LoggingString &= " Total of new files found : " & rows.Count & "<br />"
            Console.WriteLine("Total of new files found : " & rows.Count)

            Dim file As String
            Console.WriteLine("Processing the " & rows.Count & " new files")
            For Each file In NewFiles
                Console.WriteLine(" Start to process files '{0}'.", file)

                Dim file_parts() As String = file.Replace(".pdf", "").Split("_")
                Dim StatementId As Integer = 0

                Try
                    DB.BeginTransaction()

                    Sql = " INSERT INTO Statement (" _
                           & "HistoricId" _
                           & ",FileName" _
                           & ",StatementDate" _
                           & ") VALUES (" _
                           & DB.NullNumber(file_parts(0)) _
                           & "," & DB.Quote(file) _
                           & "," & DB.NullQuote(Date.ParseExact(file_parts(2) & "/01/" & file_parts(1), "MM/dd/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo)) _
                           & ")"

                    Console.WriteLine(" Insert to database '{0}'.", file)
                    StatementId = DB.InsertSQL(Sql)

                    DB.CommitTransaction()
                    ' notify
                    Console.WriteLine(" Sending notification for '{0}'.", file)

                    Dim dtBuilder As DataTable = DB.GetDataTable("select * from builder where [HistoricID]=" & DB.Number(file_parts(0)))

                    Dim rowBuilder As DataRow
                    For Each rowBuilder In dtBuilder.Rows
                        Try

                            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, rowBuilder("BuilderID"))
                            Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "NewStatement")
                            dbMsg.Message = dbMsg.Message.Replace("%%FileLink%%", AppSettings("GlobalRefererName") & "/file.aspx?Id=" & StatementId)

                            If dbBuilder.IsActive Then


                                Dim Recipients As String = String.Empty
                                Recipients = DB.ExecuteScalar("select CAST(stuff((SELECT distinct ',' + t1.Email FROM BuilderAccount  t1 WHERE t1.BuilderID  = t2.BuilderID FOR XML PATH('')),1,1,'') as varchar(max)) + ',' + t2.Email  AS Email from Builder  t2 where t2.BuilderID  = " & dbBuilder.BuilderID)
                                Dim aRecipients As String() = Recipients.Split(","c)
                                'get unique emails
                                Dim UniqueaRecipients As String() = aRecipients.Distinct.ToArray
                                'Now send Email to all builder emails/accounts
                                Dim dtBuilderAccount As DataTable = DB.GetDataTable("select * from BuilderAccount where BuilderID=" & DB.Number(dbBuilder.BuilderID) & " AND ISActive = 1 order by LastName, FirstName")
                                For Each emailRecipient As String In UniqueaRecipients
                                    Try
                                        Dim fromEmail As String = SysParam.GetValue(DB, "ContactUsEmail")
                                        Dim fromName As String = SysParam.GetValue(DB, "ContactUsName")
                                        dbMsg.Message = dbMsg.Message.Replace("%%Builder%%", dbBuilder.CompanyName)
                                        Dim addr As String = emailRecipient
                                        If SysParam.GetValue(DB, "AutoMessageTestMode") Then
                                            addr = SysParam.GetValue(DB, "AdminEmail")
                                        End If
                                        If Core.IsEmail(addr) Then
                                            Core.SendSimpleMail(fromEmail, fromName, addr, dbBuilder.CompanyName, dbMsg.Subject, dbMsg.Message)
                                        End If
                                    Catch ex As Exception
                                    End Try
                                Next
                            End If

                        Catch ex As Exception
                            Continue For
                        End Try
                    Next

                    Console.WriteLine(" End process file '{0}'.", file)
                Catch ex As Exception
                    If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                    LoggingString &= "Error found when processing the file :" & file & ex.Message & "<br />"
                    Continue For
                End Try
            Next file
            Console.WriteLine("End processing the " & rows.Count & " new files")
            'Else
            'Console.WriteLine("{0} is not a valid file or directory.", path)
            'LoggingString &= path & "is not a valid file or directory" & "<br />"
            'End If

            ' TODO
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "StatementNotifier"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = LoggingString
            dbTaskLog.Insert()

            Console.WriteLine("End StatementNotifier")
        Catch ex As Exception
            ' TODO: 

            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "StatementNotifier"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = "" & LoggingString & ex.Message & "<br />"
            dbTaskLog.Insert()
            Console.WriteLine("Catch Errors : " & LoggingString & ex.Message)
            Console.WriteLine("End StatementNotifier ")
        Finally
            DB.Close()
        End Try

    End Sub

    Private Shared Sub SendErrorNotification(ByVal sBody As String)
        Dim Sender As String = AppSettings("ErrorLogEmailFrom")
        Dim Recipients As String = AppSettings("ErrorLogEmailRecipients")
        Dim Subject As String = AppSettings("ErrorLogEmailSubject")

        Dim SmtpMail As SmtpClient = New SmtpClient(AppSettings("MailServer"))
        SmtpMail.Send(Sender, Recipients, Subject, sBody)
    End Sub

End Class