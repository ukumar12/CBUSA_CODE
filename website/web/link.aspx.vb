Imports Components
Imports System.IO
Imports DataLayer
Imports System.Data.SqlClient

Public Class Link
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim Link As String = Nothing

        'We are creating a separate list for each email, therefore LIST_ID and USER_ID are unique
        '%%userid_%%
        '%%merge lists_.ListId_%%
        Try
            Dim ListId As Integer = Request("lid")
            Dim MemberId As Integer = Request("mid")
            Dim LinkId As Integer = Request("l")

            Dim MessageId As Integer = Nothing

            SQL = "SELECT MessageId, Link FROM MailingLink WHERE MessageId IN (SELECT MessageId FROM MailingMessage WHERE (ListHTMLId = " & ListId & " or ListTextId = " & ListId & ") AND LinkId = " & LinkId & ")"
            Dim dr As SqlDataReader = DB.GetReader(SQL)
            If dr.Read Then
                MessageId = dr("MessageId")
                Link = dr("Link")
            End If
            dr.Close()

            If Not MessageId = 0 Then
                'Insert hit to database
                SQL = " INSERT INTO MailingLinkHit (MessageId, LinkId, MemberId, CreateDate) VALUES (" _
                  & MessageId & "," _
                  & LinkId & "," _
                  & MemberId & "," _
                  & DB.Quote(Now()) _
                  & ")"

                DB.ExecuteSQL(SQL)
            End If
        Catch ex As Exception
        End Try

        If Not Link = String.Empty Then
            Response.Redirect(Server.HTMLDecode(Link))
        Else
            Response.Redirect("/")
        End If
    End Sub
End Class
