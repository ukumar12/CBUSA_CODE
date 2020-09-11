Imports Components
Imports Controls
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private dtAll As DataTable
    Private SupplyPhaseId As Integer

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("SUPPLY_PHASES")
        dtAll = SupplyPhaseRow.GetList(DB)
        If Not IsPostBack Then
            SupplyPhaseId = Request("SupplyPhaseId")
            If SupplyPhaseId = Nothing Then
                Dim r As DataRow() = dtAll.Select("ParentSupplyPhaseId is null")
                If r.Length > 0 Then
                    SupplyPhaseId = r(0)("SupplyPhaseId")
                End If
            End If
            BindList()
        End If
        BindData()
    End Sub

    Private Sub BindList()
        Dim dt As DataTable = LLCRow.GetList(DB, "LLC")
        lbExclude.DataSource = dt
        lbExclude.DataTextField = "LLC"
        lbExclude.DataValueField = "LLCID"
        lbExclude.DataBind()
    End Sub

    Private Sub BindData()
        'Dim dtOpen As DataTable
        'dtOpen = SupplyPhaseRow.GetAncestors(DB, SupplyPhaseId)
        'Dim OpenList As String = String.Empty
        'For Each row As DataRow In dtOpen.Rows
        '    If OpenList = String.Empty Then
        '        OpenList = row("SupplyPhaseId")
        '    Else
        '        OpenList &= "," & row("SupplyPhaseId")
        '    End If
        'Next
        Dim root As DataRow() = dtAll.Select("SupplyPhaseId = " & DB.Number(SupplyPhaseId))

        If Not IsPostBack And SupplyPhaseId <> Nothing Then rtvMain.Value = SupplyPhaseId
        rtvMain.DataSource = dtAll
        rtvMain.DataTextName = "SupplyPhase"
        rtvMain.DataValueName = "SupplyPhaseId"
        rtvMain.ParentFieldName = "ParentSupplyPhaseId"
        rtvMain.DataBind()

        rtvSource.DataSource = dtAll
        rtvSource.DataTextName = "SupplyPhase"
        rtvSource.DataValueName = "SupplyPhaseId"
        rtvSource.ParentFieldName = "ParentSupplyPhaseId"
        rtvSource.DataBind()

        rtvDestination.DataSource = dtAll
        rtvDestination.DataTextName = "SupplyPhase"
        rtvDestination.DataValueName = "SupplyPhaseId"
        rtvDestination.ParentFieldName = "ParentSupplyPhaseId"
        rtvDestination.DataBind()
    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Dim ParentId As Integer
        Dim NewId As Integer
        If rtvMain.Value Is Nothing Then
            AddError("You must select a parent node for the new node")
            Exit Sub
        Else
            ParentId = rtvMain.Value
        End If
        Try
            Dim dtSupplyPhase As New SupplyPhaseRow(DB)
            dtSupplyPhase.ParentSupplyPhaseID = ParentId
            dtSupplyPhase.SupplyPhase = txtNewPhase.Text
            NewId = dtSupplyPhase.Insert()

            dtSupplyPhase.DeleteFromAllLLCs()
            dtSupplyPhase.InsertToLLCs(lbExclude.SelectedValues)

            Dim qs As New Components.URLParameters(Request.QueryString, "SupplyPhaseId")
            qs.Add("SupplyPhaseId", NewId)
            Response.Redirect(Request.Url.AbsolutePath & qs.ToString)
        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
        End Try
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Dim PhaseId As Integer
        If rtvMain.Value Is Nothing Then
            AddError("You must select a node to delete")
            Exit Sub
        Else
            PhaseId = rtvMain.Value
        End If

        Dim dtSupplyPhase As SupplyPhaseRow = SupplyPhaseRow.GetRow(DB, PhaseId)
        If dtSupplyPhase IsNot Nothing Then
            dtSupplyPhase.Remove()
        End If

        Dim qs As New Components.URLParameters(Request.QueryString, "SupplyPhaseId")
        qs.Add("SupplyPhaseId", dtSupplyPhase.ParentSupplyPhaseID)
        Response.Redirect(Request.Url.AbsolutePath & qs.ToString)
    End Sub

    Protected Sub btnRename_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRename.Click
        Dim PhaseId As Integer
        If rtvMain.Value Is Nothing Then
            AddError("You must select a node to rename")
            Exit Sub
        Else
            PhaseId = rtvMain.Value
        End If
        Dim dbPhase As SupplyPhaseRow = SupplyPhaseRow.GetRow(DB, PhaseId)
        If dbPhase IsNot Nothing Then
            dbPhase.SupplyPhase = txtRename.Text
            dbPhase.Update()
        End If
        Response.Redirect(Request.Url.ToString)
    End Sub

    Protected Sub btnMove_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnMove.Click
        If rtvSource.Value = Nothing Or rtvDestination.Value = Nothing Then
            AddError("You must select a Source and Destination Supply Phase")
            Exit Sub
        ElseIf rtvSource.Value = rtvDestination.Value Then
            AddError("Source and Destination Supply Phases must be different")
            Exit Sub
        ElseIf SupplyPhaseRow.GetAncestors(DB, rtvDestination.Value).Select("SupplyPhaseId=" & rtvSource.Value).Length > 0 Then
            AddError("Destination Supply Phase cannot be a child of the Source Supply Phase")
            Exit Sub
        End If

        Dim dbSource As SupplyPhaseRow = SupplyPhaseRow.GetRow(DB, rtvSource.Value)
        Dim dbDest As SupplyPhaseRow = SupplyPhaseRow.GetRow(DB, rtvDestination.Value)

        dbSource.ParentSupplyPhaseID = dbDest.SupplyPhaseID
        dbSource.Update()
        Response.Redirect(Request.Url.ToString)
    End Sub
End Class


