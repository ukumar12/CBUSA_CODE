Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Configuration
Imports System.Web
Imports System.Web.UI
Imports DataLayer
Imports Components
Imports System.ComponentModel.Design
Imports System.Web.UI.WebControls
Imports System.Web.UI.Design.Webcontrols
Imports System.Web.UI.Design
Imports System.Web.UI.HtmlControls

Namespace MasterPages

    Public Class MyContainerControlDesigner
        Inherits ContainerControlDesigner

        Public Overrides ReadOnly Property AllowResize() As Boolean
            Get
                Return True
            End Get
        End Property
    End Class

    <Designer(GetType(MyContainerControlDesigner))> _
    <ParseChildren(False)> _
    <ToolboxData("<{0}:MasterPage runat=server></{0}:MasterPage>")> _
    Public Class MasterPage
        Inherits System.Web.UI.WebControls.WebControl

        Private m_TemplateContent As String
        Private m_DefaultContent As String

        Private Template As Control = Nothing
        Private Defaults As ContentRegion = New ContentRegion
        Private Contents As ArrayList = New ArrayList

        Private p As ContentToolPageRow
        Private t As ContentToolTemplateRow
        Private tr As ContentToolTemplateRegionRow
        Private Modules As DataView
        Private DB As Database

        Private m_PageTitle As String
        Private m_MetaKeywords As String
		Private m_MetaDescription As String
		Private m_MetaRobots As String
		Private m_IsIndexed As Boolean
        Private m_IsFollowed As Boolean
        Private m_IsPrint As Boolean
        Private m_IsIdevSearch As Boolean

        Private m_DefaultButton As String
        Public Property DefaultButton() As String
            Get
                Return m_DefaultButton
            End Get
            Set(ByVal value As String)
                m_DefaultButton = value
            End Set
        End Property

        Private m_DefaultFocus As String
        Public Property DefaultFocus() As String
            Get
                Return m_DefaultFocus
            End Get
            Set(ByVal value As String)
                m_DefaultFocus = value
            End Set
        End Property

		Public Property MetaRobots() As String
			Get
				Return m_MetaRobots
			End Get
			Set(ByVal value As String)
				m_MetaRobots = value
			End Set
		End Property

		Public Property IsPrint() As Boolean
			Get
				Return m_IsPrint
			End Get
			Set(ByVal value As Boolean)
				m_IsPrint = value
			End Set
		End Property

        Public Property IsIdevSearch() As Boolean
            Get
                Return m_IsIdevSearch
            End Get
            Set(ByVal value As Boolean)
                m_IsIdevSearch = value
            End Set
        End Property

        Public Property PageTitle() As String
            Get
                Return m_PageTitle
            End Get
            Set(ByVal Value As String)
                m_PageTitle = Value
            End Set
        End Property

        Public Property MetaKeywords() As String
            Get
                Return m_MetaKeywords
            End Get
            Set(ByVal Value As String)
                m_MetaKeywords = Value
            End Set
        End Property

        Public Property MetaDescription() As String
            Get
                Return m_MetaDescription
            End Get
            Set(ByVal Value As String)
                m_MetaDescription = Value
            End Set
        End Property

        Public Property IsIndexed() As Boolean
            Get
                Return m_IsIndexed
            End Get
            Set(ByVal Value As Boolean)
                m_IsIndexed = Value
            End Set
        End Property

        Public Property IsFollowed() As Boolean
            Get
                Return m_IsFollowed
            End Get
            Set(ByVal Value As Boolean)
                m_IsFollowed = Value
            End Set
        End Property

        Protected Property TemplateContent() As String
            Get
                Return m_TemplateContent
            End Get
            Set(ByVal Value As String)
                m_TemplateContent = Value
            End Set
        End Property

        <Category("MasterPage"), Description("Control ID for Default Content")> _
        Public Property DefaultContent() As String
            Get
                Return Me.m_DefaultContent
            End Get
            Set(ByVal Value As String)
                Me.m_DefaultContent = Value
            End Set
        End Property

        Public Sub New()
        End Sub

        Protected Overrides Sub AddParsedSubObject(ByVal obj As Object)
            If TypeOf obj Is ContentRegion Then
                Me.Contents.Add(obj)
            Else
                Me.Defaults.Controls.Add(CType(obj, Control))
            End If
        End Sub

        Protected Overrides Sub OnInit(ByVal e As EventArgs)
            If DesignMode Then Exit Sub

            Dim bp As BasePage = CType(Me.Page, BasePage)
            DB = bp.DB

            IsPrint = IIf(HttpContext.Current.Request("print") = String.Empty, False, True)
            IsIdevSearch = IIf(HttpContext.Current.Request("idevsearch") = String.Empty, False, True)

            p = ContentToolPageRow.GetRowByURL(DB, HttpContext.Current.Request("URL"))
            t = ContentToolTemplateRow.GetRow(DB, p.TemplateId)
            tr = ContentToolTemplateRegionRow.GetRow(DB, t.DefaultContentId)

            Modules = p.GetPageModules(ModuleLevel.All)

            Me.LoadModulesFromDB()
            Me.BuildMasterPage()

            Dim sm As ScriptManager = ScriptManager.GetCurrent(Me.Page)
            If sm Is Nothing And Not HttpContext.Current.Request.Url.ToString.ToLower.Contains("/admin/") Then
                sm = New ScriptManager
                sm.ID = "AjaxManager"
                'sm.ScriptMode = ScriptMode.Release
                sm.AsyncPostBackTimeout = 180
                Dim oForm As Controls.Form = Me.FindControl("main")
                If oForm IsNot Nothing Then
                    oForm.Controls.AddAt(0, sm)
                End If
            End If

            Me.BuildContents()

            'Add page controls to default control
            If Defaults.HasControls() Then
                Dim region As Control
                'If DefaultContent is blank, then page has not been registered
                'In that case add controls to this control
                If DefaultContent = Nothing Then
                    region = Me
                Else
                    region = FindControl(DefaultContent)
                End If
                If p.IsContentBefore Then
                    region.Controls.AddAt(0, Defaults)
                Else
                    region.Controls.Add(Defaults)
                End If
            End If

            'Set Default Button
            Dim form As HtmlForm = Me.FindControl("main")
            If Not form Is Nothing AndAlso Not DefaultButton Is Nothing Then
                form.DefaultButton = DefaultButton
            End If
            If Not form Is Nothing AndAlso Not DefaultFocus Is Nothing Then
                form.DefaultFocus = DefaultFocus
            End If

            'Load default values for Title and Meta tags
            PageTitle = p.Title
            IsIndexed = p.IsIndexed
            IsFollowed = p.IsFollowed
            MetaKeywords = p.MetaKeywords
            MetaDescription = p.MetaDescription

            MyBase.OnInit(e)
        End Sub

        Private Function GetRegion(ByVal ID As String) As ContentRegion
            For Each item As ContentRegion In Contents
                If item.ID = ID Then
                    Return item
                End If
            Next
            Return Nothing
        End Function

        Private Sub LoadModulesFromDB()

            For Each row As DataRowView In Modules
                Dim ID As String = row("ContentRegion")
                Dim ControlURL As String = IIf(IsDBNull(row("ControlURL")), String.Empty, row("ControlURL"))
                Dim Args As String = IIf(IsDBNull(row("Args")), String.Empty, row("Args"))
                Dim HTML As String = IIf(IsDBNull(row("HTML")), String.Empty, row("HTML"))
                Dim SkipIndexing As Boolean = Convert.ToBoolean(row("SkipIndexing"))

                'If SkipIndexing is set, then don't display control
                If IsIdevSearch AndAlso SkipIndexing Then
                    Continue For
                End If

                'create new region (if doesn't exist)
                Dim region As ContentRegion = GetRegion(ID)
                If region Is Nothing Then
                    region = New ContentRegion
                    region.ID = ID
                    Me.Contents.Add(region)
                End If

                If Not region.Mode = RegionMode.Replace Then
                    Dim ctrl As Control = Page.LoadControl(ControlURL)
                    region.Controls.Add(ctrl)

                    If TypeOf ctrl Is System.Web.UI.PartialCachingControl Then
                        Dim cached As Control = CType(ctrl, PartialCachingControl).CachedControl
                        If Not cached Is Nothing Then
                            ctrl = cached
                        End If
                    End If

                    Dim c As IModule = Nothing
                    If (TypeOf ctrl Is IModule) Then
                        c = CType(ctrl, IModule)
                    End If
                    If Not c Is Nothing Then
                        c.Args = Args
                        If Not HTML = String.Empty Then
                            c.HTMLContent = HTML
                        End If
                        c.Width = region.Width.Value
                    End If
                End If
            Next

        End Sub

        Private Sub BuildMasterPage()
            If IsPrint Then
                TemplateContent = IIf(t.PrintHTML = String.Empty, t.TemplateHTML, t.PrintHTML)
            Else
                TemplateContent = t.TemplateHTML
            End If
            DefaultContent = tr.ContentRegion

            If (Me.TemplateContent = String.Empty) Then
                'If page has not been registered then just leave the sub
                Return
            End If

            Me.Template = Page.ParseControl(TemplateContent)
            Me.Template.ID = Me.ID + "_Template"

            Dim count As Integer = Me.Template.Controls.Count
            Dim index As Integer
            For index = 0 To count - 1
                Dim control As Control = Me.Template.Controls(0)
                Me.Template.Controls.Remove(control)
                If control.Visible Then Me.Controls.Add(control)
            Next
            Me.Controls.AddAt(0, Me.Template)
        End Sub

        Private Sub BuildContent(ByVal content As ContentRegion)
            Dim region As Control = Me.FindControl(content.ID)
            If region Is Nothing Or Not (TypeOf region Is ContentRegion) Then
                If Not IsPrint Then
                    Throw New ApplicationException("ContentRegion with ID '" + content.ID + "' must be Defined")
                End If
            End If
            If Not region Is Nothing Then
                region.Controls.Clear()
            End If

            Dim count As Integer = content.Controls.Count
            Dim index As Integer
            For index = 0 To count - 1
                Dim control As Control = content.Controls(0)
                content.Controls.Remove(control)
                If Not region Is Nothing Then region.Controls.Add(control)
            Next
        End Sub

        Private Sub BuildContents()
            Dim content As ContentRegion
            For Each content In Me.Contents
                BuildContent(content)
            Next
        End Sub

        Public Overrides Sub RenderBeginTag(ByVal writer As System.Web.UI.HtmlTextWriter)
        End Sub
        Public Overrides Sub RenderEndTag(ByVal writer As System.Web.UI.HtmlTextWriter)
        End Sub

        Private Sub MasterPage_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            'Change the title and meta tags
            On Error Resume Next

            Dim Keywords As Literal = CType(Me.FindControl("MetaKeywords"), System.Web.UI.WebControls.Literal)
            Dim Description As Literal = CType(Me.FindControl("MetaDescription"), System.Web.UI.WebControls.Literal)
            Dim Robots As Literal = CType(Me.FindControl("MetaRobots"), System.Web.UI.WebControls.Literal)

            CType(Me.FindControl("PageTitle"), HtmlGenericControl).InnerText = PageTitle
            Keywords.Text = "<meta name=""keywords"" content=""" & MetaKeywords & """ />"
            Description.Text = "<meta name=""description"" content=""" & MetaDescription & """ />"
			If Not MetaRobots = String.Empty Then
				Robots.Text = MetaRobots
			Else
				If Not IsIndexed Or Not IsFollowed Then
					Robots.Text = "<meta name=""robots"" content=""" & IIf(IsIndexed, "index", "noindex") & "," & IIf(IsFollowed, "follow", "nofollow") & """ />"
				End If
				If HttpContext.Current.Request.IsSecureConnection OrElse IsPrint Then
					Robots.Text = "<meta name=""robots"" content=""noindex, nofollow"" />"
				End If
			End If
			On Error GoTo 0
        End Sub

        Public Shared ReadOnly Property BlankFileContent()
            Get
                Dim result As String = String.Empty
                result &= "<%@ Page Language=""vb"" AutoEventWireup=""false"" Inherits=""Components.SitePage"" %>" & vbCrLf
                result &= "<CT:masterpage runat=""server"" id=""CTMain"">" & vbCrLf
                result &= "</CT:masterpage>" & vbCrLf
                Return result
            End Get
        End Property
    End Class

End Namespace