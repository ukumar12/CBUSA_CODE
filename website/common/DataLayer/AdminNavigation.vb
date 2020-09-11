Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class AdminNavigationRow
        Inherits AdminNavigationRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal AdminNavigationId As Integer)
            MyBase.New(DB, AdminNavigationId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal AdminNavigationId As Integer) As AdminNavigationRow
            Dim row As AdminNavigationRow

            row = New AdminNavigationRow(DB, AdminNavigationId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal AdminNavigationId As Integer)
            Dim row As AdminNavigationRow

            row = New AdminNavigationRow(DB, AdminNavigationId)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from AdminNavigation"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

        Public Shared Function GetAllNavigationItems(ByVal DB As Database, ByVal CacheName As String) As DataTable

            Dim dtRootNavigationItems As DataTable
            If System.Web.HttpContext.Current.Cache(CacheName) Is Nothing Or _
                System.Web.HttpContext.Current.Cache(CacheName) Is Nothing Then
                dtRootNavigationItems = DB.GetDataTable("SELECT *, COALESCE(RedirectURL,'#') as AdjRedirectURL FROM AdminNavigation ORDER BY SortOrder")
                System.Web.HttpContext.Current.Cache.Insert(CacheName, dtRootNavigationItems, Nothing, DateAdd(DateInterval.Minute, 1, Now), TimeSpan.Zero)
            End If

            dtRootNavigationItems = System.Web.HttpContext.Current.Cache(CacheName)

            Return dtRootNavigationItems
        End Function

    End Class

    Public MustInherit Class AdminNavigationRowBase
        Private m_DB As Database
        Private m_AdminNavigationId As Integer = Nothing
        Private m_ParentId As Integer = Nothing
        Private m_Image As String = Nothing
        Private m_AltTag As String = Nothing
        Private m_Name As String = Nothing
        Private m_SortOrder As Integer = Nothing
        Private m_IsSelectedPath As String = Nothing
        Private m_RedirectURL As String = Nothing
        Private m_AdminSection As String = Nothing
        Private m_IsInternal As Boolean = Nothing
        Private m_IsBranch As Boolean = Nothing
        Private m_IsBreak As Boolean = Nothing



        Public Property AdminNavigationId() As Integer
            Get
                Return m_AdminNavigationId
            End Get
            Set(ByVal Value As Integer)
                m_AdminNavigationId = value
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

        Public Property Image() As String
            Get
                Return m_Image
            End Get
            Set(ByVal Value As String)
                m_Image = value
            End Set
        End Property

        Public Property AltTag() As String
            Get
                Return m_AltTag
            End Get
            Set(ByVal Value As String)
                m_AltTag = value
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

        Public Property SortOrder() As Integer
            Get
                Return m_SortOrder
            End Get
            Set(ByVal Value As Integer)
                m_SortOrder = value
            End Set
        End Property

        Public Property IsSelectedPath() As String
            Get
                Return m_IsSelectedPath
            End Get
            Set(ByVal Value As String)
                m_IsSelectedPath = value
            End Set
        End Property

        Public Property RedirectURL() As String
            Get
                Return m_RedirectURL
            End Get
            Set(ByVal Value As String)
                m_RedirectURL = value
            End Set
        End Property

        Public Property AdminSection() As String
            Get
                Return m_AdminSection
            End Get
            Set(ByVal Value As String)
                m_AdminSection = value
            End Set
        End Property

        Public Property IsInternal() As Boolean
            Get
                Return m_IsInternal
            End Get
            Set(ByVal Value As Boolean)
                m_IsInternal = value
            End Set
        End Property

        Public Property IsBranch() As Boolean
            Get
                Return m_IsBranch
            End Get
            Set(ByVal Value As Boolean)
                m_IsBranch = value
            End Set
        End Property

        Public Property IsBreak() As Boolean
            Get
                Return m_IsBreak
            End Get
            Set(ByVal Value As Boolean)
                m_IsBreak = value
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

        Public Sub New(ByVal DB As Database, ByVal AdminNavigationId As Integer)
            m_DB = DB
            m_AdminNavigationId = AdminNavigationId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM AdminNavigation WHERE AdminNavigationId = " & DB.Number(AdminNavigationId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_AdminNavigationId = Convert.ToInt32(r.Item("AdminNavigationId"))
            If IsDBNull(r.Item("ParentId")) Then
                m_ParentId = Nothing
            Else
                m_ParentId = Convert.ToInt32(r.Item("ParentId"))
            End If
            If IsDBNull(r.Item("Image")) Then
                m_Image = Nothing
            Else
                m_Image = Convert.ToString(r.Item("Image"))
            End If
            If IsDBNull(r.Item("AltTag")) Then
                m_AltTag = Nothing
            Else
                m_AltTag = Convert.ToString(r.Item("AltTag"))
            End If
            m_Name = Convert.ToString(r.Item("Name"))
            If IsDBNull(r.Item("IsSelectedPath")) Then
                m_IsSelectedPath = Nothing
            Else
                m_IsSelectedPath = Convert.ToString(r.Item("IsSelectedPath"))
            End If
            If IsDBNull(r.Item("RedirectURL")) Then
                m_RedirectURL = Nothing
            Else
                m_RedirectURL = Convert.ToString(r.Item("RedirectURL"))
            End If
            If IsDBNull(r.Item("AdminSection")) Then
                m_AdminSection = Nothing
            Else
                m_AdminSection = Convert.ToString(r.Item("AdminSection"))
            End If
            m_IsInternal = Convert.ToBoolean(r.Item("IsInternal"))
            m_IsBranch = Convert.ToBoolean(r.Item("IsBranch"))
            m_IsBreak = Convert.ToBoolean(r.Item("IsBreak"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from AdminNavigation WHERE ParentId = " & DB.NullNumber(ParentId) & " order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO AdminNavigation (" _
             & " ParentId" _
             & ",Image" _
             & ",AltTag" _
             & ",Name" _
             & ",SortOrder" _
             & ",IsSelectedPath" _
             & ",RedirectURL" _
             & ",AdminSection" _
             & ",IsInternal" _
             & ",IsBranch" _
             & ",IsBreak" _
             & ") VALUES (" _
             & m_DB.NullNumber(ParentId) _
             & "," & m_DB.Quote(Image) _
             & "," & m_DB.Quote(AltTag) _
             & "," & m_DB.Quote(Name) _
             & "," & MaxSortOrder _
             & "," & m_DB.Quote(IsSelectedPath) _
             & "," & m_DB.Quote(RedirectURL) _
             & "," & m_DB.Quote(AdminSection) _
             & "," & CInt(IsInternal) _
             & "," & CInt(IsBranch) _
             & "," & CInt(IsBreak) _
             & ")"

            AdminNavigationId = m_DB.InsertSQL(SQL)

            Return AdminNavigationId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE AdminNavigation SET " _
             & " ParentId = " & m_DB.NullNumber(ParentId) _
             & ",Image = " & m_DB.Quote(Image) _
             & ",AltTag = " & m_DB.Quote(AltTag) _
             & ",Name = " & m_DB.Quote(Name) _
             & ",IsSelectedPath = " & m_DB.Quote(IsSelectedPath) _
             & ",RedirectURL = " & m_DB.Quote(RedirectURL) _
             & ",AdminSection = " & m_DB.Quote(AdminSection) _
             & ",IsInternal = " & CInt(IsInternal) _
             & ",IsBranch = " & CInt(IsBranch) _
             & ",IsBreak = " & CInt(IsBreak) _
             & " WHERE AdminNavigationId = " & m_DB.quote(AdminNavigationId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM AdminNavigation WHERE AdminNavigationId = " & m_DB.Number(AdminNavigationId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class AdminNavigationCollection
        Inherits GenericCollection(Of AdminNavigationRow)
    End Class

End Namespace

