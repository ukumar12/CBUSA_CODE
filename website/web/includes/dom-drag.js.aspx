<%@ Page Language="VB" AutoEventWireup="false" CodeFile="dom-drag.js.aspx.vb" Inherits="domdrag" %>


/**************************************************
 * dom-drag.js
 * 09.25.2001
 * www.youngpup.net
 **************************************************
 * 10.28.2001 - fixed minor bug where events
 * sometimes fired off the handle, not the root.
 **************************************************/

var Drag = {

	obj : null,

	init : function(o, oRoot, minX, maxX, minY, maxY, bSwapHorzRef, bSwapVertRef, fXMapper, fYMapper)
	{
		o.onmousedown	= Drag.start;

		o.hmode			= bSwapHorzRef ? false : true ;
		o.vmode			= bSwapVertRef ? false : true ;

		o.root = oRoot && oRoot != null ? oRoot : o ;

		if (o.hmode  && isNaN(parseInt(o.root.style.left  ))) o.root.style.left   = "400px";
		if (o.vmode  && isNaN(parseInt(o.root.style.top   ))) o.root.style.top    = "100px";
		if (!o.hmode && isNaN(parseInt(o.root.style.right ))) o.root.style.right  = "0px";
		if (!o.vmode && isNaN(parseInt(o.root.style.bottom))) o.root.style.bottom = "0px";

		o.minX	= typeof minX != 'undefined' ? minX : null;
		o.minY	= typeof minY != 'undefined' ? minY : null;
		o.maxX	= typeof maxX != 'undefined' ? maxX : null;
		o.maxY	= typeof maxY != 'undefined' ? maxY : null;

		o.xMapper = fXMapper ? fXMapper : null;
		o.yMapper = fYMapper ? fYMapper : null;

		o.root.onDragStart	= new Function();
		o.root.onDragEnd	= new Function();
		o.root.onDrag		= new Function();
	},

	start : function(e)
	{
		var o = Drag.obj = this;
		e = Drag.fixE(e);
		var y = parseInt(o.vmode ? o.root.style.top  : o.root.style.bottom);
		var x = parseInt(o.hmode ? o.root.style.left : o.root.style.right );
		o.root.onDragStart(x, y);

		o.lastMouseX	= e.clientX;
		o.lastMouseY	= e.clientY;

		if (o.hmode) {
			if (o.minX != null)	o.minMouseX	= e.clientX - x + o.minX;
			if (o.maxX != null)	o.maxMouseX	= o.minMouseX + o.maxX - o.minX;
		} else {
			if (o.minX != null) o.maxMouseX = -o.minX + e.clientX + x;
			if (o.maxX != null) o.minMouseX = -o.maxX + e.clientX + x;
		}

		if (o.vmode) {
			if (o.minY != null)	o.minMouseY	= e.clientY - y + o.minY;
			if (o.maxY != null)	o.maxMouseY	= o.minMouseY + o.maxY - o.minY;
		} else {
			if (o.minY != null) o.maxMouseY = -o.minY + e.clientY + y;
			if (o.maxY != null) o.minMouseY = -o.maxY + e.clientY + y;
		}

		document.onmousemove	= Drag.drag;
		document.onmouseup		= Drag.end;

		return false;
	},

	drag : function(e)
	{
		e = Drag.fixE(e);
		var o = Drag.obj;

		var ey	= e.clientY;
		var ex	= e.clientX;
		var y = parseInt(o.vmode ? o.root.style.top  : o.root.style.bottom);
		var x = parseInt(o.hmode ? o.root.style.left : o.root.style.right );
		var nx, ny;

		if (o.minX != null) ex = o.hmode ? Math.max(ex, o.minMouseX) : Math.min(ex, o.maxMouseX);
		if (o.maxX != null) ex = o.hmode ? Math.min(ex, o.maxMouseX) : Math.max(ex, o.minMouseX);
		if (o.minY != null) ey = o.vmode ? Math.max(ey, o.minMouseY) : Math.min(ey, o.maxMouseY);
		if (o.maxY != null) ey = o.vmode ? Math.min(ey, o.maxMouseY) : Math.max(ey, o.minMouseY);

		nx = x + ((ex - o.lastMouseX) * (o.hmode ? 1 : -1));
		ny = y + ((ey - o.lastMouseY) * (o.vmode ? 1 : -1));

		if (o.xMapper)		nx = o.xMapper(y)
		else if (o.yMapper)	ny = o.yMapper(x)

		Drag.obj.root.style[o.hmode ? "left" : "right"] = nx + "px";
		Drag.obj.root.style[o.vmode ? "top" : "bottom"] = ny + "px";
		Drag.obj.lastMouseX	= ex;
		Drag.obj.lastMouseY	= ey;

		Drag.obj.root.onDrag(nx, ny);
		return false;
	},

	end : function()
	{
		document.onmousemove = null;
		document.onmouseup   = null;
		Drag.obj.root.onDragEnd(	parseInt(Drag.obj.root.style[Drag.obj.hmode ? "left" : "right"]),
									parseInt(Drag.obj.root.style[Drag.obj.vmode ? "top" : "bottom"]));
		Drag.obj = null;
	},

	fixE : function(e)
	{
		if (typeof e == 'undefined') e = window.event;
		if (typeof e.layerX == 'undefined') e.layerX = e.offsetX;
		if (typeof e.layerY == 'undefined') e.layerY = e.offsetY;
		return e;
	}
};

function showQW(ctrlName) {

	var qwdrag = document.getElementById("panelroot");
	var qwcustom = document.getElementById("qwcustom");
	var panhandle = document.getElementById("panelhandle");
	var txt = document.getElementById(ctrlName).innerHTML;
	
	qwcustom.innerHTML = txt;
	
	var img = document.getElementById('imgEnlargedImage');
	var tempX, tempY; 
	if (img.width!=0){
	    document.getElementById('imgHeader').style.width=''+img.width-50+'px';
	    //qwdrag.style.width='800px';
	    panhandle.style.width=img.width+200+'px';
	    if (document.all?true:false) {
	        tempX=(screen.width-img.width)/2; 
	        tempY=75; 
	    } else {
	        tempX=(screen.width-img.width)/2; 
	        tempY=75; 
	    }
	}	else   {
	    tempX=75;
	    tempY=75;
	}
	qwdrag.style.left=tempX + 'px';
	qwdrag.style.top=tempY+ getScrollY() + 'px';
	qwdrag.style.left='25px';
	qwdrag.style.top= 25 + getScrollY() + 'px';
	showLayer('panelroot')

    var ie=document.all && !window.opera;
    var veil = document.getElementById("divVeil");
	var docwidth=(ie)? document.body.clientWidth : window.innerWidth;
	        
	veil.style.width=docwidth+"px"; //set up veil over page
    veil.style.bottom="0px"; //set up veil over page
	showLayer('divVeil')
}

function getScrollY() {
    var scrollY = 0;
    
    if( document.documentElement && document.documentElement.scrollTop ) {
        scrollY = document.documentElement.scrollTop;
    }
    else if( document.body && document.body.scrollTop ) {
        scrollY = document.body.scrollTop;
    }
    else if( window.pageYOffset ) {
        scrollY = window.pageYOffset;
    }
    else if( window.scrollY ) {
        scrollY = window.scrollY;
    }       
    return scrollY;
}

function hideQW() {
   hideLayer('divVeil');
   hideLayer('panelroot');  
}

document.write('<div id="panelroot" class="screen" style="visibility:hidden; top:100px; left:150px; text-align:center; padding:8px 8px 8px 6px; position:absolute; z-index:10001;">');
document.write('<div class="bdr" style="height:100%; background-color:#ffffff; text-align:right">');
document.write('<div id="panelhandle" style="width:340px; height:33px; position:relative; text-align:right">');
document.write('<div style="top:0; left:0; position:absolute; cursor:move;"><img src="/images/spacer.gif" width="340" id="imgHeader" height="33" style="border-style:none;" alt="" /><br /></div>');
document.write('<div style="top:7px; right:8px; position:absolute; cursor:pointer;  float:right" id="CloseHeader" onclick="hideQW();">Close<br /></div>');
document.write('</div>');
document.write('<div id="qwcustom" style="padding:6px 10px 6px 10px;text-align:center"></div>');
document.write('<!--[if lt IE 7]><iframe src="javascript:false;" class="slcthide" style="width:342px;text-align:center"></iframe><![endif]-->');
document.write('</div>');
document.write('</div>');
document.write('<div id="divVeil" class="Veil" style="width:100%;visibility:hidden;z-index:10000;">&nbsp;</div>');

var theHandle = document.getElementById("panelhandle");
var theRoot   = document.getElementById("panelroot");
Drag.init(theHandle, theRoot);