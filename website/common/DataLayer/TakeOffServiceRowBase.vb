Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public MustInherit Class TakeOffServiceRowBase
        Private m_DB As Database
        Private m_TakeOffServiceID As Integer = Nothing
        Private m_Title As String = Nothing
        Private m_Description As String = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_Created As DateTime = Nothing
        Private m_Updated As DateTime = Nothing

        Public Property TakeOffServiceID As Integer
            Get
                Return m_TakeOffServiceID
            End Get
            Set(ByVal Value As Integer)
                m_TakeOffServiceID = value
            End Set
        End Property

        Public Property Title As String
            Get
                Return m_Title
            End Get
            Set(ByVal Value As String)
                m_Title = value
            End Set
        End Property

        Public Property Description As String
            Get
                Return m_Description
            End Get
            Set(ByVal Value As String)
                m_Description = value
            End Set
        End Property

        Public Property IsActive As Boolean
            Get
                Return m_IsActive
            End Get
            Set(ByVal Value As Boolean)
                m_IsActive = value
            End Set
        End Property

        Public Property Created As DateTime
            Get
                Return m_Created
            End Get
            Set(ByVal Value As DateTime)
                m_Created = value
            End Set
        End Property

        Public Property Updated As DateTime
            Get
                Return m_Updated
            End Get
            Set(ByVal Value As DateTime)
                m_Updated = value
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

        Public Sub New(ByVal DB As Database, TakeOffServiceID As Integer)
            m_DB = DB
            m_TakeOffServiceID = TakeOffServiceID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM TakeOffService WHERE TakeOffServiceID = " & DB.Number(TakeOffServiceID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_TakeOffServiceID = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_TakeOffServiceID = Core.GetInt(r.Item("TakeOffServiceID"))
            m_Title = Core.GetString(r.Item("Title"))
            m_Description = Core.GetString(r.Item("Description"))
            m_IsActive = Core.GetBoolean(r.Item("IsActive"))
            m_Created = Core.GetDate(r.Item("Created"))
            m_Updated = Core.GetDate(r.Item("Updated"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO TakeOffService (" _
             & " Title" _
             & ",Description" _
             & ",IsActive" _
             & ") VALUES (" _
             & m_DB.Quote(Title) _
             & "," & m_DB.Quote(Description) _
             & "," & CInt(IsActive) _
             & ")"

            TakeOffServiceID = m_DB.InsertSQL(SQL)

            Return TakeOffServiceID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE TakeOffService WITH (ROWLOCK) SET " _
             & " Title = " & m_DB.Quote(Title) _
             & ",Description = " & m_DB.Quote(Description) _
             & ",IsActive = " & CInt(IsActive) _
             & ",Updated = " & m_DB.NullQuote(Now) _
             & " WHERE TakeOffServiceID = " & m_DB.Quote(TakeOffServiceID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class TakeOffServiceCollection
        Inherits GenericCollection(Of TakeOffServiceRow)
    End Class

End Namespace

