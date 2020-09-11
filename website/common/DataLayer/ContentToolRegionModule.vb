Option Explicit On 

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class ContentToolRegionModuleRow
        Inherits ContentToolRegionModuleRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            MyBase.New(database)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal RegionModuleId As Integer)
            MyBase.New(database, RegionModuleId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal _Database As Database, ByVal RegionModuleId As Integer) As ContentToolRegionModuleRow
            Dim row As ContentToolRegionModuleRow

            row = New ContentToolRegionModuleRow(_Database, RegionModuleId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal _Database As Database, ByVal RegionModuleId As Integer)
            Dim row As ContentToolRegionModuleRow

            row = New ContentToolRegionModuleRow(_Database, RegionModuleId)
            row.Remove()
        End Sub

        'Custom Methods

    End Class

    Public MustInherit Class ContentToolRegionModuleRowBase
        Private m_DB As Database
        Private m_RegionModuleId As Integer = Nothing
        Private m_PageRegionId As Integer = Nothing
        Private m_SortOrder As Integer = Nothing
        Private m_ModuleId As Integer = Nothing
        Private m_Args As String = Nothing

        Public Property RegionModuleId() As Integer
            Get
                Return m_RegionModuleId
            End Get
            Set(ByVal Value As Integer)
                m_RegionModuleId = Value
            End Set
        End Property

        Public Property PageRegionId() As Integer
            Get
                Return m_PageRegionId
            End Get
            Set(ByVal Value As Integer)
                m_PageRegionId = Value
            End Set
        End Property

        Public Property SortOrder() As Integer
            Get
                Return m_SortOrder
            End Get
            Set(ByVal Value As Integer)
                m_SortOrder = Value
            End Set
        End Property

        Public Property ModuleId() As Integer
            Get
                Return m_ModuleId
            End Get
            Set(ByVal Value As Integer)
                m_ModuleId = Value
            End Set
        End Property

        Public Property Args() As String
            Get
                Return m_Args
            End Get
            Set(ByVal Value As String)
                m_Args = Value
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

        Public Sub New(ByVal database As Database, ByVal RegionModuleId As Integer)
            m_DB = database
            m_RegionModuleId = RegionModuleId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader = Nothing
            Dim SQL As String

            SQL = "SELECT * FROM ContentToolRegionModule WHERE RegionModuleId = " & DB.Quote(RegionModuleId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_RegionModuleId = Convert.ToInt32(r.Item("RegionModuleId"))
            If r.Item("PageRegionId") Is Convert.DBNull Then
                m_PageRegionId = Nothing
            Else
                m_PageRegionId = Convert.ToInt32(r.Item("PageRegionId"))
            End If
            If r.Item("SortOrder") Is Convert.DBNull Then
                m_SortOrder = Nothing
            Else
                m_SortOrder = Convert.ToInt32(r.Item("SortOrder"))
            End If
            If r.Item("ModuleId") Is Convert.DBNull Then
                m_ModuleId = Nothing
            Else
                m_ModuleId = Convert.ToInt32(r.Item("ModuleId"))
            End If
            If r.Item("Args") Is Convert.DBNull Then
                m_Args = Nothing
            Else
                m_Args = Convert.ToString(r.Item("Args"))
            End If
        End Sub 'Load

        Private ReadOnly Property InsertStatement() As String
            Get
                Dim SQL As String

                SQL = " INSERT INTO ContentToolRegionModule (" _
                 & " PageRegionId" _
                 & ",SortOrder" _
                 & ",ModuleId" _
                 & ",Args" _
                 & ") VALUES (" _
                 & m_DB.Quote(PageRegionId) _
                 & "," & m_DB.Quote(SortOrder) _
                 & "," & m_DB.Quote(ModuleId) _
                 & "," & m_DB.Quote(Args) _
                 & ")"

                Return SQL
            End Get
        End Property

        Public Overridable Sub Insert()
            m_DB.ExecuteSQL(InsertStatement)
        End Sub 'Insert

        Function AutoInsert() As Integer
            RegionModuleId = m_DB.InsertSQL(InsertStatement)
            Return RegionModuleId
        End Function


        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE ContentToolRegionModule SET " _
             & " PageRegionId = " & m_DB.Quote(PageRegionId) _
             & ",SortOrder = " & m_DB.Quote(SortOrder) _
             & ",ModuleId = " & m_DB.Quote(ModuleId) _
             & ",Args= " & m_DB.Quote(Args) _
             & " WHERE RegionModuleId = " & m_DB.Quote(RegionModuleId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM ContentToolRegionModule WHERE RegionModuleId = " & m_DB.Quote(RegionModuleId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

End Namespace


