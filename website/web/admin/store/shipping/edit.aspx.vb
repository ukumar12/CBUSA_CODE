Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected MethodId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("SHIPPING_TAX")

        MethodId = Convert.ToInt32(Request("MethodId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If MethodId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbStoreShippingMethod As StoreShippingMethodRow = StoreShippingMethodRow.GetRow(DB, MethodId)
        txtName.Text = dbStoreShippingMethod.Name
        txtUPSCode.Text = dbStoreShippingMethod.UPSCode
        txtFedExCode.Text = dbStoreShippingMethod.FedExCode
        chkIsActive.Checked = dbStoreShippingMethod.IsActive
        chkIsInternational.Checked = dbStoreShippingMethod.IsInternational
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbStoreShippingMethod As StoreShippingMethodRow

            If MethodId <> 0 Then
                dbStoreShippingMethod = StoreShippingMethodRow.GetRow(DB, MethodId)
            Else
                dbStoreShippingMethod = New StoreShippingMethodRow(DB)
            End If
            dbStoreShippingMethod.Name = txtName.Text
            dbStoreShippingMethod.UPSCode = txtUPSCode.Text
            dbStoreShippingMethod.FedExCode = txtFedExCode.Text
            dbStoreShippingMethod.IsActive = chkIsActive.Checked
            dbStoreShippingMethod.IsInternational = chkIsInternational.Checked

            If MethodId <> 0 Then
                dbStoreShippingMethod.Update()
            Else
                MethodId = dbStoreShippingMethod.Insert
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
        Response.Redirect("delete.aspx?MethodId=" & MethodId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

