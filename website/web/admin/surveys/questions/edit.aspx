<%@ Page Language="VB" ValidateRequest="False" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Admin_Survey_Question_Edit"  Title="Survey Question"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<script src="/includes/XmlHttpLookup.js" type="text/javascript"></script>
<script>

function AJAXQuestion() {
	var sItem, sConn;
	xml = getXMLHTTP();
	if(xml){
		xml.open("GET","/admin/ajax/surveys.aspx?f=QuestionTypeProperties&QuestionTypeId=" + document.getElementById('<%=drpQuestionTypeId.ClientId %>').value,true);
		xml.onreadystatechange = function() {
			if(xml.readyState==4 && xml.responseText) {
		    	if (xml.responseText.length > 0) {
					QuestionTypeChangeCallBack(xml.responseText);				
				} else {
					QuestionTypeChangeCallBack('');
				}	
			}
		}	
		xml.send(null);
	}
	
}	
function QuestionTypeChangeCallBack(sResult){
    
    document.getElementById('<%=btnChoices.ClientId %>').disabled = true;
    document.getElementById('<%=btnCategories.ClientId %>').disabled = true;
    document.getElementById('<%=btnDemographic.ClientId %>').disabled = true;
    
    var aData = sResult.split('|');
    if (aData[0] == '1'){
        document.getElementById('<%=btnChoices.ClientId %>').disabled = false;
    }
    if (aData[1] == '1'){
        document.getElementById('<%=btnCategories.ClientId %>').disabled = false; 
    }
    if (aData[2] == '1'){
        document.getElementById('<%=btnDemographic.ClientId %>').disabled = false;
    }
    
}
function QuestionTypeChange(){    
    AJAXQuestion();
    return;
}
</script>
	
<h4><% If QuestionId = 0 Then %>Add<% Else %>Edit<% End If %> Survey Question</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Question Type Id:</td>
		<td class="field"><asp:DropDownList id="drpQuestionTypeId" runat="server" /> 
		<CC:OneClickButton ID="btnCategories" runat="Server" Text="Save & Edit Categories" CssClass="btn" />
		&nbsp;
		<CC:OneClickButton ID="btnChoices" runat="Server" CssClass="btn" Text="Save & Edit Choices" />
				&nbsp;
		<CC:OneClickButton ID="btnDemographic" runat="Server" CssClass="btn" Text="Save & Select Demographics" /></td>
		<td><asp:RequiredFieldValidator ID="rfvQuestionTypeId" runat="server" Display="Dynamic" ControlToValidate="drpQuestionTypeId" ErrorMessage="Field 'Question Type Id' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Name:</td>
		<td class="field"><asp:textbox id="txtName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName" ErrorMessage="Field 'Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Text:</td>
		<td class="field"><CC:CKeditor id="txtText" runat="server" Width="600" Height="300" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Is Required:</td>
		<td class="field"><asp:CheckBox ID="chkIsRequired" runat="Server" /></td>
		<td></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Survey Question?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
