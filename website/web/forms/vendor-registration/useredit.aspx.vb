Imports Components
Imports DataLayer

Partial Class forms_vendor_registration_terms
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

            Me.cblUserRoles.DataSource = VendorRoleRow.GetList(Me.DB)
            Me.cblUserRoles.DataTextField = "VendorRole"
            Me.cblUserRoles.DataValueField = "VendorRoleId"
            Me.cblUserRoles.DataBind()

            LoadFromDB()
        End If

    End Sub

    Private Sub LoadFromDB()

        If VendorAccountId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        With dbVendorAccount
            txtFirstName.Text = dbVendorAccount.FirstName
            txtLastName.Text = dbVendorAccount.LastName
            chkIsPrimary.Checked = dbVendorAccount.IsPrimary
            cblUserRoles.SelectedValues = dbVendorAccount.GetSelectedVendorRoles
        End With

    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("users.aspx")
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If dbVendorAccount.VendorAccountID <> 0 Then
            dbVendorAccount.Remove()
        End If

        Response.Redirect("users.aspx")
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Page.Validate("UserReg")
        If Not Page.IsValid Then Exit Sub

        Try
            DB.BeginTransaction()

            With dbVendorAccount
                .FirstName = txtFirstName.Text
                .LastName = txtLastName.Text
                If chkIsPrimary.Checked And Not dbVendorAccount.IsPrimary Then
                    dbVendorAccount.SetAsPrimary(VendorId)
                End If
                .IsPrimary = chkIsPrimary.Checked
            End With

            If dbVendorAccount.VendorAccountID = 0 Then
                dbVendorAccount.Insert()
            Else
                dbVendorAccount.Update()
            End If

            dbVendorAccount.DeleteFromAllVendorRoles()
            dbVendorAccount.InsertToVendorRoles(cblUserRoles.SelectedValues)

            DB.CommitTransaction()

            Response.Redirect("users.aspx")

        Catch ex As Exception
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try

    End Sub

    'Protected Sub cvtxtUsername_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles cvftxtUsername.ServerValidate
    '    Dim Username As String = DB.ExecuteScalar("SELECT Username FROM VendorAccount WHERE Username = " & DB.Quote(txtUsername.Text))
    '    args.IsValid = (Username = String.Empty)
    'End Sub

End Class
