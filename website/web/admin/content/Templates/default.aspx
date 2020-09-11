<%@ Page Language="VB" MasterPageFile="~/controls/AdminMaster.master" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="index" title="Content Tool Template" %>

<asp:Content ID="Content" ContentPlaceHolderID="ph" Runat="Server">

<H4><FONT face="Arial">Template &nbsp;Administration</FONT></H4>

<table cellSpacing="0" cellPadding="0" border="0" id="tblList" runat="server">
<tr><td>
<asp:datagrid id="dgList" runat="server" AllowPaging="True" AutoGenerateColumns="False" CellPadding="2" CellSpacing="2" BorderWidth="0px">
<AlternatingItemStyle CssClass="alternate"></AlternatingItemStyle>
<ItemStyle CssClass="row"></ItemStyle>
<HeaderStyle CssClass="header"></HeaderStyle>
<Columns>
	<asp:TemplateColumn>
		<ItemTemplate>
			<asp:HyperLink id=Hyperlink1 runat="server" ImageUrl="/images/admin/edit.gif" NavigateUrl='<%# "/admin/content/edit.aspx?TemplateId=" & DataBinder.Eval(Container.DataItem, "TemplateId") %>' enableviewstate="False">Edit</asp:HyperLink>
		</ItemTemplate>
	</asp:TemplateColumn>
	<asp:TemplateColumn>
		<HeaderTemplate>
			<P>&nbsp;</P>
		</HeaderTemplate>
		<ItemTemplate>
			<asp:HyperLink id=Confirmlink1 runat="server" ImageUrl="/images/admin/preview.gif" NavigateUrl='<%# "edit.aspx?TemplateId=" & DataBinder.Eval(Container.DataItem, "TemplateId") & "&" & params %>' enableviewstate="False">HTML</asp:HyperLink>
		</ItemTemplate>
	</asp:TemplateColumn>
	<asp:TemplateColumn>
		<HeaderTemplate>
			Template Name
		</HeaderTemplate>
		<ItemTemplate>
			<asp:Label enableviewstate=False runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.TemplateName") %>' ID="Label1" />
		</ItemTemplate>
	</asp:TemplateColumn>
	<asp:TemplateColumn>
		<HeaderTemplate>
			Default?
		</HeaderTemplate>
		<ItemTemplate>
			<asp:Label enableviewstate=False runat="server" Text='<%# iif(DataBinder.Eval(Container, "DataItem.IsDefault"),"Yes","No") %>' ID="Label2" />
		</ItemTemplate>
	</asp:TemplateColumn>
	<asp:TemplateColumn>
		<HeaderTemplate>
			# of slots
		</HeaderTemplate>
		<ItemTemplate>
			<asp:Label enableviewstate=False runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.nofSlots") %>' ID="Label3" />
		</ItemTemplate>
	</asp:TemplateColumn>

</Columns>
<PagerStyle Visible="False"></PagerStyle>
</asp:datagrid>
</td>
</tr>
<tr>
	<TD><CC:NAVIGATOR id="myNavigator" runat="server" PagerSize="10"></CC:NAVIGATOR></TD>
</tr>
</table>

<p><asp:placeholder id="plcNoRecords" runat="server" visible="false">There are no records that mach the search criteria</asp:placeholder></p>

</asp:Content>