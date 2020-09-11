<%@ Control Language="VB" AutoEventWireup="false" CodeFile="StoreItemAttributes.ascx.vb" Inherits="StoreItemAttributes" %>
<%@ Reference Control="~/controls/Attributes/Attribute.ascx" %>
<%@ Register TagName="StoreItemEmailMe" TagPrefix="CC" Src="~/controls/StoreItemEmailMe.ascx" %>

<asp:MultiView runat="server" ID="mv">
	<asp:View runat="server" ID="TableLayout">
		<tr>
			<th style="width:64%;">Product Information</th>
			<th style="width:18%;" class="center">Price</th>
			<th style="width:18%;" class="center">Quantity</th>
		</tr>
		<asp:Repeater runat="server" ID="rpt">
			<ItemTemplate>
				<asp:Label runat="server" Visible="false" ID="lblIds" Text='<%#Container.DataItem("ItemAttributeIds")%>' />
				<asp:Label runat="server" Visible="false" ID="lblPersistedView" />
				<tr>
					<td>
						<table>
							<tr>
								<td style="width: 75px;" class="center">
									<%#IIf(IsDBNull(Container.DataItem("ProductImage")), "", "<img src=""/assets/item/cart/" & Container.DataItem("ProductImage") & """ alt=""" & dbItem.ItemName & """ />")%><br />
								</td>
								<td>
									<%=dbItem.ItemName%><br />
									<span style="font-weight:normal">Item no: <%#Container.DataItem("SKU")%><br /><br />
									<%#Regex.Replace(Convert.ToString(Container.DataItem("Description")), vbCrLf, "<br />")%>
									</span>
								</td>
							</tr>
						</table>
					</td>
					<td class="center">
						<%#IIf(dbItem.IsOnSale, "<span class=""strike"">" & FormatCurrency(dbItem.Price + Container.DataItem("Price")) & "</span><br /><span class=""bold red"">" & FormatCurrency(dbItem.SalePrice + Container.DataItem("Price")) & "</span>", FormatCurrency(dbItem.Price + Container.DataItem("Price")))%>
					</td>
					<td class="center">
						<asp:MultiView runat="server" ID="mvInventory">
						<asp:View runat="server" ID="vAddToCart">
							<asp:Literal runat="server" ID="ltlBackorder" />
							<asp:TextBox runat="server" id="txtQty" size="3" maxlength="4" cssclass="qtybox" title="Quantity" /><br />
						</asp:View>
						<asp:View runat="server" ID="vInventory">
							<asp:Literal runat="server" ID="ltlInventory" />
							<CC:StoreItemEmailMe runat="server" id="ctrlEmailMe" />
						</asp:View>
						</asp:MultiView>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	</asp:View>
	<asp:View runat="server" ID="AdminDriven">
		<div style="margin-top:10px">
		Attributes<br />
		<table border="0" cellpadding="1" cellspacing="0">
		<asp:PlaceHolder runat="server" ID="phAttributes" />
		</table>
		</div>
	</asp:View>
</asp:MultiView>