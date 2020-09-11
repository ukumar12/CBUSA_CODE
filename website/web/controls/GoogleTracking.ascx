<%@ Control Language="VB" AutoEventWireup="false" CodeFile="GoogleTracking.ascx.vb" Inherits="controls_GoogleTracking" %>
<script type="text/javascript">
var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
</script>
<script type="text/javascript">
var pageTracker = _gat._getTracker("<%=GoogleTrackingNo%>");
pageTracker._initData();
pageTracker._trackPageview();
    <asp:literal id="ltlGoogleOrderTracking" Runat="server"></asp:literal>
</script>
