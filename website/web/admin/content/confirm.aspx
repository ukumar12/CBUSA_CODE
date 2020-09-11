<%@ Page Language="VB" MasterPageFile="~/controls/AdminMaster.master" AutoEventWireup="false" CodeFile="Confirm.aspx.vb" Inherits="Confirm" title="Confirmation Page" %>
<asp:Content ID="Content" ContentPlaceHolderID="ph" Runat="Server">

<h4>Confirmation Page</h4>

<p>
The page has been published.
</p>

<p>
<ul>
<% If Not dbPage.PageURL = String.Empty Then%>
<li>Click <a href="<%=GlobalRefererName %><%=dbPage.PageURL %>" target="_blank">here</a> to see it.</li>
<li>Click <a href="/admin/content/pages/default.aspx?EditPageId=<%=dbPage.PageId %>">here</a> to edit this page.</li>
<li>Click <a href="/admin/content/pages/">here</a> to go back to edit another page.</li>
<% End If%>

<% If Not dbPage.SectionId = Nothing Then%>
<li>Click <a href="/admin/content/sections/default.aspx?EditTemplateId=<%=dbPage.TemplateId %>&EditSectionId=<%=dbPage.SectionId %>">here</a> to edit a section.</li>
<% End If%>

<%  If Not dbPage.TemplateId = Nothing Then%>
<li>Click <a href="/admin/content/templates/default.aspx?EditTemplateId=<%=dbPage.TemplateId %>">here</a> to edit a template.</li>
<% End If%>
</ul>
</p>
</asp:Content>

