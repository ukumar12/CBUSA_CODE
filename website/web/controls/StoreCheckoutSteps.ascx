<%@ Control Language="VB" AutoEventWireup="false" CodeFile="StoreCheckoutSteps.ascx.vb" Inherits="StoreCheckoutSteps" %>

<%  If Not MultipleShipToEnabled Then%>

<table border="0" cellpadding="4" style="margin-top:10px;margin-bottom:10px;">
<%  If CurrentStep = CheckouStepEnum.Billing Then%>
<tr>
	<td class="red bdr">Step 1: Billing &amp; Shipping</td>
	<td class="bdr">Step 2: Payment</td>
	<td class="bdr">Step 3: Receipt</td>
</tr>
<% end if%>

<%  If CurrentStep = CheckouStepEnum.Payment Then%>
<tr>
	<td class="bdr"><a href="/store/billing.aspx">Step 1: Billing &amp; Shipping</a></td>
	<td class="red bdr">Step 2: Payment</td>
	<td class="bdr">Step 3: Receipt</td>
</tr>
<% End if %>

<%  If CurrentStep = CheckouStepEnum.Confirmation Then%>
<tr>
	<td class="bdr">Step 1: Billing &amp; Shipping</td>
	<td class="bdr">Step 2: Payment</td>
	<td class="red bdr">Step 3: Receipt</td>
</tr>
<% End if %>
</table>

<% Else %>

<table border="0" cellpadding="4" style="margin-top:10px;margin-bottom:10px;">
<%  If CurrentStep = CheckouStepEnum.Billing Then%>
<tr>
	<td class="red bdr">Step 1: Billing</td>
	<td class="bdr">Step 2: Shipping</td>
	<td class="bdr">Step 3: Payment</td>
	<td class="bdr">Step 4: Receipt</td>
</tr>
<% end if%>

<%  If CurrentStep = CheckouStepEnum.Shipping Then%>
<tr>
	<td class="bdr"><a href="/store/billing.aspx">Step 1: Billing</a></td>
	<td class="red bdr">Step 2: Shipping</td>
	<td class="bdr">Step 3: Payment</td>
	<td class="bdr">Step 4: Receipt</td>
</tr>
<% End if %>

<%  If CurrentStep = CheckouStepEnum.Payment Then%>
<tr>
	<td class="bdr"><a href="/store/billing.aspx">Step 1: Billing</a></td>
	<td class="bdr"><a href="/store/shipping.aspx">Step 2: Shipping</a></td>
	<td class="red bdr">Step 3: Payment</td>
	<td class="bdr">Step 4: Receipt</td>
</tr>
<% End if %>

<%  If CurrentStep = CheckouStepEnum.Confirmation Then%>
<tr>
	<td class="bdr">Step 1: Billing</td>
	<td class="bdr">Step 2: Shipping</td>
	<td class="bdr">Step 3: Payment</td>
	<td class="red bdr">Step 4: Receipt</td>
</tr>
<% End if %>
</table>
<% End if %>


