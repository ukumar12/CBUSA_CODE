Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class SalesReportBuilderTotalAmountRow
		Inherits SalesReportBuilderTotalAmountRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
        Public Sub New(ByVal DB As Database, ByVal SalesReportID As Integer, ByVal BuilderId As Integer)
            MyBase.New(DB, SalesReportID, BuilderId)
        End Sub 'New
		
		'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal SalesReportID As Integer, ByVal BuilderId As Integer) As SalesReportBuilderTotalAmountRow
            Dim row As SalesReportBuilderTotalAmountRow

            row = New SalesReportBuilderTotalAmountRow(DB, SalesReportID, BuilderId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal SalesReportID As Integer, ByVal BuilderId As Integer)
            Dim row As SalesReportBuilderTotalAmountRow

            row = New SalesReportBuilderTotalAmountRow(DB, SalesReportID, BuilderId)
            row.Remove()
        End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from SalesReportBuilderTotalAmount"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods

	End Class
	
	Public MustInherit Class SalesReportBuilderTotalAmountRowBase
		Private m_DB as Database
		Private m_SalesReportID As Integer = nothing
		Private m_BuilderID As Integer = nothing
		Private m_TotalAmount As Double = nothing
		Private m_Created As DateTime = nothing
		Private m_CreatorVendorAccountID As Integer = nothing
	
	
		Public Property SalesReportID As Integer
			Get
				Return m_SalesReportID
			End Get
			Set(ByVal Value As Integer)
				m_SalesReportID = value
			End Set
		End Property
	
		Public Property BuilderID As Integer
			Get
				Return m_BuilderID
			End Get
			Set(ByVal Value As Integer)
				m_BuilderID = value
			End Set
		End Property
	
		Public Property TotalAmount As Double
			Get
				Return m_TotalAmount
			End Get
			Set(ByVal Value As Double)
				m_TotalAmount = value
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
	
        Public Sub New(ByVal DB As Database, ByVal SalesReportID As Integer, ByVal BuilderId As Integer)
            m_DB = DB
            m_SalesReportID = SalesReportID
            m_BuilderID = BuilderId
        End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
            SQL = "SELECT * FROM SalesReportBuilderTotalAmount WHERE SalesReportID = " & DB.Number(SalesReportID) & " and BuilderID=" & DB.Number(BuilderID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_SalesReportID = Convert.ToInt32(r.Item("SalesReportID"))
			m_BuilderID = Convert.ToInt32(r.Item("BuilderID"))
			m_TotalAmount = Convert.ToDouble(r.Item("TotalAmount"))
			m_Created = Convert.ToDateTime(r.Item("Created"))
			m_CreatorVendorAccountID = Convert.ToInt32(r.Item("CreatorVendorAccountID"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
            SQL = " INSERT INTO SalesReportBuilderTotalAmount (" _
             & " SalesReportID" _
             & ",BuilderID" _
             & ",TotalAmount" _
             & ",Created" _
             & ",CreatorVendorAccountID" _
             & ") VALUES (" _
             & m_DB.NullNumber(SalesReportID) _
             & "," & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.Number(TotalAmount) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(CreatorVendorAccountID) _
             & ")"

            m_DB.ExecuteSQL(SQL)
			
			Return SalesReportID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
            SQL = " UPDATE SalesReportBuilderTotalAmount SET " _
             & " TotalAmount = " & m_DB.Number(TotalAmount) _
             & " WHERE SalesReportID = " & m_DB.Quote(SalesReportID) _
             & " AND BuilderID = " & m_DB.Number(BuilderID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
            SQL = "DELETE FROM SalesReportBuilderTotalAmount WHERE SalesReportID = " & m_DB.Number(SalesReportID) & " AND BuilderID=" & m_DB.Number(BuilderID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class SalesReportBuilderTotalAmountCollection
		Inherits GenericCollection(Of SalesReportBuilderTotalAmountRow)
	End Class

End Namespace

