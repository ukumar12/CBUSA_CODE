Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected OrderDropID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("ORDER_DROPS")

		OrderDropID = Convert.ToInt32(Request("OrderDropID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
        drpState.DataSource = StateRow.GetStateList(DB)
		drpState.DataValueField = "StateId"
		drpState.DataTextField = "StateName"
		drpState.Databind
		drpState.Items.Insert(0, New ListItem("",""))
	
		If OrderDropID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbOrderDrop As OrderDropRow = OrderDropRow.GetRow(DB, OrderDropID)
		txtOrderID.Text = dbOrderDrop.OrderID
		txtDropName.Text = dbOrderDrop.DropName
		txtShipToAddress.Text = dbOrderDrop.ShipToAddress
		txtShipToAddress2.Text = dbOrderDrop.ShipToAddress2
		txtCity.Text = dbOrderDrop.City
		txtDeliveryInstructions.Text = dbOrderDrop.DeliveryInstructions
		txtNotes.Text = dbOrderDrop.Notes
		txtCreatorBuilderID.Text = dbOrderDrop.CreatorBuilderID
		ctrlZip.Value = dbOrderDrop.Zip
		dtRequestedDelivery.Value = dbOrderDrop.RequestedDelivery
		drpState.SelectedValue = dbOrderDrop.State
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbOrderDrop As OrderDropRow

			If OrderDropID <> 0 Then
				dbOrderDrop = OrderDropRow.GetRow(DB, OrderDropID)
			Else
				dbOrderDrop = New OrderDropRow(DB)
			End If
			dbOrderDrop.OrderID = txtOrderID.Text
			dbOrderDrop.DropName = txtDropName.Text
			dbOrderDrop.ShipToAddress = txtShipToAddress.Text
			dbOrderDrop.ShipToAddress2 = txtShipToAddress2.Text
			dbOrderDrop.City = txtCity.Text
			dbOrderDrop.DeliveryInstructions = txtDeliveryInstructions.Text
			dbOrderDrop.Notes = txtNotes.Text
			dbOrderDrop.CreatorBuilderID = txtCreatorBuilderID.Text
			dbOrderDrop.Zip = ctrlZip.Value
			dbOrderDrop.RequestedDelivery = dtRequestedDelivery.Value
			dbOrderDrop.State = drpState.SelectedValue
	
			If OrderDropID <> 0 Then
				dbOrderDrop.Update()
			Else
				OrderDropID = dbOrderDrop.Insert
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
		Response.Redirect("delete.aspx?OrderDropID=" & OrderDropID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
