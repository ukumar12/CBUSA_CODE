
Imports System.Data.SqlClient
Imports Components
Imports DataLayer
Imports Vindicia

Partial Class VinHoaSucess
    Inherits SitePage

    Private dbBuilder As BuilderRow

    Private dbPayment As BuilderRegistrationPaymentRow

    Protected NewRegistration As Boolean
    Private startDate As Date
    Private monthCount As Integer
    Private taxAmount As Decimal = 0.0
    Private totalAmount As Decimal = 0.0
    Private rowCount As Integer

    Protected IsSubscriptionActive As Boolean = False
    Protected IsCurrentBuilder As Boolean
    Protected currentAutoBill As Vindicia.AutoBill
    Protected dtCatchupStartDate As DateTime = Now.Date
    Protected Processor As VindiciaPaymentProcessor


    Private _RequestFromUpdate As Boolean = False


    Private initializedWebsession As Vindicia.WebSession



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

        'Making Sure We get Session id parameter from vindicia
        If (Request("Session_Id") Is Nothing) Then Response.Redirect("default.aspx")

        'Load Builder
        dbBuilder = BuilderRow.GetRow(DB, Session("BuilderId"))
        If dbBuilder.BuilderID = Nothing Then
            dbBuilder = BuilderRow.GetBuilderByGuid(DB, Guid)
        End If

        'No builder found
        If dbBuilder Is Nothing OrElse dbBuilder.BuilderID = 0 Then
            Response.Redirect("default.aspx")
        End If

        'ONLY DO PROCESSING FOR GET REQUEST
        If Not IsPostBack Then
            NewRegistration = BuilderRow.IsNewBuilder(DB, dbBuilder.BuilderID)
            Processor = New VindiciaPaymentProcessor(DB)
            Processor.IsTestMode = SysParam.GetValue(DB, "TestMode")
            CommitWebSession()
        End If


    End Sub


    Private Sub BindProducts()

        'See if builder exists in vindicia
        IsCurrentBuilder = Processor.EnsureVindiciaAccount(dbBuilder)
        If IsCurrentBuilder Then
            Dim CheckautoBills() As Vindicia.AutoBill = Processor.GetAutobills(dbBuilder)
            If CheckautoBills.Length = 0 Then
                IsCurrentBuilder = False
            End If
        End If

        If IsCurrentBuilder Then
            'Check autobills to see if they have active ones for registration and subscription
            Dim autoBills() As Vindicia.AutoBill = Processor.GetAutobills(dbBuilder)
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
                            dtCatchupStartDate = Now.Date
                        End If
                    Next
                End If

                dtCatchupStartDate = dtFirstFailure.Date

                'Display past missed charges to the user.
                If Not currentAutoBill Is Nothing And dtCatchupStartDate.Date < Now.Date Then
                    Dim dtBackBill As DateTime = dtCatchupStartDate.Date
                    Do While dtBackBill.Date < Now.Date
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

                            Case Vindicia.BillingPeriodType.Month
                                dtBackBill = dtBackBill.AddMonths(sPeriod.quantity)

                            Case Vindicia.BillingPeriodType.Week
                                dtBackBill = dtBackBill.AddDays(sPeriod.quantity * 7)

                            Case Vindicia.BillingPeriodType.Year
                                dtBackBill = dtBackBill.AddYears(sPeriod.quantity)

                        End Select
                        taxAmount += Processor.CalculateTax(dbBuilder, 1, sPeriod.prices(0).amount)
                        totalAmount += sPeriod.prices(0).amount + taxAmount
                    Loop
                End If
            ElseIf IsSubscriptionActive Then
                'If builder accesses this page by mistake and already has registration and active subscription autobill.
                'THIS SHOULD NOT HAPPEN SINCE WE WERE REDIRECTED FROM PAYMENTPAGE. STILL ADDING QUERY PARAMETER TO IDENTIFY WE ARE REDIRECTING FROM HOA SUCCESS PAGE
                Response.Redirect("/forms/builder-registration/default.aspx?RegStep=AccountProfile&vinHoa=ln221", True)
            End If
        End If

        Dim prods As Vindicia.Product() = Processor.GetProducts()
        Dim subName As String = SysParam.GetValue(DB, "SubscriptionProductId")
        Dim regName As String = SysParam.GetValue(DB, "RegistrationProductId")
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
                'This finds out registration billing plan for current builders that failed to pay it.
                'If Not IsRegistrationActive And RegType = "New" And IsRecurring = "No" Then
                '    vindiciaOneTimePlan = plans(i)
                'End If
                If BuilderSubBillingPlanMerchantKey <> "" AndAlso plan = BuilderSubBillingPlanMerchantKey Then
                    vindiciaPlan = plans(i)
                    'If NewRegistration And RegType = "New" Then
                    '    If IsRecurring = "Yes" Then
                    '        vindiciaPlan = plans(i)
                    '    End If
                    'ElseIf Not NewRegistration And RegType = "Existing" Then
                    '    If IsRecurring = "Yes" Then
                    '        vindiciaPlan = plans(i)
                    '    End If
                    '    'Builder is not new  and regtype = new 
                    'ElseIf Not NewRegistration And RegType = "New" Then
                    '    If IsRecurring = "Yes" Then
                    '        vindiciaPlan = plans(i)
                    '    End If
                    'End If
                ElseIf BuilderRegBillingPlanMerchantKey <> "" AndAlso plan = BuilderRegBillingPlanMerchantKey Then
                    vindiciaOneTimePlan = plans(i)
                    'If NewRegistration And RegType = "New" Then
                    '    vindiciaOneTimePlan = plans(i)
                    'ElseIf Not NewRegistration And RegType = "Existing" Then
                    '    vindiciaOneTimePlan = plans(i)
                    '    'Builder is not new  and regtype = new 
                    'ElseIf Not NewRegistration And RegType = "New" Then
                    '    vindiciaOneTimePlan = plans(i)
                    'End If
                End If
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
        '    vindiciaPlan = LoadHardErrorPlan(plans, BuilderSubBillingPlanMerchantKey)
        'End If
        '**************************************************************************************************************************************

        If vindiciaPlan Is Nothing And Not IsSubscriptionActive Then
            If IsCurrentBuilder Then
                'Builder exists in vindicia but no billing plan
                Logger.Error("Could not locate applicable billing plan (CURRENT builder)")
                'redirect to payment page and add the error
                HttpContext.Current.Session.Add("ADDERROR_SESSION_" & dbBuilder.HistoricID, "There are no applicable billing plans. Please <a href=""/builder/"">click here</a> to go to your dashboard.  For questions please contact your Operations Manager. Otherwise email us at customerservice@cbusa.us.")

            Else
                Logger.Error("Could not locate applicable billing plan (NEW builder)")
                HttpContext.Current.Session.Add("ADDERROR_SESSION_" & dbBuilder.HistoricID, "An error was encountered while processing this request. For immediate assistance please contact your Operations Manager. Otherwise email us at customerservice@cbusa.us.")
            End If

            'THIS SHOULD NOT HAPPEN SINCE WE WERE REDIRECTED FROM PAYMENTPAGE. STILL ADDING QUERY PARAMETER TO IDENTIFY WE ARE REDIRECTING FROM HOA SUCCESS PAGE
            Response.Redirect("/forms/builder-registration/payment.aspx?vinHoa=ln326", True)

            Exit Sub
        End If

        'If vindiciaOneTimePlan IsNot Nothing And Not IsRegistrationActive Then
        If vindiciaOneTimePlan IsNot Nothing And NewRegistration Then
            FirstBillDate = (From pair In vindiciaOneTimePlan.nameValues Where pair.name = "1stBillDate" Select pair.value).FirstOrDefault
            LoadOneTimePlan(vindiciaOneTimePlan, IIf(IsDate(FirstBillDate), FirstBillDate, Now))
            rowCount += 1
        End If

        If Not IsSubscriptionActive And dtCatchupStartDate.Date = Now.Date Then
            vindiciaPeriods = vindiciaPlan.periods
        End If
        monthCount = 0

    End Sub

    Private Sub LoadOneTimePlan(ByVal plan As Vindicia.BillingPlan, ByVal FirstBillDate As DateTime)

        If Not plan.periods(0).prices Is Nothing Then
            totalAmount += plan.periods(0).prices(0).amount
        End If

    End Sub


    Private Sub CommitWebSession()
        ' Grab data
        Dim sessionId As String = Request("Session_Id")
        Dim vinWebSessionReturn As Vindicia.Return
        Dim postedValues() As Vindicia.NameValuePair
        Dim requestRedirectFromUpdatePage As Boolean = False
        initializedWebsession = New Vindicia.WebSession
        Try
            vinWebSessionReturn = New Vindicia.WebSession().fetchByVid("", sessionId, initializedWebsession)
            If vinWebSessionReturn.returnCode <> 200 AndAlso initializedWebsession.apiReturn.returnCode <> 200 Then
                Throw New Exception("Websession was not retreived. Check Log for SOAP ID : " & vinWebSessionReturn.soapId & vbCrLf & " Returned String : " & vinWebSessionReturn.returnString)
            End If

            'Get Posted Values so that we can update our dbs
            postedValues = initializedWebsession.postValues()

            'Since we were able to retreive required values we can commit websession
            Dim responseCommit As Vindicia.[Return] = initializedWebsession.finalize("")
            If initializedWebsession.apiReturn.returnCode <> 200 Then
                Throw New Exception("Websession was able to finialized. Check Log for SOAP ID : " & responseCommit.soapId & vbCrLf & " Returned String : " & responseCommit.returnString)
            End If

            Dim WebSessionLogger As New VindiciaWebSession(DB, dbBuilder, VindiciaWebSessionMethods.Account_updatePaymentMethod, HttpContext.Current)
            WebSessionLogger.LogReturn(responseCommit, "WebSession --> Finalized Account_updatePaymentMethod")

        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
            'redirect to error url so that we can dispose the session. Websession didn't work. THIS SHOULD BE A RARE situation
            Response.Redirect(initializedWebsession.errorURL & "?vinHoa=ln370")
        End Try


        'since commit successed for paymentmethod update we can start with transaction
        Try
            'grabbing billing address posted to vindicia only if any    
            Dim getSameAsbilling As Func(Of Vindicia.NameValuePair, Boolean) = Function(sPair)
                                                                                   Return sPair.name = "ae_chkSameAsBilling"
                                                                               End Function

            Dim isSameasBilling As String = (From sPair In postedValues
                                             Where getSameAsbilling(sPair)
                                             Select sPair.value).FirstOrDefault()

            requestRedirectFromUpdatePage = (From sPair In postedValues
                                             Where sPair.name = "ae_Requested_from_page"
                                             Select sPair.value).FirstOrDefault().ToLower = "true"


            Dim account As New Vindicia.Account
            Dim newBillingAddress As New Vindicia.Address
            account.fetchByMerchantAccountId("", dbBuilder.HistoricID.ToString(), account)
            If isSameasBilling Is Nothing Then
                'Grabbing account from commited results
                With newBillingAddress
                    For Each postedValue As Vindicia.NameValuePair In postedValues
                        System.Diagnostics.Debug.WriteLine(postedValue.name & ":" & postedValue.value)

                        Select Case postedValue.name
                            Case "vin_PaymentMethod_billingAddress_name"
                                .name = postedValue.value
                            Case "vin_PaymentMethod_billingAddress_addr1"
                                .addr1 = postedValue.value
                            Case "vin_PaymentMethod_billingAddress_addr2"
                                .addr2 = postedValue.value
                            Case "vin_PaymentMethod_billingAddress_city"
                                .city = postedValue.value
                            Case "vin_PaymentMethod_billingAddress_district"
                                .district = postedValue.value
                            Case "vin_PaymentMethod_billingAddress_postalCode"
                                .postalCode = Replace(postedValue.value, "-", "")
                            Case "ae_Requested_from_page"
                                requestRedirectFromUpdatePage = Core.GetBoolean(postedValue.value)
                        End Select
                    Next
                    .country = "US"
                End With
            Else
                With newBillingAddress
                    .name = dbBuilder.CompanyName
                    .addr1 = dbBuilder.Address
                    .addr2 = dbBuilder.Address2
                    .city = dbBuilder.City
                    .district = dbBuilder.State
                    .postalCode = dbBuilder.Zip
                End With
            End If

            account.shippingAddress = newBillingAddress
            'account.updatePaymentMethod("", account.paymentMethods(0), True, Vindicia.PaymentUpdateBehavior.CatchUp, True, True, True, True, New Boolean())
            Dim successes() As String
            Dim failures() As String

            account.updatePaymentMethod("", account.paymentMethods(0), UpdateScope.AllDue, PaymentUpdateBehavior.CatchUp, True, True, True, True, UpdateScope.None, New Boolean(), successes, failures)

            account.update("", New Boolean())
        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
            'Not redirecting here since websession was already commited. 
        End Try

        If requestRedirectFromUpdatePage Then
            ProcessUpdatePageLogic()
        Else
            ProcessPaymentPageLogic()
        End If


    End Sub


    Public Sub ProcessPaymentPageLogic()

        BindProducts()

        Try
            'LOGIC COPIED FROM PAYMENT.ASPX.VB ~ btnProcess_Click SUB

            ' Builder Payment Log
            DB.BeginTransaction()
            dbPayment = New BuilderRegistrationPaymentRow(DB)
            Dim dbRegistration As BuilderRegistrationRow = BuilderRegistrationRow.GetRowByBuilder(DB, dbBuilder.BuilderID)
            dbPayment.BuilderRegistrationID = dbRegistration.BuilderRegistrationID
            dbPayment.PaymentStatusID = 1
            dbPayment.Submitted = Now()
            'Never store CC data id payment gateway is used per Ryan M.
            dbPayment.CardNumber = "Not Saved"
            dbPayment.CardholderName = "Not Saved"
            dbPayment.Insert()
            DB.CommitTransaction()

            Dim p As New VindiciaPaymentProcessor(DB)
            p.IsTestMode = CBool(SysParam.GetValue(DB, "TestMode"))

            Dim account As New Vindicia.Account
            account.fetchByMerchantAccountId("", dbBuilder.HistoricID.ToString(), account)

            'CLEANUP: THIS SUB AWAYS RETURNED TRUE PER FUNCTION LOGIC SO NO POINT OF ADDING ERROR HERE 
            p.SubmitCVVTransaction(account)

            'Per client request, if registration fee isn't processed first then don't let builder continue.
            Dim IsPaymentSuccess As Boolean = True
            Dim IsNew As Boolean = True

            'SOME Variables gets in bind products
            If vindiciaOneTimePlan IsNot Nothing And NewRegistration Then
                Dim start As String = (From pair In vindiciaOneTimePlan.nameValues Where pair.name = "1stBillDate" Select pair.value).FirstOrDefault
                If start = Nothing OrElse start = "0" Then
                    start = Now
                End If
                '****************** Registration fees should be charged immediately ***********************
                'Dim dbBBP As BuilderBillingPlanRow = BuilderBillingPlanRow.GetRow(DB, dbBuilder.BuilderID)
                If p.StartBilling(registrationProduct, vindiciaOneTimePlan, Now.Date, dbBuilder, account) Then
                    IsNew = False
                Else
                    IsPaymentSuccess = False
                    Logger.Error("Registration AutoBill failure. ReturnCode=" & p.ReturnCode & ", ReturnString=" & p.ReturnString & ", SoapID=" & p.SoapId)
                End If
            End If

            If IsPaymentSuccess Then
                Dim dbStatus As RegistrationStatusRow = RegistrationStatusRow.GetRowByStatus(DB, "Pending")
                dbRegistration.RegistrationStatusID = dbStatus.RegistrationStatusID
                dbRegistration.CompleteDate = Now
                dbRegistration.Update()

                If Not IsNew Then
                    dbBuilder.IsNew = False
                    dbBuilder.Update()
                End If

                'Only send email if builder registers for the first time.
                If Not IsCurrentBuilder And Not SysParam.GetValue(DB, "TestMode") Then
                    Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "NewBuilderForCBUSA")
                    Dim dtBuilders As DataTable = BuilderRow.GetListByLLC(DB, dbBuilder.LLCID)
                    Try
                        'Send Notification to Admins Only.

                        dbMsg.SendLLCNotificationListAReminder(LLCRow.GetLLCNotificationList(DB, dbBuilder.LLCID), "New Builder: " & vbTab & dbBuilder.CompanyName & vbCrLf & "LLC : " & Core.GetString(LLCRow.GetRow(DB, dbBuilder.LLCID).LLC) & vbCrLf & vbCrLf & GlobalRefererName)
                    Catch ex As Exception

                    End Try
                    For Each row As DataRow In dtBuilders.Rows
                        If Core.GetInt(row("BuilderId")) <> dbBuilder.BuilderID Then
                            Dim dbMsgBuilder As BuilderRow = BuilderRow.GetRow(DB, row("BuilderId"))
                            Dim LLCname As String = Core.GetString(LLCRow.GetRow(DB, dbBuilder.LLCID).LLC)
                            If LLCname <> String.Empty Then
                                LLCname = "LLC : " & LLCname
                            End If
                            dbMsg.Send(dbMsgBuilder, "New Builder: " & vbTab & dbBuilder.CompanyName & vbCrLf & LLCname & vbCrLf & vbCrLf & GlobalRefererName, CCLLCNotification:=False)
                        End If
                    Next

                    dbMsg = AutomaticMessagesRow.GetRowByTitle(DB, "NewBuilderForVendors")
                    Dim dtVendors As DataTable = VendorRow.GetListByLLC(DB, dbBuilder.LLCID)
                    Dim BuilderLLCname As String = Core.GetString(LLCRow.GetRow(DB, dbBuilder.LLCID).LLC)
                    Dim NewBuilderForVendors As String = String.Empty
                    If BuilderLLCname <> String.Empty Then
                        BuilderLLCname = "LLC : " & BuilderLLCname
                    End If

                    For Each row As DataRow In dtVendors.Rows
                        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, row("VendorId"))
                        NewBuilderForVendors = "Dear " & Core.GetString(row("CompanyName")) & ", " & vbCrLf & "The " & Core.GetString(LLCRow.GetRow(DB, dbBuilder.LLCID).BuilderGroup) & " has added a new builder partner to the CBUSA Network."
                        Dim additionalText As String = "To view the details of " & dbBuilder.CompanyName & " , please login to the Member Directory of the CBUSA Software." & vbCrLf & "https://app.custombuilders-usa.com/directory/"
                    Next

                    Try
                        Dim additionalAdminText As String = "To view the details of " & dbBuilder.CompanyName & " , please login to the Member Directory of the CBUSA Software." & vbCrLf & "https://app.custombuilders-usa.com/directory/"
                        dbMsg.SendLLCNotificationListAReminder(LLCRow.GetLLCNotificationList(DB, dbBuilder.LLCID), NewBuilderForVendors & vbCrLf & vbCrLf & "New Builder: " & vbTab & dbBuilder.CompanyName & vbCrLf & vbCrLf & additionalAdminText)
                    Catch ex As Exception

                    End Try

                    '====================================== SEND EMAIL TO ALL CBUSA ADMIN USERS ========================================= 
                    dbMsg = AutomaticMessagesRow.GetRowByTitle(DB, "NewBuilderForCBUSA")

                    Dim AdminUsers As DataTable = AdminRow.GetAllAdmins(DB)
                    For Each DataRow In AdminUsers.Rows
                        Dim dbAdmin As AdminRow = AdminRow.GetRow(DB, DataRow("adminid"))
                        Dim strAdminMessage As String = "New Builder: " & vbTab & dbBuilder.CompanyName & vbCrLf & "LLC : " & Core.GetString(LLCRow.GetRow(DB, dbBuilder.LLCID).LLC) & vbCrLf & vbCrLf & GlobalRefererName

                        Core.SendSimpleMail("customerservice@cbusa.us", "customerservice@cbusa.us", dbAdmin.Email, dbAdmin.Email, dbMsg.Subject, strAdminMessage)
                    Next
                    '====================================================================================================================
                End If
            End If

            If IsPaymentSuccess And Not IsSubscriptionActive Then
                Dim subStart As String = Now
                If Not vindiciaPlan.nameValues Is Nothing Then
                    Try
                        subStart = (From pair In vindiciaPlan.nameValues Where pair.name = "1stBillDate" Select pair.value).FirstOrDefault
                        If subStart = Nothing OrElse subStart = "0" Then
                            subStart = Now
                        End If
                    Catch ex As Exception
                    End Try
                End If

                Dim dbBBP As BuilderBillingPlanRow = BuilderBillingPlanRow.GetRow(DB, dbBuilder.BuilderID)
                'Determine if we're back-billing when creating the new subscription autobill.
                If dtCatchupStartDate.Date < CDate(subStart).Date Then
                    subStart = dtCatchupStartDate.Date
                Else
                    If dbBBP.SubscriptionStartDate.Date > Now.Date Then
                        subStart = dbBBP.SubscriptionStartDate.Date
                    Else
                        subStart = Now.Date
                    End If
                End If

                If Not p.StartBilling(vindiciaProduct, vindiciaPlan, subStart, dbBuilder, account) Then
                    IsPaymentSuccess = False
                    Logger.Error("Subscription AutoBill failure. ReturnCode=" & p.ReturnCode & ", ReturnString=" & p.ReturnString & ", SoapID=" & p.SoapId)
                    Response.Redirect("/forms/builder-registration/VinHoaError.aspx?Session_Id=" & Request("Session_Id"))
                End If
            End If

            If IsPaymentSuccess Then
                'AR 03/23/11: Client wants to stop all suspended auto bills
                'BP 4/13 Stop all suspended autobills
                Try
                    Dim autoBills() As Vindicia.AutoBill = p.GetAutobills(dbBuilder)
                    For Each ab As Vindicia.AutoBill In autoBills
                        If ab.status = Vindicia.AutoBillStatus.LegacySuspended Then
                            ab.status = Vindicia.AutoBillStatus.Cancelled
                            ab.statusSpecified = True
                            p.CancelAutoBill(ab.VID)
                        End If
                    Next
                Catch ex As Exception
                    Logger.Error("Error  CancelAutoBill  . ")
                End Try

                'Dim dbStatus As RegistrationStatusRow = RegistrationStatusRow.GetRowByStatus(DB, "Pending")
                'dbRegistration.RegistrationStatusID = dbStatus.RegistrationStatusID
                'dbRegistration.CompleteDate = Now
                'dbRegistration.Update()

                'If Not IsNew Then
                '    dbBuilder.IsNew = False
                '    dbBuilder.Update()
                'End If

                Response.Redirect("/builder/default.aspx")
            End If

        Catch ex As ApplicationException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            Response.Redirect(initializedWebsession.errorURL & "?vinHoa=ln550")
        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            Response.Redirect(initializedWebsession.errorURL & "?vinHoa=ln553")
        End Try
    End Sub


    Public Sub ProcessUpdatePageLogic()

        'Payment method and address were updated when we committed web session
        Response.Redirect("/default.aspx")

    End Sub


    'This plan does not give one month free see comments above - 
    Private Function LoadHardErrorPlan(ByVal BillingPlan() As Vindicia.BillingPlan) As Vindicia.BillingPlan
        Dim HardErrorBillingPlan As Vindicia.BillingPlan = (From p As Vindicia.BillingPlan In BillingPlan Where p.merchantBillingPlanId = "S099F000D030" Select p).FirstOrDefault
        Return HardErrorBillingPlan
    End Function

End Class
