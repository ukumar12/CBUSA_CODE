﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:2.0.50727.3053
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace Vindicia.Entitlement
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0"),  _
     System.ServiceModel.ServiceContractAttribute([Namespace]:="http://soap.vindicia.com/Entitlement", ConfigurationName:="Vindicia.Entitlement.EntitlementPortType")>  _
    Public Interface EntitlementPortType
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://soap.vindicia.com/Entitlement#fetchByEntitlementIdAndAccount", ReplyAction:="*"), _
         System.ServiceModel.XmlSerializerFormatAttribute(Style:=System.ServiceModel.OperationFormatStyle.Rpc, Use:=System.ServiceModel.OperationFormatUse.Encoded), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TaxExemption)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.NameValuePair)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.PaymentMethod))> _
        Function fetchByEntitlementIdAndAccount(<System.Runtime.InteropServices.OutAttribute()> ByRef entitlement As AllDataTypes.Entitlement, ByVal auth As AllDataTypes.Authentication, ByVal entitlementId As String, ByVal account As AllDataTypes.Account) As <System.ServiceModel.MessageParameterAttribute(Name:="return")> AllDataTypes.[Return]
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://soap.vindicia.com/Entitlement#fetchByAccount", ReplyAction:="*"), _
         System.ServiceModel.XmlSerializerFormatAttribute(Style:=System.ServiceModel.OperationFormatStyle.Rpc, Use:=System.ServiceModel.OperationFormatUse.Encoded), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TaxExemption)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.NameValuePair)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.PaymentMethod))> _
        Function fetchByAccount(<System.Runtime.InteropServices.OutAttribute()> ByRef entitlements() As AllDataTypes.Entitlement, ByVal auth As AllDataTypes.Authentication, ByVal account As AllDataTypes.Account, ByVal showAll As Boolean) As <System.ServiceModel.MessageParameterAttribute(Name:="return")> AllDataTypes.[Return]
    End Interface

    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")>  _
    Public Interface EntitlementPortTypeChannel
        Inherits Vindicia.Entitlement.EntitlementPortType, System.ServiceModel.IClientChannel
    End Interface
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")>  _
    Partial Public Class EntitlementPortTypeClient
        Inherits System.ServiceModel.ClientBase(Of Vindicia.Entitlement.EntitlementPortType)
        Implements Vindicia.Entitlement.EntitlementPortType
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String)
            MyBase.New(endpointConfigurationName)
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String, ByVal remoteAddress As String)
            MyBase.New(endpointConfigurationName, remoteAddress)
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String, ByVal remoteAddress As System.ServiceModel.EndpointAddress)
            MyBase.New(endpointConfigurationName, remoteAddress)
        End Sub
        
        Public Sub New(ByVal binding As System.ServiceModel.Channels.Binding, ByVal remoteAddress As System.ServiceModel.EndpointAddress)
            MyBase.New(binding, remoteAddress)
        End Sub
        
        Public Function fetchByEntitlementIdAndAccount(<System.Runtime.InteropServices.OutAttribute()> ByRef entitlement As AllDataTypes.Entitlement, ByVal auth As AllDataTypes.Authentication, ByVal entitlementId As String, ByVal account As AllDataTypes.Account) As AllDataTypes.[Return] Implements Vindicia.Entitlement.EntitlementPortType.fetchByEntitlementIdAndAccount
            Return MyBase.Channel.fetchByEntitlementIdAndAccount(entitlement, auth, entitlementId, account)
        End Function
        
        Public Function fetchByAccount(<System.Runtime.InteropServices.OutAttribute()> ByRef entitlements() As AllDataTypes.Entitlement, ByVal auth As AllDataTypes.Authentication, ByVal account As AllDataTypes.Account, ByVal showAll As Boolean) As AllDataTypes.[Return] Implements Vindicia.Entitlement.EntitlementPortType.fetchByAccount
            Return MyBase.Channel.fetchByAccount(entitlements, auth, account, showAll)
        End Function
    End Class
End Namespace