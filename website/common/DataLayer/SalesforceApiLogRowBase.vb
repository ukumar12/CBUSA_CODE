Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports System.Collections.Generic
Imports Components
	
Namespace DataLayer
	
	''' <summary>
	''' Represents the field-by-field internal implementation of a row in the SalesforceApiLog table in the database.
	''' </summary>
	''' <remarks>This class should be regenerated automatically whenever a database change occurs on SalesforceApiLog.</remarks>
	Public MustInherit Class SalesforceApiLogRowBase
		Private m_DB as Database
		Private m_LogId As Integer = nothing
		Private m_WebMethod As String = nothing
		Private m_UserHostAddress As String = nothing
		Private m_RequestTimestamp As DateTime = nothing
		Private m_RequestMessage As String = nothing
		Private m_ResponseTimestamp As DateTime = nothing
		Private m_ResponseMessage As String = nothing
		Private m_ErrorDescription As String = nothing
        Private m_Service As String = Nothing
	
		''' <summary>
		''' Gets or sets the value of the LogId field in SalesforceApiLog in the database.
		''' </summary>
		''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
		''' <remarks>For more information about this field, view the entry on LogId in SalesforceApiLog in the
		''' database using SQL Server Management Studio.</remarks>
		Public Property LogId As Integer
			Get
				Return m_LogId
			End Get
			Set(ByVal Value As Integer)
				m_LogId = value
			End Set
		End Property
	
		''' <summary>
		''' Gets or sets the value of the WebMethod field in SalesforceApiLog in the database.
		''' </summary>
		''' <value>A <see cref="String" /> which contains the value of the field.</value>
		''' <remarks>For more information about this field, view the entry on WebMethod in SalesforceApiLog in the
		''' database using SQL Server Management Studio.</remarks>
		Public Property WebMethod As String
			Get
				Return m_WebMethod
			End Get
			Set(ByVal Value As String)
				m_WebMethod = value
			End Set
		End Property
	
		''' <summary>
		''' Gets or sets the value of the UserHostAddress field in SalesforceApiLog in the database.
		''' </summary>
		''' <value>A <see cref="String" /> which contains the value of the field.</value>
		''' <remarks>For more information about this field, view the entry on UserHostAddress in SalesforceApiLog in the
		''' database using SQL Server Management Studio.</remarks>
		Public Property UserHostAddress As String
			Get
				Return m_UserHostAddress
			End Get
			Set(ByVal Value As String)
				m_UserHostAddress = value
			End Set
		End Property
	
		''' <summary>
		''' Gets or sets the value of the RequestTimestamp field in SalesforceApiLog in the database.
		''' </summary>
		''' <value>A <see cref="DateTime" /> which contains the value of the field.</value>
		''' <remarks>For more information about this field, view the entry on RequestTimestamp in SalesforceApiLog in the
		''' database using SQL Server Management Studio.</remarks>
		Public Property RequestTimestamp As DateTime
			Get
				Return m_RequestTimestamp
			End Get
			Set(ByVal Value As DateTime)
				m_RequestTimestamp = value
			End Set
		End Property
	
		''' <summary>
		''' Gets or sets the value of the RequestMessage field in SalesforceApiLog in the database.
		''' </summary>
		''' <value>A <see cref="String" /> which contains the value of the field.</value>
		''' <remarks>For more information about this field, view the entry on RequestMessage in SalesforceApiLog in the
		''' database using SQL Server Management Studio.</remarks>
		Public Property RequestMessage As String
			Get
				Return m_RequestMessage
			End Get
			Set(ByVal Value As String)
				m_RequestMessage = value
			End Set
		End Property
	
		''' <summary>
		''' Gets or sets the value of the ResponseTimestamp field in SalesforceApiLog in the database.
		''' </summary>
		''' <value>A <see cref="DateTime" /> which contains the value of the field.</value>
		''' <remarks>For more information about this field, view the entry on ResponseTimestamp in SalesforceApiLog in the
		''' database using SQL Server Management Studio.</remarks>
		Public Property ResponseTimestamp As DateTime
			Get
				Return m_ResponseTimestamp
			End Get
			Set(ByVal Value As DateTime)
				m_ResponseTimestamp = value
			End Set
		End Property
	
		''' <summary>
		''' Gets or sets the value of the ResponseMessage field in SalesforceApiLog in the database.
		''' </summary>
		''' <value>A <see cref="String" /> which contains the value of the field.</value>
		''' <remarks>For more information about this field, view the entry on ResponseMessage in SalesforceApiLog in the
		''' database using SQL Server Management Studio.</remarks>
		Public Property ResponseMessage As String
			Get
				Return m_ResponseMessage
			End Get
			Set(ByVal Value As String)
				m_ResponseMessage = value
			End Set
		End Property
	
		
	
		''' <summary>
		''' Gets or sets the value of the ErrorDescription field in SalesforceApiLog in the database.
		''' </summary>
		''' <value>A <see cref="String" /> which contains the value of the field.</value>
		''' <remarks>For more information about this field, view the entry on ErrorDescription in SalesforceApiLog in the
		''' database using SQL Server Management Studio.</remarks>
		Public Property ErrorDescription As String
			Get
				Return m_ErrorDescription
			End Get
			Set(ByVal Value As String)
				m_ErrorDescription = value
			End Set
		End Property
	
		''' <summary>
		''' Gets or sets the value of the Url field in SalesforceApiLog in the database.
		''' </summary>
		''' <value>A <see cref="String" /> which contains the value of the field.</value>
		''' <remarks>For more information about this field, view the entry on Url in SalesforceApiLog in the
		''' database using SQL Server Management Studio.</remarks>
        Public Property Service As String
            Get
                Return m_Service
            End Get
            Set(ByVal Value As String)
                m_Service = Value
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
	
		''' <overloads>Initializes a new instance of the <see cref="SalesforceApiLogRowBase" /> class.</overloads>
		''' <summary>
		''' Initializes a new instance of the <see cref="SalesforceApiLogRowBase" /> class using default settings.
		''' </summary>
		Public Sub New()
		End Sub 'New
	
		''' <summary>
		''' Initializes a new instance of the <see cref="SalesforceApiLogRowBase" /> class with the database connection
		''' specified by <paramref name="DB" />.
		''' </summary>
		''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
		''' <remarks>If you don't use this constructor or 
		''' <see cref="M:DataLayer.SalesforceApiLogRowBase.#ctor(Database,System.Int32)">New(Database, Integer)</see>,
		''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
		''' database.</remarks>
		Public Sub New(ByVal DB As Database)
			m_DB = DB
		End Sub 'New
	
		''' <summary>
		''' Initializes a new instance of the <see cref="SalesforceApiLogRowBase" /> class representing the row which uses
		''' <paramref name="LogId" /> as its primary key and <paramref name="DB" /> as its database connection.
		''' </summary>
		''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
		''' <param name="LogId">The primary key value of the row being referenced.</param>
		''' <remarks>If you don't use this constructor or 
		''' <see cref="M:DataLayer.SalesforceApiLogRowBase.#ctor(Database)">New(Database)</see>,
		''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
		''' database.</remarks>
		Public Sub New(ByVal DB As Database, LogId as Integer)
			m_DB = DB
			m_LogId = LogId
		End Sub 'New
		
		''' <overloads>Loads the contents of a row from the database.</overloads>
		''' <summary>
		''' Loads the row from the database specified by <see cref="LogId" />.
		''' </summary>
		''' <remarks>This method calls 
		''' <see cref="M:DataLayer.SalesforceApiLogRowBase.Load(System.Data.SqlClient.SqlDataReader)">Load(SqlDataReader)</see>.</remarks>
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM SalesforceApiLog WHERE LogId = " & DB.Number(LogId)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			Else
				m_LogId = nothing
			End If
			r.Close()
		End Sub
	
		''' <summary>
		''' Loads the contents of the row in the <see cref="SqlDataReader" /> into the 
		''' <see cref="SalesforceApiLogRowBase" />.
		''' </summary>
		''' <param name="r">A <see cref="SqlDataReader" /> currently set to the row which should be loaded.</param>
		''' <remarks>It is vital that all of the fields are properly loaded inside this method.</remarks>
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_LogId = Core.GetInt(r.Item("LogId"))
			m_WebMethod = Core.GetString(r.Item("WebMethod"))
			m_UserHostAddress = Core.GetString(r.Item("UserHostAddress"))
            m_RequestTimestamp = Core.GetDate(r.Item("RequestTimestamp"))
			m_RequestMessage = Core.GetString(r.Item("RequestMessage"))
            m_ResponseTimestamp = Core.GetDate(r.Item("ResponseTimestamp"))
			m_ResponseMessage = Core.GetString(r.Item("ResponseMessage"))
			m_ErrorDescription = Core.GetString(r.Item("ErrorDescription"))
            m_Service = Core.GetString(r.Item("Service"))
		End Sub 'Load
    
		''' <summary>
		''' Inserts the row into SalesforceApiLog using the values contained in the properties set on the 
		''' <see cref="SalesforceApiLogRowBase" />.
		''' </summary>
		''' <returns>The value of LogId for the new row.</returns>
		''' <remarks>It is vital that all of the fields are properly inserted with their appropriate values 
		''' inside this method.</remarks>
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
            SQL = " INSERT INTO SalesforceApiLog (" _
             & " WebMethod" _
             & ",UserHostAddress" _
             & ",RequestTimestamp" _
             & ",RequestMessage" _
             & ",ResponseTimestamp" _
             & ",ResponseMessage" _
             & ",ErrorDescription" _
             & ",Service" _
             & ") VALUES (" _
             & m_DB.NQuote(WebMethod) _
             & "," & m_DB.NQuote(UserHostAddress) _
             & "," & m_DB.NullQuote(RequestTimestamp) _
             & "," & m_DB.NQuote(RequestMessage) _
             & "," & m_DB.NullQuote(ResponseTimestamp) _
             & "," & m_DB.NQuote(ResponseMessage) _
             & "," & m_DB.NQuote(ErrorDescription) _
             & "," & m_DB.NQuote(Service) _
             & ")"

			LogId = m_DB.InsertSQL(SQL)
			
			Return LogId
		End Function
	
		''' <summary>
		''' Updates the row in SalesforceApiLog using the values contained in the properties set on the 
		''' <see cref="SalesforceApiLogRowBase" />.
		''' </summary>
		''' <remarks>It is vital that all of the fields are properly updated with their appropriate values 
		''' inside this method.</remarks>
		Public Overridable Sub Update()
			Dim SQL As String
	
            SQL = " UPDATE SalesforceApiLog WITH (ROWLOCK) SET " _
             & " WebMethod = " & m_DB.NQuote(WebMethod) _
             & ",UserHostAddress = " & m_DB.NQuote(UserHostAddress) _
             & ",RequestTimestamp = " & m_DB.NullQuote(RequestTimestamp) _
             & ",RequestMessage = " & m_DB.NQuote(RequestMessage) _
             & ",ResponseTimestamp = " & m_DB.NullQuote(ResponseTimestamp) _
             & ",ResponseMessage = " & m_DB.NQuote(ResponseMessage) _
             & ",ErrorDescription = " & m_DB.NQuote(ErrorDescription) _
             & ",Service = " & m_DB.NQuote(Service) _
             & " WHERE LogId = " & m_DB.Number(LogId)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	End Class
	
	''' <summary>
	''' Represents a strongly-typed list of <see cref="SalesforceApiLogRow">SalesforceApiLogRows</see> that can be accessed by index.
	''' </summary>
	''' <remarks>This class is a wrapper for 
	''' <see cref="T:Components.GenericCollection`1">GenericCollection(Of SalesforceApiLogRow)</see>.</remarks>
	Public Class SalesforceApiLogCollection
		Inherits List(Of SalesforceApiLogRow)
	End Class

End Namespace
