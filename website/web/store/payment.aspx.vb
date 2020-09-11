Imports Components
Imports DataLayer
Imports System.Data.SqlClient
Imports Utility

Partial Class store_payment
    Inherits SitePage

    Protected dbOrder As StoreOrderRow

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Cache.SetNoStore()
    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureSSL()
        ShoppingCart.EnsureOrder(DB)
        ShoppingCart.EnsureBillingInfo(DB, Session("OrderId"))
        ShoppingCart.EnsureShippingInfo(DB, Session("OrderId"))

        Dim OrderId As Integer = IIf(Session("OrderId") Is Nothing, 0, Session("OrderId"))
        Try
            DB.BeginTransaction()
            Dim ValidateErrorMessage As String = ShoppingCart.ValidateOrderItems(DB, OrderId)
            If Not ValidateErrorMessage = String.Empty Then
                AddError(ValidateErrorMessage)
            Else
                ShoppingCart.RecalculateShoppingCart(DB, OrderId)
            End If
            DB.CommitTransaction()
        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        Catch ex As ApplicationException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ex.Message)
        End Try

        ctrlCart.OrderId = OrderId
        ctrlCart.EditMode = True

        dbOrder = StoreOrderRow.GetRow(DB, OrderId)
        If Not IsPostBack Then
            BindData()
        End If

        ltlFullName.Text = Core.BuildFullName(dbOrder.BillingFirstName, String.Empty, dbOrder.BillingLastName)
        ltlCountry.Text = CountryRow.GetRowByCode(DB, dbOrder.BillingCountry).CountryName
    End Sub

    Private Sub BindData()
        drpCardType.DataSource = CreditCardTypeRow.GetAllCardTypes(DB)
        drpCardType.DataValueField = "CardTypeId"
        drpCardType.DataTextField = "Name"
        drpCardType.DataBind()
        drpCardType.Items.Insert(0, New ListItem("-- please select --", ""))

        txtCardholderName.Text = Core.BuildFullName(dbOrder.BillingFirstName, String.Empty, dbOrder.BillingLastName)
    End Sub

    Protected Sub btnProcess_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnProcess.Click
        If Not IsValid Then Exit Sub
        If ProcessOrder() Then

            ' REDIRECT TO THANK YOU PAGE
            Session("LastOrderId") = Session("OrderId")
            Session("OrderId") = Nothing
            Session("OrderNo") = Nothing

            CookieUtil.SetTripleDESEncryptedCookie("OrderId", Nothing)

            ' SEND NOTIFICATION EMAILS TO CLIENT AND RECIPIENT
            ShoppingCart.SendConfirmations(DB, dbOrder.OrderId)

			ShoppingCart.UpdateInventory(DB, dbOrder.OrderId)

            Response.Redirect("/store/confirm.aspx")
        End If
    End Sub

    Private Function ProcessOrder() As Boolean
        Try
            If Session("OrderNo") Is Nothing Then Session("OrderNo") = ShoppingCart.GenerateUniqueOrderNo(DB)

            'Save order information
            DB.BeginTransaction()
            dbOrder.MemberId = IIf(Session("MemberId") Is Nothing, 0, Session("MemberId"))
            dbOrder.OrderNo = Session("OrderNo")
            If SysParam.GetValue(DB, "SaveCreditCardInfo") = 1 Then
                dbOrder.CardholderName = txtCardholderName.Text
                dbOrder.CardTypeId = drpCardType.SelectedValue
                dbOrder.ExpirationDate = ctrlExpDate.Value
                dbOrder.CIDNumber = txtCID.Text
                dbOrder.CardNumber = txtCardNumber.Text
            End If
            dbOrder.Comments = txtComments.Text
            dbOrder.HowHeardId = drpDiscovery.SelectedID
            dbOrder.HowHeardName = IIf(drpDiscovery.SelectedValue = String.Empty, "", drpDiscovery.SelectedValue)
            DB.CommitTransaction()

            'Process payment (outside of the transaction)
            Dim Processor As Object = Nothing
            Dim TransactionType As TransactionTypeEnum

            Dim Address As New AddressInfo
            Address.AddressLn1 = dbOrder.BillingAddress1
            Address.AddressLn2 = dbOrder.BillingAddress2
            Address.City = dbOrder.BillingCity
            Address.Country = dbOrder.BillingCountry
            Address.State = dbOrder.BillingState
            Address.Zip = dbOrder.BillingZip
            Address.FirstName = dbOrder.BillingFirstName
            Address.LastName = dbOrder.BillingLastName

            Dim CC As New CreditCardInfo
            CC.CardHolderName = txtCardholderName.Text
            CC.CID = txtCID.Text
            CC.CreditCardNumber = txtCardNumber.Text
            CC.CreditCardType = CreditCardTypeRow.GetRow(DB, drpCardType.SelectedValue).Code
            CC.ExpMonth = Month(ctrlExpDate.Value)
            CC.ExpYear = Year(ctrlExpDate.Value)

            'Payflow payment processing
            If SysParam.GetValue(DB, "PayflowEnabled") = 1 Then
                Processor = New PayflowProPaymentProcessor()
                Processor.Username = SysParam.GetValue(DB, "PayflowUsername")
                Processor.Password = SysParam.GetValue(DB, "PayflowPassword")
                Processor.Custom1 = SysParam.GetValue(DB, "PayflowPartner")
                Processor.Custom2 = SysParam.GetValue(DB, "PayflowVendor")
                Processor.Timeout = SysParam.GetValue(DB, "PayflowTimeout")
                Processor.TestMode = SysParam.GetValue(DB, "TestMode")
                If SysParam.GetValue(DB, "PayflowTransactionType") = "A" Then
                    TransactionType = TransactionTypeEnum.Authorization
                Else
                    TransactionType = TransactionTypeEnum.Sale
                End If
            End If

            'Paypal payment processing
            If SysParam.GetValue(DB, "PayPalEnabled") = 1 Then
                Processor = New PayPalPaymentProcessor()
            End If

            'Authorize.net payment processing
            If SysParam.GetValue(DB, "AuthorizeNetEnabled") = 1 Then
                Processor = New AuthorizeNetPaymentProcessor()
                Processor.Username = SysParam.GetValue(DB, "AuthorizeNetAPILogin")
                Processor.Password = SysParam.GetValue(DB, "AuthorizeNetTransactionKey")
                Processor.TestMode = SysParam.GetValue(DB, "TestMode")
                If SysParam.GetValue(DB, "AuthorizeNetTransactionType") = "A" Then
                    TransactionType = TransactionTypeEnum.Authorization
                Else
                    TransactionType = TransactionTypeEnum.Sale
                End If
            End If

            Dim bError As Boolean = False
            If Not Processor Is Nothing Then
                Dim dbPaymentLog As New PaymentLogRow(DB)
                dbPaymentLog.Amount = dbOrder.Total
                dbPaymentLog.Description = "New Order"
                dbPaymentLog.OrderNo = dbOrder.OrderNo
                If Processor.SubmitPayment(dbOrder.OrderNo, TransactionType, CC, Address, dbOrder.Total) Then
                    dbPaymentLog.Response = Processor.FullResponse()
                    dbPaymentLog.TransactionNo = Processor.TransactionNo()
                    dbPaymentLog.Result = Processor.Result
                    dbPaymentLog.VerificationResponse = Processor.GetVerificationResponse()
                    If Not dbPaymentLog.VerificationResponse = String.Empty Then dbPaymentLog.IsHighRisk = True
                    dbPaymentLog.Insert()
                Else
                    dbPaymentLog.Response = Processor.FullResponse()
                    dbPaymentLog.TransactionNo = Processor.TransactionNo()
                    dbPaymentLog.Result = Processor.Result
                    dbPaymentLog.Insert()
                    AddError(Processor.ErrorMessage)
                    Return False
                End If
            End If

            'Complete order (outside of transaction)
            If Session("ref") <> Nothing Then dbOrder.ReferralCode = Session("ref")
            dbOrder.ProcessDate = Now()
            dbOrder.Update()

            Return True

        Catch ex As ApplicationException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ex.Message)
        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Function
End Class
