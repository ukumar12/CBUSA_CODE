<%@ Control EnableViewstate="true" Language="VB" AutoEventWireup="false" CodeFile="MyMoney.ascx.vb" Inherits="MyMoney" %>

<asp:Panel ID="pnlMain" runat="server">
<asp:UpdatePanel ID="upMain" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="pckgwrpr bggray">
	        <div class="pckghdgltblue">
		        Market Share Report
	        </div>
<%--<asp:button id ="btnGoToDashBoard" text="Go to DashBoard" class="btnred" runat="server" />--%>
	        <div class="pckgbdy">
        	    
        	    <%--<div class="pckghdgred" style="text-align:center;">Opt Out</div>
		        <div style="z-index:100;">
		            <p><b>Press the Opt Out button below to disable your access to the Market Share Report and prevent other vendors from seeing your data in this report.</b></p>
                    <p><CC:ConfirmButton ID="btnOptOut" runat="server" CssClass="btnblue" Text="Opt Out" Message="Are you sure you want to opt out from accessing the Market Share Report. You will not be able to access this report and other vendor will not be able to see your data." /></p>
                    
		        </div>--%>
        	    
                <div class="bdbtblhdg" style="text-align: left; font-size: 12px; color:#fff; padding: 10px 15px;margin-bottom:8px;">Select Dates</div>
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
                    <div class="bdbtblhdg" style="text-align: left; font-size: 12px; color:#fff; padding: 10px 15px;margin-bottom:8px;">Select the Supply Phase(s) you want displayed in your Market Share Report</div>
                    <div style="text-align:center;">
                        <CC:ListSelect ID="lsSupplyPhases" runat="server" AutoPostback="true" style="text-align:left; margin-left:auto; margin-right:auto;width:600px;table-layout:auto;" Height="200" AddImageUrl="/images/admin/true.gif" DeleteImageUrl="/images/admin/delete.gif" />
                    </div>

                </div>
                <div class="pckgltgraywrpr center">
                    <div class="bdbtblhdg" style="text-align: left; font-size: 12px; color:#fff; padding: 10px 15px;margin-bottom:8px;">Vendors in selected Supply Phase(s)</div>
                    <div style="text-align:left; overflow:auto; height:150px; background-color:White; margin-top:5px; margin-bottom:5px; margin-left:5px; margin-right:5px;" >
                        <b ><asp:Literal ID="ltlVendors" runat="server"></asp:Literal></b><br />
                        <asp:Repeater ID="rptVendors" runat="server">
                            <ItemTemplate><b style="margin-left:10px;"><%#DataBinder.Eval(Container.DataItem, "CompanyName")%></b><hr></ItemTemplate>
                        </asp:Repeater>
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

