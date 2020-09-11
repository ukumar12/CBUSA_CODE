<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" CodeFile="Default.aspx.vb" Inherits="admin_surveys_reports_Default" %>
<%@ Reference Control="~\admin\surveys\reports\controls\shortresponse.ascx"  %>
<%@ Reference Control="~\admin\surveys\reports\controls\longresponse.ascx"  %>
<%@ Reference Control="~\admin\surveys\reports\controls\selectone.ascx"  %>
<%@ Reference Control="~\admin\surveys\reports\controls\selectallthatapply.ascx"  %>
<%@ Reference Control="~\admin\surveys\reports\controls\rate.ascx"  %>
<%@ Reference Control="~\admin\surveys\reports\controls\standardrank.ascx"  %>
<%@ Reference Control="~\admin\surveys\reports\controls\percentrank.ascx"  %>
<%@ Reference Control="~\admin\surveys\reports\controls\date.ascx"  %>
<%@ Reference Control="~\admin\surveys\reports\controls\quantity.ascx"  %>
<%@ Reference Control="~\admin\surveys\reports\controls\demographic.ascx"  %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Survey Results</h4>

<p></p>
<CC:OneClickButton ID="btnRefresh" Text="Refresh" runat="server" CssClass="btn" />

<p></p>
<asp:PlaceHolder ID="plcReport" runat="server"></asp:PlaceHolder>

</asp:content>
