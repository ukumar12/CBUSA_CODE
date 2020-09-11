(function()
{

	var flashFilenameRegex = /\.swf(?:$|\?)/i,
			numberRegex = /^\d+(?:\.\d+)?$/;

	
	function isFlashEmbed( element )
	{
		var attributes = element.attributes;

		return ( attributes.type == 'application/x-shockwave-flash' || flashFilenameRegex.test( attributes.src || '' ) );
	}

	function cssifyLength( length )
	{
		if ( numberRegex.test( length ) )
			return length + 'px';
		return length;
	}

	function createFakeElementYoutube( editor, realElement )
	{
		var fakeElement = editor.createFakeParserElement( realElement, 'cke_youtube', 'youtube', true ),
			fakeStyle = fakeElement.attributes.style || '';

		var width = realElement.attributes.width,
			height = realElement.attributes.height;

		if ( typeof width != 'undefined' )
			fakeStyle = fakeElement.attributes.style = fakeStyle + 'width:' + cssifyLength( width ) + ';';

		if ( typeof height != 'undefined' )
			fakeStyle = fakeElement.attributes.style = fakeStyle + 'height:' + cssifyLength( height ) + ';';

		return fakeElement;
	}

	CKEDITOR.plugins.add('youtube',
	{
	    init: function(editor)
	    {
		var pluginName = 'youtube';
		CKEDITOR.dialog.add(pluginName, this.path + 'dialogs/youtube.js');

		editor.addCss(
				'img.cke_youtube' +
				'{' +
					'background-image: url(' + CKEDITOR.getUrl( this.path + 'images/placeholder.png' ) + ');' +
					'background-position: center center;' +
					'background-repeat: no-repeat;' +
					'border: 1px solid #a9a9a9;' +
					'width: 80px;' +
					'height: 80px;' +
				'}'
				);

		editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));

		editor.on( 'doubleclick', function( evt )
				{
					var element = evt.data.element;
					if ( element.is( 'img' ) && element.getAttribute('datackerealelementtype') == 'youtube' )
						evt.data.dialog = 'youtube';
				});

		editor.ui.addButton('youtube',
		    {
			label: 'YouTube',
			command: pluginName,
			icon: CKEDITOR.plugins.getPath('youtube') + '/images/youtube.gif'
		    });
	    },
	    afterInit: function(editor)
	    {
			var dataProcessor = editor.dataProcessor,
			dataFilter = dataProcessor && dataProcessor.dataFilter;

			if ( dataFilter )
			{
				dataFilter.addRules(
					{
						elements :
						{
							'cke:embed' : function( element )
							{
								if ( !isFlashEmbed( element ) ) {
									return null;
								}
								
								return createFakeElementYoutube( editor, element );
							}
						}
					},
					5);
			}
	    },

		requires : [ 'fakeobjects' ]
	});

})();