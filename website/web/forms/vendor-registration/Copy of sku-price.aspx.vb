Imports Components
Imports DataLayer
Imports IDevSearch
Imports System.Configuration.ConfigurationManager
Imports System.Linq
Imports System.Data
Imports System.IO

Partial Class skuprice
    Inherits SitePage

    Protected VendorId As Integer

    Protected dbVendor As VendorRow
    Private bExport As Boolean = False
    Private bPriceExport As Boolean = False

    Private EventArgsToRegister As New Collections.Specialized.StringDictionary

    Function GetVendorSupplyPhases(ByVal VendorId As Integer) As String
        Dim SupplyPhases As String = String.Empty
        Dim sConn As String = String.Empty
        Dim SQL As String = "Select sp.SupplyPhaseID, sp.SupplyPhase From SupplyPhase sp, VendorSupplyPhase vsp Where sp.SupplyPhaseID = vsp.SupplyPhaseID And vsp.VendorId = " & DB.Number(VendorId) & " order by sp.SupplyPhase"
        Dim dt As DataTable = DB.GetDataTable(SQL)
        For Each r As DataRow In dt.Rows
            SupplyPhases &= sConn & r("SupplyPhaseID")
            sConn = ","
        Next
        Return SupplyPhases
    End Function

    Private m_FilterList As Generic.List(Of String)

    Private Sub GetFilterList(ByRef list As Generic.List(Of String))
        If m_FilterList Is Nothing Then
            m_FilterList = New Generic.List(Of String)
            Dim SQL As String = String.Empty
            Select Case rblFilter.SelectedValue
                Case "vendor"
                    SQL = "select ProductID from VendorProductPrice where VendorID=" & DB.Number(VendorId)
                Case "phases"
                    SQL = "Select sp.ProductID From SupplyPhaseProduct sp, VendorSupplyPhase vsp Where sp.SupplyPhaseID = vsp.SupplyPhaseID And vsp.VendorId = " & DB.Number(VendorId) & " and vsp.SupplyPhaseID not in (select e.SupplyPhaseID from SupplyPhaseLLCExclusion e inner join LLCVendor l on e.LLCID=l.LLCID where l.VendorID=" & DB.Number(Session("VendorId")) & ") order by sp.ProductID"
                Case "market"
                    SQL = "select ProductID from VendorProductPrice where VendorID in (select VendorID from LLCVendor where LLCID in (select LLCID from LLCVendor where VendorID=" & DB.Number(VendorId) & "))"
                Case "all"
                    SQL = String.Empty
            End Select
            If SQL <> String.Empty Then
                Dim sdr As SqlClient.SqlDataReader = DB.GetReader(SQL)
                While sdr.Read()
                    m_FilterList.Add(sdr("ProductID"))
                End While
                sdr.Close()
                list.AddRange(m_FilterList)
            Else
                list = Nothing
            End If
        End If
    End Sub

    Private m_TreeFilterList As Generic.List(Of String)

    Private ReadOnly Property TreeFilterList() As Generic.List(Of String)
        Get
            If m_TreeFilterList Is Nothing Then
                m_TreeFilterList = New Generic.List(Of String)
                Dim sql As String = String.Empty

                Select Case rblFilter.SelectedValue
                    Case "vendor"
                        sql = "select SupplyPhaseID from SupplyPhaseProduct where ProductID in (select ProductID from VendorProductPrice where VendorID=" & DB.Number(VendorId) & ")"
                    Case "phases"
                        sql = "select SupplyPhaseID from SupplyPhaseProduct where ProductID in (Select sp.ProductID From SupplyPhaseProduct sp, VendorSupplyPhase vsp Where sp.SupplyPhaseID = vsp.SupplyPhaseID And vsp.VendorId = " & DB.Number(VendorId) & " and vsp.SupplyPhaseID not in (select e.SupplyPhaseID from SupplyPhaseLLCExclusion e inner join LLCVendor l on e.LLCID=l.LLCID where l.VendorID=" & DB.Number(Session("VendorId")) & "))"
                    Case "market"
                        sql = "select SupplyPhaseID from SupplyPhaseProduct where ProductID in (select ProductID from VendorProductPrice where VendorID in (select VendorID from LLCVendor where LLCID in (select LLCID from LLCVendor where VendorID=" & DB.Number(VendorId) & ")))"
                    Case Else
                        sql = "select SupplyPhaseID from SupplyPhase"
                End Select
                If sql <> String.Empty Then
                    Dim sdr As SqlClient.SqlDataReader = DB.GetReader(sql)
                    While sdr.Read()
                        m_TreeFilterList.Add(sdr("SupplyPhaseID"))
                    End While
                    sdr.Close()
                End If
            End If
            Return m_TreeFilterList
        End Get
    End Property


    Private m_VendorPrices As DataView
    Public ReadOnly Property dvVendorPrices() As DataView
        Get
            If m_VendorPrices Is Nothing Then
                Dim dt As DataTable = VendorProductPriceRow.GetAllVendorPrices(DB, Session("VendorId"))
                m_VendorPrices = dt.DefaultView
                m_VendorPrices.Sort = "ProductID"
            End If
            Return m_VendorPrices
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        VendorId = CType(Session("VendorId"), Integer)

        If VendorId = 0 Then
            Response.Redirect("/default.aspx")
        End If

        dbVendor = VendorRow.GetRow(Me.DB, VendorId)

        ctlSearch.MaxDocs = 32000
        ctlSearch.KeywordsTextboxId = txtKeyword.UniqueID
        ctlSearch.FilterCacheKey = Session("VendorId") & rblFilter.SelectedValue
        'ctlSearch.FilterListCallback = AddressOf GetFilterList
        Dim list As New Generic.List(Of String)
        GetFilterList(list)
        ctlSearch.FilterList = list
        ctlSearch.TreeOnlyFilter = TreeFilterList

        ctlSubSearch.MaxDocs = 32000
        ctlSubSearch.KeywordsTextboxId = txtSubKeywords.UniqueID
        If rblSubPricedOnly.SelectedValue = True Then
            'ctlSubSearch.FilterListCallback = AddressOf SubFilterListCallback
            'ctlSubSearch.FilterCacheKey = Session("VendorId")
            list = New Generic.List(Of String)
            SubFilterListCallback(list)
            ctlSubSearch.FilterList = list
        Else
            'ctlSubSearch.FilterListCallback = Nothing
            'ctlSubSearch.FilterCacheKey = Nothing
            ctlSubSearch.FilterList = Nothing
        End If
        'ctlSubSearch.TreeOnlyFilter = TreeFilterList

        If Not IsPostBack Then
            ctlSearch.PageSize = ctlNavigator.MaxPerPage
            ctlSearch.PageNumber = 1
            ctlSearch.ForceFilter = True
            ctlSearch.Search()

            ctlSubSearch.PageSize = ctlSubNavigator.MaxPerPage
            ctlSubSearch.PageNumber = 1
            ctlSubSearch.ForceFilter = True
            ctlSubSearch.Search(True)
        End If

    End Sub

    Protected Sub AddSupplyPhase(ByVal SupplyPhaseId As Integer)
        Dim dt As DataTable
        Dim SQL As String = "Select * From VendorSupplyPhase Where SupplyPhaseID = " & DB.Number(SupplyPhaseId) & " And VendorId = " & DB.Number(VendorId)

        dt = DB.GetDataTable(SQL)

        If dt.Rows.Count = 0 Then
            Try
                DB.BeginTransaction()
                SQL = "Insert Into VendorSupplyPhase (VendorId, SupplyPhaseID) Values (" & DB.Number(VendorId) & ", " & DB.Number(SupplyPhaseId) & ")"
                DB.ExecuteSQL(SQL)
                DB.CommitTransaction()
            Catch ex As Exception
                If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                AddError(ErrHandler.ErrorText(ex))
            End Try
        End If

    End Sub

    Private Function GetValue(ByVal row As DataRowView, ByVal field As String) As Object
        If row Is Nothing Then
            Return Nothing
        ElseIf IsDBNull(row(field)) Then
            Return Nothing
        Else
            Return row(field)
        End If
    End Function

    Protected Sub ctlSearch_ResultsUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlSearch.ResultsUpdated
        rptProducts.DataSource = From sr As DataRow In ctlSearch.SearchResults.ds.Tables(0).AsEnumerable _
                                 Group Join price As DataRowView In dvVendorPrices _
                                 On Core.GetString(sr("ProductID")) Equals Core.GetString(price("ProductID")) _
                                 Into grp = Group _
                                 From price In grp.DefaultIfEmpty _
                                 Select New With { _
                                    .ProductID = sr("ProductID"), _
                                    .Product = sr("ProductName"), _
                                    .SKU = sr("Sku"), _
                                    .VendorSKU = GetValue(price, "VendorSku"), _
                                    .VendorPrice = GetValue(price, "VendorPrice"), _
                                    .NextPrice = GetValue(price, "NextPrice"), _
                                    .NextPriceApplies = GetValue(price, "NextPriceApplies"), _
                                    .Updated = GetValue(price, "Updated"), _
                                    .IsDiscontinued = GetValue(price, "IsDiscontinued"), _
                                    .IsSubstitution = GetValue(price, "IsSubstitution"), _
                                    .SubstituteProduct = GetValue(price, "SubstituteProduct"), _
                                    .SubstituteSKU = GetValue(price, "SubstituteSku"), _
                                    .SubstitutePrice = GetValue(price, "SubstitutePrice") _
                                 }
        If bExport Then
            SaveExport(rptProducts.DataSource)
        End If
        If bPriceExport Then
            SavePriceExport(rptProducts.DataSource)
        End If
        rptProducts.DataBind()
        ctlNavigator.NofRecords = ctlSearch.SearchResults.Count
        If ctlNavigator.NofRecords = 0 Then
            ctlNavigator.Visible = False
        Else
            ctlNavigator.PageNumber = IIf(ctlSearch.PageNumber = Nothing, 1, ctlSearch.PageNumber)
            ctlNavigator.DataBind()
        End If
        ltlBreadcrumb.Text = ctlSearch.Breadcrumbs
        ltrErrorMsg.Visible = False
        upProducts.Update()
    End Sub

    Private Sub BindProducts()

        Dim SQL As String = "SELECT p.ProductID, p.Product, p.SKU, vpp.VendorSKU, vpp.VendorPrice, vpp.IsSubstitution, vpp.IsDiscontinued, vpp.Updated "
        SQL &= "FROM Product p INNER JOIN "
        SQL &= "SupplyPhaseProduct spp ON p.ProductID = spp.ProductID LEFT OUTER JOIN "
        SQL &= "(Select * From VendorProductPrice Where VendorId = " & DB.Number(VendorId) & ") vpp ON p.ProductID = vpp.ProductID "
        SQL &= "Where "
        If Session("SupplyPhase") = String.Empty Then
            SQL &= "spp.SupplyPhaseID In " & DB.NumberMultiple(GetVendorSupplyPhases(VendorId))
            ltlBreadcrumb.Text = "<a href=""/"">Home</a> > All Supply Phases"
        Else
            SQL &= "spp.SupplyPhaseID = " & DB.Number(Session("SupplyPhase"))
            ltlBreadcrumb.Text = "<a href=""/"">Home</a> > <a href=""sku-price.aspx?sp=N"">All Supply Phases</a> > " & SupplyPhaseRow.GetRow(DB, Session("SupplyPhase")).SupplyPhase
        End If
        If txtKeyword.Text <> String.Empty And txtKeyword.Text <> "Keyword Search" Then
            SQL &= " And ("
            SQL &= "p.Product Like " & DB.FilterQuote(txtKeyword.Text)
            SQL &= " Or p.Description Like " & DB.FilterQuote(txtKeyword.Text)
            SQL &= " Or p.SKU = " & DB.Quote(txtKeyword.Text)
            SQL &= ")"
            ltlBreadcrumb.Text = "<a href=""/"">Home</a> > <a href=""sku-price.aspx?sp=N"">All Supply Phases</a> > " & txtKeyword.Text
            Session("SupplyPhase") = String.Empty
        End If

        Dim dtProducts As DataTable = DB.GetDataTable(SQL)
        rptProducts.DataSource = dtProducts
        rptProducts.DataBind()

        If dtProducts.Rows.Count = 0 Then
            txtNoResults.Visible = True
            rptProducts.Visible = False
        Else
            txtNoResults.Visible = False
            rptProducts.Visible = True
        End If

        If bExport Then
            SaveExport(SQL)
        End If

        btnImportUpdate.Visible = False
        btnImportPriceUpdate.Visible = False

    End Sub



    Protected Sub rptProducts_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptProducts.ItemCommand
        Dim txtSku As TextBox = CType(e.Item.FindControl("txtSku"), TextBox)
        Dim txtCurrentPrice As TextBox = CType(e.Item.FindControl("txtCurrentPrice"), TextBox)
        Dim txtNextPrice As TextBox = CType(e.Item.FindControl("txtNextPrice"), TextBox)
        Dim ErrorMsg As String = String.Empty
        Dim VendorPrice As Double = 0
        Dim VendorSku As String = String.Empty
        Dim imgReq As WebControls.Image = CType(e.Item.FindControl("imgReq"), WebControls.Image)

        btnImportUpdate.Visible = False
        btnImportPriceUpdate.Visible = False

        Dim dtLLC As DataTable = DB.GetDataTable("Select l.* From LLC l, LLCProductPriceRequirement lp, LLCVendor lv Where l.LLCID = lp.LLCID And l.LLCID = lv.LLCID And lp.ProductId = " & DB.Number(e.CommandArgument) & " And lv.VendorId = " & DB.Number(VendorId))

        Dim PriceRequired As Boolean = dtLLC.Rows.Count > 0

        Dim dt As DataTable = DB.GetDataTable("Select Top 1 * From VendorProductPrice Where VendorId = " & DB.Number(VendorId) & " And ProductId = " & DB.Number(e.CommandArgument))

        For Each rvp As DataRow In dt.Rows
            If Not IsDBNull(rvp("VendorPrice")) Then
                VendorPrice = rvp("VendorPrice")
            End If
            If Not IsDBNull(rvp("VendorSku")) Then
                VendorSku = rvp("VendorSku")
            End If
        Next

        'If VendorSku = String.Empty Then
        '    If txtSku.Text = String.Empty Then
        '        ErrorMsg = "<li>Please enter your Sku for the selected product"
        '    End If
        'End If

        If txtCurrentPrice.Text <> String.Empty Then
            If Not IsNumeric(txtCurrentPrice.Text) Then
                ErrorMsg &= "<li>Please enter a valid Current Price for the selected product"
            ElseIf txtCurrentPrice.Text <= 0 Then
                ErrorMsg &= "<li>Please enter a valid Current Price for the selected product"
            ElseIf VendorPrice > 0 And Math.Round(Double.Parse(txtCurrentPrice.Text), 2) > VendorPrice Then
                ErrorMsg &= "<li>Updated Current Price cannot be greater than the Current Price"
            End If
        ElseIf VendorPrice <> Nothing Or (VendorPrice = 0 And PriceRequired) Then
            ErrorMsg &= "<li>Price for selected product is required."
        End If

        If txtNextPrice.Text <> String.Empty Then
            If Not IsNumeric(txtNextPrice.Text) Then
                ErrorMsg &= "<li>Please enter a valid Next Price for the selected product"
            ElseIf txtNextPrice.Text <= 0 Then
                ErrorMsg &= "<li>Please enter a valid Next Price for the selected product"
            End If
        End If

        If ErrorMsg <> String.Empty Then
            ltrErrorMsg.Visible = True
            ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
            ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold"">" & ErrorMsg & "</span></td></tr></table>"
            imgReq.ImageUrl = "/images/exclam.gif"
            imgReq.Visible = True
            Exit Sub
        Else
            'if txtCurrentPrice.text = nothing and txtNextPrice.Text <> nothing then
            '    txtCurrentPrice.text = txtNextPrice.text
            'end if
            ltrErrorMsg.Visible = False
            Try
                DB.BeginTransaction()
                If dt.Rows.Count > 0 Then

                    SQL = " UPDATE VendorProductPrice SET "
                    SQL &= "Updated = " & DB.Quote(Now)
                    If txtSku.Text <> String.Empty Then
                        SQL &= ",VendorSKU = " & DB.Quote(txtSku.Text)
                    End If
                    If txtCurrentPrice.Text <> String.Empty Then
                        SQL &= ",VendorPrice = " & DB.Number(Math.Round(Double.Parse(txtCurrentPrice.Text), 2))
                    End If
                    If txtNextPrice.Text <> String.Empty Then
                        SQL &= ",NextPrice = " & DB.Number(Math.Round(Double.Parse(txtNextPrice.Text), 2))
                        SQL &= ",NextPriceApplies = " & DB.Quote(ProductRow.GetPriceLockDate(DB, e.CommandArgument))
                    End If
                    SQL &= ",SubstituteQuantityMultiplier = " & DB.Number(0)
                    SQL &= ",IsUpload = " & CInt(False)
                    SQL &= ",UpdaterVendorAccountID = " & DB.Number(Session("VendorAccountId"))
                    SQL &= " WHERE VendorID = " & DB.Number(VendorId)
                    SQL &= " AND ProductID = " & DB.Number(e.CommandArgument)

                    DB.ExecuteSQL(SQL)

                Else

                    SQL = " INSERT INTO VendorProductPrice ("
                    SQL &= " ProductID"
                    SQL &= ",VendorID"
                    If txtSku.Text <> String.Empty Then
                        SQL &= ",VendorSKU"
                    End If
                    If txtCurrentPrice.Text <> String.Empty Then
                        SQL &= ",VendorPrice"
                    End If
                    If txtNextPrice.Text <> String.Empty Then
                        SQL &= ",NextPrice"
                        SQL &= ",NextPriceApplies"
                    End If
                    SQL &= ",IsSubstitution"
                    SQL &= ",IsDiscontinued"
                    SQL &= ",SubstituteQuantityMultiplier"
                    SQL &= ",IsUpload"
                    SQL &= ",Submitted"
                    SQL &= ",SubmitterVendorAccountID"
                    SQL &= ",Updated"
                    SQL &= ",UpdaterVendorAccountID"
                    SQL &= ") VALUES ("
                    SQL &= DB.Number(e.CommandArgument)
                    SQL &= "," & DB.Number(VendorId)
                    If txtSku.Text <> String.Empty Then
                        SQL &= "," & DB.Quote(txtSku.Text)
                    End If
                    If txtCurrentPrice.Text <> String.Empty Then
                        SQL &= "," & DB.Number(Math.Round(Double.Parse(txtCurrentPrice.Text), 2))
                    End If
                    If txtNextPrice.Text <> Nothing Then
                        SQL &= "," & DB.Number(Math.Round(Double.Parse(txtNextPrice.Text), 2))
                        SQL &= "," & DB.Quote(ProductRow.GetPriceLockDate(DB, e.CommandArgument))
                    End If
                    SQL &= "," & CInt(False)
                    SQL &= "," & CInt(False)
                    SQL &= "," & DB.Number(0)
                    SQL &= "," & CInt(False)
                    SQL &= "," & DB.Quote(Now)
                    SQL &= "," & DB.Number(Session("VendorAccountId"))
                    SQL &= "," & DB.Quote(Now)
                    SQL &= "," & DB.Number(Session("VendorAccountId"))
                    SQL &= ")"

                    DB.ExecuteSQL(SQL)

                End If

                If txtCurrentPrice.Text <> String.Empty AndAlso (dt.Rows.Count = 0 OrElse VendorPrice <> Double.Parse(txtCurrentPrice.Text)) Then
                    Dim dbHistory As New VendorProductPriceHistoryRow(DB)
                    dbHistory.IsSubstitution = False
                    dbHistory.IsUpload = False
                    dbHistory.ProductID = e.CommandArgument
                    dbHistory.SubmitterVendorAccountID = Session("VendorAccountId")
                    dbHistory.VendorID = Session("VendorId")
                    dbHistory.VendorPrice = txtCurrentPrice.Text
                    dbHistory.VendorSKU = txtSku.Text
                    dbHistory.Insert()
                End If

                If txtCurrentPrice.Text <> Nothing Then
                    VendorProductPriceRequestRow.RemoveRequests(DB, VendorId, e.CommandArgument)
                End If

                DB.CommitTransaction()
            Catch ex As Exception
                If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                ltrErrorMsg.Text = ErrHandler.ErrorText(ex)
            End Try
            VendorProductPriceRow.UpdateSubstitutions(DB, Session("VendorID"))

            ctlSearch.Search()

        End If

    End Sub

    Protected Sub rptProducts_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProducts.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        Dim imgReq As WebControls.Image = CType(e.Item.FindControl("imgReq"), WebControls.Image)
        Dim btnUpdate As Button = CType(e.Item.FindControl("btnUpdate"), Button)
        Dim txtCurrentPrice As TextBox = e.Item.FindControl("txtCurrentPrice")
        Dim txtNextPrice As TextBox = e.Item.FindControl("txtNextPrice")
        Dim txtSku As TextBox = e.Item.FindControl("txtSku")
        Dim ltlApplies As Literal = e.Item.FindControl("ltlApplies")
        Dim ltlSubstitute As Literal = e.Item.FindControl("ltlSubstitute")
        Dim btnSubstitute As LinkButton = e.Item.FindControl("btnSubstitute")
        Dim btnRemoveSub As LinkButton = e.Item.FindControl("btnRemoveSub")
        Dim btnDiscontinue As Button = e.Item.FindControl("btnDiscontinue")
        Dim ltlDiscontinued As Literal = e.Item.FindControl("ltlDiscontinued")
        Dim ltlDiscontinued2 As Literal = e.Item.FindControl("ltlDiscontinued2")

        If e.Item.DataItem.IsSubstitution Then
            txtCurrentPrice.Enabled = False
            txtCurrentPrice.Text = ""
            txtNextPrice.Enabled = False
            txtNextPrice.Text = ""
            txtSku.Enabled = False
            txtSku.Text = ""
            ltlSubstitute.Text = e.Item.DataItem.SubstituteProduct
            btnSubstitute.Text = "Change"
            btnRemoveSub.Visible = True
            btnDiscontinue.Visible = False
        Else
            btnSubstitute.Text = "Select Substitute"
            btnRemoveSub.Visible = False
            If e.Item.DataItem.IsDiscontinued Then
                ltlDiscontinued.Text = "<br/><span class=""smallest"">Product Discontinued</span>"
                If e.Item.DataItem.NextPriceApplies <> Nothing Then
                    ltlDiscontinued2.Text = "<br/><b>Discontinued until " & FormatDateTime(e.Item.DataItem.NextPriceApplies) & "</b>"
                Else
                    ltlDiscontinued2.Text = "<br/><b>Discontinued</b>"
                End If

                txtCurrentPrice.Text = ""
                txtCurrentPrice.Visible = False
                btnDiscontinue.Visible = False
                'btnSubstitute.Visible = False
            Else
                If IsNumeric(e.Item.DataItem.VendorPrice) Then
                    txtCurrentPrice.Text = FormatNumber(e.Item.DataItem.VendorPrice, 2)
                    btnDiscontinue.Visible = True
                    btnSubstitute.Visible = False
                Else
                    btnDiscontinue.Visible = False
                    btnSubstitute.Visible = True
                End If
            End If
            If Not IsDBNull(e.Item.DataItem.NextPrice) AndAlso e.Item.DataItem.NextPrice <> Nothing Then
                txtNextPrice.Text = FormatNumber(e.Item.DataItem.NextPrice, 2)
                ltlApplies.Text = FormatDateTime(e.Item.DataItem.NextPriceApplies, DateFormat.ShortDate)
            Else
                txtNextPrice.Text = Nothing
                ltlApplies.Text = Nothing
            End If
            btnSubstitute.Text = "(Click to Select)"
        End If

        If Not IsDBNull(e.Item.DataItem.Updated) Then
            If DateDiff("h", e.Item.DataItem.Updated, Now()) < 2 Then
                imgReq.ImageUrl = "/images/admin/True.gif"
                imgReq.Visible = True
            End If
        End If
    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        ctlSearch.Search()
        ltrErrorMsg.Visible = False
        'txtKeyword.Text = ""
    End Sub

    Public Sub ExportCSV()
        bExport = True
        ctlSearch.PageSize = 32000
        ctlSearch.Search()
    End Sub

    Public Sub ExportPriceCSV()
        bPriceExport = True
        ctlSearch.PageSize = 32000
        ctlSearch.Search()
    End Sub

    Private Sub SaveExport(ByVal q As IEnumerable)
        Dim SKU As String = String.Empty
        Dim Product As String = String.Empty
        Dim VendorSKU As String = String.Empty
        Dim VendorPrice As String = String.Empty
        Dim Updated As String = String.Empty

        Dim fname As String = "/assets/vendor/product/" & Core.GenerateFileID & ".csv"
        Dim sw As IO.StreamWriter = IO.File.CreateText(Server.MapPath(fname))
        sw.WriteLine("CBUSA SKU,Product Name,Vendor SKU,Vendor Price,Updated")
        For Each row As Object In q
            If Not IsDBNull(row.SKU) Then
                SKU = row.SKU
            Else
                SKU = ""
            End If
            If Not IsDBNull(row.Product) Then
                Product = row.Product
            Else
                Product = ""
            End If
            If Not IsDBNull(row.VendorSKU) Then
                VendorSKU = row.VendorSKU
            Else
                VendorSKU = ""
            End If
            If Not IsDBNull(row.VendorPrice) Then
                VendorPrice = row.VendorPrice
            Else
                VendorPrice = ""
            End If
            If Not IsDBNull(row.NextPrice) Then
                VendorPrice = row.NextPrice
            End If
            If Not IsDBNull(row.Updated) Then
                Updated = row.Updated
            Else
                Updated = ""
            End If
            sw.WriteLine(Core.QuoteCSV(SKU) & "," & Core.QuoteCSV(Product) & "," & Core.QuoteCSV(VendorSKU) & "," & Core.QuoteCSV(VendorPrice) & "," & Core.QuoteCSV(Updated))
        Next
        sw.Close()
        'ltrErrorMsg.Text = "<div style=""float:right;""><a href=""" & fname & """><b>Click here to download CSV file</b></a></div>"
        'ltrErrorMsg.Visible = True
        Response.Redirect(fname)
    End Sub

    Private Sub SavePriceExport(ByVal q As IEnumerable)
        Dim VendorSKU As String = String.Empty
        Dim VendorPrice As String = String.Empty
        Dim SKU As String = String.Empty
        Dim Product As String = String.Empty
        Dim Updated As String = String.Empty

        Dim fname As String = "/assets/vendor/product/" & Core.GenerateFileID & ".csv"
        Dim sw As IO.StreamWriter = IO.File.CreateText(Server.MapPath(fname))
        sw.WriteLine("Vendor SKU,Vendor Price,CBUSA SKU, Product Name, Updated")
        For Each row As Object In q
            If Not IsDBNull(row.VendorSKU) Then
                VendorSKU = row.VendorSKU
            Else
                VendorSKU = ""
            End If
            If Not IsDBNull(row.VendorPrice) Then
                VendorPrice = row.VendorPrice
            Else
                VendorPrice = ""
            End If
            If Not IsDBNull(row.NextPrice) Then
                VendorPrice = row.NextPrice
            End If
            If Not IsDBNull(row.SKU) Then
                SKU = row.SKU
            Else
                SKU = ""
            End If
            If Not IsDBNull(row.Product) Then
                Product = row.Product
            Else
                Product = ""
            End If
            If Not IsDBNull(row.Updated) Then
                Updated = row.Updated
            Else
                Updated = ""
            End If
            sw.WriteLine(Core.QuoteCSV(VendorSKU) & "," & Core.QuoteCSV(VendorPrice) & "," & Core.QuoteCSV(SKU) & "," & Core.QuoteCSV(Product) & "," & Core.QuoteCSV(Updated))
        Next
        sw.Close()
        'ltrErrorMsg.Text = "<div style=""float:right;""><a href=""" & fname & """><b>Click here to download CSV file</b></a></div>"
        'ltrErrorMsg.Visible = True
        Response.Redirect(fname)
    End Sub

    Protected Sub btnImport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnImport.Click
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

            fulDocument.Folder = "/assets/vendor/product/"
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

        Session("VendorImportFile") = NewFileName & OriginalExtension
        Try
            ImportCSV(Session("VendorImportFile"), False)
        Catch ex As Exception
            ltrErrorMsg.Text = Err.Description
            ltrErrorMsg.Visible = True
        End Try

    End Sub

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
            ImportPriceCSV(Session("VendorImportPriceFile"), False)
        Catch ex As Exception
            ltrErrorMsg.Text = Err.Description
            ltrErrorMsg.Visible = True
        End Try

    End Sub

    Protected Sub btnImportUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnImportUpdate.Click
        Try
            ImportCSV(Session("VendorImportFile"), True)
        Catch ex As Exception
            ltrErrorMsg.Text = Err.Description
            ltrErrorMsg.Visible = True
        End Try
    End Sub

    Protected Sub btnImportPriceUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnImportPriceUpdate.Click
        Try
            ImportPriceCSV(Session("VendorImportPriceFile"), True)
        Catch ex As Exception
            ltrErrorMsg.Text = Err.Description
            ltrErrorMsg.Visible = True
        End Try
    End Sub

    Private Sub ImportCSV(ByVal FileName As String, ByVal UpdateDatabase As Boolean)
        Dim aLine As String()
        Dim Count As Integer = 0
        Dim InsertCount As Integer = 0
        Dim UpdateCount As Integer = 0
        Dim BadCount As Integer = 0
        Dim SupplyPhases As String = GetVendorSupplyPhases(VendorId)
        Dim txtErr As String = String.Empty
        Dim tblErr As String = String.Empty
        Dim IsUpdate As Boolean = False
        Dim UpdateCurrent As Boolean = False
        Dim ProductId As Integer = 0
        Dim SKU As String = String.Empty
        Dim VendorSKU As String = String.Empty
        Dim VendorPrice As String = String.Empty
        Dim OldPrice As Double = Nothing

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

            If aLine.Length > 3 AndAlso aLine(0) <> String.Empty Then
                IsUpdate = False
                ProductId = 0
                SKU = Trim(Core.StripDblQuote(aLine(0)))
                VendorSKU = Trim(Core.StripDblQuote(aLine(2)))
                VendorPrice = Trim(Core.StripDblQuote(aLine(3)))
                If IsNumeric(VendorPrice) Then
                    VendorPrice = Math.Round(Double.Parse(VendorPrice), 2)
                End If
                If Count = 1 And SKU <> "CBUSA SKU" Then
                    ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
                    ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""><li>The file you uploaded does not appear to be in the correct format. The file format must be identical to the one used in the Export CSV process.</td></tr></table>"
                    Exit Sub
                End If

                If SKU = "CBUSA SKU" Then
                    Continue While
                End If

                txtErr = String.Empty

                'SKU cannot be longer than 20 char 
                If VendorSKU.Length > 20 Then
                    BadCount = BadCount + 1
                    txtErr &= "<li>Vendor SKU too long"
                    tblErr &= "<tr class=""red""><td>" & SKU & "</td><td>" & txtErr & "</td><tr>"
                    Continue While
                End If

                'Price must be a numeric value and greater than 0
                If VendorPrice <> String.Empty Then
                    If Not IsNumeric(VendorPrice) Then
                        BadCount = BadCount + 1
                        txtErr &= "<li>Invalid price"
                        tblErr &= "<tr class=""red""><td>" & SKU & "</td><td>" & txtErr & "</td><tr>"
                        Continue While
                    ElseIf VendorPrice < 0 Then
                        BadCount = BadCount + 1
                        txtErr &= "<li>Negative price"
                        tblErr &= "<tr class=""red""><td>" & SKU & "</td><td>" & txtErr & "</td><tr>"
                        Continue While
                    End If
                End If
                If txtErr = String.Empty Then
                    Dim SQL As String = "SELECT Top 1 p.ProductID, p.Product, p.SKU, vpp.VendorSKU, Cast(vpp.VendorPrice As Money) As VendorPrice, vpp.IsSubstitution, vpp.IsDiscontinued, vpp.Updated "
                    SQL &= "FROM Product p INNER JOIN "
                    SQL &= "SupplyPhaseProduct spp ON p.ProductID = spp.ProductID LEFT OUTER JOIN "
                    SQL &= "(Select * From VendorProductPrice Where VendorId = " & DB.Number(VendorId) & ") vpp ON p.ProductID = vpp.ProductID "
                    SQL &= "Where "
                    'SQL &= "spp.SupplyPhaseID In " & DB.NumberMultiple(SupplyPhases)
                    SQL &= " p.SKU = " & DB.Quote(SKU)

                    Dim dtProduct As DataTable = DB.GetDataTable(SQL)

                    'Product must exist
                    If dtProduct.Rows.Count = 0 Then
                        BadCount = BadCount + 1
                        txtErr &= "<li>Product Not Found!"
                        tblErr &= "<tr class=""red""><td>" & SKU & "</td><td>" & txtErr & "</td><tr>"
                        Continue While
                    Else
                        For Each dr As DataRow In dtProduct.Rows
                            ProductId = dr("ProductId")
                            Dim dt As DataTable = DB.GetDataTable("Select Top 1 * From VendorProductPrice Where VendorId = " & DB.Number(VendorId) & " And ProductId = " & DB.Number(ProductId))
                            IsUpdate = dt.Rows.Count > 0
                            Dim dtLLC As DataTable = DB.GetDataTable("Select l.* From LLC l, LLCProductPriceRequirement lp, LLCVendor lv Where l.LLCID = lp.LLCID And l.LLCID = lv.LLCID And lp.ProductId = " & DB.Number(ProductId) & " And lv.VendorId = " & DB.Number(VendorId))
                            Dim PriceRequired As Boolean = dtLLC.Rows.Count > 0

                            If Not IsDBNull(dr("VendorPrice")) Then
                                'Vendor price cannot be greater than current (NOT ANYMORE)
                                'If dr("VendorPrice") > 0 And VendorPrice > dr("VendorPrice") Then
                                'txtErr &= "<li>New price is greater than current"
                                'End If

                                UpdateCurrent = False

                            ElseIf PriceRequired And VendorPrice = String.Empty Then
                                txtErr &= "<li>Price required"
                            ElseIf VendorPrice <> String.Empty Then
                                UpdateCurrent = True
                            End If

                    'Vendor SKU is required if not already provided
                    If VendorSKU = String.Empty Then
                        If SKU = String.Empty Then
                            txtErr &= "<li>Vendor SKU empty"
                        End If
                    End If
                        Next
                    End If
                End If

                If txtErr <> String.Empty Then
                    BadCount = BadCount + 1
                    tblErr &= "<tr class=""red""><td>" & SKU & "</td><td>" & txtErr & "</td><tr>"
                ElseIf Not UpdateDatabase Then
                    If IsUpdate Then
                        UpdateCount = UpdateCount + 1
                    Else
                        InsertCount = InsertCount + 1
                    End If
                End If
            Else
                BadCount = BadCount + 1
                tblErr &= "<tr class=""red""><td>Row: " & Count & "</td><td>BAD ROW</td><tr>"
                Continue While
            End If

            If txtErr = String.Empty And UpdateDatabase Then

                Try
                    DB.BeginTransaction()
                    If IsUpdate Then
                        SQL = " UPDATE VendorProductPrice SET "
                        SQL &= "Updated = " & DB.Quote(Now)
                        If SKU <> String.Empty Then
                            SQL &= ",VendorSKU = " & DB.Quote(VendorSKU)
                        End If
                        If VendorPrice <> String.Empty Then
                            If UpdateCurrent Then
                                SQL &= ",VendorPrice = " & DB.Number(VendorPrice)
                            Else
                                SQL &= ",NextPrice = " & DB.Number(VendorPrice)
                                SQL &= ",NextPriceApplies = " & DB.Quote(Now())
                            End If
                        End If
                        SQL &= ",SubstituteQuantityMultiplier = " & DB.Number(0)
                        SQL &= ",IsUpload = " & CInt(True)
                        SQL &= ",UpdaterVendorAccountID = " & DB.Number(Session("VendorAccountId"))
                        SQL &= " WHERE VendorID = " & DB.Number(VendorId)
                        SQL &= " AND ProductId = " & DB.Quote(ProductId)

                        DB.ExecuteSQL(SQL)

                        UpdateCount = UpdateCount + 1

                    Else

                        SQL = " INSERT INTO VendorProductPrice ("
                        SQL &= " ProductID"
                        SQL &= ",VendorID"
                        SQL &= ",VendorSKU"
                        SQL &= ",VendorPrice"
                        SQL &= ",SubstituteQuantityMultiplier"
                        SQL &= ",IsUpload"
                        SQL &= ",Submitted"
                        SQL &= ",SubmitterVendorAccountID"
                        SQL &= ",Updated"
                        SQL &= ",UpdaterVendorAccountID"
                        SQL &= ") VALUES ("
                        SQL &= DB.Number(ProductId)
                        SQL &= "," & DB.Number(VendorId)
                        SQL &= "," & DB.Quote(VendorSKU)
                        SQL &= "," & DB.Number(VendorPrice)
                        SQL &= "," & DB.Number(0)
                        SQL &= "," & CInt(True)
                        SQL &= "," & DB.Quote(Now)
                        SQL &= "," & DB.Number(Session("VendorAccountId"))
                        SQL &= "," & DB.Quote(Now)
                        SQL &= "," & DB.Number(Session("VendorAccountId"))
                        SQL &= ")"

                        DB.ExecuteSQL(SQL)

                        InsertCount = InsertCount + 1

                    End If

                    If UpdateCurrent Then
                        Dim dbHistory As New VendorProductPriceHistoryRow(DB)
                        dbHistory.IsSubstitution = False
                        dbHistory.IsUpload = False
                        dbHistory.ProductID = ProductId
                        dbHistory.SubmitterVendorAccountID = Session("VendorAccountId")
                        dbHistory.VendorID = Session("VendorId")
                        dbHistory.VendorPrice = VendorPrice
                        dbHistory.VendorSKU = VendorSKU
                        dbHistory.Insert()
                    End If

                    If VendorPrice <> Nothing Then
                        VendorProductPriceRequestRow.RemoveRequests(DB, VendorId, ProductId)
                    End If

                    DB.CommitTransaction()
                Catch ex As Exception
                    If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                    ltrErrorMsg.Text = ErrHandler.ErrorText(ex)
                    btnImportUpdate.Visible = False
                    Exit Sub
                End Try
            End If
        End While
        f.Close()

        VendorProductPriceRow.UpdateSubstitutions(DB, Session("VendorId"))

        Count = Count - 1

        tblErr = "<tr class=""red bold""><td>Invalid Entries</td><td>" & BadCount & "</td><tr>" & tblErr

        If UpdateDatabase Then
            tblErr = "<tr class=""green""><td>Products Added</td><td>" & InsertCount & "</td><tr>" & tblErr
            tblErr = "<tr class=""green""><td>Products Updated</td><td>" & UpdateCount & "</td><tr>" & tblErr
            ltrErrorMsg.Text = "<b>Data import process completed with success. Please review the report below for more details.</b>"
            btnImportUpdate.Visible = False
            ctlSearch.Search()
        Else
            tblErr = "<tr class=""green""><td>Products To Add</td><td>" & InsertCount & "</td><tr>" & tblErr
            tblErr = "<tr class=""green""><td>Products To Update</td><td>" & UpdateCount & "</td><tr>" & tblErr
            btnImportUpdate.Visible = True
            ltrErrorMsg.Text = "<b>File uploaded successfully. Please review the report below and confirm the data import.</b>"
        End If

        tblErr = "<tr class=""green""><td>Valid Entries</td><td>" & Count - BadCount & "</td><tr>" & tblErr
        tblErr = "<tr class=""blue bold""><td>Total Rows Processed</td><td>" & Count & "</td><tr>" & tblErr

        ltrErrorMsg.Text &= "<hr><table align=""center"">" & tblErr & "</table><hr>"
        ltrErrorMsg.Visible = True

    End Sub

    Private Sub ImportPriceCSV(ByVal FileName As String, ByVal UpdateDatabase As Boolean)
        Dim aLine As String()
        Dim Count As Integer = 0
        Dim InsertCount As Integer = 0
        Dim UpdateCount As Integer = 0
        Dim BadCount As Integer = 0
        Dim SupplyPhases As String = GetVendorSupplyPhases(VendorId)
        Dim txtErr As String = String.Empty
        Dim tblErr As String = String.Empty
        Dim IsUpdate As Boolean = False
        Dim UpdateCurrent As Boolean = False
        Dim ProductId As Integer = 0
        Dim VendorSKU As String = String.Empty
        Dim VendorPrice As String = String.Empty
        
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


            If aLine.Length > 1 AndAlso aLine(0) <> String.Empty Then
                IsUpdate = False
                ProductId = 0
                VendorSKU = Trim(Core.StripDblQuote(aLine(0)))
                VendorPrice = Trim(Core.StripDblQuote(aLine(1)))
                If IsNumeric(VendorPrice) Then
                    VendorPrice = Math.Round(Double.Parse(VendorPrice), 2)
                End If
                If Count = 1 And VendorSKU <> "Vendor SKU" Then
                    ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
                    ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""><li>The file you uploaded does not appear to be in the correct format. The file format must be identical to the one used in the Export CSV process.</td></tr></table>"
                    Exit Sub
                End If

                If VendorSKU = "Vendor SKU" Then
                    Continue While
                End If

                txtErr = String.Empty

                'SKU cannot be longer than 20 char 
                If VendorSKU.Length > 20 Then
                    BadCount = BadCount + 1
                    txtErr &= "<li>Vendor SKU too long"
                    tblErr &= "<tr class=""red""><td>" & VendorSKU & "</td><td>" & txtErr & "</td><tr>"
                    Continue While
                End If

                'Price must be a numeric value and greater than 0
                If VendorPrice <> String.Empty Then
                    If Not IsNumeric(VendorPrice) Then
                        BadCount = BadCount + 1
                        txtErr &= "<li>Invalid price"
                        tblErr &= "<tr class=""red""><td>" & VendorSKU & "</td><td>" & txtErr & "</td><tr>"
                        Continue While
                    ElseIf VendorPrice < 0 Then
                        BadCount = BadCount + 1
                        txtErr &= "<li>Negative price"
                        tblErr &= "<tr class=""red""><td>" & VendorSKU & "</td><td>" & txtErr & "</td><tr>"
                        Continue While
                    End If
                End If
                If txtErr = String.Empty Then
                    Dim SQL As String = "SELECT Top 1 p.ProductID, p.Product, p.SKU, vpp.VendorSKU, Cast(vpp.VendorPrice As Money) As VendorPrice, vpp.IsSubstitution, vpp.IsDiscontinued, vpp.Updated "
                    SQL &= "FROM Product p INNER JOIN "
                    SQL &= "SupplyPhaseProduct spp ON p.ProductID = spp.ProductID INNER JOIN "
                    SQL &= "VendorProductPrice vpp ON p.ProductID = vpp.ProductID "
                    SQL &= "Where vpp.VendorSKU Is Not Null "
                    SQL &= " and spp.SupplyPhaseID In " & DB.NumberMultiple(SupplyPhases)
                    SQL &= " and vpp.VendorId = " & DB.Number(VendorId)
                    SQL &= " and vpp.VendorSKU = " & DB.Quote(VendorSKU)

                    Dim dtProduct As DataTable = DB.GetDataTable(SQL)

                    'Product must exist
                    If dtProduct.Rows.Count = 0 Then
                        BadCount = BadCount + 1
                        txtErr &= "<li>Product Not Found!"
                        tblErr &= "<tr class=""red""><td>" & VendorSKU & "</td><td>" & txtErr & "</td><tr>"
                        Continue While
                    Else
                        For Each dr As DataRow In dtProduct.Rows
                            ProductId = dr("ProductId")
                            Dim dt As DataTable = DB.GetDataTable("Select Top 1 * From VendorProductPrice Where VendorId = " & DB.Number(VendorId) & " And ProductId = " & DB.Number(ProductId))
                            IsUpdate = dt.Rows.Count > 0
                            Dim dtLLC As DataTable = DB.GetDataTable("Select l.* From LLC l, LLCProductPriceRequirement lp, LLCVendor lv Where l.LLCID = lp.LLCID And l.LLCID = lv.LLCID And lp.ProductId = " & DB.Number(ProductId) & " And lv.VendorId = " & DB.Number(VendorId))
                            Dim PriceRequired As Boolean = dtLLC.Rows.Count > 0

                            If Not IsDBNull(dr("VendorPrice")) And VendorPrice <> String.Empty Then
                                'Vendor price cannot be greater than current (NOT ANYMORE)
                                'If dr("VendorPrice") > 0 And VendorPrice > dr("VendorPrice") Then
                                '    txtErr &= "<li>New price is greater than current"
                                'End If

                                UpdateCurrent = False

                            ElseIf PriceRequired And VendorPrice = String.Empty Then
                                txtErr &= "<li>Price required"
                            ElseIf VendorPrice <> String.Empty Then
                                UpdateCurrent = True
                            End If
                        Next
                    End If
                End If

                If txtErr <> String.Empty Then
                    BadCount = BadCount + 1
                    tblErr &= "<tr class=""red""><td>" & VendorSKU & "</td><td>" & txtErr & "</td><tr>"
                ElseIf Not UpdateDatabase Then
                    If IsUpdate Then
                        UpdateCount = UpdateCount + 1
                    Else
                        InsertCount = InsertCount + 1
                    End If
                End If
            Else
                BadCount = BadCount + 1
                tblErr &= "<tr class=""red""><td>Row: " & Count & "</td><td>BAD ROW</td><tr>"
                Continue While
            End If

            If txtErr = String.Empty And UpdateDatabase Then

                Try
                    DB.BeginTransaction()
                    If IsUpdate Then
                        SQL = " UPDATE VendorProductPrice SET "
                        SQL &= "Updated = " & DB.Quote(Now)
                        If VendorPrice <> String.Empty Then
                            If UpdateCurrent Then
                                SQL &= ",VendorPrice = " & DB.Number(VendorPrice)
                            Else
                                SQL &= ",NextPrice = " & DB.Number(VendorPrice)
                                SQL &= ",NextPriceApplies = " & DB.Quote(Now())
                            End If
                        End If
                        SQL &= ",IsUpload = " & CInt(True)
                        SQL &= ",UpdaterVendorAccountID = " & DB.Number(Session("VendorAccountId"))
                        SQL &= " WHERE VendorID = " & DB.Number(VendorId)
                        SQL &= " AND ProductId = " & DB.Quote(ProductId)

                        DB.ExecuteSQL(SQL)

                        UpdateCount = UpdateCount + 1

                    Else

                        SQL = " INSERT INTO VendorProductPrice ("
                        SQL &= " ProductID"
                        SQL &= ",VendorID"
                        SQL &= ",VendorSKU"
                        SQL &= ",VendorPrice"
                        SQL &= ",SubstituteQuantityMultiplier"
                        SQL &= ",IsUpload"
                        SQL &= ",Submitted"
                        SQL &= ",SubmitterVendorAccountID"
                        SQL &= ",Updated"
                        SQL &= ",UpdaterVendorAccountID"
                        SQL &= ") VALUES ("
                        SQL &= DB.Number(ProductId)
                        SQL &= "," & DB.Number(VendorId)
                        SQL &= "," & DB.Quote(VendorSKU)
                        SQL &= "," & DB.Number(VendorPrice)
                        SQL &= "," & DB.Number(0)
                        SQL &= "," & CInt(True)
                        SQL &= "," & DB.Quote(Now)
                        SQL &= "," & DB.Number(Session("VendorAccountId"))
                        SQL &= "," & DB.Quote(Now)
                        SQL &= "," & DB.Number(Session("VendorAccountId"))
                        SQL &= ")"

                        DB.ExecuteSQL(SQL)

                        InsertCount = InsertCount + 1

                    End If

                    If UpdateCurrent Then
                        Dim dbHistory As New VendorProductPriceHistoryRow(DB)
                        dbHistory.IsSubstitution = False
                        dbHistory.IsUpload = False
                        dbHistory.ProductID = ProductId
                        dbHistory.SubmitterVendorAccountID = Session("VendorAccountId")
                        dbHistory.VendorID = Session("VendorId")
                        dbHistory.VendorPrice = VendorPrice
                        dbHistory.VendorSKU = VendorSKU
                        dbHistory.Insert()
                    End If

                    If VendorPrice <> Nothing Then
                        VendorProductPriceRequestRow.RemoveRequests(DB, VendorId, ProductId)
                    End If

                    DB.CommitTransaction()
                Catch ex As Exception
                    If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                    ltrErrorMsg.Text = ErrHandler.ErrorText(ex)
                    btnImportPriceUpdate.Visible = False
                    Exit Sub
                End Try
            End If
        End While
        f.Close()

        VendorProductPriceRow.UpdateSubstitutions(DB, Session("VendorId"))

        Count = Count - 1

        tblErr = "<tr class=""red bold""><td>Invalid Entries</td><td>" & BadCount & "</td><tr>" & tblErr

        If UpdateDatabase Then
            tblErr = "<tr class=""green""><td>Products Added</td><td>" & InsertCount & "</td><tr>" & tblErr
            tblErr = "<tr class=""green""><td>Products Updated</td><td>" & UpdateCount & "</td><tr>" & tblErr
            ltrErrorMsg.Text = "<b>Data import process completed with success. Please review the report below for more details.</b>"
            btnImportPriceUpdate.Visible = False
            ctlSearch.Search()
        Else
            tblErr = "<tr class=""green""><td>Products To Add</td><td>" & InsertCount & "</td><tr>" & tblErr
            tblErr = "<tr class=""green""><td>Products To Update</td><td>" & UpdateCount & "</td><tr>" & tblErr
            btnImportPriceUpdate.Visible = True
            ltrErrorMsg.Text = "<b>File uploaded successfully. Please review the report below and confirm the data import.</b>"
        End If

        tblErr = "<tr class=""green""><td>Valid Entries</td><td>" & Count - BadCount & "</td><tr>" & tblErr
        tblErr = "<tr class=""blue bold""><td>Total Rows Processed</td><td>" & Count & "</td><tr>" & tblErr

        ltrErrorMsg.Text &= "<hr><table align=""center"">" & tblErr & "</table><hr>"
        ltrErrorMsg.Visible = True

    End Sub

    Protected Sub btnSaveAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveAll.Click
        Dim cnt As Integer = 1
        DB.BeginTransaction()
        Try
            For Each item As RepeaterItem In rptProducts.Items
                Dim txtSku As TextBox = item.FindControl("txtSku")
                Dim txtCurrentPrice As TextBox = item.FindControl("txtCurrentPrice")
                Dim txtNextPrice As TextBox = item.FindControl("txtNextPrice")
                Dim btnUpdate As Button = item.FindControl("btnUpdate")

                If txtSku.Text <> Nothing Or txtCurrentPrice.Text <> Nothing Or txtNextPrice.Text <> Nothing Then
                    Dim p As VendorProductPriceRow = VendorProductPriceRow.GetRow(DB, Session("VendorId"), btnUpdate.CommandArgument)

                    Dim sku As String = txtSku.Text
                    Dim price As String = Regex.Replace(txtCurrentPrice.Text, "[^\d.]", "")
                    Dim nextPrice As String = Regex.Replace(txtNextPrice.Text, "[^\d.]", "")

                    Dim UpdateHistory As Boolean = False

                    If price = Nothing And txtCurrentPrice.Text <> Nothing Then
                        AddError("Price is Invalid (Row #" & cnt & ")")
                        Continue For
                    ElseIf p.VendorPrice <> Nothing AndAlso (price = Nothing OrElse Convert.ToDouble(price) > p.VendorPrice) Then
                        AddError("New Current Price must be greater than or equal to Current Price")
                        Continue For
                    End If
                    If nextPrice = Nothing AndAlso txtNextPrice.Text <> Nothing Then
                        AddError("Next Price is Invalid (Row #" & cnt & ")")
                        Continue For
                    End If

                    p.VendorID = Session("VendorId")
                    If txtCurrentPrice.Text <> Nothing Then
                        If p.VendorPrice <> price Then
                            UpdateHistory = True
                        End If
                        p.VendorPrice = txtCurrentPrice.Text
                    ElseIf txtNextPrice.Text <> Nothing Then
                        p.VendorPrice = txtNextPrice.Text
                    End If

                    If txtNextPrice.Text <> Nothing AndAlso txtNextPrice.Text <> p.NextPrice Then
                        p.NextPrice = Math.Round(Double.Parse(txtNextPrice.Text), 2)
                        p.NextPriceApplies = ProductRow.GetPriceLockDate(DB, btnUpdate.CommandArgument)
                    End If
                    p.VendorSKU = sku
                    p.IsDiscontinued = False
                    p.IsSubstitution = False
                    p.IsUpload = False
                    p.ProductID = btnUpdate.CommandArgument
                    p.SubmitterVendorAccountID = Session("VendorAccountId")
                    p.UpdaterVendorAccountID = Session("VendorAccoutnId")

                    If p.Submitted = Nothing Then
                        p.Insert()
                    Else
                        p.Update()
                    End If

                    If UpdateHistory Then
                        Dim dbHistory As New VendorProductPriceHistoryRow(DB)
                        dbHistory.IsSubstitution = False
                        dbHistory.IsUpload = False
                        dbHistory.ProductID = p.ProductID
                        dbHistory.SubmitterVendorAccountID = Session("VendorAccountId")
                        dbHistory.VendorID = Session("VendorId")
                        dbHistory.VendorPrice = p.VendorPrice
                        dbHistory.VendorSKU = p.VendorSKU

                        dbHistory.Insert()
                    End If

                    If p.VendorPrice <> Nothing Then
                        VendorProductPriceRequestRow.RemoveRequests(DB, VendorId, p.ProductID)
                    End If

                End If
            Next

            DB.CommitTransaction()
        Catch ex As SqlClient.SqlException
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try

        Try
            ctlSearch.Search()
        Catch ex As Exception
            ctlSearch.PageNumber = 1
            ctlNavigator.PageNumber = 1
            ctlSearch.Search()
        End Try
    End Sub

    Protected Sub ctlNavigator_NavigatorEvent(ByVal sender As Object, ByVal e As Controls.NavigatorEventArgs) Handles ctlNavigator.NavigatorEvent
        ctlNavigator.PageNumber = e.PageNumber
        ctlSearch.PageNumber = e.PageNumber
        ctlSearch.Search()
    End Sub


#Region "Substitutions"
    Protected Sub frmSelect_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim form As PopupForm.PopupForm = sender
        Dim item As RepeaterItem = form.NamingContainer
        If item.DataItem Is Nothing Then Exit Sub

        CType(form.FindControl("ltlProductName"), Literal).Text = item.DataItem.ProductName
    End Sub

    Protected Sub rptResults_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptResults.ItemCreated
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        'Dim form As PopupForm.PopupForm = e.Item.FindControl("frmSelect")
        'AddHandler form.TemplateLoaded, AddressOf frmSelect_TemplateLoaded
        'AddHandler form.Postback, AddressOf frmSelect_Postback
        Dim btnSelect As Button = e.Item.FindControl("btnSelect")
        AddHandler btnSelect.Click, AddressOf btnSelect_Click
        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btnSelect)
    End Sub

    Protected Sub btnSelect_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim ProductID As Integer = CType(sender, Button).CommandArgument
        Dim dbSubstitute As ProductRow = ProductRow.GetRow(DB, ProductID)

        Dim dbRequested As ProductRow = ProductRow.GetRow(DB, hdnProductID.Value)
        ltlProductName.Text = dbSubstitute.Product
        ltlRequestedProduct.Text = dbRequested.Product
        spanQuantity.InnerHtml = 1
        txtQuantity.Text = 1
        txtQuantity.Attributes.Add("onchange", "UpdateTotal(event);")
        spanRecommendedQty.InnerHtml = spanQuantity.InnerHtml
        trRecommendedQty.Visible = True
        hdnSubstituteID.Value = dbSubstitute.ProductID

        ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenSelectForm", "Sys.Application.add_load(SwapPopups)", True)
        upSelect.Update()
    End Sub


    'Private Function GetValue(ByVal drv As DataRowView, ByVal field As String) As Object
    '    If drv Is Nothing Then
    '        Return Nothing
    '    Else
    '        Return drv(field)
    '    End If
    'End Function

    Private Sub BindSearchData(ByVal tbl As DataTable, ByVal count As Integer)

        If hdnProductID.Value <> Nothing Then
            Dim dbRequestProduct As ProductRow = ProductRow.GetRow(DB, hdnProductID.Value)
            spanSubHeaderProduct.InnerHtml = dbRequestProduct.Product
        End If

        If rblSubPricedOnly.SelectedValue = True Then
            Dim q = From sr As DataRow In tbl.AsEnumerable Join vp As DataRowView In dvVendorPrices On Convert.ToInt32(sr("ProductId")) Equals Convert.ToInt32(vp("ProductId")) Select New With {.ProductID = sr("ProductId"), .VendorSku = vp("VendorSku"), .ProductSku = sr("SKU"), .ProductName = sr("ProductName"), .Description = sr("Description"), .UnitOfMeasure = sr("SizeUnitOfMeasureText")}
            Dim tblq = (From sr As DataRow In tbl.AsEnumerable Select sr("ProductID"))
            Dim vpq = (From vp As DataRowView In dvVendorPrices Select vp("ProductID"))
            rptResults.DataSource = q
            ctlSubNavigator.NofRecords = count
        Else
            Dim q = From sr As DataRow In tbl.AsEnumerable Group Join vp As DataRowView In dvVendorPrices On Convert.ToInt32(sr("ProductId")) Equals Convert.ToInt32(vp("ProductId")) Into grp = Group _
                    From vp As DataRowView In grp.DefaultIfEmpty Select New With {.ProductID = sr("ProductId"), .VendorSku = GetValue(vp, "VendorSku"), .ProductSku = sr("SKU"), .ProductName = sr("ProductName"), .Description = sr("Description"), .UnitOfMeasure = sr("SizeUnitOfMeasureText")}
            rptResults.DataSource = q
            ctlSubNavigator.NofRecords = count
        End If
        If ctlSubNavigator.NofRecords = 0 Then
            ltlSubNoResults.Visible = True
            ctlSubNavigator.Visible = False
            rptResults.Visible = False
        Else
            ltlSubNoResults.Visible = False
            ctlSubNavigator.DataBind()
            rptResults.Visible = True
            rptResults.DataBind()
        End If
    End Sub

    Protected Sub ctlSubSearch_ResultsUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlSubSearch.ResultsUpdated
        BindSearchData(ctlSubSearch.SearchResults.ds.Tables(0), ctlSubSearch.SearchResults.Count)
        ltlSubBreadcrumbs.Text = ctlSubSearch.Breadcrumbs
        upResults.Update()
    End Sub

    Protected Sub btnSubSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubSearch.Click
        ctlSubSearch.Search()
    End Sub

    Protected Sub SubFilterListCallback(ByRef list As Generic.List(Of String))
        list.AddRange(From vp As DataRowView In dvVendorPrices Where Core.GetString(vp("ProductID")) <> hdnProductID.Value Select Convert.ToString(vp.Item("ProductId")))
    End Sub

    Protected Sub rblSubPricedOnly_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblSubPricedOnly.SelectedIndexChanged
        If rblSubPricedOnly.SelectedValue = False Then
            'ctlSubSearch.FilterListCallback = Nothing
            ctlSubSearch.FilterList = Nothing
        Else
            'ctlSubSearch.FilterListCallback = AddressOf SubFilterListCallback
            Dim list As New Generic.List(Of String)
            SubFilterListCallback(list)
            ctlSubSearch.FilterList = list
        End If
        ctlSubSearch.TreeOnlyFilter = Nothing
        ctlSubSearch.Search(True)
    End Sub

    Protected Sub ctlSubNavigator_NavigatorEvent(ByVal sender As Object, ByVal e As Controls.NavigatorEventArgs) Handles ctlSubNavigator.NavigatorEvent
        ctlSubNavigator.PageNumber = e.PageNumber
        ctlSubSearch.PageNumber = e.PageNumber
        ctlSubSearch.Search()
    End Sub

    Protected Sub frmSelect_Postback1(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmSelect.Postback
        Dim ProductID As Integer = hdnSubstituteID.Value
        Dim dbSubstitute As ProductRow = ProductRow.GetRow(DB, ProductID)
        Dim dbRequestPrice As VendorProductPriceRow = VendorProductPriceRow.GetRow(DB, Session("VendorID"), hdnProductID.Value)
        Dim delete As Boolean = False

        Dim dbPrice As VendorProductSubstituteRow = VendorProductSubstituteRow.GetRow(DB, Session("VendorId"), hdnProductID.Value)
        dbPrice.ProductID = hdnProductID.Value
        dbPrice.QuantityMultiplier = txtQuantity.Text
        dbPrice.SubstituteProductID = dbSubstitute.ProductID
        dbPrice.VendorID = Session("VendorId")
        If dbPrice.Created = Nothing Then
            dbPrice.CreatorVendorAccountID = Session("VendorAccountId")
            dbPrice.Insert()
        Else
            dbPrice.Update()
        End If

        dbRequestPrice.IsSubstitution = True
        dbRequestPrice.IsDiscontinued = False
        dbRequestPrice.ProductID = hdnProductID.Value
        dbRequestPrice.SubstituteQuantityMultiplier = txtQuantity.Text
        dbRequestPrice.VendorID = Session("VendorId")
        dbRequestPrice.VendorPrice = Nothing
        dbRequestPrice.VendorSKU = Nothing
        dbRequestPrice.UpdaterVendorAccountID = Session("VendorAccountId")
        If dbRequestPrice.Submitted <> Nothing Then
            dbRequestPrice.Update()
        Else
            dbRequestPrice.SubmitterVendorAccountID = Session("VendorAccountId")
            dbRequestPrice.Insert()
        End If
        ScriptManager.RegisterStartupScript(Page, Me.GetType, "CloseSelect", "Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(CloseSelectForm)", True)
        m_VendorPrices = Nothing
        ctlSearch.Search()
        upProducts.Update()
    End Sub
#End Region

    Protected Sub frmConfirmRemoveSub_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmConfirmRemoveSub.Postback
        Dim dbProduct As VendorProductPriceRow = VendorProductPriceRow.GetRow(DB, Session("VendorId"), hdnRemoveSubProductID.Value)
        Dim dbSubstitute As VendorProductSubstituteRow = VendorProductSubstituteRow.GetRow(DB, Session("VendorId"), hdnRemoveSubProductID.Value)
        dbProduct.IsSubstitution = False
        dbProduct.Update()
        dbSubstitute.Remove()
        dbProduct.Remove()
        m_VendorPrices = Nothing
        ctlSearch.Search()
    End Sub

	Protected Sub frmConfirmDiscontinue_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmConfirmDiscontinue.Postback
		Try
			DB.BeginTransaction()

			Dim dbProduct As VendorProductPriceRow = VendorProductPriceRow.GetRow(DB, Session("VendorId"), hdnDiscontinueProductID.Value)
			dbProduct.IsDiscontinued = True
			dbProduct.VendorPrice = Nothing
			dbProduct.VendorSKU = Nothing
			If dbProduct.NextPriceApplies = Nothing Then
				dbProduct.NextPriceApplies = ProductRow.GetPriceLockDate(DB, dbProduct.ProductID)
			End If
            dbProduct.Update()
            VendorProductPriceRow.UpdateSubstitutions(DB, dbProduct.VendorID)
            m_VendorPrices = Nothing
            ctlSearch.Search()

			DB.CommitTransaction()
		Catch ex As Exception
			DB.RollbackTransaction()
		End Try
	End Sub

    Protected Sub rblFilter_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblFilter.SelectedIndexChanged
        m_TreeFilterList = Nothing
        ctlSearch.TreeOnlyFilter = TreeFilterList
        ctlSearch.PageNumber = 1
        ctlSearch.Search(True)
    End Sub
End Class
