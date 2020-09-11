Imports Components
Imports DataLayer
Imports System.Web

Public Class AssetsHelper

    Private Const CacheRefreshInterval As Integer = 10

    Public Shared Function GetAssetDimensions(ByVal DB As Database) As DataTable
        Dim SQL As String = "SELECT AssetDimensionId,AssetWidth,AssetHeight FROM dbo.AssetDimension "
        Return DB.GetDataTable(SQL)
    End Function
    Public Shared Function GetAssetOtherDetails(ByVal DB As Database) As DataTable
        Dim SQL As String = "SELECT AssetId,FirstId,AlternateText,Caption,Credit,Status FROM dbo.Asset order by AssetId desc "
        'Dim sdep As System.Web.Caching.SqlCacheDependency = New System.Web.Caching.SqlCacheDependency("cache", "Asset")
        Return DB.GetDataTable(SQL)
    End Function
    Public Shared Function GetAssetforURL(ByVal db As Database) As DataTable
        Dim sql As String = "select a.Status,a.AssetId,a.ToolSectionURL,a.FileName,a.SiteId,ae.Extension,a.FirstId from dbo.Asset a Inner Join AssetExtension ae on a.ExtensionId = ae.ExtensionId order by AssetId desc"
        'Dim sdep As System.Web.Caching.SqlCacheDependency = New System.Web.Caching.SqlCacheDependency("cache", "Asset")
        Return db.GetDataTable(sql)
    End Function

    Public Shared Function GetAssetProperties(ByVal DB As Database, ByVal AssetId As Integer, ByVal FirstId As Integer) As AssetProperties
        Dim dbAsset As AssetRow = Nothing
        If Not FirstId = 0 Then
            dbAsset = AssetRow.GetPublishedRowByFirstId(DB, FirstId)
        Else
            dbAsset = AssetRow.GetRow(DB, AssetId)
        End If

        If Not dbAsset Is Nothing Then
            If dbAsset.AssetId = Nothing Then dbAsset = AssetRow.GetRow(DB, AssetId)
        End If

        Dim aprop As AssetProperties = New AssetProperties()
        aprop.AlternateText = dbAsset.AlternateText
        aprop.Caption = dbAsset.Caption
        aprop.AssetId = dbAsset.AssetId
        aprop.FirstId = dbAsset.FirstId
        aprop.Credit = dbAsset.Credit

        Return aprop
    End Function
    Public Shared Function GetAlternateText(ByVal Db As Database, ByVal AssetId As Integer, ByVal FirstId As Integer)
        Dim dt As DataTable = GetAssetOtherDetails(Db)
        Dim drows As DataRow() = dt.Select("(Status = 'Published' and FirstId=" & Db.Number(FirstId) & ") or (AssetId=" & Db.Number(AssetId) & ")")
        Dim AlternateText As String = String.Empty
        If drows IsNot Nothing AndAlso drows.Length > 0 Then
            AlternateText = Core.GetString(drows(0)("AlternateText"))
        End If
        Return AlternateText
    End Function
    Public Shared Function GetCaption(ByVal Db As Database, ByVal AssetId As Integer, ByVal FirstId As Integer)
        Dim dt As DataTable = GetAssetOtherDetails(Db)
        Dim drows As DataRow() = dt.Select("(Status = 'Published' and FirstId=" & Db.Number(FirstId) & ") or (AssetId=" & Db.Number(AssetId) & ")")
        Dim AlternateText As String = String.Empty
        If drows IsNot Nothing AndAlso drows.Length > 0 Then
            AlternateText = Core.GetString(drows(0)("Caption"))
        End If
        Return AlternateText
    End Function
    Public Shared Function GetCredit(ByVal Db As Database, ByVal AssetId As Integer, ByVal FirstId As Integer)
        Dim dt As DataTable = GetAssetOtherDetails(Db)
        Dim drows As DataRow() = dt.Select("(Status = 'Published' and FirstId=" & Db.Number(FirstId) & ") or (AssetId=" & Db.Number(AssetId) & ")")
        Dim AlternateText As String = String.Empty
        If drows IsNot Nothing AndAlso drows.Length > 0 Then
            AlternateText = Core.GetString(drows(0)("Credit"))
        End If
        Return AlternateText
    End Function
    Public Shared Function GetAssetOriginalDimension(ByVal Db As Database, ByVal AssetId As Integer, ByVal FirstId As Integer) As DataTable
        Dim dt As DataTable = Db.GetDataTable("select Top 1 a.AssetId,a.AssetWidth,a.AssetHeight from Asset a where (FirstId = " & Db.Quote(FirstId) & " and status = 'Published')  or (AssetId=" & Db.Quote(AssetId) & ") order by AssetId desc")
        Return dt
    End Function
    Public Shared Function getAssetDimensionsByTool(ByVal Db As Database, ByVal ToolId As Integer) As DataTable
        Return Db.GetDataTable("select ad.AssetDimensionId,ad.DimensionName,cast(ad.AssetWidth as nvarchar(50))  + ' X ' + cast(ad.AssetHeight as nvarchar(50)) as DimensionValue,ads.IsDefault from AssetDimension ad Inner join assetdimensiontool ads on ad.AssetDimensionId=ads.AssetDimensionId where ToolId = " & Db.Number(ToolId))
    End Function
    Public Shared Function GetAssetUrl(ByVal DB As Database, ByVal AssetId As Integer, ByVal AssetDimensionId As Integer, Optional ByVal FirstId As Integer = 0) As String
        Dim dt As DataTable = GetAssetforURL(DB)
        Dim drows As DataRow() = dt.Select("(FirstId = " & DB.Quote(FirstId) & " and status = 'Published')  or (AssetId=" & DB.Quote(AssetId) & ")")

        Dim drow As DataRow
        If drows IsNot Nothing AndAlso drows.Length > 0 Then
            drow = drows(0)
        Else
            Return String.Empty
        End If

        Dim assetwidth As String = String.Empty
        Dim assetheight As String = String.Empty

        If Not AssetDimensionId = Nothing Then
            Dim dtdimensions As DataTable = GetAssetDimensions(DB)
            Dim drDimension As DataRow() = dtdimensions.Select("AssetDimensionId=" & AssetDimensionId)
            If drDimension.Length > 0 Then
                If Not IsDBNull(drDimension(0)("AssetWidth")) Then assetwidth = drDimension(0)("AssetWidth")
                If Not IsDBNull(drDimension(0)("AssetHeight")) Then assetheight = drDimension(0)("AssetHeight")
            End If
        End If

        If IsDBNull(drow("ToolsectionUrl")) Then Return String.Empty
        Dim url As String
        If Not AssetDimensionId = Nothing AndAlso assetwidth IsNot Nothing AndAlso assetheight IsNot Nothing Then
            If drow("Status") = "Published" Then
                url = "/assets/" & drow("SiteId") & Convert.ToString(drow("ToolSectionUrl")).Replace("\", "/") & assetwidth & "x" & assetheight & "/" & drow("Filename")
            Else
                url = "/assets/" & drow("SiteId") & "/workflow_staging" & Convert.ToString(drow("ToolSectionUrl")).Replace("\", "/") & assetwidth & "x" & assetheight & "/" & drow("AssetId") & "." & drow("Extension")
            End If
        Else
            If drow("Status") = "Published" Then
                url = "/assets/" & drow("SiteId") & drow("ToolSectionUrl").ToString.Replace("\", "/") & drow("Filename")
            Else
                url = "/assets/" & drow("SiteId") & "/workflow_staging" & drow("ToolSectionUrl").ToString.Replace("\", "/") & drow("AssetId") & "." & drow("Extension")
            End If
        End If
        Return url
    End Function

    Public Shared Function GetFileUrl(ByVal DB As Database, ByVal AssetId As Integer, Optional ByVal FirstId As Integer = 0) As String
        Dim dt As DataTable = GetAssetforURL(DB)
        Dim drows As DataRow() = dt.Select("(FirstId = " & DB.Quote(FirstId) & " and status = 'Published')  or (AssetId=" & DB.Quote(AssetId) & ")")

        Dim drow As DataRow
        If drows IsNot Nothing AndAlso drows.Length > 0 Then
            drow = drows(0)
        Else
            Return String.Empty
        End If

        If IsDBNull(drow("ToolsectionUrl")) Then Return String.Empty
        Dim url As String

        If drow("Status") = "Published" Then
            url = "/assets/" & drow("SiteId") & drow("ToolSectionUrl").ToString.Replace("\", "/") & drow("Filename")
        Else
            url = "/assets/" & drow("SiteId") & "/workflow_staging" & drow("ToolSectionUrl").ToString.Replace("\", "/") & drow("AssetId") & "." & drow("Extension")
        End If

        Return url
    End Function


    Public Shared Function GetAssetThumbnailUrl(ByVal DB As Database, ByVal AssetId As Integer, Optional ByVal firstid As Integer = 0) As String
        Dim dt As DataTable = DB.GetDataTable("select a.AssetId,a.Status,a.ToolSectionURL,a.SiteId,a.FileName,ae.Extension,ae.AssetTypeId,ae.ImageName from Asset a Inner Join AssetExtension ae on a.ExtensionId = ae.ExtensionId where (FirstId = " & DB.Quote(firstid) & " and status = 'Published')  or (AssetId=" & DB.Quote(AssetId) & ") order by AssetId desc")

        Dim drow As DataRow
        If dt.Rows.Count >= 1 Then
            drow = dt.Rows(0)
        Else
            Return String.Empty
        End If

        If IsDBNull(drow("ToolsectionUrl")) Then Return String.Empty
        If drow("AssetTypeId") = GetImageAssetType(DB) Then
            If drow("Status") = "Published" Then
                Return "/assets/" & drow("SiteId") & drow("ToolSectionUrl").ToString.Replace("\", "/") & "asset_mgr_thumbs/" & drow("Filename")
            Else
                Return "/assets/" & drow("SiteId") & "/workflow_staging" & drow("ToolSectionUrl").ToString.Replace("\", "/") & "asset_mgr_thumbs/" & drow("AssetId") & "." & drow("Extension")
            End If
        Else
            Dim FileThumbnailLocation As String = "/cms/images/filetypes/"
            If Not IsDBNull(drow("ImageName")) Then
                Return FileThumbnailLocation & drow("ImageName")
            Else
                Return FileThumbnailLocation & "file.gif"
            End If
        End If
    End Function

    Public Shared Function GetImageAssetType(ByVal DB As Database) As String
        Dim ImageassetTypeId As Integer = Nothing
        Dim context As HttpContext = HttpContext.Current
        If Not context Is Nothing Then
            ImageassetTypeId = CType(context.Cache("ImageAssetTypeId"), Integer)
        End If
        If ImageassetTypeId = Nothing Then
            ImageassetTypeId = DB.ExecuteScalar("SELECT TOP 1 AssetTypeId FROM AssetType WHERE Name = 'Images'")
            context.Cache.Insert("ImageAssetTypeId", ImageassetTypeId, Nothing, DateTime.Now.AddSeconds(10), TimeSpan.Zero)
        End If
        Return ImageassetTypeId
    End Function

    Public Shared Function GetFileThumbnail(ByVal Extension As String) As String
        Dim FileThumbnailLocation As String = "/cms/images/filetypes/"
        Select Case Extension
            Case "DOC"
                FileThumbnailLocation &= "doc.jpg"
            Case "PDF"
                FileThumbnailLocation &= "pdf.jpg"
            Case "TXT"
                FileThumbnailLocation &= "txt.jpg"
            Case "MP3"
                FileThumbnailLocation &= "mp3.jpg"
            Case "MOV"
                FileThumbnailLocation &= "mov.jpg"
            Case "WMV"
                FileThumbnailLocation &= "wmv.jpg"
            Case Else
                FileThumbnailLocation &= "file.gif"
        End Select
        Return FileThumbnailLocation
    End Function

    Public Shared Function GetAssetUrlWithoutCache(ByVal DB As Database, ByVal AssetId As Integer, ByVal AssetDimensionId As Integer, Optional ByVal FirstId As Integer = 0) As String
        Dim sql As String = "select a.Status,a.AssetId,a.ToolSectionURL,a.FileName,a.SiteId,ae.Extension,a.FirstId from dbo.Asset a Inner Join AssetExtension ae on a.ExtensionId = ae.ExtensionId  where (FirstId = " & DB.Quote(FirstId) & " and status = 'Published')  or (AssetId=" & DB.Quote(AssetId) & ") order by AssetId desc "
        Dim dt As DataTable = DB.GetDataTable(sql)
        If dt Is Nothing Then Return String.Empty
        Dim drow As DataRow = dt.Rows(0)
        Dim assetwidth As String = String.Empty
        Dim assetheight As String = String.Empty

        If Not AssetDimensionId = Nothing Then
            Dim dtdimensions As DataTable = GetAssetDimensions(DB)
            Dim drDimension As DataRow() = dtdimensions.Select("AssetDimensionId=" & AssetDimensionId)
            If drDimension.Length > 0 Then
                If Not IsDBNull(drDimension(0)("AssetWidth")) Then assetwidth = drDimension(0)("AssetWidth")
                If Not IsDBNull(drDimension(0)("AssetHeight")) Then assetheight = drDimension(0)("AssetHeight")
            End If
        End If

        If IsDBNull(drow("ToolsectionUrl")) Then Return String.Empty
        Dim url As String
        If Not AssetDimensionId = Nothing AndAlso assetwidth IsNot Nothing AndAlso assetheight IsNot Nothing Then
            If drow("Status") = "Published" Then
                url = "/assets/" & drow("SiteId") & Convert.ToString(drow("ToolSectionUrl")).Replace("\", "/") & assetwidth & "x" & assetheight & "/" & drow("Filename")
            Else
                url = "/assets/" & drow("SiteId") & "/workflow_staging" & Convert.ToString(drow("ToolSectionUrl")).Replace("\", "/") & assetwidth & "x" & assetheight & "/" & drow("AssetId") & "." & drow("Extension")
            End If
        Else
            If drow("Status") = "Published" Then
                url = "/assets/" & drow("SiteId") & drow("ToolSectionUrl").ToString.Replace("\", "/") & drow("Filename")
            Else
                url = "/assets/" & drow("SiteId") & "/workflow_staging" & drow("ToolSectionUrl").ToString.Replace("\", "/") & drow("AssetId") & "." & drow("Extension")
            End If
        End If
        Return url
    End Function

    Public Shared Function GetExtension(ByVal DB As Database, ByVal Extension As String) As AssetExtensionRow
        Dim ExtensionId As Integer = DB.ExecuteScalar("SELECT ExtensionId FROM AssetExtension WHERE Extension = " & DB.Quote(Extension))
        Return AssetExtensionRow.GetRow(DB, ExtensionId)
    End Function
    Public Shared Function GetExtensionById(ByVal db As Database, ByVal ExtensionId As Integer) As String
        Dim dbAssetExtensionRow As AssetExtensionRow = AssetExtensionRow.GetRow(db, ExtensionId)
        Return dbAssetExtensionRow.Extension
    End Function
End Class

Public Class AssetProperties
    Private m_AssetId As Integer
    Private m_FirstId As Integer
    Private m_AlternateText As String
    Private m_Caption As String
    Private m_Credit As String
    Public Property AssetId() As Integer
        Get
            Return m_AssetId
        End Get
        Set(ByVal value As Integer)
            m_AssetId = value
        End Set
    End Property
    Public Property AlternateText() As String
        Get
            Return m_AlternateText
        End Get
        Set(ByVal value As String)
            m_AlternateText = value
        End Set
    End Property
    Public Property Caption() As String
        Get
            Return m_Caption
        End Get
        Set(ByVal value As String)
            m_Caption = value
        End Set
    End Property
    Public Property Credit() As String
        Get
            Return m_Credit
        End Get
        Set(ByVal value As String)
            m_Credit = value
        End Set
    End Property
    Public Property FirstId() As Integer
        Get
            Return m_FirstId
        End Get
        Set(ByVal value As Integer)
            m_FirstId = value
        End Set
    End Property

End Class
