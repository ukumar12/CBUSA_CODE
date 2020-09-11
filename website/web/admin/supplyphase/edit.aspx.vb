Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected SupplyPhaseID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("SUPPLY_PHASES")

		SupplyPhaseID = Convert.ToInt32(Request("SupplyPhaseID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		drpParentSupplyPhaseID.Datasource = SupplyPhaseRow.GetList(DB)
		drpParentSupplyPhaseID.DataValueField = "SupplyPhaseID"
		drpParentSupplyPhaseID.DataTextField = "SupplyPhase"
		drpParentSupplyPhaseID.Databind
		drpParentSupplyPhaseID.Items.Insert(0, New ListItem("",""))
	
        cblExcludedLLC.DataSource = LLCRow.GetList(DB, "LLC")
        cblExcludedLLC.DataTextField = "LLC"
        cblExcludedLLC.DataValueField = "LLCID"
        cblExcludedLLC.DataBind()

		If SupplyPhaseID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbSupplyPhase As SupplyPhaseRow = SupplyPhaseRow.GetRow(DB, SupplyPhaseID)
		txtSupplyPhase.Text = dbSupplyPhase.SupplyPhase
		txtPriceLockDays.Text = dbSupplyPhase.PriceLockDays
		drpParentSupplyPhaseID.SelectedValue = dbSupplyPhase.ParentSupplyPhaseID
		cblExcludedLLC.SelectedValues = dbSupplyPhase.GetSelectedLLCs
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbSupplyPhase As SupplyPhaseRow

			If SupplyPhaseID <> 0 Then
				dbSupplyPhase = SupplyPhaseRow.GetRow(DB, SupplyPhaseID)
			Else
				dbSupplyPhase = New SupplyPhaseRow(DB)
			End If
			dbSupplyPhase.SupplyPhase = txtSupplyPhase.Text
			dbSupplyPhase.PriceLockDays = txtPriceLockDays.Text
			dbSupplyPhase.ParentSupplyPhaseID = drpParentSupplyPhaseID.SelectedValue
	
			If SupplyPhaseID <> 0 Then
				dbSupplyPhase.Update()
			Else
				SupplyPhaseID = dbSupplyPhase.Insert
			End If
			dbSupplyPhase.DeleteFromAllLLCs()
			dbSupplyPhase.InsertToLLCs(cblExcludedLLC.SelectedValues)
	
			DB.CommitTransaction()

	
			Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

		Catch ex As SqlException
			If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
		End Try
	End Sub

	Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
		Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
	End Sub

	Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
		Response.Redirect("delete.aspx?SupplyPhaseID=" & SupplyPhaseID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
