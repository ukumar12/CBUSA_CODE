Imports System
Imports System.Web
Imports System.Collections.Specialized
Imports System.Xml
Imports System.Web.Configuration

Namespace Components

    Public Class CustomErrorHandlerModule
        Implements IHttpModule

        Public Sub New()
        End Sub

        Public Sub Dispose() Implements IHttpModule.Dispose
        End Sub

        Public Sub Init(ByVal httpApp As System.Web.HttpApplication) Implements IHttpModule.Init
            AddHandler httpApp.Error, AddressOf Me.OnError
        End Sub

        Public Sub OnError(ByVal sender As Object, ByVal e As EventArgs)
            Dim ex As Exception = HttpContext.Current.Server.GetLastError()

            'Skip Error handling except 500
            If TypeOf (ex) Is System.Web.HttpException AndAlso CType(ex, HttpException).GetHttpCode() <> 500 Then
                Exit Sub
            End If

            Logger.Auto(Logger.GetErrorMessage(ex))

            Dim CustomErrors As CustomErrorsSection = ConfigurationManager.GetSection("system.web/customErrors")
            If CustomErrors.Mode = CustomErrorsMode.Off Then Exit Sub
            If CustomErrors.Mode = CustomErrorsMode.RemoteOnly And HttpContext.Current.Request.IsLocal Then Exit Sub

            If Not CustomErrors.Errors("500").Redirect = String.Empty Then
                HttpContext.Current.Server.ClearError()
                HttpContext.Current.Server.Transfer("/" & CustomErrors.Errors("500").Redirect)
            End If
        End Sub
    End Class
End Namespace
