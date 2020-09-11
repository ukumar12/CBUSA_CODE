<%@ Page Language="vb" AutoEventWireup="false" Inherits="member_reminders_default" CodeFile="default.aspx.vb" %>

<CT:masterpage runat="server" id="CTMain">

<h2 class="hdng">My Reminders</h2>

<p style="padding-left: 20px;"><CC:OneClickButton id="btnAdd" runat="server" Text="Add New Reminder" CSsClass="btn" /></p>

<!-- cart table -->
<div id="divItems" runat="server">
<asp:repeater id="rptReminders" EnableViewState="False" runat="server">
    <HeaderTemplate>
       <table cellspacing="0" cellpadding="0" border="0" class="bdrtop bdrright" style="width:756px; margin-left:18px;" summary="shopping cart table">
        <tr>
            <th class="trgrad" style="padding:6px 0 6px 15px;">&nbsp;</td>
	        <th class="trgrad" style="padding:6px 0 6px 0;">&nbsp;</td>
	        <th class="trgrad" style="padding:6px 0 6px 0;">Name</td>
	        <th class="trgrad" style="padding:6px 0 6px 0;">Recurs?</td>	        
	        <th class="trgrad" style="padding:6px 0 6px 0;">Date</td>
	        <th class="trgrad" style="padding:6px 0 6px 0;">1st Reminder</td>
	        <th class="trgrad" style="padding:6px 0 6px 0;">2nd Reminder</td>
	        </tr>      
        </HeaderTemplate>
        <ItemTemplate>
            <!-- row-->
	        <tr valign="top">
	        <td class="center bdrbottom bdrleft" style="padding-top: 5px; padding-bottom: 5px;">
		        <CC:OneClickButton id="btnEdit" commandname="Edit" runat="server" CssClass="btn" Text="Edit" />
	        </td>
	        <td class="blue bdrbottom" style="padding-top: 5px; padding-bottom: 5px;">
		        <CC:OneClickButton id="btnDelete" commandname="Remove" runat="server" CssClass="btn" Text="Delete" />
	        </td>
	        <td class="blue bdrbottom" style="padding-top: 5px; padding-bottom: 5px;">
		        <asp:literal id="ltlName" runat="server" />
	        </td>
	        <td class="bdrbottom" style="padding-top: 5px; padding-bottom: 5px;">
		        <asp:literal id="ltlRecurs" runat="server" />
	        </td>
	        <td class="blue bdrbottom" style="padding-top: 5px; padding-bottom: 5px;">
		        <asp:literal id="ltlDate" runat="server" />
	        </td>
	        <td class="blue bdrbottom" style="padding-top: 5px; padding-bottom: 5px;">
		        <asp:literal id="ltlFirstReminder" runat="server" />
	        </td>
	        <td class="blue bdrbottom" style="padding-top: 5px; padding-bottom: 5px;">
		        <asp:literal id="ltlSecondReminder" runat="server" />
	        </td>
	        </tr>      
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:repeater>
    </div>
    
     <div id="divNoItems" runat="server">
      <asp:literal id="ltlNoItems" runat="server" />
    </div>    
</CT:masterpage>