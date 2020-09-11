Option Strict Off

Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer
Imports Components
Imports MasterPages

Public Class PageEdit
    Inherits AdminPage

    Protected PageId As Integer
    Protected Regions As ContentToolRegionCollection
    Protected TemplateId As Integer
    Protected dbPage As ContentToolPageRow
    Protected dbTemplate As ContentToolTemplateRow
    Protected txtTitle As TextBox
    Protected txtName As TextBox
    Protected txtCustomURL As TextBox
    Protected txtMetaKeywords As TextBox
    Protected txtMetaDescription As TextBox
    Protected chkIsContentBefore As CheckBox
    Protected chkIsIndexed As CheckBox
    Protected chkIsFollowed As CheckBox
    Protected chkSkipIndexing As CheckBox
    Protected chkIsPermanent As CheckBox
    Protected drpNavigationId As DropDownList
    Protected drpSubNavigationId As DropDownList
    Protected ltlErrorMsg As Literal

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        txtTitle = New TextBox
        txtTitle.ID = "txtTitle"
        txtTitle.Width = 640

        txtName = New TextBox
        txtName.ID = "txtName"
        txtName.Width = 500

        txtCustomURL = New TextBox
        txtCustomURL.ID = "txtCustomURL"
        txtCustomURL.Width = 500

        ltlErrorMsg = New Literal

        txtMetaKeywords = New TextBox
        txtMetaKeywords.ID = "txtMetaKeywords"
        txtMetaKeywords.Width = New Unit(640)
        txtMetaKeywords.TextMode = TextBoxMode.MultiLine
        txtMetaKeywords.Rows = 5

        txtMetaDescription = New TextBox
        txtMetaDescription.ID = "txtMetaDescription"
        txtMetaDescription.Width = New Unit(640)
        txtMetaDescription.TextMode = TextBoxMode.MultiLine
        txtMetaDescription.Rows = 3

        chkIsContentBefore = New CheckBox
        chkIsContentBefore.ID = "chkContentBefore"

        chkIsIndexed = New CheckBox
        chkIsIndexed.ID = "chkIsIndexed"

        chkIsFollowed = New CheckBox
        chkIsFollowed.ID = "chkIsFollowed"

        chkSkipIndexing = New CheckBox
        chkSkipIndexing.ID = "chkSkipIndexing"

        chkIsPermanent = New CheckBox
        chkIsPermanent.ID = "chkIsPermanent"

        drpNavigationId = New DropDownList
        drpNavigationId.DataSource = ContentToolNavigationRow.GetMainMenu(DB)
        drpNavigationId.DataTextField = "Title"
        drpNavigationId.DataValueField = "NavigationId"
        drpNavigationId.AutoPostBack = True
        drpNavigationId.DataBind()

        drpSubNavigationId = New DropDownList
        drpSubNavigationId.DataTextField = "Title"
        drpSubNavigationId.DataValueField = "NavigationId"
    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("CONTENT_TOOL")

        PageId = Request("PageId")

        'If PageId is blank, then we need to autogenerate Page 
        'and redirect to this page with newly created PageId
        If PageId = Nothing Then
            EnsureRegions()
        End If

        LoadRegions()
        LoadUI(True)

        'Register hidden field to pass CommandArgument data from popup windows
        ClientScript.RegisterHiddenField("__CommandArgument", "")
        ClientScript.RegisterHiddenField("__CommandArgs", "")
    End Sub

    Private Sub EnsureRegions()
        Dim PageId As Integer = Nothing
        Dim TemplateId As Integer = Nothing
        Dim SectionId As Integer = Nothing
        Dim PageURL As String = String.Empty

        TemplateId = Request("TemplateId")
        SectionId = Request("SectionId")
        PageURL = Request("PageURL")

        If Not PageURL = String.Empty Then
            SQL = "select top 1 PageId from ContentToolPage Where PageURL=" & DB.Quote(PageURL)
        ElseIf Not SectionId = Nothing Then
            SQL = "select top 1 PageId from ContentToolPage Where TemplateId=" & TemplateId & " and SectionId = " & SectionId & " and PageURL is null "
        Else
            SQL = "select top 1 PageId from ContentToolPage Where TemplateId = " & TemplateId & " and SectionId is null and PageURL is null"
        End If

        PageId = DB.ExecuteScalar(SQL)
        If Not PageId = Nothing Then
            Response.Redirect("edit.aspx?PageId=" & PageId)
        End If

        'Insert Page and then redirect
        Dim dbPage As New ContentToolPageRow(DB)
        dbPage.PageURL = PageURL
        dbPage.SectionId = SectionId
        dbPage.TemplateId = TemplateId
        PageId = dbPage.AutoInsert()

        Response.Redirect("edit.aspx?PageId=" & PageId)
    End Sub

    Private Sub LoadRegions()
        dbPage = ContentToolPageRow.GetRow(DB, PageId)
        dbTemplate = ContentToolTemplateRow.GetRow(DB, dbPage.TemplateId)

        'All regions and modules are serialized to viewstate
        If Not IsPostBack Then
            Regions = dbPage.GetContentToolRegions()
            ViewState("Regions") = Regions
        Else
            Regions = CType(ViewState("Regions"), ContentToolRegionCollection)
        End If
    End Sub

    Private Sub LoadUI(ByVal FirstLoad As Boolean)
        LoadUI(FirstLoad, False)
    End Sub

    Private Sub LoadUI(ByVal FirstLoad As Boolean, ByVal IsPreview As Boolean)
        'LoadUI is called twice on PostBack, therefore we need to clear all controls first
        Controls.Clear()

        'Load all controls from selected template
        Dim Template As Control = Nothing

        Template = Page.ParseControl(dbTemplate.TemplateHTML)
        Template.ID = Me.ID + "_Template"

        Dim count As Integer = Template.Controls.Count
        Dim index As Integer
        For index = 0 To count - 1
            Dim control As Control = Template.Controls(0)
            Template.Controls.Remove(control)
            If ScriptManager.GetCurrent(Page) Is Nothing AndAlso TypeOf control Is HtmlForm AndAlso control.FindControl("AjaxManager") Is Nothing Then
                Dim sm As New ScriptManager
                sm.ID = "AjaxManager"
                control.Controls.AddAt(0, sm)
            End If
            If control.Visible Then Controls.Add(control)
        Next
        Controls.AddAt(0, Template)

        'Replace regions with modules
        IterateThroughChildren(Me, FirstLoad, IsPreview)
    End Sub

    Public Sub IterateThroughChildren(ByVal Parent As Control, ByVal FirstLoad As Boolean, ByVal IsPreview As Boolean)
        ' Dim AllowEdit As Boolean

        For Each c As Control In Parent.Controls

            If TypeOf c Is ContentRegion Then

                Dim r As ContentRegion = c
                c.Controls.Clear()
                'Allow Modules Edit
                '    AllowEdit = r.AllowEdit OrElse Me.LoggedInIsInternal

                'Retrieve region information
                Dim region As ContentToolRegion = Regions.FindByContentRegion(r.ID)

                'Display region boundaries
                Dim div As New HtmlGenericControl("div")
                div.ID = "div" & c.ClientID
                If Not IsPreview Then
                    div.Attributes("style") = "width:" & r.Width.ToString
                    div.Attributes("class") = "content"
                End If
                c.Controls.Add(div)

                'Display region name
                Dim divRegion As New HtmlGenericControl("div")
                divRegion.ID = "divRegion" & c.ClientID
                divRegion.Attributes("class") = "contentregion"
                divRegion.Controls.Add(New LiteralControl("<table border=0 cellpadding=0 cellspacing=0><tr><td nowrap>" & region.Name & "</td><td  width=100% align=right>"))
                divRegion.Visible = Not IsPreview
                div.Controls.Add(divRegion)


                If region.RegionType = "Custom" Then
                    'Create add button
                    Dim NofColumns As Integer = CInt(650 / r.Width.Value - 0.49999999)
                    Dim btnAdd As ImageButton = CreateImageButton("/images/admin/Create.gif", "btnAdd" & r.ClientID)
                    btnAdd.AlternateText = "Insert Programming Module"
                    btnAdd.CommandName = r.ClientID
                    btnAdd.OnClientClick = "window.open('InsertModule.aspx?ClientId=" & btnAdd.ClientID & "&NofColumns=" & NofColumns & "&Width=" & r.Width.Value & "', 'contentinsert', 'TOP=50,LEFT=50,WIDTH=800,HEIGHT=630,RESIZABLE=yes,SCROLLBARS=yes,STATUS=0'); return false;"
                    AddHandler btnAdd.Click, AddressOf btnAdd_Click
                    divRegion.Controls.Add(btnAdd)

                    'Create edit button
                    Dim btnEdit As ImageButton = CreateImageButton("/images/admin/Edit.gif", "btnEdit" & r.ClientID)
                    btnEdit.AlternateText = "Insert Text Module"
                    btnEdit.CommandName = r.ClientID
                    btnEdit.OnClientClick = "window.open('AddModule.aspx?ClientId=" & btnEdit.ClientID & "', 'contentadd', 'TOP=50,LEFT=50,WIDTH=800,HEIGHT=630,RESIZABLE=yes,SCROLLBARS=yes,STATUS=0'); return false;"
                    AddHandler btnEdit.Click, AddressOf btnEdit_Click
                    divRegion.Controls.Add(btnEdit)
                End If

                divRegion.Controls.Add(New LiteralControl("</td></tr></table>"))

                'Display Region Header
                Dim divHeader As New HtmlGenericControl("div")
                divHeader.Attributes("class") = "contentheader"
                divHeader.ID = "divHeader" & r.ClientID
                divHeader.Visible = Not IsPreview
                div.Controls.Add(divHeader)


                'Create dropdown list with Deafaut/Custom options
                Dim drp As New DropDownList
                drp.Items.Add("Default")
                drp.Items.Add("Custom")
                drp.SelectedValue = region.RegionType
                drp.AutoPostBack = True
                drp.ID = "drp" & r.ClientID
                divHeader.Controls.Add(drp)
                AddHandler drp.SelectedIndexChanged, AddressOf drp_SelectedIndexChanged


                Dim counter As Integer = 0
                For Each m As ContentToolRegionModule In region.Modules
                    'Display Module Edit header
                    Dim divEdit As HtmlGenericControl = Nothing

                    'if custoom module, then allow to delete and move modules
                    If region.RegionType = "Custom" Then
                        'Display Module Edit header
                        divEdit = New HtmlGenericControl("div")
                        divEdit.Attributes("class") = "contentedit"
                        divEdit.ID = "divEdit" & r.ClientID & "_" & m.ModuleId & "_" & m.SortOrder
                        divEdit.Visible = Not IsPreview
                        div.Controls.Add(divEdit)
                    End If

                    Dim ctrl As Control = Page.LoadControl(m.ControlURL)
                    div.Controls.Add(ctrl)
                    If TypeOf ctrl Is System.Web.UI.PartialCachingControl Then
                        Dim cached As Control = CType(ctrl, PartialCachingControl).CachedControl
                        If Not cached Is Nothing Then
                            ctrl = cached
                        End If
                    End If
                    ctrl.ID = "m" & r.ClientID & "_" & m.ModuleId & "_" & m.SortOrder
                    If (TypeOf ctrl Is IModule) Then
                        CType(ctrl, IModule).Args = m.Args
                        If Not m.HTML = String.Empty Then
                            CType(ctrl, IModule).HTMLContent = m.HTML
                        End If
                    End If

                    'if custoom module, then allow to delete and move modules
                    If region.RegionType = "Custom" Then

                        'Create move up button
                        If counter > 0 Then
                            Dim btnMoveUp As ImageButton = CreateImageButton("/images/admin/MoveUp.gif", "btnMoveUp" & r.ClientID & "_" & m.ModuleId & "_" & m.SortOrder)
                            btnMoveUp.CommandArgument = counter
                            btnMoveUp.CommandName = region.ContentRegion
                            AddHandler btnMoveUp.Click, AddressOf btnMoveUp_Click
                            divEdit.Controls.Add(btnMoveUp)
                        End If

                        'Create move down button
                        If counter < region.Modules.Count - 1 Then
                            Dim btnMoveDown As ImageButton = CreateImageButton("/images/admin/MoveDown.gif", "btnMoveDown" & r.ClientID & "_" & m.ModuleId & "_" & m.SortOrder)
                            btnMoveDown.CommandArgument = counter
                            btnMoveDown.CommandName = region.ContentRegion
                            AddHandler btnMoveDown.Click, AddressOf btnMoveDown_Click
                            divEdit.Controls.Add(btnMoveDown)
                        End If

                        'Create edit button
                        If TypeOf ctrl Is IModule Then
                            If CType(ctrl, IModule).EditMode Then
                                Dim btnUpdate As ImageButton = CreateImageButton("/images/admin/Edit.gif", "btnUpdate" & r.ClientID & "_" & m.ModuleId & "_" & m.SortOrder)
                                btnUpdate.CommandName = r.ClientID
                                btnUpdate.CommandArgument = counter
                                btnUpdate.OnClientClick = "window.open('AddModule.aspx?ClientId=" & btnUpdate.ClientID & "&ContentId=" & CType(ctrl, IModule).Args & "', 'contentedit', 'TOP=50,LEFT=50,WIDTH=800,HEIGHT=630,RESIZABLE=no,SCROLLBARS=yes,STATUS=0'); return false;"
                                AddHandler btnUpdate.Click, AddressOf btnUpdate_Click
                                divEdit.Controls.Add(btnUpdate)
                            End If
                        End If

                        'Create delete button
                        Dim btnDelete As ImageButton = CreateImageButton("/images/admin/Delete.gif", "btnDelete" & r.ClientID & "_" & m.ModuleId & "_" & m.SortOrder)
                        btnDelete.CommandArgument = counter
                        btnDelete.CommandName = region.ContentRegion
                        AddHandler btnDelete.Click, AddressOf btnDelete_Click
                        divEdit.Controls.Add(btnDelete)

                        Dim ltlModuleName As New Literal
                        ltlModuleName.Text = "<span class=""smaller""> " & m.Name & IIf(m.Args = String.Empty, "", " (" & m.Args & ")") & "</span>"
                        divEdit.Controls.Add(ltlModuleName)
                        divEdit.Visible = Not IsPreview
                    End If
                    counter += 1
                Next

            ElseIf TypeOf c Is NavigationRegion Then

                Dim r As NavigationRegion = c
                c.Controls.Clear()

                r.Controls.Add(New LiteralControl("<p></p>"))

                'Create Title text box, but for Pages only
                If Not dbPage.PageURL = String.Empty Then
                    If Not IsPostBack Then
                        txtName.Text = dbPage.Name
                        txtCustomURL.Text = dbPage.CustomURL
                        txtTitle.Text = dbPage.Title
                        chkIsContentBefore.Checked = dbPage.IsContentBefore
                        chkIsFollowed.Checked = dbPage.IsFollowed
                        chkSkipIndexing.Checked = dbPage.SkipIndexing
                        chkIsPermanent.Checked = dbPage.IsPermanent
                        chkIsIndexed.Checked = dbPage.IsIndexed
                        txtMetaKeywords.Text = dbPage.MetaKeywords
                        txtMetaDescription.Text = dbPage.MetaDescription
                        drpNavigationId.SelectedValue = dbPage.NavigationId
                        drpSubNavigationId.SelectedValue = dbPage.SubNavigationId
                    End If

                    Dim plc As New PlaceHolder

                    r.Controls.Add(ltlErrorMsg)
                    r.Controls.Add(New LiteralControl("<table border=0><tr><td valign=top class=contentregion>Page Name:</td><td valign=top class=contentbottom>"))
                    r.Controls.Add(txtName)
                    r.Controls.Add(New LiteralControl("</td></tr><tr><td valign=top class=contentregion>Custom URL</td><td valign=top class=contentbottom style='text-align:left;'>"))
                    r.Controls.Add(txtCustomURL)
                    r.Controls.Add(New LiteralControl("<br /><span class=""smaller"">Please begin url with ""/"" i.e. ""/sample-url.aspx""</span>"))
                    r.Controls.Add(New LiteralControl("</td></tr><tr><td valign=top class=contentregion>Navigation Section</td><td valign=top class=contentbottom style='text-align:left;'>"))
                    r.Controls.Add(drpNavigationId)
                    r.Controls.Add(New LiteralControl("</td></tr><tr><td valign=top class=contentregion>Navigation Sub-Section</td><td valign=top class=contentbottom style='text-align:left;'>"))
                    r.Controls.Add(drpSubNavigationId)
                    r.Controls.Add(New LiteralControl("</td></tr><tr><td valign=top class=contentregion>Display Page Content<br />before generated content</td><td valign=top class=contentbottom style='text-align:left;'>"))
                    r.Controls.Add(chkIsContentBefore)
                    r.Controls.Add(New LiteralControl("&nbsp;Yes</td></tr>"))
                    If LoggedInIsInternal Then
                        r.Controls.Add(New LiteralControl("<tr><td valign=top class=contentregion>Hide delete icon? (AE)</td><td valign=top class=contentbottom style='text-align:left;'>"))
                        r.Controls.Add(chkIsPermanent)
                        r.Controls.Add(New LiteralControl("&nbsp;Yes</td></tr>"))
                    End If
                    r.Controls.Add(New LiteralControl("<tr><td valign=top colspan=2 class=contentregion align=center><strong>Search Engine Optimization</strong></td></tr>"))
                    r.Controls.Add(New LiteralControl("<tr><td valign=top class=contentbottom colspan=2 align=center>"))
                    r.Controls.Add(Me.chkIsIndexed)
                    r.Controls.Add(New LiteralControl("Index this page "))
                    r.Controls.Add(Me.chkIsFollowed)
                    r.Controls.Add(New LiteralControl("Follow links on this page"))
                    If SysParam.GetValue(DB, "IdevSearchEnabled") Then
                        r.Controls.Add(Me.chkSkipIndexing)
                        r.Controls.Add(New LiteralControl("Skip idev&reg; search indexing"))
                    End If
                    r.Controls.Add(New LiteralControl("</td></tr><tr><td valign=top class=contentregion colspan=2>Page Title</td></tr><tr><td valign=top align=left class=contentbottom colspan=2>"))
                    r.Controls.Add(txtTitle)
                    r.Controls.Add(New LiteralControl("<tr><td valign=top class=contentregion colspan=2>Keywords</td></tr><tr><td valign=top align=left class=contentbottom colspan=2>"))
                    r.Controls.Add(Me.txtMetaKeywords)
                    r.Controls.Add(New LiteralControl("</td></tr><tr><td valign=top class=contentregion colspan=2>Description</td></tr><tr><td valign=top class=contentbottom colspan=2 align=left>"))
                    r.Controls.Add(Me.txtMetaDescription)
                    r.Controls.Add(New LiteralControl("</td></tr></table><p></p>"))

                    'Retrieve SiteSectionId value manually
                    If FirstLoad Then
                        Call RefreshValueOnPostback(drpNavigationId)
                        drpSubNavigationId.DataSource = ContentToolNavigationRow.GetSubNavigation(DB, drpNavigationId.SelectedValue)
                        On Error Resume Next
                        drpSubNavigationId.DataBind()
                        On Error GoTo 0
                        Call RefreshValueOnPostback(drpSubNavigationId)
                    End If
                End If

                Dim btnPreview As Button = CreateButton("Preview", "btnPreview")
                AddHandler btnPreview.Click, AddressOf btnPreview_Click
                r.Controls.Add(btnPreview)
                btnPreview.Visible = Not IsPreview

                Dim btnModify As Button = CreateButton("Modify", "btnModify")
                AddHandler btnModify.Click, AddressOf btnModify_Click
                r.Controls.Add(btnModify)
                btnModify.Visible = IsPreview

                r.Controls.Add(New LiteralControl("&nbsp;"))

                Dim btnPublish As Button = CreateButton("Publish", "btnPublish")
                AddHandler btnPublish.Click, AddressOf btnPublish_Click
                r.Controls.Add(btnPublish)

            End If

            If c.Controls.Count > 0 Then
                IterateThroughChildren(c, FirstLoad, IsPreview)
            End If
        Next
    End Sub

    Protected Sub btnEdit_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        Dim btn As ImageButton = CType(sender, ImageButton)
        Dim region As ContentToolRegion = Regions.FindByContentRegion(btn.CommandName)

        'Add new module
        Dim m As New ContentToolRegionModule
        m.SortOrder = region.Modules.Count + 1
        m.ControlURL = "/modules/Content.ascx"
        m.ModuleId = 1
        m.Args = Request.Form("__CommandArgument")
        m.HTML = ""
        region.Modules.Add(m)

        'Refresh Page
        LoadUI(False)
    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        Dim btn As ImageButton = CType(sender, ImageButton)
        Dim region As ContentToolRegion = Regions.FindByContentRegion(btn.CommandName)
        Dim dbModule As ContentToolModuleRow = ContentToolModuleRow.GetRow(DB, Request.Form("__CommandArgument"))
        Dim Args As String = Request.Form("__CommandArgs")

        'Add new module
        Dim m As New ContentToolRegionModule
        m.SortOrder = region.Modules.Count + 1
        m.ControlURL = dbModule.ControlURL
        m.ModuleId = dbModule.ModuleId
        If Not Args = String.Empty Then
            m.Args = Args
        Else
            m.Args = dbModule.Args
        End If
        m.HTML = dbModule.HTML
        m.Name = dbModule.Name
        region.Modules.Add(m)

        'Refresh Page
        LoadUI(False)
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        Dim btn As ImageButton = CType(sender, ImageButton)

        Dim region As ContentToolRegion = Regions.FindByContentRegion(btn.CommandName)
        region.Modules.RemoveAt(btn.CommandArgument)

        'Refresh Page
        LoadUI(False)
    End Sub

    Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        Dim btn As ImageButton = CType(sender, ImageButton)
        Dim region As ContentToolRegion = Regions.FindByContentRegion(btn.CommandName)

        'Update existing module
        Dim m As ContentToolRegionModule = region.Modules(btn.CommandArgument)
        m.Args = Request.Form("__CommandArgument")

        'Refresh Page
        LoadUI(False)
    End Sub

    Protected Sub btnMoveUp_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        Dim btn As ImageButton = CType(sender, ImageButton)

        Dim Counter As Integer = btn.CommandArgument
        Dim region As ContentToolRegion = Regions.FindByContentRegion(btn.CommandName)
        Dim m As ContentToolRegionModule = region.Modules(Counter)

        'Swap modules
        region.Modules(Counter) = region.Modules(Counter - 1)
        region.Modules(Counter - 1) = m

        'Refresh Page
        LoadUI(False)
    End Sub

    Protected Sub btnMoveDown_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        Dim btn As ImageButton = CType(sender, ImageButton)

        Dim Counter As Integer = btn.CommandArgument
        Dim region As ContentToolRegion = Regions.FindByContentRegion(btn.CommandName)
        Dim m As ContentToolRegionModule = region.Modules(Counter)

        'Swap modules
        region.Modules(Counter) = region.Modules(Counter + 1)
        region.Modules(Counter + 1) = m

        'Refresh Page
        LoadUI(False)
    End Sub

    Protected Sub drp_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim drp As DropDownList = CType(sender, DropDownList)

        Dim ContentRegion As String = Replace(drp.ID, "drp", "")
        Dim region As ContentToolRegion = Regions.FindByContentRegion(ContentRegion)
        region.RegionType = drp.SelectedValue

        'if region has been changed to default, then reload modules for this region
        'for page level, defaults should be loaded from section level
        'for section level, defaults should be loaded from template level
        If Not dbPage.PageURL = String.Empty Then
            region.Modules = dbPage.GetDefaultContentToolModules(region, ModuleLevel.Section)
        ElseIf Not dbPage.SectionId = Nothing Then
            region.Modules = dbPage.GetDefaultContentToolModules(region, ModuleLevel.Template)
        Else
            region.Modules = New ContentToolRegionModuleCollection
        End If

        'Refresh Page
        LoadUI(False)
    End Sub

    Function CreateImageButton(ByVal ImageUrl As String, ByVal Id As String) As ImageButton
        Dim btn As New ImageButton

        btn.Height = 16
        btn.Width = 16
        btn.ImageUrl = ImageUrl
        btn.ID = Id

        Return btn
    End Function

    Function CreateButton(ByVal Text As String, ByVal Id As String) As Button
        Dim btn As New Button

        btn.ID = Id
        btn.Text = Text
        btn.CssClass = "adminbtn"

        Return btn
    End Function

    Protected Sub btnPreview_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        'Refresh Page
        LoadUI(False, True)
    End Sub

    Protected Sub btnModify_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        'Refresh Page
        LoadUI(False, False)
    End Sub

    Protected Sub btnPublish_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim SQL As String = ""

        If Not dbPage.PageURL = String.Empty AndAlso drpSubNavigationId.SelectedValue = String.Empty Then
            ltlErrorMsg.Text = "<span style='color: red;'>** Error: You must select a Navigation Sub-Section **</span><br />"
            Exit Sub
        End If

        If Not txtCustomURL.Text = String.Empty AndAlso dbPage.CustomURL <> txtCustomURL.Text Then
            Dim dbCustomURLHistory As CustomURLHistoryRow = CustomURLHistoryRow.GetFromCustomURL(DB, Me.txtCustomURL.Text)
            If dbCustomURLHistory.CustomURLHistoryId > 0 Then
                ltlErrorMsg.Text = "<span style='color: red;'>Custom URL has been used in the past. For SEO purposes, please use a different custom url.**</span><br />"
                Exit Sub
            End If
            If Not URLMappingManager.IsValidFolder(txtCustomURL.Text) Then
                ltlErrorMsg.Text = "<span style='color:red;'> Error: Cannot use a URL rewrite which points to a system folder. Please try another folder **</span><br />"
                Exit Sub
            End If
            If Not URLMappingManager.IsValidURL(DB, txtCustomURL.Text) Then
				ltlErrorMsg.Text = "<span style='color: red;'>The requested Custom URL is already used. Please provide different URL**</span><br />"
                Exit Sub
            End If
        End If
        

        DB.BeginTransaction()

        'Delete all region modules
        SQL = "delete from ContentToolRegionModule where PageRegionId in (select PageRegionId from ContentToolPageRegion where PageId = " & PageId & ")"
        DB.ExecuteSQL(SQL)

        'Delete all page regions
        SQL = "delete from ContentToolPageRegion where PageId = " & PageId
        DB.ExecuteSQL(SQL)

        'Loop through all regions and modules and re-insert
        For Each region As ContentToolRegion In Regions
            Dim dbRegion As New ContentToolPageRegionRow(DB)
            dbRegion.ContentRegion = region.ContentRegion
            dbRegion.PageId = PageId
            dbRegion.RegionType = region.RegionType
            dbRegion.AutoInsert()

            Dim Counter As Integer = 1
            For Each m As ContentToolRegionModule In region.Modules
                Dim dbModule As New ContentToolRegionModuleRow(DB)
                dbModule.Args = m.Args
                dbModule.ModuleId = m.ModuleId
                dbModule.PageRegionId = dbRegion.PageRegionId
                dbModule.SortOrder = Counter
                dbModule.Insert()

                Counter += 1
            Next
        Next

        'Update page title, if applicable
        If Not dbPage.PageURL = String.Empty Then
            dbPage.Title = txtTitle.Text
            dbPage.Name = txtName.Text
            dbPage.CustomURL = txtCustomURL.Text
            dbPage.IsContentBefore = chkIsContentBefore.Checked
            dbPage.IsFollowed = Me.chkIsFollowed.Checked
            dbPage.SkipIndexing = Me.chkSkipIndexing.Checked
            dbPage.IsPermanent = Me.chkIsPermanent.Checked
            dbPage.IsIndexed = Me.chkIsIndexed.Checked
            dbPage.MetaDescription = Me.txtMetaDescription.Text
            dbPage.MetaKeywords = Me.txtMetaKeywords.Text
            dbPage.NavigationId = Me.drpNavigationId.SelectedValue
            dbPage.SubNavigationId = Me.drpSubNavigationId.SelectedValue
            dbPage.Update()
        End If

        DB.CommitTransaction()

        Page.ClientScript.RegisterStartupScript(Me.GetType, "ConfirmationScript", "document.location.href='confirm.aspx?PageId=" & PageId & "';", True)
        'Response.Redirect("/admin/content/confirm.aspx?PageId=" & PageId)
    End Sub
End Class
