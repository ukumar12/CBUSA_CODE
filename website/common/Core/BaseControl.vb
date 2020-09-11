Imports System.Web.UI
Imports System.web.Caching
Imports System.Web

Namespace Components

    Public Interface ISmartBug
        Property URL() As String
    End Interface

    Public Class BaseControl
        Inherits System.Web.UI.UserControl

        Private m_Db As Database

        Public Sub New()
        End Sub

        Protected Property DB() As Database
            Get
                Return m_Db
            End Get
            Set(ByVal value As Database)
                m_Db = value
            End Set
        End Property

        Protected Sub AddError(ByVal ErrorMessage As String)
            If TypeOf Page Is BasePage Then
                CType(Me.Page, BasePage).AddError(ErrorMessage)
            End If
        End Sub

        Protected ReadOnly Property ErrHandler() As ErrorHandler
            Get
                Dim Handler As ErrorHandler = Nothing
                If TypeOf Page Is BasePage Then
                    Handler = CType(Me.Page, BasePage).ErrHandler()
                End If
                Return Handler
            End Get
        End Property

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If TypeOf Page Is BasePage Then
                DB = CType(Me.Page, BasePage).DB
            End If
        End Sub

        Protected Sub CheckPostData(ByVal controls As ControlCollection)
            For Each ctrl As Control In controls
                If TypeOf ctrl Is IPostBackDataHandler Then
                    Dim hnd As IPostBackDataHandler = CType(ctrl, IPostBackDataHandler)
                    On Error Resume Next
                    hnd.LoadPostData(ctrl.UniqueID, Request.Form)
                    On Error GoTo 0
                End If
                If ctrl.HasControls Then
                    CheckPostData(ctrl.Controls)
                End If
            Next
        End Sub

        Protected ReadOnly Property IsAdminDisplay() As Boolean
            Get
                If InStr(HttpContext.Current.Request.Path, "/admin/") <= 0 Then
                    Return False
                Else
                    Return True
                End If
            End Get
        End Property

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If Not IsAdminDisplay Then Exit Sub

            If Parent Is Nothing Then Exit Sub
            If Not TypeOf Parent Is BasePartialCachingControl Then Exit Sub

            Dim c As BasePartialCachingControl = Parent
            If Not c Is Nothing Then
                Dim dep As New CacheDependency(Nothing, New String() {""}, Now.AddMinutes(-1))
                c.Dependency = dep
            End If
        End Sub
    End Class

End Namespace