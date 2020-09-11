Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports sforce
Imports DataLayer
Imports System.Runtime.Serialization
Imports System.Xml.Serialization

<Serializable> _
<DataContract> _
Public Class SVendorContact

    <NonSerialized> _
    <XmlIgnore> _
    <SoapIgnore> _
    Public Portal_Account_Id__c As Integer

    <NonSerialized> _
    <XmlIgnore> _
    <SoapIgnore> _
    Public Portal_Contact_Id__c As Integer

    <DataMember(IsRequired:=True)> _
    Public Id As String

    <DataMember> _
    Public FirstName As String

    <DataMember(IsRequired:=True)> _
    Public LastName As String

    <DataMember> _
    Public Title As String

    <DataMember> _
    Public Phone As String

    <DataMember> _
    Public MobilePhone As String

    <DataMember> _
    Public Fax As String

    <DataMember(IsRequired:=True)> _
    Public Email As String

    <DataMember> _
    Public Website_Username__c As String

    <DataMember> _
    Public Is_Primary__c As String

    <DataMember> _
    Public Is_Active__c As String

    <DataMember> _
    Public Require_Password_Update__c As String

    <DataMember> _
    Public Vendor_Roles__c As String

    Sub New()
        FirstName = ""
        Title = ""
        Phone = ""
        MobilePhone = ""
        Fax = ""
        Website_Username__c = ""
        Is_Primary__c = "0"
        Is_Active__c = "0"
        Require_Password_Update__c = "0"
        Vendor_Roles__c = ""
    End Sub

    Public Function getInsertsObject() As sObject
        Dim obj As sObject = New sObject
        Dim contactFields(14) As System.Xml.XmlElement
        Dim doc As System.Xml.XmlDocument = New System.Xml.XmlDocument()

        contactFields(0) = doc.CreateElement("Portal_Account_Id__c")
        contactFields(0).InnerText = Portal_Account_Id__c

        contactFields(1) = doc.CreateElement("Portal_Contact_Id__c")
        contactFields(1).InnerText = Portal_Contact_Id__c

        contactFields(2) = doc.CreateElement("AccountID")
        contactFields(2).InnerText = Id

        contactFields(3) = doc.CreateElement("FirstName")
        contactFields(3).InnerText = FirstName

        contactFields(4) = doc.CreateElement("LastName")
        contactFields(4).InnerText = LastName

        contactFields(5) = doc.CreateElement("Title")
        contactFields(5).InnerText = Title

        contactFields(6) = doc.CreateElement("Phone")
        contactFields(6).InnerText = Phone

        contactFields(7) = doc.CreateElement("MobilePhone")
        contactFields(7).InnerText = MobilePhone

        contactFields(8) = doc.CreateElement("Fax")
        contactFields(8).InnerText = Fax


        contactFields(9) = doc.CreateElement("Email")
        contactFields(9).InnerText = Email

        contactFields(10) = doc.CreateElement("Website_Username__c")
        contactFields(10).InnerText = Website_Username__c


        contactFields(11) = doc.CreateElement("Is_Primary__c")
        contactFields(11).InnerText = Is_Primary__c

        contactFields(12) = doc.CreateElement("Is_Active__c")
        contactFields(12).InnerText = Is_Active__c

        contactFields(13) = doc.CreateElement("Require_Password_Update__c")
        contactFields(13).InnerText = Require_Password_Update__c


        contactFields(14) = doc.CreateElement("Vendor_Roles__c")
        contactFields(14).InnerText = Vendor_Roles__c


        obj.type = "Contact"
        obj.Any = contactFields

        Return obj
    End Function


    Public Function getsObject() As sObject
        Dim obj As sObject = New sObject
        Dim contactFields(14) As System.Xml.XmlElement
        Dim doc As System.Xml.XmlDocument = New System.Xml.XmlDocument()

        contactFields(0) = doc.CreateElement("Portal_Account_Id__c")
        contactFields(0).InnerText = Portal_Account_Id__c

        contactFields(1) = doc.CreateElement("Portal_Contact_Id__c")
        contactFields(1).InnerText = Portal_Contact_Id__c

        contactFields(2) = doc.CreateElement("Id")
        contactFields(2).InnerText = Id

        contactFields(3) = doc.CreateElement("FirstName")
        contactFields(3).InnerText = FirstName

        contactFields(4) = doc.CreateElement("LastName")
        contactFields(4).InnerText = LastName

        contactFields(5) = doc.CreateElement("Title")
        contactFields(5).InnerText = Title

        contactFields(6) = doc.CreateElement("Phone")
        contactFields(6).InnerText = Phone

        contactFields(7) = doc.CreateElement("MobilePhone")
        contactFields(7).InnerText = MobilePhone

        contactFields(8) = doc.CreateElement("Fax")
        contactFields(8).InnerText = Fax


        contactFields(9) = doc.CreateElement("Email")
        contactFields(9).InnerText = Email

        contactFields(10) = doc.CreateElement("Website_Username__c")
        contactFields(10).InnerText = Website_Username__c


        contactFields(11) = doc.CreateElement("Is_Primary__c")
        contactFields(11).InnerText = Is_Primary__c

        contactFields(12) = doc.CreateElement("Is_Active__c")
        contactFields(12).InnerText = Is_Active__c

        contactFields(13) = doc.CreateElement("Require_Password_Update__c")
        contactFields(13).InnerText = Require_Password_Update__c


        contactFields(14) = doc.CreateElement("Vendor_Roles__c")
        contactFields(14).InnerText = Vendor_Roles__c


        obj.type = "Contact"
        obj.Any = contactFields

        Return obj
    End Function
    Public Function fill(ByRef DB As Database, ByVal dbVendorAccountRow As VendorAccountRow) As Boolean
        Portal_Account_Id__c = VendorRow.GetRow(DB, dbVendorAccountRow.VendorID).HistoricID
        Portal_Contact_Id__c = dbVendorAccountRow.VendorAccountID
        Id = dbVendorAccountRow.CRMID
        FirstName = dbVendorAccountRow.FirstName
        LastName = dbVendorAccountRow.LastName
        Title = dbVendorAccountRow.Title
        Phone = dbVendorAccountRow.Phone
        MobilePhone = dbVendorAccountRow.Mobile
        Fax = dbVendorAccountRow.Fax
        Email = dbVendorAccountRow.Email
        Website_Username__c = dbVendorAccountRow.Username
        'Website_Username__c = dbVendorAccountRow.Username
        Is_Primary__c = IIf(dbVendorAccountRow.IsPrimary = True, "1", "0")
        Is_Active__c = IIf(dbVendorAccountRow.IsActive = True, "1", "0")
        Require_Password_Update__c = IIf(dbVendorAccountRow.RequirePasswordUpdate = True, "1", "0")
        Try
            Dim aElements As String() = dbVendorAccountRow.GetSelectedVendorRoles.Split(",")
            If aElements.Count > 0 Then
                Vendor_Roles__c = CInt(aElements(0))
            Else
                Vendor_Roles__c = 0
            End If
        Catch ex As Exception
            Vendor_Roles__c = 0
        End Try

    End Function
    Public Function load(ByRef dbVendorAccountRow As VendorAccountRow) As Boolean
        dbVendorAccountRow.CRMID = IIf(Id <> "", Id, dbVendorAccountRow.CRMID)
        dbVendorAccountRow.FirstName = IIf(FirstName <> "", FirstName, dbVendorAccountRow.FirstName)
        dbVendorAccountRow.LastName = IIf(LastName <> "", LastName, dbVendorAccountRow.LastName)
        dbVendorAccountRow.Title = IIf(Title <> "", Title, dbVendorAccountRow.Title)
        dbVendorAccountRow.Phone = IIf(Phone <> "", Phone, dbVendorAccountRow.Phone)
        dbVendorAccountRow.Mobile = IIf(MobilePhone <> "", MobilePhone, dbVendorAccountRow.Mobile)
        dbVendorAccountRow.Fax = IIf(Fax <> "", Fax, dbVendorAccountRow.Fax)
        dbVendorAccountRow.Email = IIf(Email <> "", Email, dbVendorAccountRow.Email)
        dbVendorAccountRow.Username = IIf(Website_Username__c <> "", Website_Username__c, dbVendorAccountRow.Username)

        dbVendorAccountRow.IsPrimary = IIf(Is_Primary__c <> "", IIf(Is_Primary__c = "1", True, False), dbVendorAccountRow.IsPrimary)
        dbVendorAccountRow.IsActive = IIf(Is_Active__c <> "", IIf(Is_Active__c = "1", True, False), dbVendorAccountRow.IsActive)
        dbVendorAccountRow.RequirePasswordUpdate = IIf(Require_Password_Update__c <> "", IIf(Require_Password_Update__c = "1", True, False), dbVendorAccountRow.RequirePasswordUpdate)

    End Function

    Public Overrides Function ToString() As String
        Dim Fields As String = "VendorContact ( "
        Fields &= "Portal_Account_Id__c :" & Portal_Account_Id__c & ","
        Fields &= "Portal_Contact_Id__c :" & Portal_Contact_Id__c & ","
        Fields &= "Id :" & Id & ","
        Fields &= "FirstName :" & FirstName & ","
        Fields &= "LastName :" & LastName & ","
        Fields &= "Title :" & Title & ","
        Fields &= "Phone :" & Phone & ","
        Fields &= "MobilePhone :" & MobilePhone & ","
        Fields &= "Fax :" & Fax & ","
        Fields &= "Email :" & Email & ","
        Fields &= "Website_Username__c :" & Website_Username__c & ","
        Fields &= "Is_Primary__c :" & Is_Primary__c & ","
        Fields &= "Is_Active__c :" & Is_Active__c & ","
        Fields &= "Require_Password_Update__c :" & Require_Password_Update__c & ","
        Fields &= "Vendor_Roles__c :" & Vendor_Roles__c
        Fields &= " )"
        Return Fields
    End Function
End Class