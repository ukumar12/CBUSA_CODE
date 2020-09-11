Imports Components
Imports DataLayer


Partial Class forms_projects_default
    Inherits SitePage

    Private ProjectId As Integer
    Private dbProject As ProjectRow

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ProjectId = Request("ProjectId")
        dbProject = ProjectRow.GetRow(DB, ProjectId)

        If Not IsPostBack Then
            drpState.DataSource = StateRow.GetStateList(DB)
            drpState.DataTextField = "StateName"
            drpState.DataValueField = "StateCode"
            drpState.DataBind()

            drpPortfolio.DataSource = PortfolioRow.GetList(DB, "Portfolio")
            drpPortfolio.DataTextField = "Portfolio"
            drpPortfolio.DataValueField = "PortfolioID"
            drpPortfolio.DataBind()
            drpPortfolio.Items.Insert(0, New ListItem("", ""))

            drpStatus.DataSource = ProjectStatusRow.GetList(DB, "SortOrder")
            drpStatus.DataTextField = "ProjectStatus"
            drpStatus.DataValueField = "ProjectStatusID"
            drpStatus.DataBind()

            If dbProject.ProjectID <> Nothing Then
                LoadFromDb()
            End If
        End If
    End Sub

    Private Sub LoadFromDb()
        txtProjectName.Text = dbProject.ProjectName
        txtLotNo.Text = dbProject.LotNumber
        txtSubdivision.Text = dbProject.Subdivision
        txtAddress1.Text = dbProject.Address
        txtAddress2.Text = dbProject.Address2
        txtCity.Text = dbProject.City
        drpState.SelectedValue = dbProject.State
        txtZip.Text = dbProject.Zip
        acCounty.Text = dbProject.County
        drpPortfolio.SelectedValue = dbProject.PortfolioID
        drpStatus.SelectedValue = dbProject.ProjectStatusID
        dpStartDate.Value = dbProject.StartDate
        rblArchive.SelectedValue = dbProject.IsArchived
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not Page.IsValid Then
            Exit Sub
        End If

        Try
            If dbProject Is Nothing Then dbProject = ProjectRow.GetRow(DB, ProjectId)
            With dbProject
                .ProjectName = txtProjectName.Text
                .LotNumber = txtLotNo.Text
                .Subdivision = txtSubdivision.Text
                .Address = txtAddress1.Text
                .Address2 = txtAddress2.Text
                .City = txtCity.Text
                .State = drpState.SelectedValue
                .Zip = txtZip.Text
                .County = acCounty.Text
                .PortfolioID = drpPortfolio.SelectedValue
                .ProjectStatusID = drpStatus.SelectedValue
                .StartDate = dpStartDate.Value
                .IsArchived = rblArchive.SelectedValue
            End With

            If dbProject.ProjectID = Nothing Then
                dbProject.Insert()
            Else
                dbProject.Update()
            End If

            Response.Redirect("/projects/default.aspx")
        Catch ex As Exception
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
