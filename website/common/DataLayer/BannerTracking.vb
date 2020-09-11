Option Explicit On

Imports System
Imports System.Data
Imports System.Collections
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text

Namespace DataLayer

    Public Class BannerTrackingRow
        Inherits BannerTrackingRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            MyBase.New(database)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal TrackingId As Integer)
            MyBase.New(database, TrackingId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal _Database As Database, ByVal TrackingId As Integer) As BannerTrackingRow
            Dim row As BannerTrackingRow

            row = New BannerTrackingRow(_Database, TrackingId)
            row.Load()

            Return row
        End Function

        'Custom Methods
        Public Shared Sub AddImpression(ByVal db As Database, ByVal BannerId As Integer, ByVal CreateDate As DateTime)
            CreateDate = Month(CreateDate) & "/" & Day(CreateDate) & "/" & Year(CreateDate)
            Dim SQL As String = "update BannerTracking set ImpressionCount = ImpressionCount + 1 Where BannerId = " & BannerId & " and CreateDate = " & db.Quote(CreateDate)
            If db.ExecuteSQL(SQL) = 0 Then
                Dim row As BannerTrackingRow = New BannerTrackingRow(db)
                row.BannerId = BannerId
                row.CreateDate = CreateDate
                row.ImpressionCount = 1
                row.Insert()
            End If
        End Sub

        Public Shared Sub AddClick(ByVal db As Database, ByVal BannerId As Integer, ByVal CreateDate As DateTime)
            CreateDate = Month(CreateDate) & "/" & Day(CreateDate) & "/" & Year(CreateDate)
            Dim SQL As String = "update BannerTracking set ClickCount = ClickCount + 1 Where BannerId = " & BannerId & " and CreateDate = " & db.Quote(CreateDate)
            If db.ExecuteSQL(SQL) = 0 Then
                Dim row As BannerTrackingRow = New BannerTrackingRow(db)
                row.BannerId = BannerId
                row.CreateDate = CreateDate
                row.ClickCount = 1
                row.Insert()
            End If
        End Sub

    End Class

    Public MustInherit Class BannerTrackingRowBase
        Private m_DB As Database
        Private m_TrackingId As Integer = Nothing
        Private m_BannerId As Integer = Nothing
        Private m_ImpressionCount As Integer = Nothing
        Private m_ClickCount As Integer = Nothing
        Private m_CreateDate As DateTime = Nothing

        Public Property TrackingId() As Integer
            Get
                Return m_TrackingId
            End Get
            Set(ByVal Value As Integer)
                m_TrackingId = Value
            End Set
        End Property

        Public Property BannerId() As Integer
            Get
                Return m_BannerId
            End Get
            Set(ByVal Value As Integer)
                m_BannerId = Value
            End Set
        End Property

        Public Property ImpressionCount() As Integer
            Get
                Return m_ImpressionCount
            End Get
            Set(ByVal Value As Integer)
                m_ImpressionCount = Value
            End Set
        End Property

        Public Property ClickCount() As Integer
            Get
                Return m_ClickCount
            End Get
            Set(ByVal Value As Integer)
                m_ClickCount = Value
            End Set
        End Property

        Public Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
            Set(ByVal Value As DateTime)
                m_CreateDate = Value
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

        Public Sub New(ByVal database As Database, ByVal TrackingId As Integer)
            m_DB = database
            m_TrackingId = TrackingId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM BannerTracking WHERE TrackingId = " & DB.Quote(TrackingId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_BannerId = Convert.ToInt32(r.Item("BannerId"))
            If IsDBNull(r.Item("CreateDate")) Then
                m_CreateDate = Nothing
            Else
                m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            SQL = " INSERT INTO BannerTracking (" _
             & " BannerId" _
             & ",ImpressionCount" _
             & ",ClickCount" _
             & ",CreateDate" _
             & ") VALUES (" _
             & m_DB.Quote(BannerId) _
             & "," & ImpressionCount _
             & "," & ClickCount _
             & "," & m_DB.Quote(CreateDate) _
             & ")"

            TrackingId = m_DB.InsertSQL(SQL)

            Return TrackingId
        End Function

    End Class

    Public Class BannerTrackingCollection
        Inherits CollectionBase

        Public Sub New()
        End Sub

        Public Sub Add(ByVal BannerTracking As BannerTrackingRow)
            Me.List.Add(BannerTracking)
        End Sub

        Public Function Contains(ByVal BannerTracking As BannerTrackingRow) As Boolean
            Return Me.List.Contains(BannerTracking)
        End Function

        Public Function IndexOf(ByVal BannerTracking As BannerTrackingRow) As Integer
            Return Me.List.IndexOf(BannerTracking)
        End Function

        Public Sub Insert(ByVal Index As Integer, ByVal BannerTracking As BannerTrackingRow)
            Me.List.Insert(Index, BannerTracking)
        End Sub

        Default Public Property Item(ByVal Index As Integer) As BannerTrackingRow
            Get
                Return CType(Me.List.Item(Index), BannerTrackingRow)
            End Get

            Set(ByVal Value As BannerTrackingRow)
                Me.List(Index) = Value
            End Set
        End Property

        Public Sub Remove(ByVal BannerTracking As BannerTrackingRow)
            Me.List.Remove(BannerTracking)
        End Sub
    End Class

End Namespace
