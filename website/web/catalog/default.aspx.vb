﻿Imports Components
Imports DataLayer
Imports System.IO
Imports System.Linq
Imports System.Data
Imports InfoSoftGlobal

Partial Class catalog_default
    Inherits SitePage
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private OperationType As String = ""

    Protected ReadOnly Property ExportUrl() As String
        Get
            Dim Output As String = Request.ServerVariables("URL").ToString.Trim & "?"
            If Not Request.ServerVariables("QUERY_STRING") Is Nothing AndAlso Request.ServerVariables("QUERY_STRING").ToString.Trim <> String.Empty Then
                Dim TempArray As String() = Split(Request.ServerVariables("QUERY_STRING").ToString.Trim, "&")
                For Each Param As String In TempArray
                    Dim TempArray2 As String() = Split(Param.ToString.Trim, "=")
                    If UBound(TempArray2) = 1 Then
                        If TempArray2(0).ToString.Trim <> String.Empty AndAlso TempArray2(0).ToString.ToLower.Trim <> "export" Then
                            If Right(Output, 1) <> "?" Then Output &= "&"
                            Output &= TempArray2(0).ToString.Trim & "=" & TempArray2(1).ToString.Trim
                        End If
                    End If
                Next
                If Right(Output, 1) <> "?" Then Output &= "&"
            End If
            Output &= "export=y"
            Return Output
        End Get
    End Property

    Protected ReadOnly Property VendorId() As Integer
        Get
            If Session("VendorId") IsNot Nothing Then
                Return Session("VendorId")
            Else
                'Dim header As Controls.IHeaderControl = Page.FindControl("CTMain").FindControl("ctrlHeader")
                'If header.ReturnValue <> String.Empty AndAlso header.ReturnValue <> "0" Then
                '    Return header.ReturnValue
                'Else
                '    Return Nothing
                'End If

                If slPreferredVendor2.Value <> String.Empty AndAlso slPreferredVendor2.Value <> "0" Then
                    Return slPreferredVendor2.Value
                Else
                    Return Nothing
                End If
            End If
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("BuilderId") Is Nothing And Session("VendorId") Is Nothing And Session("PIQId") Is Nothing And (Not TypeOf Page.User Is AdminPrincipal OrElse CType(Page.User, AdminPrincipal).Username = Nothing) Then
            Response.Redirect("/default.aspx")
        End If

        If Session("PIQId") IsNot Nothing Then
            If Not PIQRow.GetRow(DB, Session("PIQId")).HasCatalogAccess Then
                phCatalog.Visible = False
                ltlMsg.Text = "You do not have access to this section of the website. <a href=""/default.aspx"">Click here</a> to go back to the homepage."
                Exit Sub
            End If

        End If

        Dim header As Controls.IHeaderControl = Page.FindControl("CTMain").FindControl("ctrlHeader")
        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(CType(header, Control).FindControl("slPreferredVendor"))
        AddHandler header.ControlEvent, AddressOf UpdateVendor

        If Session("BuilderId") IsNot Nothing Then
            slPreferredVendor2.WhereClause = " IsActive = 1 and VendorID in (select VendorID from LLCVendor l inner join Builder b on l.LLCID=b.LLCID where exists (select * from VendorProductPrice where VendorId=l.VendorId) and b.BuilderID=" & DB.Number(Session("BuilderID")) & ")"
            pnlPreferredVendor.Visible = True
            AddHandler slPreferredVendor2.ValueChanged, AddressOf UpdateVendor
        End If

        'ctlSearch.MaxDocs = 32000
        If ctlSearch.CatalogType <> controls_SearchSql.CATALOG_TYPE.CATALOG_TYPE_ALL AndAlso VendorId <> Nothing Then
            'ctlSearch.FilterListCallback = AddressOf FilterCallback
            'ctlSearch.FilterCacheKey = VendorId
            Dim list As New Generic.List(Of String)
            FilterCallback(list)
            'ctlSearch.FilterList = list
            thPrice.Visible = True
            thHistory.Visible = True
            thVendorSku.Visible = True
        Else
            'ctlSearch.FilterListCallback = Nothing
            'ctlSearch.FilterCacheKey = Nothing
            'ctlSearch.FilterList = Nothing

            thPrice.Visible = False
            thHistory.Visible = False
            thVendorSku.Visible = False
        End If

 PageURL = Request.Url.ToString()
        If Session("BuilderId") IsNot Nothing Then
            CurrentUserId = Session("BuilderId")
            OperationType = "Builder Top Menu Click"
        Else
            CurrentUserId = Session("VendorId")
            OperationType = "Vendor Top Menu Click"
        End If
        UserName = Session("Username")

        If Not IsPostBack Then

        Core.DataLog("Catalog", PageURL, CurrentUserId, OperationType , "", "", "", "", UserName)

            If Session("BuilderId") IsNot Nothing Then
                Session("CurrentPreferredVendor") = Nothing

                Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
                ctlSearch.FilterLLCID = dbBuilder.LLCID
                Session("InitialllcFilter") = dbBuilder.LLCID
                Dim list As New Generic.List(Of String)
                GetFilterList(list)
                'ctlSearch.FilterList = list
            End If
            ctlSearch.SearchProduct()
        End If

        If Not Request("export") Is Nothing AndAlso Request("export").ToString.Trim <> String.Empty Then
            'Response.Clear()
            'Response.Write("Creating export file.  Please wait...")
            'Response.Flush()
            ExportList()
        End If
       
    End Sub

    Protected Sub GetFilterList(ByRef list As Generic.List(Of String))
        list.AddRange(LLCRow.GetPricedProductsListForSearch(DB, BuilderRow.GetRow(DB, Session("BuilderId")).LLCID))
    End Sub

    Protected Sub UpdateVendor(ByVal sender As Object, ByVal e As System.EventArgs)
        If VendorId = Nothing Then
            'ctlSearch.FilterListCallback = Nothing
            'ctlSearch.FilterCacheKey = Nothing
            'ctlSearch.FilterList = Nothing
        Else
            'ctlSearch.FilterListCallback = AddressOf FilterCallback
            'ctlSearch.FilterCacheKey = VendorId
            Dim list As New Generic.List(Of String)
            FilterCallback(list)
            'ctlSearch.FilterList = list
        End If
        ctlSearch.SearchProduct(0, True, True)
        upResults.Update()
    End Sub

    Private Sub RegisterQuoteScript()
        Dim s As String = _
              "function OpenQuoteForm(productId) {" _
            & " var frm = $get('<%=frmQuote.ClientID %>').control;" _
            & " frm.get_input('hdnProductID').value = productId;" _
            & " var drp = frm.get_input('drpQuoteVendor');" _
            & " for(var i = 0; i < drp.options.length; i++) {" _
            & "     if(drp.options[i].value == " & VendorId & ") {" _
            & "         drp.options[i].selected = true;" _
            & "         break;" _
            & "     }" _
            & " }" _
            & " frm._doMoveToCenter();" _
            & " frm.Open();" _
            & "}"

        ScriptManager.RegisterClientScriptBlock(Page, Me.GetType, "QuoteScript", s, True)
    End Sub

    Private Sub BindData()

        If ctlSearch.CatalogType <> controls_SearchSql.CATALOG_TYPE.CATALOG_TYPE_ALL AndAlso VendorId <> Nothing Then
            Dim dtPrices As DataTable = VendorProductPriceRow.GetAllVendorPrices(DB, VendorId)

            rptProducts.DataSource = _
                From r As DataRow In ctlSearch.SearchResults.AsEnumerable _
                Group Join p As DataRow In dtPrices.Rows On Core.GetInt(r("ProductID")) Equals Core.GetInt(p("ProductID")) _
                Into grp = Group _
                From item In grp.Select(Of String())(Function(dr, idx) New String() {Core.GetString(dr("VendorPrice")), Core.GetString(dr("VendorSku")), Core.GetString(dr("SubstitutePrice")), Core.GetString(dr("SubstituteSku")), Core.GetInt(dr("SubstituteQuantityMultiplier")), Core.GetBoolean(dr("IsSubstitution")), Core.GetString(dr("SubstituteProduct"))}) _
                Select New With { _
                    .ProductID = r("ProductID"), _
                    .Product = r("Product"), _
                    .Sku = r("Sku"), _
                    .VendorPrice = item(0), _
                    .VendorSku = item(1), _
                    .SubstitutePrice = item(2), _
                    .SubstituteSku = item(3), _
                    .SubstituteQuantityMultiplier = item(4), _
                    .IsSubstitution = item(5), _
                    .SubstituteProduct = item(6) _
                }
            rptProducts.DataBind()
        Else
            rptProducts.DataSource = ctlSearch.SearchResults
            rptProducts.DataBind()
        End If
        If ctlSearch.SearchResults.Rows.Count > 0 Then
            ctlNavigator.NofRecords = ctlSearch.SearchResults.Rows(0).Item(3)
        Else
            ctlNavigator.NofRecords = 0
        End If
        ctlNavigator.DataBind()

        'rptProducts.DataSource = ctlSearch.SearchResults.ds.Tables(0)
        rptProducts.DataBind()

    End Sub

    Protected Sub ctlSearch_OnTreeNodeSelect(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlSearch.OnTreeNodeSelect
        If VendorId <> Nothing Then
            Dim list As New Generic.List(Of String)
            FilterCallback(list)
            ctlSearch.FilterList = list
        End If
    End Sub

    Protected Sub ctlSearch_ResultsUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlSearch.ResultsUpdated
        If ctlSearch.SearchResults.Rows.Count > 0 Then
            ltlNoResults.Visible = False
            ctlNavigator.Visible = True
        Else
            ctlNavigator.Visible = False
            ltlNoResults.Visible = True
        End If

        BindData()
        ltlBreadcrumbs.Text = ctlSearch.Breadcrumbs

        upResults.Update()
    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        ctlSearch.SearchProduct(0, False, False)
    End Sub

    Protected Sub btnExport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExport.Click
        'If Convert.ToInt32(ctlNavigator.NofRecords) > 5000 Then
        '    ScheduleExport()
        'Else
        '    Response.Redirect(ExportUrl)
        'End If

        ExportList()

    End Sub

    Protected Sub FilterCallback(ByRef list As Generic.List(Of String))
        Dim dt As DataTable = VendorProductPriceRow.GetAllVendorPrices(DB, VendorId)
        list.AddRange(From dr As DataRow In dt.AsEnumerable Where Not IsDBNull(dr("VendorPrice")) And Not Core.GetBoolean(dr("IsSubstitution")) Select Convert.ToString(dr("ProductID")))
        list.AddRange(From dr As DataRow In dt.AsEnumerable Where Not IsDBNull(dr("SubstitutePrice")) And Core.GetBoolean(dr("IsSubstitution")) Select Convert.ToString(dr("ProductID")))
        'list.AddRange(From dr As DataRow In dt.AsEnumerable Select Convert.ToString(dr("ProductID")))
    End Sub

    Private Sub ScheduleExport()
        divNotification.Visible = True
        Dim IdValue As Integer = 0
        Dim IdField As String = String.Empty
        Dim NotificationEmail As String = String.Empty
        If CType(Me.Page, SitePage).IsLoggedInBuilder AndAlso Not Session("BuilderAccountId") Is Nothing Then
            IdValue = Convert.ToInt32(Session("BuilderAccountId"))
            IdField = "BuilderAccountId"
            Dim dbAccount As BuilderAccountRow = BuilderAccountRow.GetRow(DB, IdValue)
            NotificationEmail = dbAccount.Email.ToString.Trim
        ElseIf CType(Me.Page, SitePage).IsLoggedInVendor AndAlso Not Session("VendorAccountId") Is Nothing Then
            IdValue = Convert.ToInt32(Session("VendorAccountId"))
            IdField = "VendorAccountId"
            Dim dbAccount As VendorAccountRow = VendorAccountRow.GetRow(DB, IdValue)
            NotificationEmail = dbAccount.Email.ToString.Trim
        ElseIf CType(Me.Page, SitePage).IsLoggedInPIQ AndAlso Not Session("PIQAccountId") Is Nothing Then
            IdValue = Convert.ToInt32(Session("PIQAccountId"))
            IdField = "PIQAccountId"
            NotificationEmail = String.Empty
        End If
        If IdValue <= 0 Then Exit Sub

        ctlSearch.PageSize = 32000
        ctlSearch.PageNumber = 1
        ctlSearch.SearchProduct()
        Dim Parameters As String = "NotificationEmail=" & NotificationEmail.ToString.Trim
        If VendorId = Nothing Then
            Parameters &= "&VendorId="
        Else
            Parameters &= "&VendorId=" & VendorId.ToString.Trim
        End If
        Dim ProductIDList As New StringBuilder
        Dim Comma As String = String.Empty
        Dim res As Object = ctlSearch.SearchResults.DefaultView
        For Each dr As Object In res
            If dr("ProductID").ToString.Trim <> String.Empty Then
                ProductIDList.Append(Comma & dr("ProductID").ToString.Trim)
                Comma = ","
            End If
        Next
        Parameters &= "&ProductIDList=" & ProductIDList.ToString.Trim
        ctlSearch.PageSize = 25
        ctlSearch.PageNumber = ctlNavigator.PageNumber
        ctlSearch.SearchProduct()
        upResults.Update()

        ExportQueueRow.Add(DB, IdValue, IdField, "product_catalog", Parameters)


    End Sub

    Private Sub ExportList()
        ctlSearch.PageSize = 25000
        ctlSearch.SearchProduct(0, False, False)

        Dim Res As Object
        If ctlSearch.CatalogType <> controls_SearchSql.CATALOG_TYPE.CATALOG_TYPE_ALL AndAlso VendorId <> Nothing Then
            Dim dtPrices As DataTable = VendorProductPriceRow.GetAllVendorPrices(DB, VendorId)
            Res = _
                From r As DataRow In ctlSearch.SearchResults.AsEnumerable _
                Group Join p As DataRow In dtPrices.Rows On Core.GetInt(r("ProductID")) Equals Core.GetInt(p("ProductID")) _
                Into grp = Group _
                From item In grp.Select(Of String())(Function(dr, idx) New String() {Core.GetString(dr("VendorPrice")), Core.GetString(dr("VendorSku")), Core.GetString(dr("SubstitutePrice")), Core.GetString(dr("SubstituteSku")), Core.GetInt(dr("SubstituteQuantityMultiplier")), Core.GetBoolean(dr("IsSubstitution")), Core.GetString(dr("SubstituteProduct"))}) _
                Select New With { _
                    .ProductID = r("ProductID"), _
                    .Product = r("Product"), _
                    .Sku = r("SKU"), _
                    .VendorPrice = item(0), _
                    .VendorSku = item(1), _
                    .SubstitutePrice = item(2), _
                    .SubstituteSku = item(3), _
                    .SubstituteQuantityMultiplier = item(4), _
                    .IsSubstitution = item(5), _
                    .SubstituteProduct = item(6) _
                }
            rptProducts.DataBind()
        Else
            Res = ctlSearch.SearchResults.DefaultView
        End If

        Dim Folder As String = "/assets/catalogs/"
        Dim i As Integer = 0
        Dim FileName As String = Core.GenerateFileID & ".csv"

        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)

        If VendorId = Nothing Then
            sw.WriteLine("CBUSA Catalog")
            sw.WriteLine("Report generated on " & DateTime.Now.ToString("d"))
            sw.WriteLine(String.Empty)
            sw.WriteLine("CBUSA SKU,Product Name")
        Else
            Dim dbVendor As VendorRow = VendorRow.GetRow(DB, VendorId)
            'sw.WriteLine("Vendor:," & dbVendor.CompanyName & vbCrLf)
            'sw.WriteLine("SKU,Name,Substitute Product,Substitute Product SKU,Price")
            sw.WriteLine("Vendor SKU,Vendor Price,CBUSA SKU,Product Name")

        End If

        For Each dr As Object In res
            If VendorId = Nothing Then
                Dim SKU As String = Core.GetString(dr("SKU"))
                Dim ProductName As String = Core.GetString(dr("Product"))

                sw.WriteLine(Core.QuoteCSV(SKU) & _
                             "," & Core.QuoteCSV(ProductName))
            Else
                Dim SKU As String = dr.SKU
                Dim ProductName As String = dr.Product
                Dim SubProduct As String = dr.SubstituteProduct
                Dim SubSku As String = dr.SubstituteSku
                Dim Price As Double = IIf(dr.IsSubstitution, dr.SubstitutePrice, dr.VendorPrice)
                Dim VendorSku As String = dr.VendorSku

                'sw.WriteLine(Core.QuoteCSV(SKU) & _
                '             "," & Core.QuoteCSV(ProductName) & "," & Core.QuoteCSV(SubProduct) & _
                '             "," & Core.QuoteCSV(SubSku) & "," & Core.QuoteCSV(Price))

                sw.WriteLine(Core.QuoteCSV(VendorSku) & "," & Core.QuoteCSV(Price) & "," & Core.QuoteCSV(SKU) & "," & Core.QuoteCSV(ProductName))
            End If
        Next
        sw.Flush()
        sw.Close()
        sw.Dispose()

        ctlSearch.PageSize = 25
        ctlSearch.PageNumber = ctlNavigator.PageNumber
        'ctlSearch.SearchProduct()
        'upResults.Update()

        Response.Redirect(Folder & FileName)
    End Sub

    Protected Sub ctlNavigator_NavigatorEvent(ByVal sender As Object, ByVal e As Controls.NavigatorEventArgs) Handles ctlNavigator.NavigatorEvent
        ctlNavigator.PageNumber = e.PageNumber
        ctlSearch.PageNumber = ctlNavigator.PageNumber
        ctlSearch.SearchProduct()
        upResults.Update()
    End Sub

    Protected Sub rptProducts_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProducts.ItemCreated
        If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim btnHistory As Button = e.Item.FindControl("btnHistory")
        Dim btnIndicator As Button = e.Item.FindControl("btnIndicator")
        Dim btnPricing As Button = e.Item.FindControl("btnPricing")

        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btnHistory)
        AddHandler btnHistory.Command, AddressOf btnHistory_Click

        If Session("VendorId") IsNot Nothing Then
            btnIndicator.Visible = False
            btnPricing.Visible = False
        Else
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btnIndicator)
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btnPricing)
            AddHandler btnIndicator.Command, AddressOf btnIndicator_Click
            AddHandler btnPricing.Command, AddressOf btnPricing_Click
        End If
    End Sub

    Protected Sub rptProducts_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProducts.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        Dim tdPrice As HtmlTableCell = e.Item.FindControl("tdPrice")
        Dim tdHistory As HtmlTableCell = e.Item.FindControl("tdHistory")
        Dim tdVendorSku As HtmlTableCell = e.Item.FindControl("tdVendorSku")
        Dim ltlProductName As Literal = e.Item.FindControl("ltlProductName")

        'quote form disabled at customer request, 3/6
        Dim btnQuote As Button = e.Item.FindControl("btnQuote")
        btnQuote.Visible = False

        If VendorId = Nothing Or ctlSearch.CatalogType = controls_SearchSql.CATALOG_TYPE.CATALOG_TYPE_ALL Then
            If e.Item.ItemIndex = 0 Then
                thPrice.Visible = False
                thHistory.Visible = False
                thVendorSku.Visible = False
            End If

            tdPrice.Visible = False
            tdHistory.Visible = False
            tdVendorSku.Visible = False
        Else
            If e.Item.ItemIndex = 0 Then
                thPrice.Visible = True
                thHistory.Visible = True
                thVendorSku.Visible = True
            End If

            tdPrice.Visible = True
            tdVendorSku.Visible = True
            tdHistory.Visible = True

            If e.Item.DataItem.IsSubstitution Then
                tdPrice.InnerHtml = FormatCurrency(e.Item.DataItem.SubstitutePrice)
                tdVendorSku.InnerHtml = e.Item.DataItem.SubstituteSku
                ltlProductName.Text = e.Item.DataItem.Product & "<br/><span class=""bold smaller"">Substitute Product: " & e.Item.DataItem.SubstituteProduct & "</span>"
            Else
                tdPrice.InnerHtml = FormatCurrency(e.Item.DataItem.VendorPrice)
                tdVendorSku.InnerHtml = e.Item.DataItem.VendorSku
            End If
        End If
    End Sub

    Protected Sub btnHistory_Click(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs)
        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, VendorId)
        ltlVendor.Text = dbVendor.CompanyName

        Dim dbProduct As ProductRow = ProductRow.GetRow(DB, e.CommandArgument)
        ltlProduct.Text = dbProduct.Product
        hdnHistoryProductID.Value = dbProduct.ProductID

        'ltlChart.Text = FusionCharts.RenderChartHTML("../../FusionCharts/FCF_Line.swf", "", GetChartXML(dbVendor, dbProduct.ProductID), "HistoryChart", "600", "400", True)
        ltlChart.Text = GetChartHtml(dbVendor, dbProduct.ProductID)
        ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenPopup", "Sys.Application.add_load(OpenHistory);", True)
        upHistory.Update()
    End Sub

    Protected Sub frmHistory_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmHistory.Postback
        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, VendorId)
        Dim dbProduct As ProductRow = ProductRow.GetRow(DB, hdnHistoryProductID.Value)

        'ltlChart.Text = FusionCharts.RenderChartHTML("../../FusionCharts/FCF_Line.swf", "", GetChartXML(dbVendor, dbProduct.ProductID), "HistoryChart", "600", "400", True)
        ltlChart.Text = GetChartHtml(dbVendor, dbProduct.ProductID)
        ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenPopup", "Sys.Application.add_load(OpenHistory);", True)
    End Sub

    Protected Sub btnIndicator_Click(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs)
        Dim dbProduct As BuilderIndicatorProductRow = BuilderIndicatorProductRow.GetRow(DB, Session("BuilderId"), e.CommandArgument)
        If dbProduct.BuilderIndicatorProductID = Nothing Then
            dbProduct.BuilderID = Session("BuilderId")
            dbProduct.ProductID = e.CommandArgument
            dbProduct.Insert()
        End If
        ScriptManager.RegisterStartupScript(Page, Me.GetType, "IndicatorConfirm", "Sys.Application.add_load(OpenIndicatorConfirm);", True)
    End Sub

    Protected Sub btnPricing_Click(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs)

        Dim dbProduct As ProductRow = ProductRow.GetRow(DB, e.CommandArgument)
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
        Dim dtPricing As DataTable
        If dbBuilder.BuilderID <> Nothing Then
            dtPricing = VendorProductPriceRow.GetAllProductPrices(DB, dbProduct.ProductID, dbBuilder.LLCID)
        Else
            dtPricing = VendorProductPriceRow.GetAllProductPrices(DB, dbProduct.ProductID)
        End If

        frmPricing.EnsureChildrenCreated()

        ltlPricingCBUSASKU.Text = dbProduct.SKU
        ltlPricingDescription.Text = dbProduct.Description
        ltlPricingHeaderProduct.Text = dbProduct.Product
        ltlPricingProduct.Text = dbProduct.Product
        ltlPricingPriceLockDate.Text = ProductRow.GetPriceLockDate(DB, dbProduct.ProductID)

        If dtPricing.Rows.Count > 0 Then
            rptPricing.Visible = True
            rptPricing.DataSource = dtPricing
            rptPricing.DataBind()
            ltlNoPrices.Text = String.Empty
        Else
            rptPricing.Visible = False
            ltlNoPrices.Text = "<b>No vendors have supplied pricing for this product</b>"
        End If
        upPricing.Update()

        ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenPricingForm", "Sys.Application.add_load(OpenPricingForm);", True)

    End Sub

    Protected Function GetChartXML(ByVal Vendor As VendorRow, ByVal ProductID As Integer) As String

        If dtHistoryFrom.Value = Nothing Then
            dtHistoryFrom.Value = Date.Now.AddMonths(-6).ToShortDateString
        End If

        If dtHistoryTo.Value = Nothing Then
            dtHistoryTo.Value = Date.Today
        End If


        Dim dt As DataTable = VendorProductPriceHistoryRow.GetPriceHistory(DB, Vendor.VendorID, ProductID, dtHistoryFrom.Value, dtHistoryTo.Value)
        Dim out As New StringBuilder
        Dim sw As New StringWriter(out)
        Dim xml As New System.Xml.XmlTextWriter(sw)

        Dim yMax As Double = 100
        Dim yMin As Double = 0

        Dim q = (From dr As DataRow In dt.AsEnumerable Select Core.GetDouble(dr("VendorPrice")))
        If q.Count > 0 Then
            yMax = q.Max
            yMin = q.Min
        End If

        If yMax <> yMin Then
            yMax += (yMax - yMin) * 0.25
            yMin -= (yMax - yMin) * 0.25
        Else
            yMax += 0.25 * yMax
            yMin -= 0.25 * yMin
        End If
        If yMin < 0 Then
            yMin = 0
        End If

        xml.QuoteChar = "'"
        xml.WriteStartElement("graph")
        xml.WriteAttributeString("caption", "Price History")
        'xml.WriteAttributeString("subcaption", Vendor.CompanyName)
        xml.WriteAttributeString("xAxisName", "Date")
        xml.WriteAttributeString("yAxisName", "Price")
        xml.WriteAttributeString("numberPrefix", "$")
        xml.WriteAttributeString("yAxisMinValue", yMin)
        xml.WriteAttributeString("yAxisMaxValue", yMax)


        For Each row As DataRow In dt.Rows
            xml.WriteStartElement("set")
            xml.WriteAttributeString("value", row("VendorPrice"))
            xml.WriteAttributeString("name", FormatDateTime(Core.GetDate(row("Submitted")), DateFormat.ShortDate))
            xml.WriteAttributeString("hoverText", FormatDateTime(Core.GetDate(row("Submitted")), DateFormat.ShortDate))
            xml.WriteEndElement()
        Next
        xml.WriteEndElement()
        xml.Close()
        sw.Close()
        Return out.ToString
    End Function

    Protected Function GetChartHtml(ByVal vendor As VendorRow, ByVal ProductID As Integer)
        Dim html As String = _
            "<object classid=""clsid:d27cdb6e-ae6d-11cf-96b8-444553540000"" codebase=""http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=8,0,0,0"" width=""600"" height=""400"" name=""LineChart"">" _
            & "<param name=""allowScriptAccess"" value=""always"" />" _
            & "<param name=""movie"" value=""/FusionCharts/FCF_Line.swf"" />" _
            & "<param name=""FlashVars"" value=""&chartWidth=600&chartHeight=400&debugMode=0&dataXML=" & GetChartXML(vendor, ProductID) & """ />" _
            & "<param name=""quality"" value=""high"" />" _
            & "<param name=""wmode"" value=""transparent"" />" _
            & "<embed src=""/FusionCharts/FCF_Line.swf"" flashVars=""&chartWidth=600&chartHeight=400&debugMode=0&dataXML=" & GetChartXML(vendor, ProductID) & """ wmode=""transparent"" quality=""high"" width=""600"" height=""400"" name=""LineChart"" type=""application/x-shockwave-flash"" pluginspage=""http://www.macromedia.com/go/getflashplayer"" />" _
            & "</object>"

        Return html
    End Function

    Protected Sub frmQuote_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmQuote.Postback
        Dim dbProduct As ProductRow = ProductRow.GetRow(DB, hdnProductID.Value)
        spanQuoteProduct.InnerHtml = dbProduct.Product

        If dtHistoryDate.Value = Nothing OrElse DateDiff(DateInterval.Day, dtHistoryDate.Value, Now) < 0 Then
            ltlQuotePrice.Text = "<b class=""red smaller"">Please select a valid date</b>"
        Else
            'Dim dbHistory As VendorProductPriceHistoryRow = VendorProductPriceHistoryRow.GetRowByDate(DB, drpQuoteVendor.SelectedValue, hdnProductID.Value, dtHistoryDate.Value)
            Dim drHistory As DataRow = VendorProductPriceRow.GetHistoricalPrice(DB, drpQuoteVendor.SelectedValue, hdnProductID.Value, dtHistoryDate.Value)
            If drHistory Is Nothing Then
                ltlQuotePrice.Text = "<b class=""red smaller"">No Matching Prices Found</b>"
            Else
                ltlQuotePrice.Text = "<b class=""smaller"" style=""display:block;width:150px;border:1px solid #000;background-color:#fff;"">Price Was:<br/><span class=""largest"">" & FormatCurrency(drHistory("VendorPrice")) & "</span><br/>On " & FormatDateTime(dtHistoryDate.Value, DateFormat.ShortDate) & "</b>"
            End If
        End If

        ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenQuoteForm", "Sys.Application.add_load(OpenQuoteForm);", True)
    End Sub

    Protected Sub frmQuote_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmQuote.TemplateLoaded
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
        drpQuoteVendor.DataSource = VendorRow.GetListByLLC(DB, dbBuilder.LLCID, "CompanyName")
        drpQuoteVendor.DataTextField = "CompanyName"
        drpQuoteVendor.DataValueField = "VendorID"
        drpQuoteVendor.DataBind()
    End Sub

    Protected Sub rptPricing_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptPricing.ItemDataBound
        If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim ltlPricingSku As Literal = e.Item.FindControl("ltlPricingSku")
        Dim ltlPricingPrice As Literal = e.Item.FindControl("ltlPricingPrice")
        Dim ltlLastUpdate As Literal = e.Item.FindControl("ltlLastUpdate")
        Dim ltlSubstituteNotes As Literal = e.Item.FindControl("ltlSubstituteNotes")

        If IsDBNull(e.Item.DataItem("SubstitutePrice")) Then
            ltlPricingSku.Text = Core.GetString(e.Item.DataItem("VendorSku"))
            ltlPricingPrice.Text = FormatCurrency(Core.GetDouble(e.Item.DataItem("VendorPrice")))
            If Not IsDBNull(e.Item.DataItem("Updated")) Then
                ltlLastUpdate.Text = FormatDateTime(e.Item.DataItem("Updated"), DateFormat.ShortDate)
            ElseIf Not IsDBNull(e.Item.DataItem("LastUpdate")) Then
                ltlLastUpdate.Text = FormatDateTime(e.Item.DataItem("LastUpdate"), DateFormat.ShortDate)
            End If
            ltlSubstituteNotes.Text = String.Empty
        Else
            ltlPricingSku.Text = Core.GetString(e.Item.DataItem("SubstituteSku"))
            ltlPricingPrice.Text = FormatCurrency(Core.GetDouble(e.Item.DataItem("SubstitutePrice")))
            If Not IsDBNull(e.Item.DataItem("LastUpdate")) Then
                ltlLastUpdate.Text = FormatDateTime(e.Item.DataItem("LastUpdate"), DateFormat.ShortDate)
            End If
            If Not IsDBNull(e.Item.DataItem("SubstituteProduct")) Then
                ltlSubstituteNotes.Text = "<b class=""smaller"">* Substitute Product: " & e.Item.DataItem("SubstituteProduct") & "</b>"
            End If
        End If
    End Sub

    Protected Sub slPreferredVendor2_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles slPreferredVendor2.ValueChanged
        If slPreferredVendor2.Value Is Nothing Or slPreferredVendor2.Value = "" Then
            Session("CurrentPreferredVendor") = Nothing
        Else
            Session("CurrentPreferredVendor") = slPreferredVendor2.Value
            ctlSearch.CatalogType = controls_SearchSql.CATALOG_TYPE.CATALOG_TYPE_MARKET
        End If
    End Sub

    Protected Sub btnClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnClose.Click
        divNotification.Visible = False
    End Sub

End Class