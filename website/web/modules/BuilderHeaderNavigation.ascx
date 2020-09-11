<%@ Control Language="VB" AutoEventWireup="false" CodeFile="BuilderHeaderNavigation.ascx.vb" Inherits="BuilderHeaderNavigation" %>

<div class="hdrwrpr">

	<div class="hdrlogo"><a href="/"><img src="/images/global/hdr-logo.gif" style="width:210px; height:70px; border-style:none;" alt="CBUSA" /></a><br /></div>
	
	<div class="hdrvendor">
		<select name="PVENDOR">
			<option value="0">Select Preferred Vendor</option><option value="1">Lorem Ipsum Dolor Sit Amet</option><option value="2">Consectetur Adipisicing Elit</option><option value="3">Sed Do Eiusmod Tempor</option><option value="4">Incididunt ut Labore</option><option value="5">Dolore Magna Aliqua</option><option value="6">Ut Eenim ad Minim Veniam</option> <option value="7">Qquis Nostrud Exercitation Ullamco</option><option value="8">Laboris Nisi ut Aliquip</option><option value="9">Duis Aute Irure Dolor</option>
		</select>
	</div>
	

	<div class="hdrsrchbx">
		<input type="text" name="KEYWORD" maxlength="30" class="hdrsbox" />
	</div>

	<div class="hdrsrchbtn">
		<input type="image" src="/images/global/btn-hdr-search.gif" class="hdrsbtn" alt="Search" />
	</div>

	<ul class="hdrnav">
        <li><a href="/directory/">MEMBER DIRECTORY</a></li>
        <li><a href="/takeoffs/">TAKE-OFFS</a></li>
        <li><a href="/order/">ORDERS</a></li>
        <li><a href="/builder/money/">MY MONEY</a></li>
        <li><a href="/rebates/builder-purchases.aspx">QUARTERLY PURCHASES</a></li>
        <li><a href="/logout.aspx">LOGOUT</a></li>
	</ul>
	
</div>