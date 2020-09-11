$(window).load(function() {

// CYCLE //

	var cCarouselObj;
	var jCounter;
	var jCurrentSlide=0;
	var jSlideCount=0;

	function onAfter(curr,next,opts) {
		jSlideCount = opts.slideCount;
		jCounter = 'Image ' + (opts.currSlide + 1) + ' of ' + jSlideCount;
		$('#jCounter').html(jCounter);
		jCurrentSlide = opts.currSlide;
		$('#navCycle').trigger('click');
		$('.sGalleryCaption').html(this.name);
	}

	function jAnchorBuilder(idx, slide) {
		return ('<li><img src="' + slide.src + '" width="50" height="50" style="cursor:pointer;" /></li>');
	}

	$('#slideshow').cycle({
		fx:      'fade',
		timeout: 0,
		speed:   300,
		delay:   70000,
		timeout: 5000,
		pause:   1,
		prev:    '#prev',
		next:    '#next',
		pager:   '#navCycle',
		display_back_and_forward: true,
		before: function() {
			$(this).css({left:'50%', marginLeft:-$(this).width()/2});
			$(this).css({top:'50%', marginTop:-$(this).height()/2});
		},
		after:onAfter,
		pagerAnchorBuilder: jAnchorBuilder
	});

// CAROUSEL //

	function mycarousel_initCallback(carousel) {

		cCarouselObj = carousel;

		$('#prev').bind('click', function() {
			jCurrentSlide--;
			$('#slideshow').cycle('pause');
			return false;
		});

		$('#next').bind('click', function() {
			jCurrentSlide++;
			$('#slideshow').cycle('pause');
			return false;
		});

		// Disable autoscrolling if the user clicks the prev or next button.
		cCarouselObj.buttonNext.bind('click', function() {
			cCarouselObj.startAuto(0);
			$('#slideshow').cycle('pause');
		});

		cCarouselObj.buttonPrev.bind('click', function() {
			cCarouselObj.startAuto(0);
			$('#slideshow').cycle('pause');
		});

		// Pause autoscrolling if the user moves with the cursor over the clip.

		carousel.clip.hover(function() {
			cCarouselObj.startAuto(0);
			$('#slideshow').cycle('pause');
			});

		$('#navCycle').bind('click', function() {
			jCurrentSlide++;
			cCarouselObj.scroll(jCurrentSlide-3);
			return false;
		});



	};

	$('#navCycle').jcarousel({
		scroll: 7,
		visible: 7,
		wrap: 'both',
		animation: 300,
        	buttonPrevHTML: '<div id="prevJCarousel">&nbsp;</div>',
        	buttonNextHTML: '<div id="nextJCarousel">&nbsp;</div>',
		buttonPrevEvent: 'click',
		buttonNextEvent: 'click',
		initCallback: mycarousel_initCallback
	});

	$('#sGallery').css('visibility','visible');

// THIS FUNCTION CENTERS MAIN IMAGES WITHIN THE CYCLE //

//	$('#slideshow img').each(function() {
//		$(this).css({left: '50%', marginLeft: -$(this).width()/2});
//	});


});