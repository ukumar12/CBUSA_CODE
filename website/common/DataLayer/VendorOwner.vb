Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class VendorOwnersRow
        Inherits VendorOwnersRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorOwnerID As Integer)
            MyBase.New(DB, VendorOwnerID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorOwnerID As Integer) As VendorOwnersRow
            Dim row As VendorOwnersRow

            row = New VendorOwnersRow(DB, VendorOwnerID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorOwnerID As Integer)
            Dim row As VendorOwnersRow

            row = New VendorOwnersRow(DB, VendorOwnerID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorOwners"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetListByVendor(ByVal DB As Database, ByVal VendorId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorOwners where VendorId = " & VendorId
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

    End Class

    Public MustInherit Class VendorOwnersRowBase
        Private m_DB As Database
        Private m_VendorOwnerID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_FirstName As String = Nothing
        Private m_LastName As String = Nothing
        Private m_Phone As String = Nothing


        Public Property VendorOwnerID() As Integer
            Get
                Return m_VendorOwnerID
            End Get
            Set(ByVal Value As Integer)
                m_VendorOwnerID = value
            End Set
        End Property

        Public Property VendorID() As Integer
            Get
                Return m_VendorID
            End Get
            Set(ByVal Value As Integer)
                m_VendorID = value
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

        Public Sub New(ByVal DB As Database, ByVal VendorOwnerID As Integer)
            m_DB = DB
            m_VendorOwnerID = VendorOwnerID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM VendorOwners WHERE VendorOwnerID = " & DB.Number(VendorOwnerID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_VendorOwnerID = Convert.ToInt32(r.Item("VendorOwnerID"))
            m_VendorID = Convert.ToInt32(r.Item("VendorID"))
            m_FirstName = Convert.ToString(r.Item("FirstName"))
            m_LastName = Convert.ToString(r.Item("LastName"))
            m_Phone = Convert.ToString(r.Item("Phone"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO VendorOwners (" _
             & " VendorID" _
             & ",FirstName" _
             & ",LastName" _
             & ",Phone" _
             & ") VALUES (" _
             & m_DB.NullNumber(VendorID) _
             & "," & m_DB.Quote(FirstName) _
             & "," & m_DB.Quote(LastName) _
             & "," & m_DB.Quote(Phone) _
             & ")"

            VendorOwnerID = m_DB.InsertSQL(SQL)

            Return VendorOwnerID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE VendorOwners SET " _
             & " VendorID = " & m_DB.NullNumber(VendorID) _
             & ",FirstName = " & m_DB.Quote(FirstName) _
             & ",LastName = " & m_DB.Quote(LastName) _
             & ",Phone = " & m_DB.Quote(Phone) _
             & " WHERE VendorOwnerID = " & m_DB.quote(VendorOwnerID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM VendorOwners WHERE VendorOwnerID = " & m_DB.Number(VendorOwnerID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class VendorOwnersCollection
        Inherits GenericCollection(Of VendorOwnersRow)
    End Class

End Namespace


