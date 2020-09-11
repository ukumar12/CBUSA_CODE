Imports System.Web.UI

Namespace Controls

	<ToolboxData("<{0}:ConfirmImageButton runat=server></{0}:ConfirmImageButton>")> _
	Public Class ConfirmImageButton
		Inherits System.Web.UI.WebControls.ImageButton

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
			If Not Message Is Nothing Then
				Attributes.Add("onclick", "return confirm('" + Message.Replace("'", "\'") + "')")
			End If
			MyBase.AddAttributesToRender(writer)
		End Sub

	End Class

End Namespace
