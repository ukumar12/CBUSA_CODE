Imports Components
Imports DataLayer

Partial Class comparison_special_order
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindData()
        End If
    End Sub

    Private Sub BindData()
        Dim dt As DataTable = VendorRow.GetVendorPriceRequests(DB, Session("VendorId"))
        rptRows.DataSource = dt
        rptRows.DataBind()
    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        If Not Page.IsValid Then Exit Sub

        For Each item As RepeaterItem In rptRows.Items
            Dim dbPrice As New VendorSpecialOrderProductPriceRow(DB, Session("VendorId"), DirectCast(item.FindControl("hdnID"), HiddenField).Value)
            dbPrice.VendorSKU = DirectCast(item.FindControl("txtSku"), TextBox).Text
            dbPrice.VendorPrice = DirectCast(item.FindControl("txtPrice"), TextBox).Text
            dbPrice.Insert()
        Next
    End Sub
End Class
