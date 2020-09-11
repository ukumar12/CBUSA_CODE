Imports Components
Imports DataLayer
Imports System.Linq
Imports System.Data.SqlClient

Partial Class vindicia_test
    Inherits SitePage

    Private dbBuilder As BuilderRow
    Private dbBuilderAccount As BuilderAccountRow

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        dbBuilder = BuilderRow.GetRow(DB, Session("BuilderId"))
        dbBuilderAccount = BuilderAccountRow.GetRow(DB, Session("BuilderAccountId"))

        If Not IsPostBack Then
            drpCardType.DataSource = CreditCardTypeRow.GetAllCardTypes(DB)
            drpCardType.DataValueField = "CardTypeId"
            drpCardType.DataTextField = "Name"
            drpCardType.DataBind()
            drpCardType.Items.Insert(0, New ListItem("-- please select --", ""))

            drpState.DataSource = StateRow.GetStateList(DB)
            drpState.DataTextField = "StateCode"
            drpState.DataValueField = "StateCode"
            drpState.DataBind()

            'LoadFromDB()

            chkSameAsBilling.Checked = False
        End If
    End Sub

    'Protected Sub btnProcess_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnProcess.Click
    '    Page.Validate()
    '    If Not Page.IsValid Then Exit Sub

    '    Try
    '        Dim cc As New CreditCardInfo()
    '        cc.CardHolderName = txtCardholderFirstName.Text & " " & txtCardholderLastName.Text
    '        cc.CID = txtCID.Text
    '        cc.CreditCardNumber = txtCardNumber.Text
    '        cc.CreditCardType = drpCardType.SelectedValue
    '        cc.ExpMonth = Month(ctrlExpDate.Value)
    '        cc.ExpYear = Year(ctrlExpDate.Value)

    '        Dim addr As New VindiciaAddressInfo
    '        If chkSameAsBilling.Checked Then
    '            addr.FirstName = dbBuilderAccount.FirstName
    '            addr.LastName = dbBuilderAccount.LastName
    '            addr.Company = dbBuilder.CompanyName
    '            addr.AddressLn1 = dbBuilder.Address
    '            addr.AddressLn2 = dbBuilder.Address2
    '            addr.City = dbBuilder.City
    '            addr.State = dbBuilder.State
    '            addr.Zip = dbBuilder.Zip
    '        Else
    '            addr.FirstName = txtCardholderFirstName.Text
    '            addr.LastName = txtCardholderLastName.Text
    '            addr.Company = txtCompany.Text
    '            addr.AddressLn1 = txtAddress1.Text
    '            addr.AddressLn2 = txtAddress2.Text
    '            addr.City = txtCity.Text
    '            addr.State = drpState.SelectedValue
    '            addr.Zip = txtZip.Text
    '        End If
    '        addr.Email = dbBuilderAccount.Email
    '        addr.Country = "US"

    '        Dim p As New VindiciaPaymentProcessor(DB)
    '        p.IsTestMode = SysParam.GetValue(DB, "TestMode")

    '        'If Not p.SubmitCVVTransaction(dbBuilder, addr, cc) Then
    '        '    AddError("This transaction was declined by the processor.  Please check your payment details and try again.")
    '        '    Exit Sub
    '        'End If

    '        Dim aProducts As Vindicia.AllDataTypes.Product() = p.GetProducts()
    '        Dim aPlans As Vindicia.AllDataTypes.BillingPlan() = p.GetBillingPlans()

    '        Dim vindiciaOneTimePlan As Vindicia.AllDataTypes.BillingPlan = (From prod As Vindicia.AllDataTypes.Product In aProducts Where prod.merchantProductId = "Test" Select prod)
    '        Dim vindiciaProduct As Vindicia.AllDataTypes.Product = (From plan As Vindicia.AllDataTypes.BillingPlan In aPlans Where plan.merchantBillingPlanId = "Test" Select plan)

    '        If p.StartBilling(vindiciaProduct, vindiciaOneTimePlan, Now.Date, dbBuilder, addr, cc) Then
    '            ltlResult.Text = "<h1>Registration Successful</h1>"
    '        Else
    '            AddError("An error was encountered processing your registration. Details: " & p.ReturnString)
    '            Logger.Error("Registration AutoBill failure. ReturnCode=" & p.ReturnCode & ", ReturnString=" & p.ReturnString & ", SoapID=" & p.SoapId)
    '        End If
    '    Catch ex As ApplicationException
    '        If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
    '        AddError(ex.Message)
    '    Catch ex As SqlException
    '        If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
    '        AddError(ErrHandler.ErrorText(ex))
    '    End Try
    'End Sub
End Class
