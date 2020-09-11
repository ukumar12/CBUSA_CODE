<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Product Type Attribute"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<asp:ScriptManager ID="AjaxManager" runat="server"></asp:ScriptManager>
<script type="text/javascript">
<!--
    function InputTypeChanged() {
        var el=$get('<%=drpInputType.ClientId %>');
        var tr=$get('<%=trOptions.ClientId %>');
        var txt=$get('<%=txtDefaultValue.ClientId %>');
        var drp=$get('<%=drpDefaultValue.ClientId %>');
        var rad=$get('<%=divRadioDefault.ClientId %>');
        if(el.options[el.selectedIndex].value == '<%=DataLayer.InputType.Dropdown %>') {
            tr.style.display='';
            txt.style.display='none';
            drp.style.display='';
            rad.style.display='none';
        } else if(el.options[el.selectedIndex].value == '<%=DataLayer.InputType.YesNo %>') {
            tr.style.display='none';
            txt.style.display='none';
            drp.style.display='none';
            rad.style.display='';
        } else {
            tr.style.display='none';
            txt.style.display='';
            drp.style.display='none';
            rad.style.display='none';
        }      
    }
    function OnSubformRowAdded(sender,args) {
        var el=sender.get_element();
        var drp=$get('<%=drpDefaultValue.ClientId %>');
        while(drp.options.length > 0) 
            drp.options.remove(drp.options[0]);
        for(var i=1;i<el.rows.length;i++) {
            var value=sender.get_value(i,'ValueOption');
            if(value) {
                var opt=document.createElement('option');
                opt.value = value;
                opt.text = value;
                drp.options.add(opt);
            }
        }
    }
    function HideDefault() {
        var tr=$get('<%=trDefault.ClientId %>');
        tr.style.display='none';
    }
    function ShowDefault() {
        var tr=$get('<%=trDefault.ClientId %>');
        tr.style.display='';
    }
//-->
</script>
	
<h4><% If ProductTypeAttributeID = 0 Then %>Add<% Else %>Edit<% End If %> Product Type Attribute</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Product Type:</td>
		<td class="field"><asp:DropDownList id="drpProductTypeID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvProductTypeID" runat="server" Display="Dynamic" ControlToValidate="drpProductTypeID" ErrorMessage="Field 'Product Type' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Attribute Name:</td>
		<td class="field"><asp:textbox id="txtAttribute" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td>
		    <asp:RequiredFieldValidator ID="rfvAttribute" runat="server" Display="Dynamic" ControlToValidate="txtAttribute" ErrorMessage="Field 'Attribute Name' is blank"></asp:RequiredFieldValidator>
		    <asp:CustomValidator ID="cvUniqueName" runat="server" ControlToValidate="txtAttribute" Display="Dynamic" ErrorMessage="An attribute with this name already exists"></asp:CustomValidator>
        </td>
	</tr>
	<tr>
		<td class="required">Input Type:</td>
		<td class="field">
		    <asp:DropDownList ID="drpInputType" runat="server" onchange="InputTypeChanged();">
		    </asp:DropDownList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvInputType" runat="server" Display="Dynamic" ControlToValidate="drpInputType" ErrorMessage="Field 'Input Type' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr id="trOptions" runat="server">
	    <td class="required">Value Options</td>
	    <td class="field">
	        <CC:SubForm ID="ctrlOptions" runat="server" OnRowAdded="OnSubformRowAdded">
	            <Fields>
	                <CC:SubFormField ID="fldValue" runat="server" FieldName="ValueOption">
	                    <HtmlTemplate>
	                        <asp:TextBox ID="txtValue" runat="server" Columns="50" MaxLength="50" style="width:150px;"></asp:TextBox>
	                    </HtmlTemplate>
	                    <Inputs>
	                        <CC:SubFormInput ServerId="txtValue" IsDefaultFunction="function() {return isEmptyField(this)}" ValidateFunction="function() {return !isEmptyField(this)}" IsDataField="true" SetValueFunction="function(value) {this.value=value}"></CC:SubFormInput>
	                    </Inputs>
	                </CC:SubFormField>
	            </Fields>
	        </CC:SubForm>
	    </td>
	    <td></td>
	</tr>
	<tr>
		<td class="required"><b>Is Required?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsRequired" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" onclick="ShowDefault()" />
			<asp:ListItem Text="No" Value="False" Selected="True" onclick="HideDefault()" />
			</asp:RadioButtonList>
		</td>
	</tr>
	<tr id="trDefault" runat="server">
		<td class="optional">Default Value:</td>
		<td class="field">
		    <asp:textbox id="txtDefaultValue" runat="server" maxlength="50" columns="50" style="width: 319px;display:none;"></asp:textbox>
            <CC:NoValidateDropDownList ID="drpDefaultValue" runat="server" style="display:none;">
            </CC:NoValidateDropDownList>
            <div id="divRadioDefault" runat="server" style="display:none;">
                <asp:RadioButton ID="rbDefaultYes" runat="server" GroupName="DefaultValue" Text="Yes" />
                <asp:RadioButton ID="rbDefaultNo" runat="server" GroupName="DefaultValue" Text="No" />
            </div>
        </td>
		<td><asp:CustomValidator ID="cvDefault" runat="server" ControlToValidate="txtDefaultValue" ValidateEmptyText="true" ErrorMessage="Field 'Default Option' is invalid"></asp:CustomValidator></td>
	</tr>
	<tr>
		<td class="required"><b>Is Active?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsActive" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Product Type Attribute?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
