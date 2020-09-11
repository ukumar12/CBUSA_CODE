Option Explicit On 

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Enum ModuleLevel
        All
        Section
        Template
    End Enum

    <Serializable()> _
    Public Class ContentToolRegion
        Public ContentRegion As String
        Public Name As String
        Public RegionType As String
        Public TemplateId As Integer
        Public SectionId As Integer
        Public Modules As ContentToolRegionModuleCollection
    End Class

    <Serializable()> _
    Public Class ContentToolRegionModule
        Public SortOrder As Integer
        Public ModuleId As Integer
        Public ControlURL As String
        Public Args As String
        Public Name As String
        Public HTML As String
    End Class

    <Serializable()> _
    Public Class ContentToolRegionModuleCollection
        Inherits GenericSerializableCollection(Of ContentToolRegionModule)
    End Class

    <Serializable()> _
    Public Class ContentToolRegionCollection
        Inherits GenericSerializableCollection(Of ContentToolRegion)

        Public Function FindByContentRegion(ByVal ContentRegion As String) As ContentToolRegion
            For i As Integer = 0 To Me.List.Count - 1
                Dim region As ContentToolRegion = Me.List.Item(i)
                If region.ContentRegion = ContentRegion Then
                    Return region
                End If
            Next
            Return Nothing
        End Function
    End Class

    Public Class ContentToolPageRow
        Inherits ContentToolPageRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            MyBase.New(database)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal PageId As Integer)
            MyBase.New(database, PageId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal _Database As Database, ByVal PageId As Integer) As ContentToolPageRow
            Dim row As ContentToolPageRow

            row = New ContentToolPageRow(_Database, PageId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal _Database As Database, ByVal PageId As Integer)
            Dim row As ContentToolPageRow

            row = New ContentToolPageRow(_Database, PageId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetRowByCustomURL(ByVal DB As Database, ByVal Url As String) As ContentToolPageRow
            Dim SQL As String = "SELECT * FROM ContentToolPage WHERE CustomURL = " & DB.Quote(Url)
            Dim r As SqlDataReader
            Dim row As ContentToolPageRow = New ContentToolPageRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

        Public Shared Function GetRowByURL(ByVal _Database As Database, ByVal URL As String) As ContentToolPageRow
            Dim row As ContentToolPageRow

            row = New ContentToolPageRow(_Database)
            row.PageURL = URL
            row.LoadByURL()

            Return row
        End Function

        Public Function GetDefaultContentToolModules(ByVal region As ContentToolRegion, ByVal Level As ModuleLevel) As ContentToolRegionModuleCollection
            Dim dvModules As DataView = GetPageModules(Level)
            Dim modules As New ContentToolRegionModuleCollection

            dvModules.RowFilter = "ContentRegion = " & DB.Quote(region.ContentRegion)
            For Each row As DataRowView In dvModules

                Dim m As New ContentToolRegionModule
                m.Args = IIf(IsDBNull(row("Args")), String.Empty, row("Args"))
                m.HTML = IIf(IsDBNull(row("HTML")), String.Empty, row("HTML"))
                m.ModuleId = row("ModuleId")
                m.ControlURL = IIf(IsDBNull(row("ControlURL")), String.Empty, row("ControlURL"))
                m.SortOrder = row("SortOrder")
                m.Name = row("ModuleName")

                modules.Add(m)
            Next
            Return modules
        End Function

        Public Function GetContentToolRegions() As ContentToolRegionCollection
            Dim col As New ContentToolRegionCollection
            Dim regions As ContentToolTemplateRegionCollection = ContentToolTemplateRegionRow.GetCollectionByTemplateId(DB, TemplateId)
            Dim modules As DataView = GetPageModules(ModuleLevel.All)

            For Each r As ContentToolTemplateRegionRow In regions

                Dim region As ContentToolRegion = New ContentToolRegion
                region.ContentRegion = r.ContentRegion
                region.Name = r.RegionName
                region.RegionType = GetRegionType(region.ContentRegion)

                region.Modules = New ContentToolRegionModuleCollection
                col.Add(region)

                modules.RowFilter = "ContentRegion = " & DB.Quote(r.ContentRegion)
                For Each row As DataRowView In modules

                    Dim m As New ContentToolRegionModule
                    m.Args = IIf(IsDBNull(row("Args")), String.Empty, row("Args"))
                    m.HTML = IIf(IsDBNull(row("HTML")), String.Empty, row("HTML"))
                    m.ModuleId = row("ModuleId")
                    m.ControlURL = IIf(IsDBNull(row("ControlURL")), String.Empty, row("ControlURL"))
                    m.SortOrder = row("SortOrder")
                    m.Name = row("ModuleName")

                    region.Modules.Add(m)
                Next
            Next
            Return col
        End Function

        Public Function GetRegionType(ByVal ContentRegion As String) As String
            Dim RegionType As String = "Default"

            Dim SQL As String = " select RegionType from ContentToolPageRegion where PageId = " & PageId & " and ContentRegion = " & DB.Quote(ContentRegion)
            Dim dr As SqlDataReader = DB.GetReader(SQL)
            If dr.Read Then
                RegionType = dr("RegionType")
            End If
            dr.Close()

            Return RegionType
        End Function

        Public Function GetPageModules(ByVal Level As ModuleLevel) As DataView
            Dim SQL As String = ""
            Dim Conn As String = ""

            'Get all custom regions
            If Level = ModuleLevel.All Then
                SQL &= Conn & " select 1 as Priority, ctpr.ContentRegion, ctpr.PageRegionId from ContentToolPage ctp WITH (NOLOCK) , ContentToolPageRegion ctpr  WITH (NOLOCK) where ctpr.PageId = " & PageId & " and ctp.TemplateId = " & TemplateId & " and ctp.SectionId = " & SectionId & " and ctp.PageId = " & PageId & " and ctp.PageId = ctpr.PageId and ctpr.RegionType = 'Custom'"
                Conn = " union all "
            End If
            If Level = ModuleLevel.All Or Level = ModuleLevel.Section Then
                SQL &= Conn & " select 2 as Priority, ctpr.ContentRegion, ctpr.PageRegionId from ContentToolPage ctp WITH (NOLOCK) , ContentToolPageRegion ctpr WITH (NOLOCK) where ctp.TemplateId = " & TemplateId & " and ctp.SectionId = " & SectionId & " and ctp.TemplateId = " & TemplateId & " and ctp.PageURL is null and ctp.PageId = ctpr.PageId and ctpr.RegionType = 'Custom'"
                Conn = " union all "
            End If
            If Level = ModuleLevel.All Or Level = ModuleLevel.Section Or Level = ModuleLevel.Template Then
                SQL &= Conn & " select 3 as Priority, ctpr.ContentRegion, ctpr.PageRegionId from ContentToolPage ctp WITH (NOLOCK) , ContentToolPageRegion ctpr WITH (NOLOCK) where ctp.TemplateId = " & TemplateId & " and ctp.Sectionid is null and ctp.PageURL is null and ctp.PageId = ctpr.PageId and ctpr.RegionType = 'Custom'"
                Conn = " union all "
            End If

            'Remove all regions that have been overwritten
            'Template --> Section --> Page
            Dim dt As DataTable = DB.GetDataTable(SQL)
            Dim dv As DataView = dt.DefaultView
            dv.Sort = "ContentRegion, Priority"

            Dim Priority As Integer = 0
            Dim ContentRegion As String = String.Empty
            Dim Count As Integer = dv.Table.Rows.Count()
            Dim Deleted As Integer = 0
            Dim RegionList As String = ""

            Conn = ""
            For i As Integer = 0 To Count - 1
                Dim row As DataRowView = dv.Item(i - Deleted)
                If ContentRegion = row("ContentRegion") Then
                    If row("Priority") > Priority Then
                        row.Delete()
                        Deleted += 1
                    End If
                Else
                    ContentRegion = row("ContentRegion")
                    Priority = row("Priority")

                    RegionList &= Conn & row("PageRegionId")
                    Conn = ","
                End If
            Next
            dv = Nothing
            dt = Nothing

            'Now, that we cleaned up the regions, we can retrieve modules
            SQL = ""
            SQL &= " select ctrm.SortOrder, ctrm.ModuleId, ctpr.ContentRegion, ctm.HTML, ctm.Name as ModuleName, ctm.ControlURL, coalesce(ctrm.Args, ctm.Args) as Args, cttr.RegionName, ctm.SkipIndexing "
            SQL &= " from  "
            SQL &= "    ContentToolPageRegion ctpr WITH (NOLOCK) , ContentToolRegionModule ctrm WITH (NOLOCK) ,  "
            SQL &= "    ContentToolModule ctm WITH (NOLOCK) , ContentToolTemplateRegion cttr  WITH (NOLOCK) "
            SQL &= " where  ctrm.ModuleId = ctm.ModuleId   "
            SQL &= " and 	cttr.TemplateId =  " & TemplateId
            SQL &= " and 	cttr.ContentRegion = ctpr.ContentRegion "
            SQL &= " and 	ctrm.PageRegionId in " & DB.NumberMultiple(RegionList)
            SQL &= " and	ctrm.PageRegionId = ctpr.PageRegionId "
            SQL &= " and 	ctpr.PageRegionId in " & DB.NumberMultiple(RegionList)

            dt = DB.GetDataTable(SQL)

            'Sort on webserver
            dv = dt.DefaultView
            dv.Sort = "ContentRegion, SortOrder"

            Return dv
        End Function

        Public Shared Function GetPageList(ByVal DB As Database) As DataTable
            Dim dt As DataTable

            dt = DB.GetDataTable("select PageId, Rtrim(name)+ '  (' + Rtrim(PageURL) + ')' as name, cts.SectionName from ContentToolPage ctp, ContentToolSection cts where ctp.Sectionid = cts.sectionid and ctp.PageURL is not null order by cts.Sectionname, ctp.Name")

            Return dt
        End Function

    End Class

    Public MustInherit Class ContentToolPageRowBase
        Private m_DB As Database
        Private m_PageId As Integer = Nothing
        Private m_PageURL As String = Nothing
        Private m_TemplateId As Integer = Nothing
        Private m_SectionId As Integer = Nothing
        Private m_Title As String = Nothing
        Private m_Name As String = Nothing
        Private m_CustomURL As String = Nothing
        Private m_IsContentBefore As Boolean = Nothing
        Private m_IsIndexed As Boolean = Nothing
        Private m_IsFollowed As Boolean = Nothing
        Private m_MetaDescription As String = Nothing
        Private m_MetaKeywords As String = Nothing
        Private m_ModifyDate As DateTime = Nothing
        Private m_NavigationId As Integer = Nothing
        Private m_SubNavigationId As Integer = Nothing
        Private m_SkipIndexing As Boolean = Nothing
        Private m_IsPermanent As Boolean = Nothing   
        Private OriginalCustomURL As String = Nothing

        Public Property PageId() As Integer
            Get
                Return m_PageId
            End Get
            Set(ByVal Value As Integer)
                m_PageId = Value
            End Set
        End Property

        Public Property PageURL() As String
            Get
                Return m_PageURL
            End Get
            Set(ByVal Value As String)
                m_PageURL = Value
            End Set
        End Property

        Public Property TemplateId() As Integer
            Get
                Return m_TemplateId
            End Get
            Set(ByVal Value As Integer)
                m_TemplateId = Value
            End Set
        End Property

        Public Property SectionId() As Integer
            Get
                Return m_SectionId
            End Get
            Set(ByVal Value As Integer)
                m_SectionId = Value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(ByVal Value As String)
                m_Title = Value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal Value As String)
                m_Name = Value
            End Set
        End Property

        Public Property CustomURL() As String
            Get
                Return m_CustomURL
            End Get
            Set(ByVal Value As String)
                m_CustomURL = Value
            End Set
        End Property

        Public Property IsContentBefore() As Boolean
            Get
                Return m_IsContentBefore
            End Get
            Set(ByVal Value As Boolean)
                m_IsContentBefore = value
            End Set
        End Property

        Public Property IsIndexed() As Boolean
            Get
                Return m_IsIndexed
            End Get
            Set(ByVal Value As Boolean)
                m_IsIndexed = value
            End Set
        End Property

        Public Property IsFollowed() As Boolean
            Get
                Return m_IsFollowed
            End Get
            Set(ByVal Value As Boolean)
                m_IsFollowed = value
            End Set
        End Property

        Public Property MetaDescription() As String
            Get
                Return m_MetaDescription
            End Get
            Set(ByVal Value As String)
                m_MetaDescription = value
            End Set
        End Property

        Public Property MetaKeywords() As String
            Get
                Return m_MetaKeywords
            End Get
            Set(ByVal Value As String)
                m_MetaKeywords = value
            End Set
        End Property

        Public Property ModifyDate() As DateTime
            Get
                Return m_ModifyDate
            End Get
            Set(ByVal Value As DateTime)
                m_ModifyDate = Value
            End Set
        End Property

        Public Property NavigationId() As Integer
            Get
                Return m_NavigationId
            End Get
            Set(ByVal Value As Integer)
                m_NavigationId = Value
            End Set
        End Property

        Public Property SubNavigationId() As Integer
            Get
                Return m_SubNavigationId
            End Get
            Set(ByVal Value As Integer)
                m_SubNavigationId = Value
            End Set
        End Property

        Public Property SkipIndexing() As Boolean
            Get
                Return m_SkipIndexing
            End Get
            Set(ByVal Value As Boolean)
                m_SkipIndexing = Value
            End Set
        End Property

        Public Property IsPermanent() As Boolean
            Get
                Return m_IsPermanent
            End Get
            Set(ByVal Value As Boolean)
                m_IsPermanent = Value
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

        Public Sub New(ByVal database As Database, ByVal PageId As Integer)
            m_DB = database
            m_PageId = PageId
        End Sub 'New

        Public Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM ContentToolPage WHERE PageId = " & DB.Quote(PageId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Public Overridable Sub LoadByURL()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM ContentToolPage WITH (NOLOCK) WHERE PageURL = " & DB.Quote(PageURL)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Public Overridable Sub Load(ByVal r As SqlDataReader)
            m_PageId = Convert.ToInt32(r.Item("PageId"))
            If r.Item("PageURL") Is Convert.DBNull Then
                m_PageURL = Nothing
            Else
                m_PageURL = Convert.ToString(r.Item("PageURL"))
            End If
            m_TemplateId = Convert.ToInt32(r.Item("TemplateId"))
            If r.Item("SectionId") Is Convert.DBNull Then
                m_SectionId = Nothing
            Else
                m_SectionId = Convert.ToInt32(r.Item("SectionId"))
            End If
            If r.Item("Title") Is Convert.DBNull Then
                m_Title = Nothing
            Else
                m_Title = Convert.ToString(r.Item("Title"))
            End If
            If r.Item("Name") Is Convert.DBNull Then
                m_Name = Nothing
            Else
                m_Name = Convert.ToString(r.Item("Name"))
            End If
            If r.Item("CustomURL") Is Convert.DBNull Then
                m_CustomURL = Nothing
                OriginalCustomURL = Nothing
            Else
                m_CustomURL = Convert.ToString(r.Item("CustomURL"))
                OriginalCustomURL = Convert.ToString(r.Item("CustomURL"))
            End If
            m_IsContentBefore = Convert.ToBoolean(r.Item("IsContentBefore"))
            m_IsIndexed = Convert.ToBoolean(r.Item("IsIndexed"))
            m_IsFollowed = Convert.ToBoolean(r.Item("IsFollowed"))
            m_SkipIndexing = Convert.ToBoolean(r.Item("SkipIndexing"))
            m_IsPermanent = Convert.ToBoolean(r.Item("IsPermanent"))
            If IsDBNull(r.Item("MetaDescription")) Then
                m_MetaDescription = Nothing
            Else
                m_MetaDescription = Convert.ToString(r.Item("MetaDescription"))
            End If
            If IsDBNull(r.Item("MetaKeywords")) Then
                m_MetaKeywords = Nothing
            Else
                m_MetaKeywords = Convert.ToString(r.Item("MetaKeywords"))
            End If
            If r.Item("ModifyDate") Is Convert.DBNull Then
                m_ModifyDate = Nothing
            Else
                m_ModifyDate = Convert.ToDateTime(r.Item("ModifyDate"))
            End If
            If r.Item("NavigationId") Is Convert.DBNull Then
                m_NavigationId = Nothing
            Else
                m_NavigationId = Convert.ToInt32(r.Item("NavigationId"))
            End If
            If r.Item("SubNavigationId") Is Convert.DBNull Then
                m_SubNavigationId = Nothing
            Else
                m_SubNavigationId = Convert.ToInt32(r.Item("SubNavigationId"))
            End If

        End Sub 'Load

        Private ReadOnly Property InsertStatement() As String
            Get
                Dim SQL As String

                SQL = " INSERT INTO ContentToolPage (" _
                 & " PageURL" _
                 & ",TemplateId" _
                 & ",SectionId" _
                 & ",Title" _
                 & ",Name" _
                 & ",CustomURL" _
                 & ",IsContentBefore" _
                 & ",IsIndexed" _
                 & ",IsFollowed" _
                 & ",SkipIndexing" _
                 & ",IsPermanent" _
                 & ",MetaDescription" _
                 & ",MetaKeywords" _
                 & ",ModifyDate" _
                 & ",NavigationId" _
                 & ",SubNavigationId" _
                 & ") VALUES (" _
                 & m_DB.Quote(PageURL) _
                 & "," & m_DB.Quote(TemplateId) _
                 & "," & m_DB.NullQuote(SectionId) _
                 & "," & m_DB.Quote(Title) _
                 & "," & m_DB.Quote(Name) _
                 & "," & m_DB.Quote(CustomURL) _
                 & "," & CInt(IsContentBefore) _
                 & "," & CInt(IsIndexed) _
                 & "," & CInt(IsFollowed) _
                 & "," & CInt(SkipIndexing) _
                 & "," & CInt(IsPermanent) _
                 & "," & m_DB.Quote(MetaDescription) _
                 & "," & m_DB.Quote(MetaKeywords) _
                 & "," & m_DB.Quote(Now) _
                 & "," & m_DB.NullQuote(NavigationId) _
                 & "," & m_DB.NullQuote(SubNavigationId) _
                 & ")"

                Return SQL
            End Get
        End Property

        Public Overridable Sub Insert()
            m_DB.ExecuteSQL(InsertStatement)
        End Sub 'Insert

        Function AutoInsert() As Integer
            PageId = m_DB.InsertSQL(InsertStatement)
            Return PageId
        End Function


        Public Overridable Sub Update()
            Dim SQL As String
            If OriginalCustomURL <> String.Empty And OriginalCustomURL <> m_CustomURL Then
                If m_CustomURL = "" Then
                    CustomURLHistoryRow.AddToHistory(DB, OriginalCustomURL, m_PageURL)
                Else
                    CustomURLHistoryRow.AddToHistory(DB, OriginalCustomURL, m_CustomURL)
                End If
            End If
            SQL = " UPDATE ContentToolPage SET " _
             & " PageURL = " & m_DB.Quote(PageURL) _
             & ",TemplateId = " & m_DB.Quote(TemplateId) _
             & ",SectionId = " & m_DB.NullQuote(SectionId) _
             & ",Title = " & m_DB.Quote(Title) _
             & ",Name = " & m_DB.Quote(Name) _
             & ",CustomURL = " & m_DB.Quote(CustomURL) _
             & ",IsContentBefore = " & CInt(IsContentBefore) _
             & ",IsIndexed = " & CInt(IsIndexed) _
             & ",IsFollowed = " & CInt(IsFollowed) _
             & ",SkipIndexing = " & CInt(SkipIndexing) _
             & ",IsPermanent = " & CInt(IsPermanent) _
             & ",MetaDescription = " & m_DB.Quote(MetaDescription) _
             & ",MetaKeywords = " & m_DB.Quote(MetaKeywords) _
             & ",ModifyDate = " & m_DB.Quote(Now) _
             & ",NavigationId = " & m_DB.NullQuote(NavigationId) _
             & ",SubNavigationId = " & m_DB.NullQuote(SubNavigationId) _
             & " WHERE PageId = " & m_DB.Quote(PageId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM ContentToolPage WHERE PageId = " & m_DB.Quote(PageId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class ContentToolPageCollection
        Inherits GenericCollection(Of ContentToolPageRow)
    End Class

End Namespace


