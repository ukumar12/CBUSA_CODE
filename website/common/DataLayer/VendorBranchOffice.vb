Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class VendorBranchOfficeRow
        Inherits VendorBranchOfficeRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorBranchOfficeID As Integer)
            MyBase.New(DB, VendorBranchOfficeID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorBranchOfficeID As Integer) As VendorBranchOfficeRow
            Dim row As VendorBranchOfficeRow

            row = New VendorBranchOfficeRow(DB, VendorBranchOfficeID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorBranchOfficeID As Integer)
            Dim row As VendorBranchOfficeRow

            row = New VendorBranchOfficeRow(DB, VendorBranchOfficeID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorBranchOffice"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetListByVendor(ByVal DB As Database, ByVal VendorId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorBranchOffice where VendorId = " & VendorId
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

    End Class

    Public MustInherit Class VendorBranchOfficeRowBase
        Private m_DB As Database
        Private m_VendorBranchOfficeID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_Address As String = Nothing
        Private m_Address2 As String = Nothing
        Private m_City As String = Nothing
        Private m_State As String = Nothing
        Private m_Zip As String = Nothing


        Public Property VendorBranchOfficeID() As Integer
            Get
                Return m_VendorBranchOfficeID
            End Get
            Set(ByVal Value As Integer)
                m_VendorBranchOfficeID = value
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

        Public Property Address() As String
            Get
                Return m_Address
            End Get
            Set(ByVal Value As String)
                m_Address = value
            End Set
        End Property

        Public Property Address2() As String
            Get
                Return m_Address2
            End Get
            Set(ByVal Value As String)
                m_Address2 = value
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

        Public Sub New(ByVal DB As Database, ByVal VendorBranchOfficeID As Integer)
            m_DB = DB
            m_VendorBranchOfficeID = VendorBranchOfficeID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM VendorBranchOffice WHERE VendorBranchOfficeID = " & DB.Number(VendorBranchOfficeID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_VendorBranchOfficeID = Convert.ToInt32(r.Item("VendorBranchOfficeID"))
            m_VendorID = Convert.ToInt32(r.Item("VendorID"))
            m_Address = Convert.ToString(r.Item("Address"))
            If IsDBNull(r.Item("Address2")) Then
                m_Address2 = Nothing
            Else
                m_Address2 = Convert.ToString(r.Item("Address2"))
            End If
            m_City = Convert.ToString(r.Item("City"))
            m_State = Convert.ToString(r.Item("State"))
            m_Zip = Convert.ToString(r.Item("Zip"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO VendorBranchOffice (" _
             & " VendorID" _
             & ",Address" _
             & ",Address2" _
             & ",City" _
             & ",State" _
             & ",Zip" _
             & ") VALUES (" _
             & m_DB.NullNumber(VendorID) _
             & "," & m_DB.Quote(Address) _
             & "," & m_DB.Quote(Address2) _
             & "," & m_DB.Quote(City) _
             & "," & m_DB.Quote(State) _
             & "," & m_DB.Quote(Zip) _
             & ")"

            VendorBranchOfficeID = m_DB.InsertSQL(SQL)

            Return VendorBranchOfficeID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE VendorBranchOffice SET " _
             & " VendorID = " & m_DB.NullNumber(VendorID) _
             & ",Address = " & m_DB.Quote(Address) _
             & ",Address2 = " & m_DB.Quote(Address2) _
             & ",City = " & m_DB.Quote(City) _
             & ",State = " & m_DB.Quote(State) _
             & ",Zip = " & m_DB.Quote(Zip) _
             & " WHERE VendorBranchOfficeID = " & m_DB.quote(VendorBranchOfficeID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM VendorBranchOffice WHERE VendorBranchOfficeID = " & m_DB.Number(VendorBranchOfficeID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class VendorBranchOfficeCollection
        Inherits GenericCollection(Of VendorBranchOfficeRow)
    End Class

End Namespace


