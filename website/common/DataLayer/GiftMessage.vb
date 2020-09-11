Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class GiftMessageRow
        Inherits GiftMessageRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal GiftMessageId As Integer)
            MyBase.New(DB, GiftMessageId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal GiftMessageId As Integer) As GiftMessageRow
            Dim row As GiftMessageRow

            row = New GiftMessageRow(DB, GiftMessageId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal GiftMessageId As Integer)
            Dim row As GiftMessageRow

            row = New GiftMessageRow(DB, GiftMessageId)
            row.Remove()
        End Sub
        Public Shared Function GetList(ByVal Db As Database) As DataTable
            Dim dt As DataTable
            dt = Db.GetDataTable("select * from GiftMessage order by SortOrder asc")
            Return dt
        End Function


        'Custom Methods

    End Class

    Public MustInherit Class GiftMessageRowBase
        Private m_DB As Database
        Private m_GiftMessageId As Integer = Nothing
        Private m_GiftMessage As String = Nothing
        Private m_GiftMessageLabel As String = Nothing
        Private m_SortOrder As Integer = Nothing


        Public Property GiftMessageId() As Integer
            Get
                Return m_GiftMessageId
            End Get
            Set(ByVal Value As Integer)
                m_GiftMessageId = value
            End Set
        End Property

        Public Property GiftMessage() As String
            Get
                Return m_GiftMessage
            End Get
            Set(ByVal Value As String)
                m_GiftMessage = value
            End Set
        End Property
        Public Property GiftMessageLabel() As String
            Get
                Return m_GiftMessageLabel
            End Get
            Set(ByVal Value As String)
                m_GiftMessageLabel = Value
            End Set
        End Property

        Public Property SortOrder() As Integer
            Get
                Return m_SortOrder
            End Get
            Set(ByVal Value As Integer)
                m_SortOrder = value
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

        Public Sub New(ByVal DB As Database, ByVal GiftMessageId As Integer)
            m_DB = DB
            m_GiftMessageId = GiftMessageId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM GiftMessage WHERE GiftMessageId = " & DB.Number(GiftMessageId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_GiftMessageId = Convert.ToInt32(r.Item("GiftMessageId"))
            m_GiftMessage = Convert.ToString(r.Item("GiftMessage"))
            m_GiftMessageLabel = Convert.ToString(r.Item("GiftMessageLabel"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from GiftMessage order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO GiftMessage (" _
             & " GiftMessage" _
             & ",GiftMessageLabel" _
             & ",SortOrder" _
             & ") VALUES (" _
             & m_DB.Quote(GiftMessage) _
             & "," & m_DB.Quote(GiftMessageLabel) _
             & "," & MaxSortOrder _
             & ")"

            GiftMessageId = m_DB.InsertSQL(SQL)

            Return GiftMessageId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE GiftMessage SET " _
             & " GiftMessage = " & m_DB.Quote(GiftMessage) _
             & " ,GiftMessageLabel = " & m_DB.Quote(GiftMessageLabel) _
             & " WHERE GiftMessageId = " & m_DB.Quote(GiftMessageId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM GiftMessage WHERE GiftMessageId = " & m_DB.Number(GiftMessageId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class GiftMessageCollection
        Inherits GenericCollection(Of GiftMessageRow)
    End Class

End Namespace



