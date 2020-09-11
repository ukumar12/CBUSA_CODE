Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected QuoteRequestId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("VENDORS")

        QuoteRequestId = Convert.ToInt32(Request("QuoteRequestId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        drpQuoteId.Datasource = POQuoteRow.GetList(DB, "Title")
        drpQuoteId.DataValueField = "QuoteId"
        drpQuoteId.DataTextField = "Title"
        drpQuoteId.Databind()
        drpQuoteId.Items.Insert(0, New ListItem("", ""))

        drpVendorId.Datasource = VendorRow.GetList(DB, "CompanyName")
        drpVendorId.DataValueField = "VendorID"
        drpVendorId.DataTextField = "CompanyName"
        drpVendorId.Databind()
        drpVendorId.Items.Insert(0, New ListItem("", ""))

        If QuoteRequestId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbPOQuoteRequest As POQuoteRequestRow = POQuoteRequestRow.GetRow(DB, QuoteRequestId)
        txtRequestStatus.Text = dbPOQuoteRequest.RequestStatus
        txtVendorContactName.Text = dbPOQuoteRequest.VendorContactName
        txtVendorContactPhone.Text = dbPOQuoteRequest.VendorContactPhone
        txtVendorContactEmail.Text = dbPOQuoteRequest.VendorContactEmail
        txtQuoteTotal.Text = dbPOQuoteRequest.QuoteTotal
        txtCompletionTime.Text = dbPOQuoteRequest.CompletionTime
        txtPaymentTerms.Text = dbPOQuoteRequest.PaymentTerms
        dtQuoteExpirationDate.Value = dbPOQuoteRequest.QuoteExpirationDate
        dtStartDate.Value = dbPOQuoteRequest.StartDate
        drpQuoteId.SelectedValue = dbPOQuoteRequest.QuoteId
        drpVendorId.SelectedValue = dbPOQuoteRequest.VendorId
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbPOQuoteRequest As POQuoteRequestRow

            If QuoteRequestId <> 0 Then
                dbPOQuoteRequest = POQuoteRequestRow.GetRow(DB, QuoteRequestId)
            Else
                dbPOQuoteRequest = New POQuoteRequestRow(DB)
            End If
            dbPOQuoteRequest.RequestStatus = txtRequestStatus.Text
            dbPOQuoteRequest.VendorContactName = txtVendorContactName.Text
            dbPOQuoteRequest.VendorContactPhone = txtVendorContactPhone.Text
            dbPOQuoteRequest.VendorContactEmail = txtVendorContactEmail.Text
            dbPOQuoteRequest.QuoteTotal = txtQuoteTotal.Text
            dbPOQuoteRequest.CompletionTime = txtCompletionTime.Text
            dbPOQuoteRequest.PaymentTerms = txtPaymentTerms.Text
            dbPOQuoteRequest.QuoteExpirationDate = dtQuoteExpirationDate.Value
            dbPOQuoteRequest.StartDate = dtStartDate.Value
            dbPOQuoteRequest.QuoteId = drpQuoteId.SelectedValue
            dbPOQuoteRequest.VendorId = drpVendorId.SelectedValue

            If QuoteRequestId <> 0 Then
                dbPOQuoteRequest.Update()
            Else
                QuoteRequestId = dbPOQuoteRequest.Insert
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
        Response.Redirect("delete.aspx?QuoteRequestId=" & QuoteRequestId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class