Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected Index As Integer = 0
    Protected Type As String = String.Empty

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("CALLOUTS")

        Index = Convert.ToInt32(Request("index"))
        Type = Convert.ToString(Request("type"))

        Me.divLabel.InnerHtml = UCase(Type) & " Callout" & Index

        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()

        Dim SQL As String = "SELECT TOP 1 Callout" & Index & " FROM " & Type & "Callout"
        Dim CalloutHTML As String = IIf(IsDBNull(Me.DB.ExecuteScalar(SQL)), "", Me.DB.ExecuteScalar(SQL))

        txtCallOut.Value = CalloutHTML

    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click

        Dim SQL As String = String.Empty

        If Not IsValid Then Exit Sub

        Try

            SQL = "UPDATE " & Type & "Callout SET Callout" & Index & " = " & Me.DB.NQuote(txtCallOut.Value)

            DB.BeginTransaction()
            Me.DB.ExecuteSQL(SQL)
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
