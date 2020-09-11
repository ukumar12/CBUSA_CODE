Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class UnitOfMeasureRow
		Inherits UnitOfMeasureRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, UnitOfMeasureID as Integer)
			MyBase.New(DB, UnitOfMeasureID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal UnitOfMeasureID As Integer) As UnitOfMeasureRow
			Dim row as UnitOfMeasureRow 
			
			row = New UnitOfMeasureRow(DB, UnitOfMeasureID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal UnitOfMeasureID As Integer)
			Dim row as UnitOfMeasureRow 
			
			row = New UnitOfMeasureRow(DB, UnitOfMeasureID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from UnitOfMeasure"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

        'Custom Methods
        Public Shared Function GetUnitOfMesaureList(ByVal DB As Database) As UnitOfMeasureCollection
            Dim uomCollection As UnitOfMeasureCollection = New UnitOfMeasureCollection()
            Dim dt As DataTable = DB.GetDataTable("select UnitOfMeasureId from  UnitOfMeasure")
            For Each row As DataRow In dt.Rows
                Dim dbUnitOfMeasureRow As UnitOfMeasureRow = GetRow(DB, row("UnitofMeasureId"))
                uomCollection.Add(dbUnitOfMeasureRow)
            Next
            Return uomCollection
        End Function

	End Class
	
	Public MustInherit Class UnitOfMeasureRowBase
		Private m_DB as Database
		Private m_UnitOfMeasureID As Integer = nothing
		Private m_UnitOfMeasure As String = nothing
	
	
		Public Property UnitOfMeasureID As Integer
			Get
				Return m_UnitOfMeasureID
			End Get
			Set(ByVal Value As Integer)
				m_UnitOfMeasureID = value
			End Set
		End Property
	
		Public Property UnitOfMeasure As String
			Get
				Return m_UnitOfMeasure
			End Get
			Set(ByVal Value As String)
				m_UnitOfMeasure = value
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
	
		Public Sub New(ByVal DB As Database, UnitOfMeasureID as Integer)
			m_DB = DB
			m_UnitOfMeasureID = UnitOfMeasureID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM UnitOfMeasure WHERE UnitOfMeasureID = " & DB.Number(UnitOfMeasureID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_UnitOfMeasureID = Convert.ToInt32(r.Item("UnitOfMeasureID"))
			m_UnitOfMeasure = Convert.ToString(r.Item("UnitOfMeasure"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
			SQL = " INSERT INTO UnitOfMeasure (" _
				& " UnitOfMeasure" _
				& ") VALUES (" _
				& m_DB.Quote(UnitOfMeasure) _
				& ")"

			UnitOfMeasureID = m_DB.InsertSQL(SQL)
			
			Return UnitOfMeasureID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
			SQL = " UPDATE UnitOfMeasure SET " _
				& " UnitOfMeasure = " & m_DB.Quote(UnitOfMeasure) _
				& " WHERE UnitOfMeasureID = " & m_DB.quote(UnitOfMeasureID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM UnitOfMeasure WHERE UnitOfMeasureID = " & m_DB.Number(UnitOfMeasureID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class UnitOfMeasureCollection
		Inherits GenericCollection(Of UnitOfMeasureRow)
	End Class

End Namespace

