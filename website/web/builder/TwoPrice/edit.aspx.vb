Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager
Imports System.Linq
Imports System.Data
Imports System.IO
Imports Controls
Imports System.Data.SqlClient
Imports TwoPrice.DataLayer
Imports System.Collections.Generic
Imports System.Web.Services
Imports Utility
Imports System.Web.UI.WebControls
Partial Class takeoffs_edit
    Inherits SitePage

    Dim dtPhases As DataTable
    Private TotalProducts As Integer
    Private TotalPrice As Double
    Private CountUnpricedProducts As Integer

    Protected ReadOnly Property TwoPriceTakeOffId As Integer
        Get
            Return Request("TwoPriceTakeOffId")
        End Get
    End Property

    Protected ReadOnly Property BuilderId As Integer
        Get
            Return Session("BuilderId")
        End Get
    End Property

    Private m_VendorId As Integer
    Protected ReadOnly Property VendorId As Integer
        Get
            If m_VendorId = Nothing Then
                m_VendorId = DB.ExecuteScalar("SELECT TOP 1 isnull(AwardedVendorId,0) AwardedVendorId FROM TwoPriceCampaign " &
                                            "WHERE TwoPriceCampaignId IN (SELECT TOP 1 TwoPriceCampaignId FROM TwoPriceTakeOff WHERE TwoPriceTakeOffId = " & DB.Number(TwoPriceTakeOffId) & ")")
            End If

            Return m_VendorId
        End Get
    End Property

    Private m_dbVendor As VendorRow
    Protected ReadOnly Property dbVendor As VendorRow
        Get
            If m_dbVendor Is Nothing Then
                m_dbVendor = VendorRow.GetRow(DB, VendorId)
            End If

            Return m_dbVendor
        End Get
    End Property

    Private _dbTwoPriceTakeOff As TwoPriceTakeOffRow
    Protected ReadOnly Property dbTwoPriceTakeOff As TwoPriceTakeOffRow
        Get
            If _dbTwoPriceTakeOff Is Nothing And Request("TwoPriceTakeOffId") IsNot Nothing Then
                _dbTwoPriceTakeOff = TwoPriceTakeOffRow.GetRow(DB, DB.Number(Request("TwoPriceTakeOffId")))
            End If
            Return _dbTwoPriceTakeOff
        End Get
    End Property

    Private _dbTwoPriceCampaign As TwoPriceCampaignRow
    Protected ReadOnly Property dbTwoPriceCampaign As TwoPriceCampaignRow
        Get
            If _dbTwoPriceCampaign Is Nothing Then
                _dbTwoPriceCampaign = TwoPriceCampaignRow.GetRow(DB, dbTwoPriceTakeOff.TwoPriceCampaignId)
            End If
            Return _dbTwoPriceCampaign
        End Get
    End Property

    Private m_TakeoffProductIds As Generic.List(Of String)
    Private ReadOnly Property TakeoffProductIds() As Generic.List(Of String)
        Get
            If m_TakeoffProductIds Is Nothing Then
                m_TakeoffProductIds = New Generic.List(Of String)
            End If
            Return m_TakeoffProductIds
        End Get
    End Property

    Protected m_CurrentOrder As TwoPriceOrderRow = Nothing
    Protected ReadOnly Property CurrentOrder() As TwoPriceOrderRow
        Get
            If m_CurrentOrder Is Nothing Then
                Dim UnprocessedOrderStatusId As Integer = DB.ExecuteScalar("SELECT OrderStatusID FROM OrderStatus WHERE OrderStatus = 'Unsaved'")
                Dim OrderId As Integer = (From dr As DataRow In TwoPriceOrderRow.GetList(DB).Rows
                                          Where dr.Item("VendorId") = VendorId _
                                          And Not IsDBNull(dr.Item("BuilderId")) AndAlso dr.Item("BuilderId") = BuilderId _
                                          And Not IsDBNull(dr.Item("TwoPriceCampaignId")) AndAlso dr.Item("TwoPriceCampaignId") = dbTwoPriceCampaign.TwoPriceCampaignId _
                                          And Not IsDBNull(dr.Item("OrderStatusID")) AndAlso dr.Item("OrderStatusID") = UnprocessedOrderStatusId
                                          Select dr.Item("TwoPriceOrderId")).FirstOrDefault

                m_CurrentOrder = TwoPriceOrderRow.GetRow(DB, OrderId)

                'Init order if it doesn't exist
                If m_CurrentOrder.OrderID = 0 Then
                    m_CurrentOrder.VendorID = VendorId
                    m_CurrentOrder.BuilderID = BuilderId
                    m_CurrentOrder.TwoPriceCampaignId = dbTwoPriceCampaign.TwoPriceCampaignId
                    m_CurrentOrder.OrderStatusID = UnprocessedOrderStatusId
                    m_CurrentOrder.OrderNumber = OrderRow.GetOrderNumber(DB)
                    m_CurrentOrder.TwoPriceTakeoffID = TwoPriceTakeOffId
                    'm_CurrentOrder.ImportedTakeOffID =
                    m_CurrentOrder.OrderID = m_CurrentOrder.Insert()
                End If
            End If
            Return m_CurrentOrder
        End Get
    End Property

    Private ReadOnly Property CurrentOrderProducts() As Dictionary(Of String, String)
        Get
            Return (TwoPriceOrderRow.GetOrderProducts(DB, CurrentOrder.OrderID).AsEnumerable
                    ).ToDictionary(Function(dr) CStr(dr.Item("ProductId")), Function(dr) CStr(dr.Item("TwoPriceOrderProductId")))
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureBuilderAccess()
        'ScriptManager ScriptManager = ScriptManager.GetCurrent(Me.Page);
        'ScriptManager.RegisterPostBackControl(Me.btnStartSubmitOrder);                
        'Check if expired or BuilderId null
        If dbTwoPriceCampaign.StartDate > DateTime.Now.ToLocalTime Or dbTwoPriceCampaign.EndDate < DateTime.Now.ToLocalTime Or BuilderId = Nothing Then
            Response.Redirect("default.aspx")
        End If

        Dim sm As ScriptManager = ScriptManager.GetCurrent(Page)
        sm.RegisterAsyncPostBackControl(hdnPostback)
        'sm.RegisterAsyncPostBackControl(btnStartSubmitOrder)

        If Not IsPostBack Then
            ltlTwoPriceInfo.Text = "<h1>" & dbTwoPriceCampaign.Name & "</h1><br />" &
                                   "<h2><a target=""_blank"" href='/directory/vendor.aspx?vendorid=" & dbTwoPriceCampaign.AwardedVendorId & "'>" & dbVendor.CompanyName & "</a><br /> <span class=""smallest"">Link to Awarded Vendors Details</span></h2>"
            'dbTwoPriceCampaign.StartDate & " - " & dbTwoPriceCampaign.EndDate

            BindProducts()
            RefreshNotPricedProducts()
            BindOrder()
            BindPreviousTakeoffData()
            acProject.WhereClause = "BuilderID=" & DB.Number(Session("BuilderId"))
        End If
    End Sub

    Private Sub BindProducts()
        BindSpecialProducts()
        BindNonSpecialProducts()
    End Sub

    Private Sub BindSpecialProducts()
        Dim dtProducts As DataTable = DB.GetDataTable("SELECT tpv.*, p.SKU, p.Product AS ProductName, vpp.VendorPrice As OldPrice,coalesce(vpp.VendorSKU,'') as Vendorsku " &
                                                        " FROM TwoPriceVendorProductPrice tpv " &
                                                        " JOIN TwoPriceTakeOffProduct tptop ON tptop.ProductID = tpv.ProductID " &
                                                        " LEFT JOIN Product p ON tpv.ProductID = p.ProductID " &
                                                        " LEFT JOIN VendorProductPrice vpp ON vpp.ProductID = p.ProductID AND vpp.VendorId = tpv.VendorID " &
                                                        " WHERE tpv.TwoPriceCampaignID = (SELECT TwoPriceCampaignID FROM TwoPriceTakeOff WHERE TwoPriceTakeOffID = " & TwoPriceTakeOffId & ") " &
                                                        " AND tpv.VendorID = " & VendorId &
                                                        " AND tpv.Submitted = 1" &
                                                        " AND tptop.TwoPriceTakeOffID = " & TwoPriceTakeOffId &
                                                        " ORDER BY tptop.SortOrder")

        rptProducts.DataSource = dtProducts
        rptProducts.DataBind()
    End Sub

    Private Sub BindNotPricedProducts()
        Dim dtGetAllProductsWithPendingPricing As DataTable = TwoPriceBuilderTakeOffProductPendingRow.GetAllProductsWithPendingPricing(DB, CurrentOrder.OrderID)
        gvPendingPricesTakeOff.DataSource = dtGetAllProductsWithPendingPricing
        gvPendingPricesTakeOff.DataBind()
        btnRequestPricingAll.Visible = dtGetAllProductsWithPendingPricing.Rows.Count > 0
        ltlProductsNotIncluded.Visible = dtGetAllProductsWithPendingPricing.Rows.Count > 0

        Dim TotalNonPricedItems As Integer = 0
        Dim TotalPendingPriceRequests As Integer = 0

        For Each dtRow As DataRow In dtGetAllProductsWithPendingPricing.Rows
            Dim currPricerequestState As Integer = CInt(dtRow.Item("PricerequestState"))

            If currPricerequestState <> PriceRequestState.SubstitutionAvailable And currPricerequestState <> PriceRequestState.RequestPending Then
                TotalNonPricedItems = TotalNonPricedItems + 1
            End If
        Next

        If TotalNonPricedItems > 0 Then
            btnRequestPricingAll.Style.Remove("color")
            btnRequestPricingAll.Style.Remove("border-color")
            btnRequestPricingAll.Style.Add("cursor", "hand")
            btnRequestPricingAll.Enabled = True
            hdnRequestPricingStatus.Value = "False"
        Else
            btnRequestPricingAll.Style.Add("color", "lightgrey")
            btnRequestPricingAll.Style.Add("border-color", "lightgrey")
            btnRequestPricingAll.Style.Add("cursor", "none")
            btnRequestPricingAll.Enabled = False
            hdnRequestPricingStatus.Value = "True"
        End If

    End Sub

    Private Sub BindOrder()
        TotalProducts = 0
        TotalPrice = 0
        CountUnpricedProducts = 0
        gvTakeoff.DataSource = TwoPriceOrderRow.GetOrderProducts(DB, CurrentOrder.OrderID)
        gvTakeoff.DataBind()

        BindNotPricedProducts()

        'Check if any product previously unpriced has been priced now.



        ltlTotalProducts.Text = TotalProducts
        ltlTotalPrice.Text = FormatCurrency(TotalPrice)

        'btnStartSubmitOrder.Visible = (TwoPriceBuilderTakeOffProductPendingRow.GetAllProductsWithPendingPricing(DB, CurrentOrder.OrderID).Rows.Count = 0)

        ltlTakeoffTitle.Text = GetTakeOffIdByOrderId(DB, CurrentOrder.OrderID)
    End Sub

    Protected Sub rptNonSpecialProducts_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptNonSpecialProducts.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If
        If Not IsDBNull(e.Item.DataItem.Item("VendorSku")) Then
            CType(e.Item.FindControl("hdnNonSpecialProductsVendorSku"), HiddenField).Value = e.Item.DataItem.Item("VendorSku")
        Else
            CType(e.Item.FindControl("hdnNonSpecialProductsVendorSku"), HiddenField).Value = String.Empty
        End If


    End Sub

    Protected Sub rptProducts_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProducts.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If
        CType(e.Item.FindControl("hdnVendorSku"), HiddenField).Value = e.Item.DataItem.Item("VendorSku")
        CType(e.Item.FindControl("tdPrice"), HtmlTableCell).InnerHtml = FormatCurrency(e.Item.DataItem.Item("Price"))



        Dim ProductID As String = e.Item.DataItem.Item("ProductID")
        Dim gvOldPrice As WebControls.GridView = TryCast(e.Item.FindControl("gvOldPrice"), WebControls.GridView)

        'Dim dtProducts As DataTable = DB.GetDataTable("SELECT tpv.ProductID,p.Product AS ProductName, CONVERT(DECIMAL(10,2),IsNull(vpp.VendorPrice,0)) As OldPrice,CONVERT(DECIMAL(10,2),IsNull(tpv.Price,0)) AS NewPrice " &
        '                                                " FROM TwoPriceVendorProductPrice tpv " &
        '                                                " LEFT JOIN Product p ON tpv.ProductID = p.ProductID " &
        '                                                " LEFT JOIN VendorProductPrice vpp ON vpp.ProductID = p.ProductID AND vpp.VendorId = tpv.VendorID " &
        '                                                " WHERE p.ProductID=" & ProductID & " and tpv.TwoPriceCampaignID = (SELECT TwoPriceCampaignID FROM TwoPriceTakeOff WHERE TwoPriceTakeOffID = " & TwoPriceTakeOffId & ") " &
        '                                                " And tpv.VendorID = " & VendorId &
        '                                                " And tpv.Submitted = 1")



        Dim sql As String = "Select p.Product, CAST(va.OldPrice as DECIMAL(16,2)) as OldPrice,CAST(va.NewPrice as DECIMAL(16,2)) as NewPrice,convert(varchar(max),va.UpdatedOn,101) UpdatedOn from VendorReBidAudit va join Product p " &
                                          "On va.ItemId = p.ProductId where VendorId = " & VendorId & " And p.ProductId=" & ProductID







        Dim dtProducts As DataTable = DB.GetDataTable(sql)


        Dim imgPlus As System.Web.UI.WebControls.Image = TryCast(e.Item.FindControl("imgPlus"), System.Web.UI.WebControls.Image)
        If dtProducts.Rows.Count = 1 Or dtProducts.Rows.Count = 0 Then
            imgPlus.ImageUrl = ""
        Else
            imgPlus.ImageUrl = "../images/plus.png"
        End If



        gvOldPrice.DataSource = dtProducts
        gvOldPrice.DataBind()




    End Sub

    Protected Sub rptProducts_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptProducts.ItemCommand

        Dim qty As TextBox = e.Item.FindControl("txtQty")
        Dim VendorSku As String = CType(e.Item.FindControl("hdnVendorSku"), HiddenField).Value
        If String.IsNullOrEmpty(qty.Text) Then
            Exit Sub
        End If

        If Not IsNumeric(qty.Text) Or qty.Text <= 0 Then
            Exit Sub
        End If

        Dim Price As Double = CDbl(e.CommandArgument)
        Dim ProductId As Integer = CInt(e.CommandName)
        Dim Quantity As Integer = CInt(qty.Text)



        TwoPriceOrderProductRow.AddProduct(DB, CurrentOrder.TwoPriceOrderID, ProductId, Price, Quantity, VendorSku:=VendorSku)

        BindOrder()

        qty.Text = String.Empty
        upTakeoff.Update()
    End Sub
    Protected Sub btnAddAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddAll.Click, btnAddAll2.Click
        For Each item As RepeaterItem In rptProducts.Items

            Dim qty As TextBox = item.FindControl("txtQty")
            Dim btn As Button = item.FindControl("btnAddProduct")
            Dim VendorSku As String = CType(item.FindControl("hdnVendorSku"), HiddenField).Value
            Dim ProductID As Integer = CInt(btn.CommandName)
            Dim Price As Double = CDbl(btn.CommandArgument)

            If IsNumeric(qty.Text) AndAlso CInt(qty.Text) > 0 Then
                Dim Quantity As Integer = CInt(qty.Text)
                TwoPriceOrderProductRow.AddProduct(DB, CurrentOrder.TwoPriceOrderID, ProductID, Price, Quantity, VendorSku:=VendorSku)
            End If
            qty.Text = String.Empty
        Next

        BindOrder()
        upTakeoff.Update()
    End Sub

    'Private Sub AddProduct(ProductId As Integer, Price As Double, Quantity As Integer, Optional ByVal AddToQuantity As Boolean = True, Optional VendorSku As String = Nothing, Optional PriceState As Integer = 0)
    '    Dim TwoPriceOrderProduct As TwoPriceOrderProductRow
    '    'Per ticket 205529 - customer doesnot want merge products - insert the same way as in takeoff

    '    'Product is not part of this order, add it.
    '    TwoPriceOrderProduct = New TwoPriceOrderProductRow(DB)
    '    TwoPriceOrderProduct.VendorPrice = Price
    '    TwoPriceOrderProduct.Quantity = Quantity
    '    TwoPriceOrderProduct.ProductID = ProductId
    '    TwoPriceOrderProduct.TwoPriceOrderID = CurrentOrder.TwoPriceOrderID
    '    TwoPriceOrderProduct.PriceRequestState = PriceState
    '    TwoPriceOrderProduct.VendorSku = VendorSku

    '    TwoPriceOrderProduct.TwoPriceOrderProductID = TwoPriceOrderProduct.Insert()

    'End Sub

    Private Sub AddUnpricedProduct(ProductId As Integer, Price As Double, Quantity As Integer, Optional ByVal AddToQuantity As Boolean = True, Optional ByVal RequestState As Integer = 0)
        Dim TwoPriceBuilderTakeOffProductPending As TwoPriceBuilderTakeOffProductPendingRow
        TwoPriceBuilderTakeOffProductPending = New TwoPriceBuilderTakeOffProductPendingRow(DB)
        TwoPriceBuilderTakeOffProductPending.VendorPrice = Price
        TwoPriceBuilderTakeOffProductPending.Quantity = Quantity
        TwoPriceBuilderTakeOffProductPending.ProductID = ProductId
        TwoPriceBuilderTakeOffProductPending.TwoPriceOrderID = CurrentOrder.TwoPriceOrderID
        TwoPriceBuilderTakeOffProductPending.TwoPriceCampaignID = dbTwoPriceCampaign.TwoPriceCampaignId
        TwoPriceBuilderTakeOffProductPending.VendorID = VendorId
        TwoPriceBuilderTakeOffProductPending.BuilderID = BuilderId
        TwoPriceBuilderTakeOffProductPending.PriceRequestState = PriceRequestState.Init
        TwoPriceBuilderTakeOffProductPending.TwoPriceBuilderTakeOffProductPendingID = TwoPriceBuilderTakeOffProductPending.Insert()

        Dim TwoPriceBuilderTakeoffSubstitutions As DataTable = TwoPriceTakeOffRow.GetTwoPriceBuilderTakeoffSubstitutions(DB, TwoPriceBuilderTakeOffProductPending.TwoPriceBuilderTakeOffProductPendingID, VendorId)

        For Each row In TwoPriceBuilderTakeoffSubstitutions.Rows
            TwoPriceBuilderTakeOffProductPending.PriceRequestState = PriceRequestState.SubstitutionAvailable
            TwoPriceBuilderTakeOffProductPending.SubstituteProductID = row("SubProductID")
            TwoPriceBuilderTakeOffProductPending.VendorPrice = row("VendorPrice")
            If Not IsDBNull(row("VendorSku")) Then
                TwoPriceBuilderTakeOffProductPending.VendorSku = row("VendorSku")
            End If
            TwoPriceBuilderTakeOffProductPending.Update()
            Dim TwoPriceBuilderTakeOffProductSubstitute As TwoPriceBuilderTakeOffProductSubstituterow
            TwoPriceBuilderTakeOffProductSubstitute = New TwoPriceBuilderTakeOffProductSubstituterow(DB)
            TwoPriceBuilderTakeOffProductSubstitute.TwoPriceBuilderTakeOffProductPendingID = TwoPriceBuilderTakeOffProductPending.TwoPriceBuilderTakeOffProductPendingID
            TwoPriceBuilderTakeOffProductSubstitute.VendorID = VendorId
            TwoPriceBuilderTakeOffProductSubstitute.CreatorVendorAccountID = 1
            TwoPriceBuilderTakeOffProductSubstitute.SubstituteProductID = row("SubProductID")
            TwoPriceBuilderTakeOffProductSubstitute.RecommendedQuantity = row("RecommendedQuantity")
            TwoPriceBuilderTakeOffProductSubstitute.Insert()
        Next


    End Sub

    '******************** New function added by Apala (Medullus) on 05.02.2018 for mGuard#T-1086  ***********************
    Private Sub AddUnpricedSpecialProduct(ProductId As Integer, Price As Double, Quantity As Integer, Optional ByVal AddToQuantity As Boolean = True, Optional ByVal RequestState As Integer = 0)

        Dim TwoPriceBuilderTakeOffProductPending As TwoPriceBuilderTakeOffProductPendingRow
        TwoPriceBuilderTakeOffProductPending = New TwoPriceBuilderTakeOffProductPendingRow(DB)
        TwoPriceBuilderTakeOffProductPending.VendorPrice = Price
        TwoPriceBuilderTakeOffProductPending.Quantity = Quantity
        TwoPriceBuilderTakeOffProductPending.SpecialOrderProductID = ProductId
        TwoPriceBuilderTakeOffProductPending.TwoPriceOrderID = CurrentOrder.TwoPriceOrderID
        TwoPriceBuilderTakeOffProductPending.TwoPriceCampaignID = dbTwoPriceCampaign.TwoPriceCampaignId
        TwoPriceBuilderTakeOffProductPending.VendorID = VendorId
        TwoPriceBuilderTakeOffProductPending.BuilderID = BuilderId
        TwoPriceBuilderTakeOffProductPending.PriceRequestState = PriceRequestState.Init
        TwoPriceBuilderTakeOffProductPending.TwoPriceBuilderTakeOffProductPendingID = TwoPriceBuilderTakeOffProductPending.Insert()

    End Sub

    Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        For Each item As GridViewRow In gvTakeoff.Rows
            Dim qty As TextBox = item.FindControl("txtNewQty")
            Dim btnDelete As ImageButton = item.FindControl("btnDelete")
            Dim TwoPriceOrderProductID As Integer = CInt(btnDelete.CommandName)
            Dim Price As Double = CDbl(btnDelete.CommandArgument)

            If IsNumeric(qty.Text) Then
                Dim Quantity As Integer = CInt(qty.Text)
                If Quantity = 0 Then
                    Dim TwoPriceOrderProduct As TwoPriceOrderProductRow = TwoPriceOrderProductRow.GetRow(DB, TwoPriceOrderProductID)
                    TwoPriceOrderProduct.Remove()
                Else
                    'Product is part of   order already, Update the quantity.

                    Dim TwoPriceOrderProduct As TwoPriceOrderProductRow = TwoPriceOrderProductRow.GetRow(DB, TwoPriceOrderProductID)
                    Dim UpdatedQuantity As Integer = qty.Text
                    TwoPriceOrderProduct.Quantity = UpdatedQuantity
                    TwoPriceOrderProduct.Update()
                End If
            End If
        Next

        For Each item As GridViewRow In gvPendingPricesTakeOff.Rows
            Dim qty As TextBox = item.FindControl("txtNewQty")
            Dim btnDelete As ImageButton = item.FindControl("btnDelete")
            Dim TwoPriceBuilderTakeOffProductPendingID As Integer = CInt(btnDelete.CommandName)
            Dim Price As Double = CDbl(btnDelete.CommandArgument)

            If IsNumeric(qty.Text) Then
                Dim Quantity As Integer = CInt(qty.Text)
                If Quantity = 0 Then
                    Dim TwoPriceBuilderTakeOffProductPending As TwoPriceBuilderTakeOffProductPendingRow = TwoPriceBuilderTakeOffProductPendingRow.GetRow(DB, TwoPriceBuilderTakeOffProductPendingID)
                    TwoPriceBuilderTakeOffProductPending.Remove()
                Else
                    'Product is part of   order already, Update the quantity.

                    Dim TwoPriceBuilderTakeOffProductPending As TwoPriceBuilderTakeOffProductPendingRow = TwoPriceBuilderTakeOffProductPendingRow.GetRow(DB, TwoPriceBuilderTakeOffProductPendingID)
                    Dim UpdatedQuantity As Integer = qty.Text
                    TwoPriceBuilderTakeOffProductPending.Quantity = UpdatedQuantity
                    TwoPriceBuilderTakeOffProductPending.Update()
                End If
            End If
        Next



        BindOrder()
        upTakeoff.Update()
    End Sub

    Protected Sub DeleteAll(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeleteAll.Click

        btnRequestPricingAll.Style.Remove("color")
        btnRequestPricingAll.Style.Remove("border-color")
        btnRequestPricingAll.Style.Add("cursor", "hand")
        btnRequestPricingAll.Enabled = True
        hdnRequestPricingStatus.Value = "False"

        For Each row In gvTakeoff.Rows
            Dim btn As ImageButton = row.FindControl("btnDelete")
            Dim TwoPriceOrderProductID As Integer = Core.GetInt(IIf(btn IsNot Nothing, btn.CommandName, 0))
            Dim TwoPriceOrderProduct As TwoPriceOrderProductRow = TwoPriceOrderProductRow.GetRow(DB, TwoPriceOrderProductID)
            TwoPriceOrderProduct.Remove()
        Next

        For Each row In gvPendingPricesTakeOff.Rows
            Dim btn As ImageButton = row.FindControl("btnDelete")
            Dim TwoPriceBuilderTakeOffProductPendingID As Integer = Core.GetInt(IIf(btn IsNot Nothing, btn.CommandName, 0))
            Dim TwoPriceBuilderTakeOffProductPending As TwoPriceBuilderTakeOffProductPendingRow = TwoPriceBuilderTakeOffProductPendingRow.GetRow(DB, TwoPriceBuilderTakeOffProductPendingID)
            TwoPriceBuilderTakeOffProductPendingRow.RemoveRow(DB, TwoPriceBuilderTakeOffProductPendingID)
        Next

        CurrentOrder.ImportedTakeOffID = ""
        CurrentOrder.Update()

        BindOrder()
        upTakeoff.Update()
    End Sub

    Protected Sub rptTakeoff_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvTakeoff.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then
            Exit Sub
        End If

        If Not IsDBNull(e.Row.DataItem("ProductId")) AndAlso Not TakeoffProductIds.Contains(e.Row.DataItem("ProductId")) Then
            TakeoffProductIds.Add(e.Row.DataItem("ProductId"))
        End If

        TotalProducts += e.Row.DataItem("Quantity")

        Dim ltlPrice As Literal = e.Row.FindControl("ltlPrice")
        TotalPrice += e.Row.DataItem("Price") * e.Row.DataItem("Quantity")
        'ltlPrice.Text = "0" ' FormatCurrency(e.Row.DataItem("Price") * e.Row.DataItem("Quantity"))
        ltlPrice.Text = FormatCurrency(e.Row.DataItem("Price") * e.Row.DataItem("Quantity"))

        'Highlight the item if the pricing isn't twoprice pricing
        Dim TwoPricePricing As String = DB.ExecuteScalar("Select TOP 1 Price FROM TwoPriceVendorProductPrice WHERE VendorId = " & VendorId & " And ProductID = " & e.Row.DataItem("ProductID") & " And TwoPriceCampaignID = " & dbTwoPriceCampaign.TwoPriceCampaignId & " And Submitted = 1 ")
        If TwoPricePricing Is Nothing Then
            e.Row.BackColor = Color.Yellow
            CountUnpricedProducts += 1

        End If
        'Hide notice if none of the rows are highlighted
        'If Count > 0 Then
        '    ltlProductsNotIncluded.Visible = True
        'Else
        '    ltlProductsNotIncluded.Visible = False
        'End If
    End Sub

    Protected Sub gvPendingPricesTakeOff_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvPendingPricesTakeOff.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then

            Exit Sub
        End If

        If Not IsDBNull(e.Row.DataItem("ProductId")) AndAlso Not TakeoffProductIds.Contains(e.Row.DataItem("ProductId")) Then
            TakeoffProductIds.Add(e.Row.DataItem("ProductId"))
        End If

        Dim btnRequestPricing As ImageButton = e.Row.FindControl("btnRequestPricing")
        Dim ImgRequestStatus As Image = e.Row.FindControl("ImgRequestStatus")
        ImgRequestStatus.Visible = False

        Dim ProductType As String = e.Row.DataItem("ProductType")           '********* mGuard#T-1086

        Dim ltlPrice As Literal = e.Row.FindControl("ltlPrice")
        If e.Row.DataItem("PricerequestState") = PriceRequestState.SubstitutionAvailable Then
            Dim TworiceBuilderTakeoffProductSubs As TwoPriceBuilderTakeOffProductSubstituterow = TwoPriceBuilderTakeOffProductSubstituterow.GetRow(DB, e.Row.DataItem("VendorID"), e.Row.DataItem("TwoPriceBuilderTakeOffProductPendingID"))
            If TworiceBuilderTakeoffProductSubs.SubstituteProductID > 0 Then
                Dim OpenSubForm As String = "OpenSubForm(" & TworiceBuilderTakeoffProductSubs.TwoPriceBuilderTakeOffProductPendingID & "," & TworiceBuilderTakeoffProductSubs.VendorID & "," & Core.Escape(Core.GetString(ProductRow.GetRow(DB, TworiceBuilderTakeoffProductSubs.SubstituteProductID).Product)) & "," & Core.Escape(Core.GetString(e.Row.DataItem("VendorSku"))) & ",'" & FormatCurrency(Core.GetDouble(e.Row.DataItem("VendorPrice"))) & "'," & TworiceBuilderTakeoffProductSubs.RecommendedQuantity & ");"
                btnRequestPricing.Attributes.Add("OnClick", OpenSubForm)
                ltlPrice.Text = FormatCurrency(e.Row.DataItem("VendorPrice") * e.Row.DataItem("Quantity"))
                e.Row.Attributes.Add("style", "background-color:#FFCCCC")
                ImgRequestStatus.Visible = True
                'e.Row.BackColor = Color.Blue
                ImgRequestStatus.ImageUrl = "/images/global/twoprice/icon-substitution.png"
            End If
        ElseIf e.Row.DataItem("PricerequestState") = PriceRequestState.RequestPending Then
            '********* mGuard#T-1086
            PriceReqStatus.Value = 1

            Dim OpenSpecialForm As String = "OpenSpecialForm('" & ProductType & "', " & e.Row.DataItem("TwoPriceBuilderTakeOffProductPendingID") & "," & e.Row.DataItem("VendorID") & "," & Core.Escape(Core.GetString(e.Row.DataItem("Product"))) & ",'" & FormatCurrency(e.Row.DataItem("VendorPrice")) & "');"
            '*******************************
            btnRequestPricing.Attributes.Add("OnClick", OpenSpecialForm)
            ltlPrice.Text = FormatCurrency(e.Row.DataItem("VendorPrice") * e.Row.DataItem("Quantity"))
            e.Row.Attributes.Add("style", "background-color:#26e7f1")
            ImgRequestStatus.Visible = True
            ImgRequestStatus.ImageUrl = "/images/global/twoprice/icon-pending.png"
        Else
            '********* mGuard#T-1086
            Dim OpenSpecialForm As String = "OpenSpecialForm('" & ProductType & "', " & e.Row.DataItem("TwoPriceBuilderTakeOffProductPendingID") & "," & e.Row.DataItem("VendorID") & "," & Core.Escape(Core.GetString(e.Row.DataItem("Product"))) & ",'" & FormatCurrency(e.Row.DataItem("VendorPrice")) & "');"
            '***********************
            PriceReqStatus.Value = 2
            btnRequestPricing.Attributes.Add("OnClick", OpenSpecialForm)
            ltlPrice.Text = FormatCurrency(e.Row.DataItem("VendorPrice") * e.Row.DataItem("Quantity"))
            e.Row.Attributes.Add("style", "background-color:#BCC79A")
            ImgRequestStatus.Visible = True
            ImgRequestStatus.ImageUrl = "/images/global/twoprice/icon-nopricing.png"
        End If

    End Sub

    Private Function UpdatePricingAndAddtoOrder(ByVal db As Database, ByVal dbTwoPriceBuilderTakeOffProductPendingrow As TwoPriceBuilderTakeOffProductPendingRow, Optional ByVal ProductType As String = "Regular") As Boolean
        Try
            If ProductType = "Special" Then
                TwoPriceOrderProductRow.AddSpecialOrderProduct(db, CurrentOrder.TwoPriceOrderID, dbTwoPriceBuilderTakeOffProductPendingrow.SpecialOrderProductID, dbTwoPriceBuilderTakeOffProductPendingrow.VendorPrice, dbTwoPriceBuilderTakeOffProductPendingrow.Quantity)
            Else
                TwoPriceOrderProductRow.AddProduct(db, CurrentOrder.TwoPriceOrderID, dbTwoPriceBuilderTakeOffProductPendingrow.ProductID, dbTwoPriceBuilderTakeOffProductPendingrow.VendorPrice, dbTwoPriceBuilderTakeOffProductPendingrow.Quantity)
            End If

            Return True
        Catch ex As Exception
            Logger.Error("TWOPRICE PRICING REQUEST ERROR" & ex.ToString)
            Return False
        End Try
        Return True
    End Function

    Protected Sub frmSubstitute_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAccept.Click, btnReject.Click, frmSubstitute.Postback
        If Not IsNumeric(txtSubQuantity.Text) Then
            AddError("Invalid Quantity")
            Exit Sub
        End If

        Dim btn As Button = sender
        Dim TwoPriceBuilderTakeOffProductPendingID As Integer = hdnSubTakeoffProductID.Value
        Dim VendorID As Integer = hdnSubVendorID.Value
        Dim dbTwoPriceBuildertakeOff As TwoPriceBuilderTakeOffProductPendingRow = TwoPriceBuilderTakeOffProductPendingRow.GetRow(DB, TwoPriceBuilderTakeOffProductPendingID)
        ' Dim dbTakeoffProduct As TakeOffProductRow = TakeOffProductRow.GetRow(DB, TakeoffProductID)

        Select Case btn.CommandName
            Case "Accept"
                Dim dbTwoPriceSubstitute As TwoPriceBuilderTakeOffProductSubstituterow = TwoPriceBuilderTakeOffProductSubstituterow.GetRow(DB, VendorID, TwoPriceBuilderTakeOffProductPendingID)

                dbTwoPriceBuildertakeOff.Quantity = txtSubQuantity.Text
                dbTwoPriceBuildertakeOff.PriceRequestState = PriceRequestState.SubstituteAccepted
                TwoPriceOrderProductRow.AddProduct(DB, CurrentOrder.TwoPriceOrderID, dbTwoPriceSubstitute.SubstituteProductID, dbTwoPriceBuildertakeOff.VendorPrice, dbTwoPriceBuildertakeOff.Quantity, False, dbTwoPriceBuildertakeOff.VendorSku, PriceRequestState.SubstituteAccepted)
                TwoPriceBuilderTakeOffProductPendingRow.RemoveRow(DB, TwoPriceBuilderTakeOffProductPendingID)
            Case "Reject"
                TwoPriceBuilderTakeOffProductPendingRow.RemoveRow(DB, TwoPriceBuilderTakeOffProductPendingID)
        End Select

        dbTwoPriceBuildertakeOff.Update()
        BindOrder()
    End Sub

    Protected Sub frmAverage_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAcceptAverage.Click, btnOmitAverage.Click, btnRequestAverage.Click, frmAverage.Postback
        Dim btn As Button = sender
        
        If hdnAvgTakeoffProductID.Value = "" Then Exit Sub

	Dim TwoPriceBuilderTakeOffProductPendingID As Integer = hdnAvgTakeoffProductID.Value
        Dim vendorID As Integer = hdnAvgVendorID.Value
        Dim dbTwoPriceBuilderTakeOffProductPending As TwoPriceBuilderTakeOffProductPendingRow = TwoPriceBuilderTakeOffProductPendingRow.GetRow(DB, TwoPriceBuilderTakeOffProductPendingID)

        '   Dim dbTakeoffProduct As TakeOffProductRow = TakeOffProductRow.GetRow(DB, takeoffProductID)

        Select Case btn.CommandName
            Case "Accept"
                Dim ProductType As String = hdnProductType.Value

                'BP TODO : Check If builder has priced or not  
                dbTwoPriceBuilderTakeOffProductPending.VendorPrice = 0.0
                dbTwoPriceBuilderTakeOffProductPending.PriceRequestState = PriceRequestState.UnknownPriceAccepted
                If UpdatePricingAndAddtoOrder(DB, dbTwoPriceBuilderTakeOffProductPending, ProductType) Then
                    dbTwoPriceBuilderTakeOffProductPending.Remove()
                End If

            Case "Omit"
                dbTwoPriceBuilderTakeOffProductPending.Remove()
            Case "Request"
                dbTwoPriceBuilderTakeOffProductPending.PriceRequestState = PriceRequestState.RequestPending


                Dim dbRequest As TwoPriceVendorProductPriceRequestRow = TwoPriceVendorProductPriceRequestRow.GetRow(DB, Session("BuilderId"), vendorID, TwoPriceBuilderTakeOffProductPendingID)
                Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
                Dim dbVendor As VendorRow = VendorRow.GetRow(DB, vendorID)
                If dbRequest.Created = Nothing Then
                    dbRequest.BuilderID = Session("BuilderId")
                    dbRequest.CreatorBuilderAccountID = Session("BuilderAccountId")
                    dbRequest.ProductID = dbTwoPriceBuilderTakeOffProductPending.ProductID
                    dbRequest.SpecialOrderProductID = dbTwoPriceBuilderTakeOffProductPending.SpecialOrderProductID
                    dbRequest.TwoPriceBuilderTakeOffProductPendingID = TwoPriceBuilderTakeOffProductPendingID
                    dbRequest.VendorID = vendorID
                    dbRequest.TwoPriceCampaignID = dbTwoPriceCampaign.TwoPriceCampaignId
                    dbRequest.TwoPriceOrderID = CurrentOrder.OrderID
                    dbRequest.Insert()
                End If

                Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "MissingPrice")
                Dim msg As New StringBuilder

                msg.Append(dbBuilder.CompanyName & " is requesting pricing for the following item:" & vbCrLf & vbCrLf)
                If dbTwoPriceBuilderTakeOffProductPending.ProductID <> Nothing Then
                    Dim dbProduct As ProductRow = ProductRow.GetRow(DB, dbTwoPriceBuilderTakeOffProductPending.ProductID)
                    If dbProduct.SKU = String.Empty Then
                        msg.AppendLine(dbProduct.Product)
                    Else
                        msg.AppendLine("CBUSA Sku # " & dbProduct.SKU & " - " & dbProduct.Product)
                    End If

                ElseIf dbTwoPriceBuilderTakeOffProductPending.SpecialOrderProductID <> Nothing Then
                    Dim dbProduct As SpecialOrderProductRow = SpecialOrderProductRow.GetRow(DB, dbTwoPriceBuilderTakeOffProductPending.SpecialOrderProductID)
                    msg.AppendLine(dbProduct.SpecialOrderProduct)
                End If

                dbMsg.Send(dbVendor, msg.ToString)

                dbTwoPriceBuilderTakeOffProductPending.Update()
        End Select




        BindOrder()


    End Sub

    'Changed by Susobhan for Start Order Validation.
    Protected Sub PopUpFormOrderPriceReq_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnOrderReqAccept.Click, BtnOrderReqOmit.Click, PopUpFormOrderPriceReq.Postback
        Dim btn As Button = sender
        'Dim TwoPriceBuilderTakeOffProductPendingID As Integer = hdnAvgTakeoffProductID.Value
        Dim vendorID As Integer = hdnReq2VendorID.Value
        Dim dtTwoPriceBuilderTakeOffProductPending As DataTable = TwoPriceBuilderTakeOffProductPendingRow.GetAllProductsWithPendingPricing(DB, CurrentOrder.OrderID)

        '   Dim dbTakeoffProduct As TakeOffProductRow = TakeOffProductRow.GetRow(DB, takeoffProductID)
        For Each temp As DataRow In dtTwoPriceBuilderTakeOffProductPending.Rows
            Dim dbTwoPriceBuilderTakeOffProductPending As TwoPriceBuilderTakeOffProductPendingRow = TwoPriceBuilderTakeOffProductPendingRow.GetRow(DB, temp("TwoPriceBuilderTakeOffProductPendingID"))

            Select Case btn.CommandName
                Case "Accept"


                    Dim ProductType As String = hdnProductType.Value

                    'BP TODO : Check If builder has priced or not  
                    dbTwoPriceBuilderTakeOffProductPending.VendorPrice = 0.0
                    dbTwoPriceBuilderTakeOffProductPending.PriceRequestState = PriceRequestState.UnknownPriceAccepted
                    If UpdatePricingAndAddtoOrder(DB, dbTwoPriceBuilderTakeOffProductPending, ProductType) Then
                        dbTwoPriceBuilderTakeOffProductPending.Remove()
                    End If
                    PriceReqStatus.Value = 0
                Case "Omit"
                    dbTwoPriceBuilderTakeOffProductPending.Remove()
                    PriceReqStatus.Value = 0
                Case "Request"
                    'dbTwoPriceBuilderTakeOffProductPending.PriceRequestState = PriceRequestState.RequestPending
                    RequestPricing(DB, CurrentOrder.OrderID)


                    Dim dbRequest As TwoPriceVendorProductPriceRequestRow = TwoPriceVendorProductPriceRequestRow.GetRow(DB, Session("BuilderId"), vendorID, temp("TwoPriceBuilderTakeOffProductPendingID"))
                    Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
                    Dim dbVendor As VendorRow = VendorRow.GetRow(DB, vendorID)
                    If dbRequest.Created = Nothing Then
                        dbRequest.BuilderID = Session("BuilderId")
                        dbRequest.CreatorBuilderAccountID = Session("BuilderAccountId")
                        dbRequest.ProductID = dbTwoPriceBuilderTakeOffProductPending.ProductID
                        dbRequest.SpecialOrderProductID = dbTwoPriceBuilderTakeOffProductPending.SpecialOrderProductID
                        dbRequest.TwoPriceBuilderTakeOffProductPendingID = temp("TwoPriceBuilderTakeOffProductPendingID")
                        dbRequest.VendorID = vendorID
                        dbRequest.TwoPriceCampaignID = dbTwoPriceCampaign.TwoPriceCampaignId
                        dbRequest.TwoPriceOrderID = CurrentOrder.OrderID
                        dbRequest.Insert()

                        Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "MissingPrice")
                        Dim msg As New StringBuilder

                        msg.Append(dbBuilder.CompanyName & " is requesting pricing for the following item:" & vbCrLf & vbCrLf)
                        If dbTwoPriceBuilderTakeOffProductPending.ProductID <> Nothing Then
                            Dim dbProduct As ProductRow = ProductRow.GetRow(DB, dbTwoPriceBuilderTakeOffProductPending.ProductID)
                            If dbProduct.SKU = String.Empty Then
                                msg.AppendLine(dbProduct.Product)
                            Else
                                msg.AppendLine("CBUSA Sku # " & dbProduct.SKU & " - " & dbProduct.Product)
                            End If

                        ElseIf dbTwoPriceBuilderTakeOffProductPending.SpecialOrderProductID <> Nothing Then
                            Dim dbProduct As SpecialOrderProductRow = SpecialOrderProductRow.GetRow(DB, dbTwoPriceBuilderTakeOffProductPending.SpecialOrderProductID)
                            msg.AppendLine(dbProduct.SpecialOrderProduct)
                        End If

                        dbMsg.Send(dbVendor, msg.ToString)
                    End If
                    dbTwoPriceBuilderTakeOffProductPending.Update()

            End Select
        Next



        BindOrder()


    End Sub

    Protected Sub PopupFormOrderSubmit_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnPriceReqAccept.Click, BtnPriceReqOmit.Click, BtnPriceReqRequest.Click, PopupFormOrderSubmit.Postback
        Dim btn As Button = sender
        'Dim TwoPriceBuilderTakeOffProductPendingID As Integer = hdnAvgTakeoffProductID.Value
        Dim vendorID As Integer = hdnReq1VendorID.Value
        'Dim dbTwoPriceBuilderTakeOffProductPending As TwoPriceBuilderTakeOffProductPendingRow = TwoPriceBuilderTakeOffProductPendingRow.GetRow(DB, TwoPriceBuilderTakeOffProductPendingID)

        Dim dtTwoPriceBuilderTakeOffProductPending As DataTable = TwoPriceBuilderTakeOffProductPendingRow.GetAllProductsWithPendingPricing(DB, CurrentOrder.OrderID)

        '   Dim dbTakeoffProduct As TakeOffProductRow = TakeOffProductRow.GetRow(DB, takeoffProductID)
        For Each temp As DataRow In dtTwoPriceBuilderTakeOffProductPending.Rows
            Dim dbTwoPriceBuilderTakeOffProductPending As TwoPriceBuilderTakeOffProductPendingRow = TwoPriceBuilderTakeOffProductPendingRow.GetRow(DB, temp("TwoPriceBuilderTakeOffProductPendingID"))
            '   Dim dbTakeoffProduct As TakeOffProductRow = TakeOffProductRow.GetRow(DB, takeoffProductID)

            Select Case btn.CommandName
                Case "Accept"
                    Dim ProductType As String = hdnProductType.Value

                    'BP TODO : Check If builder has priced or not  
                    dbTwoPriceBuilderTakeOffProductPending.VendorPrice = 0.0
                    dbTwoPriceBuilderTakeOffProductPending.PriceRequestState = PriceRequestState.UnknownPriceAccepted
                    If UpdatePricingAndAddtoOrder(DB, dbTwoPriceBuilderTakeOffProductPending, ProductType) Then
                        dbTwoPriceBuilderTakeOffProductPending.Remove()
                    End If
                    PriceReqStatus.Value = 0
                Case "Omit"
                    dbTwoPriceBuilderTakeOffProductPending.Remove()
                    PriceReqStatus.Value = 0
                Case "Request"
                    dbTwoPriceBuilderTakeOffProductPending.PriceRequestState = PriceRequestState.RequestPending
                    RequestPricing(DB, CurrentOrder.OrderID)


                    Dim dbRequest As TwoPriceVendorProductPriceRequestRow = TwoPriceVendorProductPriceRequestRow.GetRow(DB, Session("BuilderId"), vendorID, temp("TwoPriceBuilderTakeOffProductPendingID"))
                    Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
                    Dim dbVendor As VendorRow = VendorRow.GetRow(DB, vendorID)
                    If dbRequest.Created = Nothing Then
                        dbRequest.BuilderID = Session("BuilderId")
                        dbRequest.CreatorBuilderAccountID = Session("BuilderAccountId")
                        dbRequest.ProductID = dbTwoPriceBuilderTakeOffProductPending.ProductID
                        dbRequest.SpecialOrderProductID = dbTwoPriceBuilderTakeOffProductPending.SpecialOrderProductID
                        dbRequest.TwoPriceBuilderTakeOffProductPendingID = temp("TwoPriceBuilderTakeOffProductPendingID")
                        dbRequest.VendorID = vendorID
                        dbRequest.TwoPriceCampaignID = dbTwoPriceCampaign.TwoPriceCampaignId
                        dbRequest.TwoPriceOrderID = CurrentOrder.OrderID
                        dbRequest.Insert()

                        Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "MissingPrice")
                        Dim msg As New StringBuilder

                        msg.Append(dbBuilder.CompanyName & " is requesting pricing for the following item:" & vbCrLf & vbCrLf)
                        If dbTwoPriceBuilderTakeOffProductPending.ProductID <> Nothing Then
                            Dim dbProduct As ProductRow = ProductRow.GetRow(DB, dbTwoPriceBuilderTakeOffProductPending.ProductID)
                            If dbProduct.SKU = String.Empty Then
                                msg.AppendLine(dbProduct.Product)
                            Else
                                msg.AppendLine("CBUSA Sku # " & dbProduct.SKU & " - " & dbProduct.Product)
                            End If

                        ElseIf dbTwoPriceBuilderTakeOffProductPending.SpecialOrderProductID <> Nothing Then
                            Dim dbProduct As SpecialOrderProductRow = SpecialOrderProductRow.GetRow(DB, dbTwoPriceBuilderTakeOffProductPending.SpecialOrderProductID)
                            msg.AppendLine(dbProduct.SpecialOrderProduct)
                        End If

                        dbMsg.Send(dbVendor, msg.ToString)
                    End If
                    dbTwoPriceBuilderTakeOffProductPending.Update()
            End Select
        Next



        BindOrder()


    End Sub

    Protected Sub rptTakeoff_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvTakeoff.RowCommand
        Dim TwoPriceOrderProductID As Integer = CInt(e.CommandName)
        Dim TwoPriceOrderProduct As TwoPriceOrderProductRow = TwoPriceOrderProductRow.GetRow(DB, TwoPriceOrderProductID)
        TwoPriceOrderProduct.Remove()

        BindOrder()
        upTakeoff.Update()
    End Sub

    Protected Sub gvPendingPricesTakeOff_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvPendingPricesTakeOff.RowCommand
        If e.CommandName <> String.Empty Then
            Dim TwoPriceBuilderTakeOffProductPendingID As Integer = CInt(e.CommandName)
            Dim TwoPriceBuilderTakeOffProductPending As TwoPriceBuilderTakeOffProductPendingRow = TwoPriceBuilderTakeOffProductPendingRow.GetRow(DB, TwoPriceBuilderTakeOffProductPendingID)
            TwoPriceBuilderTakeOffProductPendingRow.RemoveRow(DB, TwoPriceBuilderTakeOffProductPendingID)
            BindOrder()
            upTakeoff.Update()
        End If

    End Sub

    Protected Sub btnStartSubmitOrder_Click(sender As Object, e As EventArgs) Handles btnStartSubmitOrder.Click

        If (PriceReqStatus.Value = 1) Then
            'ClientScript.RegisterStartupScript(Page.GetType, "Script", "OpenPriceReqFormAfterReq();", True)
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "OpenPriceReqFormAfterReq('" & CurrentOrder.VendorID & "');", True)
        ElseIf (PriceReqStatus.Value = 2) Then
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "OpenPriceReqFormBeforeReq('" & CurrentOrder.VendorID & "');", True)
        Else
            'ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "OpenPriceReqFormAfterReq();", True)

            Dim dbTwoPriceTakeOff As TwoPriceTakeOffRow = TwoPriceTakeOffRow.GetRow(DB, TwoPriceTakeOffId)
        ' Response.Write(PriceReqStatus.Value)


        If Not Session("BuilderAccountId") Is Nothing Then
                Dim BuilderAccount As BuilderAccountRow = BuilderAccountRow.GetRow(DB, Session("BuilderAccountId"))
                txtOrdererEmail.Text = BuilderAccount.Email
                txtOrdererFirstName.Text = BuilderAccount.FirstName
                txtOrdererLastName.Text = BuilderAccount.LastName
                txtOrdererPhone.Text = BuilderAccount.Phone
                txtOrderTitle.Text = dbTwoPriceTakeOff.Title
                pnBuildOrder.Visible = False
                pnSubmitOrder.Visible = True
                tblLegend.Visible = False
                'drpProjects.DataSource = ProjectRow.GetBuilderProjects(DB, BuilderId)
                'drpProjects.DataTextField = "ProjectName"
                'drpProjects.DataValueField = "ProjectId"
                'drpProjects.DataBind()
            End If
        End If
    End Sub

    Protected Sub btnSubmitOrder_Click(sender As Object, e As EventArgs) Handles btnSubmitOrder.Click
        Try
            DB.BeginTransaction()

            'Save the current order

            Dim dbProject As ProjectRow = ProjectRow.GetRow(DB, acProject.Value)

            CurrentOrder.ProjectID = acProject.Value


            CurrentOrder.VendorID = VendorId
            CurrentOrder.BuilderID = BuilderId
            CurrentOrder.CreatorBuilderID = Session("BuilderAccountId")
            CurrentOrder.DeliveryInstructions = txtDeliveryInstructions.Text
            CurrentOrder.Notes = txtNotes.Text
            CurrentOrder.OrdererEmail = txtOrdererEmail.Text
            CurrentOrder.OrdererFirstName = txtOrdererFirstName.Text
            CurrentOrder.OrdererLastName = txtOrdererLastName.Text
            CurrentOrder.OrdererPhone = txtOrdererPhone.Text
            CurrentOrder.OrderNumber = OrderRow.GetOrderNumber(DB)
            CurrentOrder.OrderStatusID = OrderStatusRow.GetDefaultStatusId(DB)
            CurrentOrder.PONumber = txtPONumber.Text
            CurrentOrder.RequestedDelivery = dpRequestedDelivery.Value

            CurrentOrder.RemoteIP = HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")
            CurrentOrder.SuperEmail = txtSuperEmail.Text
            CurrentOrder.SuperFirstName = txtSuperFirstName.Text
            CurrentOrder.SuperLastName = txtSuperLastName.Text
            CurrentOrder.SuperPhone = txtSuperPhone.Text
            CurrentOrder.Title = txtOrderTitle.Text

            CurrentOrder.TaxRate = txtTaxRate.Text

            Dim subTotal As Double = (From s In TwoPriceOrderRow.GetOrderProducts(DB, CurrentOrder.OrderID).Rows Select CDbl(s.Item("VendorPrice") * s.Item("Quantity"))).Sum()
            Dim tax As Double = subTotal * txtTaxRate.Text / 100

            CurrentOrder.Subtotal = subTotal
            CurrentOrder.Tax = tax
            CurrentOrder.Total = subTotal + tax

            CurrentOrder.Update()

            Dim aDrops As DataRow() = sfDrops.GetData()
            For Each row As DataRow In aDrops
                If Not sfDrops.InvalidRows.Contains(row("RowIndex")) Then
                    Dim dbDrop As New TwoPriceOrderDropRow(DB)
                    dbDrop.OrderID = CurrentOrder.OrderID
                    dbDrop.CreatorBuilderID = Session("BuilderAccountID")
                    dbDrop.DropName = row("txtDropName")
                    If row("dpRequestedDelivery") <> Nothing Then dbDrop.RequestedDelivery = row("dpRequestedDelivery")
                    If dbDrop.DropName <> Nothing AndAlso dbDrop.RequestedDelivery <> Nothing Then
                        dbDrop.Insert()
                    End If
                End If
            Next

            DB.CommitTransaction()

            If rbDropsYes.Checked Then
                Response.Redirect("/order/drops.aspx?OrderID=" & CurrentOrder.OrderID & "&twoprice=y")
            Else
                Response.Redirect("/order/summary.aspx?OrderID=" & CurrentOrder.OrderID & "&twoprice=y")
            End If
        Catch ex As SqlClient.SqlException
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            Logger.Error(Logger.GetErrorMessage(ex))
            AddError(ErrHandler.ErrorText(ex))
        End Try

    End Sub

    Protected Sub sfDrops_ValidateRow(ByVal sender As Object, ByVal args As Controls.SubForm.SubFormEventArguments) Handles sfDrops.ValidateRow
        args.IsValid = True
        If args.DataRow("txtDropName") = String.Empty Then
            args.IsValid = False
        End If
        If args.DataRow("dpRequestedDelivery") = String.Empty Then
            AddError("Delivery Field is mandatory.")
            args.IsValid = False
        End If
    End Sub

    Protected Sub frmSubstitute_Callback(ByVal sender As Object, ByVal args As PopupForm.PopupFormEventArgs) Handles frmSubstitute.Callback
        Dim json As New Web.Script.Serialization.JavaScriptSerializer
        Dim ret As String = json.Serialize(args.Data)
        frmSubstitute.CallbackResult = ret
    End Sub

#Region "AddProjectForm"

    Protected Sub frmProject_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmProject.TemplateLoaded
        BindProjectForm()
    End Sub

    Private Sub BindProjectForm()
        drpProjectState.DataSource = StateRow.GetStateList(DB)
        drpProjectState.DataTextField = "StateName"
        drpProjectState.DataValueField = "StateCode"
        drpProjectState.DataBind()

        drpProjectPortfolio.DataSource = PortfolioRow.GetList(DB, "Portfolio")
        drpProjectPortfolio.DataTextField = "Portfolio"
        drpProjectPortfolio.DataValueField = "PortfolioID"
        drpProjectPortfolio.DataBind()
        drpProjectPortfolio.Items.Insert(0, New ListItem("", ""))

        drpProjectStatus.DataSource = ProjectStatusRow.GetList(DB, "SortOrder")
        drpProjectStatus.DataTextField = "ProjectStatus"
        drpProjectStatus.DataValueField = "ProjectStatusID"
        drpProjectStatus.DataBind()
    End Sub

    <Web.Services.WebMethod(EnableSession:=True)>
    Public Shared Function GetTaxRate(ByVal Zip As String) As Double
        Return Math.Round(100 * GetTaxRate(Utility.GlobalDB.DB, Zip), 4)
    End Function

    <WebMethod()>
    Public Shared Function GetTaxRateFromProjectZip(ByVal ProjectID As String) As Double
        If ProjectID <> String.Empty Then
            Dim Project As ProjectRow = ProjectRow.GetRow(GlobalDB.DB, ProjectID)

            Return Math.Round(100 * GetTaxRate(Utility.GlobalDB.DB, Project.Zip), 4)
        Else
            Return String.Empty

        End If
    End Function
    Public Shared Function GetTaxRate(ByVal DB As Database, ByVal Zip As String) As Double
        Dim f As New FastTax.DOTSFastTax()
        Dim info As FastTax.TaxInfo = f.GetTaxInfo(Zip, "sales", SysParam.GetValue(DB, "FastTaxLicenseKey"))
        Return info.TotalTaxRate
    End Function

    Private Function GetTaxRate(ByVal City As String, ByVal County As String, ByVal State As String) As Double
        Dim f As New FastTax.DOTSFastTax()
        Dim info As FastTax.TaxInfo = f.GetTaxInfoByCityCountyState(City, County, State, "total", SysParam.GetValue(DB, "FastTaxLicenseKey"))
        Return info.TotalTaxRate
    End Function

    Protected Sub frmProject_Callback(ByVal sender As Object, ByVal args As PopupForm.PopupFormEventArgs) Handles frmProject.Callback
        frmProject.Validate()
        If Not frmProject.IsValid Then
            Exit Sub
        End If

        Dim dbProject As New ProjectRow(DB)
        dbProject.Address = txtProjectAddress1.Text
        dbProject.Address2 = txtProjectAddress2.Text
        dbProject.BuilderID = Session("BuilderId")
        dbProject.City = txtProjectCity.Text
        dbProject.IsArchived = rblProjectArchive.SelectedValue
        dbProject.LotNumber = txtProjectLotNo.Text
        If drpProjectPortfolio.SelectedValue <> Nothing Then
            dbProject.PortfolioID = drpProjectPortfolio.SelectedValue
        End If
        dbProject.ProjectName = txtProjectName.Text
        dbProject.ProjectStatusID = drpProjectStatus.SelectedValue
        'dbProject.StartDate = dpProjectStartDate.Value
        dbProject.State = drpProjectState.SelectedValue
        dbProject.Subdivision = txtProjectSubdivision.Text
        dbProject.Zip = txtProjectZip.Text
        dbProject.Insert()

        acProject.WhereClause = "BuilderID=" & DB.Number(Session("BuilderId"))

    End Sub

#End Region

#Region "Non-special pricing"
    Private Sub BindNonSpecialProducts(Optional ByVal SupplyPhaseIds As String = "")
        Dim dtSpecialProducts As DataTable = CType(rptProducts.DataSource, DataTable)
        Dim SpecialProductIds As String = ConcatProductIds((From dx As DataRow In dtSpecialProducts.Rows Select CStr(dx.Item("ProductId"))).ToList)
        Dim SQL As String = "SELECT vpp.*, p.Product As ProductName FROM VendorProductPrice vpp " &
                                                        " JOIN Product p ON vpp.ProductID = p.ProductID " &
                                                        " LEFT JOIN SupplyPhaseProduct spp ON spp.ProductID = p.ProductID " &
                                                        " WHERE vpp.VendorID = " & VendorId &
                                                        " AND vpp.VendorPrice IS NOT NULL"
        If SpecialProductIds <> "" Then SQL &= " AND vpp.ProductId NOT IN " &
                                                        "(" & SpecialProductIds & ")"

        Dim dtNonSpecialProducts As DataTable = DB.GetDataTable(SQL)

        Session("CurrentPreferredVendor") = VendorId
        ctlNavigator.Visible = False

        If dtNonSpecialProducts.Rows.Count = 0 Then
            btnAddNonSpecial.Enabled = False
            Exit Sub
        Else
            btnAddNonSpecial.Enabled = True
        End If

        Dim NonSpecialProductIds As String = ConcatProductIds((From dx As DataRow In dtNonSpecialProducts.Rows Select CStr(dx.Item("ProductId"))).ToList)

        ctlSearch.FilterList = NonSpecialProductIds.Split(",").ToList
        ctlSearch.SearchProduct()

        Dim SearchedProductIds As String = ConcatProductIds((From dx As DataRow In ctlSearch.SearchResults.Rows Select CStr(dx.Item("ProductId"))).ToList)
        If SearchedProductIds <> String.Empty Then
            rptNonSpecialProducts.DataSource = dtNonSpecialProducts.Select("ProductId IN (" & SearchedProductIds & ")").CopyToDataTable
            rptNonSpecialProducts.DataBind()
        End If

        upResults.Update()

    End Sub

    Protected Sub SearchParamsChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlSearch.OnTreeNodeSelect, btnSearchNonSpecial.Click
        BindProducts()
    End Sub

    Private Function ConcatProductIds(ByVal x As List(Of String)) As String
        Dim Conn As String = ""
        Dim ReturnVal As String = ""
        For Each item In x
            ReturnVal &= Conn & item
            Conn = ","
        Next
        Return ReturnVal
    End Function

    Protected Sub rptNonSpecialProducts_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptNonSpecialProducts.ItemCommand
        Dim qty As TextBox = e.Item.FindControl("txtQtyNonSpecial")
        Dim NonSpecialProductsVendorSku As String = CType(e.Item.FindControl("hdnNonSpecialProductsVendorSku"), HiddenField).Value
        If Not IsNumeric(qty.Text) Or qty.Text <= 0 Then
            Exit Sub
        End If

        Dim Price As Double = CDbl(e.CommandArgument)
        Dim ProductId As Integer = CInt(e.CommandName)
        Dim Quantity As Integer = CInt(qty.Text)

        TwoPriceOrderProductRow.AddProduct(DB, CurrentOrder.OrderID, ProductId, Price, Quantity, VendorSku:=NonSpecialProductsVendorSku)

        BindOrder()

        qty.Text = String.Empty
        upTakeoff.Update()
        upResults.Update()
    End Sub

#End Region

#Region "Existing TakeOff Products"
    Private Sub BindPreviousTakeoffData(Optional ByVal strTakeOffFilter As String = "")
        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        gvList.DataKeyNames = New String() {"TakeoffId"}
        gvList.Pager.NofRecords = TakeOffRow.GetBuilderTakeoffCount(DB, BuilderId)
        gvList.PagerSettings.Visible = False
        Dim res As DataTable = TakeOffRow.GetBuilderTakeoffs(DB, BuilderId, gvList.SortBy, gvList.SortOrder, gvList.PageIndex + 1, gvList.PageSize, "", strTakeOffFilter)

        gvList.DataSource = res.DefaultView
        gvList.DataBind()
        If gvList.Rows.Count > 0 Then
            gvList.UseAccessibleHeader = True
            gvList.HeaderRow.TableSection = TableRowSection.TableHeader
            '  gvList.FooterRow.TableSection = TableRowSection.TableFooter
        End If
    End Sub

    Private Sub RefreshNotPricedProducts()
        Dim VendorRegularPricedProducts As DataTable = DB.GetDataTable("SELECT   vpp.ProductID  AS ProductId,   vpp.VendorPrice AS Price FROm  VendorProductPrice vpp " &
                                                          " WHERE ( VendorPrice is Not NULL AND vpp.VendorId = " & VendorId & ")")
        Dim RegularPricing As Dictionary(Of String, String) = VendorRegularPricedProducts.AsEnumerable.ToDictionary(Function(dr) CStr(dr.Item("ProductId")), Function(dr) ConStr(dr.Item("Price")))

        Dim dtPendingPricingProductsInCurrentOrder As DataTable = TwoPriceBuilderTakeOffProductPendingRow.GetAllPendingPricingProductsWithInitOrPendingStatus(DB, CurrentOrder.TwoPriceOrderID)
        For Each Row As DataRow In dtPendingPricingProductsInCurrentOrder.Rows
            If IsDBNull(Row.Item("ProductId")) Or IsDBNull(Row.Item("Quantity")) Then Continue For
            Dim ProductId As Integer = Row.Item("ProductId")
            Dim VendorSkuFromTakeoff As String = String.Empty
            VendorSkuFromTakeoff = VendorProductPriceRow.GetRow(DB, VendorId, ProductId).VendorSKU

            Dim Quantity As Integer = Row.Item("Quantity")
            If RegularPricing.ContainsKey(ProductId) AndAlso RegularPricing(ProductId) <> "" Then
                TwoPriceOrderProductRow.AddProduct(DB, CurrentOrder.TwoPriceOrderID, ProductId, RegularPricing(ProductId), Quantity, VendorSku:=VendorSkuFromTakeoff)
                TwoPriceBuilderTakeOffProductPendingRow.RemoveRow(DB, Row.Item("TwoPriceBuilderTakeOffProductPendingID"))
                TwoPriceVendorProductPriceRequestRow.RemoveProductRequests(DB, ProductId)
            End If

            Dim TwoPriceBuilderTakeoffSubstitutions As DataTable = TwoPriceTakeOffRow.GetTwoPriceBuilderTakeoffSubstitutions(DB, Row.Item("TwoPriceBuilderTakeOffProductPendingID"), VendorId)
            Dim TwoPriceBuilderTakeOffProductPending As TwoPriceBuilderTakeOffProductPendingRow = TwoPriceBuilderTakeOffProductPendingRow.GetRow(DB, Row.Item("TwoPriceBuilderTakeOffProductPendingID"))
            For Each drRow In TwoPriceBuilderTakeoffSubstitutions.Rows
                TwoPriceBuilderTakeOffProductPending.PriceRequestState = PriceRequestState.SubstitutionAvailable
                TwoPriceBuilderTakeOffProductPending.SubstituteProductID = drRow("SubProductID")
                TwoPriceBuilderTakeOffProductPending.VendorPrice = drRow("VendorPrice")
                If Not IsDBNull(Row("VendorSku")) Then
                    TwoPriceBuilderTakeOffProductPending.VendorSku = drRow("VendorSku")
                End If
                TwoPriceBuilderTakeOffProductPending.Update()
                Dim TwoPriceBuilderTakeOffProductSubstitute As TwoPriceBuilderTakeOffProductSubstituterow
                TwoPriceBuilderTakeOffProductSubstitute = New TwoPriceBuilderTakeOffProductSubstituterow(DB)
                TwoPriceBuilderTakeOffProductSubstitute.TwoPriceBuilderTakeOffProductPendingID = TwoPriceBuilderTakeOffProductPending.TwoPriceBuilderTakeOffProductPendingID
                TwoPriceBuilderTakeOffProductSubstitute.VendorID = VendorId
                TwoPriceBuilderTakeOffProductSubstitute.CreatorVendorAccountID = 1
                TwoPriceBuilderTakeOffProductSubstitute.SubstituteProductID = drRow("SubProductID")
                TwoPriceBuilderTakeOffProductSubstitute.RecommendedQuantity = drRow("RecommendedQuantity")
                TwoPriceBuilderTakeOffProductSubstitute.Insert()
            Next


        Next



    End Sub
    Protected Sub btnAddTakeOffProducts_Click(sender As Object, e As EventArgs) Handles btnAddTakeOffProducts.Click
        Dim cnt As Integer = 0
        Dim TakeOffTitle As String = ""

        'Get all vendor priced products
        Dim VendorPricedProducts As DataTable = DB.GetDataTable("SELECT  tpp.ProductId  AS ProductId, tpp.Price AS TwoPrice,  tpp.TwoPriceProductPriceID, tpp.TwoPriceCampaignID FROM TwoPriceVendorProductPrice tpp " &
                                                              " WHERE (tpp.TwoPriceCampaignID = " & dbTwoPriceCampaign.TwoPriceCampaignId & " AND tpp.Submitted = 1 AND tpp.VendorId = " & VendorId & ")")

        Dim VendorRegularPricedProducts As DataTable = DB.GetDataTable("SELECT   vpp.ProductID  AS ProductId,   vpp.VendorPrice AS Price FROm  VendorProductPrice vpp " &
                                                           " WHERE ( VendorPrice is Not NULL AND vpp.VendorId = " & VendorId & ")")


        'Convert it to wo seperate dictionaries for easy lookup
        Dim TwoPricePricing As Dictionary(Of String, String) = VendorPricedProducts.AsEnumerable.ToDictionary(Function(dr) CStr(dr.Item("ProductId")), Function(dr) ConStr(dr.Item("TwoPrice")))
        Dim RegularPricing As Dictionary(Of String, String) = VendorRegularPricedProducts.AsEnumerable.ToDictionary(Function(dr) CStr(dr.Item("ProductId")), Function(dr) ConStr(dr.Item("Price")))

        Dim UnPricedProducts As New List(Of DataRow)

        For Each row As GridViewRow In gvList.Rows
            Dim cb As CheckBox = row.FindControl("cbInclude")
            If cb.Checked Then
                cnt += 1
                Dim TakeoffId As Integer = gvList.DataKeys(row.RowIndex)(0)
                TakeOffTitle = gvList.Rows(row.RowIndex).Cells(2).Text

                Dim ImportedTakeOffIDList As String
                ImportedTakeOffIDList = CurrentOrder.ImportedTakeOffID
                ImportedTakeOffIDList = String.Concat(CurrentOrder.ImportedTakeOffID, ",", TakeoffId.ToString())

                If ImportedTakeOffIDList.Substring(0, 1) = "," Then
                    ImportedTakeOffIDList = ImportedTakeOffIDList.Substring(1)
                End If
                CurrentOrder.ImportedTakeOffID = ImportedTakeOffIDList

                CurrentOrder.Update()

                Dim dbTakeOff As DataTable = TakeOffRow.GetTakeoffProductsWithSpecialOrderProducts(DB, TakeoffId)       'Changed by Apala (Medullus) on 02.02.2018 for mGuard#T-1086

                For Each dtRow As DataRow In dbTakeOff.Rows
                    If IsDBNull(dtRow.Item("ProductId")) Or IsDBNull(dtRow.Item("Quantity")) Then Continue For

                    Dim ProductType As String = dtRow.Item("ProductType")
                    Dim ProductId As Integer = dtRow.Item("ProductId")
                    Dim Quantity As Integer = dtRow.Item("Quantity")

                    Dim VendorSkuFromTakeoff As String = String.Empty
                    If ProductType = "Regular" Then
                        VendorSkuFromTakeoff = VendorProductPriceRow.GetRow(DB, VendorId, ProductId).VendorSKU

                        If TwoPricePricing.ContainsKey(ProductId) AndAlso TwoPricePricing(ProductId) <> "" Then
                            'TwoPriceOrderProductRow.AddProduct(DB, CurrentOrder.TwoPriceOrderID, ProductId, TwoPricePricing(ProductId), Quantity, VendorSku:=VendorSkuFromTakeoff)
                            TwoPriceOrderProductRow.AddProduct(DB, CurrentOrder.TwoPriceOrderID, ProductId, TwoPricePricing(ProductId), Quantity, VendorSku:=VendorSkuFromTakeoff, TakeOffID:=TakeoffId)
                        ElseIf RegularPricing.ContainsKey(ProductId) AndAlso RegularPricing(ProductId) <> "" Then
                            'TwoPriceOrderProductRow.AddProduct(DB, CurrentOrder.TwoPriceOrderID, ProductId, RegularPricing(ProductId), Quantity, VendorSku:=VendorSkuFromTakeoff)
                            TwoPriceOrderProductRow.AddProduct(DB, CurrentOrder.TwoPriceOrderID, ProductId, RegularPricing(ProductId), Quantity, VendorSku:=VendorSkuFromTakeoff, TakeOffID:=TakeoffId)
                        Else
                            AddUnpricedProduct(ProductId, 0.0, Quantity, RequestState:=PriceRequestState.Init)
                            'UnPricedProducts.Add(dtRow)
                        End If
                    ElseIf ProductType = "Special" Then

                        AddUnpricedSpecialProduct(ProductId, 0.0, Quantity, RequestState:=PriceRequestState.Init)
                    End If
                Next

                cb.Checked = False
            End If
        Next

        BindOrder()
        upTakeoff.Update()

        If UnPricedProducts.Count > 0 Then
            ' rptProductsNotIncluded.DataSource = UnPricedProducts.CopyToDataTable()
            ' rptProductsNotIncluded.DataBind()

            '   open form here
        End If
    End Sub




    Private Function ConStr(x As Object) As String
        If Not IsDBNull(x) Then
            Return CStr(x)
        Else
            Return ""
        End If
    End Function
#End Region

#Region "Product Export"
    Public Sub ExportPriceCSV() Handles btnExport.Click, btnExport2.Click
        If rptProducts.DataSource = Nothing Then BindProducts()

        Dim dt As DataTable = CType(rptProducts.DataSource, DataTable)

        SaveExport(dt)
    End Sub

    Private Sub SaveExport(ByVal q As DataTable)
        Dim VendorSKU As String = String.Empty
        Dim CbusaSKU As String = String.Empty
        Dim ProductName As String = String.Empty
        Dim TakeoffPrice As String = String.Empty

        Dim fname As String = "/assets/vendor/product/" & Core.GenerateFileID & ".csv"
        Dim sw As IO.StreamWriter = IO.File.CreateText(Server.MapPath(fname))
        sw.WriteLine("Vendor SKU, Vendor Takeoff Price, CBUSA SKU, ProductName")
        For Each row As DataRow In q.Rows
            If Not IsDBNull(row.Item("ProductId")) AndAlso Not IsDBNull(row.Item("VendorId")) Then
                Dim res = DB.ExecuteScalar("SELECT TOP 1 VendorSKU FROM VendorProductPrice WHERE VendorID = " & row.Item("VendorId") & " AND ProductId = " & row.Item("ProductId"))
                If res IsNot DBNull.Value Then VendorSKU = res
            End If

            If Not IsDBNull(row.Item("SKU")) Then
                CbusaSKU = row.Item("SKU")
            End If

            If Not IsDBNull(row.Item("ProductName")) Then
                ProductName = row.Item("ProductName")
            End If

            If Not IsDBNull(row.Item("Price")) Then
                TakeoffPrice = row.Item("Price")
            End If

            sw.WriteLine(Core.QuoteCSV(VendorSKU) & "," & Core.QuoteCSV(TakeoffPrice) & "," & Core.QuoteCSV(CbusaSKU) & "," & Core.QuoteCSV(ProductName))
        Next
        sw.Close()

        Response.Redirect(fname)
    End Sub
#End Region

    Protected Sub btnExportcsv_Click(sender As Object, e As System.EventArgs) Handles btnExportcsv.Click
        Dim dtCurrentOrderProducts As DataTable = TwoPriceOrderRow.GetOrderProducts(DB, CurrentOrder.OrderID)
        Dim dtGetAllProductsWithPendingPricing As DataTable = TwoPriceBuilderTakeOffProductPendingRow.GetAllProductsWithPendingPricingWithCbusaSKU(DB, CurrentOrder.OrderID)

        ' If dtCurrentOrderProducts.Rows.Count > 0 Then
        ExportTakeoff(dtCurrentOrderProducts, dtGetAllProductsWithPendingPricing)
        ' End If


    End Sub

    Private Sub ExportTakeoff(ByVal dtCurrentOrderProducts As DataTable, ByVal dtGetAllProductsWithPendingPricing As DataTable)
        Dim SKU As String = String.Empty
        Dim Product As String = String.Empty

        Dim UnitPrice As String = String.Empty
        Dim TotalPrice As String = String.Empty
        Dim Quantity As String = String.Empty

        Dim SubTotalQnty As Decimal = 0
        Dim SubTotalPrice As Decimal = 0


        Dim fname As String = "/assets/takeoff/" & Core.GenerateFileID & ".csv"
        Dim sw As IO.StreamWriter = IO.File.CreateText(Server.MapPath(fname))
        '******Start* Line Added by Debashis (Medullus) on 05.09.2018 for USER STORY- 16464  *******
        ''''Not Priced By Vendor

        sw.WriteLine("Products Currently not Priced by Vendor : ")
        sw.WriteLine("CBUSA SKU,Product Name,Unit Price,Quantity,Total Price")
        For Each row As Object In dtGetAllProductsWithPendingPricing.Rows

            If Not IsDBNull(row.Item("ProductSku")) Then
                SKU = row.Item("ProductSku")
            Else
                SKU = ""
            End If

            If Not IsDBNull(row.Item("Product")) Then
                Product = row.Item("Product")
            Else
                Product = ""
            End If
            If Not IsDBNull(row.Item("VendorPrice")) Then
                UnitPrice = row.Item("VendorPrice")
            Else
                UnitPrice = ""
            End If
            If Not IsDBNull(row.Item("Quantity")) Then
                Quantity = row.Item("Quantity")
            Else
                Quantity = ""
            End If
            If Not IsDBNull(row.Item("VendorPrice")) AndAlso Not IsDBNull(row.Item("Quantity")) Then
                TotalPrice = UnitPrice * Quantity
            Else
                TotalPrice = ""
            End If

            sw.WriteLine(Core.QuoteCSV(SKU) & "," & Core.QuoteCSV(Product) & "," & Core.QuoteCSV(UnitPrice) & "," & Core.QuoteCSV(Quantity) & "," & Core.QuoteCSV(TotalPrice))
        Next
        '******End* Line Added by Debashis (Medullus) on 05.09.2018 for USER STORY- 16464  *******
        '''' Priced By Vendor
        sw.WriteLine("")
        sw.WriteLine("Products Currently Priced by Vendor : ")
        sw.WriteLine("CBUSA SKU,Product Name,Unit Price,Quantity,Total Price")
        For Each row As Object In dtCurrentOrderProducts.Rows

            If Not IsDBNull(row.Item("ProductSku")) Then
                SKU = row.Item("ProductSku")
            Else
                SKU = ""
            End If

            If Not IsDBNull(row.Item("Product")) Then
                Product = row.Item("Product")
            Else
                Product = ""
            End If
            If Not IsDBNull(row.Item("Price")) Then
                UnitPrice = row.Item("Price")
            Else
                UnitPrice = ""
            End If
            If Not IsDBNull(row.Item("Quantity")) Then
                Quantity = row.Item("Quantity")
            Else
                Quantity = ""
            End If
            If Not IsDBNull(row.Item("Price")) AndAlso Not IsDBNull(row.Item("Quantity")) Then
                TotalPrice = UnitPrice * Quantity
            Else
                TotalPrice = ""
            End If

            SubTotalQnty = SubTotalQnty + Quantity
            SubTotalPrice = SubTotalPrice + TotalPrice

            sw.WriteLine(Core.QuoteCSV(SKU) & "," & Core.QuoteCSV(Product) & "," & Core.QuoteCSV(UnitPrice) & "," & Core.QuoteCSV(Quantity) & "," & Core.QuoteCSV(TotalPrice))
        Next

        sw.WriteLine("" & "," & "" & "," & "" & "," & "Sub Total" & "," & Core.QuoteCSV(SubTotalPrice))

        sw.Flush()
        sw.Close()
        sw.Dispose()
        Response.Redirect(fname)

    End Sub



    Protected Sub btnRequestPricingAll_Click(sender As Object, e As System.EventArgs) Handles btnRequestPricingAll.Click
        RequestPricing(DB, CurrentOrder.OrderID)
        BindOrder()
    End Sub

    Public Sub RequestPricing(ByVal Db As Database, ByVal OrderID As Integer)
        Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(Db, "MissingPrice")
        'Dim dbRecip As New automatic
        Dim dbVendor As VendorRow = VendorRow.GetRow(Db, VendorId)
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(Db, Session("BuilderId"))
        Dim msg As New StringBuilder

        msg.Append(dbBuilder.CompanyName & " is requesting pricing for the following items:" & vbCrLf & vbCrLf)

        Dim dtProducts As DataTable = TwoPriceBuilderTakeOffProductPendingRow.GetAllPendingPricingProductsWithInitOrPendingStatus(Db, CurrentOrder.OrderID)

        Dim HasProductList As Boolean = False

        For Each row As DataRow In dtProducts.Rows
            Dim dbTwoPriceBuilderTakeOffProductPending As TwoPriceBuilderTakeOffProductPendingRow = TwoPriceBuilderTakeOffProductPendingRow.GetRow(Db, row("TwoPriceBuilderTakeOffProductPendingID"))
            dbTwoPriceBuilderTakeOffProductPending.PriceRequestState = PriceRequestState.RequestPending
            Dim dbRequest As TwoPriceVendorProductPriceRequestRow = TwoPriceVendorProductPriceRequestRow.GetRow(Db, Session("BuilderId"), row("VendorId"), row("TwoPriceBuilderTakeOffProductPendingID"))
            '    Dim dbBuilder As BuilderRow = BuilderRow.GetRow(Db, Session("BuilderId"))
            '    Dim dbVendor As VendorRow = VendorRow.GetRow(Db, VendorId)
            If dbRequest.Created = Nothing Then
                dbRequest.BuilderID = Session("BuilderId")
                dbRequest.CreatorBuilderAccountID = Session("BuilderAccountId")
                dbRequest.ProductID = dbTwoPriceBuilderTakeOffProductPending.ProductID
                dbRequest.SpecialOrderProductID = dbTwoPriceBuilderTakeOffProductPending.SpecialOrderProductID
                dbRequest.TwoPriceBuilderTakeOffProductPendingID = row("TwoPriceBuilderTakeOffProductPendingID")
                dbRequest.VendorID = VendorId
                dbRequest.TwoPriceCampaignID = dbTwoPriceCampaign.TwoPriceCampaignId
                dbRequest.TwoPriceOrderID = CurrentOrder.OrderID
                dbRequest.Insert()
            End If

            HasProductList = True

            If dbTwoPriceBuilderTakeOffProductPending.ProductID <> Nothing Then
                Dim dbProduct As ProductRow = ProductRow.GetRow(Db, dbTwoPriceBuilderTakeOffProductPending.ProductID)
                If dbProduct.SKU = String.Empty Then
                    msg.Append(dbProduct.Product)
                Else
                    msg.Append("CBUSA Sku # " & dbProduct.SKU & " - " & dbProduct.Product)
                End If

            ElseIf dbTwoPriceBuilderTakeOffProductPending.SpecialOrderProductID <> Nothing Then
                Dim dbProduct As SpecialOrderProductRow = SpecialOrderProductRow.GetRow(Db, dbTwoPriceBuilderTakeOffProductPending.SpecialOrderProductID)
                msg.Append(dbProduct.SpecialOrderProduct)
            End If
            msg.AppendLine()

            dbTwoPriceBuilderTakeOffProductPending.Update()
        Next

        If HasProductList Then
            msg.AppendLine()
            msg.Append("Please click the link below to access the CBUSA software")
            msg.AppendLine()
            msg.Append("https://app.custombuilders-usa.com/default.aspx")
            'msg.AppendLine(dbMsg.Message)

            Dim FromName As String = SysParam.GetValue(Db, "ContactUsName")
            Dim FromEmail As String = SysParam.GetValue(Db, "ContactUsEmail")

            Dim sMsg As String = msg.ToString
            dbMsg.Send(dbVendor, msg.ToString)
        End If

    End Sub

    Protected Sub hdnPostback_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles hdnPostback.ValueChanged
        Dim filter As String = txtTakeOffFilter.Text

        BindPreviousTakeoffData(filter)
        updatePnl.Update()
    End Sub

    Public Function GetTakeOffIdByOrderId(ByVal DB As Database, ByVal OrderId As Integer) As String
        
        Dim sqlImportedTakeOffID = "SELECT ImportedTakeOffID from TwoPriceOrder where TwoPriceOrderId  = " & DB.Number(OrderId)
        Dim ImportedTakeOffIDList As String = ""
        Dim sdr As SqlDataReader = DB.GetReader(sqlImportedTakeOffID)
        If sdr.Read Then
            ImportedTakeOffIDList = IIf(IsDBNull(sdr("ImportedTakeOffID")), "", sdr("ImportedTakeOffID"))
        End If

        Dim sql As String = "SELECT SUBSTRING((SELECT ', ' + Title FROM TakeOff WHERE TakeOffId in (" & ImportedTakeOffIDList & ") ORDER BY TakeOff.Title FOR XML PATH ('')), 2, 1000) AS Title"
        Dim TakeOffTitle As String = ""

        If ImportedTakeOffIDList <> "" Then
            Dim sdr2 As SqlDataReader = DB.GetReader(sql)
            If sdr2.Read Then
                TakeOffTitle = "<br/><div style=width:60%;margin:0px auto;position:relative;background-color:#fff;border:1px solid #666;>Current Take Off Loaded Is: <br/> <h1 style=font-size:14px;>" & sdr2("Title") & "</h1></div>"
            End If

            sdr2.Close()
        End If

        sdr.Close()

        Return TakeOffTitle

    End Function

End Class
