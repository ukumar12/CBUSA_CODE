Imports Components
Imports DataLayer
Imports System.Data.SqlClient

Partial Class store_shipping
    Inherits SitePage

    Private dtRecipients As DataTable
    Private OrderId As Integer
    Protected MemberId As Integer
    Private dtCountry As DataTable
    Private dtState As DataTable
    Private dtGiftMessage As DataTable
    Private dtOrderAddresses As DataTable

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureSSL()
        ShoppingCart.EnsureOrder(DB)
        ShoppingCart.EnsureBillingInfo(DB, Session("OrderId"))

        OrderId = IIf(Session("OrderId") Is Nothing, 0, Session("OrderId"))
        MemberId = IIf(Session("MemberId") Is Nothing, 0, Session("MemberId"))

        'Redirect to payment page if multiple ship-to functionality is not enabled
        Dim MultipleShipToEnabled As Boolean = SysParam.GetValue(DB, "MultipleShipToEnabled")
        If Not MultipleShipToEnabled Then
            Response.Redirect("payment.aspx")
        End If

        If Not IsPostBack Then
            BindData()
        Else
            'Enable/Disalbe State Validators
            For Each row As RepeaterItem In rptRecipients.Items
                Dim drpCountry As DropDownList = row.FindControl("drpCountry")
                Dim rqdrpState As RequiredFieldValidator = row.FindControl("rqdrpState")
                If drpCountry.SelectedValue <> "US" Then rqdrpState.Enabled = False
            Next
        End If
    End Sub

    Private Sub BindData()
        dtRecipients = ShoppingCart.GetOrderRecipients(DB, OrderId)
        dtCountry = CountryRow.GetCountryList(DB)
        dtState = StateRow.GetStateList(DB)
        dtOrderAddresses = ShoppingCart.GetOrderAddresses(DB, OrderId, MemberId)
        dtGiftMessage = GiftMessageRow.GetList(DB)

        rptRecipients.DataSource = dtRecipients
        rptRecipients.DataBind()

        rptArray.DataSource = dtOrderAddresses
        rptArray.DataBind()
    End Sub

    Protected Sub drpCountry_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim drpCountry As DropDownList = CType(sender, DropDownList)
        Dim drpShippingMethod As DropDownList = CType(drpCountry.Parent.FindControl("drpShippingMethod"), DropDownList)
        Dim hdnCountry As HiddenField = CType(drpCountry.Parent.FindControl("hdnCountry"), HiddenField)
        hdnCountry.Value = drpCountry.SelectedValue
        BindShippingDropdown(drpCountry, drpShippingMethod)
    End Sub

    Protected Sub rptRecipients_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptRecipients.ItemDataBound
        If Not e.Item.ItemType = ListItemType.AlternatingItem AndAlso Not e.Item.ItemType = ListItemType.Item Then
            Exit Sub
        End If

        Dim drpState As DropDownList = e.Item.FindControl("drpState")
        Dim drpCountry As DropDownList = e.Item.FindControl("drpCountry")
        Dim tdGiftMessage As HtmlTableCell = e.Item.FindControl("tdGiftMessage")
        Dim drpGiftMessage As DropDownList = e.Item.FindControl("drpGiftMessage")
        Dim drpAddresses As DropDownList = e.Item.FindControl("drpAddresses")
        Dim hdnRecipient As HtmlInputHidden = e.Item.FindControl("hdnRecipient")
        Dim drpShippingMethod As DropDownList = e.Item.FindControl("drpShippingMethod")
        Dim trchkSaveShipping As HtmlTableRow = e.Item.FindControl("trchkSaveShipping")

        hdnRecipient.Value = e.Item.DataItem("RecipientId")

        drpAddresses.DataSource = dtOrderAddresses
        drpAddresses.DataTextField = "Label"
        drpAddresses.DataValueField = "LabelValue"
        drpAddresses.DataBind()
        drpAddresses.Items.Insert(0, New ListItem("-- please select --", ""))
        drpAddresses.SelectedValue = IIf(IsDBNull(e.Item.DataItem("Label")), String.Empty, Convert.ToString(e.Item.DataItem("Label")))
        If Not IsDBNull(e.Item.DataItem("Label")) And drpAddresses.SelectedValue = String.Empty Then
            drpAddresses.Items.RemoveAt(0)
            drpAddresses.Items.Insert(0, New ListItem("Other", ""))
        End If
        If drpAddresses.SelectedValue = "Same as Billing" Then trchkSaveShipping.Visible = False

        If MemberId = 0 Then
            trchkSaveShipping.Visible = False
        End If

        tdGiftMessage.Visible = Not IsDBNull(e.Item.DataItem("GiftWrap"))

        drpState.DataSource = dtState
        drpState.DataTextField = "StateName"
        drpState.DataValueField = "StateCode"
        drpState.DataBind()
        drpState.Items.Insert(0, New ListItem("", ""))
        drpState.SelectedValue = IIf(IsDBNull(e.Item.DataItem("State")), String.Empty, e.Item.DataItem("State"))

        drpCountry.DataSource = dtCountry
        drpCountry.DataTextField = "CountryName"
        drpCountry.DataValueField = "CountryCode"
        drpCountry.DataBind()
        drpCountry.Items.Insert(0, New ListItem("", ""))
        drpCountry.SelectedValue = IIf(IsDBNull(e.Item.DataItem("Country")), String.Empty, e.Item.DataItem("Country"))
        ScriptManager1.RegisterAsyncPostBackControl(drpCountry)

        drpGiftMessage.DataSource = dtGiftMessage
        drpGiftMessage.DataTextField = "GiftMessageLabel"
        drpGiftMessage.DataValueField = "GiftMessage"
        drpGiftMessage.DataBind()
        drpGiftMessage.Items.Insert(0, New ListItem("", ""))
        drpGiftMessage.SelectedValue = IIf(IsDBNull(e.Item.DataItem("GiftMessageLabel")), String.Empty, e.Item.DataItem("GiftMessageLabel"))

        Dim txtFirstName As TextBox = e.Item.FindControl("txtFirstName")
        Dim txtLastName As TextBox = e.Item.FindControl("txtLastName")
        Dim txtCompany As TextBox = e.Item.FindControl("txtCompany")
        Dim txtAddress1 As TextBox = e.Item.FindControl("txtAddress1")
        Dim txtAddress2 As TextBox = e.Item.FindControl("txtAddress2")
        Dim txtCity As TextBox = e.Item.FindControl("txtCity")
        Dim txtZip As TextBox = e.Item.FindControl("txtZip")
        Dim txtRegion As TextBox = e.Item.FindControl("txtRegion")
        Dim txtPhone As TextBox = e.Item.FindControl("txtPhone")
        Dim txtGiftMessage As LimitTextBox = e.Item.FindControl("txtGiftMessage")
        Dim hdnCountry As HiddenField = e.Item.FindControl("hdnCountry")

        txtFirstName.Text = IIf(IsDBNull(e.Item.DataItem("FirstName")), String.Empty, e.Item.DataItem("FirstName"))
        txtLastName.Text = IIf(IsDBNull(e.Item.DataItem("LastName")), String.Empty, e.Item.DataItem("LastName"))
        txtCompany.Text = IIf(IsDBNull(e.Item.DataItem("Company")), String.Empty, e.Item.DataItem("Company"))
        txtAddress1.Text = IIf(IsDBNull(e.Item.DataItem("Address1")), String.Empty, e.Item.DataItem("Address1"))
        txtAddress2.Text = IIf(IsDBNull(e.Item.DataItem("Address2")), String.Empty, e.Item.DataItem("Address2"))
        txtCity.Text = IIf(IsDBNull(e.Item.DataItem("City")), String.Empty, e.Item.DataItem("City"))
        txtZip.Text = IIf(IsDBNull(e.Item.DataItem("Zip")), String.Empty, e.Item.DataItem("Zip"))
        txtRegion.Text = IIf(IsDBNull(e.Item.DataItem("Region")), String.Empty, e.Item.DataItem("Region"))
        txtPhone.Text = IIf(IsDBNull(e.Item.DataItem("Phone")), String.Empty, e.Item.DataItem("Phone"))
        txtGiftMessage.Text = IIf(IsDBNull(e.Item.DataItem("GiftMessage")), String.Empty, e.Item.DataItem("GiftMessage"))
        hdnCountry.Value = drpCountry.SelectedValue

        ' Dynamically generate code for each pair of hiddenfield/dropdown to capture any Google Autofill events.
        Dim sAutofill As String
        sAutofill = " var sShippingCountry" & e.Item.DataItem("RecipientId") & " = document.getElementById('" & hdnCountry.ClientID & "').value;" & vbCrLf
        sAutofill &= "var sDrpShippingCountry" & e.Item.DataItem("RecipientId") & " = document.getElementById('" & drpCountry.ClientID & "')[document.getElementById('" & drpCountry.ClientID & "').selectedIndex].value;" & vbCrLf
        sAutofill &= "document.getElementById('" & hdnCountry.ClientID & "').value = sDrpShippingCountry" & e.Item.DataItem("RecipientId") & ";"
        sAutofill &= "if (sDrpShippingCountry" & e.Item.DataItem("RecipientId") & " != sShippingCountry" & e.Item.DataItem("RecipientId") & ") {" & vbCrLf
        sAutofill &= " __doPostBack('" & drpCountry.ClientID & "','');" & vbCrLf
        sAutofill &= "}"
        ltlGoogleAutoFillCapture.Text &= sAutofill

        BindShippingDropdown(drpCountry, drpShippingMethod)
        drpShippingMethod.SelectedValue = IIf(IsDBNull(e.Item.DataItem("ShippingMethodId")), String.Empty, e.Item.DataItem("ShippingMethodId"))
    End Sub

    Protected Sub BindShippingDropdown(ByVal drpCountry As DropDownList, ByVal drpShippingMethod As DropDownList)
        Dim dt As DataTable = Nothing
        If drpCountry.SelectedValue = "US" Or drpCountry.SelectedValue = "" Then
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

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        If Not IsValid Then Exit Sub

        'Validate Addresses
        Dim bError As Boolean = False
        For Each recipient As RepeaterItem In rptRecipients.Items
            Dim txtCity As TextBox = recipient.FindControl("txtCity")
            Dim txtZip As TextBox = recipient.FindControl("txtZip")
            Dim drpCountry As DropDownList = recipient.FindControl("drpCountry")
            Dim drpState As DropDownList = recipient.FindControl("drpState")
            Dim hdnRecipient As HtmlInputHidden = recipient.FindControl("hdnRecipient")
            Dim dbRecipient As StoreOrderRecipientRow = StoreOrderRecipientRow.GetRow(DB, hdnRecipient.Value)
            Dim drpShippingMethod As DropDownList = recipient.FindControl("drpShippingMethod")

            If SysParam.GetValue(DB, "UPSAddressVerification") AndAlso drpCountry.SelectedValue = "US" Then
                Dim ErrorDesc As String = String.Empty
                If Not UPS.ValidateAddress(txtCity.Text, drpState.SelectedValue, txtZip.Text, drpCountry.SelectedValue, ErrorDesc) Then
                    AddError("Your Shipping Address for '" & dbRecipient.Label & "' is not recognized. Please verify the City, State, Zipcode and Country are correct. Make sure to spell out your full City name.")
                    bError = True
                End If
            End If
        Next
        If bError Then
            Exit Sub
        End If

        If UpdateRecipients() Then Response.Redirect("/store/payment.aspx")
    End Sub

    Private Function UpdateRecipients() As Boolean
        Try
            DB.BeginTransaction()

            For Each recipient As RepeaterItem In rptRecipients.Items
                Dim hdnRecipient As HtmlInputHidden = recipient.FindControl("hdnRecipient")
                Dim dbRecipient As StoreOrderRecipientRow = StoreOrderRecipientRow.GetRow(DB, hdnRecipient.Value)
                If Not dbRecipient.OrderId = OrderId Then
                    Throw New ApplicationException("Invalid Recipient")
                End If
                Dim txtFirstName As TextBox = recipient.FindControl("txtFirstName")
                Dim txtLastName As TextBox = recipient.FindControl("txtLastName")
                Dim txtCompany As TextBox = recipient.FindControl("txtCompany")
                Dim txtAddress1 As TextBox = recipient.FindControl("txtAddress1")
                Dim txtAddress2 As TextBox = recipient.FindControl("txtAddress2")
                Dim txtCity As TextBox = recipient.FindControl("txtCity")
                Dim txtZip As TextBox = recipient.FindControl("txtZip")
                Dim txtRegion As TextBox = recipient.FindControl("txtRegion")
                Dim txtPhone As TextBox = recipient.FindControl("txtPhone")
                Dim drpCountry As DropDownList = recipient.FindControl("drpCountry")
                Dim drpState As DropDownList = recipient.FindControl("drpState")
                Dim drpGiftMessage As DropDownList = recipient.FindControl("drpGiftMessage")
                Dim txtGiftMessage As LimitTextBox = recipient.FindControl("txtGiftMessage")
                Dim drpShippingMethod As DropDownList = recipient.FindControl("drpShippingMethod")
                Dim drpAddresses As DropDownList = recipient.FindControl("drpAddresses")
                Dim chkSaveToAddressBook As CheckBox = recipient.FindControl("chkSaveToAddressBook")
                Dim AddressId As Integer = 0

                If chkSaveToAddressBook.Checked = True And MemberId > 0 Then
                    Dim dbMemberAddress As MemberAddressRow
                    If Not dbRecipient.AddressId = 0 Then
                        If drpAddresses.SelectedValue = "Default Shipping Address" Then
                            dbMemberAddress = MemberAddressRow.GetDefaultShippingRow(DB, MemberId)
                        Else
                            dbMemberAddress = MemberAddressRow.GetRow(DB, dbRecipient.AddressId)
                        End If
                    Else
                        dbMemberAddress = New MemberAddressRow(DB)
                        dbMemberAddress.AddressType = "AddressBook"
                    End If
                    dbMemberAddress.Label = dbRecipient.Label
                    dbMemberAddress.FirstName = txtFirstName.Text
                    dbMemberAddress.LastName = txtLastName.Text
                    dbMemberAddress.Company = txtCompany.Text
                    dbMemberAddress.Address1 = txtAddress1.Text
                    dbMemberAddress.Address2 = txtAddress2.Text
                    dbMemberAddress.City = txtCity.Text
                    dbMemberAddress.Zip = txtZip.Text
                    dbMemberAddress.Region = txtRegion.Text
                    dbMemberAddress.Phone = txtPhone.Text
                    dbMemberAddress.Country = drpCountry.SelectedValue
                    dbMemberAddress.State = drpState.SelectedValue
                    dbMemberAddress.MemberId = MemberId
                    If Not dbRecipient.AddressId = 0 Then
                        dbMemberAddress.Update()
                    Else
                        AddressId = dbMemberAddress.Insert()
                        dbRecipient.AddressId = AddressId
                    End If
                End If

				If Not drpAddresses.SelectedValue = String.Empty Then
                    dbRecipient.Label = drpAddresses.SelectedValue
                    If drpAddresses.SelectedValue = "Default Shipping Address" Then
                        dbRecipient.AddressId = MemberAddressRow.GetDefaultShippingRow(DB, MemberId).AddressId
                    ElseIf drpAddresses.SelectedValue = "Same as Billing" Then
                        dbRecipient.AddressId = 0
                    Else
                        dbRecipient.AddressId = DB.ExecuteScalar("select AddressId from MemberAddress where MemberId = " & DB.Quote(MemberId) & " and  Label=" & DB.Quote(drpAddresses.SelectedValue))
                    End If
                End If

                dbRecipient.FirstName = txtFirstName.Text
                dbRecipient.LastName = txtLastName.Text
                dbRecipient.Address1 = txtAddress1.Text
                dbRecipient.Address2 = txtAddress2.Text
                dbRecipient.Company = txtCompany.Text
                dbRecipient.City = txtCity.Text
                dbRecipient.Zip = txtZip.Text
                dbRecipient.Region = txtRegion.Text
                dbRecipient.Phone = txtPhone.Text
                dbRecipient.Country = drpCountry.SelectedValue
                dbRecipient.State = drpState.SelectedValue
                dbRecipient.GiftMessage = txtGiftMessage.Text
                dbRecipient.GiftMessageLabel = drpGiftMessage.Text
                dbRecipient.ShippingMethodId = drpShippingMethod.SelectedValue
                dbRecipient.Update()

            Next
            ShoppingCart.RecalculateShoppingCart(DB, OrderId)

            DB.CommitTransaction()

            Return True

        Catch ex As ApplicationException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ex.Message)
        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Function
End Class
