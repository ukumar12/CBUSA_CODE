Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class RegistrationStatusRow
		Inherits RegistrationStatusRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, RegistrationStatusID as Integer)
			MyBase.New(DB, RegistrationStatusID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal RegistrationStatusID As Integer) As RegistrationStatusRow
			Dim row as RegistrationStatusRow 
			
			row = New RegistrationStatusRow(DB, RegistrationStatusID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal RegistrationStatusID As Integer)
			Dim row as RegistrationStatusRow 
			
			row = New RegistrationStatusRow(DB, RegistrationStatusID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from RegistrationStatus"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods
        Public Shared Function GetRowByStatus(ByVal DB As Database, ByVal Status As String) As RegistrationStatusRow
            Dim out As New RegistrationStatusRow(DB)
            Dim sql As String = "select * from RegistrationStatus where RegistrationStatus=" & DB.Quote(Status)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetStatuses(ByVal DB As Database) As RegistrationStatusCollection
            Dim out As New RegistrationStatusCollection
            Dim sdr As SqlDataReader = DB.GetReader("select * from RegistrationStatus")
            While sdr.Read
                Dim item As New RegistrationStatusRow(DB)
                item.Load(sdr)
                out.Add(item)
            End While
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetStatusByStep(ByVal DB As Database, ByVal StepNumber As Integer) As RegistrationStatusRow
            Dim out As New RegistrationStatusRow(DB)
            Dim sql As String = "select * from RegistrationStatus where RegistrationStep=" & DB.Number(StepNumber)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function
    End Class
	
	Public MustInherit Class RegistrationStatusRowBase
		Private m_DB as Database
		Private m_RegistrationStatusID As Integer = nothing
        Private m_RegistrationStatus As String = Nothing
        Private m_IsComplete As Boolean = Nothing
        Private m_RegistrationStep As Integer = Nothing
	
		Public Property RegistrationStatusID As Integer
			Get
				Return m_RegistrationStatusID
			End Get
			Set(ByVal Value As Integer)
				m_RegistrationStatusID = value
			End Set
		End Property
	
		Public Property RegistrationStatus As String
			Get
				Return m_RegistrationStatus
			End Get
			Set(ByVal Value As String)
				m_RegistrationStatus = value
			End Set
        End Property

        Public Property IsComplete() As Boolean
            Get
                Return m_IsComplete
            End Get
            Set(ByVal value As Boolean)
                m_IsComplete = value
            End Set
        End Property

        Public Property RegistrationStep() As Integer
            Get
                Return m_RegistrationStep
            End Get
            Set(ByVal value As Integer)
                m_RegistrationStep = value
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
	
		Public Sub New(ByVal DB As Database, RegistrationStatusID as Integer)
			m_DB = DB
			m_RegistrationStatusID = RegistrationStatusID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM RegistrationStatus WHERE RegistrationStatusID = " & DB.Number(RegistrationStatusID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_RegistrationStatusID = Convert.ToInt32(r.Item("RegistrationStatusID"))
            m_RegistrationStatus = Convert.ToString(r.Item("RegistrationStatus"))
            m_IsComplete = Core.GetBoolean(r.Item("IsComplete"))
            m_RegistrationStep = Core.GetInt(r.Item("RegistrationStep"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
            SQL = " INSERT INTO RegistrationStatus (" _
             & " RegistrationStatus" _
             & ",IsComplete" _
             & ",RegistrationStep" _
             & ") VALUES (" _
             & m_DB.Quote(RegistrationStatus) _
             & "," & CInt(IsComplete) _
             & "," & m_DB.NullQuote(RegistrationStep) _
             & ")"

			RegistrationStatusID = m_DB.InsertSQL(SQL)
			
			Return RegistrationStatusID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
            SQL = " UPDATE RegistrationStatus SET " _
             & " RegistrationStatus = " & m_DB.Quote(RegistrationStatus) _
             & ",IsComplete = " & CInt(IsComplete) _
             & ",RegistrationStep = " & m_DB.Number(RegistrationStep) _
             & " WHERE RegistrationStatusID = " & m_DB.Quote(RegistrationStatusID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM RegistrationStatus WHERE RegistrationStatusID = " & m_DB.Number(RegistrationStatusID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class RegistrationStatusCollection
		Inherits GenericCollection(Of RegistrationStatusRow)
	End Class

End Namespace

