<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ratings.aspx.vb" Inherits="vendor_ratings" %>
<%@Register TagName="StarRatingDisplay" TagPrefix="CC" Src="~/controls/StarRatingDisplay.ascx" %>

<CT:MasterPage ID="CTMain" runat="server">
<%--<asp:button id ="btnGoToDashBoard" text="Go to DashBoard" class="btnred" runat="server" style="margin-bottom:10px;"/>--%>
    <div class="pckggraywrpr">
        <table cellpadding="0" cellspacing="5" border="0" style="width:100%;">
            <tr valign="top">
                <td width="40%">
                    <div class="pckghdgred">Overall Rating</div>
                    <CC:StarRatingDisplay id="ctlOverallRating" runat="server" />
                </td>
                <td width="60%">
                    <div class="pckghdgred">Builder Ratings &amp; Comments</div>
                    <table cellpadding="5" cellspacing="0" border="0" style="width:100%;">
                    <asp:Repeater id="rptComments" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td colspan="2" class="bold center"><%#DataBinder.Eval(Container.DataItem, "CompanyName")%> (<%#DataBinder.Eval(Container.DataItem, "Submitted")%>)</td>
                            </tr>
                            <tr valign="top">
                                <td width="50%"><asp:Literal id="ltlRatings" runat="server"></asp:Literal></td>
                                <td width="50%"><%#DataBinder.Eval(Container.DataItem, "Comment")%></td>
                            </tr>
                        </ItemTemplate>
                        <SeparatorTemplate>
                            <tr><td colspan="2" style="height:1px;background-color:#333;padding:0px;"><img src="/images/spacer.gif" style="height:1px;border-style:none;" /></td></tr>
                        </SeparatorTemplate>
                    </asp:Repeater>
                        <tr>
                            <td colspan="2" style="padding-top:20px;">
                                <CC:Navigator ID="ctlNavigator" runat="server" PagerSize="5" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        
    </div>
</CT:MasterPage>