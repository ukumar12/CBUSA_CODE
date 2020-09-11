Imports Components
Imports DataLayer
Imports System.Net.Mail
Imports System.Configuration.ConfigurationManager

Public Class ArchiveIncompleteOrders
	Public Shared Sub Run(ByVal DB As Database)
		If Not SysparamRow.GetRow(DB, "EnableInventoryManagement").Value = 1 Then
			Exit Sub
		End If

		Console.WriteLine("Running ArchiveIncompleteOrders ... ")

		Dim Days As Integer = SysparamRow.GetRow(DB, "ArchiveIncompleteOrderDays").Value

		Try
			Dim SQL, SelectSQL As String
			Dim iRowsAffected As Integer

			SelectSQL = "SELECT OrderId FROM StoreOrder WHERE CreateDate < " & DB.Quote(Now.Date.AddDays(-1 * Days)) & " AND NOT EXISTS (SELECT TOP 1 OrderItemId FROM StoreOrderItem i WHERE StoreOrder.OrderId = i.OrderId AND i.ModifyDate >= " & DB.Quote(Now.Date.AddDays(-1 * Days)) & ") AND OrderNo IS NULL AND ProcessDate IS NULL"

			SQL = "INSERT INTO ArchiveStoreOrder SELECT * FROM StoreOrder WHERE OrderId IN (" & SelectSQL & ") AND NOT EXISTS (SELECT TOP 1 OrderId FROM ArchiveStoreOrder a WHERE a.OrderId = StoreOrder.OrderId)"
			iRowsAffected = DB.ExecuteSQL(SQL)

			SQL = "INSERT INTO ArchiveStoreOrderRecipient SELECT * FROM StoreOrderRecipient WHERE OrderId IN (" & SelectSQL & ") AND NOT EXISTS (SELECT TOP 1 RecipientId FROM ArchiveStoreOrderRecipient r WHERE r.RecipientId = StoreOrderRecipient.RecipientId)"
			iRowsAffected = DB.ExecuteSQL(SQL)

			SQL = "INSERT INTO ArchiveStoreOrderItem SELECT * FROM StoreOrderItem WHERE OrderId IN (" & SelectSQL & ") AND NOT EXISTS (SELECT TOP 1 OrderItemId FROM ArchiveStoreOrderItem i WHERE i.OrderItemId = StoreOrderItem.OrderItemId)"
			iRowsAffected = DB.ExecuteSQL(SQL)

			SQL = "INSERT INTO ArchiveStoreOrderItemAttribute SELECT * FROM StoreOrderItemAttribute WHERE OrderId IN (" & SelectSQL & ") AND NOT EXISTS (SELECT TOP 1 UniqueId FROM ArchiveStoreOrderItemAttribute a WHERE a.UniqueId = StoreOrderItemAttribute.UniqueId)"
			iRowsAffected = DB.ExecuteSQL(SQL)

			SQL = "DELETE FROM StoreOrderItemAttribute WHERE OrderId IN (" & SelectSQL & ") AND EXISTS (SELECT TOP 1 UniqueId FROM ArchiveStoreOrderItemAttribute a WHERE a.UniqueId = StoreOrderItemAttribute.UniqueId)"
			iRowsAffected = DB.ExecuteSQL(SQL)

			SQL = "DELETE FROM StoreOrderItem WHERE OrderId IN (" & SelectSQL & ") AND EXISTS (SELECT TOP 1 OrderItemId FROM ArchiveStoreOrderItem i WHERE i.OrderItemId = StoreOrderItem.OrderItemId)"
			iRowsAffected = DB.ExecuteSQL(SQL)

			SQL = "DELETE FROM StoreOrderRecipient WHERE OrderId IN (" & SelectSQL & ") AND EXISTS (SELECT TOP 1 RecipientId FROM ArchiveStoreOrderRecipient r WHERE r.RecipientId = StoreOrderRecipient.RecipientId)"
			iRowsAffected = DB.ExecuteSQL(SQL)

			SQL = "DELETE FROM StoreOrder WHERE OrderId IN (" & SelectSQL & ") AND EXISTS (SELECT TOP 1 OrderId FROM ArchiveStoreOrder a WHERE a.OrderId = StoreOrder.OrderId)"
			iRowsAffected = DB.ExecuteSQL(SQL)

		Catch ex As Exception
			Logger.Error(Logger.GetErrorMessage(ex))
		End Try
	End Sub

End Class
