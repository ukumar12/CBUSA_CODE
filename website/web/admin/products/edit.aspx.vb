Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected ProductID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("PRODUCTS")
        'ctlAttributes.ProductTypeId = 2
        ProductID = Convert.ToInt32(Request("ProductID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        drpSizeUnitOfMeasureID.Datasource = UnitOfMeasureRow.GetList(DB, "UnitOfMeasure")
        drpSizeUnitOfMeasureID.DataValueField = "UnitOfMeasureID"
        drpSizeUnitOfMeasureID.DataTextField = "UnitOfMeasure"
        drpSizeUnitOfMeasureID.Databind()
        drpSizeUnitOfMeasureID.Items.Insert(0, New ListItem("", "0"))

        drpWidthUnitOfMeasureID.Datasource = UnitOfMeasureRow.GetList(DB, "UnitOfMeasure")
        drpWidthUnitOfMeasureID.DataValueField = "UnitOfMeasureID"
        drpWidthUnitOfMeasureID.DataTextField = "UnitOfMeasure"
        drpWidthUnitOfMeasureID.Databind()
        drpWidthUnitOfMeasureID.Items.Insert(0, New ListItem("", "0"))

        drpLengthUnitOfMeasureID.Datasource = UnitOfMeasureRow.GetList(DB, "UnitOfMeasure")
        drpLengthUnitOfMeasureID.DataValueField = "UnitOfMeasureID"
        drpLengthUnitOfMeasureID.DataTextField = "UnitOfMeasure"
        drpLengthUnitOfMeasureID.Databind()
        drpLengthUnitOfMeasureID.Items.Insert(0, New ListItem("", "0"))

        drpHeightUnitOfMeasureID.Datasource = UnitOfMeasureRow.GetList(DB, "UnitOfMeasure")
        drpHeightUnitOfMeasureID.DataValueField = "UnitOfMeasureID"
        drpHeightUnitOfMeasureID.DataTextField = "UnitOfMeasure"
        drpHeightUnitOfMeasureID.Databind()
        drpHeightUnitOfMeasureID.Items.Insert(0, New ListItem("", "0"))

        drpProductTypeID.Datasource = ProductTypeRow.GetList(DB, "ProductType")
        drpProductTypeID.DataValueField = "ProductTypeID"
        drpProductTypeID.DataTextField = "ProductType"
        drpProductTypeID.Databind()
        drpProductTypeID.Items.Insert(0, New ListItem("", ""))

        cblcblPricingRequired.DataSource = LLCRow.GetList(DB, "LLC")
        cblcblPricingRequired.DataTextField = "LLC"
        cblcblPricingRequired.DataValueField = "LLCID"
        cblcblPricingRequired.DataBind()


        ctvSupplyPhase.DataSource = SupplyPhaseRow.GetList(DB)
        ctvSupplyPhase.DataTextName = "SupplyPhase"
        ctvSupplyPhase.DataValueName = "SupplyPhaseId"
        ctvSupplyPhase.ParentFieldName = "ParentSupplyPhaseId"
        ctvSupplyPhase.DataBind()

        If ProductID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbProduct As ProductRow = ProductRow.GetRow(DB, ProductID)
        txtProduct.Text = dbProduct.Product
        txtDescription.Text = dbProduct.Description

        Dim dbManufacturer As ManufacturerRow = ManufacturerRow.GetRow(DB, dbProduct.ManufacturerID)
        acManufacturerID.Text = dbManufacturer.Manufacturer

        txtSize.Text = dbProduct.Size
        txtWidth.Text = dbProduct.Width
        txtLength.Text = dbProduct.Length
        txtHeight.Text = dbProduct.Height
        txtGrade.Text = dbProduct.Grade
        drpSizeUnitOfMeasureID.SelectedValue = dbProduct.SizeUnitOfMeasureID
        drpWidthUnitOfMeasureID.SelectedValue = dbProduct.WidthUnitOfMeasureID
        drpLengthUnitOfMeasureID.SelectedValue = dbProduct.LengthUnitOfMeasureID
        drpHeightUnitOfMeasureID.SelectedValue = dbProduct.HeightUnitOfMeasureID
        drpProductTypeID.SelectedValue = dbProduct.ProductTypeID

        ctlAttributes.ProductTypeId = drpProductTypeID.SelectedValue
        Dim dtValues As DataTable = ProductRow.GetAllAttributeValues(DB, dbProduct.ProductID)
        For Each row As DataRow In dtValues.Rows
            ctlAttributes.Value(row("Attribute")) = row("Value")
        Next
        ctlAttributes.DataBind()

        Dim phases As String = dbProduct.GetSelectedSupplyPhases()
        ctvSupplyPhase.Value = phases

        rblIsActive.SelectedValue = dbProduct.IsActive
        cblcblPricingRequired.SelectedValues = dbProduct.GetSelectedLLCProductPriceRequirements
        'lbPricingRequired.SelectedValues = dbProduct.GetSelectedLLCProductPriceRequirements
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Dim SQL As String
        SQL = "Select Product From Product Where Product = " & DB.Quote(txtProduct.Text) & " And ProductId <> " & DB.Number(ProductID)
        If DB.GetDataTable(SQL).Rows.Count > 0 Then
            AddError("The name <b>" & txtProduct.Text & "</b> is being use by another product.  Please select a different one.")
            Exit Sub
        End If
        Try
            DB.BeginTransaction()

            Dim dbProduct As ProductRow

            If ProductID <> 0 Then
                dbProduct = ProductRow.GetRow(DB, ProductID)
            Else
                dbProduct = New ProductRow(DB)
            End If
            dbProduct.Product = txtProduct.Text
            If dbProduct.SKU = Nothing Then
                'dbProduct.SKU = DB.ExecuteScalar("select max(cast(SKU as int)) + 1 from Product")
                dbProduct.SKU = ProductRow.GetNextSKU(DB)
            End If
            dbProduct.Description = txtDescription.Text
            If acManufacturerID.Value <> Nothing Then
                dbProduct.ManufacturerID = acManufacturerID.Value
            Else
                Dim dbManufacturer As ManufacturerRow = ManufacturerRow.GetManufacturerByName(DB, acManufacturerID.Text)
                If dbManufacturer.ManufacturerID = Nothing Then
                    If Not acManufacturerID.Text = String.Empty Then
                        dbManufacturer.Manufacturer = acManufacturerID.Text
                        dbManufacturer.Insert()
                    End If
                End If
                dbProduct.ManufacturerID = dbManufacturer.ManufacturerID
            End If

            If txtSize.Text <> "" Then
                dbProduct.Size = txtSize.Text
            End If

            If txtWidth.Text <> "" Then
                dbProduct.Width = txtWidth.Text
            End If
            If txtLength.Text <> "" Then
                dbProduct.Length = txtLength.Text
            End If
            If txtHeight.Text <> "" Then
                dbProduct.Height = txtHeight.Text
            End If
            If txtGrade.Text <> "" Then
                dbProduct.Grade = txtGrade.Text
            End If

            dbProduct.SizeUnitOfMeasureID = drpSizeUnitOfMeasureID.SelectedValue
            dbProduct.WidthUnitOfMeasureID = drpWidthUnitOfMeasureID.SelectedValue
            dbProduct.LengthUnitOfMeasureID = drpLengthUnitOfMeasureID.SelectedValue
            dbProduct.HeightUnitOfMeasureID = drpHeightUnitOfMeasureID.SelectedValue
            dbProduct.ProductTypeID = drpProductTypeID.SelectedValue
            dbProduct.IsActive = rblIsActive.SelectedValue

            If ProductID <> 0 Then
                dbProduct.UpdaterAdminID = CType(Page, AdminPage).LoggedInAdminId
                dbProduct.Update()
            Else
                dbProduct.CreatorAdminID = CType(Page, AdminPage).LoggedInAdminId
                ProductID = dbProduct.Insert
            End If

            'clear any existing 
            dbProduct.DeleteAllAttributeValues()

            Dim dtAttributes As DataTable = ProductTypeAttributeRow.GetListByType(DB, dbProduct.ProductTypeID)
            For Each row As DataRow In dtAttributes.Rows
                Dim dbValue As New ProductTypeAttributeProductValueRow(DB)
                dbValue.ProductTypeAttributeID = row("ProductTypeAttributeId")
                dbValue.Value = ctlAttributes.Value(row("Attribute"))
                dbValue.ProductID = dbProduct.ProductID
                dbValue.Insert()
            Next

            dbProduct.DeleteFromAllLLCProductPriceRequirements()
            dbProduct.InsertToLLCProductPriceRequirements(cblcblPricingRequired.SelectedValues)

            dbProduct.DeleteFromAllSupplyPhases()
            dbProduct.InsertToSupplyPhases(ctvSupplyPhase.Value)

            DB.CommitTransaction()


            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?ProductID=" & ProductID & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub drpProductTypeID_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles drpProductTypeID.SelectedIndexChanged
        ctlAttributes.ProductTypeId = drpProductTypeID.SelectedValue
        ctlAttributes.DataBind()
        upAttributes.Update()
    End Sub
End Class

