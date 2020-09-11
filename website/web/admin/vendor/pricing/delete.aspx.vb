Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class delete
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("VENDOR_PRODUCT_PRICES")

        Dim VendorID As Integer = Convert.ToInt32(Request("VendorID"))
        Dim ProductID As Integer = Convert.ToInt32(Request("ProductID"))
        Try
            DB.BeginTransaction()
            VendorProductPriceRow.RemoveRow(DB, VendorID, ProductID)
            DB.CommitTransaction()

            Response.Redirect("default.aspx?VendorID=" & VendorID & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
