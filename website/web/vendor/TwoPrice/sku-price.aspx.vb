Imports Components
Imports DataLayer
Imports System.IO
Imports TwoPrice.DataLayer
Imports Utility
Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Data.SqlClient
Imports System.Configuration

Partial Class twoskuprice
    Inherits SitePage

    Protected VendorId As Integer
    Protected TwoPriceTakeOffId As Integer
    Protected UnsavedPricing As New Dictionary(Of Integer, Decimal)
    Protected dbVendor As VendorRow
    Private bExport As Boolean = False
    Private bPriceExport As Boolean = False
    Protected Declined As Boolean = False
    Private TotalPrice As Decimal = 0
    Private TotalQuantity As Integer = 0
    Private StrQryLog As String = ""
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private TwoPriceCampaignId As String = ""

    Private _dbTwoPriceCampaign As TwoPriceCampaignRow
    Protected ReadOnly Property dbTwoPriceCampaign As TwoPriceCampaignRow
        Get
            If _dbTwoPriceCampaign Is Nothing Then
                _dbTwoPriceCampaign = TwoPriceCampaignRow.GetRow(DB, dbTwoPriceTakeOff.TwoPriceCampaignId)
            End If
            Return _dbTwoPriceCampaign
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

    Private _BidSubmitted As Boolean = Nothing
    Protected ReadOnly Property BidSubmitted As Boolean
        Get
            If _BidSubmitted = Nothing Then
                Dim Sql As String = "SELECT COUNT(*) FROM TwoPriceVendorProductPrice WHERE VendorId= " & DB.Number(VendorId) & " AND TwoPriceCampaignId = " & dbTwoPriceCampaign.TwoPriceCampaignId & " AND Submitted = 1"
                _BidSubmitted = DB.ExecuteScalar(Sql) > 0
            End If
            Return _BidSubmitted
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

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Try
            VendorId = CType(Session("VendorId"), Integer)
            TwoPriceTakeOffId = CType(Request("TwoPriceTakeOffId"), Integer)
        Catch Ex As Exception
            Response.Redirect("/vendor/twoprice/")
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not (IsLoggedInVendor() Or Session("AdminId") IsNot Nothing) Then
            Response.Redirect("/default.aspx")
        End If
        If TwoPriceTakeOffId = Nothing Or (dbTwoPriceCampaign.TwoPriceCampaignId = Nothing) Then
            Response.Redirect("default.aspx")
        End If

        If BidSubmitted Or (dbTwoPriceCampaign.Status = "Awarded" And dbTwoPriceCampaign.IsUpdatePrice = True And dbTwoPriceCampaign.AwardedVendorId = Session("VendorId")) Then
            hdnAllowPriceUpdate.Value = 0
            BindPriceHistoryData()
            Dim raw As Object = DB.ExecuteScalar("SELECT TOP 1 LastUpdated FROM TwoPriceVendorProductPrice WHERE VendorId= " & DB.Number(VendorId) & " AND TwoPriceCampaignId = " & dbTwoPriceCampaign.TwoPriceCampaignId & " AND Submitted = 1")
            If Not IsDBNull(raw) Then
                Dim dateSubmitted As DateTime = CType(raw, DateTime)
                ltlDate.Text = dateSubmitted.ToString("dddd, MMM dd yyyy 'at' h:mmtt")
            End If

            btnDecline.Visible = False
            If dbTwoPriceCampaign.IsUpdatePrice = True Then
                hdnAllowPriceUpdate.Value = 1
            End If
            If dbTwoPriceCampaign.Status = "Awarded" And dbTwoPriceCampaign.IsUpdatePrice = False Then

                btnImportPrice.Visible = False
                btnUpdateAll.Visible = False
                btnUpdateAllBottom.Visible = False
                btnSameAll.Visible = False
                btnSameAllBottom.Visible = False
                btnPrice.Visible = False
                btnSubmit.Visible = False
                btnSubmitBidTop.Visible = False
                btnDecline.Visible = False
                If dbTwoPriceCampaign.AwardedVendorId = Session("VendorId") Then
                    btnViewPriceList.Visible = True
                End If
                If dbTwoPriceCampaign.Status = "Awarded" And dbTwoPriceCampaign.IsUpdatePrice = False Then
                    Dim status As String = IIf(dbTwoPriceCampaign.AwardedVendorId = VendorId, "Your bid", "Another vendors bid")
                    ltlSubmittedName.Text = status & " has been awarded."
                    btnExport.Visible = True
                    btnExport2.Visible = True
                    'btnViewPriceList.Visible = True
                ElseIf dbTwoPriceCampaign.IsUpdatePrice = True And dbTwoPriceCampaign.AwardedVendorId <> Session("VendorId") Then
                    Dim status As String = IIf(dbTwoPriceCampaign.AwardedVendorId = VendorId, "Your bid", "Another vendors bid")
                    ltlSubmittedName.Text = status & " has been awarded."
                    btnExport.Visible = True
                    btnExport2.Visible = True
                    'btnViewPriceList.Visible = True
                End If
            ElseIf dbTwoPriceCampaign.IsUpdatePrice = True And dbTwoPriceCampaign.AwardedVendorId <> Session("VendorId") Then
                btnImportPrice.Visible = False
                btnUpdateAll.Visible = False
                btnUpdateAllBottom.Visible = False
                btnSameAll.Visible = False
                btnSameAllBottom.Visible = False
                btnPrice.Visible = False
                btnSubmit.Visible = False
                btnSubmitBidTop.Visible = False
                btnDecline.Visible = False

                If dbTwoPriceCampaign.Status = "Awarded" And dbTwoPriceCampaign.IsUpdatePrice = False Then
                    Dim status As String = IIf(dbTwoPriceCampaign.AwardedVendorId = VendorId, "Your bid", "Another vendors bid")
                    ltlSubmittedName.Text = status & " has been awarded."
                    btnExport.Visible = True
                    btnExport2.Visible = True
                    'btnViewPriceList.Visible = True
                ElseIf dbTwoPriceCampaign.IsUpdatePrice = True And dbTwoPriceCampaign.AwardedVendorId <> Session("VendorId") Then
                    Dim status As String = IIf(dbTwoPriceCampaign.AwardedVendorId = VendorId, "Your bid", "Another vendors bid")
                    ltlSubmittedName.Text = status & " has been awarded."
                    btnExport.Visible = True
                    btnExport2.Visible = True
                    'btnViewPriceList.Visible = True
                End If
            End If
        ElseIf dbTwoPriceCampaign.Status = "Awarded" Then

            btnImportPrice.Visible = False
            btnUpdateAll.Visible = False
            btnUpdateAllBottom.Visible = False
            btnSameAll.Visible = False
            btnSameAllBottom.Visible = False
            btnPrice.Visible = False
            btnSubmit.Visible = False
            btnSubmitBidTop.Visible = False
            btnDecline.Visible = False
            btnExport.Visible = True
            btnExport2.Visible = True

            If dbTwoPriceCampaign.AwardedVendorId = Session("VendorId") Then
                btnViewPriceList.Visible = True
            End If
        End If

        dbVendor = VendorRow.GetRow(DB, VendorId)

        ltlTwoPriceInfo.Text = "<h1>" & dbTwoPriceCampaign.Name & "</h1>" & dbTwoPriceCampaign.StartDate & " - " & dbTwoPriceCampaign.EndDate
        If dbTwoPriceCampaign.GetSelectedCampaignBuilders() IsNot String.Empty Then
            rptParticipatingBuilders.DataSource = DB.GetDataTable("SELECT * FROM Builder WHERE BuilderId IN (" & dbTwoPriceCampaign.GetSelectedCampaignBuilders() & ")")
            rptParticipatingBuilders.DataBind()
        End If
        BindDocuments()

        If Not IsPostBack Then
            BindData()
        End If

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorId")
        UserName = Session("Username")
        TwoPriceCampaignId = dbTwoPriceCampaign.TwoPriceCampaignId
    End Sub

    Private Sub BindPriceHistoryData()
        Dim dbPriceHistory As DataTable = DB.GetDataTable("select distinct p.ProductID,p.Product,CAST(TPVPP.Price as DECIMAL(16,2)) as NewPrice, TPTP.SortOrder from TwoPriceVendorProductPrice TPVPP join Product p" &
                                         " On TPVPP.ProductID = p.ProductId inner join TwoPriceTakeOffProduct TPTP on TPTP.ProductID=p.ProductId" &
                                         " where TPTP.TwoPriceTakeOffID =" & DB.NullNumber(TwoPriceTakeOffId) & " and TPVPP.VendorID=" & Session("VendorId") & " and TPVPP.TwoPriceCampaignID=" & dbTwoPriceCampaign.TwoPriceCampaignId & " order by TPTP.SortOrder asc")
        grdPriceHistory.DataSource = dbPriceHistory
        grdPriceHistory.DataBind()
    End Sub

    Protected Sub OnRowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim ProductID As String = grdPriceHistory.DataKeys(e.Row.RowIndex).Value.ToString()

            Dim gvOldPrice As GridView = TryCast(e.Row.FindControl("gvOldPrice"), GridView)
            Dim sql As String = "Select p.Product, CAST(va.OldPrice as DECIMAL(16,2)) as OldPrice,cast(va.NewPrice as DECIMAL(16,2)) NewPrice,UpdatedOn from VendorReBidAudit va join Product p " &
                                          "On va.ItemId = p.ProductId where VendorId = " & Session("VendorId") & " And p.ProductId=" & ProductID & " and TwoPriceCampaignID=" & dbTwoPriceCampaign.TwoPriceCampaignId & " order by UpdatedOn"


            Dim dtOldProducts As DataTable = DB.GetDataTable(sql)

            Dim imgPlus As System.Web.UI.WebControls.Image = TryCast(e.Row.FindControl("imgPlus"), System.Web.UI.WebControls.Image)
            If dtOldProducts.Rows.Count = 0 Then
                imgPlus.ImageUrl = ""
            Else
                imgPlus.ImageUrl = "../images/plus.png"
            End If

            gvOldPrice.DataSource = DB.GetDataTable(sql)
            gvOldPrice.DataBind()
        End If
    End Sub

    <WebMethod>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetProductName(ByVal pre As String) As List(Of String)
        Dim allProductName As List(Of String) = New List(Of String)()

        Dim _Conn As SqlConnection = New SqlConnection(ConfigurationManager.AppSettings("ConnectionString"))
        Dim sql As String = "Select distinct p.ProductID,p.Product from VendorReBidAudit va join Product p " &
                                          "On va.ItemId = p.ProductId where VendorId = " & CInt(HttpContext.Current.Session("VendorId")) & " And p.Product Like '%" & pre.Replace("'", "") & "%'"
        Dim _Comm As SqlCommand = New SqlCommand(sql, _Conn)

        _Conn.Open()
        Dim _Reader As IDataReader = _Comm.ExecuteReader()

        While _Reader.Read()
            allProductName.Add(_Reader("Product"))
        End While
        _Conn.Close()
        Return allProductName
    End Function

    Private Sub BindData()
        ' Dim dbProducts As DataTable = DB.GetDataTable("SELECT * FROM Product WHERE ProductId IN (SELECT ProductID FROM TwoPriceTakeOffProduct WHERE TwoPriceTakeOffId =  " & DB.NullNumber(TwoPriceTakeOffId) & ") ")
        Dim dbproducts As DataTable = DB.GetDataTable(" SELECT *, " _
                                                      & "COALESCE((SELECT Price FROM TwoPriceVendorProductPrice tpvpp WHERE tpvpp.productid = tpto.ProductID AND tpvpp.VendorID = " & DB.NullNumber(Session("VendorId")) & " AND tpvpp.TwoPriceCampaignID = (SELECT TwoPriceCampaignID FROM TwoPriceTakeOff WHERE TwoPriceTakeOffID = " & DB.NullNumber(TwoPriceTakeOffId) & ")), 0.0) AS LastPrice " _
                                                      & "FROM Product po inner join TwoPriceTakeOffProduct tpto on po.ProductID = tpto.ProductID " _
                                                      & "WHERE tpto.TwoPriceTakeOffID = " & DB.NullNumber(TwoPriceTakeOffId) & " " _
                                                      & "Order by tpto.SortOrder ")

        rptProducts.DataSource = From sr As DataRow In dbproducts.AsEnumerable
                                 Group Join price As DataRowView In dvVendorPrices
                                 On Core.GetString(sr("ProductID")) Equals Core.GetString(price("ProductID"))
                                 Into grp = Group
                                 From price In grp.DefaultIfEmpty
                                 Select New With {
                                    .ProductID = sr("ProductID"),
                                    .Product = sr("Product"),
                                    .SKU = sr("Sku"),
                                    .Quantity = GetQuantity(VendorId, sr("ProductID")),
                                    .Comments = GetComments(VendorId, sr("ProductId")),
                                    .VendorSKU = GetValue(price, "VendorSku"),
                                    .VendorPrice = FormatCurrency(CDec(GetValue(price, "VendorPrice"))),
                                    .NextPrice = GetPrice(VendorId, sr("ProductID")),
                                    .LastPrice = sr("LastPrice"),
                                    .NextPriceApplies = GetValue(price, "NextPriceApplies"),
                                    .Updated = GetLastUpdated(VendorId, sr("ProductID")),
                                    .IsDiscontinued = GetValue(price, "IsDiscontinued"),
                                    .IsSubstitution = GetValue(price, "IsSubstitution"),
                                    .SubstituteProduct = GetValue(price, "SubstituteProduct"),
                                    .SubstituteSKU = GetValue(price, "SubstituteSku"),
                                    .SubstitutePrice = GetValue(price, "SubstitutePrice")
                                 }
        rptProducts.DataBind()

        ltlTotalPrice.Text = FormatCurrency(TotalPrice)
        ltlTotalQuantity.Text = TotalQuantity

        upProducts.Update()

    End Sub

#Region "Product Export - Medullus"
    Public Function EQToDataTable(ByVal parIList As System.Collections.IEnumerable) As System.Data.DataTable
        Dim ret As New System.Data.DataTable()
        Try
            Dim ppi As System.Reflection.PropertyInfo() = Nothing
            If parIList Is Nothing Then Return ret
            For Each itm In parIList
                If ppi Is Nothing Then
                    ppi = DirectCast(itm.[GetType](), System.Type).GetProperties()
                    For Each pi As System.Reflection.PropertyInfo In ppi
                        Dim colType As System.Type = pi.PropertyType
                        If (colType.IsGenericType) AndAlso
                       (colType.GetGenericTypeDefinition() Is GetType(System.Nullable(Of ))) Then colType = colType.GetGenericArguments()(0)
                        ret.Columns.Add(New System.Data.DataColumn(pi.Name, colType))
                    Next
                End If
                Dim dr As System.Data.DataRow = ret.NewRow
                For Each pi As System.Reflection.PropertyInfo In ppi
                    dr(pi.Name) = If(pi.GetValue(itm, Nothing) Is Nothing, DBNull.Value, pi.GetValue(itm, Nothing))
                Next
                ret.Rows.Add(dr)
            Next
            For Each c As System.Data.DataColumn In ret.Columns
                c.ColumnName = c.ColumnName.Replace("_", " ")
            Next
        Catch ex As Exception
            ret = New System.Data.DataTable()
        End Try
        Return ret
    End Function

    Public Sub ExportPriceCSVNew() Handles btnExport.Click, btnExport2.Click
        If rptProducts.DataSource = Nothing Then BindData()
        Dim dt As DataTable = CType(EQToDataTable(rptProducts.DataSource), DataTable)
        SaveExport(dt)
    End Sub

    Private Sub SaveExport(ByVal q As DataTable)
        'MultiAssetLoadingBid.Attributes.Add("style", "display:block;")
        'MultiAssetLoadingBid.Visible = True

        Dim CbusaSKU As String = String.Empty
        Dim ProductName As String = String.Empty
        Dim VendorSKU As String = String.Empty
        Dim CurrPriceinSoftWare As String = String.Empty
        Dim Qnty As String = String.Empty
        Dim BidPrice As String = String.Empty
        Dim Comments As String = String.Empty

        Dim fname As String = "/assets/vendor/product/" & Core.GenerateFileID & ".csv"
        Dim sw As IO.StreamWriter = IO.File.CreateText(Server.MapPath(fname))
        sw.WriteLine("CBUSA SKU, ProductName,Vendor SKU, Current Price in Software,Quantity,Bid Price,Comments")
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
            If Not IsDBNull(row.Item("VendorSku")) Then
                VendorSKU = row.Item("VendorSku")
            Else
                VendorSKU = ""
            End If

            If Not IsDBNull(row.Item("VendorPrice")) Then
                CurrPriceinSoftWare = row.Item("VendorPrice")
            Else
                CurrPriceinSoftWare = ""
            End If
            If Not IsDBNull(row.Item("Quantity")) Then
                Qnty = row.Item("Quantity")
            Else
                Qnty = ""
            End If
            If Not IsDBNull(row.Item("NextPrice")) Then
                BidPrice = row.Item("NextPrice")
            Else
                BidPrice = ""
            End If
            If Not IsDBNull(row.Item("Comments")) Then
                Comments = row.Item("Comments")
            Else
                Comments = ""
            End If

            sw.WriteLine(Core.QuoteCSV(CbusaSKU) & "," & Core.QuoteCSV(ProductName) & "," & Core.QuoteCSV(VendorSKU) & "," & Core.QuoteCSV(CurrPriceinSoftWare) & "," & Core.QuoteCSV(Qnty) & "," & Core.QuoteCSV(BidPrice) & "," & Core.QuoteCSV(Comments))

        Next
        sw.Close()
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "windowscript", "HideLoader();", True)
        ' MultiAssetLoadingBid.Style.Add("display", "none")
        Response.Redirect(fname)

        'MultiAssetLoadingBid.Style.Add("display", "none")  'Attributes.Add("style", "display:none;")
        'MultiAssetLoadingBid.Visible = False




    End Sub
#End Region
    Private Function GetValue(ByVal row As DataRowView, ByVal field As String) As Object
        If row Is Nothing Then
            Return Nothing
        ElseIf IsDBNull(row(field)) Then
            Return Nothing
        Else
            Return If(String.IsNullOrEmpty(row(field)), "", row(field))
        End If
    End Function

    Private Function GetPrice(ByVal VendorId As Integer, ByVal ProductId As Integer) As Decimal
        Dim returnval As Decimal = Nothing
        If dbTwoPriceCampaign IsNot Nothing Then
            'Look for any unsaved prices
            Dim TwoPrice As TwoPriceVendorProductPriceRow = TwoPriceVendorProductPriceRow.GetRow(DB, VendorId, ProductId, dbTwoPriceCampaign.TwoPriceCampaignId, False)
            If TwoPrice.TwoPriceProductPriceID <> 0 Then
                returnval = TwoPrice.Price
            Else
                'Else, get submitted pricing.
                TwoPrice = TwoPriceVendorProductPriceRow.GetRow(DB, VendorId, ProductId, dbTwoPriceCampaign.TwoPriceCampaignId, True)
                returnval = TwoPrice.Price
            End If
        End If
        Return returnval
    End Function

    Private Function GetQuantity(ByVal VendorId As Integer, ByVal ProductId As Integer) As Integer
        Dim returnval As Integer = Nothing
        returnval = (From row In dbTwoPriceTakeOff.GetProductsList().Rows Where row.Item("ProductId") = ProductId Select CInt(row.Item("Quantity"))).FirstOrDefault
        Return returnval
    End Function

    Private Function GetLastUpdated(ByVal VendorId As Integer, ByVal ProductId As Integer) As DateTime
        Dim returnval As DateTime = Nothing
        If dbTwoPriceCampaign IsNot Nothing Then
            'Look for any unsaved prices
            Dim TwoPrice As TwoPriceVendorProductPriceRow = TwoPriceVendorProductPriceRow.GetRow(DB, VendorId, ProductId, dbTwoPriceCampaign.TwoPriceCampaignId, False)
            If TwoPrice.TwoPriceProductPriceID <> 0 Then
                returnval = TwoPrice.LastUpdated
            Else
                'Else, get submitted pricing.
                TwoPrice = TwoPriceVendorProductPriceRow.GetRow(DB, VendorId, ProductId, dbTwoPriceCampaign.TwoPriceCampaignId, True)
                returnval = TwoPrice.LastUpdated
            End If
        End If
        Return returnval
    End Function

    Private Function GetComments(ByVal VendorId As Integer, ByVal ProductId As Integer) As String
        Dim returnval As String = Nothing
        If dbTwoPriceCampaign IsNot Nothing Then
            'Look for any unsaved comments
            Dim TwoPrice As TwoPriceVendorProductPriceRow = TwoPriceVendorProductPriceRow.GetRow(DB, VendorId, ProductId, dbTwoPriceCampaign.TwoPriceCampaignId, False)
            If TwoPrice.TwoPriceProductPriceID <> 0 Then
                returnval = TwoPrice.Comments
            Else
                'Else, get submitted comments.
                TwoPrice = TwoPriceVendorProductPriceRow.GetRow(DB, VendorId, ProductId, dbTwoPriceCampaign.TwoPriceCampaignId, True)
                returnval = TwoPrice.Comments
            End If
        End If
        Return returnval
    End Function

    Protected Sub rptProducts_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptProducts.ItemCommand
        Dim txtSku As TextBox = CType(e.Item.FindControl("txtSku"), TextBox)
        Dim txtNextPrice As TextBox = CType(e.Item.FindControl("txtNextPrice"), TextBox)
        Dim ErrorMsg As String = String.Empty
        Dim ProductId As Integer = e.CommandArgument
        Dim Command As String = CType(e.CommandSource, Button).Text
        Dim txtPrice As TextBox = e.Item.FindControl("txtNextPrice")
        Dim hdnPreviousPrice As HiddenField = e.Item.FindControl("hdnPreviousPrice")
        Dim txtComments As TextBox = CType(e.Item.FindControl("txtComments"), TextBox)


        Select Case Command
            Case "Update"
                Dim Price As Decimal

                Dim VendorProductPrice As TwoPriceVendorProductPriceRow = TwoPriceVendorProductPriceRow.GetRow(DB, VendorId, ProductId, dbTwoPriceCampaign.TwoPriceCampaignId, False)

                If Decimal.TryParse(txtNextPrice.Text, Price) Then
                    If Price <> Nothing And (Price <> VendorProductPrice.Price Or txtComments.Text <> VendorProductPrice.Comments) Then
                        If VendorProductPrice.TwoPriceProductPriceID <> 0 Then
                            If dbTwoPriceCampaign.Status = "Awarded" Then
                                If (Price <> hdnPreviousPrice.Value) Then
                                    StrQryLog = String.Format("insert into VendorReBidAudit(ItemId,OldPrice,NewPrice,UpdatedOn,vendorId, TwoPriceCampaignID) values('{0}','{1}','{2}','{3}','{4}','{5}')",
                                                                    ProductId, VendorProductPrice.Price, txtNextPrice.Text, DateTime.Now(), Session("VendorId"), dbTwoPriceCampaign.TwoPriceCampaignId)
                                    DB.ExecuteSQL(StrQryLog)
                                End If
                            End If

                            VendorProductPrice.ProductID = ProductId
                            VendorProductPrice.LastUpdated = DateTime.Now
                            VendorProductPrice.VendorID = VendorId
                            VendorProductPrice.TwoPriceCampaignID = dbTwoPriceCampaign.TwoPriceCampaignId
                            VendorProductPrice.Comments = txtComments.Text
                            VendorProductPrice.Submitted = False
                            VendorProductPrice.Price = Price

                            VendorProductPrice.Update()
                        Else
                            VendorProductPrice.Insert()
                        End If
                        'log Update Product Price  
                        Core.DataLog("Commited Purchase Vendor", PageURL, CurrentUserId, "Update Price", ProductId, "", "", "", UserName)
                        'end log
                    End If
                End If
            Case "Same Price"
                Dim ProductRow As VendorProductPriceRow = VendorProductPriceRow.GetRow(DB, VendorId, ProductId)
                Dim Price As Decimal
                If ProductRow.ProductID <> Nothing Then
                    If Decimal.TryParse(ProductRow.VendorPrice, Price) Then
                        txtNextPrice.Text = ProductRow.VendorPrice
                    End If
                    'log  same price for all 
                    Core.DataLog("Commited Purchase Vendor", PageURL, CurrentUserId, "Same Price", ProductId, "", "", "", UserName)
                    'end log
                End If

                SaveAll(False)
        End Select

        BindData()
    End Sub

    Protected Sub rptProducts_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProducts.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        Dim imgReq As WebControls.Image = CType(e.Item.FindControl("imgReq"), WebControls.Image)
        Dim txtPrice As TextBox = e.Item.FindControl("txtNextPrice")
        Dim txtComments As TextBox = e.Item.FindControl("txtComments")
        Dim btn As Button = e.Item.FindControl("btnSyncPrice")
        Dim btn2 As Button = e.Item.FindControl("btnUpdate")

        txtPrice.Text = txtPrice.Text

        TotalPrice += CDec(txtPrice.Text) * e.Item.DataItem.Quantity
        TotalQuantity += e.Item.DataItem.Quantity

        If dbTwoPriceCampaign.Status = "Awarded" And dbTwoPriceCampaign.IsUpdatePrice = False Then
            txtPrice.Enabled = False
            txtComments.Enabled = False
            Dim c As Color = ColorTranslator.FromHtml("#E8E8E8")
            txtPrice.BackColor = c

            btn.Visible = False
            btn2.Visible = False
        ElseIf dbTwoPriceCampaign.IsUpdatePrice = True And dbTwoPriceCampaign.AwardedVendorId <> Session("VendorId") Then
            txtPrice.Enabled = False
            txtComments.Enabled = False
            Dim c As Color = ColorTranslator.FromHtml("#E8E8E8")
            txtPrice.BackColor = c

            btn.Visible = False
            btn2.Visible = False
        End If

        If Not IsDBNull(e.Item.DataItem.Updated) Then
            'If DateDiff("h", e.Item.DataItem.Updated, Now()) < 2 Then
            If e.Item.DataItem.Updated <> "12:00:00 AM" Then
                imgReq.ImageUrl = "/images/admin/True.gif"
                imgReq.Visible = True
            End If
            'End If
        End If
    End Sub

    Protected Sub btnUpdateAll_Click(sender As Object, e As EventArgs) Handles btnUpdateAll.Click, btnUpdateAllBottom.Click

        Session("UpdateAll") = 1
        SaveAll(False)

        BindData()

        'log Update Price For All
        Core.DataLog("Commited Purchase Vendor", PageURL, CurrentUserId, "Update Price For All", "", "", "", "", UserName)
        'end log
    End Sub

    Protected Sub btnSameAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSameAll.Click, btnSameAllBottom.Click
        For Each item As RepeaterItem In rptProducts.Items
            Dim ProductId As Integer = CType(item.FindControl("btnSyncPrice"), Button).CommandArgument
            Dim VendorProductPrice As TwoPriceVendorProductPriceRow = TwoPriceVendorProductPriceRow.GetRow(DB, VendorId, ProductId, dbTwoPriceCampaign.TwoPriceCampaignId)
            Dim ProductRow As VendorProductPriceRow = VendorProductPriceRow.GetRow(DB, VendorId, ProductId)
            Dim Price As Decimal
            Dim txtPrice As TextBox = item.FindControl("txtNextPrice")

            If ProductRow.ProductID <> Nothing Then
                If Decimal.TryParse(ProductRow.VendorPrice, Price) Then
                    txtPrice.Text = ProductRow.VendorPrice
                End If
            End If
        Next

        'log Same Price For All
        Core.DataLog("Commited Purchase Vendor", PageURL, CurrentUserId, "Same Price For All", "", "", "", "", UserName)
        'end log

        SaveAll(False)
        'BindData()
    End Sub

    Protected Sub btnSubmit_Click(sender As Button, e As EventArgs) Handles btnSubmit.Click, btnDecline.Click, btnSubmitBidTop.Click
        Dim bValid As Boolean = True
        hdnAllowPriceUpdate.Value = 0
        Session("BidDeclined") = False
        Session("UpdateAll") = Nothing
        If sender.ID = "btnDecline" Then
            Session("BidDeclined") = True
            btnSendMessage.Message = "Are you sure you want to decline ?"
        End If

        If Not Session("BidDeclined") Then
            For Each item As RepeaterItem In rptProducts.Items
                Dim Price As Double = Nothing
                Dim txtPrice As TextBox = item.FindControl("txtNextPrice")
                Dim Parsed As Boolean = Double.TryParse(txtPrice.Text, Price)
                Dim tr As HtmlTableRow = item.FindControl("trVendor")
                If Not Parsed Or Price = Nothing Then
                    bValid = False
                    tr.Attributes.Add("style", "background-color:Yellow")
                Else
                    tr.Attributes("style") = ""
                End If
            Next

            If dbTwoPriceCampaign.Status = "Awarded" Then
                SaveAll(True, True)
            Else
                If bValid Then
                    SaveAll(True, True)
                Else
                    SaveAll(False, True)
                End If
            End If
        End If

        If dbTwoPriceCampaign.IsUpdatePrice = True Then
            'lblSendMessageHeader.Text = ""
            'lblSendMessageHeader.Text = "Send Message to Builder For Mid Event Price Update"
            'Else
            '   lblSendMessageHeader.Text = "Send Message to CBUSA and Submit"
            hdnAllowPriceUpdate.Value = 1
        End If
        If Not bValid Then
            ltrErrorMsg.Text = "<h2><span style=""color:orangered"">YOU MUST PRICE ALL PRODUCTS.</span></h2>"
        Else
            ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenSubmitForm", "Sys.Application.add_load(OpenSubmitForm);", True)
        End If
    End Sub

    Protected Sub btnSendMessage_Click(sender As Object, e As System.EventArgs) Handles btnSendMessage.Click
        Try

            If Not Session("BidDeclined") Then
                'SaveAll(True)
            End If

            Dim dbAutoMessage As AutomaticMessagesRow

            If Session("BidDeclined") Then
                dbAutoMessage = AutomaticMessagesRow.GetRowByTitle(DB, "CampaignVendorDeclined")
                'Vendor Declined to bid
                'Clear unsubmitted bids - redundant at this point since they're all being declined
                DB.ExecuteSQL("UPDATE TwoPriceCampaignVendor_Rel SET HasDeclinedToBid = 1  WHERE VendorID = " & VendorId & " AND TwoPriceCampaignId=" & dbTwoPriceCampaign.TwoPriceCampaignId)
                DB.ExecuteSQL("DELETE FROM TwoPriceVendorProductPrice WHERE VendorID = " & VendorId & " AND TwoPriceCampaignID=" & dbTwoPriceCampaign.TwoPriceCampaignId & " AND Submitted = 0")
                ' Session("BidDeclined") = Nothing

                'log Decline Bid
                Core.DataLog("Commited Purchase Vendor", PageURL, CurrentUserId, "Decline Bid", TwoPriceCampaignId, "", "", "", UserName)
                'end log
            Else
                dbAutoMessage = AutomaticMessagesRow.GetRowByTitle(DB, "CampaignBidComplete")
                'log Submit Bid
                Core.DataLog("Commited Purchase Vendor", PageURL, CurrentUserId, "Submit Bid", TwoPriceCampaignId, "", "", "", UserName)
                'end log
            End If
            If dbTwoPriceCampaign.IsUpdatePrice = True Then
                'If Not String.IsNullOrEmpty(StrQryLog) Then
                '    DB.ExecuteSQL(StrQryLog)
                'End If
                SendMailForPriceUpdate()
                dbTwoPriceCampaign.IsUpdatePrice = False
                dbTwoPriceCampaign.Update()
            Else
                Dim sBody As String = FormatMessage(DB, dbAutoMessage, dbVendor, txtMessage.Text, dbTwoPriceCampaign.Name)
                dbAutoMessage.SendBidSubmitMail(txtDescription.Text, sBody, 552)

                'Log mail sent for accept/decline bid
                Core.DataLog("Commited Purchase Vendor", PageURL, CurrentUserId, "Mail Sent for Accept/Decline Bid", TwoPriceCampaignId, "", "", "", UserName)
                'end log
            End If
            'Dim dbTwoPriceCampaign As TwoPriceCampaignRow = TwoPriceCampaignRow.GetRow(DB, TwoPriceCampaignId)

            If Session("BidDeclined") Then
                Session("BidDeclined") = Nothing
                Response.Redirect("/vendor/twoprice/", True)
            End If
            Response.Redirect(Request.RawUrl)
        Catch ex As Exception
            pnMain.Visible = False
            pnlSendMessage.Visible = True
            pnComplete.Visible = False
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Private Function FormatMessage(ByVal DB As Database, ByVal automessage As AutomaticMessagesRow, ByVal drVendor As VendorRow, ByVal Msg As String, ByVal CampaignName As String) As String
        Dim tempMsg As String
        tempMsg = automessage.Message
        tempMsg = tempMsg.Replace("%%Vendor%%", drVendor.CompanyName)
        tempMsg = tempMsg.Replace("%%Campaign%%", CampaignName)
        tempMsg = tempMsg.Replace("%%AdditionalMessage%%", Msg)

        Return tempMsg
    End Function

    Protected Sub SaveAll(ByVal Submit As Boolean, Optional ByVal IsFinal As Boolean = False)

        Try
            For Each item As RepeaterItem In rptProducts.Items
                Dim txtSku As TextBox = CType(item.FindControl("txtSku"), TextBox)
                Dim lblCurrentPrice As Label = CType(item.FindControl("lblCurrentPrice"), Label)
                Dim txtNextPrice As TextBox = CType(item.FindControl("txtNextPrice"), TextBox)
                Dim hdnPreviousPrice As HiddenField = CType(item.FindControl("hdnPreviousPrice"), HiddenField)
                Dim txtComments As TextBox = CType(item.FindControl("txtComments"), TextBox)
                Dim ProductId As Integer = CType(item.FindControl("btnSyncPrice"), Button).CommandArgument
                Dim ErrorMsg As String = String.Empty
                Dim Price As Decimal

                Dim VendorProductPrice As TwoPriceVendorProductPriceRow = TwoPriceVendorProductPriceRow.GetRow(DB, VendorId, ProductId, dbTwoPriceCampaign.TwoPriceCampaignId, False)

                Dim OldPrice As Double = 0.00
                If CInt(hdnPreviousPrice.Value) = 0 Then        'Price is imported
                    OldPrice = lblCurrentPrice.Text         'Put CurrentPrice as OldPrice if not already priced in this bid 
                Else
                    OldPrice = hdnPreviousPrice.Value
                End If

                DB.BeginTransaction()

                If Decimal.TryParse(txtNextPrice.Text, Price) Then
                    If Price <> Nothing Then                        'And (Price <> VendorProductPrice.Price Or txtComments.Text <> VendorProductPrice.Comments Or Submit <> VendorProductPrice.Submitted) 

                        If dbTwoPriceCampaign.Status = "Awarded" Then
                            If (Price <> hdnPreviousPrice.Value) Then
                                If (IsFinal = True) Or (Session("UpdateAll") = 1) Then
                                    StrQryLog = String.Format("insert into VendorReBidAudit(ItemId,OldPrice,NewPrice,UpdatedOn,vendorId, TwoPriceCampaignID) values('{0}','{1}','{2}','{3}','{4}','{5}')",
                                                                ProductId, OldPrice, txtNextPrice.Text, DateTime.Now(), Session("VendorId"), dbTwoPriceCampaign.TwoPriceCampaignId)
                                    DB.ExecuteSQL(StrQryLog)
                                End If
                            End If
                        End If

                        VendorProductPrice.ProductID = ProductId
                        VendorProductPrice.LastUpdated = DateTime.Now
                        VendorProductPrice.VendorID = VendorId
                        VendorProductPrice.TwoPriceCampaignID = dbTwoPriceCampaign.TwoPriceCampaignId
                        VendorProductPrice.Comments = txtComments.Text
                        If (Session("UpdateAll") = 1) Then
                            VendorProductPrice.Submitted = False
                        Else
                            VendorProductPrice.Submitted = Submit
                        End If

                        VendorProductPrice.Price = Price

                        If VendorProductPrice.TwoPriceProductPriceID <> 0 Then
                            If (IsFinal = True) Or (Session("UpdateAll") = 1) Then
                                VendorProductPrice.Update()
                            End If
                        Else
                            VendorProductPrice.Insert()
                        End If
                    End If
                End If

                DB.CommitTransaction()
            Next

            'Clear unsubmitted bids - redundant at this point since they're all being submitted
            If dbTwoPriceCampaign.Status <> "Awarded" And Submit Then
                DB.ExecuteSQL("UPDATE TwoPriceCampaignVendor_Rel SET HasDeclinedToBid = 0  WHERE VendorID = " & VendorId & " AND TwoPriceCampaignId=" & dbTwoPriceCampaign.TwoPriceCampaignId)
                DB.ExecuteSQL("DELETE FROM TwoPriceVendorProductPrice WHERE VendorID = " & VendorId & " AND TwoPriceCampaignID=" & dbTwoPriceCampaign.TwoPriceCampaignId & " AND Submitted = 0")
            End If
        Catch ex As SqlClient.SqlException
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try

        m_VendorPrices = Nothing
        upProducts.Update()
    End Sub

    Protected Sub rptDocuments_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptDocuments.ItemDataBound

        If Not e.Item.ItemType = ListItemType.Item And Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim lnkMessageTitle As HtmlAnchor = e.Item.FindControl("lnkMessageTitle")
        lnkMessageTitle.HRef = "/assets/Twoprice/" & e.Item.DataItem("FileName").ToString

    End Sub

    Protected Sub BindDocuments()

        Dim SQL As String = String.Empty
        Dim dt As DataTable

        SQL = "Select * From  TwoPriceDocument  where TwoPriceCampaignId = " & DB.Number(dbTwoPriceCampaign.TwoPriceCampaignId)

        If SQL <> String.Empty Then
            dt = DB.GetDataTable(SQL)

            Me.rptDocuments.DataSource = dt
            Me.rptDocuments.DataBind()

            If dt.Rows.Count = 0 Then
                rptDocuments.Visible = False
                ltlNoDocs.Visible = True
            End If
        End If
    End Sub

    Public Sub SendMailForPriceUpdate()
        Dim SQL As String = String.Empty
        Dim SQLBuilder As String = String.Empty
        Dim mBody As String = String.Empty
        Dim FromName As String = SysParam.GetValue(GlobalDB.DB, "ContactUsName")
        Dim FromEmail As String = SysParam.GetValue(GlobalDB.DB, "ContactUsEmail")

        Dim dt As DataTable
        Dim dtBuilder As DataTable

        SQL = "select AdminId,Email,Firstname + ' ' + LastName as Name from Admin"
        'SQLBuilder = "select tcr.BuilderId,b.CompanyName,Email from TwoPriceCampaignBuilder_Rel tcr join twopricecampaign tc on tcr.TwoPriceCampaignId = tc.TwoPriceCampaignId " &
        '            "join Builder b on tcr.BuilderId = b.BuilderId " &
        '            "where tc.TwoPriceCampaignId = " & dbTwoPriceCampaign.TwoPriceCampaignId


        SQLBuilder = "select BuilderId,CompanyName,Email from (select tcr.BuilderId,b.CompanyName,Email from TwoPriceCampaignBuilder_Rel tcr join twopricecampaign tc on tcr.TwoPriceCampaignId = tc.TwoPriceCampaignId " &
                     "join Builder b on tcr.BuilderId = b.BuilderId " &
                     "where tc.TwoPriceCampaignId = " & dbTwoPriceCampaign.TwoPriceCampaignId & ") a where a.BuilderID in (select BuilderID from Builder where LLCID in (select LLCID from TwoPriceCampaignLLC_Rel where TwoPriceCampaignId=" & dbTwoPriceCampaign.TwoPriceCampaignId & "))"






        If SQL <> String.Empty Then
            dt = DB.GetDataTable(SQL)
            dtBuilder = DB.GetDataTable(SQLBuilder)
            For Each drBuilder As DataRow In dtBuilder.Rows
                mBody = "Dear " & drBuilder("CompanyName") & ", " &
                   "The pricing for the " & dbTwoPriceCampaign.Name & " originally awarded to " & dbVendor.CompanyName & " has been updated in the CBUSA Software and is now available for your access. " &
                    txtDescription.Text &
                   " You can view the prices for this campaign on our site: " & Request.Url.AbsoluteUri


                'For Each Drow As DataRow In dt.Rows
                Core.SendSimpleMail(FromEmail, FromName, drBuilder("Email"), drBuilder("CompanyName"), "Mid-Event Pricing Update for " & dbTwoPriceCampaign.Name, mBody)
                'Next
            Next
        End If
    End Sub


#Region "Export"
    Public Sub ExportCSV()
        bExport = True
    End Sub

    Public Sub ExportPriceCSV()
        bPriceExport = True
        Dim SQL As String = String.Empty

        'SQL &= "SELECT p.SKU, p.Product, twvpp.Price AS NewPrice, twvpp.LastUpdated AS Updated, twvpp.Comments"
        'SQL &= ", (Select VendorPrice From VendorProductPrice Where ProductID = p.ProductID And VendorId = " & DB.Number(VendorId) & ") AS CurrentPrice "
        'SQL &= ", (Select VendorSKU From VendorProductPrice Where ProductID = p.ProductID And VendorId = " & DB.Number(VendorId) & ") AS VendorSKU "
        'SQL &= " FROM Product p"
        'SQL &= " LEFT JOIN TwoPriceVendorProductPrice twvpp ON p.ProductId = twvpp.ProductId AND twvpp.TwoPriceCampaignID = " & dbTwoPriceCampaign.TwoPriceCampaignId & " AND twvpp.VendorId = " & DB.Number(VendorId)
        'SQL &= " WHERE p.ProductID IN (SELECT ProductID FROM TwoPriceTakeOffProduct WHERE TwoPriceTakeOffId =  " & DB.Number(TwoPriceTakeOffId) & ")"

        SQL &= "SELECT p.SKU, p.Product, twvpp.Price AS NewPrice, twvpp.LastUpdated AS Updated, twvpp.Comments, tptop.Quantity "
        SQL &= ", (Select VendorPrice From VendorProductPrice Where ProductID = p.ProductID And VendorId = " & DB.Number(VendorId) & ") AS CurrentPrice "
        SQL &= ", (Select VendorSKU From VendorProductPrice Where ProductID = p.ProductID And VendorId = " & DB.Number(VendorId) & ") AS VendorSKU "
        SQL &= " FROM Product p inner join twopricetakeoffproduct tptop on  p.ProductID = tptop.ProductID  and tptop .TwoPriceTakeOffID = " & DB.Number(TwoPriceTakeOffId)
        SQL &= " LEFT JOIN TwoPriceVendorProductPrice twvpp ON p.ProductId = twvpp.ProductId AND twvpp.TwoPriceCampaignID = " & dbTwoPriceCampaign.TwoPriceCampaignId & " AND twvpp.VendorId = " & DB.Number(VendorId)
        SQL &= " ORDER BY tptop .SortOrder "

        Dim dt As DataTable = DB.GetDataTable(SQL)

        SavePriceExport(dt)
        'log submit order
        Core.DataLog("Commited Purchase Vendor", PageURL, CurrentUserId, "Export Price CSV", "", "", "", "", UserName)
        'end log
    End Sub

    Private Sub SaveExport(ByVal q As IEnumerable)
        Dim SKU As String = String.Empty
        Dim Product As String = String.Empty
        Dim VendorSKU As String = String.Empty
        Dim VendorPrice As String = String.Empty
        'Dim Updated As String = String.Empty

        Dim fname As String = "/assets/vendor/product/" & Core.GenerateFileID & ".csv"
        Dim sw As IO.StreamWriter = IO.File.CreateText(Server.MapPath(fname))
        'TICKET_ID=217174
        sw.WriteLine("Vendor SKU,Vendor Price,CBUSA SKU,Product Name")
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
            'If Not IsDBNull(row.Updated) Then
            '    Updated = row.Updated
            'Else
            '    Updated = ""
            'End If
            sw.WriteLine(Core.QuoteCSV(VendorSKU) & "," & Core.QuoteCSV(VendorPrice) & "," & Core.QuoteCSV(SKU) & "," & Core.QuoteCSV(Product))
        Next
        sw.Close()
        Response.Redirect(fname)
    End Sub

    Private Sub SavePriceExport(ByVal q As DataTable)
        Dim CbusaSKU As String = String.Empty
        Dim VendorSKU As String = String.Empty
        Dim CurrentPrice As String = String.Empty
        Dim NewPrice As String = String.Empty
        Dim Updated As DateTime = Nothing
        Dim Product As String = String.Empty
        Dim Comments As String = String.Empty
        Dim Quantity As String = String.Empty

        Dim fname As String = "/assets/vendor/product/" & Core.GenerateFileID & ".csv"
        Dim sw As IO.StreamWriter = IO.File.CreateText(Server.MapPath(fname))
        sw.WriteLine("SKU, Vendor SKU, Product, Current Price, Bid Price, Quantity, Updated, Comments")
        For Each row As DataRow In q.Rows
            If Not IsDBNull(row.Item("SKU")) Then
                CbusaSKU = row.Item("SKU")
            Else
                CbusaSKU = ""
            End If
            If Not IsDBNull(row.Item("VendorSKU")) Then
                VendorSKU = row.Item("VendorSKU")
            Else
                VendorSKU = ""
            End If
            If Not IsDBNull(row.Item("Product")) Then
                Product = row.Item("Product")
            Else
                Product = ""
            End If
            If Not IsDBNull(row.Item("CurrentPrice")) Then
                CurrentPrice = row.Item("CurrentPrice")
            Else
                CurrentPrice = ""
            End If
            If Not IsDBNull(row.Item("NewPrice")) Then
                NewPrice = row.Item("NewPrice")
            Else
                NewPrice = ""
            End If
            If Not IsDBNull(row.Item("Quantity")) Then
                Quantity = row.Item("Quantity")
            Else
                Quantity = Nothing
            End If
            If Not IsDBNull(row.Item("Updated")) Then
                Updated = row.Item("Updated")
            Else
                Updated = Nothing
            End If
            If Not IsDBNull(row.Item("Comments")) Then
                Comments = row.Item("Comments")
            Else
                Comments = Nothing
            End If

            sw.WriteLine(Core.QuoteCSV(CbusaSKU) & "," & Core.QuoteCSV(VendorSKU) & "," & Core.QuoteCSV(Product) & "," & Core.QuoteCSV(CurrentPrice) & "," & Core.QuoteCSV(NewPrice) & "," & Core.QuoteCSV(Quantity) & "," & Core.QuoteCSV(IIf(Updated <> Nothing, Updated.ToString, "")) & "," & Core.QuoteCSV(Comments))
        Next
        sw.Close()
        'ltrErrorMsg.Text = "<div style=""float:right;""><a href=""" & fname & """><b>Click here to download CSV file</b></a></div>"
        'ltrErrorMsg.Visible = True
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
            'log Import File
            Core.DataLog("Commited Purchase Vendor", PageURL, CurrentUserId, "Import File", TwoPriceCampaignId, "", "", "", UserName)
            'end log
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

        'ltlBreadcrumb.Text = "<a href=""sku-price.aspx"">Clear All</a>"

    End Sub

    Protected Sub btnImportPriceUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnImportPriceUpdate.Click
        Try
            ImportPriceCSV(Session("VendorImportPriceFile"), True)
        Catch ex As Exception
            ltrErrorMsg.Text = Err.Description
            ltrErrorMsg.Visible = True
        End Try

        'BindData()
        upProducts.Update()
        'log submit order
        Core.DataLog("Commited Purchase Vendor", PageURL, CurrentUserId, "Import Price", TwoPriceCampaignId, "", "", "", UserName)
        'end log
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
        Dim VendorPrice As String = String.Empty
        Dim Comments As String = String.Empty

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
                CbusaSKU = Trim(Core.StripDblQuote(aLine(0)))
                VendorPrice = Trim(Core.StripDblQuote(aLine(4)))
                Comments = Trim(Core.StripDblQuote(aLine(7)))
                If IsNumeric(VendorPrice) Then
                    VendorPrice = Math.Round(Double.Parse(VendorPrice), 2)
                End If
                If Count = 1 And CbusaSKU.ToUpper <> "SKU" Then
                    ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
                    ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""><li>The file you uploaded does not appear to be in the correct format. The file format must be identical to the one used in the Export CSV process.</td></tr></table>"
                    Exit Sub
                End If

                If CbusaSKU.ToUpper = "SKU" Then
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

                'Price must be a numeric value and greater than 0
                If VendorPrice <> String.Empty Then
                    If Not IsNumeric(VendorPrice) Then
                        BadCount = BadCount + 1
                        txtErr &= "<li>Invalid price"
                        tblErr &= "<tr class=""red""><td>" & CbusaSKU & "</td><td>" & txtErr & "</td><tr>"
                        Continue While
                    ElseIf VendorPrice <= 0 Then
                        BadCount = BadCount + 1
                        txtErr &= "<li>Non-Positive price"
                        tblErr &= "<tr class=""red""><td>" & CbusaSKU & "</td><td>" & txtErr & "</td><tr>"
                        Continue While
                    End If
                End If

                If txtErr = String.Empty Then
                    Dim PriceRequired As Boolean = True

                    If VendorPrice = String.Empty OrElse Not IsNumeric(VendorPrice) Then
                        txtErr &= "<li>Price required"
                    End If

                    If txtErr = String.Empty And UpdateDatabase Then
                        Try
                            Dim VendorProductPrice As TwoPriceVendorProductPriceRow = TwoPriceVendorProductPriceRow.GetRow(DB, VendorId, ProductId, dbTwoPriceCampaign.TwoPriceCampaignId, False)

                            If dbTwoPriceCampaign.Status = "Awarded" Then
                                If VendorPrice <> VendorProductPrice.Price Then
                                    StrQryLog = String.Format("insert into VendorReBidAudit(ItemId,OldPrice,NewPrice,UpdatedOn,vendorId, TwoPriceCampaignID) values('{0}','{1}','{2}','{3}','{4}','{5}')",
                                                                 ProductId, VendorProductPrice.Price, VendorPrice, DateTime.Now(), Session("VendorId"), dbTwoPriceCampaign.TwoPriceCampaignId)
                                    DB.ExecuteSQL(StrQryLog)
                                End If
                            End If

                            DB.BeginTransaction()

                            Session("IsPriceImported") = 1           '======================== Import CSV invoked

                            'If VendorPrice <> VendorProductPrice.Price Then
                            VendorProductPrice.ProductID = ProductId
                            VendorProductPrice.LastUpdated = DateTime.Now
                            VendorProductPrice.VendorID = VendorId
                            VendorProductPrice.TwoPriceCampaignID = dbTwoPriceCampaign.TwoPriceCampaignId
                            VendorProductPrice.Submitted = False
                            VendorProductPrice.Price = VendorPrice
                            VendorProductPrice.Comments = Comments

                            If VendorProductPrice.TwoPriceProductPriceID <> 0 Then
                                VendorProductPrice.Update()
                                UpdateCount += 1
                                IsUpdate = True
                            Else
                                VendorProductPrice.Insert()
                                InsertCount += 1
                            End If

                            DB.CommitTransaction()
                        Catch ex As Exception
                            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                            ltrErrorMsg.Text = ErrHandler.ErrorText(ex)
                            btnImportPriceUpdate.Visible = False
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

        m_VendorPrices = Nothing

        Count = Count - 1

        tblErr = "<tr class=""red bold""><td>Invalid Entries</td><td>" & BadCount & "</td><tr>" & tblErr

        If UpdateDatabase Then
            tblErr = "<tr class=""green""><td>Products Added</td><td>" & InsertCount & "</td><tr>" & tblErr
            tblErr = "<tr class=""green""><td>Products Updated</td><td>" & UpdateCount & "</td><tr>" & tblErr
            ltrErrorMsg.Text = "<b>Data import process completed with success. Please review the report below for more details.</b>"
            btnImportPriceUpdate.Visible = False
        Else
            tblErr = "<tr class=""green""><td>Products To Add</td><td>" & InsertCount & "</td><tr>" & tblErr
            tblErr = "<tr class=""green""><td>Products To Update</td><td>" & UpdateCount & "</td><tr>" & tblErr
            btnImportPriceUpdate.Visible = True
            ltrErrorMsg.Text = "<b>File uploaded successfully. Please review the report below and click 'Continue Bid Pricing Import' to confirm the data import.</b>"
        End If

        tblErr = "<tr class=""green""><td>Valid Entries</td><td>" & Count - BadCount & "</td><tr>" & tblErr
        tblErr = "<tr class=""blue bold""><td>Total Rows Processed</td><td>" & Count & "</td><tr>" & tblErr

        ltrErrorMsg.Text &= "<hr><table align=""center"">" & tblErr & "</table><hr>"
        ltrErrorMsg.Visible = True
        BindData()

    End Sub
#End Region
    Protected Sub btnExportToExcel_Click(sender As Object, e As EventArgs) Handles btnExportToExcel.Click
        Dim dbPriceHistory As DataTable = DB.GetDataTable("select p.Product,va.OldPrice,va.NewPrice,convert(varchar(max),va.UpdatedOn,101) UpdatedOn from VendorReBidAudit va join Product p " &
                                          "on va.ItemId = p.ProductId where VendorId = " & Session("VendorId"))
        Dim dt As DataTable = dbPriceHistory
        Dim attachment As String = "attachment; filename=PriceHistory.xls"
        Response.ClearContent()
        Response.AddHeader("content-disposition", attachment)
        Response.ContentType = "application/vnd.ms-excel"
        Dim tab As String = ""

        For Each dc As DataColumn In dt.Columns
            Response.Write(tab & dc.ColumnName)
            tab = vbTab
        Next

        Response.Write(vbLf)
        Dim i As Integer

        For Each dr As DataRow In dt.Rows
            tab = ""

            For i = 0 To dt.Columns.Count - 1
                Response.Write(tab & dr(i).ToString())
                tab = vbTab
            Next

            Response.Write(vbLf)
        Next

        Response.[End]()
    End Sub
End Class
