<%@ Control Language="VB" EnableViewState="False" AutoEventWireup="false" CodeFile="StarRatingDisplay.ascx.vb" Inherits="StarRatingDisplay" %>

<div style="text-align:center;">
    <table cellpadding="0" cellspacing="0" border="0" style="background-color:#fff;padding:15px;text-align:left;">
    <asp:Repeater id="rptStarDisplay" runat="server" EnableViewState="False">
    <HeaderTemplate>
        <tr>
            <td colspan="2" align="left" style="height: 20px;"><b>Ratings</b></td>
        </tr>
        <tr>
            <td>Category</td>
            <td colspan="2" style="white-space:nowrap;">Average Rating</td>
        </tr>
    </HeaderTemplate>
    <ItemTemplate>
    <tr>
        <td>
            <%#DataBinder.Eval(Container.DataItem, "RatingCategory")%>:
        </td>
        <td>
            <asp:Literal ID="ltlStars" runat="server"></asp:Literal>
        </td>
        <td>
            <asp:Literal ID="ltlAverage" runat="server"></asp:Literal>
        </td>
    </tr>
    </ItemTemplate>
    <FooterTemplate>
    </FooterTemplate>
    </asp:Repeater>
        <tr class="bold larger">
            <td>
                Overall Average:
            </td>
            <td><asp:Literal ID="ltlOverallStars" runat="server"></asp:Literal></td>
            <td><asp:Literal ID="ltlOverallAverage" runat="server"></asp:Literal></td>
        </tr>
        <tr>
            <td>
                Ratings: <span class="black"><asp:Literal ID="ltrRatings" runat="server" /></span>
            </td>
            <td>
                Comments: <span class="black"><asp:Literal ID="ltrComments" runat="server" /></span>
            </td>
        </tr>
    </table>
</div>