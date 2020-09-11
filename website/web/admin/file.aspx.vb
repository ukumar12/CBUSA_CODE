Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager
Imports System.IO
Imports System.Net

Partial Class file
    Inherits AdminPage

    Private FileId As Integer
    Private path As String

    'Private dbStatement As StatementRow

    Private ReadOnly Property SitePage() As SitePage
        Get
            If TypeOf Page Is SitePage Then
                Return Page
            Else
                Return New SitePage
            End If
        End Get
    End Property

    Protected Overloads Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        CheckAccess("BUILDERS")


        path = SysParam.GetValue(DB, "StatementsFilePath")

        'Try
        FileId = CInt(Request.QueryString("Id"))
        Dim dt As DataTable = DB.GetDataTable("SELECT * FROM statement WHERE StatementId = " & DB.Number(FileId))

        Dim FilenameParts = dt.Rows(0).Item("FileName").Split("_")


        Dim FILE As String
        Dim sMIME As String
        FILE = path & dt.Rows(0).Item("FileName")
        sMIME = "application/pdf"

        Dim client As New WebClient()
        Dim buffer As [Byte]()

        'client.Credentials = New NetworkCredential("ameagle", "design")
        buffer = client.DownloadData(FILE)

        Response.ContentType = sMIME

        Response.AddHeader("Content-Disposition", "attachment;   filename=" & Server.UrlEncode(dt.Rows(0).Item("FileName")))
        Response.AddHeader("content-length", buffer.Length.ToString())

        Response.BinaryWrite(buffer)
        Response.Flush()


        'Catch ex As Exception
        '    Logger.Error(Logger.GetErrorMessage(ex))
        'End Try
    End Sub

End Class
