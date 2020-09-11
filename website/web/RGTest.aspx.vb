Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components
Imports Utilities
Imports System.Configuration.ConfigurationManager

Partial Class RGTest
    Inherits System.Web.UI.Page

    Private Sub form1_Load(sender As Object, e As EventArgs) Handles form1.Load

        Dim dtBuilderStmtBeginBalance As DataTable = Nothing
        dtBuilderStmtBeginBalance = GetDataTableForPrograms("RG_BuilderStmtBeginBalance", "5137", Convert.ToDateTime("01-01-2017").Date)

    End Sub

    Private Sub btnGenerateReport_Click(sender As Object, e As EventArgs) Handles btnGenerateReport.Click

        Dim dtBuilderStmtBeginBalance As DataTable = Nothing
        dtBuilderStmtBeginBalance = GetDataTableForPrograms("RG_BuilderStmtBeginBalance", "5137", Convert.ToDateTime(txtDateFrom.Text).Date)

    End Sub

    Private Function GetDataTableForPrograms(ByVal StoredProcedureName As String, ByVal BuilderID As Integer, ByVal DateFrom As Date) As DataTable
        Dim ResDb As New Database
        Dim dt As New DataTable
        Dim prams(1) As SqlParameter
        prams(0) = New SqlParameter("@BUILDERID", BuilderID)
        prams(1) = New SqlParameter("@DATEFROM ", DateFrom)

        Try
            ResDb.Open(DBConnectionString.GetConnectionString(AppSettings("ResgroupConnectionString"), AppSettings("ResgroupConnectionStringUsername"), AppSettings("ResgroupConnectionStringPassword")))
            ResDb.RunProc(StoredProcedureName, prams, dt)
            Return dt
        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            ResDb.Close()
        End Try

        Return Nothing

    End Function

End Class
