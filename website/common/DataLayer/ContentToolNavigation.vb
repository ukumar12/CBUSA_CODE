Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class ContentToolNavigationRow
        Inherits ContentToolNavigationRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            MyBase.New(database)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal NavigationId As Integer)
            MyBase.New(database, NavigationId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal _Database As Database, ByVal NavigationId As Integer) As ContentToolNavigationRow
            Dim row As ContentToolNavigationRow

            row = New ContentToolNavigationRow(_Database, NavigationId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal _Database As Database, ByVal NavigationId As Integer)
            Dim row As ContentToolNavigationRow

            row = New ContentToolNavigationRow(_Database, NavigationId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetMainMenu(ByVal DB As Database) As DataTable
            Dim dt As DataTable = DB.GetDataTable("select * from ContentToolNavigation where ParentId is null order by SortOrder")
            Return dt
        End Function

        Public Shared Function GetSubNavigation(ByVal Db As Database, ByVal ParentId As Integer) As DataTable
            Dim dt As DataTable = Db.GetDataTable("select * from ContentToolNavigation where ParentId = " & ParentId & " order by SortOrder")
            Return dt
        End Function

        Public Shared Function GetSubNavigationByURL(ByVal Db As Database, ByVal PageURL As String) As DataTable
            Dim SQL As String = ""

            SQL &= " select 0 as Parent, NavigationId, case when IsInternalLink = 1 then (select top 1 PageURl from ContentToolPage where PageId = ctn.PageId) else URL end as URL, Title, SortOrder from ContentToolNavigation ctn where ParentId in (select top 1 NavigationId from ContentToolPage where PageURL = " & Db.Quote(PageURL) & ") "
            SQL &= " order by Parent desc, SortOrder "

            Dim dt As DataTable = Db.GetDataTable(SQL)
            Return dt
        End Function
    End Class

    Public MustInherit Class ContentToolNavigationRowBase
        Private m_DB As Database
        Private m_NavigationId As Integer = Nothing
        Private m_Title As String = Nothing
        Private m_ParentId As Integer = Nothing
        Private m_IsInternalLink As Boolean = Nothing
        Private m_PageId As Integer = Nothing
        Private m_URL As String = Nothing
        Private m_FinalURL As String = Nothing
        Private m_Target As String = Nothing
        Private m_Parameters As String = Nothing
        Private m_SkipSiteMap As Boolean = Nothing
        Private m_SkipBreadcrumb As Boolean = Nothing

        Public Property NavigationId() As Integer
            Get
                Return m_NavigationId
            End Get
            Set(ByVal Value As Integer)
                m_NavigationId = value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(ByVal Value As String)
                m_Title = value
            End Set
        End Property

        Public Property SkipSiteMap() As Boolean
            Get
                Return m_SkipSiteMap
            End Get
            Set(ByVal Value As Boolean)
                m_SkipSiteMap = Value
            End Set
        End Property

        Public Property SkipBreadcrumb() As Boolean
            Get
                Return m_SkipBreadcrumb
            End Get
            Set(ByVal Value As Boolean)
                m_SkipBreadcrumb = Value
            End Set
        End Property

        Public Property ParentId() As Integer
            Get
                Return m_ParentId
            End Get
            Set(ByVal Value As Integer)
                m_ParentId = value
            End Set
        End Property

        Public Property IsInternalLink() As Boolean
            Get
                Return m_IsInternalLink
            End Get
            Set(ByVal Value As Boolean)
                m_IsInternalLink = value
            End Set
        End Property

        Public Property PageId() As Integer
            Get
                Return m_PageId
            End Get
            Set(ByVal Value As Integer)
                m_PageId = value
            End Set
        End Property

        Public Property URL() As String
            Get
                Return m_URL
            End Get
            Set(ByVal Value As String)
                m_URL = value
            End Set
        End Property

        Public ReadOnly Property FinalURL() As String
            Get
                Return m_FinalURL
            End Get
        End Property

        Public Property Target() As String
            Get
                Return m_Target
            End Get
            Set(ByVal Value As String)
                m_Target = value
            End Set
        End Property

        Public Property Parameters() As String
            Get
                Return m_Parameters
            End Get
            Set(ByVal Value As String)
                m_Parameters = value
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

        Public Sub New(ByVal database As Database, ByVal NavigationId As Integer)
            m_DB = database
            m_NavigationId = NavigationId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT *, case when IsInternalLink = 1 then (select top 1 PageURl from ContentToolPage where PageId = ctn.PageId) else URL end as FinalURL FROM ContentToolNavigation ctn WHERE NavigationId = " & DB.Quote(NavigationId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_Title = Convert.ToString(r.Item("Title"))
            If IsDBNull(r.Item("ParentId")) Then
                m_ParentId = Nothing
            Else
                m_ParentId = Convert.ToInt32(r.Item("ParentId"))
            End If
            m_IsInternalLink = Convert.ToBoolean(r.Item("IsInternalLink"))
            If IsDBNull(r.Item("PageId")) Then
                m_PageId = Nothing
            Else
                m_PageId = Convert.ToInt32(r.Item("PageId"))
            End If
            If IsDBNull(r.Item("URL")) Then
                m_URL = Nothing
            Else
                m_URL = Convert.ToString(r.Item("URL"))
            End If
            If IsDBNull(r.Item("FinalURL")) Then
                m_FinalURL = Nothing
            Else
                m_FinalURL = Convert.ToString(r.Item("FinalURL"))
            End If
            If IsDBNull(r.Item("Target")) Then
                m_Target = Nothing
            Else
                m_Target = Convert.ToString(r.Item("Target"))
            End If
            If IsDBNull(r.Item("Parameters")) Then
                m_Parameters = Nothing
            Else
                m_Parameters = Convert.ToString(r.Item("Parameters"))
            End If
            m_SkipSiteMap = Convert.ToBoolean(r.Item("SkipSiteMap"))
            m_SkipBreadcrumb = Convert.ToBoolean(r.Item("SkipBreadcrumb"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            If ParentId = Nothing Then
                SQL = "select coalesce(max(SortOrder),0) from ContentToolNavigation where ParentId is null"
            Else
                SQL = "select coalesce(max(SortOrder),0) from ContentToolNavigation where ParentId = " & ParentId
            End If

            Dim SortOrder As Integer = DB.ExecuteScalar(SQL)
            SortOrder += 1

            SQL = " INSERT INTO ContentToolNavigation (" _
             & " Title" _
             & ",ParentId" _
             & ",IsInternalLink" _
             & ",PageId" _
             & ",URL" _
             & ",Target" _
             & ",Parameters" _
             & ",SortOrder" _
             & ",SkipSitemap" _
             & ",SkipBreadCrumb" _
             & ") VALUES (" _
             & m_DB.Quote(Title) _
             & "," & m_DB.NullQuote(ParentId) _
             & "," & CInt(IsInternalLink) _
             & "," & m_DB.Quote(PageId) _
             & "," & m_DB.Quote(URL) _
             & "," & m_DB.Quote(Target) _
             & "," & m_DB.Quote(Parameters) _
             & "," & m_DB.Quote(SortOrder) _
             & "," & CInt(SkipSiteMap) _
             & "," & CInt(SkipBreadcrumb) _
             & ")"

            NavigationId = m_DB.InsertSQL(SQL)

            Return NavigationId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE ContentToolNavigation SET " _
             & " Title = " & m_DB.Quote(Title) _
             & ",IsInternalLink = " & CInt(IsInternalLink) _
             & ",PageId = " & m_DB.Quote(PageId) _
             & ",URL = " & m_DB.Quote(URL) _
             & ",Target = " & m_DB.Quote(Target) _
             & ",Parameters = " & m_DB.Quote(Parameters) _
             & ",SkipSitemap = " & CInt(SkipSiteMap) _
             & ",SkipBreadCrumb = " & CInt(SkipBreadcrumb) _
             & " WHERE NavigationId = " & m_DB.Quote(NavigationId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM ContentToolNavigation WHERE NavigationId = " & m_DB.Quote(NavigationId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class ContentToolNavigationCollection
        Inherits GenericCollection(Of ContentToolNavigationRow)
    End Class

End Namespace


