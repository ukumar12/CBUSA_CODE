Imports Components
Imports DataLayer
Imports Utilities

Partial Class Account

    Inherits SitePage

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        Dim SQL As String = String.Empty
        Dim dt As DataTable


        If CType(Me.Page, SitePage).IsLoggedInBuilder Then
            LoadBuilderInfo()
            SQL = BuilderRoleSQL()
        ElseIf CType(Me.Page, SitePage).IsLoggedInVendor Then
            LoadVendorInfo()
            SQL = VendorRoleSQL()
        ElseIf CType(Me.Page, SitePage).IsLoggedInPIQ Then
            LoadPIQInfo()

        End If

        If SQL <> String.Empty Then
            dt = Me.DB.GetDataTable(SQL)
            Me.rptRoles.DataSource = dt
            Me.rptRoles.DataBind()
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            drpState.DataSource = StateRow.GetStateList(DB)
            drpState.DataTextField = "StateName"
            drpState.DataValueField = "StateCode"
            drpState.DataBind()
            drpState.Items.Insert(0, New ListItem("", ""))
        End If
    End Sub

    Protected Sub frmEditAccount_Callback(ByVal sender As Object, ByVal args As PopupForm.PopupFormEventArgs) Handles frmEditAccount.Callback
        Dim json As New Web.Script.Serialization.JavaScriptSerializer
        Dim ret As String = json.Serialize(args.Data)
        frmEditAccount.CallbackResult = ret
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
        'Me.divlink.Visible = True

        Dim CompanyName As TextBox = frmEditAccount.FindControl("txtCompanyName")

        CompanyName.Text = Builder.CompanyName

        'txtCompanyName.Text = Builder.CompanyName
        'txtAddress.Text = Builder.Address
        'txtAddress2.Text = Builder.Address2
        'txtCity.Text = Builder.City
        'drpState.SelectedValue = Builder.State
        'txtZip.Text = Builder.Zip
        'txtPhone.Text = Builder.Phone
        'txtMobile.Text = Builder.Mobile
        'txtFax.Text = Builder.Fax
        'txtEmail.Text = Builder.Email
        'txtWebsiteURL.Text = Builder.WebsiteURL

    End Sub

    Private Sub LoadVendorInfo()

        Dim VendorAccount As DataLayer.VendorAccountRow
        Dim Vendor As DataLayer.VendorRow

        VendorAccount = DataLayer.VendorAccountRow.GetRow(Me.DB, Session("VendorAccountId"))
        Vendor = DataLayer.VendorRow.GetRow(Me.DB, VendorAccount.VendorID)

        Me.ltdUserName.Text = VendorAccount.FirstName & " " & VendorAccount.LastName
        Me.ltdCompanyName.Text = Vendor.CompanyName

        Me.divAddress.InnerHtml = Vendor.Address & "<br/>"
        If Vendor.Address2 <> String.Empty Then
            Me.divAddress.InnerHtml &= Vendor.Address2 & "<br/>"
        End If
        Me.divAddress.InnerHtml &= Vendor.City & ", " & Vendor.State & " " & Vendor.Zip

        'Only allow for admins to edit the company
        'Me.divlink.Visible = True

        txtCompanyName.Text = Vendor.CompanyName
        txtAddress.Text = Vendor.Address
        txtAddress2.Text = Vendor.Address2
        txtCity.Text = Vendor.City
        drpState.SelectedValue = Vendor.State
        txtZip.Text = Vendor.Zip
        txtPhone.Text = Vendor.Phone
        txtMobile.Text = Vendor.Mobile
        txtFax.Text = Vendor.Fax
        txtEmail.Text = Vendor.Email
        txtWebsiteURL.Text = Vendor.WebsiteURL

    End Sub

    Private Sub LoadPIQInfo()

        Dim PIQAccount As DataLayer.PIQAccountRow
        Dim PIQ As DataLayer.PIQRow

        PIQAccount = DataLayer.PIQAccountRow.GetRow(Me.DB, Session("PIQAccountId"))
        PIQ = DataLayer.PIQRow.GetRow(Me.DB, PIQAccount.PIQID)

        Me.ltdUserName.Text = PIQAccount.FirstName & " " & PIQAccount.LastName
        Me.ltdCompanyName.Text = PIQ.CompanyName

        Me.divAddress.InnerHtml = PIQ.Address & "<br/>"
        If PIQ.Address2 <> String.Empty Then
            Me.divAddress.InnerHtml &= PIQ.Address2 & "<br/>"
        End If
        Me.divAddress.InnerHtml &= PIQ.City & ", " & PIQ.State & " " & PIQ.Zip

        'Only allow for admins to edit the company
        'Me.divlink.Visible = True

        txtCompanyName.Text = PIQ.CompanyName
        txtAddress.Text = PIQ.Address
        txtAddress2.Text = PIQ.Address2
        txtCity.Text = PIQ.City
        drpState.SelectedValue = PIQ.State
        txtZip.Text = PIQ.Zip
        txtPhone.Text = PIQ.Phone
        txtMobile.Text = PIQ.Mobile
        txtFax.Text = PIQ.Fax
        txtEmail.Text = PIQ.Email
        txtWebsiteURL.Text = PIQ.WebsiteURL

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
        Builder.State = drpState.SelectedValue
        Builder.Zip = txtZip.Text
        Builder.Phone = txtPhone.Text
        Builder.Mobile = txtMobile.Text
        Builder.Fax = txtFax.Text
        Builder.Email = txtEmail.Text
        Builder.WebsiteURL = txtWebsiteURL.Text

        Builder.Update()

        '' Salesforce Integration
        'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
        'If sfHelper.Login() Then
        '    If sfHelper.UpsertBuilder(Builder) = False Then
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
        Vendor.State = drpState.SelectedValue
        Vendor.Zip = txtZip.Text
        Vendor.Phone = txtPhone.Text
        Vendor.Mobile = txtMobile.Text
        Vendor.Fax = txtFax.Text
        Vendor.Email = txtEmail.Text
        Vendor.WebsiteURL = txtWebsiteURL.Text

        Vendor.Update()

        '' Salesforce Integration
        'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
        'If sfHelper.Login() Then
        '    If sfHelper.UpsertVendor(Vendor) = False Then
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
        PIQ.State = drpState.SelectedValue
        PIQ.Zip = txtZip.Text
        PIQ.Phone = txtPhone.Text
        PIQ.Mobile = txtMobile.Text
        PIQ.Fax = txtFax.Text
        PIQ.Email = txtEmail.Text
        PIQ.WebsiteURL = txtWebsiteURL.Text

        PIQ.Update()

    End Sub

    Private Function BuilderRoleSQL() As String

        Dim SQL As String = String.Empty

        SQL = "SELECT" & vbCrLf
        SQL &= "  BuilderRole Role" & vbCrLf
        SQL &= "FROM" & vbCrLf
        SQL &= "  BuilderAccountBuilderRole babr" & vbCrLf
        SQL &= "  JOIN BuilderRole br ON babr.BuilderRoleID = br.BuilderRoleID" & vbCrLf

        Return SQL

    End Function

    Private Function VendorRoleSQL() As String

        Dim SQL As String = String.Empty

        SQL = "SELECT" & vbCrLf
        SQL &= "  VendorRole Role" & vbCrLf
        SQL &= "FROM" & vbCrLf
        SQL &= "  VendorAccountVendorRole vavr" & vbCrLf
        SQL &= "  JOIN VendorRole vr ON vavr.VendorRoleID = vr.VendorRoleID" & vbCrLf

        Return SQL

    End Function

End Class
