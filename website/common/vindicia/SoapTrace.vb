Imports Components
Imports System.Net
Imports System.IO

Namespace Components
    Public Class SoapTrace
        Inherits Web.Services.Protocols.SoapExtension

        Private inStream As Stream
        Private outStream As Stream
        Private init As Object

        Public Overloads Overrides Function GetInitializer(ByVal serviceType As System.Type) As Object
            Return init
        End Function

        Public Overloads Overrides Function GetInitializer(ByVal methodInfo As System.Web.Services.Protocols.LogicalMethodInfo, ByVal attribute As System.Web.Services.Protocols.SoapExtensionAttribute) As Object
            Return init
        End Function

        Public Overrides Sub Initialize(ByVal initializer As Object)
            init = initializer
        End Sub

        Public Overrides Sub ProcessMessage(ByVal message As System.Web.Services.Protocols.SoapMessage)
            If message.Stage = Web.Services.Protocols.SoapMessageStage.BeforeDeserialize Then
                Dim tr As New StreamReader(message.Stream)
                Dim str As String = tr.ReadToEnd
                tr.Close()
                Logger.Info(str)
            End If
        End Sub

        Private Sub LogMessage(ByVal msg As System.Web.Services.Protocols.SoapMessage)
            'CopyStream(inStream, outStream)
            Dim tr As New StreamReader(inStream)
            inStream.Position = 0
            Logger.Info(tr.ReadToEnd)
            inStream.Position = 0
            tr.Close()
        End Sub

        Public Overrides Function ChainStream(ByVal stream As System.IO.Stream) As System.IO.Stream
            inStream = stream
            outStream = New MemoryStream
            Return outStream
        End Function

        Private Sub CopyStream(ByVal src As Stream, ByVal dest As Stream)
            Dim tr As New StreamReader(src)
            Dim tw As New StreamWriter(dest)
            tw.WriteLine(tr.ReadToEnd)
            tw.Flush()
            tw.Close()
            tr.Close()
        End Sub
    End Class
End Namespace