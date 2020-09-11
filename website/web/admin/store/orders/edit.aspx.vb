Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components
Imports System.Web.UI

Partial Class Edit
    Inherits AdminPage
    Implements ICallbackEventHandler

    Protected OrderId As Integer
    Protected dbStoreOrder As StoreOrderRow
    Protected dbRecipient As StoreOrderRecipientRow
    Protected CallBack As String
    Private Previousurl As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("ORDERS")

        CallBack = Page.ClientScript.GetCallbackEventReference(Me, "arg", "OrderNotesRefresh", "")
        OrderId = Convert.ToInt32(Request("OrderId"))
        dbStoreOrder = StoreOrderRow.GetRow(DB, OrderId)

        'get the PaymentLog fraud indication if any for the order
        Dim dbPaymentLog As PaymentLogRow = PaymentLogRow.GetRowByOrderNo(DB, dbStoreOrder.OrderNo)
        If dbPaymentLog.IsHighRisk = True Then
            divHighRisk.InnerHtml = "Warning - High Risk Order<br />" & dbPaymentLog.VerificationResponse & "<br />"
            divHighRisk.Visible = True
        End If

        If Not IsPostBack Then
            LoadFromDB()
            ViewState("PrevUrl") = Request.QueryString("RedirectUrl") & "&" & GetPageParams(FilterFieldType.All) & "&MemberId=" & Request.QueryString("MemberId")
            Dim PaymentLogAvailable As Boolean = False
            If SysParam.GetValue(DB, "PayflowEnabled") = 1 Then PaymentLogAvailable = True
            If SysParam.GetValue(DB, "PayPalEnabled") = 1 Then PaymentLogAvailable = True
            If SysParam.GetValue(DB, "AuthorizeNetEnabled") = 1 Then PaymentLogAvailable = True
            trPaymentLog.Visible = PaymentLogAvailable
            If Not LoggedInIsInternal Then
                If SysParam.GetValue(DB, "PasswordEx") > 0 And SysParam.GetValue(DB, "SaveCreditCardInfo") > 0 Then
                    divAuthenticate.Style.Add("display", "block")
                    divPaymentInfo.Style.Add("display", "none")
                    rfvtxtSecPassword.Enabled = True
                Else
                    divAuthenticate.Style.Add("display", "none")
                    divPaymentInfo.Style.Add("display", "block")
                    rfvtxtSecPassword.Enabled = False
                End If
            Else
                divAuthenticate.Style.Add("display", "none")
                divPaymentInfo.Style.Add("display", "none")
                divInternalUser.Style.Add("display", "block")
                rfvtxtSecPassword.Enabled = False
            End If
        End If
        Previousurl = ViewState("PrevUrl")
        ctrlCart.OrderId = OrderId
    End Sub

    Public Sub RaiseCallbackEvent(ByVal eventArgument As String) Implements System.Web.UI.ICallbackEventHandler.RaiseCallbackEvent
        Dim dbNote As New StoreOrderNoteRow(DB)
        dbNote.OrderId = OrderId
        dbNote.AdminId = LoggedInAdminId
        dbNote.Note = eventArgument
        dbNote.Insert()
    End Sub

    Public Function GetCallbackResult() As String Implements System.Web.UI.ICallbackEventHandler.GetCallbackResult
        Return ""
    End Function

	Private Sub LoadFromDB()
		If OrderId = 0 Then Exit Sub

        Dim dtCountries As DataTable = CountryRow.GetCountryList(DB)
        Dim dtStates As DataTable = StateRow.GetStateList(DB)

        txtBillingFirstName.Text = dbStoreOrder.BillingFirstName
		txtBillingLastName.Text = dbStoreOrder.BillingLastName
		txtBillingCompany.Text = dbStoreOrder.BillingCompany
		txtBillingAddress1.Text = dbStoreOrder.BillingAddress1
		txtBillingAddress2.Text = dbStoreOrder.BillingAddress2
		txtBillingCity.Text = dbStoreOrder.BillingCity

        drpBillingState.DataSource = dtStates
		drpBillingState.DataValueField = "StateCode"
		drpBillingState.DataTextField = "StateName"
		drpBillingState.DataBind()
		drpBillingState.Items.Insert(0, New ListItem("", ""))
		drpBillingState.SelectedValue = dbStoreOrder.BillingState

		txtBillingRegion.Text = dbStoreOrder.BillingRegion

        drpBillingCountry.DataSource = dtCountries
		drpBillingCountry.DataTextField = "CountryName"
		drpBillingCountry.DataValueField = "CountryCode"
		drpBillingCountry.DataBind()
		drpBillingCountry.Items.Insert(0, New ListItem("", ""))
		drpBillingCountry.SelectedValue = dbStoreOrder.BillingCountry

		txtBillingZip.Text = dbStoreOrder.BillingZip
		txtBillingPhone.Text = dbStoreOrder.BillingPhone


        ltlCardholdername.Text = IIf(dbStoreOrder.CardholderName = String.Empty, "(not saved)", dbStoreOrder.CardholderName)
        ltlCardNumber.Text = IIf(dbStoreOrder.CardNumber = String.Empty, "(not saved)", dbStoreOrder.StarredCardNumber)
        If dbStoreOrder.ExpirationDate = String.Empty Then
            ltlExpirationDate.Text = "(not saved)"
        Else
            Dim ExpirationDate As DateTime = dbStoreOrder.ExpirationDate
            ltlExpirationDate.Text = Month(ExpirationDate) & "/" & Year(ExpirationDate)
        End If
        If dbStoreOrder.CardTypeId = 0 Then
            ltlCardType.Text = "(not saved)"
        Else
            ltlCardType.Text = CreditCardTypeRow.GetRow(DB, dbStoreOrder.CardTypeId).Name
        End If
        txtEmail.Text = dbStoreOrder.Email
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub

        If Not ctrlCart.ValidateForm() Then
            Exit Sub
        End If

        ' CHECK THE BILLING ADDRESS
        If SysParam.GetValue(DB, "UPSAddressVerification") AndAlso drpBillingCountry.SelectedValue = "US" Then
            Dim ErrorDesc As String = String.Empty
            If Not UPS.ValidateAddress(txtBillingCity.Text, drpBillingState.SelectedValue, txtBillingZip.Text, drpBillingCountry.SelectedValue, ErrorDesc) Then
                AddError("Entered Billing Address is not recognized. Please verify the City, State, Zipcode and Country are correct. Make sure to spell out your full City name.")
                Exit Sub
            End If
        End If

        Try
            DB.BeginTransaction()

            SaveAndUpdateOrderNotes() 'MOVE this JKC

            Dim dbStoreOrder As StoreOrderRow = StoreOrderRow.GetRow(DB, OrderId)
            dbStoreOrder.BillingFirstName = txtBillingFirstName.Text
            dbStoreOrder.BillingLastName = txtBillingLastName.Text
            dbStoreOrder.BillingCompany = txtBillingCompany.Text
            dbStoreOrder.BillingAddress1 = txtBillingAddress1.Text
            dbStoreOrder.BillingAddress2 = txtBillingAddress2.Text
            dbStoreOrder.BillingCity = txtBillingCity.Text
            dbStoreOrder.BillingState = drpBillingState.SelectedValue
            dbStoreOrder.BillingRegion = txtBillingRegion.Text
            dbStoreOrder.BillingCountry = drpBillingCountry.SelectedValue
            dbStoreOrder.BillingZip = txtBillingZip.Text
            dbStoreOrder.BillingPhone = txtBillingPhone.Text
            dbStoreOrder.Email = txtEmail.Text
            dbStoreOrder.Update()


            DB.CommitTransaction()

            Response.Redirect(Previousurl)
            'Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Private Sub SaveAndUpdateOrderNotes()

        Dim sNotes As New StringBuilder, bUpdate As Boolean = False

        sNotes.AppendLine("Order Update:")

        If dbStoreOrder.BillingFirstName <> txtBillingFirstName.Text Then
            sNotes.AppendLine("Billing First Name changed from : '" & dbStoreOrder.BillingFirstName & "' to '" & txtBillingFirstName.Text & "'<br />")
            bUpdate = True
        End If
        If dbStoreOrder.BillingLastName <> Me.txtBillingLastName.Text Then
            sNotes.AppendLine("Billing Last Name changed from : '" & dbStoreOrder.BillingLastName & "' to '" & txtBillingLastName.Text & "'<br />")
            bUpdate = True
        End If
        If dbStoreOrder.BillingAddress1 <> txtBillingAddress1.Text Then
            sNotes.AppendLine("Billing Address 1 changed from : '" & dbStoreOrder.BillingAddress1 & "' to '" & txtBillingAddress1.Text & "'<br />")
            bUpdate = True
        End If
        If dbStoreOrder.BillingAddress2 <> txtBillingAddress2.Text Then
            sNotes.AppendLine("Billing Address 2 changed from : '" & dbStoreOrder.BillingAddress2 & "' to '" & txtBillingAddress2.Text & "'<br />")
            bUpdate = True
        End If
        If dbStoreOrder.BillingCity <> txtBillingCity.Text Then
            sNotes.AppendLine("Billing City changed from : '" & dbStoreOrder.BillingCity & "' to '" & txtBillingCity.Text & "'<br />")
            bUpdate = True
        End If
        If dbStoreOrder.BillingState <> drpBillingState.Text Then
            sNotes.AppendLine("Billing State changed from : '" & dbStoreOrder.BillingState & "' to '" & drpBillingState.SelectedValue & "'<br />")
            bUpdate = True
        End If
        If dbStoreOrder.BillingRegion <> txtBillingRegion.Text Then
            sNotes.AppendLine("Billing Region changed from : '" & dbStoreOrder.BillingRegion & "' to '" & txtBillingRegion.Text & "'<br />")
            bUpdate = True
        End If
        If dbStoreOrder.BillingZip <> txtBillingZip.Text Then
            sNotes.AppendLine("Billing Zip  changed from : '" & dbStoreOrder.BillingZip & "' to '" & txtBillingZip.Text & "'<br />")
            bUpdate = True
        End If
        If dbStoreOrder.BillingCountry <> drpBillingCountry.SelectedValue Then
            sNotes.AppendLine("Billing Country changed from : '" & dbStoreOrder.BillingCountry & "' to '" & drpBillingCountry.SelectedValue & "'<br />")
            bUpdate = True
        End If
        If dbStoreOrder.BillingPhone <> Me.txtBillingPhone.Text Then
            sNotes.AppendLine("Billing Phone changed from : '" & dbStoreOrder.BillingPhone & "' to '" & txtBillingPhone.Text & "'<br />")
            bUpdate = True
        End If
        If dbStoreOrder.Email <> Me.txtEmail.Text Then
            sNotes.AppendLine("Billing Email changed from : '" & dbStoreOrder.Email & "' to '" & Me.txtEmail.Text & "'<br />")
            bUpdate = True
        End If

        If dbStoreOrder.BillingCompany <> Me.txtBillingCompany.Text Then
            sNotes.AppendLine("Billing Company changed from : '" & dbStoreOrder.BillingCompany & "' to '" & Me.txtBillingCompany.Text & "'<br />")
            bUpdate = True
        End If

        Dim CartUpdates As String = Me.ctrlCart.SaveStatusInfo
        If CartUpdates <> String.Empty Then
            sNotes.AppendLine(CartUpdates)
            bUpdate = True
        End If


        If bUpdate Then
            Dim dbNotes As New StoreOrderNoteRow(DB)
            dbNotes.AdminId = LoggedInAdminId
            dbNotes.Note = sNotes.ToString
            dbNotes.OrderId = OrderId
            dbNotes.Insert()
        End If

    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        'Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
        Response.Redirect(Previousurl)
    End Sub

    Protected Sub btnVerify_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVerify.Click
        Dim AdminId As Integer = AdminRow.ValidateSecondaryCredentials(DB, LoggedInUsername, txtUserPass.Text)
        If AdminId <> 0 Then
            divAuthenticate.Style.Add("display", "none")
            divPaymentInfo.Style.Add("display", "block")
            Core.LogEvent("User """ & Session("Username") & """  logged in using secondary password to view credit card information ", Diagnostics.EventLogEntryType.Information)
        Else
            divAuthenticate.Style.Add("display", "block")
            divPaymentInfo.Style.Add("display", "none")
            trErrorLogin.Visible = True
            Core.LogEvent("Failed login for User """ & Session("Username") & """  on secondary authentication for creditcard information ", Diagnostics.EventLogEntryType.Warning)
        End If
    End Sub
End Class
