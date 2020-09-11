Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class AutomaticMessagesRow
		Inherits AutomaticMessagesRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, AutomaticMessageID as Integer)
			MyBase.New(DB, AutomaticMessageID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal AutomaticMessageID As Integer) As AutomaticMessagesRow
			Dim row as AutomaticMessagesRow 
			
			row = New AutomaticMessagesRow(DB, AutomaticMessageID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal AutomaticMessageID As Integer)
			Dim row as AutomaticMessagesRow 
			
			row = New AutomaticMessagesRow(DB, AutomaticMessageID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from AutomaticMessages"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods

	End Class
	
	Public MustInherit Class AutomaticMessagesRowBase
		Private m_DB as Database
		Private m_AutomaticMessageID As Integer = nothing
		Private m_Subject As String = nothing
		Private m_Title As String = nothing
		Private m_Condition As String = nothing
		Private m_Message As String = nothing
		Private m_IsEmail As Boolean = nothing
		Private m_IsMessage As Boolean = nothing
		Private m_CCList As String = nothing
	
	
		Public Property AutomaticMessageID As Integer
			Get
				Return m_AutomaticMessageID
			End Get
			Set(ByVal Value As Integer)
				m_AutomaticMessageID = value
			End Set
		End Property
	
		Public Property Subject As String
			Get
				Return m_Subject
			End Get
			Set(ByVal Value As String)
				m_Subject = value
			End Set
		End Property
	
		Public Property Title As String
			Get
				Return m_Title
			End Get
			Set(ByVal Value As String)
				m_Title = value
			End Set
		End Property
	
		Public Property Condition As String
			Get
				Return m_Condition
			End Get
			Set(ByVal Value As String)
				m_Condition = value
			End Set
		End Property
	
		Public Property Message As String
			Get
				Return m_Message
			End Get
			Set(ByVal Value As String)
				m_Message = value
			End Set
		End Property
	
		Public Property IsEmail As Boolean
			Get
				Return m_IsEmail
			End Get
			Set(ByVal Value As Boolean)
				m_IsEmail = value
			End Set
		End Property
	
		Public Property IsMessage As Boolean
			Get
				Return m_IsMessage
			End Get
			Set(ByVal Value As Boolean)
				m_IsMessage = value
			End Set
		End Property
	
		Public Property CCList As String
			Get
				Return m_CCList
			End Get
			Set(ByVal Value As String)
				m_CCList = value
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
	
		Public Sub New(ByVal DB As Database, AutomaticMessageID as Integer)
			m_DB = DB
			m_AutomaticMessageID = AutomaticMessageID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM AutomaticMessages WHERE AutomaticMessageID = " & DB.Number(AutomaticMessageID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_AutomaticMessageID = Convert.ToInt32(r.Item("AutomaticMessageID"))
			m_Subject = Convert.ToString(r.Item("Subject"))
			m_Title = Convert.ToString(r.Item("Title"))
			m_Condition = Convert.ToString(r.Item("Condition"))
			m_Message = Convert.ToString(r.Item("Message"))
			m_IsEmail = Convert.ToBoolean(r.Item("IsEmail"))
			m_IsMessage = Convert.ToBoolean(r.Item("IsMessage"))
			if IsDBNull(r.Item("CCList")) then
				m_CCList = nothing
			else
				m_CCList = Convert.ToString(r.Item("CCList"))
			end if	
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
			SQL = " INSERT INTO AutomaticMessages (" _
				& " Subject" _
				& ",Title" _
				& ",Condition" _
				& ",Message" _
				& ",IsEmail" _
				& ",IsMessage" _
				& ",CCList" _
				& ") VALUES (" _
				& m_DB.Quote(Subject) _
				& "," & m_DB.Quote(Title) _
				& "," & m_DB.Quote(Condition) _
				& "," & m_DB.Quote(Message) _
				& "," & CInt(IsEmail) _
				& "," & CInt(IsMessage) _
				& "," & m_DB.Quote(CCList) _
				& ")"

			AutomaticMessageID = m_DB.InsertSQL(SQL)
			
			Return AutomaticMessageID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
			SQL = " UPDATE AutomaticMessages SET " _
				& " Subject = " & m_DB.Quote(Subject) _
				& ",Title = " & m_DB.Quote(Title) _
				& ",Condition = " & m_DB.Quote(Condition) _
				& ",Message = " & m_DB.Quote(Message) _
				& ",IsEmail = " & CInt(IsEmail) _
				& ",IsMessage = " & CInt(IsMessage) _
				& ",CCList = " & m_DB.Quote(CCList) _
				& " WHERE AutomaticMessageID = " & m_DB.quote(AutomaticMessageID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM AutomaticMessages WHERE AutomaticMessageID = " & m_DB.Number(AutomaticMessageID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class AutomaticMessagesCollection
		Inherits GenericCollection(Of AutomaticMessagesRow)
	End Class

End Namespace

