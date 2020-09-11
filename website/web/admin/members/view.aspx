<%@ Page Language="VB" AutoEventWireup="false" CodeFile="view.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Members"%>
<%@ Register Src="~/controls/MemberAddress.ascx" TagName="MemberAddress" TagPrefix="CC" %>
	
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<script language="Javascript">
<!--     
  function CheckSameAsBilling(sCheckboxName, sDivName) {
    if (document.getElementById(sCheckboxName).checked == true) {
      document.getElementById(sDivName).style.display = 'none';
    } else {
      document.getElementById(sDivName).style.display = 'block';
    }
  }   
//-->
</script>
	
<h4>View Member</h4>

<asp:LinkButton id="btnCancel" runat="server" CausesValidation="False" Text="&laquo; Go back to List" style="margin-left:15px;"  />

<table cellspacing="0" cellpadding="3" border="0" style="width:761px; margin:10px 0 0 15px;" summary="billing and shipping">
<tr>
    <td style="width:373px;" class="optional">Account Details</td>
    <td style="width:15px;"><img src="/images/spacer.gif" width="15" height="1" alt="" /><br /></td>
    <td style="width:373px;" class="optional">Account Navigation</td>
</tr>
<tr valign="top">
        <td style="width:373px;" class="field">
        <table runat="server" id="tblAccount" width="100%">
        <tr><td><b>Username:</b></td><td><%= Server.HtmlEncode(dbMember.Username)%></td></tr>
        <tr><td><b>Email Address:</b></td><td><%=Server.HtmlEncode(dbBilling.Email)%></td></tr>
        <tr><td><b>Member Since:</b></td><td><%=dbMember.CreateDate.ToLongDateString%></td></tr>
        <tr><td><b>Account Status:</b></td><td class="bold"><%If dbMember.IsActive Then%>Active<%Else%><span class="red">Disabled</span><%End If%></td></tr>
        </table>
        <CC:OneClickButton id="btnEditAccount" Text="Edit" runat="server" CssClass="btn"/>
    </td>
    <td style="width:15px;"><img src="/images/spacer.gif" width="15" height="1" alt="" /><br /></td>
    <td style="width:373px;" class="field">    
        <table border="0" width="100%">
        <tr>
        <td>
       <ul>
       <li><a runat="server" id="lnkOrder" >Order History</a></li>
       <li><a runat="server" id= "lnkWishList">Wishlist</a></li>
       <li><a runat="server" id= "lnkAddressBook" >Address Book</a></li>
       <li><a runat="server" id= "lnkReminder" >Reminders</a></li>
       <li><a runat="server" id= "lnkEmailPref">Email Preferences</a></li>
       <li><a runat="server" id= "lnkAccount" >Change Password</a></li>
       </ul>
    </td>
        <td style="vertical-align:top;">
            <CC:ConfirmButton runat="server" ID="btnSendPassword" CssClass="btn" Text="Email Password" Message="Are you sure you wish to send an email to this customer containing their password?" />
        </td>
</tr>
        </table>
    </td>
</tr>
</table>   

<table cellspacing="0" cellpadding="0" border="0" style="width:761px; margin:10px 0 0 15px;" summary="billing and shipping">
<tr>
    <td style="width:373px;" class="optional">Billing Address</td>
    <td style="width:15px;"><img src="/images/spacer.gif" width="15" height="1" alt="" /><br /></td>
    <td style="width:373px;" class="optional">Default Shipping Address</td>
</tr>
<tr valign="top">
    <td style="width:373px;" class="field">
        <CC:MemberAddress id="ctrlBillingAddress" runat="server"></CC:MemberAddress>
        <CC:OneClickButton id="btnEditBilling" runat="server" Text="Edit" CssClass="btn" />
	</td>
	<td style="width:15px;"><img src="/images/spacer.gif" width="15" height="1" alt="" /><br /></td>
	<td style="width:373px;" class="field">    
       <CC:MemberAddress id="ctrlShippingAddress" runat="server"></CC:MemberAddress>
       <CC:OneClickButton id="btnEditShipping" runat="server" Text="Edit" CssClass="btn" />            
	</td>
</tr>
</table>

<p></p>

<CC:GridView id="gvListOrderNotes" CellSpacing="2" CellPadding="2" runat="server" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no order notes for this member" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
	    <asp:templateField headerText="Submitted By" SortExpression="a.AdminId">
             <itemtemplate><asp:Literal runat="server" id="ltlSubmittedBy"></asp:Literal></itemtemplate>
        </asp:templateField>
         <asp:templateField headerText="Order#" SortExpression="OrderNo">
             <itemtemplate><asp:Literal runat="server" id="ltlOrderNo"></asp:Literal></itemtemplate>
        </asp:templateField>
		<asp:BoundField SortExpression="NoteDate" DataField="NoteDate" HeaderText="Note Date"></asp:BoundField>
		<asp:BoundField SortExpression="Note" DataField="Note" HeaderText="Note" HtmlEncode="false"></asp:BoundField>		
	</Columns>
</CC:GridView>

</asp:content>
