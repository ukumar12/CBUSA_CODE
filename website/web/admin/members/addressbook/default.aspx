<%@ Page Language="vb" AutoEventWireup="false" Inherits="member_addressbook_default" MasterPageFile="~/controls/AdminMaster.master" CodeFile="default.aspx.vb" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>View Address Book</h4><br />
Address Book Information for <asp:Label ID="txtMemberName" runat="server"></asp:Label>
|<a runat="server" id="lnkBack">&laquo; Go Back to Member Profile</a><br /><br />

<CC:OneClickButton id="btnAdd" runat="server" Text="Add New Address" CssClass="btn" /><br /><br />

<CC:GridView id="gvAddressBook" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" EmptyDataText="There are currently no entires in the address book" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
		    <ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?mode=Edit&MemberId="& MemberId & "&AddressId=" & DataBinder.Eval(Container.DataItem, "AddressId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Member?" runat="server" NavigateUrl= '<%# "delete.aspx?MemberId=" & MemberId & "&AddressId=" & DataBinder.Eval(Container.DataItem, "AddressId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
		    <HeaderTemplate>
		         Label
		    </HeaderTemplate>
		    <ItemTemplate>
		          <asp:literal id="ltlLabel" runat="server" />
		    </ItemTemplate>		
		</asp:TemplateField>
		<asp:TemplateField>
		    <HeaderTemplate>
		         Name
		    </HeaderTemplate>
		    <ItemTemplate>
		         <asp:literal id="ltlName" runat="server" />
		    </ItemTemplate>		
		</asp:TemplateField>
		 <asp:TemplateField>
		    <HeaderTemplate>
		         Address
		    </HeaderTemplate>
		    <ItemTemplate>
		         <asp:literal id="ltlAddress" runat="server" />
		    </ItemTemplate>		
		</asp:TemplateField>	
	</Columns>
</CC:GridView>
     
</asp:content>

