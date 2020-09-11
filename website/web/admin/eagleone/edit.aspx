<%@ Page Language="VB" MasterPageFile="~/controls/AdminMaster.master" Title="Edit Eagle One Comparison" AutoEventWireup="false" CodeFile="edit.aspx.vb" Inherits="admin_eagleone_edit" ValidateRequest="false" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<script type="text/javascript">
    function CheckVendor(sender, args) {
        if (document.getElementById('ctl00_ph_lsVendors_onList').childNodes.length > 1) {
            args.IsValid = true;
        } else {
            args.IsValid = false;
        }
    }
</script>

<asp:ScriptManager ID="ajaxManager" runat="server"></asp:ScriptManager>
<input type="hidden" name="ACTION" />
<h4>Eagle One Comparison Administration</h4>
<p>
    <b>Add/Edit Eagle One Comparison</b>
    <br/>

    <table width="100%" border="0">
    	<tr><td colspan=2><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
        <tr style="position:relative;z-index:5">
            <td class="required">Select Takeoff:</td>
            <td class="field"><CC:SearchList ID="slTakeoff" runat="server" style="width:300px;" Table="Takeoff" TextField="Title" ValueField="TakeoffID" WhereClause="AdminID is not null" SearchFunction="GetEagleOneTakeoffList" ViewAllLength="20" CssClass="searchlist" AutoPostback="true" /></td>
            <td><asp:RequiredFieldValidator id="rfvslTakeoff" runat="server" ControlToValidate="slTakeoff" ErrorMessage="Please select a Takeoff"></asp:RequiredFieldValidator></td>
        </tr>
        <tr style="position:relative;z-index:4;">
            <td class="required">Select LLC:</td>
            <td class="field"><CC:SearchList ID="slLLC" runat="server" style="width:300px;" Table="LLC" TextField="LLC" ValueField="LLCID" CssClass="searchlist" ViewAllLength="20" AutoPostback="true" /></td>
            <td><asp:RequiredFieldValidator id="rfvslLLC" runat="server" ControlToValidate="slLLC" ErrorMessage="Please select LLC"></asp:RequiredFieldValidator></td>
        </tr>
        <tr style="position:relative;z-index:3;">
            <td class="required">Select Vendor(s):</td>
            <td class="field">
                <asp:UpdatePanel ID="upVendors" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <asp:Literal ID="ltlSelect" runat="server">Select Takeoff and LLC above to see available Vendors.</asp:Literal>
                        <CC:ListSelect ID="lsVendors" runat="server" Height="500px" Width="600px" AddImageUrl="/images/admin/true.gif" DeleteImageUrl="/images/global/icon-remove.gif" SelectLimit="5" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="slTakeoff" />
                        <asp:AsyncPostBackTrigger ControlID="slLLC" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <asp:CustomValidator ID="cvlsVendors" runat="server" ControlToValidate="" ClientValidationFunction="CheckVendor" ErrorMessage="Please select Vendor"></asp:CustomValidator>
            </td>
        </tr>
    </table>
    <p></p>
    <CC:OneClickButton ID="btnSave" runat="server" CssClass="btn" Text="Save" />
    <CC:OneClickButton ID="btnCancel" runat="server" CssClass="btn" Text="Cancel" />
    <%--<CC:OneClickButton ID="btnDelete" runat="server" CssClass="btn" Text="Delete" />--%>


</asp:content>