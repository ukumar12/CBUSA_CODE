<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TabControl.ascx.vb" Inherits="controls_TabControl" %>

<div>
<table cellpadding="0" cellspacing="0" border="0" style="margin: 0 0 0 -20px;width:400px" valign=" top">
<tr >
    <td width="150px">
        <div id="divTabs" runat="server" style="width:150px;margin: 30px 0 0 0;">
            <ul id="ulTabs" runat="server" ></ul>           
        </div>
    </td>
    <td style="border:1px solid #e1e1e1;background-color:#fff;"> 
    <div id="divContent" runat="server">       
    </div>    
    </td>
</tr>
</table>
</div>