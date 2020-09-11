<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VendorHeaderNavigation.ascx.vb" Inherits="VendorHeaderNavigation" %>
<div class="hdrwrpr">

	<div class="hdrlogo"><a href="/"><img src="/images/global/hdr-logo.gif" style="width:210px; height:70px; border-style:none;" alt="CBUSA" /></a><br /></div>
	
	<div class="hdrsrchbx">
		<input type="text" name="KEYWORD" maxlength="30" class="hdrsbox" />
	</div>

	<div class="hdrsrchbtn">
		<input type="image" src="/images/global/btn-hdr-search.gif" class="hdrsbtn" alt="Search" />
	</div>

	<ul class="hdrnav">
        <li><a href="/directory/">CBUSA Member Directory</a></li>
        <li><a href="/order/">Orders</a></li>
        <li><a href="/forms/vendor-registration/sku-price.aspx">Update SKUs and Pricing</a></li>
        <li><a href="/">Product Catalog</a></li>
        <li><a href="/rebates/vendor-sales.aspx">Quarterly Sales Reports</a></li>
        <li><a href="/logout.aspx">Logout</a></li>
	</ul>
</div>
