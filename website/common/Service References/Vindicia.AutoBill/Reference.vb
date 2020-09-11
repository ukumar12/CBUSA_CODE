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


Namespace Vindicia.AutoBill
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0"),  _
     System.ServiceModel.ServiceContractAttribute([Namespace]:="http://soap.vindicia.com/AutoBill", ConfigurationName:="Vindicia.AutoBill.AutoBillPortType")>  _
    Public Interface AutoBillPortType
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://soap.vindicia.com/AutoBill#update", ReplyAction:="*"), _
         System.ServiceModel.XmlSerializerFormatAttribute(Style:=System.ServiceModel.OperationFormatStyle.Rpc, Use:=System.ServiceModel.OperationFormatUse.Encoded), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TransactionItem)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.Transaction)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.MerchantEntitlementId)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPrice)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPeriod)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TaxExemption)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.NameValuePair))> _
        Function update(ByVal auth As Vindicia.AllDataTypes.Authentication, ByRef autobill As Vindicia.AllDataTypes.AutoBill, <System.Runtime.InteropServices.OutAttribute()> ByRef created As Boolean, <System.Runtime.InteropServices.OutAttribute()> ByRef firstBillDate As Object, <System.Runtime.InteropServices.OutAttribute()> ByRef firstBillAmount As Decimal, <System.Runtime.InteropServices.OutAttribute()> ByRef firstBillingCurrency As String, ByVal duplicateBehavior As Vindicia.AllDataTypes.DuplicateBehavior, ByVal validatePaymentMethod As Boolean, ByVal minChargebackProbability As Integer) As <System.ServiceModel.MessageParameterAttribute(Name:="return")> Vindicia.AllDataTypes.[Return]
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://soap.vindicia.com/AutoBill#cancel", ReplyAction:="*"), _
         System.ServiceModel.XmlSerializerFormatAttribute(Style:=System.ServiceModel.OperationFormatStyle.Rpc, Use:=System.ServiceModel.OperationFormatUse.Encoded), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TransactionItem)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.Transaction)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.MerchantEntitlementId)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPrice)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPeriod)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TaxExemption)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.NameValuePair))> _
        Function cancel(ByVal auth As Vindicia.AllDataTypes.Authentication, ByRef autobill As Vindicia.AllDataTypes.AutoBill, ByVal disentitle As Boolean, ByVal force As Boolean) As <System.ServiceModel.MessageParameterAttribute(Name:="return")> Vindicia.AllDataTypes.[Return]
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://soap.vindicia.com/AutoBill#delayBillingToDate", ReplyAction:="*"), _
         System.ServiceModel.XmlSerializerFormatAttribute(Style:=System.ServiceModel.OperationFormatStyle.Rpc, Use:=System.ServiceModel.OperationFormatUse.Encoded), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TransactionItem)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.Transaction)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.MerchantEntitlementId)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPrice)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPeriod)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TaxExemption)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.NameValuePair))> _
        Function delayBillingToDate(ByVal auth As Vindicia.AllDataTypes.Authentication, ByRef autobill As Vindicia.AllDataTypes.AutoBill, <System.Xml.Serialization.SoapElementAttribute(DataType:="date"), System.Runtime.InteropServices.OutAttribute()> ByRef nextBillingDate As Date, <System.Runtime.InteropServices.OutAttribute()> ByRef nextBillingAmount As Decimal, <System.Runtime.InteropServices.OutAttribute()> ByRef nextBillingCurrency As String, <System.Xml.Serialization.SoapElementAttribute(DataType:="date")> ByVal newBillingDate As Date) As <System.ServiceModel.MessageParameterAttribute(Name:="return")> Vindicia.AllDataTypes.[Return]
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://soap.vindicia.com/AutoBill#delayBillingByDays", ReplyAction:="*"), _
         System.ServiceModel.XmlSerializerFormatAttribute(Style:=System.ServiceModel.OperationFormatStyle.Rpc, Use:=System.ServiceModel.OperationFormatUse.Encoded), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TransactionItem)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.Transaction)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.MerchantEntitlementId)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPrice)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPeriod)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TaxExemption)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.NameValuePair))> _
        Function delayBillingByDays(ByVal auth As Vindicia.AllDataTypes.Authentication, ByRef autobill As Vindicia.AllDataTypes.AutoBill, <System.Xml.Serialization.SoapElementAttribute(DataType:="date"), System.Runtime.InteropServices.OutAttribute()> ByRef nextBillingDate As Date, <System.Runtime.InteropServices.OutAttribute()> ByRef nextBillingAmount As Decimal, <System.Runtime.InteropServices.OutAttribute()> ByRef nextBillingCurrency As String, ByVal delayDays As Integer) As <System.ServiceModel.MessageParameterAttribute(Name:="return")> Vindicia.AllDataTypes.[Return]
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://soap.vindicia.com/AutoBill#changeBillingDayOfMonth", ReplyAction:="*"), _
         System.ServiceModel.XmlSerializerFormatAttribute(Style:=System.ServiceModel.OperationFormatStyle.Rpc, Use:=System.ServiceModel.OperationFormatUse.Encoded), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TransactionItem)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.Transaction)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.MerchantEntitlementId)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPrice)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPeriod)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TaxExemption)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.NameValuePair))> _
        Function changeBillingDayOfMonth(ByVal auth As Vindicia.AllDataTypes.Authentication, ByRef autobill As Vindicia.AllDataTypes.AutoBill, <System.Xml.Serialization.SoapElementAttribute(DataType:="date"), System.Runtime.InteropServices.OutAttribute()> ByRef nextBillingDate As Date, <System.Runtime.InteropServices.OutAttribute()> ByRef nextBillingAmount As Decimal, <System.Runtime.InteropServices.OutAttribute()> ByRef nextBillingCurrency As String, ByVal dayOfMonth As Integer) As <System.ServiceModel.MessageParameterAttribute(Name:="return")> Vindicia.AllDataTypes.[Return]
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://soap.vindicia.com/AutoBill#fetchByAccountAndProduct", ReplyAction:="*"), _
         System.ServiceModel.XmlSerializerFormatAttribute(Style:=System.ServiceModel.OperationFormatStyle.Rpc, Use:=System.ServiceModel.OperationFormatUse.Encoded), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TransactionItem)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.Transaction)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.MerchantEntitlementId)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPrice)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPeriod)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TaxExemption)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.NameValuePair))> _
        Function fetchByAccountAndProduct(<System.Runtime.InteropServices.OutAttribute()> ByRef autobills() As AllDataTypes.AutoBill, ByVal auth As Vindicia.AllDataTypes.Authentication, ByVal account As Vindicia.AllDataTypes.Account, ByVal product As Vindicia.AllDataTypes.Product) As <System.ServiceModel.MessageParameterAttribute(Name:="return")> Vindicia.AllDataTypes.[Return]
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://soap.vindicia.com/AutoBill#fetchByMerchantAutoBillId", ReplyAction:="*"), _
         System.ServiceModel.XmlSerializerFormatAttribute(Style:=System.ServiceModel.OperationFormatStyle.Rpc, Use:=System.ServiceModel.OperationFormatUse.Encoded), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TransactionItem)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.Transaction)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.MerchantEntitlementId)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPrice)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPeriod)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TaxExemption)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.NameValuePair))> _
        Function fetchByMerchantAutoBillId(<System.Runtime.InteropServices.OutAttribute()> ByRef autobill As Vindicia.AllDataTypes.AutoBill, ByVal auth As Vindicia.AllDataTypes.Authentication, ByVal merchantAutoBillId As String) As <System.ServiceModel.MessageParameterAttribute(Name:="return")> Vindicia.AllDataTypes.[Return]
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://soap.vindicia.com/AutoBill#fetchByVid", ReplyAction:="*"), _
         System.ServiceModel.XmlSerializerFormatAttribute(Style:=System.ServiceModel.OperationFormatStyle.Rpc, Use:=System.ServiceModel.OperationFormatUse.Encoded), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TransactionItem)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.Transaction)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.MerchantEntitlementId)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPrice)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPeriod)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TaxExemption)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.NameValuePair))> _
        Function fetchByVid(<System.Runtime.InteropServices.OutAttribute()> ByRef autobill As Vindicia.AllDataTypes.AutoBill, ByVal auth As Vindicia.AllDataTypes.Authentication, ByVal vid As String) As <System.ServiceModel.MessageParameterAttribute(Name:="return")> Vindicia.AllDataTypes.[Return]
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://soap.vindicia.com/AutoBill#fetchByEmail", ReplyAction:="*"), _
         System.ServiceModel.XmlSerializerFormatAttribute(Style:=System.ServiceModel.OperationFormatStyle.Rpc, Use:=System.ServiceModel.OperationFormatUse.Encoded), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TransactionItem)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.Transaction)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.MerchantEntitlementId)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPrice)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPeriod)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TaxExemption)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.NameValuePair))> _
        Function fetchByEmail(<System.Runtime.InteropServices.OutAttribute()> ByRef autobills() As AllDataTypes.AutoBill, ByVal auth As Vindicia.AllDataTypes.Authentication, ByVal email As String) As <System.ServiceModel.MessageParameterAttribute(Name:="return")> Vindicia.AllDataTypes.[Return]
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://soap.vindicia.com/AutoBill#fetchByAccount", ReplyAction:="*"), _
         System.ServiceModel.XmlSerializerFormatAttribute(Style:=System.ServiceModel.OperationFormatStyle.Rpc, Use:=System.ServiceModel.OperationFormatUse.Encoded), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TransactionItem)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.Transaction)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.MerchantEntitlementId)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPrice)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPeriod)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TaxExemption)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.NameValuePair))> _
        Function fetchByAccount(<System.Runtime.InteropServices.OutAttribute()> ByRef autobills() As AllDataTypes.AutoBill, ByVal auth As Vindicia.AllDataTypes.Authentication, ByVal account As Vindicia.AllDataTypes.Account) As <System.ServiceModel.MessageParameterAttribute(Name:="return")> Vindicia.AllDataTypes.[Return]
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://soap.vindicia.com/AutoBill#futureRebills", ReplyAction:="*"), _
         System.ServiceModel.XmlSerializerFormatAttribute(Style:=System.ServiceModel.OperationFormatStyle.Rpc, Use:=System.ServiceModel.OperationFormatUse.Encoded), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TransactionItem)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.Transaction)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.MerchantEntitlementId)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPrice)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.BillingPlanPeriod)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.TaxExemption)), _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(AllDataTypes.NameValuePair))> _
        Function futureRebills(<System.Runtime.InteropServices.OutAttribute()> ByRef transactions() As AllDataTypes.Transaction, ByVal auth As Vindicia.AllDataTypes.Authentication, ByVal autobill As Vindicia.AllDataTypes.AutoBill, ByVal quantity As Integer) As <System.ServiceModel.MessageParameterAttribute(Name:="return")> Vindicia.AllDataTypes.[Return]
    End Interface

    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")>  _
    Public Interface AutoBillPortTypeChannel
        Inherits Vindicia.AutoBill.AutoBillPortType, System.ServiceModel.IClientChannel
    End Interface
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")>  _
    Partial Public Class AutoBillPortTypeClient
        Inherits System.ServiceModel.ClientBase(Of Vindicia.AutoBill.AutoBillPortType)
        Implements Vindicia.AutoBill.AutoBillPortType
        
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
        
        Public Function update(ByVal auth As Vindicia.AllDataTypes.Authentication, ByRef autobill As Vindicia.AllDataTypes.AutoBill, <System.Runtime.InteropServices.OutAttribute()> ByRef created As Boolean, <System.Runtime.InteropServices.OutAttribute()> ByRef firstBillDate As Object, <System.Runtime.InteropServices.OutAttribute()> ByRef firstBillAmount As Decimal, <System.Runtime.InteropServices.OutAttribute()> ByRef firstBillingCurrency As String, ByVal duplicateBehavior As Vindicia.AllDataTypes.DuplicateBehavior, ByVal validatePaymentMethod As Boolean, ByVal minChargebackProbability As Integer) As Vindicia.AllDataTypes.[Return] Implements Vindicia.AutoBill.AutoBillPortType.update
            Return MyBase.Channel.update(auth, autobill, created, firstBillDate, firstBillAmount, firstBillingCurrency, duplicateBehavior, validatePaymentMethod, minChargebackProbability)
        End Function
        
        Public Function cancel(ByVal auth As Vindicia.AllDataTypes.Authentication, ByRef autobill As Vindicia.AllDataTypes.AutoBill, ByVal disentitle As Boolean, ByVal force As Boolean) As Vindicia.AllDataTypes.[Return] Implements Vindicia.AutoBill.AutoBillPortType.cancel
            Return MyBase.Channel.cancel(auth, autobill, disentitle, force)
        End Function
        
        Public Function delayBillingToDate(ByVal auth As Vindicia.AllDataTypes.Authentication, ByRef autobill As Vindicia.AllDataTypes.AutoBill, <System.Runtime.InteropServices.OutAttribute()> ByRef nextBillingDate As Date, <System.Runtime.InteropServices.OutAttribute()> ByRef nextBillingAmount As Decimal, <System.Runtime.InteropServices.OutAttribute()> ByRef nextBillingCurrency As String, ByVal newBillingDate As Date) As Vindicia.AllDataTypes.[Return] Implements Vindicia.AutoBill.AutoBillPortType.delayBillingToDate
            Return MyBase.Channel.delayBillingToDate(auth, autobill, nextBillingDate, nextBillingAmount, nextBillingCurrency, newBillingDate)
        End Function
        
        Public Function delayBillingByDays(ByVal auth As Vindicia.AllDataTypes.Authentication, ByRef autobill As Vindicia.AllDataTypes.AutoBill, <System.Runtime.InteropServices.OutAttribute()> ByRef nextBillingDate As Date, <System.Runtime.InteropServices.OutAttribute()> ByRef nextBillingAmount As Decimal, <System.Runtime.InteropServices.OutAttribute()> ByRef nextBillingCurrency As String, ByVal delayDays As Integer) As Vindicia.AllDataTypes.[Return] Implements Vindicia.AutoBill.AutoBillPortType.delayBillingByDays
            Return MyBase.Channel.delayBillingByDays(auth, autobill, nextBillingDate, nextBillingAmount, nextBillingCurrency, delayDays)
        End Function
        
        Public Function changeBillingDayOfMonth(ByVal auth As Vindicia.AllDataTypes.Authentication, ByRef autobill As Vindicia.AllDataTypes.AutoBill, <System.Runtime.InteropServices.OutAttribute()> ByRef nextBillingDate As Date, <System.Runtime.InteropServices.OutAttribute()> ByRef nextBillingAmount As Decimal, <System.Runtime.InteropServices.OutAttribute()> ByRef nextBillingCurrency As String, ByVal dayOfMonth As Integer) As Vindicia.AllDataTypes.[Return] Implements Vindicia.AutoBill.AutoBillPortType.changeBillingDayOfMonth
            Return MyBase.Channel.changeBillingDayOfMonth(auth, autobill, nextBillingDate, nextBillingAmount, nextBillingCurrency, dayOfMonth)
        End Function
        
        Public Function fetchByAccountAndProduct(<System.Runtime.InteropServices.OutAttribute()> ByRef autobills() As AllDataTypes.AutoBill, ByVal auth As Vindicia.AllDataTypes.Authentication, ByVal account As Vindicia.AllDataTypes.Account, ByVal product As Vindicia.AllDataTypes.Product) As Vindicia.AllDataTypes.[Return] Implements Vindicia.AutoBill.AutoBillPortType.fetchByAccountAndProduct
            Return MyBase.Channel.fetchByAccountAndProduct(autobills, auth, account, product)
        End Function
        
        Public Function fetchByMerchantAutoBillId(<System.Runtime.InteropServices.OutAttribute()> ByRef autobill As Vindicia.AllDataTypes.AutoBill, ByVal auth As Vindicia.AllDataTypes.Authentication, ByVal merchantAutoBillId As String) As Vindicia.AllDataTypes.[Return] Implements Vindicia.AutoBill.AutoBillPortType.fetchByMerchantAutoBillId
            Return MyBase.Channel.fetchByMerchantAutoBillId(autobill, auth, merchantAutoBillId)
        End Function
        
        Public Function fetchByVid(<System.Runtime.InteropServices.OutAttribute()> ByRef autobill As Vindicia.AllDataTypes.AutoBill, ByVal auth As Vindicia.AllDataTypes.Authentication, ByVal vid As String) As Vindicia.AllDataTypes.[Return] Implements Vindicia.AutoBill.AutoBillPortType.fetchByVid
            Return MyBase.Channel.fetchByVid(autobill, auth, vid)
        End Function
        
        Public Function fetchByEmail(<System.Runtime.InteropServices.OutAttribute()> ByRef autobills() As AllDataTypes.AutoBill, ByVal auth As Vindicia.AllDataTypes.Authentication, ByVal email As String) As Vindicia.AllDataTypes.[Return] Implements Vindicia.AutoBill.AutoBillPortType.fetchByEmail
            Return MyBase.Channel.fetchByEmail(autobills, auth, email)
        End Function
        
        Public Function fetchByAccount(<System.Runtime.InteropServices.OutAttribute()> ByRef autobills() As AllDataTypes.AutoBill, ByVal auth As Vindicia.AllDataTypes.Authentication, ByVal account As Vindicia.AllDataTypes.Account) As Vindicia.AllDataTypes.[Return] Implements Vindicia.AutoBill.AutoBillPortType.fetchByAccount
            Return MyBase.Channel.fetchByAccount(autobills, auth, account)
        End Function
        
        Public Function futureRebills(<System.Runtime.InteropServices.OutAttribute()> ByRef transactions() As AllDataTypes.Transaction, ByVal auth As Vindicia.AllDataTypes.Authentication, ByVal autobill As Vindicia.AllDataTypes.AutoBill, ByVal quantity As Integer) As Vindicia.AllDataTypes.[Return] Implements Vindicia.AutoBill.AutoBillPortType.futureRebills
            Return MyBase.Channel.futureRebills(transactions, auth, autobill, quantity)
        End Function
    End Class
End Namespace