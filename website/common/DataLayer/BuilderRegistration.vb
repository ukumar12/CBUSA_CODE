Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class BuilderRegistrationRow
		Inherits BuilderRegistrationRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, BuilderRegistrationID as Integer)
			MyBase.New(DB, BuilderRegistrationID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal BuilderRegistrationID As Integer) As BuilderRegistrationRow
			Dim row as BuilderRegistrationRow 
			
			row = New BuilderRegistrationRow(DB, BuilderRegistrationID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal BuilderRegistrationID As Integer)
			Dim row as BuilderRegistrationRow 
			
			row = New BuilderRegistrationRow(DB, BuilderRegistrationID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from BuilderRegistration"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods
        Public Shared Function GetRowByBuilder(ByVal DB As Database, ByVal BuilderId As Integer) As BuilderRegistrationRow
            Dim out As New BuilderRegistrationRow(DB)
            Dim sql As String = "select * from BuilderRegistration where BuilderId=" & DB.Number(BuilderId)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read() Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function
	End Class
	
	Public MustInherit Class BuilderRegistrationRowBase
		Private m_DB as Database
		Private m_BuilderRegistrationID As Integer = nothing
		Private m_BuilderID As Integer = nothing
        Private m_YearStarted As Integer = Nothing
        Private m_Employees As Integer = Nothing
        Private m_HomesBuiltAndDelivered As Integer = Nothing
        Private m_HomeStartsLastYear As Integer = Nothing
        Private m_HomeStartsNextYear As Integer = Nothing
        Private m_SizeRangeMin As Integer = Nothing
        Private m_SizeRangeMax As Integer = Nothing
        Private m_PriceRangeMin As Double = Nothing
        Private m_PriceRangeMax As Double = Nothing
        Private m_AvgCostPerSqFt As Integer = Nothing
        Private m_RevenueLastYear As Double = Nothing
        Private m_RevenueNextYear As Double = Nothing
        Private m_TotalCOGS As Double = Nothing
        Private m_Affiliations As String = Nothing
        Private m_Awards As String = Nothing
        Private m_WhereYouBuild As String = Nothing
        Private m_AcceptsTerms As Boolean = Nothing
        Private m_Submitted As DateTime = Nothing
        Private m_Updated As DateTime = Nothing
        Private m_ClosingsLastYear As Integer = Nothing
        Private m_DirectCostsLastYear As Double = Nothing
        Private m_UnsoldLastYear As Integer = Nothing
        Private m_UnderConstructionLastYear As Integer = Nothing
        Private m_CompleteDate As DateTime = Nothing
        Private m_RegistrationStatusID As Integer = Nothing

        Public Property BuilderRegistrationID() As Integer
            Get
                Return m_BuilderRegistrationID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderRegistrationID = value
            End Set
        End Property

        Public Property BuilderID() As Integer
            Get
                Return m_BuilderID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderID = value
            End Set
        End Property

        Public Property YearStarted() As Integer
            Get
                Return m_YearStarted
            End Get
            Set(ByVal Value As Integer)
                m_YearStarted = value
            End Set
        End Property

        Public Property Employees() As Integer
            Get
                Return m_Employees
            End Get
            Set(ByVal Value As Integer)
                m_Employees = value
            End Set
        End Property

        Public Property HomesBuiltAndDelivered() As Integer
            Get
                Return m_HomesBuiltAndDelivered
            End Get
            Set(ByVal Value As Integer)
                m_HomesBuiltAndDelivered = value
            End Set
        End Property

        Public Property HomeStartsLastYear() As Integer
            Get
                Return m_HomeStartsLastYear
            End Get
            Set(ByVal Value As Integer)
                m_HomeStartsLastYear = value
            End Set
        End Property

        Public Property HomeStartsNextYear() As Integer
            Get
                Return m_HomeStartsNextYear
            End Get
            Set(ByVal Value As Integer)
                m_HomeStartsNextYear = value
            End Set
        End Property

        Public Property SizeRangeMin() As Integer
            Get
                Return m_SizeRangeMin
            End Get
            Set(ByVal Value As Integer)
                m_SizeRangeMin = value
            End Set
        End Property

        Public Property SizeRangeMax() As Integer
            Get
                Return m_SizeRangeMax
            End Get
            Set(ByVal Value As Integer)
                m_SizeRangeMax = value
            End Set
        End Property

        Public Property PriceRangeMin() As Double
            Get
                Return m_PriceRangeMin
            End Get
            Set(ByVal Value As Double)
                m_PriceRangeMin = value
            End Set
        End Property

        Public Property PriceRangeMax() As Double
            Get
                Return m_PriceRangeMax
            End Get
            Set(ByVal Value As Double)
                m_PriceRangeMax = value
            End Set
        End Property

        Public Property AvgCostPerSqFt() As Integer
            Get
                Return m_AvgCostPerSqFt
            End Get
            Set(ByVal Value As Integer)
                m_AvgCostPerSqFt = value
            End Set
        End Property

        Public Property RevenueLastYear() As Double
            Get
                Return m_RevenueLastYear
            End Get
            Set(ByVal Value As Double)
                m_RevenueLastYear = value
            End Set
        End Property

        Public Property RevenueNextYear() As Double
            Get
                Return m_RevenueNextYear
            End Get
            Set(ByVal Value As Double)
                m_RevenueNextYear = value
            End Set
        End Property

        Public Property TotalCOGS() As Double
            Get
                Return m_TotalCOGS
            End Get
            Set(ByVal Value As Double)
                m_TotalCOGS = value
            End Set
        End Property

        Public Property Affiliations() As String
            Get
                Return m_Affiliations
            End Get
            Set(ByVal Value As String)
                m_Affiliations = value
            End Set
        End Property

        Public Property Awards() As String
            Get
                Return m_Awards
            End Get
            Set(ByVal value As String)
                m_Awards = value
            End Set
        End Property

        Public Property WhereYouBuild() As String
            Get
                Return m_WhereYouBuild
            End Get
            Set(ByVal Value As String)
                m_WhereYouBuild = value
            End Set
        End Property

        Public Property AcceptsTerms() As Boolean
            Get
                Return m_AcceptsTerms
            End Get
            Set(ByVal Value As Boolean)
                m_AcceptsTerms = value
            End Set
        End Property

        Public Property Submitted() As DateTime
            Get
                Return m_Submitted
            End Get
            Set(ByVal Value As DateTime)
                m_Submitted = value
            End Set
        End Property

        Public Property Updated() As DateTime
            Get
                Return m_Updated
            End Get
            Set(ByVal Value As DateTime)
                m_Updated = value
            End Set
        End Property

        Public Property ClosingsLastYear() As Integer
            Get
                Return m_ClosingsLastYear
            End Get
            Set(ByVal value As Integer)
                m_ClosingsLastYear = value
            End Set
        End Property

        Public Property DirectCostsLastYear() As Double
            Get
                Return m_DirectCostsLastYear
            End Get
            Set(ByVal value As Double)
                m_DirectCostsLastYear = value
            End Set
        End Property

        Public Property UnsoldLastYear() As Integer
            Get
                Return m_UnsoldLastYear
            End Get
            Set(ByVal value As Integer)
                m_UnsoldLastYear = value
            End Set
        End Property

        Public Property UnderConstructionLastYear() As Integer
            Get
                Return m_UnderConstructionLastYear
            End Get
            Set(ByVal value As Integer)
                m_UnderConstructionLastYear = value
            End Set
        End Property

        Public Property CompleteDate() As DateTime
            Get
                Return m_CompleteDate
            End Get
            Set(ByVal value As DateTime)
                m_CompleteDate = value
            End Set
        End Property

        Public Property RegistrationStatusID() As Integer
            Get
                Return m_RegistrationStatusID
            End Get
            Set(ByVal value As Integer)
                m_RegistrationStatusID = value
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

        Public Sub New(ByVal DB As Database, ByVal BuilderRegistrationID As Integer)
            m_DB = DB
            m_BuilderRegistrationID = BuilderRegistrationID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM BuilderRegistration WHERE BuilderRegistrationID = " & DB.Number(BuilderRegistrationID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_BuilderRegistrationID = Convert.ToInt32(r.Item("BuilderRegistrationID"))
            m_BuilderID = Convert.ToInt32(r.Item("BuilderID"))
            m_YearStarted = Convert.ToInt32(r.Item("YearStarted"))
            m_Employees = Convert.ToInt32(r.Item("Employees"))
            m_HomesBuiltAndDelivered = Convert.ToInt32(r.Item("HomesBuiltAndDelivered"))
            m_HomeStartsLastYear = Convert.ToInt32(r.Item("HomeStartsLastYear"))
            If IsDBNull(r.Item("HomeStartsNextYear")) Then
                m_HomeStartsNextYear = Nothing
            Else
                m_HomeStartsNextYear = Convert.ToInt32(r.Item("HomeStartsNextYear"))
            End If
            m_SizeRangeMin = Convert.ToInt32(r.Item("SizeRangeMin"))
            m_SizeRangeMax = Convert.ToInt32(r.Item("SizeRangeMax"))
            m_PriceRangeMin = Convert.ToDouble(r.Item("PriceRangeMin"))
            m_PriceRangeMax = Convert.ToDouble(r.Item("PriceRangeMax"))
            m_AvgCostPerSqFt = Convert.ToInt32(r.Item("AvgCostPerSqFt"))
            m_RevenueLastYear = Convert.ToDouble(r.Item("RevenueLastYear"))
            m_RevenueNextYear = Convert.ToDouble(r.Item("RevenueNextYear"))
            m_TotalCOGS = Convert.ToDouble(r.Item("TotalCOGS"))
            If IsDBNull(r.Item("Affiliations")) Then
                m_Affiliations = Nothing
            Else
                m_Affiliations = Convert.ToString(r.Item("Affiliations"))
            End If
            If IsDBNull(r.Item("Awards")) Then
                m_Awards = Nothing
            Else
                m_Awards = Convert.ToString(r.Item("Awards"))
            End If
            m_WhereYouBuild = Convert.ToString(r.Item("WhereYouBuild"))
            m_AcceptsTerms = Convert.ToBoolean(r.Item("AcceptsTerms"))
            m_Submitted = Convert.ToDateTime(r.Item("Submitted"))
            If IsDBNull(r.Item("Updated")) Then
                m_Updated = Nothing
            Else
                m_Updated = Convert.ToDateTime(r.Item("Updated"))
            End If
            m_ClosingsLastYear = Convert.ToInt32(r.Item("ClosingsLastYear"))
            m_DirectCostsLastYear = Convert.ToInt32(r.Item("DirectCostsLastYear"))
            m_UnsoldLastYear = Convert.ToInt32(r.Item("UnsoldLastYear"))
            m_UnderConstructionLastYear = Convert.ToInt32(r.Item("UnderConstructionLastYear"))
            m_CompleteDate = Core.GetDate(r.Item("CompleteDate"))
            m_RegistrationStatusID = Core.GetInt(r.Item("RegistrationStatusID"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO BuilderRegistration (" _
             & " BuilderID" _
             & ",YearStarted" _
             & ",Employees" _
             & ",HomesBuiltAndDelivered" _
             & ",HomeStartsLastYear" _
             & ",HomeStartsNextYear" _
             & ",SizeRangeMin" _
             & ",SizeRangeMax" _
             & ",PriceRangeMin" _
             & ",PriceRangeMax" _
             & ",AvgCostPerSqFt" _
             & ",RevenueLastYear" _
             & ",RevenueNextYear" _
             & ",TotalCOGS" _
             & ",Affiliations" _
             & ",Awards" _
             & ",WhereYouBuild" _
             & ",AcceptsTerms" _
             & ",Submitted" _
             & ",Updated" _
             & ",ClosingsLastYear" _
             & ",DirectCostsLastYear" _
             & ",UnsoldLastYear" _
             & ",UnderConstructionLastYear" _
             & ",CompleteDate" _
             & ",RegistrationStatusID" _
             & ") VALUES (" _
             & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.Number(YearStarted) _
             & "," & m_DB.Number(Employees) _
             & "," & m_DB.Number(HomesBuiltAndDelivered) _
             & "," & m_DB.Number(HomeStartsLastYear) _
             & "," & m_DB.Number(HomeStartsNextYear) _
             & "," & m_DB.Number(SizeRangeMin) _
             & "," & m_DB.Number(SizeRangeMax) _
             & "," & m_DB.Number(PriceRangeMin) _
             & "," & m_DB.Number(PriceRangeMax) _
             & "," & m_DB.Number(AvgCostPerSqFt) _
             & "," & m_DB.Number(RevenueLastYear) _
             & "," & m_DB.Number(RevenueNextYear) _
             & "," & m_DB.Number(TotalCOGS) _
             & "," & m_DB.Quote(Affiliations) _
             & "," & m_DB.Quote(Awards) _
             & "," & m_DB.Quote(WhereYouBuild) _
             & "," & CInt(AcceptsTerms) _
             & ", GETDATE() " _
             & "," & m_DB.NullQuote(Updated) _
             & "," & m_DB.Number(ClosingsLastYear) _
             & "," & m_DB.Number(DirectCostsLastYear) _
             & "," & m_DB.Number(UnsoldLastYear) _
             & "," & m_DB.Number(UnderConstructionLastYear) _
             & "," & m_DB.NullQuote(CompleteDate) _
             & "," & m_DB.Quote(RegistrationStatusID) _
             & ")"

            BuilderRegistrationID = m_DB.InsertSQL(SQL)

            Return BuilderRegistrationID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE BuilderRegistration SET " _
             & " BuilderID = " & m_DB.NullNumber(BuilderID) _
             & ",YearStarted = " & m_DB.Number(YearStarted) _
             & ",Employees = " & m_DB.Number(Employees) _
             & ",HomesBuiltAndDelivered = " & m_DB.Number(HomesBuiltAndDelivered) _
             & ",HomeStartsLastYear = " & m_DB.Number(HomeStartsLastYear) _
             & ",HomeStartsNextYear = " & m_DB.Number(HomeStartsNextYear) _
             & ",SizeRangeMin = " & m_DB.Number(SizeRangeMin) _
             & ",SizeRangeMax = " & m_DB.Number(SizeRangeMax) _
             & ",PriceRangeMin = " & m_DB.Number(PriceRangeMin) _
             & ",PriceRangeMax = " & m_DB.Number(PriceRangeMax) _
             & ",AvgCostPerSqFt = " & m_DB.Number(AvgCostPerSqFt) _
             & ",RevenueLastYear = " & m_DB.Number(RevenueLastYear) _
             & ",RevenueNextYear = " & m_DB.Number(RevenueNextYear) _
             & ",TotalCOGS = " & m_DB.Number(TotalCOGS) _
             & ",Affiliations = " & m_DB.Quote(Affiliations) _
             & ",Awards = " & m_DB.Quote(Awards) _
             & ",WhereYouBuild = " & m_DB.Quote(WhereYouBuild) _
             & ",AcceptsTerms = " & CInt(AcceptsTerms) _
             & ",Submitted = " & m_DB.NullQuote(Submitted) _
             & ",Updated = " & m_DB.NullQuote(Updated) _
             & ",ClosingsLastYear = " & m_DB.Number(ClosingsLastYear) _
             & ",DirectCostsLastYear = " & m_DB.Number(DirectCostsLastYear) _
             & ",UnsoldLastYear = " & m_DB.Number(UnsoldLastYear) _
             & ",UnderConstructionLastYear = " & m_DB.Number(UnderConstructionLastYear) _
             & ",CompleteDate = " & m_DB.NullQuote(CompleteDate) _
             & ",RegistrationStatusID = " & m_DB.Number(RegistrationStatusID) _
             & " WHERE BuilderRegistrationID = " & m_DB.Quote(BuilderRegistrationID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM BuilderRegistration WHERE BuilderRegistrationID = " & m_DB.Number(BuilderRegistrationID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class BuilderRegistrationCollection
		Inherits GenericCollection(Of BuilderRegistrationRow)
	End Class

End Namespace

