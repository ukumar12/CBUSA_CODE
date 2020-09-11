Imports Components
Imports DataLayer
Imports System.Data.SqlClient
Imports Utility

Public Class OrderSTatus_orderhistory_view
    Inherits SitePage

    Private OrderId As Integer
    Protected dbOrder As StoreOrderRow

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim OrderId As Integer = Crypt.DecryptTripleDes(Request("OrderId"))

        dbOrder = StoreOrderRow.GetRow(DB, OrderId)

        ctrlCart.OrderId = OrderId

    End Sub


End Class