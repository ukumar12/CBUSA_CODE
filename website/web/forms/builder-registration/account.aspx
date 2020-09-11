<%@ Page Language="VB" AutoEventWireup="false" CodeFile="account.aspx.vb" Inherits="forms_builder_registration_account" %>

<CT:MasterPage ID="CTMain" runat="server">
<asp:PlaceHolder runat="server">    
<script type="text/javascript">
<!--	
	function CheckAvailability() {
        var txtUsername = document.getElementById('<%=txtUsername.clientId %>');
        var divAvailablity = document.getElementById('divAvailability');
		if (isEmptyField(txtUsername)) {
			divAvailablity.innerHTML = '<span style=\'color=red;font-weight:bold;\'>Username cannot be blank</span>';
			return;
		}

		var xml = getXMLHTTP();
		if(xml){
			xml.open("GET","/ajax.aspx?f=CheckAvailbility&VendorAccountId=<%=VendorAccountID %>&BuilderAccountId=<%=BuilderAccountId%>&Username=" + txtUsername.value,true);
			xml.onreadystatechange = function() {
				if(xml.readyState==4 && xml.responseText) {
					if (xml.responseText.length > 0) {
					    var sUsername = txtUsername.value
					    sUsername = sUsername.replace(/</g,"&lt;");
					    sUsername = sUsername.replace(/>/g,"&gt;");
						if (xml.responseText == 'OK') {
							divAvailablity.innerHTML = '<span style=\'color=green;font-weight:bold;\'>Username ' + sUsername + ' is available</span>';
						} else {
							divAvailablity.innerHTML = '<span style=\'color=#8F0407;font-weight:bold;\'>Username ' + sUsername + ' is already taken!</span>';
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
//-->
</script>
</asp:PlaceHolder> 
        
    <div class="pckggraywrpr" style="width:500px;margin:20px auto;">
        <div class="pckghdgred">Account Information:</div>
        <table cellpadding="2" cellspacing="2" border="0">
            <tr>
                <td>&nbsp;</td>
                <td class="fieldreq">&nbsp;</td>
                <td><span class="smaller"> indicates required field</span></td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeltxtFirstName" runat="server">First Name:</span></td>
                <td class="fieldreq" id="bartxtFirstName" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox ID="txtFirstName" runat="server" MaxLength="50" style="width:150px;" CssClass="regtxt"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeltxtLastName" runat="server">Last Name:</span></td>
                <td class="fieldreq" id="bartxtLastName" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox ID="txtLastName" runat="Server" MaxLength="50" style="width:150px;" CssClass="regtxt"></asp:TextBox></td>
            </tr>
            <tr id="trTitle" runat="server">
                <td class="fieldlbl"><span id="labeltxtTitle" runat="server">Title:</span></td>
                <td class="fieldreq" id="bartxtTitle" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox id="txtTitle" runat="server" maxlength="50" style="width:150px;" cssclass="regtext"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeltxtEmail" runat="server">Email:</span></td>
                <td class="fieldreq" id="bartxtEmail" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox ID="txtEmail" runat="server" MaxLength="100" style="width:250px;" CssClass="regtxt"></asp:TextBox></td>
            </tr>
	        <tr valign="top">
		        <td class="fieldlbl" style="width:130px;"><span id="labeltxtUsername" runat="server">Username</span></td>
		        <td class="fieldreq" id="bartxtUsername" runat="server">&nbsp;</td>
		        <td class="field"><asp:textbox id="txtUsername" runat="server" style="width:150px;" maxlength="20" /><input class="btnred" type="button" value="Check Availability" onclick="CheckAvailability();"></td>
	        </tr>

            <tr>
                <td></td>
                <td class="fieldpad" colspan=2><div id="divAvailability"></div></td>
            </tr>						

            <tr>
                <td></td>
                <td class="fieldpad" colspan=2>
                <span id="psMsg" runat="server">If you want to change the password for the user, please enter data below, otherwise please leave password fields blank.</span> 
                </td>
            </tr>			

	        <tr valign="top">
		        <td class="fieldlbl" style="width:130px;"><span id="labeltxtPassword" runat="server">Password</span></td>
		        <td class="fieldreq" id="bartxtPassword" runat="server">&nbsp;</td>
		        <td class="field"><asp:textbox id="txtPassword" TextMode="password" runat="server" style="width:150px;" maxlength="20" /></td>
	        </tr>
        		
	        <tr valign="top">
		        <td class="fieldlbl" style="width:130px;"><span id="labeltxtConfirmpassword" runat="server">Confirm Password</span></td>
		        <td class="fieldreq" id="bartxtConfirmpassword" runat="server">&nbsp;</td>
		        <td class="field"><asp:textbox id="txtConfirmpassword" TextMode="password" runat="server" style="width:150px;" maxlength="20" /><br /></td>
	        </tr>
        </table>
        <table cellspacing="2" cellpadding="2" border="0" style="text-align:left;">
            <tr>
                <td class="fieldlbl"><span id="labelcbTerms" runat="server">I have read and agreed to the End User License Agreement:</span></td>
                <td class="fieldreq" id="barcbTerms" runat="server" style="width:20px;">&nbsp;</td>
                <td class="field">
                    <asp:CheckBox ID="cbTerms" runat="server" onclick="OpenTerms();" />&nbsp;<asp:Button id="btnShowTerms" runat="server" text="View Terms" cssclass="btnblue" />
                    <CC:PopupForm ID="frmEULA" runat="server" OpenMode="MoveToCenter" OpenTriggerId="btnShowTerms" Animate="true" ShowVeil="true" VeilCloses="false" CssClass="pform" style="width:900px;">
                        <FormTemplate>
                            <div class="pckggraywrpr" style="margin:0px;">
                                <div class="pckghdgred">End User License Agreement</div>
                                <div style="padding:10px;background-color:#fff;height:600px;overflow:auto;">
                                    <h1 align="center">CUSTOM BUILDERS USA</h1>
                                    <h2 align="center">END USER LICENSE AGREEMENT</h2>
                                    <p style="line-height:14px;font-size:13px;">
                                        <p>
                                            <span style="font-size:14px;text-decoration:underline;">IMPORTANT</span> – THIS IS A LEGAL DOCUMENT.  BEFORE INSTALLING, ACCESSING, OR USING ANY PART OF THIS SYSTEM, YOU SHOULD READ CAREFULLY THE FOLLOWING TERMS AND CONDITIONS CONTAINED IN THIS END USER LICENSE AGREEMENT (THE “LICENSE AGREEMENT”) AS THEY GOVERN YOUR USE OF CUSTOM BUILDERS USA, LLC'S ("CB-USA's") ON-LINE SOFTWARE AND SYSTEM (THE “CB-USA SYSTEM”).  CB-USA IS WILLING TO LICENSE THE USE OF THE CB-USA SYSTEM TO YOU ONLY ON THE CONDITION THAT YOU ACCEPT ALL OF THE TERMS AND CONDITIONS CONTAINED IN THIS LICENSE AGREEMENT.  
                                            BY CLICKING <span style="font-size:14px;">"I ACCEPT"</span> AT THE END OF THIS AGREEMENT OR BY INSTALLING, ACCESSING, OR USING ANY PART OF THE CB-USA SYSTEM, YOU ACKNOWLEDGE THAT YOU HAVE READ THIS AGREEMENT, THAT YOU UNDERSTAND IT AND ITS TERMS AND CONDITIONS, AND THAT YOU AGREE  TO BE LEGALLY BOUND BY IT AND ITS TERMS AND CONDITIONS.
                                        </p>
                                        <p>
                                            IF YOU DO NOT AGREE WITH THIS LICENSE AGREEMENT, YOU ARE NOT GRANTED PERMISSION BY CB-USA TO INSTALL, ACCESS, OR OTHERWISE USE THE CB-USA SYSTEM.  IN SUCH CASE, PLEASE CLICK <span style="font-size:14px;">"I REJECT"</span> AND PROMPTLY RETURN AND/OR DELETE ANY MATERIALS RELATED TO THE CB-USA SYSTEM THAT YOU HAVE RECEIVED FROM CB-USA OR THAT YOU HAVE IN YOUR POSSESSION.  
                                        </p>
                                        <h2 align="center">TERMS AND CONDITIONS</h2>
                                        <ol>
                                            <li><b>LICENSE GRANT.</b>  Conditioned on your continued compliance with the terms and conditions of this License Agreement, this License Agreement provides you with a revocable, limited, non-exclusive, nontransferable license to use the CB-USA System for your internal business purposes only and solely in connection with research and marketing.  This license permits you to (i) use the CB-USA System on a single laptop, terminal, workstation, or computer; (ii) access the CB-USA System from the Internet or through an on-line network, (iii) load the CB-USA System into your computer's temporary memory (RAM); and (iv) create printouts of output from the CB-USA System.  Any rights granted hereby are licensed and not sold or otherwise transferred or assigned to you or any third party.  References to "you" or "user" mean the corporate or individual licensee of the CB-USA System and any successor, permitted assign, transferee, heir, or representative thereof.</li>
                                            <li><b>LICENSE GRANT RESTRICTIONS.</b>  Except as provided above, you may not modify, alter, translate, decompile, create derivative work(s) of, distribute, disassemble, reverse engineer, broadcast, transmit, reproduce, attempt to examine the source code for, publish, license, sub-license, transfer, sell, exploit, rent, timeshare, outsource, provide on a service bureau basis, lease, grant a security interest in, transfer any right(s) in, or otherwise use in any manner not expressly permitted herein the CB-USA System or any part thereof.  In addition, you may not remove or alter any proprietary notice on the CB-USA System or use any portion of the CB-USA System independently from the CB-USA System as a whole.  All rights not expressly granted to you herein are hereby reserved to CB-USA.</li>
                                            <li><b>USER OBLIGATIONS.</b>  By installing, accessing, and using the CB-USA System, you represent that you agree to abide by all applicable local, state, national, and international laws and regulations with respect to your use of the CB-USA System.  You agree to assume all responsibility concerning your use of the CB-USA System, including providing any support and meeting any requirements of your contracts with third parties.  CB-USA assumes no responsibility or liability for any claims that may result directly or indirectly from the communications or interactions you establish using the CB-USA System.  </li>
                                            <li><b>PROPRIETARY RIGHTS.</b>  CB-USA retains all ownership right, title, and interest in and to all programs, software, information, and documentation associated with the CB-USA System as well as any data compiled, collected, or associated with the CB-USA System.  CB-USA, CB-USA Plus Logo Design, and all other names, logos, and icons identifying CB-USA and its products and services are proprietary trademarks of CB-USA and/or its licensors, as applicable, and any use of such marks without the express written permission of CB-USA is strictly prohibited.  Except as expressly provided herein, CB-USA does not grant any express or implied right to you or any other person under any intellectual or proprietary rights.  Accordingly, unauthorized use of the CB-USA System may violate intellectual property or other proprietary rights laws as well as other domestic and international laws, regulations, and statutes, including, but not limited to, United States copyright, trade secret, patent, and trademark law.  </li>
                                            <li><b>CONFIDENTIALITY.</b>  You acknowledge and agree that the CB-USA System contains proprietary trade secrets and confidential information of CB-USA and/or its licensors and suppliers, including, without limitation, information from CB-USA's building suppliers concerning pricing and discount information, cooperative or otherwise, on lumber and other building materials (the "Confidential Information").  You also acknowledge and agree that the Confidential Information of CB-USA is a valuable and material asset of CB-USA and that any disclosure or unauthorized use of such Confidential Information would be detrimental to CB-USA and its business and goodwill.  Accordingly, you agree to secure and protect the confidentiality of the Confidential Information of CB-USA (and/or its licensors and suppliers) in a manner consistent with the maintenance of CB-USA's rights therein, using at least as great a degree of care as you use to maintain the confidentiality of your own confidential information of a similar nature, but in no event using less than reasonable efforts.  You shall not, nor permit any third party to, sell, transfer, publish, disclose, or otherwise make available any portion of the Confidential Information to third parties, except as expressly authorized in this Agreement.  In particular, you acknowledge and agree that you are prohibited from disclosing or making accessible any Confidential Information of CB-USA to any non-member or to any building material supplier.  All Confidential Information of CB-USA shall remain the exclusive property of CB-USA.  These restrictions do not apply to Confidential Information which you (i) are required by law or regulation to disclose, but only to the extent and for the purposes of such law or regulation; (ii) disclose in response to a valid order of a court or other governmental body, but only to the extent of and for the purposes of such order, and only if you first notify CB-USA of the order and permit CB-USA to seek an appropriate protective order or move to quash or limit such order; or (iii) disclose with written permission of CB-USA, in compliance with any terms or conditions set by CB-USA regarding such disclosure.  Upon termination or expiration of this Agreement, you shall return to CB-USA or destroy, at the request of CB-USA, all Confidential Information of CB-USA and certify in writing to CB-USA, within ten (10) days following termination or expiration, that all such Confidential Information has been returned or destroyed.  </li>
                                            <li><b>ENFORCEMENT.</b>  You acknowledge that any breach, threatened or actual, of this License Agreement will cause irreparable injury to CB-USA and/or its licensors or suppliers, such injury would not be quantifiable in monetary damages, and CB-USA and/or its licensors or suppliers would not have an adequate remedy at law.  You therefore agree that CB-USA and/or its licensors or suppliers (or on their behalf) shall be entitled, in addition to other available remedies, to terminate or suspend immediately your membership with CB-USA and seek and be awarded an injunction or other appropriate equitable relief from a court of competent jurisdiction restraining any breach, threatened or actual, of your obligations under any provision of this License Agreement.  Accordingly, you hereby waive any requirement that CB-USA or its licensors or suppliers post any bond or other security in the event any injunctive or equitable relief is sought by or awarded to CB-USA to enforce any provision of this License Agreement.  </li>
                                            <li><b>SECURITY.</b>  You shall not, nor shall you permit any third party to, disable, circumvent, or otherwise avoid any security device, mechanism, protocol, or procedure established by CB-USA for use of the CB-USA System.  You will immediately notify CB-USA if you become aware of any unauthorized use of the CB-USA System.  </li>
                                            <li><b>SUBMISSIONS.</b>  CB-USA welcomes your feedback and suggestions about how to improve the CB-USA System.  You agree that CB-USA shall have the right to use such feedback and suggestions in any manner it deems desirable without providing any consideration or payment to you.</li>
                                            <li><b>WARRANTY DISCLAIMER.</b>  CB-USA MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE SUITABILITY, COMPLETENESS, TIMELINESS, RELIABILITY, LEGALITY, OR ACCURACY OF THE INFORMATION, SERVICES, PROGRAMS, PRODUCTS, SERVICES, AND MATERIALS ASSOCIATED WITH OR AVAILABLE THROUGH THE CB-USA SYSTEM FOR ANY PURPOSE.  THE CB-USA SYSTEM AND ANY SUCH INFORMATION, SERVICES, PROGRAMS, PRODUCTS, AND MATERIALS ARE PROVIDED “AS IS” WITHOUT WARRANTY OF ANY KIND, INCLUDING, WITHOUT LIMITATION, ALL IMPLIED WARRANTIES AND CONDITIONS OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PUROSE, TITLE, AND NON-INFRINGEMENT AS WELL AS ANY WARRANTY RELATED TO THE USE, OR THE RESULTS OF THE USE, OF THE CB-USA SYSTEM OR DOCUMENTATION ASSOCIATED THEREWITH IN TERMS OF CORRECTNESS, ACCURACY, RELIABILITY, SECURITY, OR OTHERWISE.  CB-USA DOES NOT WARRANT THAT THE CB-USA SYSTEM WILL OPERATE ERROR-FREE, UNINTERRUPTED, OR IN A MANNER THAT WILL MEET YOUR REQUIREMENTS.  THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE CB-USA SYSTEM IS WITH YOU.  </li>
                                            <li><b>LIMITATION OF LIABILITY.</b>  YOU AGREE THAT IN NO EVENT SHALL CB-USA BE LIABLE FOR ANY INDIRECT, PUNITIVE, INCIDENTAL, SPECIAL, OR CONSEQUENTIAL DAMAGES ARISING OUT OF OR IN ANY WAY CONNECTED WITH THE USE OF THIS PRODUCT BY YOU OR FOR ANY INFORMATION, SERVICES, PROGRAMS, PRODUCTS, SERVICES, AND MATERIALS AVAILABLE WITH OR THROUGH THIS PRODUCT, OR OTHERWISE ARISING OUT OF UTILIZATION OF THIS PRODUCT, WHETHER BASED IN CONTRACT, TORT, STRICT LIABILITY, OR OTHERWISE, EVEN IF CB-USA HAS BEEN ADVISED OF THE POSSIBILITY OF DAMAGES.  WITHOUT LIMITATION OF THE FOREGOING, TOTAL LIABILITY OF CB-USA FOR ANY REASON WHATSOEVER RELATED TO USE OF THIS PRODUCT OR ANY CLAIMS RELATING TO THIS AGREEMENT OR THE PRODUCT SHALL NOT EXCEED $5,000 (USD).</li>
                                            <li><b>INDEMNITY.</b>  You agree to defend, indemnify, and hold harmless CB-USA and its affiliates, employees, licensors, agents, directors, officers, partners, representatives, shareholders, servants, attorneys, predecessors, successors, and assigns from and against any and all claims, proceedings, damages, injuries, liabilities, losses, costs, and expenses (including reasonable attorneys’ fees and litigation expenses), relating to or arising from your use of the CB-USA System and any breach by you of this License Agreement.</li>
                                            <li><b>GOVERNING LAW.</b>  This License Agreement has been made in and will be construed and enforced solely in accordance with the laws of the Commonwealth of Virginia, U.S.A. as applied to agreements entered into and completely performed in the Commonwealth of Virginia.  You agree that any action to enforce this License Agreement will be brought solely in the federal or state courts in the Commonwealth of Virginia, U.S.A., and all parties to this License Agreement expressly agree to be subject to the jurisdiction of such courts.  You also acknowledge and agree that any applicable state law implementation of the Uniform Computer Information Transactions Act (including any available remedies or laws) shall not apply to this Agreement and is hereby disclaimed.  </li>
                                            <li><b>TERM AND TERMINATION.</b>  This License Agreement and your right to use the CB-USA System will take effect at the moment you click "I ACCEPT" or you install, access, or use the CB-USA System and is effective until terminated as set forth below.  This License Agreement will terminate automatically if you click "I REJECT" or if you fail to comply with any of the terms and conditions described herein, including by exceeding the scope of the license.  Termination or expiration of this License Agreement will be effective without notice.  You may also terminate at any time by ceasing to use the CB-USA System, but all applicable provisions of this Agreement will survive termination, as outlined below.  Upon termination or expiration, you must return, destroy, or delete from your system all copies of the CB-USA System (and any associated materials) in your possession.  The provisions concerning proprietary and intellectual property rights, submissions, confidentiality, indemnity, disclaimers of warranty and liability, termination, and governing law will survive the termination or expiration of this License Agreement for any reason.</li>
                                            <li><b>MISCELLANEOUS.</b>  The parties agree that this License Agreement is for the benefit of the parties hereto. Licensee may not assign this License Agreement without CB-USA's prior written consent.  Failure to insist on strict performance of any of the terms and conditions of this License Agreement will not operate as a waiver of that or any subsequent default or failure of performance.  A printed version of this License Agreement and of any related notice given in electronic form shall be admissible in judicial or administrative proceedings based upon or relating to this License Agreement to the same extent and subject to the same conditions as other business documents and records originally generated and maintained in printed form.  No joint venture, partnership, employment, or agency relationship exists between you and CB-USA as result of this License Agreement or your utilization of the CB-USA System.  This License Agreement represents the entire agreement between you and CB-USA with respect to use of the CB-USA System, and it supersedes all prior or contemporaneous communications and proposals, whether electronic, oral, or written between you and CB-USA with respect to the CB-USA System.  Please note that CB-USA reserves the right to change the terms and conditions of this License Agreement and under which the CB-USA System is extended to you by providing you in writing or electronically a copy of such revised terms.  CB-USA may change any aspect of the CB-USA System.  Your continued use of the CB-USA System will be conclusively deemed acceptance of any change to this License Agreement or the CB-USA System.  If you have questions regarding the CB-USA System or if you are interested in obtaining more information concerning CB-USA and its products or services, please contact CB-USA at customerservice@cbusa.us.</li>
                                        </ol>
                                        
                                    </p>
                                    <p class="larger" style="text-align:center;">                                        
                                        I HAVE READ AND UNDERSTOOD THE FOREGOING AGREEMENT AND AGREE TO BE BOUND BY ALL OF ITS TERMS AND CONDITIONS.  PLEASE MANIFEST YOUR ASSENT TO THIS AGREEMENT BY CLICKING ON THE APPROPRIATE LINK BELOW.<br />
                                        <asp:Button id="btnAcceptEula" runat="server" text="I HAVE READ AND UNDERSTOOD AND ACCEPT THIS AGREEMENT" cssclass="btnred" onclientclick="AcceptTerms()" /><br />
                                        <asp:Button id="btnCancelEula" runat="server" text="I REJECT THIS AGREEMENT" cssclass="btnred" onclientclick="RejectTerms()" />
                                    </p>
                                </div>
                            </div>
                        </FormTemplate>
                        <Buttons>
                            <CC:PopupFormButton ControlID="btnAcceptEula" ButtonType="ScriptOnly" />
                            <CC:PopupFormButton ControlID="btnCancelEula" ButtonType="ScriptOnly" />
                        </Buttons>
                    </CC:PopupForm>
                </td>
            </tr>
        </table> 
        <p style="text-align:center;">
            <asp:Button id="btnContinue" runat="server" text="Continue" cssclass="btnred" />
            <asp:Button id="btnCancel" runat="server" text="Cancel" onclientclick="location.href='/default.aspx'" />
        </p>   
    </div>    
    <CC:RequiredFieldValidatorFront ID="rfvtxtUsername" runat="server" ControlToValidate="txtUsername" ErrorMessage="Username is empty"></CC:RequiredFieldValidatorFront>
    <CC:RequiredFieldValidatorFront ID="rfvtxtFirstName" runat="server" ControlToValidate="txtFirstName" ErrorMessage="First Name is empty"></CC:RequiredFieldValidatorFront>
    <CC:RequiredFieldValidatorFront ID="rfvtxtLastName" runat="server" ControlToValidate="txtLastName" ErrorMessage="Last Name is empty"></CC:RequiredFieldValidatorFront>
    <CC:RequiredFieldValidatorFront ID="rfvtxtEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email Address is empty"></CC:RequiredFieldValidatorFront>
    <CC:EmailValidatorFront ID="evftxtEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email Address is invalid"></CC:EmailValidatorFront>
    <CC:CompareValidatorFront ID="cvfPassword" runat="server" ControlToCompare="txtPasswordCompare" ControlToValidate="txtPassword" ErrorMessage="Password and Confirm Password do not match"></CC:CompareValidatorFront>
    
</CT:MasterPage>
