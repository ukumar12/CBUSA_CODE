<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Survey Page" CodeFile="default.aspx.vb" Inherits="Admin_Survey_Page_Default"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Survey Page</h4>

<p></p>
<CC:OneClickButton ID="BackToSurveys" runat="server" text="<< Surveys" CssClass="btn" />
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Survey Page" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" GridLines="none" CssClass="MultilineTable" CellSpacing="0" CellPadding="5" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?SurveyId=" & SurveyID & "&PageId=" & DataBinder.Eval(Container.DataItem, "PageId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Survey Page?" runat="server" NavigateUrl= '<%# "delete.aspx?SurveyId=" & SurveyID & "&PageId=" & DataBinder.Eval(Container.DataItem, "PageId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "/admin/surveys/questions/default.aspx?PageId=" & DataBinder.Eval(Container.DataItem, "PageId") & "&SurveyId=" & DataBinder.Eval(Container.DataItem, "SurveyId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ID="lnkEditQuestions">Questions</asp:HyperLink>			    
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField DataField="Name" HeaderText="Name"></asp:BoundField>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "move.aspx?SurveyId=" & SurveyID & "&ACTION=UP&PageId=" & DataBinder.Eval(Container.DataItem, "PageId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/moveup.gif" ID="lnkMoveUp">Move Up</asp:HyperLink>
		    </ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "move.aspx?SurveyId=" & SurveyID & "&ACTION=DOWN&PageId=" & DataBinder.Eval(Container.DataItem, "PageId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/movedown.gif" ID="lnkMoveDown">Move Down</asp:HyperLink>
		    </ItemTemplate>
		</asp:TemplateField>
	</Columns>
</CC:GridView>

</asp:content>
