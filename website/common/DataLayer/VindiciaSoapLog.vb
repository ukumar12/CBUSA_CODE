Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class VindiciaSoapLogRow
		Inherits VindiciaSoapLogRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, VindiciaSoapLogId as Integer)
			MyBase.New(DB, VindiciaSoapLogId)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal VindiciaSoapLogId As Integer) As VindiciaSoapLogRow
			Dim row as VindiciaSoapLogRow 
			
			row = New VindiciaSoapLogRow(DB, VindiciaSoapLogId)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal VindiciaSoapLogId As Integer)
			Dim row as VindiciaSoapLogRow 
			
			row = New VindiciaSoapLogRow(DB, VindiciaSoapLogId)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from VindiciaSoapLog"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods

	End Class
	
	Public MustInherit Class VindiciaSoapLogRowBase
		Private m_DB as Database
		Private m_VindiciaSoapLogId As Integer = nothing
		Private m_SoapId As String = nothing
		Private m_ReturnCode As Integer = nothing
		Private m_ReturnString As String = nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_BuilderGUID As String = Nothing
        Private m_Description As String = Nothing
        Private m_SoapMethod As String = Nothing
	
		Public Property VindiciaSoapLogId As Integer
			Get
				Return m_VindiciaSoapLogId
			End Get
			Set(ByVal Value As Integer)
				m_VindiciaSoapLogId = value
			End Set
		End Property
	
		Public Property SoapId As String
			Get
				Return m_SoapId
			End Get
			Set(ByVal Value As String)
				m_SoapId = value
			End Set
		End Property
	
		Public Property ReturnCode As Integer
			Get
				Return m_ReturnCode
			End Get
			Set(ByVal Value As Integer)
				m_ReturnCode = value
			End Set
		End Property
	
		Public Property ReturnString As String
			Get
				Return m_ReturnString
			End Get
			Set(ByVal Value As String)
				m_ReturnString = value
			End Set
        End Property

        Public Property BuilderGUID() As String
            Get
                Return m_BuilderGUID
            End Get
            Set(ByVal value As String)
                m_BuilderGUID = value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return m_Description
            End Get
            Set(ByVal value As String)
                m_Description = value
            End Set
        End Property

        Public Property SoapMethod() As String
            Get
                Return m_SoapMethod
            End Get
            Set(ByVal value As String)
                m_SoapMethod = value
            End Set
        End Property

        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
        End Property
	
	
		Public Property DB() As Database
			Get
				DB = m_DB
			End Get
			Set(ByVal Value As DataBase)
				m_DB = Value
			End Set
		End Property
	
		Public Sub New()
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			m_DB = DB
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, VindiciaSoapLogId as Integer)
			m_DB = DB
			m_VindiciaSoapLogId = VindiciaSoapLogId
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM VindiciaSoapLog WHERE VindiciaSoapLogId = " & DB.Number(VindiciaSoapLogId)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_VindiciaSoapLogId = Core.GetInt(r.Item("VindiciaSoapLogId"))
			m_SoapId = Core.GetString(r.Item("SoapId"))
			m_ReturnCode = Core.GetInt(r.Item("ReturnCode"))
            m_ReturnString = Core.GetString(r.Item("ReturnString"))
            m_CreateDate = Core.GetDate(r.Item("CreateDate"))
            m_BuilderGUID = Core.GetString(r.Item("BuilderGUID"))
            m_Description = Core.GetString(r.Item("Description"))
            m_SoapMethod = Core.GetString(r.Item("SoapMethod"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
            SQL = " INSERT INTO VindiciaSoapLog (" _
             & " SoapId" _
             & ",ReturnCode" _
             & ",ReturnString" _
             & ",BuilderGuid" _
             & ",CreateDate" _
             & ",Description" _
             & ",SoapMethod" _
             & ") VALUES (" _
             & m_DB.Quote(SoapId) _
             & "," & m_DB.Number(ReturnCode) _
             & "," & m_DB.Quote(ReturnString) _
             & "," & m_DB.Quote(BuilderGUID) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.Quote(Description) _
             & "," & m_DB.Quote(SoapMethod) _
             & ")"

			VindiciaSoapLogId = m_DB.InsertSQL(SQL)
			
			Return VindiciaSoapLogId
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
            SQL = " UPDATE VindiciaSoapLog SET " _
             & " SoapId = " & m_DB.Quote(SoapId) _
             & ",ReturnCode = " & m_DB.Number(ReturnCode) _
             & ",ReturnString = " & m_DB.Quote(ReturnString) _
             & ",BuilderGUID = " & m_DB.Quote(BuilderGUID) _
             & ",Description = " & m_DB.Quote(Description) _
             & ",SoapMethod = " & m_DB.Quote(SoapMethod) _
             & " WHERE VindiciaSoapLogId = " & m_DB.Quote(VindiciaSoapLogId)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM VindiciaSoapLog WHERE VindiciaSoapLogId = " & m_DB.Number(VindiciaSoapLogId)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class VindiciaSoapLogCollection
		Inherits GenericCollection(Of VindiciaSoapLogRow)
	End Class

End Namespace

