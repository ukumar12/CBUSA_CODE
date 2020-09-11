Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class VendorRoleRow
        Inherits VendorRoleRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorRoleID As Integer)
            MyBase.New(DB, VendorRoleID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorRoleID As Integer) As VendorRoleRow
            Dim row As VendorRoleRow
            row = New VendorRoleRow(DB, VendorRoleID)
            row.Load()
            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorRoleID As Integer)
            Dim row As VendorRoleRow

            row = New VendorRoleRow(DB, VendorRoleID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorRole"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetVendorRoles(ByVal DB As Database, ByVal VendorID As Integer) As DataTable
            Dim sql As String = "select * from VendorRole r left join " _
                              & "   (select vavr.VendorRoleID, va.* from VendorAccountVendorRole vavr inner join " _
                              & "       VendorAccount va on vavr.VendorAccountID=va.VendorAccountID where va.IsActive=1 and va.VendorID=" & DB.Number(VendorID) & ") as tmp on r.VendorRoleID=tmp.VendorRoleID"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Sub AssignVendorRole(ByVal DB As Database, ByVal VendorID As Integer, ByVal VendorAccountID As Integer, ByVal RoleID As Integer)
            Dim sql As String = "update VendorAccountVendorRole set VendorAccountID=" & DB.Number(VendorAccountID) & " where VendorRoleID=" & DB.Number(RoleID) & " and VendorAccountID in (select VendorAccountID from VendorAccount where VendorID=" & DB.Number(VendorID) & ")"
            If DB.ExecuteSQL(sql) <> 1 Then
                sql = "insert into VendorAccountVendorRole (VendorAccountID,VendorRoleID) values (" & DB.Number(VendorAccountID) & "," & DB.Number(RoleID) & ")"
                DB.ExecuteSQL(sql)
            End If
        End Sub
        Public Shared Sub UpdateVendorRole(ByVal DB As Database, ByVal VendorID As Integer, ByVal VendorAccountID As Integer, ByVal PrevVendorAccountId As Integer, ByVal RoleID As Integer)
            Dim sql As String = "update VendorAccountVendorRole set VendorAccountID=" & DB.Number(VendorAccountID) & " where VendorRoleID=" & DB.Number(RoleID) & " and VendorAccountId=" & DB.Number(PrevVendorAccountId) & " and VendorAccountID in (select VendorAccountID from VendorAccount where VendorID=" & DB.Number(VendorID) & ")"
            DB.ExecuteSQL(sql)
        End Sub

        Public Shared Sub InsertVendorRole(ByVal DB As Database, ByVal VendorID As Integer, ByVal VendorAccountID As Integer, ByVal RoleID As Integer)
            Dim sql As String = "insert into VendorAccountVendorRole (VendorAccountID,VendorRoleID) values (" & DB.Number(VendorAccountID) & "," & DB.Number(RoleID) & ")"
            DB.ExecuteSQL(sql)
        End Sub

        Public Shared Sub ClearVendorRole(ByVal DB As Database, ByVal VendorID As Integer, ByVal VendorRoleID As Integer)
            Dim sql As String = "delete from VendorAccountVendorRole where VendorRoleID=" & DB.Number(VendorRoleID) _
                              & " and VendorAccountID in (select VendorAccountID from VendorAccount where VendorID=" & DB.Number(VendorID) & ")"
            DB.ExecuteSQL(sql)
        End Sub

        Public Shared Sub ClearVendorRoleForVendorAccount(ByVal DB As Database, ByVal VendorAccountID As Integer, ByVal VendorRoleID As Integer)
            Dim sql As String = "delete from VendorAccountVendorRole where VendorRoleID=" & DB.Number(VendorRoleID) _
                              & " and VendorAccountID =" & DB.Number(VendorAccountID)
            DB.ExecuteSQL(sql)
        End Sub

        Public Shared Sub ClearAllVendorRoles(ByVal DB As Database, ByVal VendorID As Integer)
            Dim sql As String = "delete from VendorAccountVendorRole where VendorAccountID in (select VendorAccountID from VendorAccount where VendorID=" & DB.Number(VendorID) & ")"
            DB.ExecuteSQL(sql)
        End Sub

    End Class

    Public MustInherit Class VendorRoleRowBase
        Private m_DB As Database
        Private m_VendorRoleID As Integer = Nothing
        Private m_VendorRole As String = Nothing


        Public Property VendorRoleID() As Integer
            Get
                Return m_VendorRoleID
            End Get
            Set(ByVal Value As Integer)
                m_VendorRoleID = value
            End Set
        End Property

        Public Property VendorRole() As String
            Get
                Return m_VendorRole
            End Get
            Set(ByVal Value As String)
                m_VendorRole = value
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

        Public Sub New(ByVal DB As Database, ByVal VendorRoleID As Integer)
            m_DB = DB
            m_VendorRoleID = VendorRoleID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM VendorRole WHERE VendorRoleID = " & DB.Number(VendorRoleID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_VendorRoleID = Convert.ToInt32(r.Item("VendorRoleID"))
            m_VendorRole = Convert.ToString(r.Item("VendorRole"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO VendorRole (" _
             & " VendorRole" _
             & ") VALUES (" _
             & m_DB.Quote(VendorRole) _
             & ")"

            VendorRoleID = m_DB.InsertSQL(SQL)

            Return VendorRoleID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE VendorRole SET " _
             & " VendorRole = " & m_DB.Quote(VendorRole) _
             & " WHERE VendorRoleID = " & m_DB.quote(VendorRoleID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM VendorRole WHERE VendorRoleID = " & m_DB.Number(VendorRoleID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class VendorRoleCollection
        Inherits GenericCollection(Of VendorRoleRow)
    End Class

End Namespace


