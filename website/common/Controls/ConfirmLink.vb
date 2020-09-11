Imports System.Web.UI

Namespace Controls

    <ToolboxData("<{0}:ConfirmLink runat=server></{0}:ConfirmLink>")> _
    Public Class ConfirmLink
        Inherits System.Web.UI.WebControls.HyperLink

        Public Sub New()
            MyBase.New()
        End Sub

        Public Property Message() As String
            Get
                Return CType(ViewState("Message"), String)
            End Get
            Set(ByVal Value As String)
                ViewState("Message") = Value
            End Set
        End Property

        Protected Overrides Sub AddAttributesToRender(ByVal writer As HtmlTextWriter)
            Attributes.Add("onclick", "return confirm('" + Message.Replace("'", "\'") + "')")
            MyBase.AddAttributesToRender(writer)
        End Sub
    End Class

End Namespace
