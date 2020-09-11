<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Admin Document"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	<SCRIPT LANGUAGE="JavaScript">
	    function setState(field, state) {
	        var chkBoxList = document.getElementById(field);
	        var chkBoxCount = chkBoxList.getElementsByTagName("input");
	        for (var i = 0; i < chkBoxCount.length; i++) {
	            chkBoxCount[i].checked = state;
	        }

	        return false;
	    }

	    function SetAll(val) {
	        for (var el = 0; el < document.forms[0].length; el++) {
	            if (document.forms[0][el].name.indexOf('chkSelect') >= 0) {
	                document.forms[0][el].checked = val;
	            }
	        }
	    }
</script>
<h4><% If AdminDocumentID = 0 Then %>Add<% Else %>Edit<% End If %> Admin Document</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Title:</td>
		<td class="field"><asp:textbox id="txtTitle" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvTitle" runat="server" Display="Dynamic" ControlToValidate="txtTitle" ErrorMessage="Field 'Title' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Uploaded:</td>
		<td class="field"><asp:Literal id="ltlUploaded" runat="server"></asp:Literal></td>
		<td></td>
	</tr>
	<tr>
		<td class="required"><b>Is Active?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsApproved" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvIsApproved" runat="server" Display="Dynamic" ControlToValidate="rblIsApproved" ErrorMessage="Field 'Is Approved' is blank"></asp:RequiredFieldValidator></td>
	</tr>

    <tr>
<th valign="top">Audience:</th>
<td valign="top" class="field">
    <CC:CheckBoxListEx ID="cblAudienceType" runat="server">
        </CC:CheckBoxListEx>
    </td> 
    <td>
    
    </td>
</tr>

 <tr>
            <th valign="top">LLC:</th>
            <td valign="top" class="field">
                <CC:CheckBoxListEx ID="F_LLC" runat="server" RepeatColumns="2" RepeatDirection="Vertical"></CC:CheckBoxListEx>
                 <input type="button" name="btnCheckAll" value="Check All" onclick="setState('<%= F_LLC.ClientID %>',true);"/>
                    <input type="button" name="btnUnCheckAll" value="Uncheck All" onclick="setState('<%= F_LLC.ClientID %>',false);"/>
            </td>
            <td>
            
            </td>
            </tr>
 <tr style ="display :none">
		<td class="optional">Document Audience History:</td>
		<td class="field"><asp:TextBox id="txtDocumentHistoryNotes" style="width: 349px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton ID="btnRecipients"  Visible = "false"  runat="server" Text="Edit Recipients" CssClass="btn" />
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Admin Document?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
