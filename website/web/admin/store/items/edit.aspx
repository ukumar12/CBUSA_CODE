<%@ Page Language="VB" ValidateRequest="False" AutoEventWireup="false" CodeFile="edit.aspx.vb" EnableEventValidation="false" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Store Item"%>
<%@ Register TagPrefix="CC" TagName="StoreDepartmentTree" Src="~/controls/StoreDepartmentTree.ascx" %>
<%@ Register TagPrefix="CC" TagName="HelpMessage" Src="~/controls/HelpMessage.ascx" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<script type="text/javascript">
function ShowUploadFrame(divUploadId,frmUploadId,url) {
    var divUpload = document.getElementById(divUploadId);
    var frmUpload = document.getElementById(frmUploadId);
    divUpload.style.visibility="visible";
    frmUpload.style.height = '200px';
    frmUpload.src = url;
}
function HideUploadFrame(divUploadId,frmUploadId) {
    var divUpload = document.getElementById(divUploadId);
    var frmUpload = document.getElementById(frmUploadId);
    
    divUpload.style.visibility="hidden";
    frmUpload.style.height = '1px';
    frmUpload.src = "";
}
function SetImageValues(hdnImageNameId,hdnImageNameValue,divImageNameId,divImageNameValue) {

    var hdnImageName = document.getElementById(hdnImageNameId);
    var divImageName = document.getElementById(divImageNameId);
    hdnImageName.value = hdnImageNameValue;
    divImageName.innerHTML = divImageNameValue;
}
function refreshBackorder() {
	var tr = document.getElementById('trBackorder');
	if(!tr) return;
	document.getElementById('<%=drpInventoryAction.ClientID%>').value == 'Backorder' ? tr.style.display = '' : tr.style.display = 'none';
}

var ctx = 'Are you sure you want to remove this attribute and all child attributes?';
var cpy = 'Are you sure you want to copy these attributes and all children attributes?\nTHIS WILL REPLACE ALL CURRENT ATTRIBUTES!';
</script> 

<h4><% If ItemId = 0 Then %>Add<% Else %>Edit<% End If %> Store Item</h4>


<table border="0" cellpadding="0" cellspacing="0" width="0" id="tabstriptable">
	<tr>
		<td>
			<table border="0" cellpadding="0" cellspacing="0" id="tabstriptablehead">
				<tr>
					<td><a href="" id="lnkItem" class="tabstripOn" onclick="this.className='tabstripOn';document.getElementById('lnkAttributes').className='tabstripOff';document.getElementById('<%=divItem.ClientId%>').style.display='block';document.getElementById('<%=divAttributes.ClientId%>').style.display='none';return false;">Product Details</a></td>
					<td></td>
					<td><a href="" id="lnkAttributes" class="tabstripOff" onclick="this.className='tabstripOn';document.getElementById('lnkItem').className='tabstripOff';document.getElementById('<%=divItem.ClientId%>').style.display='none';document.getElementById('<%=divAttributes.ClientId%>').style.display='block';return false;">Product Attributes</a></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td colspan="2" id="tabstripcontent">

			<div id="divItem" runat="server">
				<h4>Product Details</h4>
				<table border="0" cellspacing="1" cellpadding="2">
					<tr><td colspan="2"><span class="smallest"><span class=red>red color</span> - denotes required fields</span></td></tr>
					<tr>
						<td class="required">Item Name:</td>
						<td class="field"><asp:textbox id="txtItemName" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
						<td><asp:RequiredFieldValidator ID="rfvItemName" runat="server" Display="Dynamic" ControlToValidate="txtItemName" ErrorMessage="Field 'Item Name' is blank"></asp:RequiredFieldValidator></td>
					</tr>
					<tr>
						<td class="required">SKU:</td>
											<td class="field"><asp:textbox id="txtSKU" runat="server" maxlength="20" columns="20" style="width: 139px;"></asp:textbox><div class="smaller">Required when item does not have attributes</div></td>
											<td><asp:RequiredFieldValidator EnableClientScript="false" ID="rfvSKU" runat="server" Display="Dynamic" ControlToValidate="txtSKU" ErrorMessage="Field 'SKU' is blank"></asp:RequiredFieldValidator></td>
					</tr>
					<tr>
						<td class="optional">Brand:</td>
						<td class="field"><asp:DropDownList id="drpBrandId" runat="server" /></td>
						<td></td>
					</tr>
					<tr>
						<td class="required">Price:</td>
						<td class="field"><asp:textbox id="txtPrice" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
						<td><asp:RequiredFieldValidator ID="rfvPrice" runat="server" Display="Dynamic" ControlToValidate="txtPrice" ErrorMessage="Field 'Price' is blank"></asp:RequiredFieldValidator></td>
					</tr>
					<tr>
						<td class="required"><b>Is On Sale?</b></td>
						<td class="field">
							<table>
							<tr>
								<td class="field"><asp:CheckBox runat="server" ID="chkIsOnSale" /></td>
								<td class="field" style="vertical-align:middle;">Sale Price:</td>
								<td class="field"><asp:textbox id="txtSalePrice" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
								<td></td>
							</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td class="optional"><b>Is Tax Exempt?</b></td>
						<td class="field"><asp:CheckBox runat="server" ID="chkIsTaxFree" /></td>
					</tr>
					<tr>
						<td class="optional"><b>Is Gift Wrap Available?</b></td>
						<td class="field"><asp:CheckBox runat="server" ID="chkIsGiftWrap" /></td>
					</tr>
					<tr>
						<td class="optional">Page Title:</td>
						<td class="field"><asp:textbox id="txtPageTitle" runat="server" maxlength="255" style="width: 600px;"></asp:textbox><br />
						<div class="smaller" style="margin-top:2px">Descriptive <b>Page Titles</b> can help your store's item pages rank higher in search engines like <b style="border:solid 1px #666666;background:#FFFFFF">&nbsp;<b style="color:#1111FF">G</b><b style="color:#FF1111">o</b><b style="color:#DDBB00">o</b><b style="color:#1111FF">g</b><b style="color:#00AD00">l</b><b style="color:#FF1111">e</b>&nbsp;</b> and <b style="border:solid 1px #666666;background:#FFFFFF">&nbsp;<b style="color:#FF0000">Yahoo!</b>&nbsp;</b></div>
						<CC:HelpMessage runat="server" HelpCode="TitleTag" ID="hlpTitle"/></td>
					</tr>
					<tr>
						<td class="optional">Meta Description:</td>
						<td class="field"><asp:textbox TextMode="MultiLine" Rows="3" id="txtMetaDescription" runat="server" maxlength="255" columns="50" style="width: 600px;"></asp:textbox><br />
    					<div class="smaller" style="margin-top:2px">Meta Tag Description may help your store's item pages rank higher in search engines like <b style="border:solid 1px #666666;background:#FFFFFF">&nbsp;<b style="color:#1111FF">G</b><b style="color:#FF1111">o</b><b style="color:#DDBB00">o</b><b style="color:#1111FF">g</b><b style="color:#00AD00">l</b><b style="color:#FF1111">e</b>&nbsp;</b> and <b style="border:solid 1px #666666;background:#FFFFFF">&nbsp;<b style="color:#FF0000">Yahoo!</b>&nbsp;</b></div>
    					<CC:HelpMessage runat="server" HelpCode="MetaDescription" ID="hlpMetaDescription"/>
						</td>
						<td></td>
					</tr>
					<tr>
						<td class="optional">Meta Keywords:</td>
						<td class="field"><asp:textbox TextMode="MultiLine" Rows="3" id="txtMetaKeywords" runat="server" maxlength="255" columns="50" style="width: 600px;"></asp:textbox><br />
    					<div class="smaller" style="margin-top:2px">Meta Tag Keywords may can help your store's item pages rank higher in search engines like <b style="border:solid 1px #666666;background:#FFFFFF">&nbsp;<b style="color:#1111FF">G</b><b style="color:#FF1111">o</b><b style="color:#DDBB00">o</b><b style="color:#1111FF">g</b><b style="color:#00AD00">l</b><b style="color:#FF1111">e</b>&nbsp;</b> and <b style="border:solid 1px #666666;background:#FFFFFF">&nbsp;<b style="color:#FF0000">Yahoo!</b>&nbsp;</b></div>
    					<CC:HelpMessage runat="server" HelpCode="MetaKeyWords" ID="hlpMetaKeyWords"/></td>
						<td></td>
					</tr>
					<tr>
						<td class="optional">Custom URL:</td>
						<td class="field"><asp:Textbox id="txtCustomURL" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:TextBox><br /><span class="smaller">Please begin url with "/" i.e. "/sample-url.aspx"</span><br />
                            <CC:HelpMessage runat="server" HelpCode="CustomUrl" ID="hlpCustomUrl"/></td>
						<td></td>
					</tr>		
					<tr>
						<td class="optional">Description:</td>
						<td class="field"><CC:CKEditor id="txtLongDescription" runat="server" Width="600" Height="300" /></td>
						<td></td>
					</tr>
					<tr>
						<td class="optional">Additional Features:</td>
						<td class="field"><CC:CheckBoxListEx ID="cblFeatures" runat="server" RepeatColumns="3"></CC:CheckBoxListEx></td>
					</tr>
					<tr>
						<td valign="top" class="required"><b>Departments:</b></td>
						<td class="field" style="width:485px">
							Please select below all departments that this item belongs to.<br />
							<CC:StoreDepartmentTree id="ctrlStoreDepartmentTree" runat="server" /></td>
					</tr>
					<tr>
						<td colspan="2" width="650">
							If you are uploading a large image, please be patient while we automatically
							generate the thumbnails for your image. This may take a few moments for large
							file sizes.
						</td>
					</tr>
					<tr>
						<td class="optional">Image:</td>
						<td class="field"><CC:FileUpload ID="fuImage" Folder="/assets/item/original/" ImageDisplayFolder="/assets/item/thumbnail/" DisplayImage="True" runat="server" style="width: 319px;" /></td>
						<td><CC:FileUploadExtensionValidator Extensions="jpg,jpeg,gif,bmp,png" ID="feImage" runat="server" Display="Dynamic" ControlToValidate="fuImage" ErrorMessage="Invalid file extension"></CC:FileUploadExtensionValidator></td>
					</tr>
					<tr>
						<td class="optional"><b>Weight<br />and Dimensions</b></td>
						<td class="field">
							<table>
							<tr>
							<td valign="top"><span style="font-size:10px;">Weight (pounds)</span><br /><asp:textbox id="txtWeight" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
							<td width="100" align="center"> - </td>
							<td valign="top"><span style="font-size:10px;">Width (inches)</span><br /><asp:textbox id="txtWidth" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
							<td valign="top"><span style="font-size:10px;">Height (inches)</span><br /><asp:textbox id="txtHeight" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
							<td valign="top"><span style="font-size:10px;">Thickness (inches)</span><br /><asp:textbox id="txtThickness" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
							</tr>
							</table>
						</td>
						<td>
						<CC:FloatValidator ID="fvWeight" runat="server" Display="Dynamic" ControlToValidate="txtWeight" ErrorMessage="Field 'Weight' is invalid"></CC:FloatValidator>
						<CC:FloatValidator ID="fvWidth" runat="server" Display="Dynamic" ControlToValidate="txtWidth" ErrorMessage="Field 'Width' is invalid"></CC:FloatValidator>
						<CC:FloatValidator ID="fvHeight" runat="server" Display="Dynamic" ControlToValidate="txtHeight" ErrorMessage="Field 'Height' is invalid"></CC:FloatValidator>
						<CC:FloatValidator ID="fvThickness" runat="server" Display="Dynamic" ControlToValidate="txtThickness" ErrorMessage="Field 'Thickness' is invalid"></CC:FloatValidator>

						</td>
					</tr>	
					<asp:PlaceHolder runat="server" id="phInventory">
					<tr>
						<td class="required">Inventory Qty:</td>
						<td class="field"><asp:textbox id="txtInventoryQty" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
						<td><asp:RequiredFieldValidator runat="server" ID="rfvInventoryQty" ControlToValidate="txtInventoryQty" ErrorMessage="Field 'Inventory Quantity' is required" Display="dynamic" /><CC:IntegerValidator runat="server" ControlToValidate="txtInventoryQty" ErrorMessage="Field 'Inventory Quantity' is invalid" Display="dynamic" /></td>
					</tr>
					<tr>
						<td class="optional">Inv Warning Lvl:</td>
						<td class="field"><asp:textbox id="txtInventoryWarningThreshold" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
						<td><CC:IntegerValidator runat="server" ControlToValidate="txtInventoryWarningThreshold" ErrorMessage="Field 'Inventory Warning Level' is invalid" Display="dynamic" /></td>
					</tr>
					<tr>
						<td class="optional">Inv Action Lvl:</td>
						<td class="field"><asp:textbox id="txtInventoryActionThreshold" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
						<td><CC:IntegerValidator runat="server" ControlToValidate="txtInventoryActionThreshold" ErrorMessage="Field 'Inventory Action Level' is invalid" Display="dynamic" /></td>
					</tr>
					<tr>
						<td class="required">Inventory Action:</td>
						<td class="field"><asp:DropDownList runat="server" ID="drpInventoryAction" onchange="refreshBackorder(this);"><asp:ListItem Text="Out of Stock" Value="OutOfStock" /><asp:ListItem Text="Disable" Value="Disable" /><asp:ListItem Text="Backorder" Value="Backorder" /></asp:DropDownList></td>
						<td></td>
					</tr>
					<tr id="trBackorder">
						<td class="optional">Backorder Date:</td>
						<td class="field"><CC:DatePicker runat="server" ID="dpBackorderDate" /></td>
						<td><CC:DateValidator runat="server" ControlToValidate="dpBackorderDate" ErrorMessage="Field 'Backorder Date' is invalid" Display="dynamic" /></td>
					</tr>
					</asp:PlaceHolder>
					<tr>
						<td class="optional" valign="top"><b>Shipping:</b></td>
						<td class="field" valign="top">
							<table><tr>
							<td valign=top><span style="font-size:10px;">Shipping 1st Item</span><br /><asp:TextBox ID="txtShipping1" Columns="8" maxlength="8" runat="server" /><td>
							<td valign=top><span style="font-size:10px;">Shipping 2nd Item</span><br /><asp:TextBox ID="txtShipping2" Columns="8" maxlength="8" runat="server" /><td>
							<td valign=top><span style="font-size:10px;">Country Unit</span><br /><asp:TextBox ID="txtCountryUnit" Columns="8" maxlength="8" runat="server" /><td>
							</tr></table>

						<CC:FloatValidator ID="fvShipping1" runat="server" Display="Dynamic" ControlToValidate="txtShipping1" ErrorMessage="Field 'Shipping for 1st item' is invalid"></CC:FloatValidator>
						<CC:FloatValidator ID="fvShipping2" runat="server" Display="Dynamic" ControlToValidate="txtShipping2" ErrorMessage="Field 'Shipping for 2nd item' is invalid"></CC:FloatValidator>
						<CC:IntegerValidator ID="ivCountryUnit" runat="server" Display="Dynamic" ControlToValidate="txtCountryUnit" ErrorMessage="Field 'Country Unit' is invalid"></CC:IntegerValidator>

						</td>
					</tr>
					<tr>
						<td class="optional">Item Unit:</td>
						<td class="field"><asp:textbox id="txtItemUnit" runat="server" maxlength="50" columns="50" style="width: 80px;"></asp:textbox></td>
						<td></td>
					</tr>
					<tr>
						<td class="optional">Short Description:</td>
						<td class="field"><asp:TextBox ID="txtShortDescription" runat="server" Width="600" Height="70" TextMode="Multiline" /></td>
						<td></td>
					</tr>
					<tr>
						<td class="optional"><b>Is Active?</b></td>
						<td class="field"><asp:CheckBox runat="server" ID="chkIsActive" /></td>
					</tr>
					<tr>
						<td class="optional"><b>Is Featured?</b></td>
						<td class="field"><asp:CheckBox runat="server" ID="chkIsFeatured" /></td>
					</tr>
				</table>
				<asp:RequiredFieldValidator ID="rqdrpTemplateId2" runat="server" Display="Dynamic" ControlToValidate="drpTemplateId" ErrorMessage="Field 'Template' in the 'Product Attributes' tab is blank"></asp:RequiredFieldValidator>
			</div>
				
			<div id="divAttributes" runat="server" style="display:none;">
				<h4>Product Attributes</h4>

<a name="aAttributes"></a>
<asp:ScriptManager ID="AjaxManager" runat="server" />
<asp:UpdatePanel ID="AjaxPanel" runat="server" UpdateMode="Conditional">
<Triggers>
</Triggers>
<ContentTemplate>

<table border="0" cellspacing="1" cellpadding="2">
<tr>
<td class="required">Template:</td>
<td class="field"><asp:DropDownList id="drpTemplateId" runat="server" AutoPostback="True" /> <asp:checkbox runat="server" ID="chkExpandActive" Text="Keep only active node(s) expanded" Checked="true" /></td>
<td><asp:RequiredFieldValidator ID="rqdrpTemplateId" runat="server" Display="Dynamic" ControlToValidate="drpTemplateId" ErrorMessage="Field 'Template' is blank"></asp:RequiredFieldValidator></td>
</tr>
<tr runat="server" id="trdm">
<td class="optional">Display Mode:</td>
<td class="field"><asp:DropDownList runat="server" ID="drpDisplayMode"><asp:ListItem Text="Default" Value="" /><asp:ListItem Text="Admin Driven" Value="AdminDriven" /><asp:ListItem Text="Table Layout" Value="TableLayout" /></asp:DropDownList></td>
</tr>
<tr>
<td class="optional">Attributes:</td>
<td class="field">

<asp:repeater id="ra" Runat="server">
<HeaderTemplate>
<table border="0">
</HeaderTemplate>
<ItemTemplate>
</ItemTemplate>
<FooterTemplate>
</table>
</FooterTemplate>
</asp:repeater>

</td>
<td></td>
</tr>
</table>

</ContentTemplate>
</asp:UpdatePanel>

			</div>

		</td>
	</tr>
</table>

<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn" CausesValidation="false"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Store Item?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

<script language="javascript">
<!--
refreshBackorder();
//-->
</script>

</asp:content>
