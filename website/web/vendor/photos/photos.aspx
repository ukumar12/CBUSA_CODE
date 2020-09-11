<%@ Page Language="VB" AutoEventWireup="false" Title="Vendor Photos" CodeFile="photos.aspx.vb" Inherits="Photos"  %>

<CT:MasterPage ID="CTMain" runat="server">
<div class="pckgwrpr bggray">
<div class="pckghdgltblue">
    <asp:Literal id="ltlVendor" runat="server"></asp:Literal> Photos
</div>

<%--<script type="text/javascript" src="/includes/jquery-core-plugins.js"></script>--%>
    <script type="text/javascript" src="https://code.jquery.com/jquery-3.3.1.min.js"></script>
<script type="text/javascript" src="/includes/jquery-functions2.js"></script>
 
<link href="/includes/gallery.css" rel="stylesheet" type="text/css">
 

<div id="sGallery" style="visibility:hidden;">
 
	<div style="text-align:center; margin:auto; width:300px; font-family:arial; font-size:14px; font-weight:bold;">
		<a href="#"><span id="prev">Prev</span></a>
		<a href="#"><span id="next">Next</span></a>
	</div>
 
	<div id="slideshow" class="pics" style="margin:auto;clear:left;margin-top:10px">
		<asp:Literal id="ltlPhotos" runat="server"></asp:Literal>
	</div>
	
	<div class="sGalleryCaption" style="height:10px;"></div>
 
	<%--<div style="text-align:center; font-family:arial; font-size:14px; font-weight:bold; margin-top:10px;">
		<div id="jCounter"></div>
	</div>--%>
 
	<div style="text-align:center; width:525px; margin:20px auto 0 auto; text-align:auto;">
		<ul id="navCycle" class="jcarousel-skin"></ul>
	</div>
 
	<br clear="all" /><br />
 
</div>


<div class="pckgbdy" id="divNoPhotos" runat="server" visible="false"><p>No photos found for this vendor.</p></div>
</div>
</CT:MasterPage>

