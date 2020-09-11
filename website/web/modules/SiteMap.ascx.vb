Option Strict Off

Imports Components
Imports DataLayer

Partial Class SiteMapCtrl
    Inherits ModuleControl

    Private LastSection As String = String.Empty
    Private Count As Integer = 0
    Private Counter As Integer = 0

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        Dim SQL As String = String.Empty

        SQL &= " select "
        SQL &= " 	case when ctn.IsInternalLink = 1 then (select top 1 PageURl from ContentToolPage where PageId = ctn.PageId) else ctn.URL end as SubSectionURL, p.SkipSitemap As ParentSkipSitemap, ctn.SkipSitemap, ctn.Title as SubSectionName,"
        SQL &= " 	case when p.IsInternalLink = 1 then (select top 1 PageURl from ContentToolPage where PageId = p.PageId) else p.URL end as SectionURL, p.Title as SectionName"
        SQL &= " from "
        SQL &= " 	ContentToolNavigation ctn, ContentToolNavigation p"
        SQL &= " where"
        SQL &= " 	    ctn.ParentId = p.NavigationId"
        SQL &= " order by p.SortOrder, ctn.SortOrder"

        Dim dt As DataTable = DB.GetDataTable(SQL)

        'Count the visible elements
        For Each row As DataRow In dt.Rows
            If row("ParentSkipSitemap") Then
                Continue For
            End If
            If row("SkipSitemap") Then
                Continue For
            End If
            Count += 1
        Next
        rptSiteMap.DataSource = dt
        rptSiteMap.DataBind()
    End Sub

    Protected Sub rptSiteMap_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptSiteMap.ItemDataBound
        If Not e.Item.ItemType = ListItemType.Item And Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If

        If e.Item.DataItem("ParentSkipSitemap") Then
            Exit Sub
        End If
        If Not e.Item.DataItem("SkipSitemap") Then
            Counter += 1
        End If

        If e.Item.DataItem("SectionName") <> LastSection And Counter > CInt(Count / 2) Then
            e.Item.Controls.AddAt(0, New LiteralControl("</td><td valign=""top"">"))
            Counter = 0
        End If
        If e.Item.DataItem("SectionName") <> LastSection Then
            Dim divSection As HtmlGenericControl = e.Item.FindControl("divSection")
            divSection.Visible = True
        End If
        If Not e.Item.DataItem("SkipSitemap") Then
            Dim divSubSection As HtmlGenericControl = e.Item.FindControl("divSubSection")
            divSubSection.Visible = True
        End If
        LastSection = e.Item.DataItem("SectionName")
    End Sub
End Class
