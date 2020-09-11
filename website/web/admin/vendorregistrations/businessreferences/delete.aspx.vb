Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class delete
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("VENDOR_REGISTRATIONS")

        Dim VendorRegistrationBusinessReferenceID As Integer = Convert.ToInt32(Request("VendorRegistrationBusinessReferenceID"))
        Dim VendorRegistrationBusinessReference As VendorRegistrationBusinessReferenceRow = VendorRegistrationBusinessReferenceRow.GetRow(Me.DB, VendorRegistrationBusinessReferenceID)
        Try
            DB.BeginTransaction()
            VendorRegistrationBusinessReferenceRow.RemoveRow(DB, VendorRegistrationBusinessReferenceID)
            DB.CommitTransaction()

            Response.Redirect("default.aspx?VendorRegistrationId=" & VendorRegistrationBusinessReference.VendorRegistrationID & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
