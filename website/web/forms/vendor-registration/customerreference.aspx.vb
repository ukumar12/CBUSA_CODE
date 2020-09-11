Imports Components
Imports DataLayer

Partial Class forms_vendor_registration_customerreference
    Inherits SitePage

    Protected VendorId As Integer
    Protected VendorAccountId As Integer
    Protected VendorRegistrationCustomerReferenceId As Integer
    Protected dbVendor As VendorRow
    Protected dbVendorAccount As VendorAccountRow
    Protected dbVendorRegistrationCustomerReference As VendorRegistrationCustomerReferenceRow
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

        If Request("VendorRegistrationCustomerReferenceId") <> String.Empty Then
            If IsNumeric(Request("VendorRegistrationCustomerReferenceId")) Then
                VendorRegistrationCustomerReferenceId = CType(Request("VendorRegistrationCustomerReferenceId"), Integer)
                dbVendorRegistrationCustomerReference = VendorRegistrationCustomerReferenceRow.GetRow(Me.DB, VendorRegistrationCustomerReferenceId)
            Else
                dbVendorRegistrationCustomerReference = New VendorRegistrationCustomerReferenceRow(Me.DB)
            End If
        Else
            dbVendorRegistrationCustomerReference = New VendorRegistrationCustomerReferenceRow(Me.DB)
        End If

        If Not IsPostBack Then

            drpState.DataSource = DataLayer.StateRow.GetStateList(DB)
            drpState.DataTextField = "StateName"
            drpState.DataValueField = "StateCode"
            drpState.DataBind()
            drpState.Items.Insert(0, New ListItem("", ""))

            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()

        If VendorRegistrationCustomerReferenceId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        With dbVendorRegistrationCustomerReference
            txtFirstName.Text = dbVendorRegistrationCustomerReference.FirstName
            txtLastName.Text = dbVendorRegistrationCustomerReference.LastName
            txtCity.Text = dbVendorRegistrationCustomerReference.City
            drpState.SelectedValue = dbVendorRegistrationCustomerReference.State
            txtZip.Text = dbVendorRegistrationCustomerReference.Zip
            txtPhone.Text = dbVendorRegistrationCustomerReference.Phone
        End With
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("register.aspx")
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If dbVendorRegistrationCustomerReference.VendorRegistrationCustomerReferenceID <> 0 Then
            dbVendorRegistrationCustomerReference.Remove()
        End If
        Response.Redirect("register.aspx")
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Page.Validate("VendorReg")
        If Not Page.IsValid Then Exit Sub

        Try
            DB.BeginTransaction()

            With dbVendorRegistrationCustomerReference
                .FirstName = txtFirstName.Text
                .LastName = txtLastName.Text
                .City = txtCity.Text
                .State = drpState.SelectedValue
                .Zip = txtZip.Text
                .Phone = txtPhone.Text
                .VendorRegistrationID = dbVendorRegistration.VendorRegistrationID
            End With

            If dbVendorRegistrationCustomerReference.VendorRegistrationCustomerReferenceID = 0 Then
                dbVendorRegistrationCustomerReference.Insert()
            Else
                dbVendorRegistrationCustomerReference.Update()
            End If

            DB.CommitTransaction()

            Response.Redirect("register.aspx")

        Catch ex As Exception
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try

    End Sub

End Class
