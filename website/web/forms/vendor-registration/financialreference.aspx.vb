Imports Components
Imports DataLayer

Partial Class forms_vendor_registration_financialreference
    Inherits SitePage

    Protected VendorId As Integer
    Protected VendorAccountId As Integer
    Protected VendorRegistrationFinancialReferenceId As Integer
    Protected dbVendor As VendorRow
    Protected dbVendorAccount As VendorAccountRow
    Protected dbVendorRegistrationFinancialReference As VendorRegistrationFinancialReferenceRow
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

        If Request("VendorRegistrationFinancialReferenceId") <> String.Empty Then
            If IsNumeric(Request("VendorRegistrationFinancialReferenceId")) Then
                VendorRegistrationFinancialReferenceId = CType(Request("VendorRegistrationFinancialReferenceId"), Integer)
                dbVendorRegistrationFinancialReference = VendorRegistrationFinancialReferenceRow.GetRow(Me.DB, VendorRegistrationFinancialReferenceId)
            Else
                dbVendorRegistrationFinancialReference = New VendorRegistrationFinancialReferenceRow(Me.DB)
            End If
        Else
            dbVendorRegistrationFinancialReference = New VendorRegistrationFinancialReferenceRow(Me.DB)
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

        If VendorRegistrationFinancialReferenceId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        With dbVendorRegistrationFinancialReference
            txtFirstName.Text = dbVendorRegistrationFinancialReference.FirstName
            txtLastName.Text = dbVendorRegistrationFinancialReference.LastName
            txtBusinessName.Text = dbVendorRegistrationFinancialReference.CompanyName
            txtCity.Text = dbVendorRegistrationFinancialReference.City
            drpState.SelectedValue = dbVendorRegistrationFinancialReference.State
            txtZip.Text = dbVendorRegistrationFinancialReference.Zip
            txtPhone.Text = dbVendorRegistrationFinancialReference.Phone
        End With
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("register.aspx")
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If dbVendorRegistrationFinancialReference.VendorRegistrationFinancialReferenceID <> 0 Then
            dbVendorRegistrationFinancialReference.Remove()
        End If
        Response.Redirect("register.aspx")
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Page.Validate("VendorReg")
        If Not Page.IsValid Then Exit Sub

        Try
            DB.BeginTransaction()

            With dbVendorRegistrationFinancialReference
                .FirstName = txtFirstName.Text
                .LastName = txtLastName.Text
                .City = txtCity.Text
                .State = drpState.SelectedValue
                .Zip = txtZip.Text
                .Phone = txtPhone.Text
                .CompanyName = txtBusinessName.Text
                .VendorRegistrationID = dbVendorRegistration.VendorRegistrationID
            End With

            If dbVendorRegistrationFinancialReference.VendorRegistrationFinancialReferenceID = 0 Then
                dbVendorRegistrationFinancialReference.Insert()
            Else
                dbVendorRegistrationFinancialReference.Update()
            End If

            DB.CommitTransaction()

            Response.Redirect("register.aspx")

        Catch ex As Exception
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try

    End Sub

End Class
