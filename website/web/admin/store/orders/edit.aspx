<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Web Order Edit" CodeFile="edit.aspx.vb" Inherits="Edit"  %>
<%@ Register TagName="StoreShoppingCartAdmin" TagPrefix="CC" Src="~/controls/StoreShoppingCartAdmin.ascx" %>
<%@ Import Namespace="DataLayer" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<asp:ScriptManager ID="AjaxManager" runat="server" />


<h4>Web Order</h4>

<div style="background-color:#FF0000;color:#FFFFFF; margin-bottom:10px;" id="divHighRisk" runat="server" visible="false" />

<asp:Literal runat="server" ID="ltlErrorMessage" ></asp:Literal>

<table cellpadding="2" cellspacing="1">
	<tr>
		<td class="optional" style="width:150px;">Order No:</td>
		<td class="field"><% =dbStoreOrder.OrderNo%></td>
	</tr>
	<tr>
		<td class="optional">Order Date:</td>
		<td class="field"><% =dbStoreOrder.ProcessDate%></td>
	</tr>
	<tr>
		<td class="optional">IP Address:</td>
		<td class="field"><%=dbStoreOrder.RemoteIP%></td>
	</tr>
	<% If Not dbStoreOrder.PromotionCode = Nothing Then%>
	<tr>
		<td class="optional">Promotion Code:</td>
		<td class="field"><%=dbStoreOrder.PromotionCode %></td>
		<td></td>
	</tr>
	<% end if %>
	<% If Not dbStoreOrder.HowHeardId = 0 Then%>
	<tr>
		<td class="optional">How Heard:</td>
		<td class="field"><%=dbStoreOrder.HowHeardName%></td>
		<td></td>
	</tr>
	<% end if %>
	<% If Not dbStoreOrder.Comments = String.Empty Then%>
	<tr>
		<td class="optional">Special<br />Instructions:</td>
		<td class="field"><div style="width:465px;"><%=dbStoreOrder.Comments%></div></td>
		<td></td>
	</tr>
	<% end if %>
	<tr>
		<td class="optional">Payment Log:</td>
		<td class="field"><a href="/admin/store/payments/default.aspx?F_OrderNo=<%=dbStoreOrder.OrderNo%>"><%=dbStoreOrder.OrderNo%></a></td>
		<td></td>
	</tr>
</table>


<h4>Order Details</h4>

<div style="width:800px">
<CC:StoreShoppingCartAdmin ID="ctrlCart" runat="server" EditMode="False" />
</div>

<p></p>
<h4>Order Notes</h4>

<script language="javascript">
<!--
    function OrderNotesSubmit(arg, context) {
    <%=CallBack%>
    }
    function OrderNotesRefresh(arg,context) {
        document.getElementById('iframenotes').src = "/admin/store/orders/notes.aspx?OrderId=<%=dbStoreOrder.OrderId%>"
        document.getElementById('<%=txtNote.ClientId %>').value = '';
    }
//-->
</script>

<table>
<tr>
<td valign="top">

<asp:TextBox ID="txtNote" runat="Server" TextMode="MultiLine" Columns="70" Rows="3" style="width:600px"></asp:TextBox>
<input type="button" name="btnSubmitNote" value="Add Note" class="btn" onclick="OrderNotesSubmit(<%=txtNote.ClientId%>.value,'');"/>
<br />

<iframe src="/admin/store/orders/notes.aspx?OrderId=<%=dbStoreOrder.OrderId%>" width="806" id="iframenotes" height="150" frameborder="0" scrolling="yes"></iframe>
</td>
</tr>
</table>

<h4>Billing Address</h4>
<table cellpadding="2" cellspacing="1">
<tr>
	<td class="required" style="width:150px;">First Name:</td>
	<td class="field"><asp:textbox id="txtBillingFirstName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
	<td><asp:RequiredFieldValidator ID="rfvBillingFirstName" runat="server" Display="Dynamic" ControlToValidate="txtBillingFirstName" ErrorMessage="Field 'Billing First Name' is blank"></asp:RequiredFieldValidator></td>
</tr>
<tr>
	<td class="required">Last Name:</td>
	<td class="field"><asp:textbox id="txtBillingLastName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
	<td><asp:RequiredFieldValidator ID="rfvBillingLastName" runat="server" Display="Dynamic" ControlToValidate="txtBillingLastName" ErrorMessage="Field 'Billing Last Name' is blank"></asp:RequiredFieldValidator></td>
</tr>
<tr>
	<td class="required">Email:</td>
	<td class="field"><asp:textbox id="txtEmail" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
	<td><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is blank"></asp:RequiredFieldValidator></td>
</tr>
<tr>
	<td class="optional">Company:</td>
	<td class="field"><asp:textbox id="txtBillingCompany" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
	<td></td>
</tr>
<tr>
	<td class="required">Address 1:</td>
	<td class="field"><asp:textbox id="txtBillingAddress1" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
	<td><asp:RequiredFieldValidator ID="rfvBillingAddress1" runat="server" Display="Dynamic" ControlToValidate="txtBillingAddress1" ErrorMessage="Field 'Billing Address 1' is blank"></asp:RequiredFieldValidator></td>
</tr>
<tr>
	<td class="optional">Address 2:</td>
	<td class="field"><asp:textbox id="txtBillingAddress2" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
	<td></td>
</tr>
<tr>
	<td class="required">City:</td>
	<td class="field"><asp:textbox id="txtBillingCity" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
	<td><asp:RequiredFieldValidator ID="rfvBillingCity" runat="server" Display="Dynamic" ControlToValidate="txtBillingCity" ErrorMessage="Field 'Billing City' is blank"></asp:RequiredFieldValidator></td>
</tr>
<tr>
	<td class="optional">State:</td>
	<td class="field"><asp:DropDownList id="drpBillingState" runat="server" /></td>
	<td></td>
</tr>
<tr>
	<td class="optional">Province/Region:<div class="smallest">(if applicable)</div></td>
	<td class="field"><asp:textbox id="txtBillingRegion" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
	<td></td>
</tr>
<tr>
	<td class="required">Country:</td>
	<td class="field"><asp:DropDownList id="drpBillingCountry" runat="server" /></td>
	<td><asp:RequiredFieldValidator ID="rfvBillingCountry" runat="server" Display="Dynamic" ControlToValidate="drpBillingCountry" ErrorMessage="Field 'Billing Country' is blank"></asp:RequiredFieldValidator></td>
</tr>
<tr>
	<td class="required">Zip:</td>
	<td class="field"><asp:textbox id="txtBillingZip" runat="server" maxlength="15" columns="15" style="width: 109px;"></asp:textbox></td>
	<td><asp:RequiredFieldValidator ID="rfvBillingZip" runat="server" Display="Dynamic" ControlToValidate="txtBillingZip" ErrorMessage="Field 'Billing Zip' is blank"></asp:RequiredFieldValidator></td>
</tr>
<tr>
	<td class="required">Phone:</td>
	<td class="field"><asp:textbox id="txtBillingPhone" runat="server" maxlength="15" columns="15" style="width: 109px;"></asp:textbox></td>
	<td><asp:RequiredFieldValidator ID="rfvBillingPhone" runat="server" Display="Dynamic" ControlToValidate="txtBillingPhone" ErrorMessage="Field 'Billing Phone' is blank"></asp:RequiredFieldValidator></td>
</tr>
</table>

<asp:UpdatePanel ID="AjaxPanel" runat="server" UpdateMode="Conditional">
<Triggers>
</Triggers>
<ContentTemplate>



<div id="divAuthenticate" runat="server">
<h4>Payment Information-Additional Authentication</h4>
<asp:Panel runat="server" id="pnlSecondary" defaultbutton="btnVerify" >
<table cellpadding="1" cellspacing="1" runat="Server" id="tblSecondaryAuthentication">
<tr runat="server" visible="false" id="trErrorLogin">
   <td colspan="3">Invalid Credentials: Please re-enter secondary password and try again</td>
</tr>
<tr>
	<td class="required">Secondary Password:</td>
	<td class="field"><asp:textbox id="txtUserPass" TextMode="password" runat="server"/></td>
	<td><CC:OneClickButton CssClass="btn" text="Verify" runat="server" ValidationGroup="SecPass" id="btnVerify" /></td>
	<td><asp:requiredfieldvalidator validationgroup="SecPass" enabled="false" ControlToValidate="txtUserPass" Display="Dynamic" ErrorMessage="Please enter password" runat="server" id="rfvtxtSecPassword" /></td>
</tr>
</table>
</asp:Panel>
</div>

<div id="divInternalUser" runat="Server" style="display:none">
<h4>Payment information</h4>
 You are logged in as an internal user. Internal Users are not allowed to view Payment information.
</div>

<div id="divPaymentInfo" runat="server">
<h4>Payment information</h4>

<table cellpadding="2" cellspacing="1" runat="Server" id="tblPaymentTypeCC">
	<tr>
		<td style="width:150px;" class="optional" id="tdCardNumber" runat="server">Card Number:</td>
		<td class="field"><asp:Literal ID="ltlCardNumber" runat="Server"></asp:Literal></td>
        <td>&nbsp;</td>
	</tr>
	<tr>
		<td class="optional" style="width:100px;">Card Type:</td>
		<td class="field"><asp:Literal ID="ltlCardType" runat="server" /></td>
        <td>&nbsp;</td>
    </tr>
	<tr>
		<td class="optional" style="width:100px;">Expiration Date:</td>
		<td class="field"><asp:Literal ID="ltlExpirationDate" runat="server" /></td>
		<td>&nbsp;</td>
	</tr>
	<tr>
		<td class="optional" style="width:100px;">Cardholder Name:</td>
		<td class="field"><asp:Literal ID="ltlCardholdername" runat="server" /></td>
        <td>&nbsp;</td>
	</tr>
	<tr id="trPaymentLog" runat="server">
		<td class="optional">Payment Log:</td>
		<td class="field"><a href="/admin/store/payments/default.aspx?F_OrderNo=<%=dbStoreOrder.OrderNo%>"><%=dbStoreOrder.OrderNo%></a></td>
		<td></td>
	</tr>
</table>

</div>

</ContentTemplate>
</asp:UpdatePanel>

<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

  
</asp:content>

