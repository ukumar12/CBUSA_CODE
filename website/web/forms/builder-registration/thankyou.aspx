<%@ Page Language="vb" AutoEventWireup="false" Inherits="Components.SitePage" %>
<%@Register TagName="BuilderRegistrationSteps" TagPrefix="CC" Src="~/controls/BuilderRegistrationSteps.ascx" %>
<CT:masterpage runat="server" id="CTMain">
    <CC:BuilderREgistrationSteps ID="ctlSteps" runat="server" RegistrationStep="5" />    
    <div class="pckggraywrpr">
        <div class="pckghdgred">Registration Complete</div>
        
    </div>
</CT:masterpage>
