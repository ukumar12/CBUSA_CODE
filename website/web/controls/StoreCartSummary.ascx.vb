Imports Components
Imports DataLayer

Partial Class StoreCartSummary
    Inherits BaseControl

    Protected Summary As StoreOrderSummary
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Summary = StoreOrderRow.GetOrderSummary(DB, Session("OrderId"))
    End Sub
End Class
