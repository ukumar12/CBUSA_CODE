<%@ Control EnableViewstate="False" Language="VB" AutoEventWireup="false" CodeFile="Documents.ascx.vb" Inherits="_default" %>
     <%--<asp:button id ="btnDashBoard" text="Go to DashBoard" class="btnred" runat="server" />--%>
<div class="pckgltgraywrpr" style="margin-top:10px;">
	<div class="pckghdgblue">
		<div style="float:left;width:200px">My Documents</div>
        <div style="float:right;">
            <%--<CC:RadioButtonEx ID="rdoAllDocuments" GroupName="DocumentOwner" runat="server" AutoPostBack="True" Text="All" EnableViewState="True"  /> <span style="padding-top:-5px" > All </span>--%>
		    <CC:RadioButtonEx ID="rdoCBUSADocuments" GroupName="DocumentOwner" runat="server" AutoPostBack="True" Text="CBUSA" EnableViewState="True"/> <span style="padding-top:-5px" > CBUSA </span>
		    <%--<CC:RadioButtonEx ID="rdoOurDocuments" GroupName="DocumentOwner" runat="server" AutoPostBack="True" Text="Ours" EnableViewState="True" />  <span style="padding-top:-5px" > Ours </span>--%>
	    </div>
	</div>
    <div>
    <asp:Repeater id="rptDocuments" runat="server">
    <HeaderTemplate>
        <table cellspacing="0" cellpadding="0" border="0" class="dochdrtbl">
			<tr>
			    <th>Document Title</th>
			    <th>Document Type</th>
			    <th>&nbsp;</th>
			</tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr class='<%#IIf(Container.ItemIndex Mod 2 = 1, "alternate", "row") %>'>
            
            <td class="dcrow" id="tdMessageRow" runat="server" >
                <a id="lnkMessageTitle" runat="server" class="dctitlelink" target="_blank" href="/" ><%#DataBinder.Eval(Container.DataItem, "Title")%></a>&#160;
                <p class="dcposteddate">Uploaded:&#160;&#160;<%#FormatDateTime(DataBinder.Eval(Container.DataItem, "Uploaded"), DateFormat.ShortDate)%></p>
            </td>
            <td></td>
            <td>
                <CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this document?" runat="server" NavigateUrl="" ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
    </table>
    </FooterTemplate>
    </asp:Repeater>	
    <div class="dcrow" id="divNoCurrentDocuments" runat="server"><p class="dcmessagetext">You have no documents available.</p></div> 
	<div class="btnhldrrt" style="margin:1px;display:none;">
        <a id="lnkAddDocument" runat="server" class="btnblue" href="javascript:void(0);" >Add Document</a>&#160;&#160;<a id="lnkViewAll" runat="server" class="btnblue" href="/" >View All</a>
    </div>
</div>
<div id="divAddDocument" runat="server" class="window" style="width:400px;"><%--border:1px solid #000;background-color:#fff;--%>
   <div class="pckggraywrpr automargin" >
    <div class="pckghdgred" >Add Document</div>
    <div class="dcmain">
        <table>
	        <tr>
		        <td class="required">Title:</td>
		        <td class="field"><asp:textbox id="txtTitle" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
	        </tr>
	        <tr>
		        <td class="required">Document:</td>
		        <td class="field"><CC:FileUpload ID="fulDocument" runat="server" /></td>
		        <td></td>
	        </tr>
        </table>
        <br />
        <p align="center">
            <asp:Button id="btnAdd" runat="server" cssclass="btnred" text="Add" CausesValidation="false" />
            <asp:Button id="btnCancel" runat="server" cssclass="btnred" text="Close" CausesValidation="false" />        
        </p>
    </div>
       </div>
</div>
<CC:DivWindow ID="ctrlAddDocument" runat="server" TargetControlID="divAddDocument" TriggerId="lnkAddDocument" CloseTriggerId="btnCancel" ShowVeil="true" VeilCloses="true" />
<div id="divDesigner" runat="server" EnableViewState="False">
    <div class="contentbottom">
        <span class="smaller">Return Count:</span>
        <asp:DropDownList Id="drpReturnCount" CausesValidation="False" runat="server" AutoPostBack="True" />
        <span class="smaller">Display View All Link:</span>
        <asp:DropDownList Id="drpDisplayViewAllLink" CausesValidation="False" runat="server" AutoPostBack="True" />
        <input type="hidden" id="hdnField" runat="server" name="hdnField" />
    </div>
</div>
</div>
<br />
