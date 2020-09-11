<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MultiFileUploadNew.ascx.vb" Inherits="controls_Documents_MultiFileUploadNew" %>
<script type="text/javascript">

    function SetiFrameProperties() {
        var ifrmUpload = document.getElementsByTagName("iframe")[0];

        ifrmUpload.onload = function () {
            var innerDoc = ifrmUpload.contentDocument || ifrmUpload.contentWindow.document;

            var ifrmHdnFileUploadLimit = innerDoc.getElementById("hdnFileUploadLimit");
            ifrmHdnFileUploadLimit.value = "<%=FileUploadLimit %>";

            var ifrmHdnFileSizeLimit = innerDoc.getElementById("hdnFileSizeLimit");
            ifrmHdnFileSizeLimit.value = "<%=FileSizeLimit %>";

            var ifrmHdnFileTypes = innerDoc.getElementById("hdnFileTypes");
            ifrmHdnFileTypes.value = "<%=FileTypes %>";

            var ifrmHdnUploadUrl = innerDoc.getElementById("hdnUploadUrl");
            ifrmHdnUploadUrl.value = "<%=UploadUrl %>";
        };
    }
    
    $(document).ready(function () {
        SetiFrameProperties();
    });

</script>
<link href="../../includes/font-awesome.min.css" rel="stylesheet" />
<div class="pckghdgblue" style="background:#e1e1e1">	
    Click on Upload to select the Documents you would like to upload:
</div>

<div id="fileupload-control">
    <p>Upload up to <%=FileUploadLimit%> <%=FileTypesDescription%> (<%=FileTypes.Replace(";", ", ")%>), each having a maximum size of <%=Math.Round((FileSizeLimit / 1024))%>MB.</p>
    <iframe id="ifrmUploadControl" runat="server" src="~/controls/Documents/FileUpload.aspx?ful=" style="width:900px;border:none;">

    </iframe>
</div>

<br />