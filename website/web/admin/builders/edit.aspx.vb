Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components
Imports Vindicia
Imports sforce
Imports Utilities

Public Class Edit
    Inherits AdminPage

    Protected BuilderID As Integer
    Protected p As VindiciaPaymentProcessor

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BUILDERS")
        p = New VindiciaPaymentProcessor(DB)
        p.IsTestMode = DataLayer.SysParam.GetValue(DB, "TestMode")
        BuilderID = Convert.ToInt32(Request("BuilderID"))
        If Not IsPostBack Then
            'Populate Registration Plans
            F_RegBillingPlan.DataSource = BillingPlanRow.GetActiveBillingPlans(DB, 1, "SortValue")
            F_RegBillingPlan.DataValueField = "BillingPlanId"
            F_RegBillingPlan.DataTextField = "DisplayValue"
            F_RegBillingPlan.DataBind()
            F_RegBillingPlan.Items.Insert(0, New ListItem("-- SELECT --", ""))

            'Populate Subscription Plans
            F_SubBillingPlan.DataSource = BillingPlanRow.GetActiveBillingPlans(DB, 2, "SortValue")
            F_SubBillingPlan.DataValueField = "BillingPlanId"
            F_SubBillingPlan.DataTextField = "DisplayValue"
            F_SubBillingPlan.DataBind()
            F_SubBillingPlan.Items.Insert(0, New ListItem("-- SELECT --", ""))

            '-----------------------------------------------------------
            'Populate Free Trial Period dropdown
            ddlFreeTrialPeriod.Items.Clear()

            Dim liFreeTrialPeriod0 As New ListItem("None", 0)
            ddlFreeTrialPeriod.Items.Add(liFreeTrialPeriod0)

            Dim liFreeTrialPeriod1 As New ListItem("1 month", 1)
            ddlFreeTrialPeriod.Items.Add(liFreeTrialPeriod1)

            Dim liFreeTrialPeriod3 As New ListItem("3 months", 3)
            ddlFreeTrialPeriod.Items.Add(liFreeTrialPeriod3)

            Dim liFreeTrialPeriod6 As New ListItem("6 months", 6)
            ddlFreeTrialPeriod.Items.Add(liFreeTrialPeriod6)

            Dim liFreeTrialPeriod12 As New ListItem("12 months", 12)
            ddlFreeTrialPeriod.Items.Add(liFreeTrialPeriod12)
            '---------------------------------------------------------------

            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        drpLLC.DataSource = LLCRow.GetList(DB, "LLC")
        drpLLC.DataTextField = "LLC"
        drpLLC.DataValueField = "LLCID"
        drpLLC.DataBind()
        drpLLC.Items.Insert(0, New ListItem("", ""))

        drpRegBillingPlanExternalKey.DataSource = BillingPlanRow.GetList(DB)
        drpRegBillingPlanExternalKey.DataTextField = "ExternalKey"
        drpRegBillingPlanExternalKey.DataValueField = "BillingPlanID"
        drpRegBillingPlanExternalKey.DataBind()
        drpRegBillingPlanExternalKey.Items.Insert(0, New ListItem("", ""))

        drpSubBillingPlanExternalKey.DataSource = BillingPlanRow.GetList(DB)
        drpSubBillingPlanExternalKey.DataTextField = "ExternalKey"
        drpSubBillingPlanExternalKey.DataValueField = "BillingPlanID"
        drpSubBillingPlanExternalKey.DataBind()
        drpSubBillingPlanExternalKey.Items.Insert(0, New ListItem("", ""))

        drpLLCRegBillingPlan.DataSource = LLCRow.GetList(DB, "LLC")
        drpLLCRegBillingPlan.DataTextField = "RegBillingPlanID"
        drpLLCRegBillingPlan.DataValueField = "LLCID"
        drpLLCRegBillingPlan.DataBind()
        drpLLCRegBillingPlan.Items.Insert(0, New ListItem("", ""))

        drpLLCSubBillingPlan.DataSource = LLCRow.GetList(DB, "LLC")
        drpLLCSubBillingPlan.DataTextField = "SubBillingPlanID"
        drpLLCSubBillingPlan.DataValueField = "LLCID"
        drpLLCSubBillingPlan.DataBind()
        drpLLCSubBillingPlan.Items.Insert(0, New ListItem("", ""))

        drpLLCBillingStartDate.DataSource = LLCRow.GetList(DB, "LLC")
        drpLLCBillingStartDate.DataTextField = "BillingStartDate"
        drpLLCBillingStartDate.DataValueField = "LLCID"
        drpLLCBillingStartDate.DataBind()
        drpLLCBillingStartDate.Items.Insert(0, New ListItem("", ""))

        drpRegistrationStatusID.DataSource = RegistrationStatusRow.GetList(DB)
        drpRegistrationStatusID.DataValueField = "RegistrationStatusID"
        drpRegistrationStatusID.DataTextField = "RegistrationStatus"
        drpRegistrationStatusID.DataBind()
        drpRegistrationStatusID.Items.Insert(0, New ListItem("", ""))

        drpPreferredVendorID.DataSource = VendorRow.GetList(DB)
        drpPreferredVendorID.DataValueField = "VendorID"
        drpPreferredVendorID.DataTextField = "CompanyName"
        drpPreferredVendorID.DataBind()
        drpPreferredVendorID.Items.Insert(0, New ListItem("", ""))

        drpState.DataSource = StateRow.GetStateList(DB)
        drpState.DataTextField = "StateName"
        drpState.DataValueField = "StateCode"
        drpState.DataBind()
        drpState.Items.Insert(0, New ListItem("", ""))

        If BuilderID = 0 Then
            txtBillingStartDate.Visible = False

            ddlFreeTrialPeriod.Items(0).Enabled = False                'Remove "None" option from Trial Period dropdown

            dtpBillingStartDate.Value = Now.AddDays(1).Date            'Set next day as default Billing Start Date
            txtSubscriptionStartDate.Text = Now.AddDays(1).AddMonths(1).Date        'Set next day as default Subscription Start Date

            btnDelete.Visible = False
            Exit Sub
        End If

        If BuilderID > 0 Then
            drpLLC.Enabled = False
            btnSave.Message = Nothing
        End If

        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, BuilderID)
        txtCRMID.Text = dbBuilder.CRMID
        txtCRMID.Enabled = False
        drpLLC.SelectedValue = dbBuilder.LLCID

        'POPULATE BILLING PLAN BASED ON LLC
        drpLLCRegBillingPlan.SelectedValue = dbBuilder.LLCID
        drpLLCSubBillingPlan.SelectedValue = dbBuilder.LLCID
        drpLLCBillingStartDate.SelectedValue = dbBuilder.LLCID
        F_RegBillingPlan.SelectedValue = drpLLCRegBillingPlan.SelectedItem.Text
        F_SubBillingPlan.SelectedValue = drpLLCSubBillingPlan.SelectedItem.Text
        drpRegBillingPlanExternalKey.SelectedValue = drpLLCRegBillingPlan.SelectedItem.Text
        drpSubBillingPlanExternalKey.SelectedValue = drpLLCSubBillingPlan.SelectedItem.Text
        'dtpBillingStartDate.Value = CDate(drpLLCBillingStartDate.SelectedItem.Text).Date

        txtCompanyName.Text = dbBuilder.CompanyName
        txtAlias.Text = dbBuilder.CompanyAlias
        txtAddress.Text = dbBuilder.Address
        txtAddress2.Text = dbBuilder.Address2
        txtCity.Text = dbBuilder.City
        drpState.SelectedValue = dbBuilder.State
        txtZip.Text = dbBuilder.Zip
        txtPhone.Text = dbBuilder.Phone
        txtMobile.Text = dbBuilder.Mobile
        txtFax.Text = dbBuilder.Fax
        txtEmail.Text = dbBuilder.Email
        txtWebsiteURL.Text = dbBuilder.WebsiteURL
        drpRegistrationStatusID.SelectedValue = dbBuilder.RegistrationStatusID
        drpPreferredVendorID.SelectedValue = dbBuilder.PreferredVendorID
        rblIsActive.SelectedValue = dbBuilder.IsActive
        rblIsNew.SelectedValue = dbBuilder.IsNew
        rblSkipEntitlementCheck.SelectedValue = dbBuilder.SkipEntitlementCheck
        rblIsPlansOnline.SelectedValue = dbBuilder.IsPlansOnline
        rblDocumentAccess.SelectedValue = dbBuilder.HasDocumentsAccess
        rblQuarterlyReportingOn.SelectedValue = dbBuilder.QuarterlyReportingOn

        If rblIsNew.SelectedValue = "True" Then
            ddlFreeTrialPeriod.Items(0).Enabled = False
        End If

        Dim dbBuilderBillingPlan As BuilderBillingPlanRow = BuilderBillingPlanRow.GetRow(DB, BuilderID)

        If dbBuilderBillingPlan.BuilderBillingPlanId > 0 Then
            F_RegBillingPlan.SelectedValue = dbBuilderBillingPlan.RegBillingPlanId
            F_SubBillingPlan.SelectedValue = dbBuilderBillingPlan.SubBillingPlanId

            drpRegBillingPlanExternalKey.SelectedValue = dbBuilderBillingPlan.RegBillingPlanId
            drpSubBillingPlanExternalKey.SelectedValue = dbBuilderBillingPlan.SubBillingPlanId

            ddlFreeTrialPeriod.SelectedValue = dbBuilderBillingPlan.FreeTrialPeriod

            dtpBillingStartDate.Value = dbBuilderBillingPlan.BillingStartDate
            txtSubscriptionStartDate.Text = dbBuilderBillingPlan.SubscriptionStartDate.Date
            txtBillingStartDate.Text = dbBuilderBillingPlan.BillingStartDate.Date

            txtBillingLastSuccess.Text = dbBuilderBillingPlan.BillingLastSuccess
            txtBillingSubscriptionAutobill.Text = dbBuilderBillingPlan.BillingSubscriptionAutobill
            txtBillingMembershipAutobill.Text = dbBuilderBillingPlan.BillingMembershipAutobill
            txtBillingProcessorID.Text = dbBuilderBillingPlan.BillingProcessorId

            hdnPrevRegBillingPlan.Value = dbBuilderBillingPlan.RegBillingPlanId
            hdnPrevBillingStartDate.Value = dbBuilderBillingPlan.BillingStartDate
        Else
            dtpBillingStartDate.Value = Now.Date
            txtSubscriptionStartDate.Text = Now.Date
        End If

        PopulateVindiciaBillingDetails()

        If dbBuilder.IsNew = False Then
            dtpBillingStartDate.Visible = False
            ddlFreeTrialPeriod.Enabled = False
            txtBillingStartDate.Visible = True
        Else
            txtBillingStartDate.Visible = False
        End If

    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim UpdatedAccount As Boolean = False

        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbBuilder As BuilderRow

            If BuilderID <> 0 Then
                dbBuilder = BuilderRow.GetRow(DB, BuilderID)
            Else
                dbBuilder = New BuilderRow(DB)
            End If
            dbBuilder.CRMID = txtCRMID.Text
            If drpLLC.SelectedValue <> String.Empty Then
                dbBuilder.LLCID = drpLLC.SelectedValue
            End If
            dbBuilder.CompanyName = txtCompanyName.Text
            dbBuilder.CompanyAlias = txtAlias.Text
            dbBuilder.Address = txtAddress.Text
            dbBuilder.Address2 = txtAddress2.Text
            dbBuilder.City = txtCity.Text
            dbBuilder.State = drpState.SelectedValue
            dbBuilder.Zip = txtZip.Text
            dbBuilder.Phone = txtPhone.Text
            dbBuilder.Mobile = txtMobile.Text
            dbBuilder.Fax = txtFax.Text
            dbBuilder.Email = txtEmail.Text
            dbBuilder.WebsiteURL = txtWebsiteURL.Text
            Dim dbStat As RegistrationStatusRow = RegistrationStatusRow.GetStatusByStep(DB, 1)
            dbBuilder.RegistrationStatusID = dbStat.RegistrationStatusID
            If drpPreferredVendorID.SelectedValue <> String.Empty Then
                dbBuilder.PreferredVendorID = drpPreferredVendorID.SelectedValue
            End If

            dbBuilder.IsActive = rblIsActive.SelectedValue
            dbBuilder.IsNew = rblIsNew.SelectedValue
            dbBuilder.SkipEntitlementCheck = rblSkipEntitlementCheck.SelectedValue
            dbBuilder.IsPlansOnline = rblIsPlansOnline.SelectedValue
            dbBuilder.HasDocumentsAccess = rblDocumentAccess.SelectedValue
            dbBuilder.QuarterlyReportingOn = rblQuarterlyReportingOn.SelectedValue

            dbBuilder.Updated = Now()

            If BuilderID <> 0 Then
                UpdatedAccount = True
                dbBuilder.Update()
                If p.UpdateVindiciaAddress(dbBuilder) Then
                    Logger.Info("Account updated")
                End If
            Else
                dbBuilder.Submitted = Now()
                BuilderID = dbBuilder.Insert
            End If

            '----------- BILLING PLAN -----------
            Dim dbBuilderBillingPlan As BuilderBillingPlanRow
            dbBuilderBillingPlan = BuilderBillingPlanRow.GetRow(DB, BuilderID)

            If dbBuilderBillingPlan.BuilderBillingPlanId = 0 Then
                dbBuilderBillingPlan = New BuilderBillingPlanRow(DB, BuilderID)
            End If

            dbBuilderBillingPlan.BuilderId = BuilderID

            If F_RegBillingPlan.SelectedValue = 0 Then
                dbBuilderBillingPlan.RegBillingPlanId = BillingPlanRow.GetDefaultBillingPlan(DB, BILLING_PLAN_TYPE.REGISTRATION).BillingPlanId
            Else
                dbBuilderBillingPlan.RegBillingPlanId = CInt(F_RegBillingPlan.SelectedValue)
            End If

            If F_SubBillingPlan.SelectedValue = 0 Then
                dbBuilderBillingPlan.SubBillingPlanId = BillingPlanRow.GetDefaultBillingPlan(DB, BILLING_PLAN_TYPE.SUBSCRIPTION).BillingPlanId
            Else
                dbBuilderBillingPlan.SubBillingPlanId = CInt(F_SubBillingPlan.SelectedValue)
            End If

            dbBuilderBillingPlan.FreeTrialPeriod = CInt(ddlFreeTrialPeriod.SelectedValue)

            If dtpBillingStartDate.Value = New Date(1, 1, 1) Then
                dbBuilderBillingPlan.BillingStartDate = Now.Date
            Else
                dbBuilderBillingPlan.BillingStartDate = dtpBillingStartDate.Value
            End If

            If txtSubscriptionStartDate.Text = New Date(1, 1, 1) Then
                dbBuilderBillingPlan.SubscriptionStartDate = Now.Date
            Else
                dbBuilderBillingPlan.SubscriptionStartDate = DateAdd(DateInterval.Month, CInt(ddlFreeTrialPeriod.SelectedValue), dbBuilderBillingPlan.BillingStartDate)
            End If

            dbBuilderBillingPlan.BillingSubscriptionAutobill = txtBillingSubscriptionAutobill.Text
            dbBuilderBillingPlan.BillingMembershipAutobill = txtBillingMembershipAutobill.Text
            dbBuilderBillingPlan.BillingProcessorId = txtBillingProcessorID.Text

            If txtBillingLastSuccess.Text <> "" AndAlso IsDate(txtBillingLastSuccess.Text) Then
                dbBuilderBillingPlan.BillingLastSuccess = txtBillingLastSuccess.Text
            End If

            If dbBuilderBillingPlan.BuilderBillingPlanId = 0 Then
                dbBuilderBillingPlan.Insert()
            Else
                dbBuilderBillingPlan.Update()
            End If
            '------------------------------------

            DB.CommitTransaction()

            DB.ExecuteSQL("Delete from AdminDocumentBuilderRecipient Where BuilderID = " & BuilderID)

            If Not dbBuilder.IsActive Then
                DB.ExecuteSQL("Update  BuilderAccount Set IsActive = 0 Where BuilderID = " & DB.Number(BuilderID))
            End If

            '=================== Apala (Medullus) - Sales Force related code 11.08.2017 ===================
            '' Salesforce Integration
            'If UpdatedAccount Then
            '    Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
            '    If sfHelper.Login() Then
            '        If sfHelper.UpsertBuilder(BuilderRow.GetRow(DB, BuilderID)) = False Then
            '            'throw error
            '        End If
            '    End If
            'Else
            '    Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
            '    If sfHelper.Login() Then
            '        If sfHelper.InsertBuilder(BuilderRow.GetRow(DB, BuilderID)) = False Then
            '            'throw error
            '        End If
            '    End If
            'End If
            '===============================================================================================


            'Add this builder to existing documents AdminDocumentBuilderRecipient
            If dbBuilder.HasDocumentsAccess Then
                Dim sql As String = "select adat.AdminDocumentID , " & BuilderID & " AS BuilderID  from AdminDocumentDocumentAudienceType adat  Inner join  AdminDocument  ad " _
                            & " on adat.AdminDocumentID = ad.AdminDocumentID  left join AdminDocumentLLC adl on adl.AdminDocumentID = ad.AdminDocumentID  where adat.DocumentAudienceTypeid =   " _
                            & "(Select DocumentAudienceTypeid From documentaudiencetype where audiencetype = 'Builder') " _
                            & " AND adl.llcid = (Select LLCID From Builder  where BuilderID  = " & BuilderID & ") "
                Dim dt As DataTable = DB.GetDataTable(sql)
                ' setup bulk copy
                If dt.Rows.Count > 0 Then
                    Dim bc As New SqlClient.SqlBulkCopy(DB.Connection)
                    bc.ColumnMappings.Add("AdminDocumentID", "AdminDocumentID")
                    bc.ColumnMappings.Add("BuilderID", "BuilderID")
                    bc.DestinationTableName = "AdminDocumentBuilderRecipient"
                    bc.BulkCopyTimeout = 300
                    'Populate table
                    bc.WriteToServer(dt)
                    bc.Close()
                End If


            End If


            'Dim ResultResponse = ""
            'Dim WebMethod = ""
            'Dim objectError = ""
            'Dim requestApiTimestamp As DateTime = DateTime.Now
            'Dim userIp As String = HttpContext.Current.Request.UserHostAddress
            'Try
            '    If UpdatedAccount = True Then
            '        Dim mSBuilderAccount As SBuilderAccount = New SBuilderAccount()
            '        mSBuilderAccount.fill(DB, dbBuilder)
            '        objectError = mSBuilderAccount.ToString()
            '        Dim sf As SalesForce.SalesForce = New SalesForce.SalesForce(DB)

            '        ' login
            '        WebMethod = "Login"
            '        If sf.login() Then
            '            ResultResponse = "SUCCESS"
            '        Else
            '            ResultResponse = "FAIL"
            '        End If

            '        Dim dbSalesforceApiLog As SalesforceApiLogRow = New SalesforceApiLogRow(DB)
            '        dbSalesforceApiLog.WebMethod = WebMethod
            '        dbSalesforceApiLog.UserHostAddress = userIp
            '        dbSalesforceApiLog.RequestTimestamp = requestApiTimestamp
            '        dbSalesforceApiLog.RequestMessage = mSBuilderAccount.ToString()
            '        dbSalesforceApiLog.ResponseTimestamp = DateTime.Now
            '        dbSalesforceApiLog.ResponseMessage = ResultResponse
            '        dbSalesforceApiLog.Service = "OUT"
            '        dbSalesforceApiLog.Insert()

            '        If ResultResponse = "SUCCESS" Then
            '            WebMethod = "upsert: Update Builder Account"
            '            requestApiTimestamp = DateTime.Now
            '            Dim sObjects() As sObject = New sObject() {mSBuilderAccount.getsObject()}
            '            sf.upsert(sObjects, "Portal_Account_Id__c")
            '            dbSalesforceApiLog = New SalesforceApiLogRow(DB)
            '            dbSalesforceApiLog.WebMethod = WebMethod
            '            dbSalesforceApiLog.UserHostAddress = userIp
            '            dbSalesforceApiLog.RequestTimestamp = requestApiTimestamp
            '            dbSalesforceApiLog.RequestMessage = mSBuilderAccount.ToString()
            '            dbSalesforceApiLog.ResponseTimestamp = DateTime.Now
            '            dbSalesforceApiLog.ResponseMessage = ResultResponse
            '            dbSalesforceApiLog.Service = "OUT"
            '            dbSalesforceApiLog.Insert()
            '            If UpdatedContact = True Then
            '                WebMethod = "upsert: Update Builder Contact"
            '                requestApiTimestamp = DateTime.Now
            '                Dim mSBuilderContact As SBuilderContact = New SBuilderContact()
            '                mSBuilderContact.fill(DB, dbBuilderAccount)
            '                mSBuilderContact.Portal_Account_Id__c = mSBuilderAccount.Portal_Account_Id__c
            '                objectError = mSBuilderContact.ToString()
            '                sObjects = New sObject() {mSBuilderContact.getsObject()}
            '                sf.upsert(sObjects, "Portal_Contact_Id__c")
            '                dbSalesforceApiLog = New SalesforceApiLogRow(DB)
            '                dbSalesforceApiLog.WebMethod = WebMethod
            '                dbSalesforceApiLog.UserHostAddress = userIp
            '                dbSalesforceApiLog.RequestTimestamp = requestApiTimestamp
            '                dbSalesforceApiLog.RequestMessage = mSBuilderContact.ToString()
            '                dbSalesforceApiLog.ResponseTimestamp = DateTime.Now
            '                dbSalesforceApiLog.ResponseMessage = ResultResponse
            '                dbSalesforceApiLog.Service = "OUT"
            '                dbSalesforceApiLog.Insert()

            '            End If
            '        End If
            '    End If
            'Catch ex As Exception
            '    ResultResponse = "FAIL :" & ex.Message
            '    Dim dbSalesforceApiLog As SalesforceApiLogRow = New SalesforceApiLogRow(DB)
            '    dbSalesforceApiLog.WebMethod = WebMethod
            '    dbSalesforceApiLog.UserHostAddress = userIp
            '    dbSalesforceApiLog.RequestTimestamp = requestApiTimestamp
            '    dbSalesforceApiLog.RequestMessage = objectError
            '    dbSalesforceApiLog.ResponseTimestamp = DateTime.Now
            '    dbSalesforceApiLog.ResponseMessage = ResultResponse
            '    dbSalesforceApiLog.Service = "OUT"
            '    dbSalesforceApiLog.Insert()
            'End Try

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Private Sub btnUpgradeBillingPlan_Click(sender As Object, e As EventArgs) Handles btnUpgradeBillingPlan.Click

        '---------------------------------------------------------------------------------------------------------------------------------------------------------------
        Dim dbBuilderBillingPlan As BuilderBillingPlanRow
        dbBuilderBillingPlan = BuilderBillingPlanRow.GetRow(DB, BuilderID)

        If dbBuilderBillingPlan.BuilderBillingPlanId = 0 Then
            dbBuilderBillingPlan = New BuilderBillingPlanRow(DB, BuilderID)
        End If

        dbBuilderBillingPlan.BuilderId = BuilderID

        If F_RegBillingPlan.SelectedValue = 0 Then
            dbBuilderBillingPlan.RegBillingPlanId = BillingPlanRow.GetDefaultBillingPlan(DB, BILLING_PLAN_TYPE.REGISTRATION).BillingPlanId
        Else
            dbBuilderBillingPlan.RegBillingPlanId = CInt(F_RegBillingPlan.SelectedValue)
        End If

        If F_SubBillingPlan.SelectedValue = 0 Then
            dbBuilderBillingPlan.SubBillingPlanId = BillingPlanRow.GetDefaultBillingPlan(DB, BILLING_PLAN_TYPE.SUBSCRIPTION).BillingPlanId
        Else
            dbBuilderBillingPlan.SubBillingPlanId = CInt(F_SubBillingPlan.SelectedValue)
        End If

        dbBuilderBillingPlan.FreeTrialPeriod = CInt(ddlFreeTrialPeriod.SelectedValue)

        If dtpBillingStartDate.Value = New Date(1, 1, 1) Then
            dbBuilderBillingPlan.BillingStartDate = Now.Date
        Else
            dbBuilderBillingPlan.BillingStartDate = dtpBillingStartDate.Value
        End If

        If txtSubscriptionStartDate.Text = New Date(1, 1, 1) Then
            dbBuilderBillingPlan.SubscriptionStartDate = Now.Date
        Else
            dbBuilderBillingPlan.SubscriptionStartDate = DateAdd(DateInterval.Month, CInt(ddlFreeTrialPeriod.SelectedValue), dbBuilderBillingPlan.BillingStartDate)
        End If

        dbBuilderBillingPlan.BillingSubscriptionAutobill = txtBillingSubscriptionAutobill.Text
        dbBuilderBillingPlan.BillingMembershipAutobill = txtBillingMembershipAutobill.Text
        dbBuilderBillingPlan.BillingProcessorId = txtBillingProcessorID.Text

        If txtBillingLastSuccess.Text <> "" AndAlso IsDate(txtBillingLastSuccess.Text) Then
            dbBuilderBillingPlan.BillingLastSuccess = txtBillingLastSuccess.Text
        End If

        If dbBuilderBillingPlan.BuilderBillingPlanId = 0 Then
            dbBuilderBillingPlan.Insert()
        Else
            dbBuilderBillingPlan.Update()
        End If
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        '------------- UPDATE BUILDER AUTOBILL ---------------
        If BuilderID <> 0 Then
            UpdateBuilderAutoBill()
        End If
        '-----------------------------------------------------

        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?BuilderID=" & BuilderID & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnRegistrations_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRegistrations.Click
        Response.Redirect("/admin/builders/builderregistration/default.aspx?F_BuilderID=" & BuilderID & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub PopulateVindiciaBillingDetails()
        p = New VindiciaPaymentProcessor(DB)
        p.IsTestMode = DataLayer.SysParam.GetValue(DB, "TestMode")

        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, BuilderID)
        Dim autoBills() As Vindicia.AutoBill = p.GetAutobills(dbBuilder)

        If Not autoBills Is Nothing Then
            For Each ab As Vindicia.AutoBill In autoBills
                Dim Product As String = ab.items(0).product.merchantProductId
                If Product = "Registration" Then
                    txtBillingProcessorID.Text = ab.account.merchantAccountId
                    txtBillingMembershipAutobill.Text = ab.merchantAutoBillId
                ElseIf Product = "Subscription" Then
                    If ab.status = Vindicia.AutoBillStatus.Active Then
                        txtBillingSubscriptionAutobill.Text = ab.merchantAutoBillId
                        txtBillingProcessorID.Text = ab.account.merchantAccountId

                        Dim tran As New Vindicia.Transaction
                        Dim trans As Vindicia.Transaction()

                        Dim srd As String = "'{""transactions"": [""VID"", ""merchantTransactionId"", ""amount"", ""timestamp""]}'"
                        tran.fetchByAccount(srd, ab.account, False, True, 0, True, 10, True, trans)

                        txtBillingLastSuccess.Text = trans(0).originalActivityDate.Date.ToShortDateString()
                    End If
                End If
            Next
        End If

    End Sub

    '------------- UPDATE BUILDER AUTOBILL ---------------
    Private Sub UpdateBuilderAutoBill()

        p = New VindiciaPaymentProcessor(DB)
        p.IsTestMode = DataLayer.SysParam.GetValue(DB, "TestMode")

        Dim VinBillingPlans As Vindicia.BillingPlan() = p.GetBillingPlansForAdmin()
        Dim selectedBillingPlan As New Vindicia.BillingPlan

        For Each bp As BillingPlan In VinBillingPlans
            If bp.status = BillingPlanStatus.Active Then
                If bp.merchantBillingPlanId = drpSubBillingPlanExternalKey.SelectedItem.Text Then
                    selectedBillingPlan = bp
                End If
            End If
        Next

        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, BuilderID)
        Dim autoBills() As Vindicia.AutoBill = p.GetAutobills(dbBuilder)

        If Not autoBills Is Nothing Then
            For Each ab As Vindicia.AutoBill In autoBills
                Dim Product As String = ab.items(0).product.merchantProductId
                If Product = "Subscription" Then
                    If ab.status = Vindicia.AutoBillStatus.Active Or ab.status = Vindicia.AutoBillStatus.PendingActivation Then
                        Dim Tran As Vindicia.Transaction = Nothing
                        Dim Ref As Vindicia.Refund() = Nothing

                        Dim arrABIM() As Vindicia.AutoBillItemModification = Nothing
                        ReDim arrABIM(0)
                        Dim iItemCounter As Int16 = 0

                        Dim sitem As New Vindicia.AutoBillItem

                        If ab.items.Length > 0 Then
                            For Each itm As Vindicia.AutoBillItem In ab.items
                                Dim objAuBiItMod As New AutoBillItemModification()

                                sitem = itm

                                objAuBiItMod.removeAutoBillItem = sitem

                                If CDate(txtSubscriptionStartDate.Text).Date <= Now.Date Then
                                    sitem.startDate = Now.Date.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'")
                                Else
                                    sitem.startDate = CDate(txtSubscriptionStartDate.Text).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'")
                                End If

                                objAuBiItMod.removeAutoBillItem = itm
                                objAuBiItMod.addAutoBillItem = sitem

                                arrABIM(iItemCounter) = objAuBiItMod

                                iItemCounter = iItemCounter + 1

                                Exit For
                            Next
                        Else
                            If CDate(txtSubscriptionStartDate.Text).Date <= Now.Date Then
                                sitem.startDate = Now.Date.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'")
                            Else
                                sitem.startDate = CDate(txtSubscriptionStartDate.Text).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'")
                            End If

                            Dim objAuBiItMod As New AutoBillItemModification()
                            objAuBiItMod.addAutoBillItem = sitem

                            arrABIM(0) = objAuBiItMod
                        End If

                        Dim strMessageBody As String = "Billing Plan Upgraded for "
                        Dim ret As Vindicia.Return
                        Try
                            '****************************** FOR CHANGING BILLING DAY OF THE MONTH ***************************
                            'ab.billingDay = CDate(txtSubscriptionStartDate.Text).Day
                            'ret = ab.changeBillingDayOfMonth(CDate(txtSubscriptionStartDate.Text).Day, Now.Date, False, Nothing, False, "")
                            '************************************************************************************************

                            ret = ab.modify("", False, "nextBill", selectedBillingPlan, arrABIM, False, False, Tran, sitem.startDate, True, Ref)

                            Dim strLogValue As String = String.Concat("UpdateBuilderAutoBill() -- ", ab.merchantAutoBillId, " | ", selectedBillingPlan.merchantBillingPlanId, " | ", sitem.startDate)
                            p.LogReturn(strLogValue, ret, dbBuilder.HistoricID)

                            strMessageBody = String.Concat(strMessageBody, dbBuilder.HistoricID.ToString(), " to ", selectedBillingPlan.merchantBillingPlanId, " | Result: ", ret.returnString, " | Upgraded by : ", Session("AdminId").ToString())
                        Catch ex As Exception
                            strMessageBody = String.Concat(strMessageBody, dbBuilder.HistoricID.ToString(), " to ", selectedBillingPlan.merchantBillingPlanId, " | Error: ", ex.Message, " | Upgraded by : ", Session("AdminId").ToString())
                        End Try

                        NotifyDevTeamOfBillingPlanUpgrade(strMessageBody)
                    End If
                End If
            Next
        End If

    End Sub

    Private Sub NotifyDevTeamOfBillingPlanUpgrade(ByVal strMessageDetails As String)
        Core.SendSimpleMail("customerservice@cbusa.com", "customerservice@cbusa.com", "abasu@medullus.com", "abasu@medullus.com", "Billing Plan Upgraded for " & BuilderID.ToString(), strMessageDetails)
    End Sub

End Class
