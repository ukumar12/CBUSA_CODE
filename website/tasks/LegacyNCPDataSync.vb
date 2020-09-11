Imports Components

Public Class LegacyNCPDataSync

    Public Shared Sub Run(ByVal DB As Database)
        Try
            Console.WriteLine("Running LegacyNCPDataSync... ")

            DB.ExecuteSQL("EXEC sp_NCPLegacyDataSync")

            Console.WriteLine("Finished LegacyNCPDataSync ... ")

        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
        Finally
            If Not DB Is Nothing Then DB.Close()
        End Try
    End Sub

End Class
