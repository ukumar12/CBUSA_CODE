Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports Controls
Imports Components
Imports DataLayer
Imports System.IO
Imports System.Configuration.ConfigurationManager
Imports System.XML

Public Class ajax
	Inherits System.Web.UI.Page

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Cache.SetNoStore()

        Dim FunctionName As String = Request("f")
        Select Case FunctionName
            Case "HtmlValidation"
                ValidateHtml()
        End Select
    End Sub

    Public Shared Function GetRenderedHtml(ByVal Url As String) As String
        Dim Results As New System.Text.StringBuilder
        Dim req As System.Net.HttpWebRequest = System.Net.WebRequest.Create(Url)
        Dim d As New XmlDocument()
        d.Load(req.GetResponse().GetResponseStream())
        Dim errors As XmlNodeList = d.SelectNodes("/Envelope/Body/markupvalidationresponse/errors/errorlist/error")

        If errors.count = 0 Then
            Results.AppendLine("<b>No Errors Found</b>")
        Else
            Results.AppendLine("<b>" & errors.Count & " errors were found:<br/>")
            Results.AppendLine("<table cellpadding=""5"" cellspacing=""0"" border=""0""><tr><th>Line</th><th>Source</th><th>Explanation</th></tr>")
            For Each n As XmlNode In errors
                Dim line = n.SelectSingleNode("/line").Value
                Dim source = n.SelectSingleNode("/source").Value
                Dim explanation = n.SelectSingleNode("/explanation").Value
                results.AppendLine("<tr><td>" & Line & "</td><td>" & HttpContext.Current.Server.HtmlEncode(source) & "</td><td>" & HttpContext.Current.Server.HtmlEncode(explanation) & "</td></tr>")
            Next
            results.AppendLine("</table>")
        End If

        Return Results.ToString
    End Function

    Private Sub ValidateHtml()
        Dim qs As New UrlParameters
        Dim HTML As String = HttpContext.Current.Server.UrlEncode("<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd""><html lang=""en"" xmlns=""http://www.w3.org/1999/xhtml""><head><title></title></head><body>" & request("html") & "</body></html>")
        Response.Write(GetRenderedHtml("http://xhtmlvalidator.americaneagle.com/w3c-validator/check?output=soap12&fragment=" & html))
    End Sub

End Class