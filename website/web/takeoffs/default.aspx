<%@ Page Language="VB" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="takeoffs_default" %>
<%@ Register TagName="VendorRating" TagPrefix="CC" Src="~/controls/VendorRating.ascx" %>

<CT:MasterPage ID="CTMain" runat="server">
    <asp:PlaceHolder runat="server">
<script type="text/javascript">
    function ToggleSaveForm() {
        var div = $get('divSaveForm');
        var lnk = $get('lnkSaveForm');
        if (div.style.display == 'none') {
            lnk.innerHTML = 'Hide Form';
        } else {
            lnk.innerHTML = 'Save Takeoff';
        }
        $(div).slideToggle('fast', null);
    }
    function OpenConfirmForm(newID) {
        $get('<%=hdnNewID.ClientID %>').value = newID;
        var frm = $get('<%=frmConfirm.ClientID %>').control;
        frm._doMoveToCenter();
        frm.Open();
        return false;
    }
</script>
        
        <CC:PopupForm ID="frmConfirm" runat="server" CssClass="pform" OpenMode="MoveToCenter" Animate="true" ShowVeil="true" VeilCloses="false" CloseTriggerId="btnCancel" style="width:300px;">
            <FormTemplate>
                <div class="pckggraywrpr" style="margin:0px;">
                    <div class="pckghdgred">Change Current Takeoff?</div>
                    <div class="bold center" style="padding:15px;">
                         Another takeoff is currently active.  Are you sure you want to continue?<br /><br />
                         <asp:Placeholder id="phSaveForm" runat="server">
                         <a id="lnkSaveForm" onclick="ToggleSaveForm()" style="cursor:pointer;">Save Takeoff</a><br />
                         <div id="divSaveForm" style="border:1px solid #000;background-color:#fff;display:none;">
                             <table cellpadding="2" cellspacing="2" border="0">
                                <tr><td colspan="3"><b>Enter a title to save your current takeoff.</b></td></tr>
                                <tr>
                                    <td><b>Title:</b></td>
                                    <td class="fieldreq">&nbsp;</td>
                                    <td><asp:TextBox id="txtTitle" runat="server" Maxlength="100" columns="25"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td><b>Project:</b></td>
                                    <td>&nbsp;</td>
                                    <td>
                                        <CC:SearchList ID="lstProjects" runat="server" Table="Project" TextField="ProjectName" ValueField="ProjectID" AllowNew="false" CssClass="searchlist" ViewAllLength="5" Width="100" />
                                        <a href="/projects/edit.aspx" class="smaller" target="_blank">Click Here to start a new Project</a>
                                    </td>
                                </tr>
                             </table>
                         </div>
                         </asp:Placeholder>
                         <asp:Button id="btnContinue" runat="server" text="Continue" cssclass="btnred" />
                         <asp:Button id="btnCancel" runat="server" text="Cancel" cssclass="btnred" />
                         <asp:HiddenField id="hdnNewID" runat="server"></asp:HiddenField>
                        </div>
                </div>
            </FormTemplate>
            <Buttons>
                <CC:PopupFormButton ControlId="btnContinue" ButtonType="Postback" />
                <CC:PopupFormButton ControlId="btnCancel" ButtonType="ScriptOnly" />
            </Buttons>
        </CC:PopupForm>
        
        <div class="pckggraywrpr">
            <div class="pckghdgred">Saved Takeoffs</div>
            <div>
                <div style="padding-bottom: 15px;">
                    <asp:Button id="btnNew" runat="server" cssclass="btngold" text="Create New Takeoff" />&nbsp;
                    <asp:Button id="btnCurrent" runat="server" cssclass="btngold" visible="false" />&nbsp;
                    <asp:Button id="btnViewAll" runat="server" cssclass="btngold" visible= "false" text="View All Takeoffs" />
                </div>
                <div style="float:right;padding-right:20px;">
                    Page Size: 
                    <asp:DropDownList id="ddlPageSize" runat="server" AutoPostBack="True">
                        <asp:ListItem value="10" Text="10"></asp:ListItem>
                        <asp:ListItem value="25" Text="25" Selected="True"></asp:ListItem>
                        <asp:ListItem value="50" Text="50"></asp:ListItem>
                        <asp:ListItem value="100" Text="100"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <CC:GridView id="gvList" CellSpacing="2" CellPadding="2" CssClass="tblcomprlen" runat="server" PageSize="25" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	            <HeaderStyle CssClass="sbttl" />
	            <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top" Font-Size="Larger"></AlternatingRowStyle>
	            <RowStyle CssClass="row" VerticalAlign="Top" Font-Size="Larger" Font-Bold="true"></RowStyle>
	            <Columns>
		            <asp:TemplateField>
		                <ItemStyle width="20px" />
		                <ItemTemplate>
		                    <asp:CheckBox id="cbInclude" runat="server"></asp:CheckBox>
		                </ItemTemplate>
		            </asp:TemplateField>
		            <asp:TemplateField>
			            <ItemStyle width="20px" />
			            <ItemTemplate>
			            <asp:ImageButton id="btnEdit" runat="server" CommandName="ChangeTakeoff" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "TakeoffID") %>' ImageUrl="/images/admin/edit.gif"></asp:ImageButton>
			            </ItemTemplate>
		            </asp:TemplateField>
		            <asp:TemplateField>
		                <ItemStyle width="20px" />
			            <ItemTemplate>
			            <CC:ConfirmImageButton Message="Are you sure that you want to remove this Takeoff?" runat="server" ImageUrl="/images/admin/delete.gif" ID="lnkDelete" CommandName="DeleteTakeoff" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "TakeoffID") %>' ></CC:ConfirmImageButton>
			            </ItemTemplate>
		            </asp:TemplateField>
                    <asp:BoundField DataField="Project" HeaderText="Project" SortExpression="ProjectName" />
                    <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" />
                    <asp:BoundField DataField="Saved" HeaderText="Save Date" SortExpression="Saved" />
                    <asp:TemplateField HeaderText="Saved Comparisons">
                        <ItemTemplate>
                            <asp:Literal id="ltlComparisons" runat="server"></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateField>
	            </Columns>
            </CC:GridView>
            <asp:Button id="btnAdd" runat="server" text="Add Products from Selected Takeoff(s) to Current Takeoff" cssclass="btnred" />
        </div>
    </asp:PlaceHolder>
</CT:MasterPage>