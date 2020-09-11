Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    ''' <summary>
    ''' Represents a row in the AssetType table in the database.
    ''' </summary>
    ''' <remarks>Custom methods should be placed in this class, not in <see cref="AssetTypeRowBase" />.</remarks>
    Public Class AssetTypeRow
        Inherits AssetTypeRowBase

        ''' <overloads>Initializes a new instance of the <see cref="AssetTypeRow" /> class.</overloads>
        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetTypeRow" /> class using default settings.
        ''' </summary>
        ''' <remarks>This constructor calls 
        ''' <see cref="M:DataLayer.AssetTypeRowBase.#ctor">AssetTypeRowBase.New</see>.</remarks>
        Public Sub New()
            MyBase.New()
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetTypeRow" /> class with the database connection
        ''' specified by <paramref name="DB" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <remarks><para>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.AssetTypeRow.#ctor(Database,System.Int32)">New(Database, Integer)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</para>
        ''' <para>This constructor calls 
        ''' <see cref="M:DataLayer.AssetTypeRowBase.#ctor(Database)">AssetTypeRowBase.New(Database)</see>.</para></remarks>
        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetTypeRow" /> class representing the row which uses
        ''' <paramref name="AssetTypeId" /> as its primary key and <paramref name="DB" /> as its database connection.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetTypeId">The primary key value of the row being referenced.</param>
        ''' <remarks><para>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.AssetTypeRow.#ctor(Database)">New(Database)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</para>
        ''' <para>This constructor calls 
        ''' <see cref="M:DataLayer.AssetTypeRowBase.#ctor(Database,System.Int32)">AssetTypeRowBase.New(Database, Integer)</see>.</para></remarks>
        Public Sub New(ByVal DB As Database, ByVal AssetTypeId As Integer)
            MyBase.New(DB, AssetTypeId)
        End Sub 'New

        ''' <summary>
        ''' Gets the row from the specified <see cref="Database" /> with the specified 
        ''' <paramref name="AssetTypeId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetTypeId">The primary key value of the row being retrieved.</param>
        ''' <returns>An instance of <see cref="AssetTypeRow" /> loaded with the values from the specified 
        ''' row in the database.</returns>
        ''' <remarks>This method uses <see cref="M:DataLayer.AssetTypeRowBase.Load">Load</see>.</remarks>
        Public Shared Function GetRow(ByVal DB As Database, ByVal AssetTypeId As Integer) As AssetTypeRow
            Dim row As AssetTypeRow

            row = New AssetTypeRow(DB, AssetTypeId)
            row.Load()

            Return row
        End Function

        ''' <summary>
        ''' Removes the row from the specified <see cref="Database" /> with the specified 
        ''' <paramref name="AssetTypeId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetTypeId">The primary key value of the row being removed.</param>
        ''' <remarks>This method calls <see cref="Remove" />.</remarks>
        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal AssetTypeId As Integer)
            Dim row As AssetTypeRow

            row = New AssetTypeRow(DB, AssetTypeId)
            row.Remove()
        End Sub

        ''' <summary>
        ''' Retrieves the AssetType table from the specified <see cref="Database" /> ordered based on 
        ''' <paramref name="SortBy" /> and <paramref name="SortOrder" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="SortBy">The SQL field name to sort the results by.</param>
        ''' <param name="SortOrder">The order by which to sort the results (ASC, DESC).  The default value of this
        ''' parameter is "ASC".</param>
        ''' <returns>A <see cref="DataTable" /> containing the data returned by the query.</returns>
        ''' <remarks>If <paramref name="SortBy" /> is not provided, the data is not sorted during the query.</remarks>
        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from AssetType"
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
        ''' Retrieves a <see cref="DataTable" /> containing all asset types ordered by <see cref="Name" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <returns>A <see cref="DataTable" /> of rows from AssetType ordered by <see cref="Name" />.</returns>
        ''' <remarks>This method is deprecated. Use <see cref="GetList" /> with SortBy set to "Name".</remarks>
        Public Shared Function GetAllAssetTypes(ByVal DB As Database) As DataTable
            Dim dt As DataTable = DB.GetDataTable("select * from AssetType order by Name")
            Return dt
        End Function
    End Class

    ''' <summary>
    ''' Represents the field-by-field internal implementation of a row in the AssetType table in the database.
    ''' </summary>
    ''' <remarks>This class should be regenerated automatically whenever a database change occurs on AssetType.</remarks>
    Public MustInherit Class AssetTypeRowBase
        Private m_DB As Database
        Private m_AssetTypeId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_CreateAdminId As Integer = Nothing
        Private m_ModifyDate As DateTime = Nothing
        Private m_ModifyAdminId As Integer = Nothing

        ''' <summary>
        ''' Gets or sets the value of the AssetTypeId field in AssetType in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AssetTypeId in AssetType in the
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
        ''' Gets or sets the value of the Name field in AssetType in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on Name in AssetType in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal Value As String)
                m_Name = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the CreateAdminId field in AssetType in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on CreateAdminId in AssetType in the
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
        ''' Gets or sets the value of the ModifyAdminId field in AssetType in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on ModifyAdminId in AssetType in the
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
        ''' Gets the value of the CreateDate field in AssetType in the database.
        ''' </summary>
        ''' <value>A <see cref="DateTime" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on CreateDate in AssetType in the
        ''' database using SQL Server Management Studio.</remarks>
        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
        End Property

        ''' <summary>
        ''' Gets the value of the ModifyDate field in AssetType in the database.
        ''' </summary>
        ''' <value>A <see cref="DateTime" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on ModifyDate in AssetType in the
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

        ''' <overloads>Initializes a new instance of the <see cref="AssetTypeRowBase" /> class.</overloads>
        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetTypeRowBase" /> class using default settings.
        ''' </summary>
        Public Sub New()
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetTypeRowBase" /> class with the database connection
        ''' specified by <paramref name="DB" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <remarks>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.AssetTypeRowBase.#ctor(Database,System.Int32)">New(Database, Integer)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</remarks>
        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetTypeRowBase" /> class representing the row which uses
        ''' <paramref name="AssetTypeId" /> as its primary key and <paramref name="DB" /> as its database connection.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetTypeId">The primary key value of the row being referenced.</param>
        ''' <remarks>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.AssetTypeRowBase.#ctor(Database)">New(Database)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</remarks>
        Public Sub New(ByVal DB As Database, ByVal AssetTypeId As Integer)
            m_DB = DB
            m_AssetTypeId = AssetTypeId
        End Sub 'New

        ''' <overloads>Loads the contents of a row from the database.</overloads>
        ''' <summary>
        ''' Loads the row from the database specified by <see cref="AssetTypeId" />.
        ''' </summary>
        ''' <remarks>This method calls 
        ''' <see cref="M:DataLayer.AssetTypeRowBase.Load(System.Data.SqlClient.SqlDataReader)">Load(SqlDataReader)</see>.</remarks>
        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM AssetType WHERE AssetTypeId = " & DB.Number(AssetTypeId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                AssetTypeId = Nothing
            End If
            r.Close()
        End Sub


        ''' <summary>
        ''' Loads the contents of the row in the <see cref="SqlDataReader" /> into the 
        ''' <see cref="AssetTypeRowBase" />.
        ''' </summary>
        ''' <param name="r">A <see cref="SqlDataReader" /> currently set to the row which should be loaded.</param>
        ''' <remarks>It is vital that all of the fields are properly loaded inside this method.</remarks>
        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_AssetTypeId = Convert.ToInt32(r.Item("AssetTypeId"))
            m_Name = Convert.ToString(r.Item("Name"))
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            m_CreateAdminId = Convert.ToInt32(r.Item("CreateAdminId"))
            m_ModifyDate = Convert.ToDateTime(r.Item("ModifyDate"))
            m_ModifyAdminId = Convert.ToInt32(r.Item("ModifyAdminId"))
        End Sub 'Load

        ''' <summary>
        ''' Inserts the row into AssetType using the values contained in the properties set on the 
        ''' <see cref="AssetTypeRowBase" />.
        ''' </summary>
        ''' <returns>The value of AssetTypeId for the new row.</returns>
        ''' <remarks>It is vital that all of the fields are properly inserted with their appropriate values 
        ''' inside this method.</remarks>
        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO AssetType (" _
             & " Name" _
             & ",CreateDate" _
             & ",CreateAdminId" _
             & ",ModifyDate" _
             & ",ModifyAdminId" _
             & ") VALUES (" _
             & m_DB.Quote(Name) _
             & "," & m_DB.NullQuote(Now) _
             & "," & CInt(CreateAdminId) _
             & "," & m_DB.NullQuote(Now) _
             & "," & CInt(ModifyAdminId) _
             & ")"

            AssetTypeId = m_DB.InsertSQL(SQL)

            Return AssetTypeId
        End Function

        ''' <summary>
        ''' Updates the row in AssetType using the values contained in the properties set on the 
        ''' <see cref="AssetTypeRowBase" />.
        ''' </summary>
        ''' <remarks>It is vital that all of the fields are properly updated with their appropriate values 
        ''' inside this method.</remarks>
        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE AssetType SET " _
             & " Name = " & m_DB.Quote(Name) _
             & ",CreateAdminId = " & CInt(CreateAdminId) _
             & ",ModifyDate = " & m_DB.NullQuote(Now) _
             & ",ModifyAdminId = " & CInt(ModifyAdminId) _
             & " WHERE AssetTypeId = " & m_DB.Quote(AssetTypeId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        ''' <summary>
        ''' Removes this row from the specified <see cref="Database" />.
        ''' </summary>
        ''' <remarks>This method removes the row from the AssetType table.</remarks>
        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM AssetType WHERE AssetTypeId = " & m_DB.Number(AssetTypeId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    ''' <summary>
    ''' Represents a strongly-typed list of <see cref="AssetTypeRow">AssetTypeRows</see> that can be accessed by index.
    ''' </summary>
    ''' <remarks>This class is a wrapper for 
    ''' <see cref="T:Components.GenericCollection`1">GenericCollection(Of AssetTypeRow)</see>.</remarks>
    Public Class AssetTypeCollection
        Inherits GenericCollection(Of AssetTypeRow)
    End Class

End Namespace



