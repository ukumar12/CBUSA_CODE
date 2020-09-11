Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected ProjectID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("PROJECTS")

        ProjectID = Convert.ToInt32(Request("ProjectID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        drpBuilderID.Datasource = BuilderRow.GetList(DB, "CompanyName")
        drpBuilderID.DataValueField = "BuilderID"
        drpBuilderID.DataTextField = "CompanyName"
        drpBuilderID.Databind()
        drpBuilderID.Items.Insert(0, New ListItem("", ""))

        drpState.DataSource = StateRow.GetStateList(DB)
        drpState.DataValueField = "StateCode"
        drpState.DataTextField = "StateName"
        drpState.Databind()
        drpState.Items.Insert(0, New ListItem("", ""))

        drpPortfolioID.Datasource = PortfolioRow.GetList(DB, "Portfolio")
        drpPortfolioID.DataValueField = "PortfolioID"
        drpPortfolioID.DataTextField = "Portfolio"
        drpPortfolioID.Databind()
        drpPortfolioID.Items.Insert(0, New ListItem("", ""))

        drpProjectStatusID.Datasource = ProjectStatusRow.GetList(DB, "ProjectStatus")
        drpProjectStatusID.DataValueField = "ProjectStatusID"
        drpProjectStatusID.DataTextField = "ProjectStatus"
        drpProjectStatusID.Databind()
        drpProjectStatusID.Items.Insert(0, New ListItem("", ""))

        If ProjectID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbProject As ProjectRow = ProjectRow.GetRow(DB, ProjectID)
        txtProjectName.Text = dbProject.ProjectName
        txtLotNumber.Text = dbProject.LotNumber
        txtSubdivision.Text = dbProject.Subdivision
        txtAddress.Text = dbProject.Address
        txtAddress2.Text = dbProject.Address2
        txtCity.Text = dbProject.City
        txtCounty.Text = dbProject.County
        ctrlZip.Value = dbProject.Zip
        dtStartDate.Value = dbProject.StartDate
        drpBuilderID.SelectedValue = dbProject.BuilderID
        drpState.SelectedValue = dbProject.State
        drpPortfolioID.SelectedValue = dbProject.PortfolioID
        drpProjectStatusID.SelectedValue = dbProject.ProjectStatusID
        rblIsArchived.SelectedValue = dbProject.IsArchived
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbProject As ProjectRow

            If ProjectID <> 0 Then
                dbProject = ProjectRow.GetRow(DB, ProjectID)
            Else
                dbProject = New ProjectRow(DB)
            End If
            dbProject.ProjectName = txtProjectName.Text
            dbProject.LotNumber = txtLotNumber.Text
            dbProject.Subdivision = txtSubdivision.Text
            dbProject.Address = txtAddress.Text
            dbProject.Address2 = txtAddress2.Text
            dbProject.City = txtCity.Text
            dbProject.County = txtCounty.Text
            dbProject.Zip = ctrlZip.Value
            dbProject.StartDate = dtStartDate.Value
            dbProject.BuilderID = drpBuilderID.SelectedValue
            dbProject.State = drpState.SelectedValue
            dbProject.PortfolioID = drpPortfolioID.SelectedValue
            dbProject.ProjectStatusID = drpProjectStatusID.SelectedValue
            dbProject.IsArchived = rblIsArchived.SelectedValue

            If ProjectID <> 0 Then
                dbProject.Update()
            Else
                ProjectID = dbProject.Insert
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
        Response.Redirect("delete.aspx?ProjectID=" & ProjectID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

