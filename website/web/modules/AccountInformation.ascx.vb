Option Strict Off

Imports Components
Imports DataLayer
Imports Utilities


Partial Class AccountInformation
    Inherits ModuleControl

    Private AccountId As Integer = 0
    Protected BuilderAccountID As Integer
    Protected VendorAccountID As Integer
    Private PageUrl As String = ""
    Private CurrentUserId As String = ""
    Private UserName As String = ""

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        Dim SQL As String = String.Empty
        Dim dt As DataTable
        Dim dbVendor As VendorRow
        Dim dbPIQ As PIQRow

        Try

            If Not IsAdminDisplay Then

                If CType(Me.Page, SitePage).IsLoggedInBuilder Then
                    LoadBuilderInfo()
                    AccountId = Session("BuilderAccountId")
                    hdnAccountType.Value = "B"
                    'SQL = BuilderRoleSQL()
                ElseIf CType(Me.Page, SitePage).IsLoggedInVendor Then
                    LoadVendorInfo()
                    AccountId = Session("VendorAccountId")
                    hdnAccountType.Value = "V"
                    'SQL = VendorRoleSQL()
                ElseIf CType(Me.Page, SitePage).IsLoggedInPIQ Then
                    LoadPIQInfo()
                    AccountId = Session("PIQAccountId")
                    hdnAccountType.Value = "P"
                    'SQL = PIQRoleSQL()
                End If

                'If SQL <> String.Empty Then
                '    dt = Me.DB.GetDataTable(SQL)
                '    Me.rptRoles.DataSource = dt
                '    Me.rptRoles.DataBind()
                'End If

            Else
                Me.rfvCompanyName.Enabled = False
                Me.rfvAddress.Enabled = False
                Me.rfvCity.Enabled = False
                Me.rfvState.Enabled = False
                Me.rfvZip.Enabled = False
                Me.rfvPhone.Enabled = False
                Me.rfvEmail.Enabled = False
                Me.lnkvWebsiteURL.Enabled = False
            End If

        Catch ex As Exception

        End Try

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        PageUrl = Request.Url.ToString()
    End Sub

    Private Sub LoadBuilderInfo()

        Dim BuilderAccount As DataLayer.BuilderAccountRow
        Dim Builder As DataLayer.BuilderRow

        BuilderAccount = DataLayer.BuilderAccountRow.GetRow(Me.DB, Session("BuilderAccountId"))
        Builder = DataLayer.BuilderRow.GetRow(Me.DB, BuilderAccount.BuilderID)

        Me.ltdUserName.Text = BuilderAccount.FirstName & " " & BuilderAccount.LastName
        Me.ltdCompanyName.Text = Builder.CompanyName

        Me.divAddress.InnerHtml = Builder.Address & "<br/>"
        If Builder.Address2 <> String.Empty Then
            Me.divAddress.InnerHtml &= Builder.Address2 & "<br/>"
        End If
        Me.divAddress.InnerHtml &= Builder.City & ", " & Builder.State & " " & Builder.Zip

        'Only allow for admins to edit the company
        Me.divlink.Visible = True
        lblHeader.Text = "Company Information"
        txtCompanyName.Text = Builder.CompanyName
        txtWebsiteURL.Text = Builder.WebsiteURL
        txtAddress.Text = Builder.Address
        txtAddress2.Text = Builder.Address2
        txtCity.Text = Builder.City
        txtState.Text = Builder.State
        txtZip.Text = Builder.Zip
        txtCompanyFax.Text = Builder.Fax
        txtCompanyPhone.Text = Builder.Phone
        txtCompanyEmail.Text = Builder.Email

        txtFirstName.Text = BuilderAccount.FirstName
        txtLastName.Text = BuilderAccount.LastName
        txtPhone.Text = BuilderAccount.Phone
        txtMobile.Text = BuilderAccount.Mobile
        txtFax.Text = BuilderAccount.Fax
        txtEmail.Text = BuilderAccount.Email
        txtUserName.Text = BuilderAccount.Username
        trLogo.Visible = False

    End Sub

    Private Sub LoadVendorInfo()

        Dim VendorAccount As DataLayer.VendorAccountRow
        Dim Vendor As DataLayer.VendorRow

        VendorAccount = DataLayer.VendorAccountRow.GetRow(Me.DB, Session("VendorAccountId"))
        Vendor = DataLayer.VendorRow.GetRow(Me.DB, VendorAccount.VendorID)

        If Not Vendor.LogoFile = String.Empty Then
            imgLogo.Src = "/assets/vendor/logo/standard/" & Vendor.LogoFile
            imgLogo.Alt = Vendor.CompanyName
            imgLogo.Visible = True
        End If

        Me.ltdUserName.Text = VendorAccount.FirstName & " " & VendorAccount.LastName
        Me.ltdCompanyName.Text = Vendor.CompanyName

        Me.divAddress.InnerHtml = Vendor.Address & "<br/>"
        If Vendor.Address2 <> String.Empty Then
            Me.divAddress.InnerHtml &= Vendor.Address2 & "<br/>"
        End If
        Me.divAddress.InnerHtml &= Vendor.City & ", " & Vendor.State & " " & Vendor.Zip

        'Only allow for admins to edit the company
        Me.divlink.Visible = True
        lblHeader.Text = "Vendor Information"
        txtCompanyName.Text = Vendor.CompanyName
        txtAddress.Text = Vendor.Address
        txtAddress2.Text = Vendor.Address2
        txtCity.Text = Vendor.City
        txtState.Text = Vendor.State
        txtZip.Text = Vendor.Zip
        txtWebsiteURL.Text = Vendor.WebsiteURL
        txtCompanyFax.Text = Vendor.Fax
        txtCompanyPhone.Text = Vendor.Phone
        txtCompanyEmail.Text = Vendor.Email
        fuLogoFile.CurrentFileName = Vendor.LogoFile
        fuLogoFile.Folder = "/assets/vendor/logo/original/"
        fuLogoFile.ImageDisplayFolder = "/assets/vendor/logo/standard/"

        txtFirstName.Text = VendorAccount.FirstName
        txtLastName.Text = VendorAccount.LastName
        txtPhone.Text = VendorAccount.Phone
        txtMobile.Text = VendorAccount.Mobile
        txtFax.Text = VendorAccount.Fax
        txtEmail.Text = VendorAccount.Email
        txtUserName.Text = VendorAccount.Username


    End Sub

    Private Sub LoadPIQInfo()

        Dim PIQAccount As DataLayer.PIQAccountRow
        Dim PIQ As DataLayer.PIQRow

        PIQAccount = DataLayer.PIQAccountRow.GetRow(Me.DB, Session("PIQAccountId"))
        PIQ = DataLayer.PIQRow.GetRow(Me.DB, PIQAccount.PIQID)

        If Not PIQ.LogoFile = String.Empty Then
            imgLogo.Src = "/assets/piq/logo/standard/" & PIQ.LogoFile
            imgLogo.Alt = PIQ.CompanyName
            imgLogo.Visible = True
        End If

        Me.ltdUserName.Text = PIQAccount.FirstName & " " & PIQAccount.LastName
        Me.ltdCompanyName.Text = PIQ.CompanyName

        Me.divAddress.InnerHtml = PIQ.Address & "<br/>"
        If PIQ.Address2 <> String.Empty Then
            Me.divAddress.InnerHtml &= PIQ.Address2 & "<br/>"
        End If
        Me.divAddress.InnerHtml &= PIQ.City & ", " & PIQ.State & " " & PIQ.Zip

        'Only allow for admins to edit the company
        Me.divlink.Visible = True

        txtCompanyName.Text = PIQ.CompanyName
        txtAddress.Text = PIQ.Address
        txtAddress2.Text = PIQ.Address2
        txtCity.Text = PIQ.City
        txtState.Text = PIQ.State
        txtZip.Text = PIQ.Zip
        txtCompanyPhone.Text = PIQ.Phone
        txtCompanyFax.Text = PIQ.Fax
        txtCompanyEmail.Text = PIQ.Email
        txtWebsiteURL.Text = PIQ.WebsiteURL
        txtCompanyEmail.Text = PIQ.Email
        fuLogoFile.CurrentFileName = PIQ.LogoFile
        fuLogoFile.Folder = "/assets/piq/logo/original/"
        fuLogoFile.ImageDisplayFolder = "/assets/piq/logo/standard/"

        txtFirstName.Text = PIQAccount.FirstName
        txtLastName.Text = PIQAccount.LastName
        txtPhone.Text = PIQAccount.Phone
        txtMobile.Text = PIQAccount.Mobile
        txtFax.Text = PIQAccount.Fax
        txtEmail.Text = PIQAccount.Email
        txtUserName.Text = PIQAccount.Username

    End Sub

    Protected Sub btnSaveInfo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click

        Try

            If CType(Me.Page, SitePage).IsLoggedInBuilder Then
                SaveBuilderInfo()
            ElseIf CType(Me.Page, SitePage).IsLoggedInVendor Then
                SaveVendorInfo()
            ElseIf CType(Me.Page, SitePage).IsLoggedInPIQ Then
                SavePIQInfo()
            Else
                Response.Redirect("/default.aspx")
            End If
            Response.Redirect(Request.Url.AbsoluteUri)

        Catch ex As Exception

        End Try

    End Sub

    Private Sub SaveBuilderInfo()

        Dim BuilderAccount As DataLayer.BuilderAccountRow
        Dim Builder As DataLayer.BuilderRow

        BuilderAccount = DataLayer.BuilderAccountRow.GetRow(Me.DB, Session("BuilderAccountId"))
        Builder = DataLayer.BuilderRow.GetRow(Me.DB, BuilderAccount.BuilderID)

        Builder.CompanyName = txtCompanyName.Text
        Builder.Address = txtAddress.Text
        Builder.Address2 = txtAddress2.Text
        Builder.City = txtCity.Text
        Builder.State = txtState.Text
        Builder.Zip = txtZip.Text
        Builder.WebsiteURL = txtWebsiteURL.Text
        Builder.Phone = txtCompanyPhone.Text
        Builder.Fax = txtCompanyFax.Text
        Builder.Email = txtCompanyEmail.Text

        BuilderAccount.FirstName = txtFirstName.Text
        BuilderAccount.LastName = txtLastName.Text
        BuilderAccount.Phone = txtPhone.Text
        BuilderAccount.Mobile = txtMobile.Text
        BuilderAccount.Fax = txtFax.Text
        BuilderAccount.Email = txtEmail.Text
        BuilderAccount.Username = txtUserName.Text

        If Not txtPassword.Text = String.Empty Then

            'Log password Change
            If Not BuilderAccount.Password = txtPassword.Text Then
                CurrentUserId = Session("BuilderAccountId")
                UserName = Session("UserName")
                Core.DataLog("Edit Account Information", PageUrl, CurrentUserId, "Builder Password Change", "", "", "", "", UserName)
            End If
            'End log
            BuilderAccount.Password = txtPassword.Text
            BuilderAccount.RequirePasswordUpdate = False
        End If

        Builder.Update()
        BuilderAccount.Update()
        '' Salesforce Integration
        'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
        'If sfHelper.Login() Then
        '    If sfHelper.UpsertBuilder(Builder) = False Then
        '        'throw error
        '    End If
        '    If sfHelper.UpsertBuilderAccount(BuilderAccount) = False Then
        '        'throw error
        '    End If

        'End If

    End Sub

    Private Sub SaveVendorInfo()

        Dim VendorAccount As DataLayer.VendorAccountRow
        Dim Vendor As DataLayer.VendorRow

        VendorAccount = DataLayer.VendorAccountRow.GetRow(Me.DB, Session("VendorAccountId"))
        Vendor = DataLayer.VendorRow.GetRow(Me.DB, VendorAccount.VendorID)

        Vendor.CompanyName = txtCompanyName.Text
        Vendor.Address = txtAddress.Text
        Vendor.Address2 = txtAddress2.Text
        Vendor.City = txtCity.Text
        Vendor.State = txtState.Text
        Vendor.Zip = txtZip.Text
        Vendor.WebsiteURL = txtWebsiteURL.Text
        Vendor.Phone = txtCompanyPhone.Text
        Vendor.Fax = txtCompanyFax.Text
        Vendor.Email = txtCompanyEmail.Text

        VendorAccount.FirstName = txtFirstName.Text
        VendorAccount.LastName = txtLastName.Text
        VendorAccount.Phone = txtPhone.Text
        VendorAccount.Mobile = txtMobile.Text
        VendorAccount.Fax = txtFax.Text
        VendorAccount.Email = txtEmail.Text
        VendorAccount.Username = txtUserName.Text

        If Not txtPassword.Text = String.Empty Then

            'Log password Change
            If Not VendorAccount.Password = txtPassword.Text Then
                CurrentUserId = Session("VendorAccountId")
                UserName = Session("UserName")
                Core.DataLog("Edit Account Information", PageUrl, CurrentUserId, "Vendor Password Change", "", "", "", "", UserName)
            End If
            'End log
            VendorAccount.Password = txtPassword.Text
            VendorAccount.RequirePasswordUpdate = False
        End If
        fuLogoFile.Folder = "/assets/vendor/logo/original/"

        If fuLogoFile.NewFileName <> String.Empty Then
            fuLogoFile.SaveNewFile()
            Vendor.LogoFile = fuLogoFile.NewFileName
            Dim VendorLogoMaxWidth As Integer = CType(SysParam.GetValue(DB, "VendorLogoMaxWidth"), Integer)
            Dim VendorLogoMaxHeight As Integer = CType(SysParam.GetValue(DB, "VendorLogoMaxHeight"), Integer)
            Core.ResizeImageWithQuality(Server.MapPath("/assets/vendor/logo/original/" & Vendor.LogoFile), Server.MapPath("/assets/vendor/logo/standard/" & Vendor.LogoFile), VendorLogoMaxWidth, VendorLogoMaxHeight, 100)
        ElseIf fuLogoFile.MarkedToDelete Then
            Vendor.LogoFile = Nothing
        End If

        Vendor.Update()
        VendorAccount.Update()

        '' Salesforce Integration
        'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
        'If sfHelper.Login() Then
        '    If sfHelper.UpsertVendor(Vendor) = False Then
        '        'throw error
        '    End If
        '    If sfHelper.UpsertVendorAccount(VendorAccount) = False Then
        '        'throw error
        '    End If
        'End If

    End Sub

    Private Sub SavePIQInfo()

        Dim PIQAccount As DataLayer.PIQAccountRow
        Dim PIQ As DataLayer.PIQRow

        PIQAccount = DataLayer.PIQAccountRow.GetRow(Me.DB, Session("PIQAccountId"))
        PIQ = DataLayer.PIQRow.GetRow(Me.DB, PIQAccount.PIQID)

        PIQ.CompanyName = txtCompanyName.Text
        PIQ.Address = txtAddress.Text
        PIQ.Address2 = txtAddress2.Text
        PIQ.City = txtCity.Text
        PIQ.State = txtState.Text
        PIQ.Zip = txtZip.Text
        PIQ.Phone = txtCompanyPhone.Text
        PIQ.Fax = txtCompanyFax.Text
        PIQ.WebsiteURL = txtWebsiteURL.Text
        PIQ.Email = txtCompanyEmail.Text

        fuLogoFile.Folder = "/assets/piq/logo/original/"

        If fuLogoFile.NewFileName <> String.Empty Then
            fuLogoFile.SaveNewFile()
            PIQ.LogoFile = fuLogoFile.NewFileName
            Dim PIQLogoMaxWidth As Integer = CType(SysParam.GetValue(DB, "PIQLogoWidth"), Integer)
            Dim PIQLogoMaxHeight As Integer = CType(SysParam.GetValue(DB, "PIQLogoHeight"), Integer)
            Core.ResizeImageWithQuality(Server.MapPath("/assets/piq/logo/original/" & PIQ.LogoFile), Server.MapPath("/assets/piq/logo/standard/" & PIQ.LogoFile), PIQLogoMaxWidth, PIQLogoMaxHeight, 100)
        ElseIf fuLogoFile.MarkedToDelete Then
            PIQ.LogoFile = Nothing
        End If

        PIQ.Update()

        PIQAccount.FirstName = txtFirstName.Text
        PIQAccount.LastName = txtLastName.Text
        PIQAccount.Email = txtEmail.Text
        PIQAccount.Fax = txtFax.Text
        PIQAccount.Mobile = txtMobile.Text
        PIQAccount.Phone = txtPhone.Text
        PIQAccount.Username = txtUserName.Text
        If Not txtPassword.Text = String.Empty Then
            PIQAccount.Password = txtPassword.Text
            PIQAccount.RequirePasswordUpdate = False
        End If
        PIQAccount.Update()
    End Sub

    Private Function BuilderRoleSQL() As String

        Dim SQL As String = String.Empty

        SQL = "SELECT" & vbCrLf
        SQL &= "  BuilderRole Role" & vbCrLf
        SQL &= "FROM" & vbCrLf
        SQL &= "  BuilderAccountBuilderRole babr" & vbCrLf
        SQL &= "  JOIN BuilderRole br ON babr.BuilderRoleID = br.BuilderRoleID" & vbCrLf
        SQL &= "WHERE" & vbCrLf
        SQL &= "  babr.BuilderAccountId = " & AccountId & vbCrLf
        Return SQL

    End Function

    Private Function VendorRoleSQL() As String

        Dim SQL As String = String.Empty

        SQL = "SELECT" & vbCrLf
        SQL &= "  VendorRole Role" & vbCrLf
        SQL &= "FROM" & vbCrLf
        SQL &= "  VendorAccountVendorRole vavr" & vbCrLf
        SQL &= "  JOIN VendorRole vr ON vavr.VendorRoleID = vr.VendorRoleID" & vbCrLf
        SQL &= "WHERE" & vbCrLf
        SQL &= "  vavr.VendorAccountId = " & AccountId & vbCrLf
        Return SQL

    End Function

    Private Function PIQRoleSQL() As String

        Dim SQL As String = String.Empty

        SQL = "SELECT" & vbCrLf
        SQL &= "  PIQRole Role" & vbCrLf
        SQL &= "FROM" & vbCrLf
        SQL &= "  PIQAccountPIQRole vavr" & vbCrLf
        SQL &= "  JOIN PIQRole vr ON vavr.PIQRoleID = vr.PIQRoleID" & vbCrLf
        SQL &= "WHERE" & vbCrLf
        SQL &= "  vavr.PIQAccountId = " & AccountId & vbCrLf
        Return SQL

    End Function

End Class

