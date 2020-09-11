Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected VendorRegistrationID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("VENDOR_REGISTRATIONS")

		VendorRegistrationID = Convert.ToInt32(Request("VendorRegistrationID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		drpVendorID.Datasource = VendorRow.GetList(DB,"CompanyName")
		drpVendorID.DataValueField = "VendorID"
		drpVendorID.DataTextField = "CompanyName"
		drpVendorID.Databind
		drpVendorID.Items.Insert(0, New ListItem("",""))
	
		drpPrimarySupplyPhaseID.Datasource = SupplyPhaseRow.GetList(DB,"SupplyPhase")
		drpPrimarySupplyPhaseID.DataValueField = "SupplyPhaseID"
		drpPrimarySupplyPhaseID.DataTextField = "SupplyPhase"
		drpPrimarySupplyPhaseID.Databind
		drpPrimarySupplyPhaseID.Items.Insert(0, New ListItem("",""))
	
		drpSecondarySupplyPhaseID.Datasource = SupplyPhaseRow.GetList(DB,"SupplyPhase")
		drpSecondarySupplyPhaseID.DataValueField = "SupplyPhaseID"
		drpSecondarySupplyPhaseID.DataTextField = "SupplyPhase"
		drpSecondarySupplyPhaseID.Databind
		drpSecondarySupplyPhaseID.Items.Insert(0, New ListItem("",""))
	
		drpRegistrationStatusID.Datasource = RegistrationStatusRow.GetList(DB,"RegistrationStatus")
		drpRegistrationStatusID.DataValueField = "RegistrationStatusID"
		drpRegistrationStatusID.DataTextField = "RegistrationStatus"
		drpRegistrationStatusID.Databind
		drpRegistrationStatusID.Items.Insert(0, New ListItem("",""))
	
		If VendorRegistrationID = 0 Then
            btnDelete.Visible = False
            btnEditBusiness.Visible = False
            btnEditCustomer.Visible = False
            btnEditFinancial.Visible = False
            btnEditMember.Visible = False
			Exit Sub
		End if

		Dim dbVendorRegistration As VendorRegistrationRow = VendorRegistrationRow.GetRow(DB, VendorRegistrationID)
		txtHistoricVendorID.Text = dbVendorRegistration.HistoricVendorID
        txtYearStarted.Text = dbVendorRegistration.YearStarted
        txtEmployees.Text = dbVendorRegistration.Employees
        txtProductsOffered.Text = dbVendorRegistration.ProductsOffered
        txtCompanyMemberships.Text = dbVendorRegistration.CompanyMemberships
        txtSubsidiaryExplanation.Text = dbVendorRegistration.SubsidiaryExplanation
        txtLawsuitExplanation.Text = dbVendorRegistration.LawsuitExplanation
        txtSupplyArea.Text = dbVendorRegistration.SupplyArea
        txtPreparerFirstName.Text = dbVendorRegistration.PreparerFirstName
        txtPreparerLastName.Text = dbVendorRegistration.PreparerLastName
        txtNotes.Text = dbVendorRegistration.Notes
        dtSubmitted.Value = dbVendorRegistration.Submitted.ToString("MM/dd/yyyy")
        dtUpdated.Value = dbVendorRegistration.Updated.ToString("MM/dd/yyyy")
        dtApproved.Value = dbVendorRegistration.Approved
        drpVendorID.SelectedValue = dbVendorRegistration.VendorID
        drpPrimarySupplyPhaseID.SelectedValue = dbVendorRegistration.PrimarySupplyPhaseID
        drpSecondarySupplyPhaseID.SelectedValue = dbVendorRegistration.SecondarySupplyPhaseID
        drpRegistrationStatusID.SelectedValue = dbVendorRegistration.RegistrationStatusID
        rblIsSubsidiary.SelectedValue = dbVendorRegistration.IsSubsidiary
        rblHadLawsuit.SelectedValue = dbVendorRegistration.HadLawsuit
        rblAcceptsTerms.SelectedValue = dbVendorRegistration.AcceptsTerms
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbVendorRegistration As VendorRegistrationRow

            If VendorRegistrationID <> 0 Then
                dbVendorRegistration = VendorRegistrationRow.GetRow(DB, VendorRegistrationID)
            Else
                dbVendorRegistration = New VendorRegistrationRow(DB)
            End If
            dbVendorRegistration.HistoricVendorID = IIf(txtHistoricVendorID.Text = "", Nothing, txtHistoricVendorID.Text)
            dbVendorRegistration.YearStarted = txtYearStarted.Text
            dbVendorRegistration.Employees = txtEmployees.Text
            dbVendorRegistration.ProductsOffered = txtProductsOffered.Text
            dbVendorRegistration.CompanyMemberships = txtCompanyMemberships.Text
            dbVendorRegistration.SubsidiaryExplanation = txtSubsidiaryExplanation.Text
            dbVendorRegistration.LawsuitExplanation = txtLawsuitExplanation.Text
            dbVendorRegistration.SupplyArea = txtSupplyArea.Text
            dbVendorRegistration.PreparerFirstName = txtPreparerFirstName.Text
            dbVendorRegistration.PreparerLastName = txtPreparerLastName.Text
            dbVendorRegistration.Notes = txtNotes.Text
            dbVendorRegistration.Approved = dtApproved.Value
            dbVendorRegistration.VendorID = drpVendorID.SelectedValue
            dbVendorRegistration.PrimarySupplyPhaseID = drpPrimarySupplyPhaseID.SelectedValue
            dbVendorRegistration.SecondarySupplyPhaseID = drpSecondarySupplyPhaseID.SelectedValue
            dbVendorRegistration.RegistrationStatusID = drpRegistrationStatusID.SelectedValue
            dbVendorRegistration.IsSubsidiary = rblIsSubsidiary.SelectedValue
            dbVendorRegistration.HadLawsuit = rblHadLawsuit.SelectedValue
            dbVendorRegistration.AcceptsTerms = rblAcceptsTerms.SelectedValue

            If VendorRegistrationID <> 0 Then
                dbVendorRegistration.Update()
            Else
                VendorRegistrationID = dbVendorRegistration.Insert
            End If

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
		Response.Redirect("delete.aspx?VendorRegistrationID=" & VendorRegistrationID & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnEditBusiness_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEditBusiness.Click
        Response.Redirect("businessreferences/default.aspx?VendorRegistrationID=" & VendorRegistrationID)
    End Sub

    Protected Sub btnEditCustomer_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEditCustomer.Click
        Response.Redirect("customerreferences/default.aspx?VendorRegistrationID=" & VendorRegistrationID)
    End Sub

    Protected Sub btnEditFinancial_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEditFinancial.Click
        Response.Redirect("financialreferences/default.aspx?VendorRegistrationID=" & VendorRegistrationID)
    End Sub

    Protected Sub btnEditMember_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEditMember.Click
        Response.Redirect("memberreferences/default.aspx?VendorRegistrationID=" & VendorRegistrationID)
    End Sub
End Class
