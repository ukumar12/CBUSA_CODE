Imports Components
Imports DataLayer
Imports Vindicia
Imports Utilities

Partial Class forms_builder_registration_default
    Inherits SitePage

    Protected BuilderId As Integer
    Protected BuilderAccountId As Integer
    Protected dbBuilder As BuilderRow
    Protected dbBuilderAccount As BuilderAccountRow
    Protected p As VindiciaPaymentProcessor
    Protected NewRegistration As Boolean
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

        If Session("BuilderId") Is Nothing Then
            dbBuilder = BuilderRow.GetBuilderByGuid(DB, Guid)
        Else
            dbBuilder = BuilderRow.GetRow(DB, Session("BuilderId"))
        End If
        BuilderId = dbBuilder.BuilderID
        Session("BuilderId") = BuilderId
        p = New VindiciaPaymentProcessor(DB)
        p.IsTestMode = SysParam.GetValue(DB, "TestMode")
        dbBuilderAccount = BuilderAccountRow.GetRow(DB, Session("BuilderAccountId"))
        BuilderAccountId = dbBuilderAccount.BuilderAccountID

        If dbBuilder Is Nothing Then
            dbBuilder = BuilderRow.GetRow(DB, dbBuilderAccount.BuilderID)
            BuilderId = dbBuilder.BuilderID
        End If

        If dbBuilder.BuilderID = Nothing Or dbBuilderAccount.BuilderAccountID = Nothing Then
            Response.Redirect("/default.aspx")
        End If

        Dim dbRegistration As BuilderRegistrationRow = BuilderRegistrationRow.GetRowByBuilder(DB, dbBuilder.BuilderID)
        If dbRegistration IsNot Nothing Then
            Dim dbStatus As RegistrationStatusRow = RegistrationStatusRow.GetRow(DB, dbRegistration.RegistrationStatusID)
            If dbRegistration.CompleteDate.Year = Now.Year OrElse Request.QueryString("RegStep") = "AccountProfile" Then
                'Response.Redirect("/builder/default.aspx")
                ctrlSteps.Visible = False
                btnContinue.Visible = False
                btnDashboard.Visible = True
                btnCancel.Visible = True
            Else
                ctrlSteps.Visible = True
                btnContinue.Visible = True
                btnDashboard.Visible = False
                btnCancel.Visible = False

                'Select Case dbStatus.RegistrationStep
                '    Case 1
                '        Response.Redirect("company.aspx")
                '    Case 2
                '        Response.Redirect("financial.aspx")
                '    Case 3
                '        Response.Redirect("payment.aspx")
                'End Select
            End If

            If dbRegistration.CompleteDate <> Nothing AndAlso dbRegistration.CompleteDate.Year = Now.Year Then
                btnDashboard.Visible = True
                btnContinue.Visible = False
                ctrlSteps.Visible = False

                trEULA.Visible = False
                rfvcbAsp.Enabled = False
            Else
                btnDashboard.Visible = False
                btnContinue.Visible = True

                ctrlSteps.Visible = True
            End If



        End If

        NewRegistration = BuilderRow.IsNewBuilder(DB, dbBuilder.BuilderID)

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")

        If Not IsPostBack Then

        Core.DataLog("Edit Account Information", PageURL, CurrentUserId, "Builder Edit Profile Left Menu Click", "", "", "", "", UserName)

            drpState.DataSource = StateRow.GetStateList(DB)
            drpState.DataTextField = "StateCode"
            drpState.DataValueField = "StateCode"
            drpState.DataBind()
            drpState.Items.Insert(0, New ListItem("Please Select", ""))

            LoadFromDB()
        End If

    End Sub

    Private Sub LoadFromDB()

        If BuilderAccountId = 0 Then
            psMsg.Visible = False
        Else
            rqtxtPassword.Enabled = dbBuilderAccount.RequirePasswordUpdate()
        End If

        If BuilderId = 0 Then
            Exit Sub
        End If

        With dbBuilderAccount
            txtFirstName.Text = dbBuilderAccount.FirstName
            txtLastName.Text = dbBuilderAccount.LastName
            txtEmail.Text = dbBuilderAccount.Email
            txtUsername.Text = dbBuilderAccount.Username
            txtTitle.Text = dbBuilderAccount.Title
            txtAccountFax.Text = dbBuilderAccount.Fax
            txtAccountMobile.Text = dbBuilderAccount.Mobile
            txtAccountPhone.Text = dbBuilderAccount.Phone
        End With

        With dbBuilder
            txtCompanyName.Text = .CompanyName
            txtAddress.Text = .Address
            txtAddress2.Text = .Address2
            txtCity.Text = .City
            drpState.SelectedValue = .State
            txtZip.Text = .Zip
            txtPhone.Text = .Phone
            'txtMobile.Text = .Mobile
            txtFax.Text = .Fax
            txtWebsiteUrl.Text = .WebsiteURL
            txtCompanyEmail.Text = .Email
        End With
    End Sub

    Private Function Process() As Boolean
        If Not Page.IsValid Then Exit Function

        Try
            Dim UpdatedAccount As Boolean = False
            Dim UpdatedContact As Boolean = False

            DB.BeginTransaction()

            DB.ExecuteSQL("update BuilderAccount set IsPrimary=0 where BuilderID=" & DB.Number(BuilderId))

            With dbBuilder
                .CompanyName = txtCompanyName.Text
                .Address = txtAddress.Text
                .Address2 = txtAddress2.Text
                .City = txtCity.Text
                .State = drpState.SelectedValue
                .Zip = txtZip.Text
                .Phone = txtPhone.Text
                .Fax = txtFax.Text
                '.Mobile = txtMobile.Text
                .WebsiteURL = txtWebsiteUrl.Text
                .Email = txtCompanyEmail.Text
                If .HistoricID = Nothing Then
                    .HistoricID = BuilderRow.GetNextHistoricID(DB)
                End If
            End With

            If dbBuilder.BuilderID = 0 Then
                dbBuilder.Submitted = Now()
                dbBuilder.Insert()
            Else

                dbBuilder.Update()
                UpdatedAccount = True
                Try
                    If Not p.UpdateVindiciaAddress(dbBuilder) Then
                        Logger.Error("Vindicia Address Update Failed :" & dbBuilder.BuilderID)
                    End If
                Catch ex As Exception
                    Logger.Error("Vindicia Address Update Failed :" & dbBuilder.BuilderID)
                End Try

            End If

            With dbBuilderAccount
                .FirstName = txtFirstName.Text
                .LastName = txtLastName.Text
                .Email = txtEmail.Text
                .Username = txtUsername.Text
                .BuilderID = dbBuilder.BuilderID
                .IsActive = True
                .IsPrimary = True
                .Title = txtTitle.Text
                .Phone = txtAccountPhone.Text
                .Mobile = txtAccountMobile.Text
                .Fax = txtAccountFax.Text
                If Not txtPassword.Text = String.Empty Then
                    .Password = txtPassword.Text
                    .RequirePasswordUpdate = False
                End If
            End With

            If BuilderAccountId = 0 Then
                dbBuilderAccount.Insert()
            Else
                dbBuilderAccount.Update()
                UpdatedContact = True
            End If

            Dim dbRegistration As BuilderRegistrationRow = BuilderRegistrationRow.GetRowByBuilder(DB, Session("BuilderId"))
            If dbRegistration.Submitted <> Nothing Then
                Dim dbStat As RegistrationStatusRow = RegistrationStatusRow.GetStatusByStep(DB, 1)
                dbRegistration.RegistrationStatusID = dbStat.RegistrationStatusID
                dbRegistration.AcceptsTerms = cbAsp.Checked
                dbRegistration.Update()
            Else
                Dim dbNewRegistration = New BuilderRegistrationRow(DB)
                dbNewRegistration.BuilderID = Session("BuilderId")
                dbNewRegistration.AcceptsTerms = cbAsp.Checked
                Dim dbStat As RegistrationStatusRow = RegistrationStatusRow.GetStatusByStep(DB, 3)
                dbNewRegistration.RegistrationStatusID = dbStat.RegistrationStatusID
                dbNewRegistration.YearStarted = 0
                dbNewRegistration.Employees = 0
                dbNewRegistration.HomesBuiltAndDelivered = 0
                dbNewRegistration.HomeStartsLastYear = 0
                dbNewRegistration.HomeStartsNextYear = 0
                dbNewRegistration.SizeRangeMin = 0
                dbNewRegistration.SizeRangeMax = 0
                dbNewRegistration.PriceRangeMin = 0
                dbNewRegistration.PriceRangeMax = 0
                dbNewRegistration.AvgCostPerSqFt = 0
                dbNewRegistration.RevenueLastYear = 0
                dbNewRegistration.RevenueNextYear = 0
                dbNewRegistration.TotalCOGS = 0
                dbNewRegistration.Affiliations = ""
                dbNewRegistration.WhereYouBuild = ""
                dbNewRegistration.WhereYouBuild = " "
                dbNewRegistration.Submitted = Now()
                dbNewRegistration.Insert()
            End If

            DB.CommitTransaction()

            '' Salesforce Integration
            'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
            'If sfHelper.Login() Then
            '    If UpdatedAccount = True Then
            '        If sfHelper.UpsertBuilder(dbBuilder) = False Then
            '            'throw error
            '        End If
            '    End If
            '    If UpdatedContact = True Then
            '        If sfHelper.UpsertBuilderAccount(dbBuilderAccount) = False Then
            '            'throw error
            '        End If
            '    Else
            '        If sfHelper.InsertBuilderAccount(dbBuilderAccount, dbBuilder.CRMID) = False Then
            '            'throw error
            '        End If
            '    End If
            'End If

            Return True
        Catch ex As Exception
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
        Return False
    End Function

    Protected Sub btnContinue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnContinue.Click
        If Process() Then
            'log Btn Continue Click
            Core.DataLog("Edit Account Information", PageURL, CurrentUserId, "Btn Continue", "", "", "", "", UserName)
            'end log
            Response.Redirect("payment.aspx?id=" & dbBuilder.Guid)
        End If
    End Sub

    Protected Sub btnDashboard_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDashboard.Click
        If Process() Then
            Response.Redirect("/builder/default.aspx")
        End If
    End Sub

    'Protected Sub btnGoToDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnGoToDashBoard.Click
       ' Response.Redirect("/vendor/default.aspx")
   ' End Sub
End Class
