<%@ Control EnableViewstate="True" Language="VB" AutoEventWireup="false" CodeFile="MyMoneyOld.ascx.vb" Inherits="MyMoneyOld" %>

<asp:Panel ID="pnlMain" runat="server">
<asp:UpdatePanel ID="upMain" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="pckgwrpr bggray">
	        <div class="pckghdgltblue">
		        Market Share Report
	        </div>
	        <div class="pckgbdy">
        	    
        	    <div class="pckghdgred" style="text-align:center;">Opt Out</div>
		        <div style="z-index:100;">
		            <p><b>Press the Opt Out button below to disable your access to the Market Share Report and prevent other vendors from seeing your data in this report.</b></p>
                    <p><CC:ConfirmButton ID="btnOptOut" runat="server" CssClass="btnblue" Text="Opt Out" Message="Are you sure you want to opt out from accessing the Market Share Report. You will not be able to access this report and other vendor will not be able to see your data." /></p>
                    
		        </div>
        	    
                <div class="pckghdgred" style="text-align:center;">Select Dates</div>
		        <div style="z-index:100;">
		        <table cellspacing="2" cellpadding="1" border="0">
		            <col width="" />
		            <col width="" />
		            <col width="" />
		            <tr>
		                <td class="bold" style="padding:4px 2px;">Start Qtr/Yr:</td>
		                <td>
                            <CC:DropDownListEx ID="drpStartQuarter" runat="server" >
                            </CC:DropDownListEx>
                        </td>
		                <td>
                            <CC:DropDownListEx ID="drpStartYear" runat="server" >
                            </CC:DropDownListEx>
                        </td>
		            <%--</tr>
		            <tr>--%>
		                <td class="bold" style="padding:4px 2px;">End Qtr/Yr:</td>
		                <td>
                            <CC:DropDownListEx ID="drpEndQuarter" runat="server" >
                            </CC:DropDownListEx>
                        </td>
		                <td>
                            <CC:DropDownListEx ID="drpEndYear" runat="server">
                            </CC:DropDownListEx>
		                </td>
		            </tr>
		        </table>
                </div>
                <div class="pckgltgraywrpr center">
                    <div class="pckghdgred">Select Supply Phases</div>
                    <div style="text-align:center;">
                        <CC:ListSelect ID="lsSupplyPhases" runat="server" AutoPostback="true" style="text-align:left; margin-left:auto; margin-right:auto;width:600px;table-layout:auto;" Height="200" AddImageUrl="/images/admin/true.gif" DeleteImageUrl="/images/admin/delete.gif" />
                    </div>

                </div>
                <div class="pckgltgraywrpr center">
                    <div class="pckghdgred">Select Vendors From Supply Phase</div>
                    <div style="text-align:center;" >
                        <CC:ListSelect ID="lsVendors" runat="server" style="text-align:left; margin-left:auto; margin-right:auto;width:600px;table-layout:auto;" Height="200" AddImageUrl="/images/admin/true.gif" DeleteImageUrl="/images/admin/delete.gif" />
                    </div>

                </div>
                <asp:Button ID="btnGenerateChart"  cssclass="btnblue" runat="server" Text="Generate Chart" CausesValidation="false" />
		        <br />
		        <div id="divChart" runat="server" style="margin-left:5px; z-index:1; text-align:center;">
		           <br />
                    <asp:Literal id="ltlChart" runat="server"></asp:Literal>
                    <div style="text-align:center; padding:10px;">
                        <asp:Table id="tblChartLegend" runat="server" Width = "80%" HorizontalAlign="Center" BackColor="White" ></asp:Table>
                    </div>
                    
                </div>

	        </div>
	        
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Panel>

<asp:Panel ID="pnlAccessDenied" runat="server">
<div class="pckgwhtwrpr">
	<div class="pckghdgred">
		Messages and Alerts
	</div>
    <div class="amrow" id="ctl05_divNoCurrentMessages"><p class="ammessagetext">You do not have access to the Market Share Report please contact us if you would like to enable this feature.</p></div>
    
</div>
</asp:Panel>

