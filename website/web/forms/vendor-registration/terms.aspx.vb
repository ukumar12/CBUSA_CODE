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
            LoadFromDB()
        End If

    End Sub

    Private Sub LoadFromDB()

    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click

        If dbVendorRegistration.VendorRegistrationID = 0 Or dbVendorAccount.VendorAccountID = 0 Then
            Exit Sub
        End If

        dbVendorRegistration.AcceptsTerms = True
        dbVendorRegistration.PreparerFirstName = dbVendorAccount.FirstName
        dbVendorRegistration.PreparerLastName = dbVendorAccount.LastName
        dbVendorRegistration.Update()

        Dim sBody As String = String.Empty
        Dim sRegistrationMessage As String = String.Empty
        Dim Email As String = SysParam.GetValue(DB, "VendorRegistrationEmail")

        sBody &= "Company Name: " & dbVendor.CompanyName & vbCrLf
        sBody &= "Address: " & dbVendor.Address & vbCrLf
        sBody &= "Address2: " & dbVendor.Address2 & vbCrLf
        sBody &= "City: " & dbVendor.City & vbCrLf
        sBody &= "State: " & dbVendor.State & vbCrLf
        sBody &= "Zip: " & dbVendor.Zip & vbCrLf
        sBody &= "Email: " & dbVendor.Email & vbCrLf

        sRegistrationMessage = "The following vendor has a registration that is awaiting approval." & vbCrLf
        sRegistrationMessage &= vbCrLf
        sRegistrationMessage &= sBody & vbCrLf

        Core.SendSimpleMail(Email, SysParam.GetValue(DB, "VendorRegistrationEmailName"), Email, SysParam.GetValue(DB, "VendorRegistrationEmailName"), SysParam.GetValue(DB, "VendorRegistrationApprovalSubjectAdmin"), sRegistrationMessage)

        Response.Redirect("users.aspx")

    End Sub

End Class
