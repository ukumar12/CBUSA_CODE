<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Builder" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<script type ="text/javascript">
    function EditPerformanceSurveyData(txtCtrlID, PerfSurveyID) {
        var editedValue = document.getElementById(txtCtrlID).value;

        document.getElementById("hdnPerfSurveyID").value = PerfSurveyID;
        document.getElementById("hdnNewSurveyData").value = editedValue;

        __doPostBack('btnDummySave', 'JavaScript');

        return false;
    }
</script>
<h4>Builder Performance Survey</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
    <tr>
        <th valign="top">Historic ID:</th>
        <td valign="top" class="field"><asp:textbox id="F_HistoricId" runat="server" Columns="50" Width="100" MaxLength="5" TextMode="Number" EnableViewState="true"></asp:textbox></td>
        <th valign="top">Builder Company:</th>
        <td valign="top" class="field">
            <asp:DropDownList ID="ddlBuilder" runat="server" style="width:250px;" EnableViewState="true"></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <th valign="top">Quarter:</th>
        <td valign="top" class="field">
            <asp:DropDownList ID="ddlQuarter" runat="server" style="width:75px;" EnableViewState="true"></asp:DropDownList>
        </td>
        <th valign="top">Year:</th>
        <td valign="top" class="field">
            <asp:DropDownList ID="ddlYear" runat="server" style="width:80px;" EnableViewState="true"></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <th valign="top">LLC:</th>
        <td valign="top" class="field">
            <asp:DropDownList ID="ddlLLC" runat="server" style="width:120px;" EnableViewState="true"></asp:DropDownList>
        </td>
        <td align="right" colspan="2">
            <asp:HiddenField ID="hdnPerfSurveyID" runat="server" ClientIDMode="Static" Value="0" />
            <asp:HiddenField ID="hdnNewSurveyData" runat="server" ClientIDMode="Static" Value="" />
            <asp:HiddenField ID="hdnReportingQuarter" runat="server" ClientIDMode="Static" Value="" />
            <asp:HiddenField ID="hdnReportingYear" runat="server" ClientIDMode="Static" Value="" />
            <asp:HiddenField ID="hdnProjectionQuarter" runat="server" ClientIDMode="Static" Value="" />
            <asp:HiddenField ID="hdnProjectionYear" runat="server" ClientIDMode="Static" Value="" />
            <CC:OneClickButton id="btnUpdateHomeStartsTo0" Runat="server" Text="Update Non-Reported As 0" cssClass="btn" />
            <CC:OneClickButton id="btnDummySave" Runat="server" Text="Dummy" cssClass="btn" style="display:none;" />
            <CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
            <input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
        </td>
    </tr>
</table>
</asp:Panel>
<p></p>
<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:BoundField SortExpression="HistoricId" DataField="HistoricId" HeaderText="Historic ID"></asp:BoundField>
        <asp:BoundField SortExpression="CompanyName" DataField="CompanyName" HeaderText="Builder Company"></asp:BoundField>
		<asp:BoundField SortExpression="Quarter" DataField="Quarter" HeaderText="Quarter"></asp:BoundField>
		<asp:BoundField SortExpression="Year" DataField="Year" HeaderText="Year"></asp:BoundField>
		<asp:BoundField SortExpression="ProjectedValue" DataField="ProjectedValue" HeaderText="Projected Value"></asp:BoundField>
        <asp:TemplateField ItemStyle-VerticalAlign="Middle" HeaderText="Edit Projection Data">
			<ItemTemplate>
                <asp:TextBox ID="txtEditProjectionData" EnableViewState="false" TextMode="Number" runat="server" Width="40px" Text='<%# DataBinder.Eval(Container.DataItem, "ProjectedValue")%>' style="vertical-align:text-bottom"></asp:TextBox>
                <asp:HyperLink ID="lnkUpdateProjection" onclick="EditPerformanceSurveyData(this);" EnableViewState="False" runat="server" NavigateUrl='<%# "/admin/builders/PerformanceSurvey/default.aspx?psid=" & DataBinder.Eval(Container.DataItem, "PerformanceSurveyId") %>' ImageUrl="~/images/admin/Save.png" ImageHeight="16" ImageWidth="16" style="vertical-align:text-bottom;">Update</asp:HyperLink>
  			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="SurveyData" DataField="SurveyData" HeaderText="Actual Value"></asp:BoundField>
        <asp:TemplateField ItemStyle-VerticalAlign="Middle" HeaderText="Edit Actual Data">
			<ItemTemplate>
                <asp:TextBox ID="txtEditSurveyData" EnableViewState="false" TextMode="Number" runat="server" Width="40px" Text='<%# DataBinder.Eval(Container.DataItem, "SurveyData")%>' style="vertical-align:text-bottom"></asp:TextBox>
                <asp:HyperLink ID="lnkUpdate" onclick="EditPerformanceSurveyData(this);" EnableViewState="False" runat="server" NavigateUrl='<%# "/admin/builders/PerformanceSurvey/default.aspx?psid=" & DataBinder.Eval(Container.DataItem, "PerformanceSurveyId") %>' ImageUrl="~/images/admin/Save.png" ImageHeight="16" ImageWidth="16" style="vertical-align:text-bottom;">Update</asp:HyperLink>
  			</ItemTemplate>
		</asp:TemplateField>
        <asp:BoundField SortExpression="UpdatedOn" DataField="UpdatedOn" HeaderText="Last Updated On"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>
