Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected StateId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("SHIPPING_TAX")

        StateId = Convert.ToInt32(Request("StateId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If StateId = 0 Then
            Exit Sub
        End If

        Dim dbState As StateRow = StateRow.GetRow(DB, StateId)
        ltlStateCode.Text = dbState.StateCode
        ltlStateName.Text = dbState.StateName
        txtTaxRate.Text = dbState.TaxRate
        chkTaxShipping.Checked = dbState.TaxShipping
        chkTaxGiftWrap.Checked = dbState.TaxGiftWrap
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbState As StateRow

            If StateId <> 0 Then
                dbState = StateRow.GetRow(DB, StateId)
            Else
                dbState = New StateRow(DB)
            End If
            dbState.TaxRate = txtTaxRate.Text
            dbState.TaxShipping = chkTaxShipping.Checked
            dbState.TaxGiftWrap = chkTaxGiftWrap.Checked

            If StateId <> 0 Then
                dbState.Update()
            Else
                StateId = dbState.Insert
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
End Class

