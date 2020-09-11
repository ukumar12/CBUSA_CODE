Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected BuilderRoleID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'CheckAccess("BUILDER_ROLES")

        BuilderRoleID = Convert.ToInt32(Request("BuilderRoleID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If BuilderRoleID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbBuilderRole As BuilderRoleRow = BuilderRoleRow.GetRow(DB, BuilderRoleID)
        txtBuilderRole.Text = dbBuilderRole.BuilderRole
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbBuilderRole As BuilderRoleRow

            If BuilderRoleID <> 0 Then
                dbBuilderRole = BuilderRoleRow.GetRow(DB, BuilderRoleID)
            Else
                dbBuilderRole = New BuilderRoleRow(DB)
            End If
            dbBuilderRole.BuilderRole = txtBuilderRole.Text

            If BuilderRoleID <> 0 Then
                dbBuilderRole.Update()
            Else
                BuilderRoleID = dbBuilderRole.Insert
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
        Response.Redirect("delete.aspx?BuilderRoleID=" & BuilderRoleID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

