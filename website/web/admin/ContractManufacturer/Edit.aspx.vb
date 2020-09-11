Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected NCPManufacturerID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BUILDERS")

        NCPManufacturerID = Convert.ToInt32(Request("NCPManufacturerID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()

        drpMailingState.DataSource = StateRow.GetStateList(DB)
        drpMailingState.DataTextField = "StateName"
        drpMailingState.DataValueField = "StateCode"
        drpMailingState.DataBind()
        drpMailingState.Items.Insert(0, New ListItem("", ""))
        If NCPManufacturerID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbNCPManufacturer As NCPManufacturerRow = NCPManufacturerRow.GetRow(DB, NCPManufacturerID)

        txtCompanyName.Text = dbNCPManufacturer.CompanyName
        txtMailingAddress.Text = dbNCPManufacturer.MailingAddress
        txtMailingCity.Text = dbNCPManufacturer.MailingCity
        drpMailingState.SelectedValue = dbNCPManufacturer.MailingState
        txtMailingZip.Text = dbNCPManufacturer.MailingZip
        txtWebsite.Text = dbNCPManufacturer.Website
        txtPrimaryContactName.Text = dbNCPManufacturer.PrimaryContactName
        txtPrimaryContactEmail.Text = dbNCPManufacturer.PrimaryContactEmail
        txtPrimaryContactPhone.Text = dbNCPManufacturer.PrimaryContactPhone
        txtAPContactName.Text = dbNCPManufacturer.APContactName
        txtAPContactEmail.Text = dbNCPManufacturer.APContactEmail
        txtAPContactPhone.Text = dbNCPManufacturer.APContactPhone
        txtPaymentTerms.Text = dbNCPManufacturer.PaymentTerms
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbNCPManufacturer As NCPManufacturerRow

            If NCPManufacturerID <> 0 Then
                dbNCPManufacturer = NCPManufacturerRow.GetRow(DB, NCPManufacturerID)
            Else
                dbNCPManufacturer = New NCPManufacturerRow(DB)
            End If

            dbNCPManufacturer.ClassID = "NCP"
            dbNCPManufacturer.CompanyName = txtCompanyName.Text
            dbNCPManufacturer.MailingAddress = txtMailingAddress.Text
            dbNCPManufacturer.MailingCity = txtMailingCity.Text
            dbNCPManufacturer.MailingState = drpMailingState.SelectedValue
            dbNCPManufacturer.MailingZip = txtMailingZip.Text
            dbNCPManufacturer.Website = txtWebsite.Text
            dbNCPManufacturer.PrimaryContactName = txtPrimaryContactName.Text
            dbNCPManufacturer.PrimaryContactEmail = txtPrimaryContactEmail.Text
            dbNCPManufacturer.PrimaryContactPhone = txtPrimaryContactPhone.Text
            dbNCPManufacturer.APContactName = txtAPContactName.Text
            dbNCPManufacturer.APContactEmail = txtAPContactEmail.Text
            dbNCPManufacturer.APContactPhone = txtAPContactPhone.Text
            dbNCPManufacturer.PaymentTerms = txtPaymentTerms.Text

            If NCPManufacturerID <> 0 Then
                dbNCPManufacturer.Update()
            Else
                NCPManufacturerID = dbNCPManufacturer.Insert
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
        Response.Redirect("delete.aspx?NCPManufacturerID=" & NCPManufacturerID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
