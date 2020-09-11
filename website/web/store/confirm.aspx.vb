Imports Components
Imports DataLayer
Imports System.Data.SqlClient
Imports Utility

Partial Class store_confirm
    Inherits SitePage

    Protected dbOrder As StoreOrderRow
    Protected dbCustomText As CustomTextRow
    Protected IsPrint As Boolean = False

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim OrderId As Integer = Session("LastOrderId")
        If Not Request("c") = String.Empty Then
            OrderId = Crypt.DecryptTripleDes(Request("OrderId"))
        End If
        If Not Request("print") = String.Empty Then
            IsPrint = True
        End If

        ctrlCart.OrderId = OrderId
        ctrlCart.EditMode = False

        dbOrder = StoreOrderRow.GetRow(DB, OrderId)
        dbCustomText = CustomTextRow.GetRowByCode(DB, "OrderConfirmation")
        If Not IsPostBack Then
            BindData()
        End If
        ltlFullName.Text = Core.BuildFullName(dbOrder.BillingFirstName, String.Empty, dbOrder.BillingLastName)
        ltlCountry.Text = CountryRow.GetRowByCode(DB, dbOrder.BillingCountry).CountryName
    End Sub

    Private Sub BindData()
        If dbOrder.CardTypeId = 0 Then
            ltlCardType.Text = "(not saved)"
        Else
            ltlCardType.Text = CreditCardTypeRow.GetRow(DB, dbOrder.CardTypeId).Name
        End If
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
