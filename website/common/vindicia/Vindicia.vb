Imports Components
Imports DataLayer
Imports Vindicia
Imports System.Reflection
Imports System.Web

Namespace Components

    Public Class VindiciaPaymentProcessor

        Protected DB As Database

        Private m_ReturnCode As String
        Public ReadOnly Property ReturnCode() As String
            Get
                Return m_ReturnCode
            End Get
        End Property

        Private m_ReturnString As String
        Public ReadOnly Property ReturnString() As String
            Get
                Return m_ReturnString
            End Get
        End Property

        Private m_SoapId As String
        Public ReadOnly Property SoapId() As String
            Get
                Return m_SoapId
            End Get
        End Property

        Private m_IsTestMode As Boolean
        Public Property IsTestMode() As Boolean
            Get
                Return m_IsTestMode
            End Get
            Set(ByVal value As Boolean)
                m_IsTestMode = value
            End Set
        End Property

        Public Sub New(ByVal DB As Database)
            Me.DB = DB
        End Sub

        Public Function GetProducts() As Vindicia.Product()
            Dim client As New Vindicia.Product()
            InitializeEnvironment()

            Dim out(0) As Vindicia.Product

            Dim ret As Vindicia.Return = client.fetchAll("", 0, True, 100, True, out)
            LogReturn("Product.fetchAll() -- GetProducts", ret)
            Return out
        End Function

        Public Function GetBillingPlans() As Vindicia.BillingPlan()
            Dim client As New Vindicia.BillingPlan
            InitializeEnvironment()

            Dim out(0) As Vindicia.BillingPlan

            Dim ret As Vindicia.Return = client.fetchAll("", 0, True, 100, True, out)
            LogReturn("BillingPlan.fetchAll() -- GetBillingPlans", ret)
            Return out
        End Function

        Public Function GetBillingPlansForAdmin() As Vindicia.BillingPlan()
            Dim VindiciaUserName As String = SysParam.GetValue(DB, "VindiciaLogin")
            Dim VindiciaPassword As String = SysParam.GetValue(DB, "VindiciaPassword")

            InitializeEnvironment()

            Dim client As New Vindicia.BillingPlan(VindiciaUserName, VindiciaPassword)
            Dim out(0) As Vindicia.BillingPlan

            Dim ret As Vindicia.Return = client.fetchAll("", 0, True, 100, True, out)
            LogReturn("BillingPlan.fetchAll() -- GetBillingPlans", ret)
            Return out
        End Function

        Public Function GetBillingPlanByMerchantID(ByVal pstrMerchantBillingPlanId As String) As Vindicia.BillingPlan
            Dim VindiciaUserName As String = SysParam.GetValue(DB, "VindiciaLogin")
            Dim VindiciaPassword As String = SysParam.GetValue(DB, "VindiciaPassword")

            InitializeEnvironment()

            Dim client As New Vindicia.BillingPlan(VindiciaUserName, VindiciaPassword)

            Dim out(0) As Vindicia.BillingPlan
            Dim matchingBillingPlan As Vindicia.BillingPlan = New BillingPlan()

            Dim ret As Vindicia.Return = client.fetchAll("", 0, True, 100, True, out)
            LogReturn("BillingPlan.fetchAll() -- GetBillingPlans", ret)

            For Each bp As Vindicia.BillingPlan In out
                If bp.merchantBillingPlanId = pstrMerchantBillingPlanId Then
                    matchingBillingPlan = bp
                    Exit For
                End If
            Next

            Return matchingBillingPlan

        End Function

        Public Function CalculateTax(ByVal Builder As BuilderRow, ByVal Quantity As Integer, ByVal Price As Decimal) As Decimal

            InitializeEnvironment()
            Dim address As New Vindicia.Address
            With address
                .name = Builder.CompanyName
                .addr1 = Builder.Address
                .district = Builder.State
                .postalCode = Builder.Zip
                .country = "US"
            End With
            Dim account As New Vindicia.Account
            With account
                .merchantAccountId = Builder.HistoricID
                .shippingAddress = address
                .name = Builder.CompanyName
            End With
            'Dim CreditCard As New Vindicia.CreditCard
            'With CreditCard
            '    .account = "4444222211113333"
            '    .expirationDate = "201510"
            'End With
            'Dim paymentmethod As New Vindicia.PaymentMethod
            'With paymentmethod
            '    .accountHolderName = "Test for Tax"
            '    .billingAddress = address
            '    .type = PaymentMethodType.CreditCard
            '    .creditCard = CreditCard
            '    .sortOrder = 1
            '    .active = True

            'End With

            'ReDim account.paymentMethods(0)
            'account.paymentMethods(0) = paymentmethod

            'Dim ret As Vindicia.Return = account.update(New Boolean)

            'If ret.returnCode = 200 Then
            '    Dim test As String = "account done"

            'End If

            Dim transactionItem As New Vindicia.TransactionItem

            With transactionItem
                .sku = "Subscription"
                .price = Price
                .quantity = Quantity
                .name = "Subscription"
            End With

            Dim transaction As New Vindicia.Transaction

            With transaction
                .account = account
                .amount = Price
                .currency = "USD"
                .merchantTransactionId = Core.GenerateFileID
                .shippingAddress = address
                ReDim .transactionItems(0)
                .transactionItems(0) = New Vindicia.TransactionItem()
                .transactionItems(0) = transactionItem
            End With

            Dim taxitems() As Vindicia.SalesTax = Nothing
            Dim totaltax As Decimal = 0.0
            Dim taxspec As Boolean
            Dim rettax As Vindicia.Return = transaction.calculateSalesTax("", Nothing, Nothing, Nothing, Nothing, taxitems, totaltax, taxspec)

            If rettax.returnCode = 200 Then
                Return totaltax
            End If


            '$response = $tx->calculateSalesTax();
            'if ($response['returnCode'] == 200) {
            '    print "Address Dim test4 As Decimal = totaltaxtype used for computing tax: ";
            '    print $response['data']->addressType . "\n";
            '    print "Taxes added: \n";

            '    $transaction = $response['data']->transaction;
            '    $transaction_items = $transaction->transactionItems;    

            '    foreach($transaction_items as $tran_item) {

            '        print "Tax line item: " . $tran_item->tax;
            '    }

            '    print "Total tax: " . $response['data']->totalTax;

            '}
            'else{
            '    print_r($response);
            '}

            '?>
            Return totaltax
        End Function

        Private Sub EnsurePaymentMethod(ByRef acct As Vindicia.Account, ByVal addr As VindiciaAddressInfo, ByVal cc As CreditCardInfo)
            If acct.paymentMethods Is Nothing OrElse acct.paymentMethods.Length = 0 Then
                ReDim acct.paymentMethods(0)
                acct.paymentMethods(0) = GetVindiciaPaymentMethod(addr, cc)
            Else
                Dim pm As Vindicia.PaymentMethod = acct.paymentMethods(0)
            End If
        End Sub

        Public Function StartBilling(ByVal Product As Vindicia.Product, ByVal Plan As Vindicia.BillingPlan, ByVal StartDate As DateTime, ByVal Builder As BuilderRow, ByVal addr As VindiciaAddressInfo, ByVal cc As CreditCardInfo) As Boolean
            Dim ab As New Vindicia.AutoBill
            InitializeEnvironment()
            'extra try/catch to make sure invalid account errors are caught
            Try
                ab.account = UpdateVindiciaAccount(Builder, addr, cc)
                If ab.account Is Nothing Then
                    ab.account = CreateVindiciaAccount(Builder, addr, cc)

                    If ab.account.paymentMethods Is Nothing OrElse ab.account.paymentMethods.Length = 0 Then
                        ReDim ab.account.paymentMethods(0)
                    End If
                End If
            Catch ex As Exception
                Logger.Error(Logger.GetErrorMessage(ex))
                Throw New ApplicationException("Error creating Vindicia account")
            End Try

            ab.paymentMethod = GetVindiciaPaymentMethod(addr, cc) 'ab.account.paymentMethods(0)

            ReDim ab.items(0)
            ab.items(0) = New AutoBillItem()
            ab.items(0).product = Product
            Dim LLCAffliateID As String = String.Empty
            ab.billingPlan = Plan
            ab.sourceIp = HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")
            ab.startTimestamp = StartDate
            ab.startTimestampSpecified = True
            ab.merchantAutoBillId = Core.GenerateFileID
            Try
                LLCAffliateID = LLCRow.GetRow(DB, Builder.LLCID).AffiliateID.ToString("D3")
                ab.merchantAffiliateId = LLCAffliateID
            Catch ex As Exception
            End Try


            'For suspended plans
            ' Dim client As New AutoBill.AutoBillPortTypeClient

            Dim created As Boolean = Nothing
            Dim firstBillDate As DateTime = Nothing
            Dim firstBillAmount As String = Nothing
            Dim firstBillCurrency As String = Nothing
            Dim transactionStatus As Vindicia.TransactionStatus = Nothing
            Dim score As Integer = 0
            Dim scoreCodes() As Vindicia.ScoreCode = Nothing
            Dim initialTrans As Vindicia.Transaction = Nothing
            Dim trn As Vindicia.Transaction

            'Dim ret As Vindicia.Return = ab.update(DuplicateBehavior.Fail, True, True, True, SysParam.GetValue(DB, "VindiciaFraudThreshold"), True, True, True, True, True, String.Empty, False, False, created, transactionStatus, firstBillDate, True, firstBillAmount, True, firstBillCurrency, score, False, scoreCodes)
            Dim ret As Vindicia.Return = ab.update("", ImmediateAuthFailurePolicy.putAutoBillInRetryCycleIfPaymentMethodIsValid, False, True, True, SysParam.GetValue(DB, "VindiciaFraudThreshold"), True, True, True, True, True, String.Empty, False, False, "", InitialAuthStrategy.AuthImminentBilling, True, trn, transactionStatus, score, False, scoreCodes, initialTrans)
            ' Dim ret As Vindicia.Return = AutoBill.update(GetAuth(), ab, created, firstBillDate, firstBillAmount, firstBillCurrency,
            '  AllDataTypes.DuplicateBehavior.Fail, True, SysParam.GetValue(DB, "VindiciaFraudThreshold"))
            'LogReturn("AutoBill.update() -- StartBilling", ret, Builder.HistoricID)
            Return ret.returnCode = 200
        End Function

        Public Function StartBilling(ByVal Product As Vindicia.Product, ByVal Plan As Vindicia.BillingPlan, ByVal StartDate As DateTime, ByVal Builder As BuilderRow, ByVal account As Vindicia.Account) As Boolean
            Dim ab As New Vindicia.AutoBill
            InitializeEnvironment()
            'extra try/catch to make sure invalid account errors are caught

            ab.account = account

            ab.paymentMethod = GetAccountPaymentMethods(account)

            ReDim ab.items(0)
            ab.items(0) = New AutoBillItem()
            ab.items(0).product = Product
            Dim LLCAffliateID As String = String.Empty
            ab.billingPlan = Plan
            ab.sourceIp = HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")
            ab.startTimestamp = StartDate
            ab.startTimestampSpecified = True
            ab.merchantAutoBillId = Core.GenerateFileID
            Try
                LLCAffliateID = LLCRow.GetRow(DB, Builder.LLCID).AffiliateID.ToString("D3")
                ab.merchantAffiliateId = LLCAffliateID
            Catch ex As Exception
            End Try


            'For suspended plans
            ' Dim client As New AutoBill.AutoBillPortTypeClient

            Dim created As Boolean = Nothing
            Dim firstBillDate As DateTime = Nothing
            Dim firstBillAmount As String = Nothing
            Dim firstBillCurrency As String = Nothing
            Dim transactionStatus As Vindicia.TransactionStatus = Nothing
            Dim score As Integer = 0
            Dim scoreCodes() As Vindicia.ScoreCode = Nothing
            Dim initialTrans As Vindicia.Transaction = Nothing
            Dim trn As Vindicia.Transaction

            Dim immAuthFailurePolicy As ImmediateAuthFailurePolicy

            If Plan.merchantBillingPlanId.StartsWith("VIN-REG") Then
                immAuthFailurePolicy = ImmediateAuthFailurePolicy.doNotSaveAutoBill
            Else
                immAuthFailurePolicy = ImmediateAuthFailurePolicy.putAutoBillInRetryCycleIfPaymentMethodIsValid
            End If

            'Dim ret As Vindicia.Return = ab.update(DuplicateBehavior.Fail, True, True, True, SysParam.GetValue(DB, "VindiciaFraudThreshold"), True, True, True, True, True, String.Empty, False, False, created, transactionStatus, firstBillDate, True, firstBillAmount, True, firstBillCurrency, score, False, scoreCodes)
            Dim ret As Vindicia.Return = ab.update("", immAuthFailurePolicy, True, True, True, SysParam.GetValue(DB, "VindiciaFraudThreshold"), True, True, True, True, True, String.Empty, False, False, "", InitialAuthStrategy.AuthImminentBilling, True, trn, transactionStatus, score, False, scoreCodes, initialTrans)

            ' Dim ret As Vindicia.Return = AutoBill.update(GetAuth(), ab, created, firstBillDate, firstBillAmount, firstBillCurrency,
            '  AllDataTypes.DuplicateBehavior.Fail, True, SysParam.GetValue(DB, "VindiciaFraudThreshold"))
            'LogReturn("AutoBill.update() -- StartBilling", ret, Builder.HistoricID)
            Return ret.returnCode = 200

        End Function

        Public Function CheckSubscriptionStatus(ByVal builder As BuilderRow, ByRef RegDate As DateTime) As Boolean

            If builder.SkipEntitlementCheck Then
                Return True
            Else
                Dim acct As Vindicia.Account = GetVindiciaAccount(builder)
                If acct Is Nothing Then
                    Return False
                Else
                    Dim entitlements As Vindicia.Entitlement() = GetEntitlements(acct)
                    If entitlements IsNot Nothing Then
                        Logger.Info("Returned " & entitlements.Length & " entitlements")
                    End If
                    For Each e As Vindicia.Entitlement In entitlements
                        If e.active Then
                            Logger.Info("Active Entitlement found: " & e.merchantEntitlementId)
                            Return True
                        End If
                    Next
                    'AR 3/23/11: Client wants to always send user to update payment screen if they done have any active entitlements.
                    Return False
                    'Dim abs As Vindicia.AutoBill() = GetExistingAutoBills(acct)
                    'If abs Is Nothing OrElse abs.Length = 0 Then
                    '    Return False
                    'Else
                    '    For Each ab As Vindicia.AutoBill In abs
                    '        If ab.status = Vindicia.AutoBillStatus.Active Then
                    '            RegDate = ab.startTimestamp
                    '            Return True
                    '        End If
                    '    Next
                    '    Return False
                    'End If
                End If
            End If


            'Dim acct As AllDataTypes.Account = GetVindiciaAccount(builder)
            'If acct Is Nothing Then
            '    Return False
            'Else
            '    If SysParam.GetValue(DB, "UseEntitlements") Then

            '        'override API result for flagged accounts -- needed for Vindicia bug, 4/6/2009
            '        If builder.SkipEntitlementCheck Then Return True

            '        'Dim entitlements As AllDataTypes.Entitlement() = GetEntitlements(acct)
            '        'If entitlements IsNot Nothing Then
            '        '    Logger.Info("Returned " & entitlements.Length & " entitlements")
            '        'End If
            '        'For Each e As AllDataTypes.Entitlement In entitlements
            '        '    If e.active Then
            '        '        Logger.Info("Active Entitlement found: " & e.merchantEntitlementId)
            '        '        Return True
            '        '    End If
            '        'Next
            '        Dim abs As AllDataTypes.AutoBill() = GetExistingAutoBills(acct)
            '        If abs Is Nothing OrElse abs.Length = 0 Then
            '            Return False
            '        Else
            '            For Each ab As AllDataTypes.AutoBill In abs
            '                If ab.status = AllDataTypes.AutoBillStatus.Active Then
            '                    RegDate = ab.startTimestamp
            '                    Return True
            '                End If
            '            Next
            '            Return False
            '        End If
            '    Else
            '        Dim abs As AllDataTypes.AutoBill() = GetExistingAutoBills(acct)
            '        If abs Is Nothing OrElse abs.Length = 0 Then
            '            Return False
            '        Else
            '            For Each ab As AllDataTypes.AutoBill In abs
            '                If ab.status = AllDataTypes.AutoBillStatus.Active Then
            '                    RegDate = ab.startTimestamp
            '                    Return True
            '                End If
            '            Next
            '            Return False
            '        End If
            '    End If
            'End If
        End Function

        Private Function GetEntitlements(ByVal acct As Vindicia.Account) As Vindicia.Entitlement()
            Dim client As New Vindicia.Entitlement
            InitializeEnvironment()


            Dim out As Vindicia.Entitlement()
            '  Dim ret As Vindicia.Return = client.fetchByAccount(out, GetAuth, acct, True)
            Dim ret As Vindicia.Return = client.fetchByAccount("", acct, False, False, False, False, out)
            LogReturn("Entitlement.fetchByAccount() -- GetEntitlements", ret, acct.merchantAccountId)
            Return out
        End Function

        Protected Function GetAuth() As Vindicia.Authentication

            'TESTING - disables SSL validation
            System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf SecurityCallback

            '********** Added following line (by Apala - Medullus) on 19.12.2017 for PCI Compliance
            System.Net.ServicePointManager.SecurityProtocol = Net.SecurityProtocolType.Tls12

            Dim auth As New Vindicia.Authentication()
            auth.login = SysParam.GetValue(DB, "VindiciaLogin")
            auth.password = SysParam.GetValue(DB, "VindiciaPassword")
            auth.version = "7.0"
            Return auth
        End Function

        Protected Function SecurityCallback() As Boolean
            Return True
        End Function

        Protected Function GetEndpointAddress() As String
            Return IIf(IsTestMode, SysParam.GetValue(DB, "VindiciaServiceTestAddress"), SysParam.GetValue(DB, "VindiciaServiceAddress"))
        End Function

        Protected Sub InitializeEnvironment()
            '********** Added following line (by Apala - Medullus) on 19.12.2017 for PCI Compliance
            System.Net.ServicePointManager.SecurityProtocol = Net.SecurityProtocolType.Tls12
            System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf SecurityCallback

            Vindicia.Environment.SetEndpoint(IIf(IsTestMode, "https://soap.prodtest.sj.vindicia.com", SysParam.GetValue(DB, "VindiciaServiceAddress")))
            Vindicia.Environment.SetAuth(SysParam.GetValue(DB, "VindiciaLogin"), SysParam.GetValue(DB, "VindiciaPassword"))
        End Sub

        'Protected Sub InitializeEnvironment()
        '    System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf SecurityCallback
        '    Vindicia.Environment.SetEndpoint(IIf(IsTestMode, "https://soap.vindicia.com", "https://soap.vindicia.com"))
        '    Vindicia.Environment.SetAuth("cbusa_soap", "aDP47SrijDUP0WytGRvqXD3aztWCVpz0")
        'End Sub

        Public Sub LogReturn(ByVal Method As String, ByVal ret As Vindicia.Return, Optional ByVal Guid As String = Nothing)
            m_ReturnCode = ret.returnCode
            m_ReturnString = ret.returnString
            m_SoapId = ret.soapId

            Dim dbLog As New VindiciaSoapLogRow(DB)
            dbLog.SoapId = m_SoapId
            dbLog.ReturnCode = m_ReturnCode
            dbLog.ReturnString = m_ReturnString
            dbLog.SoapMethod = Method
            If Guid = Nothing Then
                If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session("BuilderId") IsNot Nothing Then
                    Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, HttpContext.Current.Session("BuilderId"))
                    dbLog.BuilderGUID = dbBuilder.HistoricID
                End If
            Else
                dbLog.BuilderGUID = Guid
            End If

            dbLog.Insert()

            If Integer.Parse(m_ReturnCode) > 0 Then
                'Logger.Error("Vindicia Error: ReturnCode=" & m_ReturnCode & ", ReturnString=" & m_ReturnString)
                'Throw New ApplicationException("There was an error communicating with the payment processor.  Please retry your request.")
            End If
        End Sub

        Private Function CreateVindiciaAccount(ByVal builder As BuilderRow, ByVal addr As VindiciaAddressInfo, ByVal cc As CreditCardInfo) As Vindicia.Account
            'Dim acct As AllDataTypes.Account = GetVindiciaAccount(builder)
            'If acct Is Nothing Then
            Dim address As New Vindicia.Address
            With address
                .name = builder.CompanyName
                .addr1 = builder.Address
                .district = builder.State
                .postalCode = builder.Zip
                .country = "US"
            End With

            Dim acct As New Vindicia.Account()
            acct.merchantAccountId = builder.HistoricID
            'End If
            acct.company = builder.CompanyName
            acct.emailAddress = addr.Email
            acct.emailTypePreferenceSpecified = True
            acct.emailTypePreference = EmailPreference.html
            acct.name = Core.BuildFullName(addr.FirstName, String.Empty, addr.LastName)
            acct.shippingAddress = address
            ReDim acct.paymentMethods(0)
            acct.paymentMethods(0) = GetVindiciaPaymentMethod(addr, cc)
            Return acct
        End Function

        Private Function UpdateVindiciaAccount(ByVal Builder As BuilderRow, ByVal addr As VindiciaAddressInfo, ByVal cc As CreditCardInfo) As Vindicia.Account
            Dim acct As Vindicia.Account = GetVindiciaAccount(Builder)
            If acct Is Nothing Then
                Return Nothing
            End If

            Dim Physicaladdress As New Vindicia.Address
            With Physicaladdress
                .name = Builder.CompanyName
                .addr1 = Builder.Address
                .district = Builder.State
                .postalCode = Builder.Zip
                .city = Builder.City
                .country = "US"
            End With

            Dim client As New Vindicia.Account
            'client.Endpoint.Address = New ServiceModel.EndpointAddress(GetEndpointAddress())
            InitializeEnvironment()

            acct.merchantAccountId = Builder.HistoricID
            acct.company = Builder.CompanyName
            acct.emailAddress = addr.Email
            acct.shippingAddress = Physicaladdress
            acct.emailTypePreferenceSpecified = True
            acct.emailTypePreference = EmailPreference.html
            acct.merchantAccountId = Builder.HistoricID
            acct.name = Core.BuildFullName(addr.FirstName, String.Empty, addr.LastName)

            ' Dim ret As alldatatypes.Return = client.update(GetAuth, acct, New Boolean)
            Dim ret As Vindicia.Return = client.update("", New Boolean)
            LogReturn("Account.update() -- UpdateVindiciaAccount", ret, Builder.HistoricID)

            'ReDim Preserve acct.paymentMethods(0)
            'Dim valid As Boolean = False
            'acct.paymentMethods(0) = GetVindiciaPaymentMethod(addr, cc)
            'ret = client.updatePaymentMethod(GetAuth, acct, valid, acct.paymentMethods(0), True, AllDataTypes.PaymentUpdateBehavior.CatchUp, -1)
            'LogReturn("Account.updatePaymentMethod() -- UpdateVindiciaAccount", ret, Builder.HistoricID)
            'If Not valid Then
            '    Throw New ApplicationException("Account information could not be validated.")
            'End If
            Return acct
        End Function

        Public Function UpdateVindiciaAddress(ByVal Builder As BuilderRow) As Boolean
            InitializeEnvironment()
            Dim acct As Vindicia.Account = GetVindiciaAccount(Builder)
            If acct Is Nothing Then
                Return False
            End If

            Dim Physicaladdress As New Vindicia.Address
            With Physicaladdress
                .name = Builder.CompanyName
                .addr1 = Builder.Address
                .district = Builder.State
                .postalCode = Builder.Zip
                .city = Builder.City
                .country = "US"
            End With

            ' Dim client As New Vindicia.Account
            'client.Endpoint.Address = New ServiceModel.EndpointAddress(GetEndpointAddress())


            ' acct.merchantAccountId = Builder.HistoricID
            acct.company = Builder.CompanyName
            acct.emailAddress = Builder.Email
            acct.shippingAddress = Physicaladdress
            acct.emailTypePreferenceSpecified = True
            acct.emailTypePreference = EmailPreference.html
            'acct.merchantAccountId = Builder.HistoricID
            ' Dim ret As alldatatypes.Return = client.update(GetAuth, acct, New Boolean)
            Dim ret As Vindicia.Return = acct.update("", New Boolean)
            LogReturn("AccountAddress.update() -- UpdateVindiciaAccount", ret, Builder.HistoricID)

            'ReDim Preserve acct.paymentMethods(0)
            'Dim valid As Boolean = False
            'acct.paymentMethods(0) = GetVindiciaPaymentMethod(addr, cc)
            'ret = client.updatePaymentMethod(GetAuth, acct, valid, acct.paymentMethods(0), True, AllDataTypes.PaymentUpdateBehavior.CatchUp, -1)
            'LogReturn("Account.updatePaymentMethod() -- UpdateVindiciaAccount", ret, Builder.HistoricID)
            'If Not valid Then
            '    Throw New ApplicationException("Account information could not be validated.")
            'End If
            If ret.returnCode = 200 Then
                LogReturn("acct.update() -- UpdateVindiciaAddress", ret, Builder.HistoricID)
                Return True
            Else
                LogReturn("acct.update() -- Failed UpdateVindiciaAddress", ret, Builder.HistoricID)
                Return False
            End If
        End Function

        Public Function EnsureVindiciaAccount(ByVal builder As BuilderRow) As Boolean
            Dim acct As Vindicia.Account = GetVindiciaAccount(builder)
            If acct Is Nothing Then
                Return False
            Else
                Return True
            End If
        End Function

        Private Function GetVindiciaAccount(ByVal builder As BuilderRow) As Vindicia.Account
            InitializeEnvironment()

            Dim client As New Vindicia.Account

            Dim acct As New Vindicia.Account

            '   Dim acct2() As Vindicia.Account = Nothing

            acct.merchantAccountId = builder.HistoricID
            Dim ret As Vindicia.Return = client.fetchByMerchantAccountId("", builder.HistoricID, acct)
            '  Dim ret2 As Vindicia.Return = client.fetchByEmail("bhavin.patel@americaneagle.com", acct2)
            LogReturn("Account.fetchByMerchantAccountId() -- GetVindiciaAccount", ret, builder.HistoricID)
            If ret.returnCode = 200 Then
                Return acct
            Else
                Return Nothing
            End If

        End Function

        Private Function GetExistingAutoBills(ByVal acct As Vindicia.Account) As Vindicia.AutoBill()
            Dim client As New Vindicia.AutoBill
            InitializeEnvironment()


            Dim bills As Vindicia.AutoBill()
            Dim ret As Vindicia.Return = client.fetchByAccount("", acct, False, False, 1, False, 1, False, bills)
            LogReturn("AutoBill.fetchByAccount() -- GetExistingAutoBills", ret, acct.merchantAccountId)
            If ret.returnCode = 200 Then
                Return bills
            Else
                Return Nothing
            End If
        End Function

        Private Function GetVindiciaPaymentMethod(ByVal addr As VindiciaAddressInfo, ByVal cc As CreditCardInfo) As Vindicia.PaymentMethod
            Dim meth As New Vindicia.PaymentMethod
            meth.accountHolderName = cc.CardHolderName
            meth.billingAddress = GetVindiciaAddress(addr)
            meth.creditCard = GetVindiciaCreditCard(cc)
            meth.type = Vindicia.PaymentMethodType.CreditCard
            ReDim meth.nameValues(0)
            meth.nameValues(0) = New Vindicia.NameValuePair()
            meth.nameValues(0).name = "CVN"
            meth.nameValues(0).value = cc.CID
            meth.typeSpecified = True
            Return meth
        End Function

        Private Function GetVindiciaAddress(ByVal address As VindiciaAddressInfo) As Vindicia.Address
            Dim addr As New Vindicia.Address
            addr.addr1 = address.AddressLn1
            addr.addr2 = address.AddressLn2
            addr.city = address.City
            addr.country = address.Country
            addr.county = address.County
            addr.district = address.State
            addr.name = Core.BuildFullName(address.FirstName, String.Empty, address.LastName)
            addr.postalCode = address.Zip
            Return addr
        End Function

        Private Function GetVindiciaAddress(ByVal address As Vindicia.Address) As Vindicia.Address
            Return address
        End Function

        Private Function GetVindiciaCreditCard(ByVal cardInfo As CreditCardInfo) As Vindicia.CreditCard
            Dim cc As New Vindicia.CreditCard
            cc.account = cardInfo.CreditCardNumber
            cc.expirationDate = cardInfo.ExpYear.ToString & IIf(cardInfo.ExpMonth < 10, "0", "") & cardInfo.ExpMonth.ToString("##")
            Return cc
        End Function

        Private Function GetVindiciaCreditCard(ByRef account As Vindicia.Account) As Vindicia.CreditCard
            Dim accountPaymentMethod As New Vindicia.PaymentMethod
            accountPaymentMethod.fetchByAccount("", account, False, False, account.paymentMethods)
            Return account.paymentMethods(0).creditCard
        End Function

        Private Function GetAccountPaymentMethods(ByRef account As Vindicia.Account) As Vindicia.PaymentMethod
            Dim client As New Vindicia.PaymentMethod
            Dim accountPaymentMethod() As Vindicia.PaymentMethod
            client.fetchByAccount("", account, True, True, accountPaymentMethod)

            If accountPaymentMethod.Length = 0 Then
                Return client
            End If

            Return accountPaymentMethod(0)
        End Function

        Public Function SubmitCVVTransaction(ByRef acc As Vindicia.Account) As Boolean

            InitializeEnvironment()

            Dim tran As New Vindicia.Transaction()
            tran.account = acc
            tran.amount = SysParam.GetValue(DB, "CVVTestAmount")
            tran.sourcePaymentMethod = GetAccountPaymentMethods(acc)
            tran.sourcePaymentMethod.type = PaymentMethodType.CreditCard
            tran.billingStatementIdentifier = SysParam.GetValue(DB, "CVVTestDescription")
            tran.merchantTransactionId = Core.GenerateFileID()
            ReDim tran.transactionItems(0)
            tran.transactionItems(0) = New Vindicia.TransactionItem()
            With tran.transactionItems(0)
                .name = "Validation Check"
                .sku = "11111"
                .price = tran.amount
                .quantity = 1
            End With

            Dim score As Integer = 0
            Dim scoreCodes() As Vindicia.ScoreCode = Nothing
            Dim ret As Vindicia.Return = tran.auth("", SysParam.GetValue(DB, "VindiciaFraudThreshold"), False, False, String.Empty, False, False, score, False, scoreCodes)
            LogCVVReturn(ret, tran)

            Return True

            'NOTE: FOLLOWING SIMILAR LOGIC AND RETURNING TRUE LIKE ORIGINAL FUNCTION SubmitCVVTransaction
            'If ret.returnCode = 200 _
            '    AndAlso (tran.statusLog(0).status = TransactionStatusType.Authorized Or tran.statusLog(0).status = TransactionStatusType.AuthorizedForValidation) _
            '    AndAlso New String() {"M", "U", "S"}.Contains(tran.statusLog(0).creditCardStatus.cvnCode.ToUpper) _
            '    AndAlso tran.statusLog(0).creditCardStatus.avsCode.ToUpper = "Y" Then

            '    Return True
            'Else
            '    Return False
            'End If
        End Function

        Public Function SubmitCVVTransaction(ByVal builder As BuilderRow, ByVal addr As VindiciaAddressInfo, ByVal cc As CreditCardInfo) As Boolean
            Dim client As New Vindicia.Transaction
            InitializeEnvironment()

            Dim tran As New Vindicia.Transaction()
            tran.account = UpdateVindiciaAccount(builder, addr, cc)
            If tran.account Is Nothing Then
                tran.account = CreateVindiciaAccount(builder, addr, cc)
            End If
            tran.amount = SysParam.GetValue(DB, "CVVTestAmount")
            tran.billingStatementIdentifier = SysParam.GetValue(DB, "CVVTestDescription")
            tran.sourcePaymentMethod = GetVindiciaPaymentMethod(addr, cc)
            tran.merchantTransactionId = Core.GenerateFileID()
            ReDim tran.transactionItems(0)
            tran.transactionItems(0) = New Vindicia.TransactionItem()
            With tran.transactionItems(0)
                .name = "Validation Check"
                .sku = "11111"
                .price = tran.amount
                .quantity = 1
            End With

            Dim score As Integer = 0
            Dim scoreCodes() As Vindicia.ScoreCode = Nothing
            ' Dim ret As Vindicia.Return = client.auth(GetAuth, tran, True, SysParam.GetValue(DB, "VindiciaFraudThreshold"))
            Dim ret As Vindicia.Return = client.auth("", SysParam.GetValue(DB, "VindiciaFraudThreshold"), False, False, String.Empty, False, False, score, False, scoreCodes)
            LogCVVReturn(ret, tran)


            Return True
            If ret.returnCode = 200 _
                AndAlso (tran.statusLog(0).status = TransactionStatusType.Authorized Or tran.statusLog(0).status = TransactionStatusType.AuthorizedForValidation) _
                AndAlso New String() {"M", "U", "S"}.Contains(tran.statusLog(0).creditCardStatus.cvnCode.ToUpper) _
                AndAlso tran.statusLog(0).creditCardStatus.avsCode.ToUpper = "Y" Then

                Return True
            Else
                Return False
            End If
        End Function

        Private Sub LogCVVReturn(ByVal ret As Vindicia.Return, ByVal tran As Vindicia.Transaction)
            m_ReturnCode = ret.returnCode
            m_ReturnString = ret.returnString
            m_SoapId = ret.soapId

            Dim dbLog As New VindiciaSoapLogRow(DB)
            dbLog.SoapId = m_SoapId
            dbLog.ReturnCode = m_ReturnCode
            dbLog.ReturnString = m_ReturnString & vbCrLf & vbCrLf

            'get transaction details in try/catch block in case null from invalid transaction
            Try

                dbLog.ReturnString &= "CVV2 Return Code: " & tran.statusLog(0).status.ToString & vbCrLf
                dbLog.ReturnString &= "AVS Return Code: " & tran.statusLog(0).creditCardStatus.avsCode & vbCrLf
                dbLog.ReturnString &= "Auth Return Code: " & tran.statusLog(0).creditCardStatus.authCode & vbCrLf
                dbLog.ReturnString &= "Verification Code: " & tran.statusLog(0).creditCardStatus.cvnCode & vbCrLf
            Catch ex As Exception
                Logger.Error(Logger.GetErrorMessage(ex))
            End Try

            If HttpContext.Current IsNot Nothing Then
                dbLog.BuilderGUID = HttpContext.Current.Session("BuilderId")
            End If
            dbLog.Insert()
        End Sub

        Public Function UpdateActivePaymentMethod(ByVal Builder As BuilderRow, ByVal addr As VindiciaAddressInfo, ByVal cc As CreditCardInfo) As Boolean
            Dim acct As Vindicia.Account = GetVindiciaAccount(Builder)
            If acct Is Nothing Then
                Return False
            End If

            Dim client As New Vindicia.Account
            ' InitializeEnvironment()

            Dim address As New Vindicia.Address
            With address
                .name = Builder.CompanyName
                .addr1 = Builder.Address
                .district = Builder.State
                .postalCode = Builder.Zip
                .country = "US"
            End With
            acct.merchantAccountId = Builder.HistoricID
            acct.company = Builder.CompanyName
            acct.emailAddress = addr.Email
            acct.emailTypePreferenceSpecified = True
            acct.emailTypePreference = EmailPreference.html
            acct.merchantAccountId = Builder.HistoricID
            acct.shippingAddress = address

            acct.name = Core.BuildFullName(addr.FirstName, String.Empty, addr.LastName)
            Dim created As Boolean
            Dim ret As Vindicia.Return = acct.update("", created)
            LogReturn("Account.update() -- UpdateActivePaymentMethod", ret, Builder.HistoricID)

            ReDim Preserve acct.paymentMethods(0)
            acct.paymentMethods(0) = GetVindiciaPaymentMethod(addr, cc)
            Dim validated As Boolean

            'ret = client.updatePaymentMethod(GetAuth, acct, True, acct.paymentMethods(0), True, AllDataTypes.PaymentUpdateBehavior.CatchUp, -1)
            'ret = acct.updatePaymentMethod(acct.paymentMethods(0), True, Vindicia.PaymentUpdateBehavior.CatchUp, True, True, True, True, validated)
            Dim successes() As String
            Dim failures() As String

            ret = acct.updatePaymentMethod("", acct.paymentMethods(0), UpdateScope.AllActive, PaymentUpdateBehavior.CatchUp, True, True, True, True, UpdateScope.None, validated, successes, failures)

            LogReturn("Account.updatePaymentMethod() -- UpdateActivePaymentMethod", ret, Builder.HistoricID)
            Return ret.returnCode = 200
        End Function

        Public Function UpdateActivePaymentMethod(acct As Vindicia.Account, addr As Vindicia.Address) As Boolean

            InitializeEnvironment()
            If acct Is Nothing Then
                Return False
            End If

            Dim validated As Boolean
            acct.paymentMethods(0).billingAddress = addr
            'Dim ret = acct.updatePaymentMethod(paymentMethod:=acct.paymentMethods(0), replaceOnAllAutoBills:=True, updateBehavior:=Vindicia.PaymentUpdateBehavior.Update _
            '    , ignoreAvsPolicy:=True, ignoreAvsPolicySpecified:=True, ignoreCvnPolicy:=True, ignoreCvnPolicySpecified:=True, validated:=validated)
            Dim successes() As String
            Dim failures() As String

            Dim ret = acct.updatePaymentMethod("", paymentMethod:=acct.paymentMethods(0), updateScopeOnAccount:=UpdateScope.AllActive, updateBehavior:=PaymentUpdateBehavior.Update, ignoreAvsPolicy:=True, ignoreAvsPolicySpecified:=True, ignoreCvnPolicy:=True, ignoreCvnPolicySpecified:=True, updateScopeOnChildren:=UpdateScope.None, validated:=validated, successes:=successes, failures:=failures)

            LogReturn("Account.updatePaymentMethod() -- UpdateActivePaymentMethod", ret, acct.merchantAccountId)
            Return ret.returnCode = 200
        End Function

        Public Function GetAutobills(ByVal Builder As BuilderRow) As Vindicia.AutoBill()

            Dim acct As Vindicia.Account = GetVindiciaAccount(Builder)

            If acct Is Nothing Then
                Return Nothing
            End If

            Dim client As New Vindicia.AutoBill

            InitializeEnvironment()

            Dim out As Vindicia.AutoBill()

            Dim ret As Vindicia.Return = client.fetchByAccount("", acct, False, False, 1, False, 1, False, out)
            LogReturn("AutoBill.fetchByAccount() -- GetAutobills", ret, Builder.HistoricID)
            Return out
        End Function

        Public Function CancelAutoBill(ByVal AutoBillVid As String) As Boolean
            Dim client As New Vindicia.AutoBill
            InitializeEnvironment()
            client.VID = AutoBillVid
            Dim transaction() As Vindicia.Transaction = Nothing
            Dim refunds() As Vindicia.Refund = Nothing
            Dim returnVal As Boolean = True




            Dim ret As Vindicia.Return = client.cancel("", False, True, False, False, False, True, "106", transaction, refunds)
            LogReturn("AutoBill.update() -- UpdateAutoBills", ret)
            returnVal = returnVal And (ret.returnCode = 200)
            Return returnVal
        End Function

        Public Function UpdateMerchantAffID(ByVal AutoBillVid As String, ByVal dbbuilder As BuilderRow, ByVal ab As Vindicia.AutoBill, ByVal AffliateID As String) As Boolean
            Dim client As New Vindicia.AutoBill
            InitializeEnvironment()
            client.VID = ab.VID
            client.merchantAffiliateId = AffliateID

            Dim returnVal As Boolean = True
            Dim ret As Vindicia.Return
            Dim acct As Vindicia.Account = GetVindiciaAccount(dbbuilder)
            client.account = acct

            ' Dim abs As Vindicia.AutoBill() = GetExistingAutoBills(acct)
            Dim atmo As Vindicia.AutoBillItemModification() = Nothing

            Dim transs As New Vindicia.Transaction


            Dim created As Boolean = Nothing
            Dim firstBillDate As DateTime = Nothing
            Dim firstBillAmount As String = Nothing
            Dim firstBillCurrency As String = Nothing
            Dim transactionStatus As Vindicia.TransactionStatus = Nothing
            Dim score As Integer = 0
            Dim scoreCodes() As Vindicia.ScoreCode = Nothing

            'ret = client.update(DuplicateBehavior.Fail, True, False, False, SysParam.GetValue(DB, "VindiciaFraudThreshold"), True, True, True, True, True, String.Empty, False, False, created, transactionStatus, firstBillDate, True, firstBillAmount, True, firstBillCurrency, score, False, scoreCodes)

            LogReturn("AutoBill.update() -- UpdateAutoBills", ret)
            returnVal = (ret.returnCode = 200)


            Return returnVal
        End Function

        Public Function UpdateAutoBills(ByVal abs() As Vindicia.AutoBill) As Boolean
            Dim client As New Vindicia.AutoBill
            InitializeEnvironment()


            Dim returnVal As Boolean = True

            For Each ab As Vindicia.AutoBill In abs
                'Dim ret As Vindicia.Return = client.update(GetAuth, ab, New Boolean, New DateTime, New Decimal, String.Empty, AllDataTypes.DuplicateBehavior.SucceedIgnore, False, SysParam.GetValue(DB, "VindiciaFraudThreshold"))
                Dim created As Boolean

                Dim firstBillDate As DateTime = Nothing
                Dim firstBillAmount As String = Nothing
                Dim firstBillCurrency As String = Nothing
                Dim transactionStatus As Vindicia.TransactionStatus = Nothing
                Dim score As Integer = 0
                Dim scoreCodes() As Vindicia.ScoreCode = Nothing
                'Dim ret As Vindicia.Return = ab.update(Vindicia.DuplicateBehavior.SucceedIgnore, True, False, False, SysParam.GetValue(DB, "VindiciaFraudThreshold"), True, True, False, True, False, String.Empty, False, False, created, transactionStatus, firstBillDate, True, firstBillAmount, True, firstBillCurrency, True, score, scoreCodes)

                'LogReturn("AutoBill.update() -- UpdateAutoBills", ret)
                'returnVal = returnVal And (ret.returnCode = 200)
            Next
            Return "" ' returnVal
        End Function

        Public Function GetTransactionHistory(ByVal Builder As BuilderRow) As Vindicia.Transaction()
            Dim client As New Vindicia.Transaction
            InitializeEnvironment()

            Dim out As New Generic.List(Of Vindicia.Transaction)

            'Dim autoBills() As Vindicia.AutoBill = GetAutobills(Builder)
            'If autoBills Is Nothing Then
            '    Return Nothing
            'End If

            Dim LatestAutoBill As Vindicia.AutoBill = GetAutobills(Builder).OrderBy(Function(obj) obj.endTimestamp).LastOrDefault()
            If LatestAutoBill Is Nothing Then
                Return Nothing
            End If

            'For Each ab As Vindicia.AutoBill In autoBills
            '    Dim temp As Vindicia.Transaction()
            '    'Dim ret As Vindicia.Return = client.fetchByAutobill(temp, GetAuth, ab)
            '    Dim ret As Vindicia.Return = client.fetchByAutobill(ab, temp)
            '    LogReturn("Transaction.fetchByAutobill() -- GetTransactionHistory()", ret, Builder.HistoricID)
            '    If ret.returnCode = 200 AndAlso temp IsNot Nothing Then
            '        out.AddRange(temp)
            '    End If
            'Next

            Dim temp As Vindicia.Transaction()
            Dim ret As Vindicia.Return = client.fetchByAutobill("", LatestAutoBill, temp)
            LogReturn("Transaction.fetchByAutobill() -- GetTransactionHistory()", ret, Builder.HistoricID)
            If ret.returnCode = 200 AndAlso temp IsNot Nothing Then
                out.AddRange(temp)
            End If

            Return out.ToArray

        End Function

    End Class

    Public Class VindiciaAddressInfo
        Public Guid As String
        Public Company As String
        Public FirstName As String
        Public LastName As String
        Public AddressLn1 As String
        Public AddressLn2 As String
        Public City As String
        Public State As String
        Public Zip As String
        Public County As String
        Public Country As String
        Public Email As String
    End Class

End Namespace

