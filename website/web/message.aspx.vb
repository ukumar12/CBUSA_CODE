Imports Components
Imports System.IO
Imports DataLayer
Imports System.Data.SqlClient

Public Class Message
    Inherits BasePage

    Protected HTML As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ListId As Integer = Request("lid")
        Dim MemberId As Integer = Request("mid")
        Dim Email As String = Request("e")

        'We are creating a separate list for each email, therefore LIST_ID and USER_ID are unique
        '%%userid_%%
        '%%merge lists_.ListId_%%

        Try
            Dim Text As String = DB.ExecuteScalar("SELECT TOP 1 MessageText FROM MailingMessage WHERE ListTextId = " & ListId)
            HTML = DB.ExecuteScalar("SELECT TOP 1 MessageHTML FROM MailingMessage WHERE ListHTMLId = " & ListId)
            If HTML = String.Empty Then
                HTML = "<pre>" & Text & "</pre>"
            End If

            'DON'T DISPLAY BROWSER_LINK HERE
            HTML = Replace(HTML, Replace(System.Configuration.ConfigurationManager.AppSettings("LyrisBrowserLinkHTML"), "{LINK}", System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/message.aspx?e=%%EmailAddr_%%&mid=%%userid_%%&lid=%%merge lists_.ListId_%%"), "")
            HTML = Replace(HTML, Replace(System.Configuration.ConfigurationManager.AppSettings("LyrisBrowserLinkText"), "{LINK}", System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/message.aspx?e=%%EmailAddr_%%&mid=%%userid_%%&lid=%%merge lists_.ListId_%%"), "")

            'REPLACE MERGE TAGS
            HTML = Replace(HTML, "%%userid_%%", MemberId)
            HTML = Replace(HTML, "%%merge lists_.ListId_%%", ListId)
            HTML = Replace(HTML, "%%EmailAddr_%%", Email)

        Catch ex As Exception
        End Try
    End Sub
End Class
