Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected PriceComparisonID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("PRICE_COMPARISONS")

        PriceComparisonID = Convert.ToInt32(Request("PriceComparisonID"))
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

        drpAdminID.DataSource = AdminRow.GetAllAdmins(DB)
        drpAdminID.DataValueField = "AdminId"
        drpAdminID.DataTextField = "Username"
        drpAdminID.Databind()
        drpAdminID.Items.Insert(0, New ListItem("", ""))

        If PriceComparisonID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbPriceComparison As PriceComparisonRow = PriceComparisonRow.GetRow(DB, PriceComparisonID)
        drpBuilderID.SelectedValue = dbPriceComparison.BuilderID
        drpAdminID.SelectedValue = dbPriceComparison.AdminID
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbPriceComparison As PriceComparisonRow

            If PriceComparisonID <> 0 Then
                dbPriceComparison = PriceComparisonRow.GetRow(DB, PriceComparisonID)
            Else
                dbPriceComparison = New PriceComparisonRow(DB)
            End If
            dbPriceComparison.BuilderID = drpBuilderID.SelectedValue
            dbPriceComparison.AdminID = drpAdminID.SelectedValue

            If PriceComparisonID <> 0 Then
                dbPriceComparison.Update()
            Else
                PriceComparisonID = dbPriceComparison.Insert
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
        Response.Redirect("delete.aspx?PriceComparisonID=" & PriceComparisonID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

