Imports Components
Imports Controls
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer
Imports System.Collections.Generic
Imports System.Collections
Imports System.IO
Imports System.Linq


Partial Class Index
    Inherits AdminPage

    Dim supphase As DataTable
    Dim supplyPhases As String
    Dim buildCondition As String



    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("PRODUCTS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_Product.Text = Request("F_Product")
            F_SKU.Text = Request("F_SKU")
            F_IsActive.Text = Request("F_IsActive")

            F_LLC.DataSource = LLCRow.GetList(DB, "LLC")
            F_LLC.DataTextField = "LLC"
            F_LLC.DataValueField = "LLCID"
            F_LLC.DataBind()
            F_VendorID.DataSource = VendorRow.GetList(DB, "CompanyName")
            F_VendorID.DataValueField = "VendorID"
            F_VendorID.DataTextField = "CompanyName"
            F_VendorID.DataBind()
            F_VendorID.Items.Insert(0, New ListItem("-- ALL --", ""))

            'BindTreeView()
            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "ProductID"

            BindList()
            'ctvSupplyPhase.Attributes.Add("onclick", "OnTreeClick(event)")
        End If
        BindTreeView()
    End Sub

    Private Sub BindTreeView()

        ctvSupplyPhase.DataSource = SupplyPhaseRow.GetList(DB)
        ctvSupplyPhase.DataTextName = "SupplyPhase"
        ctvSupplyPhase.DataValueName = "SupplyPhaseId"
        ctvSupplyPhase.ParentFieldName = "ParentSupplyPhaseId"
        ctvSupplyPhase.DataBind()

    End Sub

    Private Sub BindList()
        Dim res As DataTable = GetListForVendorSku()
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub
        'BindTreeView()
        gvList.PageIndex = 0
        BindList()

    End Sub

    Protected Sub ExportVendorPricing_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExportVendorPricing.Click
        If Not IsValid Then Exit Sub

        ExportListForVendorSku()

    End Sub

    Private Function GetListForVendorSku() As DataTable
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT  TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " v.CompanyName , vpp.VendorID ,vpp.VendorSKU , p.Product , p. SKU  , p.Description ,vpp.VendorPrice  ,p.ProductID ,p.IsActive  "
        SQL = "from product p inner join VendorProductPrice  vpp  on p.ProductID = vpp.ProductID inner join Vendor v on vpp.VendorID = v.VendorID  "

        If Not F_Product.Text = String.Empty Then
            SQL = SQL & Conn & "p.Product LIKE " & DB.FilterQuote(F_Product.Text)
            Conn = " AND "
        End If
        If Not F_SKU.Text = String.Empty Then
            SQL = SQL & Conn & "p.SKU LIKE " & DB.FilterQuote(F_SKU.Text)
            Conn = " AND "
        End If
        If Not F_IsActive.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "p.IsActive  = " & DB.Number(F_IsActive.SelectedValue)
            Conn = " AND "
        End If

        If Not F_VendorID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "v.VendorID = " & DB.Quote(F_VendorID.SelectedValue)
            Conn = " AND "
        End If

        If Not F_LLC.SelectedValues = String.Empty Then
            SQL = SQL & Conn & " v.VendorID in (select VendorID from LLCVendor where LLCID in " & DB.NumberMultiple(F_LLC.SelectedValues) & ")"
            Conn = " AND "
        End If

        If Not ctvSupplyPhase.Value = Nothing Then
            SQL = SQL & Conn & "p.productID IN (SELECT ProductID from supplyphaseproduct where supplyphaseid IN " & DB.NumberMultiple(ctvSupplyPhase.Value) & ")"
            Conn = " AND "
        End If

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Return DB.GetDataTable(SQLFields & SQL & " ORDER BY v.vendorID")
    End Function

    Private Sub ExportListForVendorSku()
        gvList.PageSize = 5000

        Dim res As DataTable = GetListForVendorSku()
        Dim Folder As String = "/assets/catalogs/"
        Dim i As Integer
        Dim FileName As String = Core.GenerateFileID & ".csv"
        Dim listSupplyPhases As Generic.List(Of String) = New Generic.List(Of String)

        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)

        ' Dim sw As StreamWriter = New StreamWriter("C:\Product\product.csv", False)
        sw.WriteLine("Companyname,  ProductName,VendorSKU, ProductSKU ,ProductDescription, Vendorprice,  IsActive , VendorLLC")

        For Each row As DataRow In res.Rows


            Dim ProductName As String = row("Product")


            Dim VendorSKU As String = String.Empty
            If Not IsDBNull(row("VendorSKU")) Then
                VendorSKU = row("VendorSKU")
            Else
                VendorSKU = "  "
            End If

            Dim VendorLLC As String = String.Empty
            If Not IsDBNull(ListLLC(row("VendorID"))) Then
                VendorLLC = ListLLC(row("VendorID"))
            Else
                VendorLLC = " "
            End If


            Dim ProductSKU As String = String.Empty

            If Not IsDBNull(row("SKU")) Then
                ProductSKU = row("SKU")
            Else
                ProductSKU = "  "
            End If


            Dim ProductDescription As String = String.Empty
            If Not IsDBNull(row("Description")) Then
                ProductDescription = row("Description")
            Else
                ProductDescription = "  "
            End If

            Dim vendorprice As String = String.Empty

            If Not IsDBNull(row("VendorPrice")) Then
                vendorprice = row("VendorPrice")
            Else
                vendorprice = "  "
            End If

            Dim companyname As String = String.Empty


            If Not IsDBNull(row("CompanyName")) Then
                companyname = row("CompanyName")
            Else
                companyname = "  "
            End If
            Dim IsActive As String = String.Empty

            If row("IsActive") = True Then
                IsActive = "Yes"
            Else
                IsActive = "No"
            End If

            ' sw.WriteLine(Core.QuoteCSV(SKU) & "," & Core.QuoteCSV(ProductName))
            sw.WriteLine(Core.QuoteCSV(companyname) & "," & Core.QuoteCSV(ProductName) & "," & Core.QuoteCSV(VendorSKU) & "," & Core.QuoteCSV(ProductSKU) & "," & Core.QuoteCSV(ProductDescription) & "," & Core.QuoteCSV(vendorprice) & "," & Core.QuoteCSV(IsActive) & "," & Core.QuoteCSV(VendorLLC))

        Next


        sw.Flush()
        sw.Close()
        sw.Dispose()
        Response.Redirect(Folder & FileName)

    End Sub


    Private Function ListLLC(ByVal VendorID As String) As String

        Dim dtLLCPricing As DataTable = VendorRow.GetLLCList(DB, Vendorid)
        Dim strLLCPricing As String = String.Empty

        For Each row As DataRow In dtLLCPricing.Rows
            strLLCPricing &= row("LLC") & "|"
        Next

        If strLLCPricing <> String.Empty AndAlso strLLCPricing.EndsWith("|") Then strLLCPricing = strLLCPricing.Substring(0, strLLCPricing.Length - 1)

        Return strLLCPricing

    End Function

End Class

