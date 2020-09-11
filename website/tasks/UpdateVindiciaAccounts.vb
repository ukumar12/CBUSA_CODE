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


Public Class UpdateVindiciaAccounts

    Public Shared Sub Run(ByVal DB As Database)
        Console.WriteLine("Running UpdateVindiciaAccounts ... ")

        Dim dtBuilders As DataTable = DB.GetDataTable("Select * from Builder")
       Dim p As New VindiciaPaymentProcessor(DB)
        p.IsTestMode = DataLayer.SysParam.GetValue(DB, "TestMode")
        For Each builder As DataRow In dtBuilders.Rows
            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, builder("BuilderID"))
            If p.UpdateVindiciaAddress(dbBuilder) Then
                Console.WriteLine("Vindicia Updated : " & dbBuilder.BuilderID & "" & dbBuilder.HistoricID)
            End If



        Next


    End Sub
End Class
