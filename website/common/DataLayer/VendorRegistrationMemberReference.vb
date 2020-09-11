Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class VendorRegistrationMemberReferenceRow
        Inherits VendorRegistrationMemberReferenceRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorRegistrationMemberReferenceID As Integer)
            MyBase.New(DB, VendorRegistrationMemberReferenceID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorRegistrationMemberReferenceID As Integer) As VendorRegistrationMemberReferenceRow
            Dim row As VendorRegistrationMemberReferenceRow

            row = New VendorRegistrationMemberReferenceRow(DB, VendorRegistrationMemberReferenceID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorRegistrationMemberReferenceID As Integer)
            Dim row As VendorRegistrationMemberReferenceRow

            row = New VendorRegistrationMemberReferenceRow(DB, VendorRegistrationMemberReferenceID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorRegistrationMemberReference"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetListByVendorRegistration(ByVal DB As Database, ByVal VendorRegistrationId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorRegistrationMemberReference where VendorRegistrationId = " & VendorRegistrationId
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

    End Class

    Public MustInherit Class VendorRegistrationMemberReferenceRowBase
        Private m_DB As Database
        Private m_VendorRegistrationMemberReferenceID As Integer = Nothing
        Private m_VendorRegistrationID As Integer = Nothing
        Private m_FirstName As String = Nothing
        Private m_LastName As String = Nothing
        Private m_CompanyName As String = Nothing


        Public Property VendorRegistrationMemberReferenceID() As Integer
            Get
                Return m_VendorRegistrationMemberReferenceID
            End Get
            Set(ByVal Value As Integer)
                m_VendorRegistrationMemberReferenceID = value
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

        Public Sub New(ByVal DB As Database, ByVal VendorRegistrationMemberReferenceID As Integer)
            m_DB = DB
            m_VendorRegistrationMemberReferenceID = VendorRegistrationMemberReferenceID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM VendorRegistrationMemberReference WHERE VendorRegistrationMemberReferenceID = " & DB.Number(VendorRegistrationMemberReferenceID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_VendorRegistrationMemberReferenceID = Convert.ToInt32(r.Item("VendorRegistrationMemberReferenceID"))
            m_VendorRegistrationID = Convert.ToInt32(r.Item("VendorRegistrationID"))
            m_FirstName = Convert.ToString(r.Item("FirstName"))
            m_LastName = Convert.ToString(r.Item("LastName"))
            m_CompanyName = Convert.ToString(r.Item("CompanyName"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO VendorRegistrationMemberReference (" _
             & " VendorRegistrationID" _
             & ",FirstName" _
             & ",LastName" _
             & ",CompanyName" _
             & ") VALUES (" _
             & m_DB.NullNumber(VendorRegistrationID) _
             & "," & m_DB.Quote(FirstName) _
             & "," & m_DB.Quote(LastName) _
             & "," & m_DB.Quote(CompanyName) _
             & ")"

            VendorRegistrationMemberReferenceID = m_DB.InsertSQL(SQL)

            Return VendorRegistrationMemberReferenceID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE VendorRegistrationMemberReference SET " _
             & " VendorRegistrationID = " & m_DB.NullNumber(VendorRegistrationID) _
             & ",FirstName = " & m_DB.Quote(FirstName) _
             & ",LastName = " & m_DB.Quote(LastName) _
             & ",CompanyName = " & m_DB.Quote(CompanyName) _
             & " WHERE VendorRegistrationMemberReferenceID = " & m_DB.quote(VendorRegistrationMemberReferenceID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM VendorRegistrationMemberReference WHERE VendorRegistrationMemberReferenceID = " & m_DB.Number(VendorRegistrationMemberReferenceID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class VendorRegistrationMemberReferenceCollection
        Inherits GenericCollection(Of VendorRegistrationMemberReferenceRow)
    End Class

End Namespace


