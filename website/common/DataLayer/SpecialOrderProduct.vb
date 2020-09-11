Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class SpecialOrderProductRow
		Inherits SpecialOrderProductRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, SpecialOrderProductID as Integer)
			MyBase.New(DB, SpecialOrderProductID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal SpecialOrderProductID As Integer) As SpecialOrderProductRow
			Dim row as SpecialOrderProductRow 
			
			row = New SpecialOrderProductRow(DB, SpecialOrderProductID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal SpecialOrderProductID As Integer)
			Dim row as SpecialOrderProductRow 
			
			row = New SpecialOrderProductRow(DB, SpecialOrderProductID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from SpecialOrderProduct"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods

	End Class
	
	Public MustInherit Class SpecialOrderProductRowBase
		Private m_DB as Database
		Private m_SpecialOrderProductID As Integer = nothing
		Private m_BuilderID As Integer = nothing
		Private m_SpecialOrderProduct As String = nothing
		Private m_Description As String = nothing
		Private m_UnitOfMeasureID As Integer = nothing
		Private m_Created As DateTime = nothing
	
	
		Public Property SpecialOrderProductID As Integer
			Get
				Return m_SpecialOrderProductID
			End Get
			Set(ByVal Value As Integer)
				m_SpecialOrderProductID = value
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
	
		Public Property SpecialOrderProduct As String
			Get
				Return m_SpecialOrderProduct
			End Get
			Set(ByVal Value As String)
				m_SpecialOrderProduct = value
			End Set
		End Property
	
		Public Property Description As String
			Get
				Return m_Description
			End Get
			Set(ByVal Value As String)
				m_Description = value
			End Set
		End Property
	
		Public Property UnitOfMeasureID As Integer
			Get
				Return m_UnitOfMeasureID
			End Get
			Set(ByVal Value As Integer)
				m_UnitOfMeasureID = value
			End Set
		End Property
	
        Public ReadOnly Property Created() As DateTime
            Get
                Return m_Created
            End Get
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
	
		Public Sub New(ByVal DB As Database, SpecialOrderProductID as Integer)
			m_DB = DB
			m_SpecialOrderProductID = SpecialOrderProductID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM SpecialOrderProduct WHERE SpecialOrderProductID = " & DB.Number(SpecialOrderProductID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_SpecialOrderProductID = Convert.ToInt32(r.Item("SpecialOrderProductID"))
			m_BuilderID = Convert.ToInt32(r.Item("BuilderID"))
			m_SpecialOrderProduct = Convert.ToString(r.Item("SpecialOrderProduct"))
			m_Description = Convert.ToString(r.Item("Description"))
			m_UnitOfMeasureID = Convert.ToInt32(r.Item("UnitOfMeasureID"))
			m_Created = Convert.ToDateTime(r.Item("Created"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
            SQL = " INSERT INTO SpecialOrderProduct (" _
             & " BuilderID" _
             & ",SpecialOrderProduct" _
             & ",Description" _
             & ",UnitOfMeasureID" _
             & ",Created" _
             & ") VALUES (" _
             & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.Quote(SpecialOrderProduct) _
             & "," & m_DB.Quote(Description) _
             & "," & m_DB.NullNumber(UnitOfMeasureID) _
             & "," & m_DB.NullQuote(Now) _
             & ")"

			SpecialOrderProductID = m_DB.InsertSQL(SQL)
			
			Return SpecialOrderProductID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
            SQL = " UPDATE SpecialOrderProduct SET " _
             & " BuilderID = " & m_DB.NullNumber(BuilderID) _
             & ",SpecialOrderProduct = " & m_DB.Quote(SpecialOrderProduct) _
             & ",Description = " & m_DB.Quote(Description) _
             & ",UnitOfMeasureID = " & m_DB.NullNumber(UnitOfMeasureID) _
             & " WHERE SpecialOrderProductID = " & m_DB.Quote(SpecialOrderProductID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM SpecialOrderProduct WHERE SpecialOrderProductID = " & m_DB.Number(SpecialOrderProductID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class SpecialOrderProductCollection
		Inherits GenericCollection(Of SpecialOrderProductRow)
	End Class

End Namespace

