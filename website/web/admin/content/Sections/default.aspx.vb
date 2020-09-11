Imports Components
Imports System.Data

Partial Class index
    Inherits AdminPage

    Protected params As String

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("CONTENT_TOOL")
        If Not IsPostBack Then BindDataGrid()
    End Sub

    Private Sub BindDataGrid()
        params = GetPageParams(FilterFieldType.All)

        If Not IsPostBack Then
            ViewState("F_SortBy") = Replace(Request("F_SortBy"), ";", "")
            ViewState("F_SortOrder") = Replace(Request("F_SortOrder"), ";", "")
            ViewState("F_PG") = Request("F_PG")
        End If
        If ViewState("F_SortBy") Is Nothing Then
            ViewState("F_SortBy") = "SectionName"
        End If
        If ViewState("F_SortOrder") Is Nothing Then
            ViewState("F_SortOrder") = "ASC"
        End If

        ' BUILD QUERY
        Dim sConn As String
        sConn = " WHERE "
        SQL = "select cts.*, ctt.TemplateId, ctt.TemplateName from ContentToolSection cts, ContentToolTemplate ctt"
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

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?SectionId=0&" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub myNavigator_PagingEvent(ByVal sender As Object, ByVal e As Controls.NavigatorEventArgs) Handles myNavigator.NavigatorEvent
        ViewState("F_PG") = e.PageNumber
        BindDataGrid()
    End Sub
End Class

