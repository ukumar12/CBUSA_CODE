<%@ Page Language="VB" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="forms_builder_registration_default" %>
<%@ Register TagName="BuilderRegistrationSteps" TagPrefix="CC" Src="~/controls/BuilderRegistrationSteps.ascx" %>

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
        if (xml) {
            xml.open("GET", "/ajax.aspx?f=CheckAvailbility&BuilderAccountId=<%=BuilderAccountId%>&Username=" + txtUsername.value, true);
            xml.onreadystatechange = function () {
                if (xml.readyState == 4 && xml.responseText) {
                    if (xml.responseText.length > 0) {
                        var sUsername = txtUsername.value
                        sUsername = sUsername.replace(/</g, "&lt;");
                        sUsername = sUsername.replace(/>/g, "&gt;");
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



    function OpenAsp() {
        var frm = $get('<%=frmASP.ClientID %>').control;
        frm._doMoveToCenter();
        frm.Open();
    }
    function AcceptAsp() {
        var cb = $get('<%=cbAsp.ClientID %>');
        cb.checked = true;
        $get('<%=frmAsp.ClientID %>').control.Close();
    }
    function RejectAsp() {
        var cb = $get('<%=cbAsp.ClientID %>');
        cb.checked = false;
        $get('<%=frmAsp.ClientID %>').control.Close();
    }
 
//-->
</script>
</asp:PlaceHolder> 
<div style="margin:10px auto;text-align:center;">
<%--<asp:button id ="btnGoToDashBoard" text="Go to DashBoard" class="btnred" runat="server" style="margin-right:176px;"/>--%>
    <CC:BuilderRegistrationSteps ID="ctrlSteps" runat="server" RegistrationStep="1" />
</div>
<asp:Panel id="pnlForm" runat="server" DefaultButton="btnContinue">
<table cellpadding="0" cellspacing="5" border="0">
    <tr valign="top">
    <td class="pckggraywrpr" style="width:45%;">
        <div class="pckghdgred">User Information:</div>
        <table cellpadding="2" cellspacing="2" border="0">
            <tr>
                <td>&nbsp;</td>
                <td class="fieldreq">&nbsp;</td>
                <td><span class="smaller"> indicates required field</span></td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeltxtFirstName" runat="server">First Name:</span></td>
                <td class="fieldreq" id="bartxtFirstName" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox ID="txtFirstName" runat="server" MaxLength="50" Rows="50" style="width:150px;" CssClass="regtxt"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeltxtLastName" runat="server">Last Name:</span></td>
                <td class="fieldreq" id="bartxtLastName" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox ID="txtLastName" runat="Server" MaxLength="50" Rows="50" style="width:150px;" CssClass="regtxt"></asp:TextBox></td>
            </tr>
            <tr id="trTitle" runat="server">
                <td class="fieldlbl"><span id="labeltxtTitle" runat="server">Title:</span></td>
                <td class="fieldreq" id="bartxtTitle" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox id="txtTitle" runat="server" maxlength="50" style="width:150px;" cssclass="regtext"></asp:TextBox></td>
            </tr>   
            <tr>
                <td class="fieldlbl"><span id="labeltxtAccountPhone" runat="server">Phone:</span></td>
                <td class="fieldreq" id="battxtAccountPhone" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox ID="txtAccountPhone" runat="server" MaxLength="50" Rows="50" style="width:150px;" CssClass="regtxt"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeltxtAccountMobile" runat="server">Mobile:</span></td>
                <td class="fieldnorm" id="bartxtAccountMobile" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox ID="txtAccountMobile" runat="server" MaxLength="50" Rows="50" style="width:150px;" CssClass="regtxt"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeltxtAccountFax" runat="server">Fax:</span></td>
                <td class="fieldnorm" id="bartxtAccountFax" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox ID="txtAccountFax" runat="server" MaxLength="50" Rows="50" style="width:150px;" CssClass="regtxt"></asp:TextBox></td>
            </tr>         
            <tr>
                <td class="fieldlbl"><span id="labeltxtEmail" runat="server">Email:</span></td>
                <td class="fieldreq" id="bartxtEmail" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox ID="txtEmail" runat="server" MaxLength="100" Rows="50" style="width:250px;" CssClass="regtxt"></asp:TextBox></td>
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
    </td>

    <td class="pckggraywrpr" style="width:45%;">    
        <div class="pckghdgred">Company Information:</div>
        <table cellpadding="2" cellspacing="2" border="0">
            <tr>
                <td class="fieldlbl"><span id="labeltxtCompanyName" runat="server">Company Name:</span></td>
                <td class="fieldreq" id="bartxtCompanyName" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox ID="txtCompanyName" runat="server" MaxLength="100" Rows="70"  style="width:200px;" CssClass="regtxt"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeltxtAddress" runat="server">Address:</span></td>
                <td class="fieldreq" id="bartxtAddress" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox ID="txtAddress" runat="server" MaxLength="50" columns="50" style="width:250px;" CssClass="regtxt"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="fieldlbl">Address line 2:</td>
                <td>&nbsp;</td>
                <td class="field"><asp:TextBox ID="txtAddress2" runat="server" MaxLength="50" Rows="50" style="width:250px;" CssClass="regtxt"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeltxtCity" runat="server">City:</span></td>
                <td class="fieldreq" id="bartxtCity" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox ID="txtCity" runat="server" MaxLength="50" Rows="50" style="width:150px;" CssClass="regtxt"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeldrpState" runat="server">State:</span></td>
                <td class="fieldreq" id="bardrpState" runat="server">&nbsp;</td>
                <td class="field"><asp:DropDownList id="drpState" runat="server"></asp:DropDownList></td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeltxtZip" runat="server">Zip Code:</span></td>
                <td class="fieldreq" id="bartxtZip" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox ID="txtZip" runat="server" MaxLength="15" Rows="15" style="width:50px;" CssClass="regtxtshort"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeltxtPhone" runat="server">Phone:</span></td>
                <td class="fieldreq" id="bartxtPhone" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox ID="txtPhone" runat="server" MaxLength="50" Rows="50" style="width:150px;" CssClass="regtxt"></asp:TextBox></td>
            </tr>
            
            <tr>
                <td class="fieldlbl"><span id="labeltxtFax" runat="server">Fax:</span></td>
                <td class="fieldnorm" id="bartxtFax" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox ID="txtFax" runat="server" MaxLength="50" Rows="50" style="width:150px;" CssClass="regtxt"></asp:TextBox></td>
            </tr>
            
            <tr>
                <td class="fieldlbl"><span id="labeltxtWebsiteUrl" runat="server">Website:</span></td>
                <td class="fieldnorm" id="bartxtWebsiteUrl" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox ID="txtWebsiteUrl" runat="server" MaxLength="100" Rows="50" style="width:350px;" CssClass="regtxt"></asp:TextBox></td>
            </tr>
           
            <tr>
                <td class="fieldlbl"><span id="labeltxtCompanyEmail" runat="server">Company Email:</span></td>
                <td class="fieldnorm" id="bartxtCompanyEmail" runat="server">&nbsp;</td>
                <td class="field"><asp:TextBox ID="txtCompanyEmail" runat="server" MaxLength="100" Rows="50" style="width:350px;" CssClass="regtxt"></asp:TextBox></td>
            </tr>

        </table>
    </td>
    </tr>
        <tr id="trEULA" runat="server" valign="top">
        <td colspan="2" align="center">
            <table cellspacing="2" cellpadding="2" border="0" style="text-align:left;">
                <tr>
                    <td class="fieldreq" id="barcbAsp" runat="server" style="width:20px;"></td>
					<td class="fieldlbl"><asp:CheckBox ID="cbAsp" runat="server" onclick="OpenAsp();" /><span id="labelcbAsp" runat="server">I have read and agree to the End User License Agreement and CBUSA Antitrust Compliance Guide</span></td>
                    <td></td>
					</tr>
					<tr>
                    <td class="field" colspan="3" style="padding-left:208px;">
                        &nbsp;<asp:Button id="btnShowAsp" runat="server" text="View Agreement" cssclass="btnblue" />
                        <CC:PopupForm ID="frmAsp" runat="server" OpenMode="MoveToCenter" OpenTriggerId="btnShowAsp" Animate="true" ShowVeil="true" VeilCloses="false" CssClass="pform" style="width:900px;">
                            <FormTemplate>
                                <div class="pckggraywrpr" style="margin:0px;">
                                    <div class="pckghdgred">End User License Agreement</div>
                                    <div style="padding:10px;background-color:#fff;overflow:auto;">
                                    <div style="MARGIN: 0in 0in 6pt" align="center"><font size="5"><strong>CBUSA</strong></font></div>
                                    <div style="MARGIN: 0in 0in 6pt" align="center"><strong><font size="5">END USER LICENSE AGREEMENT</font></strong></div>
																		
									<p style="line-height:14px;font-size:13px;">
                                        <p>
                                            <strong><span style="font-size:14px;text-decoration:underline;">IMPORTANT</span> – THIS IS A LEGAL DOCUMENT.  BEFORE INSTALLING, ACCESSING, OR USING ANY PART OF THIS SYSTEM, YOU SHOULD READ CAREFULLY THE FOLLOWING TERMS AND CONDITIONS CONTAINED IN THIS END USER LICENSE AGREEMENT (THE “LICENSE AGREEMENT”) AS THEY GOVERN YOUR USE OF CBUSA, LLC'S ("CBUSA's") ON-LINE SOFTWARE AND SYSTEM (THE “CBUSA SYSTEM”).  CBUSA IS WILLING TO LICENSE THE USE OF THE CBUSA SYSTEM TO YOU ONLY ON THE CONDITION THAT YOU ACCEPT ALL OF THE TERMS AND CONDITIONS CONTAINED IN THIS LICENSE AGREEMENT.  
                                            BY CLICKING <span style="font-size:14px;">"I ACCEPT"</span> AT THE END OF THIS AGREEMENT OR BY INSTALLING, ACCESSING, OR USING ANY PART OF THE CBUSA SYSTEM, YOU ACKNOWLEDGE THAT YOU HAVE READ THIS AGREEMENT, THAT YOU UNDERSTAND IT AND ITS TERMS AND CONDITIONS, AND THAT YOU AGREE  TO BE LEGALLY BOUND BY IT AND ITS TERMS AND CONDITIONS.</strong>
                                        </p>
                                        <p><strong>
                                            IF YOU DO NOT AGREE WITH THIS LICENSE AGREEMENT, YOU ARE NOT GRANTED PERMISSION BY CBUSA TO INSTALL, ACCESS, OR OTHERWISE USE THE CBUSA SYSTEM.  IN SUCH CASE, PLEASE CLICK <span style="font-size:14px;">"I REJECT"</span> AND PROMPTLY RETURN AND/OR DELETE ANY MATERIALS RELATED TO THE CBUSA SYSTEM THAT YOU HAVE RECEIVED FROM CBUSA OR THAT YOU HAVE IN YOUR POSSESSION.  </strong>
                                        </p>
                                        <h2 align="center">TERMS AND CONDITIONS</h2>
                                        <ol>
                                            <li><b>LICENSE GRANT.</b>  Conditioned on your continued compliance with the terms and conditions of this License Agreement, this License Agreement provides you with a revocable, limited, non-exclusive, nontransferable license to use the CBUSA System for your internal business purposes only and solely in connection with research and marketing.  This license permits you to (i) use the CBUSA System on a single laptop, terminal, workstation, or computer; (ii) access the CBUSA System from the Internet or through an on-line network, (iii) load the CBUSA System into your computer's temporary memory (RAM); and (iv) create printouts of output from the CBUSA System.  Any rights granted hereby are licensed and not sold or otherwise transferred or assigned to you or any third party.  References to "you" or "user" mean the corporate or individual licensee of the CBUSA System and any successor, permitted assign, transferee, heir, or representative thereof.</li>
                                            <li><b>LICENSE GRANT RESTRICTIONS.</b>  Except as provided above, you may not modify, alter, translate, decompile, create derivative work(s) of, distribute, disassemble, reverse engineer, broadcast, transmit, reproduce, attempt to examine the source code for, publish, license, sub-license, transfer, sell, exploit, rent, timeshare, outsource, provide on a service bureau basis, lease, grant a security interest in, transfer any right(s) in, or otherwise use in any manner not expressly permitted herein the CBUSA System or any part thereof.  In addition, you may not remove or alter any proprietary notice on the CBUSA System or use any portion of the CBUSA System independently from the CBUSA System as a whole.  All rights not expressly granted to you herein are hereby reserved to CBUSA.</li>
                                            <li><b>USER OBLIGATIONS.</b>  By installing, accessing, and using the CBUSA System, you represent that you agree to abide by all applicable local, state, national, and international laws and regulations with respect to your use of the CBUSA System.  You agree to assume all responsibility concerning your use of the CBUSA System, including providing any support and meeting any requirements of your contracts with third parties.  CBUSA assumes no responsibility or liability for any claims that may result directly or indirectly from the communications or interactions you establish using the CBUSA System.  </li>
                                            <li><b>PROPRIETARY RIGHTS.</b>  CBUSA retains all ownership right, title, and interest in and to all programs, software, information, and documentation associated with the CBUSA System as well as any data compiled, collected, or associated with the CBUSA System.  CBUSA, CBUSA Plus Logo Design, and all other names, logos, and icons identifying CBUSA and its products and services are proprietary trademarks of CBUSA and/or its licensors, as applicable, and any use of such marks without the express written permission of CBUSA is strictly prohibited.  Except as expressly provided herein, CBUSA does not grant any express or implied right to you or any other person under any intellectual or proprietary rights.  Accordingly, unauthorized use of the CBUSA System may violate intellectual property or other proprietary rights laws as well as other domestic and international laws, regulations, and statutes, including, but not limited to, United States copyright, trade secret, patent, and trademark law.  </li>
                                            <li><b>CONFIDENTIALITY.</b>  You acknowledge and agree that the CBUSA System contains proprietary trade secrets and confidential information of CBUSA and/or its licensors and suppliers, including, without limitation, information from CBUSA's building suppliers concerning pricing and discount information, cooperative or otherwise, on lumber and other building materials (the "Confidential Information").  You also acknowledge and agree that the Confidential Information of CBUSA is a valuable and material asset of CBUSA and that any disclosure or unauthorized use of such Confidential Information would be detrimental to CBUSA and its business and goodwill.  Accordingly, you agree to secure and protect the confidentiality of the Confidential Information of CBUSA (and/or its licensors and suppliers) in a manner consistent with the maintenance of CBUSA's rights therein, using at least as great a degree of care as you use to maintain the confidentiality of your own confidential information of a similar nature, but in no event using less than reasonable efforts.  You shall not, nor permit any third party to, sell, transfer, publish, disclose, or otherwise make available any portion of the Confidential Information to third parties, except as expressly authorized in this Agreement.  In particular, you acknowledge and agree that you are prohibited from disclosing or making accessible any Confidential Information of CBUSA to any non-member or to any building material supplier.  All Confidential Information of CBUSA shall remain the exclusive property of CBUSA.  These restrictions do not apply to Confidential Information which you (i) are required by law or regulation to disclose, but only to the extent and for the purposes of such law or regulation; (ii) disclose in response to a valid order of a court or other governmental body, but only to the extent of and for the purposes of such order, and only if you first notify CBUSA of the order and permit CBUSA to seek an appropriate protective order or move to quash or limit such order; or (iii) disclose with written permission of CBUSA, in compliance with any terms or conditions set by CBUSA regarding such disclosure.  Upon termination or expiration of this Agreement, you shall return to CBUSA or destroy, at the request of CBUSA, all Confidential Information of CBUSA and certify in writing to CBUSA, within ten (10) days following termination or expiration, that all such Confidential Information has been returned or destroyed.  </li>
                                            <li><b>ENFORCEMENT.</b>  You acknowledge that any breach, threatened or actual, of this License Agreement will cause irreparable injury to CBUSA and/or its licensors or suppliers, such injury would not be quantifiable in monetary damages, and CBUSA and/or its licensors or suppliers would not have an adequate remedy at law.  You therefore agree that CBUSA and/or its licensors or suppliers (or on their behalf) shall be entitled, in addition to other available remedies, to terminate or suspend immediately your membership with CBUSA and seek and be awarded an injunction or other appropriate equitable relief from a court of competent jurisdiction restraining any breach, threatened or actual, of your obligations under any provision of this License Agreement.  Accordingly, you hereby waive any requirement that CBUSA or its licensors or suppliers post any bond or other security in the event any injunctive or equitable relief is sought by or awarded to CBUSA to enforce any provision of this License Agreement.  </li>
                                            <li><b>SECURITY.</b>  You shall not, nor shall you permit any third party to, disable, circumvent, or otherwise avoid any security device, mechanism, protocol, or procedure established by CBUSA for use of the CBUSA System.  You will immediately notify CBUSA if you become aware of any unauthorized use of the CBUSA System.  </li>
                                            <li><b>SUBMISSIONS.</b>  CBUSA welcomes your feedback and suggestions about how to improve the CBUSA System.  You agree that CBUSA shall have the right to use such feedback and suggestions in any manner it deems desirable without providing any consideration or payment to you.</li>
                                            <li><b>WARRANTY DISCLAIMER.</b>  CBUSA MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE SUITABILITY, COMPLETENESS, TIMELINESS, RELIABILITY, LEGALITY, OR ACCURACY OF THE INFORMATION, SERVICES, PROGRAMS, PRODUCTS, SERVICES, AND MATERIALS ASSOCIATED WITH OR AVAILABLE THROUGH THE CBUSA SYSTEM FOR ANY PURPOSE.  THE CBUSA SYSTEM AND ANY SUCH INFORMATION, SERVICES, PROGRAMS, PRODUCTS, AND MATERIALS ARE PROVIDED “AS IS” WITHOUT WARRANTY OF ANY KIND, INCLUDING, WITHOUT LIMITATION, ALL IMPLIED WARRANTIES AND CONDITIONS OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PUROSE, TITLE, AND NON-INFRINGEMENT AS WELL AS ANY WARRANTY RELATED TO THE USE, OR THE RESULTS OF THE USE, OF THE CBUSA SYSTEM OR DOCUMENTATION ASSOCIATED THEREWITH IN TERMS OF CORRECTNESS, ACCURACY, RELIABILITY, SECURITY, OR OTHERWISE.  CBUSA DOES NOT WARRANT THAT THE CBUSA SYSTEM WILL OPERATE ERROR-FREE, UNINTERRUPTED, OR IN A MANNER THAT WILL MEET YOUR REQUIREMENTS.  THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE CBUSA SYSTEM IS WITH YOU.  </li>
                                            <li><b>LIMITATION OF LIABILITY.</b>  YOU AGREE THAT IN NO EVENT SHALL CBUSA BE LIABLE FOR ANY INDIRECT, PUNITIVE, INCIDENTAL, SPECIAL, OR CONSEQUENTIAL DAMAGES ARISING OUT OF OR IN ANY WAY CONNECTED WITH THE USE OF THIS PRODUCT BY YOU OR FOR ANY INFORMATION, SERVICES, PROGRAMS, PRODUCTS, SERVICES, AND MATERIALS AVAILABLE WITH OR THROUGH THIS PRODUCT, OR OTHERWISE ARISING OUT OF UTILIZATION OF THIS PRODUCT, WHETHER BASED IN CONTRACT, TORT, STRICT LIABILITY, OR OTHERWISE, EVEN IF CBUSA HAS BEEN ADVISED OF THE POSSIBILITY OF DAMAGES.  WITHOUT LIMITATION OF THE FOREGOING, TOTAL LIABILITY OF CBUSA FOR ANY REASON WHATSOEVER RELATED TO USE OF THIS PRODUCT OR ANY CLAIMS RELATING TO THIS AGREEMENT OR THE PRODUCT SHALL NOT EXCEED $5,000 (USD).</li>
                                            <li><b>INDEMNITY.</b>  You agree to defend, indemnify, and hold harmless CBUSA and its affiliates, employees, licensors, agents, directors, officers, partners, representatives, shareholders, servants, attorneys, predecessors, successors, and assigns from and against any and all claims, proceedings, damages, injuries, liabilities, losses, costs, and expenses (including reasonable attorneys’ fees and litigation expenses), relating to or arising from your use of the CBUSA System and any breach by you of this License Agreement.</li>
                                            <li><b>GOVERNING LAW.</b>  This License Agreement has been made in and will be construed and enforced solely in accordance with the laws of the Commonwealth of Virginia, U.S.A. as applied to agreements entered into and completely performed in the Commonwealth of Virginia.  You agree that any action to enforce this License Agreement will be brought solely in the federal or state courts in the Commonwealth of Virginia, U.S.A., and all parties to this License Agreement expressly agree to be subject to the jurisdiction of such courts.  You also acknowledge and agree that any applicable state law implementation of the Uniform Computer Information Transactions Act (including any available remedies or laws) shall not apply to this Agreement and is hereby disclaimed.  </li>
                                            <li><b>TERM AND TERMINATION.</b>  This License Agreement and your right to use the CBUSA System will take effect at the moment you click "I ACCEPT" or you install, access, or use the CBUSA System and is effective until terminated as set forth below.  This License Agreement will terminate automatically if you click "I REJECT" or if you fail to comply with any of the terms and conditions described herein, including by exceeding the scope of the license.  Termination or expiration of this License Agreement will be effective without notice.  You may also terminate at any time by ceasing to use the CBUSA System, but all applicable provisions of this Agreement will survive termination, as outlined below.  Upon termination or expiration, you must return, destroy, or delete from your system all copies of the CBUSA System (and any associated materials) in your possession.  The provisions concerning proprietary and intellectual property rights, submissions, confidentiality, indemnity, disclaimers of warranty and liability, termination, and governing law will survive the termination or expiration of this License Agreement for any reason.</li>
                                            <li><b>MISCELLANEOUS.</b>  The parties agree that this License Agreement is for the benefit of the parties hereto. Licensee may not assign this License Agreement without CBUSA's prior written consent.  Failure to insist on strict performance of any of the terms and conditions of this License Agreement will not operate as a waiver of that or any subsequent default or failure of performance.  A printed version of this License Agreement and of any related notice given in electronic form shall be admissible in judicial or administrative proceedings based upon or relating to this License Agreement to the same extent and subject to the same conditions as other business documents and records originally generated and maintained in printed form.  No joint venture, partnership, employment, or agency relationship exists between you and CBUSA as result of this License Agreement or your utilization of the CBUSA System.  This License Agreement represents the entire agreement between you and CBUSA with respect to use of the CBUSA System, and it supersedes all prior or contemporaneous communications and proposals, whether electronic, oral, or written between you and CBUSA with respect to the CBUSA System.  Please note that CBUSA reserves the right to change the terms and conditions of this License Agreement and under which the CBUSA System is extended to you by providing you in writing or electronically a copy of such revised terms.  CBUSA may change any aspect of the CBUSA System.  Your continued use of the CBUSA System will be conclusively deemed acceptance of any change to this License Agreement or the CBUSA System.  If you have questions regarding the CBUSA System or if you are interested in obtaining more information concerning CBUSA and its products or services, please contact CBUSA at customerservice@cbusa.us.</li>
                                        </ol>
									<div style="MARGIN: 0in 0in 0pt"><strong>&nbsp;</strong></div>
									<div style="background: #c60000; width:100%; height:3px;"><hr style="display:none;"/></div>
									<div style="MARGIN: 0in 0in 0pt"><strong>&nbsp;</strong></div>
									
									<div style="MARGIN: 0in 0in 6pt" align="center"><strong><font size="5">ANTITRUST COMPLIANCE GUIDE FOR CBUSA MEMBERS AND STAFF</font></strong></div>
									
                               									
									<div style="MARGIN: 0in 0in 0pt"><strong>CBUSA's policy is to comply fully and strictly with both federal and state antitrust laws. </strong></div><div style="MARGIN: 0in 0in 0pt"><strong>&nbsp;</strong></div>

<div style="MARGIN: 0in 0in 0pt"><strong>It is important to remember that buying groups are generally procompetitive in nature, but that antitrust risks may arise by bringing competitors together. CBUSA's policy is motivated by a firm respect and belief in the antitrust laws and the free market philosophy underlying these laws as well as by recognition of the potentially severe detrimental consequences of antitrust violations.  Our aim is to conduct ourselves in such a way as to avoid any potential for antitrust exposure in the first instance.  </strong></div><div style="MARGIN: 0in 0in 0pt"><strong>&nbsp;</strong></div>

<div style="MARGIN: 0in 0in 0pt"><strong>Full compliance with the antitrust laws is a requirement for CBUSA membership, and responsibility for compliance rests with each member.</strong></div><div style="MARGIN: 0in 0in 0pt"><strong>&nbsp;</strong></div>

<div style="MARGIN: 0in 0in 0pt"><strong>In order to comply with the antitrust laws, competitors (whether builder competitors or supplier competitors) should not discuss certain subjects when they are together&mdash;either at formal meetings or in informal contacts with other industry members.  Topics to avoid discussing with competitors with respect to sales to builder members' customers include:  prices, price trends, timing of price changes; costs of common inputs; margins, terms of sale, discounts and rebates, promotional programs, inventory levels, production levels, capacities, new projects, and the like.  Further, with rare exceptions that should be made only upon the advice of counsel, members are prohibited from:</strong></div><div style="MARGIN: 0in 0in 0pt"><strong>&nbsp;</strong></div>				

                                    <ul>
                                        <li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Fixing or setting prices for selling products or services to customers (either homebuyers or builder members);</font></div>
                                        </li>

<li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Allocating geographic markets or customers between or among competitors;</font></div>
                                        </li>

<li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Bid rigging, bid rotation, or otherwise distorting the bid process;</font></div>
                                        </li>

<li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Boycotting customers or suppliers.  Boycotting occurs when competitors agree with or pressure each other not to deal with others.  It is permissible, however, for CBUSA to encourage members builders to use member vendors in order to achieve the best terms from those vendors;</font></div>
                                        </li>

<li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Agreeing upon levels of production or output; </font></div>
                                        </li>

<li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Conspiring to exclude competitors or customers from the market; and</font></div>
                                        </li>

<li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Discussing specific sales or marketing plans outside of the joint programs operated by CBUSA, or any company's individual confidential strategies.</font></div></li>
										
                                       
                                    </ul>
                                    <div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
									
									<div style="MARGIN: 0in 0in 0pt"><strong>Members have an obligation to terminate any discussion, seek legal counsel's advice, or, if necessary, terminate any meeting if the discussion might be construed to raise any antitrust risks.</strong></div>
									<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
									
                                   <h2 align="center">INTRODUCTION</h2>
						<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>									
									
<div style="MARGIN: 0in 0in 0pt">Joint purchasing organizations (or "buying groups") such as CBUSA and its affiliates are recognized as valuable tools of American business.  Joint purchasing is procompetitive in that it can enable smaller companies to come together to reduce costs by negotiating favorable prices or other terms that they otherwise could not obtain on their own.  Nevertheless, since CBUSA is by its nature a combination of competitors and customers, CBUSA and its members must ensure that their activities do not constitute an illegal restraint of trade, or even create the appearance of such an anticompetitive restraint. </div> 
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>

<div style="MARGIN: 0in 0in 0pt">All CBUSA members, directors, officers, employees, vendors, manufacturer partners, and affiliates  must be aware of the ever-present threat of antitrust liability&mdash;at formal and informal meetings of the group, trade shows, educational events, cocktail parties, dinners, and social events; and in telephone and on-line conversations and correspondence.  </div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>

<div style="MARGIN: 0in 0in 0pt">CBUSA believes that competitive markets are necessary for the continuing success of its members and for the long-term viability of the buying group as a whole.  The nation's antitrust laws are designed to promote competition by prohibiting certain kinds of behavior.  For these reasons, CBUSA expects all of its members, directors, officers, employees, and affiliates to comply with the applicable federal and state antitrust laws.</div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
  
<div style="MARGIN: 0in 0in 0pt">The CBUSA Antitrust Compliance Guide should aid CBUSA members, directors, officers, employees, and affiliates on general antitrust questions and issues.  As these guidelines do not address every situation with antitrust consequences that may arise, CBUSA advises those confronted with sensitive antitrust issues to consult with CBUSA's legal counsel, their local affiliate's legal counsel, or their own company counsel. </div> 
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>

									 <h2 align="center">SUMMARY OF ANTITRUST DOS AND DON'TS</h2>
									 <div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
									
<div style="MARGIN: 0in 0in 0pt">The intent of this Guide is not to make you an antitrust lawyer but to give you enough information about the law so you will know a dangerous area when you see it.  The following are some of the most critical "Dos and Don'ts" for antitrust compliance:</div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>

<div style="MARGIN: 0in 0in 0pt"><STRONG>Don'ts:</strong></div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>

<UL>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Don't discuss or agree with competitors as to home/project sales prices or home/project pricing methods (i.e., overhead, profit margins, mark-ups, etc.).</font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Don't discuss or agree with competitors as to uniform terms of sale, reduced warranties, or contract provisions.  Warranties and/or contracts that are endorsed by CBUSA as a best demonstrated practice may be permissible. </font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Don't discuss or agree with competitors as to customer pricing practices or strategies, including methods, timing, or implementation of price changes.</font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Don't discuss or agree with competitors as to discounts or rebates to customers.</font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Don't discuss or agree with competitors whether or not to deal with certain customers or in certain markets or territories.  </font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Don't discuss or agree with competitors, suppliers, or customers cutting off or not dealing with certain companies outside of the CBUSA.</font></div>  
	<ul><li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">CBUSA may make decisions regarding dealing with certain companies on behalf of its members and for CBUSA to encourage members to deal only with those vendors who are approved by and under contract with the local LLC. </font></div></li>
	<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">It is permissible to exchange information regarding quality, service level, or other business-related issues, but members must not indicate to others their intention to stop using specific companies.  </font></div></li>
	<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Members may not discuss whether any company is unethical or unfair.  Any discussion of this nature must be based on objective facts (e.g., a company did not pay its bills on time) and not subjective opinions (e.g., the company is "lousy" to deal with).</font></div></li></ul>
</li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Don't refuse to sell one product to a customer unless it agrees to buy a second product or service.</font></div></li>
</UL>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>

<div style="MARGIN: 0in 0in 0pt"><strong>Dos:</strong></div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>

<UL>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Do know the purpose of all meetings with competitors.</font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Do ask for an agenda if you do not receive one with your meeting notice.  Do not participate in meetings that do not have set agendas, or agendas that appear improper or overly vague.  If you have any questions about any agenda item, contact an attorney before attending.</font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Do request that counsel be present at any discussion that involves potentially competitively sensitive information.</font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Do seek legal review of any "code of ethics," "industry guidelines," "standards," or the like that are not provided to you or endorsed by CBUSA.</font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Do remember that anything you write, including e-mail and instant messages, may be closely scrutinized.  Be clear in all written documents so that there is no question as to the meaning of the communication.</font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Do seek legal advice if you have any questions about antitrust issues.</font></div></li>
</UL>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>

<h2 align="center">OVERVIEW OF THE ANTITRUST LAWS</h2>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>

<div style="MARGIN: 0in 0in 0pt">Broadly stated, the basic objective of the antitrust laws is to preserve and promote competition and the free enterprise system.  These laws are premised on the assumption that private enterprise and free competition are the most efficient ways to allocate resources, to produce goods at the lowest possible price, and to assure the production of high quality products.  </div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">The U.S. antitrust statutes of principal concern to companies and individuals that participate in joint purchasing organizations are Section 1 of the Sherman Act and Section 5 of the Federal Trade Commission Act.  These laws prohibit all contracts, combinations, and conspiracies that unreasonably restrain trade.  </div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">In addition, all U.S. states have adopted laws that address antitrust and fair trade matters.  State laws usually are interpreted and applied in a similar fashion to the federal laws.  In general, strict compliance with the federal antitrust laws will result in compliance with the state laws. </div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">Some activities are regarded as unreasonable by their very nature and are, therefore, considered illegal "<i>per se</i>," which means that they are conclusively presumed to be unlawful.  Practices in the "<i>per se</i>" category include naked agreements between competitors to fix prices; agreements to agree not to deal with or pressure others to not deal with competitors, suppliers, or customers (group boycott) outside the scope of the decisions by CBUSA,; and agreements to allocate markets or limit production.  Conduct that does not unambiguously injure competition is not "<i>per se</i>" unlawful, but rather is analyzed under the "rule of reason."  Under the "rule of reason," courts will analyze agreements or conduct by examining all of the facts and circumstances that surround the conduct in question to determine whether the actions unreasonably restrain trade.</div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
									 <h2 align="center">WHY IS COMPLIANCE WITH THE ANTITRUST LAWS IMPORTANT?</h2>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>									
<div style="MARGIN: 0in 0in 0pt">Aside from the fact that CBUSA is committed to abiding by the laws of all jurisdictions in which it operates, the penalties for violations of the antitrust laws can be very severe&mdash;both for CBUSA members and for individuals.</div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt"><strong>For Members:</strong></div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<UL>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Under U.S. antitrust laws, corporations can be fined up to $100 million per violation.  Courts also can impose an "alternate fine" of up to twice the gain to the perpetrator or twice the loss to the victim as a result of illegal behavior.  </font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Courts or government antitrust agencies can impose permanent restrictions limiting corporate activity.</font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Private actions&mdash;by customers or competitors who can show they were harmed by the perpetrator's actions&mdash;can result in damages many times the size of a government-imposed fine.</font></div></li>
</UL>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt"><strong>For Individuals:</strong></div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<UL>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Violations of the Sherman Antitrust Act are felonies.  </font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Individuals can be imprisoned for up to ten years, fined up to $1 million, or both, per violation.</font></div></li>
</UL>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt"><strong>For CBUSA:</strong></div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<UL>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Injunctions or other orders issued by the courts may prevent CBUSA from pursuing association business.</font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">On occasion, courts have ordered trade associations to disband.  </font></div></li>
</UL>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">Dealing with a government antitrust investigation or a private antitrust lawsuit is expensive, time-consuming, and distracting.  In addition, an investigation or lawsuit can seriously damage the reputation of CBUSA, its members, and individuals.  It is important to emphasize that these penalties, damages, and distractions are entirely avoidable&mdash;by understanding in very basic terms what the antitrust laws require, and by consulting with legal counsel whenever you are in doubt.</div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>

									 <h2 align="center">INTERACTION WITH COMPETITORS </h2>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">The basic premise of the antitrust laws is that competition entails every company making its business decisions <strong>independently of the others</strong>.  Each of the offenses highlighted below, as well as other antitrust law violations, have at their core some form of "agreement" among otherwise independent companies.  It is important to understand that an "agreement" in antitrust terms rarely means a written agreement signed by all of the "parties" to the agreement.  More often than not, agreements are inferred, by judges or juries, from facts and circumstances that suggest the existence of an understanding.  Agreements can be direct or indirect, explicit or tacit.  Plaintiffs can prove an agreement with all sorts of evidence, including, most typically, circumstantial evidence.</div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>

<div style="MARGIN: 0in 0in 0pt">In the discussion that follows, bear in mind that an "agreement" is a very flexible concept under the antitrust laws.  For this reason, it is important that your statements, actions, and writings be as clear and unambiguous as possible, so as to avoid misinterpretation or misconstruction after the fact.  Never give the impression that any illegal agreement has been reached with a competitor or that inappropriate information has been exchanged.</div>  
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">The following outlines some of the activities involving competitors that can lead to violations of the antitrust laws:</div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>

<div style="MARGIN: 0in 0in 0pt"><strong>Price Fixing</strong></div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">In the United States, any agreement with a competitor establishing, altering, or relating to sales prices, or terms and conditions of sale, is unlawful, regardless of the circumstances.  Price fixing is considered unlawful under the antitrust laws regardless of the reasons why it is undertaken (<em>per se</em> unlawful).  You should not communicate with a competitor to obtain their prices, or have any discussions with competitors on pricing methods, pricing strategies, margins, costs, price increases, credit terms, or terms and conditions of sale under any circumstances.</div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt"><strong>Allocating Markets</strong></div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">Unlawful agreements allocating markets occur when competitors divide territories or customers among themselves.  Customer or market allocation is <em>per se</em> unlawful in the United States.  For example, two competitors cannot agree that one will sell into one geographic market or to a group of customers, and the other will sell in a different geographic market or to a different group of customers.</div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt"><strong>Bid Rigging</strong></div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">Any agreement with a competitor on any method by which prices or bids will be determined is <em>per se</em> unlawful.  Illegal bid rigging also includes agreements or understandings among competitors to (1) rotate bids or contracts; (2) determine who will bid and who will not bid, or who will bid to which customers, or who will bid high and who will bid low; (3) fix the prices that individual competitors will bid; or (4) exchange information about the value or terms of bids between competitors in advance of submitting bids.</div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt"><strong>Group Boycotts</strong></div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">An unlawful group boycott occurs when competitors, suppliers, or customers agree with each other (or pressure another person) not to deal with others.  This should be distinguished from a unilateral refusal to deal, where a company decides on its own, and without consulting any other company, that it does not want to buy from or sell to another company, which is usually lawful (except, for example, in certain cases where the supplier has a dominant market position).  Where CBUSA has made a determination not to allow certain suppliers to be members, that decision must be based on clearly articulated reasonable criteria.  Examples include size and scope of the supplier, on-time delivery percentage, financial condition, among others.</div>
	<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>	
									 <h2 align="center">DEALING WITH CUSTOMERS OR SUPPLIERS</h2>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>		
<div style="MARGIN: 0in 0in 0pt"><strong>Refusals to Deal&mdash;Decisions by Individual Members</strong></div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">Generally, companies have the right to select their customers and suppliers and may refuse to deal with anyone for any reason or for no reason at all.  However, in some circumstances, companies must be able to show objective reasons for refusing to sell to a customer or to purchase from a supplier.  </div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">Keep in mind that CBUSA may make decisions regarding which vendors may participate in the group buying program.  Situations that may present antitrust risks occur when competitors agree or confer with each other about refusing to deal with others outside of the CBUSA context.  This can transform a <u>unilateral</u> decision into an unlawful group boycott.  To the extent a member builder individually decides not to deal with a particular customer or supplier or not to sell in a particular market, the decision should be made for independent business reasons and not because of an agreement or understanding with another company.  </div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt"><strong>Refusals to Deal&mdash;Decisions by CBUSA</strong></div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">To the extent CBUSA chooses to exclude certain suppliers from the buying group, those decisions should be made at the regional or national level and should be based on clearly articulated objective criteria such as size and scope of the supplier, financial condition, etc.  As discussed above, individual members should never agree among themselves to refuse to deal with a specific supplier or customer.</div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">Keep in mind that if the local CBUSA affiliate has a particularly strong market position, the affiliate's ability to refuse to deal with certain customers or suppliers may be limited under the antitrust laws.  Although having a strong market position is not necessarily problematic on its own, using this position to exclude others may raise significant antitrust issues.  If your affiliate has a strong market position and is considering refusing to deal with certain customers or suppliers, you should consult with legal counsel.  </div>
	<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>								
	

									 <h2 align="center">MONOPOLIZATION</h2>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>									 
<div style="MARGIN: 0in 0in 0pt">An entity has "monopoly power" or a "dominant position" if it has the power to control market prices or exclude competition.  Relevant factors in determining whether an entity has a dominant market position include:  its market share; the entity's position relative to that of its competitors; the existence of barriers to entry into the market; the dependence of customers on a particular product or service; and the extent of any vertical integration.  </div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">Note that for purposes of antitrust analysis, an entity may be either a single company or an association of competitors such as CBUSA.  </div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">An attempt to monopolize also may be unlawful.  This is the case, for example, when an entity engages in anticompetitive practices (1) with the specific intent to eliminate competition, and (2) where the entity has such a large market share that it has the power to eliminate competition or set prices.  </div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">Even entities with monopoly power or a dominant market position may continue to compete fairly to increase the size of their business.  It is not an offense to be dominant; problems arise with the abuse of such dominance.  Entities cannot use their market position to further entrench their monopoly power or abuse their dominant position.  Determining whether these restrictions apply to particular markets, and whether particular business practices may create problems in this regard, are complex issues that must be discussed with legal counsel.</div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">The following is a non-exhaustive list of the kinds of activities which can cause problems if an entity has a dominant market position:</div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<UL>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Refusal to supply&mdash;there must be objective reasons for refusing to sell to customers/resellers.</font></div></li>


<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Imposing unfair purchase or selling prices, or other unfair trading conditions&mdash;for example, imposing excessively high prices or onerous contract terms that the entity only can obtain as a result of holding a dominant position. </font></div></li>

<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Predatory pricing&mdash;pricing below a specified measure of costs with the intention of driving a competitor out of the market.</font></div></li>
</UL>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>									 
									 <h2 align="center">MONOPSONY POWER</h2>									 
	<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>								 
<div style="MARGIN: 0in 0in 0pt">Joint purchasing, by definition, means greater purchasing power by the group than each of the buyers might have individually.  This generally is procompetive because it results in lower prices.  If a buying group can reduce purchase prices below competitive levels by reducing the total quantity purchased in the entire market, however, it may raise antitrust concerns.  Where prices are set through the artificial manipulation of powerful buyers rather than by competitive forces, suppliers may leave the market and consumers may be harmed by the resulting loss of choice.  This power to eliminate suppliers is known as monopsony power.  </div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">To determine whether there is a monopsony risk, it is first necessary to determine whether the entity has market power in the relevant market.  If the group has market power (typically defined as more than 35% of the relevant market), legal counsel should be consulted to determine the potential for monopsony issues. </div>
	<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
									 <h2 align="center">ANTITRUST ISSUES SPECIFIC TO BUYING GROUPS</h2>	
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>	
<div style="MARGIN: 0in 0in 0pt">The antitrust enforcers may view buying groups as potential sources of illegal competitor collaboration.  If a meeting is followed by parallel action among competitors, such as an increase in prices or a reduction in output, it could be inferred that improper activity took place.  </div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">While virtually all of the antirust issues generally applicable to individual companies apply to buying groups, there are some special antitrust issues that are raised by specific types of activities.  Designing rules and programs carefully with the assistance of counsel, however, can minimize the antitrust risks.  </div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt"><strong>Membership</strong></div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">Buying groups are permitted to adopt objective and reasonable standards for membership.  Exclusionary membership practices that affect a market participant's ability to compete, however, may raise antitrust issues.  Similarly, denial of membership or discrimination in membership terms may place competitors at a disadvantage if membership is necessary to compete in the industry on equal terms.  A buying group that does not have market power generally is free to limit its membership in any way consistent with achieving the efficiency goals of the group.  However, even a buying group without market power should be aware that antitrust concerns may arise from excluding or expelling a firm that competes with members if the purpose is to raise the excluded firm's costs or cause other competitive harm.</div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">Thus, membership criteria must be clearly articulated and based on neutral, objective factors calculated to promote efficiency-enhancing and pro-competitive goals.  This is a particular concern in those markets where the buying group has a significant share and membership may be necessary to be an effective competitor.  It is permissible to require members to fund a share of the group's capital requirements in order to prevent "free riders."</div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">Another important issue with respect to membership is that builder members must disclose any ownership interest in vendor members.  If a builder member has an interest in a vendor member, CBUSA will put safeguards in place so that the builder member is prohibited from access to any competitively-sensitive information about the vendor's competitors.  </div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt"><strong>Information Exchanges, Data Collection, and Dissemination</strong></div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">Structured properly, an information exchange program is a legitimate and necessary function of a buying group.  Compilations of reasonably-available public information and other data collection and statistical reporting, conducted under reasonable guidelines, will not run afoul of the antitrust laws.  Nonetheless, because of the risk that information collected as part of an information exchange could be used for unlawful purposes (for example, as the basis for an agreement to fix prices or restrict output between competitors), a number of precautions must be taken:</div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<UL>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Clearly articulate the purpose and procompetitive benefits of the information exchange program, and keep it closely focused on those criteria.</font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Limit the types of information provided by members to CBUSA.  Only information reasonably necessary for the group to function efficiently should be submitted.  Confidential non-public information regarding matters beyond the scope of the buying group should not be shared.  For example, the value of the individual builder member's specific purchases made outside of the buying group should not be shared because other members could use this information in conjunction with information available through CBUSA to coordinate pricing to customers.</font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">The information should be collected by CBUSA staff or other independent third-party collectors.  Participating members should not be involved in the collection or compilation of the raw data. </font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">The third party should treat specific information provided by participating members confidentially and should not disclose it in its raw form to any other participant or a third party. </font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Published data should be reported in an aggregated form so that information relating to individual transactions is not disclosed and cannot be determined.  Such data (e.g., purchase volumes) should be masked or aggregated so that individual member information is not revealed.</font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">The report should not include information about future prices or other future forecast information. </font></div></li>
</UL>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>

<div style="MARGIN: 0in 0in 0pt">No new data collection or information exchange program should be embarked upon without the approval of counsel.  All such programs should be reviewed from time to time for antitrust concerns.</div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt"><strong>Standardized Costs </strong></div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">If buying group members compete with each other in downstream markets, their equal costs for jointly purchased inputs could potentially reduce price competition if such costs represent a major percentage of downstream selling prices.  This typically is in issue if the input cost exceeds 20% of the members' revenues from the sale of the downstream product.  If the cost exceeds this threshold, the buying group should be able to demonstrate why no antitrust concerns are present.  </div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt"><strong>Best Practices</strong></div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<div style="MARGIN: 0in 0in 0pt">The process of developing industry best practices, if properly executed, can be a beneficial function of an industry group.  Certain precautions should be taken in evaluating and adopting a position on a best practices issue to ensure that a position taken by CBUSA accurately reflects member interests and does not implicate any antitrust concerns such as price fixing or group boycott issues.  Adoption of a best practice or a position on a best practice with anticompetitive intent to limit or prevent certain competitors from competing effectively could lead to antitrust liability.  Therefore, these guidelines should be followed in articulating a CBUSA position on any standard:  </div>
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
<UL>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Consider all relevant opinions.</font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Articulate a sound business rationale or technical basis for the practice based on legitimate objective justifications.</font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">The best practice must be reasonably related to the goals it is intended to achieve.</font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Revisit practices over time as necessary to reflect the beliefs of the membership and the current state of the industry.  </font></div></li>
<li><div style="MARGIN: 0in 0in 6pt" align="left"><font size="2">Members must disclose voluntarily any proprietary interest (e.g., a patent) they may have in a particular best practice that the organization adopts; failure to disclose intellectual property and other interests in a specific best practice may lead to antitrust liability. </font></div></li>
</UL>
	<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
									 <h2 align="center">CONCLUSION</h2>		
<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>	
<div style="MARGIN: 0in 0in 0pt">This Guide is intended as an aid to assist you in understanding and fulfilling <u>your</u> responsibility to comply with CBUSA's antitrust policies.  It is not intended to make you an expert, but rather to help you identify antitrust issues that could arise in the course of your job responsibilities.  Always contact legal counsel for further guidance.</div>
	<div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
	
                                    <div style="MARGIN: 0in 0in 0pt" align="center"><font size="3">I HAVE READ AND UNDERSTOOD THE FOREGOING AGREEMENT AND AGREE TO BE BOUND BY ALL OF ITS TERMS AND CONDITIONS.&nbsp;PLEASE MANIFEST YOUR ASSENT TO THIS AGREEMENT BY CLICKING ON THE APPROPRIATE LINK BELOW.</font></div>
                                    <div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
                                    <table style="MARGIN: auto auto auto 0.2in; BORDER-COLLAPSE: collapse" cellspacing="0" cellpadding="0" border="0">
                                        <tbody>
                                            <tr>
                                                <td style="BORDER-RIGHT: #ece9d8; PADDING-RIGHT: 5.4pt; BORDER-TOP: #ece9d8; PADDING-LEFT: 5.4pt; PADDING-BOTTOM: 0in; BORDER-LEFT: #ece9d8; WIDTH: 261pt; PADDING-TOP: 0in; BORDER-BOTTOM: #ece9d8; BACKGROUND-COLOR: transparent" valign="top" width="348">
                                                <div style="MARGIN: 0in 0in 0pt" align="center"><asp:Button id="btnAcceptAsp" runat="server" text="I HAVE READ AND UNDERSTOOD AND ACCEPT THIS AGREEMENT" cssclass="btnred" onclientclick="AcceptAsp()" /><br /></div>
                                                </td>
                                                <td style="BORDER-RIGHT: #ece9d8; PADDING-RIGHT: 5.4pt; BORDER-TOP: #ece9d8; PADDING-LEFT: 5.4pt; PADDING-BOTTOM: 0in; BORDER-LEFT: #ece9d8; WIDTH: 243pt; PADDING-TOP: 0in; BORDER-BOTTOM: #ece9d8; BACKGROUND-COLOR: transparent" valign="top" width="324">
                                                <div style="MARGIN: 0in 0in 0pt" align="center"><asp:Button id="btnCancelAsp" runat="server" text="I REJECT THIS AGREEMENT" cssclass="btnred" onclientclick="RejectAsp()" /></div>
                                                </td>

                                                  <td style="BORDER-RIGHT: #ece9d8; PADDING-RIGHT: 5.4pt; BORDER-TOP: #ece9d8; PADDING-LEFT: 5.4pt; PADDING-BOTTOM: 0in; BORDER-LEFT: #ece9d8; WIDTH: 243pt; PADDING-TOP: 0in; BORDER-BOTTOM: #ece9d8; BACKGROUND-COLOR: transparent" valign="top" width="324">
                                                <div style="MARGIN: 0in 0in 0pt" align="center">  <input type="button" class="btnred" value="Print This Page" onclick="window.open('BuilderEULA.aspx', 'PrintPage', ''); return false;" /></div>
                                                </td>

                                            </tr>
                                        </tbody>
                                    </table>    
                                    </div>
                                </div>
                            </FormTemplate>
                            <Buttons>
                                <CC:PopupFormButton ControlID="btnAcceptAsp" ButtonType="ScriptOnly" />
                                <CC:PopupFormButton ControlID="btnCancelAsp" ButtonType="ScriptOnly" />
                            </Buttons>
                        </CC:PopupForm>
                    </td>
                </tr>
            </table>
        </td>    
        </tr>                


</table>      
<div style="clear:both;">&nbsp;</div>
<p style="text-align:center;">
    <CC:OneClickButton ID="btnContinue" runat="server" Text="Continue" CssClass="btnred"  />
    <CC:OneClickButton ID="btnDashboard" runat="server" Text="Save" cssclass="btnred" />
    <CC:OneClickButton ID="btnCancel" runat="server" Text="Cancel" CssClass="btnred" OnClientClick="history.go(-1);return false;" />
</p>


<CC:RequiredFieldValidatorFront ID="rfvtxtFirstName" runat="server" ControlToValidate="txtFirstName" ErrorMessage="Field 'First Name' is blank" ></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtLastName" runat="server" ControlToValidate="txtLastName" ErrorMessage="Field 'Last Name' is blank" ></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtTitle" runat="server" ControlToValidate="txtTitle" ErrorMessage="Field 'Title' is blank"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtAccountPhone" runat="server" ControlToValidate="txtAccountPhone" ErrorMessage="You must provide a 'Phone' for membership" ></CC:RequiredFieldValidatorFront>
<CC:PhoneValidatorFront id="pvtxtAccountPhone" runat="server" ControlToValidate="txtAccountPhone" ErrorMessage="Field 'Phone' is invalid" ></CC:PhoneValidatorFront>
<CC:RequiredFieldvalidatorFront ID="rqtxtUsername" runat="server" Display="None" ControlToValidate="txtUsername" ErrorMessage="You must provide a username for your membership" />
<CC:CustomValidatorFront id="cvtxtUsername" runat="server" Display="none" ControlToValidate="txtUsername" ErrorMessage="Entered username has been already taken" /> 
<CC:requiredfieldvalidatorfront id="rqtxtPassword" runat="server" Display="none" controltoValidate="txtPassword" ErrorMessage="You must provide a password for your membership" />
<CC:PasswordValidatorFront id="pvtxtPassword" runat="server" Display="none" controltovalidate="txtPassword" ErrorMessage="Password must contain minimum 4 characters and must contain both numeric and alphabetic characters" />
<CC:comparevalidatorfront id="cvtxtPassword" runat="server" Display="none" ControlToValidate="txtPassword" ControlToCompare="txtConfirmPassword" Operator="Equal" Type="String" ErrorMessage="The passwords you entered do not match" /> 

<CC:RequiredFieldValidatorFront ID="rfvtxtCompanyName" runat='server' ControlToValidate="txtCompanyName" ErrorMessage="Field 'Company Name' is blank" ></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtAddress" runat="server" ControlToValidate="txtAddress" ErrorMessage="Field 'Address' is blank" ></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtCity" runat="server" ControlToValidate="txtCity" ErrorMessage="Field 'City' is blank" ></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvdrpState" runat="server" ControlToValidate="drpState" ErrorMessage="Field 'State' is blank" ></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtZip" runat="server" ControlToValidate="txtZip" ErrorMessage="Field 'Zip Code' is blank" ></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtPhone" runat="server" ControlToValidate="txtPhone" ErrorMessage="Field 'Phone' is blank" ></CC:RequiredFieldValidatorFront>
<CC:PhoneValidatorFront id="pvftxtPhone" runat="server" ControlToValidate="txtPhone" ErrorMessage="Field 'Phone' is invalid" ></CC:PhoneValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Field 'Company Email' is blank" ></CC:RequiredFieldValidatorFront>
<CC:EmailValidatorFront id="evftxtEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Field 'Company Email' is invalid" ></CC:EmailValidatorFront>
<CC:RequiredCheckboxValidatorFront ID="rfvcbAsp" ControlToValidate="cbAsp" ErrorMessage="You must agree to the End User License Agreement before you can proceed." Display="Dynamic" runat="server"></CC:RequiredCheckboxValidatorFront>

</asp:Panel>
</CT:MasterPage>