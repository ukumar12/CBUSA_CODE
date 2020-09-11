Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class StoreOrderItemAttributeRow
        Inherits StoreOrderItemAttributeRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal UniqueId As Integer)
            MyBase.New(DB, UniqueId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal UniqueId As Integer) As StoreOrderItemAttributeRow
            Dim row As StoreOrderItemAttributeRow

            row = New StoreOrderItemAttributeRow(DB, UniqueId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal UniqueId As Integer)
            Dim row As StoreOrderItemAttributeRow

            row = New StoreOrderItemAttributeRow(DB, UniqueId)
            row.Remove()
        End Sub

        'Custom Methods

    End Class

    Public MustInherit Class StoreOrderItemAttributeRowBase
        Private m_DB As Database
        Private m_UniqueId As Integer = Nothing
        Private m_OrderId As Integer = Nothing
        Private m_ItemAttributeId As Integer = Nothing
		Private m_ParentAttributeId As Integer = Nothing
        Private m_OrderItemId As Integer = Nothing
        Private m_TemplateAttributeId As Integer = Nothing
        Private m_ItemId As Integer = Nothing
        Private m_AttributeValue As String = Nothing
        Private m_SKU As String = Nothing
        Private m_Price As Double = Nothing
		Private m_Weight As Double = Nothing
		Private m_ImageName As String = Nothing
		Private m_ImageAlt As String = Nothing
		Private m_ProductImage As String = Nothing
		Private m_ProductAlt As String = Nothing
        Private m_SortOrder As Integer = Nothing


        Public Property UniqueId() As Integer
            Get
                Return m_UniqueId
            End Get
            Set(ByVal Value As Integer)
                m_UniqueId = value
            End Set
        End Property

        Public Property OrderId() As Integer
            Get
                Return m_OrderId
            End Get
            Set(ByVal Value As Integer)
                m_OrderId = value
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

		Public Property ParentAttributeId() As Integer
			Get
				Return m_ParentAttributeId
			End Get
			Set(ByVal value As Integer)
				m_ParentAttributeId = value
			End Set
		End Property

        Public Property OrderItemId() As Integer
            Get
                Return m_OrderItemId
            End Get
            Set(ByVal Value As Integer)
                m_OrderItemId = value
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

        Public Sub New(ByVal DB As Database, ByVal UniqueId As Integer)
            m_DB = DB
            m_UniqueId = UniqueId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM StoreOrderItemAttribute WHERE UniqueId = " & DB.Number(UniqueId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_UniqueId = Convert.ToInt32(r.Item("UniqueId"))
            m_OrderId = Convert.ToInt32(r.Item("OrderId"))
            m_ItemAttributeId = Convert.ToInt32(r.Item("ItemAttributeId"))
            m_OrderItemId = Convert.ToInt32(r.Item("OrderItemId"))
            m_TemplateAttributeId = Convert.ToInt32(r.Item("TemplateAttributeId"))
            m_ItemId = Convert.ToInt32(r.Item("ItemId"))
            m_AttributeValue = Convert.ToString(r.Item("AttributeValue"))
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
			If IsDBNull(r.Item("ParentAttributeId")) Then
				m_ParentAttributeId = Nothing
			Else
				m_ParentAttributeId = Convert.ToInt32(r.Item("ParentAttributeId"))
			End If
			m_Weight = Convert.ToDouble(r.Item("Weight"))
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
			m_SortOrder = Convert.ToInt32(r.Item("SortOrder"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from StoreOrderItemAttribute order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO StoreOrderItemAttribute (" _
             & " OrderId" _
             & ",ItemAttributeId" _
			 & ",ParentAttributeId" _
             & ",OrderItemId" _
             & ",TemplateAttributeId" _
             & ",ItemId" _
             & ",AttributeValue" _
             & ",SKU" _
             & ",Price" _
			 & ",Weight" _
			 & ",ImageName" _
			 & ",ProductImage" _
			 & ",ImageAlt" _
			 & ",ProductAlt" _
             & ",SortOrder" _
             & ") VALUES (" _
             & m_DB.NullNumber(OrderId) _
             & "," & m_DB.NullNumber(ItemAttributeId) _
			 & "," & m_DB.NullNumber(ParentAttributeId) _
             & "," & m_DB.NullNumber(OrderItemId) _
             & "," & m_DB.NullNumber(TemplateAttributeId) _
             & "," & m_DB.NullNumber(ItemId) _
             & "," & m_DB.Quote(AttributeValue) _
             & "," & m_DB.Quote(SKU) _
             & "," & m_DB.Number(Price) _
			 & "," & m_DB.Number(Weight) _
			 & "," & m_DB.Quote(ImageName) _
			 & "," & m_DB.Quote(ProductImage) _
			 & "," & m_DB.Quote(ImageAlt) _
			 & "," & m_DB.Quote(ProductAlt) _
             & "," & MaxSortOrder _
             & ")"

            UniqueId = m_DB.InsertSQL(SQL)

            Return UniqueId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE StoreOrderItemAttribute SET " _
             & " OrderId = " & m_DB.NullNumber(OrderId) _
             & ",ItemAttributeId = " & m_DB.NullNumber(ItemAttributeId) _
			 & ",ParentAttributeId = " & m_DB.NullNumber(ParentAttributeId) _
             & ",OrderItemId = " & m_DB.NullNumber(OrderItemId) _
             & ",TemplateAttributeId = " & m_DB.NullNumber(TemplateAttributeId) _
             & ",ItemId = " & m_DB.NullNumber(ItemId) _
             & ",AttributeValue = " & m_DB.Quote(AttributeValue) _
             & ",SKU = " & m_DB.Quote(SKU) _
             & ",Price = " & m_DB.Number(Price) _
			 & ",Weight = " & m_DB.Number(Weight) _
			 & ",ImageName = " & m_DB.Quote(ImageName) _
			 & ",ProductImage = " & m_DB.Quote(ProductImage) _
			 & ",ImageAlt = " & m_DB.Quote(ImageAlt) _
			 & ",ProductAlt = " & m_DB.Quote(ProductAlt) _
			 & " WHERE UniqueId = " & m_DB.Quote(UniqueId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM StoreOrderItemAttribute WHERE UniqueId = " & m_DB.Quote(UniqueId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class StoreOrderItemAttributeCollection
        Inherits GenericCollection(Of StoreOrderItemAttributeRow)
    End Class

End Namespace


