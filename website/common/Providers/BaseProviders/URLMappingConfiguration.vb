Imports System
Imports System.Configuration

Namespace Components

    Public Class URLMappingConfiguration
        Inherits ConfigurationSection

        <ConfigurationProperty("providers")> _
        Public ReadOnly Property Providers() As ProviderSettingsCollection
            Get
                Return CType(MyBase.Item("providers"), ProviderSettingsCollection)
            End Get
        End Property

        <ConfigurationProperty("defaultProvider", DefaultValue:="RegExURLMappingProvider")> _
        <StringValidator(MinLength:=1)> _
        Public Property DefaultProvider() As String
            Get
                Return CType(MyBase.Item("defaultProvider"), String)
            End Get
            Set(ByVal Value As String)
                MyBase.Item("defaultProvider") = Value
            End Set
        End Property
    End Class
End Namespace
