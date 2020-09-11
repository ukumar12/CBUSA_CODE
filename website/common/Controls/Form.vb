Imports System.Web.UI
Imports System.Web
Imports System.Text
Imports System.IO
Imports System.Xml

Namespace Controls
    Public Class Form
        Inherits System.Web.UI.HtmlControls.HtmlForm

        Protected Overrides Sub RenderBeginTag(ByVal writer As HtmlTextWriter)
            Dim strWriter As New StringWriter
            Dim htmlWriter As New HtmlTextWriter(strWriter)
            MyBase.RenderBeginTag(htmlWriter)

            Dim xmlDoc As New XmlDocument
            xmlDoc.LoadXml(strWriter.ToString() & "</form>")
            Dim node As XmlNode = xmlDoc.SelectSingleNode("/form")
            node.Attributes("action").Value = HttpContext.Current.Request.RawUrl

            'Check for both types of closing tags 
            writer.Write(node.OuterXml.Replace(" />", ">").Replace("</form>", ""))
        End Sub
    End Class

End Namespace