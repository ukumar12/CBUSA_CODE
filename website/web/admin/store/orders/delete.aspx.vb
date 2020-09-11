Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class delete
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("ORDERS")

        Dim OrderId As Integer = Convert.ToInt32(Request("OrderId"))
        Try
            DB.BeginTransaction()

            Dim dbOrder As StoreOrderRow = StoreOrderRow.GetRow(DB, OrderId)
            dbOrder.ProcessDate = Nothing
            dbOrder.Update()

            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class

