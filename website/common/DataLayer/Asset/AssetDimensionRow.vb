Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    ''' <summary>
    ''' Represents a row in the AssetDimension table in the database.
    ''' </summary>
    ''' <remarks>Custom methods should be placed in this class, not in <see cref="AssetDimensionRowBase" />.</remarks>
    Public Class AssetDimensionRow
        Inherits AssetDimensionRowBase

        ''' <overloads>Initializes a new instance of the <see cref="AssetDimensionRow" /> class.</overloads>
        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetDimensionRow" /> class using default settings.
        ''' </summary>
        ''' <remarks>This constructor calls 
        ''' <see cref="M:DataLayer.AssetDimensionRowBase.#ctor">AssetDimensionRowBase.New</see>.</remarks>
        Public Sub New()
            MyBase.New()
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetDimensionRow" /> class with the database connection
        ''' specified by <paramref name="DB" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <remarks><para>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.AssetDimensionRow.#ctor(Database,System.Int32)">New(Database, Integer)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</para>
        ''' <para>This constructor calls 
        ''' <see cref="M:DataLayer.AssetDimensionRowBase.#ctor(Database)">AssetDimensionRowBase.New(Database)</see>.</para></remarks>
        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetDimensionRow" /> class representing the row which uses
        ''' <paramref name="AssetDimensionId" /> as its primary key and <paramref name="DB" /> as its database connection.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetDimensionId">The primary key value of the row being referenced.</param>
        ''' <remarks><para>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.AssetDimensionRow.#ctor(Database)">New(Database)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</para>
        ''' <para>This constructor calls 
        ''' <see cref="M:DataLayer.AssetDimensionRowBase.#ctor(Database,System.Int32)">AssetDimensionRowBase.New(Database, Integer)</see>.</para></remarks>
        Public Sub New(ByVal DB As Database, ByVal AssetDimensionId As Integer)
            MyBase.New(DB, AssetDimensionId)
        End Sub 'New

        ''' <summary>
        ''' Gets the row from the specified <see cref="Database" /> with the specified 
        ''' <paramref name="AssetDimensionId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetDimensionId">The primary key value of the row being retrieved.</param>
        ''' <returns>An instance of <see cref="AssetDimensionRow" /> loaded with the values from the specified 
        ''' row in the database.</returns>
        ''' <remarks>This method uses <see cref="M:DataLayer.AssetDimensionRowBase.Load">Load</see>.</remarks>
        Public Shared Function GetRow(ByVal DB As Database, ByVal AssetDimensionId As Integer) As AssetDimensionRow
            Dim row As AssetDimensionRow

            row = New AssetDimensionRow(DB, AssetDimensionId)
            row.Load()

            Return row
        End Function

        ''' <summary>
        ''' Removes the row from the specified <see cref="Database" /> with the specified 
        ''' <paramref name="AssetDimensionId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetDimensionId">The primary key value of the row being removed.</param>
        ''' <remarks>This method calls <see cref="Remove" />.</remarks>
        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal AssetDimensionId As Integer)
            Dim row As AssetDimensionRow

            row = New AssetDimensionRow(DB, AssetDimensionId)
            row.Remove()
        End Sub

        ''' <summary>
        ''' Retrieves the AssetDimension table from the specified <see cref="Database" /> ordered based on 
        ''' <paramref name="SortBy" /> and <paramref name="SortOrder" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="SortBy">The SQL field name to sort the results by.</param>
        ''' <param name="SortOrder">The order by which to sort the results (ASC, DESC).  The default value of this
        ''' parameter is "ASC".</param>
        ''' <returns>A <see cref="DataTable" /> containing the data returned by the query.</returns>
        ''' <remarks>If <paramref name="SortBy" /> is not provided, the data is not sorted during the query.</remarks>
        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from AssetDimension"
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
        ''' Retrieves a <see cref="DataTable" /> containing all the AssetDimensions linked to their associated
        ''' WorkflowTools.
        ''' </summary>
        ''' <param name="db">A reference to the <see cref="Database" /> for the application.</param>
        ''' <returns>A <see cref="DataTable" /> containing information on the AssetDimensions associated with each
        ''' WorkflowTool (and information on each such WorkflowTool as well).</returns>
        ''' <remarks><para>Table WorkflowTool is linked to table AssetDimension through table 
        ''' AssetDimensionTool.</para>
        ''' <para>All fields from AssetDimension and WorkflowTool are returned, along with 
        ''' IsDefault from AssetDimensionTool.</para></remarks>
        Public Shared Function GetDimensionsForAllTools(ByVal db As Database) As DataTable
            Return db.GetDataTable("select ad.*,wt.*,adas.isdefault from assetdimensiontool adas inner join WorkflowTool wt on wt.ToolId=adas.ToolId inner join assetdimension ad  on ad.AssetDimensionId = adas.AssetDimensionId")
        End Function

        ''' <summary>
        ''' Retrieves a <see cref="DataTable" /> containing all the AssetDimensions associated with the specified
        ''' <paramref name="ToolId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="ToolId">The primary key value of the tool to get asset dimensions for.</param>
        ''' <returns>A <see cref="DataTable" /> of all the asset dimensions for the specified tool.</returns>
        ''' <remarks>This method employs a SELECT DISTINCT in order to purify results.</remarks>
        Public Shared Function GetAllAssetDimensionsByTool(ByVal DB As Database, ByVal ToolId As Integer) As DataTable
            Return DB.GetDataTable("SELECT DISTINCT ad.* FROM AssetDimension ad Inner Join assetdimensiontool adas on adas.AssetDimensionId=ad.AssetDimensionId WHERE adas.ToolId = " & DB.Number(ToolId))
        End Function

        ''' <summary>
        ''' Retrieves the default <see cref="AssetDimensionId" /> for the tool with the specified <paramref name="Code" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="Code">A value of the <see cref="AdminActionRow.Code" /> field in the AdminAction table
        ''' which refers to a specific tool.</param>
        ''' <returns>The primary key associated with the default asset dimension for the specified tool.</returns>
        ''' <remarks>This method employs a SELECT DISTINCT in order to purify results.</remarks>
        Public Shared Function GetDefaultAssetDimensionId(ByVal DB As Database, ByVal Code As String) As Integer
            Return DB.ExecuteScalar("SELECT DISTINCT adas.AssetDimensionId FROM AdminAction as1, assetdimensiontool adas WHERE as1.ActionId = adas.ToolId AND as1.Code = " & DB.Quote(Code))
        End Function

        ''' <summary>
        ''' Retrieves the AssetDimension table from the specified <see cref="Database" /> ordered by
        ''' <see cref="DimensionName" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <returns>A <see cref="DataTable" /> containing the data returned by the query.</returns>
        ''' <remarks>This method is deprecated.  Use <see cref="GetList" /> with SortBy set to "DimensionName".</remarks>
        Public Shared Function GetAllAssetDimensions(ByVal DB As Database) As DataTable
            Dim dt As DataTable = DB.GetDataTable("select * from AssetDimension WHERE ParentId IS NULL order by DimensionName")
            Return dt
        End Function

        ''' <summary>
        ''' Retrieves the AssetDimension table from the specified <see cref="Database" /> ordered by
        ''' <see cref="DimensionName" />, automatically excluding the specified 
        ''' <paramref name="AssetDimensionId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetDimensionId">The primary key value of the asset dimension to exclude.</param>
        ''' <returns>A <see cref="DataTable" /> containing the data returned by the query.</returns>
        Public Shared Function GetSelectiveAssetDimensions(ByVal DB As Database, ByVal AssetDimensionId As Integer) As DataTable
            Dim dt As DataTable = DB.GetDataTable("select * from AssetDimension WHERE AssetDimensionId <> " & DB.Number(AssetDimensionId) & " AND ParentId IS NULL OR ParentId = 0 order by DimensionName")
            Return dt
        End Function

        ''' <summary>
        ''' Gets a <see cref="DataTable" /> containing the width and height of the specified 
        ''' <paramref name="AssetDimensionId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetDimensionId">The primary key value of the <see cref="AssetDimensionRow" /> to
        ''' retrieve information for.</param>
        ''' <returns>A <see cref="DataTable" /> with two fields: Width, and Height.</returns>
        ''' <remarks>This method was used with the old, hierarchical asset dimensions.</remarks>
        Public Shared Function GetDimensions(ByVal DB As Database, ByVal AssetDimensionId As String) As DataTable
            Dim dt As DataTable = DB.GetDataTable("SELECT AssetWidth, AssetHeight FROM AssetDimension WHERE AssetDimensionId = " & DB.Number(AssetDimensionId) & " OR ParentId = " & DB.Number(AssetDimensionId))
            Dim dtDimensions As DataTable = New DataTable("DimensionsTable")
            dtDimensions.Columns.Add("Width")
            dtDimensions.Columns.Add("Height")
            For Each row As DataRow In dt.Rows
                Dim Width As String = Convert.ToString(row("AssetWidth"))
                Dim Height As String = Convert.ToString(row("AssetHeight"))
                Dim dtRow As DataRow = dtDimensions.NewRow()
                dtRow.Item("Width") = Width
                dtRow.Item("Height") = Height
                dtDimensions.Rows.Add(dtRow)
            Next
            Return dtDimensions
        End Function

        ''' <summary>
        ''' Updates the AssetDimensionTool table to associate <paramref name="AssetDimensionId" /> with
        ''' <paramref name="ToolId" /> in the row specified by <paramref name="Id" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="ToolId">The ToolId to use in the association.</param>
        ''' <param name="AssetDimensionId">The <see cref="AssetDimensionId" /> to use in the association.</param>
        ''' <param name="Id">The primary key value of the row to update.</param>
        ''' <returns>The number of rows affected by the query.</returns>
        ''' <remarks>This is used in the save method of the asset dimension groups page.</remarks>
        Public Shared Function UpdateDefaultAssetDimensionGroup(ByVal DB As Database, ByVal ToolId As Integer, ByVal AssetDimensionId As Integer, ByVal Id As Integer) As Integer
            Dim SQL As String

            SQL = " UPDATE assetdimensiontool SET " _
             & " AssetDimensionId = " & DB.Number(AssetDimensionId) _
             & ",ToolId = " & DB.Number(toolid) _
             & " WHERE Id = " & DB.Quote(Id)

            Return DB.ExecuteSQL(SQL)
        End Function

        ''' <summary>
        ''' Inserts into the AssetDimensionTool table in order to associate 
        ''' <paramref name="AssetDimensionId" /> with <paramref name="ToolId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="ToolId">The ToolId to use in the association.</param>
        ''' <param name="AssetDimensionId">The <see cref="AssetDimensionId" /> to use in the association.</param>
        ''' <returns>The primary key value of the new AssetDimensionTool row.</returns>
        ''' <remarks>This is used in the save method of the asset dimension groups page.</remarks>
        Public Shared Function InsertDefaultAssetDimensionGroup(ByVal DB As Database, ByVal ToolId As Integer, ByVal AssetDimensionId As Integer) As Integer
            Dim SQL As String

            SQL = " INSERT INTO assetdimensiontool (" _
             & " AssetDimensionId" _
             & ",ToolId" _
             & ") VALUES (" _
             & CInt(AssetDimensionId) _
             & "," & CInt(ToolId) _
             & ")"

            Return DB.InsertSQL(SQL)
        End Function

        ''' <summary>
        ''' Gets the <see cref="AssetDimensionId" /> of the row representing the default row for the specified
        ''' <paramref name="ToolId" />.
        ''' </summary>
        ''' <param name="db">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="ToolId">The value of ToolId in the WHERE clause.</param>
        ''' <returns>The primary key value of the default row for the specified tool.</returns>
        ''' <remarks>This method may fail if called on a <paramref name="ToolId" /> which appears more than once
        ''' in the AssetDimensionTool table.</remarks>
        Public Shared Function GetDefaultDimension(ByVal db As Database, ByVal ToolId As Integer) As Integer
            Return db.ExecuteScalar("select AssetDimensionId from assetdimensiontool where ToolId=" & db.Number(ToolId))
        End Function

    End Class

    ''' <summary>
    ''' Represents the field-by-field internal implementation of a row in the AssetDimension table in the database.
    ''' </summary>
    ''' <remarks>This class should be regenerated automatically whenever a database change occurs on AssetDimension.</remarks>
    Public MustInherit Class AssetDimensionRowBase
        Private m_DB As Database
        Private m_AssetDimensionId As Integer = Nothing
        Private m_DimensionName As String = Nothing
        Private m_AssetWidth As Integer = Nothing
        Private m_AssetHeight As Integer = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_CreateAdminId As Integer = Nothing
        Private m_ModifyDate As DateTime = Nothing
        Private m_ModifyAdminId As Integer = Nothing

        ''' <summary>
        ''' Gets or sets the value of the AssetDimensionId field in AssetDimension in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AssetDimensionId in AssetDimension in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property AssetDimensionId() As Integer
            Get
                Return m_AssetDimensionId
            End Get
            Set(ByVal Value As Integer)
                m_AssetDimensionId = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the DimensionName field in AssetDimension in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on DimensionName in AssetDimension in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property DimensionName() As String
            Get
                Return m_DimensionName
            End Get
            Set(ByVal Value As String)
                m_DimensionName = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the AssetWidth field in AssetDimension in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AssetWidth in AssetDimension in the
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
        ''' Gets or sets the value of the AssetHeight field in AssetDimension in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AssetHeight in AssetDimension in the
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
        ''' Gets or sets the value of the CreateAdminId field in AssetDimension in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on CreateAdminId in AssetDimension in the
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
        ''' Gets or sets the value of the ModifyAdminId field in AssetDimension in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on ModifyAdminId in AssetDimension in the
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
        ''' Gets the value of the CreateDate field in AssetDimension in the database.
        ''' </summary>
        ''' <value>A <see cref="DateTime" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on CreateDate in AssetDimension in the
        ''' database using SQL Server Management Studio.</remarks>
        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
        End Property

        ''' <summary>
        ''' Gets the value of the ModifyDate field in AssetDimension in the database.
        ''' </summary>
        ''' <value>A <see cref="DateTime" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on ModifyDate in AssetDimension in the
        ''' database using SQL Server Management Studio.</remarks>
        Public ReadOnly Property ModifyDate() As DateTime
            Get
                Return m_ModifyDate
            End Get
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

        ''' <overloads>Initializes a new instance of the <see cref="AssetDimensionRowBase" /> class.</overloads>
        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetDimensionRowBase" /> class using default settings.
        ''' </summary>
        Public Sub New()
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetDimensionRowBase" /> class with the database connection
        ''' specified by <paramref name="DB" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <remarks>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.AssetDimensionRowBase.#ctor(Database,System.Int32)">New(Database, Integer)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</remarks>
        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetDimensionRowBase" /> class representing the row which uses
        ''' <paramref name="AssetDimensionId" /> as its primary key and <paramref name="DB" /> as its database connection.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetDimensionId">The primary key value of the row being referenced.</param>
        ''' <remarks>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.AssetDimensionRowBase.#ctor(Database)">New(Database)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</remarks>
        Public Sub New(ByVal DB As Database, ByVal AssetDimensionId As Integer)
            m_DB = DB
            m_AssetDimensionId = AssetDimensionId
        End Sub 'New

        ''' <overloads>Loads the contents of a row from the database.</overloads>
        ''' <summary>
        ''' Loads the row from the database specified by <see cref="AssetDimensionId" />.
        ''' </summary>
        ''' <remarks>This method calls 
        ''' <see cref="M:DataLayer.AssetDimensionRowBase.Load(System.Data.SqlClient.SqlDataReader)">Load(SqlDataReader)</see>.</remarks>
        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM AssetDimension WHERE AssetDimensionId = " & DB.Number(AssetDimensionId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                AssetDimensionId = Nothing
            End If
            r.Close()
        End Sub

        ''' <summary>
        ''' Loads the contents of the row in the <see cref="SqlDataReader" /> into the 
        ''' <see cref="AssetDimensionRowBase" />.
        ''' </summary>
        ''' <param name="r">A <see cref="SqlDataReader" /> currently set to the row which should be loaded.</param>
        ''' <remarks>It is vital that all of the fields are properly loaded inside this method.</remarks>
        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_AssetDimensionId = Convert.ToInt32(r.Item("AssetDimensionId"))
            m_DimensionName = Convert.ToString(r.Item("DimensionName"))
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
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            m_CreateAdminId = Convert.ToInt32(r.Item("CreateAdminId"))
            m_ModifyDate = Convert.ToDateTime(r.Item("ModifyDate"))
            m_ModifyAdminId = Convert.ToInt32(r.Item("ModifyAdminId"))
        End Sub 'Load

        ''' <summary>
        ''' Inserts the row into AssetDimension using the values contained in the properties set on the 
        ''' <see cref="AssetDimensionRowBase" />.
        ''' </summary>
        ''' <returns>The value of AssetDimensionId for the new row.</returns>
        ''' <remarks>It is vital that all of the fields are properly inserted with their appropriate values 
        ''' inside this method.</remarks>
        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO AssetDimension (" _
             & "DimensionName" _
             & ",AssetWidth" _
             & ",AssetHeight" _
             & ",CreateDate" _
             & ",CreateAdminId" _
             & ",ModifyDate" _
             & ",ModifyAdminId" _
             & ") VALUES (" _
             & m_DB.Quote(DimensionName) _
             & "," & m_DB.Number(AssetWidth) _
             & "," & m_DB.Number(AssetHeight) _
             & "," & m_DB.NullQuote(Now) _
             & "," & CInt(CreateAdminId) _
             & "," & m_DB.NullQuote(Now) _
             & "," & CInt(ModifyAdminId) _
             & ")"

            AssetDimensionId = m_DB.InsertSQL(SQL)

            Return AssetDimensionId
        End Function

        ''' <summary>
        ''' Updates the row in AssetDimension using the values contained in the properties set on the 
        ''' <see cref="AssetDimensionRowBase" />.
        ''' </summary>
        ''' <remarks>It is vital that all of the fields are properly updated with their appropriate values 
        ''' inside this method.</remarks>
        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE AssetDimension SET " _
             & "DimensionName = " & m_DB.Quote(DimensionName) _
             & ",AssetWidth = " & m_DB.Number(AssetWidth) _
             & ",AssetHeight = " & m_DB.Number(AssetHeight) _
             & ",CreateAdminId = " & CInt(CreateAdminId) _
             & ",ModifyDate = " & m_DB.NullQuote(Now) _
             & ",ModifyAdminId = " & CInt(ModifyAdminId) _
             & " WHERE AssetDimensionId = " & m_DB.Quote(AssetDimensionId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        ''' <summary>
        ''' Removes this row from the specified <see cref="Database" />.
        ''' </summary>
        ''' <remarks>This method removes the row from the AssetDimension table.</remarks>
        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM AssetDimension WHERE AssetDimensionId = " & m_DB.Number(AssetDimensionId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    ''' <summary>
    ''' Represents a strongly-typed list of <see cref="AssetDimensionRow">AssetDimensionRows</see> that can be accessed by index.
    ''' </summary>
    ''' <remarks>This class is a wrapper for 
    ''' <see cref="T:Components.GenericCollection`1">GenericCollection(Of AssetDimensionRow)</see>.</remarks>
    Public Class AssetDimensionCollection
        Inherits GenericCollection(Of AssetDimensionRow)
    End Class

End Namespace


