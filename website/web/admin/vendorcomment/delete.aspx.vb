Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class delete
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("VENDOR_COMMENTS")

        Dim VendorCommentID As Integer = Convert.ToInt32(Request("VendorCommentID"))
        Dim BuilderID As Integer = Convert.ToInt32(Request("BuilderID"))
        Dim vendorID As Integer = Convert.ToInt32(Request("VendorID"))
        Try
            DB.BeginTransaction()
            VendorCommentRow.RemoveRow(DB, VendorCommentID)

            DB.ExecuteSQL("Delete from VendorRatingCategoryRating where builderID = " & BuilderID & " AND VendorID = " & vendorID)
            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class

