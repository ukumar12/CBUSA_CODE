Imports Components
Imports DataLayer
Imports System.Data.SqlClient
Imports Controls

Partial Class admin_members_address
    Inherits AdminPage
    Protected memberId As Integer
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("MEMBERS")
        MemberId = Convert.ToInt32(Request("MemberId"))
        If Not IsPostBack Then
            LoadDropdowns()

            Dim dbMember As MemberRow = MemberRow.GetRow(DB, memberId)
            Dim dbBilling As MemberAddressRow = MemberAddressRow.GetDefaultBillingRow(DB, memberId)
            txtMemberName.Text = "<b>" + Core.BuildFullName(dbBilling.FirstName, dbBilling.MiddleInitial, dbBilling.LastName) + " (" + dbMember.Username + ")</b>"

            Dim dbShipping As MemberAddressRow = MemberAddressRow.GetDefaultShippingRow(DB, memberId)

            txtBillingFirstName.Text = dbBilling.FirstName
            txtBillingLastName.Text = dbBilling.LastName
            txtBillingAddress1.Text = dbBilling.Address1
            txtBillingAddress2.Text = dbBilling.Address2
            txtBillingCompany.Text = dbBilling.Company
            txtBillingCity.Text = dbBilling.City
            drpBillingState.SelectedValue = dbBilling.State
            txtBillingZip.Text = dbBilling.Zip
            txtBillingRegion.Text = dbBilling.Region
            drpBillingCountry.SelectedValue = dbBilling.Country
            txtBillingPhone.Text = dbBilling.Phone

            txtShippingCompany.Text = dbShipping.Company
            txtShippingFirstName.Text = dbShipping.FirstName
            txtShippingLastName.Text = dbShipping.LastName
            txtShippingAddress1.Text = dbShipping.Address1
            txtShippingAddress2.Text = dbShipping.Address2
            txtShippingCity.Text = dbShipping.City
            drpShippingState.SelectedValue = dbShipping.State
            txtShippingZip.Text = dbShipping.Zip
            txtShippingRegion.Text = dbShipping.Region
            drpShippingCountry.SelectedValue = dbShipping.Country
            txtShippingPhone.Text = dbShipping.Phone

            lnkBack.HRef = "/admin/members/view.aspx?MemberId=" & memberId & "&" & GetPageParams(FilterFieldType.All)
            If dbMember.IsSameDefaultAddress Then chkSameAsBilling.Checked = True
        End If

        ToggleShippingValidators(Not chkSameAsBilling.Checked)

        If drpShippingCountry.SelectedValue <> "US" Then rqdrpShippingState.Enabled = False
        If drpBillingCountry.SelectedValue <> "US" Then rqdrpBillingState.Enabled = False
    End Sub

    Private Sub LoadDropdowns()
        Dim ds As DataTable = StateRow.GetStateList(DB)
        drpBillingState.DataSource = ds
        drpBillingState.DataTextField = "StateName"
        drpBillingState.DataValueField = "StateCode"
        drpBillingState.DataBind()
        drpBillingState.Items.Insert(0, New ListItem("", ""))

        drpShippingState.DataSource = ds
        drpShippingState.DataTextField = "StateName"
        drpShippingState.DataValueField = "StateCode"
        drpShippingState.DataBind()
        drpShippingState.Items.Insert(0, New ListItem("", ""))

        ds = CountryRow.GetCountryList(DB)
        drpBillingCountry.DataSource = ds
        drpBillingCountry.DataTextField = "CountryName"
        drpBillingCountry.DataValueField = "CountryCode"
        drpBillingCountry.DataBind()
        drpBillingCountry.Items.Insert(0, New ListItem("", ""))

        drpShippingCountry.DataSource = ds
        drpShippingCountry.DataTextField = "CountryName"
        drpShippingCountry.DataValueField = "CountryCode"
        drpShippingCountry.DataBind()
        drpShippingCountry.Items.Insert(0, New ListItem("", ""))
    End Sub

    Private Sub ToggleShippingValidators(ByVal value As Boolean)
        rqtxtShippingFirstName.Enabled = value
        rqtxtShippingLastName.Enabled = value
        rqtxtShippingAddress1.Enabled = value
        rqtxtShippingCity.Enabled = value
        rqtxtShippingZip.Enabled = value
        rqdrpShippingState.Enabled = value
        rqtxtShippingPhone.Enabled = value
        rqdrpShippingCountry.Enabled = value
    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        If Not IsValid Then Exit Sub

        Dim bError As Boolean = False
        ' CHECK THE BILLING ADDRESS
        If SysParam.GetValue(DB, "UPSAddressVerification") AndAlso drpBillingCountry.SelectedValue = "US" Then
            Dim ErrorDesc As String = String.Empty
            If Not UPS.ValidateAddress(txtBillingCity.Text, drpBillingState.SelectedValue, txtBillingZip.Text, drpBillingCountry.SelectedValue, ErrorDesc) Then
                AddError("Your Billing Address is not recognized. Please verify the City, State, Zipcode and Country are correct. Make sure to spell out your full City name.")
                bError = True
            End If
        End If

        ' CHECK THE SHIPPING ADDRESS
        If SysParam.GetValue(DB, "UPSAddressVerification") AndAlso Not chkSameAsBilling.Checked AndAlso drpShippingCountry.SelectedValue = "US" Then
            Dim ErrorDesc As String = String.Empty
            If Not UPS.ValidateAddress(txtShippingCity.Text, drpShippingState.SelectedValue, txtShippingZip.Text, drpShippingCountry.SelectedValue, ErrorDesc) Then
                AddError("Your Default Shipping Address is not recognized. Please verify the City, State, Zipcode and Country are correct. Make sure to spell out your full City name.")
                bError = True
            End If
        End If

        If bError Then
            Exit Sub
        End If

        Try
            DB.BeginTransaction()

            Dim dbMember As MemberRow = MemberRow.GetRow(DB, memberId)
            dbMember.IsSameDefaultAddress = chkSameAsBilling.Checked
            dbMember.ModifyDate = Date.Now
            dbMember.Update()

            Dim dbBilling As MemberAddressRow = MemberAddressRow.GetDefaultBillingRow(DB, memberId)
            dbBilling.FirstName = txtBillingFirstName.Text
            dbBilling.LastName = txtBillingLastName.Text
            dbBilling.Address1 = txtBillingAddress1.Text
            dbBilling.Address2 = txtBillingAddress2.Text
            dbBilling.Company = txtBillingCompany.Text
            dbBilling.City = txtBillingCity.Text
            dbBilling.State = drpBillingState.SelectedValue
            dbBilling.Zip = txtBillingZip.Text
            dbBilling.Region = txtBillingRegion.Text
            dbBilling.Country = drpBillingCountry.SelectedValue
            dbBilling.Phone = txtBillingPhone.Text
            dbBilling.Update()

            Dim dbShipping As MemberAddressRow = MemberAddressRow.GetDefaultShippingRow(DB, memberId)
            If chkSameAsBilling.Checked = True Then
                dbShipping.Company = txtBillingCompany.Text
                dbShipping.FirstName = txtBillingFirstName.Text
                dbShipping.LastName = txtBillingLastName.Text
                dbShipping.Address1 = txtBillingAddress1.Text
                dbShipping.Address2 = txtBillingAddress2.Text
                dbShipping.City = txtBillingCity.Text
                dbShipping.State = drpBillingState.SelectedValue
                dbShipping.Zip = txtBillingZip.Text
                dbShipping.Region = txtBillingRegion.Text
                dbShipping.Country = drpBillingCountry.SelectedValue
                dbShipping.Phone = txtBillingPhone.Text
            Else
                dbShipping.Company = txtShippingCompany.Text
                dbShipping.FirstName = txtShippingFirstName.Text
                dbShipping.LastName = txtShippingLastName.Text
                dbShipping.Address1 = txtShippingAddress1.Text
                dbShipping.Address2 = txtShippingAddress2.Text
                dbShipping.City = txtShippingCity.Text
                dbShipping.State = drpShippingState.SelectedValue
                dbShipping.Zip = txtShippingZip.Text
                dbShipping.Region = txtShippingRegion.Text
                dbShipping.Country = drpShippingCountry.SelectedValue
                dbShipping.Phone = txtShippingPhone.Text
            End If
            dbShipping.Update()

            DB.CommitTransaction()

            Response.Redirect("view.aspx?memberId=" & memberId & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try

    End Sub
    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("/admin/members/view.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

End Class
