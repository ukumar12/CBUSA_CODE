<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PIQHeaderNavigation.ascx.vb" Inherits="PIQHeaderNavigation" %>

<div class="hdrwrpr">

	<div class="hdrlogo"><a href="/"><img src="/images/global/hdr-logo.gif" style="width:210px; height:70px; border-style:none;" alt="CBUSA" /></a><br /></div>
	
	<div class="hdrvendor">
	</div>
	

	<div class="hdrsrchbx">
		<input type="text" name="KEYWORD" maxlength="30" class="hdrsbox" />
	</div>

	<div class="hdrsrchbtn">
		<input type="image" src="/images/global/btn-hdr-search.gif" class="hdrsbtn" alt="Search" />
	</div>

	<ul class="hdrnav">
		<li><a href="/pi/account.aspx">USER ACCOUNTS</a>
        <li><a href="/directory/">CBUSA MEMBER DIRECTORY</a>
        <li><a href="/vendor/">UPDATE INFO/ADS PAGE</a>
        <li><a href="/products/">PRODUCT CATALOG</a>
        <li><a href="/logout.aspx">LOGOUT</a></li>
	</ul>
	
</div>