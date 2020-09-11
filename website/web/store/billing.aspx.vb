Imports Components
Imports DataLayer
Imports System.Data.SqlClient

Partial Class store_billing
    Inherits SitePage

    Protected MultipleShipToEnabled As Boolean = False
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureSSL()
        ShoppingCart.EnsureOrder(DB)

        MultipleShipToEnabled = SysParam.GetValue(DB, "MultipleShipToEnabled")
        divRegister.Visible = Not IsLoggedIn()
        tblNewsletter.Visible = Not IsLoggedIn()

        If Not IsPostBack Then
            LoadDropdowns()
            LoadMailingList()

            rbtnCreateAccountNo.Checked = True

            tdShipping.Visible = Not MultipleShipToEnabled
            rqdrpShippingMethod.Enabled = Not MultipleShipToEnabled
            If Session("memberId") Is Nothing Then
                trSaveBilling.Visible = False
            Else
                chkSaveAsBilling.Checked = True
            End If
            PopulateFields()
        End If
        ToggleShippingValidators(Not chkSameAsBilling.Checked)

        'Enable/Disable newsletter and state validators
        cvrbtnNewsletterYesAtLeastOne.Enabled = rbtnNewsletterYes.Checked
        If drpShippingCountry.SelectedValue <> "US" Then rqdrpShippingState.Enabled = False
        If drpBillingCountry.SelectedValue <> "US" Then rqdrpBillingState.Enabled = False

        'Enable/Disable username/password validators
        rqtxtUsername.Enabled = rbtnCreateAccountYes.Checked
        cvtxtUsername.Enabled = rbtnCreateAccountYes.Checked
        rqtxtPassword.Enabled = rbtnCreateAccountYes.Checked
        pvtxtPassword.Enabled = rbtnCreateAccountYes.Checked
        cvtxtPassword.Enabled = rbtnCreateAccountYes.Checked
    End Sub

    Private Sub PopulateFields()
        Dim dbOrder As StoreOrderRow = StoreOrderRow.GetRow(DB, Session("OrderId"))

        If IsLoggedIn() AndAlso dbOrder.BillingLastName = String.Empty Then
            Dim MemberId As Integer = IIf(Session("MemberId") Is Nothing, 0, Session("MemberId"))
            Dim dbAddress As MemberAddressRow = MemberAddressRow.GetDefaultBillingRow(DB, MemberId)

            'prefill data with default billing information
            txtBillingEmail.Text = dbAddress.Email
            txtBillingFirstName.Text = dbAddress.FirstName
            txtBillingLastName.Text = dbAddress.LastName
            txtBillingAddress1.Text = dbAddress.Address1
            txtBillingAddress2.Text = dbAddress.Address2
            txtBillingCompany.Text = dbAddress.Company
            txtBillingCity.Text = dbAddress.City
            drpBillingState.SelectedValue = dbAddress.State
            txtBillingZip.Text = dbAddress.Zip
            txtBillingRegion.Text = dbAddress.Region
            drpBillingCountry.SelectedValue = dbAddress.Country
            txtBillingPhone.Text = dbAddress.Phone
            If drpBillingCountry.SelectedValue = String.Empty Then drpBillingCountry.SelectedValue = "US"
        Else
            txtBillingEmail.Text = dbOrder.Email
            txtBillingFirstName.Text = dbOrder.BillingFirstName
            txtBillingLastName.Text = dbOrder.BillingLastName
            txtBillingAddress1.Text = dbOrder.BillingAddress1
            txtBillingAddress2.Text = dbOrder.BillingAddress2
            txtBillingCompany.Text = dbOrder.BillingCompany
            txtBillingCity.Text = dbOrder.BillingCity
            drpBillingState.SelectedValue = dbOrder.BillingState
            txtBillingZip.Text = dbOrder.BillingZip
            txtBillingRegion.Text = dbOrder.BillingRegion
            drpBillingCountry.SelectedValue = dbOrder.BillingCountry
            txtBillingPhone.Text = dbOrder.BillingPhone
            If drpBillingCountry.SelectedValue = String.Empty Then drpBillingCountry.SelectedValue = "US"
        End If
        hdnBillingCountry.Value = drpBillingCountry.SelectedValue

        If Not MultipleShipToEnabled Then
            Dim dbRecipient As StoreOrderRecipientRow = StoreOrderRecipientRow.GetDefaultRecipient(DB, Session("OrderId"))
            txtShippingFirstName.Text = dbRecipient.FirstName
            txtShippingLastName.Text = dbRecipient.LastName
            txtShippingAddress1.Text = dbRecipient.Address1
            txtShippingAddress2.Text = dbRecipient.Address2
            txtShippingCompany.Text = dbRecipient.Company
            txtShippingCity.Text = dbRecipient.City
            drpShippingState.SelectedValue = dbRecipient.State
            txtShippingZip.Text = dbRecipient.Zip
            txtShippingRegion.Text = dbRecipient.Region
            drpShippingCountry.SelectedValue = dbRecipient.Country
            txtShippingPhone.Text = dbRecipient.Phone
            drpShippingMethod.SelectedValue = dbRecipient.ShippingMethodId
            If Convert.ToBoolean(dbRecipient.GiftWrapping) = True Then
                drpGiftMessage.SelectedValue = dbRecipient.GiftMessageLabel
                txtGiftMessage.Text = dbRecipient.GiftMessage
            Else
                tblGiftMessage.Visible = False
            End If
            hdnShippingCountry.Value = drpShippingCountry.SelectedValue
        End If

        'Same as billing is set to true by default
        If txtShippingFirstName.Text = String.Empty Then chkSameAsBilling.Checked = True

        'Initialize email preferences
        Dim dbMailingMember As MailingMemberRow = MailingMemberRow.GetRowByEmail(DB, dbOrder.Email)
        rbtnNewsletterYes.Checked = (dbMailingMember.Status = "ACTIVE" And Not dbMailingMember.SubscribedLists = String.Empty)
        rbtnNewsletterNo.Checked = Not rbtnNewsletterYes.Checked
        If dbMailingMember.MemberId = 0 Then
            rbtnFormatHTML.Checked = True
            For Each ol As ListItem In chklstMailingLists.Items
                ol.Selected = True
            Next
        Else
            rbtnFormatHTML.Checked = (dbMailingMember.MimeType = "HTML")
            rbtnFormatTEXT.Checked = Not rbtnFormatHTML.Checked
            chklstMailingLists.SelectedValues = dbMailingMember.SubscribedLists
        End If
    End Sub

    Private Sub LoadMailingList()
        chklstMailingLists.DataSource = MailingListRow.GetPermanentLists(DB)
        chklstMailingLists.DataValueField = "ListId"
        chklstMailingLists.DataTextField = "Name"
        chklstMailingLists.DataBind()
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

        BindShippingDropdown()
        If Not MultipleShipToEnabled Then
            Dim dtGiftMessage As DataTable = GiftMessageRow.GetList(DB)
            drpGiftMessage.DataSource = dtGiftMessage
            drpGiftMessage.DataTextField = "GiftMessageLabel"
            drpGiftMessage.DataValueField = "GiftMessage"
            drpGiftMessage.DataBind()
            drpGiftMessage.Items.Insert(0, New ListItem("", ""))
        End If
    End Sub

    Private Sub BindShippingDropdown()
        Dim dt As DataTable = Nothing
        If (drpBillingCountry.SelectedValue = "" And drpShippingCountry.SelectedValue = "") Or (chkSameAsBilling.Checked AndAlso drpBillingCountry.SelectedValue = "US") Or (chkSameAsBilling.Checked = False AndAlso drpShippingCountry.SelectedValue = "US") Then
            dt = StoreShippingMethodRow.GetDomesticShippingMethods(DB)
        Else
            dt = StoreShippingMethodRow.GetInternationalShippingMethods(DB)
        End If
        drpShippingMethod.DataSource = dt
        drpShippingMethod.DataTextField = "Name"
        drpShippingMethod.DataValueField = "MethodId"
        drpShippingMethod.DataBind()
        drpShippingMethod.Items.Insert(0, New ListItem(String.Empty, String.Empty))
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

    Private Function CreateMemberAccount() As Integer
        Dim MemberId As Integer = 0
        Dim dbMember As MemberRow = New MemberRow(DB)

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

        'If Multiple ship-to enabled then don't insert any shipping information
        If MultipleShipToEnabled Then
            Return MemberId
        End If

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

        Return MemberId
    End Function

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
        If Not MultipleShipToEnabled Then
            If SysParam.GetValue(DB, "UPSAddressVerification") AndAlso Not chkSameAsBilling.Checked AndAlso drpShippingCountry.SelectedValue = "US" Then
                Dim ErrorDesc As String = String.Empty
                If Not UPS.ValidateAddress(txtShippingCity.Text, drpShippingState.SelectedValue, txtShippingZip.Text, drpShippingCountry.SelectedValue, ErrorDesc) Then
                    AddError("Your Shipping Address is not recognized. Please verify the City, State, Zipcode and Country are correct. Make sure to spell out your full City name.")
                    bError = True
                End If
            End If
        End If

        If bError Then
            Exit Sub
        End If

        Try
            DB.BeginTransaction()

            Dim MemberId As Integer = 0
            If IsLoggedIn() Then
                MemberId = Session("MemberId")
            End If
            If Not IsLoggedIn() AndAlso rbtnCreateAccountYes.Checked Then
                MemberId = CreateMemberAccount()
            End If

            'Update order billing information
            Dim dbOrder As StoreOrderRow = StoreOrderRow.GetRow(DB, Session("OrderId"))
            dbOrder.MemberId = MemberId
            dbOrder.Email = txtBillingEmail.Text
            dbOrder.BillingFirstName = txtBillingFirstName.Text
            dbOrder.BillingLastName = txtBillingLastName.Text
            dbOrder.BillingAddress1 = txtBillingAddress1.Text
            dbOrder.BillingAddress2 = txtBillingAddress2.Text
            dbOrder.BillingCompany = txtBillingCompany.Text
            dbOrder.BillingCity = txtBillingCity.Text
            dbOrder.BillingState = drpBillingState.SelectedValue
            dbOrder.BillingZip = txtBillingZip.Text
            dbOrder.BillingRegion = txtBillingRegion.Text
            dbOrder.BillingCountry = drpBillingCountry.SelectedValue
            dbOrder.BillingPhone = txtBillingPhone.Text
            dbOrder.Update()

            If MemberId > 0 Then
                If chkSaveAsBilling.Checked = True Then
                    DB.ExecuteSQL("Update Member set IsSameDefaultAddress = 'False' where Memberid =" & DB.Number(MemberId))
                    Dim dbBillingAddress As MemberAddressRow = MemberAddressRow.GetDefaultBillingRow(DB, MemberId)
                    dbBillingAddress.FirstName = txtBillingFirstName.Text
                    dbBillingAddress.LastName = txtBillingLastName.Text
                    dbBillingAddress.Address1 = txtBillingAddress1.Text
                    dbBillingAddress.Address2 = txtBillingAddress2.Text
                    dbBillingAddress.Company = txtBillingCompany.Text
                    dbBillingAddress.City = txtBillingCity.Text
                    dbBillingAddress.State = drpBillingState.SelectedValue
                    dbBillingAddress.Zip = txtBillingZip.Text
                    dbBillingAddress.Region = txtBillingRegion.Text
                    dbBillingAddress.Country = drpBillingCountry.SelectedValue
                    dbBillingAddress.Phone = txtBillingPhone.Text
                    dbBillingAddress.Update()
                End If
            End If
         

            'Update order shipping information
            If Not MultipleShipToEnabled Then
                Dim dbRecipient As StoreOrderRecipientRow = StoreOrderRecipientRow.GetDefaultRecipient(DB, Session("OrderId"))
                If chkSameAsBilling.Checked = True Then
                    dbRecipient.FirstName = txtBillingFirstName.Text
                    dbRecipient.LastName = txtBillingLastName.Text
                    dbRecipient.Address1 = txtBillingAddress1.Text
                    dbRecipient.Address2 = txtBillingAddress2.Text
                    dbRecipient.City = txtBillingCity.Text
                    dbRecipient.State = drpBillingState.SelectedValue
                    dbRecipient.Zip = txtBillingZip.Text
                    dbRecipient.Region = txtBillingRegion.Text
                    dbRecipient.Country = drpBillingCountry.SelectedValue
                    dbRecipient.Phone = txtBillingPhone.Text
                Else
                    dbRecipient.FirstName = txtShippingFirstName.Text
                    dbRecipient.LastName = txtShippingLastName.Text
                    dbRecipient.Address1 = txtShippingAddress1.Text
                    dbRecipient.Address2 = txtShippingAddress2.Text
                    dbRecipient.City = txtShippingCity.Text
                    dbRecipient.State = drpShippingState.SelectedValue
                    dbRecipient.Zip = txtShippingZip.Text
                    dbRecipient.Region = txtShippingRegion.Text
                    dbRecipient.Country = drpShippingCountry.SelectedValue
                    dbRecipient.Phone = txtShippingPhone.Text
                End If
                dbRecipient.GiftMessage = txtGiftMessage.Text
                dbRecipient.GiftMessageLabel = drpGiftMessage.SelectedValue
                dbRecipient.ShippingMethodId = IIf(drpShippingMethod.SelectedValue = String.Empty, 0, drpShippingMethod.SelectedValue)
                dbRecipient.Update()
            End If

            'Update MailingMember, but only if user subscribed for the newsletter and when the user
            'is not logged in
            If Not IsLoggedIn() Then
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
            End If

            DB.CommitTransaction()

            'Log in member automatically, if the user have choosen to create account
            If rbtnCreateAccountYes.Checked AndAlso Not IsLoggedIn() And MemberId > 0 Then
                Session("MemberId") = MemberId
            End If
            Response.Redirect("shipping.aspx")

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub drpShippingCountry_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles drpShippingCountry.SelectedIndexChanged
        If Not MultipleShipToEnabled Then BindShippingDropdown()
    End Sub

    Protected Sub drpBillingCountry_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles drpBillingCountry.SelectedIndexChanged
        If Not MultipleShipToEnabled Then BindShippingDropdown()
    End Sub

    Protected Sub chkSameAsBilling_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSameAsBilling.CheckedChanged
        If Not MultipleShipToEnabled Then BindShippingDropdown()
    End Sub
End Class
