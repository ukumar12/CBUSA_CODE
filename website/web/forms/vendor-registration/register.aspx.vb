Imports Components
Imports DataLayer
Imports Utilities

Partial Class forms_vendor_registration_register
    Inherits SitePage

    Protected VendorId As Integer
    Protected VendorAccountId As Integer
    Protected dbVendor As VendorRow
    Protected dbVendorAccount As VendorAccountRow
    Protected dbVendorRegistration As VendorRegistrationRow
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("VendorId") <> 0 Then
            VendorId = CType(Session("VendorId"), Integer)
            dbVendor = VendorRow.GetRow(Me.DB, VendorId)
            dbVendorRegistration = VendorRegistrationRow.GetRowByVendor(Me.DB, VendorId)
        Else
            Response.Redirect("/default.aspx")
        End If

        If dbVendorRegistration.CompleteDate <> Nothing Then
            ctlSteps.Visible = False
            btnContinue.Visible = False
            btnBack.Text = "Cancel"
        Else
            btnDashboard.Visible = False
            btnBack.Text = "Go Back"
        End If

        If Session("VendorAccountId") <> 0 Then
            VendorAccountId = CType(Session("VendorAccountId"), Integer)
            dbVendorAccount = VendorAccountRow.GetRow(Me.DB, VendorAccountId)
        Else
            Response.Redirect("/default.aspx")
        End If

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorId")
        UserName = Session("Username")

        If Not IsPostBack Then
         Core.DataLog("Edit Directory Information", PageURL, CurrentUserId, "Vendor Left Menu Click", "", "", "", "", UserName)
            LoadFromDB()
        End If

    End Sub

    Private Sub LoadFromDB()

        If dbVendor.VendorID = 0 Then
            Exit Sub
        End If

        With dbVendor
            txtServices.Text = .ServicesOffered
            txtSpecialty.Text = .SpecialtyServices
            txtDiscounts.Text = .Discounts
            txtTerms.Text = .PaymentTerms
            txtRebate.Text = .RebateProgram
            txtAcceptedCards.Text = .AcceptedCards
            txtBrands.Text = .BrandsSupplied
            If .AcceptedCards <> String.Empty Then
                rbAcceptCardsYes.Checked = True
                divCardTypes.Style("display") = ""
            Else
                rbAcceptCardsNo.Checked = True
                divCardTypes.Style("display") = "none"
            End If
        End With

    End Sub

    Private Function SaveForm() As Boolean
        dbVendor.ServicesOffered = txtServices.Text
        dbVendor.SpecialtyServices = txtSpecialty.Text
        dbVendor.Discounts = txtDiscounts.Text
        dbVendor.PaymentTerms = txtTerms.Text
        dbVendor.RebateProgram = txtRebate.Text
        dbVendor.BrandsSupplied = txtBrands.Text
        If rbAcceptCardsYes.Checked Then
            dbVendor.AcceptedCards = txtAcceptedCards.Text
        Else
            dbVendor.AcceptedCards = Nothing
        End If

        DB.BeginTransaction()
        Try

            dbVendor.Update()

            '' Salesforce Integration
            'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
            'If sfHelper.Login() Then
            '    If sfHelper.UpsertVendor(dbVendor) = False Then
            '        'throw error
            '    End If

            'End If
            If dbVendorRegistration.CompleteDate = Nothing OrElse dbVendorRegistration.CompleteDate.Year <> Now.Year Then
                Dim dbStat As RegistrationStatusRow = RegistrationStatusRow.GetStatusByStep(DB, 2)
                dbVendorRegistration.RegistrationStatusID = dbStat.RegistrationStatusID
                dbVendorRegistration.Update()
            End If

            DB.CommitTransaction()
        Catch ex As SqlClient.SqlException
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
            Return False
        End Try
        Return True
    End Function

    Protected Sub btnContinue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnContinue.Click
        If Not IsValid Then
            Exit Sub
        End If
        If SaveForm() Then
            Response.Redirect("users.aspx?guid=" & dbVendor.GUID)
        End If
    End Sub

    Protected Sub btnDashboard_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDashboard.Click
        If Not IsValid Then
            Exit Sub
        End If
        'log Btn Save Changes 
        Core.DataLog("Edit Directory Information", PageURL, CurrentUserId, "Btn Save Changes", "", "", "", "", UserName)
        'end log
        If SaveForm() Then
            Response.Redirect("/vendor/default.aspx")
        End If
    End Sub

   ' Protected Sub btnGoToDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnGoToDashBoard.Click
       ' Response.Redirect("/vendor/default.aspx")
   ' End Sub

End Class
