$(document).ready(function() {

	$('#slideshow').cycle({
		fx:      'fade',
		timeout:  0,
		speed:    250,
		pager:   '#navCycle',
		display_back_and_forward: true,
		pagerAnchorBuilder: function(idx, slide) {
		        return ('<li><img src="' + slide.src + '" width="75" height="75" style="cursor:pointer;" /></li>');
    	}
	});


	$('#navCycle').jcarousel({
		scroll: 5,
		visible: 5,
		wrap:'circular',
		animation:400
	});
	
	$('.togglelinks').click(function() {
		 $(this).children('ul').slideToggle();
		
	});
	
	$('.togglelinks').click(function(event) {
        event.stopPropagation();
    });
	 
	
}); 
