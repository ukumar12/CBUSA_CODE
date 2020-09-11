Imports Components
Imports DataLayer

Partial Class forms_products_recommend
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureVendorAccess()

        If Not IsPostBack Then
            drpProductType.DataSource = ProductTypeRow.GetList(DB)
            drpProductType.DataTextField = "ProductType"
            drpProductType.DataValueField = "ProductTypeId"
            drpProductType.DataBind()
        End If
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not Page.IsValid Then Exit Sub

        Dim dbProduct As New RecommendedProductRow(DB)
        With dbProduct
            .RecommendedProduct = txtProductName.Text
            .Description = txtDescription.Text
            .Grade = txtGrade.Text
            .ManufacturerID = acManufacturer.Value
            .ProductTypeID = drpProductType.SelectedValue
            .Size = txtSize.Text
            .UnitOfMeasureID = drpUnitOfMeasure.SelectedValue
            '.Submitted = Now
            .VendorID = Session("VendorId")
        End With

        dbProduct.Insert()

        Dim emailList As String = SysParam.GetValue(DB, "RecommendProductEmailList")
        Dim emails() As String = emailList.Split(",")
        Dim fromEmail As String = SysParam.GetValue(DB, "ContactUsEmail")
        Dim fromName As String = SysParam.GetValue(DB, "ContactUsName")

        Dim msg As String = "A new recommended product has been submitted on CBUSA.us:" & vbCrLf & vbCrLf
        msg &= "Product Name: " & dbProduct.RecommendedProduct & vbCrLf
        msg &= "Description: " & dbProduct.Description & vbCrLf & vbCrLf
        msg &= "Manufacturer: " & acManufacturer.Text
        msg &= "Size: " & txtSize.Text
        msg &= "Unit: " & drpUnitOfMeasure.SelectedItem.Text
        msg &= "Grade: " & dbProduct.Grade
        msg &= "Product Type: " & drpProductType.SelectedItem.Text

        For Each toEmail As String In emails
            Core.SendSimpleMail(fromEmail, fromName, toEmail, toEmail, "New CBUSA.us recommended product", msg)
        Next

        Response.Redirect("/products/default.aspx")
    End Sub
End Class
