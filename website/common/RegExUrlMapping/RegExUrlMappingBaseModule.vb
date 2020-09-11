Imports Microsoft.VisualBasic
Imports System.Web

'http://pietschsoft.com/blog/post.aspx?postid=762
Namespace RegExUrlMapping

    Public Class RegExUrlMappingBaseModule
        Implements System.Web.IHttpModule

        Sub Init(ByVal app As HttpApplication) Implements IHttpModule.Init
            AddHandler app.AuthorizeRequest, AddressOf Me.BaseModuleRewriter_AuthorizeRequest
        End Sub

        Sub Dispose() Implements System.Web.IHttpModule.Dispose
        End Sub

        Sub BaseModuleRewriter_AuthorizeRequest(ByVal sender As Object, ByVal e As EventArgs)
            Dim app As HttpApplication = CType(sender, HttpApplication)
            Rewrite(app.Request.Path, app)
        End Sub

        Overridable Sub Rewrite(ByVal requestedPath As String, ByVal app As HttpApplication)
        End Sub
    End Class



End Namespace

