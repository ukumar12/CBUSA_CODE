Imports Components
Imports DataLayer
Imports Controls

Partial Class StoreItemAttributes
    Inherits BaseControl

	Protected dbItem As StoreItemRow
	Private htControls As New Hashtable
	Private PreviouslySet As Boolean = False
	Private FinalLevelAttributes As New ArrayList
	Private InventoryControlledAttributeId As Integer
	Private EnableInventoryManagement As Boolean
	Private EnableAttributeInventoryManagement As Boolean
	Private MinutesToCacheAttributes As Integer

#Region "Properties"
	Private m_DisplayMode As String
	Public Property DisplayMode() As String
		Get
			Return m_DisplayMode
		End Get
		Set(ByVal value As String)
			m_DisplayMode = value
		End Set
	End Property

	'The Attribute Tree
	Private m_dtTree As DataTable
	Public Property dtTree() As DataTable
		Get
			'If Cache("dtTree_" & ItemId) Is Nothing Then
			m_dtTree = DB.GetDataTable("exec sp_GetAttributeTree " & ItemId)
			m_dtTree.TableName = "Tree"
			Dim dv As DataView = m_dtTree.DefaultView
			dv.RowFilter = "IsInventoryManagement = 1"
			If dv.Count > 0 Then
				InventoryControlledAttributeId = dv(dv.Count - 1)("TemplateAttributeId")
			End If
			Return m_dtTree
			'If MinutesToCacheAttributes = 0 Then Return m_dtTree
			'Cache.Insert("dtTree_" & ItemId, m_dtTree, Nothing, DateAdd(DateInterval.Minute, MinutesToCacheAttributes, Now), TimeSpan.Zero)
			'End If
			'Return Cache("dtTree_" & ItemId)
		End Get
		Set(ByVal value As DataTable)
			'If MinutesToCacheAttributes = 0 Then m_dtTree = value Else Cache("dtTree_" & ItemId) = value
		End Set
	End Property

	'The Template Attribute Tree
	Private m_dtAttribute As DataTable
	Public Property dtAttribute() As DataTable
		Get
			'If Cache("dtAttribute_" & dbItem.TemplateId) Is Nothing Then
			m_dtAttribute = DB.GetDataTable("exec sp_GetTemplateAttributeTreeByTemplate " & dbItem.TemplateId)
			m_dtAttribute.TableName = "AttributeTree"
			Return m_dtAttribute
			'If MinutesToCacheAttributes = 0 Then Return m_dtAttribute
			'Cache.Insert("dtAttribute_" & dbItem.TemplateId, m_dtAttribute, Nothing, DateAdd(DateInterval.Minute, MinutesToCacheAttributes, Now), TimeSpan.Zero)
			'End If
			'Return Cache("dtAttribute_" & dbItem.TemplateId)
		End Get
		Set(ByVal value As DataTable)
			'If MinutesToCacheAttributes = 0 Then m_dtAttribute = value Else Cache("dtAttribute_" & dbItem.TemplateId) = value
		End Set
	End Property

	'The Table Attribute Tree
	Private m_dtAttributeTable As DataTable
	Public Property dtAttributeTable() As DataTable
		Get
			'If Cache("dtAttributeTable_" & dbItem.ItemId) Is Nothing Then
			m_dtAttributeTable = DB.GetDataTable("exec sp_GetAttributeTreeTableLayout " & dbItem.ItemId & ", " & dbItem.TemplateId)
			m_dtAttributeTable.TableName = "AttributeTree"
			Return m_dtAttributeTable
			'If MinutesToCacheAttributes = 0 Then Return m_dtAttributeTable
			'Cache.Insert("dtAttributeTable_" & dbItem.ItemId, m_dtAttributeTable, Nothing, DateAdd(DateInterval.Minute, MinutesToCacheAttributes, Now), TimeSpan.Zero)
			'End If
			'Return Cache("dtAttributeTable_" & dbItem.ItemId)
		End Get
		Set(ByVal value As DataTable)
			'If MinutesToCacheAttributes = 0 Then m_dtAttributeTable = value Else Cache("dtAttributeTable_" & dbItem.ItemId) = value
		End Set
	End Property

	'The Selected Attribute Collection
	Private m_Values As ItemAttributeCollection
	Public Property Values() As ItemAttributeCollection
		Get
			Select Case DisplayMode
				Case "TableLayout"
					Return Nothing
				Case Else
					Return ViewState("Values")
			End Select
		End Get
		Set(ByVal value As ItemAttributeCollection)
			Select Case DisplayMode
				Case "TableLayout"
				Case Else
					ViewState("Values") = value
			End Select
		End Set
	End Property

	Public ReadOnly Property AttributeRepeater() As Repeater
		Get
			Return rpt
		End Get
	End Property

    Public Property ItemId() As Integer
        Get
            Return ViewState("ItemId")
        End Get
        Set(ByVal value As Integer)
            ViewState("ItemId") = value
        End Set
    End Property

	Private m_ltlPrice
	Public Property ltlPrice() As Literal
        Get
			Return m_ltlPrice
        End Get
		Set(ByVal value As Literal)
			m_ltlPrice = value
        End Set
    End Property

	Private m_ltlPriceSale
	Public Property ltlPriceSale() As Literal
		Get
			Return m_ltlPriceSale
		End Get
		Set(ByVal value As Literal)
			m_ltlPriceSale = value
		End Set
	End Property

	Private m_ltlSalePrice
	Public Property ltlSalePrice() As Literal
		Get
			Return m_ltlSalePrice
		End Get
		Set(ByVal value As Literal)
			m_ltlSalePrice = value
		End Set
	End Property

	Private m_ProductImage As HtmlImage
	Public Property ProductImage() As HtmlImage
		Get
			Return m_ProductImage
		End Get
		Set(ByVal value As HtmlImage)
			m_ProductImage = value
		End Set
	End Property

	Private m_ProductImageEnlarge As HtmlImage
	Public Property ProductImageEnlarge() As HtmlImage
		Get
			Return m_ProductImageEnlarge
		End Get
		Set(ByVal value As HtmlImage)
			m_ProductImageEnlarge = value
		End Set
	End Property

	Private m_mvAddToCart As MultiView
	Public Property mvAddToCart() As MultiView
		Get
			Return m_mvAddToCart
		End Get
		Set(ByVal value As MultiView)
			m_mvAddToCart = value
		End Set
	End Property

	Private m_ltlBackorder As Literal
	Public Property ltlBackorder() As Literal
		Get
			Return m_ltlBackorder
		End Get
		Set(ByVal value As Literal)
			m_ltlBackorder = value
		End Set
	End Property

	Private m_EmailMe As Object
	Public Property EmailMe() As Object
		Get
			Return m_EmailMe
		End Get
		Set(ByVal value As Object)
			m_EmailMe = value
		End Set
	End Property
#End Region

#Region "Bind Attributes"
	Private Sub BindData()
		If dtAttribute.Rows.Count = 0 Then
			Me.Visible = False
			Values.Clear()
			Exit Sub
		End If

		Dim AdditionalPrice As Double = 0

		For Each r As DataRow In dtAttribute.Rows

			Dim att As Controls.Attribute = Me.FindControl("att_" & r("TemplateAttributeId"))
			Dim ltl As Literal = att.FindControl("ltlAttributeName")
			Dim drp As DropDownList = att.FindControl("drpAttribute")
			Dim hdn As HtmlInputHidden = att.FindControl("hdnAttribute")
			Dim ph As PlaceHolder = att.FindControl("phAttribute")

			Dim i As Integer

			'Store the current value
			Dim Value As String = hdn.Value

			ltl.Text = r("AttributeName")

			'Build the RowFilter
			Dim RowFilter As String = "TemplateAttributeId = " & r("TemplateAttributeId")
			If Not IsDBNull(r("ParentId")) AndAlso Values.Count > 0 Then
				RowFilter &= " AND ParentAttributeId IN ("
				For i = 0 To Values.Count - 1
					RowFilter &= IIf(i = 0, "", ",") & Values(i).ItemAttributeId
				Next
				RowFilter &= ")"
			End If

			'Copy contents of the tree to the filtered view
			Dim dv As New DataView
			dv.Table = dtTree
			dv.RowFilter = RowFilter
			dv.Sort = "MasterSortOrder"

			'Loop through the filtered table to see if we have the value we need
			Dim ClearValue As Boolean = True
			For i = 0 To dv.Count - 1

				'If we've just come to this page and our values are already set
				If Not IsPostBack AndAlso Values.Count > 0 AndAlso (Value = Nothing OrElse Value = "0") Then
					For j As Integer = 0 To Values.Count - 1
						If Values(j).ItemAttributeId = dv(i)("ItemAttributeId") Then
							Value = Values(j).ItemAttributeId
							Exit For
						End If
					Next
				End If

				'If we've found our match make sure we don't clear the value
				If Convert.ToString(dv(i)("ItemAttributeId")) = Value Then
					ClearValue = False
					Exit For
				End If
			Next

			'Clear the value if it is invalid
			If ClearValue Then Value = Nothing

			Select Case r("AttributeType")
				Case "dropdown"
					CreateDropDownList(r, AdditionalPrice, drp, hdn, ph, dv, Value)
				Case "swatch"
					CreateSwatchList(r, AdditionalPrice, drp, hdn, ph, dv, Value)
				Case "radio"
					CreateRadioButtonList(r, AdditionalPrice, drp, hdn, ph, dv, Value)
			End Select
		Next

		If dbItem.IsOnSale Then
			If Not ltlPriceSale Is Nothing Then ltlPriceSale.Text = FormatCurrency(dbItem.Price + AdditionalPrice)
			If Not ltlSalePrice Is Nothing Then ltlSalePrice.Text = FormatCurrency(dbItem.SalePrice + AdditionalPrice)
		Else
			If Not ltlPrice Is Nothing Then ltlPrice.Text = FormatCurrency(dbItem.Price + AdditionalPrice)
		End If
	End Sub

	Private Sub CreateDropDownList(ByVal r As DataRow, ByRef AdditionalPrice As Double, ByRef drp As DropDownList, ByRef hdn As HtmlInputHidden, ByRef ph As PlaceHolder, ByVal dv As DataView, ByVal Value As String)
		drp.Items.Clear()

		Dim a As New ItemAttribute, i As Integer

		'Bind the attributes
		For i = 0 To dv.Count - 1
			Dim el As DataRowView = dv(i)
            Dim text As String = el("AttributeValue")
            If el("Price") > 0 Then
                text &= " [Add " & FormatCurrency(el("Price")) & "]"
            End If
            drp.Items.Add(New ListItem(text, el("ItemAttributeId")))

			'If this item matches the value that has been set
			If (Value = Nothing AndAlso i = 0) OrElse Value = Convert.ToString(el("ItemAttributeId")) Then
				LoadItemAttribute(a, el)
				AdditionalPrice += a.Price
				hdn.Value = a.ItemAttributeId
                End If
            Next

		'Restore the currently selected value if set and still available
		If Not Value = Nothing Then
			drp.SelectedValue = Value
		End If
	End Sub

	Private Sub CreateSwatchList(ByVal r As DataRow, ByRef AdditionalPrice As Double, ByRef drp As DropDownList, ByRef hdn As HtmlInputHidden, ByRef ph As PlaceHolder, ByVal dv As DataView, ByVal Value As String)
		Dim a As New ItemAttribute, i As Integer

		''Uncomment this line to enable DropDownList AND Swatches
		'CreateDropDownList(r, AdditionalPrice, drp, hdn, ph, dv, Value)

		For i = 0 To dv.Count - 1
			Dim el As DataRowView = dv(i)

			Dim div As HtmlGenericControl = ph.FindControl("div_" & el("TemplateAttributeId"))
			Dim lnk As LinkButton = ph.FindControl("lnk_" & i)

			lnk.CommandArgument = el("ItemAttributeId")
			lnk.Visible = True

			'Determine alt text for image
			Dim alt As String, CssClass As String
			If Not IsDBNull(el("ImageAlt")) Then
				alt = el("ImageAlt")
			Else
				alt = el("AttributeValue")
			End If
			alt = alt.Replace("""", "'")

			Dim text As String = el("AttributeValue")
			If el("Price") > 0 Then
				text &= " [Add " & FormatCurrency(el("Price")) & "]"
			End If

			'If this item matches the value that has been set
			If (Value = Nothing AndAlso i = 0) OrElse Value = Convert.ToString(el("ItemAttributeId")) Then
				LoadItemAttribute(a, el)
				AdditionalPrice += a.Price
				hdn.Value = a.ItemAttributeId
				div.InnerHtml = text
				lnk.Attributes.Add("onclick", "return false;")
				CssClass = "swatchon"
			Else
				lnk.Attributes.Remove("onclick")
				CssClass = "swatchoff"
			End If

			If Not IsDBNull(el("ImageName")) Then
				lnk.Text = "<img src=""/assets/item/swatch/" & el("ImageName") & """ alt=""" & alt & """ class=""" & CssClass & """ />"
			Else
				lnk.Text = "<img src=""/assets/item/swatch/na.jpg"" alt=""" & alt & """ class=""" & CssClass & """ />"
			End If
        Next

		'Hide unnecessary swatches
		For i = dv.Count To htControls("ControlCount_" & r("TemplateAttributeId")) - 1
			ph.FindControl("lnk_" & i).Visible = False
		Next
	End Sub

	Private Sub CreateRadioButtonList(ByVal r As DataRow, ByRef AdditionalPrice As Double, ByRef drp As DropDownList, ByRef hdn As HtmlInputHidden, ByRef ph As PlaceHolder, ByVal dv As DataView, ByVal Value As String)
		Dim a As New ItemAttribute, i As Integer

		For i = 0 To dv.Count - 1
			Dim el As DataRowView = dv(i)

			Dim rb As RadioButton = ph.FindControl("rb_" & i)
			rb.Visible = True

			Dim text As String = el("AttributeValue")
			If el("Price") > 0 Then
				text &= " [Add " & FormatCurrency(el("Price")) & "]"
			End If

			rb.Text = text
			rb.Attributes.Add("Value", el("ItemAttributeId"))

			'If this item matches the value that has been set
			If (Value = Nothing AndAlso i = 0) OrElse Value = Convert.ToString(el("ItemAttributeId")) Then
				LoadItemAttribute(a, el)
				rb.Checked = True
				AdditionalPrice += a.Price
				hdn.Value = a.ItemAttributeId
			Else
				rb.Checked = False
			End If
		Next

		'Hide unnecessary radio buttons
		For i = dv.Count To htControls("ControlCount_" & r("TemplateAttributeId")) - 1
			ph.FindControl("rb_" & i).Visible = False
		Next
	End Sub

	Private Sub LoadItemAttribute(ByRef a As ItemAttribute, ByVal r As DataRowView)
		If Not EmailMe Is Nothing Then EmailMe.Visible = False

		If r("TemplateAttributeId") = InventoryControlledAttributeId Then
			Dim InventoryQty As Integer = dbItem.InventoryQty
			Dim BackorderDate As DateTime = dbItem.BackorderDate
			Dim InventoryAction As String = dbItem.InventoryAction
			Dim InventoryActionThreshold As Integer = SysParam.GetValue(DB, "InventoryActionThreshold")
			If Not dbItem.InventoryActionThreshold = Nothing Then InventoryActionThreshold = dbItem.InventoryActionThreshold

			If EnableAttributeInventoryManagement Then
				InventoryQty = r("InventoryQty")
				If Not IsDBNull(r("InventoryAction")) Then InventoryAction = r("InventoryAction")
				If Not IsDBNull(r("InventoryActionThreshold")) Then InventoryActionThreshold = r("InventoryActionThreshold")
				If Not IsDBNull(r("BackorderDate")) Then BackorderDate = r("BackorderDate") Else BackorderDate = Nothing
			End If

			If Not EnableInventoryManagement OrElse InventoryQty > InventoryActionThreshold OrElse InventoryAction = "Backorder" Then
				If Not mvAddToCart Is Nothing AndAlso Not mvAddToCart.FindControl("vQty") Is Nothing Then mvAddToCart.SetActiveView(mvAddToCart.FindControl("vQty"))
				If EnableInventoryManagement AndAlso InventoryQty <= InventoryActionThreshold AndAlso InventoryAction = "Backorder" Then
					If Not EnableInventoryManagement OrElse InventoryQty > InventoryActionThreshold OrElse InventoryAction = "Backorder" Then
						If Not ltlBackorder Is Nothing Then ltlBackorder.Text = "<tr><td colspan=""2""><strong>Backorder</strong>" & IIf(Not BackorderDate = Nothing, "<br /><span class=""smaller"">Estimated Ship Date: " & BackorderDate.ToShortDateString & "</span>", "") & "</td></tr>"
					Else
						ltlBackorder.Text = String.Empty
					End If
				Else
					ltlBackorder.Text = String.Empty
				End If
			Else
				If Not mvAddToCart Is Nothing AndAlso Not mvAddToCart.FindControl("vOutOfStock") Is Nothing Then mvAddToCart.SetActiveView(mvAddToCart.FindControl("vOutOfStock"))
				If Not EmailMe Is Nothing Then
					EmailMe.ItemAttributeId = r("ItemAttributeId")
					EmailMe.Visible = True
				End If
			End If
		End If

		a.AttributeName = r("AttributeName")
		a.AttributeType = r("AttributeType")
		a.AttributeValue = r("AttributeValue")
		a.ImageAlt = IIf(IsDBNull(r("ImageAlt")), Nothing, r("ImageAlt"))
		a.ImageName = IIf(IsDBNull(r("ImageName")), Nothing, r("ImageName"))
		a.ItemAttributeId = r("ItemAttributeId")
		a.ItemId = ItemId
		a.ParentAttributeId = IIf(IsDBNull(r("ParentAttributeId")), Nothing, r("ParentAttributeId"))
		a.Price = r("Price")
		a.ProductAlt = IIf(IsDBNull(r("ProductAlt")), Nothing, r("ProductAlt"))
		a.ProductImage = IIf(IsDBNull(r("ProductImage")), Nothing, r("ProductImage"))
		a.SKU = IIf(IsDBNull(r("SKU")), Nothing, r("SKU"))
		a.SortOrder = r("SortOrder")
		a.TemplateAttributeId = r("TemplateAttributeId")
		a.Weight = r("Weight")
		If Not ProductImage Is Nothing Then
			If Not a.ProductImage = Nothing Then
				ProductImage.Src = "/assets/item/regular/" & a.ProductImage
				If Not a.ProductAlt = Nothing Then ProductImage.Alt = a.ProductAlt
				PreviouslySet = True
			ElseIf Not PreviouslySet Then
				If dbItem.Image = Nothing Then
					ProductImage.Src = "/images/noimage.gif"
					a.ProductAlt = dbItem.ItemName
				Else
					ProductImage.Src = "/assets/item/regular/" & dbItem.Image
					a.ProductAlt = dbItem.ItemName
				End If
			End If
		End If
		If Not ProductImageEnlarge Is Nothing Then
			If Not ProductImage.Src = Nothing Then ProductImageEnlarge.Src = ProductImage.Src.Replace("/regular/", "/large/") Else ProductImageEnlarge.Visible = False
		End If

		'Check for Attribute selection (compares TemplateAttributeId)
		For Each ia As ItemAttribute In Values
			If a.CompareTo(ia) = 0 Then
				Exit Sub
			End If
		Next
		Values.Add(a)
	End Sub
#End Region

#Region "Event Handlers"
	Private Sub drp_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
		Dim hdn As HtmlInputHidden = sender.Parent.FindControl("hdnAttribute")
		Values.Clear()
		hdn.Value = sender.SelectedValue
        BindData()
		ScriptManager.RegisterStartupScript(Me.Page, Me.GetType, "SetFocus", "document.getElementById('" & sender.ClientID & "').focus();", True)
	End Sub

	Private Sub lnk_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
		Select Case e.CommandName
			Case "SwatchClick"
				Dim hdn As HtmlInputHidden = sender.Parent.FindControl("hdnAttribute")
				Values.Clear()
				hdn.Value = sender.CommandArgument
				BindData()
		End Select
	End Sub

	Private Sub rb_CheckChanged(ByVal sender As Object, ByVal e As EventArgs)
		Dim hdn As HtmlInputHidden = sender.Parent.FindControl("hdnAttribute")
		Values.Clear()
		hdn.Value = sender.Attributes("Value")
		BindData()
		ScriptManager.RegisterStartupScript(Me.Page, Me.GetType, "SetFocus", "document.getElementById('" & sender.ClientID & "').focus();", True)
	End Sub

	Private Sub rpt_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rpt.ItemDataBound
		If Not e.Item.ItemType = ListItemType.Item AndAlso Not e.Item.ItemType = ListItemType.AlternatingItem Then
			Exit Sub
		End If

		Dim mvInventory As MultiView = e.Item.FindControl("mvInventory")
		mvInventory.SetActiveView(e.Item.FindControl("vAddToCart"))

		Dim lblPersistedView As Label = e.Item.FindControl("lblPersistedView")
		lblPersistedView.Text = "vAddToCart"

		If Not InventoryControlledAttributeId = Nothing AndAlso EnableInventoryManagement Then
			Dim InventoryQty As Integer = dbItem.InventoryQty
			Dim BackorderDate As DateTime = dbItem.BackorderDate
			Dim InventoryAction As String = dbItem.InventoryAction
			Dim InventoryActionThreshold As Integer = SysParam.GetValue(DB, "InventoryActionThreshold")
			If Not dbItem.InventoryActionThreshold = Nothing Then InventoryActionThreshold = dbItem.InventoryActionThreshold

			If EnableAttributeInventoryManagement Then
				InventoryQty = e.Item.DataItem("InventoryQty")
				If Not IsDBNull(e.Item.DataItem("InventoryAction")) Then InventoryAction = e.Item.DataItem("InventoryAction")
				If Not IsDBNull(e.Item.DataItem("InventoryActionThreshold")) Then InventoryActionThreshold = e.Item.DataItem("InventoryActionThreshold")
				If Not IsDBNull(e.Item.DataItem("BackorderDate")) Then BackorderDate = e.Item.DataItem("BackorderDate") Else BackorderDate = Nothing
			End If

			If InventoryQty <= InventoryActionThreshold Then
				If InventoryAction = "Backorder" Then
					Dim ltlBackorder As Literal = e.Item.FindControl("ltlBackorder")
					ltlBackorder.Text = "<p><strong>Backorder</strong>"
					If Not BackorderDate = Nothing Then
						ltlBackorder.Text &= "<br /><span class=""smaller"" style=""font-weight:normal;"">Estimated Ship Date: " & BackorderDate.ToShortDateString & "</span>"
					End If
					ltlBackorder.Text &= "</p>"
				Else
					mvInventory.SetActiveView(e.Item.FindControl("vInventory"))
					lblPersistedView.Text = "vInventory"
					CType(e.Item.FindControl("ltlInventory"), Literal).Text = "<p class=""bold"">This item is currently out of stock.</p>"
					CType(e.Item.FindControl("ctrlEmailMe"), Object).ItemAttributeId = e.Item.DataItem("ItemAttributeId")
				End If
			End If
		End If
	End Sub
#End Region

#Region "Create Controls"
	Private Sub CreateControls()
		For Each r As DataRow In dtAttribute.Rows
			Dim att As Controls.Attribute = LoadControl("/controls/Attributes/Attribute.ascx")
			Dim drp As DropDownList = att.FindControl("drpAttribute")
			Dim ph As PlaceHolder = att.FindControl("phAttribute")

			att.ID = "att_" & r("TemplateAttributeId")
			phAttributes.Controls.Add(att)

			Select Case r("AttributeType")
				Case "dropdown"

					drp.Attributes.Add("onchange", "this.blur()")
					AddHandler drp.SelectedIndexChanged, AddressOf drp_SelectedIndexChanged

				Case "swatch"

					''Uncomment this line to enable DropDownList AND Swatches
					'AddHandler drp.SelectedIndexChanged, AddressOf drp_SelectedIndexChanged

					''Comment this line to enable DropDownList AND Swatches
					drp.Visible = False

					Dim div As New HtmlGenericControl("div")
					div.ID = "div_" & r("TemplateAttributeId")
					ph.Controls.Add(div)

					Dim divSwatches As New HtmlGenericControl("div")
					ph.Controls.Add(divSwatches)

					'Determine how many swatches we need
					Dim dv As New DataView
					dv.Table = dtTree
					dv.RowFilter = "TemplateAttributeId = " & r("TemplateAttributeId") & " AND ControlCount > 0"
					dv.Sort = "ControlCount DESC"

					If dv.Count > 0 Then
						htControls("ControlCount_" & r("TemplateAttributeId")) = dv(0)("ControlCount")
						For i As Integer = 0 To htControls("ControlCount_" & r("TemplateAttributeId")) - 1
							Dim lnk As New LinkButton
							lnk.ID = "lnk_" & i
							lnk.CommandName = "SwatchClick"
							AddHandler lnk.Command, AddressOf lnk_Command
							divSwatches.Controls.Add(lnk)
						Next
					End If

				Case "radio"

					drp.Visible = False

					Dim div As New HtmlGenericControl("div")
					ph.Controls.Add(div)

					'Determine how many swatches we need
					Dim dv As New DataView
					dv.Table = dtTree
					dv.RowFilter = "TemplateAttributeId = " & r("TemplateAttributeId") & " AND ControlCount > 0"
					dv.Sort = "ControlCount DESC"

					If dv.Count > 0 Then
						htControls("ControlCount_" & r("TemplateAttributeId")) = dv(0)("ControlCount")
						For i As Integer = 0 To htControls("ControlCount_" & r("TemplateAttributeId")) - 1
							Dim rb As New RadioButton
							rb.ID = "rb_" & i
							rb.GroupName = "rbGroup_" & r("TemplateAttributeId")
							rb.AutoPostBack = True
							AddHandler rb.CheckedChanged, AddressOf rb_CheckChanged
							div.Controls.Add(rb)
						Next
					End If

			End Select
		Next
	End Sub

	Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
		'Register the controls for event validation
		For Each ctrl As Control In phAttributes.Controls
			If TypeOf ctrl Is Controls.Attribute Then
				For Each c As Control In ctrl.Controls
					If TypeOf c Is DropDownList OrElse TypeOf c Is LinkButton OrElse TypeOf c Is RadioButton Then Me.Page.ClientScript.RegisterForEventValidation(c.ID, Me.ToString)
				Next
			End If
		Next
		MyBase.Render(writer)
	End Sub
#End Region

	Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
		If ItemId = Nothing Then
			Me.Visible = False
			Exit Sub
		End If

		MinutesToCacheAttributes = SysParam.GetValue(DB, "MinutesToCacheAttributes")
		EnableInventoryManagement = SysParam.GetValue(DB, "EnableInventoryManagement")
		EnableAttributeInventoryManagement = EnableInventoryManagement AndAlso SysParam.GetValue(DB, "EnableAttributeInventoryManagement") = 1

		dbItem = StoreItemRow.GetRow(DB, ItemId)
		If Values Is Nothing Then Values = New ItemAttributeCollection

		If DisplayMode = Nothing Then DisplayMode = "AdminDriven"
		Select Case DisplayMode
			Case "TableLayout"
				If Not IsPostBack Then
					mv.SetActiveView(TableLayout)
					rpt.DataSource = dtAttributeTable
					rpt.DataBind()
				Else
					For Each i As RepeaterItem In rpt.Items
						Dim mvInventory As MultiView = i.FindControl("mvInventory")
						Dim lblPersistedView As Label = i.FindControl("lblPersistedView")
						mvInventory.SetActiveView(i.FindControl(lblPersistedView.Text))
					Next
				End If
			Case Else
				mv.SetActiveView(AdminDriven)
				CreateControls()
				If Not IsPostBack Then
					BindData()
				End If
		End Select
    End Sub
End Class

