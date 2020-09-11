Imports Components
Imports DataLayer
Imports System.Data.SqlClient
Imports Utility
Imports System.Security.Cryptography.X509Certificates
Imports System.Linq
Imports Vindicia

Partial Class forms_builder_registration_payment
    Inherits SitePage

    'Private ReadOnly Property Now() As DateTime
    '    Get
    '        Dim m_Now As String = SysParam.GetValue(DB, "DemoRegistrationDate")
    '        If m_Now = Nothing OrElse Not IsDate(Now) OrElse DateDiff(DateInterval.Day, Now, Convert.ToDateTime(m_Now)) < 0 Then
    '            Return DateTime.Now()
    '        Else
    '            Return Convert.ToDateTime(m_Now)
    '        End If
    '    End Get
    'End Property

    Private dbBuilder As BuilderRow
    Private dbBuilderBillingPlan As BuilderBillingPlanRow
    Private dbBuilderAccount As BuilderAccountRow
    Private dbPayment As BuilderRegistrationPaymentRow

    Protected NewRegistration As Boolean
    Private startDate As Date
    Private monthCount As Integer
    Private taxAmount As Decimal = 0.0
    Private totalAmount As Decimal = 0.0
    Private rowCount As Integer
    'Protected IsRegistrationActive As Boolean = False 'Client requested not to validate for regitration fee anymore because past members have paid this with a check.
    Protected IsSubscriptionActive As Boolean = False
    Protected IsCurrentBuilder As Boolean
    Protected currentAutoBill As Vindicia.AutoBill
    Protected dtCatchupStartDate As DateTime = Now.Date
    Protected Processor As VindiciaPaymentProcessor
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Protected Property registrationProduct() As Vindicia.Product
        Get
            Return Session("RegistrationProduct")
        End Get
        Set(ByVal value As Vindicia.Product)
            Session("RegistrationProduct") = value
        End Set
    End Property

    Protected Property vindiciaProduct() As Vindicia.Product
        Get
            Return Session("VindiciaProduct")
        End Get
        Set(ByVal value As Vindicia.Product)
            Session("VindiciaProduct") = value
        End Set
    End Property
    Protected Property vindiciaPlan() As Vindicia.BillingPlan
        Get
            Return Session("VindiciaPlan")
        End Get
        Set(ByVal value As Vindicia.BillingPlan)
            Session("VindiciaPlan") = value
        End Set
    End Property
    Protected Property vindiciaOneTimePlan() As Vindicia.BillingPlan
        Get
            Return Session("VindiciaOneTimePlan")
        End Get
        Set(ByVal value As Vindicia.BillingPlan)
            Session("VindiciaOneTimePlan") = value
        End Set
    End Property
    Protected Property vindiciaPeriods() As Vindicia.BillingPlanPeriod()
        Get
            Return Session("VindiciaPeriods")
        End Get
        Set(ByVal value As Vindicia.BillingPlanPeriod())
            Session("VindiciaPeriods") = value
        End Set
    End Property

    Protected ReadOnly Property Guid() As String
        Get
            If Request("id") IsNot Nothing Then
                Return Request("id")
            End If
            Return String.Empty
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        dbBuilder = BuilderRow.GetRow(DB, Session("BuilderId"))
        If dbBuilder.BuilderID = Nothing Then
            dbBuilder = BuilderRow.GetBuilderByGuid(DB, Guid)
        End If

        dbBuilderBillingPlan = BuilderBillingPlanRow.GetRow(DB, dbBuilder.BuilderID)

        dbBuilderAccount = BuilderAccountRow.GetRow(DB, Session("BuilderAccountId"))
        If dbBuilderAccount.BuilderAccountID = Nothing Then
            dbBuilderAccount = BuilderAccountRow.GetPrimaryAccount(DB, dbBuilder.BuilderID)
        End If

        NewRegistration = BuilderRow.IsNewBuilder(DB, dbBuilder.BuilderID)

        If dbBuilder Is Nothing OrElse dbBuilder.BuilderID = 0 Then
            Response.Redirect("default.aspx")
        End If

        If SysParam.GetValue(DB, "UseEntitlements") Then
            If dbBuilder.SkipEntitlementCheck Then
                Response.Redirect("/builder/")
            End If
        End If
        Processor = New VindiciaPaymentProcessor(DB)
        Processor.IsTestMode = SysParam.GetValue(DB, "TestMode")
        BindProducts()

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")

        If Not IsPostBack Then

        Core.DataLog("User Credit Card Information", PageURL, CurrentUserId, "Builder Edit Profile Left Menu Click", "", "", "", "", UserName)

            vnhostedPayment.Builder = dbBuilder

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

            'chkSameAsBilling.Checked = False

            If Request("vinHoaErr") IsNot Nothing Then
                AddError("Error Occurred while processing your request. Please try again")
            End If

        End If

    End Sub

    'Private Sub LoadFromDB()
    '    txtAddress1.Text = dbBuilder.Address
    '    txtAddress2.Text = dbBuilder.Address2
    '    txtCity.Text = dbBuilder.City
    '    txtCompany.Text = dbBuilder.CompanyName
    '    txtFirstName.Text = dbBuilderAccount.FirstName
    '    txtLastName.Text = dbBuilderAccount.LastName
    '    txtZip.Text = dbBuilder.Zip
    'End Sub

    Private Sub BindProducts()

        'See if builder exists in vindicia
        IsCurrentBuilder = Processor.EnsureVindiciaAccount(dbBuilder)

        Dim autoBills() As Vindicia.AutoBill = Processor.GetAutobills(dbBuilder)

        If IsCurrentBuilder Then
            If autoBills.Length = 0 Then
                IsCurrentBuilder = False
            End If
        End If


        If IsCurrentBuilder Then
            'Check autobills to see if they have active ones for registration and subscription
            For Each ab As Vindicia.AutoBill In autoBills

                'Dim Product As String = ab.product
                Dim Product As String = ab.items(0).product.merchantProductId
                If Product = "Registration" Then
                    'If ab.status = Vindicia.AllDataTypes.AutoBillStatus.Active Then
                    '    IsRegistrationActive = True
                    'End If
                ElseIf Product = "Subscription" Then
                    If ab.status = Vindicia.AutoBillStatus.Active Then
                        IsSubscriptionActive = True
                    ElseIf ab.status = Vindicia.AutoBillStatus.LegacySuspended Then
                        'Suspended means that autobill is in Hard Error (failed to charge credit card in the account)
                        'We use this autobill to determine back-bill start date (to account for passed missed payments)
                        currentAutoBill = ab
                    End If
                End If
            Next

            'This is used to sort subscripotion transaction history
            Dim dtTH As DataTable = New DataTable
            dtTH.Columns.Add("TDate")
            dtTH.Columns("TDate").DataType = System.Type.GetType("System.DateTime")
            dtTH.Columns.Add("TStatus")

            'If we find that builder has to pay for something we check transaction history to determine:
            '1) if builder has ever successfully paid for registration fee
            '2) if builder has any past missed charges
            'If Not IsRegistrationActive Or Not IsSubscriptionActive Then
            If Not IsSubscriptionActive And Not currentAutoBill Is Nothing Then
                Dim transactionHistory() As Vindicia.Transaction = Processor.GetTransactionHistory(dbBuilder)
                For Each th As Vindicia.Transaction In transactionHistory
                    Dim Product As String = th.transactionItems(0).sku
                    'If th.status.status <> Vindicia.AllDataTypes.TransactionStatusType.Cancelled Then
                    '    If Product = "Registration" Then
                    '        '1) yes
                    '        IsRegistrationActive = True
                    '    End If
                    'End If
                    If Product = "Subscription" Then
                        Dim dr As DataRow = dtTH.NewRow()
                        dr("TDate") = th.timestamp.ToString
                        dr("TStatus") = th.statusLog(0).status
                        dtTH.Rows.Add(dr)
                        dtTH.AcceptChanges()
                    End If
                Next

                Dim dtFirstFailure As DateTime = Now()

                'Go back in time an determine the date of the first failed transaction date.  This will be the back-bill start date for the subscription autobill.
                If dtTH.Rows.Count > 0 Then
                    Dim dv As DataView = dtTH.DefaultView
                    dv.Sort = "TDate Desc"
                    For j As Integer = 0 To dv.Count - 1
                        Dim row As DataRowView = dv(j)
                        Dim dTH As DateTime = row(0)
                        Dim sTH As String = row(1)
                        If sTH = "Captured" Then
                            'dtCatchupStartDate = dtFirstFailure
                            Exit For
                        ElseIf sTH = "Cancelled" Then
                            dtFirstFailure = dTH
                            'If dTH > currentAutoBill.startTimestamp Then
                            '    dtCatchupStartDate = dTH
                            'End If
                        Else
                            dtCatchupStartDate = Now()
                        End If
                    Next
                End If

                dtCatchupStartDate = dtFirstFailure

                'Display past missed charges to the user.
                If Not currentAutoBill Is Nothing And dtCatchupStartDate.Date < Now.Date Then
                    Dim dtBackBill As DateTime = dtCatchupStartDate
                    Dim sBackBill As String = String.Empty

                    Do While dtBackBill.Date < Now.Date
                        Dim dtTemp = dtBackBill
                        Dim sInterval As String = String.Empty
                        Dim sPeriod As Vindicia.BillingPlanPeriod = Nothing

                        For Each amount In currentAutoBill.billingPlan.periods
                            If amount.prices IsNot Nothing Then
                                sPeriod = amount
                                Exit For
                            End If

                        Next
                        Select Case sPeriod.type
                            Case Vindicia.BillingPeriodType.Day
                                dtBackBill = dtBackBill.AddDays(sPeriod.quantity)
                                sInterval = "day"
                            Case Vindicia.BillingPeriodType.Month
                                dtBackBill = dtBackBill.AddMonths(sPeriod.quantity)
                                sInterval = "month"
                            Case Vindicia.BillingPeriodType.Week
                                dtBackBill = dtBackBill.AddDays(sPeriod.quantity * 7)
                                sInterval = "week"
                            Case Vindicia.BillingPeriodType.Year
                                dtBackBill = dtBackBill.AddYears(sPeriod.quantity)
                                sInterval = "year"
                        End Select
                        'If dtBackBill.Date >= Now.Date Then Exit Do
                        sBackBill &= "<tr>"
                        sBackBill &= "<td>PAST DUE Membership Fee for <b>" & dtTemp.Date & "</b></td>"
                        sBackBill &= "<td>" & Now.Date & IIf(dtBackBill.Date >= Now.Date, " and every " & sPeriod.quantity & " " & sInterval & "(s) from " & dtTemp.Date, "") & "</td>"
                        sBackBill &= "<td>" & FormatCurrency(sPeriod.prices(0).amount) & "</td>"
                        sBackBill &= "</tr>"
                        sBackBill &= "<tr><td colspan=""3"" style=""height:1px;background-color:#aaa;padding:0px;margin:0px;""><img src=""/images/spacer.gif"" style=""height:1px;"" alt="""" /></td></tr>"
                        taxAmount += Processor.CalculateTax(dbBuilder, 1, sPeriod.prices(0).amount)
                        totalAmount += sPeriod.prices(0).amount + taxAmount
                    Loop

                    ltlBackBill.Text = sBackBill
                End If
            ElseIf IsSubscriptionActive Then
                'If builder accesses this page by mistake and already has registration and active subscription autobill.
                Response.Redirect("/forms/builder-registration/default.aspx?RegStep=AccountProfile")
            End If

        End If

        Dim prods As Vindicia.Product() = Processor.GetProducts()

        Dim subName As String = SysParam.GetValue(DB, "SubscriptionProductId")
        Dim regName As String = SysParam.GetValue(DB, "RegistrationProductId")

        'vindiciaProduct = (From prod As Vindicia.AllDataTypes.Product In prods Where prod.merchantProductId = subName Select prod).FirstOrDefault
        For Each prod As Vindicia.Product In prods
            If prod.merchantProductId = subName Then
                vindiciaProduct = prod
            ElseIf prod.merchantProductId = regName Then
                registrationProduct = prod
            End If
        Next
        Dim plans As Vindicia.BillingPlan() = Processor.GetBillingPlans()

        '----------------------------------------------------------------------------------
        '------ FOLLOWING LINES ADDED BY APALA (MEDULLUS) FOR LLC-BASED BILLING PLAN CHANGE
        Dim BuilderBillingPlanID As Integer = 0
        Dim BuilderSubBillingPlanMerchantKey As String = ""
        Dim BuilderRegBillingPlanMerchantKey As String = ""
        Dim dbBuilderBillingPlan As BuilderBillingPlanRow
        dbBuilderBillingPlan = BuilderBillingPlanRow.GetRow(DB, dbBuilder.BuilderID)
        If dbBuilderBillingPlan.BuilderBillingPlanId > 0 Then
            Dim dbBillingPlan As BillingPlanRow

            dbBillingPlan = BillingPlanRow.GetRow(DB, dbBuilderBillingPlan.SubBillingPlanId)
            BuilderSubBillingPlanMerchantKey = dbBillingPlan.ExternalKey

            dbBillingPlan = BillingPlanRow.GetRow(DB, dbBuilderBillingPlan.RegBillingPlanId)
            BuilderRegBillingPlanMerchantKey = dbBillingPlan.ExternalKey
        End If
        '----------------------------------------------------------------------------------

        Dim startDate As DateTime = Nothing
        Dim endDate As DateTime = Nothing
        Dim RegType As String = String.Empty
        Dim FirstBillDate As String = String.Empty
        Dim IsRecurring As String = String.Empty
        Dim ApplicablePlans As New ArrayList
        Dim plan As String
        Dim i As Integer
        For i = 0 To plans.Length - 1
            If plans(i).nameValues Is Nothing Then Continue For

            If plans(i).merchantBillingPlanId <> BuilderRegBillingPlanMerchantKey AndAlso plans(i).merchantBillingPlanId <> BuilderSubBillingPlanMerchantKey Then Continue For

            plan = plans(i).merchantBillingPlanId
            startDate = Nothing
            endDate = Nothing
            RegType = Nothing
            FirstBillDate = Nothing
            IsRecurring = Nothing
            For Each Pair As Vindicia.NameValuePair In plans(i).nameValues
                Select Case Pair.name
                    Case "StartDate"
                        startDate = Pair.value
                    Case "EndDate"
                        endDate = Pair.value
                    Case "RegistrationType"
                        RegType = Pair.value
                    Case "1stBillDate"
                        FirstBillDate = Pair.value
                    Case "Recurring"
                        IsRecurring = Pair.value
                End Select
            Next

            If Now >= startDate And (endDate = Nothing OrElse Now <= endDate) Then
                If BuilderSubBillingPlanMerchantKey <> "" AndAlso plan = BuilderSubBillingPlanMerchantKey Then
                    If IsRecurring.ToLower() = "Yes".ToLower() Then
                        vindiciaPlan = plans(i)
                    End If
                ElseIf BuilderRegBillingPlanMerchantKey <> "" AndAlso plan = BuilderRegBillingPlanMerchantKey Then
                    vindiciaOneTimePlan = plans(i)
                End If

                'This finds out registration billing plan for current builders that failed to pay it.
                'If Not IsRegistrationActive And RegType = "New" And IsRecurring = "No" Then
                '    vindiciaOneTimePlan = plans(i)
                'End If

            End If

        Next

        'AR 03/04/11: Client wants to have builder pay for current suspended auto bill
        'If vindiciaPlan Is Nothing And Not IsSubscriptionActive And Not currentAutoBill Is Nothing Then
        '    vindiciaPlan = currentAutoBill.billingPlan
        'End If
        'BP-11/7/2014  When a builder experiences a ‘Hard Error,’ or the current auto-bill has reached the maximum number of unsuccessful billing attempts,
        ' the builder is assigned a new billing plan S099F030D030 after updating payment method in the software. 
        'For example builder 4515 experienced a hard error on 10/6/14. The builder then updated payment method on 10/24 and the 
        'builder was assigned a new autobill with billing plan S099F030D030. The problem is that this billing plan gives them 30 free days, 
        'so they were not billed on 10/24 for the missed 10/6 billing cycle. The next scheduled billing is not until 11/6, 12/6, 1/6/15 etc.
        ' We need to find a way that when a builder is assigned a new autobill after a failed billing, that the do not receive a free 30 days
        'FROM CLIENT : There is a billing plan in the system S099F000D030. 
        'This is the one builders should be assigned after a hard failure. The one they are currently assigned at registration is S099F030D030.

        '*********** APALA (MEDULLUS) : Following lines commented out to overrule hardcoded Hard Error Recovery plan
        'If Not IsSubscriptionActive And Not currentAutoBill Is Nothing Then
        '    vindiciaPlan = LoadHardErrorPlan(plans)
        'End If
        '**************************************************************************************************************************************

        If vindiciaPlan Is Nothing And Not IsSubscriptionActive Then
            If IsCurrentBuilder Then
                'Builder exists in vindicia but no billing plan
                Logger.Error("Could not locate applicable billing plan (CURRENT builder)")
                AddError("There are no applicable billing plans. Please <a href=""/builder/"">click here</a> to go to your dashboard.  For questions please contact your Operations Manager. Otherwise email us at customerservice@cbusa.us.")
                pnlForm.Visible = False
            Else
                Logger.Error("Could not locate applicable billing plan (NEW builder)")
                AddError("An error was encountered while processing this request. For immediate assistance please contact your Operations Manager. Otherwise email us at customerservice@cbusa.us.")
                pnlForm.Visible = False
            End If

            Exit Sub
        End If

        'If vindiciaOneTimePlan IsNot Nothing And Not IsRegistrationActive Then
        If vindiciaOneTimePlan IsNot Nothing And NewRegistration Then
            FirstBillDate = (From pair In vindiciaOneTimePlan.nameValues Where pair.name = "1stBillDate" Select pair.value).FirstOrDefault

            LoadOneTimePlan(vindiciaOneTimePlan, IIf(IsDate(dbBuilderBillingPlan.BillingStartDate), Now.Date, Now))
            rowCount += 1
        Else
            phRegistrationFee.Visible = False
        End If
        'If startDate = Nothing Or RegType = String.Empty Then
        '    Logger.Error("Could not locate applicable billing plan")
        '    AddError()
        '    Exit Sub
        'Else
        '    vindiciaPlan = plans(i)
        '    If NewRegistration Then
        '        LoadNewProduct(prods)
        '        rowCount += 1
        '    Else
        '        phRegistrationFee.Visible = False
        '    End If
        'End If

        'ltlName.Text = vindiciaPlan.description

        'If Not IsRegistrationActive Then
        ltlRegistrationFee.Text = "Registration Fee"
        'Else
        'ltlRegistrationFee.Text = "Recurring Monthly Membership Fee"
        'End If

        If Not IsSubscriptionActive And dtCatchupStartDate.Date = Now.Date Then
            vindiciaPeriods = vindiciaPlan.periods
            rptBillingSchedule.DataSource = vindiciaPeriods
            rptBillingSchedule.DataBind()
        End If

        ltlTotalTax.Text = FormatCurrency(taxAmount, 2)
        ltlTotal.Text = FormatCurrency(totalAmount, 2)
        monthCount = 0

    End Sub

    'Private Sub LoadNewProduct(ByVal products() As Vindicia.AllDataTypes.Product)
    '    Dim regName As String = SysParam.GetValue(DB, "RegistrationProductId")

    '    Dim product As Vindicia.AllDataTypes.Product = (From p As Vindicia.AllDataTypes.Product In products Where p.merchantProductId = regName Select p).FirstOrDefault
    '    If product Is Nothing Then
    '        Logger.Error("Could not locate Registration product")
    '        AddError()
    '        Exit Sub
    '    End If

    '    registrationProduct = product
    '    Dim plan As Vindicia.AllDataTypes.BillingPlan = product.defaultBillingPlan
    '    ltlWhen.Text = FormatDateTime(Now, DateFormat.ShortDate)
    '    ltlPrice.Text = FormatCurrency(plan.periods(0).prices(0).amount)
    '    'ltlRegDescription.Text = plan.description
    '    'ltlRegAmount.Text = FormatCurrency(plan.periods(0).prices(0).amount)
    'End Sub

    Private Sub LoadOneTimePlan(ByVal plan As Vindicia.BillingPlan, ByVal FirstBillDate As DateTime)
        ltlWhen.Text = FormatDateTime(FirstBillDate, DateFormat.ShortDate)

        If Not plan.periods(0).prices Is Nothing Then
            ltlPrice.Text = FormatCurrency(plan.periods(0).prices(0).amount)
            totalAmount += plan.periods(0).prices(0).amount
        Else
            ltlPrice.Text = "Free"
        End If
    End Sub

    'Protected Sub btnProcess_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnProcess.Click
    '    If chkSameAsBilling.Checked Then
    '        DisableBillingValidators()
    '    End If
    '    Page.Validate()
    '    If Not Page.IsValid Then Exit Sub




    '    Try
    '        DB.BeginTransaction()
    '        dbPayment = New BuilderRegistrationPaymentRow(DB)
    '        Dim dbRegistration As BuilderRegistrationRow = BuilderRegistrationRow.GetRowByBuilder(DB, dbBuilder.BuilderID)

    '        dbPayment.BuilderRegistrationID = dbRegistration.BuilderRegistrationID
    '        dbPayment.PaymentStatusID = 1
    '        dbPayment.Submitted = Now()

    '        'Never store CC data id payment gateway is used per Ryan M.
    '        'If SysParam.GetValue(DB, "SaveCreditCardInfo") Then
    '        dbPayment.CardNumber = "Not Saved" 'Crypt.EncryptTripleDes(txtCardNumber.Text)
    '        dbPayment.CardholderName = "Not Saved" 'txtCardholderFirstName.Text & " " & txtCardholderLastName.Text
    '        '    dbPayment.CardTypeID = drpCardType.SelectedValue
    '        '    dbPayment.ExpirationDate = ctrlExpDate.Value
    '        '    dbPayment.CIDNumber = Crypt.EncryptTripleDes(txtCID.Text)
    '        'End If

    '        dbPayment.Insert()

    '        DB.CommitTransaction()


    '        Dim cc As New CreditCardInfo()
    '        cc.CardHolderName = txtCardholderFirstName.Text & " " & txtCardholderLastName.Text
    '        cc.CID = txtCID.Text
    '        cc.CreditCardNumber = txtCardNumber.Text.Replace("-", "")
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
    '            addr.Zip = Replace(dbBuilder.Zip, "-", "")
    '        Else
    '            addr.FirstName = txtCardholderFirstName.Text
    '            addr.LastName = txtCardholderLastName.Text
    '            addr.Company = txtCompany.Text
    '            addr.AddressLn1 = txtAddress1.Text
    '            addr.AddressLn2 = txtAddress2.Text
    '            addr.City = txtCity.Text
    '            addr.State = drpState.SelectedValue
    '            addr.Zip = Replace(txtZip.Text, "-", "")
    '        End If
    '        addr.Email = dbBuilderAccount.Email
    '        addr.Country = "US"

    '        Dim p As New VindiciaPaymentProcessor(DB)
    '        p.IsTestMode = SysParam.GetValue(DB, "TestMode")

    '        'If Not p.SubmitCVVTransaction(dbBuilder, addr, cc) Then
    '        '    AddError("This transaction was declined by the processor.  Please check your payment details and try again.")
    '        '    Exit Sub
    '        'End If

    '        'Per client request, if registration fee isn't processed first then don't let builder continue.
    '        Dim IsPaymentSuccess As Boolean = True
    '        Dim IsNew As Boolean = True

    '        If vindiciaOneTimePlan IsNot Nothing And NewRegistration Then
    '            Dim start As String = (From pair In vindiciaOneTimePlan.nameValues Where pair.name = "1stBillDate" Select pair.value).FirstOrDefault
    '            If start = Nothing OrElse start = "0" Then
    '                start = Now
    '            End If
    '            If p.StartBilling(registrationProduct, vindiciaOneTimePlan, start, dbBuilder, addr, cc) Then
    '                ltlResult.Text = "<h1>Registration Successful</h1>"
    '                IsNew = False
    '            Else
    '                IsPaymentSuccess = False
    '                AddError("An error was encountered processing your registration. Details: " & p.ReturnString)
    '                Logger.Error("Registration AutoBill failure. ReturnCode=" & p.ReturnCode & ", ReturnString=" & p.ReturnString & ", SoapID=" & p.SoapId)
    '            End If
    '        End IfLoadHardErrorPlan

    '        If IsPaymentSuccess And Not IsSubscriptionActive Then
    '            Dim subStart As String = Now
    '            If Not vindiciaPlan.nameValues Is Nothing Then
    '                Try
    '                    subStart = (From pair In vindiciaPlan.nameValues Where pair.name = "1stBillDate" Select pair.value).FirstOrDefault
    '                    If subStart = Nothing OrElse subStart = "0" Then
    '                        subStart = Now
    '                    End If
    '                Catch ex As Exception
    '                End Try
    '            End If
    '            'Determine if we're back-billing when creating the new subscription autobill.
    '            If dtCatchupStartDate < subStart Then
    '                subStart = dtCatchupStartDate
    '            End If

    '            If p.StartBilling(vindiciaProduct, vindiciaPlan, subStart, dbBuilder, addr, cc) Then
    '                'ltlResult.Text &= "<h1>Success</h1><br/>Code: " & p.ReturnCode & "<br/>Status: " & p.ReturnString & "<br/>SoapID: " & p.SoapId
    '            Else
    '                IsPaymentSuccess = False
    '                AddError("An error was encountered processing your registration. Details: " & p.ReturnString)
    '                Logger.Error("Subscription AutoBill failure. ReturnCode=" & p.ReturnCode & ", ReturnString=" & p.ReturnString & ", SoapID=" & p.SoapId)
    '            End If
    '        End If

    '        If IsPaymentSuccess Then
    '            'AR 03/23/11: Client wants to stop all suspended auto bills
    '            'BP 4/13 Stop all suspended autobills
    '            Try
    '                Dim autoBills() As Vindicia.AutoBill = p.GetAutobills(dbBuilder)
    '                For Each ab As Vindicia.AutoBill In autoBills
    '                    If ab.status = Vindicia.AutoBillStatus.Suspended Then
    '                        ab.status = Vindicia.AutoBillStatus.Cancelled
    '                        ab.statusSpecified = True
    '                        p.CancelAutoBill(ab.VID)
    '                    End If
    '                Next
    '            Catch ex As Exception
    '                Logger.Error("Error  CancelAutoBill  . ")

    '            End Try



    '            Dim dbStatus As RegistrationStatusRow = RegistrationStatusRow.GetRowByStatus(DB, "Pending")
    '            dbRegistration.RegistrationStatusID = dbStatus.RegistrationStatusID
    '            dbRegistration.CompleteDate = Now
    '            dbRegistration.Update()

    '            If Not IsNew Then
    '                dbBuilder.IsNew = False
    '                dbBuilder.Update()
    '            End If

    '            'Only send email if builder registers for the first time.
    '            If Not IsCurrentBuilder And Not SysParam.GetValue(DB, "TestMode") Then
    '                Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "NewBuilderForBuilders")
    '                Dim dtBuilders As DataTable = BuilderRow.GetListByLLC(DB, dbBuilder.LLCID)
    '                Try
    '                    dbMsg.SendLLCNotificationListAReminder(LLCRow.GetLLCNotificationList(DB, dbBuilder.LLCID), "New Builder: " & vbTab & dbBuilder.CompanyName & vbCrLf & "LLC : " & Core.GetString(LLCRow.GetRow(DB, dbBuilder.LLCID).LLC) & vbCrLf & vbCrLf & GlobalRefererName)
    '                Catch ex As Exception

    '                End Try
    '                For Each row As DataRow In dtBuilders.Rows
    '                    If Core.GetInt(row("BuilderId")) <> dbBuilder.BuilderID Then
    '                        Dim dbMsgBuilder As BuilderRow = BuilderRow.GetRow(DB, row("BuilderId"))
    '                        Dim LLCname As String = Core.GetString(LLCRow.GetRow(DB, dbBuilder.LLCID).LLC)
    '                        If LLCname <> String.Empty Then
    '                            LLCname = "LLC : " & LLCname
    '                        End If
    '                        dbMsg.Send(dbMsgBuilder, "New Builder: " & vbTab & dbBuilder.CompanyName & vbCrLf & LLCname & vbCrLf & vbCrLf & GlobalRefererName, CCLLCNotification:=False)
    '                    End If
    '                Next



    '                dbMsg = AutomaticMessagesRow.GetRowByTitle(DB, "NewBuilderForVendors")
    '                Dim dtVendors As DataTable = VendorRow.GetListByLLC(DB, dbBuilder.LLCID)
    '                Dim BuilderLLCname As String = Core.GetString(LLCRow.GetRow(DB, dbBuilder.LLCID).LLC)
    '                Dim NewBuilderForVendors As String = String.Empty
    '                If BuilderLLCname <> String.Empty Then
    '                    BuilderLLCname = "LLC : " & BuilderLLCname
    '                End If

    '                For Each row As DataRow In dtVendors.Rows
    '                    Dim dbVendor As VendorRow = VendorRow.GetRow(DB, row("VendorId"))
    '                    NewBuilderForVendors = "Dear " & Core.GetString(row("CompanyName")) & ", " & vbCrLf & "The " & Core.GetString(LLCRow.GetRow(DB, dbBuilder.LLCID).BuilderGroup) & " has added a new builder partner to the CBUSA Network."
    '                    Dim additionalText As String = "To view the details of " & dbBuilder.CompanyName & " , please login to the Member Directory of the CBUSA Software." & vbCrLf & "https://app.custombuilders-usa.com/directory/"
    '                    '301552
    '                    ' dbMsg.Send(dbVendor, NewBuilderForVendors & vbCrLf & vbCrLf & "New Builder: " & vbTab & dbBuilder.CompanyName & vbCrLf & vbCrLf & additionalText, CCLLCNotification:=False)
    '                Next

    '                Try
    '                    Dim additionalAdminText As String = "To view the details of " & dbBuilder.CompanyName & " , please login to the Member Directory of the CBUSA Software." & vbCrLf & "https://app.custombuilders-usa.com/directory/"
    '                    dbMsg.SendLLCNotificationListAReminder(LLCRow.GetLLCNotificationList(DB, dbBuilder.LLCID), NewBuilderForVendors & vbCrLf & vbCrLf & "New Builder: " & vbTab & dbBuilder.CompanyName & vbCrLf & vbCrLf & additionalAdminText)
    '                Catch ex As Exception

    '                End Try

    '            End If

    '            Response.Redirect("/builder/default.aspx")
    '        End If

    '    Catch ex As ApplicationException
    '        If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
    '        AddError(ex.Message)
    '    Catch ex As SqlException
    '        If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
    '        AddError(ErrHandler.ErrorText(ex))
    '    End Try
    'End Sub

    'Private Sub DisableBillingValidators()
    '    rfvtxtAddress1.Enabled = False
    '    rfvtxtCity.Enabled = False
    '    rfvdrpState.Enabled = False
    '    rfvtxtZip.Enabled = False
    'End Sub

    Private Overloads Sub AddError()
        AddError("There was an error loading product data. Please retry your request.")
    End Sub
    'This plan does not give one month free see comments above - 
    Private Function LoadHardErrorPlan(ByVal BillingPlan() As Vindicia.BillingPlan) As Vindicia.BillingPlan
        Dim HardErrorBillingPlan As Vindicia.BillingPlan = (From p As Vindicia.BillingPlan In BillingPlan Where p.merchantBillingPlanId = "S099F000D030" Select p).FirstOrDefault
        Return HardErrorBillingPlan
    End Function
    Protected Sub rptBillingSchedule_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptBillingSchedule.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        Dim period As Vindicia.BillingPlanPeriod = e.Item.DataItem

        Dim cycleStart As Date = Nothing
        If startDate = Nothing Then
            If Not vindiciaPlan.nameValues Is Nothing Then
                For Each pair In vindiciaPlan.nameValues
                    If pair.name = "1stBillDate" And IsDate(pair.value) Then
                        startDate = pair.value
                        Exit For
                    End If
                Next
            End If
            If startDate = Nothing Then
                startDate = Now
            End If
            'startDate = (From p In vindiciaPlan.nameValues Where p.name = "1stBillDate" Select p.value).FirstOrDefault
            'If startDate = Nothing Then
            '    startDate = FormatDateTime(Now, DateFormat.ShortDate)
            'End If
            cycleStart = startDate
        Else
            cycleStart = DateAdd(DateInterval.Month, monthCount, startDate)
        End If

        If dbBuilderBillingPlan.SubscriptionStartDate > Now.Date Then
            cycleStart = dbBuilderBillingPlan.SubscriptionStartDate.Date
        Else
            cycleStart = Now.Date
        End If

        Try
            If period.prices(0).amount > 0 Then

                'If period.prices(0).amount = 0 Then
                '    rptBillingSchedule.Visible = False
                '    Exit Sub
                'End If

                Dim tr As HtmlTableRow = e.Item.FindControl("trRow")
                'If rowCount Mod 2 = 1 Then
                '    tr.Attributes("class") = "alternate"
                'Else
                '    tr.Attributes("class") = ""
                'End If
                rowCount += 1

                Dim ltlWhen As Literal = e.Item.FindControl("ltlWhen")
                Dim ltlPrice As Literal = e.Item.FindControl("ltlPrice")
                taxAmount += Processor.CalculateTax(dbBuilder, 1, period.prices(0).amount)
                totalAmount += period.prices(0).amount + taxAmount
                'If e.Item.ItemIndex = rptBillingSchedule.DataSource.Length - 1 Then
                '    ltlWhen.Text = FormatDateTime(DateAdd(DateInterval.Month, monthCount, Now), DateFormat.ShortDate) & " and every " & period.quantity & " " & IIf(period.quantity = 1, period.type.ToString, period.type.ToString & "s") & " thereafter"
                'Else
                '    ltlWhen.Text = FormatDateTime(DateAdd(DateInterval.Month, monthCount, Now), DateFormat.ShortDate)
                'End If


                If e.Item.ItemIndex = rptBillingSchedule.DataSource.Length - 1 Then
                    ltlWhen.Text = FormatDateTime(cycleStart, DateFormat.ShortDate) & " and every " & period.quantity & " " & IIf(period.quantity = 1, period.type.ToString, period.type.ToString & "s") & " thereafter"
                Else
                    ltlWhen.Text = FormatDateTime(cycleStart, DateFormat.ShortDate)
                End If

                ltlPrice.Text = FormatCurrency(period.prices(0).amount)
            Else
                e.Item.Visible = False
            End If
        Catch ex As Exception
            e.Item.Visible = False
        End Try

        monthCount += period.cycles * period.quantity
    End Sub


    'Protected Sub btnGoToDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnGoToDashBoard.Click
        'Response.Redirect("/vendor/default.aspx")
   ' End Sub
End Class
