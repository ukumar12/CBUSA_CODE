<%@ Control EnableViewstate="False" Language="VB" AutoEventWireup="false" CodeFile="_MarketIndicators.ascx.vb" Inherits="_default" %>

<div class="pckgltgraywrpr">
	<div class="pckghdgltblue nobdr">
		My Market Indicators
	</div> 
<%--    <div class="stacktblwrpr thememarket">
		<div class="bdbtblhdg">
			<div class="caption">Biggest Price Changers</div>
		    <div class="clear">&nbsp;</div>
		</div>
        <CC:GridView id="gvAdminIndicators" width="100%" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="False" AllowSorting="False"  EmptyDataText="There are no CBUSA product watches setup." AutoGenerateColumns="True" BorderWidth="0" PagerSettings-Position="Bottom">
        </CC:GridView>
    </div>--%>
    <div class="stacktblwrpr thememarket">
		<div class="bdbtblhdg">
			<div class="caption">Vendor Product Prices</div>
			<div class="sttngs"><a id="lnkSetting" runat="server" class="miheaderlink" href="javascript:void(0);" >Settings</a></div>
		    <div class="clear">&nbsp;</div>
	    </div> 
        <CC:GridView id="gvBuilderIndicators" width="100%" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="False" AllowSorting="False"  EmptyDataText="You currently are not watching any products." AutoGenerateColumns="True" BorderWidth="0" PagerSettings-Position="Bottom">
        </CC:GridView>
    </div>
    <div class="stacktblwrpr thememarket">
		<div class="bdbtblhdg">
			<div class="caption">More Pricing Information</div>
		    <div class="clear">&nbsp;</div>
		</div>
        <div class="misubsectionmain" id="divMorePricingInfo" runat="server">
        </div>
    </div>
    <div class="clear">&nbsp;</div>
</div>
<br />
<div id="divSettingsEdit" runat="server" class="window" style="border:1px solid #000;background-color:#fff;width:210px;">
    <div class="pckghdgred" >Market Indicator Settings</div>
    <div class="mimainpopup">
        <table>
            <tr>
                <td colspan="2">
                     <b>Vendor Selection</b>
                </td>
            </tr>
	        <tr style="position:relative;z-index:10;">
		        <td class="required">Position 1:</td>
		        <td><CC:SearchList ID="acdVendorID1" runat="server" Table="Vendor" TextField="CompanyName" ValueField="VendorID" AllowNew="false" CssClass="searchlist" ViewAllLength="10"></CC:SearchList></td>
	        </tr>
	        <tr style="position:relative;z-index:9;">
		        <td class="required">Position 2:</td>
		        <td><CC:SearchList ID="acdVendorID2" runat="server" Table="Vendor" TextField="CompanyName" ValueField="VendorID" AllowNew="false" CssClass="searchlist" ViewAllLength="10"></CC:SearchList></td>
	        </tr>
	        <tr style="position:relative;z-index:8;">
		        <td class="required">Position 3:</td>
		        <td><CC:SearchList ID="acdVendorID3" runat="server" Table="Vendor" TextField="CompanyName" ValueField="VendorID" AllowNew="false" CssClass="searchlist" Viewalllength="10"></CC:SearchList></td>
	        </tr>
        </table>
        <br />
        <table>
            <tr style="position:relative;z-index:7;">
                <td colspan="2">
                     <b>Product Selection</b>
                </td>
            </tr>
	        <tr style="position:relative;z-index:6;">
		        <td class="required">Position 1:</td>
		        <td><CC:SearchList ID="acdProductID1" runat="server" Table="Product" TextField="Product" ValueField="ProductID" AllowNew="false" CssClass="searchlist" ViewAllLength="10"></CC:SearchList></td>
	        </tr>
	        <tr style="position:relative;z-index:5;">
		        <td class="required">Position 2:</td>
		        <td><CC:SearchList ID="acdProductID2" runat="server" Table="Product" TextField="Product" ValueField="ProductID" AllowNew="false" CssClass="searchlist" ViewAllLength="10"></CC:SearchList></td>
	        </tr>
	        <tr style="position:relative;z-index:4;">
		        <td class="required">Position 3:</td>
		        <td><CC:SearchList ID="acdProductID3" runat="server" Table="Product" TextField="Product" ValueField="ProductID" AllowNew="false" CssClass="searchlist" ViewAllLength="10"></CC:SearchList></td>
	        </tr>
	        <tr style="position:relative;z-index:3;">
		        <td class="required">Position 4:</td>
		        <td><CC:SearchList ID="acdProductID4" runat="server" Table="Product" TextField="Product" ValueField="ProductID" AllowNew="false" CssClass="searchlist" ViewAllLength="10"></CC:SearchList></td>
	        </tr>
	        <tr style="position:relative;z-index:2;">
		        <td class="required">Position 5:</td>
		        <td><CC:SearchList ID="acdProductID5" runat="server" Table="Product" TextField="Product" ValueField="ProductID" AllowNew="false" CssClass="searchlist" ViewAllLength="10"></CC:SearchList></td>
	        </tr>
        </table>
        <br />
        <asp:Button id="btnSave" runat="server" cssclass="btnred" text="Save"  CausesValidation="false" />
        <asp:Button id="btnCancel" runat="server" cssclass="btnred" text="Close" />
    </div>
</div>
<CC:DivWindow ID="ctrlSettingsEdit" runat="server" TargetControlID="divSettingsEdit" TriggerId="lnkSetting" CloseTriggerId="btnCancel" ShowVeil="true" VeilCloses="true" />