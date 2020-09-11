<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Committed Purchase Event"%>
<%@ Register Src="~/controls/Documents/MultiFileUploadNew.ascx" TagName="MultiUploadDocument" TagPrefix="CC" %>
 

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">	
    <link rel="stylesheet" href="/includes/style.css" type="text/css" /> 
<h4><% If TwoPriceCampaignId = 0 Then%>Add<% Else %>Edit<% End If %> Committed Purchase Event</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Name:</td>
		<td class="field"><asp:textbox id="txtName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName" ErrorMessage="Field 'Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Status:</td>
		<td class="field"><asp:Dropdownlist id="drpStatus" runat="server"></asp:Dropdownlist></td>
		<td><asp:RequiredFieldValidator ID="rfvStatus" runat="server" Display="Dynamic" ControlToValidate="drpStatus" ErrorMessage="Field 'Status' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Start Date:</td>
		<td class="field"><CC:DatePicker ID="dtStartDate" runat="server"></CC:DatePicker></td>
		<td><CC:RequiredDateValidator Display="Dynamic" runat="server" id="rdtvStartDate" ControlToValidate="dtStartDate" ErrorMessage="Date Field 'Start Date' is blank" /><CC:DateValidator Display="Dynamic" runat="server" id="dtvStartDate" ControlToValidate="dtStartDate" ErrorMessage="Date Field 'Start Date' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">End Date:</td>
		<td class="field"><CC:DatePicker ID="dtEndDate" runat="server"></CC:DatePicker></td>
		<td><CC:RequiredDateValidator Display="Dynamic" runat="server" id="rdtvEndDate" ControlToValidate="dtEndDate" ErrorMessage="Date Field 'End Date' is blank" /><CC:DateValidator Display="Dynamic" runat="server" id="dtvEndDate" ControlToValidate="dtEndDate" ErrorMessage="Date Field 'End Date' is invalid" /></td>
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
    <tr>
        <td class="required"><b>Allow Price Update?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblPriceUpdate" RepeatDirection="Horizontal" Enabled="False">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
    </tr>
    <tr>
		<td class="required">LLC:</td>
		<td class="field"><CC:CheckBoxListEx ID="cblLLC" runat="server" RepeatColumns="3"></CC:CheckBoxListEx></td>
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
<CC:OneClickButton id="btnSave" runat="server" Text="Save And Close" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Committed Purchase Event?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>



<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.6/jquery.min.js" type="text/javascript"></script>
<script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js" type="text/javascript"></script>
<link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css" rel="Stylesheet" type="text/css" />

<script type="text/javascript">
  $('ph_dtStartDate_txtDatePicker').datepicker({ minDate: 0,maxDate: 0});
</script>
</asp:content>

