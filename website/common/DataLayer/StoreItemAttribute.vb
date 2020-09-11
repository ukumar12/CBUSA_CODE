Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports System.Net.Mail
Imports Components

Namespace DataLayer

    Public Class StoreItemAttributeRow
        Inherits StoreItemAttributeRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ItemAttributeId As Integer)
            MyBase.New(DB, ItemAttributeId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal ItemAttributeId As Integer) As StoreItemAttributeRow
            Dim row As StoreItemAttributeRow

            row = New StoreItemAttributeRow(DB, ItemAttributeId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ItemAttributeId As Integer)
            Dim row As StoreItemAttributeRow

            row = New StoreItemAttributeRow(DB, ItemAttributeId)
            row.Remove()
        End Sub

        'Custom Methods
		Public Shared Sub LoadTempAttributes(ByVal DB As Database, ByVal ItemId As Integer, ByVal Guid As String)
			Dim SQL As String = "DELETE FROM StoreItemAttributeTemp WHERE Guid IN (SELECT Guid FROM StoreItemAttributeTempGuid WHERE CreateDate <= GetDate() - 1)"
			DB.ExecuteSQL(SQL)

			SQL = "DELETE FROM StoreItemAttributeTempGuid WHERE Guid NOT IN (SELECT Guid FROM StoreItemAttributeTemp)"
			DB.ExecuteSQL(SQL)

			SQL = "INSERT INTO StoreItemAttributeTempGuid (Guid) Values (" & DB.Quote(Guid) & ")"
			DB.ExecuteSQL(SQL)

			'Insert the records
			SQL = "INSERT INTO StoreItemAttributeTemp (Guid,ItemAttributeId,TemplateAttributeId,ParentAttributeId,ItemId,AttributeValue,SKU,Price,Weight,InventoryQty,InventoryActionThreshold,InventoryWarningThreshold,InventoryAction,BackorderDate,ImageName,ImageAlt,ProductImage,ProductAlt,IsActive,SortOrder) SELECT " & DB.Quote(Guid) & ",ItemAttributeId,TemplateAttributeId,ParentAttributeId,ItemId,AttributeValue,SKU,Price,Weight,InventoryQty,InventoryActionThreshold,InventoryWarningThreshold,InventoryAction,BackorderDate,ImageName,ImageAlt,ProductImage,ProductAlt,IsActive,SortOrder FROM StoreItemAttribute WHERE ItemId = " & ItemId
			DB.ExecuteSQL(SQL)

			'Update ParentIds
			SQL = "UPDATE StoreItemAttributeTemp SET ParentAttributeId = tmp.TempAttributeId FROM (SELECT TempAttributeId, ItemAttributeId, Guid FROM StoreItemAttributeTemp) tmp WHERE StoreItemAttributeTemp.Guid = tmp.Guid AND StoreItemAttributeTemp.ParentAttributeId = tmp.ItemAttributeId AND StoreItemAttributeTemp.Guid = " & DB.Quote(Guid) & " AND ItemId = " & ItemId
			DB.ExecuteSQL(SQL)
        End Sub

		Public Shared Sub SaveTempAttributes(ByVal DB As Database, ByVal ItemId As Integer, ByVal Guid As String)
			' Remove attributes which are no longer part of this item
			Dim SQL As String = "DELETE FROM StoreItemAttribute WHERE ItemId=" & ItemId & " AND (ItemAttributeId NOT IN (SELECT ItemAttributeId FROM StoreItemAttributeTemp WHERE ItemAttributeId IS NOT NULL AND ItemId=" & ItemId & " AND Guid = " & DB.Quote(Guid) & ") OR TemplateAttributeId NOT IN (SELECT TemplateAttributeId FROM StoreItemTemplateAttribute WHERE TemplateId = (SELECT TOP 1 TemplateId FROM StoreItem WHERE ItemId = " & ItemId & ")))"
			DB.ExecuteSQL(SQL)

			Dim dbItem As StoreItemRow = StoreItemRow.GetRow(DB, ItemId)

			SQL = "SELECT Top 1 TemplateAttributeId FROM StoreItemTemplateAttribute WHERE TemplateId = " & dbItem.TemplateId & " ORDER BY SortOrder"
			Dim RootId As Integer = DB.ExecuteScalar(SQL)

			'Update existing records
			SQL = "UPDATE StoreItemAttribute SET " & IIf(SysParam.GetValue(DB, "EnableInventoryManagement") = 1 AndAlso SysParam.GetValue(DB, "EnableAttributeInventoryManagement") = 1, "SendNotification = CASE WHEN (COALESCE(tmp.InventoryAction," & DB.Quote(dbItem.InventoryAction) & ") = 'OutOfStock' AND (SELECT TOP 1 InventoryQty FROM StoreItemAttribute sia WHERE sia.ItemAttributeId = StoreItemAttribute.ItemAttributeId) <= COALESCE(tmp.InventoryActionThreshold," & dbItem.InventoryActionThreshold & "," & DB.Quote(SysParam.GetValue(DB, "InventoryActionThreshold")) & ") AND tmp.InventoryQty > COALESCE(tmp.InventoryActionThreshold," & dbItem.InventoryActionThreshold & "," & DB.Quote(SysParam.GetValue(DB, "InventoryActionThreshold")) & ")) THEN 1 ELSE 0 END, ", "") & "TempAttributeId = tmp.TempAttributeId, TemplateAttributeId = tmp.TemplateAttributeId, ParentAttributeId = tmp.ParentAttributeId, ItemId = tmp.ItemId, AttributeValue = tmp.AttributeValue, SKU = tmp.SKU, Price = tmp.Price, Weight = tmp.Weight, InventoryQty = tmp.InventoryQty, InventoryWarningThreshold = tmp.InventoryWarningThreshold, InventoryActionThreshold = tmp.InventoryActionThreshold, InventoryAction = tmp.InventoryAction, BackorderDate = tmp.BackorderDate, ImageName = tmp.ImageName, ImageAlt = tmp.ImageAlt, ProductImage = tmp.ProductImage, ProductAlt = tmp.ProductAlt, IsActive = tmp.IsActive, SortOrder = tmp.SortOrder, IsValidated = 0 FROM (SELECT * FROM StoreItemAttributeTemp) tmp WHERE StoreItemAttribute.ItemAttributeId = tmp.ItemAttributeId AND tmp.ItemId = " & DB.Number(ItemId) & " AND Guid = " & DB.Quote(Guid)
			DB.ExecuteSQL(SQL)

			'Insert new records
			SQL = "INSERT INTO StoreItemAttribute (TempAttributeId,TemplateAttributeId,ParentAttributeId,ItemId,AttributeValue,SKU,Price,Weight,InventoryQty,InventoryWarningThreshold,InventoryActionThreshold,InventoryAction,BackorderDate,ImageName,ImageAlt,ProductImage,ProductAlt,IsActive,SortOrder,IsValidated) SELECT TempAttributeId,TemplateAttributeId,ParentAttributeId," & ItemId & ",AttributeValue,SKU,Price,Weight,InventoryQty,InventoryWarningThreshold,InventoryActionThreshold,InventoryAction,BackorderDate,ImageName,ImageAlt,ProductImage,ProductAlt,IsActive,SortOrder,0 FROM StoreItemAttributeTemp WHERE Guid = " & DB.Quote(Guid) & " AND ItemId = " & DB.Number(ItemId) & " AND ItemAttributeId IS NULL"
			DB.ExecuteSQL(SQL)

			'Update ParentIds and ControlCount
			SQL = "UPDATE StoreItemAttribute SET ParentAttributeId = tmp.ItemAttributeId FROM (SELECT a.TempAttributeId, a.ItemAttributeId, Guid FROM StoreItemAttribute a INNER JOIN StoreItemAttributeTemp t ON a.TempAttributeId = t.TempAttributeId WHERE Guid = " & DB.Quote(Guid) & " AND t.ItemId = " & DB.Number(ItemId) & ") tmp WHERE StoreItemAttribute.ParentAttributeId = tmp.TempAttributeId AND ItemId = " & ItemId
			DB.ExecuteSQL(SQL)

			'Validate attributes
			ValidateAttributes(DB, ItemId, dbItem.TemplateId)

			'Save Final SKUs
			DB.ExecuteSQL("exec sp_SaveFinalSKUs " & ItemId)

			RemoveTemporaryRecords(DB, Guid)

			UpdateControlCount(DB, ItemId)
        End Sub

		Public Shared Sub ValidateAttributes(ByVal DB As Database, ByVal ItemId As Integer, Optional ByVal TemplateId As Integer = Nothing)
			Dim SQL As String

			SQL = "SELECT TOP 1 TemplateId FROM StoreItem WHERE ItemId = " & ItemId
			If TemplateId = Nothing Then TemplateId = DB.ExecuteScalar(SQL)

			SQL = "SELECT TemplateAttributeId FROM StoreItemTemplateAttribute WHERE TemplateId = " & TemplateId & " AND ParentId IS NULL"
			Dim dt As DataTable = DB.GetDataTable(SQL)
			For Each r As DataRow In dt.Rows
				Dim dv As DataView = DB.GetDataTable("exec sp_GetTemplateAttributeTree " & r("TemplateAttributeId")).DefaultView
				dv.Sort = "level desc"
				If dv.Count > 0 Then
					SQL = "SELECT ItemAttributeId, ParentAttributeId, IsActive FROM StoreItemAttribute WHERE ItemId = " & ItemId & " AND TemplateAttributeId = " & dv(0)("TemplateAttributeId")
					Dim dvValues As DataView = DB.GetDataTable(SQL).DefaultView
					For j As Integer = 0 To dvValues.Count - 1
						ValidateParent(DB, dvValues(j))
					Next
				End If
			Next

			'Update item
			SQL = "UPDATE StoreItem SET IsActive = CASE WHEN IsActive = 1 AND (SELECT COUNT(*) FROM StoreItemTemplateAttribute WHERE TemplateId = " & TemplateId & ") = (SELECT COUNT(*) FROM (SELECT MAX(CAST(IsValidated AS INT)) AS iCount FROM StoreItemAttribute WHERE ItemId = " & ItemId & " AND IsValidated = 1 GROUP BY TemplateAttributeId) tmp) THEN 1 ELSE 0 END WHERE ItemId = " & ItemId
			DB.ExecuteSQL(SQL)
		End Sub

		Private Shared Sub ValidateParent(ByVal DB As Database, ByVal r As DataRowView)
			Dim SQL As String = "UPDATE StoreItemAttribute SET IsValidated = 1 WHERE ItemAttributeId = " & r("ItemAttributeId")
			If CBool(r("IsActive")) Then
				DB.ExecuteSQL(SQL)
			Else
				Exit Sub
			End If

			If IsDBNull(r("ParentAttributeId")) Then Exit Sub

			SQL = "SELECT TOP 1 ItemAttributeId, ParentAttributeId, IsActive FROM StoreItemAttribute WHERE ItemAttributeId = " & r("ParentAttributeId")
			Dim dv As DataView = DB.GetDataTable(SQL).DefaultView
			If dv.Count > 0 Then
				ValidateParent(DB, dv(0))
			End If
		End Sub

		Public Shared Sub RemoveTemporaryRecords(ByVal DB As Database, ByVal Guid As String)
			Dim SQL As String = "DELETE FROM StoreItemAttributeTemp WHERE Guid = " & DB.Quote(Guid)
			DB.ExecuteSQL(SQL)

			SQL = "DELETE FROM StoreItemAttributeTempGuid WHERE Guid = " & DB.Quote(Guid)
			DB.ExecuteSQL(SQL)
		End Sub

		Public Shared Function GetAttributes(ByVal DB As Database, ByVal ItemId As Integer, ByVal TemplateAttributeId As Integer, ByVal ParentAttributeId As Integer) As DataTable
			Return DB.GetDataTable("SELECT * FROM StoreItemAttribute WHERE ItemId = " & ItemId & " AND TemplateAttributeId = " & TemplateAttributeId & " AND COALESCE(ParentAttributeId,0) = " & ParentAttributeId & " ORDER BY SortOrder")
		End Function
	End Class

    Public MustInherit Class StoreItemAttributeRowBase
        Private m_DB As Database
        Private m_ItemAttributeId As Integer = Nothing
        Private m_TemplateAttributeId As Integer = Nothing
		Private m_ParentAttributeId As Integer = Nothing
		Private m_TempAttributeId As Integer = Nothing
		Private m_ItemId As Integer = Nothing
        Private m_AttributeValue As String = Nothing
        Private m_SKU As String = Nothing
		Private m_FinalSKU As String = Nothing
		Private m_Price As Double = Nothing
		Private m_Weight As Double = Nothing
		Private m_OriginalInventoryQty As Integer = Nothing
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
		Private m_IsValidated As Boolean = Nothing
		Private m_SendNotification As Boolean = Nothing
        Private m_SortOrder As Integer = Nothing


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

		Public Property TempAttributeId() As Integer
			Get
				Return m_TempAttributeId
			End Get
			Set(ByVal value As Integer)
				m_TempAttributeId = value
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

		Public Property FinalSKU() As String
			Get
				Return m_FinalSKU
			End Get
			Set(ByVal Value As String)
				m_FinalSKU = Value
			End Set
		End Property

		Public Property Price() As Double
			Get
				Return m_Price
			End Get
			Set(ByVal Value As Double)
				m_Price = Value
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

		Public Property BackorderDate() As DateTime
			Get
				Return m_BackorderDate
			End Get
			Set(ByVal value As DateTime)
				m_BackorderDate = value
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

		Public Property IsValidated() As Boolean
			Get
				Return m_IsValidated
			End Get
			Set(ByVal value As Boolean)
				m_IsValidated = value
			End Set
		End Property

		Public Property SendNotification() As Boolean
			Get
				Return m_SendNotification
			End Get
			Set(ByVal value As Boolean)
				m_SendNotification = value
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
            Set(ByVal Value As DataBase)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ItemAttributeId As Integer)
            m_DB = DB
            m_ItemAttributeId = ItemAttributeId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM StoreItemAttribute WHERE ItemAttributeId = " & DB.Number(ItemAttributeId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


		Protected Overridable Sub Load(ByVal r As Object)
			If Not TypeOf r Is SqlDataReader AndAlso Not TypeOf r Is DataRowView AndAlso Not TypeOf r Is DataRow Then Throw New ApplicationException("Invalid Load Parameters!")

			m_ItemAttributeId = Convert.ToInt32(r.Item("ItemAttributeId"))
			m_TemplateAttributeId = Convert.ToInt32(r.Item("TemplateAttributeId"))
			m_ItemId = Convert.ToInt32(r.Item("ItemId"))
			m_AttributeValue = Convert.ToString(r.Item("AttributeValue"))
			If IsDBNull(r.Item("TempAttributeId")) Then
				m_TempAttributeId = Nothing
			Else
				m_TempAttributeId = Convert.ToString(r.Item("TempAttributeId"))
			End If
			If IsDBNull(r.Item("SKU")) Then
				m_SKU = Nothing
			Else
				m_SKU = Convert.ToString(r.Item("SKU"))
			End If
			If IsDBNull(r.Item("FinalSKU")) Then
				m_FinalSKU = Nothing
			Else
				m_FinalSKU = Convert.ToString(r.Item("FinalSKU"))
			End If
			m_Price = Convert.ToDouble(r.Item("Price"))
			m_Weight = Convert.ToDouble(r.Item("Weight"))
			m_InventoryQty = Convert.ToInt32(r.Item("InventoryQty"))
			m_OriginalInventoryQty = m_InventoryQty
			If IsDBNull(r.Item("InventoryAction")) Then
				m_InventoryAction = Nothing
			Else
				m_InventoryAction = Convert.ToString(r.Item("InventoryAction"))
			End If
			m_InventoryActionThreshold = Convert.ToInt32(r.Item("InventoryActionThreshold"))
			If IsDBNull(r.Item("InventoryWarningThreshold")) Then
				m_InventoryWarningThreshold = Nothing
			Else
				m_InventoryWarningThreshold = Convert.ToInt32(r.Item("InventoryWarningThreshold"))
			End If
			If IsDBNull(r.Item("BackorderDate")) Then
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
			m_IsValidated = Convert.ToBoolean(r.Item("IsValidated"))
			m_SendNotification = Convert.ToBoolean(r.Item("SendNotification"))
			If IsDBNull(r.Item("ParentAttributeId")) Then
				m_ParentAttributeId = Nothing
			Else
				m_ParentAttributeId = Convert.ToInt32(r.Item("ParentAttributeId"))
			End If
			m_SortOrder = Convert.ToInt32(r.Item("SortOrder"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

			Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from StoreItemAttribute WHERE TemplateAttributeId=" & TemplateAttributeId & " AND ParentAttributeId " & IIf(ParentAttributeId = 0, " IS NULL", " = " & ParentAttributeId) & " AND ItemId=" & ItemId & " order by SortOrder desc")
			MaxSortOrder += 1

			ValidateInventory()

			SQL = " INSERT INTO StoreItemAttribute (" _
			 & " TemplateAttributeId" _
			 & ",ParentAttributeId" _
			 & ",TempAttributeId" _
			 & ",ItemId" _
			 & ",AttributeValue" _
			 & ",SKU" _
			 & ",FinalSKU" _
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
			 & ",IsValidated" _
			 & ",SendNotification" _
			 & ",SortOrder" _
			 & ") VALUES (" _
			 & m_DB.NullNumber(TemplateAttributeId) _
			 & "," & m_DB.NullNumber(ParentAttributeId) _
			 & "," & m_DB.NullNumber(TempAttributeId) _
			 & "," & m_DB.NullNumber(ItemId) _
			 & "," & m_DB.Quote(AttributeValue) _
			 & "," & m_DB.Quote(SKU) _
			 & "," & m_DB.Quote(FinalSKU) _
			 & "," & m_DB.Number(Price) _
			 & "," & m_DB.Number(Weight) _
			 & "," & m_DB.Number(InventoryQty) _
			 & "," & m_DB.Quote(InventoryAction) _
			 & "," & m_DB.Number(InventoryActionThreshold) _
			 & "," & m_DB.NullNumber(InventoryWarningThreshold) _
			 & "," & m_DB.NullQuote(BackorderDate) _
			 & "," & m_DB.Quote(ImageName) _
			 & "," & m_DB.Quote(ProductImage) _
			 & "," & m_DB.Quote(ImageAlt) _
			 & "," & m_DB.Quote(ProductAlt) _
			 & "," & CInt(IsActive) _
			 & "," & CInt(IsValidated) _
			 & "," & CInt(SendNotification) _
			 & "," & MaxSortOrder _
			 & ")"

            ItemAttributeId = m_DB.InsertSQL(SQL)

            Return ItemAttributeId
        End Function

        Public Overridable Sub Update()
			Dim SQL As String

			ValidateInventory()

			SQL = " UPDATE StoreItemAttribute SET " _
			 & " TemplateAttributeId = " & m_DB.NullNumber(TemplateAttributeId) _
			 & ",ParentAttributeId = " & m_DB.NullNumber(ParentAttributeId) _
			 & ",TempAttributeId = " & m_DB.NullNumber(TempAttributeId) _
			 & ",ItemId = " & m_DB.NullNumber(ItemId) _
			 & ",AttributeValue = " & m_DB.Quote(AttributeValue) _
			 & ",SKU = " & m_DB.Quote(SKU) _
			 & ",FinalSKU = " & m_DB.Quote(FinalSKU) _
			 & ",Price = " & m_DB.Number(Price) _
			 & ",Weight = " & m_DB.Number(Weight) _
			 & ",InventoryQty = " & m_DB.Number(InventoryQty) _
			 & ",InventoryAction = " & m_DB.Quote(InventoryAction) _
			 & ",InventoryActionThreshold = " & m_DB.Number(InventoryActionThreshold) _
			 & ",InventoryWarningThreshold = " & m_DB.NullNumber(InventoryWarningThreshold) _
			 & ",BackorderDate = " & m_DB.NullQuote(BackorderDate) _
			 & ",ImageName = " & m_DB.Quote(ImageName) _
			 & ",ProductImage = " & m_DB.Quote(ProductImage) _
			 & ",ImageAlt = " & m_DB.Quote(ImageAlt) _
			 & ",ProductAlt = " & m_DB.Quote(ProductAlt) _
			 & ",IsActive = " & CInt(IsActive) _
			 & ",IsValidated = " & CInt(IsValidated) _
			 & ",SendNotification = " & CInt(SendNotification) _
			 & " WHERE ItemAttributeId = " & m_DB.Quote(ItemAttributeId)

			m_DB.ExecuteSQL(SQL)

			'Update ControlCount
			UpdateControlCount(m_DB, ItemId)

		End Sub	'Update

		Protected Shared Sub UpdateControlCount(ByVal DB As Database, ByVal ItemId As Integer)
			Dim SQL As String = "update storeitemattribute set controlcount = (select CASE WHEN templateattributeid in (select templateattributeid from storeitemtemplateattribute where AttributeType IN ('swatch','radio')) THEN (SELECT COUNT(*) FROM StoreItemAttribute sia WHERE sia.ItemId= storeitemattribute.itemid AND sia.TemplateAttributeId = storeitemattribute.TemplateAttributeId AND COALESCE(sia.ParentAttributeId,0) = COALESCE(storeitemattribute.ParentAttributeId,0) GROUP BY ParentAttributeId) ELSE 0 END) where itemid = " & ItemId
			DB.ExecuteSQL(SQL)
		End Sub

		Private Sub ValidateInventory()
			If SysParam.GetValue(DB, "EnableInventoryManagement") = 1 AndAlso SysParam.GetValue(DB, "EnableAttributeInventoryManagement") = 1 Then
				If InventoryAction = "Disable" AndAlso InventoryQty <= 0 Then
					IsActive = False
				End If

				If ItemAttributeId = Nothing OrElse ItemId = Nothing Then Exit Sub

				Dim dbItem As StoreItemRow = StoreItemRow.GetRow(DB, ItemId)

				If m_OriginalInventoryQty <= InventoryActionThreshold AndAlso ((InventoryActionThreshold > 0 AndAlso InventoryQty > InventoryActionThreshold) OrElse (dbItem.InventoryActionThreshold > 0 AndAlso InventoryQty > dbItem.InventoryActionThreshold) OrElse (InventoryQty > SysParam.GetValue(DB, "InventoryActionThreshold"))) Then
					Dim Message As New MailMessage
					Dim SQL, DisplayName, sBody As String

					SQL = "sp_GetAttributeTreeTableLayout " & ItemId & ", " & dbItem.TemplateId
					Dim dv As DataView = DB.GetDataTable(SQL).DefaultView
					Dim AttributeString As String = String.Empty
					Dim dt As DataTable
					dv.RowFilter = "ItemAttributeId = " & ItemAttributeId
					If dv.Count > 0 Then
						SQL = "SELECT AttributeName + '=' + AttributeValue AS AttributeString FROM StoreItemTemplateAttribute a INNER JOIN StoreItemAttribute ia ON a.TemplateAttributeId = ia.TemplateAttributeId WHERE ItemAttributeId IN " & DB.NumberMultiple(dv(0)("ItemAttributeIds"))
						dt = DB.GetDataTable(SQL)
						For Each r As DataRow In dt.Rows
							AttributeString &= IIf(AttributeString = String.Empty, "", ", ") & r("AttributeString")
						Next

						If SysParam.GetValue(DB, "InventoryReplenishmentFromName") = Nothing Then DisplayName = SysParam.GetValue(DB, "InventoryReplenishmentFromEmail") Else DisplayName = SysParam.GetValue(DB, "InventoryReplenishmentFromName")
						Message.From = New MailAddress(SysParam.GetValue(DB, "InventoryReplenishmentFromEmail"), DisplayName)
						Message.Subject = "Product " & dbItem.ItemName & " (" & AttributeString & ") has recently become available for purchase!"
						sBody = "<p><a href=""" & System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/store/item.aspx?ItemId=" & ItemId & "&ItemNotifyId=%%ItemNotifyId%%"">Click here</a> to view this product.</p>"
						Message.IsBodyHtml = True

						SQL = "SELECT Email, ItemNotifyId FROM StoreItemNotify WHERE SendDate IS NULL AND ItemAttributeId = " & ItemAttributeId
						dt = DB.GetDataTable(SQL)

						SendNotification = False

						SQL = "UPDATE StoreItemNotify SET SendDate = GETDATE() WHERE SendDate IS NULL AND ItemAttributeId = " & ItemAttributeId
						DB.ExecuteSQL(SQL)

						For Each r As DataRow In dt.Rows
							Dim Notify As New Notification
							Message.Body = sBody.Replace("%%ItemNotifyId%%", r("ItemNotifyId"))
							Notify.Message = Message
							Notify.Recipients.Add(New Net.Mail.MailAddress(r("Email")))
							Notify.Queue()
						Next
					End If
				End If
			End If
		End Sub

		Public Sub Remove()
			Dim SQL As String

			SQL = "DELETE FROM StoreItemAttribute WHERE ItemAttributeId = " & m_DB.Quote(ItemAttributeId)
			m_DB.ExecuteSQL(SQL)
		End Sub	'Remove

    End Class

    Public Class StoreItemAttributeCollection
        Inherits GenericCollection(Of StoreItemAttributeRow)
    End Class

	<Serializable()> _
    Public Class ItemAttributeCollection
		Inherits GenericSerializableCollection(Of ItemAttribute)
    End Class

	<Serializable()> _
	Public Class ItemAttribute
		Implements IComparable

		Public Id As String
		Public AttributeName As String
		Public AttributeType As String
		Public ItemAttributeId As Integer
		Public TemplateAttributeId As Integer
		Public ParentAttributeId As Integer
		Public ItemId As Integer
		Public AttributeValue As String
		Public SKU As String
		Public Price As Double
		Public Weight As Double
		Public ImageName As String
		Public ImageAlt As String
		Public ProductImage As String
		Public ProductAlt As String
		Public IsActive As Boolean
		Public SortOrder As Integer
		Public ctrl As Object

		Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
			If Not TypeOf obj Is ItemAttribute Then Throw New ApplicationException("Invalid comparison")

			Dim Attribute As ItemAttribute = obj
			Dim Result As Integer = Me.TemplateAttributeId.CompareTo(Attribute.TemplateAttributeId)

			Return Result
		End Function

		Public Overloads Function Equals(ByVal value As ItemAttribute) As Boolean
			Return ItemAttributeId = value.ItemAttributeId
		End Function
	End Class

End Namespace

