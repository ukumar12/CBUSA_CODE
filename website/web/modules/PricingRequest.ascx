<%@ Control EnableViewstate="True" Language="VB" AutoEventWireup="false" CodeFile="PricingRequest.ascx.vb" Inherits="_default" %>
    
<div class="pckgwrpr">
	<div class="pckghdgltblue">
		Pricing Requests
	</div>

        <asp:Repeater id="rptPricingRequests" runat="server" EnableViewState="true">
        <HeaderTemplate>
	        <table cellspacing="0" cellpadding="0" border="0" class="dshbrdtbl tblprcng">
	            <tr>
	                <th>Builder</th>
	                <th>Date</th>
	                <th>Product</th>
	                <th>&nbsp;</th>
	            </tr>
        </HeaderTemplate>
        <ItemTemplate>
	            <tr>
	                <td><%--<a href="foo.asp">--%><%#DataBinder.Eval(Container.DataItem, "Builder")%><%--</a>--%></td>
	                <td><%#DataBinder.Eval(Container.DataItem, "Submitted")%></td>
	                <td><%#DataBinder.Eval(Container.DataItem, "Product")%></td>
	                <td><asp:LinkButton ID="btnUpdatePricing" runat="server" >Update</asp:LinkButton></td>
	            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
        </asp:Repeater>	
	
	<div class="dcrow" id="divNoCurrentPriceRequests" runat="server"><p class="dcmessagetext">You have no pricing requests.</p></div> 
	<div class="btnhldrrt">
        <a id="lnkViewAll" runat="server" class="btnblue" href="/" >View All</a>
    </div>

</div>

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

<div id="divProductPriceUpdate" runat="server" class="window" style="border:1px solid #000;background-color:#fff;width:210px;">
    <div class="mipopuptitle" >Product Price Update</div>
    <div class="mimainpopup">
        <table>
            <tr>
                <td colspan="2">
                     <b>Product: Title</b>
                </td>
            </tr>
	        <tr>
		        <td class="required">Product:</td>
		        <td>Price</td>
	        </tr>
        </table>
        <br />
        <asp:Button id="btnSave" runat="server" cssclass="btnred" text="Save" CausesValidation="false" />
        <asp:Button id="btnCancel" runat="server" cssclass="btnred" text="Close" />
    </div>
</div>
<CC:DivWindow ID="ctrlProductPriceUpdate" runat="server" TargetControlID="divProductPriceUpdate" TriggerId="lnkSetting" CloseTriggerId="btnCancel" ShowVeil="true" VeilCloses="true" />
