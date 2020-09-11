Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager
Imports System.IO
Imports System.Net

Partial Class file
    Inherits SitePage

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

        Dim RedirectUrl As String = "/"

        path = SysParam.GetValue(DB, "StatementsFilePath")

        Try
            FileId = CInt(Request.QueryString("Id"))
            Dim dt As DataTable = DB.GetDataTable("SELECT * FROM statement WHERE StatementId = " & DB.Number(FileId))
            
            'dbStatement = StatementRow.GetRow(DB, FileId)




            If Session("BuilderId") <> 0 Then
                Dim bHasAccess As Boolean = False


                Dim FilenameParts = dt.Rows(0).Item("FileName").Split("_")

                Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
                If dbBuilder.HistoricID = dt.Rows(0).Item("HistoricID") Then
                    Dim FILE As String
                    Dim sMIME As String
                    FILE = path & dt.Rows(0).Item("FileName")
                    sMIME = "application/pdf"

                    Dim client As New WebClient()
                    Dim buffer As [Byte]()

                    client.Credentials = New NetworkCredential("ameagle", "design")
                    buffer = client.DownloadData(FILE)

                    Response.ContentType = sMIME

                    Response.AddHeader("Content-Disposition", "attachment;   filename=" & Server.UrlEncode(dt.Rows(0).Item("FileName")))
                    Response.AddHeader("content-length", buffer.Length.ToString())

                    Response.BinaryWrite(buffer)
                    Response.Flush()
                Else
                    Response.Redirect(RedirectUrl)
                End If
            Else
                Response.Redirect(RedirectUrl & "?redir=file.aspx%3FId=" & FileId)
            End If
        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
            Response.Redirect(RedirectUrl & "?redir=file.aspx%3FId=" & FileId)
        End Try
    End Sub

End Class
