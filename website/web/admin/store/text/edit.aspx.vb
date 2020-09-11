Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected Code As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("STORE")

        Code = Request("Code")
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        Dim dbCustomText As CustomTextRow = CustomTextRow.GetRowByCode(DB, Code)
        If dbCustomText.TextId = 0 Then
            Response.Redirect("/admin/main.aspx")
        End If

        ltlTitle.Text = dbCustomText.Title
        txtValue.Value = dbCustomText.Value
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbCustomText As CustomTextRow

            dbCustomText = CustomTextRow.GetRowByCode(DB, Code)
            dbCustomText.Value = txtValue.Value

            dbCustomText.Update()
            DB.CommitTransaction()

            Response.Redirect("confirm.aspx?Text=" & ltlTitle.Text)

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("/admin/main.aspx")
    End Sub

End Class

