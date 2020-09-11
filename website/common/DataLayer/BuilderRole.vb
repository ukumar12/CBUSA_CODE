Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class BuilderRoleRow
        Inherits BuilderRoleRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BuilderRoleID As Integer)
            MyBase.New(DB, BuilderRoleID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal BuilderRoleID As Integer) As BuilderRoleRow
            Dim row As BuilderRoleRow

            row = New BuilderRoleRow(DB, BuilderRoleID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal BuilderRoleID As Integer)
            Dim row As BuilderRoleRow

            row = New BuilderRoleRow(DB, BuilderRoleID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from BuilderRole"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetBuilderRoles(ByVal DB As Database, ByVal BuilderID As Integer) As DataTable
            Dim sql As String = "select r.*,a.BuilderAccountID, a.Username from BuilderRole r left outer join (select babr.BuilderRoleID, ba.* from BuilderAccountBuilderRole babr inner join BuilderAccount ba on babr.BuilderAccountID=ba.BuilderAccountID where ba.BuilderID=" & DB.Number(BuilderID) & ") as a on r.BuilderRoleId=a.BuilderRoleId"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Sub AssignBuilderRole(ByVal DB As Database, ByVal BuilderID As Integer, ByVal BuilderAccountID As Integer, ByVal RoleID As Integer)
            Dim sql As String = "update BuilderAccountBuilderRole set BuilderAccountID=" & DB.Number(BuilderAccountID) & " where BuilderRoleId=" & DB.Number(RoleID) & " and BuilderAccountID in (select BuilderAccountID from BuilderAccount where BuilderID=" & DB.Number(BuilderID) & ")"
            If Not DB.ExecuteSQL(sql) Then
                sql = "insert into BuilderAccountBuilderRole (BuilderAccountID,BuilderRoleID) values (" & DB.Number(BuilderAccountID) & "," & DB.Number(RoleID) & ")"
                DB.ExecuteSQL(sql)
            End If
        End Sub

       

    End Class

    Public MustInherit Class BuilderRoleRowBase
        Private m_DB As Database
        Private m_BuilderRoleID As Integer = Nothing
        Private m_BuilderRole As String = Nothing


        Public Property BuilderRoleID() As Integer
            Get
                Return m_BuilderRoleID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderRoleID = value
            End Set
        End Property

        Public Property BuilderRole() As String
            Get
                Return m_BuilderRole
            End Get
            Set(ByVal Value As String)
                m_BuilderRole = value
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

        Public Sub New(ByVal DB As Database, ByVal BuilderRoleID As Integer)
            m_DB = DB
            m_BuilderRoleID = BuilderRoleID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM BuilderRole WHERE BuilderRoleID = " & DB.Number(BuilderRoleID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_BuilderRoleID = Core.GetInt(r.Item("BuilderRoleID"))
            m_BuilderRole = Core.GetString(r.Item("BuilderRole"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO BuilderRole (" _
             & " BuilderRole" _
             & ") VALUES (" _
             & m_DB.Quote(BuilderRole) _
             & ")"

            BuilderRoleID = m_DB.InsertSQL(SQL)

            Return BuilderRoleID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE BuilderRole SET " _
             & " BuilderRole = " & m_DB.Quote(BuilderRole) _
             & " WHERE BuilderRoleID = " & m_DB.quote(BuilderRoleID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM BuilderRole WHERE BuilderRoleID = " & m_DB.Number(BuilderRoleID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class BuilderRoleCollection
        Inherits GenericCollection(Of BuilderRoleRow)
    End Class

End Namespace


