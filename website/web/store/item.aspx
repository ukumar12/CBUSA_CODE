<%@ Page Language="VB" AutoEventWireup="false" CodeFile="item.aspx.vb" Inherits="store_item" %>
<%@ Register TagName="StoreRelatedItems" TagPrefix="CC" Src="~/controls/StoreRelatedItems.ascx" %>
<%@ Register TagName="StoreItemAttributes" TagPrefix="CC" Src="~/controls/StoreItemAttributes.ascx" %>
<%@ Register TagName="StoreItemEmailMe" TagPrefix="CC" Src="~/controls/StoreItemEmailMe.ascx" %>
<%@ Register TagName="StoreRecipientsDropDown" TagPrefix="CC" Src="~/controls/StoreRecipientsDropDown.ascx" %>
<%@ Register TagName="ShareAndEnjoy" TagPrefix="CC" Src="~/controls/ShareAndEnjoy.ascx" %>
<%@ Register TagName="PrintOrEmailPage" TagPrefix="CC" Src="~/controls/PrintOrEmailPage.ascx" %>

<CT:masterpage runat="server" id="CTMain">

<asp:Placeholder id="ph" runat="server">
<asp:Panel runat="server" defaultbutton="btnAdd2cart" causesvalidation="true">
<asp:ScriptManager runat="server" ID="AjaxManager" />
<script type="text/javascript">
<!--
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_beginRequest(BeginRequestHandler);
    prm.add_endRequest(EndRequestHandler);
    function BeginRequestHandler(sender, args) {
		var div = document.getElementById('divLoading');
		var larger = document.getElementById('<%=divLarger.ClientID%>');
		var details = document.getElementById('divItemDetails');
		var screen = document.getElementById('divScreen');
		var wrapper = document.getElementById('divSiteWrapper');
		var ctr;

		larger && larger.offsetHeight >= details.offsetHeight ? ctr = larger : ctr = details;

		div.style.height = ctr.offsetHeight + 'px';
		div.style.width = (ctr.offsetWidth - 4) + 'px';
		div.style.display = 'block';
		document.getElementById('divLoadingInner').style.paddingTop = '' + parseInt(ctr.offsetHeight / 2 - 28) + 'px';
		document.getElementById('divLoadingInner').innerHTML = 'Loading...<br /><img src="/images/loading.gif" style="margin-top:5px;" /><br />';
    }
    function EndRequestHandler(sender, args) {
		var div = document.getElementById('divLoading');
		var larger = document.getElementById('<%=divLarger.ClientID%>');
		var details = document.getElementById('divItemDetails');
		var screen = document.getElementById('divScreen');
		var wrapper = document.getElementById('divSiteWrapper');
		var ctr;

		larger && larger.offsetHeight >= details.offsetHeight ? ctr = larger : ctr = details;

		div.style.display = 'none';

		if(larger) {
			screen.style.height = wrapper.offsetHeight + 'px';
			screen.style.display='block';
		} else {
			screen.style.display='none'
		}

		
		LabelChange<%=RecipientDropDown.Label%>();
    }
    
	function LabelChange<%=RecipientDropDown.Label%>() {
		var ctrl = document.getElementById('<%=RecipientDropDown.RecipientDropDown.ClientID%>');
		var ctrlTr = document.getElementById('<%=RecipientDropDown.NewRow.ClientID %>');
		var ctrlTxt = document.getElementById('<%=RecipientDropDown.NewText.ClientID %>');
	    
	    if(!ctrl) return;
		//ctrlTxt.value = '';
		if (ctrl[ctrl.selectedIndex].value == 'OtherSpecify') {
			ctrlTr.style.display = '';
		} else {
			ctrlTr.style.display = 'none';
		}
	}

	if (window.addEventListener) {
		window.addEventListener('load', LabelChange<%=RecipientDropDown.Label%>, false);
	} else if (window.attachEvent) {
		window.attachEvent('onload', LabelChange<%=RecipientDropDown.Label%>);
	}
//-->
</script>

<div class="bcrmwrpr"><asp:Literal runat="server" id="ltlBreadcrumb" /></div>

<div class="tlswrpr">
    <CC:PrintOrEmailPage ID="ctrlPrintOrEmailPage" runat="server" />
</div>
<div class="sharewrpr">
    <CC:ShareAndEnjoy ID="ctrlShareAndEnjoy" runat="server" />
</div>

<asp:UpdatePanel runat="server" UpdateMode="conditional">
<Triggers>
<asp:PostBackTrigger ControlID="btnAdd2Wishlist" />
</Triggers>
<ContentTemplate>

<div style="width:170px; text-align:right; padding:2px 20px 0 0;"><asp:literal id="ltlPaging" text="here" runat="server" /></div><br />
<div id="divItemDetails" style="position:relative;z-index:5;height:1%;">
<div id="divLoading" style="border:solid 2px #999999;z-index:6;display:none;position:absolute;opacity:.93;-moz-opacity:.93;filter:alpha(opacity=93);background-color:#ffffff;" class="center"><div id="divLoadingInner"></div></div>

<div style="position:relative;z-index:4;">
	<div style="position:absolute;top:0px;left:0px;width:600px;background-color:#999999;z-index:1201;" id="divLarger" runat="server" visible="false">
		<div style="margin:2px;">
			<div style="background-color:#ffffff;padding-top:3px;">
				<table border="0" cellspacing="0" cellpadding="3" style="width:592px;">
					<tr>
						<td style="width:425px;" class="center"><asp:LinkButton runat="server" id="lnkViewDefault" class="smallest">View Default Image</asp:LinkButton></td>
						<td style="width:101px;" class="center"><span class="secondarytxtc smallest">Alternate Images</span></td>
					</tr>
					<tr valign="top">
						<td>
							<div id="divEnlargedImage" class="center">
								<img runat="server" id="imgEnlargedImage" style="border:solid 1px #e6e6e6;" /><br />
							</div>
						</td>
						<td>
							<div id="divAlternates" class="center" runat="server"></div>
						</td>
					</tr>
					<tr>
						<td colspan="2" class="center" style="margin-top:0px;padding-bottom:14px;">
							<asp:LinkButton runat="server" id="lnkCloseWindow">Close Window</asp:LinkButton>
						</td>
					</tr>
				</table>
			</div>
		</div>
	</div>
</div>

<table cellpadding="0" cellspacing="0" border="0" style="width:100%">
<tr>
<td class="vtop center">
	<asp:LinkButton runat="server" id="lnkEnlarge" style="text-decoration:none;">
	<img src="/assets/item/regular/<%=dbItem.Image%>" runat="server" id="imgProduct" alt="" /><br />
	<img src="/images/utility/enlarge.gif" style="width:36px; height:36px; vertical-align:middle" alt="Enlarge" /><span class="smaller">Enlarge</span><br /></asp:LinkButton>
    
</td>
<td style="width:10px;"><div class="colsp">&nbsp;</div></td>
<td class="vtop" style="width:100%">
<h1 class="hdng"><%=dbItem.ItemName%></h1>
<div><%=dbItem.LongDescription%></div>

<%If AttributeDisplayMode = "AdminDriven" Then%>
<div>
<CC:StoreItemAttributes ID="ctrlItemAttributes" runat="server" />
</div>
<%End If%>

<%	If AttributeDisplayMode = "AdminDriven" Then%>
	<asp:MultiView runat="server" id="mvInventory">
	<asp:View runat="server" id="vAddToCart">

		<table border="0" cellspacing="2" cellpadding="2" style="margin-top:10px;">
		<%  If dbItem.IsOnSale Then%>  
		<tr><td class="bold">Price:</td><td class="bold strike"><asp:Literal runat="server" id="ltlPriceSale" /></td></tr>
		<tr><td class="bold">Sale Price:</td><td class="bold red"><asp:Literal runat="server" id="ltlSalePrice" /></td></tr>
		<% else %>
		<tr><td class="bold">Price:</td><td class="bold"><asp:Literal runat="server" id="ltlPrice" /></td></tr>
		<% end if %>

		<asp:MultiView runat="server" id="mvAddToCart">
			<asp:View runat="server" id="vQty">
				<asp:Literal runat="server" id="ltlBackorder" />
				<tr><td class="bold nowrap">Quantity:</td><td>
				<asp:Textbox runat="server" id="txtQty" size="3" maxlength="4" class="qtybox" /></td></tr>
				<tr valign="top" runat="server" id="trShipTo"><td class="bold nowrap">Ship To:</td><td>
				<CC:StoreRecipientsDropDown ID="ctrlRecipientsDropDown" runat="server" />
				</td></tr>
				</table>
					
				<CC:RequiredFieldValidatorFront id="rvQty" ControlToValidate="txtQty" runat="server" ErrorMessage="Quantity field cannot be blank" />
				<CC:IntegerValidatorFront id="ivQty" ControlToValidate="txtQty" runat="server" ErrorMessage="Quantity field must contain valid integer value" />

				<div style="margin-top:15px;">
				<asp:ImageButton runat="server" id="btnAdd2Cart" CausesValidation="false" src="/images/utility/btn-add2cart.gif" CssClass="btnadd2cart" alt="Add to Cart"></asp:ImageButton>
				&nbsp;
				<asp:ImageButton runat="server" id="btnAdd2Wishlist" CausesValidation="false" src="/images/utility/btn-wishlist.gif" CssClass="btnwishlist" alt="Add to Wishlist"></asp:ImageButton><br />
				</div>

			</asp:View>
			<asp:View runat="server" id="vOutOfStock">
				<tr><td colspan="2" class="bold">This item is currently out of stock.</td></tr>
				<tr><td colspan="2"><CC:StoreItemEmailMe runat="server" id="ctrlEmailMeAtt" /></td></tr>
				</table>
			</asp:View>
		</asp:MultiView>

	</asp:View>
	<asp:View runat="server" id="vInventory">
		<p class="bold">This item is currently out of stock.</p>
		<CC:StoreItemEmailMe runat="server" id="ctrlEmailMe" />
	</asp:View>
	</asp:MultiView>
<%End If%>

</td>
</tr>
</table>
</div>

</ContentTemplate>
</asp:UpdatePanel>

<%	If AttributeDisplayMode = "TableLayout" Then%>
	<CC:StoreRecipientsDropDown ID="ctrlRecipientsDropDownTable" runat="server" />
	<table cellspacing="0" cellpadding="0" width="100%" class="carttbl">
		<tr>
			<td colspan="3" class="right" style="padding: 10px 5px;">
				<asp:ImageButton runat="server" id="btnAdd2CartTop" src="/images/utility/btn-add2cart.gif" CssClass="btnadd2cart" alt="Add to Cart"></asp:ImageButton>
				&nbsp;
				<asp:ImageButton runat="server" id="btnAdd2WishlistTop" src="/images/utility/btn-wishlist.gif" CssClass="btnwishlist" alt="Add to Wishlist"></asp:ImageButton><br />
			</td>
		</tr>
		<CC:StoreItemAttributes ID="ctrlItemAttributesTable" DisplayMode="TableLayout" runat="server" />
		<tr>
			<td colspan="3" class="right" style="padding: 10px 5px;">
				<asp:ImageButton runat="server" id="btnAdd2CartBottom" src="/images/utility/btn-add2cart.gif" CssClass="btnadd2cart" alt="Add to Cart"></asp:ImageButton>
				&nbsp;
				<asp:ImageButton runat="server" id="btnAdd2WishlistBottom" src="/images/utility/btn-wishlist.gif" CssClass="btnwishlist" alt="Add to Wishlist"></asp:ImageButton><br />
			</td>
		</tr>
	</table>
<%End If%>


<CC:StoreRelatedItems ID="ctrlRelatedItems" runat="server" />
</asp:Panel>
</asp:Placeholder>
</CT:masterpage>
