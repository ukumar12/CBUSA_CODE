<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Vendor Account" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<script type="text/javascript">
    function ChangeTarget() {
        document.forms[0].target = '_blank';
        window.setTimeout('document.forms[0].target="_self"', 1000);
    }
</script>

<h4>Vendor Account Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Historic ID:</th>
<td valign="top" class="field"><asp:textbox id="F_HistoricId" runat="server" Columns="50" MaxLength="5" TextMode="Number"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Vendor ID:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_VendorID" runat="server" /></td>
</tr>
<tr>
<th valign="top">Last Name:</th>
<td valign="top" class="field"><asp:textbox id="F_LastName" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Username:</th>
<td valign="top" class="field"><asp:textbox id="F_Username" runat="server" Columns="20" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Is Primary:</b></th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_IsPrimary" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Value="1">Yes</asp:ListItem>
		<asp:ListItem Value="0">No</asp:ListItem>
	</asp:DropDownList>
</td>
</tr>
<tr>
<th valign="top"><b>Is Active:</b></th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_IsActive" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Selected ="True"  Value="1">Yes</asp:ListItem>
		<asp:ListItem Value="0">No</asp:ListItem>
	</asp:DropDownList>
</td>
</tr>
<tr>
<th valign="top"><b>Created:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_CreatedLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_CreatedUbound" runat="server" /></td>
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
<tr>
<td colspan="2" align ="right"> 
<asp:button ID="btnExport" runat="server" Text="Export " CssClass="btn" />
</td>
</tr>

</table>
</asp:Panel>
<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Vendor Account" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" 
HeaderText="In order to change display order, please use header links" 
EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" DataKeyNames ="VendorID">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
		    <ItemTemplate>
		        <asp:Button ID="btnLogin" runat="server" Text="Login As Vendor" CssClass="btn" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"VendorAccountId") %>' OnClientClick="ChangeTarget()" />
		    </ItemTemplate>
		</asp:TemplateField>	
		<asp:TemplateField>
		    <ItemTemplate>
		        <asp:Button ID="btnLoginNoReg" runat="server" Text="Bypass Registration" CssClass="btn" CommandName="LoginNoReg" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"VendorAccountID") %>' OnClientClick="ChangeTarget()" />
		    </ItemTemplate>
		</asp:TemplateField>		
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?VendorAccountID=" & DataBinder.Eval(Container.DataItem, "VendorAccountID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to Inactivate this Vendor Account?" runat="server" NavigateUrl= '<%# "delete.aspx?VendorAccountID=" & DataBinder.Eval(Container.DataItem, "VendorAccountID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="VendorID" DataField="VendorID" HeaderText="Vendor I D"></asp:BoundField>
		
          <asp:BoundField SortExpression="VendorName"   DataField="VendorName" HeaderText="Company Name"></asp:BoundField>
           <asp:BoundField  SortExpression="FirstName"  DataField="FirstName" HeaderText="First Name"></asp:BoundField>
		<asp:BoundField SortExpression="LastName"  DataField="LastName" HeaderText="Last Name"></asp:BoundField>
        <asp:TemplateField HeaderText = "Market(LLC)" >
        <ItemTemplate >
        <asp:Literal ID="LLCName" runat ="server" ></asp:Literal>
        </ItemTemplate>
        </asp:TemplateField>
		<asp:BoundField DataField="Phone" HeaderText="Phone"></asp:BoundField>
		<asp:BoundField DataField="Email" HeaderText="Email"></asp:BoundField>
		 
		<asp:BoundField  SortExpression="Created"  DataField="Created" HeaderText="Created" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		 
		<asp:ImageField   DataImageUrlField="IsPrimary" HeaderText="Is Primary" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
		<asp:ImageField  DataImageUrlField="IsActive" HeaderText="Is Active" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
	</Columns>
</CC:GridView>

</asp:content>

