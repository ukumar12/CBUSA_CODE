Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components
Imports TwoPrice.DataLayer


Public Class Edit
    Inherits AdminPage

    Protected TwoPriceStatusId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("TWO_PRICE_CAMPAIGNS")

        TwoPriceStatusId = Convert.ToInt32(Request("TwoPriceStatusId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If TwoPriceStatusId = 0 Then
            rblIsActive.SelectedValue = True
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbTwoPriceStatus As TwoPriceStatusRow = TwoPriceStatusRow.GetRow(DB, TwoPriceStatusId)
        txtName.Text = dbTwoPriceStatus.Name
        txtValue.Text = dbTwoPriceStatus.Value
        rblIsActive.SelectedValue = dbTwoPriceStatus.IsActive
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbTwoPriceStatus As TwoPriceStatusRow

            If TwoPriceStatusId <> 0 Then
                dbTwoPriceStatus = TwoPriceStatusRow.GetRow(DB, TwoPriceStatusId)
            Else
                dbTwoPriceStatus = New TwoPriceStatusRow(DB)
            End If
            dbTwoPriceStatus.Name = txtName.Text
            dbTwoPriceStatus.Value = txtValue.Text
            dbTwoPriceStatus.IsActive = rblIsActive.SelectedValue

            If TwoPriceStatusId <> 0 Then
                dbTwoPriceStatus.Update()
            Else
                TwoPriceStatusId = dbTwoPriceStatus.Insert
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
        Response.Redirect("delete.aspx?TwoPriceStatusId=" & TwoPriceStatusId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class