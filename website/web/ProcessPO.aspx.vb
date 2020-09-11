Imports System
Imports System.Collections.Generic
Imports System.IO

Partial Class ProcessPO
    Inherits System.Web.UI.Page

    Private argSheetId As String
    Private argDate As String
    Private argTime As String
    Private argDropName As String
    Private argProjectId As String

    Private Sub InitiatePOProcessing()

        Dim objProcess As New System.Diagnostics.Process()

        objProcess.StartInfo.FileName = "cmd"
        objProcess.StartInfo.WorkingDirectory = "C:\VPM\XMLConverter"
        'objProcess.StartInfo.WorkingDirectory = "E:\APALA_WORKSPACE\CBUSA\VPM\TestConsoleApp\XMLConverter\bin\Debug"
        objProcess.StartInfo.FileName = "XMLConverter.exe"
        objProcess.StartInfo.Arguments = String.Concat(hdnSheetId.Value, " ", hdnDate.Value, " ", hdnTime.Value, " ", hdnDropName.Value, " ", hdnProjectId.Value)
        objProcess.Start()

    End Sub

    Private Sub btnProcess_Click(sender As Object, e As EventArgs) Handles btnProcess.Click
        hdnIsPagePostback.Value = "True"
        'InitiatePOProcessing()

        divConfirm.Style.Add("display", "none")
        divSubmit.Style.Add("display", "block")
    End Sub

    Private Sub ProcessPO_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then
            argSheetId = Request.QueryString("sheetid").Trim()
            argDate = Request.QueryString("date").Trim()
            argTime = Request.QueryString("time").Trim()
            argDropName = Request.QueryString("dropname").Trim()
            argProjectId = Request.QueryString("projectid").Trim()

            hdnSheetId.Value = argSheetId
            hdnDate.Value = argDate
            hdnTime.Value = argTime
            hdnDropName.Value = """" & argDropName & """"
            hdnProjectId.Value = argProjectId

            spnDropName.InnerText = argDropName
            spnDeliveryDate.InnerText = CDate(argDate).ToLongDateString()
            spnDeliveryTime.InnerText = CDate(argTime).ToLongTimeString()
        End If

    End Sub

End Class
