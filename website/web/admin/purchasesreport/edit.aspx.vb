Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

    Protected PurchasesReportID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("PURCHASES_REPORTS")

        PurchasesReportID = Convert.ToInt32(Request("PurchasesReportID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        drpBuilderID.Datasource = BuilderRow.GetList(DB, "CompanyName")
        drpBuilderID.DataValueField = "BuilderID"
        drpBuilderID.DataTextField = "CompanyName"
        drpBuilderID.Databind()
        drpBuilderID.Items.Insert(0, New ListItem("", ""))

        If PurchasesReportID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbPurchasesReport As PurchasesReportRow = PurchasesReportRow.GetRow(DB, PurchasesReportID)
        txtPeriodYear.Text = dbPurchasesReport.PeriodYear
        txtPeriodQuarter.Text = dbPurchasesReport.PeriodQuarter
        txtCreatorBuilderAccountID.Text = dbPurchasesReport.CreatorBuilderAccountID
        drpBuilderID.SelectedValue = dbPurchasesReport.BuilderID
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbPurchasesReport As PurchasesReportRow

            If PurchasesReportID <> 0 Then
                dbPurchasesReport = PurchasesReportRow.GetRow(DB, PurchasesReportID)
            Else
                dbPurchasesReport = New PurchasesReportRow(DB)
            End If
            dbPurchasesReport.PeriodYear = txtPeriodYear.Text
            dbPurchasesReport.PeriodQuarter = txtPeriodQuarter.Text
            dbPurchasesReport.CreatorBuilderAccountID = txtCreatorBuilderAccountID.Text
            dbPurchasesReport.BuilderID = drpBuilderID.SelectedValue

            If PurchasesReportID <> 0 Then
                dbPurchasesReport.Update()
            Else
                PurchasesReportID = dbPurchasesReport.Insert
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
        Response.Redirect("delete.aspx?PurchasesReportID=" & PurchasesReportID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
