Imports Components
Imports DataLayer
Imports Utilities

Partial Class forms_vendor_registration_companyinfo
    Inherits SitePage

    Protected VendorID As Integer

    Protected dbVendor As VendorRow
    Protected dbVendorAccount As VendorAccountRow
    Protected dbVendorRegistration As VendorRegistrationRow
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        VendorID = Session("VendorID")
        If VendorID = Nothing Then
            dbVendor = VendorRow.GetVendorByGuid(DB, Request("guid"))
            VendorID = dbVendor.VendorID
        Else
            dbVendor = VendorRow.GetRow(DB, VendorID)
        End If

        dbVendorAccount = VendorAccountRow.GetRow(DB, Session("VendorAccountId"))
        dbVendorRegistration = VendorRegistrationRow.GetRowByVendor(DB, VendorID)

        If dbVendorRegistration.CompleteDate <> Nothing Then
            btnContinueElectronic.Visible = False
            btnDashboard.Visible = True
            ctlSteps.Visible = False
            btnBack.Text = "Cancel"
            treula.Visible = False
            'Response.Redirect("/vendor/default.aspx")
        ElseIf dbVendorRegistration.VendorRegistrationID = Nothing Or dbVendorAccount.VendorAccountID = Nothing Or dbVendor.VendorID = Nothing Then
            Response.Redirect("/default.aspx")
        Else
            btnContinueElectronic.Visible = True
            btnDashboard.Visible = False
            ctlSteps.Visible = True
            btnBack.Text = "Go Back"
        End If

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorId")
        UserName = Session("Username")

        If Not IsPostBack Then
            LoadFromDB()
         Core.DataLog("Edit Company Information", PageURL, CurrentUserId, "Vendor Left Menu Click", "", "", "", "", UserName)
        End If
    End Sub

    Private Sub LoadFromDB()
        drpBusinessType.DataSource = BusinessTypeRow.GetList(DB, "BusinessType")
        drpBusinessType.DataTextField = "BusinessType"
        drpBusinessType.DataValueField = "BusinessTypeId"
        drpBusinessType.DataBind()
        drpBusinessType.Items.Insert(0, New ListItem("", ""))


        txtNumEmployees.Text = dbVendorRegistration.Employees
        txtStartYear.Text = dbVendorRegistration.YearStarted
        txtSubsidiary.Text = dbVendorRegistration.SubsidiaryExplanation
        txtSupplyArea.Text = dbVendorRegistration.SupplyArea
        txtWebsiteUrl.Text = dbVendor.WebsiteURL
        fuLogoFile.CurrentFileName = dbVendor.LogoFile
        rbSubsidiaryYes.Checked = dbVendorRegistration.IsSubsidiary
        rbSubsidiaryNo.Checked = Not rbSubsidiaryYes.Checked
        If dbVendorRegistration.AcceptsTerms = True Then
            cbAsp.Checked = True
        End If
        If rbSubsidiaryNo.Checked Then
            divSubsidiary.Style.Add("display", "none")
        End If
        drpBusinessType.SelectedValue = dbVendorRegistration.BusinessType
    End Sub

    Private Function Process() As Boolean
        Page.Validate("VendorReg")
        If Not Page.IsValid Then
            Return False
        End If

        DB.BeginTransaction()
        Try
            dbVendor.WebsiteURL = txtWebsiteUrl.Text
            Dim url As String = txtWebsiteUrl.Text
            If Not (url.ToLower.Contains("http://") Or url.ToLower.Contains("https://")) Then
                url = "http://" & url
            End If
            dbVendor.WebsiteURL = url

            If fuLogoFile.NewFileName <> String.Empty Then
                fuLogoFile.SaveNewFile()
                dbVendor.LogoFile = fuLogoFile.NewFileName
                Dim VendorLogoMaxWidth As Integer = CType(SysParam.GetValue(DB, "VendorLogoMaxWidth"), Integer)
                Dim VendorLogoMaxHeight As Integer = CType(SysParam.GetValue(DB, "VendorLogoMaxHeight"), Integer)
                Core.ResizeImageWithQuality(Server.MapPath("/assets/vendor/logo/original/" & dbVendor.LogoFile), Server.MapPath("/assets/vendor/logo/standard/" & dbVendor.LogoFile), VendorLogoMaxWidth, VendorLogoMaxHeight, 100)
            ElseIf fuLogoFile.MarkedToDelete Then
                dbVendor.LogoFile = Nothing
            End If

            dbVendor.Update()
            '' Salesforce Integration
            'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
            'If sfHelper.Login() Then
            '    If sfHelper.UpsertVendor(dbVendor) = False Then
            '        'throw error
            '    End If
            'End If

            With dbVendorRegistration
                .Employees = txtNumEmployees.Text
                .YearStarted = txtStartYear.Text
                .SubsidiaryExplanation = txtSubsidiary.Text
                .SupplyArea = txtSupplyArea.Text
                .IsSubsidiary = rbSubsidiaryYes.Checked
                .BusinessType = drpBusinessType.SelectedValue
                .AcceptsTerms = cbAsp.Checked
            End With
            If dbVendorRegistration.CompleteDate = Nothing Then
                Dim dbStat As RegistrationStatusRow = RegistrationStatusRow.GetStatusByStep(DB, 1)
                dbVendorRegistration.RegistrationStatusID = dbStat.RegistrationStatusID
            End If
            dbVendorRegistration.Update()

            DB.CommitTransaction()
            'log Update Company Profile 
            Core.DataLog("Edit Company Profile", PageURL, CurrentUserId, "Update Company Profile", "", "", "", "", UserName)
            'end log
            Return True
        Catch ex As SqlClient.SqlException
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
            Return False
        End Try

    End Function

    Protected Sub btnContinueElectronic_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnContinueElectronic.Click
        If Process() Then
            Response.Redirect("register.aspx?guid=" & dbVendor.GUID)
        End If

    End Sub

    Protected Sub btnDashboard_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDashboard.Click
        'log Btn Save Changes 
        Core.DataLog("Edit Company Profile", PageURL, CurrentUserId, "Btn Save Changes", "", "", "", "", UserName)
        'end log
        If Process() Then
            Response.Redirect("/vendor/default.aspx")
        End If
    End Sub

   ' Protected Sub btnGoToDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnGoToDashBoard.Click
       ' Response.Redirect("/vendor/default.aspx")
   ' End Sub
End Class
