Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient

Partial Class projects_default
    Inherits SitePage

    Protected dbBuilder As BuilderRow

    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private ProjectID As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureBuilderAccess()

        dbBuilder = BuilderRow.GetRow(DB, Session("BuilderId"))

        gvList.BindList = AddressOf BindList

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")

        If Not IsPostBack Then

        Core.DataLog("Project", PageURL, CurrentUserId, "Builder Top Menu Click", "", "", "", "", UserName)

            F_State.DataSource = StateRow.GetStateList(DB)
            F_State.DataValueField = "StateCode"
            F_State.DataTextField = "StateName"
            F_State.Databind()
            F_State.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_Status.DataSource = ProjectStatusRow.GetList(DB, "SortOrder")
            F_Status.DataTextField = "ProjectStatus"
            F_Status.DataValueField = "ProjectStatusID"
            F_Status.DataBind()

            F_ProjectName.Text = Request("F_ProjectName")
            F_Subdivision.Text = Request("F_Subdivision")
            F_LotNumber.Text = Request("F_LotNumber")
            F_Address1.Text = Request("F_Address1")
            F_City.Text = Request("F_City")
            F_Zip.Text = Request("F_Zip")
            F_ContactName.Text = Request("F_ContactName")
            F_ContactEmail.Text = Request("F_ContactEmail")
            F_ContactPhone.Text = Request("F_ContactPhone")

            F_Status.SelectedValue = Request("F_Status")
            F_State.SelectedValue = Request("F_State")
            F_SubmittedLBound.Text = Request("F_SubmittedLBound")
            F_SubmittedUBound.Text = Request("F_SubmittedUBound")

            If Request("F_IsArchived") <> String.Empty Then
                F_IsArchived.Text = Request("F_IsArchived")
            Else
                F_IsArchived.SelectedValue = "0"
            End If

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            gvList.PageIndex = IIf(Request("F_pg") = String.Empty, 0, Core.ProtectParam(Request("F_pg")))

            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "Submitted"
                gvList.SortOrder = "DESC"
            End If

            BindList()
        End If

    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " And "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder
        ViewState("F_pg") = gvList.PageIndex.ToString

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQLFields &= ", (Select ProjectStatus From ProjectStatus Where ProjectStatusId = Project.ProjectStatusId) As Status "
        SQL = " FROM Project Where BuilderId = " & DB.Number(Session("BuilderId"))

        If Not F_Status.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "ProjectStatusId = " & DB.Quote(F_Status.SelectedValue)
            Conn = " AND "
        End If
        If Not F_State.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "State = " & DB.Quote(F_State.SelectedValue)
            Conn = " AND "
        End If
        If Not F_ProjectName.Text = String.Empty Then
            SQL = SQL & Conn & "ProjectName LIKE " & DB.FilterQuote(F_ProjectName.Text)
            Conn = " AND "
        End If
        If Not F_Subdivision.Text = String.Empty Then
            SQL = SQL & Conn & "Subdivision LIKE " & DB.FilterQuote(F_Subdivision.Text)
            Conn = " AND "
        End If
        If Not F_LotNumber.Text = String.Empty Then
            SQL = SQL & Conn & "LotNumber LIKE " & DB.FilterQuote(F_LotNumber.Text)
            Conn = " AND "
        End If
        If Not F_Address1.Text = String.Empty Then
            SQL = SQL & Conn & "Address LIKE " & DB.FilterQuote(F_Address1.Text)
            Conn = " AND "
        End If
        If Not F_City.Text = String.Empty Then
            SQL = SQL & Conn & "City LIKE " & DB.FilterQuote(F_City.Text)
            Conn = " AND "
        End If
        If Not F_Zip.Text = String.Empty Then
            SQL = SQL & Conn & "Zip LIKE " & DB.FilterQuote(F_Zip.Text)
            Conn = " AND "
        End If
        If Not F_ContactName.Text = String.Empty Then
            SQL = SQL & Conn & "ContactName LIKE " & DB.FilterQuote(F_ContactName.Text)
            Conn = " AND "
        End If
        If Not F_ContactEmail.Text = String.Empty Then
            SQL = SQL & Conn & "ContactEmail LIKE " & DB.FilterQuote(F_ContactEmail.Text)
            Conn = " AND "
        End If
        If Not F_ContactPhone.Text = String.Empty Then
            SQL = SQL & Conn & "ContactPhone LIKE " & DB.FilterQuote(F_ContactPhone.Text)
            Conn = " AND "
        End If
        If Not F_IsArchived.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "IsArchived  = " & DB.Number(F_IsArchived.SelectedValue)
            Conn = " AND "
        End If
        If Not F_SubmittedLBound.Text = String.Empty Then
            SQL = SQL & Conn & "Submitted >= " & DB.Quote(F_SubmittedLbound.Text)
            Conn = " AND "
        End If
        If Not F_SubmittedUBound.Text = String.Empty Then
            SQL = SQL & Conn & "Submitted < " & DB.Quote(DateAdd("d", 1, F_SubmittedUbound.Text))
            Conn = " AND "
        End If

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Protected Sub gvList_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvList.RowCommand
        Select Case e.CommandName
            Case Is = "Remove"
                Try
                    DB.BeginTransaction()
                    ProjectRow.RemoveRow(DB, e.CommandArgument)
                    DB.CommitTransaction()

                    'log Delete Project
                    ProjectID = Convert.ToString(e.CommandArgument)
                    Core.DataLog("Project", PageURL, CurrentUserId, "Delete Project ", ProjectID, "", "", "", UserName)
                    'end log

                    BindList()
                Catch ex As SqlException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    AddError("This project cannot be deleted because it is referenced elsewhere!<br>" & ErrHandler.ErrorText(ex))
                End Try
        End Select
    End Sub

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If
        Dim lnkDelete As ConfirmLinkButton = e.Row.FindControl("lnkDelete")
        lnkDelete.CommandArgument = e.Row.DataItem("ProjectId")

        Dim lnkQuotes As HyperLink = e.Row.FindControl("lnkQuotes")

        lnkQuotes.Visible = dbBuilder.IsPlansOnline

    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?project=NewProject&" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub

   ' Protected Sub btnDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnDashBoard.Click
        'Response.Redirect("/builder/default.aspx")
    'End Sub

End Class
