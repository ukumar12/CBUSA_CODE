Imports Components
Imports DataLayer

Partial Class takeoffs_default
    Inherits SitePage

    Private dbTakeoff As TakeOffRow

    Private m_dtComparisons As DataTable
    Dim Project As String = String.Empty
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private CurrentTakeoffId As String = ""
    Private EditTakeoffId As String = ""

    Private ReadOnly Property dtComparisons() As DataTable
        Get
            If m_dtComparisons Is Nothing Then
                m_dtComparisons = PriceComparisonRow.GetSavedComparisons(DB, Session("BuilderId"))
            End If
            Return m_dtComparisons
        End Get
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        dbTakeoff = TakeOffRow.GetRow(DB, Session("CurrentTakeoffId"))
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not (IsLoggedInBuilder() Or IsLoggedInVendor() Or Session("AdminId") IsNot Nothing) Then
            Response.Redirect("/default.aspx")
        End If
        Project = Request("F_ProjectID")

        If Project <> Nothing Then
            btnViewAll.Visible = True
        End If

        gvList.BindList = AddressOf BindData

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")
        CurrentTakeoffId = Session("CurrentTakeoffId")

        If Not IsPostBack Then

            If Session("BuilderId") IsNot Nothing Then
                Core.DataLog("Takeoff", PageURL, CurrentUserId, "Builder Top Menu Click", "", "", "", "", UserName)
            End If

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "Saved"
                gvList.SortOrder = "Desc"
            End If

            BindData()

            If dbTakeoff.TakeOffID <> Nothing Then
                frmConfirm.OpenTriggerId = btnNew.UniqueID
                btnCurrent.Visible = True
                btnCurrent.Text = "Current Takeoff: " & IIf(dbTakeoff.Title = Nothing, "(Unsaved)", dbTakeoff.Title)
            End If
        End If

    End Sub

    Private Sub BindData()
        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        gvList.DataKeyNames = New String() {"TakeoffId"}

        gvList.Pager.NofRecords = TakeOffRow.GetBuilderTakeoffCount(DB, Session("BuilderId"), Project)

        Dim res As DataTable = TakeOffRow.GetBuilderTakeoffs(DB, Session("BuilderId"), gvList.SortBy, gvList.SortOrder, gvList.PageIndex + 1, gvList.PageSize, Project)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Protected Sub gvList_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvList.RowCommand
        If e.CommandName = "DeleteTakeoff" Then
            TakeOffRow.RemoveRow(DB, e.CommandArgument)
            'log delete takeoff
            Core.DataLog("Takeoff", PageURL, CurrentUserId, "Delete Takeoff", CurrentTakeoffId, "", "", "", UserName)
            'end log
            BindData()
        ElseIf e.CommandName = "ChangeTakeoff" Then
            ChangeCurrentTakeoff()
            dbTakeoff = TakeOffRow.GetRow(DB, e.CommandArgument)
            Session("CurrentTakeoffId") = dbTakeoff.TakeOffID
            'log edit takeoff
            EditTakeoffId = Session("CurrentTakeoffId")
            Core.DataLog("Takeoff", PageURL, CurrentUserId, "Edit Takeoff", EditTakeoffId, "", "", "", UserName)
            'end log
            Response.Redirect("edit.aspx")
        End If
    End Sub

    Protected Sub btnNew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNew.Click
        Session("CurrentTakeoffId") = Nothing
        'log create takeoff
        Core.DataLog("Takeoff", PageURL, CurrentUserId, "Create Takeoff", CurrentTakeoffId, "", "", "", UserName)
        'end log
        Response.Redirect("edit.aspx")
    End Sub

    Protected Sub frmConfirm_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmConfirm.Postback
        ChangeCurrentTakeoff()
        Session("CurrentTakeoffId") = hdnNewID.Value
        Session("PriceComparisonId") = Nothing
        Response.Redirect("edit.aspx")
    End Sub

    Private Sub ChangeCurrentTakeoff()
        Dim dbTakeoff As TakeOffRow = TakeOffRow.GetRow(DB, Session("CurrentTakeoffId"))
        If txtTitle.Text <> Nothing Then
            dbTakeoff.Title = txtTitle.Text
            If dbTakeoff.BuilderID = Nothing Then
                dbTakeoff.BuilderID = Session("BuilderId")
            End If
            If lstProjects.Value <> Nothing Then
                dbTakeoff.ProjectID = lstProjects.Value
            End If
            dbTakeoff.Update()
            txtTitle.Text = Nothing
        ElseIf dbTakeoff.Title = String.Empty Then
            dbTakeoff.Remove()
        End If
    End Sub

    Protected Sub btnCurrent_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCurrent.Click
        Response.Redirect("edit.aspx")
    End Sub

    Protected Sub frmConfirm_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmConfirm.TemplateLoaded
        If Session("BuilderId") IsNot Nothing Then
            lstProjects.WhereClause = "BuilderID=" & DB.Number(Session("BuilderID"))
        Else
            lstProjects.WhereClause = "BuilderID=" & DB.Number(Session("TakeoffForId"))
        End If

        If dbTakeoff.Title = Nothing Then
            phSaveForm.Visible = True
        Else
            txtTitle.Text = dbTakeoff.Title
            phSaveForm.Visible = False
        End If
    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Dim cnt As Integer = 0
        If Session("CurrentTakeoffId") Is Nothing Then
            Dim dbTakeoff As New TakeOffRow(DB)
            dbTakeoff.BuilderID = Session("BuilderId")
            Dim principal As AdminPrincipal = IIf(TypeOf HttpContext.Current.User Is AdminPrincipal, HttpContext.Current.User, Nothing)
            If principal IsNot Nothing Then
                dbTakeoff.AdminID = AdminRow.GetRowByUsername(DB, principal.Username).AdminId
            End If
            dbTakeoff.VendorID = Session("VendorId")
            Session("CurrentTakeoffId") = dbTakeoff.Insert
        End If
        For Each row As GridViewRow In gvList.Rows
            Dim cb As CheckBox = row.FindControl("cbInclude")
            If cb.Checked Then
                cnt += 1
                Dim TakeoffId As Integer = gvList.DataKeys(row.RowIndex)(0)
                If TakeoffId <> Nothing Then
                    TakeOffRow.CopyToTakeoff(DB, TakeoffId, Session("CurrentTakeoffId"))
                End If
            End If
        Next
        'log create takeoff
        Core.DataLog("Takeoff", PageURL, CurrentUserId, "Add Products from Existing Takeoff", CurrentTakeoffId, "", "", "", UserName)
        'end log
        If cnt = 0 Then
            AddError("You must select at least one takeoff to add from")
        Else
            Response.Redirect("edit.aspx")
        End If
    End Sub

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then
            Exit Sub
        End If

        Dim btnEdit As ImageButton = e.Row.FindControl("btnEdit")
        Dim ltlComparisons As Literal = e.Row.FindControl("ltlComparisons")

        If Session("CurrentTakeoffId") IsNot Nothing Then
            btnEdit.OnClientClick = "return OpenConfirmForm(""" & e.Row.DataItem("TakeoffID") & """);"
        End If

        'Dim dt As DataTable = PriceComparisonRow.GetLatestSavedComparison(DB, Session("BuilderId"), e.Row.DataItem("TakeoffID"))
        Dim dt As DataTable = PriceComparisonRow.GetLastComparison(DB, e.Row.DataItem("TakeoffID"))

        For Each row As DataRow In dt.Rows
            ltlComparisons.Text &= "<a class=""smallest"" href=""/comparison/default.aspx?PriceComparisonId=" & row("PriceComparisonID") & """>" & row("Created") & "</a><br/>"
        Next
    End Sub

    Protected Sub btnViewAll_Click(sender As Object, e As System.EventArgs) Handles btnViewAll.Click
        Response.Redirect("default.aspx")
    End Sub

    Private Sub ddlPageSize_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlPageSize.SelectedIndexChanged
        gvList.PageSize = CInt(ddlPageSize.SelectedValue)

        gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
        gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
        If gvList.SortBy = String.Empty Then
            gvList.SortBy = "Saved"
            gvList.SortOrder = "Desc"
        End If

        BindData()
    End Sub

End Class
