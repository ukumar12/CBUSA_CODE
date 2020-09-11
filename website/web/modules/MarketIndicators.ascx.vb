Imports Components
Imports DataLayer

Partial Class modules_MarketIndicators
    Inherits ModuleControl

    Private dvPrices As DataView

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindData()
        End If
    End Sub

    Private Sub BindData()
        Dim dtProducts As DataTable = BuilderIndicatorProductRow.GetBuilderRows(DB, Session("BuilderID"))
        If dtProducts.Rows.Count = 0 Then
            ltlNone.Text = "<p class=""bold center"">No Market Indicators have been selected.</p>"
        Else
            ltlNone.Text = String.Empty
        End If
        rptIndicators.DataSource = dtProducts
        rptIndicators.DataBind()
    End Sub

    Protected Sub rptIndicators_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptIndicators.ItemCommand
        Dim id As Integer = e.CommandArgument
        If id <> Nothing Then
            BuilderIndicatorProductRow.RemoveRow(DB, id)
        End If
        BindData()
    End Sub

    Protected Sub rptIndicators_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptIndicators.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        Dim ltlProduct As Literal = e.Item.FindControl("ltlProduct")
        Dim ltlHead As Literal = e.Item.FindControl("ltlVendorHead")
        Dim ltlPrices As Literal = e.Item.FindControl("ltlVendorPrices")

        ltlProduct.Text = Core.GetString(e.Item.DataItem("Product"))
        Dim dtPrices As DataTable = BuilderIndicatorProductRow.GetTopPrices(DB, e.Item.DataItem("BuilderIndicatorProductID"))
        For i As Integer = 0 To dtPrices.Rows.Count - 1
            ltlHead.Text &= "<th>" & Core.GetString(dtPrices.Rows(i)("Vendor")) & "</th>"

            Dim className As String = "norm"
            Dim pctChange As Double = Nothing
            If Core.GetDouble(dtPrices.Rows(i)("LastPrice")) > 0 AndAlso Core.GetDouble(dtPrices.Rows(i)("VendorPrice")) > 0 Then
                pctChange = Math.Round((dtPrices.Rows(i)("VendorPrice") - dtPrices.Rows(i)("LastPrice")) / dtPrices.Rows(i)("LastPrice") * 100, 1)
                If dtPrices.Rows(i)("LastPrice") < dtPrices.Rows(i)("VendorPrice") Then
                    className = "high"
                ElseIf dtPrices.Rows(i)("LastPrice") > dtPrices.Rows(i)("VendorPrice") Then
                    className = "low"
                End If
            End If
            If Core.GetBoolean(dtPrices.Rows(i)("IsSubstitution")) Then
                ltlPrices.Text &= "<td class=""" & className & """ style=""position:relative;"">" & FormatCurrency(Core.GetDouble(dtPrices.Rows(i)("VendorPrice"))) & "*" _
                    & "<br/><p class=""smaller center"">" & IIf(pctChange <> Nothing, pctChange & "% Change", "") _
                    & "<b onmouseover=""$('#SubDiv_" & e.Item.ItemIndex & "').slideDown('fast');"" onmouseout=""$('#SubDiv_" & e.Item.ItemIndex & "').slideUp('fast');"">* Substitute Product</b>" _
                    & "<div id=""SubDiv_" & e.Item.ItemIndex & """ style=""background-color:#fff;border:1px solid #ccc;padding:5px;position:absolute;left:0px;right:0px;top:80%;display:none;"">" & Core.GetString(dtPrices.Rows(i)("Product")) & "</div>" _
                    & "</p></td>"
            Else
                ltlPrices.Text &= "<td class=""" & className & """>" & FormatCurrency(Core.GetDouble(dtPrices.Rows(i)("VendorPrice")))
                If pctChange <> Nothing Then
                    ltlPrices.Text &= "<p class=""smaller center"">" & pctChange & "% Change</p>"
                End If
                ltlPrices.Text &= "</td>"
            End If

        Next
    End Sub

End Class
