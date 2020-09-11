Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    ''' <summary>
    ''' Represents a row in the WorkflowTool table in the database.
    ''' </summary>
    ''' <remarks>Custom methods should be placed in this class, not in <see cref="WorkflowToolRowBase" />.</remarks>
    Public Class WorkflowToolRow
        Inherits WorkflowToolRowBase

        ''' <overloads>Initializes a new instance of the <see cref="WorkflowToolRow" /> class.</overloads>
        ''' <summary>
        ''' Initializes a new instance of the <see cref="WorkflowToolRow" /> class using default settings.
        ''' </summary>
        ''' <remarks>This constructor calls 
        ''' <see cref="M:DataLayer.WorkflowToolRowBase.#ctor">WorkflowToolRowBase.New</see>.</remarks>
        Public Sub New()
            MyBase.New()
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="WorkflowToolRow" /> class with the database connection
        ''' specified by <paramref name="DB" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <remarks><para>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.WorkflowToolRow.#ctor(Database,System.Int32)">New(Database, Integer)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</para>
        ''' <para>This constructor calls 
        ''' <see cref="M:DataLayer.WorkflowToolRowBase.#ctor(Database)">WorkflowToolRowBase.New(Database)</see>.</para></remarks>
        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="WorkflowToolRow" /> class representing the row which uses
        ''' <paramref name="ToolId" /> as its primary key and <paramref name="DB" /> as its database connection.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="ToolId">The primary key value of the row being referenced.</param>
        ''' <remarks><para>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.WorkflowToolRow.#ctor(Database)">New(Database)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</para>
        ''' <para>This constructor calls 
        ''' <see cref="M:DataLayer.WorkflowToolRowBase.#ctor(Database,System.Int32)">WorkflowToolRowBase.New(Database, Integer)</see>.</para></remarks>
        Public Sub New(ByVal DB As Database, ByVal ToolId As Integer)
            MyBase.New(DB, ToolId)
        End Sub 'New

        ''' <summary>
        ''' Gets the row from the specified <see cref="Database" /> with the specified 
        ''' <paramref name="ToolId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="ToolId">The primary key value of the row being retrieved.</param>
        ''' <returns>An instance of <see cref="WorkflowToolRow" /> loaded with the values from the specified 
        ''' row in the database.</returns>
        ''' <remarks>This method uses <see cref="M:DataLayer.WorkflowToolRowBase.Load">Load</see>.</remarks>
        Public Shared Function GetRow(ByVal DB As Database, ByVal ToolId As Integer) As WorkflowToolRow
            Dim row As WorkflowToolRow

            row = New WorkflowToolRow(DB, ToolId)
            row.Load()

            Return row
        End Function

        ''' <summary>
        ''' Removes the row from the specified <see cref="Database" /> with the specified 
        ''' <paramref name="ToolId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="ToolId">The primary key value of the row being removed.</param>
        ''' <remarks>This method calls <see cref="Remove" />.</remarks>
        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ToolId As Integer)
            Dim row As WorkflowToolRow

            row = New WorkflowToolRow(DB, ToolId)
            row.Remove()
        End Sub

        ''' <summary>
        ''' Retrieves the WorkflowTool table from the specified <see cref="Database" /> ordered based on 
        ''' <paramref name="SortBy" /> and <paramref name="SortOrder" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="SortBy">The SQL field name to sort the results by.</param>
        ''' <param name="SortOrder">The order by which to sort the results (ASC, DESC).  The default value of this
        ''' parameter is "ASC".</param>
        ''' <returns>A <see cref="DataTable" /> containing the data returned by the query.</returns>
        ''' <remarks>If <paramref name="SortBy" /> is not provided, the data is not sorted during the query.</remarks>
        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from WorkflowTool"
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
        ''' Retrieves the <see cref="WorkflowToolRow" /> for the specified <paramref name="Code" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="Code">The code of the row to retrieve.</param>
        ''' <returns>A <see cref="WorkflowToolRow" /> containing the data associated with the specified 
        ''' <paramref name="Code" />.</returns>
        ''' <remarks>Use this method to retrieve a particular tool based on its code.</remarks>
        Public Shared Function GetRowByCode(ByVal DB As Database, ByVal Code As String) As WorkflowToolRow
            Dim SQL As String = "select * from WorkflowTool where Code = " & DB.Quote(Code)
            Dim r As SqlDataReader
            Dim row As WorkflowToolRow = New WorkflowToolRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

        ''' <summary>
        ''' Retrieves all workflow tools which have specified an <see cref="AlertType" />, ordered based on 
        ''' <paramref name="SortBy" /> and <paramref name="SortOrder" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="SortBy">The SQL field name to sort the results by.</param>
        ''' <param name="SortOrder">The order by which to sort the results (ASC, DESC).  The default value of this
        ''' parameter is "ASC".</param>
        ''' <returns>A <see cref="DataTable" /> of all rows in WorkflowTool which match "AlertType IS NOT NULL".</returns>
        ''' <remarks>If <paramref name="SortBy" /> is not provided, the data is not sorted during the query.</remarks>
        Public Shared Function GetAlertsList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from WorkflowTool where AlertType is not null "
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        ''' <summary>
        ''' Retrieves the <see cref="WorkflowToolRow" /> associated with the specified 
        ''' <paramref name="ModuleId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="ModuleId">The value of <see cref="ContentToolModuleRow.ModuleId" /> to look up the Tool for.</param>
        ''' <returns>The <see cref="WorkflowToolRow" /> associated with the specified module.</returns>
        ''' <remarks>This method is equivalent to <c>WorkflowToolRow.GetRow(DB, ContentToolModuleRow.GetRow(DB, ModuleId).ToolId)</c>.</remarks>
        Public Shared Function GetRowByModuleId(ByVal DB As Database, ByVal ModuleId As Integer) As WorkflowToolRow
            Dim row As New WorkflowToolRow(DB)
            Dim sql As String = "select t.* from WorkflowTool t inner join ContentToolModule m on t.ToolId=m.ToolId where m.ModuleId=" & DB.Number(ModuleId)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                row.Load(sdr)
            End If
            sdr.Close()
            Return row
        End Function

        ''' <summary>
        ''' Retrieves all tools which have specified a value for <see cref="ListControlUrl" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <returns>A <see cref="DataTable" /> of all rows in WorkflowTool which match "ListControlUrl IS NOT NULL".</returns>
        ''' <remarks>This method is used to bind the list of tools on the lists edit page.</remarks>
        Public Shared Function GetListableTools(ByVal DB As Database) As DataTable
            Dim sql As String = "select * from WorkflowTool where ListAdminUrl is not null"
            Return DB.GetDataTable(sql)
        End Function
    End Class

    ''' <summary>
    ''' Represents the field-by-field internal implementation of a row in the WorkflowTool table in the database.
    ''' </summary>
    ''' <remarks>This class should be regenerated automatically whenever a database change occurs on WorkflowTool.</remarks>
    Public MustInherit Class WorkflowToolRowBase
        Private m_DB As Database
        Private m_ToolId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_Code As String = Nothing
        Private m_AdminUrl As String = Nothing
        Private m_AlertType As String = Nothing
        Private m_Version As String = Nothing
        Private m_ListControlUrl As String = Nothing
        Private m_DisplayControlUrl As String = Nothing

        ''' <summary>
        ''' Gets or sets the value of the ToolId field in WorkflowTool in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on ToolId in WorkflowTool in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property ToolId() As Integer
            Get
                Return m_ToolId
            End Get
            Set(ByVal Value As Integer)
                m_ToolId = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the Name field in WorkflowTool in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on Name in WorkflowTool in the
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
        ''' Gets or sets the value of the Code field in WorkflowTool in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on Code in WorkflowTool in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property Code() As String
            Get
                Return m_Code
            End Get
            Set(ByVal Value As String)
                m_Code = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the AdminUrl field in WorkflowTool in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AdminUrl in WorkflowTool in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property AdminUrl() As String
            Get
                Return m_AdminUrl
            End Get
            Set(ByVal Value As String)
                m_AdminUrl = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the AlertType field in WorkflowTool in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AlertType in WorkflowTool in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property AlertType() As String
            Get
                Return m_AlertType
            End Get
            Set(ByVal value As String)
                m_AlertType = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the Version field in WorkflowTool in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on Version in WorkflowTool in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property Version() As String
            Get
                Return m_Version
            End Get
            Set(ByVal value As String)
                m_Version = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the ListControlUrl field in WorkflowTool in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on ListControlUrl in WorkflowTool in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property ListControlUrl() As String
            Get
                Return m_ListControlUrl
            End Get
            Set(ByVal value As String)
                m_ListControlUrl = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the DisplayControlUrl field in WorkflowTool in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on DisplayControlUrl in WorkflowTool in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property DisplayControlUrl() As String
            Get
                Return m_DisplayControlUrl
            End Get
            Set(ByVal value As String)
                m_DisplayControlUrl = value
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

        ''' <overloads>Initializes a new instance of the <see cref="WorkflowToolRowBase" /> class.</overloads>
        ''' <summary>
        ''' Initializes a new instance of the <see cref="WorkflowToolRowBase" /> class using default settings.
        ''' </summary>
        Public Sub New()
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="WorkflowToolRowBase" /> class with the database connection
        ''' specified by <paramref name="DB" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <remarks>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.WorkflowToolRowBase.#ctor(Database,System.Int32)">New(Database, Integer)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</remarks>
        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="WorkflowToolRowBase" /> class representing the row which uses
        ''' <paramref name="ToolId" /> as its primary key and <paramref name="DB" /> as its database connection.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="ToolId">The primary key value of the row being referenced.</param>
        ''' <remarks>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.WorkflowToolRowBase.#ctor(Database)">New(Database)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</remarks>
        Public Sub New(ByVal DB As Database, ByVal ToolId As Integer)
            m_DB = DB
            m_ToolId = ToolId
        End Sub 'New

        ''' <overloads>Loads the contents of a row from the database.</overloads>
        ''' <summary>
        ''' Loads the row from the database specified by <see cref="ToolId" />.
        ''' </summary>
        ''' <remarks>This method calls 
        ''' <see cref="M:DataLayer.WorkflowToolRowBase.Load(System.Data.SqlClient.SqlDataReader)">Load(SqlDataReader)</see>.</remarks>
        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM WorkflowTool WHERE ToolId = " & DB.Number(ToolId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                ToolId = Nothing
            End If
            r.Close()
        End Sub

        ''' <summary>
        ''' Loads the contents of the row in the <see cref="SqlDataReader" /> into the 
        ''' <see cref="WorkflowToolRowBase" />.
        ''' </summary>
        ''' <param name="r">A <see cref="SqlDataReader" /> currently set to the row which should be loaded.</param>
        ''' <remarks>It is vital that all of the fields are properly loaded inside this method.</remarks>
        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_ToolId = Core.GetInt(r.Item("ToolId"))
            m_Name = Core.GetString(r.Item("Name"))
            m_Code = Core.GetString(r.Item("Code"))
            m_AdminUrl = Core.GetString(r.Item("AdminUrl"))
            m_AlertType = Core.GetString(r.Item("AlertType"))
            m_Version = Core.GetString(r.Item("Version"))
            m_ListControlUrl = Core.GetString(r.Item("ListControlUrl"))
            m_DisplayControlUrl = Core.GetString(r.Item("DisplayControlUrl"))
        End Sub 'Load

        ''' <summary>
        ''' Inserts the row into WorkflowTool using the values contained in the properties set on the 
        ''' <see cref="WorkflowToolRowBase" />.
        ''' </summary>
        ''' <returns>The value of ToolId for the new row.</returns>
        ''' <remarks>It is vital that all of the fields are properly inserted with their appropriate values 
        ''' inside this method.</remarks>
        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO WorkflowTool (" _
             & " Name" _
             & ",Code" _
             & ",AdminUrl" _
             & ",AlertType" _
             & ",Version" _
             & ",ListControlUrl" _
             & ",DisplayControlUrl" _
             & ") VALUES (" _
             & m_DB.Quote(Name) _
             & "," & m_DB.Quote(Code) _
             & "," & m_DB.Quote(AdminUrl) _
             & "," & m_DB.Quote(AlertType) _
             & "," & m_DB.Quote(Version) _
             & "," & m_DB.Quote(ListControlUrl) _
             & "," & m_DB.Quote(DisplayControlUrl) _
             & ")"

            ToolId = m_DB.InsertSQL(SQL)

            Return ToolId
        End Function

        ''' <summary>
        ''' Updates the row in WorkflowTool using the values contained in the properties set on the 
        ''' <see cref="WorkflowToolRowBase" />.
        ''' </summary>
        ''' <remarks>It is vital that all of the fields are properly updated with their appropriate values 
        ''' inside this method.</remarks>
        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE WorkflowTool SET " _
             & " Name = " & m_DB.Quote(Name) _
             & ",Code = " & m_DB.Quote(Code) _
             & ",AdminUrl = " & m_DB.Quote(AdminUrl) _
             & ",AlertType = " & m_DB.Quote(AlertType) _
             & ",Version = " & m_DB.Quote(Version) _
             & ",ListControlUrl = " & m_DB.Quote(ListControlUrl) _
             & ",DisplayControlUrl = " & m_DB.Quote(DisplayControlUrl) _
             & " WHERE ToolId = " & m_DB.Quote(ToolId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        ''' <summary>
        ''' Removes this row from the specified <see cref="Database" />.
        ''' </summary>
        ''' <remarks>This method removes the row from the WorkflowStepMessage table.</remarks>
        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM WorkflowTool WHERE ToolId = " & m_DB.Number(ToolId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    ''' <summary>
    ''' Represents a strongly-typed list of <see cref="WorkflowToolRow">WorkflowToolRows</see> that can be accessed by index.
    ''' </summary>
    ''' <remarks>This class is a wrapper for 
    ''' <see cref="T:Components.GenericCollection`1">GenericCollection(Of WorkflowToolRow)</see>.</remarks>
    Public Class WorkflowToolCollection
        Inherits GenericCollection(Of WorkflowToolRow)
    End Class

End Namespace


