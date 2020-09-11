Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected PortfolioID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("PORTFOLIOS")

        PortfolioID = Convert.ToInt32(Request("PortfolioID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If PortfolioID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbPortfolio As PortfolioRow = PortfolioRow.GetRow(DB, PortfolioID)
        txtPortfolio.Text = dbPortfolio.Portfolio
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbPortfolio As PortfolioRow

            If PortfolioID <> 0 Then
                dbPortfolio = PortfolioRow.GetRow(DB, PortfolioID)
            Else
                dbPortfolio = New PortfolioRow(DB)
            End If
            dbPortfolio.Portfolio = txtPortfolio.Text

            If PortfolioID <> 0 Then
                dbPortfolio.Update()
            Else
                PortfolioID = dbPortfolio.Insert
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
        Response.Redirect("delete.aspx?PortfolioID=" & PortfolioID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

