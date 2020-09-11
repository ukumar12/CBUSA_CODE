Imports Components
Imports DataLayer
Imports System.Net.Mail
Imports System.Configuration.ConfigurationManager

Public Class SendInventoryNotifications

	Public Shared Sub Run(ByVal DB As Database)
		If Not SysParam.GetValue(DB, "EnableInventoryManagement") = 1 Then
			Exit Sub
		End If

		Console.WriteLine("Running SendInventoryNotifications ... ")

		Try
			SendItemNotifications(DB)
			SendAttributeNotifications(DB)
		Catch ex As Exception
			Logger.Error(Logger.GetErrorMessage(ex))
		End Try
	End Sub

	Private Shared Sub SendItemNotifications(ByVal DB As Database)
		Dim SQL As String = "SELECT ItemId, ItemName FROM StoreItem WHERE SendNotification = 1"
		Dim dt As DataTable = DB.GetDataTable(SQL)

		If dt.Rows.Count = 0 Then Exit Sub

		SQL = "UPDATE StoreItem SET SendNotification = 0 WHERE SendNotification = 1"
		DB.ExecuteSQL(SQL)

		For Each row As DataRow In dt.Rows
			Dim Message As New MailMessage
			Dim DisplayName, sBody As String

			SQL = "SELECT Email, ItemNotifyId FROM StoreItemNotify WHERE SendDate IS NULL AND ItemId = " & row("ItemId")
			Dim dtNotify As DataTable = DB.GetDataTable(SQL)

			If SysParam.GetValue(DB, "InventoryReplenishmentFromName") = Nothing Then DisplayName = SysParam.GetValue(DB, "InventoryReplenishmentFromEmail") Else DisplayName = SysParam.GetValue(DB, "InventoryReplenishmentFromName")

			sBody = "<p><a href=""" & AppSettings("baseUrl") & "/store/item.aspx?ItemId=" & row("ItemId") & "&ItemNotifyId=%%ItemNotifyId%%"">Click here</a> to view this product.</p>"

			For Each r As DataRow In dtNotify.Rows
				Core.SendHTMLMail(SysParam.GetValue(DB, "InventoryReplenishmentFromEmail"), DisplayName, r("Email"), r("Email"), "Product " & row("ItemName") & " has recently become available for purchase!", sBody.Replace("%%ItemNotifyId%%", r("ItemNotifyId")))
			Next
		Next
	End Sub

	Private Shared Sub SendAttributeNotifications(ByVal DB As Database)
		Dim SQL As String = "SELECT ItemId, ItemAttributeId FROM StoreItemAttribute WHERE SendNotification = 1"
		Dim dt As DataTable = DB.GetDataTable(SQL)
		Dim ht As New Hashtable
		Dim htTree As New Hashtable

		If dt.Rows.Count = 0 Then Exit Sub

		SQL = "UPDATE StoreItemAttribute SET SendNotification = 0 WHERE SendNotification = 1"
		DB.ExecuteSQL(SQL)

		For Each r As DataRow In dt.Rows
			If ht(r("ItemId")) Is Nothing Then ht(r("ItemId")) = StoreItemRow.GetRow(DB, r("ItemId"))

			SQL = "sp_GetAttributeTreeTableLayout " & r("ItemId") & ", " & ht(r("ItemId")).TemplateId
			If htTree(r("ItemId")) Is Nothing Then htTree(r("ItemId")) = DB.GetDataTable(SQL).DefaultView

			SendNotifications(DB, ht(r("ItemId")), htTree(r("ItemId")), r("ItemAttributeId"))
		Next
	End Sub

	Private Shared Sub SendNotifications(ByVal DB As Database, ByVal dbItem As StoreItemRow, ByVal dv As DataView, ByVal ItemAttributeId As Integer)
		Dim SQL, DisplayName, sBody As String
		Dim AttributeString As String = String.Empty
		Dim dt As DataTable
		dv.RowFilter = "ItemAttributeId = " & ItemAttributeId

		If dv.Count > 0 Then
			SQL = "SELECT AttributeName + '=' + AttributeValue AS AttributeString FROM StoreItemTemplateAttribute a INNER JOIN StoreItemAttribute ia ON a.TemplateAttributeId = ia.TemplateAttributeId WHERE ItemAttributeId IN " & DB.NumberMultiple(dv(0)("ItemAttributeIds") & " ORDER BY a.SortOrder")
			dt = DB.GetDataTable(SQL)
			For Each r As DataRow In dt.Rows
				AttributeString &= IIf(AttributeString = String.Empty, "", ", ") & r("AttributeString")
			Next

			Dim Subject As String = "Product " & dbItem.ItemName & " (" & AttributeString & ") has recently become available for purchase!"
			If SysParam.GetValue(DB, "InventoryReplenishmentFromName") = Nothing Then DisplayName = SysParam.GetValue(DB, "InventoryReplenishmentFromEmail") Else DisplayName = SysParam.GetValue(DB, "InventoryReplenishmentFromName")
			sBody = "<p><a href=""" & AppSettings("baseUrl") & "/store/item.aspx?ItemId=" & dbItem.ItemId & "&ItemNotifyId=%%ItemNotifyId%%"">Click here</a> to view this product.</p>"

			SQL = "SELECT Email, ItemNotifyId FROM StoreItemNotify WHERE SendDate IS NULL AND ItemAttributeId = " & ItemAttributeId
			dt = DB.GetDataTable(SQL)

			SQL = "UPDATE StoreItemNotify SET SendDate = GETDATE() WHERE SendDate IS NULL AND ItemAttributeId = " & ItemAttributeId
			DB.ExecuteSQL(SQL)

			For Each r As DataRow In dt.Rows
				Core.SendHTMLMail(SysParam.GetValue(DB, "InventoryReplenishmentFromEmail"), DisplayName, r("Email"), r("Email"), Subject, sBody.Replace("%%ItemNotifyId%%", r("ItemNotifyId")))
			Next
		End If
	End Sub
End Class
