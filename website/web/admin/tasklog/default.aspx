<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Task Log" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Task Log Administration</h4>

<h5>Reporting Parameters</h5>
<asp:Panel ID="pReporting" DefaultButton="btnUpdateReporting" runat="server">
<table cellpadding="2" cellspacing="4" width="600">
<tr>
<th valign="top" width="200"><b>Current Quarter End Date:</b></th>
<td valign="top" class="field"  width="400">
    <b><asp:literal ID="ltlQtrEndDate" runat="server" /></b>
</td>
</tr>
<tr>
<th valign="top"><b>Reporting Deadline Days:</b><br /><span class="smallest">(after end of quarter)</span></th>
<td valign="top" class="field">
    <asp:TextBox ID="txtReportingDeadlineDays" runat="server"></asp:TextBox>
    <asp:RequiredFieldValidator ID="rfvReportingDeadlineDays" runat="server" ValidationGroup="Reporting" ControlToValidate="txtReportingDeadlineDays" ErrorMessage="Filed 'Reporting Deadline Days' Is required." />
    <CC:IntegerValidator ID="ivReportingDeadlineDays" runat="server" ValidationGroup="Reporting" ControlToValidate="txtReportingDeadlineDays" ErrorMessage="Filed 'Reporting Deadline Days' Is invalid." />
</td>
</tr>
<tr>
<th valign="top"><b>Reporting Deadline:</b></th>
<td valign="top" class="field">
    <b><asp:literal ID="ltlReportingDeadline" runat="server" /></b>
</td>
</tr>
<tr>
<th valign="top"><b>Activate Auto Dispute Task?</b></th>
<td valign="top" class="field">
    <asp:DropDownList ID="drpActivateAutoDisputes" runat="server">
        <asp:ListItem Text="No" Value="0"></asp:ListItem>
        <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
    </asp:DropDownList>
</td>
</tr>

<tr>
<th valign="top"><b>Dispute Deadline Days:</b><br /><span class="smallest">(Days after Reporting Deadline)</span></th>
<td valign="top" class="field">
    <asp:TextBox ID="txtDiscrepancyDeadlineDays" runat="server"></asp:TextBox>
    <asp:RequiredFieldValidator ID="rfvDiscrepancyDeadlineDays" runat="server" ValidationGroup="Reporting" ControlToValidate="txtDiscrepancyDeadlineDays" ErrorMessage="Filed 'Dispute Deadline Days' Is required." />
    <CC:IntegerValidator ID="ivDiscrepancyDeadlineDays" runat="server" ValidationGroup="Reporting" ControlToValidate="txtDiscrepancyDeadlineDays" ErrorMessage="Filed 'Dispute Deadline Days' Is invalid." />
</td>
</tr>
<tr>
<th valign="top"><b>Dispute Deadline:</b></th>
<td valign="top" class="field">
    <b><asp:literal ID="ltlDiscrepancyDeadline" runat="server" /></b>
</td>
</tr>
<tr>
<th valign="top"><b>Dispute Response Deadline Days:</b><br /><span class="smallest">(Days after Dispute Deadline)</span></th>
<td valign="top" class="field">
    <asp:TextBox ID="txtDisputeDeadlineDays" runat="server"></asp:TextBox>
    <asp:RequiredFieldValidator ID="rfvDisputeDeadlineDays" runat="server" ValidationGroup="Reporting" ControlToValidate="txtDisputeDeadlineDays" ErrorMessage="Filed 'Dispute Response Deadline Days' Is required." />
    <CC:IntegerValidator ID="ivDisputeDeadlineDays" runat="server" ValidationGroup="Reporting" ControlToValidate="txtDisputeDeadlineDays" ErrorMessage="Filed 'Dispute Response Deadline Days' Is invalid." />
</td>
</tr>
<tr>
<th valign="top"><b>Dispute Response Deadline:</b></th>
<td valign="top" class="field">
    <b><asp:literal ID="ltlDisputeDeadline" runat="server" /></b>
</td>
</tr>
<tr>
<td colspan="2" align="right">
<CC:OneClickButton id="btnUpdateReporting" Runat="server" ValidationGroup="Reporting" CausesValidation="true" Text="Update Reporting Parameters" cssClass="btn" />
</td>
</tr>
</table>

</asp:Panel>
<p></p>
<table >
<tr>
<td>
 IdevSearch Update Trigger : 
</td>
<td align ="left ">
<asp:Button id="btnUpdateIdevSearch" runat="server"   CausesValidation="True"  Text="Update IdevSearch" cssClass="btn " />
<asp:Literal ID="lnkIdevSearchStatus"  runat="server"   ></asp:Literal>
</td>
</tr>
</table> 
<p>
<asp:Literal  ID="ltlIdevSearchTrigger"  runat="server"  ></asp:Literal>
</p>
<p>
<asp:Literal ID="ltlIdevIndex" runat="server"></asp:Literal>
</p>




<h5>Task Log Search</h5>
<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="4">
<tr>
<th valign="top"><b>Task Name:</b></th>
<td valign="top" class="field">
    <asp:DropDownList ID="F_TaskName" runat="server" >
        <asp:ListItem Text="--All--" Value=""></asp:ListItem>
        <asp:ListItem Text="PurgeCCInfo" Value="PurgeCCInfo"></asp:ListItem>
        <asp:ListItem Text="IdevSearch" Value="IdevSearch"></asp:ListItem>
        <asp:ListItem Text="UpdatePrices" Value="UpdatePrices"></asp:ListItem>
        <asp:ListItem Text="ProcessExportQueue" Value="ProcessExportQueue"></asp:ListItem>
        <asp:ListItem Text="NoSalesReportReceived" Value="NoSalesReportReceived"></asp:ListItem>
        <asp:ListItem Text="NoDisputeResponseReceived" Value="NoDisputeResponseReceived"></asp:ListItem>
        <asp:ListItem Text="DiscrepancyReport" Value="DiscrepancyReport"></asp:ListItem>
        <asp:ListItem Text="DiscrepencyResponseDeadlinePassed" Value="DiscrepencyResponseDeadlinePassed"></asp:ListItem>
        <asp:ListItem Text="NoPurchasesReportReceived" Value="NoPurchasesReportReceived"></asp:ListItem>
        <asp:ListItem Text="ReportingReminders" Value="ReportingReminders"></asp:ListItem>
        <asp:ListItem Text="Reporting (Auto Disputes)" Value="Reporting"></asp:ListItem>   
        <asp:ListItem Text="ProcessAutoDisputes (Fix Auto Disputes)" Value="ProcessAutoDisputes"></asp:ListItem>       
        <asp:ListItem Text="ProcessRebateTerms" Value="ProcessRebateTerms"></asp:ListItem>    
          <asp:ListItem Text="VendorReminder(Rebates)" Value="VendorReminder"></asp:ListItem>       
            <asp:ListItem Text="VendorTermination(Rebates)" Value="VendorTermination"></asp:ListItem>       
              <asp:ListItem Text="BuilderRebateNotification(Rebates)" Value="BuilderRebateNotification"></asp:ListItem>          
  
  
    </asp:DropDownList>
</td>
</tr>
<tr>
<th valign="top"><b>Log Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_LogDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_LogDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Status:</b></th>
<td valign="top" class="field">
    <asp:DropDownList ID="F_Status" runat="server">
        <asp:ListItem Text="--All--" Value=""></asp:ListItem>
        <asp:ListItem Text="Started" Value="Started"></asp:ListItem>
        <asp:ListItem Text="Skipped" Value="Skipped"></asp:ListItem>
        <asp:ListItem Text="Failed" Value="Failed"></asp:ListItem>
        <asp:ListItem Text="Completed" Value="Completed"></asp:ListItem>
    </asp:DropDownList>
</td>
</tr>
<tr>
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" CausesValidation="false" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
</table>

</asp:Panel>
<p></p>



<CC:GridView id="gvList" CellPadding="5" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use Sort By drop down" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" CssClass="MultilineTable" gridlines="none">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:BoundField SortExpression="TaskName" DataField="TaskName" HeaderText="Task Name"></asp:BoundField>
		<asp:BoundField SortExpression="Status" DataField="Status" HeaderText="Status"></asp:BoundField>
		<asp:BoundField SortExpression="LogDate" DataField="LogDate" HeaderText="Log Date" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField DataField="Msg" HeaderText="Notes"></asp:BoundField>
	</Columns>
</CC:GridView>


</asp:content>