Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class BuilderRegistrationReferenceRow
		Inherits BuilderRegistrationReferenceRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, BuilderRegistrationReferenceID as Integer)
			MyBase.New(DB, BuilderRegistrationReferenceID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal BuilderRegistrationReferenceID As Integer) As BuilderRegistrationReferenceRow
			Dim row as BuilderRegistrationReferenceRow 
			
			row = New BuilderRegistrationReferenceRow(DB, BuilderRegistrationReferenceID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal BuilderRegistrationReferenceID As Integer)
			Dim row as BuilderRegistrationReferenceRow 
			
			row = New BuilderRegistrationReferenceRow(DB, BuilderRegistrationReferenceID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from BuilderRegistrationReference"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

        'Custom Methods

        Public Shared Function GetListByRegistration(ByVal DB As Database, ByVal BuilderRegistrationID As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from BuilderRegistrationReference Where BuilderRegistrationID = " & DB.Number(BuilderRegistrationID)
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

	End Class
	
	Public MustInherit Class BuilderRegistrationReferenceRowBase
		Private m_DB as Database
		Private m_BuilderRegistrationReferenceID As Integer = nothing
		Private m_BuilderRegistrationID As Integer = nothing
		Private m_ContactFirstName As String = nothing
		Private m_ContactLastName As String = nothing
		Private m_Company As String = nothing
		Private m_Phone As String = nothing
	
	
		Public Property BuilderRegistrationReferenceID As Integer
			Get
				Return m_BuilderRegistrationReferenceID
			End Get
			Set(ByVal Value As Integer)
				m_BuilderRegistrationReferenceID = value
			End Set
		End Property
	
		Public Property BuilderRegistrationID As Integer
			Get
				Return m_BuilderRegistrationID
			End Get
			Set(ByVal Value As Integer)
				m_BuilderRegistrationID = value
			End Set
		End Property
	
		Public Property ContactFirstName As String
			Get
				Return m_ContactFirstName
			End Get
			Set(ByVal Value As String)
				m_ContactFirstName = value
			End Set
		End Property
	
		Public Property ContactLastName As String
			Get
				Return m_ContactLastName
			End Get
			Set(ByVal Value As String)
				m_ContactLastName = value
			End Set
		End Property
	
		Public Property Company As String
			Get
				Return m_Company
			End Get
			Set(ByVal Value As String)
				m_Company = value
			End Set
		End Property
	
		Public Property Phone As String
			Get
				Return m_Phone
			End Get
			Set(ByVal Value As String)
				m_Phone = value
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
	
		Public Sub New(ByVal DB As Database, BuilderRegistrationReferenceID as Integer)
			m_DB = DB
			m_BuilderRegistrationReferenceID = BuilderRegistrationReferenceID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM BuilderRegistrationReference WHERE BuilderRegistrationReferenceID = " & DB.Number(BuilderRegistrationReferenceID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_BuilderRegistrationReferenceID = Convert.ToInt32(r.Item("BuilderRegistrationReferenceID"))
			m_BuilderRegistrationID = Convert.ToInt32(r.Item("BuilderRegistrationID"))
			m_ContactFirstName = Convert.ToString(r.Item("ContactFirstName"))
			m_ContactLastName = Convert.ToString(r.Item("ContactLastName"))
			m_Company = Convert.ToString(r.Item("Company"))
			m_Phone = Convert.ToString(r.Item("Phone"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
			SQL = " INSERT INTO BuilderRegistrationReference (" _
				& " BuilderRegistrationID" _
				& ",ContactFirstName" _
				& ",ContactLastName" _
				& ",Company" _
				& ",Phone" _
				& ") VALUES (" _
				& m_DB.NullNumber(BuilderRegistrationID) _
				& "," & m_DB.Quote(ContactFirstName) _
				& "," & m_DB.Quote(ContactLastName) _
				& "," & m_DB.Quote(Company) _
				& "," & m_DB.Quote(Phone) _
				& ")"

			BuilderRegistrationReferenceID = m_DB.InsertSQL(SQL)
			
			Return BuilderRegistrationReferenceID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
			SQL = " UPDATE BuilderRegistrationReference SET " _
				& " BuilderRegistrationID = " & m_DB.NullNumber(BuilderRegistrationID) _
				& ",ContactFirstName = " & m_DB.Quote(ContactFirstName) _
				& ",ContactLastName = " & m_DB.Quote(ContactLastName) _
				& ",Company = " & m_DB.Quote(Company) _
				& ",Phone = " & m_DB.Quote(Phone) _
				& " WHERE BuilderRegistrationReferenceID = " & m_DB.quote(BuilderRegistrationReferenceID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM BuilderRegistrationReference WHERE BuilderRegistrationReferenceID = " & m_DB.Number(BuilderRegistrationReferenceID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class BuilderRegistrationReferenceCollection
		Inherits GenericCollection(Of BuilderRegistrationReferenceRow)
	End Class

End Namespace

