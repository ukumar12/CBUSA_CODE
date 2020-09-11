Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class MemberWishlistItemRow
        Inherits MemberWishlistItemRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal WishlistItemId As Integer)
            MyBase.New(DB, WishlistItemId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal WishlistItemId As Integer) As MemberWishlistItemRow
            Dim row As MemberWishlistItemRow

            row = New MemberWishlistItemRow(DB, WishlistItemId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal WishlistItemId As Integer)
            Dim row As MemberWishlistItemRow

            row = New MemberWishlistItemRow(DB, WishlistItemId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Function InsertAttribute(ByVal attr As ItemAttribute) As Integer
            Dim dbMemberWishlistItemAttribute As MemberWishlistItemAttributeRow = New MemberWishlistItemAttributeRow(DB)
            dbMemberWishlistItemAttribute.memberid = MemberId
            dbMemberWishlistItemAttribute.WishlistItemId = WishlistItemId
            dbMemberWishlistItemAttribute.ItemAttributeId = attr.ItemAttributeId
            dbMemberWishlistItemAttribute.ItemId = attr.ItemId
            dbMemberWishlistItemAttribute.SortOrder = attr.SortOrder
            dbMemberWishlistItemAttribute.AttributeValue = attr.AttributeValue
            dbMemberWishlistItemAttribute.Price = attr.Price
            dbMemberWishlistItemAttribute.SKU = attr.SKU
			dbMemberWishlistItemAttribute.Weight = attr.Weight
			dbMemberWishlistItemAttribute.ImageName = attr.ImageName
			dbMemberWishlistItemAttribute.ImageAlt = attr.ImageAlt
			dbMemberWishlistItemAttribute.ProductImage = attr.ProductImage
			dbMemberWishlistItemAttribute.ProductAlt = attr.ProductAlt
			dbMemberWishlistItemAttribute.TemplateAttributeId = attr.TemplateAttributeId
            Return dbMemberWishlistItemAttribute.Insert()
        End Function

        Public Function GetItemAttributeCollection() As ItemAttributeCollection
            Return GetItemAttributeCollection(DB, WishlistItemId)
        End Function

        Public Shared Function GetItemAttributeCollection(ByVal DB As Database, ByVal WishlistItemId As Integer) As ItemAttributeCollection
            Dim col As New ItemAttributeCollection
            Dim SQL = "select mwia.*, sita.AttributeType, sita.AttributeName from MemberWishlistItemAttribute mwia, StoreItemTemplateAttribute sita where mwia.TemplateAttributeId = sita.TemplateAttributeId and mwia.WishlistItemId = " & WishlistItemId & " order by mwia.SortOrder"
            Dim dr As SqlDataReader = DB.GetReader(SQL)
            While dr.Read
                Dim attr As New ItemAttribute
                attr.AttributeType = IIf(IsDBNull(dr("AttributeType")), String.Empty, dr("AttributeType"))
                attr.AttributeName = IIf(IsDBNull(dr("AttributeName")), String.Empty, dr("AttributeName"))
                attr.AttributeValue = IIf(IsDBNull(dr("AttributeValue")), String.Empty, dr("AttributeValue"))
				attr.ImageName = IIf(IsDBNull(dr("ImageName")), String.Empty, dr("ImageName"))
				attr.ImageAlt = IIf(IsDBNull(dr("ImageAlt")), String.Empty, dr("ImageAlt"))
				attr.ParentAttributeId = IIf(IsDBNull(dr("ParentAttributeId")), 0, dr("ParentAttributeId"))
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

    Public MustInherit Class MemberWishlistItemRowBase
        Private m_DB As Database
        Private m_WishlistItemId As Integer = Nothing
        Private m_MemberId As Integer = Nothing
        Private m_ItemId As Integer = Nothing
        Private m_Quantity As Integer = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_ModifyDate As DateTime = Nothing
        Private m_AttributeString As String = Nothing


        Public Property WishlistItemId() As Integer
            Get
                Return m_WishlistItemId
            End Get
            Set(ByVal Value As Integer)
                m_WishlistItemId = value
            End Set
        End Property

        Public Property MemberId() As Integer
            Get
                Return m_MemberId
            End Get
            Set(ByVal Value As Integer)
                m_MemberId = value
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

        Public Property Quantity() As Integer
            Get
                Return m_Quantity
            End Get
            Set(ByVal Value As Integer)
                m_Quantity = value
            End Set
        End Property

        Public Property AttributeString() As String
            Get
                Return m_AttributeString
            End Get
            Set(ByVal Value As String)
                m_AttributeString = value
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

        Public Sub New(ByVal DB As Database, ByVal WishlistItemId As Integer)
            m_DB = DB
            m_WishlistItemId = WishlistItemId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM MemberWishlistItem WHERE WishlistItemId = " & DB.Number(WishlistItemId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_WishlistItemId = Convert.ToInt32(r.Item("WishlistItemId"))
            m_MemberId = Convert.ToInt32(r.Item("MemberId"))
            m_ItemId = Convert.ToInt32(r.Item("ItemId"))
            m_Quantity = Convert.ToInt32(r.Item("Quantity"))
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            If IsDBNull(r.Item("ModifyDate")) Then
                m_ModifyDate = Nothing
            Else
                m_ModifyDate = Convert.ToDateTime(r.Item("ModifyDate"))
            End If
            If IsDBNull(r.Item("AttributeString")) Then
                m_AttributeString = Nothing
            Else
                m_AttributeString = Convert.ToString(r.Item("AttributeString"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO MemberWishlistItem (" _
             & " MemberId" _
             & ",ItemId" _
             & ",Quantity" _
             & ",CreateDate" _
             & ",ModifyDate" _
             & ",AttributeString" _
             & ") VALUES (" _
             & m_DB.NullNumber(MemberId) _
             & "," & m_DB.NullNumber(ItemId) _
             & "," & m_DB.Number(Quantity) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.Quote(AttributeString) _
             & ")"

            WishlistItemId = m_DB.InsertSQL(SQL)

            Return WishlistItemId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE MemberWishlistItem SET " _
             & " MemberId = " & m_DB.NullNumber(MemberId) _
             & ",ItemId = " & m_DB.NullNumber(ItemId) _
             & ",Quantity = " & m_DB.Number(Quantity) _
             & ",ModifyDate = " & m_DB.NullQuote(Now) _
             & ",AttributeString = " & m_DB.Quote(AttributeString) _
             & " WHERE WishlistItemId = " & m_DB.quote(WishlistItemId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM MemberWishlistItem WHERE WishlistItemId = " & m_DB.Number(WishlistItemId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class MemberWishlistItemCollection
        Inherits GenericCollection(Of MemberWishlistItemRow)
    End Class

End Namespace

