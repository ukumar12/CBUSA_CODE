Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports System.Linq
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected PIQID As Integer
    Dim PreferredVendors As String = ""
    Private dtVendors As DataTable

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("PIQ")

        PIQID = Convert.ToInt32(Request("PIQID"))
        If Not IsPostBack Then
            dtStartDate.Value = Now().ToShortDateString
            LoadFromDB()
        Else
            GetVendors()
        End If
    End Sub

    Private Sub GetVendors()
        dtVendors = CType(ViewState("dtVendors"), DataTable)
        If dtVendors.Rows.Count > 0 Then
            For i As Integer = 0 To dtVendors.Rows.Count - 1
                dtVendors.Rows(i)("CompanyName") = CType(rptVendors.Items(i).FindControl("ltrCompanyName"), Literal).Text
            Next
        End If

    End Sub

    Private Sub LoadFromDB()

        dtVendors = Me.DB.GetDataTable("Select v.VendorId, v.CompanyName from Vendor v, PIQPreferredVendor pv where v.VendorId = pv.PreferredVendorId And pv.PIQID = " & Me.DB.Number(PIQID) & " order by v.CompanyName")
        dtVendors.Columns("VendorId").AllowDBNull = False
        dtVendors.Columns("CompanyName").AllowDBNull = False

        Me.rptVendors.DataSource = dtVendors
        Me.rptVendors.DataBind()

        ViewState("dtVendors") = dtVendors

        drpState.DataSource = StateRow.GetStateList(DB)
        drpState.DataValueField = "StateCode"
        drpState.DataTextField = "StateName"
        drpState.DataBind()
        drpState.Items.Insert(0, New ListItem("", ""))

        drpVendor.DataSource = VendorRow.GetList(DB, "CompanyName")
        drpVendor.DataTextField = "CompanyName"
        drpVendor.DataValueField = "VendorID"
        drpVendor.DataBind()
        drpVendor.Items.Insert(0, New ListItem("Select a Vendor", ""))

        If PIQID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbPIQ As PIQRow = PIQRow.GetRow(DB, PIQID)
        txtCompanyName.Text = dbPIQ.CompanyName
        txtAddress.Text = dbPIQ.Address
        txtAddress2.Text = dbPIQ.Address2
        txtCity.Text = dbPIQ.City
        txtPhone.Text = dbPIQ.Phone
        txtMobile.Text = dbPIQ.Mobile
        txtFax.Text = dbPIQ.Fax
        txtWebsiteURL.Text = dbPIQ.WebsiteURL
        txtFirstName.Text = dbPIQ.FirstName
        txtLastName.Text = dbPIQ.LastName
        txtEmail.Text = dbPIQ.Email
        txtIncentivePrograms.Value = dbPIQ.IncentivePrograms
        ctrlZip.Value = dbPIQ.Zip
        dtStartDate.Value = dbPIQ.StartDate.ToShortDateString
        dtEndDate.Value = dbPIQ.EndDate.ToShortDateString
        drpState.SelectedValue = dbPIQ.State
        fuLogoFile.CurrentFileName = dbPIQ.LogoFile
        rblIsActive.SelectedValue = dbPIQ.IsActive
        rblHasCatalogAccess.SelectedValue = dbPIQ.HasCatalogAccess
        rblDocumentAccess.SelectedValue = dbPIQ.HasDocumentsAccess
        'Multi select dd code
        'Dim aElements As String() = dbPIQ.GetSelectedVendors.Split(",")
        'For Each Element As String In aElements
        '    If Element <> "" Then
        '        lbVendor.Items.FindByValue(Element).Selected = True
        '    End If
        'Next

    End Sub

    'Protected Sub sfAds_ValidateRow(ByVal sender As Object, ByVal args As Controls.SubForm.SubFormEventArguments) Handles sfAds.ValidateRow
    '    args.IsValid = True
    '    If args.DataRow("txtAltText") = String.Empty Then
    '        args.IsValid = False
    '    End If
    '    If args.DataRow("txtLinkURL") = String.Empty Then
    '        args.IsValid = False
    '    End If
    '    If args.DataRow("dpStartDate") <> String.Empty AndAlso Not IsDate(args.DataRow("dpStartDate")) Then
    '        args.IsValid = False
    '    End If
    '    If args.DataRow("dpEndDate") <> String.Empty AndAlso Not IsDate(args.DataRow("dpEndDate")) Then
    '        args.IsValid = False
    '    End If
    'End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub

        dtVendors = CType(ViewState("dtVendors"), DataTable)

        Try
            DB.BeginTransaction()

            Dim dbPIQ As PIQRow

            If PIQID <> 0 Then
                dbPIQ = PIQRow.GetRow(DB, PIQID)
            Else
                dbPIQ = New PIQRow(DB)
            End If
            dbPIQ.CompanyName = txtCompanyName.Text
            dbPIQ.Address = txtAddress.Text
            dbPIQ.Address2 = txtAddress2.Text
            dbPIQ.City = txtCity.Text
            dbPIQ.Phone = txtPhone.Text
            dbPIQ.Mobile = txtMobile.Text
            dbPIQ.Fax = txtFax.Text
            dbPIQ.WebsiteURL = txtWebsiteURL.Text
            dbPIQ.FirstName = txtFirstName.Text
            dbPIQ.LastName = txtLastName.Text
            dbPIQ.Email = txtEmail.Text
            dbPIQ.IncentivePrograms = txtIncentivePrograms.Value
            dbPIQ.Zip = ctrlZip.Value
            dbPIQ.StartDate = dtStartDate.Value
            dbPIQ.EndDate = dtEndDate.Value
            dbPIQ.State = drpState.SelectedValue

            If fuLogoFile.NewFileName <> String.Empty Then
                fuLogoFile.SaveNewFile()
                dbPIQ.LogoFile = fuLogoFile.NewFileName
                Dim PIQLogoMaxWidth As Integer = CType(SysParam.GetValue(DB, "PIQLogoWidth"), Integer)
                Dim PIQLogoMaxHeight As Integer = CType(SysParam.GetValue(DB, "PIQLogoHeight"), Integer)
                Core.ResizeImageWithQuality(Server.MapPath("/assets/piq/logo/original/" & dbPIQ.LogoFile), Server.MapPath("/assets/piq/logo/standard/" & dbPIQ.LogoFile), PIQLogoMaxWidth, PIQLogoMaxHeight, 100)
            ElseIf fuLogoFile.MarkedToDelete Then
                dbPIQ.LogoFile = Nothing
            End If

            dbPIQ.IsActive = rblIsActive.SelectedValue
            dbPIQ.HasCatalogAccess = rblHasCatalogAccess.SelectedValue
            dbPIQ.HasDocumentsAccess = rblDocumentAccess.SelectedValue
            If PIQID <> 0 Then
                dbPIQ.Updated = Now()
                dbPIQ.Update()
            Else
                dbPIQ.Submitted = Now()
                PIQID = dbPIQ.Insert

                'send message to builders

                Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "NewPIQForBuilders")
                Dim dtBuilders As DataTable = BuilderRow.GetList(DB)
                For Each row As DataRow In dtBuilders.Rows
                    Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, row("BuilderId"))
                    dbMsg.Send(dbBuilder, vbCrLf & vbCrLf & System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName"))
                Next

                dbMsg = AutomaticMessagesRow.GetRowByTitle(DB, "NewPIQForVendors")
                Dim dtVendors As DataTable = VendorRow.GetList(DB)
                For Each row As DataRow In dtVendors.Rows
                    Dim dbVendor As VendorRow = VendorRow.GetRow(DB, row("VendorId"))
                    dbMsg.Send(dbVendor, vbCrLf & vbCrLf & System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName"))
                Next
            End If


            dbPIQ.UpdateVendors(dtVendors)

            'dbPIQ.DeleteFromAllVendors()

            'Multi select dd code
            'For iLoop = 0 To lbVendor.Items.Count - 1
            '    If lbVendor.Items(iLoop).Selected Then
            '        PreferredVendors = PreferredVendors & lbVendor.Items(iLoop).Value & ","
            '    End If
            'Next
            'dbPIQ.InsertToVendors(PreferredVendors)

            'Dim aAds As DataRow() = sfAds.GetData()

            'For Each row As DataRow In aAds
            '    If Not sfAds.InvalidRows.Contains(row("RowIndex")) Then
            '        Dim dbPIQAds As New PIQAdRow(DB)
            '        dbPIQAds.PIQID = dbPIQ.PIQID
            '        dbPIQAds.AltText = row("txtAltText")
            '        dbPIQAds.LinkURL = row("txtLinkURL")
            '        dbPIQAds.IsActive = row("rblIsActive")
            '        If row("dpStartDate") <> Nothing Then dbPIQAds.StartDate = row("dpStartDate")
            '        If row("dpEndDate") <> Nothing Then dbPIQAds.StartDate = row("dpEndDate")

            '        'Dim fuAd As SubFormFileUpload = sfAds.FindControl("fuAdFile")

            '        Dim fuAd As Controls.SubFormFileUpload = (From field As Controls.SubFormField In sfAds.Fields Select field Where TypeOf field Is Controls.SubFormFileUpload).FirstOrDefault

            '        If fuAd.NewFileName <> String.Empty Then
            '            fuAd.SaveNewFile()
            '            dbPIQAds.AdFile = fuAd.NewFileName
            '            Core.ResizeImageWithQuality(Server.MapPath("/assets/piq/ads/" & dbPIQAds.AdFile), Server.MapPath("/assets/piq/ads/" & dbPIQAds.AdFile), SysParam.GetValue(Me.DB, "PIQAdsImageWidth"), SysParam.GetValue(Me.DB, "PIQAdsImageHeight"), 100)
            '            Core.ResizeImageWithQuality(Server.MapPath("/assets/piq/ads/thumbnails/" & dbPIQAds.AdFile), Server.MapPath("/assets/piq/ads/thumbnails/" & dbPIQAds.AdFile), SysParam.GetValue(Me.DB, "PIQAdsThumbnailWidth"), SysParam.GetValue(Me.DB, "PIQAdsThumbnailHeight"), 100)
            '        ElseIf fuAd.MarkedToDelete Then
            '            dbPIQAds.AdFile = Nothing
            '        End If
            '        dbPIQAds.Insert()
            '    End If
            'Next

            DB.CommitTransaction()

            If dbPIQ.HasDocumentsAccess Then
                DB.ExecuteSQL("Delete from AdminDocumentPIQRecipient Where PIQID = " & PIQID)


                Dim sql As String = "select adat.AdminDocumentID , " & PIQID & " AS PIQID  from AdminDocumentDocumentAudienceType adat  Inner join  AdminDocument  ad " _
                            & " on adat.AdminDocumentID = ad.AdminDocumentID where adat.DocumentAudienceTypeid =   " _
                            & "(Select DocumentAudienceTypeid From documentaudiencetype where audiencetype = 'PIQ') "

                Dim dt As DataTable = DB.GetDataTable(sql)
                ' setup bulk copy
                If dt.Rows.Count > 0 Then
                    Dim bc As New SqlClient.SqlBulkCopy(DB.Connection)
                    bc.ColumnMappings.Add("AdminDocumentID", "AdminDocumentID")
                    bc.ColumnMappings.Add("PIQID", "PIQID")
                    bc.DestinationTableName = "AdminDocumentPIQRecipient"
                    bc.BulkCopyTimeout = 300
                    'Populate table
                    bc.WriteToServer(dt)
                    bc.Close()
                End If


            End If


            If fuLogoFile.NewFileName <> String.Empty OrElse fuLogoFile.MarkedToDelete Then fuLogoFile.RemoveOldFile()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnAddVendor_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddVendor.Click
        dtVendors = CType(ViewState("dtVendors"), DataTable)

        If drpVendor.SelectedValue = String.Empty Then
            ltrMsg.Text = "<font color=red><b>Please select a vendor from the list below.</b></font></br>"
            ltrMsg.Visible = True
            Exit Sub
        End If

        For Each Row As DataRow In dtVendors.Rows
            If Row(0) = drpVendor.SelectedValue Then
                ltrMsg.Text = "<font color=red><b>The selected vendor is already on the list or preferred vendors above.</b></font></br>"
                ltrMsg.Visible = True
                Exit Sub
            End If
        Next
        ltrMsg.Visible = False
        Dim dr As DataRow = dtVendors.NewRow
        dr("VendorId") = drpVendor.SelectedValue
        dr("CompanyName") = VendorRow.GetRow(DB, drpVendor.SelectedValue).CompanyName

        dtVendors.Rows.Add(dr)

        Me.rptVendors.DataSource = dtVendors
        Me.rptVendors.DataBind()

        ViewState("dtVendors") = dtVendors

    End Sub

    Protected Sub rptVendors_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptVendors.ItemCommand
        dtVendors = CType(ViewState("dtVendors"), DataTable)
        Dim drTmp As DataRow = dtVendors.NewRow()
        If e.CommandName = "Delete" Then
            dtVendors.Rows.RemoveAt(e.CommandArgument)
            ltrMsg.Visible = False
        End If

        Me.rptVendors.DataSource = dtVendors
        Me.rptVendors.DataBind()

        ViewState("dtVendors") = dtVendors
    End Sub

    Protected Sub rptVendors_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptVendors.ItemDataBound
        If Not (e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item) Then
            Exit Sub
        End If
        Dim ltrVendorId As Literal = CType(e.Item.FindControl("ltrVendorId"), Literal)
        Dim ltrCompanyName As Literal = CType(e.Item.FindControl("ltrCompanyName"), Literal)
        Dim btnDelete As ImageButton = CType(e.Item.FindControl("btnDelete"), ImageButton)
        
        ltrVendorId.Text = e.Item.DataItem("VendorId")
        ltrCompanyName.Text = e.Item.DataItem("CompanyName")
        btnDelete.CommandName = "Delete"
        btnDelete.CommandArgument = e.Item.ItemIndex

        ltrVendorId.Visible = False

    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?PIQID=" & PIQID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

