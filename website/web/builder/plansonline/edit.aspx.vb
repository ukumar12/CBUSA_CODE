Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class EditProject
    Inherits SitePage

    Protected ProjectId As Integer
    Protected Redir As String = String.Empty
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private CurrentProjectId As String = ""
    Private NewProjectId As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureBuilderAccess()

        If Not CType(Me.Page, SitePage).IsLoggedInBuilder Then
            Response.Redirect("/default.aspx")
        End If

        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))

        If Not dbBuilder.IsPlansOnline Then
            Response.Redirect("noaccess.aspx")
        End If

        ProjectId = Convert.ToInt32(Request("ProjectId"))

        If Request("QuoteRequestId") <> String.Empty Then
            Redir = "quoterequestmessages.aspx?QuoteRequestId=" & Request("QuoteRequestId")
        End If

        If Not IsPostBack Then
            LoadFromDB()
        End If
        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")
    End Sub

    Private Sub LoadFromDB()
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

        If ProjectId = 0 Then
            rblArchive.SelectedValue = False

            Dim dbBuilderAccount As BuilderAccountRow = BuilderAccountRow.GetRow(DB, Session("BuilderAccountId"))
            txtContactName.Text = dbBuilderAccount.FirstName & " " & dbBuilderAccount.LastName
            txtContactPhone.Text = dbBuilderAccount.Phone
            txtContactEmail.Text = dbBuilderAccount.Email
            Exit Sub
        End If

        Dim dbProject As ProjectRow = ProjectRow.GetRow(DB, ProjectId)

        If dbProject.BuilderId <> Session("BuilderId") Then
            Response.Redirect("/default.aspx")
        End If

        txtProjectName.Text = dbProject.ProjectName
        txtLotNo.Text = dbProject.LotNumber
        txtSubdivision.Text = dbProject.Subdivision
        txtAddress1.Text = dbProject.Address
        txtAddress2.Text = dbProject.Address2
        txtCity.Text = dbProject.City
        drpState.SelectedValue = dbProject.State
        txtZip.Text = dbProject.Zip
        drpPortfolio.SelectedValue = dbProject.PortfolioID
        drpStatus.SelectedValue = dbProject.ProjectStatusID
        dpStartDate.Value = dbProject.StartDate
        rblArchive.SelectedValue = dbProject.IsArchived
        
        drpState.SelectedValue = dbProject.State

        txtContactName.Text = dbProject.ContactName
        txtContactEmail.Text = dbProject.ContactEmail
        txtContactPhone.Text = dbProject.ContactPhone

    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbProject As ProjectRow

            If ProjectId <> 0 Then
                dbProject = ProjectRow.GetRow(DB, ProjectId)
            Else
                dbProject = New ProjectRow(DB)
            End If

            With dbProject
                .BuilderID = Session("BuilderID")
                .ProjectName = txtProjectName.Text
                .LotNumber = txtLotNo.Text
                .Subdivision = txtSubdivision.Text
                .Address = txtAddress1.Text
                .Address2 = txtAddress2.Text
                .City = txtCity.Text
                .State = drpState.SelectedValue
                .Zip = txtZip.Text
                If drpPortfolio.SelectedValue <> Nothing Then
                    .PortfolioID = drpPortfolio.SelectedValue
                End If
                If drpStatus.SelectedValue <> Nothing Then
                    .ProjectStatusID = drpStatus.SelectedValue
                End If
                .StartDate = dpStartDate.Value
                .IsArchived = rblArchive.SelectedValue
                .ContactName = txtContactName.Text
                .ContactEmail = txtContactEmail.Text
                .ContactPhone = txtContactPhone.Text
            End With


            If ProjectId <> 0 Then
                dbProject.Update()
                'log  Update Project
                CurrentProjectId = dbProject.ProjectID
                Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Update Project", CurrentProjectId, "", "", "", UserName)
                'end log
            Else
                ProjectId = dbProject.Insert
                'log Create Add Project
                NewProjectId = dbProject.ProjectID
                Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Save New Project", NewProjectId, "", "", "", UserName)
                'end log
            End If

            DB.CommitTransaction()

            If Redir <> String.Empty Then
                Response.Redirect(Redir)
            Else
                Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
            End If


        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        If Redir <> String.Empty Then
            Response.Redirect(Redir)
        Else
            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
        End If
    End Sub


End Class

