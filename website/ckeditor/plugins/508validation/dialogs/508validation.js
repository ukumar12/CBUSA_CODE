(function(){
	
	var _508validation = function(editor){
		return {
			title: '508 Validation',
			minWidth : 500,
			minHeight : 300,
			contents: [{
				id: '508validation',
				label: '508 Validation',
				elements: [{
					type: 'html',
					id: 'body',
					html: '<div id="508validation" style="width: 495px; height: 280px; overflow-y: auto; overflow-x: hidden; white-space: normal;">Validating, please wait...</div>'
				}]
			}],
			onShow: function(dialog, func) {
				$.ajax({
					url: "/ckeditor/plugins/508validation/dialogs/ajax.aspx?F=508Validation&html=" + escape(editor.getData()),
					type: "GET",
					success: function(data) {
						$('#508validation').html(data.replace(/&nbsp;/gi,' ').replace('<p>','<p style="white-space: normal;"').replace('</div>','</p>'));
					},
					error: function(a,b) {
						$('#508validation').html("<p style=\"white-space: normal\">An error occurred. If you are trying to validate a large amount of HTML, please consider splitting the contnet up into smaller portions and check them separately.</p>");
					}
				});
			},
			onHide: function(dialog, func) {
				$('#508validation').text('Validating, please wait...');
			}
		}
	}
	
	CKEDITOR.dialog.add('508validation', function(editor) {
		return _508validation(editor);
	});
		
})();