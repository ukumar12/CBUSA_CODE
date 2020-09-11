Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager

Public Class RebateNotification
    Private Shared GlobalRefererName As String = AppSettings("GlobalRefererName")
    Private Shared GlobalSecureName As String = AppSettings("GlobalSecureName")
    Public Shared Sub Run(ByVal DB As Database) '

        Console.WriteLine("Running BuilderRebateNotification ... ")
        Dim dbTaskLog As TaskLogRow = Nothing
        Dim AccDB As New Database
        Dim IsActiveRebatesTask As Boolean = SysParam.GetValue(DB, "ActivateRebatesTask")

        Try
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "BuilderRebateNotification"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()
            If Not IsActiveRebatesTask Then
                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "BuilderRebateNotification"
                dbTaskLog.Status = "Skipped"
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "Script turned off. "
                dbTaskLog.Insert()
                Exit Sub
            End If

            AccDB.Open(DBConnectionString.GetConnectionString(AppSettings("ResgroupConnectionString"), AppSettings("ResgroupConnectionStringUsername"), AppSettings("ResgroupConnectionStringPassword")))
            Dim sql As String = "SELECT DISTINCT(BLDRID) FROM RG_ARReport WHERE dayspastdue >= 30 "
            Dim dtAECReport As DataTable = AccDB.GetDataTable(sql)

            Dim cnt As Integer = 0

            For Each row As DataRow In dtAECReport.Rows
                Try

                    'DB.BeginTransaction()
                    Dim sBuilderId As String
                    sBuilderId = Core.GetString(row("BLDRID"))

                    Dim dtBuilder As DataTable = DB.GetDataTable("SELECT CompanyName, Email,BuilderID FROM Builder WHERE  HistoricId = " & sBuilderId)
                    'Sometimes Builder with HistoricId exists in Resdatabase but not AE database
                    If dtBuilder.Rows.Count > 0 Then

                        Dim sBuilderCompany, sBuilderEmail As String
                        sBuilderCompany = Core.GetString(dtBuilder.Rows(0)("CompanyName"))
                        sBuilderEmail = Core.GetString(dtBuilder.Rows(0)("Email"))
                        cnt += 1
                        Dim BuilderAccountPrimaryEmailaddr As String = String.Empty
                        Dim Url As String = IIf(AppSettings("GlobalSecureName") = Nothing, AppSettings("GlobalRefererName"), AppSettings("GlobalSecureName")) & "/Email/RebateNotice.aspx?BuilderId=" & sBuilderId

                        Dim sMsg As String = Core.GetRenderedHtml(Url)
                        sMsg = Replace(sMsg, "%%BUILDER_COMPANY%%", sBuilderCompany)
                        sMsg = Replace(sMsg, "%%Rebate_Report%%", IIf(GlobalSecureName <> Nothing, GlobalSecureName, GlobalRefererName) & "/rebates/Rebate-Notification.aspx")
                        Dim sSubject As String = "Past Due CBUSA Rebates"
                        Dim addr As String = SysParam.GetValue(DB, "RebateNotificationList")
                        Dim addrCC As String = SysParam.GetValue(DB, "RebateNotificationListCC")
                        BuilderAccountPrimaryEmailaddr = DB.ExecuteScalar("select CAST(stuff((SELECT ',' + t1.Email FROM BuilderAccount t1 WHERE  t1.Email is Not null and  t1.IsActive = 1 and t1.IsPrimary = 1  and t1.builderid = t2.builderid FOR XML PATH('')),1,1,'') as varchar(max))   from builder t2 where t2.builderID = " & Core.GetInt(dtBuilder.Rows(0)("BuilderID")))

                        If String.IsNullOrEmpty(BuilderAccountPrimaryEmailaddr) Then
                            Core.SendHTMLMail("customerservice@cbusa.us", "CBUSA", sBuilderEmail, sBuilderCompany, sSubject, sMsg, addr & IIf(addrCC <> String.Empty, "," & addrCC, ""))
                        Else
                            Core.SendHTMLMailToMultipleRecipient("customerservice@cbusa.us", "CBUSA", BuilderAccountPrimaryEmailaddr, BuilderAccountPrimaryEmailaddr, sSubject, sMsg, addr & IIf(addrCC <> String.Empty, "," & addrCC, ""))
                        End If

                        Console.WriteLine("Processing builder " & cnt & " of " & dtAECReport.Rows.Count & " (" & Now() & ")")

                    End If

                    'DB.CommitTransaction()
                Catch ex As Exception
                    If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                    Console.WriteLine(ex)
                    Logger.Error(Logger.GetErrorMessage(ex))
                End Try
            Next
            ' TODO
            Console.WriteLine(cnt & " BuilderRebateNotification Sent")
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "BuilderRebateNotification"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = cnt & " BuilderRebateNotification Sent"
            dbTaskLog.Insert()

        Catch ex As Exception
            ' TODO: 

            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "BuilderRebateNotification"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        Finally
            AccDB.Close()
            DB.Close()

        End Try
    End Sub
End Class
