<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Contract Central" CodeFile="Default.aspx.vb" Inherits="Index" %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<script type="text/javascript" language="javascript">
    $(document).ready(function() {
        window.addEventListener('message', function (event) {
            if (event.data.viz == "Dis") {
                var pageLoaded = event.data.loaded;

                if (pageLoaded == 'true') {
                    $('#divNCPLoader').hide();
                    document.getElementById("ph_ddlLLC").disabled = false;

                    alert("Please select a market!");
                }
            }
        }, false);
    });

    function CheckLLCSelection() {
        var selLLC = document.getElementById("ph_ddlLLC").value;

        if (selLLC == "") {
            document.getElementById("divNCPContractCentral").style.display = "none";
        } else {
            document.getElementById("divNCPContractCentral").style.display = "block";

            var ifrmNCP = document.getElementById("ifrmNCPContractCentral");
            ifrmNCP.contentWindow.postMessage({
                'llc': selLLC,
                'viz': 'Dis',
                'location': window.location.href
            }, "*");
        }
    }
</script>

<div style="display:inline-block;margin-bottom:25px;">
    <span style="color:#475163;">Select Market:</span>
    <asp:DropDownList ID="ddlLLC" runat="server" style="width:200px;" onchange="CheckLLCSelection();" Enabled="false"></asp:DropDownList>
</div>

<div>
    <div id="divNCPContractCentral" class="stacktblwrpr themeprice" style="display:none;">
		<iframe id="ifrmNCPContractCentral" src="https://cbusa-cbusa-ncp-dev.azurewebsites.net/CbusaBuilder/ContractCentral/LLCIndex" height="550" width="1190" scrolling="yes" style="border:none;overflow-y:scroll;overflow-x:hidden;"></iframe>
    </div>
    <div id="divNCPLoader" class="stacktblwrpr themeprice">
	    <div style="background-color:#fff;height:80px;text-align:center;">
            <img src="/images/loading.gif" alt="Loading..." /><br /><br />
            <h1 class="largest">Loading NCP Contract Central... Please Wait</h1>
        </div>
    </div>
</div>
</asp:content>
