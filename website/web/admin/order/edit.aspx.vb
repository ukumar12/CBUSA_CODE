Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected OrderID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("ORDERS")

        OrderID = Convert.ToInt32(Request("OrderID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        drpVendorID.Datasource = VendorRow.GetList(DB, "CompanyName")
        drpVendorID.DataValueField = "VendorID"
        drpVendorID.DataTextField = "CompanyName"
        drpVendorID.Databind()
        drpVendorID.Items.Insert(0, New ListItem("", ""))

        drpBuilderID.Datasource = BuilderRow.GetList(DB, "CompanyName")
        drpBuilderID.DataValueField = "BuilderID"
        drpBuilderID.DataTextField = "CompanyName"
        drpBuilderID.Databind()
        drpBuilderID.Items.Insert(0, New ListItem("", ""))

        drpProjectID.Datasource = ProjectRow.GetList(DB, "ProjectName")
        drpProjectID.DataValueField = "ProjectID"
        drpProjectID.DataTextField = "ProjectName"
        drpProjectID.Databind()
        drpProjectID.Items.Insert(0, New ListItem("", ""))

        drpOrderStatusID.Datasource = OrderStatusRow.GetList(DB, "OrderStatus")
        drpOrderStatusID.DataValueField = "OrderStatusID"
        drpOrderStatusID.DataTextField = "OrderStatus"
        drpOrderStatusID.Databind()
        drpOrderStatusID.Items.Insert(0, New ListItem("", ""))

        If OrderID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbOrder As OrderRow = OrderRow.GetRow(DB, OrderID)
        txtHistoricID.Text = dbOrder.HistoricID
        txtOrderNumber.Text = dbOrder.OrderNumber
        txtHistoricVendorID.Text = dbOrder.HistoricVendorID
        txtHistoricBuilderID.Text = dbOrder.HistoricBuilderID
        txtHistoricProjectID.Text = dbOrder.HistoricProjectID
        txtTitle.Text = dbOrder.Title
        txtPONumber.Text = dbOrder.PONumber
        txtOrdererFirstName.Text = dbOrder.OrdererFirstName
        txtOrdererLastName.Text = dbOrder.OrdererLastName
        txtOrdererEmail.Text = dbOrder.OrdererEmail
        txtOrdererPhone.Text = dbOrder.OrdererPhone
        txtSuperFirstName.Text = dbOrder.SuperFirstName
        txtSuperLastName.Text = dbOrder.SuperLastName
        txtSuperEmail.Text = dbOrder.SuperEmail
        txtSuperPhone.Text = dbOrder.SuperPhone
        txtSubtotal.Text = dbOrder.Subtotal
        txtTax.Text = dbOrder.Tax
        txtTotal.Text = dbOrder.Total
        txtHistoricOrderStatusID.Text = dbOrder.HistoricOrderStatusID
        txtDeliveryInstructions.Text = dbOrder.DeliveryInstructions
        txtNotes.Text = dbOrder.Notes
        txtRemoteIP.Text = dbOrder.RemoteIP
        txtCreatorBuilderID.Text = dbOrder.CreatorBuilderID
        dtRequestedDelivery.Value = dbOrder.RequestedDelivery
        drpVendorID.SelectedValue = dbOrder.VendorID
        drpBuilderID.SelectedValue = dbOrder.BuilderID
        drpProjectID.SelectedValue = dbOrder.ProjectID
        drpOrderStatusID.SelectedValue = dbOrder.OrderStatusID
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbOrder As OrderRow

            If OrderID <> 0 Then
                dbOrder = OrderRow.GetRow(DB, OrderID)
            Else
                dbOrder = New OrderRow(DB)
            End If
            dbOrder.HistoricID = txtHistoricID.Text
            dbOrder.OrderNumber = txtOrderNumber.Text
            dbOrder.HistoricVendorID = txtHistoricVendorID.Text
            dbOrder.HistoricBuilderID = txtHistoricBuilderID.Text
            dbOrder.HistoricProjectID = txtHistoricProjectID.Text
            dbOrder.Title = txtTitle.Text
            dbOrder.PONumber = txtPONumber.Text
            dbOrder.OrdererFirstName = txtOrdererFirstName.Text
            dbOrder.OrdererLastName = txtOrdererLastName.Text
            dbOrder.OrdererEmail = txtOrdererEmail.Text
            dbOrder.OrdererPhone = txtOrdererPhone.Text
            dbOrder.SuperFirstName = txtSuperFirstName.Text
            dbOrder.SuperLastName = txtSuperLastName.Text
            dbOrder.SuperEmail = txtSuperEmail.Text
            dbOrder.SuperPhone = txtSuperPhone.Text
            dbOrder.Subtotal = txtSubtotal.Text
            dbOrder.Tax = txtTax.Text
            dbOrder.Total = txtTotal.Text
            dbOrder.HistoricOrderStatusID = txtHistoricOrderStatusID.Text
            dbOrder.DeliveryInstructions = txtDeliveryInstructions.Text
            dbOrder.Notes = txtNotes.Text
            dbOrder.RemoteIP = txtRemoteIP.Text
            dbOrder.CreatorBuilderID = txtCreatorBuilderID.Text
            dbOrder.RequestedDelivery = dtRequestedDelivery.Value
            dbOrder.VendorID = drpVendorID.SelectedValue
            dbOrder.BuilderID = drpBuilderID.SelectedValue
            dbOrder.ProjectID = drpProjectID.SelectedValue
            dbOrder.OrderStatusID = drpOrderStatusID.SelectedValue

            If OrderID <> 0 Then
                dbOrder.Update()
            Else
                OrderID = dbOrder.Insert
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
        Response.Redirect("delete.aspx?OrderID=" & OrderID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

