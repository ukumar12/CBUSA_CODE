<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Navigation"%>
	
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If NavigationId = 0 Then %>Add<% Else %>Edit<% End If %> Navigation</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class="red">red color</span> - denotes required fields</td></tr>
	<tr>
		<td class="required" style="width:150px;">Title:</td>
		<td class="field" style="width:550px;" ><asp:textbox id="txtTitle" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvTitle" runat="server" Display="Dynamic" ControlToValidate="txtTitle" ErrorMessage="Field 'Title' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Target:</td>
		<td class="field">
		<asp:DropDownList id="drpTarget" runat="server">
		<asp:ListItem Value="" Text="Not Set (default)" />
		<asp:ListItem Value="_blank" Text="New Window" />
		<asp:ListItem Value="_top" Text="Topmost Window" />
		<asp:ListItem Value="_self" Text="Self Window" />
		<asp:ListItem Value="_parent" Text="Parent Window" />
		</asp:DropDownList>
		</td>
		<td></td>
	</tr>
	<tr>
		<td class="optional"><b>Internal Link?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkIsInternalLink" AutoPostback="True" /></td>
    </tr>
	<tr>
		<td class="optional"><b>Skip Sitemap?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkSkipSitemap" /></td>
    </tr>
	<tr>
		<td class="optional"><b>Skip Breadcrumb?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkSkipBreadcrumb" /></td>
    </tr>
</table>
    
<table id="tblInternal" border="0" cellspacing="1" cellpadding="2" runat="server">
	<tr>
		<td class="optional" style="width:150px;" >Page:</td>
		<td class="field" style="width:550px;" ><CC:DropDownListEx id="drpPageId" runat="server"></CC:DropDownListEx></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Additional<br />Parameters:</td>
		<td class="field"><asp:textbox id="txtParameters" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
</table>

<table id="tblExternal" border="0" cellspacing="1" cellpadding="2" runat="server">
	<tr>
		<td class="optional" style="width:150px;">URL:</td>
		<td class="field" style="width:550px;" ><asp:textbox id="txtURL" runat="server" maxlength="255" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Navigation?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

