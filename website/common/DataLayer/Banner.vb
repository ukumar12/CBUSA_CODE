Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class BannerRow
        Inherits BannerRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BannerId As Integer)
            MyBase.New(DB, BannerId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal BannerId As Integer) As BannerRow
            Dim row As BannerRow

            row = New BannerRow(DB, BannerId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal BannerId As Integer)
            Dim row As BannerRow

            row = New BannerRow(DB, BannerId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetActiveBanners(ByVal DB As Database) As DataTable
            Return DB.GetDataTable("select BannerId, Name + ' (' + filename + ')' as Name from Banner b where b.IsActive = 1 order by Name")
        End Function

        Public Shared Function GetList(ByVal DB As Database, ByVal MinWidth As Integer, ByVal MaxWidth As Integer) As DataTable
            Return DB.GetDataTable("select * from Banner b where b.IsActive = 1 and Width >= " & MinWidth & " and Width <= " & MaxWidth & "order by Name")
        End Function

        Public Shared Function GetRandomRow(ByVal DB As Database, ByVal BannerGroupId As Integer) As BannerRow
            Return GetRandomRow(DB, BannerGroupId, String.Empty)
        End Function

        Public Shared Function GetRandomRow(ByVal DB As Database, ByVal BannerGroupId As Integer, ByVal Excluded As String) As BannerRow
            Dim SQL As String = String.Empty

            SQL &= " select b.*, bbg.* from Banner b, BannerBannerGroup bbg where b.BannerId = bbg.BannerId"
            SQL &= " and b.IsActive = 1 and BannerGroupId = " & BannerGroupId
            SQL &= " and case when bbg.DateFrom is null then " & DB.Quote(DateTime.Today) & " else bbg.DateFrom end <= " & DB.Quote(DateTime.Today)
            SQL &= " and case when bbg.DateTo is null then " & DB.Quote(DateTime.Today) & " else bbg.DateTo end >= " & DB.Quote(DateTime.Today)
            If Not Excluded = String.Empty Then
                SQL &= " AND b.BannerId NOT IN " & DB.NumberMultiple(Excluded)
            End If
            SQL &= " order by b.BannerId"

            Dim dtBanners As DataTable = DB.GetDataTable(SQL)
            Dim dbBanner As BannerRow = New BannerRow(DB)

            Dim TotalWeight As Integer = 0
            For Each row As DataRow In dtBanners.Rows
                TotalWeight += row("Weight")
            Next

            Dim rnd As Random = New Random(CInt(DateTime.Now.Millisecond))
            Dim Index As Integer = rnd.Next(TotalWeight)

            Dim Current As Integer = 0
            For Each row As DataRow In dtBanners.Rows
                If Index >= Current AndAlso Index <= Current + row("Weight") Then
                    dbBanner.Load(row)
                    Exit For
                End If
                Current += row("Weight")
            Next
            Return dbBanner
        End Function


    End Class

    Public MustInherit Class BannerRowBase
        Private m_DB As Database
        Private m_BannerId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_Link As String = Nothing
        Private m_FileName As String = Nothing
        Private m_BannerType As String = Nothing
        Private m_Width As Integer = Nothing
        Private m_Height As Integer = Nothing
        Private m_AltText As String = Nothing
        Private m_HTML As String = Nothing
        Private m_Target As String = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_IsOrderTracking As Boolean = Nothing

        Public Property BannerId() As Integer
            Get
                Return m_BannerId
            End Get
            Set(ByVal Value As Integer)
                m_BannerId = value
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

        Public Property Link() As String
            Get
                Return m_Link
            End Get
            Set(ByVal Value As String)
                m_Link = value
            End Set
        End Property

        Public Property FileName() As String
            Get
                Return m_FileName
            End Get
            Set(ByVal Value As String)
                m_FileName = Value
            End Set
        End Property

        Public Property BannerType() As String
            Get
                Return m_BannerType
            End Get
            Set(ByVal Value As String)
                m_BannerType = Value
            End Set
        End Property

        Public Property Width() As Integer
            Get
                Return m_Width
            End Get
            Set(ByVal Value As Integer)
                m_Width = Value
            End Set
        End Property

        Public Property Height() As Integer
            Get
                Return m_Height
            End Get
            Set(ByVal Value As Integer)
                m_Height = Value
            End Set
        End Property

        Public Property AltText() As String
            Get
                Return m_AltText
            End Get
            Set(ByVal Value As String)
                m_AltText = value
            End Set
        End Property

        Public Property HTML() As String
            Get
                Return m_HTML
            End Get
            Set(ByVal Value As String)
                m_HTML = Value
            End Set
        End Property

        Public Property Target() As String
            Get
                Return m_Target
            End Get
            Set(ByVal Value As String)
                m_Target = value
            End Set
        End Property

        Public Property IsActive() As Boolean
            Get
                Return m_IsActive
            End Get
            Set(ByVal Value As Boolean)
                m_IsActive = value
            End Set
        End Property

        Public Property IsOrderTracking() As Boolean
            Get
                Return m_IsOrderTracking
            End Get
            Set(ByVal Value As Boolean)
                m_IsOrderTracking = Value
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

        Public Sub New(ByVal DB As Database, ByVal BannerId As Integer)
            m_DB = DB
            m_BannerId = BannerId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM Banner WHERE BannerId = " & DB.Number(BannerId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As DataRow)
            m_BannerId = Convert.ToInt32(r.Item("BannerId"))
            m_Name = Convert.ToString(r.Item("Name"))
            m_Link = Convert.ToString(r.Item("Link"))
            m_FileName = Convert.ToString(r.Item("FileName"))
            m_BannerType = Convert.ToString(r.Item("BannerType"))
            m_Width = Convert.ToInt32(r.Item("Width"))
            m_Height = Convert.ToInt32(r.Item("Height"))
            If IsDBNull(r.Item("AltText")) Then
                m_AltText = Nothing
            Else
                m_AltText = Convert.ToString(r.Item("AltText"))
            End If
            If IsDBNull(r.Item("HTML")) Then
                m_HTML = Nothing
            Else
                m_HTML = Convert.ToString(r.Item("HTML"))
            End If
            If IsDBNull(r.Item("Target")) Then
                m_Target = Nothing
            Else
                m_Target = Convert.ToString(r.Item("Target"))
            End If
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
            m_IsOrderTracking = Convert.ToBoolean(r.Item("IsOrderTracking"))
        End Sub 'Load

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_BannerId = Convert.ToInt32(r.Item("BannerId"))
            m_Name = Convert.ToString(r.Item("Name"))
            m_Link = Convert.ToString(r.Item("Link"))
            m_FileName = Convert.ToString(r.Item("FileName"))
            m_BannerType = Convert.ToString(r.Item("BannerType"))
            m_Width = Convert.ToInt32(r.Item("Width"))
            m_Height = Convert.ToInt32(r.Item("Height"))
            If IsDBNull(r.Item("AltText")) Then
                m_AltText = Nothing
            Else
                m_AltText = Convert.ToString(r.Item("AltText"))
            End If
            If IsDBNull(r.Item("HTML")) Then
                m_HTML = Nothing
            Else
                m_HTML = Convert.ToString(r.Item("HTML"))
            End If
            If IsDBNull(r.Item("Target")) Then
                m_Target = Nothing
            Else
                m_Target = Convert.ToString(r.Item("Target"))
            End If
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
            m_IsOrderTracking = Convert.ToBoolean(r.Item("IsOrderTracking"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO Banner (" _
             & " Name" _
             & ",Link" _
             & ",FileName" _
             & ",BannerType" _
             & ",Width" _
             & ",Height" _
             & ",AltText" _
             & ",HTML" _
             & ",Target" _
             & ",IsActive" _
             & ",IsOrderTracking" _
             & ") VALUES (" _
             & m_DB.Quote(Name) _
             & "," & m_DB.Quote(Link) _
             & "," & m_DB.Quote(FileName) _
             & "," & m_DB.Quote(BannerType) _
             & "," & m_DB.Number(Width) _
             & "," & m_DB.Number(Height) _
             & "," & m_DB.Quote(AltText) _
             & "," & m_DB.Quote(HTML) _
             & "," & m_DB.Quote(Target) _
             & "," & CInt(IsActive) _
             & "," & CInt(IsOrderTracking) _
             & ")"

            BannerId = m_DB.InsertSQL(SQL)

            Return BannerId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE Banner SET " _
             & " Name = " & m_DB.Quote(Name) _
             & ",Link = " & m_DB.Quote(Link) _
             & ",FileName = " & m_DB.Quote(FileName) _
             & ",BannerType = " & m_DB.Quote(BannerType) _
             & ",Width = " & m_DB.Number(Width) _
             & ",Height = " & m_DB.Number(Height) _
             & ",AltText = " & m_DB.Quote(AltText) _
             & ",HTML = " & m_DB.Quote(HTML) _
             & ",Target = " & m_DB.Quote(Target) _
             & ",IsActive = " & CInt(IsActive) _
             & ",IsOrderTracking = " & CInt(IsOrderTracking) _
             & " WHERE BannerId = " & m_DB.Quote(BannerId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM BannerTracking WHERE BannerId = " & m_DB.Number(BannerId)
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM BannerBannerGroup WHERE BannerId = " & m_DB.Number(BannerId)
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM Banner WHERE BannerId = " & m_DB.Quote(BannerId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class BannerCollection
        Inherits GenericCollection(Of BannerRow)
    End Class

End Namespace


