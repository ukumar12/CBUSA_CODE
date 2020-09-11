Imports Components
Imports DataLayer
Imports System.Data.SqlClient
Imports System.Linq
Imports System.Collections.Generic

Partial Class admin_NationalContracts_Edit
    Inherits AdminPage

    Protected ContractID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ContractID = Convert.ToInt32(Request("ContractID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        Dim dt As DataTable = LLCRow.GetList(DB, "LLC", "ASC")

        Dim allRow As DataRow = dt.NewRow
        allRow.ItemArray = dt.Rows(0).ItemArray
        allRow("LLC") = "All"
        allRow("LLCID") = "0"
        dt.Rows.InsertAt(allRow, 0)

        cblLLC.DataSource = dt
        cblLLC.DataTextField = "LLC"
        cblLLC.DataValueField = "LLCID"
        cblLLC.DataBind()

        If ContractID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbContract As NationalContractRow = NationalContractRow.GetRow(DB, ContractID)
        txtManufacturer.Text = dbContract.Manufacturer
        txtManufacturerSite.Text = dbContract.ManufacturerSite
        txtTitle.Text = dbContract.Title
        txtProducts.Text = dbContract.Products
        dpEndDate.Value = dbContract.EndDate
        dpStartDate.Value = dbContract.StartDate
        txtDetail.Text = dbContract.DescriptionPage
        rblArchiveDate.SelectedValue = dbContract.ArchiveDate
        txtContractTerm.Text = dbContract.ContractTerm

        cblLLC.SelectedValues = String.Join(",", dbContract.GetLLCs.Select(Function(x) CStr(x.LLCID)).ToArray)

        BindBuilders()
        hdnBuilders.Value = String.Join(",", dbContract.GetBuilders.Select(Function(x) CStr(x.BuilderID)).ToArray)
    End Sub

    Protected Sub BindBuilders()
        Dim dtBuilders As DataTable = DB.GetDataTable("SELECT BuilderID, Companyname, builder.LLCID,LLC  FROM Builder " & _
                                " INNER JOIN LLC " & _
                                " ON Builder.LLCID = LLC.LLCID " & _
                                " WHERE builder.LLCID IN " & DB.NumberMultiple(cblLLC.SelectedValues) & _
                                " AND builder.IsActive = 1" & _
                                " ORDER BY LLC,CompanyName")
        drpbuilders.DataSource = dtBuilders
        drpbuilders.DataGroupField = "LLC"
        drpbuilders.DataValueField = "BuilderId"
        drpbuilders.DataTextField = "CompanyName"
        drpbuilders.DataBind()
        drpbuilders.ClearSelection()
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbContract As NationalContractRow

            If ContractID <> 0 Then
                dbContract = NationalContractRow.GetRow(DB, ContractID)
            Else
                dbContract = New NationalContractRow(DB)
            End If

            dbContract.Manufacturer = txtManufacturer.Text
            dbContract.ManufacturerSite = txtManufacturerSite.Text
            dbContract.Products = txtProducts.Text
            dbContract.Title = txtTitle.Text
            dbContract.DescriptionPage = txtDetail.Text
            dbContract.StartDate = dpStartDate.Value
            dbContract.EndDate = dpEndDate.Value
            dbContract.ArchiveDate = rblArchiveDate.SelectedValue
            dbContract.ContractTerm = txtContractTerm.Text
            If ContractID <> 0 Then
                dbContract.Update()
            Else
                ContractID = dbContract.Insert
            End If

            Dim ids As List(Of String) = cblLLC.SelectedValues.Split(",").ToList
            ids.Remove("0")
            dbContract.UpdateLLCs(ids.Where(Function(x) IsNumeric(x)).Select(Function(x) CStr(x)).ToList)

            dbContract.UpdateBuilders(hdnBuilders.Value.Split(",").ToList)

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
        Response.Redirect("delete.aspx?OrderID=" & ContractID & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub cblLLC_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cblLLC.SelectedIndexChanged
        BindBuilders()

        ScriptManager.RegisterStartupScript(Me, Me.GetType, "InitMega", "initMegaSelect();", True)
        upBuilders.Update()
    End Sub
End Class
