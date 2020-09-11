Imports System.Web
Imports System.Web.UI
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports components
Imports System.Web.UI.HtmlControls

Namespace Controls

    <TargetControlType(GetType(Control))> _
    Public Class DivWindow
        Inherits ExtenderControl

        Private IsAdminMode As Boolean
        Private IsHidden As Boolean

        Private m_MoveToTop As Boolean = False
        Public Property MoveToTop() As Boolean
            Get
                Return m_MoveToTop
            End Get
            Set(ByVal value As Boolean)
                m_MoveToTop = value
            End Set
        End Property

        Private m_HeaderId As String
        Public Property HeaderId() As String
            Get
                Return m_HeaderId
            End Get
            Set(ByVal value As String)
                m_HeaderId = value
            End Set
        End Property

        Private m_CloseTriggerId As String
        Public Property CloseTriggerId() As String
            Get
                Return m_CloseTriggerId
            End Get
            Set(ByVal value As String)
                m_CloseTriggerId = value
            End Set
        End Property

        Private m_TriggerId As String
        Public Property TriggerId() As String
            Get
                Return m_TriggerId
            End Get
            Set(ByVal value As String)
                m_TriggerId = value
            End Set
        End Property

        Private m_HasShadow As Boolean
        Public Property HasShadow() As Boolean
            Get
                If HttpContext.Current.Request.Browser.Type.ToUpper().Contains("IE") Then
                    If HttpContext.Current.Request.Browser.MajorVersion <= 6 Then
                        Return False
                    End If
                End If
                Return m_HasShadow
            End Get
            Set(ByVal value As Boolean)
                m_HasShadow = value
            End Set
        End Property

        Private m_ShowVeil As Boolean
        Public Property ShowVeil() As Boolean
            Get
                Return m_ShowVeil
            End Get
            Set(ByVal value As Boolean)
                m_ShowVeil = value
            End Set
        End Property

        Private m_VeilCloses As Boolean
        Public Property VeilCloses() As Boolean
            Get
                Return m_VeilCloses
            End Get
            Set(ByVal value As Boolean)
                m_VeilCloses = value
            End Set
        End Property

        Public ReadOnly Property BehaviorId() As String
            Get
                Return FindControl(TargetControlID).ClientID
            End Get
        End Property

        Public ReadOnly Property BehaviorName() As String
            Get
                Return FindControl(TargetControlID).UniqueID
            End Get
        End Property

        Private Sub AddShadows()
            Dim ctl As Control = FindControl(TargetControlID)

            Dim div As New HtmlGenericControl("div")
            div.Attributes.Add("class", "shad-rgt")
            ctl.Controls.Add(div)

            div = New HtmlGenericControl("div")
            div.Attributes.Add("class", "shad-btm")
            ctl.Controls.Add(div)

            div = New HtmlGenericControl("div")
            div.Attributes.Add("class", "shad-cnr")
            ctl.Controls.Add(div)
        End Sub

        Protected Overrides Function GetScriptDescriptors(ByVal TargetControl As Control) As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor)
            Dim s As New ScriptBehaviorDescriptor("AE.DivWindow", TargetControl.ClientID)
            If HeaderId IsNot Nothing AndAlso HeaderId <> String.Empty Then
                s.AddElementProperty("header", TargetControl.FindControl(HeaderId).ClientID)
            End If
            If TriggerId IsNot Nothing AndAlso TriggerId <> String.Empty Then
                Dim trigger As Control = FindControl(TriggerId)
                If trigger Is Nothing Then
                    trigger = Page.FindControl(TriggerId)
                End If
                If trigger IsNot Nothing Then
                    s.AddElementProperty("trigger", trigger.ClientID)
                Else
                    Logger.Warning("Could not locate trigger, ID=" & TriggerId & " (DivWindow, GetScriptDescriptiors())")
                End If
            End If
            If CloseTriggerId IsNot Nothing AndAlso CloseTriggerId <> String.Empty Then
                Dim closeTrigger As Control = FindControl(CloseTriggerId)
                If closeTrigger Is Nothing Then
                    closeTrigger = Page.FindControl(CloseTriggerId)
                End If
                If closeTrigger IsNot Nothing Then
                    s.AddElementProperty("closeTrigger", closeTrigger.ClientID)
                Else
                    Logger.Warning("Could not locate close trigger, ID=" & CloseTriggerId & " (DivWindow, GetScriptDescriptors())")
                End If
            End If
            If MoveToTop Then
                s.AddProperty("moveTop", "true")
            End If
            If ShowVeil Then
                s.AddProperty("showVeil", "true")
                If VeilCloses Then
                    s.AddProperty("veilCloses", "true")
                End If
            End If
            s.AddProperty("name", TargetControl.UniqueID)
            Return New ScriptDescriptor() {s}
        End Function

        Protected Overrides Function GetScriptReferences() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptReference)
            Dim a As New List(Of ScriptReference)
            a.Add(New ScriptReference("/includes/controls/DivWindow.js"))
            Return a.ToArray
        End Function

        Protected Overrides Sub CreateChildControls()
            MyBase.CreateChildControls()
            If HasShadow Then
                AddShadows()
            End If
        End Sub
    End Class
End Namespace
