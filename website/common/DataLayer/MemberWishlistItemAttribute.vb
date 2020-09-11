Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class MemberWishlistItemAttributeRow
        Inherits MemberWishlistItemAttributeRowBase

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
        Public Shared Function GetRow(ByVal DB As Database, ByVal UniqueId As Integer) As MemberWishlistItemAttributeRow
            Dim row As MemberWishlistItemAttributeRow

            row = New MemberWishlistItemAttributeRow(DB, UniqueId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal UniqueId As Integer)
            Dim row As MemberWishlistItemAttributeRow

            row = New MemberWishlistItemAttributeRow(DB, UniqueId)
            row.Remove()
        End Sub

        'Custom Methods

    End Class

    Public MustInherit Class MemberWishlistItemAttributeRowBase
        Private m_DB As Database
        Private m_UniqueId As Integer = Nothing
        Private m_MemberId As Integer = Nothing
        Private m_WishlistItemId As Integer = Nothing
        Private m_ItemId As Integer = Nothing
		Private m_ItemAttributeId As Integer = Nothing
		Private m_ParentAttributeId As Integer = Nothing
		Private m_TemplateAttributeId As Integer = Nothing
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

        Public Property MemberId() As Integer
            Get
                Return m_MemberId
            End Get
            Set(ByVal Value As Integer)
                m_MemberId = Value
            End Set
        End Property

        Public Property WishlistItemId() As Integer
            Get
                Return m_WishlistItemId
            End Get
            Set(ByVal Value As Integer)
                m_WishlistItemId = value
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

		Public Property ParentAttributeId() As Integer
			Get
				Return m_ParentAttributeId
			End Get
			Set(ByVal value As Integer)
				m_ParentAttributeId = value
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
                m_SortOrder = value
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

            SQL = "SELECT * FROM MemberWishlistItemAttribute WHERE UniqueId = " & DB.Number(UniqueId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_UniqueId = Convert.ToInt32(r.Item("UniqueId"))
            m_MemberId = Convert.ToInt32(r.Item("MemberId"))
            m_WishlistItemId = Convert.ToInt32(r.Item("WishlistItemId"))
            m_ItemId = Convert.ToInt32(r.Item("ItemId"))
			m_ItemAttributeId = Convert.ToInt32(r.Item("ItemAttributeId"))
			If IsDBNull(r.Item("ParentAttributeId")) Then
				m_ParentAttributeId = Nothing
			Else
				m_ParentAttributeId = Convert.ToInt32(r.Item("ParentAttributeId"))
			End If
            m_TemplateAttributeId = Convert.ToInt32(r.Item("TemplateAttributeId"))
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
		End Sub	'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

			SQL = " INSERT INTO MemberWishlistItemAttribute (" _
			 & " MemberId" _
			 & ",WishlistItemId" _
			 & ",ItemId" _
			 & ",ItemAttributeId" _
			 & ",ParentAttributeId" _
			 & ",TemplateAttributeId" _
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
			 & m_DB.NullNumber(MemberId) _
			 & "," & m_DB.NullNumber(WishlistItemId) _
			 & "," & m_DB.NullNumber(ItemId) _
			 & "," & m_DB.NullNumber(ItemAttributeId) _
			 & "," & m_DB.NullNumber(ParentAttributeId) _
			 & "," & m_DB.NullNumber(TemplateAttributeId) _
			 & "," & m_DB.Quote(AttributeValue) _
			 & "," & m_DB.Quote(SKU) _
			 & "," & m_DB.Number(Price) _
			 & "," & m_DB.Number(Weight) _
			 & "," & m_DB.Quote(ImageName) _
			 & "," & m_DB.Quote(ProductImage) _
			 & "," & m_DB.Quote(ImageAlt) _
			 & "," & m_DB.Quote(ProductAlt) _
			 & "," & m_DB.NullNumber(SortOrder) _
			 & ")"

            UniqueId = m_DB.InsertSQL(SQL)

            Return UniqueId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

			SQL = " UPDATE MemberWishlistItemAttribute SET " _
			 & " MemberId = " & m_DB.NullNumber(MemberId) _
			 & ",WishlistItemId = " & m_DB.NullNumber(WishlistItemId) _
			 & ",ItemId = " & m_DB.NullNumber(ItemId) _
			 & ",ItemAttributeId = " & m_DB.NullNumber(ItemAttributeId) _
			 & ",ParentAttributeId = " & m_DB.NullNumber(ParentAttributeId) _
			 & ",TemplateAttributeId = " & m_DB.NullNumber(TemplateAttributeId) _
			 & ",AttributeValue = " & m_DB.Quote(AttributeValue) _
			 & ",SKU = " & m_DB.Quote(SKU) _
			 & ",Price = " & m_DB.Number(Price) _
			 & ",Weight = " & m_DB.Number(Weight) _
			 & ",ImageName = " & m_DB.Quote(ImageName) _
			 & ",ProductImage = " & m_DB.Quote(ProductImage) _
			 & ",ImageAlt = " & m_DB.Quote(ImageAlt) _
			 & ",ProductAlt = " & m_DB.Quote(ProductAlt) _
			 & ",SortOrder = " & m_DB.NullNumber(SortOrder) _
			 & " WHERE UniqueId = " & m_DB.Quote(UniqueId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM MemberWishlistItemAttribute WHERE UniqueId = " & m_DB.Number(UniqueId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class MemberWishlistItemAttributeCollection
        Inherits GenericCollection(Of MemberWishlistItemAttributeRow)
    End Class

End Namespace

