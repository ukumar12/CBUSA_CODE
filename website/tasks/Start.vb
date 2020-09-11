Imports Components
Imports System.Net.Mail
Imports System.Configuration.ConfigurationManager
Imports DataLayer

Module Start

    Sub Main()
        Dim Connectionstring As String = DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword"))
        Dim NCPConnString As String = DBConnectionString.GetConnectionString(AppSettings("NCPConnString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword"))

        Dim DB As New Database
        Dim NCPDB As New Database

        Dim args As String() = {"all"}

        Try
            If My.Application.CommandLineArgs.Count > 0 Then
                ReDim Preserve args(My.Application.CommandLineArgs.Count)
                My.Application.CommandLineArgs.CopyTo(args, 0)
            End If

            DB.Open(Connectionstring)
            NCPDB.Open(NCPConnString)

            For Each arg As String In args
                Dim task As String = LCase(arg)

                'these were not commented out by Ali

                'If task = "purgevieweditems" OrElse task = "all" Then PurgeViewedItems.Run(DB)
                'If task = "sendmemberreminders" OrElse task = "all" Then SendMemberReminders.Run(DB)
                'If task = "sendinventorynotifications" OrElse task = "all" Then SendInventoryNotifications.Run(DB)
                'If task = "archiveincompleteorders" OrElse task = "all" Then ArchiveIncompleteOrders.Run(DB)

                'these were commented out by Ali

                'AR 11/13/2009: Some logging wll need to ba added to these to see what runs and what not.  For not I'm putting try/catch statement to prevent one failed task to affect others.
                'AR 02/12/2010: Finally, this has been implemented. See Taks Logs in the admin.

                '----------------------------------------------------------------------------------------
                'Commented by Apala (Medullus) on 25.08.2017 - as IdevSearch is not used anymore
                'Try
                '    If task = "idevsearch" Then IdevSearch.Run(DB)
                'Catch ex As Exception
                'End Try

                'Try
                '    If task = "idevsearchtrigger" Then IdevSearchTrigger.Run(DB)
                'Catch ex As Exception
                'End Try
                '----------------------------------------------------------------------------------------

                Try
                    If task = "purgeccinfo" OrElse task = "all" Then PurgeCCInfo.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "updateprices" OrElse task = "all" Then UpdatePrices.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "processexportqueue" OrElse task = "all" Then ProcessExportQueue.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "nosalesreportreceived" OrElse task = "all" Then NoSalesReportReceived.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "nodisputeresponsereceived" OrElse task = "all" Then NoDisputeResponseReceived.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "discrepancyreport" OrElse task = "all" Then DiscrepancyReport.Run(DB)
                Catch ex As Exception
                End Try

                'AR 021210: Client request this to be deactivated.
                'Try
                '    If task = "discrepencyresponsedeadlinepassed" OrElse task = "all" Then DiscrepencyResponseDeadlinePassed.Run(DB)
                'Catch ex As Exception
                'End Try

                Try
                    If task = "nopurchasesreportreceived" OrElse task = "all" Then NoPurchasesReportReceived.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "reportingreminders" OrElse task = "all" Then ReportingReminders.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    'Reporting task is only run alone -- not with 'all' arg
                    'AR 08142009: This task has been reviewed and it can now run everyday just like the others. 
                    If task = "reporting" OrElse task = "all" Then Reporting.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "processautodisputes" OrElse task = "all" Then ProcessAutoDisputes.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "processrebateterms" OrElse task = "all" Then ProcessRebateTerms.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "updatecomparisons" Then UpdateComparisons.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "plansonlinevendorentrynotification" Then PlansOnlineVendorEntryNotification.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "bidrequestreminders" Then BidRequestReminders.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "builderrebatenotification" Then RebateNotification.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "twopricebidrequestreminders" Then TwoPriceBidRequestReminders.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "vendortermination" Then VendorTerminationNotification.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "vendorreminder" Then VendorReminderNotice.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "statementnotifier" Then StatementNotifier.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "updatevindiciaaccounts" Then UpdateVindiciaAccounts.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "updateaffiliateid" Then UpdateVindiciaMerchantAffiliateID.Run(DB)
                Catch ex As Exception
                End Try

                'Try
                '    If task = "autoacceptamountsfromdnrvendors" Then AutoAcceptAmountsFromDNRVendors.Run(DB)
                'Catch ex As Exception
                'End Try

                'Added by Apala (Medullus) on 20.10.2017
                Try
                    If task = "ncplegacydatasync" Then LegacyNCPDataSync.Run(NCPDB)
                Catch ex As Exception
                End Try

                Try
                    If task = "cpeventreminder" Then CpEventReminder.Run(DB)
                Catch ex As Exception
                End Try

                Try
                    If task = "salesvolumeupdate" Then SalesVolumeUpdate.Run(DB)
                Catch ex As Exception
                End Try

            Next

        Catch ex As Exception
            Dim dbTaskLog As TaskLogRow = Nothing
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "CpReminderTask"
            dbTaskLog.Status = "Error"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()

            Logger.Error(Logger.GetErrorMessage(ex))
        Finally
            If Not DB Is Nothing Then DB.Close()
            If Not NCPDB Is Nothing Then NCPDB.Close()
            'Console.ReadLine()
        End Try
    End Sub
End Module
