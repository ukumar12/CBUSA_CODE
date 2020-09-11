<%@ Page Language="VB" AutoEventWireup="false" CodeFile="sku.aspx.vb" Inherits="sku" EnableEventValidation="false" %>

<CT:MasterPage id="CTMain" runat="server">

<span id="trace"></span>

<nStuff:UpdateHistory ID="ctlHistory" runat="server"></nStuff:UpdateHistory>

<asp:UpdatePanel ID="upFilterBar" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="0" cellspacing="0" border="0" class="fltrbar">
            <tr>
                <td align="right">
                    <asp:TextBox ID="txtKeyword" runat="server" style="width:154px;color:#666;" onfocus="this.value='',this.style.color='#000'" Text="Keyword Search"></asp:TextBox>
                </td>
                <td align="left">
                    <asp:ImageButton ID="btnSearch" runat="server" ImageUrl="/images/global/btn-fltrbar-search.gif" AlternateText="Search" style="width:28px;border:none;height:26px;" />
                </td>    
            </tr>
        </table>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSearch" />
        <asp:AsyncPostBackTrigger ControlID="hdnSearch" />
    </Triggers>
</asp:UpdatePanel>
<asp:HiddenField id="hdnSearch" runat="server" value=" "></asp:HiddenField>
<table class="tblnwto" cellpadding="0" cellspacing="0" border="0">
    <tr valign="top">
        <td class="leftcol">
            <asp:UpdatePanel id="upFacets" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="True">
            <ContentTemplate>
                <div class="leftcolwrpr">
                    <div class="pckghdgred">My Supply Phases</div>
                    <CC:FrontEndTreeView ID="tvSupplyPhases" runat="server" />                       
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        
        <p></p>

        </td>       
        <td>
            <div class="maincolwrpr">
                <div class="pckghdgred" style="height:20px;">
                    SKU-Matching
                </div>
                <div style="float:right; margin-top:-32px; margin-right:5px;">
                    <asp:DropDownList id="drpMatchingType" runat="server" autopostback="true">
                        <asp:listitem value="sku-price.aspx">Enter SKUs and prices</asp:listitem>
                        <asp:listitem value="sku.aspx" selected>Enter prices later</asp:listitem>
                        <asp:listitem value="pricing.aspx">Enter SKUs later</asp:listitem>
                    </asp:DropDownList>
                    <CC:FileUpload ID="fulDocument" runat="server" />
                    <CC:OneClickButton ID="btnImport" Text="Import CSV" cssclass="btnred" runat="server" />                            
                    <asp:Button ID="btnExport" Text="Export CSV" cssclass="btnred" runat="server" OnClick="ExportCSV" />
                </div>
                <asp:UpdatePanel id="upProducts" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <div class="bcrmwrpr">
                            <asp:Literal id="ltlBreadcrumb" runat="server" enableviewstate="false"></asp:Literal>
                            <div style="float:right; margin-top:-31px;">
                                
                            </div>
                        </div>
                        
                        <asp:PlaceHolder ID="txtNoResults" runat="server" Visible="False">
                        <div style="margin-top:10px; margin-bottom:10px; margin-left:5px;">
                        <b>No products found matching your search criteria. Please select a new phase of supply or enter a new keyword.</b>
                        </div>
                        </asp:PlaceHolder >
                        <div style="margin-top:10px; margin-bottom:10px; margin-left:5px;">
                            <asp:Literal id="ltrErrorMsg" runat="server"></asp:Literal>
                            <div style="float:right;"><CC:OneClickButton ID="btnImportUpdate" runat="server" Text="Continue Import" CssClass="btnred" visible="false"/></div>
                        </div>
                            
                        <asp:Repeater id="rptProducts" runat="server">
                            <headertemplate>
                            <div style="margin-left:5px;">
                            <table>
                                <tr>
                                    <td><img src="/images/admin/true.gif" border="0"/></td>
                                    <td><span class="small">Updated successfully in the last 2 hours</span></td>
                                </tr>
                            </table>
                            </div>
                            <table class="tbltodata" cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <th></th>
                                <th align="center" colspan="2">CBUSA</th>
                                <th align="center" colspan="2">Vendor SKU</th>
                                <th colspan="4"></th>
                            </tr>
                            <tr>
                                <th></th>
                                <th>SKU</th>
                                <th>Product</th>
                                <th>Current</th>
                                <th>New</th>
                                <th>Substitution?</th>
                                <th>Discontinued?</th>
                                <th>Updated</th>
                                <th></th>
                            </tr>
                                </headertemplate>
                                <ItemTemplate>
                                    <tr class='<%#iif(Container.ItemIndex mod 2 = 1,"","alt") %>' id="trVendor" runat="server">
                                        <td><asp:Image id="imgReq" runat="server" visible="false"></asp:Image></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Sku")%></td>                                
                                        <td><%#DataBinder.Eval(Container.DataItem, "Product")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "VendorSku")%></td>
                                        <td><asp:TextBox id="txtSku" runat="server" maxlength="20" columns="6" class="inptqty"></asp:TextBox></td>
                                        <td align="center"><asp:CheckBox id="ckbSubstitute" runat="server"></asp:CheckBox></td>
                                        <td align="center"><asp:CheckBox id="ckbDiscontinued" runat="server"></asp:CheckBox></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Updated")%></td>    
                                        
                                        <td align="right">
                                            <asp:Button id="btnUpdate" runat="server" Text="Add" cssclass="btnadd" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"ProductID") %>'></asp:Button>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <footertemplate>
                                </table>
                                </footertemplate>
                            </asp:Repeater>
                        
                    </ContentTemplate>
                    <Triggers>
                        <%--<asp:AsyncPostbackTrigger ControlId="tvSupplyPhases" EventName="SelectedIndexChanged"></asp:AsyncPostbackTrigger>                    --%>
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </td>     
    </tr>
</table>



</CT:MasterPage>