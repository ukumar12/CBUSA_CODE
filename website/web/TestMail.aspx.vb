Imports Components
Imports DataLayer
Imports System.IO
Imports System.Net.Mail
Imports Newtonsoft.Json.Linq
Imports System.Threading
Imports TwoPrice.DataLayer
Imports System.Configuration.ConfigurationManager
Imports MedullusSendGridEmailLib

Partial Class TestMail
    Inherits SitePage

    Private Sub btnSendEmail_Click(sender As Object, e As EventArgs) Handles btnSendEmail.Click
        Try
            Dim ebody As String = "One November 15 ,We plan to invite vendors to submit bids for the Developed By Debashis " &
                                  "( October  to  December )  .<br /><br /><p>Currently we plan to request bids " &
                                  "for the following items in this committed buy<br /><br /></p><ul><li>Material1 " &
                                  "</li><li>Material2<br />Material3</li><li>Material4</li><li>" &
                                  "if you have aditional items in this general category that you would Like us to add to the bid request ," &
                                  "please let us know.</li></ul><br /><br />You have until Thursday, November 8, 2018 " &
                                  "to enroll in this committed purchase .If you do Not opt in by then,you will Not recive the specila " &
                                  "CBUSA pricing on materials decribed above.<br /><br />would Like to participate in the " &
                                  "Developed By Debashis ( October  to  December )  committed purchase event?<br /><br />&nbsp; " &
                                  "&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; " &
                                  "&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;" &
                                  "&nbsp;<a style='padding: 2px 4px; border: 1px solid #282927; font-size:12px; line-height:12px;font-weight:400; color:#FFF; " &
                                  "background:#38761d;text-decoration:none;' href='https://www.google.com/'>I want to participate in this Commited Buy</a>&nbsp; " &
                                  "&nbsp; &nbsp; &nbsp; &nbsp;<a style=""padding:2px 4px; border:1px solid #282927; font-size:12px; line-height:12px;font-weight:400; color:#FFF; background:#cc0000; text-decoration:none;"" " &
                                  "href=""test.aspx?id=124"">""Thanks, But I'll Pass</a><br /><br />If you have any question about this committed buy ,you can contact Priyaja Test via e-mail pdas@medullus.com or by phone at <br />" &
                                  "<br />committed purchase Event Terms and condition:"

            '   Dim ebody As String = "<p>This is a test paragraph.</p><br /><br /> This is the first line of a second paragraph. This is a <a href=""www.google.com"">test link </a>."

            Dim msgFrom As MailAddress = New MailAddress("pdas@medullus.com", "P das")
            Dim msgTo As MailAddress = New MailAddress("abasu@medullus.com", "A das")
            Dim msg As MailMessage = New MailMessage(msgFrom, msgTo)
            msg.IsBodyHtml = False
            msg.Subject = "SendGrid Test email"
            msg.Body = ebody

            Dim sb As New StringBuilder()
            dvEventDetails.RenderControl(New HtmlTextWriter(New StringWriter(sb)))
            Dim MailBody As String = System.Net.WebUtility.HtmlDecode(sb.ToString())

            SendMail(msg, "", "")

        Catch ex As Exception
            Response.Write(ex.Message.ToString())
        End Try
    End Sub

    Public Sub SendMail(ByVal msg As MailMessage, Optional ByVal AttachmentList As String = "", Optional ByVal AttachmentFilePathToRead As String = "")
            Dim ClientId As Int32 = 1
            Dim APIKey As String = System.Configuration.ConfigurationManager.AppSettings("ApiKey").ToString()

        If Not msg.IsBodyHtml Then

            If Not msg.Body.Contains("<body") Or Not msg.Body.Contains("<Body") Or Not msg.Body.Contains("<BODY") Then
                Dim HtmlTemplate As String
                '  HtmlTemplate = CStr(HttpContext.Current.Cache.Get("HtmlTemplate"))

                If HttpContext.Current Is Nothing Then
                    HtmlTemplate = CStr(HttpRuntime.Cache.Get("HtmlTemplate"))
                Else
                    HtmlTemplate = CStr(HttpContext.Current.Cache.Get("HtmlTemplate"))
                End If

                If HtmlTemplate Is Nothing Or HtmlTemplate = String.Empty Then
                    Dim GlobalRefererName As String = System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName")
                    HtmlTemplate = Core.GetRenderedHtml(GlobalRefererName & "/emailtemplate.aspx")
                    Try
                        If HttpContext.Current Is Nothing Then
                            HttpRuntime.Cache.Insert("HtmlTemplate", HtmlTemplate)
                        Else
                            HttpContext.Current.Cache.Insert("HtmlTemplate", HtmlTemplate)
                        End If
                    Catch ex As Exception
                    End Try
                End If
                Dim newBody As String = Replace(HtmlTemplate, "%%sBody%%", Core.Text2HTML(msg.Body))
                msg.Body = newBody
                msg.IsBodyHtml = True
            End If

        End If

        Try
                Dim EmailDetails As String = Newtonsoft.Json.JsonConvert.SerializeObject(New With {Key .EmailReceiverId = msg.To.ToString()})
                Dim header = Newtonsoft.Json.JsonConvert.SerializeObject(
                                New With
                                {
                                    Key .emailHeader = New With
                                    {
                                        Key .EmailSenderId = msg.From.ToString(),
                                        Key .EmailSubject = msg.Subject.ToString(),
                                        Key .EmailBody = "",
                                        Key .EmailHTMLBody = msg.Body.ToString(),
                                        Key .ClientId = ClientId,
                                        Key .EmailSenderPasswd = "",
                                        Key .APIKey = APIKey
                                }, Key .emailDetails = {Newtonsoft.Json.JsonConvert.DeserializeObject(EmailDetails)}})

            Dim Parameters As JObject = JObject.Parse("" & header)

            Dim sendMail As MedullusSendGridEmailLib.MedullusSendGridEmailLib
            sendMail = New MedullusSendGridEmailLib.MedullusSendGridEmailLib()

            Dim Result As String = sendMail.SendStaticEmailBySendGrid(Parameters, "", "")

            Response.Write(Result)

        Catch ex As Exception
            Response.Write(ex.Message())
        End Try

        End Sub

    Protected Sub RblPortfolio_CheckedChanged(sender As Object, e As EventArgs)

        Response.Write("portfolio")
        Response.End()

    End Sub

    Protected Sub RblCustom_CheckedChanged(sender As Object, e As EventArgs) Handles RblCustom.CheckedChanged

        Response.Write("custom")
        Response.End()
    End Sub
End Class
