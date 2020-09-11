Imports Components
Imports System.Data

Partial Class index
    Inherits AdminPage

    Protected params As String

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("CONTENT_TOOL")

        If Not IsPostBack Then
            InitializeFilterFields()
            BindDataGrid()
        End If
    End Sub

    Private Sub InitializeFilterFields()
        Dim SQL As String = ""

        If IsPostBack Then
            Exit Sub
        End If

        'Bind templates
        SQL = "SELECT TemplateId, TemplateName FROM ContentToolTemplate order by TemplateName"
        Dim dtTemplates As DataTable = DB.GetDataTable(SQL)
        F_TemplateId.DataValueField = "TemplateId"
        F_TemplateId.DataTextField = "TemplateName"
        F_TemplateId.DataSource = dtTemplates
        F_TemplateId.DataBind()
        F_TemplateId.Items.Insert(0, New ListItem("-- ALL --", ""))
        F_TemplateId.SelectedValue = Request("F_TemplateId")

        'Bind sections
        SQL = "SELECT SectionId, SectionName + ' (' + Folder + ')' AS Section FROM ContentToolSection order by SectionName"
        Dim dtSections As DataTable = DB.GetDataTable(SQL)
        F_SectionId.DataValueField = "SectionId"
        F_SectionId.DataTextField = "Section"
        F_SectionId.DataSource = dtSections
        F_SectionId.DataBind()
        F_SectionId.Items.Insert(0, New ListItem("-- ALL --", ""))
        F_SectionId.SelectedValue = Request("F_SectionId")

        F_Name.Text = Request("F_Name")
        F_PageURL.Text = Request("F_PageURL")

    End Sub

    Private Sub BindDataGrid()
        params = GetPageParams(FilterFieldType.All)

        If Not IsPostBack Then
            ViewState("F_SortBy") = Replace(Request("F_SortBy"), ";", "")
            ViewState("F_SortOrder") = Replace(Request("F_SortOrder"), ";", "")
            ViewState("F_PG") = Request("F_PG")
        End If
        If ViewState("F_SortBy") Is Nothing Then
            ViewState("F_SortBy") = "Name"
        End If
        If ViewState("F_SortOrder") Is Nothing Then
            ViewState("F_SortOrder") = "ASC"
        End If

        ' BUILD QUERY
        Dim sConn As String
        sConn = " and "

        SQL = "select ctp.*, cts.SectionName, ctt.TemplateName from ContentToolPage ctp, ContentToolSection cts, ContentToolTemplate ctt where ctp.PageURL is not null and ctp.TemplateId = ctt.TemplateId and ctp.SectionId = cts.SectionId"
        If Not DB.IsEmpty(F_SectionId.SelectedValue) Then
            SQL = SQL & sConn & "ctp.SectionId = " & DB.Quote(F_SectionId.SelectedValue)
        End If
        If Not DB.IsEmpty(F_TemplateId.SelectedValue) Then
            SQL = SQL & sConn & "ctp.TemplateId = " & DB.Quote(F_TemplateId.SelectedValue)
        End If
        If Not DB.IsEmpty(F_PageURL.Text) Then
            SQL = SQL & sConn & "ctp.PageURL LIKE " & DB.FilterQuote(F_PageURL.Text)
        End If
        If Not DB.IsEmpty(F_Name.Text) Then
            SQL = SQL & sConn & "ctp.Name LIKE " & DB.FilterQuote(F_Name.Text)
        End If
        SQL = SQL & " ORDER BY " & CStr(ViewState("F_SortBy")) & " " & CStr(ViewState("F_SortOrder"))

        Dim res As DataTable = DB.GetDataTable(SQL)

        myNavigator.NofRecords = res.Rows.Count
        myNavigator.MaxPerPage = dgList.PageSize
        myNavigator.PageNumber = Math.Max(Math.Min(CType(ViewState("F_PG"), Integer), myNavigator.NofPages), 1)
        myNavigator.DataBind()

        ViewState("F_PG") = myNavigator.PageNumber
        Me.dgList.Visible = (myNavigator.NofRecords <> 0)
        Me.myNavigator.Visible = (myNavigator.NofRecords <> 0)
        plcNoRecords.Visible = (myNavigator.NofRecords = 0)

        dgList.DataSource = res.DefaultView
        dgList.CurrentPageIndex = CInt(ViewState("F_PG")) - 1
        dgList.DataBind()
    End Sub

    Private Sub btnRegister_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRegister.Click
        Response.Redirect("register.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub btnGenerate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGenerate.Click
        Response.Redirect("generate.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        ViewState("F_PG") = 1
        BindDataGrid()
    End Sub

    Private Sub myNavigator_PagingEvent(ByVal sender As Object, ByVal e As Controls.NavigatorEventArgs) Handles myNavigator.NavigatorEvent
        ViewState("F_PG") = e.PageNumber
        BindDataGrid()
    End Sub

End Class

