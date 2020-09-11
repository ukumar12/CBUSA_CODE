<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Mailing Message" CodeFile="default.aspx.vb" Inherits="index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Broadcast E-mails</h4>

<span class="smaller">Please provide search criteria below</span>
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Sent Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_SentDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_SentDateUbound" runat="server" /></td>
</tr>
</table>
</td>
<td>
<CC:DateValidator ID="DateValidator1" runat="server" Display="Dynamic" ControlToValidate="F_SentDateLbound" ErrorMessage="Invalid 'From date'"></CC:DateValidator><br />
<CC:DateValidator ID="DateValidator2" runat="server" Display="Dynamic" ControlToValidate="F_SentDateUbound" ErrorMessage="Invalid 'To date'"></CC:DateValidator>
</td>
</tr>
<tr>
<th valign="top"><b>Template:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_TemplateId" runat="server" /></td>
</tr>
<tr>
<th valign="top">Status:</th>
<td valign="top" class="field">
<asp:DropDownList ID="F_Status" runat="server">
<asp:ListItem Value="">-- ALL --</asp:ListItem>
<asp:ListItem Value="SAVED">Saved</asp:ListItem>
<asp:ListItem Value="SENT">Sent</asp:ListItem>
<asp:ListItem Value="SCHEDULED">Scheduled</asp:ListItem>
</asp:DropDownList>
</td>
</tr>
<tr>
<th valign="top">Recipient Type:</th>
<td valign="top" class="field">
<asp:DropDownList ID="F_TargetType" runat="server">
<asp:ListItem Value="">-- ALL --</asp:ListItem>
<asp:ListItem Value="DYNAMIC">Uploaded List(s)</asp:ListItem>
<asp:ListItem Value="MEMBER">Subscribers</asp:ListItem>
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
<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New E-mail" CausesValidation="false"  cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "layout.aspx?MessageId=" & DataBinder.Eval(Container.DataItem, "MessageId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEditView">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Mailing Message?" runat="server" NavigateUrl= '<%# "delete.aspx?MessageId=" & DataBinder.Eval(Container.DataItem, "MessageId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="mm.MessageId" DataField="MessageId" HeaderText="ID"></asp:BoundField>
		<asp:BoundField SortExpression="mm.Name" DataField="Name" HeaderText="Name"></asp:BoundField>
		<asp:BoundField SortExpression="mt.TemplateName" DataField="TemplateName" HeaderText="Template"></asp:BoundField>
        <asp:TemplateField SortExpression="Y" >
            <HeaderTemplate>
                    <asp:LinkButton enableviewstate="False" CommandArgument="mm.MimeType" CommandName="sort" id="SortMime" runat="server">MIME Type</asp:LinkButton>
                    <asp:LinkButton enableviewstate="False" visible='<%# gvList.SortBy = "mm.MimeType" and gvList.SortOrder = "ASC" %>' CommandArgument="mm.MimeType" CommandName=sort id="Linkbutton4" runat="server"><img border="0" src="/images/admin/Asc3.gif" alt="" align="absmiddle" /></asp:LinkButton>
                    <asp:LinkButton enableviewstate="False" visible='<%# gvList.SortBy = "mm.MimeType" and gvList.SortOrder = "DESC" %>' CommandArgument="mm.MimeType" CommandName=sort id="sortLoginDesc" runat="server"><img border="0" src="/images/admin/Desc3.gif" alt="" align="absmiddle" /></asp:LinkButton>
                    <br />
                    <asp:LinkButton enableviewstate="False" CommandArgument="mm.TargetType" CommandName="sort" id="SortTarget" runat="server">Recipient Type</asp:LinkButton>
                    <asp:LinkButton enableviewstate="False" visible='<%# gvList.SortBy = "mm.TargetType" and gvList.SortOrder = "ASC" %>' CommandArgument="mm.TargetType" CommandName=sort id="Linkbutton3" runat="server"><img border="0" src="/images/admin/Asc3.gif" align="absmiddle" alt="" /></asp:LinkButton>
                    <asp:LinkButton enableviewstate="False" visible='<%# gvList.SortBy = "mm.TargetType" and gvList.SortOrder = "DESC" %>' CommandArgument="mm.TargetType" CommandName=sort id="sortLogin1Desc" runat="server"><img border="0" src="/images/admin/Desc3.gif" align="absmiddle" alt="" /></asp:LinkButton>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label enableviewstate="False" runat="server" ID="lblMimeType"></asp:Label><br />
                <asp:Label enableviewstate="False" runat="server" ID="lblTargetType"></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
		<asp:BoundField SortExpression="mm.Subject" DataField="Subject" HeaderText="Subject"></asp:BoundField>
            <asp:TemplateField SortExpression="Y" >
            <HeaderTemplate>
                    <asp:LinkButton enableviewstate="False" CommandArgument="mm.Status" CommandName="sort" id="SortStatus" runat="server">Status</asp:LinkButton>
                    <asp:LinkButton enableviewstate="False" visible='<%# gvList.SortBy = "mm.Status" and gvList.SortOrder = "ASC" %>' CommandArgument="mm.Status" CommandName=sort id="LinkbuttonStatus" runat="server"><img border="0" src="/images/admin/Asc3.gif" alt="" align="absmiddle" /></asp:LinkButton>
                    <asp:LinkButton enableviewstate="False" visible='<%# gvList.SortBy = "mm.Status" and gvList.SortOrder = "DESC" %>' CommandArgument="mm.Status" CommandName=sort id="sortStatusDesc" runat="server"><img border="0" src="/images/admin/Desc3.gif" alt="" align="absmiddle" /></asp:LinkButton>
            </HeaderTemplate>
            <ItemTemplate>
                <%# Container.Dataitem("Status")%><br />
                <asp:Label enableviewstate="False" runat="server" ID="lblDateTime"></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField SortExpression="mm.ModifyDate" DataField="ModifyDate" HeaderText="Modify Date" DataFormatString="{0: MM/dd/yyyy <br />hh:mm:ss tt}" HTMLEncode="False"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>

