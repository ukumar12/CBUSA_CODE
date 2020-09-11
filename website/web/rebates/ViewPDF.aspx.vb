Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.IO

Imports System.Configuration.ConfigurationManager
Imports System.Linq
Imports System.Security.Policy
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Net
Imports System.Diagnostics
Imports System.Security
Imports System.IO.Compression

Partial Class admin_statements_ViewPDF
    Inherits System.Web.UI.Page

    Private _AuditTrailID As Integer = 0
    Private _ProjectName As String = "CBUSA_Legacy Application"
    Private _OperationDate As DateTime = DateAndTime.Now()
    Private _ModuleName As String = "Rebate Document"
    Private _PageURL As String = ""
    Private _CurrentUserId As String = ""
    Private _OperationType As String = ""
    Private _ColumnName As String = ""
    Private _OldValue As String = ""
    Private _NewValue As String = ""
    Private StorageAccount As String = "UseDevelopmentStorage = True;"
    Private FileName As String = ""
    Private UserName As String = ""
    Public Const HtmlNewLine As String = "<br />"

    Private Sub form1_Load(sender As Object, e As EventArgs) Handles form1.Load

        Dim InvoiceNo As String = Request.QueryString("invoice")
        OpenPDF(InvoiceNo)

    End Sub

    Private Sub OpenPDF(ByVal InvoiceNo As String)
        Dim InvoicePDFPath As String = AppSettings("DestinationFilePath")

        Dim File As System.IO.FileInfo = New System.IO.FileInfo(InvoicePDFPath & "\" & InvoiceNo & ".pdf")

        If File.Exists Then
            Dim client As New WebClient()

            Dim buffer As [Byte]() = client.DownloadData(InvoicePDFPath & "\" & InvoiceNo & ".pdf")

            If buffer IsNot Nothing Then
                Response.ContentType = "application/pdf"
                Response.AddHeader("content-length", buffer.Length.ToString())
                Response.BinaryWrite(buffer)
            End If

            _OperationType = "View pdf File"
            _CurrentUserId = Session("AdminId")
            UserName = Session("Username")
            _PageURL = Request.Url.ToString()

            Dim Obj As LogAudit = New LogAudit(_AuditTrailID, _ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, "Insert", StorageAccount, FileName, UserName)
            Dim Result = Obj.CallAuditAzureFunction(_ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, FileName, UserName)

        Else
            ErrorMessage("This file does not exist - " & InvoicePDFPath & "\" & InvoiceNo & ".pdf")
        End If
    End Sub

    Public Sub ErrorMessage(ByVal Message As String)
        Dim Msg As String = Message
        Dim sb As New System.Text.StringBuilder()
        sb.Append("<script type = 'text/javascript'>")
        sb.Append("window.onload=function(){")
        sb.Append("alert('")
        sb.Append(Msg)
        sb.Append("')};")
        sb.Append("</script>")
        ClientScript.RegisterClientScriptBlock(Me.GetType(), "alert", sb.ToString())
    End Sub

End Class
