<%@ Control AutoEventWireup="false" CodeFile="MemberAddress.ascx.vb" Inherits="MemberAddress" Language="VB" %>
<%@ Import Namespace="System.Web"%>
<%@ Import Namespace="Components"%>
<%@ Import Namespace="DataLayer"%>

<table cellspacing="0" cellpadding="3" width="100%">
<tr>
	<td style="width: 70px;"><b>Name:</b></td>
	<td><% =Core.BuildFullName(Address.FirstName, String.Empty, Address.LastName)%></td>
</tr>
<%  If Not Address.Company = String.Empty Then%>
	<tr>
		<td style="width: 70px;"><b>Company:</b></td>
		<td><%=Server.HtmlEncode(Address.Company)%></td>
	</tr>
<%	end if %>
<tr>
	<td><b>Address:</b></td>
	<td><%=Server.HtmlEncode(Address.Address1)%></td>
</tr>
<%  If Not Address.Address2 = String.Empty Then%>
	<tr>
		<td><b>&#160;</b></td>
		<td><%=Server.HtmlEncode(Address.Address2)%></td>
	</tr>
<%	end if %>
<tr>
	<td><b>&nbsp;</b></td>
	<td><%=Address.City%>, <% =Address.StateName%>  <%=Address.Zip%></td>
</tr>
<%  If Not Address.Region = String.Empty Then%>
	<tr>
		<td><b>Region:</b></td>
		<td><%=Server.HtmlEncode(Address.Region)%></td>
	</tr>
<%	end if %>
<tr>
	<td><b>Country:</b></td>
	<td><%=Address.CountryName%></td>
</tr><tr>
	<td><b>Phone:</b></td>
	<td ><%=Server.HtmlEncode(Address.Phone)%></td>
</tr>
</table>