Imports Components
Imports DataLayer

Partial Class forms_vendor_registration_businessreference
    Inherits SitePage

    Protected VendorId As Integer
    Protected VendorAccountId As Integer
    Protected VendorOwnerId As Integer
    Protected dbVendor As VendorRow
    Protected dbVendorAccount As VendorAccountRow
    Protected dbVendorOwner As VendorOwnersRow
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

        If Request("VendorOwnerId") <> String.Empty Then
            If IsNumeric(Request("VendorOwnerId")) Then
                VendorOwnerId = CType(Request("VendorOwnerId"), Integer)
                dbVendorOwner = VendorOwnersRow.GetRow(Me.DB, VendorOwnerId)
            Else
                dbVendorOwner = New VendorOwnersRow(Me.DB)
            End If
        Else
            dbVendorOwner = New VendorOwnersRow(Me.DB)
        End If

        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()

        If VendorId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        With dbVendorOwner
            txtFirstName.Text = dbVendorOwner.FirstName
            txtLastName.Text = dbVendorOwner.LastName
            txtPhone.Text = dbVendorOwner.Phone
        End With
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("register.aspx")
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If dbVendorOwner.VendorOwnerID <> 0 Then
            dbVendorOwner.Remove()
        End If
        Response.Redirect("register.aspx")
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Page.Validate("VendorReg")
        If Not Page.IsValid Then Exit Sub

        Try
            DB.BeginTransaction()

            With dbVendorOwner
                .FirstName = txtFirstName.Text
                .LastName = txtLastName.Text
                .Phone = txtPhone.Text
                .VendorID = dbVendor.VendorID
            End With

            If dbVendorOwner.VendorOwnerID = 0 Then
                dbVendorOwner.Insert()
            Else
                dbVendorOwner.Update()
            End If

            DB.CommitTransaction()

            Response.Redirect("register.aspx")

        Catch ex As Exception
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try

    End Sub

End Class
