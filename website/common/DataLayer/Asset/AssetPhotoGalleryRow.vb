Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    ''' <summary>
    ''' Represents a row in the AssetPhotoGallery table in the database.
    ''' </summary>
    ''' <remarks>Custom methods should be placed in this class, not in <see cref="AssetPhotoGalleryRowBase" />.</remarks>
    Public Class AssetPhotoGalleryRow
        Inherits AssetPhotoGalleryRowBase

        ''' <overloads>Initializes a new instance of the <see cref="AssetPhotoGalleryRow" /> class.</overloads>
        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetPhotoGalleryRow" /> class using default settings.
        ''' </summary>
        ''' <remarks>This constructor calls 
        ''' <see cref="M:DataLayer.AssetPhotoGalleryRowBase.#ctor">AssetPhotoGalleryRowBase.New</see>.</remarks>
        Public Sub New()
            MyBase.New()
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetPhotoGalleryRow" /> class with the database connection
        ''' specified by <paramref name="DB" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <remarks><para>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.AssetPhotoGalleryRow.#ctor(Database,System.Int32)">New(Database, Integer)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</para>
        ''' <para>This constructor calls 
        ''' <see cref="M:DataLayer.AssetPhotoGalleryRowBase.#ctor(Database)">AssetPhotoGalleryRowBase.New(Database)</see>.</para></remarks>
        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetPhotoGalleryRow" /> class representing the row which uses
        ''' <paramref name="AssetPhotoGalleryId" /> as its primary key and <paramref name="DB" /> as its database connection.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetPhotoGalleryId">The primary key value of the row being referenced.</param>
        ''' <remarks><para>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.AssetPhotoGalleryRow.#ctor(Database)">New(Database)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</para>
        ''' <para>This constructor calls 
        ''' <see cref="M:DataLayer.AssetPhotoGalleryRowBase.#ctor(Database,System.Int32)">AssetPhotoGalleryRowBase.New(Database, Integer)</see>.</para></remarks>
        Public Sub New(ByVal DB As Database, ByVal AssetPhotoGalleryId As Integer)
            MyBase.New(DB, AssetPhotoGalleryId)
        End Sub 'New

        ''' <summary>
        ''' Gets the row from the specified <see cref="Database" /> with the specified 
        ''' <paramref name="AssetPhotoGalleryId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetPhotoGalleryId">The primary key value of the row being retrieved.</param>
        ''' <returns>An instance of <see cref="AssetPhotoGalleryRow" /> loaded with the values from the specified 
        ''' row in the database.</returns>
        ''' <remarks>This method uses <see cref="M:DataLayer.AssetPhotoGalleryRowBase.Load">Load</see>.</remarks>
        Public Shared Function GetRow(ByVal DB As Database, ByVal AssetPhotoGalleryId As Integer) As AssetPhotoGalleryRow
            Dim row As AssetPhotoGalleryRow

            row = New AssetPhotoGalleryRow(DB, AssetPhotoGalleryId)
            row.Load()

            Return row
        End Function

        ''' <summary>
        ''' Removes the row from the specified <see cref="Database" /> with the specified 
        ''' <paramref name="AssetPhotoGalleryId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetPhotoGalleryId">The primary key value of the row being removed.</param>
        ''' <remarks>This method calls <see cref="Remove" />.</remarks>
        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal AssetPhotoGalleryId As Integer)
            Dim row As AssetPhotoGalleryRow

            row = New AssetPhotoGalleryRow(DB, AssetPhotoGalleryId)
            row.Remove()
        End Sub

        ''' <summary>
        ''' Retrieves the AssetPhotoGallery table from the specified <see cref="Database" /> ordered based on 
        ''' <paramref name="SortBy" /> and <paramref name="SortOrder" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="SortBy">The SQL field name to sort the results by.</param>
        ''' <param name="SortOrder">The order by which to sort the results (ASC, DESC).  The default value of this
        ''' parameter is "ASC".</param>
        ''' <returns>A <see cref="DataTable" /> containing the data returned by the query.</returns>
        ''' <remarks>If <paramref name="SortBy" /> is not provided, the data is not sorted during the query.</remarks>
        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from AssetPhotoGallery"
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
        ''' Retrieves a <see cref="DataTable" /> containing all of the Assets associated with the specified
        ''' <paramref name="PhotoGalleryId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="PhotoGalleryId">The primary key value of a <see cref="PhotoGalleryRow" /> to retrieve
        ''' assets for.</param>
        ''' <returns>A <see cref="DataTable" /> of assets used by the specified 
        ''' <paramref name="PhotoGalleryId" />.</returns>
        ''' <remarks>The Asset table is linked to the PhotoGallery table via the AssetPhotoGallery table.</remarks>
        Public Shared Function GetGalleryAssets(ByVal DB As Database, ByVal PhotoGalleryId As Integer) As DataTable
            Dim dt As DataTable
            Dim sql As New StringBuilder()

            sql.Append("SELECT   a.FileName, a.ToolSectionUrl, a.Status, a.ExtensionId, " & vbCrLf)
            sql.Append("         ap.* " & vbCrLf)
            sql.Append("FROM     dbo.asset a " & vbCrLf)
            sql.Append("         INNER JOIN dbo.assetphotogallery ap " & vbCrLf)
            sql.Append("           ON ap.assetid = a.assetid " & vbCrLf)
            sql.Append("         INNER JOIN dbo.photogallery g " & vbCrLf)
            sql.Append("           ON g.photogalleryid = ap.photogalleryid " & vbCrLf)
            sql.Append("              AND ap.photogalleryid = " & DB.Number(PhotoGalleryId) & " " & vbCrLf)
            sql.Append("ORDER BY ap.sortorder ASC")

            dt = DB.GetDataTable(sql.ToString())

            Return dt
        End Function

        ''' <summary>
        ''' Retrieves the number of Assets associated with the specified <paramref name="PhotoGalleryId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="PhotoGalleryId">The primary key value of a <see cref="PhotoGalleryRow" /> to retrieve
        ''' a count for.</param>
        ''' <returns>The number of Assets associated with the specified <paramref name="PhotoGalleryId" />, which
        ''' is equal to the number of results returned by <see cref="GetGalleryAssets" />.</returns>
        ''' <remarks>The Asset table is linked to the PhotoGallery table via the AssetPhotoGallery table.</remarks>
        Public Shared Function GetGalleryAssetCount(ByVal DB As Database, ByVal PhotoGalleryId As Integer) As Integer
            Dim count As Integer = 0
            Dim sql As New StringBuilder()

            sql.Append("SELECT   count(*) " & vbCrLf)
            sql.Append("FROM     dbo.asset a " & vbCrLf)
            sql.Append("         INNER JOIN dbo.assetphotogallery ap " & vbCrLf)
            sql.Append("           ON ap.assetid = a.assetid " & vbCrLf)
            sql.Append("         INNER JOIN dbo.photogallery g " & vbCrLf)
            sql.Append("           ON g.photogalleryid = ap.photogalleryid " & vbCrLf)
            sql.Append("              AND ap.photogalleryid = " & DB.Number(PhotoGalleryId) & " " & vbCrLf)

            Try
                count = DB.ExecuteScalar(sql.ToString())
            Catch ex As Exception
            End Try

            Return count
        End Function

        ''' <summary>
        ''' Changes the sort order of a photo gallery which is sortable using arrows.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="PhotoGalleryId">The primary key value of the photo gallery to swap rows in.</param>
        ''' <param name="AssetId">The primary key value of the asset in the photo gallery which is being moved.</param>
        ''' <param name="action">A <see cref="String" /> determining whether the row is being moved "ip" or
        ''' "down".</param>
        ''' <remarks>This method replaces the more generic
        ''' <see cref="Core.ChangeSortOrder">Core.ChangeSortOrder</see>.</remarks>
        Public Shared Sub UpdateSortOrder(ByVal DB As Database, ByVal PhotoGalleryId As Integer, ByVal AssetId As Integer, ByVal action As String)
            Dim order As Integer = DB.ExecuteScalar("Select [SortOrder] From dbo.AssetPhotoGallery Where [AssetId] = " & DB.Number(AssetId) & " And [PhotoGalleryId] = " & DB.Number(PhotoGalleryId))
            Dim dt As DataTable
            Dim sql As New StringBuilder()
            Dim moveUp As New StringBuilder()
            Dim moveDown As New StringBuilder()

            If action = "up" Then
                sql.Append("Select Top 1 * From dbo.AssetPhotoGallery Where [PhotoGalleryId] = " & DB.Number(PhotoGalleryId) & " ")
                sql.Append("And [SortOrder] < " & DB.Number(order) & " Order By [SortOrder] Desc")
                dt = DB.GetDataTable(sql.ToString())

                If dt.Rows.Count <> 0 Then
                    moveUp.Append("Update dbo.AssetPhotoGallery Set [SortOrder] = " & DB.Number(dt.Rows(0)("SortOrder")) & " ")
                    moveUp.Append("Where [AssetId] = " & DB.Number(AssetId) & " And [PhotoGalleryId] = " & DB.Number(PhotoGalleryId))

                    moveDown.Append("Update dbo.AssetPhotoGallery Set [SortOrder] = " & DB.Number(order) & " ")
                    moveDown.Append("Where [AssetId] = " & DB.Number(dt.Rows(0)("AssetId")) & " And [PhotoGalleryId] = " & DB.Number(PhotoGalleryId))
                End If
            ElseIf action = "down" Then
                sql.Append("Select Top 1 * From dbo.AssetPhotoGallery Where [PhotoGalleryId] = " & DB.Number(PhotoGalleryId) & " ")
                sql.Append("And [SortOrder] > " & DB.Number(order) & " Order By [SortOrder] Asc")
                dt = DB.GetDataTable(sql.ToString())

                If dt.Rows.Count <> 0 Then
                    moveUp.Append("Update dbo.AssetPhotoGallery Set [SortOrder] = " & DB.Number(order) & " ")
                    moveUp.Append("Where [AssetId] = " & DB.Number(dt.Rows(0)("AssetId")) & " And [PhotoGalleryId] = " & DB.Number(PhotoGalleryId))

                    moveDown.Append("Update dbo.AssetPhotoGallery Set [SortOrder] = " & DB.Number(dt.Rows(0)("SortOrder")) & " ")
                    moveDown.Append("Where [AssetId] = " & DB.Number(AssetId) & " And [PhotoGalleryId] = " & DB.Number(PhotoGalleryId))
                End If
            End If

            Try
                DB.BeginTransaction()
                DB.ExecuteScalar(moveUp.ToString())
                DB.ExecuteScalar(moveDown.ToString())
                DB.CommitTransaction()
            Catch ex As Exception
                If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            End Try
        End Sub
    End Class
End Namespace


