Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	''' <summary>
	''' Represents a row in the SalesforceApiLog table in the database.
	''' </summary>
	''' <remarks>Custom methods should be placed in this class, not in <see cref="SalesforceApiLogRowBase" />.</remarks>
	Public Class SalesforceApiLogRow
		Inherits SalesforceApiLogRowBase
	
		''' <overloads>Initializes a new instance of the <see cref="SalesforceApiLogRow" /> class.</overloads>
		''' <summary>
		''' Initializes a new instance of the <see cref="SalesforceApiLogRow" /> class using default settings.
		''' </summary>
		''' <remarks>This constructor calls 
		''' <see cref="M:DataLayer.SalesforceApiLogRowBase.#ctor">SalesforceApiLogRowBase.New</see>.</remarks>
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		''' <summary>
		''' Initializes a new instance of the <see cref="SalesforceApiLogRow" /> class with the database connection
		''' specified by <paramref name="DB" />.
		''' </summary>
		''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
		''' <remarks><para>If you don't use this constructor or 
		''' <see cref="M:DataLayer.SalesforceApiLogRow.#ctor(Database,System.Int32)">New(Database, Integer)</see>,
		''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
		''' database.</para>
		''' <para>This constructor calls 
		''' <see cref="M:DataLayer.SalesforceApiLogRowBase.#ctor(Database)">SalesforceApiLogRowBase.New(Database)</see>.</para></remarks>
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		''' <summary>
		''' Initializes a new instance of the <see cref="SalesforceApiLogRow" /> class representing the row which uses
		''' <paramref name="LogId" /> as its primary key and <paramref name="DB" /> as its database connection.
		''' </summary>
		''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
		''' <param name="LogId">The primary key value of the row being referenced.</param>
		''' <remarks><para>If you don't use this constructor or 
		''' <see cref="M:DataLayer.SalesforceApiLogRow.#ctor(Database)">New(Database)</see>,
		''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
		''' database.</para>
		''' <para>This constructor calls 
		''' <see cref="M:DataLayer.SalesforceApiLogRowBase.#ctor(Database,System.Int32)">SalesforceApiLogRowBase.New(Database, Integer)</see>.</para></remarks>
		Public Sub New(ByVal DB As Database, LogId as Integer)
			MyBase.New(DB, LogId)
		End Sub 'New
		
		''' <summary>
		''' Gets the row from the specified <see cref="Database" /> with the specified 
		''' <paramref name="LogId" />.
		''' </summary>
		''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
		''' <param name="LogId">The primary key value of the row being retrieved.</param>
		''' <returns>An instance of <see cref="SalesforceApiLogRow" /> loaded with the values from the specified 
		''' row in the database.</returns>
		''' <remarks>This method uses <see cref="M:DataLayer.SalesforceApiLogRowBase.Load">Load</see>.</remarks>
		Public Shared Function GetRow(ByVal DB as Database, ByVal LogId As Integer) As SalesforceApiLogRow
			Dim row as SalesforceApiLogRow 
			
			row = New SalesforceApiLogRow(DB, LogId)
			row.Load()
			
			Return row
		End Function

		''' <summary>
		''' Removes the row from the specified <see cref="Database" /> with the specified 
		''' <paramref name="LogId" />.
		''' </summary>
		''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
		''' <param name="LogId">The primary key value of the row being removed.</param>
		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal LogId As Integer)
			Dim SQL As String
	
			SQL = "DELETE FROM SalesforceApiLog WHERE LogId = " & DB.Number(LogId)
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
		''' Retrieves the SalesforceApiLog table from the specified <see cref="Database" /> ordered based on 
		''' <paramref name="SortBy" /> and <paramref name="SortOrder" />.
		''' </summary>
		''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
		''' <param name="SortBy">The SQL field name to sort the results by.</param>
		''' <param name="SortOrder">The order by which to sort the results (ASC, DESC).  The default value of this
		''' parameter is "ASC".</param>
		''' <returns>A <see cref="DataTable" /> containing the data returned by the query.</returns>
		''' <remarks>If <paramref name="SortBy" /> is not provided, the data is not sorted during the query.</remarks>
		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "SELECT * FROM SalesforceApiLog"
			If Not SortBy = String.Empty Then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " ORDER BY " & SortBy & " " & SortOrder
			End If
			Return DB.GetDataTable(SQL)
		End Function
		''' <summary>
		''' Retrieves the SalesforceApiLog table from the specified <see cref="Database" /> ordered based on 
		''' <paramref name="SortBy" /> and <paramref name="SortOrder" />.
		''' </summary>
		''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
		''' <param name="SortBy">The SQL field name to sort the results by.</param>
		''' <param name="SortOrder">The order by which to sort the results (ASC, DESC).  The default value of this
		''' parameter is "ASC".</param>
		''' <returns>A <see cref="GenericCollection" /> containing the data returned by the query.</returns>
		''' <remarks>If <paramref name="SortBy" /> is not provided, the data is not sorted during the query.</remarks>
		Public Shared Function GetCollection(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As SalesforceApiLogCollection
			Dim SQL As String = "SELECT * FROM SalesforceApiLog"
			Dim colSalesforceApiLogCollection As New SalesforceApiLogCollection

			If Not SortBy = String.Empty Then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " ORDER BY " & SortBy & " " & SortOrder
			End If

			Using reader As SqlDataReader = DB.GetReader(SQL)
				While reader.Read
					Dim dbSalesforceApiLog As New SalesforceApiLogRow(DB)
					dbSalesforceApiLog.Load(reader)
					colSalesforceApiLogCollection.Add(dbSalesforceApiLog)
				End While
			End Using

			Return colSalesforceApiLogCollection
		End Function
		
		
		'Custom Methods
		'Please document with XML comments all custom methods added here.

	End Class

End Namespace
