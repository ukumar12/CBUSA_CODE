Imports System
Imports System.Configuration.Provider

Namespace Components

    Public Class URLMappingProviderCollection
        Inherits ProviderCollection

        Public Overrides Sub Add(ByVal provider As ProviderBase)
            If provider Is Nothing Then
                Throw New ArgumentNullException("The provider parameter cannot be Nothing.")
            End If

            If Not TypeOf provider Is URLMappingProvider Then
                Throw New ArgumentException("The provider parameter must be of type URLMappingProvider")
            End If
            MyBase.Add(provider)
        End Sub

        Default Public Shadows ReadOnly Property Item(ByVal name As String) As URLMappingProvider
            Get
                Return CType(MyBase.Item(name), URLMappingProvider)
            End Get
        End Property

        Public Overloads Sub CopyTo(ByVal array() As URLMappingProvider, ByVal index As Integer)
            MyBase.CopyTo(array, index)
        End Sub
    End Class

End Namespace