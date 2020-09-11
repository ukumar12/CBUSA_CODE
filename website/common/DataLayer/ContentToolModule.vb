Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class ContentToolModuleRow
        Inherits ContentToolModuleRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            MyBase.New(database)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal ModuleId As Integer)
            MyBase.New(database, ModuleId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal _Database As Database, ByVal ModuleId As Integer) As ContentToolModuleRow
            Dim row As ContentToolModuleRow

            row = New ContentToolModuleRow(_Database, ModuleId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal _Database As Database, ByVal ModuleId As Integer)
            Dim row As ContentToolModuleRow

            row = New ContentToolModuleRow(_Database, ModuleId)
            row.Remove()
        End Sub

        'Custom Methods

    End Class

    Public MustInherit Class ContentToolModuleRowBase
        Private m_DB As Database
        Private m_ModuleId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_Args As String = Nothing
        Private m_ControlURL As String = Nothing
        Private m_MinWidth As Integer = Nothing
        Private m_MaxWidth As Integer = Nothing
        Private m_HTML As String = Nothing
        Private m_SkipIndexing As Boolean = False

        Public Property ModuleId() As Integer
            Get
                Return m_ModuleId
            End Get
            Set(ByVal Value As Integer)
                m_ModuleId = Value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal Value As String)
                m_Name = value
            End Set
        End Property

        Public Property Args() As String
            Get
                Return m_Args
            End Get
            Set(ByVal Value As String)
                m_Args = value
            End Set
        End Property

        Public Property ControlURL() As String
            Get
                Return m_ControlURL
            End Get
            Set(ByVal Value As String)
                m_ControlURL = value
            End Set
        End Property

        Public Property MinWidth() As Integer
            Get
                Return m_MinWidth
            End Get
            Set(ByVal Value As Integer)
                m_MinWidth = value
            End Set
        End Property

        Public Property MaxWidth() As Integer
            Get
                Return m_MaxWidth
            End Get
            Set(ByVal Value As Integer)
                m_MaxWidth = value
            End Set
        End Property

        Public Property HTML() As String
            Get
                Return m_HTML
            End Get
            Set(ByVal Value As String)
                m_HTML = value
            End Set
        End Property

        Public Property SkipIndexing() As Boolean
            Get
                Return m_SkipIndexing
            End Get
            Set(ByVal Value As Boolean)
                m_SkipIndexing = Value
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

        Public Sub New(ByVal database As Database)
            m_DB = database
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal ModuleId As Integer)
            m_DB = database
            m_ModuleId = ModuleId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM ContentToolModule WHERE ModuleId = " & DB.Quote(ModuleId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_ModuleId = Convert.ToInt32(r.Item("ModuleId"))
            m_Name = Convert.ToString(r.Item("Name"))
            If IsDBNull(r.Item("Args")) Then
                m_Args = Nothing
            Else
                m_Args = Convert.ToString(r.Item("Args"))
            End If
            If IsDBNull(r.Item("ControlURL")) Then
                m_ControlURL = Nothing
            Else
                m_ControlURL = Convert.ToString(r.Item("ControlURL"))
            End If
            If IsDBNull(r.Item("MinWidth")) Then
                m_MinWidth = Nothing
            Else
                m_MinWidth = Convert.ToInt32(r.Item("MinWidth"))
            End If
            If IsDBNull(r.Item("MaxWidth")) Then
                m_MaxWidth = Nothing
            Else
                m_MaxWidth = Convert.ToInt32(r.Item("MaxWidth"))
            End If
            m_SkipIndexing = Convert.ToBoolean(r.Item("SkipIndexing"))
            If IsDBNull(r.Item("HTML")) Then
                m_HTML = Nothing
            Else
                m_HTML = Convert.ToString(r.Item("HTML"))
            End If
        End Sub 'Load

        Private ReadOnly Property InsertStatement() As String
            Get
                Dim SQL As String

                SQL = " INSERT INTO ContentToolModule (" _
                 & " Name" _
                 & ",Args" _
                 & ",ControlURL" _
                 & ",MinWidth" _
                 & ",MaxWidth" _
                 & ",SkipIndexing" _
                 & ",HTML" _
                 & ") VALUES (" _
                 & m_DB.Quote(Name) _
                 & "," & m_DB.Quote(Args) _
                 & "," & m_DB.Quote(ControlURL) _
                 & "," & m_DB.Quote(MinWidth) _
                 & "," & m_DB.Quote(MaxWidth) _
                 & "," & CInt(SkipIndexing) _
                 & "," & m_DB.Quote(HTML) _
                 & ")"

                Return SQL
            End Get
        End Property

        Public Overridable Sub Insert()
            m_DB.ExecuteSQL(InsertStatement)
        End Sub 'Insert

        Function AutoInsert() As Integer
            ModuleId = m_DB.InsertSQL(InsertStatement)
            Return ModuleId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE ContentToolModule SET " _
             & " Name = " & m_DB.Quote(Name) _
             & ",Args = " & m_DB.Quote(Args) _
             & ",ControlURL = " & m_DB.Quote(ControlURL) _
             & ",MinWidth = " & m_DB.Quote(MinWidth) _
             & ",MaxWidth = " & m_DB.Quote(MaxWidth) _
             & ",SkipIndexing = " & CInt(SkipIndexing) _
             & ",HTML = " & m_DB.Quote(HTML) _
             & " WHERE ModuleId = " & m_DB.Quote(ModuleId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM ContentToolModule WHERE ModuleId = " & m_DB.Quote(ModuleId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class ContentToolModuleCollection
        Inherits GenericCollection(Of ContentToolModuleRow)
    End Class

End Namespace


