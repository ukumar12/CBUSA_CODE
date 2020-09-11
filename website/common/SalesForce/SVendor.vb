Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports sforce
Imports DataLayer

Public Class SVendor


    Public Account As SVendorAccount
    Public Contact As SVendorContact


    'Initialize private variables for the class
    Sub New()
        Account = New SVendorAccount()
        Contact = New SVendorContact()
    End Sub

    Public Function getAccountObject() As sObject
        Return Account.getsObject()
    End Function
    Public Function fillAccount(ByVal DB As Database, ByVal dbVendorRow As VendorRow) As Boolean
        Account.fill(DB, dbVendorRow)
    End Function
    Public Function loadAccount(ByRef dbVendorRow As VendorRow) As Boolean
        Account.load(dbVendorRow)
        dbVendorRow.Email = IIf(Contact.Email <> "", Contact.Email, dbVendorRow.Email)
    End Function

    Public Function getContactObject() As sObject
        Return Contact.getsObject()
    End Function
    Public Function fillContact(ByVal DB As Database, ByVal dbVendorAccountRow As VendorAccountRow) As Boolean
        Contact.fill(DB, dbVendorAccountRow)
    End Function
    Public Function loadContact(ByRef dbVendorAccountRow As VendorAccountRow) As Boolean
        Contact.load(dbVendorAccountRow)
    End Function
End Class