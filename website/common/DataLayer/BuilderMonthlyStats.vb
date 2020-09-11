Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class BuilderMonthlyStatsRow
		Inherits BuilderMonthlyStatsRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
        Public Sub New(ByVal DB As Database, ByVal BuilderID As Integer, ByVal Year As Integer, ByVal Month As Integer)
            MyBase.New(DB, BuilderID, Year, Month)
        End Sub 'New
		
		'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal BuilderID As Integer, ByVal Year As Integer, ByVal Month As Integer) As BuilderMonthlyStatsRow
            Dim row As BuilderMonthlyStatsRow

            row = New BuilderMonthlyStatsRow(DB, BuilderID, Year, Month)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal BuilderID As Integer, ByVal Year As Integer, ByVal Month As Integer)
            Dim row As BuilderMonthlyStatsRow

            row = New BuilderMonthlyStatsRow(DB, BuilderID, Year, Month)
            row.Remove()
        End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from BuilderMonthlyStats"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods
        Public Shared Function GetBuilderStats(ByVal DB As Database, ByVal BuilderID As Integer, Optional ByVal SortBy As String = "Year,Month", Optional ByVal SortOrder As String = "ASC", Optional ByVal StartDate As DateTime = Nothing, Optional ByVal EndDate As DateTime = Nothing) As DataTable
            Dim sql As String = "select * from BuilderMonthlyStats where BuilderID=" & DB.Number(BuilderID)
            If StartDate <> Nothing Then
                sql &= " and cast((cast(Month as varchar) + '/1/' + cast(Year as varchar)) as DateTime) >= " & DB.Quote(StartDate)
            End If
            If EndDate <> Nothing Then
                sql &= " and cast((cast(Month as varchar) + '/1/' + cast(Year as varchar)) as DateTime) <= " & DB.Quote(EndDate)
            End If
            If SortBy <> String.Empty Then
                sql &= " order by " & Core.ProtectParam(SortBy & " " & SortOrder)
            End If
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function MonthlyDataIsCurrent(ByVal DB As Database, ByVal BuilderID As Integer) As Boolean
            Dim sMonth As Integer = SysParam.GetValue(DB, "BuilderMonthlyDataMonth")
            Dim sYear As Integer = SysParam.GetValue(DB, "BuilderMonthlyDataYear")
            Dim sDiffrence As Integer = DateDiff("m", FormatDateTime(sMonth & "/1/" & sYear, DateFormat.ShortDate), Now())
            Dim sql As String = "select * from BuilderMonthlyStats where BuilderID=" & DB.Number(BuilderID) & " and Year >= " & DB.Number(sYear) & " And Month >= " & DB.Number(sMonth) & " order by Month Asc"
            Dim dt As DataTable = DB.GetDataTable(sql)
            If dt.Rows.Count < sDiffrence Then
                Return False
            Else
                Return True
            End If
        End Function
	End Class
	
	Public MustInherit Class BuilderMonthlyStatsRowBase
		Private m_DB as Database
		Private m_BuilderID As Integer = nothing
		Private m_Year As Integer = nothing
		Private m_Month As Integer = nothing
		Private m_StartedUnits As Integer = nothing
		Private m_SoldUnits As Integer = nothing
		Private m_ClosingUnits As Integer = nothing
		Private m_UnsoldUnits As Integer = nothing
		Private m_Updated As DateTime = nothing
		Private m_TimePeriodDate As DateTime = nothing
	
	
		Public Property BuilderID As Integer
			Get
				Return m_BuilderID
			End Get
			Set(ByVal Value As Integer)
				m_BuilderID = value
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
	
        Public ReadOnly Property Updated() As DateTime
            Get
                Return m_Updated
            End Get
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
	
        Public Sub New(ByVal DB As Database, ByVal BuilderID As Integer, ByVal Year As Integer, ByVal Month As Integer)
            m_DB = DB
            m_BuilderID = BuilderID
            m_Year = Year
            m_Month = Month
        End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
            SQL = "SELECT * FROM BuilderMonthlyStats WHERE BuilderID = " & DB.Number(BuilderID) & " AND Year=" & DB.Number(Year) & " AND Month=" & DB.Number(Month)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_BuilderID = Core.GetInt(r.Item("BuilderID"))
			m_Year = Core.GetInt(r.Item("Year"))
			m_Month = Core.GetInt(r.Item("Month"))
			m_StartedUnits = Core.GetInt(r.Item("StartedUnits"))
			m_SoldUnits = Core.GetInt(r.Item("SoldUnits"))
			m_ClosingUnits = Core.GetInt(r.Item("ClosingUnits"))
			m_UnsoldUnits = Core.GetInt(r.Item("UnsoldUnits"))
			m_Updated = Core.GetDate(r.Item("Updated"))
			m_TimePeriodDate = Core.GetDate(r.Item("TimePeriodDate"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
            SQL = " INSERT INTO BuilderMonthlyStats (" _
             & "BuilderId" _
             & ",Year" _
             & ",Month" _
             & ",StartedUnits" _
             & ",SoldUnits" _
             & ",ClosingUnits" _
             & ",UnsoldUnits" _
             & ",Updated" _
             & ",TimePeriodDate" _
             & ") VALUES (" _
             & m_DB.Number(BuilderID) _
             & "," & m_DB.Number(Year) _
             & "," & m_DB.Number(Month) _
             & "," & m_DB.Number(StartedUnits) _
             & "," & m_DB.Number(SoldUnits) _
             & "," & m_DB.Number(ClosingUnits) _
             & "," & m_DB.Number(UnsoldUnits) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullQuote(TimePeriodDate) _
             & ")"

            If m_DB.ExecuteSQL(SQL) Then
                Return BuilderID
            Else
                Return Nothing
            End If
        End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
            SQL = " UPDATE BuilderMonthlyStats SET " _
             & " StartedUnits = " & m_DB.Number(StartedUnits) _
             & ",SoldUnits = " & m_DB.Number(SoldUnits) _
             & ",ClosingUnits = " & m_DB.Number(ClosingUnits) _
             & ",UnsoldUnits = " & m_DB.Number(UnsoldUnits) _
             & ",Updated = " & m_DB.NullQuote(Now) _
             & ",TimePeriodDate = " & m_DB.NullQuote(TimePeriodDate) _
             & " WHERE BuilderID = " & m_DB.Quote(BuilderID) _
             & " AND Year = " & m_DB.Number(Year) _
             & " AND Month = " & m_DB.Number(Month)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
            SQL = "DELETE FROM BuilderMonthlyStats WHERE BuilderID = " & m_DB.Number(BuilderID) & " AND Year=" & DB.Number(Year) & " AND Month=" & DB.Number(Month)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class BuilderMonthlyStatsCollection
		Inherits GenericCollection(Of BuilderMonthlyStatsRow)
	End Class

End Namespace

