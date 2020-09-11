﻿Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class AdminMessageVendorRecipientRow
        Inherits AdminMessageVendorRecipientRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal AdminMessageID As Integer)
            MyBase.New(DB, AdminMessageID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal AdminMessageID As Integer) As AdminMessageVendorRecipientRow
            Dim row As AdminMessageVendorRecipientRow

            row = New AdminMessageVendorRecipientRow(DB, AdminMessageID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal AdminMessageID As Integer)
            Dim row As AdminMessageVendorRecipientRow

            row = New AdminMessageVendorRecipientRow(DB, AdminMessageID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from AdminMessageVendorRecipient"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class AdminMessageVendorRecipientRowBase
        Private m_DB As Database
        Private m_AdminMessageID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_Created As DateTime = Nothing
        Private m_ReadDate As DateTime = Nothing
        Private m_IsActive As Boolean = Nothing


        Public Property AdminMessageID() As Integer
            Get
                Return m_AdminMessageID
            End Get
            Set(ByVal Value As Integer)
                m_AdminMessageID = Value
            End Set
        End Property

        Public Property VendorID() As Integer
            Get
                Return m_VendorID
            End Get
            Set(ByVal Value As Integer)
                m_VendorID = Value
            End Set
        End Property

        Public Property Created() As DateTime
            Get
                Return m_Created
            End Get
            Set(ByVal Value As DateTime)
                m_Created = Value
            End Set
        End Property

        Public Property ReadDate() As DateTime
            Get
                Return m_ReadDate
            End Get
            Set(ByVal Value As DateTime)
                m_ReadDate = Value
            End Set
        End Property

        Public Property IsActive() As Boolean
            Get
                Return m_IsActive
            End Get
            Set(ByVal Value As Boolean)
                m_IsActive = Value
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

        Public Sub New(ByVal DB As Database, ByVal AdminMessageID As Integer)
            m_DB = DB
            m_AdminMessageID = AdminMessageID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM AdminMessageVendorRecipient WHERE AdminMessageID = " & DB.Number(AdminMessageID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_AdminMessageID = Convert.ToInt32(r.Item("AdminMessageID"))
            m_VendorID = Convert.ToInt32(r.Item("VendorID"))
            m_Created = Convert.ToDateTime(r.Item("Created"))
            If IsDBNull(r.Item("ReadDate")) Then
                m_ReadDate = Nothing
            Else
                m_ReadDate = Convert.ToDateTime(r.Item("ReadDate"))
            End If
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO AdminMessageVendorRecipient (" _
             & " AdminDocumentID" _
             & ",VendorID" _
             & ",Created" _
             & ",ReadDate" _
             & ",IsActive" _
             & ") VALUES (" _
             & m_DB.NullNumber(AdminMessageID) _
             & "," & m_DB.NullNumber(VendorID) _
             & "," & m_DB.NullQuote(Created) _
             & "," & m_DB.NullQuote(ReadDate) _
             & "," & CInt(IsActive) _
             & ")"

            AdminMessageID = m_DB.InsertSQL(SQL)

            Return AdminMessageID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE AdminMessageVendorRecipient SET " _
             & " AdminMessageID= " & m_DB.NullNumber(AdminMessageID) _
             & ",VendorID = " & m_DB.NullNumber(VendorID) _
             & ",Created = " & m_DB.NullQuote(Created) _
             & ",ReadDate = " & m_DB.NullQuote(ReadDate) _
             & ",IsActive = " & CInt(IsActive) _
             & " WHERE AdminMessageID = " & m_DB.Quote(AdminMessageID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM AdminMessageVendorRecipient WHERE AdminMessageID = " & m_DB.Number(AdminMessageID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class AdminMessageVendorRecipientCollection
        Inherits GenericCollection(Of AdminMessageVendorRecipientRow)
    End Class

End Namespace
