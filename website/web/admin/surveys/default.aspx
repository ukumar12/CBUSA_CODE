<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Survey" CodeFile="default.aspx.vb" Inherits="Admin_Survey_Default"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Survey</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Name:</th>
<td valign="top" class="field"><asp:textbox id="F_Name" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Is Active:</b></th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_IsActive" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Value="1">Yes</asp:ListItem>
		<asp:ListItem Value="0">No</asp:ListItem>
	</asp:DropDownList>
</td>
</tr>
<tr>
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
</table>
</asp:Panel>
<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Survey" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?SurveyId=" & DataBinder.Eval(Container.DataItem, "SurveyId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Survey?" runat="server" NavigateUrl= '<%# "delete.aspx?SurveyId=" & DataBinder.Eval(Container.DataItem, "SurveyId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "/admin/surveys/page/default.aspx?SurveyId=" & DataBinder.Eval(Container.DataItem, "SurveyId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ID="lnkEditSurvey">Edit Survey</asp:HyperLink>
			    
			</ItemTemplate>
		</asp:TemplateField>
		
		<asp:BoundField SortExpression="Name" DataField="Name" HeaderText="Name"></asp:BoundField>
		<asp:BoundField SortExpression="StartDate" DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="EndDate" DataField="EndDate" HeaderText="End Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
        <asp:TemplateField>
            <itemtemplate>
			<a href="<%=System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName")%>/surveys/start.aspx?SurveyId=<%# Container.DataItem("SurveyId") %>" target="_blank"><%=System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName")%>/surveys/start.aspx?SurveyId=<%# Container.DataItem("SurveyId") %></a>
			    
			</itemtemplate>
        </asp:TemplateField>
		<asp:Checkboxfield ItemStyle-HorizontalAlign="Center" SortExpression="IsActive" DataField="IsActive" HeaderText="Is Active"/>
		<asp:Checkboxfield ItemStyle-HorizontalAlign="Center" SortExpression="IsBuilder" DataField="IsBuilder" HeaderText="Builder"/>
		<asp:Checkboxfield ItemStyle-HorizontalAlign="Center" SortExpression="IsVendor" DataField="IsVendor" HeaderText="Vendor"/>
		<asp:Checkboxfield ItemStyle-HorizontalAlign="Center" SortExpression="IsPIQ" DataField="IsPIQ" HeaderText="PIQ"/>
		
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "/admin/surveys/reports/default.aspx?SurveyId=" & DataBinder.Eval(Container.DataItem, "SurveyId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ID="lnkBtnViewReport">Results</asp:HyperLink>
			    
			</ItemTemplate>
		</asp:TemplateField>
		<asp:templateField>
		    <itemtemplate>
		        <asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "/admin/surveys/fullsurveys/default.aspx?SurveyId=" & DataBinder.Eval(Container.DataItem, "SurveyId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ID="lnkBtnViewSurveys">Surveys</asp:HyperLink>
		    </itemtemplate>
		</asp:templateField>
	</Columns>
</CC:GridView>

</asp:content>
