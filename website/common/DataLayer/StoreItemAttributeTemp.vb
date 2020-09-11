Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

	Public Class StoreItemAttributeTempRow
		Inherits StoreItemAttributeTempRowBase

		Public Sub New()
			MyBase.New()
		End Sub	'New

		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub	'New

		Public Sub New(ByVal DB As Database, ByVal TempAttributeId As Integer)
			MyBase.New(DB, TempAttributeId)
		End Sub	'New

		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB As Database, ByVal TempAttributeId As Integer) As StoreItemAttributeTempRow
			Dim row As StoreItemAttributeTempRow

			row = New StoreItemAttributeTempRow(DB, TempAttributeId)
			row.Load()

			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB As Database, ByVal TempAttributeId As Integer)
			Dim row As StoreItemAttributeTempRow

			row = New StoreItemAttributeTempRow(DB, TempAttributeId)
			row.Load()
			row.Remove()
		End Sub

		'Custom Methods
		Public Shared Function GetAttributes(ByVal DB As Database, ByVal TemplateAttributeId As Integer, ByVal Guid As String, ByVal ParentAttributeId As Integer) As DataTable
			Dim SQL As String = "SELECT at.*, ta.AttributeType, COALESCE(ta.ParentId,0) AS ParentId FROM StoreItemAttributeTemp at INNER JOIN StoreItemTemplateAttribute ta ON at.TemplateAttributeId = ta.TemplateAttributeId WHERE Guid = " & DB.Quote(Guid) & " AND at.TemplateAttributeId = " & TemplateAttributeId & " AND COALESCE(ParentAttributeId,0) = " & ParentAttributeId & " ORDER BY SortOrder"
			Return DB.GetDataTable(SQL)
		End Function

		Public Shared Sub CopyAttributes(ByVal DB As Database, ByVal TemplateAttributeId As Integer, ByVal ParentAttributeId As Integer, ByVal Guid As String)
			Dim SQL As String = "SELECT TempAttributeId, ParentAttributeId FROM StoreItemAttributeTemp WHERE Guid = " & DB.Quote(Guid) & " AND ParentAttributeId <> " & ParentAttributeId & " AND TemplateAttributeId = " & TemplateAttributeId & " ORDER BY SortOrder"
			Dim dt As DataTable = DB.GetDataTable(SQL)
			Dim ParentIds As String = String.Empty
			Dim ParentTemplateAttributeId As Integer = DB.ExecuteScalar("SELECT TOP 1 TemplateAttributeId FROM StoreItemAttributeTemp WHERE TempAttributeId = " & ParentAttributeId)

			For Each r As DataRow In dt.Rows
				StoreItemAttributeTempRow.RemoveRow(DB, r("TempAttributeId"))
				ParentIds &= IIf(ParentIds = String.Empty, "", ",") & r("ParentAttributeId")
			Next

			SQL = "SELECT * FROM StoreItemAttributeTemp t1 WHERE t1.TempAttributeId IN " & DB.NumberMultiple(ParentIds) & " OR t1.TempAttributeId IN (SELECT TempAttributeId FROM StoreItemAttributeTemp t2 WHERE t2.TemplateAttributeId = t1.TemplateAttributeId AND t2.Guid = t1.Guid AND t2.TemplateAttributeId = " & ParentTemplateAttributeId & " AND NOT EXISTS (SELECT TempAttributeId FROM StoreItemAttributeTemp t3 WHERE t3.ParentAttributeId = t2.TempAttributeId))"
			dt = DB.GetDataTable(SQL)
			For Each r As DataRow In dt.Rows
				SQL = "SELECT * FROM StoreItemAttributeTemp WHERE Guid = " & DB.Quote(Guid) & " AND TemplateAttributeId = " & TemplateAttributeId & " AND ParentAttributeId = " & ParentAttributeId & " ORDER BY SortOrder"
				Dim dtCopy As DataTable = DB.GetDataTable(SQL)
				For Each row As DataRow In dtCopy.Rows
					SaveChildren(DB, row, r("TempAttributeId"))
				Next
			Next
		End Sub

		Private Shared Sub SaveChildren(ByVal DB As Database, ByVal row As DataRow, ByVal ParentAttributeId As Integer)
			Dim dbAttribute As New StoreItemAttributeTempRow(DB)
			Dim OriginalTempAttributeId As Integer = row("TempAttributeId")
			dbAttribute.Load(row)
			dbAttribute.TempAttributeId = Nothing
			dbAttribute.ItemAttributeId = Nothing
			dbAttribute.ParentAttributeId = ParentAttributeId
			dbAttribute.Insert()

			Dim SQL As String = "SELECT * FROM StoreItemAttributeTemp WHERE ParentAttributeId = " & OriginalTempAttributeId
			Dim dtValues As DataTable = DB.GetDataTable(SQL)
			For Each r As DataRow In dtValues.Rows
				SaveChildren(DB, r, dbAttribute.TempAttributeId)
			Next
		End Sub

		Public Function ValidateLocalSettings(ByRef sError As String) As String
			Dim SQL As String = "SELECT * FROM StoreItemAttributeTemp WHERE Guid = " & DB.Quote(Guid) & " AND TemplateAttributeId = " & TemplateAttributeId & " AND COALESCE(ParentAttributeId,0) = " & ParentAttributeId & " AND (SKU = " & DB.Quote(SKU) & " OR AttributeValue = " & DB.Quote(AttributeValue) & ") AND TempAttributeId <> " & TempAttributeId
			Dim dt As DataTable = DB.GetDataTable(SQL)
			For Each r As DataRow In dt.Rows
				If Not SKU = String.Empty AndAlso Not IsDBNull(r("SKU")) AndAlso SKU = r("SKU") Then
					sError = "Another attribute in this list has already been assigned this SKU"
				End If
				If AttributeValue = r("AttributeValue") Then
					sError = "Another attribute in this list has already been assigned this value"
				End If
			Next
			Return sError
		End Function
	End Class

	Public MustInherit Class StoreItemAttributeTempRowBase
		Private m_DB As Database
		Private m_TempAttributeId As Integer = Nothing
		Private m_Guid As String = Nothing
		Private m_ItemAttributeId As Integer = Nothing
		Private m_TemplateAttributeId As Integer = Nothing
		Private m_ParentAttributeId As Integer = Nothing
		Private m_ItemId As Integer = Nothing
		Private m_AttributeValue As String = Nothing
		Private m_SKU As String = Nothing
		Private m_Price As Double = Nothing
		Private m_Weight As Double = Nothing
		Private m_InventoryQty As Integer = Nothing
		Private m_InventoryAction As String = Nothing
		Private m_InventoryActionThreshold As Integer = Nothing
		Private m_InventoryWarningThreshold As Integer = Nothing
		Private m_BackorderDate As DateTime = Nothing
		Private m_ImageName As String = Nothing
		Private m_ImageAlt As String = Nothing
		Private m_ProductImage As String = Nothing
		Private m_ProductAlt As String = Nothing
		Private m_IsActive As Boolean = Nothing
		Private m_SortOrder As Integer = Nothing


		Public Property TempAttributeId() As Integer
			Get
				Return m_TempAttributeId
			End Get
			Set(ByVal Value As Integer)
				m_TempAttributeId = value
			End Set
		End Property

		Public Property Guid() As String
			Get
				Return m_Guid
			End Get
			Set(ByVal Value As String)
				m_Guid = Value
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

		Public Property TemplateAttributeId() As Integer
			Get
				Return m_TemplateAttributeId
			End Get
			Set(ByVal Value As Integer)
				m_TemplateAttributeId = value
			End Set
		End Property

		Public Property ParentAttributeId() As Integer
			Get
				Return m_ParentAttributeId
			End Get
			Set(ByVal value As Integer)
				m_ParentAttributeId = value
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

		Public Property AttributeValue() As String
			Get
				Return m_AttributeValue
			End Get
			Set(ByVal Value As String)
				m_AttributeValue = value
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

		Public Property InventoryQty() As Integer
			Get
				Return m_InventoryQty
			End Get
			Set(ByVal value As Integer)
				m_InventoryQty = value
			End Set
		End Property

		Public Property InventoryAction() As String
			Get
				Return m_InventoryAction
			End Get
			Set(ByVal value As String)
				m_InventoryAction = value
			End Set
		End Property

		Public Property BackorderDate() As DateTime
			Get
				Return m_BackorderDate
			End Get
			Set(ByVal value As DateTime)
				m_BackorderDate = value
			End Set
		End Property

		Public Property InventoryActionThreshold() As Integer
			Get
				Return m_InventoryActionThreshold
			End Get
			Set(ByVal value As Integer)
				m_InventoryActionThreshold = value
			End Set
		End Property

		Public Property InventoryWarningThreshold() As Integer
			Get
				Return m_InventoryWarningThreshold
			End Get
			Set(ByVal value As Integer)
				m_InventoryWarningThreshold = value
			End Set
		End Property

		Public Property ImageName() As String
			Get
				Return m_ImageName
			End Get
			Set(ByVal Value As String)
				m_ImageName = Value
			End Set
		End Property

		Public Property ImageAlt() As String
			Get
				Return m_ImageAlt
			End Get
			Set(ByVal Value As String)
				m_ImageAlt = Value
			End Set
		End Property

		Public Property ProductImage() As String
			Get
				Return m_ProductImage
			End Get
			Set(ByVal Value As String)
				m_ProductImage = Value
			End Set
		End Property

		Public Property ProductAlt() As String
			Get
				Return m_ProductAlt
			End Get
			Set(ByVal value As String)
				m_ProductAlt = value
			End Set
		End Property

		Public Property IsActive() As Boolean
			Get
				Return m_IsActive
			End Get
			Set(ByVal value As Boolean)
				m_IsActive = value
			End Set
		End Property

		Public Property SortOrder() As Integer
			Get
				Return m_SortOrder
			End Get
			Set(ByVal Value As Integer)
				m_SortOrder = Value
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
		End Sub	'New

		Public Sub New(ByVal DB As Database)
			m_DB = DB
		End Sub	'New

		Public Sub New(ByVal DB As Database, ByVal TempAttributeId As Integer)
			m_DB = DB
			m_TempAttributeId = TempAttributeId
		End Sub	'New

		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String

			SQL = "SELECT * FROM StoreItemAttributeTemp WHERE TempAttributeId = " & DB.Number(TempAttributeId)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub


		Protected Overridable Sub Load(ByVal r As Object)
			If Not TypeOf r Is SqlDataReader AndAlso Not TypeOf r Is DataRowView AndAlso Not TypeOf r Is DataRow Then Throw New ApplicationException("Invalid Load Parameters!")

			m_TempAttributeId = Convert.ToInt32(r.Item("TempAttributeId"))
			m_Guid = Convert.ToString(r.Item("Guid"))
			If IsDBNull(r.Item("ItemAttributeId")) Then
				m_ItemAttributeId = Nothing
			Else
				m_ItemAttributeId = Convert.ToInt32(r.Item("ItemAttributeId"))
			End If
			m_TemplateAttributeId = Convert.ToInt32(r.Item("TemplateAttributeId"))
			If IsDBNull(r.Item("ParentAttributeId")) Then
				m_ParentAttributeId = Nothing
			Else
				m_ParentAttributeId = Convert.ToInt32(r.Item("ParentAttributeId"))
			End If
			If IsDBNull(r.Item("ItemId")) Then
				m_ItemId = Nothing
			Else
				m_ItemId = Convert.ToInt32(r.Item("ItemId"))
			End If
			m_AttributeValue = Convert.ToString(r.Item("AttributeValue"))
			If IsDBNull(r.Item("SKU")) Then
				m_SKU = Nothing
			Else
				m_SKU = Convert.ToString(r.Item("SKU"))
			End If
			m_Price = Convert.ToDouble(r.Item("Price"))
			m_Weight = Convert.ToDouble(r.Item("Weight"))
			m_InventoryQty = Convert.ToInt32(r.Item("InventoryQty"))
			If IsDBNull(r.Item("InventoryAction")) Then
				m_InventoryAction = Nothing
			Else
				m_InventoryAction = Convert.ToString(r.Item("InventoryAction"))
			End If
			If IsDBNull(r.Item("InventoryActionThreshold")) Then
				m_InventoryActionThreshold = Nothing
			Else
				m_InventoryActionThreshold = Convert.ToInt32(r.Item("InventoryActionThreshold"))
			End If
			If IsDBNull(r.Item("InventoryWarningThreshold")) Then
				m_InventoryWarningThreshold = Nothing
			Else
				m_InventoryWarningThreshold = Convert.ToInt32(r.Item("InventoryWarningThreshold"))
			End If
			If IsDBNull(r.item("BackorderDate")) Then
				m_BackorderDate = Nothing
			Else
				m_BackorderDate = Convert.ToDateTime(r.Item("BackorderDate"))
			End If
			If IsDBNull(r.Item("ImageName")) Then
				m_ImageName = Nothing
			Else
				m_ImageName = Convert.ToString(r.Item("ImageName"))
			End If
			If IsDBNull(r.Item("ProductImage")) Then
				m_ProductImage = Nothing
			Else
				m_ProductImage = Convert.ToString(r.Item("ProductImage"))
			End If
			If IsDBNull(r.Item("ImageAlt")) Then
				m_ImageAlt = Nothing
			Else
				m_ImageAlt = Convert.ToString(r.Item("ImageAlt"))
			End If
			If IsDBNull(r.Item("ProductAlt")) Then
				m_ProductAlt = Nothing
			Else
				m_ProductAlt = Convert.ToString(r.Item("ProductAlt"))
			End If
			m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
			m_SortOrder = Convert.ToInt32(r.Item("SortOrder"))
		End Sub	'Load

		Public Overridable Function Insert() As Integer
			Dim SQL As String

			Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from StoreItemAttributeTemp where ParentAttributeId " & IIf(ParentAttributeId = 0, " IS NULL", " = " & ParentAttributeId) & " AND TemplateAttributeId = " & TemplateAttributeId & " order by SortOrder desc")
			MaxSortOrder += 1

			SQL = " INSERT INTO StoreItemAttributeTemp (" _
			 & " Guid" _
			 & ",ItemAttributeId" _
			 & ",TemplateAttributeId" _
			 & ",ParentAttributeId" _
			 & ",ItemId" _
			 & ",AttributeValue" _
			 & ",SKU" _
			 & ",Price" _
			 & ",Weight" _
			 & ",InventoryQty" _
			 & ",InventoryAction" _
			 & ",InventoryActionThreshold" _
			 & ",InventoryWarningThreshold" _
			 & ",BackorderDate" _
			 & ",ImageName" _
			 & ",ProductImage" _
			 & ",ImageAlt" _
			 & ",ProductAlt" _
			 & ",IsActive" _
			 & ",SortOrder" _
			 & ") VALUES (" _
			 & m_DB.Quote(Guid) _
			 & "," & m_DB.NullNumber(ItemAttributeId) _
			 & "," & m_DB.NullNumber(TemplateAttributeId) _
			 & "," & m_DB.NullNumber(ParentAttributeId) _
			 & "," & m_DB.NullNumber(ItemId) _
			 & "," & m_DB.Quote(AttributeValue) _
			 & "," & m_DB.Quote(SKU) _
			 & "," & m_DB.Number(Price) _
			 & "," & m_DB.Number(Weight) _
			 & "," & m_DB.Number(InventoryQty) _
			 & "," & m_DB.Quote(InventoryAction) _
			 & "," & m_DB.NullNumber(InventoryActionThreshold) _
			 & "," & m_DB.NullNumber(InventoryWarningThreshold) _
			 & "," & m_DB.NullQuote(BackorderDate) _
			 & "," & m_DB.Quote(ImageName) _
			 & "," & m_DB.Quote(ProductImage) _
			 & "," & m_DB.Quote(ImageAlt) _
			 & "," & m_DB.Quote(ProductAlt) _
			 & "," & CInt(IsActive) _
			 & "," & MaxSortOrder _
			 & ")"

			TempAttributeId = m_DB.InsertSQL(SQL)

			Return TempAttributeId
		End Function

		Public Overridable Sub Update()
			Dim SQL As String

			SQL = " UPDATE StoreItemAttributeTemp SET " _
			 & " Guid = " & m_DB.Quote(Guid) _
			 & ",ItemAttributeId = " & m_DB.NullNumber(ItemAttributeId) _
			 & ",TemplateAttributeId = " & m_DB.NullNumber(TemplateAttributeId) _
			 & ",ParentAttributeId = " & m_DB.NullNumber(ParentAttributeId) _
			 & ",ItemId = " & m_DB.NullNumber(ItemId) _
			 & ",AttributeValue = " & m_DB.Quote(AttributeValue) _
			 & ",SKU = " & m_DB.Quote(SKU) _
			 & ",Price = " & m_DB.Number(Price) _
			 & ",Weight = " & m_DB.Number(Weight) _
			 & ",InventoryQty = " & m_DB.Number(InventoryQty) _
			 & ",InventoryAction = " & m_DB.Quote(InventoryAction) _
			 & ",InventoryActionThreshold = " & m_DB.NullNumber(InventoryActionThreshold) _
			 & ",InventoryWarningThreshold = " & m_DB.NullNumber(InventoryWarningThreshold) _
			 & ",BackorderDate = " & m_DB.NullQuote(BackorderDate) _
			 & ",ImageName = " & m_DB.Quote(ImageName) _
			 & ",ProductImage = " & m_DB.Quote(ProductImage) _
			 & ",ImageAlt = " & m_DB.Quote(ImageAlt) _
			 & ",ProductAlt = " & m_DB.Quote(ProductAlt) _
			 & ",IsActive = " & CInt(IsActive) _
			 & " WHERE TempAttributeId = " & m_DB.Quote(TempAttributeId)

			m_DB.ExecuteSQL(SQL)

		End Sub	'Update

		Public Sub Remove()
			Dim SQL As String = "SELECT TempAttributeId FROM StoreItemAttributeTemp WHERE ParentAttributeId = " & TempAttributeId
			Dim dtValues As DataTable = DB.GetDataTable(SQL)
			For Each r As DataRow In dtValues.Rows
				RemoveChildren(r)
			Next

			SQL = "DELETE FROM StoreItemAttributeTemp WHERE TempAttributeId = " & m_DB.Number(TempAttributeId)
			m_DB.ExecuteSQL(SQL)

			SQL = "UPDATE StoreItemAttributeTemp SET SortOrder = SortOrder - 1 WHERE Guid = " & DB.Quote(Guid) & " AND ItemId = " & ItemId & " AND TemplateAttributeId = " & TemplateAttributeId & " AND ParentAttributeId " & IIf(ParentAttributeId = 0, " IS NULL", " = " & ParentAttributeId) & " AND SortOrder > " & SortOrder
			m_DB.ExecuteSQL(SQL)
		End Sub	'Remove

		Private Sub RemoveChildren(ByVal row As DataRow)
			Dim SQL As String = "SELECT TempAttributeId FROM StoreItemAttributeTemp WHERE ParentAttributeId = " & m_DB.Number(row("TempAttributeId"))
			Dim dtValues As DataTable = DB.GetDataTable(SQL)
			For Each r As DataRow In dtValues.Rows
				RemoveChildren(r)
			Next

			SQL = "DELETE FROM StoreItemAttributeTemp WHERE TempAttributeId = " & m_DB.Number(row("TempAttributeId"))
			DB.ExecuteSQL(SQL)
		End Sub
	End Class

	Public Class StoreItemAttributeTempCollection
		Inherits GenericCollection(Of StoreItemAttributeTempRow)
	End Class

End Namespace


