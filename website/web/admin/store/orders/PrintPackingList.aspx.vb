Imports Components
Imports DataLayer

Partial Class admin_store_orders_PrintPackingList
    Inherits SitePage

    Protected Sub admin_store_orders_PrintPackingList_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim Recipients As String, Items As String, OrderId As String, OrderGuid As String

        OrderGuid = Request.QueryString("OrderGuid")
        OrderId = Request.QueryString("OrderId")
        Items = Request.QueryString("Items")
        OrderId = DB.ExecuteScalar("SELECT TOP 1 OrderId FROM StoreOrder WHERE ProcessDate IS NOT NULL AND OrderId=" & DB.Number(OrderId) & " AND Guid=" & DB.Quote(OrderGuid))

        Dim dtRecipients As DataTable = DB.GetDataTable("SELECT DISTINCT RecipientId FROM StoreOrderItem WHERE OrderId=" & DB.Number(OrderId) & " AND OrderItemId IN " & DB.NumberMultiple(Items))
        For Each objRow As DataRow In dtRecipients.Rows
            If Items <> "" Then
                Dim c As RecipientPackingListControl = CType(LoadControl("/controls/RecipientPackingList.ascx"), RecipientPackingListControl)
                c.OrderId = OrderId
                c.RecipientId = objRow("RecipientId")
                c.CartItems = Items
                plcLists.Controls.Add(c)
            End If
        Next
    End Sub
End Class
