Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.IO
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager
Imports System.Net
Imports Utility

Partial Class ResponseCaptureView
    Inherits AdminPage

    Private OrderId As Integer

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("ORDERS")

        Response.ClearContent()
        Dim dbResponseCapture As ResponseCaptureRow = ResponseCaptureRow.GetRow(DB, Request("ResponseCaptureId"))
        Response.Write(dbResponseCapture.ResponseCapture)


    End Sub

End Class

