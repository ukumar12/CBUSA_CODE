<%@ Page Title="" Language="VB" MasterPageFile="~/controls/AdminMaster.master" AutoEventWireup="false" CodeFile="InviteBuilder.aspx.vb" Inherits="admin_TwoPrice_Campaigns_InviteBuilder" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph" runat="Server">
    <div class="IframeDesign">
        <iframe id="urIframe" runat="server" width="100%"></iframe>
    </div>
    <Style type="text/css">
        .IframeDesign {
            width:100%;
            float:left;
        }
        .IframeDesign iframe{
                height: calc(100vh - 32px) !important;
                border: none;
        }
    </Style>
</asp:Content>

