Imports Components
Imports DataLayer

Partial Class forms_vendor_registration_default
    Inherits SitePage

    Protected VendorId As Integer
    Protected VendorAccountId As Integer
    Protected dbVendor As VendorRow
    Protected dbVendorAccount As VendorAccountRow
    Protected dbVendorRegistration As VendorRegistrationRow

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("VendorId") <> 0 Then
            VendorId = CType(Session("VendorId"), Integer)
            dbVendor = VendorRow.GetRow(Me.DB, VendorId)
            dbVendorRegistration = VendorRegistrationRow.GetRowByVendor(Me.DB, VendorId)
        Else
            Response.Redirect("/default.aspx")
        End If

        If Session("VendorAccountId") <> 0 Then
            VendorAccountId = CType(Session("VendorAccountId"), Integer)
            dbVendorAccount = VendorAccountRow.GetRow(Me.DB, VendorAccountId)
        Else
            Response.Redirect("/default.aspx")
        End If

        If Not IsPostBack Then

            drpState.DataSource = DataLayer.StateRow.GetStateList(DB)
            drpState.DataTextField = "StateName"
            drpState.DataValueField = "StateCode"
            drpState.DataBind()
            drpState.Items.Insert(0, New ListItem("", ""))

            drpPrimarySupplyPhase.DataSource = DataLayer.SupplyPhaseRow.GetList(Me.DB, "SupplyPhase")
            drpPrimarySupplyPhase.DataTextField = "SupplyPhase"
            drpPrimarySupplyPhase.DataValueField = "SupplyPhaseId"
            drpPrimarySupplyPhase.DataBind()
            drpPrimarySupplyPhase.Items.Insert(0, New ListItem("", ""))

            drpSeconarySupplyPhase.DataSource = DataLayer.SupplyPhaseRow.GetList(Me.DB, "SupplyPhase")
            drpSeconarySupplyPhase.DataTextField = "SupplyPhase"
            drpSeconarySupplyPhase.DataValueField = "SupplyPhaseId"
            drpSeconarySupplyPhase.DataBind()
            drpSeconarySupplyPhase.Items.Insert(0, New ListItem("", ""))

            LoadFromDB()

        End If

    End Sub

    Private Sub LoadFromDB()
        If dbVendor.VendorID = 0 Or dbVendorRegistration.VendorRegistrationID = 0 Then
            Exit Sub
        End If

        With dbVendor
            txtCompanyName.Text = .CompanyName
            txtAddress.Text = .Address
            txtAddress2.Text = .Address2
            txtCity.Text = .City
            drpState.SelectedValue = .State
            txtZip.Text = .Zip
            txtPhone.Text = .Phone
            txtMobile.Text = .Mobile
            txtPager.Text = .Pager
            txtOtherPhone.Text = .OtherPhone
            txtFax.Text = .Fax
            txtWebsiteUrl.Text = .WebsiteURL
            txtEmail.Text = .Email
        End With

        If dbVendorRegistration.PrimarySupplyPhaseID > 0 Then
            With dbVendorRegistration

                txtYearStarted.Text = .YearStarted
                txtEmployees.Text = .Employees
                txtProductsOffered.Text = .ProductsOffered
                txtCompanyMemberships.Text = .CompanyMemberships
                rblIsSubsidiary.SelectedValue = .IsSubsidiary
                txtSubsidiaryExplanation.Text = .SubsidiaryExplanation
                rblHadLawsuit.SelectedValue = .HadLawsuit
                txtLawsuitExplanation.Text = .LawsuitExplanation
                txtSupplyArea.Text = .SupplyArea
                drpPrimarySupplyPhase.SelectedValue = .PrimarySupplyPhaseID
                drpSeconarySupplyPhase.SelectedValue = .SecondarySupplyPhaseID
                txtNotes.Text = .Notes
                dbVendor.VendorID = .VendorID
            End With
        End If

    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Page.Validate("VendorReg")
        If Not Page.IsValid Then Exit Sub

        Try
            DB.BeginTransaction()

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
                .Pager = txtPager.Text
                .OtherPhone = txtOtherPhone.Text
                .WebsiteURL = txtWebsiteUrl.Text
                .Email = txtEmail.Text
            End With

            Dim dbStat As RegistrationStatusRow = RegistrationStatusRow.GetRowByStatus(DB, "Pending")

            If dbVendor.VendorID = 0 Then
                dbVendor.Insert()
            Else
                dbVendor.Update()
            End If

            dbVendorRegistration = VendorRegistrationRow.GetRowByVendor(Me.DB, VendorId)

            With dbVendorRegistration
                .YearStarted = txtYearStarted.Text
                .Employees = txtEmployees.Text
                .ProductsOffered = txtProductsOffered.Text
                .CompanyMemberships = txtCompanyMemberships.Text
                .IsSubsidiary = CType(rblIsSubsidiary.SelectedValue, Boolean)
                .SubsidiaryExplanation = txtSubsidiaryExplanation.Text
                .HadLawsuit = CType(rblHadLawsuit.SelectedValue, Boolean)
                .LawsuitExplanation = txtLawsuitExplanation.Text
                .SupplyArea = txtSupplyArea.Text
                .PrimarySupplyPhaseID = drpPrimarySupplyPhase.SelectedValue
                .SecondarySupplyPhaseID = drpSeconarySupplyPhase.SelectedValue
                .Notes = txtNotes.Text
                .VendorID = dbVendor.VendorID
            End With

            dbVendorRegistration.Update()

            DB.CommitTransaction()

            Response.Redirect("register.aspx")

        Catch ex As Exception
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))

        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("register.aspx")
    End Sub
End Class
