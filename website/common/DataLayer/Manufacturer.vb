Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class ManufacturerRow
		Inherits ManufacturerRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, ManufacturerID as Integer)
			MyBase.New(DB, ManufacturerID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal ManufacturerID As Integer) As ManufacturerRow
			Dim row as ManufacturerRow 
			
			row = New ManufacturerRow(DB, ManufacturerID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal ManufacturerID As Integer)
			Dim row as ManufacturerRow 
			
			row = New ManufacturerRow(DB, ManufacturerID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from Manufacturer"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods
        Public Shared Function GetManufacturerByName(ByVal DB As Database, ByVal Name As String) As ManufacturerRow
            Dim out As New ManufacturerRow(DB)
            Dim sdr As SqlDataReader = DB.GetReader("select * from Manufacturer where Manufacturer=" & DB.Quote(Name))
            If sdr.read() Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function
	End Class
	
	Public MustInherit Class ManufacturerRowBase
		Private m_DB as Database
		Private m_ManufacturerID As Integer = nothing
		Private m_Manufacturer As String = nothing
	
	
		Public Property ManufacturerID As Integer
			Get
				Return m_ManufacturerID
			End Get
			Set(ByVal Value As Integer)
				m_ManufacturerID = value
			End Set
		End Property
	
		Public Property Manufacturer As String
			Get
				Return m_Manufacturer
			End Get
			Set(ByVal Value As String)
				m_Manufacturer = value
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
	
		Public Sub New(ByVal DB As Database, ManufacturerID as Integer)
			m_DB = DB
			m_ManufacturerID = ManufacturerID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM Manufacturer WHERE ManufacturerID = " & DB.Number(ManufacturerID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_ManufacturerID = Convert.ToInt32(r.Item("ManufacturerID"))
			m_Manufacturer = Convert.ToString(r.Item("Manufacturer"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
			SQL = " INSERT INTO Manufacturer (" _
				& " Manufacturer" _
				& ") VALUES (" _
				& m_DB.Quote(Manufacturer) _
				& ")"

			ManufacturerID = m_DB.InsertSQL(SQL)
			
			Return ManufacturerID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
			SQL = " UPDATE Manufacturer SET " _
				& " Manufacturer = " & m_DB.Quote(Manufacturer) _
				& " WHERE ManufacturerID = " & m_DB.quote(ManufacturerID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM Manufacturer WHERE ManufacturerID = " & m_DB.Number(ManufacturerID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class ManufacturerCollection
		Inherits GenericCollection(Of ManufacturerRow)
	End Class

End Namespace

