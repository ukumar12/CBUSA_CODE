<%@ Page Language="VB" MasterPageFile="~/controls/AdminMaster.master" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="index" title="Content Tool Module" %>

<asp:Content ID="Content" ContentPlaceHolderID="ph" Runat="Server">

<H4><FONT face="Arial">Content Tool Module &nbsp;Administration</FONT></H4>

<P>
<CC:OneClickButton id="AddNew" Text="Add New Module" Runat="server" cssClass="btn"></CC:OneClickButton>
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
				<asp:HyperLink id=Hyperlink1 runat="server" ImageUrl="/images/admin/edit.gif" NavigateUrl='<%# "edit.aspx?ModuleId=" & DataBinder.Eval(Container.DataItem, "ModuleId") %>' enableviewstate="False">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<ItemTemplate>
                <CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this module?" runat="server" NavigateUrl= '<%# "delete.aspx?ModuleId=" & DataBinder.Eval(Container.DataItem, "ModuleId") %>' ImageUrl="/images/admin/delete.gif" ID="Confirmlink1">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<HeaderTemplate>
				Module Name
			</HeaderTemplate>
			<ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.Name") %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<HeaderTemplate>
			Argument(s)
			</HeaderTemplate>
			<ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.Args") %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<HeaderTemplate>
			Min. Width
			</HeaderTemplate>
			<ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.MinWidth") %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<HeaderTemplate>
			Max. Width
			</HeaderTemplate>
			<ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.MaxWidth") %>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<HeaderTemplate>
				Control URL
			</HeaderTemplate>
			<ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.ControlURL") %>
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
