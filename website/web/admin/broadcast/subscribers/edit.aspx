<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Mailing Member"%>
	
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If MemberId = 0 Then %>Add<% Else %>Edit<% End If %> Mailing Member</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class="red">red color</span> - denotes required fields</td></tr>
	<tr>
		<td class="required">Email:</td>
		<td class="field"><asp:textbox id="txtEmail" runat="server" maxlength="255" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvEmail" runat="server" Display="Dynamic" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Name:</td>
		<td class="field"><asp:textbox id="txtName" runat="server" maxlength="255" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">E-mail format:</td>
		<td class="field"><asp:RadioButtonList ID="rblMimeType" runat="server" RepeatDirection="Horizontal">
            <asp:ListItem Value="HTML">HTML</asp:ListItem>
            <asp:ListItem Value="TEXT">Plain Text</asp:ListItem>
        </asp:RadioButtonList></td>
		<td><asp:RequiredFieldValidator ID="rfvMimeType" runat="server" Display="Dynamic" ControlToValidate="rblMimeType" ErrorMessage="Field 'E-mail format' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Lists:</td>
		<td class="field"><CC:CheckBoxListEx ID="cblLists" runat="server" RepeatColumns="2"></CC:CheckBoxListEx></td>
		<td></td>
	</tr>
		<tr>
		<td class="optional">Bounce Report:</td>
		<td class="field">
		
		<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" AllowPaging="False" AllowSorting="True"  AutoGenerateColumns="False" BorderWidth="0" EmptyDataText="No email has been sent to this email address yet" >
    	<AlternatingRowStyle CssClass="alternate"></AlternatingRowStyle>
	    <RowStyle CssClass="row"></RowStyle>
	    <Columns>
		    <asp:BoundField DataField="SentDate" HeaderText="Date/Time"></asp:BoundField>
		    <asp:BoundField DataField="Name" HeaderText="E-mail Name"></asp:BoundField>
		    <asp:BoundField DataField="NumBounces" HeaderText="Bounces"></asp:BoundField>
	    </Columns>
        </CC:GridView>

		</td>
		<td></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Mailing Member?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
