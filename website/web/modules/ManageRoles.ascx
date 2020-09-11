<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ManageRoles.ascx.vb" Inherits="modules_ManageRoles" %>


<link rel="stylesheet" type="text/css" href="/includes/jquery-multiselect/jquery.multiselect.css" />
<link rel="stylesheet" href="/includes/jquery-multiselect/jquery-ui.css">
<script type="text/javascript" src="/includes/jquery-multiselect/jquery.js"></script>
<script type="text/javascript" src="/includes/jquery-multiselect/jquery-ui.min.js"></script>

<script type="text/javascript" src="/includes/jquery-multiselect/jquery.multiselect.js"></script>


<script type="text/javascript">
    function ClearResult(id) {
        $('#' + id).fadeOut('slow', null);
    }
    $(function () {

        var IsSelectionChanged = false;
        $('[id*=drpUsers]').multiselect({
            selectedList: 8,
            noneSelectedText: '',
            checkAll: function () {
                IsSelectionChanged = true;
            },
            uncheckAll: function () {
                IsSelectionChanged = true;
            },
            click: function (event, ui) {
                IsSelectionChanged = true;
            },
            close: function () {
                if (IsSelectionChanged) {
                    $('[id*=drpUsers]').prop('disabled', true);
                    webMethodUrl = "/forms/vendor-registration/users.aspx/UpdateAndSyncVendorRoles";
                    var that = $(this);
                    var RoleId = that.closest('tr').find('[id*=hdnRoleId]').val();
                    var vendorId = ($(that).val() == 'null' || $(that).val() == null) ? '' : $(that).val();
                    var spanResult = that.closest('tr').find('[id*=spanResult]');
                    // var data = { 'RoleId': RoleId, 'AccountIDs': '' + $(this).val() + '' }
                    var params = "{'RoleId': '" + RoleId + "','AccountIDs': '" + vendorId + "'}";
                    $(spanResult).fadeIn('slow', null);
                    $(spanResult).text('Please wait...');
                    $.ajax({
                        type: "POST",
                        url: webMethodUrl,
                        async: true,
                        data: params,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (msg) {
                            if (msg.d == 'true') {
                                $(spanResult).text('Update Saved.');
                                setTimeout(function () {
                                    $(spanResult).text('')
                                    $(spanResult).fadeOut('slow', null);
                                    $('[id*=drpUsers]').prop('disabled', false);
                                }, 1500);
                            } else {

                                $(spanResult).text('An error occurred. Please contact support.');
                                setTimeout(function () {
                                    $(spanResult).text('')
                                    $(spanResult).fadeOut('slow', null);
                                    $('[id*=drpUsers]').prop('disabled', false);
                                }, 2000);
                            }
                            IsSelectionChanged = false;
                           
                        },
                        error: function () {
                            IsSelectionChanged = false;
                            $('[id*=drpUsers]').prop('disabled', false);
                            alert("fail");
                        }
                    });
                }
            },
        });
    });
</script>

<div class="pckgltgraywrpr" style="margin: 0px">
    <div class="pckghdgltblue">
        Manage Account Roles
    </div>
    <table class="tblcompr largest" style="width: 100%; margin: 0px">
        <tr>
            <th>Role</th>
            <th>User Account</th>
        </tr>
        <asp:Repeater ID="rptRoles" runat="server">
            <ItemTemplate>
                <tr style="position: relative; z-index: <%#10 - Container.ItemIndex %>;">
                    <td><%#DataBinder.Eval(Container.DataItem, "VendorRole")%>
                        <asp:HiddenField ID="hdnRoleId" runat="server" />
                    </td>
                    <td>
                       
                                <span id="spanResult" runat="server" class="smaller red">
                                    <asp:Literal ID="ltlSaved" runat="server"></asp:Literal></span>
                                <%--<CC:SearchList ID="slUsers" runat="server" SearchFunction="GetVendorUsers" Table="VendorAccount" TextField="Username" ValueField="VendorAccountId" ViewAllLength="10" style="width:200px;" CssClass="searchlist" AutoPostback="true" />    --%>
                                <div>
                                    <%--<asp:DropDownList runat="server" ID="test" AutoPostBack="false" >
                                        <asp:ListItem Text="test sdfsdf" Value="testsdfdfsd"></asp:ListItem>
                                        </asp:DropDownList>--%>

                                    <asp:ListBox ID="drpUsers" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                </div>
                                <asp:HiddenField ID="hdnPrevVendorAccountId" runat="server" />
                    
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
</div>

<style>
    .ui-multiselect-menu {
        z-index: 99999 !important;
    }
</style>



