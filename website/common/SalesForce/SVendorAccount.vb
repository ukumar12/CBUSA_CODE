Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports sforce
Imports DataLayer
Imports System.Runtime.Serialization
Imports System.Xml.Serialization

<Serializable> _
<DataContract> _
Public Class SVendorAccount

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
    Public Services_Offered__c As String

    <DataMember> _
    Public Discounts__c As String

    <DataMember> _
    Public Rebate_Program__c As String

    <DataMember> _
    Public Payment_Terms_for_CBUSA_Members__c As String

    <DataMember> _
    Public Specialty_Services__c As String

    <DataMember> _
    Public Accepted_Cards__c As String

    <DataMember> _
    Public BillingStreet As String

    <DataMember> _
    Public BillingCity As String

    <DataMember> _
    Public BillingState As String

    <DataMember> _
    Public BillingPostalCode As String

    <DataMember> _
    Public Membership_Status__c As String

    <DataMember> _
    Public Enable_Market_Share__c As String

    <DataMember> _
    Public IsPlansOnline__c As String

    <DataMember> _
    Public HasDocumentsAccess__c As String

    <DataMember>
    Public ExcludedVendor__c As String

    <DataMember>
    Public RecordTypeId As String



    Sub New()
        LLC_City__c = ""
        ShippingStreet = ""
        ShippingCity = ""
        ShippingState = ""
        ShippingPostalCode = ""
        RecordTypeId = "012300000004q3Y"
        Phone = ""
        Fax = ""
        Website = ""
        Services_Offered__c = ""
        Discounts__c = ""
        Rebate_Program__c = ""
        Payment_Terms_for_CBUSA_Members__c = ""
        Specialty_Services__c = ""
        Accepted_Cards__c = ""
        BillingStreet = ""
        BillingCity = ""
        BillingState = ""
        BillingPostalCode = ""
        Membership_Status__c = "0"
        Enable_Market_Share__c = "0"
        IsPlansOnline__c = "0"
        HasDocumentsAccess__c = "0"
        ExcludedVendor__c = ""
    End Sub

    Public Function getsObject() As sObject
        Dim obj As sObject = New sObject
        Dim contactFields(26) As System.Xml.XmlElement

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

        contactFields(11) = doc.CreateElement("Services_Offered__c")
        contactFields(11).InnerText = Services_Offered__c

        contactFields(12) = doc.CreateElement("Discounts__c")
        contactFields(12).InnerText = Discounts__c

        contactFields(13) = doc.CreateElement("Rebate_Program__c")
        contactFields(13).InnerText = Rebate_Program__c


        contactFields(14) = doc.CreateElement("Payment_Terms_for_CBUSA_Members__c")
        contactFields(14).InnerText = Payment_Terms_for_CBUSA_Members__c

        contactFields(15) = doc.CreateElement("Specialty_Services__c")
        contactFields(15).InnerText = Specialty_Services__c

        contactFields(16) = doc.CreateElement("Accepted_Cards__c")
        contactFields(16).InnerText = Accepted_Cards__c

        contactFields(17) = doc.CreateElement("BillingStreet")
        contactFields(17).InnerText = BillingStreet

        contactFields(18) = doc.CreateElement("BillingCity")
        contactFields(18).InnerText = BillingCity

        contactFields(19) = doc.CreateElement("BillingState")
        contactFields(19).InnerText = BillingState

        contactFields(20) = doc.CreateElement("BillingPostalCode")
        contactFields(20).InnerText = BillingPostalCode

        contactFields(21) = doc.CreateElement("Membership_Status__c")
        contactFields(21).InnerText = IIf(Membership_Status__c = "1", "Active", "Inactive")

        contactFields(22) = doc.CreateElement("Enable_Market_Share__c")
        contactFields(22).InnerText = Enable_Market_Share__c

        contactFields(23) = doc.CreateElement("IsPlansOnline__c")
        contactFields(23).InnerText = IsPlansOnline__c

        contactFields(24) = doc.CreateElement("HasDocumentsAccess__c")
        contactFields(24).InnerText = HasDocumentsAccess__c

        contactFields(25) = doc.CreateElement("ExcludedVendor__c")
        contactFields(25).InnerText = ExcludedVendor__c
        contactFields(26) = doc.CreateElement("RecordTypeId")
        contactFields(26).InnerText = "012300000004q3Y"

        obj.type = "Account"
        obj.Any = contactFields

        Return obj
    End Function


    Public Function fill(ByRef DB As Database, ByVal dbVendorRow As VendorRow) As Boolean
        Portal_Account_Id__c = dbVendorRow.HistoricID
        Id = dbVendorRow.CRMID
        RecordTypeId = "012300000004q3Y"
        LLC_City__c = dbVendorRow.GetSelectedLLCNames()
        Name = dbVendorRow.CompanyName
        ShippingStreet = dbVendorRow.Address
        ShippingStreet = dbVendorRow.Address2
        ShippingCity = dbVendorRow.City
        ShippingState = dbVendorRow.State
        ShippingPostalCode = dbVendorRow.Zip
        Phone = dbVendorRow.Phone
        Fax = dbVendorRow.Fax
        Website = dbVendorRow.WebsiteURL
        Services_Offered__c = dbVendorRow.ServicesOffered
        Discounts__c = dbVendorRow.Discounts
        Rebate_Program__c = dbVendorRow.RebateProgram
        Payment_Terms_for_CBUSA_Members__c = dbVendorRow.PaymentTerms
        Specialty_Services__c = dbVendorRow.SpecialtyServices
        Accepted_Cards__c = dbVendorRow.AcceptedCards
        BillingStreet = dbVendorRow.BillingAddress
        BillingStreet = dbVendorRow.BillingAddress
        BillingCity = dbVendorRow.BillingCity
        BillingState = dbVendorRow.BillingState
        BillingPostalCode = dbVendorRow.BillingZip
        Membership_Status__c = IIf(dbVendorRow.IsActive = True, "1", "0")
        Enable_Market_Share__c = IIf(dbVendorRow.EnableMarketShare = True, "1", "0")
        IsPlansOnline__c = IIf(dbVendorRow.IsPlansOnline = True, "1", "0")
        HasDocumentsAccess__c = IIf(dbVendorRow.HasDocumentsAccess = True, "1", "0")
        ExcludedVendor__c = IIf(dbVendorRow.ExcludedVendor = True, "1", "0")
    End Function
    Public Function load(ByRef dbVendorRow As VendorRow) As Boolean
        dbVendorRow.CRMID = IIf(Id <> "", Id, dbVendorRow.CRMID)
        IIf(Id <> "", Id, dbVendorRow.CRMID)
        If LLC_City__c <> "" Then
            dbVendorRow.DeleteFromAllLLCs()
            dbVendorRow.InsertToLLCsByNames(LLC_City__c)
        End If
        
        dbVendorRow.CompanyName = IIf(Name <> "", Name, dbVendorRow.CompanyName)
        dbVendorRow.Address = IIf(ShippingStreet <> "", Name, dbVendorRow.Address)
        dbVendorRow.Address2 = IIf(ShippingStreet <> "", ShippingStreet, dbVendorRow.Address2)
        dbVendorRow.City = IIf(ShippingCity <> "", ShippingCity, dbVendorRow.City)
        dbVendorRow.State = IIf(ShippingState <> "", ShippingState, dbVendorRow.State)
        dbVendorRow.Zip = IIf(ShippingPostalCode <> "", ShippingPostalCode, dbVendorRow.Zip)
        dbVendorRow.Phone = IIf(Phone <> "", Phone, dbVendorRow.Phone)
        dbVendorRow.Fax = IIf(Fax <> "", Fax, dbVendorRow.Fax)
        dbVendorRow.WebsiteURL = IIf(Website <> "", Website, dbVendorRow.WebsiteURL)
        dbVendorRow.ServicesOffered = IIf(Services_Offered__c <> "", Services_Offered__c, dbVendorRow.ServicesOffered)
        dbVendorRow.Discounts = IIf(Discounts__c <> "", Discounts__c, dbVendorRow.Discounts)
        dbVendorRow.RebateProgram = IIf(Rebate_Program__c <> "", Rebate_Program__c, dbVendorRow.RebateProgram)
        dbVendorRow.PaymentTerms = IIf(Payment_Terms_for_CBUSA_Members__c <> "", Payment_Terms_for_CBUSA_Members__c, dbVendorRow.PaymentTerms)
        dbVendorRow.SpecialtyServices = IIf(Specialty_Services__c <> "", Specialty_Services__c, dbVendorRow.SpecialtyServices)
        dbVendorRow.AcceptedCards = IIf(Accepted_Cards__c <> "", Accepted_Cards__c, dbVendorRow.AcceptedCards)
        dbVendorRow.BillingAddress = IIf(BillingStreet <> "", BillingStreet, dbVendorRow.BillingAddress)
        dbVendorRow.BillingAddress = IIf(BillingStreet <> "", BillingStreet, dbVendorRow.BillingAddress)
        dbVendorRow.BillingCity = IIf(BillingCity <> "", BillingCity, dbVendorRow.BillingCity)
        dbVendorRow.BillingState = IIf(BillingState <> "", BillingState, dbVendorRow.BillingState)
        dbVendorRow.BillingZip = IIf(BillingPostalCode <> "", BillingPostalCode, dbVendorRow.BillingZip)
        dbVendorRow.IsActive = IIf(Membership_Status__c <> "", IIf(Membership_Status__c = "1", True, False), dbVendorRow.IsActive)
        dbVendorRow.EnableMarketShare = IIf(Enable_Market_Share__c <> "", IIf(Enable_Market_Share__c = "1", True, False), dbVendorRow.EnableMarketShare)

        dbVendorRow.IsPlansOnline = IIf(IsPlansOnline__c <> "", IIf(IsPlansOnline__c = "1", True, False), dbVendorRow.IsPlansOnline)
        dbVendorRow.HasDocumentsAccess = IIf(HasDocumentsAccess__c <> "", IIf(HasDocumentsAccess__c = "1", True, False), dbVendorRow.HasDocumentsAccess)
        dbVendorRow.ExcludedVendor = IIf(ExcludedVendor__c <> "", IIf(ExcludedVendor__c = "1", True, False), dbVendorRow.ExcludedVendor)
    End Function
    Public Overrides Function ToString() As String
        Dim Fields As String = "VendorAccount : ( "
        Fields &= "Portal_Account_Id__c :" & Portal_Account_Id__c & ","
        Fields &= "Id :" & Id & ","
        Fields &= "LLC_City__c :" & LLC_City__c & ","
        Fields &= "RecordTypeId :" & "012300000004q3Y" & ","
        Fields &= "Name :" & Name & ","
        Fields &= "ShippingStreet :" & ShippingStreet & ","
        Fields &= "ShippingCity :" & ShippingCity & ","
        Fields &= "ShippingState :" & ShippingState & ","
        Fields &= "ShippingPostalCode :" & ShippingPostalCode & ","
        Fields &= "Phone :" & Phone & ","
        Fields &= "Fax :" & Fax & ","
        Fields &= "Website :" & Website & ","
        Fields &= "Services_Offered__c :" & Services_Offered__c & ","
        Fields &= "Discounts__c :" & Discounts__c & ","
        Fields &= "Rebate_Program__c :" & Rebate_Program__c & ","
        Fields &= "Payment_Terms_for_CBUSA_Members__c :" & Payment_Terms_for_CBUSA_Members__c & ","
        Fields &= "Specialty_Services__c :" & Specialty_Services__c & ","
        Fields &= "Accepted_Cards__c :" & Accepted_Cards__c & ","
        Fields &= "BillingStreet :" & BillingStreet & ","
        Fields &= "BillingCity :" & BillingCity & ","
        Fields &= "BillingState :" & BillingState & ","
        Fields &= "BillingPostalCode :" & BillingPostalCode & ","
        Fields &= "Membership_Status__c :" & Membership_Status__c & ","
        Fields &= "Enable_Market_Share__c :" & Enable_Market_Share__c & ","
        Fields &= "IsPlansOnline__c :" & IsPlansOnline__c & ","
        Fields &= "HasDocumentsAccess__c :" & HasDocumentsAccess__c & ","
        Fields &= "ExcludedVendor__c :" & ExcludedVendor__c
        Fields &= " ) "
        Return Fields
    End Function

End Class