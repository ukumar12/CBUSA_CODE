Imports System.Web
Imports System.Web.UI
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Web.UI.WebControls

Namespace Controls
	<ToolboxData("<{0}:OneClickImageButton runat=server></{0}:OneClickButton>")> _
	Public Class OneClickImageButton
		Inherits System.Web.UI.WebControls.ImageButton

		Private m_PleaseWait As String = "PleaseWaitImageButton(this);"

		Public ReadOnly Property PleaseWait()
			Get
				Return m_PleaseWait
			End Get
		End Property

		Public Sub New()
			MyBase.New()
		End Sub

		'http://www.codeproject.com/aspnet/PleaseWaitButton.asp?df=100&forumid=101959&exp=0&select=1387355
		Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
			'Output the button's html (with attributes) to a dummy HtmlTextWriter
			Dim sw As StringWriter = New StringWriter()
			Dim wr As HtmlTextWriter = New HtmlTextWriter(sw)
			MyBase.Render(wr)
			Dim RenderedHTML As String = sw.ToString()
			wr.Close()
			sw.Close()

			Dim Replaced As String = String.Empty

			Dim rOnclick As Regex = New Regex("onclick[\s\n]*=[\s\n]*(""([^""]*)"")")
			Dim mOnclick As Match = rOnclick.Match(RenderedHTML)
			If mOnclick.Success Then
				Dim Existing As String = mOnclick.Groups(2).Value
				Replaced = String.Empty
				If Page.ClientScript.IsStartupScriptRegistered(GetType(BaseValidator), "ValidatorIncludeScript") AndAlso Me.CausesValidation Then
					Replaced = Existing & "; if (Page_IsValid) " & PleaseWait
				Else
					If Trim(Right(Existing, 1)) = ";" Then
						Replaced = Existing & " " & PleaseWait
					Else
						Replaced = Existing & "; " & PleaseWait
					End If
				End If
				RenderedHTML = rOnclick.Replace(RenderedHTML, "onclick=""" & Replaced & """")
			Else
				RenderedHTML = RenderedHTML.Insert(RenderedHTML.Length - 2, "onclick=""" & PleaseWait & """")
			End If

			Dim strBackColor As String
			If BackColor.Name <> "0" Then
				strBackColor = "#" & Right(BackColor.Name, 6)
			Else
				'default back color will be white
				strBackColor = "#fff"
			End If

			'Get image's size
			Dim intWidth As Integer = 0, intHeight As Integer = 0
			If ImageUrl.ToString <> String.Empty Then
				Components.Core.GetImageSize(HttpContext.Current.Server.MapPath(ImageUrl), intWidth, intHeight)
			End If

			'generate the style to force the height & width of the image
			'also set the bgcolor and the bgimg to the loading.gif
			'the functions.js.aspx will set the img.src to spacer.gif when the button is pressed
			Dim AdditionalStyle As String = "height:" & intHeight & "px;width:" & intWidth & "px;background: " & strBackColor & " url('/images/loading.gif') 50% 50% no-repeat;"

			'parse thru the existing style and add the gen'd style above
			Dim rStyle As Regex = New Regex("style[\s\n]*=[\s\n]*(""([^""]*)"")")
			Dim mStyle As Match = rStyle.Match(RenderedHTML)
			If mStyle.Success Then
				Dim Existing As String = mStyle.Groups(2).Value
				Replaced = String.Empty

				If Trim(Right(Existing, 1)) = ";" Then
					Replaced = Existing & " " & AdditionalStyle
				Else
					Replaced = Existing & "; " & AdditionalStyle
				End If
				RenderedHTML = rStyle.Replace(RenderedHTML, "style=""" & Replaced & """")
			Else
				RenderedHTML = RenderedHTML.Insert(RenderedHTML.Length - 2, "style=""" & AdditionalStyle & """")
			End If

			writer.Write(RenderedHTML)
		End Sub

		Private Sub OneClickImageButton_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
			If Convert.ToString(Me.Attributes("src")) <> String.Empty AndAlso ImageUrl = String.Empty Then
				ImageUrl = Me.Attributes("src").ToString
			End If
		End Sub
	End Class

End Namespace