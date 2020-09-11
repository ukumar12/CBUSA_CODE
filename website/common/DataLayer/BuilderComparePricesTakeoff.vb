Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class BuilderComparePricesTakeOffRow
		Inherits BuilderComparePricesTakeOffRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, BuilderID as Integer)
			MyBase.New(DB, BuilderID)
		End Sub 'New
		
		Public Sub New(ByVal DB As Database, BuilderID as Integer, SortOrder As Integer)
			MyBase.New(DB, BuilderID, SortOrder)
		End Sub 'New

        'Shared function to get one row by builder and sort order
        Public Shared Function GetRowByBuilder(ByVal DB As Database, ByVal BuilderID As Integer, ByVal SortIndex As Integer) As BuilderComparePricesTakeOffRow
            Dim row As BuilderComparePricesTakeOffRow

            row = New BuilderComparePricesTakeOffRow(DB, BuilderID, SortIndex)
            row.LoadByBuilderIndex()

            Return row
        End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal BuilderID As Integer)
			Dim row as BuilderComparePricesTakeOffRow 
			
			row = New BuilderComparePricesTakeOffRow(DB, BuilderID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from BuilderComparePricesTakeOff"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods
        Public Shared Sub DeleteByBuilder(ByVal DB As Database, ByVal BuilderID As Integer)
            Dim SQL As String = "DELETE FROM BuilderComparePricesTakeOff WHERE BuilderID = " & BuilderID
            DB.ExecuteSQL(SQL)
        End Sub

	End Class
	
	Public MustInherit Class BuilderComparePricesTakeOffRowBase
		Private m_DB as Database
		Private m_BuilderID As Integer = nothing
		Private m_TakeOffID As Integer = nothing
		Private m_SortOrder As Integer = nothing
	
	
		Public Property BuilderID As Integer
			Get
				Return m_BuilderID
			End Get
			Set(ByVal Value As Integer)
				m_BuilderID = value
			End Set
		End Property
	
		Public Property TakeOffID As Integer
			Get
				Return m_TakeOffID
			End Get
			Set(ByVal Value As Integer)
				m_TakeOffID = value
			End Set
		End Property
	
		Public Property SortOrder As Integer
			Get
				Return m_SortOrder
			End Get
			Set(ByVal Value As Integer)
				m_SortOrder = value
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
	
		Public Sub New(ByVal DB As Database, BuilderID as Integer)
			m_DB = DB
			m_BuilderID = BuilderID
		End Sub 'New
		
        Public Sub New(ByVal DB As Database, ByVal BuilderID As Integer, ByVal SortIndex As Integer)
            m_DB = DB
            m_BuilderID = BuilderID
            m_SortOrder = SortIndex
        End Sub 'New

		'Protected Overridable Sub Load()
		'	Dim r As SqlDataReader
		'	Dim SQL As String

		'	SQL = "SELECT * FROM BuilderComparePricesVendor WHERE BuilderID = " & DB.Number(BuilderID)
		'	r = m_DB.GetReader(SQL)
		'	If r.Read Then
		'		Me.Load(r)
		'	End If
		'	r.Close()
		'End Sub
	
        Protected Sub LoadByBuilderIndex()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT TOP 1 * FROM BuilderComparePricesTakeOff WHERE BuilderID = " & DB.Number(BuilderID) & " AND SortOrder = " & DB.Number(SortOrder)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_BuilderID = Core.GetInt(r.Item("BuilderID"))
			m_TakeOffID = Core.GetInt(r.Item("TakeOffID"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
			Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from BuilderComparePricesTakeOff order by SortOrder desc")
			MaxSortOrder += 1
	
			SQL = " INSERT INTO BuilderComparePricesTakeOff (" _
				& " TakeOffID" _
				& ",BuilderID" _
				& ",SortOrder" _
				& ") VALUES (" _
				& m_DB.NullNumber(TakeOffID) _
				& "," & m_DB.NullNumber(BuilderID ) _
				& "," & MaxSortOrder _
				& ")"

			BuilderID = m_DB.ExecuteSQL(SQL)
			
			Return BuilderID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
			SQL = " UPDATE BuilderComparePricesTakeOff SET " _
				& " TakeOffID = " & m_DB.NullNumber(TakeOffID) _
				& " WHERE BuilderID = " & m_DB.quote(BuilderID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM BuilderComparePricesTakeOff WHERE BuilderID = " & m_DB.Number(BuilderID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class BuilderComparePricesTakeOffCollection
		Inherits GenericCollection(Of BuilderComparePricesTakeOffRow)
	End Class

End Namespace

