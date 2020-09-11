/*****************************************************
 * ypSlideOutMenu
 * http://ypslideoutmenu.sourceforge.net/
 * 3/04/2001
 *
 * Licensed under AFL 2.0
 * http://www.opensource.org/licenses/afl-2.0.php
 *
 * Revised: 
 * - 08/29/2002 : added .hideAll()
 * - 04/15/2004 : added .writeCSS() to support more 
 *                than 30 Menus.
 *
 * --youngpup--
 *****************************************************/

ypSlideOutCart.Registry = []
ypSlideOutCart.aniLen = 800
ypSlideOutCart.hideDelay = 750
ypSlideOutCart.minCPUResolution = 10
ypSlideOutCart.hideInitialDelay = 3000

// constructor
function ypSlideOutCart(id, dir, left, top, width, height, persist)
{
	this.ie  = document.all ? 1 : 0
	this.ns4 = document.layers ? 1 : 0
	this.dom = document.getElementById ? 1 : 0
	this.css = "";

	if (this.ie || this.ns4 || this.dom) {
		this.id			 = id
		this.dir		 = dir
		this.orientation = dir == "left" || dir == "right" ? "h" : "v"
		this.dirType	 = dir == "right" || dir == "down" ? "-" : "+"
		this.dim		 = this.orientation == "h" ? width : height
		this.hideTimer	 = true
		this.aniTimer	 = false
		this.open		 = false
		this.over		 = false
		this.startTime	 = 0
		this.persist	 = persist

		// global reference to this object
		this.gRef = "ypSlideOutCart_"+id
		eval(this.gRef+"=this")

		// add this Cart object to an internal list of all Carts
		ypSlideOutCart.Registry[id] = this

		var d = document

		var strCSS = "";
		strCSS += '#' + this.id + 'Container { visibility:hidden; '
		strCSS += 'left:' + left + 'px; '
		strCSS += 'top:' + top + 'px; '
		strCSS += 'overflow:hidden; z-index:1201; }'
		strCSS += '#' + this.id + 'Container, #' + this.id + 'Content { position:absolute; '
		strCSS += 'width:' + width + 'px; '
		strCSS += 'height:' + height + 'px; '
		strCSS += 'clip:rect(0 ' + width + ' ' + height + ' 0); '
		strCSS += '}'

		this.css = strCSS;

		this.load()
	}
}

ypSlideOutCart.writeCSS = function() {
	document.writeln('<style type="text/css">');

	for (var id in ypSlideOutCart.Registry) {
		document.writeln(ypSlideOutCart.Registry[id].css);
	}

	document.writeln('</style>');
}

ypSlideOutCart.prototype.load = function() {
	var d = document
	var lyrId1 = this.id + "Container"
	var lyrId2 = this.id + "Content"
	var obj1 = this.dom ? d.getElementById(lyrId1) : this.ie ? d.all[lyrId1] : d.layers[lyrId1]
	if (obj1) var obj2 = this.ns4 ? obj1.layers[lyrId2] : this.ie ? d.all[lyrId2] : d.getElementById(lyrId2)
	var temp

	if (!obj1 || !obj2) window.setTimeout(this.gRef + ".load()", 100)
	else {
		this.container	= obj1
		this.Cart	= obj2
		this.style	= this.ns4 ? this.Cart : this.Cart.style
		this.homePos	= eval("0" + this.dirType + this.dim)
		this.outPos	= 0
		this.accelConst	= (this.outPos - this.homePos) / ypSlideOutCart.aniLen / ypSlideOutCart.aniLen 
		
		// set event handlers.
		if(!this.persist) {
			if (this.ns4) this.Cart.captureEvents(Event.MOUSEOVER | Event.MOUSEOUT);
			this.Cart.onmouseover = new Function("ypSlideOutCart.showCart('" + this.id + "')")
			this.Cart.onmouseout = new Function("ypSlideOutCart.hideCart('" + this.id + "')")
		}

		//set initial state
		this.endSlide()
	}
}
	
ypSlideOutCart.showCart = function(id)
{
	var reg = ypSlideOutCart.Registry
	var obj = ypSlideOutCart.Registry[id]
	
	if (obj.container) {
		obj.over = true

		// close other Carts.
		for (Cart in reg) if (id != Cart) ypSlideOutCart.hide(Cart)

		// if this Cart is scheduled to close, cancel it.
		if (obj.hideTimer) { reg[id].hideTimer = window.clearTimeout(reg[id].hideTimer) }

		// if this Cart is closed, open it.
		if (!obj.open && !obj.aniTimer) reg[id].startSlide(true)

		obj.Cart.onmouseover = new Function("ypSlideOutCart.showCart('" + id + "')")
	}
}

ypSlideOutCart.hideCart = function(id)
{
	// schedules the Cart to close after <hideDelay> ms, which
	// gives the user time to cancel the action if they accidentally moused out
	
	var obj = ypSlideOutCart.Registry[id]
	if (obj.container) {
		if (obj.hideTimer) window.clearTimeout(obj.hideTimer)
		obj.hideTimer = window.setTimeout("ypSlideOutCart.hide('" + id + "')", ypSlideOutCart.hideDelay);
	}
}


ypSlideOutCart.hideAll = function()
{
	var reg = ypSlideOutCart.Registry
	for (Cart in reg) {
		ypSlideOutCart.hide(Cart);
		if (Cart.hideTimer) window.clearTimeout(Cart.hideTimer);
	}
}


ypSlideOutCart.hideCartInitial = function(id)
{
	// schedules the menu to close after <hideDelay> ms, which
	// gives the user time to cancel the action if they accidentally moused out
	var obj = ypSlideOutCart.Registry[id]
	if (obj.container) {
		if (obj.hideTimer) window.clearTimeout(obj.hideTimer)
		obj.hideTimer = window.setTimeout("ypSlideOutCart.hide('" + id + "')", ypSlideOutCart.hideInitialDelay);
	}
}


ypSlideOutCart.hide = function(id)

{
	var obj = ypSlideOutCart.Registry[id]
	obj.over = false

	if (obj.hideTimer) window.clearTimeout(obj.hideTimer)
	
	// flag that this scheduled event has occured.
	obj.hideTimer = 0

	// if this Cart is open, close it.
	if (obj.open && !obj.aniTimer) obj.startSlide(false)
	
	obj.Cart.onmouseover = "";
}

ypSlideOutCart.prototype.startSlide = function(open) {
	this[open ? "onactivate" : "ondeactivate"]()
	this.open = open
	if (open) this.setVisibility(true)
	this.startTime = (new Date()).getTime()	
	this.aniTimer = window.setInterval(this.gRef + ".slide()", ypSlideOutCart.minCPUResolution)
}

ypSlideOutCart.prototype.slide = function() {
	var elapsed = (new Date()).getTime() - this.startTime
	if (elapsed > ypSlideOutCart.aniLen) { 
		this.endSlide() 
		if (!this.persist) ypSlideOutCart.hideCartInitial(this.id);
	}
	else {
		var d = Math.round(Math.pow(ypSlideOutCart.aniLen-elapsed, 2) * this.accelConst)
		if (this.open && this.dirType == "-")		d = -d
		else if (this.open && this.dirType == "+")	d = -d
		else if (!this.open && this.dirType == "-")	d = -this.dim + d
		else						d = this.dim + d

		this.moveTo(d)
	}
}

ypSlideOutCart.prototype.endSlide = function() {
	this.aniTimer = window.clearTimeout(this.aniTimer)
	this.moveTo(this.open ? this.outPos : this.homePos)
	if (!this.open) this.setVisibility(false)
	if ((this.open && !this.over) || (!this.open && this.over)) {
		this.startSlide(this.over)
	}
}

ypSlideOutCart.prototype.setVisibility = function(bShow) { 
	var s = this.ns4 ? this.container : this.container.style
	s.visibility = bShow ? "visible" : "hidden"
}
ypSlideOutCart.prototype.moveTo = function(p) { 
	this.style[this.orientation == "h" ? "left" : "top"] = this.ns4 ? p : p + "px"
}
ypSlideOutCart.prototype.getPos = function(c) {
	return parseInt(this.style[c])
}

// events

ypSlideOutCart.prototype.onactivate	= function() {  }
ypSlideOutCart.prototype.ondeactivate	= function() {  }
