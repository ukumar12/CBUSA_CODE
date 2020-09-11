Imports System
Imports System.Configuration.Provider

Namespace Components

    Public MustInherit Class URLMappingProvider
        Inherits ProviderBase

        Public MustOverride Function GetURL(ByVal DB As Database, ByVal Url As String) As String
        Public MustOverride Function IsValidURL(ByVal DB As Database, ByVal Url As String) As Boolean
    End Class
End Namespace
