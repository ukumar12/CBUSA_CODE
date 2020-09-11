Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager
Imports System.Linq
Imports System.Security.Policy

Public Class VendorTerminationNotification
    Public Shared Sub Run(ByVal DB As Database)
        Console.WriteLine("Running VendorTermination ... ")
        Dim dbTaskLog As TaskLogRow = Nothing
        Dim AccDB As New Database
        Dim IsActiveRebatesTask As Boolean = SysParam.GetValue(DB, "ActivateRebatesTask")
        Try
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "VendorTermination"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()

            If Not IsActiveRebatesTask Then
                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "VendorTermination"
                dbTaskLog.Status = "Skipped"
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "Script turned off. "
                dbTaskLog.Insert()
                Exit Sub
            End If


            AccDB.Open(DBConnectionString.GetConnectionString(AppSettings("ResgroupConnectionString"), AppSettings("ResgroupConnectionStringUsername"), AppSettings("ResgroupConnectionStringPassword")))
            Dim dtAECReport As DataTable = AccDB.GetDataTable("SELECT   Distinct(VNDRID)  FROM RG_ARReport where  DaysPastDue = 170")

            'Dim dtAECReport As DataTable = AccDB.GetDataTable(sql)

            Dim cnt As Integer = 0

            For Each row As DataRow In dtAECReport.Rows
                Try
                    'DB.BeginTransaction()

                    Dim sBuilderId, sBuilderCompany, sHistoricVendorId, sVendorCompany, sVendorEmail As String
                    sHistoricVendorId = Core.GetString(row("VNDRID")).Trim
                    ' sBuilderId = Core.GetString(row("BLDRID"))
                    Dim dtExVendors As DataTable = DB.GetDataTable("SELECT DISTINCT(VendorId) FROM Vendor WHERE excludedVendor =1 AND VendorId = " & sHistoricVendorId)
                    If (dtExVendors.Rows.Count > 0) Then
                        Continue For
                    End If
                    Dim sHistoricIdGroups As String = AccDB.ExecuteScalar("select CAST(stuff((SELECT  distinct  ',' + t1.BLDRID     FROM RG_ARReport t1 where t1.VNDRID  =" & sHistoricVendorId & "FOR XML PATH('')),1,1,'') as varchar(max))  ")

                    Dim dtBuilderGroups As DataTable = DB.GetDataTable("Select distinct(BuilderGroup) AS BuilderGroup, OperationsManager,NotificationEmailList  from LLC l inner join Builder b on l.LLCID=b.LLCID where b.HistoricID in (" & sHistoricIdGroups & ")")

                    For Each drBuilderGroup As DataRow In dtBuilderGroups.Rows
                        Dim builderGroup As String = Core.GetString(drBuilderGroup("BuilderGroup"))
                        Dim OperationsManager As String = Core.GetString(drBuilderGroup("OperationsManager"))
                        Dim NotificationEmailList As String = String.Empty
                        If Not IsDBNull(drBuilderGroup("NotificationEmailList")) Then
                            NotificationEmailList = Core.GetString(drBuilderGroup("NotificationEmailList"))
                        End If
                        Dim HistoricBuilderIdByGroup As String = DB.ExecuteScalar("select CAST(stuff((SELECT distinct ',' + CAST( b.HistoricId as varchar)  FROM Builder b  WHERE b.LLCID = l.LLCID and b.HistoricID in (" & sHistoricIdGroups & ")  FOR XML PATH('')),1,1,'') as varchar(max)) from LLC l where  l.BuilderGroup = " & DB.Quote(builderGroup))




                        Dim dtVendor As DataTable = DB.GetDataTable("SELECT CompanyName, Email FROM Vendor WHERE HistoricId = " & sHistoricVendorId)
                        If dtVendor.Rows.Count > 0 Then
                            sVendorCompany = Core.GetString(dtVendor.Rows(0)("CompanyName"))
                            sVendorEmail = Core.GetString(dtVendor.Rows(0)("Email"))
                        End If
                        cnt += 1

                        Dim Url As String = IIf(AppSettings("GlobalSecureName") = Nothing, AppSettings("GlobalRefererName"), AppSettings("GlobalSecureName")) & "/Email/vendorterminationnotice.aspx?HistoricVendorId=" & sHistoricVendorId & "&HistoricBuilderIdByGroup=" & HistoricBuilderIdByGroup

                        Dim sVendorId = DB.ExecuteScalar("SELECT vendorid FROM Vendor WHERE historicId=" & sHistoricVendorId)
                        Dim sVendorEmails As String = DB.ExecuteScalar("select CAST(stuff((SELECT ',' + t1.Email FROM VendorAccount t1 WHERE t1.IsActive = 1 AND t1.VendorID = t2.VendorID FOR XML PATH('')),1,1,'') as varchar(max)) + ', ' + t2.Email from Vendor t2 where t2.VendorID = " & sVendorId)

                        Dim sMsg As String = Core.GetRenderedHtml(Url)
                        sMsg = Replace(sMsg, "%%BUILDER_COMPANY%%", sBuilderCompany)
                        sMsg = Replace(sMsg, "%%Operations_manager%%", OperationsManager)
                        sMsg = Replace(sMsg, "%%Builder_Group%%", builderGroup)
                        Dim sSubject As String = "Past Due CBUSA Rebates"
                        Dim addr As String = SysParam.GetValue(DB, "RebateNotificationList")
                        Dim addrCC As String = SysParam.GetValue(DB, "RebateNotificationListCC")
                        ' Core.SendHTMLMail("abasu@medullus.com", "CBUSA", sBuilderEmail, sBuilderCompany, sSubject, sMsg)
                        Core.SendHTMLMailToMultipleRecipient("customerservice@cbusa.us", builderGroup, sVendorEmails, sVendorEmails, sSubject, sMsg, IIf(NotificationEmailList = "", "", NotificationEmailList) & IIf(addrCC <> String.Empty, "," & addrCC, ""), addr)
                        Console.WriteLine("Processing builder " & cnt & " of " & dtAECReport.Rows.Count & " (" & Now() & ")")
                    Next
                    'DB.CommitTransaction()
                Catch ex As Exception
                    If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                    Core.SendSimpleMail("customerservice@cbusa.us", "customerservice@cbusa.us", "abasu@medullus.com", "abasu@medullus.com", "Vendor reminder failed", ex.ToString)
                    Logger.Error(Logger.GetErrorMessage(ex))
                End Try
            Next
            ' TODO
            Console.WriteLine(cnt & " VendorTermination Sent")
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "VendorTermination"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = cnt & " VendorTermination Sent"
            dbTaskLog.Insert()

        Catch ex As Exception
            ' TODO: 
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "VendorTermination"
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
