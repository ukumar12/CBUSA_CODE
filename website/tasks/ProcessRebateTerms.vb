Imports Components
Imports DataLayer

Public Class ProcessRebateTerms

    Public Shared Sub Run(ByVal DB As Database)
        Console.WriteLine("Running ProcessRebateTerms ... ")
        Dim currentQtr As Integer = Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
        Dim lastQtr As Integer = IIf(currentQtr = 1, 4, currentQtr - 1)
        Dim lastYear As Integer = IIf(lastQtr = 4, Now.Year - 1, Now.Year)
        
        Dim dbTaskLog As TaskLogRow = Nothing
        Try
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "ProcessRebateTerms"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()

            'All Vendors that haven not reported
            Dim sql As String = "Select VendorId From Vendor Where IsActive = 1 And VendorId Not In (Select VendorId From RebateTerm Where StartYear = " & DB.Number(Now.Year) & " And StartQuarter = " & DB.Number(currentQtr) & ")" 

            Dim dtVendors As DataTable = DB.GetDataTable(sql)

            Dim cnt As Integer = 0

            For Each row As DataRow In dtVendors.Rows
                Try
                    DB.BeginTransaction()

                    cnt += 1

                    Console.WriteLine("Processing member " & cnt & " of " & dtVendors.Rows.Count & " (" & Now() & ")")

                    Dim dbLastTerm As RebateTermRow = RebateTermRow.GetRowByVendor(DB, row("VendorId"), lastYear, lastQtr)
                    
                    Dim dbTerm As RebateTermRow = New RebateTermRow(DB)
                    dbTerm = New RebateTermRow(DB)
                    dbTerm.PurchaseRangeCeiling = 999999999
                    dbTerm.PurchaseRangeFloor = 0
                    If dbLastTerm.RebateTermsID > 0 Then
                        dbTerm.RebatePercentage = dbLastTerm.RebatePercentage
                    Else
                        Dim LLCRebate As Double = DB.ExecuteScalar("select Top 1 DefaultRebate from LLC where LLCID In (select LLCId from LLCVendor where VendorId = " & DB.Number(row("VendorId")) & ")")
                        If LLCRebate <> Nothing Then
                            dbTerm.RebatePercentage = LLCRebate
                        Else
                            dbTerm.RebatePercentage = 1
                        End If
                    End If
                    dbTerm.StartQuarter = currentQtr
                    dbTerm.StartYear = Now.Year
                    dbTerm.IsAnnualPurchaseRange = False
                    dbTerm.VendorID = row("VendorId")
                    dbTerm.CreatorVendorAccountID = 1
                    dbTerm.Insert()

                    DB.CommitTransaction()
                Catch ex As Exception
                    If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                    Logger.Error(Logger.GetErrorMessage(ex))
                End Try
            Next
            Console.WriteLine(cnt & " rebate terms created")
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "ProcessRebateTerms"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = cnt & " rebate terms added"
            dbTaskLog.Insert()

        Catch ex As Exception
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "ProcessRebateTerms"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        End Try
    End Sub
End Class
