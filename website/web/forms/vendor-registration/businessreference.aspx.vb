Imports Components
Imports DataLayer

Partial Class forms_vendor_registration_businessreference
    Inherits SitePage

    Protected VendorId As Integer
    Protected VendorAccountId As Integer
    Protected VendorRegistrationBusinessReferenceId As Integer
    Protected dbVendor As VendorRow
    Protected dbVendorAccount As VendorAccountRow
    Protected dbVendorRegistrationBusinessReference As VendorRegistrationBusinessReferenceRow
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

        If Request("VendorRegistrationBusinessReferenceId") <> String.Empty Then
            If IsNumeric(Request("VendorRegistrationBusinessReferenceId")) Then
                VendorRegistrationBusinessReferenceId = CType(Request("VendorRegistrationBusinessReferenceId"), Integer)
                dbVendorRegistrationBusinessReference = VendorRegistrationBusinessReferenceRow.GetRow(Me.DB, VendorRegistrationBusinessReferenceId)
            Else
                dbVendorRegistrationBusinessReference = New VendorRegistrationBusinessReferenceRow(Me.DB)
            End If
        Else
            dbVendorRegistrationBusinessReference = New VendorRegistrationBusinessReferenceRow(Me.DB)
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

        If VendorRegistrationBusinessReferenceId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        With dbVendorRegistrationBusinessReference
            txtFirstName.Text = dbVendorRegistrationBusinessReference.FirstName
            txtLastName.Text = dbVendorRegistrationBusinessReference.LastName
            txtBusinessName.Text = dbVendorRegistrationBusinessReference.CompanyName
            txtCity.Text = dbVendorRegistrationBusinessReference.City
            drpState.SelectedValue = dbVendorRegistrationBusinessReference.State
            txtZip.Text = dbVendorRegistrationBusinessReference.Zip
            txtPhone.Text = dbVendorRegistrationBusinessReference.Phone
        End With
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("register.aspx")
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If dbVendorRegistrationBusinessReference.VendorRegistrationBusinessReferenceID <> 0 Then
            dbVendorRegistrationBusinessReference.Remove()
        End If
        Response.Redirect("register.aspx")
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Page.Validate("VendorReg")
        If Not Page.IsValid Then Exit Sub

        Try
            DB.BeginTransaction()

            With dbVendorRegistrationBusinessReference
                .FirstName = txtFirstName.Text
                .LastName = txtLastName.Text
                .City = txtCity.Text
                .State = drpState.SelectedValue
                .Zip = txtZip.Text
                .Phone = txtPhone.Text
                .CompanyName = txtBusinessName.Text
                .VendorRegistrationID = dbVendorRegistration.VendorRegistrationID
            End With

            If dbVendorRegistrationBusinessReference.VendorRegistrationBusinessReferenceID = 0 Then
                dbVendorRegistrationBusinessReference.Insert()
            Else
                dbVendorRegistrationBusinessReference.Update()
            End If

            DB.CommitTransaction()

            Response.Redirect("register.aspx")

        Catch ex As Exception
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try

    End Sub

End Class
