Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    ''' <summary>
    ''' Represents a row in the Asset table in the database.
    ''' </summary>
    ''' <remarks>Custom methods should be placed in this class, not in <see cref="AssetRowBase" />.</remarks>
    Public Class AssetRow
        Inherits AssetRowBase

        ''' <overloads>Initializes a new instance of the <see cref="AssetRow" /> class.</overloads>
        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetRow" /> class using default settings.
        ''' </summary>
        ''' <remarks>This constructor calls 
        ''' <see cref="M:DataLayer.AssetRowBase.#ctor">AssetRowBase.New</see>.</remarks>
        Public Sub New()
            MyBase.New()
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetRow" /> class with the database connection
        ''' specified by <paramref name="DB" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <remarks><para>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.AssetRow.#ctor(Database,System.Int32)">New(Database, Integer)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</para>
        ''' <para>This constructor calls 
        ''' <see cref="M:DataLayer.AssetRowBase.#ctor(Database)">AssetRowBase.New(Database)</see>.</para></remarks>
        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetRow" /> class representing the row which uses
        ''' <paramref name="AssetId" /> as its primary key and <paramref name="DB" /> as its database connection.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetId">The primary key value of the row being referenced.</param>
        ''' <remarks><para>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.AssetRow.#ctor(Database)">New(Database)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</para>
        ''' <para>This constructor calls 
        ''' <see cref="M:DataLayer.AssetRowBase.#ctor(Database,System.Int32)">AssetRowBase.New(Database, Integer)</see>.</para></remarks>
        Public Sub New(ByVal DB As Database, ByVal AssetId As Integer)
            MyBase.New(DB, AssetId)
        End Sub 'New

        ''' <summary>
        ''' Gets the row from the specified <see cref="Database" /> with the specified 
        ''' <paramref name="AssetId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetId">The primary key value of the row being retrieved.</param>
        ''' <returns>An instance of <see cref="AssetRow" /> loaded with the values from the specified 
        ''' row in the database.</returns>
        ''' <remarks>This method uses <see cref="M:DataLayer.AssetRowBase.Load">Load</see>.</remarks>
        Public Shared Function GetRow(ByVal DB As Database, ByVal AssetId As Integer) As AssetRow
            Dim row As AssetRow

            row = New AssetRow(DB, AssetId)
            row.Load()

            Return row
        End Function

        ''' <summary>
        ''' Retrieves the published <see cref="AssetRow" /> with the specified <paramref name="FirstId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="FirstId">The value of <see cref="FirstId" /> to filter by.</param>
        ''' <returns>The published <see cref="AssetRow" /> for the specified <paramref name="FirstId" />.</returns>
        ''' <remarks>There is only one published row per <see cref="FirstId" />.</remarks>
        Public Shared Function GetPublishedRowByFirstId(ByVal DB As Database, ByVal FirstId As Integer) As AssetRow
            Dim SQL As String = "SELECT * FROM Asset WHERE FirstId = " & FirstId & " and Status = 'Published'"
            Dim r As SqlDataReader
            Dim row As AssetRow = New AssetRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

        ''' <summary>
        ''' Marks the <see cref="AssetRow" /> with the specified <paramref name="AssetId" /> as deleted using the
        ''' specified <paramref name="AdminId" /> as <see cref="ModifyAdminId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetId">The primary key value of the row being marked as deleted.</param>
        ''' <param name="AdminId">The user to leave tracks as in the <see cref="ModifyAdminId" /> field.</param>
        ''' <param name="showdeleted">The value to set <see cref="ShowDeleted" /> to. The default value of this
        ''' parameter is <see langword="False" />.</param>
        ''' <remarks><para><see cref="ShowPending" /> is automatically set to <see langword="False" />.</para>
        ''' <para>To physically remove a row from the database, use <see cref="RemoveByAsset" /> instead.</para></remarks>
        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal AssetId As Integer, ByVal AdminId As Integer, Optional ByVal showdeleted As Boolean = False)
            Dim row As AssetRow = AssetRow.GetRow(DB, AssetId)
            row.StatusDate = Now
            row.Status = "Deleted"
            row.ShowDeleted = showdeleted
            row.ShowPending = False
            row.ModifyAdminId = AdminId
            row.Update()
        End Sub 'Remove

        ''' <summary>
        ''' Retrieves the Asset table from the specified <see cref="Database" /> ordered based on 
        ''' <paramref name="SortBy" /> and <paramref name="SortOrder" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="SortBy">The SQL field name to sort the results by.</param>
        ''' <param name="SortOrder">The order by which to sort the results (ASC, DESC).  The default value of this
        ''' parameter is "ASC".</param>
        ''' <returns>A <see cref="DataTable" /> containing the data returned by the query.</returns>
        ''' <remarks>If <paramref name="SortBy" /> is not provided, the data is not sorted during the query.</remarks>
        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from Asset"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        'Please document with XML comments all custom methods added here.

        ''' <summary>
        ''' Removes the row from the specified <see cref="Database" /> with the specified 
        ''' <paramref name="AssetId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetId">The primary key value of the row being removed.</param>
        ''' <remarks>Unlike <see cref="RemoveRow" />, this method physically deletes the specified row from the
        ''' database.</remarks>
        Public Shared Sub RemoveByAsset(ByVal DB As Database, ByVal AssetId As Integer)
            Dim SQL As String = "DELETE FROM AssetKeyword WHERE AssetId = " & DB.Number(AssetId)
            DB.ExecuteSQL(SQL)
        End Sub

        ''' <summary>
        ''' Associates the specified <paramref name="KeywordId" /> with the specified <paramref name="AssetId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetId">The primary key value of the asset to give a new keyword to.</param>
        ''' <param name="KeywordId">The primary key value of the new keyword to assign to the specified
        ''' <paramref name="AssetId" />.</param>
        ''' <remarks>Keywords serve as a customizable, tag-like method for categorizing assets.</remarks>
        Public Shared Sub InsertAssetKeyword(ByVal DB As Database, ByVal AssetId As Integer, ByVal KeywordId As Integer)
            Dim SQL As String = "INSERT INTO AssetKeyword (" _
             & " AssetId" _
             & ",KeywordId" _
             & ") VALUES (" _
             & DB.Number(AssetId) _
             & "," & DB.Number(KeywordId) _
            & ")"
            DB.ExecuteSQL(SQL)
        End Sub

        ''' <summary>
        ''' Retrieves a <see cref="DataTable" /> containing all the Keywords not selected for the specified
        ''' <paramref name="AssetId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetId">The primary key value of the asset to look up unselected keywords for.</param>
        ''' <returns>A <see cref="DataTable" /> containing the unselected keywords for the specified asset.</returns>
        ''' <remarks>The keywords are automatically ordered by <see cref="KeywordRow.Name" />.</remarks>
        Public Shared Function LoadUnselectedKeywords(ByVal DB As Database, ByVal AssetId As Integer) As DataTable
            Dim SQL As New StringBuilder

            'Bind Left List
            If AssetId <> 0 Then
                SQL.Append("SELECT * FROM Keyword WHERE KeywordId NOT IN")
                SQL.Append("(")
                SQL.Append("SELECT KeywordId FROM AssetKeyword WHERE AssetId = " & DB.Quote(AssetId.ToString))
                SQL.Append(") ORDER BY Name")
            Else
                SQL.Append("SELECT * FROM Keyword ORDER BY Name")
            End If
            Return DB.GetDataTable(SQL.ToString)
        End Function

        ''' <summary>
        ''' Retrieves a <see cref="DataTable" /> containing all the Keywords selected for the specified
        ''' <paramref name="AssetId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetId">The primary key value of the asset to look up selected keywords for.</param>
        ''' <returns>A <see cref="DataTable" /> containing the selected keywords for the specified asset.</returns>
        ''' <remarks>The keywords are automatically ordered by <see cref="KeywordRow.Name" />.</remarks>
        Public Shared Function LoadSelectedKeywords(ByVal DB As Database, ByVal AssetId As Integer) As DataTable
            Dim SQL As New StringBuilder

            SQL.Append("SELECT * FROM Keyword WHERE KeywordId IN ")
            SQL.Append("(")
            SQL.Append("SELECT KeywordId FROM AssetKeyword WHERE AssetId = " & DB.Quote(AssetId))
            SQL.Append(") ORDER BY Name")

            Return DB.GetDataTable(SQL.ToString)
        End Function

        ''' <summary>
        ''' Retrieves the <see cref="AssetRow" /> of the asset located at the specified <paramref name="URL" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="URL">The URL of the asset whose row shall be retrieved.</param>
        ''' <returns>An <see cref="AssetRow" /> matching the specified <paramref name="URL" />.</returns>
        ''' <remarks>This method automatically filters out /sites/sitename/ from the provided URL.</remarks>
        Public Shared Function GetRowByUrl(ByVal DB As Database, ByVal URL As String) As AssetRow
            Dim row As AssetRow
            Dim AssetId As Integer

            If URL Is Nothing Then Return Nothing
            If URL = String.Empty Then Return Nothing

            URL = Replace(URL, "/", "\")
            Dim m As Text.RegularExpressions.Match = Text.RegularExpressions.Regex.Match(URL, "\\sites\\([\d]+)\\")
            Dim SiteId As Integer = m.Groups(1).Value
            URL = URL.Replace(m.Value, "\")

            Dim SQL As String = String.Empty
            SQL &= " SELECT TOP 1 AssetId FROM Asset WHERE ToolSectionUrl + Filename = " & DB.Quote(URL)
            AssetId = DB.ExecuteScalar(SQL)

            row = AssetRow.GetRow(DB, AssetId)

            Return row
        End Function

        ''' <summary>
        ''' Retrieves the root row with the specified <paramref name="AssetId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetId">The primary key value of the asset to look up.</param>
        ''' <returns>The row with the specified <paramref name="AssetId" /> if it has a NULL 
        ''' <see cref="ParentId" />; A blank <see cref="AssetRow" /> otherwise.</returns>
        ''' <remarks>This method uses <see cref="GetRowWhere" />.</remarks>
        Public Shared Function GetParentRow(ByVal DB As Database, ByVal AssetId As Integer) As AssetRow
            Return GetRowWhere(DB, AssetId, "ParentId IS NULL")
        End Function

        ''' <summary>
        ''' Retrieves the row with the specified <paramref name="AssetId" /> if it matches the where clause
        ''' specified by <paramref name="sWhere" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetId">The primary key value of the asset to look up.</param>
        ''' <param name="sWhere">The value to use as an additional WHERE clause.</param>
        ''' <returns>The row with the specified <paramref name="AssetId" /> if it fulfills 
        ''' <paramref name="sWhere" />; A blank <see cref="AssetRow" /> otherwise.</returns>
        ''' <remarks>This method is only currently used by <see cref="GetParentRow" />.</remarks>
        Public Shared Function GetRowWhere(ByVal DB As Database, ByVal AssetId As Integer, ByVal sWhere As String) As AssetRow
            Dim SQL As String = "SELECT * FROM Asset WHERE AssetId = " & DB.Number(AssetId)
            If Not DB.IsEmpty(sWhere) Then
                SQL &= " AND " & sWhere
            End If
            Dim r As SqlDataReader
            Dim row As AssetRow = New AssetRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

        ''' <summary>
        ''' Retrieves an <see cref="AssetCollection" /> containing all of the <see cref="AssetRow">AssetRows</see>
        ''' which have this <see cref="AssetRow" /> marked as their <see cref="ParentId" />.
        ''' </summary>
        ''' <returns>An <see cref="AssetCollection" /> of the children of this <see cref="AssetRow" />.</returns>
        ''' <remarks><see cref="GetPublishedRowByFirstId" /> is probably the more useful function, because each
        ''' Asset in a group has a different <see cref="ParentId" /> (the previous entry) but they all have the 
        ''' same <see cref="FirstId" />.</remarks>
        Public Function GetChildrenCollection() As AssetCollection
            Dim col As New AssetCollection

            Dim dr As SqlDataReader = DB.GetReader("select * from Asset where ParentId = " & AssetId)
            While dr.Read()
                Dim dbAsset As New AssetRow(DB)
                dbAsset.Load(dr)
                col.Add(dbAsset)
            End While
            dr.Close()
            dr = Nothing

            Return col
        End Function

        ''' <summary>
        ''' Retrieves five pieces of related content for the specified <paramref name="AssetId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetId">The <see cref="AssetId" /> to retrieve related content for.</param>
        ''' <returns>Five pieces of content related via keyword to the specified <paramref name="AssetId" />.</returns>
        ''' <remarks>This method calls <see cref="GetRelatedContentByKeyword" /> with the default ItemCount.</remarks>
        Public Shared Function GetRelatedContent(ByVal DB As Database, ByVal AssetId As Integer) As DataTable
            Dim dt As DataTable = GetRelatedContentByKeyword(DB, AssetId)

            Return dt
        End Function

        ''' <summary>
        ''' Retrieves <paramref name="ItemCount" /> pieces of related content for the specified 
        ''' <paramref name="AssetId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetId">The <see cref="AssetId" /> to retrieve related content for.</param>
        ''' <param name="ItemCount">The number of items of related content to retrieve.  The default value of
        ''' this parameter is 5.</param>
        ''' <returns>A <see cref="DataTable" /> with <paramref name="ItemCount" /> rows of content related by
        ''' keyword to the specified <paramref name="AssetId" />.</returns>
        ''' <remarks>Types of content include:
        ''' <list type="bullet">
        ''' <item><description>Agendas</description></item>
        ''' <item><description>News Articles</description></item>
        ''' <item><description>Assets</description></item>
        ''' <item><description>Blog Posts</description></item>
        ''' <item><description>Photo Galleries</description></item>
        ''' <item><description>Video Links</description></item>
        ''' <item><description>Pages</description></item>
        ''' </list>
        ''' <para>It should be noted that this method will currently generate a <see cref="SqlException" /> if called
        ''' on account of the fact that some of these tables are currently not yet a part of Standards.</para>
        ''' <para>The fields returned are as follows:</para>
        ''' <list type="table">
        ''' <listheader>
        '''   <term>Field Name</term>
        '''   <description>Value</description>
        ''' </listheader>
        ''' <item>
        '''   <term>LinkName</term>
        '''   <description>A friendly name for the piece of linked content.</description>
        ''' </item>
        ''' <item>
        '''   <term>ItemDate</term>
        '''   <description>The primary date associated with the piece of content.</description>
        ''' </item>
        ''' <item>
        '''   <term>LinkUrl</term>
        '''   <description>The URL of the content.</description>
        ''' </item>
        ''' <item>
        '''   <term>CommonKeywordCount</term>
        '''   <description>The number of keywords shared between the current asset and the piece of related
        '''   content.</description>
        ''' </item>
        ''' </list>
        ''' </remarks>
        Public Shared Function GetRelatedContentByKeyword(ByVal DB As Database, ByVal AssetId As Integer, Optional ByVal ItemCount As Integer = 5) As DataTable
            Dim SQL As String = String.Empty
            Dim KeywordId As String = String.Empty

            ' Function will pull <ItemCount> number of related content items, matched by keyword
            ' The function could use OrderBy ItemDate to pull most recent items,
            ' or could use OrderBy CommonKeywordCount to pull down items with highest incidence of related keywords
            ' or could even use some kind of math so that only items from the past <x> days are included and then sort by common keyword count
            ' or whatever you can imagine
            ' --- cjw ---

            SQL &= " select KeywordId FROM Asset a, AssetKeyword ak where a.AssetId = " & DB.Number(AssetId) & " and a.Status = 'Published' and a.AssetId = ak.AssetId "
            Dim dt As DataTable = DB.GetDataTable(SQL)
            For Each row As DataRow In dt.Rows
                If KeywordId <> String.Empty Then KeywordId &= ","
                KeywordId &= Convert.ToString(row("KeywordId"))
            Next

            If KeywordId = String.Empty Then Return Nothing

            SQL = String.Empty
            SQL &= " select top " & DB.Number(ItemCount) & " LinkName, LinkUrl from "
            SQL &= " ( "
            SQL &= " select a.Headline as LinkName, a.PostDate as ItemDate, case when a.CustomUrl is null then '/AgendaArticle.aspx?AgendaId=' + convert(varchar, a.AgendaId) else a.CustomUrl end as LinkUrl, count(*) as CommonKeywordCount "
            SQL &= " from Agenda a, AgendaKeyword ak "
            SQL &= " where a.Status = 'Published' and a.PostDate <= getdate()  and a.AgendaId = ak.AgendaId and ak.KeywordId in " & DB.NumberMultiple(KeywordId)
            SQL &= " group by a.Headline, a.PostDate, a.CustomUrl, a.AgendaId "

            SQL &= " union "

            SQL &= " select n.Headline as LinkName, n.PostDate as ItemDate, case when n.CustomUrl is null then '/PressOfficeArticle.aspx?ArticleId=' + convert(varchar, n.ArticleId) else n.CustomUrl end as LinkUrl, count(*) as CommonKeywordCount "
            SQL &= " from NewsArticle n, NewsArticleKeyword nk "
            SQL &= " where n.Status = 'Published' and n.PostDate <= getdate()  and n.ArticleId = nk.ArticleId and nk.KeywordId in " & DB.NumberMultiple(KeywordId)
            SQL &= " group by n.Headline, n.PostDate, n.CustomUrl, n.ArticleId "

            SQL &= " union "

            SQL &= " select a.AssetName as LinkName, a.AssetDate as ItemDate, '/Asset.aspx?AssetId=' + convert(varchar, a.AssetId) as LinkUrl, count(*) as CommonKeywordCount "
            SQL &= " from Asset a, AssetKeyword ak "
            SQL &= " where a.AssetId <> " & DB.Number(AssetId) & " and a.Status = 'Published' and a.AssetId = ak.AssetId and ak.KeywordId in " & DB.NumberMultiple(KeywordId)
            SQL &= " group by a.AssetName, a.AssetDate, a.AssetId "

            SQL &= " union "

            SQL &= " select bp.PostTitle as LinkName, bp.PostDate as ItemDate, case when bp.PostUrl is null then '/BlogPost.aspx?BlogPostId=' + convert(varchar, bp.PostId) else bp.PostUrl end as LinkUrl, count(*) as CommonKeywordCount "
            SQL &= " from BlogPost bp, BlogPostKeyword bpk "
            SQL &= " where bp.Status = 'Published' and bp.PostDate <= getdate() and bp.PostId = bpk.PostId and bpk.KeywordId in " & DB.NumberMultiple(KeywordId)
            SQL &= " group by bp.PostTitle, bp.PostDate, bp.PostUrl, bp.PostId "

            SQL &= " union "

            SQL &= " select pg.PhotoGalleryName as LinkName, pg.GalleryDate as ItemDate, case when pg.CustomUrl is null then '/Slideshow.aspx?PhotoGalleryId=' + convert(varchar, pg.PhotoGalleryId) else pg.CustomUrl end as LinkUrl, count(*) as CommonKeywordCount "
            SQL &= " from PhotoGallery pg, PhotoGalleryKeyword pgk "
            SQL &= " where pg.Status = 'Published' and pg.GalleryDate <= getdate() and pg.PhotoGalleryId = pgk.PhotoGalleryId and pgk.KeywordId in " & DB.NumberMultiple(KeywordId)
            SQL &= " group by pg.PhotoGalleryName, pg.GalleryDate, pg.CustomUrl, pg.PhotoGalleryId "

            SQL &= " union "

            SQL &= " select vl.VideoTitle as ItemName, vl.PostDate as ItemDate, case when vl.CustomUrl is null then '/Video.aspx?VideoId=' + convert(varchar, vl.VideoId) else vl.CustomUrl end as LinkUrl, count(*) as CommonKeywordCount "
            SQL &= " from VideoLink vl, VideoKeyword vlk "
            SQL &= " where vl.Status = 'Published' and vl.PostDate <= getdate() and vl.VideoId = vlk.VideoId and vlk.KeywordId in " & DB.NumberMultiple(KeywordId)
            SQL &= " group by vl.VideoTitle, vl.PostDate, vl.CustomUrl, vl.VideoId "

            SQL &= " union "

            SQL &= " select ctp.Title as ItemName, ctp.PublishDate as ItemDate, case when ctp.CustomUrl is null then '/Page.aspx?PageId=' + convert(varchar, ctp.PageId) else ctp.CustomUrl end as LinkUrl, count(*) as CommonKeywordCount "
            SQL &= " from ContentToolPage ctp, ContentToolPageKeyword ctpk "
            SQL &= " where ctp.Status = 'Published' and ctp.PublishDate <= getdate() and ctp.PageId = ctpk.PageId and ctpk.KeywordId in " & DB.NumberMultiple(KeywordId)
            SQL &= " group by ctp.Title, ctp.PublishDate, ctp.CustomUrl, ctp.PageId "

            SQL &= " ) as x "
            SQL &= " order by ItemDate desc, CommonKeywordCount desc "

            Return DB.GetDataTable(SQL)
        End Function

        ''' <summary>
        ''' Retrieves all the Assets which have a <see cref="Status" /> marked as "Published".
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <returns>A <see cref="DataTable" /> of published Assets.</returns>
        ''' <remarks>This method does not sort its results.</remarks>
        Public Shared Function GetPublishedAssets(ByVal DB As Database) As DataTable
            Return DB.GetDataTable("select * from Asset where Status='Published'")
        End Function
    End Class

    ''' <summary>
    ''' Represents the field-by-field internal implementation of a row in the Asset table in the database.
    ''' </summary>
    ''' <remarks>This class should be regenerated automatically whenever a database change occurs on Asset.</remarks>
    Public MustInherit Class AssetRowBase
        Private m_DB As Database
        Private m_AssetId As Integer = Nothing
        Private m_ParentId As Integer = Nothing
        Private m_FirstId As Integer = Nothing
        Private m_AssetTypeId As Integer = Nothing
        Private m_ExtensionId As Integer = Nothing
        Private m_Filename As String = Nothing
        Private m_FilenameStaging As String = Nothing
        Private m_AssetToolId As Integer = Nothing
        Private m_ToolSectionUrl As String = Nothing
        Private m_AssetName As String = Nothing
        Private m_AssetDate As DateTime = Nothing
        Private m_IsPremium As Boolean = Nothing
        Private m_AssetWidth As Integer = Nothing
        Private m_AssetHeight As Integer = Nothing
        Private m_AssetSize As Integer = Nothing
        Private m_TempURL As String = Nothing
        Private m_Version As Integer = Nothing
        Private m_Status As String = Nothing
        Private m_StatusDate As DateTime = Nothing
        Private m_PublishDate As DateTime = Nothing
        Private m_ArchiveDate As DateTime = Nothing
        Private m_CreateAdminId As Integer = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_ModifyAdminId As Integer = Nothing
        Private m_ModifyDate As DateTime = Nothing
        Private m_TTL As Integer = Nothing
        Private m_ShowDeleted As Boolean = Nothing
        Private m_ShowPending As Boolean = Nothing
        Private m_Caption As String = Nothing
        Private m_Credit As String = Nothing
        Private m_DetailsIncomplete As Boolean = Nothing
        Private m_JustUploaded As Boolean = Nothing
        Private m_SiteId As Integer = Nothing
        Private m_AlternateText As String = Nothing
        Private m_IsDelete As Boolean = Nothing

        ''' <summary>
        ''' Gets or sets the value of the AssetId field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AssetId in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property AssetId() As Integer
            Get
                Return m_AssetId
            End Get
            Set(ByVal Value As Integer)
                m_AssetId = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the TTL field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on TTL in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property TTL() As Integer
            Get
                Return m_TTL
            End Get
            Set(ByVal Value As Integer)
                m_TTL = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the ParentId field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on ParentId in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property ParentId() As Integer
            Get
                Return m_ParentId
            End Get
            Set(ByVal Value As Integer)
                m_ParentId = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the FirstId field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on FirstId in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property FirstId() As Integer
            Get
                Return m_FirstId
            End Get
            Set(ByVal Value As Integer)
                m_FirstId = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the AssetTypeId field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AssetTypeId in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property AssetTypeId() As Integer
            Get
                Return m_AssetTypeId
            End Get
            Set(ByVal Value As Integer)
                m_AssetTypeId = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the ExtensionId field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on ExtensionId in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property ExtensionId() As Integer
            Get
                Return m_ExtensionId
            End Get
            Set(ByVal Value As Integer)
                m_ExtensionId = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the Filename field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on Filename in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property Filename() As String
            Get
                Return m_Filename
            End Get
            Set(ByVal Value As String)
                m_Filename = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the FilenameStaging field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on FilenameStaging in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property FilenameStaging() As String
            Get
                Return m_FilenameStaging
            End Get
            Set(ByVal Value As String)
                m_FilenameStaging = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the AssetToolId field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AssetToolId in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property AssetToolId() As Integer
            Get
                Return m_AssetToolId
            End Get
            Set(ByVal Value As Integer)
                m_AssetToolId = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the ToolSectionURL field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on ToolSectionURL in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property ToolSectionUrl() As String
            Get
                Return m_ToolSectionUrl
            End Get
            Set(ByVal Value As String)
                m_ToolSectionUrl = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the AssetName field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AssetName in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property AssetName() As String
            Get
                Return m_AssetName
            End Get
            Set(ByVal Value As String)
                m_AssetName = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the AssetDate field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="DateTime" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AssetDate in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property AssetDate() As DateTime
            Get
                Return m_AssetDate
            End Get
            Set(ByVal Value As DateTime)
                m_AssetDate = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the IsPremium field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Boolean" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on IsPremium in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property IsPremium() As Boolean
            Get
                Return m_IsPremium
            End Get
            Set(ByVal Value As Boolean)
                m_IsPremium = Value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the value of the IsDelete field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Boolean" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on IsDelete in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property IsDelete() As Boolean
            Get
                Return m_IsDelete
            End Get
            Set(ByVal Value As Boolean)
                m_IsDelete = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the JustUploaded field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Boolean" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on JustUploaded in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property JustUploaded() As Boolean
            Get
                Return m_JustUploaded
            End Get
            Set(ByVal Value As Boolean)
                m_JustUploaded = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the AssetWidth field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AssetWidth in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property AssetWidth() As Integer
            Get
                Return m_AssetWidth
            End Get
            Set(ByVal Value As Integer)
                m_AssetWidth = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the AssetHeight field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AssetHeight in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property AssetHeight() As Integer
            Get
                Return m_AssetHeight
            End Get
            Set(ByVal Value As Integer)
                m_AssetHeight = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the AssetSize field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AssetSize in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property AssetSize() As Integer
            Get
                Return m_AssetSize
            End Get
            Set(ByVal Value As Integer)
                m_AssetSize = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the TempURL field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on TempURL in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property TempURL() As String
            Get
                Return m_TempURL
            End Get
            Set(ByVal Value As String)
                m_TempURL = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the Version field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on Version in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property Version() As Integer
            Get
                Return m_Version
            End Get
            Set(ByVal Value As Integer)
                m_Version = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the Status field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on Status in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property Status() As String
            Get
                Return m_Status
            End Get
            Set(ByVal Value As String)
                m_Status = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the StatusDate field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="DateTime" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on StatusDate in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property StatusDate() As DateTime
            Get
                Return m_StatusDate
            End Get
            Set(ByVal Value As DateTime)
                m_StatusDate = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the PublishDate field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="DateTime" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on PublishDate in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property PublishDate() As DateTime
            Get
                Return m_PublishDate
            End Get
            Set(ByVal Value As DateTime)
                m_PublishDate = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the ArchiveDate field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="DateTime" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on ArchiveDate in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property ArchiveDate() As DateTime
            Get
                Return m_ArchiveDate
            End Get
            Set(ByVal Value As DateTime)
                m_ArchiveDate = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the CreateAdminId field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on CreateAdminId in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property CreateAdminId() As Integer
            Get
                Return m_CreateAdminId
            End Get
            Set(ByVal Value As Integer)
                m_CreateAdminId = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the ModifyAdminId field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on ModifyAdminId in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property ModifyAdminId() As Integer
            Get
                Return m_ModifyAdminId
            End Get
            Set(ByVal Value As Integer)
                m_ModifyAdminId = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the Caption field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on Caption in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property Caption() As String
            Get
                Return m_Caption
            End Get
            Set(ByVal value As String)
                m_Caption = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the Credit field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on Credit in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property Credit() As String
            Get
                Return m_Credit
            End Get
            Set(ByVal value As String)
                m_Credit = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the AlternateText field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AlternateText in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property AlternateText() As String
            Get
                Return m_AlternateText
            End Get
            Set(ByVal value As String)
                m_AlternateText = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the ShowPending field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Boolean" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on ShowPending in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property ShowPending() As Boolean
            Get
                Return m_ShowPending
            End Get
            Set(ByVal Value As Boolean)
                m_ShowPending = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the ShowDeleted field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Boolean" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on ShowDeleted in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property ShowDeleted() As Boolean
            Get
                Return m_ShowDeleted
            End Get
            Set(ByVal Value As Boolean)
                m_ShowDeleted = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the DetailsIncomplete field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Boolean" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on DetailsIncomplete in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property DetailsIncomplete() As Boolean
            Get
                Return m_DetailsIncomplete
            End Get
            Set(ByVal Value As Boolean)
                m_DetailsIncomplete = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets the value of the CreateDate field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="DateTime" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on CreateDate in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
        End Property

        ''' <summary>
        ''' Gets the value of the ModifyDate field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="DateTime" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on ModifyDate in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public ReadOnly Property ModifyDate() As DateTime
            Get
                Return m_ModifyDate
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the value of the SiteId field in Asset in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on SiteId in Asset in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property SiteId() As Integer
            Get
                Return m_SiteId
            End Get
            Set(ByVal value As Integer)
                m_SiteId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a reference to the database for this application.
        ''' </summary>
        ''' <value>A <see cref="Database" /> object for this application.</value>
        ''' <remarks>This property should be set before any SQL commands are executed using this class.</remarks>
        Public Property DB() As Database
            Get
                DB = m_DB
            End Get
            Set(ByVal Value As Database)
                m_DB = Value
            End Set
        End Property

        ''' <overloads>Initializes a new instance of the <see cref="AssetRowBase" /> class.</overloads>
        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetRowBase" /> class using default settings.
        ''' </summary>
        Public Sub New()
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetRowBase" /> class with the database connection
        ''' specified by <paramref name="DB" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <remarks>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.AssetRowBase.#ctor(Database,System.Int32)">New(Database, Integer)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</remarks>
        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetRowBase" /> class representing the row which uses
        ''' <paramref name="AssetId" /> as its primary key and <paramref name="DB" /> as its database connection.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetId">The primary key value of the row being referenced.</param>
        ''' <remarks>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.AssetRowBase.#ctor(Database)">New(Database)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</remarks>
        Public Sub New(ByVal DB As Database, ByVal AssetId As Integer)
            m_DB = DB
            m_AssetId = AssetId
        End Sub 'New

        ''' <overloads>Loads the contents of a row from the database.</overloads>
        ''' <summary>
        ''' Loads the row from the database specified by <see cref="AssetId" />.
        ''' </summary>
        ''' <remarks>This method calls 
        ''' <see cref="M:DataLayer.AssetRowBase.Load(System.Data.SqlClient.SqlDataReader)">Load(SqlDataReader)</see>.</remarks>
        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM Asset WHERE AssetId = " & DB.Number(AssetId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                AssetId = Nothing
            End If
            r.Close()
        End Sub

        ''' <summary>
        ''' Loads the contents of the row in the <see cref="SqlDataReader" /> into the 
        ''' <see cref="AssetRowBase" />.
        ''' </summary>
        ''' <param name="r">A <see cref="SqlDataReader" /> currently set to the row which should be loaded.</param>
        ''' <remarks>It is vital that all of the fields are properly loaded inside this method.</remarks>
        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_AssetId = Convert.ToInt32(r.Item("AssetId"))
            m_ParentId = Core.GetInt(r.Item("ParentId"))
            m_FirstId = Core.GetInt(r.Item("FirstId"))
            m_TTL = Convert.ToInt32(r.Item("TTL"))
            m_AssetTypeId = Convert.ToInt32(r.Item("AssetTypeId"))
            m_ExtensionId = Convert.ToInt32(r.Item("ExtensionId"))
            m_Filename = Convert.ToString(r.Item("Filename"))
            m_FilenameStaging = Convert.ToString(r.Item("FilenameStaging"))
            m_AssetName = Convert.ToString(r.Item("AssetName"))
            m_AssetDate = Convert.ToDateTime(r.Item("AssetDate"))
            m_DetailsIncomplete = Convert.ToBoolean(r.Item("DetailsIncomplete"))
            m_IsPremium = Convert.ToBoolean(r.Item("IsPremium"))
            m_JustUploaded = Convert.ToBoolean(r.Item("JustUploaded"))
            m_ShowDeleted = Convert.ToBoolean(r.Item("ShowDeleted"))
            m_ShowPending = Convert.ToBoolean(r.Item("ShowPending"))
            m_IsDelete = Core.GetBoolean(r.Item("IsDelete"))

            If IsDBNull(r.Item("AssetToolId")) Then
                m_AssetToolId = Nothing
            Else
                m_AssetToolId = Convert.ToInt32(r.Item("AssetToolId"))
            End If
            If IsDBNull(r.Item("ToolSectionURL")) Then
                m_ToolSectionUrl = Nothing
            Else
                m_ToolSectionUrl = Convert.ToString(r.Item("ToolSectionURL"))
            End If
            If IsDBNull(r.Item("AssetWidth")) Then
                m_AssetWidth = Nothing
            Else
                m_AssetWidth = Convert.ToInt32(r.Item("AssetWidth"))
            End If
            If IsDBNull(r.Item("AssetHeight")) Then
                m_AssetHeight = Nothing
            Else
                m_AssetHeight = Convert.ToInt32(r.Item("AssetHeight"))
            End If
            If IsDBNull(r.Item("AssetSize")) Then
                m_AssetSize = Nothing
            Else
                m_AssetSize = Convert.ToInt32(r.Item("AssetSize"))
            End If
            If IsDBNull(r.Item("TempURL")) Then
                m_TempURL = Nothing
            Else
                m_TempURL = Convert.ToString(r.Item("TempURL"))
            End If
            If IsDBNull(r.Item("Caption")) Then
                m_Caption = Nothing
            Else
                m_Caption = Convert.ToString(r.Item("Caption"))
            End If
            If IsDBNull(r.Item("Credit")) Then
                m_Credit = Nothing
            Else
                m_Credit = Convert.ToString(r.Item("Credit"))
            End If
            If IsDBNull(r.Item("AlternateText")) Then
                m_AlternateText = Nothing
            Else
                m_AlternateText = Convert.ToString(r.Item("AlternateText"))
            End If
            m_Version = Core.GetInt(r.Item("Version"))
            m_Status = Core.GetString(r.Item("Status"))
            m_StatusDate = Core.GetDate(r.Item("StatusDate"))
            m_PublishDate = Core.GetDate(r.Item("PublishDate"))
            m_ArchiveDate = Core.GetDate(r.Item("ArchiveDate"))
            m_CreateAdminId = Core.GetInt(r.Item("CreateAdminId"))
            m_CreateDate = Core.GetDate(r.Item("CreateDate"))
            m_ModifyAdminId = Core.GetInt(r.Item("ModifyAdminId"))
            m_ModifyDate = Core.GetDate(r.Item("ModifyDate"))
            m_SiteId = Core.GetInt(r.Item("SiteId"))
        End Sub 'Load

        ''' <summary>
        ''' Inserts the row into Asset using the values contained in the properties set on the 
        ''' <see cref="AssetRowBase" />.
        ''' </summary>
        ''' <returns>The value of AssetId for the new row.</returns>
        ''' <remarks>It is vital that all of the fields are properly inserted with their appropriate values 
        ''' inside this method.</remarks>
        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO Asset (" _
             & " AssetTypeId" _
             & ",ParentId" _
             & ",FirstId" _
             & ",ExtensionId" _
             & ",Filename" _
             & ",AssetToolId" _
             & ",ToolSectionUrl" _
             & ",AssetName" _
             & ",FilenameStaging" _
             & ",AssetDate" _
             & ",IsPremium" _
             & ",IsDelete" _
             & ",DetailsIncomplete" _
             & ",ShowPending" _
             & ",ShowDeleted" _
             & ",AssetWidth" _
             & ",AssetHeight" _
             & ",AssetSize" _
             & ",TempURL" _
             & ",Caption" _
             & ",Credit" _
             & ",AlternateText" _
             & ",Version" _
             & ",Status" _
             & ",StatusDate" _
             & ",PublishDate" _
             & ",ArchiveDate" _
             & ",CreateAdminId" _
             & ",CreateDate" _
             & ",ModifyAdminId" _
             & ",TTL" _
             & ",JustUploaded" _
             & ",ModifyDate" _
             & ",SiteId" _
             & ") VALUES (" _
             & m_DB.NullNumber(AssetTypeId) _
             & "," & m_DB.NullNumber(ParentId) _
             & "," & m_DB.NullNumber(FirstId) _
             & "," & m_DB.NullNumber(ExtensionId) _
             & "," & m_DB.Quote(Filename) _
             & "," & m_DB.NullNumber(AssetToolId) _
             & "," & m_DB.Quote(ToolSectionUrl) _
             & "," & m_DB.Quote(AssetName) _
             & "," & m_DB.Quote(FilenameStaging) _
             & "," & m_DB.NullQuote(AssetDate) _
             & "," & CInt(IsPremium) _
             & "," & CInt(IsDelete) _
             & "," & m_DB.Quote(DetailsIncomplete) _
             & "," & CInt(ShowPending) _
             & "," & CInt(ShowDeleted) _
             & "," & m_DB.Number(AssetWidth) _
             & "," & m_DB.Number(AssetHeight) _
             & "," & m_DB.Number(AssetSize) _
             & "," & m_DB.Quote(TempURL) _
             & "," & m_DB.Quote(Caption) _
             & "," & m_DB.Quote(Credit) _
             & "," & m_DB.Quote(AlternateText) _
             & "," & m_DB.Number(Version) _
             & "," & m_DB.Quote(Status) _
             & "," & m_DB.NullQuote(StatusDate) _
             & "," & m_DB.NullQuote(PublishDate) _
             & "," & m_DB.NullQuote(ArchiveDate) _
             & "," & m_DB.NullNumber(CreateAdminId) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(ModifyAdminId) _
             & "," & m_DB.Number(TTL) _
             & "," & m_DB.Quote(JustUploaded) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.Number(SiteId) _
             & ")"

            AssetId = m_DB.InsertSQL(SQL)

            If FirstId = Nothing Then
                FirstId = AssetId
                DB.ExecuteSQL("UPDATE Asset SET FirstId = " & AssetId & " WHERE AssetId = " & AssetId)
            End If

            Return AssetId
        End Function

        ''' <summary>
        ''' Updates the row in Asset using the values contained in the properties set on the 
        ''' <see cref="AssetRowBase" />.
        ''' </summary>
        ''' <remarks>It is vital that all of the fields are properly updated with their appropriate values 
        ''' inside this method.</remarks>
        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE Asset SET " _
             & " AssetTypeId = " & m_DB.NullNumber(AssetTypeId) _
             & ",ParentId = " & m_DB.NullNumber(ParentId) _
             & ",FirstId = " & m_DB.NullNumber(FirstId) _
             & ",ExtensionId = " & m_DB.NullNumber(ExtensionId) _
             & ",Filename = " & m_DB.Quote(Filename) _
             & ",AssetToolId = " & m_DB.NullNumber(AssetToolId) _
             & ",ToolSectionUrl = " & m_DB.Quote(ToolSectionUrl) _
             & ",AssetName = " & m_DB.Quote(AssetName) _
             & ",FilenameStaging = " & m_DB.Quote(FilenameStaging) _
             & ",AssetDate = " & m_DB.NullQuote(AssetDate) _
             & ",IsPremium = " & CInt(IsPremium) _
             & ",IsDelete = " & CInt(IsDelete) _
             & ",DetailsIncomplete = " & m_DB.Quote(DetailsIncomplete) _
             & ",ShowPending = " & CInt(ShowPending) _
             & ",ShowDeleted = " & CInt(ShowDeleted) _
             & ",AssetWidth = " & m_DB.Number(AssetWidth) _
             & ",AssetHeight = " & m_DB.Number(AssetHeight) _
             & ",AssetSize = " & m_DB.Number(AssetSize) _
             & ",Caption = " & m_DB.Quote(Caption) _
             & ",Credit = " & m_DB.Quote(Credit) _
             & ",AlternateText = " & m_DB.Quote(AlternateText) _
             & ",Version = " & m_DB.Number(Version) _
             & ",Status = " & m_DB.Quote(Status) _
             & ",StatusDate = " & m_DB.NullQuote(StatusDate) _
             & ",PublishDate = " & m_DB.NullQuote(PublishDate) _
             & ",ArchiveDate = " & m_DB.NullQuote(ArchiveDate) _
             & ",CreateAdminId = " & m_DB.NullNumber(CreateAdminId) _
             & ",ModifyAdminId = " & m_DB.NullNumber(ModifyAdminId) _
             & ",TTL = " & m_DB.Number(TTL) _
             & ",JustUploaded = " & CInt(JustUploaded) _
             & ",ModifyDate = " & m_DB.NullQuote(Now) _
             & ",SiteId = " & m_DB.Number(SiteId) _
             & " WHERE AssetId = " & m_DB.Quote(AssetId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    ''' <summary>
    ''' Represents a strongly-typed list of <see cref="AssetRow">AssetRows</see> that can be accessed by index.
    ''' </summary>
    ''' <remarks>This class is a wrapper for 
    ''' <see cref="T:Components.GenericCollection`1">GenericCollection(Of AssetRow)</see>.</remarks>
    Public Class AssetCollection
        Inherits GenericCollection(Of AssetRow)
    End Class

End Namespace


