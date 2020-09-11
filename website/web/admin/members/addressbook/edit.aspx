<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Inherits="member_addressbook_edit" CodeFile="edit.aspx.vb" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

   <h4><%=Request("mode")%> Address Book Entry</h4>
 		    <table cellspacing="2" cellpadding="0" border="0" style="margin-left:6px" summary="billing">            
			    	  
			    <tr>
			    <td colspan="2"><span class="smallest"><span class="red">red color-</span>denotes required field</span></td>
			   </tr>

			    <tr>
			    <td class="required" style="width:130px;"><span id="labeltxtLabel" runat="server">Label</span></td>
			    <td class="field"><asp:textbox id="txtLabel" runat="server" style="width:200px;" />
				 <div class="smallest" style="color:#999999; font-weight:normal;">(specify a short name for quick reference)</div>
			    </td>
			    <td><asp:RequiredFieldValidator ID="rfvtxtLabel" runat="server" Display="Dynamic" ControlToValidate="txtLabel" ErrorMessage="Field 'Label' is blank"  /></td>
			    </tr>
    						
			    <tr>
			    <td class="required" style="width:130px;"><span id="labeltxtFirstName" runat="server">First Name</span></td>
			    <td class="field"><asp:textbox id="txtFirstName" runat="server" style="width:200px;" /><br /></td>
			    <td><asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" ErrorMessage="Field 'First Name' is blank" Display="Dynamic" ></asp:RequiredFieldValidator> </td>
				 </tr>

			    <tr>
			    <td class="required" style="width:130px;"><span id="labeltxtLastName" runat="server">Last Name</span></td>
			    <td class="field"><asp:textbox id="txtLastName" runat="server" style="width:200px;" /><br /></td>
			    <td><asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" ErrorMessage="Field 'Last Name' is blank" Display="Dynamic" /></td>
				</tr>

			    <tr>
			    <td class="optional" style="width:130px;">Company/School</td>
			    <td class="field"><asp:textbox id="txtCompany" runat="server" style="width:200px;" /><br /></td>
				</tr>

			    <tr>
			    <td class="required" style="width:130px;"><span id="labeltxtAddress1" runat="server">Address 1</span></td>
			    <td class="field"><asp:textbox id="txtAddress1" maxlength="30" runat="server" style="width:200px;" /><br /></td>
			    <td> <asp:RequiredFieldValidator ID="rfvAddress1" runat="server" ControlToValidate="txtAddress1" ErrorMessage="Field 'Address 1' is blank" Display="Dynamic"  /></td>
				</tr>

			    <tr>
			    <td class="optional" style="width:130px;">Address 2</td>
			    <td class="field"><asp:textbox id="txtAddress2" maxlength="30" runat="server" style="width:200px;" /><br /></td>
			    </tr>

			    <tr>
			    <td class="required" style="width:130px;"><span id="labeltxtCity" runat="server">City</span></td>
			    <td class="field"><asp:textbox id="txtCity" runat="server" style="width:200px;" /><br /></td>
			    <td><asp:RequiredFieldValidator ID="rfvCity" runat="server" ControlToValidate="txtCity" ErrorMessage="Field 'City' is blank" Display="Dynamic"  /></td>
				</tr>

			    <tr>
			    <td class="required" style="width:130px;"><span id="labeldrpState" runat="server">State</span></td>
			    <td class="field">
				    <asp:dropdownlist id="drpState" runat="server" />
			    </td>
			    <td><CC:RequiredFieldValidatorFront ID="rfvState" runat="server" ControlToValidate="drpState" ErrorMessage="Field 'State' is blank" /></td>
			    </tr>

			    <tr>
			    <td class="required" style="width:130px;"><span id="labeltxtZip" runat="server">ZIP/Postal Code</span></td>
			    <td class="field"><asp:textbox id="txtZip" runat="server" style="width:100px;" /><br /></td>
			    <td><asp:RequiredFieldValidator ID="rfvZip" runat="server"  ControlToValidate="txtZip" ErrorMessage="Field 'Zip' is blank" Display="Dynamic"  /></td>
			    </tr>

			    <tr>
			    <td class="optional" style="width:130px;">Province/Region
    			    <div class="smallest" style="color:#999999; font-weight:normal;">(if applicable)</div>
			    </td>
			    <td class="field"><asp:textbox id="txtRegion" runat="server" style="width:200px;" /><br /></td>
			    <td><asp:RequiredFieldValidator ID="rfvRegion" runat="server"  EnableClientScript="False"  ControlToValidate="txtRegion" ErrorMessage="Field 'Region' is blank" Display="Dynamic"  /></td>
			    </tr>


			    <tr>
			    <td class="required" style="width:130px;"><span id="labeldrpCountry" runat="server">Country</span></td>
			    <td class="field">
				    <asp:dropdownlist id="drpCountry" runat="server" />
			    </td>
			    <td><asp:RequiredFieldValidator ID="rfvCountry" runat="server"  ControlToValidate="drpCountry" ErrorMessage="Field 'Country' is blank" Display="Dynamic"  /></td>
			    </tr>

			    <tr>
			    <td class="required" style="width:130px;"><span id="labeltxtPhone" runat="server">Phone:</span></td>
			    <td class="field"><asp:textbox id="txtPhone" runat="server" style="width:200px;" /><br /></td>
			    <td><asp:RequiredFieldValidator ID="rfvPhone" runat="server" ControlToValidate="txtPhone" ErrorMessage="Field 'Phone' is blank" Display="Dynamic" /></td>
			    </tr>          
			    </table>
    			
            <p></p>
            <CC:OneClickButton id="btnSave" runat="server" Text="Save" CssClass="btn" />
            <CC:OneClickButton id="btnDelete" CausesValidation ="False" runat="server" Text="Delete" CssClass="btn" />
            <CC:OneClickButton id="btnCancel" CausesValidation="false" runat="server" Text="Cancel" CssClass="btn" />
     

</asp:content>