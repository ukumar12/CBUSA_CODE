Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected RatingCategoryID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("RATING_CATEGORIES")

        RatingCategoryID = Convert.ToInt32(Request("RatingCategoryID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If RatingCategoryID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbRatingCategory As RatingCategoryRow = RatingCategoryRow.GetRow(DB, RatingCategoryID)
        txtRatingCategory.Text = dbRatingCategory.RatingCategory
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbRatingCategory As RatingCategoryRow

            If RatingCategoryID <> 0 Then
                dbRatingCategory = RatingCategoryRow.GetRow(DB, RatingCategoryID)
            Else
                dbRatingCategory = New RatingCategoryRow(DB)
            End If
            dbRatingCategory.RatingCategory = txtRatingCategory.Text

            If RatingCategoryID <> 0 Then
                dbRatingCategory.Update()
            Else
                RatingCategoryID = dbRatingCategory.Insert
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
        Response.Redirect("delete.aspx?RatingCategoryID=" & RatingCategoryID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

