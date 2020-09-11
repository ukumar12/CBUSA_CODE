Option Explicit On 

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class AdminSectionRow
        Inherits AdminSectionRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal SectionId As Integer)
            MyBase.New(DB, SectionId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal SectionId As Integer) As AdminSectionRow
            Dim row As AdminSectionRow

            row = New AdminSectionRow(DB, SectionId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal SectionId As Integer)
            Dim row As AdminSectionRow

            row = New AdminSectionRow(DB, SectionId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetPermissionList(ByVal DB As Database, ByVal AdminId As Integer) As AdminSectionCollection
            Dim sSQL As String
            Dim collection As New AdminSectionCollection

            sSQL = " SELECT DISTINCT ads.SectionId, ads.Code, ads.Description from Admin a, AdminAdminGroup aag, AdminAccess aa, AdminSection ads" _
                 & " WHERE a.AdminId = aag.AdminId" _
                 & " AND aag.GroupId = aa.GroupId" _
                 & " AND aa.SectionId = ads.SectionId" _
                 & " AND a.AdminId = " & DB.Quote(AdminId.ToString)

            Dim r As SqlDataReader = DB.GetReader(sSQL)
            Dim row As AdminSectionRow
            While r.Read
                row = New AdminSectionRow(DB)
                row.Load(r)
                collection.Add(row)
            End While
            r.Close()

            Return collection
        End Function

    End Class

    Public MustInherit Class AdminSectionRowBase
        Private m_DB As Database
        Private m_SectionId As Integer = Nothing
        Private m_Code As String = Nothing
        Private m_Description As String = Nothing


        Public Property SectionId() As Integer
            Get
                Return m_SectionId
            End Get
            Set(ByVal Value As Integer)
                m_SectionId = value
            End Set
        End Property

        Public Property Code() As String
            Get
                Return m_Code
            End Get
            Set(ByVal Value As String)
                m_Code = value
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

        Public Sub New(ByVal database As Database)
            m_DB = database
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal SectionId As Integer)
            m_DB = database
            m_SectionId = SectionId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM AdminSection WHERE SectionId = " & DB.Quote(SectionId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_Code = Convert.ToString(r.Item("Code"))
            m_Description = Convert.ToString(r.Item("Description"))
        End Sub 'Load

        Public Overridable Sub Insert()
            Dim SQL As String

            SQL = " INSERT INTO AdminSection (" _
             & " Code" _
             & ",Description" _
             & ") VALUES (" _
             & m_DB.Quote(Code) _
             & "," & m_DB.Quote(Description) _
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

            SQL = " UPDATE AdminSection SET " _
             & " Code = " & m_DB.Quote(Code) _
             & ",Description = " & m_DB.Quote(Description) _
             & " WHERE SectionId = " & m_DB.Quote(SectionId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM AdminSection WHERE SectionId = " & m_DB.Quote(SectionId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class AdminSectionCollection
        Inherits GenericCollection(Of AdminSectionRow)
    End Class

End Namespace


