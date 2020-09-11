<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Builder" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Builder Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Builder:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_BuilderID" runat="server" /></td>
</tr>

<tr>
<th valign="top"><b>Vendor :</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_VendorID" runat="server" /></td>
</tr>

<tr>
<th valign="top">Builder Historic ID:</th>
<td valign="top" class="field"><asp:textbox id="F_BuilderHistoricId" runat="server" Columns="50" MaxLength="5"></asp:textbox></td>
</tr>



<tr>
 
<th valign="top">Vendor Historic ID:</th>
<td valign="top" class="field"><asp:textbox id="F_VendorHistoricId" runat="server" Columns="50" MaxLength="5"></asp:textbox></td>
</tr>
 
 <tr>
	<th valign="top">LLC(s):</th>
	<td class="field"><CC:CheckBoxListEx ID="F_LLC" runat="server" RepeatColumns="3"></CC:CheckBoxListEx></td>
</tr>
<tr>
 

<th valign="top"><b>Sent Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_SubmittedLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_SubmittedUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
 
<td align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
</table>
</asp:Panel>
<p></p>


<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50"  AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
    
	<Columns>
		<asp:BoundField SortExpression="BLDRID" DataField="BuilderName" HeaderText="BuilderName "></asp:BoundField>
		<asp:BoundField SortExpression="VNDRNAME" DataField="VNDRNAME" HeaderText="VendorName"  ></asp:BoundField>
        <asp:BoundField SortExpression="l.LLC" DataField="LLC" HeaderText="BuilderLLC"  ></asp:BoundField>
		<asp:BoundField SortExpression="INVOICE" DataField="INVOICE" HeaderText="INVOICE"></asp:BoundField>
       
        <asp:TemplateField>
		    <HeaderTemplate>
		       Period-Year
		    </HeaderTemplate>
		    <ItemTemplate>
		        <b><%#DataBinder.Eval(Container.DataItem, "Period")%>-<%#DataBinder.Eval(Container.DataItem, "Year")%></b>
		    </ItemTemplate>
		</asp:TemplateField>

        <asp:BoundField SortExpression="DayspastDue" DataField="DayspastDue" HeaderText="Days Past Due"></asp:BoundField>
        <asp:BoundField SortExpression="AmountDue" DataField="AmountDue" HeaderText="AmountDue"></asp:BoundField>
        
		<asp:BoundField SortExpression="SUBMITTEDDATE" DataField="SUBMITTEDDATE" HeaderText="Email Sent DATE  "></asp:BoundField>
        
        <asp:BoundField  DataField="Email" HeaderText="EmailSentto "  ></asp:BoundField>
		</Columns>
</CC:GridView>

</asp:content>
