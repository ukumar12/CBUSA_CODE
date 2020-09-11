Imports Components
Imports DataLayer
Imports System.IO
Imports System.Net.Mail
Imports Newtonsoft.Json.Linq
Imports System.Threading
Imports TwoPrice.DataLayer
Imports System.Configuration.ConfigurationManager
Imports MedullusSendGridEmailLib
Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports Utilities

Partial Class ResendEmailFromLog
    Inherits System.Web.UI.Page

    Private Sub btnResend_Click(sender As Object, e As EventArgs) Handles btnResend.Click

        Dim ResDb As New Database
        Dim dt As New DataTable

        Try
            Dim dr As SqlDataReader

            ResDb.Open(AppSettings("ConnectionString"))

            Dim EmailLogId As String = Me.txtEmailLogID.Text
            'dr = ResDb.GetReader("SELECT * FROM EmailLog WHERE EmailLogID = " & EmailLogId)
            dr = ResDb.GetReader("SELECT * FROM EmailLog WHERE emailsentstatus = 9")

            If dr.HasRows Then
                While dr.Read()
                    Dim ToName As String = dr.Item("RecipientName")
                    Dim ToEmail As String = dr.Item("RecipientEmail")
                    Dim Subject As String = dr.Item("MessageSubject")
                    Dim MessageBody As String = dr.Item("MessageBody")

                    Core.SendHTMLMail("customerservice@cbusa.us", "CBUSA Customer Service", ToEmail, ToName, Subject, MessageBody)
                End While
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            ResDb.Close()
        End Try

    End Sub

End Class
