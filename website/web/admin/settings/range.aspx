<%@ Page Language="VB" MaintainScrollPositionOnPostback="True" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="System Parameters" CodeFile="range.aspx.vb" Inherits="range"  %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Shipping based on purchase range</h4>

<p>Please define range values used to calculate shipping amount based on purchase price.  </p>

<table cellSpacing="2" cellPadding="3" border="0">
	<asp:repeater enableviewstate="True" id="rptRanges" Runat="server">
        <HeaderTemplate>
		<tr><th>From</th><th>To</th><th>Value</th></tr>
        </HeaderTemplate>
		<ItemTemplate>
			<tr>
			<td><asp:TextBox Width="100" ID="txtFrom" runat="server" MaxLength="3" Text='<%# Math.Round(DataBinder.Eval(Container.DataItem, "ShippingFrom"),2)%>'></asp:TextBox> [float]</td>
			<td><asp:TextBox Width="100" ID="txtTo" runat="server" MaxLength="3" Text='<%# Math.Round(DataBinder.Eval(Container.DataItem, "ShippingTo"),2)%>'></asp:TextBox> [float]</td>
			<td><asp:TextBox Width="100" ID="txtValue" runat="server" MaxLength="5" Text='<%# Math.Round(DataBinder.Eval(Container.DataItem, "ShippingValue"),2)%>'></asp:TextBox> [float]
			<asp:HiddenField id="RangeId" runat="server"  Value='<%# DataBinder.Eval(Container.DataItem, "RangeId")%>' />
			</td>
			</tr>
		</ItemTemplate>
	</asp:repeater>
</table>
<CC:OneClickButton ID="btnSave" Text="Save" runat="server" CssClass="btn" />
<input class="btn" value="<< Go Back" type="button" onclick="document.location.href='/admin/settings/default.aspx'" >

</asp:content>