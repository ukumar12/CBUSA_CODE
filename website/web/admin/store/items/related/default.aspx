<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="" CodeFile="default.aspx.vb" Inherits="related"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Related Items</h4>

<script type="text/javascript" src="/includes/XmlHttpLookup.js"></script>
<script type="text/javascript">
<!--
function MyCallback(ItemId) {
	document.getElementById('ItemId').value = ItemId;
	GetItemInfo();
}
if (window.addEventListener) {
	window.addEventListener('load', InitializeQuery, false);
} else if (window.attachEvent) {
	window.attachEvent('onload', InitializeQuery);
}

function InitializeQuery() {
	InitQueryCode('LookupField', '/ajax.aspx?f=DisplayItems&guid=' + Math.floor(Math.random() * 62) + '&q=', MyCallback);
}

function GetItemInfo() {
	var sItem, sConn;
	
	xml = getXMLHTTP();
	if(xml){
		xml.open("GET","/ajax.aspx?f=GetItemInfo&ItemId=" + getValue(document.getElementById('ItemId')) + '&guid=' + Math.floor(Math.random() * 62),true);
		xml.onreadystatechange = function() {
			if(xml.readyState==4 && xml.responseText) {
				if (xml.responseText.length > 0) {
					aData = xml.responseText.split('|');
					
					sItem = '';
					sItem += 'Item Number: ' + aData[0] + '<br>';
					sItem += 'Item: ' + aData[1];
										
					document.getElementById('ItemInfo').innerHTML = sItem;
				} else {
					sItem = '';
				}	
			}
		}	
		xml.send(null);
	}
	if (!isEmptyField(document.getElementById('ItemId'))) {
		document.getElementById('<%=btnAdd.ClientID%>').disabled = false;
	} else {
		document.getElementById('<%=btnAdd.ClientID%>').disabled = true;
	}
}	
//-->
</script>
	
<p>
Related Items for <b><% =dbStoreItem.ItemName%></b> | <a href="/admin/store/items/default.aspx?<%= GetPageParams(Components.FilterFieldType.All,"F_ItemId") %>">&laquo; Go Back To Store Item List</a>
</p>
	
<p><b>Add Related Item</b></p>

<table cellspacing="2" cellpadding="3" border="0">
<tr>
	<td class="optional" valign="top"><b>Item search</b></td>
	<td class="field" style="width:400px;">
		Please enter the first few characters of the item name that belongs to the 
		family/collection below<br />
		<input type="text" id="LookupField" name="LookupField" style="BORDER-RIGHT:#999999 1px solid; PADDING-RIGHT:4px; BORDER-TOP:#999999 1px solid; PADDING-LEFT:4px; BORDER-LEFT:#999999 1px solid; WIDTH:360px; BORDER-BOTTOM:#999999 1px solid">
		<input type="hidden" name="ItemId" id="ItemId">
		<p>
		<span id="ItemInfo" style="FONT-WEIGHT:bold; WIDTH:300px; COLOR:red; HEIGHT:40px"></span>
		<br />
		Please click the "Add Related Item" button below to add the item selected as a coordinating item
		<p>
		<CC:OneClickButton id="btnAdd" runat="server" Text="Add Related Item" cssClass="btn" Enabled="False" />
		</p>
	</td>
</tr>
</table>

<p></p>	
<b>View/Update Existing Related Items</b>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use arrows" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Related Item?" runat="server" NavigateUrl= '<%# "delete.aspx?ItemId=" & DataBinder.Eval(Container.DataItem, "ItemId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField DataField="SKU" HeaderText="Item #"></asp:BoundField>
		<asp:BoundField DataField="ItemName" HeaderText="Item Name"></asp:BoundField>
		<asp:ImageField SortExpression="IsActive" DataImageUrlField="IsActive" HeaderText="Is Active" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "move.aspx?ACTION=UP&UniqueId=" & DataBinder.Eval(Container.DataItem, "UniqueId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/moveup.gif" ID="lnkMoveUp">Move Up</asp:HyperLink>
		    </ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "move.aspx?ACTION=DOWN&UniqueId=" & DataBinder.Eval(Container.DataItem, "UniqueId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/movedown.gif" ID="lnkMoveDown">Move Down</asp:HyperLink>
		    </ItemTemplate>
		</asp:TemplateField>
	</Columns>
</CC:GridView>

</asp:content>