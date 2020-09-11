Option Explicit On 

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class AdminAccessRow
        Inherits AdminAccessRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal Id As Integer)
            MyBase.New(DB, Id)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal Id As Integer) As AdminAccessRow
            Dim row As AdminAccessRow

            row = New AdminAccessRow(DB, Id)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal Id As Integer)
            Dim row As AdminAccessRow

            row = New AdminAccessRow(DB, Id)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Sub RemoveByGroup(ByVal DB As Database, ByVal GroupId As Integer)
            Dim SQL As String = "delete from AdminAccess where GroupId = " & DB.Quote(GroupId.ToString)
            DB.ExecuteSQL(SQL)
        End Sub

        Public Shared Function LoadSectionsWithoutPrivileges(ByVal DB As Database, ByVal GroupId As Integer) As DataTable
            Dim SQL As New StringBuilder

            'Bind Left List
            If GroupId <> 0 Then
                SQL.Append("SELECT * FROM AdminSection WHERE SectionId NOT IN")
                SQL.Append("(")
                SQL.Append("SELECT SectionId FROM AdminAccess WHERE GroupId = " & DB.Quote(GroupId.ToString))
                SQL.Append(") ORDER BY DESCRIPTION")
            Else
                SQL.Append("SELECT * FROM AdminSection ORDER BY DESCRIPTION")
            End If
            Return DB.GetDataTable(SQL.ToString)
        End Function

        Public Shared Function LoadSectionsWithPrivileges(ByVal DB As Database, ByVal GroupId As Integer) As DataTable
            Dim SQL As New StringBuilder

            SQL.Append("SELECT * FROM AdminSection WHERE SectionId IN ")
            SQL.Append("(")
            SQL.Append("SELECT SectionId FROM AdminAccess WHERE GroupId = " & DB.Quote(GroupId))
            SQL.Append(")")

            Return DB.GetDataTable(SQL.ToString)
        End Function

    End Class

    Public MustInherit Class AdminAccessRowBase
        Private m_DB As Database
        Private m_Id As Integer = Nothing
        Private m_SectionId As Integer = Nothing
        Private m_GroupId As Integer = Nothing

        Public Property Id() As Integer
            Get
                Return m_Id
            End Get
            Set(ByVal Value As Integer)
                m_Id = value
            End Set
        End Property

        Public Property SectionId() As Integer
            Get
                Return m_SectionId
            End Get
            Set(ByVal Value As Integer)
                m_SectionId = value
            End Set
        End Property

        Public Property GroupId() As Integer
            Get
                Return m_GroupId
            End Get
            Set(ByVal Value As Integer)
                m_GroupId = value
            End Set
        End Property

        Public Property DB() As Database
            Get
                DB = m_DB
            End Get
            Set(ByVal Value As Database)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            m_DB = database
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal Id As Integer)
            m_DB = database
            m_Id = Id
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM AdminAccess WHERE Id = " & DB.Quote(Id)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_SectionId = Convert.ToInt32(r.Item("SectionId"))
            m_GroupId = Convert.ToInt32(r.Item("GroupId"))
        End Sub 'Load

        Public Overridable Sub Insert()
            Dim SQL As String

            SQL = " INSERT INTO AdminAccess (" _
             & " SectionId" _
             & ",GroupId" _
             & ") VALUES (" _
             & m_DB.Quote(SectionId) _
             & "," & m_DB.Quote(GroupId) _
             & ")"

            m_DB.ExecuteSQL(SQL)
        End Sub 'Insert

        Function AutoInsert() As Integer
            Dim SQL As String = "SELECT SCOPE_IDENTITY()"

            Insert()
            Return m_DB.ExecuteScalar(SQL)
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE AdminAccess SET " _
             & " SectionId = " & m_DB.Quote(SectionId) _
             & ",GroupId = " & m_DB.Quote(GroupId) _
             & " WHERE Id = " & m_DB.Quote(Id)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM AdminAccess WHERE Id = " & m_DB.Quote(Id)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class AdminAccessCollection
        Inherits GenericCollection(Of AdminAccessRow)
    End Class

End Namespace


