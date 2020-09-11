<%@ Page Language="VB" AutoEventWireup="false" CodeFile="menu.css.aspx.vb" Inherits="menucss" %>
a.L1{display:block;width:134px;text-align:left;text-decoration:none;font-weight:bold;font-size:11px;color:white;border:none;border:solid 1px #111D66;padding:1px;background:#333E8F;margin-top:1px;margin-left:0px}
a.L1:link{background:#E2EBF7;color:#294568;text-decoration:none;}
a.L1:active{background:#294568;color:white;text-decoration:none;}
a.L1:visited{background:#E2EBF7;color:#294568;text-decoration:none;}
a.L1:hover{color:#326BB2;border: solid 1px #111D66;background:white;text-decoration:underline;}

.L2{display:block;width:134px;text-align:left;text-decoration:none;font-weight:bold;font-size:11px;color:white;border:none;border:solid 1px #333E8F;padding:1px 1px 1px 11px;background:#326BB2;margin-top:2px;margin-left:0px;margin-right:4px;}
a.L2{display:block;width:134px;text-align:left;text-decoration:none;font-weight:bold;font-size:11px;color:white;border:none;border:solid 1px #333E8F;padding:1px 1px 1px 11px;background:#5563BB;margin-top:2px;margin-left:0px;margin-right:4px;}
a.L2:link{background:#B8CDE7;color:#294568;text-decoration:none;}
a.L2:active{background:black;color:white;text-decoration:none;}
a.L2:visited{background:#326BB2;color:white;text-decoration:none;}
a.L2:hover{color:black;border:none;border:solid 1px #326BB2;background:white;}

<%  If Request.Browser.Browser = "IE" Then%>
a.L3{display:block;width:134px;text-align:left;text-decoration:none;font-size:10px;color:white;border:none;border:solid 1px #5563BB;border-top:0px;padding:1px 1px 1px 9px;background:#6677D4;margin-left:0px;margin-right:4px;}
<% else %>
a.L3{display:block;width:126px;text-align:left;text-decoration:none;font-size:10px;color:white;border:none;border:solid 1px #5563BB;border-top:0px;padding:1px 1px 1px 9px;background:#6677D4;margin-left:0px;margin-right:4px;}
<% end if%>
a.L3:link{background:#FFFFFF;color:#294568;text-decoration:underline;}
a.L3:active{background:black;color:#294568;text-decoration:none;}
a.L3:visited{background:#FFFFFF;color:#294568;text-decoration:none;}
a.L3:hover{color:black;border:none;border:solid 1px #5563BB;border-top:0px;background:#E2EBF7;}

.I1{border:0px;vertical-align:top;width:12px;height:16px;}
.S1{margin-bottom:4px;}