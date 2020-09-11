<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MultiFileUpload.ascx.vb" Inherits="controls_MultiFileUpload" %>


<script type="text/javascript" src="/includes/swfupload/swfupload.js"></script>
<script type="text/javascript" src="/includes/jquery.swfupload.js"></script>
<script type="text/javascript">
    function toggleElements(flag) {
        if (typeof(toggleAdmin) == "function")
            toggleAdmin(flag);
        
        //loading icon.
        if (!flag) $("#MultiAssetLoading").show();
        else $("#MultiAssetLoading").hide();
    }
    
    function swfupload_clear() {
        $("#log > li[class=success]").each(function() {
            $(this).slideUp('fast', function() {
                $(this).remove();
                if ($("#log").children().length == 0) { $("#ClearUpload").fadeOut(500); }
                $("#resourcetagging").fadeOut(500);
            });
        });
    }

    $(function() {
      $('#swfupload-control').swfupload({
        upload_url: '<%=UploadUrl %>',
        file_post_name: '<%=FilePostName %>',
        file_size_limit: '<%=FileSizeLimit %>',
        file_types: '<%=FileTypes %>',
        file_types_description: '<%=FileTypesDescription %>',
        file_upload_limit: '<%=FileUploadLimit %>',
        flash_url: "/includes/swfupload/swfupload.swf",
        button_image_url: '<%=ButtonImageUrl %>',
        button_width: '<%=ButtonWidth %>',
        button_height: '<%=ButtonHeight %>',
        button_placeholder: $('#button')[0],
        debug: false
      })
	    .bind('fileQueued', function(event, file) {
	      var listitem = '<li id="' + file.id + '" >' +
			    '<div style="float: right; width: 85%;"><div style="padding-bottom:8px;"><em>' + file.name + '</em> (' + Math.round(file.size / 1024) + ' KB) <span class="progressvalue" ></span></div>' +
			    '<div class="progressbar" ><div class="progress" ></div></div>' +
			    '<p class="status">Pending</p>' +
			    '<span class="cancel">&nbsp;</span></div><div style="clear: both;"></div>' +
			    '</li>';
	      $('#log').append(listitem);
	      $('li#' + file.id + ' .cancel').bind('click', function() {
	        var swfu = $.swfupload.getInstance('#swfupload-control');
	        swfu.cancelUpload(file.id);
	        $('li#' + file.id).slideUp('fast', function() {
	          $(this).remove();
	          if ($("#log").children().length == 0) { $("#ClearUpload").fadeOut(500); }
	        });
	      });

	      $(this).swfupload('startUpload');
	    })
	    .bind('fileQueueError', function(event, file, errorCode, message) {
	      alert(message);
	    })
	    .bind('fileDialogComplete', function(event, numFilesSelected, numFilesQueued) {
	      if (numFilesSelected > 0 && numFilesQueued > 0) {
	        toggleElements(false);
	        $("#ClearUpload").fadeIn(500);
	      }
	    })
	    .bind('uploadStart', function(event, file) {
	      var currentItem = $('#log li#' + file.id);

	      var currentPosition = $('#log li').index(currentItem);
	      var currentHeight;

	      currentHeight = (currentPosition > 0) ? ($("#log li").filter(":gt(" + (currentPosition - 1) + ")").outerHeight(true) + 15) : 0;

	      var yPos = currentPosition * currentHeight;

	      $("#log").animate({ scrollTop: yPos }, 1000);
	      currentItem.find('p.status').text('Uploading...');
	      currentItem.find('span.progressvalue').text('[0%]');
	      //currentItem.find('span.cancel').hide();
	      if (typeof (UpdateUploadedFiles) == "function")
	        UpdateUploadedFiles(null, null, 1);
	    })
	    .bind('uploadProgress', function(event, file, bytesLoaded) {
	      var percentage = Math.round((bytesLoaded / file.size) * 100);
	      $('#log li#' + file.id).find('div.progress').css('width', percentage + '%');
	      $('#log li#' + file.id).find('span.progressvalue').text('[' + percentage + '%]');
	      if (percentage == 100)
	        $('#log li#' + file.id).find('p.status').text('Saving...');
	    })
	    .bind('uploadError', function(file, code, msg) {
	        if (msg != '-280')
	            alert(msg);
	         // alert('Error: The file "' + code.name + '" cannot be processed.  Please contact an administrator for more details. ');
	    })
	    .bind('uploadSuccess', function(event, file, serverData) {
	      var item = $('#log li#' + file.id);
	      if (serverData.toString().indexOf(";") < 0) {

	        item.find('p.status').html("<span class='uploadError'>Error.</span>");
	      } else {
	        var AssetId, ThumbURL, AssetName;

	        AssetId = serverData.toString().split(";")[0];
	        ThumbURL = serverData.toString().split(";")[1];
	        AssetName = serverData.toString().split(";")[2];

	        item.find('div.progress').css('width', '100%');
	        item.find('span.progressvalue').text('[100%]');
	        item.find('span.progressvalue').css("color", "black");

	        var pathtofile = '<div style="float:left; margin:15px;"><img src="' + ThumbURL + '" alt=""></div>';

	        item.addClass('success');
	        item.find('p.status').html("<span class='complete'>Complete.</span>");
	        item.prepend(pathtofile);

	        var swfu = $.swfupload.getInstance('#swfupload-control');
	        var stats = swfu.getStats();

	        //FileUploaded() is called when a single file is done. This is currently used with the Bulk Uploader.
	        if (typeof (FileUploaded) == "function")
	          FileUploaded(AssetId, 1, ThumbURL, AssetName, stats.files_queued);

	        //get rid of current item once we're done processing.
	        //item.slideUp(1000, function() { item.remove(); });
	      }
	    })
	    .bind('uploadComplete', function(event, file) {
	      var swfu = $.swfupload.getInstance('#swfupload-control');
	      var stats = swfu.getStats();
	      toggleElements(true);
	      $('#resourcetagging').fadeIn(500);

	      // upload has completed, try the next one in the queue
	      $(this).swfupload('startUpload');
	    })

    });	

</script>

<style type="text/css">
    #swfupload-control p{ margin:10px 5px; font-size:0.9em; }
    #log{ padding: 0; margin: 0 0 10px 0; width:auto; overflow: auto; max-height: 257px; }
    #log li{ list-style: none; margin:2px 2px 5px 2px; border:1px solid #7c7c7c; padding:10px; font-size:12px; 
        font-family:Arial, Helvetica, sans-serif; color:#333; background:#fff; position:relative; background: url(/images/admin/greyback.jpg) repeat-x;}
    #log li .progressbar{ border:1px solid #333; height:5px; background:#fff; }
    #log li .progress{ background:#999; width:0%; height:5px; }
    #log li p{ margin:0; line-height:18px; }
    #log li.success{ background: url(/images/admin/greyback.jpg) repeat-x; list-style: none; }
    #log li span.cancel{ position:absolute; top:5px; right:5px; width:20px; height:20px; 
    background:url('/includes/swfupload/cancel.png') no-repeat; cursor:pointer; }
    #log li .status {text-align:right;padding-top:10px;font-weight:bold;}
    #log li .progressvalue {margin-left:10px;}
    .complete {color: #00a908;font-size:1.1em;}
    .uploadError {color: #f00;font-size:1.1em;}
</style>
<div class="pckghdgblue" style="background:#e1e1e1">	
    Click on Upload to select the Documents you would like to upload:
</div>
	
<div id="swfupload-control">
    <p>Upload up to <%=FileUploadLimit%> <%=FileTypesDescription%> (<%=FileTypes.Replace(";", ", ")%>), each having a maximum size of <%=Math.Round((FileSizeLimit / 1024))%>MB.</p>
    <input type="button" id="button" /> <img src="/images/ajaxloading5.gif" alt="Loading..." id="MultiAssetLoading" style="display: none;" />
    <p id="queuestatus"></p>
    <ol id="log"></ol>
    <input type="button" id="ClearUpload" class="btnblue" Value="Clear Uploaded List" onclick="swfupload_clear();" style="display: none;" />
</div>
<br />