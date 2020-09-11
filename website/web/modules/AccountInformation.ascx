<%@ Control EnableViewstate="False" Language="VB" AutoEventWireup="false" CodeFile="AccountInformation.ascx.vb" Inherits="AccountInformation" %>
<asp:Panel ID="pAccountInfo" runat="server" DefaultButton="btnSave">
<div class="cblock10">

    <img src="" alt="" ID="imgLogo" style="margin-left:-10px;padding-bottom:5px" runat="server" Visible="false" />
	<p>Welcome back,<br />
	<strong><asp:Literal ID="ltdUserName" runat="server" /></strong></p>

	<h3><asp:Literal ID="ltdCompanyName" runat="server" /></h3>

	<p><div class="aiaddresstext" id="divAddress" runat="server" /></p>
	<div class="aieditlink" id="divlink" runat="server">
        <a id="lnkEditLink2" onclick="javascript:ShowHideEditMenu();" class="aieditlink" href="javascript:void(0);">[Edit]<i class="fa fa-pencil" aria-hidden="true" style="padding-left:3px;"></i></a>
	    <div style="float:right;font-size:14px">
            <a id="lnkEditLink" runat="server" href="javascript:void(0);"><i class="fa fa-user-circle-o" aria-hidden="true" style="padding-left:3px;"></i></a>
        </div>
    </div>
    <asp:HiddenField ID="hdnAccountType" runat="server" value="" ClientIDMode="Static" />
</div>
<div id="divEditMenuDropdown" style="display:none;position:absolute;background-color:#e0e0e0;left:173px;width:215px;border:solid 1px;">
    <div id="divBuilderEditMenu">
        <table cellspacing="7" cellpadding="0">
            <tr>
                <td>
                    <i class="fa fa-info-circle" aria-hidden="true" style="padding-left:3px;"></i>
                    <a href="/forms/builder-registration/default.aspx">Account</a>
                </td>
                <td>
                    <i class="fa fa-credit-card" aria-hidden="true" style="padding-left:3px;"></i>
                    <a href="/builder/update.aspx">Credit Card</a>
                </td>
                <td>
                    <i class="fa fa-users" aria-hidden="true" style="padding-left:3px;"></i>
                    <a href="/builder/users.aspx">Users</a>
                </td>
            </tr>
        </table>
    </div>
    
    <div id="divVendorEditMenu">
        <table cellspacing="7" cellpadding="0">
            <tr>
                <td>
                    <i class="fa fa-info-circle" aria-hidden="true" style="padding-left:3px;"></i>
                    <a href="/forms/vendor-registration/default.aspx">Account</a>
                </td>
                <td>
                    <i class="fa fa-home" aria-hidden="true" style="padding-left:3px;"></i>
                    <a href="/forms/vendor-registration/companyprofile.aspx">Company Profile</a>
                </td>
            </tr>
            <tr>
                <td>
                    <i class="fa fa-trello" aria-hidden="true" style="padding-left:3px;"></i>
                    <a href="/forms/vendor-registration/register.aspx">Directory</a>
                </td>
                <td>
                    <i class="fa fa-users" aria-hidden="true" style="padding-left:3px;"></i>
                    <a href="/forms/vendor-registration/users.aspx">Users &amp; Roles</a>
                </td>
            </tr>
            <tr>
                <td>
                    <i class="fa fa-percent" aria-hidden="true" style="padding-left:3px;"></i>
                    <a href="/rebates/terms.aspx">Rebate Term</a>
                </td>
                <td>
                    <i class="fa fa-check-square-o" aria-hidden="true" style="padding-left:3px;"></i>
                    <a href="/forms/vendor-registration/supplyphase.aspx">Supply Phases</a>
                </td>
            </tr>
        </table>
    </div>    
</div>

<div id="divAccountEdit" runat="server" class="window" style="border:1px solid #000;background-color:#fff;width:950px;z-index:20;">
    <div class="pckghdgred" >Edit Account Information</div>
    <div class="aimain">
    <table class="tblcompr" style="width:0px">
    <tr><td valign="top" style="width:45%;">
        <table>
             <tr><th colspan="3">User Information</th></tr>
             <tr>
		        <td class="required"> First Name:</td>
		        <td class="field"><asp:textbox id="txtFirstName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		        <td><asp:RequiredFieldValidator ID="rfvFirstName" ValidationGroup="Account Information"  runat="server" Display="Dynamic" ControlToValidate="txtFirstName" ErrorMessage="Field 'First Name' is blank"></asp:RequiredFieldValidator></td>
	        </tr>
	         <tr>
		        <td class="required">Last Name:</td>
		        <td class="field"><asp:textbox id="txtLastName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		        <td><asp:RequiredFieldValidator ID="rfvLastName" ValidationGroup="Account Information"  runat="server" Display="Dynamic" ControlToValidate="txtLastName" ErrorMessage="Field 'Last Name' is blank"></asp:RequiredFieldValidator></td>
	        </tr>
	         <tr>
		        <td class="required">Phone:</td>
		        <td class="field"><asp:textbox id="txtPhone" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		        <td><asp:RequiredFieldValidator ID="rfvPhone" runat="server" ValidationGroup="Account Information"  Display="Dynamic" ControlToValidate="txtPhone" ErrorMessage="Field 'Phone' is blank"></asp:RequiredFieldValidator></td>
	        </tr>
	        <tr>
		        <td class="optional">Mobile:</td>
		        <td class="field"><asp:textbox id="txtMobile" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		        <td></td>
	        </tr>
	        <tr>
		        <td class="optional">Fax:</td>
		        <td class="field"><asp:textbox id="txtFax" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		        <td></td>
	        </tr>
	        <tr>
		        <td class="required">Email:</td>
		        <td class="field"><asp:textbox id="txtEmail" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		        <td><asp:RequiredFieldValidator ValidationGroup="Account Information"  ID="rfvEmail" runat="server" Display="Dynamic" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is blank"></asp:RequiredFieldValidator><CC:EmailValidator Display="Dynamic" runat="server" id="fvEmail" ControlToValidate="txtEmail" ValidationGroup="Account Information"  ErrorMessage="Field 'Email' is invalid" /></td>
	        </tr>
            <tr><th colspan="3">Login Information</th></tr>
             <tr>
                <td></td>
                <td class="field" colspan=2>
                <span id="Span1" style="width: 319px;" runat="server">If you want to change your username, enter a new one.</span> 
                </td>
            </tr>
             <tr>
		        <td class="required">UserName:</td>
		        <td class="field"><asp:textbox id="txtUserName" runat="server" onchange="CheckAvailability();" maxlength="20" columns="20" style="width: 319px;"></asp:textbox></td>    
	        </tr>
             <tr>
                <td class="fieldpad" colspan=2><div id="divAvailability"></div></td>
            </tr>
              <tr>
                <td></td>
                <td class="field" colspan=2>
                <span id="psMsg" style="width: 319px;" runat="server">If you want to change the password for the user, please enter data below, otherwise please leave password fields blank.</span> 
                </td>
            </tr>		

            <tr>
		<td class="required">Password:</td>
		<td class="field"><asp:textbox runat="server" id="txtPassword" style="width: 319px;" Columns="20" TextMode="Password" />
		<asp:RegularExpressionValidator id="fvPassword" Runat="server" ControlToValidate="txtPassword" Display="Dynamic" ValidationExpression="[A-Za-z0-9]{6,}" ErrorMessage="Password must contain minimum 6 alphanumeric characters"></asp:RegularExpressionValidator></td>
	</tr>
	<tr>
		<td class="required"><b>Password confirmation:</b></td>
		<td class="field"><asp:textbox runat="server" id="txtPasswordVerify" style="width: 319px;" Columns="20" TextMode="Password" /> 
		 <asp:CompareValidator ID="rfvPasswordCompareVerify" Runat="server" ValidationGroup="Account Information" ControlToCompare="txtPassword" ControlToValidate="txtPasswordVerify" Operator="Equal" Display="Dynamic" ErrorMessage="Password and Re-typed passwod don't match" />
         
         </td>
	</tr>
             	        
	        </table>
	        </td>
	        <td valign="top">
	        <table>
	        <tr> <th colspan="3">
            <asp:Label runat = "server" ID = "lblHeader" /></th></tr>	       
	        <tr>
		        <td class="required">Company Name:</td>
		        <td class="field"><asp:textbox id="txtCompanyName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		        <td><asp:RequiredFieldValidator ValidationGroup="Account Information"  ID="rfvCompanyName" runat="server" Display="Dynamic" ControlToValidate="txtCompanyName" ErrorMessage="Field 'Company Name' is blank"></asp:RequiredFieldValidator></td>
	        </tr>
	        <tr>
		        <td class="required"> Address:</td>
		        <td class="field"><asp:textbox id="txtAddress" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		        <td><asp:RequiredFieldValidator ValidationGroup="Account Information"  ID="rfvAddress" runat="server" Display="Dynamic" ControlToValidate="txtAddress" ErrorMessage="Field 'Address' is blank"></asp:RequiredFieldValidator></td>
	        </tr>
	        <tr>
		        <td class="optional"> Address 2:</td>
		        <td class="field"><asp:textbox id="txtAddress2" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		        <td></td>
	        </tr>
	        <tr>
		        <td class="required">City:</td>
		        <td class="field"><asp:textbox id="txtCity" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		        <td><asp:RequiredFieldValidator ID="rfvCity" ValidationGroup="Account Information"  runat="server" Display="Dynamic" ControlToValidate="txtCity" ErrorMessage="Field 'City' is blank"></asp:RequiredFieldValidator></td>
	        </tr>
	        <tr>
		        <td class="required">State:</td>
		        <td class="field"><asp:textbox id="txtState" runat="server" maxlength="2" columns="2" style="width: 31px;"></asp:textbox></td>
		        <td><asp:RequiredFieldValidator ID="rfvState" ValidationGroup="Account Information"  runat="server" Display="Dynamic" ControlToValidate="txtState" ErrorMessage="Field 'State' is blank"></asp:RequiredFieldValidator></td>
	        </tr>
	        <tr>
		        <td class="required">Zip:</td>
		        <td class="field"><asp:textbox id="txtZip" runat="server" maxlength="15" columns="15" style="width: 109px;"></asp:textbox></td>
		        <td><asp:RequiredFieldValidator ValidationGroup="Account Information"  ID="rfvZip" runat="server" Display="Dynamic" ControlToValidate="txtZip" ErrorMessage="Field 'Zip' is blank"></asp:RequiredFieldValidator></td>
	        </tr>
	        <tr>
		        <td class="required"> Phone:</td>
		        <td class="field"><asp:textbox id="txtCompanyPhone" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		        <td><asp:RequiredFieldValidator ID="rfvCompanyPhone" runat="server" ValidationGroup="Account Information"  Display="Dynamic" ControlToValidate="txtCompanyPhone" ErrorMessage="Field 'Company Phone' is blank"></asp:RequiredFieldValidator></td>
	        </tr>
	        
	        <tr>
		        <td class="optional"> Fax:</td>
		        <td class="field"><asp:textbox id="txtCompanyFax" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		        <td></td>
	        </tr>
	        <tr>
	            <td class="required">Company Email:</td>
	            <td class="field"><asp:TextBox ID="txtCompanyEmail" runat="server" MaxLength="50" Columns="50" style="width:319px;"></asp:TextBox></td>
	            <td><asp:RequiredFieldValidator ID="rfvtxtCompanyEmail" runat="server" ValidationGroup="Account Information" ControlToValidate="txtCompanyEmail" ErrorMessage="Email is required"></asp:RequiredFieldValidator></td>
	        </tr>
	        <tr>
		        <td class="optional">Website URL:</td>
		        <td class="field"><asp:textbox id="txtWebsiteURL" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox><br/><span class="smaller">http:// or https:// are required</span></td>
		        <td><CC:URLValidator  ValidationGroup="Account Information" Display="Dynamic" runat="server" id="lnkvWebsiteURL" ControlToValidate="txtWebsiteURL" ErrorMessage="Link 'Website URL' is invalid" /></td>
	        </tr>
	        <tr id="trLogo" runat="server">
		        <td class="optional">Logo:</td>
		        <td class="field"><CC:FileUpload ID="fuLogoFile" DisplayImage="True" runat="server" style="width: 319px;" /></td>
		        <td><CC:FileUploadExtensionValidator Extensions="jpg,jpeg,gif,bmp,png" ID="feLogoFile" runat="server" Display="Dynamic" ControlToValidate="fuLogoFile" ErrorMessage="Invalid file extension"></CC:FileUploadExtensionValidator></td>
	        </tr>
        </table>
        </td>
        </tr>
        </table>
        <p></p>
<div style="margin-bottom:10px">
        <asp:Button id="btnSave" ValidationGroup="Account Information"  runat="server" cssclass="btnred" text="Save" />
        <asp:Button id="btnCancel" runat="server" cssclass="btnred" text="Close" onClientClick="refresh();" />
</div>
    </div>
</div>
<CC:DivWindow ID="ctrlAccountEdit" runat="server" TargetControlID="divAccountEdit" TriggerId="lnkEditLink" MoveToTop="true"  ShowVeil="true" VeilCloses="false" /><%--CloseTriggerId="btnCancel"--%>
</asp:Panel>
<script type="text/javascript">
    $(document).ready(function () {
        var AccType = document.getElementById("hdnAccountType").value;

        if (AccType == "B") {
            document.getElementById("divVendorEditMenu").style.display = "none";
            document.getElementById("divBuilderEditMenu").style.display = "block";
        } else {
            if (AccType == "V") {
                document.getElementById("divBuilderEditMenu").style.display = "none";
                document.getElementById("divVendorEditMenu").style.display = "block";
            }
        }
    });

    function CheckAvailability() {
        var txtUsername = document.getElementById('<%=txtUserName.ClientId %>');
        var divAvailablity = document.getElementById('divAvailability');
        var btnsave = document.getElementById('<%=btnSave.ClientID%>');
        var xml = getXMLHTTP();
        if (xml) {
            xml.open("GET", "/ajax.aspx?f=CheckAvailbility&BuilderAccountId=<%=BuilderAccountId%>&Username=" + txtUsername.value, true);
            xml.onreadystatechange = function () {
                if (xml.readyState == 4 && xml.responseText) {
                    if (xml.responseText.length > 0) {
                        var sUsername = txtUsername.value
                        sUsername = sUsername.replace(/</g, "&lt;");
                        sUsername = sUsername.replace(/>/g, "&gt;");
                        if (xml.responseText == 'OK') {
                            divAvailablity.innerHTML = '<span style=\'color:green;font-weight:bold;\'>Username ' + sUsername + ' is available. <br>It will be yours when you click the save button.</br></span>';
                            btnsave.disabled = false;
                        } else {
                            btnsave.disabled = true;
                            divAvailablity.innerHTML = '<span style=\'color:red;font-weight:bold;\'>Username ' + sUsername + ' is already taken. Please try again.</span>';
                        }
                    } else {
                        divAvailablity.innerHTML = '<span style=\'color=red\'>Availability check is disabled for this version of the browser</span>';
                    }
                }
            }
            xml.send(null);
        } else {
            divAvailablity.innerHTML = '<span style=\'color=red\'>Availability check is disabled for this version of the browser</span>';
        }
    }

    function refresh() {
           var txtUsername = document.getElementById('<%=txtUserName.ClientId %>');
        var divAvailablity = document.getElementById('divAvailability');
        divAvailablity.style.visibility = false;
        self.close;
    }

    function ShowHideEditMenu() {
        var DivDisplay = document.getElementById('divEditMenuDropdown').style.display;

        if (DivDisplay == "none") {
            document.getElementById('divEditMenuDropdown').style.display = "block";
        } else {
            document.getElementById('divEditMenuDropdown').style.display = "none";
        }

        return false;
    }
    </script>