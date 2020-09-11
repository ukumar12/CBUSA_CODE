Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

	Public Class StoreItemNotifyRow
		Inherits StoreItemNotifyRowBase

		Public Sub New()
			MyBase.New()
		End Sub	'New

		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub	'New

		Public Sub New(ByVal DB As Database, ByVal ItemNotifyId As Integer)
			MyBase.New(DB, ItemNotifyId)
		End Sub	'New

		Public Sub New(ByVal DB As Database, ByVal Id As Integer, ByVal Email As String, Optional ByVal IsAttribute As Boolean = False)
			MyBase.New(DB, Id, Email, IsAttribute)
		End Sub	'New

		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB As Database, ByVal ItemNotifyId As Integer) As StoreItemNotifyRow
			Dim row As StoreItemNotifyRow

			row = New StoreItemNotifyRow(DB, ItemNotifyId)
			row.Load()

			Return row
		End Function

		Public Shared Function GetRow(ByVal DB As Database, ByVal Id As Integer, ByVal Email As String, Optional ByVal IsAttribute As Boolean = False) As StoreItemNotifyRow
			Dim row As StoreItemNotifyRow

			row = New StoreItemNotifyRow(DB, Id, Email, IsAttribute)
			row.Load()

			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ItemNotifyId As Integer)
			Dim row As StoreItemNotifyRow

			row = New StoreItemNotifyRow(DB, ItemNotifyId)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
			Dim SQL As String = "select * from StoreItemNotify"
			If Not SortBy = String.Empty Then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End If
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods
		Public Function GetItemAttributeCollection(ByVal ItemId As Integer, ByVal TemplateId As Integer) As ItemAttributeCollection
			Dim col As New ItemAttributeCollection
			Dim dv As DataView = DB.GetDataTable("sp_GetAttributeTreeTableLayout " & ItemId & ", " & TemplateId).DefaultView
			dv.RowFilter = "ItemAttributeId = " & ItemAttributeId
			If dv.Count > 0 Then
				Dim SQL = "select sia.*, sita.AttributeType, sita.AttributeName from StoreItemTemplateAttribute sita INNER JOIN StoreItemAttribute sia ON sita.TemplateAttributeId = sia.TemplateAttributeId WHERE sia.ItemAttributeId IN " & DB.NumberMultiple(dv(0)("ItemAttributeIds"))
				Dim dt As DataTable = DB.GetDataTable(SQL)
				For Each r As DataRow In dt.Rows
					Dim attr As New ItemAttribute
					attr.AttributeType = IIf(IsDBNull(r("AttributeType")), String.Empty, r("AttributeType"))
					attr.AttributeName = IIf(IsDBNull(r("AttributeName")), String.Empty, r("AttributeName"))
					attr.AttributeValue = IIf(IsDBNull(r("AttributeValue")), String.Empty, r("AttributeValue"))
					attr.ImageName = IIf(IsDBNull(r("ImageName")), String.Empty, r("ImageName"))
					attr.ImageAlt = IIf(IsDBNull(r("ImageAlt")), String.Empty, r("ImageAlt"))
					attr.ParentAttributeId = IIf(IsDBNull(r("ParentAttributeId")), Nothing, r("ParentAttributeId"))
					attr.ProductAlt = IIf(IsDBNull(r("ProductAlt")), String.Empty, r("ProductAlt"))
					attr.ProductImage = IIf(IsDBNull(r("ProductImage")), String.Empty, r("ProductImage"))
					attr.Weight = IIf(IsDBNull(r("Weight")), 0, r("Weight"))
					attr.ItemAttributeId = IIf(IsDBNull(r("ItemAttributeId")), 0, r("ItemAttributeId"))
					attr.ItemId = IIf(IsDBNull(r("ItemId")), 0, r("ItemId"))
					attr.Price = IIf(IsDBNull(r("Price")), 0, r("Price"))
					attr.SKU = IIf(IsDBNull(r("SKU")), String.Empty, r("SKU"))
					attr.SortOrder = IIf(IsDBNull(r("SortOrder")), 0, r("SortOrder"))
					attr.TemplateAttributeId = IIf(IsDBNull(r("TemplateAttributeId")), 0, r("TemplateAttributeId"))
					col.Add(attr)
				Next
			End If
			Return col
		End Function

	End Class

	Public MustInherit Class StoreItemNotifyRowBase
		Private m_DB As Database
		Private m_ItemNotifyId As Integer = Nothing
		Private m_Email As String = Nothing
		Private m_ItemId As Integer = Nothing
		Private m_ItemAttributeId As Integer = Nothing
		Private m_CreateDate As DateTime = Nothing
		Private m_SendDate As DateTime = Nothing
		Private m_ViewDate As DateTime = Nothing


		Public Property ItemNotifyId() As Integer
			Get
				Return m_ItemNotifyId
			End Get
			Set(ByVal Value As Integer)
				m_ItemNotifyId = value
			End Set
		End Property

		Public Property Email() As String
			Get
				Return m_Email
			End Get
			Set(ByVal Value As String)
				m_Email = value
			End Set
		End Property

		Public Property ItemId() As Integer
			Get
				Return m_ItemId
			End Get
			Set(ByVal Value As Integer)
				m_ItemId = value
			End Set
		End Property

		Public Property ItemAttributeId() As Integer
			Get
				Return m_ItemAttributeId
			End Get
			Set(ByVal Value As Integer)
				m_ItemAttributeId = value
			End Set
		End Property

		Public Property SendDate() As DateTime
			Get
				Return m_SendDate
			End Get
			Set(ByVal Value As DateTime)
				m_SendDate = value
			End Set
		End Property

		Public Property ViewDate() As DateTime
			Get
				Return m_ViewDate
			End Get
			Set(ByVal Value As DateTime)
				m_ViewDate = value
			End Set
		End Property

		Public ReadOnly Property CreateDate() As DateTime
			Get
				Return m_CreateDate
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
		End Sub	'New

		Public Sub New(ByVal DB As Database)
			m_DB = DB
		End Sub	'New

		Public Sub New(ByVal DB As Database, ByVal ItemNotifyId As Integer)
			m_DB = DB
			m_ItemNotifyId = ItemNotifyId
		End Sub	'New

		Public Sub New(ByVal DB As Database, ByVal Id As Integer, ByVal Email As String, Optional ByVal IsAttribute As Boolean = False)
			m_DB = DB
			m_Email = Email
			If IsAttribute Then m_ItemAttributeId = Id Else m_ItemId = Id
		End Sub	'New

		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String

			SQL = "SELECT * FROM StoreItemNotify WHERE " & IIf(Not ItemNotifyId = Nothing, "ItemNotifyId = " & DB.Number(ItemNotifyId), IIf(Not ItemId = Nothing, "ItemId = " & ItemId, "ItemAttributeId = " & ItemAttributeId) & " AND Email = " & DB.Quote(Email) & " AND SendDate IS NULL")
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub


		Protected Overridable Sub Load(ByVal r As sqlDataReader)
			m_ItemNotifyId = Convert.ToInt32(r.Item("ItemNotifyId"))
			m_Email = Convert.ToString(r.Item("Email"))
			If IsDBNull(r.Item("ItemId")) Then
				m_ItemId = Nothing
			Else
				m_ItemId = Convert.ToInt32(r.Item("ItemId"))
			End If
			If IsDBNull(r.Item("ItemAttributeId")) Then
				m_ItemAttributeId = Nothing
			Else
				m_ItemAttributeId = Convert.ToInt32(r.Item("ItemAttributeId"))
			End If
			m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
			If IsDBNull(r.Item("SendDate")) Then
				m_SendDate = Nothing
			Else
				m_SendDate = Convert.ToDateTime(r.Item("SendDate"))
			End If
			If IsDBNull(r.Item("ViewDate")) Then
				m_ViewDate = Nothing
			Else
				m_ViewDate = Convert.ToDateTime(r.Item("ViewDate"))
			End If
		End Sub	'Load

		Public Overridable Function Insert() As Integer
			Dim SQL As String


			SQL = " INSERT INTO StoreItemNotify (" _
			 & " Email" _
			 & ",ItemId" _
			 & ",ItemAttributeId" _
			 & ",CreateDate" _
			 & ",SendDate" _
			 & ",ViewDate" _
			 & ") VALUES (" _
			 & m_DB.Quote(Email) _
			 & "," & m_DB.NullNumber(ItemId) _
			 & "," & m_DB.NullNumber(ItemAttributeId) _
			 & "," & m_DB.NullQuote(Now) _
			 & "," & m_DB.NullQuote(SendDate) _
			 & "," & m_DB.NullQuote(ViewDate) _
			 & ")"

			ItemNotifyId = m_DB.InsertSQL(SQL)

			Return ItemNotifyId
		End Function

		Public Overridable Sub Update()
			Dim SQL As String

			SQL = " UPDATE StoreItemNotify SET " _
			 & " Email = " & m_DB.Quote(Email) _
			 & ",ItemId = " & m_DB.NullNumber(ItemId) _
			 & ",ItemAttributeId = " & m_DB.NullNumber(ItemAttributeId) _
			 & ",SendDate = " & m_DB.NullQuote(SendDate) _
			 & ",ViewDate = " & m_DB.NullQuote(ViewDate) _
			 & " WHERE ItemNotifyId = " & m_DB.quote(ItemNotifyId)

			m_DB.ExecuteSQL(SQL)

		End Sub	'Update

		Public Sub Remove()
			Dim SQL As String

			SQL = "DELETE FROM StoreItemNotify WHERE ItemNotifyId = " & m_DB.Number(ItemNotifyId)
			m_DB.ExecuteSQL(SQL)
		End Sub	'Remove
	End Class

	Public Class StoreItemNotifyCollection
		Inherits GenericCollection(Of StoreItemNotifyRow)
	End Class

End Namespace


