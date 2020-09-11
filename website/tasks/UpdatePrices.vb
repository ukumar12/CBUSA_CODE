Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager

Public Class UpdatePrices

    Public Shared Sub Run(ByVal DB As Database)
        Console.WriteLine("Running UpdatePrices ... ")
        Dim dbTaskLog As TaskLogRow = Nothing

        Try
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "UpdatePrices"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()

            DB.BeginTransaction()

            Dim sql As String = "select * from VendorProductPrice where (Coalesce(IsDiscontinued,0) = 1 or (NextPrice is not null and NextPrice > 0)) and NextPriceApplies <= " & DB.Quote(Now)
            Dim dt As DataTable = DB.GetDataTable(sql)
            For Each row As DataRow In dt.Rows
                Dim dbHistory As New VendorProductPriceHistoryRow(DB)
                dbHistory.IsSubstitution = Core.GetBoolean(row("IsSubstitution"))
                dbHistory.IsUpload = Core.GetBoolean(row("IsUpload"))
                dbHistory.ProductID = Core.GetInt(row("ProductID"))
                dbHistory.SubmitterVendorAccountID = -1
                dbHistory.VendorID = Core.GetInt(row("VendorID"))
                dbHistory.VendorPrice = Core.GetDouble(row("VendorPrice"))
                dbHistory.VendorSKU = Core.GetString(row("VendorSku"))
                dbHistory.Insert()
            Next

            sql = "update VendorProductPrice set IsDiscontinued=0, VendorPrice = NextPrice, NextPrice= null, NextPriceApplies = null where ((NextPrice is not null and NextPrice > 0) or Coalesce(IsDiscontinued,0) = 1) and NextPriceApplies is not null and NextPriceApplies <=" & DB.Quote(Now)
            DB.ExecuteSQL(sql)

            DB.CommitTransaction()

            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "UpdatePrices"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()
        Catch ex As Exception
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then
                DB.RollbackTransaction()
            End If
            Logger.Error(Logger.GetErrorMessage(ex))
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "UpdatePrices"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        End Try
    End Sub
End Class
