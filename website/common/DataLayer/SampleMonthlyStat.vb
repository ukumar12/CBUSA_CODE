Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class SampleMonthlyStatRow
		Inherits SampleMonthlyStatRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, SampleMonthlyStatID as Integer)
			MyBase.New(DB, SampleMonthlyStatID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal SampleMonthlyStatID As Integer) As SampleMonthlyStatRow
			Dim row as SampleMonthlyStatRow 
			
			row = New SampleMonthlyStatRow(DB, SampleMonthlyStatID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal SampleMonthlyStatID As Integer)
			Dim row as SampleMonthlyStatRow 
			
			row = New SampleMonthlyStatRow(DB, SampleMonthlyStatID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from SampleMonthlyStat"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods

	End Class
	
	Public MustInherit Class SampleMonthlyStatRowBase
		Private m_DB as Database
		Private m_SampleMonthlyStatID As Integer = nothing
		Private m_Year As Integer = nothing
		Private m_Month As Integer = nothing
		Private m_StartedUnits As Integer = nothing
		Private m_SoldUnits As Integer = nothing
		Private m_ClosingUnits As Integer = nothing
		Private m_UnsoldUnits As Integer = nothing
		Private m_TimePeriodDate As DateTime = nothing
	
	
		Public Property SampleMonthlyStatID As Integer
			Get
				Return m_SampleMonthlyStatID
			End Get
			Set(ByVal Value As Integer)
				m_SampleMonthlyStatID = value
			End Set
		End Property
	
		Public Property Year As Integer
			Get
				Return m_Year
			End Get
			Set(ByVal Value As Integer)
				m_Year = value
			End Set
		End Property
	
		Public Property Month As Integer
			Get
				Return m_Month
			End Get
			Set(ByVal Value As Integer)
				m_Month = value
			End Set
		End Property
	
		Public Property StartedUnits As Integer
			Get
				Return m_StartedUnits
			End Get
			Set(ByVal Value As Integer)
				m_StartedUnits = value
			End Set
		End Property
	
		Public Property SoldUnits As Integer
			Get
				Return m_SoldUnits
			End Get
			Set(ByVal Value As Integer)
				m_SoldUnits = value
			End Set
		End Property
	
		Public Property ClosingUnits As Integer
			Get
				Return m_ClosingUnits
			End Get
			Set(ByVal Value As Integer)
				m_ClosingUnits = value
			End Set
		End Property
	
		Public Property UnsoldUnits As Integer
			Get
				Return m_UnsoldUnits
			End Get
			Set(ByVal Value As Integer)
				m_UnsoldUnits = value
			End Set
		End Property
	
		Public Property TimePeriodDate As DateTime
			Get
				Return m_TimePeriodDate
			End Get
			Set(ByVal Value As DateTime)
				m_TimePeriodDate = value
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
	
		Public Sub New(ByVal DB As Database, SampleMonthlyStatID as Integer)
			m_DB = DB
			m_SampleMonthlyStatID = SampleMonthlyStatID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM SampleMonthlyStat WHERE SampleMonthlyStatID = " & DB.Number(SampleMonthlyStatID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_SampleMonthlyStatID = Core.GetInt(r.Item("SampleMonthlyStatID"))
			m_Year = Core.GetInt(r.Item("Year"))
			m_Month = Core.GetInt(r.Item("Month"))
			m_StartedUnits = Core.GetInt(r.Item("StartedUnits"))
			m_SoldUnits = Core.GetInt(r.Item("SoldUnits"))
			m_ClosingUnits = Core.GetInt(r.Item("ClosingUnits"))
			m_UnsoldUnits = Core.GetInt(r.Item("UnsoldUnits"))
			m_TimePeriodDate = Core.GetDate(r.Item("TimePeriodDate"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
			SQL = " INSERT INTO SampleMonthlyStat (" _
				& " Year" _
				& ",Month" _
				& ",StartedUnits" _
				& ",SoldUnits" _
				& ",ClosingUnits" _
				& ",UnsoldUnits" _
				& ",TimePeriodDate" _
				& ") VALUES (" _
				& m_DB.Number(Year) _
				& "," & m_DB.Number(Month) _
				& "," & m_DB.Number(StartedUnits) _
				& "," & m_DB.Number(SoldUnits) _
				& "," & m_DB.Number(ClosingUnits) _
				& "," & m_DB.Number(UnsoldUnits) _
				& "," & m_DB.NullQuote(TimePeriodDate) _
				& ")"

			SampleMonthlyStatID = m_DB.InsertSQL(SQL)
			
			Return SampleMonthlyStatID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
			SQL = " UPDATE SampleMonthlyStat SET " _
				& " Year = " & m_DB.Number(Year) _
				& ",Month = " & m_DB.Number(Month) _
				& ",StartedUnits = " & m_DB.Number(StartedUnits) _
				& ",SoldUnits = " & m_DB.Number(SoldUnits) _
				& ",ClosingUnits = " & m_DB.Number(ClosingUnits) _
				& ",UnsoldUnits = " & m_DB.Number(UnsoldUnits) _
				& ",TimePeriodDate = " & m_DB.NullQuote(TimePeriodDate) _
				& " WHERE SampleMonthlyStatID = " & m_DB.quote(SampleMonthlyStatID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM SampleMonthlyStat WHERE SampleMonthlyStatID = " & m_DB.Number(SampleMonthlyStatID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class SampleMonthlyStatCollection
		Inherits GenericCollection(Of SampleMonthlyStatRow)
	End Class

End Namespace

