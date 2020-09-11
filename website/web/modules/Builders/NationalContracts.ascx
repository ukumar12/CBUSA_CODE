<%@ Control Language="VB" AutoEventWireup="false" CodeFile="NationalContracts.ascx.vb" Inherits="modules_Builder_NationalCOntracts" %>

<div class="pckgltgraywrpr">
	<div class="pckghdgred nobdr">
		National Contracts Program
	</div> 
    <div class="stacktblwrpr themeprice">
    <div class="bdbtblhdg">
			<div class="caption">My Contracts</div>
		    <div class="clear">&nbsp;</div>
		</div>
    
        <CC:GridView id="gvListJoined" SortBy="ContractID" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="False" AllowSorting="False" EmptyDataText="You are not participating in any national contracts." AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
            <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	        <RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>

            <Columns>
                <asp:TemplateField>
                <ItemStyle width="75px" />
                    <HeaderTemplate>
                        Category
                    </HeaderTemplate>
		    	    <ItemTemplate>
		    	        <asp:Literal ID="ltlTitle" runat="server" />
		    	    </ItemTemplate>
		        </asp:TemplateField>

                <asp:TemplateField>
                 <ItemStyle width="100px" />
                    <HeaderTemplate>
                        Manufacturer
                    </HeaderTemplate>
		    	    <ItemTemplate>
		    	        <asp:Literal ID="ltlManufacturer" runat="server" />
		    	    </ItemTemplate>
		        </asp:TemplateField>
	    	    <asp:BoundField SortExpression="Products" DataField="Products" HeaderText="Products Covered"></asp:BoundField>

            <asp:TemplateField>
                 <ItemStyle width="50px" />
                    <HeaderTemplate>
                        Contract Terms
                    </HeaderTemplate>
	    		    <ItemTemplate>
                    <asp:Literal ID="ltlContractTerms" runat="server" />
	    		    </ItemTemplate>
	    	    </asp:TemplateField>


                <asp:TemplateField>
                 <ItemStyle width="125px" />
                    <HeaderTemplate>
                        Details
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Button  ID="lnkListJoinedDetails" CommandName ="ViewDetails" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"ContractID") %>' runat="server" Text="View NCP Details" Target="_blank" cssClass="btnblue" />
                     <asp:HyperLink ID="lnkListJoinedLandingPage" runat="server" Text="View NCP Details" Target="_blank" CssClass="btnblue" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </CC:GridView>
    </div>    

   <div class="stacktblwrpr themeprice">
	 <div class="bdbtblhdg">
			<div class="caption">National Contracts I Can Join</div>
            <div class="clear">&nbsp;</div>
        </div>
  
        <CC:GridView id="gvListAvail" CssClass="" SortBy="ContractID" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="False" AllowSorting="False" EmptyDataText="There are no national contracts you can join." AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
            <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	        <RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>

            <Columns>
                <asp:TemplateField>
                  <ItemStyle width="75px" />
                    <HeaderTemplate>
                        Category
                    </HeaderTemplate>
		    	    <ItemTemplate>
		    	        <asp:Literal ID="ltlTitle" runat="server" />
		    	    </ItemTemplate>
		        </asp:TemplateField>

                <asp:TemplateField>
                  <ItemStyle width="100px" />
                    <HeaderTemplate>
                        Manufacturer
                    </HeaderTemplate>
		    	    <ItemTemplate>
		    	        <asp:Literal ID="ltlManufacturer" runat="server" />
		    	    </ItemTemplate>
		        </asp:TemplateField>
		        <asp:BoundField SortExpression="Products" DataField="Products" HeaderText="Products Covered"></asp:BoundField>

               <asp:TemplateField>
                 <ItemStyle width="50px" />
                    <HeaderTemplate>
                        Contract Terms
                    </HeaderTemplate>
	    		    <ItemTemplate>
                    <asp:Literal ID="ltlContractTerms" runat="server" />
	    		    </ItemTemplate>
	    	    </asp:TemplateField>
 

                <asp:TemplateField>
                  <ItemStyle width="125px" />
                    <HeaderTemplate>
                        To Review/Join
                    </HeaderTemplate>
                    <ItemTemplate>
                     <asp:Button  ID="lnkListAvailDetails" CommandName ="ViewDetails"   CommandArgument='<%# DataBinder.Eval(Container.DataItem,"ContractID") %>' Visible ="false"  runat="server" Text="View NCP Details" Target="_blank" cssClass="btnblue" />
                       <asp:HyperLink ID="lnkListAvailLandingPage" runat="server" Text="View NCP Details" Target="_blank" CssClass="btnblue" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </CC:GridView>
    
</div>
</div>