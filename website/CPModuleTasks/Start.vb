Imports Components
Imports System.Net.Mail
Imports System.Configuration.ConfigurationManager
Imports DataLayer

Module Start

    Sub Main()

        Dim Connectionstring As String = DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword"))

        Dim DB As New Database

        Dim args As String() = {"vendorbidsubmissionreminder"}

        Try
            If My.Application.CommandLineArgs.Count > 0 Then
                ReDim Preserve args(My.Application.CommandLineArgs.Count)
                My.Application.CommandLineArgs.CopyTo(args, 0)
            End If

            DB.Open(Connectionstring)

            For Each arg As String In args
                Dim task As String = LCase(arg)

                Try
                    Try
                        If DB Is Nothing Then DB.Open(Connectionstring)
                        If task = LCase("EnteredProjectionsReminder") OrElse task = "all" Then EnteredProjectionsReminder.Run(DB)
                    Catch ex As Exception
                    End Try


                    Try
                        If DB Is Nothing Then DB.Open(Connectionstring)
                        If task = LCase("VendorBidSubmissionReminder") OrElse task = "all" Then VendorBidSubmissionReminder.Run(DB)
                    Catch ex As Exception
                    End Try

                    Try
                        If DB Is Nothing Then DB.Open(Connectionstring)
                        If task = LCase("VendorEventNotification") OrElse task = "all" Then VendorEventNotification.Run(DB)
                    Catch ex As Exception
                    End Try
                Catch ex As Exception
                End Try


            Next

        Catch ex As Exception
            Dim dbTaskLog As TaskLogRow = Nothing
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "CPModuleTasks"
            dbTaskLog.Status = "Error"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()

            Logger.Error(Logger.GetErrorMessage(ex))
        Finally
            If Not DB Is Nothing Then DB.Close()
        End Try

    End Sub

End Module
