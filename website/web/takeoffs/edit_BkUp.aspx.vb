Imports Components
Imports DataLayer
Imports IDevSearch
Imports System.Configuration.ConfigurationManager
Imports System.Linq
Imports System.Data
Imports NineRays.WebControls
Imports System.IO
Imports Controls
Imports System.Data.SqlClient
Imports TwoPrice.DataLayer
Partial Class takeoffs_edit
    Inherits SitePage
    Implements ICallbackEventHandler

    Dim dtPhases As DataTable

    Protected Property PhaseFilter() As String()
        Get
            Return ViewState("PhaseFilter")
        End Get
        Set(ByVal value As String())
            ViewState("PhaseFilter") = value
        End Set
    End Property

    Protected BuilderId As Integer
    Protected Property VendorId() As Integer
        Get
            Return ViewState("VendorId")
        End Get
        Set(ByVal value As Integer)
            ViewState("VendorId") = value
        End Set
    End Property
    Private TotalProducts As Integer
    Private TotalPrice As Double

    Private EventArgsToRegister As New Collections.Specialized.StringDictionary

    Private m_TakeoffProductIds As Generic.List(Of String)
    Private ReadOnly Property TakeoffProductIds() As Generic.List(Of String)
        Get
            If m_TakeoffProductIds Is Nothing Then
                m_TakeoffProductIds = New Generic.List(Of String)
            End If
            Return m_TakeoffProductIds
        End Get
    End Property

    Private AddedProductId As Integer

    Protected ReadOnly Property TakeoffId() As Integer
        Get
            If Not IsNumeric(Session("CurrentTakeoffId")) Then Session("CurrentTakeoffId") = Nothing
            Return Session("CurrentTakeoffId")
        End Get
    End Property

    Private dbTakeoff As TakeOffRow
    Protected ReadOnly Property Takeoff() As TakeOffRow
        Get
            If TakeoffId <> Nothing And (dbTakeoff Is Nothing OrElse dbTakeoff.TakeOffID <> TakeoffId) Then
                dbTakeoff = TakeOffRow.GetRow(DB, TakeoffId)
                Session("CurrentTakeoffId") = dbTakeoff.TakeOffID
            ElseIf dbTakeoff Is Nothing Then
                dbTakeoff = New TakeOffRow(DB)
                dbTakeoff.BuilderID = IIf(Session("TakeoffForId") IsNot Nothing, Session("TakeoffForId"), Session("BuilderId"))
                Session("CurrentTakeoffId") = dbTakeoff.Insert()
            End If
            Return dbTakeoff
        End Get
    End Property

    Private TreeFacetChanged As Boolean = False

    Private Property CachedTreeFilter() As Generic.List(Of String)
        Get
            Return ViewState("CachedTreeFilter")
        End Get
        Set(ByVal value As Generic.List(Of String))
            ViewState("CachedTreeFilter") = value
        End Set
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        ' Change Request #107449 - postback

        ' AddHandler btnSendTop.Click, AddressOf btnSend_Click
        'AddHandler btnSendBtm.Click, AddressOf btnSend_Click
        AddHandler btnAddAll.Click, AddressOf btnAddAll_Click
        AddHandler btnAddAll2.Click, AddressOf btnAddAll_Click
        AddHandler btnImport.Click, AddressOf btnImport_Click

        'Dim header As Controls.IHeaderControl = Page.FindControl("CTMain").FindControl("ctrlHeader")
        'AddHandler header.ControlEvent, AddressOf PreferredVendorSelected
        AddHandler slPreferredVendor2.ValueChanged, AddressOf PreferredVendorSelected
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsLoggedInBuilder() Then
            If IsLoggedInVendor() Or (TypeOf HttpContext.Current.User Is AdminPrincipal AndAlso CType(HttpContext.Current.User, AdminPrincipal).Username IsNot Nothing) Then
                If Session("TakeoffForId") Is Nothing Then
                    Response.Redirect("select-builder.aspx")
                End If
                aNewTakeoff.Visible = True
            Else
                Response.Redirect("/default.aspx")
            End If
        Else
            aNewTakeoff.Visible = False
        End If

        Takeoff.Update()

        slOrders.WhereClause = " BuilderId=" & DB.Number(Session("BuilderId"))
        slTakeoffs.WhereClause = " BuilderId=" & DB.Number(Session("BuilderId"))
        slProjects.WhereClause = " BuilderId=" & DB.Number(Session("BuilderId"))
        slTakeoffProject.WhereClause = "BuilderID=" & DB.Number(Session("BuilderId"))

        'Dim header As Controls.IHeaderControl = Page.FindControl("CTMain").FindControl("ctrlHeader")
        'AddHandler header.ControlEvent, AddressOf ChangeVendor
        'If header.ReturnValue <> Nothing Then
        '    VendorId = header.ReturnValue
        'ElseIf IsLoggedInVendor() Then
        '    VendorId = Session("VendorId")
        'End If

        AddHandler slPreferredVendor2.ValueChanged, AddressOf ChangeVendor
        If slPreferredVendor2.Value <> Nothing Then
            VendorId = slPreferredVendor2.Value
        ElseIf IsLoggedInVendor() Then
            VendorId = Session("VendorId")
        End If

        If Not IsPostBack Then
            If Request("TakeoffID") IsNot Nothing Then
                Session("CurrentTakeoffID") = Request("TakeoffID")
            End If

            qString = New URLParameters

            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, IIf(Session("TakeoffForId") IsNot Nothing, Session("TakeoffForId"), Session("BuilderId")))
            qString.Add("LLCFilterId", dbBuilder.LLCID)

            If Request("TakeoffId") IsNot Nothing Then
                Session("CurrentTakeoffId") = Request("TakeoffId")
            End If

            If Takeoff.BuilderID <> Session("BuilderId") And Takeoff.BuilderID <> Session("TakeoffForId") Then
                Response.Redirect("default.aspx")
            End If

            txtTakeoffTitle.Text = Takeoff.Title
            'AR: put this line outside if statment to fix isues when copying then updating quantity. Take off title used to revert back to previous one.  
            'lblTakeoffTitle.Text = Takeoff.Title

            If Takeoff.ProjectID <> Nothing Then
                Dim dbProject As ProjectRow = ProjectRow.GetRow(DB, Takeoff.ProjectID)
                slTakeoffProject.Value = dbProject.ProjectID
                slTakeoffProject.Text = dbProject.ProjectName
            ElseIf Request("ProjectID") IsNot Nothing Then
                Dim dbProject As ProjectRow = ProjectRow.GetRow(DB, Request("ProjectID"))
                slTakeoffProject.Value = dbProject.ProjectID
                slTakeoffProject.Text = dbProject.ProjectName
            End If

            If IsLoggedInBuilder() Then
                btnSendTop.Visible = False
                btnSendBtm.Visible = False
                btnSaveTop.Visible = True
                btnSaveBtm.Visible = True
                btnCopyTop.Visible = True
                btnCopyBtm.Visible = True
                divComparisonLinkBtm.Visible = True
                divComparisonLinkTop.Visible = True
                trspecialPrice.Visible = False
            Else
                labeltxtProductName.InnerHtml = "Product Name/Description"
                btnSaveTop.Visible = False
                btnSaveBtm.Visible = False
                btnSaveTop.Visible = False
                btnSaveBtm.Visible = False
                btnCopyBtm.Visible = False
                btnCopyTop.Visible = False
                divComparisonLinkBtm.Visible = False
                divComparisonLinkTop.Visible = False
                trspecialPrice.Visible = True
            End If

            BindTakeoff()
            Search()
        End If

        lblTakeoffTitle.Text = Takeoff.Title

        If VendorId = Nothing Then
            tdPriceHeader.Visible = False
            gvTakeoff.Columns(2).Visible = False
            'tdPrice.Visible = False
        Else
            tdPriceHeader.Visible = True
            gvTakeoff.Columns(2).Visible = False
            'tdPrice.Visible = True
        End If

        If Takeoff.Title <> Nothing Then
            divHead.InnerHtml = "Rename Take-off"
            btnSaveTop.Text = "Rename Take-off"
            btnSaveBtm.Text = "Rename Take-off"
        End If

        If Session("BuilderId") IsNot Nothing Then
            slPreferredVendor2.WhereClause = " VendorID in (select VendorID from LLCVendor l inner join Builder b on l.LLCID=b.LLCID where exists (select * from VendorProductPrice where VendorId=l.VendorId) and b.BuilderID=" & DB.Number(Session("BuilderID")) & ")"
            pnlPreferredVendor.Visible = True
        End If
    End Sub

    Protected Sub ChangeVendor(ByVal sender As Object, ByVal e As System.EventArgs)
        'Dim header As Controls.IHeaderControl = sender
        'VendorId = IIf(header.ReturnValue = String.Empty, Nothing, header.ReturnValue)
        VendorId = IIf(slPreferredVendor2.Value = String.Empty, Nothing, slPreferredVendor2.Value)
        Search()
        BindTakeoff()
        upFacets.Update()
        upProducts.Update()
    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        MyBase.Render(writer)
        For Each id As String In EventArgsToRegister.Keys
            Page.ClientScript.RegisterForEventValidation(id, EventArgsToRegister(id))
        Next
    End Sub

    'Protected Sub tvSupplyPhases_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tvSupplyPhases.SelectedIndexChanged
    '    'BindCrumbs()
    '    'BindProducts()
    '    'Dim qs As New URLParameters(qString.Items, "supplyphase;f;pg;guid;s")
    '    'Dim f As String = qString("f")
    '    'qs.Add("f", SearchIndex.ReplaceFacet(f, "supplyphase"))
    '    'qs.Add("supplyphase", tvSupplyPhases.CurrentNode.Value & "|" & tvSupplyPhases.CurrentNode.Name)
    '    'qString = qs
    '    'Search()
    '    'upFacets.Update()
    '    'upProducts.Update()
    'End Sub

    Private Sub BindTakeoff()
        TotalProducts = 0
        TotalPrice = 0
        'Dim dtTakeoff As DataTable = Takeoff.GetProductsList()
        Dim dtTakeoff As DataTable = TakeOffRow.GetTakeoffProducts(DB, Takeoff.TakeOffID)
        TakeoffProductIds.Clear()
        gvTakeoff.DataSource = dtTakeoff
        gvTakeoff.DataBind()
        ltlTotalProducts.Text = TotalProducts
        ltlTotalPrice.Text = FormatCurrency(TotalPrice)
        For Each id As String In TakeoffProductIds
            'Page.ClientScript.RegisterArrayDeclaration("TakeoffProductIds", id)
            ScriptManager.RegisterArrayDeclaration(Me, "TakeoffProductIds", id)
        Next
    End Sub

    Protected Sub rptProducts_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptProducts.ItemCommand
        Dim qty As TextBox = e.Item.FindControl("txtQty")

        If Not IsNumeric(qty.Text) Then
            qty.Text = 1
        End If

        If qty.Text <= 0 Then
            Exit Sub
        End If

        Takeoff.AddProduct(e.CommandArgument, qty.Text)
        BindTakeoff()

        qty.Text = String.Empty
        upTakeoff.Update()
    End Sub
    Protected Sub btnNew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNew.Click
        Session("CurrentTakeoffId") = Nothing
        Response.Redirect("edit.aspx")
    End Sub
    Protected Sub rptProducts_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProducts.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If
        If rptProducts.Items.Count > 0 Then
            Dim QtyTxt As TextBox = DirectCast(rptProducts.Items(0).FindControl("txtQty"), TextBox)
            QtyTxt.Focus()
        End If
        If VendorId = Nothing Then
            e.Item.FindControl("tdPrice").Visible = False
        Else
            CType(e.Item.FindControl("tdPrice"), HtmlTableCell).InnerHtml = FormatCurrency(e.Item.DataItem.VendorPrice)
        End If
    End Sub

    Protected Sub rptTakeoff_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvTakeoff.RowCommand
        If e.CommandName = "DeleteRecord" Then
            TakeOffProductRow.RemoveRow(DB, e.CommandArgument)
        End If
        BindTakeoff()
        upTakeoff.Update()
    End Sub

    Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        For Each item As GridViewRow In gvTakeoff.Rows
            Dim qty As TextBox = item.FindControl("txtQty")
            Dim btnDelete As ImageButton = item.FindControl("btnDelete")
            Dim dbTakeoffProduct As TakeOffProductRow = TakeOffProductRow.GetRow(DB, btnDelete.CommandArgument)
            If IsNumeric(qty.Text) Then
                If qty.Text = 0 Then
                    dbTakeoffProduct.Remove()
                Else
                    dbTakeoffProduct.Quantity = qty.Text
                    dbTakeoffProduct.Update()
                End If
            End If
        Next
        BindTakeoff()
        upTakeoff.Update()
    End Sub

    Private Sub gvList_DragAndDrop(ByVal sender As Object, ByVal e As DragAndDropEventArgs) Handles gvTakeoff.DragAndDrop
        Dim StartId As Integer = e.StartRowId
        Dim EndId As Integer = e.EndRowId

        Try
            DB.BeginTransaction()
            Core.ChangeSortOrderDragDrop(DB, "TakeOffProduct", "SortOrder", "TakeOffProductID", StartId, EndId, "TakeOffId = " & TakeoffId)
            DB.CommitTransaction()

            BindTakeoff()

        Catch ex As SqlException
            If Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub rptTakeoff_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvTakeoff.RowDataBound
        'If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
        '	Exit Sub
        'End If

        If e.Row.RowType <> DataControlRowType.DataRow Then
            Exit Sub
        End If

        If Not IsDBNull(e.Row.DataItem("ProductId")) AndAlso Not TakeoffProductIds.Contains(e.Row.DataItem("ProductId")) Then
            TakeoffProductIds.Add(e.Row.DataItem("ProductId"))
        End If

        TotalProducts += e.Row.DataItem("Quantity")

        Dim ltlPrice As Literal = e.Row.FindControl("ltlPrice")
        'wants same view for builder/vendor
        'Dim tdPrice As HtmlTableCell = e.Row.FindControl("tdPrice")
        ' If VendorId = Nothing OrElse (IsDBNull(e.Row.DataItem("VendorPrice")) Or IsDBNull(e.Row.DataItem("Quantity"))) Then
        'tdPrice.Visible = False
        'tdPrice.InnerHtml = "&nbsp;"
        ltlPrice.Text = "&nbsp;"
        'Else
        'TotalPrice += e.Row.DataItem("VendorPrice") * e.Row.DataItem("Quantity")
        ''tdPrice.InnerHtml = FormatCurrency(e.Row.DataItem("VendorPrice") * e.Row.DataItem("Quantity"))
        'ltlPrice.Text = FormatCurrency(e.Row.DataItem("VendorPrice") * e.Row.DataItem("Quantity"))
        'End If
    End Sub

    Protected Sub btnSaveSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveSubmit.Click
        If hdnIsCopy.Value <> Nothing AndAlso Convert.ToBoolean(hdnIsCopy.Value) Then
            Dim dbNewTakeoff As TakeOffRow = Takeoff.Copy(txtTakeoffTitle.Text)
            If dbNewTakeoff IsNot Nothing Then
                Session("CurrentTakeoffId") = dbNewTakeoff.TakeOffID
                dbTakeoff = dbNewTakeoff
            End If
        End If
        Takeoff.BuilderID = IIf(Session("TakeoffForId") IsNot Nothing, Session("TakeoffForId"), Session("BuilderId"))
        Takeoff.Title = txtTakeoffTitle.Text
        Takeoff.ProjectID = slTakeoffProject.Value
        Takeoff.Update()

        divHead.InnerHtml = "Rename Takeoff"
        lblSavedMsg.Text = "<b>TakeOff '" & Takeoff.Title & "' has been saved.</b><br/>"
        BindTakeoff()
    End Sub

    Protected Sub frmSendTakeoff_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmSendTakeoff.Postback
        Takeoff.BuilderID = Session("TakeoffForId")
        Takeoff.Title = txtSendTitle.Text
        Takeoff.Update()

        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Takeoff.BuilderID)
        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, Session("VendorId"))
        Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "TakeoffSent")
        Dim msg As String = "Takeoff Title:" & vbTab & vbTab & Takeoff.Title & vbCrLf & "Vendor:" & vbTab & vbTab & dbVendor.CompanyName & vbCrLf
        dbMsg.Send(dbBuilder, vbCrLf & msg)
    End Sub

    'Protected Sub drpVendor_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles drpVendor.SelectedIndexChanged
    '    VendorId = drpVendor.SelectedValue
    '    ctrlProductsFilter.VendorId = VendorId
    '    BindProducts()
    '    BindTakeoff()
    '    upProducts.Update()
    '    upTakeoff.Update()
    'End Sub

    Protected Sub btnAddSpecial_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddSpecial.Click
        Dim price As String = String.Empty
        If IsLoggedInVendor() Then
            price = Regex.Replace(txtSpecialPrice.Text, "[^\d.]", "")
            If price <> String.Empty Then
                If Double.Parse(price) <= 0 Then
                    AddError("Entered price is invalid.")
                    Exit Sub
                End If
            End If
        End If

        

        Dim dbSpecial As New SpecialOrderProductRow(DB)
        dbSpecial.SpecialOrderProduct = txtProductName.Text
        dbSpecial.BuilderID = IIf(Session("TakeoffForId") Is Nothing, Session("BuilderId"), Session("TakeoffForId"))
        dbSpecial.Description = txtDescription.Text
        dbSpecial.UnitOfMeasureID = acUnit.Value

        If dbSpecial.Description = Nothing Then
            dbSpecial.Description = "No Description Provided"
        End If

        dbSpecial.Insert()

        Dim dbProduct As New TakeOffProductRow(DB)
        dbProduct.TakeOffID = Takeoff.TakeOffID
        dbProduct.Quantity = txtSpecialQuantity.Text
        dbProduct.SpecialOrderProductID = dbSpecial.SpecialOrderProductID
        dbProduct.Insert()

        If IsLoggedInVendor() Then
            If price <> String.Empty Then
                If Double.Parse(price) > 0 Then
                    Dim dbPrice As VendorSpecialOrderProductPriceRow = VendorSpecialOrderProductPriceRow.GetRow(DB, Session("VendorId"), dbSpecial.SpecialOrderProductID)
                    dbPrice.IsSubstitution = False
                    dbPrice.SubmitterVendorAccountID = Session("VendorAccountId")
                    dbPrice.VendorID = Session("VendorId")
                    dbPrice.VendorPrice = Math.Round(Double.Parse(txtSpecialPrice.Text), 2)
                    If dbPrice.Submitted = Nothing Then
                        dbPrice.Insert()
                    Else
                        dbPrice.Update()
                    End If
                End If
            End If

        End If
        ScriptManager.RegisterStartupScript(Page, Me.GetType, "CloseSpecial", "CloseSpecialForm();", True)

        BindTakeoff()
    End Sub

    Protected Sub btnAddAll_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        For Each item As RepeaterItem In rptProducts.Items
            Dim qty As TextBox = item.FindControl("txtQty")
            Dim btn As Button = item.FindControl("btnAddProduct")
            If IsNumeric(qty.Text) AndAlso CInt(qty.Text) > 0 Then
                Takeoff.AddProduct(btn.CommandArgument, qty.Text)
            End If
        Next
        BindTakeoff()
        'BindProducts()
        Search()
        upProducts.Update()
        upTakeoff.Update()
    End Sub
    Protected Sub btnImport_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim FileInfo As System.IO.FileInfo
        Dim OriginalExtension As String
        Dim NewFileName As String

        If fulDocument.NewFileName <> String.Empty Then

            OriginalExtension = System.IO.Path.GetExtension(fulDocument.MyFile.FileName)

            If OriginalExtension <> ".csv" And OriginalExtension <> ".xls" Then
                ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
                ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""><li>Please enter a valid .csv or .xls file. Your file extension was: " & OriginalExtension & "</span></td></tr></table>"
                ltrErrorMsg.Visible = True
                Exit Sub
            End If

            fulDocument.Folder = "/assets/takeoff/"
            fulDocument.SaveNewFile()

            FileInfo = New System.IO.FileInfo(Server.MapPath(fulDocument.Folder & fulDocument.NewFileName))

            NewFileName = Core.GenerateFileID
            FileInfo.CopyTo(Server.MapPath(fulDocument.Folder & NewFileName & OriginalExtension))

            FileInfo.Delete()
        Else
            ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
            ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""><li>Please enter the file you want to upload</span></td></tr></table>"
            ltrErrorMsg.Visible = True
            Exit Sub
        End If

        Session("TakeoffImportFile") = NewFileName & OriginalExtension

        Try
            ImportTakeOff(Session("TakeoffImportFile"))
            BindTakeoff()
        Catch ex As Exception
            ltrErrorMsg.Text = Err.Description
            ltrErrorMsg.Visible = True
        End Try
    End Sub
    Protected Sub ImportTakeOff(ByVal FileName As String)
        Dim aLine As String()
        Dim Count As Integer = 0
        Dim BadCount As Integer = 0
        Dim txtErr As String = String.Empty
        Dim tblErr As String = String.Empty
        Dim ProductId As Integer = 0
        Dim SKU As String = String.Empty
        Dim Qty As String = String.Empty

        If Not File.Exists(Server.MapPath(fulDocument.Folder & FileName)) Then
            ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
            ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""></li>Cannot find the file to process</span></td></tr></table>"
            Exit Sub
        End If

        Dim f As StreamReader = New StreamReader(Server.MapPath(fulDocument.Folder & FileName))
        While Not f.EndOfStream
            Count = Count + 1

            Dim sLine As String = f.ReadLine()

            Dim bInside As Boolean = False
            For iLoop As Integer = 1 To Len(sLine)
                If Mid(sLine, iLoop, 1) = """" Then
                    If bInside = False Then
                        bInside = True
                    Else
                        bInside = False
                    End If
                End If
                If Mid(sLine, iLoop, 1) = "," Then
                    If Not bInside Then
                        sLine = Left(sLine, iLoop - 1) & "|" & Mid(sLine, iLoop + 1, Len(sLine) - iLoop)
                    End If
                End If
            Next

            aLine = sLine.Split("|")

            If aLine.Length >= 2 AndAlso aLine(0) <> String.Empty Then
                ProductId = 0
                SKU = Trim(Core.StripDblQuote(aLine(0)))
                Qty = Trim(Core.StripDblQuote(aLine(1)))
                If Count = 1 And SKU.ToUpper <> "CBUSA SKU" Then
                    ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
                    ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""><li>The file you uploaded does not appear to be in the correct format.</td></tr></table>"
                    Exit Sub
                End If

                If SKU.ToUpper = "CBUSA SKU" Then
                    Continue While
                End If

                txtErr = String.Empty
                'Quantity must be a numeric value and greater than 0
                If Qty <> String.Empty Then
                    If Not IsNumeric(Qty) Then
                        BadCount = BadCount + 1
                        txtErr &= "<li>Invalid quantity"
                        tblErr &= "<tr class=""red""><td>" & SKU & "</td><td>" & txtErr & "</td><tr>"
                        Continue While
                    ElseIf Qty <= 0 Then
                        BadCount = BadCount + 1
                        txtErr &= "<li>Non-Positive quantity"
                        tblErr &= "<tr class=""red""><td>" & SKU & "</td><td>" & txtErr & "</td><tr>"
                        Continue While
                    End If
                End If

                If txtErr = String.Empty Then
                    Dim SQL As String = "SELECT Top 1 p.ProductID"
                    SQL &= " FROM Product p "
                    SQL &= " Where "
                    SQL &= " p.SKU = " & DB.Quote(SKU)

                    Dim dtProduct As DataTable = DB.GetDataTable(SQL)

                    'Product must exist
                    If dtProduct.Rows.Count = 0 Then
                        BadCount = BadCount + 1
                        txtErr &= "<li>Product Not Found!"
                        tblErr &= "<tr class=""red""><td>" & SKU & "</td><td>" & txtErr & "</td><tr>"
                        Continue While
                    Else
                        'Add the takeoff to 
                        For Each dr As DataRow In dtProduct.Rows
                            ProductId = dr("ProductId")
                            Takeoff.AddProduct(ProductId, Qty)
                        Next
                    End If
                End If
            Else
                BadCount = BadCount + 1
                tblErr &= "<tr class=""red""><td>Row: " & Count & "</td><td>BAD ROW</td><tr>"
                Continue While
            End If
        End While
    End Sub

    Protected Sub frmAddProduct_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmAddProduct.Postback
        Dim btn As Button = sender
        Dim form As PopupForm.PopupForm = btn.NamingContainer
        Dim hdnRptId As HiddenField = form.FindControl("hdnRptItemId")
        Dim rptItem As RepeaterItem = Page.FindControl(hdnRptId.Value)

        Dim btnAdd As Button = rptItem.FindControl("btnAddProduct")
        Dim qty As TextBox = rptItem.FindControl("txtQty")
        If Not IsNumeric(qty.Text) Then
            qty.Text = 1
        End If
        If btnAdd.CommandArgument = Nothing OrElse Convert.ToInt32(qty.Text) <= 0 Then
            Exit Sub
        End If

        If Request.Form(form.UniqueID & "$LineItems") IsNot Nothing Then
            If Request.Form(form.UniqueID & "$LineItems") = "New" Then
                Takeoff.AddProduct(btnAdd.CommandArgument, qty.Text)
            Else
                Dim TakeoffProductId As Integer = Request.Form(form.UniqueID & "$LineItems")
                Dim dbProduct As TakeOffProductRow = TakeOffProductRow.GetRow(DB, TakeoffProductId)
                dbProduct.Quantity += qty.Text
                dbProduct.Update()
            End If
        End If

        BindTakeoff()
        'BindProducts()
        Search()
        upProducts.Update()
        upTakeoff.Update()
    End Sub

    Public Function GetCallbackResult() As String Implements System.Web.UI.ICallbackEventHandler.GetCallbackResult
        Dim dtItems As DataTable = Takeoff.GetProductLineItems(DB, TakeoffId, AddedProductId)
        Dim rb As RadioButton
        phLineItems.Controls.Clear()
        For Each row As DataRow In dtItems.Rows
            rb = New RadioButton
            rb.ID = row("TakeoffProductId")
            rb.Text = row("Sku") & " - " & row("Product") & " (" & row("Quantity") & ")"
            rb.GroupName = "LineItems"
            phLineItems.Controls.Add(rb)
            phLineItems.Controls.Add(New LiteralControl("<br />"))
        Next
        rb = New RadioButton
        rb.ID = "New"
        rb.Text = "Add New Line Item"
        rb.GroupName = "LineItems"
        rb.Checked = True
        phLineItems.Controls.Add(rb)

        Return frmAddProduct.GetControlHtml(phLineItems)
    End Function

    Public Sub RaiseCallbackEvent(ByVal eventArgument As String) Implements System.Web.UI.ICallbackEventHandler.RaiseCallbackEvent
        AddedProductId = eventArgument
    End Sub

#Region "Products To Compare"
    Private dtProducts As DataTable = Nothing
    Public ReadOnly Property CompareProducts() As DataTable
        Get
            If dtProducts Is Nothing Then
                If slTakeoffs.Value <> Nothing Then
                    dtProducts = TakeOffRow.GetTakeoffProducts(DB, slTakeoffs.Value)
                ElseIf slOrders.Value <> Nothing Then
                    Dim aOrders() As String = slOrders.Value.Split("-")
                    Dim SelectedOrderID As Integer = Core.GetInt(aOrders(0))
                    Dim TwoPriceCampaignOrderID As Integer = Core.GetInt(aOrders(1))
                    If TwoPriceCampaignOrderID > 0 Then
                        dtProducts = TwoPriceOrderRow.GetOrderProducts(DB, SelectedOrderID)
                    Else
                        dtProducts = OrderRow.GetOrderProducts(DB, SelectedOrderID)
                    End If
                ElseIf slProjects.Value <> Nothing Then
                    dtProducts = ProjectRow.GetProjectProducts(DB, slProjects.Value)
                End If
            End If
            Return dtProducts
        End Get
    End Property

    'Protected Sub acOrders_ValueUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles acOrders.ValueUpdated
    'End Sub

    'Protected Sub acProjects_ValueUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles acProjects.ValueUpdated
    '    acOrders.WhereClause = " BuilderId=" & DB.Number(BuilderId) & " and ProjectId=" & DB.Number(acProjects.Value)
    '    acTakeoffs.WhereClause = " BuilderId=" & DB.Number(BuilderId) & " and ProjectId=" & DB.Number(acProjects.Value)
    '    'upFilterBar.Update()
    'End Sub

    'Protected Sub acTakeoffs_ValueUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles acTakeoffs.ValueUpdated
    'End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnAdd.Click
        Dim dt As DataTable = CompareProducts
        If dt IsNot Nothing Then
            For Each row As DataRow In dt.Rows
                Takeoff.AddProduct(row("ProductId"), row("Quantity"))
            Next
            BindTakeoff()

            slProjects.Text = ""
            slTakeoffs.Text = ""
            slOrders.Text = ""
            'upFilterBar.Update()
        End If

    End Sub
#End Region


#Region "IDev"
    Private guid As String
    Private TrackingId As Integer

    Protected Property qString() As URLParameters
        Get
            If Session("SearchQueryString") Is Nothing Then
                Session("SearchQueryString") = New URLParameters(Request.QueryString)
            End If
            Return Session("SearchQueryString")
        End Get
        Set(ByVal value As URLParameters)
            Session("SearchQueryString") = value
        End Set
    End Property

    Protected Const RepeatColumns As Integer = 4

    Private Function RemovePipe(ByVal s As String) As String
        Dim pipe As String = InStrRev(s, "|")
        Return Right(s, Len(s) - pipe)
    End Function

    Private Sub Add2BreadCrumb(ByVal Text As String, ByVal link As String)
        If Not ltlBreadcrumb.Text = String.Empty Then
            ltlBreadcrumb.Text &= " <span>&gt;</span> "
        End If
        ltlBreadcrumb.Text &= "<a href=""" & link & """>" & Text & "</a>"
    End Sub

    Private Sub Add2BreadCrumb(ByVal Text As String)
        If Not ltlBreadcrumb.Text = String.Empty Then
            ltlBreadcrumb.Text &= " <span>&gt;</span> "
        End If
        ltlBreadcrumb.Text &= Text
    End Sub

    Private Function GetBreadcrumb(ByVal Text As String, ByVal Facet As String, ByVal qs As URLParameters) As String
        Return GetBreadcrumb(Text, Facet, qs, String.Empty)
    End Function

    Private Function GetBreadcrumb(ByVal Text As String, ByVal Facet As String, ByVal qs As URLParameters, ByVal Children As String) As String
        ltlBreadcrumb.Text = Nothing
        'Remove facets
        Dim f As String = qString("f")
        Dim ChildrenArray() As String = Children.Split(",")
        For Each childfacet As String In ChildrenArray
            If Not childfacet = String.Empty Then
                qs.Remove(childfacet)
                f = SearchIndex.RemoveFacet(f, childfacet)
            End If
        Next
        f = SearchIndex.RemoveFacet(f, Facet)
        'Add (x) to remove facet
        Dim xlnk As String = "<a href=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, qs.ToString("f", f)) & """><sup>(x)</sup></a>"

        'Remove facets
        Dim facets As String = RemoveFacets(Facet, qs)
        qs.Add("f", facets)
        qs.Add(Facet, qString(Facet))

        Text = RemovePipe(Text)
        Return "<a href=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, qs.ToString) & """>" & Text & "</a>" & xlnk
    End Function

    Private Function RemoveFacets(ByVal facet As String, ByRef qs As URLParameters) As String
        If qString("f") = String.Empty Then
            Return String.Empty
        End If
        Dim Facets() As String = qString("f").ToLower.Split(",")
        Dim j As Integer = Facets.Length
        Dim Result As String = String.Empty
        Dim Conn As String = String.Empty
        facet = facet.ToLower
        For i As Integer = 0 To Facets.Length - 1
            If Facets(i) = facet Then
                j = i
            End If
            If i > j Then
                qs.Remove(Facets(i))
                Facets(i) = String.Empty
            End If
            If Not Facets(i) = String.Empty Then
                Result &= Conn & Facets(i)
                Conn = ","
            End If
        Next
        Return Result
    End Function

    Private Sub AddFacet(ByRef bc As Hashtable, ByRef facets As GenericCollection(Of Facet), ByRef facetcache As GenericCollection(Of Facet), ByVal name As String, ByVal field As String, ByVal param As String)
        Dim f As New Facet
        f.Name = name
        f.Field = field
        f.MaxDocs = 0
        f.Narrow = qString(param)
        f.ZerosIncluded = False
        facets.Add(f)
        facetcache.Add(f)
        If Not qString(param) = String.Empty Then
            Dim qs As New URLParameters(qString.Items, param & ";f;pg")
            bc(param) = GetBreadcrumb(qString(param), param, qs, "")
        End If
    End Sub

    Protected Sub GetFilterList(ByRef list As Generic.List(Of String))
        list.AddRange(LLCRow.GetPricedProductsList(DB, qString("LLCFilterId")))
    End Sub

    Protected Sub GetVendorFilterList(ByRef list As Generic.List(Of String))
        list.AddRange(From v In VendorProductPriceRow.GetAllVendorPrices(DB, VendorId).AsEnumerable Select Core.GetString(v("ProductID")))
    End Sub

    Private Sub Search()
        ltlBreadcrumb.Text = Nothing
        txtKeyword.Text = Keywords

        Dim bc As New Hashtable
        guid = qString("guid")
        If guid = String.Empty Then guid = Core.GenerateFileID

        Dim index As New SearchIndex
        index.Directory = AppSettings("SearchIndexDirectory")
        index.IndexName = AppSettings("SearchIndexName")
        If qString("operator") = "OR" Then
            index.DefaultOperator = "OR"
        End If

        Dim facets As New GenericCollection(Of Facet)
        Dim facetcache As New GenericCollection(Of Facet)
        Dim query As New IndexQuery
        query.Keywords = Keywords
        query.MaxDocs = 10000
        query.Facets = facets
        query.FacetCache = facetcache
        query.SortBy = String.Empty
        query.SortReverse = False
        query.BestFragmentField = "Description"
        query.PageNo = IIf(qString("pg") = String.Empty, 1, qString("pg"))
        query.MaxPerPage = ctlNavigate.MaxPerPage
        query.ForceRefresh = Not IsPostBack
        query.FacetCacheDuration = 60 * 10 'ten minutes
        query.MaxHitsForCache = 100

        Dim aKeywords As String() = query.Keywords.Split(" ")
        If aKeywords.Length = 1 AndAlso aKeywords(0) <> "" Then
            query.Keywords = "*" & query.Keywords & "*"
        End If

        If VendorId <> Nothing Then
            'query.FilterCacheKey = VendorId
            'query.FilterListCallback = AddressOf GetVendorFilterList
            Dim list As New Generic.List(Of String)
            GetVendorFilterList(list)
            query.FilterList = list
        ElseIf qString("LLCFilterId") <> Nothing Then
            'query.FilterCacheKey = qString("LLCFilterId")
            'query.FilterListCallback = AddressOf GetFilterList
            Dim list As New Generic.List(Of String)
            GetFilterList(list)
            query.FilterList = list
        End If

        'Dim sort As String = qString("sort")
        'Select Case sort
        '    Case "priceasc"
        '        query.SortBy = "SalePrice"
        '        query.SortReverse = False
        '    Case "pricedesc"
        '        query.SortBy = "SalePrice"
        '        query.SortReverse = True
        '    Case "score", ""
        '        query.SortBy = String.Empty
        '        query.SortReverse = False

        '    Case Else
        '        query.SortBy = "ProductName"
        '        query.SortReverse = False
        'End Select
        query.SortBy = String.Empty
        query.SortReverse = False

        'AddFacet(bc, facets, facetcache, "Brand", "Brand", "brand")
        'AddFacet(bc, facets, facetcache, "SupplyPhase", "SupplyPhase", "supplyphase")
        AddFacet(bc, facets, facetcache, "ProductType", "ProductType", "producttype")
        AddFacet(bc, facets, facetcache, "Manufacturer", "Manufacturer", "manufacturer")
        AddFacet(bc, facets, facetcache, "UnitOfMeasure", "UnitOfMeasure", "unitofmeasure")

        Dim phase As New Facet
        phase.Name = "SupplyPhase"
        phase.Field = "SupplyPhase"
        phase.MaxDocs = 0
        phase.Narrow = IIf(qString("supplyphase") = Nothing, "ROOT", Server.HtmlDecode(Server.UrlDecode(qString("supplyphase"))))
        'phase.Narrow = Nothing
        phase.ZerosIncluded = False
        facets.Add(phase)
        facetcache.Add(phase)
        If Not qString("supplyphase") = String.Empty Then
            Dim qs As New URLParameters(qString.Items, "supplyphase" & ";f;pg")
            bc("supplyphase") = GetBreadcrumb(Server.UrlDecode(qString("supplyphase")), "supplyphase", qs, "")
        End If

        Dim start As DateTime = Now
        Dim sr As SearchResult = Nothing
        sr = index.Search(query)

        'If more than one word and no results, then redirect to search
        'page with OR operator
        If Not Keywords = String.Empty Then
            Dim Words() As String = Keywords.Split(" "c)
            If Words.Length > 1 Then
                Dim qs As New URLParameters(qString.Items, "operator;pg;guid;s")
                qs.Add("guid", guid)
                If sr.Count = 0 AndAlso qString("operator") <> "OR" Then
                    qString.Remove("operator")
                    qString.Add("operator", "OR")
                    Search()
                    Exit Sub
                End If
            End If
            txtNoResults.Visible = Not (sr.Count > 0)
            'trNarrow.Visible = (sr.Count > 0)
            txtOperatorOR.Visible = (qString("operator") = "OR") AndAlso (sr.Count > 0) AndAlso qString("expand") = String.Empty
        End If

        If Not qString("producttype") = String.Empty AndAlso sr.ds.Tables("producttype").Rows.Count > 0 Then
            AddAllRow(sr.ds.Tables("producttype"))
        End If
        rptProductType.DataSource = sr.ds.Tables("producttype")
        rptProductType.Visible = rptProductType.DataSource.Rows.Count > 1
        rptProductType.DataBind()

        If Not qString("manufacturer") = String.Empty AndAlso sr.ds.Tables("manufacturer").Rows.Count > 0 Then
            AddAllRow(sr.ds.Tables("manufacturer"))
        End If
        rptManufacturer.DataSource = sr.ds.Tables("manufacturer")
        rptManufacturer.Visible = rptManufacturer.DataSource.Rows.Count > 1
        rptManufacturer.DataBind()

        If Not qString("unitofmeasure") = String.Empty AndAlso sr.ds.Tables("unitofmeasure").Rows.Count > 0 Then
            AddAllRow(sr.ds.Tables("unitofmeasure"))
        End If
        rptUnitOfMeasure.DataSource = sr.ds.Tables("unitofmeasure")
        rptUnitOfMeasure.Visible = rptUnitOfMeasure.DataSource.Rows.Count > 1
        rptUnitOfMeasure.DataBind()

        If query.Keywords = String.Empty Then
            'tvSupplyPhases.UseFilter = (query.FilterListCallback IsNot Nothing)
            'tvSupplyPhases.UseFilter = (qString("LLCFilterId") IsNot Nothing)
        Else
            'tvSupplyPhases.UseFilter = True
        End If
        'If tvSupplyPhases.UseFilter Or Not (TreeFacetChanged And qString("SupplyPhase") <> Nothing) Then

        'RAY
        'Dim phaseFacet As Facet = (From f As Facet In query.Facets Where f.Name = "SupplyPhase" Select f).FirstOrDefault

        'phaseFacet.Narrow = "ROOT"
        'Dim treeSR As SearchResult = index.Search(query)

        'tvSupplyPhases.FilterList.Clear()
        'tvSupplyPhases.FilterList.AddRange(From row As DataRow In sr.ds.Tables("SupplyPhase").AsEnumerable Select Convert.ToString(row("Value")))
        Dim filter As String = String.Empty
        Dim conn As String = String.Empty
        For Each row As DataRow In sr.ds.Tables("SupplyPhase").Rows
            Dim str As String = Core.GetString(row("Value"))
            If str.ToLower <> "root" Then
                Dim split As Integer = str.LastIndexOf("|")
                filter &= conn & str.Substring(0, split)
                conn = "|"
            End If
        Next
        PhaseFilter = Nothing
        If Not filter = String.Empty Then PhaseFilter = filter.Split("|")
        'flyTreeView.Nodes.Clear()
        TreeViewAddNodes(flyTreeView.Nodes, 0, flyTreeView.SelectedNode, PhaseFilter)

        'End If
        'tvSupplyPhases.DataBind()

        If sr.Count = 0 Then
            ctlNavigate.Visible = False
        Else
            ctlNavigate.Visible = True
            ctlNavigate.NofRecords = sr.Count
            ctlNavigate.PageNumber = IIf(qString("pg") = Nothing, 1, qString("pg"))
            ctlNavigate.DataBind()
        End If

        'Display breadcrumb trail
        Add2BreadCrumb("Clear All", Page.ClientScript.GetPostBackClientHyperlink(btnSearch, "clear"))
        If Not Keywords = String.Empty Then
            Add2BreadCrumb("<a href=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, "keywords=" & Keywords) & """ /><b>" & Keywords & "</b><a href=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, "clear") & """><sup>x</sup></a>")
        End If
        If Not qString("f") = String.Empty Then
            Dim f() As String = qString("f").Split(",")
            For j As Integer = 0 To f.Length - 1
                Add2BreadCrumb(bc(LCase(f(j))))
            Next
        End If

        'Generate user guid for current session
        If Session("UserGuid") Is Nothing Then
            Session("UserGuid") = Core.GenerateFileID
        End If

        If VendorId <> Nothing Then
            Dim dtPrices As DataTable = VendorProductPriceRow.GetAllVendorPrices(DB, VendorId)
            rptProducts.DataSource = From r As DataRow In sr.ds.Tables(0).AsEnumerable Group Join p As DataRow In dtPrices.Rows On Core.GetInt(r("ProductID")) Equals Core.GetInt(p("ProductID")) Into grp = Group From item In grp.Select(Of String)(Function(dr, idx) Core.GetString(dr("VendorPrice"))) Select New With {.ProductID = r("ProductID"), .ProductName = r("ProductName"), .Sku = r("Sku"), .VendorPrice = item}
            rptProducts.DataBind()
        Else
            rptProducts.DataSource = sr.ds.Tables(0)
            rptProducts.DataBind()
        End If
        BindLLCFilterLinks()
    End Sub

    Private Sub BindLLCFilterLinks()


        If qString("LLCFilterId") = Nothing Then
            'ltlAllProducts.Text = "<b>All Products in Catalog</b>"
            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, IIf(Session("BuilderId") Is Nothing, Session("TakeoffForId"), Session("BuilderId")))
            Dim args As String = qString.ToString("LLCFilterId", dbBuilder.LLCID)


            ltlLLCOnly.Text = "<input type=""radio""   name=""llconly"" onclick=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, args) & """>All Products Priced In This Market</input>"
            ltlAllProducts.Text = "<input type=""radio"" checked=""checked""  name=""allproducts"" onclick=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, args) & """><b>All Products in Catalog</b></input>"
            'EventArgsToRegister.Add(btnsearch.UniqueId, args)
        Else
            Dim qs = New URLParameters(qString.Items, "LLCFilterId")
            Dim args As String = qs.ToString()
            ltlAllProducts.Text = "<input type=""radio""  name=""allproducts"" onclick=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, args) & """>All Products in Catalog</input>"
            ltlLLCOnly.Text = "<input type=""radio"" checked=""checked""  name=""llconly"" onclick=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, args) & """><b>All Products Priced In This Market</b></input>"
            'ltlAllProducts.Text = "<a href=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, args) & """>All Products in Catalog</a>"
            'ltlLLCOnly.Text = "<b>All Products Priced In This Market</b>"
            'EventArgsToRegister.Add(btnSearch.UniqueId, args)
        End If
    End Sub

    Protected ReadOnly Property Keywords() As String
        Get
            Dim result As String = IIf(txtKeyword.Text = "Keyword Search", Nothing, txtKeyword.Text)
            If result Is Nothing OrElse result = String.Empty Then result = Request("Keywords")
            If result Is Nothing Then result = String.Empty

            Return result
        End Get
    End Property

    Private Function GetLabelOrValueText(ByVal field As String, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs, ByVal Skip As String, ByVal fieldvalue As String) As String
        Dim qs As New URLParameters(qString.Items, field & ";" & Skip & ";f;pg;guid;s")
        qs.Add("guid", guid)
        Dim CountText As String = String.Empty
        Dim SkipArray() As String = (Skip & "").Split(";")
        If e.Item.DataItem("count") >= 0 Then
            CountText = "(" & e.Item.DataItem("count") & ")"
        End If
        Dim f As String = Request("f")
        For Each item As String In SkipArray
            If Not item = String.Empty Then f = SearchIndex.RemoveFacet(f, item)
        Next
        If e.Item.DataItem("value") = String.Empty Then
            qs.Add("f", SearchIndex.RemoveFacet(f, field))
        Else
            qs.Add("f", SearchIndex.ReplaceFacet(f, field))
        End If
        If qString(field) = e.Item.DataItem("value") Then
            Dim value As String = RemovePipe(e.Item.DataItem(fieldvalue))
            Return value & " " & CountText
        ElseIf e.Item.DataItem("count") = -1 Then
            Return "<a href=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, qs.ToString(field, e.Item.DataItem("value"))) & """>" & e.Item.DataItem("label") & "</a> " & CountText
        Else
            Dim value As String = RemovePipe(e.Item.DataItem(fieldvalue))
            Return "<a href=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, qs.ToString(field, e.Item.DataItem("value"))) & """>" & value & "</a> " & CountText
        End If
    End Function

    Private Function GetLabelText(ByVal field As String, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs, ByVal skip As String) As String
        Return GetLabelOrValueText(field, e, skip, "label")
    End Function

    Private Function GetLabelText(ByVal field As String, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) As String
        Return GetLabelOrValueText(field, e, String.Empty, "label")
    End Function

    Private Function GetValueText(ByVal field As String, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs, ByVal skip As String) As String
        Return GetLabelOrValueText(field, e, skip, "value")
    End Function

    Private Function GetValueText(ByVal field As String, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) As String
        Return GetLabelOrValueText(field, e, String.Empty, "value")
    End Function

    Private Sub AddAllRow(ByRef dt As DataTable)
        Dim row As DataRow = dt.NewRow()
        row("Name") = "All"
        row("Label") = "All"
        row("Value") = String.Empty
        row("Count") = -1
        dt.Rows.InsertAt(row, 0)
    End Sub

    Protected Sub btnSearch_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles btnSearch.Command
        Dim arg As String = IIf(e.CommandArgument = String.Empty, Request("__EVENTARGUMENT"), e.CommandArgument)
        If arg <> Nothing Then
            If arg.ToLower = "clear" Then
                qString = New URLParameters
                txtKeyword.Text = String.Empty
            ElseIf Left(arg, 9) = "keywords=" Then
                qString = New URLParameters()
                qString.Add("keywords", Right(arg, arg.Length - 9))
            Else
                qString = ParamsFromString(arg)
            End If
        Else
            qString = New URLParameters()
            qString.Add("keywords", txtKeyword.Text)
        End If
        Search()
        'upProducts.Update()
        'upFacets.Update()
    End Sub

    Protected Sub rptManufacturer_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptManufacturer.ItemDataBound
        If Not e.Item.ItemType = ListItemType.AlternatingItem AndAlso Not e.Item.ItemType = ListItemType.Item Then
            Exit Sub
        End If
        Dim Label As Literal = e.Item.FindControl("ltlLabel")
        Label.Text = GetLabelText("manufacturer", e, "")
    End Sub

    Protected Sub rptProductType_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProductType.ItemDataBound
        If Not e.Item.ItemType = ListItemType.AlternatingItem AndAlso Not e.Item.ItemType = ListItemType.Item Then
            Exit Sub
        End If
        Dim Label As Literal = e.Item.FindControl("ltlLabel")
        Label.Text = GetLabelText("producttype", e, "")
    End Sub

    Protected Sub rptUnitOfMeasure_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptUnitOfMeasure.ItemDataBound
        If Not e.Item.ItemType = ListItemType.AlternatingItem AndAlso Not e.Item.ItemType = ListItemType.Item Then
            Exit Sub
        End If
        Dim Label As Literal = e.Item.FindControl("ltlLabel")
        Label.Text = GetLabelText("unitofmeasure", e, "")
    End Sub
#End Region

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        ShowHideNodes(flyTreeView.Nodes, PhaseFilter)
        If qString IsNot Nothing AndAlso qString.Items.Count > 0 Then
            ctlHistory.AddEntry(qString.ToString)
        End If
        'BindTakeoff()
    End Sub

    Protected Sub ctlHistory_Navigate(ByVal sender As Object, ByVal e As nStuff.UpdateControls.HistoryEventArgs) Handles ctlHistory.Navigate
        If e.EntryName <> Nothing Then
            qString = ParamsFromString(e.EntryName)
            Search()
            upProducts.Update()
            upFacets.Update()
        End If
    End Sub

    Protected Sub PreferredVendorSelected(ByVal sender As Object, ByVal e As System.EventArgs)
        'Dim header As Controls.IHeaderControl = Page.FindControl("CTMain").FindControl("ctrlHeader")
        'VendorId = IIf(header.ReturnValue = String.Empty, Nothing, header.ReturnValue)
        VendorId = IIf(slPreferredVendor2.Value = String.Empty, Nothing, slPreferredVendor2.Value)
        Search()
        upProducts.Update()
    End Sub

    Private Function ParamsFromString(ByVal queryString As String) As URLParameters
        Dim out As New URLParameters
        If queryString.Length > 0 Then
            For Each item As String In queryString.Split("&")
                Dim pair As String() = item.Split("=")
                If pair(0)(0) = "?" Then
                    pair(0) = Server.UrlDecode(pair(0).Substring(1, pair(0).Length - 1))
                End If
                out.Add(pair(0), Server.UrlDecode(pair(1)))
            Next
        End If
        Return out
    End Function

    Protected Sub ctlNavigate_NavigatorEvent(ByVal sender As Object, ByVal e As Controls.NavigatorEventArgs) Handles ctlNavigate.NavigatorEvent
        ctlNavigate.PageNumber = e.PageNumber
        Dim qs As New URLParameters(qString.Items, "pg")
        qs.Add("pg", e.PageNumber)
        qString = qs
        Search()
    End Sub

    Protected Sub lnkExpand_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkExpand.Click
        'tvSupplyPhases.ExpandAll()
        'tvSupplyPhases.DataBind()
        'upFacets.Update()
        'TreeViewAddNodes(flyTreeView.Nodes, 0)
    End Sub

    Protected Sub lnkCollapse_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCollapse.Click
        'tvSupplyPhases.CollapseAll()
        'upFacets.Update()
    End Sub

    Protected Sub flyTreeView_PopulateNodes(ByVal sender As Object, ByVal e As NineRays.WebControls.FlyTreeNodeEventArgs)
        TreeViewAddNodes(e.Node.ChildNodes, e.Node.Value, Nothing, PhaseFilter)
        ShowHideNodes(e.Node.ChildNodes, PhaseFilter)
    End Sub

    Protected Sub flyTreeView_NodeSelected(ByVal sender As Object, ByVal e As NineRays.WebControls.FlyTreeNodeEventArgs)
        Dim qs As New URLParameters(qString.Items, "supplyphase;f;pg;guid;s")
        Dim f As String = qString("f")
        qs.Add("f", SearchIndex.ReplaceFacet(f, "supplyphase"))
        If e.Node.Value <> 0 Then
            qs.Add("supplyphase", e.Node.Value & "|" & Server.UrlEncode(e.Node.Text))
        Else
            'collaspe all nodes
            For Each node As FlyTreeNode In flyTreeView.Nodes
                node.Collapse()
            Next
        End If
        qString = qs
        Search()
        'upFacets.Update()
        'upProducts.Update()
    End Sub


    Private Sub ShowHideNodes(ByVal Nodes As FlyTreeNodeCollection, ByRef FilterList() As String)
        Dim NodeArray As New ArrayList()
        For Each node As FlyTreeNode In Nodes
            If Not FilterList Is Nothing AndAlso Not FilterList.Contains(node.Value) Then
                If Not node.Value = 0 Then
                    NodeArray.Add(node)
                End If
            End If
            ShowHideNodes(node.ChildNodes, FilterList)
        Next
        For Each node As FlyTreeNode In NodeArray
            node.Remove()
        Next
    End Sub

    Private Sub TreeViewAddNodes(ByVal nodes As FlyTreeNodeCollection, ByVal ParentId As Integer, ByVal SelectedNode As FlyTreeNode, Optional ByVal Filter() As String = Nothing)
        If dtPhases Is Nothing Then dtPhases = SupplyPhaseRow.GetList(DB)
        If ParentId = 0 Then

            Dim ftn As FlyTreeNode = New FlyTreeNode()
            ftn.Text = HttpUtility.HtmlEncode("All products")
            ftn.Value = 0
            ftn.Expanded = True

            If nodes.FindByValue(ftn.Value, True) Is Nothing Then
                nodes.Add(ftn)
            End If

            ParentId = SupplyPhaseRow.GetRootSupplyPhase(DB).SupplyPhaseID
        End If

        dtPhases.DefaultView.RowFilter = "ParentSupplyPhaseId = " & ParentId
        dtPhases.DefaultView.Sort = "SupplyPhase"
        For Each row As DataRowView In dtPhases.DefaultView
            Dim ftn As FlyTreeNode = New FlyTreeNode()
            ftn.Text = HttpUtility.HtmlEncode(row("SupplyPhase"))
            ftn.Value = row("SupplyPhaseId")

            If nodes.FindByValue(ftn.Value, True) Is Nothing Then
                nodes.Add(ftn)
            End If
            Try
                ftn.PopulateNodesOnDemand = dtPhases.Select("ParentSupplyPhaseId = " & row("SupplyPhaseId")).Count > 0
            Catch ex As Exception
            End Try
        Next

        If Not SelectedNode Is Nothing Then
            dtPhases.DefaultView.RowFilter = "ParentSupplyPhaseId = " & SelectedNode.Value
            dtPhases.DefaultView.Sort = "SupplyPhase"
            For Each row As DataRowView In dtPhases.DefaultView
                Dim ftn As FlyTreeNode = New FlyTreeNode()
                ftn.Text = HttpUtility.HtmlEncode(row("SupplyPhase"))
                ftn.Value = row("SupplyPhaseId")

                If SelectedNode.ChildNodes.FindByValue(ftn.Value, True) Is Nothing Then
                    SelectedNode.ChildNodes.Add(ftn)
                End If
                Try
                    ftn.PopulateNodesOnDemand = dtPhases.Select("ParentSupplyPhaseId = " & row("SupplyPhaseId")).Count > 0
                Catch ex As Exception
                End Try
            Next
        End If


    End Sub



    Protected Sub frmSendTakeoff_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmSendTakeoff.TemplateLoaded
        If Session("TakeoffForId") IsNot Nothing Then
            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("TakeoffForId"))
            ltlSendBuilder.Text = dbBuilder.CompanyName
        End If
    End Sub

   
End Class


