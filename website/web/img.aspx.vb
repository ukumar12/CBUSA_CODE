Imports Components
Imports System.IO
Imports DataLayer

Public Class img
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'We are creating a separate list for each email, therefore LIST_ID and USER_ID are unique
        '%%userid_%%
        '%%merge lists_.ListId_%%
        Try
            Dim ListId As Integer = Request("lid")
            Dim MemberId As Integer = Request("mid")

            SQL = "SELECT MessageId FROM MailingMessage WHERE (ListHTMLId = " & ListId & " or ListTextId = " & ListId & ")"
            Dim MessageId As Integer = DB.ExecuteScalar(SQL)

            If Not MessageId = 0 Then
                'Insert hit to database
                SQL = " INSERT INTO MailingMessageOpen (ListId, MessageId, MemberId, CreateDate) VALUES (" _
                  & ListId & "," _
                  & MessageId & "," _
                  & MemberId & "," _
                  & DB.Quote(Now()) _
                  & ")"

                DB.ExecuteSQL(SQL)
            End If
        Catch ex As Exception
        End Try

        Response.Clear()
        Response.Buffer = True
        Response.CacheControl = "public"

        Response.ContentType = "image/gif"
        Response.AddHeader("Content-Disposition", "filename=spacer.gif")
        Response.WriteFile(Server.MapPath("/images/spacer.gif"))
    End Sub
End Class
