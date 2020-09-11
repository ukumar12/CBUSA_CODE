<%@ Page Language="VB" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="takeoffs_default" %>

<%@ Register TagName="VendorRating" TagPrefix="CC" Src="~/controls/VendorRating.ascx" %>

<CT:MasterPage ID="CTMain" runat="server">
    <asp:PlaceHolder runat="server">
       <script type="text/javascript">

</script>
        <div class="pckggraywrpr">
            <div class="pckghdgred">Awarded Committed Purchase Events 
              </div>
            <div class="table-body-wrap" style="height: 15px;"></div>
            <CC:GridView id="gvList" CellSpacing="2" CellPadding="2" CssClass="tblcomprlen" runat="server" PageSize="10" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	            <HeaderStyle CssClass="sbttl" />
	            <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top" Font-Size="Larger"></AlternatingRowStyle>
	            <RowStyle CssClass="row" VerticalAlign="Top" Font-Size="Larger" Font-Bold="true"></RowStyle>
	            <Columns>
		            <asp:TemplateField>
                        <headertemplate>
                            Commited Purchase Event
                        </headertemplate>
			            <ItemTemplate>
                            
                            <asp:linkbutton ID="lnkCommitted" runat="server" Enabled=<%# If(Eval("DisplayValue").ToString().ToLower() = "not participating", False, True) %>  CommandName="ChangeTakeoff" CommandArgument="" Style="color:black"></asp:linkbutton>
			            </ItemTemplate> 
		            </asp:TemplateField>
                     <asp:TemplateField>
                         <headertemplate>
                            Participation
                        </headertemplate>
			            <ItemTemplate>
                             <asp:Label runat="server" id="lblDisplayValue" Text='<%# Eval("DisplayValue")%>'></asp:Label>&nbsp&nbsp
                             <asp:LinkButton id="btnProjectDetails" runat="server" commandname="OpenPopUp" ToolTip="Project Details" commandargument='<%# DataBinder.Eval(Container.DataItem, "TwoPriceCampaignId") %>' OnClick ="GetProjectDetails" visible='<%# Eval("TwoPriceBuilderParticipationTypeId") = 1%>'><i class="fa fa-info-circle" aria-hidden="true"></i></asp:LinkButton>
                         </ItemTemplate>
		            </asp:TemplateField>
                    <asp:BoundField DataField="StartDate" HeaderText="Start Date" SortExpression="StartDate" DataFormatString="{0:M/d/yyyy}" />
                    <asp:BoundField DataField="EndDate" HeaderText="End Date" SortExpression="EndDate" DataFormatString="{0:M/d/yyyy}" />
	            </Columns>
            </CC:GridView>         
        </div>
        <div class="pckggraywrpr">
            <div class="pckghdgred">Pending Committed Purchase Events</div>
            <div style="height: 15px;"></div>
            <CC:GridView id="gvpendinglist" CellSpacing="2" CellPadding="2" CssClass="tblcomprlen" runat="server" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0">
	            <HeaderStyle CssClass="sbttl" />
	            <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top" Font-Size="Larger"></AlternatingRowStyle>
	            <RowStyle CssClass="row" VerticalAlign="Top" Font-Size="Larger" Font-Bold="true"></RowStyle>
	            <Columns>

                    <asp:BoundField DataField="Name" HeaderText="Committed Purchase Event" SortExpression="Participation" />
                    <asp:BoundField DataField="StartDate" HeaderText="Start Date" SortExpression="StartDate" DataFormatString="{0:M/d/yyyy}" />
                    <asp:BoundField DataField="EndDate" HeaderText="End Date" SortExpression="EndDate" DataFormatString="{0:M/d/yyyy}" />
                    <asp:BoundField DataField="CreatedOn" HeaderText="Response Date" SortExpression="EnrollmentDate" DataFormatString="{0:M/d/yyyy}" />
                     <asp:TemplateField  HeaderText="Deadline">
                    <ItemTemplate>	
                        <asp:Label runat="server" Text='<%# If(Eval("ResponseDeadline") = "1900-01-01","",Eval("ResponseDeadline")) %>'>
                        </asp:Label>
                     </ItemTemplate>
                         </asp:TemplateField>
                    <%--<asp:BoundField DataField="<%#  %>" HeaderText="Deadline" SortExpression="Deadline" DataFormatString="{0:M/d/yyyy}" />--%>
                    <asp:BoundField DataField="DisplayValue" HeaderText="Participation" SortExpression="Participation" />  
                    <asp:BoundField DataField="TwoPriceCampaignId" HeaderText="TwoPriceCampaignId" SortExpression="TwoPriceCampaignId" visible="false" />  
                    <asp:TemplateField>
			            <ItemTemplate>			 
                            <asp:LinkButton id="lnkbtnCPParticipation" runat="server" 
                                text='<%# If(Eval("DisplayValue").ToString().ToLower() = "participate with specific projects" And DateTime.Now.Date > DateTime.Parse(Eval("ResponseDeadline")).Date And Val(Eval("AdminPermissionForPostDeadLine")) = 0, "View Projects", "Update Participation/Projects") %>'
                                style="text-decoration: underline;" onclick="GetProjectDetails" ></asp:LinkButton>
                            <asp:Label runat="server" Text='<%# Eval("TwoPriceBuilderParticipationTypeId") %>' visible="false"></asp:Label>   
                            <asp:Label runat="server" Text='<%# Eval("AdminPermissionForPostDeadLine") %>' visible="false"></asp:Label> 
                            
                            <asp:HiddenField ID="hdnAdminPermissionForPostDeadLine" runat="server" Value='<%# Eval("AdminPermissionForPostDeadLine") %>' /> 

                            
			            </ItemTemplate>
                             <HeaderTemplate>
                               Builder Action
                            </HeaderTemplate>
		            </asp:TemplateField>
	            </Columns>
            </CC:GridView>          
        </div>
                 
    </asp:PlaceHolder>
    <div id="AddPtfProject" class="overlay-bg" style="display:none;" runat="server">
    <div  class="windowPerformance committed-popup"  runat="server" style="height:auto;padding-bottom: 10px;">
    <div class="pckghdgred" style="height:auto">PARTICIPATE WITH SPECIFIC PROJECTS <a class="close-icon" onclick="CloseDiv();" href="#"><i class="fa fa-times" aria-hidden="true"  ></i></a></div>
        <div class="table-body-wrap" runat="server" id="OldStyleProjects" style="display:none;">
            <div id="PrtFolioHomes">
            <h3>
            <label id="lblPort" >Portfolio Homes</label>
            </h3>
                <CC:GridView CausesValidation="false" id="gvPtfList"  width="100%"  border="0" runat="server"  AllowSorting="True"   AutoGenerateColumns="False"   class="tblcompr" style="margin:0px;background-color: transparent;" EmptyDataText="No projects available">
                <AlternatingRowStyle CssClass="Cprow" VerticalAlign="Top" Font-Size="Larger"></AlternatingRowStyle>
                <RowStyle CssClass="Cprow" VerticalAlign="Top" Font-Size="Larger" Font-Bold="true"></RowStyle>
                <Columns>
                <asp:BoundField SortExpression="Community" DataField="Community" HeaderText="Community"></asp:BoundField>
                <asp:BoundField SortExpression="LotNumberorAddress" DataField="ProjectName" HeaderText="Lot Number or Address"></asp:BoundField>
                <asp:BoundField SortExpression="Plan" DataField="FloorPlan" HeaderText="Plan"></asp:BoundField>
                <asp:BoundField SortExpression="Status" DataField="DisplayValue" HeaderText="Status"></asp:BoundField>	
                <asp:BoundField SortExpression="StatusValue" DataField="TwoPriceBuilderProjectStatusId" HeaderText="StatusValue" ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol"></asp:BoundField>         		
                </Columns>
                </CC:GridView>
            </div>
            <div id="CustomHomes">
            <h3>
            <label id="lblCust" >Custom Homes</label></h3>
                <CC:GridView CausesValidation="false" id="gvCustList"  width="100%"  border="0" runat="server" AllowSorting="True"   AutoGenerateColumns="False"  class="tblcompr" style="margin:0px;background-color: transparent;" EmptyDataText="No projects available">
                <AlternatingRowStyle CssClass="Cprow" VerticalAlign="Top" Font-Size="Larger"></AlternatingRowStyle>
                <RowStyle CssClass="Cprow" VerticalAlign="Top" Font-Size="Larger" Font-Bold="true"></RowStyle>
                <Columns>
                <asp:BoundField SortExpression="ProjectTitle/Address" DataField="ProjectName" HeaderText="Project Title/Address"></asp:BoundField>
                <asp:BoundField SortExpression="Takeoff" DataField="TakeOffInSystem" HeaderText="Takeoff"></asp:BoundField>
                <asp:BoundField SortExpression="SF" DataField="SquareFeet" HeaderText="SF"></asp:BoundField>
                <asp:BoundField SortExpression="Stories" DataField="Stories" HeaderText="Stories"></asp:BoundField>
                <asp:BoundField SortExpression="SpecialMaterials/Notes" DataField="SpecialMaterials" HeaderText="Special Materials/Notes"></asp:BoundField>         
                <asp:BoundField SortExpression="Status" DataField="DisplayValue" HeaderText="Status"></asp:BoundField>
                <asp:BoundField SortExpression="StatusId" DataField="TwoPriceBuilderProjectStatusId" HeaderText="StatusValue" ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol"></asp:BoundField>
      
                </Columns>
                </CC:GridView>
            </div>
        </div>
          <div class="table-body-wrap" runat="server" id="NewStyleProjects" style="display:none; padding:0px!important;">
            <br />
          
                                <CC:GridView ID="grdProjList" runat="server" AutoGenerateColumns="true" OnRowDataBound="grdProjList_RowDataBound"  width="100%"  class="tblcompr" style="margin:0px;background-color: transparent;"   EmptyDataText="No projects available"
                                    ShowHeaderWhenEmpty="true" Visible="true">
                                      <AlternatingRowStyle CssClass="Cprow" VerticalAlign="Top" Font-Size="Larger"></AlternatingRowStyle>
                                <RowStyle CssClass="Cprow" VerticalAlign="Top" Font-Size="Larger" Font-Bold="true"></RowStyle>                               

                                </CC:GridView>
                  
         </div>
     </div></div>
     <script src="../../includes/moment.js"></script>
    <script type="text/javascript">
        function CloseDiv() {
            $("#AddPtfProject").hide()
        }
        function ValidateClick(ResponseDeadline, Status) {
            alert(ResponseDeadline);
            alert(Status);
            if (Status != "participate with specific projects" && ResponseDeadline == true) {
                return false;
            }
        }
    </script>
    <style type="text/css">
        .windowPerformance.committed-popup .close-icon {
            position: absolute;
            right: 10px;
            top: 50%;
            margin-top: -8px;
        }

            .windowPerformance.committed-popup .close-icon .fa {
                font-size: 16px;
                color: #ccc;
            }

        .windowPerformance.committed-popup .pckghdgred {
            position: relative;
        }

        .windowPerformance.committed-popup {
            position: absolute;
            z-index: 10;
            background-color: #e1e1e1;
            border: 2px solid #c2c2c2;
            margin-bottom: 15px;
            margin-left: -300px;
            margin-top: -150px;
            width: 600px;
            left: 50%;
            height: 294px;
            top: 50%;
        }

            .windowPerformance.committed-popup table td {
                /*border: none !important;*/
                font-size: 12px !important;
                padding: 8px 6px !important;
                border-top: 1px solid #000 !important;
            }

        .committed-popup .hiddencol {
            display: none;
            width: 0px;
        }

        .committed-popup .overlay {
            position: fixed;
            top: 0;
            bottom: 0;
            left: 0;
            right: 0;
            background: rgba(0, 0, 0, 0.7);
            transition: opacity 500ms;
            visibility: hidden;
            opacity: 0;
        }

            .committed-popup .overlay:target {
                visibility: visible;
                opacity: 1;
            }

        .committed-popup .table-body-wrap {
            overflow-y: auto;
            background-color: #FFF;
            padding: 2px 15px 8px;
            margin: 10px;
            height: 208px;
            font-family: montserrat;
        }

            .committed-popup .table-body-wrap h3 {
                margin: 15px 0px;
            }

        .windowPerformance.committed-popup table td:first-child {
            padding-left: 8px !important;
        }

        .committed-popup .table-body-wrap table {
            margin-top: 0px !important;
        }

        .tblcomprlen table, td {
            font-size: 12px;
            vertical-align: top;
        }

        .tblcomprlen i.fa.fa-info-circle {
            text-align: center;
            padding-left: 3px;
        }

        .overlay-bg {
            background: rgba(0,0,0,0.2);
            position: fixed;
            width: 100%;
            height: 100%;
            top: 0;
            left: 0px;
            z-index: 10;
        }

        #NewStyleProjects > div {
            padding-right: 15px;
            float: left;
        }
    </style>
</CT:MasterPage>
