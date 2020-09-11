Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class BannerGroupRow
        Inherits BannerGroupRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BannerGroupId As Integer)
            MyBase.New(DB, BannerGroupId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal BannerGroupId As Integer) As BannerGroupRow
            Dim row As BannerGroupRow

            row = New BannerGroupRow(DB, BannerGroupId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal BannerGroupId As Integer)
            Dim row As BannerGroupRow

            row = New BannerGroupRow(DB, BannerGroupId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetGroupList(ByVal DB As Database, ByVal Width As Integer) As DataTable
            Return DB.GetDataTable("select * from BannerGroup where MinWidth <= " & Width & " and MaxWidth >= " & Width)
        End Function

        Public Shared Function GetSelectedBannerListByBanner(ByVal Db As Database, ByVal BannerId As Integer, ByVal Width As Integer) As DataTable
            Return Db.GetDataTable("select bbg.*, (select sum(weight) as TotalWeight from BannerBannerGroup bbg2, Banner b2 where b2.IsActive = 1 and b.BannerId = b2.BannerId and bbg2.BannerGroupId = bg.BannerGroupId group by BannerGroupId) as TotalWeight from Banner b, BannerGroup bg, BannerBannerGroup bbg where b.BannerId = " & BannerId & " and bbg.BannerId = " & BannerId & " and bg.BannerGroupId = bbg.BannerGroupId and MinWidth <= " & Width & " and MaxWidth >= " & Width)
        End Function

        Public Shared Function GetSelectedBannerListByBannerGroup(ByVal Db As Database, ByVal BannerGroupId As Integer, ByVal MinWidth As Integer, ByVal MaxWidth As Integer) As DataTable
            Return Db.GetDataTable("select bbg.*, (select sum(weight) as TotalWeight from BannerBannerGroup bbg2, Banner b2 where b2.IsActive = 1 and bbg2.BannerGroupId = " & BannerGroupId & " and b.BannerId = b2.BannerId and bbg2.BannerGroupId = bg.BannerGroupId group by BannerGroupId) as TotalWeight from Banner b, BannerGroup bg, BannerBannerGroup bbg where b.BannerId = bbg.BannerId and bbg.BannerGroupId = " & BannerGroupId & " and bg.BannerGroupId = bbg.BannerGroupId and bg.BannerGroupId = " & BannerGroupId & " and b.Width >= " & MinWidth & " and b.Width <= " & MaxWidth)
        End Function

        Public Shared Function GetList(ByVal DB As Database) As DataTable
            Return DB.GetDataTable("select * from BannerGroup")
        End Function

    End Class

    Public MustInherit Class BannerGroupRowBase
        Private m_DB As Database
        Private m_BannerGroupId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_MinWidth As Integer = Nothing
        Private m_MaxWidth As Integer = Nothing


        Public Property BannerGroupId() As Integer
            Get
                Return m_BannerGroupId
            End Get
            Set(ByVal Value As Integer)
                m_BannerGroupId = value
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

        Public Sub New(ByVal DB As Database, ByVal BannerGroupId As Integer)
            m_DB = DB
            m_BannerGroupId = BannerGroupId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM BannerGroup WHERE BannerGroupId = " & DB.Number(BannerGroupId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_BannerGroupId = Convert.ToInt32(r.Item("BannerGroupId"))
            m_Name = Convert.ToString(r.Item("Name"))
            m_MinWidth = Convert.ToInt32(r.Item("MinWidth"))
            m_MaxWidth = Convert.ToInt32(r.Item("MaxWidth"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO BannerGroup (" _
             & " Name" _
             & ",MinWidth" _
             & ",MaxWidth" _
             & ") VALUES (" _
             & m_DB.Quote(Name) _
             & "," & m_DB.Number(MinWidth) _
             & "," & m_DB.Number(MaxWidth) _
             & ")"

            BannerGroupId = m_DB.InsertSQL(SQL)

            Return BannerGroupId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE BannerGroup SET " _
             & " Name = " & m_DB.Quote(Name) _
             & ",MinWidth = " & m_DB.Number(MinWidth) _
             & ",MaxWidth = " & m_DB.Number(MaxWidth) _
             & " WHERE BannerGroupId = " & m_DB.quote(BannerGroupId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM BannerBannerGroup WHERE BannerGroupId = " & m_DB.Number(BannerGroupId)
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM BannerGroup WHERE BannerGroupId = " & m_DB.Quote(BannerGroupId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class BannerGroupCollection
        Inherits GenericCollection(Of BannerGroupRow)
    End Class

End Namespace


