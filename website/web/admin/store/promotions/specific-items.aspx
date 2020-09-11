<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Store Promotion" CodeFile="specific-items.aspx.vb" Inherits="SpecificItems"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4><asp:Literal ID="ltlTitle" runat="server" /></h4>
<a href="/admin/store/promotions/default.aspx?<%= GetPageParams(Components.FilterFieldType.All,"F_SortBy;F_SortOrder") %>">&laquo; Go Back To Promotion List</a>

<script language="JavaScript" src="/includes/XmlHttpLookup.js"></script>
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
	InitQueryCode('LookupField', '/ajax.aspx?f=DisplayItemsAndDepartments&q=', MyCallback);
}

function GetItemInfo() {
	var sItem, sConn;
	xml = getXMLHTTP();
	if(xml){
		var temp = new Array();
		temp = document.getElementById('ItemId').value.split('-');
		if (temp[1] == 'Item') {
			xml.open("GET","/ajax.aspx?f=GetItemInfo&ItemId=" + temp[0],true);
			xml.onreadystatechange = function() {
				if(xml.readyState==4 && xml.responseText) {
					if (xml.responseText.length > 0) {
						aData = xml.responseText.split('|');

						sItem = '';
						sItem += 'SKU: ' + aData[0] + '<br>';
						sItem += 'Item: ' + aData[1];
						document.getElementById('ItemInfo').innerHTML = sItem;
					} else {
						sItem = '';
					}
				}
			}
			xml.send(null);

		} else if (temp[1] == 'Department') {
			xml.open("GET","/ajax.aspx?f=GetDepartmentInfo&DepartmentId=" + temp[0],true);
			xml.onreadystatechange = function() {
				if(xml.readyState==4 && xml.responseText) {
					if (xml.responseText.length > 0) {
						aData = xml.responseText.split('|');
						sItem = '';
						sItem += 'Department: ' + aData[1] + '<br>';
						document.getElementById('ItemInfo').innerHTML = sItem;
					} else {
						sItem = '';
					}
				}
			}
			xml.send(null);
		}
	}

	if (!isEmptyField(document.getElementById('ItemId'))) {
		document.getElementById('<%=btnAdd.ClientID%>').disabled = false;
	} else {
		document.getElementById('<%=btnAdd.ClientID%>').disabled = true;
	}
}
//-->
</script>
	
<p><b>Add Specific Promotion Items/Departments</b></p>

<table cellspacing="2" cellpadding="3" border="0">
<tr>
	<td class="optional" valign="top"><b>Item/Department search</b></td>
	<td class="field" style="width:400px;">
		Please enter the first few characters of the item name that belongs to the 
		family/collection below<br />
		<input type="text" id="LookupField" name="LookupField" style="BORDER-RIGHT:#999999 1px solid; PADDING-RIGHT:4px; BORDER-TOP:#999999 1px solid; PADDING-LEFT:4px; BORDER-LEFT:#999999 1px solid; WIDTH:360px; BORDER-BOTTOM:#999999 1px solid">
		<input type="hidden" name="ItemId" id="ItemId">
		<p>
		<span id="ItemInfo" style="FONT-WEIGHT:bold; WIDTH:300px; COLOR:red; HEIGHT:40px"></span>
		<br />
		Please click the "Add Item" button below to add the item selected as a coordinating item
		<p>
		<CC:OneClickButton id="btnAdd" runat="server" Text="Add Item" cssClass="btn" Enabled="False" />
		</p>
	</td>
</tr>
</table>

<p></p>	
<b>View/Update Existing Specific Items/Departments</b>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use arrows" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Item?" runat="server" NavigateUrl= '<%# "specific-delete.aspx?Id=" & DataBinder.Eval(Container.DataItem, "Id") & "&PromotionId=" & DataBinder.Eval(Container.DataItem, "PromotionId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="ItemName" DataField="ItemName" HeaderText="ItemName"></asp:BoundField>
		<asp:BoundField SortExpression="ItemType" DataField="ItemType" HeaderText="ItemType"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>
