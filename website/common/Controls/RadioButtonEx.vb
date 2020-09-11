Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.ComponentModel
Imports Components
Imports System.Web
Imports System.Collections

Namespace Controls

    Public Class RadioButtonEx
        Inherits Web.UI.WebControls.RadioButton
        Implements IPostBackDataHandler

        Public ReadOnly Property Value() As String
            Get
                Dim val As String = Attributes("value")
                If val Is Nothing Then
                    val = UniqueID
                End If
                Return val
            End Get
        End Property

        Protected Overrides Sub RaisePostDataChangedEvent() Implements IPostBackDataHandler.RaisePostDataChangedEvent
            MyBase.OnCheckedChanged(EventArgs.Empty)
        End Sub

        Protected Overrides Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements IPostBackDataHandler.LoadPostData
            Dim val As String = postCollection(GroupName)
            If Not val = Nothing AndAlso val = Value Then
                If Not Checked Then
                    Checked = True
                End If
            Else
                If Checked Then Checked = False
            End If
            Return Checked
        End Function

        Protected Overrides Sub Render(ByVal output As HtmlTextWriter)
            RenderInputTag(output)
        End Sub

        Private Sub RenderInputTag(ByVal htw As HtmlTextWriter)
            htw.AddAttribute(HtmlTextWriterAttribute.Id, ClientID)
            htw.AddAttribute(HtmlTextWriterAttribute.Type, "radio")
            htw.AddAttribute(HtmlTextWriterAttribute.Name, GroupName)
            htw.AddAttribute(HtmlTextWriterAttribute.Value, Value)
            If Checked Then htw.AddAttribute(HtmlTextWriterAttribute.Checked, "checked")
            If Not Enabled Then htw.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled")

            Dim onClick As String = Attributes("onclick")
            If AutoPostBack Then
                If Not onClick Is Nothing Then onClick = String.Empty
                onClick &= Page.ClientScript.GetPostBackEventReference(Me, String.Empty)
                htw.AddAttribute(HtmlTextWriterAttribute.Onclick, onClick)
                htw.AddAttribute("language", "javascript")
            Else
                If Not onClick Is Nothing Then htw.AddAttribute(HtmlTextWriterAttribute.Onclick, onClick)
            End If
            If AccessKey.Length > 0 Then
                htw.AddAttribute(HtmlTextWriterAttribute.Accesskey, AccessKey)
            End If
            If TabIndex <> 0 Then
                htw.AddAttribute(HtmlTextWriterAttribute.Tabindex, TabIndex.ToString(System.Globalization.NumberFormatInfo.InvariantInfo))
            End If
            htw.RenderBeginTag(HtmlTextWriterTag.Input)
            htw.RenderEndTag()
        End Sub

    End Class

End Namespace
