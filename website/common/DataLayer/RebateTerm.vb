Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class RebateTermRow
		Inherits RebateTermRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, RebateTermsID as Integer)
			MyBase.New(DB, RebateTermsID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal RebateTermsID As Integer) As RebateTermRow
			Dim row as RebateTermRow 
			
			row = New RebateTermRow(DB, RebateTermsID)
			row.Load()
			
			Return row
        End Function

        Public Shared Function GetRowByVendor(ByVal DB As Database, ByVal VendorID As Integer, ByVal StartYear As Integer, ByVal StartQuarter As Integer) As RebateTermRow
            Dim row As RebateTermRow
            Dim RebateTermsID As Integer = 0
            Try
                RebateTermsID = DB.ExecuteScalar("Select Top 1 RebateTermsID From RebateTerm Where VendorId = " & DB.Number(VendorID) & " And StartYear = " & DB.Number(StartYear) & " And StartQuarter = " & DB.Number(StartQuarter) & " Order By PurchaseRangeFloor Asc")
            Catch ex As Exception
            End Try

            row = New RebateTermRow(DB, RebateTermsID)
            row.Load()

            Return row
        End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal RebateTermsID As Integer)
			Dim row as RebateTermRow 
			
			row = New RebateTermRow(DB, RebateTermsID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from RebateTerm"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods
        Public Shared Function GetCurrentTerms(ByVal DB As Database, ByVal VendorID As Integer) As DataTable
            Dim sql As String = "select r.* from RebateTerm r, (select top 1 StartYear, StartQuarter from RebateTerm where VendorId=" & DB.Number(VendorID) & " order by StartYear desc, StartQuarter desc) as Latest where r.StartYear=Latest.StartYear and r.StartQuarter=Latest.StartQuarter and r.VendorID=" & DB.Number(VendorID) & " order by PurchaseRangeFloor asc"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetNextFloor(ByVal DB As Database, ByVal VendorID As Integer, Optional ByVal PeriodQuarter As Integer = 0, Optional ByVal PeriodYear As Integer = 0) As Double
            If PeriodQuarter = Nothing Or PeriodYear = Nothing Then
                Dim current As Integer = Math.Floor(Now.Month / 3) + 1
                PeriodQuarter = IIf(current = 4, 1, current + 1)
                PeriodYear = IIf(PeriodQuarter = 1, Now.Year + 1, Now.Year)
            End If
            Dim sql As String = "select top 1 PurchaseRangeCeiling from RebateTerm where VendorID=" & DB.Number(VendorID) & " and StartQuarter=" & DB.Number(PeriodQuarter) & " and StartYear=" & DB.Number(PeriodYear) & " order by PurchaseRangeCeiling desc"
            Dim out As Object = DB.ExecuteScalar(sql)
            Return IIf(IsDBNull(out), 0, out)
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal RebateTermsID As Integer, ByVal UpdateRanges As Boolean)
            DB.BeginTransaction()
            Try
                Dim row As New RebateTermRow(DB, RebateTermsID)
                If UpdateRanges Then
                    Dim sql As String = "update RebateTerm set PurchaseRangeCeiling=" & IIf(row.PurchaseRangeCeiling > 0, row.PurchaseRangeCeiling, "null") & " where PurchaseRangeCeiling=" & row.PurchaseRangeFloor
                    DB.ExecuteSQL(sql)
                End If
                row.Remove()

                DB.CommitTransaction()
            Catch ex As SqlException
                If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then
                    DB.RollbackTransaction()
                End If
                Logger.Error(Logger.GetErrorMessage(ex))
            End Try
        End Sub

        Public Shared Sub MakeFlat(ByVal DB As Database, ByVal VendorID As Integer)
            Dim sql As String = "delete from RebateTerm where VendorID=" & DB.Number(VendorID) & " and PurchaseRangeFloor > 0"
            DB.ExecuteSQL(sql)
        End Sub
	End Class
	
	Public MustInherit Class RebateTermRowBase
		Private m_DB as Database
		Private m_RebateTermsID As Integer = nothing
		Private m_VendorID As Integer = nothing
		Private m_StartYear As Integer = nothing
		Private m_StartQuarter As Integer = nothing
		Private m_PurchaseRangeFloor As Double = nothing
		Private m_PurchaseRangeCeiling As Double = nothing
		Private m_IsAnnualPurchaseRange As Boolean = nothing
        Private m_RebatePercentage As Double = Nothing
		Private m_Created As DateTime = nothing
		Private m_CreatorVendorAccountID As Integer = nothing
		Private m_Approved As DateTime = nothing
        Private m_ApproverAdminID As Integer = Nothing
        Private m_LogMsg As String = Nothing
	
	
		Public Property RebateTermsID As Integer
			Get
				Return m_RebateTermsID
			End Get
			Set(ByVal Value As Integer)
				m_RebateTermsID = value
			End Set
		End Property
	
		Public Property VendorID As Integer
			Get
				Return m_VendorID
			End Get
			Set(ByVal Value As Integer)
				m_VendorID = value
			End Set
		End Property
	
		Public Property StartYear As Integer
			Get
				Return m_StartYear
			End Get
			Set(ByVal Value As Integer)
				m_StartYear = value
			End Set
		End Property
	
		Public Property StartQuarter As Integer
			Get
				Return m_StartQuarter
			End Get
			Set(ByVal Value As Integer)
				m_StartQuarter = value
			End Set
		End Property
	
		Public Property PurchaseRangeFloor As Double
			Get
				Return m_PurchaseRangeFloor
			End Get
			Set(ByVal Value As Double)
				m_PurchaseRangeFloor = value
			End Set
		End Property
	
		Public Property PurchaseRangeCeiling As Double
			Get
				Return m_PurchaseRangeCeiling
			End Get
			Set(ByVal Value As Double)
				m_PurchaseRangeCeiling = value
			End Set
		End Property
	
		Public Property IsAnnualPurchaseRange As Boolean
			Get
				Return m_IsAnnualPurchaseRange
			End Get
			Set(ByVal Value As Boolean)
				m_IsAnnualPurchaseRange = value
			End Set
		End Property
	
        Public Property RebatePercentage() As Double
            Get
                Return m_RebatePercentage
            End Get
            Set(ByVal Value As Double)
                m_RebatePercentage = Value
            End Set
        End Property
	
        Public ReadOnly Property Created() As DateTime
            Get
                Return m_Created
            End Get
        End Property
	
		Public Property CreatorVendorAccountID As Integer
			Get
				Return m_CreatorVendorAccountID
			End Get
			Set(ByVal Value As Integer)
				m_CreatorVendorAccountID = value
			End Set
		End Property
	
		Public Property Approved As DateTime
			Get
				Return m_Approved
			End Get
			Set(ByVal Value As DateTime)
				m_Approved = value
			End Set
		End Property
	
		Public Property ApproverAdminID As Integer
			Get
				Return m_ApproverAdminID
			End Get
			Set(ByVal Value As Integer)
				m_ApproverAdminID = value
			End Set
        End Property

        Public Property LogMsg() As String
            Get
                Return m_LogMsg
            End Get
            Set(ByVal Value As String)
                m_LogMsg = Value
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
	
		Public Sub New(ByVal DB As Database, RebateTermsID as Integer)
			m_DB = DB
			m_RebateTermsID = RebateTermsID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM RebateTerm WHERE RebateTermsID = " & DB.Number(RebateTermsID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_RebateTermsID = Core.GetInt(r.Item("RebateTermsID"))
			m_VendorID = Core.GetInt(r.Item("VendorID"))
			m_StartYear = Core.GetInt(r.Item("StartYear"))
			m_StartQuarter = Core.GetInt(r.Item("StartQuarter"))
			m_PurchaseRangeFloor = Core.GetDouble(r.Item("PurchaseRangeFloor"))
			m_PurchaseRangeCeiling = Core.GetDouble(r.Item("PurchaseRangeCeiling"))
			m_IsAnnualPurchaseRange = Core.GetBoolean(r.Item("IsAnnualPurchaseRange"))
            m_RebatePercentage = Core.GetDouble(r.Item("RebatePercentage"))
			m_Created = Core.GetDate(r.Item("Created"))
			m_CreatorVendorAccountID = Core.GetInt(r.Item("CreatorVendorAccountID"))
			m_Approved = Core.GetDate(r.Item("Approved"))
            m_ApproverAdminID = Core.GetInt(r.Item("ApproverAdminID"))
            m_LogMsg = Core.GetString(r.Item("LogMsg"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
            SQL = " INSERT INTO RebateTerm (" _
             & " VendorID" _
             & ",StartYear" _
             & ",StartQuarter" _
             & ",PurchaseRangeFloor" _
             & ",PurchaseRangeCeiling" _
             & ",IsAnnualPurchaseRange" _
             & ",RebatePercentage" _
             & ",Created" _
             & ",CreatorVendorAccountID" _
             & ",Approved" _
             & ",ApproverAdminID" _
             & ",LogMsg" _
             & ") VALUES (" _
             & m_DB.NullNumber(VendorID) _
             & "," & m_DB.Number(StartYear) _
             & "," & m_DB.Number(StartQuarter) _
             & "," & m_DB.Number(PurchaseRangeFloor) _
             & "," & m_DB.Number(PurchaseRangeCeiling) _
             & "," & CInt(IsAnnualPurchaseRange) _
             & "," & m_DB.Number(RebatePercentage) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(CreatorVendorAccountID) _
             & "," & m_DB.NullQuote(Approved) _
             & "," & m_DB.NullNumber(ApproverAdminID) _
             & "," & m_DB.Quote(LogMsg) _
             & ")"

			RebateTermsID = m_DB.InsertSQL(SQL)
			
			Return RebateTermsID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
            SQL = " UPDATE RebateTerm SET " _
             & " VendorID = " & m_DB.NullNumber(VendorID) _
             & ",StartYear = " & m_DB.Number(StartYear) _
             & ",StartQuarter = " & m_DB.Number(StartQuarter) _
             & ",PurchaseRangeFloor = " & m_DB.Number(PurchaseRangeFloor) _
             & ",PurchaseRangeCeiling = " & m_DB.Number(PurchaseRangeCeiling) _
             & ",IsAnnualPurchaseRange = " & CInt(IsAnnualPurchaseRange) _
             & ",RebatePercentage = " & m_DB.Number(RebatePercentage) _
             & ",Approved = " & m_DB.NullQuote(Approved) _
             & ",ApproverAdminID = " & m_DB.NullNumber(ApproverAdminID) _
             & ",LogMsg = " & m_DB.Quote(LogMsg) _
             & " WHERE RebateTermsID = " & m_DB.Quote(RebateTermsID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM RebateTerm WHERE RebateTermsID = " & m_DB.Number(RebateTermsID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class RebateTermCollection
		Inherits GenericCollection(Of RebateTermRow)
	End Class

End Namespace

