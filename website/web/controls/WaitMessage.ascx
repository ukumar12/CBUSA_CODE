<%@ Control Language="VB" AutoEventWireup="false" CodeFile="WaitMessage.ascx.vb" Inherits="controls_WaitMessage" %>

<div id="divWait" runat="server" style="position:absolute;width:200px;height:175px;margin:50% auto;background-color:#fff;border:1px solid #ccc;text-align:center;">
    <img src="/images/loading.gif" alt="loading" style="border:none;margin:80px auto auto auto;" />
    <span id="spanMessage" runat="server"></span>
</div>