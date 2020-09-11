Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected CountryId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("SHIPPING_TAX")

        CountryId = Convert.ToInt32(Request("CountryId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If CountryId = 0 Then
            Exit Sub
        End If

        Dim dbCountry As CountryRow = CountryRow.GetRow(DB, CountryId)
        ltlCountryCode.Text = dbCountry.CountryCode
        ltlCountryName.Text = dbCountry.CountryName
        txtShipping.Text = dbCountry.Shipping
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbCountry As CountryRow

            If CountryId <> 0 Then
                dbCountry = CountryRow.GetRow(DB, CountryId)
            Else
                dbCountry = New CountryRow(DB)
            End If
            dbCountry.Shipping = txtShipping.Text

            If CountryId <> 0 Then
                dbCountry.Update()
            Else
                CountryId = dbCountry.Insert
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
End Class

