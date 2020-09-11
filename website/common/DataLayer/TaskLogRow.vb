Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    ''' <summary>
    ''' Represents a row in the TaskLog table in the database.
    ''' </summary>
    ''' <remarks>Custom methods should be placed in this class, not in <see cref="TaskLogRowBase" />.</remarks>
    Public Class TaskLogRow
        Inherits TaskLogRowBase

        ''' <overloads>Initializes a new instance of the <see cref="TaskLogRow" /> class.</overloads>
        ''' <summary>
        ''' Initializes a new instance of the <see cref="TaskLogRow" /> class using default settings.
        ''' </summary>
        ''' <remarks>This constructor calls 
        ''' <see cref="M:DataLayer.TaskLogRowBase.#ctor">TaskLogRowBase.New</see>.</remarks>
        Public Sub New()
            MyBase.New()
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="TaskLogRow" /> class with the database connection
        ''' specified by <paramref name="DB" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <remarks><para>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.TaskLogRow.#ctor(Database,System.Int32)">New(Database, Integer)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</para>
        ''' <para>This constructor calls 
        ''' <see cref="M:DataLayer.TaskLogRowBase.#ctor(Database)">TaskLogRowBase.New(Database)</see>.</para></remarks>
        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="TaskLogRow" /> class representing the row which uses
        ''' <paramref name="LogId" /> as its primary key and <paramref name="DB" /> as its database connection.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="LogId">The primary key value of the row being referenced.</param>
        ''' <remarks><para>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.TaskLogRow.#ctor(Database)">New(Database)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</para>
        ''' <para>This constructor calls 
        ''' <see cref="M:DataLayer.TaskLogRowBase.#ctor(Database,System.Int32)">TaskLogRowBase.New(Database, Integer)</see>.</para></remarks>
        Public Sub New(ByVal DB As Database, ByVal LogId As Integer)
            MyBase.New(DB, LogId)
        End Sub 'New

        ''' <summary>
        ''' Gets the row from the specified <see cref="Database" /> with the specified 
        ''' <paramref name="LogId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="LogId">The primary key value of the row being retrieved.</param>
        ''' <returns>An instance of <see cref="TaskLogRow" /> loaded with the values from the specified 
        ''' row in the database.</returns>
        ''' <remarks>This method uses <see cref="M:DataLayer.TaskLogRowBase.Load">Load</see>.</remarks>
        Public Shared Function GetRow(ByVal DB As Database, ByVal LogId As Integer) As TaskLogRow
            Dim row As TaskLogRow

            row = New TaskLogRow(DB, LogId)
            row.Load()

            Return row
        End Function

        ''' <summary>
        ''' Removes the row from the specified <see cref="Database" /> with the specified 
        ''' <paramref name="LogId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="LogId">The primary key value of the row being removed.</param>
        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal LogId As Integer)
            Dim SQL As String

            SQL = "DELETE FROM TaskLog WHERE LogId = " & DB.Number(LogId)
            DB.ExecuteSQL(SQL)
        End Sub

        ''' <summary>
        ''' Removes this row from the specified <see cref="Database" />.
        ''' </summary>
        ''' <remarks>This method calls <c>RemoveRow(DB, LogId)</c></remarks>
        Public Sub Remove()
            RemoveRow(DB, LogId)
        End Sub

        ''' <summary>
        ''' Retrieves the TaskLog table from the specified <see cref="Database" /> ordered based on 
        ''' <paramref name="SortBy" /> and <paramref name="SortOrder" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="SortBy">The SQL field name to sort the results by.</param>
        ''' <param name="SortOrder">The order by which to sort the results (ASC, DESC).  The default value of this
        ''' parameter is "ASC".</param>
        ''' <returns>A <see cref="DataTable" /> containing the data returned by the query.</returns>
        ''' <remarks>If <paramref name="SortBy" /> is not provided, the data is not sorted during the query.</remarks>
        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from TaskLog"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        'Please document with XML comments all custom methods added here.

    End Class

    ''' <summary>
    ''' Represents the field-by-field internal implementation of a row in the TaskLog table in the database.
    ''' </summary>
    ''' <remarks>This class should be regenerated automatically whenever a database change occurs on TaskLog.</remarks>
    Public MustInherit Class TaskLogRowBase
        Private m_DB As Database
        Private m_LogId As Integer = Nothing
        Private m_TaskName As String = Nothing
        Private m_LogDate As DateTime = Nothing
        Private m_Status As String = Nothing
        Private m_Msg As String = Nothing

        ''' <summary>
        ''' Gets or sets the value of the LogId field in TaskLog in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on LogId in TaskLog in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property LogId() As Integer
            Get
                Return m_LogId
            End Get
            Set(ByVal Value As Integer)
                m_LogId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the TaskName field in TaskLog in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on TaskName in TaskLog in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property TaskName() As String
            Get
                Return m_TaskName
            End Get
            Set(ByVal Value As String)
                m_TaskName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the LogDate field in TaskLog in the database.
        ''' </summary>
        ''' <value>A <see cref="DateTime" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on LogDate in TaskLog in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property LogDate() As DateTime
            Get
                Return m_LogDate
            End Get
            Set(ByVal Value As DateTime)
                m_LogDate = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the Status field in TaskLog in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on Status in TaskLog in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property Status() As String
            Get
                Return m_Status
            End Get
            Set(ByVal Value As String)
                m_Status = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the Msg field in TaskLog in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on Msg in TaskLog in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property Msg() As String
            Get
                Return m_Msg
            End Get
            Set(ByVal Value As String)
                m_Msg = value
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
            Set(ByVal Value As DataBase)
                m_DB = Value
            End Set
        End Property

        ''' <overloads>Initializes a new instance of the <see cref="TaskLogRowBase" /> class.</overloads>
        ''' <summary>
        ''' Initializes a new instance of the <see cref="TaskLogRowBase" /> class using default settings.
        ''' </summary>
        Public Sub New()
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="TaskLogRowBase" /> class with the database connection
        ''' specified by <paramref name="DB" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <remarks>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.TaskLogRowBase.#ctor(Database,System.Int32)">New(Database, Integer)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</remarks>
        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="TaskLogRowBase" /> class representing the row which uses
        ''' <paramref name="LogId" /> as its primary key and <paramref name="DB" /> as its database connection.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="LogId">The primary key value of the row being referenced.</param>
        ''' <remarks>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.TaskLogRowBase.#ctor(Database)">New(Database)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</remarks>
        Public Sub New(ByVal DB As Database, ByVal LogId As Integer)
            m_DB = DB
            m_LogId = LogId
        End Sub 'New

        ''' <overloads>Loads the contents of a row from the database.</overloads>
        ''' <summary>
        ''' Loads the row from the database specified by <see cref="LogId" />.
        ''' </summary>
        ''' <remarks>This method calls 
        ''' <see cref="M:DataLayer.TaskLogRowBase.Load(System.Data.SqlClient.SqlDataReader)">Load(SqlDataReader)</see>.</remarks>
        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM TaskLog WHERE LogId = " & DB.Number(LogId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_LogId = Nothing
            End If
            r.Close()
        End Sub

        ''' <summary>
        ''' Loads the contents of the row in the <see cref="SqlDataReader" /> into the 
        ''' <see cref="TaskLogRowBase" />.
        ''' </summary>
        ''' <param name="r">A <see cref="SqlDataReader" /> currently set to the row which should be loaded.</param>
        ''' <remarks>It is vital that all of the fields are properly loaded inside this method.</remarks>
        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_LogId = Core.GetInt(r.Item("LogId"))
            m_TaskName = Core.GetString(r.Item("TaskName"))
            m_LogDate = Core.GetDate(r.Item("LogDate"))
            m_Status = Core.GetString(r.Item("Status"))
            m_Msg = Core.GetString(r.Item("Msg"))
        End Sub 'Load

        ''' <summary>
        ''' Inserts the row into TaskLog using the values contained in the properties set on the 
        ''' <see cref="TaskLogRowBase" />.
        ''' </summary>
        ''' <returns>The value of LogId for the new row.</returns>
        ''' <remarks>It is vital that all of the fields are properly inserted with their appropriate values 
        ''' inside this method.</remarks>
        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO TaskLog (" _
             & " TaskName" _
             & ",LogDate" _
             & ",Status" _
             & ",Msg" _
             & ") VALUES (" _
             & m_DB.Quote(TaskName) _
             & "," & m_DB.NullQuote(LogDate) _
             & "," & m_DB.Quote(Status) _
             & "," & m_DB.Quote(Msg) _
             & ")"

            LogId = m_DB.InsertSQL(SQL)

            Return LogId
        End Function

        ''' <summary>
        ''' Updates the row in TaskLog using the values contained in the properties set on the 
        ''' <see cref="TaskLogRowBase" />.
        ''' </summary>
        ''' <remarks>It is vital that all of the fields are properly updated with their appropriate values 
        ''' inside this method.</remarks>
        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE TaskLog WITH (ROWLOCK) SET " _
             & " TaskName = " & m_DB.Quote(TaskName) _
             & ",LogDate = " & m_DB.NullQuote(LogDate) _
             & ",Status = " & m_DB.Quote(Status) _
             & ",Msg = " & m_DB.Quote(Msg) _
             & " WHERE LogId = " & m_DB.number(LogId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    ''' <summary>
    ''' Represents a strongly-typed list of <see cref="TaskLogRow">TaskLogRows</see> that can be accessed by index.
    ''' </summary>
    ''' <remarks>This class is a wrapper for 
    ''' <see cref="T:Components.GenericCollection`1">GenericCollection(Of TaskLogRow)</see>.</remarks>
    Public Class TaskLogCollection
        Inherits GenericCollection(Of TaskLogRow)
    End Class


End Namespace


