Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected FeatureId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("STORE")

        FeatureId = Convert.ToInt32(Request("FeatureId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If FeatureId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbStoreFeature As StoreFeatureRow = StoreFeatureRow.GetRow(DB, FeatureId)
        txtName.Text = dbStoreFeature.Name
        chkIsUnique.Checked = dbStoreFeature.IsUnique
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbStoreFeature As StoreFeatureRow

            If FeatureId <> 0 Then
                dbStoreFeature = StoreFeatureRow.GetRow(DB, FeatureId)
            Else
                dbStoreFeature = New StoreFeatureRow(DB)
            End If
            dbStoreFeature.Name = txtName.Text
            dbStoreFeature.IsUnique = chkIsUnique.Checked

            If FeatureId <> 0 Then
                dbStoreFeature.Update()
            Else
                FeatureId = dbStoreFeature.Insert
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
        Response.Redirect("delete.aspx?FeatureId=" & FeatureId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

