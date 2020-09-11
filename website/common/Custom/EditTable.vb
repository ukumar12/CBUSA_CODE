Imports Components
Imports DataLayer
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web
Imports System.Text
Imports System.IO

Namespace Controls
    Public Class EditTable
        Inherits CompositeControl
        Implements IScriptControl
        Implements INamingContainer

        Private Const m_TemplateKey As String = "@CONTAINER_ID@"

        Private m_InitRows As Integer = 1
        Public Property InitRows() As Integer
            Get
                Return m_InitRows
            End Get
            Set(ByVal value As Integer)
                m_InitRows = value
            End Set
        End Property

        Private m_RowTemplate As ITemplate
        <PersistenceMode(PersistenceMode.InnerProperty)> _
        Public Property RowTemplate() As ITemplate
            Get
                Return m_RowTemplate
            End Get
            Set(ByVal value As ITemplate)
                m_RowTemplate = value
            End Set
        End Property

        Public Property RenderedRowTemplate() As String
            Get
                Dim cache As Hashtable = CType(Page, BasePage).PageCache
                If cache(UniqueID & "_template") IsNot Nothing Then
                    Return cache(UniqueID & "_template")
                End If
                Return String.Empty
            End Get
            Set(ByVal value As String)
                CType(Page, BasePage).PageCache(UniqueID & "_template") = value
            End Set
        End Property

        Public Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor) Implements System.Web.UI.IScriptControl.GetScriptDescriptors
            Dim s As New ScriptControlDescriptor("AE.EditTable", Me.ID)
            s.AddProperty("template", RenderTemplate())
            s.AddProperty("templateKey", m_TemplateKey)
            s.AddProperty("initRows", InitRows)
            Return New ScriptDescriptor() {s}
        End Function

        Public Function GetScriptReferences() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptReference) Implements System.Web.UI.IScriptControl.GetScriptReferences
            Dim s As New ScriptReference("/includes/controls/EditTable.js")
            Return New ScriptReference() {s}
        End Function

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            Dim sm As ScriptManager = ScriptManager.GetCurrent(Page)
            sm.RegisterScriptControl(Of EditTable)(Me)
            MyBase.OnPreRender(e)
        End Sub

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            Dim sm As ScriptManager = ScriptManager.GetCurrent(Page)
            sm.RegisterScriptDescriptors(Me)

            writer.AddAttribute("name", Me.UniqueID)
            writer.AddAttribute("id", Me.ClientID)
            writer.AddAttribute("type", "hidden")
            writer.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Input)
            writer.RenderEndTag()
            writer.AddAttribute("id", Me.ClientID & "_table")
            writer.RenderBeginTag("table")
            writer.RenderEndTag()
        End Sub

        Private Sub EditTable_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If RenderedRowTemplate = String.Empty Then
                'RenderedRowTemplate = RenderTemplate()
            End If
        End Sub

        Private Function RenderTemplate() As String
            Dim container As New BasicNamingContainer()
            container.ID = m_TemplateKey
            RowTemplate.InstantiateIn(container)
            Me.Controls.Add(container)

            Dim out As New StringBuilder()
            Dim sw As New StringWriter(out)
            Dim html As New HtmlTextWriter(sw)
            container.RenderControl(html)
            Return out.ToString.Replace("""", "'")
        End Function

        Public Class BasicNamingContainer
            Inherits PlaceHolder
            Implements INamingContainer
        End Class
    End Class

    Public Interface IEditTableTemplate
        Inherits ITemplate

        Property Validate() As String
    End Interface
End Namespace