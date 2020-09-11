Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected OrderStatusID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("ORDER_STATUS")

		OrderStatusID = Convert.ToInt32(Request("OrderStatusID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If OrderStatusID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbOrderStatus As OrderStatusRow = OrderStatusRow.GetRow(DB, OrderStatusID)
		txtOrderStatus.Text = dbOrderStatus.OrderStatus
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbOrderStatus As OrderStatusRow

			If OrderStatusID <> 0 Then
				dbOrderStatus = OrderStatusRow.GetRow(DB, OrderStatusID)
			Else
				dbOrderStatus = New OrderStatusRow(DB)
			End If
			dbOrderStatus.OrderStatus = txtOrderStatus.Text
	
			If OrderStatusID <> 0 Then
				dbOrderStatus.Update()
			Else
				OrderStatusID = dbOrderStatus.Insert
			End If
	
			DB.CommitTransaction()

	
			Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

		Catch ex As SqlException
			If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
		End Try
	End Sub

	Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
		Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
	End Sub

	Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
		Response.Redirect("delete.aspx?OrderStatusID=" & OrderStatusID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
