Imports Components
Imports DataLayer

Partial Class forms_products_catalog_default
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'EnsureBuilderAccess()

        If Not IsPostBack Then
            ctvSupplyPhases.DataSource = SupplyPhaseRow.GetList(DB)
            ctvSupplyPhases.DataTextName = "SupplyPhase"
            ctvSupplyPhases.DataValueName = "SupplyPhaseId"
            ctvSupplyPhases.ParentFieldName = "ParentSupplyPhaseId"
            ctvSupplyPhases.DataBind()
        End If
    End Sub

    Protected Sub btnProcess_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnProcess.Click
        If Not IsValid Then Exit Sub

        If rblFormat.SelectedValue = "HTML" Then
            BindList()
        Else
            SaveExport()
        End If
    End Sub

    Private Sub BindList()
        Dim VendorId As Integer = IIf(acVendor.Value = String.Empty, Nothing, acVendor.Value)
        Dim dt As DataTable = ProductRow.GetSupplyPhaseProducts(DB, ctvSupplyPhases.Value, VendorId)

        gvList.DataSource = dt
        gvList.DataBind()
    End Sub

    Private Sub SaveExport()
        Dim VendorId As Integer = IIf(acVendor.Value = String.Empty, Nothing, acVendor.Value)
        Dim dt As DataTable = ProductRow.GetSupplyPhaseProducts(DB, ctvSupplyPhases.Value, VendorId)

        Dim fname As String = "/assets/catalogs/" & Core.GenerateFileID & ".csv"
        Dim sw As IO.StreamWriter = IO.File.CreateText(Server.MapPath(fname))
        sw.WriteLine("CBUSA Product Catalog")
        sw.Write("Supply Phase(s):,")
        sw.WriteLine(Core.QuoteCSV(SupplyPhaseRow.GetNames(DB, ctvSupplyPhases.Value)))
        If VendorId <> Nothing Then
            sw.Write("Vendor:,")
            sw.WriteLine(Core.QuoteCSV(acVendor.Text))
        End If
        sw.WriteLine(vbCrLf)
        If VendorId = Nothing Then
            sw.WriteLine("CBUSA SKU,Name,Size")
        Else
            sw.WriteLine("CBUSA SKU,Name,Size,Vendor SKU,Vendor Price")
        End If
        For Each row As DataRow In dt.Rows
            sw.WriteLine(Core.QuoteCSV(row("SKU")) & "," & Core.QuoteCSV(row("Product")) & "," & Core.QuoteCSV(row("Size")))
        Next
        sw.Close()
        ltlDownloadLink.Text = "<a href=""" & fname & """>Click here to download Excel file</a>"
    End Sub
End Class
