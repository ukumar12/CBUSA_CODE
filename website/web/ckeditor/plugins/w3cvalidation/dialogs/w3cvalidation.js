(function(){
	
	var w3cvalidation = function(editor){
		return {
			title: 'W3C HTML Validation',
			minWidth : 500,
			minHeight : 400,
			contents: [{
				id: 'HTMLValidation',
				label: 'W3C HTML Validation',
				elements: [{
					type: 'html',
					id: 'body',
					html: '<div id="htmlvalidation">Validating...</div>'
				}]
			}],
			onShow: function(dialog, func) {
				$.ajax({
					url: "/ckeditor/plugins/w3cvalidation/dialogs/ajax.aspx?F=HtmlValidation&html=" + escape(editor.getData()),
					type: "GET",
					success: function(data) {
						$('#htmlvalidation').html(data.replace(/&nbsp;/gi,' ').replace('<p>','<p style="white-space: normal;"'));
					},
					error: function(a,b) {
						$('#htmlvalidation').html("<p style=\"white-space: normal\">An error occurred. If you are trying to validate a large amount of HTML, please copy the source and use the W3C's official validation service: <a href=\"http://validator.w3.org/#validate_by_input\" target=\"_blank\" style=\"color: #00c\">http://validator.w3.org/#validate_by_input</a></p>");
					}
				});
			},
			onHide: function(dialog, func) {
				$('#htmlvalidation').html('Validating, please wait...');
			}
		}
	}
	
	CKEDITOR.dialog.add('w3cvalidation', function(editor) {
		return w3cvalidation(editor);
	});
		
})();