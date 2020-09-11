Imports Components
Imports System.Data

Partial Class index
    Inherits AdminPage

    Protected params As String

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("CONTENT_TOOL")
        BindDataGrid()
    End Sub

    Private Sub BindDataGrid()
        params = GetPageParams(FilterFieldType.All)

        If Not IsPostBack Then
            ViewState("F_SortBy") = Replace(Request("F_SortBy"), ";", "")
            ViewState("F_SortOrder") = Replace(Request("F_SortOrder"), ";", "")
            ViewState("F_PG") = Request("F_PG")
        End If
        If ViewState("F_SortBy") Is Nothing Then
            ViewState("F_SortBy") = "TemplateName"
        End If
        If ViewState("F_SortOrder") Is Nothing Then
            ViewState("F_SortOrder") = "ASC"
        End If

        SQL = " select *, (select PageId from ContentToolPage where TemplateId = ctt.TemplateId and SectionId is null and PageURL is null) as PageId, (select count(*) from ContentToolTemplateRegion where TemplateId = ctt.TemplateId) as NofSlots from ContentToolTemplate ctt "
        SQL = SQL & " ORDER BY " & CStr(ViewState("F_SortBy")) & " " & CStr(ViewState("F_SortOrder"))

        Dim res As DataTable = DB.GetDataTable(SQL)

        myNavigator.NofRecords = res.Rows.Count
        myNavigator.MaxPerPage = dgList.PageSize
        myNavigator.PageNumber = Math.Max(Math.Min(CType(ViewState("F_PG"), Integer), myNavigator.NofPages), 1)
        myNavigator.DataBind()

        ViewState("F_PG") = myNavigator.PageNumber
        tblList.Visible = (myNavigator.NofRecords <> 0)
        plcNoRecords.Visible = (myNavigator.NofRecords = 0)

        dgList.DataSource = res.DefaultView
        dgList.CurrentPageIndex = CInt(ViewState("F_PG")) - 1
        dgList.DataBind()
    End Sub

    Private Sub myNavigator_PagingEvent(ByVal sender As Object, ByVal e As Controls.NavigatorEventArgs) Handles myNavigator.NavigatorEvent
        ViewState("F_PG") = e.PageNumber
        BindDataGrid()
    End Sub

    Protected Sub dgList_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgList.ItemDataBound
        If Not (e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem) Then
            Exit Sub
        End If

        If Not LoggedInIsInternal Then
            Dim Confirmlink1 As HyperLink = e.Item.FindControl("Confirmlink1")
            If Not Confirmlink1 Is Nothing Then
                Confirmlink1.Visible = False
            End If
        End If

    End Sub
End Class
    
