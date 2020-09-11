Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected ExcludeSearchWordId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("STORE")

        ExcludeSearchWordId = Convert.ToInt32(Request("ExcludeSearchWordId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If ExcludeSearchWordId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbExcludedSearchWords As ExcludedSearchWordsRow = ExcludedSearchWordsRow.GetRow(DB, ExcludeSearchWordId)
        txtExcludeSearchWord.Text = dbExcludedSearchWords.ExcludeSearchWord
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbExcludedSearchWords As ExcludedSearchWordsRow

            If ExcludeSearchWordId <> 0 Then
                dbExcludedSearchWords = ExcludedSearchWordsRow.GetRow(DB, ExcludeSearchWordId)
            Else
                dbExcludedSearchWords = New ExcludedSearchWordsRow(DB)
            End If
            dbExcludedSearchWords.ExcludeSearchWord = txtExcludeSearchWord.Text

            If ExcludeSearchWordId <> 0 Then
                dbExcludedSearchWords.Update()
            Else
                ExcludeSearchWordId = dbExcludedSearchWords.Insert
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
        Response.Redirect("delete.aspx?ExcludeSearchWordId=" & ExcludeSearchWordId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
