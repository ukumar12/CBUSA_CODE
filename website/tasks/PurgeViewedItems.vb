Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager

Public Class PurgeViewedItems

    Public Shared Sub Run(ByVal DB As Database)
        Console.WriteLine("Running PurgeViewedItems ... ")

        Dim NumOfRecords As String = AppSettings("NumOfRecords")
        Dim bExit As Boolean = False
        Try
            If Not IsNumeric(NumOfRecords) OrElse CInt(NumOfRecords) = Nothing Then
                NumOfRecords = "1000"
            End If
            While Not bExit
                Dim RowsAffected As Integer = DB.ExecuteSQL("delete TOP (" & NumOfRecords & ") from StoreRecentlyViewed where createdate <= dateadd(day,-7,getdate())")
                bExit = (0 = RowsAffected)
                System.Threading.Thread.Sleep(1000)
            End While
        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
        End Try
    End Sub
End Class
