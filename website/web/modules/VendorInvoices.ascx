<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VendorInvoices.ascx.vb" Inherits="_default" %>

<asp:PlaceHolder runat="server">
    <script type="text/javascript">
        function OpenInvoicesConfirm() {
            //Sys.WebForms.PageRequestManager.getInstance().remove_pageLoaded(OpenInvoicesConfirm);
            Sys.Application.remove_loaded(OpenInvoicesConfirm);
            frm = $get('<%=frmConfirm.ClientID %>').control;
            //frm._doMoveToCenter();
            frm.Open();
        }

        function ShowDetails(frmid) {
            alert(frmid);
            var frm = document.getElementById(frmid);

            frm.style.position = 'absolute';
            frm.style.left = '200px';
            frm.style.top = '200px';
            frm.style.display = 'block';

            return false;
        }

        function HideDetails(frmid) {
            alert(frmid);
            var frm = document.getElementById(frmid);
            frm.style.display = 'none';
            return false;
        }
    </script>
</asp:PlaceHolder>

<CC:PopupForm ID="frmConfirm" runat="server" Width="300px" CssClass="pform" ShowVeil="true" VeilCloses="true" CloseTriggerId="btnCloseConfirm">
    <FormTemplate>
        <div class="pckggraywrpr" style="margin:0px;">
            <div class="pckghdgred">Order Status Updated</div>
            <p class="bold center" style="padding:10px;">
                Order Status and Comments have been updated, and Builder has been notified.<br /><br />
                <asp:Button ID="btnCloseConfirm" runat="server" Text="Close" CssClass="btnred" />
            </p>
        </div>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlID="btnCloseConfirm" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>

<asp:UpdatePanel ID="upOrders" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
<ContentTemplate>
<div class="pckggraywrpr"> 
	<div class="pckghdgred">
		Orders
	</div>
    <div style="float:right;padding-right:3px;padding-bottom:5px;padding-top:5px;">
        Page Size: 
        <asp:DropDownList id="ddlPageSize" runat="server" AutoPostBack="True">
            <asp:ListItem value="10" Text="10" Selected="True"></asp:ListItem>
            <asp:ListItem value="25" Text="25"></asp:ListItem>
            <asp:ListItem value="50" Text="50"></asp:ListItem>
            <asp:ListItem value="100" Text="100"></asp:ListItem>
        </asp:DropDownList>
    </div>
	<CC:GridView id="gvNewOrders" width="100%" CellSpacing="2" CellPadding="2" runat="server" PageSize="10" AllowPaging="True" AllowSorting="True"  EmptyDataText="You currently have no electronic orders to view." AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" class="tblcomprlen" style="margin:0px">
	    <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	    <RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	    <Columns>
		    <asp:templatefield headertext="Order #">
                <itemtemplate>
                <a href='<%# "/order/summary.aspx?OrderID=" & DataBinder.Eval(Container.DataItem, "OrderID") & IIf(DataBinder.Eval(Container.DataItem, "TwoPriceCampaignId") IsNot DBNull.Value, "&twoprice=y", "")%>'><%#DataBinder.Eval(Container.DataItem, "OrderNumber")%></a>
                </itemtemplate>
            </asp:templatefield>
            <asp:BoundField SortExpression="Title" DataField="Title" HeaderText="Title" />            
		    <asp:BoundField SortExpression="Builder" DataField="Builder" HeaderText="Builder"></asp:BoundField>
		    <asp:BoundField SortExpression="Created" DataField="Created" HeaderText="Order Date"></asp:BoundField>
		    <asp:BoundField SortExpression="Total" DataField="Total" HeaderText="Total" HtmlEncode="false" DataFormatString="{0:c}" ></asp:BoundField>
		    <asp:TemplateField HeaderText="Status" ItemStyle-VerticalAlign="Middle">
		        <ItemTemplate>
		            <asp:ImageButton ID="btnOpenStatus" runat="server" ImageUrl="/images/admin/edit.gif" Text="Update" />
		            <asp:Literal ID="ltlStatus" runat="server"></asp:Literal>
		            <CC:PopupForm ID="frmStatus" runat="server" CssClass="pform" OpenTriggerId="btnOpenStatus" CloseTriggerId="btnCancelStatus" OpenMode="MoveToCenter" Width="300px" ShowVeil="true" VeilCloses="true">
		                <FormTemplate>
		                    <div class="pckggraywrpr" style="margin:0px;">
		                        <div class="pckghdgred">Order Status Update</div>
		                        <table cellpadding="2" cellspacing="0" border="0" >
		                            <tr>
		                                <td colspan="2" class="largest"><b><asp:Literal ID="ltlOrderTitle" runat="server"></asp:Literal> Status</b></td>
		                            </tr>
		                            <tr>
		                                <td><b>Order Status:</b></td>
		                                <td><asp:DropDownList ID="drpStatus" runat="server"></asp:DropDownList></td>
		                            </tr>
		                            <tr>
		                                <td><b>Notes/Comments:</b></td>
		                                <td><asp:TextBox ID="txtComments" runat="server" Rows="4" TextMode="MultiLine"></asp:TextBox></td>
		                            </tr>
		                            <tr>
		                                <td colspan="2" style="text-align:center;">
		                                    <asp:Button ID="btnSaveStatus" runat="server" CssClass="btnred" Text="Update" ValidationGroup="VendorInvoices" CausesValidation="false" onclick="frmStatus_Postback"/>
		                                    <asp:Button ID="btnCancelStatus" runat="server" CssClass="btnred" Text="Cancel" ValidationGroup="VendorInvoices" CausesValidation="false" />
		                                </td>
		                            </tr>
		                        </table>
                                <asp:HiddenField ID="hdnIsTwoPrice" Value="false" runat="server" />
		                    </div>
		                </FormTemplate>
		                <Buttons>
                            <CC:PopupFormButton ControlID="btnSaveStatus" ButtonType="Postback" />
                            <CC:PopupFormButton ControlID="btnCancelStatus" ButtonType="ScriptOnly" />		                
		                </Buttons>
		            </CC:PopupForm>  
		        </ItemTemplate>
		    </asp:TemplateField>
	    </Columns>
    </CC:GridView>
        <div class="right">
		    <a id="lnkViewAll" runat="server" class="btnred" href="/vendor/invoice/default.aspx" >View All</a>
	    </div>
</div>
</ContentTemplate>
</asp:UpdatePanel>

<div id="divDesigner" runat="server" EnableViewState="False">
    <div class="contentbottom">
        <span class="smaller">Return Count:</span>
        <asp:DropDownList Id="drpReturnCount" CausesValidation="False" runat="server" AutoPostBack="True" />
        <span class="smaller">Display View All Link:</span>
        <asp:DropDownList Id="drpDisplayViewAllLink" CausesValidation="False" runat="server" AutoPostBack="True" />
        <input type="hidden" id="hdnField" runat="server" name="hdnField" />
    </div>
</div>
<br />
