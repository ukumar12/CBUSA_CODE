Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class BannerOrderTrackingRow
        Inherits BannerOrderTrackingRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal TrackingId As Integer)
            MyBase.New(DB, TrackingId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal TrackingId As Integer) As BannerOrderTrackingRow
            Dim row As BannerOrderTrackingRow

            row = New BannerOrderTrackingRow(DB, TrackingId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal TrackingId As Integer)
            Dim row As BannerOrderTrackingRow

            row = New BannerOrderTrackingRow(DB, TrackingId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Sub AddClick(ByVal DB As Database, ByVal BannerId As Integer, ByVal OrderId As Integer)
            Dim id As Integer = DB.ExecuteScalar("select BannerId from BannerOrderTracking where BannerId = " & BannerId & " and OrderId = " & OrderId)
            If id > 0 Then
                Exit Sub
            End If

            Dim dbRow As BannerOrderTrackingRow = New BannerOrderTrackingRow(DB)
            dbRow.BannerId = BannerId
            dbRow.OrderId = OrderId
            dbRow.Insert()
        End Sub

    End Class

    Public MustInherit Class BannerOrderTrackingRowBase
        Private m_DB As Database
        Private m_TrackingId As Integer = Nothing
        Private m_BannerId As Integer = Nothing
        Private m_OrderId As Integer = Nothing
        Private m_CreateDate As DateTime = Nothing


        Public Property TrackingId() As Integer
            Get
                Return m_TrackingId
            End Get
            Set(ByVal Value As Integer)
                m_TrackingId = value
            End Set
        End Property

        Public Property BannerId() As Integer
            Get
                Return m_BannerId
            End Get
            Set(ByVal Value As Integer)
                m_BannerId = value
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

        Public Sub New(ByVal DB As Database, ByVal TrackingId As Integer)
            m_DB = DB
            m_TrackingId = TrackingId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM BannerOrderTracking WHERE TrackingId = " & DB.Number(TrackingId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_TrackingId = Convert.ToInt32(r.Item("TrackingId"))
            m_BannerId = Convert.ToInt32(r.Item("BannerId"))
            m_OrderId = Convert.ToInt32(r.Item("OrderId"))
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO BannerOrderTracking (" _
             & " BannerId" _
             & ",OrderId" _
             & ",CreateDate" _
             & ") VALUES (" _
             & m_DB.NullNumber(BannerId) _
             & "," & m_DB.NullNumber(OrderId) _
             & "," & m_DB.NullQuote(Now) _
             & ")"

            TrackingId = m_DB.InsertSQL(SQL)

            Return TrackingId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE BannerOrderTracking SET " _
             & " BannerId = " & m_DB.NullNumber(BannerId) _
             & ",OrderId = " & m_DB.NullNumber(OrderId) _
             & " WHERE TrackingId = " & m_DB.quote(TrackingId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM BannerOrderTracking WHERE TrackingId = " & m_DB.Number(TrackingId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class BannerOrderTrackingCollection
        Inherits GenericCollection(Of BannerOrderTrackingRow)
    End Class

End Namespace


