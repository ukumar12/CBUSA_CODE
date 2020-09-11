Imports Components
Imports DataLayer

Partial Class test
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            tvSupplyPhases.DataSource = SupplyPhaseRow.GetList(DB)
            tvSupplyPhases.DataTextField = "SupplyPhase"
            tvSupplyPhases.DataValueField = "SupplyPhaseId"
            tvSupplyPhases.DataParentField = "ParentSupplyPhaseId"
            tvSupplyPhases.DataBind()
        End If
    End Sub
End Class
