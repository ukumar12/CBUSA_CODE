Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("MARKET_INDICATORS")

        If Not IsPostBack Then

            If DataLayer.BuilderPricingInformationRow.GetList(Me.DB).Rows.Count = 1 Then
                Me.divBuilderInfo.InnerHtml = DataLayer.BuilderPricingInformationRow.GetList(Me.DB).Rows(0).Item("PricingInformation").ToString
            Else
                Me.divBuilderInfo.InnerHtml = "N/A"
            End If

        End If
    End Sub

End Class
