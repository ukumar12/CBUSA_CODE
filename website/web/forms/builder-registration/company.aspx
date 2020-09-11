<%@ Page Language="VB" AutoEventWireup="false" CodeFile="company.aspx.vb" Inherits="forms_builder_registration_company" %>
<%@ Import Namespace="DataLayer" %>
<%@ Register TagName="BuilderRegistrationSteps" TagPrefix="CC" Src="~/controls/BuilderRegistrationSteps.ascx" %>


<CT:MasterPage id="CTMain" runat="server">
<asp:PlaceHolder runat="server">
<script type="text/javascript">
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
</script>

<div style="margin:10px auto;text-align:center;">
    <CC:BuilderRegistrationSteps ID="ctrlSteps" runat="server" RegistrationStep="2" />
</div>
<asp:Panel id="pnlForm" runat="server" cssclass="pckggraywrpr">
    <div class="pckghdgred nobdr">Company Profile</div>
    <table cellpadding="5" cellspacing="15" border="0" style="width:100%;">
        <tr valign="middle">
        <td align="center">
            <table cellpadding="2" cellspacing="2" border="0">
                <tr>
                    <td>&nbsp;</td>
                    <td class="fieldreq">&nbsp;</td>
                    <td><span class="smaller"> indicates required field</span></td>
                </tr>
                <tr>
                    <td class="fieldlbl"><span id="labeltxtNumYears" runat="server">Year Business Started:</span></td>
                    <td class="fieldreq" id="bartxtNumYears" runat="server">&nbsp;</td>
                    <td class="field"><asp:TextBox ID="txtNumYears" runat="server" MaxLength="4" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderFinance"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="fieldlbl"><span id="labeltxtNumEmployees" runat="server">Number of Employees:</span></td>
                    <td class="fieldreq" id="bartxtNumEmployees" runat="server">&nbsp;</td>
                    <td class="field"><asp:TextBox ID="txtNumEmployees" runat="server" MaxLength="10" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderFinance"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="fieldlbl"><span id="labeltxtNumDelivered" runat="server">Estimated Homes Built Since In Business:</span></td>
                    <td class="fieldreq" id="bartxtNumDelivered" runat="server">&nbsp;</td>
                    <td class="field"><asp:TextBox ID="txtNumDelivered" runat="server" MaxLength="10" Columns="10" CssClass="regtxtshort" ValidationGroup="BuilderFinance"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="fieldlbl"><span id="labeltxtPriceRangeMin" runat="server">Price Range:</span></td>
                    <td class="fieldreq" id="bartxtPriceRangeMin" runat="server">&nbsp;</td>
                    <td class="field">
                        <div style="float:left;" class="bold smaller center">
                            Minimum:<br />
                            <span><asp:TextBox ID="txtPriceRangeMin" runat="server" MaxLength="10" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderFinance"></asp:TextBox></span>
                        </div>
                        <div style="float:left;margin-left:15px;" class="bold smaller center">
                            Maximum:<br />
                            <span><asp:TextBox ID="txtPriceRangeMax" runat="server" MaxLength="10" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderFinance"></asp:TextBox></span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="fieldlbl"><span id="labeltxtAvgPerFoot" runat="server">Avg cost ($) / sq ft</span></td>
                    <td class="fieldreq" id="bartxtAvgPerFoot" runat="server">&nbsp;</td>
                    <td class="field"><asp:TextBox ID="txtAvgPerFoot" runat="server" MaxLength="10" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderFinance"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="fieldlbl"><span id="labeltxtRevenue" runat="server">Company Revenue (<%=Sysparam.GetValue(DB,"PreviousDataYear") %>):</span></td>
                    <td class="fieldreq" id="bartxtRevenue" runat="server">&nbsp;</td>
                    <td class="field"><asp:TextBox ID="txtRevenue" runat="server" MaxLength="10" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderFinance"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="fieldlbl"><span id="labeltxtRevenueProjected" runat="server">Projected Company Revenue (<%=Sysparam.GetValue(DB,"CurrentDataYear") %>):</span></td>
                    <td class="fieldreq" id="bartxtRevenueProjected" runat="server">&nbsp;</td>
                    <td class="field"><asp:TextBox ID="txtRevenueProjected" runat="server" MaxLength="10" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderFinance"></asp:TextBox></td>
                </tr>
            </table>
        </td>
        <td align="center">
            <table cellpadding="2" cellspacing="2" border="0">            
                <tr>
                    <td class="fieldlbl"><span id="labeltxtAreas" runat="server">Areas/Cities/Counties in Which You Build</span></td>
                    <td class="fieldreq" id="bartxtAreas" runat="server">&nbsp;</td>
                    <td class="field"><asp:TextBox ID="txtAreas" runat="server" Rows="5" TextMode="MultiLine" Columns="50" ValidationGroup="BuilderFinance"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="fieldlbl">Company Memberships and/or Affiliations:</td>
                    <td>&nbsp;</td>
                    <td class="field"><asp:TextBox ID="txtMemberships" runat="server" TextMode="MultiLine" Rows="5" Columns="50" ValidationGroup="BuilderFinance"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="fieldlbl">Industry-related Awards Your Company has Received:</td>
                    <td>&nbsp;</td>
                    <td class="field"><asp:TextBox id="txtAwards" runat="server" TextMode="MultiLine" Rows="5" Columns="50" ValidationGroup="BuilderFinance"></asp:TextBox></td>
                </tr>
            </table>
        </td>
        </tr>
        <tr id="trEULA" runat="server" valign="top">
        <td colspan="2" align="center">
            <table cellspacing="2" cellpadding="2" border="0" style="text-align:left;">
                <tr>
                    <td class="fieldlbl"><span id="labelcbAsp" runat="server">I have read and agreed to the End User License Agreement:</span></td>
                    <td class="fieldreq" id="barcbAsp" runat="server" style="width:20px;">&nbsp;</td>
                    <td class="field">
                        <asp:CheckBox ID="cbAsp" runat="server" onclick="OpenAsp();" />&nbsp;<asp:Button id="btnShowAsp" runat="server" text="View End User License Agreement" cssclass="btnblue" />
                        <CC:PopupForm ID="frmAsp" runat="server" OpenMode="MoveToCenter" OpenTriggerId="btnShowAsp" Animate="true" ShowVeil="true" VeilCloses="false" CssClass="pform" style="width:900px;">
                            <FormTemplate>
                                <div class="pckggraywrpr" style="margin:0px;">
                                    <div class="pckghdgred">End User License Agreement</div>
                                    <div style="padding:10px;background-color:#fff;height:600px;overflow:auto;">
                                    <div style="MARGIN: 0in 0in 6pt" align="center"><font size="5"><strong>CUSTOM BUILDERS USA</strong></font></div>
                                    <div style="MARGIN: 0in 0in 6pt" align="center"><strong><font size="5">END USER LICENSE AGREEMENT</font></strong></div>
                                    <div style="MARGIN: 0in 0in 0pt" align="justify"><strong><u><span style="FONT-SIZE: 14pt">IMPORTANT</span></u><span style="FONT-SIZE: 11pt"> &ndash; THIS IS A LEGAL DOCUMENT.&nbsp;BEFORE INSTALLING, ACCESSING, OR USING ANYTHING, YOU SHOULD READ CAREFULLY THE FOLLOWING TERMS AND CONDITIONS CONTAINED IN THIS END USER LICENSE AGREEMENT (THE &ldquo;LICENSE AGREEMENT&rdquo;) AS THEY GOVERN YOUR USE OF CUSTOM BUILDERS USA, LLC'S (&quot;CB-USA's&quot;) ON-LINE SOFTWARE AND SYSTEM AND ITS ASSOCIATED WEB SITE(S), MATERIALS, SERVERS, EQUIPMENT, DATABASES, AND INFORMATION (THE &ldquo;CB-USA SYSTEM&rdquo;).&nbsp;CB-USA IS WILLING TO LICENSE THE USE OF THE CB-USA SYSTEM TO YOU ONLY ON THE CONDITION THAT YOU ACCEPT ALL OF THE TERMS AND CONDITIONS CONTAINED IN THIS LICENSE AGREEMENT.&nbsp;</span></strong></div>
                                    <div style="MARGIN: 0in 0in 0pt"><strong>&nbsp;</strong></div>
                                    <div style="MARGIN: 0in 0in 0pt" align="justify"><strong><span style="FONT-SIZE: 11pt">BY CLICKING &quot;</span></strong><strong><u><span style="FONT-SIZE: 14pt">I ACCEPT</span></u></strong><strong><span style="FONT-SIZE: 11pt">&quot; AT THE END OF THIS AGREEMENT OR BY INSTALLING, ACCESSING, OR USING ANY PART OF THE CB-USA SYSTEM, YOU ACKNOWLEDGE THAT YOU HAVE READ THIS AGREEMENT, THAT YOU UNDERSTAND IT AND ITS TERMS AND CONDITIONS, AND THAT YOU AGREE&nbsp;TO BE BOUND LEGALLY BY IT AND ITS TERMS AND CONDITIONS.</span></strong></div>
                                    <div style="MARGIN: 0in 0in 0pt"><strong>&nbsp;</strong></div>
                                    <div style="MARGIN: 0in 0in 0pt" align="justify"><strong><span style="FONT-SIZE: 11pt">IF YOU DO NOT AGREE WITH THIS LICENSE AGREEMENT, YOU ARE NOT GRANTED PERMISSION BY CB-USA TO INSTALL, ACCESS, OR OTHERWISE USE THE CB-USA SYSTEM.&nbsp;IN SUCH CASE, PLEASE CLICK &quot;</span></strong><strong><u><span style="FONT-SIZE: 14pt">I REJECT</span></u></strong><strong><span style="FONT-SIZE: 11pt">&quot; AND PROMPTLY RETURN AND/OR DELETE ANY MATERIALS RELATED TO THE CB-USA SYSTEM THAT YOU HAVE RECEIVED FROM CB-USA OR THAT YOU HAVE IN YOUR POSSESSION.&nbsp;</span></strong></div>
                                    <div style="MARGIN: 0in 0in 0pt">&nbsp;</div>
                                    <div style="MARGIN: 0in 0in 6pt" align="center"><font size="5"><strong>TERMS AND CONDITIONS</strong></font></div>
                                    <ol>
                                        <li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2"><strong>LICENSE GRANT.</strong>&nbsp;Conditioned on your compliance with the terms and conditions of this License Agreement, including, without limitation, the obligation to pay all applicable fees and charges, this License Agreement provides you with a personal, revocable, limited, non-exclusive, non-sublicenseable, and nontransferable license to use the CB-USA System for your internal business purposes only and solely in connection with your internal <span>research and marketing.&nbsp;Specifically, this license permits you to (i) use the CB-USA System on a single laptop, terminal, workstation, or computer; (ii) access the CB-USA System from the Internet or through an on-line network, (iii) load the CB-USA System into your computer's temporary memory (RAM); and (iv) create printouts of output from the CB-USA System on a single-use, single copy basis (and not for further resale, display, transmission, or distribution).&nbsp;Any rights granted hereby are licensed and not sold or otherwise transferred or assigned to you or any third party. &nbsp;References to &quot;you&quot; or &quot;user&quot; mean the corporate or individual licensee of the CB-USA System and any permitted assign thereof.</span></font></div>
                                        </li>
                                        <li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2"><strong>LICENSE GRANT RESTRICTIONS.&nbsp;</strong><span style="FONT-SIZE: 10pt">Except as expressly permitted in this License Agreement, you may not (i) copy, store, reproduce, transmit, distribute, display, rent, lease, sell, modify, alter, license, sublicense ,or commercially exploit the CB-USA System (or any part thereof); (ii) reverse engineer, decompile, disassemble, translate, or create any derivative work of the CB-USA System (or any part thereof); (iii) access, link to, or use any source code from the CB-USA System (or any part thereof); (iv) erase or remove any proprietary or intellectual property notice contained in or on the CB-USA System (or any part thereof); (v) alter or modify any information displayed, transmitted, or printed from the CB-USA System; or (vi) use or permit use of the CB-USA System for or by any other person or entity (including, without limitation, any affiliates and subsidiaries).&nbsp;In addition, you shall not enter into any contractual relationship or other legally binding obligation with any third party or person which shall have the purpose or effect of encumbering CB-USA or use of the CB-USA System.&nbsp;You acknowledge and agree that exceeding the scope of the license herein shall be a material breach of this License Agreement and subject to the termination provisions set forth herein.&nbsp;&nbsp;</span></font></div>
                                        </li>
                                        <li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2"><strong>USER OBLIGATIONS.</strong>&nbsp;By installing, accessing, and using the CB-USA System, you represent that you agree to be responsible for and to abide by all applicable local, state, national, and international laws and regulations with respect to your use of the CB-USA System.&nbsp;You also represent that you are at least eighteen (18) years of age and will, at all times, provide true, accurate, current, and complete information when submitting information or materials on this CB-USA System, including, without limitation, when you provide information via a CB-USA System registration or submission form.&nbsp;If you provide any false, inaccurate, untrue, or incomplete information, CB-USA reserves the right to terminate immediately your access to and use of this CB-USA System. &nbsp;<span style="FONT-SIZE: 10pt">Subject to CB-USA's right to monitor and audit compliance with the terms and conditions of this License Agreement, you also acknowledge and agree that it is your responsibility to monitor your use of the CB-USA System in order to maintain compliance with the terms and conditions of this License Agreement.&nbsp;Accordingly, you </span><span style="FONT-SIZE: 10pt">agree to assume all responsibility for your use, and the results of your use, of the CB-USA System, including meeting any requirements of your contracts with third parties.&nbsp;CB-USA assumes no responsibility or liability for any claims that may result directly or indirectly from the communications or interactions you establish using the CB-USA System.&nbsp;</span><span style="FONT-SIZE: 10pt">In addition, you shall be responsible for obtaining, paying for, or providing communication lines, parts, modems, interface equipment, computers, servers, laptops, and workstations as necessary for use and maintenance of the connections and capacity between you and the CB-USA System.&nbsp;In addition, you agree to be responsible for obtaining and paying for all licenses for third party software, hardware, and firmware necessary for use and implementation of the CB-USA System.&nbsp;Moreover, although CB-USA uses commercially reasonable efforts to maintain its CB-USA System, you agree that it shall be solely your responsibility for maintaining copies, backing-up, and/or archiving all of your data or information which you use on or in connection with the CB-USA System.</span></font></div>
                                        </li>
                                        <li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2"><strong><span>DELIVERY OF INFORMATION</span>.</strong>&nbsp;In connection with use of the CB-USA System, you may provide CB-USA with additional information or data. <span>&nbsp;&nbsp;With respect to any such data, you must obtain at your sole expense all necessary consents, rights, permissions, and clearances, and provide CB-USA with reasonable proof thereof (if requested), required for CB-USA to use such information or data in connection with the CB-USA System.&nbsp;In connection with delivering and providing to CB-USA any information or data, you hereby grant to CB-USA a non-exclusive right and license to copy, distribute, create derivative works from, display, modify, reformat, transmit, and otherwise use any such information or data in order to enable CB-USA to use such information or data as necessary in connection with its CB-USA System.&nbsp;Notwithstanding the foregoing, you acknowledge and agree that CB-USA shall not be responsible for any failures, inoperability, delays, or problems caused by your failure to provide any necessary information or data for your use of the CB-USA System in a timely or accurate manner.</span></font></div>
                                        </li>
                                        <li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2"><strong><span>ADDITIONAL SERVICES</span>.</strong>&nbsp;Subject to CB-USA's sole discretion, CB-USA can provide additional services to you upon request and on a time-and-materials basis at CB-USA's then-current rates.&nbsp;Any additional services, however, shall be provided pursuant to the terms and conditions of a separate written agreement with CB-USA.</font></div>
                                        </li>
                                        <li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2"><span><strong>USE OF YOUR BUSINESS NAME.</strong></span><span style="FONT-SIZE: 10pt">&nbsp;You agree that CB-USA may use your business name to disclose that you are a licensee and/or user of CB-USA&rsquo;s products and services in CB-USA&rsquo;s advertising, promotion, and similar public disclosures with respect to the CB-USA System; provided, however, that such advertising, promotion, or similar public disclosures shall not indicate that you in any way endorse CB-USA&rsquo;s products or services, without the your permission.&nbsp;&nbsp;</span></font></div>
                                        </li>
                                        <li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2"><strong>PROPRIETARY RIGHTS.</strong>&nbsp;<span style="FONT-SIZE: 10pt">CB-USA reserves all rights, title, and interests not expressly granted herein.&nbsp;Moreover, </span><span style="FONT-SIZE: 10pt">CB-USA retains all ownership right, title, and interest in and to all programs, software, materials, and documentation associated with the CB-USA System as well as any databases or information compiled, collected, or associated with the CB-USA System.&nbsp;</span><span>CBUSA, CBUSA Plus Logo Design, CUSTOMBUILDERS USA, and all other names, logos, and icons identifying CB-USA's products and services are proprietary marks of CB-USA and/or its licensors and any use of such marks by you shall inure to the benefit of CB-USA and/or its licensors.&nbsp;You also agree that any use of such marks without CB-USA's prior written consent is strictly prohibited.&nbsp;To the extent permitted by law, as between you and CB-USA, CB-USA shall own any data created or generated in connection with your use of the CB-USA System.&nbsp;In connection with the foregoing and in accordance with any applicable laws, CB-USA shall have the right and license to use such data.&nbsp;But, CB-USA will not sell by itself or otherwise separately market by itself such data.&nbsp;Except as expressly provided herein, CB-USA does not grant any express or implied right to you or any other person under any intellectual or proprietary rights.&nbsp;Accordingly, unauthorized use of the CB-USA System may violate this License Agreement and intellectual property or other proprietary rights laws as well as other domestic and international laws, regulations, and statutes.&nbsp;</span></font></div>
                                        </li>
                                        <li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2"><strong>CONFIDENTIALITY.</strong>&nbsp;You acknowledge and agree that in connection with this License Agreement and use of the CB-USA System you may receive or gain access to the confidential, proprietary, or sensitive information of CB-USA and/or its licensors and suppliers, including, without limitation, information from CB-USA's building suppliers concerning pricing and discount information, cooperative or otherwise, on lumber and other building materials and services (the &quot;Confidential Information&quot;).&nbsp;<span style="FONT-SIZE: 10pt">Moreover, you acknowledge and agree that (i) the CB-USA System includes unpublished, licensed works, and trade secrets; (ii) independent economic advantages are derived by CB-USA from its ownership of the CB-USA System; and (iii) the CB-USA System is also Confidential Information.&nbsp;You also agree that this License Agreement and its terms are and shall be the Confidential Information of CB-USA.&nbsp;Accordingly, </span><span style="FONT-SIZE: 10pt">with respect to the Confidential Information of CB-USA, you agree to secure and protect the confidentiality of the Confidential Information of CB-USA (and/or its licensors and suppliers) in a manner consistent with the maintenance of CB-USA's rights therein, using at least as great a degree of care as you use to maintain the confidentiality of your own confidential information of a similar nature, but in no event using less than reasonable efforts.&nbsp;You also acknowledge and agree that the Confidential Information of CB-USA is a valuable and material asset of CB-USA and that any disclosure or unauthorized use of such Confidential Information would be detrimental to CB-USA and its business and goodwill.&nbsp;You therefore shall not, nor permit any third party to, sell, transfer, publish, disclose, or otherwise make available any portion of the Confidential Information to third parties, except as expressly authorized in this License Agreement.&nbsp;In particular, you acknowledge and agree that you are prohibited from disclosing or making accessible any Confidential Information of CB-USA to any non-member or to any building material supplier.&nbsp;</span><span style="FONT-SIZE: 10pt">In addition, you may not disclose this License Agreement and/or its terms to any third party or person, except as may reasonably be required to enforce the terms of this License Agreement, and/or to your attorneys or accountants or as otherwise required by law, subject in all cases to any permitted third p<span style="LETTER-SPACING: -0.15pt">arty</span> or person being under the same obligation to keep the information confidential as called for in this License Agreement.&nbsp;</span><span>All Confidential Information of CB-USA shall remain the exclusive property of CB-USA.&nbsp;These restrictions do not apply to Confidential Information which you (i) are required by law or regulation to disclose, but only to the extent and for the purposes of such law or regulation; (ii) disclose in response to a valid order of a court or other governmental body, but only to the extent of and for the purposes of such order, and only if you first notify CB-USA of the order and permit CB-USA to seek an appropriate protective order or move to quash or limit such order; or (iii) disclose with written permission of CB-USA, in compliance with any terms or conditions set by CB-USA regarding such disclosure.&nbsp;Upon termination or expiration of this License Agreement, you shall return to CB-USA or destroy, at the request of CB-USA, all Confidential Information of CB-USA and certify in writing to CB-USA, within ten (10) days following termination or expiration, that all such Confidential Information has been returned or destroyed.&nbsp;</span>&nbsp;</font></div>
                                        </li>
                                        <li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2"><strong><span>PAYMENT</span>.</strong>&nbsp;You agree to pay CB-USA the applicable fees and charges for use of the CB-USA System and any other services, solutions, and information provided by CB-USA under or in connection with this License Agreement.&nbsp;Unless otherwise expressly agreed to in writing by CB-USA, you acknowledge and agree that all fees are non-refundable.&nbsp;All payments shall be made in U.S. Dollars to CB-USA via the CB-USA System or as otherwise designated by CB-USA.&nbsp;CB-USA reserves the right to terminate or suspend access to the CB-USA System if you fail to pay any amounts when due in a timely manner.&nbsp;Moreover, you acknowledge and agree that CB-USA may seek from you the reimbursement of all reasonable costs incurred (including reasonable attorney&rsquo;s fees) in collecting past-due amounts.&nbsp;Unless otherwise specified herein, all obligations with respect to the amounts due to CB-USA shall survive any expiration or termination of this License Agreement. </font></div>
                                        </li>
                                        <li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2"><strong>ENFORCEMENT.</strong> &nbsp;<span style="FONT-SIZE: 10pt">You acknowledge that CB-USA reserves the right, at any time and without notice, to monitor compliance with the terms and conditions of this License Agreement and to otherwise protect its rights in the CB-USA System by incorporating security and management technology into the CB-USA System and monitoring usage, including, without limitation, time, date, and access.&nbsp;</span><span style="FONT-SIZE: 10pt">Any information obtained by monitoring, reviewing, or recording is subject to review by law enforcement organizations in connection with investigation or prosecution of possible criminal activity on the CB-USA System.&nbsp;CB-USA will also comply with all court orders involving requests for such information.&nbsp;Moreover, </span><span>CB-USA reserves the right to suspend or terminate immediately your access to the CB-USA System if you fail to comply with the terms and conditions of this License Agreement.&nbsp;In such event, CB-USA shall be relieved of its obligations under this License Agreement during the period of suspension and shall not be found to be in breach of this License Agreement for such relief.&nbsp;You also acknowledge that any breach, threatened or actual, of this License Agreement will cause irreparable injury to CB-USA and/or its licensors or suppliers, such injury would not be quantifiable in monetary damages, and CB-USA and/or its licensors or suppliers would not have an adequate remedy at law.&nbsp;You therefore agree that CB-USA and/or its licensors or suppliers (or on their behalf) shall be entitled, in addition to other available remedies, to terminate or suspend immediately your membership with CB-USA and seek and be awarded an injunction or other appropriate equitable relief from a court of competent jurisdiction restraining any breach, threatened or actual, of your obligations under any provision of this License Agreement.&nbsp;Accordingly, you hereby waive any requirement that CB-USA or its licensors or suppliers post any bond or other security in the event any injunctive or equitable relief is sought by or awarded to CB-USA to enforce any provision of this License Agreement.&nbsp;</span></font></div>
                                        </li>
                                        <li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2"><strong>SECURITY.</strong>&nbsp;You shall not, nor shall you permit any third party to, disable, circumvent, or otherwise avoid any security device, mechanism, protocol, or procedure established by CB-USA for use of the CB-USA System.&nbsp;You will immediately notify CB-USA if you become aware of any unauthorized use of the CB-USA System.&nbsp;</font></div>
                                        </li>
                                        <li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2"><strong>SUBMISSIONS.</strong>&nbsp;CB-USA welcomes your feedback and suggestions about CB-USA's products or services or with respect to how to improve this CB-USA System.&nbsp;By transmitting any suggestions, information, material, or other content (collectively, &ldquo;feedback&rdquo;) to CB-USA, you represent and warrant that such feedback does not infringe or violate the intellectual property or proprietary rights of any third party (including, without limitation, patents, copyrights, or trademark rights) and that you have all rights necessary to convey such feedback to CB-USA.&nbsp;In addition, any feedback received through this CB-USA System will be deemed to include a royalty-free, perpetual, irrevocable, transferable, non-exclusive right and license for CB-USA to adopt, publish, reproduce, disseminate, transmit, distribute, copy, use, create derivative works, and display (in whole or in part) worldwide, or act on such feedback without additional approval or consideration, in any form, media, or technology now known or later developed for the full term of any rights that may exist in such content, and you hereby waive any claim to the contrary.</font></div>
                                        </li>
                                        <li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2"><strong>WARRANTY DISCLAIMER.</strong>&nbsp;EXCEPT AS EXPRESSLY REPRESENTED IN THIS AGREEMENT, THE CB-USA SYSTEM IS PROVIDED &ldquo;AS IS&rdquo; AND &quot;AS AVAILABLE,&quot; AND CB-USA HEREBY DISCLAIMS ANY AND ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, WITHOUT LIMITATION, ANY IMPLIED WARRANTIES OF TITLE, NON-INFRINGEMENT, MERCHANTABILITY, OR FITNESS FOR A PARTICULAR PURPOSE. <span style="LETTER-SPACING: -0.15pt">&nbsp;CB-USA DOES NOT WARRANT, GUARANTEE, OR MAKE ANY REPRESENTATIONS REGARDING THE USE, OR THE RESULTS OF THE USE, OF THE CB-USA SYSTEM, SERVICES, AND/OR SUPPORT IN TERMS OF AVAILABILITY, ACCURACY, RELIABILITY, CURRENTNESS, COMPLETENESS, FUNCTIONALITY, INTENDED PURPOSE, OR OTHERWISE.&nbsp;CB-USA ALSO DOES NOT REPRESENT OR WARRANT THAT THE CB-USA SYSTEM WILL OPERATE ERROR-FREE, UNINTERRUPTED, OR IN A MANNER THAT WILL MEET YOUR REQUIREMENTS.&nbsp;THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE CB-USA SYSTEM IS WITH YOU.&nbsp;</span></font></div>
                                        </li>
                                        <li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2"><strong>LIMITATION OF LIABILITY.</strong>&nbsp;YOU AGREE THAT IN NO EVENT SHALL CB-USA BE LIABLE FOR ANY INDIRECT, PUNITIVE, INCIDENTAL, SPECIAL, OR CONSEQUENTIAL DAMAGES ARISING OUT OF OR IN ANY WAY CONNECTED WITH YOUR USE OF THE CB-USA SYSTEM OR ANY INFORMATION, SERVICES, PROGRAMS, PRODUCTS, SERVICES, AND MATERIALS AVAILABLE WITH OR THROUGH THE CB-USA SYSTEM, WHETHER BASED IN CONTRACT, TORT, STRICT LIABILITY, OR OTHERWISE, EVEN IF CB-USA HAS BEEN ADVISED OF THE POSSIBILITY OF DAMAGES. &nbsp;<span style="FONT-SIZE: 10pt; TEXT-TRANSFORM: uppercase">YOU acknowledge and agree that certain Services and/or YOUR access to the CB-USA System may require delivery by means of or through the use of certain third-party service providers or services, such as communication services.&nbsp;ACCORDINGLY, YOU ALSO AGREE THAT CB-USA shall not be liable for any failure to perform any of its obligations under this License Agreement due to unforeseen circumstances or causes beyond CB-USA'S reasonable control, including, without limitation, acts of God, riot, embargoes, acts of governmental authorities, fire, earthquake, flood, and accidents.&nbsp;IN ADDITION, CB-USA shall assume no responsibility or liability for the delivery, security, or availability of ANY third-party SERVICE PROVIDERS OR services.&nbsp;Moreover, the CB-USA System may enable access to or link to third-party databases or other Web sites, information, resources, materials, or content.&nbsp;YOU acknowledge that CB-USA does not control the content available through such third party databases or other website, information, resources, materials, or content. &nbsp;In ADDITION, CB-USA SHALL Assume no responsibility for AND shall have no liability to YOU or to any third party for the AVAILABILITY, accuracy, timeliness, SUBSTANCE, sequence, completeness, reliability, content, or security of any of the databases, sites, information, resources, materials, or content provided by a third party.&nbsp;Any concerns YOU may have regarding these third party sites, information, resources, materials or content should be directed to the relevant site, administrator, webmaster, or provider of such site, information, resource, material, or content.&nbsp;&nbsp;WITHOUT LIMITATION OF THE FOREGOING, TOTAL LIABILITY OF CB-USA FOR ANY REASON WHATSOEVER RELATED TO USE OF THE CB-USA SYSTEM OR ANY CLAIMS RELATING TO THIS AGREEMENT SHALL NOT EXCEED $5,000 (USD). </span></font></div>
                                        </li>
                                        <li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2"><strong>INDEMNITY.</strong>&nbsp;You agree to defend, indemnify, and hold harmless CB-USA and its affiliates, employees, licensors, agents, directors, officers, partners, representatives, shareholders, servants, attorneys, predecessors, successors, and assigns from and against any and all claims, proceedings, damages, injuries, liabilities, losses, costs, and expenses (including reasonable attorneys&rsquo; fees and litigation expenses), relating to or arising from your use of the CB-USA System and any breach by you of this License Agreement.</font></div>
                                        </li>
                                        <li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2"><strong>GOVERNING LAW.</strong>&nbsp;This License Agreement has been made in and will be construed and enforced solely in accordance with the laws of the State of Delaware, U.S.A. as applied to agreements entered into and completely performed in the State of Delaware.&nbsp;In addition, you agree that any action to enforce this License Agreement will be brought solely in the federal or state courts in the Commonwealth of Virginia, U.S.A., and all parties to this License Agreement expressly agree to be subject to the jurisdiction of such courts.&nbsp;You also acknowledge and agree that any applicable state law implementation of the Uniform Computer Information Transactions Act (including any available remedies or laws) shall not apply to this License Agreement and is hereby disclaimed.&nbsp;</font></div>
                                        </li>
                                        <li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2"><strong>TERM AND TERMINATION.</strong>&nbsp;This License Agreement and your right to use the CB-USA System will take effect at the moment you click &quot;<u>I ACCEPT</u>&quot; or you install, access, or use the CB-USA System and is effective so long as you pay all applicable fees and charges in a timely manner or until terminated earlier as set forth below. &nbsp;This License Agreement will terminate automatically if you click &quot;<u>I REJECT</u>&quot; or if you fail to comply with any of the terms and conditions described herein, including, without limitation, by exceeding the scope of the license.&nbsp;Termination or expiration of this License Agreement will be effective without notice.&nbsp;You may also terminate at any time by ceasing to use the CB-USA System, but all applicable provisions of this License Agreement will survive termination, as outlined below.&nbsp;Upon termination or expiration, you must return to CB-USA and/or destroy or delete from your computer, laptop, work station, network, or system all copies of the CB-USA System (and any associated materials) in your possession.&nbsp;The provisions concerning proprietary and intellectual property rights, submissions, confidentiality, indemnity, disclaimers of warranty and liability, termination, and governing law will survive the termination or expiration of this License Agreement for any reason.</font></div>
                                        </li>
                                        <li>
                                        <div style="MARGIN: 0in 0in 6pt" align="left"><font size="2"><span><strong>MISCELLANEOUS.</strong></span><span style="FONT-SIZE: 10pt">&nbsp;</span><span style="FONT-SIZE: 10pt">The relationship between you and CB-USA is and shall be that of licensor and licensee only and nothing in this License Agreement shall be construed or used to create or imply any relationship of partners, joint venturers, or employer and employee.&nbsp;You also agree that this License Agreement is only for your benefit. &nbsp;Accordingly, you may not assign or otherwise transfer this License Agreement or the license granted hereunder or delegate any of your duties hereunder, in whole or in part, without CB-USA's prior written consent.&nbsp;Any attempt of assignment or transfer shall be void, of no effect, and a material breach of this License Agreement.&nbsp;Failure by CB-USA to insist on strict performance of any of the terms and conditions of this License Agreement will not operate as a waiver of that or any subsequent default or failure of performance.&nbsp;In the event any provision of this License Agreement is found by an arbitrator or court of competent jurisdiction to be invalid, void ,or unenforceable, you agree that unless it materially affects the entire intent and purpose of this License Agreement, the invalidity, voidness, or unenforceability shall affect neither the validity of this License Agreement nor the remaining provisions herein, and the provision in question shall be deemed to be replaced with a valid and enforceable provision most closely reflecting the intent and purpose of the original provision.&nbsp;A printed version of this License Agreement and of any related notice given in electronic form shall be admissible in judicial or administrative proceedings based upon or relating to this License Agreement to the same extent and subject to the same conditions as other business documents and records originally generated and maintained in printed form.&nbsp;</span><span style="FONT-SIZE: 10pt">This License Agreement represents the entire agreement between you and CB-USA with respect to use of the CB-USA System, and it supersedes all prior or contemporaneous communications and proposals, whether electronic, oral, or written between you and CB-USA with respect to the CB-USA System.&nbsp;Please note that CB-USA reserves the right to change the terms and conditions of this License Agreement and under which the CB-USA System is extended to you by providing you in writing or electronically a copy of such revised terms.&nbsp;CB-USA may change any aspect of the CB-USA System at any time.&nbsp;Your continued use of the CB-USA System following any such change will be conclusively deemed acceptance of any change to this License Agreement or the CB-USA System.&nbsp;If you have questions regarding the CB-USA System or if you are interested in obtaining more information concerning CB-USA and its products or services, </span><span style="FONT-SIZE: 10pt">or want permission to use any CB-USA content, </span><span style="FONT-SIZE: 10pt">please contact CB-USA at customerservice@cbusa.us.</span></font></div>
                                        </li>
                                    </ol>
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


<p style="text-align:center;">
    <CC:OneClickButton ID="btnDashboard" runat="server" Text="Save" CssClass="btnred" />
    <asp:Button id="btnBack" runat="server" text="Go Back" onclientclick="history.go(-1);return false;" cssclass="btnred" />
    <CC:OneClickButton ID="btnContinue" runat="server" Text="Continue" CssClass="btnred" />
</p>

<CC:RequiredFieldValidatorFront ID="rfvtxtNumYears" ControlToValidate="txtNumYears" ErrorMessage="# Years in Business is required" Display="Dynamic" ValidationGroup="BuilderFinance" runat="server"></CC:RequiredFieldValidatorFront>
<CC:IntegerValidatorFront ID="ivtxtNumYears" ControlToValidate="txtNumYears" ErrorMessage="# Years in Business is invalid" Display="Dynamic" ValidationGroup="BuilderFinance" runat="server"></CC:IntegerValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtNumEmployees" ControlToValidate="txtNumEmployees" ErrorMessage="# Employees is required" Display="Dynamic" ValidationGroup="BuilderFinance" runat="server"></CC:RequiredFieldValidatorFront>
<CC:IntegerValidatorFront ID="ivtxtNumEmployees" ControlToValidate="txtNumEmployees" ErrorMessage="# Employees is invalid" Display="Dynamic" ValidationGroup="BuilderFinance" runat="server"></CC:IntegerValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtNumDelivered" ControlToValidate="txtNumDelivered" ErrorMessage="# Homes Built &amp; Delivered since in Business is required" Display="Dynamic" ValidationGroup="BuilderFinance" runat="server"></CC:RequiredFieldValidatorFront>
<CC:IntegerValidatorFront ID="ivtxtNumDelivered" ControlToValidate="txtNumDelivered" ErrorMessage="# Homes Built &amp; Delivered since in Business is invalid" Display="Dynamic" ValidationGroup="BuilderFinance" runat="server"></CC:IntegerValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtPriceRangeMin" ControlToValidate="txtPriceRangeMin" ErrorMessage="Price Range Minimum is required" Display="Dynamic" ValidationGroup="BuilderFinance" runat="server"></CC:RequiredFieldValidatorFront>
<CC:CustomCurrencyValidator ID="ivtxtPriceRangeMin" EnableClientScript="false" ControlToValidate="txtPriceRangeMin" ErrorMessage="Price Range Minimum is invalid" Display="Dynamic" ValidationGroup="BuilderFinance" runat="server"></CC:CustomCurrencyValidator>
<CC:RequiredFieldValidatorFront ID="rfvtxtPriceRangeMax" ControlToValidate="txtPriceRangeMax" ErrorMessage="Price Range Maximum is required" Display="Dynamic" ValidationGroup="BuilderFinance" runat="server"></CC:RequiredFieldValidatorFront>
<CC:CustomCurrencyValidator ID="ivtxtPriceRangeMax" EnableClientScript="false" ControlToValidate="txtPriceRangeMax" ErrorMessage="Price Range Maximum is invalid" Display="Dynamic" ValidationGroup="BuilderFinance" runat="server"></CC:CustomCurrencyValidator>
<CC:RequiredFieldValidatorFront ID="rfvtxtAvgPerFoot" ControlToValidate="txtAvgPerFoot" ErrorMessage="Avg $/sq ft is required" Display="Dynamic" ValidationGroup="BuilderFinance" runat="server"></CC:RequiredFieldValidatorFront>
<CC:CustomCurrencyValidator ID="ivtxtAvgPerFoot" EnableClientScript="false" ControlToValidate="txtAvgPerFoot" ErrorMessage="Avg $/sq ft is invalid" Display="Dynamic" ValidationGroup="BuilderFinance" runat="server"></CC:CustomCurrencyValidator>
<CC:RequiredFieldValidatorFront ID="rfvtxtRevenue" ControlToValidate="txtRevenue" ErrorMessage="Company Revenue required" Display="Dynamic" ValidationGroup="BuilderFinance" runat="server"></CC:RequiredFieldValidatorFront>
<CC:CustomCurrencyValidator ID="ivtxtRevenue" EnableClientScript="false" ControlToValidate="txtRevenue" ErrorMessage="Company Revenue is invalid" Display="Dynamic" ValidationGroup="BuilderFinance" runat="server"></CC:CustomCurrencyValidator>
<CC:RequiredFieldValidatorFront ID="rfvtxtRevenueProjected" ControlToValidate="txtRevenueProjected" ErrorMessage="Projected Company Revenue required" Display="Dynamic" ValidationGroup="BuilderFinance" runat="server"></CC:RequiredFieldValidatorFront>
<CC:CustomCurrencyValidator ID="ivtxtRevenueProjected" EnableClientScript="false" ControlToValidate="txtRevenueProjected" ErrorMessage="Projected Company Revenue is invalid" Display="Dynamic" ValidationGroup="BuilderFinance" runat="server"></CC:CustomCurrencyValidator>
<CC:RequiredFieldValidatorFront ID="rfvtxtAreas" ControlToValidate="txtAreas" ErrorMessage="Company Memberships and/or Affiliations is required" Display="Dynamic" ValidationGroup="BuilderFinance" runat="server"></CC:RequiredFieldValidatorFront>
<CC:RequiredCheckboxValidatorFront ID="rfvcbAsp" ControlToValidate="cbAsp" ErrorMessage="You must agree to the End User License Agreement before you can proceed." Display="Dynamic" ValidationGroup="BuilderFinance" runat="server"></CC:RequiredCheckboxValidatorFront>
</asp:Panel>

</asp:PlaceHolder>
</CT:MasterPage>