<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Mailing Message" CodeFile="default.aspx.vb" Inherits="index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Broadcast E-mails</h4>

<span class="smaller">Please provide search criteria below</span>
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Sent Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_SentDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_SentDateUbound" runat="server" /></td>
</tr>
</table>
</td>
<td>
<CC:DateValidator ID="DateValidator1" runat="server" Display="Dynamic" ControlToValidate="F_SentDateLbound" ErrorMessage="Invalid 'From date'"></CC:DateValidator>
<CC:DateValidator ID="DateValidator2" runat="server" Display="Dynamic" ControlToValidate="F_SentDateUbound" ErrorMessage="Invalid 'To date'"></CC:DateValidator>
</td>
</tr>
<tr>
<th valign="top"><b>Template:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_TemplateId" runat="server" /></td>
</tr>
<tr>
<th valign="top">Recipient Type:</th>
<td valign="top" class="field">
<asp:DropDownList ID="F_TargetType" runat="server">
<asp:ListItem Value="">-- ALL --</asp:ListItem>
<asp:ListItem Value="DYNAMIC">Uploaded List(s)</asp:ListItem>
<asp:ListItem Value="MEMBER">Subscribers</asp:ListItem>
</asp:DropDownList>
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
Total # of emails sent: <b><asp:Label ID="lblTotal" runat="Server"></asp:Label></b>
<p></p>
<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0px" PagerSettings-Position="Bottom" ShowFooter="True" CausesValidation="True" SortImageAsc="/images/admin/asc3.gif" SortImageDesc="/images/admin/desc3.gif" SortOrder="ASC">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<FooterStyle CssClass="header" HorizontalAlign="Right" />
	<Columns>
		<asp:BoundField SortExpression="MessageId" DataField="MessageId" HeaderText="ID"></asp:BoundField>
		<asp:BoundField SortExpression="Name" DataField="Name" HeaderText="Name"></asp:BoundField>
		<asp:BoundField SortExpression="Subject" DataField="Subject" HeaderText="Subject"></asp:BoundField>
        <asp:BoundField SortExpression="SentDate" DataField="SentDate" HeaderText="Sent Date" DataFormatString="{0: MM/dd/yyyy &lt;br /&gt;hh:mm:ss tt}" HTMLEncode="False"></asp:BoundField>
        <asp:BoundField SortExpression="HTMLCount" DataField="HTMLCount" HeaderText="#HTML">
            <itemstyle horizontalalign="Right" />
        </asp:BoundField>
        <asp:BoundField SortExpression="TextCount" DataField="TextCount" HeaderText="# Text">
            <itemstyle horizontalalign="Right" />
        </asp:BoundField>
        <asp:BoundField SortExpression="TotalCount" DataField="TotalCount" HeaderText="# Total">
            <itemstyle horizontalalign="Right" />
        </asp:BoundField>
	</Columns>
    <HeaderStyle VerticalAlign="Top" />
</CC:GridView>

</asp:content>

