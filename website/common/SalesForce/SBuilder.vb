Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports sforce
Imports DataLayer

Public Class SBuilder


    Public Account As SBuilderAccount
    Public Contact As SBuilderContact


    'Initialize private variables for the class
    Sub New()
        Account = New SBuilderAccount()
        Contact = New SBuilderContact()
    End Sub

    Public Function getAccountObject() As sObject
        Return Account.getsObject()
    End Function
    Public Function fillAccount(ByRef DB As Database, ByVal dbBuilderRow As BuilderRow) As Boolean
        Account.fill(DB, dbBuilderRow)
    End Function
    Public Function loadAccount(ByRef DB As Database, ByRef dbBuilderRow As BuilderRow) As Boolean
        Account.load(DB, dbBuilderRow)
        dbBuilderRow.Email = IIf(Contact.Email <> "", Contact.Email, dbBuilderRow.Email)
    End Function

    Public Function getContactObject() As sObject
        Return Contact.getsObject()
    End Function
    Public Function fillContact(ByRef DB As Database, ByVal dbBuilderAccountRow As BuilderAccountRow) As Boolean
        Contact.fill(DB, dbBuilderAccountRow)
    End Function
    Public Function loadContact(ByRef dbBuilderAccountRow As BuilderAccountRow) As Boolean
        Contact.load(dbBuilderAccountRow)
    End Function
End Class