Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected PromotionId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("MARKETING_TOOLS")

        PromotionId = Convert.ToInt32(Request("PromotionId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If PromotionId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbStorePromotion As StorePromotionRow = StorePromotionRow.GetRow(DB, PromotionId)
        txtPromotionName.Text = dbStorePromotion.PromotionName
        txtPromotionCode.Text = dbStorePromotion.PromotionCode
        drpPromotionType.SelectedValue = dbStorePromotion.PromotionType
        txtMessage.Text = dbStorePromotion.Message
        txtDiscount.Text = dbStorePromotion.Discount
        If dbStorePromotion.MinimumPurchase <> Nothing Then txtMinimumPurchase.Text = dbStorePromotion.MinimumPurchase
        If dbStorePromotion.MaximumPurchase <> Nothing Then txtMaximumPurchase.Text = dbStorePromotion.MaximumPurchase
        dtStartDate.Value = dbStorePromotion.StartDate
        dtEndDate.Value = dbStorePromotion.EndDate
        chkIsItemSpecific.Checked = dbStorePromotion.IsItemSpecific
        chkIsFreeShipping.Checked = dbStorePromotion.IsFreeShipping
        chkIsActive.Checked = dbStorePromotion.IsActive
        If dbStorePromotion.NumberSent > 0 Then
            txtNumberSent.Text = dbStorePromotion.NumberSent
        End If
        txtDeliveryMethod.Text = dbStorePromotion.DeliveryMethod
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbStorePromotion As StorePromotionRow

            If PromotionId <> 0 Then
                dbStorePromotion = StorePromotionRow.GetRow(DB, PromotionId)
            Else
                dbStorePromotion = New StorePromotionRow(DB)
            End If
            dbStorePromotion.PromotionName = txtPromotionName.Text
            dbStorePromotion.PromotionCode = txtPromotionCode.Text
            dbStorePromotion.PromotionType = drpPromotionType.SelectedValue
            dbStorePromotion.Message = txtMessage.Text
            dbStorePromotion.Discount = txtDiscount.Text
            dbStorePromotion.DeliveryMethod = txtDeliveryMethod.Text
            If IsNumeric(txtNumberSent.Text) Then
                dbStorePromotion.NumberSent = txtNumberSent.Text
            End If

            If txtMinimumPurchase.Text <> "" Then dbStorePromotion.MinimumPurchase = txtMinimumPurchase.Text Else dbStorePromotion.MinimumPurchase = Nothing
            If txtMaximumPurchase.Text <> "" Then dbStorePromotion.MaximumPurchase = txtMaximumPurchase.Text Else dbStorePromotion.MaximumPurchase = Nothing
            dbStorePromotion.StartDate = dtStartDate.Value
            dbStorePromotion.EndDate = dtEndDate.Value
            dbStorePromotion.IsItemSpecific = chkIsItemSpecific.Checked
            dbStorePromotion.IsFreeShipping = chkIsFreeShipping.Checked
            dbStorePromotion.IsActive = chkIsActive.Checked

            If PromotionId <> 0 Then
                dbStorePromotion.Update()
            Else
                PromotionId = dbStorePromotion.Insert
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
        Response.Redirect("delete.aspx?PromotionId=" & PromotionId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
