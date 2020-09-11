Imports System
Imports System.Configuration
Imports System.Configuration.Provider
Imports System.Web.Configuration

Namespace Components

    Public Class URLMappingManager

        'Initialization related variables and logic
        Private Shared isInitialized As Boolean = False
        Private Shared initializationException As Exception
        Private Shared initializationLock As Object = New Object()

        Shared Sub New()
            Initialize()
        End Sub

        Private Shared Sub Initialize()

            Try
                'Get the feature's configuration info
                Dim qc As URLMappingConfiguration = CType(ConfigurationManager.GetSection("URLMappingProvider"), URLMappingConfiguration)
                If qc.DefaultProvider = Nothing OrElse qc.Providers Is Nothing OrElse qc.Providers.Count < 1 Then
                    Throw New ProviderException("You must specify a valid default provider.")
                End If

                'Instantiate the providers
                providerCollection = New URLMappingProviderCollection()
                ProvidersHelper.InstantiateProviders(qc.Providers, providerCollection, GetType(URLMappingProvider))
                providerCollection.SetReadOnly()
                defaultProvider = providerCollection(qc.DefaultProvider)
                If defaultProvider Is Nothing Then
                    Throw New ConfigurationErrorsException("You must specify a default provider for the feature.", qc.ElementInformation.Properties("defaultProvider").Source, qc.ElementInformation.Properties("defaultProvider").LineNumber)
                End If
            Catch ex As Exception
                initializationException = ex
                isInitialized = True
                Throw ex
            End Try
            isInitialized = True 'error-free initialization
        End Sub

        'Public feature API
        Private Shared defaultProvider As URLMappingProvider
        Private Shared providerCollection As URLMappingProviderCollection

        Public Shared ReadOnly Property Provider() As URLMappingProvider
            Get
                Return defaultProvider
            End Get
        End Property

        Public Shared ReadOnly Property Providers() As URLMappingProviderCollection
            Get
                Return providerCollection
            End Get
        End Property

        Public Shared Function GetURL(ByVal DB As Database, ByVal Url As String) As String
            Return Provider.GetURL(DB, Url)
        End Function

        Public Shared Function IsValidFolder(ByVal Url As String) As Boolean
            Dim Folders As String() = {"bin", "app_code", "includes", "modules", "controls", "images", "assets", "userfiles", "fckeditor", "admin"}
            For Each folder As String In Folders
                If Left(LCase(Url), folder.Length + 1) = "/" & LCase(folder) Then
                    Return False
                End If
            Next
            Return True
        End Function

        Public Shared Function IsValidURL(ByVal DB As Database, ByVal Url As String) As Boolean
            Return Provider.IsValidURL(DB, Url)
        End Function
    End Class
End Namespace
