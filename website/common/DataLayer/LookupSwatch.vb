Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

	Public Class LookupSwatchRow
		Inherits LookupSwatchRowBase

		Public Sub New()
			MyBase.New()
		End Sub	'New

		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub	'New

		Public Sub New(ByVal DB As Database, ByVal SwatchId As Integer)
			MyBase.New(DB, SwatchId)
		End Sub	'New

		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB As Database, ByVal SwatchId As Integer) As LookupSwatchRow
			Dim row As LookupSwatchRow

			row = New LookupSwatchRow(DB, SwatchId)
			row.Load()

			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB As Database, ByVal SwatchId As Integer)
			Dim row As LookupSwatchRow

			row = New LookupSwatchRow(DB, SwatchId)
			row.Remove()
		End Sub

		'Custom Methods

	End Class

	Public MustInherit Class LookupSwatchRowBase
		Private m_DB As Database
		Private m_SwatchId As Integer = Nothing
		Private m_Name As String = Nothing
		Private m_SKU As String = Nothing
		Private m_Price As Double = Nothing
		Private m_Weight As Double = Nothing
		Private m_Image As String = Nothing


		Public Property SwatchId() As Integer
			Get
				Return m_SwatchId
			End Get
			Set(ByVal Value As Integer)
				m_SwatchId = value
			End Set
		End Property

		Public Property Name() As String
			Get
				Return m_Name
			End Get
			Set(ByVal Value As String)
				m_Name = value
			End Set
		End Property

		Public Property SKU() As String
			Get
				Return m_SKU
			End Get
			Set(ByVal Value As String)
				m_SKU = value
			End Set
		End Property

		Public Property Price() As Double
			Get
				Return m_Price
			End Get
			Set(ByVal Value As Double)
				m_Price = value
			End Set
		End Property

		Public Property Weight() As Double
			Get
				Return m_Weight
			End Get
			Set(ByVal Value As Double)
				m_Weight = value
			End Set
		End Property

		Public Property Image() As String
			Get
				Return m_Image
			End Get
			Set(ByVal Value As String)
				m_Image = value
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
		End Sub	'New

		Public Sub New(ByVal DB As Database)
			m_DB = DB
		End Sub	'New

		Public Sub New(ByVal DB As Database, ByVal SwatchId As Integer)
			m_DB = DB
			m_SwatchId = SwatchId
		End Sub	'New

		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String

			SQL = "SELECT * FROM LookupSwatch WHERE SwatchId = " & DB.Number(SwatchId)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub


		Protected Overridable Sub Load(ByVal r As sqlDataReader)
			m_SwatchId = Convert.ToInt32(r.Item("SwatchId"))
			m_Name = Convert.ToString(r.Item("Name"))
			If IsDBNull(r.Item("SKU")) Then
				m_SKU = Nothing
			Else
				m_SKU = Convert.ToString(r.Item("SKU"))
			End If
			If IsDBNull(r.Item("Price")) Then
				m_Price = Nothing
			Else
				m_Price = Convert.ToDouble(r.Item("Price"))
			End If
			If IsDBNull(r.Item("Weight")) Then
				m_Weight = Nothing
			Else
				m_Weight = Convert.ToDouble(r.Item("Weight"))
			End If
			m_Image = Convert.ToString(r.Item("Image"))
		End Sub	'Load

		Public Overridable Function Insert() As Integer
			Dim SQL As String


			SQL = " INSERT INTO LookupSwatch (" _
			 & " Name" _
			 & ",SKU" _
			 & ",Price" _
			 & ",Weight" _
			 & ",Image" _
			 & ") VALUES (" _
			 & m_DB.Quote(Name) _
			 & "," & m_DB.Quote(SKU) _
			 & "," & m_DB.Number(Price) _
			 & "," & m_DB.Number(Weight) _
			 & "," & m_DB.Quote(Image) _
			 & ")"

			SwatchId = m_DB.InsertSQL(SQL)

			Return SwatchId
		End Function

		Public Overridable Sub Update()
			Dim SQL As String

			SQL = " UPDATE LookupSwatch SET " _
			 & " Name = " & m_DB.Quote(Name) _
			 & ",SKU = " & m_DB.Quote(SKU) _
			 & ",Price = " & m_DB.Number(Price) _
			 & ",Weight = " & m_DB.Number(Weight) _
			 & ",Image = " & m_DB.Quote(Image) _
			 & " WHERE SwatchId = " & m_DB.quote(SwatchId)

			m_DB.ExecuteSQL(SQL)

		End Sub	'Update

		Public Sub Remove()
			Dim SQL As String

			SQL = "DELETE FROM LookupSwatch WHERE SwatchId = " & m_DB.Number(SwatchId)
			m_DB.ExecuteSQL(SQL)
		End Sub	'Remove
	End Class

	Public Class LookupSwatchCollection
		Inherits GenericCollection(Of LookupSwatchRow)
	End Class

End Namespace


