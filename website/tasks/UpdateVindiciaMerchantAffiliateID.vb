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
Imports Vindicia


Public Class UpdateVindiciaMerchantAffiliateID

    Public Shared Sub Run(ByVal DB As Database)
        Console.WriteLine("Running UpdateVindiciaAccounts ... ")
        Dim dbTaskLog As TaskLogRow = Nothing
        Try
        dbTaskLog = New TaskLogRow(DB)
        dbTaskLog.TaskName = "UpdateVindiciaAccounts"
        dbTaskLog.Status = "Started"
        dbTaskLog.LogDate = Now()
        dbTaskLog.Msg = ""
        dbTaskLog.Insert()
        Dim dtBuilders As DataTable = DB.GetDataTable("Select * from Builder  Where IsActive =1 and llcid <> 675 ")
            Dim p As New VindiciaPaymentProcessor(DB)
            Dim cnt As Integer = 0
            p.IsTestMode = DataLayer.SysParam.GetValue(DB, "TestMode")

            For Each builder As DataRow In dtBuilders.Rows

                Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, builder("BuilderID"))
                Dim LLCAffliateID As String = LLCRow.GetRow(DB, dbBuilder.LLCID).AffiliateID.ToString("D3")
                Try
                    p.IsTestMode = SysParam.GetValue(DB, "TestMode")
                    If p.EnsureVindiciaAccount(dbBuilder) Then
                        Console.WriteLine("updating " & dbBuilder.BuilderID)
                        Dim autoBills() As Vindicia.AutoBill = p.GetAutobills(dbBuilder)
                        Console.WriteLine("Got autobills " & dbBuilder.BuilderID)
                        For Each ab As Vindicia.AutoBill In autoBills

                            Try
                                If Not p.UpdateMerchantAffID(ab.VID, dbBuilder, ab, LLCAffliateID) Then
                                    Console.WriteLine("Failed  " & dbBuilder.BuilderID)
                                End If
                            Catch
                                Console.WriteLine("erroring  UpdateMerchantAffID " & dbBuilder.BuilderID)
                            End Try

                        Next

                    End If
                    cnt += 1
                Catch ex As Exception
                    Console.WriteLine("Error  UpdateVindiciaAccounts  . " & dbBuilder.BuilderID)
                End Try
            Next

            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "UpdateVindiciaAccounts"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = cnt & "VindiciaAccounts updated "
            dbTaskLog.Insert()
        Catch ex As Exception
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "UpdateVindiciaAccounts"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        End Try
        Console.WriteLine("Success UpdateVindiciaAccounts ... ")
    End Sub
End Class




