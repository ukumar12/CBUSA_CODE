Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class ResponseCaptureRow
        Inherits ResponseCaptureRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ResponseCaptureId As Integer)
            MyBase.New(DB, ResponseCaptureId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal ResponseCaptureId As Integer) As ResponseCaptureRow
            Dim row As ResponseCaptureRow

            row = New ResponseCaptureRow(DB, ResponseCaptureId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ResponseCaptureId As Integer)
            Dim row As ResponseCaptureRow

            row = New ResponseCaptureRow(DB, ResponseCaptureId)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from ResponseCapture"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class ResponseCaptureRowBase
        Private m_DB As Database
        Private m_ResponseCaptureId As Integer = Nothing
        Private m_OrderId As Integer = Nothing
        Private m_ResponseCapture As String = Nothing
        Private m_CreateDate As DateTime = Nothing


        Public Property ResponseCaptureId() As Integer
            Get
                Return m_ResponseCaptureId
            End Get
            Set(ByVal Value As Integer)
                m_ResponseCaptureId = value
            End Set
        End Property

        Public Property OrderId() As Integer
            Get
                Return m_OrderId
            End Get
            Set(ByVal Value As Integer)
                m_OrderId = value
            End Set
        End Property

        Public Property ResponseCapture() As String
            Get
                Return m_ResponseCapture
            End Get
            Set(ByVal Value As String)
                m_ResponseCapture = value
            End Set
        End Property


        Public ReadOnly Property CreateDate() As DateTime
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

        Public Sub New(ByVal DB As Database, ByVal ResponseCaptureId As Integer)
            m_DB = DB
            m_ResponseCaptureId = ResponseCaptureId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM ResponseCapture WHERE ResponseCaptureId = " & DB.Number(ResponseCaptureId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_ResponseCaptureId = Convert.ToInt32(r.Item("ResponseCaptureId"))
            m_OrderId = Convert.ToInt32(r.Item("OrderId"))
            m_ResponseCapture = Convert.ToString(r.Item("ResponseCapture"))
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO ResponseCapture (" _
             & " OrderId" _
             & ",ResponseCapture" _
             & ",CreateDate" _
             & ") VALUES (" _
             & m_DB.NullNumber(OrderId) _
             & "," & m_DB.Quote(ResponseCapture) _
             & "," & m_DB.NullQuote(Now) _
             & ")"

            ResponseCaptureId = m_DB.InsertSQL(SQL)

            Return ResponseCaptureId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE ResponseCapture SET " _
             & " OrderId = " & m_DB.NullNumber(OrderId) _
             & ",ResponseCapture = " & m_DB.Quote(ResponseCapture) _
             & " WHERE ResponseCaptureId = " & m_DB.Quote(ResponseCaptureId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM ResponseCapture WHERE ResponseCaptureId = " & m_DB.Number(ResponseCaptureId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class ResponseCaptureCollection
        Inherits GenericCollection(Of ResponseCaptureRow)
    End Class

End Namespace

