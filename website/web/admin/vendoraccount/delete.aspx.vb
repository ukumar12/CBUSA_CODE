Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer
Imports Utilities

Partial Class delete
    Inherits AdminPage

    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private DeleteVendorAccountId As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("VENDOR_ACCOUNTS")

        Dim VendorAccountID As Integer = Convert.ToInt32(Request("VendorAccountID"))
        Try
            DB.BeginTransaction()
            Dim dbAcountVendor As VendorAccountRow = VendorAccountRow.GetRow(DB, VendorAccountID)

            dbAcountVendor.IsActive = False
            dbAcountVendor.Update()

            DB.ExecuteSQL("DELETE FROM VendorAccountVendorRole where VendorAccountID = " & VendorAccountID)

            '=================== Apala (Medullus) - Sales Force related code 11.08.2017 ===================
            '' Salesforce Integration
            'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
            'If sfHelper.Login() Then
            '    If sfHelper.DeleteAccountVendor(dbAcountVendor) = False Then
            '        'throw error
            '    End If
            'End If
            '===============================================================================================

            'log delete builder
            PageURL = Request.Url.ToString()
            CurrentUserId = Session("AdminId")
            UserName = Session("Username")
            DeleteVendorAccountId = VendorAccountID
            Core.DataLog("Admin", PageURL, CurrentUserId, "Delete Vendor Account", DeleteVendorAccountId, "", "", "", UserName)
            'end log

            DB.CommitTransaction()

            'DB.BeginTransaction()
            'VendorAccountRow.RemoveRow(DB, VendorAccountID)
            'DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class

