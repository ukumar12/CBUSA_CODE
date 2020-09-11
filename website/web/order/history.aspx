<%@ Page Language="VB" AutoEventWireup="false" CodeFile="history.aspx.vb" Inherits="order_history" %>

<CT:MasterPage ID="CTMain" runat="server">
    <asp:PlaceHolder runat="server">
        <script type="text/javascript">
            function ConfirmDelete(id, name, bool) {
                var frm = $get('<%=frmDelete.ClientID %>').control;
                var hdn = frm.get_input('hdnOrderId');
                var hdnTwo = frm.get_input('hdnIsTwoPrice');
                var span = $get('spanOrderName');
                hdn.value = id;
                hdnTwo.value = bool;
                span.innerHTML = name;
                frm._doMoveToCenter();
                frm.Open();
            }

            function ShowDetails(frmid) {
                var frm = document.getElementById(frmid);

                frm.style.position = 'absolute';
                frm.style.left = '200px';
                frm.style.top = '200px';
                frm.style.display = 'block';

                return false;
            }

            function HideDetails(frmid) {
                var frm = document.getElementById(frmid);
                frm.style.display = 'none';
                return false;
            }
        </script>
    </asp:PlaceHolder>

    <CC:PopupForm ID="frmDelete" runat="server" CloseTriggerId="btnCancelDelete" ShowVeil="true" VeilCloses="true" CssClass="pform" Width="250px">
        <FormTemplate>
            <div class="pckggraywrpr automargin">
                <div class="pckghdgred">Confirm Order Delete</div>
                <div class="bold center" style="padding:10px;">
                    Are you sure you want to delete order <span id="spanOrderName"></span>?<br /><br />
                    <asp:Button id="btnOKDelete" runat="server" cssclass="btnred" Text="OK" />
                    <asp:Button id="btnCancelDelete" runat="server" cssclass="btnred" Text="Cancel" />
                    <asp:HiddenField id="hdnOrderId" runat="server"></asp:HiddenField>
                    <asp:HiddenField id="hdnIsTwoPrice" runat="server"></asp:HiddenField>
                </div>
            </div>
        </FormTemplate>
        <Buttons>
            <CC:PopupFormButton ControlID="btnOKDelete" ButtonType="Postback" />
            <CC:PopupFormButton ControlID="btnCancelDelete" ButtonType="ScriptOnly" />
        </Buttons>
    </CC:PopupForm>
    
    <div class="pckggraywrpr">
        <div class="pckghdgred">Order History</div>

        <table cellspacing="2" cellpadding="2" border="0" class="tblcompr" style="width:50%;margin:10px auto;">
            <tr>
                <th colspan="2">Filter Orders</th>
            </tr>
            <tr>
                <td>
                    <b>Project:</b>
                </td>
                <td>
                    <asp:DropDownList id="F_ProjectID" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr id="trCampaign" runat="server" visible="false">
                <td>
                    <b>Two Price Campaign:</b>
                </td>
                <td>
                    <asp:DropDownList id="F_TwoPriceCampaignID" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Order Date</b><br />
                    <span class="smaller">Between:</span>
                </td>
                <td>
                    <table cellpadding="2" cellspacing="0" border="0">
                        <tr>
                            <td class="smaller bold">Start Date</td>
                            <td class="smaller bold">End Date</td>
                        </tr>
                        <tr>
                            <td class="field"><CC:DatePicker ID="F_StartDate" runat="server"></CC:DatePicker></td>
                            <td class="field"><CC:DatePicker id="F_EndDate" runat="server"></CC:DatePicker></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Button id="btnFilter" runat="server" cssclass="btnred" text="Search" />
                    <input type="button" class="btnred" value="Clear" onclick="location.reload();" />
                </td>
            </tr>
        </table>
        <div style="float:right;padding-right:20px;">
            Page Size: 
            <asp:DropDownList id="ddlPageSize" runat="server" AutoPostBack="True">
                <asp:ListItem value="10" Text="10" Selected="True"></asp:ListItem>
                <asp:ListItem value="25" Text="25"></asp:ListItem>
                <asp:ListItem value="50" Text="50"></asp:ListItem>
                <asp:ListItem value="100" Text="100"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <CC:GridView id="gvList" BorderStyle="None" CellSpacing="0" CellPadding="5" CssClass="tblcomprlen" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
            <HeaderStyle CssClass="sbttl" />
            <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
            <RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
            <Columns>
	            <asp:TemplateField>
		            <ItemTemplate>
                        <asp:ImageButton id="lnkShowDetails" runat="server" ImageUrl="/images/admin/preview.gif" Text="..."></asp:ImageButton>
		                <CC:PopupForm id="frmDetails" runat="server" cssclass="pform" CloseTriggerID="spanClose" OpenMode="MoveToCenter" ShowVeil="true" VeilCloses="true">
                            <FormTemplate>
		                        <div class="pckggraywrpr" style="margin-bottom:0px;width:700px;">
		                        <div class="pckghdgred"><span id="spanClose" runat="server" style="cursor:pointer;float:right;">Close</span>Order Summary</div>
		                        <div style="padding:10px;">
		                            <table cellpadding="3" cellspacing="3" border="0">
		                                <tr>
		                                    <td><b>Order Date:</b></td>
		                                    <td><asp:Literal id="ltlOrderDate" runat="server"></asp:Literal></td>
                                        </tr>
                                        <tr>
		                                    <td><b>Vendor:</b></td>
		                                    <td><asp:Literal id="ltlVendor" runat="server"></asp:Literal></td>
		                                </tr>
		                                <tr>
		                                    <td><b>Project:</b></td>
		                                    <td><asp:Literal id="ltlProject" runat="server"></asp:Literal></td>
		                                </tr>
		                                <tr>
		                                    <td><b>Order Title:</b></td>
		                                    <td><asp:Literal id="ltlOrderTitle" runat="server"></asp:Literal></td>
		                                </tr>
		                                <tr>
		                                    <td><b>Order Placer Name:</b></td>
		                                    <td><asp:Literal id="ltlOrderPlacerName" runat="server"></asp:Literal></td>
		                                </tr>
		                                <tr>
		                                    <td><b>Order Placer Email:</b></td>
		                                    <td><asp:Literal id="ltlOrderPlacerEmail" runat="server"></asp:Literal></td>
		                                </tr>
		                                <tr>
		                                    <td><b>Order Placer Phone:</b></td>
		                                    <td><asp:Literal id="ltlOrderPlacerPhone" runat="server"></asp:Literal></td>
		                                </tr>
		                                <tr>
		                                    <td><b>Superintendent Name:</b></td>
		                                    <td><asp:Literal id="ltlSuperName" runat="server"></asp:Literal></td>
		                                </tr>
		                                <tr>
		                                    <td><b>Superintendent Email:</b></td>
		                                    <td><asp:Literal id="ltlSuperEmail" runat="server"></asp:Literal></td>
		                                </tr>
		                                <tr>
		                                    <td><b>Superintendent Phone:</b></td>
		                                    <td><asp:Literal id="ltlSuperPhone" runat="server"></asp:Literal></td>
		                                </tr>
		                                <tr>
		                                    <td><b>Subtotal:</b></td>
		                                    <td><asp:Literal id="ltlSubtotal" runat="server"></asp:Literal></td>
		                                </tr>
		                                <tr>
		                                    <td><b>Tax <asp:Literal id="ltlTaxRate" runat="server"></asp:Literal>:</b></td>
		                                    <td><asp:Literal id="ltlTax" runat="server"></asp:Literal></td>
		                                </tr>
		                                <tr>
		                                    <td><b>Total:</b></td>
		                                    <td><asp:Literal id="ltlTotal" runat="server"></asp:Literal></td>
		                                </tr>
		                                <tr>
		                                    <td colspan="2" style="width:100%;overflow:auto;">
		                                        <b>Order Items:</b><br />
		                                        <asp:GridView id="gvItems" runat="server" AutoGenerateColumns="false" AllowPaging="false" AllowSorting="false" class="pckgwhtwrpr">
		                                            <Columns>
		                                                <asp:BoundField DataField="ProductSku" HeaderText="Item SKU" />
		                                                <asp:BoundField DataField="ProductName" HeaderText="Item Name" />
		                                                <%--<asp:BoundField DataField="UnitOfMeasure" HeaderText="Unit Of Measure" />--%>
		                                                <asp:BoundField DataField="VendorPrice" HeaderText="Unit Price" HtmlEncode="false" DataFormatString="{0:c}" />
		                                                <asp:BoundField DataField="Quantity" HeaderText="Quantity" />
		                                                <%--<asp:BoundField DataField="ExtendedPrice" HeaderText="Extended Price" HtmlEncode="false" DataFormatString="{0:c}" />--%>
		                                            </Columns>
		                                        </asp:GridView>
		                                    </td>
		                                </tr>
		                            </table>
		                        </div>
		                        </div>
		                    </FormTemplate>
		                </CC:PopupForm>
		            </ItemTemplate>
	            </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:HyperLink id="lnkDetails" enableviewstate="false" runat="server" ImageUrl="/images/admin/edit.gif" Text="Order Details" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:HyperLink id="lnkDelete" enableviewstate="false" runat="server" ImageUrl="/images/admin/delete.gif" style="cursor:pointer;" onclick='<%# "ConfirmDelete(" & DataBinder.Eval(Container.DataItem, "OrderID") & ",""" & DataBinder.Eval(Container.DataItem, "Title") & """,""" & (DataBinder.Eval(Container.DataItem, "TwoPriceCampaignId") IsNot DBNull.Value).ToString & """);"%>' Text="Delete Order" />
                    </ItemTemplate>
                </asp:TemplateField>
              <%--  <asp:TemplateField>
                    <ItemTemplate>
                        <asp:HyperLink enableviewstate="false" runat="server" NavigateUrl='<%# "drops.aspx?OrderID="& DataBinder.Eval(Container.DataItem,"OrderId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/collection.gif" Text="Drops"></asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" SortExpression="OrderNumber" />
                <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" />
                <asp:BoundField DataField="Project" HeaderText="Project" SortExpression="Project" />
                <asp:BoundField DataField="VendorCompany" HeaderText="Vendor" SortExpression="VendorCompany" />
                <asp:BoundField DataField="OrdererLastName" HeaderText="Last Name" SortExpression="OrdererLastName" />
                <asp:BoundField DataField="Created" HeaderText="Order Date" SortExpression="Created" HtmlEncode="false" DataFormatString="{0:d}" />
                <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" HtmlEncode="false" DataFormatstring="{0:c}" />
                <asp:BoundField DataField="OrderStatus" HeaderText="Status" SortExpression="OrderStatus" />
            </Columns>
        </CC:GridView>        
    </div>
</CT:MasterPage>