﻿Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class VendorRegistrationFinancialReferenceRow
        Inherits VendorRegistrationFinancialReferenceRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorRegistrationFinancialReferenceID As Integer)
            MyBase.New(DB, VendorRegistrationFinancialReferenceID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorRegistrationFinancialReferenceID As Integer) As VendorRegistrationFinancialReferenceRow
            Dim row As VendorRegistrationFinancialReferenceRow

            row = New VendorRegistrationFinancialReferenceRow(DB, VendorRegistrationFinancialReferenceID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorRegistrationFinancialReferenceID As Integer)
            Dim row As VendorRegistrationFinancialReferenceRow

            row = New VendorRegistrationFinancialReferenceRow(DB, VendorRegistrationFinancialReferenceID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorRegistrationFinancialReference"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetListByVendorRegistration(ByVal DB As Database, ByVal VendorRegistrationId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorRegistrationFinancialReference where VendorRegistrationId = " & VendorRegistrationId
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

    End Class

    Public MustInherit Class VendorRegistrationFinancialReferenceRowBase
        Private m_DB As Database
        Private m_VendorRegistrationFinancialReferenceID As Integer = Nothing
        Private m_VendorRegistrationID As Integer = Nothing
        Private m_FirstName As String = Nothing
        Private m_LastName As String = Nothing
        Private m_CompanyName As String = Nothing
        Private m_City As String = Nothing
        Private m_State As String = Nothing
        Private m_Zip As String = Nothing
        Private m_Phone As String = Nothing


        Public Property VendorRegistrationFinancialReferenceID() As Integer
            Get
                Return m_VendorRegistrationFinancialReferenceID
            End Get
            Set(ByVal Value As Integer)
                m_VendorRegistrationFinancialReferenceID = value
            End Set
        End Property

        Public Property VendorRegistrationID() As Integer
            Get
                Return m_VendorRegistrationID
            End Get
            Set(ByVal Value As Integer)
                m_VendorRegistrationID = value
            End Set
        End Property

        Public Property FirstName() As String
            Get
                Return m_FirstName
            End Get
            Set(ByVal Value As String)
                m_FirstName = value
            End Set
        End Property

        Public Property LastName() As String
            Get
                Return m_LastName
            End Get
            Set(ByVal Value As String)
                m_LastName = value
            End Set
        End Property

        Public Property CompanyName() As String
            Get
                Return m_CompanyName
            End Get
            Set(ByVal Value As String)
                m_CompanyName = value
            End Set
        End Property

        Public Property City() As String
            Get
                Return m_City
            End Get
            Set(ByVal Value As String)
                m_City = value
            End Set
        End Property

        Public Property State() As String
            Get
                Return m_State
            End Get
            Set(ByVal Value As String)
                m_State = value
            End Set
        End Property

        Public Property Zip() As String
            Get
                Return m_Zip
            End Get
            Set(ByVal Value As String)
                m_Zip = value
            End Set
        End Property

        Public Property Phone() As String
            Get
                Return m_Phone
            End Get
            Set(ByVal Value As String)
                m_Phone = value
            End Set
        End Property


        Public Property DB() As Database
            Get
                DB = m_DB
            End Get
            Set(ByVal Value As DataBase)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorRegistrationFinancialReferenceID As Integer)
            m_DB = DB
            m_VendorRegistrationFinancialReferenceID = VendorRegistrationFinancialReferenceID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM VendorRegistrationFinancialReference WHERE VendorRegistrationFinancialReferenceID = " & DB.Number(VendorRegistrationFinancialReferenceID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_VendorRegistrationFinancialReferenceID = Convert.ToInt32(r.Item("VendorRegistrationFinancialReferenceID"))
            m_VendorRegistrationID = Convert.ToInt32(r.Item("VendorRegistrationID"))
            m_FirstName = Convert.ToString(r.Item("FirstName"))
            m_LastName = Convert.ToString(r.Item("LastName"))
            m_CompanyName = Convert.ToString(r.Item("CompanyName"))
            m_City = Convert.ToString(r.Item("City"))
            m_State = Convert.ToString(r.Item("State"))
            m_Zip = Convert.ToString(r.Item("Zip"))
            m_Phone = Convert.ToString(r.Item("Phone"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO VendorRegistrationFinancialReference (" _
             & " VendorRegistrationID" _
             & ",FirstName" _
             & ",LastName" _
             & ",CompanyName" _
             & ",City" _
             & ",State" _
             & ",Zip" _
             & ",Phone" _
             & ") VALUES (" _
             & m_DB.NullNumber(VendorRegistrationID) _
             & "," & m_DB.Quote(FirstName) _
             & "," & m_DB.Quote(LastName) _
             & "," & m_DB.Quote(CompanyName) _
             & "," & m_DB.Quote(City) _
             & "," & m_DB.Quote(State) _
             & "," & m_DB.Quote(Zip) _
             & "," & m_DB.Quote(Phone) _
             & ")"

            VendorRegistrationFinancialReferenceID = m_DB.InsertSQL(SQL)

            Return VendorRegistrationFinancialReferenceID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE VendorRegistrationFinancialReference SET " _
             & " VendorRegistrationID = " & m_DB.NullNumber(VendorRegistrationID) _
             & ",FirstName = " & m_DB.Quote(FirstName) _
             & ",LastName = " & m_DB.Quote(LastName) _
             & ",CompanyName = " & m_DB.Quote(CompanyName) _
             & ",City = " & m_DB.Quote(City) _
             & ",State = " & m_DB.Quote(State) _
             & ",Zip = " & m_DB.Quote(Zip) _
             & ",Phone = " & m_DB.Quote(Phone) _
             & " WHERE VendorRegistrationFinancialReferenceID = " & m_DB.quote(VendorRegistrationFinancialReferenceID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM VendorRegistrationFinancialReference WHERE VendorRegistrationFinancialReferenceID = " & m_DB.Number(VendorRegistrationFinancialReferenceID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class VendorRegistrationFinancialReferenceCollection
        Inherits GenericCollection(Of VendorRegistrationFinancialReferenceRow)
    End Class

End Namespace


