<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Document" CodeFile="recipient.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<SCRIPT LANGUAGE="JavaScript">
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

function SetAll(val) {
    for (var el = 0; el < document.forms[0].length;  el++) {
        if (document.forms[0][el].name.indexOf('chkSelect') >= 0) {
            document.forms[0][el].checked = val;
        }
    }
}
</script>


<h4>Add Document Recipients</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="0" cellspacing="2" border="0">
    <tr valign="top">
        <td>
            <table cellpadding="2" cellspacing="2" width="100%">
            <tr>
            <th valign="top">Audience:</th>
            <td valign="top" class="field">
                <asp:RadioButtonList id="F_Audience" runat="server">
                   <asp:ListItem selected="true">Builder</asp:ListItem>
                   <asp:ListItem>Vendor</asp:ListItem>
                   <asp:ListItem>PIQ</asp:ListItem>
                   <asp:ListItem>All</asp:ListItem>
                </asp:RadioButtonList>
            </td>
            </tr>
            <tr>
            <th valign="top">LLC:</th>
            <td valign="top" class="field">
                <CC:CheckBoxListEx ID="F_LLC" runat="server" RepeatColumns="2" RepeatDirection="Vertical"></CC:CheckBoxListEx>
            </td>
            </tr>
            <tr>
            <th valign="top">Company Name:</th>
            <td valign="top" class="field">
                <asp:textbox id="F_CompanyName" runat="server" Columns="50" MaxLength="50"></asp:textbox>
            </td>
            </tr>
            </td>
            </tr>
            </table>
        </td>
        <td>
            <table cellpadding="2" cellspacing="2" border="0" width="100%">
                <tr>
                <th valign="top">Supply Phase:</th>
                <td valign="top" class="field">
                    <CC:CheckBoxListEx ID="F_SupplyPhase" runat="server" RepeatColumns="3" RepeatDirection="Vertical">
                    </CC:CheckBoxListEx>
                    <input type="button" name="btnCheckAll" value="Check All" onClick="setState('<%= F_SupplyPhase.ClientID %>',true);"/>
                    <input type="button" name="btnUnCheckAll" value="Uncheck All" onClick="setState('<%= F_SupplyPhase.ClientID %>',false);"/>
                </td>
                </tr>
            </table>    
        </td>
    </tr>
    <tr>
        <td colspan="2" align="center">
        <CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
        <input class="btn" type="submit" value="Clear" onclick="window.location='recipient.aspx?AdminDocumentID=<%=m_AdminDocumentID %>';return false;" />
        </td>
    </tr>
</table>
</asp:Panel>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="false" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
	     <asp:TemplateField HeaderText="Select">
            <ItemTemplate>
                <asp:CheckBox ID="chkSelect" Checked="false"  runat="server" />
            </ItemTemplate>
        </asp:TemplateField> 
		<asp:BoundField DataField="ID" HeaderText="ID"></asp:BoundField>
		<asp:BoundField DataField="Audience" HeaderText="Audience Type" SortExpression="Audience"></asp:BoundField>
		<asp:BoundField SortExpression="CompanyName" DataField="CompanyName" HeaderText="Company Name"></asp:BoundField>
          <asp:TemplateField HeaderText = "Market(LLC)" >
        <ItemTemplate >
        <asp:Literal ID="LLCName" runat ="server" ></asp:Literal>
        </ItemTemplate>
        </asp:TemplateField>
	</Columns>
</CC:GridView>
<asp:Button id="btnSelectAll" runat="server" Text="Check All" onclientclick="SetAll(true);return false;" />
<asp:Button ID="btnUnSelectAll" runat="server" Text="Uncheck All" OnClientClick="SetAll(false); return false;" />
<p></p>
<CC:OneClickButton id="btnAddRecipients" Runat="server" Text="Update Recpients on this Page" cssClass="btn" />
<CC:ConfirmButton id="btnClearAll" runat="server" cssclass="btn" text="Clear All Recipients" Message="Are you sure you want to remove this document from all recipients?"></CC:ConfirmButton>
</asp:content>
