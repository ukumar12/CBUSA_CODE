Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class delete
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("VENDOR_REGISTRATIONS")

        Dim VendorRegistrationFinancialReferenceID As Integer = Convert.ToInt32(Request("VendorRegistrationFinancialReferenceID"))
        Dim VendorRegistrationFinancialReference As VendorRegistrationFinancialReferenceRow = VendorRegistrationFinancialReferenceRow.GetRow(Me.DB, VendorRegistrationFinancialReferenceID)
        Try
            DB.BeginTransaction()
            VendorRegistrationFinancialReferenceRow.RemoveRow(DB, VendorRegistrationFinancialReferenceID)
            DB.CommitTransaction()

            Response.Redirect("default.aspx?VendorRegistrationId=" & VendorRegistrationFinancialReference.VendorRegistrationID & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
