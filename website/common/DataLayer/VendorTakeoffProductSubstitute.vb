Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class VendorTakeOffProductSubstituteRow
		Inherits VendorTakeOffProductSubstituteRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
        Public Sub New(ByVal DB As Database, ByVal VendorID As Integer, ByVal TakeoffProductID As Integer)
            MyBase.New(DB, VendorID, TakeoffProductID)
        End Sub 'New
		
		'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorID As Integer, ByVal TakeoffProductID As Integer) As VendorTakeOffProductSubstituteRow
            Dim row As VendorTakeOffProductSubstituteRow

            row = New VendorTakeOffProductSubstituteRow(DB, VendorID, TakeoffProductID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorID As Integer, ByVal TakeoffProductID As Integer)
            Dim row As VendorTakeOffProductSubstituteRow

            row = New VendorTakeOffProductSubstituteRow(DB, VendorID, TakeoffProductID)
            row.Remove()
        End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from VendorTakeOffProductSubstitute"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods

	End Class
	
	Public MustInherit Class VendorTakeOffProductSubstituteRowBase
		Private m_DB as Database
		Private m_VendorID As Integer = nothing
		Private m_TakeOffProductID As Integer = nothing
		Private m_SubstituteProductID As Integer = nothing
		Private m_RecommendedQuantity As Integer = nothing
		Private m_Created As DateTime = nothing
		Private m_CreatorVendorAccountID As Integer = nothing
	
	
		Public Property VendorID As Integer
			Get
				Return m_VendorID
			End Get
			Set(ByVal Value As Integer)
				m_VendorID = value
			End Set
		End Property
	
		Public Property TakeOffProductID As Integer
			Get
				Return m_TakeOffProductID
			End Get
			Set(ByVal Value As Integer)
				m_TakeOffProductID = value
			End Set
		End Property
	
		Public Property SubstituteProductID As Integer
			Get
				Return m_SubstituteProductID
			End Get
			Set(ByVal Value As Integer)
				m_SubstituteProductID = value
			End Set
		End Property
	
		Public Property RecommendedQuantity As Integer
			Get
				Return m_RecommendedQuantity
			End Get
			Set(ByVal Value As Integer)
				m_RecommendedQuantity = value
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
	
        Public Sub New(ByVal DB As Database, ByVal VendorID As Integer, ByVal TakeoffProductID As Integer)
            m_DB = DB
            m_VendorID = VendorID
            m_TakeOffProductID = TakeoffProductID
        End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
            SQL = "SELECT * FROM VendorTakeOffProductSubstitute WHERE VendorID = " & DB.Number(VendorID) & " and TakeoffProductID=" & DB.Number(TakeOffProductID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_VendorID = Core.GetInt(r.Item("VendorID"))
			m_TakeOffProductID = Core.GetInt(r.Item("TakeOffProductID"))
			m_SubstituteProductID = Core.GetInt(r.Item("SubstituteProductID"))
			m_RecommendedQuantity = Core.GetInt(r.Item("RecommendedQuantity"))
			m_Created = Core.GetDate(r.Item("Created"))
			m_CreatorVendorAccountID = Core.GetInt(r.Item("CreatorVendorAccountID"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
            SQL = " INSERT INTO VendorTakeOffProductSubstitute (" _
             & " VendorID" _
             & ",TakeOffProductID" _
             & ",SubstituteProductID" _
             & ",RecommendedQuantity" _
             & ",Created" _
             & ",CreatorVendorAccountID" _
             & ") VALUES (" _
             & m_DB.NullNumber(VendorID) _
             & "," & m_DB.NullNumber(TakeOffProductID) _
             & "," & m_DB.NullNumber(SubstituteProductID) _
             & "," & m_DB.Number(RecommendedQuantity) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(CreatorVendorAccountID) _
             & ")"

            m_DB.ExecuteSQL(SQL)
			
			Return VendorID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
            SQL = " UPDATE VendorTakeOffProductSubstitute SET " _
             & " SubstituteProductID = " & m_DB.NullNumber(SubstituteProductID) _
             & ",RecommendedQuantity = " & m_DB.Number(RecommendedQuantity) _
             & " WHERE VendorID = " & m_DB.Quote(VendorID) _
             & " AND TakeoffProductID = " & m_DB.Quote(TakeOffProductID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
            SQL = "DELETE FROM VendorTakeOffProductSubstitute WHERE VendorID = " & m_DB.Number(VendorID) & " and TakeoffProductID = " & m_DB.Number(TakeOffProductID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class VendorTakeOffProductSubstituteCollection
		Inherits GenericCollection(Of VendorTakeOffProductSubstituteRow)
	End Class

End Namespace

