<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MailingSummary.ascx.vb" Inherits="MailingSummary" %>

<table border="0">
<tr><td valign="top">
<b>E-mail Summary</b>
<table border="0" cellspacing="2" cellpadding="3">
<tr>
	<td class="required" style="width:160px">E-mail Name:</td>
	<td class="field" valign="top"><asp:Literal ID="ltlName" runat="server" /></td>
</tr>
<tr>
	<td class="required"><b>E-mail format:</b></td>
	<td class="field"><asp:Literal ID="ltlFormat" runat="server" /></td>
</tr>
<tr>
	<td class="required"><b>From E-mail:</b></td>
	<td class="field"><asp:Literal ID="ltlFromEmail" runat="server" /></td>
</tr>
<tr>
	<td class="required"><b>From Name:</b></td>
	<td class="field"><asp:Literal ID="ltlFromName" runat="server" /></td>
</tr>
<tr>
	<td class="required"><b>Reply-To Email:</b></td>
	<td class="field"><asp:Literal ID="ltlReplyTo" runat="server" /></td>
</tr>
<tr>
	<td class="required"><b>Subject:</b></td>
	<td class="field"><asp:Literal ID="ltlSubject" runat="server" /></td>
</tr>
<tr>
	<td class="required"><b>Recipient type:</b></td>
	<td class="field"><asp:Literal ID="ltlTargetType" runat="server" /></td>
</tr>
<tr><td>&nbsp;</td></tr>
<tr>
<td colspan="2">
<asp:DataList runat="server" id="dlSlots" RepeatDirection="Horizontal" CellPadding="0" CellSpacing="0">
<ItemTemplate>
<div id="divWithText" runat="server"><a href="" onclick="return false;" class="S3">Slot #<%#Container.ItemIndex+1 %></a></div>
<div id="divNoText" runat="server"><a href="" onclick="return false;" class="S2"><strike>Slot #<%#Container.ItemIndex+1%></strike></a></div>
</ItemTemplate>
</asp:DataList>
<span class="smallest">&nbsp;slots that are gray are blank</span>

</td>
</tr>
</table>

<div style="margin-top:15px">
<%  If Mode = "Edit" Then%>
<CC:OneClickButton id="btnLayout" CausesValidation="False" Runat="server" Text="Modify Layout" cssClass="btn"></CC:OneClickButton>&nbsp;
<% end if %>
<CC:OneClickButton id="btnPreview" CausesValidation="False" Runat="server" Text="Preview" cssClass="btn"></CC:OneClickButton>
</div>

</td><td valign="top">
&nbsp;<br />
<img id="img" runat="server" src="" alt="Template Image" />
</td>
</tr>
</table>

<div style="margin-top:10px">
<b>Defined Search Criteria</b>
<table border="0" cellspacing="2" cellpadding="2">
	<tr>
		<td class="required" style="width:160px">List(s):</td>
		<td class="field"><asp:Literal ID="ltlLists" runat="server" /></td>
	</tr>
	<tr>
	    <td class="optional" style="width:160px">Subscription Date</td>
		<td class="field"><asp:Literal ID="ltlSubscriptionDate" runat="server" /></td>
	</tr>
</table>
</div>

<% if IsHTML %>
<% if Mode = "View" Then %>
<h4>Email has been targeted to <span class="red"><asp:Literal ID="ltlHTMLRecipientsView" runat="server" /></span> "HTML" recipient(s).</h4>
<h4>So far, <span class="red"><asp:Literal ID="ltlHTMLOpen" runat="server" /></span> unique recipients have read the HTML version</h4>
<% else %>
<h4>There is currently <span class="red"><asp:Literal ID="ltlHTMLRecipientsEdit" runat="server" /></span> "HTML" recipient(s) in database that match your search criteria.</h4>
<% end if%>
<% end if %>

<% if IsText %>
<% if Mode = "View" Then %>
<h4>Email has been targeted to <span class="red"><asp:Literal ID="ltlTextRecipientsView" runat="server" /></span> "Plain Text" recipient(s).</h4>
<% else %>
<h4>There is currently <span class="red"><asp:Literal ID="ltlTextRecipientsEdit" runat="server" /></span> "Plain Text" recipient(s) in database that match your search criteria.</h4>
<% end if%>
<% end if %>

<%  If Mode = "Edit" Then%>
<div style="margin-top:10px">
<CC:OneClickButton CausesValidation="False" ID="btnModify" Cssclass="btn" Text="Modify Search Criteria" runat="server" />
</div>
<% end if %>

<%  If Mode = "View" Then%>

<hr size="1">

<h4>Delivery Statistics</h4>

<% if IsHTML %>
<div style="margin-top:10px;"><b>HTML Message</b>
<asp:Literal ID="ltlHTML" runat="server"></asp:Literal>
</div>
<CC:GridView id="gvHTML" CellSpacing="2" CellPadding="2" runat="server" AllowPaging="False" AllowSorting="False" HeaderText="" EmptyDataText="There are no statistics yet. Please check again later." AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" EnableViestate="False">
	<Columns>
		<asp:BoundField DataField="ID_SuccessCount_" HeaderText="OK"></asp:BoundField>
		<asp:TemplateField>
		    <headertemplate>OK%</headertemplate>
			<ItemTemplate>
			<asp:Literal id="ltlOKPerc" runat="Server"></asp:Literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField DataField="ID_TransCount_" HeaderText="Soft"></asp:BoundField>
		<asp:TemplateField>
		    <headertemplate>Soft%</headertemplate>
			<ItemTemplate>
			<asp:Literal id="ltlSoftPerc" runat="Server"></asp:Literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField DataField="ID_UnreachCount_" HeaderText="Hard"></asp:BoundField>
		<asp:TemplateField>
		    <headertemplate>Hard%</headertemplate>
			<ItemTemplate>
			<asp:Literal id="ltlHardPerc" runat="Server"></asp:Literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField DataField="SendDate_" HeaderText="Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField DataField="Status_" HeaderText="Status"></asp:BoundField>
		<asp:TemplateField>
		    <headertemplate>Total</headertemplate>
			<ItemTemplate><asp:Literal id="ltlRecipients" runat="server"></asp:Literal></ItemTemplate>
		</asp:TemplateField>
	</Columns>
	<FooterStyle CssClass="row bold" />
</CC:GridView>
<% end if %>

<% if IsText %>
<div style="margin-top:10px;"><b>Text Message</b>
<asp:Literal ID="ltlText" runat="server"></asp:Literal>
</div>
<CC:GridView id="gvText" CellSpacing="2" CellPadding="2" runat="server" AllowPaging="False" AllowSorting="False" HeaderText="" EmptyDataText="There are no statistics yet. Please check again later." AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom"  Enableviewstate="False">
	<Columns>
		<asp:BoundField DataField="ID_SuccessCount_" HeaderText="OK"></asp:BoundField>
		<asp:TemplateField>
		    <headertemplate>OK%</headertemplate>
			<ItemTemplate>
			<asp:Literal id="ltlOKPerc" runat="Server"></asp:Literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField DataField="ID_TransCount_" HeaderText="Soft"></asp:BoundField>
		<asp:TemplateField>
		    <headertemplate>Soft%</headertemplate>
			<ItemTemplate>
			<asp:Literal id="ltlSoftPerc" runat="Server"></asp:Literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField DataField="ID_UnreachCount_" HeaderText="Hard"></asp:BoundField>
		<asp:TemplateField>
		    <headertemplate>Hard%</headertemplate>
			<ItemTemplate>
			<asp:Literal id="ltlHardPerc" runat="Server"></asp:Literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField DataField="SendDate_" HeaderText="Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField DataField="Status_" HeaderText="Status"></asp:BoundField>
		<asp:TemplateField>
		    <headertemplate>Total</headertemplate>
			<ItemTemplate><asp:Literal id="ltlRecipients" runat="server"></asp:Literal></ItemTemplate>
		</asp:TemplateField>
	</Columns>
	<FooterStyle CssClass="row bold" />
</CC:GridView>
<% end if %>

<div style="margin-top:10px;"><b>Link Tracking</b></div>
<CC:GridView id="gvLinks" CellSpacing="2" CellPadding="2" runat="server" AllowPaging="False" AllowSorting="False" HeaderText="" EmptyDataText="There are no link tracking statistics yet. Please check again later." AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate"></AlternatingRowStyle>
	<RowStyle CssClass="row"></RowStyle>
	<Columns>
		<asp:BoundField DataField="MimeType" HeaderText="Message Type"></asp:BoundField>
		<asp:BoundField DataField="Clicked" HeaderText="LinksClicked"></asp:BoundField>
		<asp:TemplateField>
			<ItemTemplate>
			<a href="links.aspx?MessageId=<%# DataBinder.Eval(Container.DataItem, "MessageId")%>&F_MimeType=<%# DataBinder.Eval(Container.DataItem, "MimeType")%>">View Details</a>
			</ItemTemplate>
		</asp:TemplateField>
	
	</Columns>
</CC:GridView>


<% end if %>
