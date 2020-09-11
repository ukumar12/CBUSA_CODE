<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Rating And Comments Data" CodeFile="displayBuilderComments.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Ratings and Comments</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Vendor:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_VendorID" runat="server" /></td>
</tr>
<tr>
<td align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<%--<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />--%>
 <input class="btn" type="reset" value="Clear"  />
</td>
</tr>
</table>
</asp:Panel>
<p></p>

<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		 
		
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove Comments and Ratings?" runat="server" NavigateUrl= '<%# "deleteBuilderComments.aspx?BuilderID=" & DataBinder.Eval(Container.DataItem, "BuilderID") & "&VendorID=" &  DataBinder.Eval(Container.DataItem, "VendorID")  %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		
	
      <asp:BoundField SortExpression="Vendor" DataField="Vendor" HeaderText="Vendor"></asp:BoundField>


      <asp:BoundField SortExpression="Comments" DataField="Comment" HeaderText="Comments  "></asp:BoundField>
      
	
		
	</Columns>
</CC:GridView>

</asp:content>

