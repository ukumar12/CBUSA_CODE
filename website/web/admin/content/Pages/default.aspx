<%@ Page Language="VB" MasterPageFile="~/controls/AdminMaster.master" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="index" title="Content Tool Template" %>

<asp:Content ID="Content" ContentPlaceHolderID="ph" Runat="Server">

<H4><FONT face="Arial">Website Page Administration</FONT></H4>

<table>
<tr>
    <th>Template</th>
    <td class="field"><asp:DropDownList ID=F_TemplateId runat=server></asp:DropDownList></td>
</tr><tr>
    <th>Section</th>
    <td class="field"><asp:DropDownList ID=F_SectionId runat=server></asp:DropDownList></td>
</tr><tr>
    <th>URL</th>
    <td class="field"><asp:textbox id="F_PageURL" runat="server" Width="300px"></asp:textbox>
</tr><tr>
    <th>Page Name</th>
    <td class="field"><asp:textbox id="F_Name" runat="server" Width="300px"></asp:textbox>
    &nbsp;
    <CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn"/>
    </td>
</tr>
</table>

<P>
<CC:OneClickButton id="btnRegister" Text="Register Existing Page" Runat="server" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton id="btnGenerate" Text="Generate New Page" Runat="server" cssClass="btn"></CC:OneClickButton>
</P>

<TABLE cellSpacing="0" cellPadding="0" border="0" id="tblList" runat="server">
<TR><TD>
	<asp:datagrid id="dgList" runat="server" AllowPaging="True" PageSize="20" AutoGenerateColumns="False" CellPadding="2" CellSpacing="2" BorderWidth="0px">
	<AlternatingItemStyle CssClass="alternate"></AlternatingItemStyle>
	<ItemStyle CssClass="row" VerticalAlign=top></ItemStyle>
	<HeaderStyle CssClass="header"></HeaderStyle>
	<Columns>
		<asp:TemplateColumn>
			<ItemTemplate>
				<asp:HyperLink id=Hyperlink1 runat="server" ImageUrl="/images/admin/edit.gif" NavigateUrl='<%# "/admin/content/edit.aspx?PageId=" & DataBinder.Eval(Container.DataItem, "PageId") %>' enableviewstate="False">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateColumn>
        <asp:TemplateColumn>
            <ItemTemplate>
                <asp:HyperLink id=Hyperlink1 runat="server" ImageUrl="/images/admin/delete.gif" NavigateUrl='<%# "delete.aspx?PageId=" & DataBinder.Eval(Container.DataItem, "PageId") & "&" & params %>' enableviewstate="False" Visible='<%# not DataBinder.Eval(Container.DataItem, "IsPermanent") %>' Enabled="false">Delete</asp:HyperLink>                
            </ItemTemplate>
        </asp:TemplateColumn>
		<asp:TemplateColumn>
			<HeaderTemplate>
				Name/URL
			</HeaderTemplate>
			<ItemTemplate>
				<b><%# DataBinder.Eval(Container, "DataItem.Name") %></b><br />
				<a target=_blank href="<%# DataBinder.Eval(Container, "DataItem.PageURL") %>"><%# DataBinder.Eval(Container, "DataItem.PageURL") %></a>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<HeaderTemplate>
				Section Name
			</HeaderTemplate>
			<ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.SectionName") %><br />
				<a href="/admin/content/edit.aspx?TemplateId=<%# DataBinder.Eval(Container, "DataItem.TemplateId") %>&SectionId=<%# DataBinder.Eval(Container, "DataItem.SectionId") %>">Edit Section</a>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<HeaderTemplate>
				Template
			</HeaderTemplate>
			<ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.TemplateName") %><br>
				<a href="/admin/content/edit.aspx?TemplateId=<%# DataBinder.Eval(Container, "DataItem.TemplateId") %>">Edit Template</a>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<HeaderTemplate>
				Last Modified
			</HeaderTemplate>
			<ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.ModifyDate") %>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
	<PagerStyle Visible="False"></PagerStyle>
</asp:datagrid>
</TD>
</TR><TR>
<TD>
	<CC:NAVIGATOR id="myNavigator" runat="server"></CC:NAVIGATOR>
</TD>
</TR>
</TABLE>

<p>
<asp:placeholder id="plcNoRecords" runat="server" visible="false">There are no records that mach the search criteria</asp:placeholder>
</p>

</asp:Content>
