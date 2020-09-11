Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components
Imports Controls

Public Class Edit
    Inherits AdminPage

    Protected ItemId As Integer
	Protected AllowOverride As Boolean = False
	Protected EnableInventoryManagement As Boolean
	Protected EnableAttributeInventoryManagement As Boolean

	Private Property InventoryControlledAttributeId() As Integer
		Get
			If ViewState("icaid") Is Nothing Then ViewState("icaid") = 0
			Return ViewState("icaid")
		End Get
		Set(ByVal value As Integer)
			ViewState("icaid") = value
		End Set
	End Property

	Private Property Expanded() As ArrayList
		Get
			If ViewState("ExpandedNodes") Is Nothing Then ViewState("ExpandedNodes") = New ArrayList
			Return ViewState("ExpandedNodes")
		End Get
		Set(ByVal value As ArrayList)
			ViewState("ExpandedNodes") = value
		End Set
	End Property

	Private Property Guid() As String
		Get
			Return ViewState("AttributeGuid")
		End Get
		Set(ByVal value As String)
			ViewState("AttributeGuid") = value
		End Set
	End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Response.Cache.SetCacheability(HttpCacheability.NoCache)
        CheckAccess("STORE")

        ItemId = Convert.ToInt32(Request("ItemId"))
        If Not IsPostBack Then
			Guid = System.Guid.NewGuid.ToString
			divItem.Style.Add("display", "block")
			divAttributes.Style.Add("display", "none")
			LoadDepartments()
            LoadFromDB()
        End If

		EnableInventoryManagement = SysParam.GetValue(DB, "EnableInventoryManagement") = 1
		EnableAttributeInventoryManagement = EnableInventoryManagement AndAlso SysParam.GetValue(DB, "EnableAttributeInventoryManagement") = 1
		AllowOverride = DB.ExecuteScalar("select count(*) from storeitemtemplateattribute where parentid is null and templateid = " & DB.Number(drpTemplateId.SelectedValue)) = 1

		phInventory.Visible = EnableInventoryManagement
		rfvInventoryQty.Enabled = EnableInventoryManagement
		BindAttributes()
    End Sub

    Private Sub LoadDepartments()
        Dim Result As String = ""

        SQL = "SELECT DepartmentId FROM StoreDepartmentItem WHERE ItemId = " & DB.Quote(ItemId)
        Dim sConn As String = ""
        Dim res As SqlDataReader = DB.GetReader(SQL)
        While res.Read
            Result = Result & sConn & res("DepartmentId")
            sConn = ","
        End While
        res.Close()
        ctrlStoreDepartmentTree.CheckedList = Result
    End Sub

    Private Sub LoadFromDB()
		drpTemplateId.DataSource = StoreItemTemplateRow.GetValidTemplates(DB)
        drpTemplateId.DataValueField = "TemplateId"
        drpTemplateId.DataTextField = "TemplateName"
        drpTemplateId.DataBind()
		drpTemplateId.Items.Insert(0, New ListItem("", ""))

        drpBrandId.DataSource = StoreBrandRow.GetBrands(DB)
        drpBrandId.DataValueField = "BrandId"
        drpBrandId.DataTextField = "Name"
        drpBrandId.DataBind()
        drpBrandId.Items.Insert(0, New ListItem("", "0"))

        cblFeatures.DataSource = StoreFeatureRow.GetAllStoreFeatures(DB)
        cblFeatures.DataTextField = "NameWithUniqueText"
        cblFeatures.DataValueField = "FeatureId"
        cblFeatures.DataBind()

		StoreItemAttributeRow.LoadTempAttributes(DB, ItemId, Guid)

        If ItemId = 0 Then
			txtInventoryQty.Text = 0
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbStoreItem As StoreItemRow = StoreItemRow.GetRow(DB, ItemId)
        txtItemName.Text = dbStoreItem.ItemName
        txtSKU.Text = dbStoreItem.SKU
        txtPageTitle.Text = dbStoreItem.PageTitle
        txtMetaDescription.Text = dbStoreItem.MetaDescription
        txtShortDescription.Text = dbStoreItem.ShortDescription
        txtMetaKeywords.Text = dbStoreItem.MetaKeywords
        txtCustomURL.Text = dbStoreItem.CustomURL

        txtWeight.Text = IIf(dbStoreItem.Weight = 0, String.Empty, dbStoreItem.Weight)
        txtWidth.Text = IIf(dbStoreItem.Width = 0, String.Empty, dbStoreItem.Width)
        txtHeight.Text = IIf(dbStoreItem.Height = 0, String.Empty, dbStoreItem.Height)
        txtThickness.Text = IIf(dbStoreItem.Thickness = 0, String.Empty, dbStoreItem.Thickness)

        txtShipping1.Text = IIf(dbStoreItem.Shipping1 = 0, String.Empty, dbStoreItem.Shipping1)
        txtShipping2.Text = IIf(dbStoreItem.Shipping2 = 0, String.Empty, dbStoreItem.Shipping2)
        txtCountryUnit.Text = IIf(dbStoreItem.CountryUnit = 0, String.Empty, dbStoreItem.CountryUnit)

        txtPrice.Text = dbStoreItem.Price
        txtItemUnit.Text = dbStoreItem.ItemUnit
        txtSalePrice.Text = dbStoreItem.SalePrice
        txtLongDescription.Value = dbStoreItem.LongDescription

        drpTemplateId.SelectedValue = dbStoreItem.TemplateId
		drpBrandId.SelectedValue = dbStoreItem.BrandId

        fuImage.CurrentFileName = dbStoreItem.Image
        chkIsOnSale.Checked = dbStoreItem.IsOnSale

        chkIsTaxFree.Checked = dbStoreItem.IsTaxFree
        chkIsActive.Checked = dbStoreItem.IsActive
        chkIsFeatured.Checked = dbStoreItem.IsFeatured
        chkIsGiftWrap.Checked = dbStoreItem.IsGiftWrap

		txtInventoryQty.Text = dbStoreItem.InventoryQty
		txtInventoryWarningThreshold.Text = dbStoreItem.InventoryWarningThreshold
		txtInventoryActionThreshold.Text = dbStoreItem.InventoryActionThreshold
		drpInventoryAction.SelectedValue = dbStoreItem.InventoryAction
		dpBackorderDate.Value = dbStoreItem.BackorderDate

        cblFeatures.SelectedValues = dbStoreItem.GetSelectedStoreFeatures

		RefreshInventoryAttribute()

		drpDisplayMode.SelectedValue = dbStoreItem.DisplayMode
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		rfvSKU.Enabled = DB.ExecuteScalar("select count(*) from storeitemtemplateattribute where templateid = " & DB.Number(drpTemplateId.SelectedValue)) = 0

		Page.Validate()
		If Not IsValid Then Exit Sub

        Try
            DB.BeginTransaction()

            Dim dbStoreItem As StoreItemRow
            If ItemId <> 0 Then
                dbStoreItem = StoreItemRow.GetRow(DB, ItemId)
			Else
                dbStoreItem = New StoreItemRow(DB)
			End If

            If Not txtCustomURL.Text = String.Empty AndAlso dbStoreItem.CustomURL <> txtCustomURL.Text Then
                Dim dbCustomURLHistory As CustomURLHistoryRow = CustomURLHistoryRow.GetFromCustomURL(DB, Me.txtCustomURL.Text)
                If dbCustomURLHistory.CustomURLHistoryId > 0 Then
                    Throw New ApplicationException("Custom URL has been used in the past. For SEO purposes, please use a different custom url. ")
                    Exit Sub
                End If
                If Not URLMappingManager.IsValidFolder(txtCustomURL.Text) Then
                    Throw New ApplicationException("Cannot use a URL rewrite which points to a system folder. Please try another folder")
                End If
                If Not URLMappingManager.IsValidURL(DB, txtCustomURL.Text) Then
                    Throw New ApplicationException("The requested Custom URL is already used. Please provide different URL")
                End If
            End If

            dbStoreItem.ItemName = txtItemName.Text
            dbStoreItem.SKU = txtSKU.Text
            dbStoreItem.PageTitle = txtPageTitle.Text
            dbStoreItem.MetaDescription = txtMetaDescription.Text
            dbStoreItem.MetaKeywords = txtMetaKeywords.Text
            dbStoreItem.CustomURL = txtCustomURL.Text

            If txtWeight.Text <> "" Then dbStoreItem.Weight = txtWeight.Text Else dbStoreItem.Weight = Nothing
            If txtWidth.Text <> "" Then dbStoreItem.Width = txtWidth.Text Else dbStoreItem.Width = Nothing
            If txtHeight.Text <> "" Then dbStoreItem.Height = txtHeight.Text Else dbStoreItem.Height = Nothing
            If txtThickness.Text <> "" Then dbStoreItem.Thickness = txtThickness.Text Else dbStoreItem.Thickness = Nothing
            If txtShipping1.Text <> "" Then dbStoreItem.Shipping1 = txtShipping1.Text Else dbStoreItem.Shipping1 = Nothing
            If txtShipping2.Text <> "" Then dbStoreItem.Shipping2 = txtShipping2.Text Else dbStoreItem.Shipping2 = Nothing
            If txtCountryUnit.Text <> "" Then dbStoreItem.CountryUnit = txtCountryUnit.Text Else dbStoreItem.CountryUnit = Nothing

            dbStoreItem.Price = txtPrice.Text
            dbStoreItem.ItemUnit = txtItemUnit.Text

            If txtSalePrice.Text <> "" Then dbStoreItem.SalePrice = txtSalePrice.Text
            dbStoreItem.LongDescription = txtLongDescription.Value
            dbStoreItem.ShortDescription = txtShortDescription.Text

            dbStoreItem.TemplateId = drpTemplateId.SelectedValue
            If drpBrandId.SelectedValue <> "" Then dbStoreItem.BrandId = drpBrandId.SelectedValue

            If fuImage.NewFileName <> String.Empty Then
                fuImage.SaveNewFile()
                dbStoreItem.Image = fuImage.NewFileName
                Core.ResizeImage(Server.MapPath("/assets/item/original/" & dbStoreItem.Image), Server.MapPath("/assets/item/cart/" & dbStoreItem.Image), SysParam.GetValue(DB, "StoreItemImageCartWidth"), SysParam.GetValue(DB, "StoreItemImageCartHeight"))
                Core.ResizeImage(Server.MapPath("/assets/item/original/" & dbStoreItem.Image), Server.MapPath("/assets/item/related/" & dbStoreItem.Image), SysParam.GetValue(DB, "StoreItemImageRelatedWidth"), SysParam.GetValue(DB, "StoreItemImageRelatedHeight"))
                Core.ResizeImage(Server.MapPath("/assets/item/original/" & dbStoreItem.Image), Server.MapPath("/assets/item/featured/" & dbStoreItem.Image), SysParam.GetValue(DB, "StoreItemImageFeaturedWidth"), SysParam.GetValue(DB, "StoreItemImageFeaturedHeight"))
                Core.ResizeImage(Server.MapPath("/assets/item/original/" & dbStoreItem.Image), Server.MapPath("/assets/item/regular/" & dbStoreItem.Image), SysParam.GetValue(DB, "StoreItemImageRegularWidth"), SysParam.GetValue(DB, "StoreItemImageRegularHeight"))
                Core.ResizeImage(Server.MapPath("/assets/item/original/" & dbStoreItem.Image), Server.MapPath("/assets/item/large/" & dbStoreItem.Image), SysParam.GetValue(DB, "StoreItemImageLargeWidth"), SysParam.GetValue(DB, "StoreItemImageLargeHeight"))
                Core.ResizeImage(Server.MapPath("/assets/item/original/" & dbStoreItem.Image), Server.MapPath("/assets/item/thumbnail/" & dbStoreItem.Image), SysParam.GetValue(DB, "StoreItemImageThumbnailWidth"), SysParam.GetValue(DB, "StoreItemImageThumbnailHeight"))

                'save ThumbnailWidth and ThumbnailHeight
                Core.GetImageSize(Server.MapPath("/assets/item/thumbnail/" & dbStoreItem.Image), dbStoreItem.ThumbnailWidth, dbStoreItem.ThumbnailHeight)

            ElseIf fuImage.MarkedToDelete Then
                dbStoreItem.Image = Nothing
            End If
			dbStoreItem.IsActive = chkIsActive.Checked
            dbStoreItem.IsOnSale = chkIsOnSale.Checked
            dbStoreItem.IsTaxFree = chkIsTaxFree.Checked
            dbStoreItem.IsFeatured = chkIsFeatured.Checked
            dbStoreItem.IsGiftWrap = chkIsGiftWrap.Checked
			dbStoreItem.InventoryQty = txtInventoryQty.Text
			If IsNumeric(txtInventoryWarningThreshold.Text) Then dbStoreItem.InventoryWarningThreshold = txtInventoryWarningThreshold.Text Else dbStoreItem.InventoryWarningThreshold = Nothing
			If IsNumeric(txtInventoryActionThreshold.Text) Then dbStoreItem.InventoryActionThreshold = txtInventoryActionThreshold.Text Else dbStoreItem.InventoryActionThreshold = Nothing
			dbStoreItem.InventoryAction = drpInventoryAction.SelectedValue
			dbStoreItem.BackorderDate = dpBackorderDate.Value
			If AllowOverride Then dbStoreItem.DisplayMode = drpDisplayMode.SelectedValue Else dbStoreItem.DisplayMode = String.Empty

			If ItemId <> 0 Then
				dbStoreItem.Update()
			Else
				ItemId = dbStoreItem.Insert()
				DB.ExecuteSQL("UPDATE StoreItemAttributeTemp SET ItemId = " & dbStoreItem.ItemId & " WHERE Guid = " & DB.Quote(Guid))
			End If

            'Save Selected Departments
            dbStoreItem.RemoveDepartmentItems()
            dbStoreItem.InsertDepartmentItems(ctrlStoreDepartmentTree.CheckedList)

            'Save Features
            dbStoreItem.DeleteFromAllStoreFeatures()
            dbStoreItem.DeleteUniqueFeatures(cblFeatures.SelectedValues)
            dbStoreItem.InsertToStoreFeatures(cblFeatures.SelectedValues)

			StoreItemAttributeRow.SaveTempAttributes(DB, ItemId, Guid)
			Dim dt As DataTable = DB.GetDataTable("SELECT FinalSKU FROM (SELECT FinalSKU FROM StoreItemAttribute WHERE FinalSKU IS NOT NULL UNION SELECT SKU AS FinalSKU FROM StoreItem WHERE TemplateId NOT IN (SELECT TemplateId FROM StoreItemTemplate WHERE IsAttributes = 1)) tmp GROUP BY FinalSKU HAVING COUNT(*) > 1")
			If Not dt.Rows.Count = 0 Then
				Dim SKUs As String = String.Empty
				For Each r As DataRow In dt.Rows
					If r("FinalSKU") = String.Empty Then
						SKUs &= IIf(SKUs = String.Empty, "", "<br />") & "<i>Empty SKU!</i>"
					Else
						SKUs &= IIf(SKUs = String.Empty, "", "<br />") & r("FinalSKU")
					End If
				Next
				Throw New ApplicationException("There were duplicate SKUs found for the following SKU(s). Please correct these errors and try again.<br />" & SKUs)
			End If

            DB.CommitTransaction()

			'Invalidate attribute cache
			Cache.Remove("dtTree_" & dbStoreItem.ItemId)
			Cache.Remove("dtAttribute_" & dbStoreItem.TemplateId)
			Cache.Remove("dtAttributeTable_" & dbStoreItem.ItemId)

			If fuImage.NewFileName <> String.Empty Or fuImage.MarkedToDelete Then
				fuImage.RemoveOldFile()
				fuImage.RemoveOldFile("/assets/item/cart/")
				fuImage.RemoveOldFile("/assets/item/related/")
				fuImage.RemoveOldFile("/assets/item/featured/")
				fuImage.RemoveOldFile("/assets/item/large/")
				fuImage.RemoveOldFile("/assets/item/regular/")
			End If

            DB.Close()
            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As ApplicationException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ex.Message)
        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex) & ex.Message)
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
		StoreItemAttributeRow.RemoveTemporaryRecords(DB, Guid)
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?ItemId=" & ItemId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

	Protected Sub drpTemplateId_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles drpTemplateId.SelectedIndexChanged
		rfvSKU.Enabled = DB.ExecuteScalar("select count(*) from storeitemtemplateattribute where templateid = " & DB.Number(drpTemplateId.SelectedValue)) > 0
		BindAttributes()
		RefreshInventoryAttribute()
	End Sub

	Private Sub RefreshInventoryAttribute()
		If Not drpTemplateId.SelectedValue = String.Empty Then
			Dim dvTemplate As DataView = DB.GetDataTable("exec sp_GetTemplateAttributeTreeByTemplate " & DB.Number(drpTemplateId.SelectedValue)).DefaultView
			dvTemplate.RowFilter = "IsInventoryManagement = 1"
			If dvTemplate.Count > 0 Then
				InventoryControlledAttributeId = dvTemplate(dvTemplate.Count - 1)("TemplateAttributeId")
			End If
		Else
			InventoryControlledAttributeId = 0
		End If
	End Sub

	Private Sub BindAttributes()
		trdm.Visible = AllowOverride
		ra.ItemTemplate = LoadTemplate("/controls/Attributes/TemplateAttributeTemplate.ascx")
		ra.DataSource = StoreItemTemplateAttributeRow.GetTemplates(DB, IIf(drpTemplateId.SelectedValue = String.Empty, 0, drpTemplateId.SelectedValue), 0, Guid, 0)
		ra.DataBind()
	End Sub

	Private Sub ra_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles ra.ItemDataBound
		If e.Item.ItemType = ListItemType.Header Then
			sender.ID = "at"
			Exit Sub
		ElseIf Not e.Item.ItemType = ListItemType.Item AndAlso Not e.Item.ItemType = ListItemType.AlternatingItem Then
			Exit Sub
		End If

		Dim rl As Repeater = e.Item.Controls(0).FindControl("rl")
		Dim SQL As String = "select top 1 SortOrder from storeitemtemplateattribute where templateattributeid = " & e.Item.DataItem("TemplateAttributeId")
		Dim ParentSortOrder As Integer = DB.ExecuteScalar(SQL)
		Dim btnSave As OneClickButton = e.Item.Controls(0).FindControl("btnSave")
		Dim t As HtmlGenericControl = e.Item.Controls(0).FindControl("t")
		Dim lt As LinkButton = e.Item.Controls(0).FindControl("lt")
		Dim c As LinkButton = e.Item.Controls(0).FindControl("c")
		Dim ShowSwatchs As Boolean = False
		Dim txtValue As TextBox = e.Item.Controls(0).FindControl("txtValue")
		Dim txtSKU As TextBox = e.Item.Controls(0).FindControl("txtSKU")
		Dim txtPrice As TextBox = e.Item.Controls(0).FindControl("txtPrice")
		Dim txtWeight As TextBox = e.Item.Controls(0).FindControl("txtWeight")
		Dim txtImageAlt As TextBox = e.Item.Controls(0).FindControl("txtImageAlt")
		Dim txtProductAlt As TextBox = e.Item.Controls(0).FindControl("txtProductAlt")
		Dim drpValue As DropDownList = e.Item.Controls(0).FindControl("drpValue")
		Dim rfvValue As RequiredFieldValidator = e.Item.Controls(0).FindControl("rfvValue")
		Dim divImageName As HtmlGenericControl = e.Item.Controls(0).FindControl("divImageName")
		Dim hdnImageName As HtmlInputHidden = e.Item.Controls(0).FindControl("hdnImageName")
		Dim divEdit As HtmlGenericControl = e.Item.Controls(0).FindControl("divEdit")
		Dim a As OneClickButton = e.Item.Controls(0).FindControl("a")
		Dim phImage As PlaceHolder = e.Item.Controls(0).FindControl("phImage")
		Dim phProductImage As PlaceHolder = e.Item.Controls(0).FindControl("phProductImage")
		Dim tdErrorMessage As HtmlTableCell = e.Item.Controls(0).FindControl("tdErrorMessage")
		Dim divProductImageName As HtmlGenericControl = e.Item.Controls(0).FindControl("divProductImageName")
		Dim hdnParentTempAttributeId As HtmlInputHidden = e.Item.Controls(0).FindControl("hdnParentTempAttributeId")
		Dim hdnTempAttributeId As HtmlInputHidden = e.Item.Controls(0).FindControl("hdnTempAttributeId")
		Dim Parent As HtmlInputHidden = CType(sender, Repeater).NamingContainer.NamingContainer.NamingContainer.FindControl("hdnTempAttributeId")
		Dim phi As PlaceHolder = e.Item.Controls(0).FindControl("phi")

		phi.Visible = EnableAttributeInventoryManagement AndAlso InventoryControlledAttributeId = e.Item.DataItem("TemplateAttributeId")

		If e.Item.DataItem("FunctionType") = "FreeText" Then
			txtValue.Visible = True
			drpValue.Visible = False
			rfvValue.ControlToValidate = "txtValue"
		Else
			txtValue.Visible = False
			drpValue.Visible = True
			rfvValue.ControlToValidate = "drpValue"
			Select Case e.Item.DataItem("FunctionType")
				Case "SpecifyValue"
					Dim values() As String = System.Text.RegularExpressions.Regex.Split(e.Item.DataItem("SpecifyValue"), vbCrLf)
					drpValue.Items.Add(New ListItem("", ""))
					For Each s As String In values
						drpValue.Items.Add(New ListItem(Trim(s), Trim(s)))
					Next

				Case "LookupTable"
					Dim s As String = String.Empty
					s &= "cast(" & e.Item.DataItem("LookupColumn") & " as varchar(50))"
					s &= " + '|'"
					If Not IsDBNull(e.Item.DataItem("SKUColumn")) Then s &= " + coalesce(cast(" & e.Item.DataItem("SKUColumn") & " as varchar(50)),'')"
					s &= " + '|'"
					If Not IsDBNull(e.Item.DataItem("PriceColumn")) Then s &= " + cast(coalesce(" & e.Item.DataItem("PriceColumn") & ",0) as varchar(50)) "
					s &= " + '|'"
					If Not IsDBNull(e.Item.DataItem("WeightColumn")) Then s &= " + cast(coalesce(" & e.Item.DataItem("WeightColumn") & ",0) as varchar(50)) "
					s &= " + '|'"
					If Not IsDBNull(e.Item.DataItem("SwatchColumn")) Then s &= " + coalesce(cast(" & e.Item.DataItem("SwatchColumn") & " as varchar(50)),'')"
					s &= " + '|'"
					If Not IsDBNull(e.Item.DataItem("SwatchAltColumn")) Then s &= " + coalesce(cast(" & e.Item.DataItem("SwatchAltColumn") & " as varchar(50)),'')"

					SQL = "select " & Core.ProtectParam(e.Item.DataItem("LookupColumn")) & IIf(CBool(e.Item.DataItem("IncludeSKU")) AndAlso Not IsDBNull(e.Item.DataItem("SKUColumn")), " + case when SKU is NULL then '' else ' (' + " & Core.ProtectParam(IIf(IsDBNull(e.Item.DataItem("SKUColumn")), "", e.Item.DataItem("SKUColumn"))) & " + ')' end ", "") & " as " & Core.ProtectParam(e.Item.DataItem("LookupColumn")) & ", " & Core.ProtectParam(s) & " as Value from " & Core.ProtectParam(e.Item.DataItem("LookupTable")) & " order by " & Core.ProtectParam(e.Item.DataItem("LookupColumn"))
					drpValue.DataSource = DB.GetDataTable(SQL)
					drpValue.DataTextField = Core.ProtectParam(e.Item.DataItem("LookupColumn"))
					drpValue.DataValueField = "Value"
					drpValue.DataBind()
					drpValue.Items.Insert(0, New ListItem("", ""))
					drpValue.Attributes.Add("onchange", "var a = this.value.split('|');document.getElementById('" & txtSKU.ClientID & "').value = a[1];document.getElementById('" & txtPrice.ClientID & "').value = a[2];document.getElementById('" & txtWeight.ClientID & "').value = a[3];if(a[4] != ''){document.getElementById('" & divImageName.ClientID & "').innerHTML = '<img src=""/assets/item/swatch/' + a[4] + '"" />';document.getElementById('" & hdnImageName.ClientID & "').value = a[4];}else{if(document.getElementById('" & divImageName.ClientID & "')){document.getElementById('" & divImageName.ClientID & "').innerHTML = '';document.getElementById('" & hdnImageName.ClientID & "').value = '';}}if(document.getElementById('" & txtImageAlt.ClientID & "')){document.getElementById('" & txtImageAlt.ClientID & "').value = a[5];}")

			End Select
		End If

		If ra Is sender OrElse Expanded.IndexOf(t.ClientID) > -1 Then
			t.Visible = True
			lt.Text = "Hide " & e.Item.DataItem("AttributeName")
		Else
			t.Visible = False
			lt.Text = "Show " & e.Item.DataItem("AttributeName")
		End If
		If Not ra Is sender Then c.Visible = t.Visible

		'only bind if necessary
		If Expanded.IndexOf(t.ClientID) > -1 OrElse sender Is ra Then
			If ViewState("EditOpen") = divEdit.ClientID Then
				tdErrorMessage.InnerHtml = ViewState("EditErrorMessage")
				tdErrorMessage.Visible = True
				ViewState("EditOpen") = Nothing
				ViewState("EditErrorMessage") = Nothing
				If Not Session("EditAttribute") Is Nothing Then
					Dim dbAttribute As StoreItemAttributeTempRow = Session("EditAttribute")
					Session("EditAttribute") = Nothing
					If e.Item.DataItem("FunctionType") = "LookupTable" Then
						For Each li As ListItem In drpValue.Items
							If li.Value.Split("|")(0) = dbAttribute.AttributeValue Then
								drpValue.SelectedIndex = drpValue.Items.IndexOf(drpValue.Items.FindByValue(li.Value))
								Exit For
							End If
						Next
					Else
						drpValue.SelectedValue = dbAttribute.AttributeValue
					End If
					txtValue.Text = dbAttribute.AttributeValue
					txtSKU.Text = dbAttribute.SKU
					txtPrice.Text = dbAttribute.Price
					txtWeight.Text = dbAttribute.Weight
					chkIsActive.Checked = dbAttribute.IsActive
					txtImageAlt.Text = dbAttribute.ImageAlt
					txtProductAlt.Text = dbAttribute.ProductAlt
					If Not dbAttribute.ImageName = Nothing Then divImageName.InnerHtml = "<img src=""/assets/item/swatch/" & dbAttribute.ImageName & """/>" Else divImageName.InnerHtml = String.Empty
					If Not dbAttribute.ProductImage = Nothing Then divProductImageName.InnerHtml = "<img src=""/assets/item/cart/" & dbAttribute.ProductImage & """/>" Else divProductImageName.InnerHtml = String.Empty
				End If
				divEdit.Visible = True
				a.Visible = False
				lt.Text = "Hide " & e.Item.DataItem("AttributeName")
			End If
			phImage.Visible = e.Item.DataItem("AttributeType") = "swatch"
			AddHandler rl.ItemDataBound, AddressOf rl_ItemDataBound
			AddHandler rl.ItemCommand, AddressOf rl_ItemCommand
			Dim dt As DataTable = StoreItemAttributeTempRow.GetAttributes(DB, e.Item.DataItem("TemplateAttributeId"), Guid, e.Item.DataItem("ParentTempAttributeId"))
			rl.DataSource = dt
			rl.DataBind()

			If dt.Rows.Count > 0 Then ShowSwatchs = e.Item.DataItem("AttributeType") = "swatch"
			rl.Controls(0).FindControl("phih").Visible = EnableAttributeInventoryManagement AndAlso InventoryControlledAttributeId = e.Item.DataItem("TemplateAttributeId")
			If Not ShowSwatchs Then
				rl.Controls(0).FindControl("thSwatch").Visible = False
			End If
		End If
	End Sub

	Private Sub rl_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs)
		If Not e.Item.ItemType = ListItemType.Item AndAlso Not e.Item.ItemType = ListItemType.AlternatingItem Then
			Exit Sub
		End If

		Dim ra As Repeater = e.Item.Controls(0).FindControl("ra")
		Dim tdSwatch As HtmlTableCell = e.Item.Controls(0).FindControl("tdSwatch")
		Dim phie As PlaceHolder = e.Item.Controls(0).FindControl("phie")
		Dim rc As HtmlTableRow = e.Item.Controls(0).FindControl("rc")

		tdSwatch.Visible = e.Item.DataItem("AttributeType") = "swatch"
		phie.Visible = EnableAttributeInventoryManagement AndAlso InventoryControlledAttributeId = e.Item.DataItem("TemplateAttributeId")

		Dim dtAttributes As DataTable = StoreItemTemplateAttributeRow.GetTemplates(DB, drpTemplateId.SelectedValue, e.Item.DataItem("TemplateAttributeId"), Guid, e.Item.DataItem("TempAttributeId"))
		AddHandler ra.ItemDataBound, AddressOf ra_ItemDataBound
		AddHandler ra.ItemCommand, AddressOf ra_ItemCommand
		ra.ItemTemplate = LoadTemplate("/controls/Attributes/TemplateAttributeTemplate.ascx")
		ra.DataSource = dtAttributes
		ra.DataBind()
		rc.Visible = dtAttributes.Rows.Count > 0
	End Sub

	Private Sub rl_ItemCommand(ByVal sender As Object, ByVal e As RepeaterCommandEventArgs)
		Dim rl As Repeater = sender
		Dim args As New RepeaterCommandEventArgs(rl.NamingContainer.Parent, sender, CType(e, CommandEventArgs))
		ra_ItemCommand(rl.NamingContainer.Parent, args)
	End Sub

	Protected Sub ra_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles ra.ItemCommand
		Dim hdnFunctionType As HtmlInputHidden = e.Item.Controls(0).FindControl("hdnFunctionType")
		Dim a As OneClickButton = e.Item.Controls(0).FindControl("a")
		Dim divEdit As HtmlGenericControl = e.Item.Controls(0).FindControl("divEdit")
		Dim hdnTempAttributeId As HtmlInputHidden = e.Item.Controls(0).FindControl("hdnTempAttributeId")
		Dim TempAttributeId As Integer = IIf(e.CommandArgument = String.Empty, 0, e.CommandArgument)
		If e.CommandName = "Save" AndAlso Not hdnTempAttributeId.Value = String.Empty AndAlso Not hdnTempAttributeId.Value = "0" Then TempAttributeId = hdnTempAttributeId.Value
		Dim txtValue As TextBox = e.Item.Controls(0).FindControl("txtValue")
		Dim drpValue As DropDownList = e.Item.Controls(0).FindControl("drpValue")
		Dim txtSKU As TextBox = e.Item.Controls(0).FindControl("txtSKU")
		Dim txtPrice As TextBox = e.Item.Controls(0).FindControl("txtPrice")
		Dim txtWeight As TextBox = e.Item.Controls(0).FindControl("txtWeight")
		Dim hdnImageName As HtmlInputHidden = e.Item.Controls(0).FindControl("hdnImageName")
		Dim divImageName As HtmlGenericControl = e.Item.Controls(0).FindControl("divImageName")
		Dim hdnProductImageName As HtmlInputHidden = e.Item.Controls(0).FindControl("hdnProductImageName")
		Dim divProductImageName As HtmlGenericControl = e.Item.Controls(0).FindControl("divProductImageName")
		Dim hdnTemplateAttributeId As HtmlInputHidden = e.Item.Controls(0).FindControl("hdnTemplateAttributeId")
		Dim dbAttribute As StoreItemAttributeTempRow = StoreItemAttributeTempRow.GetRow(DB, TempAttributeId)
		Dim btnAttributeSave As OneClickButton = e.Item.Controls(0).FindControl("btnAttributeSave")
		Dim dbTemplateAttribute As StoreItemTemplateAttributeRow = StoreItemTemplateAttributeRow.GetRow(DB, hdnTemplateAttributeId.Value)
		Dim phImage As PlaceHolder = e.Item.Controls(0).FindControl("phImage")
		Dim lnkUpload As LinkButton = e.Item.Controls(0).FindControl("lnkUpload")
		Dim lnkClear As HtmlAnchor = e.Item.Controls(0).FindControl("lnkClear")
		Dim divUpload As HtmlGenericControl = e.Item.Controls(0).FindControl("divUpload")
		Dim frmUpload As HtmlGenericControl = e.Item.Controls(0).FindControl("frmUpload")
		Dim phProductImage As PlaceHolder = e.Item.Controls(0).FindControl("phProductImage")
		Dim lnkProductImageUpload As LinkButton = e.Item.Controls(0).FindControl("lnkProductImageUpload")
		Dim lnkProductImageClear As HtmlAnchor = e.Item.Controls(0).FindControl("lnkProductImageClear")
		Dim divProductImageUpload As HtmlGenericControl = e.Item.Controls(0).FindControl("divProductImageUpload")
		Dim frmProductImageUpload As HtmlGenericControl = e.Item.Controls(0).FindControl("frmProductImageUpload")
		Dim chkIsActive As CheckBox = e.Item.Controls(0).FindControl("chkIsActive")
		Dim txtImageAlt As TextBox = e.Item.Controls(0).FindControl("txtImageAlt")
		Dim txtProductAlt As TextBox = e.Item.Controls(0).FindControl("txtProductAlt")
		Dim hdnParentTempAttributeId As HtmlInputHidden = e.Item.Controls(0).FindControl("hdnParentTempAttributeId")
		Dim t As HtmlGenericControl = e.Item.Controls(0).FindControl("t")
		Dim lt As LinkButton = e.Item.Controls(0).FindControl("lt")
		Dim c As LinkButton = e.Item.Controls(0).FindControl("c")
		Dim tdErrorMessage As HtmlTableCell = e.Item.Controls(0).FindControl("tdErrorMessage")
		Dim txtiq As TextBox = e.Item.Controls(0).FindControl("txtiq")
		Dim txtiw As TextBox = e.Item.Controls(0).FindControl("txtiw")
		Dim txtia As TextBox = e.Item.Controls(0).FindControl("txtia")
		Dim dia As DropDownList = e.Item.Controls(0).FindControl("dia")
		Dim dpbd As DatePicker = e.Item.Controls(0).FindControl("dpbd")
		Dim trbo As HtmlTableRow = e.Item.Controls(0).FindControl("trbo")

		If Not dia.SelectedValue = "Backorder" Then trbo.Style.Add("display", "none")
		dia.Attributes.Remove("onchange")
		dia.Attributes.Add("onchange", "if(this.value == 'Backorder'){document.getElementById('" & trbo.ClientID & "').style.display = 'block'}else{document.getElementById('" & trbo.ClientID & "').style.display = 'none';}")

		tdErrorMessage.Visible = True

		If chkExpandActive.Checked Then Expanded.Clear()
		Dim div As HtmlGenericControl = t
		While Not div Is Nothing
			If Expanded.IndexOf(div.ClientID) = -1 Then Expanded.Add(div.ClientID)
			If TypeOf div.NamingContainer.NamingContainer.NamingContainer.NamingContainer.NamingContainer.NamingContainer.NamingContainer Is RepeaterItem Then
				div = div.NamingContainer.NamingContainer.NamingContainer.NamingContainer.NamingContainer.NamingContainer.NamingContainer.Controls(0).FindControl("t")
			Else
				div = Nothing
			End If
		End While

		phProductImage.Visible = True
		lnkProductImageUpload.CommandName = ""
		lnkProductImageUpload.CommandArgument = hdnProductImageName.ClientID
		lnkProductImageUpload.OnClientClick = "ShowUploadFrame('" & divProductImageUpload.ClientID & "','" & frmProductImageUpload.ClientID & "','Lookup.aspx?type=product&hdnImageName=" & hdnProductImageName.ClientID & "&divImageName=" & divProductImageName.ClientID & "&divUpload=" & divProductImageUpload.ClientID & "&frmUpload=" & frmProductImageUpload.ClientID & "'); return false;"
		lnkProductImageClear.Attributes.Add("onclick", "document.getElementById('" & hdnProductImageName.ClientID & "').value = ''; document.getElementById('" & divProductImageName.ClientID & "').innerHTML = ''; return false;")

		If dbTemplateAttribute.AttributeType = "swatch" Then
			phImage.Visible = True
			lnkUpload.CommandName = ""
			lnkUpload.CommandArgument = hdnImageName.ClientID
			lnkUpload.OnClientClick = "ShowUploadFrame('" & divUpload.ClientID & "','" & frmUpload.ClientID & "','Lookup.aspx?type=swatch&hdnImageName=" & hdnImageName.ClientID & "&divImageName=" & divImageName.ClientID & "&divUpload=" & divUpload.ClientID & "&frmUpload=" & frmUpload.ClientID & "'); return false;"
			lnkClear.Attributes.Add("onclick", "document.getElementById('" & hdnImageName.ClientID & "').value = ''; document.getElementById('" & divImageName.ClientID & "').innerHTML = ''; return false;")
		Else
			phImage.Visible = False
			'phProductImage.Visible = False
		End If

		If e.CommandName = "Save" Then
			Page.Validate("Attributes")
			If Not IsValid Then Exit Sub
		End If

		If e.CommandName = "Update" Then
			hdnTempAttributeId.Value = TempAttributeId

		Else
			hdnTempAttributeId.Value = 0
		End If

		Select Case e.CommandName
			Case "Add"
				txtValue.Text = String.Empty
				txtSKU.Text = String.Empty
				txtPrice.Text = 0
				txtWeight.Text = 0
				hdnImageName.Value = String.Empty
				divImageName.InnerHtml = String.Empty
				hdnProductImageName.Value = String.Empty
				divProductImageName.InnerHtml = String.Empty
				btnAttributeSave.CommandArgument = String.Empty
				txtImageAlt.Text = String.Empty
				txtProductAlt.Text = String.Empty
				chkIsActive.Checked = False
				txtiq.Text = 0
				txtiw.Text = 0
				txtia.Text = 0
				dia.SelectedValue = String.Empty
				dpbd.Value = Nothing

				a.Visible = False
				divEdit.Visible = True
				t.Visible = True
				c.Visible = t.Visible AndAlso Not source Is ra
				lt.Text = lt.Text.Replace("Show ", "Hide ")

				Exit Sub

			Case "Toggle"
				t.Visible = Not t.Visible
				c.Visible = t.Visible
				If t.Visible Then lt.Text = lt.Text.Replace("Show ", "Hide ") Else lt.Text = lt.Text.Replace("Hide ", "Show ")
				If Not t.Visible AndAlso Expanded.IndexOf(t.ClientID) > -1 Then Expanded.RemoveAt(Expanded.IndexOf(t.ClientID))
				If Not t.Visible Then Exit Sub

			Case "Copy"
				StoreItemAttributeTempRow.CopyAttributes(DB, hdnTemplateAttributeId.Value, hdnParentTempAttributeId.Value, Guid)

			Case "Update"
				If hdnFunctionType.Value = "LookupTable" Then
					For Each li As ListItem In drpValue.Items
						If li.Value.Split("|")(0) = dbAttribute.AttributeValue Then
							drpValue.SelectedIndex = drpValue.Items.IndexOf(drpValue.Items.FindByValue(li.Value))
							Exit For
						End If
					Next
				Else
					drpValue.SelectedValue = dbAttribute.AttributeValue
				End If
				txtValue.Text = dbAttribute.AttributeValue
				txtSKU.Text = dbAttribute.SKU
				txtWeight.Text = dbAttribute.Weight
				txtPrice.Text = dbAttribute.Price
				btnAttributeSave.CommandArgument = dbAttribute.TempAttributeId
				hdnImageName.Value = dbAttribute.ImageName
				hdnProductImageName.Value = dbAttribute.ProductImage
				If Not dbAttribute.ImageName = Nothing Then divImageName.InnerHtml = "<img src=""/assets/item/swatch/" & dbAttribute.ImageName & """/>" Else divImageName.InnerHtml = String.Empty
				If Not dbAttribute.ProductImage = Nothing Then divProductImageName.InnerHtml = "<img src=""/assets/item/cart/" & dbAttribute.ProductImage & """/>" Else divProductImageName.InnerHtml = String.Empty
				txtImageAlt.Text = dbAttribute.ImageAlt
				txtProductAlt.Text = dbAttribute.ProductAlt
				chkIsActive.Checked = dbAttribute.IsActive
				txtiq.Text = dbAttribute.InventoryQty
				txtiw.Text = dbAttribute.InventoryWarningThreshold
				txtia.Text = dbAttribute.InventoryActionThreshold
				dia.SelectedValue = dbAttribute.InventoryAction
				dpbd.Value = dbAttribute.BackorderDate

				a.Visible = False
				divEdit.Visible = True

				Exit Sub

			Case "Delete"
				StoreItemAttributeTempRow.RemoveRow(DB, TempAttributeId)

			Case "Cancel"
				a.Visible = True
				divEdit.Visible = False

			Case "Save"
				If hdnFunctionType.Value = "FreeText" Then
					dbAttribute.AttributeValue = txtValue.Text
				Else
					If hdnFunctionType.Value = "LookupTable" Then
						dbAttribute.AttributeValue = drpValue.SelectedValue.Split("|")(0)
					Else
						dbAttribute.AttributeValue = drpValue.SelectedValue
					End If
				End If
				dbAttribute.ParentAttributeId = IIf(hdnParentTempAttributeId.Value = String.Empty, 0, hdnParentTempAttributeId.Value)
				dbAttribute.Price = txtPrice.Text
				dbAttribute.SKU = txtSKU.Text
				dbAttribute.TemplateAttributeId = hdnTemplateAttributeId.Value
				dbAttribute.Weight = txtWeight.Text
				dbAttribute.ItemId = ItemId
				dbAttribute.Guid = Guid
				dbAttribute.ImageName = hdnImageName.Value
				dbAttribute.ProductImage = hdnProductImageName.Value
				dbAttribute.ImageAlt = txtImageAlt.Text
				dbAttribute.ProductAlt = txtProductAlt.Text
				dbAttribute.IsActive = chkIsActive.Checked
				If IsNumeric(txtiq.Text) Then dbAttribute.InventoryQty = txtiq.Text Else dbAttribute.InventoryQty = Nothing
				If IsNumeric(txtiw.Text) Then dbAttribute.InventoryWarningThreshold = txtiw.Text Else dbAttribute.InventoryWarningThreshold = Nothing
				If IsNumeric(txtia.Text) Then dbAttribute.InventoryActionThreshold = txtia.Text Else dbAttribute.InventoryActionThreshold = 0
				dbAttribute.InventoryAction = dia.SelectedValue
				If IsDate(dpbd.Value) Then dbAttribute.BackorderDate = dpbd.Value Else dbAttribute.BackorderDate = Nothing

				Dim IsFinalAttribute As Boolean = DB.ExecuteScalar("select top 1 coalesce(TemplateAttributeId,0) from StoreItemTemplateAttribute where TemplateId = " & drpTemplateId.SelectedValue & " and SortOrder > " & dbTemplateAttribute.SortOrder) = 0

				Dim sError As String = String.Empty
				If Not IsFinalAttribute Then
					dbAttribute.ValidateLocalSettings(sError)
					If Not sError = String.Empty Then
						ViewState("EditOpen") = divEdit.ClientID
						ViewState("EditErrorMessage") = sError
						Session("EditAttribute") = dbAttribute
						Exit Select
					End If
                End If

                If EnableInventoryManagement AndAlso EnableAttributeInventoryManagement AndAlso dbAttribute.InventoryAction = "Disable" AndAlso dbAttribute.InventoryQty <= dbAttribute.InventoryActionThreshold Then
                    dbAttribute.IsActive = False
				End If

				If dbAttribute.TempAttributeId = 0 Then
					dbAttribute.Insert()
				Else
					dbAttribute.Update()
				End If
				divEdit.Visible = False
				a.Visible = True

			Case "Up"
				Core.ChangeSortOrder(DB, "TempAttributeId", "StoreItemAttributeTemp", "SortOrder", "TemplateAttributeId=" & DB.Number(hdnTemplateAttributeId.Value) & " AND COALESCE(ParentAttributeId,0) = " & DB.Number(hdnParentTempAttributeId.Value), TempAttributeId, "UP")

			Case "Down"
				Core.ChangeSortOrder(DB, "TempAttributeId", "StoreItemAttributeTemp", "SortOrder", "TemplateAttributeId=" & DB.Number(hdnTemplateAttributeId.Value) & " AND COALESCE(ParentAttributeId,0) = " & DB.Number(hdnParentTempAttributeId.Value), TempAttributeId, "DOWN")

		End Select

		BindAttributes()
	End Sub

	Protected Sub cvImageName_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs)
		Dim hdnImageName As HtmlInputHidden = CType(source, CustomValidator).Parent.FindControl("hdnImageName")
		Dim divImageName As HtmlGenericControl = CType(source, CustomValidator).Parent.FindControl("divImageName")
		If hdnImageName.Value = String.Empty Then
			args.IsValid = False
			divImageName.InnerHtml = String.Empty
		Else
			args.IsValid = True
		End If
	End Sub

	Protected Sub cvProductImageName_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs)
		Dim hdnImageName As HtmlInputHidden = CType(source, CustomValidator).Parent.FindControl("hdnProductImageName")
		Dim divImageName As HtmlGenericControl = CType(source, CustomValidator).Parent.FindControl("divProductImageName")
		If hdnImageName.Value = String.Empty Then
			args.IsValid = False
			divImageName.InnerHtml = String.Empty
		Else
			args.IsValid = True
		End If
	End Sub

End Class
