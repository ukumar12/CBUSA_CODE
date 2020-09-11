Imports Components
Imports DataLayer
Imports System.Data.SqlClient

Public Class Member_orderhistory_view
    Inherits SitePage

    Private OrderId As Integer
    Protected dbOrder As StoreOrderRow

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim OrderId As Integer
        If Session("ViewOrderId") = Nothing Then
            EnsureMemberOrderHistoryAccess()
			OrderId = IIf(IsNumeric(Request("OrderId")), Request("OrderId"), 0)
            dbOrder = StoreOrderRow.GetRow(DB, OrderId)
            If dbOrder.MemberId <> Session("memberId") Then Response.Redirect("/members/orders/")
            ltlOrderList.Text = "<a href=""default.aspx"">&laquo; Go back to Order List</a>"
        Else
            OrderId = Session("ViewOrderId")
            dbOrder = StoreOrderRow.GetRow(DB, OrderId)
            ltlOrderList.Text = ""
        End If

        ctrlCart.OrderId = OrderId
        BindData()
    End Sub

    Private Sub BindData()
        If dbOrder.CardTypeId = 0 Then
            ltlCardType.Text = "(not saved)"
        Else
            ltlCardType.Text = CreditCardTypeRow.GetRow(DB, dbOrder.CardTypeId).Name
        End If
        ltlFullName.Text = Core.BuildFullName(dbOrder.BillingFirstName, String.Empty, dbOrder.BillingLastName)
        ltlCountry.Text = CountryRow.GetRowByCode(DB, dbOrder.BillingCountry).CountryName
        ltlCardHolderName.Text = IIf(dbOrder.CardholderName = String.Empty, "(not saved)", dbOrder.CardholderName)
        ltlCardNumber.Text = IIf(dbOrder.CardNumber = String.Empty, "(not saved)", dbOrder.StarredCardNumber)
        If dbOrder.ExpirationDate = String.Empty Then
            ltlExpirationDate.Text = "(not saved)"
        Else
            Dim ExpirationDate As DateTime = dbOrder.ExpirationDate
            ltlExpirationDate.Text = Month(ExpirationDate) & "/" & Year(ExpirationDate)
        End If
        ltlCID.Text = IIf(dbOrder.CIDNumber = String.Empty, "(not saved)", dbOrder.StarredCIDNumber)
        ltlHowHeard.Text = IIf(dbOrder.HowHeardId = Nothing, "not specified", dbOrder.HowHeardName)
        trHowHeard.Visible = Not (dbOrder.HowHeardId = 0)
        ltlComments.Text = IIf(dbOrder.Comments = String.Empty, "none", dbOrder.Comments)
    End Sub

End Class