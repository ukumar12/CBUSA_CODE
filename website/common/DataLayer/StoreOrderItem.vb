Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class StoreOrderItemRow
        Inherits StoreOrderItemRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal OrderItemId As Integer)
            MyBase.New(DB, OrderItemId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal OrderItemId As Integer) As StoreOrderItemRow
            Dim row As StoreOrderItemRow

            row = New StoreOrderItemRow(DB, OrderItemId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal OrderItemId As Integer)
            Dim row As StoreOrderItemRow

            row = New StoreOrderItemRow(DB, OrderItemId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Sub LoadFromStoreItem(ByVal dbItem As StoreItemRow)
            ItemId = dbItem.ItemId
            TemplateId = dbItem.TemplateId
            ItemName = dbItem.ItemName
            SKU = dbItem.SKU
            Image = dbItem.Image
            Weight = dbItem.Weight
            Width = dbItem.Width
            Height = dbItem.Height
            Thickness = dbItem.Thickness
            ItemPrice = dbItem.Price
            ItemUnit = dbItem.ItemUnit
            SalePrice = dbItem.SalePrice
            Shipping1 = dbItem.Shipping1
            Shipping2 = dbItem.Shipping2
            CountryUnit = dbItem.CountryUnit
            IsOnSale = dbItem.IsOnSale
            IsTaxFree = dbItem.IsTaxFree
            IsFeatured = dbItem.IsFeatured
            IsGiftWrap = dbItem.IsGiftWrap
            CustomURL = dbItem.CustomURL
        End Sub

        Public Function InsertAttribute(ByVal attr As ItemAttribute) As Integer
            Dim dbOrderItemAttribute As StoreOrderItemAttributeRow = New StoreOrderItemAttributeRow(DB)
            dbOrderItemAttribute.OrderId = OrderId
            dbOrderItemAttribute.OrderItemId = OrderItemId
            dbOrderItemAttribute.AttributeValue = attr.AttributeValue
            dbOrderItemAttribute.ItemAttributeId = attr.ItemAttributeId
            dbOrderItemAttribute.ItemId = attr.ItemId
            dbOrderItemAttribute.Price = attr.Price
            dbOrderItemAttribute.SKU = attr.SKU
            dbOrderItemAttribute.SortOrder = attr.SortOrder
            dbOrderItemAttribute.TemplateAttributeId = attr.TemplateAttributeId
			dbOrderItemAttribute.Weight = attr.Weight
			dbOrderItemAttribute.ImageAlt = attr.ImageAlt
			dbOrderItemAttribute.ImageName = attr.ImageName
			dbOrderItemAttribute.ProductAlt = attr.ProductAlt
			dbOrderItemAttribute.ProductImage = attr.ProductImage
            Return dbOrderItemAttribute.Insert()
        End Function

        Public Function GetItemAttributeCollection() As ItemAttributeCollection
            Return GetItemAttributeCollection(DB, OrderId, OrderItemId)
        End Function

        Public Shared Function GetItemAttributeCollection(ByVal DB As Database, ByVal OrderId As Integer, ByVal OrderItemId As Integer) As ItemAttributeCollection
            Dim col As New ItemAttributeCollection
            Dim SQL = "select soa.*, sita.AttributeType, sita.AttributeName from StoreOrderItemAttribute soa, StoreItemTemplateAttribute sita where soa.TemplateAttributeId = sita.TemplateAttributeId and soa.OrderId = " & OrderId & " and soa.OrderItemId = " & OrderItemId & " order by soa.SortOrder"
            Dim dr As SqlDataReader = DB.GetReader(SQL)
            While dr.Read
                Dim attr As New ItemAttribute
                attr.AttributeType = IIf(IsDBNull(dr("AttributeType")), String.Empty, dr("AttributeType"))
                attr.AttributeName = IIf(IsDBNull(dr("AttributeName")), String.Empty, dr("AttributeName"))
                attr.AttributeValue = IIf(IsDBNull(dr("AttributeValue")), String.Empty, dr("AttributeValue"))
				attr.ImageName = IIf(IsDBNull(dr("ImageName")), String.Empty, dr("ImageName"))
				attr.ImageAlt = IIf(IsDBNull(dr("ImageAlt")), String.Empty, dr("ImageAlt"))
				attr.ParentAttributeId = IIf(IsDBNull(dr("ParentAttributeId")), Nothing, dr("ParentAttributeId"))
				attr.ProductAlt = IIf(IsDBNull(dr("ProductAlt")), String.Empty, dr("ProductAlt"))
				attr.ProductImage = IIf(IsDBNull(dr("ProductImage")), String.Empty, dr("ProductImage"))
				attr.Weight = IIf(IsDBNull(dr("Weight")), 0, dr("Weight"))
                attr.ItemAttributeId = IIf(IsDBNull(dr("ItemAttributeId")), 0, dr("ItemAttributeId"))
                attr.ItemId = IIf(IsDBNull(dr("ItemId")), 0, dr("ItemId"))
                attr.Price = IIf(IsDBNull(dr("Price")), 0, dr("Price"))
                attr.SKU = IIf(IsDBNull(dr("SKU")), String.Empty, dr("SKU"))
                attr.SortOrder = IIf(IsDBNull(dr("SortOrder")), 0, dr("SortOrder"))
                attr.TemplateAttributeId = IIf(IsDBNull(dr("TemplateAttributeId")), 0, dr("TemplateAttributeId"))
                col.Add(attr)
            End While
            dr.Close()
            Return col
        End Function
    End Class

    Public MustInherit Class StoreOrderItemRowBase
        Private m_DB As Database
        Private m_OrderItemId As Integer = Nothing
        Private m_OrderId As Integer = Nothing
        Private m_RecipientId As Integer = Nothing
        Private m_ItemId As Integer = Nothing
        Private m_DepartmentId As Integer = Nothing
        Private m_BrandId As Integer = Nothing
        Private m_TemplateId As Integer = Nothing
        Private m_SKU As String = Nothing
        Private m_ItemName As String = Nothing
        Private m_ItemUnit As String = Nothing
        Private m_CustomURL As String = Nothing
        Private m_AttributeString As String = Nothing
        Private m_Image As String = Nothing
        Private m_Width As Double = Nothing
        Private m_Height As Double = Nothing
        Private m_Thickness As Double = Nothing
        Private m_Weight As Double = Nothing
        Private m_Quantity As Integer = Nothing
        Private m_GiftQuantity As Integer = Nothing
        Private m_ItemPrice As Double = Nothing
        Private m_SalePrice As Double = Nothing
        Private m_Shipping1 As Double = Nothing
        Private m_Shipping2 As Double = Nothing
        Private m_CountryUnit As Integer = Nothing
        Private m_Price As Double = Nothing
        Private m_Discount As Double = Nothing
        Private m_IsOnSale As Boolean = Nothing
        Private m_IsTaxFree As Boolean = Nothing
        Private m_IsFeatured As Boolean = Nothing
        Private m_IsGiftWrap As Boolean = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_ModifyDate As DateTime = Nothing
        Private m_Status As String = Nothing
        Private m_ShippedDate As DateTime = Nothing
        Private m_TrackingNumber As String = Nothing



        Public Property OrderItemId() As Integer
            Get
                Return m_OrderItemId
            End Get
            Set(ByVal Value As Integer)
                m_OrderItemId = Value
            End Set
        End Property

        Public Property OrderId() As Integer
            Get
                Return m_OrderId
            End Get
            Set(ByVal Value As Integer)
                m_OrderId = Value
            End Set
        End Property

        Public Property RecipientId() As Integer
            Get
                Return m_RecipientId
            End Get
            Set(ByVal Value As Integer)
                m_RecipientId = Value
            End Set
        End Property

        Public Property ItemId() As Integer
            Get
                Return m_ItemId
            End Get
            Set(ByVal Value As Integer)
                m_ItemId = Value
            End Set
        End Property

        Public Property DepartmentId() As Integer
            Get
                Return m_DepartmentId
            End Get
            Set(ByVal Value As Integer)
                m_DepartmentId = Value
            End Set
        End Property

        Public Property BrandId() As Integer
            Get
                Return m_BrandId
            End Get
            Set(ByVal Value As Integer)
                m_BrandId = Value
            End Set
        End Property

        Public Property TemplateId() As Integer
            Get
                Return m_TemplateId
            End Get
            Set(ByVal Value As Integer)
                m_TemplateId = Value
            End Set
        End Property

        Public Property SKU() As String
            Get
                Return m_SKU
            End Get
            Set(ByVal Value As String)
                m_SKU = Value
            End Set
        End Property

        Public Property ItemName() As String
            Get
                Return m_ItemName
            End Get
            Set(ByVal Value As String)
                m_ItemName = Value
            End Set
        End Property

        Public Property ItemUnit() As String
            Get
                Return m_ItemUnit
            End Get
            Set(ByVal Value As String)
                m_ItemUnit = Value
            End Set
        End Property

        Public Property CustomURL() As String
            Get
                Return m_CustomURL
            End Get
            Set(ByVal Value As String)
                m_CustomURL = Value
            End Set
        End Property

        Public Property AttributeString() As String
            Get
                Return m_AttributeString
            End Get
            Set(ByVal Value As String)
                m_AttributeString = Value
            End Set
        End Property

        Public Property Image() As String
            Get
                Return m_Image
            End Get
            Set(ByVal Value As String)
                m_Image = Value
            End Set
        End Property

        Public Property Width() As Double
            Get
                Return m_Width
            End Get
            Set(ByVal Value As Double)
                m_Width = Value
            End Set
        End Property

        Public Property Height() As Double
            Get
                Return m_Height
            End Get
            Set(ByVal Value As Double)
                m_Height = Value
            End Set
        End Property

        Public Property Thickness() As Double
            Get
                Return m_Thickness
            End Get
            Set(ByVal Value As Double)
                m_Thickness = Value
            End Set
        End Property

        Public Property Weight() As Double
            Get
                Return m_Weight
            End Get
            Set(ByVal Value As Double)
                m_Weight = Value
            End Set
        End Property

        Public Property Quantity() As Integer
            Get
                Return m_Quantity
            End Get
            Set(ByVal Value As Integer)
                m_Quantity = Value
            End Set
        End Property

        Public Property GiftQuantity() As Integer
            Get
                Return m_GiftQuantity
            End Get
            Set(ByVal Value As Integer)
                m_GiftQuantity = Value
            End Set
        End Property

        Public Property ItemPrice() As Double
            Get
                Return m_ItemPrice
            End Get
            Set(ByVal Value As Double)
                m_ItemPrice = Value
            End Set
        End Property

        Public Property SalePrice() As Double
            Get
                Return m_SalePrice
            End Get
            Set(ByVal Value As Double)
                m_SalePrice = Value
            End Set
        End Property

        Public Property Shipping1() As Double
            Get
                Return m_Shipping1
            End Get
            Set(ByVal Value As Double)
                m_Shipping1 = Value
            End Set
        End Property

        Public Property Shipping2() As Double
            Get
                Return m_Shipping2
            End Get
            Set(ByVal Value As Double)
                m_Shipping2 = Value
            End Set
        End Property

        Public Property CountryUnit() As Integer
            Get
                Return m_CountryUnit
            End Get
            Set(ByVal Value As Integer)
                m_CountryUnit = Value
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

        Public Property Discount() As Double
            Get
                Return m_Discount
            End Get
            Set(ByVal Value As Double)
                m_Discount = Value
            End Set
        End Property

        Public Property IsOnSale() As Boolean
            Get
                Return m_IsOnSale
            End Get
            Set(ByVal Value As Boolean)
                m_IsOnSale = Value
            End Set
        End Property

        Public Property IsTaxFree() As Boolean
            Get
                Return m_IsTaxFree
            End Get
            Set(ByVal Value As Boolean)
                m_IsTaxFree = Value
            End Set
        End Property

        Public Property IsFeatured() As Boolean
            Get
                Return m_IsFeatured
            End Get
            Set(ByVal Value As Boolean)
                m_IsFeatured = Value
            End Set
        End Property

        Public Property IsGiftWrap() As Boolean
            Get
                Return m_IsGiftWrap
            End Get
            Set(ByVal Value As Boolean)
                m_IsGiftWrap = Value
            End Set
        End Property

        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
        End Property

        Public ReadOnly Property ModifyDate() As DateTime
            Get
                Return m_ModifyDate
            End Get
        End Property

        Public Property Status() As String
            Get
                Return m_Status
            End Get
            Set(ByVal value As String)
                m_Status = value
            End Set
        End Property

        Public Property ShippedDate() As DateTime
            Get
                Return m_ShippedDate
            End Get
            Set(ByVal value As DateTime)
                m_ShippedDate = value
            End Set
        End Property

        Public Property TrackingNumber() As String
            Get
                Return m_TrackingNumber
            End Get
            Set(ByVal value As String)
                m_TrackingNumber = value
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
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal OrderItemId As Integer)
            m_DB = DB
            m_OrderItemId = OrderItemId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM StoreOrderItem WHERE OrderItemId = " & DB.Number(OrderItemId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_OrderItemId = Convert.ToInt32(r.Item("OrderItemId"))
            m_OrderId = Convert.ToInt32(r.Item("OrderId"))
            m_RecipientId = Convert.ToInt32(r.Item("RecipientId"))
            m_ItemId = Convert.ToInt32(r.Item("ItemId"))
            If IsDBNull(r.Item("DepartmentId")) Then
                m_DepartmentId = Nothing
            Else
                m_DepartmentId = Convert.ToInt32(r.Item("DepartmentId"))
            End If
            If IsDBNull(r.Item("BrandId")) Then
                m_BrandId = Nothing
            Else
                m_BrandId = Convert.ToInt32(r.Item("BrandId"))
            End If
            If IsDBNull(r.Item("TemplateId")) Then
                m_TemplateId = Nothing
            Else
                m_TemplateId = Convert.ToInt32(r.Item("TemplateId"))
            End If
            m_SKU = Convert.ToString(r.Item("SKU"))
            m_ItemName = Convert.ToString(r.Item("ItemName"))
            If IsDBNull(r.Item("ItemUnit")) Then
                m_ItemUnit = Nothing
            Else
                m_ItemUnit = Convert.ToString(r.Item("ItemUnit"))
            End If
            If IsDBNull(r.Item("CustomURL")) Then
                m_CustomURL = Nothing
            Else
                m_CustomURL = Convert.ToString(r.Item("CustomURL"))
            End If
            If IsDBNull(r.Item("AttributeString")) Then
                m_AttributeString = Nothing
            Else
                m_AttributeString = Convert.ToString(r.Item("AttributeString"))
            End If
            If IsDBNull(r.Item("Image")) Then
                m_Image = Nothing
            Else
                m_Image = Convert.ToString(r.Item("Image"))
            End If
            If IsDBNull(r.Item("Width")) Then
                m_Width = Nothing
            Else
                m_Width = Convert.ToDouble(r.Item("Width"))
            End If
            If IsDBNull(r.Item("Height")) Then
                m_Height = Nothing
            Else
                m_Height = Convert.ToDouble(r.Item("Height"))
            End If
            If IsDBNull(r.Item("Thickness")) Then
                m_Thickness = Nothing
            Else
                m_Thickness = Convert.ToDouble(r.Item("Thickness"))
            End If
            If IsDBNull(r.Item("Weight")) Then
                m_Weight = Nothing
            Else
                m_Weight = Convert.ToDouble(r.Item("Weight"))
            End If
            m_Quantity = Convert.ToInt32(r.Item("Quantity"))
            m_GiftQuantity = Convert.ToInt32(r.Item("GiftQuantity"))
            m_ItemPrice = Convert.ToDouble(r.Item("ItemPrice"))
            If IsDBNull(r.Item("SalePrice")) Then
                m_SalePrice = Nothing
            Else
                m_SalePrice = Convert.ToDouble(r.Item("SalePrice"))
            End If
            If IsDBNull(r.Item("Shipping1")) Then
                m_Shipping1 = Nothing
            Else
                m_Shipping1 = Convert.ToDouble(r.Item("Shipping1"))
            End If
            If IsDBNull(r.Item("Shipping2")) Then
                m_Shipping2 = Nothing
            Else
                m_Shipping2 = Convert.ToDouble(r.Item("Shipping2"))
            End If
            If IsDBNull(r.Item("CountryUnit")) Then
                m_CountryUnit = Nothing
            Else
                m_CountryUnit = Convert.ToInt32(r.Item("CountryUnit"))
            End If
            m_Price = Convert.ToDouble(r.Item("Price"))
            m_Discount = Convert.ToDouble(r.Item("Discount"))
            m_IsOnSale = Convert.ToBoolean(r.Item("IsOnSale"))
            m_IsTaxFree = Convert.ToBoolean(r.Item("IsTaxFree"))
            m_IsFeatured = Convert.ToBoolean(r.Item("IsFeatured"))
            m_IsGiftWrap = Convert.ToBoolean(r.Item("IsGiftWrap"))
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            If IsDBNull(r.Item("ModifyDate")) Then
                m_ModifyDate = Nothing
            Else
                m_ModifyDate = Convert.ToDateTime(r.Item("ModifyDate"))
            End If
            If IsDBNull(r.Item("TrackingNumber")) Then
                m_TrackingNumber = Nothing
            Else
                m_TrackingNumber = Convert.ToString(r.Item("TrackingNumber"))
            End If
            If IsDBNull(r.Item("Status")) Then
                m_Status = Nothing
            Else
                m_Status = Convert.ToString(r.Item("Status"))
            End If
            If IsDBNull(r.Item("ShippedDate")) Then
                m_ShippedDate = Nothing
            Else
                m_ShippedDate = Convert.ToDateTime(r.Item("ShippedDate"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO StoreOrderItem (" _
             & " OrderId" _
             & ",RecipientId" _
             & ",ItemId" _
             & ",DepartmentId" _
             & ",BrandId" _
             & ",TemplateId" _
             & ",SKU" _
             & ",ItemName" _
             & ",ItemUnit" _
             & ",CustomURL" _
             & ",AttributeString" _
             & ",Image" _
             & ",Width" _
             & ",Height" _
             & ",Thickness" _
             & ",Weight" _
             & ",Quantity" _
             & ",GiftQuantity" _
             & ",ItemPrice" _
             & ",SalePrice" _
             & ",Shipping1" _
             & ",Shipping2" _
             & ",CountryUnit" _
             & ",Price" _
             & ",Discount" _
             & ",IsOnSale" _
             & ",IsTaxFree" _
             & ",IsFeatured" _
             & ",IsGiftWrap" _
             & ",CreateDate" _
             & ",ModifyDate" _
             & ",ShippedDate" _
             & ",TrackingNumber" _
             & ",Status" _
             & ") VALUES (" _
             & m_DB.NullNumber(OrderId) _
             & "," & m_DB.NullNumber(RecipientId) _
             & "," & m_DB.NullNumber(ItemId) _
             & "," & m_DB.NullNumber(DepartmentId) _
             & "," & m_DB.NullNumber(BrandId) _
             & "," & m_DB.NullNumber(TemplateId) _
             & "," & m_DB.Quote(SKU) _
             & "," & m_DB.Quote(ItemName) _
             & "," & m_DB.Quote(ItemUnit) _
             & "," & m_DB.Quote(CustomURL) _
             & "," & m_DB.Quote(AttributeString) _
             & "," & m_DB.Quote(Image) _
             & "," & m_DB.Number(Width) _
             & "," & m_DB.Number(Height) _
             & "," & m_DB.Number(Thickness) _
             & "," & m_DB.Number(Weight) _
             & "," & m_DB.Number(Quantity) _
             & "," & m_DB.Number(GiftQuantity) _
             & "," & m_DB.Number(ItemPrice) _
             & "," & m_DB.NullNumber(SalePrice) _
             & "," & m_DB.NullNumber(Shipping1) _
             & "," & m_DB.NullNumber(Shipping2) _
             & "," & m_DB.NullNumber(CountryUnit) _
             & "," & m_DB.Number(Price) _
             & "," & m_DB.Number(Discount) _
             & "," & CInt(IsOnSale) _
             & "," & CInt(IsTaxFree) _
             & "," & CInt(IsFeatured) _
             & "," & CInt(IsGiftWrap) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.Quote(ShippedDate) _
             & "," & m_DB.Quote(TrackingNumber) _
             & "," & m_DB.Quote(Status) _
             & ")"

            OrderItemId = m_DB.InsertSQL(SQL)

            Return OrderItemId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE StoreOrderItem SET " _
             & " OrderId = " & m_DB.NullNumber(OrderId) _
             & ",RecipientId = " & m_DB.NullNumber(RecipientId) _
             & ",ItemId = " & m_DB.NullNumber(ItemId) _
             & ",DepartmentId = " & m_DB.NullNumber(DepartmentId) _
             & ",BrandId = " & m_DB.NullNumber(BrandId) _
             & ",TemplateId = " & m_DB.NullNumber(TemplateId) _
             & ",SKU = " & m_DB.Quote(SKU) _
             & ",ItemName = " & m_DB.Quote(ItemName) _
             & ",ItemUnit = " & m_DB.Quote(ItemUnit) _
             & ",CustomURL = " & m_DB.Quote(CustomURL) _
             & ",AttributeString = " & m_DB.Quote(AttributeString) _
             & ",Image = " & m_DB.Quote(Image) _
             & ",Width = " & m_DB.Number(Width) _
             & ",Height = " & m_DB.Number(Height) _
             & ",Thickness = " & m_DB.Number(Thickness) _
             & ",Weight = " & m_DB.Number(Weight) _
             & ",Quantity = " & m_DB.Number(Quantity) _
             & ",GiftQuantity = " & m_DB.Number(GiftQuantity) _
             & ",ItemPrice = " & m_DB.Number(ItemPrice) _
             & ",SalePrice = " & m_DB.NullNumber(SalePrice) _
             & ",Shipping1 = " & m_DB.NullNumber(Shipping1) _
             & ",Shipping2 = " & m_DB.NullNumber(Shipping2) _
             & ",CountryUnit = " & m_DB.NullNumber(CountryUnit) _
             & ",Price = " & m_DB.Number(Price) _
             & ",Discount = " & m_DB.Number(Discount) _
             & ",IsOnSale = " & CInt(IsOnSale) _
             & ",IsTaxFree = " & CInt(IsTaxFree) _
             & ",IsFeatured = " & CInt(IsFeatured) _
             & ",IsGiftWrap = " & CInt(IsGiftWrap) _
             & ",ModifyDate = " & m_DB.NullQuote(Now) _
             & ",ShippedDate = " & m_DB.Quote(ShippedDate) _
             & ",TrackingNumber = " & m_DB.Quote(TrackingNumber) _
             & ",Status = " & m_DB.Quote(Status) _
             & " WHERE OrderItemId = " & m_DB.Quote(OrderItemId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM StoreOrderItem WHERE OrderItemId = " & m_DB.Quote(OrderItemId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class StoreOrderItemCollection
        Inherits GenericCollection(Of StoreOrderItemRow)
    End Class

End Namespace