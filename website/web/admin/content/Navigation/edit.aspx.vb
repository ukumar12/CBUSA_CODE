Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected NavigationId As Integer
    Protected ParentId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("CONTENT_TOOL")

        NavigationId = Convert.ToInt32(Request("NavigationId"))
        ParentId = Convert.ToInt32(Request("ParentId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        drpPageId.DataSource = ContentToolPageRow.GetPageList(DB)
        drpPageId.DataGroupField = "SectionName"
        drpPageId.DataTextField = "name"
        drpPageId.DataValueField = "PageId"
        drpPageId.DataBind()

        If NavigationId = 0 Then
            chkIsInternalLink.Checked = True
            RefreshPage()
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbContentToolNavigation As ContentToolNavigationRow = ContentToolNavigationRow.GetRow(DB, NavigationId)
        txtTitle.Text = dbContentToolNavigation.Title
        drpPageId.Text = dbContentToolNavigation.PageId
        txtURL.Text = dbContentToolNavigation.URL
        drpTarget.SelectedValue = dbContentToolNavigation.Target
        txtParameters.Text = dbContentToolNavigation.Parameters
        chkIsInternalLink.Checked = dbContentToolNavigation.IsInternalLink
        chkSkipSitemap.Checked = dbContentToolNavigation.SkipSiteMap
        chkSkipBreadcrumb.Checked = dbContentToolNavigation.SkipBreadcrumb

        RefreshPage()
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbContentToolNavigation As ContentToolNavigationRow

            If NavigationId <> 0 Then
                dbContentToolNavigation = ContentToolNavigationRow.GetRow(DB, NavigationId)
            Else
                dbContentToolNavigation = New ContentToolNavigationRow(DB)
            End If
            dbContentToolNavigation.ParentId = ParentId
            dbContentToolNavigation.Title = txtTitle.Text
            dbContentToolNavigation.PageId = drpPageId.Text
            dbContentToolNavigation.URL = txtURL.Text
            dbContentToolNavigation.Target = drpTarget.SelectedValue
            dbContentToolNavigation.Parameters = txtParameters.Text
            dbContentToolNavigation.IsInternalLink = chkIsInternalLink.Checked
            dbContentToolNavigation.SkipSiteMap = chkSkipSitemap.Checked
            dbContentToolNavigation.SkipBreadcrumb = chkSkipBreadcrumb.Checked

            If NavigationId <> 0 Then
                dbContentToolNavigation.Update()
            Else
                NavigationId = dbContentToolNavigation.Insert
            End If

            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
			AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?NavigationId=" & NavigationId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub chkIsInternalLink_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkIsInternalLink.CheckedChanged
        RefreshPage()
    End Sub

    Private Sub RefreshPage()
        tblInternal.Visible = chkIsInternalLink.Checked
        tblExternal.Visible = Not tblInternal.Visible
    End Sub
End Class

