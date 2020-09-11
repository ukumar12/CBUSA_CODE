Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("MARKET_INDICATORS")

        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()

        If DataLayer.BuilderPricingInformationRow.GetList(Me.DB).Rows.Count = 1 Then
            txtPricingInformation.Value = DataLayer.BuilderPricingInformationRow.GetList(Me.DB).Rows(0).Item("PricingInformation").ToString
        End If

    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click

        Dim SQL As String = String.Empty
        Dim dbBuilderPricingInformation As BuilderPricingInformationRow

        If Not IsValid Then Exit Sub

        Try

            If DataLayer.BuilderPricingInformationRow.GetList(Me.DB).Rows.Count = 1 Then
                dbBuilderPricingInformation = DataLayer.BuilderPricingInformationRow.GetRow(Me.DB, CType(DataLayer.BuilderPricingInformationRow.GetList(Me.DB).Rows(0).Item("BuilderPricingInformationID"), Integer))
            Else
                dbBuilderPricingInformation = New DataLayer.BuilderPricingInformationRow(Me.DB)
            End If

            dbBuilderPricingInformation.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
            dbBuilderPricingInformation.Updated = Now
            dbBuilderPricingInformation.PricingInformation = txtPricingInformation.Value

            If dbBuilderPricingInformation.BuilderPricingInformationID = 0 Then
                dbBuilderPricingInformation.Insert()
            Else
                dbBuilderPricingInformation.Update()
            End If

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
