<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit" Title="Vendor" %>

<asp:Content ContentPlaceHolderID="ph" ID="Content" runat="server">

    <h4><% If VendorID = 0 Then %>Add<% Else %>Edit<% End If %> Vendor</h4>
    <p></p>
    <CC:OneClickButton ID="btnEditOwners" runat="server" CausesValidation="false" Text="Owners" CssClass="btn"></CC:OneClickButton>
    <CC:OneClickButton ID="btnEditBranches" runat="server" CausesValidation="false" Text="Branches" CssClass="btn"></CC:OneClickButton>
    <p></p>
    <table border="0" cellspacing="1" cellpadding="2">
        <tr>
            <td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td>
        </tr>
        <tr>
            <td class="optional">CRM ID:</td>
            <td class="field">
                <asp:TextBox ID="txtCRMID" runat="server" MaxLength="20" Columns="20" Style="width: 139px;"></asp:TextBox></td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="required">Company Name:</td>
            <td class="field">
                <asp:TextBox ID="txtCompanyName" runat="server" MaxLength="50" Columns="50" Style="width: 319px;"></asp:TextBox></td>
            <td>
                <asp:RequiredFieldValidator ID="rfvCompanyName" runat="server" Display="Dynamic" ControlToValidate="txtCompanyName" ErrorMessage="Field 'Company Name' is blank"></asp:RequiredFieldValidator></td>
        </tr>
        <tr>
            <td class="required">Address:</td>
            <td class="field">
                <asp:TextBox ID="txtAddress" runat="server" MaxLength="50" Columns="50" Style="width: 319px;"></asp:TextBox></td>
            <td>
                <asp:RequiredFieldValidator ID="rfvAddress" runat="server" Display="Dynamic" ControlToValidate="txtAddress" ErrorMessage="Field 'Address' is blank"></asp:RequiredFieldValidator></td>
        </tr>
        <tr>
            <td class="optional">Address 2:</td>
            <td class="field">
                <asp:TextBox ID="txtAddress2" runat="server" MaxLength="50" Columns="50" Style="width: 319px;"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td class="required">City:</td>
            <td class="field">
                <asp:TextBox ID="txtCity" runat="server" MaxLength="50" Columns="50" Style="width: 319px;"></asp:TextBox></td>
            <td>
                <asp:RequiredFieldValidator ID="rfvCity" runat="server" Display="Dynamic" ControlToValidate="txtCity" ErrorMessage="Field 'City' is blank"></asp:RequiredFieldValidator></td>
        </tr>
        <tr>
            <td class="required">State:</td>
            <td class="field">
                <asp:DropDownList ID="drpState" runat="server" /></td>
            <td>
                <asp:RequiredFieldValidator ID="rfvState" runat="server" Display="Dynamic" ControlToValidate="drpState" ErrorMessage="Field 'State' is blank"></asp:RequiredFieldValidator></td>
        </tr>
        <tr>
            <td class="required">Zip:</td>
            <td class="field">
                <CC:Zip ID="ctrlZip" runat="server" />
            </td>
            <td>
                <CC:RequiredZipValidator ID="rfvZip" runat="server" Display="Dynamic" ControlToValidate="ctrlZip" ErrorMessage="Field 'Zip' is blank"></CC:RequiredZipValidator><CC:ZipValidator Display="Dynamic" runat="server" ID="fvZip" ControlToValidate="ctrlZip" ErrorMessage="Zip 'Zip' is invalid" /></td>
        </tr>


        <tr>
            <td class="required">Billing Address:</td>
            <td class="field">
                <asp:TextBox ID="txtBillingAddress" runat="server" MaxLength="50" Columns="50" Style="width: 319px;"></asp:TextBox></td>

        </tr>

        <tr>
            <td class="required">Billing City:</td>
            <td class="field">
                <asp:TextBox ID="txtBillingCity" runat="server" MaxLength="50" Columns="50" Style="width: 319px;"></asp:TextBox></td>

        </tr>
        <tr>
            <td class="required">Billing State:</td>
            <td class="field">
                <asp:DropDownList ID="drpbillingState" runat="server" /></td>

        </tr>
        <tr>
            <td class="required">Billing Zip:</td>
            <td class="field">
                <CC:Zip ID="ctrlbillingZip" runat="server" />
            </td>

        </tr>




        <tr>
            <td class="required">Phone:</td>
            <td class="field">
                <asp:TextBox ID="txtPhone" runat="server" MaxLength="50" Columns="50" Style="width: 319px;"></asp:TextBox></td>
            <td>
                <asp:RequiredFieldValidator ID="rfvPhone" runat="server" Display="Dynamic" ControlToValidate="txtPhone" ErrorMessage="Field 'Phone' is blank"></asp:RequiredFieldValidator></td>
        </tr>
        <tr>
            <td class="optional">Mobile:</td>
            <td class="field">
                <asp:TextBox ID="txtMobile" runat="server" MaxLength="50" Columns="50" Style="width: 319px;"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td class="optional">Pager:</td>
            <td class="field">
                <asp:TextBox ID="txtPager" runat="server" MaxLength="50" Columns="50" Style="width: 319px;"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td class="optional">Other Phone:</td>
            <td class="field">
                <asp:TextBox ID="txtOtherPhone" runat="server" MaxLength="50" Columns="50" Style="width: 319px;"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td class="optional">Fax:</td>
            <td class="field">
                <asp:TextBox ID="txtFax" runat="server" MaxLength="50" Columns="50" Style="width: 319px;"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td class="optional">Email:</td>
            <td class="field">
                <asp:TextBox ID="txtEmail" runat="server" MaxLength="100" Columns="50" Style="width: 319px;"></asp:TextBox></td>
            <td>
                <CC:EmailValidator Display="Dynamic" runat="server" ID="fvEmail" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is invalid" /></td>
            <%--<asp:RequiredFieldValidator ID="rfvEmail" runat="server" Display="Dynamic" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is blank"></asp:RequiredFieldValidator>--%>
        </tr>
        <tr>
            <td class="optional">Website URL:</td>
            <td class="field">
                <asp:TextBox ID="txtWebsiteURL" runat="server" MaxLength="100" Columns="50" Style="width: 319px;"></asp:TextBox><br />
                <span class="smaller">http:// or https:// are required</span></td>
            <td>
                <CC:URLValidator Display="Dynamic" runat="server" ID="lnkvWebsiteURL" ControlToValidate="txtWebsiteURL" ErrorMessage="Link 'Website URL' is invalid" /></td>
        </tr>
        <%--<tr>
		<td class="required">Primary Vendor Category:</td>
		<td class="field"><asp:DropDownList id="drpPrimaryVendorCategoryId" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvdrpPrimaryVendorCategoryId" runat="server" Display="Dynamic" ControlToValidate="drpPrimaryVendorCategoryId" ErrorMessage="Field 'Primary Vendor Category' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Secondary Vendor Category:</td>
		<td class="field"><asp:DropDownList id="drpSecondaryVendorCategoryId" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvdrpSecondaryVendorCategoryId" runat="server" Display="Dynamic" ControlToValidate="drpSecondaryVendorCategoryId" ErrorMessage="Field 'Secondary Vendor Category' is blank"></asp:RequiredFieldValidator></td>
	</tr>--%>
        <tr>
            <td class="optional">Category:</td>
            <td class="field">
                <CC:CheckBoxListEx ID="cblCategory" runat="server" RepeatColumns="3"></CC:CheckBoxListEx></td>
        </tr>
        <tr>
            <td class="optional">Logo:</td>
            <td class="field">
                <CC:FileUpload ID="fuLogoFile" Folder="/assets/vendor/logo/original/" ImageDisplayFolder="/assets/vendor/logo/standard/" DisplayImage="True" runat="server" Style="width: 319px;" />
            </td>
            <td>
                <CC:FileUploadExtensionValidator Extensions="jpg,jpeg,gif,bmp,png" ID="feLogoFile" runat="server" Display="Dynamic" ControlToValidate="fuLogoFile" ErrorMessage="Invalid file extension"></CC:FileUploadExtensionValidator></td>
        </tr>
        <tr>
            <td class="optional">About Us:</td>
            <td class="field">
                <asp:TextBox ID="txtAboutUs" Style="width: 349px;" Columns="55" Rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td class="required"><b>Is Active?</b></td>
            <td class="field">
                <asp:RadioButtonList runat="server" ID="rblIsActive" RepeatDirection="Horizontal">
                    <asp:ListItem Text="Yes" Value="True" />
                    <asp:ListItem Text="No" Value="False" Selected="True" />
                </asp:RadioButtonList>
            </td>
            <td>
                <asp:RequiredFieldValidator ID="rfvIsActive" runat="server" Display="Dynamic" ControlToValidate="rblIsActive" ErrorMessage="Field 'Is Active' is blank"></asp:RequiredFieldValidator></td>
        </tr>
        <tr>
            <td class="required"><b>Is Plans Online?</b></td>
            <td class="field">
                <asp:RadioButtonList runat="server" ID="rblIsPlansOnline" RepeatDirection="Horizontal">
                    <asp:ListItem Text="Yes" Value="True" />
                    <asp:ListItem Text="No" Value="False" Selected="True" />
                </asp:RadioButtonList>
            </td>
            <td></td>
        </tr>
        <tr>
            <td class="optional">Comments:</td>
            <td class="field">
                <asp:TextBox ID="txtComments" Style="width: 349px;" Columns="55" Rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td class="required">LLC:</td>
            <td class="field">
                <%--<asp:DropDownList ID="drpLLC" runat="server"></asp:DropDownList>--%>
                <CC:CheckBoxListEx ID="cblLLC" runat="server" RepeatColumns="3"></CC:CheckBoxListEx>
            </td>
        </tr>
        <tr>
            <td class="optional"><b>Enable Market Share Report?</b></td>
            <td class="field">
                <asp:CheckBox runat="server" ID="cbEnableMarketShare" />
            </td>
            <td></td>
        </tr>
        <tr>
            <td class="optional"><b>Exclude Vendor From Rebate Notices?</b></td>
            <td class="field">
                <asp:CheckBox runat="server" ID="cbExcludeVendor" />
            </td>
            <td></td>
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

        <tr>
            <td class="optional"><b>Show in Quarterly Reporting?</b></td>
            <td class="field">
                <asp:RadioButtonList runat="server" ID="rblQuarterlyReportingOn" RepeatDirection="Horizontal">
                    <asp:ListItem Text="Yes" Value="True" Selected="True" />
                    <asp:ListItem Text="No" Value="False" />
                </asp:RadioButtonList>
            </td>
            <td></td>
        </tr>

        <tr>
            <td class="required"><b>Dashboard Category</b></td>
            <td class="field">
                <asp:DropDownList ID="ddlDashboardCategory" runat="server" />
            </td>
            <td>
                <asp:RequiredFieldValidator ID="rfvDashboardCategory" runat="server" Display="Dynamic" ControlToValidate="ddlDashboardCategory" ErrorMessage="Field 'Dashboard Category' is blank"></asp:RequiredFieldValidator></td>
        </tr>
    </table>

    <p></p>

    <h5>Assign Vendor Roles</h5>
    <table border="0" cellspacing="1" cellpadding="2">
        <asp:Repeater ID="rptRoles" runat="server">
            <ItemTemplate>
                <tr>
                    <td class="required">
                        <asp:Literal ID="ltlRole" runat="server"></asp:Literal>
                        <asp:HiddenField ID="hdnRoleId" runat="server" />
                    </td>
                    <td class="field">
                        <%--<asp:DropDownList ID="drpAccount" runat="server"></asp:DropDownList>--%>
                        <asp:CheckBoxList ID="chkAccount" runat="server" RepeatLayout="Table" RepeatDirection="Horizontal" RepeatColumns ="5" AutoPostBack="false" Width="100%">
                        </asp:CheckBoxList>

                    </td>
                    <td></td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>

    <p></p>
    <CC:ConfirmButton ID="btnSave" runat="server" Text="Save" Message="Please Confirm Market Selection" CssClass="btn"></CC:ConfirmButton>
    <CC:ConfirmButton ID="btnDelete" Visible="false" runat="server" Message="Are you sure want to delete this Vendor?" Text="Delete" CssClass="btn" CausesValidation="False"></CC:ConfirmButton>
    <CC:OneClickButton ID="btnCancel" runat="server" Text="Cancel" CssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:Content>
