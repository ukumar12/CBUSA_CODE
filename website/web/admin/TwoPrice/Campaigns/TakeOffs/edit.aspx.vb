Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager
Imports System.Linq
Imports System.Data
Imports System.IO
Imports Controls
Imports System.Data.SqlClient
Imports TwoPrice.DataLayer

Partial Class TwoPriceTakeOffs_edit
    Inherits AdminPageTwoPrice
    Implements ICallbackEventHandler

    Dim dtPhases As DataTable

#Region "Properties"
    Protected ReadOnly Property CampaignName As String
        Get
            Return dbTwoPriceCampaign.Name
        End Get
    End Property

    Private _TwoPriceCampaignId As Integer = 0
    Private Property TwoPriceCampaignId As Integer
        Get
            If _TwoPriceCampaignId = 0 Then
                _TwoPriceCampaignId = Convert.ToInt32(Request.QueryString("TwoPriceCampaignId"))
            End If

            Return _TwoPriceCampaignId
        End Get
        Set(value As Integer)
            _TwoPriceCampaignId = value
        End Set
    End Property

    Private _dbTwoPriceCampaign As TwoPriceCampaignRow = Nothing
    Private Property dbTwoPriceCampaign As TwoPriceCampaignRow
        Get
            If _dbTwoPriceCampaign Is Nothing Then
                _dbTwoPriceCampaign = TwoPriceCampaignRow.GetRow(DB, TwoPriceCampaignId)
            End If

            Return _dbTwoPriceCampaign
        End Get
        Set(value As TwoPriceCampaignRow)
            _dbTwoPriceCampaign = value
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

    Protected BuilderId As Integer
    Protected Property VendorId() As Integer
        Get
            Return Nothing
        End Get
        Set(ByVal value As Integer)
            ViewState("VendorId") = value
        End Set
    End Property

    Private TotalProducts As Integer
    Private TotalPrice As Double

    Private EventArgsToRegister As New Collections.Specialized.StringDictionary

    Private m_TwoPriceTakeOffProductIds As Generic.List(Of String)
    Private ReadOnly Property TwoPriceTakeOffProductIds() As Generic.List(Of String)
        Get
            If m_TwoPriceTakeOffProductIds Is Nothing Then
                m_TwoPriceTakeOffProductIds = New Generic.List(Of String)
            End If
            Return m_TwoPriceTakeOffProductIds
        End Get
    End Property

    Private AddedProductId As Integer

    Protected ReadOnly Property TwoPriceTakeOffId() As Integer
        Get
            If Not IsNumeric(Session("CurrentTwoPriceTakeOffId")) Then Session("CurrentTwoPriceTakeOffId") = Nothing
            Return Session("CurrentTwoPriceTakeOffId")
        End Get
    End Property

    Private dbTwoPriceTakeOff As TwoPriceTakeOffRow
    Protected ReadOnly Property TwoPriceTakeOff() As TwoPriceTakeOffRow
        Get
            'Check if Campaign has a TakeOff associated to it.
            If TwoPriceCampaignId <> Nothing Then
                If dbTwoPriceTakeOff Is Nothing Then
                    dbTwoPriceTakeOff = TwoPriceTakeOffRow.GetRowByTwoPriceCampaignId(DB, TwoPriceCampaignId)
                    If dbTwoPriceTakeOff.TwoPriceTakeOffID <> Nothing Then
                        Session("CurrentTwoPriceTakeOffId") = dbTwoPriceTakeOff.TwoPriceTakeOffID
                    End If
                End If
            End If
            'If there is no takeoff create one
            If dbTwoPriceTakeOff Is Nothing OrElse dbTwoPriceTakeOff.TwoPriceTakeOffID = Nothing Then
                dbTwoPriceTakeOff = New TwoPriceTakeOffRow(DB)
                dbTwoPriceTakeOff.TwoPriceCampaignId = TwoPriceCampaignId
                Session("CurrentTwoPriceTakeOffId") = dbTwoPriceTakeOff.Insert()
            End If
            Return dbTwoPriceTakeOff
        End Get
    End Property

#End Region

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        AddHandler btnAddAll.Click, AddressOf btnAddAll_Click
        AddHandler btnAddAll2.Click, AddressOf btnAddAll_Click
        AddHandler btnImport.Click, AddressOf btnImport_Click

        AddHandler slPreferredVendor2.ValueChanged, AddressOf PreferredVendorSelected
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'If Not IsLoggedInBuilder() Then
        '	If IsLoggedInVendor() Or (TypeOf HttpContext.Current.User Is AdminPrincipal AndAlso CType(HttpContext.Current.User, AdminPrincipal).Username IsNot Nothing) Then
        '		If Session("TwoPriceTakeOffForId") Is Nothing Then
        '			Response.Redirect("select-builder.aspx")
        '		End If
        '		aNewTwoPriceTakeOff.Visible = True
        '	Else
        '		Response.Redirect("/default.aspx")
        '	End If
        'Else
        '	aNewTwoPriceTakeOff.Visible = False

        CheckAccess("TWO_PRICE_CAMPAIGNS")

        TwoPriceTakeOff.Update()

        'slOrders.WhereClause = " BuilderId=" & DB.Number(Session("BuilderId"))
        'slTwoPriceTakeOffs.WhereClause = " BuilderId=" & DB.Number(Session("BuilderId"))
        'slProjects.WhereClause = " BuilderId=" & DB.Number(Session("BuilderId"))
        'slTwoPriceTakeOffProject.WhereClause = "BuilderID=" & DB.Number(Session("BuilderId"))

        'Dim header As Controls.IHeaderControl = Page.FindControl("CTMain").FindControl("ctrlHeader")
        'AddHandler header.ControlEvent, AddressOf ChangeVendor
        'If header.ReturnValue <> Nothing Then
        '    VendorId = header.ReturnValue
        'ElseIf IsLoggedInVendor() Then
        '    VendorId = Session("VendorId")
        'End If

        AddHandler slPreferredVendor2.ValueChanged, AddressOf PreferredVendorSelected
        If slPreferredVendor2.Value <> Nothing Then
            VendorId = slPreferredVendor2.Value
        ElseIf IsLoggedInVendor() Then
            VendorId = Session("VendorId")
        End If

        If Not IsPostBack Then
            If Request("TwoPriceTakeOffID") IsNot Nothing Then
                Session("CurrentTwoPriceTakeOffID") = Request("TwoPriceTakeOffID")
            End If

            If Request("TwoPriceTakeOffId") IsNot Nothing Then
                Session("CurrentTwoPriceTakeOffId") = Request("TwoPriceTakeOffId")
            End If

            If TwoPriceTakeOff.BuilderID <> Session("BuilderId") And TwoPriceTakeOff.BuilderID <> Session("TwoPriceTakeOffForId") Then
                Response.Redirect("default.aspx")
            End If

            txtTwoPriceTakeOffTitle.Text = TwoPriceTakeOff.Title
            'AR: put this line outside if statment to fix isues when copying then updating quantity. Take off title used to revert back to previous one.  
            'lblTwoPriceTakeOffTitle.Text = TwoPriceTakeOff.Title

            If TwoPriceTakeOff.ProjectID <> Nothing Then
                Dim dbProject As ProjectRow = ProjectRow.GetRow(DB, TwoPriceTakeOff.ProjectID)
                slTwoPriceTakeOffProject.Value = dbProject.ProjectID
                slTwoPriceTakeOffProject.Text = dbProject.ProjectName
            End If

            If dbTwoPriceCampaign IsNot Nothing Then
                btnSendTop.Visible = False
                btnSendBtm.Visible = False
                btnSaveTop.Visible = False
                btnSaveBtm.Visible = False
                btnCopyTop.Visible = False
                btnCopyBtm.Visible = False
                divComparisonLinkBtm.Visible = False
                divComparisonLinkTop.Visible = False
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

            Dim twoPriceCampaign As TwoPrice.DataLayer.TwoPriceCampaignRow = TwoPrice.DataLayer.TwoPriceCampaignRow.GetRow(DB, _TwoPriceCampaignId)
            Dim selectLLC As String = twoPriceCampaign.GetSelectedLLCs()

            Dim strArr() As String
            strArr = selectLLC.Split(",")
            Dim FirstLLC As String = strArr(0)

            Session("TwoPriceLLCID") = FirstLLC

            BindTwoPriceTakeOff()
            ctlSearch.SearchProduct()
        End If

        lblTwoPriceTakeOffTitle.Text = TwoPriceTakeOff.Title

        If VendorId = Nothing Then
            tdPriceHeader.Visible = False
            gvTwoPriceTakeOff.Columns(2).Visible = False
            'tdPrice.Visible = False
        Else
            tdPriceHeader.Visible = True
            gvTwoPriceTakeOff.Columns(2).Visible = False
            'tdPrice.Visible = True
        End If

        If TwoPriceTakeOff.Title <> Nothing Then
            divHead.InnerHtml = "Rename Take-off"
            btnSaveTop.Text = "Rename Take-off"
            btnSaveBtm.Text = "Rename Take-off"
        End If

    End Sub
    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        MyBase.Render(writer)
        For Each id As String In EventArgsToRegister.Keys
            Page.ClientScript.RegisterForEventValidation(id, EventArgsToRegister(id))
        Next
    End Sub

    Private Sub BindTwoPriceTakeOff()
        TotalProducts = 0
        TotalPrice = 0
        'Dim dtTwoPriceTakeOff As DataTable = TwoPriceTakeOff.GetProductsList()
        Dim dtTwoPriceTakeOff As DataTable = TwoPriceTakeOffRow.GetTwoPriceTakeOffProducts(DB, TwoPriceTakeOff.TwoPriceTakeOffID)
        TwoPriceTakeOffProductIds.Clear()
        gvTwoPriceTakeOff.DataSource = dtTwoPriceTakeOff
        gvTwoPriceTakeOff.DataBind()
        ltlTotalProducts.Text = TotalProducts
        ltlTotalPrice.Text = FormatCurrency(TotalPrice)
        For Each id As String In TwoPriceTakeOffProductIds
            'Page.ClientScript.RegisterArrayDeclaration("TwoPriceTakeOffProductIds", id)
            ScriptManager.RegisterArrayDeclaration(Me, "TwoPriceTakeOffProductIds", id)
        Next
    End Sub
#Region "Repeaters"

    Protected Sub rptProducts_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptProducts.ItemCommand
        Dim qty As TextBox = e.Item.FindControl("txtQty")

        If Not IsNumeric(qty.Text) Then
            qty.Text = 1
        End If

        If qty.Text <= 0 Then
            Exit Sub
        End If

        TwoPriceTakeOff.AddProduct(e.CommandArgument, qty.Text)
        BindTwoPriceTakeOff()

        qty.Text = String.Empty
        upTwoPriceTakeOff.Update()
    End Sub

    Protected Sub rptProducts_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProducts.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        If VendorId = Nothing Then
            e.Item.FindControl("tdPrice").Visible = False
        Else
            CType(e.Item.FindControl("tdPrice"), HtmlTableCell).InnerHtml = FormatCurrency(e.Item.DataItem.VendorPrice)
        End If
    End Sub

    Protected Sub btnDelAll_Click(sender As Object, e As EventArgs) Handles btnDelAll.Click
        For Each row As GridViewRow In gvTwoPriceTakeOff.Rows
            Dim btn As ImageButton = row.FindControl("btnDelete")
            Dim Id As Integer = Core.GetInt(IIf(btn IsNot Nothing, btn.CommandArgument, 0))

            TwoPriceTakeOffProductRow.RemoveRow(DB, Id)
        Next

        BindTwoPriceTakeOff()
        upTwoPriceTakeOff.Update()
    End Sub

    Protected Sub rptTwoPriceTakeOff_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvTwoPriceTakeOff.RowCommand
        If e.CommandName = "DeleteRecord" Then
            TwoPriceTakeOffProductRow.RemoveRow(DB, e.CommandArgument)
        End If
        BindTwoPriceTakeOff()
        upTwoPriceTakeOff.Update()
    End Sub

    Protected Sub rptTwoPriceTakeOff_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvTwoPriceTakeOff.RowDataBound
        'If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
        '	Exit Sub
        'End If

        If e.Row.RowType <> DataControlRowType.DataRow Then
            Exit Sub
        End If

        If Not IsDBNull(e.Row.DataItem("ProductId")) AndAlso Not TwoPriceTakeOffProductIds.Contains(e.Row.DataItem("ProductId")) Then
            TwoPriceTakeOffProductIds.Add(e.Row.DataItem("ProductId"))
        End If

        TotalProducts += e.Row.DataItem("Quantity")

        Dim ltlPrice As Literal = e.Row.FindControl("ltlPrice")
        'Dim tdPrice As HtmlTableCell = e.Row.FindControl("tdPrice")
        If VendorId = Nothing OrElse (IsDBNull(e.Row.DataItem("VendorPrice")) Or IsDBNull(e.Row.DataItem("Quantity"))) Then
            'tdPrice.Visible = False
            'tdPrice.InnerHtml = "&nbsp;"
            ltlPrice.Text = "&nbsp;"
        Else
            TotalPrice += e.Row.DataItem("VendorPrice") * e.Row.DataItem("Quantity")
            'tdPrice.InnerHtml = FormatCurrency(e.Row.DataItem("VendorPrice") * e.Row.DataItem("Quantity"))
            ltlPrice.Text = FormatCurrency(e.Row.DataItem("VendorPrice") * e.Row.DataItem("Quantity"))
        End If

    End Sub

#End Region

#Region "Events"
    Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        For Each item As GridViewRow In gvTwoPriceTakeOff.Rows
            Dim qty As TextBox = item.FindControl("txtQty")
            Dim btnDelete As ImageButton = item.FindControl("btnDelete")
            Dim dbTwoPriceTakeOffProduct As TwoPriceTakeOffProductRow = TwoPriceTakeOffProductRow.GetRow(DB, btnDelete.CommandArgument)
            If IsNumeric(qty.Text) Then
                If qty.Text = 0 Then
                    dbTwoPriceTakeOffProduct.Remove()
                Else
                    dbTwoPriceTakeOffProduct.Quantity = qty.Text
                    dbTwoPriceTakeOffProduct.Update()
                End If
            End If
        Next
        BindTwoPriceTakeOff()
        upTwoPriceTakeOff.Update()
    End Sub

    Private Sub gvList_DragAndDrop(ByVal sender As Object, ByVal e As DragAndDropEventArgs) Handles gvTwoPriceTakeOff.DragAndDrop
        Dim StartId As Integer = e.StartRowId
        Dim EndId As Integer = e.EndRowId

        Try
            DB.BeginTransaction()
            Core.ChangeSortOrderDragDrop(DB, "TwoPriceTakeOffProduct", "SortOrder", "TwoPriceTakeOffProductID", StartId, EndId, "TwoPriceTakeOffId = " & TwoPriceTakeOffId)
            DB.CommitTransaction()

            BindTwoPriceTakeOff()

        Catch ex As SqlException
            If Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnSaveSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveSubmit.Click
        If hdnIsCopy.Value <> Nothing AndAlso Convert.ToBoolean(hdnIsCopy.Value) Then
            Dim dbNewTwoPriceTakeOff As TwoPriceTakeOffRow = TwoPriceTakeOff.Copy(txtTwoPriceTakeOffTitle.Text)
            If dbNewTwoPriceTakeOff IsNot Nothing Then
                Session("CurrentTwoPriceTakeOffId") = dbNewTwoPriceTakeOff.TwoPriceTakeOffID
                dbTwoPriceTakeOff = dbNewTwoPriceTakeOff
            End If
        End If
        TwoPriceTakeOff.Title = txtTwoPriceTakeOffTitle.Text
        TwoPriceTakeOff.ProjectID = slTwoPriceTakeOffProject.Value
        TwoPriceTakeOff.Update()

        divHead.InnerHtml = "Rename TwoPriceTakeOff"
        lblSavedMsg.Text = "<b>TwoPriceTakeOff '" & TwoPriceTakeOff.Title & "' has been saved.</b><br/>"
        BindTwoPriceTakeOff()
    End Sub

    Protected Sub frmSendTwoPriceTakeOff_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmSendTwoPriceTakeOff.Postback
        TwoPriceTakeOff.BuilderID = Session("TwoPriceTakeOffForId")
        TwoPriceTakeOff.Title = txtSendTitle.Text
        TwoPriceTakeOff.Update()

        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, TwoPriceTakeOff.BuilderID)
        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, Session("VendorId"))
        Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "TwoPriceTakeOffSent")
        Dim msg As String = "TwoPriceTakeOff Title:" & vbTab & vbTab & TwoPriceTakeOff.Title & vbCrLf & "Vendor:" & vbTab & vbTab & dbVendor.CompanyName & vbCrLf
        dbMsg.Send(dbBuilder, vbCrLf & msg)
    End Sub

    Protected Sub btnAddSpecial_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddSpecial.Click
        Dim dbSpecial As New SpecialOrderProductRow(DB)
        dbSpecial.SpecialOrderProduct = txtProductName.Text
        dbSpecial.Description = txtDescription.Text
        dbSpecial.UnitOfMeasureID = acUnit.Value
        dbSpecial.Insert()

        Dim dbProduct As New TwoPriceTakeOffProductRow(DB)
        dbProduct.TwoPriceTakeOffID = TwoPriceTakeOff.TwoPriceTakeOffID
        dbProduct.Quantity = txtSpecialQuantity.Text
        dbProduct.SpecialOrderProductID = dbSpecial.SpecialOrderProductID
        dbProduct.Insert()

        ScriptManager.RegisterStartupScript(Page, Me.GetType, "CloseSpecial", "CloseSpecialForm();", True)

        BindTwoPriceTakeOff()
    End Sub

    Protected Sub btnAddAll_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        For Each item As RepeaterItem In rptProducts.Items
            Dim qty As TextBox = item.FindControl("txtQty")
            Dim btn As Button = item.FindControl("btnAddProduct")
            If IsNumeric(qty.Text) AndAlso CInt(qty.Text) > 0 Then
                TwoPriceTakeOff.AddProduct(btn.CommandArgument, qty.Text)
            End If
        Next
        BindTwoPriceTakeOff()
        ctlSearch.SearchProduct(0, True, True)

        upProducts.Update()
        upTwoPriceTakeOff.Update()
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

            fulDocument.Folder = "/assets/TwoPriceTakeOff/"
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

        Session("TwoPriceTakeOffImportFile") = NewFileName & OriginalExtension

        Try
            ImportTwoPriceTakeOff(Session("TwoPriceTakeOffImportFile"))
            BindTwoPriceTakeOff()
        Catch ex As Exception
            ltrErrorMsg.Text = Err.Description
            ltrErrorMsg.Visible = True
        End Try

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
                TwoPriceTakeOff.AddProduct(btnAdd.CommandArgument, qty.Text)
            Else
                Dim TwoPriceTakeOffProductId As Integer = Request.Form(form.UniqueID & "$LineItems")
                Dim dbProduct As TwoPriceTakeOffProductRow = TwoPriceTakeOffProductRow.GetRow(DB, TwoPriceTakeOffProductId)
                dbProduct.Quantity += qty.Text
                dbProduct.Update()
            End If
        End If

        BindTwoPriceTakeOff()

        ctlSearch.SearchProduct(0, True, True)

        upProducts.Update()
        upTwoPriceTakeOff.Update()

    End Sub

    Public Function GetCallbackResult() As String Implements System.Web.UI.ICallbackEventHandler.GetCallbackResult
        Dim dtItems As DataTable = TwoPriceTakeOff.GetProductLineItems(DB, TwoPriceTakeOffId, AddedProductId)
        Dim rb As RadioButton
        phLineItems.Controls.Clear()
        For Each row As DataRow In dtItems.Rows
            rb = New RadioButton
            rb.ID = row("TwoPriceTakeOffProductId")
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

#End Region

#Region "Helper Functions"
    Protected Sub ImportTwoPriceTakeOff(ByVal FileName As String)
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
                        'Add the TwoPriceTakeOff to 
                        For Each dr As DataRow In dtProduct.Rows
                            ProductId = dr("ProductId")
                            TwoPriceTakeOff.AddProduct(ProductId, Qty)
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
#End Region

#Region "Products To Compare"
    Private dtProducts As DataTable = Nothing
    Public ReadOnly Property CompareProducts() As DataTable
        Get
            If dtProducts Is Nothing Then
                If slTwoPriceTakeOffs.Value <> Nothing Then
                    dtProducts = TwoPriceTakeOffRow.GetTwoPriceTakeOffProducts(DB, slTwoPriceTakeOffs.Value)
                ElseIf slOrders.Value <> Nothing Then
                    dtProducts = OrderRow.GetOrderProducts(DB, slOrders.Value)
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
    '    acTwoPriceTakeOffs.WhereClause = " BuilderId=" & DB.Number(BuilderId) & " and ProjectId=" & DB.Number(acProjects.Value)
    '    'upFilterBar.Update()
    'End Sub

    'Protected Sub acTwoPriceTakeOffs_ValueUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles acTwoPriceTakeOffs.ValueUpdated
    'End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnAdd.Click
        Dim dt As DataTable = CompareProducts
        For Each row As DataRow In dt.Rows
            TwoPriceTakeOff.AddProduct(row("ProductId"), row("Quantity"))
        Next
        BindTwoPriceTakeOff()

        slProjects.Text = ""
        slTwoPriceTakeOffs.Text = ""
        slOrders.Text = ""
        'upFilterBar.Update()
    End Sub

#End Region

#Region "IDev"

    Protected Sub GetFilterList(ByRef list As Generic.List(Of String))
        list.AddRange(LLCRow.GetPricedProductsListFromLLCs(DB, dbTwoPriceCampaign.GetSelectedLLCs()))
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
    End Sub

    Protected Sub ctlNavigate_NavigatorEvent(ByVal sender As Object, ByVal e As Controls.NavigatorEventArgs) Handles ctlNavigate.NavigatorEvent
        ctlNavigate.PageNumber = e.PageNumber
        ctlSearch.PageNumber = ctlNavigate.PageNumber
        ctlSearch.SearchProduct()
        upProducts.Update()
    End Sub

    Protected Sub frmSendTwoPriceTakeOff_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmSendTwoPriceTakeOff.TemplateLoaded
        If Session("TwoPriceTakeOffForId") IsNot Nothing Then
            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("TwoPriceTakeOffForId"))
            ltlSendBuilder.Text = dbBuilder.CompanyName
        End If
    End Sub

#Region "Export"

    Public Sub ExportProductCSV()
        Dim dt As DataTable = dbTwoPriceTakeOff.GetProductsList()
        SaveExport(dt)
    End Sub

    Private Sub SaveExport(ByVal q As DataTable)
        Dim CbusaSKU As String = String.Empty
        Dim ProductName As String = String.Empty
        Dim Quantity As String = String.Empty

        Dim fname As String = "/assets/vendor/product/" & Core.GenerateFileID & ".csv"
        Dim sw As IO.StreamWriter = IO.File.CreateText(Server.MapPath(fname))
        sw.WriteLine("SKU, Product Name, Quantity")
        For Each row As DataRow In q.Rows
            If Not IsDBNull(row.Item("SKU")) Then
                CbusaSKU = row.Item("SKU")
            Else
                CbusaSKU = ""
            End If
            If Not IsDBNull(row.Item("Product")) Then
                ProductName = row.Item("Product")
            Else
                ProductName = ""
            End If
            If Not IsDBNull(row.Item("Quantity")) Then
                Quantity = row.Item("Quantity")
            Else
                Quantity = ""
            End If

            sw.WriteLine(Core.QuoteCSV(CbusaSKU) & "," & Core.QuoteCSV(ProductName) & "," & Core.QuoteCSV(Quantity))
        Next
        sw.Close()
        Response.Redirect(fname)
    End Sub

#End Region

#Region "Import"
    Protected Sub btnImportPrice_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnImportPrice.Click
        Dim FileInfo As System.IO.FileInfo
        Dim OriginalExtension As String
        Dim NewFileName As String

        If fulPrice.NewFileName <> String.Empty Then

            OriginalExtension = System.IO.Path.GetExtension(fulPrice.MyFile.FileName)

            If OriginalExtension <> ".csv" And OriginalExtension <> ".xls" Then
                ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
                ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""><li>Please enter a valid .csv or .xls file. Your file extension was: " & OriginalExtension & "</span></td></tr></table>"
                ltrErrorMsg.Visible = True
                Exit Sub
            End If

            fulPrice.Folder = "/assets/vendor/product/"
            fulPrice.SaveNewFile()

            FileInfo = New System.IO.FileInfo(Server.MapPath(fulPrice.Folder & fulPrice.NewFileName))

            NewFileName = Core.GenerateFileID
            FileInfo.CopyTo(Server.MapPath(fulPrice.Folder & NewFileName & OriginalExtension))

            FileInfo.Delete()
        Else
            ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
            ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""><li>Please enter the file you want to upload</span></td></tr></table>"
            ltrErrorMsg.Visible = True
            Exit Sub
        End If

        Session("VendorImportPriceFile") = NewFileName & OriginalExtension
        Try
            ImportPriceCSV(Session("VendorImportPriceFile"), True)
        Catch ex As Exception
            ltrErrorMsg.Text = Err.Description
            ltrErrorMsg.Visible = True
        End Try

        BindTwoPriceTakeOff()
        upTwoPriceTakeOff.Update()
    End Sub

    Private Sub ImportPriceCSV(ByVal FileName As String, ByVal UpdateDatabase As Boolean)
        Dim aLine As String()
        Dim Count As Integer = 0
        Dim InsertCount As Integer = 0
        Dim UpdateCount As Integer = 0
        Dim BadCount As Integer = 0
        Dim txtErr As String = String.Empty
        Dim tblErr As String = String.Empty
        Dim IsUpdate As Boolean = False
        Dim UpdateCurrent As Boolean = False
        Dim ProductId As Integer = 0
        Dim CbusaSKU As String = String.Empty
        Dim Quantity As String = String.Empty

        If Not File.Exists(Server.MapPath(fulPrice.Folder & FileName)) Then
            ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
            ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""></li>Cannot find the file to process</span></td></tr></table>"
            Exit Sub
        End If

        Dim f As StreamReader = New StreamReader(Server.MapPath(fulPrice.Folder & FileName))
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
                IsUpdate = False
                ProductId = 0
                CbusaSKU = Trim(Core.StripDblQuote(aLine(0)))
                Quantity = Trim(Core.StripDblQuote(aLine(1)))

                If Count = 1 And CbusaSKU.ToUpper <> "CBUSA SKU" Then
                    ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
                    ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""><li>The file you uploaded does not appear to be in the correct format. The file format must be identical to the one used in the Export CSV process.</td></tr></table>"
                    Exit Sub
                End If

                If CbusaSKU.ToUpper = "CBUSA SKU" Then
                    Continue While
                End If

                txtErr = String.Empty

                'SKU must be in system
                ProductId = DB.ExecuteScalar("SELECT ProductId FROM Product WHERE SKU = '" & CbusaSKU & "'")
                If ProductId = Nothing Then
                    BadCount = BadCount + 1
                    txtErr &= "<li>CBUSA SKU invalid"
                    tblErr &= "<tr class=""red""><td>" & CbusaSKU & "</td><td>" & txtErr & "</td><tr>"
                    Continue While
                End If

                'Quantity must be a numeric value and greater than 0
                If Quantity <> String.Empty Then
                    If Not IsNumeric(Quantity) Then
                        BadCount = BadCount + 1
                        txtErr &= "<li>Invalid price"
                        tblErr &= "<tr class=""red""><td>" & CbusaSKU & "</td><td>" & txtErr & "</td><tr>"
                        Continue While
                    ElseIf Quantity <= 0 Then
                        BadCount = BadCount + 1
                        txtErr &= "<li>Non-Positive price"
                        tblErr &= "<tr class=""red""><td>" & CbusaSKU & "</td><td>" & txtErr & "</td><tr>"
                        Continue While
                    End If
                End If
                If txtErr = String.Empty Then

                    Dim dt As DataTable = DB.GetDataTable("Select Top 1 * From TwoPriceTakeOffProduct Where TwoPriceTakeOffId = " & dbTwoPriceTakeOff.TwoPriceTakeOffID & " And ProductId = " & ProductId)
                    IsUpdate = dt.Rows.Count > 0
                    Dim PriceRequired As Boolean = True

                    If Quantity = String.Empty OrElse Not IsNumeric(Quantity) Then
                        txtErr &= "<li>Quantity required"
                    End If

                    If txtErr = String.Empty And UpdateDatabase Then

                        Try
                            DB.BeginTransaction()
                            If IsUpdate Then
                                TwoPriceTakeOff.UpdateProduct(ProductId, aLine(1), True)
                                UpdateCount += 1
                            Else
                                TwoPriceTakeOff.AddProduct(ProductId, aLine(1))
                                InsertCount += 1
                            End If
                            DB.CommitTransaction()
                        Catch ex As Exception
                            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                            ltrErrorMsg.Text = ErrHandler.ErrorText(ex)
                            Exit Sub
                        End Try
                    End If

                    If txtErr <> String.Empty Then
                        BadCount = BadCount + 1
                        tblErr &= "<tr class=""red""><td>" & CbusaSKU & "</td><td>" & txtErr & "</td><tr>"
                    ElseIf Not UpdateDatabase Then
                        If IsUpdate Then
                            UpdateCount = UpdateCount + 1
                        Else
                            InsertCount = InsertCount + 1
                        End If
                    End If
                End If
            End If
        End While
        f.Close()

        Count = Count - 1

        tblErr = "<tr class=""red bold""><td>Invalid Entries</td><td>" & BadCount & "</td><tr>" & tblErr

        If UpdateDatabase Then
            tblErr = "<tr class=""green""><td>Products Added</td><td>" & InsertCount & "</td><tr>" & tblErr
            tblErr = "<tr class=""green""><td>Products Updated</td><td>" & UpdateCount & "</td><tr>" & tblErr
            ltrErrorMsg.Text = "<b>Data import process completed with success. Please review the report below for more details.</b>"
        Else
            tblErr = "<tr class=""green""><td>Products To Add</td><td>" & InsertCount & "</td><tr>" & tblErr
            tblErr = "<tr class=""green""><td>Products To Update</td><td>" & UpdateCount & "</td><tr>" & tblErr
            ltrErrorMsg.Text = "<b>File uploaded successfully. Please review the report below.</b>"
        End If

        tblErr = "<tr class=""green""><td>Valid Entries</td><td>" & Count - BadCount & "</td><tr>" & tblErr
        tblErr = "<tr class=""blue bold""><td>Total Rows Processed</td><td>" & Count & "</td><tr>" & tblErr

        ltrErrorMsg.Text &= "<hr><table align=""center"">" & tblErr & "</table><hr>"
        ltrErrorMsg.Visible = True
    End Sub

#End Region

    Protected Sub slPreferredVendor2_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles slPreferredVendor2.ValueChanged
        If slPreferredVendor2.Value Is Nothing Or slPreferredVendor2.Value = "" Then
            Session("CurrentPreferredVendor") = Nothing
        Else
            Session("CurrentPreferredVendor") = slPreferredVendor2.Value
            ctlSearch.CatalogType = controls_SearchSql.CATALOG_TYPE.CATALOG_TYPE_MARKET
        End If
    End Sub

End Class


