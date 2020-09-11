Imports System.Web
Imports System.Web.UI
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Web.UI.WebControls

Namespace Controls
    <ToolboxData("<{0}:OneClickButton runat=server></{0}:OneClickButton>")> _
    Public Class OneClickButton
        Inherits System.Web.UI.WebControls.Button

		Private m_Message As String = Nothing
		Private m_PleaseWait As String = "PleaseWait(this);"

		Public ReadOnly Property PleaseWait()
			Get
				If Not Message = Nothing Then
					m_PleaseWait = "PleaseWait(this,'" & Message.Replace("'", "\'") & "');"
				Else
					m_PleaseWait = "PleaseWait(this);"
				End If
				Return m_PleaseWait
			End Get
		End Property

		Public Property Message() As String
			Get
				Return m_Message
			End Get
			Set(ByVal value As String)
				m_Message = value
			End Set
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

            Dim rOnclick As Regex = New Regex("onclick[\s\n]*=[\s\n]*(""([^""]*)"")")
            Dim mOnclick As Match = rOnclick.Match(RenderedHTML)
            If mOnclick.Success Then
                Dim Existing As String = mOnclick.Groups(2).Value
                Dim Replaced As String = String.Empty
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
            writer.Write(RenderedHTML)
        End Sub
    End Class

End Namespace
