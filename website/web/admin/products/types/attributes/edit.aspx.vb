Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected ProductTypeAttributeID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("PRODUCT_TYPES")

		ProductTypeAttributeID = Convert.ToInt32(Request("ProductTypeAttributeID"))
		If Not IsPostBack Then
            LoadFromDB()
            ScriptManager.RegisterStartupScript(Me, Me.GetType, "InitForm", "InputTypeChanged();", True)
		End If
	End Sub

	Private Sub LoadFromDB()
		drpProductTypeID.Datasource = ProductTypeRow.GetList(DB,"ProductType")
		drpProductTypeID.DataValueField = "ProductTypeID"
		drpProductTypeID.DataTextField = "ProductType"
		drpProductTypeID.Databind
		drpProductTypeID.Items.Insert(0, New ListItem("",""))

        drpInputType.Items.Add(New ListItem("Text", InputType.Text))
        drpInputType.Items.Add(New ListItem("Number", InputType.Number))
        drpInputType.Items.Add(New ListItem("Dropdown List", InputType.Dropdown))
        drpInputType.Items.Add(New ListItem("Yes/No", InputType.YesNo))

		If ProductTypeAttributeID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbProductTypeAttribute As ProductTypeAttributeRow = ProductTypeAttributeRow.GetRow(DB, ProductTypeAttributeID)
		txtAttribute.Text = dbProductTypeAttribute.Attribute
        drpInputType.SelectedValue = dbProductTypeAttribute.InputType
        Select Case dbProductTypeAttribute.InputType
            Case InputType.Text, InputType.Number
                txtDefaultValue.Text = dbProductTypeAttribute.DefaultValue
            Case InputType.YesNo
                rbDefaultYes.Checked = dbProductTypeAttribute.DefaultValue
                rbDefaultNo.Checked = Not dbProductTypeAttribute.DefaultValue
            Case InputType.Dropdown
                Dim dtOptions As DataTable = ProductTypeAttributeRow.GetOptions(DB, ProductTypeAttributeID)
                ctrlOptions.Data = dtOptions.DefaultView

                drpDefaultValue.DataSource = dtOptions
                drpDefaultValue.DataTextField = "ValueOption"
                drpDefaultValue.DataValueField = "ValueOption"
                drpDefaultValue.DataBind()
                If dbProductTypeAttribute.DefaultValue <> String.Empty Then
                    drpDefaultValue.SelectedValue = dbProductTypeAttribute.DefaultValue
                End If
        End Select

        If Not dbProductTypeAttribute.IsRequired Then
            trDefault.Style.Item("display") = "none"
        End If

		drpProductTypeID.SelectedValue = dbProductTypeAttribute.ProductTypeID
		rblIsRequired.SelectedValue = dbProductTypeAttribute.IsRequired
		rblIsActive.SelectedValue = dbProductTypeAttribute.IsActive
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbProductTypeAttribute As ProductTypeAttributeRow

			If ProductTypeAttributeID <> 0 Then
				dbProductTypeAttribute = ProductTypeAttributeRow.GetRow(DB, ProductTypeAttributeID)
			Else
				dbProductTypeAttribute = New ProductTypeAttributeRow(DB)
			End If
			dbProductTypeAttribute.Attribute = txtAttribute.Text
            dbProductTypeAttribute.InputType = drpInputType.SelectedValue
            Select Case dbProductTypeAttribute.InputType
                Case InputType.Text, InputType.Number
                    dbProductTypeAttribute.DefaultValue = txtDefaultValue.Text
                Case InputType.Dropdown
                    dbProductTypeAttribute.DefaultValue = drpDefaultValue.SelectedValue
                Case InputType.YesNo
                    dbProductTypeAttribute.DefaultValue = rbDefaultYes.Checked
            End Select
            dbProductTypeAttribute.ProductTypeID = drpProductTypeID.SelectedValue
            dbProductTypeAttribute.IsRequired = rblIsRequired.SelectedValue
            dbProductTypeAttribute.IsActive = rblIsActive.SelectedValue

            If ProductTypeAttributeID <> 0 Then
                dbProductTypeAttribute.Update()
            Else
                ProductTypeAttributeID = dbProductTypeAttribute.Insert
            End If

            If dbProductTypeAttribute.InputType = InputType.Dropdown Then
                dbProductTypeAttribute.ClearOptions()
                Dim aRows As DataRow() = ctrlOptions.GetData()
                For Each row As DataRow In aRows
                    If row("txtValue") <> Nothing Then
                        Dim dbOption As New ProductTypeAttributeValueOptionRow(DB)
                        dbOption.ProductTypeAttributeID = dbProductTypeAttribute.ProductTypeAttributeID
                        dbOption.ValueOption = row("txtValue")
                        dbOption.Insert()
                    End If
                Next
            End If

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
		Response.Redirect("delete.aspx?ProductTypeAttributeID=" & ProductTypeAttributeID & "&" & GetPageParams(FilterFieldType.All))
	End Sub

    Protected Sub cvDefault_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles cvDefault.ServerValidate
        Select Case drpInputType.SelectedValue
            Case InputType.YesNo
                args.IsValid = Not (rblIsRequired.SelectedValue And Not (rbDefaultYes.Checked Or rbDefaultNo.Checked))
            Case InputType.Text
                args.IsValid = Not (rblIsRequired.SelectedValue And txtDefaultValue.Text <> String.Empty)
            Case InputType.Number
                args.IsValid = Not (rblIsRequired.SelectedValue And (txtDefaultValue.Text = String.Empty OrElse Not IsNumeric(txtDefaultValue.Text)))
            Case Else
                args.IsValid = True
        End Select
    End Sub

    Protected Sub cvUniqueName_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles cvUniqueName.ServerValidate
        Dim dt As DataTable = ProductTypeAttributeRow.GetListByType(DB, drpProductTypeID.SelectedValue)
        Dim rows As DataRow() = dt.Select("Attribute=" & DB.Quote(txtAttribute.Text))

        If rows.Length = 0 Then
            args.IsValid = True
        ElseIf rows(0)("ProductTypeAttributeId") = ProductTypeAttributeID Then
            args.IsValid = True
        End If
        'args.IsValid = (rows.Length = 0 Or rows(0)("ProductTypeAttributeId") = ProductTypeAttributeID)
    End Sub
End Class
