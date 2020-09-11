Imports DataLayer
Imports Components
Imports System.Data.SqlClient

Partial Class admin_members_add
    Inherits AdminPage
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("MEMBERS")

        If Not IsPostBack Then
            LoadDropdowns()
            LoadMailingList()
            chkSameAsBilling.Checked = True
            rbtnNewsletterYes.Checked = True
            rbtnFormatHTML.Checked = True
            For Each objListItem As ListItem In chklstMailingLists.Items
                objListItem.Selected = True
            Next
            drpBillingCountry.SelectedValue = "US"
        End If

        ToggleShippingValidators(Not chkSameAsBilling.Checked)
        cvrbtnNewsletterYesAtLeastOne.Enabled = rbtnNewsletterYes.Checked
        If drpShippingCountry.SelectedValue <> "US" Then rqdrpShippingState.Enabled = False
        If drpBillingCountry.SelectedValue <> "US" Then rqdrpBillingState.Enabled = False
    End Sub

    Private Sub LoadMailingList()
        chklstMailingLists.DataSource = MailingListRow.GetPermanentLists(DB)
        chklstMailingLists.DataValueField = "ListId"
        chklstMailingLists.DataTextField = "Name"
        chklstMailingLists.DataBind()
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


    Private Sub LoadDropdowns()
        Dim dt As DataTable = StateRow.GetStateList(DB)
        drpBillingState.DataSource = dt
        drpBillingState.DataTextField = "StateName"
        drpBillingState.DataValueField = "StateCode"
        drpBillingState.DataBind()
        drpBillingState.Items.Insert(0, New ListItem("", ""))

        drpShippingState.DataSource = dt
        drpShippingState.DataTextField = "StateName"
        drpShippingState.DataValueField = "StateCode"
        drpShippingState.DataBind()
        drpShippingState.Items.Insert(0, New ListItem("", ""))

        dt = CountryRow.GetCountryList(DB)
        drpBillingCountry.DataSource = dt
        drpBillingCountry.DataTextField = "CountryName"
        drpBillingCountry.DataValueField = "CountryCode"
        drpBillingCountry.DataBind()
        drpBillingCountry.Items.Insert(0, New ListItem("", ""))

        drpShippingCountry.DataSource = dt
        drpShippingCountry.DataTextField = "CountryName"
        drpShippingCountry.DataValueField = "CountryCode"
        drpShippingCountry.DataBind()
        drpShippingCountry.Items.Insert(0, New ListItem("", ""))
    End Sub

    Protected Sub cvrbtnNewsletterYes_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles cvrbtnNewsletterYes.ServerValidate
        If Not rbtnNewsletterYes.Checked And Not rbtnNewsletterNo.Checked Then
            args.IsValid = False
            Exit Sub
        End If
    End Sub

    Protected Sub cvtxtUsername_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles cvtxtUsername.ServerValidate
        Dim Username As String = DB.ExecuteScalar("SELECT Username FROM Member WHERE Username = " & DB.Quote(txtUsername.Text))
        args.IsValid = (Username = String.Empty)
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
        If SysParam.GetValue(DB, "UPSAddressVerification") AndAlso drpShippingCountry.SelectedValue = "US" Then
            Dim ErrorDesc As String = String.Empty
            If Not UPS.ValidateAddress(txtShippingCity.Text, drpShippingState.SelectedValue, txtShippingZip.Text, drpShippingCountry.SelectedValue, ErrorDesc) Then
                AddError("Your Shipping Address is not recognized. Please verify the City, State, Zipcode and Country are correct. Make sure to spell out your full City name.")
                bError = True
            End If
        End If

        If bError Then
            Exit Sub
        End If

        Try
            DB.BeginTransaction()

            Dim dbMember As MemberRow = New MemberRow(DB)
            Dim MemberId As Integer = 0

            dbMember.Username = txtUsername.Text
            dbMember.Password = txtPassword.Text
            dbMember.CreateDate = Now
            dbMember.IsActive = True
            dbMember.IsSameDefaultAddress = chkSameAsBilling.Checked
            dbMember.Guid = Core.GenerateFileID()
            MemberId = dbMember.Insert()

            Dim dbMemberBillingAddress As MemberAddressRow = New MemberAddressRow(DB)
            Dim dbMemberShippingAddress As MemberAddressRow = New MemberAddressRow(DB)

            ' Create Default Billing Address
            dbMemberBillingAddress.MemberId = MemberId
            dbMemberBillingAddress.Label = "Default Billing Address"
            dbMemberBillingAddress.AddressType = "Billing"
            dbMemberBillingAddress.Email = txtBillingEmail.Text
            dbMemberBillingAddress.FirstName = txtBillingFirstName.Text
            dbMemberBillingAddress.LastName = txtBillingLastName.Text
            dbMemberBillingAddress.Address1 = txtBillingAddress1.Text
            dbMemberBillingAddress.Address2 = txtBillingAddress2.Text
            dbMemberBillingAddress.Company = txtBillingCompany.Text
            dbMemberBillingAddress.City = txtBillingCity.Text
            dbMemberBillingAddress.State = drpBillingState.SelectedValue
            dbMemberBillingAddress.Zip = txtBillingZip.Text
            dbMemberBillingAddress.Region = txtBillingRegion.Text
            dbMemberBillingAddress.Country = drpBillingCountry.SelectedValue
            dbMemberBillingAddress.Phone = txtBillingPhone.Text
            dbMemberBillingAddress.IsDefault = True
            dbMemberBillingAddress.Insert()

            ' Create Shipping Address
            dbMemberShippingAddress.MemberId = MemberId
            dbMemberShippingAddress.Label = "Default Shipping Address"
            dbMemberShippingAddress.AddressType = "Shipping"
            If chkSameAsBilling.Checked = True Then
                dbMemberShippingAddress.FirstName = txtBillingFirstName.Text
                dbMemberShippingAddress.LastName = txtBillingLastName.Text
                dbMemberShippingAddress.Address1 = txtBillingAddress1.Text
                dbMemberShippingAddress.Address2 = txtBillingAddress2.Text
                dbMemberShippingAddress.Company = txtBillingCompany.Text
                dbMemberShippingAddress.City = txtBillingCity.Text
                dbMemberShippingAddress.State = drpBillingState.SelectedValue
                dbMemberShippingAddress.Zip = txtBillingZip.Text
                dbMemberShippingAddress.Region = txtBillingRegion.Text
                dbMemberShippingAddress.Email = txtBillingEmail.Text
                dbMemberShippingAddress.Country = drpBillingCountry.SelectedValue
                dbMemberShippingAddress.Phone = txtBillingPhone.Text
            Else
                dbMemberShippingAddress.FirstName = txtShippingFirstName.Text
                dbMemberShippingAddress.LastName = txtShippingLastName.Text
                dbMemberShippingAddress.Address1 = txtShippingAddress1.Text
                dbMemberShippingAddress.Address2 = txtShippingAddress2.Text
                dbMemberShippingAddress.Company = txtShippingCompany.Text
                dbMemberShippingAddress.City = txtShippingCity.Text
                dbMemberShippingAddress.State = drpShippingState.SelectedValue
                dbMemberShippingAddress.Zip = txtShippingZip.Text
                dbMemberShippingAddress.Region = txtShippingRegion.Text
                dbMemberShippingAddress.Country = drpShippingCountry.SelectedValue
                dbMemberShippingAddress.Phone = txtShippingPhone.Text
            End If
            dbMemberShippingAddress.IsDefault = True
            dbMemberShippingAddress.Insert()

            'Update MailingMember, but only if user subscribed for the newsletter
            Dim dbMailingMember As MailingMemberRow = MailingMemberRow.GetRowByEmail(DB, txtBillingEmail.Text)
            dbMailingMember.Email = txtBillingEmail.Text
            dbMailingMember.Name = Core.BuildFullName(txtBillingFirstName.Text, String.Empty, txtBillingLastName.Text)
            If rbtnFormatHTML.Checked = True Then dbMailingMember.MimeType = "HTML" Else dbMailingMember.MimeType = "TEXT"
            dbMailingMember.Status = "ACTIVE"
            If dbMailingMember.MemberId <> 0 Then
                dbMailingMember.Update()
            Else
                dbMailingMember.Insert()
            End If
            dbMailingMember.DeleteFromAllPermanentLists()
            If rbtnNewsletterYes.Checked Then
                dbMailingMember.InsertToLists(chklstMailingLists.SelectedValues)
            End If
            DB.CommitTransaction()
            Core.LogEvent("New member """ & dbMember.Username & """ was added by user """ & Session("Username") & """", Diagnostics.EventLogEntryType.Information)
            Response.Redirect("default.aspx")

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try

    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

End Class
