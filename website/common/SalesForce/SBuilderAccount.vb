Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports sforce
Imports DataLayer
Imports System.Runtime.Serialization
Imports System.Xml.Serialization

<Serializable> _
<DataContract> _
Public Class SBuilderAccount

    <NonSerialized> _
    <XmlIgnore> _
    <SoapIgnore> _
    Public Portal_Account_Id__c As Integer

    <DataMember(IsRequired:=True)> _
    Public Id As String

    <DataMember> _
    Public LLC_City__c As String

    <DataMember(IsRequired:=True)> _
    Public Name As String

    <DataMember> _
    Public ShippingStreet As String

    <DataMember> _
    Public ShippingCity As String

    <DataMember> _
    Public ShippingState As String

    <DataMember> _
    Public ShippingPostalCode As String

    <DataMember> _
    Public Phone As String

    <DataMember> _
    Public Fax As String

    <DataMember> _
    Public Website As String

    <DataMember> _
    Public Membership_Status__c As String

    <DataMember> _
    Public SkipEntitlementCheck__c As String

    <DataMember> _
    Public IsNew__c As String

    <DataMember> _
    Public IsPlansOnline__c As String

    <DataMember> _
    Public HasDocumentsAccess__c As String

    <NonSerialized> _
    <XmlIgnore> _
    <SoapIgnore> _
    Public RegistrationStatusID__c As Integer

    <NonSerialized> _
    <XmlIgnore> _
    <SoapIgnore> _
    Public PreferredVendorID__c As Integer

    <DataMember> _
    Public RebatesEmailPreferences__c As String


    Sub New()
        LLC_City__c = ""
        ShippingStreet = ""
        ShippingCity = ""
        ShippingState = ""
        ShippingPostalCode = ""
        Phone = ""
        Fax = ""
        Website = ""
        Membership_Status__c = "0"
        IsNew__c = "0"
        IsPlansOnline__c = "0"
        HasDocumentsAccess__c = "0"
        PreferredVendorID__c = 0
        RebatesEmailPreferences__c = ""
        RegistrationStatusID__c = "0"
        SkipEntitlementCheck__c = False
    End Sub

    Public Function getsObject() As sObject
        Dim obj As sObject = New sObject
        Dim contactFields(18) As System.Xml.XmlElement

        Dim doc As System.Xml.XmlDocument = New System.Xml.XmlDocument()
        contactFields(0) = doc.CreateElement("Portal_Account_Id__c")
        contactFields(0).InnerText = Portal_Account_Id__c

        contactFields(1) = doc.CreateElement("Id")
        contactFields(1).InnerText = Id

        contactFields(2) = doc.CreateElement("LLC_City__c")
        contactFields(2).InnerText = LLC_City__c

        contactFields(3) = doc.CreateElement("Name")
        contactFields(3).InnerText = Name

        contactFields(4) = doc.CreateElement("ShippingStreet")
        contactFields(4).InnerText = ShippingStreet

        contactFields(5) = doc.CreateElement("ShippingCity")
        contactFields(5).InnerText = ShippingCity

        contactFields(6) = doc.CreateElement("ShippingState")
        contactFields(6).InnerText = ShippingState

        contactFields(7) = doc.CreateElement("ShippingPostalCode")
        contactFields(7).InnerText = ShippingPostalCode

        contactFields(8) = doc.CreateElement("Phone")
        contactFields(8).InnerText = Phone


        contactFields(9) = doc.CreateElement("Fax")
        contactFields(9).InnerText = Fax

        contactFields(10) = doc.CreateElement("Website")
        contactFields(10).InnerText = Website

        contactFields(11) = doc.CreateElement("Membership_Status__c")
        contactFields(11).InnerText = IIf(Membership_Status__c = "1", "Active", "Inactive")

        contactFields(12) = doc.CreateElement("SkipEntitlementCheck__c")
        contactFields(12).InnerText = SkipEntitlementCheck__c

        contactFields(13) = doc.CreateElement("IsNew__c")
        contactFields(13).InnerText = IsNew__c

        contactFields(14) = doc.CreateElement("IsPlansOnline__c")
        contactFields(14).InnerText = IsPlansOnline__c

        contactFields(15) = doc.CreateElement("HasDocumentsAccess__c")
        contactFields(15).InnerText = HasDocumentsAccess__c

        contactFields(16) = doc.CreateElement("RegistrationStatusID__c")
        contactFields(16).InnerText = RegistrationStatusID__c

        contactFields(17) = doc.CreateElement("PreferredVendorID__c")
        contactFields(17).InnerText = PreferredVendorID__c

        contactFields(18) = doc.CreateElement("RebatesEmailPreferences__c")
        contactFields(18).InnerText = RebatesEmailPreferences__c

        obj.type = "Account"
        obj.Any = contactFields

        Return obj
    End Function
    Public Function fill(ByRef DB As Database, ByVal dbBuilderRow As BuilderRow) As Boolean
        Portal_Account_Id__c = dbBuilderRow.HistoricID
        Id = dbBuilderRow.CRMID
        LLC_City__c = dbBuilderRow.LLCID
        Name = dbBuilderRow.CompanyName
        ShippingStreet = dbBuilderRow.Address
        ShippingCity = dbBuilderRow.City
        ShippingState = dbBuilderRow.State
        ShippingPostalCode = dbBuilderRow.Zip
        Phone = dbBuilderRow.Phone
        Fax = dbBuilderRow.Fax
        Website = dbBuilderRow.WebsiteURL
        Membership_Status__c = IIf(dbBuilderRow.IsActive = True, "1", "0")
        SkipEntitlementCheck__c = IIf(dbBuilderRow.SkipEntitlementCheck = True, "1", "0")
        IsNew__c = IIf(dbBuilderRow.IsNew = True, "1", "0")
        IsPlansOnline__c = IIf(dbBuilderRow.IsPlansOnline = True, "1", "0")
        HasDocumentsAccess__c = IIf(dbBuilderRow.HasDocumentsAccess = True, "1", "0")
        RegistrationStatusID__c = dbBuilderRow.RegistrationStatusID
        PreferredVendorID__c = dbBuilderRow.PreferredVendorID
        RebatesEmailPreferences__c = IIf(dbBuilderRow.RebatesEmailPreferences = True, "1", "0")

        Dim dbLLC As LLCRow = LLCRow.GetRow(DB, dbBuilderRow.LLCID)
        LLC_City__c = dbLLC.LLC

    End Function
    Public Function load(ByRef DB As Database, ByRef dbBuilderRow As BuilderRow) As Boolean
        dbBuilderRow.CRMID = IIf(Id <> "", Id, dbBuilderRow.CRMID)
        dbBuilderRow.CompanyName = IIf(Name <> "", Name, dbBuilderRow.CompanyName)
        dbBuilderRow.Address = IIf(ShippingStreet <> "", ShippingStreet, dbBuilderRow.Address)
        dbBuilderRow.City = IIf(ShippingCity <> "", ShippingCity, dbBuilderRow.City)
        dbBuilderRow.State = IIf(ShippingState <> "", ShippingState, dbBuilderRow.State)
        dbBuilderRow.Zip = IIf(ShippingPostalCode <> "", ShippingPostalCode, dbBuilderRow.Zip)
        dbBuilderRow.Phone = IIf(Phone <> "", Phone, dbBuilderRow.Phone)
        dbBuilderRow.Fax = IIf(Fax <> "", Fax, dbBuilderRow.Fax)
        dbBuilderRow.WebsiteURL = IIf(Website <> "", Website, dbBuilderRow.WebsiteURL)
        dbBuilderRow.IsActive = IIf(Membership_Status__c <> "", IIf(Membership_Status__c = "1", True, False), dbBuilderRow.IsActive)
        dbBuilderRow.SkipEntitlementCheck = IIf(SkipEntitlementCheck__c <> "", IIf(SkipEntitlementCheck__c = "1", True, False), dbBuilderRow.SkipEntitlementCheck)
        dbBuilderRow.IsNew = IIf(IsNew__c <> "", IIf(IsNew__c = "1", True, False), dbBuilderRow.IsNew)
        dbBuilderRow.IsPlansOnline = IIf(IsPlansOnline__c <> "", IIf(IsPlansOnline__c = "1", True, False), dbBuilderRow.IsPlansOnline)
        dbBuilderRow.HasDocumentsAccess = IIf(HasDocumentsAccess__c <> "", IIf(HasDocumentsAccess__c = "1", True, False), dbBuilderRow.HasDocumentsAccess)
        dbBuilderRow.RegistrationStatusID = IIf(RegistrationStatusID__c = 0, 1, RegistrationStatusID__c)


        Dim PreferredVendorID As Integer
        If Not Integer.TryParse(PreferredVendorID__c, PreferredVendorID) Then
            PreferredVendorID = 0
        End If

        dbBuilderRow.PreferredVendorID = IIf(PreferredVendorID > 0, PreferredVendorID, dbBuilderRow.PreferredVendorID)
        dbBuilderRow.RebatesEmailPreferences = IIf(RebatesEmailPreferences__c <> "", IIf(RebatesEmailPreferences__c = "1", True, False), dbBuilderRow.RebatesEmailPreferences)

        If LLC_City__c <> "" Then
            Dim dbLLC As LLCRow = LLCRow.GetRowByCity(DB, LLC_City__c)
            dbBuilderRow.LLCID = dbLLC.LLCID
        End If
        
    End Function

    Public Overrides Function ToString() As String
        Dim Fields As String = "BuilderAccount ( "
        Fields &= "Portal_Account_Id__c :" & Portal_Account_Id__c & ","
        Fields &= "Id :" & Id & ","
        Fields &= "LLC_City__c :" & LLC_City__c & ","
        Fields &= "Name :" & Name & ","
        Fields &= "ShippingStreet :" & ShippingStreet & ","
        Fields &= "ShippingCity :" & ShippingCity & ","
        Fields &= "ShippingState :" & ShippingState & ","
        Fields &= "ShippingPostalCode :" & ShippingPostalCode & ","
        Fields &= "Phone :" & Phone & ","
        Fields &= "Fax :" & Fax & ","
        Fields &= "Website :" & Website & ","
        Fields &= "Membership_Status__c :" & Membership_Status__c & ","
        Fields &= "SkipEntitlementCheck__c :" & SkipEntitlementCheck__c & ","
        Fields &= "IsNew__c :" & IsNew__c & ","
        Fields &= "IsPlansOnline__c :" & IsPlansOnline__c & ","
        Fields &= "HasDocumentsAccess__c :" & HasDocumentsAccess__c & ","
        Fields &= "RegistrationStatusID__c :" & RegistrationStatusID__c & ","
        Fields &= "PreferredVendorID__c :" & PreferredVendorID__c & ","
        Fields &= "RebatesEmailPreferences__c :" & RebatesEmailPreferences__c
        Fields &= " )"
        Return Fields
    End Function
End Class