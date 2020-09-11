Imports Components
Imports System.Data

Partial Class index
    Inherits AdminPage

    Protected params As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("USERS")

        If Not IsPostBack Then
            BindDataList()
        End If
    End Sub

    Private Sub BindDataList()
        params = GetPageParams(FilterFieldType.All)

        If Not IsPostBack Then
            ViewState("F_SortBy") = Replace(Request("F_SortBy"), ";", "")
            ViewState("F_SortOrder") = Replace(Request("F_SortOrder"), ";", "")
            ViewState("F_PG") = Request("F_PG")
        End If
        If ViewState("F_SortBy") Is Nothing Then
            ViewState("F_SortBy") = "DESCRIPTION"
        End If
        If ViewState("F_SortOrder") Is Nothing Then
            ViewState("F_SortOrder") = "ASC"
        End If

        SQL = " SELECT * FROM AdminGroup"
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

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub dgList_SortCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridSortCommandEventArgs) Handles dgList.SortCommand
        If CStr(ViewState("F_SortOrder")) = "ASC" And CStr(ViewState("F_SortBy")) = e.SortExpression Then
            ViewState("F_SortOrder") = "DESC"
        Else
            ViewState("F_SortOrder") = "ASC"
        End If
        ViewState("F_SortBy") = Replace(e.SortExpression, ";", "")
        ViewState("F_PG") = 1
        BindDataList()
    End Sub

    Private Sub myNavigator_NavigatorEvent(ByVal sender As Object, ByVal e As Controls.NavigatorEventArgs) Handles myNavigator.NavigatorEvent
        ViewState("F_PG") = e.PageNumber
        BindDataList()
    End Sub

End Class
