<%@ Control Language="VB" EnableViewState="False" AutoEventWireup="false" CodeFile="CssMenu.ascx.vb" Inherits="CssMenu" %>
<%@ Import Namespace="System.Configuration.ConfigurationManager" %>
<div id="qm0" class="qmmc">
<a href="<%=AppSettings("GlobalRefererName")%>">About Us</a>
	<div>
	<a href="<%=AppSettings("GlobalRefererName")%>/">Satisfaction</a>
	<a href="<%=AppSettings("GlobalRefererName")%>">Our Goals</a>
	<a href="<%=AppSettings("GlobalRefererName")%>">Product Warranty</a>
	<a href="<%=AppSettings("GlobalRefererName")%>">Future Outlook</a>
	<a href="<%=AppSettings("GlobalRefererName")%>">Product Quality</a>
	<a href="<%=AppSettings("GlobalRefererName")%>">Continued Support</a>
	</div>

<a href="<%=AppSettings("GlobalRefererName")%>/store/">Store 1</a>
<div>
<% =GenerateDepartmentMenu(690)%>
</div>

<a href="<%=AppSettings("GlobalRefererName")%>/store/">Store 2</a>
<div>
<% =GenerateDepartmentMenu(690)%>
</div>

<a href="<%=AppSettings("GlobalRefererName")%>/service/">Customer Service</a>
	<div>
	<a href="<%=AppSettings("GlobalRefererName")%>/service/faq.aspx">View FAQ's</a>
	<a href="<%=AppSettings("GlobalRefererName")%>/service/contact.aspx">Contact Us</a>
	<a href="<%=AppSettings("GlobalRefererName")%>/service/order.aspx">Check Order Status</a>
	</div>
<span class="qmclear">&nbsp;</span>
</div>

<script type="text/javascript">
<!--
//-->
</script>

