Imports Components
Imports DataLayer

Public Class PurgeCCInfo

    Private Const NofDays As Integer = 7
    Public Shared Sub Run(ByVal DB As Database)
        Dim dbTaskLog As TaskLogRow = Nothing
        Try
            Console.WriteLine("Running PurgeCCInfo ... ")
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "PurgeCCInfo"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()
            Dim SQL As String = "select BuilderRegistrationPaymentID from BuilderRegistrationPayment where ExpirationDate Is Not Null And dateadd(d," & NofDays & ",Submitted) <" & DB.Quote(Now)
            Dim dt As DataTable = DB.GetDataTable(SQL)
            For Each row As DataRow In dt.Rows
                Dim dbPayment As BuilderRegistrationPaymentRow = BuilderRegistrationPaymentRow.GetRow(DB, row("BuilderRegistrationPaymentID"))
                'dbPayment.CardholderName = Nothing
                dbPayment.CardTypeID = Nothing
                dbPayment.ExpirationDate = Nothing
                dbPayment.CIDNumber = Nothing
                dbPayment.CardNumber = Right(Utility.Crypt.DecryptTripleDes(dbPayment.CardNumber), 4).PadLeft(16, "*")
                dbPayment.Update()
            Next
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "PurgeCCInfo"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()
        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "PurgeCCInfo"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        End Try
    End Sub
End Class
