<%@ Page Language="VB" ValidateRequest="False" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="PIQ"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<asp:ScriptManager ID="AjaxManager" runat="server" EnablePartialRendering="true" EnablePageMethods="true"></asp:ScriptManager>
	
<h4><% If PIQID = 0 Then %>Add<% Else %>Edit<% End If %> PIQ</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Company Name:</td>
		<td class="field"><asp:textbox id="txtCompanyName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvCompanyName" runat="server" Display="Dynamic" ControlToValidate="txtCompanyName" ErrorMessage="Field 'Company Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Address:</td>
		<td class="field"><asp:textbox id="txtAddress" runat="server" maxlength="250" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvAddress" runat="server" Display="Dynamic" ControlToValidate="txtAddress" ErrorMessage="Field 'Address' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Address 2:</td>
		<td class="field"><asp:textbox id="txtAddress2" runat="server" maxlength="250" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">City:</td>
		<td class="field"><asp:textbox id="txtCity" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvCity" runat="server" Display="Dynamic" ControlToValidate="txtCity" ErrorMessage="Field 'City' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">State:</td>
		<td class="field"><asp:DropDownList id="drpState" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvState" runat="server" Display="Dynamic" ControlToValidate="drpState" ErrorMessage="Field 'State' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Zip:</td>
		<td class="field"><CC:Zip id="ctrlZip" runat="server" /></td>
		<td><CC:RequiredZipValidator ID="rfvZip" runat="server" Display="Dynamic" ControlToValidate="ctrlZip" ErrorMessage="Field 'Zip' is blank"></CC:RequiredZipValidator><CC:ZipValidator Display="Dynamic" runat="server" id="fvZip" ControlToValidate="ctrlZip" ErrorMessage="Zip 'Zip' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Phone:</td>
		<td class="field"><asp:textbox id="txtPhone" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvPhone" runat="server" Display="Dynamic" ControlToValidate="txtPhone" ErrorMessage="Field 'Phone' is blank"></asp:RequiredFieldValidator></td>
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
		<td class="optional">Website Address:</td>
		<td class="field"><asp:textbox id="txtWebsiteURL" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox><br/><span class="smaller">http:// or https:// are required</span></td>
		<td><CC:URLValidator Display="Dynamic" runat="server" id="lnkvWebsiteURL" ControlToValidate="txtWebsiteURL" ErrorMessage="Link 'Website Address' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Contact First Name:</td>
		<td class="field"><asp:textbox id="txtFirstName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvFirstName" runat="server" Display="Dynamic" ControlToValidate="txtFirstName" ErrorMessage="Field 'Contact First Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Contact Last Name:</td>
		<td class="field"><asp:textbox id="txtLastName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvLastName" runat="server" Display="Dynamic" ControlToValidate="txtLastName" ErrorMessage="Field 'ContactLast Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Contact Email:</td>
		<td class="field"><asp:textbox id="txtEmail" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvEmail" runat="server" Display="Dynamic" ControlToValidate="txtEmail" ErrorMessage="Field 'Contact Email' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Logo:</td>
		<td class="field"><CC:FileUpload ID="fuLogoFile" Folder="/assets/piq/logo/original/" ImageDisplayFolder="/assets/piq/logo/standard/" DisplayImage="True" runat="server" style="width: 319px;" /></td>
		<td><CC:FileUploadExtensionValidator Extensions="jpg,jpeg,gif,bmp,png" ID="feLogoFile" runat="server" Display="Dynamic" ControlToValidate="fuLogoFile" ErrorMessage="Invalid file extension"></CC:FileUploadExtensionValidator></td>
	</tr>
	<tr>
		<td class="optional">Incentive Programs:</td>
		<td class="field"><CC:CKeditor id="txtIncentivePrograms" runat="server" Width="600" Height="300" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Preferred Vendors:</td>
		<td class="field">
		    <asp:UpdatePanel id="up" runat="server">
                <Triggers>
                </Triggers>
                <ContentTemplate>
                    <asp:Repeater ID="rptVendors" runat="server">
                        <HeaderTemplate>
                            <table>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><asp:Literal ID="ltrVendorId" runat="server"></asp:Literal></td>
                                <td><asp:Literal ID="ltrCompanyName" runat="server"></asp:Literal></td>
                                <td><asp:ImageButton ID="btnDelete" runat="server" ImageUrl="/images/admin/delete.gif" CausesValidation="false" /></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="200">
                        <ProgressTemplate>
                            <img alt="Loading...Please wait." src="/images/ajaxloading5.gif" />
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <asp:Literal ID="ltrMsg" runat="server"></asp:Literal>
                    <asp:DropDownList ID="drpVendor" runat="server" ></asp:DropDownList>
                    <asp:button ID="btnAddVendor" runat="server" Text="Add Vendor" CausesValidation="false" class="btn" />
                </ContentTemplate>
            </asp:UpdatePanel>
		    
        </td>
	</tr>
<%--	<tr>
	    <td class="optional"><b>Ads/Call Outs</b></td>
	    <td class="field">
	        <CC:SubForm id="sfAds" runat="server" InitialRows="1">
                <Fields>
                    <CC:SubFormField ID="SubFormField1" FieldName="Ad/Call Out" runat="server">
                        <HtmlTemplate>
                            <CC:SubFormFileUpload ID="fuAdFile" Folder="/assets/piq/ads" ImageDisplayFolder="/assets/piq/ads/thumbnails/" DisplayImage="True" runat="server" style="width: 175px;" />
                            <div id="divAdFileError" runat="server" class="smallest bold" style="display:none;">Ad Image is required</div>
                        </HtmlTemplate>
                        <Inputs>
                            <CC:SubFormInput ServerId="fuAdFile"  />
                        </Inputs>
                    </CC:SubFormField>
                    <CC:SubFormField ID="SubFormField2" FieldName="Alternate Text" runat="server">
                        <HtmlTemplate>
                            <asp:TextBox id="txtAltText" runat="server" columns="50" maxlength="50" style="width:75px;"></asp:TextBox>
                            <div id="divAltTextError" runat="server" class="smallest bold" style="display:none;">Alternate Text is required</div>
                        </HtmlTemplate>
                        <Inputs>
                            <CC:SubFormInput ServerId="txtAltText" ErrorSpanId="divAltTextError" ValidateFunction="function() {return !isEmptyField(this)}" />
                        </Inputs>
                    </CC:SubFormField>
                    <CC:SubFormField ID="SubFormField3" FieldName="Ad Link" runat="server">
                        <HtmlTemplate>
                            <asp:textbox id="txtLinkURL" runat="server" maxlength="500" columns="50" style="width: 75px;"></asp:textbox>
                            <div id="divLinkURLError" runat="server" class="smallest bold" style="display:none;">Ad Link is invalid</div>
                        </HtmlTemplate>
                        <Inputs>
                            <CC:SubFormInput ServerId="txtLinkURL" ErrorSpanId="divLinkURLError" ValidateFunction="function() {return isURL(this) & !isEmptyField(this)}" />
                        </Inputs>
                    </CC:SubFormField>
                    <CC:SubFormField ID="SubFormField4" FieldName="Start Date" runat="server">
                        <HtmlTemplate>
                            <CC:DatePicker ID="dpStartDate" runat="server"></CC:DatePicker>
                            <div id="divStartDateError" runat="server" class="smallest bold" style="display:none;">Start Date is invalid</div>                       
                         </HtmlTemplate>
                        <Inputs>
                            <CC:SubFormInput ServerId="dpStartDate" ErrorSpanId="divStartDateError" ClientIdSuffix="_cal" UniqueIdSuffix="$cal" ValidateFunction="function() {return isDate(this)}" />
                        </Inputs>
                    </CC:SubFormField>
                    <CC:SubFormField ID="SubFormField5" FieldName="End Date" runat="server">
                        <HtmlTemplate>
                            <CC:DatePicker ID="dpEndDate" runat="server"></CC:DatePicker>
                            <div id="divEndDateError" runat="server" class="smallest bold" style="display:none;">End Date is invalid</div>                       
                         </HtmlTemplate>
                        <Inputs>
                            <CC:SubFormInput ServerId="dpEndDate" ErrorSpanId="divEndDateError" ClientIdSuffix="_cal" UniqueIdSuffix="$cal" ValidateFunction="function() {return isDate(this)}" />
                        </Inputs>
                    </CC:SubFormField>
                    <CC:SubFormField ID="SubFormField6" FieldName="Is Active?" runat="server">
                        <HtmlTemplate>
                            <asp:RadioButtonList runat="server" ID="rblIsActive" RepeatDirection="Horizontal">
		                        <asp:ListItem Text="Yes" Value="True" />
		                        <asp:ListItem Text="No" Value="False" Selected="True" />
		                    </asp:RadioButtonList>
                        </HtmlTemplate>
                        <Inputs>
                            <CC:SubFormInput ServerId="rblIsActive"/>
                        </Inputs>
                    </CC:SubFormField>
                </Fields>
            </CC:SubForm>
	    </td>
	</tr>--%>
	<tr>
		<td class="required"><b>Is Active?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsActive" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvIsActive" runat="server" Display="Dynamic" ControlToValidate="rblIsActive" ErrorMessage="Field 'Is Active' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Start Date:</td>
		<td class="field"><CC:DatePicker ID="dtStartDate" runat="server"></CC:DatePicker></td>
		<td><CC:RequiredDateValidator Display="Dynamic" runat="server" id="rdtvStartDate" ControlToValidate="dtStartDate" ErrorMessage="Date Field 'Start Date' is blank" /><CC:DateValidator Display="Dynamic" runat="server" id="dtvStartDate" ControlToValidate="dtStartDate" ErrorMessage="Date Field 'Start Date' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">End Date:</td>
		<td class="field"><CC:DatePicker ID="dtEndDate" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvEndDate" ControlToValidate="dtEndDate" ErrorMessage="Date Field 'End Date' is invalid" /></td>
	</tr>
	<tr>
		<td class="required"><b>Has Catalog Access?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblHasCatalogAccess" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ControlToValidate="rblIsActive" ErrorMessage="Field 'Is Active' is blank"></asp:RequiredFieldValidator></td>
	</tr>

    
    <tr>
		<td class="required"><b>Has Documents Access?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblDocumentAccess" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" Selected="True" />
			<asp:ListItem Text="No" Value="False" />
			</asp:RadioButtonList>
		</td>
		<td></td>
	</tr>


</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this PIQ?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

