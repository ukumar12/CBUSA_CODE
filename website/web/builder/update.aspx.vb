Imports Components
Imports DataLayer
Imports Vindicia
Partial Class builder_update
    Inherits SitePage

    Private dbBuilder As BuilderRow
    Private dbBuilderAccount As BuilderAccountRow
    Private currentAutoBill As Vindicia.AutoBill
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        dbBuilder = BuilderRow.GetRow(DB, Session("BuilderId"))
        dbBuilderAccount = BuilderAccountRow.GetRow(DB, Session("BuilderAccountId"))

        If dbBuilder.BuilderID = Nothing Or dbBuilderAccount.BuilderAccountID = Nothing Then
            Response.Redirect("/default.aspx")
        End If

        Dim dbRegistration As BuilderRegistrationRow = BuilderRegistrationRow.GetRowByBuilder(DB, dbBuilder.BuilderID)
        'Per Brian: Remove New Year Check 010410
        'If dbRegistration.BuilderRegistrationID = Nothing OrElse dbRegistration.CompleteDate.Year <> Now.Year Then
        If dbRegistration.BuilderRegistrationID = Nothing Then
            Response.Redirect("/forms/builder-registration/default.aspx")
        End If

        If SysParam.GetValue(DB, "UseEntitlements") Then
            If dbBuilder.SkipEntitlementCheck Then
                Response.Redirect("/builder/")
            End If
        End If

        If Not IsPostBack Then

            Dim p As New VindiciaPaymentProcessor(DB)
            p.IsTestMode = SysParam.GetValue(DB, "TestMode")


            'Make sure builder exists in vindicia
            If Not p.EnsureVindiciaAccount(dbBuilder) Then
                Response.Redirect("/forms/builder-registration/payment.aspx")
            End If


            'Check autobills and transaction history to see if builder has to pay for anything.
            'Dim IsRegistrationActive As Boolean = False 'Client requested not to validate for regitration fee anymore because past members have paid this with a check.
            Dim IsSubscriptionActive As Boolean = False

            Dim autoBills() As Vindicia.AutoBill = p.GetAutobills(dbBuilder)
            For Each ab As Vindicia.AutoBill In autoBills

                Dim Product As String = ab.items(0).product.merchantProductId
                If Product = "Registration" Then
                    'If ab.status = Vindicia.AllDataTypes.AutoBillStatus.Active Then
                    '    IsRegistrationActive = True
                    'End If
                ElseIf Product = "Subscription" Then
                    If ab.status = Vindicia.AutoBillStatus.Active Then
                        IsSubscriptionActive = True
                        currentAutoBill = ab
                    End If
                End If
            Next

            'If Not IsRegistrationActive Then
            '    Dim transactionHistory() As Vindicia.AllDataTypes.Transaction = p.GetTransactionHistory(dbBuilder)
            '    For Each th As Vindicia.AllDataTypes.Transaction In transactionHistory
            '        If th.status.status <> Vindicia.AllDataTypes.TransactionStatusType.Cancelled Then
            '            Dim Product As String = th.transactionItems(0).sku
            '            If Product = "Registration" Then
            '                IsRegistrationActive = True
            '            End If
            '        End If
            '    Next
            'End If

            'If builder has to pay for anything always take them to registration payment so they can see what they're being charged.
            If Not IsSubscriptionActive Then
                Response.Redirect("/forms/builder-registration/payment.aspx")
            End If
            'Get future payments
            Try
                Dim FutureRebills() As Vindicia.Transaction = Nothing
                Dim ret As Vindicia.Return = currentAutoBill.fetchFutureRebills("", 1, FutureRebills)

                If ret.returnCode = 200 Then
                    Dim dt As Date = FutureRebills(0).timestamp
                    Dim amount As Decimal = FutureRebills(0).amount
                    Dim taxamount As Decimal = p.CalculateTax(dbBuilder, 1, FutureRebills(0).amount)
                    ltlWhen.Text = dt.ToShortDateString
                    ltlTotal.Text = taxamount + amount
                    ltlTotalTax.Text = taxamount
                    ltlPrice.Text = amount
                Else
                    divfuturePayment.Visible = False
                End If
            Catch ex As Exception
                divfuturePayment.Visible = False
                Logger.Error("future payment error : " & Session("BuilderId") & " " & ex.ToString)
            End Try

            'drpCardType.DataSource = CreditCardTypeRow.GetAllCardTypes(DB)
            'drpCardType.DataValueField = "CardTypeId"
            'drpCardType.DataTextField = "Name"
            'drpCardType.DataBind()
            'drpCardType.Items.Insert(0, New ListItem("-- please select --", ""))

            'drpState.DataSource = StateRow.GetStateList(DB)
            'drpState.DataTextField = "StateCode"
            'drpState.DataValueField = "StateCode"
            'drpState.DataBind()

            'LoadFromDB()

            vnhostedPayment.Builder = dbBuilder
            vnhostedPayment.RequestedFromUpdatePage = True
        End If
        ' EnableDisableBillingValidators()

    End Sub



    'Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click

    '    If Not IsValid Then
    '        Exit Sub
    '    End If


    '    Dim cc As New CreditCardInfo()
    '    cc.CardHolderName = txtCardholderFirstName.Text & " " & txtCardholderLastName.Text
    '    cc.CID = txtCID.Text
    '    cc.CreditCardNumber = txtCardNumber.Text.Replace("-", "")
    '    cc.CreditCardType = drpCardType.SelectedValue
    '    cc.ExpMonth = Month(ctrlExpDate.Value)
    '    cc.ExpYear = Year(ctrlExpDate.Value)

    '    Dim addr As New VindiciaAddressInfo
    '    If chkSameAsBilling.Checked Then
    '        addr.FirstName = dbBuilderAccount.FirstName
    '        addr.LastName = dbBuilderAccount.LastName
    '        addr.Company = dbBuilder.CompanyName
    '        addr.AddressLn1 = dbBuilder.Address
    '        addr.AddressLn2 = dbBuilder.Address2
    '        addr.City = dbBuilder.City
    '        addr.State = dbBuilder.State
    '        addr.Zip = Replace(dbBuilder.Zip, "-", "")
    '    Else
    '        addr.FirstName = txtCardholderFirstName.Text
    '        addr.LastName = txtCardholderLastName.Text
    '        addr.Company = txtCompany.Text
    '        addr.AddressLn1 = txtAddress1.Text
    '        addr.AddressLn2 = txtAddress2.Text
    '        addr.City = txtCity.Text
    '        addr.State = drpState.SelectedValue
    '        addr.Zip = Replace(txtZip.Text, "-", "")
    '    End If
    '    addr.Email = dbBuilderAccount.Email
    '    addr.Country = "US"

    '    Dim p As New VindiciaPaymentProcessor(DB)
    '    p.IsTestMode = SysParam.GetValue(DB, "TestMode")


    '    Try

    '        'Only update payment method with catch-up billing 
    '        If Not p.UpdateActivePaymentMethod(dbBuilder, addr, cc) Then
    '            AddError("An error was encountered while processing this transaction: " & p.ReturnString)
    '            Exit Sub
    '        Else
    '            Response.Redirect("/default.aspx")
    '        End If

    '        'Do not do anything with autobills.  Everything is handeled in the registration payment screen.

    '        'Dim autoBills() As Vindicia.AllDataTypes.AutoBill = p.GetAutobills(dbBuilder)
    '        'For Each ab As Vindicia.AllDataTypes.AutoBill In autoBills
    '        '    If ab.status = Vindicia.AllDataTypes.AutoBillStatus.Suspended Then
    '        '        ab.status = Vindicia.AllDataTypes.AutoBillStatus.Active
    '        '        ab.statusSpecified = True
    '        '    End If
    '        'Next
    '        'If p.UpdateAutoBills(autoBills) Then
    '        '    Response.Redirect("/default.aspx")
    '        'Else
    '        '    AddError("An error was encountered while processing this transaction: " & p.ReturnString)
    '        'End If

    '    Catch ex As Exception
    '        Logger.Error(Logger.GetErrorMessage(ex))
    '        AddError(ErrHandler.ErrorText(ex))
    '    End Try
    'End Sub

    'Private Sub EnableDisableBillingValidators()
    '    rfvtxtAddress1.Enabled = Not chkSameAsBilling.Checked
    '    rfvtxtCity.Enabled = Not chkSameAsBilling.Checked
    '    rfvdrpState.Enabled = Not chkSameAsBilling.Checked
    '    rfvtxtZip.Enabled = Not chkSameAsBilling.Checked
    'End Sub
End Class
