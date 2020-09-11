(function(){
	
	var ATTRTYPE_EMBED = 4;

	var attributesMap =
	{
		pluginspage : [ { type : ATTRTYPE_EMBED, name : 'pluginspage' } ],
		src : [ { type : ATTRTYPE_EMBED, name : 'src' } ],
		'class' : [ { type : ATTRTYPE_EMBED, name : 'class'} ],
		width : [ { type : ATTRTYPE_EMBED, name : 'width' } ],
		height : [ { type : ATTRTYPE_EMBED, name : 'height' } ],
		style : [ { type : ATTRTYPE_EMBED, name : 'style' } ],
	};
	
	var commitValue = function( data )
	{
		var id = this.id;
		if ( !data.info )
			data.info = {};
		data.info[id] = this.getValue();
	};
	
	function loadValue(embedNode) {
		var attributes = attributesMap[ this.id ];
		if (!attributes)
			return;
		
		var isRemove = ( this.getValue() === '' );
		for ( var i = 0 ; i < attributes.length ; i++ )
		{
			var attrDef = attributes[i];
			switch ( attrDef.type )
			{
				case ATTRTYPE_EMBED:
					if ( !embedNode )
						continue;

					if ( embedNode.getAttribute( attrDef.name ) )
					{
						value = embedNode.getAttribute( attrDef.name );
						this.setValue( value );
						return;
					}
			}
		}
	}
	
	var youtube = function(editor){
		return {
			title: 'Embed YouTube Video',
			minWidth : 500,
			minHeight : 75,
			contents: [{
				id: 'youtube',
				label: 'Embed YouTube Video',
				elements: [
				{
					type: 'text',
					id: 'txtURL',
					label: 'Video Code (http://www.youtube.com/watch?v=<b>&lt;code is here&gt;</b>)',
					labelLayout: 'vertical',
					commit : commitValue,
					setup: function(embedNode, fakeImage) {
						loadValue.apply( this, arguments );
						if ( fakeImage || embedNode )
						{
							var fakeImageSrc = embedNode.getAttribute('src');
							this.setValue(fakeImageSrc.replace('http://www.youtube.com/v/',''));
						}
					}
				},
				{
					type: 'hbox',
					widths: [ '20%', '20%', '60%' ],
					children: [
					{
						type: 'text',
						id: 'txtWidth',
						label: 'Width: ',
						width: '55px',
						validate : CKEDITOR.dialog.validate.integer( editor.lang.common.invalidWidth ),
						labelLayout: 'horizontal',
						'default': '480',
						commit : commitValue,
						setup: function(embedNode, fakeImage) {
							loadValue.apply( this, arguments );
							if ( fakeImage )
							{
								var fakeImageWidth = parseInt( fakeImage.$.style.width, 10 );
								if ( !isNaN( fakeImageWidth ) )
									this.setValue( fakeImageWidth );
							}
						}
					},
					{
						type: 'text',
						id: 'txtHeight',
						label: 'Height: ',
						width: '55px',
						validate : CKEDITOR.dialog.validate.integer( editor.lang.common.invalidHeight ),
						'default': '390',
						labelLayout: 'horizontal',
						commit : commitValue,
						setup: function(embedNode, fakeImage) {
							loadValue.apply( this, arguments );
							if ( fakeImage )
							{
								var fakeImageHeight = parseInt( fakeImage.$.style.height, 10 );
								if ( !isNaN( fakeImageHeight ) )
									this.setValue( fakeImageHeight );
							}
						}
					},
					{ 
						type: 'html',
						id: 'ph',
						html: '<div></div>'
					}
					]
				}
				
				]
			}],
			onShow: function(dialog, func) {
				// Clear previously saved elements.
				this.fakeImage = this.objectNode = this.embedNode = null;

				// Try to detect any embed or object tag that has Flash parameters.
				var fakeImage = this.getSelectedElement();
				
				if ( fakeImage && fakeImage.getAttribute('datackerealelementtype') && fakeImage.getAttribute('datackerealelementtype') == 'youtube' )
				{
					this.fakeImage = fakeImage;

					var realElement = editor.restoreRealElement(fakeImage),embedNode = null;
					
					embedNode = realElement;
					this.embedNode = embedNode;

					this.setupContent(embedNode, fakeImage);
				}
			},
			onOk: function(dialog, func) {
				var data = {};
				
				this.commitContent( data, null );
				
				//var youtubeNode = CKEDITOR.dom.element.createFromHtml('<cke:embed height="' + data.info.txtHeight + '" width="' + data.info.txtWidth + '" type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" src="http://www.youtube.com/v/' + data.info.txtURL + '"></cke:embed>');
				
				embedNode = CKEDITOR.dom.element.createFromHtml('<cke:embed></cke:embed>', editor.document);
				embedNode.setAttributes(
					{
						type: 'application/x-shockwave-flash',
						pluginspage: 'http://www.macromedia.com/go/getflashplayer',
						src: 'http://www.youtube.com/v/' + data.info.txtURL,
						width: data.info.txtWidth,
						height: data.info.txtHeight,
						'youtube': 'true'
					});
				
			
				// Refresh the fake image.
				var newFakeImage = editor.createFakeElement(embedNode, 'cke_youtube', 'youtube', true);
				
				newFakeImage.setAttributes({ width: data.info.txtWidth, height: data.info.txtHeight, 'youtube': 'true'});
				
				editor.insertElement( newFakeImage );
					
				
			},
			buttons: [CKEDITOR.dialog.okButton, CKEDITOR.dialog.cancelButton]
		}
	}
	
	CKEDITOR.dialog.add('youtube', function(editor) {
		return youtube(editor);
	});
		
})();