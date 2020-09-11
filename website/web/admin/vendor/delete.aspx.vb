Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer
Imports Utilities

Partial Class delete
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("VENDORS")

        Dim VendorID As Integer = Convert.ToInt32(Request("VendorID"))
        Try

            Dim dbVendor As VendorRow = VendorRow.GetRow(DB, VendorID)
            '' Salesforce Integration
            'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
            'If sfHelper.Login() Then
            '    If sfHelper.DeleteVendor(dbVendor) = False Then
            '        'throw error
            '    End If
            'End If

            'DB.BeginTransaction()
            'DB.ExecuteSQL("delete from VendorRegistration where VendorId=" & DB.Quote(VendorID))
            'DB.ExecuteSQL("delete from VendorAccount where VendorId=" & DB.Quote(VendorID))
            'DB.ExecuteSQL("delete from VendorSupplyPhase where VendorId=" & DB.Quote(VendorID))
            'VendorRow.RemoveRow(DB, VendorID)
            'DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            'AddError(ErrHandler.ErrorText(ex.Message))
            AddError(ex.Message)
        End Try
    End Sub
End Class
