<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master"
    Inherits="Edit" Title="N C P Content" %>

<%@ Register Src="~/controls/Documents/MultiFileUploadNew.ascx" TagName="MultiUploadDocument"
    TagPrefix="CC" %>
<asp:Content ContentPlaceHolderID="ph" ID="Content" runat="server">
    <link rel="stylesheet" href="/includes/style.css" type="text/css" />
    <h4>
        <% If NCPContentID = 0 Then%>Add<% Else%>Edit<% End If%>
        NCP Content</h4>
    <table border="0" cellspacing="1" cellpadding="2">
        <tr>
            <td colspan="2">
                <span class="smallest"><span class="red">red color</span> - denotes required fields(You
                    will be able to add documents once you save these details.)</span>
            </td>
        </tr>
        <tr>
            <td class="required">
                Name:
            </td>
            <td class="field">
                <asp:TextBox ID="txtName" runat="server" MaxLength="255" Columns="50" Style="width: 319px;"></asp:TextBox>
            </td>
            <td>
                <asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName"
                    ErrorMessage="Field 'Name' is blank"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="required">
                Description:
            </td>
            <td class="field">
                <CC:CKEditor ID="txtDescription" Style="width: 349px;" Columns="55" rows="5" TextMode="Multiline"
                    runat="server"></CC:CKEditor>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="required">
                Contracts:
            </td>
            <td class="field">
                <CC:CheckBoxListEx ID="cblContracts" runat="server" RepeatColumns="3">
                </CC:CheckBoxListEx>
            </td>
            <td><CC:RequiredCheckboxListValidatorFront id="cblContractsAtLeastOne" Display="Dynamic" runat="server" ControlToValidate="cblContracts" ErrorMessage="You must specify at least one Contract " /> </td>
        </tr>
        <tr>
            <td class="required">
                LLC:
            </td>
            <td class="field">
                <CC:CheckBoxListEx ID="cblLLC" runat="server" RepeatColumns="3">
                </CC:CheckBoxListEx>
            </td>
             <td><CC:RequiredCheckboxListValidatorFront id="cblLLCAtleastOne" Display="Dynamic" runat="server" ControlToValidate="cblLLC" ErrorMessage="You must specify at least one LLC " /> </td>
        </tr>
        <tr>
            <td class="required">
                <b>Is Active?</b>
            </td>
            <td class="field">
                <asp:RadioButtonList runat="server" ID="rblIsActive" RepeatDirection="Horizontal">
                    <asp:ListItem Text="Yes" Value="True" />
                    <asp:ListItem Text="No" Value="False" Selected="True" />
                </asp:RadioButtonList>
            </td>
            <td>
                <asp:RequiredFieldValidator ID="rfvIsActive" runat="server" Display="Dynamic" ControlToValidate="rblIsActive"
                    ErrorMessage="Field 'Is Active' is blank"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr id="trDocUpload" runat="server">
            <td class="required">
                Documents
            </td>
            <td class="field">
                <div class="pckgltgraywrpr center">
                    <div style="overflow: auto; background-color: White; margin-top: 5px; margin-bottom: 5px;
                        margin-left: 5px; margin-right: 5px;">
                        <CC:MultiUploadDocument ID="mudUpload" runat="server"></CC:MultiUploadDocument>
                    </div>
                    <asp:ScriptManager ID="ScriptManager1" runat="server">
                    </asp:ScriptManager>
                    <asp:UpdatePanel ID="upDocuments" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="pckghdgblue">
                                <asp:Button ID="btnTransferDocs" runat="server" CssClass="btnred" Text="View Uploaded Documents" />
                            </div>
                            <div style="text-align: left; overflow: auto; height: 150px; background-color: White;
                                margin-top: 5px; margin-bottom: 5px; margin-left: 5px; margin-right: 5px;">
                                <b style="margin-left: 10px;">
                                    <asp:Literal ID="ltlDocuments" runat="server"></asp:Literal></b><br />
                                <div>
                                    <CC:GridView ID="gvDocuments" CellPadding="5" runat="server" PageSize="50" AllowPaging="True"
                                        HeaderText="" EmptyDataText="No Documents" AutoGenerateColumns="False" ShowFooter="false"
                                        PagerSettings-Position="Bottom" CssClass="MultilineTable" DataKeyNames="DocumentID"
                                        GridLines="none" HeaderStyle-BackColor="#15487D" HeaderStyle-CssClass="gvTableHeader"
                                        Width="100%" BorderColor="#022D70" BorderStyle="Solid" BorderWidth="1" HeaderStyle-ForeColor="White">
                                        <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
                                        <RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemStyle CssClass="ActionButtons" Width="10" />
                                                <ItemTemplate>
                                                    <CC:ConfirmImageButton CommandName="Remove" Message="Are you sure that you want to remove this Item?"
                                                        runat="server" ImageUrl="/images/admin/delete.gif" ID="lnkDelete"></CC:ConfirmImageButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:CommandField CausesValidation="false" ShowEditButton="true" ButtonType="Image"
                                                EditImageUrl="/images/admin/edit.gif" UpdateImageUrl="/images/admin/save.gif"
                                                CancelImageUrl="/images/admin/cancel.gif" ItemStyle-Width="100px" />
                                            <asp:BoundField DataField="Title" HeaderText="Title" ControlStyle-CssClass="titlewide" />
                                            <asp:TemplateField SortExpression="Document" HeaderText="Document">
                                                <ItemTemplate>
                                                    <asp:HyperLink EnableViewState="False" runat="server" ID="LnkDocument"><%#Eval("FileName")%></asp:HyperLink>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Uploaded" ReadOnly="True" HeaderText="Uploaded" ControlStyle-CssClass="titlewide" />
                                        </Columns>
                                    </CC:GridView>
                                    <div class="dcrow" id="divNoCurrentDocuments" runat="server">
                                        <p class="dcmessagetext">
                                            You have no documents available.</p>
                                    </div>
                                    <div class="btnhldrrt">
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
    <p>
    </p>
    <CC:OneClickButton ID="btnSave" runat="server" Text="Save" CssClass="btn"></CC:OneClickButton>
    <CC:ConfirmButton ID="btnDelete" runat="server" Message="Are you sure want to delete this N C P Content?"
        Text="Delete" CssClass="btn" CausesValidation="False"></CC:ConfirmButton>
    <CC:OneClickButton ID="btnCancel" runat="server" Text="Cancel" CssClass="btn" CausesValidation="False">
    </CC:OneClickButton>
</asp:Content>
