jQuery(function($) { 
	$('#cycleshow').cycle({ 
	    fx:     'fade', 
	    speed:  'normal', 
	    timeout: 0, 
	    pager:  '.cyclenav', 
	    pagerAnchorBuilder: function(idx, slide) { 
		// return selector string for existing anchor 
		return '.cyclenav li:eq(' + idx + ') a'; 
	    } 
	});
}); 

 
