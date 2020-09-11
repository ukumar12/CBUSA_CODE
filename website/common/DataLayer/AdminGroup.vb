Option Explicit On 

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class AdminGroupRow
        Inherits AdminGroupRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal GroupId As Integer)
            MyBase.New(DB, GroupId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal GroupId As Integer) As AdminGroupRow
            Dim row As AdminGroupRow

            row = New AdminGroupRow(DB, GroupId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal GroupId As Integer)
            Dim row As AdminGroupRow

            row = New AdminGroupRow(DB, GroupId)
            row.Remove()
        End Sub

        'Custom Methods
    End Class

    Public MustInherit Class AdminGroupRowBase
        Private m_DB As Database
        Private m_GroupId As Integer = Nothing
        Private m_Description As String = Nothing

        Public Property GroupId() As Integer
            Get
                Return m_GroupId
            End Get
            Set(ByVal Value As Integer)
                m_GroupId = Value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return m_Description
            End Get
            Set(ByVal Value As String)
                m_Description = value
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

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal GroupId As Integer)
            m_DB = DB
            m_GroupId = GroupId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM AdminGroup WHERE GroupId = " & DB.Quote(GroupId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_Description = Convert.ToString(r.Item("Description"))
        End Sub 'Load

        Public Overridable Sub Insert()
            Dim SQL As String

            SQL = " INSERT INTO AdminGroup (" _
             & " Description" _
             & ") VALUES (" _
             & m_DB.Quote(Description) _
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

            SQL = " UPDATE AdminGroup SET " _
             & " Description = " & m_DB.Quote(Description) _
             & " WHERE GroupId = " & m_DB.Quote(GroupId)

            m_DB.ExecuteSQL(SQL)
        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM AdminGroup WHERE GroupId = " & m_DB.Quote(GroupId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class AdminGroupCollection
        Inherits GenericCollection(Of AdminGroupRow)
    End Class

End Namespace


