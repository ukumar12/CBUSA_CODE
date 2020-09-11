<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Referral" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Referrals</h4>

<span class="smaller">Please provide search criteria below</span>
<table cellpadding="2" cellspacing="2">
<tr>
<th><b>Click Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_ClickDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_ClickDateUbound" runat="server" /></td>
</tr>
</table>
</td>
<td>
<CC:DateValidator runat="server" ID="dvClickDateLbound" Display="Dynamic" ControlToValidate="F_ClickDateLbound" ErrorMessage="Field 'Click Date From' is invalid" ></CC:DateValidator><br />
<CC:DateValidator runat="server" ID="dvClickDateUbound" Display="Dynamic" ControlToValidate="F_ClickDateUbound" ErrorMessage="Field 'Click Date To' is invalid" ></CC:DateValidator>
</td>
</tr>
<tr>
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
</table>
<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Referral" cssClass="btn"></CC:OneClickButton>
<p></p>

<table>
<tr><td>
<b>Usage:</b><br />
Please don't put the special string "?ad=..." directly after the "/" character.<br>
<strike><%=System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName")%>/?ad=12345</strike><span style="color:red;"> - incorrect</span><br />
<%=System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName")%>/<b>default.aspx</b>?ad=12345 is <b><span style="color:green;">correct</span></b>
</td></tr>
</table>

<p></p>
<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?ReferralId=" & DataBinder.Eval(Container.DataItem, "ReferralId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Referral?" runat="server" NavigateUrl= '<%# "delete.aspx?ReferralId=" & DataBinder.Eval(Container.DataItem, "ReferralId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
		    <HeaderStyle CssClass="smallest"/>
       	    <HeaderTemplate>
            Please add this text to the end of the link<br />that are you giving to the specific company	    
		    </HeaderTemplate>
			<ItemTemplate>
			<%=System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName")%>/<b>default.aspx</b>?ref=<%#DataBinder.Eval(Container.DataItem, "Code")%>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField DataField="Name" HeaderText="Name"></asp:BoundField>
		<asp:BoundField DataField="Howmany" HeaderText="Num Clicks"></asp:BoundField>
		<asp:BoundField DataField="NofOrders" HeaderText="Num Orders"></asp:BoundField>
		<asp:BoundField DataField="Total" HeaderText="Total [$]" DataFormatString="{0:c}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField DataField="AvgOrder" HeaderText="Avg. Order [$]" DataFormatString="{0:c}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField DataField="Conversion" HeaderText="Conversion [%]" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>

