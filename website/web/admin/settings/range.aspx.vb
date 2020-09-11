Imports Components
Imports System.Data
Imports DataLayer
Imports Controls
Imports System.Data.SqlClient

Public Class range
    Inherits AdminPage

    Private RowCount As Integer = 10
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("USERS")

        If Not IsPostBack Then
            BindRepeater()
        End If
    End Sub

    Private Sub BindRepeater()
        Dim dt As DataTable = StoreShippingRangeRow.GetRanges(DB)
        For i As Integer = dt.Rows.Count To RowCount - 1
            Dim row As DataRow
            row = dt.NewRow()
            row("RangeId") = "0"
            row("ShippingFrom") = "0"
            row("ShippingTo") = "0"
            row("ShippingValue") = "0"
            dt.Rows.Add(row)
        Next
        rptRanges.DataSource = dt
        rptRanges.DataBind()
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Try
            DB.BeginTransaction()
            For Each item As RepeaterItem In rptRanges.Items
                Dim ShippingFrom As Double = CType(item.FindControl("txtFrom"), TextBox).Text
                Dim ShippingTo As Double = CType(item.FindControl("txtTo"), TextBox).Text
                Dim ShippingValue As Double = CType(item.FindControl("txtValue"), TextBox).Text
                Dim RangeId As Integer = CType(item.FindControl("RangeId"), HiddenField).Value

                Dim dbRange As StoreShippingRangeRow = StoreShippingRangeRow.GetRow(DB, RangeId)
                dbRange.ShippingFrom = ShippingFrom
                dbRange.ShippingTo = ShippingTo
                dbRange.ShippingValue = ShippingValue
                If dbRange.RangeId = 0 Then
                    dbRange.Insert()
                Else
                    dbRange.Update()
                End If
            Next
            DB.CommitTransaction()

            Response.Redirect("/admin/settings/")

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
