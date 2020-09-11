Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected BusinessTypeID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BUSINESS_TYPES")

        BusinessTypeID = Convert.ToInt32(Request("BusinessTypeID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If BusinessTypeID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbBusinessType As BusinessTypeRow = BusinessTypeRow.GetRow(DB, BusinessTypeID)
        txtBusinessType.Text = dbBusinessType.BusinessType
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbBusinessType As BusinessTypeRow

            If BusinessTypeID <> 0 Then
                dbBusinessType = BusinessTypeRow.GetRow(DB, BusinessTypeID)
            Else
                dbBusinessType = New BusinessTypeRow(DB)
            End If
            dbBusinessType.BusinessType = txtBusinessType.Text

            If BusinessTypeID <> 0 Then
                dbBusinessType.Update()
            Else
                BusinessTypeID = dbBusinessType.Insert
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
        Response.Redirect("delete.aspx?BusinessTypeID=" & BusinessTypeID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

