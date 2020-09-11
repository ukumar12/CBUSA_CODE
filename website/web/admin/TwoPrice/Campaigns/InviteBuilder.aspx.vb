Imports Components
Imports Controls
Imports DataLayer
Imports TwoPrice.DataLayer
Imports System.Linq
Imports System.Web.Services
Partial Class admin_TwoPrice_Campaigns_InviteBuilder
    Inherits AdminPage

    Protected TwoPriceCampaignId As Integer
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        CheckAccess("TWO_PRICE_CAMPAIGNS")
        TwoPriceCampaignId = Convert.ToInt32(Request("TwoPriceCampaignId"))
        If TwoPriceCampaignId = 0 Then
            AddError("A campaign is not selected.  Please select a Campaign from the admin.")
            Exit Sub
        End If
        If Not IsPostBack Then
            urIframe.Attributes.Add("src", "http://localhost:55767/EventManagement.aspx?TwoPriceCampaignId=" + TwoPriceCampaignId.ToString()) 'Local
            'urIframe.Attributes.Add("src", "https://dev.custombuilders-usa.com/cpmodule/EventManagement.aspx?TwoPriceCampaignId=" + TwoPriceCampaignId.ToString()) 'Dev
            'urIframe.Attributes.Add("src", "https://app.custombuilders-usa.com/cpmodule/EventManagement.aspx?TwoPriceCampaignId=" + TwoPriceCampaignId.ToString()) 'Live
        End If
    End Sub
End Class
