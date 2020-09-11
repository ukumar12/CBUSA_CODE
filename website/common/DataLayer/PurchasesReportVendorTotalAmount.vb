Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class PurchasesReportVendorTotalAmountRow
		Inherits PurchasesReportVendorTotalAmountRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
        Public Sub New(ByVal DB As Database, ByVal PurchasesReportID As Integer, ByVal VendorID As Integer)
            MyBase.New(DB, PurchasesReportID, VendorID)
        End Sub 'New
		
		'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal PurchasesReportID As Integer, ByVal VendorID As Integer) As PurchasesReportVendorTotalAmountRow
            Dim row As PurchasesReportVendorTotalAmountRow

            row = New PurchasesReportVendorTotalAmountRow(DB, PurchasesReportID, VendorID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal PurchasesReportID As Integer, ByVal VendorID As Integer)
            Dim row As PurchasesReportVendorTotalAmountRow

            row = New PurchasesReportVendorTotalAmountRow(DB, PurchasesReportID, VendorID)
            row.Remove()
        End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from PurchasesReportVendorTotalAmount"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods

	End Class
	
	Public MustInherit Class PurchasesReportVendorTotalAmountRowBase
		Private m_DB as Database
		Private m_PurchasesReportID As Integer = nothing
		Private m_VendorID As Integer = nothing
		Private m_TotalAmount As Double = nothing
		Private m_Created As DateTime = nothing
        Private m_CreatorBuilderAccountID As Integer = Nothing
        Private m_Modified As DateTime = Nothing
        Private m_ModifyBuilderAccountID As Integer = Nothing
        Private m_BuilderReportedInitialTotal As Double = Nothing
	
	
		Public Property PurchasesReportID As Integer
			Get
				Return m_PurchasesReportID
			End Get
			Set(ByVal Value As Integer)
				m_PurchasesReportID = value
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
	
		Public Property TotalAmount As Double
			Get
				Return m_TotalAmount
			End Get
			Set(ByVal Value As Double)
				m_TotalAmount = value
			End Set
        End Property
        Public Property BuilderReportedInitialTotal() As Double
            Get
                Return m_BuilderReportedInitialTotal
            End Get
            Set(ByVal Value As Double)
                m_BuilderReportedInitialTotal = Value
            End Set
        End Property
	
        Public ReadOnly Property Created() As DateTime
            Get
                Return m_Created
            End Get
        End Property
	
		Public Property CreatorBuilderAccountID As Integer
			Get
				Return m_CreatorBuilderAccountID
			End Get
			Set(ByVal Value As Integer)
				m_CreatorBuilderAccountID = value
			End Set
        End Property
        Public Property Modified() As DateTime
            Get
                Return m_Modified
            End Get
            Set(ByVal value As DateTime)
                m_Modified = value
            End Set
        End Property

        Public Property ModifyBuilderAccountID() As Integer
            Get
                Return m_ModifyBuilderAccountID
            End Get
            Set(ByVal Value As Integer)
                m_ModifyBuilderAccountID = Value
            End Set
        End Property

        Public Property DB() As Database
            Get
                DB = m_DB
            End Get
            Set(ByVal Value As Database)
                m_DB = Value
            End Set
        End Property
	
		Public Sub New()
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			m_DB = DB
		End Sub 'New
	
        Public Sub New(ByVal DB As Database, ByVal PurchasesReportID As Integer, ByVal VendorID As Integer)
            m_DB = DB
            m_PurchasesReportID = PurchasesReportID
            m_VendorID = VendorID
        End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
            SQL = "SELECT * FROM PurchasesReportVendorTotalAmount WHERE PurchasesReportID = " & DB.Number(PurchasesReportID) & " and VendorID=" & DB.Number(VendorID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_PurchasesReportID = Convert.ToInt32(r.Item("PurchasesReportID"))
			m_VendorID = Convert.ToInt32(r.Item("VendorID"))
			m_TotalAmount = Convert.ToDouble(r.Item("TotalAmount"))
			m_Created = Convert.ToDateTime(r.Item("Created"))
            m_CreatorBuilderAccountID = Convert.ToInt32(r.Item("CreatorBuilderAccountID"))
            If IsDBNull(r.Item("ModifyBuilderAccountId")) Then
                m_ModifyBuilderAccountID = Nothing
            Else
                m_ModifyBuilderAccountID = Convert.ToInt32(r.Item("ModifyBuilderAccountID"))
            End If
            If IsDBNull(r.Item("Modified")) Then
                m_Modified = Nothing
            Else
                m_Modified = Convert.ToDateTime(r.Item("Modified"))
            End If
            If IsDBNull(r.Item("BuilderReportedInitialTotal")) Then
                m_BuilderReportedInitialTotal = Nothing
            Else
                m_BuilderReportedInitialTotal = Convert.ToDouble(r.Item("BuilderReportedInitialTotal"))
            End If
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String

            SQL = " INSERT INTO PurchasesReportVendorTotalAmount (" _
             & " PurchasesReportID" _
             & ",VendorID" _
             & ",TotalAmount" _
             & ",Created" _
             & ",CreatorBuilderAccountID" _
             & ",BuilderReportedInitialTotal" _
             & ") VALUES (" _
             & m_DB.NullNumber(PurchasesReportID) _
             & "," & m_DB.NullNumber(VendorID) _
             & "," & m_DB.Number(TotalAmount) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(CreatorBuilderAccountID) _
             & "," & m_DB.Number(BuilderReportedInitialTotal) _
             & ")"

            m_DB.ExecuteSQL(SQL)
			
			Return PurchasesReportID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
            SQL = " UPDATE PurchasesReportVendorTotalAmount SET " _
             & " TotalAmount = " & m_DB.Number(TotalAmount) _
             & " ,Modified = " & m_DB.Quote(Modified) _
             & " ,ModifyBuilderAccountId = " & m_DB.Number(ModifyBuilderAccountID) _
             & " ,BuilderReportedInitialTotal = " & m_DB.Number(BuilderReportedInitialTotal) _
             & " WHERE PurchasesReportID = " & m_DB.Quote(PurchasesReportID) _
             & " AND VendorID = " & m_DB.Quote(VendorID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
            SQL = "DELETE FROM PurchasesReportVendorTotalAmount WHERE PurchasesReportID = " & m_DB.Number(PurchasesReportID) & " and VendorID=" & DB.Number(VendorID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class PurchasesReportVendorTotalAmountCollection
		Inherits GenericCollection(Of PurchasesReportVendorTotalAmountRow)
	End Class

End Namespace

