Imports System.Collections
Imports DataLayer
Imports System.Web.SessionState
Imports Database
Imports Components
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Configuration.ConfigurationManager

Namespace Components
    Public Class Wishlist

        Public Shared Function GetAttributeText(ByVal Db As Database, ByVal OrderItemId As Integer) As String
            Dim attributes As ItemAttributeCollection = MemberWishlistItemRow.GetItemAttributeCollection(Db, OrderItemId)
            Dim Result As String = String.Empty
            Dim Conn As String = String.Empty
            For Each attr As ItemAttribute In attributes
                Result &= Conn & attr.AttributeName & "=" & attr.AttributeValue
                Conn = "<br />"
            Next
            Return Result
        End Function

        Public Shared Sub Add2WishlistFromCart(ByVal DB As Database, ByVal MemberId As Integer, ByVal OrderId As Integer, ByVal OrderItemId As Integer)
            Dim dbOrderItem As StoreOrderItemRow = StoreOrderItemRow.GetRow(DB, OrderItemId)
            If dbOrderItem.OrderId <> OrderId Then
                Exit Sub
            End If
            Add2Wishlist(DB, MemberId, dbOrderItem.ItemId, dbOrderItem.Quantity, dbOrderItem.GetItemAttributeCollection())
        End Sub

        Public Shared Sub Add2Wishlist(ByVal DB As Database, ByVal MemberId As Integer, ByVal ItemId As Integer, ByVal Quantity As Integer, Optional ByVal ItemAttributes As ItemAttributeCollection = Nothing)
            If ItemId = 0 Then Throw New ApplicationException("Invalid Parameters")
            If Quantity = 0 Then Exit Sub

            Dim SQL As String = String.Empty
            Dim AttributeString As String = String.Empty
            Dim Conn As String = String.Empty

            If Not ItemAttributes Is Nothing Then
                For Each attr As ItemAttribute In ItemAttributes
                    AttributeString &= Conn & attr.TemplateAttributeId & attr.AttributeValue
                    Conn = ","
                Next
            End If
            Logger.Info("Attribute String = " & AttributeString)

            SQL = String.Empty
            SQL &= " update MemberWishlistItem WITH (ROWLOCK) set Quantity = Quantity + " & Quantity & ", ModifyDate = " & DB.Quote(Now)
            SQL &= " WHERE MemberId = " & MemberId
            SQL &= "   AND ItemId = " & ItemId
            If AttributeString = String.Empty Then
                SQL &= "   AND AttributeString is NULL"
            Else
                SQL &= "   AND AttributeString  = " & DB.Quote(AttributeString)
            End If

            Dim RowsAffected As Integer = DB.ExecuteSQL(SQL)
            If RowsAffected <> 1 Then
                'Insert all attributes
                Dim dbStoreItem As StoreItemRow = StoreItemRow.GetRow(DB, ItemId)
                Dim dbWishlistItem As MemberWishlistItemRow = New MemberWishlistItemRow(DB)
                With dbWishlistItem
                    .MemberId = MemberId
                    .AttributeString = AttributeString
                    .Quantity = Quantity
                    .ItemId = dbStoreItem.ItemId
                End With
                Dim WishlistItemId As Integer = dbWishlistItem.Insert()

                'Insert all attributes
                For Each attr As ItemAttribute In ItemAttributes
                    dbWishlistItem.InsertAttribute(attr)
                Next
            End If
        End Sub

        Public Shared Function GetWishlistItems(ByVal Db As Database, ByVal Memberid As Integer) As DataTable
            Return Db.GetDataTable("SELECT wi.*, si.ItemName, si.SKU, si.Image, si.CustomURL, si.BrandId, case si.IsOnSale when 1 then si.SalePrice else si.Price end as Price FROM MemberWishlistItem wi, StoreItem si where si.ItemId = wi.ItemId and MemberId = " & Memberid)
        End Function

        Public Shared Sub DeleteWishlistItem(ByVal Db As Database, ByVal MemberId As Integer, ByVal WishlistItemId As Integer)
            Dim SQL As String

            SQL = "delete from MemberWishlistItemAttribute where MemberId = " & MemberId & " and WishlistItemId = " & WishlistItemId
            Db.ExecuteSQL(SQL)

            SQL = "delete from MemberWishlistItem where MemberId = " & MemberId & " and WishlistItemId = " & WishlistItemId
            Db.ExecuteSQL(SQL)
        End Sub

        Public Shared Sub UpdateQuantity(ByVal Db As Database, ByVal MemberId As Integer, ByVal WishlistItemId As Integer, ByVal Quantity As Integer)
            Dim SQL As String
            SQL = "update MemberWishlistItem set Quantity = " & Quantity & " where MemberId = " & MemberId & " and WishlistItemId = " & WishlistItemId
            Db.ExecuteSQL(SQL)
        End Sub

    End Class
End Namespace
