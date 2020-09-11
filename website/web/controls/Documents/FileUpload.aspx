<%@ Page Language="VB" AutoEventWireup="false" CodeFile="FileUpload.aspx.vb" Inherits="controls_Documents_FileUpload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../includes/style.css" rel="stylesheet" />
    <script type="text/javascript" src="https://code.jquery.com/jquery-2.0.0.min.js"></script>
    <script type="text/javascript">
        var TotalFileCounter = 0;
        var SelectedFilesCounter = 0;

        var QueuedFiles;

        function OnFilesSelected(elem) {
            var IsValidated = true;

            var FileUploadLimit = $("#hdnFileUploadLimit").val();
            var TotalFilesSelected = elem.files.length;

            if (TotalFilesSelected > FileUploadLimit) {
                alert('Number of files selected exceeds the total number of files allowed!');
                IsValidated = false;
                return false;
            }

            for (var i = 0; i < TotalFilesSelected; i++) {
                var FileSizeLimit = $("#hdnFileSizeLimit").val();
                var FileSizeInBytes = elem.files[i].size;
                var FileSizeInKiloBytes = (FileSizeInBytes / 1024);

                if (FileSizeInKiloBytes > FileSizeLimit) {
                    alert('Size of selected file(s) exceeds the allowed quota!');
                    IsValidated = false;
                    return false;
                }

                var ValidFileTypes = $("#hdnFileTypes").val();
                var FileName = elem.files[i].name;
                var CurrFileType = "*." + FileName.substring(FileName.lastIndexOf('.') + 1, FileName.length);

                if (!ValidFileTypes.includes(CurrFileType)) {
                    alert('Uploading file(s) of type ' + CurrFileType + ' is not allowed!');
                    IsValidated = false;
                    return false;
                }
            }

            if (IsValidated) {
                SelectedFilesCounter = 0;

                QueuedFiles = elem.files;
                QueueFilesForUpload();

                //$("#ClearUpload").show();
            }

            return true;
        }

        function QueueFilesForUpload() {
            if (SelectedFilesCounter < QueuedFiles.length) {
                AsyncUploadFiles(TotalFileCounter, QueuedFiles[SelectedFilesCounter]);
            }
        }

        function ClearUploadList() {
            $("#log > li").each(function () {
                $(this).slideUp('fast', function () {
                    $(this).remove();
                    if ($("#log").children().length == 0) { $("#ClearUpload").fadeOut(200); }
                });
            });
        }

        function DeleteFile(FileIndex) {
            $("#log > li").each(function () {
                var dataFileIndex = $(this).data('file-index');
                if (dataFileIndex == FileIndex) {
                    $(this).slideUp('fast', function () {
                        $(this).remove();
                        if ($("#log").children().length == 0) { $("#ClearUpload").fadeOut(200); }
                    });
                }
            });

            return false;
        }

        function AsyncUploadFiles(i, PostedFile) {
            var FileSizeInBytes = PostedFile.size;
            var FileSizeInKiloBytes = Math.floor(FileSizeInBytes / 1024);

            var FileInfo = "<div data-file-index='" + i + "' style='float:left;font-size:13px;font-style:italic;padding-left:325px;'><span style='font-style:italic;'>" + PostedFile.name + "</span>" + " (" + FileSizeInKiloBytes + " KB)</div>";
            var DeleteIcon = "<div style='float:right;' data-file-index='" + i + "'><a href='' onclick='return DeleteFile(" + i + ");'><img src='/includes/swfupload/cancel.png' /></a></div>";

            var UploadUrl = $("#hdnUploadUrl").val();

            var data = new FormData();
            data.append('UploadedFile', PostedFile);

            $("#fileProgress").show();

            $.ajax({
                url: UploadUrl,     //'/controls/FileUploadHandler.ashx',
                type: 'POST',
                cache: false,
                contentType: false,
                processData: false,
                data: data,
                success: function (file) {
                    $("#fileProgress").hide();
                    $("#lblMessage").html("<b>" + file.name + "</b> has been uploaded.");
                    $("#log").append($("<li data-file-index='" + i + "' style='border:solid 1px;padding: 10px 5px 25px 5px;margin-top:10px;'>").html(FileInfo));

                    TotalFileCounter++;
                    SelectedFilesCounter++;
                    QueueFilesForUpload();
                },
                xhr: function () {
                    var fileXhr = $.ajaxSettings.xhr();
                    if (fileXhr.upload) {
                        fileXhr.upload.addEventListener("#fileProgress", function (e) {
                            if (e.lengthComputable) {
                                $("#fileProgress").attr({
                                    value: e.loaded,
                                    max: e.total
                                });
                            }
                        }, false);
                    }
                    return fileXhr;
                }
            });
        }
</script>
</head>
<body>
    <form id="frmUpload" runat="server" enctype="multipart/form-data">
        <div>
            <div style="padding-left:407px;">
                <input type="file" id="fluMultiFileUpload" name="fluMultiFileUpload" runat="server" multiple="multiple" onchange="OnFilesSelected(this);" style="display:none;" />
                <input type="button" id="btnUploadFile" runat="server" value="Upload" onclick="document.getElementById('fluMultiFileUpload').click();" style="height:30px;width:120px;background-color:#606060;border-color:#606060;color:whitesmoke;font-size:14px;border-style:groove;" />
            </div>
            <img src="/images/ajaxloading5.gif" alt="Loading..." id="MultiAssetLoading" style="display: none;" />
            <p id="queuestatus"></p>
            <progress id="fileProgress" style="display: none"></progress>
            <ul id="log" style="list-style-type:none;">
            </ul>
            <div style="padding-left:430px;">
                <input type="button" id="ClearUpload" class="btnblue" value="Clear Uploaded List" onclick="ClearUploadList();" style="display:none;" />
            </div>
        </div>
        <input type="hidden" id="hdnFileUploadLimit" value="" />
        <input type="hidden" id="hdnFileSizeLimit" value="" />
        <input type="hidden" id="hdnFileTypes" value="" />
        <input type="hidden" id="hdnUploadUrl" value="" />
    </form>
</body>
</html>
