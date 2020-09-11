Imports Components
Imports DataLayer

Partial Class forms_vendor_registration_memberreference
    Inherits SitePage

    Protected VendorId As Integer
    Protected VendorAccountId As Integer
    Protected VendorRegistrationMemberReferenceId As Integer
    Protected dbVendor As VendorRow
    Protected dbVendorAccount As VendorAccountRow
    Protected dbVendorRegistrationMemberReference As VendorRegistrationMemberReferenceRow
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

        If Request("VendorRegistrationMemberReferenceId") <> String.Empty Then
            If IsNumeric(Request("VendorRegistrationMemberReferenceId")) Then
                VendorRegistrationMemberReferenceId = CType(Request("VendorRegistrationMemberReferenceId"), Integer)
                dbVendorRegistrationMemberReference = VendorRegistrationMemberReferenceRow.GetRow(Me.DB, VendorRegistrationMemberReferenceId)
            Else
                dbVendorRegistrationMemberReference = New VendorRegistrationMemberReferenceRow(Me.DB)
            End If
        Else
            dbVendorRegistrationMemberReference = New VendorRegistrationMemberReferenceRow(Me.DB)
        End If

        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()

        If VendorRegistrationMemberReferenceId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        With dbVendorRegistrationMemberReference
            txtFirstName.Text = dbVendorRegistrationMemberReference.FirstName
            txtLastName.Text = dbVendorRegistrationMemberReference.LastName
            txtBusinessName.Text = dbVendorRegistrationMemberReference.CompanyName
        End With
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("register.aspx")
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If dbVendorRegistrationMemberReference.VendorRegistrationMemberReferenceID <> 0 Then
            dbVendorRegistrationMemberReference.Remove()
        End If
        Response.Redirect("register.aspx")
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not Page.IsValid Then Exit Sub

        Try
            DB.BeginTransaction()

            With dbVendorRegistrationMemberReference
                .FirstName = txtFirstName.Text
                .LastName = txtLastName.Text
                .CompanyName = txtBusinessName.Text
                .VendorRegistrationID = dbVendorRegistration.VendorRegistrationID
            End With

            If dbVendorRegistrationMemberReference.VendorRegistrationMemberReferenceID = 0 Then
                dbVendorRegistrationMemberReference.Insert()
            Else
                dbVendorRegistrationMemberReference.Update()
            End If

            DB.CommitTransaction()

            Response.Redirect("register.aspx")

        Catch ex As Exception
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try

    End Sub

End Class
