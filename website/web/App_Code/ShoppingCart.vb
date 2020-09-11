Imports System
Imports System.Collections
Imports System.Security
Imports System.Security.Cryptography
Imports System.Text
Imports DataLayer
Imports System.Web.SessionState
Imports Database
Imports Utility
Imports Components
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Configuration.ConfigurationManager
Imports System.Net

Namespace Components
    Public Class ShoppingCart

        Public Shared Function GenerateOrRetrieveOrder(ByVal DB As Database) As Integer
            Dim SQL As String
            Dim Session As HttpSessionState = HttpContext.Current.Session
            Dim Request As HttpRequest = HttpContext.Current.Request
            Dim ReferralCode As String = IIf(Session("ReferralCode") Is Nothing, String.Empty, Session("ReferralCode"))
            Dim OrderId As Integer

            If Session("OrderId") Is Nothing Then
                SQL = String.Empty
                SQL &= " INSERT INTO StoreOrder (CreateDate, Status, ReferralCode, RemoteIP, Guid) VALUES ("
                SQL &= DB.Quote(Now()) & ","
                SQL &= DB.Quote("N") & ","
                SQL &= DB.Quote(ReferralCode) & ","
                SQL &= DB.Quote(Request.ServerVariables("REMOTE_ADDR")) & ","
                SQL &= DB.Quote(Core.GenerateFileID)
                SQL &= " )"

                OrderId = DB.InsertSQL(SQL)

                Session("OrderId") = OrderId

                'Save order in the cookie
                CookieUtil.SetTripleDESEncryptedCookie("OrderId", Session("OrderId"), Today.AddDays(15))
            Else
                OrderId = Session("OrderId")
            End If

            'Save the banner order tracking (if the information was saved in the session)
            If Not Session("BannerOrderTracking") Is Nothing Then
                Dim Banner() As String = HttpContext.Current.Session("BannerOrderTracking").ToString.Split(",")
                For i As Integer = 0 To Banner.Length - 1
                    BannerOrderTrackingRow.AddClick(DB, Banner(i), OrderId)
                Next
                Session("BannerOrderTracking") = Nothing
            End If

            'Update OrderNo for Tracking search terms
            If Not Session("SearchOrderTracking") Is Nothing Then
                Dim SearchTermTracking As String = HttpContext.Current.Session("SearchOrderTracking").ToString
                DB.ExecuteSQL("Update SearchTerm Set OrderId=" & DB.Number(OrderId) & " Where TermId in (" & SearchTermTracking & ")")
                Session("SearchOrderTracking") = Nothing
            End If

            Return OrderId
        End Function

        Public Shared Function ValidateOrderItems(ByVal DB As Database, ByVal OrderId As Integer) As String
            Dim Deleted As Boolean = False
			Dim Adjusted As Boolean = False
			Dim DeletedErrorMessage As String = String.Empty
			Dim AdjustedErrorMessage As String = String.Empty
			Dim sDeletedItems As String = String.Empty
			Dim sAdjustedItems As String = String.Empty

			Dim SQL As String
			Dim dv As DataView
			Dim htTemplate As New Hashtable
			Dim htItems As New Hashtable

			Dim EnableInventoryManagement As Boolean = SysParam.GetValue(DB, "EnableInventoryManagement") = 1
			Dim EnableAttributeInventoryManagement As Boolean = SysParam.GetValue(DB, "EnableAttributeInventoryManagement") = 1
			Dim InventoryActionThreshold As Integer = SysParam.GetValue(DB, "InventoryActionThreshold")

			SQL = "SELECT * FROM (SELECT soi.SKU, soi.ItemName, soi.OrderItemId, si.ItemId, COALESCE(si.IsActive, 0) AS IsActive, si.InventoryQty, si.TemplateId, COALESCE(si.InventoryActionThreshold," & InventoryActionThreshold & ") AS InventoryActionThreshold, si.InventoryAction, soi.Quantity, IsAttributes FROM StoreOrderItem soi INNER JOIN StoreItem si ON soi.itemid = si.itemid INNER JOIN StoreItemTemplate sit ON si.TemplateId = sit.TemplateId WHERE OrderId = " & OrderId & ") AS tmp WHERE IsActive = 0 " & IIf(EnableInventoryManagement, " OR (" & IIf(EnableAttributeInventoryManagement, " IsAttributes = 0 AND ", "") & CInt(EnableInventoryManagement) & " = -1 AND InventoryQty - CASE WHEN InventoryAction = 'Backorder' THEN 0 ELSE InventoryActionThreshold END < Quantity) OR IsAttributes = 1", "")
			dv = DB.GetDataTable(SQL).DefaultView

			For Each row As DataRowView In dv
				Dim col As New ItemAttributeCollection
				Dim dtTree As DataTable = DB.GetDataTable("exec sp_GetAttributeTree " & row("ItemId"))
				Dim tblIds As DataTable = DB.GetDataTable("select itemattributeid from storeorderitemattribute where orderitemid = " & row("orderitemid"))
				For Each r As DataRow In tblIds.Rows
					For Each dr As DataRow In dtTree.Rows
						If dr("ItemAttributeId") = CInt(r("ItemAttributeId")) Then
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
							attr.ItemId = row("ItemId")
							attr.Price = IIf(IsDBNull(dr("Price")), 0, dr("Price"))
							attr.SKU = IIf(IsDBNull(dr("SKU")), String.Empty, dr("SKU"))
							attr.SortOrder = IIf(IsDBNull(dr("SortOrder")), 0, dr("SortOrder"))
							attr.TemplateAttributeId = IIf(IsDBNull(dr("TemplateAttributeId")), 0, dr("TemplateAttributeId"))
							col.Add(attr)
						End If
					Next
				Next

				If Not StoreItemRow.IsValidAttributes(DB, row("ItemId"), col) Then
					DeleteItem(DB, OrderId, row, Deleted, sDeletedItems)
					dv.Table.Rows.Remove(row.Row)
				End If
			Next

			For Each row As DataRowView In dv
				If Not CBool(row("IsActive")) Then
					DeleteItem(DB, OrderId, row, Deleted, sDeletedItems)

				ElseIf EnableInventoryManagement Then
					If CBool(row("IsAttributes")) AndAlso EnableAttributeInventoryManagement Then
						Dim InventoryControlledAttributeId As Integer

						If htTemplate(row("TemplateId")) = Nothing Then
							Dim dvTemplate As DataView = DB.GetDataTable("exec sp_GetTemplateAttributeTreeByTemplate " & row("TemplateId")).DefaultView
							dvTemplate.RowFilter = "IsInventoryManagement = 1"
							If dvTemplate.Count > 0 Then
								InventoryControlledAttributeId = dvTemplate(dvTemplate.Count - 1)("TemplateAttributeId")
							End If
							htTemplate(row("TemplateId")) = InventoryControlledAttributeId
						Else
							InventoryControlledAttributeId = htTemplate(row("TemplateId"))
						End If

						If Not InventoryControlledAttributeId = Nothing Then
							SQL = "SELECT TOP 1 ItemAttributeId FROM StoreOrderItemAttribute WHERE OrderItemId = " & row("OrderItemId") & " AND TemplateAttributeId = " & InventoryControlledAttributeId
							Dim ItemAttributeId As Integer = DB.ExecuteScalar(SQL)

							SQL = "SELECT TOP 1 CASE WHEN COALESCE(InventoryAction," & DB.Quote(row("InventoryAction")) & ") = 'Backorder' THEN " & row("Quantity") & " ELSE InventoryQty - COALESCE(InventoryActionThreshold," & InventoryActionThreshold & ") END FROM StoreItemAttribute WHERE ItemAttributeId = " & ItemAttributeId
							Dim NewQuantity As Integer = DB.ExecuteScalar(SQL)

							If NewQuantity < row("Quantity") Then AdjustItem(DB, OrderId, row, NewQuantity, Adjusted, sAdjustedItems, Deleted, sDeletedItems)
						End If
					Else
						If Not row("InventoryAction") = "Backorder" Then
							If CBool(row("IsAttributes")) Then
								If htItems(row("ItemId")) = Nothing Then htItems(row("ItemId")) = row("Quantity") Else htItems(row("ItemId")) += row("Quantity")
								If htItems(row("ItemId")) > row("InventoryQty") - row("InventoryActionThreshold") Then
									AdjustItem(DB, OrderId, row, row("Quantity") - (htItems(row("ItemId")) - (row("InventoryQty") - row("InventoryActionThreshold"))), Adjusted, sAdjustedItems, Deleted, sDeletedItems)
								End If
							Else
								AdjustItem(DB, OrderId, row, row("InventoryQty") - row("InventoryActionThreshold"), Adjusted, sAdjustedItems, Deleted, sDeletedItems)
							End If
						End If
					End If
				End If
            Next

			If Deleted OrElse Adjusted Then
				If Deleted Then DeletedErrorMessage = "Due to limited availabitlity, the following item(s) have been removed from your shopping cart:<br />"

				If Adjusted And Deleted Then AdjustedErrorMessage = "<br /> &raquo;"
				If Adjusted Then AdjustedErrorMessage &= "Due to limited availabitlity, the following item(s) have had their quantities adjusted:<br />"

                DeleteNotUsedRecipients(DB, OrderId)
                RecalculateShoppingCart(DB, OrderId, True)
				Return DeletedErrorMessage & sDeletedItems & AdjustedErrorMessage & sAdjustedItems
            Else
                Return String.Empty
            End If
        End Function

		Private Shared Sub DeleteItem(ByVal DB As Database, ByVal OrderId As Integer, ByVal row As DataRowView, ByRef Deleted As Boolean, ByRef sDeletedItems As String)
			Deleted = True
			sDeletedItems &= " - " & row("ItemName") & " (SKU: " & row("SKU") & ")<br />"
			DeleteOnlyOrderItem(DB, OrderId, row("OrderItemId"))
		End Sub

		Private Shared Sub AdjustItem(ByVal DB As Database, ByVal OrderId As Integer, ByVal row As DataRowView, ByVal NewQuantity As Integer, ByRef Adjusted As Boolean, ByRef sAdjustedItems As String, ByRef Deleted As Boolean, ByRef sDeletedItems As String)
			If NewQuantity <= 0 Then
				DeleteItem(DB, OrderId, row, Deleted, sDeletedItems)
				Exit Sub
			End If

			Adjusted = True
			sAdjustedItems &= " - " & row("ItemName") & " (SKU: " & row("SKU") & ")<br />"
			DB.ExecuteSQL("UPDATE StoreOrderItem SET Quantity = " & NewQuantity & " WHERE OrderItemId = " & DB.Number(row("OrderItemId")))
		End Sub

        Public Shared Sub EnsureOrder(ByVal Db As Database)
            If HttpContext.Current.Session("OrderId") Is Nothing Then
                HttpContext.Current.Response.Redirect("/store/cart.aspx")
            End If
            Dim SQL As String = "select OrderId from StoreOrder where OrderId = " & Db.Number(HttpContext.Current.Session("OrderId")) & " and ProcessDate is null"
            Dim Result As Integer = Db.ExecuteScalar(SQL)
            If Result = 0 Then
                HttpContext.Current.Session("OrderId") = Nothing
                CookieUtil.SetTripleDESEncryptedCookie("OrderId", Nothing)
                HttpContext.Current.Response.Redirect("cart.aspx")
            End If
        End Sub

        Public Shared Sub EnsureBillingInfo(ByVal DB As Database, ByVal OrderId As Integer)
            Dim SQL As String = "select OrderId from StoreOrder where BillingLastName is null and OrderId = " & OrderId
            Dim Result As Integer = DB.ExecuteScalar(SQL)
            If Result <> 0 Then HttpContext.Current.Response.Redirect("checkout.aspx")
        End Sub

        Public Shared Sub EnsureShippingInfo(ByVal DB As Database, ByVal OrderId As Integer)
            Dim SQL As String = String.Empty

            Dim MultipleShipToEnabled As Boolean = SysParam.GetValue(DB, "MultipleShipToEnabled")
            Dim RedirectURL As String = "billing.aspx"
            If MultipleShipToEnabled Then
                RedirectURL = "shipping.aspx"
            End If

            SQL = "SELECT TOP 1 RecipientId FROM StoreOrderRecipient WHERE OrderId = " & OrderId & " AND LastName IS NULL"
            If DB.ExecuteScalar(SQL) <> Nothing Then
                HttpContext.Current.Response.Redirect(RedirectURL)
            End If

            SQL = "SELECT TOP 1 RecipientId FROM StoreOrderRecipient WHERE OrderId = " & OrderId
            If DB.ExecuteScalar(SQL) = Nothing Then
                HttpContext.Current.Response.Redirect(RedirectURL)
            End If
        End Sub

        Public Shared Sub DeleteNotUsedRecipients(ByVal DB As Database, ByVal OrderId As Integer)
            Dim RowsAffected As Integer = DB.ExecuteSQL("DELETE FROM StoreOrderRecipient WHERE OrderId = " & OrderId & " AND RecipientId NOT IN (SELECT RecipientId FROM StoreOrderItem WHERE OrderId = " & OrderId & ")")
        End Sub

        Public Shared Function GenerateUniqueOrderNo(ByVal DB As Database) As Integer
            Return DB.InsertSQL("INSERT INTO StoreSequence (CreateDate) VALUES (" & DB.Quote(Now) & ")")
        End Function

        Public Shared Function GetOrderItems(ByVal Db As Database, ByVal OrderId As Integer) As DataTable
            Return Db.GetDataTable("SELECT sci.*, sor.IsShippingIndividually, (SELECT Name FROM StoreOrderStatus WHERE StoreORderSTatus.Code = sci.Status) as StatusName FROM StoreOrderItem sci, StoreOrderRecipient sor WHERE sci.RecipientId=sor.RecipientId AND sci.OrderId=" & OrderId)
        End Function

        Public Shared Function GetOrderItems(ByVal DB As Database, ByVal OrderId As Integer, ByVal RecipientId As Integer) As DataTable
            Return DB.GetDataTable("SELECT sci.* FROM StoreOrderItem sci WHERE OrderId=" & OrderId & " AND RecipientId=" & RecipientId)
        End Function

        Public Shared Function GetOrderRecipients(ByVal DB As Database, ByVal OrderId As Integer) As DataTable
            Return DB.GetDataTable("SELECT *,(SELECT Name FROM StoreOrderStatus WHERE StoreORderSTatus.Code = StoreOrderRecipient.Status) as StatusName, (select top 1 OrderItemId FROM StoreOrderItem WHERE OrderId = " & OrderId & " AND RecipientId = StoreOrderRecipient.RecipientId AND GiftQuantity > 0) AS GiftWrap FROM StoreOrderRecipient WHERE OrderId=" & OrderId & " order by Label")
        End Function

        Public Shared Function GetRecipients(ByVal Db As Database, ByVal OrderId As Integer) As DataTable
            Return Db.GetDataTable("select distinct sor.recipientid, label from storeorderrecipient sor inner join StoreOrderitem sci on sor.recipientid = sci.recipientid where sor.orderid = " & OrderId)
        End Function

        Public Shared Function GetOrderAddresses(ByVal Db As Database, ByVal OrderId As Integer, ByVal MemberId As Integer) As DataTable
            Dim SQL As String = String.Empty

            SQL = SQL & " select 0 AS SortOrder, 'Same as Billing' As Label, 'Same as Billing' As LabelValue, BillingFirstName AS FirstName, BillingLastName AS LastName,BillingCompany AS Company, BillingAddress1 AS Address1,BillingAddress2 AS Address2, BillingCity AS City, BillingState AS State, BillingRegion AS Region, BillingZip AS Zip, BillingPhone AS Phone, BillingCountry AS Country FROM StoreOrder WHERE OrderId = " & OrderId
            SQL = SQL & " union "
            SQL = SQL & " select 1 AS SortOrder, 'My Default Shipping Address' AS Label,'Default Shipping Address' AS LabelValue,FirstName, LastName,Company, Address1,Address2, City, State, Region, Zip, Phone, Country FROM MemberAddress WHERE AddressType IN ('Shipping') AND IsDefault = 1 AND MemberId = " & MemberId
            SQL = SQL & " union "
            SQL = SQL & " select 2 AS SortOrder, Label + ' (' + FirstName + ' ' + LastName + ')' AS Label,Label AS LabelValue,FirstName, LastName,Company, Address1,Address2, City, State, Region, Zip, Phone, Country FROM MemberAddress WHERE AddressType IN ('AddressBook') AND MemberId = " & MemberId

            Return Db.GetDataTable(SQL)
        End Function

        Public Shared Sub DeleteOnlyOrderItem(ByVal DB As Database, ByVal OrderId As Integer, ByVal OrderItemId As Integer)
            Dim SQL As String
            SQL = "delete from StoreOrderItemAttribute where OrderId = " & OrderId & " and OrderItemId = " & OrderItemId
            Db.ExecuteSQL(SQL)

            SQL = "delete from StoreOrderItem where OrderId = " & OrderId & " and OrderItemId = " & OrderItemId
            Db.ExecuteSQL(SQL)
        End Sub

        Public Shared Sub DeleteOrderItem(ByVal Db As Database, ByVal OrderId As Integer, ByVal OrderItemId As Integer)
            DeleteOnlyOrderItem(Db, OrderId, OrderItemId)
            DeleteNotUsedRecipients(Db, OrderId)
        End Sub

        Public Shared Sub UpdateQuantities(ByVal Db As Database, ByVal OrderId As Integer, ByVal OrderItemId As Integer, ByVal Quantity As Integer, ByVal GiftQuantity As Integer)
            Dim SQL As String
            SQL = "update StoreOrderItem set Quantity = " & Quantity & ", GiftQuantity = " & GiftQuantity & " where OrderId = " & OrderId & " and OrderItemId = " & OrderItemId
            Db.ExecuteSQL(SQL)
        End Sub

        Public Shared Function Add2Cart(ByVal DB As Database, ByVal OrderId As Integer, ByVal MemberId As Integer, ByVal ItemId As Integer, ByVal Quantity As Integer, ByVal DepartmentId As Integer, ByVal BrandId As Integer, ByVal Label As String, Optional ByVal ItemAttributes As ItemAttributeCollection = Nothing) As Integer
            Add2Cart = 0

            If ItemId = 0 Then Throw New ApplicationException("Invalid Parameters")
            If Quantity = 0 Then Exit Function

            Dim RecipientId As Integer = Nothing
            Dim SQL As String = String.Empty

            'Retrieve RecipientId
            If Right(Label, 4) = "_sor" Then
                RecipientId = Left(Label, Len(Label) - 4)

            ElseIf Right(Label, 4) = "_mab" Then
                RecipientId = Left(Label, Len(Label) - 4)

                'Recipient exists in the Address Book, but no StoreOrderRecipient table
                RecipientId = DB.ExecuteScalar("SELECT TOP 1 RecipientId FROM StoreOrderRecipient WHERE OrderId = " & OrderId & " AND RecipientId = " & DB.Number(RecipientId))

                If RecipientId = 0 Then
                    SQL = " INSERT INTO StoreOrderRecipient (AddressId, OrderId, Label, FirstName, MiddleInitial, LastName, Company, Address1, Address2, City, State, Region, Country, Phone, Zip) SELECT AddressId," & OrderId & ",Label, FirstName, MiddleInitial, LastName,Company,Address1,Address2,City,State,Region,Country,Phone,Zip FROM MemberAddress WHERE AddressId = " & DB.Number(Left(Label, Len(Label) - 4))
                    RecipientId = DB.InsertSQL(SQL)
                End If

            ElseIf LCase(Label) = "myself" Then
                RecipientId = DB.ExecuteScalar("SELECT TOP 1 RecipientId FROM StoreOrderRecipient WHERE OrderId=" & OrderId & " AND IsCustomer = 1")
                If RecipientId = 0 Then
                    If Not MemberId = 0 Then
                        SQL = " INSERT INTO StoreOrderRecipient (AddressId, IsCustomer, OrderId, Label, FirstName, MiddleInitial, LastName, Company, Address1, Address2, City, State, Region, Country, Phone, Zip) SELECT AddressId, 1, " & OrderId & ", coalesce(LABEL,'Myself') AS Label,FirstName, MiddleInitial, LastName,Company,Address1,Address2,City,State,Region,Country,Phone,Zip FROM MemberAddress WHERE AddressType = 'Shipping' AND IsDefault = 1 AND MemberId = " & MemberId
                        RecipientId = DB.InsertSQL(SQL)
                    End If
                End If

                If RecipientId = 0 Then
                    SQL = "INSERT INTO StoreOrderRecipient (IsCustomer, OrderId, Label) VALUES (1," & OrderId & ",'Myself')"
                    RecipientId = DB.InsertSQL(SQL)
                End If
            Else
                RecipientId = DB.ExecuteScalar("SELECT TOP 1 RecipientId FROM StoreOrderRecipient WHERE OrderId = " & OrderId & " AND Label = " & DB.Quote(Label))
                If RecipientId = 0 Then
                    SQL = "INSERT INTO StoreOrderRecipient (OrderId, Label) VALUES (" & OrderId & "," & DB.Quote(Label) & ")"
                    RecipientId = DB.InsertSQL(SQL)
                End If
            End If
            If RecipientId = 0 Then Exit Function
            Logger.Info("RecipientId = " & RecipientId)

            Dim AttributeString As String = String.Empty
			Dim Conn As String = String.Empty
			Dim ImageName As String = String.Empty
            If Not ItemAttributes Is Nothing Then
                For Each attr As ItemAttribute In ItemAttributes
					AttributeString &= Conn & attr.TemplateAttributeId & attr.AttributeValue
					If Not attr.ProductImage = Nothing Then ImageName = attr.ProductImage
                    Conn = ","
                Next
            End If
            Logger.Info("Attribute String = " & AttributeString)

            SQL = String.Empty
			SQL &= " update StoreOrderItem WITH (ROWLOCK) set Quantity = Quantity + " & Quantity & ", ModifyDate = " & DB.Quote(Now)
			If Not ImageName = String.Empty Then SQL &= ", Image = " & DB.Quote(ImageName)
            SQL &= " WHERE OrderId = " & OrderId
            SQL &= "   AND ItemId = " & ItemId
            SQL &= "   AND RecipientId = " & RecipientId
            If AttributeString = String.Empty Then
                SQL &= "   AND AttributeString is NULL"
            Else
                SQL &= "   AND AttributeString  = " & DB.Quote(AttributeString)
            End If
            Dim RowsAffected As Integer = DB.ExecuteSQL(SQL)
            If RowsAffected <> 1 Then
                'Insert all attributes
                Dim AdditionalPrice As Double = 0
				Dim AdditionalWeight As Double = 0
                Dim AdditionalSKU As String = String.Empty
                If Not ItemAttributes Is Nothing Then
                    For Each attr As ItemAttribute In ItemAttributes
                        If Not attr.Price = 0 Then
                            AdditionalPrice += attr.Price
                        End If
                        If Not attr.SKU = String.Empty Then
                            AdditionalSKU &= attr.SKU
                        End If
                        If Not attr.Weight = 0 Then
                            AdditionalWeight += attr.Weight
                        End If
                    Next
                End If

                Dim dbStoreItem As StoreItemRow = StoreItemRow.GetRow(DB, ItemId)
                Dim dbStoreOrderItem As StoreOrderItemRow = New StoreOrderItemRow(DB)
                dbStoreOrderItem.LoadFromStoreItem(dbStoreItem)
                With dbStoreOrderItem
                    If .IsOnSale Then
                        .Price = .SalePrice + AdditionalPrice
                    Else
                        .Price = .ItemPrice + AdditionalPrice
                    End If
					.Weight = .Weight + AdditionalWeight
                    .SKU = .SKU & AdditionalSKU
                    .OrderId = OrderId
                    .AttributeString = AttributeString
                    .RecipientId = RecipientId
                    .DepartmentId = DepartmentId
                    .Quantity = Quantity
					.Status = "N"
					If Not ImageName = String.Empty Then .Image = ImageName
                End With
                Dim StoreItemId As Integer = dbStoreOrderItem.Insert()

                'Insert all attributes
                If Not ItemAttributes Is Nothing Then
                    For Each attr As ItemAttribute In ItemAttributes
                        dbStoreOrderItem.InsertAttribute(attr)
                    Next
                End If

                Add2Cart = StoreItemId
            End If
        End Function

        Public Shared Sub Add2CartFromWislist(ByVal DB As Database, ByVal OrderId As Integer, ByVal MemberId As Integer, ByVal WishlistItemId As Integer)
            Dim dbWishlistItem As MemberWishlistItemRow = MemberWishlistItemRow.GetRow(DB, WishlistItemId)
            If dbWishlistItem.MemberId <> MemberId Then
                Exit Sub
            End If
            Dim dbItem As StoreItemRow = StoreItemRow.GetRow(DB, dbWishlistItem.ItemId)
            Add2Cart(DB, OrderId, MemberId, dbWishlistItem.ItemId, dbWishlistItem.Quantity, Nothing, dbItem.BrandId, "myself", dbWishlistItem.GetItemAttributeCollection())
        End Sub

        Public Shared Sub UpdatePromotionCode(ByVal DB As Database, ByVal OrderId As Integer, ByVal PromotionCode As String)
            Dim SQL As String
            SQL = "update StoreOrder set PromotionCode = " & DB.Quote(PromotionCode) & " where OrderId = " & OrderId
            DB.ExecuteSQL(SQL)
        End Sub

        Public Shared Sub RecalculateShoppingCart(ByVal DB As Database, ByVal OrderId As Integer, Optional ByVal ValidateDiscount As Boolean = True, Optional ByVal ReferralCode As String = "")
            Dim SQL As String = String.Empty

            'Step 1 - upate ReferralCode (but only if it was null before, because we only save first referral code)
            If Not ReferralCode = String.Empty Then
                DB.ExecuteSQL("update StoreOrder WITH (ROWLOCK) set ReferralCode = " & DB.Quote(ReferralCode) & " WHERE OrderId =" & OrderId & " AND ReferralCode IS NULL")
            End If

            'Step 2 - update BaseSubtotal for each Recipient
            SQL = String.Empty
            SQL &= " update StoreOrderRecipient WITH (ROWLOCK) set BaseSubTotal = coalesce(tmp.SubTotal,0) from StoreOrderRecipient sor,"
            SQL &= " (select RecipientId, (select sum(coalesce(Price*Quantity,0)) from StoreOrderItem soi where soi.RecipientId = StoreOrderRecipient.RecipientId) As SubTotal from StoreOrderRecipient where OrderId = " & OrderId
            SQL &= " group by RecipientId) as tmp where tmp.RecipientId = sor.RecipientId and sor.OrderId = " & OrderId
            DB.ExecuteSQL(SQL)

            'Step 3 - Update order sub-total (needed for Discount calculation)
            SQL = "update StoreOrder WITH (ROWLOCK) set BaseSubTotal = coalesce((select coalesce(sum(BaseSubtotal),0) from StoreOrderRecipient where OrderId =" & OrderId & "),0) where OrderId = " & OrderId
            DB.ExecuteSQL(SQL)

            'Step 4 - recalculate Discount for each OrderItem and for each Address
            If Not RecalculateDiscount(DB, OrderId, ValidateDiscount) Then
                Throw New ApplicationException("Entered promotion code is invalid or has expired. Please re-enter promotion code")
            End If

            'Step 5 - Recalculate Shipping Cost for each Recipient
            RecalculateShipping(DB, OrderId, ValidateDiscount)

            'Step 6 - recalculate GiftWrapping for each ship-to
            Dim GiftWrapPrice As Double = SysParam.GetValue(DB, "GiftWrapPrice")
            SQL = String.Empty
            SQL &= " update StoreOrderRecipient WITH (ROWLOCK) set GiftWrapping = coalesce(tmp.GiftWrapping,0) * " & GiftWrapPrice & " from StoreOrderRecipient sor,"
            SQL &= " (select RecipientId, (select sum(coalesce(GiftQuantity,0)) from StoreOrderItem soi where soi.OrderId = " & OrderId & " and soi.RecipientId = StoreOrderRecipient.RecipientId) As GiftWrapping from StoreOrderRecipient where OrderId = " & OrderId
            SQL &= " group by RecipientId ) as tmp where tmp.RecipientId = sor.RecipientId and sor.OrderId = " & OrderId
            DB.ExecuteSQL(SQL)

            'Step 7 - Calculate Tax
            RecalculateTax(DB, OrderId)

            'Step 8 - Update totals for the order
            SQL = String.Empty
            SQL &= " update StoreOrder WITH (ROWLOCK) set "
            SQL &= " Discount = tmp.Discount, Subtotal = tmp.Subtotal,"
            SQL &= " GiftWrapping = tmp.GiftWrapping, Shipping = tmp.Shipping,"
            SQL &= " Tax = tmp.Tax, Total = tmp.Total from StoreOrder so, ("
            SQL &= " select OrderId,"
            SQL &= " sum(coalesce(Discount,0)) as Discount,"
            SQL &= " sum(coalesce(Subtotal,0)) as Subtotal,"
            SQL &= " sum(coalesce(GiftWrapping,0)) as GiftWrapping,"
            SQL &= " sum(coalesce(Shipping,0)) as Shipping,"
            SQL &= " sum(coalesce(Tax,0)) as Tax,"
            SQL &= " sum(coalesce(Total,0)) as Total"
            SQL &= " from StoreOrderRecipient where OrderId = " & OrderId & " group by OrderId"
            SQL &= " ) as tmp where tmp.OrderId = so.OrderId and so.OrderId = " & OrderId
            Dim RowsAffected As Integer = DB.ExecuteSQL(SQL)
            If RowsAffected = 0 Then
                DB.ExecuteSQL(" update StoreOrder WITH (ROWLOCK) set Discount = 0,Subtotal = 0,Shipping = 0,GiftWrapping = 0,Tax = 0,Total = 0 where OrderId = " & OrderId)
            End If
        End Sub

        Private Shared Function RecalculateDiscount(ByVal DB As Database, ByVal OrderId As Integer, ByVal ValidateDiscount As Boolean) As Boolean
            Dim SQL As String = String.Empty

            'Clear Discount field
            SQL = "update StoreOrderItem WITH (ROWLOCK) set Discount = 0 where OrderId = " & OrderId
            DB.ExecuteSQL(SQL)
            SQL = "update StoreOrderRecipient WITH (ROWLOCK) set Discount = 0, Subtotal = COALESCE(BaseSubtotal,0) where OrderId = " & OrderId
            DB.ExecuteSQL(SQL)

            Dim dbOrder As StoreOrderRow = StoreOrderRow.GetRow(DB, OrderId)

            'Exit if no promotion has been entered
            If dbOrder.PromotionCode = String.Empty Then
                Logger.Info("Promotion Code is blank")
                Return True
            End If

            Dim dbPromotion As StorePromotionRow = StorePromotionRow.GetRowByCode(DB, dbOrder.PromotionCode)
            If ValidateDiscount Then
                If Not ValidatePromotion(dbOrder, dbPromotion) Then
                    Return False
                End If
            End If

            'if the puchase amount is too low
            If Not dbPromotion.MinimumPurchase = 0 Then
                If dbOrder.BaseSubtotal < dbPromotion.MinimumPurchase Then
                    Logger.Info("puchase amount is too low")
                    dbPromotion.Discount = 0
                End If
            End If

            'if the puchase amount is too high
            If Not dbPromotion.MaximumPurchase = 0 Then
                If dbOrder.BaseSubtotal > dbPromotion.MaximumPurchase Then
                    Logger.Info("puchase amount is too high")
                    dbPromotion.Discount = 0
                End If
            End If

            'Recalculate discount but only items that are part of the promotion
            SQL = String.Empty
            If dbPromotion.IsItemSpecific Then
                SQL &= " select soi.OrderItemId, Quantity*Price as SubTotal from StoreOrderItem soi where OrderId = " & OrderId
                SQL &= " and ("
                SQL &= "    soi.ItemId in (select si.ItemId from StoreDepartmentItem sdi, StorePromotionItem di, StoreItem si, StorePromotion sp where di.PromotionId = sp.PromotionId AND sdi.ItemId = si.ItemId and di.DepartmentId is not null and di.DepartmentId = sdi.DepartmentId and sp.PromotionId = " & dbPromotion.PromotionId & ")"
                SQL &= " or soi.ItemId in (select di.ItemId from StorePromotionItem di, StorePromotion sp where di.PromotionId = sp.PromotionId AND di.ItemId is not null and sp.PromotionId = " & dbPromotion.PromotionId & ")"
                SQL &= " )"
            Else
                SQL = " select soi.OrderItemId, Quantity*Price as SubTotal from StoreOrderItem soi where OrderId = " & OrderId
            End If

            Dim dt As DataTable = DB.GetDataTable(SQL)
            Dim DiscountedTotal As Double = 0
            For Each row As DataRow In dt.Rows
                DiscountedTotal += row("SubTotal")
            Next
            Logger.Info("DiscountedTotal = " & DiscountedTotal)

            For Each row As DataRow In dt.Rows
                Dim Discount As Double = 0
                Select Case LCase(dbPromotion.PromotionType)
                    Case "monetary"
                        Discount = row("Subtotal") * dbPromotion.Discount / DiscountedTotal
                    Case "percentage"
                        Discount = dbPromotion.Discount * row("Subtotal") / 100
                End Select
                Discount = Math.Round(Discount, 2)

                SQL = "update StoreOrderItem WITH (ROWLOCK) set Discount = " & Discount & " where OrderId = " & OrderId & " and OrderItemId = " & row("OrderItemId")
                DB.ExecuteSQL(SQL)
            Next

            'Update StoreOrderRecipient discount
            SQL = String.Empty
            SQL &= " update StoreOrderRecipient WITH (ROWLOCK) set Discount = tmp.Discount "
            SQL &= " from StoreOrderRecipient sor, (select "
            SQL &= " RecipientId, coalesce(sum(Discount),0) as Discount from StoreOrderItem where OrderId = " & OrderId
            SQL &= " group by RecipientId) as tmp where sor.OrderId = " & OrderId & " and sor.RecipientId = tmp.RecipientId"
            DB.ExecuteSQL(SQL)

            'Update StoreOrderRecipient SubTotal
            SQL = " update StoreOrderRecipient WITH (ROWLOCK) set SubTotal = BaseSubtotal - Discount where OrderId = " & OrderId
            DB.ExecuteSQL(SQL)

            Return True
        End Function

        Private Shared Function ValidatePromotion(ByVal dbOrder As StoreOrderRow, ByVal dbPromotion As StorePromotionRow) As Boolean
            If dbPromotion.PromotionCode = String.Empty Then
                Logger.Info("Promotion Code is blank")
                Return False
            End If

            'Exit if promotion has not started yet
            If Not dbPromotion.StartDate = Nothing Then
                If DateDiff("d", Now(), dbPromotion.StartDate) > 0 Then
                    Logger.Info("promotion has not started yet")
                    Return False
                End If
            End If

            'Exit if promotion has expired already
            If Not dbPromotion.EndDate = Nothing Then
                If DateDiff("d", dbPromotion.EndDate, Now()) > 0 Then
                    Logger.Info("promotion has expired already")
                    Return False
                End If
            End If
            Return True
        End Function

        Private Shared Sub RecalculateShipping(ByVal DB As Database, ByVal OrderId As Integer, ByVal ValidateDiscount As Boolean)
            Dim dbOrder As StoreOrderRow = StoreOrderRow.GetRow(DB, OrderId)
            Dim dbPromotion As StorePromotionRow = StorePromotionRow.GetRowByCode(DB, dbOrder.PromotionCode)

            If Not dbOrder.PromotionCode = String.Empty Then
                If ValidateDiscount Then
                    If Not ValidatePromotion(dbOrder, dbPromotion) Then
                        Throw New ApplicationException("Entered promotion code is invalid or has expired. Please re-enter promotion code.")
                    End If
                End If

                'if the puchase amount is too low
                If Not dbPromotion.MinimumPurchase = 0 Then
                    If dbOrder.BaseSubtotal < dbPromotion.MinimumPurchase Then
                        Logger.Info("puchase amount is too low")
                        dbPromotion.IsFreeShipping = False
                    End If
                End If

                'if the puchase amount is too high
                If Not dbPromotion.MaximumPurchase = 0 Then
                    If dbOrder.BaseSubtotal > dbPromotion.MaximumPurchase Then
                        Logger.Info("puchase amount is too high")
                        dbPromotion.IsFreeShipping = False
                    End If
                End If
            End If

            Dim SQL As String = String.Empty

            'Perform Calculation For Ech Ship-To Address Separatelly
            Dim dt As DataTable = GetOrderRecipients(DB, OrderId)
            For Each row As DataRow In dt.Rows
                Dim Subtotal As Double = row("Subtotal")
                Dim ShippingOption As String = SysParam.GetValue(DB, "ShippingOption")
                Dim Shipping As Double = 0

                'If Shipping Address Not Entered, Then Clear The Shipping_Option, So The Shipping Is Set To $0.00
                If IsDBNull(row("Country")) Then
                    ShippingOption = String.Empty
                End If

                Select Case ShippingOption
                    Case "ShippingRange"
                        Shipping = DB.ExecuteScalar("SELECT TOP 1 ShippingValue FROM StoreShippingRange WHERE ShippingFrom <= " & Subtotal & " AND ShippingTo >= " & Subtotal)
                    Case "ShippingPerc"
                        Dim ShippingPerc As Double = IIf(SysParam.GetValue(DB, "ShippingPerc") = String.Empty, 0, SysParam.GetValue(DB, "ShippingPerc"))
                        Shipping = Math.Round(Subtotal * ShippingPerc / 100, 2)
                    Case "ShippingSame"
                        Dim ShippingSame As Double = IIf(SysParam.GetValue(DB, "ShippingSame") = String.Empty, 0, SysParam.GetValue(DB, "ShippingSame"))
                        Shipping = ShippingSame

                    Case "ShippingUPS"

                        Dim Weight As Double = GetItemsWeight(DB, OrderId, row("RecipientId"))
                        Dim Width As Double, Height As Double, Thickness As Double
                        GetItemsDimensions(DB, OrderId, row("RecipientId"), Thickness, Width, Height)

                        Logger.Info("Weight = " & Weight)
                        Logger.Info("Thickness = " & Thickness)
                        Logger.Info("Width = " & Width)
                        Logger.Info("Height = " & Height)

                        Dim ShippingMethodId As Integer = IIf(IsDBNull(row("ShippingMethodId")), 0, row("ShippingMethodId"))
                        Dim ShippingMethod As String = DB.ExecuteScalar("SELECT top 1 UpsCode FROM StoreShippingMethod WHERE MethodId = " & ShippingMethodId)
                        'UPS GROUND IS DEFAULT SHIPPING METHOD
                        If ShippingMethod = String.Empty Then
                            ShippingMethod = "03"
                        End If

                        Dim FromCity As String = SysParam.GetValue(DB, "ShippingCity")
                        Dim FromState As String = SysParam.GetValue(DB, "ShippingState")
                        Dim FromZip As String = SysParam.GetValue(DB, "ShippingZip")
                        Dim ErrorDesc As String = String.Empty

                        Dim ShippingUPS As Double = UPS.GetRate(FromCity, FromState, FromZip, row("City"), IIf(row("Country") = "US", row("State"), IIf(IsDBNull(row("Region")), "", row("Region"))), row("Zip"), row("Country"), Weight, Width, Height, Thickness, ShippingMethod, False, ErrorDesc)
                        If ShippingUPS = -1 Then
                            Throw New ApplicationException(ErrorDesc)
                        End If
                        Shipping = ShippingUPS

                    Case "ShippingFedEx"

                        Dim Weight As Double = GetItemsWeight(DB, OrderId, row("RecipientId"))
                        Dim Width As Double, Height As Double, Thickness As Double
                        GetItemsDimensions(DB, OrderId, row("RecipientId"), Thickness, Width, Height)

                        Logger.Info("Weight = " & Weight)
                        Logger.Info("Thickness = " & Thickness)
                        Logger.Info("Width = " & Width)
                        Logger.Info("Height = " & Height)

                        Dim ShippingMethod As Object = DB.ExecuteScalar("SELECT top 1 FedExCode FROM StoreShippingMethod WHERE MethodId = " & DB.Number(row("ShippingMethodId")))
                        'UPS GROUND IS DEFAULT SHIPPING METHOD
                        If IsDBNull(ShippingMethod) = String.Empty Then
                            ShippingMethod = "FEDEXGROUND"
                        Else
                            ShippingMethod = CStr(ShippingMethod)
                        End If

                        Dim FromCity As String = SysParam.GetValue(DB, "ShippingCity")
                        Dim FromState As String = SysParam.GetValue(DB, "ShippingState")
                        Dim FromZip As String = SysParam.GetValue(DB, "ShippingZip")
                        Dim FromAddressLn1 As String = SysParam.GetValue(DB, "ShippingAddress1")
                        Dim FromAddressLn2 As String = SysParam.GetValue(DB, "ShippingAddress2")
                        Dim ErrorDesc As String = String.Empty

                        Dim ShippingFedEx As Double = FedEx.GetRate(OrderId, FromCity, FromState, FromZip, FromAddressLn1, FromAddressLn2, row("City"), IIf(row("Country") = "US", row("State"), IIf(IsDBNull(row("Region")), "", row("Region"))), row("Zip"), row("Address1"), row("Address2"), row("Country"), Weight, Width, Height, Thickness, ShippingMethod, False, ErrorDesc)
                        If ShippingFedEx = -1 Then
                            Throw New ApplicationException(ErrorDesc)
                        End If
                        Shipping = ShippingFedEx

                    Case "ShippingProd"

                        If row("Country") = "US" Then

                            ' CALCULATE SHIPPING FOR US ONLY
                            SQL = String.Empty
                            SQL &= " SELECT OrderItemId, Shipping, Price, Quantity, COALESCE(Shipping1,0) AS Shipping1, COALESCE(Shipping2,0) AS Shipping2"
                            SQL &= " FROM StoreOrderItem where OrderId = " & OrderId & " and RecipientId = " & row("RecipientId")
                            SQL &= " ORDER BY Shipping1 desc"

                            Dim FirstItem As Boolean = True
                            Dim dtItems As DataTable = DB.GetDataTable(SQL)
                            For Each item As DataRow In dtItems.Rows
                                Dim value As Double = 0
                                If FirstItem Then
                                    value = item("Shipping1") + (item("Quantity") - 1) * item("Shipping2")
                                    Shipping += value
                                    FirstItem = False
                                Else
                                    value = item("Quantity") * item("Shipping2")
                                    Shipping += value
                                End If
								DB.ExecuteSQL("update StoreOrderItem with (ROWLOCK) set Shipping = " & value & " where OrderId = " & OrderId & " and OrderItemId = " & item("OrderItemId"))
                            Next
                        Else
                            ' CALCULATE SHIPPING FOR OTHER COUNTRIES
                            SQL = String.Empty
                            SQL &= " SELECT SUM(oi.Quantity * COALESCE(oi.CountryUnit,0) * COALESCE(c.Shipping,0)) AS CountryShipping"
                            SQL &= "  FROM StoreOrderItem oi, Country c"
                            SQL &= " WHERE oi.OrderId = " & OrderId
                            SQL &= "   AND oi.RecipientId = " & row("RecipientId")
                            SQL &= "   AND  c.CountryCode = " & DB.Quote(row("Country"))
                            Shipping = DB.ExecuteScalar(SQL)
                        End If

                    Case "ShippingCustom"
                        'Implement custom shipping here
                        Shipping = 0
                    Case Else
                        Shipping = 0
                End Select

                If dbPromotion.IsFreeShipping Then Shipping = 0
                DB.ExecuteSQL("UPDATE StoreOrderRecipient WITH (ROWLOCK) SET Shipping = " & Shipping & " WHERE OrderId = " & OrderId & " AND RecipientId = " & row("RecipientId"))
            Next
        End Sub

        Private Shared Sub RecalculateTax(ByVal DB As Database, ByVal OrderId As Integer)
            DB.ExecuteSQL("UPDATE StoreOrderRecipient WITH (ROWLOCK) SET Tax = 0 WHERE OrderId = " & OrderId)

            Dim SQL As String = String.Empty
            SQL &= " select RecipientId, Subtotal,"
            SQL &= " Subtotal - coalesce((select sum(Quantity*Price - Discount) from StoreOrderitem where RecipientId = sor.RecipientId and IsTaxFree = 1),0) as TaxSubtotal,"
            SQL &= " Shipping, GiftWrapping, State, TaxRate, TaxShipping, TaxGiftWrap "
            SQL &= " from StoreOrderRecipient sor left outer join State s on sor.State = s.StateCode where sor.OrderId = " & OrderId

            Dim dt As DataTable = DB.GetDataTable(SQL)
            For Each row As DataRow In dt.Rows
                Dim Tax As Double = 0
                Dim TaxSubtotal As Double = row("TaxSubtotal")

                If IIf(IsDBNull(row("TaxShipping")), False, row("TaxShipping")) Then TaxSubtotal += IIf(IsDBNull(row("Shipping")), 0, row("Shipping"))
                If IIf(IsDBNull(row("TaxGiftWrap")), False, row("TaxGiftWrap")) Then TaxSubtotal += IIf(IsDBNull(row("GiftWrapping")), 0, row("GiftWrapping"))
                If IIf(IsDBNull(row("TaxRate")), 0, row("TaxRate")) > 0 Then Tax = Math.Round(row("TaxRate") * TaxSubtotal / 100, 2)

                Dim Total As Double = row("Subtotal") + row("Shipping") + row("GiftWrapping") + Tax

                DB.ExecuteSQL(" update StoreOrderRecipient WITH (ROWLOCK) set Tax = " & Tax & ", Total = " & Total & " where OrderId = " & OrderId & " and RecipientId = " & row("RecipientId"))
            Next
        End Sub

        Public Shared Sub SendConfirmations(ByVal DB As Database, ByVal OrderId As Integer)
            Dim dbOrder As StoreOrderRow = StoreOrderRow.GetRow(DB, OrderId)

            Dim URL As String = IIf(AppSettings("GlobalSecureName") = String.Empty, AppSettings("GlobalRefererName"), AppSettings("GlobalRefererName")) & "/store/confirm.aspx?OrderId=" & HttpUtility.UrlEncode(Crypt.EncryptTripleDes(dbOrder.OrderId.ToString)) & "&print=y&c=y"
            Dim r As HttpWebRequest = WebRequest.Create(URL)
            Dim myCache As New System.Net.CredentialCache()
            myCache.Add(New Uri(URL), "Basic", New System.Net.NetworkCredential("ameagle", "design"))
            r.Credentials = myCache

            Try
                'Get the data as an HttpWebResponse object
                Dim resp As System.Net.HttpWebResponse = r.GetResponse()
                Dim sr As New System.IO.StreamReader(resp.GetResponseStream())
                Dim HTML As String = sr.ReadToEnd()
                sr.Close()

                HTML = Replace(HTML, "href=""/", "href=""" & AppSettings("GlobalRefererName") & "/")
                HTML = Replace(HTML, "src=""/", "src=""" & AppSettings("GlobalRefererName") & "/")

                Dim FromEmail As String = SysParam.GetValue(DB, "StoreOrderFromEmail")
                Dim FromName As String = SysParam.GetValue(DB, "StoreOrderFromName")
                Dim SiteName As String = AppSettings("GlobalWebsiteName")

                If FromName = "" Then FromName = FromEmail

                Core.SendHTMLMail(FromEmail, FromName, dbOrder.Email, Core.BuildFullName(dbOrder.BillingFirstName, String.Empty, dbOrder.BillingLastName), "Your " & AppSettings("GlobalWebsiteName") & " Order Confirmation!", HTML)

            Catch wex As System.Net.WebException
            End Try
		End Sub

		Public Shared Sub UpdateInventory(ByVal DB As Database, ByVal OrderId As Integer)
			Dim EnableInventoryManagement As Boolean = SysParam.GetValue(DB, "EnableInventoryManagement") = 1
			Dim EnableAttributeInventoryManagement As Boolean = SysParam.GetValue(DB, "EnableAttributeInventoryManagement") = 1
			Dim InventoryActionThreshold As Integer = SysParam.GetValue(DB, "InventoryActionThreshold")
			Dim InventoryWarningThreshold As Integer = SysParam.GetValue(DB, "InventoryWarningThreshold")

			If Not EnableInventoryManagement Then Exit Sub

			Dim SQL As String
			Dim sBody As String = String.Empty
			Dim dv As DataView
			Dim dbOrder As StoreOrderRow = StoreOrderRow.GetRow(DB, OrderId)
			Dim htTemplate As New Hashtable
			Dim htItems As New Hashtable

			SQL = "SELECT si.InventoryQty, si.ItemId, si.ItemName, si.TemplateId, COALESCE(si.InventoryWarningThreshold," & InventoryWarningThreshold & ") AS InventoryWarningThreshold, COALESCE(si.InventoryActionThreshold," & InventoryActionThreshold & ") AS InventoryActionThreshold, si.InventoryAction, soi.OrderItemId, soi.SKU, soi.Quantity FROM StoreOrderItem soi INNER JOIN StoreItem si ON soi.ItemId = si.ItemId WHERE soi.OrderId = " & OrderId
			dv = DB.GetDataTable(SQL).DefaultView

			For Each row As DataRowView In dv
				Dim IsAttributes As Boolean = False

				If EnableAttributeInventoryManagement Then
					If htTemplate(row("TemplateId") & "_IsAttributes") Is Nothing Then
						IsAttributes = DB.ExecuteScalar("SELECT TOP 1 IsAttributes FROM StoreItemTemplate WHERE TemplateId = " & row("TemplateId"))
						htTemplate(row("TemplateId") & "_IsAttributes") = IsAttributes
					Else
						IsAttributes = htTemplate(row("TemplateId") & "_IsAttributes")
					End If
				End If

				If IsAttributes Then
					Dim InventoryControlledAttributeId As Integer

					If htTemplate(row("TemplateId")) = Nothing Then
						Dim dvTemplate As DataView = DB.GetDataTable("exec sp_GetTemplateAttributeTreeByTemplate " & row("TemplateId")).DefaultView
						dvTemplate.RowFilter = "IsInventoryManagement = 1"
						If dvTemplate.Count > 0 Then
							InventoryControlledAttributeId = dvTemplate(dvTemplate.Count - 1)("TemplateAttributeId")
						End If
						htTemplate(row("TemplateId")) = InventoryControlledAttributeId
					Else
						InventoryControlledAttributeId = htTemplate(row("TemplateId"))
					End If

					If Not InventoryControlledAttributeId = Nothing Then
						SQL = "SELECT TOP 1 soia.ItemAttributeId, COALESCE(InventoryWarningThreshold," & row("InventoryWarningThreshold") & ") AS InventoryWarningThreshold, COALESCE(InventoryActionThreshold," & row("InventoryActionThreshold") & ") AS InventoryActionThreshold, InventoryQty FROM StoreItemAttribute sia INNER JOIN StoreOrderItemAttribute soia ON sia.ItemAttributeId = soia.ItemAttributeId WHERE OrderItemId = " & row("OrderItemId") & " AND soia.TemplateAttributeId = " & InventoryControlledAttributeId
						Dim dt As DataTable = DB.GetDataTable(SQL)
						If dt.Rows.Count > 0 Then
							Dim Validate As Boolean = False

							SQL = "UPDATE StoreItemAttribute SET InventoryQty = CASE WHEN InventoryQty - " & row("Quantity") & " < 0 THEN 0 ELSE InventoryQty - " & row("Quantity") & " END "
							If dt.Rows(0)("InventoryQty") > dt.Rows(0)("InventoryActionThreshold") AndAlso dt.Rows(0)("InventoryQty") - row("Quantity") <= dt.Rows(0)("InventoryActionThreshold") Then
								Select Case row("InventoryAction")
									Case "Disable"
										Validate = True
										SQL &= ", IsActive = 0 "
									Case "Backorder"
									Case "OutOfStock"
								End Select
							End If
							SQL &= " WHERE ItemAttributeId = " & dt.Rows(0)("ItemAttributeId")
							DB.ExecuteSQL(SQL)

							If Validate Then
								Try
									DB.BeginTransaction()

									SQL = "UPDATE StoreItemAttribute SET IsValidated = 0 WHERE ItemId = " & row("ItemId")
									DB.ExecuteSQL(SQL)

									StoreItemAttributeRow.ValidateAttributes(DB, row("ItemId"))

									DB.CommitTransaction()
								Catch ex As Exception
									DB.RollbackTransaction()
								End Try
							End If

							If dt.Rows(0)("InventoryQty") > dt.Rows(0)("InventoryWarningThreshold") AndAlso dt.Rows(0)("InventoryQty") - row("Quantity") <= dt.Rows(0)("InventoryWarningThreshold") Then
								sBody &= "Item Name: " & row("ItemName") & vbCrLf
								sBody &= "SKU: " & row("SKU") & vbCrLf
								sBody &= "Quantity: " & IIf(dt.Rows(0)("InventoryQty") - row("Quantity") < 0, 0, dt.Rows(0)("InventoryQty") - row("Quantity")) & vbCrLf
								sBody &= "Warning Level: " & dt.Rows(0)("InventoryWarningThreshold") & vbCrLf & vbCrLf
							End If
						End If
					End If
				Else
					If htItems(row("ItemId")) = Nothing Then htItems(row("ItemId")) = row("Quantity") Else htItems(row("ItemId")) += row("Quantity")
					SQL = "UPDATE StoreItem SET InventoryQty = CASE WHEN InventoryQty - " & DB.Number(htItems(row("ItemId"))) & " < 0 THEN 0 ELSE InventoryQty - " & row("Quantity") & " END "
					If row("InventoryQty") > row("InventoryActionThreshold") AndAlso row("InventoryQty") - htItems(row("ItemId")) <= row("InventoryActionThreshold") Then
						Select Case row("InventoryAction")
							Case "Disable"
								SQL &= ", IsActive = 0 "
							Case "Backorder"
							Case "OutOfStock"
						End Select
					End If
					SQL &= " WHERE ItemId = " & row("ItemId")
					DB.ExecuteSQL(SQL)

					If row("InventoryQty") > row("InventoryWarningThreshold") AndAlso row("InventoryQty") - htItems(row("ItemId")) <= row("InventoryWarningThreshold") Then
						sBody &= "Item Name: " & row("ItemName") & vbCrLf
						sBody &= "SKU: " & row("SKU") & vbCrLf
						sBody &= "Quantity: " & IIf(row("InventoryQty") - row("Quantity") < 0, 0, row("InventoryQty") - row("Quantity")) & vbCrLf
						sBody &= "Warning Level: " & row("InventoryWarningThreshold") & vbCrLf & vbCrLf
					End If
				End If
			Next

			If SysParam.GetValue(DB, "EnableInventoryWarningEmail") = 1 AndAlso Not sBody = String.Empty Then
				Try
					Dim Msg As New Net.Mail.MailMessage
					Dim Notify As New Notification()

					sBody &= "Global Warning Level: " & InventoryWarningThreshold
					Msg.Body = sBody
					Msg.Subject = "Inventory Warning"

					For Each address As String In SysParam.GetValue(DB, "InventoryWarningEmail").Split(";")
						Dim addr As New Net.Mail.MailAddress(address)
						Notify.Recipients.Add(addr)
						If Msg.From Is Nothing Then Msg.From = addr
					Next

					Notify.Message = Msg
					Notify.Queue()
				Catch ex As Exception
					Logger.Error(Logger.GetErrorMessage(ex))
				End Try
			End If
		End Sub

		Public Shared Function GenerateBreadCrumb(ByVal items() As String) As String
			Dim Result As String = "<p>"
			Dim Conn As String = String.Empty

			For Each item As String In items
				Dim arr As String() = item.Split("|")
				If arr.Length > 1 Then
					Result &= Conn & "<a href=""" & arr(1) & """>" & arr(0) & "</a>"
				Else
					Result &= Conn & arr(0)
				End If
				Conn = " &raquo; "
			Next
			Return Result & "</p>"
		End Function

        Public Shared Function GetItemsWeight(ByVal DB As Database, ByVal OrderId As Integer, ByVal RecipientId As Integer) As Double
            Return DB.ExecuteScalar("SELECT COALESCE(SUM(Weight*Quantity), 0) FROM StoreOrderItem WHERE OrderId = " & OrderId & " AND RecipientId = " & RecipientId)
        End Function

        Public Shared Sub GetItemsDimensions(ByVal DB As Database, ByVal OrderId As Integer, ByVal RecipientId As Integer, ByRef Thickness As Double, ByRef Width As Double, ByRef Height As Double)
            Dim SQL As String = " SELECT COALESCE(MAX(Width), 0) AS Width, COALESCE(MAX(Thickness), 0) AS Thickness, COALESCE(MAX(Height), 0) AS Height FROM StoreOrderItem WHERE OrderId = " & OrderId & " AND RecipientId = " & RecipientId
            Dim dr As SqlDataReader = Nothing
            Try
                dr = DB.GetReader(SQL)
                If dr.Read Then
                    Thickness = IIf(IsDBNull(dr("Thickness")), 0, dr("Thickness"))
                    Width = IIf(IsDBNull(dr("Width")), 0, dr("Width"))
                    Height = IIf(IsDBNull(dr("Height")), 0, dr("Height"))
                End If
            Catch ex As Exception
            Finally
                If Not dr Is Nothing Then dr.Close()
            End Try
        End Sub

        Public Shared Function GetAttributeText(ByVal Db As Database, ByVal OrderId As Integer, ByVal OrderItemId As Integer) As String
            Dim attributes As ItemAttributeCollection = StoreOrderItemRow.GetItemAttributeCollection(Db, OrderId, OrderItemId)
            Dim Result As String = String.Empty
            Dim Conn As String = String.Empty
            For Each attr As ItemAttribute In attributes
                Result &= Conn & attr.AttributeName & "=" & attr.AttributeValue
                Conn = "<br />"
            Next
            Return Result
        End Function

    End Class

    Public Structure ShoppingCartSummary
        Public Quantity As Integer
        Public Total As Double
    End Structure

    Public Structure BaseShipping
        Public AddressId As Integer
        Public Shipping As Double
    End Structure
End Namespace
