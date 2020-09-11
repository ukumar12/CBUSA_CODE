Imports Components
Imports System.Web
Imports System.Web.UI

Namespace Controls
    Public Class UnvalidatedPostback
        Inherits Control
        Implements IPostBackEventHandler

        Public Event Postback As EventHandler

        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
            RaiseEvent Postback(Me, EventArgs.Empty)
        End Sub

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            writer.AddAttribute("type", "button")
            writer.AddAttribute("id", ClientID)
            writer.AddAttribute("name", UniqueID)
            writer.AddStyleAttribute("display", "none")
            writer.RenderBeginTag("input")
            writer.RenderEndTag()
        End Sub
    End Class
End Namespace
