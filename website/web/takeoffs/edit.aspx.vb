Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager
Imports System.Linq
Imports System.Data
Imports System.IO
Imports Controls
Imports System.Data.SqlClient
Imports TwoPrice.DataLayer
Partial Class takeoffs_edit
    Inherits SitePage
    Implements ICallbackEventHandler

    Dim dtPhases As DataTable

    Protected BuilderId As Integer

    Private TotalProducts As Integer
    Private TotalPrice As Double
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private CurrentTakeoffId As String = ""
    Private CopyTakeOffId As String = ""

    Protected Property VendorId() As Integer
        Get
            Return ViewState("VendorId")
        End Get
        Set(ByVal value As Integer)
            ViewState("VendorId") = value
        End Set
    End Property

    Protected Property PhaseFilter() As String()
        Get
            Return ViewState("PhaseFilter")
        End Get
        Set(ByVal value As String())
            ViewState("PhaseFilter") = value
        End Set

    End Property

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

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        AddHandler btnAddAll.Click, AddressOf btnAddAll_Click
        AddHandler btnAddAll2.Click, AddressOf btnAddAll_Click
        AddHandler btnImport.Click, AddressOf btnImport_Click

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

        AddHandler slPreferredVendor2.ValueChanged, AddressOf PreferredVendorSelected
        If slPreferredVendor2.Value <> Nothing Then
            VendorId = slPreferredVendor2.Value
        ElseIf IsLoggedInVendor() Then
            VendorId = Session("VendorId")
        End If

        If Not IsPostBack Then
            If Request("TakeoffID") IsNot Nothing Then
                Session("CurrentTakeoffID") = Request("TakeoffID")
            End If

            'Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, IIf(Session("TakeoffForId") IsNot Nothing, Session("TakeoffForId"), Session("BuilderId")))

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

            If Session("BuilderId") IsNot Nothing Then
                Session("CurrentPreferredVendor") = Nothing

                Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, IIf(Session("TakeoffForId") IsNot Nothing, Session("TakeoffForId"), Session("BuilderId")))

                ctlSearch.FilterLLCID = dbBuilder.LLCID
                Session("InitialllcFilter") = dbBuilder.LLCID
                Dim list As New Generic.List(Of String)
                GetFilterList(list)
                'ctlSearch.FilterList = list
            End If
            BindTakeoff()
            ctlSearch.SearchProduct()

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
        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")
        CurrentTakeoffId = Session("CurrentTakeoffId")
    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        MyBase.Render(writer)
        For Each id As String In EventArgsToRegister.Keys
            Page.ClientScript.RegisterForEventValidation(id, EventArgsToRegister(id))
        Next
    End Sub

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
        'log Create new takeoff
        Core.DataLog("Takeoff", PageURL, CurrentUserId, "Create New Takeoff", CurrentTakeoffId, "", "", "", UserName)
        'end log
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
                'log copy
                CopyTakeOffId = Session("CurrentTakeoffId")
                Core.DataLog("Takeoff", PageURL, CurrentUserId, "Copy Takeoff", CopyTakeOffId, "", "", "", UserName)
                'end log
            End If
        End If
        Takeoff.BuilderID = IIf(Session("TakeoffForId") IsNot Nothing, Session("TakeoffForId"), Session("BuilderId"))
        Takeoff.Title = txtTakeoffTitle.Text
        Takeoff.ProjectID = slTakeoffProject.Value
        Takeoff.Update()
        'log save/copy/rename takeoff
        Core.DataLog("Takeoff", PageURL, CurrentUserId, "Save Takeoff", CurrentTakeoffId, "", "", "", UserName)
        'end log

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
        'log Add special order products
        Core.DataLog("Takeoff", PageURL, CurrentUserId, "Add Special Order Products", CurrentTakeoffId, "", "", "", UserName)
        'end log
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
        ctlSearch.SearchProduct(0, True, True)

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
                        'log Upload Products to Takeoff
                        Core.DataLog("Takeoff", PageURL, CurrentUserId, "Upload Products to Takeoff", CurrentTakeoffId, "", "", "", UserName)
                        'end log
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

        ctlSearch.SearchProduct(0, True, True)

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

            'log add related products to takeoff
            Core.DataLog("Takeoff", PageURL, CurrentUserId, "Add related Products to Takeoff", CurrentTakeoffId, "", "", "", UserName)
            'end log
        End If

    End Sub

#End Region

#Region "IDev"

    Protected Sub GetFilterList(ByRef list As Generic.List(Of String))
        list.AddRange(LLCRow.GetPricedProductsListForSearch(DB, BuilderRow.GetRow(DB, Session("BuilderId")).LLCID))
    End Sub

    Protected ReadOnly Property Keywords() As String
        Get
            Dim result As String = IIf(txtKeyword.Text = "Keyword Search", Nothing, txtKeyword.Text)
            If result Is Nothing OrElse result = String.Empty Then result = Request("Keywords")
            If result Is Nothing Then result = String.Empty

            Return result
        End Get
    End Property

    Protected Sub ctlSearch_OnTreeNodeSelect(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlSearch.OnTreeNodeSelect
        If VendorId <> Nothing Then
            Dim list As New Generic.List(Of String)
            'FilterCallback(list)
            ctlSearch.FilterList = list
        End If
    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        ctlSearch.SearchProduct(0, False, False)
    End Sub

    Protected Sub ctlSearch_ResultsUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlSearch.ResultsUpdated

        If VendorId <> Nothing Then
            Dim dtPrices As DataTable = VendorProductPriceRow.GetAllVendorPrices(DB, VendorId)
            rptProducts.DataSource = From r As DataRow In ctlSearch.SearchResults.AsEnumerable Group Join p As DataRow In dtPrices.Rows On Core.GetInt(r("ProductID")) Equals Core.GetInt(p("ProductID")) Into grp = Group From item In grp.Select(Of String)(Function(dr, idx) Core.GetString(dr("VendorPrice"))) Select New With {.ProductID = r("ProductID"), .Product = r("Product"), .Sku = r("Sku"), .VendorPrice = item}
            rptProducts.DataBind()
        Else
            rptProducts.DataSource = ctlSearch.SearchResults
            rptProducts.DataBind()
        End If

        If ctlSearch.SearchResults.Rows.Count > 0 Then
            ctlNavigate.Visible = True
            ctlNavigate.NofRecords = ctlSearch.SearchResults.Rows(0).Item(3)
            ctlNavigate.PageNumber = ctlSearch.PageNumber
            ctlNavigate.DataBind()
        Else
            ctlNavigate.Visible = False
        End If

        upProducts.Update()
        upTakeoff.Update()
    End Sub

    Protected Sub rptManufacturer_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptManufacturer.ItemDataBound
        If Not e.Item.ItemType = ListItemType.AlternatingItem AndAlso Not e.Item.ItemType = ListItemType.Item Then
            Exit Sub
        End If
        Dim Label As Literal = e.Item.FindControl("ltlLabel")
        'Label.Text = GetLabelText("manufacturer", e, "")
    End Sub

    Protected Sub rptProductType_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProductType.ItemDataBound
        If Not e.Item.ItemType = ListItemType.AlternatingItem AndAlso Not e.Item.ItemType = ListItemType.Item Then
            Exit Sub
        End If
        Dim Label As Literal = e.Item.FindControl("ltlLabel")
        'Label.Text = GetLabelText("producttype", e, "")
    End Sub

    Protected Sub rptUnitOfMeasure_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptUnitOfMeasure.ItemDataBound
        If Not e.Item.ItemType = ListItemType.AlternatingItem AndAlso Not e.Item.ItemType = ListItemType.Item Then
            Exit Sub
        End If
        Dim Label As Literal = e.Item.FindControl("ltlLabel")
        'Label.Text = GetLabelText("unitofmeasure", e, "")
    End Sub

#End Region

    Protected Sub ctlHistory_Navigate(ByVal sender As Object, ByVal e As nStuff.UpdateControls.HistoryEventArgs) Handles ctlHistory.Navigate
        If e.EntryName <> Nothing Then
            ctlSearch.SearchProduct(0, False, False)
            upProducts.Update()
        End If
    End Sub

    Protected Sub PreferredVendorSelected(ByVal sender As Object, ByVal e As System.EventArgs)
        If VendorId = Nothing Then
            'ctlSearch.FilterListCallback = Nothing
            'ctlSearch.FilterCacheKey = Nothing
            'ctlSearch.FilterList = Nothing
        Else
            'ctlSearch.FilterListCallback = AddressOf FilterCallback
            'ctlSearch.FilterCacheKey = VendorId
            Dim list As New Generic.List(Of String)
            'FilterCallback(list)
            'ctlSearch.FilterList = list
        End If
        ctlSearch.SearchProduct(0, True, True)
        upProducts.Update()
        upTakeoff.Update()
    End Sub

    Protected Sub ctlNavigate_NavigatorEvent(ByVal sender As Object, ByVal e As Controls.NavigatorEventArgs) Handles ctlNavigate.NavigatorEvent
        ctlNavigate.PageNumber = e.PageNumber
        ctlSearch.PageNumber = ctlNavigate.PageNumber
        ctlSearch.SearchProduct()
        upProducts.Update()
        upTakeoff.Update()
    End Sub

    Protected Sub frmSendTakeoff_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmSendTakeoff.TemplateLoaded
        If Session("TakeoffForId") IsNot Nothing Then
            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("TakeoffForId"))
            ltlSendBuilder.Text = dbBuilder.CompanyName
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

End Class


