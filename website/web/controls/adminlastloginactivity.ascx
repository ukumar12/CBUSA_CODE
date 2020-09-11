<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AdminLastLoginActivity.ascx.vb" Inherits="AdminLastLoginActivity" %>
<h4>Login Activity</h4>
<asp:repeater id="LastActivity" runat="server">
	<HeaderTemplate>
	    <span class="smaller">Showing the last 10 successfuly logins.</span>
		<table border="0" cellspacing="0" cellpadding="4" width="600">
			<tr>
				<th>
					Username</th>
				<th>
					Full Name</th>
				<th>
					IP Address</th>
				<th>
					Entry Date/Time</th>
			</tr>
	</HeaderTemplate>
	<ItemTemplate>
		<tr valign="top" class="row">
			<td><%# DataBinder.Eval(Container.DataItem, "Username") %></td>
			<td><%# DataBinder.Eval(Container.DataItem, "FullName") %></td>
			<td><%# DataBinder.Eval(Container.DataItem, "RemoteIP") %></td>
			<td><%# DataBinder.Eval(Container.DataItem, "LoginDate") %></td>
		</tr>
	</ItemTemplate>
	<AlternatingItemTemplate>
		<tr valign="top" class="alternate">
			<td><%# DataBinder.Eval(Container.DataItem, "Username") %></td>
			<td><%# DataBinder.Eval(Container.DataItem, "FullName") %></td>
			<td><%# DataBinder.Eval(Container.DataItem, "RemoteIP") %></td>
			<td><%# DataBinder.Eval(Container.DataItem, "LoginDate") %></td>
		</tr>
	</AlternatingItemTemplate>
	<FooterTemplate>
		</table>
	</FooterTemplate>
</asp:repeater>
<asp:placeholder id="NoRecords" runat="server" visible="false">
There is no history for user login activity.
</asp:placeholder>