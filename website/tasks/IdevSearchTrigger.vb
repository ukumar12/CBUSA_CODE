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

Public Class IdevSearchTrigger

    Private Shared ConnectionString As String




    Public Shared Sub Run(ByVal DB As Database)
        Console.WriteLine("Running IdevSearch Trigger ... ")

        'if IdevSearchTrigger = true then
        'if IdevSearch last task log = completed then 
        'Run IdevSearch.Run()
        'Set IdevSearchTrigger to False


        Dim IsIdevSearchTrigger As Boolean = SysParam.GetValue(DB, "IdevSearchTrigger")
        Dim IdevSearchTriggerDateTime As DateTime = SysParam.GetValue(DB, "IdevSearchTriggerDateTime")
        Dim IdevSearchstatus As String = DB.ExecuteScalar("SELECT status FROM TaskLog WHERE Logid = (Select MAX(Logid) from TaskLog where TaskName = 'idevsearch')")

        If IsIdevSearchTrigger Then
            If IdevSearchstatus = "Completed" Then

                IdevSearch.Run(DB)

                Dim Sql As String = "Update SysParam Set Value = '0' Where Name = 'IdevSearchTrigger'"
                DB.ExecuteSQL(SQL)
            End If
        End If
        Console.WriteLine("Finished Running IdevSearch Trigger ... ")

    End Sub
End Class


