<%@ Control EnableViewstate="True" Language="VB" AutoEventWireup="false" CodeFile="MyMoneyVendor.ascx.vb" Inherits="MyMoneyVendor" %>

<div class="pckgwrpr bggray">
	<div class="pckghdgltblue">
		My Money
	</div>
	<div class="pckgbdy">

		<div style="z-index:100;">
		<table cellspacing="0" cellpadding="0" border="0" style="width:180px;">
		    <col width="120px" />
		    <col width="" />
		    <col width="" />
		    <tr>
		        <td class="bold" style="padding:4px 2px;">Start Qtr/Yr:</td>
		        <td>
                    <CC:DropDownListEx ID="drpStartQuarter" runat="server" AutoPostBack="true" >
                    </CC:DropDownListEx>
                </td>
		        <td>
                    <CC:DropDownListEx ID="drpStartYear" runat="server" AutoPostBack="true"  >
                    </CC:DropDownListEx>
                </td>
		    </tr>
		    <tr>
		        <td class="bold" style="padding:4px 2px;">End Qtr/Yr:</td>
		        <td>
                    <CC:DropDownListEx ID="drpEndQuarter" runat="server" AutoPostBack="true"  >
                    </CC:DropDownListEx>
                </td>
		        <td>
                    <CC:DropDownListEx ID="drpEndYear" runat="server" AutoPostBack="true" >
                    </CC:DropDownListEx>
		        </td>
		    </tr>
		</table>
        </div>
		<br />
		<div id="divChart" runat="server" style="margin-left:-5px; z-index:1;">
		    <%--<b>TEXT</b><br />--%>
            <object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=8,0,0,0" width="190" height="190" name="PieChart">
                <param name="allowScriptAccess" value="always" />
                <param name="movie" value="/FusionCharts/FCF_Pie3D.swf" />
                <param name="FlashVars" value="&chartWidth=190&chartHeight=190&debugMode=0&dataXML=<%=GetChartFlashString() %>" />
                <param name="quality" value="high" />
                <param name="wmode" value="transparent" />
            <embed src="/FusionCharts/FCF_Pie3D.swf" flashVars="&chartWidth=190&chartHeight=190&debugMode=0&dataXML=<%=GetChartFlashString() %>" wmode="transparent" quality="high" width="190" height="190" name="PieChart" type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" />
            </object>
            
        </div>

	</div>
	<div class="btnhldrrt"><asp:Button ID="btnViewMore" cssclass="btnblue" runat="server" Text="View More" CausesValidation="false" Visible="false" /> </div>
</div>
