  
<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="APV" CodeFile="APVQuarterlyReports.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
 
<h4>APV Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" runat="server">
<table cellpadding="2" cellspacing="2">
 <tr>
<th valign="top"><b>Builder:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_BuilderID" runat="server" /></td>
</tr>
 <tr>
	<th valign="top">LLC(s):</th>
	<td class="field"><CC:CheckBoxListEx ID="F_LLC" runat="server" RepeatColumns="3"></CC:CheckBoxListEx></td>
</tr>
 <tr>
<th valign="top"><b>Is Active:</b></th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_IsActive" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Value="1">Yes</asp:ListItem>
		<asp:ListItem Value="0">No</asp:ListItem>
	</asp:DropDownList>
</td>
</tr>

<tr>
<th valign="top"><b>Start Period Year:</b></th>
<td valign="top" class="field"> 
<CC:DropDownListEx ID="F_StartPeriodYear" runat="server">
                    </CC:DropDownListEx>

</td>
<th valign="top"><b>Start Period Quarter:</b></th>
<td valign="top" class="field"> 
<CC:DropDownListEx ID="F_StartPeriodQuarter" runat="server">
                    </CC:DropDownListEx>

</td>
</tr>

<tr>
<th valign="top"><b>End Period Year:</b></th>
<td valign="top" class="field"> 
<CC:DropDownListEx ID="F_EndPeriodYear" runat="server">
                    </CC:DropDownListEx>
</td>
<th valign="top"><b>End Quarter:</b></th>
<td valign="top" class="field">
<CC:DropDownListEx ID="F_EndPeriodQuarter" runat="server">
                    </CC:DropDownListEx>
 
</td>
</tr>

 
<tr>
<td colspan="2" align="right">
 
<CC:OneClickButton  id="btnExport" Runat="server" Text="Export" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='QtrComparisionByVendor.aspx';return false;" />
</td>
</tr>
</table>
</asp:Panel>
<p></p>

 <div runat="server" id="divDownload" visible="false">
<asp:HyperLink id="lnkDownload" runat="server">Download File</asp:HyperLink>
</div>

</asp:content>


