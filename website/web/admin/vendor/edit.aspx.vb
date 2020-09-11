Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components
Imports sforce
Imports Utilities
Imports System.Net
Imports System.IO

Public Class Edit
    Inherits AdminPage

    Protected VendorID As Integer

    Private dtAccounts As DataTable
    Dim isUpdate As Boolean = False
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("VENDORS")

        VendorID = Convert.ToInt32(Request("VendorID"))
        If Not IsPostBack Then
            LoadFromDB()
            BindRoles()
        End If
    End Sub

    Private Sub BindRoles()
        dtAccounts = DB.GetDataTable("select (LastName + ', ' + FirstName) as FullName, VendorAccountID from VendorAccount where VendorID=" & DB.Number(VendorID) & " and IsActive = 1")

        Dim sql As String =
              " select r.VendorRole, vavr.VendorAccountID from VendorRole r left outer join VendorAccountVendorRole vavr on r.VendorRoleID=vavr.VendorRoleID" _
            & " where VendorAccountID in (select VendorAccountID from VendorAccount where VendorID=" & DB.Number(VendorID)

        'Dim dtRoles As DataTable = VendorRoleRow.GetVendorRoles(DB, VendorID)
        Dim dtRoles As DataTable = DB.GetDataTable("select * from VendorRole")
        rptRoles.DataSource = dtRoles
        rptRoles.DataBind()
    End Sub

    Private Sub LoadFromDB()
        drpState.DataSource = StateRow.GetStateList(DB)
        drpState.DataValueField = "StateCode"
        drpState.DataTextField = "StateCode"
        drpState.DataBind()
        drpState.Items.Insert(0, New ListItem("", ""))

        drpbillingState.DataSource = StateRow.GetStateList(DB)
        drpbillingState.DataValueField = "StateCode"
        drpbillingState.DataTextField = "StateCode"
        drpbillingState.DataBind()
        drpbillingState.Items.Insert(0, New ListItem("", ""))


        ddlDashboardCategory.DataSource = DB.GetDataTable("SELECT DashboardCategoryID, DashboardCategory FROM VendorDashboardCategory")
        ddlDashboardCategory.DataValueField = "DashboardCategoryID"
        ddlDashboardCategory.DataTextField = "DashboardCategory"
        ddlDashboardCategory.DataBind()
        ddlDashboardCategory.Items.Insert(0, New ListItem("", ""))

        'drpPrimaryVendorCategoryId.DataSource = VendorCategoryRow.GetList(DB, "SortOrder")
        'drpPrimaryVendorCategoryId.DataValueField = "VendorCategoryID"
        'drpPrimaryVendorCategoryId.DataTextField = "Category"
        'drpPrimaryVendorCategoryId.DataBind()
        'drpPrimaryVendorCategoryId.Items.Insert(0, New ListItem("", ""))

        'drpSecondaryVendorCategoryId.DataSource = VendorCategoryRow.GetList(DB, "SortOrder")
        'drpSecondaryVendorCategoryId.DataValueField = "VendorCategoryID"
        'drpSecondaryVendorCategoryId.DataTextField = "Category"
        'drpSecondaryVendorCategoryId.DataBind()
        'drpSecondaryVendorCategoryId.Items.Insert(0, New ListItem("", ""))

        cblCategory.DataSource = VendorCategoryRow.GetList(DB, "SortOrder")
        cblCategory.DataTextField = "Category"
        cblCategory.DataValueField = "VendorCategoryID"
        cblCategory.DataBind()

        cblLLC.DataSource = LLCRow.GetList(DB)
        cblLLC.DataTextField = "LLC"
        cblLLC.DataValueField = "LLCID"
        cblLLC.DataBind()

        If VendorID = 0 Then
            btnDelete.Visible = False

            Exit Sub
        End If

        If VendorID > 0 Then
            cblLLC.Enabled = False
            btnSave.Message = Nothing
        End If

        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, VendorID)
        txtCRMID.Text = dbVendor.CRMID
        txtCRMID.Enabled = False
        txtCompanyName.Text = dbVendor.CompanyName
        txtAddress.Text = dbVendor.Address
        txtAddress2.Text = dbVendor.Address2
        txtCity.Text = dbVendor.City
        txtPhone.Text = dbVendor.Phone
        txtMobile.Text = dbVendor.Mobile
        txtPager.Text = dbVendor.Pager
        txtOtherPhone.Text = dbVendor.OtherPhone
        txtFax.Text = dbVendor.Fax
        txtEmail.Text = dbVendor.Email
        txtWebsiteURL.Text = dbVendor.WebsiteURL
        txtAboutUs.Text = dbVendor.AboutUs
        txtComments.Text = dbVendor.Comments
        ctrlZip.Value = dbVendor.Zip
        drpState.SelectedValue = dbVendor.State

        txtBillingAddress.Text = dbVendor.BillingAddress
        txtBillingCity.Text = dbVendor.BillingCity
        drpbillingState.SelectedValue = dbVendor.BillingState
        ctrlbillingZip.Value = dbVendor.BillingZip


        'drpPrimaryVendorCategoryId.SelectedValue = dbVendor.PrimaryVendorCategoryID
        'drpSecondaryVendorCategoryId.SelectedValue = dbVendor.SecondaryVendorCategoryID
        fuLogoFile.CurrentFileName = dbVendor.LogoFile
        rblIsActive.SelectedValue = dbVendor.IsActive
        'drpLLC.SelectedValue = dbVendor.LLCID
        cblLLC.SelectedValues = dbVendor.GetSelectedLLCs
        cbEnableMarketShare.Checked = dbVendor.EnableMarketShare
        cbExcludeVendor.Checked = dbVendor.ExcludedVendor
        cblCategory.SelectedValues = dbVendor.GetSelectedVendorCategories
        rblIsPlansOnline.SelectedValue = dbVendor.IsPlansOnline
        rblDocumentAccess.SelectedValue = dbVendor.HasDocumentsAccess

        rblQuarterlyReportingOn.SelectedValue = dbVendor.QuarterlyReportingOn

        ddlDashboardCategory.SelectedValue = dbVendor.DashboardCategoryID
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbVendor As VendorRow

            If VendorID <> 0 Then
                dbVendor = VendorRow.GetRow(DB, VendorID)
            Else
                dbVendor = New VendorRow(DB)
            End If
            dbVendor.CRMID = txtCRMID.Text
            dbVendor.CompanyName = txtCompanyName.Text
            dbVendor.Address = txtAddress.Text
            dbVendor.Address2 = txtAddress2.Text
            dbVendor.City = txtCity.Text
            dbVendor.Phone = txtPhone.Text
            dbVendor.Mobile = txtMobile.Text
            dbVendor.Pager = txtPager.Text
            dbVendor.OtherPhone = txtOtherPhone.Text
            dbVendor.Fax = txtFax.Text
            dbVendor.Email = txtEmail.Text
            dbVendor.WebsiteURL = txtWebsiteURL.Text
            dbVendor.AboutUs = txtAboutUs.Text
            dbVendor.Comments = txtComments.Text
            dbVendor.Zip = ctrlZip.Value
            dbVendor.State = drpState.SelectedValue

            dbVendor.BillingAddress = txtBillingAddress.Text
            dbVendor.BillingCity = txtBillingCity.Text
            dbVendor.BillingState = drpbillingState.SelectedValue
            dbVendor.BillingZip = ctrlbillingZip.Value


            'dbVendor.PrimaryVendorCategoryID = drpPrimaryVendorCategoryId.SelectedValue
            'dbVendor.SecondaryVendorCategoryID = drpSecondaryVendorCategoryId.SelectedValue
            dbVendor.IsPlansOnline = rblIsPlansOnline.SelectedValue
            If fuLogoFile.NewFileName <> String.Empty Then
                fuLogoFile.SaveNewFile()
                dbVendor.LogoFile = fuLogoFile.NewFileName
                Dim VendorLogoMaxWidth As Integer = CType(SysParam.GetValue(DB, "VendorLogoMaxWidth"), Integer)
                Dim VendorLogoMaxHeight As Integer = CType(SysParam.GetValue(DB, "VendorLogoMaxHeight"), Integer)
                Core.ResizeImageWithQuality(Server.MapPath("/assets/vendor/logo/original/" & dbVendor.LogoFile), Server.MapPath("/assets/vendor/logo/standard/" & dbVendor.LogoFile), VendorLogoMaxWidth, VendorLogoMaxHeight, 100)
            ElseIf fuLogoFile.MarkedToDelete Then
                dbVendor.LogoFile = Nothing
            End If

            dbVendor.IsActive = rblIsActive.SelectedValue
            dbVendor.EnableMarketShare = cbEnableMarketShare.Checked
            dbVendor.ExcludedVendor = cbExcludeVendor.Checked
            dbVendor.HasDocumentsAccess = rblDocumentAccess.SelectedValue
            dbVendor.QuarterlyReportingOn = rblQuarterlyReportingOn.SelectedValue
            dbVendor.DashboardCategoryID = ddlDashboardCategory.SelectedValue

            If cblLLC.SelectedValues = "" Then
                ShowJavaScriptMessage("Market Is Not Selected, Please Select Market")
            Else

                If VendorID <> 0 Then
                    dbVendor.Update()
                    isUpdate = True
                    dbVendor.DeleteFromAllLLCs()
                    dbVendor.InsertToLLCs(cblLLC.SelectedValues)

                Else
                    VendorID = dbVendor.Insert
                    dbVendor.DeleteFromAllLLCs()
                    dbVendor.InsertToLLCs(cblLLC.SelectedValues)

                End If

                'Clear all Vendor roles
                VendorRoleRow.ClearAllVendorRoles(DB, VendorID)

                'Assign New Roles to Vendor
                For Each item As RepeaterItem In rptRoles.Items
                    Dim hdnRoleId As HiddenField = item.FindControl("hdnRoleID")
                    'Dim drpAccount As DropDownList = item.FindControl("drpAccount")
                    Dim chkAccount As CheckBoxList = item.FindControl("chkAccount")

                    If Not chkAccount Is Nothing And Not hdnRoleId Is Nothing Then
                        For Each itm As ListItem In chkAccount.Items
                            If itm.Selected = True Then
                                VendorRoleRow.InsertVendorRole(DB, VendorID, itm.Value, hdnRoleId.Value)
                            End If
                        Next
                    End If
                Next

                SyncVendorRoleToCrm()

                dbVendor.DeleteFromAllVendorCategories()
                dbVendor.InsertToVendorCategories(cblCategory.SelectedValues)

                DB.CommitTransaction()


                '=================== Apala (Medullus) - Sales Force related code 11.08.2017 ===================
                '' Salesforce Integration
                'If Not dbVendor.IsActive Then
                '    DB.ExecuteSQL("Update VendorAccount Set IsActive = 0 Where VendorId = " & DB.Number(VendorID))
                'End If

                'If isUpdate Then
                '    Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
                '    If sfHelper.Login() Then
                '        If sfHelper.UpsertVendor(VendorRow.GetRow(DB, VendorID)) = False Then
                '            'throw error
                '        End If
                '    End If
                'Else
                '    Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
                '    If sfHelper.Login() Then
                '        If sfHelper.InsertVendor(VendorRow.GetRow(DB, VendorID)) = False Then
                '            'throw error
                '        End If
                '    End If
                'End If
                '===============================================================================================


                DB.ExecuteSQL("Delete from AdminDocumentVendorRecipient Where VendorID = " & VendorID)
                If dbVendor.HasDocumentsAccess Then
                    Dim sql As String = "select DISTINCT adat.AdminDocumentID , " & VendorID & " AS VendorID  from AdminDocumentDocumentAudienceType adat  Inner join  AdminDocument  ad " _
                                & " on adat.AdminDocumentID = ad.AdminDocumentID Left join AdminDocumentLLC adl on adl.AdminDocumentID = ad.AdminDocumentID  where adat.DocumentAudienceTypeid =   " _
                                & "(Select DocumentAudienceTypeid From documentaudiencetype where audiencetype = 'Vendor') "
                    If cblLLC.SelectedValues <> String.Empty Then
                        sql &= " AND  adl.llcid in (select LLCID FROM LLCVendor where VendorID = " & VendorID & ")"
                    End If

                    Dim dt As DataTable = DB.GetDataTable(sql)
                    ' setup bulk copy
                    If dt.Rows.Count > 0 Then
                        Dim bc As New SqlClient.SqlBulkCopy(DB.Connection)
                        bc.ColumnMappings.Add("AdminDocumentID", "AdminDocumentID")
                        bc.ColumnMappings.Add("VendorID", "VendorID")
                        bc.DestinationTableName = "AdminDocumentVendorRecipient"
                        bc.BulkCopyTimeout = 300
                        'Populate table
                        bc.WriteToServer(dt)
                        bc.Close()
                    End If


                End If




                If fuLogoFile.NewFileName <> String.Empty OrElse fuLogoFile.MarkedToDelete Then fuLogoFile.RemoveOldFile()

                Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
            End If
        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?VendorID=" & VendorID & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnEditBranches_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEditBranches.Click
        Response.Redirect("branches/default.aspx?VendorID=" & VendorID)
    End Sub

    Protected Sub btnEditOwners_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEditOwners.Click
        Response.Redirect("owners/default.aspx?VendorID=" & VendorID)
    End Sub

    Protected Sub rptRoles_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptRoles.ItemDataBound
        If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim ltlRole As Literal = e.Item.FindControl("ltlRole")
        ' Dim drpAccount As DropDownList = e.Item.FindControl("drpAccount")
        Dim drpAccount As CheckBoxList = e.Item.FindControl("chkAccount")

        Dim hdnRoleId As HiddenField = e.Item.FindControl("hdnRoleID")

        hdnRoleId.Value = e.Item.DataItem("VendorRoleID")
        ltlRole.Text = e.Item.DataItem("VendorRole")

        drpAccount.DataSource = dtAccounts
        drpAccount.DataTextField = "FullName"
        drpAccount.DataValueField = "VendorAccountID"
        drpAccount.DataBind()
        ' drpAccount.Items.Insert(0, New ListItem("", ""))
        'If Not IsDBNull(e.Item.DataItem("VendorAccountID")) Then
        '    drpAccount.SelectedValue = e.Item.DataItem("VendorAccountID")
        'End If

        Dim dtVendorRoleAccounts As DataTable = DB.GetDataTable(" SELECT * from VendorAccountVendorRole where VendorAccountID IN (select VendorAccountID from VendorAccount where VendorID=" & DB.Number(VendorID) & ") AND VendorRoleID= " & hdnRoleId.Value & "")
        If dtVendorRoleAccounts.Rows.Count > 0 Then
            For Each itm As ListItem In drpAccount.Items
                If dtVendorRoleAccounts.Select("VendorAccountId=" & itm.Value & "").Count > 0 Then
                    itm.Selected = True
                End If
            Next
        End If

    End Sub
    Public Sub ShowJavaScriptMessage(message As String)
        Dim cleanMessage As String = message.Replace("'", "\'")
        Dim page As Page = HttpContext.Current.CurrentHandler
        Dim script As String = String.Format("alert('{0}');", cleanMessage)
        If (page IsNot Nothing And Not page.ClientScript.IsClientScriptBlockRegistered("alert")) Then
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), "alert", script, True) ' /* addScriptTags */
        End If
    End Sub
    Private Sub SyncVendorRoleToCrm()
        Dim sql As String = "   SELECT va.CRMID,Isnull(vr.VendorRole,'No Role') VendorRole FROM VendorAccountVendorRole vavr " &
                            "   INNER JOIN VendorRole	  vr	ON vavr.VendorRoleID		=		vr.VendorRoleID " &
                            "   right outer join   VendorAccount va	ON vavr.VendorAccountID		=		va.VendorAccountID " &
                            "   WHERE va.VendorID		    =	" & DB.Number(VendorID) & " " &
                            "   AND   va.IsActive			=	1"
        Dim dtVendorRole As DataTable = DB.GetDataTable(sql)
        Dim vendorRoleList As New List(Of VendorRoleCrm)()
        If dtVendorRole.Rows.Count > 0 Then
            For Each row As DataRow In dtVendorRole.Rows

                If vendorRoleList.Count <= 0 Then
                    vendorRoleList.Add(New VendorRoleCrm With {
                        .CRMID = row("CRMID").ToString(),
                        .VendorRoleName = row("VendorRole").ToString()
                    })
                Else
                    Dim result = vendorRoleList.Where(Function(x) x.CRMID = row("CRMID").ToString()).FirstOrDefault()
                    If (Not result Is Nothing) Then
                        result.VendorRoleName = result.VendorRoleName + ";" + row("VendorRole").ToString()
                    Else
                        vendorRoleList.Add(New VendorRoleCrm With {
                                                            .CRMID = row("CRMID").ToString(),
                                                            .VendorRoleName = row("VendorRole")
                                                        })
                    End If
                End If
            Next
            ''Sync records in CRM
            For Each itm In vendorRoleList
                Dim strBody As String = "{""CONTACT_ID"":" & itm.CRMID & ",""CUSTOMFIELDS"":[{""FIELD_NAME"":""Contact_Roles2__c"",""FIELD_VALUE"":""" & itm.VendorRoleName & """}]}"
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12 Or SecurityProtocolType.Ssl3
                Dim req1 As WebRequest = WebRequest.Create("https://api.insightly.com/v3.1/Contacts")
                req1.Method = "PUT"
                req1.Headers("Authorization") = "Basic ZDgyNDdjNzAtYWIyZC00NDlkLTllMGMtNzViODAxODBkZTkyOg=="
                req1.ContentLength = strBody.Length

                If (Not strBody Is Nothing) Then
                    Dim postBytes = Encoding.ASCII.GetBytes(strBody)
                    req1.ContentLength = postBytes.Length
                    Dim requestStream As Stream = req1.GetRequestStream()
                    requestStream.Write(postBytes, 0, postBytes.Length)
                End If
                Using resp As HttpWebResponse = TryCast(req1.GetResponse(), HttpWebResponse)
                    If (resp.StatusCode = HttpStatusCode.OK) Then
                    End If
                End Using
            Next

        End If


    End Sub
End Class

Public Class VendorRoleCrm
    Public Property CRMID As String
    Public Property VendorRoleName As String

End Class
