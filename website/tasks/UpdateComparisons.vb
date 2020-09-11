Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager
Imports System.Net

Public Class UpdateComparisons

    Public Shared Sub Run(ByVal DB As Database)
        Console.WriteLine("Running UpdateComparisons ... ")
        Dim dbTaskLog As TaskLogRow = Nothing

        Try
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "UpdateComparisons"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()

            Dim sql As String = "select DateDiff(dd, pc.ModifyDate, GetDate()) As Days, pc.* from PriceComparison pc Inner Join Builder b On pc.BuilderId = b.BuilderId where b.IsActive = 1 And pc.IsAdminComparison = 1 Or pc.IsDashboard = 1 And (pc.ModifyDate Is Null Or DateDiff(dd, pc.ModifyDate, GetDate()) > 0)"
            Dim dt As DataTable = DB.GetDataTable(sql)
            Dim Count As Integer = 0
            Dim BadCount As Integer = 0
            Dim GoodCount As Integer = 0
            For Each row As DataRow In dt.Rows
                Try
                    Count += 1
                    Console.WriteLine("Processing row " & Count & " of " & dt.Rows.Count)
                    Dim request As WebRequest = WebRequest.Create(AppSettings("GlobalRefererName") & "/comparison/default.aspx?IsUpdate=Y&BuilderId=" & row("BuilderId") & "&PriceComparisonID=" & row("PriceComparisonID"))
                    request.Method = "GET"
                    request.Timeout = 45 * 1000
                    Dim response As WebResponse = request.GetResponse()
                    response.Close()
                    GoodCount += 1
                Catch ex As Exception
                    BadCount += 1
                    Console.WriteLine("Error row " & Count & " of " & dt.Rows.Count)
                    Console.WriteLine("----BuilderId: " & Core.GetInt(row("BuilderId")))
                    Console.WriteLine("----PriceComparisnId: " & Core.GetInt(row("PriceComparisonID")))
                    Console.WriteLine("----Error: " & ex.Message)
                End Try
            Next

            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "UpdateComparisons"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = "Good: " & GoodCount & "<br>Bad: " & BadCount & "<br>Total: " & Count
            dbTaskLog.Insert()
        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "UpdateComparisons"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        End Try
    End Sub
End Class
