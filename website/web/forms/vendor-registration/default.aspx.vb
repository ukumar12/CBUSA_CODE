Imports Components
Imports DataLayer
Imports Utilities

Partial Class forms_vendor_registration_default
    Inherits SitePage

    Protected VendorId As Integer
    Protected VendorAccountId As Integer
    Protected dbVendor As VendorRow
    Protected dbVendorAccount As VendorAccountRow
    Protected dbVendorRegistration As VendorRegistrationRow
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Protected ReadOnly Property Guid() As String
        Get
            If Request("id") IsNot Nothing Then
                Return Request("id")
            End If
            Return String.Empty
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("VendorAccountId") IsNot Nothing Then
            VendorAccountId = CType(Session("VendorAccountId"), Integer)
            dbVendorAccount = VendorAccountRow.GetRow(Me.DB, VendorAccountId)
            dbVendor = VendorRow.GetRow(Me.DB, dbVendorAccount.VendorID)
        ElseIf Guid <> String.Empty Then
            dbVendor = VendorRow.GetVendorByGuid(DB, Guid)
            dbVendorAccount = New VendorAccountRow(DB)
        Else
            dbVendor = New VendorRow(DB)
            dbVendorAccount = New VendorAccountRow(DB)
        End If

        If dbVendorAccount.VendorAccountID = Nothing Then
            Response.Redirect(GlobalSecureName & "/default.aspx")
        End If

        dbVendorRegistration = VendorRegistrationRow.GetRowByVendor(DB, dbVendorAccount.VendorID)
        If dbVendorRegistration.VendorRegistrationID = Nothing OrElse (dbVendorRegistration.CompleteDate <> Nothing AndAlso dbVendorRegistration.CompleteDate.Year <> Now.Year) Then
            dbVendorRegistration = New VendorRegistrationRow(DB)
            dbVendorRegistration.VendorID = dbVendorAccount.VendorID
            dbVendorRegistration.HistoricVendorID = dbVendor.HistoricVendorID
            dbVendorRegistration.PreparerFirstName = " "
            dbVendorRegistration.PreparerLastName = " "
            dbVendorRegistration.Employees = 0
            dbVendorRegistration.YearStarted = 0
            dbVendorRegistration.SubsidiaryExplanation = " "
            dbVendorRegistration.SupplyArea = " "
            dbVendorRegistration.IsSubsidiary = False
            dbVendorRegistration.BusinessType = 0
            dbVendorRegistration.ProductsOffered = " "
            dbVendorRegistration.RegistrationStatusID = -1
            dbVendorRegistration.Insert()
        End If

        If dbVendorRegistration IsNot Nothing Then
            Dim dbStatus As RegistrationStatusRow = RegistrationStatusRow.GetRow(DB, dbVendorRegistration.RegistrationStatusID)
            If dbVendorRegistration.CompleteDate <> Nothing AndAlso dbVendorRegistration.CompleteDate.Year = Now.Year Then
                ctlSteps.Visible = False
                btnContinueElectronic.Visible = False
                btnCancel.Visible = True
            Else
                btnDashboard.Visible = False
                btnCancel.Visible = False
                If Request("redir") Is Nothing OrElse Request("redir") <> False Then
                    Select Case dbStatus.RegistrationStep
                        Case 1
                            Response.Redirect("register.aspx")
                        Case 2
                            Response.Redirect("users.aspx")
                        Case 3
                            Response.Redirect("/rebates/terms.aspx")
                        Case 4
                            Response.Redirect("supplyphase.aspx")
                    End Select
                End If
            End If
        End If

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorId")
        UserName = Session("Username")

        If Not IsPostBack Then
            LoadFromDB()
        Core.DataLog("Edit Account Information", PageURL, CurrentUserId, "Vendor Left Menu Click","", "", "", "", UserName)
        End If

    End Sub

    Private Sub LoadFromDB()
        Dim dt As DataTable = StateRow.GetStateList(DB)

        drpState.DataSource = dt
        drpState.DataTextField = "StateName"
        drpState.DataValueField = "StateCode"
        drpState.DataBind()
        drpState.Items.Insert(0, New ListItem("", ""))

        drpBillingState.DataSource = dt
        drpBillingState.DataTextField = "StateName"
        drpBillingState.DataValueField = "StateCode"
        drpBillingState.DataBind()
        drpBillingState.Items.Insert(0, New ListItem("", ""))

        If dbVendorAccount.VendorAccountID = 0 Then
            rfvtxtPassword.Enabled = True
            Exit Sub
        Else
            rfvtxtPassword.Enabled = dbVendorAccount.RequirePasswordUpdate Or dbVendorAccount.Password = Nothing Or Not dbVendorAccount.PasswordEncrypted
        End If

        With dbVendorAccount
            txtFirstName.Text = .FirstName
            txtLastName.Text = .LastName
            txtEmail.Text = .Email
            txtFax.Text = .Fax
            txtMobile.Text = .Mobile
            txtPhone.Text = .Phone
            txtTitle.Text = .Title
            txtUsername.Text = .Username
            txtAccountMobile.Text = .Mobile
            txtAccountPhone.Text = .Phone
            txtAccountFax.Text = .Fax
        End With

        With dbVendor
            txtCompanyName.Text = .CompanyName
            txtAddress.Text = .Address
            txtAddress2.Text = .Address2
            txtCity.Text = .City
            drpState.SelectedValue = .State
            txtZip.Text = .Zip
            txtPhone.Text = .Phone
            'txtMobile.Text = .Mobile
            txtFax.Text = .Fax
            'txtEmail.Text = .Email
            txtBillingAddress.Text = .BillingAddress
            txtBillingCity.Text = .BillingCity
            drpBillingState.SelectedValue = .BillingState
            txtBillingZip.Text = .BillingZip
        End With
    End Sub


    Protected Function Register() As Boolean
        Page.Validate("VendorReg")
        If Not Page.IsValid Then Exit Function
        Dim UpdatedContact As Boolean = False
        Dim UpdatedAccount As Boolean = False
        Try
            DB.BeginTransaction()

            DB.ExecuteSQL("update VendorAccount set IsPrimary=0 where VendorID=" & DB.Number(VendorId))

            With dbVendor
                .CompanyName = txtCompanyName.Text
                .Address = txtAddress.Text
                .Address2 = txtAddress2.Text
                .City = txtCity.Text
                .State = drpState.SelectedValue
                .Zip = txtZip.Text
                .Phone = txtPhone.Text
                .Fax = txtFax.Text
                .Mobile = txtMobile.Text

                .Email = txtEmail.Text
                .BillingAddress = txtBillingAddress.Text
                .BillingCity = txtBillingCity.Text
                .BillingState = drpBillingState.SelectedValue
                .BillingZip = txtBillingZip.Text

                If .GUID = Nothing Then
                    .GUID = Core.GenerateFileID()
                End If
                If .HistoricID = Nothing Then
                    .HistoricID = VendorRow.GetNextHistoricID(DB)
                End If
            End With

            If dbVendor.VendorID = 0 Then
                dbVendor.Insert()
                'log Add Account Information
                Core.DataLog("Edit Account Information", PageURL, CurrentUserId, "Insert Account Information", "", "", "", "", UserName)
                'end log
            Else
                dbVendor.Update()
                'log Add Account Information
                Core.DataLog("Edit Account Information", PageURL, CurrentUserId, "Update Account Information", "", "", "", "", UserName)
                'end log
                UpdatedAccount = True
            End If

            With dbVendorAccount
                .FirstName = txtFirstName.Text
                .LastName = txtLastName.Text
                .Username = txtUsername.Text
                If txtPassword.Text <> Nothing Then
                    .Password = txtPassword.Text
                    .RequirePasswordUpdate = False
                End If
                .Phone = txtAccountPhone.Text
                .Fax = txtAccountFax.Text
                .Email = txtEmail.Text
                .Mobile = txtAccountMobile.Text
                .IsPrimary = True
                .VendorID = dbVendor.VendorID
            End With

            If dbVendorAccount.VendorAccountID = 0 Then
                dbVendorAccount.IsActive = True
                dbVendorAccount.Insert()
            Else
                dbVendorAccount.Update()
                UpdatedContact = True
            End If

            dbVendorRegistration = VendorRegistrationRow.GetRowByVendor(Me.DB, dbVendor.VendorID)

            Dim dbStat As RegistrationStatusRow = RegistrationStatusRow.GetStatusByStep(DB, 1)


            If dbVendorRegistration.VendorRegistrationID = Nothing Then
                With dbVendorRegistration
                    .YearStarted = 0
                    .Employees = 0
                    .IsSubsidiary = False
                    .SubsidiaryExplanation = String.Empty
                    .SupplyArea = " "
                    .BusinessType = 0
                    .PreparerFirstName = txtFirstName.Text
                    .PreparerLastName = txtLastName.Text
                    .AcceptsTerms = False
                    If .RegistrationStatusID = Nothing OrElse .CompleteDate = Nothing  Then
                        .RegistrationStatusID = dbStat.RegistrationStatusID
                    End If
                    .VendorID = dbVendor.VendorID

                    'no longer used?
                    .ProductsOffered = " "
                End With
                dbVendorRegistration.Insert()
            Else
                If dbVendorRegistration.RegistrationStatusID = Nothing Then
                    dbVendorRegistration.RegistrationStatusID = dbStat.RegistrationStatusID
                End If
                dbVendorRegistration.Update()
            End If

            Session("VendorId") = dbVendor.VendorID
            Session("VendorAccountId") = dbVendorAccount.VendorAccountID

            DB.CommitTransaction()


            '' Salesforce Integration
            'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
            'If sfHelper.Login() Then
            '    If UpdatedAccount Then
            '        If sfHelper.UpsertVendor(dbVendor) = False Then
            '            'throw error
            '        End If
            '    End If
            '    If UpdatedContact Then
            '        If sfHelper.UpsertVendorAccount(dbVendorAccount) = False Then
            '            'throw error
            '        End If
            '    Else
            '        If sfHelper.InsertVendorAccount(dbVendorAccount, dbVendor.CRMID) = False Then
            '            'throw error
            '        End If
            '    End If
            'End If

            Return True

        Catch ex As Exception
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
            Return False
        End Try
    End Function

    Protected Sub btnContinueElectronic_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnContinueElectronic.Click
        'log Btn Continue With Registration 
        Core.DataLog("Edit Account Information", PageURL, CurrentUserId, "Btn Continue With Registration", "", "", "", "", UserName)
        'end log
        If Register() Then
            Response.Redirect("companyprofile.aspx?guid=" & dbVendor.GUID)
        Else
            'Throw New Exception("There was a problem with your registration")
        End If

    End Sub

    Protected Sub cvtxtUsername_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles cvftxtUsername.ServerValidate
        Dim sql As String = _
            "select Username from BuilderAccount where Username=" & DB.Quote(txtUsername.Text) _
            & " union all " _
            & " select Username from PIQAccount where Username=" & DB.Quote(txtUsername.Text) _
            & " union all " _
            & " select Username from VendorAccount where Username=" & DB.Quote(txtUsername.Text) _
            & " and VendorAccountId <> " & DB.Number(dbVendorAccount.VendorAccountID)

        args.IsValid = DB.ExecuteScalar(sql) = Nothing
    End Sub

    Protected Sub btnDashboard_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDashboard.Click
        If Register() Then
            Response.Redirect("/vendor/default.aspx")
        End If
    End Sub

    'Protected Sub btnGoToDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnGoToDashBoard.Click
        'Response.Redirect("/vendor/default.aspx")
    'End Sub
End Class
