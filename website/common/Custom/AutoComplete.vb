Imports System.Web
Imports System.Web.UI
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Components
Imports System.Web.UI.HtmlControls

Namespace Controls

    Public Class AutoComplete
        Inherits System.Web.UI.WebControls.TextBox
        Implements IScriptControl
        Implements ISubFormScriptControl
        Implements IPostBackEventHandler

        Public Event ValueUpdated As EventHandler

        Public Sub New()
            MyBase.New()
            Me.AutoCompleteType = WebControls.AutoCompleteType.None
        End Sub

        Private m_Table As String
        Public Property Table() As String
            Get
                Return m_Table
            End Get
            Set(ByVal value As String)
                m_Table = value
            End Set
        End Property

        Private m_TextField As String
        Public Property TextField() As String
            Get
                Return m_TextField
            End Get
            Set(ByVal value As String)
                m_TextField = value
            End Set
        End Property

        Private m_ValueField As String
        Public Property ValueField() As String
            Get
                Return m_ValueField
            End Get
            Set(ByVal value As String)
                m_ValueField = value
            End Set
        End Property

        Private m_LoadValue As String
        Public WriteOnly Property LoadValue() As String
            Set(ByVal value As String)
                m_LoadValue = value
            End Set
        End Property

        Private m_MinLength As Integer = 1
        Public Property MinLength() As Integer
            Get
                Return m_MinLength
            End Get
            Set(ByVal value As Integer)
                m_MinLength = value
            End Set
        End Property

        Private m_WaitInterval As Integer
        Public Property WaitInterval() As Integer
            Get
                Return m_WaitInterval
            End Get
            Set(ByVal value As Integer)
                m_WaitInterval = value
            End Set
        End Property

        Private m_AllowNew As Boolean
        Public Property AllowNew() As Boolean
            Get
                Return m_AllowNew
            End Get
            Set(ByVal value As Boolean)
                m_AllowNew = value
            End Set
        End Property

        Private m_CssClass As String
        Public Overrides Property CssClass() As String
            Get
                Return m_CssClass
            End Get
            Set(ByVal value As String)
                m_CssClass = value
            End Set
        End Property

        Private m_SearchFunction As String
        Public Property SearchFunction() As String
            Get
                Return m_SearchFunction
            End Get
            Set(ByVal value As String)
                m_SearchFunction = value
            End Set
        End Property

        Private m_AutoPostBack As Boolean
        Public Overrides Property AutoPostBack() As Boolean
            Get
                Return m_AutoPostBack
            End Get
            Set(ByVal value As Boolean)
                m_AutoPostBack = value
            End Set
        End Property

        Private m_WhereClause As String
        Public Property WhereClause() As String
            Get
                Return m_WhereClause
            End Get
            Set(ByVal value As String)
                m_WhereClause = value
            End Set
        End Property

        Private m_MaxResults As Integer = 10
        Public Property MaxResults() As Integer
            Get
                Return m_MaxResults
            End Get
            Set(ByVal value As Integer)
                m_MaxResults = value
            End Set
        End Property

        Private m_OnClientValueUpdated As String
        Public Property OnClientValueUpdated() As String
            Get
                Return m_OnClientValueUpdated
            End Get
            Set(ByVal value As String)
                m_OnClientValueUpdated = value
            End Set
        End Property

        Private m_OnClientTextChanged As String
        Public Property OnClientTextChanged() As String
            Get
                Return m_OnClientTextChanged
            End Get
            Set(ByVal value As String)
                m_OnClientTextChanged = value
            End Set
        End Property

        Private m_HideList As Boolean = False
        Public Property HideList() As Boolean
            Get
                Return m_HideList
            End Get
            Set(ByVal value As Boolean)
                m_HideList = value
            End Set
        End Property


        Public ReadOnly Property Value() As String
            Get
                Return HttpContext.Current.Request.Form(Me.UniqueID & Me.IdSeparator & "hdn")
            End Get
        End Property

        Public Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor) Implements System.Web.UI.IScriptControl.GetScriptDescriptors
            Dim s As New ModifiedControlDescriptor("AE.AutoComplete", ClientID)
            s.AddProperty("table", Table)
            s.AddProperty("textField", TextField)
            s.AddProperty("valueField", ValueField)
            s.AddProperty("minLength", MinLength)
            s.AddProperty("waitInterval", WaitInterval)
            s.AddProperty("allowNew", AllowNew)
            s.AddProperty("maxResults", MaxResults)
            s.AddScriptProperty("hideList", HideList.ToString.ToLower)
            If WhereClause <> Nothing Then
                s.AddProperty("whereClause", WhereClause)
            End If
            If AutoPostBack Then
                s.AddScriptProperty("autopostback", "function() {" & Page.ClientScript.GetPostBackEventReference(Me, String.Empty) & "}")
            End If
            If SearchFunction <> Nothing Then
                s.AddProperty("searchFunction", SearchFunction)
            End If
            If CssClass <> Nothing Then
                s.AddProperty("className", CssClass)
            End If
            If OnClientValueUpdated <> String.Empty Then
                s.AddProperty("onClientUpdate", OnClientValueUpdated)
            End If
            If OnClientTextChanged <> String.Empty Then
                s.AddProperty("onTextChanged", OnClientTextChanged)
            End If
            s.AddElementProperty("hdn", Me.ClientID & Me.ClientIDSeparator & "hdn")

            's.AddProperty("name", UniqueID)
            Return New ScriptDescriptor() {s}
        End Function

        Public Function GetScriptReferences() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptReference) Implements System.Web.UI.IScriptControl.GetScriptReferences
            Dim a As New List(Of ScriptReference)
            a.Add(New ScriptReference("/includes/controls/AutoComplete.js"))
            a.Add(New ScriptReference("/includes/controls/DivWindow.js"))
            Return a.ToArray
        End Function

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            Dim sm As ScriptManager = ScriptManager.GetCurrent(Page)
            sm.RegisterScriptControl(Me)
            MyBase.OnPreRender(e)
        End Sub

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            Dim sm As ScriptManager = ScriptManager.GetCurrent(Page)
            If RenderScript And sm IsNot Nothing Then
                sm.RegisterScriptDescriptors(Me)
            End If
            Attributes.Add("autocomplete", "off")
            MyBase.Render(writer)
            writer.AddAttribute("id", Me.ClientID & Me.ClientIDSeparator & "hdn")
            writer.AddAttribute("name", Me.UniqueID & Me.IdSeparator & "hdn")
            writer.AddAttribute("type", "hidden")
            If Value <> Nothing Then writer.AddAttribute("value", Value)
            If m_LoadValue <> Nothing Then writer.AddAttribute("value", m_LoadValue)
            writer.RenderBeginTag("input")
            writer.RenderEndTag()
            'writer.Write(writer.SelfClosingTagEnd)
        End Sub

        Public Function GetScript() As String Implements ISubFormScriptControl.GetScript
            Dim s As ScriptDescriptor() = GetScriptDescriptors()
            Dim out As String = String.Empty
            Dim conn As String = ""
            For Each sd As ModifiedControlDescriptor In s
                out &= conn & sd.GetScriptPublic()
                conn = ";"
            Next
            Return out
        End Function

        Private m_RenderScript As Boolean = True
        Public Property RenderScript() As Boolean Implements ISubFormScriptControl.RenderScript
            Get
                Return m_RenderScript
            End Get
            Set(ByVal value As Boolean)
                m_RenderScript = value
            End Set
        End Property

        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
            RaiseEvent ValueUpdated(Me, EventArgs.Empty)
        End Sub
    End Class

    Public Class ModifiedControlDescriptor
        Inherits ScriptControlDescriptor

        Public Sub New(ByVal type As String, ByVal ElementId As String)
            MyBase.New(type, ElementId)
        End Sub

        Public Function GetScriptPublic() As String
            Return MyBase.GetScript()
        End Function
    End Class
End Namespace
