Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public MustInherit Class TwoPriceStatusRowBase
        Private m_DB As Database
        Private m_TwoPriceStatusId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_Value As String = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_SortOrder As Integer = Nothing
        Private m_CreateDate As DateTime = Nothing

        Public Property TwoPriceStatusId As Integer
            Get
                Return m_TwoPriceStatusId
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceStatusId = value
            End Set
        End Property

        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(ByVal Value As String)
                m_Name = value
            End Set
        End Property

        Public Property Value As String
            Get
                Return m_Value
            End Get
            Set(ByVal Value As String)
                m_Value = value
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

        Public Property SortOrder As Integer
            Get
                Return m_SortOrder
            End Get
            Set(ByVal Value As Integer)
                m_SortOrder = value
            End Set
        End Property

        Public ReadOnly Property CreateDate As DateTime
            Get
                Return m_CreateDate
            End Get
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

        Public Sub New(ByVal DB As Database, TwoPriceStatusId As Integer)
            m_DB = DB
            m_TwoPriceStatusId = TwoPriceStatusId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM TwoPriceStatus WHERE TwoPriceStatusId = " & DB.Number(TwoPriceStatusId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_TwoPriceStatusId = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_TwoPriceStatusId = Core.GetInt(r.Item("TwoPriceStatusId"))
            m_Name = Core.GetString(r.Item("Name"))
            m_Value = Core.GetString(r.Item("Value"))
            m_IsActive = Core.GetBoolean(r.Item("IsActive"))
            m_CreateDate = Core.GetDate(r.Item("CreateDate"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from TwoPriceStatus order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO TwoPriceStatus (" _
             & " Name" _
             & ",Value" _
             & ",IsActive" _
             & ",SortOrder" _
             & ",CreateDate" _
             & ") VALUES (" _
             & m_DB.Quote(Name) _
             & "," & m_DB.Quote(Value) _
             & "," & CInt(IsActive) _
             & "," & MaxSortOrder _
             & "," & m_DB.NullQuote(Now) _
             & ")"

            TwoPriceStatusId = m_DB.InsertSQL(SQL)

            Return TwoPriceStatusId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE TwoPriceStatus WITH (ROWLOCK) SET " _
             & " Name = " & m_DB.Quote(Name) _
             & ",Value = " & m_DB.Quote(Value) _
             & ",IsActive = " & CInt(IsActive) _
             & " WHERE TwoPriceStatusId = " & m_DB.quote(TwoPriceStatusId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class TwoPriceStatusCollection
        Inherits GenericCollection(Of TwoPriceStatusRow)
    End Class

End Namespace
