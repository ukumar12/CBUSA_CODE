<%@ Page Language="VB" ValidateRequest="False" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Take Off Service"%>
<%@ Register Src="~/controls/Documents/MultiFileUploadNew.ascx" TagName="MultiUploadDocument" TagPrefix="CC" %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If TakeOffServiceID = 0 Then %>Add<% Else %>Edit<% End If %> Take Off Service</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="optional">Title:</td>
		<td class="field"><asp:textbox id="txtTitle" runat="server" maxlength="255" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>

       <tr>
            <td class="required">
                LLC:
            </td>
            <td class="field">
                <CC:CheckBoxListEx ID="cblLLC" runat="server" RepeatColumns="3">
                </CC:CheckBoxListEx>
            </td>
            
        </tr>

	<tr>
		<td class="optional">Description:</td>
		<td class="field"><CC:CKEditor  id="txtDescription" runat="server" Width="600" Height="300" /></td>
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
		<td><asp:RequiredFieldValidator ID="rfvIsActive" runat="server" Display="Dynamic" ControlToValidate="rblIsActive" ErrorMessage="Field 'Is Active' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	 <tr id="trDocUpload" runat ="server" >
		<td class="required">Documents</td>
		<td class="field">
		    
		            <div class="pckgltgraywrpr center">
                       
                        <div style="overflow:auto; background-color:White; margin-top:5px; margin-bottom:5px; margin-left:5px; margin-right:5px;" >
                            <CC:MultiUploadDocument id="mudUpload" runat="server" ></CC:MultiUploadDocument>
                        </div>
                         <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                <asp:UpdatePanel ID="upDocuments" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                        <div class="pckghdgblue">
                            <asp:Button id="btnTransferDocs" runat="server" cssclass="btnred" text="Transfer Uploaded Documents" />
                        </div>
                        <div style="text-align:left; overflow:auto; height:150px; background-color:White; margin-top:5px; margin-bottom:5px; margin-left:5px; margin-right:5px;" >
                            <b style="margin-left:10px;"><asp:Literal ID="ltlDocuments" runat="server"></asp:Literal></b><br />
                            
                            <div>
                                <asp:Repeater id="rptDocuments" runat="server">
                                <HeaderTemplate>
                                    <table cellspacing="0" cellpadding="0" border="0" class="dshbrdtbl">
			                            <tr>
			                                <th>Document</th>
			                                <th>Uploaded</th>
			                                <th>Delete</th>
			                            </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr class='<%#iif(Container.ItemIndex mod 2 = 1,"alternate","row") %>'>
                                        
                                        <td class="dcrow" id="tdMessageRow" runat="server" >
                                            <a id="lnkMessageTitle" runat="server" class="dctitlelink" target="_blank" href="/" ><%#DataBinder.Eval(Container.DataItem, "Title")%></a>&#160;
                                        </td>
                                        <td><%#FormatDateTime(DataBinder.Eval(Container.DataItem, "Uploaded"), DateFormat.ShortDate)%></td>
                                        <td>
                                            <CC:ConfirmImageButton CommandName="Remove" Message="Are you sure that you want to remove this document?" runat="server" ImageUrl="/images/admin/delete.gif" ID="lnkDelete" />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                </table>
                                </FooterTemplate>
                                </asp:Repeater>	
                                <div class="dcrow" id="divNoCurrentDocuments" runat="server"><p class="dcmessagetext">You have no documents available.</p></div> 
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


<p></p>
<CC:OneClickButton id="btnSaveAndUpload" runat="server" Text="Save And Upload Document" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Take Off Service?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
