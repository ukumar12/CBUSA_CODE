Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class add
    Inherits AdminPage

    Protected RebateTermsID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("VENDOR_ACCOUNTS")

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

    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbRebateTerm As RebateTermRow

            dbRebateTerm = New RebateTermRow(DB)

            dbRebateTerm.StartYear = txtStartYear.Text
            dbRebateTerm.StartQuarter = txtStartQuarter.Text
            dbRebateTerm.PurchaseRangeFloor = 0
            dbRebateTerm.PurchaseRangeCeiling = 999999999
            dbRebateTerm.RebatePercentage = txtRebatePercentage.Text
            dbRebateTerm.CreatorVendorAccountID = 1
            dbRebateTerm.ApproverAdminID = LoggedInAdminId
            dbRebateTerm.LogMsg = txtLogMsg.Text
            dbRebateTerm.VendorID = drpVendorID.SelectedValue
            dbRebateTerm.IsAnnualPurchaseRange = False

            RebateTermsID = dbRebateTerm.Insert
            
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

