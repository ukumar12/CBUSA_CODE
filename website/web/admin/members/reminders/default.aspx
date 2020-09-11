<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Inherits="member_reminders_default" CodeFile="default.aspx.vb" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>View Reminders</h4>

Reminder Information for <asp:Label ID="txtMemberName" runat="server"></asp:Label>
| <a runat="server" id="lnkBack">&laquo; Go Back to Member Profile</a><br /><br />

<CC:OneClickButton id="btnAdd" runat="server" Text="Add New Reminder" CSsClass="btn" /><br /><br />

   
    <CC:GridView id="gvReminders" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" EmptyDataText="There are currrently no reminders for this member" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
		    <ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?MemberId="& MemberId & "&ReminderId=" & DataBinder.Eval(Container.DataItem, "ReminderId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Member?" runat="server" NavigateUrl= '<%# "delete.aspx?MemberId=" & MemberId & "&ReminderId=" & DataBinder.Eval(Container.DataItem, "ReminderId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
		    <HeaderTemplate>
		         Name
		    </HeaderTemplate>
		    <ItemTemplate>
		          <asp:literal id="ltlName" runat="server" />
		    </ItemTemplate>		
		</asp:TemplateField>
		<asp:TemplateField>
		    <HeaderTemplate>
		         Recurring
		    </HeaderTemplate>
		    <ItemTemplate>
		         <asp:literal id="ltlRecurs" runat="server" />
		    </ItemTemplate>		
		</asp:TemplateField>
		 <asp:TemplateField>
		    <HeaderTemplate>
		         Date
		    </HeaderTemplate>
		    <ItemTemplate>
		         <asp:literal id="ltlDate" runat="server" />
		    </ItemTemplate>		
		</asp:TemplateField>
		 <asp:TemplateField>
		    <HeaderTemplate>
		         First Reminder
		    </HeaderTemplate>
		    <ItemTemplate>
		         <asp:literal id="ltlFirstReminder" runat="server" />
		    </ItemTemplate>		
		</asp:TemplateField>	
		 <asp:TemplateField>
		    <HeaderTemplate>
		         Second Reminder
		    </HeaderTemplate>
		    <ItemTemplate>
		         <asp:literal id="ltlSecondReminder" runat="server" />
		    </ItemTemplate>		
		</asp:TemplateField>		
	</Columns>
    </CC:GridView>
  
    
</asp:content>