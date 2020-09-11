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
            F_Manufacturer.Text = Request("F_Manufacturer")
          
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

    Private Function GetListForExport() As DataTable
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT  TOP " & (gvList.PageIndex + 1) * gvList.PageSize & "   p. * , m.Manufacturer"
        SQL = " from product p left outer join manufacturer m on p.manufacturerid=m.manufacturerid"

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
        If Not F_Manufacturer.Value = Nothing Then
            SQL = SQL & Conn & "p.ManufacturerId=" & DB.Number(F_Manufacturer.Value)
            Conn = " AND "
        End If

        If Not ctvSupplyPhase.Value = Nothing Then
            SQL = SQL & Conn & "p.productID IN (SELECT ProductID from supplyphaseproduct where supplyphaseid IN " & DB.NumberMultiple(ctvSupplyPhase.Value) & ")"
            Conn = " AND "
        End If

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Return DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
    End Function





    Private Sub BindList()
        Dim res As DataTable = GetListForExport()
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub
        'BindTreeView()
        gvList.PageIndex = 0
        BindList()

    End Sub

	Protected Sub Categorize_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Categorize.Click
		Response.Redirect("categorize.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub
    Protected Sub export_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Export.Click
        If Not IsValid Then Exit Sub


        exportproduct()

    End Sub




    'Protected Sub ExportVendorPricing_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExportVendorPricing.Click
    '    If Not IsValid Then Exit Sub


    '    ExportListForVendorSku()

    'End Sub

    Private Sub exportproduct()
        gvList.PageSize = 35000

        Dim res As DataTable = GetListForExport()
        Dim Folder As String = "/assets/catalogs/"
        Dim i As Integer
        Dim FileName As String = Core.GenerateFileID & ".csv"
        Dim listSupplyPhases As Generic.List(Of String) = New Generic.List(Of String)

        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)

        ' Dim sw As StreamWriter = New StreamWriter("C:\Product\product.csv", False)
        sw.WriteLine("SKU, Name, Supply Phase 1, Supply Phase 2 , Supply Phase 3, Supply Phase 4, Supply Phase 5 , LLC Pricing, Description, IsActive")

        For Each row As DataRow In res.Rows

            Dim SKU As String = row("SKU")
            Dim ProductName As String = row("Product")


            Dim IsActive As String = String.Empty
            Dim LLCPricing As String = String.Empty
            Dim Phase1 As String = String.Empty
            Dim Phase2 As String = String.Empty
            Dim Phase3 As String = String.Empty
            Dim Phase4 As String = String.Empty
            Dim Phase5 As String = String.Empty

            LLCPricing = ListLLCPricing(row("ProductID"))
            listSupplyPhases = ListSupplyPhase(row("productid"))

            For i = 0 To listSupplyPhases.Count - 1
                Select Case i
                    Case 0
                        Phase1 = listSupplyPhases(i)
                    Case 1
                        Phase2 = listSupplyPhases(i)
                    Case 2
                        Phase3 = listSupplyPhases(i)
                    Case 3
                        Phase4 = listSupplyPhases(i)
                    Case 4
                        Phase5 = listSupplyPhases(i)

                End Select
            Next

            Dim Description As String = String.Empty
            If Not IsDBNull(row("Description")) Then
                Description = row("Description")
            Else
                Description = "  "
            End If
            If row("IsActive") = True Then
                IsActive = "Yes"
            Else
                IsActive = "No"
            End If

            ' sw.WriteLine(Core.QuoteCSV(SKU) & "," & Core.QuoteCSV(ProductName))
            sw.WriteLine(Core.QuoteCSV(SKU) & "," & Core.QuoteCSV(ProductName) & "," & Core.QuoteCSV(Phase1) & "," & Core.QuoteCSV(Phase2) & "," & Core.QuoteCSV(Phase3) & "," & Core.QuoteCSV(Phase4) & "," & Core.QuoteCSV(Phase5) & "," & Core.DblQuote(LLCPricing) & "," & Core.QuoteCSV(Description) & "," & Core.QuoteCSV(IsActive))

        Next


        sw.Flush()
        sw.Close()
        sw.Dispose()
        Response.Redirect(Folder & FileName)

    End Sub

    Private Function ListSupplyPhase(ByVal ProductID As String) As Generic.List(Of String)

        supphase = SupplyPhaseRow.GetSupplierPhaseList(DB, ProductID)
        Dim SupplyProductphase As Generic.List(Of String) = New Generic.List(Of String)

        For Each row As DataRow In supphase.Rows
            supplyPhases = String.Empty
            Dim supplyPhase As String = GetChildSupplyPhase(Convert.ToInt32(row("supplyphaseid")))
            If supplyPhase.EndsWith("|") Then supplyPhase = supplyPhase.Substring(0, supplyPhase.Length - 1)

            SupplyProductphase.Add(supplyPhase)
        Next

        Return SupplyProductphase

    End Function


    Public Function GetChildSupplyPhase(ByVal ParentID As Integer) As String
        Dim supplyphasedata As DataTable = SupplyPhaseRow.GetList(DB)
        Dim supplyPhase() As DataRow = supplyphasedata.Select("SupplyPhaseId=" & ParentID)

        If (supplyPhase.count > 0) Then

            If Not (supplyPhase(0)("SupplyPhase").ToString().ToLower().Trim().Contains("root")) Then
                supplyPhases = supplyPhase(0)("SupplyPhase") & "|" & supplyPhases
                GetChildSupplyPhase(Convert.ToInt32(supplyPhase(0)("ParentSupplyPhaseId")))
            End If

        End If

        Return supplyPhases

    End Function

    Private Function ListLLCPricing(ByVal ProductID As String) As String

        Dim dtLLCPricing As DataTable = SupplyPhaseRow.GetLLCPricingList(DB, ProductID)
        Dim strLLCPricing As String = String.Empty

        For Each row As DataRow In dtLLCPricing.Rows
            strLLCPricing &= row("LLC") & "|"
        Next

        If strLLCPricing <> String.Empty AndAlso strLLCPricing.EndsWith("|") Then strLLCPricing = strLLCPricing.Substring(0, strLLCPricing.Length - 1)

        Return strLLCPricing

    End Function




    'Private Function GetListForVendorSku() As DataTable
    '    Dim SQLFields, SQL As String
    '    Dim Conn As String = " where "

    '    ViewState("F_SortBy") = gvList.SortBy
    '    ViewState("F_SortOrder") = gvList.SortOrder

    '    SQLFields = "SELECT  TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " v.CompanyName , vpp.VendorID ,vpp.VendorSKU , p.Product , p. SKU  , p.Description ,vpp.VendorPrice  ,p.ProductID ,p.IsActive  "
    '    SQL = "from product p left outer join VendorProductPrice  vpp  on p.ProductID = vpp.ProductID left outer join Vendor v on vpp.VendorID = v.VendorID  "

    '    If Not F_Product.Text = String.Empty Then
    '        SQL = SQL & Conn & "p.Product LIKE " & DB.FilterQuote(F_Product.Text)
    '        Conn = " AND "
    '    End If
    '    If Not F_SKU.Text = String.Empty Then
    '        SQL = SQL & Conn & "p.SKU LIKE " & DB.FilterQuote(F_SKU.Text)
    '        Conn = " AND "
    '    End If
    '    If Not F_IsActive.SelectedValue = String.Empty Then
    '        SQL = SQL & Conn & "p.IsActive  = " & DB.Number(F_IsActive.SelectedValue)
    '        Conn = " AND "
    '    End If


    '    If Not ctvSupplyPhase.Value = Nothing Then
    '        SQL = SQL & Conn & "p.productID IN (SELECT ProductID from supplyphaseproduct where supplyphaseid IN " & DB.NumberMultiple(ctvSupplyPhase.Value) & ")"
    '        Conn = " AND "
    '    End If

    '    gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

    '    Return DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
    'End Function

    'Private Sub ExportListForVendorSku()
    '    gvList.PageSize = 5000

    '    Dim res As DataTable = GetListForVendorSku()
    '    Dim Folder As String = "/assets/catalogs/"
    '    Dim i As Integer
    '    Dim FileName As String = Core.GenerateFileID & ".csv"
    '    Dim listSupplyPhases As Generic.List(Of String) = New Generic.List(Of String)

    '    Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)

    '    ' Dim sw As StreamWriter = New StreamWriter("C:\Product\product.csv", False)
    '    sw.WriteLine("companyname,  ProductName,VendorSKU, ProductSKU ,ProductDescription, vendorprice,  IsActive")

    '    For Each row As DataRow In res.Rows


    '        Dim ProductName As String = row("Product")


    '        Dim VendorSKU As String = String.Empty
    '        If Not IsDBNull(row("VendorSKU")) Then
    '            VendorSKU = row("VendorSKU")
    '        Else
    '            VendorSKU = "  "
    '        End If


    '        Dim ProductSKU As String = String.Empty

    '        If Not IsDBNull(row("SKU")) Then
    '            ProductSKU = row("SKU")
    '        Else
    '            ProductSKU = "  "
    '        End If


    '        Dim ProductDescription As String = String.Empty
    '        If Not IsDBNull(row("Description")) Then
    '            ProductDescription = row("Description")
    '        Else
    '            ProductDescription = "  "
    '        End If

    '        Dim vendorprice As String = String.Empty

    '        If Not IsDBNull(row("VendorPrice")) Then
    '            vendorprice = row("VendorPrice")
    '        Else
    '            vendorprice = "  "
    '        End If

    '        Dim companyname As String = String.Empty


    '        If Not IsDBNull(row("CompanyName")) Then
    '            companyname = row("CompanyName")
    '        Else
    '            companyname = "  "
    '        End If
    '        Dim IsActive As String = String.Empty

    '        If row("IsActive") = True Then
    '            IsActive = "Yes"
    '        Else
    '            IsActive = "No"
    '        End If

    '        ' sw.WriteLine(Core.QuoteCSV(SKU) & "," & Core.QuoteCSV(ProductName))
    '        sw.WriteLine(Core.QuoteCSV(companyname) & "," & Core.QuoteCSV(ProductName) & "," & Core.QuoteCSV(VendorSKU) & "," & Core.QuoteCSV(ProductSKU) & "," & Core.QuoteCSV(ProductDescription) & "," & Core.QuoteCSV(vendorprice) & "," & Core.QuoteCSV(IsActive))

    '    Next


    '    sw.Flush()
    '    sw.Close()
    '    sw.Dispose()
    '    Response.Redirect(Folder & FileName)

    'End Sub

End Class

