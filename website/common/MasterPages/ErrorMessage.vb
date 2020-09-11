Imports System
Imports System.ComponentModel
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace MasterPages

    <ToolboxData("<{0}:ErrorMessage runat=server></{0}:ErrorMessage>")> _
    Public Class ErrorMessage
        Inherits System.Web.UI.WebControls.PlaceHolder

        Private m_Message As String = String.Empty
        Private m_Conn As String = ""

        Public Sub New()
            Visible = False
        End Sub

        Public Sub AddError(ByVal s As String)
            Message &= m_Conn & "&raquo; " & s
            If Not Message = String.Empty Then m_Conn = "<br />"
        End Sub

        Public Property Message() As String
            Get
                Return m_Message
            End Get
            Set(ByVal Value As String)
                m_Message = Value
            End Set
        End Property

        Public Property Width() As Integer
            Get
                Return ViewState("Width")
            End Get
            Set(ByVal Value As Integer)
                ViewState("Width") = Value
            End Set
        End Property

        Public Sub UpdateSummary()
            Dim valCol As ValidatorCollection = Page.Validators
            For Each val As IValidator In valCol
                If Not val.IsValid Then
                    AddError(val.ErrorMessage)
                End If
            Next
        End Sub

        Public Sub UpdateSummary(ByVal validationGroup As String)
            If validationGroup = String.Empty Then
                Exit Sub
            End If

            Dim valCol As ValidatorCollection = Page.Validators
            For Each val As BaseValidator In valCol
                If val.ValidationGroup = validationGroup AndAlso Not val.IsValid Then
                    AddError(val.ErrorMessage)
                End If
            Next
        End Sub

        Public Sub UpdateVisibility()
            Visible = (Not Message = String.Empty)
        End Sub

        Public Sub ClearErrors()
            Message = String.Empty
        End Sub

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            Dim WidthText As String = "98%"
            If Width <> 0 Then WidthText = Width & "px"

            writer.Write("<div align=""center"" id=""divError"">")
            writer.Write("<table style=""margin-top: 10px; margin-bottom: 10px; text-align:left; width:" & WidthText & "; background-color:#ffff99; border:#ff0000 1px solid;"" border=""0"" cellspacing=""0"" cellpadding=""10"">")
            writer.Write("<tr>")
            writer.Write("<td valign=""top"" style=""width:30px;"">")
            writer.Write("<img src=""/images/exclam.gif"" width=""24"" height=""24"" style=""border-style:none; padding-top:6px;"" alt=""Error"" /></td>")
            writer.Write("<td style=""width:100%"">")
            writer.Write("<div class=""bold red"" style=""margin-bottom:4px;"">")
            writer.Write("This form was not processed due to the following reasons:")
            writer.Write("</div>")
            writer.Write(Message)
            writer.Write("</td>")
            writer.Write("</tr>")
            writer.Write("</table>")
            writer.Write("</div>")
        End Sub
    End Class

End Namespace