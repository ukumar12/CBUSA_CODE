<%@ Page Language="VB" MasterPageFile="~/controls/AdminMaster.master" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="index" title="Content Tool Template" %>

<asp:Content ID="Content" ContentPlaceHolderID="ph" Runat="Server">

<H4><FONT face="Arial">Website Section &nbsp;Administration</FONT></H4>

<P>
<CC:OneClickButton id="AddNew" Text="Register New Section" Runat="server" cssClass="btn"></CC:OneClickButton>
</P>

<TABLE cellSpacing="0" cellPadding="0" border="0" id="tblList" runat="server">
<TR><TD>
	<asp:datagrid PageSize=50 id="dgList" runat="server" AllowPaging="True" AutoGenerateColumns="False" CellPadding="2" CellSpacing="2" BorderWidth="0px">
	<AlternatingItemStyle CssClass="alternate"></AlternatingItemStyle>
	<ItemStyle CssClass="row"></ItemStyle>
	<HeaderStyle CssClass="header"></HeaderStyle>
	<Columns>
		<asp:TemplateColumn>
			<ItemTemplate>
				<asp:HyperLink id="Hyperlink1" runat="server" ImageUrl="/images/admin/edit.gif" NavigateUrl='<%# "/admin/content/edit.aspx?TemplateId=" & DataBinder.Eval(Container.DataItem, "TemplateId") &  "&SectionId=" & DataBinder.Eval(Container.DataItem, "SectionId") %>' enableviewstate="False">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<HeaderTemplate>
				Section Name
			</HeaderTemplate>
			<ItemTemplate>
				<asp:Label id="Label1" enableviewstate="False" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SectionName") %>'/>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<HeaderTemplate>
			Folder/URL
			</HeaderTemplate>
			<ItemTemplate>
				<asp:Label enableviewstate="False" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Folder") %>' ID="Label2" />
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<HeaderTemplate>
				Template
			</HeaderTemplate>
			<ItemTemplate>
				<asp:Label enableviewstate=False runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.TemplateName") %>' ID="Label3" />
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
	<PagerStyle Visible="False"></PagerStyle>
</asp:datagrid>
</TD>
</TR><TR>
<TD>
	<CC:NAVIGATOR id="myNavigator" runat="server" PagerSize="10"></CC:NAVIGATOR>
</TD>
</TR>
</TABLE>

<p>
<asp:placeholder id="plcNoRecords" runat="server" visible="false">There are no records that mach the search criteria</asp:placeholder>
</p>

</asp:Content>
