<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Message" CodeFile="recipient.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<SCRIPT LANGUAGE="JavaScript">
<!-- 	
// -->

<!-- Begin
function setState(field,state)
{
    var chkBoxList = document.getElementById(field);
    var chkBoxCount= chkBoxList.getElementsByTagName("input");
    for(var i=0;i<chkBoxCount.length;i++)
    {
        chkBoxCount[i].checked = state;
    }
   
    return false; 
}

//  End -->
</script>

<h4>Add Message Recipients</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Audience:</th>
<td valign="top" class="field">
    <asp:RadioButtonList id="F_Audience" runat="server" >
       <asp:ListItem selected="true" Value="Builder">Builder</asp:ListItem>
       <asp:ListItem Value="Vendor">Vendor</asp:ListItem>
       <asp:ListItem Value="PIQ">PIQ</asp:ListItem>
    </asp:RadioButtonList>
</td>
</tr>
<tr>
<th valign="top">Supply Phase:</th>
<td valign="top" class="field">
    <asp:CheckBoxList ID="F_SupplyPhase" runat="server">
    </asp:CheckBoxList>
    <input type="button" name="btnCheckAll" value="Check All" onClick="setState('<%= F_SupplyPhase.ClientID %>',true);"/>
    <input type="button" name="btnUnCheckAll" value="Uncheck All" onClick="setState('<%= F_SupplyPhase.ClientID %>',false);"/>
</td>
</tr>
</tr>
<tr>
<th valign="top">Company Name:</th>
<td valign="top" class="field">
    <asp:textbox id="F_CompanyName" runat="server" Columns="50" MaxLength="50"></asp:textbox>
</td>
</tr>
<tr>
<th valign="top">LLC:</th>
<td valign="top" class="field">
    <CC:SearchList ID="F_LLC" runat="server" Table="LLC" TextField="LLC" ValueField="LLCID" AllowNew="false" CssClass="searchlist" ViewAllLength="10"></CC:SearchList>
</td>
</tr>
</td>
</tr>
<tr>
<th valign="top">First Name:</th>
<td valign="top" class="field">
    <asp:textbox id="F_FirstName" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
</td>
</tr>
<tr>
<th valign="top">Last Name:</th>
<td valign="top" class="field">
    <asp:textbox id="F_LastName" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
</table>
</td>
</tr>
<tr>
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='recipient.aspx';return false;" />
</td>
</tr>
</table>
</asp:Panel>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
	     <asp:TemplateField HeaderText="Select">
            <ItemTemplate>
                <asp:CheckBox ID="chkSelect" Checked="false"  runat="server" />
            </ItemTemplate>
        </asp:TemplateField> 
		<asp:BoundField DataField="ID" HeaderText="ID"></asp:BoundField>
		<asp:BoundField DataField="Audience" HeaderText="Audience"></asp:BoundField>
		<asp:BoundField SortExpression="CompanyName" DataField="CompanyName" HeaderText="Company Name"></asp:BoundField>
		<asp:BoundField SortExpression="FirstName" DataField="FirstName" HeaderText="First Name"></asp:BoundField>
		<asp:BoundField SortExpression="LastName" DataField="LastName" HeaderText="Last Name"></asp:BoundField>
	</Columns>
</CC:GridView>
<CC:OneClickButton id="btnClearRecipients" Runat="server" Text="Clear Selected Recipients" cssClass="btn" />
<CC:OneClickButton id="btnAddRecipients" Runat="server" Text="Add Selected Recipients" cssClass="btn" />
</asp:content>
