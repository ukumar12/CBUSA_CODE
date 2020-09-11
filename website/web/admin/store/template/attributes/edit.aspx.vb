Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected TemplateAttributeId As Integer
    Private objTemplate As StoreItemTemplateRow
	Protected IsInUse As Boolean

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("STORE")

        TemplateAttributeId = Convert.ToInt32(Request("TemplateAttributeId"))

        objTemplate = StoreItemTemplateRow.GetRow(DB, Request("TemplateId"))
        If objTemplate.TemplateId = Nothing Then
            DB.Close()
            Response.Redirect("/admin/store/template/")
        End If

		IsInUse = Not DB.ExecuteScalar("select top 1 itemid from storeitem where templateid = " & DB.Number(Request("TemplateId"))) = Nothing
		drpParentId.Enabled = Not IsInUse

		trInventory.Visible = DB.ExecuteScalar("select top 1 templateattributeid from storeitemtemplateattribute where isinventorymanagement = 1 and templateid = " & DB.Number(Request("TemplateId")) & " and templateattributeid <> " & TemplateAttributeId) = Nothing

        If Not IsPostBack Then
            ltlTemplateName.Text = objTemplate.TemplateName
            LoadFromDB()
        End If

		RefreshPage()
    End Sub

    Private Sub LoadFromDB()
		drpTable.DataSource = DB.GetDataTable("select table_name from information_schema.tables where table_name like 'Lookup%' order by table_name")
		drpTable.DataTextField = "table_name"
		drpTable.DataValueField = "table_name"
		drpTable.DataBind()
		drpTable.Items.Insert(0, New ListItem("", ""))

		drpParentId.DataSource = DB.GetDataTable("select * from storeitemtemplateattribute where templateid = " & objTemplate.TemplateId)
		drpParentId.DataTextField = "AttributeName"
		drpParentId.DataValueField = "TemplateAttributeId"
		drpParentId.DataBind()
		drpParentId.Items.Insert(0, New ListItem("", "0"))

        If TemplateAttributeId = 0 Then
            btnDelete.Visible = False
			rbFreeText.Checked = True
            Exit Sub
        End If

        Dim dbStoreItemTemplateAttribute As StoreItemTemplateAttributeRow = StoreItemTemplateAttributeRow.GetRow(DB, TemplateAttributeId)
		If trInventory.Visible AndAlso dbStoreItemTemplateAttribute.ParentId > 0 Then chkIsInventoryManagement.Enabled = False
        txtAttributeName.Text = dbStoreItemTemplateAttribute.AttributeName
        drpAttributeType.SelectedValue = dbStoreItemTemplateAttribute.AttributeType
		txtSpecifyValue.Text = dbStoreItemTemplateAttribute.SpecifyValue
		drpTable.SelectedValue = dbStoreItemTemplateAttribute.LookupTable
		If drpTable.SelectedValue <> Nothing Then
			BindColumns()
		End If
		drpColumn.SelectedValue = dbStoreItemTemplateAttribute.LookupColumn
		drpSKU.SelectedValue = dbStoreItemTemplateAttribute.SKUColumn
		chkInclude.Checked = dbStoreItemTemplateAttribute.IncludeSKU
		drpPrice.SelectedValue = dbStoreItemTemplateAttribute.PriceColumn
		drpWeight.SelectedValue = dbStoreItemTemplateAttribute.WeightColumn
		drpSwatch.SelectedValue = dbStoreItemTemplateAttribute.SwatchColumn
		drpAlt.SelectedValue = dbStoreItemTemplateAttribute.SwatchAltColumn
		drpParentId.SelectedValue = dbStoreItemTemplateAttribute.ParentId
		chkIsInventoryManagement.Checked = dbStoreItemTemplateAttribute.IsInventoryManagement
		Select Case dbStoreItemTemplateAttribute.FunctionType
			Case "SpecifyValue"
				rbSpecifyValue.Checked = True
			Case "LookupTable"
				rbLookupTable.Checked = True
			Case Else
				rbFreeText.Checked = True
		End Select
    End Sub

	Private Sub RefreshPage()
		trSpecifyValue.Visible = rbSpecifyValue.Checked
		rfvtxtSpecifyValue.Enabled = rbSpecifyValue.Checked
		trLookupTable.Visible = rbLookupTable.Checked
		rfvdrpTable.Enabled = rbLookupTable.Checked
		trLookupSKU.Visible = rbLookupTable.Checked
		trLookupPrice.Visible = rbLookupTable.Checked
		trLookupWeight.Visible = rbLookupTable.Checked
		trLookupSwatch.Visible = rbLookupTable.Checked AndAlso drpAttributeType.SelectedValue = "swatch"
		trAltSwatch.Visible = trLookupSwatch.Visible
		If drpTable.SelectedValue = String.Empty OrElse Not rbLookupTable.Checked Then
			trLookupColumn.Visible = False
		Else
			trLookupColumn.Visible = True
		End If
	End Sub

	Private Sub BindColumns()
		Dim dt As DataTable = DB.GetDataTable("select column_name from information_schema.columns where table_name = " & DB.Quote(drpTable.SelectedValue))

		drpColumn.DataSource = dt
		drpColumn.DataTextField = "column_name"
		drpColumn.DataValueField = "column_name"
		drpColumn.DataBind()

		drpSKU.DataSource = dt
		drpSKU.DataTextField = "column_name"
		drpSKU.DataValueField = "column_name"
		drpSKU.DataBind()
		drpSKU.Items.Insert(0, New ListItem("", ""))

		drpSwatch.DataSource = dt
		drpSwatch.DataTextField = "column_name"
		drpSwatch.DataValueField = "column_name"
		drpSwatch.DataBind()
		drpSwatch.Items.Insert(0, New ListItem("", ""))

		drpAlt.DataSource = dt
		drpAlt.DataTextField = "column_name"
		drpAlt.DataValueField = "column_name"
		drpAlt.DataBind()
		drpAlt.Items.Insert(0, New ListItem("", ""))

		dt = DB.GetDataTable("select column_name from information_schema.columns where data_type in ('int','float','money') and table_name = " & DB.Quote(drpTable.SelectedValue))
		drpPrice.DataSource = dt
		drpPrice.DataTextField = "column_name"
		drpPrice.DataValueField = "column_name"
		drpPrice.DataBind()
		drpPrice.Items.Insert(0, New ListItem("", ""))

		drpWeight.DataSource = dt
		drpWeight.DataTextField = "column_name"
		drpWeight.DataValueField = "column_name"
		drpWeight.DataBind()
		drpWeight.Items.Insert(0, New ListItem("", ""))
	End Sub

	Protected Sub drpTable_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles drpTable.SelectedIndexChanged
		BindColumns()
	End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		Page.Validate()
        If Not IsValid Then Exit Sub

		If Not DB.ExecuteScalar("select top 1 templateattributeid from storeitemtemplateattribute where templateattributeid <> " & TemplateAttributeId & " AND parentid = " & drpParentId.SelectedValue) = Nothing Then
			AddError("Another attribute in this template has already been assigned as a child to the selected parent attribute.")
			Exit Sub
		End If

        Try
            DB.BeginTransaction()

            Dim dbStoreItemTemplateAttribute As StoreItemTemplateAttributeRow

            If TemplateAttributeId <> 0 Then
                dbStoreItemTemplateAttribute = StoreItemTemplateAttributeRow.GetRow(DB, TemplateAttributeId)
            Else
                dbStoreItemTemplateAttribute = New StoreItemTemplateAttributeRow(DB)
            End If

			dbStoreItemTemplateAttribute.SpecifyValue = Nothing
			dbStoreItemTemplateAttribute.LookupTable = Nothing
			dbStoreItemTemplateAttribute.LookupColumn = Nothing
			dbStoreItemTemplateAttribute.SKUColumn = Nothing
			dbStoreItemTemplateAttribute.PriceColumn = Nothing
			dbStoreItemTemplateAttribute.WeightColumn = Nothing
			dbStoreItemTemplateAttribute.SwatchColumn = Nothing

			If rbSpecifyValue.Checked Then
				dbStoreItemTemplateAttribute.FunctionType = "SpecifyValue"
				dbStoreItemTemplateAttribute.SpecifyValue = txtSpecifyValue.Text
			ElseIf rbLookupTable.Checked Then
				dbStoreItemTemplateAttribute.FunctionType = "LookupTable"
				dbStoreItemTemplateAttribute.LookupTable = drpTable.SelectedValue
				dbStoreItemTemplateAttribute.LookupColumn = drpColumn.SelectedValue
				dbStoreItemTemplateAttribute.SKUColumn = drpSKU.SelectedValue
				dbStoreItemTemplateAttribute.IncludeSKU = chkInclude.Checked
				dbStoreItemTemplateAttribute.PriceColumn = drpPrice.SelectedValue
				dbStoreItemTemplateAttribute.WeightColumn = drpWeight.SelectedValue
				dbStoreItemTemplateAttribute.SwatchColumn = drpSwatch.SelectedValue
				dbStoreItemTemplateAttribute.SwatchAltColumn = drpAlt.SelectedValue
			Else
				dbStoreItemTemplateAttribute.FunctionType = "FreeText"
			End If
            dbStoreItemTemplateAttribute.TemplateId = objTemplate.TemplateId
            dbStoreItemTemplateAttribute.AttributeName = txtAttributeName.Text
            dbStoreItemTemplateAttribute.AttributeType = drpAttributeType.SelectedValue
			dbStoreItemTemplateAttribute.ParentId = drpParentId.SelectedValue
			If dbStoreItemTemplateAttribute.ParentId = Nothing Then dbStoreItemTemplateAttribute.IsInventoryManagement = chkIsInventoryManagement.Checked

            If TemplateAttributeId <> 0 Then dbStoreItemTemplateAttribute.Update() Else TemplateAttributeId = dbStoreItemTemplateAttribute.Insert

			If objTemplate.DisplayMode = "TableLayout" AndAlso dbStoreItemTemplateAttribute.ParentId = Nothing AndAlso DB.ExecuteScalar("select count(*) from storeitemtemplateattribute where parentid is null and templateid = " & objTemplate.TemplateId) > 1 Then
				Throw New ApplicationException("You cannot add multiple attribute roots to templates with a Display Type of Table Layout.")
			End If

            DB.CommitTransaction()
            Response.Redirect("default.aspx?TemplateId=" & objTemplate.TemplateId & "&" & GetPageParams(FilterFieldType.All))

		Catch ex As ApplicationException
			If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
			AddError(ex.Message)
        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?TemplateId=" & objTemplate.TemplateId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?TemplateId=" & objTemplate.TemplateId & "&TemplateAttributeId=" & TemplateAttributeId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
