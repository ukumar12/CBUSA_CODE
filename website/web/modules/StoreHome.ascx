<%@ Control Language="VB" AutoEventWireup="false" CodeFile="StoreHome.ascx.vb" Inherits="modules_StoreHome" %>
<asp:DataList runat="server" id="dlDepartments" enableviewstate="False" RepeatDirection="Horizontal" RepeatColumns="2" ItemStyle-VerticalAlign="top" ItemStyle-HorizontalAlign="right" ItemStyle-Width="50%" CellPadding="12" width="720">
	<ItemTemplate>
			<table border="0" cellspacing="4" cellpadding="0" width="100%">
				<tr valign="top">
					<td style="text-align:right;">
						<a href="<%#IIf(Not IsDBNull(Container.DataItem("CustomUrl")), Container.DataItem("CustomUrl"), "/store/main.aspx?DepartmentId=" & Container.DataItem("DepartmentId"))%>"><img src="/assets/item/related/<%#Container.DataItem("Image")%>" border="0" alt="<%#Container.DataItem("ItemName").ToString.Replace("""", "'")%>" /></a><br />
						<img src="/images/spacer.gif" width="75" height="1" /><br />
					</td>
					<td>
						<table border="0" cellspacing="0" cellpadding="0">
							<tr>
								<th align="left" style="padding-left:5px;">
								<a href="<%#IIf(Not IsDBNull(Container.DataItem("CustomURL")), Container.DataItem("CustomURL"), "/store/default.aspx?DepartmentId=" & Container.DataItem("DepartmentId"))%>" class="noul">Browse <%#Container.DataItem("Name")%></a>
								</th>
							</tr>

							<asp:Repeater runat="server" id="rpt">
								<ItemTemplate>
									<tr>
										<td style="text-align:left;width:230px;padding-bottom:3px;padding-left:5px;">
											<a href="<%#IIf(Not IsDBNull(Container.DataItem("CustomURL")), Container.DataItem("CustomURL"), "/store/default.aspx?DepartmentId=" & Container.DataItem("DepartmentId"))%>"><asp:Literal runat="server" id="lit" /></a>
										</td>
									</tr>
								</ItemTemplate>
							</asp:Repeater>

							<tr>
								<td style="text-align:left;padding-top:2px;padding-left:5px;">
									<a class="bold" href="<%#IIf(Not IsDBNull(Container.DataItem("CustomURL")), Container.DataItem("CustomURL"), "/store/default.aspx?DepartmentId=" & Container.DataItem("DepartmentId"))%>">Shop All <%#Container.DataItem("Name")%> &raquo;</a>
								</td>
							</tr>
						</table>

					</td>
					<%#IIf(Container.ItemIndex Mod 3 = 2, "", "<td><img src=""/images/spacer.gif"" width=""10"" height=""1"" /><br /></td>")%>
				</tr>
			</table>
	</ItemTemplate>
</asp:DataList>