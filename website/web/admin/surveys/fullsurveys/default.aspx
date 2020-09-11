<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Surveys" CodeFile="default.aspx.vb" Inherits="admin_surveys_surveys_default"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Individual Survey Responses</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
    <tr>
    <th valign="top"><b>Submit Date:</b></th>
    <td valign="top" class="field">
	    <table border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td class="smaller">From <CC:DatePicker id="F_SubmitDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_SubmitDateUbound" runat="server" /></td>
            </tr>
        </table>
    </td>
    </tr>
<tr>
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
</table>
</asp:Panel>
<p></p>
<p></p>

<CC:GridView id="gvList" GridLines="none" CssClass="MultilineTable" CellSpacing="0" CellPadding="5" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "/admin/surveys/fullsurveys/details.aspx?ResponseId=" & DataBinder.Eval(Container.DataItem, "ResponseId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ID="lnkViewSurvey">View Survey</asp:HyperLink>		    
			</ItemTemplate>
		</asp:TemplateField>
		
		<asp:BoundField SortExpression="CompleteDate" DataField="CompleteDate" HeaderText="Complete Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="RemoteIP" DataField="RemoteIP" HeaderText="IP Address"></asp:BoundField>
		<%--<asp:BoundField SortExpression="OrderId" DataField="OrderId" HeaderText="Order ID"></asp:BoundField>--%>
	</Columns>
</CC:GridView>

</asp:content>
