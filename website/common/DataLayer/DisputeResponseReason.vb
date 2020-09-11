Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class DisputeResponseReasonRow
		Inherits DisputeResponseReasonRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, DisputeResponseReasonID as Integer)
			MyBase.New(DB, DisputeResponseReasonID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal DisputeResponseReasonID As Integer) As DisputeResponseReasonRow
			Dim row as DisputeResponseReasonRow 
			
			row = New DisputeResponseReasonRow(DB, DisputeResponseReasonID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal DisputeResponseReasonID As Integer)
			Dim row as DisputeResponseReasonRow 
			
			row = New DisputeResponseReasonRow(DB, DisputeResponseReasonID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from DisputeResponseReason"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods

	End Class
	
	Public MustInherit Class DisputeResponseReasonRowBase
		Private m_DB as Database
		Private m_DisputeResponseReasonID As Integer = nothing
		Private m_DisputeResponseReason As String = nothing
	
	
		Public Property DisputeResponseReasonID As Integer
			Get
				Return m_DisputeResponseReasonID
			End Get
			Set(ByVal Value As Integer)
				m_DisputeResponseReasonID = value
			End Set
		End Property
	
		Public Property DisputeResponseReason As String
			Get
				Return m_DisputeResponseReason
			End Get
			Set(ByVal Value As String)
				m_DisputeResponseReason = value
			End Set
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
	
		Public Sub New(ByVal DB As Database, DisputeResponseReasonID as Integer)
			m_DB = DB
			m_DisputeResponseReasonID = DisputeResponseReasonID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM DisputeResponseReason WHERE DisputeResponseReasonID = " & DB.Number(DisputeResponseReasonID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_DisputeResponseReasonID = Core.GetInt(r.Item("DisputeResponseReasonID"))
			m_DisputeResponseReason = Core.GetString(r.Item("DisputeResponseReason"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
			SQL = " INSERT INTO DisputeResponseReason (" _
				& " DisputeResponseReason" _
				& ") VALUES (" _
				& m_DB.Quote(DisputeResponseReason) _
				& ")"

			DisputeResponseReasonID = m_DB.InsertSQL(SQL)
			
			Return DisputeResponseReasonID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
			SQL = " UPDATE DisputeResponseReason SET " _
				& " DisputeResponseReason = " & m_DB.Quote(DisputeResponseReason) _
				& " WHERE DisputeResponseReasonID = " & m_DB.quote(DisputeResponseReasonID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM DisputeResponseReason WHERE DisputeResponseReasonID = " & m_DB.Number(DisputeResponseReasonID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class DisputeResponseReasonCollection
		Inherits GenericCollection(Of DisputeResponseReasonRow)
	End Class

End Namespace

