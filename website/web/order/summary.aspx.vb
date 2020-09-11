Imports Components
Imports DataLayer
Imports TwoPrice.DataLayer
Imports System.IO

Partial Class order_summary
    Inherits SitePage

    Protected OrderID As Integer
    Private dbOrder As Object

    Private dvItems As DataView

    Private TaxRate As Double

    Private OrderSubTotal As Double
    Private OrderTotalItems As Integer
    Private OrderTotalQuantity As Integer
    Private OrderTaxTotal As Double

    Private DropSubTotal As Double
    Private DropTotalItems As Integer
    Private DropTotalQuantity As Integer
    Private DropTaxTotal As Double

    Private IsTwoPrice As Boolean

    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Protected ReadOnly Property PrintUrl() As String
        Get
            Dim Output As String = Request.ServerVariables("URL").ToString.Trim & "?"
            If Not Request.ServerVariables("QUERY_STRING") Is Nothing AndAlso Request.ServerVariables("QUERY_STRING").ToString.Trim <> String.Empty Then
                Dim TempArray As String() = Split(Request.ServerVariables("QUERY_STRING").ToString.Trim, "&")
                For Each Param As String In TempArray
                    Dim TempArray2 As String() = Split(Param.ToString.Trim, "=")
                    If UBound(TempArray2) = 1 Then
                        If TempArray2(0).ToString.Trim <> String.Empty AndAlso TempArray2(0).ToString.ToLower.Trim <> "print" Then
                            If Right(Output, 1) <> "?" Then Output &= "&"
                            Output &= TempArray2(0).ToString.Trim & "=" & TempArray2(1).ToString.Trim
                        End If
                    End If
                Next
                If Right(Output, 1) <> "?" Then Output &= "&"
            End If
            Output &= "print=y&" & GetPageParams(Components.FilterFieldType.All)
            Return Output
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("BuilderId") Is Nothing And Session("VendorId") Is Nothing Then
            Response.Redirect(GlobalSecureName & "/default.aspx")
        End If

        IsTwoPrice = Request("twoprice") = "y"

        OrderID = Request("OrderId")

        If IsTwoPrice Then
            dbOrder = TwoPriceOrderRow.GetRow(DB, OrderID)
            ltlCampaignName.Text = TwoPriceCampaignRow.GetRow(DB, dbOrder.TwoPriceCampaignId).Name
        Else
            trCampaignName.Visible = False
            dbOrder = OrderRow.GetRow(DB, OrderID)
        End If

        If dbOrder.OrderID = Nothing OrElse (dbOrder.VendorID <> Session("VendorId") And dbOrder.BuilderID <> Session("BuilderId")) Then
            Response.Redirect("default.aspx", True)
        End If

        If Not Request("print") Is Nothing AndAlso Request("print").ToString.Trim <> String.Empty Then
            pnlPrint.Visible = False
        End If

        If Not IsPostBack Then
            lnkDetails.NavigateUrl = "default.aspx?OrderId=" & OrderID & IIf(IsTwoPrice, "&twoprice=y", "")
            lnkDrops.NavigateUrl = "drops.aspx?OrderId=" & OrderID & IIf(IsTwoPrice, "&twoprice=y", "")

            LoadFromDB()
            BindData()
        End If
        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")
    End Sub

    Private Sub BindData()
        Dim dt As DataTable

        If IsTwoPrice Then
            dt = TwoPriceOrderRow.GetOrderProductsForDisplay(DB, OrderID, "SortOrder")
        Else
            dt = OrderRow.GetOrderProductsForDisplay(DB, OrderID, "SortOrder")
        End If

        Dim dbProject As ProjectRow = ProjectRow.GetRow(DB, dbOrder.ProjectID)

        'For Orders placed before manually editing taxrates
        ''If dbOrder.TaxRate <> 0 Then
        ''    TaxRate = dbOrder.Taxrate / 100
        ''Else
        'Dim client As New FastTax.DOTSFastTax()
        'Dim oRate As FastTax.TaxInfo = client.GetTaxInfo(80247, "sales", SysParam.GetValue(DB, "FastTaxLicenseKey"))
        'TaxRate = IIf(oRate Is Nothing, 0, oRate.TotalTaxRate)
        ' End If
        TaxRate = dbOrder.TaxRate / 100

        DropTotalItems = 0
        DropSubTotal = 0
        DropTotalQuantity = 0
        DropTaxTotal = 0

        AddHandler rptItems.ItemDataBound, AddressOf rptItems_ItemDataBound
        rptItems.DataSource = dt
        rptItems.DataBind()

        ltlTotalItems.Text = DropTotalItems
        ltlTotalQuantity.Text = DropTotalQuantity
        ltlTotalSubtotal.Text = FormatCurrency(DropSubTotal)
        ltlTotalTax.Text = FormatCurrency(DropTaxTotal)
        ltlTotalPrice.Text = FormatCurrency(DropSubTotal + DropTaxTotal, GroupDigits:=TriState.False)

        dvItems = dt.DefaultView
        dvItems.Sort = "DropID, SortOrder"

        If IsTwoPrice Then
            rptDrops.DataSource = TwoPriceOrderRow.GetOrderDrops(DB, OrderID, "RequestedDelivery")
        Else
            rptDrops.DataSource = OrderRow.GetOrderDrops(DB, OrderID, "RequestedDelivery")
        End If

        rptDrops.DataBind()
    End Sub

    Private Sub LoadFromDB()

        If OrderID = 0 Then
            Response.Redirect("default.aspx", True)
        End If

        If dbOrder.OrderID = Nothing OrElse (dbOrder.BuilderID <> Session("BuilderId") And dbOrder.VendorID <> Session("VendorId")) Then
            Response.Redirect("default.aspx")
        End If

        Dim dbStatus As OrderStatusRow = OrderStatusRow.GetRow(DB, OrderStatus.Unprocessed)
        If dbOrder.OrderStatusID = dbStatus.OrderStatusID Then
            btnSubmit.Text = "Submit Order to Vendor"
            lnkDetails.Visible = True
            lnkDrops.Visible = True
        Else
            btnSubmit.Text = "Update Order"
            lnkDetails.Visible = False
            lnkDrops.Visible = False
        End If

        ltlUpdated.Text = dbOrder.Updated
        ltlOrderNumber.Text = dbOrder.OrderNumber
        ltlTitle.Text = dbOrder.Title
        ltlPONumber.Text = dbOrder.PONumber
        ltlOrdererFirstName.Text = dbOrder.OrdererFirstName
        ltlOrdererLastName.Text = dbOrder.OrdererLastName
        ltlOrdererEmail.Text = dbOrder.OrdererEmail
        ltlOrdererPhone.Text = dbOrder.OrdererPhone
        ltlSuperFirstName.Text = dbOrder.SuperFirstName
        ltlSuperLastName.Text = dbOrder.SuperLastName
        ltlSuperEmail.Text = dbOrder.SuperEmail
        ltlSuperPhone.Text = dbOrder.SuperPhone
        ltlSubtotal.Text = FormatCurrency(dbOrder.Subtotal)
        ltlTax.Text = FormatCurrency(dbOrder.Tax)
        ltlTotal.Text = FormatCurrency(dbOrder.Total)
        ltlDeliveryInstructions.Text = dbOrder.DeliveryInstructions
        'ltlNotes.Text = dbOrder.Notes
        ltlRemoteIP.Text = dbOrder.RemoteIP
        txtBuilderNotes.Text = dbOrder.Notes
        txtVendorNotes.Text = dbOrder.VendorNotes
        If Session("BuilderId") Is Nothing Then
            txtBuilderNotes.Enabled = False
        ElseIf Session("VendorId") Is Nothing Then
            txtVendorNotes.Enabled = False
        End If

        Dim dbAccount As BuilderAccountRow = BuilderAccountRow.GetRow(DB, dbOrder.CreatorBuilderID)
        ltlCreatorBuilder.Text = dbAccount.Username & " (" & Core.BuildFullName(dbAccount.FirstName, "", dbAccount.LastName) & ")"
        ltlRequestedDelivery.Text = IIf(dbOrder.RequestedDelivery = Nothing, "See Drops Below", FormatDateTime(dbOrder.RequestedDelivery))

        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, dbOrder.VendorID)
        ltlVendor.Text = dbVendor.CompanyName

        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, dbOrder.BuilderID)
        ltlBuilder.Text = dbBuilder.CompanyName

        Dim dbProject As ProjectRow = ProjectRow.GetRow(DB, dbOrder.ProjectID)
        ltlProject.Text = dbProject.ProjectName
        ltlProjectAddress.Text = dbProject.Address & " " & dbProject.Address2
        ltlProjectAddress.Text &= "<br>" & dbProject.City & ", " & dbProject.State & " " & dbProject.Zip

        dbStatus = OrderStatusRow.GetRow(DB, dbOrder.OrderStatusID)
        ltlStatus.Text = dbStatus.OrderStatus
    End Sub

    Protected Sub rptDrops_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptDrops.ItemCreated
        If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim rptItems As Repeater = e.Item.FindControl("rptItems")
        AddHandler rptItems.ItemDataBound, AddressOf rptItems_ItemDataBound
    End Sub

    Protected Sub rptDrops_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptDrops.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If
        Dim ltlDropDetails As Literal = e.Item.FindControl("ltlDropDetails")
        Dim ltlDropTitle As Literal = e.Item.FindControl("ltlDropTitle")
        ltlDropTitle.Text = e.Item.DataItem("DropName")
        If Not IsDBNull(e.Item.DataItem("RequestedDelivery")) Then
            ltlDropDetails.Text &= "<b>Requested Delivery</b>&nbsp;&nbsp;-&nbsp;&nbsp;" & Server.HtmlEncode(e.Item.DataItem("RequestedDelivery"))
        End If
        If Not IsDBNull(e.Item.DataItem("Notes")) Then
            ltlDropDetails.Text &= "<p><b>Note:</b><br/>" & Server.HtmlEncode(e.Item.DataItem("Notes")) & "</p>"
        End If
        If Not IsDBNull(e.Item.DataItem("DeliveryInstructions")) Then
            ltlDropDetails.Text &= "<p><b>Delivery Instructions:</b><br/>" & Server.HtmlEncode(e.Item.DataItem("DeliveryInstructions")) & "</p>"
        End If

        DropTotalItems = 0
        DropSubTotal = 0
        DropTotalQuantity = 0
        DropTaxTotal = 0

        Dim rptItems As Repeater = e.Item.FindControl("rptItems")
        dvItems.RowFilter = "DropID=" & DB.Number(e.Item.DataItem("OrderDropID"))
        rptItems.DataSource = dvItems
        rptItems.DataBind()

        Dim ltlTotalItems As Literal = e.Item.FindControl("ltlTotalItems")
        Dim ltlTotalQuantity As Literal = e.Item.FindControl("ltlTotalQuantity")
        Dim ltlTotalPrice As Literal = e.Item.FindControl("ltlTotalPrice")
        Dim ltlSubtotal As Literal = e.Item.FindControl("ltlSubtotal")
        Dim ltlTax As Literal = e.Item.FindControl("ltlTax")

        ltlSubtotal.Text = FormatCurrency(DropSubTotal)
        ltlTax.Text = FormatCurrency(DropTaxTotal)
        ltlTotalItems.Text = DropTotalItems
        ltlTotalQuantity.Text = DropTotalQuantity
        ltlTotalPrice.Text = FormatCurrency(DropSubTotal + DropTaxTotal)

        If DropSubTotal = Nothing Then rptDrops.Visible = False
    End Sub

    Protected Sub rptItems_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim ltlVendorSku As Literal = e.Item.FindControl("ltlVendorSku")
        Dim ltlProduct As Literal = e.Item.FindControl("ltlProduct")
        Dim ltlUnitPrice As Literal = e.Item.FindControl("ltlUnitPrice")
        Dim ltlPrice As Literal = e.Item.FindControl("ltlPrice")
        Dim ltlcbusaSku As Literal = e.Item.FindControl("ltlcbusaSku")
        Dim lineTotal As Double = Core.GetDouble(e.Item.DataItem("Price")) * Core.GetInt(e.Item.DataItem("Quantity"))

        ltlVendorSku.Text = Core.GetString(e.Item.DataItem("ProductSku"))

        If Not IsDBNull(e.Item.DataItem("cbusasku")) Then
            ltlcbusaSku.Text = Core.GetString(e.Item.DataItem("cbusasku"))
        End If

        ltlProduct.Text = Server.HtmlEncode(Core.GetString(e.Item.DataItem("ProductName")))
        If IsDBNull(e.Item.DataItem("Price")) Then
            ltlUnitPrice.Text = "<b>*</b>"
        Else
            ltlUnitPrice.Text = FormatCurrency(e.Item.DataItem("Price"))
        End If

        ltlPrice.Text = FormatCurrency(lineTotal)

        DropTotalItems += 1
        DropTotalQuantity += Core.GetDouble(e.Item.DataItem("Quantity"))
        DropSubTotal += lineTotal
        DropTaxTotal += lineTotal * TaxRate
    End Sub

    'Protected Sub rptItems_ItemDataBound1(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptItems.ItemDataBound
    '    If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then
    '        Exit Sub
    '    End If

    '    OrderTotalItems += 1
    '    OrderTotalQuantity += Core.GetInt(e.Item.DataItem("Quantity"))
    '    OrderSubTotal += Core.GetDouble(e.Item.DataItem("Price")) * Core.GetInt(e.Item.DataItem("Quantity"))
    '    OrderTaxTotal += TaxRate * OrderSubTotal
    'End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        'update status here - need statuses from client

        If Session("BuilderId") IsNot Nothing Then dbOrder.Notes = txtBuilderNotes.Text
        If Session("VendorId") IsNot Nothing Then dbOrder.VendorNotes = txtVendorNotes.Text

        Dim dbUnprocessed As OrderStatusRow = OrderStatusRow.GetRow(DB, OrderStatus.Unprocessed)
        If dbOrder.OrderStatusID = dbUnprocessed.OrderStatusID Then
            Dim dbStatus As OrderStatusRow = OrderStatusRow.GetRow(DB, OrderStatus.Processing)
            dbOrder.OrderStatusID = dbStatus.OrderStatusID
            'TICKET_ID=206997 - we might need a third column for OrderPlaced or Processdate to determine when Order was placed.
            dbOrder.Created = Now
            dbOrder.Update()
            'log submit order
            Core.DataLog("Order", PageURL, CurrentUserId, "Submit Order", OrderID, "", "", "", UserName)
            'end log

            Dim dbHistory As New OrderStatusHistoryRow(DB)
            dbHistory.OrderID = dbOrder.OrderID
            dbHistory.OrderStatusID = dbOrder.OrderStatusID
            dbHistory.CreatorVendorAccountID = -1
            dbHistory.Insert()

            If Not IsTwoPrice Then
                Dim dbTakeoff As TakeOffRow = TakeOffRow.GetRow(DB, dbOrder.TakeoffID)
                If dbTakeoff.TakeOffID <> Nothing Then
                    dbTakeoff.Remove()
                End If
            End If

            Dim dbBuilderMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "OrderSent")
            Dim dbVendorMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "OrderReceived")
            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, dbOrder.BuilderID)
            Dim dbVendor As VendorRow = VendorRow.GetRow(DB, dbOrder.VendorID)
            Dim dbVendorAccount As VendorAccountRow = VendorAccountRow.GetRow(DB, dbOrder.SalesRepVendorAccountID)

            Dim sMsg As String = "Your Order " & dbOrder.Title & " was submitted to " & dbVendor.CompanyName & vbCrLf & vbCrLf & GlobalRefererName

            If dbBuilderMsg.CCList <> String.Empty Then
                dbBuilderMsg.CCList &= ","
            End If
            'TICKET_ID=328903 confirmation emails
            ' dbBuilderMsg.CCList &= dbOrder.SuperEmail & "," & dbOrder.OrdererEmail
            Try
                dbBuilderMsg.CCList &= IIf(dbBuilder.Email = dbOrder.SuperEmail, "", dbOrder.SuperEmail & ",") & IIf(dbBuilder.Email = dbOrder.OrdererEmail, "", dbOrder.OrdererEmail)
            Catch ex As Exception
                dbBuilderMsg.CCList = String.Empty
            End Try


            dbBuilderMsg.Send(dbBuilder, sMsg)

            sMsg = dbBuilder.CompanyName & " submitted the Order: " & dbOrder.Title & vbCrLf & vbCrLf & GlobalRefererName

            If dbVendorMsg.CCList <> String.Empty Then
                dbVendorMsg.CCList &= ","
            End If
            dbVendorMsg.CCList &= dbVendorAccount.Email
            dbVendorMsg.Send(dbVendor, sMsg)

            'log Mail Sent For Submit Order
            Core.DataLog("Order", PageURL, CurrentUserId, "Mail Sent For Submit Order", OrderID, "", "", "", UserName)
            'end log

            'LoadFromDB()
            'ltlStatusMsg.Text = "Your order has been submitted."
            'ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenSubmitted", "Sys.Application.add_load(OpenSubmitted);", True)
        Else

            'save any notes updates
            dbOrder.Update()

            'log submit order
            Core.DataLog("Order", PageURL, CurrentUserId, "Update Order", OrderID, "", "", "", UserName)
            'end log
            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
            Dim dbVendorMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "OrderBuilderUpdate")
            Dim dbVendor As VendorRow = VendorRow.GetRow(DB, dbOrder.VendorID)
            Dim dbvendoraccount As VendorAccountRow = VendorAccountRow.GetRow(DB, dbOrder.SalesRepVendorAccountID)
            Dim sMsg As String = dbBuilder.CompanyName & " updated the Order: " & dbOrder.Title & vbCrLf & vbCrLf & GlobalRefererName

            If dbVendorMsg.CCList <> String.Empty Then
                dbVendorMsg.CCList &= ","
            End If
            dbVendorMsg.CCList &= dbvendoraccount.Email
            dbVendorMsg.Send(dbVendor, sMsg)

            'log Mail Sent For Update Order
            Core.DataLog("Order", PageURL, CurrentUserId, "Mail Sent For Update Order", OrderID, "", "", "", UserName)
            'end log

            'ltlStatusMsg.Text = "Your order has been updated."
            'ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenSubmitted", "Sys.Application.add_load(OpenSubmitted);", True)
        End If

        If IsLoggedInVendor() Then
            Response.Redirect("/vendor/invoice/")
        Else
            Response.Redirect("history.aspx")
        End If

    End Sub



    Protected Sub btnExport_Click(sender As Object, e As System.EventArgs) Handles btnExport.Click

        If Not IsValid Then Exit Sub


        ExportList()


    End Sub
    Protected Sub btnExportMAS_Click(sender As Object, e As System.EventArgs) Handles btnExportMAS.Click

        If Not IsValid Then Exit Sub


        ExportListMASFormat()



    End Sub
    Public Sub ExportListMASFormat()
        Dim ProjectAddr As String = String.Empty
        Dim ProjectName As String = String.Empty
        Dim BuilderName As String = String.Empty
        Dim ProjectCity As String = String.Empty
        Dim ProjectState As String = String.Empty
        Dim ProjectZip As String = String.Empty
        Dim ShipLot As String = String.Empty
        Dim ShipBlock As String = String.Empty
        Dim ShipUnit As String = String.Empty
        Dim ShipDeliveryDate As String = String.Empty

        Dim dtexport As DataTable = Nothing
        If IsTwoPrice Then
            dtexport = TwoPriceOrderRow.GetOrderProductsForDisplay(DB, OrderID, "SortOrder")
        Else
            dtexport = OrderRow.GetOrderProductsForDisplay(DB, OrderID, "SortOrder")
        End If
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, dbOrder.BuilderID)
        BuilderName = dbBuilder.CompanyName

        Dim dbProject As ProjectRow = ProjectRow.GetRow(DB, dbOrder.ProjectID)
        ProjectName = dbProject.ProjectName
        ProjectAddr = dbProject.Address & " " & dbProject.Address2
        ProjectAddr &= " ," & dbProject.City & " , " & dbProject.State & " " & dbProject.Zip
        Dim taxratee As Double = dbOrder.TaxRate / 100

        Dim res As DataTable = dtexport
        Dim Folder As String = "/assets/catalogs/"
        Dim FileName As String = Core.GenerateFileID & ".txt"
        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)

        'Start with 
        'SHIP|10-12-0424|32-1571|ABC Farms-1|1234 Main Avenue|San Marcos|TX|78666|23|1490|1|03/14/2011
        'ITEM||2|Each|10' Structured Treated Post|64.130|128.26
        'ITEM|020709|10|Each|4x8-7/16 Text No Grv Soffit Hardboard Siding|19.254|192.54
        'ITEM|020715|70|Each|8X16' TX Lap Hardboard Siding (Siding)|6.654|465.78
        'NOTE|am
        'Start with SHIP data

        Dim ShipData As String = String.Empty
        Dim conn As String = "|"
        Dim PONumber As String = IIf(dbOrder.PONumber <> Nothing, dbOrder.PONumber, "")
        Dim JobNumber As String = String.Empty
        ProjectName = dbProject.ProjectName
        ProjectAddr = dbProject.Address & " " & dbProject.Address2
        ProjectCity = dbProject.City
        ProjectState = dbProject.State
        ProjectZip = dbProject.Zip

        ' sw.WriteLine("SHIP" & conn & PONumber & conn & JobNumber & conn & ProjectName & conn & ProjectAddr & conn & ProjectCity & conn & ProjectState & conn & ProjectZip & ShipLot & conn & ShipBlock & conn & ShipUnit & conn & ShipDeliveryDate)
        Dim sb As New StringBuilder()
        sb.Append("SHIP" & conn & PONumber & conn & JobNumber & conn & ProjectName & conn & ProjectAddr & conn & ProjectCity & conn & ProjectState & conn & ProjectZip & ShipLot & conn & ShipBlock & conn & ShipUnit & conn & ShipDeliveryDate & vbNewLine)
        ' sw.WriteLine(vbNewLine)

        If res.Rows.Count > 0 Then
            For Each row As DataRow In res.Rows
                Dim CbusaSku As String = String.Empty
                Dim VendorSku As String = String.Empty
                Dim Product As String = String.Empty
                Dim Quantity As String = String.Empty
                Dim UnitPRice As Double = 0
                Dim TotalPrice As Double = 0
                If Not IsDBNull(row("CbusaSku")) Then
                    CbusaSku = row("CbusaSku")
                Else
                    CbusaSku = ""
                End If

                If Not IsDBNull(row("VendorSku")) Then
                    VendorSku = row("VendorSku")
                Else
                    VendorSku = ""
                End If

                If Not IsDBNull(row("ProductName")) Then
                    Product = row("ProductName")
                Else
                    Product = ""
                End If
                If Not IsDBNull(row("Quantity")) Then
                    Quantity = row("Quantity")
                Else
                    Quantity = ""
                End If
                If Not IsDBNull(row("Price")) Then
                    UnitPRice = row("Price")
                Else
                    UnitPRice = ""
                End If

                TotalPrice = Core.GetDouble(row("Price")) * Core.GetInt(row("Quantity"))
                DropTotalItems += 1
                DropTotalQuantity += Core.GetInt(row("Quantity"))
                DropSubTotal += TotalPrice
                DropTaxTotal += TotalPrice * taxratee

                ' sw.WriteLine(Core.GetString(VendorSku) & conn & Core.GetString(CbusaSku) & conn & Core.GetString(Product) & conn & Core.GetString(Quantity) & conn & Core.GetString(UnitPRice) & conn & Core.GetString(TotalPrice))

                sb.Append("ITEM" & conn & Core.GetString(VendorSku) & conn & Core.GetString(Quantity) & conn & "Each" & conn & Core.GetString(Product) & conn & Core.GetString(Math.Round(UnitPRice, 3)) & conn & Core.GetString(Math.Round(TotalPrice, 3)) & vbNewLine)
            Next

            sb.Append("NOTE" & conn & dbOrder.DeliveryInstructions)



            sw.Flush()
            sw.Close()
            sw.Dispose()
            ' Response.Redirect(Folder & FileName)

            'log Export
            Core.DataLog("Order", PageURL, CurrentUserId, "ExportOrder MASFormat", OrderID, "", "", "", UserName)
            'end log

            Dim response As HttpResponse = HttpContext.Current.Response
            response.Clear()
            response.ContentType = "application/plain text"
            response.AddHeader("Content-Disposition", [String].Format("attachment; filename=""{0}""", FileName))
            response.Flush()
            response.Write(sb.ToString)
            response.End()
        End If
    End Sub

    Public Sub ExportList()
        Dim ProjectAddr As String = String.Empty
        Dim ProjectName As String = String.Empty
        Dim BuilderName As String = String.Empty

        Dim dtexport As DataTable = Nothing
        If IsTwoPrice Then
            dtexport = TwoPriceOrderRow.GetOrderProductsForDisplay(DB, OrderID, "SortOrder")
        Else
            dtexport = OrderRow.GetOrderProductsForDisplay(DB, OrderID, "SortOrder")
        End If
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, dbOrder.BuilderID)
        BuilderName = dbBuilder.CompanyName

        Dim dbProject As ProjectRow = ProjectRow.GetRow(DB, dbOrder.ProjectID)
        ProjectName = dbProject.ProjectName
        ProjectAddr = dbProject.Address & " " & dbProject.Address2
        ProjectAddr &= " ," & dbProject.City & " , " & dbProject.State & " " & dbProject.Zip
        Dim taxratee As Double = dbOrder.TaxRate / 100

        Dim res As DataTable = dtexport
        Dim Folder As String = "/assets/catalogs/"
        Dim FileName As String = Core.GenerateFileID & ".csv"
        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)

        sw.WriteLine(String.Empty)
        sw.WriteLine("Order Number :," & Core.QuoteCSV(dbOrder.OrderNumber))
        sw.WriteLine("Purchase Order:," & dbOrder.PONumber)
        sw.WriteLine("Builder :," & Core.QuoteCSV(BuilderName))
        sw.WriteLine("Project :," & Core.QuoteCSV(ProjectName))
        sw.WriteLine("Project Address :," & Core.QuoteCSV(ProjectAddr))
        sw.WriteLine(String.Empty)
        sw.WriteLine(String.Empty)

        sw.WriteLine("VendorSku , CbusaSku ,Product, Qty ,Unit Price , Total Price")
        If res.Rows.Count > 0 Then
            For Each row As DataRow In res.Rows
                Dim CbusaSku As String = String.Empty
                Dim VendorSku As String = String.Empty
                Dim Product As String = String.Empty
                Dim Quantity As String = String.Empty
                Dim UnitPRice As String = String.Empty
                Dim TotalPrice As Double = 0
                If Not IsDBNull(row("CbusaSku")) Then
                    CbusaSku = row("CbusaSku")
                Else
                    CbusaSku = ""
                End If

                If Not IsDBNull(row("VendorSku")) Then
                    VendorSku = row("VendorSku")
                Else
                    VendorSku = ""
                End If

                If Not IsDBNull(row("ProductName")) Then
                    Product = row("ProductName")
                Else
                    Product = ""
                End If
                If Not IsDBNull(row("Quantity")) Then
                    Quantity = row("Quantity")
                Else
                    Quantity = ""
                End If
                If Not IsDBNull(row("Price")) Then
                    UnitPRice = row("Price")
                Else
                    UnitPRice = ""
                End If

                TotalPrice = Core.GetDouble(row("Price")) * Core.GetInt(row("Quantity"))
                DropTotalItems += 1
                DropTotalQuantity += Core.GetInt(row("Quantity"))
                DropSubTotal += TotalPrice
                DropTaxTotal += TotalPrice * taxratee

                sw.WriteLine(Core.QuoteCSV(VendorSku) & "," & Core.QuoteCSV(CbusaSku) & "," & Core.QuoteCSV(Product) & "," & Core.QuoteCSV(Quantity) & "," & Core.QuoteCSV(UnitPRice) & "," & Core.QuoteCSV(TotalPrice))
            Next
            sw.WriteLine(String.Empty)
            sw.WriteLine(String.Empty)
            sw.WriteLine("Order Totals :,")
            sw.WriteLine("Total Items:," & Core.QuoteCSV(DropTotalItems))
            sw.WriteLine("Total Quantity:," & Core.QuoteCSV(DropTotalQuantity))

            sw.WriteLine("Order SubTotal :," & Core.QuoteCSV(FormatCurrency(DropSubTotal)))
            sw.WriteLine("Order Tax :," & Core.QuoteCSV(FormatCurrency(DropTaxTotal)))
            sw.WriteLine("Order Total  :," & Core.QuoteCSV(FormatCurrency(DropSubTotal + DropTaxTotal)))

            'log Export
            Core.DataLog("Order", PageURL, CurrentUserId, "ExportOrder", OrderID, "", "", "", UserName)
            'end log

            sw.Flush()
            sw.Close()
            sw.Dispose()
            Response.Redirect(Folder & FileName)
        End If
    End Sub

End Class
